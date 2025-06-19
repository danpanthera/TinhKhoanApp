-- ===================================================================
-- KIỂM TRA SỐ LƯỢNG CHỈ TIÊU (INDICATORS) TRONG CÁC BẢNG KPI
-- So sánh với số lượng chỉ tiêu canonical trong SeedKPIDefinitionMaxScore.cs
-- ===================================================================

-- Lấy số lượng chỉ tiêu expected cho mỗi vai trò (từ SeedKPIDefinitionMaxScore.cs)
-- Canonical 23 roleCodes và số lượng chỉ tiêu expected:
/*
1. TruongphongKhdn - 8 chỉ tiêu
2. TruongphongKhcn - 8 chỉ tiêu
3. PhophongKhdn - 8 chỉ tiêu
4. PhophongKhcn - 8 chỉ tiêu
5. TruongphongKhqlrr - 6 chỉ tiêu
6. PhophongKhqlrr - 6 chỉ tiêu
7. Cbtd - 8 chỉ tiêu
8. TruongphongKtnqCnl1 - 6 chỉ tiêu
9. PhophongKtnqCnl1 - 6 chỉ tiêu
10. Gdv - 6 chỉ tiêu

11. TqHkKtnb - Cần xác định từ code
12. TruongphongItThKtgs - Cần xác định từ code
13. CbItThKtgsKhqlrr - Cần xác định từ code
14. GiamdocPgd - 11 chỉ tiêu (từ code)
15. PhogiamdocPgd - Cần xác định từ code
16. PhogiamdocPgdCbtd - Cần xác định từ code
17. GiamdocCnl2 - 11 chỉ tiêu (từ code)
18. PhogiamdocCnl2Td - 8 chỉ tiêu (từ code)
19. PhogiamdocCnl2Kt - 6 chỉ tiêu (từ code)
20. TruongphongKhCnl2 - 9 chỉ tiêu (từ code)
21. PhophongKhCnl2 - 8 chỉ tiêu (từ code)
22. TruongphongKtnqCnl2 - 6 chỉ tiêu (từ code)
23. PhophongKtnqCnl2 - 5 chỉ tiêu (từ code)
*/

-- Bước 1: Lấy danh sách tất cả 23 bảng KPI assignment
SELECT 
    tableName,
    'Expected indicators count: TBD' as expected_count
FROM KpiAssignmentTables 
ORDER BY SortOrder, tableName;

-- Bước 2: Kiểm tra số lượng chỉ tiêu thực tế trong từng bảng
-- Chúng ta sẽ chạy từng query riêng cho mỗi bảng

-- TruongphongKhdn_KPI_Assignment (Expected: 8)
SELECT 'TruongphongKhdn_KPI_Assignment' as table_name, 8 as expected_count, COUNT(*) as actual_count,
    CASE WHEN COUNT(*) = 8 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM TruongphongKhdn_KPI_Assignment;

-- TruongphongKhcn_KPI_Assignment (Expected: 8)
SELECT 'TruongphongKhcn_KPI_Assignment' as table_name, 8 as expected_count, COUNT(*) as actual_count,
    CASE WHEN COUNT(*) = 8 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM TruongphongKhcn_KPI_Assignment;

-- PhophongKhdn_KPI_Assignment (Expected: 8)
SELECT 'PhophongKhdn_KPI_Assignment' as table_name, 8 as expected_count, COUNT(*) as actual_count,
    CASE WHEN COUNT(*) = 8 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM PhophongKhdn_KPI_Assignment;

-- PhophongKhcn_KPI_Assignment (Expected: 8)
SELECT 'PhophongKhcn_KPI_Assignment' as table_name, 8 as expected_count, COUNT(*) as actual_count,
    CASE WHEN COUNT(*) = 8 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM PhophongKhcn_KPI_Assignment;

-- TruongphongKhqlrr_KPI_Assignment (Expected: 6)
SELECT 'TruongphongKhqlrr_KPI_Assignment' as table_name, 6 as expected_count, COUNT(*) as actual_count,
    CASE WHEN COUNT(*) = 6 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM TruongphongKhqlrr_KPI_Assignment;

-- PhophongKhqlrr_KPI_Assignment (Expected: 6)
SELECT 'PhophongKhqlrr_KPI_Assignment' as table_name, 6 as expected_count, COUNT(*) as actual_count,
    CASE WHEN COUNT(*) = 6 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM PhophongKhqlrr_KPI_Assignment;

-- Cbtd_KPI_Assignment (Expected: 8)
SELECT 'Cbtd_KPI_Assignment' as table_name, 8 as expected_count, COUNT(*) as actual_count,
    CASE WHEN COUNT(*) = 8 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM Cbtd_KPI_Assignment;

-- TruongphongKtnqCnl1_KPI_Assignment (Expected: 6)
SELECT 'TruongphongKtnqCnl1_KPI_Assignment' as table_name, 6 as expected_count, COUNT(*) as actual_count,
    CASE WHEN COUNT(*) = 6 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM TruongphongKtnqCnl1_KPI_Assignment;

