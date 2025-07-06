-- Script thiết lập đồng bộ dữ liệu KPI từ Cấu hình sang Giao khoán
-- Ngày tạo: 06/07/2025
-- Mục đích: Đồng bộ KpiAssignmentTables → EmployeeKpiAssignments + UnitKpiScorings

USE TinhKhoanDB;
GO

PRINT N'🎯 BẮT ĐẦU THIẾT LẬP ĐỒNG BỘ DỮ LIỆU KPI';
PRINT N'================================================';

-- 1. ĐỒN BỘ DỮ LIỆU CHO "GIAO KHOÁN KPI CHO CÁN BỘ"
PRINT N'';
PRINT N'📋 1. ĐỒNG BỘ GIAO KHOÁN KPI CHO CÁN BỘ (23 bảng):';

-- Xóa dữ liệu cũ nếu có
DELETE FROM EmployeeKpiAssignments;
PRINT N'   🗑️ Đã xóa dữ liệu cũ EmployeeKpiAssignments';

-- Thêm 23 bảng KPI cho cán bộ từ KpiAssignmentTables (ID 1-23)
INSERT INTO EmployeeKpiAssignments (
    TableName,
    Description,
    AssignmentType,
    IsActive,
    CreatedAt,
    UpdatedAt
)
SELECT
    TableName,
    Description,
    'Cán bộ' as AssignmentType,
    1 as IsActive,
    GETDATE() as CreatedAt,
    GETDATE() as UpdatedAt
FROM KpiAssignmentTables
WHERE Id BETWEEN 1 AND 23
ORDER BY Id;

DECLARE @EmployeeKpiCount INT = @@ROWCOUNT;
PRINT N'   ✅ Đã thêm ' + CAST(@EmployeeKpiCount AS NVARCHAR(10)) + N' bảng KPI cho cán bộ';

-- 2. ĐỒNG BỘ DỮ LIỆU CHO "GIAO KHOÁN KPI CHO CHI NHÁNH"
PRINT N'';
PRINT N'🏢 2. ĐỒNG BỘ GIAO KHOÁN KPI CHO CHI NHÁNH (9 bảng):';

-- Xóa dữ liệu cũ nếu có
DELETE FROM UnitKpiScorings;
PRINT N'   🗑️ Đã xóa dữ liệu cũ UnitKpiScorings';

-- Thêm 9 bảng KPI cho chi nhánh từ KpiAssignmentTables (ID 24-32)
INSERT INTO UnitKpiScorings (
    TableName,
    Description,
    AssignmentType,
    IsActive,
    CreatedAt,
    UpdatedAt
)
SELECT
    TableName,
    Description,
    'Chi nhánh' as AssignmentType,
    1 as IsActive,
    GETDATE() as CreatedAt,
    GETDATE() as UpdatedAt
FROM KpiAssignmentTables
WHERE Id BETWEEN 24 AND 32
ORDER BY Id;

DECLARE @UnitKpiCount INT = @@ROWCOUNT;
PRINT N'   ✅ Đã thêm ' + CAST(@UnitKpiCount AS NVARCHAR(10)) + N' bảng KPI cho chi nhánh';

-- 3. KIỂM TRA KẾT QUẢ
PRINT N'';
PRINT N'📊 3. KIỂM TRA KẾT QUẢ ĐỒNG BỘ:';

DECLARE @TotalKpiAssignmentTables INT = (SELECT COUNT(*) FROM KpiAssignmentTables);
DECLARE @TotalEmployeeKpiAssignments INT = (SELECT COUNT(*) FROM EmployeeKpiAssignments);
DECLARE @TotalUnitKpiScorings INT = (SELECT COUNT(*) FROM UnitKpiScorings);

PRINT N'   📋 KpiAssignmentTables (Cấu hình KPI): ' + CAST(@TotalKpiAssignmentTables AS NVARCHAR(10)) + N' bảng';
PRINT N'   🧑‍💼 EmployeeKpiAssignments (Giao khoán cán bộ): ' + CAST(@TotalEmployeeKpiAssignments AS NVARCHAR(10)) + N' bảng';
PRINT N'   🏢 UnitKpiScorings (Giao khoán chi nhánh): ' + CAST(@TotalUnitKpiScorings AS NVARCHAR(10)) + N' bảng';

-- Kiểm tra tính đồng bộ
IF (@TotalEmployeeKpiAssignments = 23 AND @TotalUnitKpiScorings = 9)
BEGIN
    PRINT N'';
    PRINT N'✅ THÀNH CÔNG: Đồng bộ hoàn tất!';
    PRINT N'   - Cấu hình KPI: 32 bảng (23 cán bộ + 9 chi nhánh)';
    PRINT N'   - Giao khoán cán bộ: 23 bảng ✅';
    PRINT N'   - Giao khoán chi nhánh: 9 bảng ✅';
END
ELSE
BEGIN
    PRINT N'';
    PRINT N'⚠️ CẢNH BÁO: Số lượng không đồng bộ!';
    PRINT N'   Expected: 23 cán bộ + 9 chi nhánh';
    PRINT N'   Actual: ' + CAST(@TotalEmployeeKpiAssignments AS NVARCHAR(10)) + N' cán bộ + ' + CAST(@TotalUnitKpiScorings AS NVARCHAR(10)) + N' chi nhánh';
END

-- 4. TẠO TRIGGER ĐỒNG BỘ TỰ ĐỘNG (Optional)
PRINT N'';
PRINT N'🔄 4. THIẾT LẬP ĐỒNG BỘ TỰ ĐỘNG:';
PRINT N'   💡 Gợi ý: Tạo trigger trên KpiAssignmentTables để tự động đồng bộ';
PRINT N'   💡 Khi INSERT/UPDATE/DELETE KpiAssignmentTables → Cập nhật tương ứng';

PRINT N'';
PRINT N'🏁 HOÀN THÀNH THIẾT LẬP ĐỒNG BỘ KPI';
PRINT N'=======================================';

GO
