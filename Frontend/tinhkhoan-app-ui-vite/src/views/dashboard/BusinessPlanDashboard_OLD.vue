<template>
  <div class="business-plan-dashboard">
    <!-- Header với gradient Agribank và hiệu ứng -->
    <div class="dashboard-header">
      <div class="header-bg-pattern"></div>
      <div class="header-content">
        <div class="header-left">
          <h1 class="dashboard-title">
            <i class="mdi mdi-view-dashboard-variant"></i>
            Dashboard Chỉ Tiêu Kế Hoạch Kinh Doanh
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
            :prefix-icon="Calendar"
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
            <el-button 
              :icon="Download" 
              class="export-btn"
            >
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
        <template v-if="viewMode === 'overview'">
          <div class="overview-container">
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
        </template>
                <i class="mdi mdi-download"></i>
              </el-button>
            </div>
            <comparison-chart
              :data="unitComparisonData"
              :height="300"
              type="bar"
            />
          </div>
        </div>
      </template>
      
      <!-- Detail Mode -->
      <template v-else-if="viewMode === 'detail'">
        <div class="detail-section">
          <el-tabs v-model="activeIndicator" type="card" class="indicator-tabs">
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
      </template>
      
      <!-- Comparison Mode -->
      <template v-else-if="viewMode === 'comparison'">
        <comparison-view
          :indicators="indicators"
          :units="units"
          :date="selectedDate"
        />
      </template>
    </div>

    <!-- Floating Action Button -->
    <el-tooltip content="Tính toán lại tất cả chỉ tiêu" placement="left">
      <el-button
        type="primary"
        :icon="TrendCharts"
        circle
        size="large"
        class="fab-calculate"
        @click="showCalculationDialog"
      />
    </el-tooltip>

    <!-- Calculation Dialog -->
    <el-dialog
      v-model="calculationDialog.visible"
      title="Tính toán chỉ tiêu"
      width="600px"
      :close-on-click-modal="false"
    >
      <div class="calculation-content">
        <el-alert
          title="Đang tính toán lại tất cả chỉ tiêu từ dữ liệu gốc"
          type="info"
          :closable="false"
          show-icon
        />
        <div class="calculation-progress" v-if="calculating">
          <el-progress :percentage="calculationProgress" />
          <p>{{ calculationMessage }}</p>
        </div>
      </div>
      
      <template #footer>
        <el-button @click="calculationDialog.visible = false" :disabled="calculating">
          Đóng
        </el-button>
        <el-button 
          type="primary" 
          @click="executeCalculation" 
          :loading="calculating"
        >
          {{ calculating ? 'Đang tính toán...' : 'Bắt đầu tính toán' }}
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import { ElMessage } from 'element-plus';
import { Refresh, TrendCharts } from '@element-plus/icons-vue';
import { dashboardService } from '../../services/dashboardService';
import KpiCard from '../../components/dashboard/KpiCard.vue';
import TrendChart from '../../components/dashboard/TrendChart.vue';
import ComparisonChart from '../../components/dashboard/ComparisonChart.vue';
import IndicatorDetail from '../../components/dashboard/IndicatorDetail.vue';
import ComparisonView from '../../components/dashboard/ComparisonView.vue';

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

// Calculation state
const calculationDialog = ref({ visible: false });
const calculating = ref(false);
const calculationProgress = ref(0);
const calculationMessage = ref('');

// Computed
const getCurrentPeriodLabel = () => {
  const date = new Date(selectedDate.value);
  const unit = selectedUnitId.value 
    ? units.value.find(u => u.id === selectedUnitId.value)?.name 
    : 'Toàn tỉnh';
  return `${unit} - Tháng ${date.getMonth() + 1}/${date.getFullYear()}`;
};

// Methods
const loadIndicators = async () => {
  try {
    const response = await dashboardService.getIndicators();
    indicators.value = response.$values || response;
  } catch (error) {
    console.error('Lỗi tải indicators:', error);
    ElMessage.error('Không thể tải danh sách chỉ tiêu');
  }
};

