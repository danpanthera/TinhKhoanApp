# Git Performance Optimization Config
# Chạy script này để áp dụng các cấu hình tối ưu git

#!/bin/bash

echo "🚀 Đang cấu hình Git để tăng tốc tối đa..."

# Core performance settings
git config --global core.preloadindex true
git config --global core.fscache true
git config --global core.editor "code --wait"

# Garbage collection optimization  
git config --global gc.auto 256
git config --global gc.autopacklimit 0

# Push/Pull optimization
git config --global push.default simple
git config --global pull.rebase false

# Diff and merge optimization
git config --global merge.tool "code"
git config --global diff.tool "code"

# Cache credentials for 1 hour
git config --global credential.helper "cache --timeout=3600"

# Alias cho tốc độ
git config --global alias.s "status --short"
git config --global alias.a "add ."
git config --global alias.c "commit -m"
git config --global alias.l "log --oneline -10"
git config --global alias.ac '!git add . && git commit -m'
git config --global alias.undo "reset --soft HEAD~1"
git config --global alias.last "log -1 HEAD"
git config --global alias.visual "!gitk"

# Hiển thị cấu hình hiện tại
echo "✅ Cấu hình hoàn tất! Các alias có sẵn:"
echo "git s    - status short"
echo "git a    - add all"
echo "git c    - commit with message"
echo "git l    - log short"
echo "git ac   - add and commit"
echo "git undo - undo last commit"
echo "git last - show last commit"

echo "🎉 Git đã được tối ưu hóa tối đa!"
