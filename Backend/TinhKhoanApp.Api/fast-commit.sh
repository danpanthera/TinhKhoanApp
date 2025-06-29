#!/bin/bash
# 🚀 Fast Commit Script - Tự động commit thay đổi nhanh

echo "🔄 Fast Commit Script - Bắt đầu..."

# Chuyển về thư mục Backend
cd "$(dirname "$0")"

# Kiểm tra nếu có thay đổi
if [[ -z $(git status -s) ]]; then
    echo "✅ Không có thay đổi nào để commit"
    exit 0
fi

# Hiển thị thay đổi
echo "📝 Các file đã thay đổi:"
git status -s

# Add tất cả thay đổi
git add -A

# Auto-generated commit message
TIMESTAMP=$(date "+%Y-%m-%d %H:%M:%S")
BRANCH=$(git branch --show-current)
CHANGED_FILES=$(git diff --cached --name-only | wc -l | xargs)

COMMIT_MSG="🔄 Auto commit: $CHANGED_FILES files changed [$TIMESTAMP]

Changes:
$(git diff --cached --name-only | sed 's/^/- /')

Branch: $BRANCH"

# Commit với message tự động
git commit -m "$COMMIT_MSG"

echo "✅ Đã commit thành công với $CHANGED_FILES files"
echo "📄 Commit message: Auto commit [$TIMESTAMP]"
