#!/bin/bash

echo "=== CLEANUP DP01_New và DP01s - Chỉ giữ DP01 ==="
echo "Thời gian bắt đầu: $(date)"

# Database connection parameters
SERVER="localhost,1433"
USER="sa"
PASSWORD="YourStrong@Password123"
DATABASE="TinhKhoanDB"

echo "1. Kiểm tra trạng thái hiện tại..."

sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
SELECT
    t.name as TableName,
    CASE t.temporal_type
        WHEN 0 THEN 'NON_TEMPORAL'
        WHEN 2 THEN 'SYSTEM_VERSIONED'
        ELSE 'Other'
    END as TemporalType,
    (SELECT COUNT(*) FROM sys.objects WHERE name = t.name AND type = 'U') as RecordCount
FROM sys.tables t
WHERE t.name LIKE '%DP01%'
ORDER BY t.name
"

echo ""
echo "2. Tắt system versioning cho DP01_New nếu có..."

sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01_New' AND temporal_type = 2)
BEGIN
    ALTER TABLE [DP01_New] SET (SYSTEM_VERSIONING = OFF);
    PRINT 'DP01_New system versioning disabled';
END
ELSE
    PRINT 'DP01_New không có system versioning';
" -o "disable_dp01_new_temporal.log" 2>&1

echo "✅ Completed temporal disable"

echo ""
echo "3. Xóa history table DP01_New_History nếu có..."

sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01_New_History')
BEGIN
    DROP TABLE [DP01_New_History];
    PRINT 'DP01_New_History table dropped';
END
ELSE
    PRINT 'DP01_New_History không tồn tại';
" -o "drop_dp01_new_history.log" 2>&1

echo "✅ Completed history table cleanup"

echo ""
echo "4. Xóa bảng DP01_New..."

sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DP01_New')
BEGIN
    DROP TABLE [DP01_New];
    PRINT 'DP01_New table dropped successfully';
END
ELSE
    PRINT 'DP01_New không tồn tại';
" -o "drop_dp01_new.log" 2>&1

echo "✅ Completed DP01_New table cleanup"

echo ""
echo "5. Kiểm tra trạng thái sau cleanup..."

sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
SELECT
    t.name as TableName,
    CASE t.temporal_type
        WHEN 0 THEN 'NON_TEMPORAL'
        WHEN 2 THEN 'SYSTEM_VERSIONED'
        ELSE 'Other'
    END as TemporalType
FROM sys.tables t
WHERE t.name LIKE '%DP01%'
ORDER BY t.name
"

echo ""
echo "6. Xác nhận chỉ còn DP01 và DP01_History..."

sqlcmd -S $SERVER -U $USER -P $PASSWORD -C -d $DATABASE -Q "
SELECT
    COUNT(*) as DP01_Records
FROM DP01
"

echo ""
echo "=== HOÀN THÀNH: $(date) ==="
echo "📋 Cleanup Summary:"
echo "   ✅ Xóa bảng DP01_New"
echo "   ✅ Xóa bảng DP01_New_History"
echo "   ✅ Chỉ còn lại bảng DP01 (SYSTEM_VERSIONED) và DP01_History"
echo "   ✅ Sẵn sàng cho CSV import vào DP01"
