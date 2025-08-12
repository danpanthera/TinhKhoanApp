#!/bin/bash

# =============================================================================
# 🚀 QUICK DOCKER OPTIMIZATION - NO CONFIRMATION
# Tối ưu nhanh container mà không cần xác nhận
# =============================================================================

echo "🔧 QUICK DOCKER OPTIMIZATION"
echo "============================"

# Stop current container
echo "🛑 Stopping current container..."
docker stop azure_sql_edge_tinhkhoan 2>/dev/null || true

# Rename current container as backup
echo "📦 Backup current container..."
docker rename azure_sql_edge_tinhkhoan azure_sql_edge_tinhkhoan_backup_$(date +%Y%m%d_%H%M%S) 2>/dev/null || true

# Remove if exists
docker rm azure_sql_edge_tinhkhoan 2>/dev/null || true

echo "🚀 Creating optimized container..."

# Create optimized container
docker run -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=Dientoan@303" \
  -p 1433:1433 \
  --name azure_sql_edge_tinhkhoan \
  -v sqldata_tinhkhoan_new:/var/opt/mssql \
  --memory=2g \
  --memory-swap=3g \
  --cpus="2" \
  --restart=unless-stopped \
  --shm-size=256m \
  --ulimit memlock=-1:-1 \
  --ulimit core=0 \
  -e "MSSQL_MEMORY_LIMIT_MB=1536" \
  -e "MSSQL_PID=Developer" \
  -e "MSSQL_COLLATION=SQL_Latin1_General_CP1_CI_AS" \
  --oom-kill-disable=false \
  --log-driver=json-file \
  --log-opt max-size=50m \
  --log-opt max-file=3 \
  -d mcr.microsoft.com/azure-sql-edge:latest

echo "⏳ Waiting for container startup..."
sleep 20

if docker ps | grep -q "azure_sql_edge_tinhkhoan"; then
    echo "✅ Container optimized successfully!"
    echo ""
    echo "📊 New resource usage:"
    docker stats azure_sql_edge_tinhkhoan --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.MemPerc}}"
    echo ""
    echo "🎯 Optimizations applied:"
    echo "   • RAM: 2GB (từ không giới hạn)"
    echo "   • CPU: 2 cores (từ 6 cores)"
    echo "   • Memory Limit: 1.5GB SQL Server"
    echo "   • Shared Memory: 256MB"
    echo "   • Log size limit: 50MB x 3 files"
    echo ""
    echo "✅ Ready for backend startup!"
else
    echo "❌ Container failed to start"
    docker logs azure_sql_edge_tinhkhoan --tail 10
fi
