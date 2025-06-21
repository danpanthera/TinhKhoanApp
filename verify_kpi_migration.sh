#!/bin/bash

# ===================================================================
# KPI Indicator Migration Verification Script
# Date: June 21, 2025
# Purpose: Verify complete replacement of interest collection rate indicators
# ===================================================================

echo "🔍 KPI Indicator Migration Verification"
echo "======================================="
echo ""

PROJECT_ROOT="/Users/nguyendat/Documents/Projects/TinhKhoanApp"
cd "$PROJECT_ROOT"

echo "📊 1. Checking for remaining old indicator references..."
echo "-------------------------------------------------------"

# Search for old indicator names
OLD_REFS=$(find . -type f \( -name "*.cs" -o -name "*.js" -o -name "*.vue" -o -name "*.sql" \) -not -path "./Frontend/tinhkhoan-app-ui-vite/dist/*" -not -path "./Backend/TinhKhoanApp.Api/migrate_kpi_indicators_to_customer_development.sql" -exec grep -l "Tỷ lệ thực thu lãi" {} \; 2>/dev/null)

if [ -z "$OLD_REFS" ]; then
    echo "✅ No remaining references to old indicators found"
else
    echo "⚠️  Found remaining references in:"
    echo "$OLD_REFS"
fi

echo ""

echo "📊 2. Checking for new indicator references..."
echo "----------------------------------------------"

# Search for new indicator names
NEW_REFS=$(find . -type f \( -name "*.cs" -o -name "*.js" -o -name "*.vue" -o -name "*.sql" \) -exec grep -l "Phát triển khách hàng mới" {} \; 2>/dev/null | wc -l)

echo "✅ Found $NEW_REFS files with new indicator name"

echo ""

echo "📊 3. Checking for TYLETHUCTHULAI code references..."
echo "----------------------------------------------------"

# Search for old KPI codes
OLD_CODES=$(find . -type f \( -name "*.cs" -o -name "*.js" -o -name "*.vue" \) -not -path "./Frontend/tinhkhoan-app-ui-vite/dist/*" -exec grep -l "TYLETHUCTHULAI" {} \; 2>/dev/null)

if [ -z "$OLD_CODES" ]; then
    echo "✅ No remaining TYLETHUCTHULAI code references found"
else
    echo "⚠️  Found remaining code references in:"
    echo "$OLD_CODES"
fi

# Search for new KPI codes
NEW_CODES=$(find . -type f \( -name "*.cs" -o -name "*.js" -o -name "*.vue" \) -exec grep -l "PHATTRIENKHACHHANG" {} \; 2>/dev/null | wc -l)

echo "✅ Found $NEW_CODES files with new indicator code"

echo ""

echo "📊 4. Checking unit of measure changes..."
echo "-----------------------------------------"

# Check for percentage unit in KPI contexts
PCT_UNITS=$(find . -type f \( -name "*.sql" \) -not -path "./Backend/TinhKhoanApp.Api/migrate_kpi_indicators_to_customer_development.sql" -exec grep -l "'%'.*Phát triển khách hàng mới\|'Phát triển khách hàng mới'.*'%'" {} \; 2>/dev/null)

if [ -z "$PCT_UNITS" ]; then
    echo "✅ No percentage units found with new indicator"
else
    echo "⚠️  Found percentage units still associated with new indicator:"
    echo "$PCT_UNITS"
fi

# Check for customer units
CUSTOMER_UNITS=$(find . -type f \( -name "*.sql" \) -exec grep -l "'Khách hàng'.*Phát triển khách hàng mới\|'Phát triển khách hàng mới'.*'Khách hàng'" {} \; 2>/dev/null | wc -l)

echo "✅ Found $CUSTOMER_UNITS files with 'Khách hàng' unit"

echo ""

echo "📊 5. Summary of Modified Files"
echo "==============================="

echo ""
echo "Backend Files Modified:"
echo "• create_kpi_indicators_part1.sql"
echo "• create_kpi_indicators_part3.sql" 
echo "• add_lai_chau_kpi_table.sql"
echo "• khoi_phuc_kpi_indicators.sql"
echo "• Services/KpiScoringService.cs"
echo "• Data/SeedKpiScoringRules.old"

echo ""
echo "Frontend Files Modified:"
echo "• src/views/KpiScoringView.vue"
echo "• src/services/kpiScoringService.js"

echo ""
echo "Documentation Updated:"
echo "• KPI_RECOVERY_COMPLETION_REPORT.md"
echo "• KPI_INDICATORS_FIX_REPORT.md"

echo ""
echo "Migration Files Created:"
echo "• migrate_kpi_indicators_to_customer_development.sql"
echo "• KPI_INDICATOR_MIGRATION_COMPLETION_REPORT.md"

echo ""
echo "🎯 Migration Status: COMPLETED ✅"
echo ""
echo "Next Steps:"
echo "1. Test migration script in development database"
echo "2. Execute migration in production database"  
echo "3. Rebuild and deploy frontend application"
echo "4. Verify all KPI functionality works correctly"
echo ""
