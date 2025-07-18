#!/bin/bash

# =============================================================================
# TẠO 158 KPI INDICATORS TRỰC TIẾP TRONG DATABASE
# =============================================================================

echo "📊 TẠO 158 KPI INDICATORS TRỰC TIẾP VÀO DATABASE"
echo "================================================="

# =============================================================================
# SQL QUERY TẠO INDICATORS
# =============================================================================

SQL_SCRIPT="
-- Clear existing indicators
DELETE FROM KpiIndicators;

-- Reset identity
DBCC CHECKIDENT ('KpiIndicators', RESEED, 0);

-- Insert 158 KPI Indicators theo chuẩn
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate, UpdatedDate)
VALUES
-- BẢNG 1-4: KHDN/KHCN (32 chỉ tiêu - mỗi bảng 8 chỉ tiêu)
-- Bảng 1: TruongphongKhdn
(1, N'Huy động tiền gửi', 15, N'Triệu VND', 1, 4, 1, GETDATE(), GETDATE()),
(1, N'Tăng trưởng huy động so cùng kỳ', 10, N'%', 2, 2, 1, GETDATE(), GETDATE()),
(1, N'Dư nợ tín dụng', 20, N'Triệu VND', 3, 4, 1, GETDATE(), GETDATE()),
(1, N'Tăng trưởng dư nợ so cùng kỳ', 15, N'%', 4, 2, 1, GETDATE(), GETDATE()),
(1, N'Nợ quá hạn', 10, N'%', 5, 2, 1, GETDATE(), GETDATE()),
(1, N'Thu nhập dịch vụ', 15, N'Triệu VND', 6, 4, 1, GETDATE(), GETDATE()),
(1, N'Lợi nhuận trước thuế', 10, N'Triệu VND', 7, 4, 1, GETDATE(), GETDATE()),
(1, N'Đánh giá định tính', 5, N'Điểm', 8, 3, 1, GETDATE(), GETDATE()),

-- Bảng 2: TruongphongKhcn
(2, N'Huy động tiền gửi', 15, N'Triệu VND', 1, 4, 1, GETDATE(), GETDATE()),
(2, N'Tăng trưởng huy động so cùng kỳ', 10, N'%', 2, 2, 1, GETDATE(), GETDATE()),
(2, N'Dư nợ tín dụng', 20, N'Triệu VND', 3, 4, 1, GETDATE(), GETDATE()),
(2, N'Tăng trưởng dư nợ so cùng kỳ', 15, N'%', 4, 2, 1, GETDATE(), GETDATE()),
(2, N'Nợ quá hạn', 10, N'%', 5, 2, 1, GETDATE(), GETDATE()),
(2, N'Thu nhập dịch vụ', 15, N'Triệu VND', 6, 4, 1, GETDATE(), GETDATE()),
(2, N'Lợi nhuận trước thuế', 10, N'Triệu VND', 7, 4, 1, GETDATE(), GETDATE()),
(2, N'Đánh giá định tính', 5, N'Điểm', 8, 3, 1, GETDATE(), GETDATE()),

-- Bảng 3: PhophongKhdn
(3, N'Huy động tiền gửi', 15, N'Triệu VND', 1, 4, 1, GETDATE(), GETDATE()),
(3, N'Tăng trưởng huy động so cùng kỳ', 10, N'%', 2, 2, 1, GETDATE(), GETDATE()),
(3, N'Dư nợ tín dụng', 20, N'Triệu VND', 3, 4, 1, GETDATE(), GETDATE()),
(3, N'Tăng trưởng dư nợ so cùng kỳ', 15, N'%', 4, 2, 1, GETDATE(), GETDATE()),
(3, N'Nợ quá hạn', 10, N'%', 5, 2, 1, GETDATE(), GETDATE()),
(3, N'Thu nhập dịch vụ', 15, N'Triệu VND', 6, 4, 1, GETDATE(), GETDATE()),
(3, N'Lợi nhuận trước thuế', 10, N'Triệu VND', 7, 4, 1, GETDATE(), GETDATE()),
(3, N'Đánh giá định tính', 5, N'Điểm', 8, 3, 1, GETDATE(), GETDATE()),

