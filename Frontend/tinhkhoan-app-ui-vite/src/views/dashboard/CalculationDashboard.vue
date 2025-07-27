<template>
  <div class="calculation-dashboard agribank-theme">
    <!-- Header với thương hiệu Agribank -->
    <div class="page-header agribank-header">
      <div class="agribank-brand">
        <div class="brand-text">
          <h1 class="agribank-title">
            <i class="mdi mdi-database-sync"></i>
            Dashboard Cập nhật Tình hình Thực hiện
          </h1>
          <p class="agribank-subtitle">
            <i class="mdi mdi-information-outline"></i>
            Hệ thống tính toán và cập nhật các chỉ tiêu kinh doanh theo từng chi nhánh
          </p>
        </div>
      </div>

      <div class="header-controls agribank-controls">
        <!-- Time filters với Agribank styling -->
        <div class="filter-group agribank-filter">
          <label for="year-select" class="agribank-label">Năm báo cáo:</label>
          <select
            id="year-select"
            v-model="selectedYear"
            @change="loadData"
            @click="console.log('📅 Year dropdown clicked')"
            class="agribank-select"
            autocomplete="off"
            aria-label="Chọn năm">
            <option value="">Chọn năm</option>
            <option v-for="year in yearOptions" :key="year" :value="year">
              {{ year }}
            </option>
          </select>
        </div>

        <div class="filter-group agribank-filter">
          <label for="period-type-select" class="agribank-label">Loại kỳ báo cáo:</label>
          <select
            id="period-type-select"
            v-model="periodType"
            @change="onPeriodTypeChange"
            @click="console.log('📆 Period type dropdown clicked')"
            class="agribank-select"
            autocomplete="off"
            aria-label="Chọn loại kỳ">
            <option value="">Chọn loại kỳ</option>
            <option v-for="period in periodTypeOptions" :key="period.value" :value="period.value">
              {{ period.label }}
            </option>
          </select>
        </div>

        <div class="filter-group agribank-filter">
          <label for="period-select" class="agribank-label">Kỳ báo cáo:</label>
          <select
            id="period-select"
            v-model="selectedPeriod"
            @change="loadData"
            @click="console.log('📊 Period dropdown clicked')"
            class="agribank-select"
            autocomplete="off"
            aria-label="Chọn kỳ">
            <option value="">Chọn kỳ</option>
            <option v-for="period in periodOptions" :key="period" :value="period">
              {{ period }}
            </option>
          </select>
        </div>
      </div>

      <!-- Action Buttons Section -->
      <div class="action-buttons-section">
        <div class="action-buttons-grid">
          <button
            class="action-btn btn-calculate"
            @click="calculateSixIndicators"
            title="Tính toán 6 chỉ tiêu chính">
            <i class="mdi mdi-calculator"></i>
            <span>Tính 6 chỉ tiêu</span>
          </button>

          <button
            class="action-btn btn-capital"
            @click="navigateToCapital"
            title="Nguồn vốn">
            <i class="mdi mdi-bank-transfer"></i>
            <span>Nguồn vốn</span>
          </button>

          <button
            class="action-btn btn-debt"
            @click="navigateToDebt"
            title="Dư nợ">
            <i class="mdi mdi-account-cash"></i>
            <span>Dư nợ</span>
          </button>

          <button
            class="action-btn btn-bad-debt"
            @click="navigateToBadDebt"
            title="Nợ xấu">
            <i class="mdi mdi-alert-circle"></i>
            <span>Nợ xấu</span>
          </button>

          <button
            class="action-btn btn-service"
            @click="navigateToService"
            title="Thu dịch vụ">
            <i class="mdi mdi-receipt"></i>
            <span>Thu dịch vụ</span>
          </button>

          <button
            class="action-btn btn-xlrr"
            @click="navigateToXLRR"
            title="Thu XLRR">
            <i class="mdi mdi-cash-multiple"></i>
            <span>Thu XLRR</span>
          </button>

          <button
            class="action-btn btn-finance"
            @click="navigateToFinance"
            title="Tài chính">
            <i class="mdi mdi-chart-line"></i>
            <span>Tài Chính</span>
          </button>
        </div>
      </div>
    </div>

    <!-- Matrix Overview Section với Agribank branding -->
    <div class="overview-section agribank-section">
      <div class="section-header agribank-section-header">
        <div class="header-with-logo">
          <div class="section-icon">
            <i class="mdi mdi-view-grid agribank-icon"></i>
          </div>
          <div class="section-title-area">
            <h2 class="agribank-section-title">Tổng quan 6 chỉ tiêu chính</h2>
            <p class="agribank-section-subtitle">
              Ma trận tình hình cập nhật các chỉ tiêu theo từng chi nhánh
            </p>
            <div class="status-legend">
              <div class="legend-item legend-success">
                <i class="mdi mdi-check-circle"></i>
                <span>Đã cập nhật</span>
              </div>
              <div class="legend-item legend-error">
                <i class="mdi mdi-close-circle"></i>
                <span>Chưa cập nhật</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Agribank Matrix Table -->
      <div class="agribank-matrix-container">
        <table class="agribank-matrix-table">
          <thead>
            <tr class="agribank-header-row">
              <th class="agribank-unit-header">
                <div class="header-content">
                  <i class="mdi mdi-bank"></i>
                  <span>Chi nhánh / Đơn vị</span>
                </div>
              </th>
              <th
                v-for="indicator in sixMainIndicators"
                :key="indicator.code"
                class="agribank-indicator-header"
              >
                <div class="agribank-indicator-content">
                  <div class="indicator-icon-wrapper">
                    <i :class="indicator.icon" class="indicator-icon"></i>
                  </div>
                  <div class="indicator-text">
                    <div class="indicator-name">{{ indicator.name }}</div>
                    <div class="indicator-unit">{{ indicator.unit }}</div>
                  </div>
                </div>
              </th>
              <th class="agribank-summary-header">
                <div class="header-content">
                  <i class="mdi mdi-chart-pie"></i>
                  <span>Tổng kết</span>
                </div>
              </th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="unit in branchUnits"
              :key="unit.id"
              class="agribank-unit-row"
            >
              <td class="agribank-unit-name-cell">
                <div class="unit-info">
                  <div class="unit-icon">
                    <i :class="getUnitIcon(unit.type)"></i>
                  </div>
                  <div class="unit-name">{{ unit.name }}</div>
                </div>
              </td>
              <td
                v-for="indicator in sixMainIndicators"
                :key="`${unit.id}-${indicator.code}`"
                :class="[
                  'agribank-status-cell',
                  getStatusClass(unit.id, indicator.code)
                ]"
                @click="navigateToDetail(unit.id, indicator.code)"
              >
                <div class="agribank-status-indicator">
                  <div v-if="getIndicatorValue(unit.id, indicator.code) !== null" class="status-success-wrapper">
                    <div class="indicator-value">{{ formatIndicatorValue(getIndicatorValue(unit.id, indicator.code), indicator.code) }}</div>
                    <div class="value-unit">{{ getValueUnit(indicator.code) }}</div>
                  </div>
                  <div v-else class="status-pending-wrapper">
                    <i class="mdi mdi-clock-outline agribank-pending-icon"></i>
                    <span class="status-text">Chờ cập nhật</span>
                  </div>
                </div>
              </td>
              <td class="agribank-unit-summary-cell">
                <div class="agribank-unit-progress">
                  <div class="progress-circle">
                    <div class="circle-progress">
                      <div class="progress-number">{{ getCompletedCount(unit.id) }}</div>
                      <div class="progress-total">/{{ sixMainIndicators.length }}</div>
                    </div>
                  </div>
                  <div class="progress-status">
                    <span :class="['status-label', getProgressStatusClass(unit.id)]">
                      {{ getProgressStatus(unit.id) }}
                    </span>
                  </div>
                </div>
              </td>
            </tr>
          </tbody>
          <tfoot>
            <tr class="agribank-summary-row">
              <td class="agribank-summary-label">
                <div class="summary-header-content">
                  <i class="mdi mdi-sigma"></i>
                  <span>Tổng cộng</span>
                </div>
              </td>
              <td
                v-for="indicator in sixMainIndicators"
                :key="`summary-${indicator.code}`"
                class="agribank-indicator-summary-cell"
              >
                <div class="agribank-indicator-summary">
                  <div class="summary-numbers">
                    <span class="completed-units">{{ getIndicatorCompletedCount(indicator.code) }}</span>
                    <span class="separator">/</span>
                    <span class="total-units">{{ branchUnits.length }}</span>
                  </div>
                  <div class="summary-percentage">
                    {{ getIndicatorCompletionPercentage(indicator.code) }}%
                  </div>
                </div>
              </td>
              <td class="agribank-total-summary-cell">
                <div class="agribank-total-progress">
                  <div class="total-percentage-large">{{ getTotalCompletionPercentage() }}%</div>
                  <div class="total-label">Hoàn thành chung</div>
                </div>
              </td>
            </tr>
          </tfoot>
        </table>
      </div>
    </div>

    <!-- Loading Overlay -->
    <LoadingOverlay v-if="isLoading" />

    <!-- Additional Debug Section (if needed) -->
    <div v-if="showDebug" class="debug-section">
      <h3>Debug Information</h3>
      <pre>{{ debugInfo }}</pre>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import LoadingOverlay from '../../components/dashboard/LoadingOverlay.vue';

