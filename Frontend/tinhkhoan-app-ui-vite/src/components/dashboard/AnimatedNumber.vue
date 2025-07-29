<template>
  <span class="animated-number" :class="{ counting: isCounting }">
    {{ displayValue }}
  </span>
</template>

<script setup>
import { onMounted, ref, watch } from 'vue'
import { formatNumber as utilFormatNumber } from '../../utils/numberFormatter.js'

const props = defineProps({
  value: {
    type: Number,
    required: true,
  },
  duration: {
    type: Number,
    default: 2000,
  },
  decimals: {
    type: Number,
    default: 0,
  },
  prefix: {
    type: String,
    default: '',
  },
  suffix: {
    type: String,
    default: '',
  },
  separator: {
    type: String,
    default: ',',
  },
})

const displayValue = ref(0)
const isCounting = ref(false)

// Sử dụng utility formatter thay vì custom logic
const formatNumber = num => {
  const formatted = utilFormatNumber(num, props.decimals, false)
  return props.prefix + formatted + props.suffix
}

const animateNumber = (start, end, duration) => {
  if (start === end) {
    displayValue.value = formatNumber(end)
    return
  }

  isCounting.value = true
  const startTime = performance.now()
  const diff = end - start

  const updateNumber = currentTime => {
    const elapsed = currentTime - startTime
    const progress = Math.min(elapsed / duration, 1)

    // Easing function for smooth animation
    const easeOut = 1 - Math.pow(1 - progress, 3)
    const current = start + diff * easeOut

    displayValue.value = formatNumber(current)

    if (progress < 1) {
      requestAnimationFrame(updateNumber)
    } else {
      displayValue.value = formatNumber(end)
      isCounting.value = false
    }
  }

  requestAnimationFrame(updateNumber)
}

watch(
  () => props.value,
  (newValue, oldValue) => {
    const startValue = oldValue || 0
    animateNumber(startValue, newValue, props.duration)
  },
  { immediate: false }
)

onMounted(() => {
  animateNumber(0, props.value, props.duration)
})
</script>

<style scoped>
.animated-number {
  font-weight: 700;
  font-variant-numeric: tabular-nums;
  transition: all 0.3s ease;
}

.animated-number.counting {
  color: #8b1538;
  text-shadow: 0 0 10px rgba(139, 21, 56, 0.3);
  animation: pulse-glow 1s ease-in-out infinite alternate;
}

@keyframes pulse-glow {
  0% {
    text-shadow: 0 0 5px rgba(139, 21, 56, 0.3);
  }
  100% {
    text-shadow: 0 0 15px rgba(139, 21, 56, 0.6);
  }
}
</style>
