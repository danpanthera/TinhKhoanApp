#!/bin/bash

# This script verifies the branch names in the database directly

cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/BranchStandardizerApp

echo "Running verification query..."
dotnet run << EOF
verify
EOF

echo "Done"
