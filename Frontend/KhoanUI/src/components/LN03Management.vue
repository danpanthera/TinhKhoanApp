<template>
  <div class="ln03-management">
    <!-- Header Section -->
    <div class="header-section">
      <div class="title-area">
        <h1 class="page-title">
          <Icon icon="ph:bank" />
          LN03 - Quản Lý Dữ Liệu Khoán Vay
        </h1>
        <p class="page-description">
          Quản lý dữ liệu khoán vay với temporal table, hỗ trợ import CSV và analytics
        </p>
      </div>

      <div class="action-buttons">
        <el-button
          type="primary"
          :loading="importing"
          @click="showImportDialog = true"
        >
          <Icon icon="ph:upload" /> Import CSV
        </el-button>

        <el-button
          :loading="exporting"
          @click="exportData"
        >
          <Icon icon="ph:download" /> Export CSV
        </el-button>

        <el-button
          :loading="loading"
          @click="refreshData"
        >
          <Icon icon="ph:refresh" /> Làm mới
        </el-button>
      </div>
    </div>

    <!-- Statistics Cards -->
    <div class="stats-section">
      <el-row :gutter="20">
        <el-col :span="6">
          <el-card class="stat-card">
            <div class="stat-content">
              <div class="stat-icon total">
                <Icon icon="ph:database" />
              </div>
              <div class="stat-info">
                <div class="stat-value">
                  {{ formatNumber(totalRecords) }}
                </div>
                <div class="stat-label">
                  Tổng số bản ghi
                </div>
              </div>
            </div>
          </el-card>
        </el-col>

        <el-col :span="6">
          <el-card class="stat-card">
            <div class="stat-content">
              <div class="stat-icon branch">
                <Icon icon="ph:building-office" />
              </div>
              <div class="stat-info">
                <div class="stat-value">
                  {{ branchCount }}
                </div>
                <div class="stat-label">
                  Chi nhánh
                </div>
              </div>
            </div>
          </el-card>
        </el-col>

        <el-col :span="6">
          <el-card class="stat-card">
            <div class="stat-content">
              <div class="stat-icon amount">
                <Icon icon="ph:coins" />
              </div>
              <div class="stat-info">
                <div class="stat-value">
                  {{ formatCurrency(totalAmount) }}
                </div>
                <div class="stat-label">
                  Tổng số tiền
                </div>
              </div>
            </div>
          </el-card>
        </el-col>

        <el-col :span="6">
          <el-card class="stat-card">
            <div class="stat-content">
              <div class="stat-icon date">
                <Icon icon="ph:calendar" />
              </div>
              <div class="stat-info">
                <div class="stat-value">
                  {{ lastUpdateDate }}
                </div>
                <div class="stat-label">
                  Cập nhật cuối
                </div>
              </div>
            </div>
          </el-card>
        </el-col>
      </el-row>
    </div>

    <!-- Search and Filter Section -->
    <div class="filter-section">
      <el-card>
        <el-row :gutter="20" align="middle">
          <el-col :span="8">
            <el-input
              v-model="searchKeyword"
              placeholder="Tìm kiếm theo mã khách hàng, chi nhánh..."
              clearable
              @keyup.enter="searchData"
            >
              <template #prefix>
                <Icon icon="ph:magnifying-glass" />
              </template>
            </el-input>
          </el-col>

          <el-col :span="6">
            <el-date-picker
              v-model="dateRange"
              type="daterange"
              range-separator="đến"
              start-placeholder="Từ ngày"
              end-placeholder="Đến ngày"
              format="DD/MM/YYYY"
              value-format="YYYY-MM-DD"
              @change="filterByDate"
            />
          </el-col>

          <el-col :span="6">
            <el-select
              v-model="selectedBranch"
              placeholder="Chọn chi nhánh"
              clearable
              @change="filterByBranch"
            >
              <el-option
                v-for="branch in branchList"
                :key="branch.code"
                :label="branch.name"
                :value="branch.code"
              />
            </el-select>
          </el-col>

          <el-col :span="4">
            <el-button type="primary" @click="applyFilters">
              <Icon icon="ph:funnel" /> Lọc
            </el-button>
            <el-button @click="clearFilters">
              <Icon icon="ph:x" /> Xóa
            </el-button>
          </el-col>
        </el-row>
      </el-card>
    </div>

    <!-- Data Table -->
    <div class="table-section">
      <el-card>
        <template #header>
          <div class="table-header">
            <span class="table-title">Danh sách dữ liệu LN03</span>
            <div class="table-actions">
              <el-button
                size="small"
                :type="showTemporalHistory ? 'primary' : 'default'"
                @click="showTemporalHistory = !showTemporalHistory"
              >
                <Icon icon="ph:clock-clockwise" />
                {{ showTemporalHistory ? 'Ẩn' : 'Hiện' }} Lịch sử
              </el-button>
            </div>
          </div>
        </template>

        <el-table
          v-loading="loading"
          :data="tableData"
          stripe
          border
          height="600"
          @selection-change="handleSelectionChange"
        >
          <el-table-column type="selection" width="55" />

          <el-table-column
            prop="ngayDL"
            label="Ngày DL"
            width="120"
            sortable
          >
            <template #default="{ row }">
              {{ formatDate(row.ngayDL) }}
            </template>
          </el-table-column>

          <el-table-column prop="machinhAnh" label="Mã Chi nhánh" width="130" />
          <el-table-column prop="maKH" label="Mã KH" width="150" />
          <el-table-column prop="maCBTD" label="Mã CBTD" width="120" />

          <el-table-column
            prop="soTien1"
            label="Số tiền 1"
            width="150"
            sortable
          >
            <template #default="{ row }">
              {{ formatCurrency(row.soTien1) }}
            </template>
          </el-table-column>

          <el-table-column
            prop="soTien2"
            label="Số tiền 2"
            width="150"
            sortable
          >
            <template #default="{ row }">
              {{ formatCurrency(row.soTien2) }}
            </template>
          </el-table-column>

          <el-table-column
            prop="laiSuat"
            label="Lãi suất %"
            width="120"
            sortable
          >
            <template #default="{ row }">
              {{ formatPercentage(row.laiSuat) }}
            </template>
          </el-table-column>

          <!-- Additional columns for other data fields -->
          <el-table-column prop="column_18" label="Cột 18" width="100" />
          <el-table-column prop="column_19" label="Cột 19" width="100" />
          <el-table-column prop="column_20" label="Cột 20" width="100" />

          <el-table-column label="Thao tác" width="180" fixed="right">
            <template #default="{ row }">
              <el-button size="small" @click="viewRecord(row)">
                <Icon icon="ph:eye" /> Xem
              </el-button>
              <el-button size="small" type="primary" @click="editRecord(row)">
                <Icon icon="ph:pencil" /> Sửa
              </el-button>
              <el-button
                v-if="showTemporalHistory"
                size="small"
                type="info"
                @click="viewHistory(row)"
              >
                <Icon icon="ph:clock-clockwise" /> Lịch sử
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <!-- Pagination -->
        <div class="pagination-wrapper">
          <el-pagination
            v-model:current-page="currentPage"
            v-model:page-size="pageSize"
            :page-sizes="[10, 25, 50, 100]"
            :total="totalRecords"
            layout="total, sizes, prev, pager, next, jumper"
            @size-change="handleSizeChange"
            @current-change="handleCurrentChange"
          />
        </div>
      </el-card>
    </div>

    <!-- CSV Import Dialog -->
    <el-dialog v-model="showImportDialog" title="Import CSV File" width="600px">
      <div class="import-section">
        <el-upload
          ref="uploadRef"
          :auto-upload="false"
          :show-file-list="true"
          :limit="1"
          accept=".csv"
          @change="handleFileChange"
        >
          <template #trigger>
            <el-button type="primary">
              <Icon icon="ph:file-csv" /> Chọn file CSV
            </el-button>
          </template>

          <div class="upload-info">
            <p>• Chỉ hỗ trợ file CSV với encoding UTF-8</p>
            <p>• File phải có đúng cấu trúc 20 cột theo định dạng LN03</p>
            <p>• Dữ liệu sẽ được validate trước khi import</p>
          </div>
        </el-upload>

        <div class="import-options">
          <el-checkbox v-model="replaceExistingData">
            Thay thế dữ liệu hiện có (xóa dữ liệu cũ)
          </el-checkbox>
        </div>
      </div>

      <template #footer>
        <span class="dialog-footer">
          <el-button @click="showImportDialog = false">Hủy</el-button>
          <el-button
            type="primary"
            :loading="importing"
            :disabled="!selectedFile"
            @click="importCsvFile"
          >
            <Icon icon="ph:upload" /> Import
          </el-button>
        </span>
      </template>
    </el-dialog>

    <!-- Record Detail Dialog -->
    <el-dialog v-model="showDetailDialog" :title="`Chi tiết - ${selectedRecord?.maKH}`" width="800px">
      <div v-if="selectedRecord" class="record-detail">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="Ngày DL">
            {{ formatDate(selectedRecord.ngayDL) }}
          </el-descriptions-item>
          <el-descriptions-item label="Mã chi nhánh">
            {{ selectedRecord.machinhAnh }}
          </el-descriptions-item>
          <el-descriptions-item label="Mã khách hàng">
            {{ selectedRecord.maKH }}
          </el-descriptions-item>
          <el-descriptions-item label="Mã CBTD">
            {{ selectedRecord.maCBTD }}
          </el-descriptions-item>
          <el-descriptions-item label="Số tiền 1">
            {{ formatCurrency(selectedRecord.soTien1) }}
          </el-descriptions-item>
          <el-descriptions-item label="Số tiền 2">
            {{ formatCurrency(selectedRecord.soTien2) }}
          </el-descriptions-item>
          <el-descriptions-item label="Lãi suất">
            {{ formatPercentage(selectedRecord.laiSuat) }}
          </el-descriptions-item>
          <!-- Add more fields as needed -->
        </el-descriptions>
      </div>
    </el-dialog>

    <!-- Temporal History Dialog -->
    <el-dialog v-model="showHistoryDialog" title="Lịch sử thay đổi" width="900px">
      <div v-loading="loadingHistory" class="history-section">
        <el-timeline>
          <el-timeline-item
            v-for="(history, index) in temporalHistory"
            :key="index"
            :timestamp="formatDateTime(history.sysStartTime)"
            placement="top"
          >
            <el-card>
              <template #header>
                <div class="history-header">
                  <span>Phiên bản {{ temporalHistory.length - index }}</span>
                  <span class="history-period">
                    {{ formatDateTime(history.sysStartTime) }} -
                    {{ formatDateTime(history.sysEndTime) }}
                  </span>
                </div>
              </template>

              <div class="history-content">
                <!-- Show changed fields -->
                <el-descriptions size="small" :column="3" border>
                  <el-descriptions-item label="Mã KH">
                    {{ history.maKH }}
                  </el-descriptions-item>
                  <el-descriptions-item label="Số tiền 1">
                    {{ formatCurrency(history.soTien1) }}
                  </el-descriptions-item>
                  <el-descriptions-item label="Số tiền 2">
                    {{ formatCurrency(history.soTien2) }}
                  </el-descriptions-item>
                  <el-descriptions-item label="Lãi suất">
                    {{ formatPercentage(history.laiSuat) }}
                  </el-descriptions-item>
                </el-descriptions>
              </div>
            </el-card>
          </el-timeline-item>
        </el-timeline>
      </div>
    </el-dialog>
  </div>