const router = useRouter();

// ===== REACTIVE DATA =====
const isLoading = ref(false);
const showDebug = ref(false);

// Time-related filters
const selectedYear = ref('');
const periodType = ref('');
const selectedPeriod = ref('');

// Options for dropdowns
const yearOptions = ref(['2024', '2025']);
const periodTypeOptions = ref([
  { value: 'monthly', label: 'Tháng' },
  { value: 'quarterly', label: 'Quý' },
  { value: 'yearly', label: 'Năm' }
]);

// Dữ liệu thực tế 15 đơn vị theo yêu cầu
const branchUnits = ref([
  // 1 CNL1
  { id: 1, name: 'CN Lai Châu', code: 'CNL1', type: 'cnl1', unitCode: 'ALL' },

  // 9 CNL2
  { id: 2, name: 'Hội Sở', code: 'CNL2_HS', type: 'cnl2', unitCode: '7800' },
  { id: 3, name: 'Bình Lư', code: 'CNL2_BL', type: 'cnl2', unitCode: '7801' },
  { id: 4, name: 'Phong Thổ', code: 'CNL2_PT', type: 'cnl2', unitCode: '7802' },
  { id: 5, name: 'Sìn Hồ', code: 'CNL2_SH', type: 'cnl2', unitCode: '7803' },
  { id: 6, name: 'Bum Tở', code: 'CNL2_BT', type: 'cnl2', unitCode: '7804' },
  { id: 7, name: 'Than Uyên', code: 'CNL2_THU', type: 'cnl2', unitCode: '7805' },
  { id: 8, name: 'Đoàn Kết', code: 'CNL2_DK', type: 'cnl2', unitCode: '7806' },
  { id: 9, name: 'Tân Uyên', code: 'CNL2_TU', type: 'cnl2', unitCode: '7807' },
  { id: 10, name: 'Nậm Hàng', code: 'CNL2_NH', type: 'cnl2', unitCode: '7808' },

  // 5 PGD CNL2
  { id: 11, name: 'PGD Số 1', code: 'PGD_01', type: 'pgd', unitCode: '7806' },
  { id: 12, name: 'PGD Số 2', code: 'PGD_02', type: 'pgd', unitCode: '7806' },
  { id: 13, name: 'PGD Số 3', code: 'PGD_03', type: 'pgd', unitCode: '7807' },
  { id: 14, name: 'PGD Số 5', code: 'PGD_05', type: 'pgd', unitCode: '7802' },
  { id: 15, name: 'PGD Số 6', code: 'PGD_06', type: 'pgd', unitCode: '7805' }
]);

