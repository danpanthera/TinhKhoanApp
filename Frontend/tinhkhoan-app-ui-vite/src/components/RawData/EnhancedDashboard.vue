<template>
  <div class="enhanced-dashboard">
    <!-- Header with Quick Stats -->
    <div class="dashboard-header">
      <h2>📊 Tổng quan hiệu suất hệ thống</h2>
      <div class="quick-actions">
        <button @click="refreshAllData" :disabled="loading" class="refresh-btn">
          <span v-if="loading">⏳</span>
          <span v-else>🔄</span>
          Làm mới tất cả
        </button>
        <button @click="exportReport" class="export-btn">
          📈 Xuất báo cáo
        </button>
      </div>
    </div>

    <!-- Dashboard Stats Grid -->
    <div class="dashboard-stats" v-if="dashboardStats">
      <div class="stat-row">
        <!-- Main Metrics -->
        <div class="stat-card large">
          <div class="stat-icon">🗄️</div>
          <div class="stat-content">
            <div class="stat-value">{{ formatNumber(dashboardStats.totalImports) }}</div>
            <div class="stat-label">Tổng số imports</div>
            <div class="stat-change" :class="getChangeClass(dashboardStats.importsGrowth)">
              {{ formatGrowth(dashboardStats.importsGrowth) }}
            </div>
          </div>
        </div>

        <div class="stat-card large">
          <div class="stat-icon">📄</div>
          <div class="stat-content">
            <div class="stat-value">{{ formatNumber(dashboardStats.totalRecords) }}</div>
            <div class="stat-label">Tổng số bản ghi</div>
            <div class="stat-change" :class="getChangeClass(dashboardStats.recordsGrowth)">
              {{ formatGrowth(dashboardStats.recordsGrowth) }}
            </div>
          </div>
        </div>

        <div class="stat-card large">
          <div class="stat-icon">⚡</div>
          <div class="stat-content">
            <div class="stat-value" :style="{ color: getPerformanceColor(dashboardStats.avgResponseTime) }">
              {{ formatTime(dashboardStats.avgResponseTime) }}
            </div>
            <div class="stat-label">Thời gian phản hồi TB</div>
            <div class="stat-change" :class="getChangeClass(-dashboardStats.responseTimeChange)">
              {{ formatGrowth(-dashboardStats.responseTimeChange) }}
            </div>
          </div>
        </div>

        <div class="stat-card large">
          <div class="stat-icon">🎯</div>
          <div class="stat-content">
            <div class="stat-value" :style="{ color: getCacheColor(dashboardStats.cacheHitRate) }">
              {{ dashboardStats.cacheHitRate }}%
            </div>
            <div class="stat-label">Cache Hit Rate</div>
            <div class="stat-change" :class="getChangeClass(dashboardStats.cacheHitRateChange)">
              {{ formatGrowth(dashboardStats.cacheHitRateChange) }}
            </div>
          </div>
        </div>
      </div>

      <!-- Secondary Metrics -->
      <div class="stat-row secondary">
        <div class="stat-card">
          <div class="stat-mini">
            <span class="mini-icon">✅</span>
            <span class="mini-value">{{ dashboardStats.successfulImports }}</span>
            <span class="mini-label">Thành công</span>
          </div>
        </div>

        <div class="stat-card">
          <div class="stat-mini">
            <span class="mini-icon">❌</span>
            <span class="mini-value">{{ dashboardStats.failedImports }}</span>
            <span class="mini-label">Thất bại</span>
          </div>
        </div>

        <div class="stat-card">
          <div class="stat-mini">
            <span class="mini-icon">⏳</span>
            <span class="mini-value">{{ dashboardStats.processingImports }}</span>
            <span class="mini-label">Đang xử lý</span>
          </div>
        </div>

        <div class="stat-card">
          <div class="stat-mini">
            <span class="mini-icon">📅</span>
            <span class="mini-value">{{ dashboardStats.importsToday }}</span>
            <span class="mini-label">Hôm nay</span>
          </div>
        </div>

        <div class="stat-card">
          <div class="stat-mini">
            <span class="mini-icon">💾</span>
            <span class="mini-value">{{ formatSize(dashboardStats.totalDataSize) }}</span>
            <span class="mini-label">Dung lượng</span>
          </div>
        </div>

        <div class="stat-card">
          <div class="stat-mini">
            <span class="mini-icon">🔄</span>
            <span class="mini-value">{{ dashboardStats.scdRecords }}</span>
            <span class="mini-label">SCD Records</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Data Type Breakdown -->
    <div class="content-grid">
      <div class="chart-section">
        <div class="section-header">
          <h3>📊 Phân bố theo loại dữ liệu</h3>
          <select v-model="selectedTimePeriod" @change="loadDataTypeStats" class="time-selector">
            <option value="24h">24 giờ qua</option>
            <option value="7d">7 ngày qua</option>
            <option value="30d">30 ngày qua</option>
            <option value="90d">90 ngày qua</option>
          </select>
        </div>

        <div class="data-type-chart" v-if="dataTypeStats">
          <div class="chart-container">
            <div
              v-for="item in dataTypeStats"
              :key="item.dataType"
              class="chart-bar"
              :style="{ height: (item.percentage || 0) + '%' }"
            >
              <div class="bar-fill" :style="{ backgroundColor: getDataTypeColor(item.dataType) }"></div>
              <div class="bar-label">
                <div class="bar-type">{{ item.dataType }}</div>
                <div class="bar-count">{{ formatNumber(item.count) }}</div>
                <div class="bar-percentage">{{ (item.percentage || 0).toFixed(1) }}%</div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Performance Trends -->
      <div class="performance-section">
        <div class="section-header">
          <h3>⚡ Xu hướng hiệu suất</h3>
          <button @click="togglePerformanceDetails" class="toggle-btn">
            {{ showPerformanceDetails ? 'Ẩn chi tiết' : 'Xem chi tiết' }}
          </button>
        </div>

        <div class="performance-metrics">
          <div class="metric-item">
            <div class="metric-label">Truy vấn nhanh nhất</div>
            <div class="metric-value success">{{ formatTime(performanceData?.fastestQuery || 0) }}</div>
          </div>
          <div class="metric-item">
            <div class="metric-label">Truy vấn chậm nhất</div>
            <div class="metric-value error">{{ formatTime(performanceData?.slowestQuery || 0) }}</div>
          </div>
          <div class="metric-item">
            <div class="metric-label">Tổng truy vấn</div>
            <div class="metric-value">{{ formatNumber(performanceData?.totalQueries || 0) }}</div>
          </div>
          <div class="metric-item">
            <div class="metric-label">Truy vấn/giờ</div>
            <div class="metric-value">{{ formatNumber(performanceData?.queriesPerHour || 0) }}</div>
          </div>
        </div>

        <div v-if="showPerformanceDetails" class="performance-details">
          <div class="detail-row">
            <span class="detail-label">Cache Misses:</span>
            <span class="detail-value">{{ performanceData?.cacheMisses || 0 }}</span>
          </div>
          <div class="detail-row">
            <span class="detail-label">Memory Usage:</span>
            <span class="detail-value">{{ formatSize(performanceData?.memoryUsage || 0) }}</span>
          </div>
          <div class="detail-row">
            <span class="detail-label">Index Usage:</span>
            <span class="detail-value">{{ (performanceData?.indexUsageRate || 0).toFixed(1) }}%</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Recent Activity & Alerts -->
    <div class="activity-section">
      <div class="activity-header">
        <h3>📋 Hoạt động gần đây</h3>
        <div class="activity-filters">
          <button
            v-for="filter in activityFilters"
            :key="filter.key"
            @click="selectedActivityFilter = filter.key"
            :class="{ active: selectedActivityFilter === filter.key }"
            class="filter-btn"
          >
            {{ filter.icon }} {{ filter.label }}
          </button>
        </div>
      </div>

      <div class="activity-list">
        <div
          v-for="activity in filteredActivities"
          :key="activity.id"
          class="activity-item"
          :class="getActivityClass(activity.type)"
        >
          <div class="activity-icon">{{ getActivityIcon(activity.type) }}</div>
          <div class="activity-content">
            <div class="activity-title">{{ activity.title }}</div>
            <div class="activity-description">{{ activity.description }}</div>
            <div class="activity-meta">
              <span class="activity-time">{{ formatRelativeTime(activity.timestamp) }}</span>
              <span class="activity-user" v-if="activity.user">bởi {{ activity.user }}</span>
            </div>
          </div>
          <div class="activity-status">
            <span class="status-badge" :class="activity.status">{{ activity.status }}</span>
          </div>
        </div>
      </div>

      <div v-if="filteredActivities.length === 0" class="no-activities">
        <div class="no-data-icon">📭</div>
        <div class="no-data-text">Không có hoạt động nào trong khoảng thời gian này</div>
      </div>
    </div>

    <!-- Quick Access Panel -->
    <div class="quick-access">
      <h3>🚀 Truy cập nhanh</h3>
      <div class="quick-buttons">
        <button @click="openOptimizedTable" class="quick-btn">
          <span class="btn-icon">📊</span>
          <span class="btn-text">Bảng dữ liệu tối ưu</span>
        </button>
        <button @click="openImportTool" class="quick-btn">
          <span class="btn-icon">📤</span>
          <span class="btn-text">Import dữ liệu</span>
        </button>
        <button @click="openAdvancedSearch" class="quick-btn">
          <span class="btn-icon">🔍</span>
          <span class="btn-text">Tìm kiếm nâng cao</span>
        </button>
        <button @click="clearCache" class="quick-btn warning">
          <span class="btn-icon">🗑️</span>
          <span class="btn-text">Xóa cache</span>
        </button>
      </div>
    </div>

    <!-- Loading Overlay -->
    <div v-if="loading" class="loading-overlay">
      <div class="loading-content">
        <div class="loading-spinner">⏳</div>
        <div class="loading-text">Đang tải dữ liệu dashboard...</div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, onUnmounted, ref } from 'vue';
