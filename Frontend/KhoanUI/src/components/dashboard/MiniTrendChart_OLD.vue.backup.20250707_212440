<template>
  <div class="mini-trend-chart" ref="chartContainer"></div>
</template>

<script setup>
import { ref, onMounted, watch, nextTick } from 'vue';

const props = defineProps({
  data: {
    type: Array,
    required: true
  },
  color: {
    type: String,
    default: '#409EFF'
  },
  height: {
    type: Number,
    default: 40
  }
});

const chartContainer = ref(null);

const drawChart = () => {
  const container = chartContainer.value;
  if (!container || !props.data || props.data.length === 0) return;

  // Clear previous content
  container.innerHTML = '';

  const width = container.clientWidth || 200;
  const height = props.height;
  
  // Calculate points
  const minValue = Math.min(...props.data);
  const maxValue = Math.max(...props.data);
  const range = maxValue - minValue || 1;
  
  const points = props.data.map((value, index) => {
    const x = (index / (props.data.length - 1)) * width;
    const y = height - ((value - minValue) / range) * height;
    return `${x},${y}`;
  }).join(' ');

  // Create SVG
  const svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
  svg.setAttribute('width', width);
  svg.setAttribute('height', height);
  svg.setAttribute('viewBox', `0 0 ${width} ${height}`);

  // Create gradient definition
  const defs = document.createElementNS('http://www.w3.org/2000/svg', 'defs');
  const gradient = document.createElementNS('http://www.w3.org/2000/svg', 'linearGradient');
  gradient.setAttribute('id', `gradient-${Math.random().toString(36).substr(2, 9)}`);
  gradient.setAttribute('x1', '0%');
  gradient.setAttribute('y1', '0%');
  gradient.setAttribute('x2', '0%');
  gradient.setAttribute('y2', '100%');

  const stop1 = document.createElementNS('http://www.w3.org/2000/svg', 'stop');
  stop1.setAttribute('offset', '0%');
  stop1.setAttribute('stop-color', props.color);
  stop1.setAttribute('stop-opacity', '0.3');

  const stop2 = document.createElementNS('http://www.w3.org/2000/svg', 'stop');
  stop2.setAttribute('offset', '100%');
  stop2.setAttribute('stop-color', props.color);
  stop2.setAttribute('stop-opacity', '0.05');

  gradient.appendChild(stop1);
  gradient.appendChild(stop2);
  defs.appendChild(gradient);
  svg.appendChild(defs);

  // Create area path
  const areaPath = document.createElementNS('http://www.w3.org/2000/svg', 'path');
  const areaPoints = `M0,${height} L${points} L${width},${height} Z`;
  areaPath.setAttribute('d', areaPoints);
  areaPath.setAttribute('fill', `url(#${gradient.id})`);
  svg.appendChild(areaPath);

  // Create line path
  const linePath = document.createElementNS('http://www.w3.org/2000/svg', 'path');
  linePath.setAttribute('d', `M${points}`);
  linePath.setAttribute('fill', 'none');
  linePath.setAttribute('stroke', props.color);
  linePath.setAttribute('stroke-width', '2');
  linePath.setAttribute('stroke-linecap', 'round');
  linePath.setAttribute('stroke-linejoin', 'round');
  svg.appendChild(linePath);

  // Add dots for data points
  props.data.forEach((value, index) => {
    const x = (index / (props.data.length - 1)) * width;
    const y = height - ((value - minValue) / range) * height;
    
    const dot = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
    dot.setAttribute('cx', x);
    dot.setAttribute('cy', y);
    dot.setAttribute('r', '2');
    dot.setAttribute('fill', props.color);
    dot.setAttribute('stroke', 'white');
    dot.setAttribute('stroke-width', '1');
    svg.appendChild(dot);
  });

  container.appendChild(svg);
};

// Watch for data changes
watch(() => props.data, () => {
  nextTick(() => {
    drawChart();
  });
});

watch(() => props.color, () => {
  nextTick(() => {
    drawChart();
  });
});

onMounted(() => {
  nextTick(() => {
    drawChart();
  });
  
  // Redraw on window resize
  const resizeObserver = new ResizeObserver(() => {
    drawChart();
  });
  
  if (chartContainer.value) {
    resizeObserver.observe(chartContainer.value);
  }
});
</script>

<style scoped>
.mini-trend-chart {
  width: 100%;
  height: 40px;
  overflow: hidden;
}

.mini-trend-chart svg {
  width: 100%;
  height: 100%;
}
</style>
