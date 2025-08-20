#!/bin/bash
# Enhanced Backend Startup Script - Full Process Management

# UTF-8 Configuration
export LANG=vi_VN.UTF-8
export LC_ALL=vi_VN.UTF-8

LOG_FILE="backend.log"
PORT=5055

log() {
    echo "$(date '+%Y-%m-%d %H:%M:%S') - $1" | tee -a "$LOG_FILE"
}

# Clear log file
> "$LOG_FILE"

log "🚀 Starting TinhKhoan Backend API Service..."
log "📊 Target Port: $PORT"

# Step 1: Aggressive cleanup of all backend services
log "🧹 Stopping all existing backend services..."

# Kill any dotnet processes related to this project
pkill -f "dotnet.*run.*TinhKhoanApp" 2>/dev/null
pkill -f "dotnet.*TinhKhoanApp.Api" 2>/dev/null
pkill -f "dotnet run" 2>/dev/null

# Force kill processes using port 5055
if lsof -ti:$PORT >/dev/null 2>&1; then
    log "🔴 Port $PORT is in use, killing processes..."
    lsof -ti:$PORT | xargs kill -9 2>/dev/null || true
    sleep 2
fi

# Clean up old PID files
rm -f "$LOG_FILE.pid" backend_startup.log backend_new.log

log "✅ Cleanup completed"

# Step 2: Test build before starting
log "🔨 Testing build process..."
dotnet clean --verbosity quiet > /dev/null 2>&1
if ! dotnet build --verbosity quiet > build_test.log 2>&1; then
    log "❌ BUILD FAILED! Check build_test.log for errors:"
    cat build_test.log | tail -20
    exit 1
fi

log "✅ Build test successful"
rm -f build_test.log

# Step 3: Start the API service
log "▶️ Starting Backend API..."
nohup dotnet run --urls="http://localhost:$PORT" >> "$LOG_FILE" 2>&1 &
API_PID=$!
echo $API_PID > "$LOG_FILE.pid"

log "🆔 Backend PID: $API_PID"

# Step 4: Health check with detailed feedback
log "🔍 Testing API health..."
for i in {1..20}; do
    if curl -s --connect-timeout 3 --max-time 5 http://localhost:$PORT/api/health >/dev/null 2>&1; then
        log "✅ Backend API is healthy and responding!"
        log "🔗 API Base URL: http://localhost:$PORT"
        log "🔗 Health Check: http://localhost:$PORT/api/health"
        log "📝 Logs: $LOG_FILE"
        log "🆔 PID File: $LOG_FILE.pid"
        log ""
        log "🎉 Backend startup completed successfully!"
        exit 0
    fi

    # Check if process is still running
    if ! kill -0 $API_PID 2>/dev/null; then
        log "❌ Backend process died unexpectedly!"
        log "� Recent log entries:"
        tail -10 "$LOG_FILE"
        exit 1
    fi

    log "⏳ Waiting for API to respond... ($i/20)"
    sleep 2
done
# Step 5: Failure handling
log "❌ Backend API failed to respond within timeout!"
log "📋 Recent log entries:"
tail -20 "$LOG_FILE"
log ""
log "🔧 Troubleshooting suggestions:"
log "   1. Check for port conflicts: lsof -i:$PORT"
log "   2. Review full logs: cat $LOG_FILE"
log "   3. Check build errors: dotnet build"
log "   4. Verify database connection"

# Kill the failed process
kill -9 $API_PID 2>/dev/null || true
rm -f "$LOG_FILE.pid"
exit 1
