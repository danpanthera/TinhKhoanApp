#!/bin/bash

# 🚀 KhoanApp Full Stack Auto-Start Script
# Khởi động Docker, Backend và Frontend một cách tự động và ổn định

PROJECT_ROOT="/opt/Projects/Khoan"
BACKEND_DIR="${PROJECT_ROOT}/Backend/KhoanApp.Api"
FRONTEND_DIR="${PROJECT_ROOT}/Frontend/KhoanUI"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Step 1: Ensure Docker is stable
ensure_docker_stability() {
    log_info "🐳 Kiểm tra và đảm bảo Docker container ổn định..."

    cd "${BACKEND_DIR}"
    if ./docker_stability_monitor.sh; then
        log_success "Docker container hoạt động ổn định!"
        return 0
    else
        log_error "Không thể khởi động Docker container!"
        return 1
    fi
}

# Step 2: Run Entity Framework migrations
run_migrations() {
    log_info "🗄️ Chạy Entity Framework migrations..."

    cd "${BACKEND_DIR}"
    if dotnet ef database update; then
        log_success "Database migrations hoàn tất!"
        return 0
    else
        log_warning "Migrations gặp vấn đề, có thể database đã up-to-date"
        return 0  # Continue anyway
    fi
}

# Step 3: Start Backend
start_backend() {
    log_info "🔧 Khởi động Backend API..."

    cd "${BACKEND_DIR}"

    # Kill existing backend processes
    pkill -f "dotnet.*KhoanApp.Api" 2>/dev/null || true

    # Start backend in background
    nohup ./start_backend.sh > backend.log 2>&1 &
    BACKEND_PID=$!

    log_info "Backend đang khởi động... PID: ${BACKEND_PID}"

    # Wait for backend to be ready
    log_info "⏳ Đợi Backend API sẵn sàng..."
    for i in {1..30}; do
        if curl -s http://localhost:5055/health &>/dev/null; then
            log_success "Backend API đã sẵn sàng! (http://localhost:5055)"
            return 0
        fi
        echo -n "."
        sleep 2
    done

    log_error "Backend không phản hồi sau 60 giây!"
    return 1
}

# Step 4: Start Frontend
start_frontend() {
    log_info "🎨 Khởi động Frontend..."

    cd "${FRONTEND_DIR}"

    # Kill existing frontend processes
    pkill -f "vite.*dev" 2>/dev/null || true
    pkill -f "node.*vite" 2>/dev/null || true

    # Start frontend in background
    nohup ./start_frontend.sh > frontend.log 2>&1 &
    FRONTEND_PID=$!

    log_info "Frontend đang khởi động... PID: ${FRONTEND_PID}"

    # Wait for frontend to be ready
    log_info "⏳ Đợi Frontend dev server sẵn sàng..."
    for i in {1..20}; do
        if curl -s http://localhost:3000 &>/dev/null; then
            log_success "Frontend dev server đã sẵn sàng! (http://localhost:3000)"
            return 0
        fi
        echo -n "."
        sleep 3
    done

    log_error "Frontend không phản hồi sau 60 giây!"
    return 1
}

# Step 5: Verify full stack
verify_full_stack() {
    log_info "🔍 Kiểm tra toàn bộ stack..."

    # Check Docker
    if docker ps --format "table {{.Names}}" | grep -q "azure_sql_edge_tinhkhoan"; then
        log_success "✅ Docker SQL Edge: Running"
    else
        log_error "❌ Docker SQL Edge: Not Running"
        return 1
    fi

    # Check Backend API
    if curl -s http://localhost:5055/health &>/dev/null; then
        log_success "✅ Backend API: http://localhost:5055"
    else
        log_error "❌ Backend API: Not responding"
        return 1
    fi

    # Check Frontend
    if curl -s http://localhost:3000 &>/dev/null; then
        log_success "✅ Frontend: http://localhost:3000"
    else
        log_error "❌ Frontend: Not responding"
        return 1
    fi

    log_success "🎉 Full stack đang hoạt động hoàn hảo!"
    return 0
}

# Step 6: Show status and next steps
show_status() {
    echo ""
    log_info "📊 Trạng thái ứng dụng:"
    echo "   🐳 Docker:    azure_sql_edge_tinhkhoan"
    echo "   🗄️  Database:  TinhKhoanDB (localhost:1433)"
    echo "   🔧 Backend:   http://localhost:5055"
    echo "   🎨 Frontend:  http://localhost:3000"
    echo ""
    log_info "📋 Logs:"
    echo "   Backend:  ${BACKEND_DIR}/backend.log"
    echo "   Frontend: ${FRONTEND_DIR}/frontend.log"
    echo ""
    log_info "🎯 Bước tiếp theo:"
    echo "   - Mở trình duyệt: http://localhost:3000"
    echo "   - Test API health: http://localhost:5055/health"
    echo "   - Kiểm tra logs nếu cần debug"
}

# Main function
main() {
    echo "🚀 KhoanApp Full Stack Auto-Start"
    echo "====================================="

    # Step 1: Docker stability
    if ! ensure_docker_stability; then
        log_error "Không thể khởi động Docker. Thoát!"
        exit 1
    fi

    # Wait a bit for database to be fully ready
    log_info "⏳ Đợi database sẵn sàng..."
    sleep 10

    # Step 2: Migrations
    run_migrations

    # Step 3: Backend
    if ! start_backend; then
        log_error "Không thể khởi động Backend. Kiểm tra logs!"
        exit 1
    fi

    # Step 4: Frontend
    if ! start_frontend; then
        log_error "Không thể khởi động Frontend. Kiểm tra logs!"
        exit 1
    fi

    # Step 5: Verification
    sleep 5  # Give services time to fully start
    if ! verify_full_stack; then
        log_error "Một số service không hoạt động đúng!"
        exit 1
    fi

    # Step 6: Show status
    show_status

    log_success "🎉 KhoanApp đã khởi động thành công!"
}

# Run if executed directly
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    main "$@"
fi