const sixMainIndicators = ref([
  {
    code: 'RR01',
    name: 'Nguồn vốn',
    unit: 'Triệu VND',
    icon: 'mdi-bank-transfer'
  },
  {
    code: 'DP01',
    name: 'Dư nợ',
    unit: 'Triệu VND',
    icon: 'mdi-account-cash'
  },
  {
    code: 'GL01',
    name: 'Nợ xấu',
    unit: '% (#.## %)',
    icon: 'mdi-alert-circle'
  },
  {
    code: 'EI01',
    name: 'Thu nợ XLRR',
    unit: 'Triệu VND',
    icon: 'mdi-cash-multiple'
  },
  {
    code: 'GL41',
    name: 'Thu dịch vụ',
    unit: 'Triệu VND',
    icon: 'mdi-receipt'
  },
  {
    code: 'LN03',
    name: 'Tài chính',
    unit: 'Triệu VND',
    icon: 'mdi-chart-line-variant'
  }
]);

// Mock completion status for demo
const completionMatrix = ref({});

// ===== COMPUTED PROPERTIES =====
const periodOptions = computed(() => {
  const type = periodType.value;
  if (type === 'monthly') {
    return Array.from({length: 12}, (_, i) => `Tháng ${i + 1}`);
  } else if (type === 'quarterly') {
    return ['Quý 1', 'Quý 2', 'Quý 3', 'Quý 4'];
  } else if (type === 'yearly') {
    return ['Cả năm'];
  }
  return [];
});

