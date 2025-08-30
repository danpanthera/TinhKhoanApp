<template>
  <div class="raw-data-warehouse">
    <!-- Header Section -->
    <div class="header-section">
      <h1>🏦 KHO DỮ LIỆU THÔ</h1>
      <p class="subtitle">
        Hệ thống quản lý và import dữ liệu nghiệp vụ ngân hàng chuyên nghiệp
      </p>
    </div>

    <!-- Thông báo -->
    <div v-if="errorMessage" class="alert alert-error">
      <span class="alert-icon">⚠️</span>
      {{ errorMessage }}
      <button class="alert-close" @click="clearMessage">
        ×
      </button>
    </div>

    <div v-if="successMessage" class="alert alert-success">
      <span class="alert-icon">✅</span>
      {{ successMessage }}
      <button class="alert-close" @click="clearMessage">
        ×
      </button>
    </div>

    <!-- Loading indicator -->
    <div v-if="loading" class="loading-section">
      <div class="loading-spinner" />
      <p>{{ loadingMessage || 'Đang xử lý dữ liệu...' }}</p>
    </div>

    <!-- Control Panel -->
    <div class="control-panel">
      <div class="date-control-section">
        <h3 class="agribank-date-title">
          🗓️ Chọn ngày sao kê
        </h3>
        <div class="date-controls-enhanced">
          <div class="date-range-group">
            <div class="date-input-group">
              <label>Từ ngày:</label>
              <input v-model="selectedFromDate" type="date" class="date-input agribank-date-input">
            </div>
            <div class="date-input-group">
              <label>Đến ngày:</label>
              <input v-model="selectedToDate" type="date" class="date-input agribank-date-input">
            </div>
          </div>
          <div class="date-actions-group">
            <button class="btn-filter agribank-btn-filter" :disabled="!selectedFromDate" @click="applyDateFilter">
              🔍 Lọc theo ngày
            </button>
            <button class="btn-clear agribank-btn-clear" @click="clearDateFilter">
              🗑️ Xóa bộ lọc
            </button>
          </div>
        </div>
      </div>

      <div class="bulk-actions-section">
        <h3>⚡ Thao tác hàng loạt</h3>
        <div class="bulk-actions">
          <button class="btn-smart-import" :disabled="loading" @click="openSmartImportModal">
            🧠 Smart Import
          </button>
          <button class="btn-clear-all" :disabled="loading" @click="clearAllData">
            🗑️ Xóa toàn bộ dữ liệu
          </button>
          <button class="btn-refresh" :disabled="loading" @click="refreshAllData">
            🔄 Tải lại dữ liệu
          </button>
          <button
            class="btn-table-counts"
            :disabled="loading"
            title="Lấy số lượng records thực tế từ database"
            @click="loadTableRecordCounts"
          >
            📊 Real Counts
          </button>
          <button
            class="btn-debug"
            :disabled="loading"
            title="Debug: Force recalculate stats"
            @click="debugRecalculateStats"
          >
            🔧 Debug Stats
          </button>
        </div>
      </div>
    </div>

    <!-- Data Types List -->
    <div class="data-types-section agribank-section">
      <div class="section-header agribank-header">
        <div class="header-content">
          <div class="agribank-logo-header" />
          <div class="header-text">
            <h2>📊 BẢNG DỮ LIỆU THÔ</h2>
            <p>Theo dõi và quản lý tất cả loại dữ liệu của hệ thống Agribank Lai Châu</p>
          </div>
        </div>
        <div class="agribank-brand-line" />
      </div>

      <div class="data-types-table agribank-table">
        <table class="enhanced-table">
          <thead class="agribank-thead">
            <tr>
              <th class="col-datatype">
                Loại dữ liệu
              </th>
              <th class="col-description">
                Mô tả chi tiết
              </th>
              <th class="col-records">
                Tổng records
              </th>
              <th class="col-updated">
                Cập nhật cuối
              </th>
              <th class="col-actions">
                Thao tác nghiệp vụ
              </th>
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
                  <span class="records-count agribank-number">{{
                    formatRecordCount(getDataTypeStats(key).totalRecords)
                  }}</span>
                  <span class="records-label">bản ghi</span>
                </div>
              </td>
              <td class="col-updated last-update-cell enhanced-lastupdate">
                <span class="update-text">{{ formatDateTime(getDataTypeStats(key).lastUpdate) }}</span>
              </td>
              <td class="actions-cell">
                <button
                  class="btn-action btn-view btn-icon-only"
                  title="Xem dữ liệu import"
                  :disabled="false"
                  @click="viewDataType(key)"
                >
                  👁️
                </button>
                <button
                  class="btn-action btn-raw-view btn-icon-only"
                  title="Xem dữ liệu thô từ bảng"
                  :disabled="!selectedFromDate"
                  @click="viewRawDataFromTable(key)"
                >
                  📊
                </button>
                <button
                  class="btn-action btn-import btn-icon-only"
                  :style="{ backgroundColor: getDataTypeColor(key) }"
                  title="Import dữ liệu"
                  @click="openImportModal(key)"
                >
                  📤
                </button>
                <button
                  class="btn-action btn-delete btn-icon-only"
                  title="Xóa theo ngày đã chọn"
                  :disabled="!selectedFromDate || getDataTypeStats(key).totalRecords === 0"
                  @click="deleteDataTypeByDate(key)"
                >
                  🗑️
                </button>
                <button
                  class="btn-action btn-delete-all btn-icon-only"
                  title="Xóa toàn bộ dữ liệu bảng này"
                  :disabled="getDataTypeStats(key).totalRecords === 0"
                  @click="deleteAllDataType(key)"
                >
                  💥
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
            <div class="modal-icon">
              📤
            </div>
            <h3>Import dữ liệu {{ selectedDataType }}</h3>
          </div>
          <button class="modal-close" aria-label="Đóng" @click="closeImportModal">
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
                  id="file-upload"
                  ref="fileInput"
                  type="file"
                  multiple
                  class="file-input"
                  @change="handleFileSelect"
                >
                <label for="file-upload" class="file-input-label">
                  <span class="file-icon">📎</span>
                  <span>Chọn tệp</span>
                </label>
                <span class="file-selected-text">{{
                  selectedFiles.length > 0 ? `Đã chọn ${selectedFiles.length} tệp` : 'Chưa có tệp nào được chọn'
                }}</span>
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
                  <button class="btn-remove" title="Xóa file này" @click="removeFile(index)">
                    ×
                  </button>
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
                <div class="progress-bar" :style="{ width: `${uploadProgress}%` }" />
              </div>
              <div class="progress-details">
                <span class="progress-percentage">{{ uploadProgress }}%</span>
                <span v-if="currentUploadingFile && totalFiles > 0" class="progress-file-info">
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
              />
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button class="btn-cancel" @click="closeImportModal">
            <span class="btn-icon">✖️</span>
            <span>Hủy</span>
          </button>
          <button class="btn-submit" :disabled="selectedFiles.length === 0 || uploading" @click="performImport">
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
          <button class="modal-close" @click="closeDataViewModal">
            ×
          </button>
        </div>
        <div class="modal-body">
          <div v-if="filteredResults.length > 0" class="data-table-container">
            <!-- Show processed data if available -->
            <div
              v-if="filteredResults[0]?.isProcessedView && filteredResults[0]?.processedData"
              class="processed-data-section"
            >
              <div class="table-summary">
                <p>
                  <strong>📊 Dữ liệu đã xử lý từ {{ filteredResults[0].tableName }}</strong>
                </p>
                <p>Hiển thị {{ filteredResults[0].processedData.length }} bản ghi đã xử lý</p>
                <p class="data-source-info">
                  Nguồn: {{ filteredResults[0].dataSource }}
                </p>
              </div>

              <div class="responsive-table-wrapper">
                <table class="data-table enhanced-table">
                  <thead class="agribank-thead">
                    <tr>
                      <th style="width: 50px; text-align: center">
                        #
                      </th>
                      <th
                        v-for="(column, index) in Object.keys(filteredResults[0].processedData[0] || {}).slice(0, 10)"
                        :key="index"
                      >
                        {{ column }}
                      </th>
                    </tr>
                  </thead>
                  <tbody class="agribank-tbody">
                    <tr
                      v-for="(record, recordIndex) in filteredResults[0].processedData.slice(0, 50)"
                      :key="recordIndex"
                    >
                      <td style="text-align: center; font-weight: bold; color: #8b1538">
                        {{ recordIndex + 1 }}
                      </td>
                      <td v-for="(column, columnIndex) in Object.keys(record).slice(0, 10)" :key="columnIndex">
                        <span :title="record[column]">{{ formatCellValue(record[column]) }}</span>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>

              <div class="table-note">
                <p>
                  <i>💡 Hiển thị 10 cột đầu tiên và tối đa 50 bản ghi. Đây là dữ liệu đã xử lý và lưu trong bảng lịch
                    sử.</i>
                </p>
              </div>
            </div>

            <!-- Show direct preview data -->
            <div
              v-else-if="filteredResults[0]?.directPreview && filteredResults[0]?.previewData"
              class="direct-preview-section"
            >
              <div class="table-summary">
                <p>
                  <strong>🎯 Direct Preview từ {{ filteredResults[0].dataType }} Table</strong>
                </p>
                <p>
                  Hiển thị {{ filteredResults[0].previewData.rows?.length || 0 }} /
                  {{ filteredResults[0].recordCount }} bản ghi
                </p>
                <p class="data-source-info">
                  Nguồn: Trực tiếp từ DataTable (không qua Import Records)
                </p>
              </div>

              <div class="responsive-table-wrapper">
                <table class="data-table enhanced-table">
                  <thead class="agribank-thead">
                    <tr>
                      <th style="width: 50px; text-align: center">
                        #
                      </th>
                      <th
                        v-for="(column, index) in Object.keys(filteredResults[0].previewData.rows?.[0] || {}).slice(
                          0,
                          10
                        )"
                        :key="index"
                      >
                        {{ column }}
                      </th>
                    </tr>
                  </thead>
                  <tbody class="agribank-tbody">
                    <tr
                      v-for="(record, recordIndex) in (filteredResults[0].previewData.rows || []).slice(0, 50)"
                      :key="recordIndex"
                    >
                      <td style="text-align: center; font-weight: bold; color: #8b1538">
                        {{ recordIndex + 1 }}
                      </td>
                      <td v-for="(column, columnIndex) in Object.keys(record).slice(0, 10)" :key="columnIndex">
                        <span :title="record[column]">{{ formatCellValue(record[column]) }}</span>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>

              <div class="table-note">
                <p>
                  <i>🎯 Hiển thị 10 cột đầu tiên và tối đa 50 bản ghi từ Direct Preview. Đây là dữ liệu trực tiếp từ
                    DataTable.</i>
                </p>
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
                    <td>{{ item.FileName }}</td>
                    <td>{{ formatDateTime(item.ImportDate) }}</td>
                    <td class="agribank-number">
                      {{ formatRecordCount(item.RecordsCount) }}
                    </td>
                    <td>{{ item.Status }}</td>
                    <td>
                      <button class="btn-action btn-view" title="Xem chi tiết" @click="previewImportRecord(item.Id)">
                        👁️
                      </button>
                      <button
                        class="btn-action btn-delete"
                        title="Xóa bản ghi"
                        @click="confirmDelete(item.Id, item.FileName)"
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
          <button class="btn-cancel" @click="closeDataViewModal">
            Đóng
          </button>
        </div>
      </div>
    </div>

    <!-- Modal hiển thị dữ liệu thô -->
    <div v-if="showRawDataModal" class="modal-overlay" @click="closeRawDataModal">
      <div class="modal-content raw-data-modal" @click.stop>
        <div class="modal-header">
          <h3>📊 Chi tiết dữ liệu {{ selectedDataType }}</h3>
          <button class="modal-close" @click="closeRawDataModal">
            ×
          </button>
        </div>
        <div class="modal-body">
          <div v-if="rawDataRecords.length > 0" class="raw-data-table-container">
            <div class="table-summary">
              <p>
                <strong>📋 Hiển thị {{ rawDataRecords.length }} bản ghi đầu tiên</strong> (tối đa 20 bản ghi để đảm bảo
                hiệu năng)
              </p>
            </div>
            <div class="responsive-table-wrapper">
              <table class="raw-data-table enhanced-table">
                <thead class="agribank-thead">
                  <tr>
                    <th style="width: 50px; text-align: center">
                      #
                    </th>
                    <th v-for="(column, index) in Object.keys(rawDataRecords[0]).slice(0, 12)" :key="index">
                      {{ column }}
                    </th>
                  </tr>
                </thead>
                <tbody class="agribank-tbody">
                  <tr v-for="(record, recordIndex) in rawDataRecords" :key="recordIndex">
                    <td style="text-align: center; font-weight: bold; color: #8b1538">
                      {{ recordIndex + 1 }}
                    </td>
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
          <button class="btn-cancel" @click="closeRawDataModal">
            Đóng
          </button>
          <button v-if="rawDataRecords.length > 0" class="btn-export" @click="exportRawData">
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
          <button class="modal-close" @click="closeSmartImportModal">
            ×
          </button>
        </div>
        <div class="modal-body">
          <div class="smart-import-info">
            <div class="feature-highlights">
              <h4>✨ Tính năng Smart Import (ĐÃ TỐI ƯU):</h4>
              <ul>
                <li>🔍 <strong>Tự động phân loại</strong> file dựa trên tên file</li>
                <li>📅 <strong>Tự động extract</strong> ngày dữ liệu từ filename (pattern: *yyyymmdd.csv*)</li>
                <li>🎯 <strong>Import trực tiếp</strong> vào đúng bảng dữ liệu thô</li>
                <li>⚡ <strong>Xử lý hàng loạt</strong> nhiều file cùng lúc</li>
                <li>🚀 <strong>NHANH HƠN 5X</strong> với batch upload & parallel processing</li>
                <li>📊 <strong>Báo cáo chi tiết</strong> kết quả import</li>
              </ul>
              <div class="optimization-badge">
                <span class="badge-text">🚀 OPTIMIZED</span>
                <small>Giảm thời gian từ 5 phút xuống dưới 1 phút</small>
              </div>
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
              >
              <small class="date-help">💡 Để trống để hệ thống tự động extract từ tên file</small>
            </div>

            <div
              class="file-drop-area"
              :class="{
                'drag-over': isDragOver,
                'has-files': smartSelectedFiles.length > 0,
              }"
              @dragover.prevent="handleDragOver"
              @dragenter.prevent="handleDragEnter"
              @dragleave.prevent="handleDragLeave"
              @drop.prevent="handleSmartFileDrop"
            >
              <!-- Main Drop Zone -->
              <div class="drop-zone-main">
                <div class="upload-icon-container">
                  <div class="upload-icon-wrapper">
                    <svg
                      class="upload-icon"
                      viewBox="0 0 24 24"
                      fill="none"
                      xmlns="http://www.w3.org/2000/svg"
                    >
                      <path d="M7 16H17L12 11L7 16Z" fill="currentColor" />
                      <path
                        d="M12 4V11"
                        stroke="currentColor"
                        stroke-width="2"
                        stroke-linecap="round"
                      />
                      <path
                        d="M4 20H20"
                        stroke="currentColor"
                        stroke-width="2"
                        stroke-linecap="round"
                      />
                    </svg>
                  </div>
                  <div class="upload-sparkles">
                    <div class="sparkle sparkle-1">
                      ✨
                    </div>
                    <div class="sparkle sparkle-2">
                      �
                    </div>
                    <div class="sparkle sparkle-3">
                      ⭐
                    </div>
                  </div>
                </div>

                <div class="drop-content">
                  <h3 class="drop-title">
                    <span v-if="!isDragOver">Kéo thả file vào đây</span>
                    <span v-else class="drag-active">🎯 Thả file ngay bây giờ!</span>
                  </h3>

                  <p class="drop-subtitle">
                    Hỗ trợ file CSV, XLSX với kích thước tối đa <strong>2GB</strong>
                  </p>

                  <div class="upload-divider">
                    <span>hoặc</span>
                  </div>

                  <button type="button" class="btn-select-files" @click="$refs.smartFileInput.click()">
                    <svg class="btn-icon" viewBox="0 0 24 24" fill="none">
                      <path
                        d="M12 15L12 2"
                        stroke="currentColor"
                        stroke-width="2"
                        stroke-linecap="round"
                      />
                      <path
                        d="M8 6L12 2L16 6"
                        stroke="currentColor"
                        stroke-width="2"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                      />
                      <path
                        d="M2 17H22V19C22 20.1046 21.1046 21 20 21H4C2.89543 21 2 20.1046 2 19V17Z"
                        fill="currentColor"
                      />
                    </svg>
                    <span>Chọn file từ máy tính</span>
                  </button>

                  <input
                    ref="smartFileInput"
                    type="file"
                    multiple
                    accept=".csv,.xlsx,.xls"
                    style="display: none"
                    @change="handleSmartFileSelect"
                  >
                </div>
              </div>

              <!-- File Types Support Info -->
              <div class="supported-formats">
                <div class="format-item">
                  <span class="format-icon">📊</span>
                  <span>CSV</span>
                </div>
                <div class="format-item">
                  <span class="format-icon">📈</span>
                  <span>XLSX</span>
                </div>
                <div class="format-item">
                  <span class="format-icon">📋</span>
                  <span>XLS</span>
                </div>
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
                  <button class="btn-remove-file" @click="removeSmartFile(index)">
                    ×
                  </button>
                </div>
              </div>
            </div>

            <div v-if="smartUploading" class="smart-upload-progress">
              <div class="progress-header">
                <h4>🚀 Đang xử lý Smart Import (Parallel)...</h4>
                <div class="progress-info">
                  <span class="progress-text">{{ smartUploadProgress.current }}/{{ smartUploadProgress.total }} file</span>
                  <span class="progress-percentage">{{ smartUploadProgress.percentage }}%</span>
                </div>
              </div>
              <div class="progress-bar-container">
                <div class="progress-bar" :style="{ width: smartUploadProgress.percentage + '%' }">
                  <span class="progress-bar-text">{{ smartUploadProgress.percentage }}%</span>
                </div>
              </div>
              <div class="current-file-info">
                <p class="current-file">
                  📤 {{ smartUploadProgress.currentFile }}
                </p>
                <p v-if="smartUploadProgress.stage" class="upload-stage">
                  Status:
                  {{
                    smartUploadProgress.stage === 'uploading'
                      ? '⬆️ Đang upload'
                      : smartUploadProgress.stage === 'completed'
                        ? '✅ Hoàn thành'
                        : smartUploadProgress.stage
                  }}
                </p>
                <p v-if="smartUploadProgress.activeFiles > 0" class="parallel-info">
                  🔥 {{ smartUploadProgress.activeFiles }} file đang upload đồng thời
                </p>
              </div>
            </div>

            <div v-if="smartImportResults && smartImportResults.results" class="smart-import-results">
              <h4>📊 Kết quả Smart Import:</h4>
              <div class="results-summary">
                <div class="result-stats">
                  <span class="stat success">✅ Thành công: {{ smartImportResults.successCount }}</span>
                  <span class="stat error">❌ Lỗi: {{ smartImportResults.failureCount }}</span>
                  <span class="stat total">📁 Tổng: {{ smartImportResults.totalFiles }}</span>
                  <span v-if="smartImportResults.uploadMethod" class="stat method">
                    🚀 Method:
                    {{
                      smartImportResults.uploadMethod === 'parallel'
                        ? `Parallel (${smartImportResults.maxConcurrency} concurrent)`
                        : 'Sequential'
                    }}
                  </span>
                </div>
              </div>
              <div class="results-detail">
                <div
                  v-for="result in smartImportResults.results"
                  :key="result.index"
                  class="result-item"
                  :class="{ success: result.success, error: !result.success }"
                >
                  <div class="result-status">
                    {{ result.success ? '✅' : '❌' }}
                  </div>
                  <div class="result-info">
                    <strong>{{ result.fileName }}</strong>
                    <div v-if="result.success" class="success-details">
                      <span>Category: {{ result.result?.DataType || result.result?.detectedCategory || 'N/A' }}</span>
                      <span>Records:
                        {{
                          formatNumber(result.result?.ProcessedRecords || result.result?.importedRecords || 0, 0)
                        }}</span>
                      <span v-if="result.result?.Duration">Time: {{ result.result.Duration }}</span>
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
          <button class="btn-cancel" :disabled="smartUploading" @click="closeSmartImportModal">
            Đóng
          </button>
          <button
            class="btn-smart-upload"
            :disabled="smartSelectedFiles.length === 0 || smartUploading"
            @click="startSmartImport"
          >
            🚀 Bắt đầu Smart Import
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, ref } from 'vue'
import api from '../services/api.js'; // ✅ Import api để sử dụng trong fallback strategy
import audioService from '../services/audioService.js'
import rawDataService from '../services/rawDataService.js'
import smartImportService from '../services/smartImportService.js'
import { formatFileSize, formatNumber } from '../utils/numberFormatter.js'

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
  currentFile: '',
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
  // 🔧 CUSTOM SORT: GL02 ngay sau GL01 theo thứ tự vần ABC
  const customOrder = ['DP01', 'DPDA', 'EI01', 'GL01', 'GL02', 'GL41', 'LN01', 'LN03', 'RR01']

  // Add items in custom order first
  customOrder.forEach(key => {
    if (dataTypeDefinitions[key]) {
      sorted[key] = dataTypeDefinitions[key]
    }
  })

  // Add any remaining items not in custom order (alphabetically)
  const remainingKeys = Object.keys(dataTypeDefinitions)
    .filter(key => !customOrder.includes(key))
    .sort()

  remainingKeys.forEach(key => {
    sorted[key] = dataTypeDefinitions[key]
  })

  return sorted
})

