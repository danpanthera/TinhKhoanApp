<template>
  <div class="raw-data-warehouse">
    <!-- Header Section -->
    <div class="header-section">
      <h1>🏦 KHO DỮ LIỆU THÔ - AGRIBANK LAI CHAU</h1>
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
          <button @click="debugRecalculateStats" class="btn-debug" :disabled="loading" title="Debug: Force recalculate stats">
            🔧 Debug Stats
          </button>
        </div>
      </div>
    </div>

    <!-- Data Types List -->
    <div class="data-types-section">
      <div class="section-header">
        <h2>📊 BẢNG QUẢN LÝ DỮ LIỆU NGHIỆP VỤ</h2>
        <p>Theo dõi và quản lý tất cả loại dữ liệu của hệ thống Agribank Lai Chau</p>
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
          <button @click="closeImportModal" class="btn-cancel btn-large">🚫 Hủy</button>
          <button 
            @click="performImport" 
            class="btn-import-confirm btn-large"
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

// Debug function để test calculate stats
const debugRecalculateStats = () => {
  console.log('🔧 DEBUG: Manual recalculate stats')
  console.log('📊 Current allImports:', allImports.value.length, allImports.value)
  console.log('📋 DataTypeDefinitions keys:', Object.keys(dataTypeDefinitions))
  
  calculateDataTypeStats()
  
  console.log('📈 After recalculate - dataTypeStats:', dataTypeStats.value)
  console.log('🔍 LN01 specifically:', getDataTypeStats('LN01'))
  
  showSuccess(`🔧 Debug: Recalculated stats. Check console for details.`)
}

// Data type statistics
const getDataTypeStats = (dataType) => {
  return dataTypeStats.value[dataType] || { totalRecords: 0, lastUpdate: null }
}

