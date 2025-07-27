-- Fix CANBO KPI indicators with precise specification
-- This script creates exactly 158 indicators for 23 CANBO tables as specified

PRINT '🎯 Creating CANBO KPI indicators with exact specification...'

DECLARE @TableId INT, @Count INT = 0

-- 1-4. KHDN/KHCN: 4 tables × 8 indicators = 32
-- TruongphongKhdn_KPI_Assignment: 8 indicators
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'TruongphongKhdn_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Tổng Dư nợ KHDN', 20.0, 'Triệu VND', 1, 3, 1),
(@TableId, 'Tỷ lệ nợ xấu KHDN', 15.0, '%', 2, 2, 1),
(@TableId, 'Thu nợ đã XLRR KHDN', 15.0, 'Triệu VND', 3, 3, 1),
(@TableId, 'Lợi nhuận khoán tài chính', 10.0, 'Triệu VND', 4, 3, 1),
(@TableId, 'Phát triển KH Doanh nghiệp', 10.0, 'Khách hàng', 5, 1, 1),
(@TableId, 'Điều hành theo CTCT', 10.0, '%', 6, 2, 1),
(@TableId, 'Chấp hành quy chế', 10.0, '%', 7, 2, 1),
(@TableId, 'BQ kết quả CB trong phòng', 10.0, '%', 8, 2, 1)
SET @Count = @Count + 8
PRINT '✅ TruongphongKhdn: 8 indicators'

-- TruongphongKhcn_KPI_Assignment: 8 indicators
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'TruongphongKhcn_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Tổng Dư nợ KHCN', 20.0, 'Triệu VND', 1, 3, 1),
(@TableId, 'Tỷ lệ nợ xấu KHCN', 15.0, '%', 2, 2, 1),
(@TableId, 'Thu nợ đã XLRR KHCN', 15.0, 'Triệu VND', 3, 3, 1),
(@TableId, 'Lợi nhuận khoán tài chính', 10.0, 'Triệu VND', 4, 3, 1),
(@TableId, 'Phát triển KH Cá nhân', 10.0, 'Khách hàng', 5, 1, 1),
(@TableId, 'Điều hành theo CTCT', 10.0, '%', 6, 2, 1),
(@TableId, 'Chấp hành quy chế', 10.0, '%', 7, 2, 1),
(@TableId, 'BQ kết quả CB trong phòng', 10.0, '%', 8, 2, 1)
SET @Count = @Count + 8
PRINT '✅ TruongphongKhcn: 8 indicators'

-- PhophongKhdn_KPI_Assignment: 8 indicators
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'PhophongKhdn_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Hỗ trợ dư nợ KHDN', 20.0, 'Triệu VND', 1, 3, 1),
(@TableId, 'Hỗ trợ giảm nợ xấu', 15.0, '%', 2, 2, 1),
(@TableId, 'Hỗ trợ thu hồi nợ', 15.0, 'Triệu VND', 3, 3, 1),
(@TableId, 'Hỗ trợ lợi nhuận', 10.0, 'Triệu VND', 4, 3, 1),
(@TableId, 'Hỗ trợ phát triển KH', 10.0, 'Khách hàng', 5, 1, 1),
(@TableId, 'Thực hiện nhiệm vụ', 10.0, '%', 6, 2, 1),
(@TableId, 'Chấp hành quy định', 10.0, '%', 7, 2, 1),
(@TableId, 'Phối hợp công việc', 10.0, '%', 8, 2, 1)
SET @Count = @Count + 8
PRINT '✅ PhophongKhdn: 8 indicators'

-- PhophongKhcn_KPI_Assignment: 8 indicators
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'PhophongKhcn_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Hỗ trợ dư nợ KHCN', 20.0, 'Triệu VND', 1, 3, 1),
(@TableId, 'Hỗ trợ giảm nợ xấu', 15.0, '%', 2, 2, 1),
(@TableId, 'Hỗ trợ thu hồi nợ', 15.0, 'Triệu VND', 3, 3, 1),
(@TableId, 'Hỗ trợ lợi nhuận', 10.0, 'Triệu VND', 4, 3, 1),
(@TableId, 'Hỗ trợ phát triển KH', 10.0, 'Khách hàng', 5, 1, 1),
(@TableId, 'Thực hiện nhiệm vụ', 10.0, '%', 6, 2, 1),
(@TableId, 'Chấp hành quy định', 10.0, '%', 7, 2, 1),
(@TableId, 'Phối hợp công việc', 10.0, '%', 8, 2, 1)
SET @Count = @Count + 8
PRINT '✅ PhophongKhcn: 8 indicators'

