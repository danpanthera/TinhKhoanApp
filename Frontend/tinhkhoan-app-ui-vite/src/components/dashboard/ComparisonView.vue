<template>
  <div class="comparison-view">
    <!-- Header Controls -->
    <div class="comparison-header">
      <div class="header-title">
        <h3>So sánh hiệu quả giữa các đơn vị</h3>
        <p class="subtitle">Phân tích đa chiều các chỉ tiêu kinh doanh</p>
      </div>
      
      <div class="comparison-controls">
        <el-select v-model="selectedIndicators" multiple placeholder="Chọn chỉ tiêu" style="width: 300px">
          <el-option
            v-for="indicator in indicators"
            :key="indicator.code"
            :label="indicator.name"
            :value="indicator.code"
          />
        </el-select>
        
        <el-select v-model="comparisonType" placeholder="Loại so sánh" style="width: 200px">
          <el-option label="Theo giá trị tuyệt đối" value="absolute" />
          <el-option label="Theo tỷ lệ hoàn thành" value="completion" />
          <el-option label="Theo tăng trưởng" value="growth" />
        </el-select>
        
        <el-select v-model="chartType" placeholder="Loại biểu đồ" style="width: 150px">
          <el-option label="Cột" value="bar" />
          <el-option label="Đường" value="line" />
          <el-option label="Radar" value="radar" />
          <el-option label="Tròn" value="pie" />
        </el-select>
      </div>
    </div>

    <!-- Main Comparison Chart -->
    <div class="main-comparison">
      <div class="chart-container">
        <div class="chart-header">
          <h4>{{ getChartTitle() }}</h4>
          <div class="chart-actions">
            <el-button size="small" @click="downloadChart">
              <i class="mdi mdi-download"></i> Tải xuống
            </el-button>
            <el-button size="small" @click="fullscreenChart">
              <i class="mdi mdi-fullscreen"></i> Toàn màn hình
            </el-button>
          </div>
        </div>
        <comparison-chart
          :data="comparisonChartData"
          :height="400"
          :type="chartType"
          :comparison-type="comparisonType"
        />
      </div>
    </div>

    <!-- Detailed Comparison Grid -->
    <div class="comparison-grid">
      <el-row :gutter="20">
        <el-col :span="16">
          <!-- Ranking Table -->
          <div class="ranking-table">
            <div class="table-header">
              <h4>Bảng xếp hạng theo {{ getRankingTitle() }}</h4>
              <el-switch
                v-model="showPercentage"
                active-text="Hiển thị %"
                inactive-text="Giá trị gốc"
              />
            </div>
            
            <el-table
              :data="rankingData"
              stripe
              height="350"
              @row-click="selectUnit"
              highlight-current-row
            >
              <el-table-column type="index" label="Hạng" width="60" align="center" />
              <el-table-column prop="unitName" label="Đơn vị" width="200" />
              
              <el-table-column
                v-for="indicator in selectedIndicators"
                :key="indicator"
                :label="getIndicatorName(indicator)"
                align="right"
                min-width="120"
              >
                <template #default="{ row }">
                  <div class="indicator-cell">
                    <span class="value" :class="getValueClass(row[indicator], indicator)">
                      {{ formatCellValue(row[indicator], indicator) }}
                    </span>
                    <div class="rank-badge" :class="getRankClass(row[indicator + '_rank'])">
                      #{{ row[indicator + '_rank'] }}
                    </div>
                  </div>
                </template>
              </el-table-column>
              
              <el-table-column label="Tổng điểm" width="100" align="center">
                <template #default="{ row }">
                  <el-tag :type="getScoreType(row.totalScore)" size="small">
                    {{ row.totalScore }}/100
                  </el-tag>
                </template>
              </el-table-column>
            </el-table>
          </div>
        </el-col>
        
        <el-col :span="8">
          <!-- Performance Summary -->
          <div class="performance-summary">
            <div class="summary-header">
              <h4>Tóm tắt hiệu quả</h4>
            </div>
            
            <div class="summary-cards">
              <div class="summary-card best">
                <div class="card-icon">
                  <i class="mdi mdi-trophy"></i>
                </div>
                <div class="card-content">
                  <div class="card-title">Đơn vị xuất sắc nhất</div>
                  <div class="card-value">{{ getBestUnit().name }}</div>
                  <div class="card-subtitle">{{ getBestUnit().score }}/100 điểm</div>
                </div>
              </div>
              
              <div class="summary-card average">
                <div class="card-icon">
                  <i class="mdi mdi-chart-line"></i>
                </div>
                <div class="card-content">
                  <div class="card-title">Điểm trung bình</div>
                  <div class="card-value">{{ getAverageScore().toFixed(1) }}</div>
                  <div class="card-subtitle">trên {{ rankingData.length }} đơn vị</div>
                </div>
              </div>
              
              <div class="summary-card improvement">
                <div class="card-icon">
                  <i class="mdi mdi-trending-up"></i>
                </div>
                <div class="card-content">
                  <div class="card-title">Cần cải thiện</div>
                  <div class="card-value">{{ getImprovementCount() }}</div>
                  <div class="card-subtitle">đơn vị dưới 70 điểm</div>
                </div>
              </div>
            </div>
            
            <!-- Performance Insights -->
            <div class="insights">
              <h5>Nhận xét từ dữ liệu</h5>
              <ul class="insight-list">
                <li v-for="insight in performanceInsights" :key="insight.id">
                  <i :class="insight.icon" :style="{ color: insight.color }"></i>
                  {{ insight.text }}
                </li>
              </ul>
            </div>
          </div>
        </el-col>
      </el-row>
    </div>

    <!-- Trend Comparison -->
    <div class="trend-comparison">
      <div class="chart-container">
        <div class="chart-header">
          <h4>So sánh xu hướng theo thời gian</h4>
          <el-radio-group v-model="trendPeriod" size="small">
            <el-radio-button label="6month">6 tháng</el-radio-button>
            <el-radio-button label="12month">12 tháng</el-radio-button>
            <el-radio-button label="quarter">Theo quý</el-radio-button>
          </el-radio-group>
        </div>
        
        <trend-chart
          :data="trendComparisonData"
          :height="300"
          :indicators="selectedIndicators"
          type="multi-unit"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import ComparisonChart from './ComparisonChart.vue';