</template>

<script setup>
import ln03Service from '@/api/ln03Service'
import { formatDate, formatDateTime } from '@/utils/dateFormat'
import { formatCurrency, formatNumber, formatPercentage } from '@/utils/numberFormat'
import { Icon } from '@iconify/vue'
import { ElMessage } from 'element-plus'
import { computed, onMounted, ref } from 'vue'

// ===== REACTIVE DATA =====
const loading = ref(false)
const importing = ref(false)
const exporting = ref(false)
const loadingHistory = ref(false)

// Table data
const tableData = ref([])
const totalRecords = ref(0)
const currentPage = ref(1)
const pageSize = ref(25)

// Statistics
const branchCount = ref(0)
const totalAmount = ref(0)
const lastUpdateDate = ref('')

// Search and filters
const searchKeyword = ref('')
const dateRange = ref([])
const selectedBranch = ref('')
const branchList = ref([])

// Dialog states
const showImportDialog = ref(false)
const showDetailDialog = ref(false)
const showHistoryDialog = ref(false)
const showTemporalHistory = ref(false)

// Import related
const selectedFile = ref(null)
const replaceExistingData = ref(false)
const uploadRef = ref(null)

// Selected data
const selectedRecord = ref(null)
const selectedRecords = ref([])
const temporalHistory = ref([])

