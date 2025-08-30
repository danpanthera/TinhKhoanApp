#!/bin/bash
# Khoan Application Master Startup Script
# Starts both Backend and Frontend services

# UTF-8 Configuration
export LANG=vi_VN.UTF-8
export LC_ALL=vi_VN.UTF-8

log() {
    echo "$(date '+%Y-%m-%d %H:%M:%S') - $1"
}

log "🚀 Starting Khoan Application Services..."
log "========================================"

# Get project root (parent of Backend directory)
PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && cd .. && pwd)"
BACKEND_DIR="$PROJECT_ROOT/Backend/KhoanApp.Api"
FRONTEND_DIR="$PROJECT_ROOT/Frontend/KhoanUI"

# Step 1: Start Backend
log "📋 Step 1: Starting Backend API..."
cd "$BACKEND_DIR"
if ./start_backend.sh; then
    log "✅ Backend started successfully"
else
    log "❌ Backend startup failed!"
    exit 1
fi

log ""

# Step 2: Start Frontend
log "📋 Step 2: Starting Frontend Development Server..."
cd "$FRONTEND_DIR"
if ./start_frontend.sh; then
    log "✅ Frontend started successfully"
else
    log "❌ Frontend startup failed!"
    exit 1
fi

log ""
log "🎉 Khoan Application Services Started Successfully!"
log "========================================"
log "🔗 Backend API: http://localhost:5055"
log "🔗 Frontend UI: http://localhost:3000"
log "🔍 Health Check: http://localhost:5055/api/health"
log ""
log "📝 Service Management:"
log "   Backend PID: $(cat "$BACKEND_DIR/backend.log.pid" 2>/dev/null || echo 'N/A')"
log "   Frontend PID: $(cat "$FRONTEND_DIR/frontend.log.pid" 2>/dev/null || echo 'N/A')"
log ""
log "🛑 To stop services:"
log "   Backend: kill \$(cat $BACKEND_DIR/backend.log.pid)"
log "   Frontend: kill \$(cat $FRONTEND_DIR/frontend.log.pid)"
