#!/bin/bash

# Quick table structure check
echo "🔍 QUICK 7 TABLES CHECK"
echo "======================"

CSV_DIR="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau"

echo ""
echo "DPDA:"
csv=$(head -1 "$CSV_DIR/7808_dpda_20250331.csv" | tr ',' '\n' | wc -l)
db_total=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DPDA'" -h -1 | sed 's/[^0-9]*//g')
db_business=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DPDA' AND COLUMN_NAME NOT IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | sed 's/[^0-9]*//g')
echo "  CSV: $csv, DB Total: $db_total, DB Business: $db_business"
if [ "$csv" -eq "$db_business" ]; then echo "  ✅ MATCH"; else echo "  ❌ MISMATCH"; fi

echo ""
echo "EI01:"
csv=$(head -1 "$CSV_DIR/7808_ei01_20241231.csv" | tr ',' '\n' | wc -l)
db_total=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EI01'" -h -1 | sed 's/[^0-9]*//g')
db_business=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EI01' AND COLUMN_NAME NOT IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | sed 's/[^0-9]*//g')
echo "  CSV: $csv, DB Total: $db_total, DB Business: $db_business"
if [ "$csv" -eq "$db_business" ]; then echo "  ✅ MATCH"; else echo "  ❌ MISMATCH"; fi

echo ""
echo "GL01:"
csv=$(head -1 "$CSV_DIR/7808_gl01_2025030120250331.csv" | tr ',' '\n' | wc -l)
db_total=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'GL01'" -h -1 | sed 's/[^0-9]*//g')
db_business=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'GL01' AND COLUMN_NAME NOT IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | sed 's/[^0-9]*//g')
echo "  CSV: $csv, DB Total: $db_total, DB Business: $db_business"
if [ "$csv" -eq "$db_business" ]; then echo "  ✅ MATCH"; else echo "  ❌ MISMATCH"; fi

echo ""
echo "GL41:"
csv=$(head -1 "$CSV_DIR/7808_gl41_20250630.csv" | tr ',' '\n' | wc -l)
db_total=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'GL41'" -h -1 | sed 's/[^0-9]*//g')
db_business=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'GL41' AND COLUMN_NAME NOT IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | sed 's/[^0-9]*//g')
echo "  CSV: $csv, DB Total: $db_total, DB Business: $db_business"
if [ "$csv" -eq "$db_business" ]; then echo "  ✅ MATCH"; else echo "  ❌ MISMATCH"; fi

echo ""
echo "LN01:"
csv=$(head -1 "$CSV_DIR/7808_ln01_20241231.csv" | tr ',' '\n' | wc -l)
db_total=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LN01'" -h -1 | sed 's/[^0-9]*//g')
db_business=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LN01' AND COLUMN_NAME NOT IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | sed 's/[^0-9]*//g')
echo "  CSV: $csv, DB Total: $db_total, DB Business: $db_business"
if [ "$csv" -eq "$db_business" ]; then echo "  ✅ MATCH"; else echo "  ❌ MISMATCH"; fi

echo ""
echo "LN03:"
csv=$(head -1 "$CSV_DIR/7808_ln03_20241231.csv" | tr ',' '\n' | wc -l)
db_total=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LN03'" -h -1 | sed 's/[^0-9]*//g')
db_business=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LN03' AND COLUMN_NAME NOT IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | sed 's/[^0-9]*//g')
echo "  CSV: $csv, DB Total: $db_total, DB Business: $db_business"
if [ "$csv" -eq "$db_business" ]; then echo "  ✅ MATCH"; else echo "  ❌ MISMATCH"; fi

echo ""
echo "RR01:"
csv=$(head -1 "$CSV_DIR/7800_rr01_20250531.csv" | tr ',' '\n' | wc -l)
db_total=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'RR01'" -h -1 | sed 's/[^0-9]*//g')
db_business=$(sqlcmd -S localhost,1433 -U SA -P YourStrong@Password123 -d TinhKhoanDB -C -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'RR01' AND COLUMN_NAME NOT IN ('Id','NGAY_DL','CREATED_DATE','UPDATED_DATE','FILE_NAME')" -h -1 | sed 's/[^0-9]*//g')
echo "  CSV: $csv, DB Total: $db_total, DB Business: $db_business"
if [ "$csv" -eq "$db_business" ]; then echo "  ✅ MATCH"; else echo "  ❌ MISMATCH"; fi
