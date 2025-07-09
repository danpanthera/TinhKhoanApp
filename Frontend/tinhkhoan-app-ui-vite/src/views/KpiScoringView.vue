<template>
  <div class="kpi-scoring-container">
    <!-- 📊 Header với thống kê -->
    <div class="scoring-header">
      <h2>🎯 Hệ Thống Chấm Điểm KPI</h2>
      <div class="stats-cards">
        <div class="stat-card">
          <div class="stat-number">{{ totalKPIs }}</div>
          <div class="stat-label">Tổng KPI</div>
        </div>
        <div class="stat-card">
          <div class="stat-number">{{ scoredKPIs }}</div>
          <div class="stat-label">Đã chấm</div>
        </div>
        <div class="stat-card">
          <div class="stat-number">{{ pendingKPIs }}</div>
          <div class="stat-label">Chờ chấm</div>
        </div>
        <div class="stat-card">
          <div class="stat-number">{{ averageScore }}%</div>
          <div class="stat-label">Điểm TB</div>
        </div>
      </div>
    </div>

    <!-- 🎛️ Filter và Controls -->
    <div class="scoring-controls">
      <!-- Scoring Method Selection -->
      <div class="scoring-method-section">
        <h3>🎯 Phương thức chấm điểm</h3>
        <div class="method-selection">
          <div class="radio-group">
            <label class="radio-option">
              <input
                type="radio"
                v-model="scoringMethod"
                value="employee"
                @change="onScoringMethodChange"
              />
              <span class="radio-label">👤 Chấm theo Cán bộ</span>
            </label>
            <label class="radio-option">
              <input
                type="radio"
                v-model="scoringMethod"
                value="unit"
                @change="onScoringMethodChange"
              />
              <span class="radio-label">🏢 Chấm theo Chi nhánh</span>
            </label>
          </div>
        </div>
      </div>

      <div class="filter-section">
        <!-- Employee Selection (when employee method selected) -->
        <select
          v-if="scoringMethod === 'employee'"
          v-model="selectedEmployee"
          @change="loadEmployeeKPIs"
          class="filter-select"
        >
          <option value="">-- Chọn nhân viên --</option>
          <option v-for="emp in employees" :key="emp.id" :value="emp.id">
            {{ emp.fullName }} - {{ emp.positionName || 'Chưa có chức vụ' }}
          </option>
        </select>

        <!-- Unit Selection (when unit method selected) -->
        <select
          v-if="scoringMethod === 'unit'"
          v-model="selectedUnit"
          @change="loadUnitKPIs"
          class="filter-select"
        >
          <option value="">-- Chọn chi nhánh --</option>
          <optgroup label="Chi nhánh CNL1">
            <option
              v-for="unit in cnl1Units"
              :key="unit.id"
              :value="unit.id"
            >
              {{ unit.name }} ({{ unit.code }})
            </option>
          </optgroup>
          <optgroup label="Chi nhánh CNL2" v-if="cnl2Units.length > 0">
            <option
              v-for="unit in cnl2Units"
              :key="unit.id"
              :value="unit.id"
            >
              {{ unit.name }} ({{ unit.code }})
            </option>
          </optgroup>
        </select>

        <select v-model="selectedPeriod" @change="loadKPIData" class="filter-select">
          <option value="">-- Chọn kỳ khoán --</option>
          <option v-for="period in periods" :key="period.id" :value="period.id">
            {{ period.name }}
          </option>
        </select>

        <select v-model="filterType" @change="filterKPIs" class="filter-select">
          <option value="ALL">Tất cả KPI</option>
          <option value="QUALITATIVE">Định tính (Chấm tay)</option>
          <option value="QUANTITATIVE_RATIO">Định lượng tỷ lệ</option>
          <option value="QUANTITATIVE_ABSOLUTE">Định lượng tuyệt đối</option>
          <option value="PENDING">Chưa chấm điểm</option>
        </select>
      </div>
    </div>

    <!-- 📋 Danh sách KPI theo loại -->
    <div class="scoring-sections" v-if="(selectedEmployee || selectedUnit) && selectedPeriod">

      <!-- 📝 PHẦN 1: KPI Định Tính - Chấm Tay -->
      <div class="scoring-section" v-if="qualitativeKPIs.length > 0">
        <h3>📝 Chỉ Tiêu Định Tính - Chấm Điểm Thủ Công</h3>
        <div class="kpi-grid">
          <div v-for="kpi in qualitativeKPIs" :key="kpi.id" class="kpi-card qualitative">
            <div class="kpi-header">
              <h4>{{ kpi.kpiName }}</h4>
              <span class="kpi-badge">{{ kpi.maxScore }} điểm</span>
            </div>

            <div class="scoring-input">
              <label>Kết quả thực hiện (%):</label>
              <div class="input-group">
                <input
                  type="text"
                  :value="formatNumber(kpi.actualValue || 0)"
                  @input="(e) => handleActualValueInput(e, kpi)"
                  @blur="(e) => handleActualValueBlur(e, kpi)"
                  :class="{ 'error': kpi.error }"
                />
                <span class="unit">%</span>
              </div>
              <div v-if="kpi.error" class="error-message">{{ kpi.error }}</div>
            </div>

            <div class="score-display">
              <div class="score-item">
                <span>Chỉ tiêu:</span>
                <strong>{{ kpi.targetValue }}%</strong>
              </div>
              <div class="score-item">
                <span>Điểm đạt:</span>
                <strong class="score-value">{{ kpi.score || 0 }}/{{ kpi.maxScore }}</strong>
              </div>
              <div v-if="kpi.updatedDate" class="update-time">
                Cập nhật: {{ formatDate(kpi.updatedDate) }}
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 🧮 PHẦN 2: KPI Định Lượng Tỷ Lệ - Tính Toán -->
      <div class="scoring-section" v-if="ratioKPIs.length > 0">
        <h3>🧮 Chỉ Tiêu Định Lượng Tỷ Lệ - Tính Toán</h3>
        <div class="kpi-grid">
          <div v-for="kpi in ratioKPIs" :key="kpi.id" class="kpi-card ratio">
            <div class="kpi-header">
              <h4>{{ kpi.kpiName }}</h4>
              <span class="kpi-badge">{{ kpi.maxScore }} điểm</span>
            </div>

            <div class="ratio-inputs">
              <div class="input-row">
                <label>Tử số:</label>
                <input
                  type="text"
                  :value="formatNumber(kpi.numerator || 0)"
                  @input="(e) => handleNumeratorInput(e, kpi)"
                  @blur="(e) => handleNumeratorBlur(e, kpi)"
                />
              </div>
              <div class="input-row">
                <label>Mẫu số:</label>
                <input
                  type="text"
                  :value="formatNumber(kpi.denominator || 0)"
                  @input="(e) => handleDenominatorInput(e, kpi)"
                  @blur="(e) => handleDenominatorBlur(e, kpi)"
                />
              </div>
              <div class="calculation-result">
                <span>Kết quả:</span>
                <strong>{{ kpi.calculatedRatio || 0 }}%</strong>
              </div>
              <button
                @click="saveRatioCalculation(kpi)"
                :disabled="!kpi.numerator || !kpi.denominator"
                class="btn-calculate"
              >
                💾 Lưu Kết Quả
              </button>
            </div>

            <div class="score-display">
              <div class="score-item">
                <span>Chỉ tiêu:</span>
                <strong>{{ kpi.targetValue }}%</strong>
              </div>
              <div class="score-item">
                <span>Thực hiện:</span>
                <strong>{{ kpi.actualValue || 0 }}%</strong>
              </div>
              <div class="score-item">
                <span>Điểm đạt:</span>
                <strong class="score-value">{{ kpi.score || 0 }}/{{ kpi.maxScore }}</strong>
              </div>
              <div v-if="kpi.notes" class="calculation-notes">
                {{ kpi.notes }}
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 📊 PHẦN 3: KPI Định Lượng Tuyệt Đối - Import -->
      <div class="scoring-section" v-if="absoluteKPIs.length > 0">
        <h3>📊 Chỉ Tiêu Định Lượng Tuyệt Đối - Import Dữ Liệu</h3>

        <!-- File Upload Section -->
        <div class="import-section">
          <div class="upload-area">
            <input
              type="file"
              ref="fileInput"
              @change="handleFileUpload"
              accept=".xlsx,.csv"
              style="display: none"
            />
            <button @click="$refs.fileInput.click()" class="btn-upload">
              📂 Chọn File Excel/CSV
            </button>
            <span class="upload-info">Hỗ trợ: .xlsx, .csv</span>
          </div>

          <!-- Import Preview -->
          <div v-if="importPreview.length > 0" class="import-preview">
            <h4>🔍 Xem Trước Dữ Liệu Import</h4>
            <table>
              <thead>
                <tr>
                  <th>KPI</th>
                  <th>Chỉ tiêu</th>
                  <th>Kết quả</th>
                  <th>Điểm tính được</th>
                  <th>Trạng thái</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="row in importPreview" :key="row.id">
                  <td>{{ row.kpiName }}</td>
                  <td>{{ formatNumber(row.targetValue) }} {{ row.unit }}</td>
                  <td>{{ formatNumber(row.actualValue) }} {{ row.unit }}</td>
                  <td class="score">{{ row.calculatedScore }}/{{ row.maxScore }}</td>
                  <td>
                    <span :class="'status ' + row.status">{{ getStatusText(row.status) }}</span>
                  </td>
                </tr>
              </tbody>
            </table>
            <div class="import-actions">
              <button @click="confirmImport" class="btn-confirm">✅ Xác Nhận Import</button>
              <button @click="cancelImport" class="btn-cancel">❌ Hủy</button>
            </div>
          </div>
        </div>

        <!-- Current Absolute KPIs -->
        <div class="kpi-grid">
          <div v-for="kpi in absoluteKPIs" :key="kpi.id" class="kpi-card absolute">
            <div class="kpi-header">
              <h4>{{ kpi.kpiName }}</h4>
              <span class="kpi-badge">{{ kpi.maxScore }} điểm</span>
            </div>

            <div class="score-display">
              <div class="score-item">
                <span>Chỉ tiêu:</span>
                <strong>{{ formatNumber(kpi.targetValue) }} {{ kpi.unit }}</strong>
              </div>
              <div class="score-item">
                <span>Thực hiện:</span>
                <strong>{{ formatNumber(kpi.actualValue) || 0 }} {{ kpi.unit }}</strong>
              </div>
              <div class="score-item">
                <span>Điểm đạt:</span>
                <strong class="score-value">{{ kpi.score || 0 }}/{{ kpi.maxScore }}</strong>
              </div>
              <div class="completion-rate">
                <span>Tỷ lệ hoàn thành:</span>
                <strong>{{ getCompletionRate(kpi) }}%</strong>
              </div>
            </div>
          </div>
        </div>
      </div>

    </div>

    <!-- 📈 Tóm tắt điểm -->
    <div v-if="(selectedEmployee || selectedUnit) && selectedPeriod" class="score-summary">
      <h3>📈 Tóm Tắt Điểm Số</h3>
      <div class="summary-grid">
        <div class="summary-card">
          <h4>Định tính</h4>
          <div class="summary-score">{{ qualitativeScore }}/{{ qualitativeMaxScore }}</div>
        </div>
        <div class="summary-card">
          <h4>Định lượng tỷ lệ</h4>
          <div class="summary-score">{{ ratioScore }}/{{ ratioMaxScore }}</div>
        </div>
        <div class="summary-card">
          <h4>Định lượng tuyệt đối</h4>
          <div class="summary-score">{{ absoluteScore }}/{{ absoluteMaxScore }}</div>
        </div>
        <div class="summary-card total">
          <h4>🏆 Tổng cộng</h4>
          <div class="summary-score total-score">{{ totalScore }}/{{ totalMaxScore }}</div>
        </div>
      </div>
    </div>

    <!-- Loading và Error States -->
    <div v-if="loading" class="loading-state">⏳ Đang tải dữ liệu...</div>
    <div v-if="error" class="error-state">❌ {{ error }}</div>
  </div>
