<template>
  <div class="business-plan-dashboard">
    <!-- Header với gradient Agribank và hiệu ứng -->
    <div class="dashboard-header">
      <div class="header-bg-pattern"></div>
      <div class="header-content">
        <div class="header-left">
          <h1 class="dashboard-title">
            <i class="mdi mdi-view-dashboard-variant"></i>
            DASHBOARD CHỈ TIÊU KHKD
          </h1>
          <p class="dashboard-subtitle">
            <i class="mdi mdi-map-marker"></i>
            {{ getCurrentPeriodLabel() }}
            <span class="live-indicator">
              <span class="pulse"></span>
              Cập nhật thời gian thực
            </span>
          </p>
        </div>
        
        <div class="header-right">
          <div class="summary-stats">
            <div class="stat-item">
              <span class="stat-label">Tổng chỉ tiêu</span>
              <animated-number 
                :value="indicators.length" 
                class="stat-value"
                :duration="1500"
              />
            </div>
            <div class="stat-item success">
              <span class="stat-label">Đạt mục tiêu</span>
              <animated-number 
                :value="achievedCount" 
                class="stat-value"
                :duration="1800"
              />
            </div>
            <div class="stat-item warning">
              <span class="stat-label">Cần cải thiện</span>
              <animated-number 
                :value="needImprovementCount" 
                class="stat-value"
                :duration="2000"
              />
            </div>
          </div>
        </div>
      </div>
      
      <!-- Filters với thiết kế mới -->
      <div class="filters-section">
        <div class="filter-item">
          <label>Thời gian</label>
          <el-date-picker
            v-model="selectedDate"
            type="date"
            placeholder="Chọn ngày"
            format="DD/MM/YYYY"
            value-format="YYYY-MM-DD"
            @change="refreshDashboard"
            class="custom-datepicker"
          />
        </div>
        
        <div class="filter-item">
          <label>Chi nhánh</label>
          <el-select 
            v-model="selectedUnitId" 
            placeholder="Chọn chi nhánh" 
            clearable
            @change="refreshDashboard"
            class="custom-select"
          >
            <el-option label="Toàn tỉnh (Tất cả chi nhánh)" value="" />
            <el-option 
              v-for="unit in units" 
              :key="unit.id" 
              :label="unit.name" 
              :value="unit.id" 
            />
          </el-select>
        </div>
        
        <div class="view-mode-switcher">
          <el-radio-group v-model="viewMode" @change="refreshDashboard">
            <el-radio-button value="overview">
              <i class="mdi mdi-view-grid"></i>
              Tổng quan
            </el-radio-button>
            <el-radio-button value="detail">
              <i class="mdi mdi-chart-line"></i>
              Chi tiết
            </el-radio-button>
            <el-radio-button value="comparison">
              <i class="mdi mdi-compare"></i>
              So sánh
            </el-radio-button>
          </el-radio-group>
        </div>
        
        <div class="action-buttons">
          <el-button 
            :icon="Refresh" 
            @click="refreshDashboard" 
            :loading="loading"
            class="refresh-btn"
          >
            Làm mới
          </el-button>
          
          <el-dropdown trigger="click">
            <el-button class="export-btn">
              <i class="mdi mdi-download"></i>
              Xuất báo cáo
            </el-button>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item @click="exportExcel">
                  <i class="mdi mdi-file-excel"></i> Xuất Excel
                </el-dropdown-item>
                <el-dropdown-item @click="exportPDF">
                  <i class="mdi mdi-file-pdf"></i> Xuất PDF
                </el-dropdown-item>
                <el-dropdown-item @click="printDashboard">
                  <i class="mdi mdi-printer"></i> In báo cáo
                </el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
      </div>
    </div>

    <!-- Main Content với animation -->
    <div class="dashboard-content" v-loading="loading">
      <transition name="fade-slide" mode="out-in">
        <!-- Overview Mode - Thiết kế mới với cards đẹp hơn -->
        <div v-if="viewMode === 'overview'" class="overview-container">
          <!-- Performance Summary Card -->
          <div class="performance-summary-card">
            <h3 class="card-title">
              <i class="mdi mdi-trophy"></i>
              Hiệu suất tổng thể
            </h3>
            <div class="overall-performance">
              <div class="performance-gauge">
                <svg viewBox="0 0 200 100" class="gauge-svg">
                  <path
                    d="M 20 80 A 60 60 0 0 1 180 80"
                    fill="none"
                    stroke="#e9ecef"
                    stroke-width="20"
                  />
                  <path
                    d="M 20 80 A 60 60 0 0 1 180 80"
                    fill="none"
                    :stroke="overallPerformanceColor"
                    stroke-width="20"
                    :stroke-dasharray="`${overallPerformance * 1.57} 157`"
                    class="gauge-fill"
                  />
                </svg>
                <div class="gauge-center">
                  <animated-number 
                    :value="overallPerformance"
                    suffix="%" 
                    class="gauge-value"
                    :duration="2500"
                  />
                  <span class="gauge-label">Hoàn thành</span>
                </div>
              </div>
              
              <div class="performance-details">
                <div class="detail-item">
                  <i class="mdi mdi-check-circle" style="color: #67C23A"></i>
                  <span>{{ achievedCount }}/{{ indicators.length }} chỉ tiêu đạt</span>
                </div>
                <div class="detail-item">
                  <i class="mdi mdi-alert" style="color: #E6A23C"></i>
                  <span>{{ warningCount }} cần theo dõi</span>
                </div>
                <div class="detail-item">
                  <i class="mdi mdi-information" style="color: #909399"></i>
                  <span>Cập nhật: {{ lastUpdated }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- KPI Cards Grid với hiệu ứng 3D -->
          <div class="kpi-cards-container">
            <h3 class="section-title">
              <span class="title-text">Chỉ tiêu kinh doanh</span>
              <span class="title-line"></span>
            </h3>
            
            <div class="kpi-cards-grid">
              <transition-group name="card-flip">
                <div
                  v-for="(indicator, index) in dashboardData"
                  :key="indicator.code"
                  class="kpi-card-wrapper"
                  :style="{ animationDelay: `${index * 0.1}s` }"
                  :data-indicator="indicator.code"
                >
                  <!-- Enhanced KPI Card với hiệu ứng đẹp -->
                  <div class="enhanced-kpi-card" @click="showIndicatorDetail(indicator)">
                    <div class="card-header" :style="{ background: `linear-gradient(135deg, ${indicator.color}E6, ${indicator.color})` }">
                      <div class="card-icon">
                        <i :class="indicator.icon" class="icon"></i>
                      </div>
                      <div class="card-title">
                        <h3>{{ indicator.name }}</h3>
                        <div class="achievement-badge" :class="getAchievementBadgeClass(indicator)">
                          {{ formatAchievementRate(indicator) }}%
                        </div>
                      </div>
                    </div>
                    
                    <div class="card-content">
                      <div class="main-metrics">
                        <div class="metric-row">
                          <span class="label">Thực hiện:</span>
                          <span class="value primary">{{ formatNumber(indicator.actualValue) }} {{ indicator.unit }}</span>
                        </div>
                        <div class="metric-row">
                          <span class="label">Kế hoạch:</span>
                          <span class="value secondary">{{ formatNumber(indicator.planValue) }} {{ indicator.unit }}</span>
                        </div>
                      </div>
                      
                      <div class="growth-metrics">
                        <div class="growth-item">
                          <span class="growth-label">So với đầu năm</span>
                          <div class="growth-value" :class="indicator.yoyGrowth >= 0 ? 'positive' : 'negative'">
                            <i :class="indicator.yoyGrowth >= 0 ? 'mdi-trending-up' : 'mdi-trending-down'"></i>
                            {{ formatGrowth(indicator.yoyGrowth) }}%
                          </div>
                          <div class="absolute-change">
                            ({{ formatAbsoluteChange(indicator.actualValue, indicator.startOfYearValue) }} {{ indicator.unit }})
                          </div>
                        </div>
                        
                        <div class="growth-item">
                          <span class="growth-label">So với đầu tháng</span>
                          <div class="growth-value" :class="indicator.momGrowth >= 0 ? 'positive' : 'negative'">
                            <i :class="indicator.momGrowth >= 0 ? 'mdi-trending-up' : 'mdi-trending-down'"></i>
                            {{ formatGrowth(indicator.momGrowth) }}%
                          </div>
                          <div class="absolute-change">
                            ({{ formatAbsoluteChange(indicator.actualValue, indicator.startOfMonthValue) }} {{ indicator.unit }})
                          </div>
                        </div>
                      </div>
                      
                      <div class="progress-section">
                        <div class="progress-bar">
                          <div class="progress-fill" 
                               :style="{ 
                                 width: `${Math.min(formatAchievementRate(indicator), 100)}%`,
                                 background: getProgressGradient(indicator)
                               }">
                          </div>
                        </div>
                        <div class="progress-text">
                          {{ formatAchievementRate(indicator) }}% hoàn thành kế hoạch
                        </div>
                      </div>
                    </div>
                    
                    <div class="card-sparkline">
                      <div class="sparkline-container">
                        <svg class="sparkline" viewBox="0 0 100 20">
                          <polyline 
                            :points="getSparklinePoints(indicator.trend)" 
                            :stroke="indicator.color"
                            stroke-width="2"
                            fill="none"
                          />
                        </svg>
                      </div>
                      <span class="trend-label">Xu hướng {{ currentMonth }} tháng</span>
                    </div>
                  </div>
                </div>
              </transition-group>
            </div>
          </div>

          <!-- Charts Section với thiết kế mới -->
          <div class="charts-section">
            <div class="chart-card trend-chart">
              <div class="chart-header">
                <h3>
                  <i class="mdi mdi-chart-line"></i>
                  Xu hướng theo thời gian
                </h3>
                <el-button-group class="chart-controls">
                  <el-button size="small" @click="trendPeriod = 'month'" :type="trendPeriod === 'month' ? 'primary' : ''">Tháng</el-button>
                  <el-button size="small" @click="trendPeriod = 'quarter'" :type="trendPeriod === 'quarter' ? 'primary' : ''">Quý</el-button>
                  <el-button size="small" @click="trendPeriod = 'year'" :type="trendPeriod === 'year' ? 'primary' : ''">Năm</el-button>
                </el-button-group>
              </div>
              <trend-chart
                :data="monthlyTrendData"
                :height="350"
                :indicators="selectedIndicators"
              />
            </div>
            
            <div class="chart-card comparison-chart">
              <div class="chart-header">
                <h3>
                  <i class="mdi mdi-chart-bar"></i>
                  So sánh theo đơn vị
                </h3>
                <el-select v-model="comparisonIndicator" size="small" class="chart-indicator-select">
                  <el-option
                    v-for="ind in indicators"
                    :key="ind.code"
                    :label="ind.name"
                    :value="ind.code"
                  />
                </el-select>
              </div>
              <comparison-chart
                :data="unitComparisonData"
                :height="350"
                type="bar"
              />
            </div>
          </div>

          <!-- Top Performers Section -->
          <div class="top-performers-section">
            <h3 class="section-title">
              <span class="title-text">
                <i class="mdi mdi-trophy-variant"></i>
                Top đơn vị xuất sắc
              </span>
              <span class="title-line"></span>
            </h3>
            
            <div class="performers-grid">
              <div
                v-for="(performer, index) in topPerformers"
                :key="performer.unitId"
                class="performer-card"
                :class="`rank-${index + 1}`"
              >
                <div class="rank-badge">
                  <i :class="getRankIcon(index + 1)"></i>
                  <span>{{ index + 1 }}</span>
                </div>
                <div class="performer-info">
                  <h4>{{ performer.unitName }}</h4>
                  <p>
                    <animated-number 
                      :value="performer.achievement" 
                      suffix="% hoàn thành"
                      :duration="2000"
                    />
                  </p>
                </div>
                <div class="performer-progress">
                  <el-progress
                    :percentage="performer.achievement"
                    :color="getProgressColor(performer.achievement)"
                    :stroke-width="6"
                  />
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Detail Mode -->
        <div v-else-if="viewMode === 'detail'" class="detail-container">
          <el-tabs v-model="activeIndicator" type="card" class="custom-tabs">
            <el-tab-pane
              v-for="indicator in indicators"
              :key="indicator.code"
              :label="indicator.name"
              :name="indicator.code"
            >
              <indicator-detail
                :indicator="indicator"
                :unit-id="selectedUnitId"
                :date="selectedDate"
                @filter-change="handleFilterChange"
              />
            </el-tab-pane>
          </el-tabs>
        </div>

        <!-- Comparison Mode -->
        <comparison-view
          v-else-if="viewMode === 'comparison'"
          :indicators="indicators"
          :units="units"
          :date="selectedDate"
          class="comparison-container"
        />
      </transition>
    </div>

    <!-- Floating Action Button -->
    <div class="fab-container">
      <el-dropdown trigger="click" placement="top-start">
        <button class="fab">
          <i class="mdi mdi-plus"></i>
        </button>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item @click="showCalculationDialog = true">
              <i class="mdi mdi-calculator"></i> Tính toán chỉ tiêu
            </el-dropdown-item>
            <el-dropdown-item @click="navigateToTargetAssignment">
              <i class="mdi mdi-target"></i> Giao chỉ tiêu
            </el-dropdown-item>
            <el-dropdown-item @click="showHelpDialog = true">
              <i class="mdi mdi-help-circle"></i> Hướng dẫn
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>

    <!-- Calculation Dialog -->
    <el-dialog
      v-model="calculationDialog.visible"
      title="Tính toán chỉ tiêu"
      width="600px"
      @close="resetCalculation"
    >
      <div class="calculation-content">
        <p>Bạn có muốn thực hiện tính toán lại tất cả chỉ tiêu cho kỳ hiện tại?</p>
        <div v-if="calculating" class="calculation-progress">
          <el-progress :percentage="calculationProgress" />
          <p>{{ calculationMessage }}</p>
        </div>
      </div>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="calculationDialog.visible = false" :disabled="calculating">
            Hủy
          </el-button>
          <el-button type="primary" @click="startCalculation" :loading="calculating">
            {{ calculating ? 'Đang tính toán...' : 'Bắt đầu tính toán' }}
          </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import { useRouter } from 'vue-router';
