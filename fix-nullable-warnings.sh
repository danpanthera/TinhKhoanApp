#!/bin/bash
# Script tự động sửa các Nullable Reference Types warnings trong C#

echo "🔧 Bắt đầu sửa các Nullable Reference Types warnings..."

# Đường dẫn đến project
PROJECT_PATH="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api"

# Backup trước khi sửa
echo "📋 Tạo backup các files sẽ được sửa..."
find "$PROJECT_PATH" -name "*.cs" -not -path "*/bin/*" -not -path "*/obj/*" -not -path "*/Migrations/*" -exec cp {} {}.backup \;

# Function để sửa nullable warnings trong một file
fix_nullable_in_file() {
    local file="$1"
    echo "🔧 Đang sửa file: $(basename "$file")"
    
    # Sử dụng sed để thêm required cho các property string bắt buộc
    # Tìm các dòng có [Required] và property string không có required/nullable
    sed -i '' -E '/\[Required\]/,/public string [A-Za-z_][A-Za-z0-9_]* \{ get; set; \}/ {
        s/public string ([A-Za-z_][A-Za-z0-9_]*) \{ get; set; \}/public required string \1 { get; set; }/g
    }' "$file"
    
    # Sửa các property string không có [Required] thành nullable
    sed -i '' -E 's/public string ([A-Za-z_][A-Za-z0-9_]*) \{ get; set; \}/public string? \1 { get; set; }/g' "$file"
    
    # Loại bỏ duplicate "required" nếu có
    sed -i '' 's/public required required string/public required string/g' "$file"
    
    # Sửa các property đã có required bị duplicate
    sed -i '' 's/public string\? required/public required string/g' "$file"
}

# Tìm và sửa tất cả files .cs
echo "🔍 Tìm tất cả files C# cần sửa..."
find "$PROJECT_PATH" -name "*.cs" -not -path "*/bin/*" -not -path "*/obj/*" -not -path "*/Migrations/*" | while read -r file; do
    # Kiểm tra nếu file có nullable warning
    if grep -q "public string [A-Za-z_]" "$file"; then
        fix_nullable_in_file "$file"
    fi
done

echo "✅ Hoàn thành sửa nullable reference types!"
echo "📊 Kiểm tra lại bằng cách build project..."
