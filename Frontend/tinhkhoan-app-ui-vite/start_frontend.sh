#!/bin/bash

# Improved Frontend Startup Script - fixed to use port 3000

# UTF-8 Configuration
export LANG=vi_VN.UTF-8
export LC_ALL=vi_VN.UTF-8

LOG_FILE="frontend.log"
PORT=3000
MAX_RETRIES=3
RETRY_DELAY=2

# Function to log messages to console and file
log() {
  echo "$(date '+%Y-%m-%d %H:%M:%S') - $1" | tee -a "$LOG_FILE"
}

# Function to check if port is in use
is_port_in_use() {
  lsof -i :"$1" >/dev/null 2>&1
  return $?
}

# Function to clean up processes
cleanup() {
  log "Cleaning up any existing processes..."

  # Find and kill any existing processes using PORT
  if is_port_in_use $PORT; then
    log "Port $PORT is in use. Attempting to free it..."
    for pid in $(lsof -t -i:$PORT); do
      log "Killing process $pid using port $PORT"
      kill -15 $pid 2>/dev/null || kill -9 $pid 2>/dev/null
      sleep 1
    done
  fi

  # Also check for any npm processes that might be running the dev server
  for pid in $(ps aux | grep 'npm run dev' | grep -v grep | awk '{print $2}'); do
    log "Killing npm process $pid"
    kill -15 $pid 2>/dev/null || kill -9 $pid 2>/dev/null
    sleep 1
  done
}

# Clear log file at start
> "$LOG_FILE"

log "Starting Frontend Development Server on port $PORT"

# Clean up before starting
cleanup

# Retry mechanism for starting the server
retry_count=0
while [ $retry_count -lt $MAX_RETRIES ]; do
  if is_port_in_use $PORT; then
    log "Port $PORT is still in use after cleanup. Retrying in $RETRY_DELAY seconds..."
    sleep $RETRY_DELAY
    cleanup
    retry_count=$((retry_count + 1))
    continue
  fi

  log "Starting dev server with: npm run dev -- --port $PORT"
  npm run dev -- --port $PORT >> "$LOG_FILE" 2>&1 &

  # Check if server started successfully
  attempt=0
  max_attempts=15
  while [ $attempt -lt $max_attempts ]; do
    sleep 2
    if grep -q "Local:" "$LOG_FILE"; then
      server_url=$(grep "Local:" "$LOG_FILE" | tail -1 | awk '{print $2}')
      log "✅ Dev server started successfully! Available at: $server_url"
      log "Logs are being written to: $LOG_FILE"
      exit 0
    fi

    # Check for common errors
    if grep -q "error" "$LOG_FILE" || grep -q "Error:" "$LOG_FILE"; then
      log "❌ Error detected while starting the server. Check $LOG_FILE for details."
      break
    fi

    attempt=$((attempt + 1))
    log "Waiting for server to start... ($attempt/$max_attempts)"
  done

  log "Server didn't start properly. Retrying..."
  cleanup
  retry_count=$((retry_count + 1))
done

log "❌ Failed to start the dev server after $MAX_RETRIES attempts. Check $LOG_FILE for details."
exit 1
