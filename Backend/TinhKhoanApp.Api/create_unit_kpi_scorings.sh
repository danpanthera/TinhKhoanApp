#!/bin/bash

# =============================================================================
# CREATE UNIT KPI SCORINGS - CHI NHÁNH KPI SCORING SYSTEM
# Tạo đánh giá KPI cho các đơn vị/chi nhánh
# =============================================================================

echo "🚀 BẮT ĐẦU TẠO UNIT KPI SCORINGS..."

# Database connection
SERVER="localhost,1433"
DATABASE="TinhKhoanDB"
USERNAME="sa"
PASSWORD="YourStrong@Password123"

# 1. Kiểm tra Units và structure
echo "📊 1. KIỂM TRA UNITS VÀ STRUCTURE..."

sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "
SELECT
    COUNT(*) as TotalUnits,
    COUNT(CASE WHEN Name LIKE '%Chi nhánh%' THEN 1 END) as ChiNhanhCount,
    COUNT(CASE WHEN Name LIKE '%Phòng%' THEN 1 END) as PhongCount
FROM Units;

-- Top units by hierarchy level
SELECT TOP 10
    Id,
    Name,
    UnitCode,
    ISNULL(ParentUnitId, 0) as ParentUnitId
FROM Units
ORDER BY ISNULL(ParentUnitId, 0), Id;
"

# 2. Tạo UnitKpiScorings cho major units
echo "🎯 2. TẠO UNIT KPI SCORINGS..."

sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "
-- Tạo UnitKpiScorings cho major units (chi nhánh và phòng chính)
INSERT INTO UnitKpiScorings (UnitId, KhoanPeriodId, Scores, CreatedAt, UpdatedAt)
SELECT
    u.Id as UnitId,
    kp.Id as KhoanPeriodId,
    -- JSON scores cho các chỉ tiêu chính
    JSON_QUERY('{
        \"DoanhThu\": {\"target\": 10000000, \"actual\": ' + CAST((8000000 + (u.Id * 100000)) as VARCHAR(20)) + ', \"percentage\": ' + CAST((80 + (u.Id % 20)) as VARCHAR(10)) + '},
        \"TinDung\": {\"target\": 50000000, \"actual\": ' + CAST((40000000 + (u.Id * 500000)) as VARCHAR(20)) + ', \"percentage\": ' + CAST((85 + (u.Id % 15)) as VARCHAR(10)) + '},
        \"TienGui\": {\"target\": 80000000, \"actual\": ' + CAST((70000000 + (u.Id * 800000)) as VARCHAR(20)) + ', \"percentage\": ' + CAST((90 + (u.Id % 10)) as VARCHAR(10)) + '},
        \"ChatLuong\": {\"target\": 95, \"actual\": ' + CAST((88 + (u.Id % 7)) as VARCHAR(10)) + ', \"percentage\": ' + CAST((90 + (u.Id % 10)) as VARCHAR(10)) + '}
    }') as Scores,
    GETDATE() as CreatedAt,
    GETDATE() as UpdatedAt
FROM Units u
CROSS JOIN (SELECT TOP 1 Id FROM KhoanPeriods WHERE YEAR(StartDate) = 2025 ORDER BY StartDate) kp
WHERE
    u.Name LIKE '%Chi nhánh%' OR
    u.Name LIKE '%Hội Sở%' OR
    u.Name IN ('Phòng Khách hàng Doanh nghiệp', 'Phòng Khách hàng Cá nhân', 'Phòng Kế toán & Ngân quỹ')
AND NOT EXISTS (
    SELECT 1 FROM UnitKpiScorings uks
    WHERE uks.UnitId = u.Id AND uks.KhoanPeriodId = kp.Id
);

-- Báo cáo kết quả
SELECT 'UnitKpiScorings created: ' + CAST(@@ROWCOUNT as VARCHAR(10)) as Result;
"

# 3. Verification và analytics
echo "✅ 3. VERIFICATION VÀ ANALYTICS..."

sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "
-- Thống kê UnitKpiScorings
SELECT
    COUNT(*) as TotalScorings,
    COUNT(DISTINCT UnitId) as UniqueUnits,
    COUNT(DISTINCT KhoanPeriodId) as UniquePeriods
FROM UnitKpiScorings;

-- Top performing units by estimated performance
SELECT TOP 5
    u.Name as UnitName,
    u.UnitCode,
    uks.Scores,
    uks.CreatedAt
FROM UnitKpiScorings uks
INNER JOIN Units u ON uks.UnitId = u.Id
ORDER BY uks.CreatedAt DESC;

-- Unit categories breakdown
SELECT
    CASE
        WHEN u.Name LIKE '%Chi nhánh%' THEN 'Chi nhánh'
        WHEN u.Name LIKE '%Hội Sở%' THEN 'Hội Sở'
        WHEN u.Name LIKE '%Phòng%' THEN 'Phòng ban'
        ELSE 'Khác'
    END as UnitType,
    COUNT(uks.Id) as ScoringCount
FROM UnitKpiScorings uks
INNER JOIN Units u ON uks.UnitId = u.Id
GROUP BY
    CASE
        WHEN u.Name LIKE '%Chi nhánh%' THEN 'Chi nhánh'
        WHEN u.Name LIKE '%Hội Sở%' THEN 'Hội Sở'
        WHEN u.Name LIKE '%Phòng%' THEN 'Phòng ban'
        ELSE 'Khác'
    END;
"

# 4. Test API endpoints
echo "🔗 4. TESTING API ENDPOINTS..."

echo "Total UnitKpiScorings via API:"
curl -s "http://localhost:5055/api/UnitKpiScoring" | jq 'length // "Error occurred"'

echo ""
echo "Sample scoring via API:"
curl -s "http://localhost:5055/api/UnitKpiScoring" | jq '.[0].scores // "No data"' 2>/dev/null

echo ""
echo "✅ HOÀN THÀNH UNIT KPI SCORING SYSTEM!"
echo "📊 Chi tiết: http://localhost:5055/api/UnitKpiScoring"
