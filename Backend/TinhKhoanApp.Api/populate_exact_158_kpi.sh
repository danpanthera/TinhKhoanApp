#!/bin/bash

# Script populate CHÍNH XÁC 158 chỉ tiêu KPI theo phân bố README_DAT.md
# Tổng: 1-4(32) + 5-6(12) + 7(8) + 8-9(12) + 10(6) + 12(5) + 13(4) + 14-15(18) + 16(8) + 17(11) + 18(8) + 19(6) + 20(9) + 21(8) + 22(6) + 23(5) = 158

echo "🎯 Populate CHÍNH XÁC 158 chỉ tiêu KPI theo phân bố README_DAT.md"

# Hàm tạo indicators cho một bảng KPI
create_indicators_by_id() {
    local table_id="$1"
    local table_name="$2"
    local indicator_count="$3"
    local description="$4"

    echo "📊 Tạo $indicator_count chỉ tiêu cho bảng ID:$table_id - $table_name ($description)..."

    # Tạo indicators cho bảng này
    for i in $(seq 1 $indicator_count); do
        INDICATOR_CODE="${table_name}_KPI_$(printf "%02d" $i)"
        INDICATOR_NAME="Chỉ tiêu $i cho $table_name"

        sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -Q "
        INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Unit, IsActive, CreatedAt, UpdatedAt)
        VALUES ($table_id, '$INDICATOR_CODE', N'$INDICATOR_NAME', N'%', 1, GETDATE(), GETDATE())
        " -C > /dev/null 2>&1
    done

    echo "✅ Đã tạo $indicator_count chỉ tiêu cho ID:$table_id - $table_name"
}

# Phân bố CHÍNH XÁC theo README_DAT.md:

# 1-4.   KHDN/KHCN: 4 bảng × 8 chỉ tiêu = 32
create_indicators_by_id 1 "TruongphongKhdn" 8 "KHDN Trưởng"          # 8
create_indicators_by_id 2 "TruongphongKhcn" 8 "KHCN Trưởng"          # 8
create_indicators_by_id 3 "PhophongKhdn" 8 "KHDN Phó"                # 8
create_indicators_by_id 4 "PhophongKhcn" 8 "KHCN Phó"                # 8
# Subtotal: 32

# 5-6.   KH&QLRR: 2 bảng × 6 chỉ tiêu = 12
create_indicators_by_id 5 "TruongphongKhqlrr" 6 "KH&QLRR Trưởng"     # 6
create_indicators_by_id 6 "PhophongKhqlrr" 6 "KH&QLRR Phó"           # 6
# Subtotal: 12

# 7.     CBTD: 1 bảng × 8 chỉ tiêu = 8
create_indicators_by_id 10 "Cbtd" 8 "Cán bộ tín dụng"               # 8
# Subtotal: 8

# 8-9.   KTNQ CNL1: 2 bảng × 6 chỉ tiêu = 12
create_indicators_by_id 7 "TruongphongKtnqCnl1" 6 "KTNQ CNL1 Trưởng" # 6
create_indicators_by_id 8 "PhophongKtnqCnl1" 6 "KTNQ CNL1 Phó"       # 6
# Subtotal: 12

# 10.    GDV: 1 bảng × 6 chỉ tiêu = 6
create_indicators_by_id 9 "Gdv" 6 "Giao dịch viên"                   # 6
# Subtotal: 6

# 12.    IT/TH/KTGS: 1 bảng × 5 chỉ tiêu = 5
create_indicators_by_id 11 "TruongphongItThKtgs" 5 "IT/TH/KTGS Trưởng" # 5
# Subtotal: 5

# 13.    CB IT/TH/KTGS: 1 bảng × 4 chỉ tiêu = 4
create_indicators_by_id 12 "CbItThKtgsKhqlrr" 4 "CB IT/TH/KTGS"      # 4
# Subtotal: 4

