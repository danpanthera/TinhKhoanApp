<template>
  <div class="trend-chart" ref="chartContainer" :style="{ height: height + 'px' }"></div>
</template>

<script setup>
import { ref, onMounted, watch, nextTick, onUnmounted } from 'vue';
import * as echarts from 'echarts';

const props = defineProps({
  data: {
    type: Object,
    required: true
  },
  height: {
    type: Number,
    default: 300
  },
  indicators: {
    type: Array,
    default: () => []
  }
});

const chartContainer = ref(null);
let chartInstance = null;

const initChart = () => {
  if (!chartContainer.value) return;

  chartInstance = echarts.init(chartContainer.value);
  updateChart();
};

const updateChart = () => {
  if (!chartInstance || !props.data) return;

  const option = {
    tooltip: {
      trigger: 'axis',
      backgroundColor: 'rgba(50, 50, 50, 0.95)',
      borderColor: 'rgba(255, 255, 255, 0.2)',
      textStyle: {
        color: '#fff',
        fontSize: 13
      },
      formatter: function(params) {
        let result = `<div style="font-weight: 600; margin-bottom: 8px;">${params[0].axisValue}</div>`;
        params.forEach(param => {
          const color = param.color;
          const seriesName = param.seriesName;
          const value = param.value;
          const unit = getUnitBySeriesName(seriesName);
          result += `
            <div style="display: flex; align-items: center; justify-content: space-between; margin: 4px 0;">
              <span style="display: flex; align-items: center;">
                <span style="width: 10px; height: 10px; background: ${color}; border-radius: 50%; margin-right: 8px;"></span>
                ${seriesName}
              </span>
              <span style="font-weight: 600; margin-left: 20px;">${formatValue(value, unit)}</span>
            </div>
          `;
        });
        return result;
      }
    },
    legend: {
      top: 0,
      right: 0,
      textStyle: {
        fontSize: 12,
        color: '#606266'
      },
      itemWidth: 12,
      itemHeight: 8,
      itemGap: 20
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      top: '15%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      data: props.data.labels || [],
      axisLine: {
        lineStyle: {
          color: '#E4E7ED'
        }
      },
      axisTick: {
        lineStyle: {
          color: '#E4E7ED'
        }
      },
      axisLabel: {
        color: '#909399',
        fontSize: 12
      }
    },
    yAxis: [
      {
        type: 'value',
        name: 'Tỷ đồng',
        nameTextStyle: {
          color: '#909399',
          fontSize: 12
        },
        axisLine: {
          show: false
        },
        axisTick: {
          show: false
        },
        axisLabel: {
          color: '#909399',
          fontSize: 12,
          formatter: function(value) {
            if (value >= 1000) {
              return (value / 1000).toFixed(1) + 'K';
            }
            return value;
          }
        },
        splitLine: {
          lineStyle: {
            color: '#F5F7FA',
            type: 'dashed'
          }
        }
      },
      {
        type: 'value',
        name: '%',
        nameTextStyle: {
          color: '#909399',
          fontSize: 12
        },
        position: 'right',
        axisLine: {
          show: false
        },
        axisTick: {
          show: false
        },
        axisLabel: {
          color: '#909399',
          fontSize: 12,
          formatter: '{value}%'
        },
        splitLine: {
          show: false
        }
      }
    ],
    series: generateSeries()
  };

  chartInstance.setOption(option, true);
};

const generateSeries = () => {
  if (!props.data.datasets) return [];

  return props.data.datasets.map(dataset => {
    const isPercentage = dataset.name.includes('Tỷ lệ') || dataset.name.includes('%');
    
    return {
      name: dataset.name,
      type: 'line',
      yAxisIndex: isPercentage ? 1 : 0,
      data: dataset.data,
      smooth: true,
      symbol: 'circle',
      symbolSize: 6,
      lineStyle: {
        width: 3,
        color: dataset.color
      },
      itemStyle: {
        color: dataset.color,
        borderColor: '#fff',
        borderWidth: 2
      },
      areaStyle: {
        color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
          {
            offset: 0,
            color: dataset.color + '40'
          },
          {
            offset: 1,
            color: dataset.color + '10'
          }
        ])
      },
      emphasis: {
        focus: 'series'
      }
    };
  });
};

const getUnitBySeriesName = (seriesName) => {
  if (seriesName.includes('Tỷ lệ') || seriesName.includes('%')) {
    return '%';
  }
  return 'tỷ đồng';
};

const formatValue = (value, unit) => {
  if (unit === '%') {
    return value.toFixed(2) + '%';
  }
  return new Intl.NumberFormat('vi-VN', {
    minimumFractionDigits: 1,
    maximumFractionDigits: 1
  }).format(value) + ' ' + unit;
};

const resize = () => {
  if (chartInstance) {
    chartInstance.resize();
  }
};

// Watch for data changes
watch(() => props.data, () => {
  nextTick(() => {
    updateChart();
  });
}, { deep: true });

onMounted(() => {
  nextTick(() => {
    initChart();
  });
  
  // Add resize listener
  window.addEventListener('resize', resize);
});

onUnmounted(() => {
  if (chartInstance) {
    chartInstance.dispose();
  }
  window.removeEventListener('resize', resize);
});
</script>

<style scoped>
.trend-chart {
  width: 100%;
}
</style>
