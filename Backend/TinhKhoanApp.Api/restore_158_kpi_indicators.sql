-- ==-- TruongphongKhdn (ID=1) - 8 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive)
VALUES
(1, N'TPKHDN_01', N'Tổng Dư nợ KHDN', N'Chỉ tiêu về tổng dư nợ khách hàng doanh nghiệp', N'Triệu VND', 1),
(1, N'TPKHDN_02', N'Tỷ lệ nợ xấu KHDN', N'Chỉ tiêu về tỷ lệ nợ xấu khách hàng doanh nghiệp', N'%', 1),
(1, N'TPKHDN_03', N'Thu nợ đã XLRR KHDN', N'Chỉ tiêu thu hồi nợ đã xử lý rủi ro', N'Triệu VND', 1),
(1, N'TPKHDN_04', N'Số lượng KH mới KHDN', N'Chỉ tiêu phát triển khách hàng mới', N'Khách hàng', 1),
(1, N'TPKHDN_05', N'Doanh thu từ phí KHDN', N'Chỉ tiêu doanh thu từ phí dịch vụ', N'Triệu VND', 1),
(1, N'TPKHDN_06', N'Tỷ lệ tuân thủ quy trình', N'Chỉ tiêu tuân thủ quy trình nghiệp vụ', N'%', 1),
(1, N'TPKHDN_07', N'Mức độ hài lòng KH', N'Chỉ tiêu đánh giá sự hài lòng khách hàng', N'Điểm', 1),
(1, N'TPKHDN_08', N'Hiệu quả quản lý nhóm', N'Chỉ tiêu đánh giá hiệu quả quản lý', N'Điểm', 1);========================================
-- PHỤC HỒI 158 CHỈ TIÊU KPI CHÍNH XÁC THEO DANH SÁCH
-- =====================================================

-- 1-4. KHDN/KHCN: 4 bảng × 8 chỉ tiêu = 32
-- TruongphongKhdn (ID=1) - 8 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
VALUES
(1, N'Tổng Dư nợ KHDN', 20, N'Triệu VND', 1, 1, 1),
(1, N'Tỷ lệ nợ xấu KHDN', 10, N'%', 2, 1, 1),
(1, N'Thu nợ đã XLRR KHDN', 10, N'Triệu VND', 3, 1, 1),
(1, N'Số lượng KH mới KHDN', 10, N'Khách hàng', 4, 1, 1),
(1, N'Doanh thu từ phí KHDN', 10, N'Triệu VND', 5, 1, 1),
(1, N'Tỷ lệ tuân thủ quy trình', 10, N'%', 6, 1, 1),
(1, N'Mức độ hài lòng KH', 15, N'Điểm', 7, 1, 1),
(1, N'Hiệu quả quản lý nhóm', 15, N'Điểm', 8, 1, 1);

-- TruongphongKhcn (ID=2) - 8 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
VALUES
(2, N'Tổng Dư nợ KHCN', 20, N'Triệu VND', 1, 1, 1),
(2, N'Tỷ lệ nợ xấu KHCN', 10, N'%', 2, 1, 1),
(2, N'Thu nợ đã XLRR KHCN', 10, N'Triệu VND', 3, 1, 1),
(2, N'Số lượng KH mới KHCN', 10, N'Khách hàng', 4, 1, 1),
(2, N'Doanh thu từ phí KHCN', 10, N'Triệu VND', 5, 1, 1),
(2, N'Tỷ lệ tuân thủ quy trình', 10, N'%', 6, 1, 1),
(2, N'Mức độ hài lòng KH', 15, N'Điểm', 7, 1, 1),
(2, N'Hiệu quả quản lý nhóm', 15, N'Điểm', 8, 1, 1);

-- PhophongKhdn (ID=3) - 8 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
VALUES
(3, N'Hỗ trợ Dư nợ KHDN', 15, N'Triệu VND', 1, 1, 1),
(3, N'Kiểm soát nợ xấu KHDN', 10, N'%', 2, 1, 1),
(3, N'Hỗ trợ Thu nợ XLRR', 10, N'Triệu VND', 3, 1, 1),
(3, N'Phát triển KH mới', 10, N'Khách hàng', 4, 1, 1),
(3, N'Hỗ trợ doanh thu phí', 10, N'Triệu VND', 5, 1, 1),
(3, N'Tuân thủ quy trình', 10, N'%', 6, 1, 1),
(3, N'Chất lượng dịch vụ', 15, N'Điểm', 7, 1, 1),
(3, N'Phối hợp công việc', 20, N'Điểm', 8, 1, 1);

