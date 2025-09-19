<template>
  <div class="unit-kpi-assignment">
    <h1 class="text-primary">🏢 Giao khoán KPI Chi nhánh</h1>

    <!-- Thông báo lỗi -->
    <div v-if="errorMessage" class="alert-agribank alert-danger">
      <strong>❌ Lỗi:</strong> {{ errorMessage }}
    </div>

    <!-- Thông báo thành công -->
    <div v-if="successMessage" class="alert-agribank alert-success">
      <strong>✅ Thành công:</strong> {{ successMessage }}
    </div>

    <!-- Trạng thái loading -->
    <div v-if="loading" class="loading-agribank">
      <div class="spinner-agribank"></div>
      <p>Đang tải dữ liệu...</p>
    </div>

    <!-- Layout chính -->
    <div v-if="!loading" class="content-container">
      <!-- Period Selection -->
      <div class="card-agribank">
        <div class="card-header">
          <h3 class="card-title">📅 Chọn kỳ khoán</h3>
        </div>
        <div class="card-body">
          <div class="form-group">
            <label class="form-label">Kỳ khoán:</label>
            <select v-model="selectedPeriodId" @change="onPeriodChange" class="form-control">
              <option value="">-- Chọn kỳ khoán --</option>
              <option v-for="period in khoanPeriods" :key="period.Id || period.Id" :value="period.Id || period.Id">
                {{ period.Name || period.Name }} ({{ formatDate(period.StartDate || period.startDate) }} - {{ formatDate(period.EndDate || period.endDate) }})
              </option>
            </select>
          </div>
        </div>
      </div>

      <!-- Branch Selection -->
      <div class="card-agribank" v-if="selectedPeriodId">
        <div class="card-header">
          <h3 class="card-title">🔍 Lọc chi nhánh</h3>
        </div>
        <div class="card-body">
          <div class="form-group">
            <label class="form-label">🏢 Chi nhánh:</label>
            <select v-model="selectedBranchId" @change="onBranchChange" class="form-control">
              <option value="">-- Chọn chi nhánh --</option>
              <optgroup label="Chi nhánh CNL1">
                <option v-for="unit in cnl1Units" :key="unit.Id" :value="unit.Id">
                  🏢 {{ unit.Name }} ({{ unit.code }})
                </option>
              </optgroup>
              <optgroup label="Chi nhánh CNL2" v-if="cnl2Units.length > 0">
                <option v-for="unit in cnl2Units" :key="unit.Id" :value="unit.Id">
                  🏢 {{ unit.Name }} ({{ unit.code }}) - {{ getParentUnitCode(unit.parentUnitId) }}
                </option>
              </optgroup>
            </select>
          </div>

          <div class="alert-agribank alert-info" v-if="selectedBranch">
            <strong>📊 Đã chọn:</strong>
            Chi nhánh "{{ selectedBranch.Name }}" ({{ selectedBranch.code }})
            → <strong>{{ availableKpiIndicators.length }}</strong> chỉ tiêu KPI
          </div>
        </div>
      </div>

      <!-- KPI Assignment Section -->
      <div v-if="selectedBranchId" class="card-agribank">
        <div class="card-header">
          <h3 class="card-title">📊 Giao khoán KPI chi nhánh</h3>
          <div v-if="availableKpiIndicators.length > 0">
            <span class="badge-agribank badge-info" style="margin-right: 8px;">{{ availableKpiIndicators.length }} chỉ tiêu</span>
            <span class="badge-agribank badge-accent">{{ totalMaxScore }} điểm tối đa</span>
          </div>
        </div>

        <div class="card-body">
          <!-- KPI Status Messages -->
          <div v-if="availableKpiIndicators.length === 0" class="alert-agribank alert-info">
            <strong>ℹ️ Thông tin:</strong>
            <span v-if="loading">Đang tải danh sách chỉ tiêu KPI cho chi nhánh...</span>
            <span v-else>Chưa có chỉ tiêu KPI nào được tải. Vui lòng chọn chi nhánh để xem danh sách KPI.
              <button @click="onBranchChange" class="btn-agribank btn-outline" style="margin-left: 8px; padding: 4px 8px; font-size: 0.75rem;">
                🔄 Thử lại
              </button>
            </span>
          </div>

          <!-- KPI Table -->
          <table v-if="availableKpiIndicators.length > 0" class="table-agribank">
            <thead>
              <tr>
                <th style="width: 40%;">📊 Chỉ tiêu KPI</th>
                <th style="width: 10%;">⭐ Điểm</th>
                <th style="width: 20%;">🎯 Mục tiêu</th>
                <th style="width: 15%;">📏 Đơn vị</th>
                <th style="width: 15%;">⚡ Thao tác</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(indicator, index) in availableKpiIndicators" :key="indicator.Id">
                <td>
                  <div style="display: flex; align-items: center; gap: 8px;">
                    <span class="badge-agribank badge-primary">{{ index + 1 }}</span>
                    <span class="font-weight-semibold">{{ indicator.indicatorName }}</span>
                  </div>
                </td>
                <td style="text-align: center;">
                  <span class="badge-agribank badge-accent">{{ indicator.maxScore }}</span>
                </td>
                <td>
                  <input
                    type="text"
                    :value="formatNumber(kpiTargets[indicator.Id] || 0)"
                    placeholder="Nhập mục tiêu"
                    class="form-control"
                    style="font-size: 0.85rem; padding: 8px 12px;"
                    @input="(e) => handleKpiTargetInput(e, indicator.Id)"
                    @blur="(e) => handleKpiTargetBlur(e, indicator.Id)"
                  />
                </td>
                <td style="text-align: center;">
                  <span class="badge-agribank badge-secondary">{{ indicator.unit || 'N/A' }}</span>
                </td>
                <td>
                  <div style="display: flex; gap: 4px; justify-content: center;">
                    <button
                      @click="moveIndicatorUp(index)"
                      :disabled="index === 0"
                      class="btn-agribank btn-outline"
                      style="padding: 4px 8px; font-size: 0.75rem;"
                      title="Di chuyển lên"
                    >
                      ↑
                    </button>
                    <button
                      @click="moveIndicatorDown(index)"
                      :disabled="index === availableKpiIndicators.length - 1"
                      class="btn-agribank btn-outline"
                      style="padding: 4px 8px; font-size: 0.75rem;"
                      title="Di chuyển xuống"
                    >
                      ↓
                    </button>
                    <button
                      @click="editIndicator(indicator)"
                      class="btn-agribank btn-outline"
                      style="padding: 4px 8px; font-size: 0.75rem;"
                      title="Chỉnh sửa"
                    >
                      ✏️
                    </button>
                    <button
                      @click="clearIndicatorTarget(indicator.Id)"
                      class="btn-agribank btn-outline"
                      style="padding: 4px 8px; font-size: 0.75rem; color: var(--danger-color); border-color: var(--danger-color);"
                      title="Xóa mục tiêu"
                    >
                      🗑️
                    </button>
                  </div>
                </td>
              </tr>
              <!-- Total Row -->
              <tr style="background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%); border-top: 2px solid var(--agribank-primary);">
                <td><strong class="text-primary">🎯 TỔNG CỘNG</strong></td>
                <td style="text-align: center;"><strong class="badge-agribank badge-primary">{{ totalMaxScore }}</strong></td>
                <td><strong class="text-secondary">{{ getFilledTargetsCount() }} mục tiêu</strong></td>
                <td style="text-align: center;"><strong class="badge-agribank badge-primary">Điểm</strong></td>
                <td></td>
              </tr>
            </tbody>
          </table>
        </div>

        <div v-if="availableKpiIndicators.length > 0" class="card-body" style="text-align: center; padding-top: 0;">
          <button @click="assignKPI" :disabled="isAssigning" class="btn-agribank btn-primary" style="font-size: 1rem; padding: 16px 32px;">
            <span>{{ isAssigning ? '⏳' : '📋' }}</span>
            <span style="margin-left: 8px;">{{ isAssigning ? 'Đang giao khoán...' : 'Giao KPI cho chi nhánh' }}</span>
          </button>
        </div>
      </div>

      <!-- Current Assignments Display -->
      <div v-if="currentAssignments.length > 0" class="card-agribank">
        <div class="card-header">
          <h3 class="card-title">📋 Giao khoán hiện tại</h3>
          <span class="badge-agribank badge-success">{{ currentAssignments.length }} bản ghi</span>
        </div>

        <div class="card-body">
          <table class="table-agribank">
            <thead>
              <tr>
                <th style="width: 40%;">📊 Chỉ tiêu KPI</th>
                <th style="width: 15%;">🎯 Mục tiêu</th>
                <th style="width: 15%;">📈 Thực tế</th>
                <th style="width: 15%;">⭐ Điểm</th>
                <th style="width: 15%;">📅 Ngày giao</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="assignment in currentAssignments" :key="assignment.Id">
                <td>
                  <span class="font-weight-semibold">{{ assignment.indicatorName }}</span>
                </td>
                <td style="text-align: center;">
                  <span class="badge-agribank badge-info">{{ assignment.targetValue || 'N/A' }}</span>
                </td>
                <td style="text-align: center;">
                  <span class="badge-agribank badge-secondary">{{ assignment.actualValue || 'Chưa có' }}</span>
                </td>
                <td style="text-align: center;">
                  <span class="badge-agribank badge-accent">{{ assignment.score || 'N/A' }}</span>
                </td>
                <td style="text-align: center;">
                  <span class="text-secondary">{{ formatDate(assignment.assignedDate) }}</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import api from '../services/api.js';