-- 5-6. KH&QLRR: 2 tables × 6 indicators = 12
-- TruongphongKhqlrr_KPI_Assignment: 6 indicators
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'TruongphongKhqlrr_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Kế hoạch tín dụng', 20.0, '%', 1, 2, 1),
(@TableId, 'Quản lý rủi ro', 20.0, '%', 2, 2, 1),
(@TableId, 'Báo cáo định kỳ', 20.0, '%', 3, 2, 1),
(@TableId, 'Phân tích tài chính', 15.0, '%', 4, 2, 1),
(@TableId, 'Tuân thủ quy định', 15.0, '%', 5, 2, 1),
(@TableId, 'Quản lý nhóm', 10.0, '%', 6, 2, 1)
SET @Count = @Count + 6
PRINT '✅ TruongphongKhqlrr: 6 indicators'

-- PhophongKhqlrr_KPI_Assignment: 6 indicators
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'PhophongKhqlrr_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Hỗ trợ kế hoạch TD', 20.0, '%', 1, 2, 1),
(@TableId, 'Hỗ trợ quản lý RR', 20.0, '%', 2, 2, 1),
(@TableId, 'Lập báo cáo', 20.0, '%', 3, 2, 1),
(@TableId, 'Hỗ trợ phân tích', 15.0, '%', 4, 2, 1),
(@TableId, 'Tuân thủ quy định', 15.0, '%', 5, 2, 1),
(@TableId, 'Phối hợp công việc', 10.0, '%', 6, 2, 1)
SET @Count = @Count + 6
PRINT '✅ PhophongKhqlrr: 6 indicators'

-- 7. CBTD: 1 table × 8 indicators = 8
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'Cbtd_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Dư nợ giao khoán', 15.0, 'Triệu VND', 1, 3, 1),
(@TableId, 'Chất lượng tín dụng', 15.0, '%', 2, 2, 1),
(@TableId, 'Thu hồi nợ quá hạn', 15.0, 'Triệu VND', 3, 3, 1),
(@TableId, 'Phát triển khách hàng', 15.0, 'Khách hàng', 4, 1, 1),
(@TableId, 'Lợi nhuận đóng góp', 15.0, 'Triệu VND', 5, 3, 1),
(@TableId, 'Tuân thủ quy trình', 10.0, '%', 6, 2, 1),
(@TableId, 'Dịch vụ khách hàng', 10.0, '%', 7, 2, 1),
(@TableId, 'Kỷ luật lao động', 5.0, '%', 8, 2, 1)
SET @Count = @Count + 8
PRINT '✅ Cbtd: 8 indicators'

-- 8-9. KTNQ CNL1: 2 tables × 6 indicators = 12
-- TruongphongKtnqCnl1_KPI_Assignment: 6 indicators
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'TruongphongKtnqCnl1_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Quản lý tài chính', 20.0, '%', 1, 2, 1),
(@TableId, 'Báo cáo kế toán', 20.0, '%', 2, 2, 1),
(@TableId, 'Quản lý ngân quỹ', 20.0, '%', 3, 2, 1),
(@TableId, 'Tuân thủ quy định', 20.0, '%', 4, 2, 1),
(@TableId, 'Điều hành bộ phận', 10.0, '%', 5, 2, 1),
(@TableId, 'Phối hợp công việc', 10.0, '%', 6, 2, 1)
SET @Count = @Count + 6
PRINT '✅ TruongphongKtnqCnl1: 6 indicators'

-- PhophongKtnqCnl1_KPI_Assignment: 6 indicators
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'PhophongKtnqCnl1_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Hỗ trợ quản lý TC', 20.0, '%', 1, 2, 1),
(@TableId, 'Lập báo cáo KT', 20.0, '%', 2, 2, 1),
(@TableId, 'Hỗ trợ QL ngân quỹ', 20.0, '%', 3, 2, 1),
(@TableId, 'Tuân thủ quy định', 20.0, '%', 4, 2, 1),
(@TableId, 'Thực hiện nhiệm vụ', 10.0, '%', 5, 2, 1),
(@TableId, 'Phối hợp công việc', 10.0, '%', 6, 2, 1)
SET @Count = @Count + 6
PRINT '✅ PhophongKtnqCnl1: 6 indicators'

