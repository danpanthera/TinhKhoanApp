/**
 * Helper functions để xử lý .NET API response format
 */

/**
 * Xử lý .NET $values array format
 * @param {Object|Array} data - Data từ .NET API
 * @returns {Array} - Array đã được normalize
 */
export function normalizeNetArray(data) {
  if (!data) return []
  
  // Nếu là array thì return luôn
  if (Array.isArray(data)) return data
  
  // Nếu có $values thì return $values
  if (data.$values && Array.isArray(data.$values)) {
    return data.$values
  }
  
  // Fallback return empty array
  return []
}

/**
 * Xử lý .NET response với $id và $values
 * @param {Object} response - Response từ .NET API
 * @returns {Object} - Object đã được normalize
 */
export function normalizeNetResponse(response) {
  if (!response || !response.data) return null
  
  const data = { ...response.data }
  
  // Process all properties that might have $values
  Object.keys(data).forEach(key => {
    if (data[key] && data[key].$values) {
      data[key] = data[key].$values
    }
  })
  
  return { ...response, data }
}

/**
 * Log API response cho debugging
 * @param {string} endpoint - API endpoint
 * @param {Object} response - API response
 * @param {string} dataKey - Key chứa data cần check (e.g., 'indicators')
 */
export function logApiResponse(endpoint, response, dataKey = null) {
  console.group(`📡 API Response: ${endpoint}`)
  console.log('Status:', response.status)
  console.log('Has data:', !!response.data)
  
  if (response.data) {
    console.log('Data keys:', Object.keys(response.data))
    
    if (dataKey && response.data[dataKey]) {
      const data = response.data[dataKey]
      console.log(`${dataKey} type:`, typeof data)
      console.log(`${dataKey} is array:`, Array.isArray(data))
      console.log(`${dataKey} has $values:`, !!(data.$values))
      
      if (data.$values) {
        console.log(`${dataKey}.$values length:`, data.$values.length)
      } else if (Array.isArray(data)) {
        console.log(`${dataKey} length:`, data.length)
      }
    }
  }
  
  console.groupEnd()
}
