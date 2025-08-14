#!/bin/bash
# Test toàn diện LN03 system - CSV, Model, Database, Service, API

echo "🧪 COMPREHENSIVE LN03 SYSTEM TEST - $(date)"
echo "=================================================================="

# 1. Build check
echo ""
echo "📦 1. BUILD CHECK:"
cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api
echo "Building project..."
build_result=$(dotnet build --no-restore 2>&1)
if [[ $? -eq 0 ]]; then
    echo "✅ Build successful"
    echo "📊 Warnings: $(echo "$build_result" | grep -c "warning")"
    echo "❌ Errors: $(echo "$build_result" | grep -c "error")"
else
    echo "❌ Build failed!"
    echo "$build_result"
    exit 1
fi

# 2. Database connectivity check
echo ""
echo "🗄️  2. DATABASE CONNECTIVITY:"
if command -v sqlcmd > /dev/null; then
    db_check=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) as TableExists FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'LN03'" 2>/dev/null | grep -E '^[0-9]+$')
    if [[ "$db_check" == "1" ]]; then
        echo "✅ LN03 table exists in database"

        # Get current record count
        record_count=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM LN03" 2>/dev/null | grep -E '^[0-9]+$' || echo "0")
        echo "📍 Current LN03 records: $record_count"
    else
        echo "❌ LN03 table not found!"
    fi
else
    echo "❌ sqlcmd not available"
fi

# 3. CSV file analysis
echo ""
echo "📄 3. CSV FILE ANALYSIS:"
csv_file="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_ln03_20241231.csv"
if [[ -f "$csv_file" ]]; then
    echo "✅ CSV file found: $(basename "$csv_file")"

    # Count lines
    total_lines=$(wc -l < "$csv_file")
    data_lines=$((total_lines - 1))
    echo "📍 Total lines: $total_lines (1 header + $data_lines data)"

    # Sample first data row
    echo ""
    echo "🔍 Sample first data row (20 fields):"
    awk -F',' 'NR==2 {for(i=1; i<=NF && i<=20; i++) printf "%2d: %s\n", i, $i}' "$csv_file"
else
    echo "❌ CSV file not found: $csv_file"
fi

# 4. Model validation
echo ""
echo "📋 4. MODEL VALIDATION:"
model_file="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Models/DataTables/LN03.cs"
if [[ -f "$model_file" ]]; then
    echo "✅ LN03 model found"

    # Count properties and column orders
    total_props=$(grep -c "public.*{.*get.*set.*}" "$model_file")
    ordered_cols=$(grep -c "Column(Order" "$model_file")

    echo "📍 Total properties: $total_props"
    echo "📍 Ordered columns: $ordered_cols"

    # Check critical business columns
    echo ""
    echo "🔍 Critical business columns check:"
    for col in "MACHINHANH" "TENCHINHANH" "MAKH" "TENKH" "SOTIENXLRR" "NGAYPHATSINHXL"; do
        if grep -q "public.*$col" "$model_file"; then
            echo "✅ $col - found"
        else
            echo "❌ $col - missing"
        fi
    done

    # Check no-header columns
    echo ""
    echo "🔍 No-header columns check:"
    for col in "Column18" "Column19" "Column20"; do
        if grep -q "public.*$col" "$model_file"; then
            echo "✅ $col - found"
        else
            echo "❌ $col - missing"
        fi
    done
else
    echo "❌ Model file not found!"
fi

# 5. Service and Repository check
echo ""
echo "🔧 5. SERVICE & REPOSITORY CHECK:"

# Check service file
if [[ -f "Services/LN03Service.cs" ]]; then
    echo "✅ LN03Service found"
    service_methods=$(grep -c "public.*async.*Task" Services/LN03Service.cs || echo "0")
    echo "📍 Service methods: $service_methods"
else
    echo "❌ LN03Service not found"
fi

# Check repository
if [[ -f "Repositories/LN03Repository.cs" ]]; then
    echo "✅ LN03Repository found"
else
    echo "❌ LN03Repository not found"
fi

# Check controller
if [[ -f "Controllers/LN03Controller.cs" ]]; then
    echo "✅ LN03Controller found"
    controller_endpoints=$(grep -c "\[Http" Controllers/LN03Controller.cs || echo "0")
    echo "📍 Controller endpoints: $controller_endpoints"
else
    echo "❌ LN03Controller not found"
fi

# 6. DirectImport configuration check
echo ""
echo "🚀 6. DIRECTIMPORT CONFIGURATION:"
if grep -q '"LN03"' appsettings.json; then
    echo "✅ LN03 DirectImport configuration found"

    # Extract key settings
    always_direct=$(grep -A 10 '"LN03"' appsettings.json | grep '"AlwaysDirectImport"' | grep -o 'true\|false')
    use_custom_parser=$(grep -A 10 '"LN03"' appsettings.json | grep '"UseCustomParser"' | grep -o 'true\|false')

    echo "📍 AlwaysDirectImport: $always_direct"
    echo "📍 UseCustomParser: $use_custom_parser"
else
    echo "❌ LN03 DirectImport configuration not found"
fi

# Check DirectImportService has LN03 implementation
if grep -q "ImportLN03.*Async" Services/DirectImportService.cs; then
    echo "✅ DirectImportService has LN03 import method"

    # Count LN03-related methods
    ln03_methods=$(grep -c "LN03.*Async\|ParseLN03" Services/DirectImportService.cs || echo "0")
    echo "📍 LN03-specific methods: $ln03_methods"
else
    echo "❌ DirectImportService missing LN03 implementation"
fi

# 7. Integration readiness summary
echo ""
echo "🎯 7. SYSTEM READINESS SUMMARY:"
echo "=================================================================="

readiness_score=0
total_checks=7

# Build
[[ $? -eq 0 ]] && readiness_score=$((readiness_score + 1))

# Database
[[ "$db_check" == "1" ]] && readiness_score=$((readiness_score + 1))

# CSV
[[ -f "$csv_file" ]] && readiness_score=$((readiness_score + 1))

# Model
[[ -f "$model_file" ]] && readiness_score=$((readiness_score + 1))

# Service
[[ -f "Services/LN03Service.cs" ]] && readiness_score=$((readiness_score + 1))

# Controller
[[ -f "Controllers/LN03Controller.cs" ]] && readiness_score=$((readiness_score + 1))

# DirectImport
grep -q "ImportLN03.*Async" Services/DirectImportService.cs && readiness_score=$((readiness_score + 1))

echo "📊 READINESS SCORE: $readiness_score/$total_checks"

if [[ $readiness_score -eq $total_checks ]]; then
    echo "🎉 LN03 SYSTEM IS 100% READY FOR TESTING!"
    echo ""
    echo "🚀 NEXT STEPS:"
    echo "1. Test CSV import: POST /api/DirectImport/smart (file: ln03 CSV)"
    echo "2. Test data preview: GET /api/LN03/preview"
    echo "3. Verify temporal functionality"
    echo "4. Performance testing with large datasets"
elif [[ $readiness_score -ge 5 ]]; then
    echo "⚠️  LN03 system mostly ready - minor issues to fix"
else
    echo "❌ LN03 system needs significant work before testing"
fi

echo ""
echo "=================================================================="
echo "Test completed at $(date)"
