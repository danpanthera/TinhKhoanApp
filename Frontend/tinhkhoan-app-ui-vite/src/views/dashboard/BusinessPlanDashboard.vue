<template>
  <div class="modern-dashboard">
    <!-- Header với nền đỏ bordeaux -->
    <div class="dashboard-header-bordeaux">
      <div class="header-bg-overlay"></div>
      <div class="header-content">
        <div class="header-left">
          <div class="title-section">
            <div class="icon-container">
              <div class="icon-glow"></div>
              <i class="dashboard-icon">📊</i>
            </div>
            <div class="title-wrapper">
              <h1 class="dashboard-title">DASHBOARD TỔNG HỢP</h1>
              <p class="dashboard-subtitle">
                <span class="subtitle-icon">📅</span>
                {{ getCurrentPeriodLabel() }}
                <span class="live-indicator">
                  <span class="pulse-dot"></span>
                  Real-time
                </span>
              </p>
              <p class="current-time">
                <span class="time-icon">⏰</span>
                {{ formatCurrentTime() }}
              </p>
            </div>
          </div>
        </div>
        
        <div class="header-right">
          <!-- Bộ lọc nâng cao -->
          <div class="filter-panel">
            <div class="filter-group">
              <label class="filter-label">Chi nhánh</label>
              <el-select 
                v-model="selectedBranch" 
                placeholder="Chọn chi nhánh"
                @change="handleBranchChange"
                :loading="loading"
                filterable
                clearable
                class="branch-selector"
                size="large"
              >
                <el-option
                  v-for="branch in branches"
                  :key="branch.id"
                  :label="branch.name"
                  :value="branch.id"
                >
                  <div class="option-item">
                    <span class="option-icon">🏢</span>
                    <span class="option-text">{{ branch.name }}</span>
                  </div>
                </el-option>
              </el-select>
            </div>
            
            <div class="filter-group">
              <label class="filter-label">Thời gian</label>
              <el-date-picker
                v-model="dateRange"
                type="monthrange"
                start-placeholder="Từ tháng"
                end-placeholder="Đến tháng"
                format="MM/YYYY"
                value-format="YYYY-MM"
                @change="handleDateRangeChange"
                class="date-picker"
                size="large"
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading Overlay hiện đại -->
    <LoadingOverlay 
      :show="loading" 
      title="Đang tải Dashboard" 
      message="Đang cập nhật dữ liệu chỉ tiêu mới nhất..."
      icon="📊"
    />

    <!-- Main Dashboard Content -->
    <div class="dashboard-content" v-motion-slide-visible-once-bottom>
      <!-- Tổng quan nhanh -->
      <div class="overview-section" v-motion-fade-visible-once>
        <div class="overview-card">
          <div class="overview-header">
            <h3>🎯 Tổng quan hiệu suất</h3>
            <div class="refresh-btn" @click="refreshData" :class="{ spinning: loading }">
              <i>🔄</i>
            </div>
          </div>
          <div class="overview-stats">
            <div class="stat-item">
              <div class="stat-value">{{ overviewStats.totalTargets }}</div>
              <div class="stat-label">Tổng chỉ tiêu</div>
            </div>
            <div class="stat-divider"></div>
            <div class="stat-item">
              <div class="stat-value">{{ overviewStats.completedTargets }}</div>
              <div class="stat-label">Đã hoàn thành</div>
            </div>
            <div class="stat-divider"></div>
            <div class="stat-item">
              <div class="stat-value text-success">{{ overviewStats.avgCompletion }}%</div>
              <div class="stat-label">Tỷ lệ hoàn thành</div>
            </div>
          </div>
        </div>
      </div>

      <!-- 6 Cards chỉ tiêu với animation -->
      <div class="indicators-grid">
        <div 
          v-for="(indicator, index) in indicators" 
          :key="indicator.id"
          class="indicator-card-modern"
          :class="[indicator.class, { 'loading-pulse': loading }]"
          v-motion-slide-visible-once-bottom
          :style="{ '--delay': index * 100 + 'ms' }"
          @mouseenter="playHoverSound"
          @click="showIndicatorDetail(indicator)"
        >
          <!-- Card header với icon động -->
          <div class="card-header-modern">
            <div class="icon-wrapper">
              <div class="icon-bg"></div>
              <span class="indicator-icon">{{ indicator.icon }}</span>
            </div>
            <div class="header-text">
              <h4 class="indicator-title">{{ indicator.name }}</h4>
              <div class="status-badge" :class="getStatusClass(indicator.completionRate)">
                {{ getStatusText(indicator.completionRate) }}
              </div>
            </div>
          </div>

          <!-- Giá trị chính với animated counter -->
          <div class="value-section">
            <div class="main-value">
              <CountUp
                :end-val="indicator.currentValue"
                :options="countUpOptions"
                class="value-number"
              />
              <span class="value-unit">{{ indicator.unit }}</span>
            </div>
            
            <!-- Tăng giảm so với đầu năm -->
            <div class="change-indicator" :class="getChangeClass(indicator.changeFromYearStartPercent)">
              <span class="change-arrow">{{ getChangeArrow(indicator.changeFromYearStartPercent) }}</span>
              <span class="change-text">
                {{ formatChangePercent(indicator.changeFromYearStartPercent) }} so với đầu năm
              </span>
            </div>
          </div>

          <!-- Progress circular -->
          <div class="progress-section">
            <div class="circular-progress">
              <el-progress
                type="circle"
                :percentage="Math.min(indicator.completionRate, 100)"
                :width="80"
                :stroke-width="8"
                :color="getProgressColor(indicator.completionRate)"
                class="progress-chart"
              >
                <template #default="{ percentage }">
                  <span class="progress-text">{{ Math.round(percentage) }}%</span>
                </template>
              </el-progress>
            </div>
            
            <!-- Target vs Actual -->
            <div class="target-actual">
              <div class="target-row">
                <span class="label">Kế hoạch:</span>
                <span class="value">{{ formatNumber(indicator.targetValue) }}</span>
              </div>
              <div class="actual-row">
                <span class="label">Thực hiện:</span>
                <span class="value">{{ formatNumber(indicator.currentValue) }}</span>
              </div>
            </div>
          </div>

          <!-- Mini chart trong card -->
          <div class="mini-chart">
            <div class="chart-container" :id="`mini-chart-${indicator.id}`"></div>
          </div>
        </div>
      </div>

      <!-- Biểu đồ chi tiết -->
      <div class="charts-section" v-motion-slide-visible-once-bottom>
        <div class="charts-header">
          <h3>📈 Phân tích chi tiết</h3>
          <div class="chart-tabs">
            <div 
              v-for="tab in chartTabs" 
              :key="tab.key"
              class="chart-tab"
              :class="{ active: activeChartTab === tab.key }"
              @click="activeChartTab = tab.key"
            >
              <span class="tab-icon">{{ tab.icon }}</span>
              <span class="tab-text">{{ tab.label }}</span>
            </div>
          </div>
        </div>
        
        <div class="charts-content">
          <!-- Biểu đồ cột so sánh -->
          <div v-if="activeChartTab === 'comparison'" class="chart-panel">
            <div class="chart-wrapper">
              <div id="comparison-chart" class="chart-container-large"></div>
            </div>
          </div>
          
          <!-- Biểu đồ xu hướng -->
          <div v-if="activeChartTab === 'trend'" class="chart-panel">
            <div class="chart-wrapper">
              <div id="trend-chart" class="chart-container-large"></div>
            </div>
          </div>
          
          <!-- Biểu đồ tỷ lệ hoàn thành -->
          <div v-if="activeChartTab === 'completion'" class="chart-panel">
            <div class="chart-wrapper">
              <div id="completion-chart" class="chart-container-large"></div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal chi tiết chỉ tiêu -->
    <el-dialog
      v-model="showDetailModal"
      :title="selectedIndicator?.name"
      width="60%"
      class="indicator-detail-modal"
    >
      <div v-if="selectedIndicator" class="detail-content">
        <!-- Nội dung chi tiết sẽ được thêm sau -->
        <p>Chi tiết cho {{ selectedIndicator.name }}</p>
      </div>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch, nextTick } from 'vue';
