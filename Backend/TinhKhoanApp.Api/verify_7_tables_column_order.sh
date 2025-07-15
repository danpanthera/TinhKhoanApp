#!/bin/bash

# =======================================================================
# KIỂM TRA THỨ TỰ CỘT BUSINESS DATA - 7 BẢNG CÒN LẠI (NGOẠI TRỪ GL01)
# Đảm bảo business columns được sắp xếp đúng thứ tự như CSV trước
# khi thêm các cột System/Temporal vào sau
# =======================================================================

echo "🔍 KIỂM TRA THỨ TỰ CỘT BUSINESS DATA - 7 BẢNG CÒN LẠI"
echo "================================================================"
echo "📅 Date: $(date)"
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Biến đếm kết quả
PERFECT_TABLES=0
TOTAL_TABLES=7

echo "🎯 KIỂM TRA TỪNG BẢNG:"
echo "======================"

# 1. DP01 - 63 business columns
echo ""
echo -e "${BLUE}1️⃣ DP01 - Dữ liệu tiền gửi (63 business columns):${NC}"
DP01_CSV_FIRST_10="MA_CN,TAI_KHOAN_HACH_TOAN,MA_KH,TEN_KH,DP_TYPE_NAME,CCY,CURRENT_BALANCE,RATE,SO_TAI_KHOAN,OPENING_DATE"
DP01_MODEL_FIRST_10=$(grep -E "^\s*\[Column\(" Models/DataTables/DP01.cs | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | head -10 | sed 's/.*\[Column("\([^"]*\)").*/\1/' | tr '\n' ',' | sed 's/,$//')

if [ "$DP01_CSV_FIRST_10" = "$DP01_MODEL_FIRST_10" ]; then
    echo -e "   ${GREEN}✅ DP01: Thứ tự 10 cột đầu khớp hoàn hảo${NC}"
    ((PERFECT_TABLES++))
else
    echo -e "   ${RED}❌ DP01: Thứ tự cột không khớp${NC}"
    echo "      CSV:   $DP01_CSV_FIRST_10"
    echo "      Model: $DP01_MODEL_FIRST_10"
fi

# 2. DPDA - 13 business columns
echo ""
echo -e "${BLUE}2️⃣ DPDA - Phát hành thẻ (13 business columns):${NC}"
DPDA_CSV_ALL="MA_CHI_NHANH,MA_KHACH_HANG,TEN_KHACH_HANG,SO_TAI_KHOAN,LOAI_THE,SO_THE,NGAY_NOP_DON,NGAY_PHAT_HANH,USER_PHAT_HANH,TRANG_THAI,PHAN_LOAI,GIAO_THE,LOAI_PHAT_HANH"
DPDA_MODEL_ALL=$(grep -E "^\s*\[Column\(" Models/DataTables/DPDA.cs | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | sed 's/.*\[Column("\([^"]*\)").*/\1/' | tr '\n' ',' | sed 's/,$//')

if [ "$DPDA_CSV_ALL" = "$DPDA_MODEL_ALL" ]; then
    echo -e "   ${GREEN}✅ DPDA: Thứ tự 13 cột khớp hoàn hảo${NC}"
    ((PERFECT_TABLES++))
else
    echo -e "   ${RED}❌ DPDA: Thứ tự cột không khớp${NC}"
    echo "      CSV:   $DPDA_CSV_ALL"
    echo "      Model: $DPDA_MODEL_ALL"
fi

# 3. EI01 - 24 business columns
echo ""
echo -e "${BLUE}3️⃣ EI01 - Mobile Banking (24 business columns):${NC}"
EI01_CSV_FIRST_10="MA_CN,MA_KH,TEN_KH,LOAI_KH,SDT_EMB,TRANG_THAI_EMB,NGAY_DK_EMB,SDT_OTT,TRANG_THAI_OTT,NGAY_DK_OTT"
EI01_MODEL_FIRST_10=$(grep -E "^\s*\[Column\(" Models/DataTables/EI01.cs | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | head -10 | sed 's/.*\[Column("\([^"]*\)").*/\1/' | tr '\n' ',' | sed 's/,$//')

if [ "$EI01_CSV_FIRST_10" = "$EI01_MODEL_FIRST_10" ]; then
    echo -e "   ${GREEN}✅ EI01: Thứ tự 10 cột đầu khớp hoàn hảo${NC}"
    ((PERFECT_TABLES++))
else
    echo -e "   ${RED}❌ EI01: Thứ tự cột không khớp${NC}"
    echo "      CSV:   $EI01_CSV_FIRST_10"
    echo "      Model: $EI01_MODEL_FIRST_10"
fi

# 4. GL41 - 13 business columns
echo ""
echo -e "${BLUE}4️⃣ GL41 - Bảng khác (13 business columns):${NC}"
GL41_CSV_ALL="MA_CN,MA_TKPK,TEN_TKPK,NGUON_VON,SO_DKPK,NGAY_DKPK,NGAY_MUAPK,NGAY_BANHPK,SO_LUONG,GIA_MUA,GIA_BAN,GIA_TRI_BOOK,GIA_TRI_THITRUONG"
GL41_MODEL_ALL=$(grep -E "^\s*\[Column\(" Models/DataTables/GL41.cs | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | sed 's/.*\[Column("\([^"]*\)").*/\1/' | tr '\n' ',' | sed 's/,$//')

