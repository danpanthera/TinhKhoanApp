#!/bin/bash

# 🔧 Docker Troubleshooting & Root Cause Analysis
# Phân tích và khắc phục nguyên nhân Docker container tự động dừng

CONTAINER_NAME="azure_sql_edge_tinhkhoan"
LOG_FILE="/tmp/docker_analysis_$(date +%Y%m%d_%H%M%S).log"

echo "🔍 DOCKER TROUBLESHOOTING & ROOT CAUSE ANALYSIS" | tee "$LOG_FILE"
echo "=================================================" | tee -a "$LOG_FILE"
echo "Thời gian: $(date)" | tee -a "$LOG_FILE"
echo "" | tee -a "$LOG_FILE"

# 1. KIỂM TRA TÀI NGUYÊN HỆ THỐNG
echo "📊 1. PHÂN TÍCH TÀI NGUYÊN HỆ THỐNG:" | tee -a "$LOG_FILE"
echo "-----------------------------------" | tee -a "$LOG_FILE"

# Memory usage
echo "💾 Memory Usage:" | tee -a "$LOG_FILE"
vm_stat | head -5 | tee -a "$LOG_FILE"
echo "" | tee -a "$LOG_FILE"

# Disk space
echo "💽 Disk Space:" | tee -a "$LOG_FILE"
df -h | head -5 | tee -a "$LOG_FILE"
echo "" | tee -a "$LOG_FILE"

# Docker resources
echo "🐳 Docker Resource Usage:" | tee -a "$LOG_FILE"
docker system df 2>/dev/null | tee -a "$LOG_FILE"
echo "" | tee -a "$LOG_FILE"

# 2. KIỂM TRA DOCKER LOGS
echo "📋 2. PHÂN TÍCH DOCKER LOGS:" | tee -a "$LOG_FILE"
echo "-----------------------------" | tee -a "$LOG_FILE"

if docker ps -a --format "table {{.Names}}" | grep -q "$CONTAINER_NAME"; then
    echo "Container logs (last 50 lines):" | tee -a "$LOG_FILE"
    docker logs "$CONTAINER_NAME" --tail 50 2>&1 | tee -a "$LOG_FILE"
    echo "" | tee -a "$LOG_FILE"

    echo "Container inspection:" | tee -a "$LOG_FILE"
    docker inspect "$CONTAINER_NAME" --format='{{json .State}}' 2>/dev/null | jq . 2>/dev/null | tee -a "$LOG_FILE"
    echo "" | tee -a "$LOG_FILE"
else
    echo "❌ Container $CONTAINER_NAME không tồn tại" | tee -a "$LOG_FILE"
fi

# 3. PHÂN TÍCH NGUYÊN NHÂN
echo "🔍 3. PHÂN TÍCH NGUYÊN NHÂN:" | tee -a "$LOG_FILE"
echo "----------------------------" | tee -a "$LOG_FILE"

# Check for OOM (Out of Memory)
if docker logs "$CONTAINER_NAME" 2>&1 | grep -i "killed\|oom\|memory\|out of memory" | head -5; then
    echo "⚠️  NGUYÊN NHÂN: Out of Memory (OOM)" | tee -a "$LOG_FILE"
    echo "   - Container bị kill do thiếu memory" | tee -a "$LOG_FILE"
    echo "   - Giải pháp: Tăng memory limit hoặc giảm memory usage" | tee -a "$LOG_FILE"
    OOM_DETECTED=true
else
    OOM_DETECTED=false
fi

# Check for disk space issues
DISK_USAGE=$(df / | awk 'NR==2 {print $5}' | sed 's/%//')
if [ "$DISK_USAGE" -gt 90 ]; then
    echo "⚠️  NGUYÊN NHÂN: Disk Space đầy" | tee -a "$LOG_FILE"
    echo "   - Disk usage: ${DISK_USAGE}%" | tee -a "$LOG_FILE"
    echo "   - Giải pháp: Dọn dẹp disk space" | tee -a "$LOG_FILE"
    DISK_FULL=true
else
    DISK_FULL=false
fi

# Check for SQL Server specific errors
if docker logs "$CONTAINER_NAME" 2>&1 | grep -i "error\|fail\|crash\|exception" | head -5; then
    echo "⚠️  NGUYÊN NHÂN: SQL Server Errors" | tee -a "$LOG_FILE"
    echo "   - SQL Server gặp lỗi internal" | tee -a "$LOG_FILE"
    echo "   - Giải pháp: Restart với configuration tối ưu" | tee -a "$LOG_FILE"
    SQL_ERROR=true
else
    SQL_ERROR=false
fi

echo "" | tee -a "$LOG_FILE"

# 4. GIẢI PHÁP TỐI ÍU
echo "💡 4. GIẢI PHÁP TỐI ƯU:" | tee -a "$LOG_FILE"
echo "-----------------------" | tee -a "$LOG_FILE"

echo "🔧 Cấu hình Docker container tối ưu:" | tee -a "$LOG_FILE"
echo "   - Memory limit: 4GB (hard limit)" | tee -a "$LOG_FILE"
echo "   - Swap limit: 6GB (cho buffer)" | tee -a "$LOG_FILE"
echo "   - Restart policy: unless-stopped" | tee -a "$LOG_FILE"
echo "   - Health check: 30s interval" | tee -a "$LOG_FILE"
echo "   - SQL Server memory: 3GB max" | tee -a "$LOG_FILE"
echo "" | tee -a "$LOG_FILE"

