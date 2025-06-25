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
            
            <!-- Upload progress indicator -->
            <div v-if="uploading" class="upload-progress-container">
              <div class="progress-bar-wrapper">
                <div class="progress-bar" :style="{ width: `${uploadProgress}%` }"></div>
              </div>
              <div class="progress-details">
                <span class="progress-percentage">{{ uploadProgress }}%</span>
                <span class="progress-status">{{ getUploadStatusText() }}</span>
              </div>
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

    <!-- Modal hiển thị dữ liệu đã import -->
    <div v-if="showDataViewModal" class="modal-overlay" @click="closeDataViewModal">
      <div class="modal-content data-view-modal" @click.stop>
        <div class="modal-header">
          <h3>Dữ liệu {{ selectedDataType }} {{ statementDateFormatted }}</h3>
          <button @click="closeDataViewModal" class="modal-close">×</button>
        </div>
        <div class="modal-body">
          <div v-if="filteredResults.length > 0" class="data-table-container">
            <table class="data-table enhanced-table">
              <thead class="agribank-thead">
                <tr>
                  <th>Tên file</th>
                  <th>Ngày import</th>
                  <th>Số bản ghi</th>
                  <th>Trạng thái</th>
                  <th>Thao tác</th>
                </tr>
              </thead>
              <tbody class="agribank-tbody">
                <tr v-for="(item, index) in filteredResults" :key="index">
                  <td>{{ item.fileName }}</td>
                  <td>{{ formatDateTime(item.importDate) }}</td>
                  <td class="agribank-number">{{ formatRecordCount(item.recordsCount) }}</td>
                  <td>{{ item.status }}</td>
                  <td>
                    <button 
                      @click="previewData(item.id)" 
                      class="btn-action btn-view"
                      title="Xem chi tiết"
                    >
                      👁️
                    </button>
                    <button 
                      @click="confirmDelete(item.id, item.fileName)" 
                      class="btn-action btn-delete"
                      title="Xóa bản ghi"
                    >
                      🗑️
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div v-else class="no-data-message">
            <p>Không có dữ liệu import nào {{ selectedFromDate ? 'cho ngày đã chọn' : '' }}</p>
          </div>
        </div>
        <div class="modal-footer">
          <button @click="closeDataViewModal" class="btn-cancel">Đóng</button>
        </div>
      </div>
    </div>

    <!-- Modal hiển thị dữ liệu thô -->
    <div v-if="showRawDataModal" class="modal-overlay" @click="closeRawDataModal">
      <div class="modal-content raw-data-modal" @click.stop>
        <div class="modal-header">
          <h3>Dữ liệu thô {{ selectedDataType }} {{ statementDateFormatted }}</h3>
          <button @click="closeRawDataModal" class="modal-close">×</button>
        </div>
        <div class="modal-body">
          <div v-if="rawDataRecords.length > 0" class="raw-data-table-container">
            <div class="table-summary">
              <p>Hiển thị {{ rawDataRecords.length }} bản ghi dữ liệu thô</p>
            </div>
            <div class="responsive-table-wrapper">
              <table class="raw-data-table enhanced-table">
                <thead class="agribank-thead">
                  <tr>
                    <th v-for="(column, index) in Object.keys(rawDataRecords[0]).slice(0, 10)" :key="index">
                      {{ column }}
                    </th>
                  </tr>
                </thead>
                <tbody class="agribank-tbody">
                  <tr v-for="(record, recordIndex) in rawDataRecords" :key="recordIndex">
                    <td v-for="(column, columnIndex) in Object.keys(record).slice(0, 10)" :key="columnIndex">
                      {{ record[column] }}
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
            <div class="table-note">
              <p><i>Lưu ý: Bảng hiển thị tối đa 10 cột đầu tiên để dễ đọc</i></p>
            </div>
          </div>
          <div v-else class="no-data-message">
            <p>Không có dữ liệu thô nào {{ selectedFromDate ? 'cho ngày đã chọn' : '' }}</p>
          </div>
        </div>
        <div class="modal-footer">
          <button @click="closeRawDataModal" class="btn-cancel">Đóng</button>
          <button 
            v-if="rawDataRecords.length > 0" 
            @click="exportRawData" 
            class="btn-export"
          >
            📥 Xuất dữ liệu
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
const filteredResults = ref([])
const rawDataRecords = ref([])

// Modal state
const showImportModal = ref(false)
const showDataViewModal = ref(false)
const showRawDataModal = ref(false)
const selectedDataType = ref(null)
const selectedFiles = ref([])
const archivePassword = ref('')
const importNotes = ref('')
const uploading = ref(false)
const uploadProgress = ref(0)
const statementDateFormatted = computed(() => {
  if (!selectedFromDate.value) return ''
  return `(${formatDate(selectedFromDate.value)})`
})

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

