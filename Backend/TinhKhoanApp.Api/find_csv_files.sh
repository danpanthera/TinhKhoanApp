#!/bin/bash

echo "🔍 TÌM KIẾM VÀ PHÂN TÍCH CSV FILES GỐC"
echo "======================================"
echo "$(date '+%Y-%m-%d %H:%M:%S') - Bắt đầu tìm kiếm CSV files"
echo ""

# Tìm kiếm CSV files trong các thư mục có thể
echo "📂 Tìm kiếm CSV files trong các thư mục..."

# Các đường dẫn có thể chứa CSV files
search_paths=(
    "/Users/nguyendat/Documents/DuLieuImport/DuLieuMau"
    "/Users/nguyendat/Documents/DuLieuImport"
    "/Users/nguyendat/Documents/Projects/TinhKhoanApp"
    "/Users/nguyendat/Downloads"
    "/Users/nguyendat/Desktop"
    "$HOME/Documents"
)

found_files=()

for path in "${search_paths[@]}"; do
    if [ -d "$path" ]; then
        echo "🔍 Checking: $path"
        csv_files=$(find "$path" -name "*.csv" -type f 2>/dev/null | head -20)
        if [ ! -z "$csv_files" ]; then
            echo "✅ Found CSV files in: $path"
            while IFS= read -r file; do
                if [[ "$file" =~ (dp01|dpda|ei01|gl01|gl41|ln01|ln03|rr01) ]]; then
                    echo "  📄 Relevant: $(basename "$file")"
                    found_files+=("$file")
                fi
            done <<< "$csv_files"
        fi
    else
        echo "❌ Directory not found: $path"
    fi
done

echo ""
echo "📋 SUMMARY OF FOUND CSV FILES:"
echo "=============================="

if [ ${#found_files[@]} -eq 0 ]; then
    echo "❌ No relevant CSV files found!"
    echo ""
    echo "💡 SUGGESTION:"
    echo "=============="
    echo "1. Kiểm tra thư mục /Users/nguyendat/Documents/DuLieuImport/DuLieuMau"
    echo "2. Hoặc cung cấp đường dẫn chính xác đến CSV files"
    echo "3. CSV files cần có pattern: *dp01*, *dpda*, *ei01*, *gl01*, *gl41*, *ln01*, *ln03*, *rr01*"
    echo ""
    echo "🔧 NEXT STEPS WITHOUT CSV:"
    echo "=========================="
    echo "1. Tạo mock table structure dựa trên documented columns"
    echo "2. Hoặc extract structure từ existing data patterns"
    echo "3. Manual recreation với proper column names"
else
    echo "✅ Found ${#found_files[@]} relevant CSV files:"
    for file in "${found_files[@]}"; do
        echo "  📄 $file"
    done

    echo ""
    echo "🔍 ANALYZING FIRST FEW FILES:"
    echo "============================="

    for file in "${found_files[@]:0:3}"; do
        echo ""
        echo "📊 File: $(basename "$file")"
        echo "Size: $(ls -lh "$file" | awk '{print $5}')"
        echo "First line (headers):"
        head -1 "$file" 2>/dev/null | cut -c1-200
        echo ""
        echo "Column count: $(head -1 "$file" 2>/dev/null | tr ',' '\n' | wc -l | tr -d ' ')"
        echo "---"
    done
fi

echo ""
echo "✅ Search completed $(date '+%H:%M:%S')"