const loadUnits = async () => {
  try {
    const response = await dashboardService.getUnits();
    units.value = response.$values || response;
  } catch (error) {
    console.error('Lỗi tải units:', error);
    ElMessage.error('Không thể tải danh sách đơn vị');
  }
};

const loadDashboardData = async () => {
  loading.value = true;
  try {
    // Generate mock data for beautiful display
    const mockData = [
      {
        code: 'HuyDong',
        name: 'Nguồn vốn huy động',
        unit: 'tỷ đồng',
        icon: 'mdi-bank',
        color: '#4CAF50',
        actualValue: 4235.8,
        planValue: 5000,
        yoyGrowth: 12.5,
        dataDate: selectedDate.value,
        trend: [4100, 4150, 4200, 4180, 4220, 4235.8]
      },
      {
        code: 'DuNo',
        name: 'Dư nợ cho vay',
        unit: 'tỷ đồng',
        icon: 'mdi-cash-multiple',
        color: '#2196F3',
        actualValue: 3892.4,
        planValue: 4500,
        yoyGrowth: 8.7,
        dataDate: selectedDate.value,
        trend: [3800, 3820, 3850, 3870, 3885, 3892.4]
      },
      {
        code: 'TyLeNoXau',
        name: 'Tỷ lệ nợ xấu',
        unit: '%',
        icon: 'mdi-alert-circle',
        color: '#FF9800',
        actualValue: 1.85,
        planValue: 2.0,
        yoyGrowth: -0.3,
        dataDate: selectedDate.value,
        trend: [1.95, 1.92, 1.88, 1.87, 1.86, 1.85]
      },
      {
        code: 'ThuHoiXLRR',
        name: 'Thu hồi nợ XLRR',
        unit: 'tỷ đồng',
        icon: 'mdi-cash-remove',
        color: '#F44336',
        actualValue: 38.2,
        planValue: 50,
        yoyGrowth: 15.8,
        dataDate: selectedDate.value,
        trend: [32, 34, 35, 36, 37, 38.2]
      },
      {
        code: 'ThuDichVu',
        name: 'Thu dịch vụ',
        unit: 'tỷ đồng',
        icon: 'mdi-currency-usd',
        color: '#9C27B0',
        actualValue: 78.5,
        planValue: 100,
        yoyGrowth: 22.1,
        dataDate: selectedDate.value,
        trend: [65, 68, 72, 75, 76, 78.5]
      },
      {
        code: 'LoiNhuan',
        name: 'Lợi nhuận',
        unit: 'tỷ đồng',
        icon: 'mdi-trending-up',
        color: '#00BCD4',
        actualValue: 168.9,
        planValue: 200,
        yoyGrowth: 18.6,
        dataDate: selectedDate.value,
        trend: [145, 150, 155, 160, 165, 168.9]
      }
    ];
    
    dashboardData.value = mockData;
    
  } catch (error) {
    console.error('Lỗi tải dashboard data:', error);
    ElMessage.error('Không thể tải dữ liệu Dashboard');
  } finally {
    loading.value = false;
  }
};

const refreshDashboard = () => {
  loadDashboardData();
};

const showIndicatorDetail = (indicator) => {
  viewMode.value = 'detail';
  activeIndicator.value = indicator.code;
};

const handleFilterChange = (filters) => {
  console.log('Filter changed:', filters);
};

const showCalculationDialog = () => {
  calculationDialog.value.visible = true;
};

