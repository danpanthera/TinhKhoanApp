# Database Export Information

## Export Details
- **Date**: 2025-06-19 19:00:00
- **Source**: KhoanDB (SQL Server)
- **Container**: sql_server_tinhkhoan
- **Total Tables**: 27

## Exported Files

### Schema Files
- `schema_tables.sql` - Table definitions and structure
- `schema_indexes.sql` - All indexes (non-clustered, unique, etc.)
- `schema_constraints.sql` - Primary keys and foreign key constraints
- `README.sql` - Conversion notes and instructions

### Data Files (CSV format)
- `units_data.csv` - Units table data with headers
- `employees_data.csv` - Employees table data with headers  
- `roles_data.csv` - Roles table data with headers
- `kpidefinitions_data.csv` - KPI Definitions table data
- `khoanperiods_data.csv` - Khoan Periods table data

## Usage for PostgreSQL Migration

1. **Schema Creation**:
   ```bash
   # Review and modify data types in schema files
   # Apply in this order:
   psql -f schema_tables.sql
   psql -f schema_indexes.sql
   psql -f schema_constraints.sql
   ```

2. **Data Import**:
   ```bash
   # Import CSV files
   \COPY units FROM 'units_data.csv' WITH CSV HEADER;
   \COPY employees FROM 'employees_data.csv' WITH CSV HEADER;
   # ... etc for other tables
   ```

3. **Data Type Conversions Needed**:
   - `nvarchar` → `VARCHAR` or `TEXT`
   - `datetime2` → `TIMESTAMPTZ`
   - `bit` → `BOOLEAN`
   - `int IDENTITY` → `SERIAL`
   - `uniqueidentifier` → `UUID`

## Validation Steps
- Verify row counts after import
- Test foreign key relationships
- Validate data integrity
- Performance test with indexes