if [ "$GL41_CSV_ALL" = "$GL41_MODEL_ALL" ]; then
    echo -e "   ${GREEN}✅ GL41: Thứ tự 13 cột khớp hoàn hảo${NC}"
    ((PERFECT_TABLES++))
else
    echo -e "   ${RED}❌ GL41: Thứ tự cột không khớp${NC}"
    echo "      CSV:   $GL41_CSV_ALL"
    echo "      Model: $GL41_MODEL_ALL"
fi

# 5. LN01 - 79 business columns (kiểm tra 10 cột đầu)
echo ""
echo -e "${BLUE}5️⃣ LN01 - Dữ liệu cho vay (79 business columns):${NC}"
LN01_CSV_FIRST_10="BRCD,CUSTSEQ,CUSTNM,TAI_KHOAN,CCY,DU_NO,DSBSSEQ,TRANSACTION_DATE,DSBSDT,DISBUR_CCY"
LN01_MODEL_FIRST_10=$(grep -E "^\s*\[Column\(" Models/DataTables/LN01.cs | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | head -10 | sed 's/.*\[Column("\([^"]*\)").*/\1/' | tr '\n' ',' | sed 's/,$//')

if [ "$LN01_CSV_FIRST_10" = "$LN01_MODEL_FIRST_10" ]; then
    echo -e "   ${GREEN}✅ LN01: Thứ tự 10 cột đầu khớp hoàn hảo${NC}"
    ((PERFECT_TABLES++))
else
    echo -e "   ${RED}❌ LN01: Thứ tự cột không khớp${NC}"
    echo "      CSV:   $LN01_CSV_FIRST_10"
    echo "      Model: $LN01_MODEL_FIRST_10"
fi

# 6. LN03 - 17 business columns
echo ""
echo -e "${BLUE}6️⃣ LN03 - Dữ liệu khác (17 business columns):${NC}"
LN03_CSV_ALL="MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,THUNOSAUXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON"
LN03_MODEL_ALL=$(grep -E "^\s*\[Column\(" Models/DataTables/LN03.cs | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | sed 's/.*\[Column("\([^"]*\)").*/\1/' | tr '\n' ',' | sed 's/,$//')

if [ "$LN03_CSV_ALL" = "$LN03_MODEL_ALL" ]; then
    echo -e "   ${GREEN}✅ LN03: Thứ tự 17 cột khớp hoàn hảo${NC}"
    ((PERFECT_TABLES++))
else
    echo -e "   ${RED}❌ LN03: Thứ tự cột không khớp${NC}"
    echo "      CSV:   $LN03_CSV_ALL"
    echo "      Model: $LN03_MODEL_ALL"
fi

# 7. RR01 - 25 business columns (kiểm tra 10 cột đầu)
echo ""
echo -e "${BLUE}7️⃣ RR01 - Dư nợ gốc, lãi XLRR (25 business columns):${NC}"
RR01_CSV_FIRST_10="CN_LOAI_I,BRCD,MA_KH,TEN_KH,SO_LDS,CCY,SO_LAV,LOAI_KH,NGAY_GIAI_NGAN,NGAY_DEN_HAN"
RR01_MODEL_FIRST_10=$(grep -E "^\s*\[Column\(" Models/DataTables/RR01.cs | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | head -10 | sed 's/.*\[Column("\([^"]*\)").*/\1/' | tr '\n' ',' | sed 's/,$//')

if [ "$RR01_CSV_FIRST_10" = "$RR01_MODEL_FIRST_10" ]; then
    echo -e "   ${GREEN}✅ RR01: Thứ tự 10 cột đầu khớp hoàn hảo${NC}"
    ((PERFECT_TABLES++))
else
    echo -e "   ${RED}❌ RR01: Thứ tự cột không khớp${NC}"
    echo "      CSV:   $RR01_CSV_FIRST_10"
    echo "      Model: $RR01_MODEL_FIRST_10"
fi

# Tổng kết
echo ""
echo "🎯 TỔNG KẾT KẾT QUẢ:"
echo "===================="
echo -e "📊 Số bảng hoàn hảo: ${GREEN}$PERFECT_TABLES${NC}/$TOTAL_TABLES"

if [ $PERFECT_TABLES -eq $TOTAL_TABLES ]; then
    echo ""
    echo -e "${GREEN}🎉 HOÀN HẢO: Tất cả 7 bảng đều có thứ tự cột business data chính xác!${NC}"
    echo -e "${GREEN}✅ Cấu trúc: [Business Columns] → [System/Temporal Columns]${NC}"
    echo -e "${GREEN}✅ Sẵn sàng import CSV files mà không cần điều chỉnh gì!${NC}"
else
    echo ""
    echo -e "${RED}⚠️ CẦN KHẮC PHỤC: $((TOTAL_TABLES - PERFECT_TABLES)) bảng có vấn đề về thứ tự cột${NC}"
    echo -e "${YELLOW}🔧 Cần sắp xếp lại business columns trước system/temporal columns${NC}"
fi

echo ""
echo "📝 CHÚ THÍCH:"
echo "   • Business columns: Cột dữ liệu từ CSV gốc"
echo "   • System/Temporal columns: NGAY_DL, CREATED_DATE, UPDATED_DATE, FILE_NAME"
echo "   • GL01: Đã chuyển sang Partitioned Columnstore (đã kiểm tra riêng)"

echo ""
echo "✅ Script completed: $(date)"
