using Khoan.Api.Models;

namespace Khoan.Api.Data
{
    public static class RoleSeeder
    {
        public static void SeedRoles(ApplicationDbContext context)
        {
            // Danh sách 23 roleCode từ SeedKPIDefinitionMaxScore.cs
            var kpiRoleCodes = new List<string>
            {
                "TruongphongKhdn",
                "TruongphongKhcn",
                "PhophongKhdn", 
                "PhophongKhcn",
                "TruongphongKhqlrr",
                "PhophongKhqlrr",
                "Cbtd",
                "TruongphongKtnqCnl1",
                "PhophongKtnqCnl1",
                "Gdv",
                "TqHkKtnb",
                "TruongphongItThKtgs",
                "CbItThKtgsKhqlrr",
                "GiamdocPgd",
                "PhogiamdocPgd",
                "PhogiamdocPgdCbtd",
                "GiamdocCnl2",
                "PhogiamdocCnl2Td",
                "PhogiamdocCnl2Kt",
                "TruongphongKhCnl2",
                "PhophongKhCnl2",
                "TruongphongKtnqCnl2",
                "PhophongKtnqCnl2"
            };

            // Kiểm tra xem các KPI roles đã tồn tại chưa
            var existingKpiRoles = context.Roles
                .Where(r => kpiRoleCodes.Contains(r.Name))
                .Select(r => r.Name)
                .ToList();

            // Nếu đã có đủ 23 KPI roles thì không cần seed nữa
            if (existingKpiRoles.Count >= 23)
            {
                Console.WriteLine($"✅ Đã có {existingKpiRoles.Count} vai trò KPI trong hệ thống, bỏ qua seeding");
                return;
            }

            var rolesToAdd = new List<Role>();

            // Chỉ tạo những roles chưa tồn tại
            var missingRoles = kpiRoleCodes.Except(existingKpiRoles).ToList();

            foreach (var roleCode in missingRoles)
            {
                string description = roleCode switch
                {
                    "TruongphongKhdn" => "Trưởng phòng KHDN",
                    "TruongphongKhcn" => "Trưởng phòng KHCN",
                    "PhophongKhdn" => "Phó phòng KHDN",
                    "PhophongKhcn" => "Phó phòng KHCN",
                    "TruongphongKhqlrr" => "Trưởng phòng Kế hoạch và Quản lý rủi ro",
                    "PhophongKhqlrr" => "Phó phòng Kế hoạch và Quản lý rủi ro",
                    "Cbtd" => "Cán bộ Tín dụng",
                    "TruongphongKtnqCnl1" => "Trưởng phòng KTNQ CNL1",
                    "PhophongKtnqCnl1" => "Phó phòng KTNQ CNL1",
                    "Gdv" => "Giao dịch viên",
                    "TqHkKtnb" => "TQ/Hậu kiểm/Kế toán nội bộ",
                    "TruongphongItThKtgs" => "Trưởng phó phòng IT/TH/KTGS",
                    "CbItThKtgsKhqlrr" => "CB IT/TH/KTGS/KHQLRR",
                    "GiamdocPgd" => "Giám đốc PGD",
                    "PhogiamdocPgd" => "Phó giám đốc PGD",
                    "PhogiamdocPgdCbtd" => "Phó giám đốc PGD kiêm CBTD",
                    "GiamdocCnl2" => "Giám đốc CNL2",
                    "PhogiamdocCnl2Td" => "Phó giám đốc CNL2 phụ trách Tín dụng",
                    "PhogiamdocCnl2Kt" => "Phó giám đốc CNL2 Phụ trách Kế toán",
                    "TruongphongKhCnl2" => "Trưởng phòng KH CNL2",
                    "PhophongKhCnl2" => "Phó phòng KH CNL2",
                    "TruongphongKtnqCnl2" => "Trưởng phòng KTNQ CNL2",
                    "PhophongKtnqCnl2" => "Phó phòng KTNQ CNL2",
                    _ => roleCode
                };

                rolesToAdd.Add(new Role
                {
                    Name = roleCode,
                    Description = description
                });
            }

            if (rolesToAdd.Any())
            {
                context.Roles.AddRange(rolesToAdd);
                context.SaveChanges();
                Console.WriteLine($"✅ Đã thêm {rolesToAdd.Count} vai trò KPI mới vào hệ thống");
            }
            else
            {
                Console.WriteLine("✅ Tất cả vai trò KPI đã tồn tại trong hệ thống");
            }
        }
    }
}