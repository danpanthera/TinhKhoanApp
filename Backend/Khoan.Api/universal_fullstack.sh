#!/bin/bash
# Universal Fullstack Startup Script
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
        BACKEND_DIR="$PROJECT_ROOT/Backend/Khoan.Api"
    # Check if we're in backend directory
    elif [[ -f "Khoan.Api.csproj" ]]; then
        BACKEND_DIR="$(pwd)"
        PROJECT_ROOT="$(dirname "$(dirname "$BACKEND_DIR")")"
        FRONTEND_DIR="$PROJECT_ROOT/Frontend/KhoanUI"
    # Try to find project root by walking up
    else
        local current_dir="$(pwd)"
        while [[ "$current_dir" != "/" ]]; do
            if [[ -d "$current_dir/Frontend/KhoanUI" && -d "$current_dir/Backend/Khoan.Api" ]]; then
                PROJECT_ROOT="$current_dir"
                FRONTEND_DIR="$PROJECT_ROOT/Frontend/KhoanUI"
                BACKEND_DIR="$PROJECT_ROOT/Backend/Khoan.Api"
                break
            fi
            current_dir="$(dirname "$current_dir")"
        done
    fi

    # Fallback to absolute paths if auto-detection fails
    if [[ -z "$FRONTEND_DIR" || -z "$BACKEND_DIR" ]]; then
        PROJECT_ROOT="/opt/Projects/Khoan"
        FRONTEND_DIR="$PROJECT_ROOT/Frontend/KhoanUI"
        BACKEND_DIR="$PROJECT_ROOT/Backend/Khoan.Api"
    fi
}

# Configuration
detect_project_dirs
FRONTEND_PORT=3000
BACKEND_PORT=5055

# PID files (stored in respective directories)
FRONTEND_PID_FILE="$FRONTEND_DIR/frontend.pid"
BACKEND_PID_FILE="$BACKEND_DIR/backend.pid"

# Log files (stored in respective directories)
FRONTEND_LOG="$FRONTEND_DIR/frontend.log"
BACKEND_LOG="$BACKEND_DIR/backend.log"

# Log functions
log() {
    echo -e "${GREEN}[$(date '+%Y-%m-%d %H:%M:%S')]${NC} $1"
}

error() {
    echo -e "${RED}[$(date '+%Y-%m-%d %H:%M:%S')] ERROR:${NC} $1"
}

warn() {
    echo -e "${YELLOW}[$(date '+%Y-%m-%d %H:%M:%S')] WARNING:${NC} $1"
}

info() {
    echo -e "${BLUE}[$(date '+%Y-%m-%d %H:%M:%S')] INFO:${NC} $1"
}

# Utility functions
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

# Stop existing processes
stop_services() {
    log "🛑 Stopping existing services..."

    # Stop frontend
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

    # Stop backend
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

    # Force kill ports
    kill_port $FRONTEND_PORT
    kill_port $BACKEND_PORT

    sleep 2
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

    # Build backend
    info "Building backend..."
    if ! dotnet build Khoan.Api.csproj --no-restore --verbosity quiet >> "$BACKEND_LOG" 2>&1; then
        error "Backend build failed. Check log: $BACKEND_LOG"
        return 1
    fi

    # Start backend in background
    nohup dotnet run --project Khoan.Api.csproj --no-build --urls "http://localhost:$BACKEND_PORT" >> "$BACKEND_LOG" 2>&1 &
    local backend_pid=$!
    echo "$backend_pid" > "$BACKEND_PID_FILE"

    # Wait for backend to start
    local wait_time=0
    local max_wait=30

    while [ $wait_time -lt $max_wait ]; do
        if is_port_in_use $BACKEND_PORT; then
            log "✅ Backend started successfully on port $BACKEND_PORT (PID: $backend_pid)"
            return 0
        fi
        sleep 1
        wait_time=$((wait_time + 1))
        printf "."
    done

    error "Backend failed to start within $max_wait seconds"
    return 1
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

    # Check if node_modules exists
    if [ ! -d "node_modules" ]; then
        info "Installing frontend dependencies..."
        if ! npm install >> "$FRONTEND_LOG" 2>&1; then
            error "Frontend dependency installation failed. Check log: $FRONTEND_LOG"
            return 1
        fi
    fi

    # Start frontend in background
    nohup npm run dev -- --port $FRONTEND_PORT --host >> "$FRONTEND_LOG" 2>&1 &
    local frontend_pid=$!
    echo "$frontend_pid" > "$FRONTEND_PID_FILE"

    # Wait for frontend to start
    local wait_time=0
    local max_wait=45

    while [ $wait_time -lt $max_wait ]; do
        if is_port_in_use $FRONTEND_PORT; then
            log "✅ Frontend started successfully on port $FRONTEND_PORT (PID: $frontend_pid)"
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
    info "🏥 Performing health checks..."

    # Check backend
    if curl -s "http://localhost:$BACKEND_PORT/health" >/dev/null 2>&1; then
        log "✅ Backend health check passed"
    else
        warn "⚠️  Backend health check failed"
    fi

    # Check frontend
    if curl -s "http://localhost:$FRONTEND_PORT" >/dev/null 2>&1; then
        log "✅ Frontend health check passed"
    else
        warn "⚠️  Frontend health check failed"
    fi
}

# Main execution
main() {
    log "🌟 Universal Fullstack Startup Script"
    log "📂 Current directory: $(pwd)"
    log "🎯 Project root: $PROJECT_ROOT"
    log "📊 Frontend: $FRONTEND_DIR"
    log "🔧 Backend: $BACKEND_DIR"
    log ""
    log "🌐 Frontend: http://localhost:$FRONTEND_PORT"
    log "🔧 Backend API: http://localhost:$BACKEND_PORT"
    log "📖 API Docs: http://localhost:$BACKEND_PORT/swagger"

    # Validate directories
    if [ ! -d "$FRONTEND_DIR" ] || [ ! -d "$BACKEND_DIR" ]; then
        error "Could not find required directories:"
        error "  Frontend: $FRONTEND_DIR"
        error "  Backend: $BACKEND_DIR"
        error "Please run this script from within the Khoan project workspace"
        exit 1
    fi

    # Stop existing services
    stop_services

    # Start services
    if start_backend && start_frontend; then
        sleep 3
        health_check
        log "🎉 Fullstack application started successfully!"
        log "🌐 Access the application at: http://localhost:$FRONTEND_PORT"
        log "📚 API documentation at: http://localhost:$BACKEND_PORT/swagger"
        log ""
        log "💡 Logs:"
        log "   Frontend: $FRONTEND_LOG"
        log "   Backend: $BACKEND_LOG"
        log "💡 PIDs:"
        log "   Frontend: $FRONTEND_PID_FILE"
        log "   Backend: $BACKEND_PID_FILE"
    else
        error "Failed to start fullstack application"
        stop_services
        exit 1
    fi
}

# Handle Ctrl+C gracefully
trap 'log "Received interrupt signal, stopping services..."; stop_services; exit 0' INT TERM

main "$@"
