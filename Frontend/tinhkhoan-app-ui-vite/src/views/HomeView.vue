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

// Hàm check xem text có bị overflow không với margin safety và multiple methods
const checkTextOverflow = (element) => {
  if (!element) return false;
  
  const parent = element.parentElement;
  if (!parent) return false;
  
  // Method 1: So sánh getBoundingClientRect width
  const elementRect = element.getBoundingClientRect();
  const parentRect = parent.getBoundingClientRect();
  const elementWidth = elementRect.width;
  const parentWidth = parentRect.width;
  const safetyMargin = 20; // 20px safety margin
  
  const overflowByRect = elementWidth > (parentWidth - safetyMargin);
  
  // Method 2: So sánh scrollWidth với offsetWidth
  const overflowByScroll = element.scrollWidth > (element.offsetWidth + 5);
  
  // Method 3: Check position
  const overflowByPosition = elementRect.right > parentRect.right || elementRect.left < parentRect.left;
  
  const isOverflow = overflowByRect || overflowByScroll || overflowByPosition;
  
  if (isOverflow) {
    console.log(`⚠️ Text overflow detected:`, {
      elementWidth: elementWidth.toFixed(1),
      parentWidth: parentWidth.toFixed(1),
      scrollWidth: element.scrollWidth,
      offsetWidth: element.offsetWidth,
      overflowByRect,
      overflowByScroll,
      overflowByPosition
    });
  }
  
  return isOverflow;
};

// Hàm tính toán scale factor với binary search để tìm scale tối ưu
const calculateOptimalScale = async (element) => {
  if (!element) return 1;
  
  const container = element.parentElement;
  if (!container) return 1;
  
  // Reset về scale 1 để đo kích thước gốc
  element.style.transform = 'scaleX(1)';
  await nextTick();
  
  const containerWidth = container.offsetWidth - 40; // Margin 40px tổng để an toàn hơn
  const originalTextWidth = element.scrollWidth;
  
  console.log(`📏 Đo text "${element.textContent.substring(0, 20)}...":`, {
    containerWidth,
    originalTextWidth,
    overflow: originalTextWidth > containerWidth
  });
  
  // Nếu text vừa với container thì không cần scale
  if (originalTextWidth <= containerWidth) {
    console.log('✅ Text vừa khít, không cần scale');
    return 1;
  }
  
  // Binary search để tìm scale factor tối ưu
  let minScale = 0.2; // Scale tối thiểu thấp hơn
  let maxScale = 1.0;
  let optimalScale = 0.8; // Default safe scale
  let iterations = 0;
  const maxIterations = 20; // Tăng số lần thử
  
  while (minScale <= maxScale && iterations < maxIterations) {
    const midScale = (minScale + maxScale) / 2;
    
    // Test scale này
    element.style.transform = `scaleX(${midScale})`;
    await nextTick();
    
    // Đo lại sau khi apply transform
    const currentWidth = element.getBoundingClientRect().width;
    
    console.log(`🔍 Test scale ${midScale.toFixed(3)}: currentWidth=${currentWidth.toFixed(1)}, container=${containerWidth}`);
    
    if (currentWidth <= containerWidth) {
      // Scale này OK, thử scale lớn hơn
      optimalScale = midScale;
      minScale = midScale + 0.001;
    } else {
      // Scale này quá lớn, thử scale nhỏ hơn
      maxScale = midScale - 0.001;
    }
    
    iterations++;
  }
  
  // Safety margin: giảm 5% để đảm bảo không overflow
  const safeScale = Math.max(0.2, optimalScale * 0.95);
  
  console.log(`🎯 Tìm được optimal scale: ${optimalScale.toFixed(3)}, safe scale: ${safeScale.toFixed(3)} sau ${iterations} lần thử`);
  return safeScale;
};