import TrendChart from './TrendChart.vue';

const props = defineProps({
  indicators: {
    type: Array,
    required: true
  },
  units: {
    type: Array,
    required: true
  },
  date: {
    type: String,
    required: true
  }
});

// State
const selectedIndicators = ref(['HuyDong', 'DuNo', 'LoiNhuan']);
const comparisonType = ref('completion');
const chartType = ref('bar');
const showPercentage = ref(true);
const trendPeriod = ref('6month');

// Mock data - thay thế bằng API calls thực tế
const comparisonChartData = ref([
  { unitName: 'CN Lai Châu', HuyDong: 1450, DuNo: 1200, LoiNhuan: 145 },
  { unitName: 'CN Điện Biên', HuyDong: 980, DuNo: 850, LoiNhuan: 98 },
  { unitName: 'CN Sơn La', HuyDong: 1200, DuNo: 1050, LoiNhuan: 120 },
  { unitName: 'PGD Mường Tè', HuyDong: 250, DuNo: 220, LoiNhuan: 25 },
  { unitName: 'PGD Tam Đường', HuyDong: 180, DuNo: 160, LoiNhuan: 18 }
]);

const rankingData = ref([
  {
    unitName: 'CN Lai Châu',
    HuyDong: 107.4, HuyDong_rank: 1,
    DuNo: 109.1, DuNo_rank: 1,
    LoiNhuan: 112.5, LoiNhuan_rank: 1,
    totalScore: 95
  },
  {
    unitName: 'CN Sơn La',
    HuyDong: 104.2, HuyDong_rank: 2,
    DuNo: 105.8, DuNo_rank: 2,
    LoiNhuan: 108.3, LoiNhuan_rank: 2,
    totalScore: 88
  },
  {
    unitName: 'CN Điện Biên',
    HuyDong: 98.0, HuyDong_rank: 3,
    DuNo: 95.5, DuNo_rank: 4,
    LoiNhuan: 92.1, LoiNhuan_rank: 4,
    totalScore: 75
  },
  {
    unitName: 'PGD Mường Tè',
    HuyDong: 102.1, HuyDong_rank: 4,
    DuNo: 98.2, DuNo_rank: 3,
    LoiNhuan: 95.6, LoiNhuan_rank: 3,
    totalScore: 82
  },
  {
    unitName: 'PGD Tam Đường',
    HuyDong: 90.0, HuyDong_rank: 5,
    DuNo: 88.5, DuNo_rank: 5,
    LoiNhuan: 85.2, LoiNhuan_rank: 5,
    totalScore: 68
  }
]);

