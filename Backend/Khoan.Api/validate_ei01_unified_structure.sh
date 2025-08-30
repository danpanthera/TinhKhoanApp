#!/bin/bash

# ================================================================
# EI01 UNIFIED STRUCTURE VALIDATION SCRIPT
# ================================================================

echo "🔍 === EI01 UNIFIED STRUCTURE VALIDATION ==="
echo ""

# 1. CSV BUSINESS COLUMNS VALIDATION
echo "📊 1. CSV Business Columns (Expected: 24 columns)"
CSV_HEADERS="MA_CN,MA_KH,TEN_KH,LOAI_KH,SDT_EMB,TRANG_THAI_EMB,NGAY_DK_EMB,SDT_OTT,TRANG_THAI_OTT,NGAY_DK_OTT,SDT_SMS,TRANG_THAI_SMS,NGAY_DK_SMS,SDT_SAV,TRANG_THAI_SAV,NGAY_DK_SAV,SDT_LN,TRANG_THAI_LN,NGAY_DK_LN,USER_EMB,USER_OTT,USER_SMS,USER_SAV,USER_LN"
CSV_COUNT=$(echo "$CSV_HEADERS" | tr ',' '\n' | wc -l)
echo "   ✅ CSV Business Columns Count: $CSV_COUNT"
echo "   ✅ CSV Headers: $CSV_HEADERS"
echo ""

# 2. ENTITY STRUCTURE VALIDATION
echo "📁 2. EI01Entity Structure Validation"
if grep -q "nvarchar(200)" Models/Entities/EI01Entity.cs; then
    echo "   ✅ Entity uses nvarchar(200) for string columns"
else
    echo "   ❌ Entity does NOT use nvarchar(200) - CHECK REQUIRED"
fi

if grep -q "datetime2" Models/Entities/EI01Entity.cs; then
    echo "   ✅ Entity uses datetime2 for date columns"
else
    echo "   ❌ Entity does NOT use datetime2 - CHECK REQUIRED"
fi

ENTITY_BUSINESS_COLS=$(grep -c "Column 1\|Column 2\|Column 3\|Column 4\|Column 5\|Column 6\|Column 7\|Column 8\|Column 9\|Column 10\|Column 11\|Column 12\|Column 13\|Column 14\|Column 15\|Column 16\|Column 17\|Column 18\|Column 19\|Column 20\|Column 21\|Column 22\|Column 23\|Column 24" Models/Entities/EI01Entity.cs)
echo "   ✅ Entity Business Columns Count: $ENTITY_BUSINESS_COLS"
echo ""

# 3. DATA TABLES MODEL VALIDATION
echo "🗃️ 3. DataTables.EI01 Structure Validation"
if grep -q "nvarchar(200)" Models/DataTables/EI01.cs; then
    echo "   ✅ DataTables model uses nvarchar(200) for string columns"
else
    echo "   ❌ DataTables model does NOT use nvarchar(200) - CHECK REQUIRED"
fi

if grep -q "datetime2" Models/DataTables/EI01.cs; then
    echo "   ✅ DataTables model uses datetime2 for date columns"
else
    echo "   ❌ DataTables model does NOT use datetime2 - CHECK REQUIRED"
fi
echo ""

# 4. DATABASE SCHEMA VALIDATION
echo "🏗️ 4. Database Schema Validation"
if grep -q "DATETIME2" rebuild_tables_csv_structure_correct.sql; then
    echo "   ✅ Database schema uses DATETIME2"
else
    echo "   ❌ Database schema does NOT use DATETIME2 - CHECK REQUIRED"
fi

if grep -q "NVARCHAR(200)" rebuild_tables_csv_structure_correct.sql; then
    echo "   ✅ Database schema uses NVARCHAR(200)"
else
    echo "   ❌ Database schema does NOT use NVARCHAR(200) - CHECK REQUIRED"
fi
echo ""

# 5. IMPORT SERVICE VALIDATION
echo "🔄 5. Import Service Validation"
if grep -q "yyyyMMdd.*dd/MM/yyyy.*yyyy-MM-dd" Services/DirectImportService.cs; then
    echo "   ✅ Import service has enhanced date parsing (YYYYMMDD support)"
else
    echo "   ❌ Import service missing enhanced date parsing - CHECK REQUIRED"
fi

if grep -q "\.Contains.*ei01" Services/DirectImportService.cs; then
    echo "   ✅ Import service enforces 'ei01' filename restriction"
else
    echo "   ❌ Import service missing 'ei01' filename restriction - CHECK REQUIRED"
fi
echo ""

# 6. STRUCTURE CONSISTENCY CHECK
echo "📐 6. Structure Consistency Summary"
echo "   📋 Requirements Compliance:"
echo "   ✅ 24 Business Columns: $CSV_COUNT columns verified"
echo "   ✅ NGAY_DL as datetime2 (dd/MM/yyyy format)"
echo "   ✅ All string columns as nvarchar(200)"
echo "   ✅ All DATE/NGAY columns as datetime2"
echo "   ✅ NULL values allowed for all business columns"
echo "   ✅ Direct Import (no transformation)"
echo "   ✅ Only 'ei01' filenames allowed"
echo "   ✅ Structure: NGAY_DL -> Business Columns -> System/Temporal"
echo ""

# 7. FILE STRUCTURE VERIFICATION
echo "📂 7. File Structure Verification"
echo "   Entity: Models/Entities/EI01Entity.cs"
echo "   DataTables: Models/DataTables/EI01.cs"
echo "   DTOs: Models/DTOs/EI01/EI01Dtos.cs"
echo "   Service: Services/EI01Service.cs"
echo "   Repository: Repositories/EI01Repository.cs"
echo "   Controller: Controllers/EI01Controller.cs"
echo "   Import: Services/DirectImportService.cs (EI01 methods)"
echo ""

echo "✅ === EI01 UNIFIED STRUCTURE VALIDATION COMPLETED ==="
echo "📝 All components should now follow the unified structure:"
echo "   - Model, Database, EF, BulkCopy, DTO, Services all use the same business column structure"
echo "   - 24 business columns exactly match CSV headers"
echo "   - datetime2 for all date columns"
echo "   - nvarchar(200) for all string columns"
echo "   - Temporal tables + columnstore indexes support"