import rawDataService from '../../services/rawDataService.js';
import { formatNumber } from '../../utils/numberFormatter.js';

// Reactive data
const loading = ref(false);
const dashboardStats = ref(null);
const dataTypeStats = ref([]);
const performanceData = ref(null);
const recentActivities = ref([]);
const selectedTimePeriod = ref('24h');
const selectedActivityFilter = ref('all');
const showPerformanceDetails = ref(false);

// Activity filters
const activityFilters = [
  { key: 'all', label: 'Tất cả', icon: '📋' },
  { key: 'import', label: 'Import', icon: '📤' },
  { key: 'query', label: 'Truy vấn', icon: '🔍' },
  { key: 'error', label: 'Lỗi', icon: '❌' },
  { key: 'cache', label: 'Cache', icon: '💾' }
];

// Auto-refresh timer
let refreshTimer = null;

// Computed
const filteredActivities = computed(() => {
  if (selectedActivityFilter.value === 'all') {
    return recentActivities.value;
  }
  return recentActivities.value.filter(activity => activity.type === selectedActivityFilter.value);
});

// Methods
const loadAllData = async () => {
  loading.value = true;

  try {
    // Load dashboard stats
    const statsResult = await rawDataService.getDashboardStats();
    if (statsResult.success) {
      dashboardStats.value = statsResult.data;
    }

    // Load performance data
    const perfResult = await rawDataService.getPerformanceStats(selectedTimePeriod.value);
    if (perfResult.success) {
      performanceData.value = perfResult.data;
    }

    // Load data type stats
    await loadDataTypeStats();

    // Load recent activities (mock data for now)
    loadRecentActivities();

  } catch (error) {
    console.error('Error loading dashboard data:', error);
  } finally {
    loading.value = false;
  }
};

