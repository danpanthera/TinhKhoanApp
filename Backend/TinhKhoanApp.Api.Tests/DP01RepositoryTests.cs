using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Khoan.Api.Data;
using Khoan.Api.Models.DataTables;
using Khoan.Api.Repositories;
using Xunit;

namespace Khoan.Api.Tests.Repositories
{
    public class DP01RepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        
        public DP01RepositoryTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"DP01TestDb_{Guid.NewGuid()}")
                .Options;
                
            // Seed the database
            using (var context = new ApplicationDbContext(_options))
            {
                context.Database.EnsureCreated();
                
                if (!context.Set<DP01>().Any())
                {
                    context.Set<DP01>().AddRange(GetSampleDP01Data());
                    context.SaveChanges();
                }
            }
        }
        
        [Fact]
        public async Task GetRecentAsync_ShouldReturnOrderedByCreatedAt()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new DP01Repository(context);
            
            // Act
            var result = (await repository.GetRecentAsync(3)).ToList();
            
            // Assert
            Assert.Equal(3, result.Count);
            Assert.True(result[0].CreatedAt >= result[1].CreatedAt);
            Assert.True(result[1].CreatedAt >= result[2].CreatedAt);
        }
        
        [Fact]
        public async Task GetByBranchCodeAsync_ShouldReturnMatchingRecords()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new DP01Repository(context);
            
            // Act
            var result = (await repository.GetByBranchCodeAsync("7800")).ToList();
            
            // Assert
            Assert.True(result.Count > 0);
            Assert.All(result, item => Assert.Equal("7800", item.MA_CN));
        }
        
        [Fact]
        public async Task GetByCustomerCodeAsync_ShouldReturnMatchingRecords()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new DP01Repository(context);
            
            // Act
            var result = (await repository.GetByCustomerCodeAsync("CUST001")).ToList();
            
            // Assert
            Assert.True(result.Count > 0);
            Assert.All(result, item => Assert.Equal("CUST001", item.MA_KH));
        }
        
        [Fact]
        public async Task GetByAccountNumberAsync_ShouldReturnMatchingRecords()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new DP01Repository(context);
            
            // Act
            var result = (await repository.GetByAccountNumberAsync("ACC001")).ToList();
            
            // Assert
            Assert.True(result.Count > 0);
            Assert.All(result, item => Assert.Equal("ACC001", item.SO_TAI_KHOAN));
        }
        
        [Fact]
        public async Task GetTotalBalanceByBranchAsync_ShouldCalculateCorrectly()
        {
            // Arrange
            using var context = new ApplicationDbContext(_options);
            var repository = new DP01Repository(context);
            
            // Act
            var result = await repository.GetTotalBalanceByBranchAsync("7800");
            
            // Assert
            var expectedSum = GetSampleDP01Data()
                .Where(x => x.MA_CN == "7800")
                .Sum(x => x.CURRENT_BALANCE ?? 0);
                
            Assert.Equal(expectedSum, result);
        }
        
        private static List<DP01> GetSampleDP01Data()
        {
            var now = DateTime.Now;
            var yesterday = now.AddDays(-1);
            var twoDaysAgo = now.AddDays(-2);
            
            return new List<DP01>
            {
                new DP01
                {
                    Id = 1,
                    NGAY_DL = DateTime.Today,
                    MA_CN = "7800",
                    MA_KH = "CUST001",
                    TEN_KH = "Nguyen Van A",
                    CCY = "VND",
                    CURRENT_BALANCE = 1000000,
                    RATE = 3.4m,
                    SO_TAI_KHOAN = "ACC001",
                    OPENING_DATE = new DateTime(2025, 1, 1),
                    MATURITY_DATE = new DateTime(2026, 1, 1),
                    ADDRESS = "123 Nguyen Hue, Ho Chi Minh City",
                    CreatedAt = now,
                    UpdatedAt = now,
                    DataSource = "Test",
                    ImportDateTime = now
                },
                new DP01
                {
                    Id = 2,
                    NGAY_DL = DateTime.Today,
                    MA_CN = "7800",
                    MA_KH = "CUST002",
                    TEN_KH = "Tran Thi B",
                    CCY = "VND",
                    CURRENT_BALANCE = 2000000,
                    RATE = 3.5m,
                    SO_TAI_KHOAN = "ACC002",
                    OPENING_DATE = new DateTime(2025, 2, 1),
                    MATURITY_DATE = new DateTime(2026, 2, 1),
                    ADDRESS = "456 Le Loi, Ho Chi Minh City",
                    CreatedAt = yesterday,
                    UpdatedAt = yesterday,
                    DataSource = "Test",
                    ImportDateTime = yesterday
                },
                new DP01
                {
                    Id = 3,
                    NGAY_DL = DateTime.Today,
                    MA_CN = "7801",
                    MA_KH = "CUST001",
                    TEN_KH = "Nguyen Van A",
                    CCY = "USD",
                    CURRENT_BALANCE = 10000,
                    RATE = 2.1m,
                    SO_TAI_KHOAN = "ACC003",
                    OPENING_DATE = new DateTime(2025, 3, 1),
                    MATURITY_DATE = new DateTime(2026, 3, 1),
                    ADDRESS = "123 Nguyen Hue, Ho Chi Minh City",
                    CreatedAt = twoDaysAgo,
                    UpdatedAt = twoDaysAgo,
                    DataSource = "Test",
                    ImportDateTime = twoDaysAgo
                },
                new DP01
                {
                    Id = 4,
                    NGAY_DL = DateTime.Today.AddDays(-1),
                    MA_CN = "7801",
                    MA_KH = "CUST003",
                    TEN_KH = "Le Van C",
                    CCY = "VND",
                    CURRENT_BALANCE = 3000000,
                    RATE = 3.6m,
                    SO_TAI_KHOAN = "ACC004",
                    OPENING_DATE = new DateTime(2025, 4, 1),
                    MATURITY_DATE = new DateTime(2026, 4, 1),
                    ADDRESS = "789 Dong Khoi, Ho Chi Minh City",
                    CreatedAt = twoDaysAgo,
                    UpdatedAt = twoDaysAgo,
                    DataSource = "Test",
                    ImportDateTime = twoDaysAgo
                },
                new DP01
                {
                    Id = 5,
                    NGAY_DL = DateTime.Today.AddDays(-1),
                    MA_CN = "7800",
                    MA_KH = "CUST004",
                    TEN_KH = "Pham Thi D",
                    CCY = "VND",
                    CURRENT_BALANCE = 4000000,
                    RATE = 3.7m,
                    SO_TAI_KHOAN = "ACC005",
                    OPENING_DATE = new DateTime(2025, 5, 1),
                    MATURITY_DATE = new DateTime(2026, 5, 1),
                    ADDRESS = "101 Hai Ba Trung, Ho Chi Minh City",
                    CreatedAt = twoDaysAgo,
                    UpdatedAt = twoDaysAgo,
                    DataSource = "Test",
                    ImportDateTime = twoDaysAgo
                }
            };
        }
    }
}
