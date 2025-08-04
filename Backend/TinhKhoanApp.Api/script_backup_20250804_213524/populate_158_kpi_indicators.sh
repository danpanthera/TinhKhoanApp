#!/bin/bash

# Script để populate đúng 158 chỉ tiêu KPI theo phân bố trong README_DAT.md
#
# Phân bố 158 chỉ tiêu theo vai trò:
# 1-4.   KHDN/KHCN: 4 bảng × 8 chỉ tiêu = 32
# 5-6.   KH&QLRR: 2 bảng × 6 chỉ tiêu = 12
# 7.     CBTD: 1 bảng × 8 chỉ tiêu = 8
# 8-9.   KTNQ CNL1: 2 bảng × 6 chỉ tiêu = 12
# 10.    GDV: 1 bảng × 6 chỉ tiêu = 6
# 12.    IT/TH/KTGS: 1 bảng × 5 chỉ tiêu = 5
# 13.    CB IT/TH/KTGS: 1 bảng × 4 chỉ tiêu = 4
# 14-15. GĐ PGD: 2 bảng × 9 chỉ tiêu = 18
# 16.    PGĐ CBTD: 1 bảng × 8 chỉ tiêu = 8
# 17.    GĐ CNL2: 1 bảng × 11 chỉ tiêu = 11
# 18.    PGĐ CNL2 TD: 1 bảng × 8 chỉ tiêu = 8
# 19.    PGĐ CNL2 KT: 1 bảng × 6 chỉ tiêu = 6
# 20.    TP KH CNL2: 1 bảng × 9 chỉ tiêu = 9
# 21.    PP KH CNL2: 1 bảng × 8 chỉ tiêu = 8
# 22.    TP KTNQ CNL2: 1 bảng × 6 chỉ tiêu = 6
# 23.    PP KTNQ CNL2: 1 bảng × 5 chỉ tiêu = 5

echo "🎯 Populate đúng 158 chỉ tiêu KPI theo phân bố trong README_DAT.md"

# Kiểm tra xem bảng KpiIndicators có tồn tại không
echo "🔍 Kiểm tra bảng KpiIndicators..."
INDICATOR_CHECK=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'KpiIndicators'" -h -1 -C 2>/dev/null | head -1 | tr -d ' \n\r')

echo "Debug: INDICATOR_CHECK = '$INDICATOR_CHECK'"

if [[ ! "$INDICATOR_CHECK" =~ ^1 ]]; then
    echo "❌ Bảng KpiIndicators không tồn tại. Cần tạo bảng trước!"
    exit 1
fi

# Clear existing indicators
echo "🧹 Xóa các indicators cũ..."
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -Q "DELETE FROM KpiIndicators" -C > /dev/null 2>&1

# Hàm tạo indicators cho một bảng KPI
create_indicators() {
    local table_name="$1"
    local indicator_count="$2"
    local category="$3"

    echo "📊 Tạo $indicator_count chỉ tiêu cho bảng $table_name ($category)..."

    # Lấy TableId từ KpiAssignmentTables
    TABLE_ID=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -Q "SELECT Id FROM KpiAssignmentTables WHERE TableName = '$table_name'" -h -1 -C 2>/dev/null | tr -d ' ')

    if [ -z "$TABLE_ID" ] || [ "$TABLE_ID" = "" ]; then
        echo "⚠️  Không tìm thấy bảng $table_name trong KpiAssignmentTables"
        return
    fi

    # Tạo indicators cho bảng này
    for i in $(seq 1 $indicator_count); do
        INDICATOR_CODE="${table_name}_KPI_$(printf "%02d" $i)"
        INDICATOR_NAME="Chỉ tiêu $i cho $table_name"

        sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -Q "
        INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Unit, IsActive, CreatedAt, UpdatedAt)
        VALUES ($TABLE_ID, '$INDICATOR_CODE', N'$INDICATOR_NAME', N'%', 1, GETDATE(), GETDATE())
        " -C > /dev/null 2>&1
    done

    echo "✅ Đã tạo $indicator_count chỉ tiêu cho $table_name (TableId: $TABLE_ID)"
}

# 1-4. KHDN/KHCN: 4 bảng × 8 chỉ tiêu = 32
create_indicators "TruongphongKhdn" 8 "KHDN"
create_indicators "TruongphongKhcn" 8 "KHCN"
create_indicators "PhophongKhdn" 8 "KHDN"
create_indicators "PhophongKhcn" 8 "KHCN"

