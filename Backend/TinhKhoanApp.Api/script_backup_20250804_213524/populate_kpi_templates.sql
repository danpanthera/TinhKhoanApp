-- Populate KPI Templates for TinhKhoanApp
-- Nhập dữ liệu mẫu KPI: 23 bảng cán bộ + 9 bảng đơn vị

-- Clear existing data first (optional)
-- DELETE FROM KpiIndicators;
-- DELETE FROM KpiAssignmentTables;

-- 1. EMPLOYEE KPI TABLES (23 bảng cho cán bộ)
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category) VALUES
('EMPLOYEE', 'CB_TD01', 'Thủ trưởng Ngân hàng', 'THU_TRUONG'),
('EMPLOYEE', 'CB_TD02', 'Phó Thủ trưởng Ngân hàng', 'THU_TRUONG'),
('EMPLOYEE', 'CB_TD03', 'Trưởng phòng/Giám đốc trung tâm', 'TRUONG_PHONG'),
('EMPLOYEE', 'CB_TD04', 'Phó Trưởng phòng/Phó Giám đốc trung tâm', 'PHO_TRUONG_PHONG'),
('EMPLOYEE', 'CB_KD01', 'Chuyên viên Khách hàng doanh nghiệp', 'KINH_DOANH'),
('EMPLOYEE', 'CB_KD02', 'Chuyên viên Khách hàng cá nhân', 'KINH_DOANH'),
('EMPLOYEE', 'CB_KD03', 'Giao dịch viên Chi nhánh', 'KINH_DOANH'),
('EMPLOYEE', 'CB_KD04', 'Giao dịch viên Phòng giao dịch', 'KINH_DOANH'),
('EMPLOYEE', 'CB_TD05', 'Trưởng Phòng giao dịch', 'TRUONG_PHONG'),
('EMPLOYEE', 'CB_TD06', 'Phó Trưởng Phòng giao dịch', 'PHO_TRUONG_PHONG'),
('EMPLOYEE', 'CB_KT01', 'Chuyên viên Kế toán', 'KE_TOAN'),
('EMPLOYEE', 'CB_KT02', 'Trưởng bộ phận Kế toán', 'KE_TOAN'),
('EMPLOYEE', 'CB_KSNB01', 'Chuyên viên Kiểm soát nội bộ', 'KIEM_SOAT'),
('EMPLOYEE', 'CB_KSNB02', 'Trưởng bộ phận KSNB', 'KIEM_SOAT'),
('EMPLOYEE', 'CB_QTST01', 'Chuyên viên Quản trị sự thuận', 'QUAN_TRI'),
('EMPLOYEE', 'CB_QTST02', 'Trưởng bộ phận QTST', 'QUAN_TRI'),
('EMPLOYEE', 'CB_IT01', 'Chuyên viên Công nghệ thông tin', 'CONG_NGHE'),
('EMPLOYEE', 'CB_IT02', 'Trưởng bộ phận CNTT', 'CONG_NGHE'),
('EMPLOYEE', 'CB_TDNH01', 'Chuyên viên Tín dụng ngân hàng', 'TIN_DUNG'),
('EMPLOYEE', 'CB_TDNH02', 'Trưởng bộ phận Tín dụng', 'TIN_DUNG'),
('EMPLOYEE', 'CB_KHDN01', 'Chuyên viên Khách hàng doanh nghiệp cấp cao', 'KINH_DOANH'),
('EMPLOYEE', 'CB_KHCN01', 'Chuyên viên Khách hàng cá nhân cấp cao', 'KINH_DOANH'),
('EMPLOYEE', 'CB_BV01', 'Nhân viên Bảo vệ an ninh', 'BAO_VE');

-- 2. UNIT KPI TABLES (9 bảng cho đơn vị)
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category) VALUES
('UNIT', 'DV_CN01', 'Chi nhánh cấp I', 'CHI_NHANH'),
('UNIT', 'DV_CN02', 'Chi nhánh cấp II', 'CHI_NHANH'),
('UNIT', 'DV_PGD01', 'Phòng giao dịch cấp I', 'PHONG_GIAO_DICH'),
('UNIT', 'DV_PGD02', 'Phòng giao dịch cấp II', 'PHONG_GIAO_DICH'),
('UNIT', 'DV_TT01', 'Trung tâm chuyên biệt', 'TRUNG_TAM'),
('UNIT', 'DV_PH01', 'Phòng chức năng cấp I', 'PHONG_CHUC_NANG'),
('UNIT', 'DV_PH02', 'Phòng chức năng cấp II', 'PHONG_CHUC_NANG'),
('UNIT', 'DV_BP01', 'Bộ phận trực thuộc', 'BO_PHAN'),
('UNIT', 'DV_KV01', 'Khu vực kinh doanh', 'KHU_VUC');

-- 3. Populate KPI Indicators for Employee Tables
-- Example indicators for employee KPIs
DECLARE @TableId INT;

