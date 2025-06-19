-- Script cập nhật tên chi nhánh với mã số để sắp xếp đúng thứ tự
-- Cập nhật các chi nhánh với mã số từ 7800-7808

-- Hội sở tỉnh (7800)
UPDATE KpiAssignmentTables 
SET TableName = 'Hội sở (7800)'
WHERE Id = 30 AND TableName = 'Hội sở';

-- Chi nhánh H. Tam Đường (7801) - đã có mã
-- Chi nhánh H. Phong Thổ (7802) - đã có mã

-- Chi nhánh H. Sìn Hồ (7803)
UPDATE KpiAssignmentTables 
SET TableName = 'Chi nhánh H. Sìn Hồ (7803)'
WHERE Id = 27 AND TableName = 'Chi nhánh H. Sìn Hồ';

-- Chi nhánh H. Mường Tè (7804) 
SET TableName = 'Chi nhánh H. Mường Tè (7804)'
WHERE Id = 28 AND TableName = 'Chi nhánh H. Mường Tè';

-- Chi nhánh H. Than Uyên (7805)
UPDATE KpiAssignmentTables 
SET TableName = 'Chi nhánh H. Than Uyên (7805)'
WHERE Id = 29 AND TableName = 'Chi nhánh H. Than Uyên';

-- Chi nhánh Thành Phố (7806) 
UPDATE KpiAssignmentTables 
SET TableName = 'Chi nhánh Thành Phố (7806)'
WHERE Id = 30 AND TableName = 'Chi nhánh Thành Phố';

-- Chi nhánh H. Tân Uyên (7807)
UPDATE KpiAssignmentTables 
SET TableName = 'Chi nhánh H. Tân Uyên (7807)'
WHERE Id = 31 AND TableName = 'Chi nhánh H. Tân Uyên';

-- Chi nhánh H. Nậm Nhùn (7808)
UPDATE KpiAssignmentTables 
SET TableName = 'Chi nhánh H. Nậm Nhùn (7808)'
WHERE Id = 32 AND TableName = 'Chi nhánh H. Nậm Nhùn';



-- Kiểm tra kết quả
SELECT 'Danh sách chi nhánh sau khi cập nhật mã số:' as info;
SELECT Id, TableName 
FROM KpiAssignmentTables 
WHERE Category = 'Dành cho Chi nhánh' 
ORDER BY 
  CASE 
    WHEN TableName LIKE '%(7800)%' THEN 1
    WHEN TableName LIKE '%(7801)%' THEN 2
    WHEN TableName LIKE '%(7802)%' THEN 3
    WHEN TableName LIKE '%(7803)%' THEN 4
    WHEN TableName LIKE '%(7804)%' THEN 5
    WHEN TableName LIKE '%(7805)%' THEN 6
    WHEN TableName LIKE '%(7806)%' THEN 7
    WHEN TableName LIKE '%(7807)%' THEN 8
    WHEN TableName LIKE '%(7808)%' THEN 9
    ELSE 99
  END;
