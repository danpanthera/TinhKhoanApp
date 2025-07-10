#!/bin/bash

# 🎯 Test All 12 Raw Data Tables with Smart Import
# Script kiểm tra tất cả 12 bảng dữ liệu thô

echo "🎯 ===== KIỂM TRA TẤT CẢ 12 BẢNG DỮ LIỆU THÔ ====="

# Create sample files for each data type
echo "📝 Tạo file test cho từng loại dữ liệu..."

# 1. DP01 - Account balances
echo "NGAY_DL,MA_CN,TK_KH,SO_DU_CUOI_KY,LOAI_TK" > test_dp01_full.csv
echo "31/05/2025,7808,123456,1000000,Tiet_Kiem" >> test_dp01_full.csv
echo "31/05/2025,7808,123457,2000000,Tiet_Kiem" >> test_dp01_full.csv

# 2. LN01 - Loan data
echo "NGAY_DL,MA_CN,MA_KH,SO_HD_VAY,SO_TIEN_VAY" > test_ln01_full.csv
echo "31/05/2025,7808,KH001,HD001,10000000" >> test_ln01_full.csv
echo "31/05/2025,7808,KH002,HD002,20000000" >> test_ln01_full.csv

# 3. LN02 - Loan details (already exists)
# Using existing 7808_ln02_20250531.csv

# 4. LN03 - Bad debt
echo "NGAY_DL,MA_CN,MA_KH,SO_HD_VAY,SO_TIEN_NO_XAU" > test_ln03_full.csv
echo "31/05/2025,7808,KH001,HD001,500000" >> test_ln03_full.csv
echo "31/05/2025,7808,KH002,HD002,1000000" >> test_ln03_full.csv

# 5. DB01 - Deposit data
echo "NGAY_DL,MA_CN,MA_KH,SO_TK,SO_DU_TIEN_GUI" > test_db01_full.csv
echo "31/05/2025,7808,KH001,TK001,5000000" >> test_db01_full.csv
echo "31/05/2025,7808,KH002,TK002,10000000" >> test_db01_full.csv

# 6. GL01 - General ledger
echo "NGAY_DL,MA_CN,TK_KT,SO_DU_DAU_KY,SO_DU_CUOI_KY" > test_gl01_full.csv
echo "31/05/2025,7808,1111,1000000,1200000" >> test_gl01_full.csv
echo "31/05/2025,7808,1112,2000000,2500000" >> test_gl01_full.csv

# 7. GL41 - Trial balance
echo "NGAY_DL,MA_CN,TK_KT,SO_DU_NO,SO_DU_CO" > test_gl41_full.csv
echo "31/05/2025,7808,1111,1000000,0" >> test_gl41_full.csv
echo "31/05/2025,7808,2111,0,500000" >> test_gl41_full.csv

# 8. DPDA - Detailed account data  
echo "NGAY_DL,MA_CN,TK_KH,CHI_TIET_TK,SO_DU" > test_dpda_full.csv
echo "31/05/2025,7808,123456,Chi_Tiet_1,1000000" >> test_dpda_full.csv
echo "31/05/2025,7808,123457,Chi_Tiet_2,2000000" >> test_dpda_full.csv

# 9. EI01 - External information
echo "NGAY_DL,MA_CN,LOAI_THONG_TIN,GIA_TRI,MO_TA" > test_ei01_full.csv
echo "31/05/2025,7808,NGOAI_TE,23500,USD_Rate" >> test_ei01_full.csv
echo "31/05/2025,7808,LAI_SUAT,5.5,Base_Rate" >> test_ei01_full.csv

# 10. KH03 - Customer data
echo "NGAY_DL,MA_CN,MA_KH,TEN_KH,LOAI_KH" > test_kh03_full.csv
echo "31/05/2025,7808,KH001,Nguyen_Van_A,Ca_Nhan" >> test_kh03_full.csv
echo "31/05/2025,7808,KH002,Tran_Thi_B,Ca_Nhan" >> test_kh03_full.csv

# 11. RR01 - Risk report
echo "NGAY_DL,MA_CN,LOAI_RUI_RO,MUC_DO_RUI_RO,GIA_TRI" > test_rr01_full.csv
echo "31/05/2025,7808,TIN_DUNG,CAO,1000000" >> test_rr01_full.csv
echo "31/05/2025,7808,THANH_KHOAN,TRUNG_BINH,500000" >> test_rr01_full.csv

