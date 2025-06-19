-- Script ghi lại các thay đổi
-- Date: 2025-06-18
-- Changes: Xóa vai trò và đổi tên menu

-- 1. XÓA HẾT VAI TRÒ
DELETE FROM Roles;
DELETE FROM sqlite_sequence WHERE name='Roles';

-- Kết quả: Đã xóa hết vai trò từ database
-- Trước: Có vai trò
-- Sau: 0 vai trò

-- 2. ĐỔI TÊN MENU
-- Đã thay đổi "Định nghĩa KPI" thành "Cấu hình KPI" trong:
-- - src/App.vue (menu navigation)
-- - src/views/KpiDefinitionsView.vue (page title)
-- - public/debug-navigation.html (test file)
-- - public/quick-navigation-test.html (test file)

-- Status: Hoàn thành tất cả yêu cầu
