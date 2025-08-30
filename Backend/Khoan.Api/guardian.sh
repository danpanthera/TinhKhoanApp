#!/bin/bash

# 🛡️ Khoan Project Guardian - Keeps backend/frontend running continuously
# This script monitors and restarts services if they die

PROJECT_NAME="KhoanApp"
BACKEND_DIR="/opt/Projects/Khoan/Backend/KhoanApp.Api"
FRONTEND_DIR="/opt/Projects/Khoan/Frontend/KhoanUI"
GUARDIAN_LOG="guardian.log"
GUARDIAN_MAX_LOG_SIZE=${GUARDIAN_MAX_LOG_SIZE:-2M}
GUARDIAN_LOG_BACKUPS=${GUARDIAN_LOG_BACKUPS:-4}

g_parse_size_bytes() {
    local size=$1
    if [[ $size =~ ^([0-9]+)[Kk]$ ]]; then
        echo $(( BASH_REMATCH[1] * 1024 ))
    elif [[ $size =~ ^([0-9]+)[Mm]$ ]]; then
        echo $(( BASH_REMATCH[1] * 1024 * 1024 ))
    elif [[ $size =~ ^[0-9]+$ ]]; then
        echo $size
    else
        echo $((2*1024*1024))
    fi
}

rotate_guardian_log() {
    local file="$GUARDIAN_LOG"
    [ ! -f "$file" ] && return 0
    local max_bytes=$(g_parse_size_bytes "$GUARDIAN_MAX_LOG_SIZE")
    local current_size=$(wc -c < "$file" 2>/dev/null || echo 0)
    if [ "$current_size" -ge "$max_bytes" ]; then
        for (( i=GUARDIAN_LOG_BACKUPS-1; i>=1; i-- )); do
            if [ -f "${file}.${i}" ]; then mv "${file}.${i}" "${file}.$((i+1))" 2>/dev/null || true; fi
        done
        mv "$file" "${file}.1" 2>/dev/null || true
        touch "$file"
        echo "$(date '+%Y-%m-%d %H:%M:%S') - 🔄 Rotated guardian log (size=${current_size})" >> "$file"
    fi
}

# ---- Configuration (override via env vars) ----
CHECK_INTERVAL=${CHECK_INTERVAL:-30}          # seconds between health checks
BACKEND_HEALTH_PATH="http://localhost:5055/api/health"
FRONTEND_HEALTH_PATH="http://localhost:3000"  # simple root probe
BUILD_CHECK=${BUILD_CHECK:-true}              # run dotnet build before backend restart
VERBOSE_HEALTH=${VERBOSE_HEALTH:-true}        # true -> log every cycle (or on change), false -> only on state change / restart
LOG_EVERY_N=${LOG_EVERY_N:-10}                # if VERBOSE_HEALTH=false, still force a periodic heartbeat every N cycles
MAX_BACKEND_RESTARTS=${MAX_BACKEND_RESTARTS:-20}
BACKOFF_BASE=${BACKOFF_BASE:-2}               # exponential backoff base seconds
BACKEND_FAIL_RESET_WINDOW=${BACKEND_FAIL_RESET_WINDOW:-600} # seconds to reset failure counters

# Internal state
cycle_counter=0
backend_prev_state="unknown"
frontend_prev_state="unknown"
backend_restart_count=0
backend_last_fail_ts=0

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

log() {
    echo -e "$(date '+%Y-%m-%d %H:%M:%S') - $1" | tee -a "$GUARDIAN_LOG"
}

check_backend() {
    if curl -s --max-time 3 "$BACKEND_HEALTH_PATH" > /dev/null 2>&1; then
        return 0  # Backend is healthy
    else
        return 1  # Backend is not responding
    fi
}

check_frontend() {
    if curl -s --max-time 3 "$FRONTEND_HEALTH_PATH" > /dev/null 2>&1; then
        return 0  # Frontend is healthy
    else
        return 1  # Frontend is not responding
    fi
}