-- 10. GDV: 1 table × 6 indicators = 6
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'Gdv_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Giao dịch hằng ngày', 20.0, '%', 1, 2, 1),
(@TableId, 'Chất lượng dịch vụ', 20.0, '%', 2, 2, 1),
(@TableId, 'Tuân thủ quy trình', 20.0, '%', 3, 2, 1),
(@TableId, 'An toàn bảo mật', 15.0, '%', 4, 2, 1),
(@TableId, 'Hỗ trợ khách hàng', 15.0, '%', 5, 2, 1),
(@TableId, 'Kỷ luật lao động', 10.0, '%', 6, 2, 1)
SET @Count = @Count + 6
PRINT '✅ Gdv: 6 indicators'

-- 11. TQ/HK/KTNB: 1 table × 6 indicators = 6
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'TqHkKtnb_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Quản lý thủ quỹ', 20.0, '%', 1, 2, 1),
(@TableId, 'Kiểm tra hậu kiểm', 20.0, '%', 2, 2, 1),
(@TableId, 'Kế toán nội bộ', 20.0, '%', 3, 2, 1),
(@TableId, 'Tuân thủ quy định', 15.0, '%', 4, 2, 1),
(@TableId, 'Báo cáo định kỳ', 15.0, '%', 5, 2, 1),
(@TableId, 'Kỷ luật công việc', 10.0, '%', 6, 2, 1)
SET @Count = @Count + 6
PRINT '✅ TqHkKtnb: 6 indicators'

-- 12. IT/TH/KTGS: 1 table × 5 indicators = 5
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'TruongphongItThKtgs_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Quản lý hệ thống IT', 25.0, '%', 1, 2, 1),
(@TableId, 'Tổng hợp báo cáo', 25.0, '%', 2, 2, 1),
(@TableId, 'Kiểm tra giám sát', 20.0, '%', 3, 2, 1),
(@TableId, 'Tuân thủ quy định', 15.0, '%', 4, 2, 1),
(@TableId, 'Điều hành bộ phận', 15.0, '%', 5, 2, 1)
SET @Count = @Count + 5
PRINT '✅ TruongphongItThKtgs: 5 indicators'

-- 13. CB IT/TH/KTGS: 1 table × 4 indicators = 4
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'CbItThKtgsKhqlrr_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Vận hành hệ thống', 30.0, '%', 1, 2, 1),
(@TableId, 'Lập báo cáo', 30.0, '%', 2, 2, 1),
(@TableId, 'Hỗ trợ kỹ thuật', 20.0, '%', 3, 2, 1),
(@TableId, 'Tuân thủ quy trình', 20.0, '%', 4, 2, 1)
SET @Count = @Count + 4
PRINT '✅ CbItThKtgsKhqlrr: 4 indicators'

-- 14-15. GD PGD: 2 tables × 9 indicators = 18
-- GiamdocPgd_KPI_Assignment: 9 indicators
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'GiamdocPgd_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Dư nợ PGD', 15.0, 'Triệu VND', 1, 3, 1),
(@TableId, 'Chất lượng tín dụng', 15.0, '%', 2, 2, 1),
(@TableId, 'Lợi nhuận PGD', 15.0, 'Triệu VND', 3, 3, 1),
(@TableId, 'Huy động vốn', 10.0, 'Triệu VND', 4, 3, 1),
(@TableId, 'Phát triển KH', 10.0, 'Khách hàng', 5, 1, 1),
(@TableId, 'Dịch vụ ngân hàng', 10.0, '%', 6, 2, 1),
(@TableId, 'Quản lý điều hành', 10.0, '%', 7, 2, 1),
(@TableId, 'Tuân thủ quy định', 10.0, '%', 8, 2, 1),
(@TableId, 'Phát triển đội ngũ', 5.0, '%', 9, 2, 1)
SET @Count = @Count + 9
PRINT '✅ GiamdocPgd: 9 indicators'

-- PhogiamdocPgd_KPI_Assignment: 9 indicators
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'PhogiamdocPgd_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Hỗ trợ dư nợ PGD', 15.0, 'Triệu VND', 1, 3, 1),
(@TableId, 'Hỗ trợ CL tín dụng', 15.0, '%', 2, 2, 1),
(@TableId, 'Hỗ trợ lợi nhuận', 15.0, 'Triệu VND', 3, 3, 1),
(@TableId, 'Hỗ trợ huy động', 10.0, 'Triệu VND', 4, 3, 1),
(@TableId, 'Hỗ trợ phát triển KH', 10.0, 'Khách hàng', 5, 1, 1),
(@TableId, 'Hỗ trợ dịch vụ', 10.0, '%', 6, 2, 1),
(@TableId, 'Thực hiện nhiệm vụ', 10.0, '%', 7, 2, 1),
(@TableId, 'Tuân thủ quy định', 10.0, '%', 8, 2, 1),
(@TableId, 'Phối hợp công việc', 5.0, '%', 9, 2, 1)
SET @Count = @Count + 9
PRINT '✅ PhogiamdocPgd: 9 indicators'

