<template>
  <div class="streaming-export-demo">
    <h2>🚀 Streaming Export Demo</h2>
    
    <!-- Export Controls -->
    <div class="export-controls">
      <div class="control-group">
        <label>Export Type:</label>
        <select v-model="exportType" class="form-control">
          <option value="employees">Employees</option>
          <option value="rawdata">Raw Data</option>
        </select>
      </div>
      
      <div class="control-group" v-if="exportType === 'employees'">
        <label>Unit Filter:</label>
        <input 
          v-model="unitId" 
          type="number" 
          placeholder="Unit ID (optional)" 
          class="form-control"
        />
      </div>
      
      <div class="control-group" v-if="exportType === 'rawdata'">
        <label>Import ID:</label>
        <input 
          v-model="importId" 
          type="number" 
          placeholder="Import ID (optional)" 
          class="form-control"
        />
      </div>
      
      <div class="control-group">
        <label>Format:</label>
        <select v-model="format" class="form-control">
          <option value="excel">Excel (.xlsx)</option>
          <option value="csv">CSV (.csv)</option>
        </select>
      </div>
      
      <div class="action-buttons">
        <button 
          @click="startStreamingExport" 
          :disabled="isExporting" 
          class="btn btn-primary"
        >
          {{ isExporting ? 'Exporting...' : 'Start Streaming Export' }}
        </button>
        
        <button 
          @click="downloadDirectly" 
          :disabled="isExporting" 
          class="btn btn-success"
        >
          Download Directly
        </button>
        
        <button 
          @click="stopExport" 
          :disabled="!isExporting" 
          class="btn btn-danger"
        >
          Cancel Export
        </button>
      </div>
    </div>

    <!-- Progress Display -->
    <div v-if="currentProgress" class="progress-section">
      <h3>Export Progress</h3>
      
      <!-- Progress Bar -->
      <div class="progress-bar-container">
        <div class="progress-bar">
          <div 
            class="progress-fill" 
            :style="{ width: currentProgress.percentComplete + '%' }"
            :class="{ 
              'error': currentProgress.hasError,
              'completed': currentProgress.isCompleted && !currentProgress.hasError
            }"
          ></div>
        </div>
        <span class="progress-text">
          {{ Math.round(currentProgress.percentComplete) }}%
        </span>
      </div>
      
      <!-- Progress Details -->
      <div class="progress-details">
        <div class="detail-row">
          <span class="label">Stage:</span>
          <span class="value" :class="currentProgress.stage.toLowerCase()">
            {{ currentProgress.stage }}
          </span>
        </div>
        
        <div class="detail-row">
          <span class="label">Records:</span>
          <span class="value">
            {{ currentProgress.processedRecords.toLocaleString() }} / 
            {{ currentProgress.totalRecords.toLocaleString() }}
          </span>
        </div>
        
        <div class="detail-row">
          <span class="label">Current Batch:</span>
          <span class="value">{{ currentProgress.currentBatch }}</span>
        </div>
        
        <div class="detail-row">
          <span class="label">Elapsed Time:</span>
          <span class="value">{{ formatDuration(currentProgress.elapsedTime) }}</span>
        </div>
        
        <div class="detail-row" v-if="currentProgress.estimatedTimeRemaining">
          <span class="label">ETA:</span>
          <span class="value">{{ formatDuration(currentProgress.estimatedTimeRemaining) }}</span>
        </div>
        
        <div class="detail-row" v-if="currentProgress.hasError">
          <span class="label">Error:</span>
          <span class="value error">{{ currentProgress.errorMessage }}</span>
        </div>
      </div>
      
      <!-- Current Data Preview -->
      <div v-if="currentProgress.currentData && currentProgress.currentData.length" class="data-preview">
        <h4>Current Batch Preview:</h4>
        <pre>{{ JSON.stringify(currentProgress.currentData, null, 2) }}</pre>
      </div>
    </div>

    <!-- Export History -->
    <div class="export-history">
      <h3>Export History</h3>
      <div v-if="exportHistory.length === 0" class="no-history">
        No exports yet
      </div>
      <div v-else class="history-list">
        <div 
          v-for="(export_, index) in exportHistory" 
          :key="index" 
          class="history-item"
          :class="{ 
            'completed': export_.isCompleted && !export_.hasError,
            'error': export_.hasError
          }"
        >
          <div class="history-header">
            <span class="export-type">{{ export_.type }}</span>
            <span class="export-time">{{ new Date(export_.startTime).toLocaleString() }}</span>
            <span class="export-status">{{ export_.stage }}</span>
          </div>
          <div class="history-details">
            <span>{{ export_.processedRecords }} / {{ export_.totalRecords }} records</span>
            <span>{{ formatDuration(export_.elapsedTime) }}</span>
            <span v-if="export_.hasError" class="error">{{ export_.errorMessage }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive } from 'vue'

