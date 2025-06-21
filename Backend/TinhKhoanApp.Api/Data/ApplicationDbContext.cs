using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models; // Đảm bảo namespace này đúng với nơi Sếp đặt các Model
using TinhKhoanApp.Api.Models.RawData; // Thêm namespace cho Raw Data models
using TinhKhoanApp.Api.Models.Temporal; // Thêm namespace cho Temporal models

namespace TinhKhoanApp.Api.Data // Sử dụng block-scoped namespace cho rõ ràng
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Khai báo các DbSet<T> cho mỗi Model
        public DbSet<Unit> Units { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<EmployeeRole> EmployeeRoles { get; set; }
        public DbSet<KhoanPeriod> KhoanPeriods { get; set; }
        public DbSet<EmployeeKhoanAssignment> EmployeeKhoanAssignments { get; set; }
        public DbSet<EmployeeKhoanAssignmentDetail> EmployeeKhoanAssignmentDetails { get; set; }
        public DbSet<UnitKhoanAssignment> UnitKhoanAssignments { get; set; }
        public DbSet<UnitKhoanAssignmentDetail> UnitKhoanAssignmentDetails { get; set; }
        public DbSet<TransactionAdjustmentFactor> TransactionAdjustmentFactors { get; set; }
        public DbSet<SalaryParameter> SalaryParameters { get; set; }
        public DbSet<FinalPayout> FinalPayouts { get; set; }

        // DbSets cho hệ thống KPI mới
        public DbSet<KpiAssignmentTable> KpiAssignmentTables { get; set; }
        public DbSet<KpiIndicator> KpiIndicators { get; set; }
        public DbSet<EmployeeKpiTarget> EmployeeKpiTargets { get; set; }
        public DbSet<KPIDefinition> KPIDefinitions { get; set; }
        public DbSet<EmployeeKpiAssignment> EmployeeKpiAssignments { get; set; }

        // DbSets cho hệ thống chấm điểm KPI Chi nhánh
        public DbSet<UnitKpiScoring> UnitKpiScorings { get; set; }
        public DbSet<UnitKpiScoringDetail> UnitKpiScoringDetails { get; set; }
        public DbSet<UnitKpiScoringCriteria> UnitKpiScoringCriterias { get; set; }
        
        // DbSet cho bảng quy tắc tính điểm cộng/trừ KPI
        public DbSet<KpiScoringRule> KpiScoringRules { get; set; }

        // DbSets cho hệ thống Import dữ liệu
        public DbSet<ImportedDataRecord> ImportedDataRecords { get; set; }
        public DbSet<ImportedDataItem> ImportedDataItems { get; set; }
        
        // 🗄️ DbSets cho hệ thống Kho Dữ liệu Thô (Legacy)
        public DbSet<Models.RawDataImport> LegacyRawDataImports { get; set; }
        public DbSet<RawDataRecord> RawDataRecords { get; set; }
        
        // 🚀 DbSets cho hệ thống Temporal Tables (High Performance)
        public DbSet<Models.Temporal.RawDataImport> RawDataImports { get; set; }
        public DbSet<RawDataImportArchive> RawDataImportArchives { get; set; }
        public DbSet<Models.Temporal.ImportLog> ImportLogs { get; set; }
        
        // 📊 DbSets cho hệ thống SCD Type 2 History Tables
        public DbSet<LN01History> LN01History { get; set; }
        public DbSet<GL01History> GL01History { get; set; }
        
        // 🆕 DbSets cho các bảng SCD Type 2 mới
        public DbSet<LN03History> LN03History { get; set; }
        public DbSet<EI01History> EI01History { get; set; }
        public DbSet<DPDAHistory> DPDAHistory { get; set; }
        public DbSet<DB01History> DB01History { get; set; }
        public DbSet<KH03History> KH03History { get; set; }
        public DbSet<BC57History> BC57History { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình khóa chính phức hợp cho EmployeeRole
            modelBuilder.Entity<EmployeeRole>()
                .HasKey(er => new { er.EmployeeId, er.RoleId });

            // Cấu hình mối quan hệ nhiều-nhiều giữa Employee và Role qua EmployeeRole
            modelBuilder.Entity<EmployeeRole>()
                .HasOne(er => er.Employee)
                .WithMany(e => e.EmployeeRoles)
                .HasForeignKey(er => er.EmployeeId);

            modelBuilder.Entity<EmployeeRole>()
                .HasOne(er => er.Role)
                .WithMany(r => r.EmployeeRoles)
                .HasForeignKey(er => er.RoleId);

            // Cấu hình mối quan hệ tự tham chiếu của Unit (Đơn vị cha - Đơn vị con)
            modelBuilder.Entity<Unit>()
                .HasOne(u => u.ParentUnit)
                .WithMany(u => u.ChildUnits)
                .HasForeignKey(u => u.ParentUnitId)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn chặn việc xóa đệ quy

            // Cấu hình quan hệ EmployeeKhoanAssignment - Employee - KhoanPeriod
            modelBuilder.Entity<EmployeeKhoanAssignment>()
                .HasOne(e => e.Employee)
                .WithMany()
                .HasForeignKey(e => e.EmployeeId);
            modelBuilder.Entity<EmployeeKhoanAssignment>()
                .HasOne(e => e.KhoanPeriod)
                .WithMany()
                .HasForeignKey(e => e.KhoanPeriodId);
            // Cấu hình quan hệ UnitKhoanAssignment - Unit - KhoanPeriod
            modelBuilder.Entity<UnitKhoanAssignment>()
                .HasOne(u => u.Unit)
                .WithMany()
                .HasForeignKey(u => u.UnitId);
            modelBuilder.Entity<UnitKhoanAssignment>()
                .HasOne(u => u.KhoanPeriod)
                .WithMany()
                .HasForeignKey(u => u.KhoanPeriodId);

            // Thêm các cấu hình Fluent API khác ở đây nếu cần (ví dụ: unique constraints, indexes)
            // modelBuilder.Entity<Employee>()
            //     .HasIndex(e => e.Username)
            //     .IsUnique();
            // modelBuilder.Entity<Employee>()
            //     .HasIndex(e => e.EmployeeCode)
            //     .IsUnique();
            // modelBuilder.Entity<Role>()
            //     .HasIndex(r => r.Name)
            //     .IsUnique();

            // Cấu hình quan hệ cho các bảng nghiệp vụ KPI/lương nếu cần

            // Cấu hình quan hệ cho UnitKpiScoring
            modelBuilder.Entity<UnitKpiScoring>()
                .HasOne(s => s.UnitKhoanAssignment)
                .WithMany()
                .HasForeignKey(s => s.UnitKhoanAssignmentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UnitKpiScoring>()
                .HasOne(s => s.KhoanPeriod)
                .WithMany()
                .HasForeignKey(s => s.KhoanPeriodId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UnitKpiScoring>()
                .HasOne(s => s.Unit)
                .WithMany()
                .HasForeignKey(s => s.UnitId)
                .OnDelete(DeleteBehavior.NoAction);

            // Cấu hình quan hệ cho UnitKpiScoringDetail
            modelBuilder.Entity<UnitKpiScoringDetail>()
                .HasOne(d => d.UnitKpiScoring)
                .WithMany(s => s.ScoringDetails)
                .HasForeignKey(d => d.UnitKpiScoringId);

            modelBuilder.Entity<UnitKpiScoringDetail>()
                .HasOne(d => d.KpiIndicator)
                .WithMany()
                .HasForeignKey(d => d.KpiIndicatorId);

            // Cấu hình quan hệ cho UnitKpiScoringCriteria
            modelBuilder.Entity<UnitKpiScoringCriteria>()
                .HasOne(c => c.UnitKpiScoring)
                .WithMany(s => s.ScoringCriteria)
                .HasForeignKey(c => c.UnitKpiScoringId);

            // Cấu hình cho KpiScoringRule
            modelBuilder.Entity<KpiScoringRule>(entity =>
            {
                // Tạo index cho việc tìm kiếm nhanh theo tên chỉ tiêu
                entity.HasIndex(e => e.KpiIndicatorName)
                      .HasDatabaseName("IX_KpiScoringRules_IndicatorName");
                      
                // Thiết lập giá trị mặc định cho RuleType
                entity.Property(e => e.RuleType)
                      .HasDefaultValue("COMPLETION_RATE");
                      
                entity.Property(e => e.IsActive)
                      .HasDefaultValue(true);
            });

            // Cấu hình rõ ràng quan hệ của KpiIndicator chỉ với KpiAssignmentTable
            modelBuilder.Entity<KpiIndicator>()
                .HasOne(k => k.Table)
                .WithMany(t => t.Indicators)
                .HasForeignKey(k => k.TableId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Ngăn Entity Framework tự động tạo quan hệ giữa KPIDefinition và KpiIndicator
            modelBuilder.Entity<KPIDefinition>()
                .Ignore(k => k.KpiIndicators);
        }
    }
}