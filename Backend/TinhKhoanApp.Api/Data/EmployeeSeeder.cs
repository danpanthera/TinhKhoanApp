using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using Khoan.Api.Models;

namespace Khoan.Api.Data
{
    public static class EmployeeSeeder
    {
        public static async Task SeedEmployees(ApplicationDbContext context)
        {
            // Kiểm tra xem đã có nhân viên nào chưa (ngoài admin)
            var existingEmployees = await context.Employees.CountAsync();
            if (existingEmployees > 1) // Đã có admin và nhân viên khác
            {
                return;
            }

            // Lấy Unit IDs từ database
            var units = await context.Units.ToDictionaryAsync(u => u.Code, u => u.Id);
            var positions = await context.Positions.ToDictionaryAsync(p => p.Name, p => p.Id);

            // Kiểm tra xem có đủ units và positions không
            if (!units.Any() || !positions.Any())
            {
                Console.WriteLine("Units hoặc Positions chưa được seeded. Bỏ qua seeding employees.");
                return;
            }

            var employees = new List<Employee>
            {
                // CNL1 - Agribank CN Lai Châu
                new Employee
                {
                    EmployeeCode = "GD001",
                    CBCode = "123456001",
                    FullName = "Nguyễn Văn An",
                    Username = "giamdoc.laichau",
                    Email = "giamdoc@agribank.com.vn",
                    PhoneNumber = "0987654321",
                    IsActive = true,
                    UnitId = units.GetValueOrDefault("CNL1LC", units.Values.First()),
                    PositionId = positions.GetValueOrDefault("Giamdoc", positions.Values.First())
                },

                // Phòng KHDN
                new Employee
                {
                    EmployeeCode = "TP001",
                    CBCode = "123456002",
                    FullName = "Trần Thị Bình",
                    Username = "tp.khdn",
                    Email = "tp.khdn@agribank.com.vn",
                    PhoneNumber = "0987654322",
                    IsActive = true,
                    UnitId = units.GetValueOrDefault("Khdn", units.Values.First()),
                    PositionId = positions.GetValueOrDefault("Truongphong", positions.Values.First())
                },

                new Employee
                {
                    EmployeeCode = "PP001",
                    CBCode = "123456003",
                    FullName = "Lê Văn Cường",
                    Username = "pp.khdn",
                    Email = "pp.khdn@agribank.com.vn",
                    PhoneNumber = "0987654323",
                    IsActive = true,
                    UnitId = units.GetValueOrDefault("Khdn", units.Values.First()),
                    PositionId = positions.GetValueOrDefault("Phophong", positions.Values.First())
                },

                new Employee
                {
                    EmployeeCode = "CB001",
                    CBCode = "123456004",
                    FullName = "Phạm Thị Dung",
                    Username = "cb.khdn1",
                    Email = "cb.khdn1@agribank.com.vn",
                    PhoneNumber = "0987654324",
                    IsActive = true,
                    UnitId = units.GetValueOrDefault("Khdn", units.Values.First()),
                    PositionId = positions.GetValueOrDefault("CB", positions.Values.First())
                },

                // Phòng KHCN
                new Employee
                {
                    EmployeeCode = "TP002",
                    CBCode = "123456005",
                    FullName = "Hoàng Văn Đức",
                    Username = "tp.khcn",
                    Email = "tp.khcn@agribank.com.vn",
                    PhoneNumber = "0987654325",
                    IsActive = true,
                    UnitId = units.GetValueOrDefault("Khcn", units.Values.First()),
                    PositionId = positions.GetValueOrDefault("Truongphong", positions.Values.First())
                },

                new Employee
                {
                    EmployeeCode = "PP002",
                    CBCode = "123456006",
                    FullName = "Vũ Thị Hoa",
                    Username = "pp.khcn",
                    Email = "pp.khcn@agribank.com.vn",
                    PhoneNumber = "0987654326",
                    IsActive = true,
                    UnitId = units.GetValueOrDefault("Khcn", units.Values.First()),
                    PositionId = positions.GetValueOrDefault("Phophong", positions.Values.First())
                },

                // Phòng KHQLRR
                new Employee
                {
                    EmployeeCode = "TP003",
                    CBCode = "123456007",
                    FullName = "Đặng Văn Hùng",
                    Username = "tp.khqlrr",
                    Email = "tp.khqlrr@agribank.com.vn",
                    PhoneNumber = "0987654327",
                    IsActive = true,
                    UnitId = units.GetValueOrDefault("Khqlrr", units.Values.First()),
                    PositionId = positions.GetValueOrDefault("Truongphong", positions.Values.First())
                },

                new Employee
                {
                    EmployeeCode = "CB002",
                    CBCode = "123456008",
                    FullName = "Bùi Thị Lan",
                    Username = "cb.khqlrr",
                    Email = "cb.khqlrr@agribank.com.vn",
                    PhoneNumber = "0987654328",
                    IsActive = true,
                    UnitId = units.GetValueOrDefault("Khqlrr", units.Values.First()),
                    PositionId = positions.GetValueOrDefault("CB", positions.Values.First())
                },

                // Phòng KTNQ
                new Employee
                {
                    EmployeeCode = "TP004",
                    CBCode = "123456009",
                    FullName = "Ngô Văn Long",
                    Username = "tp.ktnq",
                    Email = "tp.ktnq@agribank.com.vn",
                    PhoneNumber = "0987654329",
                    IsActive = true,
                    UnitId = units.GetValueOrDefault("KtnqCNL1", units.Values.First()),
                    PositionId = positions.GetValueOrDefault("Truongphong", positions.Values.First())
                },

                new Employee
                {
                    EmployeeCode = "KT001",
                    CBCode = "123456010",
                    FullName = "Lý Thị Mai",
                    Username = "kt.ktnq",
                    Email = "kt.ktnq@agribank.com.vn",
                    PhoneNumber = "0987654330",
                    IsActive = true,
                    UnitId = units.GetValueOrDefault("KtnqCNL1", units.Values.First()),
                    PositionId = positions.GetValueOrDefault("KeToan", positions.Values.First())
                },

                new Employee
                {
                    EmployeeCode = "TQ001",
                    CBCode = "123456011",
                    FullName = "Trịnh Văn Nam",
                    Username = "tq.ktnq",
                    Email = "tq.ktnq@agribank.com.vn",
                    PhoneNumber = "0987654331",
                    IsActive = true,
                    UnitId = units.GetValueOrDefault("KtnqCNL1", units.Values.First()),
                    PositionId = positions.GetValueOrDefault("ThuQuy", positions.Values.First())
                },

                // Phòng KTGS
                new Employee
                {
                    EmployeeCode = "TP005",
                    CBCode = "123456012",
                    FullName = "Đinh Thị Oanh",
                    Username = "tp.ktgs",
                    Email = "tp.ktgs@agribank.com.vn",
                    PhoneNumber = "0987654332",
                    IsActive = true,
                    UnitId = units.GetValueOrDefault("KTGSNB", units.Values.First()),
                    PositionId = positions.GetValueOrDefault("Truongphong", positions.Values.First())
                },

                // Phòng Tổng hợp
                new Employee
                {
                    EmployeeCode = "TP006",
                    CBCode = "123456013",
                    FullName = "Cao Văn Phong",
                    Username = "tp.tonghop",
                    Email = "tp.tonghop@agribank.com.vn",
                    PhoneNumber = "0987654333",
                    IsActive = true,
                    UnitId = units.GetValueOrDefault("TONGHOP", units.Values.First()),
                    PositionId = positions.GetValueOrDefault("Truongphong", positions.Values.First())
                },


            };

            await context.Employees.AddRangeAsync(employees);
            await context.SaveChangesAsync();

            Console.WriteLine($"✅ Đã thêm {employees.Count} nhân viên mẫu vào hệ thống");
        }
    }
}
