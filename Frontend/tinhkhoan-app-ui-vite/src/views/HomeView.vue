<template>
  <div class="home">
    <div class="welcome-hero">
      <div class="hero-content">
        <h1 class="hero-title">
          <img src="/src/assets/Logo-Agribank-2.png" alt="Agribank Logo" class="hero-logo" />
          <br />
          <div class="text-container">
            <span ref="adaptiveTextLine1" class="hero-text adaptive-text-line-1"> AGRIBANK LAI CHAU CENTER </span>
          </div>
        </h1>
        <div class="hero-subtitle">
          <div class="text-container">
            <span ref="adaptiveTextLine2" class="hero-text adaptive-text-line-2">
              HỆ THỐNG QUẢN LÝ KHOÁN | HỆ THỐNG BÁO CÁO
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { isAuthenticated } from '../services/auth'

const router = useRouter()

// Refs cho 2 dòng text
const adaptiveTextLine1 = ref(null)
const adaptiveTextLine2 = ref(null)

// 🎨 Simple text effects - chỉ màu đỏ và trắng ngọc trai
const textEffectState = ref('classic') // classic hoặc pearl

// 🎨 Apply simple text effects
const applyTextEffects = () => {
  const textElement = adaptiveTextLine1.value
  if (!textElement) return

  // Remove all weather classes (nếu còn sót)
  textElement.classList.remove(
    'weather-sunny',
    'weather-rainy',
    'weather-cloudy',
    'weather-stormy',
    'weather-snowy',
    'weather-clear-night'
  )

  // Remove previous effect classes
  textElement.classList.remove('text-effect-classic', 'text-effect-pearl')

  // Add current effect class
  textElement.classList.add(`text-effect-${textEffectState.value}`)

  // Toggle effect với tỷ lệ 30% thời gian đỏ bordeaux, 70% thời gian trắng ngọc trai
  setTimeout(() => {
    if (textEffectState.value === 'classic') {
      textEffectState.value = 'pearl' // Chuyển sang trắng ngọc trai
      setTimeout(() => {
        applyTextEffects() // Gọi lại hàm này sẽ chuyển về classic sau khoảng thời gian dài hơn
      }, 21000) // 70% thời gian (70% của 30s = 21s) hiển thị trắng ngọc trai
    } else {
      textEffectState.value = 'classic' // Chuyển về đỏ bordeaux
      setTimeout(() => {
        applyTextEffects() // Gọi lại hàm này sẽ chuyển về pearl sau khoảng thời gian ngắn hơn
      }, 9000) // 30% thời gian (30% của 30s = 9s) hiển thị đỏ bordeaux
    }
  }, 100) // Đợi 100ms để đảm bảo DOM đã cập nhật

  console.log('🎨 Applied text effect:', textEffectState.value, 'with 30/70 timing ratio')
}

