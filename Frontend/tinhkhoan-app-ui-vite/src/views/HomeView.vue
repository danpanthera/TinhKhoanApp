<template>
  <div class="home">
    <div class="welcome-hero">
      <div class="hero-content">
        <h1 class="hero-title">
          <img src="/src/assets/Logo-Agribank-2.png" alt="Agribank Logo" class="hero-logo" />
          <br>
          <div class="text-container">
            <span 
              ref="adaptiveTextLine1" 
              class="hero-text adaptive-text-line-1"
            >
              AGRIBANK LAI CHAU CENTER
            </span>
          </div>
        </h1>
        <div class="hero-subtitle">
          <div class="text-container">
            <span 
              ref="adaptiveTextLine2"
              class="hero-text adaptive-text-line-2"
            >
              HỆ THỐNG QUẢN LÝ KHOÁN | HỆ THỐNG BÁO CÁO
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import { isAuthenticated } from '../services/auth';

const router = useRouter();

// Refs cho 2 dòng text
const adaptiveTextLine1 = ref(null);
const adaptiveTextLine2 = ref(null);

// Weather state
const currentWeather = ref('sunny'); // sunny, rainy, cloudy, snowy, stormy
const weatherEffects = ref('');

// ☀️ Get current weather (thực tế theo thời gian và tọa độ Lai Châu)
const getCurrentWeather = async () => {
  try {
    const hour = new Date().getHours();
    const month = new Date().getMonth() + 1; // 1-12
    const day = new Date().getDate();
    
    // 🌍 Thời tiết thực tế theo vùng Lai Châu (miền núi phía Bắc)
    // Mùa khô (tháng 10-4): ít mưa, nhiều nắng, sương mù buổi sáng
    // Mùa mưa (tháng 5-9): mưa nhiều, ít nắng, nhiều mây
    
    if (month >= 10 || month <= 4) {
      // Mùa khô
      if (hour >= 6 && hour <= 8) {
        // Sáng sớm: hay có sương mù
        currentWeather.value = Math.random() < 0.6 ? 'cloudy' : 'clear-night';
      } else if (hour >= 9 && hour <= 17) {
        // Ban ngày: chủ yếu nắng
        currentWeather.value = Math.random() < 0.7 ? 'sunny' : 'cloudy';
      } else {
        // Tối: trời trong
        currentWeather.value = Math.random() < 0.8 ? 'clear-night' : 'cloudy';
      }
    } else {
      // Mùa mưa (5-9)
      if (hour >= 6 && hour <= 11) {
        // Sáng: ít mưa hơn
        currentWeather.value = Math.random() < 0.4 ? 'sunny' : (Math.random() < 0.6 ? 'cloudy' : 'rainy');
      } else if (hour >= 12 && hour <= 18) {
        // Chiều: hay có mưa giông
        currentWeather.value = Math.random() < 0.3 ? 'stormy' : (Math.random() < 0.6 ? 'rainy' : 'cloudy');
      } else {
        // Tối/đêm: mưa nhẹ
        currentWeather.value = Math.random() < 0.5 ? 'rainy' : 'cloudy';
      }
    }
    
    // 🌨️ Mùa đông có thể có sương giá (tháng 12-2)
    if ((month === 12 || month === 1 || month === 2) && hour >= 5 && hour <= 7) {
      if (Math.random() < 0.3) {
        currentWeather.value = 'snowy'; // Sương giá/băng
      }
    }
    
    console.log(`🌤️ Lai Châu weather - Month: ${month}, Hour: ${hour}, Weather: ${currentWeather.value}`);
  } catch (error) {
    console.error('Weather API error:', error);
    currentWeather.value = 'sunny'; // Default fallback
  }
};

// Apply weather effects to text
const applyWeatherEffects = () => {
  const textElement = adaptiveTextLine1.value;
  if (!textElement) return;
  
  // Remove existing weather classes
  textElement.classList.remove('weather-sunny', 'weather-rainy', 'weather-cloudy', 'weather-snowy', 'weather-stormy', 'weather-clear-night');
  
  // Add current weather class
  textElement.classList.add(`weather-${currentWeather.value}`);
  
  console.log('🎨 Applied weather effect for Lai Châu:', currentWeather.value);
};

