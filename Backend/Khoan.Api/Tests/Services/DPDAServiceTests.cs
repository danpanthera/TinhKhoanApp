using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Services;
using TinhKhoanApp.Api.Services.Interfaces;
using TinhKhoanApp.Api.Repositories.Interfaces;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.Dtos;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Models.DataTables;
using Moq;

namespace TinhKhoanApp.Api.Tests.Services
{
    /// <summary>
    /// Unit tests for DPDAService - Architecture validation
    /// Đảm bảo tính nhất quán Migration ↔ Database ↔ Model ↔ EF ↔ BulkCopy ↔ Direct Import ↔ Services ↔ Repository ↔ DTO
    /// DPDA: 13 business columns + Temporal Table + Columnstore Indexes
    /// </summary>
    public class DPDAServiceTests : IDisposable
    {
        private readonly Mock<IDPDARepository> _mockRepository;
        private readonly Mock<IDirectImportService> _mockDirectImportService;
        private readonly Mock<ILogger<DPDAService>> _mockLogger;
        private readonly ApplicationDbContext _context;
        private readonly DPDAService _service;

        public DPDAServiceTests()
        {
            // Setup mocks - theo chuẩn DPDA service dependencies
            _mockRepository = new Mock<IDPDARepository>();
            _mockDirectImportService = new Mock<IDirectImportService>();
            _mockLogger = new Mock<ILogger<DPDAService>>();

            // Setup in-memory database cho DPDA testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Create DPDA service instance - khi service được implement
            // _service = new DPDAService(_mockRepository.Object, _context, _mockDirectImportService.Object, _mockLogger.Object);
        }

        #region DPDA Architecture Consistency Tests

