using Microsoft.EntityFrameworkCore;
using Moq;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Services;
using Xunit;

namespace TinhKhoanApp.Api.Tests
{
    /// <summary>
    /// Unit Test cho LN01Service
    /// </summary>
    public class LN01ServiceTests
    {
        private readonly Mock<ILN01Repository> _mockRepository;
        private readonly ILN01Service _service;

        public LN01ServiceTests()
        {
            _mockRepository = new Mock<ILN01Repository>();
            _service = new LN01Service(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllLN01Records()
        {
            // Arrange
            var testData = new List<LN01>
            {
                new LN01 { Id = 1, BRCD = "7808", CUSTSEQ = "CUST001", TAI_KHOAN = "ACC001", DU_NO_THUC_TE = 1000, NGAY_DL = DateTime.Now },
                new LN01 { Id = 2, BRCD = "7808", CUSTSEQ = "CUST002", TAI_KHOAN = "ACC002", DU_NO_THUC_TE = 2000, NGAY_DL = DateTime.Now }
            };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(testData);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("7808", result.First().BranchCode);
            Assert.Equal("CUST001", result.First().CustomerCode);
            Assert.Equal(1000, result.First().DuNoThucTe);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnLN01Record()
        {
            // Arrange
            var testRecord = new LN01
            {
                Id = 1,
                BRCD = "7808",
                CUSTSEQ = "CUST001",
                TAI_KHOAN = "ACC001",
                DU_NO_THUC_TE = 1000,
                NGAY_DL = DateTime.Now
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(testRecord);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("7808", result.BranchCode);
            Assert.Equal("CUST001", result.CustomerCode);
            Assert.Equal(1000, result.DuNoThucTe);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(999))
                .ReturnsAsync((LN01)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByBranchCodeAsync_ShouldReturnBranchRecords()
        {
            // Arrange
            var testData = new List<LN01>
            {
                new LN01 { Id = 1, BRCD = "7808", CUSTSEQ = "CUST001", TAI_KHOAN = "ACC001", DU_NO_THUC_TE = 1000, NGAY_DL = DateTime.Now },
                new LN01 { Id = 2, BRCD = "7808", CUSTSEQ = "CUST002", TAI_KHOAN = "ACC002", DU_NO_THUC_TE = 2000, NGAY_DL = DateTime.Now }
            };

            _mockRepository.Setup(repo => repo.GetByBranchCodeAsync("7808", 100))
                .ReturnsAsync(testData);

            // Act
            var result = await _service.GetByBranchCodeAsync("7808", 100);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal("7808", item.BranchCode));
        }

        [Fact]
        public async Task GetTotalLoanAmountByBranchAsync_ShouldReturnCorrectTotal()
        {
            // Arrange
            decimal expectedTotal = 3000;
            _mockRepository.Setup(repo => repo.GetTotalLoanAmountByBranchAsync("7808", null))
                .ReturnsAsync(expectedTotal);

            // Act
            var result = await _service.GetTotalLoanAmountByBranchAsync("7808", null);

            // Assert
            Assert.Equal(expectedTotal, result);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreatedLN01()
        {
            // Arrange
            var createDto = new CreateLN01DTO
            {
                BranchCode = "7808",
                CustomerCode = "CUST001",
                TaiKhoan = "ACC001",
                Currency = "VND",
                DuNoThucTe = 1000,
                NgayDL = DateTime.Now
            };

            var createdEntity = new LN01
            {
                Id = 1,
                BRCD = "7808",
                CUSTSEQ = "CUST001",
                TAI_KHOAN = "ACC001",
                CCY = "VND",
                DU_NO_THUC_TE = 1000,
                NGAY_DL = DateTime.Now,
                CREATED_DATE = DateTime.Now
            };

            _mockRepository.Setup(repo => repo.AddAsync(It.IsAny<LN01>()))
                .ReturnsAsync(createdEntity);

            _mockRepository.Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("7808", result.BranchCode);
            Assert.Equal("CUST001", result.CustomerCode);
            Assert.Equal(1000, result.DuNoThucTe);
        }

        [Fact]
        public async Task UpdateAsync_WithValidId_ShouldReturnUpdatedLN01()
        {
            // Arrange
            var existingEntity = new LN01
            {
                Id = 1,
                BRCD = "7808",
                CUSTSEQ = "CUST001",
                TAI_KHOAN = "ACC001",
                CCY = "VND",
                DU_NO_THUC_TE = 1000,
                NGAY_DL = DateTime.Now,
                CREATED_DATE = DateTime.Now
            };

            var updateDto = new UpdateLN01DTO
            {
                DuNoThucTe = 2000,
                NhomNo = "2"
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
            Assert.Equal(2000, result.DuNoThucTe);
            Assert.Equal("2", result.NhomNo);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
        {
            // Arrange
            var existingEntity = new LN01
            {
                Id = 1,
                BRCD = "7808",
                CUSTSEQ = "CUST001",
                TAI_KHOAN = "ACC001",
                CCY = "VND",
                DU_NO_THUC_TE = 1000,
                NGAY_DL = DateTime.Now,
                CREATED_DATE = DateTime.Now
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
                .ReturnsAsync((LN01)null);

            // Act
            var result = await _service.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}
