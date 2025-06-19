#!/bin/bash

echo "Testing raw data import..."

# Test the import endpoint
echo "Importing file 7800_GL01_20250531.csv..."
response=$(curl -s -X POST "http://localhost:5055/api/RawData/import" \
  -H "Content-Type: multipart/form-data" \
  -F "file=@7800_GL01_20250531.csv" \
  -F "unitCode=7800" \
  -F "category=GL01" \
  -F "month=5" \
  -F "year=2025")

echo "Import response:"
echo "$response"
echo ""

# Check dynamic tables after import
echo "Checking dynamic tables after import..."
tables_response=$(curl -s -X GET "http://localhost:5055/api/RawData/debug/tables")
echo "Tables response:"
echo "$tables_response"
echo ""

# Try to get raw data list
echo "Getting raw data list..."
list_response=$(curl -s -X GET "http://localhost:5055/api/RawData")
echo "List response:"
echo "$list_response"
echo ""