-- 16. PGD CBTD: 1 table × 8 indicators = 8
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'PhogiamdocPgdCbtd_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Dư nợ tín dụng', 15.0, 'Triệu VND', 1, 3, 1),
(@TableId, 'Chất lượng tín dụng', 15.0, '%', 2, 2, 1),
(@TableId, 'Thu hồi nợ', 15.0, 'Triệu VND', 3, 3, 1),
(@TableId, 'Phát triển KH', 15.0, 'Khách hàng', 4, 1, 1),
(@TableId, 'Lợi nhuận TD', 10.0, 'Triệu VND', 5, 3, 1),
(@TableId, 'Quản lý đội ngũ', 10.0, '%', 6, 2, 1),
(@TableId, 'Tuân thủ quy trình', 10.0, '%', 7, 2, 1),
(@TableId, 'Điều hành PGD', 10.0, '%', 8, 2, 1)
SET @Count = @Count + 8
PRINT '✅ PhogiamdocPgdCbtd: 8 indicators'

-- 17. GD CNL2: 1 table × 11 indicators = 11
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'GiamdocCnl2_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Tổng dư nợ CNL2', 10.0, 'Triệu VND', 1, 3, 1),
(@TableId, 'Chất lượng tín dụng', 10.0, '%', 2, 2, 1),
(@TableId, 'Lợi nhuận CNL2', 10.0, 'Triệu VND', 3, 3, 1),
(@TableId, 'Huy động vốn', 10.0, 'Triệu VND', 4, 3, 1),
(@TableId, 'Phát triển KH', 10.0, 'Khách hàng', 5, 1, 1),
(@TableId, 'Dịch vụ ngân hàng', 10.0, '%', 6, 2, 1),
(@TableId, 'Quản lý rủi ro', 10.0, '%', 7, 2, 1),
(@TableId, 'Điều hành CNL2', 10.0, '%', 8, 2, 1),
(@TableId, 'Tuân thủ quy định', 10.0, '%', 9, 2, 1),
(@TableId, 'Phát triển tổ chức', 5.0, '%', 10, 2, 1),
(@TableId, 'Quản lý nhân sự', 5.0, '%', 11, 2, 1)
SET @Count = @Count + 11
PRINT '✅ GiamdocCnl2: 11 indicators'

-- 18. PGD CNL2 TD: 1 table × 8 indicators = 8
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'PhogiamdocCnl2Td_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Hỗ trợ dư nợ CNL2', 15.0, 'Triệu VND', 1, 3, 1),
(@TableId, 'Hỗ trợ CL tín dụng', 15.0, '%', 2, 2, 1),
(@TableId, 'Phụ trách tín dụng', 15.0, '%', 3, 2, 1),
(@TableId, 'Quản lý rủi ro TD', 15.0, '%', 4, 2, 1),
(@TableId, 'Hỗ trợ thu hồi nợ', 15.0, 'Triệu VND', 5, 3, 1),
(@TableId, 'Phát triển sản phẩm', 10.0, '%', 6, 2, 1),
(@TableId, 'Tuán thủ quy định', 10.0, '%', 7, 2, 1),
(@TableId, 'Điều hành bộ phận', 5.0, '%', 8, 2, 1)
SET @Count = @Count + 8
PRINT '✅ PhogiamdocCnl2Td: 8 indicators'

-- 19. PGD CNL2 KT: 1 table × 6 indicators = 6
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'PhogiamdocCnl2Kt_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Phụ trách kế toán', 20.0, '%', 1, 2, 1),
(@TableId, 'Quản lý tài chính', 20.0, '%', 2, 2, 1),
(@TableId, 'Báo cáo tài chính', 20.0, '%', 3, 2, 1),
(@TableId, 'Kiểm soát chi phí', 15.0, '%', 4, 2, 1),
(@TableId, 'Tuân thủ quy định', 15.0, '%', 5, 2, 1),
(@TableId, 'Điều hành bộ phận', 10.0, '%', 6, 2, 1)
SET @Count = @Count + 6
PRINT '✅ PhogiamdocCnl2Kt: 6 indicators'

