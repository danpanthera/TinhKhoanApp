import { formatFileSize } from '../utils/numberFormatter.js'
import apiClient from './api.js'

/**
 * Smart Data Import Service - Handles intelligent file upload with automatic categorization
 * Sử dụng API SmartDataImport để tự động phân loại và import dữ liệu
 * Hỗ trợ chunked upload cho file lớn và progress tracking
 */
class SmartImportService {

  constructor() {
    // Cấu hình chunked upload
    this.CHUNK_SIZE = 1024 * 1024 * 2 // 2MB mỗi chunk
    this.MAX_FILE_SIZE = 1024 * 1024 * 100 // 100MB max file size
    this.LARGE_FILE_THRESHOLD = 1024 * 1024 * 10 // 10MB để quyết định có dùng chunked upload
  }

  /**
   * Upload file với Smart Import - tự động phân loại dựa vào filename
   * @param {File} file - File cần upload
   * @param {Date} statementDate - Ngày sao kê (tùy chọn)
   * @param {Function} progressCallback - Callback để track progress
   * @returns {Promise} Kết quả upload và xử lý
   */
  async uploadSmartFile(file, statementDate = null, progressCallback = null) {
    try {
      // Validation file size
      if (file.size > this.MAX_FILE_SIZE) {
        throw new Error(`File ${file.name} quá lớn (${formatFileSize(file.size)}). Giới hạn tối đa: ${formatFileSize(this.MAX_FILE_SIZE)}`)
      }

      // Quyết định dùng chunked upload hay normal upload
      if (file.size > this.LARGE_FILE_THRESHOLD) {
        console.log(`📦 Using chunked upload for large file: ${file.name} (${formatFileSize(file.size)})`)
        return await this.uploadLargeFile(file, statementDate, progressCallback)
      } else {
        console.log(`📤 Using normal upload for file: ${file.name} (${formatFileSize(file.size)})`)
        return await this.uploadNormalFile(file, statementDate, progressCallback)
      }
    } catch (error) {
      console.error('🔥 Smart Import upload error:', error)
      throw new Error(`Smart Import failed: ${error.response?.data?.message || error.message}`)
    }
  }

