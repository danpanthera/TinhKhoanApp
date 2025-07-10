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
              <option v-for="period in khoanPeriods" :key="getId(period)" :value="getId(period)">
                {{ getName(period) }} ({{ formatDate(safeGet(period, 'StartDate')) }} - {{ formatDate(safeGet(period, 'EndDate')) }})
              </option>
            </select>
          </div>
        </div>
      </div>

      <!-- Branch and Department Filter for Employees -->
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
                  <option v-for="branch in branchOptions" :key="branch.Id" :value="branch.Id">
                    🏢 {{ branch.Name }} ({{ branch.Code }})
                  </option>
                </select>
              </div>
            </div>

            <div class="col" v-if="selectedBranchId">
              <div class="form-group">
                <label class="form-label">🏬 Phòng ban:</label>
                <select v-model="selectedDepartmentId" @change="onDepartmentChange" class="form-control">
                  <option value="">-- Tất cả phòng ban --</option>
                  <option v-for="dept in departmentOptions" :key="dept.Id" :value="dept.Id">
                    🏬 {{ dept.Name }} ({{ dept.Code }})
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

      <!-- Employee Table (moved up to show right after filtering) -->
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
              <tr v-for="employee in filteredEmployees" :key="employee.Id">
                <td style="text-align: center;">
                  <input
                    type="checkbox"
                    :value="employee.Id"
                    v-model="selectedEmployeeIds"
                    @change="validateEmployeeRoles"
                  />
                </td>
                <td>
                  <div>
                    <div class="font-weight-semibold">{{ safeGet(employee, 'FullName') }}</div>
                    <small class="text-muted">{{ safeGet(employee, 'EmployeeCode') }}</small>
                  </div>
                </td>
                <td>
                  <span class="badge-agribank badge-danger">{{ getEmployeeRole(employee) }}</span>
                </td>
                <td>
                  <span class="text-secondary">{{ getUnitName(safeGet(employee, 'UnitId')) }}</span>
                </td>
                <td>
                  <span class="text-secondary">{{ safeGet(employee, 'PositionName') || 'N/A' }}</span>
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

      <!-- KPI Table Selection (appears after selecting employees) -->
      <div class="card-agribank" v-if="selectedEmployeeIds.length > 0">
        <div class="card-header">
          <h3 class="card-title">📊 Chọn bảng KPI cho Cán bộ</h3>
        </div>
        <div class="card-body">
          <div class="form-group">
            <label class="form-label">📋 Bảng KPI:</label>
            <select v-model="selectedTableId" @change="onTableChange" class="form-control">
              <option value="">-- Chọn bảng KPI --</option>
              <option v-for="table in staffKpiTables" :key="getId(table)" :value="getId(table)">
                📊 {{ safeGet(table, 'Description') || safeGet(table, 'TableName') }} ({{ safeGet(table, 'IndicatorCount') }} chỉ tiêu)
              </option>
            </select>
          </div>

          <div class="alert-agribank alert-info" v-if="selectedTableId && selectedKpiTable">
            <strong>📊 Đã chọn:</strong>
            "{{ safeGet(selectedKpiTable, 'Description') || safeGet(selectedKpiTable, 'TableName') }}" → <strong>{{ safeGet(selectedKpiTable, 'IndicatorCount') }}</strong> chỉ tiêu KPI
          </div>
        </div>
      </div>

      <!-- KPI Indicators Table -->
      <div v-if="selectedTableId && indicators.length > 0" class="card-agribank">
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
              <tr v-for="(indicator, index) in indicators" :key="indicator.Id">
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
                    :value="formatTargetValue(indicator, targetValues[indicator.Id])"
                    @input="(e) => handleTargetInput(e, indicator.Id)"
                    @blur="(e) => handleTargetBlur(e, indicator.Id)"
                    placeholder="Nhập mục tiêu"
                    class="form-control"
                    style="font-size: 0.85rem; padding: 8px 12px;"
                    :class="{ 'error': targetErrors[indicator.Id] }"
                  />
                  <div v-if="targetErrors[indicator.Id]" class="text-danger" style="font-size: 0.75rem; margin-top: 4px;">
                    {{ targetErrors[indicator.Id] }}
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
                <td style="text-align: center;"><strong class="badge-agribank badge-primary">{{ getTotalMaxScore() }}</strong></td>
                <td><strong class="text-secondary">{{ getTotalTargets() }} mục tiêu</strong></td>
                <td style="text-align: center;"><strong class="badge-agribank badge-success">{{ getTotalScore() }}</strong></td>
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
import { computed, onMounted, ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import api from '../services/api.js';
import { logApiResponse, normalizeNetArray } from '../utils/apiHelpers.js';
import { getId, getName, safeGet } from '../utils/casingSafeAccess.js';
import { useNumberInput } from '../utils/numberFormat';

const router = useRouter();

// 🔢 Initialize number input utility
const { handleInput, handleBlur, formatNumber, parseFormattedNumber } = useNumberInput({
  maxDecimalPlaces: 2,
  allowNegative: false
});

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

// Enhanced number input handlers for employee targets with unit-specific validation
const handleTargetInput = (event, indicatorId) => {
  const indicator = indicators.value.find(ind => ind.Id === indicatorId);
  const unit = getIndicatorUnit(indicator);

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
      targetErrors.value[indicatorId] = 'Giá trị tối đa là 100%';
    } else {
      delete targetErrors.value[indicatorId];
    }
  }

  // For "Triệu VND", format with thousand separators and limit to 8 digits
  if (unit === 'Triệu VND') {
    // Remove formatting first to get clean number
    let cleanNumber = numericValue.replace(/[,.\s]/g, '');

    // Limit to 8 digits maximum
    if (cleanNumber.length > 8) {
      cleanNumber = cleanNumber.substring(0, 8);
      targetErrors.value[indicatorId] = 'Giá trị tối đa là 8 chữ số (99,999,999 triệu VND)';
    } else {
      delete targetErrors.value[indicatorId];
    }

    const numValue = parseFloat(cleanNumber);
    if (!isNaN(numValue) && cleanNumber !== '') {
      // Format with thousand separators
      const formatted = new Intl.NumberFormat('vi-VN').format(numValue);
      event.target.value = formatted;
      targetValues.value[indicatorId] = numValue;
      return;
    } else if (cleanNumber === '') {
      event.target.value = '';
      targetValues.value[indicatorId] = null;
      delete targetErrors.value[indicatorId];
      return;
    }
  }

  // For percentage, keep as decimal
  if (unit === '%') {
    const numValue = parseFloat(numericValue);
    if (!isNaN(numValue)) {
      event.target.value = numValue.toString();
      targetValues.value[indicatorId] = numValue;
    }
    return;
  }

  // Default handling for other units
  const numValue = parseFloat(numericValue);
  if (!isNaN(numValue)) {
    event.target.value = numValue.toString();
    targetValues.value[indicatorId] = numValue;
    delete targetErrors.value[indicatorId];
  } else if (inputValue.trim() === '') {
    targetValues.value[indicatorId] = null;
    delete targetErrors.value[indicatorId];
  } else {
    targetErrors.value[indicatorId] = 'Vui lòng chỉ nhập số';
  }
};

