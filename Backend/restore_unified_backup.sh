#!/bin/bash

# Unified Architecture Backup Restore Script
# Created: $(date)

BACKUP_FILE="unified_architecture_backup_20250831_221931.tar.gz"
RESTORE_DIR="./Khoan.Api"

echo "🔄 RESTORING UNIFIED ARCHITECTURE FROM BACKUP..."
echo "Backup File: $BACKUP_FILE"
echo "Restore Directory: $RESTORE_DIR"

# Check if backup file exists
if [ ! -f "$BACKUP_FILE" ]; then
    echo "❌ Backup file not found: $BACKUP_FILE"
    exit 1
fi

# Create backup of current state if it exists
if [ -d "$RESTORE_DIR" ]; then
    echo "📦 Creating safety backup of current state..."
    mv "$RESTORE_DIR" "${RESTORE_DIR}_backup_$(date +%Y%m%d_%H%M%S)"
fi

# Extract backup
echo "🚀 Extracting backup..."
tar -xzf "$BACKUP_FILE"

echo "✅ BACKUP RESTORED SUCCESSFULLY!"
echo "🏗️  Unified Architecture with DirectImportController ready"
echo "📁 Files restored to: $RESTORE_DIR"
echo ""
echo "Next Steps:"
echo "1. cd Khoan.Api"
echo "2. dotnet restore"
echo "3. dotnet build"
echo "4. dotnet run"