// Methods
const clearMessage = () => {
  errorMessage.value = ''
  successMessage.value = ''
}

const showError = message => {
  errorMessage.value = message
  console.error('❌ Error message:', message)
  setTimeout(() => {
    errorMessage.value = ''
  }, 5000)
}

const showDetailedError = (mainMessage, error) => {
  // Hiển thị thông báo lỗi chi tiết hơn để dễ dàng debug
  console.error('❌ Detailed Error:', mainMessage)
  console.error('❌ Error Object:', error)
  console.error('❌ Error Details:', {
    errorType: typeof error,
    errorMessage: error?.message,
    errorResponse: error?.response,
    errorData: error?.response?.data,
    errorStatus: error?.response?.status,
    errorCode: error?.code,
    // Serialize object để xem chi tiết
    fullError: JSON.stringify(error, null, 2),
  })

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
    if (uploadProgress.value < 15) return `Đang tải file ${uploadedFiles.value}/${totalFiles.value} lên server...`
    if (uploadProgress.value < 85) return `Đang xử lý file ${uploadedFiles.value}/${totalFiles.value}...`
    if (uploadProgress.value < 100) return `Đang hoàn tất xử lý ${totalFiles.value} files...`
  }

  return 'Đã hoàn thành tất cả!'
}

// Format date từ chuỗi ISO
const formatDate = dateString => {
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

// 📊 Load actual table record counts from database
const loadTableRecordCounts = async () => {
  try {
    console.log('📊 Loading actual table record counts from database')

    const result = await rawDataService.getTableRecordCounts()

    if (result.success && result.data) {
      console.log('✅ Real table counts loaded:', result.data)

      // Update dataTypeStats with real counts from database
      const updatedStats = { ...dataTypeStats.value }

      Object.keys(result.data).forEach(tableName => {
        const realCount = result.data[tableName]

        if (updatedStats[tableName]) {
          updatedStats[tableName] = {
            ...updatedStats[tableName],
            totalRecords: realCount, // Override with real database count
            realRecords: realCount, // Store original real count
            isFromDatabase: true, // Flag to indicate data source
          }
        } else {
          // Initialize if not exists
          updatedStats[tableName] = {
            totalRecords: realCount,
            realRecords: realCount,
            lastUpdate: null,
            count: 0,
            isFromDatabase: true,
          }
        }

        console.log(`📊 ${tableName}: ${realCount} records (from database)`)
      })

      dataTypeStats.value = updatedStats

      showSuccess(`✅ Đã cập nhật số lượng records thực tế từ database`)
      return true
    } else {
      console.error('❌ Failed to load table counts:', result.error)
      showError(`Lỗi khi tải số lượng records: ${result.error}`)
      return false
    }
  } catch (error) {
    console.error('❌ Error loading table record counts:', error)
    showError(`Lỗi khi tải số lượng records: ${error.message}`)
    return false
  }
}

// Data type statistics
const getDataTypeStats = dataType => {
  const stats = dataTypeStats.value[dataType] || { totalRecords: 0, lastUpdate: null }
  // Fix NaN issue: ensure totalRecords is always a valid number
  const totalRecords = parseInt(stats.totalRecords) || 0
  return {
    ...stats,
    totalRecords: totalRecords, // Return raw number, formatting will be done in template
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
    // 🔧 FIX: Consistent với rawDataService mapping - dataType đã được map từ Category
    const dataType = imp.dataType || imp.Category || imp.FileType || 'UNKNOWN'

    if (!stats[dataType]) {
      stats[dataType] = { totalRecords: 0, lastUpdate: null, count: 0 }
    }

    stats[dataType].count++
    // 🔧 FIX: Ưu tiên recordsCount đã được map trong rawDataService
    const recordCount = parseInt(imp.recordsCount || imp.RecordsCount) || 0
    stats[dataType].totalRecords += recordCount

    // 🐛 DEBUG: Log để xem data mapping
    console.log(`📊 Processing ${dataType}: ${recordCount} records from`, imp.fileName || imp.FileName)
    console.log(`📊 Full import item:`, imp)

    const importDate = imp.ImportDate || imp.importDate
    if (importDate && importDate !== '0001-01-01T00:00:00') {
      const importDateTime = new Date(importDate)
      if (
        !isNaN(importDateTime.getTime()) &&
        (!stats[dataType].lastUpdate || importDateTime > new Date(stats[dataType].lastUpdate))
      ) {
        stats[dataType].lastUpdate = importDate
      }
    }
  })

  dataTypeStats.value = stats
}

