#!/bin/bash

# 🚀 IMPROVED Backend Startup Script - NO HANGING!
# Fixed by GitHub Copilot - Aug 3, 2025

LOG_FILE="backend.log"
PORT=5055
MAX_RETRIES=2
TIMEOUT=30

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

# Enhanced logging function
log() {
    echo -e "$(date '+%Y-%m-%d %H:%M:%S') - $1"
    echo "$(date '+%Y-%m-%d %H:%M:%S') - $1" >> "$LOG_FILE"
}

# Quick port check
is_port_in_use() {
    lsof -i :"$1" >/dev/null 2>&1
}

# Fast health check
check_api_health() {
    curl -s --max-time 3 http://localhost:$PORT/health >/dev/null 2>&1
}

# Aggressive cleanup
force_cleanup() {
    log "🧹 Force cleanup processes on port $PORT..."

    # Kill by port
    if is_port_in_use $PORT; then
        lsof -ti:$PORT | xargs kill -9 2>/dev/null
    fi

    # Kill dotnet processes
    pkill -f "dotnet.*run" 2>/dev/null
    pkill -f "dotnet.*TinhKhoanApp" 2>/dev/null

    sleep 2
}

# Quick startup check
quick_check() {
    if check_api_health; then
        log "✅ API already running and healthy!"
        exit 0
    fi
}

# Main startup
main() {
    # Clear log
    > "$LOG_FILE"

    log "🚀 Starting Backend API on port $PORT"

    # Quick check first
    quick_check

    # Force cleanup
    force_cleanup

    # Build if needed (quick check)
    if [[ ! -f "bin/Debug/net8.0/TinhKhoanApp.Api.dll" ]]; then
        log "🔨 Building project..."
        dotnet build -q >> "$LOG_FILE" 2>&1
        if [ $? -ne 0 ]; then
            log "❌ Build failed"
            exit 1
        fi
    fi

    # Start with timeout and background execution
    log "▶️  Starting API with timeout protection..."

    # Use timeout command to prevent hanging
    timeout $TIMEOUT dotnet run --urls=http://localhost:$PORT >> "$LOG_FILE" 2>&1 &
    API_PID=$!

    # Fast health checking loop
    log "⏳ Checking API health..."
    for i in {1..10}; do
        if check_api_health; then
            log "✅ API healthy and responsive!"
            log "📍 Backend running: http://localhost:$PORT"
            log "📍 Health check: http://localhost:$PORT/health"
            exit 0
        fi

        # Check if process died
        if ! kill -0 $API_PID 2>/dev/null; then
            log "❌ API process died unexpectedly"
            tail -5 "$LOG_FILE"
            exit 1
        fi

        sleep 2
    done

    log "❌ API not responding after $TIMEOUT seconds"
    kill -9 $API_PID 2>/dev/null
    tail -5 "$LOG_FILE"
    exit 1
}

# Execute
main "$@"
