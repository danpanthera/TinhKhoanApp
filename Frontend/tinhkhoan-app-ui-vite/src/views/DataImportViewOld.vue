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

    <!-- Statistics Dashboard -->
    <div v-if="importStats" class="stats-dashboard">
      <div class="stats-grid">
        <div class="stat-card">
          <div class="stat-icon">📊</div>
          <div class="stat-info">
            <h3>{{ importStats.totalImports }}</h3>
            <p>Tổng Import</p>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon">📋</div>
          <div class="stat-info">
            <h3>{{ formatNumber(importStats.totalRecords) }}</h3>
            <p>Tổng Records</p>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon">✅</div>
          <div class="stat-info">
            <h3>{{ importStats.successfulImports }}</h3>
            <p>Thành công</p>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon">❌</div>
          <div class="stat-info">
            <h3>{{ importStats.failedImports }}</h3>
            <p>Thất bại</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Data Type Import Buttons -->
    <div class="import-section">
      <div class="section-header">
        <h2>📤 Import Dữ liệu theo Loại</h2>
        <p>Chọn loại dữ liệu phù hợp để import. Hỗ trợ file nén có mật khẩu (ZIP, 7Z, RAR)</p>
      </div>

      <div class="data-type-grid">
        <div 
          v-for="(dataType, key) in dataTypeDefinitions" 
          :key="key"
          class="data-type-card"
          :class="{ 'active': selectedDataType === key }"
          @click="selectDataType(key)"
        >
          <div class="card-header">
            <span class="card-icon" :style="{ color: getDataTypeColor(key) }">
              {{ dataType.icon }}
            </span>
            <h3>{{ dataType.name }}</h3>
          </div>
          <p class="card-description">{{ dataType.description }}</p>
          <div class="card-footer">
            <span class="file-formats">{{ dataType.acceptedFormats.join(', ') }}</span>
            <div class="card-actions">
              <button 
                @click.stop="viewDataType(key)" 
                class="btn-view"
                title="Xem dữ liệu đã import"
              >
                👁️ Xem
              </button>
              <button 
                @click.stop="openImportModal(key)" 
                class="btn-import"
                :style="{ backgroundColor: getDataTypeColor(key) }"
              >
                📤 Import
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Import History Section -->
    <div class="history-section">
      <div class="section-header">
        <h2>📋 Lịch sử Import</h2>
        <div class="header-actions">
          <input 
            v-model="searchQuery" 
            type="text" 
            placeholder="🔍 Tìm kiếm theo tên file, loại dữ liệu..."
            class="search-input"
          />
          <div class="date-filters">
            <input 
              v-model="filterStatementDate" 
              type="date" 
              placeholder="Ngày sao kê"
              class="date-input"
              title="Lọc theo ngày sao kê"
            />
            <select v-model="filterFileType" class="file-type-select">
              <option value="">Tất cả loại file</option>
              <option value="LN01">LN01 - Dữ liệu làm ngoài giờ</option>
              <option value="GL01">GL01 - Dữ liệu tổng quát</option>
              <option value="DP01">DP01 - Dữ liệu tiền gửi</option>
            </select>
          </div>
          <button @click="refreshImports" class="btn-refresh" :disabled="loading">
            🔄 Refresh
          </button>
        </div>
      </div>

      <div v-if="filteredImports.length === 0 && !loading" class="empty-state">
        <div class="empty-icon">📭</div>
        <h3>Chưa có dữ liệu import nào</h3>
        <p>Bắt đầu import dữ liệu bằng cách chọn một trong các loại dữ liệu ở trên</p>
      </div>

      <div v-else class="imports-table">
        <table>
          <thead>
            <tr>
              <th>Loại dữ liệu</th>
              <th>Tên file</th>
              <th>Ngày sao kê</th>
              <th>Ngày import</th>
              <th>Records</th>
              <th>Trạng thái</th>
              <th>Người import</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="imp in paginatedImports" :key="imp.id">
              <td>
                <span 
                  class="data-type-badge" 
                  :style="{ backgroundColor: getDataTypeColor(imp.dataType) }"
                >
                  {{ imp.dataType }}
                </span>
              </td>
              <td class="filename-cell">
                <span class="filename">{{ imp.fileName }}</span>
                <span v-if="imp.isArchiveFile" class="archive-badge">🗂️ Archive</span>
              </td>
              <td>{{ formatDate(imp.statementDate) }}</td>
              <td>{{ formatDate(imp.importDate) }}</td>
              <td class="records-cell">
                <span class="records-count">{{ formatNumber(imp.recordsCount) }}</span>
              </td>
              <td>
                <span 
                  class="status-badge" 
                  :class="getStatusClass(imp.status)"
                >
                  {{ getStatusText(imp.status) }}
                </span>
              </td>
              <td>{{ imp.importedBy }}</td>
              <td class="actions-cell">
                <button 
                  @click="previewImport(imp.id)" 
                  class="btn-action btn-preview"
                  title="Xem trước dữ liệu"
                >
                  👁️
                </button>
                <button 
                  v-if="imp.compressedData"
                  @click="viewCompressedData(imp.id)" 
                  class="btn-action btn-compress"
                  :title="`Xem dữ liệu nén (tỷ lệ: ${(imp.compressionRatio * 100).toFixed(1)}%)`"
                >
                  📦
                </button>
                <button 
                  @click="deleteImport(imp.id)" 
                  class="btn-action btn-delete"
                  title="Xóa import"
                >
                  🗑️
                </button>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- Pagination -->
        <div v-if="totalPages > 1" class="pagination">
          <button 
            @click="currentPage = 1" 
            :disabled="currentPage === 1"
            class="pagination-btn"
          >
            ⏮️
          </button>
          <button 
            @click="currentPage--" 
            :disabled="currentPage === 1"
            class="pagination-btn"
          >
            ⏪
          </button>
          <span class="pagination-info">
            Trang {{ currentPage }} / {{ totalPages }}
          </span>
          <button 
            @click="currentPage++" 
            :disabled="currentPage === totalPages"
            class="pagination-btn"
          >
            ⏩
          </button>
          <button 
            @click="currentPage = totalPages" 
            :disabled="currentPage === totalPages"
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
              <label>📁 Chọn File</label>
              <div 
                class="upload-area" 
                @drop="handleFileDrop" 
                @dragover.prevent 
                @dragenter.prevent
              >
                <div class="upload-content">
                  <div class="upload-icon">📄</div>
                  <p>Kéo thả file vào đây hoặc click để chọn</p>
                  <p class="upload-hint">
                    Hỗ trợ: {{ dataTypeDefinitions[selectedDataType]?.acceptedFormats.join(', ') }}, 
                    Archive (ZIP, 7Z, RAR)
                  </p>
                  <input 
                    ref="fileInput" 
                    type="file" 
                    @change="handleFileSelect" 
                    :accept="getAcceptTypes()"
                    multiple
                    hidden
                  />
                  <button @click="$refs.fileInput.click()" class="btn-choose-file">
                    Chọn File
                  </button>
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
            </div>

            <!-- Archive Password -->
            <div v-if="hasArchiveFile" class="form-group">
              <label>🔐 Mật khẩu file nén (nếu có)</label>
              <input 
                v-model="archivePassword" 
                type="password" 
                placeholder="Nhập mật khẩu file nén..."
                class="form-input"
              />
              <small class="form-hint">Để trống nếu file không có mật khẩu</small>
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
            <div class="empty-icon">📭</div>
            <p>Không có dữ liệu để hiển thị</p>
          </div>
        </div>

        <div class="modal-footer">
          <button @click="closePreviewModal" class="btn-cancel">Đóng</button>
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
const uploading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')
const loadingMessage = ref('')

