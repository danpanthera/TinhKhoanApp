-- 🏗️ Script tạo bảng SCD Type 2 cho các loại dữ liệu thô còn lại
-- Mở rộng cho: LN03, EI01, DPDA, DB01, KH03, BC57
-- Thiết kế: Slowly Changing Dimension Type 2 với versioning và thời gian hiệu lực

-- =======================================
-- 📊 LN03 - Dữ liệu Nợ XLRR History
-- =======================================
CREATE TABLE IF NOT EXISTS LN03_History (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    
    -- 🔑 Business Key (để identify unique record)
    BusinessKey TEXT NOT NULL,
    
    -- 📅 SCD Type 2 Fields
    EffectiveDate DATETIME NOT NULL,
    ExpiryDate DATETIME,
    IsCurrent BOOLEAN NOT NULL DEFAULT 1,
    RowVersion INTEGER NOT NULL DEFAULT 1,
    
    -- 🗃️ Metadata Fields
    ImportId TEXT NOT NULL,
    StatementDate DATE NOT NULL,
    ProcessedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DataHash TEXT NOT NULL,
    
    -- 📊 Business Data Fields for LN03 (Nợ XLRR)
    MaKhachHang TEXT,
    TenKhachHang TEXT,
    MaHopDong TEXT,
    LoaiHopDong TEXT,
    SoTienGoc DECIMAL(18,2),
    SoTienLai DECIMAL(18,2),
    TongNo DECIMAL(18,2),
    NgayDaoHan DATE,
    TinhTrangNo TEXT,
    NhomNo INTEGER,
    MaChiNhanh TEXT,
    TenChiNhanh TEXT,
    NgayTao DATE,
    NgayCapNhat DATE,
    
    -- 📝 Additional JSON field for flexible data
    AdditionalData TEXT,
    
    -- 🏷️ Indexes for performance
    UNIQUE(BusinessKey, EffectiveDate)
);

-- =======================================
-- 📊 EI01 - Dữ liệu Mobile Banking History  
-- =======================================
CREATE TABLE IF NOT EXISTS EI01_History (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    
    -- 🔑 Business Key
    BusinessKey TEXT NOT NULL,
    
    -- 📅 SCD Type 2 Fields
    EffectiveDate DATETIME NOT NULL,
    ExpiryDate DATETIME,
    IsCurrent BOOLEAN NOT NULL DEFAULT 1,
    RowVersion INTEGER NOT NULL DEFAULT 1,
    
    -- 🗃️ Metadata Fields
    ImportId TEXT NOT NULL,
    StatementDate DATE NOT NULL,
    ProcessedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DataHash TEXT NOT NULL,
    
    -- 📊 Business Data Fields for EI01 (Mobile Banking)
    MaKhachHang TEXT,
    TenKhachHang TEXT,
    SoTaiKhoan TEXT,
    LoaiGiaoDich TEXT,
    MaGiaoDich TEXT,
    SoTien DECIMAL(18,2),
    NgayGiaoDich DATETIME,
    ThoiGianGiaoDich DATETIME,
    TrangThaiGiaoDich TEXT,
    NoiDungGiaoDich TEXT,
    MaChiNhanh TEXT,
    TenChiNhanh TEXT,
    Channel TEXT,
    DeviceInfo TEXT,
    
    -- 📝 Additional JSON field for flexible data
    AdditionalData TEXT,
    
    -- 🏷️ Indexes for performance
    UNIQUE(BusinessKey, EffectiveDate)
);

-- =======================================
-- 📊 DPDA - Dữ liệu Sao kê phát hành thẻ History
-- =======================================
CREATE TABLE IF NOT EXISTS DPDA_History (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    
    -- 🔑 Business Key
    BusinessKey TEXT NOT NULL,
    
    -- 📅 SCD Type 2 Fields
    EffectiveDate DATETIME NOT NULL,
    ExpiryDate DATETIME,
    IsCurrent BOOLEAN NOT NULL DEFAULT 1,
    RowVersion INTEGER NOT NULL DEFAULT 1,
    
    -- 🗃️ Metadata Fields
    ImportId TEXT NOT NULL,
    StatementDate DATE NOT NULL,
    ProcessedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DataHash TEXT NOT NULL,
    
    -- 📊 Business Data Fields for DPDA (Phát hành thẻ)
    MaKhachHang TEXT,
    TenKhachHang TEXT,
    SoThe TEXT,
    LoaiThe TEXT,
    TrangThaiThe TEXT,
    NgayPhatHanh DATE,
    NgayHetHan DATE,
    HanMucThe DECIMAL(18,2),
    SoDuHienTai DECIMAL(18,2),
    SoTienDaSD DECIMAL(18,2),
    MaChiNhanh TEXT,
    TenChiNhanh TEXT,
    NgayTao DATE,
    NgayCapNhat DATE,
    
    -- 📝 Additional JSON field for flexible data
    AdditionalData TEXT,
    
    -- 🏷️ Indexes for performance
    UNIQUE(BusinessKey, EffectiveDate)
);

