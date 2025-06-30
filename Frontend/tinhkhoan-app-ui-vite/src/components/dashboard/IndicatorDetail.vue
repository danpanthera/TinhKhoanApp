<template>
  <div class="indicator-detail">
    <!-- Header với thông tin chỉ tiêu -->
    <div class="detail-header">
      <div class="indicator-info">
        <i :class="indicator.icon" :style="{ color: indicator.color }" class="indicator-icon"></i>
        <div>
          <h3>{{ indicator.name }}</h3>
          <p class="indicator-description">{{ indicator.description || 'Theo dõi hiệu quả hoạt động kinh doanh' }}</p>
        </div>
      </div>

      <div class="detail-actions">
        <el-button-group>
          <el-button size="small" :type="timeRange === 'month' ? 'primary' : 'default'" @click="timeRange = 'month'">
            Tháng
          </el-button>
          <el-button size="small" :type="timeRange === 'quarter' ? 'primary' : 'default'" @click="timeRange = 'quarter'">
            Quý
          </el-button>
          <el-button size="small" :type="timeRange === 'year' ? 'primary' : 'default'" @click="timeRange = 'year'">
            Năm
          </el-button>
        </el-button-group>
        <el-button size="small" @click="exportDetail">
          <i class="mdi mdi-download"></i> Xuất báo cáo
        </el-button>
      </div>
    </div>

    <!-- KPI Overview Cards -->
    <div class="detail-overview">
      <el-row :gutter="20">
        <el-col :span="6">
          <div class="overview-card">
            <div class="card-title">Giá trị hiện tại</div>
            <div class="card-value" :style="{ color: indicator.color }">
              {{ formatValue(indicator.actualValue) }}
              <span class="unit">{{ indicator.unit }}</span>
            </div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="overview-card">
            <div class="card-title">Kế hoạch</div>
            <div class="card-value">
              {{ formatValue(indicator.planValue) }}
              <span class="unit">{{ indicator.unit }}</span>
            </div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="overview-card">
            <div class="card-title">Tỷ lệ hoàn thành</div>
            <div class="card-value" :class="completionClass">
              {{ completionRate.toFixed(1) }}%
            </div>
          </div>
        </el-col>
        <el-col :span="6">
          <div class="overview-card">
            <div class="card-title">So với cùng kỳ</div>
            <div class="card-value" :class="indicator.yoyGrowth >= 0 ? 'positive' : 'negative'">
              {{ indicator.yoyGrowth >= 0 ? '+' : '' }}{{ indicator.yoyGrowth }}%
            </div>
          </div>
        </el-col>
      </el-row>
    </div>

    <!-- Detailed Chart -->
    <div class="detail-charts">
      <el-row :gutter="20">
        <el-col :span="16">
          <div class="chart-container">
            <div class="chart-header">
              <h4>Xu hướng theo {{ timeRange === 'month' ? 'tháng' : timeRange === 'quarter' ? 'quý' : 'năm' }}</h4>
              <el-checkbox-group v-model="selectedMetrics" size="small">
                <el-checkbox label="actual">Thực hiện</el-checkbox>
                <el-checkbox label="plan">Kế hoạch</el-checkbox>
                <el-checkbox label="yoy">Cùng kỳ năm trước</el-checkbox>
              </el-checkbox-group>
            </div>
            <trend-chart
              :data="detailTrendData"
              :height="350"
              :indicators="[indicator.code]"
              :metrics="selectedMetrics"
            />
          </div>
        </el-col>
        <el-col :span="8">
          <div class="chart-container">
            <div class="chart-header">
              <h4>Phân tích theo đơn vị</h4>
            </div>
            <comparison-chart
              :data="unitDetailData"
              :height="350"
              type="bar"
              :indicator="indicator.code"
            />
          </div>
        </el-col>
      </el-row>
    </div>

    <!-- Breakdown Table -->
    <div class="detail-table">
      <div class="table-header">
        <h4>Chi tiết theo đơn vị</h4>
        <div class="table-actions">
          <el-input
            v-model="searchText"
            placeholder="Tìm kiếm đơn vị..."
            size="small"
            style="width: 200px"
          >
            <template #prefix>
              <i class="mdi mdi-magnify"></i>
            </template>
          </el-input>
        </div>
      </div>

      <el-table
        :data="filteredTableData"
        stripe
        height="300"
        @sort-change="handleSortChange"
      >
        <el-table-column prop="unitName" label="Đơn vị" width="180" />
        <el-table-column prop="actualValue" label="Thực hiện" align="right" sortable width="120">
          <template #default="{ row }">
            {{ formatValue(row.actualValue) }}
          </template>
        </el-table-column>
        <el-table-column prop="planValue" label="Kế hoạch" align="right" sortable width="120">
          <template #default="{ row }">
            {{ formatValue(row.planValue) }}
          </template>
        </el-table-column>
        <el-table-column prop="completionRate" label="Tỷ lệ (%)" align="right" sortable width="100">
          <template #default="{ row }">
            <span :class="getCompletionClass(row.completionRate)">
              {{ row.completionRate.toFixed(1) }}%
            </span>
          </template>
        </el-table-column>
        <el-table-column prop="vsStartYear" label="So với đầu năm" align="right" sortable width="140">
          <template #default="{ row }">
            <span :class="getChangeClass(row.vsStartYear, indicator.code)">
              {{ row.vsStartYear >= 0 ? '+' : '' }}{{ formatValue(row.vsStartYear) }}
              ({{ row.vsStartYearPercent >= 0 ? '+' : '' }}{{ row.vsStartYearPercent.toFixed(1) }}%)
            </span>
          </template>
        </el-table-column>
        <el-table-column prop="vsStartMonth" label="So với đầu tháng" align="right" sortable width="140">
          <template #default="{ row }">
            <span :class="getChangeClass(row.vsStartMonth, indicator.code)">
              {{ row.vsStartMonth >= 0 ? '+' : '' }}{{ formatValue(row.vsStartMonth) }}
              ({{ row.vsStartMonthPercent >= 0 ? '+' : '' }}{{ row.vsStartMonthPercent.toFixed(1) }}%)
            </span>
          </template>
        </el-table-column>
        <el-table-column prop="yoyGrowth" label="So với cùng kỳ (%)" align="right" sortable width="140">
          <template #default="{ row }">
            <span :class="getChangeClass(row.yoyGrowth, indicator.code)">
              {{ row.yoyGrowth >= 0 ? '+' : '' }}{{ row.yoyGrowth }}%
            </span>
          </template>
        </el-table-column>
        <el-table-column label="Thao tác" width="100" align="center" fixed="right">
          <template #default="{ row }">
            <el-button size="small" type="text" @click="drillDown(row)">
              Chi tiết
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>
  </div>
