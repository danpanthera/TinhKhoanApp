#!/bin/bash

# Script tạo dữ liệu mẫu cho frontend testing
BASE_URL="http://localhost:5055/api"

echo "🚀 Bắt đầu tạo dữ liệu mẫu..."

# Tạo thêm Positions
echo "📋 Tạo thêm Positions..."
curl -X POST "$BASE_URL/Positions" -H "Content-Type: application/json" -d '{"name": "Giao dịch viên", "code": "GDV", "description": "Giao dịch viên chi nhánh", "level": 6}'
curl -X POST "$BASE_URL/Positions" -H "Content-Type: application/json" -d '{"name": "Thủ quỹ", "code": "TQ", "description": "Thủ quỹ chi nhánh", "level": 7}'
curl -X POST "$BASE_URL/Positions" -H "Content-Type: application/json" -d '{"name": "Chuyên viên", "code": "CV", "description": "Chuyên viên nghiệp vụ", "level": 8}'

# Tạo thêm Employees
echo "👥 Tạo thêm Employees..."
curl -X POST "$BASE_URL/Employees" -H "Content-Type: application/json" -d '{"fullName": "Lê Văn C", "employeeCode": "LVC003", "cbCode": "CB003", "username": "levanc", "email": "levanc@agribank.com", "unitId": 3, "positionId": 3, "roleId": 1}'
curl -X POST "$BASE_URL/Employees" -H "Content-Type: application/json" -d '{"fullName": "Phạm Thị D", "employeeCode": "PTD004", "cbCode": "CB004", "username": "phamthid", "email": "phamthid@agribank.com", "unitId": 4, "positionId": 4, "roleId": 1}'
curl -X POST "$BASE_URL/Employees" -H "Content-Type: application/json" -d '{"fullName": "Hoàng Văn E", "employeeCode": "HVE005", "cbCode": "CB005", "username": "hoangvane", "email": "hoangvane@agribank.com", "unitId": 5, "positionId": 5, "roleId": 1}'
curl -X POST "$BASE_URL/Employees" -H "Content-Type: application/json" -d '{"fullName": "Ngô Thị F", "employeeCode": "NTF006", "cbCode": "CB006", "username": "ngothif", "email": "ngothif@agribank.com", "unitId": 10, "positionId": 6, "roleId": 1}'
curl -X POST "$BASE_URL/Employees" -H "Content-Type: application/json" -d '{"fullName": "Vũ Văn G", "employeeCode": "VVG007", "cbCode": "CB007", "username": "vuvang", "email": "vuvang@agribank.com", "unitId": 11, "positionId": 7, "roleId": 1}'

# Tạo thêm KhoanPeriods
echo "📅 Tạo thêm KhoanPeriods..."
curl -X POST "$BASE_URL/KhoanPeriods" -H "Content-Type: application/json" -d '{"name": "Kỳ khoán Quý 2/2025", "type": 1, "startDate": "2025-04-01T00:00:00Z", "endDate": "2025-06-30T23:59:59Z", "status": 1}'
curl -X POST "$BASE_URL/KhoanPeriods" -H "Content-Type: application/json" -d '{"name": "Kỳ khoán Năm 2025", "type": 2, "startDate": "2025-01-01T00:00:00Z", "endDate": "2025-12-31T23:59:59Z", "status": 0}'
curl -X POST "$BASE_URL/KhoanPeriods" -H "Content-Type: application/json" -d '{"name": "Kỳ khoán Tháng 8/2025", "type": 0, "startDate": "2025-08-01T00:00:00Z", "endDate": "2025-08-31T23:59:59Z", "status": 0}'

echo "✅ Hoàn tất tạo dữ liệu mẫu!"

echo "📊 Kiểm tra kết quả:"
echo "Units: $(curl -s "$BASE_URL/Units" | jq '. | length') items"
echo "Positions: $(curl -s "$BASE_URL/Positions" | jq '. | length') items"
echo "Employees: $(curl -s "$BASE_URL/Employees" | jq '. | length') items"
echo "KhoanPeriods: $(curl -s "$BASE_URL/KhoanPeriods" | jq '. | length') items"
