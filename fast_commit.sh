#!/bin/bash

# =====================================================
# SUPER FAST COMMIT SCRIPT - JULY 13, 2025
# Commit siêu nhanh với tối ưu hóa performance
# =====================================================

if [ $# -eq 0 ]; then
    echo "❌ Usage: ./fast_commit.sh \"commit message\""
    exit 1
fi

COMMIT_MSG="$1"

echo "🚀 SUPER FAST COMMIT..."

# Chỉ add các file quan trọng
git add \
    README_DAT.md \
    Backend/TinhKhoanApp.Api/*.cs \
    Backend/TinhKhoanApp.Api/**/*.cs \
    Backend/TinhKhoanApp.Api/*.sh \
    Frontend/tinhkhoan-app-ui-vite/src/**/*.js \
    Frontend/tinhkhoan-app-ui-vite/src/**/*.vue \
    Frontend/tinhkhoan-app-ui-vite/*.js \
    Frontend/tinhkhoan-app-ui-vite/*.sh \
    restart_project.sh \
    .gitignore \
    2>/dev/null

# Commit nhanh
git commit -m "$COMMIT_MSG"

if [ $? -eq 0 ]; then
    echo "✅ Fast commit completed: $COMMIT_MSG"
else
    echo "❌ Commit failed"
    exit 1
fi
