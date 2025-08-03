#!/bin/bash
# Improved Backend Script - NO HANGING

LOG_FILE="backend.log"
PORT=5055

log() {
    echo "$(date '+%H:%M:%S') - $1" | tee -a "$LOG_FILE"
}

# Clear log
> "$LOG_FILE"

log "🚀 Starting Backend API..."

# Quick health check
if curl -s --max-time 2 http://localhost:$PORT/health > /dev/null 2>&1; then
    log "✅ API already running!"
    exit 0
fi

# Aggressive cleanup
log "🧹 Cleaning up..."
pkill -f "dotnet.*run" 2>/dev/null
pkill -f "dotnet.*TinhKhoanApp" 2>/dev/null
lsof -ti:$PORT | xargs kill -9 2>/dev/null || true
sleep 3

# Quick build check
if [[ ! -f "bin/Debug/net8.0/TinhKhoanApp.Api.dll" ]]; then
    log "🔨 Building..."
    dotnet build -q >> "$LOG_FILE" 2>&1
fi

log "▶️ Starting API with timeout..."
timeout 30s dotnet run --urls=http://localhost:$PORT >> "$LOG_FILE" 2>&1 &
API_PID=$!

# Quick health check loop
for i in {1..15}; do
    if curl -s --max-time 2 http://localhost:$PORT/health > /dev/null 2>&1; then
        log "✅ API started successfully!"
        exit 0
    fi
    
    if ! kill -0 $API_PID 2>/dev/null; then
        log "❌ API process died"
        exit 1
    fi
    
    sleep 2
done

log "❌ API timeout"
kill -9 $API_PID 2>/dev/null
exit 1
