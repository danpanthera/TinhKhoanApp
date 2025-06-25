# Git Aliases cho TinhKhoanApp - Siêu nhanh commit
# Thêm vào ~/.zshrc hoặc ~/.bashrc

# Alias cơ bản
alias gs='git status --short'
alias ga='git add .'
alias gc='git commit -m'
alias gl='git log --oneline -10'
alias gp='git push'
alias gpu='git pull'

# Alias combo siêu nhanh
alias gac='git add . && git commit -m'
alias gacp='git add . && git commit -m "$1" && git push'

# Function commit nhanh với timestamp
qc() {
    if [ -z "$1" ]; then
        msg="🔄 Quick commit - $(date +"%H:%M:%S")"
    else
        msg="$1"
    fi
    git add . && git commit -m "$msg"
    echo "✅ Committed: $msg"
}

# Function commit và push
qcp() {
    if [ -z "$1" ]; then
        msg="🔄 Quick commit - $(date +"%H:%M:%S")"
    else
        msg="$1"
    fi
    git add . && git commit -m "$msg" && git push
    echo "✅ Committed and pushed: $msg"
}

# Function undo commit cuối
undo() {
    git reset --soft HEAD~1
    echo "↩️ Undid last commit"
}

# Function show git info nhanh
info() {
    echo "📊 Git Info:"
    echo "Branch: $(git branch --show-current)"
    echo "Status:"
    git status --short
    echo "Last commits:"
    git log --oneline -5
}

echo "🚀 Git aliases loaded! Sử dụng: qc, qcp, gs, ga, gc, undo, info"