</template>

<script setup>
import { useApiService } from '@/composables/useApiService';
import { computed, onMounted, ref } from 'vue';

// 🎛️ Reactive Data
const loading = ref(false)
const error = ref('')
const employees = ref([])
const periods = ref([])
const scoringMethod = ref('employee') // 'employee' or 'unit'
const selectedEmployee = ref('')
const selectedUnit = ref('')
const selectedPeriod = ref('')
const filterType = ref('ALL')
const allKPIs = ref([])
const importPreview = ref([])

// Unit data for unit scoring
const cnl1Units = ref([])
const cnl2Units = ref([])

// 📊 Computed Properties cho các loại KPI
const qualitativeKPIs = computed(() =>
  allKPIs.value.filter(kpi => kpi.inputType === 'QUALITATIVE')
)

const ratioKPIs = computed(() =>
  allKPIs.value.filter(kpi => kpi.inputType === 'QUANTITATIVE_RATIO')
)

const absoluteKPIs = computed(() =>
  allKPIs.value.filter(kpi => kpi.inputType === 'QUANTITATIVE_ABSOLUTE')
)

// 📈 Computed Properties cho điểm số
const qualitativeScore = computed(() =>
  qualitativeKPIs.value.reduce((sum, kpi) => sum + (kpi.score || 0), 0)
)

