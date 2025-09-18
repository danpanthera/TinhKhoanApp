using Khoan.Api.Data;
using Khoan.Api.Models;

namespace Khoan.Api.Data
{
    public static class KhoanPeriodSeeder
    {
        public static void SeedKhoanPeriods(ApplicationDbContext context)
        {
            // Kiểm tra xem đã có dữ liệu chưa
            if (context.KhoanPeriods.Any())
            {
                return; // Đã có dữ liệu rồi, không cần seed
            }

            var periods = new List<KhoanPeriod>();

            // 1. Kỳ tháng 01/2025
            periods.Add(new KhoanPeriod
            {
                Name = "Tháng 01/2025",
                Type = PeriodType.MONTHLY,
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 1, 31, 23, 59, 59),
                Status = PeriodStatus.OPEN
            });

            // 2. Kỳ quý I/2025
            periods.Add(new KhoanPeriod
            {
                Name = "Quý I/2025",
                Type = PeriodType.QUARTERLY,
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 3, 31, 23, 59, 59),
                Status = PeriodStatus.DRAFT
            });

            // 3. Kỳ năm 2025
            periods.Add(new KhoanPeriod
            {
                Name = "Năm 2025",
                Type = PeriodType.ANNUAL,
                StartDate = new DateTime(2025, 1, 1),
                EndDate = new DateTime(2025, 12, 31, 23, 59, 59),
                Status = PeriodStatus.DRAFT
            });

            // 4. Kỳ tháng 02/2025
            periods.Add(new KhoanPeriod
            {
                Name = "Tháng 02/2025",
                Type = PeriodType.MONTHLY,
                StartDate = new DateTime(2025, 2, 1),
                EndDate = new DateTime(2025, 2, 28, 23, 59, 59),
                Status = PeriodStatus.DRAFT
            });

            // 5. Kỳ tháng 03/2025
            periods.Add(new KhoanPeriod
            {
                Name = "Tháng 03/2025",
                Type = PeriodType.MONTHLY,
                StartDate = new DateTime(2025, 3, 1),
                EndDate = new DateTime(2025, 3, 31, 23, 59, 59),
                Status = PeriodStatus.DRAFT
            });

            // 6. Kỳ quý II/2025
            periods.Add(new KhoanPeriod
            {
                Name = "Quý II/2025",
                Type = PeriodType.QUARTERLY,
                StartDate = new DateTime(2025, 4, 1),
                EndDate = new DateTime(2025, 6, 30, 23, 59, 59),
                Status = PeriodStatus.DRAFT
            });

            context.KhoanPeriods.AddRange(periods);
            context.SaveChanges();

            Console.WriteLine($"✅ Đã seed {periods.Count} kỳ khoán mẫu");
        }
    }
}