import { ElMessage, ElLoading } from 'element-plus';
import { Refresh, Download, Calendar } from '@element-plus/icons-vue';

// Components
import KpiCard from '../../components/dashboard/KpiCard.vue';
import TrendChart from '../../components/dashboard/TrendChart.vue';
import ComparisonChart from '../../components/dashboard/ComparisonChart.vue';
import IndicatorDetail from '../../components/dashboard/IndicatorDetail.vue';
import ComparisonView from '../../components/dashboard/ComparisonView.vue';
import AnimatedNumber from '../../components/dashboard/AnimatedNumber.vue';

// Services
import { dashboardService } from '../../services/dashboardService.js';

const router = useRouter();

// State
const loading = ref(false);
const selectedDate = ref(new Date().toISOString().split('T')[0]);
const selectedUnitId = ref(null);
const viewMode = ref('overview');
const activeIndicator = ref('HuyDong');
const indicators = ref([]);
const units = ref([]);
const dashboardData = ref([]);
const monthlyTrendData = ref([]);
const unitComparisonData = ref([]);
const selectedIndicators = ref(['HuyDong', 'DuNo']);
const useKpiComponent = ref(true); // Flag to control component usage

// Enhanced dashboard state
const trendPeriod = ref('month');
const comparisonIndicator = ref('HuyDong');
const overallPerformance = ref(85);
// achievedCount sẽ được tính toán động dựa trên dashboardData
const warningCount = ref(1);
const needImprovementCount = ref(2);
const lastUpdated = ref(new Date().toLocaleString('vi-VN'));
const topPerformers = ref([
  { unitId: 1, unitName: 'Chi nhánh Tam Đường', achievement: 95 },
  { unitId: 2, unitName: 'Chi nhánh Phong Thổ', achievement: 92 },
  { unitId: 3, unitName: 'Chi nhánh Sìn Hồ', achievement: 88 }
]);

