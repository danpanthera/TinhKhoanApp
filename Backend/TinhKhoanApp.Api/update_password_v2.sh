#!/bin/bash

# Tạo password hash mới cho "admin123"
# Sử dụng cost factor thấp hơn để đảm bảo tương thích
HASH='$2a$11$2G.3I4HWHwwTGF3Ey3JME.9iYbRvYJGvY6KYgJ6UZFfJ/qR7a8aBq'

# Update với hash mới
sqlite3 TinhKhoanDB.db "UPDATE Employees SET PasswordHash = '$HASH' WHERE Username = 'admin';"

echo "✅ Updated admin password hash"
echo "📝 New hash: $HASH"

# Verify update
echo "🔍 Current admin record:"
sqlite3 TinhKhoanDB.db "SELECT Username, PasswordHash FROM Employees WHERE Username = 'admin';"