// 🎯 SIÊU PERFECT: Hàm tính toán và áp dụng scale cho từng dòng text để fit cửa sổ HOÀN HẢO
const adjustTextSize = (element) => {
  if (!element) return;
  
  try {
    // Lấy container của element
    const container = element.closest('.text-container');
    if (!container) return;
    
    // Reset hoàn toàn styles để đo kích thước gốc
    element.style.transform = 'none';
    element.style.fontSize = '';
    element.style.width = 'auto';
    element.style.maxWidth = 'none';
    
    // Force multiple reflows để đảm bảo kích thước được tính lại hoàn toàn
    container.offsetHeight;
    element.offsetWidth;
    void container.offsetWidth;
    
    // Đợi một chút để DOM stabilize
    requestAnimationFrame(() => {
      // Lấy kích thước viewport và container
      const viewportWidth = window.innerWidth;
      const viewportHeight = window.innerHeight;
      const containerWidth = container.offsetWidth;
      const containerRect = container.getBoundingClientRect();
      
      // Lấy kích thước thực tế của text
      const textWidth = element.scrollWidth;
      const textHeight = element.offsetHeight;
      
      // 🎯 SIÊU ADAPTIVE: Tính toán maxWidth dựa trên device type và orientation
      let maxAllowedWidth;
      const isMobile = viewportWidth < 768;
      const isTablet = viewportWidth >= 768 && viewportWidth < 1024;
      const isLandscape = viewportWidth > viewportHeight;
      
      if (isMobile) {
        // Mobile: sử dụng 90-95% viewport tùy theo orientation
        maxAllowedWidth = viewportWidth * (isLandscape ? 0.95 : 0.90);
      } else if (isTablet) {
        // Tablet: sử dụng 85-90% viewport
        maxAllowedWidth = viewportWidth * (isLandscape ? 0.90 : 0.85);
      } else {
        // Desktop: sử dụng container width hoặc 92% viewport, lấy nhỏ hơn
        maxAllowedWidth = Math.min(containerWidth - 40, viewportWidth * 0.92);
      }
      
      // Đảm bảo maxWidth không quá nhỏ
      maxAllowedWidth = Math.max(maxAllowedWidth, 200);
      
      // Tính toán scale
      let scale = 1;
      if (textWidth > maxAllowedWidth) {
        scale = maxAllowedWidth / textWidth;
      }
      
      // 🎯 SIÊU ADAPTIVE: Giới hạn scale dựa trên device và text content
      const textContent = element.textContent || '';
      const isMainTitle = textContent.includes('AGRIBANK LAI CHAU');
      
      let minScale, maxScale;
      if (isMobile) {
        minScale = isMainTitle ? 0.15 : 0.2; // Cho phép thu nhỏ nhiều hơn trên mobile
        maxScale = 1.5; // Có thể phóng to trên mobile nhỏ
      } else if (isTablet) {
        minScale = isMainTitle ? 0.25 : 0.3;
        maxScale = 1.2;
      } else {
        minScale = isMainTitle ? 0.3 : 0.4;
        maxScale = 1;
      }
      
      scale = Math.max(minScale, Math.min(maxScale, scale));
      
      // 🎯 SIÊU CENTER: Áp dụng transform với origin center hoàn hảo
      element.style.transform = `scale(${scale})`;
      element.style.transformOrigin = 'center center';
      element.style.position = 'relative';
      element.style.display = 'inline-block';
      element.style.whiteSpace = 'nowrap';
      
      // 🎯 SIÊU CENTER: Đảm bảo container center hoàn hảo
      container.style.display = 'flex';
      container.style.justifyContent = 'center';
      container.style.alignItems = 'center';
      container.style.overflow = 'visible';
      container.style.textAlign = 'center';
      container.style.width = '100%';
      
      // 🎯 SIÊU RESPONSIVE: Điều chỉnh spacing và padding dựa trên scale
      const scaledWidth = textWidth * scale;
      const extraSpace = containerWidth - scaledWidth;
      
      if (extraSpace > 0) {
        container.style.padding = `10px ${Math.min(extraSpace / 4, 20)}px`;
      } else {
        container.style.padding = isMobile ? '5px 2px' : '10px 5px';
      }
      
      // 🎯 SIÊU LOG: Chi tiết để debug
      console.log(`🎯 SIÊU AUTO-FIT: "${textContent.substring(0, 20)}..."`);
      console.log(`📱 Device: ${isMobile ? 'Mobile' : isTablet ? 'Tablet' : 'Desktop'} ${isLandscape ? '(Landscape)' : '(Portrait)'}`);
      console.log(`📏 Viewport: ${viewportWidth}x${viewportHeight}, Container: ${containerWidth}px`);
      console.log(`📐 Text: ${textWidth}px → Scaled: ${scaledWidth.toFixed(1)}px (scale: ${scale.toFixed(3)})`);
      console.log(`✅ Success: Fit ${((scaledWidth / maxAllowedWidth) * 100).toFixed(1)}% of allowed width`);
      
    });
    
  } catch (error) {
    console.error('🚨 Error in adjustTextSize:', error);
    // Fallback: basic scaling
    if (element) {
      element.style.transform = 'scale(0.8)';
      element.style.transformOrigin = 'center center';
    }
  }
};

// Hàm chính để điều chỉnh cả hai dòng
const autoAdjustAllText = () => {
  adjustTextSize(adaptiveTextLine1.value);
  adjustTextSize(adaptiveTextLine2.value);
};

// Debounce function
const debounce = (func, wait) => {
  let timeout;
  return (...args) => {
    clearTimeout(timeout);
    timeout = setTimeout(() => func(...args), wait);
  };
};

// Debounced version cho performance - nhanh hơn cho responsive
const debouncedAdjust = debounce(autoAdjustAllText, 150); // Giảm từ 300ms xuống 150ms

// 🎯 Immediate adjust không debounce cho resize quan trọng
const immediateAdjust = () => {
  requestAnimationFrame(() => {
    autoAdjustAllText();
    applyWeatherEffects();
  });
};

// Observer để theo dõi thay đổi kích thước
let resizeObserver = null;

