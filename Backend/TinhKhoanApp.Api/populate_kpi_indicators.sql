-- Populate KpiIndicators from KPIDefinitions to KpiAssignmentTables
-- This script creates KpiIndicators records that link KPIDefinitions to specific assignment tables

PRINT '🚀 Starting KPI Indicators population from KPIDefinitions...'

-- First, let's see what we have
PRINT '📊 Current data summary:'
SELECT 'KPIDefinitions' as TableName, COUNT(*) as RecordCount FROM KPIDefinitions
UNION ALL
SELECT 'KpiAssignmentTables' as TableName, COUNT(*) as RecordCount FROM KpiAssignmentTables
UNION ALL
SELECT 'KpiIndicators' as TableName, COUNT(*) as RecordCount FROM KpiIndicators

PRINT ''
PRINT '🏢 Available KPI Assignment Tables:'
SELECT Id, TableName, Description, Category FROM KpiAssignmentTables ORDER BY Category, TableName

PRINT ''
PRINT '📋 Sample KPIDefinitions:'
SELECT TOP 5 Id, KpiCode, KpiName, MaxScore, ValueType FROM KPIDefinitions

-- Now populate KpiIndicators based on pattern matching between KPIDefinitions and KpiAssignmentTables
PRINT ''
PRINT '🔄 Populating KpiIndicators...'

-- Strategy: Match KPIDefinitions to tables based on the KpiCode pattern
-- For example: "TruongphongKhdn_01" should go to table with TableName containing "TruongphongKhdn"

-- Clear existing data first
DELETE FROM KpiIndicators;
PRINT '🧹 Cleared existing KpiIndicators'

DECLARE @InsertedCount INT = 0;

-- Create a cursor to process each KPIDefinition
DECLARE kpi_cursor CURSOR FOR
SELECT
    d.Id as DefinitionId,
    d.KpiCode,
    d.KpiName,
    d.MaxScore,
    d.ValueType,
    d.UnitOfMeasure,
    d.IsActive
FROM KPIDefinitions d
WHERE d.IsActive = 1;

DECLARE @DefinitionId INT, @KpiCode NVARCHAR(100), @KpiName NVARCHAR(500), @MaxScore DECIMAL(10,2),
        @ValueType INT, @UnitOfMeasure NVARCHAR(50), @IsActive BIT;

OPEN kpi_cursor;
FETCH NEXT FROM kpi_cursor INTO @DefinitionId, @KpiCode, @KpiName, @MaxScore, @ValueType, @UnitOfMeasure, @IsActive;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Extract the role/table identifier from KpiCode (before the underscore and number)
    DECLARE @RolePattern NVARCHAR(100);
    DECLARE @UnderscorePos INT = CHARINDEX('_', @KpiCode);

    IF @UnderscorePos > 0
    BEGIN
        SET @RolePattern = LEFT(@KpiCode, @UnderscorePos - 1);

        -- Find matching assignment table
        DECLARE @TableId INT = NULL;

        -- Direct match first
        SELECT @TableId = Id
        FROM KpiAssignmentTables
        WHERE TableName LIKE '%' + @RolePattern + '%'
           OR Description LIKE '%' + @RolePattern + '%';

        -- If no direct match, try some specific mappings for common patterns
        IF @TableId IS NULL
        BEGIN
            -- Handle special cases where naming might not match exactly
            IF @RolePattern LIKE '%Cnl%' OR @RolePattern LIKE '%CNL%'
                SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName LIKE '%' + REPLACE(@RolePattern, 'Cnl', 'CNL') + '%';

            IF @TableId IS NULL AND @RolePattern LIKE '%Pgd%'
                SELECT @TableId = Id FROM KpiAssignmentTables WHERE TableName LIKE '%Pgd%' OR Description LIKE '%phòng giao dịch%';
        END

        -- If we found a matching table, create the indicator
        IF @TableId IS NOT NULL
        BEGIN
            -- Get next order index for this table
            DECLARE @NextOrderIndex INT;
            SELECT @NextOrderIndex = ISNULL(MAX(OrderIndex), 0) + 1
            FROM KpiIndicators
            WHERE TableId = @TableId;

            -- Insert the indicator
            INSERT INTO KpiIndicators (
                TableId,
                IndicatorName,
                MaxScore,
                Unit,
                OrderIndex,
                ValueType,
                IsActive
            ) VALUES (
                @TableId,
                @KpiName,
                @MaxScore,
                @UnitOfMeasure,
                @NextOrderIndex,
                @ValueType,
                @IsActive
            );

            SET @InsertedCount = @InsertedCount + 1;

            IF @InsertedCount % 20 = 0
                PRINT '✅ Processed ' + CAST(@InsertedCount AS NVARCHAR(10)) + ' indicators...';
        END
        ELSE
        BEGIN
            PRINT '⚠️  No matching table found for KpiCode: ' + @KpiCode + ' (Pattern: ' + @RolePattern + ')';
        END
    END
    ELSE
    BEGIN
        PRINT '⚠️  Invalid KpiCode format (no underscore): ' + @KpiCode;
    END

    FETCH NEXT FROM kpi_cursor INTO @DefinitionId, @KpiCode, @KpiName, @MaxScore, @ValueType, @UnitOfMeasure, @IsActive;
END

CLOSE kpi_cursor;
DEALLOCATE kpi_cursor;

PRINT ''
PRINT '✅ KPI Indicators population completed!'
PRINT '📈 Total indicators created: ' + CAST(@InsertedCount AS NVARCHAR(10))

-- Final summary
PRINT ''
PRINT '📊 Final data summary:'
SELECT 'KPIDefinitions' as TableName, COUNT(*) as RecordCount FROM KPIDefinitions
UNION ALL
SELECT 'KpiAssignmentTables' as TableName, COUNT(*) as RecordCount FROM KpiAssignmentTables
UNION ALL
SELECT 'KpiIndicators' as TableName, COUNT(*) as RecordCount FROM KpiIndicators

PRINT ''
PRINT '🎯 KpiIndicators per table:'
SELECT
    t.TableName,
    t.Description,
    t.Category,
    COUNT(i.Id) as IndicatorCount
FROM KpiAssignmentTables t
LEFT JOIN KpiIndicators i ON t.Id = i.TableId
GROUP BY t.Id, t.TableName, t.Description, t.Category
ORDER BY t.Category, t.TableName

PRINT ''
PRINT '🎉 Script execution completed successfully!'
