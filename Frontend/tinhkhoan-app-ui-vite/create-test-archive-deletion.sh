#!/bin/bash

# Script để tạo file archive test cho việc kiểm tra xóa file nén - ĐỒNG BỘ TẤT CẢ LOẠI DỮ LIỆU

echo "🔧 Tạo file test archive cho tất cả loại dữ liệu..."

# Tạo thư mục temp cho test data
mkdir -p test_archive_data

# 🏦 LN01 - Dữ liệu LOAN
cat > test_archive_data/LN01_test_data.csv << 'EOF'
MANDT,BUKRS,KUNNR,NAME1,GJAHR,MONAT,WAERS,DMBTR,WRBTR
100,1000,0000123456,Test Customer LN01-1,2025,01,VND,100000000,100000000
100,1000,0000123457,Test Customer LN01-2,2025,01,VND,200000000,200000000
100,1000,0000123458,Test Customer LN01-3,2025,01,VND,300000000,300000000
EOF

# 🔄 LN02 - Sao kê biến động nhóm nợ
cat > test_archive_data/LN02_test_data.csv << 'EOF'
MANDT,BUKRS,KUNNR,NAME1,OLD_GROUP,NEW_GROUP,CHANGE_DATE,REASON
100,1000,0000223456,Customer LN02-1,1,2,2025-01-15,Risk deterioration
100,1000,0000223457,Customer LN02-2,2,3,2025-01-16,Payment delay
100,1000,0000223458,Customer LN02-3,3,2,2025-01-17,Risk improvement
EOF

# ⚠️ LN03 - Dữ liệu Nợ XLRR
cat > test_archive_data/LN03_test_data.csv << 'EOF'
MANDT,BUKRS,KUNNR,NAME1,PRINCIPAL_DUE,INTEREST_DUE,OVERDUE_DAYS,CLASSIFICATION
100,1000,0000323456,NPL Customer LN03-1,50000000,5000000,90,Substandard
100,1000,0000323457,NPL Customer LN03-2,75000000,7500000,180,Doubtful
100,1000,0000323458,NPL Customer LN03-3,100000000,10000000,360,Loss
EOF

# 💰 DP01 - Dữ liệu Tiền gửi
cat > test_archive_data/DP01_test_data.csv << 'EOF'
MANDT,BUKRS,KONTO,SALDO,WAERS,GJAHR,MONAT,INTEREST_RATE,TERM
100,1000,1234567890,10000000,VND,2025,01,3.5,12
100,1000,1234567891,20000000,VND,2025,01,4.0,6
100,1000,1234567892,30000000,VND,2025,01,4.5,24
EOF

# 📱 EI01 - Mobile banking
cat > test_archive_data/EI01_test_data.csv << 'EOF'
MANDT,BUKRS,ACCOUNT_NO,TRANSACTION_TYPE,AMOUNT,FEE,DATETIME,STATUS
100,1000,MB30001,Transfer,500000,11000,2025-01-15 09:30:00,Success
100,1000,MB30002,Bill Payment,200000,0,2025-01-15 10:15:00,Success
100,1000,MB30003,Mobile Topup,100000,0,2025-01-15 11:00:00,Failed
EOF

# 📖 GL01 - Bút toán GDV
cat > test_archive_data/GL01_test_data.csv << 'EOF'
MANDT,BUKRS,GJAHR,BELNR,BUZEI,HKONT,SHKZG,DMBTR,WRBTR,WAERS,TEXT
100,1000,2025,1000000001,001,1131000,S,1000000,1000000,VND,Teller transaction
100,1000,2025,1000000001,002,2111000,H,1000000,1000000,VND,Teller transaction
100,1000,2025,1000000002,001,1131000,S,2000000,2000000,VND,Cash deposit
EOF