// 🎯 SIÊU PERFECT: Hàm tính toán và áp dụng scale cho từng dòng text để fit cửa sổ HOÀN TOÀN
const adjustTextSize = element => {
  if (!element) return

  try {
    // Lấy container của element
    const container = element.closest('.text-container')
    if (!container) return

    // Reset hoàn toàn styles để đo kích thước gốc
    element.style.transform = 'none'
    element.style.fontSize = ''
    element.style.width = 'auto'
    element.style.maxWidth = 'none'
    element.style.minWidth = 'auto'

    // Force multiple reflows để đảm bảo kích thước được tính lại hoàn toàn
    container.offsetHeight
    element.offsetWidth
    void container.offsetWidth

    // Đợi để DOM hoàn toàn stabilize
    requestAnimationFrame(() => {
      requestAnimationFrame(() => {
        // Lấy kích thước viewport và container
        const viewportWidth = window.innerWidth
        const viewportHeight = window.innerHeight
        const containerWidth = container.offsetWidth

        // 🔥 SIÊU CHÍNH XÁC: Tạo element clone để đo kích thước thật
        const clone = element.cloneNode(true)
        clone.style.position = 'absolute'
        clone.style.top = '-9999px'
        clone.style.left = '-9999px'
        clone.style.transform = 'none'
        clone.style.fontSize = window.getComputedStyle(element).fontSize
        clone.style.fontFamily = window.getComputedStyle(element).fontFamily
        clone.style.fontWeight = window.getComputedStyle(element).fontWeight
        clone.style.letterSpacing = window.getComputedStyle(element).letterSpacing
        clone.style.whiteSpace = 'nowrap'
        clone.style.visibility = 'hidden'
        clone.style.width = 'auto'
        clone.style.maxWidth = 'none'
        clone.style.minWidth = 'auto'
        document.body.appendChild(clone)

        // Đo kích thước thực tế sử dụng scrollWidth để chính xác hơn
        const realTextWidth = Math.max(clone.offsetWidth, clone.scrollWidth)
        const realTextHeight = clone.offsetHeight
        document.body.removeChild(clone)

        // 🎯 SIÊU ULTRA ADAPTIVE: Tính toán maxWidth dựa trên device và content
        let maxAllowedWidth
        const isMobile = viewportWidth < 768
        const isTablet = viewportWidth >= 768 && viewportWidth < 1024
        const isLandscape = viewportWidth > viewportHeight
        const isUltraSmall = viewportWidth < 350

        if (isUltraSmall) {
          // Ultra small screens: sử dụng 95-98% viewport
          maxAllowedWidth = viewportWidth * 0.95
        } else if (isMobile) {
          // Mobile: sử dụng 88-95% viewport tùy theo orientation
          maxAllowedWidth = viewportWidth * (isLandscape ? 0.95 : 0.88)
        } else if (isTablet) {
          // Tablet: sử dụng 85-92% viewport
          maxAllowedWidth = viewportWidth * (isLandscape ? 0.92 : 0.85)
        } else {
          // Desktop: sử dụng container width hoặc 88% viewport
          maxAllowedWidth = Math.min(containerWidth - 40, viewportWidth * 0.88)
        }

        // Đảm bảo maxWidth không quá nhỏ
        maxAllowedWidth = Math.max(maxAllowedWidth, 120)

        // Tính toán scale chính xác
        let scale = 1
        if (realTextWidth > maxAllowedWidth) {
          scale = maxAllowedWidth / realTextWidth
        }

        // 🎯 SIÊU ADAPTIVE: Giới hạn scale dựa trên device và text content
        const textContent = element.textContent || ''
        const isMainTitle = textContent.includes('AGRIBANK LAI CHAU')

        let minScale, maxScale
        if (isUltraSmall) {
          minScale = 0.08 // Cho phép thu nhỏ RẤT RẤT nhiều trên ultra small
          maxScale = 1.8
        } else if (isMobile) {
          minScale = isMainTitle ? 0.1 : 0.12 // Cho phép thu nhỏ RẤT nhiều trên mobile
          maxScale = 1.6 // Có thể phóng to trên mobile nhỏ
        } else if (isTablet) {
          minScale = isMainTitle ? 0.2 : 0.25
          maxScale = 1.3
        } else {
          minScale = isMainTitle ? 0.3 : 0.35
          maxScale = 1.1
        }

        scale = Math.max(minScale, Math.min(maxScale, scale))

        // 🎯 SIÊU CENTER: Áp dụng transform với origin center hoàn hảo
        element.style.transform = `scale(${scale})`
        element.style.transformOrigin = 'center center'
        element.style.position = 'relative'
        element.style.display = 'inline-block'
        element.style.whiteSpace = 'nowrap'
        element.style.width = 'auto'
        element.style.maxWidth = 'none'
        element.style.minWidth = 'auto'

        // 🎯 SIÊU CENTER: Đảm bảo container center hoàn hảo
        container.style.display = 'flex'
        container.style.justifyContent = 'center'
        container.style.alignItems = 'center'
        container.style.overflow = 'visible'
        container.style.textAlign = 'center'
        container.style.width = '100%'
        container.style.boxSizing = 'border-box'

        // 🎯 SIÊU RESPONSIVE: Điều chỉnh spacing và padding dựa trên scale
        const scaledWidth = realTextWidth * scale
        const extraSpace = Math.max(0, viewportWidth - scaledWidth)

        if (extraSpace > 100) {
          container.style.padding = `10px ${Math.min(extraSpace / 6, 25)}px`
        } else if (extraSpace > 40) {
          container.style.padding = `8px ${Math.min(extraSpace / 4, 15)}px`
        } else {
          container.style.padding = isUltraSmall ? '4px 1px' : isMobile ? '6px 2px' : '8px 4px'
        }

        // 🎯 SIÊU LOG: Chi tiết để debug
        console.log(`🎯 SIÊU AUTO-FIT: "${textContent.substring(0, 20)}..."`)
        console.log(
          `📱 Device: ${isUltraSmall ? 'Ultra-Small' : isMobile ? 'Mobile' : isTablet ? 'Tablet' : 'Desktop'} ${isLandscape ? '(Landscape)' : '(Portrait)'}`
        )
        console.log(`📏 Viewport: ${viewportWidth}x${viewportHeight}, Container: ${containerWidth}px`)
        console.log(
          `📐 RealText: ${realTextWidth}px → Scaled: ${scaledWidth.toFixed(1)}px (scale: ${scale.toFixed(3)})`
        )
        console.log(
          `🎯 MaxWidth: ${maxAllowedWidth.toFixed(0)}px, Usage: ${((scaledWidth / maxAllowedWidth) * 100).toFixed(1)}%`
        )
        console.log(`✅ Success: Text perfectly fits with scale bounds [${minScale}-${maxScale}]`)
      })
    })
  } catch (error) {
    console.error('🚨 Error in adjustTextSize:', error)
    // Fallback: basic scaling based on viewport
    if (element) {
      const vw = window.innerWidth
      const fallbackScale = vw < 350 ? 0.15 : vw < 480 ? 0.5 : vw < 768 ? 0.7 : 0.9
      element.style.transform = `scale(${fallbackScale})`
      element.style.transformOrigin = 'center center'
    }
  }
}

