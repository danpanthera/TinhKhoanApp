#!/bin/bash

# =============================================================================
# 🔄 RESTORE ORIGINAL DOCKER CONTAINER
# Phục hồi lại container với cấu hình ban đầu theo README_DAT.md
# =============================================================================

echo "🔄 RESTORE ORIGINAL DOCKER CONTAINER"
echo "===================================="

# Stop và xóa container hiện tại nếu có
echo "🛑 Stopping and removing current containers..."
docker stop azure_sql_edge_tinhkhoan 2>/dev/null || true
docker rm azure_sql_edge_tinhkhoan 2>/dev/null || true

# Xóa các container backup cũ
echo "🗑️  Cleaning up backup containers..."
docker ps -a --format "{{.Names}}" | grep "azure_sql_edge_tinhkhoan_backup" | xargs -r docker rm -f

echo "🚀 Creating original container with full resources..."

# Tạo lại container với cấu hình ban đầu - Full resources
docker run -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=Dientoan@303" \
  -p 1433:1433 \
  --name azure_sql_edge_tinhkhoan \
  -v sqldata_tinhkhoan_new:/var/opt/mssql \
  --memory=4g \
  --memory-swap=8g \
  --restart=unless-stopped \
  --shm-size=1g \
  --ulimit memlock=-1:-1 \
  --ulimit core=0 \
  -e "MSSQL_MEMORY_LIMIT_MB=3072" \
  -e "MSSQL_PID=Developer" \
  -d mcr.microsoft.com/azure-sql-edge:latest

echo "⏳ Waiting for container startup..."
sleep 20

# Kiểm tra container
if docker ps | grep -q "azure_sql_edge_tinhkhoan"; then
    echo "✅ Original container restored successfully!"
    echo ""
    echo "📊 Container resource usage:"
    docker stats azure_sql_edge_tinhkhoan --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.MemPerc}}"
    echo ""
    echo "🎯 Original configuration restored:"
    echo "   • RAM: 4GB (original)"
    echo "   • Memory Swap: 8GB"
    echo "   • Shared Memory: 1GB"
    echo "   • SQL Memory: 3GB"
    echo "   • CPU: Unlimited (6 cores available)"
    echo "   • Auto restart: enabled"
    echo ""

    # Wait for SQL Server to be ready and test connection
    echo "⏳ Waiting for SQL Server to be ready..."
    sleep 15

    if command -v sqlcmd &> /dev/null; then
        echo "🧪 Testing database connection..."
        if sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -Q "SELECT 1" -o /dev/null 2>&1; then
            echo "✅ Database connection successful!"
        else
            echo "⚠️  Database still starting up - this is normal"
            echo "   Try connecting in 1-2 minutes"
        fi
    fi

    echo ""
    echo "✅ Original Docker container fully restored!"
    echo "📝 Database: TinhKhoanDB"
    echo "🔐 Password: Dientoan@303"
    echo "🌐 Port: 1433"

else
    echo "❌ Container failed to start"
    echo "Checking logs:"
    docker logs azure_sql_edge_tinhkhoan --tail 10
    exit 1
fi
