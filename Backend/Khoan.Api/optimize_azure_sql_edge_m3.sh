#!/bin/bash

echo "⚡ TỐI ƯU AZURE SQL EDGE CHO M3"
echo "================================"

# Restart với cấu hình tối ưu cho M3
docker stop azure_sql_edge_tinhkhoan
docker rm azure_sql_edge_tinhkhoan

docker run -d \
  --name azure_sql_edge_tinhkhoan \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=Dientoan@303" \
  -e "MSSQL_MEMORY_LIMIT_MB=3072" \
  -e "MSSQL_AGENT_ENABLED=true" \
  -p 1433:1433 \
  -v azure_sql_edge_data:/var/opt/mssql \
  --platform linux/arm64 \
  --restart unless-stopped \
  --memory=4g \
  --memory-swap=6g \
  --cpus="6" \
  --ulimit memlock=-1:-1 \
  --ulimit core=0 \
  mcr.microsoft.com/azure-sql-edge:1.0.7

echo "✅ Đã tối ưu cho M3 với 6 CPU cores và 4GB RAM!"

# Đợi container khởi động
echo "⏳ Đợi container khởi động..."
sleep 15

# Test kết nối
echo "🔍 Kiểm tra kết nối..."
for i in {1..6}; do
    if sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -C \
        -Q "SELECT 1" >/dev/null 2>&1; then
        echo "✅ Container đã sẵn sàng!"
        break
    else
        echo "⏳ Đợi database khởi động... ($i/6)"
        sleep 5
    fi

    if [ $i -eq 6 ]; then
        echo "❌ Timeout! Hãy kiểm tra logs: docker logs azure_sql_edge_tinhkhoan"
        exit 1
    fi
done

# Tạo database nếu chưa có
echo "🏗️ Đảm bảo database TinhKhoanDB tồn tại..."
sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -C -Q "
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'TinhKhoanDB')
BEGIN
    CREATE DATABASE TinhKhoanDB COLLATE SQL_Latin1_General_CP1_CI_AS;
    PRINT 'Database TinhKhoanDB created!'
END
ELSE
    PRINT 'Database TinhKhoanDB already exists!'
"

echo ""
echo "🎯 OPTIMIZED AZURE SQL EDGE M3 STATUS"
echo "======================================"
sqlcmd -S localhost,1433 -U sa -P "Dientoan@303" -C \
  -Q "SELECT @@VERSION"

echo ""
echo "📊 Performance Stats:"
docker stats azure_sql_edge_tinhkhoan --no-stream --format "table {{.Name}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.MemPerc}}"

echo ""
echo "🔥 M3 OPTIMIZED & READY!"
echo "========================"
echo "🚀 Database: TinhKhoanDB ready"
echo "⚡ M3 Performance: 6 cores, 4GB RAM"
echo "🎯 Connection: Server=localhost,1433;Database=TinhKhoanDB;User Id=sa;Password=Dientoan@303;TrustServerCertificate=true"
