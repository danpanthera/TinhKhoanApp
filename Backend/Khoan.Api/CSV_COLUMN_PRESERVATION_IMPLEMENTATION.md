# CSV Column Name Preservation Implementation

## Overview

This implementation ensures that when importing CSV files into the database, the system preserves the original CSV column names throughout the entire process - from import to storage to export.

## Key Components Implemented

### 1. Updated CsvColumnMappingConfig.cs

- **Purpose**: Configuration for validating CSV headers against expected column names
- **Change**: Modified to ensure Model properties use original CSV column names directly
- **Key Methods**:
  - `GetColumnMappingForCategory()`: Returns mapping where both key and value are the original CSV column names
  - `ValidateCsvHeaders()`: Validates that CSV headers match expected Model properties
  - `GetValidCsvColumnsForCategory()`: Returns set of valid CSV column names for a category

### 2. New History Models with Original CSV Column Names

- **LN01_CsvHistory**: For loan data with properties like `BRCD`, `CUSTSEQ`, `TAI_KHOAN`, etc.
- **Key Features**:
  - Properties named exactly as CSV columns (e.g., `BRCD` not `MaChiNhanh`)
  - Uses `[Column("OriginalName")]` attributes to map to database columns
  - Includes SCD Type 2 metadata fields (`BusinessKey`, `EffectiveDate`, etc.)
  - Stores complete raw data in `RawDataJson` field

### 3. RawDataProcessingService.cs

- **Purpose**: Service to transform imported CSV data into History models
- **Key Methods**:
  - `ProcessImportedDataToHistoryAsync()`: Main processing method
  - `ValidateImportedDataForCategoryAsync()`: Validates CSV structure before processing
- **Features**:
  - Preserves original CSV column names when mapping to Model properties
  - Generates business keys and data hashes for SCD Type 2 tracking
  - Handles data type conversion (string, decimal, datetime, int)
  - Comprehensive error handling and logging

### 4. Enhanced DataImportController.cs

- **New Endpoint**: `POST /api/DataImport/{id}/process`
- **Purpose**: Process imported CSV data into History tables
- **Features**:
  - Validates CSV headers against category requirements
  - Uses RawDataProcessingService for actual processing
  - Returns detailed processing results including batch ID and statistics
  - Preserves original column names throughout the process

### 5. Updated ApplicationDbContext.cs

- **Added DbSets**:
  - `LN01_History`: For CSV-preserving loan data
  - Support for other CSV-preserving models
- **Features**: Proper Entity Framework configuration for new models

## Data Flow

### 1. CSV Import (Existing)

```
CSV File → DataImportController.upload → ImportedDataItems (RawData with original column names)
```

### 2. Processing to History (New)

```
ImportedDataItems → RawDataProcessingService → History Models (original CSV column names)
```

### 3. Export (Future Enhancement)

```
History Models → Export Service → CSV with original column names
```

## Key Benefits

### 1. Preserved Data Integrity

- Original CSV column names are maintained throughout the system
- No data transformation or loss during import/export
- Maintains data lineage and traceability

### 2. Consistent Mapping

- Properties in Model classes match CSV headers exactly
- Database columns use original CSV names (via `[Column]` attributes)
- Eliminates confusion between source and target naming

### 3. Validation and Error Handling

- CSV headers are validated against expected Model properties
- Comprehensive error reporting for invalid data structures
- Detailed logging for troubleshooting

### 4. SCD Type 2 Support

- Full slowly changing dimension support with history tracking
- Business key generation for unique record identification
- Effective dating and versioning

## Usage Examples

### 1. Process LN01 CSV Data

```http
POST /api/DataImport/123/process
Content-Type: application/json

{
  "category": "LN01",
  "statementDate": "2024-12-31"
}
```

### 2. Validate CSV Structure

The service automatically validates that CSV headers match the expected Model properties for the category.

### 3. Query Processed Data

```csharp
var loanData = await _context.LN01_History
    .Where(x => x.IsCurrent && x.BRCD == "001")
    .ToListAsync();
```

## Database Schema

### LN01_CsvHistory Table

- **Table Name**: `LN01_CsvHistory`
- **Key Columns**:
  - SCD Type 2 fields: `BusinessKey`, `EffectiveDate`, `ExpiryDate`, `IsCurrent`, `RowVersion`
  - Metadata: `ImportId`, `StatementDate`, `ProcessedDate`, `DataHash`
  - Business Data: `BRCD`, `CUSTSEQ`, `CUSTNM`, `TAI_KHOAN`, `CCY`, `DU_NO`, etc.
  - Raw Data: `RawDataJson`, `AdditionalData`

## Migration

- **Migration Name**: `AddCsvPreservingHistoryModels`
- **Purpose**: Creates new tables for CSV-preserving History models
- **Status**: ✅ Created and ready to apply

## Testing Strategy

### 1. Unit Tests (Recommended)

- Test CsvColumnMappingConfig validation methods
- Test RawDataProcessingService data transformation
- Test error handling for invalid CSV structures

### 2. Integration Tests (Recommended)

- End-to-end CSV import → process → query workflow
- Test with actual CSV files from different categories
- Validate data integrity throughout the process

### 3. Manual Testing

- Upload CSV files via DataImportController
- Process data using the new endpoint
- Verify original column names are preserved
- Check History tables contain correct data

## Future Enhancements

### 1. Export Enhancement

- Update export functionality to use original CSV column names
- Ensure round-trip compatibility (import → export → import)

### 2. Additional Categories

- Add support for GAHR26, GLCB41, and other categories
- Extend RawDataProcessingService with category-specific processors

### 3. Performance Optimization

- Batch processing for large CSV files
- Async processing with status tracking
- Memory optimization for large datasets

### 4. Data Validation

- Business rule validation during processing
- Data quality checks and reporting
- Duplicate detection and handling

## Conclusion

This implementation successfully addresses the requirement to preserve original CSV column names throughout the import process. The system now:

1. ✅ Maintains original CSV column names in Model properties
2. ✅ Uses original CSV column names as database column names
3. ✅ Validates CSV headers against Model structure
4. ✅ Processes imported data while preserving column names
5. ✅ Provides comprehensive error handling and logging
6. ✅ Supports SCD Type 2 history tracking

The foundation is now in place for a robust CSV import system that maintains data integrity and original naming conventions.
