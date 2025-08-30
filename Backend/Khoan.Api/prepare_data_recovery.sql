-- ==================================================
-- SCRIPT CHUẨN BỊ PHỤC HỒI DỮ LIỆU TỪ DATABASE CŨ
-- TINHKHOAN APP - TỪNG BƯỚC AN TOÀN
-- ==================================================

USE TinhKhoanDB;
GO

PRINT '========================================================';
PRINT '📋 HƯỚNG DẪN PHỤC HỒI DỮ LIỆU TỪ DATABASE CŨ';
PRINT '========================================================';
PRINT '';

-- ==================================================
-- BƯỚC 1: BACKUP DATABASE CŨ
-- ==================================================
PRINT '🔹 BƯỚC 1: Backup database cũ (SQL Server)';
PRINT '   Lệnh: sqlcmd -S old_server -Q "BACKUP DATABASE TinhKhoanDB TO DISK = ''C:\backup\TinhKhoanDB_backup.bak''"';
PRINT '';

-- ==================================================
-- BƯỚC 2: DANH SÁCH CÁC BẢNG CẦN PHỤC HỒI
-- ==================================================
PRINT '🔹 BƯỚC 2: Danh sách bảng cần phục hồi (THEO THỨ TỰ ƯU TIÊN):';
PRINT '';

-- Hiển thị danh sách bảng theo thứ tự dependency
SELECT
    ROW_NUMBER() OVER (ORDER BY
        CASE TABLE_NAME
            WHEN 'Units' THEN 1
            WHEN 'Positions' THEN 2
            WHEN 'Roles' THEN 3
            WHEN 'Employees' THEN 4
            WHEN 'EmployeeRoles' THEN 5
            WHEN 'KPIDefinitions' THEN 6
            WHEN 'KpiIndicators' THEN 7
            WHEN 'KpiScoringRules' THEN 8
            WHEN 'BusinessPlanTargets' THEN 9
            WHEN 'KhoanPeriods' THEN 10
            ELSE 99
        END
    ) as Priority,
    TABLE_NAME as [Tên Bảng],
    CASE TABLE_NAME
        WHEN 'Units' THEN 'Đơn vị/Phòng ban'
        WHEN 'Positions' THEN 'Chức vụ'
        WHEN 'Roles' THEN 'Vai trò hệ thống'
        WHEN 'Employees' THEN 'Nhân viên'
        WHEN 'EmployeeRoles' THEN 'Phân quyền nhân viên'
        WHEN 'KPIDefinitions' THEN 'Định nghĩa KPI'
        WHEN 'KpiIndicators' THEN 'Chỉ tiêu KPI'
        WHEN 'KpiScoringRules' THEN 'Quy tắc tính điểm KPI'
        WHEN 'BusinessPlanTargets' THEN 'Mục tiêu kế hoạch kinh doanh'
        WHEN 'KhoanPeriods' THEN 'Kỳ khoán'
        ELSE 'Dữ liệu nghiệp vụ khác'
    END as [Mô Tả],
    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = t.TABLE_NAME) as [Số Cột]
FROM INFORMATION_SCHEMA.TABLES t
WHERE TABLE_TYPE = 'BASE TABLE'
    AND TABLE_NAME IN (
        'Units', 'Positions', 'Roles', 'Employees', 'EmployeeRoles',
        'KPIDefinitions', 'KpiIndicators', 'KpiScoringRules',
        'BusinessPlanTargets', 'KhoanPeriods'
    )
ORDER BY Priority;

PRINT '';

-- ==================================================
-- BƯỚC 3: SCRIPT TEMPLATE EXPORT DATA
-- ==================================================
PRINT '🔹 BƯỚC 3: Template script export dữ liệu:';
PRINT '';
PRINT '-- Export Units';
PRINT 'bcp "SELECT * FROM TinhKhoanDB.dbo.Units" queryout "Units.csv" -c -t"," -S old_server -T';
PRINT '';
PRINT '-- Export Positions';
PRINT 'bcp "SELECT * FROM TinhKhoanDB.dbo.Positions" queryout "Positions.csv" -c -t"," -S old_server -T';
PRINT '';
PRINT '-- Export Employees (trừ admin đã tạo)';
PRINT 'bcp "SELECT * FROM TinhKhoanDB.dbo.Employees WHERE Username != ''admin''" queryout "Employees.csv" -c -t"," -S old_server -T';
PRINT '';

-- ==================================================
-- BƯỚC 4: KIỂM TRA DỮ LIỆU HIỆN TẠI
-- ==================================================
PRINT '🔹 BƯỚC 4: Dữ liệu hiện tại trong Azure SQL Edge:';
PRINT '';

SELECT
    t.TABLE_NAME as [Tên Bảng],
    CASE t.TABLE_NAME
        WHEN 'Units' THEN (SELECT COUNT(*) FROM Units)
        WHEN 'Positions' THEN (SELECT COUNT(*) FROM Positions)
        WHEN 'Employees' THEN (SELECT COUNT(*) FROM Employees)
        WHEN 'KPIDefinitions' THEN (SELECT COUNT(*) FROM KPIDefinitions)
        WHEN 'KpiIndicators' THEN (SELECT COUNT(*) FROM KpiIndicators)
        WHEN 'BusinessPlanTargets' THEN (SELECT COUNT(*) FROM BusinessPlanTargets)
        ELSE 0
    END as [Số Record],
    CASE
        WHEN t.TABLE_NAME = 'Units' AND (SELECT COUNT(*) FROM Units) > 1 THEN 'Có dữ liệu'
        WHEN t.TABLE_NAME = 'Positions' AND (SELECT COUNT(*) FROM Positions) > 1 THEN 'Có dữ liệu'
        WHEN t.TABLE_NAME = 'Employees' AND (SELECT COUNT(*) FROM Employees) > 1 THEN 'Có dữ liệu'
        WHEN t.TABLE_NAME IN ('Units', 'Positions', 'Employees') THEN 'Chỉ có admin temp'
        ELSE 'Trống - cần import'
    END as [Trạng Thái]
FROM INFORMATION_SCHEMA.TABLES t
WHERE TABLE_TYPE = 'BASE TABLE'
    AND TABLE_NAME IN ('Units', 'Positions', 'Employees', 'KPIDefinitions', 'KpiIndicators', 'BusinessPlanTargets')
ORDER BY TABLE_NAME;

PRINT '';
PRINT '========================================================';
PRINT '⚠️  LƯU Ý QUAN TRỌNG:';
PRINT '========================================================';
PRINT '1. 🚫 KHÔNG xóa user admin/admin123 đã tạo';
PRINT '2. 🔄 Phục hồi dữ liệu theo thứ tự dependency';
PRINT '3. 🔍 Kiểm tra foreign key constraints trước khi import';
PRINT '4. 💾 Backup Azure SQL Edge trước khi import lớn';
PRINT '5. ✅ Test từng bảng nhỏ trước khi import toàn bộ';
PRINT '';
PRINT '🎯 MỤC TIÊU: Có đầy đủ dữ liệu thực để:';
PRINT '   - Đăng nhập hệ thống';
PRINT '   - Xem danh sách đơn vị, nhân viên';
PRINT '   - Hiển thị 23 bảng KPI';
PRINT '   - Thực hiện import/export dữ liệu';
PRINT '========================================================';

GO
