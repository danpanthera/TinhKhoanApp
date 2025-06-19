<template>
  <div class="optimized-data-table">
    <!-- Performance Stats -->
    <div v-if="showStats" class="stats-section">
      <div class="stats-grid">
        <div class="stat-card">
          <div class="stat-value">{{ formatRecordCount(stats.totalRecords) }}</div>
          <div class="stat-label">Tổng số bản ghi</div>
        </div>
        <div class="stat-card">
          <div class="stat-value">{{ formatRecordCount(stats.loadedRecords) }}</div>
          <div class="stat-label">Đã tải</div>
        </div>
        <div class="stat-card">
          <div class="stat-value" :style="{ color: getPerformanceColor(responseTime) }">
            ⚡ {{ formatPerformanceMetric(responseTime) }}
          </div>
          <div class="stat-label">Thời gian phản hồi</div>
        </div>
        <div class="stat-card">
          <div class="stat-value" :style="{ color: stats.cacheHitRate > 80 ? '#52c41a' : '#faad14' }">
            {{ stats.cacheHitRate }}%
          </div>
          <div class="stat-label">Cache Hit Rate</div>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div v-if="showFilters" class="filters-section">
      <div class="filters-grid">
        <div class="filter-item">
          <input 
            type="text" 
            placeholder="🔍 Tìm kiếm..." 
            v-model="searchInput"
            @input="debouncedSearch"
            class="search-input"
          />
        </div>
        <div class="filter-item">
          <select v-model="selectedDataType" @change="onFilterChange" class="filter-select">
            <option value="">Tất cả loại dữ liệu</option>
            <option v-for="(def, key) in dataTypeDefinitions" :key="key" :value="key">
              {{ def.icon }} {{ key }} - {{ def.description }}
            </option>
          </select>
        </div>
        <div class="filter-item">
          <select v-model="sortBy" @change="onFilterChange" class="filter-select">
            <option value="ImportDate">Ngày import</option>
            <option value="RecordsCount">Số bản ghi</option>
            <option value="FileName">Tên file</option>
          </select>
        </div>
        <div class="filter-item">
          <select v-model="sortOrder" @change="onFilterChange" class="filter-select">
            <option value="desc">Giảm dần</option>
            <option value="asc">Tăng dần</option>
          </select>
        </div>
        <div class="filter-item">
          <button @click="refreshData" :disabled="loading" class="refresh-btn">
            <span v-if="loading">⏳</span>
            <span v-else>🔄</span>
            Làm mới
          </button>
          <button @click="showPerformancePanel = true" class="performance-btn">
            📊 Hiệu suất
          </button>
        </div>
      </div>
    </div>

    <!-- Data Table -->
    <div class="table-container">
      <div v-if="loading && data.length === 0" class="loading-container">
        <div class="loading-spinner">⏳</div>
        <div>Đang tải dữ liệu...</div>
      </div>

      <!-- Virtual Scrolling Table -->
      <div v-else-if="enableVirtualScroll" class="virtual-table">
        <div class="table-header">
          <div class="table-info">
            Đang hiển thị {{ virtualData.length }} / {{ stats.totalRecords }} bản ghi
            <span v-if="isLoadingMore" class="loading-more">⏳ Đang tải thêm...</span>
          </div>
        </div>
        
        <div 
          class="virtual-scroll-container" 
          :style="{ height: height + 'px' }"
          @scroll="handleScroll"
          ref="scrollContainer"
        >
          <div class="virtual-list" :style="{ height: (virtualData.length * ROW_HEIGHT) + 'px' }">
            <div 
              v-for="(record, index) in visibleData" 
              :key="record.id"
              class="virtual-row"
              :style="{ 
                transform: `translateY(${(startIndex + index) * ROW_HEIGHT}px)`,
                height: ROW_HEIGHT + 'px'
              }"
            >
              <div class="row-content">
                <div class="col col-id">{{ record.id }}</div>
                <div class="col col-type">
                  <span class="data-type-tag" :style="{ backgroundColor: getDataTypeColor(record.dataType) }">
                    {{ getDataTypeIcon(record.dataType) }} {{ record.dataType }}
                  </span>
                </div>
                <div class="col col-filename" :title="record.fileName">
                  {{ record.fileName }}
                </div>
                <div class="col col-records">
                  <span class="record-count">{{ formatRecordCount(record.recordsCount) }}</span>
                </div>
                <div class="col col-date">
                  {{ formatDate(record.importDate) }}
                </div>
                <div class="col col-status">
                  <span class="status-tag" :class="getStatusClass(record.status)">
                    {{ record.status }}
                  </span>
                </div>
                <div class="col col-actions">
                  <button @click="viewDetails(record)" class="action-btn" title="Xem chi tiết">👁️</button>
                  <button @click="downloadData(record)" class="action-btn" title="Tải xuống">⬇️</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Regular Paginated Table -->
      <div v-else class="regular-table">
        <table class="data-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Loại dữ liệu</th>
              <th>Tên file</th>
              <th>Số bản ghi</th>
              <th>Ngày import</th>
              <th>Trạng thái</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="record in data" :key="record.id">
              <td>{{ record.id }}</td>
              <td>
                <span class="data-type-tag" :style="{ backgroundColor: getDataTypeColor(record.dataType) }">
                  {{ getDataTypeIcon(record.dataType) }} {{ record.dataType }}
                </span>
              </td>
              <td class="filename-cell" :title="record.fileName">{{ record.fileName }}</td>
              <td>
                <span class="record-count">{{ formatRecordCount(record.recordsCount) }}</span>
              </td>
              <td>{{ formatDate(record.importDate) }}</td>
              <td>
                <span class="status-tag" :class="getStatusClass(record.status)">
                  {{ record.status }}
                </span>
              </td>
              <td>
                <button @click="viewDetails(record)" class="action-btn" title="Xem chi tiết">👁️</button>
                <button @click="downloadData(record)" class="action-btn" title="Tải xuống">⬇️</button>
              </td>
            </tr>
          </tbody>
        </table>

        <!-- Pagination -->
        <div class="pagination">
          <div class="pagination-info">
            {{ (page - 1) * pageSize + 1 }}-{{ Math.min(page * pageSize, total) }} của {{ total }} bản ghi
          </div>
          <div class="pagination-controls">
            <button @click="goToPage(page - 1)" :disabled="page <= 1" class="page-btn">Trước</button>
            <span class="page-numbers">
              <button 
                v-for="pageNum in visiblePages" 
                :key="pageNum"
                @click="goToPage(pageNum)"
                :class="{ active: pageNum === page }"
                class="page-btn"
              >
                {{ pageNum }}
              </button>
            </span>
            <button @click="goToPage(page + 1)" :disabled="page >= totalPages" class="page-btn">Sau</button>
          </div>
          <div class="page-size-selector">
            <select v-model="pageSize" @change="onPageSizeChange" class="page-size-select">
              <option value="25">25/trang</option>
              <option value="50">50/trang</option>
              <option value="100">100/trang</option>
              <option value="200">200/trang</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- Performance Panel -->
    <div v-if="showPerformancePanel" class="performance-panel">
      <div class="panel-backdrop" @click="showPerformancePanel = false"></div>
      <div class="panel-content">
        <div class="panel-header">
          <h3>📊 Thống kê hiệu suất</h3>
          <button @click="showPerformancePanel = false" class="close-btn">×</button>
        </div>
        <div class="panel-body">
          <div v-if="performanceData" class="performance-stats">
            <div class="perf-stat">
              <div class="perf-label">Tổng số truy vấn (24h)</div>
              <div class="perf-value">{{ performanceData.totalQueries }}</div>
            </div>
            <div class="perf-stat">
              <div class="perf-label">Thời gian phản hồi TB</div>
              <div class="perf-value">{{ formatPerformanceMetric(performanceData.avgResponseTime) }}</div>
            </div>
            <div class="perf-stat">
              <div class="perf-label">Cache Hit Rate tổng</div>
              <div class="perf-value">{{ performanceData.cacheHitRate }}%</div>
            </div>
            <div class="perf-stat">
              <div class="perf-label">Slowest Query</div>
              <div class="perf-value">{{ formatPerformanceMetric(performanceData.slowestQuery) }}</div>
            </div>
          </div>
          <div v-else class="loading-performance">
            Đang tải dữ liệu hiệu suất...
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted, watch, nextTick } from 'vue';
import rawDataService from '../../services/rawDataService.js';

