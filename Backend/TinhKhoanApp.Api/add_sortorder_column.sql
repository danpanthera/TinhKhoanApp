-- Add SortOrder column to Units table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Units]') AND name = 'SortOrder')
BEGIN
    ALTER TABLE [Units] ADD [SortOrder] int NULL DEFAULT 0;
    PRINT 'SortOrder column added to Units table';
END
ELSE
BEGIN
    PRINT 'SortOrder column already exists in Units table';
END

-- Update existing records with default sort order
UPDATE [Units] SET [SortOrder] = 0 WHERE [SortOrder] IS NULL;

-- Optional: Set specific sort orders for known branches
UPDATE [Units] SET [SortOrder] = 1 WHERE [Code] = 'HO';
UPDATE [Units] SET [SortOrder] = 2 WHERE [Code] = 'PNVL1';
UPDATE [Units] SET [SortOrder] = 3 WHERE [Code] = 'PNVL2';
UPDATE [Units] SET [SortOrder] = 4 WHERE [Code] = 'PNDL1';
UPDATE [Units] SET [SortOrder] = 5 WHERE [Code] = 'PNLC1';
UPDATE [Units] SET [SortOrder] = 6 WHERE [Code] = 'PNHS1';
UPDATE [Units] SET [SortOrder] = 7 WHERE [Code] = 'PNMB1';
UPDATE [Units] SET [SortOrder] = 8 WHERE [Code] = 'PNCB1';
UPDATE [Units] SET [SortOrder] = 9 WHERE [Code] = 'PNVP1';
UPDATE [Units] SET [SortOrder] = 10 WHERE [Code] = 'PNBG1';

PRINT 'SortOrder values updated for existing Units';
