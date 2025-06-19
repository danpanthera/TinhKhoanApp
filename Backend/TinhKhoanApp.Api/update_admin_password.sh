#!/bin/bash

# Tạo password hash cho admin123
# BCrypt hash cho "admin123" với cost 12
HASH='$2a$12$vKz7.aDSVJgTUQc4MfN8tOqvOJKEu4WnGQMVYdCEYHxGKEQQJ/g4m'

# Update admin user với password hash
sqlite3 TinhKhoanDB.db "UPDATE Employees SET PasswordHash = '$HASH' WHERE Username = 'admin';"

echo "✅ Admin password updated successfully!"
echo "👤 Username: admin"
echo "🔑 Password: admin123"
echo ""
echo "📝 Checking update:"
sqlite3 TinhKhoanDB.db "SELECT Username, PasswordHash FROM Employees WHERE Username = 'admin';"
