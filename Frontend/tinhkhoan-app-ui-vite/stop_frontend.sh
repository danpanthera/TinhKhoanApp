#!/bin/bash
# Frontend Stop Script

# UTF-8 Configuration
export LANG=vi_VN.UTF-8
export LC_ALL=vi_VN.UTF-8

LOG_FILE="frontend.log"
PID_FILE="frontend.log.pid"
PORT=3000

log() {
    echo "$(date '+%Y-%m-%d %H:%M:%S') - $1" | tee -a "$LOG_FILE"
}

log "🛑 Stopping TinhKhoan Frontend Development Server..."

# Check if PID file exists
if [ -f "$PID_FILE" ]; then
    DEV_PID=$(cat "$PID_FILE")
    log "🆔 Found Frontend PID: $DEV_PID"

    # Check if process is actually running
    if kill -0 $DEV_PID 2>/dev/null; then
        log "⏳ Stopping frontend process..."
        kill -15 $DEV_PID 2>/dev/null

        # Wait for graceful shutdown
        for i in {1..10}; do
            if ! kill -0 $DEV_PID 2>/dev/null; then
                log "✅ Frontend stopped gracefully"
                rm -f "$PID_FILE"
                exit 0
            fi
            sleep 1
        done

        # Force kill if still running
        log "🔨 Force stopping frontend process..."
        kill -9 $DEV_PID 2>/dev/null || true
        rm -f "$PID_FILE"
        log "✅ Frontend force stopped"
    else
        log "⚠️ Frontend process not running (removing stale PID file)"
        rm -f "$PID_FILE"
    fi
else
    log "ℹ️ No PID file found"
fi

# Additional cleanup - kill any remaining npm/node processes
log "🧹 Cleaning up any remaining frontend processes..."
pkill -f "npm run dev" 2>/dev/null || true
pkill -f "vite.*dev" 2>/dev/null || true
pkill -f "node.*vite" 2>/dev/null || true

# Force kill anything using port 3000
if lsof -ti:$PORT >/dev/null 2>&1; then
    log "🔴 Killing processes using port $PORT..."
    lsof -ti:$PORT | xargs kill -9 2>/dev/null || true
fi

# Verify port is free
sleep 2
if lsof -ti:$PORT >/dev/null 2>&1; then
    log "❌ Port $PORT still occupied"
    exit 1
else
    log "✅ Frontend cleanup completed"
    exit 0
fi