const handleTargetBlur = (event, indicatorId) => {
  const indicator = indicators.value.find(ind => ind.Id === indicatorId);
  const unit = getIndicatorUnit(indicator);
  const inputValue = event.target.value;

  if (inputValue.trim() === '') {
    targetValues.value[indicatorId] = null;
    delete targetErrors.value[indicatorId];
    return;
  }

  // Parse the final value
  let numericValue = inputValue.replace(/[^\d.,]/g, '').replace(',', '.');
  const numValue = parseFloat(numericValue);

  if (isNaN(numValue)) {
    targetErrors.value[indicatorId] = 'Giá trị không hợp lệ';
    return;
  }

  // Final validation and formatting
  if (unit === '%') {
    if (numValue > 100) {
      targetErrors.value[indicatorId] = 'Giá trị tối đa là 100%';
      event.target.value = '100';
      targetValues.value[indicatorId] = 100;
    } else {
      event.target.value = numValue.toString();
      targetValues.value[indicatorId] = numValue;
      delete targetErrors.value[indicatorId];
    }
  } else if (unit === 'Triệu VND') {
    // Remove formatting and limit to 8 digits
    let cleanNumber = inputValue.replace(/[,.\s]/g, '');
    if (cleanNumber.length > 8) {
      cleanNumber = cleanNumber.substring(0, 8);
      targetErrors.value[indicatorId] = 'Giá trị tối đa là 8 chữ số (99,999,999 triệu VND)';
    }

    const finalValue = parseFloat(cleanNumber);
    if (!isNaN(finalValue)) {
      const formatted = new Intl.NumberFormat('vi-VN').format(finalValue);
      event.target.value = formatted;
      targetValues.value[indicatorId] = finalValue;
      if (cleanNumber.length <= 8) {
        delete targetErrors.value[indicatorId];
      }
    }
  } else {
    event.target.value = numValue.toString();
    targetValues.value[indicatorId] = numValue;
    delete targetErrors.value[indicatorId];
  }
};

