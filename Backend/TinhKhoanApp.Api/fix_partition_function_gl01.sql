-- 🔧 FIX PARTITION FUNCTION FOR GL01 - DATE TYPE
-- Sửa partition function để phù hợp với DATE type
-- Created: 2025-07-19

SET QUOTED_IDENTIFIER ON;
GO

PRINT '🔧 FIXING PARTITION FUNCTION FOR GL01...';
PRINT '======================================';

-- Drop existing partition scheme and function if exists
IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'PS_GL01_Date')
BEGIN
    DROP PARTITION SCHEME PS_GL01_Date;
    PRINT '   - Dropped existing partition scheme';
END

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'PF_GL01_Date')
BEGIN
    DROP PARTITION FUNCTION PF_GL01_Date;
    PRINT '   - Dropped existing partition function';
END

-- Create partition function with DATE type (not DATETIME2)
CREATE PARTITION FUNCTION PF_GL01_Date (DATE)
AS RANGE RIGHT FOR VALUES (
    '2024-01-01', '2024-02-01', '2024-03-01', '2024-04-01',
    '2024-05-01', '2024-06-01', '2024-07-01', '2024-08-01',
    '2024-09-01', '2024-10-01', '2024-11-01', '2024-12-01',
    '2025-01-01', '2025-02-01', '2025-03-01', '2025-04-01',
    '2025-05-01', '2025-06-01', '2025-07-01', '2025-08-01',
    '2025-09-01', '2025-10-01', '2025-11-01', '2025-12-01'
);

PRINT '✅ Partition function created with DATE type';

-- Create partition scheme
CREATE PARTITION SCHEME PS_GL01_Date
AS PARTITION PF_GL01_Date
ALL TO ([PRIMARY]);

PRINT '✅ Partition scheme created';
PRINT '';
PRINT '🎯 Ready to recreate GL01 table!';
GO
