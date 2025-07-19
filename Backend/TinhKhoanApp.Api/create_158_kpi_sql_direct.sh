#!/bin/bash

echo "🎯 TẠO 158 CHỈ TIÊU KPI TRỰC TIẾP VÀO DATABASE"
echo "==============================================="

# Kết nối SQL Server (bỏ qua SSL)
SQLCMD="sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -d TinhKhoanDB -C -Q"

echo "🔧 Kiểm tra kết nối database..."
result=$($SQLCMD "SELECT COUNT(*) as TableCount FROM KpiAssignmentTables" 2>/dev/null | grep -o '[0-9]*' | head -1)
if [ ! -z "$result" ] && [ "$result" -gt "0" ]; then
    echo "✅ Database đã sẵn sàng ($result bảng KPI)"
else
    echo "❌ Không thể kết nối database. Thử tạo trực tiếp..."
fi

echo ""
echo "🗑️  Xóa tất cả KPI indicators cũ..."
$SQLCMD "DELETE FROM KpiIndicators"

echo ""
echo "🔧 Tạo 158 chỉ tiêu KPI theo danh sách CHÍNH XÁC..."

# BẢNG 1: TruongphongKhdn (ID: 33) - 8 chỉ tiêu
echo ""
echo "📋 1. TruongphongKhdn (8 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(33, N'Tổng Dư nợ KHDN', 20, N'Triệu VND', 1, 4, 1),
(33, N'Tỷ lệ nợ xấu KHDN', 10, N'%', 2, 2, 1),
(33, N'Thu nợ đã XLRR KHDN', 10, N'Triệu VND', 3, 4, 1),
(33, N'Lợi nhuận khoán tài chính', 10, N'Triệu VND', 4, 4, 1),
(33, N'Phát triển Khách hàng Doanh nghiệp', 10, N'Khách hàng', 5, 1, 1),
(33, N'Điều hành theo chương trình công tác', 20, N'%', 6, 2, 1),
(33, N'Chấp hành quy chế, quy trình nghiệp vụ', 10, N'%', 7, 2, 1),
(33, N'BQ kết quả thực hiện CB trong phòng mình phụ trách', 10, N'%', 8, 2, 1);
"

# BẢNG 2: TruongphongKhcn (ID: 34) - 8 chỉ tiêu
echo "📋 2. TruongphongKhcn (8 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(34, N'Tổng Dư nợ KHCN', 20, N'Triệu VND', 1, 4, 1),
(34, N'Tỷ lệ nợ xấu KHCN', 10, N'%', 2, 2, 1),
(34, N'Thu nợ đã XLRR KHCN', 10, N'Triệu VND', 3, 4, 1),
(34, N'Lợi nhuận khoán tài chính', 10, N'Triệu VND', 4, 4, 1),
(34, N'Phát triển Khách hàng Cá nhân', 10, N'Khách hàng', 5, 1, 1),
(34, N'Điều hành theo chương trình công tác', 20, N'%', 6, 2, 1),
(34, N'Chấp hành quy chế, quy trình nghiệp vụ', 10, N'%', 7, 2, 1),
(34, N'BQ kết quả thực hiện CB trong phòng mình phụ trách', 10, N'%', 8, 2, 1);
"

# BẢNG 3: PhophongKhdn (ID: 35) - 8 chỉ tiêu
echo "📋 3. PhophongKhdn (8 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(35, N'Tổng Dư nợ KHDN', 20, N'Triệu VND', 1, 4, 1),
(35, N'Tỷ lệ nợ xấu KHDN', 10, N'%', 2, 2, 1),
(35, N'Thu nợ đã XLRR KHDN', 10, N'Triệu VND', 3, 4, 1),
(35, N'Lợi nhuận khoán tài chính', 10, N'Triệu VND', 4, 4, 1),
(35, N'Phát triển Khách hàng Doanh nghiệp', 10, N'Khách hàng', 5, 1, 1),
(35, N'Điều hành theo chương trình công tác', 20, N'%', 6, 2, 1),
(35, N'Chấp hành quy chế, quy trình nghiệp vụ', 10, N'%', 7, 2, 1),
(35, N'BQ kết quả thực hiện CB trong phòng mình phụ trách', 10, N'%', 8, 2, 1);
"