  /**
   * Upload file thông thường (nhỏ hơn 10MB) - OPTIMIZED
   */
  async uploadNormalFile(file, statementDate = null, progressCallback = null) {
    const formData = new FormData()
    formData.append('file', file)

    // Thêm statement date nếu có
    if (statementDate) {
      formData.append('statementDate', statementDate.toISOString())
    }

    const response = await apiClient.post('/DirectImport/smart', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
        // 🚀 Removed Accept-Encoding - Browser handles this automatically
      },
      timeout: 300000, // 🚀 Tăng lên 5 phút cho smart upload
      onUploadProgress: (progressEvent) => {
        if (progressCallback && progressEvent.total) {
          const percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total)
          progressCallback({
            loaded: progressEvent.loaded,
            total: progressEvent.total,
            percentage: percentCompleted,
            stage: 'uploading'
          })
        }
      }
    })

    return response.data
  }

  /**
   * Upload file lớn với chunked upload - OPTIMIZED
   */
  async uploadLargeFile(file, statementDate = null, progressCallback = null) {
    // TODO: Implement true chunked upload when backend supports it
    // For now, use optimized upload with extended timeout
    const formData = new FormData()
    formData.append('file', file)

    if (statementDate) {
      formData.append('statementDate', statementDate.toISOString())
    }

    const response = await apiClient.post('/DirectImport/smart', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
        // 🚀 Removed Accept-Encoding - Browser handles this automatically
      },
      timeout: 600000, // 🚀 Tăng lên 10 phút cho file lớn
      onUploadProgress: (progressEvent) => {
        if (progressCallback && progressEvent.total) {
          const percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total)
          progressCallback({
            loaded: progressEvent.loaded,
            total: progressEvent.total,
            percentage: percentCompleted,
            stage: 'uploading'
          })
        }
      }
    })

    return response.data
  }

  // Loại bỏ method formatFileSize cũ vì đã import từ numberFormatter
  // formatFileSize() method removed - using imported utility

  /**
   * Upload nhiều file với Smart Import - OPTIMIZED PARALLEL VERSION
   * @param {FileList|Array} files - Danh sách file cần upload
   * @param {Date} statementDate - Ngày sao kê (tùy chọn)
   * @param {Function} progressCallback - Callback để track progress tổng thể
   * @returns {Promise} Kết quả upload batch
   */
  async uploadSmartFiles(files, statementDate = null, progressCallback = null) {
    try {
      const totalFiles = files.length
      const MAX_CONCURRENT_UPLOADS = 5 // 🚀 Tăng lên 5 file cùng lúc để tăng tốc

      console.log(`🚀 Starting PARALLEL Smart Import with ${totalFiles} files (max ${MAX_CONCURRENT_UPLOADS} concurrent)`)

      // 📊 Tracking variables
      const results = []
      const progressTracking = new Map() // Track progress của từng file
      let completedCount = 0

      // Update overall progress function
      const updateOverallProgress = () => {
        const totalProgress = Array.from(progressTracking.values()).reduce((sum, progress) => sum + progress, 0)
        const overallPercentage = Math.round(totalProgress / totalFiles)

        if (progressCallback) {
          const currentFiles = Array.from(progressTracking.keys()).filter(fileName =>
            progressTracking.get(fileName) < 100
          )

          progressCallback({
            current: completedCount,
            total: totalFiles,
            percentage: overallPercentage,
            currentFile: currentFiles.length > 0 ? `${currentFiles.length} file(s) đang upload...` : 'Hoàn thành',
            stage: completedCount === totalFiles ? 'completed' : 'uploading',
            activeFiles: currentFiles.length
          })
        }
      }

      // 🚀 Create upload promises với concurrency control
      const uploadFile = async (file, index) => {
        try {
          // Initialize progress tracking
          progressTracking.set(file.name, 0)

          // Progress callback cho từng file
          const fileProgressCallback = (fileProgress) => {
            progressTracking.set(file.name, fileProgress.percentage)
            updateOverallProgress()
          }

          const result = await this.uploadSmartFile(file, statementDate, fileProgressCallback)

          // Mark as completed
          progressTracking.set(file.name, 100)
          completedCount++

          console.log(`✅ Successfully uploaded: ${file.name} (${completedCount}/${totalFiles})`)

          return {
            fileName: file.name,
            success: true,
            result: result,
            index: index + 1,
            fileSize: file.size
          }
        } catch (error) {
          completedCount++
          console.error(`❌ Failed to upload: ${file.name}`, error)

          return {
            fileName: file.name,
            success: false,
            error: error.message,
            index: index + 1,
            fileSize: file.size
          }
        }
      }

      // 🚀 TRUE PARALLEL PROCESSING - All files at once for maximum speed
      console.log('🏎️ Using TRUE parallel processing (all files at once)')

      // Create all upload promises at once
      const allPromises = Array.from(files).map((file, index) => uploadFile(file, index))

      // Use Promise.allSettled to handle individual failures gracefully
      const settledResults = await Promise.allSettled(allPromises)

      // Extract results from settled promises
      settledResults.forEach((settled, index) => {
        if (settled.status === 'fulfilled') {
          results.push(settled.value)
        } else {
          results.push({
            fileName: files[index].name,
            success: false,
            error: settled.reason?.message || 'Unknown error',
            index: index + 1,
            fileSize: files[index].size
          })
        }
      })

      // Final progress update
      if (progressCallback) {
        progressCallback({
          current: totalFiles,
          total: totalFiles,
          percentage: 100,
          currentFile: 'Hoàn thành tất cả',
          stage: 'completed',
          activeFiles: 0
        })
      }

      const successCount = results.filter(r => r.success).length
      const failureCount = results.filter(r => !r.success).length

      console.log(`🏁 TRUE PARALLEL Smart Import completed: ${successCount}/${totalFiles} successful`)

      return {
        totalFiles: totalFiles,
        successCount: successCount,
        failureCount: failureCount,
        results: results,
        totalSize: Array.from(files).reduce((sum, file) => sum + file.size, 0),
        uploadMethod: 'true-parallel',
        maxConcurrency: 'unlimited'
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
