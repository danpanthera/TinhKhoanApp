-- Fix KPI Indicators for CANBO tables according to standard specification
-- This script corrects the number of indicators per table based on the requirements

PRINT '🔧 Starting KPI Indicators correction for CANBO tables...'

-- First, let's see the current state
PRINT '📊 Current CANBO table indicator counts:'
SELECT
    t.TableName,
    LEFT(t.Description, 50) + '...' as Description,
    COUNT(i.Id) as CurrentCount
FROM KpiAssignmentTables t
LEFT JOIN KpiIndicators i ON t.Id = i.TableId
WHERE t.Category = 'CANBO'
GROUP BY t.Id, t.TableName, t.Description
ORDER BY t.TableName

-- Define the correct specification for each table type
PRINT ''
PRINT '🎯 Target specification:'
PRINT '1-4.   KHDN/KHCN: 4 tables × 8 indicators = 32'
PRINT '5-6.   KH&QLRR: 2 tables × 6 indicators = 12'
PRINT '7.     CBTD: 1 table × 8 indicators = 8'
PRINT '8-9.   KTNQ CNL1: 2 tables × 6 indicators = 12'
PRINT '10.    GDV: 1 table × 6 indicators = 6'
PRINT '11.    TQ/HK/KTNB: 1 table × 6 indicators = 6 (missing from count)'
PRINT '12.    IT/TH/KTGS: 1 table × 5 indicators = 5'
PRINT '13.    CB IT/TH/KTGS: 1 table × 4 indicators = 4'
PRINT '14-15. GD PGD: 2 tables × 9 indicators = 18'
PRINT '16.    PGD CBTD: 1 table × 8 indicators = 8'
PRINT '17.    GD CNL2: 1 table × 11 indicators = 11'
PRINT '18.    PGD CNL2 TD: 1 table × 8 indicators = 8'
PRINT '19.    PGD CNL2 KT: 1 table × 6 indicators = 6'
PRINT '20.    TP KH CNL2: 1 table × 9 indicators = 9'
PRINT '21.    PP KH CNL2: 1 table × 8 indicators = 8'
PRINT '22.    TP KTNQ CNL2: 1 table × 6 indicators = 6'
PRINT '23.    PP KTNQ CNL2: 1 table × 5 indicators = 5'
PRINT 'Total: 158 indicators'

-- Clear all existing CANBO indicators first
PRINT ''
PRINT '🧹 Clearing existing CANBO indicators...'
DELETE i FROM KpiIndicators i
INNER JOIN KpiAssignmentTables t ON i.TableId = t.Id
WHERE t.Category = 'CANBO'

DECLARE @ClearedCount INT = @@ROWCOUNT
PRINT '✅ Cleared ' + CAST(@ClearedCount AS NVARCHAR(10)) + ' existing indicators'

-- Now repopulate with correct counts
PRINT ''
PRINT '🔨 Repopulating indicators with correct specification...'

DECLARE @TotalInserted INT = 0

-- Helper procedure to create indicators for a table
DECLARE @TableId INT, @TableName NVARCHAR(200), @TargetCount INT, @InsertedForTable INT

-- 1-4. KHDN/KHCN: 4 tables × 8 indicators = 32
DECLARE khdn_khcn_cursor CURSOR FOR
SELECT Id, TableName FROM KpiAssignmentTables
WHERE Category = 'CANBO' AND (
    TableName LIKE '%Khdn%' OR
    TableName LIKE '%Khcn%'
)