# BẢNG 4: PhophongKhcn (ID: 36) - 8 chỉ tiêu
echo "📋 4. PhophongKhcn (8 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(36, N'Tổng Dư nợ KHCN', 20, N'Triệu VND', 1, 4, 1),
(36, N'Tỷ lệ nợ xấu KHCN', 10, N'%', 2, 2, 1),
(36, N'Thu nợ đã XLRR KHCN', 10, N'Triệu VND', 3, 4, 1),
(36, N'Lợi nhuận khoán tài chính', 10, N'Triệu VND', 4, 4, 1),
(36, N'Phát triển Khách hàng Cá nhân', 10, N'Khách hàng', 5, 1, 1),
(36, N'Điều hành theo chương trình công tác', 20, N'%', 6, 2, 1),
(36, N'Chấp hành quy chế, quy trình nghiệp vụ', 10, N'%', 7, 2, 1),
(36, N'BQ kết quả thực hiện CB trong phòng mình phụ trách', 10, N'%', 8, 2, 1);
"

# BẢNG 5: TruongphongKhqlrr (ID: 37) - 6 chỉ tiêu
echo "📋 5. TruongphongKhqlrr (6 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(37, N'Tổng nguồn vốn', 10, N'Triệu VND', 1, 4, 1),
(37, N'Tổng dư nợ', 10, N'Triệu VND', 2, 4, 1),
(37, N'Lợi nhuận khoán tài chính', 10, N'Triệu VND', 3, 4, 1),
(37, N'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 50, N'%', 4, 2, 1),
(37, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, N'%', 5, 2, 1),
(37, N'Kết quả thực hiện BQ của CB trong phòng', 10, N'%', 6, 2, 1);
"

# BẢNG 6: PhophongKhqlrr (ID: 38) - 6 chỉ tiêu
echo "📋 6. PhophongKhqlrr (6 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(38, N'Tổng nguồn vốn', 10, N'Triệu VND', 1, 4, 1),
(38, N'Tổng dư nợ', 10, N'Triệu VND', 2, 4, 1),
(38, N'Lợi nhuận khoán tài chính', 10, N'Triệu VND', 3, 4, 1),
(38, N'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 50, N'%', 4, 2, 1),
(38, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, N'%', 5, 2, 1),
(38, N'Kết quả thực hiện BQ của CB trong phòng mình phụ trách', 10, N'%', 6, 2, 1);
"

# BẢNG 7: Cbtd (ID: 39) - 8 chỉ tiêu
echo "📋 7. Cbtd (8 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(39, N'Tổng dư nợ BQ', 30, N'Triệu VND', 1, 4, 1),
(39, N'Tỷ lệ nợ xấu', 15, N'%', 2, 2, 1),
(39, N'Phát triển Khách hàng', 10, N'Khách hàng', 3, 1, 1),
(39, N'Thu nợ đã XLRR (nếu không có nợ XLRR thì cộng vào chỉ tiêu Dư nợ)', 10, N'Triệu VND', 4, 4, 1),
(39, N'Thực hiện nhiệm vụ theo chương trình công tác', 10, N'%', 5, 2, 1),
(39, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, N'%', 6, 2, 1),
(39, N'Tổng nguồn vốn huy động BQ', 10, N'Triệu VND', 7, 4, 1),
(39, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5, N'%', 8, 2, 1);
"

# BẢNG 8: TruongphongKtnqCnl1 (ID: 40) - 6 chỉ tiêu
echo "📋 8. TruongphongKtnqCnl1 (6 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(40, N'Tổng nguồn vốn', 10, N'Triệu VND', 1, 4, 1),
(40, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 2, 4, 1),
(40, N'Thu dịch vụ thanh toán trong nước', 10, N'Triệu VND', 3, 4, 1),
(40, N'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 40, N'%', 4, 2, 1),
(40, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, N'%', 5, 2, 1),
(40, N'Kết quả thực hiện BQ của CB trong phòng', 10, N'%', 6, 2, 1);
"

# BẢNG 9: PhophongKtnqCnl1 (ID: 41) - 6 chỉ tiêu
echo "📋 9. PhophongKtnqCnl1 (6 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(41, N'Tổng nguồn vốn', 10, N'Triệu VND', 1, 4, 1),
(41, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 2, 4, 1),
(41, N'Thu dịch vụ thanh toán trong nước', 10, N'Triệu VND', 3, 4, 1),
(41, N'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng', 40, N'%', 4, 2, 1),
(41, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, N'%', 5, 2, 1),
(41, N'Kết quả thực hiện BQ của CB thuộc mình phụ trách', 10, N'%', 6, 2, 1);
"

# BẢNG 10: Gdv (ID: 42) - 6 chỉ tiêu
echo "📋 10. Gdv (6 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(42, N'Số bút toán giao dịch BQ', 50, N'BT', 1, 1, 1),
(42, N'Số bút toán hủy', 15, N'BT', 2, 1, 1),
(42, N'Thực hiện chức năng, nhiệm vụ được giao', 10, N'%', 3, 2, 1),
(42, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, N'%', 4, 2, 1),
(42, N'Tổng nguồn vốn huy động BQ', 10, N'Triệu VND', 5, 4, 1),
(42, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5, N'%', 6, 2, 1);
"

# BẢNG 12: TruongphoItThKtgs (ID: 44) - 5 chỉ tiêu
echo "📋 12. TruongphoItThKtgs (5 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(44, N'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng', 65, N'%', 1, 2, 1),
(44, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, N'%', 2, 2, 1),
(44, N'Tổng nguồn vốn huy động BQ', 10, N'Triệu VND', 3, 4, 1),
(44, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5, N'%', 4, 2, 1),
(44, N'Kết quả thực hiện BQ của cán bộ trong phòng', 10, N'%', 5, 2, 1);
"

# BẢNG 13: CBItThKtgsKhqlrr (ID: 45) - 4 chỉ tiêu
echo "📋 13. CBItThKtgsKhqlrr (4 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(45, N'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ được giao', 75, N'%', 1, 2, 1),
(45, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, N'%', 2, 2, 1),
(45, N'Tổng nguồn vốn huy động BQ', 10, N'Triệu VND', 3, 4, 1),
(45, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5, N'%', 4, 2, 1);
"

# BẢNG 14: GiamdocPgd (ID: 46) - 9 chỉ tiêu
echo "📋 14. GiamdocPgd (9 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(46, N'Tổng nguồn vốn BQ', 15, N'Triệu VND', 1, 4, 1),
(46, N'Tổng dư nợ BQ', 15, N'Triệu VND', 2, 4, 1),
(46, N'Tỷ lệ nợ xấu', 10, N'%', 3, 2, 1),
(46, N'Phát triển Khách hàng', 10, N'Khách hàng', 4, 1, 1),
(46, N'Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)', 5, N'Triệu VND', 5, 4, 1),
(46, N'Thu dịch vụ', 10, N'Triệu VND', 6, 4, 1),
(46, N'Lợi nhuận khoán tài chính', 15, N'Triệu VND', 7, 4, 1),
(46, N'Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, N'%', 8, 2, 1),
(46, N'BQ kết quả thực hiện của CB trong phòng', 10, N'%', 9, 2, 1);
"

# BẢNG 15: PhogiamdocPgd (ID: 47) - 9 chỉ tiêu
echo "📋 15. PhogiamdocPgd (9 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(47, N'Tổng nguồn vốn BQ', 15, N'Triệu VND', 1, 4, 1),
(47, N'Tổng dư nợ BQ', 15, N'Triệu VND', 2, 4, 1),
(47, N'Tỷ lệ nợ xấu', 10, N'%', 3, 2, 1),
(47, N'Phát triển Khách hàng', 10, N'Khách hàng', 4, 1, 1),
(47, N'Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)', 5, N'Triệu VND', 5, 4, 1),
(47, N'Thu dịch vụ', 10, N'Triệu VND', 6, 4, 1),
(47, N'Lợi nhuận khoán tài chính', 15, N'Triệu VND', 7, 4, 1),
(47, N'Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, N'%', 8, 2, 1),
(47, N'BQ kết quả thực hiện của CB trong phòng', 10, N'%', 9, 2, 1);
"

# BẢNG 16: PhogiamdocPgdCbtd (ID: 48) - 8 chỉ tiêu
echo "📋 16. PhogiamdocPgdCbtd (8 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(48, N'Tổng dư nợ BQ', 30, N'Triệu VND', 1, 4, 1),
(48, N'Tỷ lệ nợ xấu', 15, N'%', 2, 2, 1),
(48, N'Phát triển Khách hàng', 10, N'Khách hàng', 3, 1, 1),
(48, N'Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)', 10, N'Triệu VND', 4, 4, 1),
(48, N'Thực hiện nhiệm vụ theo chương trình công tác', 10, N'%', 5, 2, 1),
(48, N'Tổng nguồn vốn huy động BQ', 10, N'Triệu VND', 6, 4, 1),
(48, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5, N'%', 7, 2, 1),
(48, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, N'%', 8, 2, 1);
"

# BẢNG 17: GiamdocCnl2 (ID: 49) - 11 chỉ tiêu
echo "📋 17. GiamdocCnl2 (11 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(49, N'Tổng nguồn vốn cuối kỳ', 5, N'Triệu VND', 1, 4, 1),
(49, N'Tổng nguồn vốn huy động BQ trong kỳ', 10, N'Triệu VND', 2, 4, 1),
(49, N'Tổng dư nợ cuối kỳ', 5, N'Triệu VND', 3, 4, 1),
(49, N'Tổng dư nợ BQ trong kỳ', 10, N'Triệu VND', 4, 4, 1),
(49, N'Tổng dư nợ HSX&CN', 5, N'Triệu VND', 5, 4, 1),
(49, N'Tỷ lệ nợ xấu nội bảng', 10, N'%', 6, 2, 1),
(49, N'Thu nợ đã XLRR', 5, N'Triệu VND', 7, 4, 1),
(49, N'Phát triển Khách hàng', 10, N'Khách hàng', 8, 1, 1),
(49, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 9, 4, 1),
(49, N'Thu dịch vụ', 10, N'Triệu VND', 10, 4, 1),
(49, N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, N'%', 11, 2, 1);
"

# BẢNG 18: PhogiamdocCnl2Td (ID: 50) - 8 chỉ tiêu
echo "📋 18. PhogiamdocCnl2Td (8 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(50, N'Tổng dư nợ cho vay', 20, N'Triệu VND', 1, 4, 1),
(50, N'Tổng dư nợ cho vay HSX&CN', 10, N'Triệu VND', 2, 4, 1),
(50, N'Thu nợ đã xử lý', 10, N'Triệu VND', 3, 4, 1),
(50, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 4, 4, 1),
(50, N'Tỷ lệ nợ xấu', 10, N'%', 5, 2, 1),
(50, N'Phát triển Khách hàng', 10, N'Khách hàng', 6, 1, 1),
(50, N'Điều hành theo chương trình công tác, nhiệm vụ được giao', 10, N'%', 7, 2, 1),
(50, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, N'%', 8, 2, 1);
"

# BẢNG 19: PhogiamdocCnl2Kt (ID: 51) - 6 chỉ tiêu
echo "📋 19. PhogiamdocCnl2Kt (6 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(51, N'Tổng nguồn vốn', 20, N'Triệu VND', 1, 4, 1),
(51, N'Lợi nhuận khoán tài chính', 30, N'Triệu VND', 2, 4, 1),
(51, N'Tổng doanh thu phí dịch vụ', 20, N'Triệu VND', 3, 4, 1),
(51, N'Số thẻ phát hành', 10, N'cái', 4, 1, 1),
(51, N'Điều hành theo chương trình công tác, nhiệm vụ được giao', 10, N'%', 5, 2, 1),
(51, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, N'%', 6, 2, 1);
"

# BẢNG 20: TruongphongKhCnl2 (ID: 52) - 9 chỉ tiêu
echo "📋 20. TruongphongKhCnl2 (9 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(52, N'Tổng dư nợ', 20, N'Triệu VND', 1, 4, 1),
(52, N'Tỷ lệ nợ xấu', 15, N'%', 2, 2, 1),
(52, N'Phát triển Khách hàng', 10, N'Khách hàng', 3, 1, 1),
(52, N'Thu nợ đã XLRR', 10, N'Triệu VND', 4, 4, 1),
(52, N'Điều hành theo chương trình công tác', 10, N'%', 5, 2, 1),
(52, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, N'%', 6, 2, 1),
(52, N'Tổng nguồn vốn huy động BQ', 10, N'Triệu VND', 7, 4, 1),
(52, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5, N'%', 8, 2, 1),
(52, N'Kết quả thực hiện BQ của CB trong phòng', 10, N'%', 9, 2, 1);
"

# BẢNG 21: PhophongKhCnl2 (ID: 53) - 8 chỉ tiêu
echo "📋 21. PhophongKhCnl2 (8 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(53, N'Tổng dư nợ BQ', 30, N'Triệu VND', 1, 4, 1),
(53, N'Tỷ lệ nợ xấu', 15, N'%', 2, 2, 1),
(53, N'Phát triển Khách hàng', 10, N'Khách hàng', 3, 1, 1),
(53, N'Thu nợ đã XLRR', 10, N'Triệu VND', 4, 4, 1),
(53, N'Thực hiện nhiệm vụ theo chương trình công tác', 10, N'%', 5, 2, 1),
(53, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, N'%', 6, 2, 1),
(53, N'Tổng nguồn vốn huy động BQ', 10, N'Triệu VND', 7, 4, 1),
(53, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5, N'%', 8, 2, 1);
"

# BẢNG 22: TruongphongKtnqCnl2 (ID: 54) - 6 chỉ tiêu
echo "📋 22. TruongphongKtnqCnl2 (6 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(54, N'Tổng nguồn vốn', 10, N'Triệu VND', 1, 4, 1),
(54, N'Lợi nhuận khoán tài chính', 20, N'Triệu VND', 2, 4, 1),
(54, N'Thu dịch vụ thanh toán trong nước', 10, N'Triệu VND', 3, 4, 1),
(54, N'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng', 40, N'%', 4, 2, 1),
(54, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, N'%', 5, 2, 1),
(54, N'Kết quả thực hiện BQ của CB trong phòng', 10, N'%', 6, 2, 1);
"

# BẢNG 23: PhophongKtnqCnl2 (ID: 55) - 5 chỉ tiêu
echo "📋 23. PhophongKtnqCnl2 (5 chỉ tiêu):"

$SQLCMD "
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(55, N'Số bút toán giao dịch BQ', 40, N'BT', 1, 1, 1),
(55, N'Số bút toán hủy', 20, N'BT', 2, 1, 1),
(55, N'Thực hiện nhiệm vụ theo chương trình công tác', 25, N'%', 3, 2, 1),
(55, N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', 10, N'%', 4, 2, 1),
(55, N'Hoàn thành chỉ tiêu giao khoán SPDV', 5, N'%', 5, 2, 1);
"

echo ""
echo "✅ HOÀN THÀNH TẠO KPI INDICATORS!"
echo "================================="

# Kiểm tra kết quả
echo ""
echo "🔍 Kiểm tra kết quả..."
total_count=$($SQLCMD "SELECT COUNT(*) FROM KpiIndicators" | grep -o '[0-9]*' | head -1)

echo "📊 Tổng số KPI Indicators: $total_count/158"

if [ "$total_count" -eq 158 ]; then
    echo "🎉 ĐÃ TẠO ĐỦ 158 CHỈ TIÊU KPI!"
else
    echo "⚠️  Chỉ có $total_count/158 chỉ tiêu. Kiểm tra lại!"
fi

echo ""
echo "🎯 PHÂN BỐ THEO BẢNG KPI (CHÍNH XÁC):"
echo "====================================="
echo "1. TruongphongKhdn: 8 chỉ tiêu"
echo "2. TruongphongKhcn: 8 chỉ tiêu"
echo "3. PhophongKhdn: 8 chỉ tiêu"
echo "4. PhophongKhcn: 8 chỉ tiêu"
echo "5. TruongphongKhqlrr: 6 chỉ tiêu"
echo "6. PhophongKhqlrr: 6 chỉ tiêu"
echo "7. Cbtd: 8 chỉ tiêu"
echo "8. TruongphongKtnqCnl1: 6 chỉ tiêu"
echo "9. PhophongKtnqCnl1: 6 chỉ tiêu"
echo "10. Gdv: 6 chỉ tiêu"
echo "11. TqHkKtnb: 0 chỉ tiêu (đợi giao sau)"
echo "12. TruongphoItThKtgs: 5 chỉ tiêu"
echo "13. CBItThKtgsKhqlrr: 4 chỉ tiêu"
echo "14. GiamdocPgd: 9 chỉ tiêu"
echo "15. PhogiamdocPgd: 9 chỉ tiêu"
echo "16. PhogiamdocPgdCbtd: 8 chỉ tiêu"
echo "17. GiamdocCnl2: 11 chỉ tiêu"
echo "18. PhogiamdocCnl2Td: 8 chỉ tiêu"
echo "19. PhogiamdocCnl2Kt: 6 chỉ tiêu"
echo "20. TruongphongKhCnl2: 9 chỉ tiêu"
echo "21. PhophongKhCnl2: 8 chỉ tiêu"
echo "22. TruongphongKtnqCnl2: 6 chỉ tiêu"
echo "23. PhophongKtnqCnl2: 5 chỉ tiêu"
echo "────────────────────────────────────────────"
echo "TỔNG: 158 chỉ tiêu cho 23 bảng cán bộ ✅"
echo "(Ghi chú: TqHkKtnb chưa có chỉ tiêu cụ thể)"

echo ""
echo "🚀 CÁC BƯỚC TIẾP THEO:"
echo "====================="
echo "1. ✅ Kiểm tra frontend dropdown hiển thị đúng tên bảng KPI"
echo "2. 🔄 Tạo Employee KPI Assignments"
echo "3. 🔄 Tạo Unit KPI Scorings cho chi nhánh"
echo "4. 🔄 Import dữ liệu CSV cho 8 bảng core"
echo "5. 🔄 Tạo thêm roles/units nếu cần"