// Props
const props = defineProps({
  dataType: {
    type: String,
    default: 'all'
  },
  height: {
    type: Number,
    default: 600
  },
  showFilters: {
    type: Boolean,
    default: true
  },
  showStats: {
    type: Boolean,
    default: true
  },
  enableVirtualScroll: {
    type: Boolean,
    default: true
  }
});

// Constants
const ROW_HEIGHT = 50;
const VISIBLE_ROWS_BUFFER = 5;

// Reactive data
const data = ref([]);
const virtualData = ref([]);
const loading = ref(false);
const isLoadingMore = ref(false);
const searchInput = ref('');
const searchTerm = ref('');
const selectedDataType = ref('');
const sortBy = ref('ImportDate');
const sortOrder = ref('desc');
const page = ref(1);
const pageSize = ref(50);
const total = ref(0);
const responseTime = ref(0);
const hasNextPage = ref(true);
const showPerformancePanel = ref(false);
const performanceData = ref(null);

// Virtual scrolling
const scrollContainer = ref(null);
const startIndex = ref(0);
const visibleRowCount = ref(0);

// Stats
const stats = ref({
  totalRecords: 0,
  loadedRecords: 0,
  avgResponseTime: 0,
  cacheHitRate: 0
});

// Data type definitions
const dataTypeDefinitions = rawDataService.getDataTypeDefinitions();

