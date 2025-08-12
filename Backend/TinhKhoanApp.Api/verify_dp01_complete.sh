#!/bin/bash

# =============================================================================
# 🔍 COMPREHENSIVE DP01 VERIFICATION SCRIPT
# August 12, 2025 - Complete DP01 Layer Verification
# =============================================================================

echo "🔍 COMPREHENSIVE DP01 VERIFICATION REPORT"
echo "=========================================="

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print status
print_status() {
    local status=$1
    local message=$2
    if [ "$status" = "OK" ]; then
        echo -e "${GREEN}✅ $message${NC}"
    elif [ "$status" = "WARNING" ]; then
        echo -e "${YELLOW}⚠️  $message${NC}"
    else
        echo -e "${RED}❌ $message${NC}"
    fi
}

echo ""
echo "1️⃣  CSV STRUCTURE VERIFICATION"
echo "=============================="

CSV_FILE="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_dp01_20241231.csv"
if [ -f "$CSV_FILE" ]; then
    COLUMN_COUNT=$(head -1 "$CSV_FILE" | tr ',' '\n' | wc -l | tr -d ' ')
    print_status "OK" "CSV file found: 7800_dp01_20241231.csv"
    print_status "OK" "CSV columns count: $COLUMN_COUNT (expected: 63)"

    if [ "$COLUMN_COUNT" = "63" ]; then
        print_status "OK" "CSV column count MATCHES requirement (63 business columns)"
    else
        print_status "ERROR" "CSV column count MISMATCH (expected 63, got $COLUMN_COUNT)"
    fi

    # Show first few headers
    echo "📊 First 10 CSV headers:"
    head -1 "$CSV_FILE" | tr ',' '\n' | head -10 | nl
else
    print_status "ERROR" "CSV file not found at expected location"
fi

echo ""
echo "2️⃣  DATABASE STRUCTURE VERIFICATION"
echo "=================================="

# Check if DP01 table exists
TABLE_EXISTS=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'DP01'" -h-1 -W 2>/dev/null | tail -n +2 | head -1 | tr -d ' \r\n')

if [ "${TABLE_EXISTS:-0}" = "1" ]; then
    print_status "OK" "DP01 table exists in database"

    # Count total columns
    TOTAL_COLUMNS=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DP01'" -h-1 -W 2>/dev/null | tail -n +2 | head -1 | tr -d ' \r\n')
    print_status "OK" "Total database columns: ${TOTAL_COLUMNS:-0}"

    # Check temporal table
    TEMPORAL_TYPE=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT temporal_type FROM sys.tables WHERE name = 'DP01'" -h-1 -W 2>/dev/null | tail -n +2 | head -1 | tr -d ' \r\n')
    if [ "${TEMPORAL_TYPE:-0}" = "2" ]; then
        print_status "OK" "DP01 is SYSTEM_VERSIONED_TEMPORAL_TABLE"
    else
        print_status "ERROR" "DP01 is NOT temporal table (temporal_type: ${TEMPORAL_TYPE:-none})"
    fi

    # Check columnstore index
    COLUMNSTORE_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM sys.indexes i INNER JOIN sys.tables t ON i.object_id = t.object_id WHERE t.name = 'DP01' AND i.type = 6" -h-1 -W 2>/dev/null | tail -n +2 | head -1 | tr -d ' \r\n')
    if [ "${COLUMNSTORE_COUNT:-0}" -gt "0" ]; then
        print_status "OK" "DP01 has columnstore index for analytics"
    else
        print_status "WARNING" "DP01 missing columnstore index"
    fi

    # Check record count
    RECORD_COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM [DP01]" -h-1 -W 2>/dev/null | tail -n +2 | head -1 | tr -d ' \r\n')
    print_status "OK" "DP01 current records: ${RECORD_COUNT:-0}"

    # Verify key business columns exist
    echo "📋 Key business columns verification:"
    for col in "MA_CN" "TAI_KHOAN_HACH_TOAN" "MA_KH" "TEN_KH" "CURRENT_BALANCE" "RATE" "SO_TAI_KHOAN" "ADDRESS"; do
        COL_EXISTS=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DP01' AND COLUMN_NAME = '$col'" -h-1 -W 2>/dev/null | tail -n +2 | head -1 | tr -d ' \r\n')
        if [ "${COL_EXISTS:-0}" = "1" ]; then
            print_status "OK" "   Column $col: EXISTS"
        else
            print_status "ERROR" "   Column $col: MISSING"
        fi
    done

