using TinhKhoanApp.Api.Models;
using TinhKhoanApp.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace TinhKhoanApp.Api.Data
{
    /// <summary>
    /// Seeder tạo 23 bảng KPI chuẩn cho các vai trò cán bộ + 9 bảng cho chi nhánh
    /// Sử dụng definitions từ SeedKPIDefinitionMaxScore.cs để đảm bảo tính nhất quán
    /// </summary>
    public static class KpiAssignmentTableSeeder
    {
        // Mapping từ KpiTableType enum sang RoleCode string (chuẩn hóa theo SeedKPIDefinitionMaxScore.cs)
        public static readonly Dictionary<KpiTableType, string> TableTypeToRoleCodeMapping = new()
        {
            // Vai trò cán bộ (23 loại) - theo SeedKPIDefinitionMaxScore.cs
            { KpiTableType.TruongphongKhdn, "TruongphongKhdn" },
            { KpiTableType.TruongphongKhcn, "TruongphongKhcn" },
            { KpiTableType.PhophongKhdn, "PhophongKhdn" },
            { KpiTableType.PhophongKhcn, "PhophongKhcn" },
            { KpiTableType.TruongphongKhqlrr, "TruongphongKhqlrr" },
            { KpiTableType.PhophongKhqlrr, "PhophongKhqlrr" },
            { KpiTableType.Cbtd, "Cbtd" },
            { KpiTableType.TruongphongKtnqCnl1, "TruongphongKtnqCnl1" },
            { KpiTableType.PhophongKtnqCnl1, "PhophongKtnqCnl1" },
            { KpiTableType.Gdv, "Gdv" },
            { KpiTableType.TqHkKtnb, "TqHkKtnb" },
            { KpiTableType.TruongphoItThKtgs, "TruongphongItThKtgs" },
            { KpiTableType.CBItThKtgsKhqlrr, "CbItThKtgsKhqlrr" },
            { KpiTableType.GiamdocPgd, "GiamdocPgd" },
            { KpiTableType.PhogiamdocPgd, "PhogiamdocPgd" },
            { KpiTableType.PhogiamdocPgdCbtd, "PhogiamdocPgdCbtd" },
            { KpiTableType.GiamdocCnl2, "GiamdocCnl2" },
            { KpiTableType.PhogiamdocCnl2Td, "PhogiamdocCnl2Td" },
            { KpiTableType.PhogiamdocCnl2Kt, "PhogiamdocCnl2Kt" },
            { KpiTableType.TruongphongKhCnl2, "TruongphongKhCnl2" },
            { KpiTableType.PhophongKhCnl2, "PhophongKhCnl2" },
            { KpiTableType.TruongphongKtnqCnl2, "TruongphongKtnqCnl2" },
            { KpiTableType.PhophongKtnqCnl2, "PhophongKtnqCnl2" },
            // Chi nhánh (9 chi nhánh) - sẽ có logic riêng
            { KpiTableType.HoiSo, "HoiSo" },
            { KpiTableType.CnTamDuong, "CnBinhLu" },
            { KpiTableType.CnPhongTho, "CnPhongTho" },
            { KpiTableType.CnSinHo, "CnSinHo" },
            { KpiTableType.CnMuongTe, "CnBumTo" },
            { KpiTableType.CnThanUyen, "CnThanUyen" },
            { KpiTableType.CnThanhPho, "CnDoanKet" },
            { KpiTableType.CnTanUyen, "CnTanUyen" },
            { KpiTableType.CnNamNhun, "CnNamHang" }
        };

        // Mapping từ KpiTableType sang role descriptions chuẩn
        public static readonly Dictionary<KpiTableType, string> TableTypeToDescriptionMapping = new()
        {
            // Vai trò cán bộ - descriptions từ SeedKPIDefinitionMaxScore.cs
            { KpiTableType.TruongphongKhdn, "Trưởng phòng KHDN" },
            { KpiTableType.TruongphongKhcn, "Trưởng phòng KHCN" },
            { KpiTableType.PhophongKhdn, "Phó phòng KHDN" },
            { KpiTableType.PhophongKhcn, "Phó phòng KHCN" },
            { KpiTableType.TruongphongKhqlrr, "Trưởng phòng Kế hoạch và Quản lý rủi ro" },
            { KpiTableType.PhophongKhqlrr, "Phó phòng Kế hoạch và Quản lý rủi ro" },
            { KpiTableType.Cbtd, "Cán bộ tín dụng" },
            { KpiTableType.TruongphongKtnqCnl1, "Trưởng phòng Kế toán & Ngân quỹ CNL1" },
            { KpiTableType.PhophongKtnqCnl1, "Phó phòng Kế toán & Ngân quỹ CNL1" },
            { KpiTableType.Gdv, "Giao dịch viên" },
            { KpiTableType.TqHkKtnb, "TQ/Hậu kiểm/Kế toán nội bộ" },
            { KpiTableType.TruongphoItThKtgs, "Trưởng phó phòng IT/TH/KTGS" },
            { KpiTableType.CBItThKtgsKhqlrr, "CB IT/TH/KTGS/KHQLRR" },
            { KpiTableType.GiamdocPgd, "Giám đốc PGD" },
            { KpiTableType.PhogiamdocPgd, "Phó giám đốc PGD" },
            { KpiTableType.PhogiamdocPgdCbtd, "Phó giám đốc PGD kiêm CBTD" },
            { KpiTableType.GiamdocCnl2, "Giám đốc CNL2" },
            { KpiTableType.PhogiamdocCnl2Td, "Phó giám đốc CNL2 phụ trách Tín dụng" },
            { KpiTableType.PhogiamdocCnl2Kt, "Phó giám đốc CNL2 Phụ trách Kế toán" },
            { KpiTableType.TruongphongKhCnl2, "Trưởng phòng KH CNL2" },
            { KpiTableType.PhophongKhCnl2, "Phó phòng KH CNL2" },
            { KpiTableType.TruongphongKtnqCnl2, "Trưởng phòng Kế toán & Ngân quỹ CNL2" },
            { KpiTableType.PhophongKtnqCnl2, "Phó phòng Kế toán & Ngân quỹ CNL2" },
            // Chi nhánh (cập nhật tên mới)
            { KpiTableType.HoiSo, "Hội sở" },
            { KpiTableType.CnTamDuong, "Chi nhánh Bình Lư" },
            { KpiTableType.CnPhongTho, "Chi nhánh Phong Thổ" },
            { KpiTableType.CnSinHo, "Chi nhánh Sin Hồ" },
            { KpiTableType.CnMuongTe, "Chi nhánh Bum Tở" },
            { KpiTableType.CnThanUyen, "Chi nhánh Than Uyên" },
            { KpiTableType.CnThanhPho, "Chi nhánh Đoàn Kết" },
            { KpiTableType.CnTanUyen, "Chi nhánh Tân Uyên" },
            { KpiTableType.CnNamNhun, "Chi nhánh Nậm Hàng" }
        };

        public static void SeedKpiAssignmentTables(ApplicationDbContext context)
        {
            try
            {
                Console.WriteLine("=== Bắt đầu seed 32 bảng KPI Assignment (23 vai trò + 9 chi nhánh) ===");

                // Kiểm tra xem đã có bảng nào chưa
                var existingTables = context.KpiAssignmentTables.Count();
                if (existingTables > 0)
                {
                    Console.WriteLine($"⚠️ Đã có {existingTables} bảng KPI. Bỏ qua seeding để tránh trùng lặp.");
                    return;
                }

                var tablesToCreate = new List<KpiAssignmentTable>();
                var indicatorsToCreate = new List<KpiIndicator>();

                foreach (var mapping in TableTypeToRoleCodeMapping)
                {
                    var tableType = mapping.Key;
                    var roleCode = mapping.Value;
                    var description = TableTypeToDescriptionMapping[tableType];

                    Console.WriteLine($"📋 Tạo bảng KPI cho {description} (TableType: {tableType}, RoleCode: {roleCode})");

                    // Tạo KPI Assignment Table
                    var kpiTable = new KpiAssignmentTable
                    {
                        TableType = tableType,
                        TableName = $"{roleCode}_KPI_Assignment",
                        Description = $"Bảng KPI cho {description}",
                        Category = GetCategoryForTableType(tableType),
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    };

                    tablesToCreate.Add(kpiTable);
                }

                // Thêm tất cả bảng vào context và save để có ID
                context.KpiAssignmentTables.AddRange(tablesToCreate);
                context.SaveChanges();

                Console.WriteLine($"✅ Đã tạo {tablesToCreate.Count} bảng KPI Assignment thành công");

                // Tạo indicators cho từng bảng dựa trên definitions trong bảng KPIDefinitions
                foreach (var kpiTable in tablesToCreate)
                {
                    var indicators = CreateIndicatorsForTableFromDefinitions(context, kpiTable);
                    indicatorsToCreate.AddRange(indicators);
                }

                // Thêm tất cả indicators
                context.KpiIndicators.AddRange(indicatorsToCreate);
                context.SaveChanges();

                Console.WriteLine($"✅ Đã tạo {indicatorsToCreate.Count} KPI Indicators theo definitions chuẩn");
                Console.WriteLine("=== Hoàn thành seed KPI Assignment Tables ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi seed KPI Assignment Tables: {ex.Message}");
                Console.WriteLine($"📍 Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Tạo indicators từ definitions chuẩn trong bảng KPIDefinitions
        /// </summary>
        private static List<KpiIndicator> CreateIndicatorsForTableFromDefinitions(ApplicationDbContext context, KpiAssignmentTable kpiTable)
        {
            var indicators = new List<KpiIndicator>();
            var roleCode = TableTypeToRoleCodeMapping[kpiTable.TableType];

            // Lấy KPI definitions từ database theo roleCode
            var kpiDefinitions = context.KPIDefinitions
                .Where(k => k.KpiCode.StartsWith(roleCode + "_"))
                .OrderBy(k => k.KpiCode)
                .ToList();

            // Nếu không tìm thấy definitions (có thể là chi nhánh), sử dụng template từ GiamdocCnl2
            if (!kpiDefinitions.Any() && IsBranchTableType(kpiTable.TableType))
            {
                Console.WriteLine($"⚠️ Không tìm thấy KPI definitions cho {roleCode}, sử dụng template GiamdocCnl2");
                kpiDefinitions = context.KPIDefinitions
                    .Where(k => k.KpiCode.StartsWith("GiamdocCnl2_"))
                    .OrderBy(k => k.KpiCode)
                    .ToList();
            }

            for (int i = 0; i < kpiDefinitions.Count; i++)
            {
                var definition = kpiDefinitions[i];
                var indicator = new KpiIndicator
                {
                    TableId = kpiTable.Id,
                    IndicatorName = definition.KpiName,
                    MaxScore = definition.MaxScore,
                    Unit = definition.UnitOfMeasure,
                    OrderIndex = i + 1,
                    ValueType = definition.ValueType,
                    IsActive = definition.IsActive
                };

                indicators.Add(indicator);
            }

            Console.WriteLine($"📊 Tạo {indicators.Count} indicators cho {roleCode} từ bảng KPIDefinitions");
            return indicators;
        }

        /// <summary>
        /// Kiểm tra xem table type có phải là chi nhánh không
        /// </summary>
        private static bool IsBranchTableType(KpiTableType tableType)
        {
            return tableType >= KpiTableType.HoiSo && tableType <= KpiTableType.CnNamNhun;
        }

        /// <summary>
        /// Lấy category cho table type
        /// </summary>
        private static string GetCategoryForTableType(KpiTableType tableType)
        {
            return IsBranchTableType(tableType) ? "CHINHANH" : "CANBO";
        }
    }
}
