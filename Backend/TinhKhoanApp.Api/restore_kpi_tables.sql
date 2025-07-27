-- ================================================
-- Script phục hồi 32 bảng KPI Assignment từ KpiAssignmentTableSeeder.cs
-- 23 bảng cho cán bộ + 9 bảng cho chi nhánh
-- Dành cho bảng B2: Dropdown KPI tables
-- ================================================

USE TinhKhoanDB;
GO

-- Xóa toàn bộ dữ liệu hiện tại để tránh trung lặp
PRINT '🗑️ Xóa dữ liệu KPI tables hiện tại...';
DELETE FROM KpiIndicators WHERE KpiAssignmentTableId IN (SELECT Id FROM KpiAssignmentTables);
DELETE FROM KpiAssignmentTables;
GO

-- Reset IDENTITY seed để bắt đầu từ ID = 1
DBCC CHECKIDENT ('KpiAssignmentTables', RESEED, 0);
GO

PRINT '🚀 Bắt đầu phục hồi 32 bảng KPI Assignment...';

-- Thêm 23 bảng KPI cho cán bộ (TableType 1-23)
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, IsActive, CreatedDate) VALUES
-- Vai trò cán bộ (23 bảng) - Category = 'CANBO' để match với frontend
(1, 'TruongphongKhdn_KPI_Assignment', N'Bảng KPI cho Trưởng phòng KHDN', N'CANBO', 1, GETUTCDATE()),
(2, 'TruongphongKhcn_KPI_Assignment', N'Bảng KPI cho Trưởng phòng KHCN', N'CANBO', 1, GETUTCDATE()),
(3, 'PhophongKhdn_KPI_Assignment', N'Bảng KPI cho Phó phòng KHDN', N'CANBO', 1, GETUTCDATE()),
(4, 'PhophongKhcn_KPI_Assignment', N'Bảng KPI cho Phó phòng KHCN', N'CANBO', 1, GETUTCDATE()),
(5, 'TruongphongKhqlrr_KPI_Assignment', N'Bảng KPI cho Trưởng phòng Kế hoạch và Quản lý rủi ro', N'CANBO', 1, GETUTCDATE()),
(6, 'PhophongKhqlrr_KPI_Assignment', N'Bảng KPI cho Phó phòng Kế hoạch và Quản lý rủi ro', N'CANBO', 1, GETUTCDATE()),
(7, 'Cbtd_KPI_Assignment', N'Bảng KPI cho Cán bộ tín dụng', N'CANBO', 1, GETUTCDATE()),
(8, 'TruongphongKtnqCnl1_KPI_Assignment', N'Bảng KPI cho Trưởng phòng Kế toán & Ngân quỹ CNL1', N'CANBO', 1, GETUTCDATE()),
(9, 'PhophongKtnqCnl1_KPI_Assignment', N'Bảng KPI cho Phó phòng Kế toán & Ngân quỹ CNL1', N'CANBO', 1, GETUTCDATE()),
(10, 'Gdv_KPI_Assignment', N'Bảng KPI cho Giao dịch viên', N'CANBO', 1, GETUTCDATE()),
(11, 'TqHkKtnb_KPI_Assignment', N'Bảng KPI cho TQ/Hậu kiểm/Kế toán nội bộ', N'CANBO', 1, GETUTCDATE()),
(12, 'TruongphongItThKtgs_KPI_Assignment', N'Bảng KPI cho Trưởng phó phòng IT/TH/KTGS', N'CANBO', 1, GETUTCDATE()),
(13, 'CbItThKtgsKhqlrr_KPI_Assignment', N'Bảng KPI cho CB IT/TH/KTGS/KHQLRR', N'CANBO', 1, GETUTCDATE()),
(14, 'GiamdocPgd_KPI_Assignment', N'Bảng KPI cho Giám đốc PGD', N'CANBO', 1, GETUTCDATE()),
(15, 'PhogiamdocPgd_KPI_Assignment', N'Bảng KPI cho Phó giám đốc PGD', N'CANBO', 1, GETUTCDATE()),
(16, 'PhogiamdocPgdCbtd_KPI_Assignment', N'Bảng KPI cho Phó giám đốc PGD kiêm CBTD', N'CANBO', 1, GETUTCDATE()),
(17, 'GiamdocCnl2_KPI_Assignment', N'Bảng KPI cho Giám đốc CNL2', N'CANBO', 1, GETUTCDATE()),
(18, 'PhogiamdocCnl2Td_KPI_Assignment', N'Bảng KPI cho Phó giám đốc CNL2 phụ trách Tín dụng', N'CANBO', 1, GETUTCDATE()),
(19, 'PhogiamdocCnl2Kt_KPI_Assignment', N'Bảng KPI cho Phó giám đốc CNL2 Phụ trách Kế toán', N'CANBO', 1, GETUTCDATE()),
(20, 'TruongphongKhCnl2_KPI_Assignment', N'Bảng KPI cho Trưởng phòng KH CNL2', N'CANBO', 1, GETUTCDATE()),
(21, 'PhophongKhCnl2_KPI_Assignment', N'Bảng KPI cho Phó phòng KH CNL2', N'CANBO', 1, GETUTCDATE()),
(22, 'TruongphongKtnqCnl2_KPI_Assignment', N'Bảng KPI cho Trưởng phòng Kế toán & Ngân quỹ CNL2', N'CANBO', 1, GETUTCDATE()),
(23, 'PhophongKtnqCnl2_KPI_Assignment', N'Bảng KPI cho Phó phòng Kế toán & Ngân quỹ CNL2', N'CANBO', 1, GETUTCDATE()),