# 💳 DPDA - Sao kê phát hành thẻ
cat > test_archive_data/DPDA_test_data.csv << 'EOF'
MANDT,BUKRS,CARD_NO,CARDHOLDER_NAME,CARD_TYPE,ISSUE_DATE,EXPIRY_DATE,STATUS
100,1000,****-****-****-1001,DPDA Cardholder 1,Visa Debit,2025-01-15,2028-01-15,Active
100,1000,****-****-****-1002,DPDA Cardholder 2,Mastercard Credit,2025-01-14,2028-01-14,Active
100,1000,****-****-****-1003,DPDA Cardholder 3,NAPAS ATM,2025-01-13,2028-01-13,Blocked
EOF

# 🏠 DB01 - TSDB và Không TSDB
cat > test_archive_data/DB01_test_data.csv << 'EOF'
MANDT,BUKRS,ACCOUNT_NO,CUSTOMER_NAME,COLLATERAL_TYPE,COLLATERAL_VALUE,LTV_RATIO,VALUATION_DATE
100,1000,TSDB40001,DB01 Customer 1,Real Estate,500000000,70,2025-01-15
100,1000,TSDB40002,DB01 Customer 2,Vehicle,200000000,80,2025-01-14
100,1000,TSDB40003,DB01 Customer 3,Equipment,300000000,75,2025-01-13
EOF

# 🏢 KH03 - Khách hàng pháp nhân
cat > test_archive_data/KH03_test_data.csv << 'EOF'
MANDT,BUKRS,CUSTOMER_ID,COMPANY_NAME,TAX_CODE,CHARTER_CAPITAL,REVENUE,PROFIT
100,1000,KH50001,ABC Company Ltd,0123456789,10000000000,50000000000,2000000000
100,1000,KH50002,XYZ Corporation,0123456790,15000000000,75000000000,3500000000
100,1000,KH50003,DEF Enterprise,0123456791,20000000000,100000000000,5000000000
EOF

# 💹 BC57 - Lãi dự thu
cat > test_archive_data/BC57_test_data.csv << 'EOF'
MANDT,BUKRS,ACCOUNT_NO,CUSTOMER_NAME,ACCRUED_INTEREST,COLLECTED_INTEREST,REMAINING_INTEREST,ACCRUAL_DATE
100,1000,BC57_60001,BC57 Customer 1,5000000,2000000,3000000,2025-01-15
100,1000,BC57_60002,BC57 Customer 2,6000000,2500000,3500000,2025-01-14
100,1000,BC57_60003,BC57 Customer 3,7000000,3000000,4000000,2025-01-13
EOF

# 🚨 RR01 - Dư nợ gốc, lãi XLRR
cat > test_archive_data/RR01_test_data.csv << 'EOF'
MANDT,BUKRS,ACCOUNT_NO,CUSTOMER_NAME,PRINCIPAL_OUTSTANDING,INTEREST_OUTSTANDING,TOTAL_OUTSTANDING,NPL_DATE
100,1000,RR01_70001,RR01 Customer 1,100000000,10000000,110000000,2024-07-15
100,1000,RR01_70002,RR01 Customer 2,125000000,12000000,137000000,2024-06-20
100,1000,RR01_70003,RR01 Customer 3,150000000,15000000,165000000,2024-05-10
EOF

# 📊 7800_DT_KHKD1 - Báo cáo KHKD (DT)
cat > test_archive_data/7800_DT_KHKD1_test_data.csv << 'EOF'
MANDT,BUKRS,BRANCH_CODE,BRANCH_NAME,TARGET_REVENUE,ACTUAL_REVENUE,VARIANCE,ACHIEVEMENT_RATE
100,1000,CN101,Branch 101,1000000000,1200000000,200000000,120
100,1000,CN102,Branch 102,1500000000,1400000000,-100000000,93.3
100,1000,CN103,Branch 103,2000000000,2200000000,200000000,110
EOF

# 📋 GLCB41 - Bảng cân đối
cat > test_archive_data/GLCB41_test_data.csv << 'EOF'
MANDT,BUKRS,ACCOUNT_CODE,ACCOUNT_NAME,OPENING_BALANCE,DEBIT_TURNOVER,CREDIT_TURNOVER,CLOSING_BALANCE
100,1000,TK1010,Cash and Banks,500000000,200000000,150000000,550000000
100,1000,TK1020,Short-term Investments,600000000,250000000,180000000,670000000
100,1000,TK1030,Loans and Advances,700000000,300000000,200000000,800000000
EOF