// Import management
const imports = ref([])
const selectedDataType = ref(null)
const showImportModal = ref(false)
const searchQuery = ref('')
const filterStatementDate = ref('')
const filterFileType = ref('')

// Modal state
const showPreviewModal = ref(false)
const selectedImport = ref(null)
const previewData = ref([])

// File handling
const selectedFiles = ref([])
const archivePassword = ref('')
const importNotes = ref('')

// Data type definitions
const dataTypeDefinitions = rawDataService.getDataTypeDefinitions()

// Import statistics
const importStats = ref(null)

// Computed properties
const filteredImports = computed(() => {
  let filtered = imports.value
  
  // Filter by search query
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(imp => 
      imp.fileName.toLowerCase().includes(query) ||
      imp.dataType.toLowerCase().includes(query) ||
      imp.notes?.toLowerCase().includes(query)
    )
  }
  
  // Filter by statement date
  if (filterStatementDate.value) {
    const filterDate = new Date(filterStatementDate.value).toISOString().split('T')[0]
    filtered = filtered.filter(imp => {
      if (!imp.statementDate) return false
      const impDate = new Date(imp.statementDate).toISOString().split('T')[0]
      return impDate === filterDate
    })
  }
  
  // Filter by file type
  if (filterFileType.value) {
    filtered = filtered.filter(imp => 
      imp.fileName.includes(filterFileType.value) ||
      imp.dataType.includes(filterFileType.value)
    )
  }
  
  return filtered
})

