-- =====================================
-- SCRIPT THIẾT LẬP HỆ THỐNG KPI HOÀN CHỈNH
-- Ngày: 12/07/2025
-- Tác giả: GitHub Copilot
-- =====================================

USE TinhKhoanDB;
GO

-- Xóa các bảng cũ nếu tồn tại (để có thể chạy lại script)
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UnitKpiScorings]') AND type in (N'U'))
    DROP TABLE [dbo].[UnitKpiScorings];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EmployeeKpiAssignments]') AND type in (N'U'))
    DROP TABLE [dbo].[EmployeeKpiAssignments];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[KpiIndicators]') AND type in (N'U'))
    DROP TABLE [dbo].[KpiIndicators];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[KpiAssignmentTables]') AND type in (N'U'))
    DROP TABLE [dbo].[KpiAssignmentTables];

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[KhoanPeriods]') AND type in (N'U'))
    DROP TABLE [dbo].[KhoanPeriods];

-- =====================================
-- 1. TẠO BẢNG KHOAN PERIODS (17 kỳ năm 2025)
-- =====================================
CREATE TABLE [dbo].[KhoanPeriods](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [PeriodName] [nvarchar](100) NOT NULL,
    [PeriodCode] [nvarchar](20) NOT NULL,
    [Year] [int] NOT NULL,
    [Month] [int] NULL,
    [Quarter] [int] NULL,
    [StartDate] [datetime2](7) NOT NULL,
    [EndDate] [datetime2](7) NOT NULL,
    [Status] [int] NOT NULL, -- 0=Inactive, 1=Active, 2=Completed
    [Type] [int] NOT NULL, -- 1=Monthly, 2=Quarterly, 3=Yearly
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_KhoanPeriods] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- =====================================
-- 2. TẠO BẢNG KPI ASSIGNMENT TABLES (32 bảng template)
-- =====================================
CREATE TABLE [dbo].[KpiAssignmentTables](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [TableName] [nvarchar](50) NOT NULL,
    [Description] [nvarchar](255) NOT NULL,
    [Category] [nvarchar](20) NOT NULL, -- CANBO hoặc CHINHANH
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_KpiAssignmentTables] PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- =====================================
-- 3. TẠO BẢNG KPI INDICATORS (chỉ tiêu trong mỗi bảng)
-- =====================================
CREATE TABLE [dbo].[KpiIndicators](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [TableId] [int] NOT NULL,
    [IndicatorCode] [nvarchar](20) NOT NULL,
    [IndicatorName] [nvarchar](255) NOT NULL,
    [Unit] [nvarchar](50) NULL,
    [Weight] [decimal](5,2) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_KpiIndicators] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_KpiIndicators_KpiAssignmentTables] FOREIGN KEY([TableId]) REFERENCES [dbo].[KpiAssignmentTables] ([Id])
);

-- =====================================
-- 4. TẠO BẢNG EMPLOYEE KPI ASSIGNMENTS (giao khoán cho cán bộ)
-- =====================================
CREATE TABLE [dbo].[EmployeeKpiAssignments](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [EmployeeId] [int] NOT NULL,
    [IndicatorId] [int] NOT NULL,
    [KhoanPeriodId] [int] NOT NULL,
    [TargetValue] [decimal](18,2) NOT NULL,
    [ActualValue] [decimal](18,2) NULL,
    [Score] [decimal](5,2) NULL,
    [Status] [int] NOT NULL DEFAULT 1, -- 1=Active, 2=Completed, 3=Cancelled
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_EmployeeKpiAssignments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmployeeKpiAssignments_Employees] FOREIGN KEY([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]),
    CONSTRAINT [FK_EmployeeKpiAssignments_KpiIndicators] FOREIGN KEY([IndicatorId]) REFERENCES [dbo].[KpiIndicators] ([Id]),
    CONSTRAINT [FK_EmployeeKpiAssignments_KhoanPeriods] FOREIGN KEY([KhoanPeriodId]) REFERENCES [dbo].[KhoanPeriods] ([Id])
);

-- =====================================
-- 5. TẠO BẢNG UNIT KPI SCORINGS (đánh giá KPI cho chi nhánh)
-- =====================================
CREATE TABLE [dbo].[UnitKpiScorings](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [UnitId] [int] NOT NULL,
    [KhoanPeriodId] [int] NOT NULL,
    [TotalScore] [decimal](5,2) NULL,
    [Ranking] [int] NULL,
    [Status] [int] NOT NULL DEFAULT 1,
    [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_UnitKpiScorings] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UnitKpiScorings_Units] FOREIGN KEY([UnitId]) REFERENCES [dbo].[Units] ([Id]),
    CONSTRAINT [FK_UnitKpiScorings_KhoanPeriods] FOREIGN KEY([KhoanPeriodId]) REFERENCES [dbo].[KhoanPeriods] ([Id])
);