OPEN khdn_khcn_cursor
FETCH NEXT FROM khdn_khcn_cursor INTO @TableId, @TableName

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @TargetCount = 8
    SET @InsertedForTable = 0

    -- Insert 8 indicators for KHDN/KHCN tables
    WHILE @InsertedForTable < @TargetCount
    BEGIN
        SET @InsertedForTable = @InsertedForTable + 1
        INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
        VALUES (
            @TableId,
            'Chỉ tiêu ' + CAST(@InsertedForTable AS NVARCHAR(2)) + ' - ' + REPLACE(@TableName, '_KPI_Assignment', ''),
            CASE @InsertedForTable
                WHEN 1 THEN 20.0
                WHEN 2 THEN 15.0
                WHEN 3 THEN 15.0
                WHEN 4 THEN 10.0
                WHEN 5 THEN 10.0
                WHEN 6 THEN 10.0
                WHEN 7 THEN 10.0
                ELSE 10.0
            END,
            CASE @InsertedForTable
                WHEN 1 THEN 'Triệu VND'
                WHEN 2 THEN '%'
                WHEN 3 THEN 'Triệu VND'
                ELSE '%'
            END,
            @InsertedForTable,
            CASE @InsertedForTable
                WHEN 1 THEN 3 -- CURRENCY
                WHEN 3 THEN 3 -- CURRENCY
                ELSE 2 -- PERCENTAGE
            END,
            1
        )
    END

    SET @TotalInserted = @TotalInserted + @TargetCount
    PRINT '✅ ' + @TableName + ': ' + CAST(@TargetCount AS NVARCHAR(2)) + ' indicators'

    FETCH NEXT FROM khdn_khcn_cursor INTO @TableId, @TableName
END

CLOSE khdn_khcn_cursor
DEALLOCATE khdn_khcn_cursor

-- 5-6. KH&QLRR: 2 tables × 6 indicators = 12
DECLARE khqlrr_cursor CURSOR FOR
SELECT Id, TableName FROM KpiAssignmentTables
WHERE Category = 'CANBO' AND TableName LIKE '%Khqlrr%'

OPEN khqlrr_cursor
FETCH NEXT FROM khqlrr_cursor INTO @TableId, @TableName

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @TargetCount = 6
    SET @InsertedForTable = 0

    WHILE @InsertedForTable < @TargetCount
    BEGIN
        SET @InsertedForTable = @InsertedForTable + 1
        INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
        VALUES (
            @TableId,
            'Chỉ tiêu ' + CAST(@InsertedForTable AS NVARCHAR(2)) + ' - KH&QLRR',
            CASE @InsertedForTable
                WHEN 1 THEN 20.0
                WHEN 2 THEN 20.0
                WHEN 3 THEN 20.0
                WHEN 4 THEN 15.0
                WHEN 5 THEN 15.0
                ELSE 10.0
            END,
            '%',
            @InsertedForTable,
            2, -- PERCENTAGE
            1
        )
    END

    SET @TotalInserted = @TotalInserted + @TargetCount
    PRINT '✅ ' + @TableName + ': ' + CAST(@TargetCount AS NVARCHAR(2)) + ' indicators'

    FETCH NEXT FROM khqlrr_cursor INTO @TableId, @TableName
END

CLOSE khqlrr_cursor
DEALLOCATE khqlrr_cursor

-- 7. CBTD: 1 table × 8 indicators = 8
SELECT @TableId = Id, @TableName = TableName FROM KpiAssignmentTables
WHERE Category = 'CANBO' AND TableName LIKE '%Cbtd%'

IF @TableId IS NOT NULL
BEGIN
    SET @TargetCount = 8
    SET @InsertedForTable = 0

    WHILE @InsertedForTable < @TargetCount
    BEGIN
        SET @InsertedForTable = @InsertedForTable + 1
        INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
        VALUES (
            @TableId,
            'Chỉ tiêu ' + CAST(@InsertedForTable AS NVARCHAR(2)) + ' - CBTD',
            CASE @InsertedForTable
                WHEN 1 THEN 15.0
                WHEN 2 THEN 15.0
                WHEN 3 THEN 15.0
                WHEN 4 THEN 15.0
                WHEN 5 THEN 15.0
                WHEN 6 THEN 10.0
                WHEN 7 THEN 10.0
                ELSE 5.0
            END,
            CASE @InsertedForTable
                WHEN 1 THEN 'Triệu VND'
                WHEN 2 THEN '%'
                ELSE '%'
            END,
            @InsertedForTable,
            CASE @InsertedForTable
                WHEN 1 THEN 3 -- CURRENCY
                ELSE 2 -- PERCENTAGE
            END,
            1
        )
    END

    SET @TotalInserted = @TotalInserted + @TargetCount
    PRINT '✅ ' + @TableName + ': ' + CAST(@TargetCount AS NVARCHAR(2)) + ' indicators'
END

-- 8-9. KTNQ CNL1: 2 tables × 6 indicators = 12
DECLARE ktnq_cnl1_cursor CURSOR FOR
SELECT Id, TableName FROM KpiAssignmentTables
WHERE Category = 'CANBO' AND TableName LIKE '%KtnqCnl1%'

