import apiClient from './api.js'

/**
 * Smart Data Import Service - Handles intelligent file upload with automatic categorization
 * Sử dụng API SmartDataImport để tự động phân loại và import dữ liệu
 */
class SmartImportService {

  /**
   * Upload file với Smart Import - tự động phân loại dựa vào filename
   * OPTIMIZED VERSION với timeout ngắn hơn và retry logic
   * @param {File} file - File cần upload
   * @param {Date} statementDate - Ngày sao kê (tùy chọn)
   * @returns {Promise} Kết quả upload và xử lý
   */
  async uploadSmartFile(file, statementDate = null) {
    const MAX_RETRIES = 2
    const TIMEOUT = 30000 // 30 giây thay vì 5 phút

    for (let attempt = 1; attempt <= MAX_RETRIES; attempt++) {
      try {
        console.log(`📤 Smart Import attempt ${attempt}/${MAX_RETRIES} for: ${file.name}`)

        const formData = new FormData()
        formData.append('file', file)

        // Thêm statement date nếu có
        if (statementDate) {
          formData.append('statementDate', statementDate.toISOString())
        }

        // ✅ OPTIMIZATION: Giảm timeout và thêm progress tracking
        const response = await apiClient.post('/SmartDataImport/upload', formData, {
          headers: {
            'Content-Type': 'multipart/form-data'
          },
          timeout: TIMEOUT,
          onUploadProgress: (progressEvent) => {
            if (progressEvent.lengthComputable) {
              const percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total)
              console.log(`📊 Upload progress for ${file.name}: ${percentCompleted}%`)
            }
          }
        })

        console.log(`✅ Smart Import success on attempt ${attempt} for: ${file.name}`)
        return response.data

      } catch (error) {
        console.error(`❌ Smart Import attempt ${attempt} failed for ${file.name}:`, error.message)

        // Nếu là lần thử cuối hoặc lỗi không retry được
        if (attempt === MAX_RETRIES || this.isNonRetryableError(error)) {
          throw new Error(`Smart Import failed after ${attempt} attempt(s): ${error.response?.data?.message || error.message}`)
        }

        // Đợi trước khi retry (exponential backoff)
        const delay = Math.min(1000 * Math.pow(2, attempt - 1), 5000) // 1s, 2s, max 5s
        console.log(`⏳ Retrying in ${delay}ms...`)
        await new Promise(resolve => setTimeout(resolve, delay))
      }
    }
  }

  /**
   * Kiểm tra xem lỗi có nên retry không
   */
  isNonRetryableError(error) {
    const status = error.response?.status

    // Không retry với lỗi client (4xx) trừ timeout
    if (status >= 400 && status < 500 && status !== 408 && status !== 429) {
      return true
    }

    // Không retry với lỗi file format
    if (error.message?.includes('format') || error.message?.includes('invalid file')) {
      return true
    }

    return false
  }

  /**
   * Upload nhiều file với Smart Import - OPTIMIZED VERSION
   * @param {FileList|Array} files - Danh sách file cần upload
   * @param {Date} statementDate - Ngày sao kê (tùy chọn)
   * @param {Function} progressCallback - Callback để update progress
   * @returns {Promise} Kết quả upload batch
   */
  async uploadSmartFiles(files, statementDate = null, progressCallback = null) {
    try {
      const results = []
      const startTime = Date.now()

      // ✅ OPTIMIZATION 1: Batch upload nhỏ thay vì từng file một
      const BATCH_SIZE = 3 // Upload 3 file cùng lúc
      const batches = []

      for (let i = 0; i < files.length; i += BATCH_SIZE) {
        batches.push(files.slice(i, i + BATCH_SIZE))
      }

      console.log(`🚀 Smart Import SUPER OPTIMIZED: ${files.length} files using batch endpoint`)

      // ✅ SUPER OPTIMIZATION: Sử dụng batch upload endpoint khi có ít file
      if (files.length <= 5) {
        console.log('📦 Using single batch upload for', files.length, 'files')
        const result = await this.uploadSmartFilesBatch(files, statementDate, progressCallback)
        result.duration = (Date.now() - startTime) / 1000
        return result
      }

      let completedFiles = 0

      for (let batchIndex = 0; batchIndex < batches.length; batchIndex++) {
        const batch = batches[batchIndex]
        console.log(`� Processing batch ${batchIndex + 1}/${batches.length} (${batch.length} files)`)

        // ✅ OPTIMIZATION 2: Upload song song trong batch
        const batchPromises = batch.map(async (file, fileIndex) => {
          const overallIndex = batchIndex * BATCH_SIZE + fileIndex

          try {
            // Update progress cho file hiện tại
            if (progressCallback) {
              progressCallback({
                current: completedFiles + 1,
                total: files.length,
                percentage: Math.round((completedFiles / files.length) * 100),
                currentFile: file.name,
                status: 'uploading'
              })
            }

            console.log(`📤 Uploading ${overallIndex + 1}/${files.length}: ${file.name}`)

            const result = await this.uploadSmartFile(file, statementDate)

            completedFiles++

            // Update progress sau khi hoàn thành
            if (progressCallback) {
              progressCallback({
                current: completedFiles,
                total: files.length,
                percentage: Math.round((completedFiles / files.length) * 100),
                currentFile: file.name,
                status: 'completed'
              })
            }

            return {
              fileName: file.name,
              success: true,
              result: result,
              index: overallIndex + 1
            }
          } catch (error) {
            completedFiles++

            if (progressCallback) {
              progressCallback({
                current: completedFiles,
                total: files.length,
                percentage: Math.round((completedFiles / files.length) * 100),
                currentFile: file.name,
                status: 'error'
              })
            }

            return {
              fileName: file.name,
              success: false,
              error: error.message,
              index: overallIndex + 1
            }
          }
        })

        // Đợi batch hoàn thành
        const batchResults = await Promise.all(batchPromises)
        results.push(...batchResults)

        // ✅ OPTIMIZATION 3: Thêm delay nhỏ giữa các batch để tránh overwhelm server
        if (batchIndex < batches.length - 1) {
          await new Promise(resolve => setTimeout(resolve, 500)) // 500ms delay
        }
      }

      const totalTime = (Date.now() - startTime) / 1000
      console.log(`✅ Smart Import completed in ${totalTime.toFixed(2)}s`)

      return {
        totalFiles: files.length,
        successCount: results.filter(r => r.success).length,
        failureCount: results.filter(r => !r.success).length,
        results: results,
        duration: totalTime
      }
    } catch (error) {
      console.error('🔥 Smart Import batch upload error:', error)
      throw new Error(`Smart Import batch failed: ${error.message}`)
    }
  }

  /**
   * Upload batch files qua endpoint /upload-multiple - SUPER OPTIMIZED
   */
  async uploadSmartFilesBatch(files, statementDate = null, progressCallback = null) {
    try {
      if (progressCallback) {
        progressCallback({
          current: 0,
          total: files.length,
          percentage: 0,
          currentFile: 'Preparing batch upload...',
          status: 'preparing'
        })
      }

      const formData = new FormData()

      // Thêm tất cả files
      files.forEach((file, index) => {
        formData.append('files', file)
      })

      // Thêm statement date nếu có
      if (statementDate) {
        formData.append('statementDate', statementDate.toISOString())
      }

      if (progressCallback) {
        progressCallback({
          current: 0,
          total: files.length,
          percentage: 10,
          currentFile: 'Uploading batch...',
          status: 'uploading'
        })
      }

      const response = await apiClient.post('/SmartDataImport/upload-multiple', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        },
        timeout: 60000, // 1 phút cho batch
        onUploadProgress: (progressEvent) => {
          if (progressEvent.lengthComputable && progressCallback) {
            const uploadPercent = Math.round((progressEvent.loaded * 100) / progressEvent.total)
            progressCallback({
              current: Math.floor(files.length * uploadPercent / 100),
              total: files.length,
              percentage: uploadPercent,
              currentFile: 'Processing batch...',
              status: 'processing'
            })
          }
        }
      })

      const result = response.data

      if (progressCallback) {
        progressCallback({
          current: files.length,
          total: files.length,
          percentage: 100,
          currentFile: 'Completed!',
          status: 'completed'
        })
      }

      return {
        totalFiles: result.totalFiles || files.length,
        successCount: result.successCount || 0,
        failureCount: (result.totalFiles || files.length) - (result.successCount || 0),
        results: result.results?.map((r, index) => ({
          fileName: r.fileName,
          success: r.success,
          result: r,
          index: index + 1
        })) || [],
        duration: result.results?.[0]?.duration || 0
      }

    } catch (error) {
      console.error('🔥 Batch upload error:', error)
      throw new Error(`Batch upload failed: ${error.response?.data?.message || error.message}`)
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