// Hàm chính để điều chỉnh cả hai dòng
const autoAdjustAllText = () => {
  adjustTextSize(adaptiveTextLine1.value)
  adjustTextSize(adaptiveTextLine2.value)
}

// Debounce function
const debounce = (func, wait) => {
  let timeout
  return (...args) => {
    clearTimeout(timeout)
    timeout = setTimeout(() => func(...args), wait)
  }
}

// Debounced version cho performance - nhanh hơn cho responsive
const debouncedAdjust = debounce(autoAdjustAllText, 150) // Giảm từ 300ms xuống 150ms

// 🎯 Immediate adjust không debounce cho resize quan trọng
const immediateAdjust = () => {
  requestAnimationFrame(() => {
    autoAdjustAllText()
    applyTextEffects()
  })
}

// Observer để theo dõi thay đổi kích thước
let resizeObserver = null

onMounted(async () => {
  if (!isAuthenticated()) {
    router.push('/login')
    return
  }

  // Khởi tạo text sizing với delay
  setTimeout(() => {
    autoAdjustAllText()
    applyTextEffects()
  }, 100)

  setTimeout(() => {
    autoAdjustAllText()
    applyTextEffects()
  }, 500)

  // Theo dõi sự kiện resize window với immediate response
  window.addEventListener('resize', () => {
    immediateAdjust() // Immediate cho resize quan trọng
    debouncedAdjust() // Debounce backup
  })

  // Theo dõi orientation change trên mobile
  window.addEventListener('orientationchange', () => {
    setTimeout(immediateAdjust, 200) // Delay ngắn cho orientation change
  })

  // Dùng ResizeObserver để theo dõi thay đổi kích thước container
  if (window.ResizeObserver) {
    resizeObserver = new ResizeObserver(() => {
      debouncedAdjust()
      applyTextEffects()
    })

    // Theo dõi cả hai container
    const containers = document.querySelectorAll('.text-container')
    containers.forEach(container => {
      resizeObserver.observe(container)
    })
  }

  console.log('🎨 Simple adaptive text system initialized')
})

