<template>
  <div class="raw-data-warehouse">
    <!-- Header Section -->
    <div class="header-section">
      <h1>🗄️ KHO DỮ LIỆU THÔ</h1>
      <p class="subtitle">Import và quản lý dữ liệu thô theo từng loại dữ liệu chuyên biệt</p>
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
        <h3>🗓️ Chọn ngày sao kê</h3>
        <div class="date-controls">
          <div class="date-range">
            <label>Từ ngày:</label>
            <input 
              v-model="selectedFromDate" 
              type="date" 
              class="date-input"
            />
            <label>Đến ngày:</label>
            <input 
              v-model="selectedToDate" 
              type="date" 
              class="date-input"
            />
          </div>
          <button @click="applyDateFilter" class="btn-filter" :disabled="!selectedFromDate">
            🔍 Lọc theo ngày
          </button>
          <button @click="clearDateFilter" class="btn-clear">
            🗑️ Xóa bộ lọc
          </button>
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
        </div>
      </div>
    </div>

    <!-- Data Types List -->
    <div class="data-types-section">
      <div class="section-header">
        <h2>📊 Danh sách loại dữ liệu</h2>
        <p>Mỗi loại dữ liệu được hiển thị với các thao tác: Xem, Import, Xóa theo ngày</p>
      </div>

      <div class="data-types-table">
        <table>
          <thead>
            <tr>
              <th>Loại dữ liệu</th>
              <th>Mô tả</th>
              <th>Định dạng file</th>
              <th>Tổng records</th>
              <th>Cập nhật cuối</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(dataType, key) in sortedDataTypeDefinitions" :key="key">
              <td>
                <div class="data-type-info">
                  <span class="data-type-icon">{{ dataType.icon }}</span>
                  <strong>{{ key }}</strong>
                </div>
              </td>
              <td class="description-cell">{{ dataType.description }}</td>
              <td>
                <span class="file-formats">{{ dataType.acceptedFormats.join(', ') }}</span>
              </td>
              <td class="records-cell">
                <span class="records-count">{{ getDataTypeStats(key).totalRecords }}</span>
              </td>
              <td class="last-update-cell">{{ getDataTypeStats(key).lastUpdate || 'Chưa có dữ liệu' }}</td>
              <td class="actions-cell">
                <button 
                  @click="viewDataType(key)" 
                  class="btn-action btn-view"
                  title="Xem dữ liệu import"
                  :disabled="false"
                >
                  👁️ Xem Import ({{ getDataTypeStats(key).totalRecords }})
                </button>
                <button 
                  @click="viewRawDataFromTable(key)" 
                  class="btn-action btn-raw-view"
                  title="Xem dữ liệu thô từ bảng"
                  :disabled="!selectedFromDate"
                >
                  🗄️ Xem Thô
                </button>
                <button 
                  @click="openImportModal(key)" 
                  class="btn-action btn-import"
                  :style="{ backgroundColor: getDataTypeColor(key) }"
                  title="Import dữ liệu"
                >
                  📤 Import
                </button>
                <button 
                  @click="deleteDataTypeByDate(key)" 
                  class="btn-action btn-delete"
                  title="Xóa theo ngày đã chọn"
                  :disabled="!selectedFromDate || getDataTypeStats(key).totalRecords === 0"
                >
                  🗑️ Xóa
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Filtered Results Section (when date filter is applied) -->
    <div v-if="filteredResults.length > 0" class="filtered-results-section">
      <div class="section-header">
        <h2>🔍 Kết quả lọc theo ngày</h2>
        <p>Hiển thị {{ filteredResults.length }} bản ghi từ {{ formatDate(selectedFromDate) }} 
           {{ selectedToDate ? ' đến ' + formatDate(selectedToDate) : '' }}</p>
      </div>

      <div class="results-table">
        <table>
          <thead>
            <tr>
              <th>Loại dữ liệu</th>
              <th>Tên file</th>
              <th>Ngày sao kê</th>
              <th>Ngày import</th>
              <th>Records</th>
              <th>Trạng thái</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in paginatedFilteredResults" :key="item.id">
              <td>
                <span 
                  class="data-type-badge" 
                  :style="{ backgroundColor: getDataTypeColor(item.dataType) }"
                >
                  {{ item.dataType }}
                </span>
              </td>
              <td class="filename-cell">
                <span class="filename">{{ item.fileName }}</span>
              </td>
              <td>{{ formatDate(item.statementDate) }}</td>
              <td>{{ formatDate(item.importDate) }}</td>
              <td class="records-cell">
                <span class="records-count">{{ formatNumber(item.recordsCount) }}</span>
              </td>
              <td>
                <span 
                  class="status-badge" 
                  :class="getStatusClass(item.status)"
                >
                  {{ getStatusText(item.status) }}
                </span>
              </td>
              <td class="actions-cell">
                <button 
                  @click="previewImport(item)" 
                  class="btn-action btn-preview"
                  title="Xem trước dữ liệu"
                >
                  👁️
                </button>
                <button 
                  @click="deleteImport(item)" 
                  class="btn-action btn-delete"
                  title="Xóa import này"
                >
                  🗑️
                </button>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- Pagination for filtered results -->
        <div v-if="filteredResultsTotalPages > 1" class="pagination">
          <button 
            @click="filteredResultsCurrentPage = 1" 
            :disabled="filteredResultsCurrentPage === 1"
            class="pagination-btn"
          >
            ⏮️
          </button>
          <button 
            @click="filteredResultsCurrentPage--" 
            :disabled="filteredResultsCurrentPage === 1"
            class="pagination-btn"
          >
            ⏪
          </button>
          <span class="pagination-info">
            Trang {{ filteredResultsCurrentPage }} / {{ filteredResultsTotalPages }}
          </span>
          <button 
            @click="filteredResultsCurrentPage++" 
            :disabled="filteredResultsCurrentPage === filteredResultsTotalPages"
            class="pagination-btn"
          >
            ⏩
          </button>
          <button 
            @click="filteredResultsCurrentPage = filteredResultsTotalPages" 
            :disabled="filteredResultsCurrentPage === filteredResultsTotalPages"
            class="pagination-btn"
          >
            ⏭️
          </button>
        </div>
      </div>
    </div>

    <!-- Import Modal -->
    <div v-if="showImportModal" class="modal-overlay" @click="closeImportModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>
            {{ dataTypeDefinitions[selectedDataType]?.icon }} 
            Import {{ dataTypeDefinitions[selectedDataType]?.name }}
          </h3>
          <button @click="closeImportModal" class="modal-close">×</button>
        </div>

        <div class="modal-body">
          <div class="import-form">
            <!-- File Upload -->
            <div class="form-group">
              <label>📁 Chọn file để import</label>
              <div 
                class="upload-area"
                @drop.prevent="handleFileDrop"
                @dragover.prevent
                @click="$refs.fileInput.click()"
              >
                <input 
                  ref="fileInput"
                  type="file" 
                  multiple 
                  :accept="getAcceptTypes()"
                  @change="handleFileSelect"
                  style="display: none;"
                />
                <div class="upload-content">
                  <span class="upload-icon">📤</span>
                  <p><strong>Kéo thả file vào đây hoặc click để chọn</strong></p>
                  <p class="upload-hint">Hỗ trợ: {{ dataTypeDefinitions[selectedDataType]?.acceptedFormats.join(', ') }}</p>
                </div>
              </div>
            </div>

            <!-- Selected Files -->
            <div v-if="selectedFiles.length > 0" class="selected-files">
              <h4>
                📋 File đã chọn
                <span class="file-count-badge">{{ selectedFiles.length }}</span>
              </h4>
              <div v-for="(file, index) in selectedFiles" :key="index" class="file-item">
                <span class="file-icon">{{ getFileIcon(file.name) }}</span>
                <span class="file-name">{{ file.name }}</span>
                <span class="file-size">{{ formatFileSize(file.size) }}</span>
                <button @click="removeFile(index)" class="btn-remove-file" :title="`Xóa file ${file.name}`">×</button>
              </div>
            </div>

            <!-- Archive Password -->
            <div v-if="hasArchiveFile" class="form-group">
              <label>🔐 Mật khẩu file nén (nếu có)</label>
              
              <!-- Checkbox tự động điền mật khẩu mặc định -->
              <div class="auto-password-section">
                <label class="checkbox-wrapper">
                  <input 
                    type="checkbox" 
                    v-model="useDefaultPassword"
                    @change="onDefaultPasswordToggle"
                  />
                  <span class="checkmark"></span>
                  <span class="checkbox-label">🔑 Tự động điền mật khẩu mặc định (Snk6S4GV)</span>
                </label>
              </div>
              
              <input 
                v-model="archivePassword" 
                type="password" 
                placeholder="Nhập mật khẩu file nén..."
                class="form-input"
                :class="{ 'auto-filled': useDefaultPassword }"
              />
              <small class="form-hint">
                <span v-if="useDefaultPassword">✅ Đang sử dụng mật khẩu mặc định. Bạn có thể sửa nếu cần.</span>
                <span v-else>Để trống nếu file không có mật khẩu</span>
              </small>
            </div>

            <!-- Notes -->
            <div class="form-group">
              <label>📝 Ghi chú</label>
              <textarea 
                v-model="importNotes" 
                placeholder="Ghi chú về dữ liệu import (không bắt buộc)..."
                class="form-textarea"
                rows="3"
              ></textarea>
            </div>
          </div>
          
          <!-- Progress Section -->
          <div v-if="uploading" class="upload-progress-section">
            <div class="progress-header">
              <h4>📤 Tiến độ upload</h4>
              <div class="progress-stats">
                <span class="progress-percentage">{{ uploadProgress }}%</span>
                <span v-if="remainingTime > 0" class="remaining-time">
                  ⏱️ Còn lại: {{ remainingTimeFormatted }}
                </span>
              </div>
            </div>
            
            <div class="progress-bar-container">
              <div class="progress-bar" :style="{ width: uploadProgress + '%' }"></div>
            </div>
            
            <div class="progress-message">
              {{ loadingMessage }}
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button @click="closeImportModal" class="btn-cancel">Hủy</button>
          <button 
            @click="performImport" 
            class="btn-import-confirm"
            :disabled="selectedFiles.length === 0 || uploading"
            :style="{ backgroundColor: getDataTypeColor(selectedDataType) }"
          >
            {{ uploading ? '⏳ Đang import...' : '📤 Import Dữ liệu' }}
          </button>
        </div>
      </div>
    </div>

    <!-- Preview Modal -->
    <div v-if="showPreviewModal" class="modal-overlay" @click="closePreviewModal">
      <div class="modal-content modal-large" @click.stop>
        <div class="modal-header">
          <h3>👁️ Xem trước: {{ selectedImport?.fileName }}</h3>
          <button @click="closePreviewModal" class="modal-close">×</button>
        </div>

        <div class="modal-body">
          <div v-if="previewData && previewData.length > 0" class="preview-content">
            {{ console.log('🖼️ Modal rendering with data:', previewData.length, 'records') }}
            <!-- Preview Info -->
            <div class="preview-info">
              <div class="info-grid">
                <div class="info-item">
                  <label>Loại dữ liệu:</label>
                  <span class="data-type-badge" :style="{ backgroundColor: getDataTypeColor(selectedImport?.dataType) }">
                    {{ selectedImport?.dataType }}
                  </span>
                </div>
                <div class="info-item">
                  <label>Ngày sao kê:</label>
                  <span>{{ formatDate(selectedImport?.statementDate) }}</span>
                </div>
                <div class="info-item">
                  <label>Ngày import:</label>
                  <span>{{ formatDate(selectedImport?.importDate) }}</span>
                </div>
                <div class="info-item">
                  <label>Số records:</label>
                  <span>{{ formatNumber(previewData.length) }}</span>
                </div>
              </div>
            </div>

            <!-- Data Table -->
            <div class="preview-table">
              <h4>📊 Dữ liệu mẫu (hiển thị tối đa 100 records)</h4>
              <div class="table-wrapper">
                <table v-if="previewData.length > 0">
                  <thead>
                    <tr>
                      <th v-for="(column, index) in Object.keys(previewData[0] || {})" :key="index">{{ column }}</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(record, index) in previewData.slice(0, 100)" :key="index">
                      <td v-for="(column, colIndex) in Object.keys(previewData[0] || {})" :key="colIndex">
                        {{ record[column] || '-' }}
                      </td>
                    </tr>
                  </tbody>
                </table>
                <div v-else class="no-data">
                  <p>Không có dữ liệu để hiển thị</p>
                </div>
              </div>
              <div v-if="previewData.length > 100" class="preview-note">
                💡 Chỉ hiển thị 100 records đầu tiên. Tổng cộng: {{ previewData.length }} records
              </div>
            </div>
          </div>
          <div v-else class="no-preview-data">
            {{ console.log('📭 Modal showing no data. previewData:', previewData) }}
            <div class="empty-icon">📭</div>
            <p>Không có dữ liệu để hiển thị</p>
          </div>
        </div>

        <div class="modal-footer">
          <button @click="closePreviewModal" class="btn-cancel">Đóng</button>
        </div>
      </div>
    </div>

    <!-- Confirmation Modal -->
    <div v-if="showConfirmationModal" class="modal-overlay" @click="cancelConfirmation">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>⚠️ Xác nhận thao tác</h3>
          <button @click="cancelConfirmation" class="modal-close">×</button>
        </div>
        <div class="modal-body">
          <p>{{ confirmationMessage }}</p>
          <div v-if="existingImports.length > 0" class="existing-imports">
            <h4>📋 Dữ liệu hiện có:</h4>
            <ul>
              <li v-for="imp in existingImports" :key="imp.id">
                {{ imp.fileName }} - {{ formatNumber(imp.recordsCount) }} records ({{ formatDate(imp.importDate) }})
              </li>
            </ul>
          </div>
        </div>
        <div class="modal-footer">
          <button @click="cancelConfirmation" class="btn-cancel">Hủy</button>
          <button @click="confirmAction" class="btn-confirm" :style="{ backgroundColor: '#dc3545' }">
            {{ confirmButtonText }}
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
const filteredResults = ref([])
const filteredResultsCurrentPage = ref(1)
const itemsPerPage = 20

