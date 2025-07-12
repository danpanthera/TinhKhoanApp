#!/bin/bash

echo "🔍 SO SÁNH CHI TIẾT CỘT LN03 - CSV vs MODEL"
echo "==========================================="
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

MODEL_FILE="Models/DataTables/LN03.cs"

echo -e "${BLUE}📊 BẢNG LN03 - SO SÁNH CỘT CSV vs MODEL${NC}"
echo ""

echo -e "${YELLOW}1. Danh sách cột từ CSV (20 cột):${NC}"
# Create expected CSV columns
CSV_COLUMNS_LIST=$(echo "MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,THUNOSAUXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON,R,S,T" | tr ',' '\n' | nl)
echo "$CSV_COLUMNS_LIST"

echo ""
echo -e "${YELLOW}2. Danh sách business columns từ Model (20 cột):${NC}"
MODEL_COLUMNS_LIST=$(grep -E "^\s*\[Column\(" "$MODEL_FILE" | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | sed 's/.*\[Column("\([^"]*\)").*/\1/' | nl)
echo "$MODEL_COLUMNS_LIST"

echo ""
echo -e "${YELLOW}3. So sánh từng cột:${NC}"

# Extract CSV columns
CSV_COLS=($(echo "MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,THUNOSAUXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON,R,S,T" | tr ',' ' '))
MODEL_COLS=($(grep -E "^\s*\[Column\(" "$MODEL_FILE" | grep -v -E "(NGAY_DL|CREATED_DATE|UPDATED_DATE|FILE_NAME)" | sed 's/.*\[Column("\([^"]*\)").*/\1/'))

MATCH_COUNT=0
TOTAL_COUNT=${#CSV_COLS[@]}

for i in $(seq 0 $((${#CSV_COLS[@]} - 1))); do
    CSV_COL="${CSV_COLS[$i]}"
    MODEL_COL="${MODEL_COLS[$i]}"

    if [ "$CSV_COL" = "$MODEL_COL" ]; then
        echo -e "   ${GREEN}✅ $((i+1)). $CSV_COL = $MODEL_COL${NC}"
        ((MATCH_COUNT++))
    else
        echo -e "   ${RED}❌ $((i+1)). $CSV_COL ≠ $MODEL_COL${NC}"
    fi
done

echo ""
echo -e "${YELLOW}4. Kết quả tổng quan:${NC}"
echo -e "   📊 Tổng số cột: $TOTAL_COUNT"
echo -e "   ✅ Cột khớp: $MATCH_COUNT"
echo -e "   ❌ Cột không khớp: $((TOTAL_COUNT - MATCH_COUNT))"

if [ $MATCH_COUNT -eq $TOTAL_COUNT ]; then
    echo ""
    echo -e "${GREEN}🎉 KẾT LUẬN: Bảng LN03 HOÀN HẢO!${NC}"
    echo -e "${GREEN}   ✅ 20/20 business columns khớp hoàn toàn với CSV${NC}"
    echo -e "${GREEN}   ✅ Thứ tự cột đúng 100%${NC}"
    echo -e "${GREEN}   ✅ Tên cột chính xác 100% (bao gồm 3 cột cuối R, S, T)${NC}"
    echo -e "${GREEN}   ✅ Sẵn sàng import dữ liệu thực tế!${NC}"
else
    echo ""
    echo -e "${RED}⚠️  CÓ KHÁC BIỆT: Cần kiểm tra và điều chỉnh!${NC}"
fi

echo ""
echo -e "${BLUE}📋 THÔNG TIN BỔ SUNG:${NC}"
echo -e "   🏦 Nghiệp vụ: Nợ xử lý rủi ro (Risk Management)"
echo -e "   📊 System columns: +4 (Id, NGAY_DL, CREATED_DATE, UPDATED_DATE, FILE_NAME)"
echo -e "   🔄 Temporal table: Theo dõi lịch sử thay đổi"
echo -e "   ⚡ Columnstore: Tối ưu analytics và reporting"
echo -e "   🆕 Đặc biệt: 3 cột cuối (R, S, T) được thêm cho dữ liệu trống tiêu đề"

echo ""
echo "==========================================="
