<template>
  <div class="kpi-card" :class="cardClass" @click="$emit('click', indicator)">
    <!-- Background gradient overlay -->
    <div class="card-background" :style="{ background: gradientBackground }"></div>

    <!-- Card content -->
    <div class="card-content">
      <!-- Header với icon và title -->
      <div class="card-header">
        <div class="icon-container" :style="{ backgroundColor: indicator.color + '20', color: indicator.color }">
          <i :class="indicator.icon"></i>
        </div>
        <div class="card-title">
          <h4>{{ indicator.Name }}</h4>
          <span class="unit">{{ indicator.unit }}</span>
        </div>
        <div class="status-badge" :class="statusClass">
          <i :class="statusIcon"></i>
        </div>
      </div>

      <!-- Giá trị chính -->
      <div class="card-value">
        <div class="actual-value">
          <span class="number">{{ formatValue(indicator.actualValue) }}</span>
          <span class="unit-text">{{ indicator.unit }}</span>
        </div>
        <div class="target-info">
          <span class="target-label">Kế hoạch:</span>
          <span class="target-value">{{ formatValue(indicator.planValue) }}</span>
        </div>
      </div>

      <!-- Thanh tiến độ với animation -->
      <div class="card-progress">
        <div class="progress-info">
          <span class="progress-label">Hoàn thành</span>
          <span class="progress-percentage" :style="{ color: progressColor }"> {{ completionRate.toFixed(1) }}% </span>
        </div>
        <div class="progress-bar-container">
          <div
            class="progress-bar"
            :style="{
              width: Math.min(completionRate, 100) + '%',
              backgroundColor: progressColor,
              boxShadow: `0 2px 8px ${progressColor}40`,
            }"
          ></div>
          <!-- Overflow indicator for > 100% -->
          <div v-if="completionRate > 100" class="overflow-indicator" :style="{ backgroundColor: progressColor }">
            +{{ (completionRate - 100).toFixed(1) }}%
          </div>
        </div>
      </div>

      <!-- Mini trend chart -->
      <div v-if="showTrend && indicator.trend" class="card-trend">
        <div class="trend-header">
          <span class="trend-label">Xu hướng 6 tháng</span>
          <span class="trend-change" :class="trendClass">
            <i :class="trendIcon"></i>
            {{ Math.abs(trendChange).toFixed(1) }}%
          </span>
        </div>
        <mini-trend-chart :data="indicator.trend" :color="indicator.color" />
      </div>

      <!-- Footer với thông tin bổ sung -->
      <div class="card-footer">
        <div class="footer-stat">
          <div class="stat-label">So với cùng kỳ</div>
          <div class="stat-value" :class="yoyClass">
            <i :class="yoyIcon"></i>
            {{ Math.abs(indicator.yoyGrowth).toFixed(1) }}%
          </div>
        </div>
        <div class="footer-stat">
          <div class="stat-label">Cập nhật</div>
          <div class="stat-value">{{ formatDate(indicator.dataDate) }}</div>
        </div>
        <div class="footer-stat">
          <div class="stat-label">Trạng thái</div>
          <div class="stat-value" :class="statusClass">{{ statusText }}</div>
        </div>
      </div>
    </div>

    <!-- Click ripple effect -->
    <div class="ripple-container">
      <div class="ripple"></div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import MiniTrendChart from './MiniTrendChart.vue'

const props = defineProps({
  indicator: {
    type: Object,
    required: true,
  },
  showTrend: {
    type: Boolean,
    default: false,
  },
})

const emit = defineEmits(['click'])

// Computed properties
const completionRate = computed(() => {
  if (!props.indicator.planValue || props.indicator.planValue === 0) return 0

  // Special logic for bad debt ratio (lower is better)
  if (props.indicator.Code === 'TyLeNoXau') {
    if (props.indicator.actualValue <= props.indicator.planValue) {
      return 100 // Target achieved
    }
    return (props.indicator.planValue / props.indicator.actualValue) * 100
  }

  return (props.indicator.actualValue / props.indicator.planValue) * 100
})

const progressColor = computed(() => {
  const rate = completionRate.value
  if (rate >= 100) return '#67C23A' // Green - achieved
  if (rate >= 90) return '#95D475' // Light green - close
  if (rate >= 80) return '#E6A23C' // Orange - warning
  if (rate >= 60) return '#F89406' // Dark orange
  return '#F56C6C' // Red - danger
})

