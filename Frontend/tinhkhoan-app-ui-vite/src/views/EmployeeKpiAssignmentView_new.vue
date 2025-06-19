<template>
  <div class="employee-kpi-assignment">
    <h1 class="text-primary">🎯 Giao khoán KPI cho Cán bộ</h1>
    
    <div v-if="errorMessage" class="alert-agribank alert-danger">
      <strong>❌ Lỗi:</strong> {{ errorMessage }}
    </div>
    
    <div v-if="loading" class="loading-agribank">
      <div class="spinner-agribank"></div>
      <p>Đang tải dữ liệu...</p>
    </div>
    
    <div v-else class="content-container">
      <!-- Period Selection -->
      <div class="card-agribank">
        <div class="card-header">
          <h3 class="card-title">📅 Chọn kỳ khoán</h3>
        </div>
        <div class="card-body">
          <div class="form-group">
            <label class="form-label">Kỳ khoán:</label>
            <select v-model="selectedPeriodId" class="form-control">
              <option value="">-- Chọn kỳ khoán --</option>
              <option v-for="period in khoanPeriods" :key="period.id" :value="period.id">
                {{ period.name }} ({{ formatDate(period.startDate) }} - {{ formatDate(period.endDate) }})
              </option>
            </select>
          </div>
        </div>
      </div>

      <!-- Branch and Department Filter -->
      <div class="card-agribank" v-if="selectedPeriodId">
        <div class="card-header">
          <h3 class="card-title">🔍 Lọc cán bộ theo đơn vị</h3>
        </div>
        <div class="card-body">
          <div class="row">
            <div class="col">
              <div class="form-group">
                <label class="form-label">🏢 Chi nhánh:</label>
                <select v-model="selectedBranchId" @change="onBranchChange" class="form-control">
                  <option value="">-- Tất cả chi nhánh --</option>
                  <option v-for="branch in branchOptions" :key="branch.id" :value="branch.id">
                    🏢 {{ branch.name }} ({{ branch.code }})
                  </option>
                </select>
              </div>
            </div>
            
            <div class="col" v-if="selectedBranchId">
              <div class="form-group">
                <label class="form-label">🏬 Phòng ban:</label>
                <select v-model="selectedDepartmentId" @change="onDepartmentChange" class="form-control">
                  <option value="">-- Tất cả phòng ban --</option>
                  <option v-for="dept in departmentOptions" :key="dept.id" :value="dept.id">
                    🏬 {{ dept.name }} ({{ dept.code }})
                  </option>
                </select>
              </div>
            </div>
          </div>
          
          <div class="alert-agribank alert-info" v-if="selectedBranchId || selectedDepartmentId">
            <strong>📊 Đang lọc:</strong> 
            <span v-if="selectedBranchId">Chi nhánh "{{ getBranchName() }}"</span>
            <span v-if="selectedDepartmentId"> → Phòng ban "{{ getDepartmentName() }}"</span>
            → Tìm thấy <strong>{{ filteredEmployeesCount }}</strong> cán bộ phù hợp
          </div>
        </div>
      </div>
      
      <!-- Employee Table -->
      <div v-if="selectedBranchId || selectedDepartmentId" class="card-agribank">
        <div class="card-header">
          <h3 class="card-title">👥 Danh sách Cán bộ ({{ filteredEmployeesCount }} người)</h3>
          <div>
            <span class="badge-agribank badge-primary">🏢 {{ getBranchName() || 'Tất cả chi nhánh' }}</span>
            <span class="badge-agribank badge-secondary" v-if="selectedDepartmentId" style="margin-left: 8px;">🏬 {{ getDepartmentName() }}</span>
          </div>
        </div>
        
        <div class="card-body">
          <table class="table-agribank">
            <thead>
              <tr>
                <th style="width: 50px; text-align: center;">
                  <input 
                    type="checkbox" 
                    @change="toggleAllEmployees"
                    :checked="areAllEmployeesSelected"
                    :indeterminate="areSomeEmployeesSelected"
                  />
                </th>
                <th style="width: 30%;">👤 Họ và tên</th>
                <th style="width: 20%;">🏷️ Vai trò</th>
                <th style="width: 25%;">🏢 Đơn vị</th>
                <th style="width: 20%;">💼 Chức vụ</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="employee in filteredEmployees" :key="employee.id">
                <td style="text-align: center;">
                  <input 
                    type="checkbox" 
                    :value="employee.id"
                    v-model="selectedEmployeeIds"
                    @change="validateEmployeeRoles"
                  />
                </td>
                <td>
                  <div>
                    <div class="font-weight-semibold">{{ employee.fullName }}</div>
                    <small class="text-muted">{{ employee.employeeCode }}</small>
                  </div>
                </td>
                <td>
                  <span class="badge-agribank badge-danger">{{ getEmployeeRole(employee) }}</span>
                </td>
                <td>
                  <span class="text-secondary">{{ getUnitName(employee.unitId) }}</span>
                </td>
                <td>
                  <span class="text-secondary">{{ employee.positionName || 'N/A' }}</span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        
        <div class="card-body" v-if="selectedEmployeeIds.length > 0" style="padding-top: 0;">
          <div class="alert-agribank alert-success">
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px;">
              <strong>✅ Đã chọn {{ selectedEmployeeIds.length }} cán bộ</strong>
              <button @click="clearSelectedEmployees" class="btn-agribank btn-outline" style="padding: 4px 8px; font-size: 0.75rem;">
                🗑️ Xóa tất cả
              </button>
            </div>
            <div style="display: flex; flex-wrap: wrap; gap: 8px;">
              <span v-for="empId in selectedEmployeeIds" :key="empId" class="badge-agribank badge-accent" style="display: inline-flex; align-items: center; gap: 4px;">
                {{ getEmployeeShortName(empId) }}
                <button @click="removeEmployee(empId)" style="background: none; border: none; color: inherit; cursor: pointer; font-weight: bold;">×</button>
              </span>
            </div>
          </div>
        </div>
      </div>
      
      <!-- KPI Table Selection -->
      <div class="card-agribank" v-if="selectedPeriodId">
        <div class="card-header">
          <h3 class="card-title">📊 Chọn bảng KPI</h3>
        </div>
        <div class="card-body">
          <div class="form-group">
            <label class="form-label">Bảng KPI:</label>
            <select v-model="selectedTableId" @change="loadTableDetails" class="form-control">
              <option value="">-- Chọn bảng --</option>
              <option v-for="table in kpiTables" :key="table.id" :value="table.id">
                {{ table.tableName }}
              </option>
            </select>
          </div>
        </div>
      </div>
      
      <!-- KPI Indicators Table -->
      <div v-if="indicators.length > 0" class="card-agribank">
        <div class="card-header">
          <h3 class="card-title">📊 {{ getKpiTableTitle() }}</h3>
          <div>
            <span class="badge-agribank badge-info" style="margin-right: 8px;">{{ indicators.length }} chỉ tiêu</span>
            <span class="badge-agribank badge-success">{{ selectedEmployeeIds.length }} cán bộ được chọn</span>
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
              <tr v-for="(indicator, index) in indicators" :key="indicator.id">
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
                    :value="getDisplayValue(indicator.id, getIndicatorUnit(indicator))"
                    @input="validateNumberInput($event, indicator.id, getIndicatorUnit(indicator))"
                    @focus="handleInputFocus($event, indicator.id)"
                    @blur="handleInputBlur($event, indicator.id, getIndicatorUnit(indicator))"
                    placeholder="Nhập mục tiêu"
                    class="form-control"
                    style="font-size: 0.85rem; padding: 8px 12px;"
                    :class="{ 'error': targetErrors[indicator.id] }"
                  />
                  <div v-if="targetErrors[indicator.id]" class="text-danger" style="font-size: 0.75rem; margin-top: 4px;">
                    {{ targetErrors[indicator.id] }}
                  </div>
                </td>
                <td style="text-align: center;">
                  <span class="badge-agribank badge-secondary">{{ getIndicatorUnit(indicator) }}</span>
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
                      :disabled="index === indicators.length - 1"
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
                <td style="text-align: center;"><strong class="badge-agribank badge-primary">{{ getTotalMaxScore() }}</strong></td>
                <td><strong class="text-secondary">{{ getTotalTargets() }} mục tiêu</strong></td>
                <td style="text-align: center;"><strong class="badge-agribank badge-primary">Điểm</strong></td>
                <td></td>
              </tr>
            </tbody>
          </table>
        </div>
        
        <div class="card-body" style="text-align: center; padding-top: 0;">
          <button @click="assignKPI" :disabled="saving" class="btn-agribank btn-primary" style="font-size: 1rem; padding: 16px 32px;">
            <span>{{ saving ? '⏳' : '📋' }}</span>
            <span style="margin-left: 8px;">{{ saving ? 'Đang giao khoán...' : 'Giao khoán KPI' }}</span>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import api from '@/services/api'

