-- ============================================
-- 🔄 SCRIPT CẬP NHẬT CẤU TRÚC ĐƠN VỊ
-- Chuyển PNVL2 về Hội Sở và đổi mã phòng ban
-- ============================================

BEGIN TRANSACTION;

-- 1. CHUYỂN TẤT CẢ CÁC PNVL2 VỀ HỘI SỞ (ID: 46)
PRINT '🔄 Đang chuyển các PNVL2 về Hội Sở...'

UPDATE Units
SET ParentUnitId = 46
WHERE Type = 'PNVL2';

PRINT '✅ Đã chuyển các PNVL2 về Hội Sở'

-- 2. ĐỔI MÃ ĐƠN VỊ CỦA CÁC PHÒNG BAN
PRINT '🔄 Đang đổi mã các phòng ban...'

-- CnLaiChauKtnq -> HoiSoKtnq
UPDATE Units
SET Code = 'HoiSoKtnq'
WHERE Code = 'CnLaiChauKtnq';

-- CnLaiChauKhdn -> HoiSoKhdn
UPDATE Units
SET Code = 'HoiSoKhdn'
WHERE Code = 'CnLaiChauKhdn';

-- CnLaiChauKhcn -> HoiSoKhcn
UPDATE Units
SET Code = 'HoiSoKhcn'
WHERE Code = 'CnLaiChauKhcn';

-- CnLaiChauTonghop -> HoiSoTongHop
UPDATE Units
SET Code = 'HoiSoTongHop'
WHERE Code = 'CnLaiChauTonghop';

-- CnLaiChauKtgs -> HoiSoKtgs
UPDATE Units
SET Code = 'HoiSoKtgs'
WHERE Code = 'CnLaiChauKtgs';

-- CnLaiChauKhqlrr -> HoiSoKhqlrr
UPDATE Units
SET Code = 'HoiSoKhqlrr'
WHERE Code = 'CnLaiChauKhqlrr';

PRINT '✅ Đã đổi mã các phòng ban'

-- 3. CHUYỂN CÁC PHÒNG BAN NÀY VỀ HỘI SỞ (ID: 46)
PRINT '🔄 Đang chuyển các phòng ban về Hội Sở...'

UPDATE Units
SET ParentUnitId = 46
WHERE Code IN ('HoiSoKtnq', 'HoiSoKhdn', 'HoiSoKhcn', 'HoiSoTongHop', 'HoiSoKtgs', 'HoiSoKhqlrr');

PRINT '✅ Đã chuyển các phòng ban về Hội Sở'

COMMIT TRANSACTION;

-- 4. KIỂM TRA KẾT QUẢ
PRINT '📊 Kiểm tra kết quả:'

SELECT 'HoiSo_Units' as Category, COUNT(*) as Count
FROM Units
WHERE ParentUnitId = 46
UNION ALL
SELECT 'PNVL2_under_HoiSo' as Category, COUNT(*) as Count
FROM Units
WHERE Type = 'PNVL2' AND ParentUnitId = 46
UNION ALL
SELECT 'Renamed_Departments' as Category, COUNT(*) as Count
FROM Units
WHERE Code LIKE 'HoiSo%';

-- Hiển thị chi tiết các unit thuộc Hội Sở
SELECT Id, Code, Name, Type, ParentUnitId
FROM Units
WHERE ParentUnitId = 46
ORDER BY Type, Code;

PRINT '🎉 HOÀN THÀNH CẬP NHẬT CẤU TRÚC ĐƠN VỊ!'
