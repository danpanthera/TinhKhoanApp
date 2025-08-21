<template>
  <div class="kpi-actual-values">
    <div class="header-section">
      <h1>📊 B5 - Tình hình thực hiện</h1>
      <p class="subtitle">
        Nhập và cập nhật giá trị thực hiện cho các chỉ tiêu KPI đã giao khoán
      </p>
    </div>

    <!-- Tab Navigation with Clear Categories -->
    <div class="tab-navigation">
      <button class="tab-button" :class="{ active: activeTab === 'employee' }" @click="switchTab('employee')">
        👤 CANBO - Cập nhật KPI Cán bộ
      </button>
      <button class="tab-button" :class="{ active: activeTab === 'unit' }" @click="switchTab('unit')">
        🏢 CHINHANH - Cập nhật KPI Chi nhánh
      </button>
    </div>

    <!-- Error Message -->
    <div v-if="errorMessage" class="error-message">
      <p>{{ errorMessage }}</p>
    </div>

    <!-- Success Message -->
    <div v-if="successMessage" class="success-message">
      <p>{{ successMessage }}</p>
    </div>

    <!-- Tab Content -->
    <div class="tab-content">
      <!-- Employee Tab -->
      <div v-show="activeTab === 'employee'" class="tab-pane">
        <!-- Filter Section -->
        <div class="filter-section">
          <div class="form-container">
            <h2>🔍 Tìm kiếm giao khoán cán bộ</h2>

            <div class="filter-form">
              <div class="form-row">
                <div class="form-group">
                  <label for="branchFilter">🏢 Chi nhánh:</label>
                  <select id="branchFilter" v-model="selectedBranchId" @change="onBranchChange">
                    <option value="">
                      -- Tất cả chi nhánh --
                    </option>
                    <option v-for="branch in branchOptions" :key="branch.Id" :value="branch.Id">
                      🏢 {{ branch.Name }} ({{ branch.Code }})
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <label for="departmentFilter">🏬 Phòng ban:</label>
                  <select id="departmentFilter" v-model="selectedDepartmentId" @change="onDepartmentChange">
                    <option value="">
                      -- Tất cả phòng ban --
                    </option>
                    <option v-for="dept in departmentOptions" :key="dept.Id" :value="dept.Id">
                      🏬 {{ dept.Name }} ({{ dept.Code }})
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <label for="employeeFilter">Nhân viên:</label>
                  <select id="employeeFilter" v-model="selectedEmployeeId" @change="onFilterChange">
                    <option value="">
                      -- Tất cả nhân viên --
                    </option>
                    <option v-for="employee in filteredEmployees" :key="employee.Id" :value="employee.Id">
                      {{ employee.fullName }} - {{ employee.unit?.Name }}
                    </option>
                  </select>
                </div>
              </div>

              <div class="form-row">
                <div class="form-group">
                  <label for="periodFilter">Kỳ khoán:</label>
                  <select id="periodFilter" v-model="selectedPeriodId" @change="onFilterChange">
                    <option value="">
                      -- Tất cả kỳ khoán --
                    </option>
                    <option v-for="period in khoanPeriods" :key="period.Id" :value="period.Id">
                      {{ period.Name }} ({{ formatDate(period.startDate) }} - {{ formatDate(period.endDate) }})
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <button
                    :disabled="!canSearch || searching"
                    class="action-button search-btn"
                    @click="searchAssignments"
                  >
                    {{ searching ? 'Đang tìm...' : '🔍 Tìm kiếm' }}
                  </button>
                </div>
              </div>

              <div v-if="selectedBranchId || selectedDepartmentId" class="filter-summary">
                <p>
                  📊 <strong>Đang lọc:</strong>
                  <span v-if="selectedBranchId">Chi nhánh "{{ getBranchName() }}"</span>
                  <span v-if="selectedDepartmentId"> → Phòng ban "{{ getDepartmentName() }}"</span>
                  → Tìm thấy <strong>{{ filteredEmployees.length }}</strong> nhân viên phù hợp
                </p>
              </div>
            </div>
          </div>
        </div>

        <!-- Loading State -->
        <div v-if="searching" class="loading-section">
          <div class="loading-spinner" />
          <p>Đang tìm kiếm giao khoán...</p>
        </div>

        <!-- Assignments List -->
        <div v-if="!searching && assignments.length > 0" class="assignments-section">
          <div class="form-container">
            <div class="assignments-header">
              <h2>Giao khoán KPI</h2>
              <div class="assignments-summary">
                <span class="summary-item">
                  <strong>{{ assignments.length }}</strong> giao khoán
                </span>
                <span class="summary-item">
                  <strong>{{ pendingCount }}</strong> chưa hoàn thành
                </span>
                <span class="summary-item">
                  <strong>{{ completedCount }}</strong> đã hoàn thành
                </span>
              </div>
            </div>

            <div class="assignments-table">
              <table>
                <thead>
                  <tr>
                    <th>Nhân viên</th>
                    <th>Kỳ khoán</th>
                    <th>Chỉ tiêu KPI</th>
                    <th>Mục tiêu</th>
                    <th>Thực hiện</th>
                    <th>Điểm</th>
                    <th>Trạng thái</th>
                    <th>Hành động</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="assignment in assignments" :key="assignment.Id" class="assignment-row">
                    <td class="employee-info">
                      <div class="employee-name">
                        {{ assignment.employee?.fullName }}
                      </div>
                      <div class="employee-unit">
                        {{ assignment.employee?.unit?.Name }}
                      </div>
                    </td>
                    <td class="period-info">
                      <div class="period-name">
                        {{ assignment.khoanPeriod?.periodName }}
                      </div>
                      <div class="period-dates">
                        {{ formatDate(assignment.khoanPeriod?.startDate) }} -
                        {{ formatDate(assignment.khoanPeriod?.endDate) }}
                      </div>
                    </td>
                    <td class="indicator-info">
                      <div class="indicator-name">
                        {{ assignment.indicator?.indicatorName }}
                      </div>
                      <div class="indicator-details">
                        Tối đa: {{ assignment.indicator?.maxScore }} điểm ({{ assignment.indicator?.unit }})
                      </div>
                    </td>
                    <td class="target-value">
                      <span class="value">{{ assignment.targetValue }}</span>
                      <span class="unit">{{ assignment.indicator?.unit }}</span>
                    </td>
                    <td class="actual-value">
                      <div v-if="editingAssignment === assignment.Id" class="edit-mode">
                        <input
                          v-model="editingActualValueFormatted"
                          type="text"
                          :placeholder="`Nhập giá trị (${assignment.indicator?.unit})`"
                          class="actual-input"
                          @input="e => handleActualValueInput(e)"
                          @blur="e => handleActualValueBlur(e)"
                          @keyup.enter="saveActualValue(assignment)"
                          @keyup.escape="cancelEdit"
                        >
                      </div>
                      <div v-else class="view-mode">
                        <span v-if="assignment.actualValue != null" class="value">
                          {{ assignment.actualValue }}
                        </span>
                        <span v-else class="no-value">Chưa nhập</span>
                        <span class="unit">{{ assignment.indicator?.unit }}</span>
                      </div>
                    </td>
                    <td class="score-value">
                      <span v-if="assignment.score != null" class="score">
                        {{ assignment.score.toFixed(2) }}
                      </span>
                      <span v-else class="no-score">Chưa tính</span>
                    </td>
                    <td class="status">
                      <span class="status-badge" :class="getStatusClass(assignment)">
                        {{ getStatusText(assignment) }}
                      </span>
                    </td>
                    <td class="actions">
                      <div v-if="editingAssignment === assignment.Id" class="edit-actions">
                        <button
                          :disabled="savingActual"
                          class="action-btn save-btn"
                          title="Lưu giá trị"
                          @click="saveActualValue(assignment)"
                        >
                          ✅ Lưu
                        </button>
                        <button
                          :disabled="savingActual"
                          class="action-btn cancel-btn"
                          title="Hủy chỉnh sửa"
                          @click="cancelEdit"
                        >
                          ❌ Hủy
                        </button>
                      </div>
                      <div v-else class="view-actions">
                        <button
                          class="action-btn execute-btn"
                          title="Cập nhật tình hình thực hiện chỉ tiêu này"
                          @click="startEdit(assignment)"
                        >
                          🎯 Thực hiện
                        </button>
                        <button
                          v-if="assignment.actualValue != null"
                          class="action-btn details-btn"
                          title="Xem chi tiết tính điểm"
                          @click="viewCalculationDetails(assignment)"
                        >
                          📊 Chi tiết
                        </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div
          v-if="
            !searching &&
              assignments.length === 0 &&
              (selectedEmployeeId || selectedPeriodId || selectedBranchId || selectedDepartmentId)
          "
          class="empty-state"
        >
          <div class="form-container">
            <div class="empty-content">
              <div class="empty-icon">
                📋
              </div>
              <h3>Không tìm thấy giao khoán</h3>
              <p>Không có giao khoán KPI nào phù hợp với tiêu chí tìm kiếm.</p>
            </div>
          </div>
        </div>

        <!-- Initial State -->
        <div
          v-if="
            !searching &&
              assignments.length === 0 &&
              !selectedEmployeeId &&
              !selectedPeriodId &&
              !selectedBranchId &&
              !selectedDepartmentId
          "
          class="initial-state"
        >
          <div class="form-container">
            <div class="initial-content">
              <div class="initial-icon">
                🎯
              </div>
              <h3>Chọn tiêu chí tìm kiếm</h3>
              <p>Vui lòng chọn nhân viên hoặc kỳ khoán để hiển thị các giao khoán KPI.</p>
            </div>
          </div>
        </div>
      </div>
      <!-- End Employee Tab -->

      <!-- Unit Tab -->
      <div v-show="activeTab === 'unit'" class="tab-pane">
        <!-- Unit Filter Section -->
        <div class="filter-section">
          <div class="form-container">
            <h2>🔍 Tìm kiếm giao khoán chi nhánh</h2>

            <div class="filter-form">
              <div class="form-row">
                <div class="form-group">
                  <label for="unitBranchFilter">🏢 Chi nhánh:</label>
                  <select id="unitBranchFilter" v-model="selectedUnitBranchId" @change="onUnitBranchChange">
                    <option value="">
                      -- Chọn chi nhánh --
                    </option>
                    <option v-for="branch in branchOptions" :key="branch.Id" :value="branch.Id">
                      🏢 {{ branch.Name }} ({{ branch.Code }})
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <label for="unitPeriodFilter">📅 Kỳ khoán:</label>
                  <select id="unitPeriodFilter" v-model="selectedUnitPeriodId" @change="onUnitFilterChange">
                    <option value="">
                      -- Chọn kỳ khoán --
                    </option>
                    <option v-for="period in khoanPeriods" :key="period.Id" :value="period.Id">
                      {{ period.Name }} ({{ formatDate(period.startDate) }} - {{ formatDate(period.endDate) }})
                    </option>
                  </select>
                </div>

                <div class="form-group">
                  <button
                    :disabled="!canSearchUnit || searchingUnit"
                    class="action-button search-btn"
                    @click="searchUnitAssignments"
                  >
                    {{ searchingUnit ? 'Đang tìm...' : '🔍 Tìm kiếm' }}
                  </button>
                </div>
              </div>

              <div v-if="selectedUnitBranchId" class="filter-summary">
                <p>
                  📊 <strong>Chi nhánh được chọn:</strong> "{{ getUnitBranchName() }}" trong kỳ "{{
                    getUnitPeriodName()
                  }}"
                </p>
              </div>
            </div>
          </div>
        </div>

        <!-- Unit Loading State -->
        <div v-if="searchingUnit" class="loading-section">
          <div class="loading-spinner" />
          <p>Đang tìm kiếm giao khoán chi nhánh...</p>
        </div>

        <!-- Unit Assignments Results -->
        <div v-if="!searchingUnit && unitAssignments.length > 0" class="assignments-section">
          <div class="form-container">
            <div class="assignments-header">
              <h2>📋 Giao khoán KPI Chi nhánh</h2>
              <div class="assignments-summary">
                <span class="summary-item">
                  <strong>{{ unitAssignments.length }}</strong> chỉ tiêu
                </span>
                <span class="summary-item">
                  <strong>{{ unitPendingCount }}</strong> chưa nhập
                </span>
                <span class="summary-item">
                  <strong>{{ unitCompletedCount }}</strong> hoàn thành
                </span>
              </div>
            </div>

            <div class="table-responsive">
              <table class="assignments-table">
                <thead>
                  <tr>
                    <th>STT</th>
                    <th>Chỉ tiêu KPI</th>
                    <th>Mục tiêu</th>
                    <th>Giá trị thực hiện</th>
                    <th>Điểm</th>
                    <th>Trạng thái</th>
                    <th>Thao tác</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(assignment, index) in unitAssignments" :key="assignment.Id">
                    <td>{{ index + 1 }}</td>
                    <td>
                      <div class="kpi-info">
                        <strong>{{ assignment.legacyKPIName || 'Chỉ tiêu KPI' }}</strong>
                        <span class="kpi-description">{{ assignment.legacyKPICode || '' }}</span>
                      </div>
                    </td>
                    <td class="target-cell">
                      <div class="target-info">
                        <span class="target-value">{{ assignment.targetValue || 'Chưa xác định' }}</span>
                        <span class="target-unit">VND</span>
                      </div>
                    </td>
                    <td class="actual-cell">
                      <div v-if="editingUnitAssignment === assignment.Id" class="edit-actual">
                        <input
                          v-model="editingUnitActualValueFormatted"
                          type="text"
                          class="actual-input"
                          @input="e => handleUnitActualValueInput(e)"
                          @blur="e => handleUnitActualValueBlur(e)"
                          @keyup.enter="saveUnitActualValue(assignment)"
                          @keyup.escape="cancelUnitEdit"
                        >
                      </div>
                      <div v-else class="view-actual">
                        <span class="actual-value">{{ assignment.actualValue || 'Chưa nhập' }}</span>
                      </div>
                    </td>
                    <td class="score-cell">
                      <span class="score-value" :class="getStatusClass(assignment)">
                        {{ assignment.score ? assignment.score.toFixed(1) : 'N/A' }}
                      </span>
                    </td>
                    <td class="status-cell">
                      <span class="status-badge" :class="getStatusClass(assignment)">
                        {{ getStatusText(assignment) }}
                      </span>
                    </td>
                    <td class="action-cell">
                      <div v-if="editingUnitAssignment === assignment.Id" class="edit-actions">
                        <button
                          :disabled="savingUnitActual"
                          class="action-btn save-btn"
                          title="Lưu giá trị"
                          @click="saveUnitActualValue(assignment)"
                        >
                          ✅ Lưu
                        </button>
                        <button
                          :disabled="savingUnitActual"
                          class="action-btn cancel-btn"
                          title="Hủy chỉnh sửa"
                          @click="cancelUnitEdit"
                        >
                          ❌ Hủy
                        </button>
                      </div>
                      <div v-else class="view-actions">
                        <button
                          class="action-btn execute-btn"
                          title="Cập nhật tình hình thực hiện chỉ tiêu này"
                          @click="startUnitEdit(assignment)"
                        >
                          🎯 Thực hiện
                        </button>
                        <button
                          v-if="assignment.actualValue != null"
                          class="action-btn details-btn"
                          title="Xem chi tiết tính điểm"
                          @click="viewUnitCalculationDetails(assignment)"
                        >
                          📊 Chi tiết
                        </button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <!-- Unit Empty State -->
        <div
          v-if="!searchingUnit && unitAssignments.length === 0 && (selectedUnitBranchId || selectedUnitPeriodId)"
          class="empty-state"
        >
          <div class="form-container">
            <div class="empty-content">
              <div class="empty-icon">
                📋
              </div>
              <h3>Không tìm thấy giao khoán</h3>
              <p>Không có giao khoán KPI nào cho chi nhánh này trong kỳ được chọn.</p>
            </div>
          </div>
        </div>

        <!-- Unit Initial State -->
        <div
          v-if="!searchingUnit && unitAssignments.length === 0 && !selectedUnitBranchId && !selectedUnitPeriodId"
          class="initial-state"
        >
          <div class="form-container">
            <div class="initial-content">
              <div class="initial-icon">
                🏢
              </div>
              <h3>Chọn chi nhánh và kỳ khoán</h3>
              <p>Vui lòng chọn chi nhánh và kỳ khoán để hiển thị các giao khoán KPI.</p>
            </div>
          </div>
        </div>
      </div>
      <!-- End Unit Tab -->
    </div>
    <!-- End Tab Content -->
  </div>
