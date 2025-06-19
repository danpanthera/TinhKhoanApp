## 📋 **HƯỚNG DẪN CHI TIẾT CHUYỂN ĐỔI TỪ SQL SERVER SANG POSTGRESQL**

Tôi sẽ hướng dẫn anh từng bước một cách chi tiết và an toàn nhất.

## 🛡️ **BƯỚC 1: BACKUP TOÀN BỘ DỮ LIỆU HIỆN TẠI**

### **1.1. Backup SQL Server Database**

```bash
# 1. Kết nối vào SQL Server container
docker exec -it <sql-server-container-name> /bin/bash

# 2. Backup database bằng sqlcmd
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourPassword' -Q "
BACKUP DATABASE TinhKhoanDB 
TO DISK = '/var/opt/mssql/backup/TinhKhoanDB_$(date +%Y%m%d_%H%M%S).bak'
WITH FORMAT, INIT, COMPRESSION"

# 3. Copy backup file ra host
docker cp <container-name>:/var/opt/mssql/backup/TinhKhoanDB_*.bak ./backups/
```

### **1.2. Export Schema và Data riêng biệt**

```sql
-- Tạo script cho schema (chạy trong Azure Data Studio hoặc SSMS)
-- 1. Right-click database → Tasks → Generate Scripts
-- 2. Select all tables, views, procedures
-- 3. Save as: TinhKhoanDB_Schema.sql

-- Export data dạng CSV cho mỗi table
-- Dùng Azure Data Studio: 
-- Right-click table → Export as CSV
```

### **1.3. Backup Source Code**

```bash
# Backup toàn bộ project
cd ~/Documents/Projects/TinhKhoanApp
git add .
git commit -m "Backup before PostgreSQL migration"
git push

# Tạo branch mới cho migration
git checkout -b feature/postgresql-migration
```

## 🐘 **BƯỚC 2: CÀI ĐẶT POSTGRESQL**

### **2.1. Cài PostgreSQL trên macOS**

```bash
# Option 1: Dùng Homebrew (Recommended)
brew install postgresql@16
brew services start postgresql@16

# Option 2: Dùng Postgres.app (GUI)
# Download từ https://postgresapp.com/
# Drag to Applications và start

# Option 3: Dùng Docker (cho consistency với team)
# Tạo file docker-compose.yml
```

### **2.2. Tạo docker-compose.yml**

```yaml
# ~/Documents/Projects/TinhKhoanApp/docker-compose.yml
version: '3.8'

services:
  postgres:
    image: postgres:16-alpine
    container_name: tinhkhoan_postgres
    restart: unless-stopped
    environment:
      POSTGRES_USER: tinhkhoan_user
      POSTGRES_PASSWORD: DevPassword123!
      POSTGRES_DB: tinhkhoandb
      PGDATA: /data/postgres
    volumes:
      - postgres_data:/data/postgres
      - ./Database/PostgreSQL/init:/docker-entrypoint-initdb.d
      - ./Database/PostgreSQL/backups:/backups
    ports:
      - "5432:5432"
    networks:
      - tinhkhoan_network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U tinhkhoan_user -d tinhkhoandb"]
      interval: 10s
      timeout: 5s
      retries: 5

  pgadmin:
    image: dpage/pgadmin4:latest
    container_name: tinhkhoan_pgadmin
    restart: unless-stopped
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@tinhkhoan.local
      PGADMIN_DEFAULT_PASSWORD: admin123
      PGADMIN_CONFIG_SERVER_MODE: 'False'
    volumes:
      - pgadmin_data:/var/lib/pgadmin
    ports:
      - "5050:80"
    networks:
      - tinhkhoan_network
    depends_on:
      - postgres

volumes:
  postgres_data:
  pgadmin_data:

networks:
  tinhkhoan_network:
    driver: bridge
```

### **2.3. Start PostgreSQL**

```bash
# Start services
cd ~/Documents/Projects/TinhKhoanApp
docker-compose up -d

# Verify PostgreSQL is running
docker-compose ps

# Test connection
docker exec -it tinhkhoan_postgres psql -U tinhkhoan_user -d tinhkhoandb -c "SELECT version();"
```

## 🔄 **BƯỚC 3: MIGRATE SCHEMA**

### **3.1. Tạo Schema Converter Script**

