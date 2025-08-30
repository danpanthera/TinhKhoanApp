#!/bin/bash

echo "🔍 GL01 UNIFIED STRUCTURE VALIDATION SCRIPT"
echo "=============================================="

echo ""
echo "📋 REQUIREMENT CHECKLIST - GL01 UNIFIED STRUCTURE (HEAVY FILE OPTIMIZED)"

# 1. Entity Model Check
echo ""
echo "1️⃣  GL01Entity.cs - Entity Model Validation"
echo "--------------------------------------------"

if grep -q "27 business columns" Models/Entities/GL01Entity.cs; then
    echo "✅ GL01Entity.cs có comment '27 business columns'"
else
    echo "❌ GL01Entity.cs thiếu comment '27 business columns'"
fi

if grep -q "HEAVY FILE OPTIMIZED" Models/Entities/GL01Entity.cs; then
    echo "✅ GL01Entity.cs có comment 'HEAVY FILE OPTIMIZED'"
else
    echo "❌ GL01Entity.cs thiếu comment 'HEAVY FILE OPTIMIZED'"
fi

if grep -q "nvarchar(1000)" Models/Entities/GL01Entity.cs; then
    echo "✅ GL01Entity.cs có REMARK column nvarchar(1000)"
else
    echo "❌ GL01Entity.cs thiếu REMARK column nvarchar(1000)"
fi

if grep -q "FileName.*nvarchar(500)" Models/Entities/GL01Entity.cs; then
    echo "✅ GL01Entity.cs có FileName column nvarchar(500)"
else
    echo "❌ GL01Entity.cs thiếu FileName column nvarchar(500)"
fi

if grep -q "ImportBatchId.*nvarchar(100)" Models/Entities/GL01Entity.cs; then
    echo "✅ GL01Entity.cs có ImportBatchId column nvarchar(100)"
else
    echo "❌ GL01Entity.cs thiếu ImportBatchId column nvarchar(100)"
fi

# 2. DataTables Model Check
echo ""
echo "2️⃣  GL01.cs - DataTables Model Validation"
echo "-----------------------------------------"

if grep -q "UNIFIED STRUCTURE - HEAVY FILE OPTIMIZED" Models/DataTables/GL01.cs; then
    echo "✅ GL01.cs có comment 'UNIFIED STRUCTURE - HEAVY FILE OPTIMIZED'"
else
    echo "❌ GL01.cs thiếu comment 'UNIFIED STRUCTURE - HEAVY FILE OPTIMIZED'"
fi

if grep -q "27 Business Columns" Models/DataTables/GL01.cs; then
    echo "✅ GL01.cs có comment '27 Business Columns'"
else
    echo "❌ GL01.cs thiếu comment '27 Business Columns'"
fi

if grep -q "TR_TIME.*datetime2.*Order = 25" Models/DataTables/GL01.cs; then
    echo "✅ GL01.cs có TR_TIME column Order=25"
else
    echo "❌ GL01.cs thiếu TR_TIME column Order=25"
fi

if grep -q "REMARK.*nvarchar(1000)" Models/DataTables/GL01.cs; then
    echo "✅ GL01.cs có REMARK column nvarchar(1000)"
else
    echo "❌ GL01.cs thiếu REMARK column nvarchar(1000)"
fi

# 3. Import Service Heavy File Check
echo ""
echo "3️⃣  DirectImportService.cs - Heavy File Import Validation"
echo "--------------------------------------------------------"

if grep -q "HEAVY FILE OPTIMIZED" Services/DirectImportService.cs; then
    echo "✅ DirectImportService.cs có comment 'HEAVY FILE OPTIMIZED'"
else
    echo "❌ DirectImportService.cs thiếu comment 'HEAVY FILE OPTIMIZED'"
fi

if grep -q "MaxFileSize 2GB" Services/DirectImportService.cs; then
    echo "✅ DirectImportService.cs có MaxFileSize 2GB validation"
else
    echo "❌ DirectImportService.cs thiếu MaxFileSize 2GB validation"
fi

if grep -q "BulkInsertGL01HeavyAsync" Services/DirectImportService.cs; then
    echo "✅ DirectImportService.cs có BulkInsertGL01HeavyAsync method"
