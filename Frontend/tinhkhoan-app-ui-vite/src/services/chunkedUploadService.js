/**
 * 🚀 CHUNKED UPLOAD SERVICE - Upload file lớn bằng chunks
 * Tối ưu cho file hàng trăm MB với resumable upload
 */

const CHUNK_SIZE = 5 * 1024 * 1024 // 5MB chunks
const MAX_RETRIES = 3
const RETRY_DELAY = 1000 // 1 second

class ChunkedUploadService {
  constructor(apiUrl = '/api/DirectImport') {
    this.apiUrl = apiUrl
    this.activeUploads = new Map()
  }

  /**
   * 🔄 Upload file bằng chunks với progress tracking
   */
  async uploadFileChunked(file, options = {}) {
    const { onProgress = () => {}, onChunkProgress = () => {}, dataType = null, chunkSize = CHUNK_SIZE } = options

    const uploadId = this.generateUploadId()
    const totalChunks = Math.ceil(file.size / chunkSize)

    console.log(`🚀 [CHUNKED_UPLOAD] Starting upload: ${file.name}, Size: ${file.size}, Chunks: ${totalChunks}`)

    try {
      // Tạo upload session
      const sessionResult = await this.createUploadSession({
        fileName: file.name,
        fileSize: file.size,
        totalChunks,
        dataType: dataType || this.detectDataType(file.name),
      })

      const { sessionId } = sessionResult
      this.activeUploads.set(uploadId, { sessionId, file, totalChunks, uploadedChunks: 0 })

      let uploadedBytes = 0
      const uploadedChunks = new Set()

      // Upload từng chunk
      for (let chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++) {
        const start = chunkIndex * chunkSize
        const end = Math.min(start + chunkSize, file.size)
        const chunk = file.slice(start, end)

        let retries = 0
        let success = false

        while (!success && retries < MAX_RETRIES) {
          try {
            await this.uploadChunk(sessionId, chunkIndex, chunk, {
              onProgress: progress => {
                onChunkProgress(chunkIndex, progress)
              },
            })

            uploadedChunks.add(chunkIndex)
            uploadedBytes = uploadedChunks.size * chunkSize
            if (chunkIndex === totalChunks - 1) {
              uploadedBytes = file.size // Last chunk might be smaller
            }

            success = true

            // Update overall progress
            const overallProgress = (uploadedBytes / file.size) * 100
            onProgress(overallProgress)

            console.log(`✅ [CHUNK_${chunkIndex}] Uploaded successfully (${overallProgress.toFixed(1)}%)`)
          } catch (error) {
            retries++
            console.warn(`⚠️ [CHUNK_${chunkIndex}] Retry ${retries}/${MAX_RETRIES}: ${error.message}`)

            if (retries < MAX_RETRIES) {
              await this.delay(RETRY_DELAY * retries)
            } else {
              throw new Error(`Failed to upload chunk ${chunkIndex} after ${MAX_RETRIES} retries`)
            }
          }
        }
      }

      // Finalize upload
      const result = await this.finalizeUpload(sessionId)
      this.activeUploads.delete(uploadId)

      console.log(`🎉 [CHUNKED_UPLOAD] Completed: ${file.name}, Records: ${result.processedRecords}`)

      return {
        success: true,
        uploadId,
        sessionId,
        ...result,
      }
    } catch (error) {
      this.activeUploads.delete(uploadId)
      console.error(`❌ [CHUNKED_UPLOAD] Error:`, error)
      throw error
    }
  }

  /**
   * 📊 PARALLEL CHUNKED UPLOAD - Upload nhiều chunks song song
   */
  async uploadFileParallel(file, options = {}) {
    const {
      onProgress = () => {},
      maxConcurrent = 3, // Số chunks upload song song
      chunkSize = CHUNK_SIZE,
    } = options

    const totalChunks = Math.ceil(file.size / chunkSize)
    const uploadPromises = []
    const semaphore = new Semaphore(maxConcurrent)

    console.log(
      `🔄 [PARALLEL_UPLOAD] Starting parallel upload: ${totalChunks} chunks, max concurrent: ${maxConcurrent}`
    )

    // Create upload session
    const sessionResult = await this.createUploadSession({
      fileName: file.name,
      fileSize: file.size,
      totalChunks,
      dataType: this.detectDataType(file.name),
    })

    const { sessionId } = sessionResult
    let completedChunks = 0

    // Create upload promises for all chunks
    for (let chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++) {
      const uploadPromise = semaphore.acquire().then(async () => {
        try {
          const start = chunkIndex * chunkSize
          const end = Math.min(start + chunkSize, file.size)
          const chunk = file.slice(start, end)

          await this.uploadChunkWithRetry(sessionId, chunkIndex, chunk)

          completedChunks++
          const progress = (completedChunks / totalChunks) * 100
          onProgress(progress)

          console.log(`✅ [PARALLEL_CHUNK_${chunkIndex}] Uploaded (${progress.toFixed(1)}%)`)
        } finally {
          semaphore.release()
        }
      })

      uploadPromises.push(uploadPromise)
    }

    await Promise.all(uploadPromises)

    // Finalize upload
    const result = await this.finalizeUpload(sessionId)

    console.log(`🎉 [PARALLEL_UPLOAD] Completed: ${file.name}`)

    return {
      success: true,
      sessionId,
      ...result,
    }
  }

