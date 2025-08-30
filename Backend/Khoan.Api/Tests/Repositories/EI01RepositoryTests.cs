using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Repositories;

namespace TinhKhoanApp.Api.Tests.Repositories
{
    public class EI01RepositoryTests
    {
        private readonly Mock<DbSet<EI01>> _mockSet;
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly EI01Repository _repository;
        private readonly List<EI01> _data;

        public EI01RepositoryTests()
        {
            // Sample test data for EI01 (Thông tin dịch vụ ngân hàng điện tử)
            _data = new List<EI01>
            {
                new EI01 {
                    Id = 1,
                    NGAY_DL = new DateTime(2025, 3, 1),
                    MA_CN = "7808",
                    MA_KH = "KH001",
                    TEN_KH = "Công ty ABC",
                    LOAI_KH = "1", // Doanh nghiệp
                    SO_DT = "0901234567",
                    EMAIL = "abc@example.com",
                    LOAI_DV = "IB", // Internet Banking
                    NGAY_DK = new DateTime(2025, 1, 15),
                    TRANG_THAI = "1", // Hoạt động
                    HAN_MUC_GD = 5000000,
                    HAN_MUC_NGAY = 50000000
                },
                new EI01 {
                    Id = 2,
                    NGAY_DL = new DateTime(2025, 3, 1),
                    MA_CN = "7808",
                    MA_KH = "KH002",
                    TEN_KH = "Nguyễn Văn A",
                    LOAI_KH = "2", // Cá nhân
                    SO_DT = "0912345678",
                    EMAIL = "nguyenvana@example.com",
                    LOAI_DV = "MB", // Mobile Banking
                    NGAY_DK = new DateTime(2025, 2, 10),
                    TRANG_THAI = "1", // Hoạt động
                    HAN_MUC_GD = 2000000,
                    HAN_MUC_NGAY = 10000000
                },
                new EI01 {
                    Id = 3,
                    NGAY_DL = new DateTime(2025, 3, 2),
                    MA_CN = "7800",
                    MA_KH = "KH003",
                    TEN_KH = "Trần Thị B",
                    LOAI_KH = "2", // Cá nhân
                    SO_DT = "0923456789",
                    EMAIL = "tranthib@example.com",
                    LOAI_DV = "SMS", // SMS Banking
                    NGAY_DK = new DateTime(2025, 2, 20),
                    TRANG_THAI = "0", // Không hoạt động
                    HAN_MUC_GD = 1000000,
                    HAN_MUC_NGAY = 5000000,
                    LY_DO_KHOA = "Yêu cầu của khách hàng"
                },
                new EI01 {
                    Id = 4,
                    NGAY_DL = new DateTime(2025, 3, 2),
                    MA_CN = "7800",
                    MA_KH = "KH004",
                    TEN_KH = "Công ty XYZ",
                    LOAI_KH = "1", // Doanh nghiệp
                    SO_DT = "0934567890",
                    EMAIL = "xyz@example.com",
                    LOAI_DV = "IB", // Internet Banking
                    NGAY_DK = new DateTime(2025, 1, 5),
                    TRANG_THAI = "1", // Hoạt động
                    HAN_MUC_GD = 10000000,
                    HAN_MUC_NGAY = 100000000
                },
                new EI01 {
                    Id = 5,
                    NGAY_DL = new DateTime(2025, 3, 2),
                    MA_CN = "7808",
                    MA_KH = "KH005",
                    TEN_KH = "Lê Văn C",
                    LOAI_KH = "2", // Cá nhân
                    SO_DT = "0901234567", // Cùng số điện thoại với KH001
                    EMAIL = "levanc@example.com",
                    LOAI_DV = "MB", // Mobile Banking
                    NGAY_DK = new DateTime(2025, 3, 1),
                    TRANG_THAI = "1", // Hoạt động
                    HAN_MUC_GD = 2000000,
                    HAN_MUC_NGAY = 10000000
                }
            };

            // Mock DbSet
            _mockSet = MockDbSet(_data);

            // Mock Context
            _mockContext = new Mock<ApplicationDbContext>();
            _mockContext.Setup(c => c.EI01).Returns(_mockSet.Object);

            // Create repository with mock context
            _repository = new EI01Repository(_mockContext.Object);
        }