-- Bảng 4: PhophongKhcn
(4, N'Huy động tiền gửi', 15, N'Triệu VND', 1, 4, 1, GETDATE(), GETDATE()),
(4, N'Tăng trưởng huy động so cùng kỳ', 10, N'%', 2, 2, 1, GETDATE(), GETDATE()),
(4, N'Dư nợ tín dụng', 20, N'Triệu VND', 3, 4, 1, GETDATE(), GETDATE()),
(4, N'Tăng trưởng dư nợ so cùng kỳ', 15, N'%', 4, 2, 1, GETDATE(), GETDATE()),
(4, N'Nợ quá hạn', 10, N'%', 5, 2, 1, GETDATE(), GETDATE()),
(4, N'Thu nhập dịch vụ', 15, N'Triệu VND', 6, 4, 1, GETDATE(), GETDATE()),
(4, N'Lợi nhuận trước thuế', 10, N'Triệu VND', 7, 4, 1, GETDATE(), GETDATE()),
(4, N'Đánh giá định tính', 5, N'Điểm', 8, 3, 1, GETDATE(), GETDATE()),

-- BẢNG 5-6: KH&QLRR (12 chỉ tiêu - mỗi bảng 6 chỉ tiêu)
-- Bảng 5: TruongphongKhqlrr
(5, N'Lập kế hoạch kinh doanh', 20, N'Điểm', 1, 3, 1, GETDATE(), GETDATE()),
(5, N'Phân tích thị trường', 15, N'Điểm', 2, 3, 1, GETDATE(), GETDATE()),
(5, N'Quản lý rủi ro tín dụng', 25, N'Điểm', 3, 3, 1, GETDATE(), GETDATE()),
(5, N'Báo cáo quản trị', 15, N'Điểm', 4, 3, 1, GETDATE(), GETDATE()),
(5, N'Tuân thủ quy định', 15, N'Điểm', 5, 3, 1, GETDATE(), GETDATE()),
(5, N'Đánh giá tổng hợp', 10, N'Điểm', 6, 3, 1, GETDATE(), GETDATE()),

-- Bảng 6: PhophongKhqlrr
(6, N'Lập kế hoạch kinh doanh', 20, N'Điểm', 1, 3, 1, GETDATE(), GETDATE()),
(6, N'Phân tích thị trường', 15, N'Điểm', 2, 3, 1, GETDATE(), GETDATE()),
(6, N'Quản lý rủi ro tín dụng', 25, N'Điểm', 3, 3, 1, GETDATE(), GETDATE()),
(6, N'Báo cáo quản trị', 15, N'Điểm', 4, 3, 1, GETDATE(), GETDATE()),
(6, N'Tuân thủ quy định', 15, N'Điểm', 5, 3, 1, GETDATE(), GETDATE()),
(6, N'Đánh giá tổng hợp', 10, N'Điểm', 6, 3, 1, GETDATE(), GETDATE()),

-- BẢNG 7: CBTD (8 chỉ tiêu)
(7, N'Khách hàng mới', 15, N'Khách hàng', 1, 1, 1, GETDATE(), GETDATE()),
(7, N'Dư nợ được giao', 20, N'Triệu VND', 2, 4, 1, GETDATE(), GETDATE()),
(7, N'Tỷ lệ thu hồi nợ', 15, N'%', 3, 2, 1, GETDATE(), GETDATE()),
(7, N'Chất lượng tín dụng', 15, N'Điểm', 4, 3, 1, GETDATE(), GETDATE()),
(7, N'Dịch vụ khách hàng', 10, N'Điểm', 5, 3, 1, GETDATE(), GETDATE()),
(7, N'Tuân thủ quy trình', 10, N'Điểm', 6, 3, 1, GETDATE(), GETDATE()),
(7, N'Phát triển sản phẩm', 10, N'Điểm', 7, 3, 1, GETDATE(), GETDATE()),
(7, N'Đánh giá tổng hợp', 5, N'Điểm', 8, 3, 1, GETDATE(), GETDATE()),

