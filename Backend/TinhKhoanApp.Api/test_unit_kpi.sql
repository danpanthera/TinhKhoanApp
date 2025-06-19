-- Create simplified UnitKpiScoring test endpoint by using direct SQL
-- Test basic UnitKpiScorings data without complex joins

SELECT 
    Id,
    UnitId,
    ScoringPeriod,
    TotalScore,
    MaxPossibleScore,
    CompletionRate,
    Grade,
    Notes,
    CreatedDate,
    IsActive
FROM UnitKpiScorings
WHERE IsActive = 1
ORDER BY CreatedDate DESC;
