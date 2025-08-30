/*
GL41 Temporal Verification Script
Chạy an toàn sau khi áp dụng hotfix GL41TemporalHotfix.
*/

-- 1. Trạng thái temporal của GL41
SELECT t.name AS BaseTable,
       t.temporal_type,             -- 2 = SYSTEM_VERSIONING ON
       ht.name AS HistoryTable
FROM sys.tables t
LEFT JOIN sys.tables ht ON t.history_table_id = ht.object_id
WHERE t.name = 'GL41';

-- 2. Số lượng cột (bỏ cột period ẩn)
SELECT 'Base' AS TableName, COUNT(*) AS ColumnCount
FROM sys.columns WHERE object_id = OBJECT_ID('dbo.GL41') AND name NOT IN ('ValidFrom','ValidTo')
UNION ALL
SELECT 'History', COUNT(*)
FROM sys.columns WHERE object_id = OBJECT_ID('dbo.GL41_History') AND name NOT IN ('ValidFrom','ValidTo');

-- 3. So khớp thứ tự & tên cột
WITH B AS (
  SELECT ROW_NUMBER() OVER(ORDER BY column_id) rn, name
  FROM sys.columns
  WHERE object_id = OBJECT_ID('dbo.GL41') AND name NOT IN ('ValidFrom','ValidTo')
),
H AS (
  SELECT ROW_NUMBER() OVER(ORDER BY column_id) rn, name
  FROM sys.columns
  WHERE object_id = OBJECT_ID('dbo.GL41_History') AND name NOT IN ('ValidFrom','ValidTo')
)
SELECT B.rn,
       B.name AS BaseCol,
       H.name AS HistCol,
       CASE WHEN B.name = H.name THEN 'OK' ELSE 'MISMATCH' END AS Status
FROM B JOIN H ON B.rn = H.rn
ORDER BY B.rn;

-- 4. Cột đầu tiên phi-period
SELECT TOP 1 name AS FirstBaseCol FROM sys.columns WHERE object_id = OBJECT_ID('dbo.GL41') ORDER BY column_id;
SELECT TOP 1 name AS FirstHistCol FROM sys.columns WHERE object_id = OBJECT_ID('dbo.GL41_History') ORDER BY column_id;

-- 5. Test phát sinh bản ghi history (CHỈ chạy nếu muốn kiểm thử động)
-- LƯU Ý: Có thể tạo thêm 1 version mới.
-- DECLARE @TestId BIGINT = (SELECT TOP 1 Id FROM GL41 ORDER BY NEWID());
-- DECLARE @OldVal DECIMAL(18,2) = (SELECT ST_GHICO FROM GL41 WHERE Id=@TestId);
-- UPDATE GL41 SET ST_GHICO = ISNULL(ST_GHICO,0)+1 WHERE Id=@TestId;
-- SELECT TOP 5 * FROM GL41_History WHERE Id=@TestId ORDER BY ValidFrom DESC;
-- -- Khôi phục (tuỳ chọn):
-- UPDATE GL41 SET ST_GHICO = @OldVal WHERE Id=@TestId;

-- 6. Đảm bảo không có identity trong history
SELECT name, is_identity
FROM sys.columns
WHERE object_id = OBJECT_ID('dbo.GL41_History') AND is_identity=1;  -- Kết quả rỗng là OK

-- 7. Tổng hợp nhanh trạng thái so khớp
WITH B2 AS (
  SELECT ROW_NUMBER() OVER(ORDER BY column_id) rn, name
  FROM sys.columns WHERE object_id = OBJECT_ID('dbo.GL41') AND name NOT IN ('ValidFrom','ValidTo')
),
H2 AS (
  SELECT ROW_NUMBER() OVER(ORDER BY column_id) rn, name
  FROM sys.columns WHERE object_id = OBJECT_ID('dbo.GL41_History') AND name NOT IN ('ValidFrom','ValidTo')
)
SELECT CASE WHEN EXISTS (
  SELECT 1 FROM B2 JOIN H2 ON B2.rn=H2.rn WHERE B2.name<>H2.name)
  THEN 'Mismatch' ELSE 'OK' END AS ColumnOrderStatus;
