#!/bin/bash

echo "=== SO SÁNH CHI TIẾT RR01 CSV vs MODEL ==="

# Lấy danh sách cột từ CSV
echo "📋 DANH SÁCH CỘT CSV (25 cột):"
head -1 "/Users/nguyendat/Documents/DuLieuImport/20250531/RR01_20250531_7800/7806_rr01_20250531.csv" | tr ',' '\n' | sed 's/﻿//g' | nl

echo ""
echo "📋 DANH SÁCH CỘT MODEL (business columns only):"
grep -n "Column(" Models/DataTables/RR01.cs | grep -v "NGAY_DL\|CREATED_DATE\|UPDATED_DATE\|FILE_NAME" | nl

echo ""
echo "🔍 CỘT TRONG MODEL:"
grep "Column(" Models/DataTables/RR01.cs | grep -o '"[^"]*"' | sed 's/"//g' | grep -v "NGAY_DL\|CREATED_DATE\|UPDATED_DATE\|FILE_NAME" | nl

echo ""
echo "🎯 PHÂN TÍCH:"
echo "CSV có 25 cột header nhưng dữ liệu chỉ có 24 cột"
echo "Model hiện tại có $(grep -c "\[Column(" Models/DataTables/RR01.cs) cột [Column] attributes"
echo "Trong đó có 4 system columns: NGAY_DL, CREATED_DATE, UPDATED_DATE, FILE_NAME"
echo "Business columns trong model: $(($(grep -c "\[Column(" Models/DataTables/RR01.cs) - 4))"