import { logApiResponse, normalizeNetArray } from '../utils/apiHelpers.js';
import { useNumberInput } from '../utils/numberFormat';

// Router instance
const router = useRouter();

// 🔢 Initialize number input utility
const { handleInput, handleBlur, formatNumber, parseFormattedNumber } = useNumberInput({
  maxDecimalPlaces: 2,
  allowNegative: false
});

// Reactive data
const loading = ref(false)
const isAssigning = ref(false)
const errorMessage = ref('')
const successMessage = ref('')
const khoanPeriods = ref([])
const selectedPeriodId = ref('')
const selectedBranchId = ref('')
const units = ref([])
const kpiTables = ref([])
const availableKpiIndicators = ref([])
const kpiTargets = ref({})
const currentAssignments = ref([])

// Computed properties
// CNL1: Chỉ có Hội Sở (1 đơn vị)
const cnl1Units = computed(() => {
  const filtered = units.value.filter(unit => {
    const type = (unit.Type || '').toUpperCase()
    return type === 'CNL1'
  }).sort((a, b) => {
    // Primary sort: SortOrder (nulls last)
    const sortOrderA = a.sortOrder ?? Number.MAX_SAFE_INTEGER;
    const sortOrderB = b.sortOrder ?? Number.MAX_SAFE_INTEGER;

    if (sortOrderA !== sortOrderB) {
      return sortOrderA - sortOrderB;
    }

    // Secondary sort: Name
    return (a.Name || '').localeCompare(b.Name || '');
  })

  console.log('🏢 CNL1 Units:', filtered.length, filtered.map(u => u.Name))
  return filtered
})