-- =====================================
-- 6. INSERT DỮ LIỆU 17 KHOAN PERIODS NĂM 2025
-- =====================================
INSERT INTO [dbo].[KhoanPeriods] ([PeriodName], [PeriodCode], [Year], [Month], [Quarter], [StartDate], [EndDate], [Status], [Type])
VALUES
-- 12 tháng
('Tháng 1/2025', 'T1-2025', 2025, 1, 1, '2025-01-01', '2025-01-31', 1, 1),
('Tháng 2/2025', 'T2-2025', 2025, 2, 1, '2025-02-01', '2025-02-28', 1, 1),
('Tháng 3/2025', 'T3-2025', 2025, 3, 1, '2025-03-01', '2025-03-31', 1, 1),
('Tháng 4/2025', 'T4-2025', 2025, 4, 2, '2025-04-01', '2025-04-30', 1, 1),
('Tháng 5/2025', 'T5-2025', 2025, 5, 2, '2025-05-01', '2025-05-31', 1, 1),
('Tháng 6/2025', 'T6-2025', 2025, 6, 2, '2025-06-01', '2025-06-30', 1, 1),
('Tháng 7/2025', 'T7-2025', 2025, 7, 3, '2025-07-01', '2025-07-31', 1, 1),
('Tháng 8/2025', 'T8-2025', 2025, 8, 3, '2025-08-01', '2025-08-31', 1, 1),
('Tháng 9/2025', 'T9-2025', 2025, 9, 3, '2025-09-01', '2025-09-30', 1, 1),
('Tháng 10/2025', 'T10-2025', 2025, 10, 4, '2025-10-01', '2025-10-31', 1, 1),
('Tháng 11/2025', 'T11-2025', 2025, 11, 4, '2025-11-01', '2025-11-30', 1, 1),
('Tháng 12/2025', 'T12-2025', 2025, 12, 4, '2025-12-01', '2025-12-31', 1, 1),
-- 4 quý
('Quý 1/2025', 'Q1-2025', 2025, NULL, 1, '2025-01-01', '2025-03-31', 1, 2),
('Quý 2/2025', 'Q2-2025', 2025, NULL, 2, '2025-04-01', '2025-06-30', 1, 2),
('Quý 3/2025', 'Q3-2025', 2025, NULL, 3, '2025-07-01', '2025-09-30', 1, 2),
('Quý 4/2025', 'Q4-2025', 2025, NULL, 4, '2025-10-01', '2025-12-31', 1, 2),
-- 1 năm
('Năm 2025', 'Y-2025', 2025, NULL, NULL, '2025-01-01', '2025-12-31', 1, 3);

-- =====================================
-- 7. INSERT 32 BẢNG KPI ASSIGNMENT TABLES
-- =====================================

-- 23 bảng dành cho cán bộ (Category = "CANBO")
INSERT INTO [dbo].[KpiAssignmentTables] ([TableName], [Description], [Category])
VALUES
('TruongphongKhdn', 'Trưởng phòng KHDN', 'CANBO'),
('TruongphongKhcn', 'Trưởng phòng KHCN', 'CANBO'),
('PhophongKhdn', 'Phó phòng KHDN', 'CANBO'),
('PhophongKhcn', 'Phó phòng KHCN', 'CANBO'),
('TruongphongKhqlrr', 'Trưởng phòng KH&QLRR', 'CANBO'),
('PhophongKhqlrr', 'Phó phòng KH&QLRR', 'CANBO'),
('Cbtd', 'Cán bộ tín dụng', 'CANBO'),
('TruongphongKtnqCnl1', 'Trưởng phòng KTNQ CNL1', 'CANBO'),
('PhophongKtnqCnl1', 'Phó phòng KTNQ CNL1', 'CANBO'),
('Gdv', 'Giao dịch viên', 'CANBO'),
('TqHkKtnb', 'Thủ quỹ | Hậu kiểm | KTNB', 'CANBO'),
('TruongphoItThKtgs', 'Trưởng phó IT | Tổng hợp | KTGS', 'CANBO'),
('CBItThKtgsKhqlrr', 'Cán bộ IT | Tổng hợp | KTGS | KH&QLRR', 'CANBO'),
('GiamdocPgd', 'Giám đốc Phòng giao dịch', 'CANBO'),
('PhogiamdocPgd', 'Phó giám đốc Phòng giao dịch', 'CANBO'),
('PhogiamdocPgdCbtd', 'Phó giám đốc PGD kiêm CBTD', 'CANBO'),
('GiamdocCnl2', 'Giám đốc CNL2', 'CANBO'),
('PhogiamdocCnl2Td', 'Phó giám đốc CNL2 phụ trách TD', 'CANBO'),
('PhogiamdocCnl2Kt', 'Phó giám đốc CNL2 phụ trách KT', 'CANBO'),
('TruongphongKhCnl2', 'Trưởng phòng KH CNL2', 'CANBO'),
('PhophongKhCnl2', 'Phó phòng KH CNL2', 'CANBO'),
('TruongphongKtnqCnl2', 'Trưởng phòng KTNQ CNL2', 'CANBO'),
('PhophongKtnqCnl2', 'Phó phòng KTNQ CNL2', 'CANBO');

