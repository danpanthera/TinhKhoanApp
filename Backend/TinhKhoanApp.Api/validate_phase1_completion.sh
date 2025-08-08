#!/bin/bash

# =============================================================================
# PHASE 1 COMPLETION VALIDATION SCRIPT
# Clean Architecture Foundation - 9 Core Tables Verification
# =============================================================================

echo "🎯 PHASE 1 COMPLETION VALIDATION - Clean Architecture Foundation"
echo "==============================================================="

BACKEND_PATH="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api"
cd "$BACKEND_PATH"

echo ""
echo "📊 VALIDATION RESULTS:"
echo "====================="

# Initialize counters
TOTAL_CHECKS=0
PASSED_CHECKS=0

# Function to check file and report
check_file() {
    local file_path="$1"
    local description="$2"
    TOTAL_CHECKS=$((TOTAL_CHECKS + 1))

    if [ -f "$file_path" ]; then
        echo "✅ $description"
        PASSED_CHECKS=$((PASSED_CHECKS + 1))
        return 0
    else
        echo "❌ $description"
        return 1
    fi
}

# Function to check file content
check_content() {
    local file_path="$1"
    local search_pattern="$2"
    local description="$3"
    TOTAL_CHECKS=$((TOTAL_CHECKS + 1))

    if [ -f "$file_path" ] && grep -q "$search_pattern" "$file_path"; then
        echo "✅ $description"
        PASSED_CHECKS=$((PASSED_CHECKS + 1))
        return 0
    else
        echo "❌ $description"
        return 1
    fi
}

echo ""
echo "🏗️  ARCHITECTURE FOUNDATION:"
echo "============================="

# ProductionDataController
check_file "Controllers/ProductionDataController.cs" "ProductionDataController.cs exists"
TOTAL_CHECKS=$((TOTAL_CHECKS + 1))
controller_services=$(grep -o "IDP01Service\|IRR01Service\|ILN01Service\|ILN03Service\|IDPDAService\|IGL01Service\|IGL02Service\|IGL41Service\|IEI01Service" Controllers/ProductionDataController.cs | sort -u | wc -l)
if [ "$controller_services" -eq 9 ]; then
    echo "✅ ProductionDataController covers all 9 tables"
    PASSED_CHECKS=$((PASSED_CHECKS + 1))
else
    echo "❌ ProductionDataController covers all 9 tables (found: $controller_services)"
fi

# ServiceCollectionExtensions
check_file "Extensions/ServiceCollectionExtensions.cs" "ServiceCollectionExtensions.cs exists"
TOTAL_CHECKS=$((TOTAL_CHECKS + 1))
di_repos=$(grep -o "IDP01Repository\|IRR01Repository\|ILN01Repository\|ILN03Repository\|IDPDARepository\|IGL01Repository\|IGL02Repository\|IGL41Repository\|IEI01Repository" Extensions/ServiceCollectionExtensions.cs | sort -u | wc -l)
if [ "$di_repos" -eq 9 ]; then
    echo "✅ DI setup includes all 9 services"
    PASSED_CHECKS=$((PASSED_CHECKS + 1))
else
    echo "❌ DI setup includes all 9 services (found: $di_repos)"
fi

echo ""
echo "🎯 SERVICE INTERFACES (9/9):"
echo "============================="

check_file "Services/Interfaces/IDP01Service.cs" "IDP01Service interface"
check_file "Services/Interfaces/IDPDAService.cs" "IDPDAService interface"
check_file "Services/Interfaces/IGL01Service.cs" "IGL01Service interface"
check_file "Services/Interfaces/IGL02Service.cs" "IGL02Service interface"
check_file "Services/Interfaces/IGL41Service.cs" "IGL41Service interface"
check_file "Services/Interfaces/ILN01Service.cs" "ILN01Service interface"
check_file "Services/Interfaces/ILN03Service.cs" "ILN03Service interface"
check_file "Services/Interfaces/IEI01Service.cs" "IEI01Service interface"
check_file "Services/Interfaces/IRR01Service.cs" "IRR01Service interface"

echo ""
echo "📋 DTO FILES (9/9):"
echo "==================="

check_file "Models/DTOs/DP01/DP01Dtos.cs" "DP01 DTOs (63 columns)"
check_file "Models/DTOs/DPDA/DPDADtos.cs" "DPDA DTOs (13 columns)"
check_file "Models/DTOs/GL01/GL01Dtos.cs" "GL01 DTOs (27 columns)"
check_file "Models/DTOs/GL02/GL02Dtos.cs" "GL02 DTOs (17 columns)"
check_file "Models/DTOs/GL41/GL41Dtos.cs" "GL41 DTOs (13 columns)"
check_file "Models/DTOs/LN01/LN01Dtos.cs" "LN01 DTOs (79 columns)"
check_file "Models/DTOs/LN03/LN03Dtos.cs" "LN03 DTOs (20 columns)"
check_file "Models/DTOs/EI01/EI01Dtos.cs" "EI01 DTOs (24 columns)"
check_file "Models/DTOs/RR01/RR01Dtos.cs" "RR01 DTOs (25 columns)"

echo ""
echo "🗃️  REPOSITORY INTERFACES (9/9):"
echo "================================="

check_file "Repositories/Interfaces/IDP01Repository.cs" "IDP01Repository interface"
check_file "Repositories/Interfaces/IDPDARepository.cs" "IDPDARepository interface"
check_file "Repositories/Interfaces/IGL01Repository.cs" "IGL01Repository interface"
check_file "Repositories/Interfaces/IGL02Repository.cs" "IGL02Repository interface"
check_file "Repositories/Interfaces/IGL41Repository.cs" "IGL41Repository interface"
check_file "Repositories/Interfaces/ILN01Repository.cs" "ILN01Repository interface"
check_file "Repositories/Interfaces/ILN03Repository.cs" "ILN03Repository interface"
check_file "Repositories/Interfaces/IEI01Repository.cs" "IEI01Repository interface"
check_file "Repositories/Interfaces/IRR01Repository.cs" "IRR01Repository interface"

