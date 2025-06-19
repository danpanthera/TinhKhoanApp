-- Recreate ImportedDataRecords and ImportedDataItems tables with correct schema

CREATE TABLE "ImportedDataRecords" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ImportedDataRecords" PRIMARY KEY AUTOINCREMENT,
    "FileName" TEXT NOT NULL,
    "FileType" TEXT NOT NULL,
    "Category" TEXT NOT NULL,
    "ImportDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "ImportedBy" TEXT NOT NULL,
    "Status" TEXT NOT NULL DEFAULT 'Pending',
    "RecordsCount" INTEGER NOT NULL DEFAULT 0,
    "OriginalFileData" BLOB,
    "Notes" TEXT
);

CREATE TABLE "ImportedDataItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ImportedDataItems" PRIMARY KEY AUTOINCREMENT,
    "ImportedDataRecordId" INTEGER NOT NULL,
    "RawData" TEXT NOT NULL,
    "ProcessedDate" TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "ProcessingNotes" TEXT,
    CONSTRAINT "FK_ImportedDataItems_ImportedDataRecords_ImportedDataRecordId" 
        FOREIGN KEY ("ImportedDataRecordId") REFERENCES "ImportedDataRecords" ("Id")
);

SELECT 'Tables recreated successfully' as result;
