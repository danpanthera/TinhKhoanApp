-- Tạo lại bảng UnitKpiScorings với schema đúng
DROP TABLE IF EXISTS UnitKpiScorings_backup;
CREATE TABLE UnitKpiScorings_backup AS SELECT * FROM UnitKpiScorings;

DROP TABLE UnitKpiScorings;

CREATE TABLE "UnitKpiScorings" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_UnitKpiScorings" PRIMARY KEY AUTOINCREMENT,
    "UnitKhoanAssignmentId" INTEGER NOT NULL,
    "KhoanPeriodId" INTEGER NOT NULL,
    "UnitId" INTEGER NOT NULL,
    "ScoringDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "TotalScore" REAL NOT NULL DEFAULT 0,
    "BaseScore" REAL NOT NULL DEFAULT 0,
    "AdjustmentScore" REAL NOT NULL DEFAULT 0,
    "Notes" TEXT,
    "ScoredBy" TEXT DEFAULT 'system',
    "CreatedAt" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "UpdatedAt" TEXT,
    CONSTRAINT "FK_UnitKpiScorings_Units_UnitId" FOREIGN KEY ("UnitId") REFERENCES "Units" ("Id"),
    CONSTRAINT "FK_UnitKpiScorings_KhoanPeriods_KhoanPeriodId" FOREIGN KEY ("KhoanPeriodId") REFERENCES "KhoanPeriods" ("Id")
);

-- Thêm dữ liệu test mới
INSERT INTO UnitKpiScorings (UnitKhoanAssignmentId, KhoanPeriodId, UnitId, TotalScore, BaseScore, AdjustmentScore, Notes, ScoredBy, CreatedAt, UpdatedAt) 
VALUES 
(1, 3, 2, 85.5, 85.5, 0.0, 'Chấm điểm tháng 6/2025', 'system', '2025-06-15 12:13:11', '2025-06-15 12:13:11'),
(1, 3, 3, 78.2, 78.2, 0.0, 'Chấm điểm tháng 6/2025', 'system', '2025-06-15 12:13:11', '2025-06-15 12:13:11'),
(1, 5, 2, 92.0, 90.0, 2.0, 'Chấm điểm quý II/2025 với điểm cộng', 'admin', '2025-06-15 12:15:00', '2025-06-15 12:15:00'),
(1, 5, 3, 87.5, 85.0, 2.5, 'Chấm điểm quý II/2025 với điểm cộng', 'admin', '2025-06-15 12:15:00', '2025-06-15 12:15:00');

-- Xóa bảng backup
DROP TABLE UnitKpiScorings_backup;

-- Kiểm tra dữ liệu
SELECT 
    Id, UnitId, KhoanPeriodId, TotalScore, BaseScore, AdjustmentScore, Notes, ScoredBy, CreatedAt
FROM UnitKpiScorings;
