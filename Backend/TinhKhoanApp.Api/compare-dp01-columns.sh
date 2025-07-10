#!/bin/bash

# ===================================================================
# SCRIPT SO SÁNH CHI TIẾT TÊN CỘT THEO HEADER CSV
# ===================================================================

echo "🔍 ===== SO SÁNH CHI TIẾT TÊN CỘT DP01 ====="
echo ""

# Header CSV gốc cho DP01
DP01_HEADER="MA_CN,TAI_KHOAN_HACH_TOAN,MA_KH,TEN_KH,DP_TYPE_NAME,CCY,CURRENT_BALANCE,RATE,SO_TAI_KHOAN,OPENING_DATE,MATURITY_DATE,ADDRESS,NOTENO,MONTH_TERM,TERM_DP_NAME,TIME_DP_NAME,MA_PGD,TEN_PGD,DP_TYPE_CODE,RENEW_DATE,CUST_TYPE,CUST_TYPE_NAME,CUST_TYPE_DETAIL,CUST_DETAIL_NAME,PREVIOUS_DP_CAP_DATE,NEXT_DP_CAP_DATE,ID_NUMBER,ISSUED_BY,ISSUE_DATE,SEX_TYPE,BIRTH_DATE,TELEPHONE,ACRUAL_AMOUNT,ACRUAL_AMOUNT_END,ACCOUNT_STATUS,DRAMT,CRAMT,EMPLOYEE_NUMBER,EMPLOYEE_NAME,SPECIAL_RATE,AUTO_RENEWAL,CLOSE_DATE,LOCAL_PROVIN_NAME,LOCAL_DISTRICT_NAME,LOCAL_WARD_NAME,TERM_DP_TYPE,TIME_DP_TYPE,STATES_CODE,ZIP_CODE,COUNTRY_CODE,TAX_CODE_LOCATION,MA_CAN_BO_PT,TEN_CAN_BO_PT,PHONG_CAN_BO_PT,NGUOI_NUOC_NGOAI,QUOC_TICH,MA_CAN_BO_AGRIBANK,NGUOI_GIOI_THIEU,TEN_NGUOI_GIOI_THIEU,CONTRACT_COUTS_DAY,SO_KY_AD_LSDB,UNTBUSCD,TYGIA"

echo "📄 Lấy cột từ Model DP01.cs (loại bỏ cột system)..."
grep "\[Column(" Models/DataTables/DP01.cs | grep -o '"[^"]*"' | tr -d '"' | grep -v "NGAY_DL" | grep -v "CREATED_DATE" | grep -v "UPDATED_DATE" | grep -v "FILE_NAME" | sort > /tmp/dp01_model_cols.txt

echo "📄 Lấy cột từ Header CSV..."
echo "$DP01_HEADER" | tr ',' '\n' | sort > /tmp/dp01_header_cols.txt

echo ""
echo "📊 So sánh số lượng:"
MODEL_COUNT=$(cat /tmp/dp01_model_cols.txt | wc -l | tr -d ' ')
HEADER_COUNT=$(cat /tmp/dp01_header_cols.txt | wc -l | tr -d ' ')
echo "   Model có: $MODEL_COUNT cột"
echo "   Header có: $HEADER_COUNT cột"

echo ""
echo "🔍 Cột có trong Header nhưng thiếu trong Model:"
comm -23 /tmp/dp01_header_cols.txt /tmp/dp01_model_cols.txt

echo ""
echo "🔍 Cột có trong Model nhưng không có trong Header:"
comm -13 /tmp/dp01_header_cols.txt /tmp/dp01_model_cols.txt

echo ""
if [ "$MODEL_COUNT" -eq "$HEADER_COUNT" ] && [ -z "$(comm -3 /tmp/dp01_header_cols.txt /tmp/dp01_model_cols.txt)" ]; then
    echo "✅ DP01: HOÀN TOÀN CHÍNH XÁC!"
else
    echo "❌ DP01: CÓ SAI SÓT!"
fi

# Cleanup
rm -f /tmp/dp01_model_cols.txt /tmp/dp01_header_cols.txt
