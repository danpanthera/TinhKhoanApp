#!/bin/bash
# Enhanced Backend Stop Script

LOG_FILE="backend.log"
PID_FILE="backend.log.pid"
PORT=5055

log() {
    echo "$(date '+%Y-%m-%d %H:%M:%S') - $1" | tee -a "$LOG_FILE"
}

log "🛑 Stopping Khoan Backend API Server..."

# Check if PID file exists and stop the specific process
if [ -f "$PID_FILE" ]; then
    BACKEND_PID=$(cat "$PID_FILE")
    log "🆔 Found Backend PID: $BACKEND_PID"

    if kill -0 $BACKEND_PID 2>/dev/null; then
        log "⏳ Stopping backend process gracefully..."
        kill -15 $BACKEND_PID 2>/dev/null

        # Wait for graceful shutdown (max 15 seconds)
        for i in {1..15}; do
            if ! kill -0 $BACKEND_PID 2>/dev/null; then
                log "✅ Backend stopped gracefully"
                rm -f "$PID_FILE"
                break
            fi
            sleep 1
        done

        # Force kill if still running
        if kill -0 $BACKEND_PID 2>/dev/null; then
            log "🔨 Force stopping backend process..."
            kill -9 $BACKEND_PID 2>/dev/null || true
            rm -f "$PID_FILE"
            log "✅ Backend force stopped"
        fi
    else
        log "⚠️ Backend process not running (removing stale PID file)"
        rm -f "$PID_FILE"
    fi
else
    log "ℹ️ No PID file found"
fi

# Comprehensive cleanup of .NET processes
log "🧹 Cleaning up .NET processes..."

# Kill dotnet watch processes
pkill -f "dotnet.*watch" 2>/dev/null || true

# Kill dotnet run processes
pkill -f "dotnet.*run" 2>/dev/null || true

# Kill KhoanApp processes
pkill -f "KhoanApp" 2>/dev/null || true

# Kill any process using our specific port
if lsof -ti:$PORT >/dev/null 2>&1; then
    log "🔴 Killing processes using port $PORT..."
    lsof -ti:$PORT | xargs kill -9 2>/dev/null || true
fi

# Wait a moment for processes to fully terminate
sleep 3

# Final verification
if lsof -ti:$PORT >/dev/null 2>&1; then
    log "❌ Port $PORT still occupied after cleanup"
    lsof -i:$PORT
    exit 1
else
    log "✅ Backend cleanup completed - port $PORT is free"
    exit 0
fi
