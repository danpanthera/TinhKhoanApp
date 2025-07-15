#!/bin/bash

# 🔍 SIMPLE VERIFICATION OF 7 CORE TABLES
echo "🔍 VERIFY 7 CORE TABLES STRUCTURE"
echo "=================================="

CSV_DIR="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau"

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

echo ""
echo "📊 COMPARISON RESULTS:"
echo "====================="

# DPDA
echo ""
echo -e "${BLUE}DPDA:${NC}"
csv_cols=$(head -1 "$CSV_DIR/7808_dpda_20250331.csv" | tr ',' '\n' | wc -l | tr -d ' ')
total_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DPDA'" -h -1 | tr -d '\r\n ' | sed 's/[^0-9]*//g')
business_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DPDA' AND COLUMN_NAME NOT IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | tr -d '\r\n ' | sed 's/[^0-9]*//g')
system_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DPDA' AND COLUMN_NAME IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | tr -d '\r\n ' | sed 's/[^0-9]*//g')

echo "  📁 CSV: $csv_cols columns"
echo "  🗄️  DB Total: $total_cols (Business: $business_cols + System: $system_cols)"
if [ "$csv_cols" -eq "$business_cols" ]; then
    echo -e "  ✅ ${GREEN}PERFECT MATCH${NC}"
else
    diff=$((business_cols - csv_cols))
    echo -e "  ❌ ${RED}MISMATCH: $diff columns difference${NC}"
fi

# EI01
echo ""
echo -e "${BLUE}EI01:${NC}"
csv_cols=$(head -1 "$CSV_DIR/7808_ei01_20241231.csv" | tr ',' '\n' | wc -l | tr -d ' ')
total_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EI01'" -h -1 | tr -d '\r\n ' | grep -E '^[0-9]+$')
business_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EI01' AND COLUMN_NAME NOT IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | tr -d '\r\n ' | grep -E '^[0-9]+$')
system_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EI01' AND COLUMN_NAME IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | tr -d '\r\n ' | grep -E '^[0-9]+$')

echo "  📁 CSV: $csv_cols columns"
echo "  🗄️  DB Total: $total_cols (Business: $business_cols + System: $system_cols)"
if [ "$csv_cols" -eq "$business_cols" ]; then
    echo -e "  ✅ ${GREEN}PERFECT MATCH${NC}"
else
    diff=$((business_cols - csv_cols))
    echo -e "  ❌ ${RED}MISMATCH: $diff columns difference${NC}"
fi

# GL01
echo ""
echo -e "${BLUE}GL01:${NC}"
csv_cols=$(head -1 "$CSV_DIR/7808_gl01_2025030120250331.csv" | tr ',' '\n' | wc -l | tr -d ' ')
total_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'GL01'" -h -1 | tr -d '\r\n ' | grep -E '^[0-9]+$')
business_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'GL01' AND COLUMN_NAME NOT IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | tr -d '\r\n ' | grep -E '^[0-9]+$')
system_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'GL01' AND COLUMN_NAME IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | tr -d '\r\n ' | grep -E '^[0-9]+$')

echo "  📁 CSV: $csv_cols columns"
echo "  🗄️  DB Total: $total_cols (Business: $business_cols + System: $system_cols)"
if [ "$csv_cols" -eq "$business_cols" ]; then
    echo -e "  ✅ ${GREEN}PERFECT MATCH${NC}"
else
    diff=$((business_cols - csv_cols))
    echo -e "  ❌ ${RED}MISMATCH: $diff columns difference${NC}"
fi

# GL41
echo ""
echo -e "${BLUE}GL41:${NC}"
csv_cols=$(head -1 "$CSV_DIR/7808_gl41_20250630.csv" | tr ',' '\n' | wc -l | tr -d ' ')
total_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'GL41'" -h -1 | tr -d '\r\n ' | grep -E '^[0-9]+$')
business_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'GL41' AND COLUMN_NAME NOT IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | tr -d '\r\n ' | grep -E '^[0-9]+$')
system_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'GL41' AND COLUMN_NAME IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | tr -d '\r\n ' | grep -E '^[0-9]+$')

