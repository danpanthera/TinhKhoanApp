-- 🚀 GL01 ULTRA OPTIMIZATION SQL SCRIPT
-- Tối ưu database cho import GL01 nhanh nhất có thể

USE TinhKhoanDB;
GO

-- 1. Disable constraints tạm thời để tăng tốc import
ALTER TABLE GL01 NOCHECK CONSTRAINT ALL;
GO

-- 2. Drop indexes tạm thời để tăng tốc insert
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GL01_NGAY_DL' AND object_id = OBJECT_ID('GL01'))
    DROP INDEX IX_GL01_NGAY_DL ON GL01;
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GL01_MA_KH' AND object_id = OBJECT_ID('GL01'))
    DROP INDEX IX_GL01_MA_KH ON GL01;
GO

-- 3. Set recovery model to simple để giảm log overhead
ALTER DATABASE TinhKhoanDB SET RECOVERY SIMPLE;
GO

-- 4. Increase tempdb auto-growth for better performance
ALTER DATABASE tempdb MODIFY FILE (NAME = 'tempdev', FILEGROWTH = 100MB);
GO

-- 5. Set compatibility level for parallel operations
ALTER DATABASE TinhKhoanDB SET COMPATIBILITY_LEVEL = 150;
GO

-- 6. Enable parallel insert operations
ALTER DATABASE TinhKhoanDB SET PARAMETERIZATION FORCED;
GO

PRINT '🚀 GL01 Database optimization completed!';
PRINT '⚡ Ready for ultra-fast import';
GO
