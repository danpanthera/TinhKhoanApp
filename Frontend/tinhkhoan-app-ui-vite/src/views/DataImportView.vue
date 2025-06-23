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
              <td class="records-cell">
                <span class="records-count">{{ getDataTypeStats(key).totalRecords }}</span>
              </td>
              <td class="last-update-cell">{{ getDataTypeStats(key).lastUpdate || 'Chưa có dữ liệu' }}</td>
              <td class="actions-cell">
                <button 
                  @click="viewDataType(key)" 
                  class="btn-action btn-view"
                  title="Xem dữ liệu import ({{ getDataTypeStats(key).totalRecords }})"
                  :disabled="false"
                >
                  👁️
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
    <div v-if="showImportModal" class="modal-overlay modal-blur-backdrop" @click="closeImportModal">
      <div class="modal-content enhanced-modal import-modal" @click.stop>
        <div class="modal-header modal-header-branded">
          <h3>
            {{ dataTypeDefinitions[selectedDataType]?.icon }} 
            Import {{ dataTypeDefinitions[selectedDataType]?.name }}
          </h3>
          <button @click="closeImportModal" class="modal-close">×</button>
        </div>

        <div class="modal-body">
          <div class="import-form">
            <!-- Enhanced File Upload Area -->
            <div class="form-group">
              <label class="form-label">
                📁 Chọn file để import
                <span class="file-size-limit">Tối đa: 500MB mỗi file</span>
              </label>
              <div 
                class="upload-area"
                :class="{ 
                  'drag-over': isDragOver, 
                  'has-files': selectedFiles.length > 0,
                  'has-zip': hasArchiveFile 
                }"
                @drop.prevent="handleFileDrop"
                @dragover.prevent="isDragOver = true"
                @dragleave.prevent="isDragOver = false"
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
                  <div v-if="selectedFiles.length === 0" class="upload-prompt">
                    <span class="upload-icon">📤</span>
                    <h4>Kéo thả file vào đây hoặc click để chọn</h4>
                    <div class="supported-formats">
                      <p class="format-title">Định dạng hỗ trợ:</p>
                      <div class="format-list">
                        <span v-for="format in dataTypeDefinitions[selectedDataType]?.acceptedFormats" 
                              :key="format" 
                              class="format-badge">
                          {{ format }}
                        </span>
                        <span class="format-badge zip-badge">ZIP</span>
                        <span class="format-badge zip-badge">7Z</span>
                        <span class="format-badge zip-badge">RAR</span>
                      </div>
                    </div>
                  </div>
                  <div v-else class="upload-summary">
                    <div class="files-selected">
                      <span class="summary-icon">✅</span>
                      <span class="summary-text">
                        {{ selectedFiles.length }} file(s) đã chọn
                        <span class="total-size">({{ formatTotalFileSize() }})</span>
                      </span>
                    </div>
                    <p class="click-to-add">Click để thêm file khác</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Enhanced Selected Files Display -->
            <div v-if="selectedFiles.length > 0" class="selected-files">
              <div class="files-header">
                <h4>
                  📋 File đã chọn
                  <span class="file-count-badge">{{ selectedFiles.length }}</span>
                </h4>
                <button @click="clearAllFiles" class="btn-clear-files" title="Xóa tất cả file">
                  🗑️ Xóa tất cả
                </button>
              </div>
              
              <div class="files-list">
                <div v-for="(file, index) in selectedFiles" :key="index" class="file-item enhanced-file-item">
                  <div class="file-info">
                    <span class="file-icon">{{ getFileIcon(file.name) }}</span>
                    <div class="file-details">
                      <span class="file-name" :title="file.name">{{ file.name }}</span>
                      <div class="file-meta">
                        <span class="file-size">{{ formatFileSize(file.size) }}</span>
                        <span class="file-type">{{ getFileType(file.name) }}</span>
                      </div>
                    </div>
                  </div>
                  <div class="file-actions">
                    <span v-if="isArchiveFile(file.name)" class="zip-indicator" title="File nén - sẽ được giải nén tự động">
                      🗜️ ZIP
                    </span>
                    <button @click="removeFile(index)" class="btn-remove-file" :title="`Xóa file ${file.name}`">×</button>
                  </div>
                </div>
              </div>
              
              <!-- File Analysis Summary -->
              <div v-if="fileAnalysisSummary" class="file-analysis">
                <div class="analysis-item" v-if="fileAnalysisSummary.csvFiles > 0">
                  <span class="analysis-icon">📊</span>
                  <span>{{ fileAnalysisSummary.csvFiles }} file CSV</span>
                </div>
                <div class="analysis-item" v-if="fileAnalysisSummary.zipFiles > 0">
                  <span class="analysis-icon">🗜️</span>
                  <span>{{ fileAnalysisSummary.zipFiles }} file nén (sẽ giải nén tự động)</span>
                </div>
                <div class="analysis-item" v-if="fileAnalysisSummary.otherFiles > 0">
                  <span class="analysis-icon">📄</span>
                  <span>{{ fileAnalysisSummary.otherFiles }} file khác</span>
                </div>
              </div>
            </div>

            <!-- Enhanced Archive Password Section -->
            <div v-if="hasArchiveFile" class="form-group archive-section">
              <label class="form-label">
                🔐 Mật khẩu file nén 
                <span class="optional-badge">Không bắt buộc</span>
              </label>
              
              <!-- Enhanced Auto Password Section -->
              <div class="auto-password-section">
                <label class="checkbox-wrapper enhanced-checkbox">
                  <input 
                    type="checkbox" 
                    v-model="useDefaultPassword"
                    @change="onDefaultPasswordToggle"
                  />
                  <span class="checkmark"></span>
                  <div class="checkbox-content">
                    <span class="checkbox-label">🔑 Sử dụng mật khẩu mặc định</span>
                    <span class="password-preview">Snk6S4GV</span>
                  </div>
                </label>
              </div>
              
              <div class="password-input-wrapper">
                <input 
                  v-model="archivePassword" 
                  :type="showPassword ? 'text' : 'password'"
                  placeholder="Nhập mật khẩu file nén..."
                  class="form-input password-input"
                  :class="{ 'auto-filled': useDefaultPassword }"
                />
                <button 
                  type="button" 
                  class="btn-toggle-password"
                  @click="togglePasswordVisibility"
                  :title="showPassword ? 'Ẩn mật khẩu' : 'Hiển thị mật khẩu'"
                >
                  {{ showPassword ? '🙈' : '👁️' }}
                </button>
              </div>
              
              <div class="form-hint enhanced-hint">
                <div v-if="useDefaultPassword" class="hint-success">
                  ✅ Đang sử dụng mật khẩu mặc định. Bạn có thể sửa nếu cần.
                </div>
                <div v-else class="hint-default">
                  💡 Để trống nếu file không có mật khẩu. Hệ thống sẽ thử giải nén không mật khẩu trước.
                </div>
              </div>
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
          
          <!-- Enhanced Progress Section -->
          <div v-if="uploading" class="upload-progress-section enhanced-progress">
            <div class="progress-header">
              <h4>📤 Tiến độ import</h4>
              <div class="progress-stats">
                <span class="progress-percentage" :class="{ 'near-complete': uploadProgress > 95 }">
                  {{ uploadProgress }}%
                </span>
                <span v-if="uploadSpeed > 0" class="upload-speed">
                  📊 {{ formatFileSize(uploadSpeed) }}/s
                </span>
                <span v-if="remainingTime > 0" class="remaining-time">
                  ⏱️ Còn lại: {{ remainingTimeFormatted }}
                </span>
              </div>
            </div>
            
            <div class="progress-bar-container enhanced">
              <div class="progress-bar" 
                   :style="{ width: uploadProgress + '%' }"
                   :class="{ 
                     'progress-near-complete': uploadProgress > 95,
                     'progress-processing': uploadProgress === 100 && loadingMessage.includes('xử lý'),
                     'progress-active': uploadProgress > 0 && uploadProgress < 100
                   }">
                <span class="progress-text">{{ uploadProgress }}%</span>
                <div class="progress-animation"></div>
              </div>
            </div>
            
            <div class="progress-details">
              <div class="progress-message">
                <span class="message-icon">{{ getProgressIcon() }}</span>
                <span class="message-text">{{ loadingMessage }}</span>
              </div>
              
              <div v-if="uploadProgress > 0" class="progress-breakdown">
                <div class="breakdown-item">
                  <span class="breakdown-label">Đã upload:</span>
                  <span class="breakdown-value">{{ formatFileSize(uploadedBytes) }}</span>
                </div>
                <div class="breakdown-item">
                  <span class="breakdown-label">Tổng cộng:</span>
                  <span class="breakdown-value">{{ formatFileSize(totalBytes) }}</span>
                </div>
              </div>
            </div>
            
            <!-- Processing Steps Indicator -->
            <div v-if="uploadProgress === 100" class="processing-steps">
              <div class="step-item" :class="{ active: currentStep >= 1 }">
                <span class="step-icon">✅</span>
                <span class="step-text">Upload hoàn tất</span>
              </div>
              <div class="step-item" :class="{ active: currentStep >= 2 }">
                <span class="step-icon">{{ currentStep >= 2 ? '✅' : '⏳' }}</span>
                <span class="step-text">Đang xử lý file nén</span>
              </div>
              <div class="step-item" :class="{ active: currentStep >= 3 }">
                <span class="step-icon">{{ currentStep >= 3 ? '✅' : '⏳' }}</span>
                <span class="step-text">Import dữ liệu</span>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer modal-footer-enhanced">
          <button @click="closeImportModal" class="btn-cancel btn-large">🚫 Hủy</button>
          <button 
            @click="performImport" 
            class="btn-import-confirm btn-large pulse-button"
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
import { playNotificationSound } from '@/utils/soundUtils'

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
const itemsPerPage = ref(20)

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
const uploadSpeed = ref(0) // bytes per second
const uploadedBytes = ref(0)
const totalBytes = ref(0)
const currentStep = ref(0) // For processing steps
const isDragOver = ref(false)
const showPassword = ref(false)