// Computed
const totalPages = computed(() => Math.ceil(total.value / pageSize.value));

const visiblePages = computed(() => {
  const pages = [];
  const start = Math.max(1, page.value - 2);
  const end = Math.min(totalPages.value, page.value + 2);
  
  for (let i = start; i <= end; i++) {
    pages.push(i);
  }
  return pages;
});

const visibleData = computed(() => {
  if (!props.enableVirtualScroll) return [];
  
  const start = Math.max(0, startIndex.value - VISIBLE_ROWS_BUFFER);
  const end = Math.min(virtualData.value.length, startIndex.value + visibleRowCount.value + VISIBLE_ROWS_BUFFER);
  
  return virtualData.value.slice(start, end);
});

// Methods
const loadData = async (pageNum = page.value, size = pageSize.value, search = searchTerm.value) => {
  loading.value = true;
  const startTime = performance.now();

  try {
    const result = await rawDataService.getOptimizedImports(pageNum, size, search, sortBy.value, sortOrder.value);
    
    if (result.success) {
      data.value = result.data.items || [];
      total.value = result.data.totalCount || 0;
      
      // Update performance metrics
      const endTime = performance.now();
      responseTime.value = endTime - startTime;
      
      stats.value = {
        totalRecords: result.data.totalCount || 0,
        loadedRecords: result.data.items?.length || 0,
        avgResponseTime: (stats.value.avgResponseTime + responseTime.value) / 2,
        cacheHitRate: result.data.cacheHitRate || 0
      };
    } else {
      console.error('Error loading data:', result.error);
    }
  } catch (error) {
    console.error('Error loading data:', error);
  } finally {
    loading.value = false;
  }
};

const loadVirtualData = async (offset = 0, limit = pageSize.value, search = searchTerm.value) => {
  if (offset === 0) {
    virtualData.value = [];
    isLoadingMore.value = false;
  }
  
  if (isLoadingMore.value) return;
  isLoadingMore.value = true;
  
  const startTime = performance.now();

  try {
    const result = await rawDataService.getOptimizedRecords('all', offset, limit, search);
    
    if (result.success) {
      const newData = result.data.items || [];
      
      if (offset === 0) {
        virtualData.value = newData;
      } else {
        virtualData.value = [...virtualData.value, ...newData];
      }
      
      hasNextPage.value = newData.length === limit;
      
      // Update performance metrics
      const endTime = performance.now();
      responseTime.value = endTime - startTime;
      
      stats.value = {
        totalRecords: result.data.totalCount || 0,
        loadedRecords: virtualData.value.length,
        avgResponseTime: (stats.value.avgResponseTime + responseTime.value) / 2,
        cacheHitRate: result.data.cacheHitRate || 0
      };
    }
  } catch (error) {
    console.error('Error loading virtual data:', error);
  } finally {
    isLoadingMore.value = false;
  }
};

const loadPerformanceData = async () => {
  try {
    const result = await rawDataService.getPerformanceStats('24h');
    if (result.success) {
      performanceData.value = result.data;
    }
  } catch (error) {
    console.error('Error loading performance data:', error);
  }
};

