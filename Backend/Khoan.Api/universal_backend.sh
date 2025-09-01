#!/bin/bash
# Universal Backend Startup Script
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
    # Check if we're already in backend directory
    if [[ -f "Khoan.Api.csproj" ]]; then
        BACKEND_DIR="$(pwd)"
        PROJECT_ROOT="$(dirname "$(dirname "$BACKEND_DIR")")"
    # Check if we're in frontend directory
    elif [[ -f "package.json" && -f "vite.config.js" ]]; then
        FRONTEND_DIR="$(pwd)"
        PROJECT_ROOT="$(dirname "$(dirname "$FRONTEND_DIR")")"
        BACKEND_DIR="$PROJECT_ROOT/Backend/Khoan.Api"
    # Try to find project root by walking up
    else
        local current_dir="$(pwd)"
        while [[ "$current_dir" != "/" ]]; do
            if [[ -d "$current_dir/Backend/Khoan.Api" && -f "$current_dir/Backend/Khoan.Api/Khoan.Api.csproj" ]]; then
                PROJECT_ROOT="$current_dir"
                BACKEND_DIR="$PROJECT_ROOT/Backend/Khoan.Api"
                break
            fi
            current_dir="$(dirname "$current_dir")"
        done
    fi
    
    # Fallback to absolute paths if auto-detection fails
    if [[ -z "$BACKEND_DIR" ]]; then
        BACKEND_DIR="/opt/Projects/Khoan/Backend/Khoan.Api"
        PROJECT_ROOT="/opt/Projects/Khoan"
    fi
}

# Configuration
detect_project_dirs
BACKEND_PORT=5055
LOG_FILE="$BACKEND_DIR/backend.log"
MAX_LOG_SIZE=${BACKEND_MAX_LOG_SIZE:-5M}
LOG_BACKUPS=${BACKEND_LOG_BACKUPS:-5}
BACKEND_PID_FILE="$BACKEND_DIR/backend.pid"

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
        echo "$(date '+%Y-%m-%d %H:%M:%S') - 🔄 Rotated $file (size=${current_size} bytes, max=${max_bytes})" >> "$file"
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

# Stop existing backend
stop_backend() {
    log "🛑 Stopping existing backend..."
    
    if [ -f "$BACKEND_PID_FILE" ]; then
        local backend_pid=$(cat "$BACKEND_PID_FILE" 2>/dev/null)
        if [ -n "$backend_pid" ] && kill -0 "$backend_pid" 2>/dev/null; then
            warn "Stopping backend process $backend_pid"
            kill -TERM "$backend_pid" 2>/dev/null || true
            sleep 3
            kill -KILL "$backend_pid" 2>/dev/null || true
        fi
        rm -f "$BACKEND_PID_FILE"
    fi
    
    kill_port $BACKEND_PORT
    sleep 1
}

# Start backend
start_backend() {
    log "🚀 Starting Backend (Khoan.Api) on port $BACKEND_PORT..."
    log "📁 Backend directory: $BACKEND_DIR"
    
    if [ ! -d "$BACKEND_DIR" ]; then
        error "Backend directory not found: $BACKEND_DIR"
        return 1
    fi
    
    if [ ! -f "$BACKEND_DIR/Khoan.Api.csproj" ]; then
        error "Khoan.Api.csproj not found in: $BACKEND_DIR"
        return 1
    fi
    
    cd "$BACKEND_DIR"
    
    # Rotate logs
    rotate_log_if_needed "$LOG_FILE"
    
    # Build backend
    info "Building backend..."
    if ! dotnet build Khoan.Api.csproj --no-restore --verbosity quiet >> "$LOG_FILE" 2>&1; then
        error "Backend build failed. Check log: $LOG_FILE"
        return 1
    fi
    
    # Start backend in background
    nohup dotnet run --project Khoan.Api.csproj --no-build --urls "http://localhost:$BACKEND_PORT" >> "$LOG_FILE" 2>&1 &
    local backend_pid=$!
    echo "$backend_pid" > "$BACKEND_PID_FILE"
    
    # Wait for backend to start
    local wait_time=0
    local max_wait=30
    
    while [ $wait_time -lt $max_wait ]; do
        if is_port_in_use $BACKEND_PORT; then
            log "✅ Backend started successfully on port $BACKEND_PORT (PID: $backend_pid)"
            log "🌐 Backend API: http://localhost:$BACKEND_PORT"
            log "📖 API Docs: http://localhost:$BACKEND_PORT/swagger"
            return 0
        fi
        sleep 1
        wait_time=$((wait_time + 1))
        printf "."
    done
    
    error "Backend failed to start within $max_wait seconds"
    return 1
}

# Health check
health_check() {
    info "🏥 Performing health check..."
    
    if curl -s "http://localhost:$BACKEND_PORT/health" >/dev/null 2>&1; then
        log "✅ Backend health check passed"
    else
        warn "⚠️  Backend health check failed"
    fi
}

# Main execution
main() {
    log "🔧 Universal Backend Startup Script"
    log "📂 Current directory: $(pwd)"
    log "🎯 Target backend: $BACKEND_DIR"
    
    # Validate directories
    if [ ! -d "$BACKEND_DIR" ]; then
        error "Could not find backend directory: $BACKEND_DIR"
        error "Please run this script from within the Khoan project workspace"
        exit 1
    fi
    
    # Stop existing and start new
    stop_backend
    
    if start_backend; then
        sleep 2
        health_check
        log "🎉 Backend startup completed!"
        log "💡 Stop with: pkill -f 'dotnet.*Khoan.Api' or kill PID from: $BACKEND_PID_FILE"
    else
        error "Failed to start backend"
        exit 1
    fi
}

# Handle Ctrl+C gracefully
trap 'log "Received interrupt signal, stopping backend..."; stop_backend; exit 0' INT TERM

main "$@"