import { ElMessage, ElDialog } from 'element-plus';
import { useMotion } from '@vueuse/motion';
import CountUp from 'vue-countup-v2';
import * as echarts from 'echarts';
import dayjs from 'dayjs';
import LoadingOverlay from '../../components/dashboard/LoadingOverlay.vue';
import { dashboardService } from '../../services/dashboardService.js';

// State quản lý
const loading = ref(false);
const selectedBranch = ref('HoiSo'); // Mặc định chọn Hội Sở
const dateRange = ref([dayjs().format('YYYY-MM'), dayjs().format('YYYY-MM')]); // Tháng hiện tại
const currentTime = ref(new Date());
const showDetailModal = ref(false);
const selectedIndicator = ref(null);
const activeChartTab = ref('comparison');

// Danh sách chi nhánh (14 chi nhánh theo yêu cầu)
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

// 6 chỉ tiêu dashboard chính
const indicators = ref([
  {
    id: 'nguon_von',
    name: 'Nguồn vốn',
    icon: '💰',
    class: 'nguon-von',
    unit: 'tỷ',
    format: 'currency',
    currentValue: 1250.5,
    targetValue: 1200.0,
    completionRate: 104.2,
    changeFromYearStart: 125.3,
    changeFromYearStartPercent: 11.2
  },
  {
    id: 'du_no',
    name: 'Dư nợ',
    icon: '💳',
    class: 'du-no',
    unit: 'tỷ',
    format: 'currency',
    currentValue: 980.3,
    targetValue: 1000.0,
    completionRate: 98.0,
    changeFromYearStart: 45.8,
    changeFromYearStartPercent: 4.9
  },
  {
    id: 'no_xau',
    name: 'Nợ Xấu',
    icon: '⚠️',
    class: 'no-xau',
    unit: '%',
    format: 'percent',
    currentValue: 1.8,
    targetValue: 2.0,
    completionRate: 90.0,
    changeFromYearStart: -0.3,
    changeFromYearStartPercent: -14.3
  },
  {
    id: 'thu_no_xlrr',
    name: 'Thu nợ đã XLRR',
    icon: '📈',
    class: 'thu-no-xlrr',
    unit: 'tỷ',
    format: 'currency',
    currentValue: 45.7,
    targetValue: 50.0,
    completionRate: 91.4,
    changeFromYearStart: 8.2,
    changeFromYearStartPercent: 21.9
  },
  {
    id: 'thu_dich_vu',
    name: 'Thu dịch vụ',
    icon: '🏦',
    class: 'thu-dich-vu',
    unit: 'tỷ',
    format: 'currency',
    currentValue: 28.9,
    targetValue: 30.0,
    completionRate: 96.3,
    changeFromYearStart: 3.1,
    changeFromYearStartPercent: 12.0
  },
  {
    id: 'tai_chinh',
    name: 'Tài chính',
    icon: '💵',
    class: 'tai-chinh',
    unit: 'tỷ',
    format: 'currency',
    currentValue: 156.4,
    targetValue: 160.0,
    completionRate: 97.8,
    changeFromYearStart: 18.6,
    changeFromYearStartPercent: 13.5
  }
]);