const handleScroll = () => {
  if (!scrollContainer.value) return;
  
  const scrollTop = scrollContainer.value.scrollTop;
  const newStartIndex = Math.floor(scrollTop / ROW_HEIGHT);
  startIndex.value = newStartIndex;
  
  // Load more data when near the end
  const remainingRows = virtualData.value.length - (newStartIndex + visibleRowCount.value);
  if (remainingRows < 10 && hasNextPage.value && !isLoadingMore.value) {
    loadVirtualData(virtualData.value.length, pageSize.value);
  }
};

const calculateVisibleRows = () => {
  if (scrollContainer.value) {
    visibleRowCount.value = Math.ceil(scrollContainer.value.clientHeight / ROW_HEIGHT);
  }
};

const debouncedSearch = debounce(() => {
  searchTerm.value = searchInput.value;
  if (props.enableVirtualScroll) {
    loadVirtualData(0, pageSize.value);
  } else {
    page.value = 1;
    loadData();
  }
}, 300);

const onFilterChange = () => {
  if (props.enableVirtualScroll) {
    loadVirtualData(0, pageSize.value);
  } else {
    page.value = 1;
    loadData();
  }
};

const refreshData = () => {
  if (props.enableVirtualScroll) {
    loadVirtualData(0, pageSize.value);
  } else {
    loadData();
  }
};

const goToPage = (pageNum) => {
  if (pageNum >= 1 && pageNum <= totalPages.value) {
    page.value = pageNum;
    loadData();
  }
};

const onPageSizeChange = () => {
  page.value = 1;
  loadData();
};

const viewDetails = (record) => {
  console.log('View details:', record);
  // Implement detail view
};

const downloadData = (record) => {
  console.log('Download data:', record);
  // Implement download
};

// Utility functions
const formatRecordCount = (count) => {
  return rawDataService.formatRecordCount(count);
};

const formatPerformanceMetric = (value) => {
  return rawDataService.formatPerformanceMetric(value);
};

const getPerformanceColor = (time) => {
  return rawDataService.getPerformanceColor(time);
};

const getDataTypeColor = (dataType) => {
  return rawDataService.getDataTypeColor(dataType);
};

const getDataTypeIcon = (dataType) => {
  return dataTypeDefinitions[dataType]?.icon || '📄';
};

const formatDate = (date) => {
  return rawDataService.formatDate(date);
};

const getStatusClass = (status) => {
  return {
    'status-success': status === 'Completed',
    'status-error': status === 'Failed',
    'status-processing': status === 'Processing'
  };
};

// Debounce utility
function debounce(func, wait) {
  let timeout;
  return function executedFunction(...args) {
    const later = () => {
      clearTimeout(timeout);
      func(...args);
    };
    clearTimeout(timeout);
    timeout = setTimeout(later, wait);
  };
}

// Lifecycle
onMounted(() => {
  if (props.enableVirtualScroll) {
    loadVirtualData();
    nextTick(() => {
      calculateVisibleRows();
    });
  } else {
    loadData();
  }
  
  if (props.showStats) {
    loadPerformanceData();
  }
});

onUnmounted(() => {
  // Cleanup if needed
});

// Watchers
watch(() => props.enableVirtualScroll, (newVal) => {
  if (newVal) {
    loadVirtualData();
  } else {
    loadData();
  }
});
</script>

<style scoped>
.optimized-data-table {
  width: 100%;
  margin: 0;
  padding: 0;
}

/* Stats Section */
.stats-section {
  margin-bottom: 16px;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
}

.stat-card {
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 16px;
  text-align: center;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.stat-value {
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 4px;
}

.stat-label {
  font-size: 12px;
  color: #666;
  text-transform: uppercase;
}

/* Filters Section */
.filters-section {
  background: #f8f9fa;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 16px;
  margin-bottom: 16px;
}

.filters-grid {
  display: grid;
  grid-template-columns: 2fr 2fr 1fr 1fr 2fr;
  gap: 12px;
  align-items: center;
}

.search-input,
.filter-select {
  padding: 8px 12px;
  border: 1px solid #d0d0d0;
  border-radius: 4px;
  font-size: 14px;
  width: 100%;
}

.refresh-btn,
.performance-btn {
  padding: 8px 12px;
  border: 1px solid #1890ff;
  background: #1890ff;
  color: white;
  border-radius: 4px;
  cursor: pointer;
  margin-right: 8px;
  font-size: 14px;
}

.refresh-btn:hover,
.performance-btn:hover {
  background: #40a9ff;
}

.refresh-btn:disabled {
  background: #ccc;
  cursor: not-allowed;
}

/* Table Container */
.table-container {
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  overflow: hidden;
}

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 200px;
  font-size: 16px;
}

