/**
 * 🌍 Global Number Formatting Utilities for TinhKhoan App
 * Chuẩn UTF-8 Tiếng Việt & US Number Format (#,###.00)
 */

// US Number Format: #,###.00
export const formatNumber = (value, decimals = 2) => {
  if (value === null || value === undefined || isNaN(value)) return '0.00'

  return new Intl.NumberFormat('en-US', {
    minimumFractionDigits: decimals,
    maximumFractionDigits: decimals,
    useGrouping: true,
  }).format(Number(value))
}

// Currency format with VND
export const formatCurrency = (value, decimals = 2) => {
  if (value === null || value === undefined || isNaN(value)) return '0.00 VND'

  return (
    new Intl.NumberFormat('en-US', {
      minimumFractionDigits: decimals,
      maximumFractionDigits: decimals,
      useGrouping: true,
    }).format(Number(value)) + ' VND'
  )
}

// Percentage format
export const formatPercentage = (value, decimals = 2) => {
  if (value === null || value === undefined || isNaN(value)) return '0.00%'

  return (
    new Intl.NumberFormat('en-US', {
      minimumFractionDigits: decimals,
      maximumFractionDigits: decimals,
      useGrouping: true,
    }).format(Number(value)) + '%'
  )
}

// Parse US formatted number back to numeric
export const parseFormattedNumber = formattedValue => {
  if (!formattedValue) return 0

  // Remove commas and parse as float
  return parseFloat(formattedValue.toString().replace(/,/g, '')) || 0
}

// Composable for number input formatting
export const useNumberInput = () => {
  const formatInput = value => formatNumber(value)
  const parseInput = value => parseFormattedNumber(value)

  return {
    formatInput,
    parseInput,
    formatNumber,
    formatCurrency,
    formatPercentage,
    parseFormattedNumber,
  }
}

// Vue.js global filter/directive
export const numberFormatMixin = {
  methods: {
    $formatNumber: formatNumber,
    $formatCurrency: formatCurrency,
    $formatPercentage: formatPercentage,
    $parseNumber: parseFormattedNumber,
  },
}
