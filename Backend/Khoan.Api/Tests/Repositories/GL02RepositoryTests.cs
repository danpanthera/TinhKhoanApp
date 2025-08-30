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
    public class GL02RepositoryTests
    {
        private readonly Mock<DbSet<GL02>> _mockSet;
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly GL02Repository _repository;
        private readonly List<GL02> _data;

        public GL02RepositoryTests()
        {
            // Sample test data
            _data = new List<GL02>
            {
                new GL02 {
                    Id = 1,
                    NGAY_DL = new DateTime(2024, 12, 1),
                    TRBRCD = "1000",
                    USERID = "1000A07800",
                    JOURSEQ = "1",
                    DYTRSEQ = "1",
                    LOCAC = "421101",
                    CCY = "VND",
                    BUSCD = "DP",
                    UNIT = "DA",
                    TRCD = "",
                    CUSTOMER = "7800-329810401",
                    TRTP = "Normal",
                    REFERENCE = "7.80021E+12",
                    REMARK = "Withdrawal BankNet ATM",
                    DRAMOUNT = 203300,
                    CRAMOUNT = 0,
                    CRTDTM = new DateTime(2024, 12, 1, 11, 35, 22),
                    CREATED_DATE = DateTime.Now.AddDays(-1)
                },
                new GL02 {
                    Id = 2,
                    NGAY_DL = new DateTime(2024, 12, 1),
                    TRBRCD = "1000",
                    USERID = "1000A07800",
                    JOURSEQ = "1",
                    DYTRSEQ = "5",
                    LOCAC = "519101",
                    CCY = "VND",
                    BUSCD = "GL",
                    UNIT = "CB",
                    TRCD = "",
                    CUSTOMER = "7800-000007709",
                    TRTP = "Normal",
                    REFERENCE = "7.80021E+12",
                    REMARK = "Withdrawal BankNet ATM",
                    DRAMOUNT = 0,
                    CRAMOUNT = 203300,
                    CRTDTM = new DateTime(2024, 12, 1, 11, 35, 22),
                    CREATED_DATE = DateTime.Now.AddDays(-2)
                },
                new GL02 {
                    Id = 3,
                    NGAY_DL = new DateTime(2024, 12, 1),
                    TRBRCD = "1000",
                    USERID = "1000A07800",
                    JOURSEQ = "2",
                    DYTRSEQ = "1",
                    LOCAC = "421101",
                    CCY = "VND",
                    BUSCD = "DP",
                    UNIT = "DA",
                    TRCD = "",
                    CUSTOMER = "7800-411762635",
                    TRTP = "Normal",
                    REFERENCE = "7.80021E+12",
                    REMARK = "Withdrawal BankNet ATM",
                    DRAMOUNT = 203300,
                    CRAMOUNT = 0,
                    CRTDTM = new DateTime(2024, 12, 1, 17, 23, 23),
                    CREATED_DATE = DateTime.Now.AddDays(-3)
                },
                new GL02 {
                    Id = 4,
                    NGAY_DL = new DateTime(2024, 12, 1),
                    TRBRCD = "1000",
                    USERID = "1000A07800",
                    JOURSEQ = "2",
                    DYTRSEQ = "5",
                    LOCAC = "519101",
                    CCY = "VND",
                    BUSCD = "GL",
                    UNIT = "CB",
                    TRCD = "",
                    CUSTOMER = "7800-000007709",
                    TRTP = "Normal",
                    REFERENCE = "7.80021E+12",
                    REMARK = "Withdrawal BankNet ATM",
                    DRAMOUNT = 0,
                    CRAMOUNT = 203300,
                    CRTDTM = new DateTime(2024, 12, 1, 17, 23, 23),
                    CREATED_DATE = DateTime.Now.AddDays(-4)
                },
                new GL02 {
                    Id = 5,
                    NGAY_DL = new DateTime(2024, 12, 2),
                    TRBRCD = "2000",
                    USERID = "2000A07808",
                    JOURSEQ = "1",
                    DYTRSEQ = "1",
                    LOCAC = "421102",
                    CCY = "VND",
                    BUSCD = "DP",
                    UNIT = "DA",
                    TRCD = "TR001",
                    CUSTOMER = "7808-123456789",
                    TRTP = "Special",
                    REFERENCE = "7.80082E+12",
                    REMARK = "Deposit at Branch",
                    DRAMOUNT = 0,
                    CRAMOUNT = 500000,
                    CRTDTM = new DateTime(2024, 12, 2, 10, 15, 30),
                    CREATED_DATE = DateTime.Now.AddDays(-5)
                }
            };

            // Mock DbSet
            _mockSet = MockDbSet(_data);

            // Mock Context
            _mockContext = new Mock<ApplicationDbContext>();
            _mockContext.Setup(c => c.GL02).Returns(_mockSet.Object);

            // Create repository with mock context
            _repository = new GL02Repository(_mockContext.Object);
        }

        [Fact]
        public async Task GetRecentAsync_ReturnsSpecifiedNumberOfRecords()
        {
            // Act
            var result = await _repository.GetRecentAsync(2);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByDateAsync_WithValidDate_ReturnsCorrectRecords()
        {
            // Arrange
            var date = new DateTime(2024, 12, 1);

            // Act
            var result = await _repository.GetByDateAsync(date);

            // Assert
            Assert.Equal(4, result.Count());
            Assert.All(result, item => Assert.Equal(date.Date, item.NGAY_DL.Date));
        }

        [Fact]
        public async Task GetByBranchCodeAsync_WithValidCode_ReturnsCorrectRecords()
        {
            // Arrange
            var branchCode = "1000";

            // Act
            var result = await _repository.GetByBranchCodeAsync(branchCode);

            // Assert
            Assert.Equal(4, result.Count());
            Assert.All(result, item => Assert.Equal(branchCode, item.TRBRCD));
        }

        [Fact]
        public async Task GetByUnitAsync_WithValidUnit_ReturnsCorrectRecords()
        {
            // Arrange
            var unit = "DA";

            // Act
            var result = await _repository.GetByUnitAsync(unit);

            // Assert
            Assert.Equal(3, result.Count());
            Assert.All(result, item => Assert.Equal(unit, item.UNIT));
        }

        [Fact]
        public async Task GetByAccountCodeAsync_WithValidCode_ReturnsCorrectRecords()
        {
            // Arrange
            var accountCode = "421101";

            // Act
            var result = await _repository.GetByAccountCodeAsync(accountCode);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(accountCode, item.LOCAC));
        }

        [Fact]
        public async Task GetByCustomerAsync_WithValidCustomer_ReturnsCorrectRecords()
        {
            // Arrange
            var customer = "7800-000007709";

            // Act
            var result = await _repository.GetByCustomerAsync(customer);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(customer, item.CUSTOMER));
        }

        [Fact]
        public async Task GetByTransactionTypeAsync_WithValidType_ReturnsCorrectRecords()
        {
            // Arrange
            var transactionType = "Normal";

            // Act
            var result = await _repository.GetByTransactionTypeAsync(transactionType);

            // Assert
            Assert.Equal(4, result.Count());
            Assert.All(result, item => Assert.Equal(transactionType, item.TRTP));
        }

        [Fact]
        public async Task GetTotalTransactionsByUnitAsync_WithValidData_ReturnsCorrectSum()
        {
            // Arrange
            var unit = "DA";
            var drCrFlag = "DR";

            // Act
            var result = await _repository.GetTotalTransactionsByUnitAsync(unit, drCrFlag);

            // Assert
            Assert.Equal(406600, result); // 203300 + 203300
        }

        [Fact]
        public async Task GetTotalTransactionsByDateAsync_WithValidData_ReturnsCorrectSum()
        {
            // Arrange
            var date = new DateTime(2024, 12, 1);
            var drCrFlag = "CR";

            // Act
            var result = await _repository.GetTotalTransactionsByDateAsync(date, drCrFlag);

            // Assert
            Assert.Equal(406600, result); // 203300 + 203300
        }

        [Fact]
        public async Task GetTotalTransactionsByBranchAsync_WithValidData_ReturnsCorrectSum()
        {
            // Arrange
            var branchCode = "2000";
            var drCrFlag = "CR";

            // Act
            var result = await _repository.GetTotalTransactionsByBranchAsync(branchCode, drCrFlag);

            // Assert
            Assert.Equal(500000, result);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsCorrectRecord()
        {
            // Arrange
            long id = 5;

            // Act
            var result = await _repository.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("2000", result.TRBRCD);
            Assert.Equal("7808-123456789", result.CUSTOMER);
        }

        [Fact]
        public async Task GetByDateRangeAsync_WithValidRange_ReturnsCorrectRecords()
        {
            // Arrange
            var fromDate = new DateTime(2024, 12, 1);
            var toDate = new DateTime(2024, 12, 2);

            // Act
            var result = await _repository.GetByDateRangeAsync(fromDate, toDate);

            // Assert
            Assert.Equal(5, result.Count());
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