// Hàm auto-adjust thông minh với iterative scaling
const autoAdjustTextSize = async () => {
  if (!adaptiveTextLine1.value || !adaptiveTextLine2.value) {
    console.warn('⚠️ Text elements chưa ready');
    return;
  }
  
  try {
    console.log('🔄 Bắt đầu auto-adjust text size...');
    
    // Bước 1: Tính optimal scale cho dòng 1 (chủ đạo)
    const optimalScale1 = await calculateOptimalScale(adaptiveTextLine1.value);
    scaleFactorLine1.value = optimalScale1;
    
    // Apply scale cho dòng 1
    adaptiveTextLine1.value.style.transform = `scaleX(${optimalScale1})`;
    await nextTick();
    
    // Bước 2: Tính optimal scale cho dòng 2
    const optimalScale2 = await calculateOptimalScale(adaptiveTextLine2.value);
    
    // Dòng 2 không được lớn hơn dòng 1 (proportional rule)
    const finalScale2 = Math.min(optimalScale2, optimalScale1);
    scaleFactorLine2.value = finalScale2;
    
    // Apply final scales
    adaptiveTextLine1.value.style.transform = `scaleX(${optimalScale1})`;
    adaptiveTextLine2.value.style.transform = `scaleX(${finalScale2})`;
    
    console.log('✅ Auto-adjust hoàn thành:', {
      line1Scale: optimalScale1.toFixed(3),
      line2Scale: finalScale2.toFixed(3),
      line1Overflow: checkTextOverflow(adaptiveTextLine1.value),
      line2Overflow: checkTextOverflow(adaptiveTextLine2.value)
    });
    
    // Final check - nếu vẫn overflow thì force scale nhỏ hơn với multiple iterations
    let finalCheck = 0;
    const maxFinalChecks = 10; // Tăng số lần check cuối
    
    while ((checkTextOverflow(adaptiveTextLine1.value) || checkTextOverflow(adaptiveTextLine2.value)) && finalCheck < maxFinalChecks) {
      console.log(`🔧 Final adjustment #${finalCheck + 1}`);
      const adjustmentFactor = 0.92; // Giảm từng 8% thay vì 5%
      scaleFactorLine1.value *= adjustmentFactor;
      scaleFactorLine2.value *= adjustmentFactor;
      
      // Áp dụng scale mới
      adaptiveTextLine1.value.style.transform = `scaleX(${scaleFactorLine1.value})`;
      adaptiveTextLine2.value.style.transform = `scaleX(${scaleFactorLine2.value})`;
      
      await nextTick();
      
      // Thêm delay nhỏ để đảm bảo rendering hoàn thành
      await new Promise(resolve => setTimeout(resolve, 10));
      
      finalCheck++;
    }
    
    // Ultra-safe final check: Nếu vẫn overflow, force về scale tối thiểu
    if (checkTextOverflow(adaptiveTextLine1.value) || checkTextOverflow(adaptiveTextLine2.value)) {
      console.log(`🚨 Force ultra-safe scaling`);
      scaleFactorLine1.value = Math.max(0.2, scaleFactorLine1.value * 0.8);
      scaleFactorLine2.value = Math.max(0.2, scaleFactorLine2.value * 0.8);
      
      adaptiveTextLine1.value.style.transform = `scaleX(${scaleFactorLine1.value})`;
      adaptiveTextLine2.value.style.transform = `scaleX(${scaleFactorLine2.value})`;
    }
    
  } catch (error) {
    console.error('❌ Lỗi auto-adjust text:', error);
    // Fallback safe values - scale nhỏ hơn để đảm bảo an toàn
    scaleFactorLine1.value = 0.6;
    scaleFactorLine2.value = 0.6;
    
    // Apply fallback scales
    if (adaptiveTextLine1.value && adaptiveTextLine2.value) {
      adaptiveTextLine1.value.style.transform = `scaleX(${scaleFactorLine1.value})`;
      adaptiveTextLine2.value.style.transform = `scaleX(${scaleFactorLine2.value})`;
    }
  }
};

// Debounce function với delay ngắn hơn cho responsive tốt hơn
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

// Throttle function để giới hạn số lần gọi liên tiếp
const throttle = (func, limit) => {
  let inThrottle;
  return function(...args) {
    if (!inThrottle) {
      func.apply(this, args);
      inThrottle = true;
      setTimeout(() => inThrottle = false, limit);
    }
  };
};

// Debounced resize handler với delay 100ms cho responsive nhanh
const debouncedAutoAdjust = debounce(autoAdjustTextSize, 100);

// Throttled version cho events thường xuyên
const throttledAutoAdjust = throttle(autoAdjustTextSize, 200);

// Real-time monitoring với MutationObserver
let resizeObserver = null;
let mutationObserver = null;

onMounted(() => {
  if (!isAuthenticated()) {
    router.push('/login');
    return;
  }
  
  // Khởi tạo auto-adjust sau khi component mounted với multiple delays để chắc chắn
  setTimeout(autoAdjustTextSize, 100);
  setTimeout(autoAdjustTextSize, 300);
  setTimeout(autoAdjustTextSize, 500);
  
  // Lắng nghe resize window
  window.addEventListener('resize', debouncedAutoAdjust);
  
  // Lắng nghe orientation change cho mobile
  window.addEventListener('orientationchange', () => {
    setTimeout(autoAdjustTextSize, 200);
  });
  
  // Sử dụng ResizeObserver để theo dõi container
  if (window.ResizeObserver && adaptiveTextLine1.value?.parentElement) {
    resizeObserver = new ResizeObserver(throttledAutoAdjust);
    resizeObserver.observe(adaptiveTextLine1.value.parentElement);
    
    // Observe cả hai text elements
    if (adaptiveTextLine2.value?.parentElement) {
      resizeObserver.observe(adaptiveTextLine2.value.parentElement);
    }
  }
  
  // Lắng nghe font load events với multiple checks
  if (document.fonts) {
    document.fonts.ready.then(() => {
      setTimeout(autoAdjustTextSize, 100);
    });
    
    // Listen for font load events
    document.fonts.addEventListener('loadingdone', () => {
      setTimeout(autoAdjustTextSize, 50);
    });
  }
  
  // Mutation observer để theo dõi thay đổi DOM
  if (window.MutationObserver) {
    mutationObserver = new MutationObserver(throttledAutoAdjust);
    
    if (adaptiveTextLine1.value?.parentElement) {
      mutationObserver.observe(adaptiveTextLine1.value.parentElement, {
        attributes: true,
        childList: true,
        subtree: true,
        attributeFilter: ['style', 'class']
      });
    }
  }
  
  console.log('🎨 Enhanced adaptive text system initialized');
});

onUnmounted(() => {
  // Cleanup event listeners
  window.removeEventListener('resize', debouncedAutoAdjust);
  window.removeEventListener('orientationchange', debouncedAutoAdjust);
  
  if (resizeObserver) {
    resizeObserver.disconnect();
  }
  
  if (mutationObserver) {
    mutationObserver.disconnect();
  }
  
  console.log('🧹 Adaptive text system cleaned up');
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
