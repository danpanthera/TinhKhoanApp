#!/bin/bash

# Script để đánh dấu các legacy methods trong DataImportController
echo "🔄 Đánh dấu legacy methods trong DataImportController.cs..."

# Backup current file
cp Controllers/DataImportController.cs Controllers/DataImportController.cs.before_legacy_mark

# Sử dụng sed để thêm [Obsolete] attribute cho các methods legacy
sed -i.bak '
/private.*ImportedDataItem/i\
        [Obsolete("Legacy method - Use DirectImportService instead")]
/public.*ImportedDataItem/i\
        [Obsolete("Legacy method - Use DirectImportService instead")]
' Controllers/DataImportController.cs

echo "✅ Đã đánh dấu legacy methods với [Obsolete] attribute"
