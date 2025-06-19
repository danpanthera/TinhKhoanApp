-- Tạo chỉ tiêu KPI cho 5 chi nhánh mới thêm (TableId 34-38)
-- Mỗi chi nhánh có 11 chỉ tiêu giống nhau

-- Chi nhánh H. Tam Dương (7801) - TableId 34
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(34, 'Tổng vốn dư nợ', 'Chỉ tiêu tổng vốn dư nợ của chi nhánh', 10, 'Tỷ đồng', 1, 'NUMBER', 1, datetime('now')),
(34, 'Huy động tiền gửi', 'Chỉ tiêu huy động tiền gửi của chi nhánh', 15, 'Tỷ đồng', 2, 'NUMBER', 1, datetime('now')),
(34, 'Lợi nhuận', 'Chỉ tiêu lợi nhuận của chi nhánh', 20, 'Triệu đồng', 3, 'NUMBER', 1, datetime('now')),
(34, 'Nợ xấu (NPL)', 'Tỷ lệ nợ xấu của chi nhánh', 15, '%', 4, 'PERCENTAGE', 1, datetime('now')),
(34, 'Chi phí hoạt động (CIR)', 'Tỷ lệ chi phí hoạt động so với thu nhập của chi nhánh', 10, '%', 5, 'PERCENTAGE', 1, datetime('now')),
(34, 'Tỷ suất sinh lời trên tài sản (ROA)', 'Tỷ suất sinh lời trên tài sản của chi nhánh', 10, '%', 6, 'PERCENTAGE', 1, datetime('now')),
(34, 'Tỷ suất sinh lời trên vốn chủ sở hữu (ROE)', 'Tỷ suất sinh lời trên vốn chủ sở hữu của chi nhánh', 10, '%', 7, 'PERCENTAGE', 1, datetime('now')),
(34, 'Tỷ lệ CASA', 'Tỷ lệ tiền gửi không kỳ hạn và tiền gửi tiết kiệm của chi nhánh', 5, '%', 8, 'PERCENTAGE', 1, datetime('now')),
(34, 'Tỷ lệ cho vay trên huy động (LDR)', 'Tỷ lệ cho vay trên huy động của chi nhánh', 2.5, '%', 9, 'PERCENTAGE', 1, datetime('now')),
(34, 'Biên lãi suất thuần (NIM)', 'Biên lãi suất thuần của chi nhánh', 1.5, '%', 10, 'PERCENTAGE', 1, datetime('now')),
(34, 'Kiểm soát nội bộ', 'Điểm kiểm soát nội bộ của chi nhánh', 1, 'Điểm', 11, 'NUMBER', 1, datetime('now'));

-- Chi nhánh H. Phong Thổ (7802) - TableId 35
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(35, 'Tổng vốn dư nợ', 'Chỉ tiêu tổng vốn dư nợ của chi nhánh', 10, 'Tỷ đồng', 1, 'NUMBER', 1, datetime('now')),
(35, 'Huy động tiền gửi', 'Chỉ tiêu huy động tiền gửi của chi nhánh', 15, 'Tỷ đồng', 2, 'NUMBER', 1, datetime('now')),
(35, 'Lợi nhuận', 'Chỉ tiêu lợi nhuận của chi nhánh', 20, 'Triệu đồng', 3, 'NUMBER', 1, datetime('now')),
(35, 'Nợ xấu (NPL)', 'Tỷ lệ nợ xấu của chi nhánh', 15, '%', 4, 'PERCENTAGE', 1, datetime('now')),
(35, 'Chi phí hoạt động (CIR)', 'Tỷ lệ chi phí hoạt động so với thu nhập của chi nhánh', 10, '%', 5, 'PERCENTAGE', 1, datetime('now')),
(35, 'Tỷ suất sinh lời trên tài sản (ROA)', 'Tỷ suất sinh lời trên tài sản của chi nhánh', 10, '%', 6, 'PERCENTAGE', 1, datetime('now')),
(35, 'Tỷ suất sinh lời trên vốn chủ sở hữu (ROE)', 'Tỷ suất sinh lời trên vốn chủ sở hữu của chi nhánh', 10, '%', 7, 'PERCENTAGE', 1, datetime('now')),
(35, 'Tỷ lệ CASA', 'Tỷ lệ tiền gửi không kỳ hạn và tiền gửi tiết kiệm của chi nhánh', 5, '%', 8, 'PERCENTAGE', 1, datetime('now')),
(35, 'Tỷ lệ cho vay trên huy động (LDR)', 'Tỷ lệ cho vay trên huy động của chi nhánh', 2.5, '%', 9, 'PERCENTAGE', 1, datetime('now')),
(35, 'Biên lãi suất thuần (NIM)', 'Biên lãi suất thuần của chi nhánh', 1.5, '%', 10, 'PERCENTAGE', 1, datetime('now')),
(35, 'Kiểm soát nội bộ', 'Điểm kiểm soát nội bộ của chi nhánh', 1, 'Điểm', 11, 'NUMBER', 1, datetime('now'));

