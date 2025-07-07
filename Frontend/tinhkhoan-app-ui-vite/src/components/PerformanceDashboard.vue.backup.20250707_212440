<template>
  <div class="performance-dashboard">
    <div class="dashboard-header">
      <h1>🚀 Performance Dashboard</h1>
      <div class="refresh-controls">
        <button @click="refreshAll" :disabled="isLoading" class="btn btn-primary">
          {{ isLoading ? 'Refreshing...' : 'Refresh All' }}
        </button>
        <button @click="resetCounters" class="btn btn-warning">
          Reset Counters
        </button>
        <div class="auto-refresh">
          <label>
            <input 
              type="checkbox" 
              v-model="autoRefresh"
              @change="toggleAutoRefresh"
            />
            Auto-refresh ({{ refreshInterval / 1000 }}s)
          </label>
        </div>
      </div>
    </div>

    <!-- System Metrics -->
    <div class="metrics-section">
      <h2>💻 System Metrics</h2>
      <div class="metrics-grid">
        <div class="metric-card">
          <h3>Memory Usage</h3>
          <div class="metric-value">
            {{ formatBytes(systemMetrics?.workingSetMB * 1024 * 1024) }}
          </div>
          <div class="metric-subtitle">Working Set</div>
        </div>
        
        <div class="metric-card">
          <h3>GC Memory</h3>
          <div class="metric-value">
            {{ formatBytes(systemMetrics?.gcTotalMemoryMB * 1024 * 1024) }}
          </div>
          <div class="metric-subtitle">Garbage Collector</div>
        </div>
        
        <div class="metric-card">
          <h3>Threads</h3>
          <div class="metric-value">{{ systemMetrics?.threadCount || 0 }}</div>
          <div class="metric-subtitle">Active Threads</div>
        </div>
        
        <div class="metric-card">
          <h3>GC Collections</h3>
          <div class="metric-value">
            G0: {{ systemMetrics?.gen0Collections || 0 }}<br>
            G1: {{ systemMetrics?.gen1Collections || 0 }}<br>
            G2: {{ systemMetrics?.gen2Collections || 0 }}
          </div>
          <div class="metric-subtitle">By Generation</div>
        </div>
      </div>
    </div>

    <!-- Cache Metrics -->
    <div class="metrics-section">
      <h2>🔄 Cache Performance</h2>
      <div class="metrics-grid">
        <div class="metric-card">
          <h3>Hit Rate</h3>
          <div class="metric-value success">
            {{ cacheMetrics?.hitRate || 0 }}%
          </div>
          <div class="metric-subtitle">Cache Efficiency</div>
        </div>
        
        <div class="metric-card">
          <h3>Cache Hits</h3>
          <div class="metric-value">{{ cacheMetrics?.hits || 0 }}</div>
          <div class="metric-subtitle">Successful Hits</div>
        </div>
        
        <div class="metric-card">
          <h3>Cache Misses</h3>
          <div class="metric-value">{{ cacheMetrics?.misses || 0 }}</div>
          <div class="metric-subtitle">Cache Misses</div>
        </div>
        
        <div class="metric-card">
          <h3>Hit Ratio Chart</h3>
          <div class="progress-chart">
            <div class="progress-bar">
              <div 
                class="progress-fill success" 
                :style="{ width: (cacheMetrics?.hitRate || 0) + '%' }"
              ></div>
            </div>
          </div>
          <div class="metric-subtitle">Visual Representation</div>
        </div>
      </div>
    </div>

    <!-- Export Metrics -->
    <div class="metrics-section">
      <h2>📊 Export Operations</h2>
      <div class="metrics-grid">
        <div class="metric-card">
          <h3>Active Exports</h3>
          <div class="metric-value warning">{{ exportMetrics?.active || 0 }}</div>
          <div class="metric-subtitle">Currently Running</div>
        </div>
        
        <div class="metric-card">
          <h3>Completed</h3>
          <div class="metric-value success">{{ exportMetrics?.completed || 0 }}</div>
          <div class="metric-subtitle">Successfully Finished</div>
        </div>
        
        <div class="metric-card">
          <h3>Failed</h3>
          <div class="metric-value error">{{ exportMetrics?.failed || 0 }}</div>
          <div class="metric-subtitle">With Errors</div>
        </div>
        
        <div class="metric-card">
          <h3>Success Rate</h3>
          <div class="metric-value" :class="getSuccessRateClass(exportMetrics?.successRate)">
            {{ exportMetrics?.successRate || 0 }}%
          </div>
          <div class="metric-subtitle">Overall Success</div>
        </div>
      </div>
    </div>

    <!-- Database Metrics -->
    <div class="metrics-section" v-if="databaseMetrics">
      <h2>🗄️ Database Performance</h2>
      <div class="metrics-grid">
        <div class="metric-card">
          <h3>Query Count</h3>
          <div class="metric-value">{{ databaseMetrics.queryCount || 0 }}</div>
          <div class="metric-subtitle">Total Queries</div>
        </div>
        
        <div class="metric-card">
          <h3>Avg Query Time</h3>
          <div class="metric-value">{{ databaseMetrics.avgQueryTime || 0 }}ms</div>
          <div class="metric-subtitle">Average Duration</div>
        </div>
        
        <div class="metric-card">
          <h3>Slow Queries</h3>
          <div class="metric-value warning">{{ databaseMetrics.slowQueries || 0 }}</div>
          <div class="metric-subtitle">&gt; 1000ms</div>
        </div>
        
        <div class="metric-card">
          <h3>Connection Pool</h3>
          <div class="metric-value">{{ databaseMetrics.activeConnections || 0 }}</div>
          <div class="metric-subtitle">Active Connections</div>
        </div>
      </div>
    </div>

    <!-- Real-time Chart -->
    <div class="metrics-section">
      <h2>📈 Real-time Performance</h2>
      <div class="chart-container">
        <canvas ref="performanceChart" width="800" height="300"></canvas>
      </div>
    </div>

    <!-- Recent Activity Log -->
    <div class="metrics-section">
      <h2>📝 Recent Activity</h2>
      <div class="activity-log">
        <div 
          v-for="(activity, index) in recentActivity" 
          :key="index"
          class="activity-item"
          :class="activity.type"
        >
          <span class="activity-time">{{ formatTime(activity.timestamp) }}</span>
          <span class="activity-message">{{ activity.message }}</span>
        </div>
        <div v-if="recentActivity.length === 0" class="no-activity">
          No recent activity
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, onMounted, onUnmounted } from 'vue'

