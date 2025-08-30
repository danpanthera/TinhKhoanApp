#!/bin/bash
# Fast commit script theo quy tắc của anh
# Sử dụng: ./fast_commit.sh "commit message"

if [ -z "$1" ]; then
    echo "❌ Thiếu commit message!"
    echo "Sử dụng: ./fast_commit.sh \"your message\""
    exit 1
fi

echo "🚀 Fast commit đang thực hiện..."
echo "📝 Message: $1"

# Add tất cả changes
git add .

# Commit với message
git commit -m "$1"

echo "✅ Fast commit hoàn thành!"
echo "📊 Status sau commit:"
git status --short
