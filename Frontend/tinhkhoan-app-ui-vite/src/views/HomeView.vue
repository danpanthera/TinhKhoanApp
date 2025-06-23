<template>
  <div class="home">
    <div class="welcome-hero">
      <div class="hero-content">
        <h1 class="hero-title">
          <img src="/src/assets/Logo-Agribank-2.png" alt="Agribank Logo" class="hero-logo" />
          <br>
          <span 
            ref="adaptiveTextLine1" 
            class="hero-text adaptive-text-line-1"
            :style="{ transform: `scaleX(${scaleFactorLine1})` }"
          >
            AGRIBANK LAI CHAU CENTER
          </span>
        </h1>
        <p class="hero-subtitle">
          <span 
            ref="adaptiveTextLine2"
            class="hero-text adaptive-text-line-2"
            :style="{ transform: `scaleX(${scaleFactorLine2})` }"
          >
            HỆ THỐNG QUẢN LÝ KHOÁN | HỆ THỐNG BÁO CÁO
          </span>
        </p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref, onUnmounted, nextTick } from 'vue';
import { useRouter } from 'vue-router';
import { isAuthenticated } from '../services/auth';

const router = useRouter();

// Refs cho 2 dòng text
const adaptiveTextLine1 = ref(null);
const adaptiveTextLine2 = ref(null);

// Scale factors cho 2 dòng
const scaleFactorLine1 = ref(1);
const scaleFactorLine2 = ref(1);

// Hàm check xem text có bị overflow không
const checkTextOverflow = (element) => {
  if (!element) return false;
  return element.scrollWidth > element.offsetWidth;
};

// Hàm tính toán scale factor để text vừa với container
const calculateScaleFactor = (element, currentScale = 1) => {
  if (!element) return 1;
  
  // Reset về scale 1 để đo kích thước thực
  element.style.transform = 'scaleX(1)';
  
  const containerWidth = element.parentElement.offsetWidth;
  const textWidth = element.scrollWidth;
  
  // Nếu text vừa với container thì không cần scale
  if (textWidth <= containerWidth) {
    return 1;
  }
  
  // Tính scale factor để text vừa với container (với một chút margin)
  return Math.max(0.6, (containerWidth * 0.95) / textWidth);
};

// Hàm auto-adjust thông minh cho 2 dòng text
const autoAdjustTextSize = () => {
  nextTick(() => {
    try {
      // Tính scale factor cho dòng 1 (AGRIBANK LAI CHAU CENTER) - dòng chủ đạo
      const scaleLine1 = calculateScaleFactor(adaptiveTextLine1.value);
      scaleFactorLine1.value = scaleLine1;
      
      // Dòng 2 theo tỷ lệ của dòng 1 (proportional scaling)
      const scaleLine2 = calculateScaleFactor(adaptiveTextLine2.value);
      
      // Nếu dòng 1 bị thu nhỏ, dòng 2 cũng thu theo tỷ lệ
      if (scaleLine1 < 1) {
        scaleFactorLine2.value = Math.min(scaleLine1, scaleLine2);
      } else {
        scaleFactorLine2.value = scaleLine2;
      }
      
      console.log('📏 Auto-adjust text:', {
        line1Scale: scaleFactorLine1.value,
        line2Scale: scaleFactorLine2.value,
        line1Width: adaptiveTextLine1.value?.scrollWidth,
        line2Width: adaptiveTextLine2.value?.scrollWidth,
        containerWidth: adaptiveTextLine1.value?.parentElement?.offsetWidth
      });
      
    } catch (error) {
      console.warn('❌ Lỗi auto-adjust text:', error);
    }
  });
};

// Debounce function để tối ưu performance
const debounce = (func, wait) => {
  let timeout;
  return function executedFunction(...args) {
    const later = () => {
      clearTimeout(timeout);
      func(...args);
    };
    clearTimeout(timeout);
    timeout = setTimeout(later, wait);
  };
};

