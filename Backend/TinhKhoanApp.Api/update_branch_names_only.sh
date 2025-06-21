#!/bin/bash

# Update branch names in the database (names only)

# 1. Agribank CN Lai Châu (7800) -> Chi nhánh Lai Châu
curl -X PUT "http://localhost:5055/api/Units/4398" \
  -H "Content-Type: application/json" \
  -d '{"id": 4398, "name": "Chi nhánh Lai Châu"}'

# 2. Agribank CN H. Tam Đường -> Chi nhánh Tam Đường
curl -X PUT "http://localhost:5055/api/Units/1002" \
  -H "Content-Type: application/json" \
  -d '{"id": 1002, "name": "Chi nhánh Tam Đường"}'

# 3. Agribank CN H. Phong Thổ -> Chi nhánh Phong Thổ
curl -X PUT "http://localhost:5055/api/Units/1003" \
  -H "Content-Type: application/json" \
  -d '{"id": 1003, "name": "Chi nhánh Phong Thổ"}'

# 4. Agribank CN H. Sìn Hồ -> Chi nhánh Sìn Hồ
curl -X PUT "http://localhost:5055/api/Units/1004" \
  -H "Content-Type: application/json" \
  -d '{"id": 1004, "name": "Chi nhánh Sìn Hồ"}'

# 5. Agribank CN H. Mường Tè -> Chi nhánh Mường Tè
curl -X PUT "http://localhost:5055/api/Units/1005" \
  -H "Content-Type: application/json" \
  -d '{"id": 1005, "name": "Chi nhánh Mường Tè"}'

# 6. Agribank CN H. Than Uyên -> Chi nhánh Than Uyên
curl -X PUT "http://localhost:5055/api/Units/1006" \
  -H "Content-Type: application/json" \
  -d '{"id": 1006, "name": "Chi nhánh Than Uyên"}'

# 7. Agribank CN Thành Phố -> Chi nhánh Thành Phố
curl -X PUT "http://localhost:5055/api/Units/1007" \
  -H "Content-Type: application/json" \
  -d '{"id": 1007, "name": "Chi nhánh Thành Phố"}'

# 8. Agribank CN H. Tân Uyên -> Chi nhánh Tân Uyên
curl -X PUT "http://localhost:5055/api/Units/1008" \
  -H "Content-Type: application/json" \
  -d '{"id": 1008, "name": "Chi nhánh Tân Uyên"}'

# 9. Agribank CN H. Nậm Nhùn -> Chi nhánh Nậm Nhùn
curl -X PUT "http://localhost:5055/api/Units/1009" \
  -H "Content-Type: application/json" \
  -d '{"id": 1009, "name": "Chi nhánh Nậm Nhùn"}'

# 10. Update main branch: Agribank CN Lai Châu -> Chi nhánh Lai Châu
curl -X PUT "http://localhost:5055/api/Units/1" \
  -H "Content-Type: application/json" \
  -d '{"id": 1, "name": "Chi nhánh Lai Châu"}'

echo "Updated all branch names in the database"