-- PhophongKtnqCnl1_KPI_Assignment (Expected: 6)
SELECT 'PhophongKtnqCnl1_KPI_Assignment' as table_name, 6 as expected_count, COUNT(*) as actual_count,
    CASE WHEN COUNT(*) = 6 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM PhophongKtnqCnl1_KPI_Assignment;

-- Gdv_KPI_Assignment (Expected: 6)
SELECT 'Gdv_KPI_Assignment' as table_name, 6 as expected_count, COUNT(*) as actual_count,
    CASE WHEN COUNT(*) = 6 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM Gdv_KPI_Assignment;

-- Các vai trò còn lại (11-23) - cần xác định expected count từ code:

-- TqHkKtnb_KPI_Assignment
SELECT 'TqHkKtnb_KPI_Assignment' as table_name, 'TBD' as expected_count, COUNT(*) as actual_count,
    'Need to check code' as status
FROM TqHkKtnb_KPI_Assignment;

-- TruongphongItThKtgs_KPI_Assignment
SELECT 'TruongphongItThKtgs_KPI_Assignment' as table_name, 'TBD' as expected_count, COUNT(*) as actual_count,
    'Need to check code' as status
FROM TruongphongItThKtgs_KPI_Assignment;

-- CbItThKtgsKhqlrr_KPI_Assignment
SELECT 'CbItThKtgsKhqlrr_KPI_Assignment' as table_name, 'TBD' as expected_count, COUNT(*) as actual_count,
    'Need to check code' as status
FROM CbItThKtgsKhqlrr_KPI_Assignment;

-- GiamdocPgd_KPI_Assignment (Expected: 11 từ code)
SELECT 'GiamdocPgd_KPI_Assignment' as table_name, 11 as expected_count, COUNT(*) as actual_count,
    CASE WHEN COUNT(*) = 11 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM GiamdocPgd_KPI_Assignment;

-- PhogiamdocPgd_KPI_Assignment
SELECT 'PhogiamdocPgd_KPI_Assignment' as table_name, 'TBD' as expected_count, COUNT(*) as actual_count,
    'Need to check code' as status
FROM PhogiamdocPgd_KPI_Assignment;

-- PhogiamdocPgdCbtd_KPI_Assignment
SELECT 'PhogiamdocPgdCbtd_KPI_Assignment' as table_name, 'TBD' as expected_count, COUNT(*) as actual_count,
    'Need to check code' as status
FROM PhogiamdocPgdCbtd_KPI_Assignment;

-- GiamdocCnl2_KPI_Assignment (Expected: 11 từ code)
SELECT 'GiamdocCnl2_KPI_Assignment' as table_name, 11 as expected_count, COUNT(*) as actual_count,
    CASE WHEN COUNT(*) = 11 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM GiamdocCnl2_KPI_Assignment;

-- PhogiamdocCnl2Td_KPI_Assignment (Expected: 8 từ code)
SELECT 'PhogiamdocCnl2Td_KPI_Assignment' as table_name, 8 as expected_count, COUNT(*) as actual_count,
    CASE WHEN COUNT(*) = 8 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM PhogiamdocCnl2Td_KPI_Assignment;

-- PhogiamdocCnl2Kt_KPI_Assignment (Expected: 6 từ code)
SELECT 'PhogiamdocCnl2Kt_KPI_Assignment' as table_name, 6 as expected_count, COUNT(*) as actual_count,
    CASE WHEN COUNT(*) = 6 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM PhogiamdocCnl2Kt_KPI_Assignment;

-- TruongphongKhCnl2_KPI_Assignment (Expected: 9 từ code)
SELECT 'TruongphongKhCnl2_KPI_Assignment' as table_name, 9 as expected_count, COUNT(*) as actual_count,
    CASE WHEN COUNT(*) = 9 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM TruongphongKhCnl2_KPI_Assignment;

-- PhophongKhCnl2_KPI_Assignment (Expected: 8 từ code)
SELECT 'PhophongKhCnl2_KPI_Assignment' as table_name, 8 as expected_count, COUNT(*) as actual_count,
    CASE WHEN COUNT(*) = 8 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM PhophongKhCnl2_KPI_Assignment;

-- TruongphongKtnqCnl2_KPI_Assignment (Expected: 6 từ code)
SELECT 'TruongphongKtnqCnl2_KPI_Assignment' as table_name, 6 as expected_count, COUNT(*) as actual_count,
    CASE WHEN COUNT(*) = 6 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM TruongphongKtnqCnl2_KPI_Assignment;

-- PhophongKtnqCnl2_KPI_Assignment (Expected: 5 từ code)
SELECT 'PhophongKtnqCnl2_KPI_Assignment' as table_name, 5 as expected_count, COUNT(*) as actual_count,
    CASE WHEN COUNT(*) = 5 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM PhophongKtnqCnl2_KPI_Assignment;

-- Bước 3: Kiểm tra các chỉ tiêu cụ thể trong bảng có thể có vấn đề
-- Ví dụ: Nếu thấy một bảng có số lượng chỉ tiêu không đúng, kiểm tra chi tiết:

-- SELECT KpiCode, KpiName, MaxScore, ValueType, UnitOfMeasure 
-- FROM [TableName]_KPI_Assignment 
-- ORDER BY KpiCode;
