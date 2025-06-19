-- 🔍 Script tạo Views cho dữ liệu hiện tại của các bảng SCD Type 2 mới
-- Mở rộng cho: LN03, EI01, DPDA, DB01, KH03, BC57
-- Thiết kế: Views hiển thị dữ liệu hiện tại (IsCurrent = 1)

-- =======================================
-- 📊 LN03_Current - View dữ liệu Nợ XLRR hiện tại
-- =======================================
CREATE VIEW IF NOT EXISTS LN03_Current AS
SELECT 
    Id,
    BusinessKey,
    EffectiveDate,
    RowVersion,
    ImportId,
    StatementDate,
    ProcessedDate,
    
    -- Business fields
    MaKhachHang,
    TenKhachHang,
    MaHopDong,
    LoaiHopDong,
    SoTienGoc,
    SoTienLai,
    TongNo,
    NgayDaoHan,
    TinhTrangNo,
    NhomNo,
    MaChiNhanh,
    TenChiNhanh,
    NgayTao,
    NgayCapNhat,
    AdditionalData
FROM LN03_History 
WHERE IsCurrent = 1;

-- =======================================
-- 📊 EI01_Current - View dữ liệu Mobile Banking hiện tại
-- =======================================
CREATE VIEW IF NOT EXISTS EI01_Current AS
SELECT 
    Id,
    BusinessKey,
    EffectiveDate,
    RowVersion,
    ImportId,
    StatementDate,
    ProcessedDate,
    
    -- Business fields
    MaKhachHang,
    TenKhachHang,
    SoTaiKhoan,
    LoaiGiaoDich,
    MaGiaoDich,
    SoTien,
    NgayGiaoDich,
    ThoiGianGiaoDich,
    TrangThaiGiaoDich,
    NoiDungGiaoDich,
    MaChiNhanh,
    TenChiNhanh,
    Channel,
    DeviceInfo,
    AdditionalData
FROM EI01_History 
WHERE IsCurrent = 1;

-- =======================================
-- 📊 DPDA_Current - View dữ liệu Phát hành thẻ hiện tại
-- =======================================
CREATE VIEW IF NOT EXISTS DPDA_Current AS
SELECT 
    Id,
    BusinessKey,
    EffectiveDate,
    RowVersion,
    ImportId,
    StatementDate,
    ProcessedDate,
    
    -- Business fields
    MaKhachHang,
    TenKhachHang,
    SoThe,
    LoaiThe,
    TrangThaiThe,
    NgayPhatHanh,
    NgayHetHan,
    HanMucThe,
    SoDuHienTai,
    SoTienDaSD,
    MaChiNhanh,
    TenChiNhanh,
    NgayTao,
    NgayCapNhat,
    AdditionalData
FROM DPDA_History 
WHERE IsCurrent = 1;

-- =======================================
-- 📊 DB01_Current - View dữ liệu TSDB hiện tại
-- =======================================
CREATE VIEW IF NOT EXISTS DB01_Current AS
SELECT 
    Id,
    BusinessKey,
    EffectiveDate,
    RowVersion,
    ImportId,
    StatementDate,
    ProcessedDate,
    
    -- Business fields
    MaKhachHang,
    TenKhachHang,
    SoTaiKhoan,
    LoaiTaiKhoan,
    SoDu,
    SoDuKhaDung,
    NgayMoTK,
    TrangThaiTK,
    LaiSuat,
    KyHan,
    NgayDaoHan,
    SoTienGocGuy,
    TienLaiDuThu,
    MaChiNhanh,
    TenChiNhanh,
    LoaiHinhDB,
    AdditionalData
FROM DB01_History 
WHERE IsCurrent = 1;

-- =======================================
-- 📊 KH03_Current - View dữ liệu Khách hàng pháp nhân hiện tại
-- =======================================
CREATE VIEW IF NOT EXISTS KH03_Current AS
SELECT 
    Id,
    BusinessKey,
    EffectiveDate,
    RowVersion,
    ImportId,
    StatementDate,
    ProcessedDate,
    
    -- Business fields
    MaKhachHang,
    TenKhachHang,
    MaSoThue,
    DiaChi,
    SoDienThoai,
    Email,
    NguoiDaiDien,
    ChucVuNDD,
    NgaySinh,
    CMND_CCCD,
    NgayCapCMND,
    NoiCapCMND,
    LoaiKhachHang,
    PhanKhuc,
    MaChiNhanh,
    TenChiNhanh,
    TrangThaiKH,
    NgayTao,
    NgayCapNhat,
    AdditionalData