// CNL2: 8 CN + 5 PGD = 13 đơn vị (sắp xếp theo sortOrder)
const cnl2Units = computed(() => {
  const filtered = units.value
    .filter(unit => {
      const type = (unit.Type || '').toUpperCase()
      return type === 'CNL2'
    })
    .sort((a, b) => {
      // Sắp xếp theo sortOrder
      const sortOrderA = a.sortOrder ?? Number.MAX_SAFE_INTEGER;
      const sortOrderB = b.sortOrder ?? Number.MAX_SAFE_INTEGER;

      if (sortOrderA !== sortOrderB) {
        return sortOrderA - sortOrderB;
      }

      // Nếu sortOrder bằng nhau thì sắp xếp theo tên
      return (a.Name || '').localeCompare(b.Name || '');
    })

  console.log('🏢 CNL2 Units:', filtered.length, filtered.map(u => u.Name))
  return filtered
})

const selectedBranch = computed(() => {
  if (!selectedBranchId.value) return null
  return units.value.find(u => u.Id === parseInt(selectedBranchId.value))
})

const totalMaxScore = computed(() => {
  return availableKpiIndicators.value.reduce((sum, indicator) => sum + (indicator.maxScore || 0), 0)
})

// Methods
async function loadInitialData() {
  loading.value = true
  errorMessage.value = ''

  try {
    const [periodsResponse, tablesResponse] = await Promise.all([
      api.get('/KhoanPeriods'),
      api.get('/KpiAssignment/tables')
    ])

    khoanPeriods.value = periodsResponse.data || []

    // Sử dụng danh sách 15 chi nhánh chuẩn hóa giống CalculationDashboard (cập nhật tên mới)
    units.value = [
      { id: 1, name: 'Hội Sở', code: 'HoiSo', type: 'CNL1', sortOrder: 1 },
      { id: 2, name: 'CN Bình Lư', code: 'CnBinhLu', type: 'CNL2', sortOrder: 2 },
      { id: 3, name: 'CN Phong Thổ', code: 'CnPhongTho', type: 'CNL2', sortOrder: 3 },
      { id: 4, name: 'CN Sin Hồ', code: 'CnSinHo', type: 'CNL2', sortOrder: 4 },
      { id: 5, name: 'CN Bum Tở', code: 'CnBumTo', type: 'CNL2', sortOrder: 5 },
      { id: 6, name: 'CN Than Uyên', code: 'CnThanUyen', type: 'CNL2', sortOrder: 6 },
      { id: 7, name: 'CN Đoàn Kết', code: 'CnDoanKet', type: 'CNL2', sortOrder: 7 },
      { id: 8, name: 'CN Tân Uyên', code: 'CnTanUyen', type: 'CNL2', sortOrder: 8 },
      { id: 9, name: 'CN Nậm Hàng', code: 'CnNamHang', type: 'CNL2', sortOrder: 9 },
      { id: 10, name: 'CN Phong Thổ - PGD Số 5', code: 'CnPhongThoPgdSo5', type: 'CNL2', sortOrder: 10, parentUnitId: 3 },
      { id: 11, name: 'CN Than Uyên - PGD Số 6', code: 'CnThanUyenPgdSo6', type: 'CNL2', sortOrder: 11, parentUnitId: 6 },
      { id: 12, name: 'CN Đoàn Kết - PGD Số 1', code: 'CnDoanKetPgdSo1', type: 'CNL2', sortOrder: 12, parentUnitId: 7 },
      { id: 13, name: 'CN Đoàn Kết - PGD Số 2', code: 'CnDoanKetPgdSo2', type: 'CNL2', sortOrder: 13, parentUnitId: 7 },
      { id: 14, name: 'CN Tân Uyên - PGD Số 3', code: 'CnTanUyenPgdSo3', type: 'CNL2', sortOrder: 14, parentUnitId: 8 }
    ]

    kpiTables.value = tablesResponse.data || []

    console.log('✅ Loaded periods:', khoanPeriods.value.length)
    console.log('✅ Loaded units:', units.value.length)
    console.log('✅ Units detail:', units.value.map(u => `${u.Name} (${u.Type})`))
    console.log('✅ Loaded KPI tables:', kpiTables.value.length)

  } catch (error) {
    console.error('Error loading initial data:', error)
    errorMessage.value = 'Không thể tải dữ liệu: ' + (error.response?.data?.message || error.message)
  } finally {
    loading.value = false
  }
}

