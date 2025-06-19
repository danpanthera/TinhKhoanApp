-- Script xóa toàn bộ vai trò
-- Thực hiện ngày: 17/06/2025
-- Mục đích: Xóa hết các vai trò hiện có để chuẩn bị tạo lại

-- Bước 1: Hiển thị thông tin trước khi xóa
SELECT 'TRƯỚC KHI XÓA:' as info;
SELECT COUNT(*) as 'Tong so vai tro' FROM Roles;

-- Bước 2: Xóa tất cả vai trò
DELETE FROM Roles;

-- Bước 3: Reset auto-increment counter
DELETE FROM sqlite_sequence WHERE name='Roles';

-- Bước 4: Kiểm tra kết quả
SELECT 'SAU KHI XÓA:' as info;
SELECT COUNT(*) as 'Tong so vai tro' FROM Roles;

-- Bước 5: Xác nhận auto-increment đã reset
SELECT 'TRẠNG THÁI AUTO-INCREMENT:' as info;
SELECT 
    CASE 
        WHEN EXISTS(SELECT 1 FROM sqlite_sequence WHERE name='Roles') 
        THEN 'Auto-increment tồn tại' 
        ELSE 'Auto-increment đã reset' 
    END as status;

SELECT '✅ HOÀN THÀNH: Đã xóa hết tất cả vai trò!' as result;