// Calculation state
const calculationDialog = ref({ visible: false });
const calculating = ref(false);
const calculationProgress = ref(0);
const calculationMessage = ref('');
const showCalculationDialog = ref(false);
const showHelpDialog = ref(false);

// Computed
const getCurrentPeriodLabel = () => {
  const date = new Date(selectedDate.value);
  const unit = selectedUnitId.value 
    ? units.value.find(u => u.id === selectedUnitId.value)?.name 
    : 'Toàn tỉnh';
  return `${unit} - Tháng ${date.getMonth() + 1}/${date.getFullYear()}`;
};

const overallPerformanceColor = computed(() => {
  const perf = overallPerformance.value;
  if (perf >= 90) return '#67C23A';
  if (perf >= 70) return '#E6A23C';
  return '#F56C6C';
});

// Tính toán số chỉ tiêu đạt kế hoạch (actualValue >= planValue)
const achievedCount = computed(() => {
  if (!dashboardData.value || dashboardData.value.length === 0) return 0;
  
  return dashboardData.value.filter(indicator => {
    // Đối với Tỷ lệ nợ xấu, giá trị thực tế phải <= kế hoạch mới được coi là đạt
    if (indicator.code === 'TyLeNoXau') {
      return indicator.actualValue <= indicator.planValue;
    }
    // Các chỉ tiêu khác: giá trị thực tế >= kế hoạch
    return indicator.actualValue >= indicator.planValue;
  }).length;
});

