#!/bin/bash
# Enhanced Frontend Startup Script - Full Process Management

# UTF-8 Configuration
export LANG=vi_VN.UTF-8
export LC_ALL=vi_VN.UTF-8

LOG_FILE="frontend.log"
MAX_LOG_SIZE=${FRONTEND_MAX_LOG_SIZE:-5M}
LOG_BACKUPS=${FRONTEND_LOG_BACKUPS:-5}

parse_size_bytes() {
    local size=$1
    if [[ $size =~ ^([0-9]+)[Kk]$ ]]; then
        echo $(( BASH_REMATCH[1] * 1024 ))
    elif [[ $size =~ ^([0-9]+)[Mm]$ ]]; then
        echo $(( BASH_REMATCH[1] * 1024 * 1024 ))
    elif [[ $size =~ ^[0-9]+$ ]]; then
        echo $size
    else
        echo $((5*1024*1024))
    fi
}

rotate_log_if_needed() {
    local file=$1
    [ ! -f "$file" ] && return 0
    local max_bytes=$(parse_size_bytes "$MAX_LOG_SIZE")
    local current_size=$(wc -c < "$file" 2>/dev/null || echo 0)
    if [ "$current_size" -ge "$max_bytes" ]; then
        for (( i=LOG_BACKUPS-1; i>=1; i-- )); do
            if [ -f "${file}.${i}" ]; then
                mv "${file}.${i}" "${file}.$((i+1))" 2>/dev/null || true
            fi
        done
        mv "$file" "${file}.1" 2>/dev/null || true
        touch "$file"
        echo "$(date '+%Y-%m-%d %H:%M:%S') - 🔄 Rotated $file" >> "$file"
    fi
}
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

# Rotate instead of truncating
rotate_log_if_needed "$LOG_FILE"
touch "$LOG_FILE"
# Start each run with a fresh log to avoid stale readiness matches
: > "$LOG_FILE"

log "🚀 Starting Khoan Frontend Development Server (rotation: size>$MAX_LOG_SIZE keep $LOG_BACKUPS)"
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
# Start in background (no nohup) and keep this script alive to avoid VS Code task manager
# terminating the orphaned process when the script exits instantly.
npm run dev -- --port $PORT >> "$LOG_FILE" 2>&1 &
DEV_PID=$!
echo $DEV_PID > "frontend.log.pid"
log "🔗 Dev process started (PID will be supervised by this script)"

log "🆔 Frontend PID: $DEV_PID"

# Step 5: Monitor startup with detailed feedback
log "🔍 Monitoring server startup..."
start_time=$(date +%s)

trap 'log "🛑 Termination signal received. Stopping frontend (PID $DEV_PID)..."; kill $DEV_PID 2>/dev/null || true; wait $DEV_PID 2>/dev/null || true; log "✅ Frontend stopped."; rm -f frontend.log.pid; exit 0' INT TERM

while true; do
    current_time=$(date +%s)
    elapsed=$((current_time - start_time))

    # Check if process is still running
    if ! kill -0 $DEV_PID 2>/dev/null; then
        log "❌ Frontend process died unexpectedly!"
        log "📋 Recent log entries:"
        tail -40 "$LOG_FILE"
        exit 1
    fi

    # Consider server ready when either port is listening or HTTP responds
    if lsof -nP -iTCP:$PORT -sTCP:LISTEN >/dev/null 2>&1; then
    log "✅ Frontend server started successfully!"
    log "🔗 Dev Server: http://localhost:$PORT"
    log "📝 Logs: $LOG_FILE"
    log "🆔 PID File: frontend.log.pid"
    log "🎉 Entering monitor mode (script will keep running to keep process alive)."
    break
    fi
    if curl -s -I http://localhost:$PORT/ >/dev/null 2>&1; then
    log "✅ Frontend server responded on http://localhost:$PORT"
    log "📝 Logs: $LOG_FILE"
    log "🆔 PID File: frontend.log.pid"
    log "🎉 Entering monitor mode (script will keep running to keep process alive)."
    break
    fi

    # Check for errors
    if grep -qi "error\|failed\|cannot" "$LOG_FILE" 2>/dev/null; then
        log "❌ Error detected during startup!"
        log "📋 Recent error logs:"
        grep -i "error\|failed\|cannot" "$LOG_FILE" | tail -10
        kill -9 $DEV_PID 2>/dev/null || true
        rm -f "frontend.log.pid"
        exit 1
    fi

    # Timeout check
    if [ $elapsed -ge $MAX_STARTUP_TIME ]; then
        log "❌ Frontend startup timeout after ${MAX_STARTUP_TIME}s!"
        log "📋 Recent log entries:"
        tail -50 "$LOG_FILE"
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

# Passive monitor: wait on the dev process and report exit.
log "🩺 Monitoring frontend process (PID $DEV_PID). Press Ctrl+C to stop."
wait $DEV_PID
EXIT_CODE=$?
log "⚠️ Frontend process exited with code $EXIT_CODE"
rm -f frontend.log.pid
exit $EXIT_CODE