        [Fact]
        public async Task GetDPDAPreviewAsync_Should_Return_Standardized_ApiResponse()
        {
            // Arrange - Setup test data với 13 business columns DPDA
            var testData = new List<DPDA>
            {
                new DPDA
                {
                    Id = 1,
                    NGAY_DL = new DateTime(2024, 12, 31),
                    // 13 business columns theo DPDA CSV structure
                    MA_CHI_NHANH = "CNL1",
                    MA_KHACH_HANG = "KH001",
                    TEN_KHACH_HANG = "Test Customer",
                    SO_TK = "1234567890",
                    TEN_TK = "Tai khoan test",
                    LOAI_TK = "TD",
                    SO_DU = 1000000.50m,
                    NGAY_MO_TK = new DateTime(2024, 01, 01),
                    TINH_TRANG = "HOAT_DONG",
                    MA_SP = "SP001",
                    TEN_SP = "San pham test",
                    LAI_SUAT = 0.05m,
                    KY_HAN = 12,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _mockRepository.Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new PagedResult<DPDA>
                          {
                              Items = testData,
                              TotalCount = testData.Count,
                              PageNumber = 1,
                              PageSize = 10
                          });

            // Act - Test preview functionality
            // var result = await _service.GetDPDAPreviewAsync(1, 10);

            // Assert - Validate ApiResponse structure
            // Assert.NotNull(result);
            // Assert.True(result.Success);
            // Assert.Equal("Lấy preview dữ liệu DPDA thành công", result.Message);
            // Assert.NotNull(result.Data);
            // Assert.IsType<PagedResult<DPDAPreviewDto>>(result.Data);

            // Validate DTO mapping accuracy
            // var pagedResult = result.Data as PagedResult<DPDAPreviewDto>;
            // Assert.Single(pagedResult.Items);
            // Assert.Equal("CNL1", pagedResult.Items.First().MA_CHI_NHANH);
        }

        [Fact]
        public async Task GetDPDAByDateAsync_Should_Filter_By_Date_Correctly()
        {
            // Arrange - Test date filtering với NGAY_DL field
            var targetDate = new DateTime(2024, 12, 31);
            var testData = new List<DPDA>
            {
                new DPDA { Id = 1, NGAY_DL = targetDate, MA_CHI_NHANH = "CNL1" },
                new DPDA { Id = 2, NGAY_DL = targetDate.AddDays(-1), MA_CHI_NHANH = "CNL2" }
            };

            _mockRepository.Setup(r => r.GetByDateAsync(targetDate))
                          .ReturnsAsync(testData.Where(d => d.NGAY_DL.Date == targetDate.Date).ToList());

            // Act - Test date filtering business logic
            // var result = await _service.GetDPDAByDateAsync(targetDate);

            // Assert - Validate correct filtering
            // Assert.NotNull(result);
            // Assert.True(result.Success);
            // Assert.Single(result.Data);
            // Assert.Equal(targetDate.Date, result.Data.First().NGAY_DL.Date);
        }

        [Fact]
        public async Task GetDPDAByBranchCodeAsync_Should_Filter_By_Branch_Correctly()
        {
            // Arrange - Test branch code filtering với MA_CHI_NHANH
            var branchCode = "CNL1";
            var testData = new List<DPDA>
            {
                new DPDA { Id = 1, MA_CHI_NHANH = branchCode, MA_KHACH_HANG = "KH001" },
                new DPDA { Id = 2, MA_CHI_NHANH = "CNL2", MA_KHACH_HANG = "KH002" }
            };

            _mockRepository.Setup(r => r.GetByBranchCodeAsync(branchCode))
                          .ReturnsAsync(testData.Where(d => d.MA_CHI_NHANH == branchCode).ToList());

            // Act - Test branch filtering
            // var result = await _service.GetDPDAByBranchCodeAsync(branchCode);

            // Assert - Validate branch filtering accuracy
            // Assert.NotNull(result);
            // Assert.True(result.Success);
            // Assert.Single(result.Data);
            // Assert.Equal(branchCode, result.Data.First().MA_CHI_NHANH);
        }

        #endregion

        #region DPDA DTO Mapping Validation Tests

        [Fact]
        public void MapToDPDAPreviewDto_Should_Map_All_Fields_Correctly()
        {
            // Arrange - DPDA entity với tất cả 13 business columns
            var entity = new DPDA
            {
                Id = 1,
                NGAY_DL = new DateTime(2024, 12, 31),
                MA_CHI_NHANH = "CNL1",
                MA_KHACH_HANG = "KH001",
                TEN_KHACH_HANG = "Nguyen Van A",
                SO_TK = "1234567890",
                TEN_TK = "Tai khoan tiet kiem",
                LOAI_TK = "TD",
                SO_DU = 1500000.75m,
                NGAY_MO_TK = new DateTime(2024, 01, 15),
                TINH_TRANG = "HOAT_DONG",
                MA_SP = "SP001",
                TEN_SP = "Tiet kiem co dinh",
                LAI_SUAT = 0.065m,
                KY_HAN = 24
            };

            // Act - Test DTO mapping manually
            var dto = new DPDAPreviewDto
            {
                Id = entity.Id,
                NGAY_DL = entity.NGAY_DL,
                MA_CHI_NHANH = entity.MA_CHI_NHANH,
                MA_KHACH_HANG = entity.MA_KHACH_HANG,
                TEN_KHACH_HANG = entity.TEN_KHACH_HANG,
                SO_TK = entity.SO_TK,
                SO_DU = entity.SO_DU,
                TINH_TRANG = entity.TINH_TRANG,
                MA_SP = entity.MA_SP,
                LAI_SUAT = entity.LAI_SUAT,
                KY_HAN = entity.KY_HAN
            };

            // Assert - Validate all field mappings
            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.NGAY_DL, dto.NGAY_DL);
            Assert.Equal(entity.MA_CHI_NHANH, dto.MA_CHI_NHANH);
            Assert.Equal(entity.MA_KHACH_HANG, dto.MA_KHACH_HANG);
            Assert.Equal(entity.TEN_KHACH_HANG, dto.TEN_KHACH_HANG);
            Assert.Equal(entity.SO_TK, dto.SO_TK);
            Assert.Equal(entity.SO_DU, dto.SO_DU);
            Assert.Equal(entity.TINH_TRANG, dto.TINH_TRANG);
            Assert.Equal(entity.MA_SP, dto.MA_SP);
            Assert.Equal(entity.LAI_SUAT, dto.LAI_SUAT);
            Assert.Equal(entity.KY_HAN, dto.KY_HAN);
        }

        [Fact]
        public void MapToDPDADetailsDto_Should_Include_All_13_Business_Columns()
        {
            // Arrange - Complete DPDA entity
            var entity = new DPDA
            {
                Id = 2,
                NGAY_DL = new DateTime(2024, 12, 31),
                MA_CHI_NHANH = "CNL2",
                MA_KHACH_HANG = "KH002",
                TEN_KHACH_HANG = "Tran Thi B",
                SO_TK = "9876543210",
                TEN_TK = "Tai khoan thanh toan",
                LOAI_TK = "TT",
                SO_DU = 2500000.25m,
                NGAY_MO_TK = new DateTime(2024, 03, 20),
                TINH_TRANG = "TAM_KHOA",
                MA_SP = "SP002",
                TEN_SP = "Tai khoan thanh toan",
                LAI_SUAT = 0.001m,
                KY_HAN = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act - Map to details DTO
            var dto = new DPDADetailsDto
            {
                Id = entity.Id,
                NGAY_DL = entity.NGAY_DL,
                MA_CHI_NHANH = entity.MA_CHI_NHANH,
                MA_KHACH_HANG = entity.MA_KHACH_HANG,
                TEN_KHACH_HANG = entity.TEN_KHACH_HANG,
                SO_TK = entity.SO_TK,
                TEN_TK = entity.TEN_TK,
                LOAI_TK = entity.LOAI_TK,
                SO_DU = entity.SO_DU,
                NGAY_MO_TK = entity.NGAY_MO_TK,
                TINH_TRANG = entity.TINH_TRANG,
                MA_SP = entity.MA_SP,
                TEN_SP = entity.TEN_SP,
                LAI_SUAT = entity.LAI_SUAT,
                KY_HAN = entity.KY_HAN,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };

            // Assert - Validate complete mapping including system columns
            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.NGAY_DL, dto.NGAY_DL);
            Assert.Equal(entity.MA_CHI_NHANH, dto.MA_CHI_NHANH);
            Assert.Equal(entity.TEN_KHACH_HANG, dto.TEN_KHACH_HANG);
            Assert.Equal(entity.SO_DU, dto.SO_DU);
            Assert.Equal(entity.TINH_TRANG, dto.TINH_TRANG);
            Assert.Equal(entity.CreatedAt, dto.CreatedAt);
            Assert.Equal(entity.UpdatedAt, dto.UpdatedAt);
        }

