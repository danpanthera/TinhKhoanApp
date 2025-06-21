-- Script tạo indexes tối ưu cho SQL Server
-- Date: 2025-06-20

-- 1. Indexes cho bảng Employees
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_employees_unitid')
    CREATE NONCLUSTERED INDEX idx_employees_unitid ON [Employees](unit_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_employees_positionid')
    CREATE NONCLUSTERED INDEX idx_employees_positionid ON [Employees](position_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_employees_username')
    CREATE NONCLUSTERED INDEX idx_employees_username ON [Employees](username);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_employees_employeecode')
    CREATE NONCLUSTERED INDEX idx_employees_employeecode ON [Employees](employee_code);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_employees_isactive')
    CREATE NONCLUSTERED INDEX idx_employees_isactive ON [Employees](is_active);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_employees_fullname')
    CREATE NONCLUSTERED INDEX idx_employees_fullname ON [Employees](full_name);

-- 2. Indexes cho bảng Units
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_units_parentunitid')
    CREATE NONCLUSTERED INDEX idx_units_parentunitid ON [Units](parent_unit_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_units_type')
    CREATE NONCLUSTERED INDEX idx_units_type ON [Units]([UnitType]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_units_code')
    CREATE NONCLUSTERED INDEX idx_units_code ON [Units]([UnitCode]);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_units_name')
    CREATE NONCLUSTERED INDEX idx_units_name ON [Units]([UnitName]);

-- 3. Indexes cho bảng Positions
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_positions_name')
    CREATE NONCLUSTERED INDEX idx_positions_name ON [Positions](name);

-- 4. Indexes cho bảng Roles
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_roles_name')
    CREATE NONCLUSTERED INDEX idx_roles_name ON [Roles](name);

-- 5. Indexes cho bảng EmployeeRoles
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_employeeroles_employeeid')
    CREATE NONCLUSTERED INDEX idx_employeeroles_employeeid ON [EmployeeRoles](employee_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_employeeroles_roleid')
    CREATE NONCLUSTERED INDEX idx_employeeroles_roleid ON [EmployeeRoles](role_id);

-- 6. Indexes cho bảng KpiIndicators
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_kpiindicators_tableid')
    CREATE NONCLUSTERED INDEX idx_kpiindicators_tableid ON [KpiIndicators](table_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_kpiindicators_orderindex')
    CREATE NONCLUSTERED INDEX idx_kpiindicators_orderindex ON [KpiIndicators](order_index);

-- 7. Indexes cho Import tables
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_rawdataimports_datatype')
    CREATE NONCLUSTERED INDEX idx_rawdataimports_datatype ON raw_data_imports(data_type);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_rawdataimports_statementdate')
    CREATE NONCLUSTERED INDEX idx_rawdataimports_statementdate ON raw_data_imports(statement_date);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_rawdatarecords_importid')
    CREATE NONCLUSTERED INDEX idx_rawdatarecords_importid ON raw_data_records(raw_data_import_id);

-- 8. Composite indexes cho queries phức tạp
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_employees_unit_position')
    CREATE NONCLUSTERED INDEX idx_employees_unit_position ON [Employees](unit_id, position_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_employees_active_unit')
    CREATE NONCLUSTERED INDEX idx_employees_active_unit ON [Employees](is_active, unit_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_kpiindicators_table_order')
    CREATE NONCLUSTERED INDEX idx_kpiindicators_table_order ON [KpiIndicators](table_id, order_index);

-- 9. Indexes cho KPI tables
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_kpidefinitions_name')
    CREATE NONCLUSTERED INDEX idx_kpidefinitions_name ON kpi_definitions(kpi_name);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_kpidefinitions_isactive')
    CREATE NONCLUSTERED INDEX idx_kpidefinitions_isactive ON kpi_definitions(is_active);

-- 10. Indexes cho Assignment tables
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_employeekpiassignments_employeeid')
    CREATE NONCLUSTERED INDEX idx_employeekpiassignments_employeeid ON [EmployeeKpiAssignments](employee_id);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'idx_employeekpiassignments_kpiid')
    CREATE NONCLUSTERED INDEX idx_employeekpiassignments_kpiid ON [EmployeeKpiAssignments](kpi_definition_id);

-- Update statistics for SQL Server
UPDATE STATISTICS [Employees];
UPDATE STATISTICS [Units];
UPDATE STATISTICS [Positions];
UPDATE STATISTICS [Roles];
UPDATE STATISTICS [EmployeeRoles];
UPDATE STATISTICS [KpiIndicators];
UPDATE STATISTICS kpi_definitions;
UPDATE STATISTICS raw_data_imports;
UPDATE STATISTICS raw_data_records;
