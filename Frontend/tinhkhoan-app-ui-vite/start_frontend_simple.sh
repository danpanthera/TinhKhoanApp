#!/bin/bash

# Simple frontend startup script - No waiting, just start
LOG_FILE="frontend.log"
PORT=3000

# Function to log messages
log() {
  echo "$(date '+%Y-%m-%d %H:%M:%S') - $1" | tee -a "$LOG_FILE"
}

# Kill any existing processes on port
log "🧹 Cleaning up port $PORT..."
pkill -f "vite.*3000" 2>/dev/null || true
lsof -ti:$PORT | xargs kill -9 2>/dev/null || true

# Wait a moment for cleanup
sleep 1

# Start the frontend immediately without waiting
log "🎨 Starting TinhKhoan Frontend on port $PORT..."
log "📝 Logs will be written to: $LOG_FILE"

# Start npm dev server and let it run
exec npm run dev 2>&1 | tee -a "$LOG_FILE"
