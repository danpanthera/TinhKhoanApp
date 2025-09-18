using Xunit;
using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using Khoan.Api.Models.DataTables;
using Khoan.Api.Repositories;
using Khoan.Api.Repositories.Interfaces;

namespace Khoan.Api.Tests.Repositories
{
    /// <summary>
    /// Repository tests cho DPDA - Data Access Layer validation
    /// Kiểm tra consistency với database structure: 13 business columns + Temporal Table + Columnstore
    /// </summary>
    public class DPDARepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IDPDARepository _repository;

        public DPDARepositoryTests()
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
                    NGAY_PHAT_HANH = null,
                CREATED_DATE = DateTime.Now.AddDays(-5)
            },
                new DPDA
                {
                    Id = 3,
                    NGAY_DL = new DateTime(2025, 3, 2),
                    MA_CHI_NHANH = "7800",
                    MA_KHACH_HANG = "111222",
                    TEN_KHACH_HANG = "Lê Văn C",
                    SO_TAI_KHOAN = "1000111222",
                    LOAI_THE = "ATM",
                    SO_THE = "9704567890123456",
                    TRANG_THAI = "ACTIVE",
                    NGAY_NOP_DON = new DateTime(2025, 2, 10),
                    NGAY_PHAT_HANH = new DateTime(2025, 2, 15),
                    CREATED_DATE = DateTime.Now.AddDays(-15)
                },
                new DPDA
                {
                    Id = 4,
                    NGAY_DL = new DateTime(2025, 3, 2),
                    MA_CHI_NHANH = "7800",
                    MA_KHACH_HANG = "222333",
                    TEN_KHACH_HANG = "Phạm Thị D",
                    SO_TAI_KHOAN = "1000222333",
                    LOAI_THE = "MASTERCARD",
                    SO_THE = "5412345678901234",
                    TRANG_THAI = "REJECTED",
                    NGAY_NOP_DON = new DateTime(2025, 2, 5),
                    NGAY_PHAT_HANH = null,
                    CREATED_DATE = DateTime.Now.AddDays(-20)
                },
            };

        // Configure mock DbSet
        _mockSet = new Mock<DbSet<DPDA>>();
            _mockSet.As<IQueryable<DPDA>>().Setup(m => m.Provider).Returns(_data.AsQueryable().Provider);
        _mockSet.As<IQueryable<DPDA>>().Setup(m => m.Expression).Returns(_data.AsQueryable().Expression);
        _mockSet.As<IQueryable<DPDA>>().Setup(m => m.ElementType).Returns(_data.AsQueryable().ElementType);
        _mockSet.As<IQueryable<DPDA>>().Setup(m => m.GetEnumerator()).Returns(_data.AsQueryable().GetEnumerator());

        // Configure mock DbContext
        _mockContext = new Mock<ApplicationDbContext>();
            _mockContext.Setup(c => c.Set<DPDA>()).Returns(_mockSet.Object);

        // Create repository with mock context
        _repository = new DPDARepository(_mockContext.Object);
        }

        [Fact]
        public async Task GetRecentAsync_ShouldReturnOrderedByCreatedDate()
        {
            // Act
            var result = await _repository.GetRecentAsync(2);

            // Assert
            Assert.Equal(2, result.Count());
            // Verify the ordering is correct based on CREATED_DATE
            var expectedOrder = _data.OrderByDescending(d => d.CREATED_DATE).Take(2).Select(d => d.Id).ToList();
            var actualOrder = result.Select(d => d.Id).ToList();
            Assert.Equal(expectedOrder, actualOrder);
        }

        [Fact]
        public async Task GetByDateAsync_ShouldReturnCorrectRecords()
        {
            // Arrange
            var date = new DateTime(2025, 3, 1);

            // Act
            var result = await _repository.GetByDateAsync(date);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(date.Date, item.NGAY_DL.Date));
        }

        [Fact]
        public async Task GetByBranchCodeAsync_ShouldReturnCorrectRecords()
        {
            // Arrange
            var branchCode = "7800";

            // Act
            var result = await _repository.GetByBranchCodeAsync(branchCode);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(branchCode, item.MA_CHI_NHANH));
        }

        [Fact]
        public async Task GetByCustomerCodeAsync_ShouldReturnCorrectRecords()
        {
            // Arrange
            var customerCode = "123456";

            // Act
            var result = await _repository.GetByCustomerCodeAsync(customerCode);

            // Assert
            Assert.Equal(1, result.Count());
            Assert.All(result, item => Assert.Equal(customerCode, item.MA_KHACH_HANG));
        }

        [Fact]
        public async Task GetByAccountNumberAsync_ShouldReturnCorrectRecords()
        {
            // Arrange
            var accountNumber = "1000111222";

            // Act
            var result = await _repository.GetByAccountNumberAsync(accountNumber);

            // Assert
            Assert.Equal(1, result.Count());
            Assert.All(result, item => Assert.Equal(accountNumber, item.SO_TAI_KHOAN));
        }

        [Fact]
        public async Task GetByCardNumberAsync_ShouldReturnCorrectRecords()
        {
            // Arrange
            var cardNumber = "9704123456789012";

            // Act
            var result = await _repository.GetByCardNumberAsync(cardNumber);

            // Assert
            Assert.Equal(1, result.Count());
            Assert.All(result, item => Assert.Equal(cardNumber, item.SO_THE));
        }

        [Fact]
        public async Task GetByStatusAsync_ShouldReturnCorrectRecords()
        {
            // Arrange
            var status = "ACTIVE";

            // Act
            var result = await _repository.GetByStatusAsync(status);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(status, item.TRANG_THAI));
        }

        [Fact]
        public async Task GetCardCountByBranchAndStatusAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            var branchCode = "7808";
            var status = "PENDING";

            // Setup mock for CountAsync
            _mockSet.Setup(m => m.CountAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<DPDA, bool>>>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(1);

            // Act
            var result = await _repository.GetCardCountByBranchAndStatusAsync(branchCode, status);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void UpdateRange_ShouldCallUpdateRangeOnDbSet()
        {
            // Arrange
            var entities = new List<DPDA> { new DPDA(), new DPDA() };

            // Act
            _repository.UpdateRange(entities);

            // Assert
            _mockSet.Verify(m => m.UpdateRange(entities), Times.Once);
        }
    }
}
