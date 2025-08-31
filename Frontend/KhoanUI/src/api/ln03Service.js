import { apiClient } from './apiClient'

/**
 * 🏦 LN03 Loan Data Service
 * Comprehensive API service for LN03 temporal table operations
 * Supports full CRUD, temporal queries, analytics, and CSV import
 */
export const ln03Service = {
  // ===== BASIC DATA OPERATIONS =====

  /**
   * Get total count of LN03 records
   */
  async getCount(params = {}) {
    try {
      const response = await apiClient.get('/api/LN03/count', { params })
      return response.data
    } catch (error) {
      console.error('❌ LN03 Count Error:', error)
      throw error
    }
  },

  /**
   * Get paginated LN03 records
   */
  async getRecords(params = {}) {
    try {
      const response = await apiClient.get('/api/LN03', { params })
      return response.data
    } catch (error) {
      console.error('❌ LN03 Records Error:', error)
      throw error
    }
  },

  /**
   * Get single LN03 record by ID
   */
  async getById(id) {
    try {
      const response = await apiClient.get(`/api/LN03/${id}`)
      return response.data
    } catch (error) {
      console.error(`❌ LN03 Get By ID Error (${id}):`, error)
      throw error
    }
  },

  /**
   * Create new LN03 record
   */
  async create(data) {
    try {
      const response = await apiClient.post('/api/LN03', data)
      return response.data
    } catch (error) {
      console.error('❌ LN03 Create Error:', error)
      throw error
    }
  },

  /**
   * Update existing LN03 record
   */
  async update(id, data) {
    try {
      const response = await apiClient.put(`/api/LN03/${id}`, data)
      return response.data
    } catch (error) {
      console.error(`❌ LN03 Update Error (${id}):`, error)
      throw error
    }
  },

  /**
   * Delete LN03 record
   */
  async delete(id) {
    try {
      const response = await apiClient.delete(`/api/LN03/${id}`)
      return response.data
    } catch (error) {
      console.error(`❌ LN03 Delete Error (${id}):`, error)
      throw error
    }
  },

  // ===== CSV IMPORT OPERATIONS =====

  /**
   * Import CSV file to LN03 table
   */
  async importCsv(file, replaceExistingData = false) {
    try {
      const formData = new FormData()
      formData.append('file', file)
      formData.append('replaceExistingData', replaceExistingData.toString())

      const response = await apiClient.post('/api/LN03/import-csv', formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
        timeout: 60000, // 60 seconds for large files
      })
      return response.data
    } catch (error) {
      console.error('❌ LN03 CSV Import Error:', error)
      throw error
    }
  },

  /**
   * Get CSV import template
   */
  async getCsvTemplate() {
    try {
      const response = await apiClient.get('/api/LN03/csv-template', {
        responseType: 'blob',
      })
      return response.data
    } catch (error) {
      console.error('❌ LN03 CSV Template Error:', error)
      throw error
    }
  },

  // ===== SEARCH & FILTER OPERATIONS =====

  /**
   * Get records by branch code
   */
  async getByBranch(branchCode, params = {}) {
    try {
      const response = await apiClient.get(`/api/LN03/by-branch/${branchCode}`, { params })
      return response.data
    } catch (error) {
      console.error(`❌ LN03 By Branch Error (${branchCode}):`, error)
      throw error
    }
  },

  /**
   * Get records by customer code
   */
  async getByCustomer(customerCode, params = {}) {
    try {
      const response = await apiClient.get(`/api/LN03/by-customer/${customerCode}`, { params })
      return response.data
    } catch (error) {
      console.error(`❌ LN03 By Customer Error (${customerCode}):`, error)
      throw error
    }
  },

  /**
   * Get records by date range
   */
  async getByDateRange(startDate, endDate, params = {}) {
    try {
      const response = await apiClient.get('/api/LN03/by-date-range', {
        params: {
          startDate: startDate.toISOString(),
          endDate: endDate.toISOString(),
          ...params,
        },
      })
      return response.data
    } catch (error) {
      console.error(`❌ LN03 By Date Range Error (${startDate} - ${endDate}):`, error)
      throw error
    }
  },

  /**
   * Search records with keyword
   */
  async search(keyword, params = {}) {
    try {
      const response = await apiClient.get('/api/LN03/search', {
        params: { keyword, ...params },
      })
      return response.data
    } catch (error) {
      console.error(`❌ LN03 Search Error (${keyword}):`, error)
      throw error
    }
  },

  // ===== TEMPORAL TABLE OPERATIONS =====

  /**
   * Get temporal history for a specific record
   */
  async getTemporalHistory(id) {
    try {
      const response = await apiClient.get(`/api/LN03/${id}/history`)
      return response.data
    } catch (error) {
      console.error(`❌ LN03 Temporal History Error (${id}):`, error)
      throw error
    }
  },

  /**
   * Get temporal data for a specific time period
   */
  async getTemporalData(asOfDate) {
    try {
      const response = await apiClient.get('/api/LN03/temporal', {
        params: { asOfDate: asOfDate.toISOString() },
      })
      return response.data
    } catch (error) {
      console.error(`❌ LN03 Temporal Data Error (${asOfDate}):`, error)
      throw error
    }
  },

  // ===== ANALYTICS & REPORTING =====

  /**
   * Get loan summary statistics
   */
  async getSummary() {
    try {
      const response = await apiClient.get('/api/LN03/summary')
      return response.data
    } catch (error) {
      console.error('❌ LN03 Summary Error:', error)
      throw error
    }
  },

  /**
   * Get loan amounts by branch
   */
  async getLoanAmountsByBranch(fromDate = null, toDate = null) {
    try {
      const params = {}
      if (fromDate) params.fromDate = fromDate.toISOString()
      if (toDate) params.toDate = toDate.toISOString()

      const response = await apiClient.get('/api/LN03/loan-amounts-by-branch', { params })
      return response.data
    } catch (error) {
      console.error('❌ LN03 Loan Amounts By Branch Error:', error)
      throw error
    }
  },

  /**
   * Get loan counts by type
   */
  async getLoanCountsByType(fromDate = null, toDate = null) {
    try {
      const params = {}
      if (fromDate) params.fromDate = fromDate.toISOString()
      if (toDate) params.toDate = toDate.toISOString()

      const response = await apiClient.get('/api/LN03/loan-counts-by-type', { params })
      return response.data
    } catch (error) {
      console.error('❌ LN03 Loan Counts By Type Error:', error)
      throw error
    }
  },

  /**
   * Get top customers by loan amount
   */
  async getTopCustomers(topCount = 10) {
    try {
      const response = await apiClient.get('/api/LN03/top-customers', {
        params: { topCount },
      })
      return response.data
    } catch (error) {
      console.error('❌ LN03 Top Customers Error:', error)
      throw error
    }
  },

  // ===== UTILITY OPERATIONS =====

  /**
   * Get available data dates
   */
  async getAvailableDates() {
    try {
      const response = await apiClient.get('/api/LN03/available-dates')
      return response.data
    } catch (error) {
      console.error('❌ LN03 Available Dates Error:', error)
      throw error
    }
  },

  /**
   * Export data to CSV
   */
  async exportToCsv(params = {}) {
    try {
      const response = await apiClient.get('/api/LN03/export-csv', {
        params,
        responseType: 'blob',
      })
      return response.data
    } catch (error) {
      console.error('❌ LN03 Export CSV Error:', error)
      throw error
    }
  },

  // ===== BULK OPERATIONS =====

  /**
   * Bulk insert records
   */
  async bulkInsert(records) {
    try {
      const response = await apiClient.post('/api/LN03/bulk-insert', records)
      return response.data
    } catch (error) {
      console.error('❌ LN03 Bulk Insert Error:', error)
      throw error
    }
  },

  /**
   * Bulk update records
   */
  async bulkUpdate(records) {
    try {
      const response = await apiClient.put('/api/LN03/bulk-update', records)
      return response.data
    } catch (error) {
      console.error('❌ LN03 Bulk Update Error:', error)
      throw error
    }
  },

  /**
   * Bulk delete records
   */
  async bulkDelete(ids) {
    try {
      const response = await apiClient.delete('/api/LN03/bulk-delete', {
        data: { ids },
      })
      return response.data
    } catch (error) {
      console.error('❌ LN03 Bulk Delete Error:', error)
      throw error
    }
  },
}

export default ln03Service
