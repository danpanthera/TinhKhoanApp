-- SQL script to add Lai Chau KPI table and indicators
-- This script adds the missing KPI table for Lai Chau provincial branch

USE [TinhKhoanDb]
GO

-- Insert the new KPI table for Lai Chau provincial branch
INSERT INTO [dbo].[KpiAssignmentTables] (
    [TableType], 
    [TableName], 
    [Description], 
    [Category], 
    [IsActive], 
    [CreatedDate]
)
VALUES (
    'CnTinhLaiChau',
    'Chi nhánh tỉnh Lai Châu',
    'Bảng giao khoán KPI cho Chi nhánh tỉnh Lai Châu (sử dụng chỉ tiêu giống các chi nhánh loại II)',
    'Dành cho Chi nhánh',
    1,
    GETDATE()
)
GO

-- Get the ID of the newly inserted table
DECLARE @TableId int
SELECT @TableId = Id FROM [dbo].[KpiAssignmentTables] WHERE [TableType] = 'CnTinhLaiChau'

-- Insert the 11 KPI indicators for Lai Chau (same as CNL2 branches)
INSERT INTO [dbo].[KpiIndicators] ([TableId], [IndicatorName], [MaxScore], [Unit], [OrderIndex])
VALUES 
    (@TableId, 'Tổng nguồn vốn cuối kỳ', 5, 'Tỷ VND', 1),
    (@TableId, 'Tổng nguồn vốn huy động BQ trong kỳ', 10, 'Tỷ VND', 2),
    (@TableId, 'Tổng dư nợ cuối kỳ', 5, 'Tỷ VND', 3),
    (@TableId, 'Tổng dư nợ BQ trong kỳ', 10, 'Tỷ VND', 4),
    (@TableId, 'Tổng dư nợ HSX&CN', 5, 'Tỷ VND', 5),
    (@TableId, 'Tỷ lệ nợ xấu nội bảng', 10, '%', 6),
    (@TableId, 'Thu nợ đã XLRR', 5, 'Tỷ VND', 7),
    (@TableId, 'Tỷ lệ thực thu lãi', 10, '%', 8),
    (@TableId, 'Lợi nhuận khoán tài chính', 20, 'Tỷ VND', 9),
    (@TableId, 'Thu dịch vụ', 10, 'Tỷ VND', 10),
    (@TableId, 'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank', 10, '%', 11)
GO

-- Verify the insertion
SELECT 
    t.Id, 
    t.TableType, 
    t.TableName, 
    t.Description,
    t.Category,
    COUNT(i.Id) as IndicatorCount,
    SUM(i.MaxScore) as TotalMaxScore
FROM [dbo].[KpiAssignmentTables] t
LEFT JOIN [dbo].[KpiIndicators] i ON t.Id = i.TableId
WHERE t.TableType = 'CnTinhLaiChau'
GROUP BY t.Id, t.TableType, t.TableName, t.Description, t.Category
GO
