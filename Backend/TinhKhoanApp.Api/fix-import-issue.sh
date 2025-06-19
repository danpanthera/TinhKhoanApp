#!/bin/bash

echo "🔧 GIẢI PHÁP FIX LỖI IMPORT"
echo "=========================="

echo ""
echo "📋 Vấn đề hiện tại:"
echo "   - Backend báo lỗi: 'An error occurred while saving the entity changes'"
echo "   - Database SQL Server không kết nối được"
echo "   - Import function không hoạt động"

echo ""
echo "🎯 Các giải pháp có thể áp dụng:"
echo ""

echo "1. 🔄 QUICK FIX - Sử dụng SQLite thay vì SQL Server:"
echo "   - Sửa connection string trong appsettings.json"
echo "   - Chạy migration với SQLite"
echo "   - Đơn giản và nhanh chóng cho development"

echo ""
echo "2. 🗄️ SETUP SQL SERVER:"
echo "   - Cài đặt và khởi động SQL Server"
echo "   - Tạo database TinhKhoanDB"
echo "   - Chạy migration để tạo tables"

echo ""
echo "3. 🧪 TEST MODE - Import không lưu database:"
echo "   - Tạo test endpoint chỉ parse file"
echo "   - Trả về kết quả mà không lưu"
echo "   - Tốt cho testing logic import"

echo ""
echo "4. 💾 IN-MEMORY DATABASE:"
echo "   - Sử dụng InMemory database cho test"
echo "   - Không cần setup database external"
echo "   - Tốt cho development và test"

echo ""
echo "🚀 RECOMMENDED: Áp dụng giải pháp #1 (SQLite)"
echo ""

echo "Bạn muốn áp dụng giải pháp nào?"
echo "1) SQLite (Recommended)"
echo "2) Test Mode (No Database)"
echo "3) In-Memory Database"
echo "4) Setup SQL Server"
echo ""

read -p "Chọn (1-4): " choice

case $choice in
    1)
        echo "🔄 Đang chuyển sang SQLite..."
        echo "Backup appsettings.json..."
        cp appsettings.json appsettings.json.backup
        
        echo "Update connection string..."
        cat > appsettings.json << 'EOF'
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=TinhKhoanDB.db"
  },
  "Jwt": {
    "Key": "TinhKhoanAppSuperSecretKey2025!@#",
    "Issuer": "TinhKhoanApp"
  }
}
EOF
        
        echo "✅ Updated connection string to SQLite"
        echo "📦 Installing SQLite package..."
        dotnet add package Microsoft.EntityFrameworkCore.Sqlite
        
        echo "🔄 Running migration..."
        dotnet ef database update
        
        echo "✅ SQLite setup completed!"
        ;;
    2)
        echo "🧪 Setting up Test Mode..."
        echo "TestImportController already created"
        echo "✅ Use /api/TestImport endpoints for testing"
        ;;
    3)
        echo "💾 Setting up In-Memory Database..."
        echo "This requires code changes in Program.cs"
        ;;
    4)
        echo "🗄️ SQL Server setup instructions:"
        echo "1. Install SQL Server"
        echo "2. Create database: CREATE DATABASE TinhKhoanDB"
        echo "3. Run: dotnet ef database update"
        ;;
    *)
        echo "❌ Invalid choice"
        ;;
esac

echo ""
echo "🔧 Next steps:"
echo "1. Restart backend: dotnet run --urls='http://localhost:5055'"
echo "2. Test import with frontend or debug page"
echo "3. Check backend logs for any issues"
