using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models; // Đảm bảo namespace này đúng với nơi Sếp đặt các Model
using TinhKhoanApp.Api.Models.RawData; // Thêm namespace cho Raw Data models
using TinhKhoanApp.Api.Models.Temporal; // Thêm namespace cho Temporal models
using TinhKhoanApp.Api.Models.Dashboard; // Thêm namespace cho Dashboard models

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
        // Temporarily commented out while using ImportedDataRecords
        // public DbSet<Models.Temporal.RawDataImport> RawDataImports { get; set; }
        public DbSet<Models.Temporal.OptimizedRawDataImport> OptimizedRawDataImports { get; set; }
        public DbSet<RawDataImportArchive> RawDataImportArchives { get; set; }
        public DbSet<Models.Temporal.ImportLog> ImportLogs { get; set; }

        // 📊 DbSets cho hệ thống SCD Type 2 History Tables
        public DbSet<LN01History> LN01History { get; set; }
        public DbSet<GL01History> GL01History { get; set; }

        // 🆕 DbSets cho các bảng History với tên cột CSV gốc
        public DbSet<LN01_History> LN01_History { get; set; }

        // 🆕 DbSets cho các bảng SCD Type 2 mới
        public DbSet<LN03History> LN03History { get; set; }
        public DbSet<EI01History> EI01History { get; set; }
        public DbSet<DPDAHistory> DPDAHistory { get; set; }
        public DbSet<DB01History> DB01History { get; set; }
        public DbSet<KH03History> KH03History { get; set; }
        public DbSet<BC57History> BC57History { get; set; }

        // 🚀 DbSets cho các bảng còn thiếu temporal tables
        public DbSet<DT_KHKD1_History> DT_KHKD1_History { get; set; }
        public DbSet<GAHR26_History> GAHR26_History { get; set; }
        public DbSet<GL41_History> GL41_History { get; set; }

        // 💰 DbSets cho 3 bảng dữ liệu thô mới với Temporal Tables
        public DbSet<ThuXLRR> ThuXLRR { get; set; }
        public DbSet<MSIT72_TSBD> MSIT72_TSBD { get; set; }
        public DbSet<MSIT72_TSGH> MSIT72_TSGH { get; set; }

        // 📊 DbSets cho hệ thống Dashboard Kế hoạch Kinh doanh
        public DbSet<DashboardIndicator> DashboardIndicators { get; set; }
        public DbSet<BusinessPlanTarget> BusinessPlanTargets { get; set; }
        public DbSet<DashboardCalculation> DashboardCalculations { get; set; }

        // 💰 DbSet cho bảng DP01 - Dữ liệu báo cáo tài chính theo ngày
        public DbSet<DP01> DP01s { get; set; }

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

            // === DECIMAL PRECISION CONFIGURATION ===
            // Fix all decimal property precision warnings

            // EmployeeKhoanAssignmentDetail
            modelBuilder.Entity<EmployeeKhoanAssignmentDetail>(entity =>
            {
                entity.Property(e => e.ActualValue).HasPrecision(18, 2);
                entity.Property(e => e.Score).HasPrecision(18, 2);
                entity.Property(e => e.TargetValue).HasPrecision(18, 2);
            });

            // EmployeeKpiTarget
            modelBuilder.Entity<EmployeeKpiTarget>(entity =>
            {
                entity.Property(e => e.ActualValue).HasPrecision(18, 2);
                entity.Property(e => e.Score).HasPrecision(18, 2);
                entity.Property(e => e.TargetValue).HasPrecision(18, 2);
            });

            // FinalPayout
            modelBuilder.Entity<FinalPayout>(entity =>
            {
                entity.Property(e => e.CompletionFactor).HasPrecision(18, 4);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.Property(e => e.V1).HasPrecision(18, 2);
                entity.Property(e => e.V2).HasPrecision(18, 2);
            });

            // KPIDefinition
            modelBuilder.Entity<KPIDefinition>(entity =>
            {
                entity.Property(e => e.MaxScore).HasPrecision(18, 2);
            });

            // KpiIndicator
            modelBuilder.Entity<KpiIndicator>(entity =>
            {
                entity.Property(e => e.MaxScore).HasPrecision(18, 2);
            });

            // KpiScoringRule
            modelBuilder.Entity<KpiScoringRule>(entity =>
            {
                entity.Property(e => e.BonusPoints).HasPrecision(18, 2);
                entity.Property(e => e.MaxValue).HasPrecision(18, 2);
                entity.Property(e => e.MinValue).HasPrecision(18, 2);
                entity.Property(e => e.PenaltyPoints).HasPrecision(18, 2);
            });

            // SalaryParameter
            modelBuilder.Entity<SalaryParameter>(entity =>
            {
                entity.Property(e => e.Value).HasPrecision(18, 2);
            });

            // TransactionAdjustmentFactor
            modelBuilder.Entity<TransactionAdjustmentFactor>(entity =>
            {
                entity.Property(e => e.Factor).HasPrecision(18, 4);
            });

            // UnitKhoanAssignmentDetail
            modelBuilder.Entity<UnitKhoanAssignmentDetail>(entity =>
            {
                entity.Property(e => e.ActualValue).HasPrecision(18, 2);
                entity.Property(e => e.Score).HasPrecision(18, 2);
                entity.Property(e => e.TargetValue).HasPrecision(18, 2);
            });

            // === DASHBOARD CONFIGURATION ===

            // Cấu hình DashboardIndicator
            modelBuilder.Entity<DashboardIndicator>(entity =>
            {
                entity.HasIndex(d => d.Code).IsUnique();
                entity.Property(d => d.CreatedDate).HasDefaultValueSql("GETDATE()");
            });

            // Cấu hình BusinessPlanTarget
            modelBuilder.Entity<BusinessPlanTarget>(entity =>
            {
                // Unique constraint: một đơn vị chỉ có một kế hoạch cho một chỉ tiêu trong một kỳ
                entity.HasIndex(b => new { b.DashboardIndicatorId, b.UnitId, b.Year, b.Quarter, b.Month })
                      .IsUnique()
                      .HasDatabaseName("IX_BusinessPlanTarget_Unique");

                entity.Property(b => b.TargetValue).HasPrecision(18, 2);
                entity.Property(b => b.CreatedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(b => b.Status).HasDefaultValue("Draft");

                // Quan hệ với DashboardIndicator
                entity.HasOne(b => b.DashboardIndicator)
                      .WithMany()
                      .HasForeignKey(b => b.DashboardIndicatorId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Quan hệ với Unit
                entity.HasOne(b => b.Unit)
                      .WithMany()
                      .HasForeignKey(b => b.UnitId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Cấu hình DashboardCalculation
            modelBuilder.Entity<DashboardCalculation>(entity =>
            {
                // Unique constraint: một đơn vị chỉ có một kết quả tính toán cho một chỉ tiêu trong một ngày
                entity.HasIndex(d => new { d.DashboardIndicatorId, d.UnitId, d.CalculationDate })
                      .IsUnique()
                      .HasDatabaseName("IX_DashboardCalculation_Unique");

                entity.Property(d => d.ActualValue).HasPrecision(18, 2);
                entity.Property(d => d.CreatedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(d => d.Status).HasDefaultValue("Success");

                // Quan hệ với DashboardIndicator
                entity.HasOne(d => d.DashboardIndicator)
                      .WithMany()
                      .HasForeignKey(d => d.DashboardIndicatorId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Quan hệ với Unit
                entity.HasOne(d => d.Unit)
                      .WithMany()
                      .HasForeignKey(d => d.UnitId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 🚀 === TEMPORAL TABLES + COLUMNSTORE INDEXES CONFIGURATION ===

            // 📊 Cấu hình Temporal Tables cho ImportedDataRecord với history tracking
            // ✅ Đã fix các vấn đề compression columns, bật lại temporal tables
            modelBuilder.Entity<ImportedDataRecord>(entity =>
            {
                // Bật Temporal Table với shadow properties để tracking lịch sử thay đổi
                entity.ToTable(tb => tb.IsTemporal(ttb =>
                {
                    ttb.UseHistoryTable("ImportedDataRecords_History");
                    ttb.HasPeriodStart("SysStartTime").HasColumnName("SysStartTime");
                    ttb.HasPeriodEnd("SysEndTime").HasColumnName("SysEndTime");
                }));

                // ⚠️ QUAN TRỌNG: Định nghĩa shadow properties cho temporal columns
                entity.Property<DateTime>("SysStartTime").HasColumnName("SysStartTime");
                entity.Property<DateTime>("SysEndTime").HasColumnName("SysEndTime");

                // Indexes for performance theo chuẩn Columnstore
                entity.HasIndex(e => e.StatementDate)
                      .HasDatabaseName("IX_ImportedDataRecords_StatementDate");

                entity.HasIndex(e => new { e.Category, e.ImportDate })
                      .HasDatabaseName("IX_ImportedDataRecords_Category_ImportDate");

                entity.HasIndex(e => e.Status)
                      .HasDatabaseName("IX_ImportedDataRecords_Status");

                // Bổ sung index cho temporal table queries
                entity.HasIndex(e => e.ImportDate)
                      .HasDatabaseName("IX_ImportedDataRecords_ImportDate");
            });

            // 📈 Cấu hình Temporal Tables cho ImportedDataItem với Columnstore Index
            modelBuilder.Entity<ImportedDataItem>(entity =>
            {
                // Enable Temporal Table với shadow properties cho Big Data Analytics
                entity.ToTable(tb => tb.IsTemporal(ttb =>
                {
                    ttb.UseHistoryTable("ImportedDataItems_History");
                    ttb.HasPeriodStart("SysStartTime").HasColumnName("SysStartTime");
                    ttb.HasPeriodEnd("SysEndTime").HasColumnName("SysEndTime");
                }));

                // ⚠️ QUAN TRỌNG: Định nghĩa shadow properties cho temporal columns
                entity.Property<DateTime>("SysStartTime").HasColumnName("SysStartTime");
                entity.Property<DateTime>("SysEndTime").HasColumnName("SysEndTime");

                // Indexes cho analytics performance với Columnstore optimization
                entity.HasIndex(e => e.ProcessedDate)
                      .HasDatabaseName("IX_ImportedDataItems_ProcessedDate");

                entity.HasIndex(e => e.ImportedDataRecordId)
                      .HasDatabaseName("IX_ImportedDataItems_RecordId");

                // Index kết hợp cho temporal queries
                entity.HasIndex(e => new { e.ImportedDataRecordId, e.ProcessedDate })
                      .HasDatabaseName("IX_ImportedDataItems_Record_Date");

                // JSON indexing (SQL Server 2016+) cho RawData
                entity.Property(e => e.RawData)
                      .HasColumnType("nvarchar(max)");
            });

            // 🎯 Custom SQL để tạo Columnstore Index (sẽ chạy qua migration)
            // Columnstore Index cho analytics performance trên ImportedDataItems và History
            // Em sẽ tạo migration riêng để:
            // 1. CREATE NONCLUSTERED COLUMNSTORE INDEX IX_ImportedDataItems_Columnstore
            //    ON ImportedDataItems (ImportedDataRecordId, ProcessedDate, RawData)
            //    WHERE ProcessedDate >= '2024-01-01'
            //
            // 2. CREATE NONCLUSTERED COLUMNSTORE INDEX IX_ImportedDataItems_History_Columnstore
            //    ON ImportedDataItems_History (ImportedDataRecordId, ProcessedDate, RawData, SysStartTime, SysEndTime)
            //    WHERE ProcessedDate >= '2024-01-01'
            //
            // 3. CREATE NONCLUSTERED COLUMNSTORE INDEX IX_ImportedDataRecords_History_Columnstore
            //    ON ImportedDataRecords_History (Category, ImportDate, StatementDate, Status, SysStartTime, SysEndTime)
            //    WHERE ImportDate >= '2024-01-01'

            // 🚀 === CẤU HÌNH TEMPORAL TABLES VỚI TÊN CỘT CSV GỐC ===
            // Sử dụng History models cho temporal configuration nhưng đảm bảo main table có tên cột đúng

            // Cấu hình tên cột CSV gốc cho các bảng
            ConfigureMainTableWithOriginalColumns(modelBuilder);

            // Cấu hình temporal table cho KH03 (chỉ thiếu temporal table)
            ConfigureTemporalTable<KH03History>(modelBuilder, "KH03", "KH03_History");

            // Đảm bảo các bảng đã có cũng được cấu hình đúng
            ConfigureTemporalTable<DPDAHistory>(modelBuilder, "DPDA", "DPDA_History");
            ConfigureTemporalTable<EI01History>(modelBuilder, "EI01", "EI01_History");
        }

        // 🔧 Helper method để cấu hình Temporal Table
        private void ConfigureTemporalTable<T>(ModelBuilder modelBuilder, string tableName, string historyTableName) where T : class
        {
            modelBuilder.Entity<T>(entity =>
            {
                // Cấu hình bảng thành Temporal Table với shadow properties
                entity.ToTable(tableName, b => b.IsTemporal(ttb =>
                {
                    ttb.HasPeriodStart("SysStartTime").HasColumnName("SysStartTime");
                    ttb.HasPeriodEnd("SysEndTime").HasColumnName("SysEndTime");
                    ttb.UseHistoryTable(historyTableName);
                }));

                // Thêm shadow properties cho temporal columns (không được có default value)
                entity.Property<DateTime>("SysStartTime").HasColumnName("SysStartTime");
                entity.Property<DateTime>("SysEndTime").HasColumnName("SysEndTime");

                // Indexes for performance theo chuẩn Columnstore (kiểm tra property tồn tại)
                var entityType = typeof(T);
                if (entityType.GetProperty("StatementDate") != null)
                {
                    entity.HasIndex("StatementDate")
                          .HasDatabaseName($"IX_{tableName}_StatementDate");
                }

                if (entityType.GetProperty("ProcessedDate") != null)
                {
                    entity.HasIndex("ProcessedDate")
                          .HasDatabaseName($"IX_{tableName}_ProcessedDate");
                }

                if (entityType.GetProperty("IsCurrent") != null)
                {
                    entity.HasIndex("IsCurrent")
                          .HasDatabaseName($"IX_{tableName}_IsCurrent");
                }
            });
        }

        /// <summary>
        /// Cấu hình các bảng chính với tên cột CSV gốc
        /// </summary>
        private void ConfigureMainTableWithOriginalColumns(ModelBuilder modelBuilder)
        {
            // Tạm thời comment out vì cần xem lại cấu hình temporal table
            // Thay vào đó, đảm bảo History models có tên cột chính xác            // 🏦 Cấu hình cho bảng DP01 - Dữ liệu báo cáo tài chính
            modelBuilder.Entity<DP01>(entity =>
            {
                entity.ToTable("DP01");

                // Cấu hình precision cho CURRENT_BALANCE (18,2 để lưu số tiền lớn)
                entity.Property(e => e.CURRENT_BALANCE)
                    .HasPrecision(18, 2);

                // Tạo clustered columnstore index cho hiệu suất cao
                entity.HasIndex(e => new { e.DATA_DATE, e.MA_CN, e.MA_PGD })
                    .HasDatabaseName("IX_DP01_DateBranchPGD_Clustered");

                // Index cho tài khoản hạch toán để filter nhanh
                entity.HasIndex(e => e.TAI_KHOAN_HACH_TOAN)
                    .HasDatabaseName("IX_DP01_Account");

                // Index cho mã chi nhánh
                entity.HasIndex(e => e.MA_CN)
                    .HasDatabaseName("IX_DP01_Branch");
            });
        }
    }
}