.loading-spinner {
  font-size: 24px;
  margin-bottom: 8px;
}

/* Virtual Scrolling */
.virtual-table {
  height: 100%;
}

.table-header {
  padding: 12px 16px;
  background: #f8f9fa;
  border-bottom: 1px solid #e0e0e0;
  font-size: 14px;
  color: #666;
}

.loading-more {
  margin-left: 8px;
  font-size: 12px;
}

.virtual-scroll-container {
  overflow-y: auto;
  position: relative;
}

.virtual-list {
  position: relative;
}

.virtual-row {
  position: absolute;
  width: 100%;
  left: 0;
  border-bottom: 1px solid #f0f0f0;
}

.row-content {
  display: grid;
  grid-template-columns: 60px 120px 2fr 100px 150px 100px 100px;
  gap: 12px;
  align-items: center;
  padding: 8px 16px;
  height: 100%;
}

.col {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.col-filename {
  cursor: help;
}

/* Regular Table */
.data-table {
  width: 100%;
  border-collapse: collapse;
}

.data-table th,
.data-table td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #f0f0f0;
}

.data-table th {
  background: #f8f9fa;
  font-weight: 600;
  color: #333;
}

.filename-cell {
  max-width: 300px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  cursor: help;
}

/* Tags and Status */
.data-type-tag {
  display: inline-block;
  padding: 4px 8px;
  border-radius: 4px;
  color: white;
  font-size: 12px;
  font-weight: 500;
}

.record-count {
  background: #e6f7ff;
  color: #1890ff;
  padding: 2px 6px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 500;
}

.status-tag {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.status-success {
  background: #f6ffed;
  color: #52c41a;
  border: 1px solid #b7eb8f;
}

.status-error {
  background: #fff2f0;
  color: #ff4d4f;
  border: 1px solid #ffb3b3;
}

.status-processing {
  background: #fff7e6;
  color: #fa8c16;
  border: 1px solid #ffd591;
}

/* Action Buttons */
.action-btn {
  background: none;
  border: 1px solid #d0d0d0;
  padding: 4px 8px;
  margin: 0 2px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
}

.action-btn:hover {
  background: #f0f0f0;
}

/* Pagination */
.pagination {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px;
  background: #f8f9fa;
  border-top: 1px solid #e0e0e0;
}

.pagination-controls {
  display: flex;
  align-items: center;
  gap: 8px;
}

.page-btn {
  padding: 6px 12px;
  border: 1px solid #d0d0d0;
  background: white;
  cursor: pointer;
  border-radius: 4px;
}

.page-btn:hover {
  background: #f0f0f0;
}

.page-btn.active {
  background: #1890ff;
  color: white;
  border-color: #1890ff;
}

.page-btn:disabled {
  background: #f5f5f5;
  color: #ccc;
  cursor: not-allowed;
}

.page-size-select {
  padding: 6px 8px;
  border: 1px solid #d0d0d0;
  border-radius: 4px;
}

/* Performance Panel */
.performance-panel {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 1000;
}

.panel-backdrop {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
}

.panel-content {
  position: absolute;
  right: 0;
  top: 0;
  bottom: 0;
  width: 400px;
  background: white;
  box-shadow: -2px 0 8px rgba(0, 0, 0, 0.15);
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px;
  border-bottom: 1px solid #e0e0e0;
}

.close-btn {
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
  padding: 4px;
}

.panel-body {
  padding: 16px;
}

.performance-stats {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.perf-stat {
  padding: 16px;
  background: #f8f9fa;
  border-radius: 8px;
}

.perf-label {
  font-size: 12px;
  color: #666;
  margin-bottom: 4px;
}

.perf-value {
  font-size: 18px;
  font-weight: bold;
  color: #333;
}

.loading-performance {
  text-align: center;
  padding: 32px;
  color: #666;
}

/* Responsive */
@media (max-width: 768px) {
  .filters-grid {
    grid-template-columns: 1fr;
  }
  
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .row-content {
    grid-template-columns: 1fr;
    gap: 4px;
  }
  
  .panel-content {
    width: 100%;
  }
}
</style>