function onPeriodChange() {
  console.log('📅 Period changed to:', selectedPeriodId.value)
  selectedBranchId.value = ''
  availableKpiIndicators.value = []
  kpiTargets.value = {}
  currentAssignments.value = []
  errorMessage.value = ''
}

// Helper function để xử lý string operations an toàn
function safeStringIncludes(str, searchString) {
  return str && typeof str === 'string' && str.toLowerCase().includes(searchString.toLowerCase())
}

// Helper function để xử lý type checking an toàn
function safeStringEquals(str1, str2) {
  return str1 && typeof str1 === 'string' && str1 === str2
}

async function onBranchChange() {
  console.log('🏢 Branch changed to:', selectedBranchId.value)
  availableKpiIndicators.value = []
  kpiTargets.value = {}
  currentAssignments.value = []
  clearMessages()

  if (!selectedBranchId.value) {
    console.log('❌ No branch selected, clearing data')
    return
  }

  try {
    // Load KPI indicators for branch
    const branch = selectedBranch.value
    if (!branch) {
      console.log('❌ Branch not found in units array')
      return
    }

    console.log('📍 Selected branch details:', {
      id: branch.Id,
      name: branch.Name,
      code: branch.code,
      type: branch.type
    })

    // Find appropriate KPI table based on branch type
    let kpiTable = null
    const branchType = (branch.Type && typeof branch.Type === 'string' ? branch.Type : '').toUpperCase()
    console.log('🔍 Branch type:', branchType)
    console.log('📊 Available KPI tables:', kpiTables.value.length)

    // Log all available branch tables
    const branchTables = kpiTables.value.filter(t =>
      t && t.category && (
        t.category === 'Dành cho Chi nhánh' ||
        safeStringIncludes(t.category, 'chi nhánh') ||
        safeStringIncludes(t.category, 'branch') ||
        safeStringIncludes(t.category, 'unit')
      )
    )
    console.log('🏢 Branch KPI tables found:', branchTables.length)
    branchTables.forEach(table => {
      console.log(`   📊 ${table.tableName} (ID: ${table.Id}, Type: ${table.tableType || 'N/A'}, Category: ${table.category})`)
    })

    // Match branch type with KPI table
    if (branchType === 'CNL1') {
      // For CNL1, try multiple matching strategies
      kpiTable = branchTables.find(t =>
        safeStringEquals(t.tableType, 'HoiSo') ||
        safeStringEquals(t.tableType, 'CnBinhLu') ||
        safeStringIncludes(t.tableName, 'hội sở (7800)') ||
        safeStringIncludes(t.tableName, 'cnl1') ||
        safeStringIncludes(t.tableName, 'chi nhánh cấp 1')
      )
      console.log('🎯 Looking for CNL1 table, found:', kpiTable?.tableName || 'None')
    } else if (branchType === 'CNL2') {
      // For CNL2, try multiple matching strategies
      kpiTable = branchTables.find(t =>
        safeStringEquals(t.tableType, 'GiamdocCnl2') ||
        safeStringEquals(t.tableType, 'CnPhongTho') ||
        safeStringIncludes(t.tableName, 'giám đốc cnl2') ||
        safeStringIncludes(t.tableName, 'cnl2') ||
        safeStringIncludes(t.tableName, 'chi nhánh cấp 2')
      )
      console.log('🎯 Looking for CNL2 table, found:', kpiTable?.tableName || 'None')
    }

    // If still no match, try fallback patterns
    if (!kpiTable) {
      console.log('🔍 No direct match, trying fallback patterns...')
      // Look for any table containing branch-related keywords
      kpiTable = branchTables.find(t =>
        safeStringIncludes(t.tableName, 'chi nhánh') ||
        safeStringIncludes(t.tableName, 'cnl') ||
        safeStringIncludes(t.tableType, 'cnl')
      )
      console.log('🎯 Fallback search found:', kpiTable?.tableName || 'None')
    }

    // Fallback to first available branch table
    if (!kpiTable && branchTables.length > 0) {
      kpiTable = branchTables[0]
      console.log('⚠️ No specific match, using first branch table as fallback:', kpiTable.tableName)
    }

    if (kpiTable) {
      console.log('✅ Selected KPI table:', {
        id: kpiTable.Id,
        name: kpiTable.tableName,
        type: kpiTable.tableType,
        category: kpiTable.category
      })

      console.log('🔄 Loading KPI indicators...')
      const response = await api.get(`/KpiAssignment/tables/${kpiTable.Id}`)

      // Use helper function to log API response
      logApiResponse(`/KpiAssignment/tables/${kpiTable.Id}`, response, 'indicators')

      if (response.data && response.data.indicators) {
        // Use helper function to normalize .NET array format
        availableKpiIndicators.value = normalizeNetArray(response.data.indicators)
        console.log('✅ Loaded KPI indicators:', availableKpiIndicators.value.length)

        // Log sample indicators
        if (availableKpiIndicators.value.length > 0) {
          console.log('📋 Sample indicators:')
          availableKpiIndicators.value.slice(0, 3).forEach((ind, idx) => {
            console.log(`   ${idx + 1}. ${ind.indicatorName} (${ind.maxScore} points, ${ind.unit || 'N/A'})`)
          })
        }
      } else {
        console.log('⚠️ API response missing indicators array')
        availableKpiIndicators.value = []
      }
    } else {
      console.log('❌ No suitable KPI table found for branch type:', branchType)
      console.log('Available tables:', kpiTables.value.map(t => ({ id: t.Id, name: t.tableName, category: t.category, type: t.tableType })))
      errorMessage.value = `Không tìm thấy bảng KPI phù hợp cho chi nhánh loại ${branchType}. Có ${kpiTables.value.length} bảng KPI tổng cộng.`
    }

    // Load current assignments
    await loadCurrentAssignments()

  } catch (error) {
    console.error('❌ Error loading branch KPI data:', error)
    console.error('Error details:', {
      status: error.response?.Status,
      message: error.response?.data?.message || error.message,
      url: error.config?.url
    })
    errorMessage.value = 'Không thể tải dữ liệu KPI cho chi nhánh: ' + (error.response?.data?.message || error.message)
    availableKpiIndicators.value = []
  }
}

