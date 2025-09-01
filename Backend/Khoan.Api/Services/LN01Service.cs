using Khoan.Api.Interfaces;
using Khoan.Api.Dtos.LN01;
using Khoan.Api.Models.Entities; // Fixed: Use Models.Entities for consistency
using Khoan.Api.Data;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace Khoan.Api.Services
{
    public class LN01Service : ILN01Service
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LN01Service> _logger;

        public LN01Service(ApplicationDbContext context, ILogger<LN01Service> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<LN01PreviewDto>> GetAllAsync()
        {
            return await _context.LN01
                .OrderByDescending(x => x.NGAY_DL)
                .Take(1000)
                .Select(x => new LN01PreviewDto
                {
                    Id = (int)x.Id, // Cast long to int
                    NGAY_DL = x.NGAY_DL,
                    BRCD = x.BRCD,
                    CUSTSEQ = x.CUSTSEQ,
                    CUSTNM = x.CUSTNM,
                    TAI_KHOAN = x.TAI_KHOAN,
                    CCY = x.CCY,
                    DU_NO = x.DU_NO, // Direct assignment - both are decimal?
                    LOAN_TYPE = x.LOAN_TYPE,
                    OFFICER_NAME = x.OFFICER_NAME
                })
                .ToListAsync();
        }

        public async Task<LN01DetailsDto> GetByIdAsync(int id)
        {
            var entity = await _context.LN01.FindAsync((long)id);
            if (entity == null) return null;

            return new LN01DetailsDto
            {
                Id = (int)entity.Id,
                NGAY_DL = entity.NGAY_DL,
                BRCD = entity.BRCD,
                CUSTSEQ = entity.CUSTSEQ,
                CUSTNM = entity.CUSTNM,
                TAI_KHOAN = entity.TAI_KHOAN,
                CCY = entity.CCY,
                DU_NO = entity.DU_NO,
                DSBSSEQ = entity.DSBSSEQ,
                TRANSACTION_DATE = entity.TRANSACTION_DATE,
                DSBSDT = entity.DSBSDT,
                DISBUR_CCY = entity.DISBUR_CCY,
                DISBURSEMENT_AMOUNT = entity.DISBURSEMENT_AMOUNT,
                DSBSMATDT = entity.DSBSMATDT,
                BSRTCD = entity.BSRTCD,
                INTEREST_RATE = entity.INTEREST_RATE,
                APPRSEQ = entity.APRSEQ,
                APPRDT = entity.APPRDT,
                APPR_CCY = entity.APPR_CCY,
                APPRAMT = entity.APPRAMT,
                APPRMATDT = entity.APPRMATDT,
                LOAN_TYPE = entity.LOAN_TYPE,
                FUND_RESOURCE_CODE = entity.FUND_RESOURCE_CODE,
                FUND_PURPOSE_CODE = entity.FUND_PURPOSE_CODE,
                REPAYMENT_AMOUNT = entity.REPAYMENT_AMOUNT,
                NEXT_REPAY_DATE = entity.NEXT_REPAY_DATE,
                NEXT_REPAY_AMOUNT = entity.NEXT_REPAY_AMOUNT,
                NEXT_INT_REPAY_DATE = entity.NEXT_INT_REPAY_DATE,
                OFFICER_ID = entity.OFFICER_ID,
                OFFICER_NAME = entity.OFFICER_NAME,
                INTEREST_AMOUNT = entity.INTEREST_AMOUNT,
                PASTDUE_INTEREST_AMOUNT = entity.PASTDUE_INTEREST_AMOUNT,
                TOTAL_INTEREST_REPAY_AMOUNT = entity.TOTAL_INTEREST_REPAY_AMOUNT,
                CUSTOMER_TYPE_CODE = entity.CUSTOMER_TYPE_CODE,
                CUSTOMER_TYPE_CODE_DETAIL = entity.CUSTOMER_TYPE_CODE_DETAIL,
                TRCTCD = entity.TRCTCD,
                TRCTNM = entity.TRCTNM,
                ADDR1 = entity.ADDR1,
                PROVINCE = entity.PROVINCE,
                LCLPROVINNM = entity.LCLPROVINNM,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public async Task<LN01DetailsDto> CreateAsync(LN01CreateDto dto)
        {
            var entity = new LN01Entity
            {
                NGAY_DL = dto.NGAY_DL ?? DateTime.Now,
                BRCD = dto.BRCD,
                CUSTNM = dto.CUSTNM,
                TAI_KHOAN = dto.TAI_KHOAN,
                CCY = dto.CCY,
                DU_NO = dto.DU_NO, // Keep as decimal?
                DISBURSEMENT_AMOUNT = dto.DISBURSEMENT_AMOUNT, // Keep as decimal?
                APPRAMT = dto.APPRAMT, // Keep as decimal?
                INTEREST_RATE = dto.INTEREST_RATE, // Keep as decimal?
                LOAN_TYPE = dto.LOAN_TYPE,
                OFFICER_ID = dto.OFFICER_ID,
                OFFICER_NAME = dto.OFFICER_NAME,
                TRANSACTION_DATE = dto.TRANSACTION_DATE, // Keep as DateTime?
                APPRDT = dto.APPRDT, // Keep as DateTime?
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.LN01.Add(entity);
            await _context.SaveChangesAsync();

            return await GetByIdAsync((int)entity.Id); // Cast long to int
        }

        public async Task<LN01DetailsDto> UpdateAsync(int id, LN01UpdateDto dto)
        {
            var entity = await _context.LN01.FindAsync(id);
            if (entity == null) return null;

            entity.CUSTNM = dto.CUSTNM ?? entity.CUSTNM;
            entity.DU_NO = dto.DU_NO ?? entity.DU_NO; // Keep as decimal?
            entity.DISBURSEMENT_AMOUNT = dto.DISBURSEMENT_AMOUNT ?? entity.DISBURSEMENT_AMOUNT; // Keep as decimal?
            entity.APPRAMT = dto.APPRAMT ?? entity.APPRAMT; // Keep as decimal?
            entity.UpdatedAt = DateTime.Now; // Fixed property name

            await _context.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.LN01.FindAsync(id);
            if (entity == null) return false;

            _context.LN01.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<LN01SummaryDto> GetSummaryAsync()
        {
            var totalRecords = await _context.LN01.CountAsync();
            var totalAmount = await _context.LN01
                .Where(x => x.DU_NO.HasValue)
                .SumAsync(x => x.DU_NO ?? 0); // Direct decimal? usage

            return new LN01SummaryDto
            {
                TotalLoans = totalRecords,
                TotalOutstanding = totalAmount,
                TotalDisbursement = totalAmount, // Simplified for now
                TotalApproved = totalAmount, // Simplified for now
                AverageInterestRate = 0, // Will calculate later
                ActiveLoans = totalRecords,
                OverdueLoans = 0, // Will calculate later
                LastUpdate = DateTime.Now
            };
        }

        public async Task<IEnumerable<LN01PreviewDto>> GetByBranchAsync(string branchCode)
        {
            return await _context.LN01
                .Where(x => x.BRCD == branchCode)
                .OrderByDescending(x => x.NGAY_DL)
                .Take(100)
                .Select(x => new LN01PreviewDto
                {
                    Id = (int)x.Id, // Cast long to int
                    NGAY_DL = x.NGAY_DL,
                    BRCD = x.BRCD,
                    CUSTSEQ = x.CUSTSEQ,
                    CUSTNM = x.CUSTNM,
                    TAI_KHOAN = x.TAI_KHOAN,
                    CCY = x.CCY,
                    DU_NO = x.DU_NO, // Direct use of decimal?
                    LOAN_TYPE = x.LOAN_TYPE,
                    OFFICER_NAME = x.OFFICER_NAME
                })
                .ToListAsync();
        }

        public async Task<LN01ImportResultDto> ImportFromCsvAsync(Stream csvStream, LN01ConfigDto? config = null)
        {
            var result = new LN01ImportResultDto
            {
                ImportDateTime = DateTime.Now,
                TotalRows = 0,
                SuccessRows = 0,
                ErrorRows = 0,
                Errors = new List<string>()
            };

            try
            {
                using var reader = new StreamReader(csvStream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                
                var records = csv.GetRecords<dynamic>().ToList();
                result.TotalRows = records.Count;

                foreach (var record in records)
                {
                    try
                    {
                        // Import logic will be implemented later
                        result.SuccessRows++;
                    }
                    catch (Exception ex)
                    {
                        result.ErrorRows++;
                        result.Errors.Add($"Row error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Import failed: {ex.Message}");
            }

            return result;
        }

        public async Task<Stream> ExportToCsvAsync()
        {
            var records = await GetAllAsync();
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            await csv.WriteRecordsAsync(records);
            await writer.FlushAsync();
            stream.Position = 0;
            return stream;
        }

        public async Task<IEnumerable<LN01PreviewDto>> GetRecentAsync(int count = 100)
        {
            return await _context.LN01
                .OrderByDescending(x => x.NGAY_DL)
                .Take(count)
                .Select(x => new LN01PreviewDto
                {
                    Id = (int)x.Id, // Cast long to int
                    NGAY_DL = x.NGAY_DL,
                    BRCD = x.BRCD,
                    CUSTSEQ = x.CUSTSEQ,
                    CUSTNM = x.CUSTNM,
                    TAI_KHOAN = x.TAI_KHOAN,
                    CCY = x.CCY,
                    DU_NO = x.DU_NO, // Direct use of decimal?
                    LOAN_TYPE = x.LOAN_TYPE,
                    OFFICER_NAME = x.OFFICER_NAME
                })
                .ToListAsync();
        }

        // Helper methods
        private static decimal? ParseDecimal(string value)
        {
            return decimal.TryParse(value, out var result) ? result : null;
        }

        private static DateTime? ParseDateTime(string value)
        {
            return DateTime.TryParse(value, out var result) ? result : null;
        }
    }
}