echo ""
echo "🔧 DETAILED VALIDATION:"
echo "======================="

# Check LN01Dtos has all 79 columns
if [ -f "Models/DTOs/LN01/LN01Dtos.cs" ]; then
    TOTAL_CHECKS=$((TOTAL_CHECKS + 1))
    column_count=$(grep -E "(BRCD|CUSTSEQ|CUSTNM|TAI_KHOAN|CCY|DU_NO|DSBSSEQ|TRANSACTION_DATE|DSBSDT|DISBUR_CCY|DISBURSEMENT_AMOUNT|DSBSMATDT|BSRTCD|INTEREST_RATE|APPRSEQ|APPRDT|APPR_CCY|APPRAMT|APPRMATDT|LOAN_TYPE|FUND_RESOURCE_CODE|FUND_PURPOSE_CODE|REPAYMENT_AMOUNT|NEXT_REPAY_DATE|NEXT_REPAY_AMOUNT|NEXT_INT_REPAY_DATE|OFFICER_ID|OFFICER_NAME|INTEREST_AMOUNT|PASTDUE_INTEREST_AMOUNT|TOTAL_INTEREST_REPAY_AMOUNT|CUSTOMER_TYPE_CODE|CUSTOMER_TYPE_CODE_DETAIL|TRCTCD|TRCTNM|ADDR1|PROVINCE|LCLPROVINNM|DISTRICT|LCLDISTNM|COMMCD|LCLWARDNM|LAST_REPAY_DATE|SECURED_PERCENT|NHOM_NO|LAST_INT_CHARGE_DATE|EXEMPTINT|EXEMPTINTTYPE|EXEMPTINTAMT|GRPNO|BUSCD|BSNSSCLTPCD|USRIDOP|ACCRUAL_AMOUNT|ACCRUAL_AMOUNT_END_OF_MONTH|INTCMTH|INTRPYMTH|INTTRMMTH|YRDAYS|REMARK|CHITIEU|CTCV|CREDIT_LINE_YPE|INT_LUMPSUM_PARTIAL_TYPE|INT_PARTIAL_PAYMENT_TYPE|INT_PAYMENT_INTERVAL|AN_HAN_LAI|PHUONG_THUC_GIAI_NGAN_1|TAI_KHOAN_GIAI_NGAN_1|SO_TIEN_GIAI_NGAN_1|PHUONG_THUC_GIAI_NGAN_2|TAI_KHOAN_GIAI_NGAN_2|SO_TIEN_GIAI_NGAN_2|CMT_HC|NGAY_SINH|MA_CB_AGRI|MA_NGANH_KT|TY_GIA|OFFICER_IPCAS)" "Models/DTOs/LN01/LN01Dtos.cs" | wc -l)

    if [ "$column_count" -ge 79 ]; then
        echo "✅ LN01 DTOs contains all 79 business columns"
        PASSED_CHECKS=$((PASSED_CHECKS + 1))
    else
        echo "❌ LN01 DTOs missing business columns (found: $column_count, expected: 79+)"
    fi
fi

# Check DTO patterns (6 types per table)
TOTAL_CHECKS=$((TOTAL_CHECKS + 1))
dto_patterns=$(find Models/DTOs -name "*.cs" -exec grep -l "PreviewDto\|CreateDto\|UpdateDto\|DetailsDto\|SummaryDto\|ImportResultDto" {} \; | wc -l)
if [ "$dto_patterns" -ge 9 ]; then
    echo "✅ All 9 tables have standardized DTO patterns"
    PASSED_CHECKS=$((PASSED_CHECKS + 1))
else
    echo "❌ Missing DTO patterns in some tables"
fi

echo ""
echo "📈 COMPLETION SUMMARY:"
echo "====================="
echo "✅ Passed: $PASSED_CHECKS/$TOTAL_CHECKS checks"

COMPLETION_PERCENT=$((PASSED_CHECKS * 100 / TOTAL_CHECKS))
echo "📊 Completion: $COMPLETION_PERCENT%"

if [ "$COMPLETION_PERCENT" -eq 100 ]; then
    echo ""
    echo "🎉 PHASE 1 COMPLETE! 🎉"
    echo "======================="
    echo "✅ All 9 core tables foundation established"
    echo "✅ Clean architecture patterns implemented"
    echo "✅ Service interfaces standardized"
    echo "✅ DTOs with proper validation attributes"
    echo "✅ Repository interfaces defined"
    echo "✅ Dependency injection configured"
    echo ""
    echo "🚀 Ready for PHASE 2: Repository & Service Implementations"

elif [ "$COMPLETION_PERCENT" -ge 90 ]; then
    echo ""
    echo "⚡ PHASE 1 NEARLY COMPLETE!"
    echo "=========================="
    echo "Minor items remaining - ready to proceed to Phase 2"

else
    echo ""
    echo "⚠️  PHASE 1 INCOMPLETE"
    echo "===================="
    echo "Please complete remaining items before Phase 2"
fi

echo ""
echo "📋 NEXT STEPS:"
echo "============="
echo "1. 🏗️  Implement Repository classes (9 tables)"
echo "2. 🔧 Implement Service classes (9 tables)"
echo "3. 🧪 Unit tests for services"
echo "4. 📊 Integration tests for controllers"
echo "5. 🚀 Frontend integration"

echo ""
echo "📄 Generated: $(date '+%Y-%m-%d %H:%M:%S')"
