-- =============================================================
-- PHỤC HỒI 32 BẢNG KPI ASSIGNMENT TABLES (ADAPTED FOR NEW SCHEMA)
-- 23 bảng KPI cho cán bộ + 9 bảng KPI cho chi nhánh
-- Adapted for current schema with CreatedAt, UpdatedAt columns
-- =============================================================

USE TinhKhoanDB;
GO

PRINT '🚀 PHỤC HỒI 32 BẢNG KPI ASSIGNMENT TABLES';
PRINT '========================================';
PRINT '';

-- Clear existing template data
DELETE FROM KpiIndicators;
DELETE FROM KpiAssignmentTables;
DBCC CHECKIDENT ('KpiAssignmentTables', RESEED, 0);

-- =============================================================
-- PHẦN 1: 23 BẢNG KPI CHO CÁN BỘ
-- =============================================================
PRINT '📥 PHẦN 1: Tạo 23 bảng KPI cho cán bộ...';

-- 1. Bảng KPI cho Trưởng phòng Khách hàng Doanh nghiệp
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'TruongphongKhdn', N'Bảng KPI Trưởng phòng Khách hàng Doanh nghiệp', N'TRUONG_PHONG', GETDATE(), GETDATE());

-- 2. Bảng KPI cho Trưởng phòng Khách hàng Cá nhân
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'TruongphongKhcn', N'Bảng KPI Trưởng phòng Khách hàng Cá nhân', N'TRUONG_PHONG', GETDATE(), GETDATE());

-- 3. Bảng KPI cho Phó phòng Khách hàng Doanh nghiệp
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'PhophongKhdn', N'Bảng KPI Phó phòng Khách hàng Doanh nghiệp', N'PHO_TRUONG_PHONG', GETDATE(), GETDATE());

-- 4. Bảng KPI cho Phó phòng Khách hàng Cá nhân
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'PhophongKhcn', N'Bảng KPI Phó phòng Khách hàng Cá nhân', N'PHO_TRUONG_PHONG', GETDATE(), GETDATE());

-- 5. Bảng KPI cho Trưởng phòng Kế hoạch & Quản lý rủi ro
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'TruongphongKhqlrr', N'Bảng KPI Trưởng phòng Kế hoạch & Quản lý rủi ro', N'TRUONG_PHONG', GETDATE(), GETDATE());

-- 6. Bảng KPI cho Phó phòng Kế hoạch & Quản lý rủi ro
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'PhophongKhqlrr', N'Bảng KPI Phó phòng Kế hoạch & Quản lý rủi ro', N'PHO_TRUONG_PHONG', GETDATE(), GETDATE());

-- 7. Bảng KPI cho Trưởng phòng Kế toán & Ngân quỹ CNL1
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'TruongphongKtnqCnl1', N'Bảng KPI Trưởng phòng Kế toán & Ngân quỹ CNL1', N'TRUONG_PHONG', GETDATE(), GETDATE());

-- 8. Bảng KPI cho Phó phòng Kế toán & Ngân quỹ CNL1
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'PhophongKtnqCnl1', N'Bảng KPI Phó phòng Kế toán & Ngân quỹ CNL1', N'PHO_TRUONG_PHONG', GETDATE(), GETDATE());

-- 9. Bảng KPI cho Giao dịch viên
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'Gdv', N'Bảng KPI Giao dịch viên', N'KINH_DOANH', GETDATE(), GETDATE());

-- 10. Bảng KPI cho Cán bộ tín dụng
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'Cbtd', N'Bảng KPI Cán bộ tín dụng', N'TIN_DUNG', GETDATE(), GETDATE());

-- 11. Bảng KPI cho Trưởng phòng IT/Tổng hợp/Kiểm tra giám sát
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'TruongphongItThKtgs', N'Bảng KPI Trưởng phòng IT/TH/KTGS', N'TRUONG_PHONG', GETDATE(), GETDATE());

-- 12. Bảng KPI cho Cán bộ IT/Tổng hợp/Kiểm tra giám sát & Kế hoạch quản lý rủi ro
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'CbItThKtgsKhqlrr', N'Bảng KPI CB IT/TH/KTGS & KHQLRR', N'CONG_NGHE', GETDATE(), GETDATE());

-- 13. Bảng KPI cho Giám đốc PGD
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'GiamdocPgd', N'Bảng KPI Giám đốc Phòng giao dịch', N'TRUONG_PHONG', GETDATE(), GETDATE());

-- 14. Bảng KPI cho Phó Giám đốc PGD
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'PhogiamdocPgd', N'Bảng KPI Phó Giám đốc Phòng giao dịch', N'PHO_TRUONG_PHONG', GETDATE(), GETDATE());

-- 15. Bảng KPI cho Phó Giám đốc PGD Cán bộ tín dụng
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'PhogiamdocPgdCbtd', N'Bảng KPI Phó Giám đốc PGD Cán bộ tín dụng', N'TIN_DUNG', GETDATE(), GETDATE());