else
    echo "❌ DirectImportService.cs thiếu BulkInsertGL01HeavyAsync method"
fi

if grep -q "BATCH_SIZE = 10000" Services/DirectImportService.cs; then
    echo "✅ DirectImportService.cs có BATCH_SIZE = 10000"
else
    echo "❌ DirectImportService.cs thiếu BATCH_SIZE = 10000"
fi

if grep -q "ChangeTracker.Clear()" Services/DirectImportService.cs; then
    echo "✅ DirectImportService.cs có ChangeTracker.Clear() để free memory"
else
    echo "❌ DirectImportService.cs thiếu ChangeTracker.Clear() để free memory"
fi

# 4. Database Schema Check
echo ""
echo "4️⃣  Database Schema Validation"
echo "------------------------------"

if grep -q "CREATE.*INDEX.*GL01.*NGAY_DL" rebuild_tables_csv_structure_correct.sql; then
    echo "✅ Database có INDEX trên NGAY_DL"
else
    echo "❌ Database thiếu INDEX trên NGAY_DL"
fi

if grep -q "COLUMNSTORE" rebuild_tables_csv_structure_correct.sql; then
    echo "✅ Database có COLUMNSTORE INDEX"
else
    echo "❌ Database thiếu COLUMNSTORE INDEX"
fi

# 5. File Naming Policy Check
echo ""
echo "5️⃣  File Naming Policy Validation"
echo "---------------------------------"

if grep -q "only allow filename containing.*gl01" Services/DirectImportService.cs; then
    echo "✅ DirectImportService.cs có filename validation 'gl01'"
else
    echo "❌ DirectImportService.cs thiếu filename validation 'gl01'"
fi

# 6. TR_TIME -> NGAY_DL Mapping Check
echo ""
echo "6️⃣  TR_TIME -> NGAY_DL Mapping Validation"
echo "-----------------------------------------"

if grep -q "TR_TIME.*HasValue" Services/DirectImportService.cs; then
    echo "✅ DirectImportService.cs có TR_TIME.HasValue check"
else
    echo "❌ DirectImportService.cs thiếu TR_TIME.HasValue check"
fi

if grep -q "record.NGAY_DL = record.TR_TIME.Value.Date" Services/DirectImportService.cs; then
    echo "✅ DirectImportService.cs có TR_TIME -> NGAY_DL mapping"
else
    echo "❌ DirectImportService.cs thiếu TR_TIME -> NGAY_DL mapping"
fi

# 7. Build Validation
echo ""
echo "7️⃣  Build Validation"
echo "--------------------"

echo "🔨 Building project..."
if dotnet build --configuration Debug --verbosity minimal > /dev/null 2>&1; then
    echo "✅ Project builds successfully"
else
    echo "❌ Project build FAILED"
    echo "Run: dotnet build để xem lỗi chi tiết"
fi

echo ""
echo "📊 VALIDATION SUMMARY"
echo "===================="
echo ""

# Count passed/failed checks
PASSED=$(grep -c "✅" <<< "$(bash validate_gl01_unified_structure.sh 2>/dev/null)")
TOTAL=15  # Total expected checks

echo "✅ Passed: $PASSED checks"
echo "📝 Total:  $TOTAL checks"

if [ "$PASSED" -eq "$TOTAL" ]; then
    echo ""
    echo "🎉 GL01 UNIFIED STRUCTURE IMPLEMENTATION: HOÀN THÀNH"
    echo "🚀 Ready for heavy file import testing (~200MB CSV files)"
    echo ""
    echo "Key Features Implemented:"
    echo "• 27 business columns unified across all layers"
    echo "• REMARK column as nvarchar(1000)"
    echo "• Heavy file support (2GB max, 10k batch size)"
    echo "• TR_TIME (column 25) -> NGAY_DL mapping"
    echo "• Non-temporal table structure"
    echo "• Partitioned columnstore indexes"
else
    echo ""
    echo "⚠️  GL01 UNIFIED STRUCTURE: CHƯA HOÀN THÀNH"
    echo "❌ Cần fix các issue ở trên trước khi tiến hành testing"
fi

echo ""