// ===== COMPUTED PROPERTIES =====
const currentFilters = computed(() => ({
  keyword: searchKeyword.value,
  dateRange: dateRange.value,
  branchCode: selectedBranch.value,
  page: currentPage.value,
  pageSize: pageSize.value,
}))

// ===== LIFECYCLE HOOKS =====
onMounted(async () => {
  await initializeData()
})

// ===== METHODS =====
const initializeData = async () => {
  await Promise.all([
    loadTableData(),
    loadStatistics(),
    loadBranchList(),
  ])
}

const loadTableData = async () => {
  try {
    loading.value = true
    const params = {
      page: currentPage.value,
      pageSize: pageSize.value,
      keyword: searchKeyword.value || undefined,
      branchCode: selectedBranch.value || undefined,
    }

    if (dateRange.value && dateRange.value.length === 2) {
      params.startDate = dateRange.value[0]
      params.endDate = dateRange.value[1]
    }

    const response = await ln03Service.getRecords(params)

    if (response.success) {
      tableData.value = response.data.items || response.data
      totalRecords.value = response.data.totalCount || response.data.length
    } else {
      ElMessage.error(response.message || 'Không thể tải dữ liệu')
    }
  } catch (error) {
    console.error('Error loading table data:', error)
    ElMessage.error('Lỗi khi tải dữ liệu bảng')
  } finally {
    loading.value = false
  }
}