-- Chi nhánh H. Sin Hồ (7803) - TableId 36
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(36, 'Tổng vốn dư nợ', 'Chỉ tiêu tổng vốn dư nợ của chi nhánh', 10, 'Tỷ đồng', 1, 'NUMBER', 1, datetime('now')),
(36, 'Huy động tiền gửi', 'Chỉ tiêu huy động tiền gửi của chi nhánh', 15, 'Tỷ đồng', 2, 'NUMBER', 1, datetime('now')),
(36, 'Lợi nhuận', 'Chỉ tiêu lợi nhuận của chi nhánh', 20, 'Triệu đồng', 3, 'NUMBER', 1, datetime('now')),
(36, 'Nợ xấu (NPL)', 'Tỷ lệ nợ xấu của chi nhánh', 15, '%', 4, 'PERCENTAGE', 1, datetime('now')),
(36, 'Chi phí hoạt động (CIR)', 'Tỷ lệ chi phí hoạt động so với thu nhập của chi nhánh', 10, '%', 5, 'PERCENTAGE', 1, datetime('now')),
(36, 'Tỷ suất sinh lời trên tài sản (ROA)', 'Tỷ suất sinh lời trên tài sản của chi nhánh', 10, '%', 6, 'PERCENTAGE', 1, datetime('now')),
(36, 'Tỷ suất sinh lời trên vốn chủ sở hữu (ROE)', 'Tỷ suất sinh lời trên vốn chủ sở hữu của chi nhánh', 10, '%', 7, 'PERCENTAGE', 1, datetime('now')),
(36, 'Tỷ lệ CASA', 'Tỷ lệ tiền gửi không kỳ hạn và tiền gửi tiết kiệm của chi nhánh', 5, '%', 8, 'PERCENTAGE', 1, datetime('now')),
(36, 'Tỷ lệ cho vay trên huy động (LDR)', 'Tỷ lệ cho vay trên huy động của chi nhánh', 2.5, '%', 9, 'PERCENTAGE', 1, datetime('now')),
(36, 'Biên lãi suất thuần (NIM)', 'Biên lãi suất thuần của chi nhánh', 1.5, '%', 10, 'PERCENTAGE', 1, datetime('now')),
(36, 'Kiểm soát nội bộ', 'Điểm kiểm soát nội bộ của chi nhánh', 1, 'Điểm', 11, 'NUMBER', 1, datetime('now'));