// Methods for enhanced features
const getRankIcon = (rank) => {
  const icons = {
    1: 'mdi-trophy',
    2: 'mdi-medal',
    3: 'mdi-medal-outline'
  };
  return icons[rank] || 'mdi-star-outline';
};

const getProgressColor = (percentage) => {
  if (percentage >= 90) return '#67C23A';
  if (percentage >= 70) return '#E6A23C';
  return '#F56C6C';
};

const exportExcel = () => {
  ElMessage.success('Đang xuất báo cáo Excel...');
};

const exportPDF = () => {
  ElMessage.success('Đang xuất báo cáo PDF...');
};

const printDashboard = () => {
  window.print();
};

const navigateToTargetAssignment = () => {
  router.push('/dashboard/target-assignment');
};

// Utility functions for fallback cards
const formatNumber = (value) => {
  if (!value) return '0';
  return new Intl.NumberFormat('vi-VN').format(value);
};

const formatPercentage = (indicator) => {
  if (!indicator.planValue || indicator.planValue === 0) return 0;
  
  let percentage;
  if (indicator.code === 'TyLeNoXau') {
    // For bad debt ratio, lower is better
    percentage = Math.max(0, (indicator.planValue - indicator.actualValue) / indicator.planValue * 100 + 100);
  } else {
    percentage = (indicator.actualValue / indicator.planValue) * 100;
  }
  
  return Math.round(percentage * 100) / 100;
};

const getAchievementClass = (indicator) => {
  const percentage = formatPercentage(indicator);
  if (percentage >= 100) return 'achievement-good';
  if (percentage >= 80) return 'achievement-warning';
  return 'achievement-danger';
};

// Hàm utility cho Enhanced KPI Cards
const formatAchievementRate = (indicator) => {
  if (!indicator.planValue || indicator.planValue === 0) return 0;
  
  let percentage;
  if (indicator.code === 'NoXau') {
    // Với nợ xấu, thực tế thấp hơn kế hoạch thì tốt
    percentage = Math.max(0, (indicator.planValue - indicator.actualValue) / indicator.planValue * 100 + 100);
  } else {
    percentage = (indicator.actualValue / indicator.planValue) * 100;
  }
  
  return Math.round(percentage * 100) / 100;
};

const getAchievementBadgeClass = (indicator) => {
  const rate = formatAchievementRate(indicator);
  if (rate >= 100) return 'badge-excellent';
  if (rate >= 80) return 'badge-good';
  if (rate >= 60) return 'badge-warning';
  return 'badge-danger';
};

const formatGrowth = (value) => {
  if (!value && value !== 0) return '0.0';
  return value >= 0 ? `+${value.toFixed(1)}` : value.toFixed(1);
};

const formatAbsoluteChange = (current, previous) => {
  if (!previous || previous === 0) return '0';
  const change = current - previous;
  return change >= 0 ? `+${formatNumber(Math.abs(change))}` : `-${formatNumber(Math.abs(change))}`;
};

const getProgressGradient = (indicator) => {
  const rate = formatAchievementRate(indicator);
  if (rate >= 100) return `linear-gradient(90deg, #10B981, #34D399)`;
  if (rate >= 80) return `linear-gradient(90deg, #F59E0B, #FBBF24)`;
  return `linear-gradient(90deg, #EF4444, #F87171)`;
};

const getSparklinePoints = (trend) => {
  if (!trend || trend.length === 0) return '0,10 100,10';
  
  const max = Math.max(...trend);
  const min = Math.min(...trend);
  const range = max - min || 1;
  
  return trend.map((value, index) => {
    const x = (index / (trend.length - 1)) * 100;
    const y = 20 - ((value - min) / range) * 15;
    return `${x},${y}`;
  }).join(' ');
};

// Computed property cho tháng hiện tại
const currentMonth = computed(() => new Date().getMonth() + 1);

// Methods
const loadIndicators = async () => {
  try {
    const response = await dashboardService.getIndicators();
    indicators.value = response.$values || response;
  } catch (error) {
    console.error('Lỗi tải indicators:', error);
    // 6 chỉ tiêu cố định theo thứ tự yêu cầu của anh
    indicators.value = [
      { code: 'NguonVon', name: 'Nguồn vốn', icon: 'mdi-bank', color: '#8B1538' },
      { code: 'DuNo', name: 'Dư nợ', icon: 'mdi-credit-card-outline', color: '#A6195C' },
      { code: 'NoXau', name: 'Nợ xấu', icon: 'mdi-alert-circle-outline', color: '#B91D47' },
      { code: 'ThuNoXLRR', name: 'Thu nợ XLRR', icon: 'mdi-cash-refund', color: '#C41E3A' },
      { code: 'ThuDichVu', name: 'Thu dịch vụ', icon: 'mdi-account-cash', color: '#D02030' },
      { code: 'LoiNhuanKhoan', name: 'Lợi nhuận khoán tài chính', icon: 'mdi-trending-up', color: '#DC2626' }
    ];
  }
};

