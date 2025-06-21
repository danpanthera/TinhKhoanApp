#!/bin/bash

# 🎨 Script demo các cập nhật UX mới
# Kiểm tra icons KPI, ảnh HDR và typography

echo "🎨 AGRIBANK LAI CHÂU - DEMO CẬP NHẬT UX MỚI"
echo "=============================================="
echo ""

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m'

print_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_highlight() {
    echo -e "${YELLOW}🎯 $1${NC}"
}

cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite

echo "1. KIỂM TRA ICONS KPI MỚI"
echo "========================="

# Kiểm tra các icons mới trong dashboard
dashboard_file="src/views/dashboard/BusinessPlanDashboard.vue"

icons=(
    "mdi-hand-coin:Huy động vốn"
    "mdi-credit-card-multiple-outline:Dư nợ"
    "mdi-shield-alert-outline:Tỷ lệ nợ xấu"
    "mdi-cash-refund:Thu nợ đã XLRR"
    "mdi-account-cash-outline:Thu dịch vụ"
    "mdi-finance:Lợi nhuận khoán"
)

for icon_kpi in "${icons[@]}"; do
    IFS=':' read -r icon kpi <<< "$icon_kpi"
    if grep -q "$icon" "$dashboard_file"; then
        print_success "$kpi: $icon"
    else
        echo "❌ $kpi: $icon"
    fi
done

echo ""
echo "2. KIỂM TRA ẢNH HDR MỚI"
echo "======================="

bg_dir="public/images/backgrounds"
hdr_images=(
    "nature-mountain-sunset-hdr.jpg:Núi non hoàng hôn HDR"
    "nature-lake-forest-hdr.jpg:Hồ rừng xanh mướt HDR"
    "nature-green-forest-path-hdr.jpg:Đường mòn rừng xanh HDR"
)

for img_desc in "${hdr_images[@]}"; do
    IFS=':' read -r img desc <<< "$img_desc"
    if [ -f "$bg_dir/$img" ]; then
        size=$(stat -f%z "$bg_dir/$img" 2>/dev/null || echo "0")
        if [ $size -gt 50000 ]; then
            print_success "$desc (${size} bytes)"
        else
            echo "⚠️  $desc (file quá nhỏ: ${size} bytes)"
        fi
    else
        echo "❌ $desc (không tồn tại)"
    fi
done

echo ""
echo "3. KIỂM TRA TYPOGRAPHY TRANG CHỦ"
echo "==============================="

home_file="src/views/HomeView.vue"

# Kiểm tra chữ viết hoa
if grep -q "AGRIBANK LAI CHAU CENTER" "$home_file"; then
    print_success "Chữ viết hoa: AGRIBANK LAI CHAU CENTER"
else
    echo "❌ Chữ chưa viết hoa"
fi

if grep -q "HỆ THỐNG QUẢN LÝ" "$home_file"; then
    print_success "Subtitle viết hoa: HỆ THỐNG QUẢN LÝ..."
else
    echo "❌ Subtitle chưa viết hoa"
fi

# Kiểm tra font size
if grep -q "font-size: 4.5rem" "$home_file"; then
    print_success "Title font size tăng: 4.5rem (+2 cỡ từ 3.5rem)"
else
    echo "❌ Title font size chưa tăng"
fi

if grep -q "font-size: 1.9rem" "$home_file"; then
    print_success "Subtitle font size tăng: 1.9rem (+2 cỡ từ 1.5rem)"
else
    echo "❌ Subtitle font size chưa tăng"
fi

echo ""
echo "4. KIỂM TRA SERVER VÀ MỞ DEMO"
echo "============================"

# Kiểm tra server
if curl -s http://localhost:3000 > /dev/null; then
    print_success "Frontend server đang chạy"
    
    print_highlight "Mở trang chủ để xem typography mới..."
    if command -v open >/dev/null 2>&1; then
        open "http://localhost:3000" 2>/dev/null
    fi
    
    sleep 3
    
    print_highlight "Mở dashboard để xem icons KPI mới..."
    if command -v open >/dev/null 2>&1; then
        open "http://localhost:3000/#/dashboard/business-plan" 2>/dev/null
    fi
    
else
    echo "❌ Frontend server không chạy - khởi động server trước"
    print_info "Chạy: npm run dev"
fi

echo ""
echo "5. TỔNG QUAN CÁC CẬP NHẬT"
echo "========================"

print_info "🎨 CÁC CẢI THIỆN UX ĐÃ HOÀN THÀNH:"
echo ""
echo "   📍 KPI ICONS - Biểu tượng sinh động và phù hợp:"
echo "      💰 Huy động vốn: Bàn tay đựng tiền xu"
echo "      💳 Dư nợ: Thẻ tín dụng multiple"
echo "      🛡️  Tỷ lệ nợ xấu: Khiên cảnh báo"
echo "      💸 Thu nợ đã XLRR: Biểu tượng hoàn tiền"
echo "      💼 Thu dịch vụ: Tài khoản tiền mặt"
echo "      📊 Lợi nhuận khoán: Biểu tượng tài chính"
echo ""
echo "   🌅 ẢNH NỀN HDR - Thiên nhiên hùng vĩ:"
echo "      🏔️  Núi non hoàng hôn với ánh sáng vàng"
echo "      🏞️  Hồ nước xanh mướt trong rừng"
echo "      🌲 Con đường mòn qua rừng xanh tươi"
echo ""
echo "   ✍️  TYPOGRAPHY - Font chữ lớn và rõ ràng:"
echo "      📝 AGRIBANK LAI CHAU CENTER (viết hoa)"
echo "      📝 HỆ THỐNG QUẢN LÝ... (viết hoa)"
echo "      📏 Font size tăng +2 cỡ cho cả desktop và mobile"

echo ""
echo "=============================================="
print_highlight "🎉 DEMO HOÀN TẤT - TRẢI NGHIỆM UX TUYỆT VỜI!"
echo ""
print_info "URLs để test:"
echo "   🏠 Trang chủ: http://localhost:3000"
echo "   📊 Dashboard: http://localhost:3000/#/dashboard/business-plan"
echo ""
print_info "Các cải thiện làm cho giao diện:"
echo "   ✨ Sinh động hơn với icons phù hợp"
echo "   🎨 Đẹp mắt hơn với ảnh HDR chất lượng cao"
echo "   👁️  Dễ đọc hơn với font chữ lớn và rõ ràng"
echo "   💼 Chuyên nghiệp hơn với typography chuẩn"
echo "=============================================="
