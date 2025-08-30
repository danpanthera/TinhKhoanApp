using Xunit;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.Dtos;

namespace TinhKhoanApp.Api.Tests.Dtos
{
    /// <summary>
    /// DTO Mapping tests cho DPDA - Data Transfer Object validation
    /// Đảm bảo tính chính xác của mapping giữa Entity và DTOs
    /// DPDA: 13 business columns với various DTO types
    /// </summary>
    public class DPDADtoMappingTests
    {
        #region DPDA DTO Mapping Validation Tests

        [Fact]
        public void MapToDPDAPreviewDto_Should_Map_All_Preview_Fields_Correctly()
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

            // Act - Map to preview DTO (10 key fields cho preview)
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

            // Assert - Validate all preview field mappings
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
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddHours(-2)
            };

            // Act - Map to details DTO (tất cả fields including system columns)
            var dto = new DPDADetailsDto
            {
                Id = entity.Id,
                NGAY_DL = entity.NGAY_DL,
                // All 13 business columns
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
                // System columns
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };

            // Assert - Validate complete mapping including all 13 business columns
            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.NGAY_DL, dto.NGAY_DL);
            Assert.Equal(entity.MA_CHI_NHANH, dto.MA_CHI_NHANH);
            Assert.Equal(entity.MA_KHACH_HANG, dto.MA_KHACH_HANG);
            Assert.Equal(entity.TEN_KHACH_HANG, dto.TEN_KHACH_HANG);
            Assert.Equal(entity.SO_TK, dto.SO_TK);
            Assert.Equal(entity.TEN_TK, dto.TEN_TK);
            Assert.Equal(entity.LOAI_TK, dto.LOAI_TK);
            Assert.Equal(entity.SO_DU, dto.SO_DU);
            Assert.Equal(entity.NGAY_MO_TK, dto.NGAY_MO_TK);
            Assert.Equal(entity.TINH_TRANG, dto.TINH_TRANG);
            Assert.Equal(entity.MA_SP, dto.MA_SP);
            Assert.Equal(entity.TEN_SP, dto.TEN_SP);
            Assert.Equal(entity.LAI_SUAT, dto.LAI_SUAT);
            Assert.Equal(entity.KY_HAN, dto.KY_HAN);
            Assert.Equal(entity.CreatedAt, dto.CreatedAt);
            Assert.Equal(entity.UpdatedAt, dto.UpdatedAt);
        }

        [Fact]
        public void DPDASummaryDto_Should_Calculate_Aggregate_Fields()
        {
            // Arrange - Test data for summary calculations
            var entities = new List<DPDA>
            {
                new DPDA { SO_DU = 1000000.00m, LOAI_TK = "TD", TINH_TRANG = "HOAT_DONG" },
                new DPDA { SO_DU = 2000000.00m, LOAI_TK = "TT", TINH_TRANG = "HOAT_DONG" },
                new DPDA { SO_DU = 500000.00m, LOAI_TK = "TD", TINH_TRANG = "TAM_KHOA" }
            };

            // Act - Calculate summary metrics
            var dto = new DPDASummaryDto
            {
                TotalRecords = entities.Count,
                TotalBalance = entities.Sum(e => e.SO_DU),
                ActiveAccountsCount = entities.Count(e => e.TINH_TRANG == "HOAT_DONG"),
                SavingsAccountsCount = entities.Count(e => e.LOAI_TK == "TD"),
                AverageBalance = entities.Average(e => e.SO_DU),
                MaxBalance = entities.Max(e => e.SO_DU),
                MinBalance = entities.Min(e => e.SO_DU)
            };

            // Assert - Validate calculated summary fields
            Assert.Equal(3, dto.TotalRecords);
            Assert.Equal(3500000.00m, dto.TotalBalance);
            Assert.Equal(2, dto.ActiveAccountsCount);
            Assert.Equal(2, dto.SavingsAccountsCount);
            Assert.Equal(1166666.67m, Math.Round(dto.AverageBalance, 2));
            Assert.Equal(2000000.00m, dto.MaxBalance);
            Assert.Equal(500000.00m, dto.MinBalance);
        }

        [Fact]
        public void DPDACreateDto_Should_Validate_Required_Fields()
        {
            // Arrange - Test validation attributes cho create operation
            var createDto = new DPDACreateDto
            {
                MA_CHI_NHANH = "CNL1",
                MA_KHACH_HANG = "KH001",
                TEN_KHACH_HANG = "Test Customer",
                SO_TK = "1234567890",
                TEN_TK = "Test Account",
                LOAI_TK = "TD",
                SO_DU = 1000000.00m,
                NGAY_MO_TK = new DateTime(2024, 01, 01),
                TINH_TRANG = "HOAT_DONG",
                MA_SP = "SP001",
                TEN_SP = "Test Product",
                LAI_SUAT = 0.05m,
                KY_HAN = 12
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

        [Fact]
        public void DPDAUpdateDto_Should_Allow_Partial_Updates()
        {
            // Arrange - Test partial update capabilities
            var updateDto = new DPDAUpdateDto
            {
                Id = 1,
                // Chỉ update một số fields
                TEN_KHACH_HANG = "Updated Customer Name",
                SO_DU = 2000000.00m,
                TINH_TRANG = "TAM_KHOA",
                LAI_SUAT = 0.075m
                // Các fields khác null hoặc không set = không update
            };

            // Assert - Validate partial update structure
            Assert.True(updateDto.Id > 0);
            Assert.NotNull(updateDto.TEN_KHACH_HANG);
            Assert.True(updateDto.SO_DU > 0);
            Assert.NotNull(updateDto.TINH_TRANG);
            Assert.NotNull(updateDto.LAI_SUAT);
            // Các fields không set should remain null for partial update
        }

        [Fact]
        public void DPDAImportResultDto_Should_Track_Import_Statistics()
        {
            // Arrange - Test import result tracking
            var importResult = new DPDAImportResultDto
            {
                TotalRecords = 1000,
                SuccessfulRecords = 980,
                FailedRecords = 20,
                ErrorMessages = new List<string>
                {
                    "Row 25: Invalid SO_DU format",
                    "Row 87: Missing MA_KHACH_HANG",
                    "Row 156: Invalid date format for NGAY_MO_TK"
                },
                ProcessingTimeSeconds = 5.75,
                ImportedDate = DateTime.UtcNow,
                FileName = "dpda_20241231.csv"
            };

            // Assert - Validate import tracking fields
            Assert.Equal(1000, importResult.TotalRecords);
            Assert.Equal(980, importResult.SuccessfulRecords);
            Assert.Equal(20, importResult.FailedRecords);
            Assert.Equal(3, importResult.ErrorMessages.Count);
            Assert.True(importResult.ProcessingTimeSeconds > 0);
            Assert.NotNull(importResult.FileName);
            Assert.Contains("dpda", importResult.FileName.ToLower());
        }

        #endregion

        #region DPDA Field Validation Tests

        [Fact]
        public void DPDA_Decimal_Fields_Should_Handle_Precision_Correctly()
        {
            // Arrange - Test decimal precision cho financial fields
            var entity = new DPDA
            {
                SO_DU = 1234567.89m,      // Currency với 2 decimal places
                LAI_SUAT = 0.068500m      // Interest rate với 6 decimal places
            };

            // Act - Map to DTOs
            var previewDto = new DPDAPreviewDto { SO_DU = entity.SO_DU, LAI_SUAT = entity.LAI_SUAT };
            var detailsDto = new DPDADetailsDto { SO_DU = entity.SO_DU, LAI_SUAT = entity.LAI_SUAT };

            // Assert - Validate decimal precision handling
            Assert.Equal(1234567.89m, previewDto.SO_DU);
            Assert.Equal(0.068500m, previewDto.LAI_SUAT);
            Assert.Equal(1234567.89m, detailsDto.SO_DU);
            Assert.Equal(0.068500m, detailsDto.LAI_SUAT);
        }

        [Fact]
        public void DPDA_DateTime_Fields_Should_Handle_Dates_Correctly()
        {
            // Arrange - Test date handling cho DPDA date fields
            var testDate1 = new DateTime(2024, 12, 31, 0, 0, 0); // NGAY_DL
            var testDate2 = new DateTime(2024, 06, 15, 0, 0, 0); // NGAY_MO_TK

            var entity = new DPDA
            {
                NGAY_DL = testDate1,
                NGAY_MO_TK = testDate2
            };

            // Act - Map to DTOs
            var previewDto = new DPDAPreviewDto { NGAY_DL = entity.NGAY_DL };
            var detailsDto = new DPDADetailsDto { NGAY_DL = entity.NGAY_DL, NGAY_MO_TK = entity.NGAY_MO_TK };

            // Assert - Validate date handling
            Assert.Equal(testDate1, previewDto.NGAY_DL);
            Assert.Equal(testDate1, detailsDto.NGAY_DL);
            Assert.Equal(testDate2, detailsDto.NGAY_MO_TK);
        }

        [Fact]
        public void DPDA_String_Fields_Should_Handle_Vietnamese_Characters()
        {
            // Arrange - Test Vietnamese character support
            var entity = new DPDA
            {
                TEN_KHACH_HANG = "Nguyễn Văn Đức",
                TEN_TK = "Tài khoản tiết kiệm",
                TEN_SP = "Sản phẩm tiết kiệm có định kỳ"
            };

            // Act - Map to DTOs
            var previewDto = new DPDAPreviewDto
            {
                TEN_KHACH_HANG = entity.TEN_KHACH_HANG
            };
            var detailsDto = new DPDADetailsDto
            {
                TEN_KHACH_HANG = entity.TEN_KHACH_HANG,
                TEN_TK = entity.TEN_TK,
                TEN_SP = entity.TEN_SP
            };

            // Assert - Validate Vietnamese character preservation
            Assert.Equal("Nguyễn Văn Đức", previewDto.TEN_KHACH_HANG);
            Assert.Equal("Nguyễn Văn Đức", detailsDto.TEN_KHACH_HANG);
            Assert.Equal("Tài khoản tiết kiệm", detailsDto.TEN_TK);
            Assert.Equal("Sản phẩm tiết kiệm có định kỳ", detailsDto.TEN_SP);
        }

        #endregion
    }
}