```sql
-- File: Database/PostgreSQL/init/01-create-schema.sql

-- Enable extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Create schemas
CREATE SCHEMA IF NOT EXISTS app;
CREATE SCHEMA IF NOT EXISTS audit;

-- Set search path
SET search_path TO app, public;

-- Units table
CREATE TABLE units (
    id SERIAL PRIMARY KEY,
    unit_id VARCHAR(50) UNIQUE NOT NULL,
    name VARCHAR(200) NOT NULL,
    parent_unit_id INTEGER REFERENCES units(id),
    unit_type INTEGER NOT NULL,
    description TEXT,
    sort_order INTEGER DEFAULT 999,
    is_active BOOLEAN DEFAULT true,
    created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    modified_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

-- Employees table
CREATE TABLE employees (
    id SERIAL PRIMARY KEY,
    employee_code VARCHAR(50) UNIQUE NOT NULL,
    full_name VARCHAR(200) NOT NULL,
    unit_id INTEGER REFERENCES units(id),
    position_id INTEGER,
    email VARCHAR(100),
    phone VARCHAR(20),
    is_active BOOLEAN DEFAULT true,
    created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    modified_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

-- Roles table
CREATE TABLE roles (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) UNIQUE NOT NULL,
    description TEXT,
    role_type VARCHAR(50),
    is_active BOOLEAN DEFAULT true,
    created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

-- KPI tables theo pattern cũ
CREATE TABLE kpi_definitions (
    id SERIAL PRIMARY KEY,
    kpi_code VARCHAR(50) UNIQUE NOT NULL,
    kpi_name VARCHAR(500) NOT NULL,
    description TEXT,
    unit VARCHAR(50),
    formula TEXT,
    is_active BOOLEAN DEFAULT true,
    created_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

-- Add indexes
CREATE INDEX idx_units_parent ON units(parent_unit_id);
CREATE INDEX idx_employees_unit ON employees(unit_id);
CREATE INDEX idx_employees_code ON employees(employee_code);
```

### **3.2. Tạo Delta Logging Schema**

```sql
-- File: Database/PostgreSQL/init/02-delta-logging-schema.sql

-- Delta logging tables
CREATE TABLE audit.import_batches (
    batch_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    import_type VARCHAR(100) NOT NULL,
    file_name VARCHAR(500),
    total_records INTEGER DEFAULT 0,
    inserted_records INTEGER DEFAULT 0,
    updated_records INTEGER DEFAULT 0,
    deleted_records INTEGER DEFAULT 0,
    error_records INTEGER DEFAULT 0,
    status VARCHAR(50) DEFAULT 'PENDING',
    started_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    completed_at TIMESTAMPTZ,
    error_message TEXT,
    imported_by VARCHAR(100)
);

-- Generic delta log table với partitioning
CREATE TABLE audit.delta_log (
    id BIGSERIAL,
    table_name VARCHAR(100) NOT NULL,
    record_id VARCHAR(200) NOT NULL,
    operation VARCHAR(10) NOT NULL CHECK (operation IN ('INSERT', 'UPDATE', 'DELETE')),
    old_data JSONB,
    new_data JSONB,
    changed_fields TEXT[],
    batch_id UUID REFERENCES audit.import_batches(batch_id),
    changed_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    changed_by VARCHAR(100),
    PRIMARY KEY (id, changed_at)
) PARTITION BY RANGE (changed_at);

-- Create partitions for current and next month
CREATE TABLE audit.delta_log_2024_01 PARTITION OF audit.delta_log
    FOR VALUES FROM ('2024-01-01') TO ('2024-02-01');
    
CREATE TABLE audit.delta_log_2024_02 PARTITION OF audit.delta_log
    FOR VALUES FROM ('2024-02-01') TO ('2024-03-01');

-- Indexes
CREATE INDEX idx_delta_log_table_record ON audit.delta_log(table_name, record_id);
CREATE INDEX idx_delta_log_batch ON audit.delta_log(batch_id);
CREATE INDEX idx_delta_log_changed_at ON audit.delta_log(changed_at);

-- Function to auto-create monthly partitions
CREATE OR REPLACE FUNCTION audit.create_monthly_partition()
RETURNS void AS $$
DECLARE
    start_date date;
    end_date date;
    partition_name text;
BEGIN
    start_date := date_trunc('month', CURRENT_DATE + interval '1 month');
    end_date := start_date + interval '1 month';
    partition_name := 'delta_log_' || to_char(start_date, 'YYYY_MM');
    
    EXECUTE format('CREATE TABLE IF NOT EXISTS audit.%I PARTITION OF audit.delta_log 
                    FOR VALUES FROM (%L) TO (%L)',
                    partition_name, start_date, end_date);
END;
$$ LANGUAGE plpgsql;
```