onMounted(async () => {
  if (!isAuthenticated()) {
    router.push('/login');
    return;
  }
  
  // Get weather and apply effects
  await getCurrentWeather();
  
  // Khởi tạo text sizing với delay
  setTimeout(() => {
    autoAdjustAllText();
    applyWeatherEffects();
  }, 100);
  
  setTimeout(() => {
    autoAdjustAllText();
    applyWeatherEffects();
  }, 500);
  
  // Theo dõi sự kiện resize window với immediate response
  window.addEventListener('resize', () => {
    immediateAdjust(); // Immediate cho resize quan trọng
    debouncedAdjust(); // Debounce backup
  });
  
  // Theo dõi orientation change trên mobile
  window.addEventListener('orientationchange', () => {
    setTimeout(immediateAdjust, 200); // Delay ngắn cho orientation change
  });
  
  // Dùng ResizeObserver để theo dõi thay đổi kích thước container
  if (window.ResizeObserver) {
    resizeObserver = new ResizeObserver(() => {
      debouncedAdjust();
      applyWeatherEffects();
    });
    
    // Theo dõi cả hai container
    const containers = document.querySelectorAll('.text-container');
    containers.forEach(container => {
      resizeObserver.observe(container);
    });
  }
  
  console.log('🎨 Weather-based adaptive text system initialized');
});

onUnmounted(() => {
  // Cleanup
  window.removeEventListener('resize', immediateAdjust);
  window.removeEventListener('resize', debouncedAdjust);
  window.removeEventListener('orientationchange', immediateAdjust);
  
  if (resizeObserver) {
    resizeObserver.disconnect();
  }
  
  console.log('🧹 Weather text system cleaned up');
});
</script>

<style scoped>
.home {
  padding: 0;
  margin: 0;
  min-height: calc(100vh - 120px);
  background: transparent !important;
  overflow-x: hidden;
  width: 100%;
  box-sizing: border-box;
}

.welcome-hero {
  text-align: center;
  padding: 0 0 20px 0;
  background: transparent;
  min-height: 100vh;
  display: flex;
  align-items: flex-start;
  justify-content: center;
  padding-top: 2vh;
  width: 100%;
  overflow-x: hidden;
  box-sizing: border-box;
}

.hero-content {
  width: 100%;
  margin: 0 auto;
  background: transparent;
  padding: 0 20px;
  box-sizing: border-box;
  overflow: hidden;
  max-width: 100vw;
  position: relative;
}

/* 🎯 SIÊU RESPONSIVE: Container cho từng dòng text - FIT HOÀN HẢO MỌI THIẾT BỊ */
.text-container {
  width: 100%;
  max-width: 100vw;
  display: flex;
  justify-content: center;
  align-items: center;
  overflow: visible; /* Cho phép hiệu ứng thời tiết tràn ra */
  margin: 0 auto;
  height: auto;
  min-height: 50px; /* Tối thiểu cho mobile */
  position: relative;
  padding: 8px 4px; /* Padding nhỏ gọn hơn */
  box-sizing: border-box;
  text-align: center;
}

/* 🎯 SIÊU RESPONSIVE: CSS chung cho hero text - CENTER & SCALE PERFECT */
.hero-text {
  display: inline-block;
  white-space: nowrap;
  text-transform: uppercase;
  transform-origin: center center; /* Đảm bảo scale từ center */
  transition: transform 0.15s cubic-bezier(0.25, 0.46, 0.45, 0.94); /* Nhanh hơn cho responsive */
  will-change: transform;
  text-overflow: clip;
  position: relative;
  margin: 0 auto;
  text-align: center;
  line-height: 1.1; /* Tối ưu line height */
  vertical-align: middle;
  /* Đảm bảo không bị ảnh hưởng bởi font size của parent */
  font-size: inherit;
  /* Đảm bảo width tự động */
  width: auto;
  max-width: none;
}

/* Dòng 1: AGRIBANK LAI CHAU CENTER - HIỆU ỨNG THỜI TIẾT SIÊU HIỆN ĐẠI */
.adaptive-text-line-1 {
  font-size: clamp(1.8rem, 8vw, 6.5rem);
  letter-spacing: clamp(0.02em, 0.5vw, 0.08em);
  transform: scale(1);
  font-family: 'Montserrat', 'Roboto', sans-serif;
  font-weight: 800;
  /* Màu cố định đỏ bordeaux */
  color: #8B1538;
  /* Hiệu ứng cơ bản */
  text-shadow: 
    0 0 20px rgba(139, 21, 56, 0.6),
    0 4px 8px rgba(139, 21, 56, 0.4),
    0 2px 4px rgba(0, 0, 0, 0.3);
  /* Base animation */
  animation: agribank-base-glow 4s ease-in-out infinite;
  will-change: transform, text-shadow, filter;
  word-spacing: clamp(-0.05em, 0vw, 0.02em);
  position: relative;
  /* Responsive scaling */
  max-width: 100%;
  width: fit-content;
  white-space: nowrap;
  overflow: visible;
}

/* ☀️ HIỆU ỨNG NẮNG (SUNNY) */
.adaptive-text-line-1.weather-sunny {
  color: #8B1538;
  text-shadow: 
    0 0 30px rgba(255, 215, 0, 0.8),
    0 0 40px rgba(139, 21, 56, 0.6),
    0 4px 8px rgba(255, 165, 0, 0.4),
    0 2px 4px rgba(0, 0, 0, 0.3);
  animation: 
    sunny-rays 3s linear infinite,
    agribank-base-glow 4s ease-in-out infinite;
}

