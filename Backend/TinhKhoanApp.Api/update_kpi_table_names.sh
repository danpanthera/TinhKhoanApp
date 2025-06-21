#!/bin/bash

# Update KPI table names

# 1. Chi nhánh H. Tam Đường (7801) -> Chi nhánh Tam Đường
curl -X PUT "http://localhost:5055/api/KpiAssignment/tables/1058" \
  -H "Content-Type: application/json" \
  -d '{"id": 1058, "tableName": "Chi nhánh Tam Đường", "description": "Bảng giao khoán KPI cho Chi nhánh Tam Đường", "category": "Dành cho Chi nhánh", "isActive": true}'

# 2. Chi nhánh H. Phong Thổ (7802) -> Chi nhánh Phong Thổ
curl -X PUT "http://localhost:5055/api/KpiAssignment/tables/1059" \
  -H "Content-Type: application/json" \
  -d '{"id": 1059, "tableName": "Chi nhánh Phong Thổ", "description": "Bảng giao khoán KPI cho Chi nhánh Phong Thổ", "category": "Dành cho Chi nhánh", "isActive": true}'

# 3. Chi nhánh H. Sìn Hồ -> Chi nhánh Sìn Hồ
curl -X PUT "http://localhost:5055/api/KpiAssignment/tables/1060" \
  -H "Content-Type: application/json" \
  -d '{"id": 1060, "tableName": "Chi nhánh Sìn Hồ", "description": "Bảng giao khoán KPI cho Chi nhánh Sìn Hồ", "category": "Dành cho Chi nhánh", "isActive": true}'

# 4. Chi nhánh H. Mường Tè -> Chi nhánh Mường Tè
curl -X PUT "http://localhost:5055/api/KpiAssignment/tables/1061" \
  -H "Content-Type: application/json" \
  -d '{"id": 1061, "tableName": "Chi nhánh Mường Tè", "description": "Bảng giao khoán KPI cho Chi nhánh Mường Tè", "category": "Dành cho Chi nhánh", "isActive": true}'

# 5. Chi nhánh H. Than Uyên -> Chi nhánh Than Uyên
curl -X PUT "http://localhost:5055/api/KpiAssignment/tables/1062" \
  -H "Content-Type: application/json" \
  -d '{"id": 1062, "tableName": "Chi nhánh Than Uyên", "description": "Bảng giao khoán KPI cho Chi nhánh Than Uyên", "category": "Dành cho Chi nhánh", "isActive": true}'

# 6. Chi nhánh H. Tân Uyên -> Chi nhánh Tân Uyên
curl -X PUT "http://localhost:5055/api/KpiAssignment/tables/1064" \
  -H "Content-Type: application/json" \
  -d '{"id": 1064, "tableName": "Chi nhánh Tân Uyên", "description": "Bảng giao khoán KPI cho Chi nhánh Tân Uyên", "category": "Dành cho Chi nhánh", "isActive": true}'

# 7. Chi nhánh H. Nậm Nhùn -> Chi nhánh Nậm Nhùn
curl -X PUT "http://localhost:5055/api/KpiAssignment/tables/1065" \
  -H "Content-Type: application/json" \
  -d '{"id": 1065, "tableName": "Chi nhánh Nậm Nhùn", "description": "Bảng giao khoán KPI cho Chi nhánh Nậm Nhùn", "category": "Dành cho Chi nhánh", "isActive": true}'

echo "Updated all KPI table names"
