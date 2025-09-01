# ARCHITECTURE STANDARDIZATION PLAN
## Thống nhất Entity ↔ Database cho 6 bảng BẤT KHỚP

### PATTERN CHUẨN (từ 3 bảng KHỚP: EI01, GL02, LN03):
- **Namespace**: `Models.DataTables` 
- **System Columns**: `CREATED_DATE`, `UPDATED_DATE` (KHÔNG `CreatedAt`)
- **Primary Key**: `Id` (long/int)
- **Column Order**: NGAY_DL → Business Columns → System Columns
- **Temporal**: Shadow properties (ValidFrom/ValidTo managed by SQL Server)

### FIX PLAN CHO 6 BẢNG BẤT KHỚP:

#### 1. DP01 (Model thiếu system columns):
- ✅ Current: DataTables pattern 
- ❌ Missing: CREATED_DATE, UPDATED_DATE columns
- 🔧 Action: Add missing system columns

#### 2. DPDA (System columns mismatch):  
- ✅ Current: DataTables pattern
- ❌ Wrong: CreatedAt, UpdatedAt instead of CREATED_DATE, UPDATED_DATE
- 🔧 Action: Rename system columns to match database

#### 3. GL01 (System columns mismatch):
- ✅ Current: DataTables pattern
- ❌ Wrong: CreatedAt, UpdatedAt instead of CREATED_DATE, UPDATED_DATE  
- 🔧 Action: Rename system columns to match database

#### 4. GL41 (Missing columns + naming issues):
- ✅ Current: DataTables pattern
- ❌ Missing: BATCH_ID, IMPORT_SESSION_ID columns
- ❌ Wrong: ID vs Id naming inconsistency
- 🔧 Action: Add missing columns, fix naming

#### 5. LN01 (Hoàn toàn sai pattern):
- ❌ Current: Entity pattern với CreatedAt, SysStartTime
- ❌ Location: Models/Entities/ instead of Models/DataTables/
- 🔧 Action: Convert từ Entity pattern → DataTables pattern

#### 6. RR01 (Thiếu system columns):
- ✅ Current: DataTables pattern
- ❌ Missing: CREATED_DATE, UPDATED_DATE columns
- 🔧 Action: Add missing system columns

### EXECUTION ORDER:
1. Fix easy ones first: DPDA, GL01 (rename columns)
2. Add missing columns: DP01, RR01 (add system columns)  
3. Fix complex: GL41 (missing columns + naming)
4. Complete rewrite: LN01 (Entity → DataTables conversion)
5. Update ApplicationDbContext configurations
6. Test IndexInitializers after alignment

### POST-FIX VALIDATION:
- ✅ All 9 models use DataTables pattern
- ✅ All system columns: CREATED_DATE, UPDATED_DATE
- ✅ All Primary Keys: Id (consistent casing)
- ✅ Column orders match database structure
- ✅ IndexInitializers work without "column not exist" errors
