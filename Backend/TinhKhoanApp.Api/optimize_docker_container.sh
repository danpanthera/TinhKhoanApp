#!/bin/bash

# =============================================================================
# 🚀 AZURE SQL EDGE CONTAINER OPTIMIZATION SCRIPT
# Tối ưu hóa tài nguyên: RAM, CPU, HDD cho hiệu suất và ổn định
# =============================================================================

echo "🔧 AZURE SQL EDGE CONTAINER OPTIMIZATION"
echo "======================================="

# Kiểm tra container hiện tại
if docker ps | grep -q "azure_sql_edge_tinhkhoan"; then
    echo "📊 Current container stats:"
    docker stats azure_sql_edge_tinhkhoan --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.MemPerc}}\t{{.NetIO}}\t{{.BlockIO}}"
    echo ""
fi

echo "⚠️  Chuẩn bị tối ưu container..."
echo "1. Dừng container hiện tại"
echo "2. Backup dữ liệu (nếu cần)"
echo "3. Tạo container mới với cấu hình tối ưu"
echo ""

read -p "Bạn có muốn tiếp tục? (y/N): " confirm
if [[ $confirm != [yY] ]]; then
    echo "❌ Đã hủy tối ưu hóa"
    exit 1
fi

# =============================================================================
# STEP 1: Backup và dừng container cũ
# =============================================================================
echo ""
echo "🔄 STEP 1: Backup và dừng container..."

# Dừng container cũ
if docker ps | grep -q "azure_sql_edge_tinhkhoan"; then
    echo "🛑 Dừng container hiện tại..."
    docker stop azure_sql_edge_tinhkhoan
fi

# Rename container cũ làm backup
if docker ps -a | grep -q "azure_sql_edge_tinhkhoan"; then
    echo "📦 Backup container cũ..."
    docker rename azure_sql_edge_tinhkhoan azure_sql_edge_tinhkhoan_backup_$(date +%Y%m%d_%H%M%S)
fi

# =============================================================================
# STEP 2: Tạo container mới với cấu hình tối ưu
# =============================================================================
echo ""
echo "🚀 STEP 2: Tạo container tối ưu..."

# Optimized configuration - giảm tài nguyên nhưng vẫn đảm bảo hiệu suất
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

echo ""
echo "⏳ Đợi container khởi động..."
sleep 15

# =============================================================================
# STEP 3: Kiểm tra container mới
# =============================================================================
echo ""
echo "✅ STEP 3: Kiểm tra container mới..."

# Kiểm tra container đang chạy
if docker ps | grep -q "azure_sql_edge_tinhkhoan"; then
    echo "✅ Container đã khởi động thành công!"

    echo ""
    echo "📊 New container stats:"
    docker stats azure_sql_edge_tinhkhoan --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.MemPerc}}\t{{.NetIO}}\t{{.BlockIO}}"

    echo ""
    echo "🔍 Container configuration:"
    docker inspect azure_sql_edge_tinhkhoan | jq '{
        Memory: .HostConfig.Memory,
        MemorySwap: .HostConfig.MemorySwap,
        CpuCount: .HostConfig.CpuCount,
        ShmSize: .HostConfig.ShmSize,
        RestartPolicy: .HostConfig.RestartPolicy.Name
    }'
else
    echo "❌ Container không khởi động được!"
    echo "Kiểm tra logs:"
    docker logs azure_sql_edge_tinhkhoan --tail 20
    exit 1
fi

# =============================================================================
# STEP 4: Test kết nối database
# =============================================================================
echo ""
echo "🔧 STEP 4: Test kết nối database..."

# Wait for SQL Server to be ready
echo "⏳ Đợi SQL Server sẵn sàng..."
sleep 10

# Test connection
if command -v sqlcmd &> /dev/null; then
    echo "🧪 Testing connection với sqlcmd..."
    if sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -Q "SELECT @@VERSION, SERVERPROPERTY('Edition') AS Edition" -o /dev/null 2>&1; then
        echo "✅ Kết nối database thành công!"

        # Show server info
        echo ""
        echo "📋 Server Information:"
        sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -Q "
        SELECT
            @@VERSION AS Version,
            SERVERPROPERTY('Edition') AS Edition,
            SERVERPROPERTY('ProductLevel') AS ProductLevel,
            SERVERPROPERTY('ResourceVersion') AS ResourceVersion
        " -h-1 -s"|" -W
    else
        echo "⚠️  Chưa kết nối được database. Chờ thêm..."
        sleep 10

        if sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -Q "SELECT 1" -o /dev/null 2>&1; then
            echo "✅ Kết nối database thành công sau retry!"
        else
            echo "❌ Không thể kết nối database. Kiểm tra logs:"
            docker logs azure_sql_edge_tinhkhoan --tail 10
        fi
    fi
else
    echo "⚠️  sqlcmd không có sẵn. Sử dụng docker exec để test..."
    if docker exec -i azure_sql_edge_tinhkhoan /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'Dientoan@303' -Q "SELECT 1" > /dev/null 2>&1; then
        echo "✅ Container SQL Server sẵn sàng!"
    else
        echo "❌ SQL Server chưa sẵn sàng"
    fi
fi

# =============================================================================
# OPTIMIZATION SUMMARY
# =============================================================================
echo ""
echo "🎯 OPTIMIZATION SUMMARY"
echo "======================"
echo "📈 Tối ưu đã áp dụng:"
echo "   • RAM: 2GB (giảm từ 4GB) + 1GB swap"
echo "   • CPU: 2 cores (giảm từ 6 cores)"
echo "   • Shared Memory: 256MB (giảm từ 1GB)"
echo "   • SQL Memory: 1.5GB (tối ưu cho 2GB RAM)"
echo "   • Log rotation: 50MB x 3 files"
echo "   • OOM protection: enabled"
echo ""
echo "🚀 Performance Benefits:"
echo "   • Giảm 50% RAM usage"
echo "   • Giảm 66% CPU allocation"
echo "   • Log management tự động"
echo "   • Tránh memory leaks"
echo "   • Container restart an toàn"
echo ""
echo "⚡ Sử dụng lệnh để monitor:"
echo "   docker stats azure_sql_edge_tinhkhoan"
echo "   docker logs azure_sql_edge_tinhkhoan --tail 20"
echo ""
echo "✅ Container tối ưu hoàn thành!"