const trendComparisonData = ref([
  { period: 'T1', 'CN Lai Châu': 85, 'CN Sơn La': 78, 'CN Điện Biên': 72 },
  { period: 'T2', 'CN Lai Châu': 92, 'CN Sơn La': 85, 'CN Điện Biên': 78 },
  { period: 'T3', 'CN Lai Châu': 98, 'CN Sơn La': 90, 'CN Điện Biên': 82 },
  { period: 'T4', 'CN Lai Châu': 105, 'CN Sơn La': 95, 'CN Điện Biên': 88 },
  { period: 'T5', 'CN Lai Châu': 108, 'CN Sơn La': 98, 'CN Điện Biên': 90 },
  { period: 'T6', 'CN Lai Châu': 112, 'CN Sơn La': 102, 'CN Điện Biên': 95 }
]);

// Computed
const performanceInsights = computed(() => [
  {
    id: 1,
    icon: 'mdi-trending-up',
    color: '#67C23A',
    text: 'CN Lai Châu dẫn đầu về tất cả các chỉ tiêu với tổng điểm 95/100'
  },
  {
    id: 2,
    icon: 'mdi-alert-circle',
    color: '#E6A23C',
    text: 'PGD Tam Đường cần cải thiện, chỉ đạt 68/100 điểm'
  },
  {
    id: 3,
    icon: 'mdi-chart-line',
    color: '#409EFF',
    text: 'Chỉ tiêu Lợi nhuận có sự chênh lệch lớn giữa các đơn vị'
  },
  {
    id: 4,
    icon: 'mdi-target',
    color: '#909399',
    text: 'Điểm trung bình hệ thống: 81.6/100, cần nâng cao thêm'
  }
]);

// Methods
const getChartTitle = () => {
  const typeMap = {
    absolute: 'So sánh theo giá trị tuyệt đối',
    completion: 'So sánh theo tỷ lệ hoàn thành kế hoạch',
    growth: 'So sánh theo tăng trưởng so với cùng kỳ'
  };
  return typeMap[comparisonType.value] || 'So sánh đa chiều';
};

const getRankingTitle = () => {
  const typeMap = {
    absolute: 'giá trị tuyệt đối',
    completion: 'tỷ lệ hoàn thành',
    growth: 'tăng trưởng'
  };
  return typeMap[comparisonType.value] || 'hiệu quả tổng thể';
};

const getIndicatorName = (code) => {
  const indicator = props.indicators.find(i => i.code === code);
  return indicator ? indicator.name : code;
};

const formatCellValue = (value, indicator) => {
  if (showPercentage.value && comparisonType.value === 'completion') {
    return value.toFixed(1) + '%';
  }
  
  const indicatorObj = props.indicators.find(i => i.code === indicator);
  if (indicatorObj?.unit === '%') {
    return value.toFixed(2) + '%';
  }
  
  return new Intl.NumberFormat('vi-VN').format(value);
};

const getValueClass = (value, indicator) => {
  if (comparisonType.value === 'completion') {
    if (value >= 100) return 'excellent';
    if (value >= 90) return 'good';
    if (value >= 80) return 'average';
    return 'poor';
  }
  return '';
};

const getRankClass = (rank) => {
  if (rank === 1) return 'rank-1';
  if (rank === 2) return 'rank-2';
  if (rank === 3) return 'rank-3';
  return 'rank-other';
};

const getScoreType = (score) => {
  if (score >= 90) return 'success';
  if (score >= 80) return 'warning';
  return 'danger';
};

