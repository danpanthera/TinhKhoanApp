/*
Idempotent script tạo Partitioned Columnstore Index cho GL41.
Nếu đã tồn tại, script sẽ giữ nguyên (không tạo mới). Có thể mở rộng phần partition scheme nếu cần.
*/

IF OBJECT_ID(N'[dbo].[GL41]', 'U') IS NULL
BEGIN
    RAISERROR('GL41 table does not exist', 10, 1);
    RETURN;
END

-- Bỏ qua trên Azure SQL Edge (không hỗ trợ Columnstore Index)
DECLARE @ver nvarchar(4000) = @@VERSION;
IF @ver LIKE '%Azure SQL Edge%'
BEGIN
    PRINT 'Skipping Columnstore index creation: Azure SQL Edge does not support Columnstore indexes.';
    RETURN;
END

-- Tên index cộtstore đề xuất
DECLARE @IndexName sysname = N'CCI_GL41';

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[GL41]') AND name = @IndexName
)
BEGIN
    -- Nếu bảng đã có clustered rowstore index (ví dụ PK clustered), tạo NONCLUSTERED COLUMNSTORE
    IF EXISTS (
        SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[GL41]') AND type_desc = 'CLUSTERED'
    )
    BEGIN
        IF NOT EXISTS (
            SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[GL41]') AND name = N'NCCI_GL41'
        )
        BEGIN
            PRINT 'Creating NONCLUSTERED COLUMNSTORE INDEX on GL41...';
            CREATE NONCLUSTERED COLUMNSTORE INDEX [NCCI_GL41] ON [dbo].[GL41];
        END
        ELSE
        BEGIN
            PRINT 'Nonclustered columnstore index already exists on GL41.';
        END
    END
    ELSE
    BEGIN
        PRINT 'Creating CLUSTERED COLUMNSTORE INDEX on GL41...';
        CREATE CLUSTERED COLUMNSTORE INDEX [CCI_GL41] ON [dbo].[GL41];
    END
END
ELSE
BEGIN
    PRINT 'Clustered columnstore index already exists on GL41.';
END
