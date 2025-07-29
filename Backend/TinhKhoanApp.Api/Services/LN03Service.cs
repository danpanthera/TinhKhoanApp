using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Repositories;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Dịch vụ xử lý dữ liệu LN03 (Dữ liệu phục hồi khoản vay)
    /// </summary>
    public class LN03Service : ILN03Service
    {
        private readonly ILN03Repository _repository;

        public LN03Service(ILN03Repository repository)
        {
            _repository = repository;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03DTO>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(LN03DTO.FromEntity);
        }

        /// <inheritdoc/>
        public async Task<LN03DTO?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? LN03DTO.FromEntity(entity) : null;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03DTO>> GetRecentAsync(int count = 10)
        {
            var entities = await _repository.GetRecentAsync(count);
            return entities.Select(LN03DTO.FromEntity);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03DTO>> GetByDateAsync(DateTime date, int maxResults = 100)
        {
            var entities = await _repository.GetByDateAsync(date, maxResults);
            return entities.Select(LN03DTO.FromEntity);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03DTO>> GetByBranchCodeAsync(string branchCode, int maxResults = 100)
        {
            var entities = await _repository.GetByBranchCodeAsync(branchCode, maxResults);
            return entities.Select(LN03DTO.FromEntity);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03DTO>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100)
        {
            var entities = await _repository.GetByCustomerCodeAsync(customerCode, maxResults);
            return entities.Select(LN03DTO.FromEntity);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03DTO>> GetByOfficerCodeAsync(string officerCode, int maxResults = 100)
        {
            var entities = await _repository.GetByOfficerCodeAsync(officerCode, maxResults);
            return entities.Select(LN03DTO.FromEntity);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03DTO>> GetByDebtGroupAsync(string debtGroup, int maxResults = 100)
        {
            var entities = await _repository.GetByDebtGroupAsync(debtGroup, maxResults);
            return entities.Select(LN03DTO.FromEntity);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN03DTO>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int maxResults = 100)
        {
            var entities = await _repository.GetByDateRangeAsync(fromDate, toDate, maxResults);
            return entities.Select(LN03DTO.FromEntity);
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalRiskAmountByBranchAsync(string branchCode, DateTime? date = null)
        {
            return await _repository.GetTotalRiskAmountByBranchAsync(branchCode, date);
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalDebtRecoveryByBranchAsync(string branchCode, DateTime? date = null)
        {
            return await _repository.GetTotalDebtRecoveryByBranchAsync(branchCode, date);
        }

        /// <inheritdoc/>
        public async Task<LN03DTO> CreateAsync(CreateLN03DTO createDto)
        {
            var entity = createDto.ToEntity();
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return LN03DTO.FromEntity(entity);
        }

        /// <inheritdoc/>
        public async Task<LN03DTO?> UpdateAsync(long id, UpdateLN03DTO updateDto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return null;
            }

            updateDto.UpdateEntity(entity);
            await _repository.SaveChangesAsync();
            return LN03DTO.FromEntity(entity);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return false;
            }

            await _repository.DeleteAsync(entity);
            return await _repository.SaveChangesAsync() > 0;
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(long id)
        {
            return await _repository.GetDbContext().LN03s.AnyAsync(e => e.Id == id);
        }

        /// <inheritdoc/>
        public async Task<bool> SaveChangesAsync()
        {
            return await _repository.SaveChangesAsync() > 0;
        }
    }
}