const statusClass = computed(() => {
  const rate = completionRate.value
  if (rate >= 100) return 'success'
  if (rate >= 80) return 'warning'
  return 'danger'
})

const statusIcon = computed(() => {
  const rate = completionRate.value
  if (rate >= 100) return 'mdi-check-circle'
  if (rate >= 80) return 'mdi-alert-circle'
  return 'mdi-close-circle'
})

const statusText = computed(() => {
  const rate = completionRate.value
  if (rate >= 100) return 'Đạt KH'
  if (rate >= 80) return 'Gần đạt'
  return 'Chưa đạt'
})

const cardClass = computed(() => {
  return {
    clickable: true,
    completed: completionRate.value >= 100,
    warning: completionRate.value < 100 && completionRate.value >= 80,
    danger: completionRate.value < 80,
  }
})

const gradientBackground = computed(() => {
  const baseColor = props.indicator.color
  const opacity = completionRate.value >= 100 ? '08' : '04'
  return `linear-gradient(135deg, ${baseColor}${opacity} 0%, ${baseColor}${opacity} 100%)`
})

const trendChange = computed(() => {
  if (!props.indicator.trend || props.indicator.trend.length < 2) return 0
  const first = props.indicator.trend[0]
  const last = props.indicator.trend[props.indicator.trend.length - 1]
  return ((last - first) / first) * 100
})

const trendClass = computed(() => {
  return trendChange.value >= 0 ? 'positive' : 'negative'
})

const trendIcon = computed(() => {
  return trendChange.value >= 0 ? 'mdi-trending-up' : 'mdi-trending-down'
})

const yoyClass = computed(() => {
  return props.indicator.yoyGrowth >= 0 ? 'positive' : 'negative'
})

const yoyIcon = computed(() => {
  return props.indicator.yoyGrowth >= 0 ? 'mdi-arrow-up' : 'mdi-arrow-down'
})

// Methods
const formatValue = value => {
  if (!value && value !== 0) return '0'

  if (props.indicator.unit === '%') {
    return value.toFixed(2)
  }

  // Format large numbers
  if (value >= 1000) {
    return new Intl.NumberFormat('vi-VN', {
      minimumFractionDigits: 1,
      maximumFractionDigits: 1,
    }).format(value)
  }

  return new Intl.NumberFormat('vi-VN').format(value)
}

const formatDate = date => {
  if (!date) return '-'
  return new Date(date).toLocaleDateString('vi-VN', {
    day: '2-digit',
    month: '2-digit',
  })
}
</script>

<style scoped>
.kpi-card {
  position: relative;
  background: white;
  border-radius: 16px;
  padding: 0;
  box-shadow:
    0 4px 20px rgba(0, 0, 0, 0.08),
    0 2px 8px rgba(0, 0, 0, 0.04);
  border: 1px solid rgba(255, 255, 255, 0.2);
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  overflow: hidden;
  cursor: pointer;
}

.kpi-card:hover {
  transform: translateY(-8px) scale(1.02);
  box-shadow:
    0 12px 40px rgba(0, 0, 0, 0.15),
    0 8px 16px rgba(0, 0, 0, 0.1);
}

.kpi-card.completed {
  border-left: 4px solid #67c23a;
}

.kpi-card.warning {
  border-left: 4px solid #e6a23c;
}

.kpi-card.danger {
  border-left: 4px solid #f56c6c;
}

.card-background {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 0;
}

.card-content {
  position: relative;
  z-index: 1;
  padding: 24px;
}

.card-header {
  display: flex;
  align-items: flex-start;
  gap: 16px;
  margin-bottom: 20px;
}

.icon-container {
  width: 48px;
  height: 48px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 24px;
  flex-shrink: 0;
}

.card-title {
  flex: 1;
}

.card-title h4 {
  margin: 0 0 4px 0;
  font-size: 16px;
  font-weight: 600;
  color: #303133;
  line-height: 1.4;
}

.card-title .unit {
  font-size: 12px;
  color: #909399;
  font-weight: 500;
}

.Status-badge {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 24px;
  height: 24px;
  border-radius: 50%;
  font-size: 14px;
}

.Status-badge.success {
  background-color: #f0f9ff;
  color: #67c23a;
}