// Data management
const allImports = ref([])
const dataTypeStats = ref({})

// Modal state
const showImportModal = ref(false)
const showPreviewModal = ref(false)
const showConfirmationModal = ref(false)
const selectedDataType = ref(null)
const selectedImport = ref(null)
const previewData = ref([])

// File handling
const selectedFiles = ref([])
const archivePassword = ref('')
const importNotes = ref('')
const useDefaultPassword = ref(true) // ✅ Checkbox tự động điền mật khẩu mặc định
const uploading = ref(false)
const uploadProgress = ref(0)
const remainingTime = ref(0) // milliseconds
const remainingTimeFormatted = ref('00:00') // mm:ss format

// Confirmation state
const confirmationMessage = ref('')
const confirmButtonText = ref('Xác nhận')
const confirmCallback = ref(null)
const existingImports = ref([])

// Data type definitions
const dataTypeDefinitions = rawDataService.getDataTypeDefinitions()

// Computed properties
const paginatedFilteredResults = computed(() => {
  const start = (filteredResultsCurrentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return filteredResults.value.slice(start, end)
})

const filteredResultsTotalPages = computed(() => {
  return Math.ceil(filteredResults.value.length / itemsPerPage)
})

const hasArchiveFile = computed(() => {
  return selectedFiles.value.some(file => isArchiveFile(file.name))
})

// Sort data types alphabetically by key
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
  return dataTypeStats.value[dataType] || { totalRecords: 0, lastUpdate: null }
}

