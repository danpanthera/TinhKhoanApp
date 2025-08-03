#!/bin/bash
# verify_gl01_structure_complete.sh
echo "🔍 KIỂM TRA TOÀN DIỆN CẤU TRÚC GL01..."

echo "=================================================="
echo "1. KIỂM TRA DATABASE SCHEMA GL01"
echo "=================================================="

docker exec azure_sql_edge_tinhkhoan /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Dientoan@303" -d TinhKhoanDB -C -Q "
SELECT
    COLUMN_NAME,
    ORDINAL_POSITION,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'GL01'
ORDER BY ORDINAL_POSITION;
"

echo "=================================================="
echo "2. KIỂM TRA PARTITIONED COLUMNSTORE INDEX"
echo "=================================================="

docker exec azure_sql_edge_tinhkhoan /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Dientoan@303" -d TinhKhoanDB -C -Q "
SELECT
    i.name AS IndexName,
    i.type_desc AS IndexType,
    CASE WHEN i.type = 5 THEN 'CLUSTERED COLUMNSTORE'
         WHEN i.type = 6 THEN 'NONCLUSTERED COLUMNSTORE'
         ELSE i.type_desc END AS ColumnstoreType,
    p.partition_number,
    p.rows
FROM sys.indexes i
JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
WHERE i.object_id = OBJECT_ID('GL01')
ORDER BY i.index_id, p.partition_number;
"

echo "=================================================="
echo "3. KIỂM TRA BUSINESS COLUMNS COUNT"
echo "=================================================="

docker exec azure_sql_edge_tinhkhoan /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Dientoan@303" -d TinhKhoanDB -C -Q "
SELECT
    COUNT(*) as TotalColumns,
    COUNT(CASE WHEN COLUMN_NAME NOT IN ('Id', 'NGAY_DL', 'CREATED_DATE', 'UPDATED_DATE', 'FILE_NAME') THEN 1 END) as BusinessColumns,
    COUNT(CASE WHEN COLUMN_NAME IN ('Id', 'NGAY_DL', 'CREATED_DATE', 'UPDATED_DATE', 'FILE_NAME') THEN 1 END) as SystemColumns
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'GL01';
"

echo "=================================================="
echo "4. KIỂM TRA COLUMN ORDER: NGAY_DL → Business → System"
echo "=================================================="

docker exec azure_sql_edge_tinhkhoan /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Dientoan@303" -d TinhKhoanDB -C -Q "
SELECT
    ORDINAL_POSITION,
    COLUMN_NAME,
    CASE
        WHEN COLUMN_NAME = 'NGAY_DL' THEN 'DATETIME_KEY'
        WHEN COLUMN_NAME IN ('Id', 'CREATED_DATE', 'UPDATED_DATE', 'FILE_NAME') THEN 'SYSTEM'
        ELSE 'BUSINESS'
    END as ColumnType,
    DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'GL01'
ORDER BY ORDINAL_POSITION;
"

echo "=================================================="
echo "5. KIỂM TRA CSV STRUCTURE VS DATABASE"
echo "=================================================="

# Kiểm tra file CSV mẫu GL01
CSV_FILE="7808_gl01_2025030120250331.csv"
if [ -f "$CSV_FILE" ]; then
    echo "✅ Tìm thấy file CSV mẫu: $CSV_FILE"
    echo "📊 CSV Headers (27 business columns):"
    head -1 "$CSV_FILE" | tr ',' '\n' | nl

    echo "📊 CSV Total columns:"
    head -1 "$CSV_FILE" | tr ',' '\n' | wc -l
else
    echo "❌ Không tìm thấy file CSV mẫu GL01 trong thư mục hiện tại"
    echo "📁 Đang tìm file GL01 khác..."
    find . -name "*gl01*.csv" -type f | head -3
fi

echo "=================================================="
echo "6. KIỂM TRA DIRECT IMPORT ENDPOINT"
echo "=================================================="

# Test API endpoint có hoạt động không
echo "🌐 Testing API endpoint availability..."
curl -s -o /dev/null -w "API Status: %{http_code}\n" "http://localhost:5055/api/health" || echo "❌ API không khả dụng"

echo "=================================================="
echo "7. KIỂM TRA GL01 MODEL STRUCTURE"
echo "=================================================="

# Kiểm tra Model file
MODEL_FILE="Models/DataTables/GL01.cs"
if [ -f "$MODEL_FILE" ]; then
    echo "✅ Tìm thấy GL01 Model: $MODEL_FILE"
    echo "📊 Business columns trong Model:"
    grep -E "public.*\{.*get.*set.*\}" "$MODEL_FILE" | grep -v "Id\|NGAY_DL\|CREATED_DATE\|UPDATED_DATE\|FILE_NAME" | wc -l

    echo "📊 NGAY_DL column definition:"
    grep -A2 -B2 "NGAY_DL" "$MODEL_FILE"
else
    echo "❌ Không tìm thấy GL01 Model"
fi

echo "=================================================="
echo "8. KIỂM TRA OPTIMIZATION ENDPOINT PERFORMANCE"
echo "=================================================="

# Test với file nhỏ để kiểm tra performance
TEST_FILE="gl01_test_1000_records.csv"
if [ -f "$TEST_FILE" ]; then
    echo "🚀 Testing optimized-gl01 performance với 1000 records..."
    time curl -X POST http://localhost:5055/api/directimport/optimized-gl01 \
        -F "file=@$TEST_FILE" \
        --max-time 30 \
        -w "\n⏱️  API Response time: %{time_total}s\n"
else
    echo "⚠️  Test file không có sẵn, skip performance test"
fi

echo "=================================================="
echo "9. SUMMARY CHECK RESULTS"
echo "=================================================="

echo "✅ REQUIREMENTS CHECKLIST:"
echo "□ Partitioned Columnstore (không Temporal)"
echo "□ 27 Business columns từ CSV GL01"
echo "□ NGAY_DL từ TR_TIME column (index 24)"
echo "□ Thứ tự: NGAY_DL → Business → System"
echo "□ Direct Import với optimized-gl01 endpoint"
echo "□ Filename validation: chứa 'gl01'"
echo "□ Performance: >30K records/second"

echo "=================================================="
echo "HOÀN THÀNH KIỂM TRA GL01 STRUCTURE"
echo "=================================================="
