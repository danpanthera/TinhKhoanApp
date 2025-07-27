-- FIX ENCODING CHO TẤT CẢ 158 CHỈ TIÊU KPI CÁN BỘ
-- Script sửa triệt để tất cả lỗi encoding tiếng Việt

-- 1. Chỉ tiêu cơ bản có mẫu chung
UPDATE KpiIndicators SET IndicatorName = N'Lợi nhuận khoán tài chính' WHERE IndicatorName LIKE '%L?i nhu?n khoán tài chính%';
UPDATE KpiIndicators SET IndicatorName = N'Phát triển Khách hàng Doanh nghiệp' WHERE IndicatorName = 'Phát tri?n Khách hàng Doanh nghi?p';
UPDATE KpiIndicators SET IndicatorName = N'Phát triển Khách hàng Cá nhân' WHERE IndicatorName = 'Phát tri?n Khách hàng Cá nhân';
UPDATE KpiIndicators SET IndicatorName = N'Phát triển Khách hàng' WHERE IndicatorName = 'Phát tri?n Khách hàng';
UPDATE KpiIndicators SET IndicatorName = N'Điều hành theo chương trình công tác' WHERE IndicatorName = 'Ði?u hành theo chuong trình công tác';
UPDATE KpiIndicators SET IndicatorName = N'Chấp hành quy chế, quy trình nghiệp vụ' WHERE IndicatorName = 'Ch?p hành quy ch?, quy trình nghi?p v?';
UPDATE KpiIndicators SET IndicatorName = N'BQ kết quả thực hiện CB trong phòng mình phụ trách' WHERE IndicatorName = 'BQ k?t qu? th?c hi?n CB trong phòng mình ph? trách';
UPDATE KpiIndicators SET IndicatorName = N'Tổng Dư nợ KHCN' WHERE IndicatorName = 'T?ng Du n? KHCN';
UPDATE KpiIndicators SET IndicatorName = N'Tỷ lệ nợ xấu KHCN' WHERE IndicatorName = 'T? l? n? x?u KHCN';
UPDATE KpiIndicators SET IndicatorName = N'Thu nợ đã XLRR KHCN' WHERE IndicatorName = 'Thu n? dã XLRR KHCN';
UPDATE KpiIndicators SET IndicatorName = N'Tổng Dư nợ KHDN' WHERE IndicatorName = 'T?ng Du n? KHDN';
UPDATE KpiIndicators SET IndicatorName = N'Tỷ lệ nợ xấu KHDN' WHERE IndicatorName = 'T? l? n? x?u KHDN';
UPDATE KpiIndicators SET IndicatorName = N'Thu nợ đã XLRR KHDN' WHERE IndicatorName = 'Thu n? dã XLRR KHDN';

-- 2. Chỉ tiêu về nguồn vốn
UPDATE KpiIndicators SET IndicatorName = N'Tổng nguồn vốn' WHERE IndicatorName = 'T?ng ngu?n v?n';
UPDATE KpiIndicators SET IndicatorName = N'Tổng dư nợ' WHERE IndicatorName = 'T?ng du n?';
UPDATE KpiIndicators SET IndicatorName = N'Tổng nguồn vốn huy động BQ' WHERE IndicatorName = 'T?ng ngu?n v?n huy d?ng BQ';
UPDATE KpiIndicators SET IndicatorName = N'Tổng nguồn vốn BQ' WHERE IndicatorName = 'T?ng ngu?n v?n BQ';
UPDATE KpiIndicators SET IndicatorName = N'Tổng dư nợ BQ' WHERE IndicatorName = 'T?ng du n? BQ';
UPDATE KpiIndicators SET IndicatorName = N'Tổng nguồn vốn cuối kỳ' WHERE IndicatorName = 'T?ng ngu?n v?n cu?i k?';
UPDATE KpiIndicators SET IndicatorName = N'Tổng nguồn vốn huy động BQ trong kỳ' WHERE IndicatorName = 'T?ng ngu?n v?n huy d?ng BQ trong k?';
UPDATE KpiIndicators SET IndicatorName = N'Tổng dư nợ cuối kỳ' WHERE IndicatorName = 'T?ng du n? cu?i k?';
UPDATE KpiIndicators SET IndicatorName = N'Tổng dư nợ BQ trong kỳ' WHERE IndicatorName = 'T?ng du n? BQ trong k?';
UPDATE KpiIndicators SET IndicatorName = N'Tổng dư nợ HSX&CN' WHERE IndicatorName = 'T?ng du n? HSX&CN';
UPDATE KpiIndicators SET IndicatorName = N'Tổng dư nợ cho vay' WHERE IndicatorName = 'T?ng du n? cho vay';
UPDATE KpiIndicators SET IndicatorName = N'Tổng dư nợ cho vay HSX&CN' WHERE IndicatorName = 'T?ng du n? cho vay HSX&CN';

-- 3. Chỉ tiêu về tỷ lệ nợ xấu
UPDATE KpiIndicators SET IndicatorName = N'Tỷ lệ nợ xấu' WHERE IndicatorName = 'T? l? n? x?u';
UPDATE KpiIndicators SET IndicatorName = N'Tỷ lệ nợ xấu nội bảng' WHERE IndicatorName = 'T? l? n? x?u n?i b?ng';

-- 4. Chỉ tiêu về thu nợ
UPDATE KpiIndicators SET IndicatorName = N'Thu nợ đã XLRR' WHERE IndicatorName = 'Thu n? dã XLRR';
UPDATE KpiIndicators SET IndicatorName = N'Thu nợ đã XLRR (nếu không có nợ XLRR thì cộng vào chỉ tiêu Dư nợ)' WHERE IndicatorName = 'Thu n? dã XLRR (n?u không có n? XLRR thì c?ng vào ch? tiêu Du n?)';
UPDATE KpiIndicators SET IndicatorName = N'Thu nợ đã XLRR (nếu không có thì cộng vào chỉ tiêu dư nợ)' WHERE IndicatorName = 'Thu n? dã XLRR (n?u không có thì c?ng vào ch? tiêu du n?)';
UPDATE KpiIndicators SET IndicatorName = N'Thu nợ đã xử lý' WHERE IndicatorName = 'Thu n? dã x? lý';

