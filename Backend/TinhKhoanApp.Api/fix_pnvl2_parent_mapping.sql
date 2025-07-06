-- ============================================
-- 🔄 SCRIPT TRẢ LẠI CÁC PHÒNG BAN PNVL2 VỀ ĐÚNG CHI NHÁNH MẸ
-- Mapping các phòng ban về đúng ParentUnitId
-- ============================================

BEGIN TRANSACTION;

PRINT '🔄 Bắt đầu trả lại các phòng ban PNVL2 về đúng chi nhánh mẹ...'

-- 1. TRẢ LẠI CÁC PHÒNG BAN CnBinhLu* VỀ CHI NHÁNH CnBinhLu (ID: 9)
UPDATE Units
SET ParentUnitId = 9
WHERE Type = 'PNVL2'
  AND Code LIKE 'CnBinhLu%'
  AND ParentUnitId = 46;

PRINT '✅ Đã trả lại phòng ban CnBinhLu* về chi nhánh CnBinhLu'

-- 2. TRẢ LẠI CÁC PHÒNG BAN CnPhongTho* VỀ CHI NHÁNH CnPhongTho (ID: 13)
UPDATE Units
SET ParentUnitId = 13
WHERE Type = 'PNVL2'
  AND Code LIKE 'CnPhongTho%'
  AND ParentUnitId = 46;

PRINT '✅ Đã trả lại phòng ban CnPhongTho* về chi nhánh CnPhongTho'

-- 3. TRẢ LẠI CÁC PHÒNG BAN CnSinHo* VỀ CHI NHÁNH CnSinHo (ID: 18)
UPDATE Units
SET ParentUnitId = 18
WHERE Type = 'PNVL2'
  AND Code LIKE 'CnSinHo%'
  AND ParentUnitId = 46;

PRINT '✅ Đã trả lại phòng ban CnSinHo* về chi nhánh CnSinHo'

-- 4. TRẢ LẠI CÁC PHÒNG BAN CnBumTo* VỀ CHI NHÁNH CnBumTo (ID: 22)
UPDATE Units
SET ParentUnitId = 22
WHERE Type = 'PNVL2'
  AND Code LIKE 'CnBumTo%'
  AND ParentUnitId = 46;

PRINT '✅ Đã trả lại phòng ban CnBumTo* về chi nhánh CnBumTo'

-- 5. TRẢ LẠI CÁC PHÒNG BAN CnThanUyen* VỀ CHI NHÁNH CnThanUyen (ID: 26)
UPDATE Units
SET ParentUnitId = 26
WHERE Type = 'PNVL2'
  AND Code LIKE 'CnThanUyen%'
  AND ParentUnitId = 46;

PRINT '✅ Đã trả lại phòng ban CnThanUyen* về chi nhánh CnThanUyen'

-- 6. TRẢ LẠI CÁC PHÒNG BAN CnDoanKet* VỀ CHI NHÁNH CnDoanKet (ID: 31)
UPDATE Units
SET ParentUnitId = 31
WHERE Type = 'PNVL2'
  AND Code LIKE 'CnDoanKet%'
  AND ParentUnitId = 46;

PRINT '✅ Đã trả lại phòng ban CnDoanKet* về chi nhánh CnDoanKet'

-- 7. TRẢ LẠI CÁC PHÒNG BAN CnTanUyen* VỀ CHI NHÁNH CnTanUyen (ID: 37)
UPDATE Units
SET ParentUnitId = 37
WHERE Type = 'PNVL2'
  AND Code LIKE 'CnTanUyen%'
  AND ParentUnitId = 46;

PRINT '✅ Đã trả lại phòng ban CnTanUyen* về chi nhánh CnTanUyen'

-- 8. TRẢ LẠI CÁC PHÒNG BAN CnNamHang* VỀ CHI NHÁNH CnNamHang (ID: 42)
UPDATE Units
SET ParentUnitId = 42
WHERE Type = 'PNVL2'
  AND Code LIKE 'CnNamHang%'
  AND ParentUnitId = 46;

PRINT '✅ Đã trả lại phòng ban CnNamHang* về chi nhánh CnNamHang'

-- NOTE: HoiSoBgd (ID: 2) sẽ vẫn ở lại Hội Sở (ID: 46) - đây là phòng ban chính của Hội Sở

COMMIT TRANSACTION;

PRINT '📊 Kiểm tra kết quả sau khi trả lại:'

-- Kiểm tra số lượng phòng ban còn lại ở Hội Sở
SELECT 'PNVL2_still_in_HoiSo' as Category, COUNT(*) as Count
FROM Units
WHERE Type = 'PNVL2' AND ParentUnitId = 46
UNION ALL
SELECT 'PNVL1_in_HoiSo' as Category, COUNT(*) as Count
FROM Units
WHERE Type = 'PNVL1' AND ParentUnitId = 46;

-- Hiển thị phân bổ PNVL2 theo từng chi nhánh
SELECT
    p.Name as ParentBranch,
    COUNT(*) as PNVL2_Count
FROM Units u
JOIN Units p ON u.ParentUnitId = p.Id
WHERE u.Type = 'PNVL2'
GROUP BY p.Id, p.Name
ORDER BY p.Name;

PRINT '🎉 HOÀN THÀNH TRẢ LẠI CÁC PHÒNG BAN VỀ ĐÚNG CHI NHÁNH MẸ!'
