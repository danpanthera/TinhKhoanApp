#!/bin/bash
# replace_old_scripts.sh
# Thay thế script cũ bằng script mới không bị treo

echo "🔄 REPLACING OLD SCRIPTS WITH IMPROVED VERSIONS..."
echo "=================================================="

# Backup old scripts
echo "1. Backing up old scripts..."
cp start_full_app.sh start_full_app.sh.old 2>/dev/null || echo "No old start_full_app.sh"
cp start_backend.sh start_backend.sh.old 2>/dev/null || echo "No old start_backend.sh"

# Replace with improved versions
echo "2. Replacing with improved scripts..."
cp start_full_app_improved.sh start_full_app.sh
cp start_backend_improved.sh start_backend.sh

# Ensure executable
chmod +x start_full_app.sh
chmod +x start_backend.sh
chmod +x emergency_stop.sh
chmod +x check_status.sh

echo "3. Testing new scripts..."
./check_status.sh

echo "✅ SCRIPT REPLACEMENT COMPLETED!"
echo "🎯 Main scripts updated:"
echo "   • start_full_app.sh - Now uses no-hang version"
echo "   • start_backend.sh  - Now uses improved version"
echo "   • emergency_stop.sh - Emergency cleanup"
echo "   • check_status.sh   - Quick status check"
