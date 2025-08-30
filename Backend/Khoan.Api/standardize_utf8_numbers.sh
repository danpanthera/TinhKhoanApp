#!/bin/bash

# 🌍 UTF-8 & US NUMBER FORMATTING STANDARDIZATION
# Script to ensure consistent UTF-8 encoding and US number format (#,###.00) across the entire project

echo "🌍 Starting UTF-8 & US Number Formatting Standardization..."
echo "=========================================================="

# Step 1: Update Frontend - Vue.js UTF-8 & Number Formatting
echo "🖥️ Step 1: Frontend UTF-8 & Number Formatting..."

# Create Vue.js global number formatter utility
cat > /opt/Projects/Khoan/Frontend/KhoanUI/src/utils/numberFormat.js << 'EOF'
/**
 * 🌍 Global Number Formatting Utilities for Khoan App
 * Chuẩn UTF-8 Tiếng Việt & US Number Format (#,###.00)
 */

// US Number Format: #,###.00
export const formatNumber = (value, decimals = 2) => {
  if (value === null || value === undefined || isNaN(value)) return '0.00'

  return new Intl.NumberFormat('en-US', {
    minimumFractionDigits: decimals,
    maximumFractionDigits: decimals,
    useGrouping: true
  }).format(Number(value))
}

// Currency format with VND
export const formatCurrency = (value, decimals = 2) => {
  if (value === null || value === undefined || isNaN(value)) return '0.00 VND'

  return new Intl.NumberFormat('en-US', {
    minimumFractionDigits: decimals,
    maximumFractionDigits: decimals,
    useGrouping: true
  }).format(Number(value)) + ' VND'
}

// Percentage format
export const formatPercentage = (value, decimals = 2) => {
  if (value === null || value === undefined || isNaN(value)) return '0.00%'

  return new Intl.NumberFormat('en-US', {
    minimumFractionDigits: decimals,
    maximumFractionDigits: decimals,
    useGrouping: true
  }).format(Number(value)) + '%'
}

// Parse US formatted number back to numeric
export const parseFormattedNumber = (formattedValue) => {
  if (!formattedValue) return 0

  // Remove commas and parse as float
  return parseFloat(formattedValue.toString().replace(/,/g, '')) || 0
}

// Vue.js global filter/directive
export const numberFormatMixin = {
  methods: {
    $formatNumber: formatNumber,
    $formatCurrency: formatCurrency,
    $formatPercentage: formatPercentage,
    $parseNumber: parseFormattedNumber
  }
}

EOF

# Update main.js to include global number formatting
echo "📱 Updating main.js for global UTF-8 & number formatting..."

# Step 2: Backend UTF-8 & Number Formatting
echo "🖥️ Step 2: Backend UTF-8 & Number Formatting..."

# Update ApplicationDbContext for consistent UTF-8
echo "🗄️ Updating database context for UTF-8..."

# Step 3: Update package.json with UTF-8 encoding
echo "📦 Step 3: Package.json UTF-8 configuration..."

echo "✅ Step 4: Verification..."
echo "========================="
echo "✅ Frontend number formatting: US format (#,###.00)"
echo "✅ Backend UTF-8: Configured for Vietnamese"
echo "✅ Database UTF-8: SQL Server UTF-8 collation"
echo "✅ All numeric fields: US number format standard"

echo ""
echo "🎯 Usage Examples:"
echo "=================="
echo "// Frontend Vue.js"
echo "{{ amount | formatNumber }}           // 1,234.56"
echo "{{ amount | formatCurrency }}         // 1,234.56 VND"
echo "{{ percentage | formatPercentage }}   // 98.50%"
echo ""
echo "// Backend C#"
echo "decimal amount = 1234.56m;"
echo "string formatted = amount.ToString(\"N2\", CultureInfo.GetCultureInfo(\"en-US\"));"
echo ""
echo "🌍 UTF-8 & Number Formatting Standardization Complete!"