.Status-badge.warning {
  background-color: #fdf6ec;
  color: #e6a23c;
}

.Status-badge.danger {
  background-color: #fef0f0;
  color: #f56c6c;
}

.card-value {
  margin-bottom: 20px;
}

.actual-value {
  display: flex;
  align-items: baseline;
  gap: 8px;
  margin-bottom: 8px;
}

.actual-value .number {
  font-size: 32px;
  font-weight: 700;
  color: #303133;
  line-height: 1;
}

.actual-value .unit-text {
  font-size: 14px;
  color: #909399;
  font-weight: 500;
}

.target-info {
  display: flex;
  align-items: center;
  gap: 8px;
}

.target-label {
  font-size: 14px;
  color: #606266;
}

.target-value {
  font-size: 16px;
  font-weight: 600;
  color: #909399;
}

.card-progress {
  margin-bottom: 20px;
}

.progress-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.progress-label {
  font-size: 14px;
  color: #606266;
  font-weight: 500;
}

.progress-percentage {
  font-size: 16px;
  font-weight: 700;
}

.progress-bar-container {
  position: relative;
  height: 8px;
  background-color: #f5f7fa;
  border-radius: 4px;
  overflow: visible;
}

.progress-bar {
  height: 100%;
  border-radius: 4px;
  transition: all 0.6s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
}

.progress-bar::after {
  content: '';
  position: absolute;
  top: 0;
  right: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent 0%, rgba(255, 255, 255, 0.3) 50%, transparent 100%);
  animation: shimmer 2s infinite;
  border-radius: 4px;
}

@keyframes shimmer {
  0% {
    transform: translateX(-100%);
  }
  100% {
    transform: translateX(100%);
  }
}

.overflow-indicator {
  position: absolute;
  top: -4px;
  right: -8px;
  padding: 2px 6px;
  border-radius: 8px;
  font-size: 10px;
  font-weight: 600;
  color: white;
  font-family: monospace;
}

.card-trend {
  margin-bottom: 20px;
  padding: 12px;
  background-color: #fafbfc;
  border-radius: 8px;
  border: 1px solid #ebeef5;
}

.trend-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 8px;
}

.trend-label {
  font-size: 12px;
  color: #909399;
  font-weight: 500;
}

.trend-change {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 12px;
  font-weight: 600;
}

.trend-change.positive {
  color: #67c23a;
}

.trend-change.negative {
  color: #f56c6c;
}

.card-footer {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 16px;
  padding-top: 16px;
  border-top: 1px solid #f0f2f5;
}

.footer-stat {
  text-align: center;
}

.stat-label {
  font-size: 11px;
  color: #909399;
  font-weight: 500;
  margin-bottom: 4px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.stat-value {
  font-size: 13px;
  font-weight: 600;
  color: #606266;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 4px;
}

.stat-value.positive {
  color: #67c23a;
}

.stat-value.negative {
  color: #f56c6c;
}

.stat-value.success {
  color: #67c23a;
}

.stat-value.warning {
  color: #e6a23c;
}

.stat-value.danger {
  color: #f56c6c;
}

.ripple-container {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  overflow: hidden;
  border-radius: 16px;
}

.kpi-card:active .ripple {
  animation: ripple-effect 0.6s ease-out;
}

@keyframes ripple-effect {
  0% {
    transform: scale(0);
    opacity: 0.6;
  }
  100% {
    transform: scale(2);
    opacity: 0;
  }
}

.ripple {
  position: absolute;
  border-radius: 50%;
  background-color: rgba(255, 255, 255, 0.6);
  width: 100px;
  height: 100px;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%) scale(0);
}

/* Responsive design */
@media (max-width: 768px) {
  .card-content {
    padding: 20px;
  }

  .actual-value .number {
    font-size: 28px;
  }

  .card-footer {
    grid-template-columns: repeat(2, 1fr);
    gap: 12px;
  }

  .card-header {
    gap: 12px;
  }

  .icon-container {
    width: 40px;
    height: 40px;
    font-size: 20px;
  }
}

@media (max-width: 480px) {
  .card-content {
    padding: 16px;
  }

  .actual-value .number {
    font-size: 24px;
  }

  .card-footer {
    grid-template-columns: 1fr;
    gap: 8px;
  }
}
</style>
