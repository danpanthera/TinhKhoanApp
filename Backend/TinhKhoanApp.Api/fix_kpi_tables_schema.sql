-- Cập nhật schema bảng KpiAssignmentTables
-- Backup dữ liệu cũ
CREATE TABLE KpiAssignmentTables_backup AS SELECT * FROM KpiAssignmentTables;

-- Xóa bảng cũ
DROP TABLE KpiAssignmentTables;

-- Tạo lại với schema đúng
CREATE TABLE "KpiAssignmentTables" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_KpiAssignmentTables" PRIMARY KEY AUTOINCREMENT,
    "TableType" INTEGER NOT NULL,
    "TableName" TEXT NOT NULL,
    "Description" TEXT,
    "Category" TEXT DEFAULT 'Dành cho cán bộ',
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "LastUpdated" TEXT
);

-- Tạo bảng KpiIndicators với schema đúng
CREATE TABLE IF NOT EXISTS "KpiIndicators" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_KpiIndicators" PRIMARY KEY AUTOINCREMENT,
    "TableId" INTEGER NOT NULL,
    "IndicatorName" TEXT NOT NULL,
    "Description" TEXT,
    "MaxScore" REAL NOT NULL,
    "Unit" TEXT,
    "OrderIndex" INTEGER NOT NULL DEFAULT 0,
    "ValueType" TEXT NOT NULL DEFAULT 'NUMBER',
    "IsActive" INTEGER NOT NULL DEFAULT 1,
    "CreatedDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT "FK_KpiIndicators_KpiAssignmentTables_TableId" FOREIGN KEY ("TableId") REFERENCES "KpiAssignmentTables" ("Id") ON DELETE CASCADE
);

-- Xóa backup
DROP TABLE KpiAssignmentTables_backup;

-- Kiểm tra kết quả
.schema KpiAssignmentTables
.schema KpiIndicators
