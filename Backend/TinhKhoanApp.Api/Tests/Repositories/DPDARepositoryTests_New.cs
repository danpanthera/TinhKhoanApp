using Xunit;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Repositories.Interfaces;

namespace TinhKhoanApp.Api.Tests.Repositories
{
    /// <summary>
    /// Repository tests cho DPDA - Data Access Layer validation
    /// Kiểm tra consistency với database structure: 13 business columns + Temporal Table + Columnstore
    /// </summary>
    public class DPDARepositoryTests_New : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IDPDARepository _repository;

        public DPDARepositoryTests_New()
        {
            // Setup in-memory database cho DPDA repository testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new DPDARepository(_context);
        }

        #region DPDA Repository CRUD Tests

        [Fact]
        public async Task AddAsync_Should_Create_DPDA_With_All_Business_Columns()
        {
            // Arrange - DPDA entity với 13 business columns đầy đủ
            var dpda = new DPDA
            {
                NGAY_DL = new DateTime(2024, 12, 31),
                MA_CHI_NHANH = "CNL1",
                MA_KHACH_HANG = "KH001",
                TEN_KHACH_HANG = "Nguyen Van Test",
                SO_TK = "1234567890123",
                TEN_TK = "Tai khoan tiet kiem",
                LOAI_TK = "TD",
                SO_DU = 1500000.75m,
                NGAY_MO_TK = new DateTime(2024, 01, 15),
                TINH_TRANG = "HOAT_DONG",
                MA_SP = "SP001",
                TEN_SP = "San pham tiet kiem",
                LAI_SUAT = 0.065m,
                KY_HAN = 12
            };

            // Act - Insert new DPDA record
            var result = await _repository.AddAsync(dpda);
            await _context.SaveChangesAsync();

            // Assert - Validate creation với all business columns
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("CNL1", result.MA_CHI_NHANH);
            Assert.Equal("KH001", result.MA_KHACH_HANG);
            Assert.Equal("Nguyen Van Test", result.TEN_KHACH_HANG);
            Assert.Equal("1234567890123", result.SO_TK);
            Assert.Equal("TD", result.LOAI_TK);
            Assert.Equal(1500000.75m, result.SO_DU);
            Assert.Equal("HOAT_DONG", result.TINH_TRANG);
            Assert.Equal(0.065m, result.LAI_SUAT);
            Assert.Equal(12, result.KY_HAN);
        }

        [Fact]
        public async Task GetByDateAsync_Should_Filter_By_NGAY_DL_Correctly()
        {
            // Arrange - Multiple DPDA records với different dates
            var targetDate = new DateTime(2024, 12, 31);
            var dpda1 = new DPDA { NGAY_DL = targetDate, MA_CHI_NHANH = "CNL1", MA_KHACH_HANG = "KH001", SO_DU = 1000000.00m };
            var dpda2 = new DPDA { NGAY_DL = targetDate.AddDays(-1), MA_CHI_NHANH = "CNL2", MA_KHACH_HANG = "KH002", SO_DU = 2000000.00m };
            var dpda3 = new DPDA { NGAY_DL = targetDate, MA_CHI_NHANH = "CNL3", MA_KHACH_HANG = "KH003", SO_DU = 500000.00m };

            await _repository.AddAsync(dpda1);
            await _repository.AddAsync(dpda2);
            await _repository.AddAsync(dpda3);
            await _context.SaveChangesAsync();

            // Act - Filter by date
            var results = await _repository.GetByDateAsync(targetDate);

            // Assert - Validate correct date filtering
            Assert.Equal(2, results.Count());
            Assert.All(results, r => Assert.Equal(targetDate.Date, r.NGAY_DL.Date));
            Assert.Contains(results, r => r.MA_CHI_NHANH == "CNL1");
            Assert.Contains(results, r => r.MA_CHI_NHANH == "CNL3");
        }

        [Fact]
        public async Task GetByBranchCodeAsync_Should_Filter_By_MA_CHI_NHANH()
        {
            // Arrange - DPDA records từ different branches
            var branchCode = "CNL1";
            var dpda1 = new DPDA { MA_CHI_NHANH = branchCode, MA_KHACH_HANG = "KH001", SO_TK = "123456", SO_DU = 1000000.00m };
            var dpda2 = new DPDA { MA_CHI_NHANH = "CNL2", MA_KHACH_HANG = "KH002", SO_TK = "789012", SO_DU = 2000000.00m };
            var dpda3 = new DPDA { MA_CHI_NHANH = branchCode, MA_KHACH_HANG = "KH003", SO_TK = "345678", SO_DU = 1500000.00m };

            await _repository.AddAsync(dpda1);
            await _repository.AddAsync(dpda2);
            await _repository.AddAsync(dpda3);
            await _context.SaveChangesAsync();

            // Act - Filter by branch code
            var results = await _repository.GetByBranchCodeAsync(branchCode);

            // Assert - Validate branch filtering
            Assert.Equal(2, results.Count());
            Assert.All(results, r => Assert.Equal(branchCode, r.MA_CHI_NHANH));
            Assert.Contains(results, r => r.MA_KHACH_HANG == "KH001");
            Assert.Contains(results, r => r.MA_KHACH_HANG == "KH003");
        }

