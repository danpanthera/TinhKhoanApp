#!/bin/bash
# Universal Frontend Startup Script
# Có thể chạy từ bất cứ thư mục nào trong workspace

# UTF-8 Configuration
export LANG=vi_VN.UTF-8
export LC_ALL=vi_VN.UTF-8

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Auto-detect project directories
detect_project_dirs() {
    # Check if we're already in frontend directory
    if [[ -f "package.json" && -f "vite.config.js" ]]; then
        FRONTEND_DIR="$(pwd)"
        PROJECT_ROOT="$(dirname "$(dirname "$FRONTEND_DIR")")"
    # Check if we're in backend directory
    elif [[ -f "Khoan.Api.csproj" ]]; then
        BACKEND_DIR="$(pwd)"
        PROJECT_ROOT="$(dirname "$(dirname "$BACKEND_DIR")")"
        FRONTEND_DIR="$PROJECT_ROOT/Frontend/KhoanUI"
    # Try to find project root by walking up
    else
        local current_dir="$(pwd)"
        while [[ "$current_dir" != "/" ]]; do
            if [[ -d "$current_dir/Frontend/KhoanUI" && -f "$current_dir/Frontend/KhoanUI/package.json" ]]; then
                PROJECT_ROOT="$current_dir"
                FRONTEND_DIR="$PROJECT_ROOT/Frontend/KhoanUI"
                break
            fi
            current_dir="$(dirname "$current_dir")"
        done
    fi

    # Fallback to absolute paths if auto-detection fails
    if [[ -z "$FRONTEND_DIR" ]]; then
        FRONTEND_DIR="/opt/Projects/Khoan/Frontend/KhoanUI"
        PROJECT_ROOT="/opt/Projects/Khoan"
    fi
}

# Configuration
detect_project_dirs
FRONTEND_PORT=3000
LOG_FILE="$FRONTEND_DIR/frontend.log"
MAX_LOG_SIZE=${FRONTEND_MAX_LOG_SIZE:-5M}
LOG_BACKUPS=${FRONTEND_LOG_BACKUPS:-5}
FRONTEND_PID_FILE="$FRONTEND_DIR/frontend.pid"

# Log functions
log() {
    echo -e "${GREEN}[$(date '+%Y-%m-%d %H:%M:%S')]${NC} $1" | tee -a "$LOG_FILE"
}

error() {
    echo -e "${RED}[$(date '+%Y-%m-%d %H:%M:%S')] ERROR:${NC} $1" | tee -a "$LOG_FILE"
}

warn() {
    echo -e "${YELLOW}[$(date '+%Y-%m-%d %H:%M:%S')] WARNING:${NC} $1" | tee -a "$LOG_FILE"
}

info() {
    echo -e "${BLUE}[$(date '+%Y-%m-%d %H:%M:%S')] INFO:${NC} $1" | tee -a "$LOG_FILE"
}

# Utility functions
parse_size_bytes() {
    local size=$1
    if [[ $size =~ ^([0-9]+)[Kk]$ ]]; then
        echo $(( BASH_REMATCH[1] * 1024 ))
    elif [[ $size =~ ^([0-9]+)[Mm]$ ]]; then
        echo $(( BASH_REMATCH[1] * 1024 * 1024 ))
    elif [[ $size =~ ^[0-9]+$ ]]; then
        echo $size
    else
        echo $((5*1024*1024))
    fi
}

rotate_log_if_needed() {
    local file=$1
    [ ! -f "$file" ] && return 0
    local max_bytes=$(parse_size_bytes "$MAX_LOG_SIZE")
    local current_size=$(wc -c < "$file" 2>/dev/null || echo 0)
    if [ "$current_size" -ge "$max_bytes" ]; then
        for (( i=LOG_BACKUPS-1; i>=1; i-- )); do
            if [ -f "${file}.${i}" ]; then
                mv "${file}.${i}" "${file}.$((i+1))" 2>/dev/null || true
            fi
        done
        mv "$file" "${file}.1" 2>/dev/null || true
        touch "$file"
        echo "$(date '+%Y-%m-%d %H:%M:%S') - 🔄 Rotated $file" >> "$file"
    fi
}