-- Indicators for Thủ trưởng Ngân hàng (CB_TD01)
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'CB_TD01';
INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive) VALUES
(@TableId, 'TD01_DT', 'Doanh thu tổng', 'Tổng doanh thu của ngân hàng', 'VND', 1),
(@TableId, 'TD01_LN', 'Lợi nhuận', 'Lợi nhuận trước thuế', 'VND', 1),
(@TableId, 'TD01_NPL', 'Tỷ lệ nợ xấu', 'Tỷ lệ nợ xấu toàn hàng', '%', 1),
(@TableId, 'TD01_ROA', 'ROA', 'Tỷ suất sinh lời trên tài sản', '%', 1),
(@TableId, 'TD01_KH', 'Khách hàng mới', 'Số lượng khách hàng mới', 'Khách hàng', 1);

-- Indicators for Phó Thủ trưởng (CB_TD02)
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'CB_TD02';
INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive) VALUES
(@TableId, 'TD02_HT', 'Hỗ trợ lãnh đạo', 'Điểm đánh giá hỗ trợ thủ trưởng', 'Điểm', 1),
(@TableId, 'TD02_QL', 'Quản lý bộ phận', 'Hiệu quả quản lý bộ phận phụ trách', 'Điểm', 1),
(@TableId, 'TD02_KH', 'Khách hàng VIP', 'Quản lý khách hàng quan trọng', 'Khách hàng', 1),
(@TableId, 'TD02_DT', 'Doanh thu', 'Doanh thu bộ phận phụ trách', 'VND', 1);

-- Indicators for Trưởng phòng (CB_TD03)
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'CB_TD03';
INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive) VALUES
(@TableId, 'TD03_DT', 'Doanh thu phòng', 'Doanh thu của phòng/trung tâm', 'VND', 1),
(@TableId, 'TD03_CP', 'Chi phí hoạt động', 'Kiểm soát chi phí hoạt động', 'VND', 1),
(@TableId, 'TD03_NV', 'Quản lý nhân viên', 'Hiệu quả quản lý nhân viên', 'Điểm', 1),
(@TableId, 'TD03_SP', 'Sản phẩm mới', 'Triển khai sản phẩm mới', 'Sản phẩm', 1);

-- Indicators for Chuyên viên KD KHDN (CB_KD01)
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'CB_KD01';
INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive) VALUES
(@TableId, 'KD01_DT', 'Doanh thu', 'Doanh thu từ khách hàng DN', 'VND', 1),
(@TableId, 'KD01_KH', 'Khách hàng mới', 'Số KH doanh nghiệp mới', 'Khách hàng', 1),
(@TableId, 'KD01_TD', 'Tín dụng', 'Dư nợ tín dụng KHDN', 'VND', 1),
(@TableId, 'KD01_TK', 'Tiền gửi', 'Số dư tiền gửi KHDN', 'VND', 1);

-- Indicators for Chuyên viên KD KHCN (CB_KD02)
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'CB_KD02';
INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive) VALUES
(@TableId, 'KD02_DT', 'Doanh thu', 'Doanh thu từ khách hàng CN', 'VND', 1),
(@TableId, 'KD02_KH', 'Khách hàng mới', 'Số KH cá nhân mới', 'Khách hàng', 1),
(@TableId, 'KD02_THE', 'Thẻ phát hành', 'Số thẻ phát hành mới', 'Thẻ', 1),
(@TableId, 'KD02_VV', 'Vay vốn', 'Dư nợ vay vốn KHCN', 'VND', 1);

-- 4. Populate KPI Indicators for Unit Tables
-- Indicators for Chi nhánh cấp I (DV_CN01)
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'DV_CN01';
INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive) VALUES
(@TableId, 'CN01_DT', 'Doanh thu chi nhánh', 'Tổng doanh thu chi nhánh', 'VND', 1),
(@TableId, 'CN01_LN', 'Lợi nhuận', 'Lợi nhuận chi nhánh', 'VND', 1),
(@TableId, 'CN01_KH', 'Khách hàng', 'Tổng số khách hàng', 'Khách hàng', 1),
(@TableId, 'CN01_TD', 'Tín dụng', 'Dư nợ tín dụng', 'VND', 1),
(@TableId, 'CN01_HĐ', 'Huy động', 'Số dư huy động vốn', 'VND', 1);

-- Indicators for Phòng giao dịch (DV_PGD01)
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'DV_PGD01';
INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive) VALUES
(@TableId, 'PGD01_DT', 'Doanh thu', 'Doanh thu phòng giao dịch', 'VND', 1),
(@TableId, 'PGD01_KH', 'Khách hàng mới', 'Số khách hàng mới', 'Khách hàng', 1),
(@TableId, 'PGD01_GD', 'Giao dịch', 'Số lượng giao dịch', 'Giao dịch', 1),
(@TableId, 'PGD01_DV', 'Dịch vụ', 'Chất lượng dịch vụ', 'Điểm', 1);

PRINT '✅ KPI Templates populated successfully!';
PRINT '📊 Created 32 KPI assignment tables (23 Employee + 9 Unit)';
PRINT '🎯 Sample indicators populated for key roles';
PRINT '🔄 Ready for full KPI data restoration';