-- 5. Chỉ tiêu về thực hiện nhiệm vụ
UPDATE KpiIndicators SET IndicatorName = N'Thực hiện nhiệm vụ theo chương trình công tác, chức năng nhiệm vụ của phòng' WHERE IndicatorName = 'Th?c hi?n nhi?m v? theo chuong trình công tác, ch?c nang nhi?m v? c?a phòng';
UPDATE KpiIndicators SET IndicatorName = N'Thực hiện nhiệm vụ theo chương trình công tác' WHERE IndicatorName = 'Th?c hi?n nhi?m v? theo chuong trình công tác';
UPDATE KpiIndicators SET IndicatorName = N'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ được giao' WHERE IndicatorName = 'Th?c hi?n nhi?m v? theo chuong trình công tác, các công vi?c theo ch?c nang nhi?m v? du?c giao';
UPDATE KpiIndicators SET IndicatorName = N'Thực hiện nhiệm vụ theo chương trình công tác, các công việc theo chức năng nhiệm vụ của phòng' WHERE IndicatorName = 'Th?c hi?n nhi?m v? theo chuong trình công tác, các công vi?c theo ch?c nang nhi?m v? c?a phòng';
UPDATE KpiIndicators SET IndicatorName = N'Thực hiện chức năng, nhiệm vụ được giao' WHERE IndicatorName = 'Th?c hi?n ch?c nang, nhi?m v? du?c giao';

-- 6. Chỉ tiêu về chấp hành quy chế
UPDATE KpiIndicators SET IndicatorName = N'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank' WHERE IndicatorName = 'Ch?p hành quy ch?, quy trình nghi?p v?, van hóa Agribank';
UPDATE KpiIndicators SET IndicatorName = N'Chấp hành quy chế, quy trình nghiệp vụ, nội dung chỉ đạo, điều hành của CNL1, văn hóa Agribank' WHERE IndicatorName = 'Ch?p hành quy ch?, quy trình nghi?p v?, n?i dung ch? d?o, di?u hành c?a CNL1, van hóa Agribank';

-- 7. Chỉ tiêu về kết quả thực hiện
UPDATE KpiIndicators SET IndicatorName = N'Kết quả thực hiện BQ của CB trong phòng' WHERE IndicatorName = 'K?t qu? th?c hi?n BQ c?a CB trong phòng';
UPDATE KpiIndicators SET IndicatorName = N'Kết quả thực hiện BQ của CB trong phòng mình phụ trách' WHERE IndicatorName = 'K?t qu? th?c hi?n BQ c?a CB trong phòng mình ph? trách';
UPDATE KpiIndicators SET IndicatorName = N'Kết quả thực hiện BQ của CB thuộc mình phụ trách' WHERE IndicatorName = 'K?t qu? th?c hi?n BQ c?a CB thu?c mình ph? trách';
UPDATE KpiIndicators SET IndicatorName = N'BQ kết quả thực hiện của CB trong phòng' WHERE IndicatorName = 'BQ k?t qu? th?c hi?n c?a CB trong phòng';
UPDATE KpiIndicators SET IndicatorName = N'Kết quả thực hiện BQ của cán bộ trong phòng' WHERE IndicatorName = 'K?t qu? th?c hi?n BQ c?a cán b? trong phòng';

-- 8. Chỉ tiêu về dịch vụ
UPDATE KpiIndicators SET IndicatorName = N'Thu dịch vụ thanh toán trong nước' WHERE IndicatorName = 'Thu d?ch v? thanh toán trong nu?c';
UPDATE KpiIndicators SET IndicatorName = N'Thu dịch vụ' WHERE IndicatorName = 'Thu d?ch v?';
UPDATE KpiIndicators SET IndicatorName = N'Hoàn thành chỉ tiêu giao khoán SPDV' WHERE IndicatorName = 'Hoàn thành ch? tiêu giao khoán SPDV';
UPDATE KpiIndicators SET IndicatorName = N'Tổng doanh thu phí dịch vụ' WHERE IndicatorName = 'T?ng doanh thu phí d?ch v?';

-- 9. Chỉ tiêu về bút toán và số thẻ
UPDATE KpiIndicators SET IndicatorName = N'Số bút toán giao dịch BQ' WHERE IndicatorName = 'S? bút toán giao d?ch BQ';
UPDATE KpiIndicators SET IndicatorName = N'Số bút toán hủy' WHERE IndicatorName = 'S? bút toán h?y';
UPDATE KpiIndicators SET IndicatorName = N'Số thẻ phát hành' WHERE IndicatorName = 'S? th? phát hành';

-- 10. Chỉ tiêu điều hành và thực hiện nhiệm vụ kết hợp
UPDATE KpiIndicators SET IndicatorName = N'Điều hành theo chương trình công tác, nhiệm vụ được giao' WHERE IndicatorName = 'Ði?u hành theo chuong trình công tác, nhi?m v? du?c giao';
UPDATE KpiIndicators SET IndicatorName = N'Điều hành theo chương trình công tác, chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank' WHERE IndicatorName = 'Ði?u hành theo chuong trình công tác, ch?p hành quy ch?, quy trình nghi?p v?, van hóa Agribank';

PRINT 'ĐÃ FIX XONG TẤT CẢ LỖI ENCODING TIẾNG VIỆT!';
