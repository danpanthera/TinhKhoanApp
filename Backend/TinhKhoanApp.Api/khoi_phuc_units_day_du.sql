-- Script khôi phục đầy đủ dữ liệu Units và KPI Assignments
-- CN Lai Châu gồm 06 phòng và 8 chi nhánh loại 2

-- 1. Xóa dữ liệu cũ và thêm Units đầy đủ
DELETE FROM Units WHERE Id > 1;
DELETE FROM EmployeeKpiAssignments;
DELETE FROM UnitKpiScorings;

-- Thêm CN Lai Châu và các đơn vị con
INSERT INTO Units (Id, Code, Name, Description, Type, IsActive, CreatedDate) VALUES
-- CN chính và các phòng
(2, 'CNH_LC', 'Chi nhánh Lai Châu', 'Chi nhánh chính tại Lai Châu', 'BRANCH', 1, '2025-06-15 11:32:55'),
(3, 'PHONG_TCKT', 'Phòng Tín dụng - Kiểm toán', 'Phòng Tín dụng và Kiểm toán nội bộ', 'DEPARTMENT', 1, '2025-06-15 11:32:55'),
(4, 'PHONG_KHDN', 'Phòng Khách hàng doanh nghiệp', 'Phòng Khách hàng doanh nghiệp', 'DEPARTMENT', 1, '2025-06-15 11:32:55'),
(5, 'PHONG_KHCN', 'Phòng Khách hàng cá nhân', 'Phòng Khách hàng cá nhân', 'DEPARTMENT', 1, '2025-06-15 11:32:55'),
(6, 'PHONG_QLRR', 'Phòng Quản lý rủi ro', 'Phòng Quản lý rủi ro', 'DEPARTMENT', 1, '2025-06-15 11:32:55'),
(7, 'PHONG_TCNH', 'Phòng Tài chính - Ngân hàng', 'Phòng Tài chính và Ngân hàng', 'DEPARTMENT', 1, '2025-06-15 11:32:55'),
(8, 'PHONG_HCNS', 'Phòng Hành chính - Nhân sự', 'Phòng Hành chính và Nhân sự', 'DEPARTMENT', 1, '2025-06-15 11:32:55'),

-- 8 Chi nhánh loại 2
(9, 'CN2_THAN_UYEN', 'Chi nhánh Than Uyên', 'Chi nhánh loại 2 tại Than Uyên', 'BRANCH_L2', 1, '2025-06-15 11:32:55'),
(10, 'CN2_TAM_DUONG', 'Chi nhánh Tâm Đường', 'Chi nhánh loại 2 tại Tâm Đường', 'BRANCH_L2', 1, '2025-06-15 11:32:55'),
(11, 'CN2_PHONG_THO', 'Chi nhánh Phong Thổ', 'Chi nhánh loại 2 tại Phong Thổ', 'BRANCH_L2', 1, '2025-06-15 11:32:55'),
(12, 'CN2_MUONG_TE', 'Chi nhánh Mường Tè', 'Chi nhánh loại 2 tại Mường Tè', 'BRANCH_L2', 1, '2025-06-15 11:32:55'),
(13, 'CN2_SIN_HO', 'Chi nhánh Sìn Hồ', 'Chi nhánh loại 2 tại Sìn Hồ', 'BRANCH_L2', 1, '2025-06-15 11:32:55'),
(14, 'CN2_NAM_NHUN', 'Chi nhánh Nậm Nhùn', 'Chi nhánh loại 2 tại Nậm Nhùn', 'BRANCH_L2', 1, '2025-06-15 11:32:55'),
(15, 'CN2_NOONG_HET', 'Chi nhánh Noong Hẹt', 'Chi nhánh loại 2 tại Noong Hẹt', 'BRANCH_L2', 1, '2025-06-15 11:32:55'),
(16, 'CN2_TAN_UYEN', 'Chi nhánh Tân Uyên', 'Chi nhánh loại 2 tại Tân Uyên', 'BRANCH_L2', 1, '2025-06-15 11:32:55'),

-- Phòng giao dịch tương ứng với từng chi nhánh loại 2
(17, 'PGD_THAN_UYEN', 'Phòng GD Than Uyên', 'Phòng giao dịch Than Uyên', 'TRANSACTION_OFFICE', 1, '2025-06-15 11:32:55'),
(18, 'PGD_TAM_DUONG', 'Phòng GD Tâm Đường', 'Phòng giao dịch Tâm Đường', 'TRANSACTION_OFFICE', 1, '2025-06-15 11:32:55'),
(19, 'PGD_PHONG_THO', 'Phòng GD Phong Thổ', 'Phòng giao dịch Phong Thổ', 'TRANSACTION_OFFICE', 1, '2025-06-15 11:32:55'),
(20, 'PGD_MUONG_TE', 'Phòng GD Mường Tè', 'Phòng giao dịch Mường Tè', 'TRANSACTION_OFFICE', 1, '2025-06-15 11:32:55'),
(21, 'PGD_SIN_HO', 'Phòng GD Sìn Hồ', 'Phòng giao dịch Sìn Hồ', 'TRANSACTION_OFFICE', 1, '2025-06-15 11:32:55'),
(22, 'PGD_NAM_NHUN', 'Phòng GD Nậm Nhùn', 'Phòng giao dịch Nậm Nhùn', 'TRANSACTION_OFFICE', 1, '2025-06-15 11:32:55'),
(23, 'PGD_NOONG_HET', 'Phòng GD Noong Hẹt', 'Phòng giao dịch Noong Hẹt', 'TRANSACTION_OFFICE', 1, '2025-06-15 11:32:55'),
(24, 'PGD_TAN_UYEN', 'Phòng GD Tân Uyên', 'Phòng giao dịch Tân Uyên', 'TRANSACTION_OFFICE', 1, '2025-06-15 11:32:55');

-- Kiểm tra kết quả
SELECT COUNT(*) as total_units FROM Units;
SELECT Code, Name, Type FROM Units WHERE Id > 1 ORDER BY Id;