const loadStatistics = async () => {
  try {
    const [countResponse, summaryResponse] = await Promise.all([
      ln03Service.getCount(),
      ln03Service.getSummary(),
    ])

    if (countResponse.success) {
      totalRecords.value = countResponse.data
    }

    if (summaryResponse.success) {
      const summary = summaryResponse.data
      branchCount.value = summary.branchCount || 0
      totalAmount.value = summary.totalAmount || 0
      lastUpdateDate.value = summary.lastUpdateDate ?
        formatDate(summary.lastUpdateDate) : 'N/A'
    }
  } catch (error) {
    console.error('Error loading statistics:', error)
  }
}

const loadBranchList = async () => {
  try {
    // This would typically come from a separate branch API
    // For now, we'll extract unique branches from the data
    const response = await ln03Service.getRecords({ pageSize: 1000 })

    if (response.success) {
      const branches = new Set()
      const data = response.data.items || response.data

      data.forEach(record => {
        if (record.machinhAnh) {
          branches.add(record.machinhAnh)
        }
      })

      branchList.value = Array.from(branches).map(code => ({
        code,
        name: `Chi nhánh ${code}`,
      }))
    }
  } catch (error) {
    console.error('Error loading branch list:', error)
  }
}

const refreshData = async () => {
  await initializeData()
  ElMessage.success('Đã làm mới dữ liệu')
}

const searchData = async () => {
  currentPage.value = 1
  await loadTableData()
}

const filterByDate = async () => {
  currentPage.value = 1
  await loadTableData()
}

const filterByBranch = async () => {
  currentPage.value = 1
  await loadTableData()
}

const applyFilters = async () => {
  currentPage.value = 1
  await loadTableData()
}

const clearFilters = async () => {
  searchKeyword.value = ''
  dateRange.value = []
  selectedBranch.value = ''
  currentPage.value = 1
  await loadTableData()
}

const handleSizeChange = async (size) => {
  pageSize.value = size
  currentPage.value = 1
  await loadTableData()
}

const handleCurrentChange = async (page) => {
  currentPage.value = page
  await loadTableData()
}

const handleSelectionChange = (selection) => {
  selectedRecords.value = selection
}

const viewRecord = (record) => {
  selectedRecord.value = record
  showDetailDialog.value = true
}

const editRecord = (record) => {
  // TODO: Implement edit functionality
  ElMessage.info('Tính năng chỉnh sửa sẽ được phát triển')
}

const viewHistory = async (record) => {
  try {
    loadingHistory.value = true
    showHistoryDialog.value = true

    const response = await ln03Service.getTemporalHistory(record.id)

    if (response.success) {
      temporalHistory.value = response.data
    } else {
      ElMessage.error('Không thể tải lịch sử thay đổi')
    }
  } catch (error) {
    console.error('Error loading history:', error)
    ElMessage.error('Lỗi khi tải lịch sử')
  } finally {
    loadingHistory.value = false
  }
}

