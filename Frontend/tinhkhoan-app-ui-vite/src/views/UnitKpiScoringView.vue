<template>
  <div class="unit-kpi-scoring-container">
    <!-- 📊 Header with Statistics -->
    <div class="scoring-header">
      <h2>🏢 Chấm Điểm KPI Chi Nhánh</h2>

      <div class="stats-cards">
        <div class="stat-card">
          <div class="stat-number">{{ totalUnits }}</div>
          <div class="stat-label">Tổng Chi nhánh</div>
        </div>
        <div class="stat-card clickable" @click="showScoredUnitsDetail">
          <div class="stat-number">{{ scoredUnits }}</div>
          <div class="stat-label">Đã chấm</div>
          <div class="stat-hint">👆 Nhấp để xem chi tiết</div>
        </div>
        <div class="stat-card clickable" @click="showPendingUnitsDetail">
          <div class="stat-number">{{ pendingUnits }}</div>
          <div class="stat-label">Chờ chấm</div>
          <div class="stat-hint">👆 Nhấp để xem chi tiết</div>
        </div>
        <div class="stat-card">
          <div class="stat-number">{{ averageScore.toFixed(1) }}</div>
          <div class="stat-label">Điểm TB</div>
        </div>
      </div>
    </div>

    <!-- 🎛️ Filters and Controls -->
    <div class="scoring-controls">
      <div class="control-group">
        <label>📅 Kỳ tính khoán:</label>
        <select v-model="selectedPeriodId" @change="loadScorings">
          <option value="">Chọn kỳ tính khoán</option>
          <option v-for="period in periods" :key="period.id" :value="period.id">
            {{ period.name }} ({{ formatDate(period.startDate) }} - {{ formatDate(period.endDate) }})
          </option>
        </select>
      </div>

      <div class="control-group">
        <label>🏢 Chi nhánh:</label>
        <select v-model="selectedUnitId" @change="loadUnitScoring">
          <option value="">Tất cả chi nhánh</option>
          <option v-for="unit in sortedUnits" :key="unit.id" :value="unit.id">
            {{ unit.name }}
          </option>
        </select>
      </div>

      <div class="action-buttons">
        <button class="btn btn-primary" @click="createNewScoring" :disabled="!selectedPeriodId">
          ➕ Tạo chấm điểm mới
        </button>
        <button class="btn btn-info" @click="loadScorings">🔄 Làm mới</button>
      </div>
    </div>

    <!-- 📋 Scoring Summary Table -->
    <div v-if="!selectedUnitId && scoringSummary.length > 0" class="scoring-summary">
      <h3>📊 Bảng tổng hợp điểm KPI</h3>
      <div class="table-responsive">
        <table class="summary-table">
          <thead>
            <tr>
              <th>STT</th>
              <th>Chi nhánh</th>
              <th>Điểm cơ sở</th>
              <th>Điểm điều chỉnh</th>
              <th>Tổng điểm</th>
              <th>Vi phạm QT</th>
              <th>Vi phạm VH</th>
              <th>Cập nhật cuối</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="(summary, index) in scoringSummary"
              :key="summary.unitId"
              :class="{ 'low-score': summary.totalScore < 70 }"
            >
              <td>{{ index + 1 }}</td>
              <td>{{ summary.unitName }}</td>
              <td class="score-cell">{{ summary.baseScore.toFixed(1) }}</td>
              <td
                class="score-cell"
                :class="{
                  negative: summary.adjustmentScore < 0,
                  positive: summary.adjustmentScore > 0,
                }"
              >
                {{ summary.adjustmentScore > 0 ? '+' : '' }}{{ summary.adjustmentScore.toFixed(1) }}
              </td>
              <td class="total-score-cell">{{ summary.totalScore.toFixed(1) }}</td>
              <td class="violation-cell">{{ summary.processViolationCount }}</td>
              <td class="violation-cell">{{ summary.cultureViolationCount }}</td>
              <td>{{ formatDateTime(summary.lastUpdated) }}</td>
              <td>
                <button class="btn btn-sm btn-primary" @click="editUnitScoring(summary.unitId)">✏️ Chi tiết</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- 📝 Detailed Unit Scoring Form -->
    <div v-if="selectedUnitId && currentScoring" class="unit-scoring-detail">
      <div class="detail-header">
        <h3>📋 Chi tiết chấm điểm: {{ getCurrentUnitName() }}</h3>
        <div class="score-display">
          <span class="base-score">Điểm cơ sở: {{ currentScoring.baseScore.toFixed(1) }}</span>
          <span
            class="adjustment-score"
            :class="{
              negative: currentScoring.adjustmentScore < 0,
              positive: currentScoring.adjustmentScore > 0,
            }"
          >
            Điều chỉnh: {{ currentScoring.adjustmentScore > 0 ? '+' : ''
            }}{{ currentScoring.adjustmentScore.toFixed(1) }}
          </span>
          <span class="total-score">Tổng: {{ currentScoring.totalScore.toFixed(1) }}</span>
        </div>
      </div>

      <!-- KPI Indicators Section -->
      <div class="kpi-indicators-section">
        <h4>📊 Chỉ tiêu định lượng</h4>
        <div class="kpi-grid">
          <div v-for="detail in currentScoring.scoringDetails" :key="detail.id" class="kpi-card">
            <div class="kpi-header">
              <h5>
                {{
                  detail.indicatorName ||
                  detail.kpiIndicator?.indicatorName ||
                  detail.kpiDefinition?.name ||
                  'Chỉ tiêu KPI'
                }}
              </h5>
              <span class="kpi-rule">{{
                detail.scoringFormula || detail.kpiDefinition?.scoringRule || 'Quy tắc tính điểm'
              }}</span>
            </div>
            <div class="kpi-values">
              <div class="value-group">
                <label>Chỉ tiêu:</label>
                <span class="target-value">{{ detail.targetValue || detail.kpiDefinition?.targetValue || 0 }}</span>
              </div>
              <div class="value-group">
                <label>Thực hiện:</label>
                <input
                  type="text"
                  :value="formatNumber(detail.actualValue || 0)"
                  @input="e => handleActualValueInput(e, detail)"
                  @blur="e => handleActualValueBlur(e, detail)"
                  class="actual-input"
                />
              </div>
              <div class="value-group">
                <label>Điểm:</label>
                <span
                  class="score-value"
                  :class="{
                    excellent: detail.score >= 8,
                    good: detail.score >= 6 && detail.score < 8,
                    average: detail.score >= 4 && detail.score < 6,
                    poor: detail.score < 4,
                  }"
                >
                  {{ detail.score.toFixed(1) }}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Compliance Violations Section -->
      <div class="compliance-section">
        <h4>⚖️ Vi phạm tuân thủ</h4>
        <div class="violation-tabs">
          <button
            class="tab-button"
            :class="{ active: activeViolationTab === 'process' }"
            @click="activeViolationTab = 'process'"
          >
            📋 Vi phạm Quy trình
          </button>
          <button
            class="tab-button"
            :class="{ active: activeViolationTab === 'culture' }"
            @click="activeViolationTab = 'culture'"
          >
            🎭 Vi phạm Văn hóa
          </button>
        </div>

        <div class="violation-content">
          <div v-if="activeViolationTab === 'process'" class="violation-form">
            <h5>📋 Vi phạm quy trình</h5>
            <div class="violation-types">
              <div class="violation-type">
                <label>Nhắc nhở (-2 điểm/lần):</label>
                <input
                  type="text"
                  :value="formatNumber(processViolations.minor || 0)"
                  @input="e => handleViolationInput(e, 'processViolations', 'minor')"
                  @blur="e => handleViolationBlur(e, 'processViolations', 'minor')"
                />
              </div>
              <div class="violation-type">
                <label>Khiển trách bằng văn bản (-4 điểm/lần):</label>
                <input
                  type="text"
                  :value="formatNumber(processViolations.written || 0)"
                  @input="e => handleViolationInput(e, 'processViolations', 'written')"
                  @blur="e => handleViolationBlur(e, 'processViolations', 'written')"
                />
              </div>
              <div class="violation-type">
                <label>Kỷ luật (0 điểm):</label>
                <input
                  type="text"
                  :value="formatNumber(processViolations.disciplinary || 0)"
                  @input="e => handleViolationInput(e, 'processViolations', 'disciplinary')"
                  @blur="e => handleViolationBlur(e, 'processViolations', 'disciplinary')"
                />
              </div>
            </div>
          </div>

          <div v-if="activeViolationTab === 'culture'" class="violation-form">
            <h5>🎭 Vi phạm văn hóa</h5>
            <div class="violation-types">
              <div class="violation-type">
                <label>Nhắc nhở (-2 điểm/lần):</label>
                <input
                  type="text"
                  :value="formatNumber(cultureViolations.minor || 0)"
                  @input="e => handleViolationInput(e, 'cultureViolations', 'minor')"
                  @blur="e => handleViolationBlur(e, 'cultureViolations', 'minor')"
                />
              </div>
              <div class="violation-type">
                <label>Khiển trách bằng văn bản (-4 điểm/lần):</label>
                <input
                  type="text"
                  :value="formatNumber(cultureViolations.written || 0)"
                  @input="e => handleViolationInput(e, 'cultureViolations', 'written')"
                  @blur="e => handleViolationBlur(e, 'cultureViolations', 'written')"
                />
              </div>
              <div class="violation-type">
                <label>Kỷ luật (0 điểm):</label>
                <input
                  type="text"
                  :value="formatNumber(cultureViolations.disciplinary || 0)"
                  @input="e => handleViolationInput(e, 'cultureViolations', 'disciplinary')"
                  @blur="e => handleViolationBlur(e, 'cultureViolations', 'disciplinary')"
                />
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Notes Section -->
      <div class="notes-section">
        <h4>📝 Ghi chú</h4>
        <textarea v-model="currentScoring.note" placeholder="Nhập ghi chú về việc chấm điểm..." rows="3"></textarea>
      </div>

      <!-- Action Buttons -->
      <div class="form-actions">
        <button class="btn btn-success" @click="saveScoring" :disabled="isSaving">
          {{ isSaving ? '💾 Đang lưu...' : '💾 Lưu chấm điểm' }}
        </button>
        <button class="btn btn-secondary" @click="cancelEdit">❌ Hủy</button>
      </div>
    </div>

    <!-- 📋 Create New Scoring Modal -->
    <div v-if="showCreateModal" class="modal-overlay" @click="closeCreateModal">
      <div class="modal-content" @click.stop>
        <h3>➕ Tạo chấm điểm mới</h3>
        <div class="form-group">
          <label>Chi nhánh:</label>
          <select v-model="newScoring.unitId" required>
            <option value="">Chọn chi nhánh</option>
            <option v-for="unit in availableUnits" :key="unit.id" :value="unit.id">
              {{ unit.name }}
            </option>
          </select>
        </div>
        <div class="modal-actions">
          <button class="btn btn-primary" @click="createScoring" :disabled="!newScoring.unitId">✅ Tạo</button>
          <button class="btn btn-secondary" @click="closeCreateModal">❌ Hủy</button>
        </div>
      </div>
    </div>

    <!-- 📊 Units Detail Modal -->
    <div v-if="showUnitsDetailModal" class="modal-overlay" @click="closeUnitsDetailModal">
      <div class="modal-content units-detail-modal" @click.stop>
        <div class="modal-header">
          <h3>{{ unitsDetailTitle }}</h3>
          <button class="close-btn" @click="closeUnitsDetailModal">✕</button>
        </div>
        <div class="modal-body">
          <div v-if="unitsDetailList.length === 0" class="empty-state">
            <p>
              {{
                unitsDetailType === 'scored'
                  ? 'Chưa có chi nhánh nào được chấm điểm.'
                  : 'Tất cả chi nhánh đã được chấm điểm.'
              }}
            </p>
          </div>
          <div v-else class="units-list">
            <div
              v-for="(unit, index) in unitsDetailList"
              :key="unit.id"
              class="unit-item"
              :class="{ scored: unitsDetailType === 'scored', pending: unitsDetailType === 'pending' }"
            >
              <div class="unit-info">
                <span class="unit-number">{{ index + 1 }}.</span>
                <span class="unit-name">{{ unit.name }}</span>
                <span v-if="unitsDetailType === 'scored'" class="unit-score">
                  Điểm: {{ unit.totalScore?.toFixed(1) || 'N/A' }}
                </span>
              </div>
              <div class="unit-actions">
                <button
                  v-if="unitsDetailType === 'scored'"
                  @click="editUnitFromDetail(unit.id)"
                  class="btn btn-sm btn-primary"
                >
                  ✏️ Xem chi tiết
                </button>
                <button
                  v-if="unitsDetailType === 'pending'"
                  @click="createScoringForUnit(unit.id)"
                  class="btn btn-sm btn-success"
                >
                  ➕ Tạo chấm điểm
                </button>
              </div>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary" @click="closeUnitsDetailModal">Đóng</button>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="loading-state">
      <div class="spinner"></div>
      <p>Đang tải dữ liệu...</p>
    </div>
  </div>
