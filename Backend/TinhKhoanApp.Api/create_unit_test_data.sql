-- Create test Unit Khoan Assignments
-- This script creates sample unit assignments for testing the Unit tab

-- First, let's check what units and periods we have
SELECT 'UNITS:' as info;
SELECT Id, Name, Code, Type FROM Units WHERE Type IN ('CNL1', 'CNL2') ORDER BY Code;

SELECT 'PERIODS:' as info;  
SELECT Id, PeriodName, StartDate, EndDate FROM KhoanPeriods WHERE IsActive = 1;

-- Create some unit assignments if they don't exist
-- Let's create for CNL1 (Lai Chau) and first active period

DECLARE @UnitId INT = (SELECT TOP 1 Id FROM Units WHERE Code = 'CNLAICHAU' AND Type = 'CNL1');
DECLARE @PeriodId INT = (SELECT TOP 1 Id FROM KhoanPeriods WHERE IsActive = 1);

-- Only proceed if we have both unit and period
IF @UnitId IS NOT NULL AND @PeriodId IS NOT NULL
BEGIN
    -- Check if assignment already exists
    IF NOT EXISTS (SELECT 1 FROM UnitKhoanAssignments WHERE UnitId = @UnitId AND KhoanPeriodId = @PeriodId)
    BEGIN
        -- Create the main assignment
        INSERT INTO UnitKhoanAssignments (UnitId, KhoanPeriodId, AssignedDate, Note)
        VALUES (@UnitId, @PeriodId, GETDATE(), 'Test assignment for Lai Chau branch');
        
        DECLARE @AssignmentId INT = SCOPE_IDENTITY();
        
        -- Create assignment details (KPIs)
        INSERT INTO UnitKhoanAssignmentDetails (UnitKhoanAssignmentId, LegacyKPICode, LegacyKPIName, TargetValue, ActualValue, Score, Note)
        VALUES 
        (@AssignmentId, 'DT001', 'Doanh thu huy động tiền gửi', 50000000000, NULL, NULL, 'Chỉ tiêu doanh thu huy động'),
        (@AssignmentId, 'DT002', 'Doanh thu tín dụng', 30000000000, NULL, NULL, 'Chỉ tiêu doanh thu tín dụng'),
        (@AssignmentId, 'CP001', 'Chi phí hoạt động', 15000000000, NULL, NULL, 'Chỉ tiêu chi phí hoạt động'),
        (@AssignmentId, 'LN001', 'Lợi nhuận trước thuế', 8000000000, NULL, NULL, 'Chỉ tiêu lợi nhuận'),
        (@AssignmentId, 'NPL001', 'Tỷ lệ nợ xấu', 2.5, NULL, NULL, 'Chỉ tiêu tỷ lệ nợ xấu (%)');
        
        PRINT 'Created test assignment for Lai Chau with 5 KPIs';
    END
    ELSE
    BEGIN
        PRINT 'Assignment already exists for this unit and period';
    END
END
ELSE
BEGIN
    PRINT 'Could not find Lai Chau unit or active period';
    PRINT 'UnitId: ' + ISNULL(CAST(@UnitId AS VARCHAR), 'NULL');
    PRINT 'PeriodId: ' + ISNULL(CAST(@PeriodId AS VARCHAR), 'NULL');
END

-- Create another assignment for Tam Duong if possible
DECLARE @UnitId2 INT = (SELECT TOP 1 Id FROM Units WHERE Code = 'CNTAMDUONG');
IF @UnitId2 IS NOT NULL AND @PeriodId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM UnitKhoanAssignments WHERE UnitId = @UnitId2 AND KhoanPeriodId = @PeriodId)
    BEGIN
        INSERT INTO UnitKhoanAssignments (UnitId, KhoanPeriodId, AssignedDate, Note)
        VALUES (@UnitId2, @PeriodId, GETDATE(), 'Test assignment for Tam Duong branch');
        
        DECLARE @AssignmentId2 INT = SCOPE_IDENTITY();
        
        INSERT INTO UnitKhoanAssignmentDetails (UnitKhoanAssignmentId, LegacyKPICode, LegacyKPIName, TargetValue, ActualValue, Score, Note)
        VALUES 
        (@AssignmentId2, 'DT001', 'Doanh thu huy động tiền gửi', 25000000000, 20000000000, 80.0, 'Đã cập nhật giá trị thực hiện'),
        (@AssignmentId2, 'DT002', 'Doanh thu tín dụng', 18000000000, 19000000000, 105.6, 'Vượt chỉ tiêu'),
        (@AssignmentId2, 'CP001', 'Chi phí hoạt động', 12000000000, NULL, NULL, 'Chưa cập nhật');
        
        PRINT 'Created test assignment for Tam Duong with 3 KPIs';
    END
END

-- Show final results
SELECT 'FINAL RESULTS:' as info;
SELECT 
    ua.Id as AssignmentId,
    u.Name as UnitName,
    u.Code as UnitCode,
    p.PeriodName,
    ua.AssignedDate,
    COUNT(uad.Id) as KPICount
FROM UnitKhoanAssignments ua
    INNER JOIN Units u ON ua.UnitId = u.Id
    INNER JOIN KhoanPeriods p ON ua.KhoanPeriodId = p.Id
    LEFT JOIN UnitKhoanAssignmentDetails uad ON ua.Id = uad.UnitKhoanAssignmentId
GROUP BY ua.Id, u.Name, u.Code, p.PeriodName, ua.AssignedDate
ORDER BY ua.AssignedDate DESC;

-- Show details
SELECT 
    u.Name as UnitName,
    p.PeriodName,
    uad.LegacyKPICode,
    uad.LegacyKPIName,
    uad.TargetValue,
    uad.ActualValue,
    uad.Score
FROM UnitKhoanAssignmentDetails uad
    INNER JOIN UnitKhoanAssignments ua ON uad.UnitKhoanAssignmentId = ua.Id
    INNER JOIN Units u ON ua.UnitId = u.Id
    INNER JOIN KhoanPeriods p ON ua.KhoanPeriodId = p.Id
ORDER BY u.Name, uad.LegacyKPICode;
