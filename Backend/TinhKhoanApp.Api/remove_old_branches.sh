#!/bin/bash

# Since we have duplicate records with the correct names, let's delete the ones with the incorrect names
# and keep the ones with the proper names

# Delete Agribank CN Lai Châu (id=1)
curl -X DELETE "http://localhost:5055/api/Units/1"

# Delete Agribank CN Lai Châu (7800) (id=4398)
curl -X DELETE "http://localhost:5055/api/Units/4398"

# Delete Agribank CN H. Tam Đường (id=1002)
curl -X DELETE "http://localhost:5055/api/Units/1002"

# Delete Agribank CN H. Phong Thổ (id=1003)
curl -X DELETE "http://localhost:5055/api/Units/1003"

# Delete Agribank CN H. Sìn Hồ (id=1004)
curl -X DELETE "http://localhost:5055/api/Units/1004"

# Delete Agribank CN H. Mường Tè (id=1005)
curl -X DELETE "http://localhost:5055/api/Units/1005"

# Delete Agribank CN H. Than Uyên (id=1006)
curl -X DELETE "http://localhost:5055/api/Units/1006"

# Delete Agribank CN Thành Phố (id=1007)
curl -X DELETE "http://localhost:5055/api/Units/1007"

# Delete Agribank CN H. Tân Uyên (id=1008)
curl -X DELETE "http://localhost:5055/api/Units/1008"

# Delete Agribank CN H. Nậm Nhùn (id=1009)
curl -X DELETE "http://localhost:5055/api/Units/1009"

echo "Removed units with incorrect names"
