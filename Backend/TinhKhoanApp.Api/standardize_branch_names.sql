-- SQL script to standardize branch names in the Units table

-- Update "tỉnh Lai Châu" -> "Chi nhánh Lai Châu" with code "CnLaiChau"
UPDATE Units SET name = 'Chi nhánh Lai Châu' WHERE name LIKE '%tỉnh Lai Châu%';
UPDATE Units SET name = 'Chi nhánh Lai Châu' WHERE id = 1;
UPDATE Units SET name = 'Chi nhánh Lai Châu' WHERE id = 4398;

-- Update "Tam Đường" -> "Chi nhánh Tam Đường" with code "CnTamDuong"
UPDATE Units SET name = 'Chi nhánh Tam Đường' WHERE name LIKE '%Tam Đường%';

-- Update "Phong Thổ" -> "Chi nhánh Phong Thổ" with code "CnPhongTho"
UPDATE Units SET name = 'Chi nhánh Phong Thổ' WHERE name LIKE '%Phong Thổ%';

-- Update "Sìn Hồ" -> "Chi nhánh Sìn Hồ" with code "CnSinHo"
UPDATE Units SET name = 'Chi nhánh Sìn Hồ' WHERE name LIKE '%Sìn Hồ%';

-- Update "Mường Tè" -> "Chi nhánh Mường Tè" with code "CnMuongTe"
UPDATE Units SET name = 'Chi nhánh Mường Tè' WHERE name LIKE '%Mường Tè%';

-- Update "Than Uyên" -> "Chi nhánh Than Uyên" with code "CnThanUyen"
UPDATE Units SET name = 'Chi nhánh Than Uyên' WHERE name LIKE '%Than Uyên%';

-- Update "Thành Phố" -> "Chi nhánh Thành Phố" with code "CnThanhPho"
UPDATE Units SET name = 'Chi nhánh Thành Phố' WHERE name LIKE '%Thành Phố%';

-- Update "Tân Uyên" -> "Chi nhánh Tân Uyên" with code "CnTanUyen"
UPDATE Units SET name = 'Chi nhánh Tân Uyên' WHERE name LIKE '%Tân Uyên%';

-- Update "Nậm Nhùn" -> "Chi nhánh Nậm Nhùn" with code "CnNamNhun"
UPDATE Units SET name = 'Chi nhánh Nậm Nhùn' WHERE name LIKE '%Nậm Nhùn%';