FROM KH03_History 
WHERE IsCurrent = 1;

-- =======================================
-- 📊 BC57_Current - View dữ liệu Lãi dự thu hiện tại
-- =======================================
CREATE VIEW IF NOT EXISTS BC57_Current AS
SELECT 
    Id,
    BusinessKey,
    EffectiveDate,
    RowVersion,
    ImportId,
    StatementDate,
    ProcessedDate,
    
    -- Business fields
    MaKhachHang,
    TenKhachHang,
    SoTaiKhoan,
    MaHopDong,
    LoaiSanPham,
    SoTienGoc,
    LaiSuat,
    SoNgayTinhLai,
    TienLaiDuThu,
    TienLaiQuaHan,
    NgayBatDau,
    NgayKetThuc,
    TrangThai,
    MaChiNhanh,
    TenChiNhanh,
    NgayTinhLai,
    AdditionalData
FROM BC57_History 
WHERE IsCurrent = 1;

-- =======================================
-- 📊 View thống kê tổng hợp cho tất cả bảng SCD Type 2
-- =======================================
CREATE VIEW IF NOT EXISTS AllTables_ImportStats AS
SELECT 
    'LN01' as TableName,
    'Dữ liệu LOAN' as Description,
    COUNT(*) as TotalRecords,
    COUNT(DISTINCT BusinessKey) as UniqueRecords,
    MAX(StatementDate) as LatestStatementDate,
    MAX(ProcessedDate) as LastProcessedDate,
    COUNT(DISTINCT ImportId) as ImportBatches
FROM LN01_History
WHERE IsCurrent = 1

UNION ALL

SELECT 
    'LN03' as TableName,
    'Dữ liệu Nợ XLRR' as Description,
    COUNT(*) as TotalRecords,
    COUNT(DISTINCT BusinessKey) as UniqueRecords,
    MAX(StatementDate) as LatestStatementDate,
    MAX(ProcessedDate) as LastProcessedDate,
    COUNT(DISTINCT ImportId) as ImportBatches
FROM LN03_History
WHERE IsCurrent = 1

UNION ALL

SELECT 
    'GL01' as TableName,
    'Dữ liệu bút toán GDV' as Description,
    COUNT(*) as TotalRecords,
    COUNT(DISTINCT BusinessKey) as UniqueRecords,
    MAX(StatementDate) as LatestStatementDate,
    MAX(ProcessedDate) as LastProcessedDate,
    COUNT(DISTINCT ImportId) as ImportBatches
FROM GL01_History
WHERE IsCurrent = 1

UNION ALL

SELECT 
    'DP01' as TableName,
    'Dữ liệu Tiền gửi' as Description,
    COUNT(*) as TotalRecords,
    COUNT(DISTINCT BusinessKey) as UniqueRecords,
    MAX(StatementDate) as LatestStatementDate,
    MAX(ProcessedDate) as LastProcessedDate,
    COUNT(DISTINCT ImportId) as ImportBatches
FROM DP01_History
WHERE IsCurrent = 1

UNION ALL

SELECT 
    'EI01' as TableName,
    'Dữ liệu Mobile Banking' as Description,
    COUNT(*) as TotalRecords,
    COUNT(DISTINCT BusinessKey) as UniqueRecords,
    MAX(StatementDate) as LatestStatementDate,
    MAX(ProcessedDate) as LastProcessedDate,
    COUNT(DISTINCT ImportId) as ImportBatches
FROM EI01_History
WHERE IsCurrent = 1

UNION ALL

SELECT 
    'DPDA' as TableName,
    'Dữ liệu Phát hành thẻ' as Description,
    COUNT(*) as TotalRecords,
    COUNT(DISTINCT BusinessKey) as UniqueRecords,
    MAX(StatementDate) as LatestStatementDate,
    MAX(ProcessedDate) as LastProcessedDate,
    COUNT(DISTINCT ImportId) as ImportBatches
FROM DPDA_History
WHERE IsCurrent = 1

UNION ALL

SELECT 
    'DB01' as TableName,
    'Sao kê TSDB và Không TSDB' as Description,
    COUNT(*) as TotalRecords,
    COUNT(DISTINCT BusinessKey) as UniqueRecords,
    MAX(StatementDate) as LatestStatementDate,
    MAX(ProcessedDate) as LastProcessedDate,
    COUNT(DISTINCT ImportId) as ImportBatches