-- 16. Bảng KPI cho Trưởng phòng Khách hàng CNL2
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'TruongphongKhCnl2', N'Bảng KPI Trưởng phòng Khách hàng CNL2', N'TRUONG_PHONG', GETDATE(), GETDATE());

-- 17. Bảng KPI cho Phó phòng Khách hàng CNL2
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'PhophongKhCnl2', N'Bảng KPI Phó phòng Khách hàng CNL2', N'PHO_TRUONG_PHONG', GETDATE(), GETDATE());

-- 18. Bảng KPI cho Trưởng phòng Kế toán & Ngân quỹ CNL2
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'TruongphongKtnqCnl2', N'Bảng KPI Trưởng phòng Kế toán & Ngân quỹ CNL2', N'TRUONG_PHONG', GETDATE(), GETDATE());

-- 19. Bảng KPI cho Phó phòng Kế toán & Ngân quỹ CNL2
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'PhophongKtnqCnl2', N'Bảng KPI Phó phòng Kế toán & Ngân quỹ CNL2', N'PHO_TRUONG_PHONG', GETDATE(), GETDATE());

-- 20. Bảng KPI cho Cán bộ Khách hàng Doanh nghiệp
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'CbKhdn', N'Bảng KPI Cán bộ Khách hàng Doanh nghiệp', N'KINH_DOANH', GETDATE(), GETDATE());

-- 21. Bảng KPI cho Cán bộ Khách hàng Cá nhân
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'CbKhcn', N'Bảng KPI Cán bộ Khách hàng Cá nhân', N'KINH_DOANH', GETDATE(), GETDATE());

-- 22. Bảng KPI cho Cán bộ Kế toán & Ngân quỹ
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'CbKtnq', N'Bảng KPI Cán bộ Kế toán & Ngân quỹ', N'KE_TOAN', GETDATE(), GETDATE());

-- 23. Bảng KPI cho Nhân viên khác
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('EMPLOYEE', N'NhanvienKhac', N'Bảng KPI Nhân viên khác', N'KHAC', GETDATE(), GETDATE());

-- =============================================================
-- PHẦN 2: 9 BẢNG KPI CHO CHI NHÁNH/ĐƠN VỊ
-- =============================================================
PRINT '📥 PHẦN 2: Tạo 9 bảng KPI cho chi nhánh/đơn vị...';

-- 24. Chi nhánh cấp 1
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('UNIT', N'ChinhanhCap1', N'Bảng KPI Chi nhánh cấp 1', N'CHI_NHANH', GETDATE(), GETDATE());

-- 25. Chi nhánh cấp 2
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('UNIT', N'ChinhanhCap2', N'Bảng KPI Chi nhánh cấp 2', N'CHI_NHANH', GETDATE(), GETDATE());

-- 26. Phòng giao dịch cấp 1
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('UNIT', N'PgdCap1', N'Bảng KPI Phòng giao dịch cấp 1', N'PHONG_GIAO_DICH', GETDATE(), GETDATE());

-- 27. Phòng giao dịch cấp 2
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('UNIT', N'PgdCap2', N'Bảng KPI Phòng giao dịch cấp 2', N'PHONG_GIAO_DICH', GETDATE(), GETDATE());

-- 28. Phòng giao dịch cấp 3
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('UNIT', N'PgdCap3', N'Bảng KPI Phòng giao dịch cấp 3', N'PHONG_GIAO_DICH', GETDATE(), GETDATE());

-- 29. Phòng chức năng cấp 1
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('UNIT', N'PhongchucnangCap1', N'Bảng KPI Phòng chức năng cấp 1', N'PHONG_CHUC_NANG', GETDATE(), GETDATE());

-- 30. Phòng chức năng cấp 2
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('UNIT', N'PhongchucnangCap2', N'Bảng KPI Phòng chức năng cấp 2', N'PHONG_CHUC_NANG', GETDATE(), GETDATE());

-- 31. Trung tâm chuyên biệt
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('UNIT', N'TrungtamChuyenbiet', N'Bảng KPI Trung tâm chuyên biệt', N'TRUNG_TAM', GETDATE(), GETDATE());

-- 32. Bộ phận khác
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, CreatedAt, UpdatedAt)
VALUES ('UNIT', N'BophanKhac', N'Bảng KPI Bộ phận khác', N'BO_PHAN', GETDATE(), GETDATE());

-- =============================================================
-- PHẦN 3: TẠO CÁC CHỈ TIÊU KPI MẪU
-- =============================================================
PRINT '📊 PHẦN 3: Tạo các chỉ tiêu KPI mẫu...';

-- Sample indicators for key roles
DECLARE @TableId INT;

