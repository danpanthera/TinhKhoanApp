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

// Hàm check xem text có bị overflow không - đơn giản hóa
const checkTextOverflow = (element) => {
  if (!element) return false;
  
  const parent = element.parentElement;
  if (!parent) return false;
  
  // Chỉ dùng method đơn giản nhất
  const elementWidth = element.getBoundingClientRect().width;
  const parentWidth = parent.getBoundingClientRect().width;
  const safetyMargin = 5; // Chỉ 5px margin đơn giản
  
  const isOverflow = elementWidth > (parentWidth - safetyMargin);
  
  if (isOverflow) {
    console.log(`⚠️ Simple overflow check:`, {
      element: element.textContent.substring(0, 20) + '...',
      elementWidth: elementWidth.toFixed(1),
      parentWidth: parentWidth.toFixed(1),
      overflow: isOverflow
    });
  }
  
  return isOverflow;
};

// Hàm tính toán scale cực kỳ đơn giản - chỉ tính toán matemática cơ bản
const calculateSimpleScale = (element) => {
  if (!element) return 1;
  
  const container = element.parentElement;
  if (!container) return 1;
  
  // Reset về scale 1 để đo kích thước gốc chính xác
  element.style.transform = 'scaleX(1)';
  
  // Đợi một chút để DOM update
  const containerWidth = container.getBoundingClientRect().width - 20; // 20px padding total
  const textWidth = element.getBoundingClientRect().width;
  
  console.log(`📏 Simple measure "${element.textContent.substring(0, 15)}...":`, {
    containerWidth: containerWidth.toFixed(1),
    textWidth: textWidth.toFixed(1),
    ratio: (containerWidth / textWidth).toFixed(3)
  });
  
  // Nếu text vừa hoặc nhỏ hơn container thì không cần scale
  if (textWidth <= containerWidth) {
    console.log('✅ Text vừa khít, không cần co giãn');
    return 1;
  }
  
  // Tính scale đơn giản: tỷ lệ container/text
  const simpleScale = containerWidth / textWidth;
  
  // Đảm bảo scale tối thiểu 0.5 để text vẫn đọc được rõ ràng
  const finalScale = Math.max(0.5, simpleScale);
  
  console.log(`🎯 Simple scale: ${simpleScale.toFixed(3)} → final: ${finalScale.toFixed(3)}`);
  return finalScale;
};

// Hàm auto-adjust cực kỳ đơn giản - không kiểm tra overflow, chỉ tính toán scale
const autoAdjustTextSize = () => {
  if (!adaptiveTextLine1.value || !adaptiveTextLine2.value) {
    console.warn('⚠️ Text elements chưa ready');
    return;
  }
  
  try {
    console.log('🔄 Bắt đầu ultra-simple auto-adjust...');
    
    // Thêm delay nhỏ để đảm bảo DOM đã render xong
    setTimeout(() => {
      // Dòng 1: "AGRIBANK LAI CHAU CENTER" - scaling hoàn toàn độc lập
      const scale1 = calculateSimpleScale(adaptiveTextLine1.value);
      scaleFactorLine1.value = scale1;
      adaptiveTextLine1.value.style.transform = `scaleX(${scale1})`;
      
      console.log(`📝 Dòng 1 scaled: ${scale1.toFixed(3)}`);
    }, 10);
    
    // Delay thêm cho dòng 2 để tránh conflict
    setTimeout(() => {
      // Dòng 2: "HỆ THỐNG QUẢN LÝ KHOÁN | HỆ THỐNG BÁO CÁO" - scaling hoàn toàn độc lập
      const scale2 = calculateSimpleScale(adaptiveTextLine2.value);
      scaleFactorLine2.value = scale2;
      adaptiveTextLine2.value.style.transform = `scaleX(${scale2})`;
      
      console.log(`📝 Dòng 2 scaled: ${scale2.toFixed(3)}`);
      
      console.log('✅ Ultra-simple auto-adjust hoàn thành:', {
        line1Scale: scale1.toFixed(3),
        line2Scale: scale2.toFixed(3),
        mode: 'Ultra-simple - chỉ tính toán matematica cơ bản'
      });
    }, 20);
    
  } catch (error) {
    console.error('❌ Lỗi ultra-simple auto-adjust:', error);
    // Fallback an toàn
    scaleFactorLine1.value = 0.8;
    scaleFactorLine2.value = 0.8;
    adaptiveTextLine1.value.style.transform = `scaleX(0.8)`;
    adaptiveTextLine2.value.style.transform = `scaleX(0.8)`;
  }
};

