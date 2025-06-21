-- Insert basic dashboard indicators for testing
INSERT INTO DashboardIndicators (Code, Name, Unit, Icon, Color, Description, SortOrder, IsDeleted, IsActive) VALUES
('HuyDong', 'Nguồn vốn huy động', 'tỷ đồng', 'mdi-bank', '#4CAF50', 'Tổng nguồn vốn huy động từ khách hàng', 1, 0, 1),
('DuNo', 'Dư nợ cho vay', 'tỷ đồng', 'mdi-cash-multiple', '#2196F3', 'Tổng dư nợ cho vay khách hàng', 2, 0, 1),
('TyLeNoXau', 'Tỷ lệ nợ xấu', '%', 'mdi-alert-circle', '#FF9800', 'Tỷ lệ nợ xấu trên tổng dư nợ', 3, 0, 1),
('ThuHoiXLRR', 'Thu hồi nợ XLRR', 'triệu đồng', 'mdi-cash-remove', '#F44336', 'Thu hồi nợ xử lý rủi ro', 4, 0, 1),
('ThuDichVu', 'Thu nhập dịch vụ', 'triệu đồng', 'mdi-currency-usd', '#9C27B0', 'Thu nhập từ dịch vụ ngân hàng', 5, 0, 1),
('LoiNhuan', 'Lợi nhuận', 'triệu đồng', 'mdi-trending-up', '#4CAF50', 'Lợi nhuận trước thuế', 6, 0, 1);
