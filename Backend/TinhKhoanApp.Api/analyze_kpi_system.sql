-- Script hiểu đúng mối quan hệ KPI và chuẩn bị dữ liệu
-- Ngày tạo: 06/07/2025
-- Mục đích: Làm rõ mối quan hệ giữa các bảng KPI

USE TinhKhoanDB;
GO

PRINT N'🎯 PHÂN TÍCH HỆ THỐNG KPI ASSIGNMENT';
PRINT N'====================================';

-- 1. KIỂM TRA TRẠNG THÁI HIỆN TẠI
PRINT N'';
PRINT N'📊 1. TRẠNG THÁI HIỆN TẠI:';

DECLARE @KpiAssignmentTablesCount INT = (SELECT COUNT(*) FROM KpiAssignmentTables);
DECLARE @KpiAssignmentTablesCanBo INT = (SELECT COUNT(*) FROM KpiAssignmentTables WHERE Category = 'CANBO');
DECLARE @KpiAssignmentTablesChiNhanh INT = (SELECT COUNT(*) FROM KpiAssignmentTables WHERE Category = 'CHINHANH');

DECLARE @EmployeeKpiAssignmentsCount INT = (SELECT COUNT(*) FROM EmployeeKpiAssignments);
DECLARE @UnitKpiScoringsCount INT = (SELECT COUNT(*) FROM UnitKpiScorings);

DECLARE @EmployeesCount INT = (SELECT COUNT(*) FROM Employees);
DECLARE @UnitsCount INT = (SELECT COUNT(*) FROM Units);
DECLARE @RolesCount INT = (SELECT COUNT(*) FROM Roles);
DECLARE @KpiDefinitionsCount INT = (SELECT COUNT(*) FROM KPIDefinitions);

PRINT N'   📋 KpiAssignmentTables (Cấu hình KPI): ' + CAST(@KpiAssignmentTablesCount AS NVARCHAR(10)) + N' bảng';
PRINT N'       - Cán bộ: ' + CAST(@KpiAssignmentTablesCanBo AS NVARCHAR(10)) + N' bảng';
PRINT N'       - Chi nhánh: ' + CAST(@KpiAssignmentTablesChiNhanh AS NVARCHAR(10)) + N' bảng';
PRINT N'   🧑‍💼 EmployeeKpiAssignments (Giao khoán thực tế): ' + CAST(@EmployeeKpiAssignmentsCount AS NVARCHAR(10)) + N' records';
PRINT N'   🏢 UnitKpiScorings (Đánh giá chi nhánh): ' + CAST(@UnitKpiScoringsCount AS NVARCHAR(10)) + N' records';

PRINT N'';
PRINT N'📈 2. DỮ LIỆU HỖ TRỢ:';
PRINT N'   👥 Employees: ' + CAST(@EmployeesCount AS NVARCHAR(10)) + N' nhân viên';
PRINT N'   🏢 Units: ' + CAST(@UnitsCount AS NVARCHAR(10)) + N' đơn vị';
PRINT N'   🎭 Roles: ' + CAST(@RolesCount AS NVARCHAR(10)) + N' vai trò';
PRINT N'   📊 KPIDefinitions: ' + CAST(@KpiDefinitionsCount AS NVARCHAR(10)) + N' định nghĩa KPI';

-- 2. PHÂN TÍCH MỐI QUAN HỆ
PRINT N'';
PRINT N'🔗 3. MỐI QUAN HỆ HỆ THỐNG KPI:';
PRINT N'';
PRINT N'   📋 KpiAssignmentTables (Bảng cấu hình - Template)';
PRINT N'   ├── 23 bảng KPI cho cán bộ (Category = "CANBO")';
PRINT N'   └── 9 bảng KPI cho chi nhánh (Category = "CHINHANH")';
PRINT N'';
PRINT N'   🧑‍💼 EmployeeKpiAssignments (Giao khoán thực tế cho cán bộ)';
PRINT N'   ├── EmployeeId → FK to Employees';
PRINT N'   ├── KpiDefinitionId → FK to KPIDefinitions';
PRINT N'   ├── KhoanPeriodId → FK to KhoanPeriods';
PRINT N'   └── TargetValue, ActualValue, Score';
PRINT N'';
PRINT N'   🏢 UnitKpiScorings (Đánh giá thực tế cho chi nhánh)';
PRINT N'   ├── UnitId → FK to Units';
PRINT N'   ├── KhoanPeriodId → FK to KhoanPeriods';
PRINT N'   └── TotalScore, BaseScore, AdjustmentScore';

-- 3. YÊU CẦU ĐỂ HOẠT ĐỘNG
PRINT N'';
PRINT N'🔧 4. YÊU CẦU ĐỂ HOẠT ĐỘNG GIAO KHOÁN KPI:';

IF (@EmployeesCount = 0)
    PRINT N'   ❌ Cần có danh sách Employees để giao khoán KPI';
ELSE
    PRINT N'   ✅ Đã có ' + CAST(@EmployeesCount AS NVARCHAR(10)) + N' Employees';

IF (@KpiDefinitionsCount = 0)
    PRINT N'   ❌ Cần có KPI Definitions để giao khoán';
ELSE
    PRINT N'   ✅ Đã có ' + CAST(@KpiDefinitionsCount AS NVARCHAR(10)) + N' KPI Definitions';

DECLARE @KhoanPeriodsCount INT = (SELECT COUNT(*) FROM KhoanPeriods);
IF (@KhoanPeriodsCount = 0)
    PRINT N'   ❌ Cần có Khoan Periods để xác định thời gian';
ELSE
    PRINT N'   ✅ Đã có ' + CAST(@KhoanPeriodsCount AS NVARCHAR(10)) + N' Khoan Periods';

-- 4. KẾT LUẬN VÀ HƯỚNG DẪN
PRINT N'';
PRINT N'🎯 5. TÌNH HÌNH VÀ HƯỚNG DẪN:';

IF (@KpiAssignmentTablesCount = 32 AND @UnitsCount = 46 AND @RolesCount = 23)
BEGIN
    PRINT N'   ✅ CẤU HÌNH KPI: Đã hoàn thành (32 bảng template)';
    PRINT N'   ✅ ĐƠN VỊ: Đã có đủ 46 đơn vị';
    PRINT N'   ✅ VAI TRÒ: Đã có đủ 23 vai trò';

    IF (@EmployeesCount > 0 AND @KpiDefinitionsCount > 0)
    BEGIN
        PRINT N'   🚀 SẴN SÀNG: Có thể bắt đầu giao khoán KPI thực tế!';
        PRINT N'';
        PRINT N'   📝 BƯỚC TIẾP THEO:';
        PRINT N'      1. Gán Employees vào Units và Roles';
        PRINT N'      2. Tạo EmployeeKpiAssignments dựa trên vai trò';
        PRINT N'      3. Tạo UnitKpiScorings cho đánh giá chi nhánh';
    END
    ELSE
    BEGIN
        PRINT N'   ⏳ CHUẨN BỊ: Cần import Employees và KPI Definitions';
    END
END
ELSE
BEGIN
    PRINT N'   ❌ CHƯA ĐỦ: Kiểm tra lại dữ liệu cơ bản';
END

PRINT N'';
PRINT N'🏁 HOÀN THÀNH PHÂN TÍCH';
PRINT N'======================';

GO