// Format target value based on unit type
function formatTargetValue(indicator, value) {
  if (value === null || value === undefined || value === '') return '';

  const unit = getIndicatorUnit(indicator);
  const numValue = parseFloat(value);

  if (isNaN(numValue)) return '';

  // Format based on unit type
  if (unit === 'Triệu VND') {
    return new Intl.NumberFormat('vi-VN').format(numValue);
  } else if (unit === '%') {
    return numValue.toString();
  } else {
    return numValue.toString();
  }
}

// Computed properties cho bộ lọc
// Lọc 23 bảng KPI dành cho Cán bộ
const staffKpiTables = computed(() => {
  console.log('🔍 Filtering KPI tables, total:', kpiTables.value.length)
  const filtered = kpiTables.value
    .filter(table => {
      const category = safeGet(table, 'Category')
      console.log(`Table ${safeGet(table, 'TableName')}: category = "${category}"`)
      return category === 'CANBO'
    })
    .sort((a, b) => {
      const descA = safeGet(a, 'Description') || safeGet(a, 'TableName') || ''
      const descB = safeGet(b, 'Description') || safeGet(b, 'TableName') || ''
      return descA.localeCompare(descB)
    })

  console.log('✅ Filtered staff KPI tables:', filtered.length)
  return filtered
})

// Bảng KPI đã chọn
const selectedKpiTable = computed(() => {
  if (!selectedTableId.value) return null
  return kpiTables.value.find(table => getId(table) === parseInt(selectedTableId.value))
})

// Updated branchOptions: Custom ordering theo yêu cầu Hội Sở → Nậm Hàng
const branchOptions = computed(() => {
  // Định nghĩa thứ tự theo yêu cầu: Hội Sở → Bình Lư → Phong Thổ → Sìn Hồ → Bum Tở → Than Uyên → Đoàn Kết → Tân Uyên → Nậm Hàng
  const customOrder = [
    'HoiSo',         // Hội Sở (ID=2)
    'BinhLu',        // Chi nhánh Bình Lư (ID=10)
    'PhongTho',      // Chi nhánh Phong Thổ (ID=11)
    'SinHo',         // Chi nhánh Sìn Hồ (ID=12)
    'BumTo',         // Chi nhánh Bum Tở (ID=13)
    'ThanUyen',      // Chi nhánh Than Uyên (ID=14)
    'DoanKet',       // Chi nhánh Đoàn Kết (ID=15)
    'TanUyen',       // Chi nhánh Tân Uyên (ID=16)
    'NamHang'        // Chi nhánh Nậm Hàng (ID=17)
  ];

  return units.value
    .filter(unit => {
      const type = (unit.Type || '').toUpperCase()
      return type === 'CNL1' || type === 'CNL2'
    })
    .sort((a, b) => {
      // Function để map Name thành customOrder index
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

      const indexA = getOrderIndex(a.Name || a.Name);
      const indexB = getOrderIndex(b.Name || b.Name);

      return indexA - indexB;
    })
})