const calculateDataTypeStats = () => {
  const stats = {}
  
  // Initialize all data types
  Object.keys(dataTypeDefinitions).forEach(key => {
    stats[key] = { totalRecords: 0, lastUpdate: null }
  })
  
  // Calculate from imports
  allImports.value.forEach(imp => {
    if (stats[imp.dataType]) {
      stats[imp.dataType].totalRecords += imp.recordsCount || 0
      if (!stats[imp.dataType].lastUpdate || 
          new Date(imp.importDate) > new Date(stats[imp.dataType].lastUpdate)) {
        stats[imp.dataType].lastUpdate = imp.importDate
      }
    }
  })
  
  dataTypeStats.value = stats
}

// Date filtering methods
const applyDateFilter = async () => {
  if (!selectedFromDate.value) {
    showError('Vui lòng chọn ngày bắt đầu')
    return
  }
  
  try {
    loading.value = true
    loadingMessage.value = 'Đang lọc dữ liệu theo ngày...'
    
    const fromDateStr = selectedFromDate.value.replace(/-/g, '')
    const toDateStr = selectedToDate.value ? selectedToDate.value.replace(/-/g, '') : fromDateStr
    
    // Get data for each data type within the date range
    const results = []
    for (const dataType of Object.keys(dataTypeDefinitions)) {
      try {
        const response = selectedToDate.value ? 
          await rawDataService.getByDateRange(dataType, fromDateStr, toDateStr) :
          await rawDataService.getByStatementDate(dataType, fromDateStr)
          
        if (response.success && response.data) {
          results.push(...response.data)
        }
      } catch (error) {
        // Continue with other data types if one fails
        console.warn(`Failed to get data for ${dataType}:`, error)
      }
    }
    
    filteredResults.value = results
    filteredResultsCurrentPage.value = 1
    
    if (results.length > 0) {
      showSuccess(`Tìm thấy ${results.length} bản ghi từ ${formatDate(selectedFromDate.value)} ${selectedToDate.value ? 'đến ' + formatDate(selectedToDate.value) : ''}`)
    } else {
      showError('Không tìm thấy dữ liệu trong khoảng thời gian đã chọn')
    }
    
  } catch (error) {
    console.error('Error filtering by date:', error)
    showError('Có lỗi xảy ra khi lọc dữ liệu')
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

const clearDateFilter = () => {
  selectedFromDate.value = ''
  selectedToDate.value = ''
  filteredResults.value = []
  showSuccess('Đã xóa bộ lọc ngày')
}

// Data management methods
const refreshAllData = async () => {
  try {
    loading.value = true
    loadingMessage.value = 'Đang tải lại dữ liệu...'
    
    const result = await rawDataService.getAllImports()
    if (result.success) {
      allImports.value = result.data || []
      calculateDataTypeStats()
      showSuccess('Đã tải lại dữ liệu thành công')
    } else {
      // Hiển thị thông báo lỗi chi tiết
      const errorMsg = result.error || 'Không thể tải dữ liệu'
      console.error('🔥 Chi tiết lỗi:', {
        error: result.error,
        errorCode: result.errorCode,
        errorStatus: result.errorStatus
      })
      
      // Sử dụng mock data để demo vẫn hoạt động
      if (result.fallbackData && result.fallbackData.length > 0) {
        allImports.value = result.fallbackData
        calculateDataTypeStats()
        showError(`⚠️ Chế độ Demo: ${errorMsg}`)
        console.info('🎭 Sử dụng mock data cho demo')
      } else {
        allImports.value = []
        showError(`❌ Lỗi kết nối: ${errorMsg}`)
      }
      
      // Nếu là lỗi kết nối, hiển thị hướng dẫn khắc phục
      if (result.errorCode === 'ERR_NETWORK' || result.errorCode === 'ERR_CONNECTION_REFUSED') {
        setTimeout(() => {
          alert(`🔧 HƯỚNG DẪN KHẮC PHỤC:\n\n1. Kiểm tra backend server có đang chạy không\n2. Đảm bảo server chạy trên port đúng (hiện tại: ${import.meta.env.VITE_API_BASE_URL})\n3. Kiểm tra firewall không chặn kết nối\n4. Thử restart server nếu cần thiết\n\n📝 Hiện tại đang sử dụng dữ liệu demo`)
        }, 1000)
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
  
  if (!confirm('🚨 XÁC NHẬN LẦN CUỐI: Xóa toàn bộ dữ liệu?\n\nNhấn OK để tiếp tục xóa, Cancel để hủy.')) {
    return
  }
  
  try {
    loading.value = true
    loadingMessage.value = 'Đang xóa toàn bộ dữ liệu...'
    
    const result = await rawDataService.clearAllData()
    if (result.success) {
      allImports.value = []
      filteredResults.value = []
      calculateDataTypeStats()
      showSuccess(`✅ ${result.data.message}`)
    } else {
      showError(result.error || 'Không thể xóa dữ liệu')
    }
    
  } catch (error) {
    console.error('Error clearing all data:', error)
    showError('Có lỗi xảy ra khi xóa dữ liệu')
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

// Data type actions
const viewDataType = async (dataType) => {
  try {
    loading.value = true
    loadingMessage.value = `Đang tải dữ liệu ${dataType}...`
    
    // If a date is selected, fetch data by date
    if (selectedFromDate.value) {
      const dateStr = selectedFromDate.value.replace(/-/g, '')
      const result = await rawDataService.getByStatementDate(dataType, dateStr)
      
      if (result.success) {
        filteredResults.value = result.data || []
        filteredResultsCurrentPage.value = 1
        
        if (filteredResults.value.length === 0) {
          showError(`Không có dữ liệu ${dataType} cho ngày ${formatDate(selectedFromDate.value)}`)
        } else {
          showSuccess(`Hiển thị ${filteredResults.value.length} import(s) cho loại ${dataType} ngày ${formatDate(selectedFromDate.value)}`)
        }
      } else {
        showError(`Lỗi khi tải dữ liệu: ${result.error}`)
        filteredResults.value = []
      }
    } else {
      // Filter current results by data type
      const dataTypeResults = allImports.value.filter(imp => imp.dataType === dataType)
      filteredResults.value = dataTypeResults
      filteredResultsCurrentPage.value = 1
      
      if (dataTypeResults.length === 0) {
        showError(`Chưa có dữ liệu import nào cho loại ${dataType}`)
        return
      }
      
      showSuccess(`Hiển thị ${dataTypeResults.length} import(s) cho loại ${dataType}`)
    }
    
  } catch (error) {
    console.error('Error loading data type:', error)
    showError(`Có lỗi xảy ra khi tải dữ liệu: ${error.message}`)
    filteredResults.value = []
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
    
    existingImports.value = checkResult.data.existingImports || []
    confirmationMessage.value = `Bạn có chắc chắn muốn xóa tất cả dữ liệu ${dataType} cho ngày ${formatDate(selectedFromDate.value)}?`
    confirmButtonText.value = '🗑️ Xóa dữ liệu'
    confirmCallback.value = () => performDeleteByDate(dataType, dateStr)
    showConfirmationModal.value = true
    
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
      showError(result.error || 'Không thể xóa dữ liệu')
    }
    
  } catch (error) {
    console.error('Error deleting by date:', error)
    showError('Có lỗi xảy ra khi xóa dữ liệu')
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

// Import methods
const openImportModal = (dataType) => {
  selectedDataType.value = dataType
  selectedFiles.value = []
  archivePassword.value = ''
  importNotes.value = ''
  useDefaultPassword.value = true // ✅ Mặc định tick checkbox
  showImportModal.value = true
  
  // ✅ Tự động điền mật khẩu mặc định khi mở modal
  onDefaultPasswordToggle()
}

const closeImportModal = () => {
  showImportModal.value = false
  selectedDataType.value = null
  selectedFiles.value = []
  archivePassword.value = ''
  importNotes.value = ''
  useDefaultPassword.value = true // ✅ Reset về mặc định
  uploading.value = false
  uploadProgress.value = 0
  remainingTime.value = 0
  remainingTimeFormatted.value = '00:00'
}

// ✅ Xử lý checkbox tự động điền mật khẩu mặc định
const onDefaultPasswordToggle = () => {
  if (useDefaultPassword.value) {
    archivePassword.value = 'Snk6S4GV' // ✅ Điền mật khẩu mặc định
  } else {
    archivePassword.value = '' // ✅ Xóa mật khẩu nếu bỏ tick
  }
}

const handleFileSelect = (event) => {
  const files = Array.from(event.target.files)
  addFiles(files)
}

const handleFileDrop = (event) => {
  event.preventDefault()
  const files = Array.from(event.dataTransfer.files)
  addFiles(files)
}

const addFiles = (files) => {
  const validFiles = files.filter(file => {
    // Validate file for selected data type
    const validation = rawDataService.validateFile(file, selectedDataType.value)
    if (!validation.valid) {
      showError(validation.error)
      return false
    }
    return true
  })
  
  selectedFiles.value = [...selectedFiles.value, ...validFiles]
}

const removeFile = (index) => {
  selectedFiles.value.splice(index, 1)
}

const getFileIcon = (fileName) => {
  const ext = fileName.split('.').pop().toLowerCase()
  const icons = {
    'xlsx': '📊', 'xls': '📊', 'csv': '📋', 
    'zip': '🗜️', '7z': '🗜️', 'rar': '🗜️'
  }
  return icons[ext] || '📄'
}

const formatFileSize = (bytes) => {
  if (bytes === 0) return '0 Bytes'
  const k = 1024
  const sizes = ['Bytes', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return `${parseFloat((bytes / Math.pow(k, i)).toFixed(2))} ${sizes[i]}`
}

const isArchiveFile = (fileName) => {
  return rawDataService.isArchiveFile(fileName)
}

const getAcceptTypes = () => {
  if (!selectedDataType.value) return ''
  const dataType = dataTypeDefinitions[selectedDataType.value]
  return [...dataType.acceptedFormats, '.zip', '.7z', '.rar'].join(',')
}

const performImport = async () => {
  if (selectedFiles.value.length === 0) {
    showError('Vui lòng chọn ít nhất một file')
    return
  }
  
  // Extract statement dates from filenames and check for duplicates
  const fileDates = []
  for (const file of selectedFiles.value) {
    const dateMatch = file.name.match(/(\d{8})/)
    if (dateMatch) {
      const dateStr = dateMatch[1]
      fileDates.push(dateStr)
      
      // Check for duplicate với fileName chính xác
      try {
        const checkResult = await rawDataService.checkDuplicateData(selectedDataType.value, dateStr, file.name)
        if (checkResult.success && checkResult.data.hasDuplicate) {
          existingImports.value = checkResult.data.existingImports || []
          confirmationMessage.value = `File "${file.name}" đã được import trước đó. Bạn có muốn ghi đè không?`
          confirmButtonText.value = '✅ Ghi đè dữ liệu'
          confirmCallback.value = () => executeImport()
          showConfirmationModal.value = true
          return
        }
      } catch (error) {
        console.warn('Error checking duplicate:', error)
        // Continue with import if check fails
      }
    }
  }
  
  // No duplicates found, proceed with import
  await executeImport()
}

const executeImport = async () => {
  try {
    uploading.value = true
    uploadProgress.value = 0
    loadingMessage.value = 'Đang chuẩn bị upload...'
    
    const result = await rawDataService.importData(selectedDataType.value, selectedFiles.value, {
      archivePassword: archivePassword.value,
      notes: importNotes.value,
      onProgress: (progress) => {
        uploadProgress.value = progress.percentage
        
        if (progress.isNearCompletion) {
          loadingMessage.value = `🎯 Sắp hoàn thành... ${progress.percentage}%`
        } else {
          loadingMessage.value = `📤 Đang upload: ${progress.percentage}% - ${progress.formattedSpeed} - Còn lại: ${progress.remainingTimeFormatted}`
        }
        
        // Cập nhật remaining time trên UI
        if (progress.remainingTime > 0) {
          remainingTime.value = progress.remainingTime
          remainingTimeFormatted.value = progress.remainingTimeFormatted
        }
      }
    })
    
    if (result.success) {
      uploadProgress.value = 100
      loadingMessage.value = '🎉 Upload hoàn tất! Đang xử lý dữ liệu...'
      
      // 🗑️ Kiểm tra nếu có file nén bị xóa và hiển thị thông báo đặc biệt
      const archiveDeletedResults = result.data.results?.filter(r => r.isArchiveDeleted) || []
      
      if (archiveDeletedResults.length > 0) {
        // Hiển thị thông báo xóa file nén với thời gian ngắn (2s)
        archiveDeletedResults.forEach(archiveResult => {
          showSuccess(`🗑️ File nén "${archiveResult.fileName}" đã được xóa tự động sau khi import thành công`, 2000)
        })
      }
      
      showSuccess(`✅ Import thành công! Đã xử lý ${result.data.results?.length || 1} file(s)`)
      closeImportModal()
      
      // Thêm delay để đảm bảo dữ liệu đã được lưu
      loadingMessage.value = 'Đang tải lại dữ liệu...'
      await new Promise(resolve => setTimeout(resolve, 2000))
      await refreshAllData()
    } else {
      showError(result.error || 'Import thất bại')
    }
    
  } catch (error) {
    console.error('Import error:', error)
    showError('Có lỗi xảy ra khi import dữ liệu')
  } finally {
    uploading.value = false
    loadingMessage.value = ''
  }
}

// Preview methods
const previewImport = async (importItem) => {
  try {
    loading.value = true
    loadingMessage.value = 'Đang tải preview...'
    
    console.log('👁️ Previewing import:', importItem)
    
    const result = await rawDataService.previewData(importItem.id)
    console.log('📋 Preview result:', result)
    console.log('📋 Result data type:', typeof result.data, 'isArray:', Array.isArray(result.data))
    console.log('📋 Result data content:', result.data)
    
    if (result.success) {
      selectedImport.value = importItem
      
      // 🔧 Xử lý nhiều format dữ liệu từ backend với debugging
      let records = [];
      
      // Helper function để convert $values format nếu cần
      const convertDotNetArray = (data) => {
        console.log('🔧 convertDotNetArray input:', typeof data, Array.isArray(data), data)
        if (data && typeof data === 'object' && data.$values && Array.isArray(data.$values)) {
          console.log('🔧 Converting $values format, length:', data.$values.length)
          return data.$values;
        }
        return data;
      };
      
      // ✅ Ưu tiên previewRows trước vì đây là data thật
      if (result.data.previewRows && Array.isArray(result.data.previewRows) && result.data.previewRows.length > 0) {
        console.log('📝 Processing previewRows path (priority):', typeof result.data.previewRows, Array.isArray(result.data.previewRows))
        console.log('📝 previewRows content:', result.data.previewRows)
        console.log('📝 previewRows length:', result.data.previewRows?.length)
        
        records = result.data.previewRows;
        console.log('📝 Using previewRows directly:', records.length, 'items')
      } else if (result.data.records) {
        console.log('📝 Processing records path:', typeof result.data.records, Array.isArray(result.data.records))
        let rawRecords = convertDotNetArray(result.data.records);
        records = Array.isArray(rawRecords) ? rawRecords : [];
      } else if (Array.isArray(result.data)) {
        console.log('📝 Processing direct array path')
        records = result.data;
      } else {
        // Thử convert toàn bộ result.data nếu nó có $values
        console.log('📝 Processing fallback conversion')
        let converted = convertDotNetArray(result.data);
        records = Array.isArray(converted) ? converted : [];
      }
      
      console.log('🔧 Final processed records:', records.length, 'items')
      console.log('🔧 Sample record:', records[0])
      
      // Đảm bảo records là một array thuần túy (không phải proxy)
      previewData.value = [...records]
      showPreviewModal.value = true
      
      console.log('✅ Preview data loaded:', previewData.value.length, 'records')
      console.log('✅ Preview data is array:', Array.isArray(previewData.value))
      
      showSuccess(`Đã tải ${previewData.value.length} bản ghi từ ${importItem.fileName}`)
    } else {
      console.error('❌ Preview failed:', result.error)
      showError(`Lỗi khi tải preview: ${result.error || 'Không thể lấy dữ liệu thô'}`)
    }
    
  } catch (error) {
    console.error('❌ Error loading preview:', error)
    showError(`Có lỗi xảy ra khi tải preview: ${error.message}`)
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

const closePreviewModal = () => {
  showPreviewModal.value = false
  selectedImport.value = null
  previewData.value = []
}

const deleteImport = async (importItem) => {
  confirmationMessage.value = `Bạn có chắc chắn muốn xóa import "${importItem.fileName}"?`
  confirmButtonText.value = '🗑️ Xóa'
  confirmCallback.value = () => executeDeleteImport(importItem)
  showConfirmationModal.value = true
}

const executeDeleteImport = async (importItem) => {
  try {
    loading.value = true
    loadingMessage.value = 'Đang xóa dữ liệu...'
    
    const result = await rawDataService.deleteImport(importItem.id)
    if (result.success) {
      showSuccess('Đã xóa import thành công')
      await refreshAllData()
      
      // Remove from filtered results
      filteredResults.value = filteredResults.value.filter(item => item.id !== importItem.id)
    } else {
      showError(result.error || 'Không thể xóa import')
    }
    
  } catch (error) {
    console.error('Error deleting import:', error)
    showError('Có lỗi xảy ra khi xóa dữ liệu')
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

// Confirmation modal methods
const confirmAction = async () => {
  showConfirmationModal.value = false
  if (confirmCallback.value) {
    await confirmCallback.value()
  }
}

const cancelConfirmation = () => {
  showConfirmationModal.value = false
  confirmCallback.value = null
  existingImports.value = []
}

// View raw data from table
const viewRawDataFromTable = async (dataType) => {
  try {
    loading.value = true
    loadingMessage.value = `Đang tải dữ liệu thô ${dataType}...`
    
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
    console.log('🗄️ Result data type:', typeof result.data, 'isArray:', Array.isArray(result.data))
    
    if (result.success && result.data) {
      // Helper function để convert $values format nếu cần
      const convertDotNetArray = (data) => {
        if (data && typeof data === 'object' && data.$values && Array.isArray(data.$values)) {
          console.log('🔧 Converting raw data $values format, length:', data.$values.length)
          return data.$values;
        }
        return data;
      };
      
      // Xử lý dữ liệu records từ backend mock data
      let records = [];
      if (result.data.records) {
        console.log('📝 Processing raw records path:', typeof result.data.records)
        let rawRecords = convertDotNetArray(result.data.records);
        records = Array.isArray(rawRecords) ? rawRecords : [];
      } else if (Array.isArray(result.data)) {
        console.log('📝 Processing direct array path for raw data')
        records = result.data;
      } else {
        console.log('📝 Processing fallback conversion for raw data')
        let converted = convertDotNetArray(result.data);
        records = Array.isArray(converted) ? converted : [];
      }
      
      console.log('🔧 Final processed raw records:', records.length, 'items')
      
      if (records.length === 0) {
        showError(`Không có dữ liệu thô ${dataType} cho ngày ${formatDate(selectedFromDate.value)}`)
        loading.value = false
        loadingMessage.value = ''
        return
      }
      
      // Show raw data in a modal
      selectedImport.value = {
        id: 'table-' + dataType,
        fileName: `Bảng ${dataType} - ${formatDate(selectedFromDate.value)}`,
        dataType: dataType,
        importDate: new Date().toISOString(),
        statementDate: selectedFromDate.value,
        importedBy: 'System'
      }
      
      // Đảm bảo records là một array thuần túy (không phải proxy)
      previewData.value = [...records]
      showPreviewModal.value = true
      
      console.log('✅ Raw data loaded:', previewData.value.length, 'records from table:', result.data.tableName || 'Mock Table')
      showSuccess(`Đã tải ${previewData.value.length} bản ghi từ bảng ${result.data.tableName || 'Mock_' + dataType}`)
    } else {
      console.error('❌ Raw data from table failed:', result.error)
      showError(`Lỗi khi lấy dữ liệu thô: ${result.error || 'Bảng không tồn tại hoặc chưa có dữ liệu'}`)
    }
    
  } catch (error) {
    console.error('❌ Error loading raw data from table:', error)
    showError(`Có lỗi xảy ra khi tải dữ liệu thô: ${error.message}`)
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

// Utility methods
const getDataTypeColor = (dataType) => {
  return rawDataService.getDataTypeColor(dataType)
}

const formatNumber = (num) => {
  return rawDataService.formatRecordCount(num)
}

const formatDate = (dateString) => {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleDateString('vi-VN')
}

const formatDateFromString = (dateStr) => {
  if (!dateStr || dateStr.length !== 8) return dateStr
  const year = dateStr.substring(0, 4)
  const month = dateStr.substring(4, 6)
  const day = dateStr.substring(6, 8)
  return `${day}/${month}/${year}`
}

const getStatusText = (status) => {
  const statusMap = {
    'Pending': 'Đang xử lý',
    'Completed': 'Hoàn thành', 
    'Failed': 'Thất bại',
    'Processing': 'Đang import'
  }
  return statusMap[status] || status
}

const getStatusClass = (status) => {
  const statusMap = {
    'Pending': 'status-pending',
    'Completed': 'status-completed',
    'Failed': 'status-failed', 
    'Processing': 'status-processing'
  }
  return statusMap[status] || 'status-pending'
}

// Lifecycle
onMounted(async () => {
  await refreshAllData()
})
</script>

<style scoped>
/* Raw Data Warehouse Styles */
.raw-data-warehouse {
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
  padding: 24px;
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  min-height: 100vh;
}

/* Header */
.header-section {
  text-align: center;
  margin-bottom: 32px;
}

.header-section h1 {
  color: #8B1538;
  font-size: 2.5rem;
  font-weight: 700;
  margin-bottom: 10px;
}

.subtitle {
  color: #6c757d;
  font-size: 1.1rem;
  font-weight: 500;
}

/* Alerts */
.alert {
  padding: 16px 20px;
  border-radius: 8px;
  margin-bottom: 20px;
  display: flex;
  align-items: center;
  gap: 12px;
  position: relative;
}

.alert-error {
  background: #f8d7da;
  border: 1px solid #f5c6cb;
  color: #721c24;
}

.alert-success {
  background: #d4edda;
  border: 1px solid #c3e6cb;
  color: #155724;
}

.alert-close {
  position: absolute;
  right: 16px;
  background: none;
  border: none;
  font-size: 1.2rem;
  cursor: pointer;
  color: inherit;
}

/* Loading */
.loading-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 20px;
  text-align: center;
}

.loading-spinner {
  width: 60px;
  height: 60px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #8B1538;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 20px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

/* Control Panel */
.control-panel {
  background: white;
  border-radius: 16px;
  padding: 24px;
  margin-bottom: 32px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  border: 1px solid rgba(139, 21, 56, 0.1);
}

.date-control-section,
.bulk-actions-section {
  margin-bottom: 24px;
}

.date-control-section:last-child,
.bulk-actions-section:last-child {
  margin-bottom: 0;
}

.date-control-section h3,
.bulk-actions-section h3 {
  color: #8B1538;
  font-size: 1.2rem;
  font-weight: 600;
  margin-bottom: 16px;
}

.date-controls,
.bulk-actions {
  display: flex;
  align-items: center;
  gap: 16px;
  flex-wrap: wrap;
}

.date-range {
  display: flex;
  align-items: center;
  gap: 12px;
}

.date-range label {
  font-weight: 500;
  color: #495057;
}

.date-input {
  padding: 8px 12px;
  border: 2px solid #e9ecef;
  border-radius: 6px;
  font-size: 14px;
}

.date-input:focus {
  outline: none;
  border-color: #8B1538;
}

.btn-filter,
.btn-clear,
.btn-clear-all,
.btn-refresh {
  padding: 10px 16px;
  border: none;
  border-radius: 6px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
  font-size: 14px;
}

.btn-filter {
  background: #8B1538;
  color: white;
}

.btn-filter:hover:not(:disabled) {
  background: #A91B47;
}

.btn-filter:disabled {
  background: #6c757d;
  cursor: not-allowed;
}

.btn-clear {
  background: #6c757d;
  color: white;
}

.btn-clear:hover {
  background: #5a6268;
}

.btn-clear-all {
  background: #dc3545;
  color: white;
}

.btn-clear-all:hover:not(:disabled) {
  background: #c82333;
}

.btn-refresh {
  background: #28a745;
  color: white;
}

.btn-refresh:hover:not(:disabled) {
  background: #218838;
}

/* Data Types Section */
.data-types-section,
.filtered-results-section {
  background: white;
  border-radius: 16px;
  padding: 32px;
  margin-bottom: 32px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  border: 1px solid rgba(139, 21, 56, 0.1);
}

.section-header {
  margin-bottom: 24px;
}

.section-header h2 {
  color: #8B1538;
  font-size: 1.5rem;
  font-weight: 600;
  margin-bottom: 8px;
}

.section-header p {
  color: #6c757d;
  margin: 0;
}

/* Tables */
.data-types-table,
.results-table {
  border-radius: 8px;
  overflow: hidden;
  border: 1px solid #e9ecef;
}

.data-types-table table,
.results-table table {
  width: 100%;
  border-collapse: collapse;
}

.data-types-table th,
.results-table th {
  background: linear-gradient(135deg, #8B1538 0%, #A91B47 100%);
  color: white;
  padding: 16px 12px;
  text-align: left;
  font-weight: 600;
  font-size: 0.9rem;
}

.data-types-table td,
.results-table td {
  padding: 14px 12px;
  border-bottom: 1px solid #f8f9fa;
  vertical-align: middle;
}

.data-types-table tr:hover,
.results-table tr:hover {
  background: #f8f9fa;
}

.data-type-info {
  display: flex;
  align-items: center;
  gap: 8px;
}

.data-type-info strong {
  font-size: 1.2rem;
  font-weight: 800;
  color: #1a1a1a;
  letter-spacing: 0.3px;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

.data-type-icon {
  font-size: 1.5rem;
}

.file-formats {
  font-size: 0.85rem;
  color: #6c757d;
  font-style: italic;
}

.records-cell {
  text-align: center;
}

.records-count {
  font-weight: 600;
  color: #8B1538;
}

.description-cell {
  font-size: 1.05rem;
  font-weight: 600;
  color: #1a1a1a;
  line-height: 1.4;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}

.last-update-cell {
  font-size: 1rem;
  font-weight: 600;
  color: #2c3e50;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}

.actions-cell {
  display: flex;
  gap: 8px;
}

.btn-action {
  width: 80px;
  height: 32px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
  font-size: 12px;
  font-weight: 500;
}

.btn-view { 
  background: #17a2b8; 
  color: white; 
}

.btn-import { 
  background: #28a745; 
  color: white; 
}

.btn-delete { 
  background: #dc3545; 
  color: white; 
}

.btn-preview { 
  background: #17a2b8; 
  color: white; 
}

.btn-raw-view { 
  background: #6f42c1; 
  color: white;
  font-size: 11px;
}

.btn-action:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.btn-action:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  transform: none;
}

/* Data type badges */
.data-type-badge {
  background: #28a745;
  color: white;
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 0.8rem;
  font-weight: 500;
}

.filename-cell {
  max-width: 200px;
}

.filename {
  font-weight: 500;
  display: block;
}

/* Status badges */
.status-badge {
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 0.8rem;
  font-weight: 500;
}

.status-completed {
  background: #d4edda;
  color: #155724;
}

.status-failed {
  background: #f8d7da;
  color: #721c24;
}

.status-pending {
  background: #fff3cd;
  color: #856404;
}

.status-processing {
  background: #cce5ff;
  color: #004085;
}

/* Pagination */
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 8px;
  margin-top: 16px;
}

.pagination-btn {
  background: #f8f9fa;
  border: 1px solid #dee2e6;
  color: #495057;
  padding: 8px 12px;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.pagination-btn:hover:not(:disabled) {
  background: #e9ecef;
  border-color: #adb5bd;
}

.pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-info {
  font-weight: 500;
  color: #495057;
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.6);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  backdrop-filter: blur(4px);
}

.modal-content {
  background: white;
  border-radius: 16px;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
  max-width: 90vw;
  max-height: 90vh;
  overflow: hidden;
  animation: modalSlideIn 0.3s ease-out;
}

.modal-large {
  width: 1000px;
}

.modal-header {
  background: linear-gradient(135deg, #8B1538 0%, #A91B47 100%);
  color: white;
  padding: 20px 24px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h3 {
  margin: 0;
  font-size: 1.3rem;
  font-weight: 600;
}

.modal-close {
  background: none;
  border: none;
  color: white;
  font-size: 1.5rem;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: background-color 0.3s ease;
}

.modal-close:hover {
  background: rgba(255, 255, 255, 0.2);
}

.modal-body {
  padding: 24px;
  max-height: calc(90vh - 120px);
  overflow-y: auto;
}

.modal-footer {
  padding: 20px 24px;
  border-top: 1px solid #e9ecef;
  display: flex;
  gap: 12px;
  justify-content: flex-end;
}

/* Import Form */
.import-form {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-weight: 600;
  color: #2c3e50;
}

.upload-area {
  border: 2px dashed #8B1538;
  border-radius: 12px;
  padding: 40px;
  text-align: center;
  background: linear-gradient(135deg, #fafafa 0%, #f5f5f5 100%);
  transition: all 0.3s ease;
  cursor: pointer;
}

.upload-area:hover {
  border-color: #A91B47;
  background: linear-gradient(135deg, #f0f8ff 0%, #e6f3ff 100%);
}

.upload-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
}

.upload-icon {
  font-size: 3rem;
  opacity: 0.6;
}

.upload-hint {
  font-size: 0.9rem;
  color: #6c757d;
  margin: 0;
}

/* Selected Files */
.selected-files {
  margin-top: 20px;
  padding: 20px;
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  border-radius: 12px;
  border: 2px solid #e9ecef;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.selected-files h4 {
  margin: 0 0 16px 0;
  color: #8B1538;
  font-size: 1.1rem;
  font-weight: 700;
  display: flex;
  align-items: center;
  gap: 8px;
}

.file-count-badge {
  background: linear-gradient(135deg, #8B1538 0%, #A91B47 100%);
  color: white;
  font-size: 0.8rem;
  font-weight: 600;
  padding: 4px 8px;
  border-radius: 12px;
  min-width: 20px;
  text-align: center;
  box-shadow: 0 2px 4px rgba(139, 21, 56, 0.3);
}

.file-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 16px;
  background: white;
  border-radius: 10px;
  margin-bottom: 10px;
  border: 1px solid #e9ecef;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.05);
  transition: all 0.3s ease;
}

.file-item:hover {
  transform: translateY(-1px);
  box-shadow: 0 3px 8px rgba(0, 0, 0, 0.1);
  border-color: #8B1538;
}

.file-icon {
  font-size: 1.4rem;
  min-width: 24px;
  text-align: center;
}

.file-name {
  flex: 1;
  font-weight: 600;
  color: #2c3e50;
  font-size: 0.95rem;
  word-break: break-word;
}

.file-size {
  font-size: 0.85rem;
  color: #6c757d;
  font-weight: 500;
  background: #f8f9fa;
  padding: 4px 8px;
  border-radius: 12px;
  white-space: nowrap;
}

.btn-remove-file {
  background: linear-gradient(135deg, #dc3545 0%, #c82333 100%);
  color: white;
  border: none;
  width: 28px;
  height: 28px;
  border-radius: 50%;
  cursor: pointer;
  font-size: 16px;
  font-weight: 700;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
  box-shadow: 0 2px 4px rgba(220, 53, 69, 0.3);
  min-width: 28px;
}

.btn-remove-file:hover {
  background: linear-gradient(135deg, #c82333 0%, #bd2130 100%);
  transform: scale(1.1);
  box-shadow: 0 3px 8px rgba(220, 53, 69, 0.4);
}

.form-input,
.form-textarea {
  width: 100%;
  padding: 12px 16px;
  border: 2px solid #e9ecef;
  border-radius: 8px;
  font-size: 14px;
  transition: border-color 0.3s ease;
}

.form-input:focus,
.form-textarea:focus {
  outline: none;
  border-color: #8B1538;
}

.form-hint {
  font-size: 0.8rem;
  color: #6c757d;
  font-style: italic;
}

/* ✅ CSS cho checkbox tự động điền mật khẩu */
.auto-password-section {
  margin-bottom: 12px;
}

.checkbox-wrapper {
  display: flex;
  align-items: center;
  cursor: pointer;
  margin-bottom: 8px;
}

.checkbox-wrapper input[type="checkbox"] {
  display: none;
}

.checkmark {
  position: relative;
  width: 20px;
  height: 20px;
  border: 2px solid #8B1538;
  border-radius: 4px;
  margin-right: 8px;
  transition: all 0.3s ease;
  background: white;
}

.checkbox-wrapper input[type="checkbox"]:checked + .checkmark {
  background: #8B1538;
  border-color: #8B1538;
}

.checkbox-wrapper input[type="checkbox"]:checked + .checkmark::after {
  content: '✓';
  position: absolute;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
  color: white;
  font-weight: bold;
  font-size: 14px;
}

.checkbox-label {
  font-size: 14px;
  color: #495057;
  user-select: none;
}

.checkbox-wrapper:hover .checkmark {
  border-color: #6c1a2f;
  box-shadow: 0 2px 4px rgba(139, 21, 56, 0.2);
}

.form-input.auto-filled {
  background: linear-gradient(135deg, #e8f5e8 0%, #f0fff0 100%);
  border-color: #28a745;
  color: #155724;
}

.form-input.auto-filled::placeholder {
  color: #6c757d;
  opacity: 0.7;
}

/* Preview Content */
.preview-info {
  margin-bottom: 24px;
  padding: 16px;
  background: #f8f9fa;
  border-radius: 8px;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.info-item label {
  font-size: 0.9rem;
  color: #6c757d;
  font-weight: 500;
}

.table-wrapper {
  overflow: auto;
  max-height: 400px;
  border: 1px solid #e9ecef;
  border-radius: 8px;
}

.table-wrapper table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
}

.table-wrapper th {
  background: #f8f9fa;
  padding: 12px;
  text-align: left;
  font-weight: 600;
  color: #8B1538;
  border-bottom: 2px solid #e9ecef;
  position: sticky;
  top: 0;
}

.table-wrapper td {
  padding: 10px 12px;
  border-bottom: 1px solid #f1f3f4;
}

.preview-note {
  margin-top: 16px;
  padding: 12px;
  background: #fff3cd;
  border: 1px solid #ffeaa7;
  border-radius: 6px;
  color: #856404;
  text-align: center;
}

.no-data,
.no-preview-data {
  text-align: center;
  padding: 40px 20px;
  color: #6c757d;
}

.empty-icon {
  font-size: 4rem;
  margin-bottom: 16px;
  opacity: 0.6;
}

/* Existing imports display in confirmation modal */
.existing-imports {
  margin-top: 16px;
  padding: 16px;
  background: #f8f9fa;
  border-radius: 8px;
}

.existing-imports h4 {
  margin: 0 0 12px 0;
  color: #8B1538;
  font-size: 1.1rem;
  font-weight: 700;
}

.existing-imports ul {
  margin: 0;
  padding-left: 20px;
}

.existing-imports li {
  margin-bottom: 8px;
  color: #495057;
  font-size: 0.95rem;
  font-weight: 500;
  line-height: 1.4;
}

/* Enhanced confirmation modal styling */
.modal-content h3 {
  font-size: 1.25rem;
  font-weight: 700;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

.modal-body p {
  font-size: 1.05rem;
  font-weight: 500;
  color: #2c3e50;
  line-height: 1.5;
  text-shadow: 0 1px 1px rgba(0, 0, 0, 0.05);
}

/* Upload Progress Styles */
.upload-progress-section {
  margin-top: 20px;
  padding: 20px;
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  border-radius: 12px;
  border: 1px solid #dee2e6;
}

.progress-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
}

.progress-header h4 {
  margin: 0;
  color: #495057;
  font-size: 16px;
  font-weight: 600;
}

.progress-stats {
  display: flex;
  gap: 15px;
  align-items: center;
}

.progress-percentage {
  font-size: 18px;
  font-weight: bold;
  color: #28a745;
}

.remaining-time {
  font-size: 14px;
  color: #6c757d;
  background: rgba(255, 255, 255, 0.8);
  padding: 4px 12px;
  border-radius: 20px;
  border: 1px solid #dee2e6;
}

.progress-bar-container {
  width: 100%;
  height: 12px;
  background: #e9ecef;
  border-radius: 20px;
  overflow: hidden;
  margin-bottom: 12px;
  box-shadow: inset 0 2px 4px rgba(0,0,0,0.1);
}

.progress-bar {
  height: 100%;
  background: linear-gradient(90deg, #28a745 0%, #20c997 50%, #17a2b8 100%);
  transition: width 0.3s ease;
  border-radius: 20px;
  box-shadow: 0 2px 8px rgba(40, 167, 69, 0.3);
  animation: progressShimmer 2s ease-in-out infinite;
}

@keyframes progressShimmer {
  0% { box-shadow: 0 2px 8px rgba(40, 167, 69, 0.3); }
  50% { box-shadow: 0 2px 12px rgba(40, 167, 69, 0.5); }
  100% { box-shadow: 0 2px 8px rgba(40, 167, 69, 0.3); }
}

.progress-message {
  font-size: 14px;
  color: #6c757d;
  text-align: center;
  font-style: italic;
}

/* Animations */
@keyframes modalSlideIn {
  from {
    opacity: 0;
    transform: scale(0.9) translateY(-20px);
  }
  to {
    opacity: 1;
    transform: scale(1) translateY(0);
  }
}

/* Responsive */
@media (max-width: 768px) {
  .raw-data-warehouse {
    padding: 16px;
  }
  
  .modal-large {
    width: 95vw;
  }
  
  .info-grid {
    grid-template-columns: 1fr;
  }
  
  .date-controls,
  .bulk-actions {
    flex-direction: column;
    align-items: stretch;
  }
  
  .actions-cell {
    flex-direction: column;
  }
  
  .btn-action {
    width: 100%;
  }
}
</style>