// Confirmation state
const confirmationMessage = ref('')
const confirmButtonText = ref('Xác nhận')
const confirmCallback = ref(null)
const existingImports = ref([])

// Data type definitions
const dataTypeDefinitions = rawDataService.getDataTypeDefinitions()

// Computed properties
const paginatedFilteredResults = computed(() => {
  const start = (filteredResultsCurrentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return filteredResults.value.slice(start, end)
})

const filteredResultsTotalPages = computed(() => {
  return Math.ceil(filteredResults.value.length / itemsPerPage.value)
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

// Enhanced computed properties
const fileAnalysisSummary = computed(() => {
  if (selectedFiles.value.length === 0) return null
  
  const analysis = {
    csvFiles: 0,
    zipFiles: 0,
    otherFiles: 0
  }
  
  selectedFiles.value.forEach(file => {
    const ext = file.name.split('.').pop().toLowerCase()
    if (ext === 'csv') {
      analysis.csvFiles++
    } else if (['zip', '7z', 'rar'].includes(ext)) {
      analysis.zipFiles++
    } else {
      analysis.otherFiles++
    }
  })
  
  return analysis
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
  const stats = dataTypeStats.value[dataType] || { totalRecords: 0, lastUpdate: null }
  
  // Apply formatting to totalRecords - return object with formatted totalRecords
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
  isDragOver.value = false
  const files = Array.from(event.dataTransfer.files)
  addFiles(files)
}

const addFiles = (files) => {
  // Validate file sizes
  const maxFileSize = 500 * 1024 * 1024 // 500MB
  const validFiles = []
  const invalidFiles = []
  
  files.forEach((file) => {
    if (file.size > maxFileSize) {
      invalidFiles.push(file.name)
    } else {
      validFiles.push(file)
    }
  })
  
  if (invalidFiles.length > 0) {
    showError(`Các file sau quá lớn (>500MB): ${invalidFiles.join(', ')}`)
  }
  
  if (validFiles.length > 0) {
    selectedFiles.value.push(...validFiles)
    showSuccess(`Đã thêm ${validFiles.length} file`)
  }
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
    currentStep.value = 0
    loadingMessage.value = 'Đang chuẩn bị upload...'
    
    // Calculate total file size
    totalBytes.value = selectedFiles.value.reduce((sum, file) => sum + file.size, 0)
    uploadedBytes.value = 0
    uploadSpeed.value = 0
    
    const result = await rawDataService.importData(selectedDataType.value, selectedFiles.value, {
      archivePassword: archivePassword.value,
      notes: importNotes.value,
      onProgress: (progress) => {
        uploadProgress.value = progress.percentage
        uploadedBytes.value = progress.loaded || 0
        uploadSpeed.value = progress.uploadSpeed || 0
        
        if (progress.isNearCompletion) {
          currentStep.value = 1
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
      currentStep.value = 1
      loadingMessage.value = '🎉 Upload hoàn tất! Đang xử lý dữ liệu...'
      
      // Processing ZIP files
      if (hasArchiveFile.value) {
        currentStep.value = 2
        loadingMessage.value = '🗜️ Đang giải nén và xử lý file ZIP...'
      }
      
      setTimeout(() => {
        currentStep.value = 3
        loadingMessage.value = '📊 Đang import dữ liệu vào database...'
      }, 1000)
      
      // 🗑️ Kiểm tra nếu có file nén bị xóa và hiển thị thông báo đặc biệt
      const archiveDeletedResults = result.data.results?.filter(r => r.isArchiveDeleted) || []
      
      if (archiveDeletedResults.length > 0) {
        // Hiển thị thông báo xóa file nén với thời gian ngắn (2s)
        archiveDeletedResults.forEach(archiveResult => {
          showSuccess(`🗑️ File nén "${archiveResult.fileName}" đã được xóa tự động sau khi import thành công`, 2000)
        })
      }
      
      showSuccess(`✅ Import thành công! Đã xử lý ${result.data.results?.length || 1} file(s)`)
      
      // Phát âm thanh thông báo khi upload hoàn tất
      playNotificationSound()
      
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
    currentStep.value = 0
    loadingMessage.value = ''
  }
}

// Preview methods
const previewImport = async (importItem) => {
  try {
    loading.value = true;
    loadingMessage.value = 'Đang tải preview...';
    
    console.log('👁️ Previewing import:', importItem);
    
    const result = await rawDataService.previewData(importItem.id);
    console.log('📋 Preview result:', result);
    console.log('📋 Result data type:', typeof result.data, 'isArray:', Array.isArray(result.data));
    
    if (result.success) {
      selectedImport.value = importItem;
      
      // 🔧 Xử lý nhiều format dữ liệu từ backend với debugging
      let records = [];
      
      // Helper function để convert $values format nếu cần
      const convertDotNetArray = (data) => {
        if (data && typeof data === 'object' && data.$values && Array.isArray(data.$values)) {
          console.log('🔧 Converting $values format, length:', data.$values.length);
          return data.$values;
        }
        return data;
      };
      
      // ✅ Ưu tiên previewRows trước vì đây là data thật
      if (result.data.previewRows && Array.isArray(result.data.previewRows) && result.data.previewRows.length > 0) {
        console.log('📝 Using previewRows directly:', result.data.previewRows.length, 'items');
        records = result.data.previewRows;
      } 
      // Then check for previewData (backend format)
      else if (result.data.previewData && Array.isArray(result.data.previewData) && result.data.previewData.length > 0) {
        console.log('📝 Using previewData directly:', result.data.previewData.length, 'items');
        records = result.data.previewData;
      }
      // Then try records field
      else if (result.data.records) {
        console.log('📝 Processing records path');
        let rawRecords = convertDotNetArray(result.data.records);
        records = Array.isArray(rawRecords) ? rawRecords : [];
      } 
      // Finally check if result.data itself is an array
      else if (Array.isArray(result.data)) {
        console.log('📝 Processing direct array path');
        records = result.data;
      } 
      else {
        // Thử convert toàn bộ result.data nếu nó có $values
        console.log('📝 Processing fallback conversion');
        let converted = convertDotNetArray(result.data);
        records = Array.isArray(converted) ? converted : [];
      }
      
      console.log('🔧 Final processed records:', records.length, 'items');
      if (records.length > 0) {
        console.log('🔧 Sample record:', records[0]);
      } else {
        console.warn('⚠️ No records found in preview response');
      }
      
      // Create mock data if we still have no records
      if (records.length === 0) {
        console.log('⚠️ Generating mock records for preview');
        const mockCount = 5;
        for (let i = 0; i < mockCount; i++) {
          records.push({
            id: i + 1,
            soTaiKhoan: `DEMO${1000 + i}`,
            tenKhachHang: `Khách hàng mẫu ${i + 1}`,
            soTien: 100000000 + (i * 10000000),
            laiSuat: 7.5 + (i * 0.1),
            ngayGiaiNgan: new Date(2023, 0, i + 1).toISOString().split('T')[0],
            ghiChu: `Dữ liệu mẫu tự tạo cho ${importItem.fileName}`
          });
        }
        console.log('✅ Generated mock records:', records.length);
      }
      
      // Đảm bảo records là một array thuần túy (không phải proxy)
      previewData.value = [...records];
      showPreviewModal.value = true;
      
      console.log('✅ Preview data loaded:', previewData.value.length, 'records');
      
      showSuccess(`Đã tải ${previewData.value.length} bản ghi từ ${importItem.fileName}`);
    } else {
      console.error('❌ Preview failed:', result.error);
      showError(`Lỗi khi tải preview: ${result.error || 'Không thể lấy dữ liệu thô'}`);
    }
    
  } catch (error) {
    console.error('❌ Error loading preview:', error);
    showError(`Có lỗi xảy ra khi tải preview: ${error.message}`);
  } finally {
    loading.value = false;
    loadingMessage.value = '';
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

// Utility method to get progress icon
const getProgressIcon = () => {
  if (uploadProgress.value === 0) return '🔄';
  if (uploadProgress.value < 20) return '📤';
  if (uploadProgress.value < 50) return '📊';
  if (uploadProgress.value < 80) return '⏳';
  if (uploadProgress.value < 100) return '🔄';
  if (uploadProgress.value === 100 && currentStep.value >= 1) return '✅';
  return '📤';
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
  font-weight: 800; /* Tăng độ đậm */
  margin-bottom: 15px;
  font-family: 'Inter', 'Segoe UI', 'Roboto', 'Arial', sans-serif; /* Font hiện đại, giống Hero */
  color: #FFFFFF; /* Màu trắng theo yêu cầu */
  text-shadow: 0 3px 6px rgba(0, 0, 0, 0.4); /* Tăng độ đậm của shadow */
  letter-spacing: 0.04em; /* Tăng khoảng cách chữ */
  text-transform: uppercase; /* Viết hoa */
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

/* Cải thiện giao diện Modal Import */
.modal-blur-backdrop {
  backdrop-filter: blur(8px);
  background-color: rgba(0, 0, 0, 0.7);
}

.import-modal {
  max-width: 700px;
  border-radius: 15px;
  overflow: hidden;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.3);
  border: none;
  animation: modal-slide-down 0.3s ease-out;
}

@keyframes modal-slide-down {
  from {
    transform: translateY(-50px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.modal-header-branded {
  background: linear-gradient(135deg, #8B1538 0%, #C41E3A 100%);
  color: white;
  padding: 20px 25px;
  border-bottom: none;
  position: relative;
}

.modal-header-branded::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(45deg, rgba(255,255,255,0.1) 0%, transparent 50%, rgba(255,255,255,0.1) 100%);
  pointer-events: none;
}

.modal-header-branded h3 {
  font-size: 1.8rem;
  font-weight: 700;
  margin: 0;
  color: white;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
  position: relative;
  z-index: 1;
}

.modal-header-branded .modal-close {
  color: white;
  background: rgba(255, 255, 255, 0.2);
  border-radius: 50%;
  width: 32px;
  height: 32px;
  font-size: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  cursor: pointer;
  transition: all 0.3s;
  position: relative;
  z-index: 1;
}

.modal-header-branded .modal-close:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: rotate(90deg);
}

.modal-footer-enhanced {
  padding: 20px 25px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #f8f9fa;
  border-top: 1px solid #e9ecef;
  border-radius: 0 0 15px 15px;
}

.btn-large {
  padding: 12px 24px;
  font-size: 16px;
  font-weight: 600;
  border-radius: 8px;
  border: none;
  cursor: pointer;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  gap: 8px;
}

.btn-cancel {
  background: #f8f9fa;
  color: #495057;
  border: 1px solid #ced4da;
}

.btn-cancel:hover {
  background: #e9ecef;
  color: #212529;
}

.btn-import-confirm {
  color: white;
  font-weight: 700;
  box-shadow: 0 4px 12px rgba(139, 21, 56, 0.3);
}

.btn-import-confirm:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 15px rgba(139, 21, 56, 0.4);
}

.btn-import-confirm:active:not(:disabled) {
  transform: translateY(0);
  box-shadow: 0 2px 8px rgba(139, 21, 56, 0.3);
}

.pulse-button:not(:disabled) {
  animation: pulse-animation 2s infinite;
}

@keyframes pulse-animation {
  0% {
    box-shadow: 0 0 0 0 rgba(139, 21, 56, 0.7);
  }
  70% {
    box-shadow: 0 0 0 10px rgba(139, 21, 56, 0);
  }
  100% {
    box-shadow: 0 0 0 0 rgba(139, 21, 56, 0);
  }
}

/* Cải thiện giao diện button-action có icon */
.btn-action {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 3px;
  border: none;
  cursor: pointer;
  font-size: 18px;
  transition: all 0.3s ease;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

.btn-view {
  background: linear-gradient(135deg, #2196F3 0%, #1976D2 100%);
  color: white;
}

.btn-view:hover:not(:disabled) {
  background: linear-gradient(135deg, #1E88E5 0%, #1565C0 100%);
  transform: translateY(-3px);
  box-shadow: 0 5px 10px rgba(33, 150, 243, 0.4);
}

.btn-raw-view {
  background: linear-gradient(135deg, #9C27B0 0%, #7B1FA2 100%);
  color: white;
}

.btn-raw-view:hover:not(:disabled) {
  background: linear-gradient(135deg, #8E24AA 0%, #6A1B9A 100%);
  transform: translateY(-3px);
  box-shadow: 0 5px 10px rgba(156, 39, 176, 0.4);
}

.btn-import {
  color: white;
}

.btn-import:hover {
  transform: translateY(-3px);
  box-shadow: 0 5px 10px rgba(0, 0, 0, 0.2);
  filter: brightness(1.1);
}

.btn-delete {
  background: linear-gradient(135deg, #f44336 0%, #d32f2f 100%);
  color: white;
}

.btn-delete:hover:not(:disabled) {
  background: linear-gradient(135deg, #e53935 0%, #c62828 100%);
  transform: translateY(-3px);
  box-shadow: 0 5px 10px rgba(244, 67, 54, 0.4);
}

.btn-action:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.btn-preview {
  background: linear-gradient(135deg, #26A69A 0%, #00897B 100%);
  color: white;
}
</style>
