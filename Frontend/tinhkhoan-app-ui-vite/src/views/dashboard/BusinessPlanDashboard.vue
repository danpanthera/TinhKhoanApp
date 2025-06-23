<template>
  <div class="general-dashboard">
    <!-- Header với gradient Agribank -->
    <div class="dashboard-header">
      <div class="header-bg-pattern"></div>
      <div class="header-content">
        <div class="header-left">
          <h1 class="dashboard-title">
            <i class="mdi mdi-view-dashboard-variant"></i>
            DASHBOARD TỔNG HỢP
          </h1>
          <p class="dashboard-subtitle">
            <i class="mdi mdi-calendar"></i>
            {{ getCurrentPeriodLabel() }}
          </p>
        </div>
        <div class="header-right">
          <!-- Dropdown chọn chi nhánh -->
          <div class="branch-selector">
            <el-select 
              v-model="selectedBranch" 
              placeholder="Chọn chi nhánh"
              @change="loadDashboardData"
              :loading="loading"
            >
              <el-option
                v-for="branch in branches"
                :key="branch.id"
                :label="branch.name"
                :value="branch.id"
              />
            </el-select>
          </div>
        </div>
      </div>
    </div>

    <!-- 6 Cards chỉ tiêu -->
    <div class="indicators-grid">
      <div 
        v-for="(indicator, index) in indicators" 
        :key="indicator.id"
        class="indicator-card"
        :class="[indicator.class, { 'loading': loading }]"
        :style="{ animationDelay: `${index * 0.1}s` }"
      >
        <div class="card-header">
          <div class="card-icon">{{ indicator.icon }}</div>
          <div class="card-title">{{ indicator.name }}</div>
        </div>
        
        <div class="card-body">
          <!-- Giá trị hiện tại -->
          <div class="current-value">
            <animated-number
              :value="indicator.currentValue"
              :format="indicator.format"
              :duration="1500"
              class="value-number"
            />
            <span class="value-unit">{{ indicator.unit }}</span>
          </div>

          <!-- Biểu đồ tỷ lệ hoàn thành -->
          <div class="progress-chart">
            <el-progress
              type="circle"
              :percentage="indicator.completionRate"
              :width="120"
              :stroke-width="8"
              :color="getProgressColor(indicator.completionRate)"
            >
              <template #default="{ percentage }">
                <div class="progress-content">
                  <span class="percentage">{{ percentage }}%</span>
                  <span class="label">Hoàn thành</span>
                </div>
              </template>
            </el-progress>
          </div>

          <!-- So sánh với đầu năm -->
          <div class="comparison">
            <div class="comparison-item">
              <span class="comparison-label">So với đầu năm:</span>
              <div class="comparison-value">
                <span 
                  class="arrow"
                  :class="indicator.changeFromYearStart >= 0 ? 'up' : 'down'"
                >
                  {{ indicator.changeFromYearStart >= 0 ? '↑' : '↓' }}
                </span>
                <span class="absolute-change">
                  {{ formatNumber(Math.abs(indicator.changeFromYearStart)) }}
                  {{ indicator.unit }}
                </span>
                <span 
                  class="percent-change"
                  :class="indicator.changeFromYearStart >= 0 ? 'positive' : 'negative'"
                >
                  ({{ indicator.changeFromYearStartPercent >= 0 ? '+' : '' }}{{ indicator.changeFromYearStartPercent }}%)
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Footer với chi tiết -->
        <div class="card-footer">
          <div class="footer-item">
            <span class="footer-label">Kế hoạch:</span>
            <span class="footer-value">{{ formatNumber(indicator.targetValue) }} {{ indicator.unit }}</span>
          </div>
          <div class="footer-item">
            <span class="footer-label">Thực hiện:</span>
            <span class="footer-value">{{ formatNumber(indicator.currentValue) }} {{ indicator.unit }}</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading overlay -->
    <div v-if="loading" class="loading-overlay">
      <div class="loading-spinner"></div>
      <p>Đang tải dữ liệu...</p>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import { ElMessage } from 'element-plus';
import AnimatedNumber from '../../components/dashboard/AnimatedNumber.vue';
import { dashboardService } from '../../services/dashboardService.js';

// State
const loading = ref(false);
const selectedBranch = ref(null);

