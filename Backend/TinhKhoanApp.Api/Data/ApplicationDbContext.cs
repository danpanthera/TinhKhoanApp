using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models; // Đảm bảo namespace này đúng với nơi Sếp đặt các Model
using TinhKhoanApp.Api.Models.RawData; // Thêm namespace cho Raw Data models
using TinhKhoanApp.Api.Models.Temporal; // Thêm namespace cho Temporal models
using TinhKhoanApp.Api.Models.Dashboard; // Thêm namespace cho Dashboard models
using DataTables = TinhKhoanApp.Api.Models.DataTables; // Alias để tránh conflict

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
        // ✅ CLEANED: Removed legacy ImportedDataItem - Using DirectImportService workflow only

        // 🚀 DbSets cho 8 bảng dữ liệu thô chính (DirectImport với Temporal Tables + Columnstore)
        public DbSet<DataTables.DP01> DP01 { get; set; } // Re-enabled for basic access
        public DbSet<DataTables.LN01> LN01 { get; set; }
        public DbSet<DataTables.LN03> LN03 { get; set; }
        public DbSet<DataTables.GL01> GL01 { get; set; }
        public DbSet<DataTables.GL02> GL02 { get; set; }
        public DbSet<DataTables.GL41> GL41 { get; set; }
        public DbSet<DataTables.DPDA> DPDA { get; set; }
        public DbSet<DataTables.EI01> EI01 { get; set; }
        public DbSet<DataTables.RR01> RR01 { get; set; }

        // 🔄 DbSets with plural names for backward compatibility
        // Note: DP01s removed - using DP01 directly
        public DbSet<DataTables.LN01> LN01s { get; set; }
        public DbSet<DataTables.LN03> LN03s { get; set; }
        public DbSet<DataTables.GL01> GL01s { get; set; }
        public DbSet<DataTables.GL02> GL02s { get; set; }
        public DbSet<DataTables.GL41> GL41s { get; set; }
        public DbSet<DataTables.DPDA> DPDAs { get; set; }
        public DbSet<DataTables.EI01> EI01s { get; set; }
        public DbSet<DataTables.RR01> RR01s { get; set; }

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

        // 🚀 DbSets cho các bảng còn thiếu temporal tables
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình explicit cho Employee model
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IsActive)
                    .HasColumnName("IsActive")
                    .HasColumnType("bit")
                    .IsRequired()
                    .HasDefaultValue(true);
            });

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

            // === DECIMAL PRECISION CONFIGURATION FOR DATA TABLES ===
            // Fix specific decimal property precision warnings for the 12 data tables

            // DP01 decimal properties + Temporal Table - ENABLED
            modelBuilder.Entity<DataTables.DP01>(entity =>
            {
                // Decimal precision for AMOUNT/BALANCE columns
                entity.Property(e => e.CURRENT_BALANCE).HasPrecision(18, 2);
                entity.Property(e => e.RATE).HasPrecision(18, 6);
                entity.Property(e => e.ACRUAL_AMOUNT).HasPrecision(18, 2);
                entity.Property(e => e.ACRUAL_AMOUNT_END).HasPrecision(18, 2);
                entity.Property(e => e.DRAMT).HasPrecision(18, 2);
                entity.Property(e => e.CRAMT).HasPrecision(18, 2);
                entity.Property(e => e.SPECIAL_RATE).HasPrecision(18, 6);
                entity.Property(e => e.TYGIA).HasPrecision(18, 6);

                // Temporal Table configuration
                entity.ToTable(tb => tb.IsTemporal(ttb =>
                {
                    ttb.UseHistoryTable("DP01_History");
                    ttb.HasPeriodStart("ValidFrom");
                    ttb.HasPeriodEnd("ValidTo");
                }));

                // Create columnstore index for analytics
                entity.HasIndex(e => new { e.NGAY_DL, e.MA_CN, e.CURRENT_BALANCE, e.MA_PGD })
                    .HasDatabaseName("NCCI_DP01_Analytics")
                    .IsUnique(false);
            });

            // GL01 decimal properties
            modelBuilder.Entity<DataTables.GL01>(entity =>
            {
                entity.Property(e => e.SO_TIEN_GD).HasPrecision(18, 2);
                entity.Property(e => e.TR_EX_RT).HasPrecision(18, 6);
            });

            // GL02 decimal properties
            modelBuilder.Entity<DataTables.GL02>(entity =>
            {
                entity.Property(e => e.DRAMOUNT).HasPrecision(18, 2);
                entity.Property(e => e.CRAMOUNT).HasPrecision(18, 2);
            });

            // GL41 decimal properties - Updated for new 13-column structure
            modelBuilder.Entity<DataTables.GL41>(entity =>
            {
                entity.Property(e => e.DN_DAUKY).HasPrecision(18, 2);
                entity.Property(e => e.DC_DAUKY).HasPrecision(18, 2);
                entity.Property(e => e.SBT_NO).HasPrecision(18, 2);
                entity.Property(e => e.ST_GHINO).HasPrecision(18, 2);
                entity.Property(e => e.SBT_CO).HasPrecision(18, 2);
                entity.Property(e => e.ST_GHICO).HasPrecision(18, 2);
                entity.Property(e => e.DN_CUOIKY).HasPrecision(18, 2);
                entity.Property(e => e.DC_CUOIKY).HasPrecision(18, 2);
            });

            // 🚀 === TEMPORAL TABLES + COLUMNSTORE INDEXES CONFIGURATION ===

            // 📊 Cấu hình Temporal Tables cho ImportedDataRecords với history tracking
            // ✅ Đã fix các vấn đề compression columns, bật lại temporal tables
            // TODO: Enable temporal table sau khi hệ thống ổn định hoàn toàn
            // modelBuilder.Entity<ImportedDataRecord>(entity =>
            // {
            //     entity.ToTable(tb => tb.IsTemporal(ttb =>
            //     {
            //         ttb.UseHistoryTable("ImportedDataRecords_History");
            //         ttb.HasPeriodStart("SysStartTime").HasColumnName("SysStartTime");
            //         ttb.HasPeriodEnd("SysEndTime").HasColumnName("SysEndTime");
            //     }));
            //
            //     entity.Property<DateTime>("SysStartTime").HasColumnName("SysStartTime");
            //     entity.Property<DateTime>("SysEndTime").HasColumnName("SysEndTime");
            //
            //     entity.HasIndex(e => e.StatementDate).HasDatabaseName("IX_ImportedDataRecords_StatementDate");
            //     entity.HasIndex(e => new { e.Category, e.ImportDate }).HasDatabaseName("IX_ImportedDataRecords_Category_ImportDate");
            //     entity.HasIndex(e => e.Status).HasDatabaseName("IX_ImportedDataRecords_Status");
            //     entity.HasIndex(e => e.ImportDate).HasDatabaseName("IX_ImportedDataRecords_ImportDate");
            // });            // ✅ CLEANED: Removed legacy ImportedDataItem configuration - Direct Import only

            // 🎯 Custom SQL để tạo Columnstore Index (sẽ chạy qua migration)
            // ✅ CLEANED: Direct Import workflow - data stored in specific tables with optimized indexes
            // Direct Import workflow stores data directly in specific tables with their own indexes

            // 🚀 === CẤU HÌNH TEMPORAL TABLES VỚI TÊN CỘT CSV GỐC ===
            // Sử dụng History models cho temporal configuration nhưng đảm bảo main table có tên cột đúng

            // Cấu hình tên cột CSV gốc cho các bảng
            ConfigureMainTableWithOriginalColumns(modelBuilder);



            // Đảm bảo các bảng đã có cũng được cấu hình đúng - COMMENT TẠM THỜI ĐỂ TRÁNH CONFLICT
            // ConfigureTemporalTable<DPDAHistory>(modelBuilder, "DPDA", "DPDA_History");
            // ConfigureTemporalTable<EI01History>(modelBuilder, "EI01", "EI01_History");

            // 🆕 Cấu hình Temporal Tables cho các bảng dữ liệu mới
            // Mỗi bảng sẽ có Temporal Tables + Columnstore Indexes tự động
            ConfigureNewDataTables(modelBuilder);
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
            // Cấu hình bảng DP01 với cấu trúc temporal table + columnstore
            ConfigureDataTableWithTemporal<DataTables.DP01>(modelBuilder, "DP01");
        }

        /// <summary>
        /// Cấu hình Temporal Tables + Columnstore Indexes cho các bảng dữ liệu mới
        /// Theo quy ước: mỗi loại file sẽ có bảng riêng với đầy đủ Temporal capabilities
        /// </summary>
        private void ConfigureNewDataTables(ModelBuilder modelBuilder)
        {
            // 💰 Cấu hình bảng DPDA - Tiền gửi của dân
            ConfigureDataTableWithTemporal<DataTables.DPDA>(modelBuilder, "DPDA");

            // 📊 Cấu hình bảng EI01 - Thu nhập khác
            ConfigureDataTableWithTemporal<DataTables.EI01>(modelBuilder, "EI01");

            // 📋 Cấu hình bảng GL01 - Sổ cái tổng hợp (Partitioned Columnstore - NOT Temporal)
            ConfigureDataTableBasic<DataTables.GL01>(modelBuilder, "GL01");

            // � Cấu hình bảng GL02 - Giao dịch sổ cái (Partitioned Columnstore - NOT Temporal)
            ConfigureDataTableBasic<DataTables.GL02>(modelBuilder, "GL02");

            // �📊 Cấu hình bảng GL41 - Số dư sổ cái
            ConfigureDataTableWithTemporal<DataTables.GL41>(modelBuilder, "GL41");

            // 🏷️ Cấu hình bảng LN01 - Cho vay
            ConfigureDataTableWithTemporal<DataTables.LN01>(modelBuilder, "LN01");

            // ⚠️ Cấu hình bảng LN03 - Nợ xấu
            ConfigureDataTableWithTemporal<DataTables.LN03>(modelBuilder, "LN03");

            // 📈 Cấu hình bảng RR01 - Tỷ lệ
            ConfigureDataTableWithTemporal<DataTables.RR01>(modelBuilder, "RR01");
        }

        /// <summary>
        /// Cấu hình cơ bản cho bảng dữ liệu (không temporal)
        /// </summary>
        private void ConfigureDataTableBasic<T>(ModelBuilder modelBuilder, string tableName) where T : class
        {
            modelBuilder.Entity<T>(entity =>
            {
                // Cấu hình tên bảng
                entity.ToTable(tableName);

                // Cấu hình precision cho các trường tiền tệ
                foreach (var property in typeof(T).GetProperties())
                {
                    if (property.PropertyType == typeof(decimal?) || property.PropertyType == typeof(decimal))
                    {
                        var propertyName = property.Name;
                        if (propertyName.Contains("TIEN") || propertyName.Contains("DU_NO") ||
                            propertyName.Contains("GIA_TRI") || propertyName.Contains("BALANCE") ||
                            propertyName.Contains("PLAN_") || propertyName.Contains("ACTUAL_"))
                        {
                            entity.Property(propertyName).HasPrecision(18, 2);
                        }
                        else if (propertyName.Contains("LAI_SUAT") || propertyName.Contains("TY_LE"))
                        {
                            entity.Property(propertyName).HasPrecision(10, 6);
                        }
                        else if (propertyName.Equals("ACHIEVEMENT_RATE"))
                        {
                            entity.Property(propertyName).HasPrecision(18, 2);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Cấu hình Temporal Table + Columnstore Index cho một bảng dữ liệu
        /// </summary>
        private void ConfigureDataTableWithTemporal<T>(ModelBuilder modelBuilder, string tableName) where T : class
        {
            modelBuilder.Entity<T>(entity =>
            {
                // Cấu hình bảng thành Temporal Table với explicit ValidFrom/ValidTo columns
                entity.ToTable(tableName, tb => tb.IsTemporal(ttb =>
                {
                    ttb.HasPeriodStart("ValidFrom").HasColumnName("ValidFrom");
                    ttb.HasPeriodEnd("ValidTo").HasColumnName("ValidTo");
                    ttb.UseHistoryTable($"{tableName}_History");
                }));

                // Indexes tối ưu cho báo cáo và truy vấn
                var entityType = typeof(T);

                // Index cho NGAY_DL (tất cả bảng đều có)
                entity.HasIndex("NGAY_DL")
                    .HasDatabaseName($"IX_{tableName}_NGAY_DL");

                // Index cho MA_CN (tất cả bảng đều có)
                if (entityType.GetProperty("MA_CN") != null)
                {
                    entity.HasIndex("MA_CN")
                        .HasDatabaseName($"IX_{tableName}_MaCN");
                }

                // Index kết hợp cho performance tốt nhất
                if (entityType.GetProperty("MA_CN") != null)
                {
                    entity.HasIndex(new[] { "NGAY_DL", "MA_CN" })
                        .HasDatabaseName($"IX_{tableName}_NGAY_DL_MaCN");
                }

                // Index cho MA_PGD nếu có
                if (entityType.GetProperty("MA_PGD") != null)
                {
                    entity.HasIndex("MA_PGD")
                        .HasDatabaseName($"IX_{tableName}_MaPGD");
                }

                // Cấu hình precision cho các trường tiền tệ
                foreach (var property in entityType.GetProperties())
                {
                    if (property.PropertyType == typeof(decimal?) || property.PropertyType == typeof(decimal))
                    {
                        var propertyName = property.Name;
                        if (propertyName.Contains("TIEN") || propertyName.Contains("DU_NO") ||
                            propertyName.Contains("GIA_TRI") || propertyName.Contains("BALANCE") ||
                            propertyName.Contains("PLAN_") || propertyName.Contains("ACTUAL_") ||
                            propertyName.Contains("AMOUNT") || propertyName.Contains("APPRAMT") ||
                            propertyName.Contains("DISBURSEMENT") || propertyName.Contains("REPAYMENT") ||
                            propertyName.Contains("INTEREST") || propertyName.Contains("EXEMPT") ||
                            propertyName.Contains("INTCMTH") || propertyName.Contains("INTRPYMTH") ||
                            propertyName.Contains("ACCRUAL") || propertyName.Contains("PASTDUE") ||
                            propertyName.Contains("TOTAL_") || propertyName.Contains("NEXT_") ||
                            propertyName.Contains("BDS") || propertyName.Contains("DS") ||
                            propertyName.Contains("DUNO") || propertyName.Contains("THU_") ||
                            propertyName.Contains("TSK") || propertyName.Contains("DOC_") ||
                            propertyName.Contains("CONLAINGOAIBANG") || propertyName.Contains("DUNONOIBANG") ||
                            propertyName.Contains("THUNOSAUXL"))
                        {
                            entity.Property(propertyName).HasPrecision(18, 2);
                        }
                        else if (propertyName.Contains("LAI_SUAT") || propertyName.Contains("TY_LE") ||
                                propertyName.Contains("RATE") || propertyName.Contains("PERCENT") ||
                                propertyName.Contains("TY_GIA"))
                        {
                            entity.Property(propertyName).HasPrecision(10, 6);
                        }
                        else if (propertyName.Equals("ACHIEVEMENT_RATE"))
                        {
                            entity.Property(propertyName).HasPrecision(18, 2);
                        }
                        else
                        {
                            // Default precision for other decimal fields
                            entity.Property(propertyName).HasPrecision(18, 2);
                        }
                    }
                }
            });
        }
    }
}