        [Fact]
        public async Task GetByCustomerCodeAsync_Should_Filter_By_MA_KHACH_HANG()
        {
            // Arrange - DPDA records cho same customer
            var customerCode = "KH001";
            var dpda1 = new DPDA { MA_KHACH_HANG = customerCode, SO_TK = "123456", MA_CHI_NHANH = "CNL1", SO_DU = 1000000.00m };
            var dpda2 = new DPDA { MA_KHACH_HANG = "KH002", SO_TK = "789012", MA_CHI_NHANH = "CNL2", SO_DU = 2000000.00m };
            var dpda3 = new DPDA { MA_KHACH_HANG = customerCode, SO_TK = "345678", MA_CHI_NHANH = "CNL1", SO_DU = 1500000.00m };

            await _repository.AddAsync(dpda1);
            await _repository.AddAsync(dpda2);
            await _repository.AddAsync(dpda3);
            await _context.SaveChangesAsync();

            // Act - Filter by customer code
            var results = await _repository.GetByCustomerCodeAsync(customerCode);

            // Assert - Validate customer filtering
            Assert.Equal(2, results.Count());
            Assert.All(results, r => Assert.Equal(customerCode, r.MA_KHACH_HANG));
            Assert.Contains(results, r => r.SO_TK == "123456");
            Assert.Contains(results, r => r.SO_TK == "345678");
        }

        #endregion

        #region DPDA Database Schema Validation Tests

        [Fact]
        public async Task DPDA_Entity_Should_Support_All_13_Business_Columns()
        {
            // Arrange - DPDA với tất cả 13 business columns theo CSV structure
            var dpda = new DPDA
            {
                // System columns
                NGAY_DL = new DateTime(2024, 12, 31),

                // Business columns (1-13) theo DPDA CSV structure
                MA_CHI_NHANH = "CNL1",           // Column 1
                MA_KHACH_HANG = "KH123456",      // Column 2
                TEN_KHACH_HANG = "NGUYEN VAN LONG", // Column 3
                SO_TK = "1234567890123456",      // Column 4
                TEN_TK = "TAI KHOAN TIET KIEM BINH THUONG", // Column 5
                LOAI_TK = "TD",                  // Column 6
                SO_DU = 15750000.50m,           // Column 7
                NGAY_MO_TK = new DateTime(2024, 06, 15), // Column 8
                TINH_TRANG = "HOAT_DONG",       // Column 9
                MA_SP = "SP001",                 // Column 10
                TEN_SP = "TIET KIEM CO DINH 12 THANG", // Column 11
                LAI_SUAT = 0.068m,              // Column 12
                KY_HAN = 12                      // Column 13
            };

            // Act - Save entity với all business columns
            var result = await _repository.AddAsync(dpda);
            await _context.SaveChangesAsync();

            // Retrieve and validate
            var saved = await _repository.GetByIdAsync(result.Id);

            // Assert - Validate all 13 business columns saved correctly
            Assert.NotNull(saved);
            Assert.Equal("CNL1", saved.MA_CHI_NHANH);
            Assert.Equal("KH123456", saved.MA_KHACH_HANG);
            Assert.Equal("NGUYEN VAN LONG", saved.TEN_KHACH_HANG);
            Assert.Equal("1234567890123456", saved.SO_TK);
            Assert.Equal("TAI KHOAN TIET KIEM BINH THUONG", saved.TEN_TK);
            Assert.Equal("TD", saved.LOAI_TK);
            Assert.Equal(15750000.50m, saved.SO_DU);
            Assert.Equal(new DateTime(2024, 06, 15), saved.NGAY_MO_TK);
            Assert.Equal("HOAT_DONG", saved.TINH_TRANG);
            Assert.Equal("SP001", saved.MA_SP);
            Assert.Equal("TIET KIEM CO DINH 12 THANG", saved.TEN_SP);
            Assert.Equal(0.068m, saved.LAI_SUAT);
            Assert.Equal(12, saved.KY_HAN);
        }

        [Fact]
        public async Task DPDA_Should_Handle_Null_Values_In_Optional_Fields()
        {
            // Arrange - DPDA với some optional fields null
            var dpda = new DPDA
            {
                NGAY_DL = new DateTime(2024, 12, 31),
                MA_CHI_NHANH = "CNL1",
                MA_KHACH_HANG = "KH001",
                SO_TK = "123456",
                LOAI_TK = "TT",
                SO_DU = 0.00m,
                TINH_TRANG = "HOAT_DONG",
                // Optional fields null
                TEN_KHACH_HANG = null,
                TEN_TK = null,
                NGAY_MO_TK = null,
                MA_SP = null,
                TEN_SP = null,
                LAI_SUAT = null,
                KY_HAN = null
            };

            // Act - Save entity với null optional fields
            var result = await _repository.AddAsync(dpda);
            await _context.SaveChangesAsync();

            // Assert - Validate null handling
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("CNL1", result.MA_CHI_NHANH);
            Assert.Equal("KH001", result.MA_KHACH_HANG);
            Assert.Null(result.TEN_KHACH_HANG);
            Assert.Null(result.NGAY_MO_TK);
        }

        #endregion

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