// Debounce đơn giản
const debounce = (func, wait) => {
  let timeout;
  return (...args) => {
    clearTimeout(timeout);
    timeout = setTimeout(() => func(...args), wait);
  };
};

// Debounced version với delay dài hơn để ổn định
const debouncedAutoAdjust = debounce(autoAdjustTextSize, 300);

// Monitoring đơn giản
let resizeObserver = null;

onMounted(() => {
  if (!isAuthenticated()) {
    router.push('/login');
    return;
  }
  
  // Khởi tạo với delays khác nhau để đảm bảo hoạt động
  setTimeout(autoAdjustTextSize, 100);
  setTimeout(autoAdjustTextSize, 500);
  setTimeout(autoAdjustTextSize, 1000);
  
  // Chỉ lắng nghe resize window
  window.addEventListener('resize', debouncedAutoAdjust);
  
  // ResizeObserver đơn giản
  if (window.ResizeObserver && adaptiveTextLine1.value?.parentElement) {
    resizeObserver = new ResizeObserver(debouncedAutoAdjust);
    resizeObserver.observe(adaptiveTextLine1.value.parentElement);
  }
  
  console.log('🎨 Ultra-simple adaptive text system initialized');
});

onUnmounted(() => {
  // Cleanup đơn giản
  window.removeEventListener('resize', debouncedAutoAdjust);
  
  if (resizeObserver) {
    resizeObserver.disconnect();
  }
  
  console.log('🧹 Ultra-simple adaptive text system cleaned up');
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
  padding: 0 20px; /* Tăng padding để đảm bảo text không sát biên */
  box-sizing: border-box; /* Đảm bảo padding không làm tăng width */
  overflow: hidden; /* Chặn overflow ở level container */
  max-width: 100vw; /* Không vượt quá viewport width */
  position: relative;
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
  transition: transform 0.3s cubic-bezier(0.25, 0.46, 0.45, 0.94); /* Smooth transition với easing */
  will-change: transform; /* Tối ưu hiệu năng animation */
  text-overflow: clip; /* Không hiển thị ... khi overflow */
  position: relative;
  /* Backup constraints */
  min-width: 0;
  flex-shrink: 1;
}

/* Dòng 1: AGRIBANK LAI CHAU CENTER */
.adaptive-text-line-1 {
  font-size: clamp(1.8rem, 8vw, 6.5rem); /* Auto scaling font size */
  letter-spacing: clamp(0.02em, 0.5vw, 0.08em); /* Auto scaling letter spacing */
  transform: scaleX(1); /* Mặc định không co giãn theo chiều ngang */
  will-change: transform; /* Tối ưu hiệu năng animation */
  /* Enhanced constraints */
  max-width: calc(100vw - 60px); /* Đảm bảo không vượt quá viewport - padding */
  word-spacing: clamp(-0.05em, 0vw, 0.02em); /* Tinh chỉnh word spacing */
}

/* Dòng 2: HỆ THỐNG QUẢN LÝ KHOÁN | HỆ THỐNG BÁO CÁO */
.adaptive-text-line-2 {
  font-size: clamp(1.2rem, 5vw, 3.5rem); /* Nhỏ hơn dòng 1 một chút */
  letter-spacing: clamp(0.01em, 0.3vw, 0.05em); /* Auto scaling letter spacing */
  transform: scaleX(1); /* Mặc định không co giãn theo chiều ngang */
  will-change: transform; /* Tối ưu hiệu năng animation */
  /* Enhanced constraints */
  max-width: calc(100vw - 60px); /* Đảm bảo không vượt quá viewport - padding */
  word-spacing: clamp(-0.03em, 0vw, 0.01em); /* Tinh chỉnh word spacing */
}

/* Media queries cho responsive co giãn đơn giản - chỉ fallback */
@media (max-width: 1200px) {
  .adaptive-text-line-1 {
    font-size: clamp(1.6rem, 7vw, 5.5rem); /* Giảm size một chút */
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