const loadDataTypeStats = async () => {
  try {
    // This would be a real API call in production
    // For now, using mock data based on dashboard stats
    if (dashboardStats.value) {
      const mockStats = [
        { dataType: 'LN01', count: Math.floor(dashboardStats.value.totalImports * 0.3), percentage: 30 },
        { dataType: 'DP01', count: Math.floor(dashboardStats.value.totalImports * 0.25), percentage: 25 },
        { dataType: 'GL01', count: Math.floor(dashboardStats.value.totalImports * 0.2), percentage: 20 },
        { dataType: 'EI01', count: Math.floor(dashboardStats.value.totalImports * 0.15), percentage: 15 },
        { dataType: 'BC57', count: Math.floor(dashboardStats.value.totalImports * 0.1), percentage: 10 }
      ];
      dataTypeStats.value = mockStats.sort((a, b) => b.percentage - a.percentage);
    }
  } catch (error) {
    console.error('Error loading data type stats:', error);
  }
};

const loadRecentActivities = () => {
  // Mock recent activities - in production this would be from API
  const now = new Date();
  const activities = [
    {
      id: '1',
      type: 'import',
      title: 'Import dữ liệu LN01',
      description: 'Đã import thành công 15,234 bản ghi từ file 7800_LN01_20250531.csv',
      timestamp: new Date(now - 5 * 60 * 1000), // 5 minutes ago
      status: 'success',
      user: 'Admin'
    },
    {
      id: '2',
      type: 'cache',
      title: 'Cache refresh',
      description: 'Hệ thống đã tự động làm mới cache cho bảng optimized imports',
      timestamp: new Date(now - 15 * 60 * 1000), // 15 minutes ago
      status: 'info'
    },
    {
      id: '3',
      type: 'query',
      title: 'Truy vấn chậm',
      description: 'Phát hiện truy vấn SCD History với thời gian phản hồi 2.3s',
      timestamp: new Date(now - 30 * 60 * 1000), // 30 minutes ago
      status: 'warning'
    },
    {
      id: '4',
      type: 'error',
      title: 'Lỗi import',
      description: 'Import file 7801_GL01_20250531.csv thất bại: định dạng không hợp lệ',
      timestamp: new Date(now - 45 * 60 * 1000), // 45 minutes ago
      status: 'error'
    }
  ];

  recentActivities.value = activities;
};

