#!/bin/bash
# debug_smart_import.sh
# Script debug Smart Import API issues

echo "🔍 DEBUGGING SMART IMPORT 400 ERROR"
echo "================================="

# Test API endpoint
echo "1. Testing Smart Import endpoint..."
curl -X POST http://localhost:5055/api/DirectImport/smart \
  -H "Content-Type: application/json" \
  -d '{"test": "data"}' \
  -w "\nHTTP Status: %{http_code}\n" \
  -v

echo -e "\n2. Testing health endpoint..."
curl http://localhost:5055/health

echo -e "\n3. Checking backend logs for errors..."
grep -i "error\|exception\|400" backend.log | tail -10

echo -e "\n4. Testing with minimal payload..."
curl -X POST http://localhost:5055/api/DirectImport/smart \
  -H "Content-Type: multipart/form-data" \
  -F "file=@7800_ln03_20241231.csv" \
  -w "\nHTTP Status: %{http_code}\n" 2>/dev/null

echo -e "\n5. Checking DirectImport controller..."
if [ -f "Controllers/DirectImportController.cs" ]; then
    echo "✅ DirectImportController.cs exists"
    grep -n "smart\|Smart" Controllers/DirectImportController.cs | head -5
else
    echo "❌ DirectImportController.cs not found"
fi

echo -e "\n6. Checking available endpoints..."
curl -s http://localhost:5055/swagger/v1/swagger.json | jq '.paths | keys' 2>/dev/null || echo "Swagger not available"

echo -e "\n================================="
echo "Debug completed!"