OPEN ktnq_cnl1_cursor
FETCH NEXT FROM ktnq_cnl1_cursor INTO @TableId, @TableName

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @TargetCount = 6
    SET @InsertedForTable = 0

    WHILE @InsertedForTable < @TargetCount
    BEGIN
        SET @InsertedForTable = @InsertedForTable + 1
        INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
        VALUES (
            @TableId,
            'Chỉ tiêu ' + CAST(@InsertedForTable AS NVARCHAR(2)) + ' - KTNQ CNL1',
            CASE @InsertedForTable
                WHEN 1 THEN 20.0
                WHEN 2 THEN 20.0
                WHEN 3 THEN 20.0
                WHEN 4 THEN 20.0
                WHEN 5 THEN 10.0
                ELSE 10.0
            END,
            '%',
            @InsertedForTable,
            2, -- PERCENTAGE
            1
        )
    END

    SET @TotalInserted = @TotalInserted + @TargetCount
    PRINT '✅ ' + @TableName + ': ' + CAST(@TargetCount AS NVARCHAR(2)) + ' indicators'

    FETCH NEXT FROM ktnq_cnl1_cursor INTO @TableId, @TableName
END

CLOSE ktnq_cnl1_cursor
DEALLOCATE ktnq_cnl1_cursor

-- 10. GDV: 1 table × 6 indicators = 6
SELECT @TableId = Id, @TableName = TableName FROM KpiAssignmentTables
WHERE Category = 'CANBO' AND TableName LIKE '%Gdv%'

IF @TableId IS NOT NULL
BEGIN
    SET @TargetCount = 6
    SET @InsertedForTable = 0

    WHILE @InsertedForTable < @TargetCount
    BEGIN
        SET @InsertedForTable = @InsertedForTable + 1
        INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
        VALUES (
            @TableId,
            'Chỉ tiêu ' + CAST(@InsertedForTable AS NVARCHAR(2)) + ' - GDV',
            CASE @InsertedForTable
                WHEN 1 THEN 20.0
                WHEN 2 THEN 20.0
                WHEN 3 THEN 20.0
                WHEN 4 THEN 15.0
                WHEN 5 THEN 15.0
                ELSE 10.0
            END,
            '%',
            @InsertedForTable,
            2, -- PERCENTAGE
            1
        )
    END

    SET @TotalInserted = @TotalInserted + @TargetCount
    PRINT '✅ ' + @TableName + ': ' + CAST(@TargetCount AS NVARCHAR(2)) + ' indicators'
END

-- 11. TQ/HK/KTNB: 1 table × 6 indicators = 6
SELECT @TableId = Id, @TableName = TableName FROM KpiAssignmentTables
WHERE Category = 'CANBO' AND TableName LIKE '%TqHkKtnb%'

IF @TableId IS NOT NULL
BEGIN
    SET @TargetCount = 6
    SET @InsertedForTable = 0

    WHILE @InsertedForTable < @TargetCount
    BEGIN
        SET @InsertedForTable = @InsertedForTable + 1
        INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
        VALUES (
            @TableId,
            'Chỉ tiêu ' + CAST(@InsertedForTable AS NVARCHAR(2)) + ' - TQ/HK/KTNB',
            CASE @InsertedForTable
                WHEN 1 THEN 20.0
                WHEN 2 THEN 20.0
                WHEN 3 THEN 20.0
                WHEN 4 THEN 15.0
                WHEN 5 THEN 15.0
                ELSE 10.0
            END,
            '%',
            @InsertedForTable,
            2, -- PERCENTAGE
            1
        )
    END

    SET @TotalInserted = @TotalInserted + @TargetCount
    PRINT '✅ ' + @TableName + ': ' + CAST(@TargetCount AS NVARCHAR(2)) + ' indicators'
END

-- 12. IT/TH/KTGS: 1 table × 5 indicators = 5
SELECT @TableId = Id, @TableName = TableName FROM KpiAssignmentTables
WHERE Category = 'CANBO' AND TableName LIKE '%TruongphongItThKtgs%'

