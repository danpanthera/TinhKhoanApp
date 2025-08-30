<!-- DataTablesView.vue - Direct Import và Preview cho 8 bảng dữ liệu chính -->
<template>
  <div class="data-tables-view">
    <!-- Header -->
    <div class="view-header">
      <h1 class="title">
        🗄️ DataTables Management
      </h1>
      <p class="subtitle">
        Direct Import & Preview cho 8 bảng dữ liệu chính
      </p>
    </div>

    <!-- Summary Cards -->
    <div class="summary-cards">
      <div v-for="table in dataTablesSummary" :key="table.tableName" class="card">
        <div class="card-header">
          <h3>{{ table.tableName }}</h3>
          <span :class="['storage-badge', table.storageType.toLowerCase()]">
            {{ table.storageType }}
          </span>
        </div>
        <div class="card-body">
          <div class="stat">
            <span class="label">Records:</span>
            <span class="value">{{ formatNumber(table.recordCount) }}</span>
          </div>
          <div class="stat">
            <span class="label">Columnstore:</span>
            <span :class="['value', table.hasColumnstore === 'Yes' ? 'enabled' : 'disabled']">
              {{ table.hasColumnstore }}
            </span>
          </div>
        </div>
        <div class="card-actions">
          <button class="btn btn-preview" @click="previewTable(table.tableName)">
            👁️ Preview
          </button>
          <button class="btn btn-import" @click="openImportDialog(table.tableName)">
            📥 Import
          </button>
        </div>
      </div>
    </div>

    <!-- Active Table Section -->
    <div v-if="activeTable" class="active-section">
      <div class="section-header">
        <h2>{{ activeTable.toUpperCase() }} - {{ getStorageType(activeTable) }}</h2>
        <div class="section-actions">
          <button class="btn btn-refresh" @click="refreshPreview">
            🔄 Refresh
          </button>
          <button class="btn btn-close" @click="clearActiveTable">
            ❌ Close
          </button>
        </div>
      </div>

      <!-- Preview Data -->
      <div v-if="previewData.length > 0" class="preview-section">
        <h3>📋 Preview Direct (10 bản ghi gần nhất)</h3>
        <div class="data-table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th v-for="column in previewColumns" :key="column">
                  {{ column }}
                </th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(row, index) in previewData" :key="index">
                <td v-for="column in previewColumns" :key="column" :class="getColumnClass(column)">
                  {{ formatCellValue(row[column], column) }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <p class="preview-note">
          ✅ Dữ liệu lấy trực tiếp từ bảng {{ activeTable.toUpperCase() }} ({{ getStorageType(activeTable) }})
        </p>
      </div>

      <!-- No Data -->
      <div v-else class="no-data">
        <p>📋 Chưa có dữ liệu trong bảng {{ activeTable.toUpperCase() }}</p>
        <button class="btn btn-import" @click="openImportDialog(activeTable)">
          📥 Import Data
        </button>
      </div>
    </div>

    <!-- Import Dialog -->
    <div v-if="showImportDialog" class="modal-overlay" @click="closeImportDialog">
      <div class="modal-dialog" @click.stop>
        <div class="modal-header">
          <h3>📥 Import Data - {{ importTableName.toUpperCase() }}</h3>
          <button class="btn btn-close" @click="closeImportDialog">
            ❌
          </button>
        </div>

        <div class="modal-body">
          <div class="import-options">
            <div class="option-card active">
              <h4>📄 CSV File Upload</h4>
              <p>Upload CSV file để import trực tiếp vào bảng {{ importTableName.toUpperCase() }}</p>

              <div class="file-upload">
                <input
                  ref="fileInput"
                  type="file"
                  accept=".csv"
                  class="file-input"
                  @change="handleFileSelect"
                >
                <button class="btn btn-upload" @click="$refs.fileInput.click()">
                  📁 Choose CSV File
                </button>
                <span v-if="selectedFile" class="file-name">
                  {{ selectedFile.name }}
                </span>
              </div>

              <div v-if="csvPreview.length > 0" class="csv-preview">
                <h5>📋 CSV Preview (5 rows):</h5>
                <div class="preview-table">
                  <table>
                    <thead>
                      <tr>
                        <th v-for="header in csvHeaders" :key="header">
                          {{ header }}
                        </th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="(row, index) in csvPreview.slice(0, 5)" :key="index">
                        <td v-for="header in csvHeaders" :key="header">
                          {{ row[header] }}
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
                <p class="preview-info">
                  Showing 5 of {{ csvPreview.length }} rows
                </p>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button class="btn btn-cancel" @click="closeImportDialog">
            Cancel
          </button>
          <button :disabled="!selectedFile || importing" class="btn btn-primary" @click="executeImport">
            <span v-if="importing">⏳ Importing...</span>
            <span v-else>📥 Import Data</span>
          </button>
        </div>
      </div>
    </div>

    <!-- Loading Overlay -->
    <div v-if="loading" class="loading-overlay">
      <div class="loading-content">
        <div class="spinner" />
        <p>{{ loadingMessage }}</p>
      </div>
    </div>
  </div>
</template>

<script>
import { formatNgayDL } from '@/utils/date'
import { useNumberInput } from '@/utils/numberFormat'
import { onMounted, ref } from 'vue'

export default {
  name: 'DataTablesView',
  setup() {
    const { formatNumber, formatCurrency } = useNumberInput()

    // Reactive data
    const dataTablesSummary = ref([])
    const activeTable = ref(null)
    const previewData = ref([])
    const previewColumns = ref([])
    const loading = ref(false)
    const loadingMessage = ref('')

    // Import dialog
    const showImportDialog = ref(false)
    const importTableName = ref('')
    const selectedFile = ref(null)
    const csvPreview = ref([])
    const csvHeaders = ref([])
    const importing = ref(false)

    // 8 Core DataTables configuration
    const dataTablesConfig = {
      GL01: { name: 'GL01', storageType: 'Partitioned', description: 'General Ledger Data' },
      DP01: { name: 'DP01', storageType: 'Temporal', description: 'Deposit Data' },
      DPDA: { name: 'DPDA', storageType: 'Temporal', description: 'Deposit Details' },
      EI01: { name: 'EI01', storageType: 'Temporal', description: 'Employee Information' },
      GL41: { name: 'GL41', storageType: 'Temporal', description: 'GL Summary' },
      LN01: { name: 'LN01', storageType: 'Temporal', description: 'Loan Data' },
      LN03: { name: 'LN03', storageType: 'Temporal', description: 'Loan Collateral' },
      RR01: { name: 'RR01', storageType: 'Temporal', description: 'Risk Rating' },
    }

    // Methods
    const fetchDataTablesSummary = async () => {
      loading.value = true
      loadingMessage.value = 'Loading DataTables summary...'

      try {
        const response = await fetch('/api/datatables/summary')
        const result = await response.json()

        if (result.success) {
          dataTablesSummary.value = result.tables
        }
      } catch (error) {
        console.error('Error fetching summary:', error)
      } finally {
        loading.value = false
      }
    }

    const previewTable = async tableName => {
      loading.value = true
      loadingMessage.value = `Loading ${tableName} preview...`
      activeTable.value = tableName

      try {
        const response = await fetch(`/api/datatables/${tableName.toLowerCase()}/preview`)
        const result = await response.json()

        if (result.success && result.data.length > 0) {
          previewData.value = result.data
          previewColumns.value = Object.keys(result.data[0])
        } else {
          previewData.value = []
          previewColumns.value = []
        }
      } catch (error) {
        console.error('Error previewing table:', error)
        previewData.value = []
      } finally {
        loading.value = false
      }
    }

    const refreshPreview = () => {
      if (activeTable.value) {
        previewTable(activeTable.value)
      }
    }

    const clearActiveTable = () => {
      activeTable.value = null
      previewData.value = []
      previewColumns.value = []
    }

    const openImportDialog = tableName => {
      importTableName.value = tableName
      showImportDialog.value = true
      selectedFile.value = null
      csvPreview.value = []
      csvHeaders.value = []
    }

    const closeImportDialog = () => {
      showImportDialog.value = false
      selectedFile.value = null
      csvPreview.value = []
      csvHeaders.value = []
    }

    const handleFileSelect = event => {
      const file = event.target.files[0]
      if (file && file.type === 'text/csv') {
        selectedFile.value = file
        parseCSVPreview(file)
      }
    }

    const parseCSVPreview = file => {
      const reader = new FileReader()
      reader.onload = e => {
        const csv = e.target.result
        const lines = csv.split('\n').filter(line => line.trim())

        if (lines.length > 0) {
          csvHeaders.value = lines[0].split(',').map(h => h.trim())

          const data = lines.slice(1).map(line => {
            const values = line.split(',')
            const row = {}
            csvHeaders.value.forEach((header, index) => {
              row[header] = values[index]?.trim() || ''
            })
            return row
          })

          csvPreview.value = data
        }
      }
      reader.readAsText(file)
    }

    const executeImport = async () => {
      if (!selectedFile.value) return

      importing.value = true

      try {
        const formData = new FormData()
        formData.append('file', selectedFile.value)
        formData.append('tableName', importTableName.value)

        const response = await fetch(`/api/datatables/${importTableName.value.toLowerCase()}/import`, {
          method: 'POST',
          body: formData,
        })

        const result = await response.json()

        if (result.success) {
          alert(`✅ Import thành công ${result.importedRecords} bản ghi vào ${importTableName.value}`)
          closeImportDialog()

          // Refresh preview if this table is active
          if (activeTable.value === importTableName.value) {
            refreshPreview()
          }

          // Refresh summary
          fetchDataTablesSummary()
        } else {
          alert(`❌ Import failed: ${result.message}`)
        }
      } catch (error) {
        console.error('Import error:', error)
        alert(`❌ Import error: ${error.message}`)
      } finally {
        importing.value = false
      }
    }

    const getStorageType = tableName => {
      return dataTablesConfig[tableName.toUpperCase()]?.storageType || 'Unknown'
    }

    const getColumnClass = column => {
      const numericColumns = ['amount', 'balance', 'salary', 'bonus', 'rate']
      const isNumeric = numericColumns.some(col => column.toLowerCase().includes(col))
      return isNumeric ? 'numeric' : ''
    }

    const formatCellValue = (value, column) => {
      if (value === null || value === undefined) return ''

      // Special-case NGAY_DL formatting
      if (column && column.toString().toLowerCase() === 'ngay_dl') {
        return formatNgayDL(value)
      }

      // Format numeric columns
      if (getColumnClass(column) === 'numeric') {
        return formatCurrency(value)
      }

      // Format dates
      if (column.toLowerCase().includes('date') || column.toLowerCase().includes('time')) {
        return new Date(value).toLocaleDateString('vi-VN')
      }

      return value
    }

    // Lifecycle
    onMounted(() => {
      fetchDataTablesSummary()
    })

    return {
      // Data
      dataTablesSummary,
      activeTable,
      previewData,
      previewColumns,
      loading,
      loadingMessage,
      showImportDialog,
      importTableName,
      selectedFile,
      csvPreview,
      csvHeaders,
      importing,

      // Methods
      previewTable,
      refreshPreview,
      clearActiveTable,
      openImportDialog,
      closeImportDialog,
      handleFileSelect,
      executeImport,
      getStorageType,
      getColumnClass,
      formatCellValue,
      formatNumber,
      formatCurrency,
  formatNgayDL,
    }
  },
}
</script>

<style scoped>
.data-tables-view {
  padding: 20px;
  max-width: 1400px;
  margin: 0 auto;
}

.view-header {
  text-align: center;
  margin-bottom: 30px;
}

.title {
  font-size: 2.5rem;
  color: #2c3e50;
  margin-bottom: 10px;
}

.subtitle {
  font-size: 1.2rem;
  color: #7f8c8d;
}

.summary-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.card {
  background: white;
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  border: 1px solid #e1e8ed;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
}

.card-header h3 {
  margin: 0;
  font-size: 1.3rem;
  color: #2c3e50;
}

.storage-badge {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: bold;
  text-transform: uppercase;
}

.storage-badge.partitioned {
  background: #e8f5e8;
  color: #27ae60;
}

.storage-badge.temporal {
  background: #e3f2fd;
  color: #1976d2;
}

.card-body {
  margin-bottom: 15px;
}

.stat {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
}

.stat .label {
  color: #7f8c8d;
}

.stat .value {
  font-weight: bold;
  color: #2c3e50;
}

.stat .value.enabled {
  color: #27ae60;
}

.stat .value.disabled {
  color: #e74c3c;
}

.card-actions {
  display: flex;
  gap: 10px;
}

.btn {
  padding: 8px 16px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 0.9rem;
  transition: all 0.2s;
}

.btn-preview {
  background: #3498db;
  color: white;
}

.btn-import {
  background: #27ae60;
  color: white;
}

.btn:hover {
  transform: translateY(-1px);
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.active-section {
  background: white;
  border-radius: 12px;
  padding: 25px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  padding-bottom: 15px;
  border-bottom: 2px solid #ecf0f1;
}

.section-actions {
  display: flex;
  gap: 10px;
}

.btn-refresh {
  background: #f39c12;
  color: white;
}

.btn-close {
  background: #e74c3c;
  color: white;
}

.data-table-container {
  overflow-x: auto;
  margin: 15px 0;
}

.data-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
}

.data-table th,
.data-table td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #ecf0f1;
}