else
    print_status "ERROR" "DP01 table does not exist in database"
fi

echo ""
echo "3️⃣  ENTITY MODEL VERIFICATION"
echo "============================"

ENTITY_FILE="Models/Entities/DP01Entity.cs"
if [ -f "$ENTITY_FILE" ]; then
    print_status "OK" "DP01Entity.cs file exists"

    # Check file size (should be substantial)
    FILE_SIZE=$(stat -f%z "$ENTITY_FILE" 2>/dev/null || echo "0")
    if [ "$FILE_SIZE" -gt "5000" ]; then
        print_status "OK" "Entity file size: $FILE_SIZE bytes (substantial)"
    else
        print_status "WARNING" "Entity file size: $FILE_SIZE bytes (too small?)"
    fi

    # Check key patterns
    if grep -q "ITemporalEntity" "$ENTITY_FILE"; then
        print_status "OK" "Implements ITemporalEntity interface"
    else
        print_status "ERROR" "Does NOT implement ITemporalEntity interface"
    fi

    if grep -q "63 business columns" "$ENTITY_FILE"; then
        print_status "OK" "Documentation mentions 63 business columns"
    else
        print_status "WARNING" "Missing 63 business columns documentation"
    fi

    # Check some key business properties
    for prop in "MA_CN" "TAI_KHOAN_HACH_TOAN" "MA_KH" "CURRENT_BALANCE" "ADDRESS"; do
        if grep -q "public.*$prop" "$ENTITY_FILE"; then
            print_status "OK" "   Property $prop: EXISTS"
        else
            print_status "ERROR" "   Property $prop: MISSING"
        fi
    done

else
    print_status "ERROR" "DP01Entity.cs file not found"
fi

echo ""
echo "4️⃣  DTO LAYER VERIFICATION"
echo "========================="

DTO_FILE="Models/DTOs/DP01/DP01Dtos.cs"
if [ -f "$DTO_FILE" ]; then
    print_status "OK" "DP01Dtos.cs file exists"

    # Check for DP01PreviewDto
    if grep -q "class DP01PreviewDto" "$DTO_FILE"; then
        print_status "OK" "DP01PreviewDto class exists"

        # Count properties in DP01PreviewDto
        DTO_PROPS=$(grep -c "public.*{" "$DTO_FILE" | head -1)
        print_status "OK" "DTO properties count: ${DTO_PROPS:-0}"
    else
        print_status "ERROR" "DP01PreviewDto class NOT found"
    fi

    if grep -q "class DP01CreateDto" "$DTO_FILE"; then
        print_status "OK" "DP01CreateDto class exists"
    else
        print_status "WARNING" "DP01CreateDto class NOT found"
    fi

    if grep -q "class DP01DetailsDto" "$DTO_FILE"; then
        print_status "OK" "DP01DetailsDto class exists"
    else
        print_status "WARNING" "DP01DetailsDto class NOT found"
    fi

else
    print_status "ERROR" "DP01Dtos.cs file not found"
fi

echo ""
echo "5️⃣  REPOSITORY LAYER VERIFICATION"
echo "==============================="

REPO_FILE="Repositories/DP01Repository.cs"
INTERFACE_FILE="Repositories/IDP01Repository.cs"

if [ -f "$REPO_FILE" ] && [ -f "$INTERFACE_FILE" ]; then
    print_status "OK" "DP01Repository files exist"

    REPO_SIZE=$(stat -f%z "$REPO_FILE" 2>/dev/null || echo "0")
    INTERFACE_SIZE=$(stat -f%z "$INTERFACE_FILE" 2>/dev/null || echo "0")

    if [ "$REPO_SIZE" -gt "1000" ]; then
        print_status "OK" "Repository implementation size: $REPO_SIZE bytes"
    else
        print_status "WARNING" "Repository implementation too small: $REPO_SIZE bytes"
    fi

    if [ "$INTERFACE_SIZE" -gt "500" ]; then
        print_status "OK" "Repository interface size: $INTERFACE_SIZE bytes"
    else
        print_status "WARNING" "Repository interface too small: $INTERFACE_SIZE bytes"
    fi

    # Check key methods
    for method in "GetByDateAsync" "GetByBranchCodeAsync" "GetByCustomerCodeAsync"; do
        if grep -q "$method" "$REPO_FILE"; then
            print_status "OK" "   Method $method: EXISTS"
        else
            print_status "WARNING" "   Method $method: MISSING"
        fi
    done