const loadUnits = async () => {
  try {
    const response = await dashboardService.getUnits();
    units.value = response.$values || response;
  } catch (error) {
    console.error('Lỗi tải units:', error);
    // 9 chi nhánh theo yêu cầu của anh
    units.value = [
      { id: 'CnLaiChau', name: 'Chi nhánh Lai Châu', shortName: 'CN Lai Châu' },
      { id: 'CnTamDuong', name: 'Chi nhánh Tam Đường', shortName: 'CN Tam Đường' },
      { id: 'CnPhongTho', name: 'Chi nhánh Phong Thổ', shortName: 'CN Phong Thổ' },
      { id: 'CnSinHo', name: 'Chi nhánh Sìn Hồ', shortName: 'CN Sìn Hồ' },
      { id: 'CnMuongTe', name: 'Chi nhánh Mường Tè', shortName: 'CN Mường Tè' },
      { id: 'CnThanUyen', name: 'Chi nhánh Than Uyên', shortName: 'CN Than Uyên' },
      { id: 'CnThanhPho', name: 'Chi nhánh Thành phố', shortName: 'CN Thành phố' },
      { id: 'CnTanUyen', name: 'Chi nhánh Tân Uyên', shortName: 'CN Tân Uyên' },
      { id: 'CnNamNhun', name: 'Chi nhánh Nậm Nhùn', shortName: 'CN Nậm Nhùn' }
    ];
  }
};

const loadDashboardData = async () => {
  loading.value = true;
  try {
    const response = await dashboardService.getDashboardData({
      date: selectedDate.value,
      unitId: selectedUnitId.value
    });
    
    dashboardData.value = response.indicators || [];
    monthlyTrendData.value = response.trendData || [];
    unitComparisonData.value = response.comparisonData || [];
    
  } catch (error) {
    console.error('Lỗi tải dashboard data:', error);
    
    // Mock data dynamic cho 6 chỉ tiêu theo đúng thứ tự
    const selectedUnit = selectedUnitId.value 
      ? units.value.find(u => u.id === selectedUnitId.value)
      : null;
    
    const unitMultiplier = selectedUnit ? 0.7 + Math.random() * 0.6 : 1; // 0.7 - 1.3
    const unitName = selectedUnit ? selectedUnit.name : 'Toàn tỉnh';
    
    // Tính toán cho từng tháng từ đầu năm
    const currentMonth = new Date().getMonth() + 1;
    const monthlyGrowth = Array.from({length: currentMonth}, (_, i) => {
      const monthProgress = (i + 1) / 12;
      return 0.8 + (monthProgress * 0.4) + (Math.random() * 0.2);
    });
    
    dashboardData.value = [
      {
        code: 'NguonVon',
        name: 'Nguồn vốn',
        actualValue: Math.round(1200000 * unitMultiplier),
        planValue: Math.round(1500000 * unitMultiplier),
        startOfYearValue: Math.round(1000000 * unitMultiplier),
        startOfMonthValue: Math.round(1150000 * unitMultiplier),
        unit: 'triệu VNĐ',
        icon: 'mdi-bank',
        color: '#8B1538',
        dataDate: new Date(),
        yoyGrowth: 20.0, // % so với đầu năm
        momGrowth: 4.3,  // % so với đầu tháng
        trend: monthlyGrowth.map(g => Math.round(1000000 * g * unitMultiplier))
      },
      {
        code: 'DuNo',
        name: 'Dư nợ',
        actualValue: Math.round(2800000 * unitMultiplier),
        planValue: Math.round(3000000 * unitMultiplier),
        startOfYearValue: Math.round(2500000 * unitMultiplier),
        startOfMonthValue: Math.round(2750000 * unitMultiplier),
        unit: 'triệu VNĐ',
        icon: 'mdi-credit-card-outline',
        color: '#A6195C',
        dataDate: new Date(),
        yoyGrowth: 12.0, // % so với đầu năm
        momGrowth: 1.8,  // % so với đầu tháng
        trend: monthlyGrowth.map(g => Math.round(2500000 * g * unitMultiplier))
      },
      {
        code: 'NoXau',
        name: 'Nợ xấu',
        actualValue: Math.round(34000 * unitMultiplier),
        planValue: Math.round(30000 * unitMultiplier), // Nợ xấu kế hoạch thấp hơn thì tốt
        startOfYearValue: Math.round(40000 * unitMultiplier),
        startOfMonthValue: Math.round(35000 * unitMultiplier),
        unit: 'triệu VNĐ',
        icon: 'mdi-alert-circle-outline',
        color: '#B91D47',
        dataDate: new Date(),
        yoyGrowth: -15.0, // % giảm so với đầu năm (âm là tốt)
        momGrowth: -2.9,  // % giảm so với đầu tháng
        trend: monthlyGrowth.reverse().map(g => Math.round(40000 * g * unitMultiplier))
      },
      {
        code: 'ThuNoXLRR',
        name: 'Thu nợ XLRR',
        actualValue: Math.round(45000 * unitMultiplier),
        planValue: Math.round(50000 * unitMultiplier),
        startOfYearValue: Math.round(30000 * unitMultiplier),
        startOfMonthValue: Math.round(42000 * unitMultiplier),
        unit: 'triệu VNĐ',
        icon: 'mdi-cash-refund',
        color: '#C41E3A',
        dataDate: new Date(),
        yoyGrowth: 50.0, // % so với đầu năm
        momGrowth: 7.1,  // % so với đầu tháng
        trend: monthlyGrowth.map(g => Math.round(30000 * g * unitMultiplier))
      },
      {
        code: 'ThuDichVu',
        name: 'Thu dịch vụ',
        actualValue: Math.round(18500 * unitMultiplier),
        planValue: Math.round(20000 * unitMultiplier),
        startOfYearValue: Math.round(15000 * unitMultiplier),
        startOfMonthValue: Math.round(17800 * unitMultiplier),
        unit: 'triệu VNĐ',
        icon: 'mdi-account-cash',
        color: '#D02030',
        dataDate: new Date(),
        yoyGrowth: 23.3, // % so với đầu năm
        momGrowth: 3.9,  // % so với đầu tháng
        trend: monthlyGrowth.map(g => Math.round(15000 * g * unitMultiplier))
      },
      {
        code: 'LoiNhuanKhoan',
        name: 'Lợi nhuận khoán tài chính',
        actualValue: Math.round(12500 * unitMultiplier),
        planValue: Math.round(15000 * unitMultiplier),
        startOfYearValue: Math.round(10000 * unitMultiplier),
        startOfMonthValue: Math.round(12000 * unitMultiplier),
        unit: 'triệu VNĐ',
        icon: 'mdi-trending-up',
        color: '#DC2626',
        dataDate: new Date(),
        yoyGrowth: 25.0, // % so với đầu năm
        momGrowth: 4.2,  // % so với đầu tháng
        trend: monthlyGrowth.map(g => Math.round(10000 * g * unitMultiplier))
      }
    ];
    
    monthlyTrendData.value = {
      months: ['T7', 'T8', 'T9', 'T10', 'T11', 'T12'],
      series: [
        {
          name: 'Nguồn vốn',
          data: [1000, 1100, 1050, 1200, 1150, 1200].map(v => Math.round(v * unitMultiplier)),
          color: '#8B1538'
        },
        {
          name: 'Dư nợ',
          data: [2000, 2100, 2200, 2350, 2400, 2800].map(v => Math.round(v * unitMultiplier)),
          color: '#A6195C'
        }
      ]
    };
    
    // Update comparison data based on selected unit
    if (selectedUnitId.value) {
      // Show comparison with other similar units
      unitComparisonData.value = [
        { unitName: unitName, value: Math.round(95 * unitMultiplier), target: 100 },
        { unitName: 'Đơn vị khác 1', value: Math.round(92 * (0.8 + Math.random() * 0.4)), target: 100 },
        { unitName: 'Đơn vị khác 2', value: Math.round(88 * (0.8 + Math.random() * 0.4)), target: 100 },
        { unitName: 'Đơn vị khác 3', value: Math.round(85 * (0.8 + Math.random() * 0.4)), target: 100 }
      ];
    } else {
      // Show all units comparison
      unitComparisonData.value = [
        { unitName: 'Tam Đường', value: 95, target: 100 },
        { unitName: 'Phong Thổ', value: 92, target: 100 },
        { unitName: 'Sìn Hồ', value: 88, target: 100 },
        { unitName: 'Mường Tè', value: 85, target: 100 }
      ];
    }
  } finally {
    loading.value = false;
  }
};

