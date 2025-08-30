#!/bin/bash
# 🚀 Quick Start Script - Khởi động toàn bộ hệ thống

echo "🚀 Quick Start Script - Khởi động hệ thống Khoan App..."

# Màu sắc
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Hàm log với màu
log_info() { echo -e "${BLUE}ℹ️  $1${NC}"; }
log_success() { echo -e "${GREEN}✅ $1${NC}"; }
log_warning() { echo -e "${YELLOW}⚠️  $1${NC}"; }
log_error() { echo -e "${RED}❌ $1${NC}"; }

# Lấy đường dẫn project
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BACKEND_DIR="$SCRIPT_DIR"
FRONTEND_DIR="/opt/Projects/Khoan/Frontend/KhoanUI"

log_info "Backend: $BACKEND_DIR"
log_info "Frontend: $FRONTEND_DIR"

# 1. Kiểm tra dependencies
log_info "Kiểm tra dependencies..."

# Kiểm tra .NET
if ! command -v dotnet &> /dev/null; then
    log_error ".NET Core không được cài đặt"
    exit 1
fi
log_success ".NET Core: $(dotnet --version)"

# Kiểm tra Node.js
if ! command -v node &> /dev/null; then
    log_error "Node.js không được cài đặt"
    exit 1
fi
log_success "Node.js: $(node --version)"

# Kiểm tra npm
if ! command -v npm &> /dev/null; then
    log_error "npm không được cài đặt"
    exit 1
fi
log_success "npm: $(npm --version)"

# 2. Dừng các process cũ
log_info "Dừng các process cũ..."
pkill -f "dotnet.*KhoanApp" 2>/dev/null || true
lsof -ti:5055 | xargs kill -9 2>/dev/null || true
lsof -ti:5173 | xargs kill -9 2>/dev/null || true
lsof -ti:3000 | xargs kill -9 2>/dev/null || true
log_success "Đã dừng các process cũ"

# 3. Khởi động Backend
log_info "Khởi động Backend API..."
cd "$BACKEND_DIR"

# Restore packages nếu cần
if [[ ! -d "bin" ]] || [[ ! -d "obj" ]]; then
    log_info "Restoring .NET packages..."
    dotnet restore
fi

# Build
log_info "Building Backend..."
if ! dotnet build --configuration Release; then
    log_error "Backend build thất bại"
    exit 1
fi

# Chạy backend trong background
log_info "Khởi động Backend API trên port 5055..."
nohup dotnet run --urls=http://localhost:5055 > /tmp/backend.log 2>&1 &
BACKEND_PID=$!
log_success "Backend đã khởi động (PID: $BACKEND_PID)"

# Đợi backend khởi động
sleep 3

# Kiểm tra backend
if curl -s -f "http://localhost:5055/api/GeneralDashboard/test" > /dev/null; then
    log_success "Backend API đã sẵn sàng"
else
    log_warning "Backend API chưa sẵn sàng, đợi thêm..."
    sleep 2
fi

# 4. Khởi động Frontend
log_info "Khởi động Frontend..."

# Kiểm tra thư mục Frontend tồn tại
if [[ ! -d "$FRONTEND_DIR" ]]; then
    log_error "Không tìm thấy thư mục Frontend: $FRONTEND_DIR"
    exit 1
fi

cd "$FRONTEND_DIR"

# Install dependencies nếu cần
if [[ ! -d "node_modules" ]]; then
    log_info "Installing npm packages..."
    npm install
fi

# Chạy frontend trong background
log_info "Khởi động Frontend dev server..."
nohup npm run dev > /tmp/frontend.log 2>&1 &
FRONTEND_PID=$!
log_success "Frontend đã khởi động (PID: $FRONTEND_PID)"

# Đợi frontend khởi động và kiểm tra port thực tế
sleep 3
FRONTEND_PORT=""
if curl -s -f "http://localhost:5173" > /dev/null 2>&1; then
    FRONTEND_PORT="5173"
elif curl -s -f "http://localhost:3000" > /dev/null 2>&1; then
    FRONTEND_PORT="3000"
elif curl -s -f "http://localhost:4173" > /dev/null 2>&1; then
    FRONTEND_PORT="4173"
fi

if [[ -n "$FRONTEND_PORT" ]]; then
    log_success "Frontend đã sẵn sàng trên port $FRONTEND_PORT"
else
    log_warning "Frontend đang khởi động, kiểm tra log: tail -f /tmp/frontend.log"
fi

# 5. Tóm tắt
echo ""
log_success "🎉 Hệ thống đã khởi động hoàn tất!"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "🔗 Backend API:  http://localhost:5055"
if [[ -n "$FRONTEND_PORT" ]]; then
    echo "🔗 Frontend App: http://localhost:$FRONTEND_PORT"
else
    echo "🔗 Frontend App: đang khởi động... (kiểm tra log)"
fi
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📋 Process IDs:"
echo "   Backend PID:  $BACKEND_PID"
echo "   Frontend PID: $FRONTEND_PID"
echo ""
echo "📄 Logs:"
echo "   Backend:  tail -f /tmp/backend.log"
echo "   Frontend: tail -f /tmp/frontend.log"
echo ""
echo "🛑 Để dừng hệ thống:"
echo "   kill $BACKEND_PID $FRONTEND_PID"
echo "   pkill -f 'dotnet.*KhoanApp'"
echo ""

# Mở browser (tùy chọn)
if command -v open &> /dev/null && [[ -n "$FRONTEND_PORT" ]]; then
    log_info "Mở browser sau 3 giây..."
    sleep 3
    open "http://localhost:$FRONTEND_PORT"
elif command -v open &> /dev/null; then
    log_info "Mở browser với port mặc định sau 5 giây..."
    sleep 5
    open "http://localhost:3000" || open "http://localhost:5173"
fi
