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
          <label>Đơn vị</label>
          <el-select 
            v-model="selectedUnitId" 
            placeholder="Chọn đơn vị" 
            clearable
            @change="refreshDashboard"
            class="custom-select"
          >
            <el-option label="Toàn tỉnh" value="" />
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
                >
                  <kpi-card
                    :indicator="indicator"
                    :show-trend="true"
                    @click="showIndicatorDetail(indicator)"
                    class="enhanced-kpi-card"
                  />
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

// Methods
const loadIndicators = async () => {
  try {
    const response = await dashboardService.getIndicators();
    indicators.value = response.$values || response;
  } catch (error) {
    console.error('Lỗi tải indicators:', error);
    // Mock data for demo - 6 chỉ tiêu cố định theo yêu cầu
    indicators.value = [
      { code: 'HuyDong', name: 'Huy động vốn' },
      { code: 'DuNo', name: 'Dư nợ' },
      { code: 'TyLeNoXau', name: 'Tỷ lệ nợ xấu' },
      { code: 'ThuNoXLRR', name: 'Thu nợ đã XLRR' },
      { code: 'ThuDichVu', name: 'Thu dịch vụ' },
      { code: 'LoiNhuanKhoan', name: 'Lợi nhuận khoán tài chính' }
    ];
  }
};

