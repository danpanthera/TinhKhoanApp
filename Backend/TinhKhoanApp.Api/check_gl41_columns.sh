#!/bin/bash

echo "🔍 KIỂM TRA ĐỐI CHIẾU CỘT GL41 vs CSV"
echo "=================================="

echo ""
echo "📊 CSV Header có 13 cột:"
CSV_COLS="MA_CN,LOAI_TIEN,MA_TK,TEN_TK,LOAI_BT,DN_DAUKY,DC_DAUKY,SBT_NO,ST_GHINO,SBT_CO,ST_GHICO,DN_CUOIKY,DC_CUOIKY"

echo "$CSV_COLS" | tr ',' '\n' | nl

echo ""
echo "🎯 Model GL41.cs có các cột business data:"
grep "\[Column(" Models/DataTables/GL41.cs | \
grep -v "NgayDL\|NGAY_DL\|CREATED_DATE\|UPDATED_DATE" | \
sed 's/.*\[Column("\([^"]*\)").*/\1/' | \
nl

echo ""
echo "✅ KIỂM TRA SO SÁNH:"
echo "CSV có $(echo "$CSV_COLS" | tr ',' '\n' | wc -l) cột"
echo "Model có $(grep "\[Column(" Models/DataTables/GL41.cs | grep -v "NgayDL\|NGAY_DL\|CREATED_DATE\|UPDATED_DATE" | wc -l) cột business data"

echo ""
echo "🔍 Tìm cột khác biệt:"
# Tạo file tạm chứa CSV columns
echo "$CSV_COLS" | tr ',' '\n' > /tmp/csv_gl41_cols.txt

# Tạo file tạm chứa Model columns
grep "\[Column(" Models/DataTables/GL41.cs | \
grep -v "NgayDL\|NGAY_DL\|CREATED_DATE\|UPDATED_DATE" | \
sed 's/.*\[Column("\([^"]*\)").*/\1/' > /tmp/model_gl41_cols.txt

echo "❌ Cột trong CSV nhưng KHÔNG có trong Model:"
comm -23 <(sort /tmp/csv_gl41_cols.txt) <(sort /tmp/model_gl41_cols.txt)

echo ""
echo "❌ Cột trong Model nhưng KHÔNG có trong CSV:"
comm -13 <(sort /tmp/csv_gl41_cols.txt) <(sort /tmp/model_gl41_cols.txt)

echo ""
echo "✅ Cột có trong cả CSV và Model:"
comm -12 <(sort /tmp/csv_gl41_cols.txt) <(sort /tmp/model_gl41_cols.txt) | wc -l

# Cleanup
rm -f /tmp/csv_gl41_cols.txt /tmp/model_gl41_cols.txt

echo ""
echo "📋 KẾT LUẬN:"
if [ $(comm -23 <(echo "$CSV_COLS" | tr ',' '\n' | sort) <(grep "\[Column(" Models/DataTables/GL41.cs | grep -v "NgayDL\|NGAY_DL\|CREATED_DATE\|UPDATED_DATE" | sed 's/.*\[Column("\([^"]*\)").*/\1/' | sort) | wc -l) -eq 0 ] && \
   [ $(comm -13 <(echo "$CSV_COLS" | tr ',' '\n' | sort) <(grep "\[Column(" Models/DataTables/GL41.cs | grep -v "NgayDL\|NGAY_DL\|CREATED_DATE\|UPDATED_DATE" | sed 's/.*\[Column("\([^"]*\)").*/\1/' | sort) | wc -l) -eq 0 ]; then
    echo "🎉 HOÀN HẢO! Model GL41 đã khớp 100% với CSV"
else
    echo "⚠️  CẦN CẬP NHẬT model GL41 để khớp với CSV"
fi