## 🔧 **BƯỚC 4: UPDATE APPLICATION CODE**

### **4.1. Update NuGet Packages**

```xml
<!-- Backend/TinhKhoanApp.Api/TinhKhoanApp.Api.csproj -->
<!-- Remove these -->
<!--
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.Data.SqlClient" Version="5.1.4" />
-->

<!-- Add these -->
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
<PackageReference Include="Npgsql" Version="8.0.0" />
<PackageReference Include="EFCore.NamingConventions" Version="8.0.0" />
```

### **4.2. Update appsettings.json**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=tinhkhoandb;Username=tinhkhoan_user;Password=DevPassword123!;Include Error Detail=true"
  },
  "DatabaseProvider": "PostgreSQL",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

### **4.3. Update Program.cs**

```csharp
// Program.cs
using Npgsql;

// Remove SQL Server configuration
// services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlServer(connectionString));

// Add PostgreSQL configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "app");
            npgsqlOptions.EnableRetryOnFailure(3);
        })
    .UseSnakeCaseNamingConvention() // Convert PascalCase to snake_case
    .LogTo(Console.WriteLine, LogLevel.Information);
});

// Add Data Protection với PostgreSQL
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<ApplicationDbContext>();

// Register Delta Logging Service
builder.Services.AddScoped<IDeltaLoggingService, PostgresDeltaLoggingService>();
builder.Services.AddScoped<IDataImportService, DataImportService>();
```

### **4.4. Update DbContext**

```csharp
// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using Npgsql;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets remain the same
    public DbSet<Unit> Units { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Role> Roles { get; set; }
    // ... other DbSets

    // New DbSets for Delta Logging
    public DbSet<ImportBatch> ImportBatches { get; set; }
    public DbSet<DeltaLog> DeltaLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set default schema
        modelBuilder.HasDefaultSchema("app");

        // Configure audit schema tables
        modelBuilder.Entity<ImportBatch>().ToTable("import_batches", "audit");
        modelBuilder.Entity<DeltaLog>().ToTable("delta_log", "audit");

        // Configure JSONB columns
        modelBuilder.Entity<DeltaLog>()
            .Property(e => e.OldData)
            .HasColumnType("jsonb");

        modelBuilder.Entity<DeltaLog>()
            .Property(e => e.NewData)
            .HasColumnType("jsonb");

        // Configure array columns
        modelBuilder.Entity<DeltaLog>()
            .Property(e => e.ChangedFields)
            .HasColumnType("text[]");

        // Apply configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    // Override SaveChanges to auto-populate timestamps
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;
            
            if (entry.State == EntityState.Added)
            {
                entity.CreatedDate = DateTime.UtcNow;
            }
            
            entity.ModifiedDate = DateTime.UtcNow;
        }
    }
}
```

## 📊 **BƯỚC 5: IMPLEMENT DELTA LOGGING SERVICE**

### **5.1. Create Delta Logging Service**