IF @TableId IS NOT NULL
BEGIN
    SET @TargetCount = 5
    SET @InsertedForTable = 0

    WHILE @InsertedForTable < @TargetCount
    BEGIN
        SET @InsertedForTable = @InsertedForTable + 1
        INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
        VALUES (
            @TableId,
            'Chỉ tiêu ' + CAST(@InsertedForTable AS NVARCHAR(2)) + ' - IT/TH/KTGS',
            CASE @InsertedForTable
                WHEN 1 THEN 25.0
                WHEN 2 THEN 25.0
                WHEN 3 THEN 20.0
                WHEN 4 THEN 15.0
                ELSE 15.0
            END,
            '%',
            @InsertedForTable,
            2, -- PERCENTAGE
            1
        )
    END

    SET @TotalInserted = @TotalInserted + @TargetCount
    PRINT '✅ ' + @TableName + ': ' + CAST(@TargetCount AS NVARCHAR(2)) + ' indicators'
END

-- 13. CB IT/TH/KTGS: 1 table × 4 indicators = 4 (already correct)
SELECT @TableId = Id, @TableName = TableName FROM KpiAssignmentTables
WHERE Category = 'CANBO' AND TableName LIKE '%CbItThKtgsKhqlrr%'

IF @TableId IS NOT NULL
BEGIN
    SET @TargetCount = 4
    SET @InsertedForTable = 0

    WHILE @InsertedForTable < @TargetCount
    BEGIN
        SET @InsertedForTable = @InsertedForTable + 1
        INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
        VALUES (
            @TableId,
            'Chỉ tiêu ' + CAST(@InsertedForTable AS NVARCHAR(2)) + ' - CB IT/TH/KTGS/KHQLRR',
            CASE @InsertedForTable
                WHEN 1 THEN 30.0
                WHEN 2 THEN 30.0
                WHEN 3 THEN 20.0
                ELSE 20.0
            END,
            '%',
            @InsertedForTable,
            2, -- PERCENTAGE
            1
        )
    END

    SET @TotalInserted = @TotalInserted + @TargetCount
    PRINT '✅ ' + @TableName + ': ' + CAST(@TargetCount AS NVARCHAR(2)) + ' indicators'
END

-- 14-15. GD PGD: 2 tables × 9 indicators = 18
DECLARE gd_pgd_cursor CURSOR FOR
SELECT Id, TableName FROM KpiAssignmentTables
WHERE Category = 'CANBO' AND TableName LIKE '%GiamdocPgd%' AND TableName NOT LIKE '%Cbtd%'

OPEN gd_pgd_cursor
FETCH NEXT FROM gd_pgd_cursor INTO @TableId, @TableName

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @TargetCount = 9
    SET @InsertedForTable = 0

    WHILE @InsertedForTable < @TargetCount
    BEGIN
        SET @InsertedForTable = @InsertedForTable + 1
        INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
        VALUES (
            @TableId,
            'Chỉ tiêu ' + CAST(@InsertedForTable AS NVARCHAR(2)) + ' - GD PGD',
            CASE @InsertedForTable
                WHEN 1 THEN 15.0
                WHEN 2 THEN 15.0
                WHEN 3 THEN 15.0
                WHEN 4 THEN 10.0
                WHEN 5 THEN 10.0
                WHEN 6 THEN 10.0
                WHEN 7 THEN 10.0
                WHEN 8 THEN 10.0
                ELSE 5.0
            END,
            CASE @InsertedForTable
                WHEN 1 THEN 'Triệu VND'
                WHEN 2 THEN '%'
                ELSE '%'
            END,
            @InsertedForTable,
            CASE @InsertedForTable
                WHEN 1 THEN 3 -- CURRENCY
                ELSE 2 -- PERCENTAGE
            END,
            1
        )
    END

    SET @TotalInserted = @TotalInserted + @TargetCount
    PRINT '✅ ' + @TableName + ': ' + CAST(@TargetCount AS NVARCHAR(2)) + ' indicators'

    FETCH NEXT FROM gd_pgd_cursor INTO @TableId, @TableName
END

CLOSE gd_pgd_cursor
DEALLOCATE gd_pgd_cursor

-- 16. PGD CBTD: 1 table × 8 indicators = 8
SELECT @TableId = Id, @TableName = TableName FROM KpiAssignmentTables
WHERE Category = 'CANBO' AND TableName LIKE '%PhogiamdocPgdCbtd%'