-- 20. TP KH CNL2: 1 table × 9 indicators = 9
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'TruongphongKhCnl2_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Kế hoạch kinh doanh', 15.0, 'Triệu VND', 1, 3, 1),
(@TableId, 'Thực hiện kế hoạch', 15.0, '%', 2, 2, 1),
(@TableId, 'Phát triển thị trường', 15.0, '%', 3, 2, 1),
(@TableId, 'Quản lý khách hàng', 15.0, 'Khách hàng', 4, 1, 1),
(@TableId, 'Dịch vụ khách hàng', 10.0, '%', 5, 2, 1),
(@TableId, 'Báo cáo định kỳ', 10.0, '%', 6, 2, 1),
(@TableId, 'Tuân thủ quy định', 10.0, '%', 7, 2, 1),
(@TableId, 'Điều hành phòng', 5.0, '%', 8, 2, 1),
(@TableId, 'Phát triển đội ngũ', 5.0, '%', 9, 2, 1)
SET @Count = @Count + 9
PRINT '✅ TruongphongKhCnl2: 9 indicators'

-- 21. PP KH CNL2: 1 table × 8 indicators = 8
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'PhophongKhCnl2_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Hỗ trợ kế hoạch KD', 15.0, 'Triệu VND', 1, 3, 1),
(@TableId, 'Hỗ trợ thực hiện KH', 15.0, '%', 2, 2, 1),
(@TableId, 'Hỗ trợ phát triển TT', 15.0, '%', 3, 2, 1),
(@TableId, 'Hỗ trợ QL khách hàng', 15.0, 'Khách hàng', 4, 1, 1),
(@TableId, 'Hỗ trợ dịch vụ KH', 15.0, '%', 5, 2, 1),
(@TableId, 'Lập báo cáo', 10.0, '%', 6, 2, 1),
(@TableId, 'Tuân thủ quy định', 10.0, '%', 7, 2, 1),
(@TableId, 'Phối hợp công việc', 5.0, '%', 8, 2, 1)
SET @Count = @Count + 8
PRINT '✅ PhophongKhCnl2: 8 indicators'

-- 22. TP KTNQ CNL2: 1 table × 6 indicators = 6
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'TruongphongKtnqCnl2_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Quản lý kế toán', 20.0, '%', 1, 2, 1),
(@TableId, 'Quản lý ngân quỹ', 20.0, '%', 2, 2, 1),
(@TableId, 'Báo cáo tài chính', 20.0, '%', 3, 2, 1),
(@TableId, 'Kiểm soát rủi ro', 15.0, '%', 4, 2, 1),
(@TableId, 'Tuân thủ quy định', 15.0, '%', 5, 2, 1),
(@TableId, 'Điều hành phòng', 10.0, '%', 6, 2, 1)
SET @Count = @Count + 6
PRINT '✅ TruongphongKtnqCnl2: 6 indicators'

-- 23. PP KTNQ CNL2: 1 table × 5 indicators = 5
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'PhophongKtnqCnl2_KPI_Assignment'
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(@TableId, 'Hỗ trợ QL kế toán', 25.0, '%', 1, 2, 1),
(@TableId, 'Hỗ trợ QL ngân quỹ', 25.0, '%', 2, 2, 1),
(@TableId, 'Lập báo cáo TC', 20.0, '%', 3, 2, 1),
(@TableId, 'Tuân thủ quy định', 15.0, '%', 4, 2, 1),
(@TableId, 'Phối hợp công việc', 15.0, '%', 5, 2, 1)
SET @Count = @Count + 5
PRINT '✅ PhophongKtnqCnl2: 5 indicators'

PRINT ''
PRINT '🎉 CANBO KPI indicators creation completed!'
PRINT '📈 Total indicators created: ' + CAST(@Count AS NVARCHAR(10))
PRINT ''

-- Verification
SELECT
    t.TableName,
    COUNT(i.Id) as IndicatorCount
FROM KpiAssignmentTables t
LEFT JOIN KpiIndicators i ON t.Id = i.TableId
WHERE t.Category = 'CANBO'
GROUP BY t.Id, t.TableName
ORDER BY t.TableName

PRINT ''
SELECT COUNT(*) as TotalCANBOIndicators
FROM KpiIndicators i
INNER JOIN KpiAssignmentTables t ON i.TableId = t.Id
WHERE t.Category = 'CANBO'

PRINT '✅ Script completed successfully!'
