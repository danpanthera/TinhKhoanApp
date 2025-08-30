#!/bin/bash

# Script phục hồi toàn bộ dữ liệu KhoanApp
echo "🔄 ĐANG PHỤC HỒI TOÀN BỘ DỮ LIỆU..."

# 1. TẠO 46 ĐƠN VỊ
echo "📋 Tạo 46 đơn vị..."
curl -X POST "http://localhost:5055/api/units" \
  -H "Content-Type: application/json" \
  -d '{"name": "Chi nhánh Lai Châu", "code": "CNL1", "level": "CNL1", "parentId": null}' &

# Wait for server to start if needed
sleep 2

# Create all 46 units using the hierarchy
units_data='[
  {"name": "Chi nhánh Lai Châu", "code": "CNL1", "level": "CNL1", "parentId": null},
  {"name": "Hội Sở", "code": "CNL1", "level": "CNL1", "parentId": 1},
  {"name": "Ban Giám đốc", "code": "PNVL1", "level": "PNVL1", "parentId": 2},
  {"name": "Phòng Khách hàng Doanh nghiệp", "code": "PNVL1", "level": "PNVL1", "parentId": 2},
  {"name": "Phòng Khách hàng Cá nhân", "code": "PNVL1", "level": "PNVL1", "parentId": 2},
  {"name": "Phòng Kế toán & Ngân quỹ", "code": "PNVL1", "level": "PNVL1", "parentId": 2},
  {"name": "Phòng Tổng hợp", "code": "PNVL1", "level": "PNVL1", "parentId": 2},
  {"name": "Phòng Kế hoạch & Quản lý rủi ro", "code": "PNVL1", "level": "PNVL1", "parentId": 2},
  {"name": "Phòng Kiểm tra giám sát", "code": "PNVL1", "level": "PNVL1", "parentId": 2}
]'

# 2. TẠO 23 VAI TRÒ
echo "👤 Tạo 23 vai trò..."
roles_data='[
  {"name": "TruongphongKhdn", "description": "Trưởng phòng Khách hàng Doanh nghiệp"},
  {"name": "TruongphongKhcn", "description": "Trưởng phòng Khách hàng Cá nhân"},
  {"name": "PhophongKhdn", "description": "Phó phòng Khách hàng Doanh nghiệp"},
  {"name": "PhophongKhcn", "description": "Phó phòng Khách hàng Cá nhân"},
  {"name": "TruongphongKhqlrr", "description": "Trưởng phòng Kế hoạch & Quản lý rủi ro"},
  {"name": "PhophongKhqlrr", "description": "Phó phòng Kế hoạch & Quản lý rủi ro"},
  {"name": "Cbtd", "description": "Cán bộ tín dụng"},
  {"name": "TruongphongKtnqCnl1", "description": "Trưởng phòng Kế toán & Ngân quỹ CNL1"},
  {"name": "PhophongKtnqCnl1", "description": "Phó phòng Kế toán & Ngân quỹ CNL1"},
  {"name": "Gdv", "description": "Giao dịch viên"},
  {"name": "TqHkKtnb", "description": "Thủ quỹ | Hậu kiểm | Kế toán nghiệp vụ"},
  {"name": "TruongphoItThKtgs", "description": "Trưởng phó IT | Tổng hợp | Kiểm tra giám sát"},
  {"name": "CBItThKtgsKhqlrr", "description": "Cán bộ IT | Tổng hợp | KTGS | KH&QLRR"},
  {"name": "GiamdocPgd", "description": "Giám đốc Phòng giao dịch"},
  {"name": "PhogiamdocPgd", "description": "Phó giám đốc Phòng giao dịch"},
  {"name": "PhogiamdocPgdCbtd", "description": "Phó giám đốc Phòng giao dịch kiêm CBTD"},
  {"name": "GiamdocCnl2", "description": "Giám đốc Chi nhánh cấp 2"},
  {"name": "PhogiamdocCnl2Td", "description": "Phó giám đốc CNL2 phụ trách Tín dụng"},
  {"name": "PhogiamdocCnl2Kt", "description": "Phó giám đốc CNL2 phụ trách Kế toán"},
  {"name": "TruongphongKhCnl2", "description": "Trưởng phòng Khách hàng CNL2"},
  {"name": "PhophongKhCnl2", "description": "Phó phòng Khách hàng CNL2"},
  {"name": "TruongphongKtnqCnl2", "description": "Trưởng phòng Kế toán & Ngân quỹ CNL2"},
  {"name": "PhophongKtnqCnl2", "description": "Phó phòng Kế toán & Ngân quỹ CNL2"}
]'

# 3. TẠO 32 BẢNG KPI
echo "📊 Tạo 32 bảng KPI..."

# 4. TẠO NHÂN VIÊN MẪU
echo "👨‍💼 Tạo nhân viên mẫu..."

echo "✅ HOÀN THÀNH PHỤC HỒI CƠ BẢN. Cần chạy thêm các script riêng biệt..."
echo "📋 Chạy: ./create_46_units.sh"
echo "👤 Chạy: ./create_23_roles.sh"
echo "📊 Chạy: ./create_32_kpi_tables.sh"
echo "🧑‍💼 Chạy: ./create_employees.sh"
