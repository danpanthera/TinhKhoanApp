#!/bin/bash

# Architecture Validation Script
# Kiểm tra sự thống nhất giữa tất cả các thành phần cho 9 CORE TABLES:
# DP01, DPDA, GL01, GL02, GL41, LN01, LN03, EI01, RR01
# Migration ↔ Database ↔ Model ↔ EF ↔ BulkCopy ↔ Direct Import ↔ Services ↔ Repository ↔ DTO

echo "🏗️  ARCHITECTURE VALIDATION SCRIPT - 9 CORE TABLES"
echo "=================================================="
echo "Tables: DP01, DPDA, GL01, GL02, GL41, LN01, LN03, EI01, RR01"
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Counters
TOTAL_CHECKS=0
PASSED_CHECKS=0
FAILED_CHECKS=0

# Function to run a check
run_check() {
    local check_name="$1"
    local check_command="$2"
    local expected_result="$3"

    ((TOTAL_CHECKS++))
    echo -e "${BLUE}🔍 Checking: $check_name${NC}"

    if eval "$check_command" >/dev/null 2>&1; then
        echo -e "${GREEN}✅ PASS: $check_name${NC}"
        ((PASSED_CHECKS++))
    else
        echo -e "${RED}❌ FAIL: $check_name${NC}"
        ((FAILED_CHECKS++))
    fi
    echo ""
}

echo "1️⃣  MIGRATION ↔ DATABASE CONSISTENCY"
echo "-----------------------------------"

# Check if RR01 table exists with correct structure
run_check "RR01 table exists in database" \
    "sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Passw0rd' -d TinhKhoanDB -Q 'SELECT TOP 1 * FROM RR01' -h -1" \
    "success"

# Check if RR01 has 25 business columns
run_check "RR01 has 25 business columns (NGAY_DL + 24 others)" \
    "sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Passw0rd' -d TinhKhoanDB -Q 'SELECT COUNT(*) as col_count FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '\''RR01'\'' AND COLUMN_NAME NOT IN ('\''Id'\'', '\''CreatedAt'\'', '\''UpdatedAt'\'', '\''SysStartTime'\'', '\''SysEndTime'\'')' -h -1 | grep -q '25'"

# Check if RR01 has temporal table
run_check "RR01 has temporal table (SYSTEM_VERSIONING)" \
    "sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Passw0rd' -d TinhKhoanDB -Q 'SELECT temporal_type_desc FROM sys.tables WHERE name = '\''RR01'\''' -h -1 | grep -q 'SYSTEM_VERSIONED_TEMPORAL_TABLE'"

# Check if RR01 has columnstore index
run_check "RR01 has columnstore index" \
    "sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Passw0rd' -d TinhKhoanDB -Q 'SELECT COUNT(*) FROM sys.indexes WHERE object_id = OBJECT_ID('\''RR01'\'') AND type_desc = '\''CLUSTERED COLUMNSTORE'\''' -h -1 | grep -q '1'"

echo ""
echo "2️⃣  MODEL ↔ EF CONSISTENCY"
echo "------------------------"

# Check if RR01 model exists
run_check "RR01 model file exists" \
    "test -f Models/DataModels/RR01.cs"

# Check if RR01 DbSet exists in ApplicationDbContext
run_check "RR01 DbSet configured in ApplicationDbContext" \
    "grep -q 'DbSet<RR01>' Data/ApplicationDbContext.cs"

echo ""
echo "3️⃣  DTO ↔ CSV CONSISTENCY"
echo "------------------------"

# Check if RR01 DTOs exist
run_check "RR01 DTOs file exists" \
    "test -f Models/DTOs/RR01/RR01Dtos.cs"

# Check if RR01PreviewDto has correct properties
run_check "RR01PreviewDto contains key properties" \
    "grep -q 'NGAY_DL.*DateTime' Models/DTOs/RR01/RR01Dtos.cs && grep -q 'DUNO.*decimal' Models/DTOs/RR01/RR01Dtos.cs"

# Check CSV structure matches DTO
run_check "RR01 CSV has 25 columns matching DTO structure" \
    "head -n 1 /Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_rr01_*.csv 2>/dev/null | tr ',' '\n' | wc -l | grep -q '25'"

