#!/bin/bash

# =====================================================
# TEST DIRECT IMPORT DP01 - KIỂM TRA CHỨC NĂNG
# Test import CSV vào bảng DP01 thay vì DP01_New
# =====================================================

echo "🧪 TEST DIRECT IMPORT VÀO BẢNG DP01..."

# Test với file CSV nhỏ
TEST_CSV="/tmp/test_dp01.csv"

# Tạo file CSV test với header giống DP01
echo "Id,NGAY_DL,MA_CN,MA_PGD,TAI_KHOAN_HACH_TOAN,CURRENT_BALANCE,ACRUAL_AMOUNT_END,DRAMT,CRAMT,ACRUAL_AMOUNT,CREATED_DATE,UPDATED_DATE,FILE_NAME,ACCOUNT_STATUS,ADDRESS,AUTO_RENEWAL,BIRTH_DATE,CCY,CLOSE_DATE,CONTRACT_COUTS_DAY,COUNTRY_CODE,CUST_DETAIL_NAME,CUST_TYPE,CUST_TYPE_DETAIL,CUST_TYPE_NAME,DP_TYPE_CODE,DP_TYPE_NAME,EMPLOYEE_NAME,EMPLOYEE_NUMBER,ID_NUMBER,ISSUED_BY,ISSUE_DATE,LOCAL_DISTRICT_NAME,LOCAL_PROVIN_NAME,LOCAL_WARD_NAME,MATURITY_DATE,MA_CAN_BO_AGRIBANK,MA_CAN_BO_PT,MA_KH,MONTH_TERM,NEXT_DP_CAP_DATE,NGUOI_GIOI_THIEU,NGUOI_NUOC_NGOAI,NOTENO,OPENING_DATE,PHONG_CAN_BO_PT,PREVIOUS_DP_CAP_DATE,QUOC_TICH,RATE,RENEW_DATE,SEX_TYPE,SO_KY_AD_LSDB,SO_TAI_KHOAN,SPECIAL_RATE,STATES_CODE,TAX_CODE_LOCATION,TELEPHONE,TEN_CAN_BO_PT,TEN_KH,TEN_NGUOI_GIOI_THIEU,TEN_PGD,TERM_DP_NAME,TERM_DP_TYPE,TIME_DP_NAME,TIME_DP_TYPE,TYGIA,UNTBUSCD,ZIP_CODE" > $TEST_CSV

# Thêm 1 dòng dữ liệu test
echo "1,2025-07-13,001,PGD001,123456,1000000.00,0,0,0,0,2025-07-13,2025-07-13,test_dp01.csv,ACTIVE,Test Address,Y,1990-01-01,VND,2026-07-13,365,VN,Test Customer,INDIVIDUAL,REGULAR,Regular Customer,TD01,Time Deposit,Test Employee,EMP001,123456789,Test Authority,2025-01-01,Test District,Test Province,Test Ward,2026-07-13,CB001,PT001,KH001,12,2026-07-13,GT001,N,NOTE001,2025-07-13,Test Department,2025-06-13,VN,5.50,2025-07-13,M,SKY001,TK001,6.00,ACTIVE,Test Tax Location,0123456789,Test PT Officer,Test Customer Name,Test Introducer,Test PGD,12 Month Term,TD,Time Deposit Type,TD,23000,UNIT001,100000" >> $TEST_CSV

echo "📄 File CSV test đã tạo: $TEST_CSV"

# Test API import
echo "🚀 Test import CSV vào bảng DP01..."
curl -X POST "http://localhost:5055/api/directimport/dp01" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@$TEST_CSV" \
  -F "clearExisting=true" | jq '.'

echo ""
echo "📊 Kiểm tra số lượng records sau import..."
curl -X GET "http://localhost:5055/api/testdata/summary" | jq '.'

echo ""
echo "🔍 Kiểm tra dữ liệu trong database..."
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -d TinhKhoanDB -C -Q "
SELECT
    COUNT(*) as TotalRecords,
    MAX(TEN_KH) as SampleCustomerName,
    MAX(MA_CN) as SampleBranchCode
FROM DP01;
" -h -1

echo ""
echo "✅ Test hoàn thành!"

# Cleanup
rm -f $TEST_CSV
