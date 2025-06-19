-- So sánh kết quả với yêu cầu ban đầu

SELECT 'SO SÁNH VỚI YÊU CẦU BAN ĐẦU:' as info;

-- Bảng so sánh TableType Code với tên thực tế
WITH RequiredRoles AS (
    SELECT 1 as Id, 'TruongphongKhdn' as TableTypeCode, 'Trưởng phòng KHDN' as RequiredName
    UNION ALL SELECT 2, 'TruongphongKhcn', 'Trưởng phòng KHCN'
    UNION ALL SELECT 3, 'PhophongKhdn', 'Phó phòng KHDN'
    UNION ALL SELECT 4, 'PhophongKhcn', 'Phó phòng KHCN'
    UNION ALL SELECT 5, 'TruongphongKhqlrr', 'Trưởng phòng KH&QLRR'
    UNION ALL SELECT 6, 'PhophongKhqlrr', 'Phó phòng KH&QLRR'
    UNION ALL SELECT 7, 'Cbtd', 'Cán bộ tín dụng'
    UNION ALL SELECT 8, 'TruongphongKtnqCnl1', 'Trưởng phòng KTNV CNL1'
    UNION ALL SELECT 9, 'PhophongKtnqCnl1', 'Phó phòng KTNV CNL1'
    UNION ALL SELECT 10, 'Gdv', 'GDV'
    UNION ALL SELECT 11, 'TqHkKtnb', 'TQ/HK/KTNB'
    UNION ALL SELECT 12, 'TruongphoItThKtgs', 'Trưởng phòng IT/TH/KTGS'
    UNION ALL SELECT 13, 'CBItThKtgsKhqlrr', 'CB IT/TH/KTGS/KHQLRR'
    UNION ALL SELECT 14, 'GiamdocPgd', 'Giám đốc Phòng giao dịch'
    UNION ALL SELECT 15, 'PhogiamdocPgd', 'Phó giám đốc Phòng giao dịch'
    UNION ALL SELECT 16, 'PhogiamdocPgdCbtd', 'Phó giám đốc Phòng giao dịch kiêm CBTD'
    UNION ALL SELECT 17, 'GiamdocCnl2', 'Giám đốc CNL2'
    UNION ALL SELECT 18, 'PhogiamdocCnl2Td', 'Phó giám đốc CNL2 phụ trách Tín dụng'
    UNION ALL SELECT 19, 'PhogiamdocCnl2Kt', 'Phó giám đốc CNL2 phụ trách Kinh tế'
    UNION ALL SELECT 20, 'TruongphongKhCnl2', 'Trưởng phòng KH CNL2'
    UNION ALL SELECT 21, 'PhophongKhCnl2', 'Phó phòng KH CNL2'
    UNION ALL SELECT 22, 'TruongphongKtnqCnl2', 'Trưởng phòng KTNV CNL2'
    UNION ALL SELECT 23, 'PhophongKtnqCnl2', 'Phó phòng KTNV CNL2'
)
SELECT 
    req.Id as 'ID',
    req.TableTypeCode as 'Mã TableType',
    req.RequiredName as 'Tên yêu cầu',
    r.Name as 'Tên hiện tại',
    CASE 
        WHEN req.RequiredName = r.Name THEN '✅ Khớp'
        ELSE '❌ Khác biệt'
    END as 'Trạng thái'
FROM RequiredRoles req
LEFT JOIN Roles r ON req.Id = r.Id
ORDER BY req.Id;

-- Kiểm tra tương ứng với KpiAssignmentTables
SELECT 'KIỂM TRA TƯƠNG ỨNG VỚI KPI ASSIGNMENT TABLES:' as info;
SELECT 
    r.Id as 'Role ID',
    r.Name as 'Role Name',
    k.Id as 'KPI Table ID',
    k.TableName as 'KPI Table Name',
    k.TableType as 'TableType',
    CASE 
        WHEN r.Id = k.Id THEN '✅ ID Match'
        ELSE '❌ ID Mismatch'
    END as 'Status'
FROM Roles r
LEFT JOIN KpiAssignmentTables k ON r.Id = k.Id AND k.Category = 'Dành cho Cán bộ'
ORDER BY r.Id;
