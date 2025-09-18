using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Khoan.Api.Services;
using Khoan.Api.Services.Interfaces;
using Khoan.Api.Repositories.Interfaces;
using Khoan.Api.Data;
using Khoan.Api.Models.Dtos.GL02;
using Khoan.Api.Models.Common;
using Khoan.Api.Models.DataTables;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Khoan.Api.Tests.Services
{
    /// <summary>
    /// Unit tests for GL02Service - Architecture validation
    /// Ensures consistency across Migration ↔ Database ↔ Model ↔ EF ↔ BulkCopy ↔ Direct Import ↔ Services ↔ Repository ↔ DTO layers
    /// GL02: Partitioned Columnstore (NO temporal features), 17 business columns, NGAY_DL from TRDATE column
    /// </summary>
    public class GL02ServiceTests : IDisposable
    {
        private readonly Mock<IGL02Repository> _mockRepository;
        private readonly Mock<ICsvParsingService> _mockCsvParsingService;
        private readonly Mock<ILogger<GL02Service>> _mockLogger;
        private readonly ApplicationDbContext _context;
        private readonly GL02Service _service;

        public GL02ServiceTests()
        {
            // Setup mocks - Thiết lập mock objects cho dependencies
            _mockRepository = new Mock<IGL02Repository>();
            _mockCsvParsingService = new Mock<ICsvParsingService>();
            _mockLogger = new Mock<ILogger<GL02Service>>();

            // Setup in-memory database - Thiết lập database trong memory cho testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Create service instance - Tạo instance của GL02Service với mocked dependencies
            _service = new GL02Service(_mockRepository.Object, _context, _mockCsvParsingService.Object, _mockLogger.Object);
        }

        #region Architecture Consistency Tests

        [Fact]
        public async Task GetPreviewAsync_Should_Return_Standardized_ApiResponse()
        {
            // Arrange - Thiết lập dữ liệu test
            var expectedPagedResult = new PagedResult<GL02>
            {
                Items = new List<GL02>(),
                TotalCount = 0,
                Page = 1,
                PageSize = 10,
                TotalPages = 0
            };

            _mockRepository.Setup(r => r.GetPagedAsync(1, 10, null))
                          .ReturnsAsync(expectedPagedResult);

            // Act - Thực thi method cần test
            var result = await _service.GetPreviewAsync(1, 10, null);

            // Assert - Kiểm tra kết quả
            Assert.NotNull(result);
            Assert.IsType<ApiResponse<PagedResult<GL02PreviewDto>>>(result);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Page);
            Assert.Equal(10, result.Data.PageSize);
        }

        [Fact]
        public async Task GetPreviewAsync_Should_Map_GL02_To_GL02PreviewDto_Correctly()
        {
            // Arrange - Thiết lập test data với GL02 entity
            var testGL02 = CreateTestGL02();
            var pagedResult = new PagedResult<GL02>
            {
                Items = new List<GL02> { testGL02 },
                TotalCount = 1,
                Page = 1,
                PageSize = 10,
                TotalPages = 1
            };

            _mockRepository.Setup(r => r.GetPagedAsync(1, 10, null))
                          .ReturnsAsync(pagedResult);

            // Act - Gọi service method
            var result = await _service.GetPreviewAsync(1, 10, null);

            // Assert - Kiểm tra mapping từ GL02 entity sang GL02PreviewDto
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data.Items);

            var previewDto = result.Data.Items.First();
            Assert.Equal(testGL02.Id, previewDto.Id);
            Assert.Equal(testGL02.NGAY_DL.Date, previewDto.TRDATE?.Date);
            Assert.Equal(testGL02.TRBRCD, previewDto.TRBRCD);
            Assert.Equal(testGL02.USERID, previewDto.USERID);
            Assert.Equal(testGL02.LOCAC, previewDto.LOCAC);
            Assert.Equal(testGL02.CCY, previewDto.CCY);
            Assert.Equal(testGL02.TRCD, previewDto.TRCD);
            Assert.Equal(testGL02.CUSTOMER, previewDto.CUSTOMER);
            Assert.Equal(testGL02.DRAMOUNT, previewDto.DRAMOUNT);
            Assert.Equal(testGL02.CRAMOUNT, previewDto.CRAMOUNT);
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Use_CsvParsingService_Correctly()
        {
            // Arrange - Thiết lập mock file và expected result
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test_gl02.csv");
            mockFile.Setup(f => f.Length).Returns(1000);

            var expectedImportResult = new GL02ImportResultDto
            {
                Success = true,
                Message = "Import successful",
                TotalRecords = 100,
                ProcessedRecords = 100,
                SuccessfulRecords = 100,
                FailedRecords = 0,
                ExecutionTime = TimeSpan.FromSeconds(5),
                ImportedAt = DateTime.UtcNow
            };

            _mockCsvParsingService.Setup(c => c.ImportGL02CsvAsync(mockFile.Object))
                                 .ReturnsAsync(expectedImportResult);

            // Act - Thực hiện import
            var result = await _service.ImportCsvAsync(mockFile.Object);

            // Assert - Kiểm tra kết quả import
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(100, result.Data.TotalRecords);
            Assert.Equal(100, result.Data.SuccessfulRecords);
            Assert.Equal(0, result.Data.FailedRecords);

            // Verify rằng CsvParsingService được gọi đúng cách
            _mockCsvParsingService.Verify(c => c.ImportGL02CsvAsync(mockFile.Object), Times.Once);
        }

        #endregion

        #region GL02-Specific Architecture Tests

        [Fact]
        public void GL02_Should_Use_Partitioned_Columnstore_Architecture()
        {
            // Arrange & Assert - GL02 should NOT have temporal properties (unlike DP01, DPDA, etc.)
            var gl02Type = typeof(GL02);
            var properties = gl02Type.GetProperties();

            // GL02 should NOT have SysStartTime/SysEndTime (temporal properties) in the entity
            Assert.DoesNotContain(properties, p => p.Name == "SysStartTime");
            Assert.DoesNotContain(properties, p => p.Name == "SysEndTime");

            // But should have basic system columns
            Assert.Contains(properties, p => p.Name == "Id");
            Assert.Contains(properties, p => p.Name == "CREATED_DATE");
            Assert.Contains(properties, p => p.Name == "UPDATED_DATE");
            Assert.Contains(properties, p => p.Name == "FILE_NAME");
        }

        [Fact]
        public void GL02_Should_Have_NGAY_DL_From_TRDATE_Mapping()
        {
            // Arrange - Tạo GL02 entity
            var gl02 = new GL02();

            // Act - Test TRDATE to NGAY_DL conversion
            gl02.TRDATE = "20250810"; // yyyyMMdd format from CSV

            // Assert - Kiểm tra conversion đúng
            Assert.Equal(new DateTime(2025, 8, 10), gl02.NGAY_DL);
        }

        [Fact]
        public void GL02_Should_Have_17_Business_Columns()
        {
            // Arrange & Act - Lấy properties của GL02 entity
            var gl02Type = typeof(GL02);
            var properties = gl02Type.GetProperties();

            // Assert - Check all 17 business columns from CSV structure
            var expectedBusinessColumns = new[]
            {
                "TRBRCD", "USERID", "JOURSEQ", "DYTRSEQ", "LOCAC", "CCY", "BUSCD", 
                "UNIT", "TRCD", "CUSTOMER", "TRTP", "REFERENCE", "REMARK", "DRAMOUNT", "CRAMOUNT", "CRTDTM"
            };

            foreach (var expectedColumn in expectedBusinessColumns)
            {
                Assert.Contains(properties, p => p.Name == expectedColumn,
                    $"GL02 entity should have business column {expectedColumn}");
            }

            // Plus NGAY_DL makes 17 total business columns
            Assert.Contains(properties, p => p.Name == "NGAY_DL");
        }

        [Fact]
        public void GL02CreateDto_Should_Have_All_17_Business_Columns()
        {
            // Arrange & Act - Kiểm tra GL02CreateDto properties
            var createDto = new GL02CreateDto();
            var properties = typeof(GL02CreateDto).GetProperties();

            // Assert - Verify DTO has all 17 business columns from CSV
            Assert.True(properties.Length >= 17, $"GL02CreateDto should have at least 17 properties, but has {properties.Length}");

            // Key properties must exist matching CSV structure: TRDATE,TRBRCD,USERID,JOURSEQ,DYTRSEQ,LOCAC,CCY,BUSCD,UNIT,TRCD,CUSTOMER,TRTP,REFERENCE,REMARK,DRAMOUNT,CRAMOUNT,CRTDTM
            var expectedBusinessColumns = new[]
            {
                "TRDATE", "TRBRCD", "USERID", "JOURSEQ", "DYTRSEQ", "LOCAC", "CCY", "BUSCD", 
                "UNIT", "TRCD", "CUSTOMER", "TRTP", "REFERENCE", "REMARK", "DRAMOUNT", "CRAMOUNT", "CRTDTM"
            };

            foreach (var expectedColumn in expectedBusinessColumns)
            {
                Assert.Contains(properties, p => p.Name == expectedColumn, 
                    $"GL02CreateDto should have property {expectedColumn}");
            }
        }

        [Fact]
        public void GL02SummaryDto_Should_Have_Financial_Analysis_Properties()
        {
            // Arrange & Act - Kiểm tra GL02SummaryDto properties
            var summaryDto = new GL02SummaryDto();
            var properties = typeof(GL02SummaryDto).GetProperties();
            
            // Assert - Check financial analysis properties
            var expectedSummaryProperties = new[]
            {
                "TrDate", "TotalRecords", "TotalDebitAmount", "TotalCreditAmount", "NetAmount",
                "AmountByCurrency", "RecordsByBranch", "RecordsByTransactionType", 
                "DebitAmountByAccount", "CreditAmountByAccount", "GeneratedAt"
            };

            foreach (var expectedProp in expectedSummaryProperties)
            {
                Assert.Contains(properties, p => p.Name == expectedProp,
                    $"GL02SummaryDto should have property {expectedProp}");
            }
        }

        [Fact]
        public void GL02ImportResultDto_Should_Have_Import_Tracking_Properties()
        {
            // Arrange & Act - Kiểm tra GL02ImportResultDto properties
            var importResultDto = new GL02ImportResultDto();
            var properties = typeof(GL02ImportResultDto).GetProperties();
            
            // Assert - Check import result properties
            var expectedImportProperties = new[]
            {
                "Success", "Message", "TotalRecords", "ProcessedRecords", "SuccessfulRecords", 
                "FailedRecords", "Errors", "ExecutionTime", "ImportedAt", "ImportStatistics"
            };

            foreach (var expectedProp in expectedImportProperties)
            {
                Assert.Contains(properties, p => p.Name == expectedProp,
                    $"GL02ImportResultDto should have property {expectedProp}");
            }
        }

        #endregion

        #region Error Handling Tests

        [Fact]
        public async Task GetPreviewAsync_Should_Handle_Repository_Exception()
        {
            // Arrange - Setup repository để throw exception
            _mockRepository.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime?>()))
                          .ThrowsAsync(new Exception("Database connection failed"));

            // Act - Gọi service method
            var result = await _service.GetPreviewAsync(1, 10, null);

            // Assert - Kiểm tra error handling
            Assert.False(result.Success);
            Assert.Contains("Failed to retrieve GL02 preview data", result.Message);
            Assert.NotEmpty(result.Errors);
            Assert.Contains("Database connection failed", result.Errors);
        }

        [Fact]
        public async Task ImportCsvAsync_Should_Handle_CsvParsingService_Exception()
        {
            // Arrange - Setup CSV parsing service để throw exception
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("invalid.csv");

            _mockCsvParsingService.Setup(c => c.ImportGL02CsvAsync(It.IsAny<IFormFile>()))
                                 .ThrowsAsync(new Exception("CSV parsing failed"));

            // Act - Thực hiện import với invalid file
            var result = await _service.ImportCsvAsync(mockFile.Object);

            // Assert - Kiểm tra error handling
            Assert.False(result.Success);
            Assert.Contains("Failed to import GL02 CSV file", result.Message);
            Assert.NotEmpty(result.Errors);
        }

        #endregion

        #region Service Interface Compliance Tests

        [Fact]
        public void GL02Service_Should_Implement_IGL02Service_Interface()
        {
            // Assert - Kiểm tra GL02Service implements IGL02Service
            Assert.IsAssignableFrom<IGL02Service>(_service);
        }

        [Fact]
        public void IGL02Service_Should_Have_Standardized_Method_Signatures()
        {
            // Arrange - Lấy interface methods
            var serviceInterface = typeof(IGL02Service);
            var methods = serviceInterface.GetMethods();

            // Assert - Check key method signatures
            var getPreviewMethod = methods.FirstOrDefault(m => m.Name == "GetPreviewAsync");
            Assert.NotNull(getPreviewMethod);
            Assert.Equal(typeof(Task<ApiResponse<PagedResult<GL02PreviewDto>>>), getPreviewMethod.ReturnType);

            var importCsvMethod = methods.FirstOrDefault(m => m.Name == "ImportCsvAsync");
            Assert.NotNull(importCsvMethod);
            Assert.Equal(typeof(Task<ApiResponse<GL02ImportResultDto>>), importCsvMethod.ReturnType);

            var getSummaryMethod = methods.FirstOrDefault(m => m.Name == "GetSummaryAsync");
            Assert.NotNull(getSummaryMethod);
            Assert.Equal(typeof(Task<ApiResponse<GL02SummaryDto>>), getSummaryMethod.ReturnType);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Tạo GL02 test record với cấu trúc hoàn chỉnh
        /// Partitioned Columnstore: 17 business columns + system columns (NO temporal)
        /// CSV Structure: TRDATE,TRBRCD,USERID,JOURSEQ,DYTRSEQ,LOCAC,CCY,BUSCD,UNIT,TRCD,CUSTOMER,TRTP,REFERENCE,REMARK,DRAMOUNT,CRAMOUNT,CRTDTM
        /// </summary>
        private GL02 CreateTestGL02()
        {
            return new GL02
            {
                Id = 1,
                NGAY_DL = DateTime.Today,
                TRBRCD = "001", // Branch code
                USERID = "USER001", // User ID
                JOURSEQ = "JOU001", // Journal sequence
                DYTRSEQ = "DT001", // Daily transaction sequence
                LOCAC = "LOC001", // Location account
                CCY = "VND", // Currency
                BUSCD = "BUS001", // Business code
                UNIT = "UNIT001", // Unit
                TRCD = "TR001", // Transaction code
                CUSTOMER = "CUST001", // Customer
                TRTP = "01", // Transaction type
                REFERENCE = "REF001", // Reference
                REMARK = "Test GL02 transaction", // Remark
                DRAMOUNT = 1000.50m, // Debit amount
                CRAMOUNT = 500.25m, // Credit amount
                CRTDTM = DateTime.Now, // Created datetime
                CREATED_DATE = DateTime.UtcNow,
                UPDATED_DATE = DateTime.UtcNow,
                FILE_NAME = "test_gl02_20250810.csv"
            };
        }

        #endregion

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
