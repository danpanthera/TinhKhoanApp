# DP01 TABLE COMPLETION PLAN - CSV-FIRST ARCHITECTURE

## PROGRESS TRACKING

-   ✅ DP01Entity: Completed with 63 business columns aligned to CSV structure
-   ✅ DP01PreviewDto: Created with all 63 business columns
-   ✅ DP01DetailsDto: Completed with full system + business fields
-   ✅ IDP01Repository: Updated to use DP01Entity
-   ✅ DP01Repository: Implemented with DP01Entity support
-   ✅ IDP01Service: Interface updated with correct DTO references
-   ✅ DP01Service: Service layer with full mapping implemented

## CURRENT ISSUES TO RESOLVE

### 1. NAMESPACE CONSISTENCY (HIGH PRIORITY)

**Problem**: System has mixed `DTOs` (uppercase) vs `Dtos` (lowercase) namespaces

-   Controllers/Services reference `KhoanApp.Api.Models.DTOs.*` (non-existent)
-   Actual DTOs located in `KhoanApp.Api.Models.Dtos.*`

**Solution**: Global find/replace across codebase

```bash
# Replace all DTOs with Dtos
find . -name "*.cs" -exec sed -i 's/Models\.DTOs/Models.Dtos/g' {} \;
```

### 2. DP01 INTEGRATION COMPLETION

**Remaining Tasks**:

-   ✅ Entity Framework DbContext registration for DP01Entity
-   ✅ Dependency injection setup (IDP01Service/Repository)
-   ✅ Controller namespace updates
-   🔄 Database migration (if needed)
-   🔄 Testing integration

### 3. MISSING DTOs FOR OTHER TABLES

**Tables Needing DTO Creation**:

-   GL01, GL02, GL41: Missing Preview/Details/Create/Update DTOs
-   EI01: Missing all DTO types
-   DPDA: Missing all DTO types
-   LN03: Partial DTO implementation
-   RR01: Currently disabled

## SYSTEMATIC COMPLETION APPROACH

### PHASE 1: NAMESPACE FIX (IMMEDIATE)

```bash
# Execute global namespace fix
cd /Users/nguyendat/Documents/Projects/KhoanApp/Backend/KhoanApp.Api
find . -name "*.cs" -exec sed -i 's/Models\.DTOs/Models.Dtos/g' {} \;
```

### PHASE 2: DP01 COMPLETION VALIDATION

1. Build test after namespace fix
2. Verify DP01Service/Repository integration
3. Test DP01Controller endpoints
4. Confirm CSV column alignment (63 business columns)

### PHASE 3: NEXT TABLE TARGET

**Selection Criteria**: Choose table with most complete existing structure

-   **Candidate**: GL01/GL02 (appear partially implemented)
-   **Approach**: Follow exact DP01 pattern:
    1. CSV analysis → Business column extraction
    2. Entity alignment → DTO creation → Service/Repository → Controller
    3. Ensure NGAY_DL → Business Columns → System Columns structure

## CSV-FIRST ARCHITECTURE COMPLIANCE

### VALIDATION CHECKLIST FOR EACH TABLE:

-   [ ] CSV file analyzed for exact column count and order
-   [ ] Entity matches CSV structure exactly (no transformations)
-   [ ] DTO contains all business columns + system fields
-   [ ] Service layer maps all 63 columns correctly
-   [ ] Repository uses correct Entity (not DataTable model)
-   [ ] Controller references correct namespace

## CRITICAL SUCCESS FACTORS

### 1. CSV Business Column Integrity

-   **Requirement**: All 63 DP01 business columns must map 1:1 with CSV
-   **Validation**: `head -1 CSV | column count === Entity property count`

### 2. No Data Transformation

-   **Rule**: Direct Import only, no column name changes
-   **Structure**: `NGAY_DL → Business Columns (1-63) → Temporal/System Columns`

### 3. Layer Consistency

-   **Pattern**: CSV ← Database ← Model ← EF ← BulkCopy ← Direct Import ← DTO ← Services ← Repository ← Entity ← Controller
-   **Validation**: Each layer must reference correct namespace and entity types

## NEXT IMMEDIATE ACTIONS

1. **Execute Namespace Fix** (5 minutes)
2. **Test Build** (validate DP01 completion)
3. **Choose Next Table** (GL01 or GL02 based on existing structure)
4. **Document Progress** (update this plan)

---

**OBJECTIVE**: Complete systematic table implementation following DP01 success pattern
**SUCCESS METRIC**: Each table achieves clean build with full CSV business column support
