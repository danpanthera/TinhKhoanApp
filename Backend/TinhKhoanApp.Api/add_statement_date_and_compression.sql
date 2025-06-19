-- Add StatementDate and compression fields to ImportedDataRecords table
ALTER TABLE ImportedDataRecords ADD COLUMN StatementDate TEXT;
ALTER TABLE ImportedDataRecords ADD COLUMN CompressedData BLOB;
ALTER TABLE ImportedDataRecords ADD COLUMN CompressionRatio REAL DEFAULT 0.0;

-- Update existing records to have compression ratio 0
UPDATE ImportedDataRecords SET CompressionRatio = 0.0 WHERE CompressionRatio IS NULL;
