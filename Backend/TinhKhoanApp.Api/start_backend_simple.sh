#!/bin/bash

# Simple backend startup script - No waiting, just start
LOG_FILE="backend.log"
PORT=5055

# Function to log messages
log() {
  echo "$(date '+%Y-%m-%d %H:%M:%S') - $1" | tee -a "$LOG_FILE"
}

# Kill any existing processes on port
log "🧹 Cleaning up port $PORT..."
pkill -f "dotnet.*TinhKhoanApp.Api" 2>/dev/null || true
lsof -ti:$PORT | xargs kill -9 2>/dev/null || true

# Wait a moment for cleanup
sleep 1

# Start the API immediately without waiting
log "🚀 Starting TinhKhoanApp.Api on port $PORT..."
log "📝 Logs will be written to: $LOG_FILE"

# Start dotnet and let it run
exec dotnet run --urls "http://localhost:$PORT" 2>&1 | tee -a "$LOG_FILE"
