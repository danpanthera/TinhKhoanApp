-- ================================================
-- Script phục hồi các kỳ khoán chuẩn từ KhoanPeriodSeeder.cs
-- Dành cho bảng B1: Quản lý kỳ khoán
-- ================================================

USE TinhKhoanDB;
GO

-- Xóa toàn bộ dữ liệu hiện tại để tránh trung lặp
PRINT '🗑️ Xóa dữ liệu kỳ khoán hiện tại...';
DELETE FROM KhoanPeriods;
GO

-- Reset IDENTITY seed để bắt đầu từ ID = 1
DBCC CHECKIDENT ('KhoanPeriods', RESEED, 0);
GO

PRINT '🚀 Bắt đầu phục hồi các kỳ khoán chuẩn...';

-- Thêm các kỳ khoán theo thứ tự trong KhoanPeriodSeeder.cs
INSERT INTO KhoanPeriods ([Name], [Type], StartDate, EndDate, [Status]) VALUES
(N'Tháng 01/2025', 0, '2025-01-01 00:00:00', '2025-01-31 23:59:59', 2),  -- MONTHLY, OPEN
(N'Quý I/2025', 1, '2025-01-01 00:00:00', '2025-03-31 23:59:59', 0),      -- QUARTERLY, DRAFT
(N'Năm 2025', 2, '2025-01-01 00:00:00', '2025-12-31 23:59:59', 0),        -- ANNUAL, DRAFT
(N'Tháng 02/2025', 0, '2025-02-01 00:00:00', '2025-02-28 23:59:59', 0),   -- MONTHLY, DRAFT
(N'Tháng 03/2025', 0, '2025-03-01 00:00:00', '2025-03-31 23:59:59', 0),   -- MONTHLY, DRAFT
(N'Quý II/2025', 1, '2025-04-01 00:00:00', '2025-06-30 23:59:59', 0);     -- QUARTERLY, DRAFT

-- Kiểm tra kết quả
PRINT '📊 Kết quả phục hồi:';
SELECT
    'Total Periods' as Category,
    COUNT(*) as Count
FROM KhoanPeriods;

PRINT '✅ Hoàn thành phục hồi kỳ khoán!';

-- Hiển thị các kỳ khoán đã được tạo
PRINT '🔍 Các kỳ khoán đã được tạo:';
SELECT
    Id,
    [Name],
    CASE [Type]
        WHEN 0 THEN 'MONTHLY'
        WHEN 1 THEN 'QUARTERLY'
        WHEN 2 THEN 'ANNUAL'
        ELSE 'UNKNOWN'
    END as [Type],
    StartDate,
    EndDate,
    CASE [Status]
        WHEN 0 THEN 'DRAFT'
        WHEN 1 THEN 'ACTIVE'
        WHEN 2 THEN 'OPEN'
        WHEN 3 THEN 'CLOSED'
        ELSE 'UNKNOWN'
    END as [Status]
FROM KhoanPeriods
ORDER BY StartDate;

PRINT '📋 Tổng số kỳ khoán theo từng loại:';
SELECT
    CASE [Type]
        WHEN 0 THEN 'MONTHLY'
        WHEN 1 THEN 'QUARTERLY'
        WHEN 2 THEN 'ANNUAL'
        ELSE 'UNKNOWN'
    END as PeriodType,
    COUNT(*) as Count
FROM KhoanPeriods
GROUP BY [Type]
ORDER BY Count DESC;
