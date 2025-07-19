#!/bin/bash

# Script để verify column mapping giữa CSV và Models
# Kiểm tra 63 business columns của DP01

echo "=== DP01 COLUMN VERIFICATION ==="
echo "CSV Columns (expected 63):"
head -1 /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/7808_dp01_20241231.csv | \
  sed 's/﻿//' | tr ',' '\n' | nl

echo ""
echo "Model Business Columns (skipping Id, NGAY_DL, system columns):"
grep '\[Column(' /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Models/DataTables/DP01.cs | \
  grep -v '"Id"' | grep -v '"NGAY_DL"' | \
  grep -v '"CreatedAt"' | grep -v '"UpdatedAt"' | grep -v '"IsDeleted"' | \
  grep -v '"SysStartTime"' | grep -v '"SysEndTime"' | \
  sed 's/.*\[Column("//' | sed 's/".*//' | sed 's/,.*//' | nl

echo ""
echo "Business Column Count Check:"
CSV_COUNT=$(head -1 /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/7808_dp01_20241231.csv | tr ',' '\n' | wc -l)
MODEL_COUNT=$(grep '\[Column(' /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/Models/DataTables/DP01.cs | \
  grep -v '"Id"' | grep -v '"NGAY_DL"' | \
  grep -v '"CreatedAt"' | grep -v '"UpdatedAt"' | grep -v '"IsDeleted"' | \
  grep -v '"SysStartTime"' | grep -v '"SysEndTime"' | wc -l)

echo "CSV columns: $CSV_COUNT"
echo "Model business columns: $MODEL_COUNT"

if [ "$CSV_COUNT" -eq "$MODEL_COUNT" ]; then
    echo "✅ PASS: Column count matches!"
else
    echo "❌ FAIL: Column count mismatch!"
fi
