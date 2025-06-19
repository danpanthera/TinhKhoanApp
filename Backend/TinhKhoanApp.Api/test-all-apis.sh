#!/bin/bash

echo "=== KIỂM TRA TRẠNG THÁI BACKEND APIS ==="
echo ""

BASE_URL="http://localhost:5055/api"

echo "1. Testing Units API:"
curl -s -X GET "$BASE_URL/Units" -H "accept: application/json" | head -c 200
echo ""
echo ""

echo "2. Testing Positions API:"
curl -s -X GET "$BASE_URL/Positions" -H "accept: application/json" | head -c 200  
echo ""
echo ""

echo "3. Testing Employees API:"
curl -s -X GET "$BASE_URL/Employees" -H "accept: application/json" | head -c 200
echo ""
echo ""

echo "4. Testing Roles API:"
curl -s -X GET "$BASE_URL/Roles" -H "accept: application/json" | head -c 200
echo ""
echo ""

echo "=== KIỂM TRA DỮ LIỆU TRONG DATABASE ==="
echo ""

cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api

echo "5. Units count:"
sqlite3 TinhKhoanDB.db "SELECT COUNT(*) FROM Units;"

echo "6. Positions count:"  
sqlite3 TinhKhoanDB.db "SELECT COUNT(*) FROM Positions;"

echo "7. Employees count:"
sqlite3 TinhKhoanDB.db "SELECT COUNT(*) FROM Employees;"

echo "8. Roles count:"
sqlite3 TinhKhoanDB.db "SELECT COUNT(*) FROM Roles;"

echo ""
echo "=== CHI TIẾT DỮ LIỆU ==="
echo ""

echo "Units data:"
sqlite3 TinhKhoanDB.db "SELECT Id, UnitCode, UnitName, UnitType FROM Units;"
echo ""

echo "Positions data:"
sqlite3 TinhKhoanDB.db "SELECT Id, Name, Description FROM Positions;"
echo ""

echo "Employees data:"
sqlite3 TinhKhoanDB.db "SELECT Id, EmployeeCode, FullName, UnitId, PositionId FROM Employees;"
echo ""

echo "Roles data (first 5):"
sqlite3 TinhKhoanDB.db "SELECT Id, Name, Description FROM Roles LIMIT 5;"
echo ""

echo "=== HOÀN TẤT KIỂM TRA ==="
