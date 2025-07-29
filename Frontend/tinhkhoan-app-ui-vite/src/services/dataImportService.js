import apiClient from './api.js'
import ChunkedUploadService from './chunkedUploadService.js'

/**
 * 🚀 Enhanced Data Import Service - Multiple upload strategies for optimal performance
 */
class DataImportService {
  /**
   * Upload a file to the server using Direct Import (tăng tốc 2-5x)
   * @param {File} file - The file to upload
   * @param {string} category - The category of data (không dùng nữa)
   * @param {Function} onProgress - Progress callback function
   * @returns {Promise} Upload result
   */
  async uploadFile(file, category = 'General', onProgress = null) {
    try {
      const formData = new FormData()
      formData.append('file', file)

      // 🚀 Enhanced config for large files
      const config = {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
        timeout: 900000, // 15 phút cho file rất lớn
        maxContentLength: Infinity,
        maxBodyLength: Infinity,
        // 📊 Progress tracking
        onUploadProgress: progressEvent => {
          if (onProgress && progressEvent.total) {
            const percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total)
            onProgress(percentCompleted, progressEvent.loaded, progressEvent.total)
          }
        },
      }

      console.log(`🚀 Starting upload for ${file.name} (${(file.size / 1024 / 1024).toFixed(2)} MB)`)

      // 🚀 Sử dụng Direct Import API - tăng tốc 2-5x
      const response = await apiClient.post('/DirectImport/smart', formData, config)

      console.log(`✅ Upload completed for ${file.name}`)
      return response.data
    } catch (error) {
      console.error('Error uploading file:', error)
      if (error.code === 'ECONNABORTED') {
        throw new Error(`Upload timeout: File quá lớn hoặc kết nối chậm. Thử lại với file nhỏ hơn.`)
      }
      throw new Error(`Direct Import failed: ${error.response?.data?.message || error.message}`)
    }
  }

  /**
   * Upload multiple files using Direct Import (tăng tốc 2-5x)
   * @param {FileList|Array} files - Files to upload
   * @param {string} category - The category of data (không dùng nữa)
   * @returns {Promise} Upload results
   */
  async uploadFiles(files, category = 'General') {
    try {
      const results = []

      // 🚀 Upload từng file bằng Direct Import API song song
      const uploadPromises = Array.from(files).map(async file => {
        const formData = new FormData()
        formData.append('file', file)

        const response = await apiClient.post('/DirectImport/smart', formData, {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
          timeout: 600000, // 10 phút cho file lớn
        })

        return {
          fileName: file.name,
          result: response.data,
        }
      })

      const uploadResults = await Promise.all(uploadPromises)

      return {
        success: true,
        results: uploadResults,
        totalFiles: files.length,
        message: `Successfully uploaded ${files.length} files using Direct Import`,
      }
    } catch (error) {
      console.error('Error uploading files:', error)
      throw new Error(`Direct Import failed: ${error.response?.data?.message || error.message}`)
    }
  }

  /**
   * Get all imported data records
   * @returns {Promise<Array>} List of imported data records
   */
  async getImportedData() {
    try {
      const response = await apiClient.get('/DataImport')

      // Handle the .NET $values format
      if (response.data && response.data.$values) {
        return response.data.$values
      }

      return response.data || []
    } catch (error) {
      console.error('Error getting imported data:', error)
      throw new Error(`Failed to get imported data: ${error.response?.data?.message || error.message}`)
    }
  }

  /**
   * Get preview of imported data
   * @param {number} recordId - ID of the import record
   * @returns {Promise<Object>} Preview data with columns and records
   */
  async getDataPreview(recordId) {
    try {
      const response = await apiClient.get(`/DataImport/${recordId}/preview`)
      return response.data
    } catch (error) {
      console.error('Error getting data preview:', error)
      throw new Error(`Failed to get data preview: ${error.response?.data?.message || error.message}`)
    }
  }

  /**
   * Download original file
   * @param {number} recordId - ID of the import record
   * @returns {Promise} File download
   */
  async downloadOriginalFile(recordId) {
    try {
      const response = await apiClient.get(`/DataImport/${recordId}/download`, {
        responseType: 'blob',
      })

      // Create download link
      const url = window.URL.createObjectURL(new Blob([response.data]))
      const link = document.createElement('a')
      link.href = url

      // Get filename from response headers
      const contentDisposition = response.headers['content-disposition']
      let filename = 'download'
      if (contentDisposition) {
        const filenameMatch = contentDisposition.match(/filename="(.+)"/)
        if (filenameMatch) {
          filename = filenameMatch[1]
        }
      }

      link.setAttribute('download', filename)
      document.body.appendChild(link)
      link.click()
      link.remove()
      window.URL.revokeObjectURL(url)

      return { success: true, filename }
    } catch (error) {
      console.error('Error downloading file:', error)
      throw new Error(`Download failed: ${error.response?.data?.message || error.message}`)
    }
  }

  /**
   * Delete an imported data record
   * @param {number} recordId - ID of the record to delete
   * @returns {Promise} Delete result
   */
  async deleteData(recordId) {
    try {
      const response = await apiClient.delete(`/DataImport/${recordId}`)
      return response.data
    } catch (error) {
      console.error('Error deleting data:', error)
      throw new Error(`Delete failed: ${error.response?.data?.message || error.message}`)
    }
  }

  /**
   * Export all data to Excel
   * @returns {Promise} File download
   */
  async exportAllData() {
    try {
      const response = await apiClient.get('/DataImport/export', {
        responseType: 'blob',
      })

      // Create download link
      const url = window.URL.createObjectURL(new Blob([response.data]))
      const link = document.createElement('a')
      link.href = url

      // Generate filename with timestamp
      const timestamp = new Date().toISOString().slice(0, 19).replace(/[:-]/g, '')
      const filename = `KPI_Data_Export_${timestamp}.xlsx`

      link.setAttribute('download', filename)
      document.body.appendChild(link)
      link.click()
      link.remove()
      window.URL.revokeObjectURL(url)

      return { success: true, filename }
    } catch (error) {
      console.error('Error exporting data:', error)
      throw new Error(`Export failed: ${error.response?.data?.message || error.message}`)
    }
  }

  /**
   * Get KPI categories for data classification
   * @returns {Array} List of predefined KPI categories
   */
  getKpiCategories() {
    return [
      {
        id: 1,
        name: 'Nguồn vốn & Dư nợ',
        icon: '💰',
        description: 'Tổng nguồn vốn, dư nợ, nguồn vốn bình quân, dư nợ bình quân',
        indicators: ['Tổng nguồn vốn', 'Dư nợ', 'Nguồn vốn BQ', 'Dư nợ BQ'],
      },
      {
        id: 2,
        name: 'Lợi nhuận & Tài chính',
        icon: '📈',
        description: 'Lợi nhuận khoán tài chính, các chỉ tiêu hiệu quả kinh doanh',
        indicators: ['Lợi nhuận khoán', 'ROA', 'ROE', 'NIM'],
      },
      {
        id: 3,
        name: 'Doanh thu Dịch vụ',
        icon: '🏪',
        description: 'Tổng doanh thu phí dịch vụ, các loại phí ngân hàng',
        indicators: ['Doanh thu phí DV', 'Phí chuyển khoản', 'Phí thẻ', 'Phí bảo hiểm'],
      },
      {
        id: 4,
        name: 'Thẻ & Sản phẩm',
        icon: '💳',
        description: 'Số thẻ phát hành, các sản phẩm ngân hàng điện tử',
        indicators: ['Số thẻ phát hành', 'Giao dịch ATM', 'Internet Banking', 'Mobile Banking'],
      },
      {
        id: 5,
        name: 'Thu nợ & Rủi ro',
        icon: '⚠️',
        description: 'Thu nợ đã XLRR, các chỉ tiêu quản lý rủi ro tín dụng',
        indicators: ['Thu nợ XLRR', 'Nợ xấu', 'Tỷ lệ NPL', 'Dự phòng rủi ro'],
      },
      {
        id: 6,
        name: 'Khách hàng & Tiếp thị',
        icon: '👥',
        description: 'Số lượng khách hàng mới, chỉ tiêu marketing và bán hàng',
        indicators: ['KH cá nhân mới', 'KH doanh nghiệp mới', 'Sản phẩm cross-sell'],
      },
    ]
  }

  /**
   * Validate file before upload
   * @param {File} file - File to validate
   * @returns {Object} Validation result
   */
  validateFile(file) {
    const maxSize = 10 * 1024 * 1024 // 10MB
    const allowedTypes = [
      'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', // .xlsx
      'application/vnd.ms-excel', // .xls
      'text/csv', // .csv
      'application/pdf', // .pdf
    ]

    const allowedExtensions = /\.(xlsx|xls|csv|pdf)$/i

    if (!allowedTypes.includes(file.type) && !allowedExtensions.test(file.name)) {
      return {
        valid: false,
        error: 'File type not supported. Only Excel, CSV, and PDF files are allowed.',
      }
    }

    if (file.size > maxSize) {
      return {
        valid: false,
        error: 'File size exceeds 10MB limit.',
      }
    }

    return { valid: true }
  }

  /**
   * Format file size for display
   * @param {number} bytes - File size in bytes
   * @returns {string} Formatted file size
   */
  formatFileSize(bytes) {
    if (bytes === 0) return '0 Bytes'

    const k = 1024
    const sizes = ['Bytes', 'KB', 'MB', 'GB']
    const i = Math.floor(Math.log(bytes) / Math.log(k))

    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
  }

  /**
   * Get file type icon
   * @param {string} mimeType - MIME type of the file
   * @returns {string} Icon emoji
   */
  getFileIcon(mimeType) {
    if (mimeType.includes('excel') || mimeType.includes('spreadsheet')) {
      return '📊'
    } else if (mimeType.includes('csv')) {
      return '📋'
    } else if (mimeType.includes('pdf')) {
      return '📄'
    }
    return '📁'
  }

  // 🎯 SMART UPLOAD - Automatically choose best upload method based on file size
  async uploadFileSmart(file, category = 'General', onProgress = null) {
    const fileSizeMB = file.size / 1024 / 1024

    try {
      if (fileSizeMB < 50) {
        // Small files: regular upload
        console.log(`📤 [SMART_UPLOAD] Using regular upload for ${file.name} (${fileSizeMB.toFixed(2)} MB)`)
        return await this.uploadFile(file, category, onProgress)
      } else if (fileSizeMB < 200) {
        // Medium files: streaming upload
        console.log(`🚀 [SMART_UPLOAD] Using streaming upload for ${file.name} (${fileSizeMB.toFixed(2)} MB)`)
        return await this.uploadFileStreaming(file, onProgress)
      } else if (fileSizeMB < 500) {
        // Large files: chunked upload
        console.log(`🔄 [SMART_UPLOAD] Using chunked upload for ${file.name} (${fileSizeMB.toFixed(2)} MB)`)
        return await this.uploadFileChunked(file, { onProgress })
      } else {
        // Very large files: parallel chunked upload
        console.log(`⚡ [SMART_UPLOAD] Using parallel upload for ${file.name} (${fileSizeMB.toFixed(2)} MB)`)
        return await this.uploadFileParallel(file, { onProgress })
      }
    } catch (error) {
      console.error('❌ [SMART_UPLOAD] Smart upload error:', error)
      throw error
    }
  }

  // 🚀 STREAMING UPLOAD - For large files (50MB+)
  async uploadFileStreaming(file, onProgress = null) {
    try {
      const formData = new FormData()
      formData.append('file', file)

      const config = {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
        timeout: 30 * 60 * 1000, // 30 minutes for streaming
        maxContentLength: Infinity,
        maxBodyLength: Infinity,
        onUploadProgress: progressEvent => {
          if (onProgress && progressEvent.total) {
            const percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total)
            onProgress(percentCompleted, progressEvent.loaded, progressEvent.total)
          }
        },
      }

      console.log(
        `🚀 [STREAMING_UPLOAD] Starting streaming upload: ${file.name} (${(file.size / 1024 / 1024).toFixed(2)} MB)`
      )

      const response = await apiClient.post('/DirectImport/stream', formData, config)
      return response.data
    } catch (error) {
      console.error('❌ [STREAMING_UPLOAD] Upload error:', error)
      throw error
    }
  }

  // 🔄 CHUNKED UPLOAD - For very large files (100MB+) with resume capability
  async uploadFileChunked(file, options = {}) {
    try {
      const chunkedService = new ChunkedUploadService('/api/DirectImport')

      const uploadOptions = {
        onProgress: options.onProgress || (() => {}),
        onChunkProgress: options.onChunkProgress || (() => {}),
        chunkSize: options.chunkSize || 5 * 1024 * 1024, // 5MB chunks
      }

      console.log(
        `🔄 [CHUNKED_UPLOAD] Starting chunked upload: ${file.name} (${(file.size / 1024 / 1024).toFixed(2)} MB)`
      )

      const result = await chunkedService.uploadFileChunked(file, uploadOptions)
      return result
    } catch (error) {
      console.error('❌ [CHUNKED_UPLOAD] Upload error:', error)
      throw error
    }
  }

  // 🔄 PARALLEL CHUNKED UPLOAD - For extremely large files with parallel processing
  async uploadFileParallel(file, options = {}) {
    try {
      const chunkedService = new ChunkedUploadService('/api/DirectImport')

      const uploadOptions = {
        onProgress: options.onProgress || (() => {}),
        maxConcurrent: options.maxConcurrent || 3,
        chunkSize: options.chunkSize || 10 * 1024 * 1024, // 10MB chunks for parallel
      }

      console.log(
        `🔄 [PARALLEL_UPLOAD] Starting parallel upload: ${file.name} (${(file.size / 1024 / 1024).toFixed(2)} MB)`
      )

      const result = await chunkedService.uploadFileParallel(file, uploadOptions)
      return result
    } catch (error) {
      console.error('❌ [PARALLEL_UPLOAD] Upload error:', error)
      throw error
    }
  }

  // Resume interrupted chunked upload
  async resumeUpload(sessionId, file, options = {}) {
    try {
      const chunkedService = new ChunkedUploadService('/api/DirectImport')

      console.log(`🔄 [RESUME_UPLOAD] Resuming upload for session: ${sessionId}`)

      const result = await chunkedService.resumeUpload(sessionId, file, options)
      return result
    } catch (error) {
      console.error('❌ [RESUME_UPLOAD] Resume error:', error)
      throw error
    }
  }

  // Cancel active upload
  async cancelUpload(uploadId) {
    try {
      const chunkedService = new ChunkedUploadService('/api/DirectImport')
      await chunkedService.cancelUpload(uploadId)

      console.log(`🚫 [CANCEL_UPLOAD] Cancelled upload: ${uploadId}`)
    } catch (error) {
      console.error('❌ [CANCEL_UPLOAD] Cancel error:', error)
      throw error
    }
  }

  // Get active uploads
  getActiveUploads() {
    const chunkedService = new ChunkedUploadService('/api/DirectImport')
    return chunkedService.getActiveUploads()
  }
}

// Create and export singleton instance
const dataImportService = new DataImportService()

export default dataImportService
export { DataImportService }
