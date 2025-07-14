#!/bin/bash

echo "=== KIỂM TRA ĐỦ CỘT DP01 ==="

# Lấy CSV columns
head -1 /Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7808_dp01_20241231.csv | sed 's/\xEF\xBB\xBF//' | tr ',' '\n' | sort > csv_cols.txt

# Lấy DB business columns
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB -Q "
SELECT c.name
FROM sys.columns c
WHERE c.object_id = OBJECT_ID('DP01')
    AND c.name NOT IN ('Id', 'CREATED_DATE', 'UPDATED_DATE', 'FILE_NAME', 'ValidFrom', 'ValidTo', 'NGAY_DL')
ORDER BY c.name
" -h -1 | grep -v "^$" | grep -v "affected" | sort > db_cols.txt

echo "CSV cột: $(cat csv_cols.txt | wc -l)"
echo "DB cột:  $(cat db_cols.txt | wc -l)"

echo ""
echo "Cột CSV thiếu trong DB:"
comm -23 csv_cols.txt db_cols.txt

echo ""
echo "Cột DB thừa:"
comm -13 csv_cols.txt db_cols.txt

echo ""
if diff csv_cols.txt db_cols.txt > /dev/null; then
    echo "🎉 HOÀN HẢO: DP01 có đủ 63 cột business từ CSV!"
else
    echo "⚠️  Có khác biệt cần kiểm tra"
fi

rm -f csv_cols.txt db_cols.txt