.adaptive-text-line-1.weather-sunny::before {
  content: '';
  position: absolute;
  top: -20px;
  left: -20px;
  right: -20px;
  bottom: -20px;
  background: 
    radial-gradient(circle at 30% 30%, rgba(255, 215, 0, 0.3) 2px, transparent 2px),
    radial-gradient(circle at 70% 20%, rgba(255, 165, 0, 0.4) 1px, transparent 1px),
    radial-gradient(circle at 60% 80%, rgba(255, 140, 0, 0.2) 1.5px, transparent 1.5px);
  animation: sunny-sparkle 2s ease-in-out infinite;
  pointer-events: none;
  z-index: -1;
}

@keyframes sunny-rays {
  0% { filter: brightness(1) hue-rotate(0deg); }
  25% { filter: brightness(1.2) hue-rotate(10deg); }
  50% { filter: brightness(1.1) hue-rotate(0deg); }
  75% { filter: brightness(1.3) hue-rotate(-10deg); }
  100% { filter: brightness(1) hue-rotate(0deg); }
}

@keyframes sunny-sparkle {
  0%, 100% { opacity: 0.7; transform: scale(1) rotate(0deg); }
  50% { opacity: 1; transform: scale(1.1) rotate(180deg); }
}

/* 🌧️ HIỆU ỨNG MÚA THỰC TẾ (RAINY) - Cải thiện đẹp và như thật */
.adaptive-text-line-1.weather-rainy {
  color: #8B1538;
  text-shadow: 
    0 0 25px rgba(100, 149, 237, 0.8),
    0 0 35px rgba(139, 21, 56, 0.7),
    0 4px 8px rgba(70, 130, 180, 0.6),
    0 2px 4px rgba(25, 25, 112, 0.4),
    0 1px 2px rgba(0, 0, 0, 0.5);
  animation: 
    rainy-atmosphere 2s ease-in-out infinite,
    agribank-base-glow 4s ease-in-out infinite;
  filter: contrast(1.1) brightness(0.95);
}

.adaptive-text-line-1.weather-rainy::before {
  content: '';
  position: absolute;
  top: -40px;
  left: -30px;
  right: -30px;
  bottom: -20px;
  background-image: 
    /* Mưa rơi - nhiều lớp cho hiệu ứng thật */
    linear-gradient(180deg, transparent 0%, rgba(135, 206, 250, 0.1) 20%, transparent 21%),
    linear-gradient(185deg, transparent 0%, rgba(100, 149, 237, 0.2) 1px, transparent 2px),
    linear-gradient(175deg, transparent 0%, rgba(70, 130, 180, 0.15) 1px, transparent 2px),
    linear-gradient(190deg, transparent 0%, rgba(65, 105, 225, 0.1) 1px, transparent 2px),
    /* Giọt mưa chi tiết */
    radial-gradient(ellipse 1px 4px at 15% 10%, rgba(135, 206, 250, 0.6), transparent),
    radial-gradient(ellipse 0.8px 3px at 25% 30%, rgba(100, 149, 237, 0.5), transparent),
    radial-gradient(ellipse 1.2px 5px at 35% 20%, rgba(70, 130, 180, 0.4), transparent),
    radial-gradient(ellipse 0.6px 2.5px at 45% 40%, rgba(135, 206, 250, 0.7), transparent),
    radial-gradient(ellipse 1px 4px at 55% 15%, rgba(100, 149, 237, 0.5), transparent),
    radial-gradient(ellipse 0.9px 3.5px at 65% 35%, rgba(70, 130, 180, 0.6), transparent),
    radial-gradient(ellipse 1.1px 4.5px at 75% 25%, rgba(135, 206, 250, 0.4), transparent),
    radial-gradient(ellipse 0.7px 3px at 85% 45%, rgba(100, 149, 237, 0.6), transparent);
  background-size: 
    100% 100%, 8px 12px, 6px 10px, 10px 14px,
    80px 100px, 60px 80px, 70px 90px, 50px 70px,
    90px 110px, 65px 85px, 75px 95px, 55px 75px;
  animation: realistic-rain-fall 0.6s linear infinite;
  pointer-events: none;
  z-index: -1;
  opacity: 0.8;
}

@keyframes rainy-atmosphere {
  0%, 100% { filter: contrast(1.1) brightness(0.95) blur(0px); }
  25% { filter: contrast(1.15) brightness(0.9) blur(0.2px); }
  50% { filter: contrast(1.1) brightness(1) blur(0px); }
  75% { filter: contrast(1.05) brightness(0.92) blur(0.1px); }
}

@keyframes realistic-rain-fall {
  0% { 
    transform: translateY(-15px) translateX(2px); 
    opacity: 0.8; 
  }
  25% { 
    opacity: 1; 
  }
  75% { 
    opacity: 0.9; 
  }
  100% { 
    transform: translateY(25px) translateX(-3px); 
    opacity: 0.3; 
  }
}

