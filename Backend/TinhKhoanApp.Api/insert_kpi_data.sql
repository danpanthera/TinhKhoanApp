-- INSERT 17 KHOAN PERIODS NĂM 2025
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

-- INSERT 32 BẢNG KPI ASSIGNMENT TABLES
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