// Debounced resize handler
const debouncedAutoAdjust = debounce(autoAdjustTextSize, 150);

// ResizeObserver để theo dõi thay đổi kích thước container
let resizeObserver = null;

onMounted(() => {
  if (!isAuthenticated()) {
    router.push('/login');
    return;
  }
  
  // Khởi tạo auto-adjust sau khi component mounted
  setTimeout(autoAdjustTextSize, 100);
  
  // Lắng nghe resize window
  window.addEventListener('resize', debouncedAutoAdjust);
  
  // Sử dụng ResizeObserver để theo dõi container
  if (window.ResizeObserver && adaptiveTextLine1.value?.parentElement) {
    resizeObserver = new ResizeObserver(debouncedAutoAdjust);
    resizeObserver.observe(adaptiveTextLine1.value.parentElement);
  }
  
  // Lắng nghe font load events
  if (document.fonts) {
    document.fonts.ready.then(autoAdjustTextSize);
  }
  
  console.log('🎨 Adaptive text system initialized');
});

onUnmounted(() => {
  // Cleanup event listeners
  window.removeEventListener('resize', debouncedAutoAdjust);
  
  if (resizeObserver) {
    resizeObserver.disconnect();
  }
});
</script>

<style scoped>
.home {
  padding: 0;
  margin: 0;
  min-height: calc(100vh - 120px);
  background: transparent !important; /* Đảm bảo không có nền nào */
  overflow-x: hidden; /* Ngăn cuộn ngang */
  width: 100%; /* Đảm bảo chiều rộng không vượt quá viewport */
  box-sizing: border-box; /* Đảm bảo padding không làm tăng width */
}

.welcome-hero {
  text-align: center;
  padding: 0 0 20px 0; /* Bỏ padding ngang để tránh overflow */
  background: transparent;
  min-height: 100vh; /* Chiếm toàn bộ màn hình */
  display: flex;
  align-items: flex-start; /* Đẩy lên trên */
  justify-content: center; /* Căn giữa theo chiều ngang */
  padding-top: 2vh; /* Giảm từ 4vh xuống 2vh để đưa lên cao hơn khoảng 1.5cm */
  width: 100%; /* Đảm bảo chiều rộng đúng với viewport */
  overflow-x: hidden; /* Ngăn cuộn ngang */
  box-sizing: border-box; /* Đảm bảo padding không làm tăng width */
}

.hero-content {
  width: auto;
  margin: 0 auto;
  background: transparent;
  padding: 0 10px; /* Thêm padding để tránh text sát rìa */
  box-sizing: border-box; /* Đảm bảo padding không làm tăng width */
  overflow: visible; /* Cho phép nội dung hiển thị đầy đủ */
}

/* CSS cho chữ thẳng, không cong - Enhanced Responsive */
.hero-text {
  display: inline-block;
  transform: none;
  text-transform: uppercase;
  /* Smart text wrapping */
  word-break: keep-all; /* Giữ nguyên từ, không ngắt trong từ */
  overflow-wrap: anywhere; /* Chỉ ngắt khi thực sự cần thiết */
  hyphens: none; /* Không dùng dấu gạch ngang */
}

/* === ADAPTIVE TEXT SCALING - Chữ co giãn tự động không xuống dòng === */
.adaptive-text-line-1,
.adaptive-text-line-2 {
  display: inline-block;
  white-space: nowrap; /* Không cho phép xuống dòng */
  overflow: hidden; /* Ẩn nội dung bị tràn */
  max-width: 100%; /* Giới hạn trong container */
  transform-origin: center; /* Căn giữa khi scale */
  transition: all 0.3s ease; /* Smooth transition */
}

