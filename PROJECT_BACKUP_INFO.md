# TinhKhoanApp - Source Code Backup Information

## Project Structure
```
TinhKhoanApp/
├── Backend/TinhKhoanApp.Api/          # .NET 8 Web API
│   ├── Controllers/                   # API Controllers
│   ├── Data/                         # Entity Framework DbContext & Models  
│   ├── Database/                     # Database exports & migration files
│   │   ├── Backups/                  # SQL Server backups
│   │   ├── Schema/                   # Schema export files
│   │   └── Data/                     # Data export files (CSV)
│   ├── Models/                       # Domain models
│   ├── Services/                     # Business logic services
│   └── Migrations/                   # EF Core migrations
├── Frontend/tinhkhoan-app-ui/         # Vue.js frontend (legacy)
└── Frontend/tinhkhoan-app-ui-vite/    # Vite + Vue.js frontend (current)
```

## Technology Stack
- **Backend**: .NET 8, Entity Framework Core, SQL Server (Docker)
- **Frontend**: Vue.js 3, Vite, Tailwind CSS, PWA enabled
- **Database**: SQL Server (current), migrating to PostgreSQL

## Git Repository Status
- **Initialized**: 2025-06-19
- **Initial commit**: c0cd3b0 - Database backup before PostgreSQL migration
- **Current branch**: main

## Database Backup Information

### Full Backup
- **File**: `TinhKhoanDB_20250619_185213.bak` (2.5MB)
- **Date**: 2025-06-19 18:52:13
- **Tables**: 27 tables
- **Container**: sql_server_tinhkhoan
- **SA Password**: YourStrong@Password123

### Schema Export
- `schema_tables.sql` - Table structures
- `schema_indexes.sql` - Database indexes  
- `schema_constraints.sql` - Primary/Foreign keys
- `README.sql` - Migration notes

### Data Export (CSV)
- `units_data.csv` - Units/branches data
- `employees_data.csv` - Employee records
- `roles_data.csv` - Role definitions
- `kpidefinitions_data.csv` - KPI definitions
- `khoanperiods_data.csv` - Khoan periods

## Key Features Implemented
- Employee management with role assignments
- KPI definitions and scoring system
- Unit/branch hierarchical structure
- Data import from CSV/Excel files
- Payroll calculation system
- PWA capabilities with offline support

## Next Steps
1. ✅ Backup completed
2. 🔄 Setup PostgreSQL environment
3. 🔄 Implement Delta Logging pattern
4. 🔄 Migrate data to PostgreSQL
5. 🔄 Update application code for PostgreSQL

## Migration Goals
- Move from SQL Server to PostgreSQL
- Implement Incremental Delta Logging for efficient data management
- Improve performance for large dataset imports
- Maintain data integrity and historical tracking

## Contact & Maintenance
- **Repository**: Local Git repository
- **Backup Strategy**: Git commits + database exports
- **Documentation**: Embedded in codebase + markdown files
