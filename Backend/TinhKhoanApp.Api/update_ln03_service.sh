#!/bin/bash
# update_ln03_service.sh
# Cập nhật DirectImportService để hỗ trợ LN03 với 20 cột

echo "🔧 UPDATING LN03 IN DIRECTIMPORTSERVICE"
echo "========================================"

echo "1. Backup DirectImportService hiện tại..."
cp Services/DirectImportService.cs Services/DirectImportService.cs.backup

echo "2. Cập nhật CreateLN03DataTable method..."

# Tạo patch script để cập nhật method CreateLN03DataTable
cat > update_ln03_datatable.patch << 'PATCH_EOF'
        /// <summary>
                 /// Tạo DataTable cho LN03 với đúng column types (20 business columns)
                 /// </summary>
        private void CreateLN03DataTable(DataTable dataTable, string[]? headers)
        {
            // CSV Business columns 1-17 (có header)
            dataTable.Columns.Add("MACHINHANH", typeof(string));
            dataTable.Columns.Add("TENCHINHANH", typeof(string));
            dataTable.Columns.Add("MAKH", typeof(string));
            dataTable.Columns.Add("TENKH", typeof(string));
            dataTable.Columns.Add("SOHOPDONG", typeof(string));
            dataTable.Columns.Add("SOTIENXLRR", typeof(decimal));
            dataTable.Columns.Add("NGAYPHATSINHXL", typeof(string));  // Keep as string for date parsing
            dataTable.Columns.Add("THUNOSAUXL", typeof(decimal));
            dataTable.Columns.Add("CONLAINGOAIBANG", typeof(decimal));
            dataTable.Columns.Add("DUNONOIBANG", typeof(decimal));
            dataTable.Columns.Add("NHOMNO", typeof(string));  // Changed to string
            dataTable.Columns.Add("MACBTD", typeof(string));
            dataTable.Columns.Add("TENCBTD", typeof(string));
            dataTable.Columns.Add("MAPGD", typeof(string));
            dataTable.Columns.Add("TAIKHOANHACHTOAN", typeof(string));
            dataTable.Columns.Add("REFNO", typeof(string));
            dataTable.Columns.Add("LOAINGUONVON", typeof(string));

            // CSV columns 18-20 (không có header trong file)
            dataTable.Columns.Add("MALOAI", typeof(string));           // Column 18: "100"
            dataTable.Columns.Add("LOAIKHACHHANG", typeof(string));    // Column 19: "Cá nhân"
            dataTable.Columns.Add("HANMUCPHEDUYET", typeof(decimal));  // Column 20: số tiền

            // System columns
            dataTable.Columns.Add("Id", typeof(long));
            dataTable.Columns.Add("NGAY_DL", typeof(DateTime));
            dataTable.Columns.Add("CreatedAt", typeof(DateTime));
            dataTable.Columns.Add("UpdatedAt", typeof(DateTime));
            dataTable.Columns.Add("ImportId", typeof(int));
        }
PATCH_EOF

echo "3. Cập nhật method ImportLN03DirectAsync để sử dụng custom parser..."

cat > ln03_import_method.cs << 'METHOD_EOF'
        /// <summary>
        /// Import LN03 - Bad debt data với 20 cột (17 có header + 3 không header)
        /// </summary>
        public async Task<DirectImportResult> ImportLN03DirectAsync(IFormFile file, string? statementDate = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.LogInformation("🚀 [LN03] Starting import với 20 cột support...");

            try
            {
                var content = await ReadFileContentAsync(file);
                var ngayDl = ExtractStatementDateFromContent(content, file.FileName, statementDate);

                // Sử dụng custom parser cho LN03
                var records = LN03CsvParser.ParseCsv(content, ngayDl);

                if (!records.Any())
                {
                    return new DirectImportResult
                    {
                        Success = false,
                        Message = "Không có dữ liệu hợp lệ trong file LN03",
                        RowsProcessed = 0
                    };
                }

                // Bulk insert using Entity Framework
                await _context.LN03.AddRangeAsync(records);
                var rowsAffected = await _context.SaveChangesAsync();

                stopwatch.Stop();
                _logger.LogInformation($"✅ [LN03] Import completed: {rowsAffected} rows in {stopwatch.ElapsedMilliseconds}ms");

                return new DirectImportResult
                {
                    Success = true,
                    Message = $"LN03 import thành công: {rowsAffected} rows",
                    RowsProcessed = rowsAffected,
                    ExecutionTimeMs = stopwatch.ElapsedMilliseconds
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "❌ [LN03] Import failed");

                return new DirectImportResult
                {
                    Success = false,
                    Message = $"LN03 import failed: {ex.Message}",
                    RowsProcessed = 0,
                    ExecutionTimeMs = stopwatch.ElapsedMilliseconds
                };
            }
        }
METHOD_EOF

echo "4. Cập nhật column mapping cho LN03..."

cat > update_column_mapping.cs << 'MAPPING_EOF'
            ["LN03"] = new[]
            {
                "MACHINHANH", "TENCHINHANH", "MAKH", "TENKH", "SOHOPDONG",
                "SOTIENXLRR", "NGAYPHATSINHXL", "THUNOSAUXL", "CONLAINGOAIBANG", "DUNONOIBANG",
                "NHOMNO", "MACBTD", "TENCBTD", "MAPGD", "TAIKHOANHACHTOAN",
                "REFNO", "LOAINGUONVON", "MALOAI", "LOAIKHACHHANG", "HANMUCPHEDUYET"
            },
MAPPING_EOF

echo "5. Register LN03CsvParser trong DI container..."

echo "Cần thêm vào Program.cs:"
echo "builder.Services.AddScoped<LN03CsvParser>();"

echo "6. Tạo test script cho LN03..."

cat > test_ln03_import.sh << 'TEST_EOF'
#!/bin/bash
echo "🧪 Testing LN03 Import với 20 cột..."

LN03_FILE="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_ln03_20241231.csv"

if [ -f "$LN03_FILE" ]; then
    echo "Testing với file: $LN03_FILE"

    # Test bằng curl
    curl -X POST \
      http://localhost:5055/api/DirectImport/smart \
      -F "file=@$LN03_FILE" \
      -F "statementDate=2024-12-31" \
      -H "Content-Type: multipart/form-data"

    echo ""
    echo "✅ Test completed!"
else
    echo "❌ File không tìm thấy: $LN03_FILE"
fi
TEST_EOF

chmod +x test_ln03_import.sh

echo "✅ LN03 SERVICE UPDATE COMPLETED!"
echo "================================="
echo "🎯 Files created:"
echo "   • update_ln03_datatable.patch - Patch cho DataTable"
echo "   • ln03_import_method.cs - Method import mới"
echo "   • update_column_mapping.cs - Column mapping"
echo "   • test_ln03_import.sh - Test script"
echo ""
echo "📋 Manual steps needed:"
echo "   1. Apply patches to DirectImportService.cs"
echo "   2. Add LN03CsvParser to DI in Program.cs"
echo "   3. Run migration: ./create_ln03_migration.sh"
echo "   4. Test: ./test_ln03_import.sh"
