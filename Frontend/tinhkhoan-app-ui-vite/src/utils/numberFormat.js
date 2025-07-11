// 🔢 utils/numberFormat.js - Utility cho định dạng số theo chuẩn US
// Quy ước: Dấu "," cho ngăn cách hàng nghìn, dấu "." cho thập phân (#,###.00)

/**
 * Format số thành chuỗi với định dạng US (#,###.00)
 * @param {number|string} value - Giá trị số cần format
 * @param {object} options - Tùy chọn format
 * @returns {string} Chuỗi đã format
 */
export const formatNumber = (value, options = {}) => {
  const {
    minimumFractionDigits = 0,
    maximumFractionDigits = 2,
    useGrouping = true
  } = options;

  if (!value && value !== 0) return '';

  const numValue = typeof value === 'string' ? parseFloat(value.replace(/,/g, '')) : Number(value);

  if (isNaN(numValue)) return '';

  // Sử dụng Intl.NumberFormat cho định dạng US với dấu phẩy ngăn cách
  return numValue.toLocaleString('en-US', {
    minimumFractionDigits,
    maximumFractionDigits,
    useGrouping
  });
};

/**
 * Format số tiền theo chuẩn US (#,###.00)
 * @param {number|string} value - Giá trị số cần format
 * @param {string} currency - Đơn vị tiền tệ (VND, triệu VND, triệu VND)
 * @returns {string} Chuỗi tiền tệ đã format
 */
export const formatCurrency = (value, currency = 'VND') => {
  const formatted = formatNumber(value, { maximumFractionDigits: 2 });
  return `${formatted} ${currency}`;
};

/**
 * Parse chuỗi đã format thành số
 * @param {string} formattedValue - Chuỗi đã format
 * @returns {number} Giá trị số
 */
export const parseFormattedNumber = (formattedValue) => {
  if (!formattedValue) return 0;

  // Loại bỏ tất cả dấu phẩy (ngăn cách hàng nghìn)
  const cleanValue = String(formattedValue).replace(/,/g, '');
  const numValue = parseFloat(cleanValue);

  return isNaN(numValue) ? 0 : numValue;
};

/**
 * Validate input chỉ cho phép số, dấu phẩy và dấu chấm
 * @param {string} value - Giá trị input
 * @returns {string} Giá trị đã clean
 */
export const sanitizeNumberInput = (value) => {
  if (!value) return '';

  // Chỉ cho phép: số (0-9), dấu phẩy (,), dấu chấm (.)
  return String(value).replace(/[^\d,.]/g, '');
};

/**
 * Format số trong khi nhập liệu (realtime)
 * @param {string} value - Giá trị đang nhập
 * @returns {string} Giá trị đã format
 */
export const formatNumberRealtime = (value) => {
  if (!value) return '';

  // Clean input
  const sanitized = sanitizeNumberInput(value);

  // Tách phần nguyên và phần thập phân
  const parts = sanitized.split('.');
  let integerPart = parts[0];
  let decimalPart = parts[1];

  // Loại bỏ dấu phẩy cũ và thêm dấu phẩy mới cho phần nguyên
  integerPart = integerPart.replace(/,/g, '');
  if (integerPart) {
    integerPart = parseInt(integerPart).toLocaleString('vi-VN');
  }

  // Kết hợp lại
  let result = integerPart;
  if (decimalPart !== undefined) {
    result += '.' + decimalPart;
  }

  return result;
};

/**
 * Tạo composable cho number input
 * @param {object} options - Tùy chọn
 * @returns {object} Methods và utilities
 */
export const useNumberInput = (options = {}) => {
  const {
    maxDecimalPlaces = 2,
    allowNegative = false
  } = options;

  const handleInput = (event, callback) => {
    const input = event.target;
    const cursorPosition = input.selectionStart;
    const oldValue = input.value;

    // Clean và format value
    let newValue = sanitizeNumberInput(input.value);

    // Kiểm tra số dấu chấm (chỉ cho phép 1)
    const dotCount = (newValue.match(/\./g) || []).length;
    if (dotCount > 1) {
      // Giữ lại dấu chấm đầu tiên
      const firstDotIndex = newValue.indexOf('.');
      newValue = newValue.substring(0, firstDotIndex + 1) +
                newValue.substring(firstDotIndex + 1).replace(/\./g, '');
    }

    // Giới hạn số chữ số thập phân
    const parts = newValue.split('.');
    if (parts[1] && parts[1].length > maxDecimalPlaces) {
      parts[1] = parts[1].substring(0, maxDecimalPlaces);
      newValue = parts.join('.');
    }

    // Xử lý số âm
    if (!allowNegative && newValue.startsWith('-')) {
      newValue = newValue.substring(1);
    }

    // Format với dấu phẩy
    const formattedValue = formatNumberRealtime(newValue);

    // Cập nhật input
    input.value = formattedValue;

    // Điều chỉnh cursor position
    const lengthDiff = formattedValue.length - oldValue.length;
    const newCursorPosition = cursorPosition + lengthDiff;
    setTimeout(() => {
      input.setSelectionRange(newCursorPosition, newCursorPosition);
    }, 0);

    // Callback với giá trị số
    if (callback) {
      callback(parseFormattedNumber(formattedValue), formattedValue);
    }
  };

  const handleBlur = (event, callback) => {
    const value = parseFormattedNumber(event.target.value);
    const formatted = formatNumber(value);
    event.target.value = formatted;

    if (callback) {
      callback(value, formatted);
    }
  };

  return {
    handleInput,
    handleBlur,
    formatNumber,
    parseFormattedNumber,
    sanitizeNumberInput,
    formatNumberRealtime
  };
};

// Export default cho dễ import
export default {
  formatNumber,
  formatCurrency,
  parseFormattedNumber,
  sanitizeNumberInput,
  formatNumberRealtime,
  useNumberInput
};