-- Chi nhánh (9 bảng) với TableType 200-208 - Category = 'CHINHANH' để match với frontend
-- Sắp xếp theo thứ tự từ Hội Sở -> Nậm Hàng
(200, 'HoiSo_KPI_Assignment', N'Bảng KPI cho Hội sở', N'CHINHANH', 1, GETUTCDATE()),
(201, 'CnBinhLu_KPI_Assignment', N'Bảng KPI cho Chi nhánh Bình Lư', N'CHINHANH', 1, GETUTCDATE()),
(202, 'CnPhongTho_KPI_Assignment', N'Bảng KPI cho Chi nhánh Phong Thổ', N'CHINHANH', 1, GETUTCDATE()),
(203, 'CnSinHo_KPI_Assignment', N'Bảng KPI cho Chi nhánh Sin Hồ', N'CHINHANH', 1, GETUTCDATE()),
(204, 'CnBumTo_KPI_Assignment', N'Bảng KPI cho Chi nhánh Bum Tở', N'CHINHANH', 1, GETUTCDATE()),
(205, 'CnThanUyen_KPI_Assignment', N'Bảng KPI cho Chi nhánh Than Uyên', N'CHINHANH', 1, GETUTCDATE()),
(206, 'CnDoanKet_KPI_Assignment', N'Bảng KPI cho Chi nhánh Đoàn Kết', N'CHINHANH', 1, GETUTCDATE()),
(207, 'CnTanUyen_KPI_Assignment', N'Bảng KPI cho Chi nhánh Tân Uyên', N'CHINHANH', 1, GETUTCDATE()),
(208, 'CnNamHang_KPI_Assignment', N'Bảng KPI cho Chi nhánh Nậm Hàng', N'CHINHANH', 1, GETUTCDATE());-- Kiểm tra kết quả
PRINT '📊 Kết quả phục hồi:';
SELECT
    'Total KPI Tables' as Category,
    COUNT(*) as Count
FROM KpiAssignmentTables;

PRINT '✅ Hoàn thành phục hồi 32 bảng KPI Assignment!';

-- Hiển thị một số bảng mẫu để xác minh
PRINT '🔍 Mẫu các bảng KPI đã được tạo:';
SELECT TOP 5
    Id,
    TableType,
    TableName,
    Description,
    Category
FROM KpiAssignmentTables
ORDER BY TableType;

PRINT '📋 Tổng số bảng KPI theo từng category:';
SELECT
    Category,
    COUNT(*) as Count
FROM KpiAssignmentTables
GROUP BY Category
ORDER BY Count DESC;

PRINT '📋 Chi tiết theo loại (phân tab chính xác):';
SELECT
    CASE
        WHEN Category = 'CANBO' THEN 'Tab Dành cho cán bộ'
        WHEN Category = 'CHINHANH' THEN 'Tab Dành cho chi nhánh'
        ELSE 'Khác'
    END as TabCategory,
    COUNT(*) as Count
FROM KpiAssignmentTables
GROUP BY Category
ORDER BY Count DESC;
