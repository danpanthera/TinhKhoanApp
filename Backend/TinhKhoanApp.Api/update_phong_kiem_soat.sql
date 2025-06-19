-- Script SQL để sửa mô tả từ "Kiểm soát" thành "Kế hoạch"
-- Thực thi script này trong database để cập nhật dữ liệu

-- Cập nhật Description
UPDATE KpiAssignmentTables 
SET Description = REPLACE(Description, 'Kiểm soát và Quản lý rủi ro', 'Kế hoạch và Quản lý rủi ro')
WHERE Description LIKE '%Kiểm soát và Quản lý rủi ro%';

-- Cập nhật TableName
UPDATE KpiAssignmentTables 
SET TableName = REPLACE(TableName, 'Kiểm soát và Quản lý rủi ro', 'Kế hoạch và Quản lý rủi ro')
WHERE TableName LIKE '%Kiểm soát và Quản lý rủi ro%';

-- Kiểm tra kết quả sau khi update
SELECT Id, TableName, Description 
FROM KpiAssignmentTables 
WHERE TableName LIKE '%Kế hoạch và Quản lý rủi ro%' 
   OR Description LIKE '%Kế hoạch và Quản lý rủi ro%';

-- Kiểm tra xem còn bản ghi nào có "Kiểm soát" không
SELECT Id, TableName, Description 
FROM KpiAssignmentTables 
WHERE TableName LIKE '%Kiểm soát%' 
   OR Description LIKE '%Kiểm soát%';