```csharp
// Services/PostgresDeltaLoggingService.cs
using Npgsql;
using NpgsqlTypes;

public interface IDeltaLoggingService
{
    Task<ImportResult> ImportWithDeltaAsync<T>(
        IEnumerable<T> records,
        string tableName,
        Func<T, string> keySelector,
        string importedBy = "System");
}

public class PostgresDeltaLoggingService : IDeltaLoggingService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PostgresDeltaLoggingService> _logger;

    public PostgresDeltaLoggingService(
        ApplicationDbContext context,
        ILogger<PostgresDeltaLoggingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ImportResult> ImportWithDeltaAsync<T>(
        IEnumerable<T> records,
        string tableName,
        Func<T, string> keySelector,
        string importedBy = "System")
    {
        var batchId = Guid.NewGuid();
        var importBatch = new ImportBatch
        {
            BatchId = batchId,
            ImportType = tableName,
            TotalRecords = records.Count(),
            Status = "PROCESSING",
            ImportedBy = importedBy,
            StartedAt = DateTime.UtcNow
        };

        _context.ImportBatches.Add(importBatch);
        await _context.SaveChangesAsync();

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 1. Truncate staging table
            await _context.Database.ExecuteSqlRawAsync(
                $"TRUNCATE TABLE app.{tableName}_staging");

            // 2. Bulk insert to staging
            await BulkInsertToStagingAsync(records, tableName);

            // 3. Process deltas
            var stats = await ProcessDeltasAsync(tableName, batchId, keySelector);

            // 4. Update import batch
            importBatch.InsertedRecords = stats.Inserted;
            importBatch.UpdatedRecords = stats.Updated;
            importBatch.Status = "COMPLETED";
            importBatch.CompletedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation(
                "Import completed for {TableName}: {Inserted} inserted, {Updated} updated",
                tableName, stats.Inserted, stats.Updated);

            return new ImportResult
            {
                BatchId = batchId,
                Inserted = stats.Inserted,
                Updated = stats.Updated,
                TotalProcessed = stats.Inserted + stats.Updated
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            
            importBatch.Status = "FAILED";
            importBatch.ErrorMessage = ex.Message;
            await _context.SaveChangesAsync();
            
            _logger.LogError(ex, "Import failed for {TableName}", tableName);
            throw;
        }
    }

    private async Task BulkInsertToStagingAsync<T>(
        IEnumerable<T> records, 
        string tableName)
    {
        var connection = (NpgsqlConnection)_context.Database.GetDbConnection();
        await connection.OpenAsync();

        using var writer = connection.BeginBinaryImport(
            $"COPY app.{tableName}_staging FROM STDIN (FORMAT BINARY)");

        foreach (var record in records)
        {
            writer.StartRow();
            
            // Map properties to columns
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(record);
                writer.Write(value ?? DBNull.Value);
            }
        }

        await writer.CompleteAsync();
    }

    private async Task<DeltaStats> ProcessDeltasAsync(
        string tableName, 
        Guid batchId,
        Func<dynamic, string> keySelector)
    {
        var sql = $@"
            WITH changes AS (
                SELECT 
                    s.*,
                    c.data as current_data,
                    CASE 
                        WHEN c.id IS NULL THEN 'INSERT'
                        WHEN s.data IS DISTINCT FROM c.data THEN 'UPDATE'
                    END as operation
                FROM app.{tableName}_staging s
                LEFT JOIN app.{tableName}_current c 
                    ON s.record_key = c.record_key
                WHERE c.id IS NULL OR s.data IS DISTINCT FROM c.data
            ),
            logged AS (
                INSERT INTO audit.delta_log 
                (table_name, record_id, operation, old_data, new_data, batch_id)
                SELECT 
                    '{tableName}',
                    record_key,
                    operation,
                    current_data,
                    data,
                    @batchId::uuid
                FROM changes
                RETURNING operation
            )
            SELECT 
                COUNT(*) FILTER (WHERE operation = 'INSERT') as inserted,
                COUNT(*) FILTER (WHERE operation = 'UPDATE') as updated
            FROM logged";

        var stats = await _context.Database
            .SqlQueryRaw<DeltaStats>(sql, new NpgsqlParameter("batchId", batchId))
            .FirstOrDefaultAsync();

        // Apply changes to current table
        await _context.Database.ExecuteSqlRawAsync($@"
            INSERT INTO app.{tableName}_current 
            SELECT * FROM app.{tableName}_staging
            ON CONFLICT (record_key) 
            DO UPDATE SET 
                data = EXCLUDED.data,
                modified_date = CURRENT_TIMESTAMP");

        return stats ?? new DeltaStats();
    }
}
```

## 🔄 **BƯỚC 6: MIGRATE DATA**

### **6.1. Create Migration Scripts**

```bash
# Create migration directory
mkdir -p Database/Migrations

# Create data export script
cat > Database/Migrations/export_sqlserver_data.sql << 'EOF'
-- Export each table to CSV
-- Run in Azure Data Studio or SSMS

-- Units
SELECT * FROM Units
INTO OUTFILE '/tmp/units.csv'
FIELDS TERMINATED BY ',' 
ENCLOSED BY '"'
LINES TERMINATED BY '\n';

-- Employees  
SELECT * FROM Employees
INTO OUTFILE '/tmp/employees.csv'
FIELDS TERMINATED BY ','
ENCLOSED BY '"' 
LINES TERMINATED BY '\n';

-- Continue for other tables...
EOF
```

