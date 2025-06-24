<template>
  <div class="raw-data-warehouse">
    <!-- Header Section -->
    <div class="header-section">
      <h1>🏦 KHO DỮ LIỆU THÔ</h1>
      <p class="subtitle">Hệ thống quản lý và import dữ liệu nghiệp vụ ngân hàng chuyên nghiệp</p>
    </div>

    <!-- Thông báo -->
    <div v-if="errorMessage" class="alert alert-error">
      <span class="alert-icon">⚠️</span>
      {{ errorMessage }}
      <button @click="clearMessage" class="alert-close">×</button>
    </div>

    <div v-if="successMessage" class="alert alert-success">
      <span class="alert-icon">✅</span>
      {{ successMessage }}
      <button @click="clearMessage" class="alert-close">×</button>
    </div>

    <!-- Loading indicator -->
    <div v-if="loading" class="loading-section">
      <div class="loading-spinner"></div>
      <p>{{ loadingMessage || 'Đang xử lý dữ liệu...' }}</p>
    </div>

    <!-- Control Panel -->
    <div class="control-panel">
      <div class="date-control-section">
        <h3 class="agribank-date-title">🗓️ Chọn ngày sao kê</h3>
        <div class="date-controls-enhanced">
          <div class="date-range-group">
            <div class="date-input-group">
              <label>Từ ngày:</label>
              <input 
                v-model="selectedFromDate" 
                type="date" 
                class="date-input agribank-date-input"
              />
            </div>
            <div class="date-input-group">
              <label>Đến ngày:</label>
              <input 
                v-model="selectedToDate" 
                type="date" 
                class="date-input agribank-date-input"
              />
            </div>
          </div>
          <div class="date-actions-group">
            <button @click="applyDateFilter" class="btn-filter agribank-btn-filter" :disabled="!selectedFromDate">
              🔍 Lọc theo ngày
            </button>
            <button @click="clearDateFilter" class="btn-clear agribank-btn-clear">
              🗑️ Xóa bộ lọc
            </button>
          </div>
        </div>
      </div>

      <div class="bulk-actions-section">
        <h3>⚡ Thao tác hàng loạt</h3>
        <div class="bulk-actions">
          <button @click="clearAllData" class="btn-clear-all" :disabled="loading">
            🗑️ Xóa toàn bộ dữ liệu
          </button>
          <button @click="refreshAllData" class="btn-refresh" :disabled="loading">
            🔄 Tải lại dữ liệu
          </button>
          <button @click="debugRecalculateStats" class="btn-debug" :disabled="loading" title="Debug: Force recalculate stats">
            🔧 Debug Stats
          </button>
        </div>
      </div>
    </div>

    <!-- Data Types List -->
    <div class="data-types-section agribank-section">
      <div class="section-header agribank-header">
        <div class="header-content">
          <div class="agribank-logo-header"></div>
          <div class="header-text">
            <h2>📊 BẢNG QUẢN LÝ DỮ LIỆU NGHIỆP VỤ</h2>
            <p>Theo dõi và quản lý tất cả loại dữ liệu của hệ thống Agribank Lai Châu</p>
          </div>
        </div>
        <div class="agribank-brand-line"></div>
      </div>

      <div class="data-types-table agribank-table">
        <table class="enhanced-table">
          <thead class="agribank-thead">
            <tr>
              <th class="col-datatype">Loại dữ liệu</th>
              <th class="col-description">Mô tả chi tiết</th>
              <th class="col-records">Tổng records</th>
              <th class="col-updated">Cập nhật cuối</th>
              <th class="col-actions">Thao tác nghiệp vụ</th>
            </tr>
          </thead>
          <tbody class="agribank-tbody">
            <tr v-for="(dataType, key) in sortedDataTypeDefinitions" :key="key" class="data-row enhanced-row">
              <td class="col-datatype">
                <div class="data-type-info enhanced-datatype">
                  <span class="data-type-icon agribank-icon">{{ dataType.icon }}</span>
                  <div class="datatype-details">
                    <strong class="datatype-name">{{ key }}</strong>
                    <span class="datatype-category">{{ getCategoryName(key) }}</span>
                  </div>
                </div>
              </td>
              <td class="col-description description-cell enhanced-description">
                <span class="description-text">{{ dataType.description }}</span>
              </td>
              <td class="col-records records-cell enhanced-records">
                <div class="records-info">
                  <span class="records-count agribank-number">{{ formatRecordCount(getDataTypeStats(key).totalRecords) }}</span>
                  <span class="records-label">bản ghi</span>
                </div>
              </td>
              <td class="col-updated last-update-cell enhanced-lastupdate">
                <span class="update-text">{{ formatDateTime(getDataTypeStats(key).lastUpdate) }}</span>
              </td>
              <td class="actions-cell">
                <button 
                  @click="viewDataType(key)" 
                  class="btn-action btn-view btn-icon-only"
                  title="Xem dữ liệu import"
                  :disabled="false"
                >
                  👁️
                </button>
                <button 
                  @click="viewRawDataFromTable(key)" 
                  class="btn-action btn-raw-view btn-icon-only"
                  title="Xem dữ liệu thô từ bảng"
                  :disabled="!selectedFromDate"
                >
                  📊
                </button>
                <button 
                  @click="openImportModal(key)" 
                  class="btn-action btn-import btn-icon-only"
                  :style="{ backgroundColor: getDataTypeColor(key) }"
                  title="Import dữ liệu"
                >
                  📤
                </button>
                <button 
                  @click="deleteDataTypeByDate(key)" 
                  class="btn-action btn-delete btn-icon-only"
                  title="Xóa theo ngày đã chọn"
                  :disabled="!selectedFromDate || getDataTypeStats(key).totalRecords === 0"
                >
                  🗑️
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Import Modal đầy đủ -->
    <div v-if="showImportModal" class="modal-overlay" @click="closeImportModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>Import dữ liệu {{ selectedDataType }}</h3>
          <button @click="closeImportModal" class="modal-close">×</button>
        </div>
        <div class="modal-body">
          <!-- Form upload file -->
          <div class="import-form">
            <div class="form-group">
              <label class="form-label">Chọn file để import:</label>
              <input 
                type="file" 
                ref="fileInput"
                multiple 
                @change="handleFileSelect" 
                class="file-input"
              />
            </div>
            
            <!-- Danh sách file đã chọn -->
            <div v-if="selectedFiles.length > 0" class="selected-files">
              <h4>Files đã chọn:</h4>
              <ul class="files-list">
                <li v-for="(file, index) in selectedFiles" :key="index" class="file-item">
                  {{ file.name }} ({{ formatFileSize(file.size) }})
                  <button @click="removeFile(index)" class="btn-remove">×</button>
                </li>
              </ul>
            </div>
            
            <!-- Phần nhập mật khẩu cho file nén nếu cần -->
            <div v-if="hasArchiveFile" class="form-group">
              <label class="form-label">Mật khẩu file nén (nếu có):</label>
              <input 
                type="password" 
                v-model="archivePassword" 
                class="password-input" 
                placeholder="Nhập mật khẩu file nén..." 
              />
            </div>
            
            <!-- Ghi chú -->
            <div class="form-group">
              <label class="form-label">Ghi chú:</label>
              <textarea 
                v-model="importNotes" 
                class="notes-input" 
                placeholder="Thêm ghi chú cho lần import này..."
              ></textarea>
            </div>
          </div>
        </div>
        
        <div class="modal-footer">
          <button @click="closeImportModal" class="btn-cancel">Hủy</button>
          <button 
            @click="performImport" 
            class="btn-submit"
            :disabled="selectedFiles.length === 0 || uploading"
          >
            {{ uploading ? 'Đang xử lý...' : 'Import Dữ liệu' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import rawDataService from '@/services/rawDataService'

// Reactive state
const loading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')
const loadingMessage = ref('')

// Date filtering
const selectedFromDate = ref('')
const selectedToDate = ref('')

// Data management
const allImports = ref([])
const dataTypeStats = ref({})

// Modal state
const showImportModal = ref(false)
const selectedDataType = ref(null)
const selectedFiles = ref([])
const archivePassword = ref('')
const importNotes = ref('')
const uploading = ref(false)
const uploadProgress = ref(0)

// Data type definitions - lấy từ service
const dataTypeDefinitions = rawDataService.getDataTypeDefinitions()

// Computed properties
const sortedDataTypeDefinitions = computed(() => {
  const sorted = {}
  const sortedKeys = Object.keys(dataTypeDefinitions).sort()
  sortedKeys.forEach(key => {
    sorted[key] = dataTypeDefinitions[key]
  })
  return sorted
})

// Methods
const clearMessage = () => {
  errorMessage.value = ''
  successMessage.value = ''
}

const showError = (message) => {
  errorMessage.value = message
  setTimeout(() => {
    errorMessage.value = ''
  }, 5000)
}

const showSuccess = (message, timeout = 3000) => {
  successMessage.value = message
  setTimeout(() => {
    successMessage.value = ''
  }, timeout)
}

// Data type statistics
const getDataTypeStats = (dataType) => {
  const stats = dataTypeStats.value[dataType] || { totalRecords: 0, lastUpdate: null }
  return {
    ...stats,
    totalRecords: rawDataService.formatRecordCount(stats.totalRecords)
  }
}

const calculateDataTypeStats = () => {
  console.log('🔧 Calculating data type stats from imports:', allImports.value.length)
  const stats = {}
  
  // Initialize all data types để hiển thị 0 nếu không có dữ liệu
  Object.keys(dataTypeDefinitions).forEach(key => {
    stats[key] = { totalRecords: 0, lastUpdate: null, count: 0 }
  })
  
  // Calculate from imports
  allImports.value.forEach(imp => {
    const dataType = imp.category || imp.dataType || imp.fileType || 'UNKNOWN'
    
    if (!stats[dataType]) {
      stats[dataType] = { totalRecords: 0, lastUpdate: null, count: 0 }
    }
    
    stats[dataType].count++
    const recordCount = parseInt(imp.recordsCount) || 0
    stats[dataType].totalRecords += recordCount
    
    const importDate = imp.importDate;
    if (importDate && importDate !== "0001-01-01T00:00:00") {
      const importDateTime = new Date(importDate)
      if (!stats[dataType].lastUpdate || 
          importDateTime > new Date(stats[dataType].lastUpdate)) {
        stats[dataType].lastUpdate = importDate
      }
    }
  })
  
  dataTypeStats.value = stats
}

// Debug function
const debugRecalculateStats = () => {
  console.log('🔧 DEBUG: Manual recalculate stats')
  calculateDataTypeStats()
  showSuccess(`🔧 Debug: Recalculated stats. Check console for details.`)
}

// Date filtering methods
const applyDateFilter = async () => {
  if (!selectedFromDate.value) {
    showError('Vui lòng chọn ngày bắt đầu')
    return
  }
  showSuccess('Chức năng lọc theo ngày đang được phát triển...')
}

const clearDateFilter = () => {
  selectedFromDate.value = ''
  selectedToDate.value = ''
  showSuccess('Đã xóa bộ lọc ngày')
}

// Data management methods
const refreshAllData = async () => {
  try {
    loading.value = true
    loadingMessage.value = 'Đang tải lại dữ liệu...'
    
    console.log('🔄 Starting refresh all data...')
    
    const result = await rawDataService.getAllImports()
    console.log('📊 Raw result from getAllImports:', result)

    if (result.success) {
      allImports.value = result.data || []
      console.log('✅ Loaded imports:', allImports.value.length, 'items')
      
      calculateDataTypeStats()
      
      showSuccess(`✅ Đã tải lại dữ liệu thành công (${allImports.value.length} imports)`)
    } else {
      const errorMsg = result.error || 'Không thể tải dữ liệu'
      console.error('🔥 Chi tiết lỗi:', {
        error: result.error,
        errorCode: result.errorCode,
        errorStatus: result.errorStatus
      })
      
      if (result.fallbackData && result.fallbackData.length > 0) {
        allImports.value = result.fallbackData
        calculateDataTypeStats()
        showError(`⚠️ Chế độ Demo: ${errorMsg}`)
      } else {
        allImports.value = []
        calculateDataTypeStats()
        showError(errorMsg)
      }
    }
    
  } catch (error) {
    console.error('Error refreshing data:', error)
    showError('Có lỗi xảy ra khi tải dữ liệu')
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

const clearAllData = async () => {
  if (!confirm('⚠️ BẠN CÓ CHẮC CHẮN MUỐN XÓA TOÀN BỘ DỮ LIỆU?\n\nThao tác này sẽ xóa tất cả dữ liệu đã import và KHÔNG THỂ KHÔI PHỤC!')) {
    return
  }
  
  try {
    loading.value = true
    loadingMessage.value = 'Đang xóa toàn bộ dữ liệu...'
    
    const result = await rawDataService.clearAllData()
    if (result.success) {
      allImports.value = []
      dataTypeStats.value = {}
      
      const data = result.data || result
      const message = `✅ Đã xóa thành công ${data.recordsCleared || 0} bản ghi import`
      showSuccess(message, 5000)
      
      setTimeout(async () => {
        await refreshAllData()
        calculateDataTypeStats()
      }, 1500)
      
    } else {
      showError(result.message || result.error || 'Không thể xóa dữ liệu')
    }
    
  } catch (error) {
    console.error('❌ Error clearing all data:', error)
    showError('Có lỗi xảy ra khi xóa dữ liệu: ' + error.message)
  } finally {
    setTimeout(() => {
      loading.value = false
      loadingMessage.value = ''
    }, 2000)
  }
}

// Data type actions - stubs cho các chức năng sẽ phát triển
const viewDataType = async (dataType) => {
  showSuccess(`Chức năng xem dữ liệu ${dataType} đang được phát triển...`)
}

const deleteDataTypeByDate = async (dataType) => {
  if (!selectedFromDate.value) {
    showError('Vui lòng chọn ngày để xóa dữ liệu')
    return
  }
  showSuccess(`Chức năng xóa dữ liệu ${dataType} đang được phát triển...`)
}

const viewRawDataFromTable = async (dataType) => {
  if (!selectedFromDate.value) {
    showError('Vui lòng chọn ngày để xem dữ liệu thô')
    return
  }
  showSuccess(`Chức năng xem dữ liệu thô ${dataType} đang được phát triển...`)
}

// Import methods
const openImportModal = (dataType) => {
  selectedDataType.value = dataType
  showImportModal.value = true
  // Reset form
  selectedFiles.value = []
  archivePassword.value = ''
  importNotes.value = ''
  uploading.value = false
  uploadProgress.value = 0
}

const closeImportModal = () => {
  showImportModal.value = false
  selectedDataType.value = null
}

// Xử lý file
const handleFileSelect = (event) => {
  const files = event.target.files
  if (files.length > 0) {
    selectedFiles.value = [...selectedFiles.value, ...Array.from(files)]
  }
}

const removeFile = (index) => {
  selectedFiles.value.splice(index, 1)
}

const isArchiveFile = (fileName) => {
  const ext = fileName.split('.').pop().toLowerCase()
  return ['zip', 'rar', '7z'].includes(ext)
}

const formatFileSize = (size) => {
  if (size < 1024) return size + ' B'
  else if (size < 1024 * 1024) return (size / 1024).toFixed(2) + ' KB'
  else if (size < 1024 * 1024 * 1024) return (size / (1024 * 1024)).toFixed(2) + ' MB'
  else return (size / (1024 * 1024 * 1024)).toFixed(2) + ' GB'
}

// Thực hiện import
const performImport = async () => {
  if (selectedFiles.value.length === 0) {
    showError('Vui lòng chọn ít nhất một file để import')
    return
  }

  try {
    uploading.value = true
    uploadProgress.value = 0
    
    const options = {
      archivePassword: archivePassword.value,
      notes: importNotes.value,
      onProgress: (progress) => {
        uploadProgress.value = progress
      }
    }
    
    const result = await rawDataService.importData(selectedDataType.value, selectedFiles.value, options)
    
    if (result.success) {
      showSuccess(`Import thành công ${result.processedFiles || selectedFiles.value.length} file`)
      closeImportModal()
      await refreshAllData() // Tải lại dữ liệu sau khi import
    } else {
      showError(`Lỗi khi import: ${result.error}`)
    }
  } catch (error) {
    console.error('❌ Lỗi khi import:', error)
    showError(`Lỗi khi import: ${error.message || 'Không xác định'}`)
  } finally {
    uploading.value = false
  }
}

// Utility methods
const getDataTypeColor = (dataType) => {
  return rawDataService.getDataTypeColor(dataType)
}

const formatRecordCount = (count) => {
  return rawDataService.formatRecordCount(count)
}

const formatDateTime = (dateString) => {
  if (!dateString || dateString === "0001-01-01T00:00:00") {
    return 'Chưa có dữ liệu'
  }
  
  try {
    const date = new Date(dateString)
    if (isNaN(date.getTime())) {
      return 'Ngày không hợp lệ'
    }
    
    const day = String(date.getDate()).padStart(2, '0')
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const year = date.getFullYear()
    const hours = String(date.getHours()).padStart(2, '0')
    const minutes = String(date.getMinutes()).padStart(2, '0')
    const seconds = String(date.getSeconds()).padStart(2, '0')
    
    return `${day}/${month}/${year} - ${hours}:${minutes}:${seconds}`
  } catch (error) {
    console.error('Error formatting date:', error)
    return 'Lỗi format ngày'
  }
}

const getCategoryName = (dataType) => {
  if (dataType.startsWith('D')) return 'Dữ liệu tiền gửi'
  if (dataType.startsWith('L')) return 'Dữ liệu cho vay'
  if (dataType.startsWith('R')) return 'Dữ liệu rủi ro'
  if (dataType.startsWith('G')) return 'Dữ liệu tài khoản'
  return 'Dữ liệu khác'
}

// Lifecycle
onMounted(async () => {
  await refreshAllData()
})
</script>

<style scoped>
/* 🏦 AGRIBANK BRAND STYLING */
.header-section {
  background: linear-gradient(135deg, #8B1538 0%, #C41E3A 50%, #8B1538 100%);
  color: white;
  padding: 40px 30px;
  text-align: center;
  margin-bottom: 30px;
  border-radius: 15px;
  box-shadow: 0 10px 30px rgba(139, 21, 56, 0.3);
}

.header-section h1 {
  margin: 0 0 10px 0;
  font-size: 2.5rem;
  font-weight: bold;
  text-shadow: 2px 2px 4px rgba(0,0,0,0.3);
}

.subtitle {
  font-size: 1.1rem;
  opacity: 0.9;
  margin: 0;
}

/* Alert styles */
.alert {
  padding: 15px 20px;
  border-radius: 8px;
  margin-bottom: 20px;
  display: flex;
  align-items: center;
  gap: 10px;
  position: relative;
}

.alert-error {
  background: #fee;
  border: 1px solid #fcc;
  color: #c33;
}

.alert-success {
  background: #efe;
  border: 1px solid #cfc;
  color: #363;
}

.alert-close {
  position: absolute;
  right: 10px;
  background: none;
  border: none;
  font-size: 18px;
  cursor: pointer;
  color: inherit;
}

/* Loading styles */
.loading-section {
  text-align: center;
  padding: 30px;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #8B1538;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 15px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Control panel */
.control-panel {
  background: #f8f9fa;
  padding: 25px;
  border-radius: 12px;
  margin-bottom: 30px;
  border: 1px solid #e9ecef;
}

.date-control-section {
  margin-bottom: 25px;
}

.agribank-date-title {
  color: #8B1538;
  margin-bottom: 15px;
  font-size: 1.3rem;
}

.date-controls-enhanced {
  display: flex;
  gap: 20px;
  align-items: flex-end;
  flex-wrap: wrap;
}

.date-range-group {
  display: flex;
  gap: 15px;
}

.date-input-group {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.date-input-group label {
  font-weight: 600;
  color: #333;
}

.agribank-date-input {
  padding: 8px 12px;
  border: 2px solid #ddd;
  border-radius: 6px;
  font-size: 14px;
}

.agribank-date-input:focus {
  border-color: #8B1538;
  outline: none;
}

.date-actions-group {
  display: flex;
  gap: 10px;
}

.agribank-btn-filter, .agribank-btn-clear {
  padding: 8px 16px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  font-size: 14px;
}

.agribank-btn-filter {
  background: #8B1538;
  color: white;
}

.agribank-btn-filter:hover:not(:disabled) {
  background: #a91d42;
}

.agribank-btn-filter:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.agribank-btn-clear {
  background: #6c757d;
  color: white;
}

.agribank-btn-clear:hover {
  background: #545b62;
}

/* Bulk actions */
.bulk-actions-section h3 {
  color: #8B1538;
  margin-bottom: 15px;
}

.bulk-actions {
  display: flex;
  gap: 15px;
  flex-wrap: wrap;
}

.btn-clear-all, .btn-refresh, .btn-debug {
  padding: 10px 20px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  font-size: 14px;
}

.btn-clear-all {
  background: #dc3545;
  color: white;
}

.btn-refresh {
  background: #28a745;
  color: white;
}

.btn-debug {
  background: #17a2b8;
  color: white;
}

.btn-clear-all:hover:not(:disabled), .btn-refresh:hover:not(:disabled), .btn-debug:hover:not(:disabled) {
  opacity: 0.9;
}

.btn-clear-all:disabled, .btn-refresh:disabled, .btn-debug:disabled {
  background: #ccc;
  cursor: not-allowed;
}

/* Data types section */
.agribank-section {
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
  overflow: hidden;
}

.agribank-header {
  background: linear-gradient(135deg, #8B1538 0%, #C41E3A 100%);
  color: white;
  padding: 25px;
}

.header-content {
  display: flex;
  align-items: center;
  gap: 20px;
}

.header-text h2 {
  margin: 0 0 8px 0;
  font-size: 1.8rem;
}

.header-text p {
  margin: 0;
  opacity: 0.9;
}

.agribank-brand-line {
  height: 4px;
  background: linear-gradient(90deg, #fff 0%, rgba(255,255,255,0.5) 50%, #fff 100%);
  margin-top: 15px;
}

/* Table styles */
.agribank-table {
  padding: 0;
}

.enhanced-table {
  width: 100%;
  border-collapse: collapse;
}

.agribank-thead {
  background: #f8f9fa;
}

.agribank-thead th {
  padding: 15px;
  text-align: left;
  font-weight: 600;
  color: #333;
  border-bottom: 2px solid #dee2e6;
}

.agribank-tbody tr {
  border-bottom: 1px solid #dee2e6;
}

.agribank-tbody tr:hover {
  background: #f8f9fa;
}

.agribank-tbody td {
  padding: 15px;
  vertical-align: middle;
}

/* Data type info */
.enhanced-datatype {
  display: flex;
  align-items: center;
  gap: 12px;
}

.agribank-icon {
  font-size: 1.5rem;
}

.datatype-details {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.datatype-name {
  font-size: 1.1rem;
  color: #8B1538;
}

.datatype-category {
  font-size: 0.85rem;
  color: #666;
}

/* Records info */
.records-info {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}

.agribank-number {
  font-size: 1.2rem;
  font-weight: bold;
  color: #8B1538;
}

.records-label {
  font-size: 0.85rem;
  color: #666;
}

/* Actions */
.actions-cell {
  text-align: center;
}

.btn-action {
  padding: 8px 12px;
  margin: 0 2px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 16px;
  transition: all 0.2s;
}

.btn-view {
  background: #007bff;
  color: white;
}

.btn-raw-view {
  background: #28a745;
  color: white;
}

.btn-import {
  color: white;
}

.btn-delete {
  background: #dc3545;
  color: white;
}

.btn-action:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0,0,0,0.2);
}

.btn-action:disabled {
  background: #ccc;
  cursor: not-allowed;
  transform: none;
}

/* Modal styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0,0,0,0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 12px;
  max-width: 500px;
  width: 90%;
  max-height: 80vh;
  overflow-y: auto;
}

.modal-header {
  background: #8B1538;
  color: white;
  padding: 20px;
  border-radius: 12px 12px 0 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h3 {
  margin: 0;
}

.modal-close {
  background: none;
  border: none;
  color: white;
  font-size: 24px;
  cursor: pointer;
}

.modal-body {
  padding: 20px;
}

/* Responsive */
@media (max-width: 768px) {
  .date-controls-enhanced {
    flex-direction: column;
    align-items: stretch;
  }
  
  .date-range-group {
    flex-direction: column;
  }
  
  .enhanced-table {
    font-size: 14px;
  }
  
  .agribank-thead th,
  .agribank-tbody td {
    padding: 10px 8px;
  }
}
</style>