const qualitativeMaxScore = computed(() =>
  qualitativeKPIs.value.reduce((sum, kpi) => sum + kpi.maxScore, 0)
)

const ratioScore = computed(() =>
  ratioKPIs.value.reduce((sum, kpi) => sum + (kpi.score || 0), 0)
)

const ratioMaxScore = computed(() =>
  ratioKPIs.value.reduce((sum, kpi) => sum + kpi.maxScore, 0)
)

const absoluteScore = computed(() =>
  absoluteKPIs.value.reduce((sum, kpi) => sum + (kpi.score || 0), 0)
)

const absoluteMaxScore = computed(() =>
  absoluteKPIs.value.reduce((sum, kpi) => sum + kpi.maxScore, 0)
)

const totalScore = computed(() =>
  qualitativeScore.value + ratioScore.value + absoluteScore.value
)

const totalMaxScore = computed(() =>
  qualitativeMaxScore.value + ratioMaxScore.value + absoluteMaxScore.value
)

// 📊 Stats
const totalKPIs = computed(() => allKPIs.value.length)
const scoredKPIs = computed(() => allKPIs.value.filter(kpi => kpi.score > 0).length)
const pendingKPIs = computed(() => totalKPIs.value - scoredKPIs.value)
const averageScore = computed(() => {
  if (totalMaxScore.value === 0) return 0
  return Math.round((totalScore.value / totalMaxScore.value) * 100)
})

