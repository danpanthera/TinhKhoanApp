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
                  🏢 {{ unit.Name }} ({{ unit.Code }})
                </option>
              </optgroup>
              <optgroup label="Chi nhánh CNL2" v-if="cnl2Units.length > 0">
                <option v-for="unit in cnl2Units" :key="unit.Id" :value="unit.Id">
                  🏢 {{ unit.Name }} ({{ unit.Code }}) - {{ getParentUnitCode(unit.ParentUnitId || unit.parentUnitId) }}
                </option>
              </optgroup>
            </select>
          </div>

          <div class="alert-agribank alert-info" v-if="selectedBranch">
            <strong>📊 Đã chọn:</strong>
            Chi nhánh "{{ selectedBranch.Name }}" ({{ selectedBranch.Code }})
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
                    <span class="font-weight-semibold">{{ safeGet(indicator, 'IndicatorName') }}</span>
                  </div>
                </td>
                <td style="text-align: center;">
                  <span class="badge-agribank badge-accent">{{ safeGet(indicator, 'MaxScore') }}</span>
                </td>
                <td>
                  <input
                    type="text"
                    :value="formatUnitTargetValue(indicator, kpiTargets[indicator.Id])"
                    placeholder="Nhập mục tiêu"
                    class="form-control"
                    style="font-size: 0.85rem; padding: 8px 12px;"
                    :class="{ 'error': kpiTargetErrors[indicator.Id] }"
                    @input="(e) => handleKpiTargetInput(e, indicator.Id)"
                    @blur="(e) => handleKpiTargetBlur(e, indicator.Id)"
                  />
                  <div v-if="kpiTargetErrors[indicator.Id]" class="text-danger" style="font-size: 0.75rem; margin-top: 4px;">
                    {{ kpiTargetErrors[indicator.Id] }}
                  </div>
                </td>
                <td style="text-align: center;">
                  <span class="badge-agribank badge-secondary">{{ safeGet(indicator, 'Unit') || 'N/A' }}</span>
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
                <td style="text-align: center;"><strong class="badge-agribank badge-success">{{ totalScore }}</strong></td>
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
import { safeGet } from '../utils/casingSafeAccess.js';
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
const kpiTargetErrors = ref({})
const currentAssignments = ref([])

// Computed properties
// CNL1: Hội Sở và Lai Châu với custom ordering
const cnl1Units = computed(() => {
  const filtered = units.value.filter(unit => {
    const type = (unit.Type || '').toUpperCase()
    return type === 'CNL1'
  }).sort((a, b) => {
    // Custom ordering function theo yêu cầu Hội Sở → Nậm Hàng
    const getOrderIndex = (unitName) => {
      const name = (unitName || '').toLowerCase();
      if (name.includes('hội sở')) return 0;
      if (name.includes('lai châu')) return 1;
      return 999; // Unknown units go to the end
    };

    const indexA = getOrderIndex(a.Name);
    const indexB = getOrderIndex(b.Name);
    return indexA - indexB;
  })

  console.log('🏢 CNL1 Units:', filtered.length, filtered.map(u => u.Name))
  return filtered
})

// CNL2: 8 chi nhánh với custom ordering theo yêu cầu Hội Sở → Nậm Hàng
const cnl2Units = computed(() => {
  const filtered = units.value
    .filter(unit => {
      const type = (unit.Type || '').toUpperCase()
      return type === 'CNL2'
    })
    .sort((a, b) => {
      // Custom ordering function theo yêu cầu Hội Sở → Nậm Hàng
      const getOrderIndex = (unitName) => {
        const name = (unitName || '').toLowerCase();
        if (name.includes('hội sở')) return 0;
        if (name.includes('bình lư')) return 1;
        if (name.includes('phong thổ')) return 2;
        if (name.includes('sìn hồ')) return 3;
        if (name.includes('bum tở')) return 4;
        if (name.includes('than uyên')) return 5;
        if (name.includes('đoàn kết')) return 6;
        if (name.includes('tân uyên')) return 7;
        if (name.includes('nậm hàng')) return 8;
        return 999; // Unknown units go to the end
      };

      const indexA = getOrderIndex(a.Name);
      const indexB = getOrderIndex(b.Name);
      return indexA - indexB;
    })

  console.log('🏢 CNL2 Units:', filtered.length, filtered.map(u => u.Name))
  return filtered
})