export default {
  name: 'PerformanceDashboard',
  setup() {
    const isLoading = ref(false)
    const autoRefresh = ref(true)
    const refreshInterval = ref(5000) // 5 seconds
    const refreshTimer = ref(null)
    
    const systemMetrics = ref(null)
    const cacheMetrics = ref(null)
    const exportMetrics = ref(null)
    const databaseMetrics = ref(null)
    const recentActivity = ref([])
    
    const performanceChart = ref(null)
    const chartData = reactive({
      labels: [],
      memoryData: [],
      cpuData: [],
      requestsData: []
    })

    const baseUrl = 'http://localhost:5123/api/PerformanceDashboard'

    const fetchSystemMetrics = async () => {
      try {
        const response = await fetch(`${baseUrl}/system`)
        const data = await response.json()
        systemMetrics.value = data.system
        
        addActivity('info', 'System metrics updated')
        return data.system
      } catch (error) {
        console.error('Error fetching system metrics:', error)
        addActivity('error', 'Failed to fetch system metrics')
        return null
      }
    }

    const fetchCacheMetrics = async () => {
      try {
        const response = await fetch(`${baseUrl}/cache`)
        const data = await response.json()
        cacheMetrics.value = data.cache
        
        addActivity('info', `Cache hit rate: ${data.cache.hitRate}%`)
        return data.cache
      } catch (error) {
        console.error('Error fetching cache metrics:', error)
        addActivity('error', 'Failed to fetch cache metrics')
        return null
      }
    }

    const fetchExportMetrics = async () => {
      try {
        const response = await fetch(`${baseUrl}/exports`)
        const data = await response.json()
        exportMetrics.value = data.exports
        
        if (data.exports.active > 0) {
          addActivity('warning', `${data.exports.active} active exports`)
        }
        return data.exports
      } catch (error) {
        console.error('Error fetching export metrics:', error)
        addActivity('error', 'Failed to fetch export metrics')
        return null
      }
    }

    const fetchDatabaseMetrics = async () => {
      try {
        const response = await fetch(`${baseUrl}/database`)
        const data = await response.json()
        databaseMetrics.value = data.database
        return data.database
      } catch (error) {
        console.error('Error fetching database metrics:', error)
        return null
      }
    }

    const refreshAll = async () => {
      isLoading.value = true
      try {
        await Promise.all([
          fetchSystemMetrics(),
          fetchCacheMetrics(), 
          fetchExportMetrics(),
          fetchDatabaseMetrics()
        ])
        
        updateChart()
        addActivity('success', 'All metrics refreshed')
      } catch (error) {
        console.error('Error refreshing metrics:', error)
        addActivity('error', 'Failed to refresh some metrics')
      } finally {
        isLoading.value = false
      }
    }

    const resetCounters = async () => {
      try {
        const response = await fetch(`${baseUrl}/reset`, { method: 'POST' })
        if (response.ok) {
          addActivity('info', 'Performance counters reset')
          await refreshAll()
        }
      } catch (error) {
        console.error('Error resetting counters:', error)
        addActivity('error', 'Failed to reset counters')
      }
    }

    const toggleAutoRefresh = () => {
      if (autoRefresh.value) {
        startAutoRefresh()
      } else {
        stopAutoRefresh()
      }
    }

    const startAutoRefresh = () => {
      stopAutoRefresh()
      refreshTimer.value = setInterval(() => {
        refreshAll()
      }, refreshInterval.value)
    }

    const stopAutoRefresh = () => {
      if (refreshTimer.value) {
        clearInterval(refreshTimer.value)
        refreshTimer.value = null
      }
    }

    const updateChart = () => {
      const now = new Date().toLocaleTimeString()
      
      chartData.labels.push(now)
      chartData.memoryData.push(systemMetrics.value?.workingSetMB || 0)
      chartData.requestsData.push(cacheMetrics.value?.hits || 0)
      
      // Keep only last 20 data points
      if (chartData.labels.length > 20) {
        chartData.labels.shift()
        chartData.memoryData.shift()
        chartData.requestsData.shift()
      }
      
      drawChart()
    }

    const drawChart = () => {
      const canvas = performanceChart.value
      if (!canvas) return
      
      const ctx = canvas.getContext('2d')
      const width = canvas.width
      const height = canvas.height
      
      // Clear canvas
      ctx.clearRect(0, 0, width, height)
      
      // Draw background
      ctx.fillStyle = '#f8f9fa'
      ctx.fillRect(0, 0, width, height)
      
      // Draw grid
      ctx.strokeStyle = '#e9ecef'
      ctx.lineWidth = 1
      
      for (let i = 0; i <= 10; i++) {
        const y = (height / 10) * i
        ctx.beginPath()
        ctx.moveTo(0, y)
        ctx.lineTo(width, y)
        ctx.stroke()
      }
      
      // Draw memory usage line
      if (chartData.memoryData.length > 1) {
        ctx.strokeStyle = '#007bff'
        ctx.lineWidth = 2
        ctx.beginPath()
        
        const maxMemory = Math.max(...chartData.memoryData) || 1
        
        chartData.memoryData.forEach((value, index) => {
          const x = (width / (chartData.memoryData.length - 1)) * index
          const y = height - (value / maxMemory) * height
          
          if (index === 0) {
            ctx.moveTo(x, y)
          } else {
            ctx.lineTo(x, y)
          }
        })
        
        ctx.stroke()
      }
    }

    const addActivity = (type, message) => {
      recentActivity.value.unshift({
        type,
        message,
        timestamp: new Date()
      })
      
      // Keep only last 50 activities
      if (recentActivity.value.length > 50) {
        recentActivity.value.pop()
      }
    }

    const formatBytes = (bytes) => {
      if (bytes === 0) return '0 B'
      const k = 1024
      const sizes = ['B', 'KB', 'MB', 'GB']
      const i = Math.floor(Math.log(bytes) / Math.log(k))
      return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
    }

    const formatTime = (timestamp) => {
      return timestamp.toLocaleTimeString()
    }

    const getSuccessRateClass = (rate) => {
      rate = rate || 0
      if (rate >= 95) return 'success'
      if (rate >= 80) return 'warning'
      return 'error'
    }

    onMounted(() => {
      refreshAll()
      if (autoRefresh.value) {
        startAutoRefresh()
      }
    })

    onUnmounted(() => {
      stopAutoRefresh()
    })

    return {
      isLoading,
      autoRefresh,
      refreshInterval,
      systemMetrics,
      cacheMetrics,
      exportMetrics,
      databaseMetrics,
      recentActivity,
      performanceChart,
      refreshAll,
      resetCounters,
      toggleAutoRefresh,
      formatBytes,
      formatTime,
      getSuccessRateClass
    }
  }
}
</script>