### **6.2. Import Data to PostgreSQL**

```sql
-- File: Database/Migrations/import_to_postgresql.sql

-- Import Units
COPY app.units (id, unit_id, name, parent_unit_id, unit_type, description, sort_order, is_active)
FROM '/tmp/units.csv' 
WITH (FORMAT CSV, HEADER true, DELIMITER ',', QUOTE '"');

-- Import Employees
COPY app.employees (id, employee_code, full_name, unit_id, position_id, email, phone, is_active)
FROM '/tmp/employees.csv'
WITH (FORMAT CSV, HEADER true, DELIMITER ',', QUOTE '"');

-- Reset sequences
SELECT setval('app.units_id_seq', (SELECT MAX(id) FROM app.units));
SELECT setval('app.employees_id_seq', (SELECT MAX(id) FROM app.employees));
```

### **6.3. Run Migration**

```bash
# 1. Export from SQL Server
docker exec -it <sql-container> /opt/mssql-tools/bin/bcp "SELECT * FROM TinhKhoanDB.dbo.Units" queryout /tmp/units.csv -c -t, -T -S localhost -U sa

# 2. Copy files
docker cp <sql-container>:/tmp/*.csv ./Database/Migrations/

# 3. Import to PostgreSQL
docker cp ./Database/Migrations/*.csv tinhkhoan_postgres:/tmp/
docker exec -it tinhkhoan_postgres psql -U tinhkhoan_user -d tinhkhoandb -f /docker-entrypoint-initdb.d/import_data.sql
```

## ✅ **BƯỚC 7: TESTING & VALIDATION**

### **7.1. Create Test Script**

```csharp
// Tests/PostgreSQLMigrationTests.cs
[TestClass]
public class PostgreSQLMigrationTests
{
    private readonly ApplicationDbContext _context;

    [TestMethod]
    public async Task ValidateDataMigration()
    {
        // 1. Count records
        var unitCount = await _context.Units.CountAsync();
        var employeeCount = await _context.Employees.CountAsync();
        
        Assert.IsTrue(unitCount > 0, "Units should be migrated");
        Assert.IsTrue(employeeCount > 0, "Employees should be migrated");

        // 2. Test relationships
        var unitsWithParent = await _context.Units
            .Include(u => u.ParentUnit)
            .Where(u => u.ParentUnitId != null)
            .ToListAsync();
            
        Assert.IsTrue(unitsWithParent.All(u => u.ParentUnit != null));

        // 3. Test Delta Logging
        var testData = new List<Employee> { /* test employees */ };
        var result = await _deltaService.ImportWithDeltaAsync(
            testData, "employees", e => e.EmployeeCode);
            
        Assert.IsTrue(result.TotalProcessed > 0);
    }
}
```

### **7.2. Verify Application**

```bash
# 1. Run application
cd Backend/TinhKhoanApp.Api
dotnet run

# 2. Test endpoints
curl http://localhost:5000/api/units
curl http://localhost:5000/api/employees

# 3. Test import
curl -X POST http://localhost:5000/api/import/employees \
  -F "file=@test_employees.csv"
```

## 📋 **BƯỚC 8: FINAL CHECKLIST**

```markdown
## Migration Completion Checklist

### Pre-Migration
- [x] Backup SQL Server database
- [x] Backup source code
- [x] Document current state

### PostgreSQL Setup  
- [x] Install PostgreSQL
- [x] Create database and users
- [x] Setup pgAdmin

### Schema Migration
- [x] Convert SQL Server schema to PostgreSQL
- [x] Create Delta Logging tables
- [x] Setup partitioning

### Code Migration
- [x] Update NuGet packages
- [x] Update connection strings
- [x] Update DbContext
- [x] Implement Delta Logging Service

### Data Migration
- [x] Export data from SQL Server
- [x] Import data to PostgreSQL
- [x] Verify data integrity
- [x] Test relationships

### Testing
- [x] Unit tests pass
- [x] Integration tests pass
- [x] Manual testing complete
- [x] Performance acceptable

### Cleanup
- [ ] Remove SQL Server containers
- [ ] Update documentation
- [ ] Update deployment scripts
- [ ] Notify team
```

## 🎉 **BƯỚC 9: COMMIT & CELEBRATE**