.data-table th {
  background: #f8f9fa;
  font-weight: bold;
  color: #2c3e50;
  position: sticky;
  top: 0;
}

.data-table td.numeric {
  text-align: right;
  font-family: 'Courier New', monospace;
}

.preview-note {
  margin-top: 15px;
  padding: 10px;
  background: #e8f5e8;
  border-radius: 6px;
  color: #27ae60;
  font-style: italic;
}

.no-data {
  text-align: center;
  padding: 40px;
  color: #7f8c8d;
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-dialog {
  background: white;
  border-radius: 12px;
  width: 90%;
  max-width: 600px;
  max-height: 80vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  border-bottom: 1px solid #ecf0f1;
}

.modal-body {
  padding: 20px;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  padding: 20px;
  border-top: 1px solid #ecf0f1;
}

.option-card {
  border: 2px solid #ecf0f1;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 15px;
}

.option-card.active {
  border-color: #3498db;
  background: #f8fbff;
}

.file-upload {
  display: flex;
  align-items: center;
  gap: 15px;
  margin: 15px 0;
}

.file-input {
  display: none;
}

.btn-upload {
  background: #34495e;
  color: white;
}

.file-name {
  color: #27ae60;
  font-style: italic;
}

.csv-preview {
  margin-top: 20px;
}

.preview-table {
  max-height: 200px;
  overflow-y: auto;
  border: 1px solid #ecf0f1;
  border-radius: 6px;
}

.preview-table table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.8rem;
}

.preview-table th,
.preview-table td {
  padding: 8px;
  border-bottom: 1px solid #ecf0f1;
  text-align: left;
}

.preview-table th {
  background: #f8f9fa;
  position: sticky;
  top: 0;
}

.preview-info {
  margin-top: 10px;
  color: #7f8c8d;
  font-size: 0.9rem;
}

/* Loading Styles */
.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(255, 255, 255, 0.9);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
}

.loading-content {
  text-align: center;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #3498db;
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

/* Responsive */
@media (max-width: 768px) {
  .summary-cards {
    grid-template-columns: 1fr;
  }

  .section-header {
    flex-direction: column;
    gap: 15px;
    align-items: stretch;
  }

  .section-actions {
    justify-content: center;
  }
}
</style>
