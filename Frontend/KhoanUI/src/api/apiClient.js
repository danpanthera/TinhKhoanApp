/**
 * 🚀 API Client Configuration
 * Axios-based API client for Khoan application
 */

import axios from 'axios'

// API Base URL - adjust according to your backend
const API_BASE_URL = 'http://localhost:5056'

// Create axios instance with default config
export const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 30000, // 30 seconds timeout for file uploads
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor for logging
apiClient.interceptors.request.use(
  (config) => {
    console.log(`🚀 API Request: ${config.method?.toUpperCase()} ${config.url}`)
    return config
  },
  (error) => {
    console.error('❌ Request Error:', error)
    return Promise.reject(error)
  },
)

// Response interceptor for error handling
apiClient.interceptors.response.use(
  (response) => {
    console.log(`✅ API Response: ${response.status} ${response.config.url}`)
    return response
  },
  (error) => {
    console.error('❌ API Error:', error.response?.status, error.response?.data || error.message)
    return Promise.reject(error)
  },
)

export default apiClient