</template>

<script setup>
import messages from '@/i18n/messages'
import { computed, nextTick, onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import api from '../services/api'
import { isAuthenticated } from '../services/auth'
import { useNumberInput } from '../utils/numberFormat'

const _router = useRouter()

// Reactive data
const searching = ref(false)
const savingActual = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

// Tab management
const activeTab = ref('employee')

const employees = ref([])
const units = ref([])
const khoanPeriods = ref([])
const assignments = ref([])

const selectedEmployeeId = ref('')
const selectedPeriodId = ref('')
const selectedBranchId = ref('')
const selectedDepartmentId = ref('')

const editingAssignment = ref(null)
const editingActualValue = ref('')
const editingActualValueFormatted = ref('')

// Unit tab specific data
const unitAssignments = ref([])
const selectedUnitBranchId = ref('')
const selectedUnitPeriodId = ref('')
const searchingUnit = ref(false)
const editingUnitAssignment = ref(null)
const editingUnitActualValue = ref('')
const editingUnitActualValueFormatted = ref('')
const savingUnitActual = ref(false)

// 🔢 Initialize number input utility
const { handleInput, handleBlur, formatNumber, parseFormattedNumber } = useNumberInput({
  maxDecimalPlaces: 2,
  allowNegative: false,
})

// Computed properties
// Updated branchOptions: Custom ordering as requested
const branchOptions = computed(() => {
  if (!units.value || units.value.length === 0) return []

  // Định nghĩa thứ tự theo yêu cầu mới: Lai Châu -> Hội Sở -> Bình Lư -> Phong Thổ -> Sìn Hồ -> Bum Tở -> Than Uyên -> Đoàn Kết -> Tân Uyên -> Nậm Hàng
  const customOrder = [
    'CnLaiChau', // Chi nhánh tỉnh Lai Châu
    'HoiSo', // Hội Sở
    'CnBinhLu', // Chi nhánh Bình Lư
    'CnPhongTho', // Chi nhánh Phong Thổ
    'CnSinHo', // Chi nhánh Sìn Hồ
    'CnBumTo', // Chi nhánh Bum Tở
    'CnThanUyen', // Chi nhánh Than Uyên
    'CnDoanKet', // Chi nhánh Đoàn Kết
    'CnTanUyen', // Chi nhánh Tân Uyên
    'CnNamHang', // Chi nhánh Nậm Hàng
  ]

  return units.value
    .filter(unit => {
      const type = (unit.Type || '').toUpperCase()
      return type === 'CNL1' || type === 'CNL2'
    })
    .sort((a, b) => {
      const indexA = customOrder.indexOf(a.Code)
      const indexB = customOrder.indexOf(b.Code)

      // Nếu cả hai đều có trong custom order, sắp xếp theo thứ tự đó
      if (indexA !== -1 && indexB !== -1) {
        return indexA - indexB
      }

      // Nếu chỉ có một trong hai có trong custom order, ưu tiên cái đó
      if (indexA !== -1) return -1
      if (indexB !== -1) return 1

      // Nếu cả hai đều không có trong custom order, sắp xếp theo tên
      return (a.Name || '').localeCompare(b.Name || '')
    })
})

const departmentOptions = computed(() => {
  if (!selectedBranchId.value || !units.value || units.value.length === 0) return []

  const branchId = parseInt(selectedBranchId.value)
  const branch = units.value.find(u => u.Id === branchId)
  if (!branch) return []

  const children = units.value.filter(u => (u.parentUnitId || u.ParentUnitId) === branchId)

  // Lọc chỉ lấy các phòng nghiệp vụ (PNVL1, PNVL2) và phòng giao dịch (PGD), loại bỏ các chi nhánh con (CNL2)
  const departments = children.filter(u => {
    const unitType = (u.Type || '').toUpperCase()
    return unitType.includes('PNVL') || unitType === 'PGD'
  })

  // Sắp xếp theo thứ tự: Ban Giám đốc, Phòng Khách hàng, Phòng Kế toán & Ngân quỹ, Phòng giao dịch
  const getDepartmentSortOrder = name => {
    const lowerName = (name || '').toLowerCase()
    if (lowerName.includes('ban giám đốc')) return 1
    if (lowerName.includes('phòng khách hàng')) return 2
    if (lowerName.includes('phòng kế toán')) return 3
    if (lowerName.includes('phòng giao dịch')) return 4
    return 999
  }

  return departments.sort((a, b) => getDepartmentSortOrder(a.Name) - getDepartmentSortOrder(b.Name))
})

const filteredEmployees = computed(() => {
  if (!employees.value || employees.value.length === 0) {
    console.log('🤵 No employees loaded yet')
    return []
  }

  let filtered = [...employees.value]
  console.log('🤵 Total employees:', filtered.length)

  if (selectedDepartmentId.value) {
    const deptId = parseInt(selectedDepartmentId.value)
    filtered = filtered.filter(emp => {
      const empUnitId = emp.unitId || emp.UnitId
      return empUnitId === deptId
    })
    console.log(`🤵 Filtered by department ${deptId}:`, filtered.length)
    console.log(`🤵 Sample filtered employees:`, filtered.slice(0, 2).map(e => ({
      name: e.fullName,
      unitId: e.unitId || e.UnitId,
    })))
  } else if (selectedBranchId.value) {
    const branchDepartments = departmentOptions.value.map(dept => dept.Id)
    console.log('🏢 Branch departments:', branchDepartments)
    filtered = filtered.filter(emp => {
      const empUnitId = emp.unitId || emp.UnitId
      return branchDepartments.includes(empUnitId)
    })
    console.log(`🤵 Filtered by branch:`, filtered.length)
  }

  return filtered
})

const canSearch = computed(() => {
  return selectedEmployeeId.value || selectedPeriodId.value || selectedBranchId.value || selectedDepartmentId.value
})

const pendingCount = computed(() => {
  return assignments.value.filter(a => a.actualValue == null).length
})

const completedCount = computed(() => {
  return assignments.value.filter(a => a.actualValue != null).length
})

// Unit tab computed properties
const unitPendingCount = computed(() => {
  return unitAssignments.value.filter(a => a.actualValue == null).length
})

const unitCompletedCount = computed(() => {
  return unitAssignments.value.filter(a => a.actualValue != null).length
})

const canSearchUnit = computed(() => {
  return selectedUnitBranchId.value && selectedUnitPeriodId.value
})

// Methods

const clearMessages = () => {
  errorMessage.value = ''
  successMessage.value = ''
}

const showError = message => {
  errorMessage.value = message
  successMessage.value = ''
  setTimeout(() => {
    errorMessage.value = ''
  }, 5000)
}

const showSuccess = message => {
  successMessage.value = message
  errorMessage.value = ''
  setTimeout(() => {
    successMessage.value = ''
  }, 3000)
}

const formatDate = dateString => {
  if (!dateString) return ''
  const date = new Date(dateString)
  return date.toLocaleDateString('vi-VN')
}

const getBranchName = () => {
  if (!selectedBranchId.value) return ''
  const branch = units.value.find(u => u.Id === parseInt(selectedBranchId.value))
  return branch ? branch.Name : ''
}

const getDepartmentName = () => {
  if (!selectedDepartmentId.value) return ''
  const dept = units.value.find(u => u.Id === parseInt(selectedDepartmentId.value))
  return dept ? dept.Name : ''
}

const onBranchChange = () => {
  console.log('🔄 Branch changed to:', selectedBranchId.value)
  selectedDepartmentId.value = ''
  selectedEmployeeId.value = ''
  assignments.value = []
  clearMessages()

  // Force reactivity update
  nextTick(() => {
    console.log('📊 Available departments for branch:', departmentOptions.value.length)
    console.log('📊 Department options:', departmentOptions.value.map(d => ({ id: d.Id, name: d.Name })))
    console.log('🤵 Available employees for branch:', filteredEmployees.value.length)
  })
}

const onDepartmentChange = () => {
  console.log('🔄 Department changed to:', selectedDepartmentId.value)
  selectedEmployeeId.value = ''
  assignments.value = []
  clearMessages()

  // Force reactivity update
  nextTick(() => {
    console.log('🤵 Available employees for department:', filteredEmployees.value.length)
    console.log('🤵 Employee options:', filteredEmployees.value.map(e => ({ id: e.Id, name: e.fullName, unitId: e.unitId || e.UnitId })))
  })
}

const getStatusClass = assignment => {
  if (assignment.actualValue != null) {
    if (assignment.score != null && assignment.score > 0) {
      return 'completed'
    } else {
      return 'entered'
    }
  }
  return 'pending'
}

const getStatusText = assignment => {
  if (assignment.actualValue != null) {
    if (assignment.score != null && assignment.score > 0) {
      return 'Hoàn thành'
    } else {
      return 'Đã nhập'
    }
  }
  return 'Chưa nhập'
}

const fetchEmployees = async () => {
  try {
    console.log('🔄 Fetching employees...')
    const response = await api.get('/Employees')
    let employeesData = []

    if (response.data && response.data.Data && Array.isArray(response.data.Data)) {
      employeesData = response.data.Data
    } else if (response.data && Array.isArray(response.data.$values)) {
      employeesData = response.data.$values
    } else if (Array.isArray(response.data)) {
      employeesData = response.data
    }

    employees.value = employeesData.filter(emp => emp.isActive)
    console.log('✅ Employees loaded:', employees.value.length)

    // Debug: Log sample employee structure
    if (employees.value.length > 0) {
      console.log('📊 Sample employee structure:', {
        id: employees.value[0].Id,
        name: employees.value[0].fullName,
        unitId: employees.value[0].unitId,
        UnitId: employees.value[0].UnitId,
        unit: employees.value[0].unit,
      })

      // Debug: Log first few employees with unit mapping
      console.log('📊 First 3 employees with unit mapping:')
      employees.value.slice(0, 3).forEach((emp, idx) => {
        console.log(`${idx + 1}.`, {
          name: emp.fullName,
          unitId: emp.unitId || emp.UnitId,
          hasUnit: !!emp.unit,
        })
      })
    }
  } catch (error) {
    console.error('❌ Error loading employees:', error)
    showError('Không thể tải danh sách nhân viên. Vui lòng thử lại.')
    throw error // Re-throw to be caught by Promise.all
  }
}

const fetchKhoanPeriods = async () => {
  try {
    console.log('🔄 Fetching khoan periods...')
    const response = await api.get('/KhoanPeriods')
    let periodsData = []

    if (response.data && response.data.Data && Array.isArray(response.data.Data)) {
      periodsData = response.data.Data
    } else if (response.data && Array.isArray(response.data.$values)) {
      periodsData = response.data.$values
    } else if (Array.isArray(response.data)) {
      periodsData = response.data
    }

    khoanPeriods.value = periodsData.filter(
      period => period.Status === 'OPEN' || period.Status === 'PROCESSING' || period.Status === 'PENDINGAPPROVAL' || period.Status === 'DRAFT',
    )
    console.log('✅ Khoan Periods loaded:', khoanPeriods.value.length)
  } catch (error) {
    console.error('❌ Error loading khoan periods:', error)
    showError('Không thể tải danh sách kỳ khoán. Vui lòng thử lại.')
    throw error // Re-throw to be caught by Promise.all
  }
}

const fetchUnits = async () => {
  try {
    console.log('🔄 Fetching units...')
    const response = await api.get('/Units')
    let unitsData = []

    if (response.data && response.data.Data && Array.isArray(response.data.Data)) {
      unitsData = response.data.Data
    } else if (response.data && Array.isArray(response.data.$values)) {
      unitsData = response.data.$values
    } else if (Array.isArray(response.data)) {
      unitsData = response.data
    }

    units.value = unitsData
    console.log('✅ Units loaded:', units.value.length)

    // Debug: Log sample unit structure
    if (units.value.length > 0) {
      console.log('📊 Sample unit structure:', {
        id: units.value[0].Id,
        name: units.value[0].Name,
        parentUnitId: units.value[0].parentUnitId,
        ParentUnitId: units.value[0].ParentUnitId,
        type: units.value[0].Type,
      })
    }
  } catch (error) {
    console.error('❌ Error loading units:', error)
    showError('Không thể tải danh sách đơn vị. Vui lòng thử lại.')
    throw error // Re-throw to be caught by Promise.all
  }
}

const onFilterChange = () => {
  assignments.value = []
  clearMessages()
}

const searchAssignments = async () => {
  if (!canSearch.value) {
    showError('Vui lòng chọn ít nhất một tiêu chí tìm kiếm.')
    return
  }

  try {
    searching.value = true
    clearMessages()

    // Build search URL based on filters
    let url = '/KpiAssignment/search?'
    const params = []

    if (selectedEmployeeId.value) {
      params.push(`employeeId=${selectedEmployeeId.value}`)
    }

    if (selectedPeriodId.value) {
      params.push(`periodId=${selectedPeriodId.value}`)
    }

    // Add unit-based filtering
    if (selectedDepartmentId.value) {
      params.push(`unitId=${selectedDepartmentId.value}`)
    } else if (selectedBranchId.value && !selectedEmployeeId.value) {
      // If branch selected but no specific department/employee, get all departments under the branch
      const branchDepartments = departmentOptions.value.map(dept => dept.Id)
      if (branchDepartments.length > 0) {
        // For simplicity, we'll use the first department or handle multiple department search differently
        // Note: This is a limitation of the current API that only accepts single unitId
        // A better approach would be to extend API to accept multiple unitIds
        params.push(`unitId=${branchDepartments[0]}`)
      }
    }

    url += params.join('&')

    const response = await api.get(url)
    let assignmentsData = []
    let employeesFromResponse = []

    // New backend wrapper: { assignments: [...], employees: [...] }
    const data = response.data
    if (data && data.assignments) {
      assignmentsData = Array.isArray(data.assignments) ? data.assignments : []
      if (Array.isArray(data.employees)) {
        // Merge employees if current employees list empty or filtered context requires
        if (!employees.value || employees.value.length === 0) {
          employees.value = data.employees.map(e => ({
            Id: e.id || e.Id,
            fullName: e.fullName || e.FullName,
            unitId: e.unitId || e.UnitId,
            positionName: e.positionName || e.PositionName,
            isActive: true,
          }))
        }
        employeesFromResponse = data.employees
      }
    } else if (data && data.Data && Array.isArray(data.Data)) {
      assignmentsData = data.Data
    } else if (data && Array.isArray(data.$values)) {
      assignmentsData = data.$values
    } else if (Array.isArray(data)) {
      assignmentsData = data
    }

    assignments.value = assignmentsData

    console.log('Assignments loaded:', assignments.value.length, 'employeesFromResponse:', employeesFromResponse.length)

    if (assignments.value.length > 0) {
      showSuccess(`Tìm thấy ${assignments.value.length} giao khoán KPI.`)
    } else if (employeesFromResponse.length > 0) {
      // No assignments yet, but employees exist => guide user
      successMessage.value = `Chưa có giao khoán KPI cho kỳ đã chọn. Có ${employeesFromResponse.length} cán bộ trong đơn vị này. Chọn cán bộ và tiến hành giao hoặc cập nhật khi có dữ liệu.`
    } else if (assignments.value.length === 0 && !searching.value) {
      successMessage.value = 'Không có giao khoán hoặc danh sách cán bộ cho tiêu chí tìm kiếm hiện tại.'
    }
  } catch (error) {
    console.error('Error searching assignments:', error)
    showError('Có lỗi xảy ra khi tìm kiếm. Vui lòng thử lại.')
  } finally {
    searching.value = false
  }
}

const startEdit = assignment => {
  editingAssignment.value = assignment.Id
  editingActualValue.value = assignment.actualValue || ''
  editingActualValueFormatted.value = assignment.actualValue ? formatNumber(assignment.actualValue) : ''
}

const cancelEdit = () => {
  editingAssignment.value = null
  editingActualValue.value = ''
  editingActualValueFormatted.value = ''
}

const saveActualValue = async assignment => {
  try {
    savingActual.value = true
    clearMessages()

    const updateData = {
      assignmentId: assignment.Id,
      actualValue: editingActualValue.value ? parseFloat(editingActualValue.value) : null,
    }

    const response = await api.put('/KpiAssignment/update-single-actual', updateData)

    // Update local data
    const index = assignments.value.findIndex(a => a.Id === assignment.Id)
    if (index !== -1) {
      assignments.value[index].actualValue = updateData.actualValue

      // Update score if returned from API
      if (response.data && response.data.score != null) {
        assignments.value[index].score = response.data.score
      }
    }

    showSuccess('Cập nhật giá trị thực hiện thành công!')

    // Exit edit mode
    cancelEdit()
  } catch (error) {
    console.error('Error updating actual value:', error)
    if (error.response && error.response.data && error.response.data.message) {
      showError(`Lỗi cập nhật: ${error.response.data.message}`)
    } else {
      showError('Có lỗi xảy ra khi cập nhật giá trị. Vui lòng thử lại.')
    }
  } finally {
    savingActual.value = false
  }
}

// Number input handlers for employee actual values
const handleActualValueInput = event => {
  const formattedValue = handleInput(event)
  editingActualValueFormatted.value = formattedValue
  editingActualValue.value = parseFormattedNumber(formattedValue)
}

const handleActualValueBlur = event => {
  const formattedValue = handleBlur(event)
  editingActualValueFormatted.value = formattedValue
  editingActualValue.value = parseFormattedNumber(formattedValue)
}

// Unit tab methods
const searchUnitAssignments = async () => {
  if (!canSearchUnit.value) {
    showError('Vui lòng chọn chi nhánh và kỳ khoán.')
    return
  }

  try {
    searchingUnit.value = true
    clearMessages()

    console.log('🔍 Searching unit assignments...', {
      unitId: selectedUnitBranchId.value,
      periodId: selectedUnitPeriodId.value,
    })

    // Use dedicated backend endpoint for unit assignments
    const response = await api.get(
      `/UnitKhoanAssignments/search?unitId=${selectedUnitBranchId.value}&periodId=${selectedUnitPeriodId.value}`,
    )

    let assignmentsData = []
    if (response.data && response.data.Data && Array.isArray(response.data.Data)) {
      assignmentsData = response.data.Data
    } else if (response.data && Array.isArray(response.data.$values)) {
      assignmentsData = response.data.$values
    } else if (Array.isArray(response.data)) {
      assignmentsData = response.data
    }

    unitAssignments.value = assignmentsData

    console.log('✅ Unit assignments loaded:', unitAssignments.value.length)

    if (unitAssignments.value.length > 0) {
      showSuccess(`Tìm thấy ${unitAssignments.value.length} giao khoán KPI cho chi nhánh.`)
    } else {
      // Gọi empty-info để lấy metadata
      await fetchUnitEmptyInfo()
    }
  } catch (error) {
    console.error('❌ Error searching unit assignments:', error)
    showError('Có lỗi xảy ra khi tìm kiếm. Vui lòng thử lại.')
  } finally {
    searchingUnit.value = false
  }
}

const startUnitEdit = assignment => {
  editingUnitAssignment.value = assignment.Id
  editingUnitActualValue.value = assignment.actualValue || ''
  editingUnitActualValueFormatted.value = assignment.actualValue ? formatNumber(assignment.actualValue) : ''
}

const cancelUnitEdit = () => {
  editingUnitAssignment.value = null
  editingUnitActualValue.value = ''
  editingUnitActualValueFormatted.value = ''
}

const saveUnitActualValue = async assignment => {
  try {
    savingUnitActual.value = true
    clearMessages()

    const updateData = {
      assignmentDetailId: assignment.Id, // backend expects detail id
      actualValue: editingUnitActualValue.value ? parseFloat(editingUnitActualValue.value) : null,
    }

    console.log('💾 Saving unit actual value:', updateData)

  const response = await api.put('/UnitKhoanAssignments/update-actual', updateData)

    // Update local data
    const index = unitAssignments.value.findIndex(a => a.Id === assignment.Id)
    if (index !== -1) {
      unitAssignments.value[index].actualValue = updateData.actualValue

      // Update score if returned from API
      if (response.data && response.data.score != null) {
        unitAssignments.value[index].score = response.data.score
      }
    }

  showSuccess(messages.unitUpdateSuccess)

    // Exit edit mode
    cancelUnitEdit()
  } catch (error) {
    console.error('❌ Error updating unit actual value:', error)
    if (error.response && error.response.data && error.response.data.message) {
      showError(`Lỗi cập nhật: ${error.response.data.message}`)
    } else {
      showError('Có lỗi xảy ra khi cập nhật giá trị cho chi nhánh. Vui lòng thử lại.')
    }
  } finally {
    savingUnitActual.value = false
  }
}

// Number input handlers for unit actual values
const handleUnitActualValueInput = event => {
  const formattedValue = handleInput(event)
  editingUnitActualValueFormatted.value = formattedValue
  editingUnitActualValue.value = parseFormattedNumber(formattedValue)
}

const handleUnitActualValueBlur = event => {
  const formattedValue = handleBlur(event)
  editingUnitActualValueFormatted.value = formattedValue
  editingUnitActualValue.value = parseFormattedNumber(formattedValue)
}

const onUnitFilterChange = () => {
  unitAssignments.value = []
  clearMessages()
}

const onUnitBranchChange = () => {
  unitAssignments.value = []
  clearMessages()
}

// Lấy empty info khi không có assignment
const unitEmptyInfo = ref(null)
const fetchingEmptyInfo = ref(false)
const fetchUnitEmptyInfo = async () => {
  if (!selectedUnitBranchId.value || !selectedUnitPeriodId.value) return
  try {
    fetchingEmptyInfo.value = true
    successMessage.value = messages.loadingUnitEmptyInfo
    const response = await api.get(`/UnitKhoanAssignments/empty-info?unitId=${selectedUnitBranchId.value}&periodId=${selectedUnitPeriodId.value}`)
    unitEmptyInfo.value = response.data
    if (unitEmptyInfo.value) {
      successMessage.value = `${messages.emptyUnitAssignments(unitEmptyInfo.value.unitName, unitEmptyInfo.value.periodName)} ${messages.noAssignmentsHint}`
    } else {
      successMessage.value = messages.emptyEmployeeAssignments
    }
  } catch (err) {
    console.error('empty-info error', err)
    showError(messages.unitEmptyInfoError)
  } finally {
    fetchingEmptyInfo.value = false
  }
}

const getUnitBranchName = () => {
  if (!selectedUnitBranchId.value) return ''
  const branch = units.value.find(u => u.Id === parseInt(selectedUnitBranchId.value))
  return branch ? branch.Name : ''
}

const getUnitPeriodName = () => {
  if (!selectedUnitPeriodId.value) return ''
  const period = khoanPeriods.value.find(p => p.Id === parseInt(selectedUnitPeriodId.value))
  return period ? period.Name : ''
}

// Tab management methods
const switchTab = tabName => {
  activeTab.value = tabName
  clearMessages()

  // Clear data when switching tabs
  if (tabName === 'employee') {
    unitAssignments.value = []
    selectedUnitBranchId.value = ''
    selectedUnitPeriodId.value = ''
  } else if (tabName === 'unit') {
    assignments.value = []
    selectedEmployeeId.value = ''
    selectedPeriodId.value = ''
    selectedBranchId.value = ''
    selectedDepartmentId.value = ''
  }
}

// View calculation details
const viewCalculationDetails = (assignment) => {
  clearMessages()

  const unit = assignment.indicator?.unit || 'N/A'
  const targetValue = assignment.targetValue || 0
  const actualValue = assignment.actualValue || 0
  const score = assignment.score || 0
  const maxScore = assignment.indicator?.maxScore || 0

  // Calculate completion percentage
  const completionRate = targetValue > 0 ? (actualValue / targetValue * 100).toFixed(2) : 0

  // Determine performance status
  let performanceStatus = ''
  let _statusClass = ''

  if (completionRate >= 100) {
    performanceStatus = '✅ Đạt chỉ tiêu'
    _statusClass = 'success'
  } else if (completionRate >= 80) {
    performanceStatus = '⚠️ Gần đạt chỉ tiêu'
    _statusClass = 'warning'
  } else {
    performanceStatus = '❌ Chưa đạt chỉ tiêu'
    _statusClass = 'danger'
  }

  const detailsMessage = `
📊 CHI TIẾT TÍNH ĐIỂM KPI

👤 Cán bộ: ${assignment.employee?.fullName}
📋 Chỉ tiêu: ${assignment.indicator?.indicatorName}
📅 Kỳ khoán: ${assignment.khoanPeriod?.periodName}

🎯 MỤC TIÊU VÀ THỰC HIỆN:
• Chỉ tiêu giao: ${formatNumber(targetValue)} ${unit}
• Giá trị thực hiện: ${formatNumber(actualValue)} ${unit}
• Tỷ lệ hoàn thành: ${completionRate}%

⭐ ĐIỂM SỐ:
• Điểm đạt được: ${score.toFixed(2)}/${maxScore} điểm
• Trạng thái: ${performanceStatus}

💡 Ghi chú: Cách tính điểm chi tiết sẽ được cập nhật theo quy chế của từng chỉ tiêu.
  `.trim()

  showSuccess(detailsMessage)
}

// View calculation details for Unit KPI
const viewUnitCalculationDetails = (assignment) => {
  clearMessages()

  const targetValue = assignment.targetValue || 0
  const actualValue = assignment.actualValue || 0
  const score = assignment.score || 0

  // Calculate completion percentage
  const completionRate = targetValue > 0 ? (actualValue / targetValue * 100).toFixed(2) : 0

  // Determine performance status
  let performanceStatus = ''

  if (completionRate >= 100) {
    performanceStatus = '✅ Đạt chỉ tiêu'
  } else if (completionRate >= 80) {
    performanceStatus = '⚠️ Gần đạt chỉ tiêu'
  } else {
    performanceStatus = '❌ Chưa đạt chỉ tiêu'
  }

  const detailsMessage = `
📊 CHI TIẾT TÍNH ĐIỂM KPI CHI NHÁNH

🏢 Chi nhánh: ${getSelectedBranchName()}
📋 Chỉ tiêu: ${assignment.legacyKPIName || 'Chỉ tiêu KPI'}
📅 Kỳ khoán: ${getSelectedPeriodName()}

🎯 MỤC TIÊU VÀ THỰC HIỆN:
• Chỉ tiêu giao: ${formatNumber(targetValue)} VND
• Giá trị thực hiện: ${formatNumber(actualValue)} VND
• Tỷ lệ hoàn thành: ${completionRate}%

⭐ ĐIỂM SỐ:
• Điểm đạt được: ${score.toFixed(2)} điểm
• Trạng thái: ${performanceStatus}

💡 Ghi chú: Cách tính điểm chi tiết sẽ được cập nhật theo quy chế của từng chỉ tiêu chi nhánh.
  `.trim()

  showSuccess(detailsMessage)
}

// Helper functions for Unit tab
const getSelectedBranchName = () => {
  if (!selectedUnitBranchId.value || !units.value) return 'N/A'
  const branch = units.value.find(u => u.Id === parseInt(selectedUnitBranchId.value))
  return branch ? branch.Name : 'N/A'
}

const getSelectedPeriodName = () => {
  if (!selectedUnitPeriodId.value || !khoanPeriods.value) return 'N/A'
  const period = khoanPeriods.value.find(p => p.Id === parseInt(selectedUnitPeriodId.value))
  return period ? period.Name : 'N/A'
}

// Lifecycle
onMounted(async () => {
  // Note: Authentication is temporarily bypassed via router meta.public for debugging
  if (!isAuthenticated()) {
    console.warn('⚠️ User not authenticated, but route allows public access for debugging')
  }

  try {
    console.log('🔄 KpiActualValuesView: Starting initial data load...')

    // Load data sequentially to avoid race conditions
    console.log('📊 Loading employees...')
    await fetchEmployees()

    console.log('🏢 Loading units...')
    await fetchUnits()

    console.log('📅 Loading khoan periods...')
    await fetchKhoanPeriods()

    console.log('✅ KpiActualValuesView: All data loaded successfully')
    console.log(
      `📊 Loaded: ${employees.value.length} employees, ${units.value.length} units, ${khoanPeriods.value.length} periods`,
    )
  } catch (error) {
    console.error('❌ KpiActualValuesView: Error loading initial data:', error)
    showError('Không thể tải dữ liệu ban đầu. Vui lòng tải lại trang.')
  }
})

// Watch for debugging filtered employees
watch(filteredEmployees, (newVal) => {
  console.log('👁️ filteredEmployees changed:', newVal.length, 'employees')
}, { immediate: true })

watch(selectedDepartmentId, (newVal) => {
  console.log('👁️ selectedDepartmentId changed to:', newVal)
}, { immediate: true })
</script>

<style scoped>
.kpi-actual-values {
  max-width: 1400px;
  margin: 0 auto;
  padding: 20px;
}

.header-section {
  text-align: center;
  margin-bottom: 30px;
}

.header-section h1 {
  color: #8b1538;
  font-size: 2.5rem;
  font-weight: 700;
  margin-bottom: 10px;
}

.subtitle {
  color: #a91b47;
  font-size: 1.2rem;
  font-weight: 500;
}

/* Tab Navigation Styles */
.tab-navigation {
  display: flex;
  justify-content: center;
  margin-bottom: 30px;
  border-bottom: 2px solid #e2e8f0;
}

.tab-button {
  padding: 12px 24px;
  font-size: 16px;
  font-weight: 600;
  border: none;
  background: transparent;
  cursor: pointer;
  color: #6c757d;
  border-bottom: 3px solid transparent;
  transition: all 0.3s ease;
  margin: 0 10px;
}

.tab-button:hover {
  color: #8b1538;
  background-color: #f8f9fa;
}

.tab-button.active {
  color: #8b1538;
  border-bottom-color: #8b1538;
  background-color: #fff;
}

.tab-content {
  min-height: 400px;
}

.tab-pane {
  display: block;
}

@media (max-width: 768px) {
  .tab-navigation {
    flex-direction: column;
    align-items: center;
  }

  .tab-button {
    margin: 5px 0;
    width: 200px;
  }
}

.error-message {
  background-color: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
  padding: 12px 18px;
  margin-bottom: 20px;
  border-radius: 6px;
  text-align: center;
}

.success-message {
  background-color: #d4edda;
  color: #155724;
  border: 1px solid #c3e6cb;
  padding: 12px 18px;
  margin-bottom: 20px;
  border-radius: 6px;
  text-align: center;
}

.loading-section {
  text-align: center;
  padding: 40px;
}

.loading-spinner {
  border: 3px solid rgba(139, 21, 56, 0.3);
  border-radius: 50%;
  border-top: 3px solid #8b1538;
  width: 30px;
  height: 30px;
  animation: spin 1s linear infinite;
  display: inline-block;
  margin-bottom: 16px;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

.form-container {
  background-color: #ffffff;
  padding: 25px;
  border-radius: 12px;
  border: 1px solid #dde0e3;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
  margin-bottom: 25px;
}

.form-container h2 {
  margin-top: 0;
  margin-bottom: 20px;
  color: #8b1538;
  font-size: 1.5rem;
  font-weight: 600;
}

.filter-form {
  margin-top: 20px;
}

.form-row {
  display: flex;
  gap: 20px;
  align-items: end;
  margin-bottom: 20px;
}

.form-group {
  flex: 1;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  font-weight: 600;
  color: #2c3e50;
}

.form-group select {
  width: 100%;
  padding: 12px 16px;
  border: 2px solid #e2e8f0;
  border-radius: 8px;
  font-size: 14px;
  background-color: #ffffff;
  transition: border-color 0.3s ease;
}

.form-group select:focus {
  outline: none;
  border-color: #8b1538;
  box-shadow: 0 0 0 3px rgba(139, 21, 56, 0.1);
}

.action-button {
  padding: 12px 20px;
  font-size: 14px;
  font-weight: 600;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  height: fit-content;
}

.action-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.search-btn {
  background: linear-gradient(135deg, #8b1538, #a91b47);
  color: white;
  min-width: 120px;
}

.search-btn:hover:not(:disabled) {
  background: linear-gradient(135deg, #6b1028, #891b42);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(139, 21, 56, 0.3);
}

.assignments-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  flex-wrap: wrap;
  gap: 15px;
}

.assignments-summary {
  display: flex;
  gap: 20px;
  flex-wrap: wrap;
}

.summary-item {
  color: #6c757d;
  font-size: 0.9rem;
}

.summary-item strong {
  color: #8b1538;
  font-size: 1.1rem;
}

.assignments-table {
  overflow-x: auto;
}

.assignments-table table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 15px;
  min-width: 1200px;
}

.assignments-table th,
.assignments-table td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #dee2e6;
  vertical-align: top;
}

.assignments-table th {
  background-color: #f8f9fa;
  font-weight: 600;
  color: #495057;
  border-bottom: 2px solid #dee2e6;
  position: sticky;
  top: 0;
  z-index: 10;
}

.assignment-row:hover {
  background-color: #f8f9fa;
}

.employee-info,
.period-info,
.indicator-info {
  min-width: 150px;
}

.employee-name,
.period-name,
.indicator-name {
  font-weight: 600;
  color: #2c3e50;
  margin-bottom: 4px;
}

.employee-unit,
.period-dates,
.indicator-details {
  font-size: 0.85rem;
  color: #6c757d;
}

.target-value,
.actual-value,
.score-value {
  text-align: center;
  min-width: 100px;
}

.value {
  font-weight: 600;
  color: #2c3e50;
}

.unit {
  font-size: 0.85rem;
  color: #6c757d;
  margin-left: 4px;
}

.no-value,
.no-score {
  color: #6c757d;
  font-style: italic;
}

.score {
  font-weight: 600;
  color: #8b1538;
}

.actual-input {
  width: 100%;
  padding: 6px 8px;
  border: 2px solid #8b1538;
  border-radius: 4px;
  font-size: 14px;
  text-align: center;
}

.actual-input:focus {
  outline: none;
  box-shadow: 0 0 0 2px rgba(139, 21, 56, 0.2);
}

.status {
  text-align: center;
  min-width: 100px;
}

.status-badge {
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 0.75rem;
  font-weight: 600;
}

.status-badge.pending {
  background-color: #fff3cd;
  color: #856404;
}

.status-badge.entered {
  background-color: #cce5ff;
  color: #004085;
}

.status-badge.completed {
  background-color: #d4edda;
  color: #155724;
}

.actions {
  text-align: center;
  min-width: 100px;
}

.edit-actions,
.view-actions {
  display: flex;
  gap: 5px;
  justify-content: center;
}

.action-btn {
  padding: 6px 12px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 12px;
  font-weight: 500;
  transition: all 0.3s ease;
  white-space: nowrap;
}

.action-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.edit-btn {
  background: #ffc107;
  color: #212529;
}

.edit-btn:hover:not(:disabled) {
  background: #e0a800;
}

.execute-btn {
  background: #8b1538;
  color: white;
  font-weight: 600;
}

.execute-btn:hover:not(:disabled) {
  background: #a01a42;
  transform: translateY(-1px);
  box-shadow: 0 2px 4px rgba(139, 21, 56, 0.3);
}

.details-btn {
  background: #17a2b8;
  color: white;
}

.details-btn:hover:not(:disabled) {
  background: #138496;
}

.save-btn {
  background: #28a745;
  color: white;
}

.save-btn:hover:not(:disabled) {
  background: #218838;
}

.cancel-btn {
  background: #6c757d;
  color: white;
}

.cancel-btn:hover:not(:disabled) {
  background: #5a6268;
}

.empty-state,
.initial-state {
  margin-top: 20px;
}

.empty-content,
.initial-content {
  text-align: center;
  padding: 60px 40px;
}

.empty-icon,
.initial-icon {
  font-size: 4rem;
  margin-bottom: 20px;
}

.empty-content h3,
.initial-content h3 {
  color: #8b1538;
  font-size: 1.5rem;
  font-weight: 600;
  margin-bottom: 10px;
}

.empty-content p,
.initial-content p {
  color: #6c757d;
  font-size: 1.1rem;
  line-height: 1.6;
}

.filter-summary {
  margin-top: 15px;
  padding: 12px 15px;
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  border-left: 4px solid #8b1538;
  border-radius: 6px;
  font-size: 0.9rem;
}

.filter-summary p {
  margin: 0;
  color: #495057;
}

.filter-summary strong {
  color: #8b1538;
}

/* Responsive Design */
@media (max-width: 768px) {
  .kpi-actual-values {
    padding: 15px;
  }

  .header-section h1 {
    font-size: 2rem;
  }

  .form-row {
    flex-direction: column;
    gap: 0;
  }

  .form-group {
    margin-bottom: 20px;
  }

  .assignments-header {
    flex-direction: column;
    align-items: flex-start;
  }

  .assignments-summary {
    justify-content: space-between;
    width: 100%;
  }

  .assignments-table {
    margin: 0 -25px;
    padding: 0 25px;
  }
}

@media (max-width: 480px) {
  .header-section h1 {
    font-size: 1.8rem;
  }

  .subtitle {
    font-size: 1rem;
  }

  .form-container {
    padding: 20px;
  }

  .assignments-summary {
    flex-direction: column;
    gap: 8px;
  }
}
</style>
