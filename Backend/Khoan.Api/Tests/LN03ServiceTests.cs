using Microsoft.EntityFrameworkCore;
using Moq;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.Dtos;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Services;
using Xunit;

namespace TinhKhoanApp.Api.Tests
{
    /// <summary>
    /// Unit Test cho LN03Service
    /// </summary>
    public class LN03ServiceTests
    {
        private readonly Mock<ILN03Repository> _mockRepository;
        private readonly ILN03Service _service;

        public LN03ServiceTests()
        {
            _mockRepository = new Mock<ILN03Repository>();
            _service = new LN03Service(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllLN03Records()
        {
            // Arrange
            var testData = new List<LN03>
            {
                new LN03 { Id = 1, MACHINHANH = "7808", MAKH = "CUST001", SOHOPDONG = "HD001", SOTIENXLRR = "1000", NGAY_DL = DateTime.Now },
                new LN03 { Id = 2, MACHINHANH = "7808", MAKH = "CUST002", SOHOPDONG = "HD002", SOTIENXLRR = "2000", NGAY_DL = DateTime.Now }
            };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(testData);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("7808", result.First().MaChiNhanh);
            Assert.Equal("CUST001", result.First().MaKH);
            Assert.Equal("HD001", result.First().SoHopDong);
            Assert.Equal("1000", result.First().SoTienXLRR);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnLN03Record()
        {
            // Arrange
            var testRecord = new LN03
            {
                Id = 1,
                MACHINHANH = "7808",
                MAKH = "CUST001",
                SOHOPDONG = "HD001",
                SOTIENXLRR = "1000",
                NGAY_DL = DateTime.Now
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(testRecord);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("7808", result.MaChiNhanh);
            Assert.Equal("CUST001", result.MaKH);
            Assert.Equal("HD001", result.SoHopDong);
            Assert.Equal("1000", result.SoTienXLRR);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((LN03)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByBranchCodeAsync_ShouldReturnBranchRecords()
        {
            // Arrange
            var testData = new List<LN03>
            {
                new LN03 { Id = 1, MACHINHANH = "7808", MAKH = "CUST001", SOHOPDONG = "HD001", SOTIENXLRR = "1000", NGAY_DL = DateTime.Now },
                new LN03 { Id = 2, MACHINHANH = "7808", MAKH = "CUST002", SOHOPDONG = "HD002", SOTIENXLRR = "2000", NGAY_DL = DateTime.Now }
            };

            _mockRepository.Setup(repo => repo.GetByBranchCodeAsync("7808", 100))
                .ReturnsAsync(testData);

            // Act
            var result = await _service.GetByBranchCodeAsync("7808", 100);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal("7808", item.MaChiNhanh));
        }

        [Fact]
        public async Task GetTotalRiskAmountByBranchAsync_ShouldReturnCorrectTotal()
        {
            // Arrange
            decimal expectedTotal = 3000;
            _mockRepository.Setup(repo => repo.GetTotalRiskAmountByBranchAsync("7808", null))
                .ReturnsAsync(expectedTotal);

            // Act
            var result = await _service.GetTotalRiskAmountByBranchAsync("7808", null);

            // Assert
            Assert.Equal(expectedTotal, result);
        }

        [Fact]
        public async Task GetTotalDebtRecoveryByBranchAsync_ShouldReturnCorrectTotal()
        {
            // Arrange
            decimal expectedTotal = 1500;
            _mockRepository.Setup(repo => repo.GetTotalDebtRecoveryByBranchAsync("7808", null))
                .ReturnsAsync(expectedTotal);

            // Act
            var result = await _service.GetTotalDebtRecoveryByBranchAsync("7808", null);

            // Assert
            Assert.Equal(expectedTotal, result);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedLN03()
        {
            // Arrange
            var createDto = new CreateLN03DTO
            {
                MaChiNhanh = "7808",
                MaKH = "CUST001",
                SoHopDong = "HD001",
                SoTienXLRR = "1000",
                NgayDL = DateTime.Now
            };

            var createdEntity = new LN03
            {
                Id = 1,
                MACHINHANH = "7808",
                MAKH = "CUST001",
                SOHOPDONG = "HD001",
                SOTIENXLRR = "1000",
                NGAY_DL = DateTime.Now,
                CREATED_DATE = DateTime.Now,
                UPDATED_DATE = DateTime.Now
            };

            _mockRepository.Setup(repo => repo.AddAsync(It.IsAny<LN03>()))
                .ReturnsAsync(createdEntity);

            _mockRepository.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("7808", result.MaChiNhanh);
            Assert.Equal("CUST001", result.MaKH);
            Assert.Equal("HD001", result.SoHopDong);
            Assert.Equal("1000", result.SoTienXLRR);
        }

        [Fact]
        public async Task UpdateAsync_WithValidId_ShouldReturnUpdatedLN03()
        {
            // Arrange
            var existingEntity = new LN03
            {
                Id = 1,
                MACHINHANH = "7808",
                MAKH = "CUST001",
                SOHOPDONG = "HD001",
                SOTIENXLRR = "1000",
                THUNOSAUXL = "500",
                NGAY_DL = DateTime.Now,
                CREATED_DATE = DateTime.Now,
                UPDATED_DATE = DateTime.Now
            };

            var updateDto = new UpdateLN03DTO
            {
                SoTienXLRR = "2000",
                ThuNoSauXL = "1000"
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingEntity);

            _mockRepository.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.UpdateAsync(1, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("2000", result.SoTienXLRR);
            Assert.Equal("1000", result.ThuNoSauXL);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            var existingEntity = new LN03
            {
                Id = 1,
                MACHINHANH = "7808",
                MAKH = "CUST001",
                SOHOPDONG = "HD001",
                SOTIENXLRR = "1000",
                NGAY_DL = DateTime.Now,
                CREATED_DATE = DateTime.Now,
                UPDATED_DATE = DateTime.Now
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingEntity);

            _mockRepository.Setup(repo => repo.DeleteAsync(existingEntity))
                .Returns(Task.CompletedTask);

            _mockRepository.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((LN03)null);

            // Act
            var result = await _service.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}