// Tổng quan thống kê
const overviewStats = computed(() => {
  const total = indicators.value.length;
  const completed = indicators.value.filter(i => i.completionRate >= 100).length;
  const avgCompletion = indicators.value.reduce((sum, i) => sum + i.completionRate, 0) / total;
  
  return {
    totalTargets: total,
    completedTargets: completed,
    avgCompletion: Math.round(avgCompletion * 10) / 10
  };
});

// Tabs cho biểu đồ
const chartTabs = ref([
  { key: 'comparison', label: 'So sánh', icon: '📊' },
  { key: 'trend', label: 'Xu hướng', icon: '📈' },
  { key: 'completion', label: 'Hoàn thành', icon: '🎯' }
]);

// Cấu hình CountUp
const countUpOptions = {
  useEasing: true,
  useGrouping: true,
  separator: ',',
  decimal: '.',
  duration: 2
};

// Sound effects (tái sử dụng code cũ)
const audioContext = ref(null);
const sounds = ref({
  hover: null,
  click: null,
  success: null,
  notification: null
});

// Phương thức âm thanh
const initAudio = () => {
  try {
    audioContext.value = new (window.AudioContext || window.webkitAudioContext)();
    createSounds();
  } catch (error) {
    console.warn('Audio not supported:', error);
  }
};

const createSounds = () => {
  if (!audioContext.value) return;
  
  sounds.value.hover = createTone(800, 0.1, 0.05);
  sounds.value.click = createTone(1200, 0.15, 0.1);
  sounds.value.success = createTone(600, 0.3, 0.2);
  sounds.value.notification = createTone(900, 0.2, 0.15);
};

const createTone = (frequency, duration, volume) => {
  return () => {
    if (!audioContext.value) return;
    
    const oscillator = audioContext.value.createOscillator();
    const gainNode = audioContext.value.createGain();
    
    oscillator.connect(gainNode);
    gainNode.connect(audioContext.value.destination);
    
    oscillator.frequency.setValueAtTime(frequency, audioContext.value.currentTime);
    oscillator.type = 'sine';
    
    gainNode.gain.setValueAtTime(0, audioContext.value.currentTime);
    gainNode.gain.linearRampToValueAtTime(volume, audioContext.value.currentTime + 0.01);
    gainNode.gain.exponentialRampToValueAtTime(0.001, audioContext.value.currentTime + duration);
    
    oscillator.start(audioContext.value.currentTime);
    oscillator.stop(audioContext.value.currentTime + duration);
  };
};

