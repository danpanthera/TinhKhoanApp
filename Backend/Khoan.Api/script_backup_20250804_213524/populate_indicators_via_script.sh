#!/bin/bash

# 🏢 Script shell để populate 9 bảng KPI chi nhánh qua HTTP API
# Sử dụng SQL trực tiếp thông qua SQL Server REST API hoặc exec query

echo "🔧 Tạo chỉ tiêu cho 9 bảng KPI chi nhánh..."

# Script SQL để chạy trong database
SQL_SCRIPT="
-- Xóa chỉ tiêu cũ của 9 bảng chi nhánh (ID 24-32)
DELETE FROM KpiIndicators WHERE KpiAssignmentTableId BETWEEN 24 AND 32;

-- Tạo chỉ tiêu cho từng bảng chi nhánh (giống ID=17)
-- Bảng 24: KPI_CnBinhLu
INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(24, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
(24, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
(24, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
(24, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
(24, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
(24, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
(24, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
(24, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);

-- Bảng 25: KPI_CnPhongTho
INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(25, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
(25, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
(25, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
(25, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
(25, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
(25, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
(25, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
(25, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);

-- Bảng 26: KPI_CnSinHo
INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(26, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
(26, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
(26, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
(26, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
(26, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
(26, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
(26, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
(26, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);

-- Bảng 27: KPI_CnBumTo
INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(27, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
(27, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
(27, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
(27, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
(27, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
(27, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
(27, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
(27, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);

-- Bảng 28: KPI_CnThanUyen
INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(28, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
(28, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
(28, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
(28, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
(28, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
(28, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
(28, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
(28, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);

-- Bảng 29: KPI_CnDoanKet
INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(29, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
(29, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
(29, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
(29, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
(29, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
(29, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
(29, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
(29, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);

-- Bảng 30: KPI_CnTanUyen
INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(30, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
(30, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
(30, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
(30, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
(30, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
(30, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
(30, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
(30, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);

-- Bảng 31: KPI_CnNamHang
INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(31, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
(31, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
(31, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
(31, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
(31, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
(31, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
(31, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
(31, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);

-- Bảng 32: KPI_HoiSo
INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(32, N'Tổng dư nợ BQ', 30.00, N'Triệu VND', 1, 'NUMBER', 1),
(32, N'Tỷ lệ nợ xấu', 15.00, N'%', 2, 'NUMBER', 1),
(32, N'Phát triển Khách hàng', 10.00, N'Khách hàng', 3, 'NUMBER', 1),
(32, N'Thu nợ đã XLRR', 10.00, N'Triệu VND', 4, 'NUMBER', 1),
(32, N'Thực hiện nhiệm vụ theo chương trình công tác', 10.00, N'%', 5, 'NUMBER', 1),
(32, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10.00, N'%', 6, 'NUMBER', 1),
(32, N'Tổng nguồn vốn huy động BQ', 10.00, N'Triệu VND', 7, 'NUMBER', 1),
(32, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5.00, N'%', 8, 'NUMBER', 1);
"

# Lưu SQL vào file tạm
echo "$SQL_SCRIPT" > /tmp/populate_indicators.sql

# Thử nhiều cách để execute SQL
echo "🔄 Thử chạy SQL qua container..."

# Thử 1: sqlcmd qua docker exec (nếu có)
if docker exec azure_sql_edge_tinhkhoan which sqlcmd >/dev/null 2>&1; then
    echo "✅ Tìm thấy sqlcmd, chạy script..."
    docker exec -i azure_sql_edge_tinhkhoan sqlcmd -S localhost -U sa -P "YourStrongPassword123" -d TinhKhoanDB < /tmp/populate_indicators.sql
elif docker exec azure_sql_edge_tinhkhoan which /opt/mssql-tools/bin/sqlcmd >/dev/null 2>&1; then
    echo "✅ Tìm thấy sqlcmd tại /opt/mssql-tools/bin/, chạy script..."
    docker exec -i azure_sql_edge_tinhkhoan /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrongPassword123" -d TinhKhoanDB < /tmp/populate_indicators.sql
else
    echo "❌ Không tìm thấy sqlcmd trong container"
    echo "🔄 Thử sử dụng API .NET để thực thi SQL..."

    # Tạo endpoint tạm thời để execute raw SQL (nếu cần thiết)
    echo "⚠️  Cần thực hiện populate thủ công hoặc qua SQL Server Management Studio"
    echo "📄 File SQL đã tạo tại: /tmp/populate_indicators.sql"
fi

# Kiểm tra kết quả
echo "🔍 Kiểm tra kết quả qua API..."
curl -s "http://localhost:5055/api/KpiAssignment/tables" | jq '.[] | select(.id >= 24 and .id <= 32) | {id, tableName, indicatorCount}'

echo "✅ Hoàn thành populate script!"