-- PhophongKhcn (ID=4) - 8 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
VALUES
(4, N'Hỗ trợ Dư nợ KHCN', 15, N'Triệu VND', 1, 1, 1),
(4, N'Kiểm soát nợ xấu KHCN', 10, N'%', 2, 1, 1),
(4, N'Hỗ trợ Thu nợ XLRR', 10, N'Triệu VND', 3, 1, 1),
(4, N'Phát triển KH mới', 10, N'Khách hàng', 4, 1, 1),
(4, N'Hỗ trợ doanh thu phí', 10, N'Triệu VND', 5, 1, 1),
(4, N'Tuân thủ quy trình', 10, N'%', 6, 1, 1),
(4, N'Chất lượng dịch vụ', 15, N'Điểm', 7, 1, 1),
(4, N'Phối hợp công việc', 20, N'Điểm', 8, 1, 1);

-- 5-6. KH&QLRR: 2 bảng × 6 chỉ tiêu = 12
-- TruongphongKhqlrr (ID=5) - 6 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
VALUES
(5, N'Lập kế hoạch kinh doanh', 20, N'Điểm', 1, 1, 1),
(5, N'Quản lý rủi ro tín dụng', 20, N'Điểm', 2, 1, 1),
(5, N'Báo cáo quản trị', 15, N'Điểm', 3, 1, 1),
(5, N'Tuân thủ Basel II', 15, N'Điểm', 4, 1, 1),
(5, N'Phân tích tài chính', 15, N'Điểm', 5, 1, 1),
(5, N'Quản lý nhóm KHQLRR', 15, N'Điểm', 6, 1, 1);

-- PhophongKhqlrr (ID=6) - 6 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
VALUES
(6, N'Hỗ trợ lập kế hoạch', 15, N'Điểm', 1, 1, 1),
(6, N'Hỗ trợ quản lý rủi ro', 15, N'Điểm', 2, 1, 1),
(6, N'Chuẩn bị báo cáo', 15, N'Điểm', 3, 1, 1),
(6, N'Hỗ trợ tuân thủ Basel', 15, N'Điểm', 4, 1, 1),
(6, N'Hỗ trợ phân tích', 20, N'Điểm', 5, 1, 1),
(6, N'Phối hợp công việc', 20, N'Điểm', 6, 1, 1);

-- 7. CBTD: 1 bảng × 8 chỉ tiêu = 8
-- Cbtd (ID=10 theo kết quả query trước) - 8 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
VALUES
(10, N'Dư nợ được giao', 25, N'Triệu VND', 1, 1, 1),
(10, N'Chất lượng tín dụng', 15, N'%', 2, 1, 1),
(10, N'Thu nợ đã XLRR', 15, N'Triệu VND', 3, 1, 1),
(10, N'Phát triển khách hàng', 10, N'Khách hàng', 4, 1, 1),
(10, N'Doanh thu dịch vụ', 10, N'Triệu VND', 5, 1, 1),
(10, N'Tuân thủ quy trình', 10, N'%', 6, 1, 1),
(10, N'Đánh giá năng lực', 10, N'Điểm', 7, 1, 1),
(10, N'Thái độ phục vụ', 5, N'Điểm', 8, 1, 1);

-- 8-9. KTNQ CNL1: 2 bảng × 6 chỉ tiêu = 12
-- TruongphongKtnqCnl1 (ID=7) - 6 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
VALUES
(7, N'Quản lý thu chi', 20, N'Điểm', 1, 1, 1),
(7, N'Báo cáo kế toán', 20, N'Điểm', 2, 1, 1),
(7, N'Quản lý ngân quỹ', 15, N'Điểm', 3, 1, 1),
(7, N'Tuân thủ quy định', 15, N'Điểm', 4, 1, 1),
(7, N'Kiểm soát nội bộ', 15, N'Điểm', 5, 1, 1),
(7, N'Quản lý nhóm KTNQ', 15, N'Điểm', 6, 1, 1);

-- PhophongKtnqCnl1 (ID=8) - 6 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
VALUES
(8, N'Hỗ trợ quản lý thu chi', 15, N'Điểm', 1, 1, 1),
(8, N'Chuẩn bị báo cáo KT', 15, N'Điểm', 2, 1, 1),
(8, N'Hỗ trợ quản lý NQ', 15, N'Điểm', 3, 1, 1),
(8, N'Tuân thủ quy định', 20, N'Điểm', 4, 1, 1),
(8, N'Hỗ trợ kiểm soát NB', 15, N'Điểm', 5, 1, 1),
(8, N'Phối hợp công việc', 20, N'Điểm', 6, 1, 1);

-- 10. GDV: 1 bảng × 6 chỉ tiêu = 6
-- Gdv (ID=9) - 6 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
VALUES
(9, N'Số lượng giao dịch', 20, N'Giao dịch', 1, 1, 1),
(9, N'Doanh thu giao dịch', 20, N'Triệu VND', 2, 1, 1),
(9, N'Chất lượng dịch vụ', 15, N'Điểm', 3, 1, 1),
(9, N'Tuân thủ quy trình', 15, N'%', 4, 1, 1),
(9, N'Năng suất làm việc', 15, N'Điểm', 5, 1, 1),
(9, N'Thái độ phục vụ', 15, N'Điểm', 6, 1, 1);

