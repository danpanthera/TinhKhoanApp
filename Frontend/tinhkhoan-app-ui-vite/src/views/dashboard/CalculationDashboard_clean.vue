<template>
  <div class="calculation-dashboard agribank-theme">
    <!-- Header với thương hiệu Agribank -->
    <div class="page-header agribank-header">
      <div class="agribank-brand">
        <img src="/agribank-logo.svg" alt="Agribank Logo" class="agribank-logo">
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
                  <div v-if="isCompleted(unit.id, indicator.code)" class="status-success-wrapper">
                    <i class="mdi mdi-check-circle agribank-success-icon"></i>
                    <span class="status-text">Hoàn thành</span>
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

// Sample data for development
const branchUnits = ref([
  { id: 1, name: 'Chi nhánh Hà Nội', code: 'HN001', type: 'branch' },
  { id: 2, name: 'Chi nhánh TP.HCM', code: 'HCM001', type: 'branch' },
  { id: 3, name: 'Chi nhánh Đà Nẵng', code: 'DN001', type: 'branch' },
  { id: 4, name: 'Chi nhánh Hải Phòng', code: 'HP001', type: 'branch' },
  { id: 5, name: 'Chi nhánh Cần Thơ', code: 'CT001', type: 'branch' },
  { id: 6, name: 'Chi nhánh Nghệ An', code: 'NA001', type: 'branch' },
  { id: 7, name: 'Chi nhánh Thái Nguyên', code: 'TN001', type: 'branch' },
  { id: 8, name: 'Chi nhánh Quảng Ninh', code: 'QN001', type: 'branch' },
  { id: 9, name: 'Chi nhánh Bình Dương', code: 'BD001', type: 'branch' },
  { id: 10, name: 'Chi nhánh Đồng Nai', code: 'DN002', type: 'branch' },
  { id: 11, name: 'Chi nhánh Long An', code: 'LA001', type: 'branch' },
  { id: 12, name: 'Chi nhánh Tiền Giang', code: 'TG001', type: 'branch' },
  { id: 13, name: 'Chi nhánh An Giang', code: 'AG001', type: 'branch' },
  { id: 14, name: 'Chi nhánh Kiên Giang', code: 'KG001', type: 'branch' },
  { id: 15, name: 'Chi nhánh Bạc Liêu', code: 'BL001', type: 'branch' }
]);

const sixMainIndicators = ref([
  {
    code: 'DP01',
    name: 'Dư nợ tín dụng',
    unit: 'Tỷ VND',
    icon: 'mdi-account-cash'
  },
  {
    code: 'RR01',
    name: 'Huy động vốn',
    unit: 'Tỷ VND',
    icon: 'mdi-bank-transfer'
  },
  {
    code: 'LN03',
    name: 'Lợi nhuận',
    unit: 'Tỷ VND',
    icon: 'mdi-trending-up'
  },
  {
    code: 'GL01',
    name: 'Thu nhập lãi',
    unit: 'Tỷ VND',
    icon: 'mdi-cash-multiple'
  },
  {
    code: 'EI01',
    name: 'Chi phí hoạt động',
    unit: 'Tỷ VND',
    icon: 'mdi-chart-line-variant'
  },
  {
    code: 'GL41',
    name: 'Phí dịch vụ',
    unit: 'Tỷ VND',
    icon: 'mdi-receipt'
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

// Initialize mock data
const initializeMockData = () => {
  const matrix = {};
  branchUnits.value.forEach(unit => {
    matrix[unit.id] = {};
    sixMainIndicators.value.forEach(indicator => {
      // Random completion status for demo
      matrix[unit.id][indicator.code] = Math.random() > 0.3;
    });
  });
  completionMatrix.value = matrix;
  console.log('🎯 Mock completion matrix initialized:', matrix);
};

// Status checking methods
const isCompleted = (unitId, indicatorCode) => {
  return completionMatrix.value[unitId]?.[indicatorCode] || false;
};

const getStatusClass = (unitId, indicatorCode) => {
  return isCompleted(unitId, indicatorCode) ? 'agribank-status-completed' : 'agribank-status-pending';
};

const getCompletedCount = (unitId) => {
  if (!completionMatrix.value[unitId]) return 0;
  return Object.values(completionMatrix.value[unitId]).filter(Boolean).length;
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
    case 'branch':
      return 'mdi-bank';
    case 'department':
      return 'mdi-office-building';
    default:
      return 'mdi-domain';
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
    console.log('⚠️ Missing required filters');
    return;
  }

  isLoading.value = true;
  console.log('🔄 Loading data with filters:', {
    year: selectedYear.value,
    periodType: periodType.value,
    period: selectedPeriod.value
  });

  try {
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000));

    // Refresh mock data
    initializeMockData();

    console.log('✅ Data loaded successfully');
  } catch (error) {
    console.error('❌ Error loading data:', error);
  } finally {
    isLoading.value = false;
  }
};

// ===== LIFECYCLE =====
onMounted(() => {
  console.log('🚀 CalculationDashboard mounted');

  // Set default values
  selectedYear.value = '2024';
  periodType.value = 'monthly';
  selectedPeriod.value = 'Tháng 12';

  // Initialize mock data
  initializeMockData();
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
