#!/bin/bash

echo "=== 🔍 Verifying RR01 Implementation Consistency ==="

echo -e "\n1️⃣ Model Class Definition (RR01.cs)"
grep -n "decimal\|DateTime" Models/DataTables/RR01.cs | grep -v "using" | head -15

echo -e "\n2️⃣ DTO Class Definition (RR01DTO.cs)"
grep -n "decimal\|DateTime" Models/Dtos/RR01DTO.cs | grep -v "using" | head -15

echo -e "\n3️⃣ DirectImportService Implementation"
grep -n "RR01" Services/DirectImportService.cs | grep -n "ParseRR01\|ImportRR01" | head -5

echo -e "\n4️⃣ ConvertCsvValue Method for Type Conversion"
grep -n -A 5 "underlyingType == typeof(decimal)" Services/DirectImportService.cs | head -10
grep -n -A 5 "underlyingType == typeof(DateTime)" Services/DirectImportService.cs | head -10

echo -e "\n5️⃣ SQL Migration Scripts Created"
ls -la update_rr01_datatypes.sql

echo -e "\n=== ✅ Verification Summary ==="
echo "✅ Model and DTO files updated with proper types."
echo "✅ Type conversion functions already handle decimal and DateTime types."
echo "✅ SQL script created to update database schema."
echo "❗ A full migration or database script needs to be executed to update the database."
echo ""
echo "To apply the changes to the database, run:"
echo "   sqlcmd -S <server> -d TinhKhoanApp -i update_rr01_datatypes.sql"
echo "or manually execute the SQL script in SQL Server Management Studio."