-- 9 bảng dành cho chi nhánh (Category = "CHINHANH")
INSERT INTO [dbo].[KpiAssignmentTables] ([TableName], [Description], [Category])
VALUES
('HoiSo', 'KPI cho Hội Sở', 'CHINHANH'),
('BinhLu', 'KPI cho Chi nhánh Bình Lư', 'CHINHANH'),
('PhongTho', 'KPI cho Chi nhánh Phong Thổ', 'CHINHANH'),
('SinHo', 'KPI cho Chi nhánh Sìn Hồ', 'CHINHANH'),
('BumTo', 'KPI cho Chi nhánh Bum Tở', 'CHINHANH'),
('ThanUyen', 'KPI cho Chi nhánh Than Uyên', 'CHINHANH'),
('DoanKet', 'KPI cho Chi nhánh Đoàn Kết', 'CHINHANH'),
('TanUyen', 'KPI cho Chi nhánh Tân Uyên', 'CHINHANH'),
('NamHang', 'KPI cho Chi nhánh Nậm Hàng', 'CHINHANH');

-- =====================================
-- 8. TẠO TEMPORAL TABLES & COLUMNSTORE INDEXES
-- =====================================

-- Thêm Temporal Tables cho audit trail
ALTER TABLE [dbo].[KpiAssignmentTables]
ADD
    SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT SYSUTCDATETIME(),
    SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT '9999-12-31 23:59:59.9999999',
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);

ALTER TABLE [dbo].[KpiAssignmentTables]
SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.KpiAssignmentTables_History));

ALTER TABLE [dbo].[KpiIndicators]
ADD
    SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT SYSUTCDATETIME(),
    SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT '9999-12-31 23:59:59.9999999',
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);

ALTER TABLE [dbo].[KpiIndicators]
SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.KpiIndicators_History));

ALTER TABLE [dbo].[EmployeeKpiAssignments]
ADD
    SysStartTime datetime2 GENERATED ALWAYS AS ROW START HIDDEN DEFAULT SYSUTCDATETIME(),
    SysEndTime datetime2 GENERATED ALWAYS AS ROW END HIDDEN DEFAULT '9999-12-31 23:59:59.9999999',
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);

ALTER TABLE [dbo].[EmployeeKpiAssignments]
SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.EmployeeKpiAssignments_History));

-- Tạo Columnstore Indexes cho analytics
CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_KpiAssignmentTables_Analytics]
ON [dbo].[KpiAssignmentTables] ([Id], [TableName], [Category], [CreatedAt]);

CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_EmployeeKpiAssignments_Analytics]
ON [dbo].[EmployeeKpiAssignments] ([EmployeeId], [IndicatorId], [KhoanPeriodId], [TargetValue], [ActualValue], [Score]);

CREATE NONCLUSTERED COLUMNSTORE INDEX [IX_UnitKpiScorings_Analytics]
ON [dbo].[UnitKpiScorings] ([UnitId], [KhoanPeriodId], [TotalScore], [Ranking]);

-- =====================================
-- 9. THÔNG BÁO HOÀN THÀNH
-- =====================================
PRINT '✅ HOÀN THÀNH THIẾT LẬP HỆ THỐNG KPI:';
PRINT '📋 17 Khoan Periods cho năm 2025';
PRINT '📊 32 KPI Assignment Tables (23 cán bộ + 9 chi nhánh)';
PRINT '⚡ Temporal Tables + Columnstore Indexes';
PRINT '🔗 Foreign Key relationships';
PRINT '🎯 Sẵn sàng populate 158 chỉ tiêu KPI!';

-- Hiển thị thống kê
SELECT 'KhoanPeriods' as TableName, COUNT(*) as RecordCount FROM KhoanPeriods
UNION ALL
SELECT 'KpiAssignmentTables', COUNT(*) FROM KpiAssignmentTables
UNION ALL
SELECT 'KpiIndicators', COUNT(*) FROM KpiIndicators
UNION ALL
SELECT 'EmployeeKpiAssignments', COUNT(*) FROM EmployeeKpiAssignments
UNION ALL
SELECT 'UnitKpiScorings', COUNT(*) FROM UnitKpiScorings;

GO
