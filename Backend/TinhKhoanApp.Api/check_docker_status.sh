#!/bin/bash

# =============================================================================
# 📊 DOCKER CONTAINER STATUS CHECKER
# Kiểm tra trạng thái và cấu hình chi tiết của container
# =============================================================================

echo "📊 DOCKER CONTAINER STATUS"
echo "=========================="

# Kiểm tra container đang chạy
echo "🔍 Container Status:"
docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}\t{{.RunningFor}}"
echo ""

if docker ps | grep -q "azure_sql_edge_tinhkhoan"; then
    echo "✅ Container azure_sql_edge_tinhkhoan is running"

    # Resource usage
    echo ""
    echo "📈 Current Resource Usage:"
    docker stats azure_sql_edge_tinhkhoan --no-stream --format "table {{.Container}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.MemPerc}}\t{{.NetIO}}\t{{.BlockIO}}\t{{.PIDs}}"

    # Container configuration
    echo ""
    echo "⚙️  Container Configuration:"
    docker inspect azure_sql_edge_tinhkhoan | jq '{
        Name: .Name,
        Memory: (.HostConfig.Memory / 1024 / 1024 / 1024 | tostring + "GB"),
        MemorySwap: (.HostConfig.MemorySwap / 1024 / 1024 / 1024 | tostring + "GB"),
        ShmSize: (.HostConfig.ShmSize / 1024 / 1024 | tostring + "MB"),
        CpuShares: .HostConfig.CpuShares,
        RestartPolicy: .HostConfig.RestartPolicy.Name,
        Status: .State.Status,
        StartedAt: .State.StartedAt
    }'

    # SQL Server status
    echo ""
    echo "🗄️  SQL Server Status:"
    if command -v sqlcmd &> /dev/null; then
        if sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -Q "SELECT @@VERSION" -h-1 2>/dev/null | head -1; then
            echo "✅ SQL Server is responding"

            echo ""
            echo "📋 Databases:"
            sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -Q "SELECT name FROM sys.databases" -h-1 -s" | " | grep -v "^$"

        else
            echo "⚠️  SQL Server not ready yet"
        fi
    else
        echo "ℹ️  sqlcmd not available for testing"
    fi

else
    echo "❌ Container azure_sql_edge_tinhkhoan is not running"

    # Check if container exists but stopped
    if docker ps -a | grep -q "azure_sql_edge_tinhkhoan"; then
        echo "🔧 Container exists but stopped. Status:"
        docker ps -a --format "table {{.Names}}\t{{.Status}}" | grep azure_sql_edge_tinhkhoan

        echo ""
        echo "📋 Recent logs:"
        docker logs azure_sql_edge_tinhkhoan --tail 5
    else
        echo "🚫 Container does not exist"
    fi
fi

echo ""
echo "🐳 Docker System Info:"
docker system df --format "table {{.Type}}\t{{.Total}}\t{{.Active}}\t{{.Size}}\t{{.Reclaimable}}"
