-- Tạo chỉ tiêu KPI cho 5 chi nhánh mới thêm (TableType 25-29)
-- Mỗi chi nhánh có 11 chỉ tiêu giống nhau

-- Chi nhánh H. Tam Dương (7801) - TableType 25
INSERT INTO KpiIndicators (IndicatorCode, IndicatorName, Unit, Weight, Description, KpiAssignmentTableId, IsActive, CreatedDate) VALUES
('CN_TVD', 'Tổng vốn dư nợ', 'Tỷ đồng', 10, 'Chỉ tiêu tổng vốn dư nợ của chi nhánh', 34, 1, datetime('now')),
('CN_HDD', 'Huy động tiền gửi', 'Tỷ đồng', 15, 'Chỉ tiêu huy động tiền gửi của chi nhánh', 34, 1, datetime('now')),
('CN_LN', 'Lợi nhuận', 'Triệu đồng', 20, 'Chỉ tiêu lợi nhuận của chi nhánh', 34, 1, datetime('now')),
('CN_NPL', 'Nợ xấu (NPL)', '%', 15, 'Tỷ lệ nợ xấu của chi nhánh', 34, 1, datetime('now')),
('CN_CIR', 'Chi phí hoạt động (CIR)', '%', 10, 'Tỷ lệ chi phí hoạt động so với thu nhập của chi nhánh', 34, 1, datetime('now')),
('CN_ROA', 'Tỷ suất sinh lời trên tài sản (ROA)', '%', 10, 'Tỷ suất sinh lời trên tài sản của chi nhánh', 34, 1, datetime('now')),
('CN_ROE', 'Tỷ suất sinh lời trên vốn chủ sở hữu (ROE)', '%', 10, 'Tỷ suất sinh lời trên vốn chủ sở hữu của chi nhánh', 34, 1, datetime('now')),
('CN_CASA', 'Tỷ lệ CASA', '%', 5, 'Tỷ lệ tiền gửi không kỳ hạn và tiền gửi tiết kiệm của chi nhánh', 34, 1, datetime('now')),
('CN_LDR', 'Tỷ lệ cho vay trên huy động (LDR)', '%', 2.5, 'Tỷ lệ cho vay trên huy động của chi nhánh', 34, 1, datetime('now')),
('CN_NIM', 'Biên lãi suất thuần (NIM)', '%', 1.5, 'Biên lãi suất thuần của chi nhánh', 34, 1, datetime('now')),
('CN_KSNB', 'Kiểm soát nội bộ', 'Điểm', 1, 'Điểm kiểm soát nội bộ của chi nhánh', 34, 1, datetime('now'));

-- Chi nhánh H. Phong Thổ (7802) - TableType 26
INSERT INTO KpiIndicators (IndicatorCode, IndicatorName, Unit, Weight, Description, KpiAssignmentTableId, IsActive, CreatedDate) VALUES
('CN_TVD', 'Tổng vốn dư nợ', 'Tỷ đồng', 10, 'Chỉ tiêu tổng vốn dư nợ của chi nhánh', 35, 1, datetime('now')),
('CN_HDD', 'Huy động tiền gửi', 'Tỷ đồng', 15, 'Chỉ tiêu huy động tiền gửi của chi nhánh', 35, 1, datetime('now')),
('CN_LN', 'Lợi nhuận', 'Triệu đồng', 20, 'Chỉ tiêu lợi nhuận của chi nhánh', 35, 1, datetime('now')),
('CN_NPL', 'Nợ xấu (NPL)', '%', 15, 'Tỷ lệ nợ xấu của chi nhánh', 35, 1, datetime('now')),
('CN_CIR', 'Chi phí hoạt động (CIR)', '%', 10, 'Tỷ lệ chi phí hoạt động so với thu nhập của chi nhánh', 35, 1, datetime('now')),
('CN_ROA', 'Tỷ suất sinh lời trên tài sản (ROA)', '%', 10, 'Tỷ suất sinh lời trên tài sản của chi nhánh', 35, 1, datetime('now')),
('CN_ROE', 'Tỷ suất sinh lời trên vốn chủ sở hữu (ROE)', '%', 10, 'Tỷ suất sinh lời trên vốn chủ sở hữu của chi nhánh', 35, 1, datetime('now')),
('CN_CASA', 'Tỷ lệ CASA', '%', 5, 'Tỷ lệ tiền gửi không kỳ hạn và tiền gửi tiết kiệm của chi nhánh', 35, 1, datetime('now')),
('CN_LDR', 'Tỷ lệ cho vay trên huy động (LDR)', '%', 2.5, 'Tỷ lệ cho vay trên huy động của chi nhánh', 35, 1, datetime('now')),
('CN_NIM', 'Biên lãi suất thuần (NIM)', '%', 1.5, 'Biên lãi suất thuần của chi nhánh', 35, 1, datetime('now')),
('CN_KSNB', 'Kiểm soát nội bộ', 'Điểm', 1, 'Điểm kiểm soát nội bộ của chi nhánh', 35, 1, datetime('now'));

