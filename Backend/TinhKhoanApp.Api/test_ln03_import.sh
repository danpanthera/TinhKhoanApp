#!/bin/bash
echo "🧪 Testing LN03 Import với 20 cột..."

LN03_FILE="/Users/nguyendat/Documents/DuLieuImport/DuLieuMau/7800_ln03_20241231.csv"

if [ -f "$LN03_FILE" ]; then
    echo "Testing với file: $LN03_FILE"
    
    # Test bằng curl
    curl -X POST \
      http://localhost:5055/api/DirectImport/smart \
      -F "file=@$LN03_FILE" \
      -F "statementDate=2024-12-31" \
      -H "Content-Type: multipart/form-data"
      
    echo ""
    echo "✅ Test completed!"
else
    echo "❌ File không tìm thấy: $LN03_FILE"
fi
