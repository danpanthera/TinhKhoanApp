-- Fix place names script
USE TinhKhoanDB;

PRINT 'Starting place names fix...';

-- Fix "Chi nhánh Bình Lu" to "Chi nhánh Bình Lư"
UPDATE KpiAssignmentTables
SET Description = N'Chi nhánh Bình Lư'
WHERE Id = 31 AND Description = N'Chi nhánh Bình Lu';

PRINT 'Place names fix completed.';

-- Verify the fix
SELECT Id, Description, Category
FROM KpiAssignmentTables
WHERE Id = 31;