echo "📁 Tạo file archive test cho tất cả loại dữ liệu..."

# Tạo file ZIP cho từng loại dữ liệu
if command -v zip >/dev/null 2>&1; then
    echo "📦 Tạo test archives cho tất cả loại dữ liệu..."
    
    # LN series
    zip -j test_archive_LN01.zip test_archive_data/LN01_test_data.csv
    zip -j test_archive_LN02.zip test_archive_data/LN02_test_data.csv
    zip -j test_archive_LN03.zip test_archive_data/LN03_test_data.csv
    
    # DP series
    zip -j test_archive_DP01.zip test_archive_data/DP01_test_data.csv
    zip -j test_archive_DPDA.zip test_archive_data/DPDA_test_data.csv
    
    # Others
    zip -j test_archive_EI01.zip test_archive_data/EI01_test_data.csv
    zip -j test_archive_GL01.zip test_archive_data/GL01_test_data.csv
    zip -j test_archive_DB01.zip test_archive_data/DB01_test_data.csv
    zip -j test_archive_KH03.zip test_archive_data/KH03_test_data.csv
    zip -j test_archive_BC57.zip test_archive_data/BC57_test_data.csv
    zip -j test_archive_RR01.zip test_archive_data/RR01_test_data.csv
    zip -j test_archive_7800_DT_KHKD1.zip test_archive_data/7800_DT_KHKD1_test_data.csv
    zip -j test_archive_GLCB41.zip test_archive_data/GLCB41_test_data.csv
    
    # Multi-file archive
    echo "📦 Tạo test_archive_ALL_TYPES.zip (chứa tất cả loại dữ liệu)..."
    zip -j test_archive_ALL_TYPES.zip test_archive_data/*.csv
fi

# Dọn dẹp thư mục temp
rm -rf test_archive_data

echo "✅ Hoàn thành! Các file test archive đã được tạo cho TẤT CẢ loại dữ liệu:"
ls -la test_archive_*.zip 2>/dev/null || echo "⚠️ Không tìm thấy file zip (có thể chưa cài đặt zip utility)"

echo ""
echo "🧪 Hướng dẫn test đồng bộ tất cả loại dữ liệu:"
echo "1. Mở file test-archive-deletion.html trong browser"
echo "2. Chọn một trong các file test_archive_*.zip vừa tạo (13 loại dữ liệu)"
echo "3. Chọn Data Type tương ứng từ dropdown (LN01, LN02, LN03, DP01, EI01, GL01, DPDA, DB01, KH03, BC57, RR01, 7800_DT_KHKD1, GLCB41)"
echo "4. Click 'Preview Import' để xem preview"
echo "5. Click 'Import Archive' để test import và xóa file nén"
echo "6. Kiểm tra log trong response để xem file nén có được xóa thực sự không"
echo ""
echo "📋 Danh sách file test:"
echo "   - test_archive_LN01.zip (Dữ liệu LOAN)"
echo "   - test_archive_LN02.zip (Biến động nhóm nợ)"
echo "   - test_archive_LN03.zip (Nợ XLRR)"
echo "   - test_archive_DP01.zip (Tiền gửi)"
echo "   - test_archive_DPDA.zip (Phát hành thẻ)"
echo "   - test_archive_EI01.zip (Mobile banking)"
echo "   - test_archive_GL01.zip (Bút toán GDV)"
echo "   - test_archive_DB01.zip (TSDB)"
echo "   - test_archive_KH03.zip (Khách hàng pháp nhân)"
echo "   - test_archive_BC57.zip (Lãi dự thu)"
echo "   - test_archive_RR01.zip (Dư nợ XLRR)"
echo "   - test_archive_7800_DT_KHKD1.zip (Báo cáo KHKD)"
echo "   - test_archive_GLCB41.zip (Bảng cân đối)"
echo "   - test_archive_ALL_TYPES.zip (Tất cả loại - Multi-file)"
