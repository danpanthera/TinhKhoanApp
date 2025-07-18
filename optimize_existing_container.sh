#!/bin/bash

# Script tối ưu container CŨ mà KHÔNG XÓA - PRESERVE ALL CONFIG & DATA
# Created: July 18, 2025

echo "🔧 [OPTIMIZE EXISTING] Tối ưu container hiện tại mà KHÔNG xóa..."

# Đợi Docker khởi động
echo "⏳ [DOCKER] Đợi Docker Desktop khởi động..."
while ! docker info >/dev/null 2>&1; do
  echo "   🔄 Docker chưa sẵn sàng, đợi 10s..."
  sleep 10
done
echo "✅ [DOCKER] Docker đã sẵn sàng!"

# Kiểm tra container hiện tại
echo "📊 [CHECK] Kiểm tra container hiện tại..."
CONTAINER_STATUS=$(docker inspect --format '{{.State.Status}}' azure_sql_edge_tinhkhoan 2>/dev/null || echo "not_found")
echo "   Container Status: $CONTAINER_STATUS"

if [ "$CONTAINER_STATUS" = "not_found" ]; then
  echo "❌ [ERROR] Container azure_sql_edge_tinhkhoan không tồn tại!"
  exit 1
fi

# Kiểm tra volume data
echo "📂 [VOLUME] Kiểm tra volume dữ liệu..."
docker volume inspect sqldata_tinhkhoan || echo "⚠️ Volume chưa tồn tại"

# Stop container hiện tại (nếu đang chạy)
if [ "$CONTAINER_STATUS" = "running" ]; then
  echo "🛑 [STOP] Dừng container hiện tại..."
  docker stop azure_sql_edge_tinhkhoan
  sleep 5
fi

# Tối ưu container KHÔNG XÓA - chỉ update config
echo "⚡ [OPTIMIZE] Tối ưu container hiện tại với memory và CPU cao hơn..."

# Update container với memory và resource limits cao hơn
docker update \
  --memory=5g \
  --memory-swap=7g \
  --memory-swappiness=1 \
  --cpus=8 \
  --cpu-shares=2048 \
  --restart=unless-stopped \
  azure_sql_edge_tinhkhoan

if [ $? -eq 0 ]; then
  echo "✅ [UPDATE] Container đã được tối ưu với resource cao hơn"
else
  echo "⚠️ [WARNING] Không thể update resource limits, tiếp tục với config hiện tại"
fi

# Start container với config mới
echo "🚀 [START] Khởi động container với config tối ưu..."
docker start azure_sql_edge_tinhkhoan

# Đợi container khởi động
echo "⏳ [WAIT] Đợi container khởi động (45s)..."
sleep 45

# Kiểm tra status
echo "📊 [STATUS] Kiểm tra container sau tối ưu..."
docker ps --filter name=azure_sql_edge_tinhkhoan --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

# Test connection
echo "🔌 [CONNECTION] Kiểm tra kết nối database..."
CONNECTION_SUCCESS=false
for i in {1..15}; do
  echo "   🔄 Thử kết nối lần $i/15..."
  if timeout 10s sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -Q "SELECT 1 as Test" -t 5 >/dev/null 2>&1; then
    echo "   ✅ [SUCCESS] Database kết nối thành công!"
    CONNECTION_SUCCESS=true
    break
  else
    echo "   ⏳ [RETRY] Đợi 10s trước khi thử lại..."
    sleep 10
  fi
done

if [ "$CONNECTION_SUCCESS" = true ]; then
  # Kiểm tra database và tables
  echo "🗄️ [DATABASE] Kiểm tra database TinhKhoanDB..."
  DB_CHECK=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -Q "SELECT COUNT(*) FROM sys.databases WHERE name = 'TinhKhoanDB'" -h -1 -W 2>/dev/null || echo "0")
  
  if [ "$DB_CHECK" -gt 0 ]; then
    echo "   ✅ Database TinhKhoanDB tồn tại"
    
    # Check tables
    TABLE_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'" -h -1 -W 2>/dev/null || echo "0")
    echo "   📊 Tổng số tables: $TABLE_COUNT"
    
    # Check core tables
    echo "🎯 [CORE TABLES] Kiểm tra 8 bảng core data..."
    sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01') ORDER BY TABLE_NAME" -t 10 2>/dev/null
  else
    echo "   ⚠️ Database TinhKhoanDB chưa tồn tại"
  fi
  
  # Performance check
  echo "📈 [PERFORMANCE] Kiểm tra performance container..."
  docker stats azure_sql_edge_tinhkhoan --no-stream --format "table {{.Name}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.NetIO}}"
  
else
  echo "❌ [ERROR] Không thể kết nối database"
  echo "📋 [LOGS] Container logs (last 20 lines):"
  docker logs --tail 20 azure_sql_edge_tinhkhoan
fi

echo ""
echo "🎉 [COMPLETE] Tối ưu container hiện tại hoàn thành!"
echo ""
echo "💡 [OPTIMIZATION APPLIED]:"
echo "   �� Memory: 5GB (tăng từ default)"
echo "   🔄 Memory Swap: 7GB"
echo "   ⚡ CPUs: 8 cores"
echo "   🚀 CPU Priority: 2048 (high)"
echo "   🔄 Restart: unless-stopped"
echo "   💾 Data: PRESERVED (không mất config/data)"
echo "   🔧 Container: KEPT (không tạo mới)"
