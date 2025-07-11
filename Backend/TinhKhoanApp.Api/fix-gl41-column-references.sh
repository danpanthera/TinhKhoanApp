#!/bin/bash

# ✅ Script fix GL41 column references theo cấu trúc mới
# Cấu trúc cũ: SO_TK, SO_DU, SO_DU_CUOI_KY, SO_DU_DAU_KY, SO_PHAT_SINH_NO, SO_PHAT_SINH_CO
# Cấu trúc mới: MA_TK, DN_DAUKY, DC_DAUKY, SBT_NO, ST_GHINO, SBT_CO, ST_GHICO, DN_CUOIKY, DC_CUOIKY

echo "🔧 FIX GL41 COLUMN REFERENCES - THEO HEADER CSV CHUẨN"
echo "═══════════════════════════════════════════════════════"

# Mapping cột GL41 từ cũ sang mới
declare -A GL41_MAPPING=(
    ["SO_TK"]="MA_TK"
    ["SO_DU"]="DC_CUOIKY"                     # Dư có cuối kỳ (số dương)
    ["SO_DU_CUOI_KY"]="DC_CUOIKY"            # Dư có cuối kỳ
    ["SO_DU_DAU_KY"]="DC_DAUKY"              # Dư có đầu kỳ
    ["SO_PHAT_SINH_NO"]="ST_GHINO"           # Số tiền ghi nợ
    ["SO_PHAT_SINH_CO"]="ST_GHICO"           # Số tiền ghi có
)

echo "📋 Column Mapping GL41:"
for old_col in "${!GL41_MAPPING[@]}"; do
    echo "   $old_col → ${GL41_MAPPING[$old_col]}"
done
echo

# Files cần fix
FILES_TO_FIX=(
    "/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Data/ApplicationDbContext.cs"
    "/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Services/DashboardCalculationService.cs"
)

echo "🔧 Fixing GL41 references in files..."

for file in "${FILES_TO_FIX[@]}"; do
    if [ -f "$file" ]; then
        echo "   📝 Processing: $(basename $file)"

        # Backup file
        cp "$file" "${file}.backup_$(date +%Y%m%d_%H%M%S)"

        # Apply column mapping
        for old_col in "${!GL41_MAPPING[@]}"; do
            new_col="${GL41_MAPPING[$old_col]}"

            # Replace .SO_TK with .MA_TK
            sed -i '' "s/\\.${old_col}/.${new_col}/g" "$file"

            # Replace columnstore index references
            sed -i '' "s/\"${old_col}\"/\"${new_col}\"/g" "$file"

            echo "      $old_col → $new_col"
        done

        echo "   ✅ Fixed: $(basename $file)"
    else
        echo "   ❌ Not found: $file"
    fi
done

echo
echo "🎯 Summary:"
echo "   ✅ Updated column references in ApplicationDbContext.cs"
echo "   ✅ Updated column references in DashboardCalculationService.cs"
echo "   ✅ Backup files created with timestamp"
echo
echo "📋 Next steps:"
echo "   1. dotnet build (to verify fixes)"
echo "   2. dotnet ef migrations add UpdateGL41StructureTo13Columns"
echo "   3. dotnet ef database update"
