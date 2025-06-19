-- Script để dọn sạch dữ liệu KPI cũ theo vai trò
-- Được tạo để chuẩn bị cho việc định nghĩa lại chỉ tiêu mới cho 23 vai trò chuẩn

BEGIN TRANSACTION;

-- 1. Xóa tất cả dữ liệu từ bảng KPIDefinitions (chỉ tiêu KPI theo vai trò)
DELETE FROM KPIDefinitions;
PRINT 'Đã xóa tất cả dữ liệu từ bảng KPIDefinitions';

-- 2. Xóa tất cả dữ liệu từ bảng KpiDefinitionMaxScores nếu có
-- (Bảng này có thể chứa dữ liệu liên quan đến điểm tối đa của chỉ tiêu)
DELETE FROM KpiDefinitionMaxScores WHERE 1=1;
PRINT 'Đã xóa tất cả dữ liệu từ bảng KpiDefinitionMaxScores (nếu có)';

-- 3. Reset ID sequence cho SQLite (nếu cần)
-- Với SQLite, AUTOINCREMENT sẽ tự động được reset khi bảng trống
UPDATE sqlite_sequence SET seq = 0 WHERE name = 'KPIDefinitions';
UPDATE sqlite_sequence SET seq = 0 WHERE name = 'KpiDefinitionMaxScores';
PRINT 'Đã reset sequence cho các bảng KPI';

-- 4. Xóa các role-based KPI assignments cũ nếu có
-- (Tùy thuộc vào cấu trúc database, có thể có bảng liên kết giữa role và KPI)
-- DELETE FROM RoleKpiAssignments WHERE 1=1; -- Uncomment nếu bảng này tồn tại

PRINT 'Hoàn thành dọn sạch dữ liệu KPI cũ theo vai trò';
PRINT 'Hệ thống đã sẵn sàng để định nghĩa lại chỉ tiêu mới cho 23 vai trò chuẩn';

COMMIT;
