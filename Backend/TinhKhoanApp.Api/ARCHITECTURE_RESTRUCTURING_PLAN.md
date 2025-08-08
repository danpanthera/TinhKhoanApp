# 🏗️ COMPREHENSIVE CODE ARCHITECTURE RESTRUCTURING PLAN

## Áp dụng Clean Architecture cho TinhKhoanApp - August 8, 2025

### 📋 **CURRENT STATE ANALYSIS**

**✅ EXISTING COMPONENTS (PARTIAL):**

-   **Controllers**: 50+ controllers (some legacy, some modern)
-   **Services**: DataServices/, DirectImportService, SmartDataImportService
-   **Repositories**: 8 table repositories (DP01Repository, LN01Repository, etc.)
-   **Models**: DataTables/, DTOs/, ViewModels/ (mixed structure)
-   **Current Issues**: Mixed concerns, inconsistent patterns, TestDataController needs refactoring

**❌ MISSING COMPONENTS:**

-   Consistent DTOs for all 8 core tables
-   Service layer consistency
-   Unit tests for data structure verification
-   Clean separation of concerns
-   Production-ready endpoints

### 🎯 **RESTRUCTURING OBJECTIVES**

1. **Repository Layer**: ✅ **EXISTS** - 8 table repositories (User: phải là 9 tables ?)
2. **Service Layer**: 🔄 **STANDARDIZE** - Consistent business logic services
3. **DTO Layer**: ❌ **CREATE** - Complete DTO/ViewModels for all tables
4. **Controller Layer**: 🔄 **REFACTOR** - Clean HTTP request handling
5. **Unit Tests**: ❌ **CREATE** - Structure and functionality verification
6. **Consistency Verification**: ❌ **IMPLEMENT** - Cross-layer validation

### 🚀 **IMPLEMENTATION PHASES**

#### **PHASE 1: DTO/VIEWMODEL STANDARDIZATION (2-3 hours)**

-   Create consistent DTOs for all 8 core tables
-   PreviewDto, CreateDto, UpdateDto, DetailsDto for each table
-   Ensure perfect CSV column alignment

#### **PHASE 2: SERVICE LAYER CONSISTENCY (3-4 hours)**

-   Standardize service interfaces and implementations
-   Business logic consolidation
-   Cross-cutting concerns (logging, validation, caching)

#### **PHASE 3: CONTROLLER REFACTORING (2-3 hours)**

-   Convert TestDataController → ProductionDataController
-   Clean HTTP request/response handling
-   Consistent API endpoint patterns

#### **PHASE 4: UNIT TESTING FRAMEWORK (4-5 hours)**

-   Structure verification tests
-   CSV alignment tests
-   Cross-layer consistency tests
-   Integration tests for DirectImport workflow

#### **PHASE 5: CROSS-LAYER VALIDATION (1-2 hours)**

-   Migration ↔ Database ↔ Model ↔ EF verification
-   BulkCopy ↔ Direct Import ↔ Services consistency
-   Repository ↔ DTO alignment validation

### 📊 **8 CORE TABLES COMPLIANCE MATRIX**

| Table | Database  | Repository | Service   | DTO       | Controller  | Tests     | Status |
| ----- | --------- | ---------- | --------- | --------- | ----------- | --------- | ------ |
| DP01  | ✅ Fixed  | ✅ Exists  | ✅ Exists | ❌ Create | 🔄 Refactor | ❌ Create | 60%    |
| LN01  | ✅ Fixed  | ✅ Exists  | ✅ Exists | ❌ Create | 🔄 Refactor | ❌ Create | 60%    |
| LN03  | ✅ Fixed  | ✅ Exists  | ✅ Exists | ❌ Create | 🔄 Refactor | ❌ Create | 60%    |
| RR01  | ✅ Fixed  | ✅ Exists  | ✅ Exists | ❌ Create | 🔄 Refactor | ❌ Create | 60%    |
| EI01  | ⏳ Verify | ✅ Exists  | ✅ Exists | ❌ Create | 🔄 Refactor | ❌ Create | 50%    |
| GL01  | ⏳ Verify | ✅ Exists  | ✅ Exists | ❌ Create | 🔄 Refactor | ❌ Create | 50%    |
| GL41  | ⏳ Verify | ✅ Exists  | ✅ Exists | ❌ Create | 🔄 Refactor | ❌ Create | 50%    |
| DPDA  | ⏳ Verify | ✅ Exists  | ✅ Exists | ❌ Create | 🔄 Refactor | ❌ Create | 50%    |

### 🔧 **TECHNICAL IMPLEMENTATION STANDARDS**

**Repository Pattern:**

```csharp
public interface IDP01Repository : IRepository<DP01>
{
    Task<IEnumerable<DP01>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<DP01PreviewDto> GetPreviewDataAsync(int limit = 10);
    Task<int> BulkInsertAsync(IEnumerable<DP01> entities);
}
```

**Service Pattern:**

```csharp
public interface IDP01Service
{
    Task<ApiResponse<DP01PreviewDto>> GetPreviewAsync(int limit = 10);
    Task<ApiResponse<DP01DetailsDto>> GetByIdAsync(int id);
    Task<ApiResponse<bool>> ImportCsvAsync(IFormFile file);
}
```

**DTO Pattern:**

```csharp
public class DP01PreviewDto
{
    // CSV-aligned properties - 63 business columns
    public string MA_CN { get; set; }
    public string TAI_KHOAN_HACH_TOAN { get; set; }
    // ... remaining 61 business columns

    // System properties
    public DateTime NGAY_DL { get; set; }
    public DateTime CreatedDate { get; set; }
}
```

### 📋 **SUCCESS CRITERIA**

1. **100% CSV Alignment**: All DTOs match CSV structure exactly
2. **Consistent Patterns**: Same interfaces/implementations across all 8 tables
3. **Clean Separation**: Controllers handle HTTP, Services handle logic, Repositories handle data
4. **Full Test Coverage**: Unit tests verify structure and functionality
5. **Production Ready**: Replace TestDataController with clean ProductionDataController

### ⚡ **NEXT STEPS**

1. **Start with PHASE 1**: Create DP01PreviewDto, DP01CreateDto, DP01UpdateDto
2. **Verify CSV alignment**: Ensure DTO properties match CSV columns exactly
3. **Replicate pattern**: Apply same DTO structure to all 8 tables
4. **Test consistency**: Cross-layer validation scripts

**ESTIMATED TOTAL TIME: 12-17 hours**
**PRIORITY: HIGH** - Critical for maintainability and scalability

---

**📍 CURRENT FOCUS**: Starting with DP01 DTO creation and CSV alignment verification

🚀 Next Steps Available:
Interface Creation - Tạo repository & service interfaces
Controller Implementation - API endpoints cho all tables
Dependency Injection - Configure all services trong Startup
Integration Testing - Test end-to-end workflows
Database Migration - Apply all entity changes