IF @TableId IS NOT NULL
BEGIN
    SET @TargetCount = 8
    SET @InsertedForTable = 0

    WHILE @InsertedForTable < @TargetCount
    BEGIN
        SET @InsertedForTable = @InsertedForTable + 1
        INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
        VALUES (
            @TableId,
            'Chỉ tiêu ' + CAST(@InsertedForTable AS NVARCHAR(2)) + ' - PGD CBTD',
            CASE @InsertedForTable
                WHEN 1 THEN 15.0
                WHEN 2 THEN 15.0
                WHEN 3 THEN 15.0
                WHEN 4 THEN 15.0
                WHEN 5 THEN 10.0
                WHEN 6 THEN 10.0
                WHEN 7 THEN 10.0
                ELSE 10.0
            END,
            CASE @InsertedForTable
                WHEN 1 THEN 'Triệu VND'
                WHEN 2 THEN '%'
                ELSE '%'
            END,
            @InsertedForTable,
            CASE @InsertedForTable
                WHEN 1 THEN 3 -- CURRENCY
                ELSE 2 -- PERCENTAGE
            END,
            1
        )
    END

    SET @TotalInserted = @TotalInserted + @TargetCount
    PRINT '✅ ' + @TableName + ': ' + CAST(@TargetCount AS NVARCHAR(2)) + ' indicators'
END

-- 17. GD CNL2: 1 table × 11 indicators = 11
SELECT @TableId = Id, @TableName = TableName FROM KpiAssignmentTables
WHERE Category = 'CANBO' AND TableName LIKE '%GiamdocCnl2%'

IF @TableId IS NOT NULL
BEGIN
    SET @TargetCount = 11
    SET @InsertedForTable = 0

    WHILE @InsertedForTable < @TargetCount
    BEGIN
        SET @InsertedForTable = @InsertedForTable + 1
        INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
        VALUES (
            @TableId,
            'Chỉ tiêu ' + CAST(@InsertedForTable AS NVARCHAR(2)) + ' - GD CNL2',
            CASE @InsertedForTable
                WHEN 1 THEN 10.0
                WHEN 2 THEN 10.0
                WHEN 3 THEN 10.0
                WHEN 4 THEN 10.0
                WHEN 5 THEN 10.0
                WHEN 6 THEN 10.0
                WHEN 7 THEN 10.0
                WHEN 8 THEN 10.0
                WHEN 9 THEN 10.0
                WHEN 10 THEN 5.0
                ELSE 5.0
            END,
            CASE @InsertedForTable
                WHEN 1 THEN 'Triệu VND'
                WHEN 2 THEN '%'
                ELSE '%'
            END,
            @InsertedForTable,
            CASE @InsertedForTable
                WHEN 1 THEN 3 -- CURRENCY
                ELSE 2 -- PERCENTAGE
            END,
            1
        )
    END

    SET @TotalInserted = @TotalInserted + @TargetCount
    PRINT '✅ ' + @TableName + ': ' + CAST(@TargetCount AS NVARCHAR(2)) + ' indicators'
END

-- 18. PGD CNL2 TD: 1 table × 8 indicators = 8 (already correct)
SELECT @TableId = Id, @TableName = TableName FROM KpiAssignmentTables
WHERE Category = 'CANBO' AND TableName LIKE '%PhogiamdocCnl2Td%'

IF @TableId IS NOT NULL
BEGIN
    SET @TargetCount = 8
    SET @InsertedForTable = 0

    WHILE @InsertedForTable < @TargetCount
    BEGIN
        SET @InsertedForTable = @InsertedForTable + 1
        INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
        VALUES (
            @TableId,
            'Chỉ tiêu ' + CAST(@InsertedForTable AS NVARCHAR(2)) + ' - PGD CNL2 TD',
            CASE @InsertedForTable
                WHEN 1 THEN 15.0
                WHEN 2 THEN 15.0
                WHEN 3 THEN 15.0
                WHEN 4 THEN 15.0
                WHEN 5 THEN 15.0
                WHEN 6 THEN 10.0
                WHEN 7 THEN 10.0
                ELSE 5.0
            END,
            CASE @InsertedForTable
                WHEN 1 THEN 'Triệu VND'
                WHEN 2 THEN '%'
                ELSE '%'
            END,
            @InsertedForTable,
            CASE @InsertedForTable
                WHEN 1 THEN 3 -- CURRENCY
                ELSE 2 -- PERCENTAGE
            END,
            1
        )
    END

    SET @TotalInserted = @TotalInserted + @TargetCount
    PRINT '✅ ' + @TableName + ': ' + CAST(@TargetCount AS NVARCHAR(2)) + ' indicators'
