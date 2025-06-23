<template>
  <div class="calculation-dashboard">
    <!-- Header -->
    <div class="page-header">
      <div class="header-title">
        <h2>
          <i class="mdi mdi-database-plus"></i>
          Cập nhật số liệu vào bảng
        </h2>
        <p class="subtitle">Tính toán và cập nhật các chỉ tiêu kinh doanh theo chi nhánh</p>
      </div>
      
      <div class="header-controls">
        <!-- Time filters -->
        <select v-model="selectedYear" @change="loadData" class="form-select">
          <option value="">Chọn năm</option>
          <option v-for="year in yearOptions" :key="year" :value="year">
            {{ year }}
          </option>
        </select>
        
        <select v-model="periodType" @change="onPeriodTypeChange" class="form-select">
          <option value="">Chọn loại kỳ</option>
          <option v-for="period in periodTypeOptions" :key="period.value" :value="period.value">
            {{ period.label }}
          </option>
        </select>
        
        <select v-if="periodType === 'QUARTER'" v-model="selectedPeriod" @change="loadData" class="form-select">
          <option value="">Chọn quý</option>
          <option v-for="quarter in quarterOptions" :key="quarter.value" :value="quarter.value">
            {{ quarter.label }}
          </option>
        </select>
        
        <select v-if="periodType === 'MONTH'" v-model="selectedPeriod" @change="loadData" class="form-select">
          <option value="">Chọn tháng</option>
          <option v-for="month in monthOptions" :key="month.value" :value="month.value">
            {{ month.label }}
          </option>
        </select>
        
        <select v-model="selectedUnitId" @change="loadData" class="form-select">
          <option value="">Tất cả đơn vị (Toàn tỉnh)</option>
          <option v-for="unit in units" :key="unit.id" :value="unit.id">
            {{ unit.name }}
          </option>
        </select>
        
        <!-- Action buttons -->
        <button @click="loadData" :disabled="loading" class="btn btn-primary">
          {{ loading ? 'Đang tải...' : '🔄 Tải lại' }}
        </button>
        
        <button @click="triggerCalculation" :disabled="calculating" class="btn btn-success">
          {{ calculating ? 'Đang tính...' : '⚡ Tính toán' }}
        </button>
      </div>
    </div>

    <!-- Enhanced Loading Overlays -->
    <LoadingOverlay 
      :show="loading" 
      title="Đang tải dữ liệu" 
      message="Đang truy xuất dữ liệu từ hệ thống..."
      icon="📊"
    />
    
    <LoadingOverlay 
      :show="calculating" 
      title="Đang tính toán" 
      message="Vui lòng chờ trong khi hệ thống tính toán các chỉ tiêu..."
      icon="⚡"
    />

    <!-- Error Message -->
    <div v-if="errorMessage" class="error-message">
      <p>❌ {{ errorMessage }}</p>
    </div>

    <!-- Success Message -->
    <div v-if="successMessage" class="success-message">
      <p>✅ {{ successMessage }}</p>
    </div>

    <!-- Dashboard Content -->
    <div v-if="!loading" class="dashboard-content">
      
      <!-- KPI Overview Cards -->
      <div class="overview-section">
        <h3>📊 Tổng quan chỉ tiêu</h3>
        <div class="kpi-cards">
          <div class="kpi-card">
            <div class="card-icon">🎯</div>
            <div class="card-content">
              <div class="card-value">{{ formatNumber(overview.totalTargets) }}</div>
              <div class="card-label">Tổng số chỉ tiêu</div>
            </div>
          </div>
          
          <div class="kpi-card">
            <div class="card-icon">✅</div>
            <div class="card-content">
              <div class="card-value">{{ formatNumber(overview.completedTargets) }}</div>
              <div class="card-label">Đã hoàn thành</div>
            </div>
          </div>
          
          <div class="kpi-card">
            <div class="card-icon">📈</div>
            <div class="card-content">
              <div class="card-value">{{ formatPercentage(overview.achievementRate) }}</div>
              <div class="card-label">Tỷ lệ đạt được</div>
            </div>
          </div>
          
          <div class="kpi-card">
            <div class="card-icon">💰</div>
            <div class="card-content">
              <div class="card-value">{{ formatNumber(overview.totalValue) }}</div>
              <div class="card-label">Tổng giá trị (VND)</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Performance by Unit -->
      <div class="performance-section">
        <h3>🏢 Hiệu suất theo đơn vị</h3>
        <div class="performance-table-container">
          <table v-if="performanceData.length > 0" class="performance-table">
            <thead>
              <tr>
                <th>Đơn vị</th>
                <th>Số chỉ tiêu</th>
                <th>Đã hoàn thành</th>
                <th>Tỷ lệ hoàn thành</th>
                <th>Tổng giá trị mục tiêu</th>
                <th>Giá trị đạt được</th>
                <th>Trạng thái</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="unit in performanceData" :key="unit.unitId">
                <td class="unit-name">{{ unit.unitName }}</td>
                <td class="number-cell">{{ formatNumber(unit.totalTargets) }}</td>
                <td class="number-cell">{{ formatNumber(unit.completedTargets) }}</td>
                <td class="number-cell">
                  <div class="progress-container">
                    <div class="progress-bar">
                      <div 
                        class="progress-fill" 
                        :style="{ width: unit.completionRate + '%' }"
                        :class="getProgressClass(unit.completionRate)"
                      ></div>
                    </div>
                    <span class="progress-text">{{ formatPercentage(unit.completionRate) }}</span>
                  </div>
                </td>
                <td class="number-cell">{{ formatNumber(unit.totalTargetValue) }}</td>
                <td class="number-cell">{{ formatNumber(unit.actualValue) }}</td>
                <td>
                  <span :class="['status-badge', getStatusClass(unit.completionRate)]">
                    {{ getStatusText(unit.completionRate) }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
          
          <div v-else class="no-data">
            <p>Không có dữ liệu hiệu suất cho kỳ đã chọn.</p>
          </div>
        </div>
      </div>

      <!-- Calculation Results -->
      <div class="calculation-section">
        <h3>🔢 Kết quả tính toán</h3>
        <div class="calculation-results">
          <div v-if="calculationResults.length > 0" class="results-table-container">
            <table class="results-table">
              <thead>
                <tr>
                  <th>Chỉ tiêu</th>
                  <th>Đơn vị</th>
                  <th>Giá trị mục tiêu</th>
                  <th>Giá trị thực hiện</th>
                  <th>Tỷ lệ đạt được</th>
                  <th>Điểm số</th>
                  <th>Thời gian tính</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="result in calculationResults" :key="result.id">
                  <td>{{ result.indicatorName }}</td>
                  <td>{{ result.unitName }}</td>
                  <td class="number-cell">{{ formatNumber(result.targetValue) }}</td>
                  <td class="number-cell">{{ formatNumber(result.actualValue) }}</td>
                  <td class="number-cell">
                    <span :class="['percentage', getPerformanceClass(result.achievementRate)]">
                      {{ formatPercentage(result.achievementRate) }}
                    </span>
                  </td>
                  <td class="number-cell">
                    <span :class="['score', getScoreClass(result.score)]">
                      {{ formatNumber(result.score) }}
                    </span>
                  </td>
                  <td>{{ formatDateTime(result.calculationDate) }}</td>
                </tr>
              </tbody>
            </table>
          </div>
          
          <div v-else class="no-data">
            <p>Chưa có kết quả tính toán nào. Nhấn nút "Tính toán" để bắt đầu.</p>
          </div>
        </div>
      </div>

      <!-- 6 Chỉ tiêu sau khi tính toán -->
      <div v-if="showCalculationResults" class="calculation-results">
        <div class="results-header">
          <h3>
            <i class="mdi mdi-chart-line"></i>
            Kết quả tính toán chỉ tiêu
          </h3>
          <div class="selected-unit-info">
            <span v-if="selectedUnitId">{{ getSelectedUnitName() }}</span>
            <span v-else>Toàn tỉnh (7800-7808)</span>
          </div>
        </div>

        <!-- Warning nếu thiếu chỉ tiêu -->
        <div v-if="missingIndicators.length > 0" class="warning-box">
          <i class="mdi mdi-alert-circle"></i>
          <div class="warning-content">
            <h4>⚠️ Cảnh báo: Thiếu dữ liệu</h4>
            <p>Các chỉ tiêu sau chưa được tính toán đầy đủ:</p>
            <ul>
              <li v-for="indicator in missingIndicators" :key="indicator">{{ indicator }}</li>
            </ul>
          </div>
        </div>

        <!-- Grid hiển thị 6 chỉ tiêu -->
        <div class="indicators-results-grid">
          <div 
            v-for="(indicator, index) in calculatedIndicators" 
            :key="indicator.id"
            class="result-card"
            :class="[indicator.class, { 'calculated': indicator.calculated, 'missing': !indicator.calculated }]"
            :style="{ animationDelay: `${index * 0.1}s` }"
          >
            <div class="result-card-header">
              <div class="result-icon">{{ indicator.icon }}</div>
              <div class="result-title">{{ indicator.name }}</div>
              <div class="result-status">
                <i v-if="indicator.calculated" class="mdi mdi-check-circle status-success"></i>
                <i v-else class="mdi mdi-alert-circle status-warning"></i>
              </div>
            </div>
            
            <div class="result-body">
              <div class="result-value">
                <span class="value-number">{{ formatNumber(indicator.value) }}</span>
                <span class="value-unit">{{ indicator.unit }}</span>
              </div>
              
              <div class="result-status-text">
                <span v-if="indicator.calculated" class="calculated-text">✅ Đã tính toán</span>
                <span v-else class="missing-text">❌ Chưa có dữ liệu</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Trend Analysis -->
      <div class="trend-section">
        <h3>📈 Phân tích xu hướng</h3>
        <div class="trend-controls">
          <button 
            @click="loadTrendData('MONTH')" 
            :class="['trend-btn', { active: trendPeriod === 'MONTH' }]"
          >
            Theo tháng
          </button>
          <button 
            @click="loadTrendData('QUARTER')" 
            :class="['trend-btn', { active: trendPeriod === 'QUARTER' }]"
          >
            Theo quý
          </button>
          <button 
            @click="loadTrendData('YEAR')" 
            :class="['trend-btn', { active: trendPeriod === 'YEAR' }]"
          >
            Theo năm
          </button>
        </div>
        
        <div v-if="trendData.length > 0" class="trend-chart">
          <!-- Simple trend visualization -->
          <div class="chart-container">
            <div v-for="(point, index) in trendData" :key="index" class="trend-point">
              <div class="point-value">{{ formatPercentage(point.achievementRate) }}</div>
              <div class="point-bar">
                <div 
                  class="bar-fill" 
                  :style="{ height: (point.achievementRate || 0) + '%' }"
                  :class="getPerformanceClass(point.achievementRate)"
                ></div>
              </div>
              <div class="point-label">{{ point.periodLabel }}</div>
            </div>
          </div>
        </div>
        
        <div v-else class="no-data">
          <p>Chưa có dữ liệu xu hướng.</p>
        </div>
      </div>

    </div>

    <!-- Action buttons -->
    <div v-if="!loading" class="action-section">
      <button @click="exportDashboard" class="btn btn-info">
        📊 Xuất báo cáo Dashboard
      </button>
      <button @click="refreshData" class="btn btn-secondary">
        🔄 Làm mới dữ liệu
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import { useRouter } from 'vue-router';
import { isAuthenticated } from '../../services/auth';
import { dashboardService } from '../../services/dashboardService';
import LoadingOverlay from '../../components/dashboard/LoadingOverlay.vue';

const router = useRouter();

// Reactive data
const loading = ref(false);
const calculating = ref(false);
const errorMessage = ref('');
const successMessage = ref('');

// Filters
const selectedYear = ref(new Date().getFullYear());
const periodType = ref('');
const selectedPeriod = ref('');
const selectedUnitId = ref('');
const trendPeriod = ref('MONTH');

// Data
const units = ref([
  { id: 'HoiSo', name: 'Hội Sở', code: '7800' },
  { id: 'CnTamDuong', name: 'CN Tam Đường', code: '7801' },
  { id: 'CnPhongTho', name: 'CN Phong Thổ', code: '7802' },
  { id: 'CnSinHo', name: 'CN Sin Hồ', code: '7803' },
  { id: 'CnMuongTe', name: 'CN Mường Tè', code: '7804' },
  { id: 'CnThanUyen', name: 'CN Than Uyên', code: '7805' },
  { id: 'CnThanhpho', name: 'CN Thành phố', code: '7806' },
  { id: 'CnTanUyen', name: 'CN Tân Uyên', code: '7807' },
  { id: 'CnNamNhun', name: 'CN Nậm Nhùn', code: '7808' },
  { id: 'CnPhongThoPgdMuongSo', name: 'CN Phong Thổ - PGD Mường So', code: '7802' },
  { id: 'CnThanUyenPgdMuongThan', name: 'CN Than Uyên - PGD Mường Than', code: '7805' },
  { id: 'CnThanhPhoPgdso1', name: 'CN Thành phố - PGD số 1', code: '7806' },
  { id: 'CnThanhPhoPgdso2', name: 'CN Thành phố - PGD số 2', code: '7806' },
  { id: 'CnTanUyenPgdso3', name: 'CN Tân Uyên - PGD số 3', code: '7807' }
]);
const overview = ref({
  totalTargets: 0,
  completedTargets: 0,
  achievementRate: 0,
  totalValue: 0
});
const performanceData = ref([]);
const calculationResults = ref([]);
const trendData = ref([]);

// 6 chỉ tiêu tính toán
const calculatedIndicators = ref([
  {
    id: 'nguon_von',
    name: 'Nguồn vốn',
    icon: '💰',
    class: 'nguon-von',
    unit: 'tỷ',
    value: 0,
    calculated: false
  },
  {
    id: 'du_no',
    name: 'Dư nợ',
    icon: '💳',
    class: 'du-no',
    unit: 'tỷ',
    value: 0,
    calculated: false
  },
  {
    id: 'no_xau',
    name: 'Nợ Xấu',
    icon: '⚠️',
    class: 'no-xau',
    unit: '%',
    value: 0,
    calculated: false
  },
  {
    id: 'thu_no_xlrr',
    name: 'Thu nợ đã XLRR',
    icon: '📈',
    class: 'thu-no-xlrr',
    unit: 'tỷ',
    value: 0,
    calculated: false
  },
  {
    id: 'thu_dich_vu',
    name: 'Thu dịch vụ',
    icon: '🏦',
    class: 'thu-dich-vu',
    unit: 'tỷ',
    value: 0,
    calculated: false
  },
  {
    id: 'tai_chinh',
    name: 'Tài chính',
    icon: '💵',
    class: 'tai-chinh',
    unit: 'tỷ',
    value: 0,
    calculated: false
  }
]);

// Options
const yearOptions = ref(dashboardService.getYearOptions());
const quarterOptions = ref(dashboardService.getQuarterOptions());
const monthOptions = ref(dashboardService.getMonthOptions());
const periodTypeOptions = ref(dashboardService.getPeriodTypeOptions());

// Reactive variables
const showCalculationResults = ref(false);

// Computed properties
const missingIndicators = computed(() => {
  return calculatedIndicators.value
    .filter(indicator => !indicator.calculated)
    .map(indicator => indicator.name);
});

const getSelectedUnitName = () => {
  if (!selectedUnitId.value) return 'Tất cả đơn vị';
  const unit = units.value.find(u => u.id === selectedUnitId.value);
  return unit ? unit.name : 'Không xác định';
};

// Methods
const loadUnits = async () => {
  try {
    const response = await dashboardService.getUnits();
    units.value = response || [];
  } catch (error) {
    console.error('Error loading units:', error);
    errorMessage.value = 'Không thể tải danh sách đơn vị';
  }
};

const loadData = async () => {
  if (!selectedYear.value) return;
  
  loading.value = true;
  errorMessage.value = '';
  
  try {
    const params = {
      year: selectedYear.value
    };
    
    if (periodType.value) params.periodType = periodType.value;
    if (selectedPeriod.value && periodType.value !== 'YEAR') params.period = selectedPeriod.value;
    if (selectedUnitId.value) params.unitId = selectedUnitId.value;
    
    // Load dashboard data
    const dashboardData = await dashboardService.getDashboardData(params);
    if (dashboardData) {
      overview.value = dashboardData.overview || overview.value;
      performanceData.value = dashboardData.performanceByUnit || [];
    }
    
    // Load calculation results
    const calculationData = await dashboardService.getCalculationResults(params);
    calculationResults.value = calculationData || [];
    
  } catch (error) {
    console.error('Error loading dashboard data:', error);
    errorMessage.value = 'Không thể tải dữ liệu dashboard';
  } finally {
    loading.value = false;
  }
};

const loadTrendData = async (period) => {
  trendPeriod.value = period;
  
  try {
    const params = {
      year: selectedYear.value,
      periodType: period
    };
    
    if (selectedUnitId.value) params.unitId = selectedUnitId.value;
    
    const response = await dashboardService.getTrendData(params);
    trendData.value = response || [];
  } catch (error) {
    console.error('Error loading trend data:', error);
    errorMessage.value = 'Không thể tải dữ liệu xu hướng';
  }
};

const triggerCalculation = async () => {
  if (!selectedYear.value) {
    errorMessage.value = 'Vui lòng chọn năm để tính toán';
    return;
  }
  
  calculating.value = true;
  errorMessage.value = '';
  successMessage.value = '';
  showCalculationResults.value = false;
  
  try {
    const params = {
      year: selectedYear.value
    };
    
    if (periodType.value) params.periodType = periodType.value;
    if (selectedPeriod.value && periodType.value !== 'YEAR') params.period = selectedPeriod.value;
    if (selectedUnitId.value) params.unitId = selectedUnitId.value;
    
    // Tính toán theo chi nhánh được chọn
    let unitCodes = [];
    if (selectedUnitId.value) {
      const selectedUnit = units.value.find(u => u.id === selectedUnitId.value);
      unitCodes = [selectedUnit?.code];
    } else {
      // Tất cả đơn vị: từ 7800 -> 7808
      unitCodes = ['7800', '7801', '7802', '7803', '7804', '7805', '7806', '7807', '7808'];
    }
    
    params.unitCodes = unitCodes;
    
    await dashboardService.triggerCalculations(params);
    
    // Mock dữ liệu tính toán 6 chỉ tiêu (sau này sẽ thay bằng API thực)
    setTimeout(() => {
      // Simulate calculation results
      calculatedIndicators.value[0].value = 1250.5;
      calculatedIndicators.value[0].calculated = true;
      
      calculatedIndicators.value[1].value = 980.3;
      calculatedIndicators.value[1].calculated = true;
      
      calculatedIndicators.value[2].value = 1.8;
      calculatedIndicators.value[2].calculated = true;
      
      calculatedIndicators.value[3].value = 45.7;
      calculatedIndicators.value[3].calculated = true;
      
      calculatedIndicators.value[4].value = 28.9;
      calculatedIndicators.value[4].calculated = true;
      
      calculatedIndicators.value[5].value = 156.4;
      calculatedIndicators.value[5].calculated = true;
      
      showCalculationResults.value = true;
      successMessage.value = 'Tính toán hoàn thành thành công cho ' + (selectedUnitId.value ? getSelectedUnitName() : 'toàn tỉnh');
    }, 1000);
    
    // Reload data after calculation
    await loadData();
    
  } catch (error) {
    console.error('Error triggering calculation:', error);
    errorMessage.value = 'Có lỗi xảy ra khi thực hiện tính toán: ' + (error.response?.data?.message || error.message);
  } finally {
    calculating.value = false;
  }
};

const onPeriodTypeChange = () => {
  selectedPeriod.value = '';
  loadData();
};

const refreshData = () => {
  loadData();
  loadTrendData(trendPeriod.value);
};

const exportDashboard = () => {
  // TODO: Implement export functionality
  alert('Chức năng xuất báo cáo sẽ được phát triển trong phiên bản tiếp theo');
};

// Utility methods
const formatNumber = (value) => {
  if (!value && value !== 0) return '0';
  return Number(value).toLocaleString('vi-VN');
};

const formatPercentage = (value) => {
  if (!value && value !== 0) return '0%';
  return Number(value).toFixed(1) + '%';
};

const formatDateTime = (dateString) => {
  if (!dateString) return '';
  return new Date(dateString).toLocaleString('vi-VN');
};

const getProgressClass = (rate) => {
  if (rate >= 90) return 'excellent';
  if (rate >= 75) return 'good';
  if (rate >= 50) return 'average';
  return 'poor';
};

const getStatusClass = (rate) => {
  if (rate >= 90) return 'excellent';
  if (rate >= 75) return 'good';
  if (rate >= 50) return 'average';
  return 'poor';
};

const getStatusText = (rate) => {
  if (rate >= 90) return 'Xuất sắc';
  if (rate >= 75) return 'Tốt';
  if (rate >= 50) return 'Trung bình';
  return 'Cần cải thiện';
};

const getPerformanceClass = (rate) => {
  if (rate >= 100) return 'over-target';
  if (rate >= 90) return 'excellent';
  if (rate >= 75) return 'good';
  if (rate >= 50) return 'average';
  return 'poor';
};

const getScoreClass = (score) => {
  if (score >= 90) return 'high-score';
  if (score >= 70) return 'medium-score';
  return 'low-score';
};

// Clear messages after 5 seconds
watch([errorMessage, successMessage], () => {
  setTimeout(() => {
    errorMessage.value = '';
    successMessage.value = '';
  }, 5000);
});

// Lifecycle
onMounted(async () => {
  if (!isAuthenticated()) {
    router.push('/login');
    return;
  }
  
  await loadUnits();
  await loadData();
  await loadTrendData(trendPeriod.value);
});
</script>

<style scoped>
.calculation-dashboard {
  padding: 20px;
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  min-height: calc(100vh - 60px);
}

.page-header {
  background: linear-gradient(135deg, #8B1538 0%, #A6195C 50%, #B91D47 100%);
  color: white;
  padding: 30px;
  box-shadow: 0 4px 20px rgba(139, 21, 56, 0.3);
  position: relative;
  overflow: hidden;
}

.page-header::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23ffffff' fill-opacity='0.03'%3E%3Ccircle cx='30' cy='30' r='2'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E") repeat;
  z-index: 1;
}

.header-title {
  position: relative;
  z-index: 2;
  margin-bottom: 25px;
}

.page-header h2 {
  margin: 0;
  color: white;
  font-weight: 600;
  font-size: 28px;
  display: flex;
  align-items: center;
  gap: 15px;
  font-family: 'Segoe UI', 'Open Sans', sans-serif;
}

.page-header h2 i {
  font-size: 32px;
  opacity: 0.9;
}

.subtitle {
  margin: 8px 0 0 47px;
  font-size: 16px;
  opacity: 0.9;
  font-family: 'Segoe UI', 'Open Sans', sans-serif;
}

.header-controls {
  display: flex;
  gap: 15px;
  flex-wrap: wrap;
  align-items: center;
  position: relative;
  z-index: 2;
}

.dashboard-content {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.overview-section,
.performance-section,
.calculation-section,
.trend-section {
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.overview-section h3,
.performance-section h3,
.calculation-section h3,
.trend-section h3 {
  margin: 0 0 20px 0;
  color: #303133;
  font-size: 18px;
}

.kpi-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 20px;
}

.kpi-card {
  display: flex;
  align-items: center;
  padding: 20px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.card-icon {
  font-size: 36px;
  margin-right: 16px;
}

.card-content {
  flex: 1;
}

.card-value {
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 4px;
}

.card-label {
  font-size: 14px;
  opacity: 0.9;
}

.performance-table-container,
.results-table-container {
  overflow-x: auto;
}

.performance-table,
.results-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
}

.performance-table th,
.performance-table td,
.results-table th,
.results-table td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #eee;
}

.performance-table th,
.results-table th {
  background: #f5f7fa;
  font-weight: 600;
  color: #303133;
}

.number-cell {
  text-align: right;
  font-family: monospace;
}

.unit-name {
  font-weight: 600;
  color: #8B1538;
}

.progress-container {
  display: flex;
  align-items: center;
  gap: 8px;
}

.progress-bar {
  flex: 1;
  height: 8px;
  background: #f0f0f0;
  border-radius: 4px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  transition: width 0.3s ease;
}

.progress-fill.excellent {
  background: linear-gradient(90deg, #52c41a, #73d13d);
}

.progress-fill.good {
  background: linear-gradient(90deg, #8B1538, #A6195C);
}

.progress-fill.average {
  background: linear-gradient(90deg, #faad14, #ffc53d);
}

.progress-fill.poor {
  background: linear-gradient(90deg, #ff4d4f, #ff7875);
}

.progress-text {
  font-size: 12px;
  font-weight: 600;
  min-width: 40px;
}

.status-badge {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.status-badge.excellent {
  background: #d4edda;
  color: #155724;
}

.status-badge.good {
  background: #cce5ff;
  color: #004085;
}

.status-badge.average {
  background: #fff3cd;
  color: #856404;
}

.status-badge.poor {
  background: #f8d7da;
  color: #721c24;
}

.percentage.over-target {
  color: #52c41a;
  font-weight: bold;
}

.percentage.excellent {
  color: #8B1538;
  font-weight: bold;
}

.percentage.good {
  color: #faad14;
}

.percentage.average {
  color: #fa8c16;
}

.percentage.poor {
  color: #ff4d4f;
}

.score.high-score {
  color: #52c41a;
  font-weight: bold;
}

.score.medium-score {
  color: #8B1538;
}

.score.low-score {
  color: #ff4d4f;
}

.trend-controls {
  display: flex;
  gap: 8px;
  margin-bottom: 20px;
}

.trend-btn {
  padding: 8px 16px;
  border: 1px solid #d9d9d9;
  background: white;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.trend-btn:hover {
  border-color: #8B1538;
  color: #8B1538;
}

.trend-btn.active {
  background: #8B1538;
  color: white;
  border-color: #8B1538;
}

.chart-container {
  display: flex;
  align-items: end;
  justify-content: space-around;
  height: 200px;
  padding: 20px 0;
  border: 1px solid #f0f0f0;
  border-radius: 4px;
}

.trend-point {
  display: flex;
  flex-direction: column;
  align-items: center;
  min-width: 60px;
}

.point-value {
  font-size: 12px;
  font-weight: 600;
  margin-bottom: 4px;
  color: #666;
}

.point-bar {
  width: 20px;
  height: 120px;
  background: #f0f0f0;
  border-radius: 10px;
  overflow: hidden;
  display: flex;
  align-items: end;
}

.bar-fill {
  width: 100%;
  border-radius: 10px;
  transition: height 0.5s ease;
  min-height: 2px;
}

.point-label {
  font-size: 10px;
  color: #666;
  margin-top: 8px;
  text-align: center;
}

.btn {
  padding: 8px 16px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.3s ease;
}

.btn-primary {
  background: #8B1538;
  color: white;
}

.btn-primary:hover {
  background: #A6195C;
}

.btn-success {
  background: #52c41a;
  color: white;
}

.btn-success:hover {
  background: #73d13d;
}

.btn-info {
  background: #13c2c2;
  color: white;
}

.btn-info:hover {
  background: #36cfc9;
}

.btn-secondary {
  background: #d9d9d9;
  color: #333;
}

.btn-secondary:hover {
  background: #f0f0f0;
}

.form-select {
  padding: 8px 12px;
  border: 1px solid #d9d9d9;
  border-radius: 4px;
  font-size: 14px;
}

.form-select:focus {
  outline: none;
  border-color: #8B1538;
  box-shadow: 0 0 0 2px rgba(24, 144, 255, 0.2);
}

.loading-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 20px;
  text-align: center;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #8B1538;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 16px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error-message {
  background: #fff2f0;
  border: 1px solid #ffccc7;
  color: #a8071a;
  padding: 12px 16px;
  border-radius: 4px;
  margin-bottom: 16px;
}

.success-message {
  background: #f6ffed;
  border: 1px solid #b7eb8f;
  color: #389e0d;
  padding: 12px 16px;
  border-radius: 4px;
  margin-bottom: 16px;
}

.no-data {
  text-align: center;
  padding: 40px;
  color: #8c8c8c;
}

.action-section {
  display: flex;
  gap: 12px;
  justify-content: center;
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

/* Calculation Results Styles */
.calculation-results {
  margin-top: 30px;
  padding: 24px;
  background: linear-gradient(135deg, #f8f9ff 0%, #f0f2ff 100%);
  border-radius: 16px;
  border: 1px solid #e6f0ff;
}

.results-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
  padding-bottom: 16px;
  border-bottom: 2px solid #e6f0ff;
}

.results-header h3 {
  color: #1890ff;
  font-size: 24px;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 8px;
}

.selected-unit-info {
  background: #1890ff;
  color: white;
  padding: 8px 16px;
  border-radius: 20px;
  font-size: 14px;
  font-weight: 500;
}

.warning-box {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  background: #fff7e6;
  border: 1px solid #ffd666;
  border-radius: 12px;
  padding: 16px;
  margin-bottom: 24px;
  animation: shake 0.5s ease-in-out;
}

.warning-box i {
  color: #fa8c16;
  font-size: 24px;
  margin-top: 4px;
}

.warning-content h4 {
  color: #d48806;
  margin: 0 0 8px 0;
  font-size: 16px;
}

.warning-content p {
  margin: 0 0 12px 0;
  color: #8c5a00;
}

.warning-content ul {
  margin: 0;
  padding-left: 20px;
  color: #8c5a00;
}

.indicators-results-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 20px;
}

.result-card {
  background: white;
  border-radius: 16px;
  padding: 24px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
  animation: fadeInUp 0.6s ease-out;
  position: relative;
  overflow: hidden;
}

.result-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, var(--card-color) 0%, var(--card-color-light) 100%);
}

.result-card.calculated {
  border: 2px solid #52c41a;
  transform: scale(1.02);
}

.result-card.missing {
  border: 2px solid #ff7875;
  opacity: 0.7;
}

.result-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
}

/* Card color variables */
.result-card.nguon-von { --card-color: #52c41a; --card-color-light: #95de64; }
.result-card.du-no { --card-color: #1890ff; --card-color-light: #69c0ff; }
.result-card.no-xau { --card-color: #fa541c; --card-color-light: #ff7a45; }
.result-card.thu-no-xlrr { --card-color: #722ed1; --card-color-light: #b37feb; }
.result-card.thu-dich-vu { --card-color: #13c2c2; --card-color-light: #5cdbd3; }
.result-card.tai-chinh { --card-color: #faad14; --card-color-light: #ffc53d; }

.result-card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 20px;
}

.result-icon {
  font-size: 32px;
  margin-right: 12px;
}

.result-title {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  flex: 1;
}

.result-status .status-success {
  color: #52c41a;
  font-size: 24px;
}

.result-status .status-warning {
  color: #fa8c16;
  font-size: 24px;
}

.result-body {
  text-align: center;
}

.result-value {
  display: flex;
  align-items: baseline;
  justify-content: center;
  gap: 8px;
  margin-bottom: 16px;
}

.value-number {
  font-size: 32px;
  font-weight: 700;
  color: var(--card-color);
}

.value-unit {
  font-size: 16px;
  color: #666;
}

.result-status-text {
  padding: 8px 16px;
  border-radius: 20px;
  font-size: 14px;
  font-weight: 500;
  display: inline-block;
}

.calculated-text {
  background: #f6ffed;
  color: #389e0d;
  border: 1px solid #b7eb8f;
}

.missing-text {
  background: #fff2f0;
  color: #a8071a;
  border: 1px solid #ffccc7;
}

@keyframes shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-5px); }
  75% { transform: translateX(5px); }
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Responsive */
@media (max-width: 768px) {
  .header-controls {
    flex-direction: column;
    align-items: stretch;
  }
  
  .kpi-cards {
    grid-template-columns: 1fr;
  }
  
  .kpi-card {
    text-align: center;
  }
  
  .card-icon {
    margin-right: 0;
    margin-bottom: 8px;
  }
  
  .trend-controls {
    flex-direction: column;
  }
  
  .chart-container {
    height: 150px;
    padding: 10px 0;
  }
  
  .point-bar {
    height: 80px;
  }
  
  .action-section {
    flex-direction: column;
  }
}
</style>