/* Dòng 1: AGRIBANK LAI CHAU CENTER */
.adaptive-text-line-1 {
  font-size: clamp(1.8rem, 8vw, 6.5rem); /* Auto scaling font size */
  letter-spacing: clamp(0.02em, 0.5vw, 0.08em); /* Auto scaling letter spacing */
  transform: scaleX(1); /* Mặc định không co giãn theo chiều ngang */
  will-change: transform; /* Tối ưu hiệu năng animation */
}

/* Dòng 2: HỆ THỐNG QUẢN LÝ KHOÁN | HỆ THỐNG BÁO CÁO */
.adaptive-text-line-2 {
  font-size: clamp(1.2rem, 5vw, 3.5rem); /* Nhỏ hơn dòng 1 một chút */
  letter-spacing: clamp(0.01em, 0.3vw, 0.05em); /* Auto scaling letter spacing */
  transform: scaleX(1); /* Mặc định không co giãn theo chiều ngang */
  will-change: transform; /* Tối ưu hiệu năng animation */
}

/* Media queries cho responsive co giãn nâng cao */
@media (max-width: 1200px) {
  .adaptive-text-line-1 {
    transform: scaleX(0.95); /* Co nhẹ 5% khi màn hình nhỏ hơn */
  }
  .adaptive-text-line-2 {
    transform: scaleX(0.96); /* Co nhẹ 4% khi màn hình nhỏ hơn */
  }
}

@media (max-width: 992px) {
  .adaptive-text-line-1 {
    transform: scaleX(0.9); /* Co 10% cho tablet */
  }
  .adaptive-text-line-2 {
    transform: scaleX(0.92); /* Co 8% cho tablet */
  }
}

@media (max-width: 768px) {
  .adaptive-text-line-1 {
    transform: scaleX(0.85); /* Co 15% cho mobile */
  }
  .adaptive-text-line-2 {
    transform: scaleX(0.88); /* Co 12% cho mobile */
  }
}

@media (max-width: 576px) {
  .adaptive-text-line-1 {
    transform: scaleX(0.8); /* Co 20% cho mobile nhỏ */
  }
  .adaptive-text-line-2 {
    transform: scaleX(0.83); /* Co 17% cho mobile nhỏ */
  }
}

@media (max-width: 480px) {
  .adaptive-text-line-1 {
    transform: scaleX(0.75); /* Co 25% cho mobile rất nhỏ */
  }
  .adaptive-text-line-2 {
    transform: scaleX(0.78); /* Co 22% cho mobile rất nhỏ */
  }
}

@media (max-width: 320px) {
  .adaptive-text-line-1 {
    transform: scaleX(0.7); /* Co tối đa 30% cho màn hình cực nhỏ */
  }
  .adaptive-text-line-2 {
    transform: scaleX(0.73); /* Co tối đa 27% cho màn hình cực nhỏ */
  }
}

.hero-title {
  font-size: clamp(2.5rem, 8vw, 6.5rem); /* Responsive font-size tự động co giãn */
  font-weight: 800;
  font-family: 'Inter', 'Segoe UI', 'Roboto', 'Arial', sans-serif;
  color: #8B1538 !important;
  margin-bottom: 25px;
  text-shadow: 
    0 8px 16px rgba(139, 21, 56, 0.4),
    0 4px 8px rgba(0, 0, 0, 0.3),
    0 2px 4px rgba(139, 21, 56, 0.6);
  line-height: 1.1;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  background: none;
  -webkit-background-clip: unset;
  -webkit-text-fill-color: #8B1538;
  background-clip: unset;
  transform: none;
  filter: drop-shadow(0 10px 20px rgba(139, 21, 56, 0.3));
  /* Responsive text wrapping */
  white-space: normal; /* Cho phép xuống dòng */
  word-wrap: break-word; /* Tự động ngắt từ nếu quá dài */
  overflow-wrap: break-word; /* Hỗ trợ break word tốt hơn */
  hyphens: auto; /* Tự động thêm dấu gạch ngang khi ngắt từ */
  width: 100%;
  max-width: 100%;
  overflow: visible;
  text-align: center; /* Căn giữa text */
}