// 🔧 API Service
const { get, post, put } = useApiService()

// 🚀 Lifecycle
onMounted(async () => {
  await loadInitialData()
})

// 📦 Methods
const loadInitialData = async () => {
  try {
    loading.value = true
    const [employeesData, periodsData] = await Promise.all([
      get('/Employees'),
      get('/KhoanPeriods')
    ])
    employees.value = employeesData
    periods.value = periodsData

    // Load units data for unit scoring
    await loadUnitsData()
  } catch (err) {
    error.value = 'Lỗi tải dữ liệu ban đầu'
    console.error(err)
  } finally {
    loading.value = false
  }
}

const loadEmployeeKPIs = async () => {
  if (!selectedEmployee.value || !selectedPeriod.value) return

  try {
    loading.value = true
    // Sử dụng API EmployeeKpiAssignment thay vì employee-kpi-targets
    const data = await get(`/EmployeeKpiAssignment/employee/${selectedEmployee.value}`)

    // Lọc theo period nếu cần
    const filteredData = data.filter(kpi => kpi.khoanPeriodId == selectedPeriod.value)

    // Phân loại KPI và thêm thông tin input type
    allKPIs.value = filteredData.map((kpi) => {
      // Tạm thời phân loại dựa trên tên KPI
      let inputType = 'QUALITATIVE' // Mặc định là định tính

      if (kpi.kpiName.includes('Tỷ lệ nợ xấu') || kpi.kpiName.includes('Phát triển khách hàng mới')) {
        inputType = 'QUANTITATIVE_RATIO' // Tỷ lệ cần tính toán
      } else if (kpi.kpiName.includes('Lợi nhuận') || kpi.kpiName.includes('Doanh số')) {
        inputType = 'QUANTITATIVE_ABSOLUTE' // Định lượng tuyệt đối
      }

      return {
        ...kpi,
        inputType: inputType,
        numerator: null,
        denominator: null,
        calculatedRatio: null,
        error: null
      }
    }) // Đóng function map()
  } catch (err) {
    error.value = 'Lỗi tải KPI của nhân viên'
    console.error(err)
  } finally {
    loading.value = false
  }
}