const loading = ref(false)
const saving = ref(false)
const errorMessage = ref('')
const kpiTables = ref([])
const employees = ref([])
const units = ref([])
const indicators = ref([])
const khoanPeriods = ref([])
const selectedTableId = ref('')
const selectedEmployeeIds = ref([])
const selectedBranchId = ref('')
const selectedDepartmentId = ref('')
const selectedPeriodId = ref('')
const targetValues = ref({})
const targetErrors = ref({})

// Computed properties cho bộ lọc
// Updated branchOptions: Use SortOrder from backend instead of hardcoded sorting
const branchOptions = computed(() => {
  return units.value
    .filter(unit => {
      const type = (unit.type || '').toUpperCase()
      return type === 'CNL1' || type === 'CNL2'
    })
    .sort((a, b) => {
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

const departmentOptions = computed(() => {
  if (!selectedBranchId.value) return []
  
  const branch = units.value.find(u => u.id === parseInt(selectedBranchId.value))
  if (!branch) return []
  
  const children = units.value.filter(u => u.parentUnitId === branch.id)
  const branchType = (branch.type || '').toUpperCase()
  
  if (branchType === 'CNL1') {
    return children.filter(u => {
      const unitType = (u.type || '').toUpperCase()
      return unitType === 'PNVL1'
    }).sort((a, b) => (a.name || '').localeCompare(b.name || ''))
  } else if (branchType === 'CNL2') {
    return children.filter(u => {
      const unitType = (u.type || '').toUpperCase()
      return unitType === 'PNVL2' || unitType === 'PGDL2'
    }).sort((a, b) => (a.name || '').localeCompare(b.name || ''))
  }
  
  return []
})

const filteredEmployees = computed(() => {
  let filtered = employees.value

  if (selectedBranchId.value) {
    const branchId = parseInt(selectedBranchId.value)
    filtered = filtered.filter(emp => {
      const empUnit = units.value.find(u => u.id === emp.unitId)
      if (!empUnit) return false
      
      if (empUnit.id === branchId) return true
      
      let parent = empUnit
      while (parent && parent.parentUnitId) {
        parent = units.value.find(u => u.id === parent.parentUnitId)
        if (parent && parent.id === branchId) return true
      }
      
      return false
    })
  }

  if (selectedDepartmentId.value) {
    const deptId = parseInt(selectedDepartmentId.value)
    filtered = filtered.filter(emp => {
      const empUnit = units.value.find(u => u.id === emp.unitId)
      if (!empUnit) return false
      
      if (empUnit.id === deptId) return true
      
      let parent = empUnit
      while (parent && parent.parentUnitId) {
        parent = units.value.find(u => u.id === parent.parentUnitId)
        if (parent && parent.id === deptId) return true
      }
      
      return false
    })
  }

  return filtered.filter(emp => emp.fullName && emp.fullName.trim() !== '')
})

const filteredEmployeesCount = computed(() => filteredEmployees.value.length)

const areAllEmployeesSelected = computed(() => {
  return filteredEmployees.value.length > 0 && 
         filteredEmployees.value.every(emp => selectedEmployeeIds.value.includes(emp.id))
})

const areSomeEmployeesSelected = computed(() => {
  return selectedEmployeeIds.value.length > 0 && 
         !areAllEmployeesSelected.value
})

async function loadInitialData() {
  loading.value = true
  errorMessage.value = ''
  
  try {
    const [tablesResponse, employeesResponse, unitsResponse, periodsResponse] = await Promise.all([
      api.get('/KpiAssignment/tables'),
      api.get('/employees'),
      api.get('/units'),
      api.get('/khoan-periods')
    ])
    
    kpiTables.value = tablesResponse.data || []
    employees.value = employeesResponse.data || []
    units.value = unitsResponse.data || []
    khoanPeriods.value = periodsResponse.data || []
    
    console.log('KPI Tables loaded:', kpiTables.value.length)
    console.log('Employees loaded:', employees.value.length)
    console.log('Units loaded:', units.value.length)
    console.log('Periods loaded:', khoanPeriods.value.length)
    
  } catch (error) {
    console.error('Error loading initial data:', error)
    errorMessage.value = 'Không thể tải dữ liệu: ' + (error.response?.data?.message || error.message)
  } finally {
    loading.value = false
  }
}

async function loadTableDetails() {
  if (!selectedTableId.value) {
    indicators.value = []
    return
  }
  
  try {
    const response = await api.get(`/KpiAssignment/tables/${selectedTableId.value}`)
    indicators.value = response.data.indicators || []
    targetValues.value = {}
    targetErrors.value = {}
    
    console.log('Loaded KPI indicators:', indicators.value.length)
  } catch (error) {
    console.error('Error loading table details:', error)
    errorMessage.value = 'Không thể tải chi tiết bảng KPI: ' + (error.response?.data?.message || error.message)
  }
}

function onBranchChange() {
  selectedDepartmentId.value = ''
  selectedEmployeeIds.value = []
}

function onDepartmentChange() {
  selectedEmployeeIds.value = []
}

function toggleAllEmployees() {
  if (areAllEmployeesSelected.value) {
    selectedEmployeeIds.value = []
  } else {
    selectedEmployeeIds.value = filteredEmployees.value.map(emp => emp.id)
  }
  validateEmployeeRoles()
}

function clearSelectedEmployees() {
  selectedEmployeeIds.value = []
}

function removeEmployee(empId) {
  selectedEmployeeIds.value = selectedEmployeeIds.value.filter(id => id !== empId)
}

function getBranchName() {
  if (!selectedBranchId.value) return ''
  const branch = units.value.find(u => u.id === parseInt(selectedBranchId.value))
  return branch ? branch.name : ''
}

function getDepartmentName() {
  if (!selectedDepartmentId.value) return ''
  const dept = units.value.find(u => u.id === parseInt(selectedDepartmentId.value))
  return dept ? dept.name : ''
}

function getUnitName(unitId) {
  const unit = units.value.find(u => u.id === unitId)
  return unit ? unit.name : 'N/A'
}

function getEmployeeRole(employee) {
  return employee.roleName || employee.positionName || 'Cán bộ'
}

function getEmployeeShortName(empId) {
  const emp = employees.value.find(e => e.id === empId)
  if (!emp) return 'N/A'
  
  const names = emp.fullName.split(' ')
  if (names.length >= 2) {
    return names[names.length - 2] + ' ' + names[names.length - 1]
  }
  return emp.fullName
}

function getKpiTableTitle() {
  const table = kpiTables.value.find(t => t.id === parseInt(selectedTableId.value))
  return table ? table.tableName : 'Bảng KPI'
}

function getIndicatorUnit(indicator) {
  return indicator.unit || 'N/A'
}

function getDisplayValue(indicatorId, unit) {
  const value = targetValues.value[indicatorId]
  if (value === undefined || value === null || value === '') return ''
  
  if (unit === '%') {
    return value.toString()
  }
  
  return value.toString()
}

function validateNumberInput(event, indicatorId, unit) {
  const value = event.target.value.trim()
  
  if (value === '') {
    targetValues.value[indicatorId] = null
    delete targetErrors.value[indicatorId]
    return
  }
  
  const numValue = parseFloat(value)
  if (isNaN(numValue)) {
    targetErrors.value[indicatorId] = 'Vui lòng nhập số hợp lệ'
    return
  }
  
  if (unit === '%' && (numValue < 0 || numValue > 100)) {
    targetErrors.value[indicatorId] = 'Phần trăm phải từ 0 đến 100'
    return
  }
  
  if (numValue < 0) {
    targetErrors.value[indicatorId] = 'Giá trị không được âm'
    return
  }
  
  targetValues.value[indicatorId] = numValue
  delete targetErrors.value[indicatorId]
}

function handleInputFocus(event, indicatorId) {
  const value = targetValues.value[indicatorId]
  if (value !== undefined && value !== null) {
    event.target.value = value.toString()
  }
}

function handleInputBlur(event, indicatorId, unit) {
  validateNumberInput(event, indicatorId, unit)
}

function validateEmployeeRoles() {
  // Implement validation logic if needed
}

function moveIndicatorUp(index) {
  if (index > 0) {
    const temp = indicators.value[index]
    indicators.value[index] = indicators.value[index - 1]
    indicators.value[index - 1] = temp
  }
}

function moveIndicatorDown(index) {
  if (index < indicators.value.length - 1) {
    const temp = indicators.value[index]
    indicators.value[index] = indicators.value[index + 1]
    indicators.value[index + 1] = temp
  }
}

function editIndicator(indicator) {
  // Implement edit functionality
  console.log('Edit indicator:', indicator)
}

function clearIndicatorTarget(indicatorId) {
  delete targetValues.value[indicatorId]
  delete targetErrors.value[indicatorId]
}

function getTotalMaxScore() {
  return indicators.value.reduce((sum, ind) => sum + (ind.maxScore || 0), 0)
}

function getTotalTargets() {
  return Object.keys(targetValues.value).length
}

function formatDate(dateString) {
  if (!dateString) return 'N/A'
  const date = new Date(dateString)
  return date.toLocaleDateString('vi-VN')
}

async function assignKPI() {
  if (selectedEmployeeIds.value.length === 0) {
    errorMessage.value = 'Vui lòng chọn ít nhất một cán bộ'
    return
  }
  
  if (!selectedPeriodId.value) {
    errorMessage.value = 'Vui lòng chọn kỳ khoán'
    return
  }
  
  const targets = Object.entries(targetValues.value)
    .filter(([_, value]) => value !== null && value !== undefined)
    .map(([indicatorId, value]) => ({
      indicatorId: parseInt(indicatorId),
      targetValue: value,
      notes: ''
    }))
  
  if (targets.length === 0) {
    errorMessage.value = 'Vui lòng nhập ít nhất một mục tiêu'
    return
  }
  
  saving.value = true
  errorMessage.value = ''
  
  try {
    for (const employeeId of selectedEmployeeIds.value) {
      const request = {
        employeeId: employeeId,
        khoanPeriodId: parseInt(selectedPeriodId.value),
        targets: targets
      }
      
      await api.post('/KpiAssignment/assign', request)
    }
    
    // Show success message
    alert(`Đã giao khoán KPI thành công cho ${selectedEmployeeIds.value.length} cán bộ`)
    
    // Reset form
    selectedEmployeeIds.value = []
    targetValues.value = {}
    targetErrors.value = {}
    
  } catch (error) {
    console.error('Error assigning KPI:', error)
    errorMessage.value = 'Lỗi khi giao khoán KPI: ' + (error.response?.data?.message || error.message)
  } finally {
    saving.value = false
  }
}

onMounted(() => {
  loadInitialData()
})
</script>

<style scoped>
.employee-kpi-assignment {
  padding: 20px;
  max-width: 1400px;
  margin: 0 auto;
}

.content-container {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.row {
  display: flex;
  gap: 20px;
}

.col {
  flex: 1;
}

.form-group {
  margin-bottom: 0;
}

.error input {
  border-color: var(--danger-color) !important;
}

@media (max-width: 768px) {
  .row {
    flex-direction: column;
    gap: 0;
  }
  
  .col {
    margin-bottom: 16px;
  }
  
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