.nature-icon {
  font-size: 4rem;
  vertical-align: middle;
  margin-right: 16px;
  animation: gentle-sway 3s ease-in-out infinite;
}

.hero-logo {
  height: 90px; /* Tăng kích thước logo để phù hợp */
  width: auto;
  margin-bottom: 20px; /* Giảm khoảng cách để gọn hơn */
  filter: drop-shadow(0 8px 16px rgba(139, 21, 56, 0.5)); /* Bóng đậm hơn màu Agribank */
  animation: gentle-sway 4s ease-in-out infinite; /* Chậm hơn, mềm mại hơn */
  transform: scale(1.1); /* Phóng to nhẹ để nổi bật */
}

@keyframes gentle-sway {
  0%, 100% { transform: rotate(-5deg); }
  50% { transform: rotate(5deg); }
}

/* Hiệu ứng lung linh cho chữ AGRIBANK LAI CHAU CENTER */
@keyframes text-shimmer {
  0% {
    background-position: -200% center;
    filter: brightness(1) drop-shadow(0 0 20px rgba(255, 223, 0, 0.3));
  }
  50% {
    background-position: 200% center;
    filter: brightness(1.2) drop-shadow(0 0 30px rgba(255, 223, 0, 0.6));
  }
  100% {
    background-position: -200% center;
    filter: brightness(1) drop-shadow(0 0 20px rgba(255, 223, 0, 0.3));
  }
}

.hero-subtitle {
  font-size: clamp(1.2rem, 4vw, 2.6rem); /* Responsive font-size tự động co giãn */
  font-family: 'Inter', 'Segoe UI', 'Roboto', 'Arial', sans-serif;
  color: #8B1538 !important;
  margin-bottom: 0;
  margin-top: 5px;
  font-weight: 700;
  font-style: normal;
  text-shadow: 
    0 6px 12px rgba(139, 21, 56, 0.35),
    0 3px 6px rgba(0, 0, 0, 0.25),
    0 1px 3px rgba(139, 21, 56, 0.5);
  letter-spacing: 0.05em;
  line-height: 1.3;
  background: none;
  -webkit-background-clip: unset;
  -webkit-text-fill-color: #8B1538;
  background-clip: unset;
  transform: none;
  filter: drop-shadow(0 6px 12px rgba(139, 21, 56, 0.25));
  /* Responsive text wrapping */
  white-space: normal; /* Cho phép xuống dòng */
  word-wrap: break-word; /* Tự động ngắt từ nếu quá dài */
  overflow-wrap: break-word; /* Hỗ trợ break word tốt hơn */
  hyphens: auto; /* Tự động thêm dấu gạch ngang khi ngắt từ */
  width: 100%;
  max-width: 100%;
  overflow: visible;
  text-align: center; /* Căn giữa text */
}

/* Responsive Design - Enhanced */
@media (max-width: 1200px) {
  .hero-content {
    padding: 0 20px; /* Tăng padding cho màn hình lớn */
  }
}

@media (max-width: 768px) {
  .welcome-hero {
    min-height: 100vh;
    padding: 0 15px 20px 15px;
    padding-top: 1.5vh;
  }
  
  .hero-content {
    padding: 0 10px; /* Giảm padding cho tablet */
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
    padding: 0 5px; /* Padding nhỏ nhất cho mobile */
  }
  
  .hero-logo {
    height: 60px;
    transform: scale(1.0);
  }
}

/* Extra small screens - điện thoại nhỏ */
@media (max-width: 320px) {
  .hero-content {
    padding: 0 2px; /* Padding tối thiểu */
  }
  
  .hero-title {
    letter-spacing: 0.02em; /* Giảm letter-spacing để tiết kiệm không gian */
  }
  
  .hero-subtitle {
    letter-spacing: 0.01em; /* Giảm letter-spacing cho subtitle */
  }
}
</style>
