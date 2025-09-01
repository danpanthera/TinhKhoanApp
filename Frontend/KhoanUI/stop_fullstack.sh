#!/bin/bash
# Stop Fullstack Application - KhoanUI Frontend + Khoan.Api Backend

# UTF-8 Configuration
export LANG=vi_VN.UTF-8
export LC_ALL=vi_VN.UTF-8

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
FRONTEND_DIR="/opt/Projects/Khoan/Frontend/KhoanUI"
BACKEND_DIR="/opt/Projects/Khoan/Backend/Khoan.Api"
FRONTEND_PORT=3000
BACKEND_PORT=5055

# PID files
FRONTEND_PID_FILE="frontend.pid"
BACKEND_PID_FILE="backend.pid"

# Log function
log() {
    echo -e "${GREEN}[$(date '+%Y-%m-%d %H:%M:%S')]${NC} $1"
}

warn() {
    echo -e "${YELLOW}[$(date '+%Y-%m-%d %H:%M:%S')] WARNING:${NC} $1"
}

# Kill process on port
kill_port() {
    local port=$1
    local pids=$(lsof -ti:$port 2>/dev/null)
    if [ -n "$pids" ]; then
        warn "Killing processes on port $port: $pids"
        echo "$pids" | xargs kill -9 2>/dev/null || true
        sleep 2
    fi
}

# Stop services
stop_services() {
    log "🛑 Stopping Fullstack Application..."

    # Stop frontend
    if [ -f "$FRONTEND_DIR/$FRONTEND_PID_FILE" ]; then
        local frontend_pid=$(cat "$FRONTEND_DIR/$FRONTEND_PID_FILE" 2>/dev/null)
        if [ -n "$frontend_pid" ] && kill -0 "$frontend_pid" 2>/dev/null; then
            log "Stopping frontend process $frontend_pid"
            kill -TERM "$frontend_pid" 2>/dev/null || true
            sleep 3
            kill -KILL "$frontend_pid" 2>/dev/null || true
        fi
        rm -f "$FRONTEND_DIR/$FRONTEND_PID_FILE"
        log "✅ Frontend stopped"
    fi

    # Stop backend
    if [ -f "$BACKEND_DIR/$BACKEND_PID_FILE" ]; then
        local backend_pid=$(cat "$BACKEND_DIR/$BACKEND_PID_FILE" 2>/dev/null)
        if [ -n "$backend_pid" ] && kill -0 "$backend_pid" 2>/dev/null; then
            log "Stopping backend process $backend_pid"
            kill -TERM "$backend_pid" 2>/dev/null || true
            sleep 3
            kill -KILL "$backend_pid" 2>/dev/null || true
        fi
        rm -f "$BACKEND_DIR/$BACKEND_PID_FILE"
        log "✅ Backend stopped"
    fi

    # Force kill ports
    kill_port $FRONTEND_PORT
    kill_port $BACKEND_PORT

    log "🎯 All services stopped successfully"
}

# Main execution
main() {
    stop_services
}

main "$@"
