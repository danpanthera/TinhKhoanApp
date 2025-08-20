#!/bin/bash
# Master Service Management Script

# UTF-8 Configuration
export LANG=vi_VN.UTF-8
export LC_ALL=vi_VN.UTF-8

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BACKEND_DIR="$(cd "$SCRIPT_DIR/../../Backend/TinhKhoanApp.Api" && pwd)"
FRONTEND_DIR="$SCRIPT_DIR"

log() {
    echo "$(date '+%Y-%m-%d %H:%M:%S') - $1"
}

# Function to stop all services
stop_all_services() {
    log "🛑 Stopping all TinhKhoan services..."

    # Stop frontend first
    if [ -f "$FRONTEND_DIR/stop_frontend.sh" ]; then
        log "🔴 Stopping frontend..."
        cd "$FRONTEND_DIR"
        ./stop_frontend.sh
    fi

    # Stop backend
    if [ -f "$BACKEND_DIR/stop_backend.sh" ]; then
        log "🔴 Stopping backend..."
        cd "$BACKEND_DIR"
        ./stop_backend.sh
    fi

    log "✅ All services stopped"
}

# Function to start all services
start_all_services() {
    log "🚀 Starting all TinhKhoan services..."

    # Start backend first
    if [ -f "$BACKEND_DIR/start_backend.sh" ]; then
        log "🟢 Starting backend..."
        cd "$BACKEND_DIR"
        ./start_backend.sh &
        sleep 10  # Give backend time to start
    else
        log "❌ Backend start script not found!"
        exit 1
    fi

    # Start frontend
    if [ -f "$FRONTEND_DIR/start_frontend.sh" ]; then
        log "🟢 Starting frontend..."
        cd "$FRONTEND_DIR"
        ./start_frontend.sh &
        sleep 5  # Give frontend time to start
    else
        log "❌ Frontend start script not found!"
        exit 1
    fi

    log "✅ All services started"
}

# Function to restart all services
restart_all_services() {
    log "🔄 Restarting all TinhKhoan services..."
    stop_all_services
    sleep 5
    start_all_services
}

# Function to check service status
check_status() {
    log "📊 Checking service status..."

    # Check backend
    if lsof -ti:5055 >/dev/null 2>&1; then
        log "✅ Backend: Running (port 5055)"
    else
        log "❌ Backend: Not running"
    fi

    # Check frontend
    if lsof -ti:3000 >/dev/null 2>&1; then
        log "✅ Frontend: Running (port 3000)"
    else
        log "❌ Frontend: Not running"
    fi
}

# Parse command line arguments
case "${1:-start}" in
    "start")
        start_all_services
        ;;
    "stop")
        stop_all_services
        ;;
    "restart")
        restart_all_services
        ;;
    "status")
        check_status
        ;;
    "help"|"-h"|"--help")
        echo "TinhKhoan Service Manager"
        echo "Usage: $0 [start|stop|restart|status|help]"
        echo ""
        echo "Commands:"
        echo "  start    - Start all services (default)"
        echo "  stop     - Stop all services"
        echo "  restart  - Restart all services"
        echo "  status   - Check service status"
        echo "  help     - Show this help message"
        ;;
    *)
        log "❌ Invalid command: $1"
        log "Use '$0 help' for usage information"
        exit 1
        ;;
esac