# 12. DT_KHKD1 - Business plan data
echo "NGAY_DL,MA_CN,CHI_TIEU,KE_HOACH,THUC_HIEN" > test_dt_khkd1_full.csv
echo "31/05/2025,7808,DOANH_THU,1000000000,900000000" >> test_dt_khkd1_full.csv
echo "31/05/2025,7808,LOI_NHUAN,100000000,95000000" >> test_dt_khkd1_full.csv

echo "✅ Đã tạo file test cho tất cả 12 loại dữ liệu"
echo ""

# Test each data type
echo "🧪 Bắt đầu test import cho từng loại dữ liệu..."

# Results tracking
DP01_RESULT=0
LN01_RESULT=0
LN02_RESULT=0
LN03_RESULT=0
DB01_RESULT=0
GL01_RESULT=0
GL41_RESULT=0
DPDA_RESULT=0
EI01_RESULT=0
KH03_RESULT=0
RR01_RESULT=0
DT_KHKD1_RESULT=0

# Test function
test_import() {
    local file=$1
    local type=$2
    local var_name=$3
    echo "   Testing $type..."
    local result=$(curl -s -X POST "http://localhost:5055/api/DirectImport/smart" \
        -F "file=@$file" | jq -r '.ProcessedRecords // 0')
    eval "$var_name=$result"
    echo "   -> $type: $result records"
}

# Execute tests
test_import "test_dp01_full.csv" "DP01" "DP01_RESULT"
test_import "test_ln01_full.csv" "LN01" "LN01_RESULT"
test_import "7808_ln02_20250531.csv" "LN02" "LN02_RESULT"
test_import "test_ln03_full.csv" "LN03" "LN03_RESULT"
test_import "test_db01_full.csv" "DB01" "DB01_RESULT"
test_import "test_gl01_full.csv" "GL01" "GL01_RESULT"
test_import "test_gl41_full.csv" "GL41" "GL41_RESULT"
test_import "test_dpda_full.csv" "DPDA" "DPDA_RESULT"
test_import "test_ei01_full.csv" "EI01" "EI01_RESULT"
test_import "test_kh03_full.csv" "KH03" "KH03_RESULT"
test_import "test_rr01_full.csv" "RR01" "RR01_RESULT"
test_import "test_dt_khkd1_full.csv" "DT_KHKD1" "DT_KHKD1_RESULT"

echo ""
echo "📊 ===== KẾT QUẢ KIỂM TRA 12 BẢNG DỮ LIỆU ====="

# Summary
total_success=0
total_tests=12

# Check results
check_result() {
    local type=$1
    local result=$2
    if [ "$result" -gt 0 ]; then
        echo "✅ $type: $result records"
        ((total_success++))
    else
        echo "❌ $type: $result records"
    fi
}

check_result "DP01" $DP01_RESULT
check_result "LN01" $LN01_RESULT
check_result "LN02" $LN02_RESULT
check_result "LN03" $LN03_RESULT
check_result "DB01" $DB01_RESULT
check_result "GL01" $GL01_RESULT
check_result "GL41" $GL41_RESULT
check_result "DPDA" $DPDA_RESULT
check_result "EI01" $EI01_RESULT
check_result "KH03" $KH03_RESULT
check_result "RR01" $RR01_RESULT
check_result "DT_KHKD1" $DT_KHKD1_RESULT

echo ""
echo "📈 TỔNG KẾT:"
echo "   ✅ Thành công: $total_success/$total_tests bảng"
echo "   📊 Tỷ lệ thành công: $((total_success * 100 / total_tests))%"

if [ $total_success -eq $total_tests ]; then
    echo ""
    echo "🎉 HOÀN HẢO! TẤT CẢ 12 BẢNG DỮ LIỆU HOẠT ĐỘNG!"
    echo "   🚀 Smart Import hoạt động cho tất cả data types"
    echo "   📊 Tất cả bảng đều import thành công"
    echo "   ✅ Hệ thống sẵn sàng production"
else
    echo ""
    echo "⚠️  VẪN CÓ VẤN ĐỀ VỚI $((total_tests - total_success)) BẢNG"
    echo "   Cần kiểm tra lại backend mapping cho các bảng lỗi"
fi

# Cleanup
rm -f test_*_full.csv

echo ""
echo "🎯 KIỂM TRA 12 BẢNG DỮ LIỆU HOÀN TẤT!"
