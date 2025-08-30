<template>
  <div class="mini-trend-chart" ref="chartContainer">
    <svg :width="width" :height="height" :viewBox="`0 0 ${width} ${height}`">
      <!-- Gradient definition -->
      <defs>
        <linearGradient :id="gradientId" x1="0%" y1="0%" x2="0%" y2="100%">
          <stop offset="0%" :style="`stop-color:${color};stop-opacity:0.8`" />
          <stop offset="100%" :style="`stop-color:${color};stop-opacity:0.1`" />
        </linearGradient>
      </defs>

      <!-- Area fill với gradient -->
      <path v-if="gradient" :d="areaPath" :fill="`url(#${gradientId})`" opacity="0.3" />

      <!-- Line -->
      <path
        :d="linePath"
        :stroke="color"
        :stroke-width="strokeWidth"
        fill="none"
        stroke-linecap="round"
        stroke-linejoin="round"
      />

      <!-- Dots -->
      <circle
        v-for="(point, index) in points"
        :key="index"
        :cx="point.x"
        :cy="point.y"
        :r="3"
        :fill="color"
        :opacity="index === points.length - 1 ? 1 : 0.6"
      />

      <!-- Animated dot for latest point -->
      <circle
        v-if="points.length > 0"
        :cx="points[points.length - 1].x"
        :cy="points[points.length - 1].y"
        r="5"
        :fill="color"
        opacity="0.3"
        class="pulse-dot"
      />
    </svg>
  </div>
</template>

<script setup>
import { computed, ref } from 'vue'

const props = defineProps({
  data: {
    type: Array,
    required: true,
  },
  color: {
    type: String,
    default: '#8B1538',
  },
  width: {
    type: Number,
    default: 200,
  },
  height: {
    type: Number,
    default: 50,
  },
  strokeWidth: {
    type: Number,
    default: 2,
  },
  gradient: {
    type: Boolean,
    default: false,
  },
})

const chartContainer = ref(null)
const gradientId = `gradient-${Math.random().toString(36).substr(2, 9)}`

const normalizedData = computed(() => {
  if (!props.data || props.data.length === 0) return []

  const min = Math.min(...props.data)
  const max = Math.max(...props.data)
  const range = max - min || 1

  return props.data.map(value => (value - min) / range)
})

const points = computed(() => {
  const data = normalizedData.value
  if (data.length === 0) return []

  const padding = 5
  const chartWidth = props.width - padding * 2
  const chartHeight = props.height - padding * 2
  const stepX = chartWidth / (data.length - 1 || 1)

  return data.map((value, index) => ({
    x: padding + index * stepX,
    y: padding + (1 - value) * chartHeight,
  }))
})

const linePath = computed(() => {
  if (points.value.length === 0) return ''

  return points.value.reduce((path, point, index) => {
    const command = index === 0 ? 'M' : 'L'
    return `${path} ${command} ${point.x} ${point.y}`
  }, '')
})

const areaPath = computed(() => {
  if (points.value.length === 0) return ''

  const firstPoint = points.value[0]
  const lastPoint = points.value[points.value.length - 1]

  return `${linePath.value} L ${lastPoint.x} ${props.height - 5} L ${firstPoint.x} ${props.height - 5} Z`
})
</script>

<style scoped>
.mini-trend-chart {
  width: 100%;
  height: 100%;
}

.pulse-dot {
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}

@keyframes pulse {
  0%,
  100% {
    opacity: 0.3;
    r: 5;
  }
  50% {
    opacity: 0.8;
    r: 8;
  }
}
</style>
