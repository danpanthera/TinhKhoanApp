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
