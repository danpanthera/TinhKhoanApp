#!/bin/bash

echo "🔍 KIỂM TRA ĐỐI CHIẾU CỘT DP01 vs CSV"
echo "=================================="

echo ""
echo "📊 CSV Header có 61 cột:"
CSV_COLS="MA_CN,TAI_KHOAN_HACH_TOAN,MA_KH,TEN_KH,DP_TYPE_NAME,CCY,CURRENT_BALANCE,RATE,SO_TAI_KHOAN,OPENING_DATE,MATURITY_DATE,ADDRESS,NOTENO,MONTH_TERM,TERM_DP_NAME,TIME_DP_NAME,MA_PGD,TEN_PGD,DP_TYPE_CODE,RENEW_DATE,CUST_TYPE,CUST_TYPE_NAME,CUST_TYPE_DETAIL,CUST_DETAIL_NAME,PREVIOUS_DP_CAP_DATE,NEXT_DP_CAP_DATE,ID_NUMBER,ISSUED_BY,ISSUE_DATE,SEX_TYPE,BIRTH_DATE,TELEPHONE,ACRUAL_AMOUNT,ACRUAL_AMOUNT_END,ACCOUNT_STATUS,DRAMT,CRAMT,EMPLOYEE_NUMBER,EMPLOYEE_NAME,SPECIAL_RATE,AUTO_RENEWAL,CLOSE_DATE,LOCAL_PROVIN_NAME,LOCAL_DISTRICT_NAME,LOCAL_WARD_NAME,TERM_DP_TYPE,TIME_DP_TYPE,STATES_CODE,ZIP_CODE,COUNTRY_CODE,TAX_CODE_LOCATION,MA_CAN_BO_PT,TEN_CAN_BO_PT,PHONG_CAN_BO_PT,NGUOI_NUOC_NGOAI,QUOC_TICH,MA_CAN_BO_AGRIBANK,NGUOI_GIOI_THIEU,TEN_NGUOI_GIOI_THIEU,CONTRACT_COUTS_DAY,SO_KY_AD_LSDB,UNTBUSCD,TYGIA"

echo "$CSV_COLS" | tr ',' '\n' | nl

echo ""
echo "🎯 Model DP01.cs có các cột business data:"
grep "\[Column(" Models/DataTables/DP01.cs | \
grep -v "NGAY_DL\|CREATED_DATE\|UPDATED_DATE\|FILE_NAME" | \
sed 's/.*\[Column("\([^"]*\)").*/\1/' | \
nl

echo ""
echo "✅ KIỂM TRA SO SÁNH:"
echo "CSV có $(echo "$CSV_COLS" | tr ',' '\n' | wc -l) cột"
echo "Model có $(grep "\[Column(" Models/DataTables/DP01.cs | grep -v "NGAY_DL\|CREATED_DATE\|UPDATED_DATE\|FILE_NAME" | wc -l) cột business data"

echo ""
echo "🔍 Tìm cột khác biệt:"
# Tạo file tạm chứa CSV columns
echo "$CSV_COLS" | tr ',' '\n' > /tmp/csv_cols.txt

# Tạo file tạm chứa Model columns
grep "\[Column(" Models/DataTables/DP01.cs | \
grep -v "NGAY_DL\|CREATED_DATE\|UPDATED_DATE\|FILE_NAME" | \
sed 's/.*\[Column("\([^"]*\)").*/\1/' > /tmp/model_cols.txt

echo "❌ Cột trong CSV nhưng KHÔNG có trong Model:"
comm -23 <(sort /tmp/csv_cols.txt) <(sort /tmp/model_cols.txt)

echo ""
echo "❌ Cột trong Model nhưng KHÔNG có trong CSV:"
comm -13 <(sort /tmp/csv_cols.txt) <(sort /tmp/model_cols.txt)

echo ""
echo "✅ Cột có trong cả CSV và Model:"
comm -12 <(sort /tmp/csv_cols.txt) <(sort /tmp/model_cols.txt) | wc -l

# Cleanup
rm -f /tmp/csv_cols.txt /tmp/model_cols.txt

echo ""
echo "📋 KẾT LUẬN:"
if [ $(comm -23 <(echo "$CSV_COLS" | tr ',' '\n' | sort) <(grep "\[Column(" Models/DataTables/DP01.cs | grep -v "NGAY_DL\|CREATED_DATE\|UPDATED_DATE\|FILE_NAME" | sed 's/.*\[Column("\([^"]*\)").*/\1/' | sort) | wc -l) -eq 0 ] && \
   [ $(comm -13 <(echo "$CSV_COLS" | tr ',' '\n' | sort) <(grep "\[Column(" Models/DataTables/DP01.cs | grep -v "NGAY_DL\|CREATED_DATE\|UPDATED_DATE\|FILE_NAME" | sed 's/.*\[Column("\([^"]*\)").*/\1/' | sort) | wc -l) -eq 0 ]; then
    echo "🎉 HOÀN HẢO! Model DP01 đã khớp 100% với CSV"
else
    echo "⚠️  CẦN CẬP NHẬT model DP01 để khớp với CSV"
fi
