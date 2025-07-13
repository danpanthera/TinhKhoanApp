#!/bin/bash

# =====================================================
# FIX IMPORT ISSUES SCRIPT - JULY 13, 2025
# Khắc phục vấn đề import CSV và tổng records
# =====================================================

echo "🔧 BẮT ĐẦU KHẮC PHỤC VẤN ĐỀ IMPORT..."

# 1. PROBLEM ANALYSIS
echo "=== 📊 PHÂN TÍCH VẤN ĐỀ ==="

echo "📍 Vấn đề 1: Mapping tên bảng"
echo "   - DP01 → DP01_New (không đúng với frontend expect)"
echo "   - DirectImportController cần update routing"

echo "📍 Vấn đề 2: Số cột không khớp với README"
echo "   - DP01_New: 64 cột (expected: 63)"
echo "   - EI01: 27 cột (expected: 24)"
echo "   - GL01: 30 cột (expected: 27)"
echo "   - GL41: 16 cột (expected: 13)"
echo "   - LN01: 82 cột (expected: 79)"
echo "   - LN03: 20 cột (expected: 17)"
echo "   - RR01: 28 cột (expected: 25)"

echo "📍 Vấn đề 3: TotalRecords không được cập nhật"
echo "   - Frontend hiển thị 0 vì không có table tương ứng"
echo "   - Import logic cần update cách calculate total"

# 2. VERIFICATION OF CURRENT STATE
echo ""
echo "=== 🔍 KIỂM TRA HIỆN TRẠNG ==="

echo "🔹 Kiểm tra records trong 8 bảng core:"
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -C -Q "
SELECT
    CASE
        WHEN t.name = 'DP01_New' THEN 'DP01'
        ELSE t.name
    END as TableName,
    ISNULL(ps.row_count, 0) as RecordCount
FROM sys.tables t
LEFT JOIN sys.dm_db_partition_stats ps ON t.object_id = ps.object_id AND ps.index_id IN (0,1)
WHERE t.name IN ('DP01_New', 'LN01', 'LN03', 'GL01', 'GL41', 'DPDA', 'EI01', 'RR01')
ORDER BY
    CASE t.name
        WHEN 'DP01_New' THEN 'DP01'
        ELSE t.name
    END" 2>/dev/null

# 3. CHECK DATA INTEGRITY
echo ""
echo "🔹 Kiểm tra tính toàn vẹn dữ liệu DP01_New:"
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -C -Q "
SELECT
    COUNT(*) as TotalRecords,
    COUNT(DISTINCT MA_CN) as UniqueBranches,
    MIN(CREATED_DATE) as FirstImport,
    MAX(CREATED_DATE) as LastImport
FROM DP01_New" 2>/dev/null

# 4. SOLUTION RECOMMENDATIONS
echo ""
echo "=== 💡 GIẢI PHÁP KHUYẾN NGHỊ ==="

echo "🎯 Fix 1: DirectImportController Table Routing"
echo "   - Cập nhật mapping DP01 → DP01_New trong DirectImportController"
echo "   - Hoặc rename DP01_New → DP01 để consistent với frontend"

echo "🎯 Fix 2: Column Count Verification"
echo "   - Kiểm tra lại CSV headers vs database schema"
echo "   - Update migration để match exactly với CSV structure"

echo "🎯 Fix 3: TotalRecords Display"
echo "   - Frontend cần query đúng table name (DP01_New thay vì DP01)"
echo "   - API endpoint cần return correct table data"

echo ""
echo "=== 🚀 THỰC HIỆN FIX ==="

# Check API endpoint response
echo "🔹 Testing API endpoint cho data summary:"
curl -s "http://localhost:5055/api/TestData/summary" | head -200

echo ""
echo "✅ Phân tích hoàn thành. Cần fix:"
echo "   1. Table name mapping trong DirectImportController"
echo "   2. Frontend API calls sử dụng đúng table names"
echo "   3. Database schema alignment với CSV structure"