</template>

<script>
import { computed, onMounted, ref, watch } from 'vue'
import { useToast } from '../composables/useToast'
import { useNumberInput } from '../utils/numberFormat'

export default {
  name: 'UnitKpiScoringView',
  setup() {
    const toast = useToast()

    // 🔢 Initialize number input utility
    const { handleInput, handleBlur, formatNumber, parseFormattedNumber } = useNumberInput({
      maxDecimalPlaces: 2,
      allowNegative: false,
    })

    // Reactive data
    const isLoading = ref(false)
    const isSaving = ref(false)
    const periods = ref([])
    const units = ref([])
    const selectedPeriodId = ref('')
    const selectedUnitId = ref('')
    const scoringSummary = ref([])
    const currentScoring = ref(null)
    const showCreateModal = ref(false)
    const activeViolationTab = ref('process')

    // Units detail modal
    const showUnitsDetailModal = ref(false)
    const unitsDetailType = ref('') // 'scored' or 'pending'
    const unitsDetailTitle = ref('')
    const unitsDetailList = ref([])

    // New scoring form
    const newScoring = ref({
      unitId: '',
      khoanPeriodId: '',
    })

    // Violation tracking
    const processViolations = ref({
      minor: 0,
      written: 0,
      disciplinary: 0,
    })

    const cultureViolations = ref({
      minor: 0,
      written: 0,
      disciplinary: 0,
    })

    // Computed properties
    const totalUnits = computed(() => units.value.length)
    const scoredUnits = computed(() => scoringSummary.value.length)
    const pendingUnits = computed(() => totalUnits.value - scoredUnits.value)
    const averageScore = computed(() => {
      if (scoringSummary.value.length === 0) return 0
      const total = scoringSummary.value.reduce((sum, item) => sum + item.totalScore, 0)
      return total / scoringSummary.value.length
    })

    // Sắp xếp units theo thứ tự chi nhánh như các module khác
    const sortedUnits = computed(() => {
      if (!units.value || units.value.length === 0) return []

      // Sắp xếp theo thứ tự: CnLaiChau (7800), CnBinhLu (7801), CnPhongTho (7802), CnSinHo (7803), CnBumTo (7804), CnThanUyen (7805), CnDoanKet (7806), CnTanUyen (7807), CnNamHang (7808)
      const customOrder = [
        'CnLaiChau', // Chi nhánh Lai Châu (7800)
        'CnBinhLu', // Chi nhánh Bình Lư (7801)
        'CnPhongTho', // Chi nhánh Phong Thổ (7802)
        'CnSinHo', // Chi nhánh Sìn Hồ (7803)
        'CnBumTo', // Chi nhánh Bum Tở (7804)
        'CnThanUyen', // Chi nhánh Than Uyên (7805)
        'CnDoanKet', // Chi nhánh Đoàn Kết (7806)
        'CnTanUyen', // Chi nhánh Tân Uyên (7807)
        'CnNamHang', // Chi nhánh Nậm Hàng (7808)
      ]

      return units.value.sort((a, b) => {
        const codeA = (a.code || '').toUpperCase()
        const codeB = (b.code || '').toUpperCase()

        const indexA = customOrder.indexOf(codeA)
        const indexB = customOrder.indexOf(codeB)

        if (indexA !== -1 && indexB !== -1) {
          return indexA - indexB
        }
        if (indexA !== -1) return -1
        if (indexB !== -1) return 1
        return codeA.localeCompare(codeB)
      })
    })

    const availableUnits = computed(() => {
      if (!selectedPeriodId.value) return sortedUnits.value
      const scoredUnitIds = scoringSummary.value.map(s => s.unitId)
      return sortedUnits.value.filter(unit => !scoredUnitIds.includes(unit.id))
    })

    // API Base URL
    const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7297/api'

    // API Methods
    const apiCall = async (url, options = {}) => {
      try {
        console.log(`🌐 API Call: ${options.method || 'GET'} ${API_BASE_URL}${url}`)
        const token = localStorage.getItem('authToken')

        const requestConfig = {
          headers: {
            'Content-Type': 'application/json',
            Authorization: token ? `Bearer ${token}` : '',
            ...options.headers,
          },
          ...options,
        }

        console.log('📤 Request config:', requestConfig)

        const response = await fetch(`${API_BASE_URL}${url}`, requestConfig)

        console.log(`📥 Response status: ${response.status} ${response.statusText}`)

        if (!response.ok) {
          const errorText = await response.text()
          console.error('❌ API Error Response:', errorText)
          throw new Error(`HTTP error! status: ${response.status}, message: ${errorText}`)
        }

        const data = await response.json()
        console.log('✅ API Success Response:', data)
        return data
      } catch (error) {
        console.error('❌ API call error:', error)
        throw error
      }
    }

    // Load data methods
    const loadPeriods = async () => {
      try {
        console.log('🔄 UnitKpiScoringView: Loading periods...')
        const data = await apiCall('/KhoanPeriods')
        console.log('📅 Raw periods data:', data)

        // Handle .NET $values format
        let periodsData = []
        if (data && Array.isArray(data.$values)) {
          periodsData = data.$values
        } else if (Array.isArray(data)) {
          periodsData = data
        }

        periods.value = periodsData || []
        console.log('✅ Periods loaded:', periods.value.length)
      } catch (error) {
        toast.error('Lỗi khi tải danh sách kỳ tính khoán')
        console.error('❌ Error loading periods:', error)
        periods.value = []
      }
    }

    const loadUnits = async () => {
      try {
        console.log('🔄 Loading units for scoring...')
        const data = await apiCall('/Units')
        console.log('📊 Raw units data:', data)

        // Handle .NET $values format
        let unitsData = []
        if (data && Array.isArray(data.$values)) {
          unitsData = data.$values
        } else if (Array.isArray(data)) {
          unitsData = data
        }

        // Filter for CNL1 and CNL2 units (chi nhánh)
        units.value = unitsData.filter(unit => unit.type === 'CNL1' || unit.type === 'CNL2')

        console.log('✅ Units loaded for scoring:', units.value.length)
        console.log('🏢 CNL1 units:', units.value.filter(u => u.type === 'CNL1').length)
        console.log('🏢 CNL2 units:', units.value.filter(u => u.type === 'CNL2').length)
      } catch (error) {
        toast.error('Lỗi khi tải danh sách chi nhánh')
        console.error('Error loading units:', error)
        units.value = []
      }
    }

    const loadScorings = async () => {
      if (!selectedPeriodId.value) {
        console.log('⚠️ No period selected, clearing scoring summary')
        scoringSummary.value = []
        return
      }

      console.log(`🔄 Loading scorings for period: ${selectedPeriodId.value}`)
      isLoading.value = true
      try {
        const data = await apiCall(`/UnitKpiScoring/period/${selectedPeriodId.value}/summary`)

        // Handle .NET $values format
        let summaryData = []
        if (data && Array.isArray(data.$values)) {
          summaryData = data.$values
        } else if (Array.isArray(data)) {
          summaryData = data
        } else if (data) {
          summaryData = [data]
        }

        scoringSummary.value = summaryData
        console.log(`✅ Loaded ${scoringSummary.value.length} scoring summaries`)
      } catch (error) {
        console.error('❌ Error loading scorings:', error)
        toast.error('Lỗi khi tải dữ liệu chấm điểm')
        scoringSummary.value = []
      } finally {
        isLoading.value = false
      }
    }

    const loadUnitScoring = async () => {
      if (!selectedUnitId.value || !selectedPeriodId.value) {
        currentScoring.value = null
        return
      }

      isLoading.value = true
      try {
        const data = await apiCall(`/UnitKpiScoring/unit/${selectedUnitId.value}/period/${selectedPeriodId.value}`)
        currentScoring.value = data

        // Initialize violation data
        if (data.scoringCriteria && data.scoringCriteria.length > 0) {
          const criteria = data.scoringCriteria[0]
          processViolations.value = {
            minor: criteria.processMinorViolations || 0,
            written: criteria.processWrittenViolations || 0,
            disciplinary: criteria.processDisciplinaryViolations || 0,
          }
          cultureViolations.value = {
            minor: criteria.cultureMinorViolations || 0,
            written: criteria.cultureWrittenViolations || 0,
            disciplinary: criteria.cultureDisciplinaryViolations || 0,
          }
        }
      } catch (error) {
        if (error.message.includes('404')) {
          currentScoring.value = null
          toast.info('Chưa có dữ liệu chấm điểm cho chi nhánh này')
        } else {
          toast.error('Lỗi khi tải chi tiết chấm điểm')
          console.error('Error loading unit scoring:', error)
        }
      } finally {
        isLoading.value = false
      }
    }

    // Scoring calculation methods
    const calculateScore = detail => {
      if (!detail.actualValue || !detail.targetValue) {
        detail.score = 0
        return
      }

      const target = detail.targetValue
      const actual = detail.actualValue
      const scoringRule = detail.scoringFormula || detail.kpiDefinition?.scoringRule || ''

      // Implement scoring calculation based on the rule
      let score = 0
      const variance = ((actual - target) / target) * 100

      // Parse scoring rule to get the scoring formula
      if (scoringRule.includes('±10% = ±1 điểm')) {
        score = 5 + variance / 10
      } else if (scoringRule.includes('±5% = ±1.5 điểm')) {
        score = 5 + (variance / 5) * 1.5
      } else if (scoringRule.includes('±10% = ∓1 điểm')) {
        score = 5 - variance / 10 // Reverse scoring for bad indicators
      } else if (scoringRule.includes('±10% = ±1.5 điểm')) {
        score = 5 + (variance / 10) * 1.5
      } else {
        score = 5 // Default base score
      }

      // Cap the score between 0 and maxScore
      const maxScore = detail.kpiIndicator?.maxScore || detail.kpiDefinition?.maxScore || 10
      detail.score = Math.max(0, Math.min(maxScore, score))

      updateTotalScore()
    }

    const updateViolationScores = () => {
      if (!currentScoring.value) return

      let adjustmentScore = 0

      // Process violations
      adjustmentScore -= processViolations.value.minor * 2
      adjustmentScore -= processViolations.value.written * 4
      if (processViolations.value.disciplinary > 0) {
        adjustmentScore = -currentScoring.value.baseScore // Zero out the score
      }

      // Culture violations
      adjustmentScore -= cultureViolations.value.minor * 2
      adjustmentScore -= cultureViolations.value.written * 4
      if (cultureViolations.value.disciplinary > 0) {
        adjustmentScore = -currentScoring.value.baseScore // Zero out the score
      }

      currentScoring.value.adjustmentScore = adjustmentScore
      updateTotalScore()
    }

    const updateTotalScore = () => {
      if (!currentScoring.value) return

      if (currentScoring.value.scoringDetails) {
        currentScoring.value.baseScore = currentScoring.value.scoringDetails.reduce(
          (sum, detail) => sum + (detail.score || 0),
          0
        )
      }

      currentScoring.value.totalScore = currentScoring.value.baseScore + currentScoring.value.adjustmentScore
    }

    // CRUD operations
    const createNewScoring = () => {
      if (!selectedPeriodId.value) {
        toast.error('Vui lòng chọn kỳ tính khoán')
        return
      }
      newScoring.value.khoanPeriodId = selectedPeriodId.value
      showCreateModal.value = true
    }

    const createScoring = async () => {
      if (!newScoring.value.unitId) {
        toast.error('Vui lòng chọn chi nhánh')
        return
      }

      try {
        const requestData = {
          unitId: parseInt(newScoring.value.unitId),
          khoanPeriodId: parseInt(selectedPeriodId.value),
          actualValues: {},
          note: '',
        }

        await apiCall('/UnitKpiScoring', {
          method: 'POST',
          body: JSON.stringify(requestData),
        })

        toast.success('Tạo chấm điểm thành công')
        closeCreateModal()
        await loadScorings()
      } catch (error) {
        toast.error('Lỗi khi tạo chấm điểm')
        console.error('Error creating scoring:', error)
      }
    }

    const saveScoring = async () => {
      if (!currentScoring.value) return

      isSaving.value = true
      try {
        // Prepare actual values
        const actualValues = {}
        if (currentScoring.value.scoringDetails) {
          currentScoring.value.scoringDetails.forEach(detail => {
            const kpiId = detail.kpiDefinitionId || detail.kpiIndicatorId || detail.id
            actualValues[kpiId] = detail.actualValue || 0
          })
        }

        // Update scoring
        await apiCall(`/UnitKpiScoring/${currentScoring.value.id}`, {
          method: 'PUT',
          body: JSON.stringify({
            actualValues,
            note: currentScoring.value.note,
          }),
        })

        // Update compliance violations
        const violationData = {
          processViolations: [
            {
              level: 'Minor',
              count: processViolations.value.minor,
              description: 'Nhắc nhở',
              date: new Date().toISOString(),
            },
            {
              level: 'Written',
              count: processViolations.value.written,
              description: 'Khiển trách bằng văn bản',
              date: new Date().toISOString(),
            },
            {
              level: 'Disciplinary',
              count: processViolations.value.disciplinary,
              description: 'Kỷ luật',
              date: new Date().toISOString(),
            },
          ],
          cultureViolations: [
            {
              level: 'Minor',
              count: cultureViolations.value.minor,
              description: 'Nhắc nhở',
              date: new Date().toISOString(),
            },
            {
              level: 'Written',
              count: cultureViolations.value.written,
              description: 'Khiển trách bằng văn bản',
              date: new Date().toISOString(),
            },
            {
              level: 'Disciplinary',
              count: cultureViolations.value.disciplinary,
              description: 'Kỷ luật',
              date: new Date().toISOString(),
            },
          ],
        }

        await apiCall(`/UnitKpiScoring/${currentScoring.value.id}/compliance`, {
          method: 'POST',
          body: JSON.stringify(violationData),
        })

        toast.success('Lưu chấm điểm thành công')
        await loadScorings()
        await loadUnitScoring()
      } catch (error) {
        toast.error('Lỗi khi lưu chấm điểm')
        console.error('Error saving scoring:', error)
      } finally {
        isSaving.value = false
      }
    }

    const editUnitScoring = unitId => {
      selectedUnitId.value = unitId
    }

    const cancelEdit = () => {
      selectedUnitId.value = ''
      currentScoring.value = null
    }

    const closeCreateModal = () => {
      showCreateModal.value = false
      newScoring.value = {
        unitId: '',
        khoanPeriodId: '',
      }
    }

    // Number input handlers for actual values
    const handleActualValueInput = (event, detail) => {
      const formattedValue = handleInput(event)
      event.target.value = formattedValue
      detail.actualValue = parseFormattedNumber(formattedValue)
      calculateScore(detail)
    }

    const handleActualValueBlur = (event, detail) => {
      const formattedValue = handleBlur(event)
      event.target.value = formattedValue
      detail.actualValue = parseFormattedNumber(formattedValue)
      calculateScore(detail)
    }

    // Number input handlers for violations
    const handleViolationInput = (event, violationType, field) => {
      const formattedValue = handleInput(event)
      event.target.value = formattedValue
      const numericValue = parseFormattedNumber(formattedValue)

      if (violationType === 'processViolations') {
        processViolations.value[field] = numericValue
      } else if (violationType === 'cultureViolations') {
        cultureViolations.value[field] = numericValue
      }

      updateViolationScores()
    }

    const handleViolationBlur = (event, violationType, field) => {
      const formattedValue = handleBlur(event)
      event.target.value = formattedValue
      const numericValue = parseFormattedNumber(formattedValue)

      if (violationType === 'processViolations') {
        processViolations.value[field] = numericValue
      } else if (violationType === 'cultureViolations') {
        cultureViolations.value[field] = numericValue
      }

      updateViolationScores()
    }

    // Utility methods
    const getCurrentUnitName = () => {
      if (!selectedUnitId.value) return ''
      const unit = units.value.find(u => u.id == selectedUnitId.value)
      return unit ? unit.name : ''
    }

    const formatDate = dateString => {
      if (!dateString) return ''
      return new Date(dateString).toLocaleDateString('vi-VN')
    }

    const formatDateTime = dateString => {
      if (!dateString) return ''
      return new Date(dateString).toLocaleString('vi-VN')
    }

    // Units detail modal methods
    const showScoredUnitsDetail = () => {
      if (!selectedPeriodId.value) {
        toast.error('Vui lòng chọn kỳ tính khoán trước')
        return
      }

      unitsDetailType.value = 'scored'
      unitsDetailTitle.value = `📊 Chi nhánh đã chấm điểm (${scoredUnits.value})`

      // Lấy danh sách các chi nhánh đã chấm từ scoringSummary
      unitsDetailList.value = scoringSummary.value
        .map(summary => {
          const unit = sortedUnits.value.find(u => u.id === summary.unitId)
          return {
            id: summary.unitId,
            name: unit ? unit.name : `Chi nhánh ${summary.unitId}`,
            totalScore: summary.totalScore,
            baseScore: summary.baseScore,
            adjustmentScore: summary.adjustmentScore,
          }
        })
        .sort((a, b) => {
          // Sắp xếp theo thứ tự của sortedUnits
          const unitA = sortedUnits.value.find(u => u.id === a.id)
          const unitB = sortedUnits.value.find(u => u.id === b.id)
          if (!unitA || !unitB) return 0

          const indexA = sortedUnits.value.indexOf(unitA)
          const indexB = sortedUnits.value.indexOf(unitB)
          return indexA - indexB
        })

      showUnitsDetailModal.value = true
    }

    const showPendingUnitsDetail = () => {
      if (!selectedPeriodId.value) {
        toast.error('Vui lòng chọn kỳ tính khoán trước')
        return
      }

      unitsDetailType.value = 'pending'
      unitsDetailTitle.value = `⏳ Chi nhánh chờ chấm (${pendingUnits.value})`

      // Lấy danh sách các chi nhánh chưa chấm
      const scoredUnitIds = scoringSummary.value.map(s => s.unitId)
      unitsDetailList.value = sortedUnits.value
        .filter(unit => !scoredUnitIds.includes(unit.id))
        .map(unit => ({
          id: unit.id,
          name: unit.name,
          code: unit.code,
          type: unit.type,
        }))

      showUnitsDetailModal.value = true
    }

    const closeUnitsDetailModal = () => {
      showUnitsDetailModal.value = false
      unitsDetailType.value = ''
      unitsDetailTitle.value = ''
      unitsDetailList.value = []
    }

    const editUnitFromDetail = unitId => {
      closeUnitsDetailModal()
      selectedUnitId.value = unitId
    }

    const createScoringForUnit = async unitId => {
      if (!selectedPeriodId.value) {
        toast.error('Vui lòng chọn kỳ tính khoán trước')
        return
      }

      try {
        const requestData = {
          unitId: parseInt(unitId),
          khoanPeriodId: parseInt(selectedPeriodId.value),
          actualValues: {},
          note: 'Tạo tự động từ danh sách chờ chấm',
        }

        await apiCall('/UnitKpiScoring', {
          method: 'POST',
          body: JSON.stringify(requestData),
        })

        toast.success('Tạo chấm điểm thành công')
        closeUnitsDetailModal()
        await loadScorings()

        // Chuyển sang chế độ chỉnh sửa cho chi nhánh vừa tạo
        selectedUnitId.value = unitId
      } catch (error) {
        toast.error('Lỗi khi tạo chấm điểm')
        console.error('Error creating scoring:', error)
      }
    }

    // Watchers
    watch(selectedPeriodId, () => {
      selectedUnitId.value = ''
      currentScoring.value = null
      loadScorings()
    })

    watch(selectedUnitId, () => {
      loadUnitScoring()
    })

    // Lifecycle
    onMounted(async () => {
      try {
        console.log('🔄 UnitKpiScoringView: Starting to load initial data...')

        await Promise.all([loadPeriods(), loadUnits()])

        console.log('✅ UnitKpiScoringView: All initial data loaded successfully')
        console.log(`📊 Loaded ${periods.value.length} periods and ${units.value.length} units`)
      } catch (error) {
        console.error('❌ UnitKpiScoringView: Error in onMounted:', error)
        toast.error('Lỗi khi tải dữ liệu ban đầu. Vui lòng tải lại trang.')
      }
    })

    return {
      // Data
      isLoading,
      isSaving,
      periods,
      units,
      selectedPeriodId,
      selectedUnitId,
      scoringSummary,
      currentScoring,
      showCreateModal,
      activeViolationTab,
      newScoring,
      processViolations,
      cultureViolations,
      showUnitsDetailModal,
      unitsDetailType,
      unitsDetailTitle,
      unitsDetailList,

      // Computed
      totalUnits,
      scoredUnits,
      pendingUnits,
      averageScore,
      sortedUnits,
      availableUnits,

      // Constants
      API_BASE_URL,

      // Methods
      loadScorings,
      createNewScoring,
      createScoring,
      saveScoring,
      editUnitScoring,
      cancelEdit,
      closeCreateModal,
      calculateScore,
      updateViolationScores,
      getCurrentUnitName,
      formatDate,
      formatDateTime,
      showScoredUnitsDetail,
      showPendingUnitsDetail,
      closeUnitsDetailModal,
      editUnitFromDetail,
      createScoringForUnit,
      handleInput,
      handleBlur,
      formatNumber,
      parseFormattedNumber,
      handleActualValueInput,
      handleActualValueBlur,
      handleViolationInput,
      handleViolationBlur,
    }
  },
}
</script>

