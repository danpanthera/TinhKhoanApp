#!/bin/bash
# Simple supervised backend starter for Khoan.Api (enhanced with flags)
set -euo pipefail

LOG=backend.log
PORT=${PORT:-5055}
MAX_WAIT=60
PROJECT_DIR="$(cd "$(dirname "$0")" && pwd)/Khoan.Api"
PROJECT_FILE="$PROJECT_DIR/Khoan.Api.csproj"

# Flags
MONITOR_MODE=1         # 1 = keep monitoring; 0 = exit after healthy
QUICK_BUILD=0          # if 1, still build but exit after health OK
SKIP_BUILD=0           # if 1, skip build step entirely

# Parse args
while [[ $# -gt 0 ]]; do
  case "$1" in
    --quick-build)
      QUICK_BUILD=1
      MONITOR_MODE=0
      shift
      ;;
    --no-monitor|--detach|--once)
      MONITOR_MODE=0
      shift
      ;;
    --skip-build)
      SKIP_BUILD=1
      shift
      ;;
    -p|--port)
      PORT="$2"; shift 2
      ;;
    *)
      # ignore unknown flags for compatibility
      shift
      ;;
  esac
done

cd "$PROJECT_DIR"

echo "[INFO] Starting backend Khoan.Api on port $PORT" | tee -a "$LOG"

# Kill existing listeners on port
if lsof -tiTCP:$PORT -sTCP:LISTEN >/dev/null 2>&1; then
  echo "[INFO] Killing existing process on :$PORT" | tee -a "$LOG"
  for p in $(lsof -tiTCP:$PORT -sTCP:LISTEN); do kill -9 $p 2>/dev/null || true; done
fi

if [[ "$SKIP_BUILD" -eq 1 ]]; then
  echo "[INFO] Skip build as requested" | tee -a "$LOG"
else
  # Build (capture errors) - specify project to avoid MSB1011 when multiple projects exist
  if ! dotnet build "$PROJECT_FILE" -c Debug >> "$LOG" 2>&1; then
    echo "[ERROR] Build failed. Tail of log:" | tee -a "$LOG"
    tail -40 "$LOG"
    exit 1
  fi
  echo "[INFO] Build succeeded" | tee -a "$LOG"
fi

# Run in background, explicitly targeting the project and URL
ASPNETCORE_URLS="http://localhost:$PORT" dotnet run --no-build --project "$PROJECT_FILE" >> "$LOG" 2>&1 &
APP_PID=$!

echo $APP_PID > backend.pid

echo "[INFO] Backend PID $APP_PID" | tee -a "$LOG"

start_time=$(date +%s)
while true; do
  if ! kill -0 $APP_PID 2>/dev/null; then
    echo "[ERROR] Backend process exited prematurely" | tee -a "$LOG"
    tail -40 "$LOG"
    exit 1
  fi
  if lsof -nP -iTCP:$PORT -sTCP:LISTEN >/dev/null 2>&1; then
    # Health check: try multiple endpoints
    if curl -fsS http://localhost:$PORT/api/health >/dev/null 2>&1 || \
       curl -fsS http://localhost:$PORT/api/Health >/dev/null 2>&1 || \
       curl -fsS http://localhost:$PORT/health >/dev/null 2>&1; then
      echo "[OK] Backend listening on http://localhost:$PORT (health OK)" | tee -a "$LOG"
    else
      echo "[WARN] Backend listening but health endpoint not responding yet" | tee -a "$LOG"
    fi
    break
  fi
  now=$(date +%s)
  if [ $((now-start_time)) -ge $MAX_WAIT ]; then
    echo "[ERROR] Timeout waiting for backend to listen on :$PORT" | tee -a "$LOG"
    tail -60 "$LOG"
    exit 1
  fi
  echo "[WAIT] Waiting for port $PORT..." | tee -a "$LOG"
  sleep 2
done

if [[ "$MONITOR_MODE" -eq 0 ]]; then
  echo "[INFO] Startup complete. Detaching (no monitor mode). PID=$APP_PID" | tee -a "$LOG"
  exit 0
fi

echo "[INFO] Monitoring (Ctrl+C to stop)" | tee -a "$LOG"
trap 'echo "[INFO] Stopping backend PID $APP_PID" | tee -a "$LOG"; kill $APP_PID 2>/dev/null || true; wait $APP_PID 2>/dev/null || true; rm -f backend.pid; exit 0' INT TERM
wait $APP_PID
EXIT_CODE=$?
echo "[INFO] Backend exited with code $EXIT_CODE" | tee -a "$LOG"
rm -f backend.pid
exit $EXIT_CODE
