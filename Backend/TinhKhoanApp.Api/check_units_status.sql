-- Check the status of units after standardization
SELECT id, name, code
FROM Units
WHERE name LIKE '%Chi nhánh%'
   OR name LIKE '%Lai Châu%'
   OR name LIKE '%Tam Đường%'
   OR name LIKE '%Phong Thổ%'
   OR name LIKE '%Sìn Hồ%'
   OR name LIKE '%Mường Tè%'
   OR name LIKE '%Than Uyên%'
   OR name LIKE '%Thành Phố%'
   OR name LIKE '%Tân Uyên%'
   OR name LIKE '%Nậm Nhùn%'
ORDER BY id;