const paginatedImports = computed(() => {
  return filteredImports.value.slice(0, 20) // Show first 20 results
})

// Methods
const selectDataType = (dataType) => {
  selectedDataType.value = dataType
}

const viewDataType = (dataType) => {
  // Filter imports by data type
  const dataTypeImports = imports.value.filter(imp => imp.dataType === dataType)
  
  if (dataTypeImports.length === 0) {
    showError(`Chưa có dữ liệu import nào cho loại ${dataTypeDefinitions[dataType]?.name}`)
    return
  }
  
  // Set search query to filter by dataType
  searchQuery.value = dataType
  
  // Scroll to history section
  const historySection = document.querySelector('.history-section')
  if (historySection) {
    historySection.scrollIntoView({ 
      behavior: 'smooth',
      block: 'start'
    })
  }
  
  // Show success message
  showSuccess(`Hiển thị ${dataTypeImports.length} import(s) cho loại ${dataTypeDefinitions[dataType]?.name}`)
}

const openImportModal = (dataType) => {
  selectedDataType.value = dataType
  selectedFiles.value = []
  archivePassword.value = ''
  importNotes.value = ''
  showImportModal.value = true
}

const closeImportModal = () => {
  showImportModal.value = false
  selectedDataType.value = null
  selectedFiles.value = []
  archivePassword.value = ''
  importNotes.value = ''
}

const getDataTypeColor = (dataType) => {
  return rawDataService.getDataTypeColor(dataType)
}

const formatNumber = (num) => {
  return rawDataService.formatRecordCount(num)
}

const formatDate = (dateString) => {
  return rawDataService.formatDate(dateString)
}