const playHoverSound = () => {
  if (sounds.value.hover) {
    sounds.value.hover();
  }
};

const playClickSound = () => {
  if (sounds.value.click) {
    sounds.value.click();
  }
};

const playSuccessSound = () => {
  if (sounds.value.success) {
    sounds.value.success();
  }
};

// Phương thức tiện ích
const getCurrentPeriodLabel = () => {
  const now = new Date();
  return `Tháng ${now.getMonth() + 1}/${now.getFullYear()}`;
};

const formatCurrentTime = () => {
  return currentTime.value.toLocaleString('vi-VN', { 
    year: 'numeric', 
    month: '2-digit', 
    day: '2-digit',
    hour: '2-digit', 
    minute: '2-digit', 
    second: '2-digit' 
  });
};

const formatNumber = (value) => {
  if (!value) return '0';
  return new Intl.NumberFormat('vi-VN').format(value);
};

const formatChangePercent = (value) => {
  if (value === null || value === undefined) return '0%';
  const sign = value >= 0 ? '+' : '';
  return `${sign}${value.toFixed(1)}%`;
};

const getStatusClass = (completionRate) => {
  if (completionRate >= 100) return 'status-excellent';
  if (completionRate >= 90) return 'status-good';
  if (completionRate >= 70) return 'status-average';
  return 'status-poor';
};

const getStatusText = (completionRate) => {
  if (completionRate >= 100) return 'Xuất sắc';
  if (completionRate >= 90) return 'Tốt';
  if (completionRate >= 70) return 'Khá';
  return 'Cần cải thiện';
};

const getChangeClass = (percent) => {
  return percent >= 0 ? 'change-positive' : 'change-negative';
};

const getChangeArrow = (percent) => {
  return percent >= 0 ? '↗️' : '↘️';
};

const getProgressColor = (percentage) => {
  if (percentage >= 100) return '#52c41a';
  if (percentage >= 90) return '#1890ff';
  if (percentage >= 70) return '#faad14';
  return '#f5222d';
};

// Xử lý sự kiện
const handleBranchChange = async () => {
  playClickSound();
  await loadDashboardData();
};

const handleDateRangeChange = async () => {
  await loadDashboardData();
};

const refreshData = async () => {
  await loadDashboardData();
};

const showIndicatorDetail = (indicator) => {
  selectedIndicator.value = indicator;
  showDetailModal.value = true;
};

// Tải dữ liệu dashboard
const loadDashboardData = async () => {
  loading.value = true;
  try {
    // Gọi API để lấy dữ liệu thực tế
    const data = await dashboardService.getGeneralDashboardData(selectedBranch.value);
    
    if (data && data.indicators) {
      // Cập nhật dữ liệu từ API
      indicators.value = data.indicators;
    }
    
    // Tạo biểu đồ sau khi có dữ liệu
    await nextTick();
    createCharts();
    
    playSuccessSound();
    ElMessage.success({
      message: 'Dữ liệu đã được cập nhật thành công',
      type: 'success',
      duration: 2000,
      showClose: true
    });
    
  } catch (error) {
    console.error('Error loading dashboard data:', error);
    ElMessage.error({
      message: 'Không thể tải dữ liệu dashboard. Vui lòng thử lại!',
      type: 'error',
      duration: 3000,
      showClose: true
    });
  } finally {
    loading.value = false;
  }
};

// Tạo biểu đồ với ECharts
const createCharts = () => {
  createComparisonChart();
  createTrendChart();
  createCompletionChart();
  createMiniCharts();
};

const createComparisonChart = () => {
  const chartDom = document.getElementById('comparison-chart');
  if (!chartDom) return;
  
  const myChart = echarts.init(chartDom);
  const option = {
    title: {
      text: 'So sánh Thực hiện vs Kế hoạch',
      left: 'center',
      textStyle: { color: '#333', fontSize: 16, fontWeight: 'bold' }
    },
    tooltip: {
      trigger: 'axis',
      axisPointer: { type: 'shadow' }
    },
    legend: {
      data: ['Kế hoạch', 'Thực hiện'],
      bottom: 10
    },
    xAxis: {
      type: 'category',
      data: indicators.value.map(i => i.name),
      axisLabel: { rotate: 45, fontSize: 10 }
    },
    yAxis: { type: 'value' },
    series: [
      {
        name: 'Kế hoạch',
        type: 'bar',
        data: indicators.value.map(i => i.targetValue),
        itemStyle: { color: '#91caff' }
      },
      {
        name: 'Thực hiện',
        type: 'bar',
        data: indicators.value.map(i => i.currentValue),
        itemStyle: { color: '#1890ff' }
      }
    ]
  };
  
  myChart.setOption(option);
};