        [Fact]
        public async Task GetAsync_ReturnsSpecifiedNumberOfRecords()
        {
            // Act
            var result = await _repository.GetAsync(take: 2);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByDateAsync_WithValidDate_ReturnsCorrectRecords()
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
        public async Task GetByBranchCodeAsync_WithValidCode_ReturnsCorrectRecords()
        {
            // Arrange
            var branchCode = "7808";

            // Act
            var result = await _repository.GetByBranchCodeAsync(branchCode);

            // Assert
            Assert.Equal(3, result.Count());
            Assert.All(result, item => Assert.Equal(branchCode, item.MA_CN));
        }

        [Fact]
        public async Task GetByCustomerCodeAsync_WithValidCode_ReturnsCorrectRecords()
        {
            // Arrange
            var customerCode = "KH001";

            // Act
            var result = await _repository.GetByCustomerCodeAsync(customerCode);

            // Assert
            Assert.Single(result);
            Assert.Equal(customerCode, result.First().MA_KH);
        }

        [Fact]
        public async Task GetByPhoneNumberAsync_WithValidNumber_ReturnsCorrectRecords()
        {
            // Arrange
            var phoneNumber = "0901234567";

            // Act
            var result = await _repository.GetByPhoneNumberAsync(phoneNumber);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(phoneNumber, item.SO_DT));
        }

        [Fact]
        public async Task GetByCustomerTypeAsync_WithValidType_ReturnsCorrectRecords()
        {
            // Arrange
            var customerType = "1"; // Doanh nghiệp

            // Act
            var result = await _repository.GetByCustomerTypeAsync(customerType);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(customerType, item.LOAI_KH));
        }

        [Fact]
        public async Task GetByServiceStatusAsync_WithValidStatus_ReturnsCorrectRecords()
        {
            // Arrange
            var serviceType = "IB";
            var status = "1"; // Hoạt động

            // Act
            var result = await _repository.GetByServiceStatusAsync(serviceType, status);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(serviceType, item.LOAI_DV));
            Assert.All(result, item => Assert.Equal(status, item.TRANG_THAI));
        }

        [Fact]
        public async Task GetByRegistrationDateRangeAsync_WithValidRange_ReturnsCorrectRecords()
        {
            // Arrange
            var fromDate = new DateTime(2025, 2, 1);
            var toDate = new DateTime(2025, 3, 1);
            var serviceType = "MB";

            // Act
            var result = await _repository.GetByRegistrationDateRangeAsync(fromDate, toDate, serviceType);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, item =>
                Assert.True(item.NGAY_DK >= fromDate && item.NGAY_DK <= toDate && item.LOAI_DV == serviceType));
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCorrectRecord()
        {
            // Arrange
            long id = 3;

            // Act
            var result = await _repository.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("KH003", result.MA_KH);
            Assert.Equal("Trần Thị B", result.TEN_KH);
        }

        [Fact]
        public async Task GetPagedAsync_WithValidPredicate_ReturnsCorrectRecords()
        {
            // Arrange - Get all records from branch 7800
            var predicate = new Func<EI01, bool>(e => e.MA_CN == "7800");

            // Act
            var (totalCount, items) = await _repository.GetPagedAsync(
                e => predicate(e),
                1,
                10,
                q => q.OrderBy(e => e.Id));

            // Assert
            Assert.Equal(2, totalCount);
            Assert.Equal(2, items.Count());
            Assert.All(items, item => Assert.Equal("7800", item.MA_CN));
        }

        #region Helper Methods

        private static Mock<DbSet<T>> MockDbSet<T>(IEnumerable<T> data) where T : class
        {
            var queryableData = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryableData.GetEnumerator());

            // Setup async operations to work with Entity Framework
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                .Returns<object[]>(ids => Task.FromResult(data.FirstOrDefault(d => GetId(d).Equals(ids[0]))));

            return mockSet;
        }

        private static object GetId<T>(T entity)
        {
            var prop = typeof(T).GetProperty("Id") ?? typeof(T).GetProperty("ID");
            return prop?.GetValue(entity) ?? 0;
        }

        #endregion
    }
}
