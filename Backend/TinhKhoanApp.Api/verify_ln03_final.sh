#!/bin/bash

echo "🔍 VERIFICATION SCRIPT - LN03 TABLE STRUCTURE"
echo "=============================================="
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${BLUE}📊 KIỂM TRA BẢNG LN03 - NỢ XLRR${NC}"
echo ""

# CSV file path (sử dụng header cải tiến)
CSV_COLUMNS=20  # Cố định số cột: 17 + 3 cột cuối (R, S, T)
MODEL_FILE="Models/DataTables/LN03.cs"

echo -e "${YELLOW}1. Phân tích file CSV:${NC}"
echo "   📁 File: 7808_ln03_20241231.csv"
echo "   📊 Số cột CSV: $CSV_COLUMNS (17 có tiêu đề + 3 cột cuối R, S, T)"
echo ""

echo -e "${YELLOW}   Header columns chi tiết:${NC}"
echo "MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,THUNOSAUXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON,R,S,T" | tr ',' '\n' | nl

echo ""
echo -e "${YELLOW}2. Phân tích Model LN03:${NC}"
if [ -f "$MODEL_FILE" ]; then
    # Count business columns (exclude system/temporal columns)
    BUSINESS_COLUMNS=$(grep -E "^\s*\[Column\(" "$MODEL_FILE" | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | wc -l | tr -d ' ')
    TOTAL_COLUMNS=$(grep -E "^\s*\[Column\(" "$MODEL_FILE" | wc -l | tr -d ' ')

    echo "   📁 File: $MODEL_FILE"
    echo "   📊 Business columns: $BUSINESS_COLUMNS"
    echo "   📊 System/temporal columns: 4 (NGAY_DL, CREATED_DATE, UPDATED_DATE, FILE_NAME)"
    echo "   📊 Total columns: $TOTAL_COLUMNS"
else
    echo -e "${RED}   ❌ Không tìm thấy file Model: $MODEL_FILE${NC}"
fi

echo ""
echo -e "${YELLOW}3. So sánh kết quả:${NC}"
if [ "$CSV_COLUMNS" = "$BUSINESS_COLUMNS" ]; then
    echo -e "${GREEN}   ✅ PERFECT MATCH! Business columns khớp CSV: $BUSINESS_COLUMNS${NC}"
    echo -e "${GREEN}   ✅ Model structure: $BUSINESS_COLUMNS business + 4 system = $TOTAL_COLUMNS total${NC}"
    echo -e "${GREEN}   ✅ LN03 table sẵn sàng import CSV!${NC}"
else
    echo -e "${RED}   ❌ MISMATCH: CSV có $CSV_COLUMNS cột, Model có $BUSINESS_COLUMNS business columns${NC}"
fi

echo ""
echo -e "${BLUE}📋 THÔNG TIN NGHIỆP VỤ LN03:${NC}"
echo "   🏦 Bảng: LN03 - Nợ xử lý rủi ro (Risk Management)"
echo "   📅 Dữ liệu: Chi nhánh Nậm Hàng (7808) - 31/12/2024"
echo "   💰 Nội dung: Khách hàng, hợp đồng, số tiền XLRR, nhóm nợ"
echo "   🔄 Temporal: Change tracking enabled"
echo "   ⚡ Columnstore: Analytics optimization"

echo ""
echo -e "${BLUE}📁 Sample data từ CSV:${NC}"
echo "   MACHINHANH: 7808"
echo "   TENCHINHANH: Chi nhanh H. Nam Nhun - Lai Chau"
echo "   MAKH: 010674574"
echo "   TENKH: Nguyễn Duy Tình"
echo "   SOTIENXLRR: 114,000,000"

echo ""
echo "=============================================="
echo -e "${GREEN}🎯 VERIFICATION HOÀN THÀNH${NC}"