is_port_in_use() {
    local port=$1
    lsof -ti:$port >/dev/null 2>&1
}

kill_port() {
    local port=$1
    local pids=$(lsof -ti:$port 2>/dev/null)
    if [ -n "$pids" ]; then
        warn "Killing processes on port $port: $pids"
        echo "$pids" | xargs kill -9 2>/dev/null || true
        sleep 2
    fi
}

# Stop existing frontend
stop_frontend() {
    log "🛑 Stopping existing frontend..."

    if [ -f "$FRONTEND_PID_FILE" ]; then
        local frontend_pid=$(cat "$FRONTEND_PID_FILE" 2>/dev/null)
        if [ -n "$frontend_pid" ] && kill -0 "$frontend_pid" 2>/dev/null; then
            warn "Stopping frontend process $frontend_pid"
            kill -TERM "$frontend_pid" 2>/dev/null || true
            sleep 3
            kill -KILL "$frontend_pid" 2>/dev/null || true
        fi
        rm -f "$FRONTEND_PID_FILE"
    fi

    kill_port $FRONTEND_PORT
    sleep 1
}

# Start frontend
start_frontend() {
    log "🎨 Starting Frontend (KhoanUI) on port $FRONTEND_PORT..."
    log "📁 Frontend directory: $FRONTEND_DIR"

    if [ ! -d "$FRONTEND_DIR" ]; then
        error "Frontend directory not found: $FRONTEND_DIR"
        return 1
    fi

    if [ ! -f "$FRONTEND_DIR/package.json" ]; then
        error "package.json not found in: $FRONTEND_DIR"
        return 1
    fi

    cd "$FRONTEND_DIR"

    # Rotate logs
    rotate_log_if_needed "$LOG_FILE"

    # Check if node_modules exists
    if [ ! -d "node_modules" ]; then
        info "Installing frontend dependencies..."
        if ! npm install >> "$LOG_FILE" 2>&1; then
            error "Frontend dependency installation failed. Check log: $LOG_FILE"
            return 1
        fi
    fi

    # Start frontend in background
    nohup npm run dev -- --port $FRONTEND_PORT --host >> "$LOG_FILE" 2>&1 &
    local frontend_pid=$!
    echo "$frontend_pid" > "$FRONTEND_PID_FILE"

    # Wait for frontend to start
    local wait_time=0
    local max_wait=45

    while [ $wait_time -lt $max_wait ]; do
        if is_port_in_use $FRONTEND_PORT; then
            log "✅ Frontend started successfully on port $FRONTEND_PORT (PID: $frontend_pid)"
            log "🌐 Frontend: http://localhost:$FRONTEND_PORT"
            return 0
        fi
        sleep 1
        wait_time=$((wait_time + 1))
        printf "."
    done

    error "Frontend failed to start within $max_wait seconds"
    return 1
}

# Health check
health_check() {
    info "🏥 Performing health check..."

    if curl -s "http://localhost:$FRONTEND_PORT" >/dev/null 2>&1; then
        log "✅ Frontend health check passed"
    else
        warn "⚠️  Frontend health check failed"
    fi
}

# Main execution
main() {
    log "🎨 Universal Frontend Startup Script"
    log "📂 Current directory: $(pwd)"
    log "🎯 Target frontend: $FRONTEND_DIR"

    # Validate directories
    if [ ! -d "$FRONTEND_DIR" ]; then
        error "Could not find frontend directory: $FRONTEND_DIR"
        error "Please run this script from within the Khoan project workspace"
        exit 1
    fi

    # Stop existing and start new
    stop_frontend

    if start_frontend; then
        sleep 2
        health_check
        log "🎉 Frontend startup completed!"
        log "💡 Stop with: pkill -f 'npm.*dev' or kill PID from: $FRONTEND_PID_FILE"
    else
        error "Failed to start frontend"
        exit 1
    fi
}

# Handle Ctrl+C gracefully
trap 'log "Received interrupt signal, stopping frontend..."; stop_frontend; exit 0' INT TERM

main "$@"
