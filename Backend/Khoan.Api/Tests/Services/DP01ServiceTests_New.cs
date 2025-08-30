using Xunit;
using Moq;
using TinhKhoanApp.Api.Services;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.Dtos.DP01;

namespace TinhKhoanApp.Api.Tests.Services
{
    /// <summary>
    /// Unit tests cho DP01Service - đảm bảo business logic đúng với CSV structure
    /// Kiểm tra consistency: CSV ↔ Model ↔ Repository ↔ Service ↔ DTO
    /// </summary>
    public class DP01ServiceTests : IDisposable
    {
        private readonly Mock<IDP01Repository> _mockRepository;
        private readonly DP01Service _service;

        public DP01ServiceTests()
        {
            _mockRepository = new Mock<IDP01Repository>();
            _service = new DP01Service(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_DP01PreviewDto_Collection()
        {
            // Arrange - tạo test data với CSV business columns
            var testData = new List<DP01>
            {
                new DP01
                {
                    Id = 1,
                    NGAY_DL = DateTime.Now,
                    MA_CN = "001", // Business column từ CSV
                    TAI_KHOAN_HACH_TOAN = "1234567890", // Business column từ CSV
                    MA_KH = "KH001", // Business column từ CSV
                    TEN_KH = "Nguyen Van A", // Business column từ CSV
                    CURRENT_BALANCE = 1000000, // Business column từ CSV
                    SO_TAI_KHOAN = "0011001234567", // Business column từ CSV
                    CCY = "VND"
                },
                new DP01
                {
                    Id = 2,
                    NGAY_DL = DateTime.Now,
                    MA_CN = "002",
                    TAI_KHOAN_HACH_TOAN = "2234567890",
                    MA_KH = "KH002",
                    TEN_KH = "Tran Thi B",
                    CURRENT_BALANCE = 2000000,
                    SO_TAI_KHOAN = "0022001234567",
                    CCY = "VND"
                }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(testData);

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var firstItem = result.First();
            Assert.Equal(1, firstItem.Id);
            Assert.Equal("001", firstItem.MA_CN); // Kiểm tra business column mapping
            Assert.Equal("KH001", firstItem.MA_KH);
            Assert.Equal("Nguyen Van A", firstItem.TEN_KH);
            Assert.Equal(1000000, firstItem.CURRENT_BALANCE);
            Assert.Equal("0011001234567", firstItem.SO_TAI_KHOAN);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_DP01DetailsDto_When_Found()
        {
            // Arrange
            var testDp01 = new DP01
            {
                Id = 1,
                NGAY_DL = DateTime.Now,
                MA_CN = "001",
                TAI_KHOAN_HACH_TOAN = "1234567890",
                MA_KH = "KH001",
                TEN_KH = "Nguyen Van A",
                CURRENT_BALANCE = 1000000,
                SO_TAI_KHOAN = "0011001234567"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(testDp01);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("001", result.MA_CN); // Verify CSV business column
            Assert.Equal("KH001", result.MA_KH);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_Not_Found()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((DP01?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByBranchCodeAsync_Should_Return_Records_For_Branch()
        {
            // Arrange
            var testData = new List<DP01>
            {
                new DP01
                {
                    Id = 1,
                    MA_CN = "001",
                    MA_KH = "KH001",
                    CURRENT_BALANCE = 1000000
                },
                new DP01
                {
                    Id = 2,
                    MA_CN = "001",
                    MA_KH = "KH002",
                    CURRENT_BALANCE = 2000000
                }
            };

            _mockRepository.Setup(r => r.GetByBranchCodeAsync("001", 100)).ReturnsAsync(testData);

            // Act
            var result = await _service.GetByBranchCodeAsync("001", 100);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, item => Assert.Equal("001", item.MA_CN));
        }

        [Fact]
        public async Task GetTotalBalanceByBranchAsync_Should_Return_Correct_Total()
        {
            // Arrange
            var expectedBalance = 5000000m;
            _mockRepository.Setup(r => r.GetTotalBalanceByBranchAsync("001", null))
                          .ReturnsAsync(expectedBalance);

            // Act
            var result = await _service.GetTotalBalanceByBranchAsync("001");

            // Assert
            Assert.Equal(expectedBalance, result);
        }

        [Fact]
        public async Task GetStatisticsAsync_Should_Return_Valid_Statistics()
        {
            // Arrange
            var testData = new List<DP01>
            {
                new DP01 { MA_CN = "001", MA_KH = "KH001", CURRENT_BALANCE = 1000000 },
                new DP01 { MA_CN = "002", MA_KH = "KH002", CURRENT_BALANCE = 2000000 },
                new DP01 { MA_CN = "001", MA_KH = "KH003", CURRENT_BALANCE = 3000000 }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(testData);

            // Act
            var result = await _service.GetStatisticsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.TotalRecords);
            Assert.Equal(6000000m, result.TotalBalance);
            Assert.Equal(2000000m, result.AverageBalance);
        }

        [Fact]
        public async Task CreateAsync_Should_Create_New_DP01_Record()
        {
            // Arrange
            var createDto = new DP01CreateDto
            {
                MA_CN = "001",
                TAI_KHOAN_HACH_TOAN = "1234567890",
                MA_KH = "KH001",
                TEN_KH = "Nguyen Van A",
                CURRENT_BALANCE = 1000000,
                SO_TAI_KHOAN = "0011001234567"
            };

            var createdDp01 = new DP01 { Id = 1 };
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<DP01>())).Returns(Task.CompletedTask);
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<DP01>()), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Record_When_Exists()
        {
            // Arrange
            var existingDp01 = new DP01 { Id = 1, MA_CN = "001" };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingDp01);
            _mockRepository.Setup(r => r.Remove(existingDp01));
            _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.Remove(existingDp01), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_Should_Return_False_When_Not_Found()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((DP01?)null);

            // Act
            var result = await _service.DeleteAsync(999);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.Remove(It.IsAny<DP01>()), Times.Never);
        }

        public void Dispose()
        {
            // Cleanup if needed
        }
    }
}