else
    print_status "ERROR" "DP01Repository files missing"
fi

echo ""
echo "6️⃣  SERVICE LAYER VERIFICATION"
echo "============================"

SERVICE_FILE="Services/DP01Service.cs"
INTERFACE_FILE="Services/Interfaces/IDP01Service.cs"

if [ -f "$SERVICE_FILE" ] && [ -f "$INTERFACE_FILE" ]; then
    print_status "OK" "DP01Service files exist"

    SERVICE_SIZE=$(stat -f%z "$SERVICE_FILE" 2>/dev/null || echo "0")
    INTERFACE_SIZE=$(stat -f%z "$INTERFACE_FILE" 2>/dev/null || echo "0")

    if [ "$SERVICE_SIZE" -gt "5000" ]; then
        print_status "OK" "Service implementation size: $SERVICE_SIZE bytes"
    else
        print_status "WARNING" "Service implementation too small: $SERVICE_SIZE bytes"
    fi

    # Check key methods
    for method in "GetAllAsync" "GetByIdAsync" "GetByDateAsync"; do
        if grep -q "$method" "$SERVICE_FILE"; then
            print_status "OK" "   Method $method: EXISTS"
        else
            print_status "WARNING" "   Method $method: MISSING"
        fi
    done

else
    print_status "ERROR" "DP01Service files missing"
fi

echo ""
echo "7️⃣  CONTROLLER LAYER VERIFICATION"
echo "==============================="

CONTROLLER_FILE="Controllers/DP01Controller.cs"
if [ -f "$CONTROLLER_FILE" ]; then
    print_status "OK" "DP01Controller.cs file exists"

    CONTROLLER_SIZE=$(stat -f%z "$CONTROLLER_FILE" 2>/dev/null || echo "0")
    if [ "$CONTROLLER_SIZE" -gt "3000" ]; then
        print_status "OK" "Controller size: $CONTROLLER_SIZE bytes"
    else
        print_status "WARNING" "Controller too small: $CONTROLLER_SIZE bytes"
    fi

    # Check key endpoints
    for endpoint in "GetAllDP01" "GetDP01ById" "GetDP01ByDate"; do
        if grep -q "$endpoint" "$CONTROLLER_FILE"; then
            print_status "OK" "   Endpoint $endpoint: EXISTS"
        else
            print_status "WARNING" "   Endpoint $endpoint: MISSING"
        fi
    done

else
    print_status "ERROR" "DP01Controller.cs file not found"
fi

echo ""
echo "8️⃣  DIRECT IMPORT VERIFICATION"
echo "============================"

IMPORT_SERVICE="Services/DirectImportService.cs"
if [ -f "$IMPORT_SERVICE" ]; then
    print_status "OK" "DirectImportService.cs file exists"

    if grep -q "ImportDP01Async" "$IMPORT_SERVICE"; then
        print_status "OK" "ImportDP01Async method exists"
    else
        print_status "ERROR" "ImportDP01Async method MISSING"
    fi

    if grep -q "ParseDP01CsvAsync" "$IMPORT_SERVICE"; then
        print_status "OK" "ParseDP01CsvAsync method exists"
    else
        print_status "ERROR" "ParseDP01CsvAsync method MISSING"
    fi

    if grep -q "DP01.*63.*business.*columns" "$IMPORT_SERVICE"; then
        print_status "OK" "Documentation mentions 63 business columns"
    else
        print_status "WARNING" "Missing business columns documentation"
    fi

else
    print_status "ERROR" "DirectImportService.cs file not found"
fi

echo ""
echo "9️⃣  BUILD VERIFICATION"
echo "===================="

echo "🔨 Building project..."
BUILD_OUTPUT=$(dotnet build --no-restore -v q 2>&1)
BUILD_EXIT_CODE=$?

if [ $BUILD_EXIT_CODE -eq 0 ]; then
    print_status "OK" "Project builds successfully"

    # Count warnings/errors
    ERROR_COUNT=$(echo "$BUILD_OUTPUT" | grep -c "error" || echo "0")
    WARNING_COUNT=$(echo "$BUILD_OUTPUT" | grep -c "warning" || echo "0")

    print_status "OK" "Build errors: $ERROR_COUNT"
    if [ "$WARNING_COUNT" -lt "10" ]; then
        print_status "OK" "Build warnings: $WARNING_COUNT (acceptable)"
    else
        print_status "WARNING" "Build warnings: $WARNING_COUNT (too many)"
    fi

