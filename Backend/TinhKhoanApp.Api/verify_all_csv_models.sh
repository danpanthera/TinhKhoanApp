#!/bin/bash

# Comprehensive CSV-Model Column Verification Script
# Kiểm tra tất cả models có đúng số lượng và thứ tự business columns với CSV

echo "🔍 CSV-MODEL COLUMN VERIFICATION REPORT"
echo "======================================"

# Function to count business columns in model (excluding system columns)
count_model_business_columns() {
    local model_file=$1
    grep '\[Column(' "$model_file" | \
      grep -v '"Id"' | grep -v '"NGAY_DL"' | \
      grep -v '"CreatedAt"' | grep -v '"UpdatedAt"' | grep -v '"IsDeleted"' | \
      grep -v '"SysStartTime"' | grep -v '"SysEndTime"' | \
      wc -l
}

# Function to extract business column names from model
extract_model_business_columns() {
    local model_file=$1
    grep '\[Column(' "$model_file" | \
      grep -v '"Id"' | grep -v '"NGAY_DL"' | \
      grep -v '"CreatedAt"' | grep -v '"UpdatedAt"' | grep -v '"IsDeleted"' | \
      grep -v '"SysStartTime"' | grep -v '"SysEndTime"' | \
      sed 's/.*\[Column("//' | sed 's/".*//' | sed 's/,.*//'
}

# Function to verify one table
verify_table() {
    local table_name=$1
    local csv_file=$2
    local model_file=$3

    echo ""
    echo "📊 $table_name VERIFICATION"
    echo "-------------------------"

    if [ ! -f "$csv_file" ]; then
        echo "❌ CSV file not found: $csv_file"
        return 1
    fi

    if [ ! -f "$model_file" ]; then
        echo "❌ Model file not found: $model_file"
        return 1
    fi

    # Count columns
    csv_count=$(head -1 "$csv_file" | tr ',' '\n' | wc -l)
    model_count=$(count_model_business_columns "$model_file")

    echo "CSV columns: $csv_count"
    echo "Model business columns: $model_count"

    # Check count
    if [ "$csv_count" -eq "$model_count" ]; then
        echo "✅ COUNT: PASS - Column count matches"
    else
        echo "❌ COUNT: FAIL - Column count mismatch"
        return 1
    fi

    # Check order (first 5 columns)
    echo ""
    echo "📋 First 5 columns comparison:"
    echo "CSV:   $(head -1 "$csv_file" | cut -d',' -f1-5)"
    echo "Model: $(extract_model_business_columns "$model_file" | head -5 | tr '\n' ',' | sed 's/,$//')"

    # Check exact match of first 5
    csv_first5=$(head -1 "$csv_file" | sed 's/﻿//' | cut -d',' -f1-5)
    model_first5=$(extract_model_business_columns "$model_file" | head -5 | tr '\n' ',' | sed 's/,$//')

    if [ "$csv_first5" = "$model_first5" ]; then
        echo "✅ ORDER: PASS - Column order matches"
    else
        echo "❌ ORDER: FAIL - Column order mismatch"
        return 1
    fi

    echo "✅ $table_name: ALL CHECKS PASSED"
    return 0
}

# Main verification
BACKEND_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api"
MODELS_DIR="$BACKEND_DIR/Models/DataTables"

total_passed=0
total_checked=0

# Verify each table
verify_table "DP01" "$BACKEND_DIR/7808_dp01_20241231.csv" "$MODELS_DIR/DP01.cs"
if [ $? -eq 0 ]; then ((total_passed++)); fi
((total_checked++))

verify_table "EI01" "$BACKEND_DIR/7808_ei01_20241231.csv" "$MODELS_DIR/EI01.cs"
if [ $? -eq 0 ]; then ((total_passed++)); fi
((total_checked++))

verify_table "GL01" "$BACKEND_DIR/sample_gl01.csv" "$MODELS_DIR/GL01.cs"
if [ $? -eq 0 ]; then ((total_passed++)); fi
((total_checked++))

verify_table "LN01" "$BACKEND_DIR/sample_ln01.csv" "$MODELS_DIR/LN01.cs"
if [ $? -eq 0 ]; then ((total_passed++)); fi
((total_checked++))

verify_table "LN03" "$BACKEND_DIR/sample_ln03.csv" "$MODELS_DIR/LN03.cs"
if [ $? -eq 0 ]; then ((total_passed++)); fi
((total_checked++))

verify_table "DPDA" "$BACKEND_DIR/sample_dpda.csv" "$MODELS_DIR/DPDA.cs"
if [ $? -eq 0 ]; then ((total_passed++)); fi
((total_checked++))

verify_table "GL41" "$BACKEND_DIR/sample_gl41.csv" "$MODELS_DIR/GL41.cs"
if [ $? -eq 0 ]; then ((total_passed++)); fi
((total_checked++))

verify_table "RR01" "$BACKEND_DIR/sample_rr01.csv" "$MODELS_DIR/RR01.cs"
if [ $? -eq 0 ]; then ((total_passed++)); fi
((total_checked++))# Summary
echo ""
echo "🎯 FINAL SUMMARY"
echo "==============="
echo "Tables checked: $total_checked"
echo "Tables passed: $total_passed"

if [ "$total_passed" -eq "$total_checked" ]; then
    echo "🎉 ALL TABLES PASSED VERIFICATION!"
    exit 0
else
    echo "⚠️  Some tables failed verification"
    exit 1
fi
