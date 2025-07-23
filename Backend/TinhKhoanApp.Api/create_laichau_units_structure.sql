-- =============================================
-- TẠO CẤU TRÚC ĐƠN VỊ CHI NHÁNH LAI CHÂU
-- Tổng: 46 đơn vị theo cấu trúc phân cấp
-- =============================================

PRINT '🏢 BẮT ĐẦU TẠO CẤU TRÚC ĐƠN VỊ CHI NHÁNH LAI CHÂU'
PRINT '=================================================='

-- Xóa dữ liệu cũ nếu có
DELETE FROM Units WHERE Id >= 1 AND Id <= 46;
PRINT '🗑️ Đã xóa dữ liệu đơn vị cũ (ID 1-46)'

-- Reset identity if needed
DBCC CHECKIDENT ('Units', RESEED, 0);

-- =============================================
-- CNL1: Chi nhánh cấp 1 (ROOT + Hội Sở)
-- =============================================

-- ROOT: Chi nhánh Lai Châu
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId, IsActive, CreatedDate, UpdatedDate) VALUES
(1, N'Chi nhánh Lai Châu', 'CN_LAICHAU', 'CNL1', NULL, 1, GETDATE(), GETDATE());

-- Hội Sở
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId, IsActive, CreatedDate, UpdatedDate) VALUES
(2, N'Hội Sở', 'HOISO', 'CNL1', 1, 1, GETDATE(), GETDATE());

PRINT '✅ Đã tạo 2 đơn vị CNL1: Chi nhánh Lai Châu (ROOT) + Hội Sở'

-- =============================================
-- PNVL1: 7 Phòng ban Hội Sở
-- =============================================

INSERT INTO Units (Id, Name, Code, Type, ParentUnitId, IsActive, CreatedDate, UpdatedDate) VALUES
(3, N'Ban Giám đốc', 'BGD_HOISO', 'PNVL1', 2, 1, GETDATE(), GETDATE()),
(4, N'Phòng Khách hàng Doanh nghiệp', 'PKIDN_HOISO', 'PNVL1', 2, 1, GETDATE(), GETDATE()),
(5, N'Phòng Khách hàng Cá nhân', 'PKHCN_HOISO', 'PNVL1', 2, 1, GETDATE(), GETDATE()),
(6, N'Phòng Kế toán & Ngân quỹ', 'PKTNQ_HOISO', 'PNVL1', 2, 1, GETDATE(), GETDATE()),
(7, N'Phòng Tổng hợp', 'PTH_HOISO', 'PNVL1', 2, 1, GETDATE(), GETDATE()),
(8, N'Phòng Kế hoạch & Quản lý rủi ro', 'PKHQLRR_HOISO', 'PNVL1', 2, 1, GETDATE(), GETDATE()),
(9, N'Phòng Kiểm tra giám sát', 'PKTGS_HOISO', 'PNVL1', 2, 1, GETDATE(), GETDATE());

PRINT '✅ Đã tạo 7 phòng ban PNVL1 thuộc Hội Sở'

-- =============================================
-- CNL2: 8 Chi nhánh cấp 2
-- =============================================

INSERT INTO Units (Id, Name, Code, Type, ParentUnitId, IsActive, CreatedDate, UpdatedDate) VALUES
(10, N'Chi nhánh Bình Lư', 'CN_BINHLU', 'CNL2', 1, 1, GETDATE(), GETDATE()),
(11, N'Chi nhánh Phong Thổ', 'CN_PHONGTHO', 'CNL2', 1, 1, GETDATE(), GETDATE()),
(12, N'Chi nhánh Sìn Hồ', 'CN_SINHO', 'CNL2', 1, 1, GETDATE(), GETDATE()),
(13, N'Chi nhánh Bum Tở', 'CN_BUMTO', 'CNL2', 1, 1, GETDATE(), GETDATE()),
(14, N'Chi nhánh Than Uyên', 'CN_THANUYEN', 'CNL2', 1, 1, GETDATE(), GETDATE()),
(15, N'Chi nhánh Đoàn Kết', 'CN_DOANKET', 'CNL2', 1, 1, GETDATE(), GETDATE()),
(16, N'Chi nhánh Tân Uyên', 'CN_TANUYEN', 'CNL2', 1, 1, GETDATE(), GETDATE()),
(17, N'Chi nhánh Nậm Hàng', 'CN_NAMHANG', 'CNL2', 1, 1, GETDATE(), GETDATE());

PRINT '✅ Đã tạo 8 chi nhánh CNL2 cấp 2'

-- =============================================
-- PNVL2: 24 Phòng ban chi nhánh (3 phòng/chi nhánh)
-- =============================================