-- =======================================
-- 📊 DB01 - Sao kê TSDB và Không TSDB History
-- =======================================
CREATE TABLE IF NOT EXISTS DB01_History (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    
    -- 🔑 Business Key
    BusinessKey TEXT NOT NULL,
    
    -- 📅 SCD Type 2 Fields
    EffectiveDate DATETIME NOT NULL,
    ExpiryDate DATETIME,
    IsCurrent BOOLEAN NOT NULL DEFAULT 1,
    RowVersion INTEGER NOT NULL DEFAULT 1,
    
    -- 🗃️ Metadata Fields
    ImportId TEXT NOT NULL,
    StatementDate DATE NOT NULL,
    ProcessedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DataHash TEXT NOT NULL,
    
    -- 📊 Business Data Fields for DB01 (TSDB)
    MaKhachHang TEXT,
    TenKhachHang TEXT,
    SoTaiKhoan TEXT,
    LoaiTaiKhoan TEXT,
    SoDu DECIMAL(18,2),
    SoDuKhaDung DECIMAL(18,2),
    NgayMoTK DATE,
    TrangThaiTK TEXT,
    LaiSuat DECIMAL(5,4),
    KyHan INTEGER,
    NgayDaoHan DATE,
    SoTienGocGuy DECIMAL(18,2),
    TienLaiDuThu DECIMAL(18,2),
    MaChiNhanh TEXT,
    TenChiNhanh TEXT,
    LoaiHinhDB TEXT, -- TSDB/Không TSDB
    
    -- 📝 Additional JSON field for flexible data
    AdditionalData TEXT,
    
    -- 🏷️ Indexes for performance
    UNIQUE(BusinessKey, EffectiveDate)
);

-- =======================================
-- 📊 KH03 - Sao kê Khách hàng pháp nhân History
-- =======================================
CREATE TABLE IF NOT EXISTS KH03_History (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    
    -- 🔑 Business Key
    BusinessKey TEXT NOT NULL,
    
    -- 📅 SCD Type 2 Fields
    EffectiveDate DATETIME NOT NULL,
    ExpiryDate DATETIME,
    IsCurrent BOOLEAN NOT NULL DEFAULT 1,
    RowVersion INTEGER NOT NULL DEFAULT 1,
    
    -- 🗃️ Metadata Fields
    ImportId TEXT NOT NULL,
    StatementDate DATE NOT NULL,
    ProcessedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DataHash TEXT NOT NULL,
    
    -- 📊 Business Data Fields for KH03 (Khách hàng pháp nhân)
    MaKhachHang TEXT,
    TenKhachHang TEXT,
    MaSoThue TEXT,
    DiaChi TEXT,
    SoDienThoai TEXT,
    Email TEXT,
    NguoiDaiDien TEXT,
    ChucVuNDD TEXT,
    NgaySinh DATE,
    CMND_CCCD TEXT,
    NgayCapCMND DATE,
    NoiCapCMND TEXT,
    LoaiKhachHang TEXT,
    PhanKhuc TEXT,
    MaChiNhanh TEXT,
    TenChiNhanh TEXT,
    TrangThaiKH TEXT,
    NgayTao DATE,
    NgayCapNhat DATE,
    
    -- 📝 Additional JSON field for flexible data
    AdditionalData TEXT,
    
    -- 🏷️ Indexes for performance
    UNIQUE(BusinessKey, EffectiveDate)
);

