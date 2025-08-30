#!/bin/bash

echo "⚡ SCRIPT KHỞI ĐỘNG AZURE SQL EDGE ARM64 CHO M3"
echo "==============================================="

# Kiểm tra và khởi động container
if docker ps -q -f name=azure_sql_edge_tinhkhoan | grep -q .; then
    echo "✅ Azure SQL Edge đang chạy"
else
    if docker ps -aq -f name=azure_sql_edge_tinhkhoan | grep -q .; then
        echo "🔄 Khởi động Azure SQL Edge..."
        docker start azure_sql_edge_tinhkhoan
        sleep 10
    else
        echo "❌ Container azure_sql_edge_tinhkhoan không tồn tại"
        echo "📝 Vui lòng chạy azure_sql_edge_m3_migration.sh trước"
        exit 1
    fi
fi

# Test connection từ host macOS
echo "🔍 Kiểm tra kết nối database..."
for i in {1..6}; do
    if sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -C \
        -Q "SELECT 1" >/dev/null 2>&1; then
        echo "✅ Database đã sẵn sàng!"
        break
    else
        echo "⏳ Đợi database khởi động... ($i/6)"
        sleep 5
    fi

    if [ $i -eq 6 ]; then
        echo "❌ Không thể kết nối database"
        exit 1
    fi
done

echo ""
echo "🎯 AZURE SQL EDGE ARM64 STATUS"
echo "==============================="
sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -C \
  -Q "SELECT @@VERSION"

echo ""
echo "📊 Performance Stats:"
docker stats azure_sql_edge_tinhkhoan --no-stream --format "table {{.Name}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.MemPerc}}"

echo ""
echo "🔥 READY FOR DEVELOPMENT!"
echo "========================="
echo "🚀 Database: KhoanDB ready trên port 1433"
echo "⚡ Native ARM64 performance trên M3"
echo "🎯 Connection: Server=localhost,1433;Database=KhoanDB;User Id=sa;Password=Dientoan@303;TrustServerCertificate=true"
