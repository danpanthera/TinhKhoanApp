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

// Hàm tính toán và áp dụng scale cho từng dòng text
const adjustTextSize = (element) => {
  if (!element) return;
  
  // Lấy container của element
  const container = element.closest('.text-container');
  if (!container) return;
  
  // Reset scale để đo kích thước gốc
  element.style.transform = 'scale(1)';
  
  // Lấy kích thước
  const containerWidth = container.offsetWidth;
  const textWidth = element.offsetWidth;
  
  // Nếu text vừa hoặc nhỏ hơn container thì không cần scale
  if (textWidth <= containerWidth) {
    element.style.transform = 'scale(1)';
    console.log(`✅ Text vừa khít: ${element.textContent.substring(0, 15)}...`);
    return;
  }
  
  // Tính scale tỷ lệ
  let scale = containerWidth / textWidth;
  
  // Giới hạn scale tối thiểu là 0.4 để text vẫn đọc được
  scale = Math.max(0.4, scale);
  
  // Áp dụng scale
  element.style.transform = `scale(${scale})`;
  
  console.log(`� Scaled text: "${element.textContent.substring(0, 15)}..." - scale: ${scale.toFixed(3)}`);
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

// Debounced version cho performance
const debouncedAdjust = debounce(autoAdjustAllText, 300);

// Observer để theo dõi thay đổi kích thước
let resizeObserver = null;

onMounted(() => {
  if (!isAuthenticated()) {
    router.push('/login');
    return;
  }
  
  // Khởi tạo lần đầu với delay
  setTimeout(autoAdjustAllText, 100);
  setTimeout(autoAdjustAllText, 500);
  
  // Theo dõi sự kiện resize window
  window.addEventListener('resize', debouncedAdjust);
  
  // Dùng ResizeObserver để theo dõi thay đổi kích thước container
  if (window.ResizeObserver) {
    resizeObserver = new ResizeObserver(debouncedAdjust);
    
    // Theo dõi cả hai container
    const containers = document.querySelectorAll('.text-container');
    containers.forEach(container => {
      resizeObserver.observe(container);
    });
  }
  
  console.log('🎨 Simple adaptive text system initialized');
});

onUnmounted(() => {
  // Cleanup
  window.removeEventListener('resize', debouncedAdjust);
  
  if (resizeObserver) {
    resizeObserver.disconnect();
  }
  
  console.log('🧹 Adaptive text system cleaned up');
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

/* Container cho từng dòng text */
.text-container {
  width: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  overflow: hidden;
  margin: 0 auto;
  height: auto;
  position: relative;
}

/* CSS chung cho hero text */
.hero-text {
  display: inline-block;
  white-space: nowrap;
  text-transform: uppercase;
  transform-origin: center;
  transition: transform 0.3s cubic-bezier(0.25, 0.46, 0.45, 0.94);
  will-change: transform;
  text-overflow: clip;
}

/* Dòng 1: AGRIBANK LAI CHAU CENTER - HIỆU ỨNG CHUYỂN MÀU ĐUỆT ĐỜI */
.adaptive-text-line-1 {
  font-size: clamp(1.8rem, 8vw, 6.5rem);
  letter-spacing: clamp(0.02em, 0.5vw, 0.08em);
  transform: scale(1);
  font-family: 'Montserrat', 'Roboto', sans-serif;
  font-weight: 800;
  /* Gradient chuyển màu từ đỏ bordeaux sang trắng ngọc trai */
  background: linear-gradient(45deg, 
    #8B1538 0%,     /* Đỏ bordeaux đậm */
    #C41E3A 25%,    /* Đỏ bordeaux sáng */
    #f5f5f1 50%,    /* Trắng ngọc trai */
    #C41E3A 75%,    /* Đỏ bordeaux sáng */
    #8B1538 100%    /* Đỏ bordeaux đậm */
  );
  background-size: 400% 400%;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  /* Animation chuyển màu mượt mà */
  animation: agribank-gradient-flow 4s ease-in-out infinite;
  /* Hiệu ứng đổ bóng để tạo độ sâu */
  filter: drop-shadow(0 4px 8px rgba(139, 21, 56, 0.3))
          drop-shadow(0 2px 4px rgba(0, 0, 0, 0.2));
  will-change: transform, background-position;
  word-spacing: clamp(-0.05em, 0vw, 0.02em);
  /* Thêm hiệu ứng shimmer nhẹ */
  position: relative;
}

/* Keyframes cho hiệu ứng chuyển màu gradient */
@keyframes agribank-gradient-flow {
  0% {
    background-position: 0% 50%;
  }
  50% {
    background-position: 100% 50%;
  }
  100% {
    background-position: 0% 50%;
  }
}

/* Hiệu ứng shimmer overlay cho dòng chữ chính */
.adaptive-text-line-1::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, 
    transparent 0%, 
    rgba(245, 245, 241, 0.6) 50%, 
    transparent 100%
  );
  animation: shimmer 3s ease-in-out infinite;
  pointer-events: none;
}

/* Keyframes cho hiệu ứng shimmer */
@keyframes shimmer {
  0% {
    left: -100%;
  }
  50% {
    left: 100%;
  }
  100% {
    left: 100%;
  }
}

/* Dòng 2: HỆ THỐNG QUẢN LÝ KHOÁN - HIỆU ỨNG BỔ TRỢ */
.adaptive-text-line-2 {
  font-size: clamp(1rem, 4vw, 2.8rem);
  letter-spacing: clamp(0.01em, 0.3vw, 0.05em);
  transform: scale(1);
  will-change: transform;
  font-family: 'Roboto Condensed', 'Arial', sans-serif;
  font-weight: 500;
  /* Gradient nhẹ nhàng hơn cho dòng phụ */
  background: linear-gradient(45deg, 
    #8B1538 0%,     /* Đỏ bordeaux */
    #A91B47 50%,    /* Đỏ bordeaux nhạt */
    #8B1538 100%    /* Đỏ bordeaux */
  );
  background-size: 200% 200%;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  animation: subtitle-gradient 5s ease-in-out infinite;
  opacity: 0.9;
  word-spacing: clamp(-0.03em, 0vw, 0.01em);
}

/* Keyframes cho dòng phụ */
@keyframes subtitle-gradient {
  0%, 100% {
    background-position: 0% 50%;
  }
  50% {
    background-position: 100% 50%;
  }
}

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

/* Responsive Design - Enhanced */
@media (max-width: 1200px) {
  .hero-content {
    padding: 0 20px;
  }
}

@media (max-width: 768px) {
  .welcome-hero {
    min-height: 100vh;
    padding: 0 15px 20px 15px;
    padding-top: 1.5vh;
  }
  
  .hero-content {
    padding: 0 10px;
  }
  
  .hero-logo {
    height: 70px;
    transform: scale(1.05);
  }
}

@media (max-width: 480px) {
  .welcome-hero {
    min-height: 100vh;
    padding: 0 10px 20px 10px;
    padding-top: 1vh;
  }
  
  .hero-content {
    padding: 0 5px;
  }
  
  .hero-logo {
    height: 60px;
    transform: scale(1.0);
  }
}

/* Extra small screens */
@media (max-width: 320px) {
  .hero-content {
    padding: 0 2px;
  }
  
  .hero-title {
    letter-spacing: 0.02em;
  }
  
  .hero-subtitle {
    letter-spacing: 0.01em;
  }
}
</style>
