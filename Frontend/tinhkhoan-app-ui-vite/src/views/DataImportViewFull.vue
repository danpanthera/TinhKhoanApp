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
          <button @click="openSmartImportModal" class="btn-smart-import" :disabled="loading">
            🧠 Smart Import
          </button>
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
            <h2>📊 BẢNG DỮ LIỆU THÔ</h2>
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
      <div class="modal-content import-modal" @click.stop>
        <div class="modal-header">
          <div class="modal-header-content">
            <div class="modal-icon">📤</div>
            <h3>Import dữ liệu {{ selectedDataType }}</h3>
          </div>
          <button @click="closeImportModal" class="modal-close" aria-label="Đóng">
            <span aria-hidden="true">×</span>
          </button>
        </div>
        <div class="modal-body">
          <!-- Form upload file -->
          <div class="import-form">
            <div class="form-group">
              <label class="form-label">Chọn file để import:</label>
              <div class="file-input-container">
                <input
                  type="file"
                  ref="fileInput"
                  multiple
                  @change="handleFileSelect"
                  class="file-input"
                  id="file-upload"
                />
                <label for="file-upload" class="file-input-label">
                  <span class="file-icon">📎</span>
                  <span>Chọn tệp</span>
                </label>
                <span class="file-selected-text">{{ selectedFiles.length > 0 ?
                  `Đã chọn ${selectedFiles.length} tệp` : 'Chưa có tệp nào được chọn' }}</span>
              </div>
            </div>

            <!-- Danh sách file đã chọn -->
            <div v-if="selectedFiles.length > 0" class="selected-files">
              <h4>Files đã chọn:</h4>
              <ul class="files-list">
                <li v-for="(file, index) in selectedFiles" :key="index" class="file-item">
                  <div class="file-info">
                    <span class="file-icon">{{ getFileIcon(file.name) }}</span>
                    <span class="file-name">{{ file.name }}</span>
                    <span class="file-size">({{ formatFileSize(file.size) }})</span>
                  </div>
                  <button @click="removeFile(index)" class="btn-remove" title="Xóa file này">×</button>
                </li>
              </ul>
            </div>

            <!-- Upload progress indicator -->
            <div v-if="uploading" class="upload-progress-container">
              <div class="upload-status">
                <span class="upload-status-icon">{{ getUploadStatusIcon() }}</span>
                <span class="upload-status-text">{{ getUploadStatusText() }}</span>
              </div>
              <div class="progress-bar-wrapper">
                <div class="progress-bar" :style="{ width: `${uploadProgress}%` }"></div>
              </div>
              <div class="progress-details">
                <span class="progress-percentage">{{ uploadProgress }}%</span>
                <span class="progress-file-info" v-if="currentUploadingFile && totalFiles > 0">
                  <strong>{{ currentUploadingFile }}</strong>
                  <br>
                  <small>Đang xử lý file {{ uploadedFiles }}/{{ totalFiles }}</small>
                </span>
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
          <button @click="closeImportModal" class="btn-cancel">
            <span class="btn-icon">✖️</span>
            <span>Hủy</span>
          </button>
          <button
            @click="performImport"
            class="btn-submit"
            :disabled="selectedFiles.length === 0 || uploading"
          >
            <span class="btn-icon">{{ uploading ? '⏳' : '📤' }}</span>
            <span>{{ uploading ? 'Đang xử lý...' : 'Import Dữ liệu' }}</span>
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
            <!-- Show processed data if available -->
            <div v-if="filteredResults[0]?.isProcessedView && filteredResults[0]?.processedData" class="processed-data-section">
              <div class="table-summary">
                <p><strong>📊 Dữ liệu đã xử lý từ {{ filteredResults[0].tableName }}</strong></p>
                <p>Hiển thị {{ filteredResults[0].processedData.length }} bản ghi đã xử lý</p>
                <p class="data-source-info">Nguồn: {{ filteredResults[0].dataSource }}</p>
              </div>

              <div class="responsive-table-wrapper">
                <table class="data-table enhanced-table">
                  <thead class="agribank-thead">
                    <tr>
                      <th style="width: 50px; text-align: center;">#</th>
                      <th v-for="(column, index) in Object.keys(filteredResults[0].processedData[0] || {}).slice(0, 10)" :key="index">
                        {{ column }}
                      </th>
                    </tr>
                  </thead>
                  <tbody class="agribank-tbody">
                    <tr v-for="(record, recordIndex) in filteredResults[0].processedData.slice(0, 50)" :key="recordIndex">
                      <td style="text-align: center; font-weight: bold; color: #8B1538;">{{ recordIndex + 1 }}</td>
                      <td v-for="(column, columnIndex) in Object.keys(record).slice(0, 10)" :key="columnIndex">
                        <span :title="record[column]">{{ formatCellValue(record[column]) }}</span>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>

              <div class="table-note">
                <p><i>💡 Hiển thị 10 cột đầu tiên và tối đa 50 bản ghi. Đây là dữ liệu đã xử lý và lưu trong bảng lịch sử.</i></p>
              </div>
            </div>

            <!-- Show import list if no processed data -->
            <div v-else>
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
          <h3>📊 Chi tiết dữ liệu {{ selectedDataType }}</h3>
          <button @click="closeRawDataModal" class="modal-close">×</button>
        </div>
        <div class="modal-body">
          <div v-if="rawDataRecords.length > 0" class="raw-data-table-container">
            <div class="table-summary">
              <p><strong>📋 Hiển thị {{ rawDataRecords.length }} bản ghi đầu tiên</strong>
                (tối đa 20 bản ghi để đảm bảo hiệu năng)</p>
            </div>
            <div class="responsive-table-wrapper">
              <table class="raw-data-table enhanced-table">
                <thead class="agribank-thead">
                  <tr>
                    <th style="width: 50px; text-align: center;">#</th>
                    <th v-for="(column, index) in Object.keys(rawDataRecords[0]).slice(0, 12)" :key="index">
                      {{ column }}
                    </th>
                  </tr>
                </thead>
                <tbody class="agribank-tbody">
                  <tr v-for="(record, recordIndex) in rawDataRecords" :key="recordIndex">
                    <td style="text-align: center; font-weight: bold; color: #8B1538;">{{ recordIndex + 1 }}</td>
                    <td v-for="(column, columnIndex) in Object.keys(record).slice(0, 12)" :key="columnIndex">
                      <span :title="record[column]">{{ formatCellValue(record[column]) }}</span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
            <div class="table-note">
              <p><i>💡 Lưu ý: Hiển thị tối đa 12 cột đầu tiên. Hover vào ô để xem đầy đủ nội dung.</i></p>
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

    <!-- Smart Import Modal -->
    <div v-if="showSmartImportModal" class="modal-overlay" @click="closeSmartImportModal">
      <div class="modal-content smart-import-modal" @click.stop>
        <div class="modal-header">
          <h3>🧠 Smart Import - Tự động phân loại dữ liệu</h3>
          <button class="modal-close" @click="closeSmartImportModal">×</button>
        </div>
        <div class="modal-body">
          <div class="smart-import-info">
            <div class="feature-highlights">
              <h4>✨ Tính năng Smart Import:</h4>
              <ul>
                <li>🔍 <strong>Tự động phân loại</strong> file dựa trên tên file</li>
                <li>📅 <strong>Tự động extract</strong> ngày dữ liệu từ filename (pattern: *yyyymmdd.csv*)</li>
                <li>🎯 <strong>Import trực tiếp</strong> vào đúng bảng dữ liệu thô</li>
                <li>⚡ <strong>Xử lý hàng loạt</strong> nhiều file cùng lúc</li>
                <li>📊 <strong>Báo cáo chi tiết</strong> kết quả import</li>
              </ul>
            </div>
          </div>

          <div class="smart-upload-section">
            <div class="date-input-section">
              <label for="smartStatementDate">📅 Ngày sao kê (tùy chọn):</label>
              <input
                id="smartStatementDate"
                v-model="smartStatementDate"
                type="date"
                class="date-input agribank-date-input"
                title="Nếu không chọn, hệ thống sẽ tự động extract từ tên file"
              />
              <small class="date-help">💡 Để trống để hệ thống tự động extract từ tên file</small>
            </div>

            <div class="file-drop-area"
                 :class="{ 'drag-over': isDragOver }"
                 @dragover.prevent="isDragOver = true"
                 @dragleave.prevent="isDragOver = false"
                 @drop.prevent="handleSmartFileDrop">
              <div class="drop-content">
                <div class="upload-icon">📁</div>
                <p class="drop-text">Kéo thả file vào đây hoặc</p>
                <button type="button" class="btn-select-files" @click="$refs.smartFileInput.click()">
                  Chọn file
                </button>
                <input
                  ref="smartFileInput"
                  type="file"
                  multiple
                  accept=".csv,.xlsx,.xls"
                  style="display: none"
                  @change="handleSmartFileSelect"
                />
              </div>
            </div>

            <div v-if="smartSelectedFiles.length > 0" class="selected-files-list">
              <h4>📋 File đã chọn ({{ smartSelectedFiles.length }}):</h4>
              <div class="files-preview">
                <div v-for="(file, index) in smartSelectedFiles" :key="index" class="file-item">
                  <div class="file-info">
                    <span class="file-name">{{ file.name }}</span>
                    <span class="file-size">({{ formatFileSize(file.size) }})</span>
                    <span class="detected-category" :class="'category-' + detectCategory(file.name)">
                      {{ detectCategory(file.name) }}
                    </span>
                    <span v-if="extractDateFromFileName(file.name)" class="detected-date">
                      📅 {{ formatDate(extractDateFromFileName(file.name)) }}
                    </span>
                  </div>
                  <button @click="removeSmartFile(index)" class="btn-remove-file">×</button>
                </div>
              </div>
            </div>

            <div v-if="smartUploading" class="smart-upload-progress">
              <div class="progress-header">
                <h4>🚀 Đang xử lý Smart Import...</h4>
                <span class="progress-text">{{ smartUploadProgress.current }}/{{ smartUploadProgress.total }}</span>
              </div>
              <div class="progress-bar-container">
                <div class="progress-bar" :style="{ width: smartUploadProgress.percentage + '%' }"></div>
              </div>
              <p class="current-file">📤 {{ smartUploadProgress.currentFile }}</p>
            </div>

            <div v-if="smartImportResults && smartImportResults.results" class="smart-import-results">
              <h4>📊 Kết quả Smart Import:</h4>
              <div class="results-summary">
                <div class="result-stats">
                  <span class="stat success">✅ Thành công: {{ smartImportResults.successCount }}</span>
                  <span class="stat error">❌ Lỗi: {{ smartImportResults.failureCount }}</span>
                  <span class="stat total">📁 Tổng: {{ smartImportResults.totalFiles }}</span>
                </div>
              </div>
              <div class="results-detail">
                <div v-for="result in smartImportResults.results" :key="result.index"
                     class="result-item" :class="{ success: result.success, error: !result.success }">
                  <div class="result-status">
                    {{ result.success ? '✅' : '❌' }}
                  </div>
                  <div class="result-info">
                    <strong>{{ result.fileName }}</strong>
                    <div v-if="result.success" class="success-details">
                      <span>Category: {{ result.result?.detectedCategory || 'N/A' }}</span>
                      <span>Records: {{ result.result?.importedRecords || 0 }}</span>
                    </div>
                    <div v-else class="error-details">
                      <span class="error-message">{{ result.error }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button @click="closeSmartImportModal" class="btn-cancel" :disabled="smartUploading">
            Đóng
          </button>
          <button
            @click="startSmartImport"
            class="btn-smart-upload"
            :disabled="smartSelectedFiles.length === 0 || smartUploading"
          >
            🚀 Bắt đầu Smart Import
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import api from '@/services/api'; // ✅ Import api để sử dụng trong fallback strategy
import rawDataService from '@/services/rawDataService';
import smartImportService from '@/services/smartImportService';
import { computed, ref } from 'vue';

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
const importNotes = ref('')
const uploading = ref(false)

// Smart Import state
const showSmartImportModal = ref(false)
const smartSelectedFiles = ref([])
const smartStatementDate = ref('')
const smartUploading = ref(false)
const smartImportResults = ref(null)
const smartUploadProgress = ref({
  current: 0,
  total: 0,
  percentage: 0,
  currentFile: ''
})
const isDragOver = ref(false)

// State cho upload progress tracking chi tiết
const uploadProgress = ref(0)
const currentUploadingFile = ref('')
const uploadedFiles = ref(0)
const totalFiles = ref(0)
const uploadStartTime = ref(null)
const estimatedTimePerFile = ref(5000) // 5 giây ước tính mỗi file
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

// Methods
const clearMessage = () => {
  errorMessage.value = ''
  successMessage.value = ''
}

const showError = (message) => {
  errorMessage.value = message
  console.error('❌ Error message:', message)
  setTimeout(() => {
    errorMessage.value = ''
  }, 5000)
}

const showDetailedError = (mainMessage, error) => {
  // Hiển thị thông báo lỗi chi tiết hơn để dễ dàng debug
  console.error('❌ Detailed Error:', mainMessage);
  console.error('❌ Error Object:', error);
  console.error('❌ Error Details:', {
    errorType: typeof error,
    errorMessage: error?.message,
    errorResponse: error?.response,
    errorData: error?.response?.data,
    errorStatus: error?.response?.status,
    errorCode: error?.code,
    // Serialize object để xem chi tiết
    fullError: JSON.stringify(error, null, 2)
  });

  let detailedMessage = mainMessage

  // Xử lý các loại lỗi khác nhau
  if (error?.success === false) {
    // Trường hợp API response với success: false
    detailedMessage += `: ${error.error || 'Unknown API error'}`
  } else if (error?.response?.data?.message) {
    detailedMessage += `: ${error.response.data.message}`
  } else if (error?.message) {
    detailedMessage += `: ${error.message}`
  } else if (typeof error === 'string') {
    detailedMessage += `: ${error}`
  } else if (error?.error) {
    detailedMessage += `: ${error.error}`
  }

  // Thêm thông tin debug nếu cần
  if (process.env.NODE_ENV === 'development') {
    detailedMessage += ` (Status: ${error?.response?.status || error?.status || 'unknown'})`
  }

  errorMessage.value = detailedMessage
  setTimeout(() => {
    errorMessage.value = ''
  }, 8000) // Hiển thị lâu hơn để người dùng có thể đọc
}

const showSuccess = (message, timeout = 3000) => {
  successMessage.value = message
  setTimeout(() => {
    successMessage.value = ''
  }, timeout)
}

// Upload status text với thông tin chi tiết
const getUploadStatusText = () => {
  if (uploadProgress.value === 0) return 'Đang chuẩn bị upload...'

  if (totalFiles.value <= 1) {
    // Single file upload
    if (uploadProgress.value < 20) return 'Đang tải file lên server...'
    if (uploadProgress.value < 60) return 'Đang xử lý và phân tích dữ liệu...'
    if (uploadProgress.value < 90) return 'Đang lưu vào cơ sở dữ liệu...'
    if (uploadProgress.value < 100) return 'Sắp hoàn thành...'
  } else {
    // Multiple files upload
    if (uploadProgress.value < 15) return `Đang tải file ${uploadedFiles}/${totalFiles} lên server...`
    if (uploadProgress.value < 85) return `Đang xử lý file ${uploadedFiles}/${totalFiles}...`
    if (uploadProgress.value < 100) return `Đang hoàn tất xử lý ${totalFiles} files...`
  }

  return 'Đã hoàn thành tất cả!'
}

// Format date từ chuỗi ISO
const formatDate = (dateString) => {
  if (!dateString) return 'N/A'

  try {
    const date = new Date(dateString)
    if (isNaN(date.getTime())) {
      return 'Ngày không hợp lệ'
    }

    const day = String(date.getDate()).padStart(2, '0')
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const year = date.getFullYear()

    return `${day}/${month}/${year}`
  } catch (error) {
    console.error('Error formatting date:', error)
    return 'Lỗi format ngày'
  }
}

// Data type statistics
const getDataTypeStats = (dataType) => {
  const stats = dataTypeStats.value[dataType] || { totalRecords: 0, lastUpdate: null }
  // Fix NaN issue: ensure totalRecords is always a valid number
  const totalRecords = parseInt(stats.totalRecords) || 0
  return {
    ...stats,
    totalRecords: formatRecordCount(totalRecords) // Use local formatRecordCount instead
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

  try {
    loading.value = true
    loadingMessage.value = 'Đang lọc dữ liệu theo ngày...'

    // Format dates to yyyyMMdd
    const fromDateFormatted = selectedFromDate.value.replace(/-/g, '')
    const toDateFormatted = selectedToDate.value ? selectedToDate.value.replace(/-/g, '') : fromDateFormatted

    // Get data for all data types in date range
    const allResults = []

    for (const dataType of Object.keys(dataTypeDefinitions)) {
      try {
        if (selectedFromDate.value === selectedToDate.value || !selectedToDate.value) {
          // Single date filter
          const result = await rawDataService.getByStatementDate(dataType, fromDateFormatted)
          if (result.success && result.data.length > 0) {
            allResults.push(...result.data)
          }
        } else {
          // Date range filter
          const result = await rawDataService.getByDateRange(dataType, selectedFromDate.value, selectedToDate.value)
          if (result.success && result.data.length > 0) {
            allResults.push(...result.data)
          }
        }
      } catch (error) {
        console.warn(`No data found for ${dataType} in date range`)
      }
    }

    if (allResults.length > 0) {
      filteredResults.value = allResults
      showSuccess(`✅ Tìm thấy ${allResults.length} bản ghi trong khoảng thời gian đã chọn`)
    } else {
      filteredResults.value = []
      showError('Không tìm thấy dữ liệu trong khoảng thời gian đã chọn')
    }
  } catch (error) {
    console.error('Error filtering by date:', error)
    showError('Có lỗi xảy ra khi lọc dữ liệu theo ngày')
  } finally {
    loading.value = false
  }
}

const clearDateFilter = () => {
  selectedFromDate.value = ''
  selectedToDate.value = ''
  showSuccess('Đã xóa bộ lọc ngày')
}

// Data management methods
const refreshAllData = async (skipSuccessMessage = false) => {
  try {
    loading.value = true
    loadingMessage.value = 'Đang tải lại dữ liệu...'

    console.log('🔄 Starting refresh all data...')

    const result = await rawDataService.getAllImports()
    console.log('📊 Raw result from getAllImports:', {
      success: result.success,
      dataLength: result.data ? result.data.length : 0,
      error: result.error,
      resultType: typeof result
    })

    if (result.success) {
      allImports.value = result.data || []
      console.log('✅ Loaded imports:', allImports.value.length, 'items')

      // Debug log để kiểm tra dữ liệu
      if (allImports.value.length > 0) {
        console.log('📊 Sample import data:', allImports.value[0])
      }

      calculateDataTypeStats()

      if (!skipSuccessMessage) {
        showSuccess(`✅ Đã tải lại dữ liệu thành công (${allImports.value.length} imports)`)
      }

      return { success: true, data: allImports.value };
    } else {
      const errorMsg = result.error || 'Không thể tải dữ liệu'
      console.error('🔥 Chi tiết lỗi getAllImports:', {
        error: result.error,
        errorCode: result.errorCode,
        errorStatus: result.errorStatus,
        fullResult: result
      })

      if (result.fallbackData && result.fallbackData.length > 0) {
        allImports.value = result.fallbackData
        calculateDataTypeStats()
        if (!skipSuccessMessage) {
          showError(`⚠️ Chế độ Demo: ${errorMsg}`)
        }
        return { success: false, error: errorMsg, fallback: true };
      } else {
        allImports.value = []
        calculateDataTypeStats()
        if (!skipSuccessMessage) {
          console.error('❌ Error in refreshAllData, will not show error to user during import flow')
        }
        return { success: false, error: errorMsg };
      }
    }

  } catch (error) {
    console.error('❌ Exception in refreshAllData:', error)
    if (!skipSuccessMessage) {
      console.error('❌ Refresh error, will not show to user during import flow')
    }
    return { success: false, error: error.message };
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

// ✅ Thêm hàm refresh dữ liệu với nhiều cách fallback khác nhau
const refreshDataWithFallback = async () => {
  console.log('🔄 Refresh data with multiple fallback strategies...');

  try {
    // Chiến thuật 1: Gọi getRecentImports (nhanh nhất)
    console.log('📊 Strategy 1: getRecentImports');
    const recentResult = await rawDataService.getRecentImports(50);

    if (recentResult.success && recentResult.data && recentResult.data.length > 0) {
      console.log('✅ Strategy 1 success:', recentResult.data.length, 'items');
      allImports.value = recentResult.data;
      calculateDataTypeStats();
      return { success: true, data: recentResult.data, strategy: 'getRecentImports' };
    }

    // Chiến thuật 2: Gọi getAllImports
    console.log('📊 Strategy 2: getAllImports');
    const importResult = await rawDataService.getAllImports();

    if (importResult.success && importResult.data && importResult.data.length > 0) {
      console.log('✅ Strategy 2 success:', importResult.data.length, 'items');
      allImports.value = importResult.data;
      calculateDataTypeStats();
      return { success: true, data: importResult.data, strategy: 'getAllImports' };
    }

    // Chiến thuật 3: Gọi getAllData
    console.log('📊 Strategy 3: getAllData');
    const dataResult = await rawDataService.getAllData();

    if (dataResult.success && dataResult.data && dataResult.data.length > 0) {
      console.log('✅ Strategy 3 success:', dataResult.data.length, 'items');
      allImports.value = dataResult.data;
      calculateDataTypeStats();
      return { success: true, data: dataResult.data, strategy: 'getAllData' };
    }

    // Chiến thuật 4: Gọi trực tiếp API endpoint recent
    console.log('📊 Strategy 4: Direct API recent call');
    const directRecentResult = await api.get('/RawData/recent?limit=50');

    if (directRecentResult.data && Array.isArray(directRecentResult.data)) {
      const mappedData = directRecentResult.data.map(item => ({
        ...item,
        dataType: item.category || item.dataType || item.fileType || 'UNKNOWN',
        category: item.category || item.dataType || '',
        recordsCount: parseInt(item.recordsCount || 0),
        fileName: item.fileName || 'Unknown File'
      }));

      console.log('✅ Strategy 4 success:', mappedData.length, 'items');
      allImports.value = mappedData;
      calculateDataTypeStats();
      return { success: true, data: mappedData, strategy: 'directRecentAPI' };
    }

    // Chiến thuật 5: Gọi trực tiếp API endpoint chính
    console.log('📊 Strategy 5: Direct API call');
    const directResult = await api.get('/RawData');

    if (directResult.data && Array.isArray(directResult.data)) {
      const mappedData = directResult.data.map(item => ({
        ...item,
        dataType: item.category || item.dataType || item.fileType || 'UNKNOWN',
        category: item.category || item.dataType || '',
        recordsCount: parseInt(item.recordsCount || 0),
        fileName: item.fileName || 'Unknown File'
      }));

      console.log('✅ Strategy 5 success:', mappedData.length, 'items');
      allImports.value = mappedData;
      calculateDataTypeStats();
      return { success: true, data: mappedData, strategy: 'directAPI' };
    }

    console.log('❌ All strategies failed');
    return { success: false, error: 'All refresh strategies failed' };

  } catch (error) {
    console.error('❌ Error in refreshDataWithFallback:', error);
    return { success: false, error: error.message };
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
          // 🔥 ENHANCED: For BC57, DPDA, LN01, GL41, and 7800_DT_KHKD1, try to show processed data instead of raw import data
          if (['BC57', 'DPDA', 'LN01', 'GL41', '7800_DT_KHKD1'].includes(dataType.toUpperCase()) && filteredResults.value.length > 0) {
            const importId = filteredResults.value[0].id
            console.log(`🔄 Fetching processed data for ${dataType} import ID: ${importId}`)

            const processedResult = await rawDataService.getProcessedData(importId)
            if (processedResult.success && processedResult.data.processedData && processedResult.data.processedData.length > 0) {
              // Replace import list with processed data for better viewing
              filteredResults.value = [{
                ...filteredResults.value[0],
                processedData: processedResult.data.processedData,
                tableName: processedResult.data.tableName,
                dataSource: processedResult.data.dataSource,
                isProcessedView: true
              }]

              showSuccess(`📊 Hiển thị ${processedResult.data.processedData.length} bản ghi đã xử lý từ ${processedResult.data.tableName}`)
            } else {
              showSuccess(`Hiển thị ${filteredResults.value.length} import(s) cho loại ${dataType} ngày ${formatDate(selectedFromDate.value)}`)
            }
          } else {
            showSuccess(`Hiển thị ${filteredResults.value.length} import(s) cho loại ${dataType} ngày ${formatDate(selectedFromDate.value)}`)
          }

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
      // ✅ FIX: Hiển thị modal với dữ liệu thay vì chỉ báo "tính năng đang phát triển"
      const previewRows = result.data.previewRows || result.data.PreviewData || result.data.previewData || []

      if (previewRows && previewRows.length > 0) {
        // Hiển thị tối đa 20 bản ghi đầu như yêu cầu
        const recordsToShow = previewRows.slice(0, 20)

        // Cập nhật state để hiển thị modal
        rawDataRecords.value = recordsToShow
        selectedDataType.value = result.data.importInfo?.DataType || result.data.dataType || 'Dữ liệu chi tiết'

        showSuccess(`✅ Đã tải ${recordsToShow.length} bản ghi chi tiết đầu tiên`)
        showRawDataModal.value = true
      } else {
        showError('Không tìm thấy dữ liệu chi tiết trong bản ghi này')
      }
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
  importNotes.value = ''
  uploading.value = false
  uploadProgress.value = 0
  currentUploadingFile.value = ''
  uploadedFiles.value = 0
  totalFiles.value = 0
  showImportModal.value = true
}

// Đóng modal import
const closeImportModal = () => {
  if (uploading.value && uploadProgress.value < 100) {
    // Nếu đang upload, hiển thị xác nhận
    if (!confirm('Bạn có chắc muốn hủy quá trình import? Dữ liệu đang được tải lên sẽ bị mất.')) {
      return; // Người dùng không muốn hủy
    }
    // TODO: Hủy quá trình upload nếu cần
  }

  showImportModal.value = false
  selectedFiles.value = []
  importNotes.value = ''
  uploading.value = false
  uploadProgress.value = 0
}

// Thực hiện import dữ liệu
const performImport = async () => {
  if (selectedFiles.value.length === 0) {
    showError('Vui lòng chọn ít nhất một file để import')
    return
  }

  uploading.value = true
  uploadProgress.value = 0
  totalFiles.value = selectedFiles.value.length
  uploadedFiles.value = 0

  try {
    // Log thông tin trước khi gọi API
    console.log(`📤 Importing data for ${selectedDataType.value} with ${selectedFiles.value.length} files...`, {
      dataType: selectedDataType.value,
      files: selectedFiles.value.map(f => ({ name: f.name, size: f.size })),
      notes: importNotes.value,
      statementDate: selectedFromDate.value
    })

    currentUploadingFile.value = selectedFiles.value[0].name

    // Reset progress tracking
    uploadProgress.value = 0
    uploadedFiles.value = 0
    uploadStartTime.value = Date.now()

    // Cập nhật tổng số files để hiển thị
    totalFiles.value = selectedFiles.value.length

    // Ước tính thời gian dựa trên size file
    const avgFileSize = selectedFiles.value.reduce((sum, f) => sum + f.size, 0) / selectedFiles.value.length
    estimatedTimePerFile.value = Math.max(3000, Math.min(15000, avgFileSize / 50000)) // 3-15 giây tùy size

    // Chuẩn bị options cho API call với progress tracking cải tiến
    const options = {
      notes: importNotes.value,
      statementDate: selectedFromDate.value,
      onProgress: (progressInfo) => {
        // Cập nhật thông tin progress chung từ backend
        uploadProgress.value = progressInfo.percentage

        // Tính toán file đang được xử lý dựa trên tiến độ và thời gian
        const elapsedTime = Date.now() - uploadStartTime.value
        const estimatedCurrentFile = Math.min(
          Math.floor(elapsedTime / estimatedTimePerFile.value),
          Math.floor(progressInfo.percentage / (100 / selectedFiles.value.length))
        )

        // Đảm bảo index không vượt quá số file có sẵn
        const fileIndex = Math.max(0, Math.min(estimatedCurrentFile, selectedFiles.value.length - 1))

        // Cập nhật file hiện tại đang được xử lý
        if (fileIndex >= 0 && fileIndex < selectedFiles.value.length) {
          currentUploadingFile.value = selectedFiles.value[fileIndex].name

          // Logic cập nhật số file đã upload dựa trên progress
          if (progressInfo.percentage < 10) {
            uploadedFiles.value = 0
          } else if (progressInfo.percentage >= 95) {
            uploadedFiles.value = selectedFiles.value.length
            currentUploadingFile.value = "Hoàn thành tất cả files"
          } else {
            // Tính toán số file đã hoàn thành dựa trên progress
            const completedFiles = Math.floor((progressInfo.percentage / 100) * selectedFiles.value.length)
            uploadedFiles.value = Math.min(completedFiles + 1, selectedFiles.value.length) // +1 cho file đang xử lý
          }
        }

        console.log(`📊 Upload Progress: ${progressInfo.percentage}%, File ${uploadedFiles.value}/${totalFiles.value}: ${currentUploadingFile.value}`)
      }
    }

    // Gọi API thực tế thay vì mô phỏng
    const response = await rawDataService.importData(selectedDataType.value, selectedFiles.value, options)

    if (response.success) {
      uploadProgress.value = 100
      setTimeout(async () => {
        uploading.value = false
        showSuccess(`Import dữ liệu ${selectedDataType.value} thành công!`)

        // Đóng modal import
        closeImportModal()

        // ✅ FIX: Làm mới dữ liệu với độ trễ đủ để backend xử lý xong
        setTimeout(async () => {
          console.log('� Refresh data sau khi import thành công...');

          try {
            loading.value = true
            loadingMessage.value = `Đang tải dữ liệu mới nhất...`

            // ✅ FIX: Sử dụng hàm refresh với fallback strategies
            const refreshResult = await refreshDataWithFallback()

            console.log('📊 Dữ liệu sau khi refresh:', {
              success: refreshResult.success,
              strategy: refreshResult.strategy,
              totalImports: allImports.value.length,
              dataTypes: allImports.value.map(imp => imp.dataType || imp.category || imp.fileType).filter((v, i, a) => a.indexOf(v) === i)
            });

            if (refreshResult.success && allImports.value.length > 0) {
              // ✅ Lọc và hiển thị dữ liệu theo loại đã import
              const dataTypeResults = allImports.value.filter(imp => {
                const typeMatches =
                  (imp.dataType && imp.dataType.includes(selectedDataType.value)) ||
                  (imp.category && imp.category.includes(selectedDataType.value)) ||
                  (imp.fileType && imp.fileType.includes(selectedDataType.value));

                return typeMatches;
              });

              console.log(`🔍 Filtered results for ${selectedDataType.value}:`, dataTypeResults.length);

              if (dataTypeResults.length > 0) {
                filteredResults.value = dataTypeResults;
                showSuccess(`✅ Hiển thị ${dataTypeResults.length} import(s) cho loại ${selectedDataType.value}`);
                showDataViewModal.value = true;
              } else {
                // ✅ Hiển thị tất cả dữ liệu mới nhất nếu không tìm thấy theo loại cụ thể
                filteredResults.value = allImports.value.slice(0, 10); // Hiển thị 10 import mới nhất
                showSuccess(`✅ Hiển thị ${filteredResults.value.length} bản ghi import mới nhất`);
                showDataViewModal.value = true;
              }
            } else {
              console.log('⚠️ Không có dữ liệu sau khi refresh, thử gọi API trực tiếp...');

              // Thử gọi API trực tiếp để lấy dữ liệu
              const directResult = await rawDataService.getAllData();

              if (directResult.success && directResult.data && directResult.data.length > 0) {
                console.log(`✅ API trực tiếp trả về ${directResult.data.length} bản ghi`);

                filteredResults.value = directResult.data.slice(0, 10); // Hiển thị 10 bản ghi mới nhất
                showSuccess(`✅ Hiển thị ${filteredResults.value.length} bản ghi import mới nhất`);
                showDataViewModal.value = true;
              } else {
                showSuccess(`✅ Import thành công! Vui lòng nhấn "🔄 Tải lại dữ liệu" để xem kết quả.`);
              }
            }

          } catch (error) {
            console.error('❌ Error fetching data after import:', error);
            showSuccess(`✅ Import thành công! Vui lòng nhấn "🔄 Tải lại dữ liệu" để xem kết quả.`);
          } finally {
            loading.value = false;
            loadingMessage.value = '';
          }
        }, 2500); // ✅ Tăng delay thành 2.5 giây để đảm bảo backend xử lý xong
      }, 1000)
    } else {
      showDetailedError(`Lỗi khi import dữ liệu`, response)
      uploading.value = false
    }
  } catch (error) {
    console.error('Error importing data:', error)
    showDetailedError(`Lỗi khi import dữ liệu`, error)
    uploading.value = false
  }
}

// Xóa file khỏi danh sách chọn
const removeFile = (index) => {
  selectedFiles.value.splice(index, 1)
}

// Lấy icon tương ứng với loại file
const getFileIcon = (fileName) => {
  const extension = fileName.split('.').pop()?.toLowerCase() || ''

  const icons = {
    'pdf': '📄',
    'doc': '📝',
    'docx': '📝',
    'xls': '📊',
    'xlsx': '📊',
    'csv': '📋',
    'txt': '📄',
    'zip': '📦',
    'rar': '📦',
    '7z': '📦',
    'png': '🖼️',
    'jpg': '🖼️',
    'jpeg': '🖼️',
    'gif': '🖼️'
  }

  return icons[extension] || '📄'
}

// Lấy icon trạng thái upload
const getUploadStatusIcon = () => {
  if (uploadProgress.value === 0) return '⏳'
  if (uploadProgress.value < 20) return '📤'
  if (uploadProgress.value < 50) return '📤'
  if (uploadProgress.value < 90) return '🔄'
  if (uploadProgress.value < 100) return '🔄'
  return '✅'
}

// Hàm kiểm tra nếu file là file nén
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
  // Fix NaN issue - ensure proper type checking and conversion
  if (count === null || count === undefined || count === '' || isNaN(Number(count))) {
    return '0'
  }

  // Convert to number and format with thousands separator
  const numericCount = Number(count)
  return new Intl.NumberFormat('vi-VN').format(numericCount)
}

// ✅ THÊM MỚI: Hàm format giá trị trong cell để hiển thị đẹp hơn
const formatCellValue = (value) => {
  if (value === null || value === undefined) return '—'
  if (value === '') return '(trống)'

  // Nếu là string dài, cắt ngắn
  if (typeof value === 'string') {
    if (value.length > 50) {
      return value.substring(0, 47) + '...'
    }
    return value
  }

  // Nếu là số, format với dấu phân cách
  if (typeof value === 'number') {
    return new Intl.NumberFormat('vi-VN').format(value)
  }

  // Nếu là date, format ngày
  if (value instanceof Date || (typeof value === 'string' && value.match(/^\d{4}-\d{2}-\d{2}/))) {
    try {
      const date = new Date(value)
      return date.toLocaleDateString('vi-VN')
    } catch (e) {
      return value
    }
  }

  return String(value)
}

// ==================== SMART IMPORT METHODS ====================

// Mở modal Smart Import
const openSmartImportModal = () => {
  showSmartImportModal.value = true
  smartSelectedFiles.value = []
  smartImportResults.value = null
  smartUploadProgress.value = { current: 0, total: 0, percentage: 0, currentFile: '' }
}

// Đóng modal Smart Import
const closeSmartImportModal = () => {
  if (!smartUploading.value) {
    showSmartImportModal.value = false
    smartSelectedFiles.value = []
    smartImportResults.value = null
    smartStatementDate.value = ''
  }
}

// Xử lý chọn file Smart Import
const handleSmartFileSelect = (event) => {
  console.log('🔍 handleSmartFileSelect called', event)
  const files = event.target.files
  console.log('🔍 Files selected:', files ? files.length : 0)
  if (files.length === 0) return

  smartSelectedFiles.value = Array.from(files)
  console.log('🔍 smartSelectedFiles updated:', smartSelectedFiles.value.length)
}

// Xử lý kéo thả file
const handleSmartFileDrop = (event) => {
  console.log('🔍 handleSmartFileDrop called', event)
  isDragOver.value = false
  const files = event.dataTransfer.files
  console.log('🔍 Files dropped:', files ? files.length : 0)
  if (files.length === 0) return

  smartSelectedFiles.value = Array.from(files)
  console.log('🔍 smartSelectedFiles updated:', smartSelectedFiles.value.length)
}

// Xóa file khỏi danh sách
const removeSmartFile = (index) => {
  smartSelectedFiles.value.splice(index, 1)
}

// Detect category từ filename
const detectCategory = (fileName) => {
  return smartImportService.detectCategoryFromFileName(fileName)
}

// Extract date từ filename
const extractDateFromFileName = (fileName) => {
  return smartImportService.extractDateFromFileName(fileName)
}

// Bắt đầu Smart Import - OPTIMIZED VERSION
const startSmartImport = async () => {
  if (smartSelectedFiles.value.length === 0) {
    errorMessage.value = 'Vui lòng chọn ít nhất một file'
    return
  }

  smartUploading.value = true
  smartImportResults.value = null
  errorMessage.value = ''
  successMessage.value = ''

  try {
    // Prepare statement date
    let statementDate = null
    if (smartStatementDate.value) {
      statementDate = new Date(smartStatementDate.value)
    }

    // Setup progress tracking
    smartUploadProgress.value = {
      current: 0,
      total: smartSelectedFiles.value.length,
      percentage: 0,
      currentFile: ''
    }

    console.log('🧠 Starting OPTIMIZED Smart Import with', smartSelectedFiles.value.length, 'files')

    // ✅ OPTIMIZATION: Sử dụng callback để update progress real-time
    const progressCallback = (progressInfo) => {
      smartUploadProgress.value = {
        current: progressInfo.current,
        total: progressInfo.total,
        percentage: progressInfo.percentage,
        currentFile: progressInfo.currentFile
      }

      console.log(`📊 Progress: ${progressInfo.current}/${progressInfo.total} (${progressInfo.percentage}%) - ${progressInfo.currentFile}`)
    }

    // Call OPTIMIZED Smart Import Service với progress callback
    const results = await smartImportService.uploadSmartFiles(
      smartSelectedFiles.value,
      statementDate,
      progressCallback
    )

    smartImportResults.value = results

    // ✅ OPTIMIZATION: Hiển thị thông tin thời gian
    const avgTimePerFile = results.duration ? (results.duration / results.totalFiles).toFixed(1) : 'N/A'

    if (results.successCount > 0) {
      successMessage.value = `✅ Smart Import hoàn thành! ${results.successCount}/${results.totalFiles} file thành công trong ${results.duration?.toFixed(1)}s (avg: ${avgTimePerFile}s/file)`

      // Refresh data sau khi import thành công
      await refreshAllData()
    }

    if (results.failureCount > 0) {
      errorMessage.value = `⚠️ ${results.failureCount}/${results.totalFiles} file import thất bại. Xem chi tiết bên dưới.`
    }

  } catch (error) {
    console.error('🔥 Smart Import error:', error)
    errorMessage.value = `Lỗi Smart Import: ${error.message}`
  } finally {
    smartUploading.value = false
    smartUploadProgress.value.percentage = 100
  }
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
  color: #f5f5f1; /* Màu trắng ngọc trai */
}

.subtitle {
  font-size: 1.1rem;
  opacity: 0.9;
  margin: 0;
  color: #f5f5f1; /* Màu trắng ngọc trai */
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
  color: #f5f5f1; /* Màu trắng ngọc trai */
}

.header-text p {
  margin: 0;
  opacity: 0.9;
  color: #f5f5f1; /* Màu trắng ngọc trai */
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
  background: rgba(0,0,0,0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
  backdrop-filter: blur(3px);
  animation: fadeIn 0.2s ease;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.modal-content {
  background: white;
  border-radius: 12px;
  max-width: 500px;
  width: 90%;
  max-height: 80vh;
  overflow-y: auto;
  box-shadow: 0 10px 25px rgba(0,0,0,0.3);
  animation: slideDown 0.3s ease;
  border: 1px solid rgba(0,0,0,0.1);
}

.import-modal {
  max-width: 600px;
}

@keyframes slideDown {
  from { transform: translateY(-30px); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}

.modal-header {
  background: linear-gradient(135deg, #8B1538 0%, #C41E3A 100%);
  color: white;
  padding: 20px;
  border-radius: 12px 12px 0 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header-content {
  display: flex;
  align-items: center;
  gap: 12px;
}

.modal-icon {
  font-size: 24px;
}

.modal-header h3 {
  margin: 0;
  font-size: 1.3rem;
}

.modal-close {
  background: none;
  border: none;
  color: white;
  font-size: 24px;
  cursor: pointer;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: all 0.2s;
}

.modal-close:hover {
  background: rgba(255,255,255,0.2);
}

.modal-body {
  padding: 25px;
}

.modal-footer {
  padding: 20px;
  display: flex;
  justify-content: flex-end;
  gap: 15px;
  border-top: 1px solid #eee;
}

/* Form styling */
.import-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-label {
  font-weight: 600;
  color: #333;
  font-size: 0.95rem;
}

.file-input-container {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  align-items: center;
}

.file-input {
  display: none;
}

.file-input-label {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 16px;
  background: #8B1538;
  color: white;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.2s;
}

.file-input-label:hover {
  background: #a91d42;
  transform: translateY(-2px);
}

.file-selected-text {
  font-size: 0.9rem;
  color: #666;
}

.selected-files {
  background: #f8f9fa;
  padding: 15px;
  border-radius: 8px;
  border: 1px solid #eee;
}

.selected-files h4 {
  margin-top: 0;
  margin-bottom: 10px;
  font-size: 0.95rem;
  color: #333;
}

.files-list {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.file-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: white;
  padding: 10px 15px;
  border-radius: 6px;
  border: 1px solid #eee;
}

.file-info {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 0.9rem;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.file-icon {
  font-size: 1.2rem;
}

.file-name {
  font-weight: 500;
  max-width: 300px;
  overflow: hidden;
  text-overflow: ellipsis;
}

.file-size {
  color: #666;
  font-size: 0.85rem;
}

.btn-remove {
  background: none;
  border: none;
  color: #dc3545;
  font-size: 18px;
  cursor: pointer;
  width: 25px;
  height: 25px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: all 0.2s;
}

.btn-remove:hover {
  background: #ffeeee;
}

/* Upload progress styles */
.upload-progress-container {
  background: #f8f9fa;
  padding: 20px;
  border-radius: 8px;
  border: 1px solid #eee;
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.upload-status {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 5px;
}

.upload-status-icon {
  font-size: 1.2rem;
}

.upload-status-text {
  font-weight: 600;
  color: #333;
}

.progress-bar-wrapper {
  height: 12px;
  background-color: #e0e0e0;
  border-radius: 6px;
  overflow: hidden;
  margin-bottom: 8px;
  box-shadow: inset 0 1px 3px rgba(0,0,0,0.1);
}

.progress-bar {
  height: 100%;
  background: linear-gradient(90deg, #8B1538 0%, #C41E3A 100%);
  border-radius: 6px;
  transition: width 0.3s ease;
  position: relative;
  overflow: hidden;
}

.progress-bar::after {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(
    -45deg,
    rgba(255, 255, 255, 0.2) 25%,
    transparent 25%,
    transparent 50%,
    rgba(255, 255, 255, 0.2) 50%,
    rgba(255, 255, 255, 0.2) 75%,
    transparent 75%
  );
  background-size: 30px 30px;
  animation: progressStripes 1s linear infinite;
  z-index: 1;
}

@keyframes progressStripes {
  0% { background-position: 0 0; }
  100% { background-position: 30px 0; }
}

.progress-details {
  display: flex;
  justify-content: space-between;
  font-size: 0.85rem;
  color: #666;
}

.progress-percentage {
  font-weight: bold;
  color: #8B1538;
}

.progress-file-info {
  font-style: italic;
  max-width: 70%;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.notes-input {
  width: 100%;
  min-height: 80px;
  padding: 10px 15px;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 0.95rem;
  resize: vertical;
}

.notes-input:focus {
  outline: none;
  border-color: #8B1538;
}

/* Button styles */
.btn-cancel,
.btn-submit {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 20px;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  border: none;
}

.btn-cancel {
  background: #f0f0f0;
  color: #333;
}

.btn-cancel:hover {
  background: #e0e0e0;
}

.btn-submit {
  background: linear-gradient(135deg, #8B1538 0%, #C41E3A 100%);
  color: white;
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

.btn-icon {
  font-size: 1.1rem;
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
  max-height:   80vh;
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

/* Processed data styles */
.processed-data-section {
  margin-bottom: 20px;
}

.data-source-info {
  color: #8B1538;
  font-weight: 600;
  font-size: 0.9rem;
}

.table-summary p {
  margin: 5px 0;
}

.table-summary .data-source-info {
  background: #f8f9fa;
  padding: 5px 10px;
  border-radius: 4px;
  border-left: 3px solid #8B1538;
}

/* ==================== SMART IMPORT STYLES ==================== */

.btn-smart-import {
  background: linear-gradient(135deg, #007bff 0%, #0056b3 100%);
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s ease;
  box-shadow: 0 2px 4px rgba(0,123,255,0.3);
}

.btn-smart-import:hover:not(:disabled) {
  background: linear-gradient(135deg, #0056b3 0%, #004085 100%);
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0,123,255,0.4);
}

.btn-smart-import:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none;
}

.smart-import-modal {
  max-width: 900px;
  width: 90vw;
  max-height: 90vh;
  overflow-y: auto;
}

.smart-import-info {
  background: linear-gradient(135deg, #e3f2fd 0%, #bbdefb 100%);
  border: 1px solid #2196f3;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 20px;
}

.feature-highlights h4 {
  color: #1976d2;
  margin-bottom: 12px;
  font-size: 1.1rem;
}

.feature-highlights ul {
  margin: 0;
  padding-left: 20px;
}

.feature-highlights li {
  margin-bottom: 8px;
  color: #424242;
  line-height: 1.5;
}

.date-input-section {
  margin-bottom: 20px;
}

.date-input-section label {
  display: block;
  margin-bottom: 8px;
  font-weight: 600;
  color: #333;
}

.date-help {
  display: block;
  margin-top: 4px;
  color: #666;
  font-style: italic;
}

.file-drop-area {
  border: 3px dashed #ddd;
  border-radius: 12px;
  padding: 40px 20px;
  text-align: center;
  transition: all 0.3s ease;
  background: #fafafa;
  cursor: pointer;
}

.file-drop-area.drag-over {
  border-color: #007bff;
  background: #e3f2fd;
  transform: scale(1.02);
}

.file-drop-area:hover {
  border-color: #007bff;
  background: #f0f8ff;
}

.drop-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 15px;
}

.upload-icon {
  font-size: 48px;
  color: #999;
}

.drop-text {
  font-size: 1.1rem;
  color: #666;
  margin: 0;
}

.btn-select-files {
  background: #007bff;
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-select-files:hover {
  background: #0056b3;
}

.selected-files-list {
  margin-top: 20px;
  padding: 20px;
  background: #f8f9fa;
  border-radius: 8px;
  border: 1px solid #dee2e6;
}

.selected-files-list h4 {
  margin-top: 0;
  margin-bottom: 15px;
  color: #333;
}

.files-preview {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.file-item {
  display: flex;
  justify-content: between;
  align-items: center;
  padding: 12px;
  background: white;
  border: 1px solid #ddd;
  border-radius: 6px;
  transition: all 0.2s ease;
}

.file-item:hover {
  border-color: #007bff;
  box-shadow: 0 2px 4px rgba(0,123,255,0.1);
}

.file-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.file-name {
  font-weight: 600;
  color: #333;
}

.file-size {
  font-size: 0.9rem;
  color: #666;
}

.detected-category {
  font-size: 0.85rem;
  font-weight: 600;
  padding: 2px 8px;
  border-radius: 12px;
  display: inline-block;
  width: fit-content;
}

.category-DP01 { background: #e3f2fd; color: #1976d2; }
.category-LN01 { background: #e8f5e8; color: #388e3c; }
.category-LN02 { background: #e8f5e8; color: #388e3c; }
.category-LN03 { background: #e8f5e8; color: #388e3c; }
.category-GL01 { background: #fff3e0; color: #f57c00; }
.category-GL41 { background: #fff3e0; color: #f57c00; }
.category-DB01 { background: #fce4ec; color: #c2185b; }
.category-DPDA { background: #e3f2fd; color: #1976d2; }
.category-EI01 { background: #f3e5f5; color: #7b1fa2; }
.category-KH03 { background: #e0f2f1; color: #00695c; }
.category-RR01 { background: #ffebee; color: #d32f2f; }
.category-DT_KHKD1 { background: #e8eaf6; color: #3f51b5; }
.category-UNKNOWN { background: #f5f5f5; color: #757575; }

.detected-date {
  font-size: 0.85rem;
  color: #666;
  font-style: italic;
}

.btn-remove-file {
  background: #ff4444;
  color: white;
  border: none;
  width: 30px;
  height: 30px;
  border-radius: 50%;
  cursor: pointer;
  font-size: 16px;
  font-weight: bold;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
}

.btn-remove-file:hover {
  background: #cc0000;
  transform: scale(1.1);
}

.smart-upload-progress {
  margin-top: 20px;
  padding: 20px;
  background: #f8f9fa;
  border-radius: 8px;
  border: 1px solid #dee2e6;
}

.progress-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}

.progress-header h4 {
  margin: 0;
  color: #007bff;
}

.progress-text {
  font-weight: 600;
  color: #666;
}

.progress-bar-container {
  width: 100%;
  height: 8px;
  background: #e9ecef;
  border-radius: 4px;
  overflow: hidden;
  margin-bottom: 10px;
}

.progress-bar {
  height: 100%;
  background: linear-gradient(90deg, #007bff, #0056b3);
  transition: width 0.3s ease;
}

.current-file {
  margin: 0;
  color: #666;
  font-style: italic;
}

.smart-import-results {
  margin-top: 20px;
  padding: 20px;
  background: #f8f9fa;
  border-radius: 8px;
  border: 1px solid #dee2e6;
}

.smart-import-results h4 {
  margin-top: 0;
  margin-bottom: 15px;
  color: #333;
}

.results-summary {
  margin-bottom: 20px;
}

.result-stats {
  display: flex;
  gap: 20px;
  flex-wrap: wrap;
}

.stat {
  padding: 8px 12px;
  border-radius: 6px;
  font-weight: 600;
  font-size: 0.9rem;
}

.stat.success {
  background: #d4edda;
  color: #155724;
}

.stat.error {
  background: #f8d7da;
  color: #721c24;
}

.stat.total {
  background: #d1ecf1;
  color: #0c5460;
}

.results-detail {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.result-item {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  padding: 12px;
  border-radius: 6px;
  border: 1px solid #ddd;
}

.result-item.success {
  background: #d4edda;
  border-color: #c3e6cb;
}

.result-item.error {
  background: #f8d7da;
  border-color: #f5c6cb;
}

.result-status {
  font-size: 1.2rem;
  line-height: 1;
}

.result-info {
  flex: 1;
}

.result-info strong {
  display: block;
  margin-bottom: 5px;
  color: #333;
}

.success-details,
.error-details {
  display: flex;
  flex-direction: column;
  gap: 2px;
  font-size: 0.9rem;
}

.success-details span {
  color: #155724;
}

.error-message {
  color: #721c24;
  font-style: italic;
}

.btn-smart-upload {
  background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 8px;
}

.btn-smart-upload:hover:not(:disabled) {
  background: linear-gradient(135deg, #218838 0%, #1e7e34 100%);
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(40,167,69,0.3);
}

.btn-smart-upload:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none;
}
</style>