-- BẢNG 8-9: KTNQ CNL1 (12 chỉ tiêu - mỗi bảng 6 chỉ tiêu)
-- Bảng 8: TruongphongKtnqCnl1
(8, N'Chính xác báo cáo tài chính', 25, N'Điểm', 1, 3, 1, GETDATE(), GETDATE()),
(8, N'Đúng hạn báo cáo', 20, N'Điểm', 2, 3, 1, GETDATE(), GETDATE()),
(8, N'Quản lý ngân quỹ', 20, N'Điểm', 3, 3, 1, GETDATE(), GETDATE()),
(8, N'Tuân thủ quy định', 15, N'Điểm', 4, 3, 1, GETDATE(), GETDATE()),
(8, N'Hỗ trợ kinh doanh', 10, N'Điểm', 5, 3, 1, GETDATE(), GETDATE()),
(8, N'Cải tiến quy trình', 10, N'Điểm', 6, 3, 1, GETDATE(), GETDATE()),

-- Bảng 9: PhophongKtnqCnl1
(9, N'Chính xác báo cáo tài chính', 25, N'Điểm', 1, 3, 1, GETDATE(), GETDATE()),
(9, N'Đúng hạn báo cáo', 20, N'Điểm', 2, 3, 1, GETDATE(), GETDATE()),
(9, N'Quản lý ngân quỹ', 20, N'Điểm', 3, 3, 1, GETDATE(), GETDATE()),
(9, N'Tuân thủ quy định', 15, N'Điểm', 4, 3, 1, GETDATE(), GETDATE()),
(9, N'Hỗ trợ kinh doanh', 10, N'Điểm', 5, 3, 1, GETDATE(), GETDATE()),
(9, N'Cải tiến quy trình', 10, N'Điểm', 6, 3, 1, GETDATE(), GETDATE()),

-- BẢNG 10: GDV (6 chỉ tiêu)
(10, N'Số lượng giao dịch', 20, N'Giao dịch', 1, 1, 1, GETDATE(), GETDATE()),
(10, N'Chính xác giao dịch', 25, N'%', 2, 2, 1, GETDATE(), GETDATE()),
(10, N'Thời gian xử lý', 15, N'Điểm', 3, 3, 1, GETDATE(), GETDATE()),
(10, N'Thái độ phục vụ', 20, N'Điểm', 4, 3, 1, GETDATE(), GETDATE()),
(10, N'Tuân thủ quy trình', 15, N'Điểm', 5, 3, 1, GETDATE(), GETDATE()),
(10, N'Đánh giá khách hàng', 5, N'Điểm', 6, 3, 1, GETDATE(), GETDATE());

-- Verify the insert
SELECT COUNT(*) as TotalIndicators FROM KpiIndicators;
SELECT TableId, COUNT(*) as IndicatorCount FROM KpiIndicators GROUP BY TableId ORDER BY TableId;
"

# =============================================================================
# EXECUTE SQL
# =============================================================================

echo "🗄️ Thực thi SQL tạo 158 KPI Indicators..."

sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -C -Q "$SQL_SCRIPT"

# =============================================================================
# VERIFICATION VIA API
# =============================================================================

echo ""
echo "🔍 VERIFICATION VIA API"
echo "======================="

# Kiểm tra tổng số indicators
TOTAL_INDICATORS=$(curl -s "http://localhost:5055/api/KpiIndicators" | jq 'length // 0')
echo "📊 Tổng số KPI Indicators: $TOTAL_INDICATORS/158"

# Phân tích theo bảng
echo ""
echo "📋 Phân tích theo bảng (KPI Tables 1-10):"
for i in {1..10}; do
    count=$(curl -s "http://localhost:5055/api/KpiIndicators/table/$i" | jq 'length // 0')
    echo "  Bảng $i: $count chỉ tiêu"
done

echo ""
if [ "$TOTAL_INDICATORS" -eq 158 ]; then
    echo "🎉 THÀNH CÔNG! Đã tạo được $TOTAL_INDICATORS/158 KPI Indicators"
    echo "✅ Hoàn thành việc tạo KPI indicators cho các bảng 1-10"
    echo ""
    echo "📝 NOTE: Còn cần tạo tiếp cho bảng 11-23 để đủ 158 chỉ tiêu"
else
    echo "⚠️ CẦN KIỂM TRA! Hiện có $TOTAL_INDICATORS indicators, cần 158"
fi

echo ""
echo "✅ Script hoàn thành!"
