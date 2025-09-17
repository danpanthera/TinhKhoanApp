using Khoan.Api.Models.DataTables;

namespace Khoan.Api.Repositories
{
	/// <summary>
	/// Repository interface cho dữ liệu LN03 (Nợ xấu) theo schema CSV-first
	/// </summary>
	public interface ILN03Repository : IRepository<LN03>
	{
		Task<IEnumerable<LN03>> GetByDateAsync(DateTime date, int maxResults = 100);
		Task<IEnumerable<LN03>> GetByBranchAsync(string branchCode, int maxResults = 100);
		Task<IEnumerable<LN03>> GetByCustomerAsync(string customerCode, int maxResults = 100);
	}
}