onUnmounted(() => {
  // Cleanup
  window.removeEventListener('resize', immediateAdjust)
  window.removeEventListener('resize', debouncedAdjust)
  window.removeEventListener('orientationchange', immediateAdjust)

  if (resizeObserver) {
    resizeObserver.disconnect()
  }

  console.log('🧹 Simple text system cleaned up')
})
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
  padding-top: 0cm; /* Giảm từ 1cm xuống 0cm để dịch logo và chữ lên trên thêm 1cm nữa */
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

/* Dòng 1: AGRIBANK LAI CHAU CENTER - HIỆU ỨNG ĐƠN GIẢN ĐỎ VÀ TRẮNG NGỌC TRAI */
.adaptive-text-line-1 {
  font-size: clamp(1.8rem, 8vw, 6.5rem);
  letter-spacing: clamp(0.02em, 0.5vw, 0.08em);
  transform: scale(1);
  font-family: 'Montserrat', 'Roboto', sans-serif;
  font-weight: 800;
  /* Màu đỏ bordeaux cố định */
  color: #8b1538;
  /* Hiệu ứng đơn giản */
  text-shadow:
    0 0 20px rgba(139, 21, 56, 0.6),
    0 4px 8px rgba(139, 21, 56, 0.4),
    0 2px 4px rgba(0, 0, 0, 0.3);
  /* Base animation nhẹ nhàng */
  animation: agribank-classic-glow 4s ease-in-out infinite;
  will-change: transform, text-shadow;
  word-spacing: clamp(-0.05em, 0vw, 0.02em);
  position: relative;
  /* Responsive scaling */
  max-width: 100%;
  width: fit-content;
  white-space: nowrap;
  overflow: visible;
}

/* 🎨 HIỆU ỨNG CLASSIC - ĐỎ BORDEAUX */
.adaptive-text-line-1.text-effect-classic {
  color: #8b1538;
  text-shadow:
    0 0 25px rgba(139, 21, 56, 0.8),
    0 4px 8px rgba(139, 21, 56, 0.5),
    0 2px 4px rgba(0, 0, 0, 0.4);
  animation: agribank-classic-glow 4s ease-in-out infinite;
  transition:
    color 5s ease-in-out,
    text-shadow 5s ease-in-out; /* Transition chậm hơn */
}

/* 🎨 HIỆU ỨNG PEARL - TRẮNG NGỌC TRAI */
.adaptive-text-line-1.text-effect-pearl {
  color: #f8f8ff; /* Ghost White - trắng ngọc trai */
  text-shadow:
    0 0 30px rgba(248, 248, 255, 0.9),
    0 0 20px rgba(220, 220, 220, 0.7),
    0 4px 8px rgba(200, 200, 200, 0.5),
    0 2px 4px rgba(100, 100, 100, 0.4);
  animation: pearl-shimmer 5s ease-in-out infinite;
  transition:
    color 5s ease-in-out,
    text-shadow 5s ease-in-out; /* Transition chậm hơn */
}

@keyframes agribank-classic-glow {
  0%,
  100% {
    text-shadow:
      0 0 20px rgba(139, 21, 56, 0.6),
      0 4px 8px rgba(139, 21, 56, 0.4),
      0 2px 4px rgba(0, 0, 0, 0.3);
  }
  50% {
    text-shadow:
      0 0 30px rgba(139, 21, 56, 0.8),
      0 4px 12px rgba(139, 21, 56, 0.6),
      0 2px 6px rgba(0, 0, 0, 0.4);
  }
}

@keyframes pearl-shimmer {
  0%,
  100% {
    text-shadow:
      0 0 25px rgba(248, 248, 255, 0.8),
      0 0 15px rgba(220, 220, 255, 0.6),
      0 4px 8px rgba(200, 200, 200, 0.4),
      0 2px 4px rgba(100, 100, 100, 0.3);
  }
  50% {
    text-shadow:
      0 0 35px rgba(248, 248, 255, 1),
      0 0 25px rgba(240, 240, 240, 0.8),
      0 4px 12px rgba(220, 220, 220, 0.6),
      0 2px 6px rgba(150, 150, 150, 0.4);
  }
}