```bash
# Commit changes
git add .
git commit -m "feat: Migrate from SQL Server to PostgreSQL with Delta Logging"
git push origin feature/postgresql-migration

# Create Pull Request
gh pr create --title "PostgreSQL Migration" --body "Migrated from SQL Server to PostgreSQL with Delta Logging support"

# Stop old SQL Server
docker-compose -f docker-compose.old.yml down

# Celebrate! 🎉
echo "Migration completed successfully!"
```

## 📚 **RESOURCES & TROUBLESHOOTING**

### **Common Issues & Solutions**

```yaml
Issue: "column does not exist"
Solution: PostgreSQL is case-sensitive. Use double quotes or snake_case

Issue: "duplicate key violation"  
Solution: Reset sequences after import

Issue: "could not connect to server"
Solution: Check PostgreSQL is running and ports are correct

Issue: "permission denied"
Solution: Grant proper permissions to database user
```

### **Useful Commands**

```bash
# PostgreSQL
psql -U tinhkhoan_user -d tinhkhoandb  # Connect
\dt app.*                               # List tables
\d+ app.employees                       # Describe table
\q                                      # Quit

# Docker
docker-compose logs -f postgres         # View logs
docker-compose exec postgres pg_dump    # Backup
docker-compose restart postgres         # Restart
```

**Chúc anh migration thành công! Nếu gặp vấn đề gì, hãy cho tôi biết nhé!** 🚀

Similar code found with 1 license typeusing Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq; // Cần cho .Any()
using System.Threading.Tasks;
using TinhKhoanApp.Api.Data; // Để sử dụng ApplicationDbContext
using TinhKhoanApp.Api.Models; // Để sử dụng Model Role

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")] // Đường dẫn sẽ là "api/Roles"
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Roles (Lấy tất cả các Vai trò)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }

        // GET: api/Roles/5 (Lấy một Vai trò theo Id)
        [HttpGet("{id}")]
        public async Task<ActionResult<Role>> GetRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound(); // Trả về 404 Not Found nếu không tìm thấy
            }

            return Ok(role); // Trả về Vai trò tìm được
        }

        // POST: api/Roles (Tạo một Vai trò mới)
        [HttpPost]
        public async Task<ActionResult<Role>> PostRole([FromBody] Role role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra xem tên vai trò đã tồn tại chưa (vì tên vai trò nên là duy nhất)
            if (await _context.Roles.AnyAsync(r => r.Name == role.Name))
            {
                //ModelState.AddModelError("Name", "Tên vai trò đã tồn tại."); // Thêm lỗi vào ModelState
                return Conflict(new { message = $"Tên vai trò '{role.Name}' đã tồn tại." }); // Trả về lỗi 409 Conflict
            }

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
        }

        // PUT: api/Roles/5 (Cập nhật một Vai trò đã có)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole(int id, [FromBody] Role role)
        {
            if (id != role.Id)
            {
                return BadRequest("ID trong URL không khớp với ID trong nội dung request.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra xem tên vai trò mới có trùng với tên vai trò khác không (ngoại trừ chính nó)
            if (await _context.Roles.AnyAsync(r => r.Name == role.Name && r.Id != id))
            {
                //ModelState.AddModelError("Name", "Tên vai trò đã tồn tại.");
                return Conflict(new { message = $"Tên vai trò '{role.Name}' đã tồn tại." });
            }

            _context.Entry(role).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Roles.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Trả về 204 No Content sau khi cập nhật thành công
        }

        // DELETE: api/Roles/5 (Xóa một Vai trò)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            // Cân nhắc: Nếu Vai trò này đang được gán cho Nhân viên nào đó (qua bảng EmployeeRoles),
            // Sếp có muốn cho xóa không? Hoặc Sếp có muốn xóa các bản ghi tương ứng trong EmployeeRoles trước không?
            // Hiện tại, nếu có ràng buộc khóa ngoại, EF Core sẽ báo lỗi.
            // Mình sẽ thêm logic kiểm tra và xử lý chi tiết hơn sau này nếu cần.
            // Ví dụ, kiểm tra xem có EmployeeRole nào đang sử dụng RoleId này không:
            var isRoleInUse = await _context.EmployeeRoles.AnyAsync(er => er.RoleId == id);
            if (isRoleInUse)
            {
                return BadRequest(new { message = $"Vai trò này đang được sử dụng bởi một hoặc nhiều nhân viên và không thể xóa." });
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent(); // Trả về 204 No Content sau khi xóa thành công
        }
    }
}