  /**
   * 🔄 Resume upload từ chunk đã upload trước đó
   */
  async resumeUpload(sessionId, file, options = {}) {
    const { onProgress = () => {} } = options

    try {
      // Lấy thông tin chunks đã upload
      const uploadInfo = await this.getUploadInfo(sessionId)
      const { uploadedChunks, totalChunks } = uploadInfo

      const chunkSize = Math.ceil(file.size / totalChunks)
      const remainingChunks = []

      for (let i = 0; i < totalChunks; i++) {
        if (!uploadedChunks.includes(i)) {
          remainingChunks.push(i)
        }
      }

      console.log(`🔄 [RESUME_UPLOAD] Resuming: ${remainingChunks.length}/${totalChunks} chunks remaining`)

      let uploadedBytes = uploadedChunks.length * chunkSize

      // Upload remaining chunks
      for (const chunkIndex of remainingChunks) {
        const start = chunkIndex * chunkSize
        const end = Math.min(start + chunkSize, file.size)
        const chunk = file.slice(start, end)

        await this.uploadChunkWithRetry(sessionId, chunkIndex, chunk)

        uploadedBytes += chunk.size
        const progress = (uploadedBytes / file.size) * 100
        onProgress(progress)
      }

      // Finalize upload
      const result = await this.finalizeUpload(sessionId)

      return {
        success: true,
        sessionId,
        resumed: true,
        ...result,
      }
    } catch (error) {
      console.error(`❌ [RESUME_UPLOAD] Error:`, error)
      throw error
    }
  }

  /**
   * 🆔 Create upload session
   */
  async createUploadSession(metadata) {
    const response = await fetch(`${this.apiUrl}/create-session`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(metadata),
    })

    if (!response.ok) {
      throw new Error(`Failed to create upload session: ${response.statusText}`)
    }

    return await response.json()
  }

  /**
   * 📤 Upload single chunk
   */
  async uploadChunk(sessionId, chunkIndex, chunk, options = {}) {
    const { onProgress = () => {} } = options

    const formData = new FormData()
    formData.append('sessionId', sessionId)
    formData.append('chunkIndex', chunkIndex)
    formData.append('chunk', chunk)

    const response = await fetch(`${this.apiUrl}/upload-chunk`, {
      method: 'POST',
      body: formData,
      onUploadProgress: event => {
        if (event.lengthComputable) {
          const progress = (event.loaded / event.total) * 100
          onProgress(progress)
        }
      },
    })

    if (!response.ok) {
      throw new Error(`Failed to upload chunk ${chunkIndex}: ${response.statusText}`)
    }

    return await response.json()
  }

  /**
   * 🔄 Upload chunk with retry logic
   */
  async uploadChunkWithRetry(sessionId, chunkIndex, chunk) {
    let retries = 0

    while (retries < MAX_RETRIES) {
      try {
        await this.uploadChunk(sessionId, chunkIndex, chunk)
        return
      } catch (error) {
        retries++
        if (retries < MAX_RETRIES) {
          await this.delay(RETRY_DELAY * retries)
        } else {
          throw error
        }
      }
    }
  }

  /**
   * ✅ Finalize upload session
   */
  async finalizeUpload(sessionId) {
    const response = await fetch(`${this.apiUrl}/finalize/${sessionId}`, {
      method: 'POST',
    })

    if (!response.ok) {
      throw new Error(`Failed to finalize upload: ${response.statusText}`)
    }

    return await response.json()
  }

  /**
   * 📊 Get upload info (for resume)
   */
  async getUploadInfo(sessionId) {
    const response = await fetch(`${this.apiUrl}/upload-info/${sessionId}`)

    if (!response.ok) {
      throw new Error(`Failed to get upload info: ${response.statusText}`)
    }

    return await response.json()
  }

  /**
   * 🎯 Detect data type from filename
   */
  detectDataType(fileName) {
    const upperName = fileName.toUpperCase()

    if (upperName.includes('DP01')) return 'DP01'
    if (upperName.includes('LN01')) return 'LN01'
    if (upperName.includes('LN03')) return 'LN03'
    if (upperName.includes('GL01')) return 'GL01'
    if (upperName.includes('GL41')) return 'GL41'
    if (upperName.includes('CA01')) return 'CA01'
    if (upperName.includes('RR01')) return 'RR01'
    if (upperName.includes('TR01')) return 'TR01'

    return 'DP01' // Default
  }

  /**
   * 🆔 Generate unique upload ID
   */
  generateUploadId() {
    return `upload_${Date.now()}_${Math.random().toString(36).substring(2, 9)}`
  }

  /**
   * ⏱️ Delay utility
   */
  delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms))
  }

  /**
   * 🚫 Cancel active upload
   */
  async cancelUpload(uploadId) {
    const upload = this.activeUploads.get(uploadId)
    if (upload) {
      try {
        await fetch(`${this.apiUrl}/cancel/${upload.sessionId}`, {
          method: 'DELETE',
        })
      } catch (error) {
        console.warn('Failed to cancel upload on server:', error)
      }

      this.activeUploads.delete(uploadId)
      console.log(`🚫 [CANCEL_UPLOAD] Cancelled: ${uploadId}`)
    }
  }

  /**
   * 📊 Get active uploads
   */
  getActiveUploads() {
    return Array.from(this.activeUploads.entries()).map(([id, info]) => ({
      id,
      ...info,
    }))
  }
}

/**
 * 🔒 Semaphore for limiting concurrent operations
 */
class Semaphore {
  constructor(maxConcurrency) {
    this.maxConcurrency = maxConcurrency
    this.currentConcurrency = 0
    this.queue = []
  }

  async acquire() {
    return new Promise(resolve => {
      if (this.currentConcurrency < this.maxConcurrency) {
        this.currentConcurrency++
        resolve()
      } else {
        this.queue.push(resolve)
      }
    })
  }

  release() {
    this.currentConcurrency--
    if (this.queue.length > 0) {
      const next = this.queue.shift()
      this.currentConcurrency++
      next()
    }
  }
}

export default ChunkedUploadService
