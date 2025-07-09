#!/bin/bash

echo "🗂️ Tạo migration để drop ImportedDataItems table..."

# Tạo migration mới
dotnet ef migrations add DropImportedDataItemsTable --context ApplicationDbContext

echo "📋 Migration đã được tạo. Hãy kiểm tra file migration trước khi apply!"
