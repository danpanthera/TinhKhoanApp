#!/bin/bash

echo "🧹 Cleaning up duplicate using statements..."

# Remove duplicate using TinhKhoanApp.Api.Models.DTOs; statements
find . -name "*.cs" -type f -exec awk '
!seen[$0]++ || !/^using TinhKhoanApp\.Api\.Models\.DTOs;$/ { print }
' {} \; -exec mv {} {}.tmp \; -exec mv {}.tmp {} \;

echo "✅ Cleanup completed!"