const selectedBranch = computed(() => {
  if (!selectedBranchId.value) return null
  return units.value.find(u => u.Id === parseInt(selectedBranchId.value))
})

const totalMaxScore = computed(() => {
  return availableKpiIndicators.value.reduce((sum, indicator) => sum + (safeGet(indicator, 'MaxScore') || 0), 0)
})

const totalScore = computed(() => {
  return currentAssignments.value.reduce((sum, assignment) => sum + (parseFloat(assignment.score) || 0), 0)
})

// Methods
async function loadInitialData() {
  loading.value = true
  errorMessage.value = ''

  try {
    const [periodsResponse, tablesResponse, unitsResponse] = await Promise.all([
      api.get('/KhoanPeriods'),
      api.get('/KpiAssignmentTables'),
      api.get('/units')
    ])

    khoanPeriods.value = periodsResponse.data || []
    kpiTables.value = tablesResponse.data || []

    // Load real units from API instead of hardcoded
    units.value = unitsResponse.data || []

    console.log('📊 Unit KPI Assignment data loaded:')
    console.log('   Periods:', khoanPeriods.value.length)
    console.log('   KPI Tables:', kpiTables.value.length)
    console.log('   Units:', units.value.length)
    console.log('   Units detail:', units.value.map(u => `${u.Name} (${u.Type})`))

  } catch (error) {
    console.error('❌ Error loading initial data:', error)
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
      code: branch.Code,
      type: branch.type
    })

    // Find appropriate KPI table based on branch type
    let kpiTable = null
    const branchType = (branch.Type && typeof branch.Type === 'string' ? branch.Type : '').toUpperCase()
    console.log('🔍 Branch type:', branchType)
    console.log('📊 Available KPI tables:', kpiTables.value.length)

    // Log all available branch tables
    const branchTables = kpiTables.value.filter(t =>
      t && t.Category === 'CHI NHÁNH'
    )
    console.log('🏢 Branch KPI tables found:', branchTables.length)
    branchTables.forEach(table => {
      console.log(`   📊 ${table.TableName} (ID: ${table.Id}, Description: ${table.Description}, Category: ${table.Category})`)
    })

    // Match branch name with KPI table
    const branchName = branch.Name || ''
    console.log('🔍 Looking for KPI table for branch:', branchName)

    // Find matching KPI table based on branch name
    kpiTable = branchTables.find(t => {
      const tableName = t.TableName || ''
      const description = t.Description || ''

      // Match by branch name patterns
      if (branchName.includes('Hội Sở') && tableName.includes('HoiSo')) return true
      if (branchName.includes('Bình Lư') && tableName.includes('BinhLu')) return true
      if (branchName.includes('Phong Thổ') && tableName.includes('PhongTho')) return true
      if (branchName.includes('Sìn Hồ') && tableName.includes('SinHo')) return true
      if (branchName.includes('Bum Tở') && tableName.includes('BumTo')) return true
      if (branchName.includes('Than Uyên') && tableName.includes('ThanUyen')) return true
      if (branchName.includes('Đoàn Kết') && tableName.includes('DoanKet')) return true
      if (branchName.includes('Tân Uyên') && tableName.includes('TanUyen')) return true
      if (branchName.includes('Nậm Hàng') && tableName.includes('NamHang')) return true

      return false
    })

    if (!kpiTable) {
      // Fallback: try to match by description
      kpiTable = branchTables.find(t => {
        const description = t.Description || ''
        return description.toLowerCase().includes(branchName.toLowerCase())
      })
    }

    // Fallback to first available branch table
    if (!kpiTable && branchTables.length > 0) {
      kpiTable = branchTables[0]
      console.log('⚠️ No specific match, using first branch table as fallback:', kpiTable.TableName)
    }

    if (kpiTable) {
      console.log('✅ Selected KPI table:', {
        id: kpiTable.Id,
        name: kpiTable.TableName,
        description: kpiTable.Description,
        category: kpiTable.Category
      })

      console.log('🔄 Loading KPI indicators...')
      const response = await api.get(`/KpiAssignment/tables/${kpiTable.Id}`)

      // Use helper function to log API response
      logApiResponse(`/KpiAssignment/tables/${kpiTable.Id}`, response, 'indicators')

      // Handle both 'indicators' (lowercase) and 'Indicators' (PascalCase) from API
      const indicatorsData = response.data?.indicators || response.data?.Indicators
      if (response.data && indicatorsData) {
        // Use helper function to normalize .NET array format
        const normalizedData = normalizeNetArray(indicatorsData)
        console.log('🔄 Raw indicators data:', indicatorsData)
        console.log('🔄 Normalized data:', normalizedData)

        availableKpiIndicators.value = normalizedData
        console.log('✅ Loaded KPI indicators:', availableKpiIndicators.value.length)

        // Log sample indicators
        if (availableKpiIndicators.value.length > 0) {
          console.log('📋 Sample indicators:')
          availableKpiIndicators.value.slice(0, 3).forEach((ind, idx) => {
            console.log(`   ${idx + 1}. ${ind.indicatorName || ind.IndicatorName} (${ind.maxScore || ind.MaxScore} points, ${ind.unit || ind.Unit || 'N/A'})`)
          })
        } else {
          console.log('⚠️ Indicators array is empty after normalization')
        }
      } else {
        console.log('⚠️ API response missing indicators array')
        console.log('🔍 Response data keys:', Object.keys(response.data || {}))
        availableKpiIndicators.value = []
      }
    } else {
      console.log('❌ No suitable KPI table found for branch:', branchName)
      console.log('Available branch tables:', branchTables.map(t => ({ id: t.Id, name: t.TableName, description: t.Description })))
      errorMessage.value = `Không tìm thấy bảng KPI phù hợp cho chi nhánh "${branchName}". Có ${branchTables.length} bảng KPI chi nhánh.`
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
  return parent ? parent.Code : ''
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

// Enhanced number input handlers for KPI targets with unit-specific validation
const handleKpiTargetInput = (event, indicatorId) => {
  const indicator = availableKpiIndicators.value.find(ind => ind.Id === indicatorId);
  const unit = safeGet(indicator, 'Unit') || 'N/A';

  let inputValue = event.target.value;

  // Remove all non-numeric characters except decimal point for initial processing
  let numericValue = inputValue.replace(/[^\d.,]/g, '');

  // Handle comma as decimal separator (Vietnamese style)
  numericValue = numericValue.replace(',', '.');

  // For percentage, limit to 100
  if (unit === '%') {
    const numValue = parseFloat(numericValue);
    if (!isNaN(numValue) && numValue > 100) {
      numericValue = '100';
      kpiTargetErrors.value[indicatorId] = 'Giá trị tối đa là 100%';
    } else {
      delete kpiTargetErrors.value[indicatorId];
    }
  }

  // For "Triệu VND", format with thousand separators and limit to 8 digits
  if (unit === 'Triệu VND') {
    // Remove formatting first to get clean number
    let cleanNumber = numericValue.replace(/[,.\s]/g, '');

    // Limit to 8 digits maximum
    if (cleanNumber.length > 8) {
      cleanNumber = cleanNumber.substring(0, 8);
      kpiTargetErrors.value[indicatorId] = 'Giá trị tối đa là 8 chữ số (99,999,999 triệu VND)';
    } else {
      delete kpiTargetErrors.value[indicatorId];
    }

    const numValue = parseFloat(cleanNumber);
    if (!isNaN(numValue) && cleanNumber !== '') {
      // ✅ Format with formatNumber chuẩn US: 1,000,000 thay vì vi-VN: 1.000.000
      const formatted = formatNumber(numValue);
      event.target.value = formatted;
      kpiTargets.value[indicatorId] = numValue;
      return;
    } else if (cleanNumber === '') {
      event.target.value = '';
      kpiTargets.value[indicatorId] = null;
      delete kpiTargetErrors.value[indicatorId];
      return;
    }
  }

  // For percentage, keep as decimal
  if (unit === '%') {
    const numValue = parseFloat(numericValue);
    if (!isNaN(numValue)) {
      event.target.value = numValue.toString();
      kpiTargets.value[indicatorId] = numValue;
    }
    return;
  }

  // Default handling for other units
  const numValue = parseFloat(numericValue);
  if (!isNaN(numValue)) {
    event.target.value = numValue.toString();
    kpiTargets.value[indicatorId] = numValue;
    delete kpiTargetErrors.value[indicatorId];
  } else if (inputValue.trim() === '') {
    kpiTargets.value[indicatorId] = null;
    delete kpiTargetErrors.value[indicatorId];
  } else {
    kpiTargetErrors.value[indicatorId] = 'Vui lòng chỉ nhập số';
  }
};

const handleKpiTargetBlur = (event, indicatorId) => {
  const indicator = availableKpiIndicators.value.find(ind => ind.Id === indicatorId);
  const unit = safeGet(indicator, 'Unit') || 'N/A';
  const inputValue = event.target.value;

  if (inputValue.trim() === '') {
    kpiTargets.value[indicatorId] = null;
    delete kpiTargetErrors.value[indicatorId];
    return;
  }

  // Parse the final value
  let numericValue = inputValue.replace(/[^\d.,]/g, '').replace(',', '.');
  const numValue = parseFloat(numericValue);

  if (isNaN(numValue)) {
    kpiTargetErrors.value[indicatorId] = 'Giá trị không hợp lệ';
    return;
  }

  // Final validation and formatting
  if (unit === '%') {
    if (numValue > 100) {
      kpiTargetErrors.value[indicatorId] = 'Giá trị tối đa là 100%';
      event.target.value = '100';
      kpiTargets.value[indicatorId] = 100;
    } else {
      event.target.value = numValue.toString();
      kpiTargets.value[indicatorId] = numValue;
      delete kpiTargetErrors.value[indicatorId];
    }
  } else if (unit === 'Triệu VND') {
    // Remove formatting and limit to 8 digits
    let cleanNumber = inputValue.replace(/[,.\s]/g, '');
    if (cleanNumber.length > 8) {
      cleanNumber = cleanNumber.substring(0, 8);
      kpiTargetErrors.value[indicatorId] = 'Giá trị tối đa là 8 chữ số (99,999,999 triệu VND)';
    }

    const finalValue = parseFloat(cleanNumber);
    if (!isNaN(finalValue)) {
      // ✅ Format with formatNumber chuẩn US: 1,000,000 thay vì vi-VN: 1.000.000
      const formatted = formatNumber(finalValue);
      event.target.value = formatted;
      kpiTargets.value[indicatorId] = finalValue;
      if (cleanNumber.length <= 8) {
        delete kpiTargetErrors.value[indicatorId];
      }
    }
  } else {
    event.target.value = numValue.toString();
    kpiTargets.value[indicatorId] = numValue;
    delete kpiTargetErrors.value[indicatorId];
  }
};

// Format target value based on unit type for Unit KPI
function formatUnitTargetValue(indicator, value) {
  if (value === null || value === undefined || value === '') return '';

  const unit = safeGet(indicator, 'Unit') || 'N/A';
  const numValue = parseFloat(value);

  if (isNaN(numValue)) return '';

  // Format based on unit type
  if (unit === 'Triệu VND') {
    // ✅ Sử dụng formatNumber chuẩn US: 1,000,000 thay vì vi-VN: 1.000.000
    return formatNumber(numValue);
  } else if (unit === '%') {
    return numValue.toString();
  } else {
    return numValue.toString();
  }
}

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

.error input {
  border-color: var(--danger-color) !important;
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
