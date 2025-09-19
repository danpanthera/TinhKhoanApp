<template>
  <div class="comparison-chart" ref="chartContainer" :style="{ height: height + 'px' }"></div>
</template>

<script setup>
import { ref, onMounted, watch, nextTick, onUnmounted } from 'vue';
import * as echarts from 'echarts';

const props = defineProps({
  data: {
    type: Array,
    required: true
  },
  height: {
    type: Number,
    default: 300
  },
  type: {
    type: String,
    default: 'bar' // bar, radar, pie
  }
});

const chartContainer = ref(null);
let chartInstance = null;

const colors = [
  '#4CAF50', '#2196F3', '#FF9800', '#F44336', '#9C27B0', '#00BCD4',
  '#8BC34A', '#3F51B5', '#FFC107', '#E91E63', '#607D8B', '#795548'
];

const initChart = () => {
  if (!chartContainer.value) return;

  chartInstance = echarts.init(chartContainer.value);
  updateChart();
};

const updateChart = () => {
  if (!chartInstance || !props.data || props.data.length === 0) return;

  let option = {};

  switch (props.type) {
    case 'bar':
      option = getBarChartOption();
      break;
    case 'radar':
      option = getRadarChartOption();
      break;
    case 'pie':
      option = getPieChartOption();
      break;
    default:
      option = getBarChartOption();
  }

  chartInstance.setOption(option, true);
};

const getBarChartOption = () => {
  const indicators = ['HuyDong', 'DuNo', 'LoiNhuan'];
  const units = props.data.map(item => item.name);

  return {
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
          const value = param.value;
          const unit = getIndicatorUnit(param.seriesName);
          result += `
            <div style="display: flex; align-items: center; justify-content: space-between; margin: 4px 0;">
              <span style="display: flex; align-items: center;">
                <span style="width: 10px; height: 10px; background: ${param.color}; border-radius: 2px; margin-right: 8px;"></span>
                ${param.seriesName}
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
      itemHeight: 8
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
      data: units,
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
        fontSize: 12,
        interval: 0,
        rotate: units.length > 6 ? 45 : 0
      }
    },
    yAxis: {
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
            return (value / 1000).toFixed(0) + 'K';
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
    series: indicators.map((indicator, index) => ({
      name: getIndicatorName(indicator),
      type: 'bar',
      data: props.data.map(item => item[indicator] || 0),
      itemStyle: {
        color: colors[index],
        borderRadius: [4, 4, 0, 0]
      },
      emphasis: {
        itemStyle: {
          color: colors[index],
          shadowBlur: 10,
          shadowColor: colors[index] + '50'
        }
      }
    }))
  };
};

const getRadarChartOption = () => {
  const indicators = [
    { name: 'Nguồn vốn huy động', max: 6000 },
    { name: 'Dư nợ cho vay', max: 5000 },
    { name: 'Lợi nhuận', max: 300 },
    { name: 'Thu dịch vụ', max: 150 },
    { name: 'Thu hồi nợ XLRR', max: 80 }
  ];

  return {
    tooltip: {
      trigger: 'item',
      backgroundColor: 'rgba(50, 50, 50, 0.95)',
      borderColor: 'rgba(255, 255, 255, 0.2)',
      textStyle: {
        color: '#fff',
        fontSize: 13
      }
    },
    legend: {
      top: 0,
      right: 0,
      textStyle: {
        fontSize: 12,
        color: '#606266'
      }
    },
    radar: {
      indicator: indicators,
      center: ['50%', '55%'],
      radius: '60%',
      axisName: {
        color: '#606266',
        fontSize: 12
      },
      splitArea: {
        areaStyle: {
          color: ['rgba(250, 250, 250, 0.1)', 'rgba(200, 200, 200, 0.1)']
        }
      },
      axisLine: {
        lineStyle: {
          color: '#E4E7ED'
        }
      },
      splitLine: {
        lineStyle: {
          color: '#E4E7ED'
        }
      }
    },
    series: {
      type: 'radar',
      data: props.data.slice(0, 5).map((item, index) => ({
        value: [
          item.HuyDong || 0,
          item.DuNo || 0,
          item.LoiNhuan || 0,
          item.ThuDichVu || 0,
          item.ThuHoiXLRR || 0
        ],
        name: item.name,
        itemStyle: {
          color: colors[index]
        },
        areaStyle: {
          color: colors[index] + '20'
        }
      }))
    }
  };
};

const getPieChartOption = () => {
  const total = props.data.reduce((sum, item) => sum + (item.HuyDong || 0), 0);
  
  return {
    tooltip: {
      trigger: 'item',
      backgroundColor: 'rgba(50, 50, 50, 0.95)',
      borderColor: 'rgba(255, 255, 255, 0.2)',
      textStyle: {
        color: '#fff',
        fontSize: 13
      },
      formatter: '{a} <br/>{b}: {c} ({d}%)'
    },
    legend: {
      top: '5%',
      left: 'center',
      textStyle: {
        fontSize: 12,
        color: '#606266'
      }
    },
    series: {
      name: 'Nguồn vốn huy động',
      type: 'pie',
      radius: ['30%', '70%'],
      center: ['50%', '60%'],
      avoidLabelOverlap: false,
      itemStyle: {
        borderRadius: 8,
        borderColor: '#fff',
        borderWidth: 2
      },
      label: {
        show: false,
        position: 'center'
      },
      emphasis: {
        label: {
          show: true,
          fontSize: 14,
          fontWeight: 'bold'
        },
        itemStyle: {
          shadowBlur: 10,
          shadowOffsetX: 0,
          shadowColor: 'rgba(0, 0, 0, 0.5)'
        }
      },
      labelLine: {
        show: false
      },
      data: props.data.map((item, index) => ({
        value: item.HuyDong || 0,
        name: item.name,
        itemStyle: {
          color: colors[index % colors.length]
        }
      }))
    }
  };
};

const getIndicatorName = (code) => {
  const names = {
    HuyDong: 'Nguồn vốn huy động',
    DuNo: 'Dư nợ cho vay',
    LoiNhuan: 'Lợi nhuận',
    ThuDichVu: 'Thu dịch vụ',
    ThuHoiXLRR: 'Thu hồi nợ XLRR'
  };
  return names[code] || code;
};

const getIndicatorUnit = (name) => {
  return 'tỷ đồng';
};

const formatValue = (value, unit) => {
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

watch(() => props.type, () => {
  nextTick(() => {
    updateChart();
  });
});

onMounted(() => {
  nextTick(() => {
    initChart();
  });
  
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
.comparison-chart {
  width: 100%;
}
</style>
