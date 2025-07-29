using Microsoft.Extensions.Logging;
using Moq;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Repositories;
using TinhKhoanApp.Api.Services;

namespace TinhKhoanApp.Api.Tests
{
    public class RR01ServiceTests
    {
        private readonly Mock<IRR01Repository> _mockRepository;
        private readonly Mock<ILogger<RR01Service>> _mockLogger;
        private readonly RR01Service _service;

        public RR01ServiceTests()
        {
            _mockRepository = new Mock<IRR01Repository>();
            _mockLogger = new Mock<ILogger<RR01Service>>();
            _service = new RR01Service(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllRecords()
        {
            // Arrange
            var testRecords = GetTestRR01Entities();
            _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(testRecords);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(testRecords.Count, result.Count());
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnRecord()
        {
            // Arrange
            var testEntity = GetTestRR01Entities().First();
            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(testEntity);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testEntity.Id, result.Id);
            _mockRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<long>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((RR01)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<long>()), Times.Once);
        }

        [Fact]
        public async Task GetByDateAsync_ShouldReturnFilteredRecords()
        {
            // Arrange
            var testDate = new DateTime(2024, 12, 31);
            var testRecords = GetTestRR01Entities().Where(r => r.NGAY_DL.Date == testDate.Date).ToList();
            _mockRepository.Setup(repo => repo.GetByDateAsync(testDate)).ReturnsAsync(testRecords);

            // Act
            var result = await _service.GetByDateAsync(testDate);

            // Assert
            Assert.Equal(testRecords.Count, result.Count());
            _mockRepository.Verify(repo => repo.GetByDateAsync(testDate), Times.Once);
        }

        [Fact]
        public async Task GetByBranchAsync_ShouldReturnFilteredRecords()
        {
            // Arrange
            var branchCode = "7800";
            var testRecords = GetTestRR01Entities().Where(r => r.BRCD.Trim() == branchCode).ToList();
            _mockRepository.Setup(repo => repo.GetByBranchAsync(branchCode, null)).ReturnsAsync(testRecords);

            // Act
            var result = await _service.GetByBranchAsync(branchCode);

            // Assert
            Assert.Equal(testRecords.Count, result.Count());
            _mockRepository.Verify(repo => repo.GetByBranchAsync(branchCode, null), Times.Once);
        }

        [Fact]
        public async Task GetByCustomerAsync_ShouldReturnFilteredRecords()
        {
            // Arrange
            var customerId = "161524751";
            var testRecords = GetTestRR01Entities().Where(r => r.MA_KH.Trim() == customerId).ToList();
            _mockRepository.Setup(repo => repo.GetByCustomerAsync(customerId, null)).ReturnsAsync(testRecords);

            // Act
            var result = await _service.GetByCustomerAsync(customerId);

            // Assert
            Assert.Equal(testRecords.Count, result.Count());
            _mockRepository.Verify(repo => repo.GetByCustomerAsync(customerId, null), Times.Once);
        }

        [Fact]
        public async Task GetPagedAsync_ShouldReturnPagedRecords()
        {
            // Arrange
            var pageNumber = 1;
            var pageSize = 10;
            var testRecords = GetTestRR01Entities().Take(pageSize).ToList();
            var totalCount = 25;
            _mockRepository.Setup(repo => repo.GetPagedAsync(pageNumber, pageSize, null, null, null))
                .ReturnsAsync((testRecords, totalCount));

            // Act
            var result = await _service.GetPagedAsync(pageNumber, pageSize);

            // Assert
            Assert.Equal(testRecords.Count, result.Records.Count());
            Assert.Equal(totalCount, result.TotalCount);
            _mockRepository.Verify(repo => repo.GetPagedAsync(pageNumber, pageSize, null, null, null), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateAndReturnRecord()
        {
            // Arrange
            var createDto = new CreateRR01DTO
            {
                NGAY_DL = new DateTime(2024, 12, 31),
                BRCD = "7800",
                MA_KH = "161524751",
                TEN_KH = "Test Customer",
                DUNO_GOC_HIENTAI = "100000"
            };
            var entity = createDto.ToEntity();
            entity.Id = 1; // Simulate ID assignment by database

            _mockRepository.Setup(repo => repo.AddAsync(It.IsAny<RR01>()))
                .ReturnsAsync((RR01 r) =>
                {
                    r.Id = 1; // Simulate ID assignment
                    return r;
                });

            // Act
            var result = await _service.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(createDto.BRCD, result.BRCD);
            Assert.Equal(createDto.MA_KH, result.MA_KH);
            _mockRepository.Verify(repo => repo.AddAsync(It.IsAny<RR01>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithExistingId_ShouldUpdateAndReturnRecord()
        {
            // Arrange
            var existingEntity = GetTestRR01Entities().First();
            var updateDto = new UpdateRR01DTO
            {
                TEN_KH = "Updated Customer Name",
                DUNO_GOC_HIENTAI = "150000"
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(existingEntity.Id))
                .ReturnsAsync(existingEntity);
            _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<RR01>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(existingEntity.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateDto.TEN_KH, result.TEN_KH);
            Assert.Equal(updateDto.DUNO_GOC_HIENTAI, result.DUNO_GOC_HIENTAI);
            _mockRepository.Verify(repo => repo.GetByIdAsync(existingEntity.Id), Times.Once);
            _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<RR01>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistingId_ShouldReturnNull()
        {
            // Arrange
            var updateDto = new UpdateRR01DTO
            {
                TEN_KH = "Updated Customer Name"
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>()))
                .ReturnsAsync((RR01)null);

            // Act
            var result = await _service.UpdateAsync(999, updateDto);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<long>()), Times.Once);
            _mockRepository.Verify(repo => repo.UpdateAsync(It.IsAny<RR01>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WithExistingId_ShouldReturnTrue()
        {
            // Arrange
            var existingEntity = GetTestRR01Entities().First();

            _mockRepository.Setup(repo => repo.GetByIdAsync(existingEntity.Id))
                .ReturnsAsync(existingEntity);
            _mockRepository.Setup(repo => repo.DeleteAsync(It.IsAny<RR01>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(existingEntity.Id);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(repo => repo.GetByIdAsync(existingEntity.Id), Times.Once);
            _mockRepository.Verify(repo => repo.DeleteAsync(It.IsAny<RR01>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingId_ShouldReturnFalse()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<long>()))
                .ReturnsAsync((RR01)null);

            // Act
            var result = await _service.DeleteAsync(999);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<long>()), Times.Once);
            _mockRepository.Verify(repo => repo.DeleteAsync(It.IsAny<RR01>()), Times.Never);
        }

        [Fact]
        public async Task GetDistinctDatesAsync_ShouldReturnDistinctDates()
        {
            // Arrange
            var testRecords = GetTestRR01Entities();
            var expectedDates = testRecords.Select(r => r.NGAY_DL.Date).Distinct().OrderByDescending(d => d);

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(testRecords);

            // Act
            var result = await _service.GetDistinctDatesAsync();

            // Assert
            Assert.Equal(expectedDates.Count(), result.Count());
            Assert.Equal(expectedDates, result);
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        // Helper method to create test entities
        private List<RR01> GetTestRR01Entities()
        {
            return new List<RR01>
            {
                new RR01
                {
                    Id = 1,
                    NGAY_DL = new DateTime(2024, 12, 31),
                    CN_LOAI_I = "7800",
                    BRCD = "7800",
                    MA_KH = "161524751",
                    TEN_KH = "Bùi Ngọc Hiền",
                    SO_LDS = "7800-LDS-201405119",
                    CCY = "VND",
                    SO_LAV = "7800LAV201400137",
                    LOAI_KH = "100",
                    NGAY_GIAI_NGAN = "20140303",
                    NGAY_DEN_HAN = "20170224",
                    VAMC_FLG = "N",
                    NGAY_XLRR = "20161103",
                    DUNO_GOC_BAN_DAU = "1490000000",
                    DUNO_LAI_TICHLUY_BD = "323678810",
                    DOC_DAUKY_DA_THU_HT = "1490000000",
                    DUNO_GOC_HIENTAI = "0",
                    DUNO_LAI_HIENTAI = "0",
                    DUNO_NGAN_HAN = "",
                    DUNO_TRUNG_HAN = "0",
                    DUNO_DAI_HAN = "",
                    THU_GOC = "149000000",
                    THU_LAI = "0",
                    BDS = "0",
                    DS = "830000000",
                    TSK = "",
                    CREATED_DATE = DateTime.Now,
                    UPDATED_DATE = DateTime.Now,
                    FILE_NAME = "7800_rr01_20241231.csv"
                },
                new RR01
                {
                    Id = 2,
                    NGAY_DL = new DateTime(2024, 12, 31),
                    CN_LOAI_I = "7800",
                    BRCD = "7800",
                    MA_KH = "161524751",
                    TEN_KH = "Bùi Ngọc Hiền",
                    SO_LDS = "7800-LDS-201500533",
                    CCY = "VND",
                    SO_LAV = "7800LAV201400846",
                    LOAI_KH = "100",
                    NGAY_GIAI_NGAN = "20150427",
                    NGAY_DEN_HAN = "20151127",
                    VAMC_FLG = "N",
                    NGAY_XLRR = "20161103",
                    DUNO_GOC_BAN_DAU = "1000000",
                    DUNO_LAI_TICHLUY_BD = "50020834",
                    DOC_DAUKY_DA_THU_HT = "1000000",
                    DUNO_GOC_HIENTAI = "0",
                    DUNO_LAI_HIENTAI = "0",
                    DUNO_NGAN_HAN = "0",
                    DUNO_TRUNG_HAN = "",
                    DUNO_DAI_HAN = "",
                    THU_GOC = "1000000",
                    THU_LAI = "0",
                    BDS = "0",
                    DS = "830000000",
                    TSK = "",
                    CREATED_DATE = DateTime.Now,
                    UPDATED_DATE = DateTime.Now,
                    FILE_NAME = "7800_rr01_20241231.csv"
                },
                new RR01
                {
                    Id = 3,
                    NGAY_DL = new DateTime(2025, 5, 31),
                    CN_LOAI_I = "7800",
                    BRCD = "7800",
                    MA_KH = "287301245",
                    TEN_KH = "Nguyễn Văn A",
                    SO_LDS = "7800-LDS-202501234",
                    CCY = "VND",
                    SO_LAV = "7800LAV202500123",
                    LOAI_KH = "100",
                    NGAY_GIAI_NGAN = "20250101",
                    NGAY_DEN_HAN = "20270101",
                    VAMC_FLG = "N",
                    NGAY_XLRR = "20250301",
                    DUNO_GOC_BAN_DAU = "5000000000",
                    DUNO_LAI_TICHLUY_BD = "250000000",
                    DOC_DAUKY_DA_THU_HT = "1000000000",
                    DUNO_GOC_HIENTAI = "4000000000",
                    DUNO_LAI_HIENTAI = "200000000",
                    DUNO_NGAN_HAN = "",
                    DUNO_TRUNG_HAN = "4000000000",
                    DUNO_DAI_HAN = "",
                    THU_GOC = "1000000000",
                    THU_LAI = "50000000",
                    BDS = "6000000000",
                    DS = "0",
                    TSK = "0",
                    CREATED_DATE = DateTime.Now,
                    UPDATED_DATE = DateTime.Now,
                    FILE_NAME = "7800_rr01_20250531.csv"
                }
            };
        }
    }
}
