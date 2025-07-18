#!/bin/bash

# Script tối ưu Azure SQL Edge container MÀ KHÔNG MẤT DATA
# Sử dụng existing volume: sqldata_tinhkhoan
# Created: July 18, 2025

echo "🔧 [CONTAINER OPTIMIZATION] Bắt đầu tối ưu database container (SAFE MODE)..."

# Kiểm tra volume hiện tại
echo "📊 [VOLUME] Kiểm tra volume dữ liệu hiện tại..."
docker volume inspect sqldata_tinhkhoan

# Kiểm tra container hiện tại
echo "📊 [CHECK] Kiểm tra trạng thái container hiện tại..."
docker ps -a --filter name=azure_sql_edge_tinhkhoan

# Stop container nhẹ nhàng
echo "🛑 [STOP] Dừng container hiện tại..."
docker stop azure_sql_edge_tinhkhoan

# Commit container hiện tại để backup (safety)
echo "💾 [BACKUP] Tạo backup image của container hiện tại..."
docker commit azure_sql_edge_tinhkhoan azure_sql_edge_backup:$(date +%Y%m%d_%H%M%S)

# Xóa container cũ (chỉ container, KHÔNG XÓA VOLUME)
echo "🗑️ [REMOVE] Xóa container cũ để tạo mới với config tối ưu..."
docker rm azure_sql_edge_tinhkhoan

# Tạo container mới với config tối ưu và SỬ DỤNG VOLUME CŨ
echo "🚀 [CREATE] Tạo container mới với config tối ưu và volume hiện tại..."
docker run \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=YourStrong@Password123" \
  -e "MSSQL_PID=Developer" \
  -e "MSSQL_MEMORY_LIMIT_MB=2048" \
  -e "MSSQL_LCID=1033" \
  -e "MSSQL_COLLATION=SQL_Latin1_General_CP1_CI_AS" \
  -p 1433:1433 \
  --name azure_sql_edge_tinhkhoan \
  --memory=3g \
  --memory-swap=4g \
  --memory-swappiness=10 \
  --restart=unless-stopped \
  --health-cmd="sqlcmd -S localhost -U SA -P 'YourStrong@Password123' -Q 'SELECT 1' -b -t 1" \
  --health-interval=30s \
  --health-timeout=10s \
  --health-retries=3 \
  -v sqldata_tinhkhoan:/var/opt/mssql \
  -d mcr.microsoft.com/azure-sql-edge:latest

echo "⏳ [WAIT] Đợi container khởi động..."
sleep 40

# Kiểm tra container
echo "📊 [CHECK] Kiểm tra container mới..."
docker ps --filter name=azure_sql_edge_tinhkhoan

# Test kết nối
echo "🔌 [TEST] Kiểm tra kết nối database..."
for i in {1..15}; do
  if sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -Q "SELECT 1 as Test" -t 5; then
    echo "✅ [SUCCESS] Database kết nối thành công!"
    break
  else
    echo "⏳ [RETRY] Thử lại lần $i/15..."
    sleep 15
  fi
done

# Kiểm tra database TinhKhoanDB
echo "🗄️ [DATABASE] Kiểm tra database TinhKhoanDB..."
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -Q "SELECT name FROM sys.databases WHERE name = 'TinhKhoanDB'" -t 10

# Kiểm tra tables nếu database tồn tại
echo "📋 [TABLES] Kiểm tra số lượng tables trong TinhKhoanDB..."
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "SELECT COUNT(*) as TotalTables FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'" -t 10

# Kiểm tra core tables
echo "🎯 [CORE TABLES] Kiểm tra 8 bảng core data..."
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01') ORDER BY TABLE_NAME" -t 10

echo "🎉 [COMPLETE] Tối ưu container hoàn thành!"
echo "💡 [INFO] Container mới có:"
echo "   - Memory limit: 3GB (conservative)"
echo "   - Memory swap: 4GB" 
echo "   - Auto restart: unless-stopped"
echo "   - Health check: enabled"
echo "   - Data volume: sqldata_tinhkhoan (PRESERVED)"
echo "   - Backup created: azure_sql_edge_backup:$(date +%Y%m%d_%H%M%S)"