-- =======================================
-- 📊 BC57 - Sao kê Lãi dự thu History
-- =======================================
CREATE TABLE IF NOT EXISTS BC57_History (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    
    -- 🔑 Business Key
    BusinessKey TEXT NOT NULL,
    
    -- 📅 SCD Type 2 Fields
    EffectiveDate DATETIME NOT NULL,
    ExpiryDate DATETIME,
    IsCurrent BOOLEAN NOT NULL DEFAULT 1,
    RowVersion INTEGER NOT NULL DEFAULT 1,
    
    -- 🗃️ Metadata Fields
    ImportId TEXT NOT NULL,
    StatementDate DATE NOT NULL,
    ProcessedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    DataHash TEXT NOT NULL,
    
    -- 📊 Business Data Fields for BC57 (Lãi dự thu)
    MaKhachHang TEXT,
    TenKhachHang TEXT,
    SoTaiKhoan TEXT,
    MaHopDong TEXT,
    LoaiSanPham TEXT,
    SoTienGoc DECIMAL(18,2),
    LaiSuat DECIMAL(5,4),
    SoNgayTinhLai INTEGER,
    TienLaiDuThu DECIMAL(18,2),
    TienLaiQuaHan DECIMAL(18,2),
    NgayBatDau DATE,
    NgayKetThuc DATE,
    TrangThai TEXT,
    MaChiNhanh TEXT,
    TenChiNhanh TEXT,
    NgayTinhLai DATE,
    
    -- 📝 Additional JSON field for flexible data
    AdditionalData TEXT,
    
    -- 🏷️ Indexes for performance
    UNIQUE(BusinessKey, EffectiveDate)
);

-- =======================================
-- 🗂️ Staging Tables for Import Process
-- =======================================

