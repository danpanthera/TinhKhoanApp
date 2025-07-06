-- =============================================================
-- PHỤC HỒI 32 BẢNG KPI ASSIGNMENT TABLES
-- 23 bảng KPI cho cán bộ + 9 bảng KPI cho chi nhánh  
-- Ngày: 06/07/2025
-- =============================================================

USE TinhKhoanDB;
GO

PRINT '🚀 PHỤC HỒI 32 BẢNG KPI ASSIGNMENT TABLES';
PRINT '========================================';
PRINT '';

-- Xóa dữ liệu cũ
DELETE FROM KpiAssignmentTables;
DBCC CHECKIDENT ('KpiAssignmentTables', RESEED, 0);

-- =============================================================
-- PHẦN 1: 23 BẢNG KPI CHO CÁN BỘ
-- =============================================================
PRINT '📥 PHẦN 1: Tạo 23 bảng KPI cho cán bộ...';

SET IDENTITY_INSERT KpiAssignmentTables ON;

-- 1. Bảng KPI cho Trưởng phòng Khách hàng Doanh nghiệp
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate) 
VALUES (1, 1, N'TruongphongKhdn', N'Bảng KPI Trưởng phòng Khách hàng Doanh nghiệp', N'CANBO', 1, GETDATE());

-- 2. Bảng KPI cho Trưởng phòng Khách hàng Cá nhân  
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (2, 1, N'TruongphongKhcn', N'Bảng KPI Trưởng phòng Khách hàng Cá nhân', N'CANBO', 1, GETDATE());

-- 3. Bảng KPI cho Phó phòng Khách hàng Doanh nghiệp
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (3, 1, N'PhophongKhdn', N'Bảng KPI Phó phòng Khách hàng Doanh nghiệp', N'CANBO', 1, GETDATE());

-- 4. Bảng KPI cho Phó phòng Khách hàng Cá nhân
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (4, 1, N'PhophongKhcn', N'Bảng KPI Phó phòng Khách hàng Cá nhân', N'CANBO', 1, GETDATE());

-- 5. Bảng KPI cho Trưởng phòng Kế hoạch & Quản lý rủi ro
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (5, 1, N'TruongphongKhqlrr', N'Bảng KPI Trưởng phòng Kế hoạch & Quản lý rủi ro', N'CANBO', 1, GETDATE());

-- 6. Bảng KPI cho Phó phòng Kế hoạch & Quản lý rủi ro  
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (6, 1, N'PhophongKhqlrr', N'Bảng KPI Phó phòng Kế hoạch & Quản lý rủi ro', N'CANBO', 1, GETDATE());

-- 7. Bảng KPI cho Trưởng phòng Kế toán & Ngân quỹ CNL1
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (7, 1, N'TruongphongKtnqCnl1', N'Bảng KPI Trưởng phòng Kế toán & Ngân quỹ CNL1', N'CANBO', 1, GETDATE());

-- 8. Bảng KPI cho Phó phòng Kế toán & Ngân quỹ CNL1
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (8, 1, N'PhophongKtnqCnl1', N'Bảng KPI Phó phòng Kế toán & Ngân quỹ CNL1', N'CANBO', 1, GETDATE());

-- 9. Bảng KPI cho Giao dịch viên
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (9, 1, N'Gdv', N'Bảng KPI Giao dịch viên', N'CANBO', 1, GETDATE());

-- 10. Bảng KPI cho Cán bộ tín dụng
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (10, 1, N'Cbtd', N'Bảng KPI Cán bộ tín dụng', N'CANBO', 1, GETDATE());

-- 11. Bảng KPI cho Trưởng phòng IT/Tổng hợp/Kiểm tra giám sát
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (11, 1, N'TruongphongItThKtgs', N'Bảng KPI Trưởng phòng IT/TH/KTGS', N'CANBO', 1, GETDATE());

-- 12. Bảng KPI cho Cán bộ IT/Tổng hợp/Kiểm tra giám sát & Kế hoạch quản lý rủi ro
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (12, 1, N'CbItThKtgsKhqlrr', N'Bảng KPI CB IT/TH/KTGS & KHQLRR', N'CANBO', 1, GETDATE());

-- 13. Bảng KPI cho Giám đốc PGD
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (13, 1, N'GiamdocPgd', N'Bảng KPI Giám đốc Phòng giao dịch', N'CANBO', 1, GETDATE());

-- 14. Bảng KPI cho Phó Giám đốc PGD
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (14, 1, N'PhogiamdocPgd', N'Bảng KPI Phó Giám đốc Phòng giao dịch', N'CANBO', 1, GETDATE());

-- 15. Bảng KPI cho Phó Giám đốc PGD Cán bộ tín dụng
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (15, 1, N'PhogiamdocPgdCbtd', N'Bảng KPI Phó Giám đốc PGD Cán bộ tín dụng', N'CANBO', 1, GETDATE());

-- 16. Bảng KPI cho Trưởng phòng Khách hàng CNL2
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (16, 1, N'TruongphongKhCnl2', N'Bảng KPI Trưởng phòng Khách hàng CNL2', N'CANBO', 1, GETDATE());

-- 17. Bảng KPI cho Phó phòng Khách hàng CNL2
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (17, 1, N'PhophongKhCnl2', N'Bảng KPI Phó phòng Khách hàng CNL2', N'CANBO', 1, GETDATE());

