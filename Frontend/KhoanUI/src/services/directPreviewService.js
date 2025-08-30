// 🚀 Direct Preview Service - Xem trước dữ liệu trực tiếp từ DataTables
// Nhanh hơn và hiệu quả hơn so với ImportedDataRecords

import api from './api.js'

class DirectPreviewService {
  constructor() {
    console.log('🎯 DirectPreviewService initialized')
  }

  /**
   * Lấy danh sách data types với số lượng records
   * @returns {Promise<Object>} Response với danh sách data types và counts
   */
  async getDataTypesWithCounts() {
    try {
      console.log('📊 Getting data types with counts...')
      const response = await api.get('/DataImport/direct-preview/data-types')

      console.log('✅ Data types loaded:', response.data?.length || 0, 'types')
      return {
        success: true,
        data: response.data || [],
        message: 'Lấy danh sách data types thành công',
      }
    } catch (error) {
      console.error('❌ Error getting data types:', error)
      return {
        success: false,
        data: [],
        error: error.message || 'Lỗi khi lấy danh sách data types',
        errorDetails: error.response?.data,
      }
    }
  }

  /**
   * Preview dữ liệu trực tiếp từ bảng theo data type
   * @param {string} dataType - Loại dữ liệu (DP01, EI01, etc.)
   * @param {number} page - Trang hiện tại (default: 1)
   * @param {number} pageSize - Số records per page (default: 50)
   * @returns {Promise<Object>} Response với data và pagination info
   */
  async previewDataType(dataType, page = 1, pageSize = 50) {
    try {
      if (!dataType) {
        throw new Error('Data type không được để trống')
      }

      console.log(`🔍 Previewing ${dataType} - page ${page}, size ${pageSize}`)

      const response = await api.get(`/DataImport/direct-preview/${dataType}`, {
        params: { page, pageSize },
      })

      const result = response.data
      console.log(`✅ Preview ${dataType}:`, result.TotalRecords, 'total,', result.Data?.length || 0, 'loaded')

      return {
        success: true,
        data: result.Data || [],
        totalRecords: result.TotalRecords || 0,
        page: result.Page || 1,
        pageSize: result.PageSize || pageSize,
        totalPages: result.TotalPages || 1,
        dataType: result.DataType || dataType,
        message: result.Message || `Xem trước ${dataType} thành công`,
      }
    } catch (error) {
      console.error(`❌ Error previewing ${dataType}:`, error)
      return {
        success: false,
        data: [],
        totalRecords: 0,
        page: 1,
        pageSize: pageSize,
        totalPages: 0,
        dataType: dataType,
        error: error.message || `Lỗi khi xem trước ${dataType}`,
        errorDetails: error.response?.data,
      }
    }
  }

  /**
   * Lấy thống kê tổng quan về tất cả bảng data
   * @returns {Promise<Object>} Response với statistics của tất cả bảng
   */
  async getDataStatistics() {
    try {
      console.log('📈 Getting data statistics...')
      const response = await api.get('/DataImport/direct-preview/statistics')

      const result = response.data
      console.log('✅ Statistics loaded:', result.Summary?.TotalRecords || 0, 'total records')

      return {
        success: true,
        tables: result.Tables || {},
        summary: result.Summary || {},
        message: 'Lấy thống kê thành công',
      }
    } catch (error) {
      console.error('❌ Error getting statistics:', error)
      return {
        success: false,
        tables: {},
        summary: {},
        error: error.message || 'Lỗi khi lấy thống kê',
        errorDetails: error.response?.data,
      }
    }
  }

  /**
   * Kiểm tra xem một data type có dữ liệu hay không
   * @param {string} dataType - Loại dữ liệu cần kiểm tra
   * @returns {Promise<boolean>} True nếu có dữ liệu
   */
  async hasData(dataType) {
    try {
      const result = await this.previewDataType(dataType, 1, 1)
      return result.success && result.totalRecords > 0
    } catch (error) {
      console.error(`❌ Error checking data for ${dataType}:`, error)
      return false
    }
  }

  /**
   * Lấy sample data để hiển thị quick preview
   * @param {string} dataType - Loại dữ liệu
   * @param {number} sampleSize - Số lượng records mẫu (default: 5)
   * @returns {Promise<Object>} Response với sample data
   */
  async getSampleData(dataType, sampleSize = 5) {
    try {
      console.log(`🎲 Getting sample data for ${dataType}...`)
      const result = await this.previewDataType(dataType, 1, sampleSize)

      return {
        success: result.success,
        data: result.data,
        totalRecords: result.totalRecords,
        sampleSize: result.data?.length || 0,
        dataType: dataType,
        message: result.message,
      }
    } catch (error) {
      console.error(`❌ Error getting sample data for ${dataType}:`, error)
      return {
        success: false,
        data: [],
        totalRecords: 0,
        sampleSize: 0,
        dataType: dataType,
        error: error.message || `Lỗi khi lấy sample data ${dataType}`,
      }
    }
  }

  /**
   * Format data cho display trong bảng
   * @param {Array} data - Dữ liệu thô từ API
   * @param {string} dataType - Loại dữ liệu
   * @returns {Object} Formatted data với columns và rows
   */
  formatDataForDisplay(data, dataType) {
    try {
      if (!data || !Array.isArray(data) || data.length === 0) {
        return {
          columns: [],
          rows: [],
          totalColumns: 0,
          totalRows: 0,
        }
      }

      // Lấy columns từ record đầu tiên
      const firstRecord = data[0]
      const columns = Object.keys(firstRecord).map(key => ({
        key: key,
        label: key,
        type: this.detectColumnType(firstRecord[key]),
        sortable: true,
      }))

      // Format rows
      const rows = data.map((record, index) => ({
        id: record.Id || record.id || index,
        ...record,
      }))

      console.log(`🎨 Formatted ${dataType}:`, columns.length, 'columns,', rows.length, 'rows')

      return {
        columns: columns,
        rows: rows,
        totalColumns: columns.length,
        totalRows: rows.length,
        dataType: dataType,
      }
    } catch (error) {
      console.error('❌ Error formatting data for display:', error)
      return {
        columns: [],
        rows: [],
        totalColumns: 0,
        totalRows: 0,
        error: error.message,
      }
    }
  }

  /**
   * Detect kiểu dữ liệu của column
   * @param {any} value - Giá trị cần detect type
   * @returns {string} Kiểu dữ liệu
   */
  detectColumnType(value) {
    if (value === null || value === undefined) return 'text'
    if (typeof value === 'number') return 'number'
    if (typeof value === 'boolean') return 'boolean'
    if (value instanceof Date) return 'date'
    if (typeof value === 'string') {
      // Check if it's a date string
      if (/^\d{4}-\d{2}-\d{2}/.test(value)) return 'date'
      // Check if it's a number string
      if (/^\d+(\.\d+)?$/.test(value)) return 'number'
    }
    return 'text'
  }

  /**
   * Kiểm tra xem có dữ liệu cho data type không
   */
  async hasData(dataType) {
    try {
      const result = await this.getDataTypesWithCounts()
      if (!result.success) return false

      const dataTypeInfo = result.data.find(dt => dt.DataType === dataType)
      return dataTypeInfo && dataTypeInfo.RecordCount > 0
    } catch (error) {
      console.error('Error checking data availability:', error)
      return false
    }
  }
}

// Export singleton instance
export default new DirectPreviewService()
