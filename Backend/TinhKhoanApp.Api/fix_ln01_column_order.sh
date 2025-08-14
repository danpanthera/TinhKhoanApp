#!/bin/bash

# Script to fix LN01 column order
# NGAY_DL first, then 79 business columns, then system columns

echo "🔧 Analyzing LN01 column order issue..."

# Get CSV headers with order numbers starting from 2 (after NGAY_DL)
echo "📋 CSV Headers (79 business columns):"
head -1 /Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_ln01_20241231.csv | tr ',' '\n' | nl -v2

echo ""
echo "📊 Current database schema:"
sqlcmd -S "localhost,1433" -U sa -P "Dientoan@303" -d TinhKhoanDB -C -Q "
SELECT
    c.COLUMN_NAME,
    c.ORDINAL_POSITION,
    CASE
        WHEN c.COLUMN_NAME = 'Id' THEN 'SYSTEM'
        WHEN c.COLUMN_NAME = 'NGAY_DL' THEN 'DATE KEY'
        WHEN c.COLUMN_NAME IN ('FILE_NAME', 'CREATED_DATE', 'UPDATED_DATE', 'ValidFrom', 'ValidTo') THEN 'SYSTEM'
        ELSE 'BUSINESS'
    END AS COLUMN_TYPE
FROM INFORMATION_SCHEMA.COLUMNS c
WHERE c.TABLE_NAME = 'LN01'
AND c.ORDINAL_POSITION <= 10
ORDER BY c.ORDINAL_POSITION"

echo ""
echo "✅ REQUIRED ORDER:"
echo "1. NGAY_DL (Position 1)"
echo "2-80. Business columns (79 columns from CSV)"
echo "81+. System columns (Id, FILE_NAME, CREATED_DATE, etc.)"

echo ""
echo "🚨 ISSUE FOUND: Id column is at position 1, should be at the end"
echo "Need to update model with Order attributes"