async function loadCurrentAssignments() {
  if (!selectedBranchId.value || !selectedPeriodId.value) return

  try {
    // This would be the API call to get current unit KPI assignments
    // For now, we'll show empty as the backend might not have this endpoint yet
    currentAssignments.value = []
  } catch (error) {
    console.error('Error loading current assignments:', error)
    // Don't show error for this as it's not critical
  }
}

function getParentUnitCode(parentUnitId) {
  if (!parentUnitId) return ''
  const parent = units.value.find(u => u.Id === parentUnitId)
  return parent ? parent.code : ''
}

function getFilledTargetsCount() {
  return Object.values(kpiTargets.value).filter(v => v !== null && v !== undefined && v !== '').length
}

function moveIndicatorUp(index) {
  if (index > 0) {
    const temp = availableKpiIndicators.value[index]
    availableKpiIndicators.value[index] = availableKpiIndicators.value[index - 1]
    availableKpiIndicators.value[index - 1] = temp
  }
}

function moveIndicatorDown(index) {
  if (index < availableKpiIndicators.value.length - 1) {
    const temp = availableKpiIndicators.value[index]
    availableKpiIndicators.value[index] = availableKpiIndicators.value[index + 1]
    availableKpiIndicators.value[index + 1] = temp
  }
}

function editIndicator(indicator) {
  console.log('Edit indicator:', indicator)
  // Implement edit functionality
}

