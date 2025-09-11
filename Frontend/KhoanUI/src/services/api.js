import axios from 'axios'

const baseURL = import.meta.env.DEV ? '/api' : (import.meta.env.VITE_API_BASE_URL || '/api')
const apiClient = axios.create({
  // In development, always go through Vite proxy to avoid CORS; prod can override via VITE_API_BASE_URL
  baseURL,
  headers: {
    'Content-Type': 'application/json',
    Accept: 'application/json',
  },
  timeout: 60000, // Increased timeout to 60 seconds for large Excel imports
  validateStatus: function (status) {
    return status >= 200 && status < 300 // chỉ chấp nhận status 2xx
  },
})

// Add request interceptor - Thêm interceptor cho request
apiClient.interceptors.request.use(
  config => {
    // Thêm token vào header nếu có
    const token = localStorage.getItem('token')
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`
    }
    return config
  },
  error => {
    return Promise.reject(error)
  },
)

// Add response interceptor - Thêm interceptor cho response
apiClient.interceptors.response.use(
  response => {
    // 🔧 Recursive function để xử lý .NET $values format
    function convertDotNetFormat(obj) {
      if (obj && typeof obj === 'object') {
        // Xử lý object có $values property
        if (obj.$values && Array.isArray(obj.$values)) {
          console.log('🔧 API: Converting .NET $values format to array')
          return obj.$values.map(item => convertDotNetFormat(item))
        }

        // Xử lý object thông thường - convert từng property
        if (Array.isArray(obj)) {
          return obj.map(item => convertDotNetFormat(item))
        }

        const converted = {}
        for (const [key, value] of Object.entries(obj)) {
          converted[key] = convertDotNetFormat(value)
        }
        return converted
      }

      return obj
    }

    // Áp dụng conversion cho toàn bộ response data
    response.data = convertDotNetFormat(response.data)
    return response
  },
  error => {
    if (error.response) {
      // Server responded with a status code outside of 2xx range
      console.error(`[API Error] ${error.response.status}:`, error.response.data)
    } else if (error.request) {
      // Request was made but no response received
      console.error('[API No Response]', 'Server không phản hồi. Vui lòng kiểm tra kết nối.')
    } else {
      // Something happened in setting up the request
      console.error('[API Setup Error]', error.message)
    }
    return Promise.reject(error)
  },
)

export default apiClient

// API functions cho KPI Definitions - ĐÃ ĐƯỢC DỌN SẠCH
export const kpiDefinitionsApi = {
  // TẠM THỜI VÔ HIỆU HÓA - Đang dọn sạch API cũ liên quan đến CBType
  // Sẽ được thay thế bằng API mới cho 23 vai trò chuẩn

  /*
  // CÁC API CŨ ĐÃ ĐƯỢC DỌN SẠCH:
  // - getCBTypes: Lấy danh sách các loại cán bộ (CB Types)
  // - getKPIsByCBType: Lấy KPI definitions theo CB type
  // - resetKPIsByCBType: Reset KPI theo CB type
  //
  // Sẽ được thay thế bằng:
  // - getStandardRoles: Lấy danh sách 23 vai trò chuẩn
  // - getKPIsByRole: Lấy KPI theo vai trò mới
  // - resetKPIsByRole: Reset KPI theo vai trò mới
  */

  // Placeholder methods - sẽ được cài đặt lại với logic mới
  getCBTypes: () =>
    Promise.resolve({
      data: [
        {
          Message: 'API đang được cập nhật với dữ liệu mới cho 23 vai trò chuẩn',
          Status: 'Under maintenance',
        },
      ],
    }),

  getKPIsByCBType: _cbType => Promise.reject(new Error('API tạm thời không khả dụng - đang cập nhật dữ liệu mới')),

  resetKPIsByCBType: _cbType => Promise.reject(new Error('API tạm thời không khả dụng - đang cập nhật dữ liệu mới')),

  // 🚀 Export APIs
  export: {
    // Xuất danh sách giao khoán KPI
    kpiAssignments: (periodId, format = 'excel') => {
      const url = `/Export/kpi-assignments?${periodId ? `periodId=${periodId}&` : ''}format=${format}`
      return apiClient.get(url, { responseType: 'blob' })
    },

    // Xuất danh sách nhân viên
    employees: (unitId, format = 'excel') => {
      const url = `/Export/employees?${unitId ? `unitId=${unitId}&` : ''}format=${format}`
      return apiClient.get(url, { responseType: 'blob' })
    },

    // Xuất báo cáo tổng hợp KPI
    kpiSummary: periodId => {
      const url = `/Export/kpi-summary?${periodId ? `periodId=${periodId}` : ''}`
      return apiClient.get(url, { responseType: 'blob' })
    },
  },

  // ⚡ Bulk Operations APIs
  bulk: {
    // Bulk assign KPIs to multiple employees
    assignKPIs: bulkAssignmentData => {
      console.log('🔧 API Service: Bulk assign KPIs:', bulkAssignmentData)
      return apiClient.post('/EmployeeKpiAssignment/bulk-assign', bulkAssignmentData)
    },

    // Bulk update scores for multiple assignments
    updateScores: bulkUpdateData => {
      console.log('🔧 API Service: Bulk update scores:', bulkUpdateData)
      return apiClient.put('/EmployeeKpiAssignment/bulk-update-scores', bulkUpdateData)
    },

    // Bulk delete assignments
    deleteAssignments: bulkDeleteData => {
      console.log('🔧 API Service: Bulk delete assignments:', bulkDeleteData)
      return apiClient.delete('/EmployeeKpiAssignment/bulk-delete', { data: bulkDeleteData })
    },
  },
}
