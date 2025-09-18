using Microsoft.EntityFrameworkCore;
using Khoan.Api.Models.Dtos.EI01;
using Khoan.Api.Models.Entities;
using Khoan.Api.Repositories;
using Khoan.Api.Services.Interfaces;

namespace Khoan.Api.Services
{
    /// <summary>
    /// EI01 Service Implementation - E-Banking Information Service
    /// Follows DP01Service and DPDAService pattern
    /// </summary>
    public class EI01Service : IEI01Service
    {
        private readonly IEI01Repository _repository;
        private readonly ILogger<EI01Service> _logger;

        public EI01Service(IEI01Repository repository, ILogger<EI01Service> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // === BASIC CRUD OPERATIONS ===

        public async Task<IEnumerable<EI01PreviewDto>> GetEI01PreviewAsync(int count = 10)
        {
            try
            {
                var entities = await _repository.GetRecentAsync(count);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting EI01 preview data with count: {Count}", count);
                return Enumerable.Empty<EI01PreviewDto>();
            }
        }

        public async Task<EI01DetailsDto?> GetEI01DetailAsync(long id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync((int)id);
                return entity == null ? null : MapToDetailsDto(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting EI01 detail for ID: {Id}", id);
                return null;
            }
        }

        public async Task<long> CreateEI01Async(EI01CreateDto createDto)
        {
            try
            {
                var entity = MapFromCreateDto(createDto);
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = DateTime.UtcNow;

                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Created EI01 record with ID: {Id}", entity.Id);
                return entity.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating EI01 record");
                throw;
            }
        }

        public async Task<bool> UpdateEI01Async(long id, EI01UpdateDto updateDto)
        {
            try
            {
                var entity = await _repository.GetByIdAsync((int)id);
                if (entity == null)
                {
                    _logger.LogWarning("EI01 record not found for ID: {Id}", id);
                    return false;
                }

                MapFromUpdateDto(updateDto, entity);
                entity.UpdatedAt = DateTime.UtcNow;

                _repository.Update(entity);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Updated EI01 record with ID: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating EI01 record with ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteEI01Async(long id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync((int)id);
                if (entity == null)
                {
                    _logger.LogWarning("EI01 record not found for ID: {Id}", id);
                    return false;
                }

                _repository.Remove(entity);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Deleted EI01 record with ID: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting EI01 record with ID: {Id}", id);
                throw;
            }
        }

        // === BUSINESS QUERY OPERATIONS ===

        public async Task<IEnumerable<EI01PreviewDto>> GetEI01ByDateAsync(DateTime date)
        {
            try
            {
                var entities = await _repository.GetByDateAsync(date);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting EI01 by date: {Date}", date);
                return Enumerable.Empty<EI01PreviewDto>();
            }
        }

        public async Task<IEnumerable<EI01PreviewDto>> GetEI01ByBranchAsync(string branchCode, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByBranchCodeAsync(branchCode, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting EI01 by branch: {BranchCode}", branchCode);
                return Enumerable.Empty<EI01PreviewDto>();
            }
        }

        public async Task<IEnumerable<EI01PreviewDto>> GetEI01ByCustomerAsync(string customerCode, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByCustomerCodeAsync(customerCode, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting EI01 by customer: {CustomerCode}", customerCode);
                return Enumerable.Empty<EI01PreviewDto>();
            }
        }

        public async Task<IEnumerable<EI01PreviewDto>> GetEI01ByCustomerTypeAsync(string customerType, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByCustomerTypeAsync(customerType, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting EI01 by customer type: {CustomerType}", customerType);
                return Enumerable.Empty<EI01PreviewDto>();
            }
        }

        public async Task<IEnumerable<EI01PreviewDto>> GetEI01ByPhoneNumberAsync(string phoneNumber, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByPhoneNumberAsync(phoneNumber, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting EI01 by phone: {PhoneNumber}", phoneNumber);
                return Enumerable.Empty<EI01PreviewDto>();
            }
        }

        // === E-BANKING SERVICE SPECIFIC OPERATIONS ===

        public async Task<IEnumerable<EI01PreviewDto>> GetEI01ByEMBStatusAsync(string status, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByEMBStatusAsync(status, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting EI01 by EMB status: {Status}", status);
                return Enumerable.Empty<EI01PreviewDto>();
            }
        }

        public async Task<IEnumerable<EI01PreviewDto>> GetEI01ByOTTStatusAsync(string status, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByOTTStatusAsync(status, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting EI01 by OTT status: {Status}", status);
                return Enumerable.Empty<EI01PreviewDto>();
            }
        }

        public async Task<IEnumerable<EI01PreviewDto>> GetEI01BySMSStatusAsync(string status, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetBySMSStatusAsync(status, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting EI01 by SMS status: {Status}", status);
                return Enumerable.Empty<EI01PreviewDto>();
            }
        }

        public async Task<IEnumerable<EI01PreviewDto>> GetEI01ByRegistrationDateRangeAsync(DateTime fromDate, DateTime toDate, string serviceType, int maxResults = 100)
        {
            try
            {
                var entities = await _repository.GetByRegistrationDateRangeAsync(fromDate, toDate, serviceType, maxResults);
                return entities.Select(MapToPreviewDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting EI01 by registration date range: {FromDate} - {ToDate}, Service: {ServiceType}",
                    fromDate, toDate, serviceType);
                return Enumerable.Empty<EI01PreviewDto>();
            }
        }

        // === ANALYTICS & SUMMARY OPERATIONS ===

        public async Task<EI01SummaryDto> GetEI01SummaryByBranchAsync(string branchCode)
        {
            try
            {
                var entities = await _repository.GetByBranchCodeAsync(branchCode, int.MaxValue);
                var entityList = entities.ToList();

                return new EI01SummaryDto
                {
                    BranchCode = branchCode,
                    BranchName = branchCode, // TODO: Get branch name from lookup
                    TotalCustomers = entityList.Count,

                    // EMB Statistics
                    EMB_Active = entityList.Count(e => e.TRANG_THAI_EMB == "1"),
                    EMB_Inactive = entityList.Count(e => e.TRANG_THAI_EMB == "0"),
                    EMB_Total = entityList.Count(e => !string.IsNullOrEmpty(e.SDT_EMB)),

                    // OTT Statistics
                    OTT_Active = entityList.Count(e => e.TRANG_THAI_OTT == "1"),
                    OTT_Inactive = entityList.Count(e => e.TRANG_THAI_OTT == "0"),
                    OTT_Total = entityList.Count(e => !string.IsNullOrEmpty(e.SDT_OTT)),

                    // SMS Statistics
                    SMS_Active = entityList.Count(e => e.TRANG_THAI_SMS == "1"),
                    SMS_Inactive = entityList.Count(e => e.TRANG_THAI_SMS == "0"),
                    SMS_Total = entityList.Count(e => !string.IsNullOrEmpty(e.SDT_SMS)),

                    // SAV Statistics
                    SAV_Active = entityList.Count(e => e.TRANG_THAI_SAV == "1"),
                    SAV_Inactive = entityList.Count(e => e.TRANG_THAI_SAV == "0"),
                    SAV_Total = entityList.Count(e => !string.IsNullOrEmpty(e.SDT_SAV)),

                    // LN Statistics
                    LN_Active = entityList.Count(e => e.TRANG_THAI_LN == "1"),
                    LN_Inactive = entityList.Count(e => e.TRANG_THAI_LN == "0"),
                    LN_Total = entityList.Count(e => !string.IsNullOrEmpty(e.SDT_LN)),

                    LastUpdated = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting EI01 summary for branch: {BranchCode}", branchCode);
                return new EI01SummaryDto { BranchCode = branchCode, LastUpdated = DateTime.UtcNow };
            }
        }

        public async Task<int> GetCustomerCountByBranchAndServiceAsync(string branchCode, string serviceType, string status)
        {
            try
            {
                return await _repository.GetCustomerCountByBranchAndServiceAsync(branchCode, serviceType, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer count for branch: {BranchCode}, service: {ServiceType}, status: {Status}",
                    branchCode, serviceType, status);
                return 0;
            }
        }

        // === BATCH OPERATIONS ===

        public async Task<EI01ImportResultDto> ImportEI01FromCsvAsync(Stream csvStream, string fileName)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = new EI01ImportResultDto
            {
                FileName = fileName,
                ImportDate = DateTime.UtcNow,
                Errors = new List<string>(),
                Warnings = new List<string>()
            };

            try
            {
                // TODO: Implement CSV parsing logic
                // This is a placeholder implementation
                result.Success = false;
                result.Message = "CSV import not yet implemented";
                result.TotalRecords = 0;
                result.SuccessfulRecords = 0;
                result.FailedRecords = 0;

                _logger.LogWarning("EI01 CSV import called but not yet implemented for file: {FileName}", fileName);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                result.Errors.Add(ex.Message);
                _logger.LogError(ex, "Error importing EI01 from CSV: {FileName}", fileName);
            }
            finally
            {
                stopwatch.Stop();
                result.ProcessingTime = stopwatch.Elapsed;
            }

            return result;
        }

        public async Task<int> UpdateEI01BatchAsync(IEnumerable<EI01UpdateDto> updates)
        {
            try
            {
                // TODO: Implement batch update logic
                _logger.LogWarning("EI01 batch update called but not yet implemented");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EI01 batch update");
                throw;
            }
        }

        public async Task<int> DeleteEI01BatchAsync(IEnumerable<long> ids)
        {
            try
            {
                // TODO: Implement batch delete logic
                _logger.LogWarning("EI01 batch delete called but not yet implemented");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in EI01 batch delete");
                throw;
            }
        }

        // === PRIVATE MAPPING METHODS ===

        private EI01PreviewDto MapToPreviewDto(EI01Entity entity)
        {
            return new EI01PreviewDto
            {
                Id = entity.Id,
                NGAY_DL = entity.NGAY_DL,
                MA_CN = entity.MA_CN,
                MA_KH = entity.MA_KH,
                TEN_KH = entity.TEN_KH,
                LOAI_KH = entity.LOAI_KH,
                SDT_EMB = entity.SDT_EMB,
                TRANG_THAI_EMB = entity.TRANG_THAI_EMB,
                NGAY_DK_EMB = entity.NGAY_DK_EMB,
                SDT_OTT = entity.SDT_OTT,
                TRANG_THAI_OTT = entity.TRANG_THAI_OTT,
                NGAY_DK_OTT = entity.NGAY_DK_OTT,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        private EI01DetailsDto MapToDetailsDto(EI01Entity entity)
        {
            return new EI01DetailsDto
            {
                Id = entity.Id,
                NGAY_DL = entity.NGAY_DL,

                // Business columns
                MA_CN = entity.MA_CN,
                MA_KH = entity.MA_KH,
                TEN_KH = entity.TEN_KH,
                LOAI_KH = entity.LOAI_KH,

                // EMB Service
                SDT_EMB = entity.SDT_EMB,
                TRANG_THAI_EMB = entity.TRANG_THAI_EMB,
                NGAY_DK_EMB = entity.NGAY_DK_EMB,
                USER_EMB = entity.USER_EMB,

                // OTT Service
                SDT_OTT = entity.SDT_OTT,
                TRANG_THAI_OTT = entity.TRANG_THAI_OTT,
                NGAY_DK_OTT = entity.NGAY_DK_OTT,
                USER_OTT = entity.USER_OTT,

                // SMS Service
                SDT_SMS = entity.SDT_SMS,
                TRANG_THAI_SMS = entity.TRANG_THAI_SMS,
                NGAY_DK_SMS = entity.NGAY_DK_SMS,
                USER_SMS = entity.USER_SMS,

                // SAV Service
                SDT_SAV = entity.SDT_SAV,
                TRANG_THAI_SAV = entity.TRANG_THAI_SAV,
                NGAY_DK_SAV = entity.NGAY_DK_SAV,
                USER_SAV = entity.USER_SAV,

                // LN Service
                SDT_LN = entity.SDT_LN,
                TRANG_THAI_LN = entity.TRANG_THAI_LN,
                NGAY_DK_LN = entity.NGAY_DK_LN,
                USER_LN = entity.USER_LN,

                // System columns
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                SysStartTime = entity.SysStartTime,
                SysEndTime = entity.SysEndTime
            };
        }

        private EI01Entity MapFromCreateDto(EI01CreateDto dto)
        {
            return new EI01Entity
            {
                NGAY_DL = dto.NGAY_DL,

                // Business columns
                MA_CN = dto.MA_CN,
                MA_KH = dto.MA_KH,
                TEN_KH = dto.TEN_KH,
                LOAI_KH = dto.LOAI_KH,

                // EMB Service
                SDT_EMB = dto.SDT_EMB,
                TRANG_THAI_EMB = dto.TRANG_THAI_EMB,
                NGAY_DK_EMB = dto.NGAY_DK_EMB,
                USER_EMB = dto.USER_EMB,

                // OTT Service
                SDT_OTT = dto.SDT_OTT,
                TRANG_THAI_OTT = dto.TRANG_THAI_OTT,
                NGAY_DK_OTT = dto.NGAY_DK_OTT,
                USER_OTT = dto.USER_OTT,

                // SMS Service
                SDT_SMS = dto.SDT_SMS,
                TRANG_THAI_SMS = dto.TRANG_THAI_SMS,
                NGAY_DK_SMS = dto.NGAY_DK_SMS,
                USER_SMS = dto.USER_SMS,

                // SAV Service
                SDT_SAV = dto.SDT_SAV,
                TRANG_THAI_SAV = dto.TRANG_THAI_SAV,
                NGAY_DK_SAV = dto.NGAY_DK_SAV,
                USER_SAV = dto.USER_SAV,

                // LN Service
                SDT_LN = dto.SDT_LN,
                TRANG_THAI_LN = dto.TRANG_THAI_LN,
                NGAY_DK_LN = dto.NGAY_DK_LN,
                USER_LN = dto.USER_LN
            };
        }

        private void MapFromUpdateDto(EI01UpdateDto dto, EI01Entity entity)
        {
            if (dto.NGAY_DL.HasValue) entity.NGAY_DL = dto.NGAY_DL.Value;

            // Business columns
            if (dto.MA_CN != null) entity.MA_CN = dto.MA_CN;
            if (dto.MA_KH != null) entity.MA_KH = dto.MA_KH;
            if (dto.TEN_KH != null) entity.TEN_KH = dto.TEN_KH;
            if (dto.LOAI_KH != null) entity.LOAI_KH = dto.LOAI_KH;

            // EMB Service
            if (dto.SDT_EMB != null) entity.SDT_EMB = dto.SDT_EMB;
            if (dto.TRANG_THAI_EMB != null) entity.TRANG_THAI_EMB = dto.TRANG_THAI_EMB;
            if (dto.NGAY_DK_EMB.HasValue) entity.NGAY_DK_EMB = dto.NGAY_DK_EMB;
            if (dto.USER_EMB != null) entity.USER_EMB = dto.USER_EMB;

            // OTT Service
            if (dto.SDT_OTT != null) entity.SDT_OTT = dto.SDT_OTT;
            if (dto.TRANG_THAI_OTT != null) entity.TRANG_THAI_OTT = dto.TRANG_THAI_OTT;
            if (dto.NGAY_DK_OTT.HasValue) entity.NGAY_DK_OTT = dto.NGAY_DK_OTT;
            if (dto.USER_OTT != null) entity.USER_OTT = dto.USER_OTT;

            // SMS Service
            if (dto.SDT_SMS != null) entity.SDT_SMS = dto.SDT_SMS;
            if (dto.TRANG_THAI_SMS != null) entity.TRANG_THAI_SMS = dto.TRANG_THAI_SMS;
            if (dto.NGAY_DK_SMS.HasValue) entity.NGAY_DK_SMS = dto.NGAY_DK_SMS;
            if (dto.USER_SMS != null) entity.USER_SMS = dto.USER_SMS;

            // SAV Service
            if (dto.SDT_SAV != null) entity.SDT_SAV = dto.SDT_SAV;
            if (dto.TRANG_THAI_SAV != null) entity.TRANG_THAI_SAV = dto.TRANG_THAI_SAV;
            if (dto.NGAY_DK_SAV.HasValue) entity.NGAY_DK_SAV = dto.NGAY_DK_SAV;
            if (dto.USER_SAV != null) entity.USER_SAV = dto.USER_SAV;

            // LN Service
            if (dto.SDT_LN != null) entity.SDT_LN = dto.SDT_LN;
            if (dto.TRANG_THAI_LN != null) entity.TRANG_THAI_LN = dto.TRANG_THAI_LN;
            if (dto.NGAY_DK_LN.HasValue) entity.NGAY_DK_LN = dto.NGAY_DK_LN;
            if (dto.USER_LN != null) entity.USER_LN = dto.USER_LN;
        }
    }
}
