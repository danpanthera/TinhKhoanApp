using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Services;
using TinhKhoanApp.Api.Services.Interfaces;
using TinhKhoanApp.Api.Repositories.Interfaces;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DTOs.DP01;
using TinhKhoanApp.Api.Models.Common;
using Moq;

namespace TinhKhoanApp.Api.Tests.Services
{
    /// <summary>
    /// Unit tests for DP01Service - Architecture validation
    /// Ensures consistency across Migration ↔ Database ↔ Model ↔ EF ↔ BulkCopy ↔ Direct Import ↔ Services ↔ Repository ↔ DTO layers
    /// </summary>
    public class DP01ServiceTests : IDisposable
    {
        private readonly Mock<IDP01Repository> _mockRepository;
        private readonly Mock<IDirectImportService> _mockDirectImportService;
        private readonly Mock<ILogger<DP01Service>> _mockLogger;
        private readonly ApplicationDbContext _context;
        private readonly DP01Service _service;

        public DP01ServiceTests()
        {
            // Setup mocks
            _mockRepository = new Mock<IDP01Repository>();
            _mockDirectImportService = new Mock<IDirectImportService>();
            _mockLogger = new Mock<ILogger<DP01Service>>();

            // Setup in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Create service instance
            _service = new DP01Service(_mockRepository.Object, _context, _mockDirectImportService.Object, _mockLogger.Object);
        }

        #region Architecture Consistency Tests

        [Fact]
        public async Task GetPreviewAsync_Should_Return_Standardized_ApiResponse()
        {
            // Arrange
            var expectedPagedResult = new PagedResult<Models.DataModels.DP01>
            {
                Items = new List<Models.DataModels.DP01>(),
                TotalCount = 0,
                Page = 1,
                PageSize = 10,
                TotalPages = 0
            };

            _mockRepository.Setup(r => r.GetPagedAsync(1, 10, null))
                          .ReturnsAsync(expectedPagedResult);

            // Act
            var result = await _service.GetPreviewAsync(1, 10, null);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ApiResponse<PagedResult>DP01PreviewDto>>>>(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Page);
            Assert.Equal(10, result.Data.PageSize);
        }

        [Fact]
        public async Task GetPreviewAsync_Should_Map_DP01_To_DP01PreviewDto_Correctly()
        {
            // Arrange
            var testDP01 = CreateTestDP01();
            var pagedResult = new PagedResult<Models.DataModels.DP01>
            {
                Items = new List<Models.DataModels.DP01> { testDP01 },
                TotalCount = 1,
                Page = 1,
                PageSize = 10,
                TotalPages = 1
            };

            _mockRepository.Setup(r => r.GetPagedAsync(1, 10, null))
                          .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetPreviewAsync(1, 10, null);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data.Items);

            var previewDto = result.Data.Items.First();
            Assert.Equal(testDP01.Id, previewDto.Id);
            Assert.Equal(testDP01.NGAY_DL, previewDto.NGAY_DL);
            Assert.Equal(testDP01.DV_BC, previewDto.DV_BC);
            Assert.Equal(testDP01.MA_DV, previewDto.MA_DV);
            Assert.Equal(testDP01.TEN_DV, previewDto.TEN_DV);
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Use_DirectImportService_Correctly()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test_dp01.csv");
            mockFile.Setup(f => f.Length).Returns(1000);

            var expectedImportResult = new DirectImportResult
            {
                Success = true,
                Message = "Import successful",
                TotalRecords = 100,
                ProcessedRecords = 100,
                SuccessfulRecords = 100,
                FailedRecords = 0,
                ExecutionTime = TimeSpan.FromSeconds(5)
            };

            _mockDirectImportService.Setup(d => d.ImportDP01Async(mockFile.Object))
                                   .ReturnsAsync(expectedImportResult);

            // Act
            var result = await _service.ImportCsvAsync(mockFile.Object);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(100, result.Data.TotalRecords);
            Assert.Equal(100, result.Data.SuccessfulRecords);
            Assert.Equal(0, result.Data.FailedRecords);

            _mockDirectImportService.Verify(d => d.ImportDP01Async(mockFile.Object), Times.Once);
        }

        #endregion

        #region DTO Validation Tests