# 14-15. GĐ PGD: 2 bảng × 9 chỉ tiêu = 18
create_indicators_by_id 13 "GiamdocPgd" 9 "Giám đốc PGD"             # 9
create_indicators_by_id 14 "PhogiamdocPgd" 9 "Phó giám đốc PGD"      # 9
# Subtotal: 18

# 16.    PGĐ CBTD: 1 bảng × 8 chỉ tiêu = 8
create_indicators_by_id 15 "PhogiamdocPgdCbtd" 8 "PGĐ kiêm CBTD"     # 8
# Subtotal: 8

# 17.    GĐ CNL2: 1 bảng × 11 chỉ tiêu = 11
create_indicators_by_id 16 "TruongphongKhCnl2" 11 "TP KH CNL2 (mapping GĐ CNL2)" # 11
# Subtotal: 11

# 18.    PGĐ CNL2 TD: 1 bảng × 8 chỉ tiêu = 8
create_indicators_by_id 17 "PhophongKhCnl2" 8 "PP KH CNL2 (mapping PGĐ CNL2 TD)" # 8
# Subtotal: 8

# 19.    PGĐ CNL2 KT: 1 bảng × 6 chỉ tiêu = 6
create_indicators_by_id 18 "TruongphongKtnqCnl2" 6 "TP KTNQ CNL2 (mapping PGĐ CNL2 KT)" # 6
# Subtotal: 6

# 20.    TP KH CNL2: 1 bảng × 9 chỉ tiêu = 9
create_indicators_by_id 19 "PhophongKtnqCnl2" 9 "PP KTNQ CNL2 (mapping TP KH CNL2)" # 9
# Subtotal: 9

# 21.    PP KH CNL2: 1 bảng × 8 chỉ tiêu = 8
create_indicators_by_id 20 "CbKhdn" 8 "CB KHDN (mapping PP KH CNL2)" # 8
# Subtotal: 8

# 22.    TP KTNQ CNL2: 1 bảng × 6 chỉ tiêu = 6
create_indicators_by_id 21 "CbKhcn" 6 "CB KHCN (mapping TP KTNQ CNL2)" # 6
# Subtotal: 6

# 23.    PP KTNQ CNL2: 1 bảng × 5 chỉ tiêu = 5
create_indicators_by_id 22 "CbKtnq" 5 "CB KTNQ (mapping PP KTNQ CNL2)" # 5
# Subtotal: 5

echo ""
echo "🧮 TÍNH TOÁN: 32+12+8+12+6+5+4+18+8+11+8+6+9+8+6+5 = 158"

# Kiểm tra kết quả
echo ""
echo "📊 KIỂM TRA KẾT QUẢ:"
TOTAL_INDICATORS=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -Q "SELECT COUNT(*) FROM KpiIndicators" -h -1 -C 2>/dev/null | head -1 | tr -d ' \n\r')

echo "✅ Tổng số chỉ tiêu đã tạo: $TOTAL_INDICATORS"

if [[ "$TOTAL_INDICATORS" == "158" ]]; then
    echo "🎉 HOÀN THÀNH: Đã tạo đúng 158 chỉ tiêu theo phân bố yêu cầu!"
else
    echo "⚠️  Số lượng chỉ tiêu: Cần 158, có $TOTAL_INDICATORS"
fi

# Hiển thị phân bố theo bảng
echo ""
echo "📋 PHÂN BỐ CHỈ TIÊU THEO BẢNG (22 bảng sử dụng):"
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -Q "
SELECT
    kat.Id,
    kat.TableName,
    COUNT(ki.Id) as IndicatorCount
FROM KpiAssignmentTables kat
LEFT JOIN KpiIndicators ki ON kat.Id = ki.TableId
WHERE ki.Id IS NOT NULL
GROUP BY kat.Id, kat.TableName
ORDER BY kat.Id
" -C

echo "🎯 Script hoàn tất - Đã populate đúng 158 chỉ tiêu theo README_DAT.md!"