const createTrendChart = () => {
  const chartDom = document.getElementById('trend-chart');
  if (!chartDom) return;
  
  const myChart = echarts.init(chartDom);
  // Mock dữ liệu xu hướng 6 tháng
  const months = ['T7', 'T8', 'T9', 'T10', 'T11', 'T12'];
  
  const option = {
    title: {
      text: 'Xu hướng 6 tháng gần nhất',
      left: 'center',
      textStyle: { color: '#333', fontSize: 16, fontWeight: 'bold' }
    },
    tooltip: { trigger: 'axis' },
    legend: {
      data: indicators.value.map(i => i.name),
      bottom: 10,
      type: 'scroll'
    },
    xAxis: {
      type: 'category',
      data: months
    },
    yAxis: { type: 'value' },
    series: indicators.value.map((indicator, index) => ({
      name: indicator.name,
      type: 'line',
      smooth: true,
      data: months.map(() => indicator.currentValue * (0.8 + Math.random() * 0.4)),
      lineStyle: { width: 3 }
    }))
  };
  
  myChart.setOption(option);
};

const createCompletionChart = () => {
  const chartDom = document.getElementById('completion-chart');
  if (!chartDom) return;
  
  const myChart = echarts.init(chartDom);
  const option = {
    title: {
      text: 'Tỷ lệ hoàn thành các chỉ tiêu',
      left: 'center',
      textStyle: { color: '#333', fontSize: 16, fontWeight: 'bold' }
    },
    tooltip: {
      trigger: 'item',
      formatter: '{a} <br/>{b}: {c}% ({d}%)'
    },
    legend: {
      orient: 'vertical',
      left: 'left',
      data: indicators.value.map(i => i.name)
    },
    series: [
      {
        name: 'Tỷ lệ hoàn thành',
        type: 'pie',
        radius: ['50%', '70%'],
        avoidLabelOverlap: false,
        itemStyle: {
          borderRadius: 10,
          borderColor: '#fff',
          borderWidth: 2
        },
        label: {
          show: false,
          position: 'center'
        },
        emphasis: {
          label: {
            show: true,
            fontSize: 20,
            fontWeight: 'bold'
          }
        },
        labelLine: { show: false },
        data: indicators.value.map(indicator => ({
          value: indicator.completionRate,
          name: indicator.name
        }))
      }
    ]
  };
  
  myChart.setOption(option);
};

const createMiniCharts = () => {
  indicators.value.forEach(indicator => {
    const chartDom = document.getElementById(`mini-chart-${indicator.id}`);
    if (!chartDom) return;
    
    const myChart = echarts.init(chartDom);
    // Mock dữ liệu mini chart
    const data = Array.from({length: 7}, () => Math.random() * 100);
    
    const option = {
      grid: { top: 5, left: 5, right: 5, bottom: 5 },
      xAxis: { type: 'category', show: false, data: ['', '', '', '', '', '', ''] },
      yAxis: { type: 'value', show: false },
      series: [
        {
          type: 'line',
          smooth: true,
          symbol: 'none',
          lineStyle: { color: indicator.id === 'nguon_von' ? '#52c41a' : 
                             indicator.id === 'du_no' ? '#1890ff' :
                             indicator.id === 'no_xau' ? '#fa541c' :
                             indicator.id === 'thu_no_xlrr' ? '#722ed1' :
                             indicator.id === 'thu_dich_vu' ? '#13c2c2' : '#faad14', 
                      width: 2 },
          areaStyle: { opacity: 0.3 },
          data: data
        }
      ]
    };
    
    myChart.setOption(option);
  });
};

// Lifecycle
onMounted(async () => {
  // Khởi tạo âm thanh
  initAudio();
  
  // Cập nhật thời gian real-time
  updateTime();
  setInterval(updateTime, 1000);
  
  // Tải dữ liệu ban đầu
  await loadDashboardData();
  
  // Phát âm thanh chào mừng
  setTimeout(() => {
    if (sounds.value.notification) {
      sounds.value.notification();
    }
  }, 1000);
});

