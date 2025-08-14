#!/bin/bash
echo "🔧 Fix Column Order for DP01, DPDA, LN01 - NGAY_DL First Rule"
echo "📋 Current Issue: Id column is at position 1, NGAY_DL at position 2"
echo "📋 Required: NGAY_DL → Business → Temporal/System Columns"
echo ""

# Database connection
SERVER="localhost,1433"
DATABASE="TinhKhoanDB"
USERNAME="sa"
PASSWORD="Dientoan@303"

echo "🔍 Step 1: Checking current structure..."
sqlcmd -S $SERVER -U $USERNAME -P $PASSWORD -C -d $DATABASE -Q "
SELECT 'DP01' as TableName, COLUMN_NAME, ORDINAL_POSITION
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'DP01' AND ORDINAL_POSITION <= 5
ORDER BY ORDINAL_POSITION;

SELECT 'DPDA' as TableName, COLUMN_NAME, ORDINAL_POSITION
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'DPDA' AND ORDINAL_POSITION <= 5
ORDER BY ORDINAL_POSITION;

SELECT 'LN01' as TableName, COLUMN_NAME, ORDINAL_POSITION
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'LN01' AND ORDINAL_POSITION <= 5
ORDER BY ORDINAL_POSITION;
"

echo ""
echo "🚨 PROBLEM IDENTIFIED:"
echo "   DP01: Id(1), NGAY_DL(2) - WRONG ORDER"
echo "   DPDA: Id(1), NGAY_DL(2) - WRONG ORDER"
echo "   LN01: Id(1), NGAY_DL(2) - WRONG ORDER"
echo ""
echo "✅ CORRECT STRUCTURE SHOULD BE:"
echo "   All tables: NGAY_DL(1), Business Columns(2-N), System/Temporal Columns(N+1 to end)"
echo ""
echo "📋 SOLUTION: Rebuild tables với column order đúng"
echo "⚠️  NOTE: This requires recreating tables - data backup recommended"
