-- Update UnitKpiScorings table schema to match model
ALTER TABLE UnitKpiScorings ADD COLUMN UnitKhoanAssignmentId INTEGER;
ALTER TABLE UnitKpiScorings ADD COLUMN KhoanPeriodId INTEGER;
ALTER TABLE UnitKpiScorings ADD COLUMN ScoringDate TEXT DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE UnitKpiScorings ADD COLUMN BaseScore REAL DEFAULT 0;
ALTER TABLE UnitKpiScorings ADD COLUMN AdjustmentScore REAL DEFAULT 0;
ALTER TABLE UnitKpiScorings ADD COLUMN ScoredBy TEXT DEFAULT 'system';
ALTER TABLE UnitKpiScorings ADD COLUMN CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE UnitKpiScorings ADD COLUMN UpdatedAt TEXT;

-- Update existing records with default values
UPDATE UnitKpiScorings SET 
    UnitKhoanAssignmentId = 1,
    KhoanPeriodId = 1,
    BaseScore = TotalScore,
    AdjustmentScore = 0,
    ScoredBy = 'system',
    CreatedAt = CURRENT_TIMESTAMP
WHERE UnitKhoanAssignmentId IS NULL;

-- Add missing columns to other tables that might need them
-- Add missing columns to EmployeeKpiAssignments to match model
ALTER TABLE EmployeeKpiAssignments ADD COLUMN AssignmentDate TEXT DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE EmployeeKpiAssignments ADD COLUMN CompletionDate TEXT;
ALTER TABLE EmployeeKpiAssignments ADD COLUMN CompletionRate REAL DEFAULT 0;
ALTER TABLE EmployeeKpiAssignments ADD COLUMN Score REAL DEFAULT 0;
ALTER TABLE EmployeeKpiAssignments ADD COLUMN UpdatedAt TEXT;
ALTER TABLE EmployeeKpiAssignments ADD COLUMN CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP;

-- Update existing records
UPDATE EmployeeKpiAssignments SET 
    AssignmentDate = CURRENT_TIMESTAMP,
    CreatedAt = CURRENT_TIMESTAMP
WHERE AssignmentDate IS NULL;

SELECT 'UnitKpiScorings schema updated successfully!' as Status;
