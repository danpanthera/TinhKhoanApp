#!/bin/bash

# Improved Backend Startup Script
LOG_FILE="backend.log"
PORT=5055
MAX_RETRIES=3
RETRY_DELAY=2
TIMEOUT=60

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
  
  # Also check for any dotnet processes that might be running the API
  for pid in $(ps aux | grep 'dotnet run' | grep -v grep | awk '{print $2}'); do
    log "Killing dotnet process $pid"
    kill -15 $pid 2>/dev/null || kill -9 $pid 2>/dev/null
    sleep 1
  done
}

# Clear log file at start
> "$LOG_FILE"

log "Starting Backend API Server on port $PORT"

# Clean up before starting
cleanup

# Check if project needs to be built
if [[ ! -d "bin" || ! -d "obj" ]]; then
  log "Building project first..."
  dotnet build >> "$LOG_FILE" 2>&1
  
  # Check build status
  if [ $? -ne 0 ]; then
    log "❌ Build failed. Check $LOG_FILE for details."
    exit 1
  fi
  log "✅ Build completed successfully."
fi

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
  
  log "Starting API with: dotnet run --urls=http://localhost:$PORT"
  dotnet run --urls=http://localhost:$PORT >> "$LOG_FILE" 2>&1 &
  api_pid=$!
  
  # Wait for the API to start up
  start_time=$(date +%s)
  while true; do
    current_time=$(date +%s)
    elapsed=$((current_time - start_time))
    
    if [ $elapsed -gt $TIMEOUT ]; then
      log "❌ Timeout waiting for API to start. Check $LOG_FILE for details."
      kill -15 $api_pid 2>/dev/null || kill -9 $api_pid 2>/dev/null
      break
    fi
    
    # Check if the process is still running
    if ! kill -0 $api_pid 2>/dev/null; then
      log "❌ API process terminated unexpectedly. Check $LOG_FILE for details."
      break
    fi
    
    # Check if API is responding
    if curl -s http://localhost:$PORT/health >/dev/null 2>&1 || curl -s http://localhost:$PORT/api/health >/dev/null 2>&1 || curl -s http://localhost:$PORT/api/v1/health >/dev/null 2>&1; then
      log "✅ API started successfully on http://localhost:$PORT"
      log "Logs are being written to: $LOG_FILE"
      exit 0
    fi
    
    # Check for common errors in log
    if grep -q "Error:" "$LOG_FILE" || grep -q "Exception:" "$LOG_FILE"; then
      error_line=$(grep -m 1 -E "Error:|Exception:" "$LOG_FILE")
      log "⚠️ Potential issue detected: $error_line"
      # Don't exit - many errors are non-fatal
    fi
    
    sleep 2
    log "Waiting for API to start... (${elapsed}s/$TIMEOUT)"
  done
  
  log "API didn't start properly. Retrying..."
  cleanup
  retry_count=$((retry_count + 1))
done

log "❌ Failed to start the API after $MAX_RETRIES attempts. Check $LOG_FILE for details."
exit 1
