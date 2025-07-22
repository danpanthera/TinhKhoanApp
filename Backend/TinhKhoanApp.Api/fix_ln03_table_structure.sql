-- ================================================================
-- FIX LN03 TABLE: Thêm các columns bị thiếu từ CSV LN03 gốc
-- ================================================================

USE [TinhKhoanDB]
GO

-- Drop existing LN03 table và history
IF OBJECT_ID('dbo.LN03', 'U') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[LN03] SET (SYSTEM_VERSIONING = OFF);
    DROP TABLE IF EXISTS [dbo].[LN03_History];
    DROP TABLE IF EXISTS [dbo].[LN03];
    PRINT '🗑️ Dropped existing LN03 table and history.';
END
GO

-- Create LN03 với ĐÚNG 17 columns từ CSV
CREATE TABLE [dbo].[LN03] (
    -- NGAY_DL column FIRST (system column)
    NGAY_DL DATETIME NOT NULL,

    -- 17 Business columns từ CSV LN03 (exact order from file 7800_ln03_20241231_fixed.csv)
    MACHINHANH NVARCHAR(50),
    TENCHINHANH NVARCHAR(255),
    MAKH NVARCHAR(100),
    TENKH NVARCHAR(500),
    SOHOPDONG NVARCHAR(100),          -- BỊ THIẾU trong database cũ
    SOTIENXLRR DECIMAL(18,2),
    NGAYPHATSINHXL DATETIME,          -- BỊ THIẾU trong database cũ
    THUNOSAUXL DECIMAL(18,2),
    CONLAINGOAIBANG DECIMAL(18,2),
    DUNONOIBANG DECIMAL(18,2),
    NHOMNO INT,
    MACBTD NVARCHAR(50),
    TENCBTD NVARCHAR(255),
    MAPGD NVARCHAR(50),
    TAIKHOANHACHTOAN NVARCHAR(100),
    REFNO NVARCHAR(100),
    LOAINGUONVON NVARCHAR(100),

    -- System/Temporal columns LAST
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    FILE_NAME NVARCHAR(255),
    CREATED_DATE DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
    UPDATED_DATE DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (CREATED_DATE, UPDATED_DATE)
) WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.LN03_History));

PRINT '✅ LN03 recreated with EXACT 17 columns from CSV gốc.';
PRINT '🔧 Added missing columns: SOHOPDONG, NGAYPHATSINHXL';
PRINT '📊 Total columns: 1 NGAY_DL + 17 CSV business + 4 system/temporal = 22 columns';
GO