const refreshAllData = () => {
  loadAllData();
};

const exportReport = () => {
  // Implement export functionality
  console.log('Exporting dashboard report...');
};

const togglePerformanceDetails = () => {
  showPerformanceDetails.value = !showPerformanceDetails.value;
};

const openOptimizedTable = () => {
  // Navigate to optimized table
  console.log('Opening optimized table...');
};

const openImportTool = () => {
  // Navigate to import tool
  console.log('Opening import tool...');
};

const openAdvancedSearch = () => {
  // Navigate to advanced search
  console.log('Opening advanced search...');
};

const clearCache = async () => {
  try {
    const result = await rawDataService.refreshCache('all');
    if (result.success) {
      console.log('Cache cleared successfully');
      loadAllData(); // Refresh data after cache clear
    }
  } catch (error) {
    console.error('Error clearing cache:', error);
  }
};

// Loại bỏ custom formatNumber, sử dụng utility
// const formatNumber = (num) => { ... } // Removed - using imported utility

const formatTime = (ms) => {
  if (ms < 1000) {
    return ms.toFixed(0) + 'ms';
  }
  return (ms / 1000).toFixed(2) + 's';
};

const formatSize = (bytes) => {
  const units = ['B', 'KB', 'MB', 'GB'];
  let size = bytes;
  let unitIndex = 0;

  while (size >= 1024 && unitIndex < units.length - 1) {
    size /= 1024;
    unitIndex++;
  }

  return size.toFixed(1) + units[unitIndex];
};

const formatGrowth = (growth) => {
  const sign = growth >= 0 ? '+' : '';
  return `${sign}${growth.toFixed(1)}%`;
};

const formatRelativeTime = (date) => {
  const now = new Date();
  const diff = now - date;
  const minutes = Math.floor(diff / 60000);
  const hours = Math.floor(minutes / 60);
  const days = Math.floor(hours / 24);

  if (days > 0) return `${days} ngày trước`;
  if (hours > 0) return `${hours} giờ trước`;
  if (minutes > 0) return `${minutes} phút trước`;
  return 'Vừa xong';
};

const getChangeClass = (change) => {
  return {
    'stat-positive': change > 0,
    'stat-negative': change < 0,
    'stat-neutral': change === 0
  };
};

const getPerformanceColor = (time) => {
  if (time < 100) return '#52c41a';
  if (time < 500) return '#faad14';
  if (time < 1000) return '#fa8c16';
  return '#f5222d';
};

const getCacheColor = (rate) => {
  if (rate >= 90) return '#52c41a';
  if (rate >= 80) return '#faad14';
  if (rate >= 70) return '#fa8c16';
  return '#f5222d';
};

const getDataTypeColor = (dataType) => {
  return rawDataService.getDataTypeColor(dataType);
};

const getActivityClass = (type) => {
  return `activity-${type}`;
};