const handleFileChange = (file) => {
  selectedFile.value = file.raw
}

const importCsvFile = async () => {
  if (!selectedFile.value) {
    ElMessage.warning('Vui lòng chọn file CSV')
    return
  }

  try {
    importing.value = true

    const response = await ln03Service.importCsv(
      selectedFile.value,
      replaceExistingData.value,
    )

    if (response.success) {
      const result = response.data
      ElMessage.success(
        `Import thành công: ${result.successCount} bản ghi được thêm`,
      )

      showImportDialog.value = false
      uploadRef.value?.clearFiles()
      selectedFile.value = null

      await refreshData()
    } else {
      ElMessage.error(response.message || 'Import thất bại')
    }
  } catch (error) {
    console.error('Import error:', error)
    ElMessage.error('Lỗi khi import dữ liệu')
  } finally {
    importing.value = false
  }
}

const exportData = async () => {
  try {
    exporting.value = true

    const filters = {
      keyword: searchKeyword.value || undefined,
      branchCode: selectedBranch.value || undefined,
    }

    if (dateRange.value && dateRange.value.length === 2) {
      filters.startDate = dateRange.value[0]
      filters.endDate = dateRange.value[1]
    }

    const blob = await ln03Service.exportToCsv(filters)

    // Create download link
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.style.display = 'none'
    link.href = url
    link.download = `LN03_Export_${new Date().toISOString().slice(0, 10)}.csv`

    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)

    ElMessage.success('Export dữ liệu thành công')
  } catch (error) {
    console.error('Export error:', error)
    ElMessage.error('Lỗi khi export dữ liệu')
  } finally {
    exporting.value = false
  }
}
</script>

<style scoped>
.ln03-management {
  padding: 20px;
}

.header-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.page-title {
  margin: 0;
  font-size: 28px;
  font-weight: 600;
  color: #1f2937;
  display: flex;
  align-items: center;
  gap: 12px;
}

.page-description {
  margin: 8px 0 0 0;
  color: #6b7280;
  font-size: 14px;
}

.action-buttons {
  display: flex;
  gap: 12px;
}

.stats-section {
  margin-bottom: 24px;
}

.stat-card {
  border: none;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.stat-content {
  display: flex;
  align-items: center;
  gap: 16px;
}

.stat-icon {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
  color: white;
}

.stat-icon.total {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.stat-icon.branch {
  background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
}

.stat-icon.amount {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
}

.stat-icon.date {
  background: linear-gradient(135deg, #43e97b 0%, #38f9d7 100%);
}

.stat-info {
  flex: 1;
}

.stat-value {
  font-size: 24px;
  font-weight: 700;
  color: #1f2937;
  line-height: 1;
}

.stat-label {
  font-size: 14px;
  color: #6b7280;
  margin-top: 4px;
}

.filter-section {
  margin-bottom: 24px;
}

.table-section {
  margin-bottom: 24px;
}

.table-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.table-title {
  font-size: 16px;
  font-weight: 600;
  color: #1f2937;
}

.pagination-wrapper {
  margin-top: 20px;
  display: flex;
  justify-content: flex-end;
}

.import-section {
  padding: 20px 0;
}

.upload-info {
  margin-top: 16px;
  padding: 12px;
  background: #f3f4f6;
  border-radius: 8px;
}

.upload-info p {
  margin: 4px 0;
  color: #6b7280;
  font-size: 14px;
}

.import-options {
  margin-top: 16px;
  padding-top: 16px;
  border-top: 1px solid #e5e7eb;
}

.record-detail {
  margin-top: 20px;
}

.history-section {
  max-height: 500px;
  overflow-y: auto;
}

.history-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.history-period {
  font-size: 12px;
  color: #6b7280;
}

.history-content {
  margin-top: 12px;
}

/* Element Plus overrides */
:deep(.el-card__body) {
  padding: 20px;
}

:deep(.el-table th) {
  background-color: #f8fafc;
  color: #374151;
  font-weight: 600;
}

:deep(.el-pagination) {
  padding: 0;
}
</style>
