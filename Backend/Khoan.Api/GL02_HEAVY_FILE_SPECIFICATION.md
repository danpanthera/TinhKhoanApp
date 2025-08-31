# GL02 - Heavy File Optimization Specification

## 🎯 Overview
GL02 - General Ledger Transactions table với cấu trúc tối ưu cho heavy file imports (~200MB CSV files). 

**Key Features:**
- ✅ Support file size up to 2GB  
- ✅ Batch processing 10,000 records per batch
- ✅ Upload timeout 15 minutes
- ✅ Partitioned Columnstore (NOT TEMPORAL)
- ✅ Direct import (no column transformation)
- ✅ Import policy: Only files containing "gl02"

## 📊 Database Structure

### Total Columns: 21 (NGAY_DL + 17 Business + 4 System)

#### Position 0: NGAY_DL
- **Source**: TRDATE column from CSV
- **Type**: datetime2
- **Format**: dd/MM/yyyy  
- **Purpose**: Date partitioning key

#### Positions 1-17: Business Columns (CSV Exact Order)
```
1.  TRBRCD      nvarchar(200)  - Transaction Branch Code  
2.  USERID      nvarchar(200)  - User ID
3.  JOURSEQ     nvarchar(200)  - Journal Sequence
4.  DYTRSEQ     nvarchar(200)  - Daily Transaction Sequence  
5.  LOCAC       nvarchar(200)  - Local Account
6.  CCY         nvarchar(200)  - Currency
7.  BUSCD       nvarchar(200)  - Business Code
8.  UNIT        nvarchar(200)  - Unit
9.  TRCD        nvarchar(200)  - Transaction Code
10. CUSTOMER    nvarchar(200)  - Customer
11. TRTP        nvarchar(200)  - Transaction Type
12. REFERENCE   nvarchar(200)  - Reference
13. REMARK      nvarchar(1000) - Remark (longer field)
14. DRAMOUNT    decimal(18,2)  - Debit Amount (#,###.00)
15. CRAMOUNT    decimal(18,2)  - Credit Amount (#,###.00)  
16. CRTDTM      datetime2      - Created DateTime (dd/MM/yyyy HH:mm:ss)
```

#### Positions 18-21: System Columns (NOT TEMPORAL)
```
17. Id              bigint        - Primary Key (Identity)
18. CREATED_DATE    datetime2     - Record Creation
19. UPDATED_DATE    datetime2     - Last Update  
20. FILE_NAME       nvarchar(500) - Source File Name
```

## 🗃️ CSV Structure Reference
File: `7800_gl02_2024120120241231.csv`

```csv
TRDATE,TRBRCD,USERID,JOURSEQ,DYTRSEQ,LOCAC,CCY,BUSCD,UNIT,TRCD,CUSTOMER,TRTP,REFERENCE,REMARK,DRAMOUNT,CRAMOUNT,CRTDTM
20241201,1000,1000A07800,1,1,421101,VND,DP,DA,,7800-329810401,Normal,7.80021E+12,Withdrawal BankNet ATM,203300,0,20241201 11:35:22
```

## 🏗️ Architecture Stack

### 1. Model Layer (`Models/DataTables/GL02.cs`)
- **Purpose**: Database model with exact column mapping
- **Features**: TRDATE->NGAY_DL conversion, Order attributes
- **Key**: CSV mapping property for import

### 2. Entity Layer (`Models/Entities/GL02Entity.cs`)  
- **Purpose**: EF Core entity with indexes
- **Features**: Columnstore indexes, nullable fields
- **Key**: DatabaseGenerated Identity for performance

### 3. DTO Layer (`Models/DTOs/GL02/GL02Dtos.cs`)
- **GL02PreviewDto**: Essential columns for UI preview
- **GL02DetailsDto**: All 21 columns for detail view  
- **GL02CreateDto**: Bulk insert operations with TRDATE mapping
- **GL02ImportResultDto**: Heavy file import metrics
- **GL02HeavyFileConfigDto**: Configuration settings

### 4. Repository Layer (`Repositories/GL02Repository.cs`)
- **IGL02Repository**: Heavy file operations interface
- **BulkCreateAsync()**: Batch insert 10,000 records
- **Complex queries**: Multi-criteria filtering
- **Statistics**: Summary operations for reporting