-- Chi nhánh H. Sin Hồ (7803) - TableType 27
INSERT INTO KpiIndicators (IndicatorCode, IndicatorName, Unit, Weight, Description, KpiAssignmentTableId, IsActive, CreatedDate) VALUES
('CN_TVD', 'Tổng vốn dư nợ', 'Tỷ đồng', 10, 'Chỉ tiêu tổng vốn dư nợ của chi nhánh', 36, 1, datetime('now')),
('CN_HDD', 'Huy động tiền gửi', 'Tỷ đồng', 15, 'Chỉ tiêu huy động tiền gửi của chi nhánh', 36, 1, datetime('now')),
('CN_LN', 'Lợi nhuận', 'Triệu đồng', 20, 'Chỉ tiêu lợi nhuận của chi nhánh', 36, 1, datetime('now')),
('CN_NPL', 'Nợ xấu (NPL)', '%', 15, 'Tỷ lệ nợ xấu của chi nhánh', 36, 1, datetime('now')),
('CN_CIR', 'Chi phí hoạt động (CIR)', '%', 10, 'Tỷ lệ chi phí hoạt động so với thu nhập của chi nhánh', 36, 1, datetime('now')),
('CN_ROA', 'Tỷ suất sinh lời trên tài sản (ROA)', '%', 10, 'Tỷ suất sinh lời trên tài sản của chi nhánh', 36, 1, datetime('now')),
('CN_ROE', 'Tỷ suất sinh lời trên vốn chủ sở hữu (ROE)', '%', 10, 'Tỷ suất sinh lời trên vốn chủ sở hữu của chi nhánh', 36, 1, datetime('now')),
('CN_CASA', 'Tỷ lệ CASA', '%', 5, 'Tỷ lệ tiền gửi không kỳ hạn và tiền gửi tiết kiệm của chi nhánh', 36, 1, datetime('now')),
('CN_LDR', 'Tỷ lệ cho vay trên huy động (LDR)', '%', 2.5, 'Tỷ lệ cho vay trên huy động của chi nhánh', 36, 1, datetime('now')),
('CN_NIM', 'Biên lãi suất thuần (NIM)', '%', 1.5, 'Biên lãi suất thuần của chi nhánh', 36, 1, datetime('now')),
('CN_KSNB', 'Kiểm soát nội bộ', 'Điểm', 1, 'Điểm kiểm soát nội bộ của chi nhánh', 36, 1, datetime('now'));