</template>

<script setup>
import { ElMessage } from 'element-plus';
import { computed, onMounted, ref, watch } from 'vue';
import ComparisonChart from './ComparisonChart.vue';
import TrendChart from './TrendChart.vue';

const props = defineProps({
  indicator: {
    type: Object,
    required: true
  },
  unitId: {
    type: [String, Number],
    default: null
  },
  date: {
    type: String,
    required: true
  }
});

const emit = defineEmits(['filter-change']);

// State
const timeRange = ref('month');
const selectedMetrics = ref(['actual', 'plan']);
const searchText = ref('');

// Mock data - sẽ được thay thế bằng API calls thực tế
const detailTrendData = ref([
  { period: 'T1', actual: 850, plan: 800, yoy: 780 },
  { period: 'T2', actual: 920, plan: 900, yoy: 850 },
  { period: 'T3', actual: 1050, plan: 1000, yoy: 980 },
  { period: 'T4', actual: 1180, plan: 1100, yoy: 1050 },
  { period: 'T5', actual: 1320, plan: 1200, yoy: 1180 },
  { period: 'T6', actual: 1450, plan: 1350, yoy: 1280 }
]);

const unitDetailData = ref([
  {
    unitName: 'Chi nhánh Lai Châu',
    actualValue: 1450,
    planValue: 1350,
    completionRate: 107.4,
    yoyGrowth: 12.5,
    vsStartYear: 150,
    vsStartYearPercent: 11.5,
    vsStartMonth: 30,
    vsStartMonthPercent: 2.1
  },
  {
    unitName: 'Chi nhánh Đoàn Kết',
    actualValue: 980,
    planValue: 1000,
    completionRate: 98.0,
    yoyGrowth: 8.2,
    vsStartYear: 120,
    vsStartYearPercent: 13.9,
    vsStartMonth: 25,
    vsStartMonthPercent: 2.6
  },
  {
    unitName: 'Chi nhánh Bình Lư',
    actualValue: 1200,
    planValue: 1100,
    completionRate: 109.1,
    yoyGrowth: 15.3,
    vsStartYear: 180,
    vsStartYearPercent: 17.6,
    vsStartMonth: 40,
    vsStartMonthPercent: 3.4
  },
  {
    unitName: 'Chi nhánh Tân Uyên',
    actualValue: 850,
    planValue: 900,
    completionRate: 94.4,
    yoyGrowth: 7.5,
    vsStartYear: 100,
    vsStartYearPercent: 13.3,
    vsStartMonth: 20,
    vsStartMonthPercent: 2.4
  },
  {
    unitName: 'Chi nhánh Sìn Hồ',
    actualValue: 780,
    planValue: 780,
    completionRate: 100.0,
    yoyGrowth: 9.8,
    vsStartYear: 110,
    vsStartYearPercent: 16.4,
    vsStartMonth: 15,
    vsStartMonthPercent: 2.0
  },
  {
    unitName: 'Chi nhánh Phong Thổ',
    actualValue: 920,
    planValue: 850,
    completionRate: 108.2,
    yoyGrowth: 14.2,
    vsStartYear: 130,
    vsStartYearPercent: 16.5,
    vsStartMonth: 35,
    vsStartMonthPercent: 4.0
  },
  {
    unitName: 'Chi nhánh Than Uyên',
    actualValue: 750,
    planValue: 800,
    completionRate: 93.8,
    yoyGrowth: 6.8,
    vsStartYear: 90,
    vsStartYearPercent: 13.6,
    vsStartMonth: 18,
    vsStartMonthPercent: 2.5
  },
  {
    unitName: 'Chi nhánh Bum Tở',
    actualValue: 680,
    planValue: 700,
    completionRate: 97.1,
    yoyGrowth: 8.1,
    vsStartYear: 85,
    vsStartYearPercent: 14.3,
    vsStartMonth: 12,
    vsStartMonthPercent: 1.8
  },
  {
    unitName: 'Chi nhánh Nậm Hàng',
    actualValue: 650,
    planValue: 650,
    completionRate: 100.0,
    yoyGrowth: 10.2,
    vsStartYear: 80,
    vsStartYearPercent: 14.0,
    vsStartMonth: 15,
    vsStartMonthPercent: 2.4
  }
]);

