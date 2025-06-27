using System.Collections.Generic;

namespace TinhKhoanApp.Api.Services
{
    /// <summary>
    /// Cấu hình mapping để đảm bảo tên cột CSV gốc được giữ nguyên trong database
    /// Model properties sẽ sử dụng tên CSV gốc, không chuẩn hóa sang tiếng Việt/PascalCase
    /// </summary>
    public static class CsvColumnMappingConfig
    {
        /// <summary>
        /// Mapping cho bảng LN01 (Loan data)
        /// Key: Tên property trong Model (giữ nguyên tên CSV gốc)
        /// Value: Tên cột CSV gốc (để validation)
        /// </summary>
        public static readonly Dictionary<string, string> LN01_ColumnMapping = new()
        {
            // Model Property (CSV gốc) → CSV Header (cũng chính là CSV gốc)
            { "BRCD", "BRCD" },
            { "CUSTSEQ", "CUSTSEQ" },
            { "CUSTNM", "CUSTNM" },
            { "TAI_KHOAN", "TAI_KHOAN" },
            { "CCY", "CCY" },
            { "DU_NO", "DU_NO" },
            { "DSBSSEQ", "DSBSSEQ" },
            { "TRANSACTION_DATE", "TRANSACTION_DATE" },
            { "DSBSDT", "DSBSDT" },
            { "DISBUR_CCY", "DISBUR_CCY" },
            { "DISBURSEMENT_AMOUNT", "DISBURSEMENT_AMOUNT" }
        };

        /// <summary>
        /// Mapping cho bảng GAHR26 (Employee data)
        /// Giữ nguyên tên cột CSV gốc làm property name
        /// </summary>
        public static readonly Dictionary<string, string> GAHR26_ColumnMapping = new()
        {
            // Model Property (CSV gốc) → CSV Header (cũng chính là CSV gốc)
            { "EMP_ID", "EMP_ID" },
            { "EMP_NAME", "EMP_NAME" },
            { "ID_NUMBER", "ID_NUMBER" },
            { "POSITION", "POSITION" },
            { "BRCD", "BRCD" },
            { "BRANCH_NAME", "BRANCH_NAME" },
            { "DEPARTMENT", "DEPARTMENT" },
            { "JOIN_DATE", "JOIN_DATE" },
            { "BIRTH_DATE", "BIRTH_DATE" },
            { "GENDER", "GENDER" },
            { "ADDRESS", "ADDRESS" },
            { "PHONE", "PHONE" },
            { "EMAIL", "EMAIL" },
            { "STATUS", "STATUS" },
            { "BASIC_SALARY", "BASIC_SALARY" },
            { "ALLOWANCE", "ALLOWANCE" },
            { "CREATED_DATE", "CREATED_DATE" },
            { "UPDATED_DATE", "UPDATED_DATE" }
        };

        /// <summary>
        /// Mapping cho bảng GLCB41 (General Ledger detail)
        /// Giữ nguyên tên cột CSV gốc làm property name
        /// </summary>
        public static readonly Dictionary<string, string> GLCB41_ColumnMapping = new()
        {
            // Model Property (CSV gốc) → CSV Header (cũng chính là CSV gốc)
            { "JOURNAL_NO", "JOURNAL_NO" },
            { "ACCOUNT_NO", "ACCOUNT_NO" },
            { "ACCOUNT_NAME", "ACCOUNT_NAME" },
            { "CUSTOMER_ID", "CUSTOMER_ID" },
            { "CUSTOMER_NAME", "CUSTOMER_NAME" },
            { "TRANSACTION_DATE", "TRANSACTION_DATE" },
            { "POSTING_DATE", "POSTING_DATE" },
            { "DESCRIPTION", "DESCRIPTION" },
            { "DEBIT_AMOUNT", "DEBIT_AMOUNT" },
            { "CREDIT_AMOUNT", "CREDIT_AMOUNT" },
            { "DEBIT_BALANCE", "DEBIT_BALANCE" },
            { "CREDIT_BALANCE", "CREDIT_BALANCE" },
            { "BRCD", "BRCD" },
            { "BRANCH_NAME", "BRANCH_NAME" },
            { "TRANSACTION_TYPE", "TRANSACTION_TYPE" },
            { "ORIGINAL_TRANS_ID", "ORIGINAL_TRANS_ID" },
            { "CREATED_DATE", "CREATED_DATE" },
            { "UPDATED_DATE", "UPDATED_DATE" }
        };