-- 18. Bảng KPI cho Trưởng phòng Kế toán & Ngân quỹ CNL2  
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (18, 1, N'TruongphongKtnqCnl2', N'Bảng KPI Trưởng phòng Kế toán & Ngân quỹ CNL2', N'CANBO', 1, GETDATE());

-- 19. Bảng KPI cho Phó phòng Kế toán & Ngân quỹ CNL2
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (19, 1, N'PhophongKtnqCnl2', N'Bảng KPI Phó phòng Kế toán & Ngân quỹ CNL2', N'CANBO', 1, GETDATE());

-- 20. Bảng KPI cho Phó Giám đốc CNL2 Tín dụng
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (20, 1, N'PhogiamdocCnl2Td', N'Bảng KPI Phó Giám đốc CNL2 Tín dụng', N'CANBO', 1, GETDATE());

-- 21. Bảng KPI cho Phó Giám đốc CNL2 Kế toán
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (21, 1, N'PhogiamdocCnl2Kt', N'Bảng KPI Phó Giám đốc CNL2 Kế toán', N'CANBO', 1, GETDATE());

-- 22. Bảng KPI cho Giám đốc CNL2 (Chi nhánh cấp 2)
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (22, 1, N'GiamdocCnl2', N'Bảng KPI Giám đốc Chi nhánh cấp 2', N'CANBO', 1, GETDATE());

-- 23. Bảng KPI cho Cán bộ nghiệp vụ khác
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (23, 1, N'CanBoNghiepVuKhac', N'Bảng KPI Cán bộ nghiệp vụ khác', N'CANBO', 1, GETDATE());

-- =============================================================  
-- PHẦN 2: 9 BẢNG KPI CHO CHI NHÁNH
-- =============================================================
PRINT '📥 PHẦN 2: Tạo 9 bảng KPI cho chi nhánh...';

-- 24. Bảng KPI Chi nhánh Bình Lư
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (24, 2, N'KPI_CnBinhLu', N'Bảng KPI Chi nhánh Bình Lư', N'CHINHANH', 1, GETDATE());

-- 25. Bảng KPI Chi nhánh Phong Thổ  
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (25, 2, N'KPI_CnPhongTho', N'Bảng KPI Chi nhánh Phong Thổ', N'CHINHANH', 1, GETDATE());

-- 26. Bảng KPI Chi nhánh Sìn Hồ
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (26, 2, N'KPI_CnSinHo', N'Bảng KPI Chi nhánh Sìn Hồ', N'CHINHANH', 1, GETDATE());

-- 27. Bảng KPI Chi nhánh Bum Tở
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (27, 2, N'KPI_CnBumTo', N'Bảng KPI Chi nhánh Bum Tở', N'CHINHANH', 1, GETDATE());

-- 28. Bảng KPI Chi nhánh Than Uyên
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (28, 2, N'KPI_CnThanUyen', N'Bảng KPI Chi nhánh Than Uyên', N'CHINHANH', 1, GETDATE());

-- 29. Bảng KPI Chi nhánh Đoàn Kết
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (29, 2, N'KPI_CnDoanKet', N'Bảng KPI Chi nhánh Đoàn Kết', N'CHINHANH', 1, GETDATE());

-- 30. Bảng KPI Chi nhánh Tân Uyên
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (30, 2, N'KPI_CnTanUyen', N'Bảng KPI Chi nhánh Tân Uyên', N'CHINHANH', 1, GETDATE());

-- 31. Bảng KPI Chi nhánh Nậm Hàng
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (31, 2, N'KPI_CnNamHang', N'Bảng KPI Chi nhánh Nậm Hàng', N'CHINHANH', 1, GETDATE());

-- 32. Bảng KPI Hội Sở 
INSERT INTO KpiAssignmentTables (Id, TableType, TableName, Description, Category, IsActive, CreatedDate)
VALUES (32, 2, N'KPI_HoiSo', N'Bảng KPI Hội Sở', N'CHINHANH', 1, GETDATE());

SET IDENTITY_INSERT KpiAssignmentTables OFF;

-- =============================================================
-- KIỂM TRA KẾT QUẢ PHỤC HỒI
-- =============================================================
PRINT '';
PRINT '✅ HOÀN THÀNH PHỤC HỒI 32 BẢNG KPI ASSIGNMENT TABLES';
PRINT '==================================================';

-- Thống kê tổng quan
SELECT COUNT(*) as 'Tong_so_bang_KPI' FROM KpiAssignmentTables;

-- Thống kê theo category
SELECT 
    Category as 'Loai_bang_KPI',
    COUNT(*) as 'So_luong'
FROM KpiAssignmentTables 
GROUP BY Category;

-- Thống kê theo table type  
SELECT 
    CASE TableType 
        WHEN 1 THEN 'Cán bộ'
        WHEN 2 THEN 'Chi nhánh'
        ELSE 'Khác'
    END as 'Loai_table',
    COUNT(*) as 'So_luong'
FROM KpiAssignmentTables
GROUP BY TableType;

PRINT '';
PRINT '📊 CHI TIẾT 32 BẢNG KPI:';
SELECT 
    Id,
    TableName as 'Ten_bang',
    Description as 'Mo_ta', 
    Category as 'Loai'
FROM KpiAssignmentTables 
ORDER BY Id;

PRINT '';
PRINT '🎯 KẾT QUẢ: Đã phục hồi thành công 23 bảng KPI cho cán bộ + 9 bảng KPI cho chi nhánh = 32 bảng tổng cộng';
