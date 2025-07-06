#!/bin/bash

echo "Testing Smart Data Import API..."

# Start API server in background (only if not already running)
# dotnet run &
# sleep 5  # Wait for server to start

# Test DP01 import
echo "Testing DP01 file import..."
curl -X POST "http://localhost:5055/api/SmartDataImport/import-file" \
     -H "Content-Type: multipart/form-data" \
     -F "file=@test_debug_7800_DP01_20241231.csv" \
     | jq

echo "Testing DT_KHKD1 file import..."
curl -X POST "http://localhost:5055/api/SmartDataImport/import-file" \
     -H "Content-Type: multipart/form-data" \
     -F "file=@test_7800_DT_KHKD1_202412.csv" \
     | jq

echo "Done!"