// 🏢 Load Unit KPIs (New function for unit scoring)
const loadUnitKPIs = async () => {
  if (!selectedUnit.value || !selectedPeriod.value) return

  try {
    loading.value = true
    // Load unit KPI assignments
    const data = await get(`/UnitKhoanAssignments`)

    // Filter by unit and period
    const filteredData = data.filter(assignment =>
      assignment.unitId == selectedUnit.value &&
      assignment.khoanPeriodId == selectedPeriod.value
    )

    // Transform unit assignments to KPI format
    const unitKPIs = []
    filteredData.forEach(assignment => {
      if (assignment.assignmentDetails) {
        assignment.assignmentDetails.forEach(detail => {
          unitKPIs.push({
            id: detail.id,
            kpiName: detail.legacyKPIName,
            targetValue: detail.targetValue,
            actualValue: detail.actualValue,
            score: detail.score,
            maxScore: 100, // Default max score
            unit: assignment.unit,
            notes: detail.note,
            inputType: 'QUALITATIVE', // Default to qualitative for units
            numerator: null,
            denominator: null,
            calculatedRatio: null,
            error: null
          })
        })
      }
    })

    allKPIs.value = unitKPIs
  } catch (err) {
    error.value = 'Lỗi tải KPI của chi nhánh'
    console.error(err)
  } finally {
    loading.value = false
  }
}

// 🔄 Load Units Data
const loadUnitsData = async () => {
  try {
    const unitsData = await get('/Units')
    cnl1Units.value = unitsData.filter(unit => unit.type === 'CNL1')
    cnl2Units.value = unitsData.filter(unit => unit.type === 'CNL2')
  } catch (err) {
    console.error('Error loading units:', err)
  }
}

// 🎯 Handle Scoring Method Change
const onScoringMethodChange = () => {
  // Reset selections when method changes
  selectedEmployee.value = ''
  selectedUnit.value = ''
  allKPIs.value = []

  // Load units data if switching to unit scoring
  if (scoringMethod.value === 'unit' && cnl1Units.value.length === 0) {
    loadUnitsData()
  }
}

// 🔄 Generic KPI Data Loader
const loadKPIData = () => {
  if (scoringMethod.value === 'employee') {
    loadEmployeeKPIs()
  } else if (scoringMethod.value === 'unit') {
    loadUnitKPIs()
  }
}

// 📝 Chấm điểm thủ công
const updateManualScore = async (kpi) => {
  if (!kpi.actualValue || kpi.actualValue < 0 || kpi.actualValue > 100) {
    kpi.error = 'Giá trị phải từ 0-100%'
    return
  }

  try {
    kpi.error = null
    // Sử dụng API PUT để cập nhật EmployeeKpiAssignment
    const result = await put(`/EmployeeKpiAssignment/${kpi.id}`, {
      targetValue: kpi.targetValue,
      actualValue: kpi.actualValue,
      notes: kpi.notes || 'Chấm điểm thủ công'
    })

    if (result) {
      kpi.actualValue = result.actualValue
      kpi.score = result.score
      kpi.updatedDate = result.updatedDate
      kpi.notes = result.notes
    }
  } catch (err) {
    kpi.error = 'Lỗi cập nhật điểm'
    console.error(err)
  }
}

