import apiClient from './api.js'

/**
 * Smart Data Import Service - Handles intelligent file upload with automatic categorization
 * Sử dụng API SmartDataImport để tự động phân loại và import dữ liệu
 */
class SmartImportService {

  /**
   * Upload file với Smart Import - tự động phân loại dựa vào filename
   * @param {File} file - File cần upload
   * @param {Date} statementDate - Ngày sao kê (tùy chọn)
   * @returns {Promise} Kết quả upload và xử lý
   */
  async uploadSmartFile(file, statementDate = null) {
    try {
      const formData = new FormData()
      formData.append('file', file)

      // Thêm statement date nếu có
      if (statementDate) {
        formData.append('statementDate', statementDate.toISOString())
      }

      const response = await apiClient.post('/SmartDataImport/upload', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        },
        timeout: 300000 // 5 phút timeout cho file lớn
      })

      return response.data
    } catch (error) {
      console.error('🔥 Smart Import upload error:', error)
      throw new Error(`Smart Import failed: ${error.response?.data?.message || error.message}`)
    }
  }

  /**
   * Upload nhiều file với Smart Import
   * @param {FileList|Array} files - Danh sách file cần upload
   * @param {Date} statementDate - Ngày sao kê (tùy chọn)
   * @returns {Promise} Kết quả upload batch
   */
  async uploadSmartFiles(files, statementDate = null) {
    try {
      const results = []

      // Upload từng file một để có thể tracking progress
      for (let i = 0; i < files.length; i++) {
        const file = files[i]
        console.log(`📤 Smart uploading file ${i + 1}/${files.length}: ${file.name}`)

        try {
          const result = await this.uploadSmartFile(file, statementDate)
          results.push({
            fileName: file.name,
            success: true,
            result: result,
            index: i + 1
          })
        } catch (error) {
          results.push({
            fileName: file.name,
            success: false,
            error: error.message,
            index: i + 1
          })
        }
      }

      return {
        totalFiles: files.length,
        successCount: results.filter(r => r.success).length,
        failureCount: results.filter(r => !r.success).length,
        results: results
      }
    } catch (error) {
      console.error('🔥 Smart Import batch upload error:', error)
      throw new Error(`Smart Import batch failed: ${error.message}`)
    }
  }

  /**
   * Lấy danh sách file đã upload và kết quả xử lý
   * @returns {Promise} Danh sách import records
   */
  async getImportedRecords() {
    try {
      const response = await apiClient.get('/SmartDataImport/imported-records')
      return response.data
    } catch (error) {
      console.error('🔥 Error fetching imported records:', error)
      throw new Error(`Fetch imported records failed: ${error.response?.data?.message || error.message}`)
    }
  }

  /**
   * Lấy chi tiết một import record
   * @param {number} recordId - ID của import record
   * @returns {Promise} Chi tiết import record
   */
  async getImportedRecordDetail(recordId) {
    try {
      const response = await apiClient.get(`/SmartDataImport/imported-records/${recordId}`)
      return response.data
    } catch (error) {
      console.error('🔥 Error fetching import record detail:', error)
      throw new Error(`Fetch import record detail failed: ${error.response?.data?.message || error.message}`)
    }
  }

  /**
   * Process imported data to history tables (move to raw data tables)
   * @param {number} recordId - ID của import record
   * @param {string} category - Category của data
   * @param {Date} statementDate - Ngày sao kê
   * @returns {Promise} Kết quả processing
   */
  async processToHistoryTables(recordId, category, statementDate = null) {
    try {
      const payload = {
        importedDataRecordId: recordId,
        category: category
      }

      if (statementDate) {
        payload.statementDate = statementDate.toISOString()
      }

      const response = await apiClient.post('/SmartDataImport/process-to-history', payload)
      return response.data
    } catch (error) {
      console.error('🔥 Error processing to history tables:', error)
      throw new Error(`Process to history failed: ${error.response?.data?.message || error.message}`)
    }
  }

  /**
   * Validate category support
   * @returns {Promise} Danh sách category được hỗ trợ
   */
  async getSupportedCategories() {
    try {
      const response = await apiClient.get('/SmartDataImport/supported-categories')
      return response.data
    } catch (error) {
      console.error('🔥 Error fetching supported categories:', error)
      return ['DP01', 'LN01', 'LN02', 'LN03', 'GL01', 'GL41', 'DB01', 'DPDA', 'EI01', 'KH03', 'RR01', 'DT_KHKD1'] // fallback
    }
  }

  /**
   * Tự động detect category từ filename
   * @param {string} fileName - Tên file
   * @returns {string} Category được detect
   */
  detectCategoryFromFileName(fileName) {
    const upperFileName = fileName.toUpperCase()

    // Pattern matching for different data types
    const patterns = {
      'DP01': /DP01|DEPOSIT|TIENGUI/,
      'LN01': /LN01|LOAN|CHOAVAY/,
      'LN02': /LN02|LOAN.*SCHEDULE|LICHTRANGOAN/,
      'LN03': /LN03|LOAN.*CLASSIFY|PHANLOAINO/,
      'GL01': /GL01|GENERAL.*LEDGER|SOTONGOHAP/,
      'GL41': /GL41|BALANCE|BANGSODUE/,
      'DB01': /DB01|COLLATERAL|TAISANDAMBAO/,
      'DPDA': /DPDA|DEPOSIT.*DETAIL|CHITIETTIENGUI/,
      'EI01': /EI01|INCOME|THUNHAP/,
      'KH03': /KH03|CUSTOMER|KHACHHANG/,
      'RR01': /RR01|RATIO|TYLE/,
      'DT_KHKD1': /DT_KHKD1|7800|KHKD|KINHKINHDOANH/
    }

    for (const [category, pattern] of Object.entries(patterns)) {
      if (pattern.test(upperFileName)) {
        return category
      }
    }

    return 'UNKNOWN'
  }

  /**
   * Extract NgayDL from filename
   * @param {string} fileName - Tên file
   * @returns {Date|null} Ngày DL được extract
   */
  extractDateFromFileName(fileName) {
    const patterns = [
      /(\d{4})(\d{2})(\d{2})/,  // YYYYMMDD
      /(\d{2})(\d{2})(\d{4})/,  // DDMMYYYY
      /(\d{4})-(\d{2})-(\d{2})/, // YYYY-MM-DD
      /(\d{2})-(\d{2})-(\d{4})/  // DD-MM-YYYY
    ]

    for (const pattern of patterns) {
      const match = fileName.match(pattern)
      if (match) {
        let year, month, day
        if (pattern.source.includes('(\\d{4})') && match[1].length === 4) {
          // YYYY first
          year = parseInt(match[1])
          month = parseInt(match[2])
          day = parseInt(match[3])
        } else {
          // DD first
          day = parseInt(match[1])
          month = parseInt(match[2])
          year = parseInt(match[3])
        }

        if (year > 2000 && year < 2100 && month >= 1 && month <= 12 && day >= 1 && day <= 31) {
          return new Date(year, month - 1, day)
        }
      }
    }

    return null
  }
}

// Export singleton instance
export default new SmartImportService()
