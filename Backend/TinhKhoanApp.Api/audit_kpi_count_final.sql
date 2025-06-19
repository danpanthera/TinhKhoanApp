-- ===================================================================
-- KIỂM TRA SỐ LƯỢNG CHỈ TIÊU THỰC TẾ TRONG CÁC BẢNG KPI
-- So sánh với số lượng expected từ SeedKPIDefinitionMaxScore.cs
-- ===================================================================

-- 1. TruongphongKhdn (Expected: 8)
SELECT 'TruongphongKhdn_KPI_Assignment' as table_name, 8 as expected, COUNT(*) as actual,
       CASE WHEN COUNT(*) = 8 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM TruongphongKhdn_KPI_Assignment;

-- 2. TruongphongKhcn (Expected: 8)
SELECT 'TruongphongKhcn_KPI_Assignment' as table_name, 8 as expected, COUNT(*) as actual,
       CASE WHEN COUNT(*) = 8 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM TruongphongKhcn_KPI_Assignment;

-- 3. PhophongKhdn (Expected: 8)
SELECT 'PhophongKhdn_KPI_Assignment' as table_name, 8 as expected, COUNT(*) as actual,
       CASE WHEN COUNT(*) = 8 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM PhophongKhdn_KPI_Assignment;

-- 4. PhophongKhcn (Expected: 8)
SELECT 'PhophongKhcn_KPI_Assignment' as table_name, 8 as expected, COUNT(*) as actual,
       CASE WHEN COUNT(*) = 8 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM PhophongKhcn_KPI_Assignment;

-- 5. TruongphongKhqlrr (Expected: 6)
SELECT 'TruongphongKhqlrr_KPI_Assignment' as table_name, 6 as expected, COUNT(*) as actual,
       CASE WHEN COUNT(*) = 6 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM TruongphongKhqlrr_KPI_Assignment;

-- 6. PhophongKhqlrr (Expected: 6)
SELECT 'PhophongKhqlrr_KPI_Assignment' as table_name, 6 as expected, COUNT(*) as actual,
       CASE WHEN COUNT(*) = 6 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM PhophongKhqlrr_KPI_Assignment;

-- 7. Cbtd (Expected: 8)
SELECT 'Cbtd_KPI_Assignment' as table_name, 8 as expected, COUNT(*) as actual,
       CASE WHEN COUNT(*) = 8 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM Cbtd_KPI_Assignment;

-- 8. TruongphongKtnqCnl1 (Expected: 6)
SELECT 'TruongphongKtnqCnl1_KPI_Assignment' as table_name, 6 as expected, COUNT(*) as actual,
       CASE WHEN COUNT(*) = 6 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM TruongphongKtnqCnl1_KPI_Assignment;

-- 9. PhophongKtnqCnl1 (Expected: 6)
SELECT 'PhophongKtnqCnl1_KPI_Assignment' as table_name, 6 as expected, COUNT(*) as actual,
       CASE WHEN COUNT(*) = 6 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM PhophongKtnqCnl1_KPI_Assignment;

-- 10. Gdv (Expected: 6)
SELECT 'Gdv_KPI_Assignment' as table_name, 6 as expected, COUNT(*) as actual,
       CASE WHEN COUNT(*) = 6 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM Gdv_KPI_Assignment;

-- 11. TqHkKtnb (Cần xác định từ code - tạm thời dùng -1)
SELECT 'TqHkKtnb_KPI_Assignment' as table_name, -1 as expected, COUNT(*) as actual,
       'NEED_CHECK' as status
FROM TqHkKtnb_KPI_Assignment;

-- 12. TruongphongItThKtgs (Cần xác định từ code)
SELECT 'TruongphongItThKtgs_KPI_Assignment' as table_name, -1 as expected, COUNT(*) as actual,
       'NEED_CHECK' as status
FROM TruongphongItThKtgs_KPI_Assignment;

-- 13. CbItThKtgsKhqlrr (Cần xác định từ code)
SELECT 'CbItThKtgsKhqlrr_KPI_Assignment' as table_name, -1 as expected, COUNT(*) as actual,
       'NEED_CHECK' as status
FROM CbItThKtgsKhqlrr_KPI_Assignment;

-- 14. GiamdocPgd (Expected: 11)
SELECT 'GiamdocPgd_KPI_Assignment' as table_name, 11 as expected, COUNT(*) as actual,
       CASE WHEN COUNT(*) = 11 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM GiamdocPgd_KPI_Assignment;

-- 15. PhogiamdocPgd (Cần xác định từ code)
SELECT 'PhogiamdocPgd_KPI_Assignment' as table_name, -1 as expected, COUNT(*) as actual,
       'NEED_CHECK' as status
FROM PhogiamdocPgd_KPI_Assignment;

-- 16. PhogiamdocPgdCbtd (Cần xác định từ code)
SELECT 'PhogiamdocPgdCbtd_KPI_Assignment' as table_name, -1 as expected, COUNT(*) as actual,
       'NEED_CHECK' as status
FROM PhogiamdocPgdCbtd_KPI_Assignment;

-- 17. GiamdocCnl2 (Expected: 11)
SELECT 'GiamdocCnl2_KPI_Assignment' as table_name, 11 as expected, COUNT(*) as actual,
       CASE WHEN COUNT(*) = 11 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM GiamdocCnl2_KPI_Assignment;

-- 18. PhogiamdocCnl2Td (Expected: 8)
SELECT 'PhogiamdocCnl2Td_KPI_Assignment' as table_name, 8 as expected, COUNT(*) as actual,
       CASE WHEN COUNT(*) = 8 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM PhogiamdocCnl2Td_KPI_Assignment;

-- 19. PhogiamdocCnl2Kt (Expected: 6)
SELECT 'PhogiamdocCnl2Kt_KPI_Assignment' as table_name, 6 as expected, COUNT(*) as actual,
       CASE WHEN COUNT(*) = 6 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM PhogiamdocCnl2Kt_KPI_Assignment;

-- 20. TruongphongKhCnl2 (Expected: 9)
SELECT 'TruongphongKhCnl2_KPI_Assignment' as table_name, 9 as expected, COUNT(*) as actual,
       CASE WHEN COUNT(*) = 9 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM TruongphongKhCnl2_KPI_Assignment;

-- 21. PhophongKhCnl2 (Expected: 8)
SELECT 'PhophongKhCnl2_KPI_Assignment' as table_name, 8 as expected, COUNT(*) as actual,
       CASE WHEN COUNT(*) = 8 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM PhophongKhCnl2_KPI_Assignment;

-- 22. TruongphongKtnqCnl2 (Expected: 6)
SELECT 'TruongphongKtnqCnl2_KPI_Assignment' as table_name, 6 as expected, COUNT(*) as actual,
       CASE WHEN COUNT(*) = 6 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM TruongphongKtnqCnl2_KPI_Assignment;

-- 23. PhophongKtnqCnl2 (Expected: 5)
SELECT 'PhophongKtnqCnl2_KPI_Assignment' as table_name, 5 as expected, COUNT(*) as actual,
       CASE WHEN COUNT(*) = 5 THEN '✅ OK' ELSE '❌ MISMATCH' END as status
FROM PhophongKtnqCnl2_KPI_Assignment;
