#!/bin/bash
# Stop Backend Script

PORT=5055
LOG_FILE="backend.log"
PID_FILE="$LOG_FILE.pid"

log() {
    echo "$(date '+%H:%M:%S') - $1" | tee -a "$LOG_FILE"
}

log "🛑 Stopping Backend API..."

# Kill by PID file if exists
if [[ -f "$PID_FILE" ]]; then
    PID=$(cat "$PID_FILE")
    if kill -0 $PID 2>/dev/null; then
        log "🔪 Killing process $PID"
        kill -TERM $PID 2>/dev/null
        sleep 3
        if kill -0 $PID 2>/dev/null; then
            log "🔨 Force killing process $PID"
            kill -9 $PID 2>/dev/null
        fi
    fi
    rm -f "$PID_FILE"
fi

# Aggressive cleanup
log "🧹 Cleaning up processes..."
pkill -f "dotnet.*run" 2>/dev/null
pkill -f "dotnet.*TinhKhoanApp" 2>/dev/null
lsof -ti:$PORT | xargs kill -9 2>/dev/null || true

# Verify port is free
sleep 2
if lsof -ti:$PORT >/dev/null 2>&1; then
    log "❌ Port $PORT still occupied"
    exit 1
else
    log "✅ Backend stopped successfully"
    exit 0
fi
