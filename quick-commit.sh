#!/bin/bash
# Script tự động commit nhanh cho KhoanApp
# Sử dụng: ./quick-commit.sh "commit message"

# Màu sắc cho output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function để log với màu
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Kiểm tra có message không
if [ -z "$1" ]; then
    echo "Sử dụng: ./quick-commit.sh \"commit message\""
    echo "Hoặc: ./quick-commit.sh auto (để tự động tạo message)"
    exit 1
fi

# Chuyển vào thư mục project
cd "$(dirname "$0")"

log_info "🚀 Bắt đầu quá trình commit nhanh..."

# Kiểm tra status
log_info "📊 Kiểm tra trạng thái repository..."
git status --porcelain > /tmp/git_status.txt

if [ ! -s /tmp/git_status.txt ]; then
    log_warning "⚠️  Không có thay đổi nào để commit!"
    exit 0
fi

# Hiển thị files sẽ được commit
log_info "📁 Files sẽ được commit:"
cat /tmp/git_status.txt

# Add tất cả files
log_info "➕ Đang add tất cả files..."
git add .

# Tạo commit message tự động nếu cần
if [ "$1" = "auto" ]; then
    # Đếm số files được thay đổi
    modified_count=$(git status --porcelain | grep "^M" | wc -l | tr -d ' ')
    added_count=$(git status --porcelain | grep "^A" | wc -l | tr -d ' ')
    deleted_count=$(git status --porcelain | grep "^D" | wc -l | tr -d ' ')
    
    # Tạo message tự động
    timestamp=$(date +"%Y-%m-%d %H:%M:%S")
    commit_message="🔄 Auto commit - $timestamp"
    
    if [ $modified_count -gt 0 ]; then
        commit_message="$commit_message | Modified: $modified_count files"
    fi
    
    if [ $added_count -gt 0 ]; then
        commit_message="$commit_message | Added: $added_count files"
    fi
    
    if [ $deleted_count -gt 0 ]; then
        commit_message="$commit_message | Deleted: $deleted_count files"
    fi
else
    commit_message="$1"
fi

# Commit
log_info "💾 Đang commit với message: \"$commit_message\""
if git commit -m "$commit_message"; then
    log_success "✅ Commit thành công!"
    
    # Hiển thị commit cuối cùng
    log_info "📜 Commit cuối cùng:"
    git log --oneline -1
    
    # Hiển thị branch hiện tại
    current_branch=$(git branch --show-current)
    log_info "🌿 Branch hiện tại: $current_branch"
    
    # Hỏi có muốn push không (chỉ hiển thị, không tự động push theo yêu cầu)
    log_warning "⚡ Để push, sử dụng: git push origin $current_branch"
    
else
    log_error "❌ Commit thất bại!"
    exit 1
fi

log_success "🎉 Hoàn thành quá trình commit nhanh!"
