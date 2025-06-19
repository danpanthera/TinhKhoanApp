-- Thêm các bảng KPI chi nhánh còn thiếu (TableType 25-29)
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, IsActive, CreatedDate) VALUES
(25, 'Chi nhánh H. Tam Dương (7801)', 'Bảng giao khoán KPI cho Chi nhánh H. Tam Dương', 'Dành cho Chi nhánh', 1, datetime('now')),
(26, 'Chi nhánh H. Phong Thổ (7802)', 'Bảng giao khoán KPI cho Chi nhánh H. Phong Thổ', 'Dành cho Chi nhánh', 1, datetime('now')),
(27, 'Chi nhánh H. Sin Hồ (7803)', 'Bảng giao khoán KPI cho Chi nhánh H. Sin Hồ', 'Dành cho Chi nhánh', 1, datetime('now')),
(28, 'Chi nhánh H. Mường Tè (7804)', 'Bảng giao khoán KPI cho Chi nhánh H. Mường Tè', 'Dành cho Chi nhánh', 1, datetime('now')),
(29, 'Chi nhánh H. Than Uyên (7805)', 'Bảng giao khoán KPI cho Chi nhánh H. Than Uyên', 'Dành cho Chi nhánh', 1, datetime('now'));

-- Kiểm tra danh sách các bảng KPI chi nhánh sau khi thêm
SELECT 'Danh sách các bảng KPI chi nhánh sau khi thêm:' as message;
SELECT Id, TableType, TableName, Description FROM KpiAssignmentTables WHERE TableType >= 24 ORDER BY TableType;
