-- Migration: Add IsDeleted column to RawDataRecords_SCD table
-- Date: 2025-06-17
-- Purpose: Support SCD Type 2 logic for handling deleted records

ALTER TABLE RawDataRecords_SCD 
ADD COLUMN IsDeleted BOOLEAN NOT NULL DEFAULT 0;

-- Create index for better performance on queries filtering by IsDeleted
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_IsDeleted 
ON RawDataRecords_SCD (IsDeleted);

-- Create composite index for deleted records queries
CREATE INDEX IF NOT EXISTS IX_RawDataRecords_SCD_DataSource_IsDeleted_IsCurrent 
ON RawDataRecords_SCD (DataSource, IsDeleted, IsCurrent);
