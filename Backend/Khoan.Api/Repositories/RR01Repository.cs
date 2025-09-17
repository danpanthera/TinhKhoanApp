using Microsoft.EntityFrameworkCore;
using Khoan.Api.Data;
using Khoan.Api.Models.DataTables;

namespace Khoan.Api.Repositories
{
	/// <summary>
	/// RR01 Repository - triển khai IRR01Repository (dùng DataTables.RR01)
	/// </summary>
	public class RR01Repository : Repository<RR01>, IRR01Repository
	{
		public RR01Repository(ApplicationDbContext context) : base(context)
		{
		}

		/// <inheritdoc/>
		public new async Task<IEnumerable<RR01>> GetRecentAsync(int count = 10)
		{
			return await _dbSet
				.AsNoTracking()
				.OrderByDescending(x => x.CREATED_DATE)
				.Take(count)
				.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<RR01>> GetByDateAsync(DateTime date)
		{
			return await _dbSet
				.AsNoTracking()
				.Where(x => x.NGAY_DL.Date == date.Date)
				.OrderByDescending(x => x.CREATED_DATE)
				.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<RR01>> GetByBranchCodeAsync(string branchCode, int maxResults = 100)
		{
			if (string.IsNullOrWhiteSpace(branchCode)) return Array.Empty<RR01>();

			return await _dbSet
				.AsNoTracking()
				.Where(x => x.BRCD != null && x.BRCD == branchCode)
				.OrderByDescending(x => x.NGAY_DL)
				.Take(maxResults)
				.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<RR01>> GetByCurrencyTypeAsync(string currencyType, int maxResults = 100)
		{
			if (string.IsNullOrWhiteSpace(currencyType)) return Array.Empty<RR01>();

			return await _dbSet
				.AsNoTracking()
				.Where(x => x.CCY != null && x.CCY == currencyType)
				.OrderByDescending(x => x.NGAY_DL)
				.Take(maxResults)
				.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<decimal> GetTotalBalanceByBranchAsync(string branchCode, DateTime? date = null)
		{
			if (string.IsNullOrWhiteSpace(branchCode)) return 0m;

			var query = _dbSet.AsNoTracking().Where(x => x.BRCD == branchCode);
			if (date.HasValue)
			{
				var d = date.Value.Date;
				query = query.Where(x => x.NGAY_DL.Date == d);
			}

			// Chọn tổng dư nợ gốc hiện tại làm "Balance"
			var total = await query.SumAsync(x => x.DUNO_GOC_HIENTAI ?? 0m);
			return total;
		}

		/// <inheritdoc/>
		public async Task<bool> IsDuplicateAsync(string branchCode, string currencyType, DateTime dataDate)
		{
			var d = dataDate.Date;
			return await _dbSet
				.AsNoTracking()
				.AnyAsync(x => x.NGAY_DL.Date == d && x.BRCD == branchCode && x.CCY == currencyType);
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<string>> GetDistinctBranchCodesAsync(DateTime? date = null)
		{
			var query = _dbSet.AsNoTracking().Where(x => x.BRCD != null);
			if (date.HasValue)
			{
				var d = date.Value.Date;
				query = query.Where(x => x.NGAY_DL.Date == d);
			}

			return await query
				.Select(x => x.BRCD!)
				.Distinct()
				.OrderBy(x => x)
				.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<IEnumerable<string>> GetDistinctCurrencyTypesAsync(DateTime? date = null)
		{
			var query = _dbSet.AsNoTracking().Where(x => x.CCY != null);
			if (date.HasValue)
			{
				var d = date.Value.Date;
				query = query.Where(x => x.NGAY_DL.Date == d);
			}

			return await query
				.Select(x => x.CCY!)
				.Distinct()
				.OrderBy(x => x)
				.ToListAsync();
		}

		/// <inheritdoc/>
		public async Task<int> BulkInsertAsync(IEnumerable<RR01> entities)
		{
			if (entities == null) return 0;

			// Repository-level: dùng AddRangeAsync + SaveChangesAsync.
			// High-performance bulk copy được xử lý ở DirectImportService.
			await _dbSet.AddRangeAsync(entities);
			return await _context.SaveChangesAsync();
		}

		/// <inheritdoc/>
		public async Task<Dictionary<string, object>> GetStatisticsAsync(DateTime? date = null)
		{
			var query = _dbSet.AsNoTracking();
			if (date.HasValue)
			{
				var d = date.Value.Date;
				query = query.Where(x => x.NGAY_DL.Date == d);
			}

			var totalRecords = await query.CountAsync();
			var distinctBranches = await query.Where(x => x.BRCD != null).Select(x => x.BRCD!).Distinct().CountAsync();
			var distinctCurrencies = await query.Where(x => x.CCY != null).Select(x => x.CCY!).Distinct().CountAsync();
			var totalPrincipal = await query.SumAsync(x => x.DUNO_GOC_HIENTAI ?? 0m);
			var lastImportTime = await _dbSet.MaxAsync(x => (DateTime?)x.CREATED_DATE) ?? DateTime.MinValue;

			return new Dictionary<string, object>
			{
				["date"] = date?.Date.ToString("yyyy-MM-dd") ?? "ALL",
				["totalRecords"] = totalRecords,
				["distinctBranches"] = distinctBranches,
				["distinctCurrencies"] = distinctCurrencies,
				["totalPrincipal"] = totalPrincipal,
				["lastImportTime"] = lastImportTime,
			};
		}
	}
}