const getActivityIcon = (type) => {
  const icons = {
    import: '📤',
    query: '🔍',
    error: '❌',
    cache: '💾',
    system: '⚙️'
  };
  return icons[type] || '📋';
};

// Lifecycle
onMounted(() => {
  loadAllData();

  // Set up auto-refresh every 30 seconds
  refreshTimer = setInterval(() => {
    loadAllData();
  }, 30000);
});

onUnmounted(() => {
  if (refreshTimer) {
    clearInterval(refreshTimer);
  }
});
</script>

<style scoped>
.enhanced-dashboard {
  padding: 20px;
  max-width: 1400px;
  margin: 0 auto;
}

/* Header */
.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.dashboard-header h2 {
  margin: 0;
  color: #333;
  font-size: 24px;
}

.quick-actions {
  display: flex;
  gap: 12px;
}

.refresh-btn,
.export-btn {
  padding: 8px 16px;
  border: 1px solid #1890ff;
  background: #1890ff;
  color: white;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
}

.refresh-btn:hover,
.export-btn:hover {
  background: #40a9ff;
}

.refresh-btn:disabled {
  background: #ccc;
  border-color: #ccc;
  cursor: not-allowed;
}

/* Dashboard Stats */
.dashboard-stats {
  margin-bottom: 32px;
}

.stat-row {
  display: grid;
  gap: 16px;
  margin-bottom: 16px;
}

.stat-row:first-child {
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
}

.stat-row.secondary {
  grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
}

.stat-card {
  background: white;
  border: 1px solid #e8e8e8;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  transition: transform 0.2s;
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0,0,0,0.15);
}

.stat-card.large {
  display: flex;
  align-items: center;
  gap: 16px;
}

.stat-icon {
  font-size: 32px;
  opacity: 0.8;
}

.stat-content {
  flex: 1;
}

.stat-value {
  font-size: 28px;
  font-weight: bold;
  margin-bottom: 4px;
  color: #333;
}

.stat-label {
  font-size: 14px;
  color: #666;
  margin-bottom: 4px;
}

.stat-change {
  font-size: 12px;
  font-weight: 500;
}

.stat-positive {
  color: #52c41a;
}

.stat-negative {
  color: #f5222d;
}

.stat-neutral {
  color: #666;
}

.stat-mini {
  display: flex;
  align-items: center;
  gap: 8px;
  justify-content: center;
  text-align: center;
}

.mini-icon {
  font-size: 20px;
}

.mini-value {
  font-size: 18px;
  font-weight: bold;
  color: #333;
}

.mini-label {
  font-size: 12px;
  color: #666;
}

