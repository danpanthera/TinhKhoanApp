#!/bin/bash

# Cleanup Script for TinhKhoanApp Test Files
# This script will remove redundant test files while preserving essential ones

echo "🧹 Starting test files cleanup..."

# Navigate to project root
cd "$(dirname "$0")"

# Move to public directory
cd public

# Create backup directory
mkdir -p ../backup/test-files
echo "📁 Created backup directory"

# Files to keep (most recent/final versions and essential tests)
KEEP_FILES=(
  "test-final-kpi-assignment-fixes.html"
  "test-final-employee-filter.html"
  "test-cb-validation-final.html"
  "test-audio.html"
  "test-validation-fixes.html"
  "test-vietnamese-fonts.html"
  "test-dashboard-api.html"
  "test-date-functionality.html"
)

# Backup and remove empty files
for file in test-*.html; do
  if [ ! -s "$file" ]; then
    echo "🗑️ Removing empty file: $file"
    rm "$file"
  elif [[ ! " ${KEEP_FILES[@]} " =~ " ${file} " ]]; then
    echo "📦 Backing up: $file"
    cp "$file" "../backup/test-files/"
    echo "🗑️ Removing redundant test file: $file"
    rm "$file"
  else
    echo "✅ Keeping essential test file: $file"
  fi
done

# Clean up test scripts in root directory
cd ..
mkdir -p backup/test-scripts

# Keep only final verification scripts
KEEP_SCRIPTS=(
  "final-kpi-fixes-verification.sh"
)

for file in test-*.sh; do
  if [[ ! " ${KEEP_SCRIPTS[@]} " =~ " ${file} " ]]; then
    echo "📦 Backing up: $file"
    cp "$file" "backup/test-scripts/"
    echo "🗑️ Removing redundant test script: $file"
    rm "$file"
  else
    echo "✅ Keeping essential test script: $file"
  fi
done

echo "✨ Cleanup completed. Removed files are backed up in backup/test-files and backup/test-scripts"