const departmentOptions = computed(() => {
  if (!selectedBranchId.value) return []

  const branch = units.value.find(u => u.Id === parseInt(selectedBranchId.value))
  if (!branch) return []

  // Support both ParentUnitId and parentUnitId casing
  const children = units.value.filter(u =>
    (u.ParentUnitId === branch.Id) || (u.parentUnitId === branch.Id)
  )
  const branchType = (branch.Type || '').toUpperCase()

  const getDepartmentSortOrder = (name) => {
    const lowerName = (name || '').toLowerCase()
    if (lowerName.includes('ban giám đốc')) return 1
    if (lowerName.includes('phòng khách hàng')) return 2
    if (lowerName.includes('phòng kế toán')) return 3
    if (lowerName.includes('phòng giao dịch')) return 4
    return 999
  }

  if (branchType === 'CNL1') {
    return children.filter(u => {
      const unitType = (u.Type || '').toUpperCase()
      return unitType === 'PNVL1'
    }).sort((a, b) => getDepartmentSortOrder(a.Name) - getDepartmentSortOrder(b.Name))
  } else if (branchType === 'CNL2') {
    return children.filter(u => {
      const unitType = (u.Type || '').toUpperCase()
      return unitType === 'PNVL2' || unitType === 'PGDL2'
    }).sort((a, b) => getDepartmentSortOrder(a.Name) - getDepartmentSortOrder(b.Name))
  }

  return []
})

const filteredEmployees = computed(() => {
  let filtered = employees.value

  if (selectedBranchId.value) {
    const branchId = parseInt(selectedBranchId.value)
    filtered = filtered.filter(emp => {
      // Use casing-safe helper to get unitId (supports both UnitId and unitId)
      const empUnitId = safeGet(emp, 'UnitId')
      const empUnit = units.value.find(u => getId(u) === empUnitId)

      if (!empUnit) {
        console.log(`❌ Employee ${safeGet(emp, 'FullName')} has no matching unit (UnitId: ${empUnitId})`)
        return false
      }

      // Direct match
      if (getId(empUnit) === branchId) {
        console.log(`✅ Direct match: ${safeGet(emp, 'FullName')} in ${getName(empUnit)}`)
        return true
      }

      // Check parent hierarchy using casing-safe helpers
      let parent = empUnit
      while (parent && safeGet(parent, 'ParentUnitId')) {
        const parentId = safeGet(parent, 'ParentUnitId')
        parent = units.value.find(u => getId(u) === parentId)
        if (parent && getId(parent) === branchId) {
          console.log(`✅ Parent match: ${safeGet(emp, 'FullName')} in ${getName(empUnit)} → parent ${getName(parent)}`)
          return true
        }
      }

      return false
    })
  }

  if (selectedDepartmentId.value) {
    const deptId = parseInt(selectedDepartmentId.value)
    filtered = filtered.filter(emp => {
      // Use casing-safe helper to get unitId (supports both UnitId and unitId)
      const empUnitId = safeGet(emp, 'UnitId')
      const empUnit = units.value.find(u => getId(u) === empUnitId)

      if (!empUnit) return false

      // Direct match
      if (getId(empUnit) === deptId) return true

      // Check parent hierarchy using casing-safe helpers
      let parent = empUnit
      while (parent && safeGet(parent, 'ParentUnitId')) {
        const parentId = safeGet(parent, 'ParentUnitId')
        parent = units.value.find(u => getId(u) === parentId)
        if (parent && getId(parent) === deptId) return true
      }

      return false
    })
  }

  // Use casing-safe helper for employee name
  return filtered.filter(emp => {
    const name = safeGet(emp, 'FullName')
    return name && name.trim() !== ''
  })
})

const filteredEmployeesCount = computed(() => filteredEmployees.value.length)

