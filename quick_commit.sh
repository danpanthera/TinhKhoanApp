#!/bin/bash
# Quick commit script for TinhKhoanApp
# Usage: ./quick_commit.sh "Your commit message"

if [ -z "$1" ]; then
    echo "❌ Error: Please provide a commit message"
    echo "Usage: ./quick_commit.sh \"Your commit message\""
    exit 1
fi

echo "🚀 Quick committing with message: $1"
git add . && git commit --no-verify -m "$1"

if [ $? -eq 0 ]; then
    echo "✅ Commit successful!"
else
    echo "❌ Commit failed!"
fi
