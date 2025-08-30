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
    public class GL01RepositoryTests
    {
        private readonly Mock<DbSet<GL01>> _mockSet;
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly GL01Repository _repository;
        private readonly List<GL01> _data;

        public GL01RepositoryTests()
        {
            // Sample test data
            _data = new List<GL01>
            {
                // Stubbed legacy GL01RepositoryTests removed.
                // Configure mock DbSet
                _mockSet = new Mock<DbSet<GL01>>();
                _mockSet.As<IQueryable<GL01>>().Setup(m => m.Provider).Returns(_data.AsQueryable().Provider);
                _mockSet.As<IQueryable<GL01>>().Setup(m => m.Expression).Returns(_data.AsQueryable().Expression);
                _mockSet.As<IQueryable<GL01>>().Setup(m => m.ElementType).Returns(_data.AsQueryable().ElementType);
                _mockSet.As<IQueryable<GL01>>().Setup(m => m.GetEnumerator()).Returns(_data.AsQueryable().GetEnumerator());

                // Configure mock DbContext
                _mockContext = new Mock<ApplicationDbContext>();
                _mockContext.Setup(c => c.Set<GL01>()).Returns(_mockSet.Object);

                // Create repository with mock context
                _repository = new GL01Repository(_mockContext.Object);
            }

        [Fact]
        public async Task GetRecentAsync_ShouldReturnOrderedByCreatedDate()
        {
            // Act
            var result = await _repository.GetRecentAsync(2);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.First().ID);
            Assert.Equal(2, result.Last().ID);
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
        public async Task GetByUnitCodeAsync_ShouldReturnCorrectRecords()
        {
            // Arrange
            var unitCode = "7800";

            // Act
            var result = await _repository.GetByUnitCodeAsync(unitCode);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(unitCode, item.BRCD));
        }

        [Fact]
        public async Task GetByAccountCodeAsync_ShouldReturnCorrectRecords()
        {
            // Arrange
            var accountCode = "1111";

            // Act
            var result = await _repository.GetByAccountCodeAsync(accountCode);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal(accountCode, item.TRAD_ACCT));
        }

        [Fact]
        public async Task GetTotalTransactionsByUnitAsync_ShouldCalculateCorrectSum()
        {
            // Arrange
            var unitCode = "7808";
            var drCrFlag = "DR";

            // Setup mock for SumAsync
            _mockSet.Setup(m => m.SumAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<GL01, decimal>>>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(1000000);

            // Act
            var result = await _repository.GetTotalTransactionsByUnitAsync(unitCode, drCrFlag);

            // Assert
            Assert.Equal(1000000, result);
        }

        [Fact]
        public async Task GetTotalTransactionsByDateAsync_ShouldCalculateCorrectSum()
        {
            // Arrange
            var date = new DateTime(2025, 3, 1);
            var drCrFlag = "DR";

            // Setup mock for SumAsync
            _mockSet.Setup(m => m.SumAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<GL01, decimal>>>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(1000000);

            // Act
            var result = await _repository.GetTotalTransactionsByDateAsync(date, drCrFlag);

            // Assert
            Assert.Equal(1000000, result);
        }

        [Fact]
        public void UpdateRange_ShouldCallUpdateRangeOnDbSet()
        {
            // Arrange
            var entities = new List<GL01> { new GL01(), new GL01() };

            // Act
            _repository.UpdateRange(entities);

            // Assert
            _mockSet.Verify(m => m.UpdateRange(entities), Times.Once);
        }
    }
}