// 🧮 Tính toán tỷ lệ
const calculateRatio = (kpi) => {
  if (kpi.numerator && kpi.denominator && kpi.denominator !== 0) {
    kpi.calculatedRatio = Math.round((kpi.numerator / kpi.denominator) * 100 * 100) / 100
  } else {
    kpi.calculatedRatio = null
  }
}

const saveRatioCalculation = async (kpi) => {
  try {
    // Tính toán tỷ lệ trước khi lưu
    calculateRatio(kpi)

    // Sử dụng API PUT để cập nhật với giá trị đã tính toán
    const result = await put(`/EmployeeKpiAssignment/${kpi.id}`, {
      targetValue: kpi.targetValue,
      actualValue: kpi.calculatedRatio,
      notes: `Tính từ công thức: ${kpi.numerator}/${kpi.denominator} = ${kpi.calculatedRatio}%`
    })

    if (result) {
      kpi.actualValue = result.actualValue
      kpi.score = result.score
      kpi.notes = result.notes
      kpi.updatedDate = result.updatedDate
    }
  } catch (err) {
    error.value = 'Lỗi lưu tính toán tỷ lệ'
    console.error(err)
  }
}

// 📊 Import file
const handleFileUpload = (event) => {
  const file = event.target.files[0]
  if (!file) return

  // TODO: Implement file parsing logic
  console.log('File selected:', file.name)
}

const confirmImport = async () => {
  try {
    const result = await post('/kpi-scoring/bulk-import', {
      scores: importPreview.value.map(row => ({
        targetId: row.id,
        actualValue: row.actualValue
      }))
    })

    if (result.success) {
      await loadEmployeeKPIs() // Reload để cập nhật điểm
      importPreview.value = []
    }
  } catch (err) {
    error.value = 'Lỗi import dữ liệu'
    console.error(err)
  }
}

const cancelImport = () => {
  importPreview.value = []
}

// Number input handlers for KPI scoring
const handleActualValueInput = (event, kpi) => {
  const formattedValue = handleInput(event);
  event.target.value = formattedValue;
  kpi.actualValue = parseFormattedNumber(formattedValue);
  updateManualScore(kpi);
};

const handleActualValueBlur = (event, kpi) => {
  const formattedValue = handleBlur(event);
  event.target.value = formattedValue;
  kpi.actualValue = parseFormattedNumber(formattedValue);
  updateManualScore(kpi);
};

const handleNumeratorInput = (event, kpi) => {
  const formattedValue = handleInput(event);
  event.target.value = formattedValue;
  kpi.numerator = parseFormattedNumber(formattedValue);
  calculateRatio(kpi);
};

const handleNumeratorBlur = (event, kpi) => {
  const formattedValue = handleBlur(event);
  event.target.value = formattedValue;
  kpi.numerator = parseFormattedNumber(formattedValue);
  calculateRatio(kpi);
};

const handleDenominatorInput = (event, kpi) => {
  const formattedValue = handleInput(event);
  event.target.value = formattedValue;
  kpi.denominator = parseFormattedNumber(formattedValue);
  calculateRatio(kpi);
};

const handleDenominatorBlur = (event, kpi) => {
  const formattedValue = handleBlur(event);
  event.target.value = formattedValue;
  kpi.denominator = parseFormattedNumber(formattedValue);
  calculateRatio(kpi);
};

// 🔧 Helper functions
const formatNumber = (value) => {
  if (!value && value !== 0) return ''
  return value.toLocaleString('vi-VN')
}

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleString('vi-VN')
}

const getCompletionRate = (kpi) => {
  if (!kpi.actualValue || !kpi.targetValue) return 0
  return Math.round((kpi.actualValue / kpi.targetValue) * 100)
}

const getStatusText = (status) => {
  const statusMap = {
    'valid': 'Hợp lệ',
    'invalid': 'Không hợp lệ',
    'warning': 'Cảnh báo'
  }
  return statusMap[status] || status
}
</script>

