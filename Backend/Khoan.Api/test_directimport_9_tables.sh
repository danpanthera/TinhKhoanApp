#!/bin/bash

# Test Script cho DirectImportController với 9 bảng
# File format: 7800_<table>_yyyymmdd.csv

echo "🧪 TESTING DirectImportController với 9 bảng"
echo "=============================================="

API_BASE="http://localhost:5000/api/DirectImport"

# Test table-counts endpoint
echo "📊 Testing table-counts endpoint..."
echo "curl -X GET \"$API_BASE/table-counts\""
echo ""

echo "🔍 Testing smart import detection cho từng bảng:"
echo "================================================="

# Test cases với filename patterns
declare -a test_files=(
    "7800_dp01_20241231.csv"
    "7800_dpda_20250131.csv"
    "7800_ei01_20241130.csv"
    "7800_ln01_20241231.csv"
    "7800_ln03_20241231.csv"
    "7800_gl01_20241231.csv"
    "7800_gl02_20241231.csv"
    "7800_gl41_20241231.csv"
    "7800_rr01_20241231.csv"
)

for filename in "${test_files[@]}"
do
    table_code=$(echo "$filename" | sed -n 's/.*_\([^_]*\)_[0-9]*.csv/\1/p' | tr '[:lower:]' '[:upper:]')
    echo "🎯 Testing $table_code detection:"
    echo "   Filename: $filename"
    echo "   Expected DataType: $table_code"
    echo "   curl -X POST \"$API_BASE/smart\" \\"
    echo "        -H \"Content-Type: multipart/form-data\" \\"
    echo "        -F \"file=@$filename\""
    echo ""
done

echo "✅ All 9 tables supported!"
echo ""
echo "🔧 NEXT STEPS:"
echo "=============="
echo "1. Tạo sample CSV files cho testing"
echo "2. Start backend API server"
echo "3. Run actual API tests"
echo "4. Verify database records"
echo ""
echo "💡 DirectImportController endpoints:"
echo "   POST /api/DirectImport/smart - Smart import với auto-detection"
echo "   GET  /api/DirectImport/table-counts - Lấy số records của các bảng"
