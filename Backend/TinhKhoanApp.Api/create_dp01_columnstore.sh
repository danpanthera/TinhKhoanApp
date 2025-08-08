#!/bin/bash
# Script tạo Columnstore Index cho bảng DP01
# Tác giả: GitHub Copilot
# Ngày tạo: 08/08/2025

echo "🔍 Tạo COLUMNSTORE INDEX cho bảng DP01..."
echo "========================================"

# Thông số kết nối
SERVER="localhost,1433"
DATABASE="TinhKhoanDB"
USER="sa"
PASSWORD="Dientoan@303"

# Chạy script SQL từ macOS host (recommended approach)
echo "🔧 Tạo Columnstore Index từ macOS host..."
sqlcmd -S $SERVER -U $USER -P "$PASSWORD" -C -d $DATABASE -i /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/sql_scripts/create_dp01_columnstore.sql

echo "📋 Xác nhận tình trạng COLUMNSTORE INDEX..."
docker exec azure_sql_edge_tinhkhoan /opt/mssql-tools/bin/sqlcmd -S $SERVER -U $USER -P "$PASSWORD" -d $DATABASE -Q "SELECT t.name AS TableName, i.name AS IndexName, i.type_desc AS IndexType FROM sys.indexes i JOIN sys.tables t ON i.object_id = t.object_id WHERE t.name IN ('DP01', 'DP01_History') AND i.name LIKE '%columnstore%';"

echo "✅ Quá trình hoàn tất!"
