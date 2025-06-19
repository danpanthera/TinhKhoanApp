-- Script để dọn sạch dữ liệu KPI cũ theo vai trò (SQLite compatible)
-- Được tạo để chuẩn bị cho việc định nghĩa lại chỉ tiêu mới cho 23 vai trò chuẩn

BEGIN TRANSACTION;

-- 1. Xóa tất cả dữ liệu từ bảng KPIDefinitions (chỉ tiêu KPI theo vai trò)
DELETE FROM KPIDefinitions;
SELECT 'Đã xóa tất cả dữ liệu từ bảng KPIDefinitions' AS result;

-- 2. Xóa tất cả dữ liệu từ bảng EmployeeKpiAssignments liên quan KPI cũ
DELETE FROM EmployeeKpiAssignments;
SELECT 'Đã xóa tất cả dữ liệu từ bảng EmployeeKpiAssignments' AS result;

-- 3. Xóa tất cả dữ liệu từ bảng EmployeeKpiTargets nếu có data cũ
DELETE FROM EmployeeKpiTargets;
SELECT 'Đã xóa tất cả dữ liệu từ bảng EmployeeKpiTargets' AS result;

-- 4. Reset ID sequence cho SQLite
UPDATE sqlite_sequence SET seq = 0 WHERE name = 'KPIDefinitions';
UPDATE sqlite_sequence SET seq = 0 WHERE name = 'EmployeeKpiAssignments';
UPDATE sqlite_sequence SET seq = 0 WHERE name = 'EmployeeKpiTargets';
SELECT 'Đã reset sequence cho các bảng KPI' AS result;

-- 5. Kiểm tra kết quả sau khi dọn sạch
SELECT COUNT(*) AS remaining_kpi_definitions FROM KPIDefinitions;
SELECT COUNT(*) AS remaining_kpi_assignments FROM EmployeeKpiAssignments;
SELECT COUNT(*) AS remaining_kpi_targets FROM EmployeeKpiTargets;

SELECT 'Hoàn thành dọn sạch dữ liệu KPI cũ theo vai trò' AS result;
SELECT 'Hệ thống đã sẵn sàng để định nghĩa lại chỉ tiêu mới cho 23 vai trò chuẩn' AS result;

COMMIT;