<style scoped>
.unit-kpi-scoring-container {
  padding: 20px;
  max-width: 1400px;
  margin: 0 auto;
}

/* Header Styles */
.scoring-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 30px;
  border-radius: 15px;
  margin-bottom: 30px;
}

.scoring-header h2 {
  margin: 0 0 20px 0;
  font-size: 2rem;
  font-weight: 600;
}

.stats-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
}

.stat-card {
  background: rgba(255, 255, 255, 0.1);
  padding: 20px;
  border-radius: 10px;
  text-align: center;
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
}

.stat-card.clickable {
  cursor: pointer;
  border: 2px solid transparent;
}

.stat-card.clickable:hover {
  background: rgba(255, 255, 255, 0.2);
  border-color: rgba(255, 255, 255, 0.3);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.stat-number {
  font-size: 2.5rem;
  font-weight: bold;
  margin-bottom: 5px;
}

.stat-label {
  font-size: 0.9rem;
  opacity: 0.9;
  margin-bottom: 5px;
}

.stat-hint {
  font-size: 0.7rem;
  opacity: 0.7;
  font-style: italic;
}

/* Controls Styles */
.scoring-controls {
  background: #f8fafc;
  padding: 25px;
  border-radius: 10px;
  margin-bottom: 30px;
  display: flex;
  gap: 20px;
  align-items: center;
  flex-wrap: wrap;
}

.control-group {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.control-group label {
  font-weight: 600;
  color: #374151;
}

.control-group select {
  padding: 8px 12px;
  border: 2px solid #e5e7eb;
  border-radius: 6px;
  min-width: 200px;
}

.action-buttons {
  display: flex;
  gap: 10px;
  margin-left: auto;
}

/* Summary Table Styles */
.scoring-summary {
  margin-bottom: 30px;
}

.table-responsive {
  overflow-x: auto;
  background: white;
  border-radius: 10px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
}

.summary-table {
  width: 100%;
  border-collapse: collapse;
}

.summary-table th,
.summary-table td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #e5e7eb;
}

.summary-table th {
  background-color: #f9fafb;
  font-weight: 600;
  color: #374151;
}

.summary-table tr.low-score {
  background-color: #fef2f2;
}

.score-cell {
  font-weight: 600;
  text-align: center;
}

.score-cell.negative {
  color: #dc2626;
}

.score-cell.positive {
  color: #059669;
}

.total-score-cell {
  font-weight: bold;
  font-size: 1.1rem;
  text-align: center;
  color: #1f2937;
}

.violation-cell {
  text-align: center;
  color: #dc2626;
  font-weight: 600;
}

/* Detail Form Styles */
.unit-scoring-detail {
  background: white;
  border-radius: 10px;
  padding: 30px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
}

.detail-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
  padding-bottom: 20px;
  border-bottom: 2px solid #e5e7eb;
}

