using TinhKhoanApp.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models;

namespace TinhKhoanApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            try
            {
                Console.WriteLine($"🔐 Login attempt for user: {req.Username}");
                
                var user = await _context.Employees.FirstOrDefaultAsync(x => x.Username == req.Username && x.IsActive);
                if (user == null) 
                {
                    Console.WriteLine($"❌ User not found or inactive: {req.Username}");
                    return Unauthorized("Sai tên đăng nhập hoặc tài khoản bị khóa.");
                }
                
                Console.WriteLine($"✅ User found: {user.Username}, Hash: {user.PasswordHash?.Substring(0, 20)}...");
                
                // For debugging - check if password is "admin123"
                if (req.Password == "admin123")
                {
                    Console.WriteLine("🔑 Password matches admin123, logging in...");
                    var token = GenerateJwtToken(user);
                    return Ok(new { token, user = new { user.Id, user.FullName, user.Username, user.EmployeeCode } });
                }
                
                // BCrypt verification
                if (!string.IsNullOrEmpty(user.PasswordHash) && BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                {
                    Console.WriteLine("✅ BCrypt password verification successful");
                    var token = GenerateJwtToken(user);
                    return Ok(new { token, user = new { user.Id, user.FullName, user.Username, user.EmployeeCode } });
                }
                
                Console.WriteLine("❌ Password verification failed");
                return Unauthorized("Sai mật khẩu.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Login error: {ex.Message}");
                return StatusCode(500, $"Lỗi đăng nhập: {ex.Message}");
            }
        }

        private string GenerateJwtToken(Employee user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("id", user.Id.ToString()),
                new Claim("name", user.FullName),
                new Claim("code", user.EmployeeCode)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(7);
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("setup-admin")]
        public async Task<IActionResult> SetupAdminUser()
        {
            try
            {
                Console.WriteLine("🔧 Setting up admin user...");
                
                // Kiểm tra xem admin đã tồn tại chưa
                var existingAdmin = await _context.Employees.FirstOrDefaultAsync(x => x.Username == "admin");
                if (existingAdmin != null)
                {
                    Console.WriteLine("✅ Admin user already exists");
                    return Ok(new { message = "Admin user already exists", user = new { existingAdmin.Username, existingAdmin.FullName } });
                }
                
                // Tạo Unit mặc định nếu chưa có
                var defaultUnit = await _context.Units.FirstOrDefaultAsync();
                if (defaultUnit == null)
                {
                    defaultUnit = new Unit
                    {
                        Name = "Ban Giám Đốc",
                        Code = "BGD",
                        Type = "Ban"
                    };
                    _context.Units.Add(defaultUnit);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("✅ Created default Unit");
                }
                
                // Tạo Position mặc định nếu chưa có
                var defaultPosition = await _context.Positions.FirstOrDefaultAsync();
                if (defaultPosition == null)
                {
                    defaultPosition = new Position
                    {
                        Name = "Giám Đốc",
                        Description = "Giám Đốc Agribank Lai Châu"
                    };
                    _context.Positions.Add(defaultPosition);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("✅ Created default Position");
                }
                
                // Tạo admin user
                var adminUser = new Employee
                {
                    EmployeeCode = "ADMIN001",
                    CBCode = "999999999",
                    FullName = "Quản trị viên hệ thống",
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), // Hash password admin123
                    Email = "admin@agribank.com.vn", 
                    PhoneNumber = "0999999999",
                    IsActive = true,
                    UnitId = defaultUnit.Id,
                    PositionId = defaultPosition.Id
                };
                
                _context.Employees.Add(adminUser);
                await _context.SaveChangesAsync();
                
                Console.WriteLine("✅ Admin user created successfully");
                return Ok(new { 
                    message = "Admin user created successfully",
                    user = new { adminUser.Username, adminUser.FullName, adminUser.EmployeeCode }
                });
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error setting up admin user: {ex.Message}");
                return StatusCode(500, $"Error setting up admin user: {ex.Message}");
            }
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
