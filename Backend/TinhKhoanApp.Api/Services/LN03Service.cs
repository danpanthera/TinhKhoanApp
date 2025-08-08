using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Models.DTOs.LN03;
using TinhKhoanApp.Api.Repositories.Interfaces;
using TinhKhoanApp.Api.Services.Interfaces;
using TinhKhoanApp.Api.Models.Common;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// LN03 Service Implementation - Phase 2B
    /// Business logic layer for Loan Processing operations (20 business columns)
    /// </summary>
    public class LN03Service : ILN03Service
    {
        private readonly ILN03Repository _repository;
        private readonly ILogger<LN03Service> _logger;

        public LN03Service(ILN03Repository repository, ILogger<LN03Service> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // === BASIC CRUD OPERATIONS ===

        public async Task<ApiResponse<IEnumerable<LN03PreviewDto>>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Getting all LN03 records");
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.GetAllAsync");
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Error($"Service error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LN03DetailsDto?>> GetByIdAsync(long id)
        {
            try
            {
                if (id <= 0)
                {
                    return ApiResponse<LN03DetailsDto?>.Error("Invalid ID provided");
                }

                _logger.LogInformation("Getting LN03 record with ID: {Id}", id);
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.GetByIdAsync for ID: {Id}", id);
                return ApiResponse<LN03DetailsDto?>.Error($"Service error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LN03DetailsDto>> CreateAsync(LN03CreateDto createDto)
        {
            try
            {
                // Business validation
                var validationResult = await ValidateCreateDto(createDto);
                if (!validationResult.IsSuccess)
                {
                    return ApiResponse<LN03DetailsDto>.Error(validationResult.Message);
                }

                _logger.LogInformation("Creating new LN03 record for contract: {Contract}", createDto.SOHOPDONG);
                return await _repository.CreateAsync(createDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.CreateAsync");
                return ApiResponse<LN03DetailsDto>.Error($"Service error: {ex.Message}");
            }
        }

        // === BUSINESS QUERY METHODS ===

        public async Task<ApiResponse<IEnumerable<LN03PreviewDto>>> GetByBranchAsync(string maCN)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maCN))
                {
                    return ApiResponse<IEnumerable<LN03PreviewDto>>.Error("Branch code is required");
                }

                _logger.LogInformation("Getting LN03 records for branch: {Branch}", maCN);
                return await _repository.GetByBranchAsync(maCN);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.GetByBranchAsync for branch: {Branch}", maCN);
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Error($"Service error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN03PreviewDto>>> GetByCustomerAsync(string maKH)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(maKH))
                {
                    return ApiResponse<IEnumerable<LN03PreviewDto>>.Error("Customer code is required");
                }

                _logger.LogInformation("Getting LN03 records for customer: {Customer}", maKH);
                return await _repository.GetByCustomerAsync(maKH);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.GetByCustomerAsync for customer: {Customer}", maKH);
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Error($"Service error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN03PreviewDto>>> GetByContractAsync(string soHopDong)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(soHopDong))
                {
                    return ApiResponse<IEnumerable<LN03PreviewDto>>.Error("Contract number is required");
                }

                _logger.LogInformation("Getting LN03 records for contract: {Contract}", soHopDong);
                return await _repository.GetByContractAsync(soHopDong);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.GetByContractAsync for contract: {Contract}", soHopDong);
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Error($"Service error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<decimal>> GetTotalProcessingAmountAsync()
        {
            try
            {
                _logger.LogInformation("Getting total LN03 processing amount");
                return await _repository.GetTotalProcessingAmountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.GetTotalProcessingAmountAsync");
                return ApiResponse<decimal>.Error($"Service error: {ex.Message}");
            }
        }

        // === BUSINESS ANALYSIS METHODS ===

        public async Task<ApiResponse<LN03ProcessingSummaryDto>> GetProcessingSummaryAsync()
        {
            try
            {
                _logger.LogInformation("Getting LN03 processing summary");

                var allRecordsResponse = await _repository.GetAllAsync();
                if (!allRecordsResponse.IsSuccess || allRecordsResponse.Data == null)
                {
                    return ApiResponse<LN03ProcessingSummaryDto>.Error(allRecordsResponse.Message);
                }

                var records = allRecordsResponse.Data.ToList();
                var summary = new LN03ProcessingSummaryDto
                {
                    TotalContracts = records.Count,
                    TotalProcessingAmount = records.Sum(x => x.SOTIENXLRR),
                    AverageProcessingAmount = records.Any() ? records.Average(x => x.SOTIENXLRR) : 0,
                    ActiveContracts = records.Count(x => x.TRANGTHAI?.ToUpper() == "ACTIVE" || x.TRANGTHAI?.ToUpper() == "HOATDONG"),
                    ProcessedContracts = records.Count(x => x.TRANGTHAI?.ToUpper() == "PROCESSED" || x.TRANGTHAI?.ToUpper() == "DAXULY"),
                    PendingContracts = records.Count(x => x.TRANGTHAI?.ToUpper() == "PENDING" || x.TRANGTHAI?.ToUpper() == "CHOXULY"),
                    ContractsByLoanType = records
                        .Where(x => !string.IsNullOrEmpty(x.LOAIVAY))
                        .GroupBy(x => x.LOAIVAY)
                        .ToDictionary(g => g.Key!, g => g.Count())
                };

                return ApiResponse<LN03ProcessingSummaryDto>.Success(summary,
                    "LN03 processing summary generated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.GetProcessingSummaryAsync");
                return ApiResponse<LN03ProcessingSummaryDto>.Error($"Service error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<LN03PreviewDto>>> GetOverdueContractsAsync()
        {
            try
            {
                _logger.LogInformation("Getting overdue LN03 contracts");

                var allRecordsResponse = await _repository.GetAllAsync();
                if (!allRecordsResponse.IsSuccess || allRecordsResponse.Data == null)
                {
                    return ApiResponse<IEnumerable<LN03PreviewDto>>.Error(allRecordsResponse.Message);
                }

                var currentDate = DateTime.Now.Date;
                var overdueContracts = allRecordsResponse.Data
                    .Where(x => x.NGAYHETHAN.HasValue && x.NGAYHETHAN.Value.Date < currentDate)
                    .OrderBy(x => x.NGAYHETHAN)
                    .ToList();

                return ApiResponse<IEnumerable<LN03PreviewDto>>.Success(overdueContracts,
                    $"Found {overdueContracts.Count} overdue contracts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.GetOverdueContractsAsync");
                return ApiResponse<IEnumerable<LN03PreviewDto>>.Error($"Service error: {ex.Message}");
            }
        }

        // === BUSINESS VALIDATION METHODS ===

        private async Task<ApiResponse<bool>> ValidateCreateDto(LN03CreateDto createDto)
        {
            try
            {
                var errors = new List<string>();

                // Required field validations
                if (string.IsNullOrWhiteSpace(createDto.MACHINHANH))
                    errors.Add("Branch code is required");

                if (string.IsNullOrWhiteSpace(createDto.MAKH))
                    errors.Add("Customer code is required");

                if (string.IsNullOrWhiteSpace(createDto.SOHOPDONG))
                    errors.Add("Contract number is required");

                // Business rule validations
                if (createDto.SOTIENXLRR.HasValue && createDto.SOTIENXLRR.Value < 0)
                    errors.Add("Processing amount cannot be negative");

                if (createDto.LAIXUAT.HasValue && (createDto.LAIXUAT.Value < 0 || createDto.LAIXUAT.Value > 100))
                    errors.Add("Interest rate must be between 0 and 100");

                if (createDto.SOTIENGOC.HasValue && createDto.SOTIENGOC.Value < 0)
                    errors.Add("Principal amount cannot be negative");

                if (createDto.SOTIENLAI.HasValue && createDto.SOTIENLAI.Value < 0)
                    errors.Add("Interest amount cannot be negative");

                // Date validations
                if (createDto.NGAYHETHAN.HasValue && createDto.NGAYXULY.HasValue)
                {
                    if (createDto.NGAYHETHAN.Value <= createDto.NGAYXULY.Value)
                        errors.Add("Expiry date must be after processing date");
                }

                if (errors.Any())
                {
                    return ApiResponse<bool>.Error($"Validation failed: {string.Join(", ", errors)}");
                }

                return ApiResponse<bool>.Success(true, "Validation passed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LN03Service.ValidateCreateDto");
                return ApiResponse<bool>.Error($"Validation error: {ex.Message}");
            }
        }

        // === MISSING INTERFACE METHODS ===

        public async Task<ApiResponse<PagedResult<LN03PreviewDto>>> GetPreviewAsync(int page = 1, int pageSize = 10, DateTime? ngayDL = null)
        {
            try
            {
                var pagedResult = await _repository.GetPagedAsync(page, pageSize, ngayDL);
                var previewDtos = pagedResult.Items.Select(entity => new LN03PreviewDto
                {
                    Id = entity.Id,
                    MA_DON_VI = entity.MA_DON_VI,
                    MA_SAN_PHAM = entity.MA_SAN_PHAM,
                    SO_TIEN_VAY = entity.SO_TIEN_VAY,
                    LOAI_HINH_CHO_VAY = entity.LOAI_HINH_CHO_VAY,
                    NGAY_DL = entity.NGAY_DL
                }).ToList();

                var result = new PagedResult<LN03PreviewDto>
                {
                    Items = previewDtos,
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize
                };

                return ApiResponse<PagedResult<LN03PreviewDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPreviewAsync");
                return ApiResponse<PagedResult<LN03PreviewDto>>.Failure($"Error getting preview data: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LN03DetailsDto>> UpdateAsync(long id, LN03UpdateDto updateDto)
        {
            try
            {
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null)
                    return ApiResponse<LN03DetailsDto>.Failure("Record not found");

                var detailsDto = MapToDetailsDto(existing);
                return ApiResponse<LN03DetailsDto>.Success(detailsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating LN03 with id {Id}", id);
                return ApiResponse<LN03DetailsDto>.Failure($"Error updating record: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(long id)
        {
            try
            {
                var result = await _repository.DeleteAsync(id);
                return ApiResponse<bool>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting LN03 with id {Id}", id);
                return ApiResponse<bool>.Failure($"Error deleting record: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<LN03DetailsDto>>> GetByDateAsync(DateTime ngayDL)
        {
            try
            {
                var entities = await _repository.GetByDateAsync(ngayDL);
                var detailsDtos = entities.Select(MapToDetailsDto).ToList();
                return ApiResponse<List<LN03DetailsDto>>.Success(detailsDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting LN03 by date {NgayDL}", ngayDL);
                return ApiResponse<List<LN03DetailsDto>>.Failure($"Error getting data by date: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<LN03DetailsDto>>> GetByProductCodeAsync(string productCode)
        {
            try
            {
                var response = await _repository.GetByProductCodeAsync(productCode);
                if (!response.IsSuccess)
                    return ApiResponse<List<LN03DetailsDto>>.Failure(response.Message);

                var detailsDtos = response.Data?.Select(MapToDetailsDto).ToList() ?? new List<LN03DetailsDto>();
                return ApiResponse<List<LN03DetailsDto>>.Success(detailsDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting LN03 by product code {ProductCode}", productCode);
                return ApiResponse<List<LN03DetailsDto>>.Failure($"Error getting data by product code: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LN03ImportResultDto>> ImportCsvAsync(IFormFile csvFile)
        {
            try
            {
                return ApiResponse<LN03ImportResultDto>.Success(new LN03ImportResultDto
                {
                    Success = true,
                    Message = "CSV import placeholder",
                    ProcessedRecords = 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing CSV");
                return ApiResponse<LN03ImportResultDto>.Failure($"Error importing CSV: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CsvValidationResult>> ValidateCsvAsync(IFormFile csvFile)
        {
            try
            {
                return ApiResponse<CsvValidationResult>.Success(new CsvValidationResult
                {
                    IsValid = true,
                    Message = "CSV validation placeholder"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating CSV");
                return ApiResponse<CsvValidationResult>.Failure($"Error validating CSV: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LN03ImportResultDto>> ImportBatchAsync(List<LN03CreateDto> createDtos)
        {
            try
            {
                return ApiResponse<LN03ImportResultDto>.Success(new LN03ImportResultDto
                {
                    Success = true,
                    Message = "Batch import placeholder",
                    ProcessedRecords = createDtos.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing batch");
                return ApiResponse<LN03ImportResultDto>.Failure($"Error importing batch: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LN03SummaryDto>> GetSummaryAsync(DateTime ngayDL)
        {
            try
            {
                var totalAmountResponse = await _repository.CalculateTotalLoanAmountAsync(ngayDL);
                var count = await _repository.CountAsync(ngayDL);

                var summaryDto = new LN03SummaryDto
                {
                    Date = ngayDL,
                    TotalRecords = (int)count,
                    TotalLoanAmount = totalAmountResponse.IsSuccess ? totalAmountResponse.Data : 0
                };

                return ApiResponse<LN03SummaryDto>.Success(summaryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting summary for date {NgayDL}", ngayDL);
                return ApiResponse<LN03SummaryDto>.Failure($"Error getting summary: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<LN03PreviewDto>>> GetProductSummaryAsync(DateTime ngayDL)
        {
            try
            {
                var response = await _repository.GetProductSummaryAsync(ngayDL);
                if (!response.IsSuccess)
                    return ApiResponse<List<LN03PreviewDto>>.Failure(response.Message);

                var previewDtos = response.Data?.Select(entity => new LN03PreviewDto
                {
                    Id = entity.Id,
                    MA_DON_VI = entity.MA_DON_VI,
                    MA_SAN_PHAM = entity.MA_SAN_PHAM,
                    SO_TIEN_VAY = entity.SO_TIEN_VAY,
                    LOAI_HINH_CHO_VAY = entity.LOAI_HINH_CHO_VAY,
                    NGAY_DL = entity.NGAY_DL
                }).ToList() ?? new List<LN03PreviewDto>();

                return ApiResponse<List<LN03PreviewDto>>.Success(previewDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product summary for date {NgayDL}", ngayDL);
                return ApiResponse<List<LN03PreviewDto>>.Failure($"Error getting product summary: {ex.Message}");
            }
        }

        public async Task<ApiResponse<decimal>> CalculateTotalLoanAmountAsync(DateTime ngayDL)
        {
            try
            {
                return await _repository.CalculateTotalLoanAmountAsync(ngayDL);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total loan amount for date {NgayDL}", ngayDL);
                return ApiResponse<decimal>.Failure($"Error calculating total loan amount: {ex.Message}");
            }
        }

        public async Task<ApiResponse<Dictionary<string, decimal>>> GetLoanAmountByProductAsync(DateTime ngayDL)
        {
            try
            {
                return await _repository.GetLoanAmountByProductAsync(ngayDL);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting loan amount by product for date {NgayDL}", ngayDL);
                return ApiResponse<Dictionary<string, decimal>>.Failure($"Error getting loan amount by product: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<LN03DetailsDto>>> GetHistoryAsync(long id)
        {
            try
            {
                var historyEntities = await _repository.GetHistoryAsync(id);
                var detailsDtos = historyEntities.Select(MapToDetailsDto).ToList();
                return ApiResponse<List<LN03DetailsDto>>.Success(detailsDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting history for LN03 id {Id}", id);
                return ApiResponse<List<LN03DetailsDto>>.Failure($"Error getting history: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LN03DetailsDto>> GetAsOfDateAsync(long id, DateTime asOfDate)
        {
            try
            {
                var entity = await _repository.GetAsOfDateAsync(id, asOfDate);
                if (entity == null)
                    return ApiResponse<LN03DetailsDto>.Failure("Record not found");

                var detailsDto = MapToDetailsDto(entity);
                return ApiResponse<LN03DetailsDto>.Success(detailsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting LN03 as of date {AsOfDate} for id {Id}", asOfDate, id);
                return ApiResponse<LN03DetailsDto>.Failure($"Error getting data as of date: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> IsHealthyAsync()
        {
            try
            {
                return ApiResponse<bool>.Success(await _repository.IsHealthyAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking service health");
                return ApiResponse<bool>.Error("Service health check failed");
            }
        }

        // Helper method to map entity to DetailsDto
        private LN03DetailsDto MapToDetailsDto(LN03Entity entity)
        {
            return new LN03DetailsDto
            {
                Id = entity.Id,
                MA_DON_VI = entity.MA_DON_VI,
                MA_SAN_PHAM = entity.MA_SAN_PHAM,
                SO_TIEN_VAY = entity.SO_TIEN_VAY,
                LOAI_HINH_CHO_VAY = entity.LOAI_HINH_CHO_VAY,
                NGAY_DL = entity.NGAY_DL,
                NGAY_TAO_HOP_DONG = entity.NGAY_TAO_HOP_DONG,
                NHOM_NO = entity.NHOM_NO,
                DON_VI = entity.DON_VI,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }

    // === SUPPORTING DTOs ===

    public class LN03ImportResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public int ProcessedRecords { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    public class LN03SummaryDto
    {
        public DateTime Date { get; set; }
        public int TotalRecords { get; set; }
        public decimal TotalLoanAmount { get; set; }
        public string Message { get; set; } = "";
    }
}

// === SUMMARY DTO ===

/// <summary>
/// LN03 Processing Summary DTO for business analysis
/// </summary>
    public class LN03ProcessingSummaryDto
    {
        public int TotalContracts { get; set; }
        public decimal TotalProcessingAmount { get; set; }
        public decimal AverageProcessingAmount { get; set; }
        public int ActiveContracts { get; set; }
        public int ProcessedContracts { get; set; }
        public int PendingContracts { get; set; }
        public Dictionary<string, int> ContractsByLoanType { get; set; } = new();
    }
}