FROM DB01_History
WHERE IsCurrent = 1

UNION ALL

SELECT 
    'KH03' as TableName,
    'Sao kê Khách hàng pháp nhân' as Description,
    COUNT(*) as TotalRecords,
    COUNT(DISTINCT BusinessKey) as UniqueRecords,
    MAX(StatementDate) as LatestStatementDate,
    MAX(ProcessedDate) as LastProcessedDate,
    COUNT(DISTINCT ImportId) as ImportBatches
FROM KH03_History
WHERE IsCurrent = 1

UNION ALL

SELECT 
    'BC57' as TableName,
    'Sao kê Lãi dự thu' as Description,
    COUNT(*) as TotalRecords,
    COUNT(DISTINCT BusinessKey) as UniqueRecords,
    MAX(StatementDate) as LatestStatementDate,
    MAX(ProcessedDate) as LastProcessedDate,
    COUNT(DISTINCT ImportId) as ImportBatches
FROM BC57_History
WHERE IsCurrent = 1;

-- =======================================
-- 📊 View lịch sử thay đổi cho từng bảng
-- =======================================

-- View lịch sử thay đổi LN03
CREATE VIEW IF NOT EXISTS LN03_ChangeHistory AS
SELECT 
    BusinessKey,
    RowVersion,
    EffectiveDate,
    ExpiryDate,
    ImportId,
    StatementDate,
    ProcessedDate,
    IsCurrent,
    
    -- Key business fields for change tracking
    MaKhachHang,
    TenKhachHang,
    SoTienGoc,
    SoTienLai,
    TongNo,
    TinhTrangNo,
    NhomNo
FROM LN03_History
ORDER BY BusinessKey, EffectiveDate DESC;

-- View lịch sử thay đổi EI01
CREATE VIEW IF NOT EXISTS EI01_ChangeHistory AS
SELECT 
    BusinessKey,
    RowVersion,
    EffectiveDate,
    ExpiryDate,
    ImportId,
    StatementDate,
    ProcessedDate,
    IsCurrent,
    
    -- Key business fields for change tracking
    MaKhachHang,
    TenKhachHang,
    SoTaiKhoan,
    LoaiGiaoDich,
    SoTien,
    TrangThaiGiaoDich
FROM EI01_History
ORDER BY BusinessKey, EffectiveDate DESC;

-- View lịch sử thay đổi DPDA
CREATE VIEW IF NOT EXISTS DPDA_ChangeHistory AS
SELECT 
    BusinessKey,
    RowVersion,
    EffectiveDate,
    ExpiryDate,
    ImportId,
    StatementDate,
    ProcessedDate,
    IsCurrent,
    
    -- Key business fields for change tracking
    MaKhachHang,
    TenKhachHang,
    SoThe,
    TrangThaiThe,
    HanMucThe,
    SoDuHienTai
FROM DPDA_History
ORDER BY BusinessKey, EffectiveDate DESC;

-- View lịch sử thay đổi DB01
CREATE VIEW IF NOT EXISTS DB01_ChangeHistory AS
SELECT 
    BusinessKey,
    RowVersion,
    EffectiveDate,
    ExpiryDate,
    ImportId,
    StatementDate,
    ProcessedDate,
    IsCurrent,
    
    -- Key business fields for change tracking
    MaKhachHang,
    TenKhachHang,
    SoTaiKhoan,
    SoDu,
    TrangThaiTK,
    LaiSuat
FROM DB01_History
ORDER BY BusinessKey, EffectiveDate DESC;

-- View lịch sử thay đổi KH03
CREATE VIEW IF NOT EXISTS KH03_ChangeHistory AS
SELECT 
    BusinessKey,
    RowVersion,
    EffectiveDate,
    ExpiryDate,
    ImportId,
    StatementDate,
    ProcessedDate,
    IsCurrent,
    
    -- Key business fields for change tracking
    MaKhachHang,
    TenKhachHang,
    MaSoThue,
    DiaChi,
    TrangThaiKH,
    PhanKhuc
FROM KH03_History
ORDER BY BusinessKey, EffectiveDate DESC;

-- View lịch sử thay đổi BC57
CREATE VIEW IF NOT EXISTS BC57_ChangeHistory AS
SELECT 
    BusinessKey,
    RowVersion,
    EffectiveDate,
    ExpiryDate,
    ImportId,
    StatementDate,
    ProcessedDate,
    IsCurrent,
    
    -- Key business fields for change tracking
    MaKhachHang,
    TenKhachHang,
    SoTaiKhoan,
    SoTienGoc,
    TienLaiDuThu,
    TrangThai
