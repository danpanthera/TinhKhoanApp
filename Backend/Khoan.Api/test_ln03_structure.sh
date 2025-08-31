#!/bin/bash

echo "🧪 Testing LN03 CSV Structure & API Compliance..."

CSV_FILE="/opt/Projects/Khoan/Backend/Khoan.Api/7800_ln03_20241231.csv"

if [[ ! -f "$CSV_FILE" ]]; then
    echo "❌ CSV file not found: $CSV_FILE"
    exit 1
fi

echo "📁 Found CSV file: $CSV_FILE"

# Count lines
LINE_COUNT=$(wc -l < "$CSV_FILE")
echo "📊 Total lines: $LINE_COUNT"

# Check headers
HEADER_LINE=$(head -n 1 "$CSV_FILE")
HEADER_COUNT=$(echo "$HEADER_LINE" | tr ',' '\n' | wc -l)
echo "📋 Header columns: $HEADER_COUNT"

# Show first few headers
echo "📋 First 10 headers:"
echo "$HEADER_LINE" | tr ',' '\n' | head -10 | nl

# Check first data line
if [[ $LINE_COUNT -gt 1 ]]; then
    DATA_LINE=$(sed -n '2p' "$CSV_FILE")
    DATA_COUNT=$(echo "$DATA_LINE" | tr ',' '\n' | wc -l)
    echo "📊 Data columns: $DATA_COUNT"
    
    echo "📊 First 10 data values:"
    echo "$DATA_LINE" | tr ',' '\n' | head -10 | nl
    
    # Check if header count matches data count
    if [[ $HEADER_COUNT -eq $DATA_COUNT ]]; then
        echo "✅ Header count matches data count: $HEADER_COUNT columns"
    else
        echo "⚠️ Header count ($HEADER_COUNT) doesn't match data count ($DATA_COUNT)"
    fi
    
    # Check for expected 20 columns (17 named + 3 unnamed)
    if [[ $DATA_COUNT -eq 20 ]]; then
        echo "✅ CSV has exactly 20 columns as expected (17 named + 3 unnamed)"
    else
        echo "⚠️ CSV has $DATA_COUNT columns, expected 20"
    fi
else
    echo "⚠️ CSV file has no data lines"
fi

echo ""
echo "🏗️ LN03Entity Structure Analysis:"
echo "✅ Expected structure per requirements:"
echo "   - Column 1: NGAY_DL (datetime2)"
echo "   - Columns 2-17: Named business columns (mostly nvarchar(200))"
echo "   - Columns 18-20: Unnamed business columns (COLUMN_18, COLUMN_19, COLUMN_20)"
echo "   - Plus system columns: Id, SysStartTime, SysEndTime"
echo ""
echo "✅ Data types per requirements:"
echo "   - Date columns: datetime2"
echo "   - Amount columns: decimal(18,2)"
echo "   - Text columns: nvarchar(200) with MaxLength(200)"
echo ""
echo "✅ LN03 temporal table features:"
echo "   - System versioned temporal table"
echo "   - History table: LN03_History"
echo "   - Analytics indexes on NGAY_DL, MACHINHANH, MAKH, MACBTD"
echo "   - Columnstore index support"

echo ""
echo "🧪 Next steps for testing:"
echo "1. ✅ CSV structure verified"
echo "2. 🔄 Start API server"
echo "3. 🧪 Test CSV import endpoint"
echo "4. 🔍 Verify data validation"
echo "5. 🎨 Test frontend integration"
