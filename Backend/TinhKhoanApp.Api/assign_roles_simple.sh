#!/bin/bash

echo "🎯 GÁN ROLES CHO EMPLOYEES"
echo "========================="

BASE_URL="http://localhost:5055/api"

# Employee 2: Nguyễn Văn An → Role 18
echo "Employee 2: Nguyễn Văn An → Role 18 (Phó giám đốc CNL2 phụ trách TD)"
curl -s "$BASE_URL/employees/2" | jq '{Id: .Id, EmployeeCode: .EmployeeCode, CBCode: .CBCode, FullName: .FullName, Username: .Username, Email: .Email, PhoneNumber: .PhoneNumber, IsActive: .IsActive, UnitId: .UnitId, PositionId: .PositionId, RoleIds: [18]}' > payload.json
curl -s -X PUT "$BASE_URL/employees/2" -H "Content-Type: application/json" -d @payload.json | jq '{Result: "Success", FullName: .FullName, Roles: (.Roles // [])}'

echo ""

# Employee 3: Trần Thị Bình → Role 12
echo "Employee 3: Trần Thị Bình → Role 12 (Trưởng phó IT | Tổng hợp | KTGS)"
curl -s "$BASE_URL/employees/3" | jq '{Id: .Id, EmployeeCode: .EmployeeCode, CBCode: .CBCode, FullName: .FullName, Username: .Username, Email: .Email, PhoneNumber: .PhoneNumber, IsActive: .IsActive, UnitId: .UnitId, PositionId: .PositionId, RoleIds: [12]}' > payload.json
curl -s -X PUT "$BASE_URL/employees/3" -H "Content-Type: application/json" -d @payload.json | jq '{Result: "Success", FullName: .FullName, Roles: (.Roles // [])}'

echo ""

# Employee 4: Lê Văn Cường → Role 1
echo "Employee 4: Lê Văn Cường → Role 1 (Trưởng phòng KHDN)"
curl -s "$BASE_URL/employees/4" | jq '{Id: .Id, EmployeeCode: .EmployeeCode, CBCode: .CBCode, FullName: .FullName, Username: .Username, Email: .Email, PhoneNumber: .PhoneNumber, IsActive: .IsActive, UnitId: .UnitId, PositionId: .PositionId, RoleIds: [1]}' > payload.json
curl -s -X PUT "$BASE_URL/employees/4" -H "Content-Type: application/json" -d @payload.json | jq '{Result: "Success", FullName: .FullName, Roles: (.Roles // [])}'

echo ""

# Employee 5: Phạm Thị Dung → Role 2
echo "Employee 5: Phạm Thị Dung → Role 2 (Trưởng phòng KHCN)"
curl -s "$BASE_URL/employees/5" | jq '{Id: .Id, EmployeeCode: .EmployeeCode, CBCode: .CBCode, FullName: .FullName, Username: .Username, Email: .Email, PhoneNumber: .PhoneNumber, IsActive: .IsActive, UnitId: .UnitId, PositionId: .PositionId, RoleIds: [2]}' > payload.json
curl -s -X PUT "$BASE_URL/employees/5" -H "Content-Type: application/json" -d @payload.json | jq '{Result: "Success", FullName: .FullName, Roles: (.Roles // [])}'

echo ""

# Employee 6: Hoàng Văn Em → Role 8
echo "Employee 6: Hoàng Văn Em → Role 8 (Trưởng phòng KTNQ CNL1)"
curl -s "$BASE_URL/employees/6" | jq '{Id: .Id, EmployeeCode: .EmployeeCode, CBCode: .CBCode, FullName: .FullName, Username: .Username, Email: .Email, PhoneNumber: .PhoneNumber, IsActive: .IsActive, UnitId: .UnitId, PositionId: .PositionId, RoleIds: [8]}' > payload.json
curl -s -X PUT "$BASE_URL/employees/6" -H "Content-Type: application/json" -d @payload.json | jq '{Result: "Success", FullName: .FullName, Roles: (.Roles // [])}'

echo ""

# Clean up
rm -f payload.json

echo "🏁 HOÀN THÀNH GÁN ROLES"
