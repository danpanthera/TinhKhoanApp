#!/bin/bash

# =============================================================================
# CREATE UNIT KPI SCORINGS - CORRECTED SCHEMA VERSION
# Tạo đánh giá KPI cho các đơn vị với đúng column structure
# =============================================================================

echo "🚀 BẮT ĐẦU TẠO UNIT KPI SCORINGS (CORRECTED)..."

# Database connection
SERVER="localhost,1433"
DATABASE="TinhKhoanDB"
USERNAME="sa"
PASSWORD="YourStrong@Password123"

# 1. Kiểm tra Units structure
echo "📊 1. KIỂM TRA UNITS STRUCTURE..."

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
    Code,
    ISNULL(ParentUnitId, 0) as ParentUnitId
FROM Units
ORDER BY ISNULL(ParentUnitId, 0), Id;
"

# 2. Clear existing và tạo UnitKpiScorings mới
echo "🧹 2. CLEARING EXISTING UNIT SCORINGS..."
sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "DELETE FROM UnitKpiScorings;"

echo "🎯 3. TẠO UNIT KPI SCORINGS..."

sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "
-- Tạo UnitKpiScorings với correct columns
INSERT INTO UnitKpiScorings (UnitId, KhoanPeriodId, TotalScore, MaxScore, Percentage, Ranking, Status, CreatedAt, UpdatedAt)
SELECT
    u.Id as UnitId,
    kp.Id as KhoanPeriodId,
    -- TotalScore based on unit performance simulation
    CASE
        WHEN u.Name LIKE '%Hội Sở%' THEN 950.0
        WHEN u.Name LIKE '%Chi nhánh%' THEN (800.0 + (u.Id * 10.0))
        WHEN u.Name LIKE '%Phòng%' THEN (700.0 + (u.Id * 15.0))
        ELSE 600.0
    END as TotalScore,
    1000.0 as MaxScore,  -- Standard max score
    -- Percentage calculation
    CASE
        WHEN u.Name LIKE '%Hội Sở%' THEN 95.0
        WHEN u.Name LIKE '%Chi nhánh%' THEN (80.0 + (u.Id % 15))
        WHEN u.Name LIKE '%Phòng%' THEN (70.0 + (u.Id % 25))
        ELSE 60.0
    END as Percentage,
    -- Ranking simulation
    ROW_NUMBER() OVER (ORDER BY u.Id) as Ranking,
    'Active' as Status,
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
echo "✅ 4. VERIFICATION VÀ ANALYTICS..."

sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -Q "
-- Thống kê UnitKpiScorings
SELECT
    COUNT(*) as TotalScorings,
    COUNT(DISTINCT UnitId) as UniqueUnits,
    COUNT(DISTINCT KhoanPeriodId) as UniquePeriods,
    AVG(TotalScore) as AvgTotalScore,
    AVG(Percentage) as AvgPercentage
FROM UnitKpiScorings;

-- Top 5 performing units
SELECT TOP 5
    u.Name as UnitName,
    u.Code,
    uks.TotalScore,
    uks.Percentage,
    uks.Ranking,
    uks.Status
FROM UnitKpiScorings uks
INNER JOIN Units u ON uks.UnitId = u.Id
ORDER BY uks.TotalScore DESC;

-- Unit categories performance
SELECT
    CASE
        WHEN u.Name LIKE '%Chi nhánh%' THEN 'Chi nhánh'
        WHEN u.Name LIKE '%Hội Sở%' THEN 'Hội Sở'
        WHEN u.Name LIKE '%Phòng%' THEN 'Phòng ban'
        ELSE 'Khác'
    END as UnitType,
    COUNT(uks.Id) as ScoringCount,
    AVG(uks.TotalScore) as AvgScore,
    AVG(uks.Percentage) as AvgPercentage
FROM UnitKpiScorings uks
INNER JOIN Units u ON uks.UnitId = u.Id
GROUP BY
    CASE
        WHEN u.Name LIKE '%Chi nhánh%' THEN 'Chi nhánh'
        WHEN u.Name LIKE '%Hội Sở%' THEN 'Hội Sở'
        WHEN u.Name LIKE '%Phòng%' THEN 'Phòng ban'
        ELSE 'Khác'
    END
ORDER BY AVG(uks.TotalScore) DESC;
"

# 4. Test API endpoints
echo "🔗 5. TESTING API ENDPOINTS..."

echo "Total UnitKpiScorings via API:"
curl -s "http://localhost:5055/api/UnitKpiScoring" | jq 'length // "Error occurred"'

echo ""
echo "Sample scoring via API:"
curl -s "http://localhost:5055/api/UnitKpiScoring" | jq '.[0] // "No data"' 2>/dev/null

echo ""
echo "✅ HOÀN THÀNH UNIT KPI SCORING SYSTEM!"
echo "📊 Chi tiết: http://localhost:5055/api/UnitKpiScoring"
echo "📈 Total scorings: $(sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -C -h -1 -Q "SELECT COUNT(*) FROM UnitKpiScorings")"
