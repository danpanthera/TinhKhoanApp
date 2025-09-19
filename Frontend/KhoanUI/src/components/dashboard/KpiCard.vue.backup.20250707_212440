<template>
  <div
    class="kpi-card"
    :class="cardClass"
    @click="$emit('click', indicator)"
  >
    <!-- Decorative elements -->
    <div class="card-decoration">
      <div class="decoration-circle circle-1"></div>
      <div class="decoration-circle circle-2"></div>
    </div>

    <!-- Header với icon và badge -->
    <div class="card-header">
      <div class="icon-wrapper" :style="{ background: indicator.color + '20' }">
        <i :class="indicator.icon" :style="{ color: indicator.color }"></i>
      </div>
      <div class="card-info">
        <h4 class="card-name">{{ indicator.name }}</h4>
        <span class="card-unit">Đơn vị: {{ indicator.unit }}</span>
      </div>
      <div class="status-badge" :class="statusClass">
        <i :class="statusIcon"></i>
        {{ statusText }}
      </div>
    </div>

    <!-- Main values với animation -->
    <div class="card-values">
      <div class="value-item actual">
        <span class="value-label">Thực hiện</span>
        <span class="value-number">
          <animated-number :value="indicator.actualValue" :format="formatValue" />
        </span>
      </div>
      <div class="value-divider"></div>
      <div class="value-item target">
        <span class="value-label">Kế hoạch</span>
        <span class="value-number">{{ formatValue(indicator.planValue) }}</span>
      </div>
    </div>

    <!-- Progress bar với gradient -->
    <div class="progress-section">
      <div class="progress-header">
        <span class="progress-label">Tiến độ hoàn thành</span>
        <span class="progress-percentage">{{ completionRate.toFixed(1) }}%</span>
      </div>
      <div class="progress-bar-wrapper">
        <div class="progress-bar-bg"></div>
        <div
          class="progress-bar-fill"
          :style="{
            width: completionRate + '%',
            background: progressGradient
          }"
        >
          <span class="progress-glow"></span>
        </div>
      </div>
    </div>

    <!-- Mini trend chart -->
    <div v-if="showTrend && indicator.trend" class="trend-section">
      <div class="trend-header">
        <span class="trend-label">Xu hướng 6 tháng</span>
        <span class="trend-value" :class="trendClass">
          <i :class="trendIcon"></i>
          {{ trendValue }}%
        </span>
      </div>
      <mini-trend-chart
        :data="indicator.trend"
        :color="indicator.color"
        :gradient="true"
        :height="50"
      />
    </div>

    <!-- Footer với thông tin bổ sung -->
    <div class="card-footer">
      <div class="footer-stat">
        <i class="mdi mdi-calendar"></i>
        <span>{{ formatDate(indicator.dataDate) }}</span>
      </div>
      <div class="footer-stat">
        <i class="mdi mdi-refresh"></i>
        <span>Cập nhật: {{ getTimeAgo(indicator.dataDate) }}</span>
      </div>
    </div>

    <!-- Hover effect overlay -->
    <div class="hover-overlay">
      <span class="view-detail">Xem chi tiết <i class="mdi mdi-arrow-right"></i></span>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue';
import AnimatedNumber from './AnimatedNumber.vue';
import MiniTrendChart from './MiniTrendChart.vue';

const props = defineProps({
  indicator: {
    type: Object,
    required: true
  },
  showTrend: {
    type: Boolean,
    default: false
  }
});

const emit = defineEmits(['click']);

// Computed properties
const completionRate = computed(() => {
  if (!props.indicator.planValue || props.indicator.planValue === 0) return 0;

  // Đối với tỷ lệ nợ xấu, càng thấp càng tốt
  if (props.indicator.code === 'TyLeNoXau') {
    if (props.indicator.actualValue <= props.indicator.planValue) {
      return 100; // Đạt mục tiêu
    }
    return Math.min((props.indicator.planValue / props.indicator.actualValue) * 100, 100);
  }

  return Math.min((props.indicator.actualValue / props.indicator.planValue) * 100, 120);
});

const progressGradient = computed(() => {
  const rate = completionRate.value;
  if (rate >= 100) {
    return 'linear-gradient(90deg, #52C41A 0%, #73D13D 100%)';
  } else if (rate >= 80) {
    return 'linear-gradient(90deg, #FAAD14 0%, #FFC53D 100%)';
  } else {
    return 'linear-gradient(90deg, #FF4D4F 0%, #FF7875 100%)';
  }
});

const statusClass = computed(() => {
  const rate = completionRate.value;
  if (rate >= 100) return 'status-success';
  if (rate >= 80) return 'status-warning';
  return 'status-danger';
});

