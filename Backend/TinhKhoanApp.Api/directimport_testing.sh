#!/bin/bash
# SCRIPT: directimport_testing.sh
# MỤC ĐÍCH: Test DirectImport functionality với tất cả CSV files cho 9 bảng

echo "📥 DIRECTIMPORT TESTING - 9 TABLES CSV FILES"
echo "============================================="

BACKEND_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api"
cd "$BACKEND_DIR" || exit 1

# Function test DirectImport API endpoint
test_directimport_api() {
    echo "🌐 TESTING DIRECTIMPORT API ENDPOINTS"
    echo "======================================"

    # Check if backend is running
    echo "🔌 Checking backend status..."
    backend_status=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5055/api/health 2>/dev/null || echo "000")

    if [ "$backend_status" = "200" ]; then
        echo "✅ Backend is running on localhost:5055"
    else
        echo "⚠️  Backend may not be running. Status code: $backend_status"
        echo "💡 You may need to start backend: './start_backend.sh'"
    fi

    # Test DirectImport endpoints availability
    echo ""
    echo "📋 Available DirectImport endpoints:"
    curl -s http://localhost:5055/api/DirectImport/endpoints 2>/dev/null || echo "❌ Could not fetch endpoints"

    echo ""
}

# Function test với từng CSV file
test_csv_file() {
    local table=$1
    local csv_file=$2

    echo "📄 TESTING CSV FILE: $csv_file"
    echo "==============================="

    if [ ! -f "$csv_file" ]; then
        echo "❌ CSV file not found: $csv_file"
        return 1
    fi

    # File info
    echo "📊 File information:"
    echo "   • Size: $(ls -lh $csv_file | awk '{print $5}')"
    echo "   • Lines: $(wc -l < $csv_file)"
    echo "   • Columns: $(head -1 $csv_file | tr ',' '\n' | wc -l | tr -d ' ')"

    # Header verification
    echo ""
    echo "🏷️  CSV Headers:"
    head -1 "$csv_file" | tr ',' '\n' | nl | head -10

    # Sample data
    echo ""
    echo "📋 Sample data (first 3 rows):"
    head -3 "$csv_file"

    # Validation checks
    echo ""
    echo "✅ Validation checks:"

    # Check for empty lines
    empty_lines=$(grep -c "^$" "$csv_file" 2>/dev/null || echo "0")
    echo "   • Empty lines: $empty_lines"

    # Check for consistent column count
    first_line_cols=$(head -1 "$csv_file" | tr ',' '\n' | wc -l | tr -d ' ')
    inconsistent_lines=$(awk -F',' "NF != $first_line_cols {print NR}" "$csv_file" 2>/dev/null | wc -l | tr -d ' ')
    echo "   • Inconsistent column count lines: $inconsistent_lines"

    # Check for special characters
    special_chars=$(grep -c "[^[:print:][:space:]]" "$csv_file" 2>/dev/null || echo "0")
    echo "   • Lines with special characters: $special_chars"

    echo ""
}

# Function test DirectImport cho từng bảng
test_table_import() {
    local table=$1
    local csv_file=$2

    echo "🎯 DIRECTIMPORT TEST: $table"
    echo "============================"

    test_csv_file "$table" "$csv_file"

    # Test API call (if backend is running)
    echo "🌐 API Import test:"
    if command -v curl >/dev/null 2>&1; then
        # Simulated API test (would need actual multipart form data)
        api_endpoint="http://localhost:5055/api/DirectImport/smart"
        echo "   • Endpoint: $api_endpoint"
        echo "   • Method: POST (multipart/form-data)"
        echo "   • File: $csv_file"
        echo "   • Table Target: $table"
        echo "   • Status: READY FOR TESTING"
    else
        echo "   • curl not available for API testing"
    fi

    echo ""
    echo "-------------------------------------------"
    echo ""
}

# Main execution
echo "📅 Date: $(date)"
echo "📍 Location: $BACKEND_DIR"
echo ""

# Test API first
test_directimport_api

echo "📊 CSV FILES TESTING FOR 9 TABLES"
echo "=================================="

# Define CSV files mapping
declare -A csv_files
csv_files["DP01"]="7800_dp01_20241231.csv"
csv_files["DPDA"]="7800_dpda_20250331.csv"
csv_files["EI01"]="7800_ei01_20241231.csv"
csv_files["GL01"]="7800_gl01_2024120120241231.csv"
csv_files["GL02"]="7800_gl02_2024120120241231.csv"
csv_files["GL41"]="7800_gl41_20250630.csv"
csv_files["LN01"]="7800_ln01_20241231.csv"
csv_files["LN03"]="7800_ln03_20241231.csv"
csv_files["RR01"]="7800_rr01_20241231.csv"

# Test each table
for table in "DP01" "DPDA" "EI01" "GL01" "GL02" "GL41" "LN01" "LN03" "RR01"; do
    test_table_import "$table" "${csv_files[$table]}"
done

echo "✅ COMPLETED: DirectImport Testing Analysis"
echo ""
echo "📋 NEXT STEPS FOR ACTUAL TESTING:"
echo "   1. Start backend server: './start_backend.sh'"
echo "   2. Use Postman/curl to test actual import APIs"
echo "   3. Monitor import results in database"
echo "   4. Check for data integrity and column mapping"
echo "   5. Verify error handling for malformed data"
echo ""
echo "🧪 RECOMMENDED TEST SEQUENCE:"
echo "   1. Small files first (GL41: 13 columns)"
echo "   2. Medium files (DPDA, EI01, GL02: 13-24 columns)"
echo "   3. Large files last (DP01, LN01: 63-79 columns)"