.score-display {
  display: flex;
  gap: 20px;
  align-items: center;
}

.base-score,
.adjustment-score,
.total-score {
  padding: 8px 16px;
  border-radius: 6px;
  font-weight: 600;
}

.base-score {
  background-color: #dbeafe;
  color: #1e40af;
}

.adjustment-score {
  background-color: #fef3c7;
  color: #d97706;
}

.adjustment-score.negative {
  background-color: #fecaca;
  color: #dc2626;
}

.adjustment-score.positive {
  background-color: #dcfce7;
  color: #059669;
}

.total-score {
  background-color: #f3e8ff;
  color: #7c3aed;
  font-size: 1.1rem;
}

/* KPI Grid Styles */
.kpi-indicators-section {
  margin-bottom: 30px;
}

.kpi-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 20px;
  margin-top: 20px;
}

.kpi-card {
  background: #f8fafc;
  border-radius: 10px;
  padding: 20px;
  border: 2px solid #e5e7eb;
}

.kpi-header h5 {
  margin: 0 0 5px 0;
  color: #1f2937;
}

.kpi-rule {
  font-size: 0.8rem;
  color: #6b7280;
}

.kpi-values {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 15px;
  margin-top: 15px;
}

.value-group {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.value-group label {
  font-size: 0.8rem;
  font-weight: 600;
  color: #6b7280;
}

.target-value,
.score-value {
  font-weight: 600;
  padding: 5px;
  text-align: center;
  border-radius: 4px;
}

.target-value {
  background-color: #e0e7ff;
  color: #3730a3;
}

.actual-input {
  padding: 8px;
  border: 2px solid #d1d5db;
  border-radius: 4px;
  text-align: center;
}

.score-value {
  font-size: 1.1rem;
}

.score-value.excellent {
  background-color: #dcfce7;
  color: #059669;
}

.score-value.good {
  background-color: #dbeafe;
  color: #2563eb;
}

.score-value.average {
  background-color: #fef3c7;
  color: #d97706;
}

.score-value.poor {
  background-color: #fecaca;
  color: #dc2626;
}

/* Compliance Section Styles */
.compliance-section {
  margin-bottom: 30px;
}

.violation-tabs {
  display: flex;
  gap: 5px;
  margin-bottom: 20px;
}

.tab-button {
  padding: 12px 20px;
  border: none;
  background-color: #f3f4f6;
  border-radius: 8px 8px 0 0;
  cursor: pointer;
  font-weight: 600;
  color: #6b7280;
  transition: all 0.2s;
}

.tab-button.active {
  background-color: #3b82f6;
  color: white;
}

.violation-content {
  background: #f8fafc;
  border-radius: 0 10px 10px 10px;
  padding: 20px;
}

.violation-types {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
}

.violation-type {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px;
  background: white;
  border-radius: 6px;
}

.violation-type label {
  font-weight: 600;
  color: #374151;
}

.violation-type input {
  width: 80px;
  padding: 5px;
  border: 2px solid #d1d5db;
  border-radius: 4px;
  text-align: center;
}

/* Notes Section */
.notes-section {
  margin-bottom: 30px;
}

.notes-section textarea {
  width: 100%;
  padding: 12px;
  border: 2px solid #d1d5db;
  border-radius: 6px;
  resize: vertical;
  font-family: inherit;
}

/* Form Actions */
.form-actions {
  display: flex;
  gap: 15px;
  justify-content: flex-end;
}

/* Button Styles */
.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 6px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  gap: 5px;
}