-- Chi nhánh H. Mường Tè (7804) - TableId 37
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(37, 'Tổng vốn dư nợ', 'Chỉ tiêu tổng vốn dư nợ của chi nhánh', 10, 'Tỷ đồng', 1, 'NUMBER', 1, datetime('now')),
(37, 'Huy động tiền gửi', 'Chỉ tiêu huy động tiền gửi của chi nhánh', 15, 'Tỷ đồng', 2, 'NUMBER', 1, datetime('now')),
(37, 'Lợi nhuận', 'Chỉ tiêu lợi nhuận của chi nhánh', 20, 'Triệu đồng', 3, 'NUMBER', 1, datetime('now')),
(37, 'Nợ xấu (NPL)', 'Tỷ lệ nợ xấu của chi nhánh', 15, '%', 4, 'PERCENTAGE', 1, datetime('now')),
(37, 'Chi phí hoạt động (CIR)', 'Tỷ lệ chi phí hoạt động so với thu nhập của chi nhánh', 10, '%', 5, 'PERCENTAGE', 1, datetime('now')),
(37, 'Tỷ suất sinh lời trên tài sản (ROA)', 'Tỷ suất sinh lời trên tài sản của chi nhánh', 10, '%', 6, 'PERCENTAGE', 1, datetime('now')),
(37, 'Tỷ suất sinh lời trên vốn chủ sở hữu (ROE)', 'Tỷ suất sinh lời trên vốn chủ sở hữu của chi nhánh', 10, '%', 7, 'PERCENTAGE', 1, datetime('now')),
(37, 'Tỷ lệ CASA', 'Tỷ lệ tiền gửi không kỳ hạn và tiền gửi tiết kiệm của chi nhánh', 5, '%', 8, 'PERCENTAGE', 1, datetime('now')),
(37, 'Tỷ lệ cho vay trên huy động (LDR)', 'Tỷ lệ cho vay trên huy động của chi nhánh', 2.5, '%', 9, 'PERCENTAGE', 1, datetime('now')),
(37, 'Biên lãi suất thuần (NIM)', 'Biên lãi suất thuần của chi nhánh', 1.5, '%', 10, 'PERCENTAGE', 1, datetime('now')),
(37, 'Kiểm soát nội bộ', 'Điểm kiểm soát nội bộ của chi nhánh', 1, 'Điểm', 11, 'NUMBER', 1, datetime('now'));

-- Chi nhánh H. Than Uyên (7805) - TableId 38
INSERT INTO KpiIndicators (TableId, IndicatorName, Description, MaxScore, Unit, OrderIndex, ValueType, IsActive, CreatedDate) VALUES
(38, 'Tổng vốn dư nợ', 'Chỉ tiêu tổng vốn dư nợ của chi nhánh', 10, 'Tỷ đồng', 1, 'NUMBER', 1, datetime('now')),
(38, 'Huy động tiền gửi', 'Chỉ tiêu huy động tiền gửi của chi nhánh', 15, 'Tỷ đồng', 2, 'NUMBER', 1, datetime('now')),
(38, 'Lợi nhuận', 'Chỉ tiêu lợi nhuận của chi nhánh', 20, 'Triệu đồng', 3, 'NUMBER', 1, datetime('now')),
(38, 'Nợ xấu (NPL)', 'Tỷ lệ nợ xấu của chi nhánh', 15, '%', 4, 'PERCENTAGE', 1, datetime('now')),
(38, 'Chi phí hoạt động (CIR)', 'Tỷ lệ chi phí hoạt động so với thu nhập của chi nhánh', 10, '%', 5, 'PERCENTAGE', 1, datetime('now')),
(38, 'Tỷ suất sinh lời trên tài sản (ROA)', 'Tỷ suất sinh lời trên tài sản của chi nhánh', 10, '%', 6, 'PERCENTAGE', 1, datetime('now')),
(38, 'Tỷ suất sinh lời trên vốn chủ sở hữu (ROE)', 'Tỷ suất sinh lời trên vốn chủ sở hữu của chi nhánh', 10, '%', 7, 'PERCENTAGE', 1, datetime('now')),
(38, 'Tỷ lệ CASA', 'Tỷ lệ tiền gửi không kỳ hạn và tiền gửi tiết kiệm của chi nhánh', 5, '%', 8, 'PERCENTAGE', 1, datetime('now')),
(38, 'Tỷ lệ cho vay trên huy động (LDR)', 'Tỷ lệ cho vay trên huy động của chi nhánh', 2.5, '%', 9, 'PERCENTAGE', 1, datetime('now')),
(38, 'Biên lãi suất thuần (NIM)', 'Biên lãi suất thuần của chi nhánh', 1.5, '%', 10, 'PERCENTAGE', 1, datetime('now')),
(38, 'Kiểm soát nội bộ', 'Điểm kiểm soát nội bộ của chi nhánh', 1, 'Điểm', 11, 'NUMBER', 1, datetime('now'));

-- Kiểm tra tổng số chỉ tiêu cho từng chi nhánh
SELECT 'Tổng số chỉ tiêu KPI cho từng chi nhánh:' as message;
SELECT kat.TableName, COUNT(ki.Id) as TotalIndicators
FROM KpiAssignmentTables kat
LEFT JOIN KpiIndicators ki ON kat.Id = ki.TableId
WHERE kat.TableType >= 24
GROUP BY kat.Id, kat.TableName
ORDER BY kat.TableType;