// Computed
const completionRate = computed(() => {
  if (!props.indicator.planValue || props.indicator.planValue === 0) return 0;
  return (props.indicator.actualValue / props.indicator.planValue) * 100;
});

const completionClass = computed(() => {
  const rate = completionRate.value;
  if (rate >= 100) return 'positive';
  if (rate >= 80) return 'warning';
  return 'negative';
});

const filteredTableData = computed(() => {
  if (!searchText.value) return unitDetailData.value;

  return unitDetailData.value.filter(item =>
    item.unitName.toLowerCase().includes(searchText.value.toLowerCase())
  );
});

// Methods
const formatValue = (value) => {
  if (props.indicator.unit === '%') {
    return value.toFixed(2);
  }
  return new Intl.NumberFormat('vi-VN').format(value);
};

const getCompletionClass = (rate) => {
  if (rate >= 100) return 'positive';
  if (rate >= 80) return 'warning';
  return 'negative';
};

const getChangeClass = (value, indicatorCode) => {
  // Đối với Tỷ lệ nợ xấu, giảm là tốt
  if (indicatorCode === 'TyLeNoXau') {
    return value < 0 ? 'positive' : value > 0 ? 'negative' : '';
  }
  // Đối với các chỉ tiêu khác, tăng là tốt
  return value > 0 ? 'positive' : value < 0 ? 'negative' : '';
};

const exportDetail = () => {
  ElMessage.info('Tính năng xuất báo cáo chi tiết đang phát triển');
};

const handleSortChange = ({ column, prop, order }) => {
  // TODO: Implement sorting
  console.log('Sort change:', column, prop, order);
};

const drillDown = (row) => {
  ElMessage.info(`Xem chi tiết ${row.unitName}`);
  // TODO: Navigate to unit-specific detail
};

// Watchers
watch([timeRange, selectedMetrics], () => {
  emit('filter-change', {
    timeRange: timeRange.value,
    metrics: selectedMetrics.value
  });
});

onMounted(() => {
  // Load initial data
  console.log('IndicatorDetail mounted for:', props.indicator.name);
});
</script>

<style scoped>
.indicator-detail {
  background: #f5f7fa;
  min-height: 600px;
}

.detail-header {
  background: white;
  padding: 20px;
  border-radius: 8px;
  margin-bottom: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.indicator-info {
  display: flex;
  align-items: center;
  gap: 15px;
}

.indicator-icon {
  font-size: 40px;
  opacity: 0.8;
}

.indicator-info h3 {
  margin: 0 0 5px 0;
  font-size: 20px;
  color: #303133;
}

.indicator-description {
  margin: 0;
  color: #909399;
  font-size: 14px;
}

.detail-actions {
  display: flex;
  gap: 10px;
  align-items: center;
}

.detail-overview {
  margin-bottom: 20px;
}

.overview-card {
  background: white;
  padding: 20px;
  border-radius: 8px;
  text-align: center;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.overview-card .card-title {
  font-size: 14px;
  color: #909399;
  margin-bottom: 10px;
}

.overview-card .card-value {
  font-size: 24px;
  font-weight: bold;
  color: #303133;
}

.overview-card .unit {
  font-size: 14px;
  color: #909399;
  margin-left: 5px;
}

.detail-charts,
.detail-table {
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

.table-header {
  background: white;
  padding: 15px 20px;
  border-radius: 8px 8px 0 0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid #EBEEF5;
}

.table-header h4 {
  margin: 0;
  font-size: 16px;
  color: #303133;
}

.positive {
  color: #67C23A;
}

.warning {
  color: #E6A23C;
}

.negative {
  color: #F56C6C;
}

:deep(.el-table) {
  border-radius: 0 0 8px 8px;
}
</style>
