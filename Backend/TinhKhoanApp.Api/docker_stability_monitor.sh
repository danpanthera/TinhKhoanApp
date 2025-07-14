#!/bin/bash

# 🐳 Docker Stability Monitor & Auto-Restart Script
# Đảm bảo Azure SQL Edge container hoạt động ổn định

CONTAINER_NAME="azure_sql_edge_tinhkhoan"
DB_NAME="TinhKhoanDB"
SA_PASSWORD="YourStrong@Password123"

echo "🔍 Kiểm tra tình trạng Docker container..."

# Hàm kiểm tra container có đang chạy không
check_container_running() {
    if docker ps --format "table {{.Names}}" | grep -q "^${CONTAINER_NAME}$"; then
        echo "✅ Container ${CONTAINER_NAME} đang chạy"
        return 0
    else
        echo "❌ Container ${CONTAINER_NAME} không chạy"
        return 1
    fi
}

# Hàm kiểm tra SQL Server có phản hồi không
check_sql_server_ready() {
    echo "🔄 Kiểm tra SQL Server..."
    if sqlcmd -S localhost,1433 -U sa -P "${SA_PASSWORD}" -C -Q "SELECT @@VERSION" &>/dev/null; then
        echo "✅ SQL Server phản hồi bình thường"
        return 0
    else
        echo "❌ SQL Server không phản hồi"
        return 1
    fi
}

# Hàm khởi động lại container với cấu hình tối ưu
restart_container() {
    echo "🔄 Khởi động lại container với cấu hình tối ưu..."

    # Dừng và xóa container cũ
    docker stop ${CONTAINER_NAME} 2>/dev/null || true
    docker rm ${CONTAINER_NAME} 2>/dev/null || true

    # Khởi động container mới với memory limits và auto-restart
    docker run -e "ACCEPT_EULA=Y" \
               -e "MSSQL_SA_PASSWORD=${SA_PASSWORD}" \
               -e "MSSQL_PID=Developer" \
               -e "MSSQL_MEMORY_LIMIT_MB=3072" \
               -p 1433:1433 \
               --name ${CONTAINER_NAME} \
               --memory=4g \
               --memory-swap=6g \
               --restart=unless-stopped \
               --health-cmd="sqlcmd -S localhost -U sa -P '${SA_PASSWORD}' -Q 'SELECT 1'" \
               --health-interval=30s \
               --health-timeout=10s \
               --health-retries=3 \
               -d mcr.microsoft.com/azure-sql-edge:latest

    echo "⏳ Đợi SQL Server khởi động..."
    sleep 15

    # Tạo database nếu chưa tồn tại
    sqlcmd -S localhost,1433 -U sa -P "${SA_PASSWORD}" -C -Q "
        IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '${DB_NAME}')
        CREATE DATABASE ${DB_NAME};
    "

    echo "✅ Container và database đã sẵn sàng!"
}

# Hàm dọn dẹp Docker
cleanup_docker() {
    echo "🧹 Dọn dẹp Docker resources..."
    docker system prune -f
    echo "✅ Đã dọn dẹp xong!"
}

# Hàm hiển thị thông tin tài nguyên
show_resource_info() {
    echo "📊 Thông tin tài nguyên Docker:"
    docker system df
    echo ""
    echo "💾 Thông tin memory container:"
    docker stats ${CONTAINER_NAME} --no-stream 2>/dev/null || echo "Container chưa chạy"
}

# Main script logic
main() {
    echo "🚀 Bắt đầu kiểm tra Docker stability..."

    if ! check_container_running; then
        echo "🔧 Container không chạy, khởi động lại..."
        restart_container
    elif ! check_sql_server_ready; then
        echo "🔧 SQL Server không phản hồi, khởi động lại container..."
        restart_container
    else
        echo "✅ Hệ thống hoạt động bình thường!"
    fi

    show_resource_info

    echo ""
    echo "🎯 Hướng dẫn sử dụng:"
    echo "   - Chạy script này khi gặp vấn đề Docker"
    echo "   - Container có auto-restart policy 'unless-stopped'"
    echo "   - Memory limit: 4GB, swap: 6GB"
    echo "   - Health check mỗi 30 giây"
    echo ""
    echo "🔗 Test kết nối:"
    echo "   sqlcmd -S localhost,1433 -U sa -P '${SA_PASSWORD}' -C -d ${DB_NAME}"
}

# Nếu script được chạy trực tiếp
if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    main "$@"
fi
