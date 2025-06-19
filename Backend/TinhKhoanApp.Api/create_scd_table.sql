-- Create SCD Type 2 table for Raw Data Records
-- This table stores historical versions of raw data with slowly changing dimension logic

CREATE TABLE IF NOT EXISTS RawDataRecords_SCD (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    
    -- SCD Type 2 core fields
    SourceId TEXT NOT NULL,                    -- Business key (identifies logical record)
    ValidFrom DATETIME NOT NULL,               -- When this version became valid
    ValidTo DATETIME NULL,                     -- When this version became invalid (NULL for current)
    IsCurrent INTEGER NOT NULL DEFAULT 1,     -- Is this the current version? (1=yes, 0=no)
    VersionNumber INTEGER NOT NULL DEFAULT 1, -- Version number for this record
    RecordHash TEXT NOT NULL,                  -- Hash of record content for change detection
    CreatedAt DATETIME NOT NULL,               -- When this record was created in the system
    DataSource TEXT NOT NULL,                  -- Source of the data (e.g., file name)
    
    -- Business data fields
    RawDataImportId INTEGER NOT NULL,          -- Reference to original import
    JsonData TEXT NOT NULL,                    -- The actual data stored as JSON
    DataType TEXT NOT NULL,                    -- Data type (LN01, GL01, etc.)
    BranchCode TEXT NOT NULL,                  -- Branch code extracted from filename
    StatementDate DATETIME NOT NULL,           -- Statement date from filename
    OriginalFileName TEXT NOT NULL,            -- Original filename
    ImportDate DATETIME NOT NULL,              -- When data was imported
    ProcessingStatus TEXT NOT NULL DEFAULT 'Pending', -- Processing status
    ErrorMessage TEXT NULL,                    -- Error message if any
    ProcessingNotes TEXT NULL,                 -- Processing notes
    
    -- Foreign key constraint
    FOREIGN KEY (RawDataImportId) REFERENCES RawDataImports(Id)
);

-- Create indexes for performance
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_SourceId ON RawDataRecords_SCD(SourceId);
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_IsCurrent ON RawDataRecords_SCD(IsCurrent);
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_ValidFrom ON RawDataRecords_SCD(ValidFrom);
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_ValidTo ON RawDataRecords_SCD(ValidTo);
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_DataType ON RawDataRecords_SCD(DataType);
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_BranchCode ON RawDataRecords_SCD(BranchCode);
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_StatementDate ON RawDataRecords_SCD(StatementDate);
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_ImportDate ON RawDataRecords_SCD(ImportDate);

-- Create compound index for common queries
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_Current_Lookup ON RawDataRecords_SCD(IsCurrent, DataType, BranchCode, StatementDate);
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_History_Lookup ON RawDataRecords_SCD(SourceId, VersionNumber);
