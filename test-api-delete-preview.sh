#!/bin/bash

echo "🔍 Test API Delete và Preview functionality"
echo ""

# Test API health
echo "1. Testing API health..."
curl -s "http://localhost:5055/health" | jq '.status'

# Test API records (get first few)
echo ""
echo "2. Getting first 3 records..."
curl -s "http://localhost:5055/api/DataImport/records" | jq '.[0:3] | map({Id, FileName, Category, RecordsCount})'

# Test API preview (using first record)
echo ""
echo "3. Testing preview API..."
RECORD_ID=$(curl -s "http://localhost:5055/api/DataImport/records" | jq '.[0].Id')
echo "Testing with ID: $RECORD_ID"
curl -s -w "\nHTTP_CODE:%{http_code}\n" "http://localhost:5055/api/DataImport/preview/$RECORD_ID"

# Test API delete (using last record to be safe)
echo ""
echo "4. Testing delete API..."
LAST_RECORD_ID=$(curl -s "http://localhost:5055/api/DataImport/records" | jq '.[-1].Id')
echo "Testing delete with ID: $LAST_RECORD_ID"
curl -s -w "\nHTTP_CODE:%{http_code}\n" -X DELETE "http://localhost:5055/api/DataImport/delete/$LAST_RECORD_ID" -H "Content-Type: application/json"

echo ""
echo "🎯 Test completed"