/* Dòng 2: HỆ THỐNG QUẢN LÝ KHOÁN | HỆ THỐNG BÁO CÁO */
.adaptive-text-line-2 {
  font-size: clamp(1rem, 4vw, 2.5rem);
  letter-spacing: clamp(0.01em, 0.3vw, 0.05em);
  transform: scale(1);
  font-family: 'Montserrat', 'Roboto', sans-serif;
  font-weight: 600;
  color: #8b1538; /* Đổi màu từ xám sang đỏ bordeaux giống màu dòng 1 */
  text-shadow:
    0 0 15px rgba(139, 21, 56, 0.5),
    0 2px 4px rgba(139, 21, 56, 0.3),
    0 1px 2px rgba(0, 0, 0, 0.2);
  animation:
    subtitle-glow 3s ease-in-out infinite,
    star-light 5s linear infinite; /* Thêm hiệu ứng tia sáng ngôi sao */
  position: relative;
  max-width: 100%;
  width: fit-content;
  white-space: nowrap;
  overflow: visible;
}

@keyframes subtitle-glow {
  0%,
  100% {
    text-shadow:
      0 0 10px rgba(139, 21, 56, 0.4),
      0 2px 4px rgba(139, 21, 56, 0.2),
      0 1px 2px rgba(0, 0, 0, 0.2);
  }
  50% {
    text-shadow:
      0 0 20px rgba(139, 21, 56, 0.6),
      0 2px 6px rgba(139, 21, 56, 0.4),
      0 1px 3px rgba(0, 0, 0, 0.3);
  }
}

/* Hiệu ứng tia sáng ngôi sao chạy từ trái qua phải */
@keyframes star-light {
  0% {
    background-image: linear-gradient(90deg, transparent 0%, transparent 0%);
    background-position: 0% 50%;
    background-size: 200% 100%;
    background-repeat: no-repeat;
    -webkit-background-clip: text;
    background-clip: text;
  }
  5% {
    background-image: linear-gradient(90deg, transparent 0%, rgba(255, 255, 255, 0.8) 50%, transparent 100%);
    background-position: 0% 50%;
    background-size: 200% 100%;
    background-repeat: no-repeat;
    -webkit-background-clip: text;
    background-clip: text;
  }
  15% {
    background-image: linear-gradient(90deg, transparent 0%, rgba(255, 255, 255, 0.8) 50%, transparent 100%);
    background-position: 100% 50%;
    background-size: 200% 100%;
    background-repeat: no-repeat;
    -webkit-background-clip: text;
    background-clip: text;
  }
  20%,
  100% {
    background-image: linear-gradient(90deg, transparent 0%, transparent 0%);
    background-position: 100% 50%;
    background-size: 200% 100%;
    background-repeat: no-repeat;
    -webkit-background-clip: text;
    background-clip: text;
  }
}

/* Logo animation */
.hero-logo {
  height: 90px;
  width: auto;
  margin-top: 10px; /* Giảm từ 25px xuống 10px để dịch logo lên */
  margin-bottom: 15px; /* Giảm từ 20px xuống 15px để gần với chữ hơn */
  filter: drop-shadow(0 8px 16px rgba(139, 21, 56, 0.5));
  animation: gentle-sway 4s ease-in-out infinite;
  transform: scale(1.1);
}

@keyframes gentle-sway {
  0%,
  100% {
    transform: rotate(-2deg) scale(1.1);
  }
  50% {
    transform: rotate(2deg) scale(1.1);
  }
}

.hero-subtitle {
  font-size: clamp(1.2rem, 4vw, 2.6rem);
  font-family: 'Inter', 'Segoe UI', 'Roboto', 'Arial', sans-serif;
  margin-bottom: 0;
  margin-top: -30px; /* Giảm từ -10px xuống -30px để dịch lên sát gần dòng trên hơn */
  font-weight: 700;
  font-style: normal;
  letter-spacing: 0.05em;
  line-height: 1.3;
  width: 100%;
  max-width: 100%;
  overflow: visible;
  text-align: center;
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