const updateTime = () => {
  currentTime.value = new Date();
};

// Watch thay đổi branch
watch(selectedBranch, handleBranchChange);

// Watch thay đổi tab biểu đồ
watch(activeChartTab, () => {
  nextTick(() => {
    createCharts();
  });
});

// Auto refresh mỗi 30 giây
setInterval(async () => {
  if (!loading.value) {
    await loadDashboardData();
  }
}, 30000);
</script>

<style scoped>
/* === MODERN DASHBOARD STYLES === */
.modern-dashboard {
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  position: relative;
}

/* Header với nền đỏ bordeaux */
.dashboard-header-bordeaux {
  background: linear-gradient(135deg, #722f37 0%, #8b1538 50%, #a91b60 100%);
  color: white;
  padding: 30px 0;
  position: relative;
  overflow: hidden;
  box-shadow: 0 8px 32px rgba(114, 47, 55, 0.3);
}

.header-bg-overlay {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: 
    radial-gradient(circle at 20% 50%, rgba(255, 255, 255, 0.1) 0%, transparent 50%),
    radial-gradient(circle at 80% 20%, rgba(255, 255, 255, 0.08) 0%, transparent 50%),
    radial-gradient(circle at 40% 80%, rgba(255, 255, 255, 0.06) 0%, transparent 50%);
  animation: shimmer 6s ease-in-out infinite;
}

@keyframes shimmer {
  0%, 100% { opacity: 0.5; }
  50% { opacity: 0.8; }
}

.header-content {
  max-width: 1400px;
  margin: 0 auto;
  padding: 0 30px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  position: relative;
  z-index: 2;
}

.header-left .title-section {
  display: flex;
  align-items: center;
  gap: 20px;
}

.icon-container {
  position: relative;
  width: 80px;
  height: 80px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 50%;
  backdrop-filter: blur(10px);
  border: 2px solid rgba(255, 255, 255, 0.2);
}

.icon-glow {
  position: absolute;
  top: -10px;
  left: -10px;
  right: -10px;
  bottom: -10px;
  background: radial-gradient(circle, rgba(255, 255, 255, 0.3) 0%, transparent 70%);
  border-radius: 50%;
  animation: pulse 2s ease-in-out infinite;
}

.dashboard-icon {
  font-size: 36px;
  position: relative;
  z-index: 1;
}

.title-wrapper {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.dashboard-title {
  font-size: 36px;
  font-weight: 700;
  margin: 0;
  background: linear-gradient(45deg, #ffffff 0%, #f0f0f0 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
  letter-spacing: 1px;
}

.dashboard-subtitle {
  font-size: 16px;
  opacity: 0.9;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 8px;
}

.live-indicator {
  display: flex;
  align-items: center;
  gap: 6px;
  background: rgba(255, 255, 255, 0.15);
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  backdrop-filter: blur(10px);
}

.pulse-dot {
  width: 8px;
  height: 8px;
  background: #52c41a;
  border-radius: 50%;
  animation: pulse-dot 1.5s ease-in-out infinite;
}

@keyframes pulse-dot {
  0%, 100% { transform: scale(1); opacity: 1; }
  50% { transform: scale(1.2); opacity: 0.7; }
}

.current-time {
  font-family: 'Courier New', monospace;
  font-size: 14px;
  opacity: 0.8;
  background: rgba(255, 255, 255, 0.1);
  padding: 8px 16px;
  border-radius: 25px;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  margin: 0;
}

/* Bộ lọc header */
.filter-panel {
  display: flex;
  gap: 20px;
  align-items: end;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.filter-label {
  font-size: 14px;
  font-weight: 500;
  opacity: 0.9;
}

.branch-selector,
.date-picker {
  min-width: 250px;
}

.branch-selector :deep(.el-input__wrapper),
.date-picker :deep(.el-input__wrapper) {
  background: rgba(255, 255, 255, 0.95);
  border: 1px solid rgba(255, 255, 255, 0.3);
  border-radius: 12px;
  backdrop-filter: blur(10px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.option-item {
  display: flex;
  align-items: center;
  gap: 8px;
}

.option-icon {
  font-size: 16px;
}

/* Dashboard content */
.dashboard-content {
  max-width: 1400px;
  margin: 0 auto;
  padding: 30px;
}

/* Tổng quan section */
.overview-section {
  margin-bottom: 30px;
}

.overview-card {
  background: white;
  border-radius: 16px;
  padding: 24px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  border: 1px solid rgba(114, 47, 55, 0.1);
}

.overview-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.overview-header h3 {
  margin: 0;
  font-size: 20px;
  font-weight: 600;
  color: #722f37;
}

.refresh-btn {
  width: 40px;
  height: 40px;
  background: linear-gradient(135deg, #722f37 0%, #8b1538 100%);
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
}

.refresh-btn:hover {
  transform: scale(1.1);
  box-shadow: 0 4px 12px rgba(114, 47, 55, 0.3);
}

.refresh-btn.spinning {
  animation: spin 1s linear infinite;
}

.overview-stats {
  display: flex;
  align-items: center;
  gap: 30px;
}

.stat-item {
  text-align: center;
}

.stat-value {
  font-size: 32px;
  font-weight: 700;
  color: #722f37;
  margin-bottom: 8px;
}

.stat-label {
  font-size: 14px;
  color: #666;
  opacity: 0.8;
}

.stat-divider {
  width: 1px;
  height: 40px;
  background: linear-gradient(to bottom, transparent 0%, #ddd 50%, transparent 100%);
}

.text-success {
  color: #52c41a !important;
}

/* Grid chỉ tiêu */
.indicators-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(380px, 1fr));
  gap: 24px;
  margin-bottom: 40px;
}

/* Card chỉ tiêu hiện đại */
.indicator-card-modern {
  background: white;
  border-radius: 20px;
  padding: 24px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  border: 1px solid rgba(114, 47, 55, 0.1);
  position: relative;
  overflow: hidden;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  cursor: pointer;
  animation: slideInUp 0.6s ease-out;
  animation-delay: var(--delay);
  animation-fill-mode: both;
}

.indicator-card-modern::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, var(--card-gradient-start), var(--card-gradient-end));
}

.indicator-card-modern.nguon-von {
  --card-gradient-start: #52c41a;
  --card-gradient-end: #95de64;
}

.indicator-card-modern.du-no {
  --card-gradient-start: #1890ff;
  --card-gradient-end: #69c0ff;
}

.indicator-card-modern.no-xau {
  --card-gradient-start: #fa541c;
  --card-gradient-end: #ff7a45;
}

.indicator-card-modern.thu-no-xlrr {
  --card-gradient-start: #722ed1;
  --card-gradient-end: #b37feb;
}

.indicator-card-modern.thu-dich-vu {
  --card-gradient-start: #13c2c2;
  --card-gradient-end: #5cdbd3;
}

.indicator-card-modern.tai-chinh {
  --card-gradient-start: #faad14;
  --card-gradient-end: #ffc53d;
}

.indicator-card-modern:hover {
  transform: translateY(-8px) scale(1.02);
  box-shadow: 0 16px 48px rgba(0, 0, 0, 0.15);
}

.indicator-card-modern.loading-pulse {
  animation: loading-pulse 1.5s ease-in-out infinite;
}

@keyframes loading-pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.7; }
}

@keyframes slideInUp {
  from {
    opacity: 0;
    transform: translateY(40px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Header của card */
.card-header-modern {
  display: flex;
  align-items: center;
  gap: 16px;
  margin-bottom: 20px;
}

.icon-wrapper {
  position: relative;
  width: 60px;
  height: 60px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 16px;
  background: linear-gradient(135deg, var(--card-gradient-start), var(--card-gradient-end));
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
}

.icon-bg {
  position: absolute;
  top: -5px;
  left: -5px;
  right: -5px;
  bottom: -5px;
  border-radius: 20px;
  background: linear-gradient(135deg, var(--card-gradient-start), var(--card-gradient-end));
  opacity: 0.3;
  filter: blur(8px);
}

.indicator-icon {
  font-size: 28px;
  position: relative;
  z-index: 1;
}

.header-text {
  flex: 1;
}

.indicator-title {
  font-size: 18px;
  font-weight: 600;
  color: #333;
  margin: 0 0 6px 0;
}

.status-badge {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.status-excellent {
  background: rgba(82, 196, 26, 0.1);
  color: #52c41a;
  border: 1px solid rgba(82, 196, 26, 0.3);
}

.status-good {
  background: rgba(24, 144, 255, 0.1);
  color: #1890ff;
  border: 1px solid rgba(24, 144, 255, 0.3);
}

.status-average {
  background: rgba(250, 173, 20, 0.1);
  color: #faad14;
  border: 1px solid rgba(250, 173, 20, 0.3);
}

.status-poor {
  background: rgba(245, 34, 45, 0.1);
  color: #f5222d;
  border: 1px solid rgba(245, 34, 45, 0.3);
}

/* Giá trị section */
.value-section {
  margin-bottom: 20px;
}

.main-value {
  display: flex;
  align-items: baseline;
  gap: 8px;
  margin-bottom: 12px;
}

.value-number {
  font-size: 32px;
  font-weight: 700;
  color: var(--card-gradient-start);
}

.value-unit {
  font-size: 16px;
  color: #666;
  font-weight: 500;
}

.change-indicator {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  border-radius: 12px;
  font-size: 14px;
  font-weight: 500;
}

.change-positive {
  background: rgba(82, 196, 26, 0.1);
  color: #52c41a;
  border: 1px solid rgba(82, 196, 26, 0.3);
}

.change-negative {
  background: rgba(245, 34, 45, 0.1);
  color: #f5222d;
  border: 1px solid rgba(245, 34, 45, 0.3);
}

.change-arrow {
  font-size: 16px;
}

/* Progress section */
.progress-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.circular-progress {
  flex-shrink: 0;
}

.progress-text {
  font-size: 14px;
  font-weight: 600;
  color: #333;
}

.target-actual {
  flex: 1;
  margin-left: 20px;
}

.target-row,
.actual-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
  font-size: 14px;
}

.target-row .label,
.actual-row .label {
  color: #666;
}

.target-row .value,
.actual-row .value {
  font-weight: 600;
  color: #333;
}

/* Mini chart */
.mini-chart {
  height: 60px;
  margin-top: 16px;
  padding-top: 16px;
  border-top: 1px solid rgba(0, 0, 0, 0.06);
}

.chart-container {
  width: 100%;
  height: 100%;
}

/* Charts section */
.charts-section {
  background: white;
  border-radius: 20px;
  padding: 30px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  border: 1px solid rgba(114, 47, 55, 0.1);
}

.charts-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
}

.charts-header h3 {
  margin: 0;
  font-size: 22px;
  font-weight: 600;
  color: #722f37;
}

.chart-tabs {
  display: flex;
  gap: 4px;
  background: #f5f7fa;
  padding: 4px;
  border-radius: 12px;
}

.chart-tab {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 20px;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  font-size: 14px;
  font-weight: 500;
  color: #666;
}

.chart-tab:hover {
  background: rgba(114, 47, 55, 0.1);
  color: #722f37;
}

.chart-tab.active {
  background: linear-gradient(135deg, #722f37 0%, #8b1538 100%);
  color: white;
  box-shadow: 0 2px 8px rgba(114, 47, 55, 0.3);
}

.tab-icon {
  font-size: 16px;
}

/* Chart panels */
.charts-content {
  min-height: 400px;
}

.chart-panel {
  width: 100%;
  height: 400px;
}

.chart-wrapper {
  width: 100%;
  height: 100%;
  border-radius: 12px;
  overflow: hidden;
}

.chart-container-large {
  width: 100%;
  height: 100%;
}

/* Modal styles */
.indicator-detail-modal :deep(.el-dialog) {
  border-radius: 16px;
  overflow: hidden;
}

.indicator-detail-modal :deep(.el-dialog__header) {
  background: linear-gradient(135deg, #722f37 0%, #8b1538 100%);
  color: white;
  padding: 20px 24px;
}

.detail-content {
  padding: 24px;
}

/* Animations */
@keyframes pulse {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.05); }
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

/* Responsive */
@media (max-width: 1200px) {
  .indicators-grid {
    grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  }
}

@media (max-width: 768px) {
  .header-content {
    flex-direction: column;
    gap: 20px;
    align-items: stretch;
  }
  
  .filter-panel {
    flex-direction: column;
    gap: 16px;
  }
  
  .dashboard-content {
    padding: 20px 16px;
  }
  
  .indicators-grid {
    grid-template-columns: 1fr;
    gap: 16px;
  }
  
  .overview-stats {
    flex-direction: column;
    gap: 16px;
  }
  
  .stat-divider {
    width: 40px;
    height: 1px;
  }
  
  .dashboard-title {
    font-size: 28px;
  }
  
  .charts-header {
    flex-direction: column;
    gap: 16px;
    align-items: stretch;
  }
  
  .chart-tabs {
    justify-content: center;
  }
}

@media (max-width: 480px) {
  .chart-tab {
    padding: 8px 12px;
    font-size: 12px;
  }
  
  .tab-text {
    display: none;
  }
  
  .indicator-card-modern {
    padding: 16px;
  }
  
  .value-number {
    font-size: 24px;
  }
}
</style>