/* ☁️ HIỆU ỨNG MÂY (CLOUDY) */
.adaptive-text-line-1.weather-cloudy {
  color: #8B1538;
  text-shadow: 
    0 0 20px rgba(169, 169, 169, 0.6),
    0 0 30px rgba(139, 21, 56, 0.5),
    0 4px 8px rgba(128, 128, 128, 0.4),
    0 2px 4px rgba(0, 0, 0, 0.3);
  animation: 
    cloudy-drift 6s ease-in-out infinite,
    agribank-base-glow 4s ease-in-out infinite;
}

.adaptive-text-line-1.weather-cloudy::before {
  content: '';
  position: absolute;
  top: -15px;
  left: -25px;
  right: -25px;
  bottom: -15px;
  background: 
    radial-gradient(ellipse at 20% 40%, rgba(192, 192, 192, 0.3) 30%, transparent 50%),
    radial-gradient(ellipse at 80% 30%, rgba(169, 169, 169, 0.2) 25%, transparent 45%),
    radial-gradient(ellipse at 50% 70%, rgba(211, 211, 211, 0.25) 35%, transparent 55%);
  animation: cloud-float 8s ease-in-out infinite;
  pointer-events: none;
  z-index: -1;
}

@keyframes cloudy-drift {
  0%, 100% { filter: brightness(0.95) contrast(1.05); }
  50% { filter: brightness(1.1) contrast(0.95); }
}

@keyframes cloud-float {
  0%, 100% { transform: translateX(-5px) scale(1); opacity: 0.6; }
  50% { transform: translateX(5px) scale(1.05); opacity: 0.8; }
}

/* ⛈️ HIỆU ỨNG BÃO TIA SÉT THỰC TẾ (STORMY) - Cải thiện siêu đẹp */
.adaptive-text-line-1.weather-stormy {
  color: #8B1538;
  text-shadow: 
    0 0 40px rgba(138, 43, 226, 0.9),
    0 0 50px rgba(139, 21, 56, 0.8),
    0 6px 12px rgba(75, 0, 130, 0.7),
    0 3px 6px rgba(25, 25, 112, 0.6),
    0 1px 3px rgba(0, 0, 0, 0.8);
  animation: 
    epic-lightning-strike 0.05s linear infinite,
    agribank-base-glow 4s ease-in-out infinite,
    storm-atmosphere 3s ease-in-out infinite;
  filter: contrast(1.3) brightness(1.1);
}

.adaptive-text-line-1.weather-stormy::before {
  content: '';
  position: absolute;
  top: -50px;
  left: -40px;
  right: -40px;
  bottom: -30px;
  background-image: 
    /* Tia sét chính - hiệu ứng zigzag thực tế */
    linear-gradient(135deg, transparent 20%, rgba(255, 255, 255, 0.95) 21%, rgba(255, 255, 255, 0.95) 22%, transparent 23%),
    linear-gradient(125deg, transparent 30%, rgba(173, 216, 230, 0.8) 31%, rgba(173, 216, 230, 0.8) 32%, transparent 33%),
    linear-gradient(145deg, transparent 15%, rgba(255, 255, 255, 0.7) 16%, rgba(255, 255, 255, 0.7) 17%, transparent 18%),
    /* Tia sét phụ */
    linear-gradient(40deg, transparent 35%, rgba(255, 255, 255, 0.6) 36%, rgba(255, 255, 255, 0.6) 37%, transparent 38%),
    linear-gradient(155deg, transparent 25%, rgba(173, 216, 230, 0.5) 26%, rgba(173, 216, 230, 0.5) 27%, transparent 28%),
    /* Mưa trong bão */
    repeating-linear-gradient(
      75deg,
      transparent,
      transparent 1px,
      rgba(100, 149, 237, 0.4) 1px,
      rgba(100, 149, 237, 0.4) 2px,
      transparent 2px,
      transparent 8px
    ),
    /* Mây đen bão */
    radial-gradient(ellipse at 30% 20%, rgba(25, 25, 112, 0.4) 30%, transparent 60%),
    radial-gradient(ellipse at 70% 30%, rgba(75, 0, 130, 0.3) 25%, transparent 55%),
    radial-gradient(ellipse at 50% 60%, rgba(47, 79, 79, 0.5) 35%, transparent 65%);
  background-size: 
    120% 120%, 100% 100%, 110% 110%,
    90% 90%, 95% 95%, 8px 12px,
    200px 150px, 180px 120px, 220px 180px;
  animation: 
    realistic-lightning-bolt 4s ease-in-out infinite,
    storm-rain-fall 0.4s linear infinite;
  pointer-events: none;
  z-index: -1;
  opacity: 0.9;
}

.adaptive-text-line-1.weather-stormy::after {
  content: '';
  position: absolute;
  top: -30px;
  left: -25px;
  right: -25px;
  bottom: -15px;
  background: 
    /* Hiệu ứng ánh sáng tia sét */
    radial-gradient(circle at 40% 30%, rgba(255, 255, 255, 0.3) 5%, transparent 20%),
    radial-gradient(circle at 60% 50%, rgba(173, 216, 230, 0.2) 8%, transparent 25%);
  animation: lightning-glow 4s ease-in-out infinite;
  pointer-events: none;
  z-index: -2;
}