-- Chi nhánh H. Mường Tè (7804) - TableType 28
INSERT INTO KpiIndicators (IndicatorCode, IndicatorName, Unit, Weight, Description, KpiAssignmentTableId, IsActive, CreatedDate) VALUES
('CN_TVD', 'Tổng vốn dư nợ', 'Tỷ đồng', 10, 'Chỉ tiêu tổng vốn dư nợ của chi nhánh', 37, 1, datetime('now')),
('CN_HDD', 'Huy động tiền gửi', 'Tỷ đồng', 15, 'Chỉ tiêu huy động tiền gửi của chi nhánh', 37, 1, datetime('now')),
('CN_LN', 'Lợi nhuận', 'Triệu đồng', 20, 'Chỉ tiêu lợi nhuận của chi nhánh', 37, 1, datetime('now')),
('CN_NPL', 'Nợ xấu (NPL)', '%', 15, 'Tỷ lệ nợ xấu của chi nhánh', 37, 1, datetime('now')),
('CN_CIR', 'Chi phí hoạt động (CIR)', '%', 10, 'Tỷ lệ chi phí hoạt động so với thu nhập của chi nhánh', 37, 1, datetime('now')),
('CN_ROA', 'Tỷ suất sinh lời trên tài sản (ROA)', '%', 10, 'Tỷ suất sinh lời trên tài sản của chi nhánh', 37, 1, datetime('now')),
('CN_ROE', 'Tỷ suất sinh lời trên vốn chủ sở hữu (ROE)', '%', 10, 'Tỷ suất sinh lời trên vốn chủ sở hữu của chi nhánh', 37, 1, datetime('now')),
('CN_CASA', 'Tỷ lệ CASA', '%', 5, 'Tỷ lệ tiền gửi không kỳ hạn và tiền gửi tiết kiệm của chi nhánh', 37, 1, datetime('now')),
('CN_LDR', 'Tỷ lệ cho vay trên huy động (LDR)', '%', 2.5, 'Tỷ lệ cho vay trên huy động của chi nhánh', 37, 1, datetime('now')),
('CN_NIM', 'Biên lãi suất thuần (NIM)', '%', 1.5, 'Biên lãi suất thuần của chi nhánh', 37, 1, datetime('now')),
('CN_KSNB', 'Kiểm soát nội bộ', 'Điểm', 1, 'Điểm kiểm soát nội bộ của chi nhánh', 37, 1, datetime('now'));

-- Chi nhánh H. Than Uyên (7805) - TableType 29
INSERT INTO KpiIndicators (IndicatorCode, IndicatorName, Unit, Weight, Description, KpiAssignmentTableId, IsActive, CreatedDate) VALUES
('CN_TVD', 'Tổng vốn dư nợ', 'Tỷ đồng', 10, 'Chỉ tiêu tổng vốn dư nợ của chi nhánh', 38, 1, datetime('now')),
('CN_HDD', 'Huy động tiền gửi', 'Tỷ đồng', 15, 'Chỉ tiêu huy động tiền gửi của chi nhánh', 38, 1, datetime('now')),
('CN_LN', 'Lợi nhuận', 'Triệu đồng', 20, 'Chỉ tiêu lợi nhuận của chi nhánh', 38, 1, datetime('now')),
('CN_NPL', 'Nợ xấu (NPL)', '%', 15, 'Tỷ lệ nợ xấu của chi nhánh', 38, 1, datetime('now')),
('CN_CIR', 'Chi phí hoạt động (CIR)', '%', 10, 'Tỷ lệ chi phí hoạt động so với thu nhập của chi nhánh', 38, 1, datetime('now')),
('CN_ROA', 'Tỷ suất sinh lời trên tài sản (ROA)', '%', 10, 'Tỷ suất sinh lời trên tài sản của chi nhánh', 38, 1, datetime('now')),
('CN_ROE', 'Tỷ suất sinh lời trên vốn chủ sở hữu (ROE)', '%', 10, 'Tỷ suất sinh lời trên vốn chủ sở hữu của chi nhánh', 38, 1, datetime('now')),
('CN_CASA', 'Tỷ lệ CASA', '%', 5, 'Tỷ lệ tiền gửi không kỳ hạn và tiền gửi tiết kiệm của chi nhánh', 38, 1, datetime('now')),
('CN_LDR', 'Tỷ lệ cho vay trên huy động (LDR)', '%', 2.5, 'Tỷ lệ cho vay trên huy động của chi nhánh', 38, 1, datetime('now')),
('CN_NIM', 'Biên lãi suất thuần (NIM)', '%', 1.5, 'Biên lãi suất thuần của chi nhánh', 38, 1, datetime('now')),
('CN_KSNB', 'Kiểm soát nội bộ', 'Điểm', 1, 'Điểm kiểm soát nội bộ của chi nhánh', 38, 1, datetime('now'));

-- Kiểm tra tổng số chỉ tiêu cho từng chi nhánh
SELECT 'Tổng số chỉ tiêu KPI cho từng chi nhánh:' as message;
SELECT kat.TableName, COUNT(ki.Id) as TotalIndicators
FROM KpiAssignmentTables kat
LEFT JOIN KpiIndicators ki ON kat.Id = ki.KpiAssignmentTableId
WHERE kat.TableType >= 24
GROUP BY kat.Id, kat.TableName
ORDER BY kat.TableType;
