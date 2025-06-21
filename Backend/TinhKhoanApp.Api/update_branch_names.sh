#!/bin/bash

# Update branch names in the database

# 1. Agribank CN Lai Châu (7800) -> Chi nhánh Lai Châu, code: CnLaiChau
curl -X PUT "http://localhost:5055/api/Units/4398" \
  -H "Content-Type: application/json" \
  -d '{"id": 4398, "code": "CnLaiChau", "name": "Chi nhánh Lai Châu", "type": "CNL1", "parentUnitId": null, "parentUnitName": null}'

# 2. Agribank CN H. Tam Đường -> Chi nhánh Tam Đường, code: CnTamDuong
curl -X PUT "http://localhost:5055/api/Units/1002" \
  -H "Content-Type: application/json" \
  -d '{"id": 1002, "code": "CnTamDuong", "name": "Chi nhánh Tam Đường", "type": "CNL2", "parentUnitId": 1, "parentUnitName": "Chi nhánh Lai Châu"}'

# 3. Agribank CN H. Phong Thổ -> Chi nhánh Phong Thổ, code: CnPhongTho
curl -X PUT "http://localhost:5055/api/Units/1003" \
  -H "Content-Type: application/json" \
  -d '{"id": 1003, "code": "CnPhongTho", "name": "Chi nhánh Phong Thổ", "type": "CNL2", "parentUnitId": 1, "parentUnitName": "Chi nhánh Lai Châu"}'

# 4. Agribank CN H. Sìn Hồ -> Chi nhánh Sìn Hồ, code: CnSinHo
curl -X PUT "http://localhost:5055/api/Units/1004" \
  -H "Content-Type: application/json" \
  -d '{"id": 1004, "code": "CnSinHo", "name": "Chi nhánh Sìn Hồ", "type": "CNL2", "parentUnitId": 1, "parentUnitName": "Chi nhánh Lai Châu"}'

# 5. Agribank CN H. Mường Tè -> Chi nhánh Mường Tè, code: CnMuongTe
curl -X PUT "http://localhost:5055/api/Units/1005" \
  -H "Content-Type: application/json" \
  -d '{"id": 1005, "code": "CnMuongTe", "name": "Chi nhánh Mường Tè", "type": "CNL2", "parentUnitId": 1, "parentUnitName": "Chi nhánh Lai Châu"}'

# 6. Agribank CN H. Than Uyên -> Chi nhánh Than Uyên, code: CnThanUyen
curl -X PUT "http://localhost:5055/api/Units/1006" \
  -H "Content-Type: application/json" \
  -d '{"id": 1006, "code": "CnThanUyen", "name": "Chi nhánh Than Uyên", "type": "CNL2", "parentUnitId": 1, "parentUnitName": "Chi nhánh Lai Châu"}'

# 7. Agribank CN Thành Phố -> Chi nhánh Thành Phố, code: CnThanhPho
curl -X PUT "http://localhost:5055/api/Units/1007" \
  -H "Content-Type: application/json" \
  -d '{"id": 1007, "code": "CnThanhPho", "name": "Chi nhánh Thành Phố", "type": "CNL2", "parentUnitId": 1, "parentUnitName": "Chi nhánh Lai Châu"}'

# 8. Agribank CN H. Tân Uyên -> Chi nhánh Tân Uyên, code: CnTanUyen
curl -X PUT "http://localhost:5055/api/Units/1008" \
  -H "Content-Type: application/json" \
  -d '{"id": 1008, "code": "CnTanUyen", "name": "Chi nhánh Tân Uyên", "type": "CNL2", "parentUnitId": 1, "parentUnitName": "Chi nhánh Lai Châu"}'

# 9. Agribank CN H. Nậm Nhùn -> Chi nhánh Nậm Nhùn, code: CnNamNhun
curl -X PUT "http://localhost:5055/api/Units/1009" \
  -H "Content-Type: application/json" \
  -d '{"id": 1009, "code": "CnNamNhun", "name": "Chi nhánh Nậm Nhùn", "type": "CNL2", "parentUnitId": 1, "parentUnitName": "Chi nhánh Lai Châu"}'

# 10. Update main branch: Agribank CN Lai Châu -> Chi nhánh Lai Châu
curl -X PUT "http://localhost:5055/api/Units/1" \
  -H "Content-Type: application/json" \
  -d '{"id": 1, "code": "CnLaiChau", "name": "Chi nhánh Lai Châu", "type": "CNL1", "parentUnitId": null, "parentUnitName": null}'

echo "Updated all branch names and codes in the database"