const loadUnits = async () => {
  try {
    const response = await dashboardService.getUnits();
    units.value = response.$values || response;
  } catch (error) {
    console.error('Lỗi tải units:', error);
    // Mock data for demo
    units.value = [
      { id: 1, name: 'Chi nhánh Tam Đường' },
      { id: 2, name: 'Chi nhánh Phong Thổ' },
      { id: 3, name: 'Chi nhánh Sìn Hồ' },
      { id: 4, name: 'Chi nhánh Mường Tè' }
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
    
    // Mock data dynamic based on selected unit - 6 chỉ tiêu cố định
    const selectedUnit = selectedUnitId.value 
      ? units.value.find(u => u.id === selectedUnitId.value)
      : null;
    
    const unitMultiplier = selectedUnit ? 0.7 + Math.random() * 0.6 : 1; // 0.7 - 1.3
    const unitName = selectedUnit ? selectedUnit.name : 'Toàn tỉnh';
    
    dashboardData.value = [
      {
        code: 'HuyDong',
        name: 'Huy động vốn',
        actualValue: Math.round(1200000 * unitMultiplier),
        planValue: Math.round(1500000 * unitMultiplier),
        unit: 'triệu VNĐ',
        icon: 'mdi-hand-heart', // Icon bàn tay ngửa đựng trái tim (tiền)
        color: '#8B1538',
        dataDate: new Date(),
        yoyGrowth: 12.5 * unitMultiplier,
        trend: [100, 110, 105, 120, 115, 125].map(v => Math.round(v * unitMultiplier))
      },
      {
        code: 'DuNo',
        name: 'Dư nợ',
        actualValue: Math.round(2800000 * unitMultiplier),
        planValue: Math.round(3000000 * unitMultiplier),
        unit: 'triệu VNĐ',
        icon: 'mdi-credit-card-outline', // Icon thẻ tín dụng cho dư nợ
        color: '#A6195C',
        dataDate: new Date(),
        yoyGrowth: 8.3 * unitMultiplier,
        trend: [200, 210, 220, 235, 240, 250].map(v => Math.round(v * unitMultiplier))
      },
      {
        code: 'TyLeNoXau',
        name: 'Tỷ lệ nợ xấu',
        actualValue: Math.round((1.2 + (1-unitMultiplier) * 0.8) * 100) / 100,
        planValue: 2.0,
        unit: '%',
        icon: 'mdi-shield-alert', // Icon khiên cảnh báo cho rủi ro nợ xấu
        color: '#B91D47',
        dataDate: new Date(),
        yoyGrowth: -0.5 * unitMultiplier,
        trend: [2.1, 1.9, 1.7, 1.5, 1.3, 1.2].map(v => Math.round((v + (1-unitMultiplier) * 0.5) * 100) / 100)
      },
      {
        code: 'ThuNoXLRR',
        name: 'Thu nợ đã XLRR',
        actualValue: Math.round(45000 * unitMultiplier),
        planValue: Math.round(50000 * unitMultiplier),
        unit: 'triệu VNĐ',
        icon: 'mdi-backup-restore', // Icon khôi phục cho thu nợ đã XLRR
        color: '#E91E63',
        dataDate: new Date(),
        yoyGrowth: 15.2 * unitMultiplier,
        trend: [30, 32, 35, 38, 42, 45].map(v => Math.round(v * unitMultiplier))
      },
      {
        code: 'ThuDichVu',
        name: 'Thu dịch vụ',
        actualValue: Math.round(18500 * unitMultiplier),
        planValue: Math.round(20000 * unitMultiplier),
        unit: 'triệu VNĐ',
        icon: 'mdi-cog-outline', // Icon bánh răng cho dịch vụ
        color: '#FF5722',
        dataDate: new Date(),
        yoyGrowth: 22.1 * unitMultiplier,
        trend: [12, 13, 15, 16, 17, 18.5].map(v => Math.round(v * unitMultiplier * 10) / 10)
      },
      {
        code: 'LoiNhuanKhoan',
        name: 'Lợi nhuận khoán tài chính',
        actualValue: Math.round(12500 * unitMultiplier),
        planValue: Math.round(15000 * unitMultiplier),
        unit: 'triệu VNĐ',
        icon: 'mdi-trophy-variant', // Icon cúp chiến thắng cho lợi nhuận
        color: '#9C27B0',
        dataDate: new Date(),
        yoyGrowth: 18.7 * unitMultiplier,
        trend: [8, 9, 9.5, 10, 11, 12.5].map(v => Math.round(v * unitMultiplier * 10) / 10)
      }
    ];
    
    monthlyTrendData.value = {
      months: ['T7', 'T8', 'T9', 'T10', 'T11', 'T12'],
      series: [
        {
          name: 'Huy động vốn',
          data: [1000, 1100, 1050, 1200, 1150, 1200].map(v => Math.round(v * unitMultiplier)),
          color: '#8B1538'
        },
        {
          name: 'Dư nợ cho vay',
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
  activeIndicator.value = indicator.code;
  viewMode.value = 'detail';
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
  await Promise.all([
    loadIndicators(),
    loadUnits()
  ]);
  await loadDashboardData();
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

.enhanced-kpi-card {
  transition: all 0.3s ease;
  border-radius: 16px;
  overflow: hidden;
}

.enhanced-kpi-card:hover {
  transform: translateY(-8px) scale(1.02);
  box-shadow: 0 15px 40px rgba(139, 21, 56, 0.15);
}

/* Charts section */
.charts-section {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(500px, 1fr));
  gap: 30px;
  margin-bottom: 40px;
}

.chart-card {
  background: white;
  border-radius: 20px;
  padding: 25px;
  box-shadow: 0 10px 30px rgba(0,0,0,0.1);
  animation: fadeIn 0.8s ease-out;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

.chart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.chart-header h3 {
  font-size: 18px;
  font-weight: 600;
  color: #303133;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 10px;
}

.chart-controls {
  display: flex;
  gap: 0;
}

.chart-controls :deep(.el-button) {
  background: transparent;
  color: #606266;
  border-color: #DCDFE6;
}

.chart-controls :deep(.el-button.el-button--primary) {
  background: #8B1538;
  color: white;
  border-color: #8B1538;
}

.chart-indicator-select {
  width: 180px;
}

/* Top performers section */
.top-performers-section {
  margin-top: 40px;
}

.performers-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
}

.performer-card {
  background: white;
  border-radius: 16px;
  padding: 25px;
  box-shadow: 0 5px 20px rgba(0,0,0,0.08);
  display: flex;
  align-items: center;
  gap: 20px;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.performer-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, #8B1538 0%, #B91D47 100%);
}

.performer-card.rank-1 .rank-badge {
  background: linear-gradient(135deg, #FFD700 0%, #FFA500 100%);
}

.performer-card.rank-2 .rank-badge {
  background: linear-gradient(135deg, #C0C0C0 0%, #757575 100%);
}

.performer-card.rank-3 .rank-badge {
  background: linear-gradient(135deg, #CD7F32 0%, #8B4513 100%);
}

.rank-badge {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: bold;
  box-shadow: 0 4px 10px rgba(0,0,0,0.2);
}

.rank-badge i {
  font-size: 24px;
}

.rank-badge span {
  font-size: 14px;
}

.performer-info {
  flex: 1;
}

.performer-info h4 {
  margin: 0 0 5px 0;
  font-size: 16px;
  color: #303133;
}

.performer-info p {
  margin: 0;
  font-size: 14px;
  color: #909399;
}

.performer-progress {
  width: 100px;
}

/* Floating Action Button */
.fab-container {
  position: fixed;
  bottom: 30px;
  right: 30px;
  z-index: 999;
}

.fab {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  background: linear-gradient(135deg, #8B1538 0%, #B91D47 100%);
  color: white;
  border: none;
  font-size: 24px;
  cursor: pointer;
  box-shadow: 0 6px 20px rgba(139, 21, 56, 0.4);
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.fab:hover {
  transform: scale(1.1);
  box-shadow: 0 8px 25px rgba(139, 21, 56, 0.5);
}

.fab:active {
  transform: scale(0.95);
}

/* Transitions */
.fade-slide-enter-active,
.fade-slide-leave-active {
  transition: all 0.3s ease;
}

.fade-slide-enter-from {
  opacity: 0;
  transform: translateX(30px);
}

.fade-slide-leave-to {
  opacity: 0;
  transform: translateX(-30px);
}

.card-flip-move,
.card-flip-enter-active,
.card-flip-leave-active {
  transition: all 0.5s ease;
}

.card-flip-enter-from {
  opacity: 0;
  transform: rotateY(90deg);
}

.card-flip-leave-to {
  opacity: 0;
  transform: rotateY(-90deg);
}

/* Custom tabs */
.custom-tabs :deep(.el-tabs__header) {
  background: linear-gradient(135deg, #8B1538 0%, #B91D47 100%);
  margin: 0;
  border-radius: 12px 12px 0 0;
}

.custom-tabs :deep(.el-tabs__item) {
  color: rgba(255,255,255,0.8);
  border: none;
  background: transparent;
}

.custom-tabs :deep(.el-tabs__item.is-active) {
  color: white;
  background: rgba(255,255,255,0.2);
}

/* Dialog styles */
.calculation-content {
  padding: 20px 0;
}

.calculation-progress {
  margin-top: 20px;
  text-align: center;
}

.calculation-progress p {
  margin-top: 10px;
  color: #606266;
  font-size: 14px;
}

/* Responsive design */
@media (max-width: 768px) {
  .dashboard-title {
    font-size: 24px;
  }
  
  .header-content {
    flex-direction: column;
    text-align: center;
  }
  
  .summary-stats {
    justify-content: center;
  }
  
  .filters-section {
    justify-content: center;
  }
  
  .kpi-cards-grid {
    grid-template-columns: 1fr;
  }
  
  .charts-section {
    grid-template-columns: 1fr;
  }
  
  .performers-grid {
    grid-template-columns: 1fr;
  }
  
  .overall-performance {
    flex-direction: column;
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