const getBestUnit = () => {
  const best = rankingData.value[0];
  return {
    name: best.unitName,
    score: best.totalScore
  };
};

const getAverageScore = () => {
  const total = rankingData.value.reduce((sum, unit) => sum + unit.totalScore, 0);
  return total / rankingData.value.length;
};

const getImprovementCount = () => {
  return rankingData.value.filter(unit => unit.totalScore < 70).length;
};

const selectUnit = (row) => {
  ElMessage.info(`Xem chi tiết ${row.unitName}`);
};

const downloadChart = () => {
  ElMessage.info('Tính năng tải xuống biểu đồ đang phát triển');
};

const fullscreenChart = () => {
  ElMessage.info('Tính năng xem toàn màn hình đang phát triển');
};

// Watchers
watch([selectedIndicators, comparisonType], () => {
  // Reload comparison data when filters change
  console.log('Comparison filters changed');
});

onMounted(() => {
  console.log('ComparisonView mounted');
});
</script>

<style scoped>
.comparison-view {
  background: #f5f7fa;
  min-height: 600px;
}

.comparison-header {
  background: white;
  padding: 20px;
  border-radius: 8px;
  margin-bottom: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.header-title h3 {
  margin: 0 0 5px 0;
  font-size: 20px;
  color: #303133;
}

.subtitle {
  margin: 0;
  color: #909399;
  font-size: 14px;
}

.comparison-controls {
  display: flex;
  gap: 10px;
  align-items: center;
}

.main-comparison,
.comparison-grid,
.trend-comparison {
  margin-bottom: 20px;
}

.chart-container {
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.chart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.chart-header h4 {
  margin: 0;
  font-size: 16px;
  color: #303133;
}

.chart-actions {
  display: flex;
  gap: 10px;
}

.ranking-table {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.table-header {
  padding: 15px 20px;
  border-bottom: 1px solid #EBEEF5;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.table-header h4 {
  margin: 0;
  font-size: 16px;
  color: #303133;
}

.indicator-cell {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.value.excellent { color: #67C23A; }
.value.good { color: #95D475; }
.value.average { color: #E6A23C; }
.value.poor { color: #F56C6C; }

.rank-badge {
  font-size: 10px;
  padding: 2px 6px;
  border-radius: 10px;
  font-weight: bold;
}

.rank-1 { background: #FFD700; color: #8B4513; }
.rank-2 { background: #C0C0C0; color: #696969; }
.rank-3 { background: #CD7F32; color: white; }
.rank-other { background: #E0E0E0; color: #666; }

.performance-summary {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.summary-header {
  padding: 15px 20px;
  border-bottom: 1px solid #EBEEF5;
}

.summary-header h4 {
  margin: 0;
  font-size: 16px;
  color: #303133;
}

.summary-cards {
  padding: 20px;
}

.summary-card {
  display: flex;
  align-items: center;
  gap: 15px;
  padding: 15px;
  border-radius: 8px;
  margin-bottom: 15px;
}

.summary-card.best { background: linear-gradient(135deg, #67C23A, #85CE61); }
.summary-card.average { background: linear-gradient(135deg, #409EFF, #66B1FF); }
.summary-card.improvement { background: linear-gradient(135deg, #E6A23C, #EEBC6D); }

.card-icon {
  color: white;
  font-size: 24px;
}

.card-content {
  color: white;
}

.card-title {
  font-size: 12px;
  opacity: 0.9;
  margin-bottom: 5px;
}

.card-value {
  font-size: 20px;
  font-weight: bold;
  margin-bottom: 2px;
}

.card-subtitle {
  font-size: 11px;
  opacity: 0.8;
}

.insights {
  padding: 0 20px 20px;
}

.insights h5 {
  margin: 0 0 15px 0;
  font-size: 14px;
  color: #303133;
}

.insight-list {
  list-style: none;
  margin: 0;
  padding: 0;
}

.insight-list li {
  padding: 8px 0;
  font-size: 13px;
  color: #606266;
  display: flex;
  align-items: center;
  gap: 8px;
}

.insight-list i {
  font-size: 16px;
}

:deep(.el-table .current-row) {
  background-color: #f0f9ff;
}
</style>
