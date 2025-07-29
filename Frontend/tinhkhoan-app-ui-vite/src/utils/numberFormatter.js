/**
 * Number Formatting Utilities
 * Quy ước thống nhất toàn dự án:
 * - Ngăn cách hàng nghìn: dấu phẩy (,)
 * - Dấu thập phân: dấu chấm (.)
 * VD: 1,234,567.89
 */

/**
 * Format số theo quy ước dự án: ngăn cách hàng nghìn bằng dấu phẩy, thập phân bằng dấu chấm
 * @param {number|string} value - Giá trị số cần format
 * @param {number} decimals - Số chữ số thập phân (mặc định 2)
 * @param {boolean} showZeroDecimals - Có hiển thị phần thập phân khi là 0 không (mặc định false)
 * @returns {string} Chuỗi số đã format
 */
export function formatNumber(value, decimals = 2, showZeroDecimals = false) {
  if (value === null || value === undefined || value === '') {
    return '0'
  }

  const numValue = parseFloat(value)
  if (isNaN(numValue)) {
    return '0'
  }

  // Format số với Intl.NumberFormat để đảm bảo consistency
  const formatter = new Intl.NumberFormat('en-US', {
    minimumFractionDigits: showZeroDecimals ? decimals : 0,
    maximumFractionDigits: decimals,
    useGrouping: true, // Sử dụng dấu phẩy ngăn cách hàng nghìn
  })

  return formatter.format(numValue)
}

/**
 * Format số tiền (currency) với prefix và suffix
 * @param {number|string} value - Giá trị số tiền
 * @param {string} currency - Đơn vị tiền tệ (VD: 'VND', 'USD', 'MILLION_VND')
 * @param {number} decimals - Số chữ số thập phân
 * @returns {string} Chuỗi tiền đã format
 */
export function formatCurrency(value, currency = 'VND', decimals = 0) {
  // Đặc biệt cho Triệu VND: luôn hiển thị 2 chữ số thập phân
  if (currency === 'MILLION_VND') {
    const formatted = formatNumber(value, 2, true) // Force show decimals
    return `${formatted} tr.VND`
  }

  const formatted = formatNumber(value, decimals, false)

  if (currency === 'VND') {
    return `${formatted} đ`
  } else if (currency === 'USD') {
    return `$${formatted}`
  } else {
    return `${formatted} ${currency}`
  }
}

/**
 * Format phần trăm với ký hiệu %
 * @param {number|string} value - Giá trị phần trăm (VD: 0.15 -> 15%)
 * @param {number} decimals - Số chữ số thập phân
 * @param {boolean} isAlreadyPercent - Giá trị đã là % chưa (VD: 15 thay vì 0.15)
 * @returns {string} Chuỗi % đã format
 */
export function formatPercent(value, decimals = 2, isAlreadyPercent = false) {
  const numValue = parseFloat(value)
  if (isNaN(numValue)) {
    return '0%'
  }

  const percentValue = isAlreadyPercent ? numValue : numValue * 100
  return `${formatNumber(percentValue, decimals)}%`
}

/**
 * Parse chuỗi số có format dấu phẩy thành number
 * @param {string} formattedValue - Chuỗi số đã format (VD: "1,234.56")
 * @returns {number} Giá trị số
 */
export function parseFormattedNumber(formattedValue) {
  if (typeof formattedValue !== 'string') {
    return parseFloat(formattedValue) || 0
  }

  // Loại bỏ tất cả dấu phẩy ngăn cách hàng nghìn
  const cleanedValue = formattedValue.replace(/,/g, '').trim()

  // Parse thành number
  const result = parseFloat(cleanedValue)
  return isNaN(result) ? 0 : result
}

/**
 * Validate input số theo format dự án
 * @param {string} value - Giá trị input
 * @returns {boolean} True nếu format hợp lệ
 */
export function validateNumberFormat(value) {
  if (!value || typeof value !== 'string') return false

  // Regex pattern cho format: 1,234,567.89
  // Cho phép: số không, số nguyên, số thập phân với dấu phẩy ngăn cách
  const pattern = /^-?\d{1,3}(,\d{3})*(\.\d+)?$|^-?\d+(\.\d+)?$/
  return pattern.test(value.trim())
}

/**
 * Format input number real-time khi user typing
 * @param {string} value - Giá trị user đang gõ
 * @param {number} decimals - Số chữ số thập phân tối đa
 * @returns {string} Giá trị đã format
 */
export function formatInputNumber(value, decimals = 2) {
  if (!value) return ''

  // Loại bỏ ký tự không hợp lệ, chỉ giữ số, dấu phẩy, dấu chấm, dấu trừ
  let cleaned = value.replace(/[^0-9.,-]/g, '')

  // Đảm bảo chỉ có một dấu chấm thập phân
  const dotIndex = cleaned.indexOf('.')
  if (dotIndex !== -1) {
    const beforeDot = cleaned.substring(0, dotIndex)
    const afterDot = cleaned.substring(dotIndex + 1).replace(/\./g, '') // Loại bỏ dấu chấm thừa
    cleaned = beforeDot + '.' + afterDot.substring(0, decimals) // Giới hạn số chữ số thập phân
  }

  // Parse và format lại
  try {
    const parts = cleaned.split('.')
    const integerPart = parts[0].replace(/,/g, '') // Loại bỏ dấu phẩy cũ

    if (integerPart === '') return ''

    // Format lại phần nguyên với dấu phẩy
    const formattedInteger = parseInt(integerPart).toLocaleString('en-US')

    // Kết hợp phần nguyên và thập phân
    if (parts.length > 1) {
      return formattedInteger + '.' + parts[1]
    } else {
      return formattedInteger
    }
  } catch (error) {
    return cleaned
  }
}

/**
 * Format file size theo đơn vị KB, MB, GB
 * @param {number} bytes - Số bytes
 * @returns {string} Kích thước file đã format
 */
export function formatFileSize(bytes) {
  if (bytes === 0) return '0 B'

  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB', 'TB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))

  return `${formatNumber(bytes / Math.pow(k, i), 2)} ${sizes[i]}`
}

/**
 * Format thời gian processing
 * @param {number} seconds - Số giây
 * @returns {string} Thời gian đã format
 */
export function formatProcessingTime(seconds) {
  if (seconds < 1) {
    return `${formatNumber(seconds * 1000, 0)}ms`
  } else if (seconds < 60) {
    return `${formatNumber(seconds, 2)}s`
  } else {
    const minutes = Math.floor(seconds / 60)
    const remainingSeconds = seconds % 60
    return `${minutes}m ${formatNumber(remainingSeconds, 0)}s`
  }
}

/**
 * Format số tiền Triệu VND với 2 chữ số thập phân
 * @param {number|string} value - Giá trị tiền tệ
 * @returns {string} Chuỗi tiền Triệu VND đã format
 */
export function formatMillionVND(value) {
  return formatCurrency(value, 'MILLION_VND', 2)
}

/**
 * Format input cho trường Triệu VND
 * @param {string} value - Giá trị input
 * @returns {string} Giá trị đã format
 */
export function formatMillionVNDInput(value) {
  return formatInputNumber(value, 2) // Luôn hiển thị 2 chữ số thập phân
}

// Default export cho convenience
export default {
  formatNumber,
  formatCurrency,
  formatPercent,
  parseFormattedNumber,
  validateNumberFormat,
  formatInputNumber,
  formatFileSize,
  formatProcessingTime,
  formatMillionVND,
  formatMillionVNDInput,
}