const debugInfo = computed(() => ({
  selectedYear: selectedYear.value,
  periodType: periodType.value,
  selectedPeriod: selectedPeriod.value,
  totalUnits: branchUnits.value.length,
  totalIndicators: sixMainIndicators.value.length,
  completionRate: getTotalCompletionPercentage()
}));

// ===== METHODS =====

// Load completion data từ backend API
const loadCompletionData = async () => {
  try {
    console.log('🔄 Đang tải dữ liệu completion từ backend...');

    // Gọi API để lấy dữ liệu completion cho 15 đơn vị
    const response = await fetch(`http://localhost:5055/api/kpi/completion-status?year=${selectedYear.value}&period=${selectedPeriod.value}`);

    if (response.ok) {
      const data = await response.json();
      completionMatrix.value = data;
      console.log('✅ Đã load completion data từ backend:', data);
    } else {
      console.warn('⚠️ Backend API chưa sẵn sàng, sử dụng dữ liệu mặc định');
      // Khởi tạo completion matrix rỗng cho 15 đơn vị thật
      initializeEmptyCompletionMatrix();
    }
  } catch (error) {
    console.error('❌ Lỗi khi gọi API completion:', error);
    // Fallback: khởi tạo completion matrix rỗng
    initializeEmptyCompletionMatrix();
  }
};

// Khởi tạo completion matrix rỗng cho 15 đơn vị thật
const initializeEmptyCompletionMatrix = () => {
  const matrix = {};
  branchUnits.value.forEach(unit => {
    matrix[unit.id] = {};
    sixMainIndicators.value.forEach(indicator => {
      // Tất cả đều null = chưa có số liệu, cần tính toán thực tế
      matrix[unit.id][indicator.code] = null;
    });
  });
  completionMatrix.value = matrix;
  console.log('🔧 Khởi tạo completion matrix rỗng cho 15 đơn vị thực tế');
};

