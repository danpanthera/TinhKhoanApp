using Microsoft.EntityFrameworkCore;
using TinhKhoanApp.Api.Data;
using TinhKhoanApp.Api.Models.DataTables;
using TinhKhoanApp.Api.Models.Common;
using TinhKhoanApp.Api.Repositories.Interfaces;
using TinhKhoanApp.Api.Models.Dtos;

namespace TinhKhoanApp.Api.Repositories
{
    /// <summary>
    /// Repository implementation for RR01 (Risk Report) data
    /// TEMPORARILY DISABLED - Interface compliance issues need to be resolved
    /// TODO: Fix interface implementation to match IRR01Repository requirements
    /// </summary>
    public class RR01Repository_DISABLED
    {
        private readonly ApplicationDbContext _context;

        public RR01Repository_DISABLED(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // TODO: Implement IRR01Repository interface properly
    }
}
