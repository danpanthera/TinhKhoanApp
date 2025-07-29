using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.DTOs;
using TinhKhoanApp.Api.Repositories;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Dịch vụ xử lý dữ liệu LN01 (Thông tin khoản vay)
    /// </summary>
    public class LN01Service : ILN01Service
    {
        private readonly ILN01Repository _repository;

        public LN01Service(ILN01Repository repository)
        {
            _repository = repository;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN01DTO>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(LN01DTO.FromEntity);
        }

        /// <inheritdoc/>
        public async Task<LN01DTO?> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity != null ? LN01DTO.FromEntity(entity) : null;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN01DTO>> GetRecentAsync(int count = 10)
        {
            var entities = await _repository.GetRecentAsync(count);
            return entities.Select(LN01DTO.FromEntity);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN01DTO>> GetByDateAsync(DateTime date, int maxResults = 100)
        {
            var entities = await _repository.GetByDateAsync(date, maxResults);
            return entities.Select(LN01DTO.FromEntity);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN01DTO>> GetByBranchCodeAsync(string branchCode, int maxResults = 100)
        {
            var entities = await _repository.GetByBranchCodeAsync(branchCode, maxResults);
            return entities.Select(LN01DTO.FromEntity);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN01DTO>> GetByCustomerCodeAsync(string customerCode, int maxResults = 100)
        {
            var entities = await _repository.GetByCustomerCodeAsync(customerCode, maxResults);
            return entities.Select(LN01DTO.FromEntity);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN01DTO>> GetByAccountNumberAsync(string accountNumber, int maxResults = 100)
        {
            var entities = await _repository.GetByAccountNumberAsync(accountNumber, maxResults);
            return entities.Select(LN01DTO.FromEntity);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN01DTO>> GetByDebtGroupAsync(string debtGroup, int maxResults = 100)
        {
            var entities = await _repository.GetByDebtGroupAsync(debtGroup, maxResults);
            return entities.Select(LN01DTO.FromEntity);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LN01DTO>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, int maxResults = 100)
        {
            var entities = await _repository.GetByDateRangeAsync(fromDate, toDate, maxResults);
            return entities.Select(LN01DTO.FromEntity);
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalLoanAmountByBranchAsync(string branchCode, DateTime? date = null)
        {
            return await _repository.GetTotalLoanAmountByBranchAsync(branchCode, date);
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalLoanAmountByCurrencyAsync(string currency, DateTime? date = null)
        {
            return await _repository.GetTotalLoanAmountByCurrencyAsync(currency, date);
        }

        /// <inheritdoc/>
        public async Task<LN01DTO> CreateAsync(CreateLN01DTO createDto)
        {
            var entity = createDto.ToEntity();
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return LN01DTO.FromEntity(entity);
        }

        /// <inheritdoc/>
        public async Task<LN01DTO?> UpdateAsync(long id, UpdateLN01DTO updateDto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return null;
            }

            updateDto.UpdateEntity(entity);
            await _repository.SaveChangesAsync();
            return LN01DTO.FromEntity(entity);
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
            return await _repository.GetDbContext().LN01s.AnyAsync(e => e.Id == id);
        }

        /// <inheritdoc/>
        public async Task<bool> SaveChangesAsync()
        {
            return await _repository.SaveChangesAsync() > 0;
        }
    }
}