// Status checking methods - updated to handle numeric values
const isCompleted = (unitId, indicatorCode) => {
  const value = completionMatrix.value[unitId]?.[indicatorCode];
  return value !== null && value !== undefined;
};

const getIndicatorValue = (unitId, indicatorCode) => {
  return completionMatrix.value[unitId]?.[indicatorCode] || null;
};

const formatIndicatorValue = (value, indicatorCode) => {
  if (value === null || value === undefined) return '-';

  switch (indicatorCode) {
    case 'DP01': // Nguồn vốn
    case 'LN01': // Dư nợ
    case 'LN03': // Thu nợ XLRR
    case 'GL41': // Thu dịch vụ
    case 'GL41': // Tài chính
      return new Intl.NumberFormat('vi-VN').format(value);
    case 'GL01': // Nợ xấu (%)
      return `${value}%`;
    default:
      return value.toString();
  }
};

const getValueUnit = (indicatorCode) => {
  switch (indicatorCode) {
    case 'DP01':
    case 'LN01':
    case 'LN03':
    case 'GL41':
    case 'GL41':
      return 'Tr VND';
    case 'GL01':
      return '';
    default:
      return '';
  }
};

const getStatusClass = (unitId, indicatorCode) => {
  return isCompleted(unitId, indicatorCode) ? 'agribank-status-completed' : 'agribank-status-pending';
};

const getCompletedCount = (unitId) => {
  if (!completionMatrix.value[unitId]) return 0;
  return Object.values(completionMatrix.value[unitId]).filter(value => value !== null && value !== undefined).length;
};

const getProgressStatus = (unitId) => {
  const completed = getCompletedCount(unitId);
  const total = sixMainIndicators.value.length;
  const rate = (completed / total) * 100;

  if (rate === 100) return 'Hoàn thành';
  if (rate >= 80) return 'Tốt';
  if (rate >= 50) return 'Khá';
  if (rate > 0) return 'Yếu';
  return 'Chưa bắt đầu';
};

const getProgressStatusClass = (unitId) => {
  const completed = getCompletedCount(unitId);
  const total = sixMainIndicators.value.length;
  const rate = (completed / total) * 100;

  if (rate === 100) return 'status-complete';
  if (rate >= 80) return 'status-good';
  if (rate >= 50) return 'status-fair';
  if (rate > 0) return 'status-poor';
  return 'status-none';
};

const getIndicatorCompletedCount = (indicatorCode) => {
  return branchUnits.value.filter(unit =>
    isCompleted(unit.id, indicatorCode)
  ).length;
};

const getIndicatorCompletionPercentage = (indicatorCode) => {
  const completed = getIndicatorCompletedCount(indicatorCode);
  const total = branchUnits.value.length;
  return Math.round((completed / total) * 100);
};

const getTotalCompletionPercentage = () => {
  const totalPossible = branchUnits.value.length * sixMainIndicators.value.length;
  let totalCompleted = 0;

  branchUnits.value.forEach(unit => {
    totalCompleted += getCompletedCount(unit.id);
  });

  return Math.round((totalCompleted / totalPossible) * 100);
};

const getUnitIcon = (unitType) => {
  switch(unitType) {
    case 'cnl1':
      return 'mdi-bank'; // CN Lai Châu - Chi nhánh cấp 1
    case 'cnl2':
      return 'mdi-office-building'; // CNL2 - Chi nhánh cấp 2
    case 'pgd':
      return 'mdi-domain'; // PGD - Phòng giao dịch
    default:
      return 'mdi-bank-outline';
  }
};

// Navigation methods
const navigateToDetail = (unitId, indicatorCode) => {
  console.log(`📊 Navigating to detail: Unit ${unitId}, Indicator ${indicatorCode}`);
  // Implement navigation logic here
  // router.push({
  //   name: 'IndicatorDetail',
  //   params: { unitId, indicatorCode }
  // });
};

