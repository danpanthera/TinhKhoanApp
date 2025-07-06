-- Script tạo KhoanPeriods (thời gian khoán) cho hệ thống TinhKhoanApp
-- Ngày tạo: 06/07/2025
-- Mục đích: Tạo các kỳ khoán cho năm 2025

USE TinhKhoanDB;
GO

PRINT N'🎯 BẮT ĐẦU TẠO KHOAN PERIODS (THỜI GIAN KHOÁN)';
PRINT N'===============================================';

-- 1. KIỂM TRA TRẠNG THÁI HIỆN TẠI
PRINT N'';
PRINT N'📊 1. KIỂM TRA TRẠNG THÁI HIỆN TẠI:';

DECLARE @ExistingPeriodsCount INT = (SELECT COUNT(*) FROM KhoanPeriods);
PRINT N'   📅 KhoanPeriods hiện có: ' + CAST(@ExistingPeriodsCount AS NVARCHAR(10)) + N' kỳ';

-- 2. XÓA DỮ LIỆU CŨ NẾU CÓ
IF (@ExistingPeriodsCount > 0)
BEGIN
    PRINT N'';
    PRINT N'🗑️ 2. XÓA DỮ LIỆU CŨ:';
    DELETE FROM KhoanPeriods;
    PRINT N'   ✅ Đã xóa ' + CAST(@ExistingPeriodsCount AS NVARCHAR(10)) + N' kỳ khoán cũ';
END

-- 3. TẠO CÁC KỲ KHOÁN CHO NĂM 2025
PRINT N'';
PRINT N'📅 3. TẠO CÁC KỲ KHOÁN CHO NĂM 2025:';

-- Reset identity nếu cần
DBCC CHECKIDENT('KhoanPeriods', RESEED, 0);

-- Tạo 12 kỳ khoán cho năm 2025 (theo tháng)
INSERT INTO KhoanPeriods (
    Name,
    Type,
    StartDate,
    EndDate,
    Status
) VALUES
-- Quý 1/2025
(N'Tháng 01/2025', 0, '2025-01-01', '2025-01-31', 1),
(N'Tháng 02/2025', 0, '2025-02-01', '2025-02-28', 1),
(N'Tháng 03/2025', 0, '2025-03-01', '2025-03-31', 1),

-- Quý 2/2025
(N'Tháng 04/2025', 0, '2025-04-01', '2025-04-30', 1),
(N'Tháng 05/2025', 0, '2025-05-01', '2025-05-31', 1),
(N'Tháng 06/2025', 0, '2025-06-01', '2025-06-30', 1),

-- Quý 3/2025
(N'Tháng 07/2025', 0, '2025-07-01', '2025-07-31', 1),
(N'Tháng 08/2025', 0, '2025-08-01', '2025-08-31', 1),
(N'Tháng 09/2025', 0, '2025-09-01', '2025-09-30', 1),

-- Quý 4/2025
(N'Tháng 10/2025', 0, '2025-10-01', '2025-10-31', 1),
(N'Tháng 11/2025', 0, '2025-11-01', '2025-11-30', 1),
(N'Tháng 12/2025', 0, '2025-12-01', '2025-12-31', 1);

DECLARE @MonthlyPeriodsCreated INT = @@ROWCOUNT;
PRINT N'   ✅ Đã tạo ' + CAST(@MonthlyPeriodsCreated AS NVARCHAR(10)) + N' kỳ khoán tháng';

-- Tạo 4 kỳ khoán theo quý
INSERT INTO KhoanPeriods (
    Name,
    Type,
    StartDate,
    EndDate,
    Status
) VALUES
(N'Quý I/2025', 1, '2025-01-01', '2025-03-31', 1),
(N'Quý II/2025', 1, '2025-04-01', '2025-06-30', 1),
(N'Quý III/2025', 1, '2025-07-01', '2025-09-30', 1),
(N'Quý IV/2025', 1, '2025-10-01', '2025-12-31', 1);

DECLARE @QuarterlyPeriodsCreated INT = @@ROWCOUNT;
PRINT N'   ✅ Đã tạo ' + CAST(@QuarterlyPeriodsCreated AS NVARCHAR(10)) + N' kỳ khoán quý';

-- Tạo 1 kỳ khoán cả năm
INSERT INTO KhoanPeriods (
    Name,
    Type,
    StartDate,
    EndDate,
    Status
) VALUES
(N'Năm 2025', 2, '2025-01-01', '2025-12-31', 1);

DECLARE @YearlyPeriodsCreated INT = @@ROWCOUNT;
PRINT N'   ✅ Đã tạo ' + CAST(@YearlyPeriodsCreated AS NVARCHAR(10)) + N' kỳ khoán năm';

-- 4. KIỂM TRA KẾT QUẢ
PRINT N'';
PRINT N'📊 4. KIỂM TRA KẾT QUẢ:';

DECLARE @TotalPeriodsCreated INT = (SELECT COUNT(*) FROM KhoanPeriods);
DECLARE @MonthlyCount INT = (SELECT COUNT(*) FROM KhoanPeriods WHERE Type = 0);
DECLARE @QuarterlyCount INT = (SELECT COUNT(*) FROM KhoanPeriods WHERE Type = 1);
DECLARE @YearlyCount INT = (SELECT COUNT(*) FROM KhoanPeriods WHERE Type = 2);

PRINT N'   📅 Tổng kỳ khoán: ' + CAST(@TotalPeriodsCreated AS NVARCHAR(10));
PRINT N'       - Theo tháng: ' + CAST(@MonthlyCount AS NVARCHAR(10)) + N' kỳ';
PRINT N'       - Theo quý: ' + CAST(@QuarterlyCount AS NVARCHAR(10)) + N' kỳ';
PRINT N'       - Theo năm: ' + CAST(@YearlyCount AS NVARCHAR(10)) + N' kỳ';

-- Hiển thị danh sách
PRINT N'';
PRINT N'📋 5. DANH SÁCH KỲ KHOÁN ĐÃ TẠO:';
SELECT
    Id,
    Name,
    Type,
    FORMAT(StartDate, 'dd/MM/yyyy') as StartDate,
    FORMAT(EndDate, 'dd/MM/yyyy') as EndDate,
    CASE WHEN Status = 1 THEN N'Hoạt động' ELSE N'Không hoạt động' END as Status
FROM KhoanPeriods
ORDER BY Type, StartDate;

-- 5. KẾT LUẬN
PRINT N'';
IF (@TotalPeriodsCreated = 17) -- 12 tháng + 4 quý + 1 năm
BEGIN
    PRINT N'✅ THÀNH CÔNG: Đã tạo đầy đủ ' + CAST(@TotalPeriodsCreated AS NVARCHAR(10)) + N' kỳ khoán!';
    PRINT N'   🎯 Sẵn sàng cho bước tiếp theo: Gán Employees vào Units và Roles';
END
ELSE
BEGIN
    PRINT N'⚠️ CẢNH BÁO: Số lượng kỳ khoán không đúng (' + CAST(@TotalPeriodsCreated AS NVARCHAR(10)) + N'/17)';
END

PRINT N'';
PRINT N'🏁 HOÀN THÀNH TẠO KHOAN PERIODS';
PRINT N'===============================';

GO