// Danh sách chi nhánh
const branches = ref([
  { id: 'HoiSo', name: 'Hội Sở' },
  { id: 'CnTamDuong', name: 'CN Tam Đường' },
  { id: 'CnPhongTho', name: 'CN Phong Thổ' },
  { id: 'CnSinHo', name: 'CN Sin Hồ' },
  { id: 'CnMuongTe', name: 'CN Mường Tè' },
  { id: 'CnThanUyen', name: 'CN Than Uyên' },
  { id: 'CnThanhpho', name: 'CN Thành phố' },
  { id: 'CnTanUyen', name: 'CN Tân Uyên' },
  { id: 'CnNamNhun', name: 'CN Nậm Nhùn' },
  { id: 'CnPhongThoPgdMuongSo', name: 'CN Phong Thổ - PGD Mường So' },
  { id: 'CnThanUyenPgdMuongThan', name: 'CN Than Uyên - PGD Mường Than' },
  { id: 'CnThanhPhoPgdso1', name: 'CN Thành phố - PGD số 1' },
  { id: 'CnThanhPhoPgdso2', name: 'CN Thành phố - PGD số 2' },
  { id: 'CnTanUyenPgdso3', name: 'CN Tân Uyên - PGD số 3' }
]);

// 6 chỉ tiêu dashboard
const indicators = ref([
  {
    id: 'nguon_von',
    name: 'Nguồn vốn',
    icon: '💰',
    class: 'nguon-von',
    unit: 'tỷ',
    format: 'currency',
    currentValue: 0,
    targetValue: 0,
    completionRate: 0,
    changeFromYearStart: 0,
    changeFromYearStartPercent: 0
  },
  {
    id: 'du_no',
    name: 'Dư nợ',
    icon: '💳',
    class: 'du-no',
    unit: 'tỷ',
    format: 'currency',
    currentValue: 0,
    targetValue: 0,
    completionRate: 0,
    changeFromYearStart: 0,
    changeFromYearStartPercent: 0
  },
  {
    id: 'no_xau',
    name: 'Nợ Xấu',
    icon: '⚠️',
    class: 'no-xau',
    unit: '%',
    format: 'percent',
    currentValue: 0,
    targetValue: 0,
    completionRate: 0,
    changeFromYearStart: 0,
    changeFromYearStartPercent: 0
  },
  {
    id: 'thu_no_xlrr',
    name: 'Thu nợ đã XLRR',
    icon: '📈',
    class: 'thu-no-xlrr',
    unit: 'tỷ',
    format: 'currency',
    currentValue: 0,
    targetValue: 0,
    completionRate: 0,
    changeFromYearStart: 0,
    changeFromYearStartPercent: 0
  },
  {
    id: 'thu_dich_vu',
    name: 'Thu dịch vụ',
    icon: '🏦',
    class: 'thu-dich-vu',
    unit: 'tỷ',
    format: 'currency',
    currentValue: 0,
    targetValue: 0,
    completionRate: 0,
    changeFromYearStart: 0,
    changeFromYearStartPercent: 0
  },
  {
    id: 'tai_chinh',
    name: 'Tài chính',
    icon: '💵',
    class: 'tai-chinh',
    unit: 'tỷ',
    format: 'currency',
    currentValue: 0,
    targetValue: 0,
    completionRate: 0,
    changeFromYearStart: 0,
    changeFromYearStartPercent: 0
  }
]);

// Computed
const getCurrentPeriodLabel = () => {
  const now = new Date();
  return `Tháng ${now.getMonth() + 1}/${now.getFullYear()}`;
};

// Methods
const formatNumber = (value) => {
  if (!value) return '0';
  return new Intl.NumberFormat('vi-VN').format(value);
};

const getProgressColor = (percentage) => {
  if (percentage >= 100) return '#52c41a'; // Xanh lá
  if (percentage >= 80) return '#1890ff'; // Xanh dương
  if (percentage >= 60) return '#faad14'; // Vàng
  return '#f5222d'; // Đỏ
};

const loadDashboardData = async () => {
  loading.value = true;
  try {
    const data = await dashboardService.getGeneralDashboardData(selectedBranch.value);
    
    // Cập nhật dữ liệu từ API
    if (data && data.indicators) {
      indicators.value = data.indicators;
    }
    
    ElMessage.success('Dữ liệu đã được cập nhật');
  } catch (error) {
    console.error('Error loading dashboard data:', error);
    ElMessage.error('Không thể tải dữ liệu dashboard');
  } finally {
    loading.value = false;
  }
};

// Lifecycle
onMounted(() => {
  // Load dữ liệu mặc định cho Hội Sở
  selectedBranch.value = 'HoiSo';
  loadDashboardData();
});

// Watch branch selection
watch(selectedBranch, () => {
  if (selectedBranch.value) {
    loadDashboardData();
  }
});
</script>

<style scoped>
.general-dashboard {
  min-height: 100vh;
  background: #f5f7fa;
  padding-bottom: 40px;
}

