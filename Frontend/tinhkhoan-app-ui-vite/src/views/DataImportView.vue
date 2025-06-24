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
                  title="Xem dữ liệu import ({{ getDataTypeStats(key).totalRecords }} records)"
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
                  �
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

    <!-- Filtered Results Section (when date filter is applied) - Enhanced Agribank Design -->
    <div v-if="filteredResults.length > 0" class="filtered-results-section agribank-filtered-section">
      <div class="section-header agribank-filtered-header">
        <div class="header-content agribank-filtered-content">
          <div class="agribank-search-icon">
            <div class="search-icon-wrapper">
              <span class="search-icon">🔍</span>
              <div class="search-glow"></div>
            </div>
          </div>
          <div class="header-text agribank-filtered-text">
            <h2>KẾT QUẢ LỌC THEO NGÀY</h2>
            <p class="filtered-summary">
              <span class="records-found">{{ filteredResults.length }} bản ghi</span> được tìm thấy từ 
              <span class="date-range">{{ formatDate(selectedFromDate) }}</span>
              <span v-if="selectedToDate" class="date-to"> đến <span class="date-range">{{ formatDate(selectedToDate) }}</span></span>
            </p>
            <div class="filter-stats">
              <div class="stat-item">
                <span class="stat-icon">📊</span>
                <span class="stat-label">Tổng:</span>
                <span class="stat-value">{{ filteredResults.length }}</span>
              </div>
              <div class="stat-item">
                <span class="stat-icon">📁</span>
                <span class="stat-label">Files:</span>
                <span class="stat-value">{{ getUniqueFilesCount() }}</span>
              </div>
              <div class="stat-item">
                <span class="stat-icon">🏦</span>
                <span class="stat-label">Loại:</span>
                <span class="stat-value">{{ getUniqueDataTypesCount() }}</span>
              </div>
            </div>
          </div>
        </div>
        <div class="agribank-filter-stripe"></div>
      </div>

      <div class="results-table agribank-filtered-table">
        <table class="enhanced-filtered-table">
          <thead class="agribank-filtered-thead">
            <tr>
              <th class="col-datatype-filter">Loại dữ liệu</th>
              <th class="col-filename">Tên file</th>
              <th class="col-statement">Ngày sao kê</th>
              <th class="col-import">Ngày import</th>
              <th class="col-records-filter">Records</th>
              <th class="col-status">Trạng thái</th>
              <th class="col-actions-filter">Thao tác</th>
            </tr>
          </thead>
          <tbody class="agribank-filtered-tbody">
            <tr v-for="item in paginatedFilteredResults" :key="item.id" class="filtered-row enhanced-filtered-row">
              <td class="col-datatype-filter">
                <div class="filtered-datatype-info">
                  <span 
                    class="data-type-badge agribank-filtered-badge" 
                    :style="{ backgroundColor: getDataTypeColor(item.dataType) }"
                  >
                    <span class="badge-icon">{{ getDataTypeIcon(item.dataType) }}</span>
                    <span class="badge-text">{{ item.dataType }}</span>
                  </span>
                </div>
              </td>
              <td class="col-filename filename-cell enhanced-filename">
                <div class="filename-wrapper">
                  <span class="file-icon">{{ getFileIcon(item.fileName) }}</span>
                  <span class="filename agribank-filename">{{ item.fileName }}</span>
                </div>
              </td>
              <td class="col-statement enhanced-date">
                <div class="date-info">
                  <span class="date-icon">📅</span>
                  <span class="date-text">{{ formatDate(item.statementDate) }}</span>
                </div>
              </td>
              <td class="col-import enhanced-date">
                <div class="date-info">
                  <span class="date-icon">⏰</span>
                  <span class="date-text">{{ formatDate(item.importDate) }}</span>
                </div>
              </td>
              <td class="col-records-filter records-cell enhanced-records-filter">
                <div class="records-info-filter">
                  <span class="records-icon">📊</span>
                  <span class="records-count agribank-records">{{ formatNumber(item.recordsCount) }}</span>
                  <span class="records-unit">records</span>
                </div>
              </td>
              <td class="col-status enhanced-status">
                <span 
                  class="status-badge agribank-status-badge" 
                  :class="getStatusClass(item.status)"
                >
                  <span class="status-icon">{{ getStatusIcon(item.status) }}</span>
                  <span class="status-text">{{ getStatusText(item.status) }}</span>
                </span>
              </td>
              <td class="col-actions-filter actions-cell enhanced-actions-filter">
                <div class="action-buttons-group agribank-actions-group">
                  <button 
                    @click="previewImport(item)" 
                    class="btn-action btn-preview agribank-btn-preview"
                    title="Xem trước dữ liệu"
                  >
                    👁️
                  </button>
                  <button 
                    @click="deleteImport(item)" 
                    class="btn-action btn-delete agribank-btn-delete-filter"
                    title="Xóa import này"
                  >
                    🗑️
                  </button>
                </div>
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

    <!-- Import Modal - Premium Agribank Design -->
    <div v-if="showImportModal" class="modal-overlay agribank-modal-overlay" @click="closeImportModal">
      <div class="modal-content agribank-premium-modal" @click.stop>
        <!-- Agribank Header với hiệu ứng gradient cao cấp -->
        <div class="modal-header agribank-premium-header">
          <div class="agribank-header-background">
            <div class="agribank-gradient-overlay"></div>
            <div class="agribank-pattern-overlay"></div>
          </div>
          
          <div class="agribank-header-content">
            <!-- Logo và thương hiệu -->
            <div class="agribank-brand-section">
              <div class="agribank-logo-circle">
                <div class="agribank-logo-icon">🏦</div>
                <div class="agribank-logo-glow"></div>
              </div>
              <div class="agribank-brand-text">
                <h1 class="agribank-title">AGRIBANK LAI CHÂU</h1>
                <p class="agribank-tagline">Ngân hàng Nông nghiệp và Phát triển Nông thôn</p>
              </div>
            </div>
            
            <!-- Tiêu đề modal -->
            <div class="modal-title-section">
              <div class="modal-icon-container">
                <div class="modal-icon-circle">
                  <span class="modal-icon-large">{{ dataTypeDefinitions[selectedDataType]?.icon }}</span>
                  <div class="icon-pulse"></div>
                </div>
              </div>
              <div class="modal-title-content">
                <h2 class="modal-main-title">IMPORT DỮ LIỆU NGHIỆP VỤ</h2>
                <h3 class="modal-data-type">{{ dataTypeDefinitions[selectedDataType]?.name }}</h3>
                <p class="modal-description">{{ dataTypeDefinitions[selectedDataType]?.description }}</p>
              </div>
            </div>
            
            <!-- Nút đóng cao cấp -->
            <button @click="closeImportModal" class="agribank-close-button">
              <span class="close-icon">✕</span>
              <div class="close-ripple"></div>
            </button>
          </div>
          
          <!-- Thanh thương hiệu Agribank -->
          <div class="agribank-brand-stripe">
            <div class="stripe-pattern"></div>
            <div class="stripe-glow"></div>
          </div>
        </div>

        <div class="modal-body agribank-modal-body">
          <div class="import-form agribank-form">
            <!-- Enhanced Agribank File Upload Area -->
            <div class="form-group agribank-form-group">
              <label class="form-label agribank-label">
                <span class="label-icon">📁</span>
                <span class="label-text">Chọn file để import</span>
                <span class="file-size-limit agribank-limit">Tối đa: 500MB mỗi file</span>
              </label>
              <div 
                class="upload-area agribank-upload-area"
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
                    <div class="upload-icon">�</div>
                    <h4 class="upload-title">Kéo thả file vào đây hoặc nhấp để chọn</h4>
                    <div class="supported-formats">
                      <p class="format-title">Định dạng hỗ trợ:</p>
                      <div class="format-list">
                        <span v-for="format in dataTypeDefinitions[selectedDataType]?.acceptedFormats" 
                              :key="format" 
                              class="format-badge agribank-badge">
                          {{ format }}
                        </span>
                        <span class="format-badge agribank-badge">ZIP</span>
                        <span class="format-badge agribank-badge">7Z</span>
                        <span class="format-badge agribank-badge">RAR</span>
                      </div>
                    </div>
                    <div class="agribank-logo-watermark"></div>
                  </div>
                  <div v-else class="upload-summary">
                    <div class="files-selected">
                      <span class="summary-icon">✅</span>
                      <span class="summary-text">
                        {{ selectedFiles.length }} file đã chọn
                        <span class="total-size">({{ formatTotalFileSize() }})</span>
                      </span>
                    </div>
                    <p class="click-to-add">Nhấp để thêm file hoặc kéo thả để bổ sung</p>
                    <div class="agribank-mini-logo"></div>
                  </div>
                </div>
              </div>
            </div>
            
            <!-- Danh sách file đã chọn -->
            <div v-if="selectedFiles.length > 0" class="selected-files-section">
              <div class="section-header">
                <div class="section-icon">📋</div>
                <div class="section-title">
                  <h4>File đã chọn ({{ selectedFiles.length }})</h4>
                  <p class="section-subtitle">Tổng dung lượng: {{ formatTotalFileSize() }}</p>
                </div>
                <button @click="clearAllFiles" class="btn-clear-all-files">
                  <span class="clear-icon">🗑️</span>
                  <span class="clear-text">Xóa tất cả</span>
                </button>
              </div>
              
              <div class="files-grid">
                <div v-for="(file, index) in selectedFiles" :key="index" class="file-card">
                  <div class="file-preview">
                    <div class="file-icon-circle" :class="{ 'archive': isArchiveFile(file.name) }">
                      <span class="file-icon">{{ getFileIcon(file.name) }}</span>
                    </div>
                    <div v-if="isArchiveFile(file.name)" class="archive-badge">
                      <span class="archive-icon">🗜️</span>
                      <span class="archive-text">NÉN</span>
                    </div>
                  </div>
                  
                  <div class="file-details">
                    <h5 class="file-name" :title="file.name">{{ file.name }}</h5>
                    <div class="file-meta">
                      <span class="file-size">{{ formatFileSize(file.size) }}</span>
                      <span class="file-type-badge">{{ getFileType(file.name) }}</span>
                    </div>
                  </div>
                  
                  <button @click="removeFile(index)" class="btn-remove-file" :title="`Xóa ${file.name}`">
                    <span class="remove-icon">✕</span>
                  </button>
                </div>
              </div>
            </div>
            
            <!-- Phần mật khẩu file nén -->
            <div v-if="hasArchiveFile" class="form-section password-section">
              <div class="section-header">
                <div class="section-icon">🔐</div>
                <div class="section-title">
                  <h4>Mật khẩu file nén</h4>
                  <p class="section-subtitle">Chỉ cần thiết nếu file có mật khẩu bảo vệ</p>
                </div>
              </div>
              
              <div class="password-content">
                <!-- Checkbox mật khẩu mặc định -->
                <div class="password-option">
                  <label class="premium-checkbox">
                    <input 
                      type="checkbox" 
                      v-model="useDefaultPassword"
                      @change="onDefaultPasswordToggle"
                    />
                    <span class="checkbox-mark"></span>
                    <div class="checkbox-content">
                      <span class="checkbox-title">🔑 Sử dụng mật khẩu mặc định hệ thống</span>
                      <span class="checkbox-subtitle">Snk6S4GV (được sử dụng cho hầu hết các file)</span>
                    </div>
                  </label>
                </div>
                
                <!-- Nhập mật khẩu -->
                <div class="password-input-group">
                  <input 
                    v-model="archivePassword" 
                    :type="showPassword ? 'text' : 'password'"
                    placeholder="Nhập mật khẩu file nén (để trống nếu không có)..."
                    class="premium-input password-input"
                    :class="{ 'has-default': useDefaultPassword }"
                  />
                  <button 
                    type="button" 
                    class="btn-toggle-password"
                    @click="togglePasswordVisibility"
                  >
                    <span class="toggle-icon">{{ showPassword ? '🙈' : '👁️' }}</span>
                  </button>
                </div>
                
                <div class="password-hint">
                  <div v-if="useDefaultPassword" class="hint-item success">
                    <span class="hint-icon">✅</span>
                    <span class="hint-text">Đang sử dụng mật khẩu mặc định. Có thể chỉnh sửa nếu cần.</span>
                  </div>
                  <div v-else class="hint-item info">
                    <span class="hint-icon">💡</span>
                    <span class="hint-text">Hệ thống sẽ thử giải nén không mật khẩu nếu để trống.</span>
                  </div>
                </div>
              </div>
            </div>
            
            <!-- Phần ghi chú -->
            <div class="form-section notes-section">
              <div class="section-header">
                <div class="section-icon">📝</div>
                <div class="section-title">
                  <h4>Ghi chú import</h4>
                  <p class="section-subtitle">Thêm mô tả hoặc ghi chú cho lần import này</p>
                </div>
              </div>
              
              <div class="notes-content">
                <textarea 
                  v-model="importNotes" 
                  placeholder="Ví dụ: Dữ liệu sao kê tháng 12/2024 từ chi nhánh Lai Châu..."
                  class="premium-textarea"
                  rows="3"
                ></textarea>
                <div class="notes-counter">
                  <span class="counter-text">{{ importNotes.length }}/500 ký tự</span>
                </div>
              </div>
            </div>
          </div>
          
          <!-- Progress section được di chuyển vào footer -->
          <!-- <div v-if="uploading" class="upload-progress-section enhanced-progress">
            ... phần progress cũ đã được di chuyển vào footer ...
          </div> -->
        </div>

        <!-- Modal Footer cao cấp -->
        <div class="modal-footer agribank-premium-footer">
          <div class="footer-background">
            <div class="footer-gradient"></div>
          </div>
          
          <div class="footer-content">
            <div class="footer-info">
              <div class="info-item">
                <span class="info-icon">📁</span>
                <span class="info-text">{{ selectedFiles.length }} file đã chọn</span>
              </div>
              <div v-if="selectedFiles.length > 0" class="info-item">
                <span class="info-icon">💾</span>
                <span class="info-text">{{ formatTotalFileSize() }}</span>
              </div>
              <div v-if="hasArchiveFile" class="info-item">
                <span class="info-icon">🔐</span>
                <span class="info-text">File nén được phát hiện</span>
              </div>
            </div>
            
            <div class="footer-actions">
              <button @click="closeImportModal" class="btn-cancel agribank-btn-cancel">
                <span class="btn-icon">✕</span>
                <span class="btn-text">Hủy bỏ</span>
                <div class="btn-ripple"></div>
              </button>
              
              <button 
                @click="performImport" 
                class="btn-import agribank-btn-import"
                :disabled="selectedFiles.length === 0 || uploading"
                :class="{ 'btn-importing': uploading }"
              >
                <span class="btn-icon">{{ uploading ? '⏳' : '🚀' }}</span>
                <span class="btn-text">
                  {{ uploading ? 'Đang Import...' : 'Bắt đầu Import' }}
                </span>
                <div class="btn-shine"></div>
                <div class="btn-glow"></div>
              </button>
            </div>
          </div>
          
          <!-- Progress bar trong footer khi đang upload -->
          <div v-if="uploading" class="footer-progress">
            <div class="progress-track">
              <div class="progress-fill" :style="{ width: uploadProgress + '%' }">
                <div class="progress-shimmer"></div>
              </div>
            </div>
            <div class="progress-text">
              <span>{{ uploadProgress }}% hoàn thành</span>
              <span v-if="remainingTime > 0">{{ remainingTimeFormatted }} còn lại</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Preview Modal - Enhanced Agribank Design -->
    <div v-if="showPreviewModal" class="modal-overlay agribank-preview-overlay" @click="closePreviewModal">
      <div class="modal-content modal-large agribank-preview-modal" @click.stop>
        <div class="modal-header agribank-preview-header">
          <div class="agribank-preview-title">
            <div class="preview-icon-wrapper">
              <span class="preview-icon">👁️</span>
              <div class="preview-glow"></div>
            </div>
            <div class="preview-title-text">
              <h3>XEM TRƯỚC DỮ LIỆU - AGRIBANK</h3>
              <p class="preview-subtitle">{{ selectedImport?.fileName }}</p>
            </div>
          </div>
          <div class="agribank-preview-stripe"></div>
          <button @click="closePreviewModal" class="modal-close agribank-preview-close">×</button>
        </div>

        <div class="modal-body agribank-preview-body">
          <div v-if="previewData && previewData.length > 0" class="preview-content agribank-preview-content">
            {{ console.log('🖼️ Modal rendering with data:', previewData.length, 'records') }}
            
            <!-- Preview Info Section - Agribank Style -->
            <div class="preview-info agribank-preview-info">
              <div class="info-grid agribank-info-grid">
                <div class="info-item agribank-info-item">
                  <span class="info-icon">📊</span>
                  <label>Loại dữ liệu:</label>
                  <span class="data-type-badge agribank-preview-badge" :style="{ backgroundColor: getDataTypeColor(selectedImport?.dataType) }">
                    <span class="badge-icon">{{ getDataTypeIcon(selectedImport?.dataType) }}</span>
                    <span class="badge-text">{{ selectedImport?.dataType }}</span>
                  </span>
                </div>
                <div class="info-item agribank-info-item">
                  <span class="info-icon">📅</span>
                  <label>Ngày sao kê:</label>
                  <span class="info-value">{{ formatDate(selectedImport?.statementDate) }}</span>
                </div>
                <div class="info-item agribank-info-item">
                  <span class="info-icon">⏰</span>
                  <label>Ngày import:</label>
                  <span class="info-value">{{ formatDate(selectedImport?.importDate) }}</span>
                </div>
                <div class="info-item agribank-info-item">
                  <span class="info-icon">🔢</span>
                  <label>Số records:</label>
                  <span class="info-value highlight">{{ formatNumber(previewData.length) }}</span>
                </div>
              </div>
            </div>

            <!-- Data Table - Agribank Style -->
            <div class="preview-table agribank-preview-table">
              <div class="preview-table-header agribank-table-header">
                <h4>📊 DỮ LIỆU MẪU - AGRIBANK</h4>
                <p class="table-note">Hiển thị tối đa 100 records đầu tiên</p>
              </div>
              <div class="table-wrapper agribank-table-wrapper">
                <table v-if="previewData.length > 0" class="enhanced-preview-table">
                  <thead class="agribank-preview-thead">
                    <tr>
                      <th v-for="(column, index) in Object.keys(previewData[0] || {})" :key="index" class="preview-th">
                        {{ column }}
                      </th>
                    </tr>
                  </thead>
                  <tbody class="agribank-preview-tbody">
                    <tr v-for="(record, index) in previewData.slice(0, 100)" :key="index" class="preview-row enhanced-preview-row">
                      <td v-for="(column, colIndex) in Object.keys(previewData[0] || {})" :key="colIndex" class="preview-td">
                        {{ record[column] || '-' }}
                      </td>
                    </tr>
                  </tbody>
                </table>
                <div v-else class="no-data agribank-no-data">
                  <div class="no-data-icon">📭</div>
                  <p>Không có dữ liệu để hiển thị</p>
                </div>
              </div>
              <div v-if="previewData.length > 100" class="preview-note agribank-preview-note">
                <span class="note-icon">💡</span>
                <span class="note-text">Chỉ hiển thị 100 records đầu tiên. Tổng cộng: <strong>{{ previewData.length }}</strong> records</span>
              </div>
            </div>
          </div>
          <div v-else class="no-preview-data agribank-no-preview">
            {{ console.log('📭 Modal showing no data. previewData:', previewData) }}
            <div class="empty-icon agribank-empty-icon">📭</div>
            <h4>Không có dữ liệu</h4>
            <p>Dữ liệu preview không khả dụng</p>
          </div>
        </div>

        <div class="modal-footer agribank-preview-footer">
          <button @click="closePreviewModal" class="btn-cancel agribank-btn-close">Đóng</button>
        </div>
      </div>
    </div>

    <!-- Confirmation Modal - Enhanced Agribank Style -->
    <div v-if="showConfirmationModal" class="modal-overlay agribank-confirmation-overlay" @click="cancelConfirmation">
      <div class="modal-content agribank-confirmation-modal" @click.stop>
        <div class="modal-header agribank-confirmation-header">
          <div class="agribank-header-background">
            <div class="agribank-gradient-overlay"></div>
          </div>
          <div class="confirmation-header-content">
            <div class="confirmation-title-section">
              <div class="confirmation-icon-wrapper">
                <div class="confirmation-icon">
                  <span v-if="confirmButtonText.includes('Xóa')">🗑️</span>
                  <span v-else>⚠️</span>
                </div>
                <div class="confirmation-icon-pulse"></div>
              </div>
              <div class="confirmation-title-content">
                <h3>XÁC NHẬN THAO TÁC</h3>
                <p class="confirmation-subtitle">Vui lòng xác nhận hành động của bạn</p>
              </div>
            </div>
            <button @click="cancelConfirmation" class="agribank-confirmation-close">
              <span class="close-icon">✕</span>
              <div class="close-ripple"></div>
            </button>
          </div>
          <div class="agribank-confirmation-stripe"></div>
        </div>
        <div class="modal-body agribank-confirmation-body">
          <div class="confirmation-content">
            <div class="confirmation-message-wrapper">
              <div class="confirmation-message-icon">
                <span v-if="confirmButtonText.includes('Xóa')">❗</span>
                <span v-else>ℹ️</span>
              </div>
              <p class="confirmation-message">{{ confirmationMessage }}</p>
            </div>
            <div v-if="existingImports.length > 0" class="existing-imports agribank-existing-imports">
              <h4>📋 Dữ liệu hiện có:</h4>
            <div class="existing-imports-list">
              <div v-for="imp in existingImports" :key="imp.id" class="existing-import-item">
                <span class="file-icon">{{ getFileIcon(imp.fileName) }}</span>
                <div class="existing-import-details">
                  <div class="existing-import-name">{{ imp.fileName }}</div>
                  <div class="existing-import-info">
                    <span class="records-badge">{{ formatNumber(imp.recordsCount) }} records</span>
                    <span class="date-badge">{{ formatDate(imp.importDate) }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="modal-footer agribank-confirmation-footer">
          <button @click="cancelConfirmation" class="btn-cancel agribank-btn-cancel">
            <span class="btn-icon">✖️</span>
            <span class="btn-text">Hủy bỏ</span>
          </button>
          <button 
            @click="confirmAction" 
            class="btn-confirm agribank-btn-confirm"
            :class="{ 'btn-delete': confirmButtonText.includes('Xóa') }"
          >
            <span class="btn-icon">{{ confirmButtonText.includes('Xóa') ? '🗑️' : '✓' }}</span>
            <span class="btn-text">{{ confirmButtonText }}</span>
            <div class="btn-shine"></div>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import rawDataService from '@/services/rawDataService'
// import { playNotificationSound } from '@/utils/soundUtils' // Temporarily commented out

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

// Xóa tất cả file đã chọn
const clearAllFiles = () => {
  selectedFiles.value = []
  archivePassword.value = ''
  useDefaultPassword.value = true
}

// Toggle hiển thị/ẩn mật khẩu
const togglePasswordVisibility = () => {
  showPassword.value = !showPassword.value
}

const getFileIcon = (fileName) => {
  const ext = fileName.split('.').pop().toLowerCase()
  const icons = {
    'xlsx': '📊', 'xls': '📊', 'csv': '📋', 
    'zip': '🗜️', '7z': '🗜️', 'rar': '🗜️'
  }
  return icons[ext] || '📄'
}

// Thêm các method tiện ích bị thiếu cho giao diện
const formatTotalFileSize = () => {
  // Tính tổng kích thước tất cả file đã chọn
  const totalBytes = selectedFiles.value.reduce((total, file) => total + file.size, 0)
  return formatFileSize(totalBytes)
}

const getCategoryName = (dataType) => {
  // Trả về tên danh mục dựa trên loại dữ liệu
  if (dataType.startsWith('D')) return 'Dữ liệu tiền gửi'
  if (dataType.startsWith('L')) return 'Dữ liệu cho vay'
  if (dataType.startsWith('R')) return 'Dữ liệu rủi ro'
  if (dataType.startsWith('G')) return 'Dữ liệu tài khoản'
  return 'Dữ liệu khác'
}

const formatRecordCount = (count) => {
  // Delegate to rawDataService để format số bản ghi
  return rawDataService.formatRecordCount(count)
}

// Thêm các method cho bảng lọc theo ngày
const getUniqueFilesCount = () => {
  // Đếm số file unique trong kết quả lọc
  const uniqueFiles = new Set(filteredResults.value.map(item => item.fileName))
  return uniqueFiles.size
}

const getUniqueDataTypesCount = () => {
  // Đếm số loại dữ liệu unique trong kết quả lọc
  const uniqueTypes = new Set(filteredResults.value.map(item => item.dataType))
  return uniqueTypes.size
}

const getDataTypeIcon = (dataType) => {
  // Lấy icon cho từng loại dữ liệu
  const iconMap = {
    'LN01': '💰', 'LN02': '🔄', 'LN03': '📊',
    'DP01': '🏦', 'EI01': '📱', 'GAHR26': '👥',
    'GL01': '✍️', 'DPDA': '💳', 'DB01': '📋',
    'KH03': '🏢', 'BC57': '📈'
  }
  return iconMap[dataType] || '📄'
}

const getStatusIcon = (status) => {
  // Lấy icon cho trạng thái
  const iconMap = {
    'completed': '✅',
    'success': '✅', 
    'hoàn thành': '✅',
    'failed': '❌',
    'error': '❌',
    'processing': '⏳',
    'pending': '⏸️'
  }
  return iconMap[status?.toLowerCase()] || '❓'
}

const getFileType = (fileName) => {
  // Lấy phần mở rộng của file
  const ext = fileName.split('.').pop()?.toLowerCase() || ''
  const typeMap = {
    'csv': 'CSV',
    'xlsx': 'Excel',
    'xls': 'Excel',
    'zip': 'ZIP',
    '7z': '7Z',
    'rar': 'RAR'
  }
  return typeMap[ext] || ext.toUpperCase()
}

// 🕒 Hàm format ngày giờ theo định dạng dd/mm/yyyy - hh:mm:ss
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

// Hàm phát âm thanh chuông to khi upload hoàn thành
const playLoudCompletionBell = () => {
  try {
    // Tạo AudioContext để phát âm thanh
    const audioContext = new (window.AudioContext || window.webkitAudioContext)();
    
    // Tạo âm thanh chuông to với 3 tiếng chuông
    const bellSounds = [
      { freq: 800, duration: 0.3, volume: 0.8 }, // Chuông 1
      { freq: 1000, duration: 0.3, volume: 0.9 }, // Chuông 2  
      { freq: 1200, duration: 0.5, volume: 1.0 }  // Chuông 3 (to nhất)
    ];
    
    let startTime = audioContext.currentTime;
    
    bellSounds.forEach((sound, index) => {
      // Tạo oscillator cho từng tiếng chuông
      const oscillator = audioContext.createOscillator();
      const gainNode = audioContext.createGain();
      
      // Kết nối: oscillator -> gainNode -> destination
      oscillator.connect(gainNode);
      gainNode.connect(audioContext.destination);
      
      // Cài đặt frequency và wave type
      oscillator.frequency.setValueAtTime(sound.freq, startTime);
      oscillator.type = 'sine'; // Âm thanh chuông mượt
      
      // Cài đặt volume envelope (fade in/out)
      gainNode.gain.setValueAtTime(0, startTime);
      gainNode.gain.linearRampToValueAtTime(sound.volume, startTime + 0.05);
      gainNode.gain.exponentialRampToValueAtTime(0.01, startTime + sound.duration);
      
      // Phát âm thanh
      oscillator.start(startTime);
      oscillator.stop(startTime + sound.duration);
      
      // Delay giữa các tiếng chuông
      startTime += sound.duration + 0.2;
    });
    
    console.log('🔔 Playing loud completion bell sound!');
  } catch (error) {
    console.warn('❌ Could not play completion bell:', error);
    // Fallback: sử dụng âm thanh có sẵn
    // playNotificationSound(); // Temporarily commented out
  }
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
      
      // Phát âm thanh chuông to khi upload hoàn tất
      playLoudCompletionBell()
      
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
    
    const result = await rawDataService.deleteImportRecord(importItem.id)
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
  left: 0;
  right: 0;
  bottom: 0;
  height: 100px;
  background: url('/Logo-Agribank-2.png') no-repeat right center;
  background-size: contain;
  opacity: 0.1;
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

/* 🏦 CONTROL PANEL - Date Controls với thương hiệu Agribank */
.control-panel {
  background: linear-gradient(135deg, #f8f9fa 0%, #ffffff 100%);
  border: 1px solid #e9ecef;
  border-radius: 15px;
  padding: 25px 30px;
  margin-bottom: 30px;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.05);
}

.date-control-section {
  margin-bottom: 25px;
}

.agribank-date-title {
  color: #8B1538; /* Màu đỏ thương hiệu Agribank */
  font-size: 1.4rem;
  font-weight: 700;
  margin: 0 0 20px 0;
  text-shadow: 0 1px 2px rgba(139, 21, 56, 0.1);
}

.date-controls-enhanced {
  display: flex;
  align-items: flex-end;
  gap: 25px;
  flex-wrap: wrap;
}

.date-range-group {
  display: flex;
  align-items: flex-end;
  gap: 20px;
  flex: 1;
  min-width: 300px;
}

.date-input-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.date-input-group label {
  font-weight: 600;
  color: #495057;
  font-size: 14px;
}

.agribank-date-input {
  padding: 12px 15px;
  border: 2px solid #e9ecef;
  border-radius: 10px;
  font-size: 14px;
  background: white;
  transition: all 0.3s ease;
  min-width: 150px;
}

.agribank-date-input:focus {
  border-color: #8B1538;
  outline: none;
  box-shadow: 0 0 0 3px rgba(139, 21, 56, 0.1);
}

.date-actions-group {
  display: flex;
  align-items: center;
  gap: 12px;
  flex-shrink: 0;
}

.agribank-btn-filter {
  background: linear-gradient(135deg, #8B1538 0%, #C41E3A 50%, #E63946 100%);
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 25px;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(139, 21, 56, 0.3);
  display: flex;
  align-items: center;
  gap: 8px;
}

.agribank-btn-filter:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(139, 21, 56, 0.4);
}

.agribank-btn-filter:disabled {
  background: #6c757d;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.agribank-btn-clear {
  background: #f8f9fa;
  color: #6c757d;
  border: 2px solid #e9ecef;
  padding: 12px 20px;
  border-radius: 25px;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 8px;
}

.agribank-btn-clear:hover {
  background: #e9ecef;
  border-color: #adb5bd;
  transform: translateY(-1px);
}

.bulk-actions-section {
  border-top: 1px solid #e9ecef;
  padding-top: 20px;
}

.bulk-actions-section h3 {
  color: #495057;
  font-size: 1.2rem;
  font-weight: 600;
  margin: 0 0 15px 0;
}

.bulk-actions {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
}

/* Responsive cho date controls */
@media (max-width: 768px) {
  .control-panel {
    padding: 20px;
  }
  
  .date-controls-enhanced {
    flex-direction: column;
    align-items: stretch;
    gap: 20px;
  }
  
  .date-range-group {
    flex-direction: column;
    min-width: auto;
    gap: 15px;
  }
  
  .date-actions-group {
    justify-content: stretch;
  }
  
  .agribank-btn-filter, .agribank-btn-clear {
    flex: 1;
    justify-content: center;
  }
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
  background-color: #ffebee;
  color: #c62828;
  border-color: #ffcdd2;
}

.btn-clear-all:hover {
  background-color: #ffcdd2;
}

.btn-refresh {
  background-color: #e3f2fd;
  color: #1565c0;
  border-color: #bbdefb;
}

.btn-refresh:hover {
  background-color: #bbdefb;
}

.btn-debug {
  background-color: #e8f5e9;
  color: #2e7d32;
  border-color: #c8e6c9;
}

.btn-debug:hover {
  background-color: #c8e6c9;
}

/* Nút icon chỉ hiển thị biểu tượng */
.btn-icon-only {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 0;
  font-size: 16px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  transition: all 0.3s;
}

.btn-icon-only:hover {
  transform: translateY(-3px);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.25);
}

.btn-icon-only:active {
  transform: translateY(-1px);
}

/* 🎨 Cải thiện nút thao tác responsive và icon-only */
.actions-cell {
  padding: 8px !important;
  white-space: nowrap;
  min-width: 200px;
}

.btn-action {
  margin: 2px;
  padding: 8px 12px;
  border: none;
  border-radius: 6px;
  font-size: 0.85rem;
  cursor: pointer;
  transition: all 0.2s ease;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 4px;
  min-width: 35px;
  height: 35px;
}

.btn-action.btn-icon-only {
  padding: 6px;
  min-width: 32px;
  height:  32px;
  font-size: 0.9rem;
  gap: 0;
}

.btn-action.btn-icon-only .emoji {
  font-size: 14px;
}

.btn-view {
  background-color: #3B82F6;
  color: white;
}

.btn-view:hover {
  background-color: #2563EB;
  transform: translateY(-1px);
}

.btn-raw-view {
  background-color: #8B5CF6;
  color: white;
}

.btn-raw-view:hover {
  background-color: #7C3AED;
  transform: translateY(-1px);
}

.btn-import {
  color: white;
  font-weight: 600;
}

.btn-import:hover {
  opacity: 0.9;
  transform: translateY(-1px);
}

.btn-delete {
  background-color: #EF4444;
  color: white;
}

.btn-delete:hover {
  background-color: #DC2626;
  transform: translateY(-1px);
}

.btn-delete:disabled {
  background-color: #9CA3AF;
  cursor: not-allowed;
  transform: none;
}

/* 🏦 Enhanced Agribank styling cho bảng dữ liệu */
.agribank-section {
  background: linear-gradient(135deg, #f8f9fa 0%, #e2e8f0 100%);
  border-radius: 16px;
  padding: 24px;
  margin-bottom: 24px;
  box-shadow: 0 10px 25px rgba(16, 185, 129, 0.1);
  border: 2px solid #e2e8f0;
  position: relative;
  overflow: hidden;
}

.agribank-section::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, #10B981, #059669);
}

.agribank-header {
  margin-bottom: 20px;
}

.header-content {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 12px;
}

.agribank-logo-header {
  width: 48px;
  height: 48px;
  background: linear-gradient(135deg, #10B981, #059669);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
}

.agribank-logo-header::after {
  content: '🏦';
  font-size: 24px;
  color: white;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.header-text h2 {
  margin: 0;
  font-size: 1.75rem;
  font-weight: 700;
  background: linear-gradient(135deg, #047857, #10B981);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.header-text p {
  margin: 4px 0 0 0;
  color: #64748b;
  font-size: 0.95rem;
  font-weight: 500;
}

.agribank-brand-line {
  height: 2px;
  background: linear-gradient(90deg, #10B981, #059669, transparent);
  border-radius: 1px;
  margin-top: 8px;
}

/* 📊 Enhanced table styling */
.agribank-table {
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
  border: 1px solid #e2e8f0;
}

.enhanced-table {
  width: 100%;
  border-collapse: collapse;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
}

.agribank-thead {
  background: linear-gradient(135deg, #047857, #059669);
  color: white;
}

.agribank-thead th {
  padding: 16px 12px;
  text-align: left;
  font-weight: 600;
  font-size: 0.9rem;
  letter-spacing: 0.025em;
  text-transform: uppercase;
  border-bottom: 3px solid #10B981;
}

.col-datatype { width: 20%; }
.col-description { width: 35%; }
.col-records { width: 15%; }
.col-updated { width: 15%; }
.col-actions { width: 15%; }

.agribank-tbody .enhanced-row {
  border-bottom: 1px solid #f1f5f9;
  transition: all 0.3s ease;
}

.agribank-tbody .enhanced-row:hover {
  background: linear-gradient(135deg, #f0fdf4, #ecfdf5);
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(16, 185, 129, 0.1);
}

.enhanced-datatype {
  display: flex;
  align-items: center;
  gap: 12px;
}

.agribank-icon {
  font-size: 1.5rem;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #f0fdf4, #dcfce7);
  border-radius: 10px;
  border: 2px solid #bbf7d0;
}

.datatype-details {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.datatype-name {
  font-size: 1rem;
  font-weight: 700;
  color: #047857;
  letter-spacing: 0.025em;
}

.datatype-category {
  font-size: 0.8rem;
  color: #6b7280;
  font-weight: 500;
}

.enhanced-description .description-text {
  color: #374151;
  font-weight: 500;
  line-height: 1.5;
}

.enhanced-records {
  text-align: center;
}

.records-info {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 2px;
}

.agribank-number {
  font-size: 1.1rem;
  font-weight: 700;
  color: #059669;
  font-family: 'Consolas', 'Monaco', monospace;
}

.records-label {
  font-size: 0.75rem;
  color: #6b7280;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.enhanced-lastupdate .update-text {
  color: #6b7280;
  font-size: 0.9rem;
  font-weight: 500;
}

/* 🎯 Enhanced action buttons */
.enhanced-actions {
  padding: 12px 8px !important;
}

.action-buttons-group {
  display: flex;
  gap: 6px;
  justify-content: center;
  flex-wrap: nowrap;
  min-width: 180px;
}

.agribank-btn-view {
  background: linear-gradient(135deg, #3b82f6, #2563eb) !important;
  color: white !important;
  border: none !important;
  box-shadow: 0 3px 8px rgba(59, 130, 246, 0.3) !important;
}

.agribank-btn-view:hover {
  background: linear-gradient(135deg, #2563eb, #1d4ed8) !important;
  transform: translateY(-2px) !important;
  box-shadow: 0 5px 12px rgba(59, 130, 246, 0.4) !important;
}

.agribank-btn-raw {
  background: linear-gradient(135deg, #8b5cf6, #7c3aed) !important;
  color: white !important;
  border: none !important;
  box-shadow: 0 3px 8px rgba(139, 92, 246, 0.3) !important;
}

.agribank-btn-raw:hover {
  background: linear-gradient(135deg, #7c3aed, #6d28d9) !important;
  transform: translateY(-2px) !important;
  box-shadow: 0 5px 12px rgba(139, 92, 246, 0.4) !important;
}

.agribank-btn-import {
  color: white !important;
  border: none !important;
  box-shadow: 0 3px 8px rgba(0, 0, 0, 0.2) !important;
  font-weight: 600 !important;
}

.agribank-btn-import:hover {
  transform: translateY(-2px) !important;
  box-shadow: 0 5px 12px rgba(0, 0, 0, 0.3) !important;
}

.agribank-btn-delete {
  background: linear-gradient(135deg, #ef4444, #dc2626) !important;
  color: white !important;
  border: none !important;
  box-shadow: 0 3px 8px rgba(239, 68, 68, 0.3) !important;
}

.agribank-btn-delete:hover {
  background: linear-gradient(135deg, #dc2626, #b91c1c) !important;
  transform: translateY(-2px) !important;
  box-shadow: 0 5px 12px rgba(239, 68, 68, 0.4) !important;
}

.agribank-btn-delete:disabled {
  background: #d1d5db !important;
  color: #9ca3af !important;
  cursor: not-allowed !important;
  transform: none !important;
  box-shadow: none !important;
}

/* 🚨 ENHANCED CONFIRMATION MODAL STYLING */
.agribank-confirmation-overlay {
  display: flex;
  align-items: center;
  justify-content: center;
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.7);
  z-index: 1000;
  backdrop-filter: blur(5px);
}

.agribank-confirmation-modal {
  width: 480px;
  max-width: 90%;
  background-color: white;
  border-radius: 15px;
  overflow: hidden;
  box-shadow: 0 15px 30px rgba(0, 0, 0, 0.3);
  animation: confirmationModalAppear 0.3s ease-out;
  transform: translateY(0);
  border: 1px solid rgba(139, 21, 56, 0.3);
}

@keyframes confirmationModalAppear {
  0% {
    opacity: 0;
    transform: translateY(30px);
  }
  100% {
    opacity: 1;
    transform: translateY(0);
  }
}

.agribank-confirmation-header {
  position: relative;
  padding: 0;
  overflow: hidden;
}

.agribank-header-background {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 1;
}

.agribank-gradient-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, #8B1538 0%, #C41E3A 50%, #8B1538 100%);
}

.confirmation-header-content {
  position: relative;
  z-index: 2;
  padding: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.confirmation-title-section {
  display: flex;
  align-items: center;
  gap: 15px;
}

.confirmation-icon-wrapper {
  position: relative;
  width: 50px;
  height: 50px;
}

.confirmation-icon {
  width: 50px;
  height: 50px;
  background-color: rgba(255, 255, 255, 0.2);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
  z-index: 2;
  position: relative;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.2);
}

.confirmation-icon-pulse {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  border-radius: 50%;
  background-color: rgba(255, 255, 255, 0.4);
  animation: pulse 2s infinite;
  z-index: 1;
}

@keyframes pulse {
  0% {
    transform: scale(0.95);
    opacity: 1;
  }
  70% {
    transform: scale(1.2);
    opacity: 0;
  }
  100% {
    transform: scale(0.95);
    opacity: 0;
  }
}

.confirmation-title-content {
  color: white;
}

.confirmation-title-content h3 {
  margin: 0;
  font-size: 20px;
  font-weight: 600;
  letter-spacing: 1px;
}

.confirmation-subtitle {
  margin: 5px 0 0;
  font-size: 14px;
  opacity: 0.8;
}

.agribank-confirmation-close {
  background: none;
  border: none;
  color: white;
  font-size: 24px;
  cursor: pointer;
  width: 30px;
  height: 30px;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: all 0.2s;
}

.agribank-confirmation-close:hover {
  background-color: rgba(255, 255, 255, 0.2);
}

.agribank-confirmation-close .close-ripple {
  position: absolute;
  width: 100%;
  height: 100%;
  border-radius: 50%;
  background-color: rgba(255, 255, 255, 0.3);
  transform: scale(0);
  transition: transform 0.3s;
}

.agribank-confirmation-close:hover .close-ripple {
  transform: scale(1.5);
  opacity: 0;
}

.agribank-confirmation-stripe {
  height: 6px;
  background: linear-gradient(90deg, #E10F30 0%, #BF053D 100%);
  position: relative;
  overflow: hidden;
}

.agribank-confirmation-stripe:after {
  content: '';
  position: absolute;
  top: 0;
  right: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.3), transparent);
  animation: shimmer 2s infinite;
}

@keyframes shimmer {
  0% {
    transform: translateX(-100%);
  }
  100% {
    transform: translateX(100%);
  }
}

.agribank-confirmation-body {
  padding: 20px;
}

.confirmation-content {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.confirmation-message-wrapper {
  display: flex;
  gap: 15px;
  align-items: flex-start;
  background-color: rgba(139, 21, 56, 0.05);
  padding: 15px;
  border-radius: 10px;
  border-left: 4px solid #C41E3A;
}

.confirmation-message-icon {
  font-size: 24px;
  flex-shrink: 0;
}

.confirmation-message {
  margin: 0;
  font-size: 16px;
  line-height: 1.5;
  color: #333;
}

.agribank-existing-imports {
  background-color: #f8f9fa;
  padding: 15px;
  border-radius: 10px;
  margin-top: 15px;
}

.agribank-existing-imports h4 {
  margin-top: 0;
  color: #8B1538;
  font-size: 16px;
  margin-bottom: 10px;
}

.agribank-confirmation-footer {
  display: flex;
  justify-content: flex-end;
  padding: 20px;
  gap: 15px;
  border-top: 1px solid rgba(0, 0, 0, 0.1);
  background-color: #f8f9fa;
}

.agribank-btn-cancel {
  background-color: #f1f2f3;
  color: #555;
  border: none;
  padding: 10px 20px;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 10px;
  transition: all 0.2s;
}

.agribank-btn-cancel:hover {
  background-color: #e5e5e5;
}

.agribank-btn-confirm {
  background: linear-gradient(135deg, #8B1538 0%, #C41E3A 100%);
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 10px;
  position: relative;
  overflow: hidden;
  transition: all 0.2s;
}

.agribank-btn-confirm.btn-delete {
  background: linear-gradient(135deg, #d32f2f 0%, #b71c1c 100%);
}

.agribank-btn-confirm:hover {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(139, 21, 56, 0.3);
}

.agribank-btn-confirm.btn-delete:hover {
  box-shadow: 0 5px 15px rgba(183, 28, 28, 0.3);
}

.agribank-btn-confirm .btn-shine {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent);
  transform: translateX(-100%);
}

.agribank-btn-confirm:hover .btn-shine {
  animation: shine 1.5s infinite;
}

@keyframes shine {
  0% {
    transform: translateX(-100%);
  }
  60% {
    transform: translateX(100%);
  }
  100% {
    transform: translateX(100%);
  }
}
</style>
