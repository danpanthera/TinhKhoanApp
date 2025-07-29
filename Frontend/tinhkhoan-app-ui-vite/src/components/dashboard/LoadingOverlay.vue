<template>
  <div class="loading-overlay" v-if="show">
    <div class="loading-backdrop"></div>
    <div class="loading-container">
      <div class="quantum-loader">
        <div class="loader-ring ring-1"></div>
        <div class="loader-ring ring-2"></div>
        <div class="loader-ring ring-3"></div>
        <div class="loader-core">
          <div class="core-icon">{{ icon }}</div>
        </div>
      </div>
      <div class="loading-text">
        <h3>{{ title }}</h3>
        <p>{{ message }}</p>
        <div class="progress-dots">
          <span class="dot"></span>
          <span class="dot"></span>
          <span class="dot"></span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
// defineProps là một macro của compiler, không cần import

const props = defineProps({
  show: {
    type: Boolean,
    default: false,
  },
  title: {
    type: String,
    default: 'Đang xử lý',
  },
  message: {
    type: String,
    default: 'Vui lòng chờ trong giây lát...',
  },
  icon: {
    type: String,
    default: '⚡',
  },
})
</script>

<style scoped>
.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 9999;
  display: flex;
  align-items: center;
  justify-content: center;
}

.loading-backdrop {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.7);
  backdrop-filter: blur(10px);
  animation: fadeIn 0.3s ease-out;
}

.loading-container {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 30px;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 20px;
  padding: 40px;
  backdrop-filter: blur(20px);
  animation: slideInUp 0.5s ease-out;
}

.quantum-loader {
  position: relative;
  width: 120px;
  height: 120px;
}

.loader-ring {
  position: absolute;
  border: 2px solid transparent;
  border-radius: 50%;
  animation: rotateClockwise 2s linear infinite;
}

.ring-1 {
  width: 120px;
  height: 120px;
  border-top-color: #00d4ff;
  border-right-color: #00d4ff;
  animation-duration: 2s;
}

.ring-2 {
  width: 90px;
  height: 90px;
  top: 15px;
  left: 15px;
  border-bottom-color: #ff6b6b;
  border-left-color: #ff6b6b;
  animation-direction: reverse;
  animation-duration: 1.5s;
}

.ring-3 {
  width: 60px;
  height: 60px;
  top: 30px;
  left: 30px;
  border-top-color: #4ecdc4;
  border-bottom-color: #4ecdc4;
  animation-duration: 1s;
}

.loader-core {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 40px;
  height: 40px;
  background: linear-gradient(45deg, #667eea 0%, #764ba2 100%);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  animation: pulse 1.5s ease-in-out infinite;
}

.core-icon {
  font-size: 20px;
  animation: glow 2s ease-in-out infinite alternate;
}

.loading-text {
  text-align: center;
  color: white;
}

.loading-text h3 {
  margin: 0 0 10px 0;
  font-size: 24px;
  font-weight: 600;
  background: linear-gradient(45deg, #00d4ff, #ff6b6b);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.loading-text p {
  margin: 0 0 20px 0;
  opacity: 0.8;
  font-size: 16px;
}

.progress-dots {
  display: flex;
  gap: 8px;
  justify-content: center;
}

.dot {
  width: 8px;
  height: 8px;
  background: #00d4ff;
  border-radius: 50%;
  animation: dotPulse 1.5s ease-in-out infinite;
}

.dot:nth-child(2) {
  animation-delay: 0.2s;
}

.dot:nth-child(3) {
  animation-delay: 0.4s;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes slideInUp {
  from {
    opacity: 0;
    transform: translateY(50px) scale(0.9);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

@keyframes rotateClockwise {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

@keyframes pulse {
  0%,
  100% {
    transform: translate(-50%, -50%) scale(1);
  }
  50% {
    transform: translate(-50%, -50%) scale(1.1);
  }
}

@keyframes glow {
  0% {
    text-shadow: 0 0 5px #ffffff;
  }
  100% {
    text-shadow:
      0 0 20px #ffffff,
      0 0 30px #ffffff;
  }
}

@keyframes dotPulse {
  0%,
  100% {
    transform: scale(1);
    opacity: 1;
  }
  50% {
    transform: scale(1.5);
    opacity: 0.7;
  }
}
</style>