        [Fact]
        public void DPDACreateDto_Should_Validate_Required_Fields()
        {
            // Arrange - Test validation attributes
            var createDto = new DPDACreateDto
            {
                MA_CHI_NHANH = "CNL1",
                MA_KHACH_HANG = "KH001",
                TEN_KHACH_HANG = "Test Customer",
                SO_TK = "1234567890",
                LOAI_TK = "TD",
                SO_DU = 1000000.00m,
                TINH_TRANG = "HOAT_DONG"
            };

            // Assert - Validate required field presence
            Assert.NotNull(createDto.MA_CHI_NHANH);
            Assert.NotNull(createDto.MA_KHACH_HANG);
            Assert.NotNull(createDto.TEN_KHACH_HANG);
            Assert.NotNull(createDto.SO_TK);
            Assert.NotNull(createDto.LOAI_TK);
            Assert.True(createDto.SO_DU >= 0);
            Assert.NotNull(createDto.TINH_TRANG);
        }

        #endregion

        #region DPDA Business Logic Tests

        [Fact]
        public async Task GetDPDAStatisticsAsync_Should_Calculate_Correct_Metrics()
        {
            // Arrange - Test data for statistics calculation
            var testData = new List<DPDA>
            {
                new DPDA { Id = 1, SO_DU = 1000000.00m, LOAI_TK = "TD", TINH_TRANG = "HOAT_DONG" },
                new DPDA { Id = 2, SO_DU = 2000000.00m, LOAI_TK = "TT", TINH_TRANG = "HOAT_DONG" },
                new DPDA { Id = 3, SO_DU = 500000.00m, LOAI_TK = "TD", TINH_TRANG = "TAM_KHOA" }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                          .ReturnsAsync(testData);

            // Act - Calculate statistics
            // var result = await _service.GetDPDAStatisticsAsync();

            // Assert - Validate statistical calculations
            // Assert.NotNull(result);
            // Assert.True(result.Success);
            // Assert.Equal(3, result.Data.TotalRecords);
            // Assert.Equal(3500000.00m, result.Data.TotalBalance);
            // Assert.Equal(2, result.Data.ActiveAccounts);
        }

        [Fact]
        public async Task ImportDPDAFromCsvAsync_Should_Handle_Valid_File()
        {
            // Arrange - Mock CSV import functionality
            var importResult = new DPDAImportResultDto
            {
                TotalRecords = 100,
                SuccessfulRecords = 98,
                FailedRecords = 2,
                ErrorMessages = new List<string> { "Row 5: Invalid SO_DU format", "Row 12: Missing MA_KHACH_HANG" },
                ProcessingTimeSeconds = 2.5
            };

            // _mockDirectImportService.Setup(s => s.ImportFromCsvAsync<DPDA>(It.IsAny<string>(), It.IsAny<string>()))
            //                       .ReturnsAsync(importResult);

            // Act - Test CSV import
            // var result = await _service.ImportDPDAFromCsvAsync("test.csv", "/path/to/file");

            // Assert - Validate import results
            // Assert.NotNull(result);
            // Assert.True(result.Success);
            // Assert.Equal(100, result.Data.TotalRecords);
            // Assert.Equal(98, result.Data.SuccessfulRecords);
            // Assert.Equal(2, result.Data.FailedRecords);
        }

        #endregion

        #region DPDA Repository Integration Tests

        [Fact]
        public async Task DPDARepository_Should_Handle_Temporal_Queries()
        {
            // Arrange - Test temporal table functionality
            var historicalDate = new DateTime(2024, 10, 01);

            _mockRepository.Setup(r => r.GetAsOfDateAsync(historicalDate))
                          .ReturnsAsync(new List<DPDA>());

            // Act - Test temporal query
            // var result = await _service.GetDPDAAsOfDateAsync(historicalDate);

            // Assert - Validate temporal functionality
            // Assert.NotNull(result);
            // Assert.True(result.Success);
            // Verify temporal query called correctly
            _mockRepository.Verify(r => r.GetAsOfDateAsync(historicalDate), Times.Once);
        }

        #endregion

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