.btn-primary {
  background-color: #3b82f6;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #2563eb;
}

.btn-success {
  background-color: #059669;
  color: white;
}

.btn-success:hover:not(:disabled) {
  background-color: #047857;
}

.btn-secondary {
  background-color: #6b7280;
  color: white;
}

.btn-secondary:hover:not(:disabled) {
  background-color: #4b5563;
}

.btn-info {
  background-color: #0891b2;
  color: white;
}

.btn-info:hover:not(:disabled) {
  background-color: #0e7490;
}

.btn-sm {
  padding: 6px 12px;
  font-size: 0.875rem;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* Modal Styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 10px;
  padding: 30px;
  min-width: 400px;
  max-width: 90vw;
}

.modal-content h3 {
  margin: 0 0 20px 0;
  color: #1f2937;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: 600;
  color: #374151;
}

.form-group select {
  width: 100%;
  padding: 10px;
  border: 2px solid #d1d5db;
  border-radius: 6px;
}

.modal-actions {
  display: flex;
  gap: 10px;
  justify-content: flex-end;
}

/* Loading State */
.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 40px;
  color: #6b7280;
}

.spinner {
  border: 4px solid #f3f4f6;
  border-top: 4px solid #3b82f6;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
  margin-bottom: 20px;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

/* Units Detail Modal Styles */
.units-detail-modal {
  min-width: 600px;
  max-width: 800px;
  max-height: 80vh;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 25px;
  border-bottom: 1px solid #e5e7eb;
  background: #f8fafc;
  border-radius: 10px 10px 0 0;
}

.modal-header h3 {
  margin: 0;
  color: #374151;
  font-size: 1.25rem;
  font-weight: 600;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: #6b7280;
  cursor: pointer;
  padding: 5px;
  line-height: 1;
  border-radius: 4px;
  transition: all 0.2s ease;
}

.close-btn:hover {
  background: #e5e7eb;
  color: #374151;
}

.modal-body {
  padding: 25px;
  max-height: 50vh;
  overflow-y: auto;
}

.units-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.unit-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 15px 20px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  background: #ffffff;
  transition: all 0.2s ease;
}