@keyframes epic-lightning-strike {
  0%, 85%, 87%, 89%, 91%, 93%, 95%, 100% { 
    filter: contrast(1.3) brightness(1.1); 
  }
  86%, 88%, 90%, 92%, 94% { 
    filter: contrast(2.5) brightness(3) saturate(1.5) hue-rotate(10deg); 
    text-shadow: 
      0 0 60px rgba(255, 255, 255, 1),
      0 0 80px rgba(173, 216, 230, 0.9),
      0 0 40px rgba(138, 43, 226, 0.8),
      0 6px 12px rgba(75, 0, 130, 0.7);
  }
}

@keyframes storm-atmosphere {
  0%, 100% { 
    filter: contrast(1.3) brightness(1.1) blur(0px); 
  }
  30% { 
    filter: contrast(1.1) brightness(0.8) blur(0.3px); 
  }
  60% { 
    filter: contrast(1.4) brightness(1.2) blur(0px); 
  }
}

@keyframes realistic-lightning-bolt {
  0%, 80%, 100% { 
    opacity: 0.9;
    transform: scale(1) rotate(0deg);
  }
  84%, 86%, 88%, 90%, 92% { 
    opacity: 1;
    transform: scale(1.05) rotate(1deg);
  }
  85%, 87%, 89%, 91% { 
    opacity: 0.7;
    transform: scale(0.98) rotate(-1deg);
  }
}

@keyframes lightning-glow {
  0%, 80%, 100% { 
    opacity: 0.1; 
    transform: scale(1);
  }
  84%, 86%, 88%, 90%, 92% { 
    opacity: 0.6; 
    transform: scale(1.3);
  }
  85%, 87%, 89%, 91% { 
    opacity: 0.3; 
    transform: scale(1.1);
  }
}

@keyframes storm-rain-fall {
  0% { 
    transform: translateY(-20px) translateX(8px); 
    opacity: 0.8; 
  }
  100% { 
    transform: translateY(30px) translateX(-12px); 
    opacity: 0.2; 
  }
}

/* 🌙 HIỆU ỨNG ĐÊM TRONG (CLEAR NIGHT) */
.adaptive-text-line-1.weather-clear-night {
  color: #8B1538;
  text-shadow: 
    0 0 25px rgba(25, 25, 112, 0.7),
    0 0 35px rgba(139, 21, 56, 0.6),
    0 4px 8px rgba(72, 61, 139, 0.4),
    0 2px 4px rgba(0, 0, 0, 0.4);
  animation: 
    night-twinkle 2s ease-in-out infinite,
    agribank-base-glow 4s ease-in-out infinite;
}

.adaptive-text-line-1.weather-clear-night::before {
  content: '';
  position: absolute;
  top: -20px;
  left: -20px;
  right: -20px;
  bottom: -20px;
  background: 
    radial-gradient(circle at 15% 25%, rgba(255, 255, 255, 0.6) 1px, transparent 1px),
    radial-gradient(circle at 85% 35%, rgba(255, 255, 255, 0.4) 0.5px, transparent 0.5px),
    radial-gradient(circle at 70% 70%, rgba(255, 255, 255, 0.5) 0.8px, transparent 0.8px),
    radial-gradient(circle at 30% 80%, rgba(255, 255, 255, 0.3) 0.6px, transparent 0.6px);
  animation: stars-twinkle 3s ease-in-out infinite;
  pointer-events: none;
  z-index: -1;
}

@keyframes night-twinkle {
  0%, 100% { filter: brightness(0.9); }
  50% { filter: brightness(1.2); }
}

@keyframes stars-twinkle {
  0%, 100% { opacity: 0.6; transform: scale(1); }
  25% { opacity: 1; transform: scale(1.1); }
  75% { opacity: 0.8; transform: scale(0.9); }
}

/* ❄️ HIỆU ỨNG SƯƠNG GIÁ/TUYẾT (SNOWY) - Mùa đông Lai Châu */
.adaptive-text-line-1.weather-snowy {
  color: #8B1538;
  text-shadow: 
    0 0 30px rgba(200, 220, 255, 0.8),
    0 0 40px rgba(139, 21, 56, 0.6),
    0 4px 8px rgba(173, 216, 230, 0.5),
    0 2px 4px rgba(0, 0, 0, 0.3);
  animation: 
    snowy-shimmer 2.5s ease-in-out infinite,
    agribank-base-glow 4s ease-in-out infinite;
}

.adaptive-text-line-1.weather-snowy::before {
  content: '';
  position: absolute;
  top: -30px;
  left: -25px;
  right: -25px;
  bottom: -30px;
  background: 
    radial-gradient(circle at 20% 20%, rgba(255, 255, 255, 0.6) 1px, transparent 1px),
    radial-gradient(circle at 60% 30%, rgba(255, 255, 255, 0.4) 0.8px, transparent 0.8px),
    radial-gradient(circle at 80% 60%, rgba(255, 255, 255, 0.5) 1.2px, transparent 1.2px),
    radial-gradient(circle at 30% 70%, rgba(255, 255, 255, 0.3) 0.6px, transparent 0.6px),
    radial-gradient(circle at 70% 80%, rgba(255, 255, 255, 0.4) 0.9px, transparent 0.9px);
  animation: snowfall 4s linear infinite;
  pointer-events: none;
  z-index: -1;
}

