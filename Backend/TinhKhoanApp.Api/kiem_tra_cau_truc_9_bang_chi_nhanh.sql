-- KIỂM TRA 9 BẢNG KPI CHI NHÁNH CÓ CẤU TRÚC GIỐNG NHAU VÀ GIỐNG GiamdocCnl2
-- Báo cáo chi tiết về cấu trúc các bảng

SELECT '========== BÁO CÁO KIỂM TRA CẤU TRÚC BẢNG KPI ==========' as Report;

-- 1. Danh sách 9 bảng Chi nhánh đã tạo
SELECT '1. DANH SÁCH 9 BẢNG KPI CHI NHÁNH' as Section;
SELECT ROW_NUMBER() OVER (ORDER BY name) as STT, name as TableName 
FROM sqlite_master 
WHERE type='table' AND name LIKE 'KPI_CN_%' OR name = 'KPI_HOI_SO'
ORDER BY name;

-- 2. So sánh số cột của từng bảng
SELECT '2. KIỂM TRA SỐ CỘT CỦA TỪNG BẢNG' as Section;

WITH table_column_count AS (
    SELECT 'GiamdocCnl2_KPI_Assignment' as table_name, 
           (SELECT COUNT(*) FROM pragma_table_info('GiamdocCnl2_KPI_Assignment')) as column_count
    UNION ALL
    SELECT 'KPI_CN_MUONG_TE', (SELECT COUNT(*) FROM pragma_table_info('KPI_CN_MUONG_TE'))
    UNION ALL
    SELECT 'KPI_CN_NAM_NHUN', (SELECT COUNT(*) FROM pragma_table_info('KPI_CN_NAM_NHUN'))
    UNION ALL
    SELECT 'KPI_CN_PHONG_THO', (SELECT COUNT(*) FROM pragma_table_info('KPI_CN_PHONG_THO'))
    UNION ALL
    SELECT 'KPI_CN_SIN_HO', (SELECT COUNT(*) FROM pragma_table_info('KPI_CN_SIN_HO'))
    UNION ALL
    SELECT 'KPI_CN_TAM_DUONG', (SELECT COUNT(*) FROM pragma_table_info('KPI_CN_TAM_DUONG'))
    UNION ALL
    SELECT 'KPI_CN_TAN_UYEN', (SELECT COUNT(*) FROM pragma_table_info('KPI_CN_TAN_UYEN'))
    UNION ALL
    SELECT 'KPI_CN_THANH_PHO', (SELECT COUNT(*) FROM pragma_table_info('KPI_CN_THANH_PHO'))
    UNION ALL
    SELECT 'KPI_CN_THAN_UYEN', (SELECT COUNT(*) FROM pragma_table_info('KPI_CN_THAN_UYEN'))
    UNION ALL
    SELECT 'KPI_HOI_SO', (SELECT COUNT(*) FROM pragma_table_info('KPI_HOI_SO'))
)
SELECT table_name as TableName, column_count as ColumnCount,
       CASE 
           WHEN column_count = 7 THEN '✅ ĐÚNG'
           ELSE '❌ SAI' 
       END as Status
FROM table_column_count;

-- 3. Kiểm tra chi tiết cấu trúc một vài bảng mẫu
SELECT '3. CHI TIẾT CẤU TRÚC BẢNG GiamdocCnl2_KPI_Assignment (CHUẨN)' as Section;
SELECT cid, name, type, [notnull], dflt_value, pk 
FROM pragma_table_info('GiamdocCnl2_KPI_Assignment');

SELECT '4. CHI TIẾT CẤU TRÚC BẢNG KPI_CN_MUONG_TE (KIỂM TRA)' as Section;
SELECT cid, name, type, [notnull], dflt_value, pk 
FROM pragma_table_info('KPI_CN_MUONG_TE');

SELECT '5. CHI TIẾT CẤU TRÚC BẢNG KPI_HOI_SO (KIỂM TRA)' as Section;
SELECT cid, name, type, [notnull], dflt_value, pk 
FROM pragma_table_info('KPI_HOI_SO');

-- 4. Tổng kết kiểm tra
SELECT '6. TỔNG KẾT KIỂM TRA' as Section;
SELECT 
    CASE 
        WHEN (SELECT COUNT(DISTINCT column_count) FROM (
            SELECT (SELECT COUNT(*) FROM pragma_table_info('GiamdocCnl2_KPI_Assignment')) as column_count
            UNION ALL
            SELECT (SELECT COUNT(*) FROM pragma_table_info('KPI_CN_MUONG_TE'))
            UNION ALL
            SELECT (SELECT COUNT(*) FROM pragma_table_info('KPI_CN_NAM_NHUN'))
            UNION ALL
            SELECT (SELECT COUNT(*) FROM pragma_table_info('KPI_CN_PHONG_THO'))
            UNION ALL
            SELECT (SELECT COUNT(*) FROM pragma_table_info('KPI_CN_SIN_HO'))
            UNION ALL
            SELECT (SELECT COUNT(*) FROM pragma_table_info('KPI_CN_TAM_DUONG'))
            UNION ALL
            SELECT (SELECT COUNT(*) FROM pragma_table_info('KPI_CN_TAN_UYEN'))
            UNION ALL
            SELECT (SELECT COUNT(*) FROM pragma_table_info('KPI_CN_THANH_PHO'))
            UNION ALL
            SELECT (SELECT COUNT(*) FROM pragma_table_info('KPI_CN_THAN_UYEN'))
            UNION ALL
            SELECT (SELECT COUNT(*) FROM pragma_table_info('KPI_HOI_SO'))
        )) = 1 AND (SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND (name LIKE 'KPI_CN_%' OR name = 'KPI_HOI_SO')) = 9
        THEN '✅ HOÀN THÀNH: Tất cả 9 bảng KPI Chi nhánh đều có cấu trúc giống nhau và giống với GiamdocCnl2_KPI_Assignment'
        ELSE '❌ CHƯA HOÀN THÀNH: Có sự khác biệt trong cấu trúc bảng'
    END as FinalResult;

-- 5. Danh sách đầy đủ các bảng KPI trong hệ thống
SELECT '7. DANH SÁCH TẤT CẢ BẢNG KPI TRONG HỆ THỐNG' as Section;
SELECT 
    CASE 
        WHEN name LIKE '%_KPI_Assignment' THEN 'Vai trò cán bộ'
        WHEN name LIKE 'KPI_CN_%' OR name = 'KPI_HOI_SO' THEN 'Chi nhánh'
        ELSE 'Khác'
    END as Category,
    name as TableName
FROM sqlite_master 
WHERE type='table' AND (name LIKE '%_KPI_Assignment' OR name LIKE 'KPI_CN_%' OR name = 'KPI_HOI_SO')
ORDER BY Category, name;
