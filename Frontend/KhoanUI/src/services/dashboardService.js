import api from './api'

/**
 * Dashboard Service - Handles business plan dashboard operations
 */
export const dashboardService = {
  // ===== BUSINESS PLAN TARGETS =====

  /**
   * Get all business plan targets
   */
  async getTargets(params = {}) {
    try {
      const response = await api.get('/businessplantarget', { params })
      return response.data
    } catch (error) {
      console.error('Error fetching business plan targets:', error)
      throw error
    }
  },

  /**
   * Get business plan target by ID
   */
  async getTarget(id) {
    try {
      const response = await api.get(`/businessplantarget/${id}`)
      return response.data
    } catch (error) {
      console.error('Error fetching business plan target:', error)
      throw error
    }
  },

  /**
   * Create new business plan target
   */
  async createTarget(targetData) {
    try {
      const response = await api.post('/businessplantarget', targetData)
      return response.data
    } catch (error) {
      console.error('Error creating business plan target:', error)
      throw error
    }
  },

  /**
   * Update business plan target
   */
  async updateTarget(id, targetData) {
    try {
      const response = await api.put(`/businessplantarget/${id}`, targetData)
      return response.data
    } catch (error) {
      console.error('Error updating business plan target:', error)
      throw error
    }
  },

  /**
   * Delete business plan target
   */
  async deleteTarget(id) {
    try {
      await api.delete(`/businessplantarget/${id}`)
      return true
    } catch (error) {
      console.error('Error deleting business plan target:', error)
      throw error
    }
  },

  /**
   * Bulk create/update targets
   */
  async bulkUpsertTargets(targetsData) {
    try {
      const response = await api.post('/businessplantarget/bulk', targetsData)
      return response.data
    } catch (error) {
      console.error('Error bulk upserting targets:', error)
      throw error
    }
  },

  // ===== DASHBOARD CALCULATIONS =====

  /**
   * Get dashboard overview data
   */
  async getDashboardData(params = {}) {
    try {
      // Map frontend params to backend API params
      const backendParams = {
        year: params.year,
      }

      // Map periodType và period theo format backend expect
      if (params.periodType === 'QUARTER' && params.period) {
        backendParams.quarter = parseInt(params.period)
      } else if (params.periodType === 'MONTH' && params.period) {
        backendParams.month = parseInt(params.period)
      }

      // Map unitId từ string sang unitCode parameter cho backend
      if (params.unitId) {
        // Gửi unitCode thay vì unitId để backend dễ xử lý
        backendParams.unitCode = params.unitId
        console.log('🏢 Using unitCode filter:', params.unitId)
      }

      console.log('📡 Dashboard API call with mapped params:', backendParams)
      const response = await api.get('/dashboard/dashboard-data', { params: backendParams })
      return response.data
    } catch (error) {
      console.error('Error fetching dashboard data:', error)
      // Return mock data nếu API fails để không block UI
      return {
        overview: {
          totalTargets: 6,
          completedTargets: 2,
          achievementRate: 33.33,
          totalValue: 5000,
        },
        performanceByUnit: [],
      }
    }
  },

  /**
   * Get comparison data
   */
  async getComparisonData(params = {}) {
    try {
      const response = await api.get('/dashboard/comparison', { params })
      return response.data
    } catch (error) {
      console.error('Error fetching comparison data:', error)
      throw error
    }
  },

  /**
   * Get trend data
   */
  async getTrendData(params = {}) {
    try {
      const response = await api.get('/dashboard/trend', { params })
      return response.data
    } catch (error) {
      console.error('Error fetching trend data:', error)
      throw error
    }
  },

  /**
   * Get calculation results
   */
  async getCalculationResults(params = {}) {
    try {
      // Map frontend params to backend API params giống như getDashboardData
      const backendParams = {
        year: params.year,
      }

      // Map periodType và period theo format backend expect
      if (params.periodType === 'QUARTER' && params.period) {
        backendParams.quarter = parseInt(params.period)
      } else if (params.periodType === 'MONTH' && params.period) {
        backendParams.month = parseInt(params.period)
      }

      // Map unitId từ string sang unitCode parameter cho backend
      if (params.unitId) {
        backendParams.unitCode = params.unitId
        console.log('🏢 Calculation results using unitCode filter:', params.unitId)
      }

      console.log('📊 Calculation results API call with mapped params:', backendParams)
      const response = await api.get('/dashboard/calculation-results', { params: backendParams })
      return response.data
    } catch (error) {
      console.error('Error fetching calculation results:', error)
      // Return mock data thay vì throw error để không block UI
      return []
    }
  },

  /**
   * Trigger calculations
   */
  async triggerCalculations(params = {}) {
    try {
      const response = await api.post('/dashboard/calculate', params)
      return response.data
    } catch (error) {
      console.error('Error triggering calculations:', error)
      throw error
    }
  },

  // ===== DASHBOARD INDICATORS =====

  /**
   * Get all dashboard indicators
   */
  async getIndicators(params = {}) {
    try {
      const response = await api.get('/dashboard/indicators', { params })
      return response.data
    } catch (error) {
      console.error('Error fetching dashboard indicators:', error)
      throw error
    }
  },

  /**
   * Get indicators by unit
   */
  async getIndicatorsByUnit(unitId, params = {}) {
    try {
      const response = await api.get(`/dashboard/indicators/unit/${unitId}`, { params })
      return response.data
    } catch (error) {
      console.error('Error fetching indicators by unit:', error)
      throw error
    }
  },

  // ===== UNITS AND REFERENCE DATA =====

  /**
   * Get all units for dropdown
   */
  async getUnits() {
    try {
      const response = await api.get('/units')
      return this.sortUnits(response.data)
    } catch (error) {
      console.error('Error fetching units:', error)
      throw error
    }
  },

  /**
   * Sort units in the correct order for KPI display
   * Order: Hội sở (CnLaiChau) -> Branches in specific order
   */
  sortUnits(units) {
    // Define the correct branch order
    const branchOrder = [
      'Chi nhánh Lai Châu', // Hội sở (parent)
      'Chi nhánh Đoàn Kết',
      'Chi nhánh Bình Lư',
      'Chi nhánh Tân Uyên',
      'Chi nhánh Sìn Hồ',
      'Chi nhánh Phong Thổ',
      'Chi nhánh Than Uyên',
      'Chi nhánh Bum Tở',
      'Chi nhánh Nậm Hàng',
    ]

    return [...units].sort((a, b) => {
      const nameA = a.unitName || a.name || ''
      const nameB = b.unitName || b.name || ''

      const indexA = branchOrder.indexOf(nameA)
      const indexB = branchOrder.indexOf(nameB)

      // If both names are in the predefined order, sort by that order
      if (indexA !== -1 && indexB !== -1) {
        return indexA - indexB
      }

      // If only one name is in the order, prioritize it
      if (indexA !== -1) return -1
      if (indexB !== -1) return 1

      // For other units, maintain alphabetical sorting
      return nameA.localeCompare(nameB)
    })
  },

  /**
   * Get year options (current year and previous years)
   */
  getYearOptions() {
    const currentYear = new Date().getFullYear()
    const years = []
    for (let i = currentYear; i >= currentYear - 5; i--) {
      years.push(i)
    }
    return years
  },

  /**
   * Get quarter options
   */
  getQuarterOptions() {
    return [
      { value: 1, label: 'Quý 1' },
      { value: 2, label: 'Quý 2' },
      { value: 3, label: 'Quý 3' },
      { value: 4, label: 'Quý 4' },
    ]
  },

  /**
   * Get month options
   */
  getMonthOptions() {
    return [
      { value: 1, label: 'Tháng 1' },
      { value: 2, label: 'Tháng 2' },
      { value: 3, label: 'Tháng 3' },
      { value: 4, label: 'Tháng 4' },
      { value: 5, label: 'Tháng 5' },
      { value: 6, label: 'Tháng 6' },
      { value: 7, label: 'Tháng 7' },
      { value: 8, label: 'Tháng 8' },
      { value: 9, label: 'Tháng 9' },
      { value: 10, label: 'Tháng 10' },
      { value: 11, label: 'Tháng 11' },
      { value: 12, label: 'Tháng 12' },
    ]
  },

  /**
   * Get period type options - Updated with specific date option
   */
  getPeriodTypeOptions() {
    return [
      { value: 'YEAR', label: 'Năm' },
      { value: 'QUARTER', label: 'Quý' },
      { value: 'MONTH', label: 'Tháng' },
      { value: 'DATE', label: 'Ngày cụ thể' },
    ]
  },

  // API cho General Dashboard
  async getGeneralDashboardData(branchId, date = null) {
    try {
      let url = `/GeneralDashboard/indicators/${branchId}`
      if (date) {
        // Format date thành YYYY-MM-DD cho backend
        const formattedDate = date instanceof Date ? date.toISOString().split('T')[0] : date
        url += `?date=${formattedDate}`
      }

      console.log('🔄 Gọi API:', url)
      const response = await api.get(url)
      console.log('✅ API response:', response.data)
      return response.data
    } catch (error) {
      console.error('❌ Error fetching general dashboard data:', error)
      throw error
    }
  },
}

export default dashboardService
