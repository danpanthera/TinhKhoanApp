import dayjs from 'dayjs'

/**
 * Format a date string or Date object to Vietnamese format
 * @param {string|Date} date - Date to format
 * @param {string} format - Format string (default: 'DD/MM/YYYY')
 * @returns {string} Formatted date string
 */
export const formatDate = (date, format = 'DD/MM/YYYY') => {
  if (!date) return 'N/A'
  return dayjs(date).format(format)
}

/**
 * Format a datetime string to Vietnamese format with time
 * @param {string|Date} datetime - DateTime to format
 * @param {string} format - Format string (default: 'DD/MM/YYYY HH:mm:ss')
 * @returns {string} Formatted datetime string
 */
export const formatDateTime = (datetime, format = 'DD/MM/YYYY HH:mm:ss') => {
  if (!datetime) return 'N/A'
  return dayjs(datetime).format(format)
}

/**
 * Format a date for API requests (ISO format)
 * @param {string|Date} date - Date to format
 * @returns {string} ISO formatted date string
 */
export const formatDateForApi = (date) => {
  if (!date) return null
  return dayjs(date).format('YYYY-MM-DD')
}

/**
 * Parse a date string to Date object
 * @param {string} dateString - Date string to parse
 * @returns {Date|null} Parsed Date object or null
 */
export const parseDate = (dateString) => {
  if (!dateString) return null
  const parsed = dayjs(dateString)
  return parsed.isValid() ? parsed.toDate() : null
}

/**
 * Get relative time (e.g., "2 hours ago")
 * @param {string|Date} date - Date to compare
 * @returns {string} Relative time string
 */
export const getRelativeTime = (date) => {
  if (!date) return 'N/A'
  return dayjs(date).fromNow()
}

/**
 * Check if a date is today
 * @param {string|Date} date - Date to check
 * @returns {boolean} True if date is today
 */
export const isToday = (date) => {
  if (!date) return false
  return dayjs(date).isSame(dayjs(), 'day')
}

/**
 * Check if a date is within a range
 * @param {string|Date} date - Date to check
 * @param {string|Date} startDate - Range start date
 * @param {string|Date} endDate - Range end date
 * @returns {boolean} True if date is within range
 */
export const isDateInRange = (date, startDate, endDate) => {
  if (!date || !startDate || !endDate) return false
  const checkDate = dayjs(date)
  return checkDate.isAfter(dayjs(startDate)) && checkDate.isBefore(dayjs(endDate))
}