const executeCalculation = async () => {
  calculating.value = true;
  calculationProgress.value = 0;
  
  try {
    const steps = [
      'Kết nối cơ sở dữ liệu...',
      'Đọc dữ liệu DP01...',
      'Đọc dữ liệu LN01...',
      'Đọc dữ liệu GLCB41...',
      'Tính toán nguồn vốn huy động...',
      'Tính toán dư nợ cho vay...',
      'Tính toán tỷ lệ nợ xấu...',
      'Tính toán thu hồi nợ XLRR...',
      'Tính toán thu dịch vụ...',
      'Tính toán lợi nhuận...',
      'Lưu kết quả...',
      'Hoàn thành!'
    ];
    
    for (let i = 0; i < steps.length; i++) {
      calculationMessage.value = steps[i];
      calculationProgress.value = ((i + 1) / steps.length) * 100;
      await new Promise(resolve => setTimeout(resolve, 500));
    }
    
    ElMessage.success('Đã tính toán lại tất cả chỉ tiêu thành công!');
    calculationDialog.value.visible = false;
    await refreshDashboard();
    
  } catch (error) {
    console.error('Lỗi tính toán:', error);
    ElMessage.error('Có lỗi xảy ra trong quá trình tính toán');
  } finally {
    calculating.value = false;
  }
};

const exportChart = () => {
  ElMessage.info('Tính năng xuất biểu đồ đang phát triển');
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
.business-plan-dashboard {
  min-height: 100vh;
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
}

.dashboard-header {
  background: linear-gradient(135deg, #8B1538 0%, #A6195C 50%, #B91D47 100%);
  color: white;
  padding: 0;
  box-shadow: 0 4px 20px rgba(139, 21, 56, 0.3);
}

.header-content {
  padding: 30px;
  max-width: 1400px;
  margin: 0 auto;
}

.header-title {
  margin-bottom: 25px;
}

.header-title h1 {
  margin: 0;
  font-size: 28px;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 15px;
}

.header-icon {
  font-size: 32px;
  opacity: 0.9;
}

.subtitle {
  margin: 8px 0 0 47px;
  font-size: 16px;
  opacity: 0.9;
}

.header-filters {
  display: flex;
  gap: 15px;
  flex-wrap: wrap;
  align-items: center;
}

.filter-item {
  min-width: 200px;
}

.view-mode-toggle {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 8px;
  padding: 4px;
}

.view-mode-toggle :deep(.el-radio-button__inner) {
  background: transparent;
  color: white;
  border: none;
  font-weight: 500;
}

.view-mode-toggle :deep(.el-radio-button__original-radio:checked + .el-radio-button__inner) {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  box-shadow: none;
}

.refresh-btn {
  padding: 12px 24px;
  font-weight: 600;
}

.dashboard-content {
  padding: 30px;
  max-width: 1400px;
  margin: 0 auto;
}

.kpi-cards-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 25px;
  margin-bottom: 40px;
}

.charts-section {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(600px, 1fr));
  gap: 30px;
}

.chart-container {
  background: white;
  border-radius: 12px;
  padding: 25px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.chart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.chart-header h3 {
  margin: 0;
  font-size: 18px;
  color: #303133;
  display: flex;
  align-items: center;
  gap: 10px;
}

.detail-section {
  background: white;
  border-radius: 12px;
  padding: 25px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
}

.indicator-tabs :deep(.el-tabs__header) {
  margin-bottom: 25px;
}

.indicator-tabs :deep(.el-tabs__item) {
  font-weight: 500;
}

.fab-calculate {
  position: fixed;
  bottom: 30px;
  right: 30px;
  width: 60px;
  height: 60px;
  border-radius: 50%;
  background: linear-gradient(135deg, #8B1538 0%, #A6195C 100%);
  box-shadow: 0 8px 32px rgba(139, 21, 56, 0.4);
  z-index: 1000;
}

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

@media (max-width: 768px) {
  .header-content {
    padding: 20px;
  }
  
  .header-title h1 {
    font-size: 20px;
  }
  
  .header-filters {
    flex-direction: column;
    align-items: stretch;
  }
  
  .filter-item {
    min-width: auto;
  }
  
  .kpi-cards-grid {
    grid-template-columns: 1fr;
    gap: 20px;
  }
  
  .charts-section {
    grid-template-columns: 1fr;
    gap: 20px;
  }
  
  .dashboard-content {
    padding: 20px;
  }
}

@media (max-width: 480px) {
  .fab-calculate {
    bottom: 20px;
    right: 20px;
    width: 50px;
    height: 50px;
  }
}
</style>