### 5. Service Layer (`Services/GL02Service.cs`)
- **ImportCSVAsync()**: Heavy file processing with CsvHelper
- **Batch processing**: Chunk large files into manageable batches
- **Error handling**: Comprehensive logging and recovery
- **Performance tracking**: Import metrics and timing

### 6. Controller Layer (`Controllers/GL02Controller.cs`)
- **Heavy file endpoints**: POST /api/GL02/import
- **File validation**: Size, name pattern, format checks
- **Query endpoints**: Multiple filter criteria
- **Configuration**: GET /api/GL02/config

## ⚙️ Heavy File Configuration

```csharp
MaxFileSizeBytes = 2GB
BulkInsertBatchSize = 10,000 records  
UploadTimeoutMinutes = 15 minutes
AllowedFilePatterns = "*gl02*"
UsePartitionedColumnstore = true
EnableCompressionOpts = true
```

## 🔄 Import Process Flow

### Phase 1: Validation
1. Check file exists and non-empty
2. Validate filename contains "gl02"
3. Verify file size ≤ 2GB
4. Check for duplicate imports

### Phase 2: CSV Parsing
1. Open stream with CsvHelper
2. Parse all records into `GL02CsvRecord`
3. Apply data type conversions
4. Map TRDATE → NGAY_DL

### Phase 3: Batch Processing
1. Chunk records into batches of 10,000
2. Convert to `GL02CreateDto` objects
3. Map to `GL02Entity` for database
4. Execute bulk insert per batch
5. Log progress and handle errors

### Phase 4: Result Tracking
1. Count processed/error records
2. Measure processing time
3. Generate import result metrics
4. Return comprehensive status

## 🔍 Query Optimization

### Primary Indexes
- `IX_GL02_NGAY_DL`: Date-based partitioning
- `IX_GL02_TRBRCD`: Branch code queries  
- `IX_GL02_LOCAC`: Local account lookups
- `IX_GL02_CCY`: Currency filtering
- `IX_GL02_CUSTOMER`: Customer transactions

### Query Patterns
```sql
-- Date range queries (partitioned)
WHERE NGAY_DL BETWEEN @StartDate AND @EndDate

-- Branch-specific queries  
WHERE TRBRCD = @BranchCode AND NGAY_DL >= @Date

-- Complex filtering
WHERE UNIT = @Unit AND CCY = @Currency AND LOCAC LIKE @Pattern
```

## 📈 Performance Metrics

### Expected Performance
- **Small files** (<10MB): ~1-2 seconds
- **Medium files** (10-100MB): ~10-30 seconds  
- **Large files** (100MB-1GB): ~1-5 minutes
- **Heavy files** (1-2GB): ~5-15 minutes

### Monitoring Points
- Batch processing rate (records/second)
- Memory usage during import
- Database connection pooling
- Disk I/O for columnstore writes
- Error recovery and rollback timing

## 🛠️ Development Guidelines

### Code Consistency
- All layers use exact same 17+4 column structure
- Business columns follow CSV order precisely  
- No Vietnamese transformation in column names
- NGAY_DL always derived from TRDATE

### Error Handling
- Batch-level error isolation
- Comprehensive logging with structured data
- Graceful degradation for partial imports
- Clear error messages for troubleshooting

### Testing Strategy
- Unit tests for each layer mapping
- Integration tests for full import pipeline
- Performance tests with various file sizes
- Error scenario testing (corrupt data, network issues)

## 🔧 Configuration Files

### appsettings.json
```json
{
  "GL02": {
    "MaxFileSizeBytes": 2147483648,
    "BulkInsertBatchSize": 10000,
    "UploadTimeoutMinutes": 15,
    "EnableHeavyFileLogging": true,
    "UseColumnstoreCompression": true
  }
}
```

## ✅ Validation Checklist

- [ ] Model matches CSV structure exactly (17 columns)
- [ ] Entity has correct data types and constraints  
- [ ] DTOs support TRDATE→NGAY_DL conversion
- [ ] Repository implements bulk operations
- [ ] Service handles batch processing correctly
- [ ] Controller validates file constraints  
- [ ] Import tracks metrics and errors
- [ ] All layers consistently use same structure
- [ ] Performance meets heavy file requirements
- [ ] Error handling covers edge cases

---

**Status**: ✅ SPECIFICATION COMPLETE
**Last Updated**: August 30, 2025
**Version**: 1.0 (Heavy File Optimized)
