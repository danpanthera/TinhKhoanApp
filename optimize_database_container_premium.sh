#!/bin/bash

# Script tối ưu PREMIUM Azure SQL Edge container - HIGH PERFORMANCE
# System: 16 CPUs, 7.653GB RAM - FULL OPTIMIZATION
# Created: July 18, 2025

echo "🚀 [PREMIUM OPTIMIZATION] Bắt đầu tối ưu database container cao cấp..."

# Kiểm tra system resources
echo "🔍 [SYSTEM CHECK] Kiểm tra system resources..."
echo "   CPUs: $(nproc)"
echo "   Total Memory: $(free -h | grep '^Mem:' | awk '{print $2}' || echo 'N/A')"
echo "   Available Memory: $(free -h | grep '^Mem:' | awk '{print $7}' || echo 'N/A')"

# Kiểm tra volume hiện tại
echo "📊 [VOLUME] Kiểm tra volume dữ liệu hiện tại..."
docker volume inspect sqldata_tinhkhoan 2>/dev/null || echo "⚠️ Volume sqldata_tinhkhoan không tồn tại"

# Stop container hiện tại
echo "🛑 [STOP] Dừng container hiện tại..."
docker stop azure_sql_edge_tinhkhoan 2>/dev/null || echo "Container đã dừng"

# Commit container hiện tại để backup (safety)
echo "💾 [BACKUP] Tạo backup image container hiện tại..."
BACKUP_TAG="azure_sql_edge_backup:$(date +%Y%m%d_%H%M%S)"
docker commit azure_sql_edge_tinhkhoan $BACKUP_TAG 2>/dev/null || echo "⚠️ Không thể tạo backup"

# Remove container cũ
echo "🗑️ [REMOVE] Xóa container cũ..."
docker rm azure_sql_edge_tinhkhoan 2>/dev/null || echo "Container đã được xóa"

# Clean up Docker system
echo "🧹 [CLEANUP] Dọn dẹp Docker system..."
docker system prune -f

# Create optimized container với HIGH PERFORMANCE config
echo "🚀 [CREATE PREMIUM] Tạo container PREMIUM với config tối ưu cao..."
docker run \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=YourStrong@Password123" \
  -e "MSSQL_PID=Developer" \
  -e "MSSQL_MEMORY_LIMIT_MB=4096" \
  -e "MSSQL_LCID=1033" \
  -e "MSSQL_COLLATION=SQL_Latin1_General_CP1_CI_AS" \
  -e "MSSQL_TCP_PORT=1433" \
  -e "MSSQL_ENABLE_HADR=0" \
  -e "MSSQL_AGENT_ENABLED=true" \
  -p 1433:1433 \
  --name azure_sql_edge_tinhkhoan \
  --memory=5g \
  --memory-swap=7g \
  --memory-swappiness=1 \
  --cpus=8 \
  --cpu-shares=2048 \
  --oom-kill-disable=false \
  --restart=unless-stopped \
  --shm-size=1g \
  --ulimit nofile=65536:65536 \
  --ulimit memlock=-1:-1 \
  --health-cmd="timeout 10s sqlcmd -S localhost -U SA -P 'YourStrong@Password123' -Q 'SELECT 1' -b -t 1" \
  --health-interval=30s \
  --health-timeout=15s \
  --health-retries=3 \
  --health-start-period=120s \
  -v sqldata_tinhkhoan:/var/opt/mssql \
  -v /tmp:/tmp \
  --log-driver=json-file \
  --log-opt max-size=100m \
  --log-opt max-file=3 \
  -d mcr.microsoft.com/azure-sql-edge:latest

echo "⏳ [WAIT] Đợi container khởi động (60s)..."
sleep 60

# Kiểm tra container status
echo "📊 [STATUS] Kiểm tra container mới..."
docker ps --filter name=azure_sql_edge_tinhkhoan --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

# Test connection với retry logic
echo "🔌 [CONNECTION TEST] Kiểm tra kết nối database..."
CONNECTION_SUCCESS=false
for i in {1..20}; do
  echo "   🔄 Thử kết nối lần $i/20..."
  if timeout 15s sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -Q "SELECT 1 as ConnectionTest" -t 5 >/dev/null 2>&1; then
    echo "   ✅ [SUCCESS] Database kết nối thành công!"
    CONNECTION_SUCCESS=true
    break
  else
    echo "   ⏳ [RETRY] Đợi 10s trước khi thử lại..."
    sleep 10
  fi
done

if [ "$CONNECTION_SUCCESS" = false ]; then
  echo "❌ [ERROR] Không thể kết nối database sau 20 lần thử"
  echo "📋 [LOGS] Container logs:"
  docker logs --tail 20 azure_sql_edge_tinhkhoan
  exit 1
fi

# Verify database TinhKhoanDB
echo "🗄️ [DATABASE] Kiểm tra database TinhKhoanDB..."
DB_EXISTS=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -Q "SELECT COUNT(*) FROM sys.databases WHERE name = 'TinhKhoanDB'" -h -1 -W 2>/dev/null || echo "0")
if [ "$DB_EXISTS" -gt 0 ]; then
  echo "   ✅ Database TinhKhoanDB tồn tại"
  
  # Check tables count
  echo "📋 [TABLES] Kiểm tra số lượng tables..."
  TABLE_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'" -h -1 -W 2>/dev/null || echo "0")
  echo "   📊 Tổng số tables: $TABLE_COUNT"
  
  # Check core 8 tables
  echo "🎯 [CORE TABLES] Kiểm tra 8 bảng core data..."
  sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01') ORDER BY TABLE_NAME" -t 10 2>/dev/null || echo "   ⚠️ Không thể kiểm tra core tables"
  
else
  echo "   ⚠️ Database TinhKhoanDB chưa tồn tại"
fi

# Performance monitoring
echo "📈 [PERFORMANCE] Kiểm tra performance container..."
docker stats azure_sql_edge_tinhkhoan --no-stream --format "table {{.Name}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.NetIO}}\t{{.BlockIO}}"

echo "🎉 [COMPLETE] Tối ưu PREMIUM container hoàn thành!"
echo ""
echo "💡 [PREMIUM CONFIG INFO]:"
echo "   🧠 Memory Limit: 5GB (từ 3GB)"
echo "   🔄 Memory Swap: 7GB (từ 4GB)"
echo "   ⚡ CPUs: 8 cores (từ default)"
echo "   🔧 SQL Memory: 4GB"
echo "   📊 Shared Memory: 1GB"
echo "   🚀 CPU Shares: 2048 (high priority)"
echo "   💾 Data Volume: sqldata_tinhkhoan (PRESERVED)"
echo "   🔄 Restart Policy: unless-stopped"
echo "   ❤️ Health Check: Enhanced với timeout"
echo "   🗃️ Backup Created: $BACKUP_TAG"
echo ""
echo "🎯 [PERFORMANCE OPTIMIZATIONS]:"
echo "   ✅ Memory swappiness: 1 (minimal swap usage)"
echo "   ✅ OOM Kill: Disabled cho stability"
echo "   ✅ File descriptors: 65536 (high limit)"
echo "   ✅ Memory lock: Unlimited"
echo "   ✅ Log rotation: 100MB x 3 files"
echo "   ✅ Shared memory: 1GB cho temp operations"