echo "  📁 CSV: $csv_cols columns"
echo "  🗄️  DB Total: $total_cols (Business: $business_cols + System: $system_cols)"
if [ "$csv_cols" -eq "$business_cols" ]; then
    echo -e "  ✅ ${GREEN}PERFECT MATCH${NC}"
else
    diff=$((business_cols - csv_cols))
    echo -e "  ❌ ${RED}MISMATCH: $diff columns difference${NC}"
fi

# LN01
echo ""
echo -e "${BLUE}LN01:${NC}"
csv_cols=$(head -1 "$CSV_DIR/7808_ln01_20241231.csv" | tr ',' '\n' | wc -l | tr -d ' ')
total_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LN01'" -h -1 | tr -d '\r\n ' | grep -E '^[0-9]+$')
business_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LN01' AND COLUMN_NAME NOT IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | tr -d '\r\n ' | grep -E '^[0-9]+$')
system_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LN01' AND COLUMN_NAME IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | tr -d '\r\n ' | grep -E '^[0-9]+$')

echo "  📁 CSV: $csv_cols columns"
echo "  🗄️  DB Total: $total_cols (Business: $business_cols + System: $system_cols)"
if [ "$csv_cols" -eq "$business_cols" ]; then
    echo -e "  ✅ ${GREEN}PERFECT MATCH${NC}"
else
    diff=$((business_cols - csv_cols))
    echo -e "  ❌ ${RED}MISMATCH: $diff columns difference${NC}"
fi

# LN03
echo ""
echo -e "${BLUE}LN03:${NC}"
csv_cols=$(head -1 "$CSV_DIR/7808_ln03_20241231.csv" | tr ',' '\n' | wc -l | tr -d ' ')
total_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LN03'" -h -1 | tr -d '\r\n ' | grep -E '^[0-9]+$')
business_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LN03' AND COLUMN_NAME NOT IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | tr -d '\r\n ' | grep -E '^[0-9]+$')
system_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LN03' AND COLUMN_NAME IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | tr -d '\r\n ' | grep -E '^[0-9]+$')

echo "  📁 CSV: $csv_cols columns"
echo "  🗄️  DB Total: $total_cols (Business: $business_cols + System: $system_cols)"
if [ "$csv_cols" -eq "$business_cols" ]; then
    echo -e "  ✅ ${GREEN}PERFECT MATCH${NC}"
else
    diff=$((business_cols - csv_cols))
    echo -e "  ❌ ${RED}MISMATCH: $diff columns difference${NC}"
fi

# RR01
echo ""
echo -e "${BLUE}RR01:${NC}"
csv_cols=$(head -1 "$CSV_DIR/7800_rr01_20250531.csv" | tr ',' '\n' | wc -l | tr -d ' ')
total_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'RR01'" -h -1 | tr -d '\r\n ' | grep -E '^[0-9]+$')
business_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'RR01' AND COLUMN_NAME NOT IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | tr -d '\r\n ' | grep -E '^[0-9]+$')
system_cols=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'RR01' AND COLUMN_NAME IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | tr -d '\r\n ' | grep -E '^[0-9]+$')

echo "  📁 CSV: $csv_cols columns"
echo "  🗄️  DB Total: $total_cols (Business: $business_cols + System: $system_cols)"
if [ "$csv_cols" -eq "$business_cols" ]; then
    echo -e "  ✅ ${GREEN}PERFECT MATCH${NC}"
else
    diff=$((business_cols - csv_cols))
    echo -e "  ❌ ${RED}MISMATCH: $diff columns difference${NC}"
fi

echo ""
echo "🎯 SUMMARY:"
echo "==========="
echo "✅ GREEN = CSV columns match database business columns"
echo "❌ RED = Mismatch requiring investigation"
echo ""
echo "System columns: Id, NGAY_DL, CREATED_DATE, UPDATED_DATE, FILE_NAME"
