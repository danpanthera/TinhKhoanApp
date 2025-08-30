/*
GL41 Temporal Hotfix Runner
Mục tiêu:
- Tắt SYSTEM_VERSIONING nếu đang bật
- Rebuild GL41_History theo đúng hình dạng & thứ tự cột của GL41 (đặt NGAY_DL lên đầu nếu có)
- Bổ sung cột period cho history (ValidFrom/ValidTo) nếu thiếu
- Bật lại SYSTEM_VERSIONING tham chiếu đúng GL41_History

Idempotent & an toàn chạy nhiều lần.
*/

-- Bước 1: Tắt SYSTEM_VERSIONING nếu đang bật
IF OBJECT_ID(N'dbo.GL41','U') IS NOT NULL AND OBJECTPROPERTY(OBJECT_ID(N'dbo.GL41'),'TableTemporalType') = 2
    ALTER TABLE dbo.GL41 SET (SYSTEM_VERSIONING = OFF);

-- Bước 2: Kiểm tra lệch số cột/thứ tự/thiếu cột và rebuild GL41_History khi cần
IF OBJECT_ID(N'dbo.GL41','U') IS NOT NULL AND OBJECT_ID(N'dbo.GL41_History','U') IS NOT NULL
BEGIN
    DECLARE @firstBase sysname = (SELECT TOP 1 name FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.GL41') ORDER BY column_id);
    DECLARE @firstHist sysname = (SELECT TOP 1 name FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.GL41_History') ORDER BY column_id);
    DECLARE @cntBase int = (SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.GL41') AND name NOT IN ('ValidFrom','ValidTo'));
    DECLARE @cntHist int = (SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.GL41_History') AND name NOT IN ('ValidFrom','ValidTo'));
    DECLARE @diffMissing int = (
        SELECT COUNT(*) FROM sys.columns b
        WHERE b.object_id = OBJECT_ID(N'dbo.GL41') AND b.name NOT IN ('ValidFrom','ValidTo')
          AND NOT EXISTS (
              SELECT 1 FROM sys.columns h
              WHERE h.object_id = OBJECT_ID(N'dbo.GL41_History') AND h.name = b.name)
    );

    DECLARE @needRebuild bit = CASE WHEN @cntBase <> @cntHist OR @firstBase <> @firstHist OR @diffMissing > 0 THEN 1 ELSE 0 END;

    IF @needRebuild = 1
    BEGIN
        -- Xây danh sách cột theo thứ tự: NGAY_DL trước, sau đó các cột còn lại (bỏ period)
        DECLARE @colsRest NVARCHAR(MAX) = (
            SELECT STRING_AGG(QUOTENAME(c.name), ',') WITHIN GROUP (ORDER BY c.column_id)
            FROM sys.columns c
            WHERE c.object_id = OBJECT_ID(N'dbo.GL41')
              AND c.name NOT IN ('ValidFrom','ValidTo','NGAY_DL')
        );

        DECLARE @cols NVARCHAR(MAX) = CASE WHEN EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.GL41') AND name='NGAY_DL')
            THEN QUOTENAME('NGAY_DL') + CASE WHEN @colsRest IS NOT NULL AND LEN(@colsRest)>0 THEN ',' + @colsRest ELSE '' END
            ELSE @colsRest END;

        DECLARE @sql NVARCHAR(MAX) = N'SELECT TOP 0 ' + @cols + ' INTO dbo.GL41_History_Rebuild FROM dbo.GL41;';
        EXEC(@sql);

        -- Di chuyển dữ liệu giao cắt cột từ history cũ sang bảng rebuild (nếu có dữ liệu)
        DECLARE @insertCols NVARCHAR(MAX) = (
            SELECT STRING_AGG(QUOTENAME(c.name), ',') WITHIN GROUP (ORDER BY c.column_id)
            FROM sys.columns c
            WHERE c.object_id = OBJECT_ID(N'dbo.GL41_History_Rebuild')
              AND EXISTS (
                  SELECT 1 FROM sys.columns h
                  WHERE h.object_id = OBJECT_ID(N'dbo.GL41_History')
                    AND h.name = c.name)
        );

        IF (@insertCols IS NOT NULL AND LEN(@insertCols) > 0 AND EXISTS (SELECT 1 FROM dbo.GL41_History))
        BEGIN
            DECLARE @ins NVARCHAR(MAX) = N'INSERT INTO dbo.GL41_History_Rebuild (' + @insertCols + ') SELECT ' + @insertCols + ' FROM dbo.GL41_History WITH (HOLDLOCK TABLOCKX);';
            EXEC(@ins);
        END

        DROP TABLE dbo.GL41_History;
        EXEC sp_rename 'dbo.GL41_History_Rebuild','GL41_History';

        -- Đảm bảo cột period tồn tại trên history
        IF COL_LENGTH('GL41_History','ValidFrom') IS NULL ALTER TABLE dbo.GL41_History ADD [ValidFrom] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00';
        IF COL_LENGTH('GL41_History','ValidTo') IS NULL ALTER TABLE dbo.GL41_History ADD [ValidTo] datetime2 NOT NULL DEFAULT '9999-12-31T23:59:59.9999999';
    END
END

-- Bước 3: Bật lại SYSTEM_VERSIONING nếu đang tắt
IF OBJECT_ID(N'dbo.GL41','U') IS NOT NULL
AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.GL41') AND name='ValidFrom')
AND OBJECTPROPERTY(OBJECT_ID(N'dbo.GL41'),'TableTemporalType') <> 2
BEGIN
    DECLARE @historyTableSchema sysname = SCHEMA_NAME();
    EXEC(N'ALTER TABLE [dbo].[GL41] SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [' + @historyTableSchema + '].[GL41_History]))');
END