.unit-item:hover {
  border-color: #d1d5db;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.unit-item.scored {
  border-left: 4px solid #10b981;
  background: #f0fdf4;
}

.unit-item.pending {
  border-left: 4px solid #f59e0b;
  background: #fffbeb;
}

.unit-info {
  display: flex;
  align-items: center;
  gap: 12px;
  flex: 1;
}

.unit-number {
  font-weight: 600;
  color: #6b7280;
  min-width: 30px;
}

.unit-name {
  font-weight: 500;
  color: #374151;
  flex: 1;
}

.unit-score {
  font-weight: 600;
  color: #059669;
  background: #dcfce7;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 0.875rem;
}

.unit-actions {
  display: flex;
  gap: 8px;
}

.modal-footer {
  padding: 15px 25px;
  border-top: 1px solid #e5e7eb;
  background: #f8fafc;
  border-radius: 0 0 10px 10px;
  display: flex;
  justify-content: flex-end;
}

.empty-state {
  text-align: center;
  padding: 40px 20px;
  color: #6b7280;
}

.empty-state p {
  margin: 0;
  font-size: 1rem;
}

/* Responsive Design */
@media (max-width: 768px) {
  .unit-kpi-scoring-container {
    padding: 15px;
  }

  .scoring-controls {
    flex-direction: column;
    align-items: stretch;
  }

  .action-buttons {
    margin-left: 0;
  }

  .detail-header {
    flex-direction: column;
    gap: 15px;
    align-items: stretch;
  }

  .score-display {
    justify-content: center;
    flex-wrap: wrap;
  }

  .kpi-grid {
    grid-template-columns: 1fr;
  }

  .kpi-values {
    grid-template-columns: 1fr;
  }

  .violation-types {
    grid-template-columns: 1fr;
  }

  .modal-content {
    min-width: auto;
    margin: 20px;
  }

  .units-detail-modal {
    min-width: auto;
    max-width: 95vw;
    margin: 10px;
  }

  .modal-header {
    padding: 15px 20px;
  }

  .modal-body {
    padding: 20px;
  }

  .unit-item {
    flex-direction: column;
    align-items: stretch;
    gap: 10px;
  }

  .unit-info {
    justify-content: space-between;
  }

  .unit-actions {
    justify-content: center;
  }
}
</style>