// Event handlers
const onPeriodTypeChange = () => {
  selectedPeriod.value = '';
  console.log('📆 Period type changed:', periodType.value);
};

const loadData = async () => {
  if (!selectedYear.value || !selectedPeriod.value) {
    console.log('⚠️ Thiếu thông tin bộ lọc bắt buộc');
    return;
  }

  isLoading.value = true;
  console.log('🔄 Đang load dữ liệu thực tế với bộ lọc:', {
    year: selectedYear.value,
    periodType: periodType.value,
    period: selectedPeriod.value,
    units: branchUnits.value.length
  });

  try {
    // Load completion data từ backend thực tế
    await loadCompletionData();

    console.log('✅ Đã load dữ liệu thành công từ backend');
  } catch (error) {
    console.error('❌ Lỗi khi load dữ liệu:', error);
  } finally {
    isLoading.value = false;
  }
};

// Action Button Handlers
const calculateSixIndicators = async () => {
  console.log('🧮 Bắt đầu tính toán 6 chỉ tiêu chính cho 15 đơn vị...');
  isLoading.value = true;

  try {
    // Gọi API backend để tính toán thực tế
    const response = await fetch('http://localhost:5055/api/kpi/calculate-six-indicators', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        year: selectedYear.value,
        period: selectedPeriod.value,
        periodType: periodType.value,
        units: branchUnits.value.map(unit => ({
          id: unit.id,
          code: unit.code,
          unitCode: unit.unitCode
        }))
      })
    });

    if (response.ok) {
      const result = await response.json();
      console.log('✅ Kết quả tính toán từ backend:', result);

      // Reload data sau khi tính toán
      await loadCompletionData();
    } else {
      console.warn('⚠️ Backend API chưa sẵn sàng cho tính toán');
    }

    console.log('✅ Hoàn thành tính toán 6 chỉ tiêu');
  } catch (error) {
    console.error('❌ Lỗi khi tính toán:', error);
  } finally {
    isLoading.value = false;
  }
};

const navigateToCapital = () => {
  console.log('🏦 Chuyển đến màn hình Nguồn vốn (RR01)');
  // router.push({ name: 'CapitalManagement' });
};

const navigateToDebt = () => {
  console.log('💰 Chuyển đến màn hình Dư nợ (DP01)');
  // router.push({ name: 'DebtManagement' });
};

const navigateToBadDebt = () => {
  console.log('⚠️ Chuyển đến màn hình Nợ xấu (GL01)');
  // router.push({ name: 'BadDebtManagement' });
};

const navigateToService = () => {
  console.log('🧾 Chuyển đến màn hình Thu dịch vụ (GL41)');
  // router.push({ name: 'ServiceRevenue' });
};

const navigateToXLRR = () => {
  console.log('💸 Chuyển đến màn hình Thu nợ XLRR (EI01)');
  // router.push({ name: 'XLRRRevenue' });
};

const navigateToFinance = () => {
  console.log('📊 Chuyển đến màn hình Tài chính (LN03)');
  // router.push({ name: 'FinancialReports' });
};

// ===== LIFECYCLE =====
onMounted(() => {
  console.log('🚀 CalculationDashboard mounted với 15 đơn vị thực tế');

  // Set default values
  selectedYear.value = '2024';
  periodType.value = 'monthly';
  selectedPeriod.value = 'Tháng 12';

  // Khởi tạo dữ liệu thực tế - không dùng mockdata
  initializeEmptyCompletionMatrix();

  // Load dữ liệu completion từ backend
  loadCompletionData();
});

// ===== WATCHERS =====
watch([selectedYear, selectedPeriod], () => {
  if (selectedYear.value && selectedPeriod.value) {
    loadData();
  }
});
</script>

<style scoped>
/* Import Agribank Themes */
@import '@/assets/css/agribank-theme.css';
@import '@/assets/css/agribank-dashboard.css';
</style>