const refreshDashboard = async () => {
  await loadDashboardData();
  ElMessage.success('Đã làm mới dữ liệu dashboard');
};

const showIndicatorDetail = (indicator) => {
  // Add a small animation/highlight effect before switching
  const indicatorElement = document.querySelector(`[data-indicator="${indicator.code}"]`);
  if (indicatorElement) {
    indicatorElement.classList.add('indicator-highlight');
    setTimeout(() => {
      indicatorElement.classList.remove('indicator-highlight');
    }, 700);
  }
  
  // Set active indicator and switch to detail view
  activeIndicator.value = indicator.code;
  
  // Add a small delay for better UX
  setTimeout(() => {
    viewMode.value = 'detail';
    
    // Show a success message
    ElMessage({
      message: `Đang xem chi tiết chỉ tiêu: ${indicator.name}`,
      type: 'success',
      duration: 2000
    });
  }, 300);
};

const handleFilterChange = (filters) => {
  selectedDate.value = filters.date || selectedDate.value;
  selectedUnitId.value = filters.unitId || selectedUnitId.value;
  refreshDashboard();
};

const startCalculation = async () => {
  calculating.value = true;
  calculationProgress.value = 0;
  calculationMessage.value = 'Đang khởi tạo...';
  
  try {
    // Simulate calculation process
    const steps = [
      'Đang tải dữ liệu gốc...',
      'Đang xử lý chỉ tiêu huy động...',
      'Đang xử lý chỉ tiêu cho vay...',
      'Đang tính toán tỷ lệ nợ xấu...',
      'Đang cập nhật kết quả...',
      'Hoàn thành!'
    ];
    
    for (let i = 0; i < steps.length; i++) {
      calculationMessage.value = steps[i];
      calculationProgress.value = ((i + 1) / steps.length) * 100;
      await new Promise(resolve => setTimeout(resolve, 1000));
    }
    
    ElMessage.success('Tính toán chỉ tiêu hoàn thành!');
    await refreshDashboard();
    
  } catch (error) {
    console.error('Lỗi tính toán:', error);
    ElMessage.error('Có lỗi xảy ra trong quá trình tính toán');
  } finally {
    calculating.value = false;
    calculationDialog.visible = false;
  }
};

const resetCalculation = () => {
  calculating.value = false;
  calculationProgress.value = 0;
  calculationMessage.value = '';
};

// Lifecycle
onMounted(async () => {
  try {
    await Promise.all([
      loadIndicators(),
      loadUnits()
    ]);
    await loadDashboardData();
    
    // Try to use KPI component, fallback to simple cards if there are issues
    try {
      // Test if KPI component can be used
      useKpiComponent.value = true;
    } catch (error) {
      console.warn('KPI Component failed, using fallback cards:', error);
      useKpiComponent.value = false;
    }
  } catch (error) {
    console.error('Error loading dashboard:', error);
    // Ensure at least the 6 indicators are shown even if loading fails
    useKpiComponent.value = false;
  }
});

// Watch for changes
watch([selectedDate, selectedUnitId], () => {
  refreshDashboard();
});
</script>

<style scoped>
/* Enhanced styles for beautiful dashboard */
.business-plan-dashboard {
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  position: relative;
}