/* Content Grid */
.content-grid {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 24px;
  margin-bottom: 32px;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.section-header h3 {
  margin: 0;
  color: #333;
  font-size: 18px;
}

.time-selector {
  padding: 6px 12px;
  border: 1px solid #d0d0d0;
  border-radius: 4px;
  font-size: 14px;
}

/* Chart Section */
.chart-section {
  background: white;
  border: 1px solid #e8e8e8;
  border-radius: 8px;
  padding: 20px;
}

.data-type-chart {
  height: 300px;
}

.chart-container {
  display: flex;
  align-items: end;
  justify-content: space-around;
  height: 100%;
  padding: 20px 0;
}

.chart-bar {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 60px;
  position: relative;
  cursor: pointer;
}

.bar-fill {
  width: 40px;
  border-radius: 4px 4px 0 0;
  transition: all 0.3s ease;
  min-height: 10px;
}

.chart-bar:hover .bar-fill {
  opacity: 0.8;
  transform: scale(1.05);
}

.bar-label {
  margin-top: 8px;
  text-align: center;
  font-size: 12px;
}

.bar-type {
  font-weight: bold;
  color: #333;
}

.bar-count {
  color: #666;
  margin: 2px 0;
}

.bar-percentage {
  color: #999;
  font-size: 11px;
}

/* Performance Section */
.performance-section {
  background: white;
  border: 1px solid #e8e8e8;
  border-radius: 8px;
  padding: 20px;
}

.toggle-btn {
  padding: 6px 12px;
  border: 1px solid #1890ff;
  background: transparent;
  color: #1890ff;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
}

.toggle-btn:hover {
  background: #f0f8ff;
}

.performance-metrics {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
  margin-bottom: 16px;
}

.metric-item {
  text-align: center;
}

.metric-label {
  font-size: 12px;
  color: #666;
  margin-bottom: 4px;
}

.metric-value {
  font-size: 18px;
  font-weight: bold;
  color: #333;
}

.metric-value.success {
  color: #52c41a;
}

.metric-value.error {
  color: #f5222d;
}

.performance-details {
  border-top: 1px solid #f0f0f0;
  padding-top: 16px;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
}

.detail-label {
  font-size: 14px;
  color: #666;
}

.detail-value {
  font-size: 14px;
  font-weight: 500;
  color: #333;
}

/* Activity Section */
.activity-section {
  background: white;
  border: 1px solid #e8e8e8;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 24px;
}

.activity-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.activity-header h3 {
  margin: 0;
  color: #333;
  font-size: 18px;
}

.activity-filters {
  display: flex;
  gap: 8px;
}

.filter-btn {
  padding: 6px 12px;
  border: 1px solid #d0d0d0;
  background: white;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.2s;
}

.filter-btn:hover {
  background: #f0f0f0;
}

.filter-btn.active {
  background: #1890ff;
  color: white;
  border-color: #1890ff;
}

.activity-list {
  max-height: 400px;
  overflow-y: auto;
}

.activity-item {
  display: flex;
  align-items: flex-start;
  padding: 16px;
  border-bottom: 1px solid #f0f0f0;
  transition: background 0.2s;
}

.activity-item:hover {
  background: #f8f9fa;
}

.activity-item:last-child {
  border-bottom: none;
}

.activity-icon {
  font-size: 20px;
  margin-right: 12px;
  margin-top: 2px;
}

.activity-content {
  flex: 1;
}

.activity-title {
  font-weight: 500;
  color: #333;
  margin-bottom: 4px;
}

.activity-description {
  font-size: 14px;
  color: #666;
  margin-bottom: 6px;
  line-height: 1.4;
}

.activity-meta {
  display: flex;
  gap: 12px;
  font-size: 12px;
  color: #999;
}

.activity-status {
  margin-left: 12px;
}

.status-badge {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.status-badge.success {
  background: #f6ffed;
  color: #52c41a;
  border: 1px solid #b7eb8f;
}

.status-badge.error {
  background: #fff2f0;
  color: #f5222d;
  border: 1px solid #ffb3b3;
}

.status-badge.warning {
  background: #fff7e6;
  color: #fa8c16;
  border: 1px solid #ffd591;
}

.status-badge.info {
  background: #e6f7ff;
  color: #1890ff;
  border: 1px solid #91d5ff;
}

.no-activities {
  text-align: center;
  padding: 40px;
  color: #666;
}

.no-data-icon {
  font-size: 48px;
  margin-bottom: 8px;
}

/* Quick Access */
.quick-access {
  background: white;
  border: 1px solid #e8e8e8;
  border-radius: 8px;
  padding: 20px;
}

.quick-access h3 {
  margin: 0 0 16px 0;
  color: #333;
  font-size: 18px;
}

.quick-buttons {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 12px;
}

.quick-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 16px;
  border: 1px solid #d0d0d0;
  background: white;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
  text-align: left;
}

.quick-btn:hover {
  background: #f0f8ff;
  border-color: #1890ff;
}

.quick-btn.warning:hover {
  background: #fff7e6;
  border-color: #fa8c16;
}

.btn-icon {
  font-size: 18px;
}

.btn-text {
  font-size: 14px;
  font-weight: 500;
}

/* Loading Overlay */
.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(255, 255, 255, 0.8);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.loading-content {
  text-align: center;
}

.loading-spinner {
  font-size: 32px;
  margin-bottom: 8px;
}

.loading-text {
  font-size: 16px;
  color: #666;
}

/* Responsive */
@media (max-width: 768px) {
  .dashboard-header {
    flex-direction: column;
    gap: 16px;
    align-items: stretch;
  }

  .content-grid {
    grid-template-columns: 1fr;
  }

  .stat-row:first-child {
    grid-template-columns: 1fr 1fr;
  }

  .activity-header {
    flex-direction: column;
    gap: 12px;
    align-items: stretch;
  }

  .activity-filters {
    justify-content: center;
  }

  .quick-buttons {
    grid-template-columns: 1fr;
  }
}
</style>