END

-- 19. PGD CNL2 KT: 1 table × 6 indicators = 6
SELECT @TableId = Id, @TableName = TableName FROM KpiAssignmentTables
WHERE Category = 'CANBO' AND TableName LIKE '%PhogiamdocCnl2Kt%'

IF @TableId IS NOT NULL
BEGIN
    SET @TargetCount = 6
    SET @InsertedForTable = 0

    WHILE @InsertedForTable < @TargetCount
    BEGIN
        SET @InsertedForTable = @InsertedForTable + 1
        INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
        VALUES (
            @TableId,
            'Chỉ tiêu ' + CAST(@InsertedForTable AS NVARCHAR(2)) + ' - PGD CNL2 KT',
            CASE @InsertedForTable
                WHEN 1 THEN 20.0
                WHEN 2 THEN 20.0
                WHEN 3 THEN 20.0
                WHEN 4 THEN 15.0
                WHEN 5 THEN 15.0
                ELSE 10.0
            END,
            '%',
            @InsertedForTable,
            2, -- PERCENTAGE
            1
        )
    END

    SET @TotalInserted = @TotalInserted + @TargetCount
    PRINT '✅ ' + @TableName + ': ' + CAST(@TargetCount AS NVARCHAR(2)) + ' indicators'
END

-- 20. TP KH CNL2: 1 table × 9 indicators = 9
SELECT @TableId = Id, @TableName = TableName FROM KpiAssignmentTables
WHERE Category = 'CANBO' AND TableName LIKE '%TruongphongKhCnl2%'

IF @TableId IS NOT NULL
BEGIN
    SET @TargetCount = 9
    SET @InsertedForTable = 0

    WHILE @InsertedForTable < @TargetCount
    BEGIN
        SET @InsertedForTable = @InsertedForTable + 1
        INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
        VALUES (
            @TableId,
            'Chỉ tiêu ' + CAST(@InsertedForTable AS NVARCHAR(2)) + ' - TP KH CNL2',
            CASE @InsertedForTable
                WHEN 1 THEN 15.0
                WHEN 2 THEN 15.0
                WHEN 3 THEN 15.0
                WHEN 4 THEN 15.0
                WHEN 5 THEN 10.0
                WHEN 6 THEN 10.0
                WHEN 7 THEN 10.0
                WHEN 8 THEN 5.0
                ELSE 5.0
            END,
            CASE @InsertedForTable
                WHEN 1 THEN 'Triệu VND'
                WHEN 2 THEN '%'
                ELSE '%'
            END,
            @InsertedForTable,
            CASE @InsertedForTable
                WHEN 1 THEN 3 -- CURRENCY
                ELSE 2 -- PERCENTAGE
            END,
            1
        )
    END

    SET @TotalInserted = @TotalInserted + @TargetCount
    PRINT '✅ ' + @TableName + ': ' + CAST(@TargetCount AS NVARCHAR(2)) + ' indicators'
END

-- 21. PP KH CNL2: 1 table × 8 indicators = 8
SELECT @TableId = Id, @TableName = TableName FROM KpiAssignmentTables
WHERE Category = 'CANBO' AND TableName LIKE '%PhophongKhCnl2%'