<style scoped>
.performance-dashboard {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
  padding-bottom: 15px;
  border-bottom: 2px solid #e9ecef;
}

.dashboard-header h1 {
  margin: 0;
  color: #2c3e50;
}

.refresh-controls {
  display: flex;
  gap: 15px;
  align-items: center;
}

.auto-refresh {
  display: flex;
  align-items: center;
  gap: 5px;
}

.metrics-section {
  margin-bottom: 40px;
}

.metrics-section h2 {
  color: #34495e;
  margin-bottom: 20px;
  padding-bottom: 10px;
  border-bottom: 1px solid #ecf0f1;
}

.metrics-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
}

.metric-card {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  border: 1px solid #e9ecef;
  transition: transform 0.2s ease;
}

.metric-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 20px rgba(0,0,0,0.15);
}

.metric-card h3 {
  margin: 0 0 10px 0;
  color: #6c757d;
  font-size: 14px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.metric-value {
  font-size: 24px;
  font-weight: bold;
  margin-bottom: 5px;
  color: #2c3e50;
}

.metric-value.success { color: #28a745; }
.metric-value.warning { color: #ffc107; }
.metric-value.error { color: #dc3545; }

.metric-subtitle {
  font-size: 12px;
  color: #6c757d;
}

.progress-chart {
  margin: 10px 0;
}

.progress-bar {
  width: 100%;
  height: 20px;
  background-color: #e9ecef;
  border-radius: 10px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  transition: width 0.3s ease;
}

.progress-fill.success { background-color: #28a745; }
.progress-fill.warning { background-color: #ffc107; }
.progress-fill.error { background-color: #dc3545; }

.chart-container {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

.activity-log {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  max-height: 300px;
  overflow-y: auto;
}

.activity-item {
  display: flex;
  justify-content: space-between;
  padding: 8px 0;
  border-bottom: 1px solid #f8f9fa;
}

.activity-item:last-child {
  border-bottom: none;
}

.activity-time {
  font-size: 12px;
  color: #6c757d;
  min-width: 80px;
}

.activity-message {
  flex: 1;
  margin-left: 15px;
}

.activity-item.success .activity-message { color: #28a745; }
.activity-item.warning .activity-message { color: #ffc107; }
.activity-item.error .activity-message { color: #dc3545; }
.activity-item.info .activity-message { color: #17a2b8; }

.no-activity {
  text-align: center;
  color: #6c757d;
  font-style: italic;
}

.btn {
  padding: 8px 16px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.2s;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-primary {
  background-color: #007bff;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background-color: #0056b3;
}

.btn-warning {
  background-color: #ffc107;
  color: #212529;
}

.btn-warning:hover {
  background-color: #e0a800;
}
</style>
