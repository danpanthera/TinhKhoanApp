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
    <div class="data-types-section agribank-section">
      <div class="section-header agribank-header">
        <div class="header-content">
          <div class="agribank-logo-header"></div>
          <div class="header-text">
            <h2>📊 BẢNG QUẢN LÝ DỮ LIỆU NGHIỆP VỤ AGRIBANK</h2>
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
                <span class="update-text">{{ getDataTypeStats(key).lastUpdate || 'Chưa có dữ liệu' }}</span>
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
            <h2>KẾT QUẢ LỌC THEO NGÀY - AGRIBANK</h2>
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

    <!-- Confirmation Modal -->
    <div v-if="showConfirmationModal" class="modal-overlay" @click="cancelConfirmation">
      <div class="modal-content enhanced-confirmation-modal" @click.stop>
        <div class="modal-header">
          <h3>⚠️ Xác nhận thao tác</h3>
          <button @click="cancelConfirmation" class="modal-close">×</button>
        </div>
        <div class="modal-body">
          <div class="confirmation-icon-wrapper">
            <div class="confirmation-icon">⚠️</div>
          </div>
          <p class="confirmation-message">{{ confirmationMessage }}</p>
          <div v-if="existingImports.length > 0" class="existing-imports">
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
        <div class="modal-footer confirmation-footer">
          <button @click="cancelConfirmation" class="btn-cancel btn-large">Hủy</button>
          <button @click="confirmAction" class="btn-confirm btn-large" :style="{ backgroundColor: '#dc3545' }">
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
    'failed': '❌',
    'error': '❌',
    'processing': '⏳',
    'pending': '⏸️'
  }
  return iconMap[status] || '❓'
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
    playNotificationSound();
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
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
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

