-- ===================================================================
-- KPI INDICATOR MIGRATION: Replace Interest Collection Rates with Customer Development
-- Date: June 21, 2025
-- Purpose: Replace "Tỷ lệ thực thu lãi" indicators with "Phát triển khách hàng mới"
-- ===================================================================

-- Backup existing KPI indicators before migration
CREATE TABLE KpiIndicators_Backup_20250621 AS 
SELECT * FROM KpiIndicators 
WHERE KpiIndicatorName LIKE '%Tỷ lệ thực thu lãi%';

-- Update all KPI Indicators: Replace "Tỷ lệ thực thu lãi KHDN" with "Phát triển khách hàng mới"
UPDATE KpiIndicators 
SET 
    KpiIndicatorName = 'Phát triển khách hàng mới',
    UnitOfMeasure = 'Khách hàng',
    ValueType = 'NUMBER'
WHERE KpiIndicatorName = 'Tỷ lệ thực thu lãi KHDN';

-- Update all KPI Indicators: Replace "Tỷ lệ thực thu lãi KHCN" with "Phát triển khách hàng mới"
UPDATE KpiIndicators 
SET 
    KpiIndicatorName = 'Phát triển khách hàng mới',
    UnitOfMeasure = 'Khách hàng',
    ValueType = 'NUMBER'
WHERE KpiIndicatorName = 'Tỷ lệ thực thu lãi KHCN';

-- Update all KPI Indicators: Replace "Tỷ lệ thực thu lãi" with "Phát triển khách hàng mới"
UPDATE KpiIndicators 
SET 
    KpiIndicatorName = 'Phát triển khách hàng mới',
    UnitOfMeasure = 'Khách hàng',
    ValueType = 'NUMBER'
WHERE KpiIndicatorName = 'Tỷ lệ thực thu lãi';

-- Update KpiDefinitions table if it exists
UPDATE KpiDefinitions 
SET 
    KpiName = 'Phát triển khách hàng mới',
    UnitOfMeasure = 'Khách hàng',
    ValueType = 'NUMBER'
WHERE KpiName LIKE '%Tỷ lệ thực thu lãi%';

-- Update any existing KPI scoring data to reflect the new unit of measure
-- Note: Actual values may need manual review as they change from percentages to counts
UPDATE UnitKpiScorings 
SET Notes = COALESCE(Notes, '') + ' [Migrated from Tỷ lệ thực thu lãi to Phát triển khách hàng mới on 2025-06-21]'
WHERE KpiIndicatorId IN (
    SELECT Id FROM KpiIndicators 
    WHERE KpiIndicatorName = 'Phát triển khách hàng mới'
);

-- Update any staff KPI scoring tables
-- This is a template - adjust table names based on actual schema
-- Example for branch tables:
UPDATE CnLaiChauKpiScorings 
SET Notes = COALESCE(Notes, '') + ' [Migrated from Tỷ lệ thực thu lãi to Phát triển khách hàng mới on 2025-06-21]'
WHERE KpiIndicatorName = 'Phát triển khách hàng mới';

UPDATE CnTamDuongKpiScorings 
SET Notes = COALESCE(Notes, '') + ' [Migrated from Tỷ lệ thực thu lãi to Phát triển khách hàng mới on 2025-06-21]'
WHERE KpiIndicatorName = 'Phát triển khách hàng mới';

UPDATE CnPhongThoKpiScorings 
SET Notes = COALESCE(Notes, '') + ' [Migrated from Tỷ lệ thực thu lãi to Phát triển khách hàng mới on 2025-06-21]'
WHERE KpiIndicatorName = 'Phát triển khách hàng mới';

UPDATE CnSinHoKpiScorings 
SET Notes = COALESCE(Notes, '') + ' [Migrated from Tỷ lệ thực thu lãi to Phát triển khách hàng mới on 2025-06-21]'
WHERE KpiIndicatorName = 'Phát triển khách hàng mới';

UPDATE CnMuongTeKpiScorings 
SET Notes = COALESCE(Notes, '') + ' [Migrated from Tỷ lệ thực thu lãi to Phát triển khách hàng mới on 2025-06-21]'
WHERE KpiIndicatorName = 'Phát triển khách hàng mới';

UPDATE CnThanUyenKpiScorings 
SET Notes = COALESCE(Notes, '') + ' [Migrated from Tỷ lệ thực thu lãi to Phát triển khách hàng mới on 2025-06-21]'
WHERE KpiIndicatorName = 'Phát triển khách hàng mới';

UPDATE CnThanhPhoKpiScorings 
SET Notes = COALESCE(Notes, '') + ' [Migrated from Tỷ lệ thực thu lãi to Phát triển khách hàng mới on 2025-06-21]'
WHERE KpiIndicatorName = 'Phát triển khách hàng mới';

UPDATE CnTanUyenKpiScorings 
SET Notes = COALESCE(Notes, '') + ' [Migrated from Tỷ lệ thực thu lãi to Phát triển khách hàng mới on 2025-06-21]'
WHERE KpiIndicatorName = 'Phát triển khách hàng mới';

UPDATE CnNamNhunKpiScorings 
SET Notes = COALESCE(Notes, '') + ' [Migrated from Tỷ lệ thực thu lãi to Phát triển khách hàng mới on 2025-06-21]'
WHERE KpiIndicatorName = 'Phát triển khách hàng mới';

-- Update staff KPI tables (assuming similar pattern)
-- Add updates for staff tables like PNVL1KpiScorings, etc. as needed

-- Verification queries
SELECT 'KpiIndicators Updated' as Status, COUNT(*) as Count 
FROM KpiIndicators 
WHERE KpiIndicatorName = 'Phát triển khách hàng mới';

SELECT 'Old Indicators Remaining' as Status, COUNT(*) as Count 
FROM KpiIndicators 
WHERE KpiIndicatorName LIKE '%Tỷ lệ thực thu lãi%';

-- Log the migration
INSERT INTO MigrationLog (MigrationDate, Description, Status)
VALUES (CURRENT_TIMESTAMP, 'KPI Indicator Migration: Replaced Tỷ lệ thực thu lãi with Phát triển khách hàng mới', 'COMPLETED');

COMMIT;