<style scoped>
/* 🎨 Styling cho scoring interface */
.kpi-scoring-container {
  padding: 20px;
  max-width: 1400px;
  margin: 0 auto;
}

.scoring-header {
  margin-bottom: 30px;
}

.stats-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 15px;
  margin-top: 15px;
}

.stat-card {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 20px;
  border-radius: 10px;
  text-align: center;
}

.stat-number {
  font-size: 2em;
  font-weight: bold;
  margin-bottom: 5px;
}

.stat-label {
  font-size: 0.9em;
  opacity: 0.9;
}

.scoring-controls {
  background: white;
  padding: 20px;
  border-radius: 10px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  margin-bottom: 30px;
}

/* 🎯 Scoring Method Selection */
.scoring-method-section {
  margin-bottom: 25px;
  padding: 20px;
  background: #f8f9fa;
  border-radius: 8px;
  border-left: 4px solid #8B0000;
}

.scoring-method-section h3 {
  color: #8B0000;
  margin-bottom: 15px;
  font-size: 1.2rem;
}

.method-selection {
  display: flex;
  justify-content: center;
}

.radio-group {
  display: flex;
  gap: 30px;
  align-items: center;
}

.radio-option {
  display: flex;
  align-items: center;
  cursor: pointer;
  padding: 12px 20px;
  border: 2px solid #dee2e6;
  border-radius: 8px;
  background: white;
  transition: all 0.3s ease;
}

.radio-option:hover {
  border-color: #8B0000;
  background: #f8f9fa;
}

.radio-option input[type="radio"] {
  margin-right: 10px;
  width: 18px;
  height: 18px;
  accent-color: #8B0000;
}

.radio-option input[type="radio"]:checked + .radio-label {
  color: #8B0000;
  font-weight: 600;
}

.radio-option:has(input[type="radio"]:checked) {
  border-color: #8B0000;
  background: #fff5f5;
}

.radio-label {
  font-size: 1rem;
  color: #495057;
  transition: all 0.3s ease;
}

/* Filter Select Updates */
.filter-select {
  padding: 10px;
  border: 1px solid #ced4da;
  border-radius: 6px;
  font-size: 1rem;
  background: white;
  margin-right: 15px;
}

.filter-select:focus {
  border-color: #8B0000;
  outline: none;
  box-shadow: 0 0 0 2px rgba(139, 0, 0, 0.1);
}

.filter-section {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 15px;
}

.filter-section select {
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 5px;
  font-size: 14px;
}

.scoring-section {
  background: white;
  margin-bottom: 30px;
  border-radius: 10px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  overflow: hidden;
}

.scoring-section h3 {
  background: #f8f9fa;
  margin: 0;
  padding: 20px;
  border-bottom: 1px solid #eee;
  font-size: 1.3em;
}

.kpi-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
  gap: 20px;
  padding: 20px;
}

.kpi-card {
  border: 1px solid #eee;
  border-radius: 10px;
  padding: 20px;
  background: #fafafa;
}

.kpi-card.qualitative {
  border-left: 4px solid #28a745;
}

.kpi-card.ratio {
  border-left: 4px solid #007bff;
}

.kpi-card.absolute {
  border-left: 4px solid #ffc107;
}

.kpi-header {
  display: flex;
  justify-content: between;
  align-items: flex-start;
  margin-bottom: 15px;
}

.kpi-header h4 {
  margin: 0;
  flex: 1;
  font-size: 1.1em;
  line-height: 1.3;
}

.kpi-badge {
  background: #6c757d;
  color: white;
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 0.8em;
  margin-left: 10px;
}

.scoring-input,
.ratio-inputs {
  margin-bottom: 15px;
}

.input-group {
  display: flex;
  align-items: center;
  margin-top: 5px;
}

.input-group input {
  flex: 1;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
  margin-right: 8px;
}

.input-group input.error {
  border-color: #dc3545;
}

.unit {
  font-weight: bold;
  color: #6c757d;
}