/* 🔍 Enhanced Agribank Filtered Results Design */
.agribank-filtered-section {
  background: linear-gradient(135deg, #f0fdf4 0%, #ecfdf5 100%);
  border-radius: 20px;
  padding: 28px;
  margin: 32px 0;
  box-shadow: 0 12px 30px rgba(16, 185, 129, 0.15);
  border: 3px solid #bbf7d0;
  position: relative;
  overflow: hidden;
}

.agribank-filtered-section::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 6px;
  background: linear-gradient(90deg, #10B981, #059669, #047857, #10B981);
  animation: shimmerTop 3s infinite;
}

.agribank-filtered-header {
  margin-bottom: 24px;
}

.agribank-filtered-content {
  display: flex;
  align-items: flex-start;
  gap: 20px;
  margin-bottom: 16px;
}

.agribank-search-icon {
  position: relative;
}

.search-icon-wrapper {
  width: 64px;
  height: 64px;
  background: linear-gradient(135deg, #10B981, #059669);
  border-radius: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 8px 20px rgba(16, 185, 129, 0.4);
  position: relative;
  z-index: 2;
}

.search-icon {
  font-size: 28px;
  color: white;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.search-glow {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 90px;
  height: 90px;
  background: radial-gradient(circle, rgba(16, 185, 129, 0.4), transparent);
  border-radius: 50%;
  animation: searchPulse 2s infinite;
}

@keyframes searchPulse {
  0%, 100% {
    transform: translate(-50%, -50%) scale(1);
    opacity: 0.7;
  }
  50% {
    transform: translate(-50%, -50%) scale(1.2);
    opacity: 0.3;
  }
}

.agribank-filtered-text h2 {
  margin: 0 0 8px 0;
  font-size: 1.8rem;
  font-weight: 800;
  background: linear-gradient(135deg, #047857, #10B981);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  letter-spacing: 0.5px;
}

.filtered-summary {
  color: #374151;
  font-size: 1.1rem;
  font-weight: 600;
  margin-bottom: 12px;
  line-height: 1.5;
}

.records-found {
  background: linear-gradient(135deg, #10B981, #059669);
  color: white;
  padding: 4px 12px;
  border-radius: 12px;
  font-weight: 700;
  box-shadow: 0 3px 8px rgba(16, 185, 129, 0.3);
}

.date-range {
  background: linear-gradient(135deg, #fef3c7, #fde68a);
  color: #92400e;
  padding: 3px 10px;
  border-radius: 8px;
  font-weight: 600;
  border: 1px solid #fbbf24;
}

.filter-stats {
  display: flex;
  gap: 16px;
  flex-wrap: wrap;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 6px;
  background: rgba(255, 255, 255, 0.8);
  padding: 8px 12px;
  border-radius: 12px;
  border: 2px solid #d1fae5;
  box-shadow: 0 3px 8px rgba(16, 185, 129, 0.1);
}

.stat-icon {
  font-size: 1.1rem;
}

.stat-label {
  font-weight: 600;
  color: #374151;
  font-size: 0.9rem;
}

.stat-value {
  font-weight: 800;
  color: #059669;
  font-family: 'Consolas', monospace;
  background: linear-gradient(135deg, #ecfdf5, #d1fae5);
  padding: 2px 8px;
  border-radius: 6px;
  border: 1px solid #bbf7d0;
}

.agribank-filter-stripe {
  height: 3px;
  background: linear-gradient(90deg, #10B981, #059669, transparent);
  border-radius: 2px;
  margin-top: 12px;
}

/* 📊 Enhanced Filtered Table */
.agribank-filtered-table {
  background: white;
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 8px 25px rgba(16, 185, 129, 0.12);
  border: 2px solid #d1fae5;
}

.agribank-filtered-thead {
  background: linear-gradient(135deg, #047857, #059669, #10B981);
  color: white;
}

.agribank-filtered-thead th {
  padding: 18px 14px;
  text-align: left;
  font-weight: 700;
  font-size: 0.95rem;
  letter-spacing: 0.03em;
  text-transform: uppercase;
  border-bottom: 4px solid #34d399;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
}

.col-datatype-filter { width: 15%; }
.col-filename { width: 25%; }
.col-statement { width: 12%; }
.col-import { width: 12%; }
.col-records-filter { width: 12%; }
.col-status { width: 12%; }
.col-actions-filter { width: 12%; }

.agribank-filtered-tbody .enhanced-filtered-row {
  border-bottom: 1px solid #f0fdf4;
  transition: all 0.3s ease;
  background: linear-gradient(135deg, #ffffff 0%, #fafffe 100%);
}

.agribank-filtered-tbody .enhanced-filtered-row:hover {
  background: linear-gradient(135deg, #ecfdf5, #d1fae5);
  transform: translateY(-2px);
  box-shadow: 0 6px 15px rgba(16, 185, 129, 0.15);
}

.agribank-filtered-tbody .enhanced-filtered-row:nth-child(even) {
  background: linear-gradient(135deg, #f9fffe 0%, #f0fdf4 100%);
}

.agribank-filtered-tbody .enhanced-filtered-row:nth-child(even):hover {
  background: linear-gradient(135deg, #ecfdf5, #d1fae5);
}

/* Enhanced Data Type Badge for Filtered */
.agribank-filtered-badge {
  display: flex !important;
  align-items: center !important;
  gap: 8px !important;
  padding: 8px 12px !important;
  border-radius: 12px !important;
  color: white !important;
  font-weight: 700 !important;
  font-size: 0.85rem !important;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2) !important;
  border: 2px solid rgba(255, 255, 255, 0.3) !important;
  min-width: 90px !important;
  justify-content: center !important;
}

.badge-icon {
  font-size: 1.1rem;
}

.badge-text {
  letter-spacing: 0.5px;
}

/* Enhanced Filename */
.filename-wrapper {
  display: flex;
  align-items: center;
  gap: 10px;
}

.file-icon {
  font-size: 1.3rem;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #f0fdf4, #ecfdf5);
  border-radius: 8px;
  border: 1px solid #d1fae5;
}

.agribank-filename {
  font-weight: 600;
  color: #374151;
  font-size: 0.9rem;
  max-width: 200px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

/* Enhanced Date Info */
.date-info {
  display: flex;
  align-items: center;
  gap: 8px;
}

.date-icon {
  font-size: 1rem;
  color: #059669;
}

.date-text {
  font-weight: 600;
  color: #374151;
  font-size: 0.85rem;
  font-family: 'Consolas', monospace;
}

/* Enhanced Records for Filtered */
.records-info-filter {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 4px;
  text-align: center;
}

.records-icon {
  font-size: 1.1rem;
  color: #059669;
}

.agribank-records {
  font-size: 1.1rem;
  font-weight: 800;
  color: #047857;
  font-family: 'Consolas', monospace;
  background: linear-gradient(135deg, #ecfdf5, #d1fae5);
  padding: 4px 8px;
  border-radius: 8px;
  border: 1px solid #bbf7d0;
}

.records-unit {
  font-size: 0.7rem;
  color: #6b7280;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  font-weight: 600;
}

/* Enhanced Status Badge */
.agribank-status-badge {
  display: flex !important;
  align-items: center !important;
  gap: 6px !important;
  padding: 6px 10px !important;
  border-radius: 12px !important;
  font-weight: 600 !important;
  font-size: 0.8rem !important;
  min-width: 80px !important;
  justify-content: center !important;
  box-shadow: 0 3px 8px rgba(0, 0, 0, 0.1) !important;
}

.status-icon {
  font-size: 1rem;
}

/* Enhanced Actions for Filtered */
.agribank-actions-group {
  display: flex;
  gap: 8px;
  justify-content: center;
}

.agribank-btn-preview {
  background: linear-gradient(135deg, #3b82f6, #2563eb) !important;
  color: white !important;
  border: none !important;
  width: 36px !important;
  height: 36px !important;
  border-radius: 10px !important;
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  font-size: 14px !important;
  box-shadow: 0 4px 10px rgba(59, 130, 246, 0.3) !important;
  transition: all 0.3s ease !important;
}

.agribank-btn-preview:hover {
  background: linear-gradient(135deg, #2563eb, #1d4ed8) !important;
  transform: translateY(-2px) !important;
  box-shadow: 0 6px 15px rgba(59, 130, 246, 0.4) !important;
}

.agribank-btn-delete-filter {
  background: linear-gradient(135deg, #ef4444, #dc2626) !important;
  color: white !important;
  border: none !important;
  width: 36px !important;
  height: 36px !important;
  border-radius: 10px !important;
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  font-size: 14px !important;
  box-shadow: 0 4px 10px rgba(239, 68, 68, 0.3) !important;
  transition: all 0.3s ease !important;
}

.agribank-btn-delete-filter:hover {
  background: linear-gradient(135deg, #dc2626, #b91c1c) !important;
  transform: translateY(-2px) !important;
  box-shadow: 0 6px 15px rgba(239, 68, 68, 0.4) !important;
}

/* 📱 Responsive for Filtered Table */
@media (max-width: 1024px) {
  .agribank-filtered-section {
    padding: 20px;
    margin: 24px 0;
  }
  
  .agribank-filtered-content {
    flex-direction: column;
    text-align: center;
    gap: 16px;
  }
  
  .search-icon-wrapper {
    width: 56px;
    height: 56px;
  }
  
  .search-icon {
    font-size: 24px;
  }
  
  .filter-stats {
    justify-content: center;
  }
  
  .agribank-filtered-thead th {
    padding: 14px 10px;
    font-size: 0.85rem;
  }
  
  .enhanced-filtered-row td {
    padding: 12px 8px;
  }
  
  .agribank-actions-group {
    gap: 6px;
  }
  
  .agribank-btn-preview,
  .agribank-btn-delete-filter {
    width: 32px !important;
    height: 32px !important;
    font-size: 12px !important;
  }
}

@media (max-width: 768px) {
  .agribank-filtered-text h2 {
    font-size: 1.5rem;
  }
  
  .filtered-summary {
    font-size: 1rem;
  }
  
  .stat-item {
    padding: 6px 10px;
  }
  
  .agribank-filename {
    max-width: 120px;
  }
  
  .date-text {
    font-size: 0.75rem;
  }
  
  .agribank-filtered-thead th {
    padding: 12px 6px;
    font-size: 0.8rem;
  }
  
  .enhanced-filtered-row td {
    padding: 10px 6px;
  }
}

/* 👁️ Enhanced Agribank Preview Modal Design */
.agribank-preview-overlay {
  background: rgba(0, 0, 0, 0.7);
  backdrop-filter: blur(8px);
}

.agribank-preview-modal {
  background: linear-gradient(135deg, #f0fdf4 0%, #ecfdf5 100%);
  border-radius: 20px;
  max-width: 95vw;
  max-height: 95vh;
  border: 3px solid #bbf7d0;
  box-shadow: 0 20px 60px rgba(16, 185, 129, 0.3);
  overflow: hidden;
  position: relative;
}

.agribank-preview-modal::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 6px;
  background: linear-gradient(90deg, #10B981, #059669, #047857, #10B981);
  animation: shimmerTop 3s infinite;
}

.agribank-preview-header {
  background: linear-gradient(135deg, #047857, #059669, #10B981);
  color: white;
  padding: 24px 28px;
  border-bottom: 4px solid #34d399;
  position: relative;
}

.agribank-preview-title {
  display: flex;
  align-items: center;
  gap: 20px;
  margin-bottom: 16px;
}

.preview-icon-wrapper {
  width: 64px;
  height: 64px;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.2), rgba(255, 255, 255, 0.1));
  border-radius: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.3);
  position: relative;
  z-index: 2;
}

.preview-icon {
  font-size: 28px;
  color: white;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.preview-glow {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 90px;
  height: 90px;
  background: radial-gradient(circle, rgba(255, 255, 255, 0.3), transparent);
  border-radius: 50%;
  animation: previewPulse 2s infinite;
}

@keyframes previewPulse {
  0%, 100% {
    transform: translate(-50%, -50%) scale(1);
    opacity: 0.7;
  }
  50% {
    transform: translate(-50%, -50%) scale(1.2);
    opacity: 0.3;
  }
}

.preview-title-text h3 {
  margin: 0 0 8px 0;
  font-size: 1.8rem;
  font-weight: 800;
  color: white;
  letter-spacing: 0.5px;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.preview-subtitle {
  color: rgba(255, 255, 255, 0.9);
  font-size: 1.1rem;
  font-weight: 600;
  margin: 0;
  background: rgba(255, 255, 255, 0.1);
  padding: 6px 12px;
  border-radius: 8px;
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.agribank-preview-stripe {
  height: 3px;
  background: linear-gradient(90deg, rgba(255, 255, 255, 0.8), rgba(255, 255, 255, 0.3), rgba(255, 255, 255, 0.8));
  border-radius: 2px;
}

.agribank-preview-close {
  background: rgba(255, 255, 255, 0.2) !important;
  border: 2px solid rgba(255, 255, 255, 0.3) !important;
  color: white !important;
  font-size: 24px !important;
  font-weight: bold !important;
  width: 40px !important;
  height: 40px !important;
  border-radius: 50% !important;
  transition: all 0.3s ease !important;
}

.agribank-preview-close:hover {
  background: rgba(255, 255, 255, 0.3) !important;
  transform: scale(1.1) !important;
}

.agribank-preview-body {
  padding: 28px;
  max-height: 70vh;
  overflow-y: auto;
}

/* Preview Info Section */
.agribank-preview-info {
  background: rgba(255, 255, 255, 0.8);
  border-radius: 16px;
  padding: 20px;
  margin-bottom: 24px;
  border: 2px solid #d1fae5;
  box-shadow: 0 8px 20px rgba(16, 185, 129, 0.1);
}

.agribank-info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 16px;
}

.agribank-info-item {
  display: flex;
  align-items: center;
  gap: 10px;
  background: linear-gradient(135deg, #ffffff, #f9fffe);
  padding: 12px 16px;
  border-radius: 12px;
  border: 2px solid #bbf7d0;
  box-shadow: 0 3px 8px rgba(16, 185, 129, 0.1);
}

.info-icon {
  font-size: 1.2rem;
  width: 24px;
  text-align: center;
}

.agribank-info-item label {
  font-weight: 600;
  color: #374151;
  font-size: 0.95rem;
  min-width: 80px;
}

.info-value {
  font-weight: 700;
  color: #059669;
  font-family: 'Consolas', monospace;
  background: linear-gradient(135deg, #ecfdf5, #d1fae5);
  padding: 4px 10px;
  border-radius: 8px;
  border: 1px solid #bbf7d0;
}

.info-value.highlight {
  background: linear-gradient(135deg, #10B981, #059669);
  color: white;
  box-shadow: 0 3px 8px rgba(16, 185, 129, 0.3);
}

.agribank-preview-badge {
  display: flex !important;
  align-items: center !important;
  gap: 8px !important;
  padding: 6px 12px !important;
  border-radius: 10px !important;
  color: white !important;
  font-weight: 700 !important;
  font-size: 0.85rem !important;
  box-shadow: 0 3px 8px rgba(0, 0, 0, 0.2) !important;
  border: 2px solid rgba(255, 255, 255, 0.3) !important;
  min-width: 80px !important;
  justify-content: center !important;
}

/* Preview Table Section */
.agribank-preview-table {
  background: white;
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 8px 25px rgba(16, 185, 129, 0.12);
  border: 2px solid #d1fae5;
}

.agribank-table-header {
  background: linear-gradient(135deg, #047857, #059669, #10B981);
  color: white;
  padding: 20px 24px;
  text-align: center;
}

.agribank-table-header h4 {
  margin: 0 0 8px 0;
  font-size: 1.4rem;
  font-weight: 800;
  letter-spacing: 0.5px;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.table-note {
  margin: 0;
  color: rgba(255, 255, 255, 0.9);
  font-size: 1rem;
  font-weight: 500;
}

.agribank-table-wrapper {
  max-height: 500px;
  overflow: auto;
  background: white;
}

.enhanced-preview-table {
  width: 100%;
  border-collapse: collapse;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
}

.agribank-preview-thead {
  background: linear-gradient(135deg, #047857, #059669, #10B981);
  color: white;
  position: sticky;
  top: 0;
  z-index: 10;
}

.preview-th {
  padding: 16px 12px !important;
  text-align: left !important;
  font-weight: 700 !important;
  font-size: 0.9rem !important;
  letter-spacing: 0.03em !important;
  text-transform: uppercase !important;
  border-bottom: 3px solid #34d399 !important;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.2) !important;
  border-right: 1px solid rgba(255, 255, 255, 0.2) !important;
}

.agribank-preview-tbody .enhanced-preview-row {
  border-bottom: 1px solid #f0fdf4;
  transition: all 0.3s ease;
  background: linear-gradient(135deg, #ffffff 0%, #fafffe 100%);
}

.agribank-preview-tbody .enhanced-preview-row:hover {
  background: linear-gradient(135deg, #ecfdf5, #d1fae5);
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(16, 185, 129, 0.15);
}

.agribank-preview-tbody .enhanced-preview-row:nth-child(even) {
  background: linear-gradient(135deg, #f9fffe 0%, #f0fdf4 100%);
}

.agribank-preview-tbody .enhanced-preview-row:nth-child(even):hover {
  background: linear-gradient(135deg, #ecfdf5, #d1fae5);
}

.preview-td {
  padding: 12px !important;
  color: #374151 !important;
  font-size: 0.9rem !important;
  border-right: 1px solid #f0fdf4 !important;
  font-family: 'Consolas', monospace !important;
  max-width: 200px !important;
  overflow: hidden !important;
  text-overflow: ellipsis !important;
  white-space: nowrap !important;
}

/* Preview Note */
.agribank-preview-note {
  background: linear-gradient(135deg, #fef3c7, #fde68a);
  color: #92400e;
  padding: 12px 20px;
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 0.95rem;
  font-weight: 600;
  border-top: 2px solid #fbbf24;
}

.note-icon {
  font-size: 1.1rem;
}

.note-text {
  flex: 1;
}

/* No Data States */
.agribank-no-data,
.agribank-no-preview {
  text-align: center;
  padding: 60px 20px;
  color: #6b7280;
  background: linear-gradient(135deg, #ffffff, #f9fffe);
  border-radius: 16px;
  border: 2px solid #d1fae5;
}

.agribank-empty-icon,
.no-data-icon {
  font-size: 4rem;
  margin-bottom: 16px;
  opacity: 0.7;
}

.agribank-no-preview h4 {
  color: #374151;
  margin: 0 0 8px 0;
  font-size: 1.2rem;
  font-weight: 600;
}

.agribank-no-preview p {
  color: #6b7280;
  margin: 0;
  font-size: 1rem;
}

/* Preview Footer */
.agribank-preview-footer {
  background: linear-gradient(135deg, #f0fdf4, #ecfdf5);
  border-top: 3px solid #bbf7d0;
  padding: 20px 28px;
  text-align: center;
}

.agribank-btn-close {
  background: linear-gradient(135deg, #059669, #10B981) !important;
  color: white !important;
  border: none !important;
  padding: 12px 32px !important;
  border-radius: 12px !important;
  font-weight: 700 !important;
  font-size: 1rem !important;
  cursor: pointer !important;
  transition: all 0.3s ease !important;
  box-shadow: 0 6px 15px rgba(16, 185, 129, 0.3) !important;
}

.agribank-btn-close:hover {
  background: linear-gradient(135deg, #047857, #059669) !important;
  transform: translateY(-2px) !important;
  box-shadow: 0 8px 20px rgba(16, 185, 129, 0.4) !important;
}

/* Responsive Design for Preview Modal */
@media (max-width: 1200px) {
  .agribank-preview-modal {
    max-width: 98vw;
  }
  
  .agribank-info-grid {
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  }
}

@media (max-width: 768px) {
  .agribank-preview-title {
    flex-direction: column;
    text-align: center;
    gap: 16px;
  }
  
  .preview-icon-wrapper {
    width: 56px;
    height: 56px;
  }
  
  .preview-icon {
    font-size: 24px;
  }
  
  .preview-title-text h3 {
    font-size: 1.5rem;
  }
  
  .agribank-info-grid {
    grid-template-columns: 1fr;
    gap: 12px;
  }
  
  .agribank-info-item {
    padding: 10px 12px;
  }
  
  .preview-th {
    padding: 12px 8px !important;
    font-size: 0.8rem !important;
  }
  
  .preview-td {
    padding: 10px 8px !important;
    font-size: 0.85rem !important;
    max-width: 120px !important;
  }
  
  .agribank-table-wrapper {
    max-height: 400px;
  }
}

/* 🏦 PREMIUM AGRIBANK MODAL STYLING */
/* Modal overlay cao cấp */
.agribank-modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.7);
  backdrop-filter: blur(8px);
  z-index: 2000;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
  animation: modalOverlayFadeIn 0.3s ease-out;
}

@keyframes modalOverlayFadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

/* Modal content cao cấp */
.agribank-premium-modal {
  background: white;
  border-radius: 20px;
  box-shadow: 0 25px 50px rgba(0, 0, 0, 0.25);
  width: 100%;
  max-width: 900px;
  max-height: 85vh; /* Đảm bảo không vượt quá chiều cao màn hình */
  overflow: hidden;
  animation: modalSlideIn 0.4s ease-out;
  position: relative;
  display: flex;
  flex-direction: column; /* Để footer luôn ở dưới cùng */
}

@keyframes modalSlideIn {
  from {
    opacity: 0;
    transform: translateY(-50px) scale(0.95);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

/* Header cao cấp với gradient và hiệu ứng */
.agribank-premium-header {
  position: relative;
  color: white;
  padding: 0;
  overflow: hidden;
}

.agribank-header-background {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(135deg, #8B1538 0%, #C41E3A 30%, #E63946 70%, #C41E3A 100%);
}

.agribank-gradient-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(45deg, rgba(255,255,255,0.1) 0%, transparent 50%, rgba(255,255,255,0.05) 100%);
}

.agribank-pattern-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-image: 
    radial-gradient(circle at 20% 20%, rgba(255,255,255,0.1) 1px, transparent 1px),
    radial-gradient(circle at 80% 80%, rgba(255,255,255,0.05) 1px, transparent 1px);
  background-size: 30px 30px;
}

.agribank-header-content {
  position: relative;
  z-index: 1;
  padding: 30px 40px;
}

/* Phần thương hiệu */
.agribank-brand-section {
  display: flex;
  align-items: center;
  gap: 20px;
  margin-bottom: 25px;
}

.agribank-logo-circle {
  position: relative;
  width: 70px;
  height: 70px;
  background: rgba(255,255,255,0.15);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 2px solid rgba(255,255,255,0.3);
}

.agribank-logo-icon {
  font-size: 32px;
  filter: drop-shadow(0 2px 4px rgba(0,0,0,0.3));
}

.agribank-logo-glow {
  position: absolute;
  top: -5px;
  left: -5px;
  right: -5px;
  bottom: -5px;
  border-radius: 50%;
  background: linear-gradient(45deg, rgba(255,255,255,0.3), transparent, rgba(255,255,255,0.1));
  animation: logoGlow 3s ease-in-out infinite;
}

@keyframes logoGlow {
  0%, 100% { opacity: 0.7; transform: scale(1); }
  50% { opacity: 1; transform: scale(1.05); }
}

.agribank-brand-text {
  flex: 1;
}

.agribank-title {
  font-size: 1.8rem;
  font-weight: 800;
  margin: 0 0 8px 0;
  text-shadow: 0 2px 4px rgba(0,0,0,0.3);
  letter-spacing: 0.5px;
}

.agribank-tagline {
  font-size: 0.95rem;
  margin: 0;
  opacity: 0.9;
  font-weight: 400;
}

/* Phần tiêu đề modal */
.modal-title-section {
  display: flex;
  align-items: center;
  gap: 25px;
}

.modal-icon-container {
  position: relative;
}

.modal-icon-circle {
  width: 80px;
  height: 80px;
  background: rgba(255,255,255,0.2);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 3px solid rgba(255,255,255,0.4);
  position: relative;
}

.modal-icon-large {
  font-size: 36px;
  filter: drop-shadow(0 2px 6px rgba(0,0,0,0.4));
}

.icon-pulse {
  position: absolute;
  top: -10px;
  left: -10px;
  right: -10px;
  bottom: -10px;
  border-radius: 50%;
  border: 2px solid rgba(255,255,255,0.5);
  animation: iconPulse 2s ease-in-out infinite;
}

@keyframes iconPulse {
  0%, 100% { opacity: 0; transform: scale(1); }
  50% { opacity: 1; transform: scale(1.1); }
}

.modal-title-content {
  flex: 1;
}

.modal-main-title {
  font-size: 1.6rem;
  font-weight: 700;
  margin: 0 0 8px 0;
  text-shadow: 0 2px 4px rgba(0,0,0,0.3);
}

.modal-data-type {
  font-size: 1.3rem;
  font-weight: 600;
  margin: 0 0 8px 0;
  color: rgba(255,255,255,0.95);
}

.modal-description {
  font-size: 1rem;
  margin: 0;
  opacity: 0.85;
  font-weight: 400;
}

/* Nút đóng cao cấp */
.agribank-close-button {
  position: absolute;
  top: 20px;
  right: 20px;
  width: 45px;
  height: 45px;
  background: rgba(255,255,255,0.15);
  border: 2px solid rgba(255,255,255,0.3);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
  z-index: 2;
}

.agribank-close-button:hover {
  background: rgba(255,255,255,0.25);
  border-color: rgba(255,255,255,0.5);
  transform: scale(1.05);
}

.close-icon {
  font-size: 20px;
  color: white;
  font-weight: bold;
}

.close-ripple {
  position: absolute;
  top: -5px;
  left: -5px;
  right: -5px;
  bottom: -5px;
  border-radius: 50%;
  border: 1px solid rgba(255,255,255,0.3);
  opacity: 0;
  animation: closeRipple 2s ease-in-out infinite;
}

@keyframes closeRipple {
  0% { opacity: 0; transform: scale(1); }
  50% { opacity: 0.7; transform: scale(1.2); }
  100% { opacity: 0; transform: scale(1.4); }
}

/* Thanh thương hiệu */
.agribank-brand-stripe {
  height: 8px;
  background: linear-gradient(90deg, 
    rgba(255,255,255,0.3) 0%,
    rgba(255,255,255,0.6) 25%,
    rgba(255,255,255,0.8) 50%,
    rgba(255,255,255,0.6) 75%,
    rgba(255,255,255,0.3) 100%
  );
  position: relative;
  overflow: hidden;
}

.stripe-pattern {
  position: absolute;
  top: 0;
  left: -100%;
  right: 0;
  bottom: 0;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.8), transparent);
  animation: stripeMove 3s linear infinite;
}

@keyframes stripeMove {
  0% { left: -100%; }
  100% { left: 100%; }
}

.stripe-glow {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(90deg, 
    rgba(255,255,255,0.1),
    rgba(255,255,255,0.3),
    rgba(255,255,255,0.1)
  );
}

/* Modal Body cao cấp */
.agribank-premium-body {
  padding: 30px 40px;
  background: #fafafa;
  flex: 1; /* Chiếm không gian còn lại */
  overflow-y: auto; /* Cho phép scroll khi nội dung quá dài */
  max-height: calc(85vh - 200px); /* Trừ đi chiều cao header và footer */
}

/* Custom scrollbar cho modal body */
.agribank-premium-body::-webkit-scrollbar {
  width: 8px;
}

.agribank-premium-body::-webkit-scrollbar-track {
  background: #f1f3f4;
  border-radius: 4px;
}

.agribank-premium-body::-webkit-scrollbar-thumb {
  background: linear-gradient(135deg, #8B1538, #C41E3A);
  border-radius: 4px;
}

.agribank-premium-body::-webkit-scrollbar-thumb:hover {
  background: linear-gradient(135deg, #C41E3A, #E63946);
}

/* Hướng dẫn nhanh */
.agribank-quick-guide {
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  border-radius: 15px;
  padding: 15px 20px; /* Giảm padding */
  margin-bottom: 20px; /* Giảm margin */
  border: 1px solid #dee2e6;
}

.guide-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 15px;
}

.guide-icon {
  font-size: 20px;
}

.guide-header h4 {
  margin: 0;
  color: #495057;
  font-weight: 600;
}

.guide-steps {
  display: flex;
  gap: 20px;
  flex-wrap: wrap;
}

.guide-step {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 15px;
  background: white;
  border-radius: 10px;
  border: 1px solid #e9ecef;
  flex: 1;
  min-width: 200px;
}

.step-number {
  width: 24px;
  height: 24px;
  background: linear-gradient(135deg, #8B1538, #C41E3A);
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: bold;
}

.step-text {
  font-size: 14px;
  color: #495057;
}

/* Form cao cấp */
.agribank-premium-form {
  background: white;
  border-radius: 15px;
  border: 1px solid #e9ecef;
  overflow: hidden;
}

.form-section {
  margin-bottom: 15px; /* Giảm khoảng cách giữa các section */
}

.form-section:last-child {
  margin-bottom: 0; /* Bỏ margin cho section cuối */
}

.agribank-upload-section {
  padding: 0;
}

.section-header {
  display: flex;
  align-items: center;
  gap: 15px;
  padding: 15px 20px; /* Giảm padding */
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  border-bottom: 1px solid #dee2e6;
}

.section-icon {
  font-size: 24px;
  color: #8B1538;
}

.section-title {
  flex: 1;
}

.section-title h4 {
  margin: 0 0 4px 0;
  font-weight: 600;
  color: #495057;
}

.section-subtitle {
  margin: 0;
  font-size: 14px;
  color: #6c757d;
}

.file-limit-badge {
  background: linear-gradient(135deg, #8B1538, #C41E3A);
  color: white;
  padding: 8px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 6px;
}

.limit-icon {
  font-size: 14px;
}

/* Khu vực upload cao cấp */
.agribank-upload-zone {
  padding: 30px 25px; /* Giảm padding */
  border: 3px dashed #dee2e6;
  background: #fafafa;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.agribank-upload-zone:hover {
  border-color: #8B1538;
  background: #f8f9fa;
}

.agribank-upload-zone.drag-active {
  border-color: #C41E3A;
  background: linear-gradient(135deg, rgba(196, 30, 58, 0.1), rgba(139, 21, 56, 0.05));
  transform: scale(1.02);
}

.agribank-upload-zone.has-files {
  border-style: solid;
  border-color: #28a745;
  background: linear-gradient(135deg, rgba(40, 167, 69, 0.1), rgba(40, 167, 69, 0.05));
}

/* Upload empty state */
.upload-empty-state {
  text-align: center;
  position: relative;
}

.upload-visual {
  position: relative;
  margin-bottom: 20px; /* Giảm margin */
}

.upload-icon-circle {
  width: 80px; /* Giảm kích thước */
  height: 80px;
  background: linear-gradient(135deg, #8B1538, #C41E3A);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 15px; /* Giảm margin */
  position: relative;
  box-shadow: 0 10px 30px rgba(139, 21, 56, 0.3);
}

.upload-main-icon {
  font-size: 32px; /* Giảm kích thước */
  color: white;
  filter: drop-shadow(0 2px 4px rgba(0,0,0,0.3));
}

.upload-icon-pulse {
  position: absolute;
  top: -10px;
  left: -10px;
  right: -10px;
  bottom: -10px;
  border-radius: 50%;
  border: 3px solid rgba(139, 21, 56, 0.3);
  animation: uploadPulse 2s ease-in-out infinite;
}

@keyframes uploadPulse {
  0%, 100% { opacity: 0; transform: scale(1); }
  50% { opacity: 1; transform: scale(1.1); }
}

.upload-arrows {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 150px;
  height: 150px;
}

.arrow {
  position: absolute;
  font-size: 20px;
  color: #8B1538;
  opacity: 0.6;
  animation: arrowFloat 3s ease-in-out infinite;
}

.arrow-1 { top: 20px; right: 20px; animation-delay: 0s; }
.arrow-2 { top: 20px; left: 20px; animation-delay: 0.5s; }
.arrow-3 { bottom: 20px; left: 20px; animation-delay: 1s; }
.arrow-4 { bottom: 20px; right: 20px; animation-delay: 1.5s; }

@keyframes arrowFloat {
  0%, 100% { opacity: 0.6; transform: translateY(0); }
  50% { opacity: 1; transform: translateY(-5px); }
}

.upload-content h3 {
  font-size: 1.4rem;
  font-weight: 600;
  color: #495057;
  margin: 0 0 8px 0;
}

.upload-description {
  font-size: 1rem;
  color: #6c757d;
  margin: 0 0 25px 0;
}

.format-support {
  background: white;
  border-radius: 15px;
  padding: 15px; /* Giảm padding */
  border: 1px solid #e9ecef;
  margin-bottom: 15px; /* Giảm margin */
}

.format-title {
  font-weight: 600;
  color: #495057;
  margin: 0 0 15px 0;
  font-size: 14px;
}

.format-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(100px, 1fr));
  gap: 10px;
}

.format-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 12px 8px;
  background: #f8f9fa;
  border-radius: 8px;
  border: 1px solid #e9ecef;
  transition: all 0.3s ease;
}

.format-item:hover {
  background: #e9ecef;
  transform: translateY(-2px);
}

.format-item.archive {
  background: linear-gradient(135deg, #ffc107, #ff8c00);
  color: white;
  border-color: #ff8c00;
}

.format-icon {
  font-size: 18px;
  margin-bottom: 4px;
}

.format-name {
  font-size: 12px;
  font-weight: 600;
}

/* Watermark */
.agribank-watermark {
  position: absolute;
  bottom: 20px;
  right: 20px;
  display: flex;
  flex-direction: column;
  align-items: center;
  opacity: 0.3;
}

.watermark-logo {
  font-size: 24px;
  margin-bottom: 4px;
}

.watermark-text {
  font-size: 10px;
  font-weight: bold;
  color: #8B1538;
  letter-spacing: 1px;
}

/* Upload has files */
.upload-has-files {
  display: flex;
  align-items: center;
  gap: 20px;
}

.files-summary {
  display: flex;
  align-items: center;
  gap: 20px;
  flex: 1;
}

.summary-icon-circle {
  width: 60px;
  height: 60px;
  background: linear-gradient(135deg, #28a745, #20c997);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
}

.summary-icon {
  font-size: 24px;
  color: white;
}

.summary-glow {
  position: absolute;
  top: -5px;
  left: -5px;
  right: -5px;
  bottom: -5px;
  border-radius: 50%;
  border: 2px solid rgba(40, 167, 69, 0.3);
  animation: summaryGlow 2s ease-in-out infinite;
}

@keyframes summaryGlow {
  0%, 100% { opacity: 0.7; transform: scale(1); }
  50% { opacity: 1; transform: scale(1.1); }
}

.summary-content h4 {
  font-size: 1.2rem;
  font-weight: 600;
  color: #495057;
  margin: 0 0 4px 0;
}

.summary-size {
  font-size: 14px;
  color: #28a745;
  font-weight: 500;
  margin: 0 0 4px 0;
}

.summary-action {
  font-size: 13px;
  color: #6c757d;
  margin: 0;
}

/* Selected Files Section */
.selected-files-section {
  background: white;
  border: 1px solid #e9ecef;
  border-radius: 15px;
  overflow: hidden;
  margin-bottom: 15px;
}

.btn-clear-all-files {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 12px;
  background: #f8f9fa;
  border: 1px solid #dee2e6;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 500;
  color: #6c757d;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-clear-all-files:hover {
  background: #e9ecef;
  color: #495057;
}

.files-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 15px;
  padding: 20px;
}

.file-card {
  background: #f8f9fa;
  border: 1px solid #e9ecef;
  border-radius: 12px;
  padding: 15px;
  position: relative;
  transition: all 0.3s ease;
}

.file-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
}

.file-preview {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 10px;
}

.file-icon-circle {
  width: 40px;
  height: 40px;
  background: linear-gradient(135deg, #6c757d, #495057);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 18px;
  color: white;
}

.file-icon-circle.archive {
  background: linear-gradient(135deg, #ffc107, #ff8c00);
}

.archive-badge {
  background: #ff8c00;
  color: white;
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 10px;
  font-weight: bold;
  display: flex;
  align-items: center;
  gap: 4px;
}

.file-details h5 {
  margin: 0 0 6px 0;
  font-size: 14px;
  font-weight: 600;
  color: #495057;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.file-meta {
  display: flex;
  align-items: center;
  gap: 8px;
}

.file-size {
  font-size: 12px;
  color: #6c757d;
}

.file-type-badge {
  background: #e9ecef;
  color: #495057;
  padding: 2px 6px;
  border-radius: 8px;
  font-size: 10px;
  font-weight: 500;
}

.btn-remove-file {
  position: absolute;
  top: 8px;
  right: 8px;
  width: 24px;
  height: 24px;
  background: #dc3545;
  color: white;
  border: none;
  border-radius: 50%;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: bold;
  transition: all 0.3s ease;
}

.btn-remove-file:hover {
  background: #c82333;
  transform: scale(1.1);
}

/* Password Section */
.password-section {
  background: white;
  border: 1px solid #e9ecef;
  border-radius: 15px;
  overflow: hidden;
  margin-bottom: 15px;
}

.password-content {
  padding: 20px;
}

.password-option {
  margin-bottom: 15px;
}

.premium-checkbox {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  cursor: pointer;
}

.premium-checkbox input[type="checkbox"] {
  margin: 0;
  width: 18px;
  height: 18px;
  accent-color: #8B1538;
}

.checkbox-mark {
  width: 18px;
  height: 18px;
  border: 2px solid #dee2e6;
  border-radius: 4px;
  position: relative;
  background: white;
  transition: all 0.3s ease;
}

.checkbox-content {
  flex: 1;
}

.checkbox-title {
  font-weight: 600;
  color: #495057;
  font-size: 14px;
  display: block;
  margin-bottom: 4px;
}

.checkbox-subtitle {
  font-size: 12px;
  color: #6c757d;
  font-family: monospace;
}

.password-input-group {
  position: relative;
  margin-bottom: 15px;
}

.premium-input {
  width: 100%;
  padding: 12px 50px 12px 15px;
  border: 2px solid #e9ecef;
  border-radius: 10px;
  font-size: 14px;
  background: white;
  transition: all 0.3s ease;
}

.premium-input:focus {
  border-color: #8B1538;
  outline: none;
  box-shadow: 0 0 0 3px rgba(139, 21, 56, 0.1);
}

.premium-input.has-default {
  border-color: #28a745;
  background: rgba(40, 167, 69, 0.05);
}

.btn-toggle-password {
  position: absolute;
  right: 12px;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.3s ease;
}

.btn-toggle-password:hover {
  background: #f8f9fa;
}

.password-hint {
  margin-top: 10px;
}

.hint-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  border-radius: 8px;
  font-size: 13px;
}

.hint-item.success {
  background: rgba(40, 167, 69, 0.1);
  color: #155724;
}

.hint-item.info {
  background: rgba(13, 110, 253, 0.1);
  color: #0c5460;
}

/* Notes Section */
.notes-section {
  background: white;
  border: 1px solid #e9ecef;
  border-radius: 15px;
  overflow: hidden;
}

.notes-content {
  padding: 20px;
}

.premium-textarea {
  width: 100%;
  padding: 12px 15px;
  border: 2px solid #e9ecef;
  border-radius: 10px;
  font-size: 14px;
  background: white;
  resize: vertical;
  min-height: 80px;
  font-family: inherit;
  transition: all 0.3s ease;
}

.premium-textarea:focus {
  border-color: #8B1538;
  outline: none;
  box-shadow: 0 0 0 3px rgba(139, 21, 56, 0.1);
}

.notes-counter {
  text-align: right;
  margin-top: 8px;
}

.counter-text {
  font-size: 12px;
  color: #6c757d;
}

/* Mobile responsive cho các section mới */
@media (max-width: 768px) {
  .files-grid {
    grid-template-columns: 1fr;
    padding: 15px;
    gap: 12px;
  }
  
  .file-card {
    padding: 12px;
  }
  
  .password-content, .notes-content {
    padding: 15px;
  }
  
  .premium-input, .premium-textarea {
    padding: 10px 12px;
    font-size: 14px;
  }
  
  .password-input-group .premium-input {
    padding-right: 45px;
  }
}

/* Modal Footer cao cấp */
.agribank-premium-footer {
  background: white;
  border-top: 1px solid #e9ecef;
  position: relative;
  overflow: hidden;
  flex-shrink: 0; /* Không cho footer bị co lại */
  margin-top: auto; /* Đẩy footer xuống dưới cùng */
}

.footer-background {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(135deg, #f8f9fa 0%, #ffffff 100%);
}

.footer-gradient {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 2px;
  background: linear-gradient(90deg, #8B1538, #C41E3A, #8B1538);
}

.footer-content {
  position: relative;
  z-index: 1;
  padding: 20px 30px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 20px;
}

.footer-info {
  display: flex;
  align-items: center;
  gap: 20px;
  flex: 1;
}

.info-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  background: rgba(139, 21, 56, 0.1);
  border-radius: 20px;
  font-size: 14px;
  font-weight: 500;
}

.info-icon {
  font-size: 16px;
}

.info-text {
  color: #495057;
}

.footer-actions {
  display: flex;
  align-items: center;
  gap: 15px;
}

/* Nút hủy cao cấp */
.agribank-btn-cancel {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 24px;
  background: #f8f9fa;
  color: #6c757d;
  border: 2px solid #dee2e6;
  border-radius: 25px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.agribank-btn-cancel:hover {
  background: #e9ecef;
  border-color: #adb5bd;
  color: #495057;
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(0,0,0,0.1);
}

/* Nút import cao cấp */
.agribank-btn-import {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 14px 30px;
  background: linear-gradient(135deg, #8B1538 0%, #C41E3A 50%, #E63946 100%);
  color: white;
  border: none;
  border-radius: 25px;
  font-weight: 700;
  font-size: 16px;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
  box-shadow: 0 8px 25px rgba(139, 21, 56, 0.3);
}

.agribank-btn-import:hover:not(:disabled) {
  transform: translateY(-3px);
  box-shadow: 0 12px 35px rgba(139, 21, 56, 0.4);
}

.agribank-btn-import:disabled {
  opacity: 0.7;
  cursor: not-allowed;
  transform: none;
}

.agribank-btn-import.btn-importing {
  background: linear-gradient(135deg, #6c757d 0%, #495057 100%);
}

.btn-icon {
  font-size: 18px;
  filter: drop-shadow(0 1px 2px rgba(0,0,0,0.3));
}

.btn-text {
  font-weight: 700;
  text-shadow: 0 1px 2px rgba(0,0,0,0.2);
}

/* Hiệu ứng shine cho nút import */
.btn-shine {
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.3), transparent);
  transition: left 0.6s ease;
}

.agribank-btn-import:hover .btn-shine {
  left: 100%;
}

/* Hiệu ứng glow */
.btn-glow {
  position: absolute;
  top: -2px;
  left: -2px;
  right: -2px;
  bottom: -2px;
  border-radius: 25px;
  background: linear-gradient(135deg, #8B1538, #C41E3A);
  z-index: -1;
  opacity: 0;
  transition: opacity 0.3s ease;
}

.agribank-btn-import:hover .btn-glow {
  opacity: 0.7;
  animation: btnGlow 2s ease-in-out infinite;
}

@keyframes btnGlow {
  0%, 100% { transform: scale(1); opacity: 0.7; }
  50% { transform: scale(1.05); opacity: 1; }
}

/* Ripple effect */
.btn-ripple {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 0;
  height: 0;
  border-radius: 50%;
  background: rgba(255,255,255,0.3);
  transform: translate(-50%, -50%);
  transition: width 0.3s ease, height 0.3s ease;
}

.agribank-btn-cancel:active .btn-ripple {
  width: 100px;
  height: 100px;
}

/* Footer progress */
.footer-progress {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  background: rgba(248, 249, 250, 0.95);
  backdrop-filter: blur(5px);
  padding: 15px 30px;
  border-top: 1px solid #e9ecef;
}

.progress-track {
  height: 6px;
  background: #e9ecef;
  border-radius: 3px;
  overflow: hidden;
  margin-bottom: 8px;
}

.progress-fill {
  height: 100%;
  background: linear-gradient(90deg, #8B1538, #C41E3A, #E63946);
  border-radius: 3px;
  transition: width 0.3s ease;
  position: relative;
  overflow: hidden;
}

.progress-shimmer {
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.4), transparent);
  animation: progressShimmer 2s linear infinite;
}

@keyframes progressShimmer {
  0% { left: -100%; }
  100% { left: 100%; }
}

.progress-text {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 12px;
  color: #6c757d;
  font-weight: 500;
}

/* Responsive */
@media (max-width: 768px) {
  .agribank-premium-modal {
    margin: 10px;
    max-width: calc(100vw - 20px);
    max-height: 95vh; /* Tăng chiều cao cho mobile */
  }
  
  .agribank-header-content {
    padding: 15px 20px; /* Giảm padding cho mobile */
  }
  
  .agribank-brand-section {
    flex-direction: column;
    text-align: center;
    gap: 10px; /* Giảm gap */
  }
  
  .modal-title-section {
    flex-direction: column;
    text-align: center;
    gap: 10px; /* Giảm gap */
  }
  
  .agribank-premium-body {
    padding: 15px 20px; /* Giảm padding cho mobile */
    max-height: calc(95vh - 180px); /* Điều chỉnh cho mobile */
  }
  
  .agribank-quick-guide {
    padding: 10px 15px; /* Giảm padding cho mobile */
    margin-bottom: 15px;
  }
  
  .guide-steps {
    flex-direction: column;
    gap: 10px; /* Giảm gap */
  }
  
  .guide-step {
    min-width: auto; /* Bỏ min-width cho mobile */
    padding: 8px 12px; /* Giảm padding */
  }
  
  .section-header {
    padding: 12px 15px; /* Giảm padding cho mobile */
  }
  
  .agribank-upload-zone {
    padding: 20px 15px; /* Giảm padding cho mobile */
  }
  
  .upload-icon-circle {
    width: 60px; /* Giảm kích thước cho mobile */
    height: 60px;
  }
  
  .upload-main-icon {
    font-size: 24px; /* Giảm kích thước cho mobile */
  }
  
  .footer-content {
    flex-direction: column;
    gap: 15px;
    padding: 15px 20px;
  }
  
  .footer-info {
    justify-content: center;
    flex-wrap: wrap;
    gap: 10px; /* Giảm gap */
  }
  
  .info-item {
    padding: 6px 10px; /* Giảm padding */
    font-size: 12px; /* Giảm font-size */
  }
  
  .footer-actions {
    width: 100%;
    justify-content: space-between;
  }
  
  .agribank-btn-cancel, .agribank-btn-import {
    flex: 1;
    justify-content: center;
    padding: 12px 20px; /* Giảm padding cho mobile */
    font-size: 14px; /* Giảm font-size */
  }
}

/* CSS cho các phần mới trong modal */
/* Selected Files Section */
.selected-files-section {
  margin-top: 20px;
  background: white;
  border-radius: 15px;
  border: 1px solid #e9ecef;
  overflow: hidden;
}

.selected-files-section .section-header {
  background: linear-gradient(135deg, #e3f2fd 0%, #f3e5f5 100%);
  border-bottom: 1px solid #e1bee7;
}

.btn-clear-all-files {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 8px 16px;
  background: linear-gradient(135deg, #ff5722, #ff7043);
  color: white;
  border: none;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-clear-all-files:hover {
  background: linear-gradient(135deg, #e64a19, #ff5722);
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(255, 87, 34, 0.3);
}

.files-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 15px;
  padding: 20px;
}

.file-card {
  background: #f8f9fa;
  border: 2px solid #e9ecef;
  border-radius: 12px;
  padding: 15px;
  transition: all 0.3s ease;
  position: relative;
  display: flex;
  align-items: center;
  gap: 12px;
}

.file-card:hover {
  border-color: #8B1538;
  background: white;
  box-shadow: 0 5px 15px rgba(139, 21, 56, 0.1);
  transform: translateY(-2px);
}

.file-preview {
  position: relative;
  flex-shrink: 0;
}

.file-icon-circle {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  background: linear-gradient(135deg, #6c757d, #495057);
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 20px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.15);
}

.file-icon-circle.archive {
  background: linear-gradient(135deg, #ffc107, #ff8c00);
}

.archive-badge {
  position: absolute;
  top: -8px;
  right: -8px;
  background: linear-gradient(135deg, #ffc107, #ff8c00);
  color: white;
  font-size: 10px;
  font-weight: bold;
  padding: 2px 6px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  gap: 2px;
  box-shadow: 0 2px 6px rgba(0,0,0,0.2);
}

.file-details {
  flex: 1;
  min-width: 0;
}

.file-details .file-name {
  font-weight: 600;
  color: #495057;
  margin: 0 0 6px 0;
  font-size: 14px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.file-meta {
  display: flex;
  align-items: center;
  gap: 8px;
}

.file-size {
  font-size: 12px;
  color: #6c757d;
  font-weight: 500;
}

.file-type-badge {
  background: linear-gradient(135deg, #8B1538, #C41E3A);
  color: white;
  font-size: 10px;
  font-weight: bold;
  padding: 2px 8px;
  border-radius: 10px;
  text-transform: uppercase;
}

.btn-remove-file {
  width: 30px;
  height: 30px;
  border-radius: 50%;
  background: linear-gradient(135deg, #dc3545, #c82333);
  color: white;
  border: none;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  transition: all 0.3s ease;
  flex-shrink: 0;
}

.btn-remove-file:hover {
  background: linear-gradient(135deg, #c82333, #bd2130);
  transform: scale(1.1);
  box-shadow: 0 4px 12px rgba(220, 53, 69, 0.3);
}

.remove-icon {
  font-size: 14px;
}

/* Password Section */
.password-section {
  margin-top: 20px;
}

.password-section .section-header {
  background: linear-gradient(135deg, #fff3cd 0%, #ffeaa7 100%);
  border-bottom: 1px solid #ffeaa7;
}

.password-content {
  padding: 20px;
}

.password-option {
  margin-bottom: 15px;
}

.premium-checkbox {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  cursor: pointer;
  padding: 15px;
  background: #f8f9fa;
  border-radius: 10px;
  border: 2px solid #e9ecef;
  transition: all 0.3s ease;
}

.premium-checkbox:hover {
  border-color: #8B1538;
  background: white;
}

.premium-checkbox input[type="checkbox"] {
  width: 20px;
  height: 20px;
  margin: 0;
  cursor: pointer;
}

.checkbox-mark {
  width: 20px;
  height: 20px;
  border: 2px solid #8B1538;
  border-radius: 4px;
  position: relative;
  background: white;
  cursor: pointer;
  transition: all 0.3s ease;
}

.premium-checkbox input[type="checkbox"]:checked + .checkbox-mark {
  background: linear-gradient(135deg, #8B1538, #C41E3A);
  border-color: #8B1538;
}

.premium-checkbox input[type="checkbox"]:checked + .checkbox-mark::after {
  content: '✓';
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  color: white;
  font-weight: bold;
  font-size: 14px;
}

.checkbox-content {
  flex: 1;
}

.checkbox-title {
  display: block;
  font-weight: 600;
  color: #495057;
  margin-bottom: 4px;
}

.checkbox-subtitle {
  display: block;
  font-size: 12px;
  color: #6c757d;
  font-style: italic;
}

.password-input-group {
  position: relative;
  margin-bottom: 15px;
}

.premium-input {
  width: 100%;
  padding: 15px 50px 15px 20px;
  border: 2px solid #e9ecef;
  border-radius: 10px;
  font-size: 14px;
  transition: all 0.3s ease;
  background: white;
}

.premium-input:focus {
  border-color: #8B1538;
  box-shadow: 0 0 0 3px rgba(139, 21, 56, 0.1);
  outline: none;
}

.premium-input.has-default {
  background: linear-gradient(135deg, #d4edda 0%, #c3e6cb 100%);
  border-color: #28a745;
}

.btn-toggle-password {
  position: absolute;
  right: 15px;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  cursor: pointer;
  padding: 5px;
  border-radius: 50%;
  transition: all 0.3s ease;
}

.btn-toggle-password:hover {
  background: rgba(139, 21, 56, 0.1);
}

.toggle-icon {
  font-size: 16px;
}

.password-hint {
  background: #f8f9fa;
  border-radius: 8px;
  padding: 12px;
}

.hint-item {
  display: flex;
  align-items: center;
  gap: 8px;
}

.hint-item.success {
  color: #155724;
}

.hint-item.info {
  color: #495057;
}

.hint-icon {
  font-size: 16px;
  flex-shrink: 0;
}

.hint-text {
  font-size: 13px;
  font-weight: 500;
}

/* Notes Section */
.notes-section {
  margin-top: 20px;
}

.notes-section .section-header {
  background: linear-gradient(135deg, #e8f5e8 0%, #c8e6c9 100%);
  border-bottom: 1px solid #c8e6c9;
}

.notes-content {
  padding: 20px;
}

.premium-textarea {
  width: 100%;
  padding: 15px;
  border: 2px solid #e9ecef;
  border-radius: 10px;
  font-size: 14px;
  font-family: inherit;
  resize: vertical;
  transition: all 0.3s ease;
  background: white;
  min-height: 80px;
}

.premium-textarea:focus {
  border-color: #8B1538;
  box-shadow: 0 0 0 3px rgba(139, 21, 56, 0.1);
  outline: none;
}

.notes-counter {
  display: flex;
  justify-content: flex-end;
  margin-top: 8px;
}

.counter-text {
  font-size: 12px;
  color: #6c757d;
  font-weight: 500;
}

/* Responsive cho mobile */
@media (max-width: 768px) {
  .files-grid {
    grid-template-columns: 1fr;
    gap: 10px;
    padding: 15px;
  }
  
  .file-card {
    padding: 12px;
  }
  
  .premium-checkbox {
    padding: 12px;
  }
  
  .password-content, .notes-content {
    padding: 15px;
  }
}
</style>