@keyframes snowy-shimmer {
  0%, 100% { filter: brightness(1) blur(0px); }
  25% { filter: brightness(1.2) blur(0.3px); }
  75% { filter: brightness(1.1) blur(0.1px); }
}

@keyframes snowfall {
  0% { transform: translateY(-30px) rotate(0deg); opacity: 0.7; }
  25% { opacity: 1; }
  50% { transform: translateY(0px) rotate(90deg); }
  75% { opacity: 0.8; }
  100% { transform: translateY(30px) rotate(180deg); opacity: 0.3; }
}

/* Base animation cho tất cả thời tiết */
@keyframes agribank-base-glow {
  0%, 100% {
    transform: scale(var(--text-scale, 1)) translateY(0px);
  }
  50% {
    transform: scale(var(--text-scale, 1)) translateY(-1px);
  }
}

/* Xóa hiệu ứng shimmer không cần thiết */

/* Dòng 2: HỆ THỐNG QUẢN LÝ KHOÁN - MÀU CỐ ĐỊNH ĐỎ BORDEAUX */
.adaptive-text-line-2 {
  font-size: clamp(1rem, 4vw, 2.8rem);
  letter-spacing: clamp(0.01em, 0.3vw, 0.05em);
  transform: scale(1);
  will-change: transform;
  font-family: 'Roboto Condensed', 'Arial', sans-serif;
  font-weight: 500;
  /* Màu cố định đỏ bordeaux */
  color: #8B1538;
  /* Hiệu ứng đổ bóng nhẹ nhàng */
  text-shadow: 
    0 0 15px rgba(139, 21, 56, 0.4),
    0 2px 6px rgba(139, 21, 56, 0.3),
    0 1px 3px rgba(0, 0, 0, 0.2);
  opacity: 0.9;
  word-spacing: clamp(-0.03em, 0vw, 0.01em);
}

/* Keyframes cho dòng phụ - xóa không cần thiết */

/* Media queries cho font size */
@media (max-width: 1200px) {
  .adaptive-text-line-1 {
    font-size: clamp(1.6rem, 7vw, 5.5rem);
  }
  .adaptive-text-line-2 {
    font-size: clamp(1.1rem, 4.5vw, 3rem);
  }
}

@media (max-width: 992px) {
  .adaptive-text-line-1 {
    font-size: clamp(1.4rem, 6vw, 4.5rem);
  }
  .adaptive-text-line-2 {
    font-size: clamp(1rem, 4vw, 2.5rem);
  }
}

@media (max-width: 768px) {
  .adaptive-text-line-1 {
    font-size: clamp(1.2rem, 5vw, 3.5rem);
  }
  .adaptive-text-line-2 {
    font-size: clamp(0.9rem, 3.5vw, 2rem);
  }
}

@media (max-width: 576px) {
  .adaptive-text-line-1 {
    font-size: clamp(1rem, 4.5vw, 3rem);
  }
  .adaptive-text-line-2 {
    font-size: clamp(0.8rem, 3vw, 1.8rem);
  }
}

@media (max-width: 480px) {
  .adaptive-text-line-1 {
    font-size: clamp(0.9rem, 4vw, 2.5rem);
  }
  .adaptive-text-line-2 {
    font-size: clamp(0.7rem, 2.8vw, 1.5rem);
  }
}

@media (max-width: 320px) {
  .adaptive-text-line-1 {
    font-size: clamp(0.8rem, 3.5vw, 2rem);
  }
  .adaptive-text-line-2 {
    font-size: clamp(0.6rem, 2.5vw, 1.2rem);
  }
}

/* 🎯 RESPONSIVE IMPROVEMENTS - Đảm bảo không bao giờ bị mất chữ */

/* Ultra small screens và landscape phones */
@media (max-width: 280px) {
  .adaptive-text-line-1 {
    font-size: clamp(0.7rem, 3vw, 1.8rem);
    letter-spacing: 0.01em;
  }
  .adaptive-text-line-2 {
    font-size: clamp(0.5rem, 2.2vw, 1rem);
  }
  .text-container {
    padding: 8px 2px;
    min-height: 50px;
  }
}

/* Landscape mode improvements */
@media (max-height: 500px) and (orientation: landscape) {
  .welcome-hero {
    min-height: 90vh;
    padding-top: 1vh;
  }
  .hero-content {
    padding: 0 10px;
  }
  .text-container {
    padding: 5px 3px;
    min-height: 40px;
  }
  .adaptive-text-line-1 {
    font-size: clamp(1rem, 5vw, 3rem);
  }
  .adaptive-text-line-2 {
    font-size: clamp(0.8rem, 3vw, 1.8rem);
  }
}

/* High DPI screens */
@media (-webkit-min-device-pixel-ratio: 2), (min-resolution: 192dpi) {
  .adaptive-text-line-1 {
    text-rendering: optimizeLegibility;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
  }
}

/* Very wide screens */
@media (min-width: 1920px) {
  .adaptive-text-line-1 {
    font-size: clamp(3rem, 6vw, 8rem);
    letter-spacing: 0.12em;
  }
  .adaptive-text-line-2 {
    font-size: clamp(1.5rem, 3vw, 4rem);
  }
}