# 5-6. KH&QLRR: 2 bảng × 6 chỉ tiêu = 12
create_indicators "TruongphongKhqlrr" 6 "KH&QLRR"
create_indicators "PhophongKhqlrr" 6 "KH&QLRR"

# 7. CBTD: 1 bảng × 8 chỉ tiêu = 8
create_indicators "Cbtd" 8 "CBTD"

# 8-9. KTNQ CNL1: 2 bảng × 6 chỉ tiêu = 12
create_indicators "TruongphongKtnqCnl1" 6 "KTNQ CNL1"
create_indicators "PhophongKtnqCnl1" 6 "KTNQ CNL1"

# 10. GDV: 1 bảng × 6 chỉ tiêu = 6
create_indicators "Gdv" 6 "GDV"

# 12. IT/TH/KTGS: 1 bảng × 5 chỉ tiêu = 5
create_indicators "TruongphoItThKtgs" 5 "IT/TH/KTGS"

# 13. CB IT/TH/KTGS: 1 bảng × 4 chỉ tiêu = 4
create_indicators "CBItThKtgsKhqlrr" 4 "CB IT/TH/KTGS"

# 14-15. GĐ PGD: 2 bảng × 9 chỉ tiêu = 18
create_indicators "GiamdocPgd" 9 "GĐ PGD"
create_indicators "PhogiamdocPgd" 9 "GĐ PGD"

# 16. PGĐ CBTD: 1 bảng × 8 chỉ tiêu = 8
create_indicators "PhogiamdocPgdCbtd" 8 "PGĐ CBTD"

# 17. GĐ CNL2: 1 bảng × 11 chỉ tiêu = 11
create_indicators "GiamdocCnl2" 11 "GĐ CNL2"

# 18. PGĐ CNL2 TD: 1 bảng × 8 chỉ tiêu = 8
create_indicators "PhogiamdocCnl2Td" 8 "PGĐ CNL2 TD"

# 19. PGĐ CNL2 KT: 1 bảng × 6 chỉ tiêu = 6
create_indicators "PhogiamdocCnl2Kt" 6 "PGĐ CNL2 KT"

# 20. TP KH CNL2: 1 bảng × 9 chỉ tiêu = 9
create_indicators "TruongphongKhCnl2" 9 "TP KH CNL2"

# 21. PP KH CNL2: 1 bảng × 8 chỉ tiêu = 8
create_indicators "PhophongKhCnl2" 8 "PP KH CNL2"

# 22. TP KTNQ CNL2: 1 bảng × 6 chỉ tiêu = 6
create_indicators "TruongphongKtnqCnl2" 6 "TP KTNQ CNL2"

# 23. PP KTNQ CNL2: 1 bảng × 5 chỉ tiêu = 5
create_indicators "PhophongKtnqCnl2" 5 "PP KTNQ CNL2"

# Kiểm tra kết quả
echo ""
echo "📊 KIỂM TRA KẾT QUẢ:"
TOTAL_INDICATORS=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -Q "SELECT COUNT(*) FROM KpiIndicators" -h -1 -C 2>/dev/null | tr -d ' ')

echo "✅ Tổng số chỉ tiêu đã tạo: $TOTAL_INDICATORS"

if [ "$TOTAL_INDICATORS" = "158" ]; then
    echo "🎉 HOÀN THÀNH: Đã tạo đúng 158 chỉ tiêu theo phân bố yêu cầu!"
else
    echo "⚠️  Cảnh báo: Số lượng chỉ tiêu không đúng (cần 158, có $TOTAL_INDICATORS)"
fi

# Hiển thị phân bố theo bảng
echo ""
echo "📋 PHÂN BỐ CHỈ TIÊU THEO BẢNG:"
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -Q "
SELECT
    kat.TableName,
    COUNT(ki.Id) as IndicatorCount,
    kat.Category
FROM KpiAssignmentTables kat
LEFT JOIN KpiIndicators ki ON kat.Id = ki.TableId
WHERE kat.Category = 'EMPLOYEE'
GROUP BY kat.TableName, kat.Category
ORDER BY kat.Id
" -C

echo "🎯 Script hoàn tất!"