else
    print_status "ERROR" "Project build FAILED"
    echo "Build output:"
    echo "$BUILD_OUTPUT"
fi

echo ""
echo "🎯 FINAL SUMMARY"
echo "================"

echo "📊 DP01 Layer Completeness:"

# Calculate scores
CSV_SCORE=0
DB_SCORE=0
ENTITY_SCORE=0
DTO_SCORE=0
REPO_SCORE=0
SERVICE_SCORE=0
CONTROLLER_SCORE=0
IMPORT_SCORE=0

# CSV Score (10 points)
if [ -f "$CSV_FILE" ] && [ "$COLUMN_COUNT" = "63" ]; then
    CSV_SCORE=10
fi

# Database Score (20 points)
if [ "${TABLE_EXISTS:-0}" = "1" ]; then
    DB_SCORE=$((DB_SCORE + 5))
    if [ "${TEMPORAL_TYPE:-0}" = "2" ]; then
        DB_SCORE=$((DB_SCORE + 10))
    fi
    if [ "${COLUMNSTORE_COUNT:-0}" -gt "0" ]; then
        DB_SCORE=$((DB_SCORE + 5))
    fi
fi

# Entity Score (15 points)
if [ -f "$ENTITY_FILE" ]; then
    ENTITY_SCORE=$((ENTITY_SCORE + 5))
    if [ "$FILE_SIZE" -gt "5000" ]; then
        ENTITY_SCORE=$((ENTITY_SCORE + 5))
    fi
    if grep -q "ITemporalEntity" "$ENTITY_FILE"; then
        ENTITY_SCORE=$((ENTITY_SCORE + 5))
    fi
fi

# DTO Score (10 points)
if [ -f "$DTO_FILE" ]; then
    DTO_SCORE=$((DTO_SCORE + 5))
    if grep -q "class DP01PreviewDto" "$DTO_FILE"; then
        DTO_SCORE=$((DTO_SCORE + 5))
    fi
fi

# Repository Score (10 points)
if [ -f "$REPO_FILE" ] && [ -f "Repositories/IDP01Repository.cs" ]; then
    REPO_SCORE=$((REPO_SCORE + 10))
fi

# Service Score (15 points)
if [ -f "$SERVICE_FILE" ] && [ -f "Services/Interfaces/IDP01Service.cs" ]; then
    SERVICE_SCORE=$((SERVICE_SCORE + 15))
fi

# Controller Score (10 points)
if [ -f "$CONTROLLER_FILE" ]; then
    CONTROLLER_SCORE=$((CONTROLLER_SCORE + 10))
fi

# Import Score (10 points)
if [ -f "$IMPORT_SERVICE" ] && grep -q "ImportDP01Async" "$IMPORT_SERVICE"; then
    IMPORT_SCORE=$((IMPORT_SCORE + 10))
fi

TOTAL_SCORE=$((CSV_SCORE + DB_SCORE + ENTITY_SCORE + DTO_SCORE + REPO_SCORE + SERVICE_SCORE + CONTROLLER_SCORE + IMPORT_SCORE))
MAX_SCORE=100

echo "• CSV Structure: $CSV_SCORE/10"
echo "• Database Layer: $DB_SCORE/20"
echo "• Entity Model: $ENTITY_SCORE/15"
echo "• DTO Layer: $DTO_SCORE/10"
echo "• Repository: $REPO_SCORE/10"
echo "• Service: $SERVICE_SCORE/15"
echo "• Controller: $CONTROLLER_SCORE/10"
echo "• Direct Import: $IMPORT_SCORE/10"
echo ""
echo "🏆 TOTAL SCORE: $TOTAL_SCORE/$MAX_SCORE ($((TOTAL_SCORE * 100 / MAX_SCORE))%)"

if [ "$TOTAL_SCORE" -ge "90" ]; then
    print_status "OK" "DP01 IMPLEMENTATION: EXCELLENT (90%+)"
elif [ "$TOTAL_SCORE" -ge "80" ]; then
    print_status "OK" "DP01 IMPLEMENTATION: GOOD (80%+)"
elif [ "$TOTAL_SCORE" -ge "70" ]; then
    print_status "WARNING" "DP01 IMPLEMENTATION: ACCEPTABLE (70%+)"
else
    print_status "ERROR" "DP01 IMPLEMENTATION: NEEDS WORK (<70%)"
fi

echo ""
echo "✅ DP01 verification completed!"
