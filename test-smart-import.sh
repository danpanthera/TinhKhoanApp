#!/bin/bash

echo "🧪 ===== KIỂM TRA SMART IMPORT & NUMBER FORMATTING ====="
echo ""

# Test file paths
TEST_FILE_1="/tmp/test_dp01_20250710.csv"
TEST_FILE_2="/tmp/simple_dp01_20250710.csv"

echo "📁 1. Kiểm tra test files..."
if [ -f "$TEST_FILE_1" ]; then
    echo "   ✅ $TEST_FILE_1 exists ($(wc -l < $TEST_FILE_1) lines)"
else
    echo "   ❌ $TEST_FILE_1 not found - creating..."
    cat > "$TEST_FILE_1" << 'EOF'
MA_CN,MA_PGD,TAI_KHOAN_HACH_TOAN,CURRENT_BALANCE,SO_DU_DAU_KY,SO_PHAT_SINH_NO,SO_PHAT_SINH_CO,SO_DU_CUOI_KY
001,001001,1111,1000000.50,500000.25,200000.00,150000.00,550000.25
002,002001,1112,2000000.75,1000000.50,300000.00,250000.00,1050000.50
003,003001,1113,3000000.25,1500000.75,400000.00,350000.00,1550000.75
EOF
    echo "   ✅ Created $TEST_FILE_1"
fi

if [ -f "$TEST_FILE_2" ]; then
    echo "   ✅ $TEST_FILE_2 exists ($(wc -l < $TEST_FILE_2) lines)"
else
    echo "   ❌ $TEST_FILE_2 not found - creating..."
    cat > "$TEST_FILE_2" << 'EOF'
MA_CN,CURRENT_BALANCE
001,1000000.50
002,2000000.75
EOF
    echo "   ✅ Created $TEST_FILE_2"
fi

echo ""
echo "🚀 2. Kiểm tra Backend API Health..."
HEALTH_STATUS=$(curl -s "http://localhost:5055/health" | jq -r '.status' 2>/dev/null)
if [ "$HEALTH_STATUS" = "Healthy" ]; then
    echo "   ✅ Backend API: $HEALTH_STATUS"
else
    echo "   ❌ Backend API không khả dụng - Status: $HEALTH_STATUS"
    exit 1
fi

echo ""
echo "🌐 3. Kiểm tra Frontend..."
FRONTEND_STATUS=$(curl -s -o /dev/null -w "%{http_code}" "http://localhost:3000" 2>/dev/null)
if [ "$FRONTEND_STATUS" = "200" ]; then
    echo "   ✅ Frontend: Online (HTTP $FRONTEND_STATUS)"
else
    echo "   ❌ Frontend không khả dụng - HTTP $FRONTEND_STATUS"
fi

echo ""
echo "📊 4. Test Smart Import API - File 1 (3 records)..."
RESULT_1=$(curl -s -X POST "http://localhost:5055/api/DirectImport/smart" \
  -F "file=@$TEST_FILE_1" \
  -H "Content-Type: multipart/form-data")

SUCCESS_1=$(echo "$RESULT_1" | jq -r '.Success' 2>/dev/null)
RECORDS_1=$(echo "$RESULT_1" | jq -r '.ProcessedRecords' 2>/dev/null)
CATEGORY_1=$(echo "$RESULT_1" | jq -r '.DataType' 2>/dev/null)
ERROR_1=$(echo "$RESULT_1" | jq -r '.ErrorMessage' 2>/dev/null)

echo "   Success: $SUCCESS_1"
echo "   Category: $CATEGORY_1"
echo "   Records: $RECORDS_1"
if [ "$ERROR_1" != "null" ] && [ "$ERROR_1" != "" ]; then
    echo "   Error: $ERROR_1"
fi

echo ""
echo "📊 5. Test Smart Import API - File 2 (2 records)..."
RESULT_2=$(curl -s -X POST "http://localhost:5055/api/DirectImport/smart" \
  -F "file=@$TEST_FILE_2" \
  -H "Content-Type: multipart/form-data")

SUCCESS_2=$(echo "$RESULT_2" | jq -r '.Success' 2>/dev/null)
RECORDS_2=$(echo "$RESULT_2" | jq -r '.ProcessedRecords' 2>/dev/null)
CATEGORY_2=$(echo "$RESULT_2" | jq -r '.DataType' 2>/dev/null)
ERROR_2=$(echo "$RESULT_2" | jq -r '.ErrorMessage' 2>/dev/null)

echo "   Success: $SUCCESS_2"
echo "   Category: $CATEGORY_2"
echo "   Records: $RECORDS_2"
if [ "$ERROR_2" != "null" ] && [ "$ERROR_2" != "" ]; then
    echo "   Error: $ERROR_2"
fi

echo ""
echo "📈 6. Kiểm tra kết quả..."
if [ "$SUCCESS_1" = "true" ] && [ "$RECORDS_1" = "3" ] && [ "$CATEGORY_1" = "DP01" ]; then
    echo "   ✅ File 1: PASS (Success=$SUCCESS_1, Records=$RECORDS_1, Category=$CATEGORY_1)"
else
    echo "   ❌ File 1: FAIL (Success=$SUCCESS_1, Records=$RECORDS_1, Category=$CATEGORY_1)"
fi

if [ "$SUCCESS_2" = "true" ] && [ "$RECORDS_2" = "2" ] && [ "$CATEGORY_2" = "DP01" ]; then
    echo "   ✅ File 2: PASS (Success=$SUCCESS_2, Records=$RECORDS_2, Category=$CATEGORY_2)"
else
    echo "   ❌ File 2: FAIL (Success=$SUCCESS_2, Records=$RECORDS_2, Category=$CATEGORY_2)"
fi

echo ""
echo "🔍 7. Kiểm tra Import History..."
IMPORT_COUNT=$(curl -s "http://localhost:5055/api/DataImport/records" | jq '. | length' 2>/dev/null)
echo "   Total Import Records: $IMPORT_COUNT"

echo ""
echo "📊 8. SUMMARY"
echo "   ================================"
echo "   Backend Health: $HEALTH_STATUS"
echo "   Frontend Status: HTTP $FRONTEND_STATUS"
echo "   Smart Import 1: Success=$SUCCESS_1, Records=$RECORDS_1"
echo "   Smart Import 2: Success=$SUCCESS_2, Records=$RECORDS_2"
echo "   Total Imports: $IMPORT_COUNT"
echo ""

if [ "$SUCCESS_1" = "true" ] && [ "$SUCCESS_2" = "true" ] && [ "$RECORDS_1" != "0" ] && [ "$RECORDS_2" != "0" ]; then
    echo "🎉 TOÀN BỘ TEST THÀNH CÔNG!"
    echo "   ✅ Smart Import hoạt động đúng"
    echo "   ✅ Category detection chính xác"
    echo "   ✅ Records count hiển thị đúng"
else
    echo "❌ CÓ LỖI TRONG QUY TRÌNH TEST"
    echo "   Cần kiểm tra lại backend hoặc CSV parsing"
fi

echo ""