export default {
  name: 'StreamingExportDemo',
  setup() {
    const exportType = ref('employees')
    const unitId = ref(null)
    const importId = ref(null)
    const format = ref('excel')
    const isExporting = ref(false)
    const currentProgress = ref(null)
    const exportHistory = ref([])
    const abortController = ref(null)

    const startStreamingExport = async () => {
      if (isExporting.value) return

      isExporting.value = true
      currentProgress.value = null
      abortController.value = new AbortController()

      try {
        const baseUrl = 'http://localhost:5123/api/StreamingExport'
        let url = `${baseUrl}/${exportType.value}/stream?format=${format.value}`
        
        if (exportType.value === 'employees' && unitId.value) {
          url += `&unitId=${unitId.value}`
        } else if (exportType.value === 'rawdata' && importId.value) {
          url += `&importId=${importId.value}`
        }

        const response = await fetch(url, {
          signal: abortController.value.signal,
          headers: {
            'Accept': 'application/json',
          }
        })

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`)
        }

        const reader = response.body.getReader()
        const decoder = new TextDecoder()
        let buffer = ''

        while (true) {
          const { done, value } = await reader.read()
          
          if (done) break

          buffer += decoder.decode(value, { stream: true })
          
          // Process complete JSON objects
          const lines = buffer.split('\n')
          buffer = lines.pop() || '' // Keep incomplete line in buffer
          
          for (const line of lines) {
            if (line.trim()) {
              try {
                const progress = JSON.parse(line)
                currentProgress.value = progress
                
                if (progress.isCompleted) {
                  addToHistory(progress)
                  isExporting.value = false
                  break
                }
              } catch (e) {
                console.warn('Failed to parse progress line:', line, e)
              }
            }
          }
        }
      } catch (error) {
        console.error('Streaming export error:', error)
        currentProgress.value = {
          stage: 'Error',
          hasError: true,
          errorMessage: error.message,
          isCompleted: true
        }
        isExporting.value = false
      }
    }

    const downloadDirectly = async () => {
      try {
        const baseUrl = 'http://localhost:5123/api/StreamingExport'
        let url = `${baseUrl}/${exportType.value}/download?format=${format.value}`
        
        if (exportType.value === 'employees' && unitId.value) {
          url += `&unitId=${unitId.value}`
        } else if (exportType.value === 'rawdata' && importId.value) {
          url += `&importId=${importId.value}`
        }

        const response = await fetch(url)
        
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`)
        }

        const blob = await response.blob()
        const downloadUrl = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = downloadUrl
        
        // Get filename from Content-Disposition header or use default
        const disposition = response.headers.get('Content-Disposition')
        let filename = `export_${exportType.value}_${Date.now()}.${format.value === 'excel' ? 'xlsx' : 'csv'}`
        
        if (disposition) {
          const filenameMatch = disposition.match(/filename="?([^"]+)"?/)
          if (filenameMatch) {
            filename = filenameMatch[1]
          }
        }
        
        link.download = filename
        document.body.appendChild(link)
        link.click()
        document.body.removeChild(link)
        window.URL.revokeObjectURL(downloadUrl)
        
        console.log('Download completed:', filename)
      } catch (error) {
        console.error('Download error:', error)
        alert('Download failed: ' + error.message)
      }
    }

    const stopExport = () => {
      if (abortController.value) {
        abortController.value.abort()
        abortController.value = null
      }
      isExporting.value = false
      currentProgress.value = {
        stage: 'Cancelled',
        hasError: true,
        errorMessage: 'Export cancelled by user',
        isCompleted: true
      }
    }

    const addToHistory = (progress) => {
      exportHistory.value.unshift({
        type: exportType.value,
        startTime: new Date().toISOString(),
        stage: progress.stage,
        totalRecords: progress.totalRecords,
        processedRecords: progress.processedRecords,
        elapsedTime: progress.elapsedTime,
        isCompleted: progress.isCompleted,
        hasError: progress.hasError,
        errorMessage: progress.errorMessage
      })
      
      // Keep only last 10 exports
      if (exportHistory.value.length > 10) {
        exportHistory.value = exportHistory.value.slice(0, 10)
      }
    }

    const formatDuration = (duration) => {
      if (!duration) return '00:00:00'
      
      // Parse duration string format "HH:MM:SS.mmm" or TimeSpan format
      let totalSeconds = 0
      
      if (typeof duration === 'string') {
        const parts = duration.split(':')
        if (parts.length >= 3) {
          totalSeconds = parseInt(parts[0]) * 3600 + parseInt(parts[1]) * 60 + parseFloat(parts[2])
        }
      } else if (typeof duration === 'object' && duration.totalSeconds) {
        totalSeconds = duration.totalSeconds
      }
      
      const hours = Math.floor(totalSeconds / 3600)
      const minutes = Math.floor((totalSeconds % 3600) / 60)
      const seconds = Math.floor(totalSeconds % 60)
      
      return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`
    }

    return {
      exportType,
      unitId,
      importId,
      format,
      isExporting,
      currentProgress,
      exportHistory,
      startStreamingExport,
      downloadDirectly,
      stopExport,
      formatDuration
    }
  }
}
</script>

<style scoped>
.streaming-export-demo {
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

.export-controls {
  background: #f8f9fa;
  padding: 20px;
  border-radius: 8px;
  margin-bottom: 20px;
}

.control-group {
  display: flex;
  align-items: center;
  margin-bottom: 15px;
  gap: 10px;
}

.control-group label {
  font-weight: bold;
  min-width: 100px;
}

.form-control {
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.action-buttons {
  display: flex;
  gap: 10px;
  margin-top: 20px;
}

.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-primary {
  background: #007bff;
  color: white;
}

.btn-success {
  background: #28a745;
  color: white;
}

.btn-danger {
  background: #dc3545;
  color: white;
}

.progress-section {
  background: white;
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 20px;
}

.progress-bar-container {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 20px;
}

.progress-bar {
  flex: 1;
  height: 20px;
  background: #e9ecef;
  border-radius: 10px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  background: #28a745;
  transition: width 0.3s ease;
}

.progress-fill.error {
  background: #dc3545;
}

.progress-fill.completed {
  background: #17a2b8;
}

.progress-text {
  font-weight: bold;
  min-width: 50px;
}

.progress-details {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 10px;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  padding: 5px 0;
  border-bottom: 1px solid #eee;
}

.label {
  font-weight: bold;
}

.value.processing {
  color: #007bff;
}

.value.completed {
  color: #28a745;
}

.value.error {
  color: #dc3545;
}

.data-preview {
  margin-top: 20px;
  background: #f8f9fa;
  padding: 15px;
  border-radius: 4px;
}

.data-preview pre {
  font-size: 12px;
  max-height: 200px;
  overflow-y: auto;
}

.export-history {
  background: white;
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 20px;
}

.no-history {
  text-align: center;
  color: #6c757d;
  padding: 20px;
}

.history-list {
  max-height: 400px;
  overflow-y: auto;
}

.history-item {
  border: 1px solid #eee;
  border-radius: 4px;
  padding: 15px;
  margin-bottom: 10px;
}

.history-item.completed {
  border-left: 4px solid #28a745;
}

.history-item.error {
  border-left: 4px solid #dc3545;
}

.history-header {
  display: flex;
  justify-content: space-between;
  font-weight: bold;
  margin-bottom: 5px;
}

.history-details {
  display: flex;
  justify-content: space-between;
  font-size: 14px;
  color: #6c757d;
}

.export-type {
  text-transform: capitalize;
}

.export-status {
  text-transform: uppercase;
  font-size: 12px;
}
</style>
