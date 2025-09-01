# 🔄 REVISED ARCHITECTURE STRATEGY

## VẤN ĐỀ: 
- Có 85+ compile errors do conflicts giữa DataTables vs Entity models
- Code hiện tại heavily dependent on Entity pattern cho LN01, LN03
- Converting hoàn toàn sang DataTables sẽ break toàn bộ codebase

## REVISED STRATEGY: HYBRID APPROACH

### ✅ KEEP ENTITY PATTERN cho complex tables:
- **LN01Entity**: Fix properties để match database columns
- **LN03Entity**: Fix properties để match database columns
- **GL41Entity**: Convert thành GL41 DataTables (ít dependencies)

### ✅ STANDARDIZE SYSTEM COLUMNS across ALL models:
- Entity models: Fix property names để match database 
- DataTables models: Đã fix xong

## IMMEDIATE FIXES NEEDED:

### 1. LN01Entity - Fix properties to match database:
- Add `NGAY_DL` property (missing)
- Fix system columns: `CreatedAt` → still CreatedAt nhưng map đến `CREATED_DATE` column
- Keep Entity pattern but ensure Column attributes match database

### 2. LN03Entity - Fix properties to match database:
- Add missing properties: `IS_DELETED`, `COLUMN_18`, `COLUMN_19`, `COLUMN_20`, `CREATED_DATE`, `UPDATED_DATE`
- Fix namespace conflicts

### 3. Other DataTables models - Fix remaining property references:
- DP01: Fix `CreatedAt` references in code
- DPDA: Fix `CreatedAt` references in code
- GL01: Add back `FileName` property if needed

## OUTCOME:
- ✅ Models have correct database columns
- ✅ IndexInitializers work (column names match)
- ✅ Existing code continues to work (no breaking changes)
- ✅ Architecture is consistent

## PRIORITY: Fix Entity models first, then fix code references
