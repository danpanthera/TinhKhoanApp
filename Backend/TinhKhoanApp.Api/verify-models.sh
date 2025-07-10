#!/bin/bash

# ===================================================================
# SCRIPT VERIFY CÁC MODEL KHỚP VỚI HEADER CSV GỐC
# Kiểm tra từng model có đúng số cột, tên cột theo header không
# ===================================================================

echo "🔍 Kiểm tra các model có khớp với header CSV gốc không..."

MODELS_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Models/DataTables"

# === FUNCTION ĐẾM SỐ COLUMN TRONG MODEL ===
count_columns() {
    local model_file="$1"
    local count=$(grep -c "public.*{.*get.*set.*}" "$model_file")
    echo $count
}

# === FUNCTION LẤY DANH SÁCH COLUMN TỪ MODEL ===
list_columns() {
    local model_file="$1"
    echo "📋 Columns trong $(basename $model_file):"
    grep -o '\[Column("[^"]*")\]' "$model_file" | grep -v "CREATED_DATE\|UPDATED_DATE\|FILE_NAME" | sort
    echo ""
}

echo "=== KIỂM TRA CÁC MODEL CHÍNH ==="

# === 1. DP01 (55 cột) ===
echo "🔹 DP01.cs - Expect: 55 cột"
dp01_count=$(count_columns "$MODELS_DIR/DP01.cs")
echo "   Thực tế: $dp01_count cột"
if [[ $dp01_count -eq 55 ]]; then
    echo "   ✅ ĐÚNG số cột"
else
    echo "   ❌ SAI số cột"
fi

# === 2. DPDA (13 cột) ===
echo "🔹 DPDA.cs - Expect: 13 cột"
dpda_count=$(count_columns "$MODELS_DIR/DPDA.cs")
echo "   Thực tế: $dpda_count cột"
if [[ $dpda_count -eq 13 ]]; then
    echo "   ✅ ĐÚNG số cột"
else
    echo "   ❌ SAI số cột"
fi

# === 3. EI01 (24 cột) ===
echo "🔹 EI01.cs - Expect: 24 cột"
ei01_count=$(count_columns "$MODELS_DIR/EI01.cs")
echo "   Thực tế: $ei01_count cột"
if [[ $ei01_count -eq 24 ]]; then
    echo "   ✅ ĐÚNG số cột"
else
    echo "   ❌ SAI số cột"
fi

# === 4. GL01 (27 cột) ===
echo "🔹 GL01.cs - Expect: 27 cột"
gl01_count=$(count_columns "$MODELS_DIR/GL01.cs")
echo "   Thực tế: $gl01_count cột"
if [[ $gl01_count -eq 27 ]]; then
    echo "   ✅ ĐÚNG số cột"
else
    echo "   ❌ SAI số cột"
fi

# === 5. KH03 (38 cột) ===
echo "🔹 KH03.cs - Expect: 38 cột"
kh03_count=$(count_columns "$MODELS_DIR/KH03.cs")
echo "   Thực tế: $kh03_count cột"
if [[ $kh03_count -eq 38 ]]; then
    echo "   ✅ ĐÚNG số cột"
else
    echo "   ❌ SAI số cột"
fi

# === 6. LN01 (67 cột) ===
echo "🔹 LN01.cs - Expect: 67 cột"
ln01_count=$(count_columns "$MODELS_DIR/LN01.cs")
echo "   Thực tế: $ln01_count cột"
if [[ $ln01_count -eq 67 ]]; then
    echo "   ✅ ĐÚNG số cột"
else
    echo "   ❌ SAI số cột"
fi

# === 7. LN02 (11 cột) ===
echo "🔹 LN02.cs - Expect: 11 cột"
ln02_count=$(count_columns "$MODELS_DIR/LN02.cs")
echo "   Thực tế: $ln02_count cột"
if [[ $ln02_count -eq 11 ]]; then
    echo "   ✅ ĐÚNG số cột"
else
    echo "   ❌ SAI số cột"
fi

# === 8. LN03 (17 cột) ===
echo "🔹 LN03.cs - Expect: 17 cột"
ln03_count=$(count_columns "$MODELS_DIR/LN03.cs")
echo "   Thực tế: $ln03_count cột"
if [[ $ln03_count -eq 17 ]]; then
    echo "   ✅ ĐÚNG số cột"
else
    echo "   ❌ SAI số cột"
fi

# === 9. RR01 (25 cột) ===
echo "🔹 RR01.cs - Expect: 25 cột"
rr01_count=$(count_columns "$MODELS_DIR/RR01.cs")
echo "   Thực tế: $rr01_count cột"
if [[ $rr01_count -eq 25 ]]; then
    echo "   ✅ ĐÚNG số cột"
else
    echo "   ❌ SAI số cột"
fi

# === 10. TSDB01 (16 cột) ===
echo "🔹 TSDB01.cs - Expect: 16 cột"
tsdb01_count=$(count_columns "$MODELS_DIR/TSDB01.cs")
echo "   Thực tế: $tsdb01_count cột"
if [[ $tsdb01_count -eq 16 ]]; then
    echo "   ✅ ĐÚNG số cột"
else
    echo "   ❌ SAI số cột"
fi

echo ""
echo "=== DANH SÁCH CÁC COLUMN CHI TIẾT ==="
list_columns "$MODELS_DIR/DP01.cs"
list_columns "$MODELS_DIR/DPDA.cs"
list_columns "$MODELS_DIR/EI01.cs"
list_columns "$MODELS_DIR/GL01.cs"
list_columns "$MODELS_DIR/KH03.cs"
list_columns "$MODELS_DIR/LN01.cs"
list_columns "$MODELS_DIR/LN02.cs"
list_columns "$MODELS_DIR/LN03.cs"
list_columns "$MODELS_DIR/RR01.cs"
list_columns "$MODELS_DIR/TSDB01.cs"

echo "🎯 Hoàn thành kiểm tra!"