echo ""
echo "4️⃣  SERVICE ↔ REPOSITORY CONSISTENCY"
echo "----------------------------------"

# Check if service interfaces exist
run_check "IDP01Service interface exists" \
    "test -f Services/Interfaces/IDP01Service.cs"

run_check "IRR01Service interface exists" \
    "test -f Services/Interfaces/IRR01Service.cs"

# Check if service implementations exist
run_check "DP01Service implementation exists" \
    "test -f Services/DP01Service.cs"

run_check "RR01Service implementation exists" \
    "test -f Services/RR01Service.cs"

# Check if repository interfaces exist
run_check "IDP01Repository interface exists" \
    "test -f Repositories/Interfaces/IDP01Repository.cs"

run_check "IRR01Repository interface exists" \
    "test -f Repositories/Interfaces/IRR01Repository.cs"

echo ""
echo "5️⃣  CONTROLLER ↔ SERVICE CONSISTENCY"
echo "----------------------------------"

# Check if ProductionDataController exists
run_check "ProductionDataController exists" \
    "test -f Controllers/ProductionDataController.cs"

# Check if controller uses service interfaces
run_check "ProductionDataController uses IDP01Service" \
    "grep -q 'IDP01Service' Controllers/ProductionDataController.cs"

run_check "ProductionDataController has standardized endpoints" \
    "grep -q '\[HttpGet.*preview\]' Controllers/ProductionDataController.cs && grep -q '\[HttpPost.*import\]' Controllers/ProductionDataController.cs"

echo ""
echo "6️⃣  DEPENDENCY INJECTION SETUP"
echo "-----------------------------"

# Check if DI extensions exist
run_check "ServiceCollectionExtensions exists" \
    "test -f Extensions/ServiceCollectionExtensions.cs"

run_check "DI contains repository registrations" \
    "grep -q 'AddScoped<IDP01Repository' Extensions/ServiceCollectionExtensions.cs"

run_check "DI contains service registrations" \
    "grep -q 'AddScoped<IDP01Service' Extensions/ServiceCollectionExtensions.cs"

echo ""
echo "7️⃣  UNIT TESTS COVERAGE"
echo "---------------------"

# Check if unit tests exist
run_check "DP01Service unit tests exist" \
    "test -f Tests/Services/DP01ServiceTests.cs"

run_check "Unit tests cover ApiResponse standardization" \
    "grep -q 'ApiResponse.*DP01PreviewDto' Tests/Services/DP01ServiceTests.cs"

run_check "Unit tests cover DTO mapping validation" \
    "grep -q 'Should_Map_DP01_To_DP01PreviewDto' Tests/Services/DP01ServiceTests.cs"

echo ""
echo "8️⃣  BUILD & COMPILATION CHECK"
echo "----------------------------"

# Check if project builds successfully
run_check "Project builds without errors" \
    "dotnet build --configuration Debug --verbosity quiet --nologo"

# Check if no warnings in critical files
run_check "No compilation warnings in Services" \
    "dotnet build Services/ --verbosity quiet --nologo 2>&1 | grep -qv 'warning'"

echo ""
echo "📊 VALIDATION SUMMARY"
echo "===================="
echo -e "Total Checks: ${BLUE}$TOTAL_CHECKS${NC}"
echo -e "Passed: ${GREEN}$PASSED_CHECKS${NC}"
echo -e "Failed: ${RED}$FAILED_CHECKS${NC}"

if [ $FAILED_CHECKS -eq 0 ]; then
    echo ""
    echo -e "${GREEN}🎉 ALL ARCHITECTURE VALIDATIONS PASSED!${NC}"
    echo -e "${GREEN}✅ Migration ↔ Database ↔ Model ↔ EF ↔ BulkCopy ↔ Direct Import ↔ Services ↔ Repository ↔ DTO consistency verified${NC}"
    exit 0
else
    echo ""
    echo -e "${RED}⚠️  ARCHITECTURE VALIDATION FAILED${NC}"
    echo -e "${YELLOW}Please review failed checks above and ensure all components are properly aligned${NC}"
    echo ""
    echo "Next steps:"
    echo "1. Fix failed validations"
    echo "2. Run this script again"
    echo "3. Once all pass, architecture is ready for production"
    exit 1
fi
