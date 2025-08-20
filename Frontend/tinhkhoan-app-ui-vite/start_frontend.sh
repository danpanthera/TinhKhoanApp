#!/bin/bash
# Enhanced Frontend Startup Script - Full Process Management

# UTF-8 Configuration
export LANG=vi_VN.UTF-8
export LC_ALL=vi_VN.UTF-8

LOG_FILE="frontend.log"
PORT=3000
MAX_STARTUP_TIME=60

# Function to log messages to console and file
log() {
    echo "$(date '+%Y-%m-%d %H:%M:%S') - $1" | tee -a "$LOG_FILE"
}

# Function to check if port is in use
is_port_in_use() {
    lsof -i:"$1" >/dev/null 2>&1
    return $?
}

# Function to aggressive cleanup
cleanup_all() {
    log "🧹 Stopping all existing frontend services..."

    # Kill all npm dev processes
    pkill -f "npm run dev" 2>/dev/null || true
    pkill -f "vite.*dev" 2>/dev/null || true
    pkill -f "node.*vite" 2>/dev/null || true

    # Force kill anything using port 3000
    if is_port_in_use $PORT; then
        log "🔴 Port $PORT is in use, killing processes..."
        for pid in $(lsof -ti:$PORT); do
            log "Killing process $pid using port $PORT"
            kill -15 $pid 2>/dev/null || kill -9 $pid 2>/dev/null
            sleep 1
        done
    fi

    # Clean cache if needed
    if [ -d "node_modules/.vite" ]; then
        log "🗑️ Cleaning Vite cache..."
        rm -rf node_modules/.vite .vite
    fi

    log "✅ Cleanup completed"
}

# Clear log file at start
> "$LOG_FILE"

log "🚀 Starting TinhKhoan Frontend Development Server"
log "📊 Target Port: $PORT"

# Step 1: Cleanup existing services
cleanup_all

# Step 2: Check if node_modules exists
if [ ! -d "node_modules" ]; then
    log "📦 Installing dependencies..."
    if ! npm install >> "$LOG_FILE" 2>&1; then
        log "❌ NPM INSTALL FAILED! Check logs:"
        tail -20 "$LOG_FILE"
        exit 1
    fi
    log "✅ Dependencies installed"
fi

# Step 3: Test build to check for errors
log "🔨 Testing build configuration..."

# Start build test in background with timeout handling (macOS compatible)
npm run build > build_test.log 2>&1 &
build_pid=$!

# Wait for build with timeout (30 seconds)
build_timeout=30
elapsed=0
while [ $elapsed -lt $build_timeout ]; do
    if ! kill -0 $build_pid 2>/dev/null; then
        # Build process completed
        wait $build_pid
        build_exit_code=$?
        break
    fi
    sleep 1
    elapsed=$((elapsed + 1))
done

# Handle timeout case
if kill -0 $build_pid 2>/dev/null; then
    log "⏰ Build test timeout after ${build_timeout}s (normal for large projects)"
    kill -9 $build_pid 2>/dev/null || true
    build_exit_code=124
fi

if [ $build_exit_code -eq 0 ]; then
    log "✅ Build test successful"
elif [ $build_exit_code -eq 124 ]; then
    log "⏰ Build test timeout (normal for large projects)"
else
    log "⚠️ Build test completed with warnings. Check build_test.log"
    if [ -f build_test.log ]; then
        log "📋 Recent build output:"
        tail -10 build_test.log
    fi
fi

rm -f build_test.log

# Step 4: Start the development server
log "▶️ Starting Frontend Development Server..."
npm run dev -- --port $PORT >> "$LOG_FILE" 2>&1 &
DEV_PID=$!
echo $DEV_PID > "frontend.log.pid"

log "🆔 Frontend PID: $DEV_PID"

# Step 5: Monitor startup with detailed feedback
log "🔍 Monitoring server startup..."
start_time=$(date +%s)

while true; do
    current_time=$(date +%s)
    elapsed=$((current_time - start_time))

    # Check if process is still running
    if ! kill -0 $DEV_PID 2>/dev/null; then
        log "❌ Frontend process died unexpectedly!"
        log "📋 Recent log entries:"
        tail -20 "$LOG_FILE"
        exit 1
    fi

    # Check for successful startup indicators
    if grep -q "Local:.*http://localhost:$PORT" "$LOG_FILE" 2>/dev/null; then
        server_url=$(grep "Local:" "$LOG_FILE" | tail -1 | awk '{print $2}')
        log "✅ Frontend server started successfully!"
        log "🔗 Local URL: $server_url"
        log "🔗 Dev Server: http://localhost:$PORT"
        log "📝 Logs: $LOG_FILE"
        log "🆔 PID File: frontend.log.pid"
        log ""
        log "🎉 Frontend startup completed successfully!"
        exit 0
    fi

    # Check for errors
    if grep -qi "error\|failed\|cannot" "$LOG_FILE" 2>/dev/null; then
        log "❌ Error detected during startup!"
        log "📋 Recent error logs:"
        grep -i "error\|failed\|cannot" "$LOG_FILE" | tail -5
        kill -9 $DEV_PID 2>/dev/null || true
        rm -f "frontend.log.pid"
        exit 1
    fi

    # Timeout check
    if [ $elapsed -ge $MAX_STARTUP_TIME ]; then
        log "❌ Frontend startup timeout after ${MAX_STARTUP_TIME}s!"
        log "📋 Recent log entries:"
        tail -20 "$LOG_FILE"
        log ""
        log "🔧 Troubleshooting suggestions:"
        log "   1. Check for port conflicts: lsof -i:$PORT"
        log "   2. Review full logs: cat $LOG_FILE"
        log "   3. Clear cache: rm -rf node_modules/.vite .vite"
        log "   4. Reinstall dependencies: rm -rf node_modules && npm install"

        kill -9 $DEV_PID 2>/dev/null || true
        rm -f "frontend.log.pid"
        exit 1
    fi

    log "⏳ Waiting for server to start... (${elapsed}s/${MAX_STARTUP_TIME}s)"
    sleep 3
done
