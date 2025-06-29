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
              <h1 class="dashboard-title">DASHBOARD CÁC CHỈ TIÊU KHKD</h1>
              <p class="dashboard-subtitle">
                <span class="subtitle-icon">📅</span>
                <span class="time-white">{{ getCurrentPeriodLabel() }}</span>
                <span class="live-indicator">
                  <span class="pulse-dot"></span>
                  <span class="realtime-white">Real-time</span>
                </span>
              </p>
              <p class="current-time">
                <span class="time-icon">⏰</span>
                <span class="time-white">{{ formatCurrentTime() }}</span>
              </p>
            </div>
          </div>
        </div>

        <div class="header-right">
          <!-- Bộ lọc nâng cao -->
          <div class="filter-panel">
            <div class="filter-group">
              <label for="branch-selector" class="filter-label-enhanced">
                <span class="label-icon">🏢</span>
                <span class="label-text">Chi nhánh</span>
              </label>
              <el-select
                id="branch-selector"
                v-model="selectedBranch"
                placeholder="Chọn chi nhánh để phân tích"
                @change="handleBranchChange"
                @focus="isUserInteraction = true"
                :loading="loading"
                filterable
                clearable
                class="branch-selector-enhanced"
                size="large"
                autocomplete="organization"
                aria-label="Chọn chi nhánh"
              >
                <el-option
                  v-for="branch in branches"
                  :key="branch.id"
                  :label="branch.name"
                  :value="branch.id"
                >
                  <div class="option-item-enhanced">
                    <span class="option-icon">🏢</span>
                    <span class="option-text">{{ branch.name }}</span>
                    <span class="option-badge">{{ getBranchStatus(branch.id) }}</span>
                  </div>
                </el-option>
              </el-select>
            </div>

            <div class="filter-group">
              <label for="date-range-picker" class="filter-label-enhanced">
                <span class="label-icon">📅</span>
                <span class="label-text">Thời gian</span>
              </label>
              <el-date-picker
                id="date-range-picker"
                v-model="dateRange"
                type="monthrange"
                start-placeholder="Từ tháng"
                end-placeholder="Đến tháng"
                format="MM/YYYY"
                value-format="YYYY-MM"
                @change="handleDateRangeChange"
                class="date-picker-enhanced"
                autocomplete="off"
                aria-label="Chọn khoảng thời gian"
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
    <div class="dashboard-content">
      <!-- Tổng quan nhanh -->
      <div class="overview-section">
        <div class="overview-card">
          <div class="overview-header">
            <h3>🎯 Tổng quan hiệu suất của {{ getSelectedBranchName() }}</h3>
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
              <span
                :ref="`counter-${indicator.id}`"
                class="value-number animated-counter"
                :data-target="indicator.currentValue"
                :style="{ color: getProgressColor(indicator.completionRate) }"
              >
                {{ animatedValues[indicator.id] || indicator.currentValue }}
              </span>
              <span class="value-unit">{{ indicator.unit }}</span>
            </div>

            <!-- Tăng giảm so với đầu năm và đầu tháng -->
            <div class="changes-container">
              <!-- So với đầu năm -->
              <div class="change-indicator" :class="getChangeClass(indicator.changeFromYearStartPercent, indicator.id)">
                <span class="change-arrow">{{ getChangeArrow(indicator.changeFromYearStartPercent, indicator.id) }}</span>
                <span class="change-text">
                  {{ formatChangePercent(indicator.changeFromYearStartPercent) }} so với đầu năm
                </span>
              </div>

              <!-- So với đầu tháng (mới thêm) -->
              <div class="change-indicator" :class="getChangeClass(indicator.changeFromMonthStart || 0, indicator.id)">
                <span class="change-arrow">{{ getChangeArrow(indicator.changeFromMonthStart || 0, indicator.id) }}</span>
                <span class="change-text">
                  {{ formatChangePercent(indicator.changeFromMonthStart || 0) }} so với đầu tháng
                </span>
              </div>
            </div>
          </div>

          <!-- Progress section với dual charts -->
          <div class="progress-section-dual">
            <!-- So với kế hoạch năm -->
            <div class="progress-item">
              <div class="progress-header">
                <span class="progress-label">Kế hoạch năm</span>
                <span class="progress-percentage">{{ Math.round(indicator.completionRate) }}%</span>
              </div>
              <el-progress
                type="circle"
                :percentage="Math.min(indicator.completionRate, 100)"
                :width="65"
                :stroke-width="6"
                :color="getProgressColor(indicator.completionRate)"
                class="progress-chart-small"
              >
                <template #default="{ percentage }">
                  <span class="progress-text-small">{{ Math.round(percentage) }}%</span>
                </template>
              </el-progress>
            </div>

            <!-- So với kế hoạch quý (mới thêm) -->
            <div class="progress-item">
              <div class="progress-header">
                <span class="progress-label">Kế hoạch quý</span>
                <span class="progress-percentage">{{ Math.round(indicator.quarterCompletionRate || 0) }}%</span>
              </div>
              <el-progress
                type="circle"
                :percentage="Math.min(indicator.quarterCompletionRate || 0, 100)"
                :width="65"
                :stroke-width="6"
                :color="getProgressColor(indicator.quarterCompletionRate || 0)"
                class="progress-chart-small"
              >
                <template #default="{ percentage }">
                  <span class="progress-text-small">{{ Math.round(percentage) }}%</span>
                </template>
              </el-progress>
            </div>
          </div>

          <!-- Mini chart trong card -->
          <div class="mini-chart">
            <div class="chart-container" :id="`mini-chart-${indicator.id}`"></div>
          </div>
        </div>
      </div>

      <!-- Biểu đồ chi tiết -->
      <div class="charts-section">
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

    <!-- Modal chi tiết chỉ tiêu với popup overlay -->
    <el-dialog
      v-model="showDetailModal"
      :title="`Chi tiết chỉ tiêu: ${selectedIndicator?.name}`"
      width="80%"
      class="indicator-detail-modal-enhanced"
      :show-close="false"
      :close-on-click-modal="true"
      center
    >
      <div v-if="selectedIndicator" class="detail-content-enhanced">
        <!-- Header với icon và tổng quan -->
        <div class="detail-header">
          <div class="detail-title-section">
            <span class="detail-icon">{{ selectedIndicator.icon }}</span>
            <div class="detail-title-text">
              <h2>{{ selectedIndicator.name }}</h2>
              <p class="detail-subtitle">Phân tích chi tiết tăng giảm và xu hướng</p>
            </div>
          </div>
          <button @click="showDetailModal = false" class="close-btn-enhanced">
            <i class="close-icon">✕</i>
          </button>
        </div>

        <!-- Tổng quan số liệu chính -->
        <div class="detail-overview">
          <div class="overview-card-detail">
            <div class="overview-label">Thực hiện</div>
            <div class="overview-value current">{{ formatNumber(selectedIndicator.currentValue) }} {{ selectedIndicator.unit }}</div>
          </div>
          <div class="overview-card-detail">
            <div class="overview-label">Kế hoạch năm</div>
            <div class="overview-value target">{{ formatNumber(selectedIndicator.targetValue) }} {{ selectedIndicator.unit }}</div>
          </div>
          <div class="overview-card-detail">
            <div class="overview-label">Hoàn thành</div>
            <div class="overview-value completion" :class="getCompletionClass(selectedIndicator.completionRate)">
              {{ selectedIndicator.completionRate.toFixed(1) }}%
            </div>
          </div>
          <div class="overview-card-detail">
            <div class="overview-label">Kế hoạch quý</div>
            <div class="overview-value quarter" :class="getCompletionClass(selectedIndicator.quarterCompletionRate || 0)">
              {{ (selectedIndicator.quarterCompletionRate || 0).toFixed(1) }}%
            </div>
          </div>
        </div>

        <!-- Phân tích tăng giảm chi tiết -->
        <div class="detail-analysis">
          <div class="analysis-section">
            <h3 class="analysis-title">📈 Phân tích tăng giảm so với đầu năm</h3>
            <div class="analysis-content">
              <div class="change-detail-card positive">
                <div class="change-header">
                  <span class="change-icon">{{ getChangeArrow(selectedIndicator.changeFromYearStartPercent, selectedIndicator.id) }}</span>
                  <span class="change-title">So với đầu năm</span>
                </div>
                <div class="change-stats">
                  <div class="change-percentage">{{ formatChangePercent(selectedIndicator.changeFromYearStartPercent) }}</div>
                  <div class="change-description">{{ getChangeDescription(selectedIndicator.changeFromYearStartPercent, 'năm') }}</div>
                </div>

                <!-- Danh sách khách hàng/cán bộ góp phần -->
                <div class="contributors-section">
                  <h4>🏆 Đóng góp tích cực:</h4>
                  <div class="contributors-list">
                    <div v-for="contributor in getTopContributors(selectedIndicator.id, 'year')" :key="contributor.id" class="contributor-item positive">
                      <span class="contributor-name">{{ contributor.name }}</span>
                      <span class="contributor-value">+{{ formatNumber(contributor.contribution) }}</span>
                    </div>
                  </div>
                </div>
              </div>

              <div class="change-detail-card neutral">
                <div class="change-header">
                  <span class="change-icon">{{ getChangeArrow(selectedIndicator.changeFromMonthStart || 0, selectedIndicator.id) }}</span>
                  <span class="change-title">So với đầu tháng</span>
                </div>
                <div class="change-stats">
                  <div class="change-percentage">{{ formatChangePercent(selectedIndicator.changeFromMonthStart || 0) }}</div>
                  <div class="change-description">{{ getChangeDescription(selectedIndicator.changeFromMonthStart || 0, 'tháng') }}</div>
                </div>

                <!-- Danh sách khách hàng/cán bộ góp phần -->
                <div class="contributors-section">
                  <h4>📊 Đóng góp trong tháng:</h4>
                  <div class="contributors-list">
                    <div v-for="contributor in getTopContributors(selectedIndicator.id, 'month')" :key="contributor.id" class="contributor-item neutral">
                      <span class="contributor-name">{{ contributor.name }}</span>
                      <span class="contributor-value">{{ contributor.contribution > 0 ? '+' : '' }}{{ formatNumber(contributor.contribution) }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Biểu đồ xu hướng trong modal -->
          <div class="detail-chart-section">
            <h3 class="chart-title">📈 Xu hướng 12 tháng gần nhất</h3>
            <div class="detail-chart-container">
              <div id="detail-trend-chart" style="height: 300px;"></div>
            </div>
          </div>
        </div>

        <!-- Action buttons -->
        <div class="detail-actions">
          <el-button @click="exportIndicatorDetail" type="primary" size="large">
            📊 Xuất báo cáo chi tiết
          </el-button>
          <el-button @click="showDetailModal = false" size="large">
            Đóng
          </el-button>
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<script setup>
import dayjs from 'dayjs';
import * as echarts from 'echarts';
import { ElDialog, ElMessage } from 'element-plus';
import { computed, nextTick, onBeforeUnmount, onMounted, ref, watch } from 'vue';
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
const animatedValues = ref({}); // Giá trị animated cho counters

// Danh sách 15 chi nhánh chuẩn hóa theo quy ước mới
const branches = ref([
  { id: 'CnLaiChau', name: 'CN Lai Châu' },
  { id: 'HoiSo', name: 'Hội Sở' },
  { id: 'CnTamDuong', name: 'CN Tam Đường' },
  { id: 'CnPhongTho', name: 'CN Phong Thổ' },
  { id: 'CnSinHo', name: 'CN Sin Hồ' },
  { id: 'CnMuongTe', name: 'CN Mường Tè' },
  { id: 'CnThanUyen', name: 'CN Than Uyên' },
  { id: 'CnThanhPho', name: 'CN Thành Phố' },
  { id: 'CnTanUyen', name: 'CN Tân Uyên' },
  { id: 'CnNamNhun', name: 'CN Nậm Nhùn' },
  { id: 'CnPhongThoPgdMuongSo', name: 'CN Phong Thổ - PGD Mường So' },
  { id: 'CnThanUyenPgdMuongThan', name: 'CN Than Uyên - PGD Mường Than' },
  { id: 'CnThanhPhoPgdSo1', name: 'CN Thành Phố - PGD Số 1' },
  { id: 'CnThanhPhoPgdSo2', name: 'CN Thành Phố - PGD Số 2' },
  { id: 'CnTanUyenPgdSo3', name: 'CN Tân Uyên - PGD Số 3' }
]);

// 6 chỉ tiêu dashboard chính với dữ liệu đầy đủ
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
    quarterTargetValue: 300.0,
    completionRate: 104.2,
    quarterCompletionRate: 112.5,
    changeFromYearStart: 125.3,
    changeFromYearStartPercent: 11.2,
    changeFromMonthStart: 35.7,
    changeFromMonthStartPercent: 2.9
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
    quarterTargetValue: 250.0,
    completionRate: 98.0,
    quarterCompletionRate: 105.2,
    changeFromYearStart: 45.8,
    changeFromYearStartPercent: 4.9,
    changeFromMonthStart: 12.4,
    changeFromMonthStartPercent: 1.3
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
    quarterTargetValue: 1.9,
    completionRate: 90.0,
    quarterCompletionRate: 94.7,
    changeFromYearStart: -0.3,
    changeFromYearStartPercent: -14.3,
    changeFromMonthStart: -0.1,
    changeFromMonthStartPercent: -5.3
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
    quarterTargetValue: 12.5,
    completionRate: 91.4,
    quarterCompletionRate: 109.8,
    changeFromYearStart: 8.2,
    changeFromYearStartPercent: 21.9,
    changeFromMonthStart: 2.8,
    changeFromMonthStartPercent: 6.5
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
    quarterTargetValue: 7.5,
    completionRate: 96.3,
    quarterCompletionRate: 115.7,
    changeFromYearStart: 3.1,
    changeFromYearStartPercent: 12.0,
    changeFromMonthStart: 1.2,
    changeFromMonthStartPercent: 4.3
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
    quarterTargetValue: 40.0,
    completionRate: 97.8,
    quarterCompletionRate: 117.3,
    changeFromYearStart: 18.6,
    changeFromYearStartPercent: 13.5,
    changeFromMonthStart: 4.9,
    changeFromMonthStartPercent: 3.2
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

// Cấu hình animated counters tự tạo
const animateCounter = (indicatorId, targetValue, duration = 2000) => {
  const startValue = animatedValues.value[indicatorId] || 0;
  const startTime = Date.now();

  const animate = () => {
    const currentTime = Date.now();
    const elapsed = currentTime - startTime;
    const progress = Math.min(elapsed / duration, 1);

    // Sử dụng easing function cho animation mượt
    const easeOutQuart = 1 - Math.pow(1 - progress, 4);
    const currentValue = startValue + (targetValue - startValue) * easeOutQuart;

    animatedValues.value[indicatorId] = Math.round(currentValue * 10) / 10;

    if (progress < 1) {
      requestAnimationFrame(animate);
    } else {
      animatedValues.value[indicatorId] = targetValue;
    }
  };

  animate();
};

// Khởi động animation cho tất cả counters
const startAllCounterAnimations = () => {
  indicators.value.forEach(indicator => {
    animateCounter(indicator.id, indicator.currentValue);
  });
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

const getChangeClass = (percent, indicatorId = null) => {
  // Xử lý đặc biệt cho chỉ tiêu Nợ xấu (ngược lại với các chỉ tiêu khác)
  if (indicatorId === 'no_xau') {
    return percent >= 0 ? 'change-negative' : 'change-positive'; // Nợ xấu tăng = xấu (đỏ), giảm = tốt (xanh)
  }
  // Các chỉ tiêu khác: tăng = tốt (xanh), giảm = xấu (đỏ)
  return percent >= 0 ? 'change-positive' : 'change-negative';
};

const getChangeArrow = (percent, indicatorId = null) => {
  // Arrow không thay đổi - vẫn hiển thị đúng hướng tăng/giảm
  return percent >= 0 ? '↗️' : '↘️';
};

const getProgressColor = (percentage) => {
  // Thống nhất màu sắc theo yêu cầu:
  // < 25%: màu đỏ đậm (bordeaux)
  // 26-50%: màu cam nhạt
  // 51-75%: màu xanh dương nhạt
  // > 75%: màu xanh lá cây
  if (percentage >= 75) return '#52c41a'; // Xanh lá cây
  if (percentage >= 51) return '#1890ff'; // Xanh dương nhạt
  if (percentage >= 26) return '#faad14'; // Cam nhạt
  return '#8B1538'; // Đỏ đậm bordeaux (< 25%)
};

// Xử lý sự kiện
const isUserInteraction = ref(false);

const handleBranchChange = async () => {
  // Chỉ phát âm thanh khi user chủ động thay đổi qua UI
  if (isUserInteraction.value) {
    playClickSound();
    isUserInteraction.value = false; // Reset flag
  }
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

    // Khởi động animation cho counters
    setTimeout(() => {
      startAllCounterAnimations();
    }, 300);

    // Tạo biểu đồ sau khi có dữ liệu với delay để đảm bảo DOM
    await nextTick();
    setTimeout(() => {
      createCharts();
    }, 200);

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

// Tạo biểu đồ với ECharts (có error handling và delay)
const createCharts = async () => {
  try {
    // Đợi DOM render hoàn toàn
    await nextTick();

    // Thêm delay nhỏ để đảm bảo tất cả element đã sẵn sàng
    setTimeout(() => {
      createComparisonChart();
      createTrendChart();
      createCompletionChart();
      createMiniCharts();
    }, 100);

  } catch (error) {
    console.warn('Error creating charts:', error);
  }
};

const createComparisonChart = () => {
  try {
    const chartDom = document.getElementById('comparison-chart');
    if (!chartDom || !chartDom.parentNode) {
      console.log('⚠️ Comparison chart container not ready, skipping...');
      return;
    }

    // Dispose existing chart instance nếu có
    const existingChart = echarts.getInstanceByDom(chartDom);
    if (existingChart) {
      existingChart.dispose();
    }

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
  } catch (error) {
    console.warn('Error creating comparison chart:', error);
  }
};

const createTrendChart = () => {
  try {
    const chartDom = document.getElementById('trend-chart');
    if (!chartDom || !chartDom.parentNode) {
      // Bỏ log để tránh spam console
      return;
    }

    // Dispose existing chart instance nếu có
    const existingChart = echarts.getInstanceByDom(chartDom);
    if (existingChart) {
      existingChart.dispose();
    }

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
  } catch (error) {
    // Chỉ log lỗi thực sự, bỏ warning để tránh spam console
    console.error('Error creating trend chart:', error);
  }
};

const createCompletionChart = () => {
  try {
    const chartDom = document.getElementById('completion-chart');
    if (!chartDom || !chartDom.parentNode) {
      console.log('⚠️ Completion chart container not ready, skipping...');
      return;
    }

    // Dispose existing chart instance nếu có
    const existingChart = echarts.getInstanceByDom(chartDom);
    if (existingChart) {
      existingChart.dispose();
    }

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
  } catch (error) {
    console.warn('Error creating completion chart:', error);
  }
};

const createMiniCharts = () => {
  try {
    indicators.value.forEach(indicator => {
      const chartDom = document.getElementById(`mini-chart-${indicator.id}`);
      if (!chartDom || !chartDom.parentNode) {
        console.log(`⚠️ Mini chart container not ready for ${indicator.id}, skipping...`);
        return;
      }

      // Dispose existing chart instance nếu có
      const existingChart = echarts.getInstanceByDom(chartDom);
      if (existingChart) {
        existingChart.dispose();
      }

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
  } catch (error) {
    console.warn('Error creating mini charts:', error);
  }
};

// ==================== CÁC FUNCTION CHO POPUP CHI TIẾT ====================

// Mock dữ liệu contributors cho từng chỉ tiêu
const contributorsData = ref({
  nguon_von: {
    year: [
      { id: 1, name: 'Nguyễn Văn A - CN Lai Châu', contribution: 85.2 },
      { id: 2, name: 'Trần Thị B - CN Tam Đường', contribution: 67.5 },
      { id: 3, name: 'Lê Văn C - Hội Sở', contribution: 54.8 },
      { id: 4, name: 'Phạm Thị D - CN Phong Thổ', contribution: 43.2 },
      { id: 5, name: 'Hoàng Văn E - CN Sin Hồ', contribution: 38.7 }
    ],
    month: [
      { id: 1, name: 'Nguyễn Văn A - CN Lai Châu', contribution: 12.5 },
      { id: 2, name: 'Trần Thị B - CN Tam Đường', contribution: 8.7 },
      { id: 3, name: 'Lê Văn C - Hội Sở', contribution: 6.9 },
      { id: 4, name: 'Phạm Thị D - CN Phong Thổ', contribution: 5.2 },
      { id: 5, name: 'Hoàng Văn E - CN Sin Hồ', contribution: 2.4 }
    ]
  },
  du_no: {
    year: [
      { id: 1, name: 'KH Công ty TNHH ABC', contribution: 25.3 },
      { id: 2, name: 'KH Doanh nghiệp XYZ', contribution: 18.7 },
      { id: 3, name: 'KH Cá nhân Nguyễn Văn M', contribution: 12.4 },
      { id: 4, name: 'KH HTX Nông nghiệp DEF', contribution: 9.8 },
      { id: 5, name: 'KH Cửa hàng GHI', contribution: 7.2 }
    ],
    month: [
      { id: 1, name: 'KH Công ty TNHH ABC', contribution: 4.2 },
      { id: 2, name: 'KH Doanh nghiệp XYZ', contribution: 3.1 },
      { id: 3, name: 'KH Cá nhân Nguyễn Văn M', contribution: 2.8 },
      { id: 4, name: 'KH HTX Nông nghiệp DEF', contribution: 1.5 },
      { id: 5, name: 'KH Cửa hàng GHI', contribution: 0.8 }
    ]
  },
  no_xau: {
    year: [
      { id: 1, name: 'Giảm nợ nhóm 3-4-5 (KH ABC)', contribution: -0.12 },
      { id: 2, name: 'Thu hồi nợ quá hạn (KH XYZ)', contribution: -0.08 },
      { id: 3, name: 'Xử lý tài sản đảm bảo', contribution: -0.06 },
      { id: 4, name: 'Tái cơ cấu thành công', contribution: -0.04 },
      { id: 5, name: 'Thanh toán trước hạn', contribution: -0.03 }
    ],
    month: [
      { id: 1, name: 'Giảm nợ nhóm 3-4-5 (KH ABC)', contribution: -0.04 },
      { id: 2, name: 'Thu hồi nợ quá hạn (KH XYZ)', contribution: -0.03 },
      { id: 3, name: 'Xử lý tài sản đảm bảo', contribution: -0.02 },
      { id: 4, name: 'Tái cơ cấu thành công', contribution: -0.01 },
      { id: 5, name: 'Thanh toán trước hạn', contribution: -0.01 }
    ]
  },
  thu_no_xlrr: {
    year: [
      { id: 1, name: 'Bán đấu giá TS bảo đảm KH ABC', contribution: 3.2 },
      { id: 2, name: 'Thu từ bảo lãnh KH XYZ', contribution: 2.8 },
      { id: 3, name: 'Thanh lý hợp đồng bảo hiểm', contribution: 1.5 },
      { id: 4, name: 'Thu từ người thứ ba', contribution: 0.7 },
      { id: 5, name: 'Thu hồi từ tài khoản phong tỏa', contribution: 0.5 }
    ],
    month: [
      { id: 1, name: 'Bán đấu giá TS bảo đảm KH ABC', contribution: 1.1 },
      { id: 2, name: 'Thu từ bảo lãnh KH XYZ', contribution: 0.9 },
      { id: 3, name: 'Thanh lý hợp đồng bảo hiểm', contribution: 0.5 },
      { id: 4, name: 'Thu từ người thứ ba', contribution: 0.2 },
      { id: 5, name: 'Thu hồi từ tài khoản phong tỏa', contribution: 0.1 }
    ]
  },
  thu_dich_vu: {
    year: [
      { id: 1, name: 'Phí giao dịch chuyển tiền', contribution: 1.2 },
      { id: 2, name: 'Phí dịch vụ thẻ', contribution: 0.9 },
      { id: 3, name: 'Phí bảo hiểm ngân hàng', contribution: 0.6 },
      { id: 4, name: 'Phí tư vấn tài chính', contribution: 0.3 },
      { id: 5, name: 'Phí dịch vụ khác', contribution: 0.1 }
    ],
    month: [
      { id: 1, name: 'Phí giao dịch chuyển tiền', contribution: 0.4 },
      { id: 2, name: 'Phí dịch vụ thẻ', contribution: 0.3 },
      { id: 3, name: 'Phí bảo hiểm ngân hàng', contribution: 0.2 },
      { id: 4, name: 'Phí tư vấn tài chính', contribution: 0.2 },
      { id: 5, name: 'Phí dịch vụ khác', contribution: 0.1 }
    ]
  },
  tai_chinh: {
    year: [
      { id: 1, name: 'Lãi từ cho vay khách hàng', contribution: 12.8 },
      { id: 2, name: 'Lãi từ đầu tư trái phiếu', contribution: 3.2 },
      { id: 3, name: 'Lãi tiền gửi ngân hàng khác', contribution: 1.8 },
      { id: 4, name: 'Thu nhập từ hoạt động khác', contribution: 0.8 },
      { id: 5, name: 'Lãi từ đầu tư cổ phiếu', contribution: 0.6 }
    ],
    month: [
      { id: 1, name: 'Lãi từ cho vay khách hàng', contribution: 3.2 },
      { id: 2, name: 'Lãi từ đầu tư trái phiếu', contribution: 0.8 },
      { id: 3, name: 'Lãi tiền gửi ngân hàng khác', contribution: 0.5 },
      { id: 4, name: 'Thu nhập từ hoạt động khác', contribution: 0.3 },
      { id: 5, name: 'Lãi từ đầu tư cổ phiếu', contribution: 0.1 }
    ]
  }
});

// Function lấy top contributors cho modal
const getTopContributors = (indicatorId, period = 'year') => {
  const data = contributorsData.value[indicatorId];
  if (!data) return [];

  return (data[period] || []).slice(0, 5); // Top 5 contributors
};

// Function mô tả thay đổi
const getChangeDescription = (changePercent, period) => {
  const absChange = Math.abs(changePercent);
  let level = '';

  if (absChange >= 20) level = 'mạnh';
  else if (absChange >= 10) level = 'vừa phải';
  else if (absChange >= 5) level = 'nhẹ';
  else level = 'ít';

  const direction = changePercent >= 0 ? 'tăng' : 'giảm';
  return `${direction.toUpperCase()} ${level} so với đầu ${period}`;
};

// Function lấy class completion
const getCompletionClass = (rate) => {
  if (rate >= 100) return 'excellent';
  if (rate >= 90) return 'good';
  if (rate >= 70) return 'average';
  return 'poor';
};

// Function lấy status branch (mặc định)
const getBranchStatus = (branchId) => {
  // Mock status cho demo
  const statuses = ['Hoạt động', 'Tốt', 'Khá tốt', 'Cần cải thiện'];
  return statuses[Math.floor(Math.random() * statuses.length)];
};

// Function export báo cáo chi tiết (có thể mở rộng sau)
const exportIndicatorDetail = () => {
  ElMessage.success({
    message: `Đang xuất báo cáo chi tiết cho "${selectedIndicator.value?.name}"...`,
    type: 'success',
    duration: 2000
  });

  // TODO: Implement actual export logic
  console.log('Exporting detail for:', selectedIndicator.value);
};

// Function tạo biểu đồ xu hướng trong modal
const createDetailTrendChart = () => {
  try {
    const chartDom = document.getElementById('detail-trend-chart');
    if (!chartDom || !selectedIndicator.value) return;

    const existingChart = echarts.getInstanceByDom(chartDom);
    if (existingChart) {
      existingChart.dispose();
    }

    const myChart = echarts.init(chartDom);

    // Mock data xu hướng 12 tháng
    const months = ['T1', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7', 'T8', 'T9', 'T10', 'T11', 'T12'];
    const currentValue = selectedIndicator.value.currentValue;
    const trendData = months.map((_, index) => {
      const variation = (Math.random() - 0.5) * 0.3; // ±15% variation
      return currentValue * (0.85 + (index * 0.02) + variation);
    });

    const option = {
      title: {
        text: `Xu hướng 12 tháng - ${selectedIndicator.value.name}`,
        left: 'center',
        textStyle: { fontSize: 16, fontWeight: 'bold', color: '#722f37' }
      },
      tooltip: {
        trigger: 'axis',
        axisPointer: { type: 'cross' },
        formatter: function (params) {
          return `${params[0].name}: ${params[0].value.toFixed(2)} ${selectedIndicator.value.unit}`;
        }
      },
      grid: {
        left: '3%',
        right: '4%',
        bottom: '3%',
        top: '15%',
        containLabel: true
      },
      xAxis: {
        type: 'category',
        boundaryGap: false,
        data: months,
        axisLine: { lineStyle: { color: '#722f37' } }
      },
      yAxis: {
        type: 'value',
        axisLine: { lineStyle: { color: '#722f37' } },
        axisLabel: {
          formatter: function (value) {
            return `${value.toFixed(1)}${selectedIndicator.value.unit}`;
          }
        }
      },
      series: [
        {
          name: selectedIndicator.value.name,
          type: 'line',
          smooth: true,
          symbol: 'circle',
          symbolSize: 6,
          data: trendData,
          lineStyle: {
            color: '#722f37',
            width: 3
          },
          areaStyle: {
            color: {
              type: 'linear',
              x: 0, y: 0, x2: 0, y2: 1,
              colorStops: [
                { offset: 0, color: 'rgba(114, 47, 55, 0.3)' },
                { offset: 1, color: 'rgba(114, 47, 55, 0.05)' }
              ]
            }
          },
          markLine: {
            data: [
              {
                yAxis: selectedIndicator.value.targetValue,
                name: 'Kế hoạch năm',
                lineStyle: { color: '#ff4d4f', type: 'dashed', width: 2 }
              }
            ],
            label: {
              formatter: 'Kế hoạch: {c}' + selectedIndicator.value.unit
            }
          }
        }
      ]
    };

    myChart.setOption(option);
  } catch (error) {
    console.error('Error creating detail trend chart:', error);
  }
};

// Watch cho modal để tạo biểu đồ khi mở
watch(showDetailModal, (newValue) => {
  if (newValue && selectedIndicator.value) {
    nextTick(() => {
      setTimeout(() => {
        createDetailTrendChart();
      }, 300); // Delay để đảm bảo modal đã render
    });
  }
});

// ==================== KẾT THÚC POPUP CHI TIẾT ====================

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

  // Window resize handler cho charts
  window.addEventListener('resize', handleWindowResize);
});

onBeforeUnmount(() => {
  // Cleanup window resize listener
  window.removeEventListener('resize', handleWindowResize);

  // Dispose all chart instances
  try {
    ['comparison-chart', 'trend-chart', 'completion-chart'].forEach(id => {
      const chartDom = document.getElementById(id);
      if (chartDom) {
        const chartInstance = echarts.getInstanceByDom(chartDom);
        if (chartInstance) {
          chartInstance.dispose();
        }
      }
    });

    indicators.value.forEach(indicator => {
      const chartDom = document.getElementById(`mini-chart-${indicator.id}`);
      if (chartDom) {
        const chartInstance = echarts.getInstanceByDom(chartDom);
        if (chartInstance) {
          chartInstance.dispose();
        }
      }
    });
  } catch (error) {
    console.warn('Error disposing charts:', error);
  }
});

// Window resize handler
const handleWindowResize = () => {
  setTimeout(() => {
    try {
      // Resize all chart instances
      ['comparison-chart', 'trend-chart', 'completion-chart'].forEach(id => {
        const chartDom = document.getElementById(id);
        if (chartDom) {
          const chartInstance = echarts.getInstanceByDom(chartDom);
          if (chartInstance) {
            chartInstance.resize();
          }
        }
      });

      indicators.value.forEach(indicator => {
        const chartDom = document.getElementById(`mini-chart-${indicator.id}`);
        if (chartDom) {
          const chartInstance = echarts.getInstanceByDom(chartDom);
          if (chartInstance) {
            chartInstance.resize();
          }
        }
      });
    } catch (error) {
      console.warn('Error resizing charts:', error);
    }
  }, 100);
};

// Phương thức tiện ích
const updateTime = () => {
  currentTime.value = new Date();
};

// Lấy tên chi nhánh đã chọn
const getSelectedBranchName = () => {
  if (!selectedBranch.value) return 'Toàn hệ thống';
  const branch = branches.value.find(b => b.id === selectedBranch.value);
  return branch ? branch.name : 'Toàn hệ thống';
};

// Watch thay đổi branch
watch(selectedBranch, handleBranchChange);

// Watch thay đổi tab biểu đồ với delay và kiểm tra để tránh log spam
watch(activeChartTab, (newTab, oldTab) => {
  // Chỉ tạo lại chart khi tab thực sự thay đổi và không phải lần đầu load
  if (newTab !== oldTab && oldTab !== undefined) {
    nextTick(() => {
      setTimeout(() => {
        createCharts();
      }, 150);
    });
  }
});

// Bỏ auto refresh để tránh audio spam và log liên tục
// Auto refresh đã được bỏ để tránh âm thanh và log không mong muốn
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
  color: white;
  font-weight: bold;
  opacity: 1;
  background: rgba(255, 255, 255, 0.1);
  padding: 8px 16px;
  border-radius: 25px;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  margin: 0;
}

/* Bộ lọc header - Enhanced */
.filter-panel {
  display: flex;
  gap: 25px;
  align-items: end;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

/* Enhanced filter labels */
.filter-label-enhanced {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  font-weight: 600;
  color: white;
  background: rgba(255, 255, 255, 0.15);
  padding: 6px 12px;
  border-radius: 20px;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.2);
  transition: all 0.3s ease;
  cursor: default;
}

.filter-label-enhanced:hover {
  background: rgba(255, 255, 255, 0.25);
  transform: translateY(-1px);
}

.label-icon {
  font-size: 16px;
  filter: brightness(1.2);
}

.label-text {
  font-weight: 600;
  letter-spacing: 0.5px;
}

.time-white, .realtime-white {
  color: white !important;
  font-weight: 700 !important;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
  filter: brightness(1.1);
}

.subtitle-icon, .time-icon {
  filter: brightness(1.3) contrast(1.2);
}

/* Enhanced selectors */
.branch-selector-enhanced,
.date-picker-enhanced {
  min-width: 280px;
}

.branch-selector-enhanced :deep(.el-input__wrapper),
.date-picker-enhanced :deep(.el-input__wrapper) {
  background: rgba(255, 255, 255, 0.95);
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-radius: 15px;
  backdrop-filter: blur(15px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
  transition: all 0.3s ease;
}

.branch-selector-enhanced :deep(.el-input__wrapper):hover,
.date-picker-enhanced :deep(.el-input__wrapper):hover {
  border-color: rgba(255, 255, 255, 0.6);
  box-shadow: 0 12px 35px rgba(0, 0, 0, 0.2);
  transform: translateY(-2px);
}

.branch-selector-enhanced :deep(.el-input__wrapper):focus-within,
.date-picker-enhanced :deep(.el-input__wrapper):focus-within {
  border-color: white;
  box-shadow: 0 0 0 3px rgba(255, 255, 255, 0.3);
}

/* Enhanced option items */
.option-item-enhanced {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 8px 0;
}

.option-icon {
  font-size: 16px;
  width: 20px;
  text-align: center;
}

.option-text {
  flex: 1;
  font-weight: 500;
}

.option-badge {
  background: linear-gradient(135deg, #722f37 0%, #8b1538 100%);
  color: white;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 10px;
  font-weight: 600;
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

.animated-counter {
  transition: all 0.3s ease;
  display: inline-block;
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

/* Modal styles - Enhanced popup overlay */
.indicator-detail-modal-enhanced :deep(.el-dialog) {
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 25px 50px rgba(0, 0, 0, 0.25);
  backdrop-filter: blur(10px);
}

.indicator-detail-modal-enhanced :deep(.el-dialog__header) {
  background: linear-gradient(135deg, #722f37 0%, #8b1538 50%, #a91b60 100%);
  color: white;
  padding: 0;
  border: none;
}

.indicator-detail-modal-enhanced :deep(.el-dialog__body) {
  padding: 0;
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
}

/* Detail content styles */
.detail-content-enhanced {
  min-height: 500px;
}

.detail-header {
  background: linear-gradient(135deg, #722f37 0%, #8b1538 50%, #a91b60 100%);
  color: white;
  padding: 25px 30px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  position: relative;
  overflow: hidden;
}

.detail-header::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: radial-gradient(circle at 20% 50%, rgba(255, 255, 255, 0.1) 0%, transparent 50%);
  pointer-events: none;
}

.detail-title-section {
  display: flex;
  align-items: center;
  gap: 15px;
  position: relative;
  z-index: 2;
}

.detail-icon {
  font-size: 32px;
  background: rgba(255, 255, 255, 0.15);
  padding: 12px;
  border-radius: 50%;
  backdrop-filter: blur(10px);
}

.detail-title-text h2 {
  margin: 0;
  font-size: 24px;
  font-weight: 700;
  color: white;
}

.detail-subtitle {
  margin: 5px 0 0 0;
  font-size: 14px;
  opacity: 0.9;
  color: white;
}

.close-btn-enhanced {
  background: rgba(255, 255, 255, 0.15);
  border: 1px solid rgba(255, 255, 255, 0.3);
  color: white;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.3s ease;
  backdrop-filter: blur(10px);
  position: relative;
  z-index: 2;
}

.close-btn-enhanced:hover {
  background: rgba(255, 255, 255, 0.25);
  transform: scale(1.1);
}

.close-icon {
  font-size: 18px;
  font-weight: bold;
}

/* Overview cards trong modal */
.detail-overview {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  padding: 30px;
  background: white;
}

.overview-card-detail {
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
  padding: 20px;
  border-radius: 15px;
  text-align: center;
  border: 1px solid #e2e8f0;
  transition: all 0.3s ease;
}

.overview-card-detail:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
}

.overview-label {
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  color: #64748b;
  margin-bottom: 8px;
  letter-spacing: 0.5px;
}

.overview-value {
  font-size: 18px;
  font-weight: 700;
  margin: 0;
}

.overview-value.current { color: #1e40af; }
.overview-value.target { color: #7c3aed; }
.overview-value.completion.excellent { color: #059669; }
.overview-value.completion.good { color: #0891b2; }
.overview-value.completion.average { color: #d97706; }
.overview-value.completion.poor { color: #dc2626; }
.overview-value.quarter { color: #8b5cf6; }

/* Analysis section */
.detail-analysis {
  padding: 30px;
  background: white;
}

.analysis-title {
  color: #722f37;
  font-size: 20px;
  font-weight: 700;
  margin: 0 0 25px 0;
  display: flex;
  align-items: center;
  gap: 10px;
}

.analysis-content {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 25px;
  margin-bottom: 30px;
}

.change-detail-card {
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
  border-radius: 15px;
  padding: 25px;
  border-left: 5px solid;
  transition: all 0.3s ease;
}

.change-detail-card.positive { border-left-color: #059669; }
.change-detail-card.negative { border-left-color: #dc2626; }
.change-detail-card.neutral { border-left-color: #0891b2; }

.change-detail-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.1);
}

.change-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 15px;
}

.change-icon {
  font-size: 24px;
}

.change-title {
  font-size: 16px;
  font-weight: 600;
  color: #374151;
}

.change-stats {
  margin-bottom: 20px;
}

.change-percentage {
  font-size: 28px;
  font-weight: 700;
  margin-bottom: 5px;
}

.change-detail-card.positive .change-percentage { color: #059669; }
.change-detail-card.negative .change-percentage { color: #dc2626; }
.change-detail-card.neutral .change-percentage { color: #0891b2; }

.change-description {
  font-size: 14px;
  color: #6b7280;
  font-weight: 500;
}

/* Contributors section */
.contributors-section h4 {
  color: #374151;
  font-size: 14px;
  font-weight: 600;
  margin: 0 0 15px 0;
  display: flex;
  align-items: center;
  gap: 8px;
}

.contributors-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.contributor-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 15px;
  background: white;
  border-radius: 8px;
  border-left: 3px solid;
  font-size: 13px;
  transition: all 0.2s ease;
}

.contributor-item.positive { border-left-color: #059669; }
.contributor-item.negative { border-left-color: #dc2626; }
.contributor-item.neutral { border-left-color: #0891b2; }

.contributor-item:hover {
  background: #f1f5f9;
  transform: translateX(5px);
}

.contributor-name {
  flex: 1;
  font-weight: 500;
  color: #374151;
}

.contributor-value {
  font-weight: 600;
  font-family: 'Courier New', monospace;
}

.contributor-item.positive .contributor-value { color: #059669; }
.contributor-item.negative .contributor-value { color: #dc2626; }
.contributor-item.neutral .contributor-value { color: #0891b2; }

/* Chart section trong modal */
.detail-chart-section {
  margin-top: 30px;
  padding-top: 30px;
  border-top: 2px solid #e2e8f0;
}

.chart-title {
  color: #722f37;
  font-size: 18px;
  font-weight: 700;
  margin: 0 0 20px 0;
  display: flex;
  align-items: center;
  gap: 10px;
}

.detail-chart-container {
  background: white;
  border-radius: 12px;
  border: 1px solid #e2e8f0;
  overflow: hidden;
}

/* Action buttons */
.detail-actions {
  background: #f8fafc;
  padding: 25px 30px;
  display: flex;
  justify-content: flex-end;
  gap: 15px;
  border-top: 1px solid #e2e8f0;
}

.detail-actions .el-button {
  min-width: 120px;
  border-radius: 8px;
  font-weight: 600;
}

/* Responsive cho modal */
@media (max-width: 768px) {
  .indicator-detail-modal-enhanced {
    width: 95% !important;
  }

  .analysis-content {
    grid-template-columns: 1fr;
    gap: 20px;
  }

  .detail-overview {
    grid-template-columns: repeat(2, 1fr);
    gap: 15px;
    padding: 20px;
  }

  .detail-analysis {
    padding: 20px;
  }

  .detail-actions {
    padding: 20px;
    flex-direction: column;
  }
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
