#!/bin/bash

# Script để kiểm tra các tính năng đã cập nhật trong Dashboard

echo "=== KIỂM TRA TÍNH NĂNG HIỂN THỊ CHỈ TIÊU VÀ DANH SÁCH ĐƠN VỊ ==="
echo ""

# Kiểm tra endpoint API đơn vị để xác nhận danh sách đơn vị
echo "1. Kiểm tra danh sách đơn vị từ API:"
curl -s http://localhost:5055/api/units | jq '.[] | {id, name}'
echo ""

# Kiểm tra endpoint Dashboard data
echo "2. Kiểm tra dữ liệu Dashboard API:"
curl -s "http://localhost:5055/api/dashboard/dashboard-data?date=$(date +%Y-%m-%d)" | jq '.indicators[] | {code, name}'
echo ""

echo "3. Xác nhận 6 chỉ tiêu chính:"
echo "- Nguồn vốn"
echo "- Dư nợ"
echo "- Tỷ lệ nợ xấu"
echo "- Thu nợ đã XLRR"
echo "- Thu dịch vụ"
echo "- Lợi nhuận khoán tài chính"
echo ""

echo "4. Xác nhận chi tiết tại frontend:"
echo "- Frontend đang chạy tại: http://localhost:3000/dashboard"
echo "- Hãy kiểm tra:"
echo "  + Danh sách đơn vị khi thêm chỉ tiêu mới đã được sắp xếp đúng từ CN Lai Châu đến CN Nậm Nhùn"
echo "  + Bảng chi tiết chỉ tiêu hiển thị đầy đủ thông tin về thay đổi so với đầu năm/đầu tháng và % so với kế hoạch"
echo ""

echo "=== HOÀN THÀNH KIỂM TRA ==="