const statusIcon = computed(() => {
  const rate = completionRate.value;
  if (rate >= 100) return 'mdi-check-circle';
  if (rate >= 80) return 'mdi-alert';
  return 'mdi-close-circle';
});

const statusText = computed(() => {
  const rate = completionRate.value;
  if (rate >= 100) return 'Đạt';
  if (rate >= 80) return 'Gần đạt';
  return 'Chưa đạt';
});

const cardClass = computed(() => {
  return {
    'is-clickable': true,
    'is-achieved': completionRate.value >= 100,
    'is-warning': completionRate.value < 100 && completionRate.value >= 80,
    'is-danger': completionRate.value < 80
  };
});

const trendClass = computed(() => {
  if (!props.indicator.yoyGrowth) return '';
  return props.indicator.yoyGrowth >= 0 ? 'trend-up' : 'trend-down';
});

const trendIcon = computed(() => {
  if (!props.indicator.yoyGrowth) return '';
  return props.indicator.yoyGrowth >= 0 ? 'mdi-trending-up' : 'mdi-trending-down';
});

const trendValue = computed(() => {
  if (!props.indicator.yoyGrowth) return '0';
  return Math.abs(props.indicator.yoyGrowth);
});

// Methods
const formatValue = (value) => {
  if (!value && value !== 0) return '-';

  if (props.indicator.unit === '%') {
    return value.toFixed(2) + '%';
  }

  // Format với separator nghìn và triệu
  if (value >= 1000000) {
    return (value / 1000000).toFixed(1) + ' triệu';
  } else if (value >= 1000) {
    return new Intl.NumberFormat('vi-VN').format(value);
  }

  return value.toString();
};

const formatDate = (date) => {
  if (!date) return 'Chưa cập nhật';
  return new Date(date).toLocaleDateString('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric'
  });
};

const getTimeAgo = (date) => {
  if (!date) return 'N/A';

  const now = new Date();
  const past = new Date(date);
  const diffInHours = Math.floor((now - past) / (1000 * 60 * 60));

  if (diffInHours < 1) return 'Vừa xong';
  if (diffInHours < 24) return `${diffInHours}h trước`;
  if (diffInHours < 48) return 'Hôm qua';

  const diffInDays = Math.floor(diffInHours / 24);
  return `${diffInDays} ngày trước`;
};
</script>

<style scoped>
.kpi-card {
  background: white;
  border-radius: 20px;
  padding: 28px;
  position: relative;
  overflow: hidden;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  border: 1px solid rgba(0, 0, 0, 0.06);
  /* ✨ Enhanced visual effects */
  background: linear-gradient(135deg, #ffffff 0%, #fafbff 100%);
}

.kpi-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, #8B1538 0%, #B91D47 100%);
  transform: translateX(-100%);
  transition: transform 0.6s ease;
}