echo "🧹 Cleanup actions needed:" | tee -a "$LOG_FILE"
if [ "$DISK_FULL" = true ]; then
    echo "   ✅ Dọn dẹp disk space" | tee -a "$LOG_FILE"
fi
echo "   ✅ Docker system prune" | tee -a "$LOG_FILE"
echo "   ✅ Remove old containers/images" | tee -a "$LOG_FILE"
echo "" | tee -a "$LOG_FILE"

# 5. TỰ ĐỘNG KHẮC PHỤC
echo "🛠️  5. TỰ ĐỘNG KHẮC PHỤC:" | tee -a "$LOG_FILE"
echo "-------------------------" | tee -a "$LOG_FILE"

echo "Đang thực hiện cleanup..." | tee -a "$LOG_FILE"

# Stop và remove container cũ
docker stop "$CONTAINER_NAME" 2>/dev/null || true
docker rm "$CONTAINER_NAME" 2>/dev/null || true

# Docker cleanup
echo "Cleaning up Docker resources..." | tee -a "$LOG_FILE"
docker system prune -f 2>&1 | tee -a "$LOG_FILE"

# Remove old SQL Server images (keep latest)
echo "Removing old Azure SQL Edge images..." | tee -a "$LOG_FILE"
docker images mcr.microsoft.com/azure-sql-edge --format "table {{.Repository}}:{{.Tag}}\t{{.ID}}" | \
    tail -n +2 | head -n -1 | awk '{print $2}' | xargs -r docker rmi 2>/dev/null || true

echo "" | tee -a "$LOG_FILE"

# 6. KHỞI ĐỘNG LẠI VỚI CẤU HÌNH TỐI ƯU
echo "🚀 6. KHỞI ĐỘNG LẠI VỚI CẤU HÌNH TỐI ƯU:" | tee -a "$LOG_FILE"
echo "-------------------------------------------" | tee -a "$LOG_FILE"

echo "Đang khởi động container với cấu hình tối ưu..." | tee -a "$LOG_FILE"

docker run -e "ACCEPT_EULA=Y" \
           -e "MSSQL_SA_PASSWORD=YourStrong@Password123" \
           -e "MSSQL_PID=Developer" \
           -e "MSSQL_MEMORY_LIMIT_MB=3072" \
           -e "MSSQL_COLLATION=SQL_Latin1_General_CP1_CI_AS" \
           -p 1433:1433 \
           --name "$CONTAINER_NAME" \
           --memory=4g \
           --memory-swap=6g \
           --cpus=2 \
           --restart=unless-stopped \
           --health-cmd="sqlcmd -S localhost -U sa -P 'YourStrong@Password123' -Q 'SELECT 1' -b" \
           --health-interval=30s \
           --health-timeout=10s \
           --health-retries=5 \
           --health-start-period=60s \
           -d mcr.microsoft.com/azure-sql-edge:latest 2>&1 | tee -a "$LOG_FILE"

echo "" | tee -a "$LOG_FILE"
echo "⏳ Đợi container khởi động..." | tee -a "$LOG_FILE"
sleep 20

# 7. VERIFICATION
echo "✅ 7. KIỂM TRA KẾT QUẢ:" | tee -a "$LOG_FILE"
echo "-----------------------" | tee -a "$LOG_FILE"

if docker ps --format "table {{.Names}}" | grep -q "$CONTAINER_NAME"; then
    echo "✅ Container đang chạy!" | tee -a "$LOG_FILE"

    # Test SQL connection
    echo "🔌 Testing SQL connection..." | tee -a "$LOG_FILE"
    if sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -Q "SELECT @@VERSION" &>/dev/null; then
        echo "✅ SQL Server phản hồi bình thường!" | tee -a "$LOG_FILE"

        # Create database if not exists
        sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -Q "
            IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TinhKhoanDB')
            CREATE DATABASE TinhKhoanDB;
        " 2>&1 | tee -a "$LOG_FILE"

        echo "✅ Database TinhKhoanDB sẵn sàng!" | tee -a "$LOG_FILE"
    else
        echo "❌ SQL Server chưa phản hồi" | tee -a "$LOG_FILE"
    fi

    # Show container stats
    echo "" | tee -a "$LOG_FILE"
    echo "📊 Container statistics:" | tee -a "$LOG_FILE"
    docker stats "$CONTAINER_NAME" --no-stream 2>/dev/null | tee -a "$LOG_FILE"

else
    echo "❌ Container không chạy được!" | tee -a "$LOG_FILE"
    echo "Kiểm tra logs để debug thêm:" | tee -a "$LOG_FILE"
    docker logs "$CONTAINER_NAME" --tail 20 2>&1 | tee -a "$LOG_FILE"
fi

echo "" | tee -a "$LOG_FILE"
echo "📄 Log file saved to: $LOG_FILE" | tee -a "$LOG_FILE"
echo "🎯 Để monitor liên tục, chạy: watch 'docker stats $CONTAINER_NAME --no-stream'" | tee -a "$LOG_FILE"