-- Staging table for LN03
CREATE TABLE IF NOT EXISTS LN03_Staging (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ImportId TEXT NOT NULL,
    BusinessKey TEXT NOT NULL,
    StatementDate DATE NOT NULL,
    DataHash TEXT NOT NULL,
    
    -- Business fields (same as History table)
    MaKhachHang TEXT,
    TenKhachHang TEXT,
    MaHopDong TEXT,
    LoaiHopDong TEXT,
    SoTienGoc DECIMAL(18,2),
    SoTienLai DECIMAL(18,2),
    TongNo DECIMAL(18,2),
    NgayDaoHan DATE,
    TinhTrangNo TEXT,
    NhomNo INTEGER,
    MaChiNhanh TEXT,
    TenChiNhanh TEXT,
    NgayTao DATE,
    NgayCapNhat DATE,
    AdditionalData TEXT,
    
    ProcessedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Staging table for EI01
CREATE TABLE IF NOT EXISTS EI01_Staging (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ImportId TEXT NOT NULL,
    BusinessKey TEXT NOT NULL,
    StatementDate DATE NOT NULL,
    DataHash TEXT NOT NULL,
    
    -- Business fields
    MaKhachHang TEXT,
    TenKhachHang TEXT,
    SoTaiKhoan TEXT,
    LoaiGiaoDich TEXT,
    MaGiaoDich TEXT,
    SoTien DECIMAL(18,2),
    NgayGiaoDich DATETIME,
    ThoiGianGiaoDich DATETIME,
    TrangThaiGiaoDich TEXT,
    NoiDungGiaoDich TEXT,
    MaChiNhanh TEXT,
    TenChiNhanh TEXT,
    Channel TEXT,
    DeviceInfo TEXT,
    AdditionalData TEXT,
    
    ProcessedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Staging table for DPDA
CREATE TABLE IF NOT EXISTS DPDA_Staging (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ImportId TEXT NOT NULL,
    BusinessKey TEXT NOT NULL,
    StatementDate DATE NOT NULL,
    DataHash TEXT NOT NULL,
    
    -- Business fields
    MaKhachHang TEXT,
    TenKhachHang TEXT,
    SoThe TEXT,
    LoaiThe TEXT,
    TrangThaiThe TEXT,
    NgayPhatHanh DATE,
    NgayHetHan DATE,
    HanMucThe DECIMAL(18,2),
    SoDuHienTai DECIMAL(18,2),
    SoTienDaSD DECIMAL(18,2),
    MaChiNhanh TEXT,
    TenChiNhanh TEXT,
    NgayTao DATE,
    NgayCapNhat DATE,
    AdditionalData TEXT,
    
    ProcessedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Staging table for DB01
CREATE TABLE IF NOT EXISTS DB01_Staging (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ImportId TEXT NOT NULL,
    BusinessKey TEXT NOT NULL,
    StatementDate DATE NOT NULL,
    DataHash TEXT NOT NULL,
    
    -- Business fields
    MaKhachHang TEXT,
    TenKhachHang TEXT,
    SoTaiKhoan TEXT,
    LoaiTaiKhoan TEXT,
    SoDu DECIMAL(18,2),
    SoDuKhaDung DECIMAL(18,2),
    NgayMoTK DATE,
    TrangThaiTK TEXT,
    LaiSuat DECIMAL(5,4),
    KyHan INTEGER,
    NgayDaoHan DATE,
    SoTienGocGuy DECIMAL(18,2),
    TienLaiDuThu DECIMAL(18,2),
    MaChiNhanh TEXT,
    TenChiNhanh TEXT,
    LoaiHinhDB TEXT,
    AdditionalData TEXT,
    
    ProcessedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Staging table for KH03
CREATE TABLE IF NOT EXISTS KH03_Staging (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ImportId TEXT NOT NULL,
    BusinessKey TEXT NOT NULL,
    StatementDate DATE NOT NULL,
    DataHash TEXT NOT NULL,
    
    -- Business fields
    MaKhachHang TEXT,
    TenKhachHang TEXT,
    MaSoThue TEXT,
    DiaChi TEXT,
    SoDienThoai TEXT,
    Email TEXT,
    NguoiDaiDien TEXT,
    ChucVuNDD TEXT,
    NgaySinh DATE,
    CMND_CCCD TEXT,
    NgayCapCMND DATE,
    NoiCapCMND TEXT,
    LoaiKhachHang TEXT,
    PhanKhuc TEXT,
    MaChiNhanh TEXT,
    TenChiNhanh TEXT,
    TrangThaiKH TEXT,
    NgayTao DATE,
    NgayCapNhat DATE,
    AdditionalData TEXT,
    
    ProcessedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Staging table for BC57
CREATE TABLE IF NOT EXISTS BC57_Staging (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ImportId TEXT NOT NULL,
    BusinessKey TEXT NOT NULL,
    StatementDate DATE NOT NULL,
    DataHash TEXT NOT NULL,
    
    -- Business fields
    MaKhachHang TEXT,
    TenKhachHang TEXT,
    SoTaiKhoan TEXT,
    MaHopDong TEXT,
    LoaiSanPham TEXT,
    SoTienGoc DECIMAL(18,2),
    LaiSuat DECIMAL(5,4),
    SoNgayTinhLai INTEGER,
    TienLaiDuThu DECIMAL(18,2),
    TienLaiQuaHan DECIMAL(18,2),
    NgayBatDau DATE,
    NgayKetThuc DATE,
    TrangThai TEXT,
    MaChiNhanh TEXT,
    TenChiNhanh TEXT,
    NgayTinhLai DATE,
    AdditionalData TEXT,
    
    ProcessedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- =======================================
-- 🗂️ Performance Indexes
-- =======================================

-- Indexes for LN03_History
CREATE INDEX IF NOT EXISTS idx_ln03_history_business_key ON LN03_History(BusinessKey);
CREATE INDEX IF NOT EXISTS idx_ln03_history_current ON LN03_History(IsCurrent, BusinessKey);
CREATE INDEX IF NOT EXISTS idx_ln03_history_statement_date ON LN03_History(StatementDate);
CREATE INDEX IF NOT EXISTS idx_ln03_history_import_id ON LN03_History(ImportId);
CREATE INDEX IF NOT EXISTS idx_ln03_history_ma_kh ON LN03_History(MaKhachHang);

-- Indexes for EI01_History
CREATE INDEX IF NOT EXISTS idx_ei01_history_business_key ON EI01_History(BusinessKey);
CREATE INDEX IF NOT EXISTS idx_ei01_history_current ON EI01_History(IsCurrent, BusinessKey);
CREATE INDEX IF NOT EXISTS idx_ei01_history_statement_date ON EI01_History(StatementDate);
CREATE INDEX IF NOT EXISTS idx_ei01_history_import_id ON EI01_History(ImportId);
CREATE INDEX IF NOT EXISTS idx_ei01_history_ma_kh ON EI01_History(MaKhachHang);

-- Indexes for DPDA_History
CREATE INDEX IF NOT EXISTS idx_dpda_history_business_key ON DPDA_History(BusinessKey);
CREATE INDEX IF NOT EXISTS idx_dpda_history_current ON DPDA_History(IsCurrent, BusinessKey);
CREATE INDEX IF NOT EXISTS idx_dpda_history_statement_date ON DPDA_History(StatementDate);
CREATE INDEX IF NOT EXISTS idx_dpda_history_import_id ON DPDA_History(ImportId);
CREATE INDEX IF NOT EXISTS idx_dpda_history_ma_kh ON DPDA_History(MaKhachHang);

-- Indexes for DB01_History
CREATE INDEX IF NOT EXISTS idx_db01_history_business_key ON DB01_History(BusinessKey);
CREATE INDEX IF NOT EXISTS idx_db01_history_current ON DB01_History(IsCurrent, BusinessKey);
CREATE INDEX IF NOT EXISTS idx_db01_history_statement_date ON DB01_History(StatementDate);
CREATE INDEX IF NOT EXISTS idx_db01_history_import_id ON DB01_History(ImportId);
CREATE INDEX IF NOT EXISTS idx_db01_history_ma_kh ON DB01_History(MaKhachHang);

-- Indexes for KH03_History
CREATE INDEX IF NOT EXISTS idx_kh03_history_business_key ON KH03_History(BusinessKey);
CREATE INDEX IF NOT EXISTS idx_kh03_history_current ON KH03_History(IsCurrent, BusinessKey);
CREATE INDEX IF NOT EXISTS idx_kh03_history_statement_date ON KH03_History(StatementDate);
CREATE INDEX IF NOT EXISTS idx_kh03_history_import_id ON KH03_History(ImportId);
CREATE INDEX IF NOT EXISTS idx_kh03_history_ma_kh ON KH03_History(MaKhachHang);

-- Indexes for BC57_History
CREATE INDEX IF NOT EXISTS idx_bc57_history_business_key ON BC57_History(BusinessKey);
CREATE INDEX IF NOT EXISTS idx_bc57_history_current ON BC57_History(IsCurrent, BusinessKey);
CREATE INDEX IF NOT EXISTS idx_bc57_history_statement_date ON BC57_History(StatementDate);
CREATE INDEX IF NOT EXISTS idx_bc57_history_import_id ON BC57_History(ImportId);
CREATE INDEX IF NOT EXISTS idx_bc57_history_ma_kh ON BC57_History(MaKhachHang);

-- =======================================
-- 🧹 Cleanup old staging data (Optional)
-- =======================================

-- Create cleanup triggers to remove old staging data after 7 days
CREATE TRIGGER IF NOT EXISTS cleanup_ln03_staging 
AFTER INSERT ON LN03_History
BEGIN
    DELETE FROM LN03_Staging 
    WHERE ProcessedDate < datetime('now', '-7 days');
END;

CREATE TRIGGER IF NOT EXISTS cleanup_ei01_staging 
AFTER INSERT ON EI01_History
BEGIN
    DELETE FROM EI01_Staging 
    WHERE ProcessedDate < datetime('now', '-7 days');
END;

CREATE TRIGGER IF NOT EXISTS cleanup_dpda_staging 
AFTER INSERT ON DPDA_History
BEGIN
    DELETE FROM DPDA_Staging 
    WHERE ProcessedDate < datetime('now', '-7 days');
END;

CREATE TRIGGER IF NOT EXISTS cleanup_db01_staging 
AFTER INSERT ON DB01_History
BEGIN
    DELETE FROM DB01_Staging 
    WHERE ProcessedDate < datetime('now', '-7 days');
END;

CREATE TRIGGER IF NOT EXISTS cleanup_kh03_staging 
AFTER INSERT ON KH03_History
BEGIN
    DELETE FROM KH03_Staging 
    WHERE ProcessedDate < datetime('now', '-7 days');
END;

CREATE TRIGGER IF NOT EXISTS cleanup_bc57_staging 
AFTER INSERT ON BC57_History
BEGIN
    DELETE FROM BC57_Staging 
    WHERE ProcessedDate < datetime('now', '-7 days');
END;

-- ✅ Script hoàn thành
SELECT '✅ Additional SCD Type 2 tables created successfully!' as Status;