// Debug function
const debugRecalculateStats = async () => {
  console.log('🔧 DEBUG: Manual recalculate stats')

  // Force refresh data first
  console.log('🔄 Force refreshing data...')
  await refreshAllData(true)

  // Log all imports để kiểm tra data
  console.log('📊 All imports after refresh:', allImports.value)

  if (allImports.value.length > 0) {
    console.log('📊 Sample import item:', allImports.value[0])
    console.log(
      '📊 DP01 items:',
      allImports.value.filter(imp => imp.Category === 'DP01' || imp.dataType === 'DP01' || imp.FileType === 'DP01'),
    )
  }

  // Then recalculate stats
  calculateDataTypeStats()

  // Log current stats for debugging
  console.log('📊 Current dataTypeStats:', dataTypeStats.value)
  console.log('📊 Current allImports count:', allImports.value.length)

  showSuccess(`🔧 Debug: Recalculated stats. Found ${allImports.value.length} imports. Check console for details.`)
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
      resultType: typeof result,
    })

    if (result.success) {
      allImports.value = result.data || []
      console.log('✅ Loaded imports:', allImports.value.length, 'items')

      // Debug log để kiểm tra dữ liệu
      if (allImports.value.length > 0) {
        console.log('📊 Sample import data:', allImports.value[0])
      }

      calculateDataTypeStats()

      // 📊 IMPORTANT: Load actual table record counts from database
      await loadTableRecordCounts()

      if (!skipSuccessMessage) {
        showSuccess(`✅ Đã tải lại dữ liệu thành công (${allImports.value.length} imports)`)
      }

      return { success: true, data: allImports.value }
    } else {
      const errorMsg = result.error || 'Không thể tải dữ liệu'
      console.error('🔥 Chi tiết lỗi getAllImports:', {
        error: result.error,
        errorCode: result.errorCode,
        errorStatus: result.errorStatus,
        fullResult: result,
      })

      if (result.fallbackData && result.fallbackData.length > 0) {
        allImports.value = result.fallbackData
        calculateDataTypeStats()
        if (!skipSuccessMessage) {
          showError(`⚠️ Chế độ Demo: ${errorMsg}`)
        }
        return { success: false, error: errorMsg, fallback: true }
      } else {
        allImports.value = []
        calculateDataTypeStats()
        if (!skipSuccessMessage) {
          console.error('❌ Error in refreshAllData, will not show error to user during import flow')
        }
        return { success: false, error: errorMsg }
      }
    }
  } catch (error) {
    console.error('❌ Exception in refreshAllData:', error)
    if (!skipSuccessMessage) {
      console.error('❌ Refresh error, will not show to user during import flow')
    }
    return { success: false, error: error.message }
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

// ✅ Thêm hàm refresh dữ liệu với nhiều cách fallback khác nhau
const refreshDataWithFallback = async () => {
  console.log('🔄 Refresh data with multiple fallback strategies...')

  try {
    // Chiến thuật 1: Gọi getRecentImports (nhanh nhất)
    console.log('📊 Strategy 1: getRecentImports')
    const recentResult = await rawDataService.getRecentImports(50)

    if (recentResult.success && recentResult.data && recentResult.data.length > 0) {
      console.log('✅ Strategy 1 success:', recentResult.data.length, 'items')
      allImports.value = recentResult.data
      calculateDataTypeStats()
      return { success: true, data: recentResult.data, strategy: 'getRecentImports' }
    }

    // Chiến thuật 2: Gọi getAllImports
    console.log('📊 Strategy 2: getAllImports')
    const importResult = await rawDataService.getAllImports()

    if (importResult.success && importResult.data && importResult.data.length > 0) {
      console.log('✅ Strategy 2 success:', importResult.data.length, 'items')
      allImports.value = importResult.data
      calculateDataTypeStats()
      return { success: true, data: importResult.data, strategy: 'getAllImports' }
    }

    // Chiến thuật 3: Gọi getAllData
    console.log('📊 Strategy 3: getAllData')
    const dataResult = await rawDataService.getAllData()

    if (dataResult.success && dataResult.data && dataResult.data.length > 0) {
      console.log('✅ Strategy 3 success:', dataResult.data.length, 'items')
      allImports.value = dataResult.data
      calculateDataTypeStats()
      return { success: true, data: dataResult.data, strategy: 'getAllData' }
    }

    // Chiến thuật 4: Gọi trực tiếp API endpoint recent
    console.log('📊 Strategy 4: Direct API recent call')
    const directRecentResult = await api.get('/RawData/recent?limit=50')

    if (directRecentResult.data && Array.isArray(directRecentResult.data)) {
      const mappedData = directRecentResult.data.map(item => ({
        ...item,
        dataType: item.category || item.dataType || item.fileType || 'UNKNOWN',
        category: item.category || item.dataType || '',
        recordsCount: parseInt(item.RecordsCount || 0),
        fileName: item.FileName || 'Unknown File',
      }))

      console.log('✅ Strategy 4 success:', mappedData.length, 'items')
      allImports.value = mappedData
      calculateDataTypeStats()
      return { success: true, data: mappedData, strategy: 'directRecentAPI' }
    }

    // Chiến thuật 5: Gọi trực tiếp API endpoint chính
    console.log('📊 Strategy 5: Direct API call')
    const directResult = await api.get('/RawData')

    if (directResult.data && Array.isArray(directResult.data)) {
      const mappedData = directResult.data.map(item => ({
        ...item,
        dataType: item.category || item.dataType || item.fileType || 'UNKNOWN',
        category: item.category || item.dataType || '',
        recordsCount: parseInt(item.RecordsCount || 0),
        fileName: item.FileName || 'Unknown File',
      }))

      console.log('✅ Strategy 5 success:', mappedData.length, 'items')
      allImports.value = mappedData
      calculateDataTypeStats()
      return { success: true, data: mappedData, strategy: 'directAPI' }
    }

    console.log('❌ All strategies failed')
    return { success: false, error: 'All refresh strategies failed' }
  } catch (error) {
    console.error('❌ Error in refreshDataWithFallback:', error)
    return { success: false, error: error.message }
  }
}

const clearAllData = async () => {
  if (
    !confirm(
      '⚠️ BẠN CÓ CHẮC CHẮN MUỐN XÓA TOÀN BỘ DỮ LIỆU?\n\nThao tác này sẽ xóa tất cả dữ liệu đã import và KHÔNG THỂ KHÔI PHỤC!',
    )
  ) {
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
const viewDataType = async dataType => {
  try {
    loading.value = true
    loadingMessage.value = `Đang tải dữ liệu ${dataType}...`
    selectedDataType.value = dataType

    // � ALWAYS USE DIRECT PREVIEW - bỏ logic cũ với selectedFromDate
    console.log(`🎯 Direct preview for ${dataType}...`)

    try {
      // Import DirectPreviewService
      const { default: directPreviewService } = await import('../services/directPreviewService.js')

      // Kiểm tra xem có dữ liệu không
      const hasData = await directPreviewService.hasData(dataType)
      if (!hasData) {
        showError(`Chưa có dữ liệu ${dataType} trong database`)
        return
      }

      // Lấy preview data
      const previewResult = await directPreviewService.previewDataType(dataType, 1, 100)
      if (!previewResult.success) {
        showError(`Lỗi khi xem trước ${dataType}: ${previewResult.error}`)
        return
      }

      // Format data cho hiển thị
      const formattedData = directPreviewService.formatDataForDisplay(previewResult.data, dataType)

      // Set data cho modal với additional date info if selected
      const dateInfo = selectedFromDate.value ? ` (Date: ${formatDate(selectedFromDate.value)})` : ''

      filteredResults.value = [
        {
          dataType: dataType,
          recordCount: previewResult.totalRecords,
          previewData: formattedData,
          directPreview: true, // Flag để biết đây là direct preview
          dateFilter: selectedFromDate.value || null,
        },
      ]

      showSuccess(
        `📊 Xem trước ${formattedData.rows?.length || 0}/${previewResult.totalRecords} records ${dataType}${dateInfo} (Direct Preview)`,
      )
      showDataViewModal.value = true
    } catch (error) {
      console.error('Error viewing data type:', error)
      showError(`Lỗi khi tải dữ liệu: ${error.message}`)
    } finally {
      loading.value = false
      loadingMessage.value = ''
    }
  } catch (error) {
    console.error('Error in viewDataType:', error)
    showError(`Lỗi khi xem dữ liệu: ${error.message}`)
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

const deleteDataTypeByDate = async dataType => {
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
    if (
      confirm(`Bạn có chắc chắn muốn xóa tất cả dữ liệu ${dataType} cho ngày ${formatDate(selectedFromDate.value)}?`)
    ) {
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
      filteredResults.value = filteredResults.value.filter(
        item =>
          !(
            item.dataType === dataType &&
            item.statementDate &&
            new Date(item.statementDate).toISOString().slice(0, 10).replace(/-/g, '') === dateStr
          ),
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

// Xóa toàn bộ dữ liệu của một bảng
const deleteAllDataType = async dataType => {
  const stats = getDataTypeStats(dataType)
  if (stats.totalRecords === 0) {
    showError(`Không có dữ liệu trong bảng ${dataType}`)
    return
  }

  // Hiển thị xác nhận nguy hiểm
  const confirmMsg = `⚠️ CẢNH BÁO: Bạn có chắc chắn muốn xóa TOÀN BỘ ${stats.totalRecords} bản ghi của bảng ${dataType}?\n\nHành động này KHÔNG THỂ HOÀN TÁC!`

  if (!confirm(confirmMsg)) {
    return
  }

  // Xác nhận lần 2
  if (!confirm(`Xác nhận lần cuối: XÓA TOÀN BỘ dữ liệu bảng ${dataType}?`)) {
    return
  }

  try {
    loading.value = true
    loadingMessage.value = `Đang xóa toàn bộ dữ liệu ${dataType}...`

    const result = await rawDataService.deleteAllDataType(dataType)
    if (result.success) {
      showSuccess(`✅ ${result.data.message || 'Đã xóa toàn bộ dữ liệu thành công'}`)
      await refreshAllData() // Refresh để cập nhật stats
    } else {
      showError(`Lỗi khi xóa dữ liệu: ${result.error}`)
    }
  } catch (error) {
    console.error('Error deleting all data:', error)
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
const viewRawDataFromTable = async dataType => {
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
      const convertDotNetArray = data => {
        if (data && typeof data === 'object' && data.$values && Array.isArray(data.$values)) {
          console.log('🔧 Converting raw data $values format, length:', data.$values.length)
          return data.$values
        }
        return data
      }

      // Xử lý dữ liệu records từ backend
      const records = result.data.records || []

      if (records && records.length > 0) {
        rawDataRecords.value = records
        showSuccess(`Đã tải ${records.length} bản ghi dữ liệu thô ${dataType}`)
        showRawDataModal.value = true
      } else {
        showError(`Không tìm thấy dữ liệu thô cho ${dataType} vào ngày ${formatDate(selectedFromDate.value)}`)
      }
    } else {
      showError(`Lỗi khi tải dữ liệu thô: ${result.error || 'Không tìm thấy dữ liệu'}`)
    }
  } catch (error) {
    console.error('Error viewing raw data:', error)
    showError(`Lỗi khi tải dữ liệu thô: ${error.message}`)
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

const closeRawDataModal = () => {
  showRawDataModal.value = false
  rawDataRecords.value = []
}

const exportRawData = () => {
  try {
    // Create CSV content
    let csvContent = ''

    // Get all unique headers
    const headers = new Set()
    rawDataRecords.value.forEach(record => {
      Object.keys(record).forEach(key => headers.add(key))
    })

    // Add headers
    csvContent += Array.from(headers).join(',') + '\n'

    // Add data rows
    rawDataRecords.value.forEach(record => {
      const row = Array.from(headers).map(header => {
        const value = record[header] || ''
        // Handle values with commas by wrapping in quotes
        return typeof value === 'string' && value.includes(',') ? `"${value}"` : value
      })
      csvContent += row.join(',') + '\n'
    })

    // Create download link
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.setAttribute('href', url)
    link.setAttribute('download', `rawdata-${selectedDataType.value}-${selectedFromDate.value}.csv`)
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)

    showSuccess('Đã xuất dữ liệu thành công')
  } catch (error) {
    console.error('Error exporting data:', error)
    showError(`Lỗi khi xuất dữ liệu: ${error.message}`)
  }
}

// Preview import record method - for actual import records with valid IDs
const previewImportRecord = async importId => {
  try {
    if (!importId) {
      showError('Import ID không hợp lệ')
      return
    }

    loading.value = true
    loadingMessage.value = 'Đang tải dữ liệu chi tiết...'

    const result = await rawDataService.previewData(importId)

    if (result.success && result.data) {
      // ✅ Hiển thị modal với dữ liệu thực tế từ database
      const previewRows = result.data.PreviewRows || result.data.previewRows || []

      if (previewRows && previewRows.length > 0) {
        // Hiển thị tối đa 20 bản ghi đầu
        const recordsToShow = previewRows.slice(0, 20)

        // Cập nhật state để hiển thị modal
        rawDataRecords.value = recordsToShow
        showRawDataModal.value = true
        showSuccess(`Hiển thị ${recordsToShow.length} bản ghi từ import ID: ${importId}`)
      } else {
        showError('Không có dữ liệu preview cho import này')
      }
    } else {
      showError(`Lỗi khi tải dữ liệu preview: ${result.error}`)
    }
  } catch (error) {
    console.error('Error previewing import record:', error)
    showError(`Lỗi khi xem trước: ${error.message}`)
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

// Preview data method
const previewData = async importId => {
  try {
    loading.value = true
    loadingMessage.value = 'Đang tải dữ liệu chi tiết...'

    const result = await rawDataService.previewData(importId)

    if (result.success && result.data) {
      // ✅ Hiển thị modal với dữ liệu thực tế từ database
      const previewRows = result.data.PreviewRows || result.data.previewRows || []

      if (previewRows && previewRows.length > 0) {
        // Hiển thị tối đa 20 bản ghi đầu
        const recordsToShow = previewRows.slice(0, 20)

        // Cập nhật state để hiển thị modal
        rawDataRecords.value = recordsToShow
        selectedDataType.value = result.data.Category || result.data.category || 'Dữ liệu chi tiết'

        showSuccess(
          `✅ Đã tải ${recordsToShow.length} bản ghi chi tiết từ ${result.data.TotalRecords || result.data.totalRecords} bản ghi`,
        )
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
const getCategoryName = dataType => {
  // Lấy tên category từ định nghĩa data type
  return dataTypeDefinitions[dataType]?.category || 'Chưa phân loại'
}

const formatDateTime = dateTimeString => {
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
  if (
    confirm(
      `⚠️ Bạn có chắc chắn muốn xóa bản ghi "${fileName}"?\n\nViệc xóa sẽ bao gồm:\n- Xóa bản ghi import khỏi lịch sử\n- Xóa tất cả dữ liệu liên quan trong bảng database\n\nHành động này không thể hoàn tác!`,
    )
  ) {
    try {
      loading.value = true
      loadingMessage.value = 'Đang xóa dữ liệu...'

      const result = await rawDataService.deleteImport(importId)

      if (result.success) {
        showSuccess(`✅ Đã xóa thành công bản ghi "${fileName}" (${result.recordsDeleted} bản ghi dữ liệu)`)

        // Remove from filtered results
        filteredResults.value = filteredResults.value.filter(item => item.Id !== importId)

        // Refresh all data
        await refreshAllData()
      } else {
        showError(`❌ Lỗi khi xóa bản ghi: ${result.error}`)
      }
    } catch (error) {
      console.error('Error deleting import:', error)
      showError(`❌ Lỗi khi xóa bản ghi: ${error.message}`)
    } finally {
      loading.value = false
      loadingMessage.value = ''
    }
  }
}

// Các phương thức tiện ích cho view đã được nhắc đến trong template

const getDataTypeColor = dataType => {
  // Màu sắc tương ứng với loại dữ liệu
  const colors = {
    HDMB: '#2196F3', // Xanh dương
    HDBH: '#4CAF50', // Xanh lá
    HDTH: '#FF9800', // Cam
    HDFX: '#9C27B0', // Tím
    BAOHIEM: '#E91E63', // Hồng
    DANCU: '#607D8B', // Xám xanh
    PHICHUYENTIEN: '#795548', // Nâu
    LAMVIEC: '#00BCD4', // Xanh ngọc
  }

  return colors[dataType] || '#8B1538' // Màu mặc định là màu Agribank
}

const openImportModal = dataType => {
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
      return // Người dùng không muốn hủy
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
      statementDate: selectedFromDate.value,
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
      onProgress: progressInfo => {
        // Cập nhật thông tin progress chung từ backend
        uploadProgress.value = progressInfo.percentage

        // Tính toán file đang được xử lý dựa trên tiến độ và thời gian
        const elapsedTime = Date.now() - uploadStartTime.value
        const estimatedCurrentFile = Math.min(
          Math.floor(elapsedTime / estimatedTimePerFile.value),
          Math.floor(progressInfo.percentage / (100 / selectedFiles.value.length)),
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
            currentUploadingFile.value = 'Hoàn thành tất cả files'
          } else {
            // Tính toán số file đã hoàn thành dựa trên progress
            const completedFiles = Math.floor((progressInfo.percentage / 100) * selectedFiles.value.length)
            uploadedFiles.value = Math.min(completedFiles + 1, selectedFiles.value.length) // +1 cho file đang xử lý
          }
        }

        console.log(
          `📊 Upload Progress: ${progressInfo.percentage}%, File ${uploadedFiles.value}/${totalFiles.value}: ${currentUploadingFile.value}`,
        )
      },
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
          console.log('� Refresh data sau khi import thành công...')

          try {
            loading.value = true
            loadingMessage.value = `Đang tải dữ liệu mới nhất...`

            // ✅ FIX: Sử dụng hàm refresh với fallback strategies
            const refreshResult = await refreshDataWithFallback()

            console.log('📊 Dữ liệu sau khi refresh:', {
              success: refreshResult.success,
              strategy: refreshResult.strategy,
              totalImports: allImports.value.length,
              dataTypes: allImports.value
                .map(imp => imp.dataType || imp.category || imp.fileType)
                .filter((v, i, a) => a.indexOf(v) === i),
            })

            if (refreshResult.success && allImports.value.length > 0) {
              // ✅ Lọc và hiển thị dữ liệu theo loại đã import
              const dataTypeResults = allImports.value.filter(imp => {
                const typeMatches =
                  (imp.dataType && imp.dataType.includes(selectedDataType.value)) ||
                  (imp.category && imp.category.includes(selectedDataType.value)) ||
                  (imp.fileType && imp.fileType.includes(selectedDataType.value))

                return typeMatches
              })

              console.log(`🔍 Filtered results for ${selectedDataType.value}:`, dataTypeResults.length)

              if (dataTypeResults.length > 0) {
                filteredResults.value = dataTypeResults
                showSuccess(`✅ Hiển thị ${dataTypeResults.length} import(s) cho loại ${selectedDataType.value}`)
                showDataViewModal.value = true
              } else {
                // ✅ Hiển thị tất cả dữ liệu mới nhất nếu không tìm thấy theo loại cụ thể
                filteredResults.value = allImports.value.slice(0, 10) // Hiển thị 10 import mới nhất
                showSuccess(`✅ Hiển thị ${filteredResults.value.length} bản ghi import mới nhất`)
                showDataViewModal.value = true
              }
            } else {
              console.log('⚠️ Không có dữ liệu sau khi refresh, thử gọi API trực tiếp...')

              // Thử gọi API trực tiếp để lấy dữ liệu
              const directResult = await rawDataService.getAllData()

              if (directResult.success && directResult.data && directResult.data.length > 0) {
                console.log(`✅ API trực tiếp trả về ${directResult.data.length} bản ghi`)

                filteredResults.value = directResult.data.slice(0, 10) // Hiển thị 10 bản ghi mới nhất
                showSuccess(`✅ Hiển thị ${filteredResults.value.length} bản ghi import mới nhất`)
                showDataViewModal.value = true
              } else {
                showSuccess(`✅ Import thành công! Vui lòng nhấn "🔄 Tải lại dữ liệu" để xem kết quả.`)
              }
            }
          } catch (error) {
            console.error('❌ Error fetching data after import:', error)
            showSuccess(`✅ Import thành công! Vui lòng nhấn "🔄 Tải lại dữ liệu" để xem kết quả.`)
          } finally {
            loading.value = false
            loadingMessage.value = ''
          }
        }, 2500) // ✅ Tăng delay thành 2.5 giây để đảm bảo backend xử lý xong
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
const removeFile = index => {
  selectedFiles.value.splice(index, 1)
}

// Lấy icon tương ứng với loại file
const getFileIcon = fileName => {
  const extension = fileName.split('.').pop()?.toLowerCase() || ''

  const icons = {
    pdf: '📄',
    doc: '📝',
    docx: '📝',
    xls: '📊',
    xlsx: '📊',
    csv: '📋',
    txt: '📄',
    zip: '📦',
    rar: '📦',
    '7z': '📦',
    png: '🖼️',
    jpg: '🖼️',
    jpeg: '🖼️',
    gif: '🖼️',
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
// formatFileSize được import từ ../utils/numberFormatter.js

// Xử lý chọn file
const handleFileSelect = event => {
  const files = event.target.files
  if (files.length === 0) return

  selectedFiles.value = Array.from(files)
}

// Hàm định dạng số lượng bản ghi
const formatRecordCount = count => {
  // Fix NaN issue - ensure proper type checking and conversion
  if (count === null || count === undefined || count === '' || isNaN(Number(count))) {
    return '0'
  }

  // Convert to number and format with thousands separator - US format (1,000,000)
  const numericCount = Number(count)
  return new Intl.NumberFormat('en-US').format(numericCount)
}

// ✅ THÊM MỚI: Hàm format giá trị trong cell để hiển thị đẹp hơn
const formatCellValue = value => {
  if (value === null || value === undefined) return '—'
  if (value === '') return '(trống)'

  // Nếu là string dài, cắt ngắn
  if (typeof value === 'string') {
    if (value.length > 50) {
      return value.substring(0, 47) + '...'
    }
    return value
  }

  // Nếu là số, format với dấu phân cách - US format (1,000,000)
  if (typeof value === 'number') {
    return new Intl.NumberFormat('en-US').format(value)
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
const handleSmartFileSelect = event => {
  console.log('🔍 Smart Import: File selected')
  const files = event.target.files
  if (files.length === 0) return

  smartSelectedFiles.value = Array.from(files)
  console.log('🔍 Smart Import: Files loaded:', smartSelectedFiles.value.length)
}

// Xử lý kéo thả file với animation đẹp
const handleSmartFileDrop = event => {
  console.log('🔍 Smart Import: Files dropped')
  isDragOver.value = false
  const files = event.dataTransfer.files
  if (files.length === 0) return

  smartSelectedFiles.value = Array.from(files)
  console.log('🔍 Smart Import: Files loaded via drop:', smartSelectedFiles.value.length)
}

// Xử lý drag over với hiệu ứng
const handleDragOver = event => {
  event.preventDefault()
  isDragOver.value = true
}

// Xử lý drag enter
const handleDragEnter = event => {
  event.preventDefault()
  isDragOver.value = true
}

// Xử lý drag leave với delay để tránh flicker
let dragLeaveTimeout = null
const handleDragLeave = event => {
  event.preventDefault()

  // Clear timeout cũ nếu có
  if (dragLeaveTimeout) {
    clearTimeout(dragLeaveTimeout)
  }

  // Set timeout để tránh flicker khi drag qua các element con
  dragLeaveTimeout = setTimeout(() => {
    isDragOver.value = false
  }, 50)
}

// Xóa file khỏi danh sách
const removeSmartFile = index => {
  smartSelectedFiles.value.splice(index, 1)
}

// Detect category từ filename
const detectCategory = fileName => {
  return smartImportService.detectCategoryFromFileName(fileName)
}

// Extract date từ filename
const extractDateFromFileName = fileName => {
  return smartImportService.extractDateFromFileName(fileName)
}

// Bắt đầu Smart Import - OPTIMIZED VERSION
const startSmartImport = async () => {
  if (smartSelectedFiles.value.length === 0) {
    errorMessage.value = 'Vui lòng chọn ít nhất một file'
    return
  }

  // 🚀 OPTIMIZATION: Kiểm tra file size trước khi upload
  const totalSize = smartSelectedFiles.value.reduce((sum, file) => sum + file.size, 0)
  const maxTotalSize = 500 * 1024 * 1024 // 500MB total limit

  if (totalSize > maxTotalSize) {
    errorMessage.value = `⚠️ Tổng dung lượng file quá lớn (${formatFileSize(totalSize)}). Giới hạn: ${formatFileSize(maxTotalSize)}`
    return
  }

  smartUploading.value = true
  smartImportResults.value = null
  errorMessage.value = ''
  successMessage.value = ''

  // 📊 Performance tracking
  const startTime = Date.now()

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
      currentFile: 'Chuẩn bị upload...',
    }

    console.log(
      '🧠 Starting OPTIMIZED Smart Import with',
      smartSelectedFiles.value.length,
      'files',
      `(Total size: ${formatFileSize(totalSize)})`,
    )

    // ✅ OPTIMIZATION: Sử dụng callback để update progress real-time
    const progressCallback = progressInfo => {
      smartUploadProgress.value = {
        current: progressInfo.current,
        total: progressInfo.total,
        percentage: progressInfo.percentage,
        currentFile: progressInfo.currentFile,
        stage: progressInfo.stage || 'uploading',
      }

      // 📊 Log detailed progress
      if (progressInfo.fileProgress) {
        console.log(
          `📊 File Progress: ${progressInfo.currentFile} - ${progressInfo.fileProgress.percentage}% (${formatFileSize(progressInfo.fileProgress.loaded)}/${formatFileSize(progressInfo.fileProgress.total)})`,
        )
      }
    }

    // Call OPTIMIZED Smart Import Service với progress callback
    const results = await smartImportService.uploadSmartFiles(smartSelectedFiles.value, statementDate, progressCallback)

    // Calculate total duration
    const endTime = Date.now()
    const duration = (endTime - startTime) / 1000
    results.duration = duration

    smartImportResults.value = results

    // ✅ OPTIMIZATION: Hiển thị thông tin chi tiết
    const avgTimePerFile = duration > 0 ? (duration / results.totalFiles).toFixed(1) : 'N/A'
    const avgSpeedMBps = totalSize > 0 && duration > 0 ? (totalSize / (1024 * 1024) / duration).toFixed(2) : 'N/A'

    if (results.successCount > 0) {
      // 🔊 AUDIO NOTIFICATION: Phát âm thanh thành công
      audioService.playSuccess()

      const sizeInfo = `${formatFileSize(totalSize)}`
      successMessage.value = `✅ Smart Import hoàn thành! ${results.successCount}/${results.totalFiles} file thành công (${sizeInfo} trong ${duration.toFixed(1)}s - ${avgSpeedMBps} MB/s)`

      // Refresh data sau khi import thành công
      await refreshAllData()
    }

    if (results.failureCount > 0) {
      // 🔊 AUDIO NOTIFICATION: Âm thanh cảnh báo cho lỗi
      audioService.playNotification()
      errorMessage.value = `⚠️ ${results.failureCount}/${results.totalFiles} file import thất bại. Xem chi tiết bên dưới.`
    }

    // Log thống kê cuối
    console.log(`📈 Smart Import Stats:`, {
      totalFiles: results.totalFiles,
      successCount: results.successCount,
      failureCount: results.failureCount,
      totalSize: formatFileSize(totalSize),
      duration: `${duration.toFixed(1)}s`,
      avgTimePerFile: `${avgTimePerFile}s`,
      avgSpeed: `${avgSpeedMBps} MB/s`,
    })
  } catch (error) {
    console.error('🔥 Smart Import error:', error)

    // 🔊 AUDIO NOTIFICATION: Âm thanh lỗi
    audioService.playError()

    errorMessage.value = `Lỗi Smart Import: ${error.message}`
  } finally {
    smartUploading.value = false
    smartUploadProgress.value.percentage = 100
    smartUploadProgress.value.currentFile = 'Hoàn thành'
  }
}
</script>

<style scoped>
/* 🏦 AGRIBANK BRAND STYLING */
.header-section {
  background: linear-gradient(135deg, #8b1538 0%, #c41e3a 50%, #8b1538 100%);
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
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.3);
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
  border-top: 4px solid #8b1538;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 15px;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
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
  color: #8b1538;
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
  border-color: #8b1538;
  outline: none;
}

.date-actions-group {
  display: flex;
  gap: 10px;
}

.agribank-btn-filter,
.agribank-btn-clear {
  padding: 8px 16px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  font-size: 14px;
}

.agribank-btn-filter {
  background: #8b1538;
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
  color: #8b1538;
  margin-bottom: 15px;
}

.bulk-actions {
  display: flex;
  gap: 15px;
  flex-wrap: wrap;
}

.btn-clear-all,
.btn-refresh,
.btn-debug {
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

.btn-clear-all:hover:not(:disabled),
.btn-refresh:hover:not(:disabled),
.btn-debug:hover:not(:disabled) {
  opacity: 0.9;
}

.btn-clear-all:disabled,
.btn-refresh:disabled,
.btn-debug:disabled {
  background: #ccc;
  cursor: not-allowed;
}

/* Data types section */
.agribank-section {
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.agribank-header {
  background: linear-gradient(135deg, #8b1538 0%, #c41e3a 100%);
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
  background: linear-gradient(90deg, #fff 0%, rgba(255, 255, 255, 0.5) 50%, #fff 100%);
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
  color: #8b1538;
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
  color: #8b1538;
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
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
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
  background: rgba(0, 0, 0, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
  backdrop-filter: blur(3px);
  animation: fadeIn 0.2s ease;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

.modal-content {
  background: white;
  border-radius: 12px;
  max-width: 500px;
  width: 90%;
  max-height: 80vh;
  overflow-y: auto;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.3);
  animation: slideDown 0.3s ease;
  border: 1px solid rgba(0, 0, 0, 0.1);
}

.import-modal {
  max-width: 600px;
}

@keyframes slideDown {
  from {
    transform: translateY(-30px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.modal-header {
  background: linear-gradient(135deg, #8b1538 0%, #c41e3a 100%);
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
  background: rgba(255, 255, 255, 0.2);
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
  background: #8b1538;
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
  box-shadow: inset 0 1px 3px rgba(0, 0, 0, 0.1);
}

.progress-bar {
  height: 100%;
  background: linear-gradient(90deg, #8b1538 0%, #c41e3a 100%);
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
  0% {
    background-position: 0 0;
  }
  100% {
    background-position: 30px 0;
  }
}

.progress-details {
  display: flex;
  justify-content: space-between;
  font-size: 0.85rem;
  color: #666;
}

.progress-percentage {
  font-weight: bold;
  color: #8b1538;
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
  border-color: #8b1538;
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
  background: linear-gradient(135deg, #8b1538 0%, #c41e3a 100%);
  color: white;
}

.btn-submit:hover:not(:disabled) {
  background: linear-gradient(135deg, #7a1230 0%, #b31a33 100%);
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.btn-submit:disabled {
  background: #ccc;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

/* Agribank import styling */
.btn-submit {
  background: linear-gradient(135deg, #8b1538 0%, #c41e3a 100%);
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
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
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
  color: #8b1538;
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
  color: #8b1538;
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
  border-left: 3px solid #8b1538;
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
  box-shadow: 0 2px 4px rgba(0, 123, 255, 0.3);
}

.btn-smart-import:hover:not(:disabled) {
  background: linear-gradient(135deg, #0056b3 0%, #004085 100%);
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 123, 255, 0.4);
}

.btn-smart-import:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none;
}

/* ✨ Smart Import Optimization Badge */
.optimization-badge {
  background: linear-gradient(135deg, #00c851 0%, #00b04f 100%);
  border-radius: 12px;
  padding: 8px 16px;
  margin-top: 15px;
  text-align: center;
  box-shadow: 0 4px 12px rgba(0, 200, 81, 0.3);
  animation: optimizedGlow 2s ease-in-out infinite alternate;
}

.optimization-badge .badge-text {
  font-weight: bold;
  font-size: 0.9rem;
  color: white;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.3);
}

.optimization-badge small {
  display: block;
  color: rgba(255, 255, 255, 0.9);
  font-size: 0.75rem;
  margin-top: 4px;
}

@keyframes optimizedGlow {
  from {
    box-shadow: 0 4px 12px rgba(0, 200, 81, 0.3);
  }
  to {
    box-shadow: 0 6px 20px rgba(0, 200, 81, 0.5);
  }
}

/* 🚀 ENHANCED SMART UPLOAD PROGRESS STYLES */
.smart-upload-progress {
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  border: 2px solid #8b1538;
  border-radius: 15px;
  padding: 20px;
  margin: 20px 0;
  box-shadow: 0 8px 25px rgba(139, 21, 56, 0.2);
  animation: uploadPulse 2s ease-in-out infinite alternate;
}

.progress-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
}

.progress-header h4 {
  margin: 0;
  color: #8b1538;
  font-size: 1.2rem;
  font-weight: bold;
}

.progress-info {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 5px;
}

.progress-text {
  font-weight: 600;
  color: #333;
  font-size: 0.9rem;
}

.progress-percentage {
  font-weight: bold;
  color: #8b1538;
  font-size: 1.1rem;
}

.progress-bar-container {
  background: #e9ecef;
  border-radius: 10px;
  height: 25px;
  overflow: hidden;
  margin-bottom: 15px;
  position: relative;
  box-shadow: inset 0 2px 4px rgba(0, 0, 0, 0.1);
}

.progress-bar {
  height: 100%;
  background: linear-gradient(135deg, #8b1538 0%, #c41e3a 50%, #8b1538 100%);
  transition: width 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: flex-end;
  padding-right: 10px;
  position: relative;
  overflow: hidden;
}

.progress-bar::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.3), transparent);
  animation: shimmer 2s infinite;
}

.progress-bar-text {
  color: white;
  font-weight: bold;
  font-size: 0.8rem;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.5);
}

.current-file-info {
  background: rgba(139, 21, 56, 0.05);
  border-radius: 8px;
  padding: 12px;
}

.current-file {
  margin: 0 0 8px 0;
  font-weight: 600;
  color: #333;
  word-break: break-all;
}

.upload-stage {
  margin: 0 0 8px 0;
  font-size: 0.9rem;
  color: #666;
  font-style: italic;
}

.parallel-info {
  margin: 0;
  font-size: 0.9rem;
  color: #8b1538;
  font-weight: 600;
  background: rgba(139, 21, 56, 0.1);
  padding: 4px 8px;
  border-radius: 4px;
  display: inline-block;
}

@keyframes uploadPulse {
  0%,
  100% {
    transform: scale(1.02);
  }
  50% {
    transform: scale(1.05);
  }
}

@keyframes shimmer {
  0% {
    left: -100%;
  }
  100% {
    left: 100%;
  }
}

/* 📊 RESULT STATS STYLING */
.result-stats {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  margin-bottom: 15px;
}

.stat {
  padding: 8px 12px;
  border-radius: 6px;
  font-weight: 600;
  font-size: 0.9rem;
  white-space: nowrap;
}

.stat.success {
  background: rgba(0, 200, 81, 0.1);
  color: #00c851;
  border: 1px solid rgba(0, 200, 81, 0.3);
}

.stat.error {
  background: rgba(244, 67, 54, 0.1);
  color: #f44336;
  border: 1px solid rgba(244, 67, 54, 0.3);
}

.stat.total {
  background: rgba(139, 21, 56, 0.1);
  color: #8b1538;
  border: 1px solid rgba(139, 21, 56, 0.3);
}

.stat.method {
  background: rgba(33, 150, 243, 0.1);
  color: #2196f3;
  border: 1px solid rgba(33, 150, 243, 0.3);
  animation: methodGlow 2s ease-in-out infinite alternate;
}

@keyframes methodGlow {
  from {
    box-shadow: 0 2px 8px rgba(33, 150, 243, 0.2);
  }
  to {
    box-shadow: 0 4px 16px rgba(33, 150, 243, 0.4);
  }
}

/* ✨ DRAG & DROP AREA - SIÊU ĐẸP LUNG LINH */
.file-drop-area {
  border: 3px dashed #8b1538;
  border-radius: 20px;
  padding: 40px 20px;
  text-align: center;
  background: linear-gradient(
    135deg,
    rgba(139, 21, 56, 0.02) 0%,
    rgba(139, 21, 56, 0.05) 50%,
    rgba(139, 21, 56, 0.02) 100%
  );
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  overflow: hidden;
  margin: 20px 0;
}

.file-drop-area::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(139, 21, 56, 0.1), transparent);
  transition: left 0.6s ease;
}

.file-drop-area:hover::before {
  left: 100%;
}

.file-drop-area:hover {
  border-color: #c41e3a;
  background: linear-gradient(
    135deg,
    rgba(139, 21, 56, 0.05) 0%,
    rgba(139, 21, 56, 0.1) 50%,
    rgba(139, 21, 56, 0.05) 100%
  );
  transform: translateY(-3px);
  box-shadow: 0 12px 40px rgba(139, 21, 56, 0.15);
}

.file-drop-area.drag-over {
  border-color: #00c851;
  background: linear-gradient(135deg, rgba(0, 200, 81, 0.1) 0%, rgba(0, 200, 81, 0.2) 50%, rgba(0, 200, 81, 0.1) 100%);
  transform: scale(1.02);
  box-shadow: 0 20px 60px rgba(0, 200, 81, 0.3);
  animation: dragPulse 1s ease-in-out infinite;
}

.file-drop-area.has-files {
  border-color: #2196f3;
  background: linear-gradient(
    135deg,
    rgba(33, 150, 243, 0.05) 0%,
    rgba(33, 150, 243, 0.1) 50%,
    rgba(33, 150, 243, 0.05) 100%
  );
}

.drop-zone-main {
  position: relative;
  z-index: 2;
}

.upload-icon-container {
  position: relative;
  display: inline-block;
  margin-bottom: 20px;
}

.upload-icon-wrapper {
  width: 80px;
  height: 80px;
  background: linear-gradient(135deg, #8b1538 0%, #c41e3a 100%);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 8px 25px rgba(139, 21, 56, 0.3);
  transition: all 0.3s ease;
  position: relative;
}

.drag-over .upload-icon-wrapper {
  background: linear-gradient(135deg, #00c851 0%, #00b04f 100%);
  transform: rotate(360deg) scale(1.1);
  box-shadow: 0 12px 35px rgba(0, 200, 81, 0.4);
}

.upload-icon {
  width: 40px;
  height: 40px;
  color: white;
  transition: all 0.3s ease;
}

.upload-sparkles {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  pointer-events: none;
}

.sparkle {
  position: absolute;
  font-size: 16px;
  animation: sparkleFloat 3s ease-in-out infinite;
  opacity: 0;
}

.sparkle-1 {
  top: -30px;
  left: -30px;
  animation-delay: 0s;
}

.sparkle-2 {
  top: -40px;
  right: -35px;
  animation-delay: 1s;
}

.sparkle-3 {
  bottom: -35px;
  left: -25px;
  animation-delay: 2s;
}

.drop-title {
  font-size: 1.4rem;
  font-weight: 700;
  color: #333;
  margin: 0 0 10px 0;
  transition: all 0.3s ease;
}

.drag-active {
  color: #00c851 !important;
  animation: textBounce 0.6s ease-in-out infinite alternate;
}

.drop-subtitle {
  color: #666;
  margin: 0 0 25px 0;
  font-size: 0.95rem;
  line-height: 1.4;
}

.upload-divider {
  margin: 25px 0;
  position: relative;
  color: #999;
  font-size: 0.9rem;
}

.upload-divider::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 0;
  right: 0;
  height: 1px;
  background: linear-gradient(to right, transparent, #ddd 20%, #ddd 80%, transparent);
  z-index: 1;
}

.upload-divider span {
  background: white;
  padding: 0 15px;
  position: relative;
  z-index: 2;
}

.btn-select-files {
  background: linear-gradient(135deg, #8b1538 0%, #c41e3a 100%);
  color: white;
  border: none;
  padding: 16px 32px;
  border-radius: 50px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 12px;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 0 6px 20px rgba(139, 21, 56, 0.3);
  position: relative;
  overflow: hidden;
}

.btn-select-files::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent);
  transition: left 0.6s ease;
}

.btn-select-files:hover::before {
  left: 100%;
}

.btn-select-files:hover {
  background: linear-gradient(135deg, #a02a4a 0%, #d63654 100%);
  transform: translateY(-3px);
  box-shadow: 0 12px 35px rgba(139, 21, 56, 0.4);
}

.btn-select-files:active {
  transform: translateY(-1px);
}

.btn-icon {
  width: 20px;
  height: 20px;
  transition: transform 0.3s ease;
}

.btn-select-files:hover .btn-icon {
  transform: translateY(-2px);
}

.supported-formats {
  display: flex;
  justify-content: center;
  gap: 20px;
  margin-top: 25px;
  padding-top: 20px;
  border-top: 1px solid rgba(139, 21, 56, 0.1);
}

.format-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  padding: 12px;
  background: rgba(139, 21, 56, 0.05);
  border-radius: 12px;
  transition: all 0.3s ease;
  cursor: default;
}

.format-item:hover {
  background: rgba(139, 21, 56, 0.1);
  transform: translateY(-2px);
}

.format-icon {
  font-size: 1.5rem;
}

.format-item span:last-child {
  font-size: 0.8rem;
  font-weight: 600;
  color: #8b1538;
}

/* ANIMATIONS */
@keyframes dragPulse {
  0%,
  100% {
    transform: scale(1.02);
  }
  50% {
    transform: scale(1.05);
  }
}

@keyframes sparkleFloat {
  0%,
  100% {
    opacity: 0;
    transform: translateY(0) rotate(0deg);
  }
  50% {
    opacity: 1;
    transform: translateY(-20px) rotate(180deg);
  }
}

@keyframes textBounce {
  from {
    transform: scale(1);
  }
  to {
    transform: scale(1.05);
  }
}

/* RESPONSIVE */
@media (max-width: 768px) {
  .file-drop-area {
    padding: 30px 15px;
  }

  .upload-icon-wrapper {
    width: 60px;
    height: 60px;
  }

  .upload-icon {
    width: 30px;
    height: 30px;
  }

  .drop-title {
    font-size: 1.2rem;
  }

  .btn-select-files {
    padding: 14px 24px;
    font-size: 0.9rem;
  }

  .supported-formats {
    gap: 15px;
  }

  .format-item {
    padding: 8px;
  }
}
</style>
