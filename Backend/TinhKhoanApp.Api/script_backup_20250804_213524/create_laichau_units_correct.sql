-- =============================================
-- TẠO CẤU TRÚC ĐƠN VỊ CHI NHÁNH LAI CHÂU - CORRECT SCHEMA
-- Tổng: 46 đơn vị theo cấu trúc phân cấp
-- =============================================

PRINT '🏢 BẮT ĐẦU TẠO CẤU TRÚC ĐƠN VỊ CHI NHÁNH LAI CHÂU'

-- Xóa dữ liệu cũ nếu có
DELETE FROM Units WHERE Id >= 1 AND Id <= 46;
PRINT '🗑️ Đã xóa dữ liệu đơn vị cũ (ID 1-46)'

-- Bật IDENTITY_INSERT để insert explicit ID values
SET IDENTITY_INSERT Units ON;

-- =============================================
-- CNL1: Chi nhánh cấp 1 (ROOT + Hội Sở)
-- =============================================

INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(1, 'CN_LAICHAU', N'Chi nhánh Lai Châu', 'CNL1', NULL, 0),
(2, 'HOISO', N'Hội Sở', 'CNL1', 1, 0);

PRINT '✅ Đã tạo 2 đơn vị CNL1'

-- =============================================
-- PNVL1: 7 Phòng ban Hội Sở
-- =============================================

INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(3, 'BGD_HOISO', N'Ban Giám đốc', 'PNVL1', 2, 0),
(4, 'PKHDN_HOISO', N'Phòng Khách hàng Doanh nghiệp', 'PNVL1', 2, 0),
(5, 'PKHCN_HOISO', N'Phòng Khách hàng Cá nhân', 'PNVL1', 2, 0),
(6, 'PKTNQ_HOISO', N'Phòng Kế toán & Ngân quỹ', 'PNVL1', 2, 0),
(7, 'PTH_HOISO', N'Phòng Tổng hợp', 'PNVL1', 2, 0),
(8, 'PKHQLRR_HOISO', N'Phòng Kế hoạch & Quản lý rủi ro', 'PNVL1', 2, 0),
(9, 'PKTGS_HOISO', N'Phòng Kiểm tra giám sát', 'PNVL1', 2, 0);

PRINT '✅ Đã tạo 7 phòng ban PNVL1'

-- =============================================
-- CNL2: 8 Chi nhánh cấp 2
-- =============================================

INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(10, 'CN_BINHLU', N'Chi nhánh Bình Lư', 'CNL2', 1, 0),
(11, 'CN_PHONGTHO', N'Chi nhánh Phong Thổ', 'CNL2', 1, 0),
(12, 'CN_SINHO', N'Chi nhánh Sìn Hồ', 'CNL2', 1, 0),
(13, 'CN_BUMTO', N'Chi nhánh Bum Tở', 'CNL2', 1, 0),
(14, 'CN_THANUYEN', N'Chi nhánh Than Uyên', 'CNL2', 1, 0),
(15, 'CN_DOANKET', N'Chi nhánh Đoàn Kết', 'CNL2', 1, 0),
(16, 'CN_TANUYEN', N'Chi nhánh Tân Uyên', 'CNL2', 1, 0),
(17, 'CN_NAMHANG', N'Chi nhánh Nậm Hàng', 'CNL2', 1, 0);

PRINT '✅ Đã tạo 8 chi nhánh CNL2'

-- =============================================
-- PNVL2: 24 Phòng ban chi nhánh
-- =============================================

-- Chi nhánh Bình Lư (ID=10)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(18, 'BGD_BINHLU', N'Ban Giám đốc', 'PNVL2', 10, 0),
(19, 'PKTNQ_BINHLU', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 10, 0),
(20, 'PKH_BINHLU', N'Phòng Khách hàng', 'PNVL2', 10, 0);

-- Chi nhánh Phong Thổ (ID=11)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(21, 'BGD_PHONGTHO', N'Ban Giám đốc', 'PNVL2', 11, 0),
(22, 'PKTNQ_PHONGTHO', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 11, 0),
(23, 'PKH_PHONGTHO', N'Phòng Khách hàng', 'PNVL2', 11, 0);

-- Chi nhánh Sìn Hồ (ID=12)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(25, 'BGD_SINHO', N'Ban Giám đốc', 'PNVL2', 12, 0),
(26, 'PKTNQ_SINHO', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 12, 0),
(27, 'PKH_SINHO', N'Phòng Khách hàng', 'PNVL2', 12, 0);

-- Chi nhánh Bum Tở (ID=13)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(28, 'BGD_BUMTO', N'Ban Giám đốc', 'PNVL2', 13, 0),
(29, 'PKTNQ_BUMTO', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 13, 0),
(30, 'PKH_BUMTO', N'Phòng Khách hàng', 'PNVL2', 13, 0);

-- Chi nhánh Than Uyên (ID=14)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(31, 'BGD_THANUYEN', N'Ban Giám đốc', 'PNVL2', 14, 0),
(32, 'PKTNQ_THANUYEN', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 14, 0),
(33, 'PKH_THANUYEN', N'Phòng Khách hàng', 'PNVL2', 14, 0);

-- Chi nhánh Đoàn Kết (ID=15)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(35, 'BGD_DOANKET', N'Ban Giám đốc', 'PNVL2', 15, 0),
(36, 'PKTNQ_DOANKET', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 15, 0),
(37, 'PKH_DOANKET', N'Phòng Khách hàng', 'PNVL2', 15, 0);

-- Chi nhánh Tân Uyên (ID=16)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(40, 'BGD_TANUYEN', N'Ban Giám đốc', 'PNVL2', 16, 0),
(41, 'PKTNQ_TANUYEN', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 16, 0),
(42, 'PKH_TANUYEN', N'Phòng Khách hàng', 'PNVL2', 16, 0);

-- Chi nhánh Nậm Hàng (ID=17)
INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(44, 'BGD_NAMHANG', N'Ban Giám đốc', 'PNVL2', 17, 0),
(45, 'PKTNQ_NAMHANG', N'Phòng Kế toán & Ngân quỹ', 'PNVL2', 17, 0),
(46, 'PKH_NAMHANG', N'Phòng Khách hàng', 'PNVL2', 17, 0);

PRINT '✅ Đã tạo 24 phòng ban PNVL2'

-- =============================================
-- PGDL2: 5 Phòng giao dịch
-- =============================================

INSERT INTO Units (Id, Code, Name, Type, ParentUnitId, IsDeleted) VALUES
(24, 'PGD_SO5', N'Phòng giao dịch Số 5', 'PGDL2', 11, 0),     -- Phong Thổ
(34, 'PGD_SO6', N'Phòng giao dịch số 6', 'PGDL2', 14, 0),      -- Than Uyên
(38, 'PGD_SO1', N'Phòng giao dịch số 1', 'PGDL2', 15, 0),      -- Đoàn Kết
(39, 'PGD_SO2', N'Phòng giao dịch số 2', 'PGDL2', 15, 0),      -- Đoàn Kết
(43, 'PGD_SO3', N'Phòng giao dịch số 3', 'PGDL2', 16, 0);      -- Tân Uyên

PRINT '✅ Đã tạo 5 phòng giao dịch PGDL2'

-- Tắt IDENTITY_INSERT
SET IDENTITY_INSERT Units OFF;

-- =============================================
-- STATISTICS
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