const areAllEmployeesSelected = computed(() => {
  return filteredEmployees.value.length > 0 &&
         filteredEmployees.value.every(emp => selectedEmployeeIds.value.includes(emp.Id))
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
      api.get('/KhoanPeriods')
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
  console.log('📊 Loading table details for table ID:', selectedTableId.value)

  if (!selectedTableId.value) {
    console.log('❌ No table ID selected, clearing indicators')
    indicators.value = []
    return
  }

  try {
    console.log('🔄 Fetching KPI table details...')
    const response = await api.get(`/KpiAssignment/tables/${selectedTableId.value}`)

    // Use helper function to log API response
    logApiResponse(`/KpiAssignment/tables/${selectedTableId.value}`, response, 'indicators')

    // Handle both 'indicators' (lowercase) and 'Indicators' (PascalCase) from API
    const indicatorsData = response.data?.indicators || response.data?.Indicators
    if (response.data && indicatorsData) {
      // Use helper function to normalize .NET array format
      const normalizedData = normalizeNetArray(indicatorsData)
      console.log('🔄 Raw indicators data:', indicatorsData)
      console.log('🔄 Normalized data:', normalizedData)
      console.log('🔄 Normalized data length:', normalizedData.length)

      indicators.value = normalizedData
      console.log('✅ Loaded KPI indicators:', indicators.value.length)
      console.log('✅ Indicators array:', indicators.value)

      // Log first few indicators for debugging
      if (indicators.value.length > 0) {
        console.log('📋 Sample indicators:')
        indicators.value.slice(0, 3).forEach((ind, idx) => {
          console.log(`   ${idx + 1}. ${ind.indicatorName || ind.IndicatorName} (${ind.maxScore || ind.MaxScore} points, ${ind.unit || ind.Unit || 'N/A'})`)
        })
      } else {
        console.log('⚠️ Indicators array is empty after normalization')
      }
    } else {
      console.log('⚠️ API response missing indicators array')
      console.log('🔍 Response data keys:', Object.keys(response.data || {}))
      indicators.value = []
    }

    // Clear previous target values
    targetValues.value = {}
    targetErrors.value = {}

    // Clear any previous error messages
    if (errorMessage.value.includes('KPI')) {
      errorMessage.value = ''
    }

  } catch (error) {
    console.error('❌ Error loading table details:', error)
    console.error('Error details:', {
      status: error.response?.Status,
      message: error.response?.data?.message || error.message,
      url: error.config?.url
    })

    indicators.value = []
    errorMessage.value = 'Không thể tải chi tiết bảng KPI: ' + (error.response?.data?.message || error.message)
  }
}

function onTableChange() {
  console.log('📊 KPI table changed to:', selectedTableId.value)
  // Không xóa selectedEmployeeIds nữa để giữ trạng thái chọn cán bộ
  targetValues.value = {}
  targetErrors.value = {}

  // Tải chi tiết bảng KPI được chọn
  if (selectedTableId.value) {
    loadTableDetails()
  } else {
    indicators.value = []
  }

  // Log để debug
  const table = kpiTables.value.find(t => t.Id === parseInt(selectedTableId.value))
  console.log('Selected KPI table:', table)
}

function onBranchChange() {
  console.log('🏢 Branch changed to:', selectedBranchId.value)
  selectedDepartmentId.value = ''
  selectedEmployeeIds.value = []

  // Log để debug
  const branch = units.value.find(u => u.Id === parseInt(selectedBranchId.value))
  console.log('Selected branch:', branch)
  console.log('Available KPI tables:', kpiTables.value.length)
}

function onDepartmentChange() {
  console.log('🏬 Department changed to:', selectedDepartmentId.value)
  selectedEmployeeIds.value = []

  // Log để debug
  const dept = units.value.find(u => u.Id === parseInt(selectedDepartmentId.value))
  console.log('Selected department:', dept)
}

function validateEmployeeRoles() {
  console.log('👥 Selected employees:', selectedEmployeeIds.value)
  console.log('Employee count:', selectedEmployeeIds.value.length)
  console.log('Current selected table ID:', selectedTableId.value)
  console.log('Available KPI tables:', kpiTables.value.length)

  // Auto-select appropriate KPI table when employees are selected
  if (selectedEmployeeIds.value.length > 0) {
    if (!selectedTableId.value) {
      console.log('ℹ️ No table selected. User should manually select a KPI table from dropdown.')
      // Note: We no longer auto-select to let users choose their own KPI table
      // autoSelectKpiTable()
    } else {
      console.log('✅ Table already selected, ensuring indicators are loaded...')
      // Force reload table details to ensure indicators are displayed
      if (indicators.value.length === 0) {
        console.log('🔄 No indicators loaded, reloading table details...')
        loadTableDetails()
      }
    }
  } else {
    console.log('❌ No employees selected, clearing KPI data')
    // Clear KPI data when no employees selected
    indicators.value = []
    targetValues.value = {}
    targetErrors.value = {}
  }
}

// Auto-select KPI table function (DISABLED - users should manually select)
// This function was previously used to auto-match KPI tables based on employee roles
// but has been disabled to allow users full control over KPI table selection
function autoSelectKpiTable() {
  if (selectedEmployeeIds.value.length === 0) {
    console.log('❌ No employees selected, cannot auto-select table')
    return
  }

  console.log('🎯 Auto-selecting KPI table...')
  console.log('Available KPI tables:', kpiTables.value.map(t => ({ id: t.Id, name: t.tableName, category: t.category, type: t.tableType })))

  // Get first selected employee to determine role
  const firstEmployeeId = selectedEmployeeIds.value[0]
  const employee = employees.value.find(e => e.Id === firstEmployeeId)

  if (!employee) {
    console.log('❌ Employee not found:', firstEmployeeId)
    return
  }

  console.log('👤 First employee:', {
    id: employee.Id,
    name: employee.fullName,
    position: employee.positionName,
    role: employee.roleName,
    unitId: employee.unitId
  })

  // Find employee KPI tables
  const employeeTables = kpiTables.value.filter(t =>
    t.category === 'Dành cho Cán bộ' ||
    t.category?.toLowerCase().includes('cán bộ') ||
    t.category?.toLowerCase().includes('employee')
  )

  console.log('� Employee KPI tables found:', employeeTables.length)
  employeeTables.forEach(table => {
    console.log(`   📊 ${table.tableName} (ID: ${table.Id}, Type: ${table.tableType || 'N/A'})`)
  })

  // Find appropriate KPI table based on employee role
  let suitableTable = null
  const role = (employee.roleName || employee.positionName || '').toLowerCase()
  console.log('🔍 Employee role to match:', role)

  // Try to match role with KPI tables
  for (const table of employeeTables) {
    const tableName = table.tableName.toLowerCase()
    console.log(`🔍 Checking table: "${table.tableName}" against role: "${role}"`)

    // Specific role matching
    if (role.includes('trưởng phòng')) {
      if (tableName.includes('trưởng phòng')) {
        if ((role.includes('khdn') && tableName.includes('khdn')) ||
            (role.includes('khcn') && tableName.includes('khcn'))) {
          suitableTable = table
          console.log('✅ Match found: Trưởng phòng with specific department')
          break
        } else if (tableName.includes('trưởng phòng') && !tableName.includes('khdn') && !tableName.includes('khcn')) {
          suitableTable = table
          console.log('✅ Match found: General Trưởng phòng')
          break
        }
      }
    } else if (role.includes('phó phòng')) {
      if (tableName.includes('phó phòng')) {
        if ((role.includes('khdn') && tableName.includes('khdn')) ||
            (role.includes('khcn') && tableName.includes('khcn'))) {
          suitableTable = table
          console.log('✅ Match found: Phó phòng with specific department')
          break
        } else if (tableName.includes('phó phòng') && !tableName.includes('khdn') && !tableName.includes('khcn')) {
          suitableTable = table
          console.log('✅ Match found: General Phó phòng')
          break
        }
      }
    } else if (role.includes('giao dịch') || role.includes('gdv')) {
      if (tableName.includes('gdv') || tableName.includes('giao dịch')) {
        suitableTable = table
        console.log('✅ Match found: Giao dịch viên')
        break
      }
    } else if (role.includes('cán bộ') || role.includes('cb')) {
      if (tableName.includes('cbtd') || tableName.includes('cán bộ')) {
        suitableTable = table
        console.log('✅ Match found: Cán bộ')
        break
      }
    }
  }

  // Fallback to first employee table if no specific match
  if (!suitableTable && employeeTables.length > 0) {
    suitableTable = employeeTables[0]
    console.log('⚠️ No specific match, using first employee table as fallback')
  }

  if (suitableTable) {
    console.log('✅ Auto-selected KPI table:', {
      id: suitableTable.Id,
      name: suitableTable.tableName,
      category: suitableTable.category
    })
    selectedTableId.value = suitableTable.Id.toString()
    // Force load table details with a slight delay to ensure state is updated
    setTimeout(() => {
      loadTableDetails()
    }, 100)
  } else {
    console.log('❌ No suitable KPI table found')
    console.log('Available tables:', kpiTables.value.map(t => ({ id: t.Id, name: t.tableName, category: t.category })))

    // Show user message
    errorMessage.value = 'Không tìm thấy bảng KPI phù hợp. Vui lòng chọn bảng KPI thủ công.'
  }
}

function toggleAllEmployees() {
  if (areAllEmployeesSelected.value) {
    selectedEmployeeIds.value = []
  } else {
    selectedEmployeeIds.value = filteredEmployees.value.map(emp => emp.Id)
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
  const branch = units.value.find(u => u.Id === parseInt(selectedBranchId.value))
  return branch ? branch.Name : ''
}

function getDepartmentName() {
  if (!selectedDepartmentId.value) return ''
  const dept = units.value.find(u => u.Id === parseInt(selectedDepartmentId.value))
  return dept ? dept.Name : ''
}

function getUnitName(unitId) {
  const unit = units.value.find(u => u.Id === unitId)
  return unit ? unit.Name : 'N/A'
}

function getEmployeeRole(employee) {
  // Check for role name from EmployeeRoles array
  const roles = safeGet(employee, 'EmployeeRoles') || []
  if (roles.length > 0) {
    const role = roles[0]
    return safeGet(role, 'Description') || safeGet(role, 'Name') || 'Cán bộ'
  }

  // Fallback to position name or default
  return safeGet(employee, 'PositionName') || 'Cán bộ'
}

function getEmployeeShortName(empId) {
  const emp = employees.value.find(e => getId(e) === empId)
  if (!emp) return 'N/A'

  const fullName = safeGet(emp, 'FullName')
  if (!fullName) return 'N/A'

  const names = fullName.split(' ')
  if (names.length >= 2) {
    return names[names.length - 2] + ' ' + names[names.length - 1]
  }
  return fullName
}

function getKpiTableTitle() {
  const table = kpiTables.value.find(t => t.Id === parseInt(selectedTableId.value))
  return table ? (table.description || table.tableName) : 'Bảng KPI'
}

function getIndicatorUnit(indicator) {
  return safeGet(indicator, 'Unit') || 'N/A'
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
  return indicators.value.reduce((sum, ind) => sum + (safeGet(ind, 'MaxScore') || 0), 0)
}

function getTotalScore() {
  return Object.values(targetValues.value).reduce((sum, score) => sum + (parseFloat(score) || 0), 0)
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

// Watcher để tự động load KPI table khi chọn cán bộ và table
watch([selectedEmployeeIds, selectedTableId], ([newEmployeeIds, newTableId]) => {
  console.log('👀 Watcher triggered:', { employeeIds: newEmployeeIds, tableId: newTableId })

  if (newEmployeeIds.length > 0 && newTableId) {
    console.log('✅ Both employees and table selected, loading KPI details...')
    loadTableDetails()
  } else if (newEmployeeIds.length > 0 && !newTableId) {
    console.log('ℹ️ Employees selected but no table. User should manually select KPI table from dropdown.')
    // Note: We no longer auto-select to let users choose their own KPI table
    // autoSelectKpiTable()
  }
}, { immediate: false })

// Watcher để auto-load KPI khi chọn period
watch(selectedPeriodId, (newPeriodId) => {
  console.log('📅 Period changed:', newPeriodId)
  if (newPeriodId) {
    // Clear previous selections when period changes
    selectedEmployeeIds.value = []
    selectedTableId.value = ''
    indicators.value = []
    targetValues.value = {}
    targetErrors.value = {}
  }
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
