#!/bin/bash
# Fullstack Startup Script - KhoanUI Frontend + Khoan.Api Backend
# Cấu hình: Frontend port 3000, Backend port 5055

# UTF-8 Configuration
export LANG=vi_VN.UTF-8
export LC_ALL=vi_VN.UTF-8

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
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

error() {
    echo -e "${RED}[$(date '+%Y-%m-%d %H:%M:%S')] ERROR:${NC} $1"
}

warn() {
    echo -e "${YELLOW}[$(date '+%Y-%m-%d %H:%M:%S')] WARNING:${NC} $1"
}

info() {
    echo -e "${BLUE}[$(date '+%Y-%m-%d %H:%M:%S')] INFO:${NC} $1"
}

# Check if port is in use
is_port_in_use() {
    local port=$1
    lsof -ti:$port >/dev/null 2>&1
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

# Stop existing processes
stop_services() {
    log "🛑 Stopping existing services..."

    # Stop frontend
    if [ -f "$FRONTEND_DIR/$FRONTEND_PID_FILE" ]; then
        local frontend_pid=$(cat "$FRONTEND_DIR/$FRONTEND_PID_FILE" 2>/dev/null)
        if [ -n "$frontend_pid" ] && kill -0 "$frontend_pid" 2>/dev/null; then
            warn "Stopping frontend process $frontend_pid"
            kill -TERM "$frontend_pid" 2>/dev/null || true
            sleep 3
            kill -KILL "$frontend_pid" 2>/dev/null || true
        fi
        rm -f "$FRONTEND_DIR/$FRONTEND_PID_FILE"
    fi

    # Stop backend
    if [ -f "$BACKEND_DIR/$BACKEND_PID_FILE" ]; then
        local backend_pid=$(cat "$BACKEND_DIR/$BACKEND_PID_FILE" 2>/dev/null)
        if [ -n "$backend_pid" ] && kill -0 "$backend_pid" 2>/dev/null; then
            warn "Stopping backend process $backend_pid"
            kill -TERM "$backend_pid" 2>/dev/null || true
            sleep 3
            kill -KILL "$backend_pid" 2>/dev/null || true
        fi
        rm -f "$BACKEND_DIR/$BACKEND_PID_FILE"
    fi

    # Force kill ports
    kill_port $FRONTEND_PORT
    kill_port $BACKEND_PORT

    sleep 2
}

# Start backend
start_backend() {
    log "🚀 Starting Backend (Khoan.Api) on port $BACKEND_PORT..."

    if [ ! -d "$BACKEND_DIR" ]; then
        error "Backend directory not found: $BACKEND_DIR"
        return 1
    fi

    cd "$BACKEND_DIR"

    # Build backend
    info "Building backend..."
    dotnet build Khoan.Api.csproj --no-restore --verbosity quiet
    if [ $? -ne 0 ]; then
        error "Backend build failed"
        return 1
    fi

    # Start backend in background
    nohup dotnet run --project Khoan.Api.csproj --no-build --urls "http://localhost:$BACKEND_PORT" > backend.log 2>&1 &
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

    if [ ! -d "$FRONTEND_DIR" ]; then
        error "Frontend directory not found: $FRONTEND_DIR"
        return 1
    fi

    cd "$FRONTEND_DIR"

    # Check if node_modules exists
    if [ ! -d "node_modules" ]; then
        info "Installing frontend dependencies..."
        npm install
        if [ $? -ne 0 ]; then
            error "Frontend dependency installation failed"
            return 1
        fi
    fi

    # Start frontend in background
    nohup npm run dev -- --port $FRONTEND_PORT --host > frontend.log 2>&1 &
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
    log "🌟 Starting KhoanUI + Khoan.Api Fullstack Application"
    log "📊 Frontend: http://localhost:$FRONTEND_PORT"
    log "🔧 Backend API: http://localhost:$BACKEND_PORT"
    log "📖 API Docs: http://localhost:$BACKEND_PORT/swagger"

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
        log "💡 To stop the services, run: ./stop_fullstack.sh"
        log "📝 Logs: frontend.log (Frontend), backend.log (Backend)"
    else
        error "Failed to start fullstack application"
        stop_services
        exit 1
    fi
}

# Handle Ctrl+C gracefully
trap 'log "Received interrupt signal, stopping services..."; stop_services; exit 0' INT TERM

main "$@"