const calculateDataTypeStats = () => {
  console.log('🔧 Calculating data type stats from imports:', allImports.value.length)
  const stats = {}
  
  // Initialize all data types để hiển thị 0 nếu không có dữ liệu
  Object.keys(dataTypeDefinitions).forEach(key => {
    stats[key] = { totalRecords: 0, lastUpdate: null, count: 0 }
  })
  
  // Calculate from imports với logic cải thiện - FIX MAPPING VẤN ĐỀ
  allImports.value.forEach(imp => {
    // 🔧 FIX TRIỆT ĐỂ: Backend trả về category="LN01", không có dataType hoặc fileType có ý nghĩa
    // Ưu tiên category trước, sau đó fileType, rồi dataType 
    const dataType = imp.category || imp.dataType || imp.fileType || 'UNKNOWN'
    console.log(`📊 Processing import: ${imp.fileName}, category: ${imp.category}, fileType: ${imp.fileType}, dataType: ${imp.dataType}, final: ${dataType}, records: ${imp.recordsCount}`)
    
    // Nếu chưa có stats cho data type này, khởi tạo
    if (!stats[dataType]) {
      stats[dataType] = { totalRecords: 0, lastUpdate: null, count: 0 }
    }
    
    // ✅ Đếm số lượng imports
    stats[dataType].count++
    
    // 🔧 Sử dụng đúng field name từ backend: recordsCount và đảm bảo là số
    const recordCount = parseInt(imp.recordsCount) || 0
    stats[dataType].totalRecords += recordCount
    
    // 🔧 Handle invalid dates từ backend (0001-01-01) và cập nhật lastUpdate
    const importDate = imp.importDate;
    if (importDate && importDate !== "0001-01-01T00:00:00") {
      const importDateTime = new Date(importDate)
      if (!stats[dataType].lastUpdate || 
          importDateTime > new Date(stats[dataType].lastUpdate)) {
        stats[dataType].lastUpdate = importDate
      }
    }
  })
  
  console.log('📈 Final calculated stats:', stats)
  dataTypeStats.value = stats
  
  // 🔧 Force reactive update để đảm bảo UI refresh
  console.log('🔄 Forcing reactive update for UI...')
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
    
    console.log('🔄 Starting refresh all data...')
    
    // 🔧 Clear cache trước khi load để luôn có dữ liệu mới nhất
    localStorage.removeItem('rawDataCache')
    
    const result = await rawDataService.getAllImports()
    console.log('📊 Raw result from getAllImports:', result)

    if (result.success) {
      allImports.value = result.data || []
      console.log('✅ Loaded imports:', allImports.value.length, 'items')
      console.log('🔍 Sample import data:', allImports.value.length > 0 ? allImports.value[0] : 'No data')
      
      // Force recalculation of stats sau khi có dữ liệu mới
      calculateDataTypeStats()
      
      // 🔧 DEBUG: Log stats sau khi calculate
      console.log('📈 Stats after calculation:', dataTypeStats.value)
      console.log('📊 LN01 stats specifically:', dataTypeStats.value['LN01'])
      
      // Also refresh filtered results if there are any filters active
      if (selectedFromDate.value) {
        console.log('🔍 Reapplying date filter...')
        await applyDateFilter()
      }
      
      showSuccess(`✅ Đã tải lại dữ liệu thành công (${allImports.value.length} imports)`)
    } else {
      // Hiển thị thông báo lỗi chi tiết
      const errorMsg = result.error || 'Không thể tải dữ liệu'
      console.error('🔥 Chi tiết lỗi:', {
        error: result.error,
        errorCode: result.errorCode,
        errorStatus: result.errorStatus
      })
      
      // Sử dụng mock data để demo vẫn hoạt động nếu có
      if (result.fallbackData && result.fallbackData.length > 0) {
        allImports.value = result.fallbackData
        calculateDataTypeStats()
        showError(`⚠️ Chế độ Demo: ${errorMsg}`)
        console.info('🎭 Sử dụng mock data cho demo')
      } else {
        allImports.value = []
        calculateDataTypeStats()
        showError(errorMsg)
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
      // 🔧 Reset cache và force clear triệt để
      allImports.value = []
      filteredResults.value = []
      dataTypeStats.value = {}
      
      // ✅ Xóa tất cả cache có thể
      localStorage.removeItem('rawDataCache')
      localStorage.removeItem('dataTypeStats')  
      localStorage.removeItem('lastRefresh')
      sessionStorage.clear() // Clear session cache
      
      // Thông báo chi tiết từ backend
      const data = result.data || result
      const message = `✅ Đã xóa thành công ${data.recordsCleared || 0} bản ghi import, ${data.itemsCleared || 0} items dữ liệu${data.dynamicTablesCleared ? ` và ${data.dynamicTablesCleared} bảng dữ liệu động` : ''}`
      showSuccess(message, 5000) // Hiển thị lâu hơn để user đọc
      
      console.log('🗑️ Clear completed. Details:', data)
      
      // Force refresh sau delay để đảm bảo DB đã update hoàn toàn
      setTimeout(async () => {
        console.log('🔄 Force refreshing data after clear...')
        loadingMessage.value = 'Đang tải lại dữ liệu sau khi xóa...'
        
        await refreshAllData()
        
        // Force tính toán lại stats để đảm bảo hiển thị 0
        calculateDataTypeStats()
        
        console.log('✅ Refresh after clear completed')
        loadingMessage.value = ''
      }, 1500) // Tăng delay để chắc chắn
      
    } else {
      showError(result.message || result.error || 'Không thể xóa dữ liệu')
      console.error('❌ Clear failed:', result)
    }
    
  } catch (error) {
    console.error('❌ Error clearing all data:', error)
    showError('Có lỗi xảy ra khi xóa dữ liệu: ' + error.message)
  } finally {
    // Reset loading state sau một khoảng thời gian
    setTimeout(() => {
      loading.value = false
      loadingMessage.value = ''
    }, 2000)
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
      
      // 🔧 TĂNG THỜI GIAN DELAY để đảm bảo database đã được update hoàn toàn
      loadingMessage.value = 'Đang tải lại dữ liệu...'
      console.log('⏳ Waiting for database to update...')
      await new Promise(resolve => setTimeout(resolve, 4000)) // Tăng từ 2s lên 4s
      
      console.log('🔄 Now refreshing data after import...')
      await refreshAllData()
      
      // Force reload stats một lần nữa để chắc chắn
      console.log('🔄 Force recalculating stats...')
      calculateDataTypeStats()
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
/* 🏦 AGRIBANK BRAND STYLING - Header section */
.header-section {
  background: linear-gradient(135deg, #8B1538 0%, #C41E3A 50%, #8B1538 100%);
  color: white;
  padding: 40px 30px;
  text-align: center;
  margin-bottom: 30px;
  border-radius: 15px;
  box-shadow: 0 10px 30px rgba(139, 21, 56, 0.3);
  position: relative;
  overflow: hidden;
}

.header-section::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent 0%, rgba(255,255,255,0.1) 50%, transparent 100%);
  animation: shimmer 3s infinite;
}

@keyframes shimmer {
  0% { left: -100%; }
  100% { left: 100%; }
}

.header-section h1 {
  font-size: 2.8rem;
  font-weight: 700;
  margin-bottom: 15px;
  font-family: 'Playfair Display', 'Georgia', serif;
  text-shadow: 0 3px 6px rgba(0, 0, 0, 0.3);
  letter-spacing: 0.02em;
}

.header-section .subtitle {
  font-size: 1.2rem;
  color: rgba(255, 255, 255, 0.9);
  margin: 0;
  font-weight: 400;
  font-style: italic;
}

/* 🏦 Alert styling với thương hiệu Agribank */
.alert {
  padding: 15px 20px;
  border-radius: 10px;
  margin-bottom: 20px;
  display: flex;
  align-items: center;
  gap: 12px;
  font-weight: 500;
  position: relative;
  overflow: hidden;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.alert::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  bottom: 0;
  width: 4px;
  background: currentColor;
}

.alert-success {
  background: linear-gradient(135deg, #d4edda 0%, #c3e6cb 100%);
  color: #155724;
  border: 1px solid #c3e6cb;
}

.alert-error {
  background: linear-gradient(135deg, #f8d7da 0%, #f5c6cb 100%);
  color: #721c24;
  border: 1px solid #f5c6cb;
}

.alert-icon {
  font-size: 1.2rem;
  flex-shrink: 0;
}

.alert-close {
  background: none;
  border: none;
  font-size: 1.2rem;
  cursor: pointer;
  color: currentColor;
  opacity: 0.7;
  margin-left: auto;
  padding: 0;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: all 0.3s ease;
}

.alert-close:hover {
  opacity: 1;
  background: rgba(0, 0, 0, 0.1);
}

/* Loading styling với thương hiệu Agribank */
.loading-section {
  text-align: center;
  padding: 40px 20px;
  background: linear-gradient(135deg, #f8f9fa 0%, #fff 100%);
  border-radius: 15px;
  margin-bottom: 25px;
  border: 2px solid #8B1538;
  box-shadow: 0 6px 20px rgba(139, 21, 56, 0.15);
}

.loading-spinner {
  width: 50px;
  height: 50px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #8B1538;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 20px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.loading-section p {
  color: #8B1538;
  font-weight: 600;
  font-size: 1.1rem;
  margin: 0;
}

/* Debug button */
.btn-debug {
  background: linear-gradient(45deg, #ff6b6b, #ffa726);
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(255, 107, 107, 0.3);
}

.btn-debug:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(255, 107, 107, 0.4);
  background: linear-gradient(45deg, #ff5252, #ff9800);
}

.btn-debug:active {
  transform: translateY(0);
}

.btn-debug:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none;
}

/* Existing styles for other buttons */
.bulk-actions {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
}

.btn-clear-all, .btn-refresh {
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  border: none;
}

.btn-clear-all {
  background: linear-gradient(45deg, #dc3545, #c82333);
  color: white;
  box-shadow: 0 2px 8px rgba(220, 53, 69, 0.3);
}

.btn-refresh {
  background: linear-gradient(45deg, #28a745, #20c997);
  color: white;
  box-shadow: 0 2px 8px rgba(40, 167, 69, 0.3);
}

.btn-clear-all:hover, .btn-refresh:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

/* 🏦 AGRIBANK BRAND STYLING - Bảng Kho dữ liệu thô */

/* Container chính */
.data-import-view {
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  min-height: 100vh;
  font-family: 'Segoe UI', 'Arial', sans-serif;
}

/* Header section với thương hiệu Agribank */
.controls-section {
  background: linear-gradient(135deg, #8B1538 0%, #A6195C 50%, #B91D47 100%);
  color: white;
  padding: 30px;
  border-radius: 15px 15px 0 0;
  margin-bottom: 0;
  box-shadow: 0 8px 25px rgba(139, 21, 56, 0.3);
}

.controls-section h1 {
  color: white;
  font-size: 2.5rem;
  font-weight: 700;
  text-shadow: 0 3px 6px rgba(0, 0, 0, 0.3);
  margin-bottom: 10px;
  font-family: 'Playfair Display', 'Georgia', serif;
}

.controls-section .subtitle {
  color: rgba(255, 255, 255, 0.9);
  font-size: 1.1rem;
  margin-bottom: 20px;
}

/* Data types section - Table styling */
.data-types-section {
  background: white;
  border-radius: 0 0 15px 15px;
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.section-header {
  background: linear-gradient(135deg, #8B1538 0%, #C41E3A 100%);
  color: white;
  padding: 25px 30px;
  border-bottom: 3px solid #8B1538;
  position: relative;
  overflow: hidden;
}

.section-header::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(45deg, rgba(255,255,255,0.1) 0%, transparent 50%, rgba(255,255,255,0.1) 100%);
  pointer-events: none;
}

.section-header h2 {
  color: white;
  font-size: 1.8rem;
  font-weight: 600;
  margin-bottom: 8px;
  font-family: 'Playfair Display', 'Georgia', serif;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
  position: relative;
  z-index: 1;
}

.section-header p {
  color: rgba(255, 255, 255, 0.9);
  margin: 0;
  font-size: 1rem;
  position: relative;
  z-index: 1;
}

/* Table styling với thương hiệu Agribank */
.data-types-table {
  overflow-x: auto;
}

.data-types-table table {
  width: 100%;
  border-collapse: collapse;
  background: white;
  font-size: 14px;
}

.data-types-table thead tr {
  background: linear-gradient(135deg, #8B1538 0%, #A6195C 100%);
  color: white;
}

.data-types-table thead th {
  padding: 18px 15px;
  text-align: left;
  font-weight: 600;
  font-size: 14px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  border: none;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.3);
  position: relative;
}

.data-types-table thead th:after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 2px;
  background: linear-gradient(90deg, transparent 0%, rgba(255,255,255,0.3) 50%, transparent 100%);
}

.data-types-table tbody tr {
  border-bottom: 1px solid #e9ecef;
  transition: all 0.3s ease;
}

.data-types-table tbody tr:hover {
  background: linear-gradient(135deg, #f8f9fa 0%, #fff8f8 100%);
  transform: translateY(-1px);
  box-shadow: 0 4px 15px rgba(139, 21, 56, 0.1);
}

.data-types-table tbody tr:nth-child(even) {
  background: #fafbfc;
}

.data-types-table tbody tr:nth-child(even):hover {
  background: linear-gradient(135deg, #f8f9fa 0%, #fff5f5 100%);
}

.data-types-table tbody td {
  padding: 15px;
  vertical-align: middle;
  border: none;
}

/* Data type info styling */
.data-type-info {
  display: flex;
  align-items: center;
  gap: 12px;
}

.data-type-icon {
  font-size: 1.5rem;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #8B1538 0%, #A6195C 100%);
  color: white;
  border-radius: 10px;
  box-shadow: 0 3px 8px rgba(139, 21, 56, 0.3);
  animation: gentle-pulse 3s ease-in-out infinite;
}

@keyframes gentle-pulse {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.05); }
}

.data-type-info strong {
  color: #8B1538;
  font-weight: 700;
  font-size: 1.1rem;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

/* Records count styling */
.records-cell {
  text-align: center;
}

.records-count {
  display: inline-block;
  background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
  color: white;
  padding: 6px 12px;
  border-radius: 20px;
  font-weight: 600;
  font-size: 0.9rem;
  min-width: 60px;
  box-shadow: 0 2px 6px rgba(40, 167, 69, 0.3);
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.3);
}

/* Actions cell styling */
.actions-cell {
  text-align: center;
}

.btn-action {
  padding: 8px 12px;
  margin: 2px 4px;
  border: none;
  border-radius: 8px;
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  text-decoration: none;
  display: inline-block;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.15);
}

.btn-view {
  background: linear-gradient(135deg, #007bff 0%, #0056b3 100%);
  color: white;
}

.btn-raw-view {
  background: linear-gradient(135deg, #6f42c1 0%, #5a2d91 100%);
  color: white;
}

.btn-import {
  background: linear-gradient(135deg, #8B1538 0%, #A6195C 100%);
  color: white;
}

.btn-delete {
  background: linear-gradient(135deg, #dc3545 0%, #c82333 100%);
  color: white;
}

.btn-action:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.25);
  filter: brightness(1.1);
}

.btn-action:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none;
}

/* File formats styling */
.file-formats {
  color: #6c757d;
  font-size: 0.9rem;
  font-family: 'Courier New', monospace;
  background: #f8f9fa;
  padding: 4px 8px;
  border-radius: 6px;
  border-left: 3px solid #8B1538;
}

/* Description cell */
.description-cell {
  color: #495057;
  font-style: italic;
  max-width: 200px;
}

/* Last update cell */
.last-update-cell {
  color: #6c757d;
  font-size: 0.9rem;
  font-family: 'Courier New', monospace;
}

/* Control Panel styling với thương hiệu Agribank */
.control-panel {
  background: linear-gradient(135deg, #f8f9fa 0%, #fff 100%);
  padding: 25px;
  border-radius: 15px;
  margin-bottom: 25px;
  border: 2px solid #8B1538;
  box-shadow: 0 6px 20px rgba(139, 21, 56, 0.15);
  position: relative;
  overflow: hidden;
}

.control-panel::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, #8B1538 0%, #C41E3A 50%, #8B1538 100%);
}

.date-control-section,
.bulk-actions-section {
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(10px);
  padding: 20px;
  border-radius: 12px;
  margin-bottom: 20px;
  border: 1px solid rgba(139, 21, 56, 0.2);
  box-shadow: 0 3px 10px rgba(139, 21, 56, 0.1);
}

.date-control-section:last-child,
.bulk-actions-section:last-child {
  margin-bottom: 0;
}

.date-control-section h3,
.bulk-actions-section h3 {
  color: #8B1538;
  margin-bottom: 15px;
  font-weight: 700;
  font-size: 1.3rem;
  font-family: 'Playfair Display', 'Georgia', serif;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

/* Date controls */
.date-controls {
  display: flex;
  flex-wrap: wrap;
  gap: 15px;
  align-items: center;
}

.date-range {
  display: flex;
  gap: 10px;
  align-items: center;
}

.date-range label {
  color: #8B1538;
  font-weight: 600;
  font-size: 0.95rem;
}

.date-input {
  padding: 8px 12px;
  border: 2px solid #e9ecef;
  border-radius: 8px;
  font-size: 0.9rem;
  transition: all 0.3s ease;
}

.date-input:focus {
  border-color: #8B1538;
  box-shadow: 0 0 0 3px rgba(139, 21, 56, 0.1);
  outline: none;
}

/* Bulk actions */
.bulk-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.btn-filter,
.btn-clear,
.btn-clear-all,
.btn-refresh,
.btn-debug {
  padding: 10px 16px;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  gap: 6px;
  box-shadow: 0 3px 8px rgba(0, 0, 0, 0.15);
}

.btn-filter {
  background: linear-gradient(135deg, #8B1538 0%, #C41E3A 100%);
  color: white;
}

.btn-clear {
  background: linear-gradient(135deg, #6c757d 0%, #495057 100%);
  color: white;
}

.btn-clear-all {
  background: linear-gradient(135deg, #dc3545 0%, #c82333 100%);
  color: white;
}

.btn-refresh {
  background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
  color: white;
}

.btn-debug {
  background: linear-gradient(135deg, #ffc107 0%, #e0a800 100%);
  color: #212529;
}

.btn-filter:hover,
.btn-clear:hover,
.btn-clear-all:hover,
.btn-refresh:hover,
.btn-debug:hover {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.25);
  filter: brightness(1.1);
}

.btn-filter:disabled,
.btn-clear:disabled,
.btn-clear-all:disabled,
.btn-refresh:disabled,
.btn-debug:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none;
}

.bulk-actions-section h3 {
  color: #8B1538;
  margin-bottom: 15px;
  font-weight: 600;
}

/* Responsive improvements */
@media (max-width: 768px) {
  .controls-section {
    padding: 20px;
    border-radius: 10px 10px 0 0;
  }
  
  .controls-section h1 {
    font-size: 2rem;
  }
  
  .data-types-table {
    font-size: 12px;
  }
  
  .data-types-table thead th,
  .data-types-table tbody td {
    padding: 10px 8px;
  }
  
  .btn-action {
    padding: 6px 8px;
    font-size: 11px;
    margin: 1px 2px;
  }
  
  .data-type-icon {
    width: 30px;
    height: 30px;
    font-size: 1.2rem;
  }
}
</style>