        [Fact]
        public void DP01PreviewDto_Should_Have_Correct_Properties()
        {
            // Arrange & Act
            var previewDto = new DP01PreviewDto();

            // Assert - Check that DTO has all required properties matching CSV structure
            var properties = typeof(DP01PreviewDto).GetProperties();
            var expectedProperties = new[]
            {
                "Id", "NGAY_DL", "DV_BC", "MA_DV", "TEN_DV", "LOAI_BC", "ND", "SO_TK", "TEN_TK",
                "CreatedAt", "UpdatedAt"
            };

            foreach (var expectedProp in expectedProperties)
            {
                Assert.Contains(properties, p => p.Name == expectedProp);
            }
        }

        [Fact]
        public void DP01CreateDto_Should_Have_All_63_Business_Columns()
        {
            // Arrange & Act
            var createDto = new DP01CreateDto();

            // Assert - Verify DTO has all 63 business columns from CSV
            var properties = typeof(DP01CreateDto).GetProperties();

            // Should have NGAY_DL + 62 other business columns
            Assert.True(properties.Length >= 63, $"DP01CreateDto should have at least 63 properties, but has {properties.Length}");

            // Key properties must exist
            Assert.Contains(properties, p => p.Name == "NGAY_DL");
            Assert.Contains(properties, p => p.Name == "DV_BC");
            Assert.Contains(properties, p => p.Name == "MA_DV");
            Assert.Contains(properties, p => p.Name == "TEN_DV");
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task GetPreviewAsync_Should_Handle_Repository_Exception()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>()))
                          .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _service.GetPreviewAsync(1, 10, null);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Failed to retrieve DP01 preview data", result.Message);
            Assert.NotEmpty(result.Errors);
            Assert.Contains("Database connection failed", result.Errors);
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Handle_DirectImportService_Exception()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("invalid.csv");

            _mockDirectImportService.Setup(d => d.ImportDP01Async(It.IsAny<IFormFile>()))
                                   .ThrowsAsync(new Exception("CSV parsing failed"));

            // Act
            var result = await _service.ImportCsvAsync(mockFile.Object);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Failed to import DP01 CSV file", result.Message);
            Assert.NotEmpty(result.Errors);
        }

        #endregion

        #region Service Interface Compliance Tests

        [Fact]
        public void DP01Service_Should_Implement_IDP01Service_Interface()
        {
            // Assert
            Assert.IsAssignableFrom<IDP01Service>(_service);
        }

        [Fact]
        public void IDP01Service_Should_Have_Standardized_Method_Signatures()
        {
            // Arrange
            var serviceInterface = typeof(IDP01Service);
            var methods = serviceInterface.GetMethods();

            // Assert - Check key method signatures
            var getPreviewMethod = methods.FirstOrDefault(m => m.Name == "GetPreviewAsync");
            Assert.NotNull(getPreviewMethod);
            Assert.Equal(typeof(Task<ApiResponse<PagedResult>DP01PreviewDto>>>>), getPreviewMethod.ReturnType);

            var importCsvMethod = methods.FirstOrDefault(m => m.Name == "ImportCsvAsync");
            Assert.NotNull(importCsvMethod);
            Assert.Equal(typeof(Task<ApiResponse<DP01ImportResultDto>>), importCsvMethod.ReturnType);

            var getSummaryMethod = methods.FirstOrDefault(m => m.Name == "GetSummaryAsync");
            Assert.NotNull(getSummaryMethod);
            Assert.Equal(typeof(Task<ApiResponse<DP01SummaryDto>>), getSummaryMethod.ReturnType);
        }

        #endregion

        #region Helper Methods

        private Models.DataModels.DP01 CreateTestDP01()
        {
            return new Models.DataModels.DP01
            {
                Id = 1,
                NGAY_DL = DateTime.Today,
                DV_BC = "TEST_DV",
                MA_DV = "001",
                TEN_DV = "Test Department",
                LOAI_BC = "01",
                ND = "Test Content",
                SO_TK = "1234567890",
                TEN_TK = "Test Account",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                SysStartTime = DateTime.UtcNow,
                SysEndTime = DateTime.MaxValue
            };
        }

        #endregion

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

namespace TinhKhoanApp.Api.Models.Common
{
    /// <summary>
    /// DirectImport operation result - matches DirectImportService output
    /// </summary>
    public class DirectImportResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public long TotalRecords { get; set; }
        public long ProcessedRecords { get; set; }
        public long SuccessfulRecords { get; set; }
        public long FailedRecords { get; set; }
        public List<string> Errors { get; set; } = new();
        public TimeSpan ExecutionTime { get; set; }
        public Dictionary<string, object> AdditionalInfo { get; set; } = new();
    }
}
