-- =============================================
-- TẠO CẤU TRÚC ĐƠN VỊ CHI NHÁNH LAI CHÂU - VERSION SIMPLE
-- Tổng: 46 đơn vị theo cấu trúc phân cấp
-- =============================================

PRINT '🏢 BẮT ĐẦU TẠO CẤU TRÚC ĐƠN VỊ CHI NHÁNH LAI CHÂU'

-- Xóa dữ liệu cũ nếu có
DELETE FROM Units WHERE Id >= 1 AND Id <= 46;
PRINT '🗑️ Đã xóa dữ liệu đơn vị cũ (ID 1-46)'

-- =============================================
-- CNL1: Chi nhánh cấp 1 (ROOT + Hội Sở)
-- =============================================

-- ROOT: Chi nhánh Lai Châu
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId, IsActive, CreatedDate, UpdatedDate) VALUES
(1, N'Chi nhánh Lai Châu', 'CN_LAICHAU', 'CNL1', NULL, 1, GETDATE(), GETDATE());

-- Hội Sở
INSERT INTO Units (Id, Name, Code, Type, ParentUnitId, IsActive, CreatedDate, UpdatedDate) VALUES
(2, N'Hội Sở', 'HOISO', 'CNL1', 1, 1, GETDATE(), GETDATE());

PRINT '✅ Đã tạo 2 đơn vị CNL1'

-- =============================================
-- PNVL1: 7 Phòng ban Hội Sở
-- =============================================

INSERT INTO Units (Id, Name, Code, Type, ParentUnitId, IsActive, CreatedDate, UpdatedDate) VALUES
(3, N'Ban Giám đốc', 'BGD_HOISO', 'PNVL1', 2, 1, GETDATE(), GETDATE()),
(4, N'Phòng Khách hàng Doanh nghiệp', 'PKHDN_HOISO', 'PNVL1', 2, 1, GETDATE(), GETDATE()),
(5, N'Phòng Khách hàng Cá nhân', 'PKHCN_HOISO', 'PNVL1', 2, 1, GETDATE(), GETDATE()),
(6, N'Phòng Kế toán & Ngân quỹ', 'PKTNQ_HOISO', 'PNVL1', 2, 1, GETDATE(), GETDATE()),
(7, N'Phòng Tổng hợp', 'PTH_HOISO', 'PNVL1', 2, 1, GETDATE(), GETDATE()),
(8, N'Phòng Kế hoạch & Quản lý rủi ro', 'PKHQLRR_HOISO', 'PNVL1', 2, 1, GETDATE(), GETDATE()),
(9, N'Phòng Kiểm tra giám sát', 'PKTGS_HOISO', 'PNVL1', 2, 1, GETDATE(), GETDATE());

PRINT '✅ Đã tạo 7 phòng ban PNVL1'

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

PRINT '✅ Đã tạo 8 chi nhánh CNL2'

-- =============================================
-- PNVL2: 24 Phòng ban chi nhánh
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

PRINT '✅ Đã tạo 24 phòng ban PNVL2'

-- =============================================
-- PGDL2: 5 Phòng giao dịch
-- =============================================

INSERT INTO Units (Id, Name, Code, Type, ParentUnitId, IsActive, CreatedDate, UpdatedDate) VALUES
(24, N'Phòng giao dịch Số 5', 'PGD_SO5', 'PGDL2', 11, 1, GETDATE(), GETDATE()),     -- Phong Thổ
(34, N'Phòng giao dịch số 6', 'PGD_SO6', 'PGDL2', 14, 1, GETDATE(), GETDATE()),      -- Than Uyên
(38, N'Phòng giao dịch số 1', 'PGD_SO1', 'PGDL2', 15, 1, GETDATE(), GETDATE()),      -- Đoàn Kết
(39, N'Phòng giao dịch số 2', 'PGD_SO2', 'PGDL2', 15, 1, GETDATE(), GETDATE()),      -- Đoàn Kết
(43, N'Phòng giao dịch số 3', 'PGD_SO3', 'PGDL2', 16, 1, GETDATE(), GETDATE());      -- Tân Uyên

PRINT '✅ Đã tạo 5 phòng giao dịch PGDL2'

-- =============================================
-- SIMPLE STATISTICS
-- =============================================

DECLARE @total_count INT
SELECT @total_count = COUNT(*) FROM Units WHERE Id BETWEEN 1 AND 46

PRINT ''
PRINT '📊 THỐNG KÊ:'
PRINT 'CNL1: 2, CNL2: 8, PNVL1: 7, PNVL2: 24, PGDL2: 5'
PRINT 'TỔNG CỘNG: ' + CAST(@total_count AS NVARCHAR(10)) + ' đơn vị'

IF @total_count = 46
    PRINT '🎉 THÀNH CÔNG: Đã tạo đúng 46 đơn vị!'
ELSE
    PRINT '⚠️ CẢNH BÁO: Sai số lượng (' + CAST(@total_count AS NVARCHAR(10)) + '/46)'

PRINT '✅ HOÀN TẤT TẠO CẤU TRÚC ĐƠN VỊ'
