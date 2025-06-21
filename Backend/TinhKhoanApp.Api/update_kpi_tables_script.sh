#!/bin/bash

# =============================================================
# Script tự động cập nhật 4 bảng KPI từ KTNV sang KTNQ
# =============================================================

echo "🚀 Bắt đầu cập nhật terminology cho 4 bảng KPI..."

# Di chuyển đến thư mục backend
cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api

echo "📊 Chạy seeding để cập nhật terminology..."
# Chạy seeding để áp dụng TerminologyUpdater
dotnet run seed

echo "✅ Hoàn thành cập nhật 4 bảng KPI:"
echo "   1. Trưởng phòng KTNV CNL1 -> Trưởng phòng KTNQ CNL1 (TruongphongKtnqCnl1)"
echo "   2. Phó phòng KTNV CNL1 -> Phó phòng KTNQ CNL1 (PhophongKtnqCnl1)"
echo "   3. Trưởng phòng KTNV CNL2 -> Trưởng phòng KTNQ CNL2 (TruongphongKtnqCnl2)"
echo "   4. Phó phòng KTNV CNL2 -> Phó phòng KTNQ CNL2 (PhophongKtnqCnl2)"

echo "🎯 Kiểm tra kết quả trong database hoặc frontend."