        /// <summary>
        /// Mapping cho bảng 7800_DT_KHKD1 (Business plan data)
        /// Giữ nguyên tên cột CSV gốc làm property name
        /// </summary>
        public static readonly Dictionary<string, string> DT_KHKD1_ColumnMapping = new()
        {
            // Model Property (CSV gốc) → CSV Header (cũng chính là CSV gốc)
            { "BRCD", "BRCD" },
            { "BRANCH_NAME", "BRANCH_NAME" },
            { "INDICATOR_TYPE", "INDICATOR_TYPE" },
            { "INDICATOR_NAME", "INDICATOR_NAME" },
            { "PLAN_YEAR", "PLAN_YEAR" },
            { "PLAN_QUARTER", "PLAN_QUARTER" },
            { "PLAN_MONTH", "PLAN_MONTH" },
            { "ACTUAL_YEAR", "ACTUAL_YEAR" },
            { "ACTUAL_QUARTER", "ACTUAL_QUARTER" },
            { "ACTUAL_MONTH", "ACTUAL_MONTH" },
            { "ACHIEVEMENT_RATE", "ACHIEVEMENT_RATE" },
            { "YEAR", "YEAR" },
            { "QUARTER", "QUARTER" },
            { "MONTH", "MONTH" },
            { "CREATED_DATE", "CREATED_DATE" },
            { "UPDATED_DATE", "UPDATED_DATE" }
        };

        /// <summary>
        /// Lấy mapping config dựa trên category
        /// Trả về mapping giữa Model property và CSV header (cả hai đều giữ nguyên tên CSV gốc)
        /// </summary>
        public static Dictionary<string, string>? GetColumnMappingForCategory(string category)
        {
            return category?.ToUpper() switch
            {
                "LN01" => LN01_ColumnMapping,
                "GAHR26" => GAHR26_ColumnMapping,
                "GLCB41" => GLCB41_ColumnMapping,
                "7800_DT_KHKD1" => DT_KHKD1_ColumnMapping,
                _ => null
            };
        }

        /// <summary>
        /// Lấy danh sách tên cột CSV hợp lệ cho category
        /// </summary>
        public static HashSet<string> GetValidCsvColumnsForCategory(string category)
        {
            var mapping = GetColumnMappingForCategory(category);
            return mapping?.Keys.ToHashSet() ?? new HashSet<string>();
        }

        /// <summary>
        /// Kiểm tra xem CSV header có valid không (tên cột có khớp với Model property không)
        /// </summary>
        public static bool IsValidCsvHeader(string category, string csvHeader)
        {
            var validColumns = GetValidCsvColumnsForCategory(category);
            return validColumns.Contains(csvHeader);
        }

        /// <summary>
        /// Validate tất cả các CSV headers có hợp lệ với category không
        /// </summary>
        public static (bool IsValid, List<string> InvalidHeaders, List<string> ValidHeaders) ValidateCsvHeaders(string category, IEnumerable<string> csvHeaders)
        {
            var validColumns = GetValidCsvColumnsForCategory(category);
            var validHeaders = new List<string>();
            var invalidHeaders = new List<string>();

            foreach (var header in csvHeaders)
            {
                if (validColumns.Contains(header))
                {
                    validHeaders.Add(header);
                }
                else
                {
                    invalidHeaders.Add(header);
                }
            }

            return (invalidHeaders.Count == 0, invalidHeaders, validHeaders);
        }
    }
}