IF @TableId IS NOT NULL
BEGIN
    SET @TargetCount = 8
    SET @InsertedForTable = 0

    WHILE @InsertedForTable < @TargetCount
    BEGIN
        SET @InsertedForTable = @InsertedForTable + 1
        INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
        VALUES (
            @TableId,
            'Chỉ tiêu ' + CAST(@InsertedForTable AS NVARCHAR(2)) + ' - PP KH CNL2',
            CASE @InsertedForTable
                WHEN 1 THEN 15.0
                WHEN 2 THEN 15.0
                WHEN 3 THEN 15.0
                WHEN 4 THEN 15.0
                WHEN 5 THEN 15.0
                WHEN 6 THEN 10.0
                WHEN 7 THEN 10.0
                ELSE 5.0
            END,
            CASE @InsertedForTable
                WHEN 1 THEN 'Triệu VND'
                WHEN 2 THEN '%'
                ELSE '%'
            END,
            @InsertedForTable,
            CASE @InsertedForTable
                WHEN 1 THEN 3 -- CURRENCY
                ELSE 2 -- PERCENTAGE
            END,
            1
        )
    END

    SET @TotalInserted = @TotalInserted + @TargetCount
    PRINT '✅ ' + @TableName + ': ' + CAST(@TargetCount AS NVARCHAR(2)) + ' indicators'
END

-- 22. TP KTNQ CNL2: 1 table × 6 indicators = 6 (already correct)
SELECT @TableId = Id, @TableName = TableName FROM KpiAssignmentTables
WHERE Category = 'CANBO' AND TableName LIKE '%TruongphongKtnqCnl2%'

IF @TableId IS NOT NULL
BEGIN
    SET @TargetCount = 6
    SET @InsertedForTable = 0

    WHILE @InsertedForTable < @TargetCount
    BEGIN
        SET @InsertedForTable = @InsertedForTable + 1
        INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
        VALUES (
            @TableId,
            'Chỉ tiêu ' + CAST(@InsertedForTable AS NVARCHAR(2)) + ' - TP KTNQ CNL2',
            CASE @InsertedForTable
                WHEN 1 THEN 20.0
                WHEN 2 THEN 20.0
                WHEN 3 THEN 20.0
                WHEN 4 THEN 15.0
                WHEN 5 THEN 15.0
                ELSE 10.0
            END,
            '%',
            @InsertedForTable,
            2, -- PERCENTAGE
            1
        )
    END

    SET @TotalInserted = @TotalInserted + @TargetCount
    PRINT '✅ ' + @TableName + ': ' + CAST(@TargetCount AS NVARCHAR(2)) + ' indicators'
END

-- 23. PP KTNQ CNL2: 1 table × 5 indicators = 5 (already correct)
SELECT @TableId = Id, @TableName = TableName FROM KpiAssignmentTables
WHERE Category = 'CANBO' AND TableName LIKE '%PhophongKtnqCnl2%'

IF @TableId IS NOT NULL
BEGIN
    SET @TargetCount = 5
    SET @InsertedForTable = 0

    WHILE @InsertedForTable < @TargetCount
    BEGIN
        SET @InsertedForTable = @InsertedForTable + 1
        INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive)
        VALUES (
            @TableId,
            'Chỉ tiêu ' + CAST(@InsertedForTable AS NVARCHAR(2)) + ' - PP KTNQ CNL2',
            CASE @InsertedForTable
                WHEN 1 THEN 25.0
                WHEN 2 THEN 25.0
                WHEN 3 THEN 20.0
                WHEN 4 THEN 15.0
                ELSE 15.0
            END,
            '%',
            @InsertedForTable,
            2, -- PERCENTAGE
            1
        )
    END

    SET @TotalInserted = @TotalInserted + @TargetCount
    PRINT '✅ ' + @TableName + ': ' + CAST(@TargetCount AS NVARCHAR(2)) + ' indicators'
END

PRINT ''
PRINT '🎉 KPI Indicators correction completed!'
PRINT '📈 Total indicators created: ' + CAST(@TotalInserted AS NVARCHAR(10))

-- Final verification
PRINT ''
PRINT '📊 Final CANBO table indicator counts:'
SELECT
    t.TableName,
    LEFT(t.Description, 40) + '...' as Description,
    COUNT(i.Id) as FinalCount
FROM KpiAssignmentTables t
LEFT JOIN KpiIndicators i ON t.Id = i.TableId
WHERE t.Category = 'CANBO'
GROUP BY t.Id, t.TableName, t.Description
ORDER BY t.TableName

PRINT ''
PRINT '✅ Total CANBO indicators:'
SELECT COUNT(*) as TotalCANBOIndicators
FROM KpiIndicators i
INNER JOIN KpiAssignmentTables t ON i.TableId = t.Id
WHERE t.Category = 'CANBO'

PRINT ''
PRINT '🎯 Script execution completed successfully!'