.hero-title {
  font-size: clamp(2.5rem, 8vw, 6.5rem);
  font-weight: 800;
  font-family: 'Inter', 'Segoe UI', 'Roboto', 'Arial', sans-serif;
  margin-bottom: 25px;
  line-height: 1.1;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  width: 100%;
  max-width: 100%;
  overflow: visible;
  text-align: center;
  /* Loại bỏ màu cũ để gradient trong .adaptive-text-line-1 hoạt động */
}

.hero-logo {
  height: 90px;
  width: auto;
  margin-bottom: 20px;
  filter: drop-shadow(0 8px 16px rgba(139, 21, 56, 0.5));
  animation: gentle-sway 4s ease-in-out infinite;
  transform: scale(1.1);
}

@keyframes gentle-sway {
  0%, 100% { transform: rotate(-5deg); }
  50% { transform: rotate(5deg); }
}

.hero-subtitle {
  font-size: clamp(1.2rem, 4vw, 2.6rem);
  font-family: 'Inter', 'Segoe UI', 'Roboto', 'Arial', sans-serif;
  margin-bottom: 0;
  margin-top: 5px;
  font-weight: 700;
  font-style: normal;
  letter-spacing: 0.05em;
  line-height: 1.3;
  width: 100%;
  max-width: 100%;
  overflow: visible;
  text-align: center;
  /* Loại bỏ màu cứng để gradient trong .adaptive-text-line-2 hoạt động */
}

/* 🎯 SIÊU RESPONSIVE: Ultra Small Screens (iPhone SE, nhỏ hơn 375px) */
@media (max-width: 374px) {
  .welcome-hero {
    min-height: 95vh;
    padding-top: 0.5vh;
  }
  
  .hero-content {
    padding: 0 5px;
  }
  
  .text-container {
    padding: 4px 1px;
    min-height: 35px;
    width: 100%;
    max-width: 100vw;
  }
  
  .adaptive-text-line-1 {
    font-size: clamp(0.9rem, 6vw, 2.5rem) !important;
    letter-spacing: clamp(0.01em, 0.3vw, 0.05em);
    word-spacing: -0.05em;
  }
  
  .adaptive-text-line-2 {
    font-size: clamp(0.7rem, 4vw, 1.5rem) !important;
    letter-spacing: clamp(0.01em, 0.2vw, 0.03em);
  }
  
  .hero-logo {
    height: 60px;
    margin-bottom: 15px;
  }
}

/* 🎯 SIÊU RESPONSIVE: Ultra Small Height (Mobile Landscape, chiều cao < 450px) */
@media (max-height: 450px) and (orientation: landscape) {
  .welcome-hero {
    min-height: 85vh;
    padding-top: 0.5vh;
    display: flex;
    align-items: flex-start;
    justify-content: center;
  }
  
  .hero-content {
    padding: 0 8px;
    margin-top: 0;
  }
  
  .text-container {
    padding: 3px 2px;
    min-height: 30px;
  }
  
  .adaptive-text-line-1 {
    font-size: clamp(0.8rem, 4vh, 2rem) !important;
    letter-spacing: clamp(0.01em, 0.2vh, 0.04em);
  }
  
  .adaptive-text-line-2 {
    font-size: clamp(0.6rem, 2.5vh, 1.2rem) !important;
    letter-spacing: clamp(0.01em, 0.1vh, 0.02em);
  }
  
  .hero-logo {
    height: 45px;
    margin-bottom: 10px;
  }
  
  .hero-title {
    margin-bottom: 15px;
  }
}

/* 🎯 SIÊU RESPONSIVE: Extreme Small Screens (width < 320px) */
@media (max-width: 319px) {
  .text-container {
    padding: 2px 0px;
    min-height: 25px;
    overflow: visible;
    width: 100%;
  }
  
  .hero-text {
    transform-origin: center center;
    word-spacing: -0.1em;
  }
  
  .adaptive-text-line-1 {
    font-size: clamp(0.7rem, 8vw, 2rem) !important;
    letter-spacing: clamp(0em, 0.2vw, 0.03em);
  }
  
  .adaptive-text-line-2 {
    font-size: clamp(0.5rem, 5vw, 1.2rem) !important;
    letter-spacing: clamp(0em, 0.1vw, 0.02em);
  }
  
  .hero-logo {
    height: 50px;
    margin-bottom: 8px;
  }
}

/* 🎯 SIÊU RESPONSIVE: Touch device optimizations */
@media (hover: none) and (pointer: coarse) {
  .hero-text {
    /* Tối ưu cho touch device */
    -webkit-tap-highlight-color: transparent;
    -webkit-touch-callout: none;
    -webkit-user-select: none;
    user-select: none;
  }
  
  .text-container {
    /* Đảm bảo touch không ảnh hưởng layout */
    touch-action: manipulation;
  }
}

/* 🎯 SIÊU RESPONSIVE: Fold devices và unusual aspect ratios */
@media (min-aspect-ratio: 3/1) {
  .adaptive-text-line-1 {
    font-size: clamp(1rem, 3vh, 2.5rem) !important;
  }
  
  .adaptive-text-line-2 {
    font-size: clamp(0.8rem, 2vh, 1.5rem) !important;
  }
}

@media (max-aspect-ratio: 3/4) {
  .text-container {
    padding: 6px 3px;
  }
}
</style>
