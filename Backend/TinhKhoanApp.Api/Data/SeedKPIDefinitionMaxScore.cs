using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Data
{
    /// <summary>
    /// Seed các chỉ tiêu KPI cho 23 vai trò cán bộ chuẩn - ĐẦY ĐỦ TẤT CẢ VAI TRÒ
    /// Mỗi vai trò có bảng KPI riêng với các chỉ tiêu và điểm số cụ thể
    /// Phần 1 (Vai trò 1-10): Các vai trò cốt lõi của hệ thống
    /// Phần 2 (Vai trò 11-23): Các vai trò bổ sung và chi nhánh
    /// </summary>
    public static class SeedKPIDefinitionMaxScore
    {
        public static void SeedKPIDefinitions(ApplicationDbContext context)
        {
            Console.WriteLine("🔄 Bắt đầu seed các chỉ tiêu KPI cho 23 vai trò cán bộ...");

            try
            {
                // Xóa dữ liệu cũ (nếu có) để tránh trùng lặp
                if (context.KPIDefinitions.Any())
                {
                    Console.WriteLine("⚠️ Phát hiện dữ liệu KPI cũ, đang xóa để tạo mới...");
                    context.KPIDefinitions.RemoveRange(context.KPIDefinitions);
                    context.SaveChanges();
                }

                // PHẦN 1: Seed 10 vai trò đầu tiên (1-10)
                SeedTruongphongKhdn(context);      // 1. Trưởng phòng KHDN
                SeedTruongphongKhcn(context);      // 2. Trưởng phòng KHCN
                SeedPhophongKhdn(context);         // 3. Phó phòng KHDN
                SeedPhophongKhcn(context);         // 4. Phó phòng KHCN
                SeedTruongphongKhqlrr(context);    // 5. Trưởng phòng KHQLRR
                SeedPhophongKhqlrr(context);       // 6. Phó phòng KHQLRR
                SeedCbtd(context);                 // 7. CBTD
                SeedTruongphongKtnqCnl1(context);  // 8. Trưởng phòng KTNQ CNL1
                SeedPhophongKtnqCnl1(context);     // 9. Phó phòng KTNQ CNL1
                SeedGdv(context);                  // 10. GDV

                // PHẦN 2: Seed 13 vai trò còn lại (11-23)
                SeedTqHkKtnb(context);             // 11. TQ/HK/KTNB
                SeedTruongphongItThKtgs(context);  // 12. Trưởng phó phòng IT/TH/KTGS
                SeedCbItThKtgsKhqlrr(context);     // 13. CB IT/TH/KTGS/KHQLRR
                SeedGiamdocPgd(context);           // 14. Giám đốc PGD
                SeedPhogiamdocPgd(context);        // 15. Phó giám đốc PGD
                SeedPhogiamdocPgdCbtd(context);    // 16. Phó giám đốc PGD kiêm CBTD
                SeedGiamdocCnl2(context);          // 17. Giám đốc CNL2
                SeedPhogiamdocCnl2Td(context);     // 18. Phó giám đốc CNL2 phụ trách Tín dụng
                SeedPhogiamdocCnl2Kt(context);     // 19. Phó giám đốc CNL2 phụ trách Kinh tế
                SeedTruongphongKhCnl2(context);    // 20. Trưởng phòng KH CNL2
                SeedPhophongKhCnl2(context);       // 21. Phó phòng KH CNL2
                SeedTruongphongKtnqCnl2(context);  // 22. Trưởng phòng KTNQ CNL2
                SeedPhophongKtnqCnl2(context);     // 23. Phó phòng KTNQ CNL2

                // Lưu tất cả thay đổi
                context.SaveChanges();
                
                Console.WriteLine("✅ Hoàn thành seed tất cả 23 vai trò cho hệ thống KPI!");
                Console.WriteLine($"📊 Tổng số chỉ tiêu KPI đã tạo: {context.KPIDefinitions.Count()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi seed KPI definitions: {ex.Message}");
                Console.WriteLine($"📍 Chi tiết lỗi: {ex.InnerException?.Message}");
                throw;
            }
        }

        #region 1. TruongphongKhdn - Trưởng phòng KHDN (8 chỉ tiêu)
        private static void SeedTruongphongKhdn(ApplicationDbContext context)
        {
            var roleCode = "TruongphongKhdn";
            var roleDescription = "Trưởng phòng KHDN";
            Console.WriteLine($"📋 Đang seed KPI cho {roleDescription}...");

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Tổng Dư nợ KHDN", Description = "Tổng dư nợ khách hàng doanh nghiệp", MaxScore = 20, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Tỷ lệ nợ xấu KHDN", Description = "Tỷ lệ nợ xấu khách hàng doanh nghiệp", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Thu nợ đã XLRR KHDN", Description = "Thu nợ đã xử lý rủi ro khách hàng doanh nghiệp", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Lợi nhuận khoán tài chính", Description = "Lợi nhuận khoán tài chính", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Phát triển Khách hàng Doanh nghiệp", Description = "Phát triển khách hàng doanh nghiệp mới", MaxScore = 10, ValueType = KpiValueType.NUMBER, UnitOfMeasure = "Khách hàng", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Điều hành theo chương trình công tác", Description = "Điều hành theo chương trình công tác", MaxScore = 20, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_07", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ", Description = "Chấp hành quy chế, quy trình nghiệp vụ", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_08", KpiName = "BQ kết quả thực hiện CB trong phòng mình phụ trách", Description = "Bình quân kết quả thực hiện cán bộ trong phòng mình phụ trách", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }
        #endregion

        #region 2. TruongphongKhcn - Trưởng phòng KHCN (8 chỉ tiêu)
        private static void SeedTruongphongKhcn(ApplicationDbContext context)
        {
            var roleCode = "TruongphongKhcn";
            var roleDescription = "Trưởng phòng KHCN";
            Console.WriteLine($"📋 Đang seed KPI cho {roleDescription}...");

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Tổng Dư nợ KHCN", Description = "Tổng dư nợ khách hàng cá nhân", MaxScore = 20, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Tỷ lệ nợ xấu KHCN", Description = "Tỷ lệ nợ xấu khách hàng cá nhân", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Thu nợ đã XLRR KHCN", Description = "Thu nợ đã xử lý rủi ro khách hàng cá nhân", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Lợi nhuận khoán tài chính", Description = "Lợi nhuận khoán tài chính", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Phát triển Khách hàng Cá nhân", Description = "Phát triển khách hàng cá nhân mới", MaxScore = 10, ValueType = KpiValueType.NUMBER, UnitOfMeasure = "Khách hàng", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Điều hành theo chương trình công tác", Description = "Điều hành theo chương trình công tác", MaxScore = 20, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_07", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ", Description = "Chấp hành quy chế, quy trình nghiệp vụ", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_08", KpiName = "BQ kết quả thực hiện CB trong phòng mình phụ trách", Description = "Bình quân kết quả thực hiện cán bộ trong phòng mình phụ trách", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }
        #endregion

        #region 3. PhophongKhdn - Phó phòng KHDN (8 chỉ tiêu)
        private static void SeedPhophongKhdn(ApplicationDbContext context)
        {
            var roleCode = "PhophongKhdn";
            var roleDescription = "Phó phòng KHDN";
            Console.WriteLine($"📋 Đang seed KPI cho {roleDescription}...");

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Tổng Dư nợ KHDN", Description = "Tổng dư nợ khách hàng doanh nghiệp", MaxScore = 20, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Tỷ lệ nợ xấu KHDN", Description = "Tỷ lệ nợ xấu khách hàng doanh nghiệp", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Thu nợ đã XLRR KHDN", Description = "Thu nợ đã xử lý rủi ro khách hàng doanh nghiệp", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Lợi nhuận khoán tài chính", Description = "Lợi nhuận khoán tài chính", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Phát triển Khách hàng Doanh nghiệp", Description = "Phát triển khách hàng doanh nghiệp mới", MaxScore = 10, ValueType = KpiValueType.NUMBER, UnitOfMeasure = "Khách hàng", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Điều hành theo chương trình công tác", Description = "Điều hành theo chương trình công tác", MaxScore = 20, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_07", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ", Description = "Chấp hành quy chế, quy trình nghiệp vụ", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_08", KpiName = "BQ kết quả thực hiện CB trong phòng mình phụ trách", Description = "Bình quân kết quả thực hiện cán bộ trong phòng mình phụ trách", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }
        #endregion

        #region 4. PhophongKhcn - Phó phòng KHCN (8 chỉ tiêu)
        private static void SeedPhophongKhcn(ApplicationDbContext context)
        {
            var roleCode = "PhophongKhcn";
            var roleDescription = "Phó phòng KHCN";
            Console.WriteLine($"📋 Đang seed KPI cho {roleDescription}...");

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Tổng Dư nợ KHCN", Description = "Tổng dư nợ khách hàng cá nhân", MaxScore = 20, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Tỷ lệ nợ xấu KHCN", Description = "Tỷ lệ nợ xấu khách hàng cá nhân", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Thu nợ đã XLRR KHCN", Description = "Thu nợ đã xử lý rủi ro khách hàng cá nhân", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Lợi nhuận khoán tài chính", Description = "Lợi nhuận khoán tài chính", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Phát triển Khách hàng Cá nhân", Description = "Phát triển khách hàng cá nhân mới", MaxScore = 10, ValueType = KpiValueType.NUMBER, UnitOfMeasure = "Khách hàng", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Điều hành theo chương trình công tác", Description = "Điều hành theo chương trình công tác", MaxScore = 20, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_07", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ", Description = "Chấp hành quy chế, quy trình nghiệp vụ", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_08", KpiName = "BQ kết quả thực hiện CB trong phòng mình phụ trách", Description = "Bình quân kết quả thực hiện cán bộ trong phòng mình phụ trách", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }
        #endregion

        #region 5. TruongphongKhqlrr - Trưởng phòng Kế hoạch và Quản lý rủi ro (6 chỉ tiêu)
        private static void SeedTruongphongKhqlrr(ApplicationDbContext context)
        {
            var roleCode = "TruongphongKhqlrr";
            var roleDescription = "Trưởng phòng Kế hoạch và Quản lý rủi ro";
            Console.WriteLine($"📋 Đang seed KPI cho {roleDescription}...");

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Tổng nguồn vốn", Description = "Tổng nguồn vốn", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Tổng dư nợ", Description = "Tổng dư nợ", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Lợi nhuận khoán tài chính", Description = "Lợi nhuận khoán tài chính", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng", Description = "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng", MaxScore = 50, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", Description = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Kết quả thực hiện BQ của CB trong phòng", Description = "Kết quả thực hiện bình quân của cán bộ trong phòng", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }
        #endregion

        #region 6. PhophongKhqlrr - Phó phòng Kế hoạch và Quản lý rủi ro (6 chỉ tiêu)
        private static void SeedPhophongKhqlrr(ApplicationDbContext context)
        {
            var roleCode = "PhophongKhqlrr";
            var roleDescription = "Phó phòng Kế hoạch và Quản lý rủi ro";
            Console.WriteLine($"📋 Đang seed KPI cho {roleDescription}...");

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Tổng nguồn vốn", Description = "Tổng nguồn vốn", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Tổng dư nợ", Description = "Tổng dư nợ", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Lợi nhuận khoán tài chính", Description = "Lợi nhuận khoán tài chính", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng", Description = "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng", MaxScore = 50, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", Description = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Kết quả thực hiện BQ của CB trong phòng mình phụ trách", Description = "Kết quả thực hiện bình quân của cán bộ trong phòng mình phụ trách", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }
        #endregion

        #region 7. Cbtd - Cán bộ Tín dụng (8 chỉ tiêu)
        private static void SeedCbtd(ApplicationDbContext context)
        {
            var roleCode = "Cbtd";
            var roleDescription = "Cán bộ Tín dụng";
            Console.WriteLine($"📋 Đang seed KPI cho {roleDescription}...");

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Tổng dư nợ BQ", Description = "Tổng dư nợ bình quân", MaxScore = 30, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Tỷ lệ nợ xấu", Description = "Tỷ lệ nợ xấu", MaxScore = 15, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Phát triển Khách hàng", Description = "Phát triển khách hàng mới", MaxScore = 10, ValueType = KpiValueType.NUMBER, UnitOfMeasure = "Khách hàng", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Thu nợ đã XLRR (nếu không có nợ XLRR thì cộng vào chỉ tiêu Dư nợ)", Description = "Thu nợ đã xử lý rủi ro (nếu không có nợ XLRR thì cộng vào chỉ tiêu Dư nợ)", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Thực hiện nhiệm vụ theo chương trình công tác", Description = "Thực hiện nhiệm vụ theo chương trình công tác", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", Description = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_07", KpiName = "Tổng nguồn vốn huy động BQ", Description = "Tổng nguồn vốn huy động bình quân", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_08", KpiName = "Hoàn thành chỉ tiêu giao khoán SPDV", Description = "Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ", MaxScore = 5, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }
        #endregion

        #region 8. TruongphongKtnqCnl1 - Trưởng phòng KTNQ CNL1 (6 chỉ tiêu)
        private static void SeedTruongphongKtnqCnl1(ApplicationDbContext context)
        {
            var roleCode = "TruongphongKtnqCnl1";
            var roleDescription = "Trưởng phòng KTNQ CNL1";
            Console.WriteLine($"📋 Đang seed KPI cho {roleDescription}...");

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Tổng nguồn vốn", Description = "Tổng nguồn vốn", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Lợi nhuận khoán tài chính", Description = "Lợi nhuận khoán tài chính", MaxScore = 20, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Thu dịch vụ thanh toán trong nước", Description = "Thu dịch vụ thanh toán trong nước", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng", Description = "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng", MaxScore = 40, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", Description = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Kết quả thực hiện BQ của CB trong phòng", Description = "Kết quả thực hiện bình quân của cán bộ trong phòng", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }
        #endregion

        #region 9. PhophongKtnqCnl1 - Phó phòng KTNQ CNL1 (6 chỉ tiêu)
        private static void SeedPhophongKtnqCnl1(ApplicationDbContext context)
        {
            var roleCode = "PhophongKtnqCnl1";
            var roleDescription = "Phó phòng KTNQ CNL1";
            Console.WriteLine($"📋 Đang seed KPI cho {roleDescription}...");

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Tổng nguồn vốn", Description = "Tổng nguồn vốn", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Lợi nhuận khoán tài chính", Description = "Lợi nhuận khoán tài chính", MaxScore = 20, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Thu dịch vụ thanh toán trong nước", Description = "Thu dịch vụ thanh toán trong nước", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng", Description = "Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng", MaxScore = 40, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", Description = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Kết quả thực hiện BQ của CB thuộc mình phụ trách", Description = "Kết quả thực hiện bình quân của cán bộ thuộc mình phụ trách", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }
        #endregion

        #region 10. Gdv - Giao dịch viên (6 chỉ tiêu)
        private static void SeedGdv(ApplicationDbContext context)
        {
            var roleCode = "Gdv";
            var roleDescription = "Giao dịch viên";
            Console.WriteLine($"📋 Đang seed KPI cho {roleDescription}...");

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Số bút toán giao dịch BQ", Description = "Số bút toán giao dịch bình quân", MaxScore = 50, ValueType = KpiValueType.NUMBER, UnitOfMeasure = "BT", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Số bút toán hủy", Description = "Số bút toán hủy", MaxScore = 15, ValueType = KpiValueType.NUMBER, UnitOfMeasure = "BT", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Thực hiện chức năng, nhiệm vụ được giao", Description = "Thực hiện chức năng, nhiệm vụ được giao", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", Description = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Tổng nguồn vốn huy động BQ", Description = "Tổng nguồn vốn huy động bình quân", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Hoàn thành chỉ tiêu giao khoán SPDV", Description = "Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ", MaxScore = 5, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }
        #endregion

        #region VAI TRÒ 11-23: SEED CÁC VAI TRÒ CÒN LẠI

        /// <summary>
        /// VAI TRÒ 11: TQ/HK/KTNB - Thủ quỹ/Hậu kiểm/Kế toán nội bộ
        /// </summary>
        private static void SeedTqHkKtnb(ApplicationDbContext context)
        {
            var roleDescription = "TQ/Hậu kiểm/Kế toán nội bộ";
            Console.WriteLine($"📋 Đang seed KPI cho {roleDescription}...");
            Console.WriteLine("⚠️ Lưu ý: Đợi TP KTNQ/Giám đốc CN loại 2 trực tiếp giao sau (chưa có cụ thể trong 186)");

            // Tạm thời không seed chỉ tiêu cho vai trò này vì chưa có quy định cụ thể
            var kpiDefinitions = new List<KPIDefinition>();

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription} (chờ quy định cụ thể)");
        }

        /// <summary>
        /// VAI TRÒ 12: Trưởng phó phòng IT/TH/KTGS
        /// </summary>
        private static void SeedTruongphongItThKtgs(ApplicationDbContext context)
        {
            var roleCode = "TruongphongItThKtgs";
            var roleDescription = "Trưởng phó phòng IT/TH/KTGS";
            Console.WriteLine($"📋 Đang seed KPI cho {roleDescription}...");

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng", Description = "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng", MaxScore = 65, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", Description = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Tổng nguồn vốn huy động BQ", Description = "Tổng nguồn vốn huy động bình quân", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Hoàn thành chỉ tiêu giao khoán SPDV", Description = "Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ", MaxScore = 5, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Kết quả thực hiện BQ của cán bộ trong phòng", Description = "Kết quả thực hiện bình quân của cán bộ trong phòng", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }

        /// <summary>
        /// VAI TRÒ 13: CB IT/TH/KTGS/KHQLRR
        /// </summary>
        private static void SeedCbItThKtgsKhqlrr(ApplicationDbContext context)
        {
            var roleCode = "CbItThKtgsKhqlrr";
            var roleDescription = "CB IT/TH/KTGS/KHQLRR";

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ được giao", Description = "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ được giao", MaxScore = 75, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", Description = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Tổng nguồn vốn huy động BQ", Description = "Tổng nguồn vốn huy động bình quân", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Hoàn thành chỉ tiêu giao khoán SPDV", Description = "Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ", MaxScore = 5, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }

        /// <summary>
        /// VAI TRÒ 14: Giám đốc PGD
        /// </summary>
        private static void SeedGiamdocPgd(ApplicationDbContext context)
        {
            var roleCode = "GiamdocPgd";
            var roleDescription = "Giám đốc PGD";

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Tổng nguồn vốn BQ", Description = "Tổng nguồn vốn bình quân", MaxScore = 15, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Tổng dư nợ BQ", Description = "Tổng dư nợ bình quân", MaxScore = 15, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Tỷ lệ nợ xấu", Description = "Tỷ lệ nợ xấu", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Phát triển Khách hàng", Description = "Phát triển khách hàng mới", MaxScore = 10, ValueType = KpiValueType.NUMBER, UnitOfMeasure = "Khách hàng", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)", Description = "Thu nợ đã xử lý rủi ro (nếu không có thì cộng vào chỉ tiêu dư nợ)", MaxScore = 5, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Thu dịch vụ", Description = "Thu dịch vụ", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_07", KpiName = "Lợi nhuận khoán tài chính", Description = "Lợi nhuận khoán tài chính", MaxScore = 15, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_08", KpiName = "Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", Description = "Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_09", KpiName = "BQ kết quả thực hiện của CB trong phòng", Description = "Bình quân kết quả thực hiện của cán bộ trong phòng", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }

        /// <summary>
        /// VAI TRÒ 15: Phó giám đốc PGD
        /// </summary>
        private static void SeedPhogiamdocPgd(ApplicationDbContext context)
        {
            var roleCode = "PhogiamdocPgd";
            var roleDescription = "Phó giám đốc PGD";

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Tổng nguồn vốn BQ", Description = "Tổng nguồn vốn bình quân", MaxScore = 15, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Tổng dư nợ BQ", Description = "Tổng dư nợ bình quân", MaxScore = 15, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Tỷ lệ nợ xấu", Description = "Tỷ lệ nợ xấu", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Phát triển Khách hàng", Description = "Phát triển khách hàng mới", MaxScore = 10, ValueType = KpiValueType.NUMBER, UnitOfMeasure = "Khách hàng", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)", Description = "Thu nợ đã xử lý rủi ro (nếu không có thì cộng vào chỉ tiêu dư nợ)", MaxScore = 5, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Thu dịch vụ", Description = "Thu dịch vụ", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_07", KpiName = "Lợi nhuận khoán tài chính", Description = "Lợi nhuận khoán tài chính", MaxScore = 15, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_08", KpiName = "Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", Description = "Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_09", KpiName = "BQ kết quả thực hiện của CB trong phòng", Description = "Bình quân kết quả thực hiện của cán bộ trong phòng", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }

        /// <summary>
        /// VAI TRÒ 16: Phó giám đốc PGD kiêm CBTD
        /// </summary>
        private static void SeedPhogiamdocPgdCbtd(ApplicationDbContext context)
        {
            var roleCode = "PhogiamdocPgdCbtd";
            var roleDescription = "Phó giám đốc PGD kiêm CBTD";

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Tổng dư nợ BQ", Description = "Tổng dư nợ bình quân", MaxScore = 30, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Tỷ lệ nợ xấu", Description = "Tỷ lệ nợ xấu", MaxScore = 15, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Phát triển Khách hàng", Description = "Phát triển khách hàng mới", MaxScore = 10, ValueType = KpiValueType.NUMBER, UnitOfMeasure = "Khách hàng", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)", Description = "Thu nợ đã xử lý rủi ro (nếu không có thì cộng vào chỉ tiêu dư nợ)", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Thực hiện nhiệm vụ theo chương trình công tác", Description = "Thực hiện nhiệm vụ theo chương trình công tác", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Tổng nguồn vốn huy động BQ", Description = "Tổng nguồn vốn huy động bình quân", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_07", KpiName = "Hoàn thành chỉ tiêu giao khoán SPDV", Description = "Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ", MaxScore = 5, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_08", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", Description = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }

        /// <summary>
        /// VAI TRÒ 17: Giám đốc CNL2
        /// </summary>
        private static void SeedGiamdocCnl2(ApplicationDbContext context)
        {
            var roleCode = "GiamdocCnl2";
            var roleDescription = "Giám đốc CNL2";

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Tổng nguồn vốn cuối kỳ", Description = "Tổng nguồn vốn cuối kỳ", MaxScore = 5, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Tổng nguồn vốn huy động BQ trong kỳ", Description = "Tổng nguồn vốn huy động bình quân trong kỳ", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Tổng dư nợ cuối kỳ", Description = "Tổng dư nợ cuối kỳ", MaxScore = 5, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Tổng dư nợ BQ trong kỳ", Description = "Tổng dư nợ bình quân trong kỳ", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Tổng dư nợ HSX&CN", Description = "Tổng dư nợ hợp tác xã và chủ nợ", MaxScore = 5, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Tỷ lệ nợ xấu nội bảng", Description = "Tỷ lệ nợ xấu nội bảng", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_07", KpiName = "Thu nợ đã XLRR", Description = "Thu nợ đã xử lý rủi ro", MaxScore = 5, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_08", KpiName = "Phát triển Khách hàng", Description = "Phát triển khách hàng mới", MaxScore = 10, ValueType = KpiValueType.NUMBER, UnitOfMeasure = "Khách hàng", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_09", KpiName = "Lợi nhuận khoán tài chính", Description = "Lợi nhuận khoán tài chính", MaxScore = 20, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_10", KpiName = "Thu dịch vụ", Description = "Thu dịch vụ", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_11", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank", Description = "Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }

        /// <summary>
        /// VAI TRÒ 18: Phó giám đốc CNL2 phụ trách Tín dụng
        /// </summary>
        private static void SeedPhogiamdocCnl2Td(ApplicationDbContext context)
        {
            var roleCode = "PhogiamdocCnl2Td";
            var roleDescription = "Phó giám đốc CNL2 phụ trách Tín dụng";

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Tổng dư nợ cho vay", Description = "Tổng dư nợ cho vay", MaxScore = 20, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Tổng dư nợ cho vay HSX&CN", Description = "Tổng dư nợ cho vay hợp tác xã và chủ nợ", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Thu nợ đã xử lý", Description = "Thu nợ đã xử lý", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Lợi nhuận khoán tài chính", Description = "Lợi nhuận khoán tài chính", MaxScore = 20, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Tỷ lệ nợ xấu", Description = "Tỷ lệ nợ xấu", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Phát triển Khách hàng", Description = "Phát triển khách hàng mới", MaxScore = 10, ValueType = KpiValueType.NUMBER, UnitOfMeasure = "Khách hàng", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_07", KpiName = "Điều hành theo chương trình công tác, nhiệm vụ được giao", Description = "Điều hành theo chương trình công tác, nhiệm vụ được giao", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_08", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", Description = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }

        /// <summary>
        /// VAI TRÒ 19: Phó giám đốc CNL2 phụ trách Kế toán
        /// </summary>
        private static void SeedPhogiamdocCnl2Kt(ApplicationDbContext context)
        {
            var roleCode = "PhogiamdocCnl2Kt";
            var roleDescription = "Phó giám đốc CNL2 phụ trách Kế toán";

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Tổng nguồn vốn", Description = "Tổng nguồn vốn", MaxScore = 20, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Lợi nhuận khoán tài chính", Description = "Lợi nhuận khoán tài chính", MaxScore = 30, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Tổng doanh thu phí dịch vụ", Description = "Tổng doanh thu phí dịch vụ", MaxScore = 20, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Số thẻ phát hành", Description = "Số thẻ phát hành", MaxScore = 10, ValueType = KpiValueType.NUMBER, UnitOfMeasure = "cái", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Điều hành theo chương trình công tác, nhiệm vụ được giao", Description = "Điều hành theo chương trình công tác, nhiệm vụ được giao", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", Description = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }

        /// <summary>
        /// VAI TRÒ 20: Trưởng phòng KH CNL2
        /// </summary>
        private static void SeedTruongphongKhCnl2(ApplicationDbContext context)
        {
            var roleCode = "TruongphongKhCnl2";
            var roleDescription = "Trưởng phòng KH CNL2";

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Tổng dư nợ", Description = "Tổng dư nợ", MaxScore = 20, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Tỷ lệ nợ xấu", Description = "Tỷ lệ nợ xấu", MaxScore = 15, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Phát triển Khách hàng", Description = "Phát triển khách hàng mới", MaxScore = 10, ValueType = KpiValueType.NUMBER, UnitOfMeasure = "Khách hàng", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Thu nợ đã XLRR", Description = "Thu nợ đã xử lý rủi ro", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Điều hành theo chương trình công tác", Description = "Điều hành theo chương trình công tác", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", Description = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_07", KpiName = "Tổng nguồn vốn huy động BQ", Description = "Tổng nguồn vốn huy động bình quân", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_08", KpiName = "Hoàn thành chỉ tiêu giao khoán SPDV", Description = "Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ", MaxScore = 5, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_09", KpiName = "Kết quả thực hiện BQ của CB trong phòng", Description = "Kết quả thực hiện bình quân của cán bộ trong phòng", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }

        /// <summary>
        /// VAI TRÒ 21: Phó phòng KH CNL2
        /// </summary>
        private static void SeedPhophongKhCnl2(ApplicationDbContext context)
        {
            var roleCode = "PhophongKhCnl2";
            var roleDescription = "Phó phòng KH CNL2";

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Tổng dư nợ BQ", Description = "Tổng dư nợ bình quân", MaxScore = 30, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Tỷ lệ nợ xấu", Description = "Tỷ lệ nợ xấu", MaxScore = 15, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Phát triển Khách hàng", Description = "Phát triển khách hàng mới", MaxScore = 10, ValueType = KpiValueType.NUMBER, UnitOfMeasure = "Khách hàng", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Thu nợ đã XLRR", Description = "Thu nợ đã xử lý rủi ro", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Thực hiện nhiệm vụ theo chương trình công tác", Description = "Thực hiện nhiệm vụ theo chương trình công tác", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", Description = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_07", KpiName = "Tổng nguồn vốn huy động BQ", Description = "Tổng nguồn vốn huy động bình quân", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_08", KpiName = "Hoàn thành chỉ tiêu giao khoán SPDV", Description = "Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ", MaxScore = 5, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }

        /// <summary>
        /// VAI TRÒ 22: Trưởng phòng KTNQ CNL2
        /// </summary>
        private static void SeedTruongphongKtnqCnl2(ApplicationDbContext context)
        {
            var roleCode = "TruongphongKtnqCnl2";
            var roleDescription = "Trưởng phòng KTNQ CNL2";

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Tổng nguồn vốn", Description = "Tổng nguồn vốn", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Lợi nhuận khoán tài chính", Description = "Lợi nhuận khoán tài chính", MaxScore = 20, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Thu dịch vụ thanh toán trong nước", Description = "Thu dịch vụ thanh toán trong nước", MaxScore = 10, ValueType = KpiValueType.CURRENCY, UnitOfMeasure = "Tỷ VND", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng", Description = "Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng", MaxScore = 40, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", Description = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_06", KpiName = "Kết quả thực hiện BQ của CB trong phòng", Description = "Kết quả thực hiện bình quân của cán bộ trong phòng", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }

        /// <summary>
        /// VAI TRÒ 23: Phó phòng KTNQ CNL2
        /// </summary>
        private static void SeedPhophongKtnqCnl2(ApplicationDbContext context)
        {
            var roleCode = "PhophongKtnqCnl2";
            var roleDescription = "Phó phòng KTNQ CNL2";
            Console.WriteLine($"📋 Đang seed KPI cho {roleDescription}...");

            var kpiDefinitions = new List<KPIDefinition>
            {
                new KPIDefinition { KpiCode = $"{roleCode}_01", KpiName = "Số bút toán giao dịch BQ", Description = "Số bút toán giao dịch bình quân", MaxScore = 40, ValueType = KpiValueType.NUMBER, UnitOfMeasure = "BT", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_02", KpiName = "Số bút toán hủy", Description = "Số bút toán hủy", MaxScore = 20, ValueType = KpiValueType.NUMBER, UnitOfMeasure = "BT", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_03", KpiName = "Thực hiện nhiệm vụ theo chương trình công tác", Description = "Thực hiện nhiệm vụ theo chương trình công tác", MaxScore = 25, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_04", KpiName = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", Description = "Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank", MaxScore = 10, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow },
                new KPIDefinition { KpiCode = $"{roleCode}_05", KpiName = "Hoàn thành chỉ tiêu giao khoán SPDV", Description = "Hoàn thành chỉ tiêu giao khoán sản phẩm dịch vụ", MaxScore = 5, ValueType = KpiValueType.PERCENTAGE, UnitOfMeasure = "%", IsActive = true, Version = "1.0", CreatedDate = DateTime.UtcNow }
            };

            context.KPIDefinitions.AddRange(kpiDefinitions);
            Console.WriteLine($"✅ Đã thêm {kpiDefinitions.Count} chỉ tiêu cho {roleDescription}");
        }

        #endregion
    }
}