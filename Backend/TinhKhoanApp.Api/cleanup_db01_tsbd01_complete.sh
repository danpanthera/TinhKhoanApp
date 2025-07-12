#!/bin/bash

# 🗑️ CLEANUP SCRIPT: Xóa toàn bộ DB01 và TSBD01 khỏi dự án
# Ngày: $(date '+%d/%m/%Y %H:%M:%S')
# Mục đích: Dọn dẹp hoàn toàn tất cả references đến DB01 và TSBD01

echo "🚀 Bắt đầu cleanup toàn bộ DB01 và TSBD01..."

# =============================================================================
# PHASE 1: XÓA MODEL FILES
# =============================================================================
echo "📁 PHASE 1: Xóa các model files..."

# Xóa model files nếu tồn tại
if [ -f "Models/DataTables/DB01.cs" ]; then
    rm "Models/DataTables/DB01.cs"
    echo "✅ Đã xóa Models/DataTables/DB01.cs"
fi

if [ -f "Models/DataTables/TSBD01.cs" ]; then
    rm "Models/DataTables/TSBD01.cs"
    echo "✅ Đã xóa Models/DataTables/TSBD01.cs"
fi

# =============================================================================
# PHASE 2: GREP VÀ LIỆT KÊ TẤT CẢ FILE CHỨA DB01 HOẶC TSBD01
# =============================================================================
echo "🔍 PHASE 2: Tìm kiếm tất cả references..."

echo "📋 Files chứa DB01:"
find . -type f \( -name "*.cs" -o -name "*.js" -o -name "*.ts" -o -name "*.vue" -o -name "*.sql" -o -name "*.md" -o -name "*.json" -o -name "*.sh" \) ! -path "./node_modules/*" ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" -exec grep -l "DB01" {} \; 2>/dev/null | sort

echo ""
echo "📋 Files chứa TSBD01:"
find . -type f \( -name "*.cs" -o -name "*.js" -o -name "*.ts" -o -name "*.vue" -o -name "*.sql" -o -name "*.md" -o -name "*.json" -o -name "*.sh" \) ! -path "./node_modules/*" ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" -exec grep -l "TSBD01" {} \; 2>/dev/null | sort

echo ""
echo "📋 Files chứa TSDB01:"
find . -type f \( -name "*.cs" -o -name "*.js" -o -name "*.ts" -o -name "*.vue" -o -name "*.sql" -o -name "*.md" -o -name "*.json" -o -name "*.sh" \) ! -path "./node_modules/*" ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" -exec grep -l "TSDB01" {} \; 2>/dev/null | sort

# =============================================================================
# PHASE 3: COUNT REFERENCES
# =============================================================================
echo ""
echo "📊 PHASE 3: Đếm số lượng references..."

DB01_COUNT=$(find . -type f \( -name "*.cs" -o -name "*.js" -o -name "*.ts" -o -name "*.vue" -o -name "*.sql" -o -name "*.md" -o -name "*.json" -o -name "*.sh" \) ! -path "./node_modules/*" ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" -exec grep -l "DB01" {} \; 2>/dev/null | wc -l)

TSBD01_COUNT=$(find . -type f \( -name "*.cs" -o -name "*.js" -o -name "*.ts" -o -name "*.vue" -o -name "*.sql" -o -name "*.md" -o -name "*.json" -o -name "*.sh" \) ! -path "./node_modules/*" ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" -exec grep -l "TSBD01" {} \; 2>/dev/null | wc -l)

TSDB01_COUNT=$(find . -type f \( -name "*.cs" -o -name "*.js" -o -name "*.ts" -o -name "*.vue" -o -name "*.sql" -o -name "*.md" -o -name "*.json" -o -name "*.sh" \) ! -path "./node_modules/*" ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" -exec grep -l "TSDB01" {} \; 2>/dev/null | wc -l)

echo "📈 Tổng kết:"
echo "  - DB01: $DB01_COUNT files"
echo "  - TSBD01: $TSBD01_COUNT files"
echo "  - TSDB01: $TSDB01_COUNT files"
echo "  - TỔNG: $((DB01_COUNT + TSBD01_COUNT + TSDB01_COUNT)) files cần cleanup"

# =============================================================================
# PHASE 4: SAVE CLEANUP REPORT
# =============================================================================
echo ""
echo "📄 PHASE 4: Tạo báo cáo cleanup..."

cat > DB01_TSBD01_CLEANUP_REPORT.md << 'EOF'
# 🗑️ DB01 & TSBD01 CLEANUP REPORT

**Ngày tạo:** $(date '+%d/%m/%Y %H:%M:%S')
**Mục đích:** Rà soát và xóa toàn bộ mọi thứ liên quan đến DB01, TSBD01, TSDB01

## 📊 TỔNG QUAN CLEANUP

### Files cần xử lý:
EOF

echo "### 🔍 DB01 References:" >> DB01_TSBD01_CLEANUP_REPORT.md
find . -type f \( -name "*.cs" -o -name "*.js" -o -name "*.ts" -o -name "*.vue" -o -name "*.sql" -o -name "*.md" -o -name "*.json" -o -name "*.sh" \) ! -path "./node_modules/*" ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" -exec grep -l "DB01" {} \; 2>/dev/null | sed 's/^/- /' >> DB01_TSBD01_CLEANUP_REPORT.md

echo "" >> DB01_TSBD01_CLEANUP_REPORT.md
echo "### 🔍 TSBD01 References:" >> DB01_TSBD01_CLEANUP_REPORT.md
find . -type f \( -name "*.cs" -o -name "*.js" -o -name "*.ts" -o -name "*.vue" -o -name "*.sql" -o -name "*.md" -o -name "*.json" -o -name "*.sh" \) ! -path "./node_modules/*" ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" -exec grep -l "TSBD01" {} \; 2>/dev/null | sed 's/^/- /' >> DB01_TSBD01_CLEANUP_REPORT.md

echo "" >> DB01_TSBD01_CLEANUP_REPORT.md
echo "### 🔍 TSDB01 References:" >> DB01_TSBD01_CLEANUP_REPORT.md
find . -type f \( -name "*.cs" -o -name "*.js" -o -name "*.ts" -o -name "*.vue" -o -name "*.sql" -o -name "*.md" -o -name "*.json" -o -name "*.sh" \) ! -path "./node_modules/*" ! -path "./.git/*" ! -path "./bin/*" ! -path "./obj/*" -exec grep -l "TSDB01" {} \; 2>/dev/null | sed 's/^/- /' >> DB01_TSBD01_CLEANUP_REPORT.md

echo "" >> DB01_TSBD01_CLEANUP_REPORT.md
echo "## 📋 NEXT STEPS" >> DB01_TSBD01_CLEANUP_REPORT.md
echo "1. Review từng file trong danh sách trên" >> DB01_TSBD01_CLEANUP_REPORT.md
echo "2. Xóa hoặc thay thế các references" >> DB01_TSBD01_CLEANUP_REPORT.md
echo "3. Test build và verify không còn lỗi" >> DB01_TSBD01_CLEANUP_REPORT.md
echo "4. Commit changes theo từng phase nhỏ" >> DB01_TSBD01_CLEANUP_REPORT.md

echo "✅ Đã tạo báo cáo: DB01_TSBD01_CLEANUP_REPORT.md"

echo ""
echo "🎯 HOÀN THÀNH! Xem báo cáo chi tiết trong DB01_TSBD01_CLEANUP_REPORT.md"
echo "⚠️  Cần thực hiện manual cleanup các files được liệt kê trong báo cáo."
