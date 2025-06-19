-- Script cập nhật TableType để tách biệt chi nhánh và cán bộ
-- Chi nhánh sẽ có TableType từ 200-208 để sắp xếp theo mã 7800-7808

-- Cập nhật chi nhánh theo thứ tự mã tăng dần
UPDATE KpiAssignmentTables SET TableType = 200 WHERE TableType = 100; -- Hội sở (7800)
UPDATE KpiAssignmentTables SET TableType = 201 WHERE TableType = 101; -- Chi nhánh H. Tam Dương (7801)
UPDATE KpiAssignmentTables SET TableType = 202 WHERE TableType = 102; -- Chi nhánh H. Phong Thổ (7802)
UPDATE KpiAssignmentTables SET TableType = 203 WHERE TableType = 103; -- Chi nhánh H. Sin Hồ (7803)
UPDATE KpiAssignmentTables SET TableType = 204 WHERE TableType = 104; -- Chi nhánh H. Mường Tè (7804)
UPDATE KpiAssignmentTables SET TableType = 205 WHERE TableType = 105; -- Chi nhánh H. Than Uyên (7805)
UPDATE KpiAssignmentTables SET TableType = 206 WHERE TableType = 106; -- Chi nhánh Thành Phố (7806)
UPDATE KpiAssignmentTables SET TableType = 207 WHERE TableType = 107; -- Chi nhánh H. Tân Uyên (7807)
UPDATE KpiAssignmentTables SET TableType = 208 WHERE TableType = 108; -- Chi nhánh H. Nậm Nhùn (7808)

-- Kiểm tra danh sách chi nhánh sau khi cập nhật
SELECT 'Danh sách chi nhánh theo thứ tự mã 7800-7808:' as message;
SELECT Id, TableType, TableName FROM KpiAssignmentTables WHERE TableType >= 200 ORDER BY TableType;

-- Kiểm tra danh sách cán bộ (chỉ còn 23 bảng)
SELECT 'Danh sách bảng KPI cán bộ (23 bảng):' as message;
SELECT COUNT(*) as TotalStaffTables FROM KpiAssignmentTables WHERE TableType < 200;