/* Header Styles */
.dashboard-header {
  position: relative;
  background: linear-gradient(135deg, #1a5f3f 0%, #2e7d4f 100%);
  color: white;
  padding: 40px 0;
  margin-bottom: 40px;
  overflow: hidden;
}

.header-bg-pattern {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  opacity: 0.1;
  background-image: 
    repeating-linear-gradient(45deg, transparent, transparent 35px, rgba(255,255,255,.1) 35px, rgba(255,255,255,.1) 70px);
}

.header-content {
  position: relative;
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 30px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.dashboard-title {
  font-size: 32px;
  margin: 0 0 10px 0;
  font-weight: 600;
  letter-spacing: 0.5px;
}

.dashboard-subtitle {
  font-size: 16px;
  opacity: 0.9;
}

.branch-selector {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 8px;
  padding: 4px;
}

.branch-selector :deep(.el-select) {
  width: 300px;
}

.branch-selector :deep(.el-input__inner) {
  background: rgba(255, 255, 255, 0.9);
  border: none;
}

/* Grid Layout cho 6 cards */
.indicators-grid {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 30px;
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 30px;
}

/* Card Styles */
.indicator-card {
  background: white;
  border-radius: 16px;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.08);
  padding: 30px;
  transition: all 0.3s ease;
  animation: fadeInUp 0.6s ease-out;
  position: relative;
  overflow: hidden;
}

.indicator-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, var(--card-color) 0%, var(--card-color-light) 100%);
}

.indicator-card.nguon-von { --card-color: #52c41a; --card-color-light: #95de64; }
.indicator-card.du-no { --card-color: #1890ff; --card-color-light: #69c0ff; }
.indicator-card.no-xau { --card-color: #fa541c; --card-color-light: #ff7a45; }
.indicator-card.thu-no-xlrr { --card-color: #722ed1; --card-color-light: #b37feb; }
.indicator-card.thu-dich-vu { --card-color: #13c2c2; --card-color-light: #5cdbd3; }
.indicator-card.tai-chinh { --card-color: #faad14; --card-color-light: #ffc53d; }

.indicator-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
}

.card-header {
  display: flex;
  align-items: center;
  margin-bottom: 24px;
}

.card-icon {
  font-size: 32px;
  margin-right: 12px;
}

.card-title {
  font-size: 20px;
  font-weight: 600;
  color: #333;
}

.card-body {
  display: grid;
  grid-template-columns: 1fr auto;
  gap: 20px;
  align-items: center;
  margin-bottom: 24px;
}

.current-value {
  display: flex;
  align-items: baseline;
  gap: 8px;
}

.value-number {
  font-size: 36px;
  font-weight: 700;
  color: var(--card-color);
}

.value-unit {
  font-size: 18px;
  color: #666;
}

.progress-chart :deep(.el-progress__text) {
  display: none;
}

.progress-content {
  text-align: center;
}

.progress-content .percentage {
  display: block;
  font-size: 24px;
  font-weight: 600;
  color: #333;
}

.progress-content .label {
  display: block;
  font-size: 12px;
  color: #666;
  margin-top: 4px;
}

.comparison {
  grid-column: 1 / -1;
  background: #f5f7fa;
  border-radius: 8px;
  padding: 16px;
}

.comparison-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.comparison-label {
  color: #666;
  font-size: 14px;
}

.comparison-value {
  display: flex;
  align-items: center;
  gap: 8px;
}

.arrow {
  font-size: 20px;
  font-weight: bold;
}

.arrow.up {
  color: #52c41a;
}

.arrow.down {
  color: #f5222d;
}

.absolute-change {
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

.percent-change {
  font-size: 14px;
  font-weight: 500;
}

.percent-change.positive {
  color: #52c41a;
}

.percent-change.negative {
  color: #f5222d;
}

.card-footer {
  display: flex;
  justify-content: space-between;
  padding-top: 20px;
  border-top: 1px solid #f0f0f0;
}

.footer-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.footer-label {
  font-size: 12px;
  color: #999;
}

.footer-value {
  font-size: 16px;
  font-weight: 600;
  color: #333;
}

/* Loading State */
.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(255, 255, 255, 0.9);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.loading-spinner {
  width: 50px;
  height: 50px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #1a5f3f;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
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
  .indicators-grid {
    grid-template-columns: 1fr;
    gap: 20px;
    padding: 0 20px;
  }
  
  .header-content {
    flex-direction: column;
    align-items: flex-start;
    gap: 20px;
  }
  
  .branch-selector :deep(.el-select) {
    width: 100%;
  }
  
  .value-number {
    font-size: 28px;
  }
}
</style>