const isArchiveFile = (fileName) => {
  return rawDataService.isArchiveFile(fileName)
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

const getAcceptTypes = () => {
  if (!selectedDataType.value) return ''
  const dataType = dataTypeDefinitions[selectedDataType.value]
  return [...dataType.acceptedFormats, '.zip', '.7z', '.rar'].join(',')
}

const hasArchiveFile = computed(() => {
  return selectedFiles.value.some(file => isArchiveFile(file.name))
})

// File handling methods
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

// Import methods
const performImport = async () => {
  if (selectedFiles.value.length === 0) {
    showError('Vui lòng chọn ít nhất một file')
    return
  }
  
  try {
    uploading.value = true
    loadingMessage.value = 'Đang upload và xử lý dữ liệu...'
    
    // Use new API with automatic statement date extraction and compression
    const category = selectedDataType.value || 'General'
    const result = await rawDataService.importDataNew(selectedFiles.value, category)
    
    if (result.success) {
      const successMessage = result.data.message || 'Import thành công!'
      showSuccess(`✅ ${successMessage}`)
      closeImportModal()
      await refreshImports()
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

// Data management methods
const refreshImports = async () => {
  try {
    loading.value = true
    loadingMessage.value = 'Đang tải danh sách import...'
    
    // Use new API with statement date and compression support
    const result = await rawDataService.getAllImportsNew()
    if (result.success) {
      imports.value = result.data || []
      // Calculate stats from the data
      importStats.value = {
        totalImports: imports.value.length,
        successfulImports: imports.value.filter(imp => imp.status === 'Completed').length,
        failedImports: imports.value.filter(imp => imp.status === 'Failed').length,
        totalRecords: imports.value.reduce((sum, imp) => sum + (imp.recordsCount || 0), 0),
        successRate: imports.value.length > 0 ? (imports.value.filter(imp => imp.status === 'Completed').length / imports.value.length * 100).toFixed(1) : 0
      }
    } else {
      showError(result.error || 'Không thể tải danh sách import')
    }
    
  } catch (error) {
    console.error('Error loading imports:', error)
    showError('Có lỗi xảy ra khi tải dữ liệu')
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

const previewImport = async (importItem) => {
  try {
    loading.value = true
    loadingMessage.value = 'Đang tải preview...'
    
    const result = await rawDataService.previewData(importItem.id)
    if (result.success) {
      selectedImport.value = importItem
      previewData.value = result.data.records || []
      showPreviewModal.value = true
    } else {
      showError(result.error || 'Không thể tải preview')
    }
    
  } catch (error) {
    console.error('Error loading preview:', error)
    showError('Có lỗi xảy ra khi tải preview')
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

const deleteImport = async (importItem) => {
  if (!confirm(`Bạn có chắc chắn muốn xóa import "${importItem.fileName}"?`)) {
    return
  }
  
  try {
    loading.value = true
    loadingMessage.value = 'Đang xóa dữ liệu...'
    
    const result = await rawDataService.deleteImport(importItem.id)
    if (result.success) {
      showSuccess('Đã xóa import thành công')
      await refreshImports()
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

const closePreviewModal = () => {
  showPreviewModal.value = false
  selectedImport.value = null
  previewData.value = []
}

// Method để xem dữ liệu nén
const viewCompressedData = async (importId) => {
  try {
    loading.value = true
    loadingMessage.value = 'Đang giải nén dữ liệu...'
    
    const result = await rawDataService.getDecompressedData(importId)
    if (result.success) {
      console.log('Decompressed data:', result.data)
      showSuccess(`✅ Giải nén thành công! Tỷ lệ nén: ${(result.data.compressionRatio * 100).toFixed(1)}%`)
      
      // You can show decompressed data in a modal or navigate to a new view
      // For now, just log it to console
    } else {
      showError(result.error || 'Không thể giải nén dữ liệu')
    }
    
  } catch (error) {
    console.error('Error decompressing data:', error)
    showError('Có lỗi xảy ra khi giải nén dữ liệu')
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

// Method để lọc theo ngày sao kê
const filterByStatementDate = async (date, fileType = null) => {
  try {
    loading.value = true
    loadingMessage.value = 'Đang tìm kiếm dữ liệu...'
    
    const result = await rawDataService.getImportsByStatementDate(date, fileType)
    if (result.success) {
      imports.value = result.data || []
      showSuccess(`✅ Tìm thấy ${imports.value.length} file(s) cho ngày ${date}`)
    } else {
      showError(result.error || 'Không tìm thấy dữ liệu')
    }
    
  } catch (error) {
    console.error('Error filtering by statement date:', error)
    showError('Có lỗi xảy ra khi tìm kiếm dữ liệu')
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

// Utility methods
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

const showSuccess = (message) => {
  successMessage.value = message
  setTimeout(() => {
    successMessage.value = ''
  }, 3000)
}

// Lifecycle
onMounted(() => {
  refreshImports()
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

/* Data Type Grid */
.data-type-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 20px;
  margin-top: 24px;
}

.data-type-card {
  background: white;
  border-radius: 12px;
  padding: 24px;
  border: 2px solid #e9ecef;
  transition: all 0.3s ease;
  cursor: pointer;
  position: relative;
  overflow: hidden;
}

.data-type-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.1);
  border-color: #8B1538;
}

.data-type-card.active {
  border-color: #8B1538;
  background: linear-gradient(135deg, #fff 0%, #f8f9fa 100%);
}

.card-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 16px;
}

.card-icon {
  font-size: 2rem;
}

.card-header h3 {
  color: #2c3e50;
  font-size: 1.2rem;
  font-weight: 600;
  margin: 0;
}

.card-description {
  color: #6c757d;
  margin-bottom: 20px;
  line-height: 1.5;
}

.card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 8px;
}

.file-formats {
  font-size: 0.8rem;
  color: #6c757d;
  font-style: italic;
  flex: 1;
}

.card-actions {
  display: flex;
  gap: 8px;
  align-items: center;
}

.btn-view {
  background: linear-gradient(135deg, #17a2b8 0%, #138496 100%);
  color: white;
  border: none;
  padding: 8px 14px;
  border-radius: 6px;
  font-size: 0.85rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 4px rgba(23, 162, 184, 0.3);
}

.btn-view:hover {
  background: linear-gradient(135deg, #138496 0%, #117a8b 100%);
  transform: translateY(-1px);
  box-shadow: 0 3px 8px rgba(23, 162, 184, 0.4);
}

.btn-import {
  background: #28a745;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 0.9rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-import:hover {
  background: #218838;
  transform: translateY(-1px);
}

.btn-compress {
  background: linear-gradient(135deg, #28a745, #20c997);
  color: white;
}

.btn-compress:hover {
  background: linear-gradient(135deg, #218838, #1db584);
}

/* Import History */
.history-section {
  background: white;
  border-radius: 16px;
  padding: 32px;
  margin-top: 32px;
  box-shadow: 0 12px 48px rgba(0, 0, 0, 0.08);
  border: 1px solid rgba(139, 21, 56, 0.1);
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-wrap: wrap;
}

.date-filters {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.date-input,
.file-type-select {
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 0.9rem;
  background: white;
}

.date-input {
  min-width: 150px;
}

.file-type-select {
  min-width: 200px;
}

.date-input:focus,
.file-type-select:focus {
  outline: none;
  border-color: #007bff;
  box-shadow: 0 0 0 2px rgba(0, 123, 255, 0.25);
}

.search-input {
  flex: 1;
  padding: 10px 16px;
  border: 2px solid #e9ecef;
  border-radius: 8px;
  font-size: 14px;
}

.search-input:focus {
  outline: none;
  border-color: #8B1538;
}

.btn-refresh {
  background: #6c757d;
  color: white;
  border: none;
  padding: 10px 16px;
  border-radius: 8px;
  font-weight: 500;
  cursor: pointer;
  transition: background 0.3s ease;
}

.btn-refresh:hover:not(:disabled) {
  background: #5a6268;
}

.btn-refresh:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Import Table */
.imports-table {
  margin-top: 24px;
  border-radius: 8px;
  overflow: hidden;
  border: 1px solid #e9ecef;
}

.imports-table table {
  width: 100%;
  border-collapse: collapse;
}

.imports-table th {
  background: linear-gradient(135deg, #8B1538 0%, #A91B47 100%);
  color: white;
  padding: 16px 12px;
  text-align: left;
  font-weight: 600;
  font-size: 0.9rem;
}

.imports-table td {
  padding: 14px 12px;
  border-bottom: 1px solid #f8f9fa;
  vertical-align: middle;
}

.imports-table tr:hover {
  background: #f8f9fa;
}

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

.archive-badge {
  background: #17a2b8;
  color: white;
  padding: 2px 6px;
  border-radius: 8px;
  font-size: 0.7rem;
  margin-top: 4px;
  display: inline-block;
}

.records-count {
  font-weight: 600;
  text-align: center;
}

.actions-cell {
  display: flex;
  gap: 8px;
}

.btn-action {
  width: 32px;
  height: 32px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
  font-size: 14px;
}

.btn-preview { background: #17a2b8; color: white; }
.btn-delete { background: #dc3545; color: white; }

.btn-action:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

/* Empty State */
.empty-state {
  text-align: center;
  padding: 60px 20px;
  color: #6c757d;
}

.empty-icon {
  font-size: 4rem;
  margin-bottom: 16px;
  opacity: 0.6;
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

.btn-choose-file {
  background: #8B1538;
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.3s ease;
}

.btn-choose-file:hover {
  background: #A91B47;
}

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
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
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
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
}

.file-item:hover {
  transform: translateY(-1px);
  box-shadow: 0 3px 8px rgba(0, 0, 0, 0.1);
  border-color: #8B1538;
}

.file-item:last-child {
  margin-bottom: 0;
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
  line-height: 1.4;
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

.btn-remove-file:active {
  transform: scale(0.95);
}

.form-input, .form-textarea {
  width: 100%;
  padding: 12px 16px;
  border: 2px solid #e9ecef;
  border-radius: 8px;
  font-size: 14px;
  transition: border-color 0.3s ease;
}

.form-input:focus, .form-textarea:focus {
  outline: none;
  border-color: #8B1538;
}

.form-hint {
  font-size: 0.8rem;
  color: #6c757d;
  font-style: italic;
}

.btn-cancel {
  background: #6c757d;
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 8px;
  font-weight: 500;
  cursor: pointer;
  transition: background 0.3s ease;
}

.btn-cancel:hover {
  background: #5a6268;
}

.btn-import-confirm {
  background: #28a745;
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-import-confirm:hover:not(:disabled) {
  background: #218838;
  transform: translateY(-1px);
}

.btn-import-confirm:disabled {
  background: #6c757d;
  cursor: not-allowed;
  transform: none;
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

.no-data, .no-preview-data {
  text-align: center;
  padding: 40px 20px;
  color: #6c757d;
}

/* Alert styles reuse from existing code */
.alert {
  padding: 16px 20px;
  border-radius: 8px;
  margin-bottom: 24px;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 10px;
  animation: slideIn 0.3s ease-out;
}

.alert-error {
  background: linear-gradient(135deg, #f8d7da 0%, #f5c6cb 100%);
  color: #721c24;
  border: 1px solid #f5c6cb;
}

.alert-success {
  background: linear-gradient(135deg, #d4edda 0%, #c3e6cb 100%);
  color: #155724;
  border: 1px solid #c3e6cb;
}

.alert-close {
  background: none;
  border: none;
  color: inherit;
  font-size: 1.2rem;
  cursor: pointer;
  margin-left: auto;
}

.alert-icon {
  font-size: 1.2rem;
}

/* Loading styles */
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

/* Stats Dashboard */
.stats-dashboard {
  margin-bottom: 32px;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
}

.stat-card {
  background: white;
  border-radius: 12px;
  padding: 24px;
  text-align: center;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  border: 1px solid rgba(139, 21, 56, 0.1);
}

.stat-icon {
  font-size: 2rem;
  margin-bottom: 12px;
}

.stat-info h3 {
  font-size: 2rem;
  font-weight: 700;
  color: #8B1538;
  margin: 0 0 8px 0;
}

.stat-info p {
  color: #6c757d;
  margin: 0;
  font-weight: 500;
}

/* Animations */
@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(15px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

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

/* File item animation */
.file-item {
  animation: fadeInUp 0.3s ease-out;
}

/* Responsive */
@media (max-width: 768px) {
  .raw-data-warehouse {
    padding: 16px;
  }
  
  .data-type-grid {
    grid-template-columns: 1fr;
  }
  
  .modal-large {
    width: 95vw;
  }
  
  .info-grid {
    grid-template-columns: 1fr;
  }
  
  .header-actions {
    flex-direction: column;
    align-items: stretch;
  }
}
</style>
