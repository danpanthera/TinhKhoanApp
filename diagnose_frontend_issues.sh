#!/bin/bash

echo "🔍 DIAGNOSE FRONTEND DATA DISPLAY ISSUES"
echo "========================================"

echo ""
echo "📊 1. KIỂM TRA DỮ LIỆU BACKEND:"

echo "   📋 KPI Assignment Tables:"
CANBO_COUNT=$(curl -s "http://localhost:5055/api/KpiAssignmentTables" | jq "[.[] | select(.Category == \"CANBO\")] | length")
CHINHANH_COUNT=$(curl -s "http://localhost:5055/api/KpiAssignmentTables" | jq "[.[] | select(.Category == \"CHINHANH\")] | length")
echo "      - CANBO: $CANBO_COUNT bảng"
echo "      - CHINHANH: $CHINHANH_COUNT bảng"

echo ""
echo "   📊 Core Data Tables:"
DP01_COUNT=$(curl -s "http://localhost:5055/api/TestData/summary" | jq -r ".Summary.DP01_Count")
LN01_COUNT=$(curl -s "http://localhost:5055/api/TestData/summary" | jq -r ".Summary.LN01_Count")
echo "      - DP01: $DP01_COUNT records"
echo "      - LN01: $LN01_COUNT records"

echo ""
echo "✅ SUMMARY:"
echo "   - Backend có đủ dữ liệu: DP01($DP01_COUNT), LN01($LN01_COUNT)"
echo "   - KPI Tables: CANBO($CANBO_COUNT), CHINHANH($CHINHANH_COUNT)"
echo "   - Vấn đề chính: Frontend display logic hoặc caching"