-- Chi nhánh Bình Lư (ID=10)
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId, IsActive, CreatedDate, UpdatedDate) VALUES
(18, N'Ban Giám đốc', 'BGD_BINHLU', 'PNVL2', 10, 1, GETDATE(), GETDATE()),
(19, N'Phòng Kế toán & Ngân quỹ', 'PKTNQ_BINHLU', 'PNVL2', 10, 1, GETDATE(), GETDATE()),
(20, N'Phòng Khách hàng', 'PKH_BINHLU', 'PNVL2', 10, 1, GETDATE(), GETDATE());

-- Chi nhánh Phong Thổ (ID=11)
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId, IsActive, CreatedDate, UpdatedDate) VALUES
(21, N'Ban Giám đốc', 'BGD_PHONGTHO', 'PNVL2', 11, 1, GETDATE(), GETDATE()),
(22, N'Phòng Kế toán & Ngân quỹ', 'PKTNQ_PHONGTHO', 'PNVL2', 11, 1, GETDATE(), GETDATE()),
(23, N'Phòng Khách hàng', 'PKH_PHONGTHO', 'PNVL2', 11, 1, GETDATE(), GETDATE());

-- Chi nhánh Sìn Hồ (ID=12)
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId, IsActive, CreatedDate, UpdatedDate) VALUES
(25, N'Ban Giám đốc', 'BGD_SINHO', 'PNVL2', 12, 1, GETDATE(), GETDATE()),
(26, N'Phòng Kế toán & Ngân quỹ', 'PKTNQ_SINHO', 'PNVL2', 12, 1, GETDATE(), GETDATE()),
(27, N'Phòng Khách hàng', 'PKH_SINHO', 'PNVL2', 12, 1, GETDATE(), GETDATE());

-- Chi nhánh Bum Tở (ID=13)
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId, IsActive, CreatedDate, UpdatedDate) VALUES
(28, N'Ban Giám đốc', 'BGD_BUMTO', 'PNVL2', 13, 1, GETDATE(), GETDATE()),
(29, N'Phòng Kế toán & Ngân quỹ', 'PKTNQ_BUMTO', 'PNVL2', 13, 1, GETDATE(), GETDATE()),
(30, N'Phòng Khách hàng', 'PKH_BUMTO', 'PNVL2', 13, 1, GETDATE(), GETDATE());

-- Chi nhánh Than Uyên (ID=14)
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId, IsActive, CreatedDate, UpdatedDate) VALUES
(31, N'Ban Giám đốc', 'BGD_THANUYEN', 'PNVL2', 14, 1, GETDATE(), GETDATE()),
(32, N'Phòng Kế toán & Ngân quỹ', 'PKTNQ_THANUYEN', 'PNVL2', 14, 1, GETDATE(), GETDATE()),
(33, N'Phòng Khách hàng', 'PKH_THANUYEN', 'PNVL2', 14, 1, GETDATE(), GETDATE());

-- Chi nhánh Đoàn Kết (ID=15)
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId, IsActive, CreatedDate, UpdatedDate) VALUES
(35, N'Ban Giám đốc', 'BGD_DOANKET', 'PNVL2', 15, 1, GETDATE(), GETDATE()),
(36, N'Phòng Kế toán & Ngân quỹ', 'PKTNQ_DOANKET', 'PNVL2', 15, 1, GETDATE(), GETDATE()),
(37, N'Phòng Khách hàng', 'PKH_DOANKET', 'PNVL2', 15, 1, GETDATE(), GETDATE());

-- Chi nhánh Tân Uyên (ID=16)
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId, IsActive, CreatedDate, UpdatedDate) VALUES
(40, N'Ban Giám đốc', 'BGD_TANUYEN', 'PNVL2', 16, 1, GETDATE(), GETDATE()),
(41, N'Phòng Kế toán & Ngân quỹ', 'PKTNQ_TANUYEN', 'PNVL2', 16, 1, GETDATE(), GETDATE()),
(42, N'Phòng Khách hàng', 'PKH_TANUYEN', 'PNVL2', 16, 1, GETDATE(), GETDATE());

-- Chi nhánh Nậm Hàng (ID=17)
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId, IsActive, CreatedDate, UpdatedDate) VALUES
(44, N'Ban Giám đốc', 'BGD_NAMHANG', 'PNVL2', 17, 1, GETDATE(), GETDATE()),
(45, N'Phòng Kế toán & Ngân quỹ', 'PKTNQ_NAMHANG', 'PNVL2', 17, 1, GETDATE(), GETDATE()),
(46, N'Phòng Khách hàng', 'PKH_NAMHANG', 'PNVL2', 17, 1, GETDATE(), GETDATE());

PRINT '✅ Đã tạo 24 phòng ban PNVL2 (3 phòng × 8 chi nhánh)'

-- =============================================
-- PGDL2: 6 Phòng giao dịch
-- =============================================