FROM BC57_History
ORDER BY BusinessKey, EffectiveDate DESC;

-- =======================================
-- 📊 View tổng hợp import gần đây
-- =======================================
CREATE VIEW IF NOT EXISTS Recent_AllTables_Imports AS
SELECT 
    'LN01' as TableName,
    ImportId,
    StatementDate,
    ProcessedDate,
    COUNT(*) as RecordCount,
    MIN(ProcessedDate) as ImportStartTime,
    MAX(ProcessedDate) as ImportEndTime
FROM LN01_History
WHERE ProcessedDate >= datetime('now', '-30 days')
GROUP BY ImportId, StatementDate, ProcessedDate

UNION ALL

SELECT 
    'LN03' as TableName,
    ImportId,
    StatementDate,
    ProcessedDate,
    COUNT(*) as RecordCount,
    MIN(ProcessedDate) as ImportStartTime,
    MAX(ProcessedDate) as ImportEndTime
FROM LN03_History
WHERE ProcessedDate >= datetime('now', '-30 days')
GROUP BY ImportId, StatementDate, ProcessedDate

UNION ALL

SELECT 
    'GL01' as TableName,
    ImportId,
    StatementDate,
    ProcessedDate,
    COUNT(*) as RecordCount,
    MIN(ProcessedDate) as ImportStartTime,
    MAX(ProcessedDate) as ImportEndTime
FROM GL01_History
WHERE ProcessedDate >= datetime('now', '-30 days')
GROUP BY ImportId, StatementDate, ProcessedDate

UNION ALL

SELECT 
    'DP01' as TableName,
    ImportId,
    StatementDate,
    ProcessedDate,
    COUNT(*) as RecordCount,
    MIN(ProcessedDate) as ImportStartTime,
    MAX(ProcessedDate) as ImportEndTime
FROM DP01_History
WHERE ProcessedDate >= datetime('now', '-30 days')
GROUP BY ImportId, StatementDate, ProcessedDate

UNION ALL

SELECT 
    'EI01' as TableName,
    ImportId,
    StatementDate,
    ProcessedDate,
    COUNT(*) as RecordCount,
    MIN(ProcessedDate) as ImportStartTime,
    MAX(ProcessedDate) as ImportEndTime
FROM EI01_History
WHERE ProcessedDate >= datetime('now', '-30 days')
GROUP BY ImportId, StatementDate, ProcessedDate

UNION ALL

SELECT 
    'DPDA' as TableName,
    ImportId,
    StatementDate,
    ProcessedDate,
    COUNT(*) as RecordCount,
    MIN(ProcessedDate) as ImportStartTime,
    MAX(ProcessedDate) as ImportEndTime
FROM DPDA_History
WHERE ProcessedDate >= datetime('now', '-30 days')
GROUP BY ImportId, StatementDate, ProcessedDate

UNION ALL

SELECT 
    'DB01' as TableName,
    ImportId,
    StatementDate,
    ProcessedDate,
    COUNT(*) as RecordCount,
    MIN(ProcessedDate) as ImportStartTime,
    MAX(ProcessedDate) as ImportEndTime
FROM DB01_History
WHERE ProcessedDate >= datetime('now', '-30 days')
GROUP BY ImportId, StatementDate, ProcessedDate

UNION ALL

SELECT 
    'KH03' as TableName,
    ImportId,
    StatementDate,
    ProcessedDate,
    COUNT(*) as RecordCount,
    MIN(ProcessedDate) as ImportStartTime,
    MAX(ProcessedDate) as ImportEndTime
FROM KH03_History
WHERE ProcessedDate >= datetime('now', '-30 days')
GROUP BY ImportId, StatementDate, ProcessedDate

UNION ALL

SELECT 
    'BC57' as TableName,
    ImportId,
    StatementDate,
    ProcessedDate,
    COUNT(*) as RecordCount,
    MIN(ProcessedDate) as ImportStartTime,
    MAX(ProcessedDate) as ImportEndTime
FROM BC57_History
WHERE ProcessedDate >= datetime('now', '-30 days')
GROUP BY ImportId, StatementDate, ProcessedDate

ORDER BY ProcessedDate DESC;

-- ✅ Views tạo thành công
SELECT '✅ Additional SCD Type 2 views created successfully!' as Status;