.error-message {
  color: #dc3545;
  font-size: 0.85em;
  margin-top: 5px;
}

.input-row {
  display: flex;
  align-items: center;
  margin-bottom: 10px;
}

.input-row label {
  width: 80px;
  font-size: 0.9em;
}

.input-row input {
  flex: 1;
  padding: 6px;
  border: 1px solid #ddd;
  border-radius: 4px;
  margin-left: 10px;
}

.calculation-result {
  text-align: center;
  padding: 10px;
  background: #e9ecef;
  border-radius: 4px;
  margin: 10px 0;
}

.btn-calculate {
  width: 100%;
  padding: 8px;
  background: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.btn-calculate:disabled {
  background: #6c757d;
  cursor: not-allowed;
}

.score-display {
  background: white;
  padding: 15px;
  border-radius: 6px;
  border: 1px solid #eee;
}

.score-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
}

.score-value {
  color: #28a745;
  font-size: 1.1em;
}

.update-time,
.calculation-notes {
  font-size: 0.8em;
  color: #6c757d;
  margin-top: 10px;
  font-style: italic;
}

.import-section {
  padding: 20px;
  background: #f8f9fa;
  margin: 20px;
  border-radius: 8px;
}

.upload-area {
  text-align: center;
  padding: 20px;
  border: 2px dashed #ccc;
  border-radius: 8px;
  margin-bottom: 20px;
}

.btn-upload {
  background: #28a745;
  color: white;
  padding: 10px 20px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  margin-right: 10px;
}

.upload-info {
  color: #6c757d;
  font-size: 0.9em;
}

.import-preview table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 15px;
}

.import-preview th,
.import-preview td {
  padding: 10px;
  border: 1px solid #ddd;
  text-align: left;
}

.import-preview th {
  background: #f8f9fa;
  font-weight: bold;
}

.import-preview .score {
  font-weight: bold;
  color: #28a745;
}

.status {
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 0.8em;
  font-weight: bold;
}

.status.valid {
  background: #d4edda;
  color: #155724;
}

.status.invalid {
  background: #f8d7da;
  color: #721c24;
}

.status.warning {
  background: #fff3cd;
  color: #856404;
}

.import-actions {
  text-align: center;
  margin-top: 20px;
}

.btn-confirm {
  background: #28a745;
  color: white;
  padding: 10px 20px;
  border: none;
  border-radius: 5px;
  margin-right: 10px;
  cursor: pointer;
}

.btn-cancel {
  background: #dc3545;
  color: white;
  padding: 10px 20px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.score-summary {
  background: white;
  padding: 30px;
  border-radius: 10px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  margin-top: 30px;
}

.summary-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  margin-top: 20px;
}

.summary-card {
  text-align: center;
  padding: 20px;
  border-radius: 10px;
  background: #f8f9fa;
  border: 1px solid #eee;
}

.summary-card.total {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.summary-card h4 {
  margin: 0 0 10px 0;
  font-size: 1em;
}

.summary-score {
  font-size: 1.8em;
  font-weight: bold;
}

.total-score {
  font-size: 2.2em;
}

.completion-rate {
  margin-top: 10px;
  padding-top: 10px;
  border-top: 1px solid #eee;
  font-size: 0.9em;
}

.loading-state,
.error-state {
  text-align: center;
  padding: 40px;
  font-size: 1.2em;
}

.error-state {
  color: #dc3545;
  background: #f8d7da;
  border-radius: 8px;
  margin: 20px 0;
}

/* 📱 Responsive Design */
@media (max-width: 768px) {
  .kpi-grid {
    grid-template-columns: 1fr;
  }

  .stats-cards {
    grid-template-columns: repeat(2, 1fr);
  }

  .filter-section {
    grid-template-columns: 1fr;
  }

  .summary-grid {
    grid-template-columns: repeat(2, 1fr);
  }

  .kpi-header {
    flex-direction: column;
    align-items: flex-start;
  }

  .kpi-badge {
    margin-left: 0;
    margin-top: 8px;
  }
}
</style>