const hasArchiveFile = computed(() => {
  return selectedFiles.value.some(file => isArchiveFile(file.name))
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

// Upload status text
const getUploadStatusText = () => {
  if (uploadProgress === 0) return 'Đang chuẩn bị...'
  if (uploadProgress < 20) return 'Đang tải dữ liệu lên...'
  if (uploadProgress < 50) return 'Đang xử lý dữ liệu...'
  if (uploadProgress < 90) return 'Đang lưu dữ liệu...'
  if (uploadProgress < 100) return 'Sắp hoàn thành...'
  return 'Đã hoàn thành!'
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
  try {
    loading.value = true
    loadingMessage.value = `Đang tải dữ liệu ${dataType}...`
    selectedDataType.value = dataType
    
    // If a date is selected, fetch data by date
    if (selectedFromDate.value) {
      const dateStr = selectedFromDate.value.replace(/-/g, '')
      const result = await rawDataService.getByStatementDate(dataType, dateStr)
      
      if (result.success) {
        filteredResults.value = result.data || []
        
        if (filteredResults.value.length === 0) {
          showError(`Không có dữ liệu ${dataType} cho ngày ${formatDate(selectedFromDate.value)}`)
        } else {
          showSuccess(`Hiển thị ${filteredResults.value.length} import(s) cho loại ${dataType} ngày ${formatDate(selectedFromDate.value)}`)
          showDataViewModal.value = true
        }
      } else {
        showError(`Lỗi khi tải dữ liệu: ${result.error}`)
        filteredResults.value = []
      }
    } else {
      // Filter current results by data type
      const dataTypeResults = allImports.value.filter(imp => 
        imp.dataType === dataType || 
        imp.category === dataType || 
        imp.fileType === dataType
      )
      filteredResults.value = dataTypeResults
      
      if (dataTypeResults.length === 0) {
        showError(`Chưa có dữ liệu import nào cho loại ${dataType}`)
        return
      }
      
      showSuccess(`Hiển thị ${dataTypeResults.length} import(s) cho loại ${dataType}`)
      showDataViewModal.value = true
    }
  } catch (error) {
    console.error('Error viewing data type:', error)
    showError(`Lỗi khi tải dữ liệu: ${error.message}`)
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

const deleteDataTypeByDate = async (dataType) => {
  if (!selectedFromDate.value) {
    showError('Vui lòng chọn ngày để xóa dữ liệu')
    return
  }
  
  const dateStr = selectedFromDate.value.replace(/-/g, '')
  
  // Check if data exists for this date
  try {
    const checkResult = await rawDataService.checkDuplicateData(dataType, dateStr)
    if (checkResult.success && !checkResult.data.hasDuplicate) {
      showError(`Không có dữ liệu ${dataType} cho ngày ${formatDate(selectedFromDate.value)}`)
      return
    }
    
    // Hiển thị xác nhận
    if (confirm(`Bạn có chắc chắn muốn xóa tất cả dữ liệu ${dataType} cho ngày ${formatDate(selectedFromDate.value)}?`)) {
      performDeleteByDate(dataType, dateStr)
    }
  } catch (error) {
    console.error('Error checking duplicate data:', error)
    showError('Có lỗi xảy ra khi kiểm tra dữ liệu')
  }
}

const performDeleteByDate = async (dataType, dateStr) => {
  try {
    loading.value = true
    loadingMessage.value = 'Đang xóa dữ liệu...'
    
    const result = await rawDataService.deleteByStatementDate(dataType, dateStr)
    if (result.success) {
      showSuccess(`✅ ${result.data.message}`)
      await refreshAllData()
      
      // Remove from filtered results if they exist
      filteredResults.value = filteredResults.value.filter(item => 
        !(item.dataType === dataType && 
          item.statementDate && 
          new Date(item.statementDate).toISOString().slice(0, 10).replace(/-/g, '') === dateStr)
      )
    } else {
      showError(`Lỗi khi xóa dữ liệu: ${result.error}`)
    }
  } catch (error) {
    console.error('Error deleting data:', error)
    showError(`Lỗi khi xóa dữ liệu: ${error.message}`)
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

// Data view modal methods

const closeDataViewModal = () => {
  showDataViewModal.value = false
}

// Raw data modal methods
const viewRawDataFromTable = async (dataType) => {
  try {
    loading.value = true
    loadingMessage.value = `Đang tải dữ liệu thô ${dataType}...`
    selectedDataType.value = dataType
    
    console.log('🗄️ Viewing raw data from table:', dataType)
    console.log('Selected date:', selectedFromDate.value)
    
    // Check if date is selected
    if (!selectedFromDate.value) {
      showError('Vui lòng chọn ngày để xem dữ liệu thô')
      loading.value = false
      loadingMessage.value = ''
      return
    }
    
    const result = await rawDataService.getRawDataFromTable(dataType, selectedFromDate.value)
    console.log('🗄️ Raw data result:', result)
    
    if (result.success && result.data) {
      // Helper function để convert $values format nếu cần
      const convertDotNetArray = (data) => {
        if (data && typeof data === 'object' && data.$values && Array.isArray(data.$values)) {
          console.log('🔧 Converting raw data $values format, length:', data.$values.length)
          return data.$values;
        }
        return data;
      };
      
      // Xử lý dữ liệu records từ backend
      const records = result.data.records || [];
      
      if (records && records.length > 0) {
        rawDataRecords.value = records;
        showSuccess(`Đã tải ${records.length} bản ghi dữ liệu thô ${dataType}`);
        showRawDataModal.value = true;
      } else {
        showError(`Không tìm thấy dữ liệu thô cho ${dataType} vào ngày ${formatDate(selectedFromDate.value)}`);
      }
    } else {
      showError(`Lỗi khi tải dữ liệu thô: ${result.error || 'Không tìm thấy dữ liệu'}`);
    }
  } catch (error) {
    console.error('Error viewing raw data:', error);
    showError(`Lỗi khi tải dữ liệu thô: ${error.message}`);
  } finally {
    loading.value = false;
    loadingMessage.value = '';
  }
}

const closeRawDataModal = () => {
  showRawDataModal.value = false
  rawDataRecords.value = []
}

const exportRawData = () => {
  try {
    // Create CSV content
    let csvContent = "";
    
    // Get all unique headers
    const headers = new Set();
    rawDataRecords.value.forEach(record => {
      Object.keys(record).forEach(key => headers.add(key));
    });
    
    // Add headers
    csvContent += Array.from(headers).join(',') + '\n';
    
    // Add data rows
    rawDataRecords.value.forEach(record => {
      const row = Array.from(headers).map(header => {
        const value = record[header] || '';
        // Handle values with commas by wrapping in quotes
        return typeof value === 'string' && value.includes(',') 
          ? `"${value}"` 
          : value;
      });
      csvContent += row.join(',') + '\n';
    });
    
    // Create download link
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.setAttribute('href', url);
    link.setAttribute('download', `rawdata-${selectedDataType.value}-${selectedFromDate.value}.csv`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    
    showSuccess('Đã xuất dữ liệu thành công');
  } catch (error) {
    console.error('Error exporting data:', error);
    showError(`Lỗi khi xuất dữ liệu: ${error.message}`);
  }
}

// Preview data method
const previewData = async (importId) => {
  try {
    loading.value = true
    loadingMessage.value = 'Đang tải dữ liệu chi tiết...'
    
    const result = await rawDataService.previewData(importId)
    
    if (result.success && result.data) {
      // TODO: Implement preview modal
      const recordsCount = result.data.previewRows?.length || 0
      showSuccess(`Đã tải ${recordsCount} bản ghi chi tiết. Tính năng đang phát triển...`)
    } else {
      showError(`Lỗi khi tải dữ liệu chi tiết: ${result.error || 'Không tìm thấy dữ liệu'}`)
    }
  } catch (error) {
    console.error('Error previewing data:', error)
    showError(`Lỗi khi tải dữ liệu chi tiết: ${error.message}`)
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

// Delete confirmation
const confirmDelete = async (importId, fileName) => {
  if (confirm(`Bạn có chắc chắn muốn xóa bản ghi "${fileName}"?`)) {
    try {
      loading.value = true
      loadingMessage.value = 'Đang xóa dữ liệu...'
      
      const result = await rawDataService.deleteImport(importId)
      
      if (result.success) {
        showSuccess(`Đã xóa thành công bản ghi "${fileName}"`)
        
        // Remove from filtered results
        filteredResults.value = filteredResults.value.filter(item => item.id !== importId)
        
        // Refresh all data
        await refreshAllData()
      } else {
        showError(`Lỗi khi xóa bản ghi: ${result.error}`)
      }
    } catch (error) {
      console.error('Error deleting import:', error)
      showError(`Lỗi khi xóa bản ghi: ${error.message}`)
    } finally {
      loading.value = false
      loadingMessage.value = ''
    }
  }
}

// Các phương thức tiện ích cho view đã được nhắc đến trong template
const getCategoryName = (dataType) => {
  // Lấy tên category từ định nghĩa data type
  return dataTypeDefinitions[dataType]?.category || 'Chưa phân loại'
}

const formatDateTime = (dateTimeString) => {
  if (!dateTimeString) return 'N/A'
  
  try {
    const date = new Date(dateTimeString)
    if (isNaN(date.getTime())) {
      return 'Thời gian không hợp lệ'
    }
    
    const day = String(date.getDate()).padStart(2, '0')
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const year = date.getFullYear()
    const hours = String(date.getHours()).padStart(2, '0')
    const minutes = String(date.getMinutes()).padStart(2, '0')
    
    return `${day}/${month}/${year} ${hours}:${minutes}`
  } catch (error) {
    console.error('Error formatting datetime:', error)
    return 'Lỗi format thời gian'
  }
}

const getDataTypeColor = (dataType) => {
  // Màu sắc tương ứng với loại dữ liệu
  const colors = {
    'HDMB': '#2196F3',       // Xanh dương
    'HDBH': '#4CAF50',       // Xanh lá
    'HDTH': '#FF9800',       // Cam
    'HDFX': '#9C27B0',       // Tím
    'BAOHIEM': '#E91E63',    // Hồng
    'DANCU': '#607D8B',      // Xám xanh
    'PHICHUYENTIEN': '#795548', // Nâu
    'LAMVIEC': '#00BCD4'     // Xanh ngọc
  }
  
  return colors[dataType] || '#8B1538' // Màu mặc định là màu Agribank
}

const openImportModal = (dataType) => {
  selectedDataType.value = dataType
  selectedFiles.value = []
  archivePassword.value = ''
  importNotes.value = ''
  uploading.value = false
  uploadProgress.value = 0
  showImportModal.value = true
}

// Hàm kiểm tra nếu file là file nén
const isArchiveFile = (fileName) => {
  const archiveExtensions = ['.zip', '.rar', '.7z', '.tar', '.gz']
  return archiveExtensions.some(ext => fileName.toLowerCase().endsWith(ext))
}

// Hàm định dạng kích thước file
const formatFileSize = (bytes) => {
  if (bytes === 0) return '0 Bytes'
  
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

// Xử lý chọn file
const handleFileSelect = (event) => {
  const files = event.target.files
  if (files.length === 0) return
  
  selectedFiles.value = Array.from(files)
}

// Hàm định dạng số lượng bản ghi
const formatRecordCount = (count) => {
  // Nếu count không phải là số, trả về giá trị ban đầu
  if (isNaN(parseInt(count))) return count || 0
  
  // Định dạng số với dấu phân cách hàng nghìn
  return new Intl.NumberFormat('vi-VN').format(count)
}
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

/* Upload progress styles */
.upload-progress-container {
  margin: 15px 0;
  padding: 15px;
  background: #f5f5f5;
  border-radius: 8px;
  border: 1px solid #ddd;
}

.progress-bar-wrapper {
  height: 12px;
  background-color: #e0e0e0;
  border-radius: 6px;
  overflow: hidden;
  margin-bottom: 8px;
}

.progress-bar {
  height: 100%;
  background: linear-gradient(90deg, #8B1538 0%, #C41E3A 100%);
  border-radius: 6px;
  transition: width 0.3s ease;
}

.progress-details {
  display: flex;
  justify-content: space-between;
  font-size: 12px;
  color: #666;
}

.progress-percentage {
  font-weight: bold;
  color: #8B1538;
}

/* Agribank import styling */
.btn-submit {
  background: linear-gradient(135deg, #8B1538 0%, #C41E3A 100%);
  color: white;
  padding: 10px 20px;
  border: none;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-submit:hover:not(:disabled) {
  background: linear-gradient(135deg, #7a1230 0%, #b31a33 100%);
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0,0,0,0.2);
}

.btn-submit:disabled {
  background: #ccc;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

/* Data modal styles */
.data-view-modal,
.raw-data-modal {
  max-width: 90%;
  width: 1000px;
  max-height: 80vh;
}

.data-table-container,
.raw-data-table-container {
  overflow-x: auto;
  margin: 0 -20px;
}

.data-table,
.raw-data-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 14px;
}

.responsive-table-wrapper {
  overflow-x: auto;
  max-height: 50vh;
}

.table-summary {
  margin-bottom: 10px;
  font-weight: bold;
  color: #8B1538;
}

.table-note {
  margin-top: 10px;
  font-size: 12px;
  color: #666;
  text-align: center;
}

.no-data-message {
  text-align: center;
  padding: 30px;
  color: #666;
  font-style: italic;
}

.btn-export {
  background: #28a745;
  color: white;
  padding: 8px 16px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
}

.btn-export:hover {
  background: #218838;
}
</style>