INSERT INTO Units (Id, Name, Code, Type, ParentUnitId, IsActive, CreatedDate, UpdatedDate) VALUES
(24, N'Phòng giao dịch Số 5', 'PGD_SO5', 'PGDL2', 11, 1, GETDATE(), GETDATE()),     -- Phong Thổ
(34, N'Phòng giao dịch số 6', 'PGD_SO6', 'PGDL2', 14, 1, GETDATE(), GETDATE()),      -- Than Uyên
(38, N'Phòng giao dịch số 1', 'PGD_SO1', 'PGDL2', 15, 1, GETDATE(), GETDATE()),      -- Đoàn Kết
(39, N'Phòng giao dịch số 2', 'PGD_SO2', 'PGDL2', 15, 1, GETDATE(), GETDATE()),      -- Đoàn Kết
(43, N'Phòng giao dịch số 3', 'PGD_SO3', 'PGDL2', 16, 1, GETDATE(), GETDATE());      -- Tân Uyên

PRINT '✅ Đã tạo 5 phòng giao dịch PGDL2'

-- =============================================
-- VERIFICATION & STATISTICS
-- =============================================

PRINT ''
PRINT '📊 THỐNG KÊ CẤU TRÚC ĐƠN VỊ:'
PRINT '================================'

DECLARE @cnl1_count INT, @cnl2_count INT, @pnvl1_count INT, @pnvl2_count INT, @pgdl2_count INT, @total_count INT

SELECT @cnl1_count = COUNT(*) FROM Units WHERE Type = 'CNL1' AND Id BETWEEN 1 AND 46
SELECT @cnl2_count = COUNT(*) FROM Units WHERE Type = 'CNL2' AND Id BETWEEN 1 AND 46
SELECT @pnvl1_count = COUNT(*) FROM Units WHERE Type = 'PNVL1' AND Id BETWEEN 1 AND 46
SELECT @pnvl2_count = COUNT(*) FROM Units WHERE Type = 'PNVL2' AND Id BETWEEN 1 AND 46
SELECT @pgdl2_count = COUNT(*) FROM Units WHERE Type = 'PGDL2' AND Id BETWEEN 1 AND 46
SELECT @total_count = COUNT(*) FROM Units WHERE Id BETWEEN 1 AND 46

PRINT '✅ CNL1 (Chi nhánh cấp 1): ' + CAST(@cnl1_count AS NVARCHAR(10)) + ' đơn vị'
PRINT '✅ CNL2 (Chi nhánh cấp 2): ' + CAST(@cnl2_count AS NVARCHAR(10)) + ' đơn vị'
PRINT '✅ PNVL1 (Phòng ban Hội Sở): ' + CAST(@pnvl1_count AS NVARCHAR(10)) + ' đơn vị'
PRINT '✅ PNVL2 (Phòng ban chi nhánh): ' + CAST(@pnvl2_count AS NVARCHAR(10)) + ' đơn vị'
PRINT '✅ PGDL2 (Phòng giao dịch): ' + CAST(@pgdl2_count AS NVARCHAR(10)) + ' đơn vị'
PRINT '📈 TỔNG CỘNG: ' + CAST(@total_count AS NVARCHAR(10)) + ' đơn vị'

-- Verify hierarchy structure
PRINT ''
PRINT '🌳 KIỂM TRA CẤU TRÚC PHÂN CẤP:'
PRINT '=============================='

-- Root level
PRINT '🏢 ROOT: ' + CAST((SELECT COUNT(*) FROM Units WHERE ParentUnitId IS NULL AND Id BETWEEN 1 AND 46) AS NVARCHAR(10)) + ' đơn vị'

-- Level 1 (under root)
PRINT '├─ Level 1: ' + CAST((SELECT COUNT(*) FROM Units WHERE ParentUnitId = 1 AND Id BETWEEN 1 AND 46) AS NVARCHAR(10)) + ' đơn vị'

-- Level 2 (under Hội Sở and branches)
DECLARE @level2_count INT
SELECT @level2_count = COUNT(*)
FROM Units u1
WHERE EXISTS (
    SELECT 1 FROM Units u2
    WHERE u2.ParentUnitId = 1
      AND u2.Id BETWEEN 1 AND 46
      AND u1.ParentUnitId = u2.Id
) AND u1.Id BETWEEN 1 AND 46

PRINT '├─ Level 2: ' + CAST(@level2_count AS NVARCHAR(10)) + ' đơn vị'

IF @total_count = 46
    PRINT '🎉 THÀNH CÔNG: Đã tạo đúng 46 đơn vị theo cấu trúc yêu cầu!'
ELSE
    PRINT '⚠️ CẢNH BÁO: Số lượng đơn vị không đúng (' + CAST(@total_count AS NVARCHAR(10)) + '/46)'

PRINT ''
PRINT '✅ HOÀN TẤT TẠO CẤU TRÚC ĐƠN VỊ CHI NHÁNH LAI CHÂU'