function clearIndicatorTarget(indicatorId) {
  delete kpiTargets.value[indicatorId]
}

async function assignKPI() {
  if (!selectedBranchId.value) {
    errorMessage.value = 'Vui lòng chọn chi nhánh'
    return
  }

  if (!selectedPeriodId.value) {
    errorMessage.value = 'Vui lòng chọn kỳ khoán'
    return
  }

  const targets = Object.entries(kpiTargets.value)
    .filter(([_, value]) => value !== null && value !== undefined && value !== '')
    .map(([indicatorId, value]) => ({
      indicatorId: parseInt(indicatorId),
      targetValue: parseFloat(value),
      notes: ''
    }))

  if (targets.length === 0) {
    errorMessage.value = 'Vui lòng nhập ít nhất một mục tiêu'
    return
  }

  isAssigning.value = true
  clearMessages()

  try {
    // This would be the API call to assign KPI to unit/branch
    // For now, we'll simulate success
    await new Promise(resolve => setTimeout(resolve, 1000))

    successMessage.value = `Đã giao khoán KPI thành công cho chi nhánh "${selectedBranch.value.Name}" với ${targets.length} chỉ tiêu`

    // Reset targets
    kpiTargets.value = {}

    // Reload current assignments
    await loadCurrentAssignments()

  } catch (error) {
    console.error('Error assigning unit KPI:', error)
    errorMessage.value = 'Lỗi khi giao khoán KPI: ' + (error.response?.data?.message || error.message)
  } finally {
    isAssigning.value = false
  }
}

function clearMessages() {
  errorMessage.value = ''
  successMessage.value = ''
}

function formatDate(dateString) {
  if (!dateString) return 'N/A'
  const date = new Date(dateString)
  return date.toLocaleDateString('vi-VN')
}

// Number input handlers for KPI targets
const handleKpiTargetInput = (event, indicatorId) => {
  const formattedValue = handleInput(event);
  event.target.value = formattedValue;
  kpiTargets.value[indicatorId] = parseFormattedNumber(formattedValue);
};

const handleKpiTargetBlur = (event, indicatorId) => {
  const formattedValue = handleBlur(event);
  event.target.value = formattedValue;
  kpiTargets.value[indicatorId] = parseFormattedNumber(formattedValue);
};

onMounted(() => {
  loadInitialData()
})

// Watcher để tự động load KPI khi chọn chi nhánh
watch([selectedPeriodId, selectedBranchId], ([newPeriodId, newBranchId]) => {
  console.log('👀 Watcher triggered:', { periodId: newPeriodId, branchId: newBranchId })

  if (newPeriodId && newBranchId) {
    console.log('✅ Both period and branch selected, loading KPI data...')
    // Force reload KPI data when both are selected
    setTimeout(() => {
      onBranchChange()
    }, 100)
  }
}, { immediate: false })
</script>

<style scoped>
.unit-kpi-assignment {
  padding: 20px;
  max-width: 1400px;
  margin: 0 auto;
}

.content-container {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group {
  margin-bottom: 0;
}

@media (max-width: 768px) {
  .card-header {
    flex-direction: column;
    align-items: flex-start !important;
    gap: 12px;
  }

  .table-agribank {
    font-size: 0.8rem;
  }

  .table-agribank th,
  .table-agribank td {
    padding: 8px 4px;
  }
}
</style>