-- Indicators for Trưởng phòng KHDN
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'TruongphongKhdn';
INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive, CreatedAt, UpdatedAt) VALUES
(@TableId, 'KHDN_DT', 'Doanh thu KHDN', 'Tổng doanh thu từ khách hàng doanh nghiệp', 'VND', 1, GETDATE(), GETDATE()),
(@TableId, 'KHDN_KH_MOI', 'Khách hàng mới', 'Số lượng khách hàng doanh nghiệp mới', 'Khách hàng', 1, GETDATE(), GETDATE()),
(@TableId, 'KHDN_TD', 'Tín dụng KHDN', 'Dư nợ tín dụng khách hàng doanh nghiệp', 'VND', 1, GETDATE(), GETDATE()),
(@TableId, 'KHDN_HUY_DONG', 'Huy động vốn', 'Số dư huy động từ KHDN', 'VND', 1, GETDATE(), GETDATE());

-- Indicators for Trưởng phòng KHCN
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'TruongphongKhcn';
INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive, CreatedAt, UpdatedAt) VALUES
(@TableId, 'KHCN_DT', 'Doanh thu KHCN', 'Tổng doanh thu từ khách hàng cá nhân', 'VND', 1, GETDATE(), GETDATE()),
(@TableId, 'KHCN_THE', 'Thẻ phát hành', 'Số lượng thẻ phát hành mới', 'Thẻ', 1, GETDATE(), GETDATE()),
(@TableId, 'KHCN_VAY_VON', 'Vay vốn KHCN', 'Dư nợ vay vốn cá nhân', 'VND', 1, GETDATE(), GETDATE()),
(@TableId, 'KHCN_TIET_KIEM', 'Tiết kiệm', 'Số dư tiết kiệm cá nhân', 'VND', 1, GETDATE(), GETDATE());

-- Indicators for GDV
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'Gdv';
INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive, CreatedAt, UpdatedAt) VALUES
(@TableId, 'GDV_GIAO_DICH', 'Số giao dịch', 'Tổng số giao dịch thực hiện', 'Giao dịch', 1, GETDATE(), GETDATE()),
(@TableId, 'GDV_DICH_VU', 'Dịch vụ bán', 'Số lượng dịch vụ bán được', 'Dịch vụ', 1, GETDATE(), GETDATE()),
(@TableId, 'GDV_CHAT_LUONG', 'Chất lượng phục vụ', 'Điểm đánh giá chất lượng', 'Điểm', 1, GETDATE(), GETDATE());

-- Indicators for Chi nhánh
SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'ChinhanhCap1';
INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive, CreatedAt, UpdatedAt) VALUES
(@TableId, 'CN_DT_TONG', 'Doanh thu tổng', 'Tổng doanh thu chi nhánh', 'VND', 1, GETDATE(), GETDATE()),
(@TableId, 'CN_LOI_NHUAN', 'Lợi nhuận', 'Lợi nhuận trước thuế', 'VND', 1, GETDATE(), GETDATE()),
(@TableId, 'CN_TY_LE_NPL', 'Tỷ lệ nợ xấu', 'Tỷ lệ nợ xấu (%)', '%', 1, GETDATE(), GETDATE()),
(@TableId, 'CN_KHACH_HANG', 'Tổng khách hàng', 'Tổng số khách hàng', 'Khách hàng', 1, GETDATE(), GETDATE());

SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName = 'PgdCap1';
INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive, CreatedAt, UpdatedAt) VALUES
(@TableId, 'PGD_DT', 'Doanh thu PGD', 'Doanh thu phòng giao dịch', 'VND', 1, GETDATE(), GETDATE()),
(@TableId, 'PGD_KH_MOI', 'Khách hàng mới', 'Số khách hàng mới trong kỳ', 'Khách hàng', 1, GETDATE(), GETDATE()),
(@TableId, 'PGD_THI_PHAN', 'Thị phần khu vực', 'Thị phần tại khu vực (%)', '%', 1, GETDATE(), GETDATE());

-- =============================================================
-- VERIFICATION & SUMMARY
-- =============================================================
PRINT '';
PRINT '✅ HOÀN THÀNH PHỤC HỒI KPI SYSTEM!';
PRINT '================================';

-- Count tables by type
SELECT
    'Total KPI Assignment Tables' as [Component],
    COUNT(*) as [Count]
FROM KpiAssignmentTables;

SELECT
    'Employee Tables' as [Component],
    COUNT(*) as [Count]
FROM KpiAssignmentTables
WHERE TableType = 'EMPLOYEE';

SELECT
    'Unit Tables' as [Component],
    COUNT(*) as [Count]
FROM KpiAssignmentTables
WHERE TableType = 'UNIT';

-- Count indicators
SELECT
    'Total KPI Indicators' as [Component],
    COUNT(*) as [Count]
FROM KpiIndicators;

-- Show categories
SELECT DISTINCT Category, COUNT(*) as TableCount
FROM KpiAssignmentTables
GROUP BY Category
ORDER BY Category;

PRINT '';
PRINT '📋 32 bảng KPI đã được tạo thành công:';
PRINT '   - 23 bảng KPI cho cán bộ';
PRINT '   - 9 bảng KPI cho đơn vị';
PRINT '🎯 Các chỉ tiêu KPI mẫu đã được thiết lập';
PRINT '🔄 Hệ thống KPI sẵn sàng để sử dụng!';