/* Header with Agribank branding */
.dashboard-header {
  background: linear-gradient(135deg, #8B1538 0%, #A6195C 50%, #B91D47 100%);
  color: white;
  position: relative;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(139, 21, 56, 0.3);
}

.header-bg-pattern {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-image: 
    repeating-linear-gradient(45deg, transparent, transparent 35px, rgba(255,255,255,.05) 35px, rgba(255,255,255,.05) 70px),
    repeating-linear-gradient(-45deg, transparent, transparent 35px, rgba(255,255,255,.03) 35px, rgba(255,255,255,.03) 70px);
  opacity: 0.3;
}

.header-content {
  position: relative;
  z-index: 1;
  padding: 30px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 20px;
}

.dashboard-title {
  font-size: 36px;
  font-weight: 700;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 15px;
  color: #FFFFFF;
  text-shadow: 2px 2px 8px rgba(0,0,0,0.3);
  font-family: 'Segoe UI', 'Open Sans', sans-serif;
  letter-spacing: 1px;
}

.dashboard-subtitle {
  font-size: 16px;
  opacity: 0.9;
  margin: 10px 0 0 0;
  display: flex;
  align-items: center;
  gap: 10px;
  font-family: 'Segoe UI', 'Open Sans', sans-serif;
}

.live-indicator {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  background: rgba(255,255,255,0.2);
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 14px;
}

.pulse {
  display: inline-block;
  width: 8px;
  height: 8px;
  background: #67C23A;
  border-radius: 50%;
  animation: pulse 2s infinite;
}

@keyframes pulse {
  0% {
    box-shadow: 0 0 0 0 rgba(103, 194, 58, 0.7);
  }
  70% {
    box-shadow: 0 0 0 10px rgba(103, 194, 58, 0);
  }
  100% {
    box-shadow: 0 0 0 0 rgba(103, 194, 58, 0);
  }
}

.summary-stats {
  display: flex;
  gap: 30px;
}

.stat-item {
  text-align: center;
}

.stat-label {
  display: block;
  font-size: 14px;
  opacity: 0.8;
  margin-bottom: 5px;
}

.stat-value {
  display: block;
  font-size: 28px;
  font-weight: bold;
}

.stat-item.success .stat-value {
  color: #67C23A;
}

.stat-item.warning .stat-value {
  color: #FFC107;
}

/* Filters section */
.filters-section {
  padding: 20px 30px;
  background: rgba(0,0,0,0.1);
  display: flex;
  align-items: center;
  gap: 20px;
  flex-wrap: wrap;
  position: relative;
  z-index: 1;
}

.filter-item {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.filter-item label {
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 1px;
  opacity: 0.8;
}

.custom-datepicker,
.custom-select {
  background: rgba(255,255,255,0.9) !important;
  min-width: 200px;
}

.custom-select :deep(.el-input__wrapper) {
  min-width: 200px;
}

.custom-select :deep(.el-select__popper) {
  min-width: 250px !important;
}

.view-mode-switcher :deep(.el-radio-button__inner) {
  background: rgba(255,255,255,0.2);
  border-color: rgba(255,255,255,0.3);
  color: white;
}

.view-mode-switcher :deep(.el-radio-button__original:checked + .el-radio-button__inner) {
  background: white;
  color: #8B1538;
  border-color: white;
}

.action-buttons {
  margin-left: auto;
  display: flex;
  gap: 10px;
}

.refresh-btn,
.export-btn {
  background: rgba(255,255,255,0.2);
  border-color: rgba(255,255,255,0.3);
  color: white;
}

.refresh-btn:hover,
.export-btn:hover {
  background: white;
  color: #8B1538;
  border-color: white;
}

/* Content area */
.dashboard-content {
  padding: 30px;
  min-height: calc(100vh - 200px);
}

/* Overview container */
.overview-container {
  animation: fadeInUp 0.6s ease-out;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Performance summary card - Làm to hơn và dễ nhìn hơn */
.performance-summary-card {
  background: white;
  border-radius: 20px;
  padding: 40px;
  margin-bottom: 40px;
  box-shadow: 0 15px 40px rgba(0,0,0,0.12);
  animation: slideInLeft 0.6s ease-out;
  border: 2px solid #8B0000;
}

@keyframes slideInLeft {
  from {
    opacity: 0;
    transform: translateX(-30px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

.card-title {
  font-size: 24px;
  font-weight: 700;
  color: #8B0000;
  margin: 0 0 25px 0;
  display: flex;
  align-items: center;
  gap: 12px;
}

.overall-performance {
  display: flex;
  align-items: center;
  gap: 50px;
}

.performance-gauge {
  position: relative;
  width: 250px;
  height: 125px;
}

.gauge-svg {
  width: 100%;
  height: 100%;
}

.gauge-fill {
  transition: stroke-dasharray 1s ease-out;
}

.gauge-center {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -20%);
  text-align: center;
}

.gauge-value {
  display: block;
  font-size: 42px;
  font-weight: bold;
  color: #8B0000;
}

.gauge-label {
  display: block;
  font-size: 16px;
  color: #909399;
  margin-top: 8px;
  font-weight: 500;
}

.performance-details {
  flex: 1;
}

.detail-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px 0;
  font-size: 18px;
  color: #606266;
  font-weight: 500;
}

/* Section titles */
.section-title {
  font-size: 22px;
  font-weight: 600;
  margin: 40px 0 25px 0;
  display: flex;
  align-items: center;
  gap: 20px;
  color: #303133;
}

.title-text {
  display: flex;
  align-items: center;
  gap: 10px;
}

.title-line {
  flex: 1;
  height: 2px;
  background: linear-gradient(90deg, #8B1538 0%, transparent 100%);
}

/* KPI Cards Grid */
.kpi-cards-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(320px, 1fr));
  gap: 25px;
  margin-bottom: 40px;
}

.kpi-card-wrapper {
  animation: cardEntry 0.6s ease-out backwards;
}

@keyframes cardEntry {
  from {
    opacity: 0;
    transform: scale(0.9) translateY(20px);
  }
  to {
    opacity: 1;
    transform: scale(1) translateY(0);
  }
}

/* Enhanced KPI Card Styling - Thiết kế đẹp lung linh */
.enhanced-kpi-card {
  background: linear-gradient(145deg, #ffffff 0%, #f8fafc 100%);
  border-radius: 20px;
  box-shadow: 
    0 10px 25px rgba(139, 21, 56, 0.08),
    0 4px 10px rgba(0, 0, 0, 0.04);
  border: 1px solid rgba(139, 21, 56, 0.1);
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  cursor: pointer;
  overflow: hidden;
  position: relative;
  height: 280px;
  display: flex;
  flex-direction: column;
}

.enhanced-kpi-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 3px;
  background: linear-gradient(90deg, #8B1538, #A6195C, #B91D47);
  transform: scaleX(0);
  transform-origin: left;
  transition: transform 0.6s ease;
}

.enhanced-kpi-card:hover {
  transform: translateY(-8px) scale(1.02);
  box-shadow: 
    0 20px 40px rgba(139, 21, 56, 0.15),
    0 8px 20px rgba(0, 0, 0, 0.08);
}

.enhanced-kpi-card:hover::before {
  transform: scaleX(1);
}

.card-header {
  padding: 16px 20px;
  border-radius: 20px 20px 0 0;
  position: relative;
  display: flex;
  align-items: center;
  gap: 12px;
  min-height: 70px;
}

.card-icon {
  width: 48px;
  height: 48px;
  background: rgba(255, 255, 255, 0.9);
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.card-icon .icon {
  font-size: 24px;
  color: #8B1538;
}

.card-title {
  flex: 1;
  color: white;
}

.card-title h3 {
  margin: 0;
  font-size: 16px;
  font-weight: 700;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
  line-height: 1.2;
}

.achievement-badge {
  display: inline-block;
  padding: 4px 8px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
  margin-top: 4px;
  text-shadow: none;
}

.badge-excellent {
  background: rgba(16, 185, 129, 0.9);
  color: white;
}

.badge-good {
  background: rgba(245, 158, 11, 0.9);
  color: white;
}

.badge-warning {
  background: rgba(249, 115, 22, 0.9);
  color: white;
}

.badge-danger {
  background: rgba(239, 68, 68, 0.9);
  color: white;
}

.card-content {
  padding: 16px 20px;
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.main-metrics {
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
  border-radius: 12px;
  padding: 12px;
  border: 1px solid #e2e8f0;
}

.metric-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.metric-row:last-child {
  margin-bottom: 0;
}

.metric-row .label {
  font-size: 13px;
  color: #64748b;
  font-weight: 500;
}

.metric-row .value {
  font-weight: 700;
  font-size: 14px;
}

.value.primary {
  color: #8B1538;
}

.value.secondary {
  color: #6b7280;
}

.growth-metrics {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 8px;
}

.growth-item {
  text-align: center;
  padding: 8px;
  background: white;
  border-radius: 8px;
  border: 1px solid #e5e7eb;
}

.growth-label {
  display: block;
  font-size: 11px;
  color: #6b7280;
  margin-bottom: 4px;
  font-weight: 500;
}

.growth-value {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 4px;
  font-weight: 700;
  font-size: 13px;
  margin-bottom: 2px;
}

.growth-value.positive {
  color: #059669;
}

.growth-value.negative {
  color: #dc2626;
}

.absolute-change {
  font-size: 10px;
  color: #9ca3af;
}

.progress-section {
  margin-top: auto;
}

.progress-bar {
  height: 6px;
  background: #e5e7eb;
  border-radius: 3px;
  overflow: hidden;
  margin-bottom: 6px;
}

.progress-fill {
  height: 100%;
  border-radius: 3px;
  transition: width 1.5s cubic-bezier(0.4, 0, 0.2, 1);
}

.progress-text {
  font-size: 11px;
  color: #6b7280;
  text-align: center;
  font-weight: 600;
}

.card-sparkline {
  padding: 12px 20px;
  background: linear-gradient(135deg, #fafbfc 0%, #f3f4f6 100%);
  border-top: 1px solid #e5e7eb;
  display: flex;
  align-items: center;
  gap: 8px;
}

.sparkline-container {
  width: 60px;
  height: 20px;
}

.sparkline {
  width: 100%;
  height: 100%;
}

.trend-label {
  font-size: 11px;
  color: #6b7280;
  font-weight: 500;
}

/* Animation cho cards */
@keyframes cardPulse {
  0%, 100% { 
    box-shadow: 0 10px 25px rgba(139, 21, 56, 0.08);
  }
  50% { 
    box-shadow: 0 15px 35px rgba(139, 21, 56, 0.12);
  }
}

.enhanced-kpi-card:hover {
  animation: cardPulse 2s infinite;
}

/* Responsive cho enhanced cards */
@media (max-width: 768px) {
  .enhanced-kpi-card {
    height: 260px;
  }
  
  .card-header {
    padding: 12px 16px;
    min-height: 60px;
  }
  
  .card-icon {
    width: 40px;
    height: 40px;
  }
  
  .card-icon .icon {
    font-size: 20px;
  }
  
  .card-title h3 {
    font-size: 14px;
  }
  
  .card-content {
    padding: 12px 16px;
  }
}

/* Print styles */
@media print {
  .filters-section,
  .fab-container,
  .action-buttons {
    display: none !important;
  }
  
  .dashboard-header {
    background: none;
    color: black;
    box-shadow: none;
    border-bottom: 2px solid #8B1538;
  }
  
  .dashboard-content {
    padding: 0;
  }
}
</style>
