#!/bin/bash

# Script kiểm tra tính nhất quán giữa frontend và backend database
# Created by GitHub Copilot - July 16, 2025

echo "🔍 KIỂM TRA TÍNH NHẤT QUÁN DỮ LIỆU FRONTEND-BACKEND"
echo "=================================================="

# Kiểm tra database connection
echo "📡 Kiểm tra kết nối database..."
DB_CHECK=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "SELECT 1" 2>/dev/null)
if [ $? -ne 0 ]; then
    echo "❌ Không thể kết nối database"
    exit 1
fi
echo "✅ Database connection OK"

# Kiểm tra backend API
echo "📡 Kiểm tra backend API..."
API_CHECK=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5055/health)
if [ "$API_CHECK" != "200" ]; then
    echo "❌ Backend API không hoạt động (HTTP $API_CHECK)"
    exit 1
fi
echo "✅ Backend API OK"

echo ""
echo "📊 KIỂM TRA TÍNH NHẤT QUÁN DỮ LIỆU:"
echo "=================================="

# Tạo báo cáo tính nhất quán
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "
WITH TableCounts AS (
    SELECT 'DP01' as TableName, COUNT(*) as ActualRecords FROM DP01
    UNION ALL SELECT 'DPDA', COUNT(*) FROM DPDA
    UNION ALL SELECT 'EI01', COUNT(*) FROM EI01
    UNION ALL SELECT 'GL01', COUNT(*) FROM GL01
    UNION ALL SELECT 'GL41', COUNT(*) FROM GL41
    UNION ALL SELECT 'LN01', COUNT(*) FROM LN01
    UNION ALL SELECT 'LN03', COUNT(*) FROM LN03
    UNION ALL SELECT 'RR01', COUNT(*) FROM RR01
),
ImportCounts AS (
    SELECT Category as TableName, ISNULL(SUM(RecordsCount), 0) as ImportRecords
    FROM ImportedDataRecords
    GROUP BY Category
)
SELECT
    tc.TableName as [Bảng],
    tc.ActualRecords as [DB Records],
    ISNULL(ic.ImportRecords, 0) as [Import Records],
    tc.ActualRecords - ISNULL(ic.ImportRecords, 0) as [Chênh lệch],
    CASE
        WHEN tc.ActualRecords = ISNULL(ic.ImportRecords, 0) THEN '✅ KHỚP'
        WHEN ic.ImportRecords IS NULL OR ic.ImportRecords = 0 THEN '⚠️ TRỐNG'
        WHEN tc.ActualRecords > ISNULL(ic.ImportRecords, 0) THEN '❌ DB THỪA'
        ELSE '❌ IMPORT THỪA'
    END as [Trạng thái]
FROM TableCounts tc
LEFT JOIN ImportCounts ic ON tc.TableName = ic.TableName
ORDER BY tc.TableName;
"

echo ""
echo "🧹 KIỂM TRA DỮ LIỆU ORPHAN:"
echo "========================="

sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "
SELECT
    'DP01' as [Bảng],
    SUM(CASE WHEN FILE_NAME IS NULL THEN 1 ELSE 0 END) as [Records NULL FILE_NAME],
    SUM(CASE WHEN NGAY_DL = '0001-01-01' THEN 1 ELSE 0 END) as [Records Invalid NGAY_DL]
FROM DP01
UNION ALL
SELECT 'DPDA',
    SUM(CASE WHEN FILE_NAME IS NULL THEN 1 ELSE 0 END),
    SUM(CASE WHEN NGAY_DL = '0001-01-01' THEN 1 ELSE 0 END)
FROM DPDA
UNION ALL
SELECT 'EI01',
    SUM(CASE WHEN FILE_NAME IS NULL THEN 1 ELSE 0 END),
    SUM(CASE WHEN NGAY_DL = '0001-01-01' THEN 1 ELSE 0 END)
FROM EI01;
"

echo ""
echo "📊 TỔNG KẾT:"
echo "==========="

# Đếm số bảng khớp
MATCH_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "
WITH TableCounts AS (
    SELECT 'DP01' as TableName, COUNT(*) as ActualRecords FROM DP01
    UNION ALL SELECT 'DPDA', COUNT(*) FROM DPDA
    UNION ALL SELECT 'EI01', COUNT(*) FROM EI01
    UNION ALL SELECT 'GL01', COUNT(*) FROM GL01
    UNION ALL SELECT 'GL41', COUNT(*) FROM GL41
    UNION ALL SELECT 'LN01', COUNT(*) FROM LN01
    UNION ALL SELECT 'LN03', COUNT(*) FROM LN03
    UNION ALL SELECT 'RR01', COUNT(*) FROM RR01
),
ImportCounts AS (
    SELECT Category as TableName, ISNULL(SUM(RecordsCount), 0) as ImportRecords
    FROM ImportedDataRecords
    GROUP BY Category
)
SELECT COUNT(*) as MatchCount
FROM TableCounts tc
LEFT JOIN ImportCounts ic ON tc.TableName = ic.TableName
WHERE tc.ActualRecords = ISNULL(ic.ImportRecords, 0);
" -h-1 | tr -d ' ')

echo "✅ Số bảng khớp: $MATCH_COUNT/8"

if [ "$MATCH_COUNT" = "8" ]; then
    echo "🎉 TẤT CẢ BẢNG KHỚP HOÀN HẢO!"
    echo "✅ Frontend và Backend đã đồng bộ 100%"
else
    echo "⚠️ CÓ $((8-MATCH_COUNT)) BẢNG CHƯA KHỚP"
    echo "💡 Khuyến nghị: Sử dụng delete function trên frontend để đồng bộ"
fi

echo ""
echo "🔧 CÁCH SỬ DỤNG:"
echo "==============="
echo "1. Nếu có bảng 'DB THỪA': Click delete trên frontend để xóa data thừa"
echo "2. Nếu có bảng 'IMPORT THỪA': Re-import file CSV để đồng bộ dữ liệu"
echo "3. Nếu có bảng 'TRỐNG': Import file CSV mới hoặc bỏ qua nếu không cần"
echo ""
echo "✅ Kiểm tra hoàn tất!"
