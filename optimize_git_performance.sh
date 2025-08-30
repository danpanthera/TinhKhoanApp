#!/bin/bash

# ===================================================
# KhoanApp Git Performance Optimization Script
# Tối ưu hóa Git để commit nhanh nhất có thể
# ===================================================

echo "🚀 Starting Git Performance Optimization..."

# 1. Navigate to project root
cd /opt/Projects/Khoan

# 2. Git Performance Settings
echo "⚙️ Configuring Git performance settings..."
git config core.preloadindex true
git config core.fscache true
git config core.untrackedCache true
git config gc.auto 256
git config core.commitGraph true
git config gc.writeCommitGraph true

# 3. Optimize for large repositories
echo "📈 Optimizing for large repository..."
git config pack.threads 0  # Use all available cores
git config pack.deltaCacheSize 2147483648  # 2GB delta cache
git config core.packedGitLimit 512m
git config core.packedGitWindowSize 512m

# 4. Enable partial clone if repository is large
echo "🌐 Enabling advanced Git features..."
git config fetch.writeCommitGraph true
git config feature.manyFiles true

# 5. Clean up unnecessary files
echo "🧹 Cleaning up workspace..."

# Remove node_modules if exists (should be in .gitignore)
find . -name "node_modules" -type d -prune -exec rm -rf {} \; 2>/dev/null || true

# Remove bin/obj folders (should be in .gitignore)
find . -path "*/bin/*" -delete 2>/dev/null || true
find . -path "*/obj/*" -delete 2>/dev/null || true

# Remove log files
find . -name "*.log" -delete 2>/dev/null || true

# Remove temporary files
find . -name "*.tmp" -delete 2>/dev/null || true
find . -name "*.temp" -delete 2>/dev/null || true

# 6. Git garbage collection (aggressive)
echo "🗑️ Running aggressive garbage collection..."
git reflog expire --expire=now --all
git gc --aggressive --prune=now

# 7. Update .gitignore to ensure we don't track unnecessary files
echo "📝 Updating .gitignore..."
cat >> .gitignore << 'EOL'

# Performance optimization - exclude large/temp files
*.log
*.tmp
*.temp
*.cache
.DS_Store
Thumbs.db

# Build artifacts
**/bin/
**/obj/
**/dist/
**/build/

# IDE files
.vscode/settings.json
.vs/
*.user
*.suo

# Node.js
**/node_modules/
npm-debug.log*
yarn-debug.log*

# Database files (if any)
*.db-shm
*.db-wal

EOL

# 8. Check final repository size
echo "📊 Final repository statistics:"
echo "Repository size: $(du -sh .git | cut -f1)"
echo "Working directory size: $(du -sh --exclude=.git . | cut -f1)"

# 9. Verify Git performance settings
echo "✅ Current Git performance settings:"
git config --list | grep -E "(core\.|gc\.|pack\.|fetch\.)"

echo ""
echo "🎉 Git Performance Optimization Completed!"
echo ""
echo "📈 Recommended workflow for fastest commits:"
echo "1. git add -A"
echo "2. git commit -m 'message'"
echo "3. Avoid 'git status' in large repositories"
echo "4. Use 'git diff --name-only' instead of full diff"
echo "5. Consider using git aliases: git config alias.ac '!git add -A && git commit -m'"
echo ""

# 10. Create useful Git aliases for speed
echo "⚡ Creating speed aliases..."
git config alias.ac '!git add -A && git commit -m'
git config alias.s 'status --porcelain'
git config alias.co 'checkout'
git config alias.br 'branch'
git config alias.cm 'commit -m'

echo "✅ Speed aliases created:"
echo "  git ac 'message' - Add all and commit"
echo "  git s - Short status"
echo "  git co - Checkout"
echo "  git br - Branch"
echo "  git cm - Commit with message"

echo ""
echo "🚀 Git optimization complete! Commits should now be significantly faster."