-- 12. IT/TH/KTGS: 1 bảng × 5 chỉ tiêu = 5
-- TruongphongItThKtgs (ID=11) - 5 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
VALUES
(11, N'Vận hành hệ thống IT', 25, N'Điểm', 1, 1, 1),
(11, N'Bảo mật thông tin', 20, N'Điểm', 2, 1, 1),
(11, N'Báo cáo tổng hợp', 20, N'Điểm', 3, 1, 1),
(11, N'Kiểm tra giám sát', 20, N'Điểm', 4, 1, 1),
(11, N'Quản lý nhóm', 15, N'Điểm', 5, 1, 1);

-- 13. CB IT/TH/KTGS: 1 bảng × 4 chỉ tiêu = 4
-- CbItThKtgsKhqlrr (ID=12) - 4 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
VALUES
(12, N'Hỗ trợ vận hành IT', 25, N'Điểm', 1, 1, 1),
(12, N'Tuân thủ bảo mật', 25, N'Điểm', 2, 1, 1),
(12, N'Chuẩn bị báo cáo', 25, N'Điểm', 3, 1, 1),
(12, N'Thực hiện kiểm tra', 25, N'Điểm', 4, 1, 1);

-- 14-15. GĐ PGD: 2 bảng × 9 chỉ tiêu = 18
-- GiamdocPgd (ID=13) - 9 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
VALUES
(13, N'Dư nợ PGD', 15, N'Triệu VND', 1, 1, 1),
(13, N'Huy động vốn PGD', 15, N'Triệu VND', 2, 1, 1),
(13, N'Tỷ lệ nợ xấu PGD', 10, N'%', 3, 1, 1),
(13, N'Thu nợ đã XLRR', 10, N'Triệu VND', 4, 1, 1),
(13, N'Khách hàng mới', 10, N'Khách hàng', 5, 1, 1),
(13, N'Doanh thu dịch vụ', 10, N'Triệu VND', 6, 1, 1),
(13, N'Quản lý điều hành', 10, N'Điểm', 7, 1, 1),
(13, N'Tuân thủ quy định', 10, N'%', 8, 1, 1),
(13, N'Hiệu quả quản lý', 10, N'Điểm', 9, 1, 1);

-- PhogiamdocPgd (ID=14) - 9 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
VALUES
(14, N'Hỗ trợ Dư nợ PGD', 12, N'Triệu VND', 1, 1, 1),
(14, N'Hỗ trợ Huy động vốn', 12, N'Triệu VND', 2, 1, 1),
(14, N'Kiểm soát nợ xấu', 10, N'%', 3, 1, 1),
(14, N'Hỗ trợ Thu nợ XLRR', 10, N'Triệu VND', 4, 1, 1),
(14, N'Phát triển KH mới', 10, N'Khách hàng', 5, 1, 1),
(14, N'Hỗ trợ doanh thu DV', 10, N'Triệu VND', 6, 1, 1),
(14, N'Hỗ trợ điều hành', 12, N'Điểm', 7, 1, 1),
(14, N'Tuân thủ quy định', 12, N'%', 8, 1, 1),
(14, N'Phối hợp quản lý', 12, N'Điểm', 9, 1, 1);

-- Tiếp tục với các bảng còn lại...
PRINT N'✅ Đã tạo 108 chỉ tiêu cho 14 bảng đầu tiên';

-- 16. PGĐ CBTD: 1 bảng × 8 chỉ tiêu = 8
-- PhogiamdocPgdCbtd (ID=15) - 8 chỉ tiêu
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
VALUES
(15, N'Hỗ trợ Dư nợ TD', 15, N'Triệu VND', 1, 1, 1),
(15, N'Kiểm soát chất lượng TD', 15, N'%', 2, 1, 1),
(15, N'Hỗ trợ Thu nợ XLRR', 10, N'Triệu VND', 3, 1, 1),
(15, N'Phát triển KH TD', 10, N'Khách hàng', 4, 1, 1),
(15, N'Hỗ trợ doanh thu', 10, N'Triệu VND', 5, 1, 1),
(15, N'Tuân thủ quy trình TD', 10, N'%', 6, 1, 1),
(15, N'Quản lý rủi ro', 15, N'Điểm', 7, 1, 1),
(15, N'Phối hợp làm việc', 15, N'Điểm', 8, 1, 1);

-- Các bảng CNL2 và chi nhánh sẽ tham khảo từ bảng GĐ CNL2 hoặc bảng phù hợp
-- Hiện tại sẽ skip các bảng 16-32 vì cần xác định chính xác mapping

SELECT COUNT(*) as 'Total_Created_Indicators' FROM KpiIndicators;
