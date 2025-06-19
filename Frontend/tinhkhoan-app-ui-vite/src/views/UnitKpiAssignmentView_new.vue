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
              <option v-for="period in khoanPeriods" :key="period.id" :value="period.id">
                {{ period.name }} ({{ formatDate(period.startDate) }} - {{ formatDate(period.endDate) }})
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
                <option v-for="unit in cnl1Units" :key="unit.id" :value="unit.id">
                  🏢 {{ unit.name }} ({{ unit.code }})
                </option>
              </optgroup>
              <optgroup label="Chi nhánh CNL2" v-if="cnl2Units.length > 0">
                <option v-for="unit in cnl2Units" :key="unit.id" :value="unit.id">
                  🏢 {{ unit.name }} ({{ unit.code }}) - {{ getParentUnitCode(unit.parentUnitId) }}
                </option>
              </optgroup>
            </select>
          </div>
          
          <div class="alert-agribank alert-info" v-if="selectedBranch">
            <strong>📊 Đã chọn:</strong> 
            Chi nhánh "{{ selectedBranch.name }}" ({{ selectedBranch.code }})
            → <strong>{{ availableKpiIndicators.length }}</strong> chỉ tiêu KPI
          </div>
        </div>
      </div>

      <!-- KPI Assignment Section -->
      <div v-if="availableKpiIndicators.length > 0" class="card-agribank">
        <div class="card-header">
          <h3 class="card-title">📊 Giao khoán KPI chi nhánh</h3>
          <div>
            <span class="badge-agribank badge-info" style="margin-right: 8px;">{{ availableKpiIndicators.length }} chỉ tiêu</span>
            <span class="badge-agribank badge-accent">{{ totalMaxScore }} điểm tối đa</span>
          </div>
        </div>
        
        <div class="card-body">
          <table class="table-agribank">
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
              <tr v-for="(indicator, index) in availableKpiIndicators" :key="indicator.id">
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
                    type="number" 
                    step="0.01"
                    v-model="kpiTargets[indicator.id]"
                    placeholder="Nhập mục tiêu"
                    class="form-control"
                    style="font-size: 0.85rem; padding: 8px 12px;"
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
                      @click="clearIndicatorTarget(indicator.id)" 
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
        
        <div class="card-body" style="text-align: center; padding-top: 0;">
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
              <tr v-for="assignment in currentAssignments" :key="assignment.id">
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
import { ref, onMounted, computed } from 'vue'
import api from '@/services/api'

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
// Updated cnl1Units: Use SortOrder from backend instead of hardcoded sorting
const cnl1Units = computed(() => {
  return units.value.filter(unit => {
    const type = (unit.type || '').toUpperCase()
    return type === 'CNL1'
  }).sort((a, b) => {
    // Primary sort: SortOrder (nulls last)
    const sortOrderA = a.sortOrder ?? Number.MAX_SAFE_INTEGER;
    const sortOrderB = b.sortOrder ?? Number.MAX_SAFE_INTEGER;
    
    if (sortOrderA !== sortOrderB) {
      return sortOrderA - sortOrderB;
    }
    
    // Secondary sort: Name
    return (a.name || '').localeCompare(b.name || '');
  })
})

// Updated cnl2Units: Use SortOrder from backend instead of hardcoded sorting
const cnl2Units = computed(() => {
  return units.value.filter(unit => {
    const type = (unit.type || '').toUpperCase()
    return type === 'CNL2'
  }).sort((a, b) => {
    // Primary sort: SortOrder (nulls last)
    const sortOrderA = a.sortOrder ?? Number.MAX_SAFE_INTEGER;
    const sortOrderB = b.sortOrder ?? Number.MAX_SAFE_INTEGER;
    
    if (sortOrderA !== sortOrderB) {
      return sortOrderA - sortOrderB;
    }
    
    // Secondary sort: Name
    return (a.name || '').localeCompare(b.name || '');
  })
})

const selectedBranch = computed(() => {
  if (!selectedBranchId.value) return null
  return units.value.find(u => u.id === parseInt(selectedBranchId.value))
})

const totalMaxScore = computed(() => {
  return availableKpiIndicators.value.reduce((sum, indicator) => sum + (indicator.maxScore || 0), 0)
})

// Methods
async function loadInitialData() {
  loading.value = true
  errorMessage.value = ''
  
  try {
    const [periodsResponse, unitsResponse, tablesResponse] = await Promise.all([
      api.get('/khoan-periods'),
      api.get('/units'),
      api.get('/KpiAssignment/tables')
    ])
    
    khoanPeriods.value = periodsResponse.data || []
    units.value = unitsResponse.data || []
    kpiTables.value = tablesResponse.data || []
    
    console.log('Loaded periods:', khoanPeriods.value.length)
    console.log('Loaded units:', units.value.length)
    console.log('Loaded KPI tables:', kpiTables.value.length)
    
  } catch (error) {
    console.error('Error loading initial data:', error)
    errorMessage.value = 'Không thể tải dữ liệu: ' + (error.response?.data?.message || error.message)
  } finally {
    loading.value = false
  }
}

function onPeriodChange() {
  selectedBranchId.value = ''
  availableKpiIndicators.value = []
  kpiTargets.value = {}
  currentAssignments.value = []
  clearMessages()
}

async function onBranchChange() {
  availableKpiIndicators.value = []
  kpiTargets.value = {}
  currentAssignments.value = []
  clearMessages()
  
  if (!selectedBranchId.value) return
  
  try {
    // Load KPI indicators for branch
    const branch = selectedBranch.value
    if (!branch) return
    
    // Find appropriate KPI table based on branch type
    let kpiTable = null
    const branchType = (branch.type || '').toUpperCase()
    
    if (branchType === 'CNL1') {
      // For CNL1, find unit-specific table or use generic branch table
      kpiTable = kpiTables.value.find(t => t.tableType === 'CnTamDuong') // Use as template
    } else if (branchType === 'CNL2') {
      kpiTable = kpiTables.value.find(t => t.tableType === 'CnPhongTho') // Use as template
    }
    
    if (!kpiTable) {
      // Fallback to first available branch table
      kpiTable = kpiTables.value.find(t => t.category === 'Dành cho Chi nhánh')
    }
    
    if (kpiTable) {
      const response = await api.get(`/KpiAssignment/tables/${kpiTable.id}`)
      availableKpiIndicators.value = response.data.indicators || []
      console.log('Loaded KPI indicators for branch:', availableKpiIndicators.value.length)
    }
    
    // Load current assignments
    await loadCurrentAssignments()
    
  } catch (error) {
    console.error('Error loading branch KPI data:', error)
    errorMessage.value = 'Không thể tải dữ liệu KPI cho chi nhánh: ' + (error.response?.data?.message || error.message)
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
  const parent = units.value.find(u => u.id === parentUnitId)
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
    
    successMessage.value = `Đã giao khoán KPI thành công cho chi nhánh "${selectedBranch.value.name}" với ${targets.length} chỉ tiêu`
    
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

onMounted(() => {
  loadInitialData()
})
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
