#!/bin/bash
echo "🔧 COMPREHENSIVE FIX: NGAY_DL First Column Order Enforcement"
echo "📋 Target Tables: DP01, DPDA, LN01"
echo "📋 Rule: NGAY_DL(1) → Business Columns(2-N) → System/Temporal Columns(N+1 to end)"
echo ""

# Database connection
SERVER="localhost,1433"
DATABASE="TinhKhoanDB"
USERNAME="sa"
PASSWORD="Dientoan@303"

echo "🔍 Step 1: Analyzing current column counts and structure..."

# Check column counts
echo "DP01 Columns:"
sqlcmd -S $SERVER -U $USERNAME -P $PASSWORD -C -d $DATABASE -Q "
SELECT COUNT(*) as TotalColumns FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DP01';
SELECT TOP 5 COLUMN_NAME, ORDINAL_POSITION FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DP01' ORDER BY ORDINAL_POSITION;
"

echo "DPDA Columns:"
sqlcmd -S $SERVER -U $USERNAME -P $PASSWORD -C -d $DATABASE -Q "
SELECT COUNT(*) as TotalColumns FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DPDA';
SELECT TOP 5 COLUMN_NAME, ORDINAL_POSITION FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'DPDA' ORDER BY ORDINAL_POSITION;
"

echo "LN01 Columns:"
sqlcmd -S $SERVER -U $USERNAME -P $PASSWORD -C -d $DATABASE -Q "
SELECT COUNT(*) as TotalColumns FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LN01';
SELECT TOP 5 COLUMN_NAME, ORDINAL_POSITION FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'LN01' ORDER BY ORDINAL_POSITION;
"

echo ""
echo "✅ ANALYSIS COMPLETE"
echo "📋 NEXT STEPS:"
echo "   1. Update Models (.cs files) - NGAY_DL first in property order"
echo "   2. Create EF Migration to rebuild database structure"
echo "   3. Apply migration to reorder columns"
echo ""
echo "🚨 IMPORTANT: This affects both code and database structure"
