#!/bin/bash

echo "🔍 VERIFICATION SCRIPT - LN01 TABLE STRUCTURE"
echo "=============================================="
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${BLUE}📊 KIỂM TRA BẢNG LN01 - CHO VAY${NC}"
echo ""

# CSV file path
CSV_FILE="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7808_ln01_20241231.csv"
MODEL_FILE="Models/DataTables/LN01.cs"

echo -e "${YELLOW}1. Phân tích file CSV:${NC}"
if [ -f "$CSV_FILE" ]; then
    CSV_COLUMNS=$(head -1 "$CSV_FILE" | tr ',' '\n' | wc -l | tr -d ' ')
    echo "   📁 File: $(basename $CSV_FILE)"
    echo "   📊 Số cột CSV: $CSV_COLUMNS"
    echo ""

    echo -e "${YELLOW}   Header columns chi tiết:${NC}"
    head -1 "$CSV_FILE" | tr ',' '\n' | nl | head -20
    echo "   ... (và $(($CSV_COLUMNS - 20)) cột còn lại)"
else
    echo -e "${RED}   ❌ Không tìm thấy file CSV: $CSV_FILE${NC}"
fi

echo ""
echo -e "${YELLOW}2. Phân tích Model LN01:${NC}"
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
    echo -e "${GREEN}   ✅ LN01 table sẵn sàng import CSV!${NC}"
else
    echo -e "${RED}   ❌ MISMATCH: CSV có $CSV_COLUMNS cột, Model có $BUSINESS_COLUMNS business columns${NC}"
fi

echo ""
echo -e "${BLUE}📋 THÔNG TIN NGHIỆP VỤ LN01:${NC}"
echo "   🏦 Bảng: LN01 - Cho vay (Loan Records)"
echo "   📅 Dữ liệu: Chi nhánh Nậm Hàng (7808) - 31/12/2024"
echo "   💰 Nội dung: Hồ sơ vay, giải ngân, lãi suất, thông tin khách hàng"
echo "   🔄 Temporal: Change tracking enabled"
echo "   ⚡ Columnstore: Analytics optimization"

echo ""
echo -e "${BLUE}📁 Sample data từ CSV:${NC}"
if [ -f "$CSV_FILE" ]; then
    echo "   Header: $(head -1 "$CSV_FILE" | cut -d',' -f1-5)..."
    echo "   Sample: $(tail -n +2 "$CSV_FILE" | head -1 | cut -d',' -f1-5)..."
fi

echo ""
echo "=============================================="
echo -e "${GREEN}🎯 VERIFICATION HOÀN THÀNH${NC}"
