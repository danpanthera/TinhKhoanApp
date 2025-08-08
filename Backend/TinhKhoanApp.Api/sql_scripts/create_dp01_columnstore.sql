-- Script để tạo Columnstore Index cho các bảng DP01
-- Tạo ngày: 08/08/2025

-- Xóa index nếu đã tồn tại để tránh lỗi
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DP01_Columnstore' AND object_id = OBJECT_ID('dbo.DP01'))
BEGIN
    DROP INDEX IX_DP01_Columnstore ON dbo.DP01;
    PRINT 'Đã xóa index IX_DP01_Columnstore cũ';
END

IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_DP01_History_Columnstore' AND object_id = OBJECT_ID('dbo.DP01_History'))
BEGIN
    DROP INDEX IX_DP01_History_Columnstore ON dbo.DP01_History;
    PRINT 'Đã xóa index IX_DP01_History_Columnstore cũ';
END

-- Tạo nonclustered columnstore index cho bảng chính
CREATE NONCLUSTERED COLUMNSTORE INDEX IX_DP01_Columnstore
ON dbo.DP01 (NGAY_DL, MA_CN, MA_KH, TAI_KHOAN_HACH_TOAN)
WITH (DROP_EXISTING = OFF);
PRINT 'Đã tạo NONCLUSTERED COLUMNSTORE INDEX cho DP01';-- Tạo clustered columnstore index cho bảng lịch sử
CREATE CLUSTERED COLUMNSTORE INDEX IX_DP01_History_Columnstore
ON dbo.DP01_History
WITH (DROP_EXISTING = ON);
PRINT 'Đã tạo CLUSTERED COLUMNSTORE INDEX cho DP01_History';

-- Hiển thị thông tin về index đã tạo
SELECT
    t.name AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType
FROM
    sys.indexes i
JOIN
    sys.tables t ON i.object_id = t.object_id
WHERE
    t.name IN ('DP01', 'DP01_History')
    AND i.name LIKE '%columnstore%';