start_backend() {
    log "${YELLOW}🔄 Starting Backend...${NC}"
    cd "$BACKEND_DIR"
    if [ "$BUILD_CHECK" = "true" ]; then
        log "${YELLOW}🧪 Running dotnet build pre-check...${NC}"
        if ! dotnet build --configuration Debug --verbosity quiet > /dev/null 2>&1; then
            log "${RED}❌ Build failed – skipping backend restart (will retry later).${NC}"
            backend_last_fail_ts=$(date +%s)
            return 1
        fi
        log "${GREEN}✅ Build OK – proceeding to run backend.${NC}"
    fi
    ./start_backend.sh &
    sleep 12  # Give it time to start
}

start_frontend() {
    log "${YELLOW}🔄 Starting Frontend...${NC}"
    cd "$FRONTEND_DIR"
    ./start_frontend.sh &
    sleep 8  # Give it time to start
}

log_state_change() {
    local service=$1
    local old=$2
    local new=$3
    if [ "$old" != "$new" ]; then
        log "${BLUE}ℹ️  State change: ${service}: ${old} -> ${new}${NC}"
    fi
}

maybe_log_heartbeat() {
    if [ "$VERBOSE_HEALTH" = "true" ]; then
        return 0
    fi
    # VERBOSE_HEALTH=false => only log every LOG_EVERY_N cycles
    if [ $((cycle_counter % LOG_EVERY_N)) -eq 0 ]; then
        log "⏱️  Heartbeat cycle=${cycle_counter} backend=${backend_prev_state} frontend=${frontend_prev_state}"
    fi
}

# Initial cleanup
> "$GUARDIAN_LOG"
rotate_guardian_log
log "${BLUE}🛡️ Khoan Guardian Started${NC}"
log "${BLUE}📍 Monitoring Backend: $BACKEND_HEALTH_PATH${NC}"
log "${BLUE}📍 Monitoring Frontend: $FRONTEND_HEALTH_PATH${NC}"
log "${BLUE}⏱️  Interval: ${CHECK_INTERVAL}s (override with CHECK_INTERVAL)${NC}"
log "${BLUE}🔧 Build pre-check: ${BUILD_CHECK} | Verbose health: ${VERBOSE_HEALTH} | Backoff base: ${BACKOFF_BASE}s${NC}"

# Start services initially
if ! check_backend; then
    log "${RED}❌ Backend not running, starting...${NC}"
    start_backend
fi

if ! check_frontend; then
    log "${RED}❌ Frontend not running, starting...${NC}"
    start_frontend
fi

# Monitoring loop
while true; do
    cycle_counter=$((cycle_counter + 1))
    now_ts=$(date +%s)

    # Reset failure counters if window passed
    if [ $((now_ts - backend_last_fail_ts)) -gt $BACKEND_FAIL_RESET_WINDOW ]; then
        backend_restart_count=0
    fi

    # BACKEND
    if check_backend; then
        new_state="healthy"
        backend_restart_count=0
    else
        new_state="down"
        log "${RED}⚠️ Backend DOWN (cycle=${cycle_counter})${NC}"
        if [ $backend_restart_count -ge $MAX_BACKEND_RESTARTS ]; then
            log "${RED}🛑 Max backend restart attempts (${MAX_BACKEND_RESTARTS}) reached – pausing restarts.${NC}"
        else
            # Exponential backoff delay before restart attempt based on restart count
            delay=$((BACKOFF_BASE ** backend_restart_count))
            if [ $delay -gt 120 ]; then delay=120; fi
            log "${YELLOW}⏳ Backoff ${delay}s before restart attempt (#$((backend_restart_count+1))).${NC}"
            sleep $delay
            start_backend || true
            backend_restart_count=$((backend_restart_count + 1))
            backend_last_fail_ts=$(date +%s)
        fi
    fi
    log_state_change "Backend" "$backend_prev_state" "$new_state"
    backend_prev_state="$new_state"

    # FRONTEND
    if check_frontend; then
        new_state="healthy"
    else
        new_state="down"
        log "${RED}⚠️ Frontend DOWN (cycle=${cycle_counter}) – restarting...${NC}"
        start_frontend || true
    fi
    log_state_change "Frontend" "$frontend_prev_state" "$new_state"
    frontend_prev_state="$new_state"

    maybe_log_heartbeat
    rotate_guardian_log

    sleep "$CHECK_INTERVAL"
done
