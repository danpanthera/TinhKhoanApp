#!/bin/bash
# Script để thực hiện việc sắp xếp lại cột vật lý của bảng DP01
# Tác giả: GitHub Copilot
# Ngày tạo: 08/08/2025

echo "🛠️ Bắt đầu tiến trình sắp xếp lại cột DP01..."
echo "=============================================="

# Thông số kết nối
SERVER="localhost,1433"
DATABASE="KhoanDB"
USER="sa"
PASSWORD="Dientoan@303"# Sao chép script SQL vào container
docker cp /opt/Projects/Khoan/Backend/KhoanApp.Api/dp01_reorder_columns.sql azure_sql_edge_tinhkhoan:/var/opt/mssql/data/dp01_reorder_columns.sql

# Xác nhận dung lượng bảng trước khi thực hiện
echo "📊 Dung lượng bảng DP01 trước khi sắp xếp lại:"
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -Q "SELECT t.name AS TableName, p.rows AS RowCounts, SUM(a.total_pages) * 8 AS TotalSpaceKB FROM sys.tables t INNER JOIN sys.indexes i ON t.object_id = i.object_id INNER JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id WHERE t.name = 'DP01' GROUP BY t.name, p.rows;"

# Tạo bảng mới với cấu trúc cột đã được sắp xếp lại
echo "🔄 Tạo bảng DP01_New với thứ tự cột được tối ưu..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -i /var/opt/mssql/data/dp01_reorder_columns.sql

# Xem số lượng bản ghi đã được sao chép
echo "🔍 Kiểm tra số lượng bản ghi đã sao chép:"
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -Q "SELECT 'DP01' AS TableName, COUNT(*) AS RecordCount FROM dbo.DP01 UNION SELECT 'DP01_New' AS TableName, COUNT(*) AS RecordCount FROM dbo.DP01_New;"

# Xác nhận dung lượng bảng mới
echo "📊 So sánh dung lượng giữa bảng cũ và bảng mới:"
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d KhoanDB -Q "SELECT t.name AS TableName, p.rows AS RowCounts, SUM(a.total_pages) * 8 AS TotalSpaceKB FROM sys.tables t INNER JOIN sys.indexes i ON t.object_id = i.object_id INNER JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id WHERE t.name IN ('DP01', 'DP01_New') GROUP BY t.name, p.rows;"

echo "✅ Quá trình sắp xếp lại cột hoàn tất!"
echo ""
echo "⚠️ LƯU Ý: Script này chỉ tạo bảng mới và sao chép dữ liệu."
echo "   Để hoàn tất việc đổi tên bảng, hãy chạy các lệnh sau trong SQL:"
echo "   EXEC sp_rename 'dbo.DP01', 'DP01_Old';"
echo "   EXEC sp_rename 'dbo.DP01_New', 'DP01';"
echo "   DROP TABLE dbo.DP01_Old;"