.kpi-card::after {
  /* ✨ Glowing border effect */
  content: '';
  position: absolute;
  top: -2px;
  left: -2px;
  right: -2px;
  bottom: -2px;
  background: linear-gradient(45deg, #8B1538, #B91D47, #8B1538);
  border-radius: 22px;
  z-index: -1;
  opacity: 0;
  filter: blur(10px);
  transition: opacity 0.3s ease;
}

.kpi-card:hover::before {
  transform: translateX(0);
}

.kpi-card:hover::after {
  opacity: 0.3;
  animation: borderGlow 2s ease-in-out infinite alternate;
}

.kpi-card.is-clickable {
  cursor: pointer;
}

.kpi-card:hover {
  transform: translateY(-8px) scale(1.02);
  box-shadow:
    0 20px 50px rgba(139, 21, 56, 0.2),
    0 0 30px rgba(139, 21, 56, 0.1);
}

/* ✨ Achievement status effects */
.kpi-card.is-achieved {
  background: linear-gradient(135deg, #f6ffed 0%, #f0f9ff 100%);
  border-color: rgba(82, 196, 26, 0.3);
}

.kpi-card.is-achieved::after {
  background: linear-gradient(45deg, #52c41a, #73d13d, #52c41a);
}

.kpi-card.is-achieved:hover {
  box-shadow:
    0 20px 50px rgba(82, 196, 26, 0.2),
    0 0 30px rgba(82, 196, 26, 0.1);
}

.kpi-card.is-warning {
  background: linear-gradient(135deg, #fffbe6 0%, #fff7e6 100%);
  border-color: rgba(250, 173, 20, 0.3);
}

.kpi-card.is-warning::after {
  background: linear-gradient(45deg, #faad14, #ffc53d, #faad14);
}

.kpi-card.is-danger {
  background: linear-gradient(135deg, #fff2f0 0%, #fff1f0 100%);
  border-color: rgba(255, 77, 79, 0.3);
}

.kpi-card.is-danger::after {
  background: linear-gradient(45deg, #ff4d4f, #ff7875, #ff4d4f);
}

/* ✨ Spectacular animations */
@keyframes borderGlow {
  0% { filter: blur(8px) brightness(1); }
  100% { filter: blur(12px) brightness(1.2); }
}

@keyframes pulse {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.8; transform: scale(1.05); }
}

@keyframes shimmer {
  0% { transform: translateX(-100%); }
  100% { transform: translateX(100%); }
}

@keyframes float {
  0%, 100% { transform: translateY(0px); }
  50% { transform: translateY(-5px); }
}

@keyframes sparkle {
  0%, 100% { opacity: 0; transform: scale(0) rotate(0deg); }
  50% { opacity: 1; transform: scale(1) rotate(180deg); }
}

/* Decorative elements */
.card-decoration {
  position: absolute;
  top: -20px;
  right: -20px;
  width: 120px;
  height: 120px;
  pointer-events: none;
}

.decoration-circle {
  position: absolute;
  border-radius: 50%;
  opacity: 0.05;
  transition: all 0.3s ease;
}

.kpi-card:hover .decoration-circle {
  opacity: 0.1;
  animation: float 3s ease-in-out infinite;
}

.circle-1 {
  width: 100px;
  height: 100px;
  background: #8B1538;
  top: 0;
  right: 0;
}

.circle-2 {
  width: 60px;
  height: 60px;
  background: #B91D47;
  top: 20px;
  right: 20px;
  animation-delay: 0.5s;
}

/* Header section */
.card-header {
  display: flex;
  align-items: flex-start;
  gap: 15px;
  margin-bottom: 25px;
}

.icon-wrapper {
  width: 56px;
  height: 56px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  transition: all 0.3s ease;
}

.icon-wrapper i {
  font-size: 28px;
  transition: all 0.3s ease;
}

.kpi-card:hover .icon-wrapper {
  transform: scale(1.1) rotate(5deg);
}

.card-info {
  flex: 1;
}

.card-name {
  margin: 0 0 6px 0;
  font-size: 18px;
  font-weight: 600;
  color: #1f2937;
  line-height: 1.4;
}

.card-unit {
  font-size: 13px;
  color: #6b7280;
  font-weight: 400;
}

.status-badge {
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 4px;
  white-space: nowrap;
}

.status-badge i {
  font-size: 14px;
}

.status-success {
  background: #f0f9ff;
  color: #0369a1;
}

.status-warning {
  background: #fef3c7;
  color: #d97706;
}

.status-danger {
  background: #fee2e2;
  color: #dc2626;
}

/* Values section */
.card-values {
  display: flex;
  align-items: center;
  margin-bottom: 25px;
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
  border-radius: 16px;
  padding: 20px;
  position: relative;
  /* ✨ Enhanced with shadows and border */
  box-shadow:
    inset 0 1px 0 rgba(255, 255, 255, 0.8),
    0 2px 8px rgba(0, 0, 0, 0.05);
  border: 1px solid rgba(148, 163, 184, 0.1);
  overflow: hidden;
}

.card-values::before {
  /* ✨ Animated background shimmer */
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(
    90deg,
    transparent,
    rgba(139, 21, 56, 0.03),
    transparent
  );
  animation: valueShimmer 3s ease-in-out infinite;
}

.value-item {
  flex: 1;
  text-align: center;
  position: relative;
  transition: all 0.3s ease;
}

.value-item:hover {
  transform: scale(1.05);
}

.value-label {
  display: block;
  font-size: 13px;
  color: #6b7280;
  margin-bottom: 8px;
  font-weight: 500;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.value-number {
  display: block;
  font-size: 28px;
  font-weight: 800;
  color: #1f2937;
  line-height: 1;
  position: relative;
  /* ✨ Enhanced typography with gradient */
  background: linear-gradient(135deg, #1f2937 0%, #374151 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.value-item.actual .value-number {
  /* ✨ Special effects for actual value */
  background: linear-gradient(135deg, #8B1538 0%, #B91D47 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  position: relative;
}

.value-item.actual .value-number::after {
  /* ✨ Pulse effect for actual value */
  content: '';
  position: absolute;
  inset: -2px;
  border-radius: 8px;
  background: linear-gradient(135deg, #8B1538, #B91D47);
  opacity: 0;
  z-index: -1;
  filter: blur(6px);
  animation: valuePulse 2s ease-in-out infinite;
}

.kpi-card.is-achieved .value-item.actual .value-number::after {
  background: linear-gradient(135deg, #52c41a, #73d13d);
}

.value-divider {
  width: 2px;
  height: 40px;
  background: linear-gradient(180deg, transparent 0%, #e5e7eb 50%, transparent 100%);
  margin: 0 20px;
  position: relative;
  /* ✨ Animated divider */
  overflow: hidden;
}

.value-divider::before {
  content: '';
  position: absolute;
  top: -50%;
  left: 0;
  right: 0;
  height: 100%;
  background: linear-gradient(180deg, transparent, #8B1538, transparent);
  animation: dividerPulse 3s ease-in-out infinite;
}

/* ✨ Additional spectacular animations */
@keyframes valueShimmer {
  0% { left: -100%; }
  50% { left: 100%; }
  100% { left: 100%; }
}

@keyframes valuePulse {
  0%, 100% { opacity: 0; transform: scale(1); }
  50% { opacity: 0.3; transform: scale(1.1); }
}

@keyframes dividerPulse {
  0%, 100% { transform: translateY(150%); }
  50% { transform: translateY(-50%); }
}

/* Progress section */
.progress-section {
  margin-bottom: 20px;
}

.progress-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}

.progress-label {
  font-size: 13px;
  color: #6b7280;
  font-weight: 500;
}

.progress-percentage {
  font-size: 16px;
  font-weight: 700;
  color: #1f2937;
}

.progress-bar-wrapper {
  position: relative;
  height: 12px;
  background: #f3f4f6;
  border-radius: 6px;
  overflow: hidden;
  /* ✨ Enhanced with gradient and glow */
  background: linear-gradient(90deg, #f1f5f9 0%, #e2e8f0 50%, #f1f5f9 100%);
  box-shadow: inset 0 2px 4px rgba(0, 0, 0, 0.1);
}

.progress-bar-bg {
  position: absolute;
  inset: 0;
  background: repeating-linear-gradient(
    45deg,
    transparent,
    transparent 8px,
    rgba(0,0,0,0.03) 8px,
    rgba(0,0,0,0.03) 16px
  );
  /* ✨ Animated background pattern */
  animation: backgroundShift 3s linear infinite;
}

.progress-bar-fill {
  height: 100%;
  border-radius: 6px;
  transition: width 1.5s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  /* ✨ Enhanced with multiple shadows and animation */
  box-shadow:
    0 0 10px rgba(139, 21, 56, 0.4),
    inset 0 1px 0 rgba(255, 255, 255, 0.3);
  animation: progressPulse 2s ease-in-out infinite;
}

.progress-glow {
  position: absolute;
  top: -2px;
  right: 0;
  bottom: -2px;
  width: 60px;
  background: linear-gradient(90deg,
    transparent,
    rgba(255, 255, 255, 0.8),
    rgba(255, 255, 255, 0.9),
    rgba(255, 255, 255, 0.8),
    transparent
  );
  border-radius: 8px;
  animation: shimmer 2.5s ease-in-out infinite;
  filter: blur(1px);
}

/* ✨ Achievement sparkles effect */
.progress-bar-fill::after {
  content: '✨';
  position: absolute;
  right: -8px;
  top: 50%;
  transform: translateY(-50%);
  font-size: 16px;
  opacity: 0;
  animation: sparkle 3s ease-in-out infinite;
}

.kpi-card.is-achieved .progress-bar-fill::after {
  opacity: 1;
}

/* ✨ Enhanced animations */
@keyframes backgroundShift {
  0% { background-position: 0 0; }
  100% { background-position: 32px 0; }
}

@keyframes progressPulse {
  0%, 100% { transform: scaleY(1); }
  50% { transform: scaleY(1.1); }
}

/* Trend section */
.trend-section {
  background: #f9fafb;
  border-radius: 12px;
  padding: 16px;
  margin-bottom: 20px;
}

.trend-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.trend-label {
  font-size: 13px;
  color: #6b7280;
  font-weight: 500;
}

.trend-value {
  font-size: 14px;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 4px;
}

.trend-up {
  color: #10b981;
}

.trend-down {
  color: #ef4444;
}

/* Footer */
.card-footer {
  display: flex;
  justify-content: space-between;
  padding-top: 20px;
  border-top: 1px solid #f3f4f6;
}

.footer-stat {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 12px;
  color: #9ca3af;
}

.footer-stat i {
  font-size: 14px;
}

/* Hover overlay */
.hover-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(139, 21, 56, 0.95) 0%, rgba(185, 29, 71, 0.95) 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  opacity: 0;
  transition: opacity 0.3s ease;
  pointer-events: none;
}

.kpi-card:hover .hover-overlay {
  opacity: 1;
}

.view-detail {
  color: white;
  font-size: 16px;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 8px;
  transform: translateY(10px);
  transition: transform 0.3s ease;
}

.kpi-card:hover .view-detail {
  transform: translateY(0);
}

/* Responsive */
@media (max-width: 480px) {
  .kpi-card {
    padding: 20px;
  }

  .card-values {
    padding: 16px;
  }

  .value-number {
    font-size: 22px;
  }

  .card-name {
    font-size: 16px;
  }
}
</style>
