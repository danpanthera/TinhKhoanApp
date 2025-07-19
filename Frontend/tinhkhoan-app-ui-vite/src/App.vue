<template>
  <div class="app-container">
    <!-- Dynamic Background -->
    <div class="dynamic-background">
      <div
        v-for="(image, index) in backgroundImages"
        :key="index"
        :class="['background-slide', { active: currentImageIndex === index }]"
        :style="{ backgroundImage: `url(${image})` }"
      ></div>
      <div class="background-overlay"></div>

      <!-- Background indicators -->
      <div class="background-indicators">
        <div
          v-for="(image, index) in backgroundImages"
          :key="`indicator-${index}`"
          :class="['indicator', { active: currentImageIndex === index }]"
          @click="currentImageIndex = index"
          :title="backgroundNames[index]"
        ></div>
      </div>

      <!-- Background name display - HIDDEN -->
      <!-- <div class="background-name">
        {{ backgroundNames[currentImageIndex] }}
      </div> -->
    </div>

    <!-- Main Content -->
    <div class="main-content">
      <nav class="main-nav">
        <router-link to="/" class="nav-logo">
          <img src="/Logo-Agribank-2.png" alt="Agribank Logo" class="nav-logo-img" />
        </router-link>

        <!-- HR Information Dropdown Menu -->
        <div class="nav-dropdown" @mouseenter="handleHRMouseEnter" @mouseleave="handleHRMouseLeave">
          <a href="#" class="nav-dropdown-trigger" :class="{ active: isHRSectionActive }">
            <span>🏢 Chi nhánh/Nhân sự</span>
            <svg class="dropdown-arrow" :class="{ rotated: showHRMenu }" viewBox="0 0 24 24" width="16" height="16">
              <path fill="currentColor" d="M7 10l5 5 5-5z"/>
            </svg>
          </a>
          <div class="nav-dropdown-menu" :class="{ show: showHRMenu }" @click="hideAllMenus">
            <router-link to="/units" class="dropdown-item">
              <span class="item-icon">🏬</span>
              <span>Đơn vị</span>
            </router-link>
            <router-link to="/employees" class="dropdown-item">
              <span class="item-icon">👥</span>
              <span>Nhân viên</span>
            </router-link>
            <router-link to="/positions" class="dropdown-item">
              <span class="item-icon">💼</span>
              <span>Chức vụ</span>
            </router-link>
            <router-link to="/roles" class="dropdown-item">
              <span class="item-icon">🎭</span>
              <span>Vai trò</span>
            </router-link>
          </div>
        </div>

        <!-- KPI Management Dropdown Menu -->
        <div class="nav-dropdown" @mouseenter="handleKPIMouseEnter" @mouseleave="handleKPIMouseLeave">
          <a href="#" class="nav-dropdown-trigger" :class="{ active: isKPISectionActive }">
            <span>📊 Quản lý KPI</span>
            <svg class="dropdown-arrow" :class="{ rotated: showKPIMenu }" viewBox="0 0 24 24" width="16" height="16">
              <path fill="currentColor" d="M7 10l5 5 5-5z"/>
            </svg>
          </a>
          <div class="nav-dropdown-menu" :class="{ show: showKPIMenu }" @click="hideAllMenus">
            <router-link to="/khoan-periods" class="dropdown-item">
              <span class="item-icon">📅</span>
              <span>Kỳ khoán</span>
            </router-link>
            <router-link to="/kpi-definitions" class="dropdown-item">
              <span class="item-icon">⚙️</span>
              <span>Cấu hình KPI</span>
            </router-link>
            <router-link to="/employee-kpi-assignment" class="dropdown-item">
              <span class="item-icon">👤</span>
              <span>Giao khoán KPI theo Cán bộ</span>
            </router-link>
            <router-link to="/unit-kpi-assignment" class="dropdown-item">
              <span class="item-icon">🏢</span>
              <span>Giao khoán KPI theo Chi nhánh</span>
            </router-link>
            <router-link to="/kpi-actual-values" class="dropdown-item">
              <span class="item-icon">📊</span>
              <span>Cập nhật Giá trị thực hiện</span>
            </router-link>
            <router-link to="/kpi-scoring" class="dropdown-item">
              <span class="item-icon">🎯</span>
              <span>Chấm điểm KPI</span>
            </router-link>
            <router-link to="/unit-kpi-scoring" class="dropdown-item">
              <span class="item-icon">🏢</span>
              <span>Chấm điểm KPI Chi nhánh</span>
            </router-link>
            <router-link to="/kpi-scoring" class="dropdown-item">
              <span class="item-icon">🎮</span>
              <span>Demo Chấm điểm KPI</span>
            </router-link>
            <router-link to="/data-import" class="dropdown-item">
              <span class="item-icon">🗄️</span>
              <span>KHO DỮ LIỆU THÔ</span>
            </router-link>
            <router-link to="/payroll-report" class="dropdown-item">
              <span class="item-icon">💰</span>
              <span>Bảng lương/Báo cáo</span>
            </router-link>
          </div>
        </div>

        <!-- Dashboard Dropdown Menu -->
        <div class="nav-dropdown" @mouseenter="handleDashboardMouseEnter" @mouseleave="handleDashboardMouseLeave">
          <a href="#" class="nav-dropdown-trigger" :class="{ active: isDashboardSectionActive }">
            <span>📈 Dashboard</span>
            <svg class="dropdown-arrow" :class="{ rotated: showDashboardMenu }" viewBox="0 0 24 24" width="16" height="16">
              <path fill="currentColor" d="M7 10l5 5 5-5z"/>
            </svg>
          </a>
          <div class="nav-dropdown-menu" :class="{ show: showDashboardMenu }" @click="hideAllMenus">
            <router-link to="/dashboard/target-assignment" class="dropdown-item">
              <span class="item-icon">🎯</span>
              <span>1. Giao chỉ tiêu</span>
            </router-link>
            <router-link to="/dashboard/calculation" class="dropdown-item">
              <span class="item-icon">🧮</span>
              <span>2. Cập nhật</span>
            </router-link>
            <router-link to="/dashboard/business-plan" class="dropdown-item">
              <span class="item-icon">�</span>
              <span>3. Dashboard</span>
            </router-link>
          </div>
        </div>

        <!-- About Dropdown Menu -->
        <div class="nav-dropdown" @mouseenter="handleAboutMouseEnter" @mouseleave="handleAboutMouseLeave">
          <a href="#" class="nav-dropdown-trigger" :class="{ active: isAboutSectionActive }">
            <span>ℹ️ Giới thiệu</span>
            <svg class="dropdown-arrow" :class="{ rotated: showAboutMenu }" viewBox="0 0 24 24" width="16" height="16">
              <path fill="currentColor" d="M7 10l5 5 5-5z"/>
            </svg>
          </a>
          <div class="nav-dropdown-menu" :class="{ show: showAboutMenu }" @click="hideAllMenus">
            <router-link to="/about/info" class="dropdown-item">
              <span class="item-icon">©️</span>
              <span>Thông tin bản quyền</span>
            </router-link>
            <router-link to="/about/user-guide" class="dropdown-item">
              <span class="item-icon">📖</span>
              <span>Hướng dẫn sử dụng</span>
            </router-link>
            <router-link to="/about/software-info" class="dropdown-item">
              <span class="item-icon">🔧</span>
              <span>Thông tin phần mềm</span>
            </router-link>
          </div>
        </div>

        <span class="nav-spacer"></span>
        <!-- Compact controls -->
        <div class="nav-compact-controls">
          <!-- Theme Switcher -->
          <ThemeSwitcher />
          <a href="#" @click.prevent="handleLogout" class="logout-btn">
            <span class="logout-icon">🚪</span>
            <span class="logout-text">Đăng xuất</span>
          </a>
        </div>
      </nav>
      <div class="content-container">
        <router-view />
      </div>

      <!-- App Footer - Chân trang với thông tin user -->
      <AppFooter />
    </div>

    <!-- PWA Install Prompt và các thông báo PWA -->
    <PWAInstallPrompt />

    <!-- Offline Indicator and Sync Status -->
    <OfflineIndicator />

    <!-- Debug Panel for development -->
    <DebugPanel v-if="isDevelopment" />
  </div>
</template>

<script setup>
import AppFooter from '@/components/AppFooter.vue';
import DebugPanel from '@/components/DebugPanel.vue';
import OfflineIndicator from '@/components/OfflineIndicator.vue';
import PWAInstallPrompt from '@/components/PWAInstallPrompt.vue';
import ThemeSwitcher from '@/components/ThemeSwitcher.vue';
import { isAuthenticated, logout } from '@/services/auth';
import { computed, onMounted, onUnmounted, ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';

const router = useRouter();
const route = useRoute();

// Development flag for debug panel
const isDevelopment = ref(import.meta.env.DEV);

if (!isAuthenticated() && router.currentRoute.value.path !== '/login') {
  router.push('/login');
}

const handleLogout = () => {
  logout();
  router.push('/login');
};

// HR Dropdown menu state
const showHRMenu = ref(false);

// KPI Dropdown menu state
const showKPIMenu = ref(false);

// Dashboard Dropdown menu state
const showDashboardMenu = ref(false);

// About Dropdown menu state
const showAboutMenu = ref(false);

// Dropdown menu timeout references
const hrMenuTimeout = ref(null);
const kpiMenuTimeout = ref(null);
const dashboardMenuTimeout = ref(null);
const aboutMenuTimeout = ref(null);

// Enhanced dropdown handlers with auto-hide functionality
const handleHRMouseEnter = () => {
  if (hrMenuTimeout.value) {
    clearTimeout(hrMenuTimeout.value);
    hrMenuTimeout.value = null;
  }
  showHRMenu.value = true;
};

const handleHRMouseLeave = () => {
  hrMenuTimeout.value = setTimeout(() => {
    showHRMenu.value = false;
  }, 300); // 300ms delay before hiding
};

const handleKPIMouseEnter = () => {
  if (kpiMenuTimeout.value) {
    clearTimeout(kpiMenuTimeout.value);
    kpiMenuTimeout.value = null;
  }
  showKPIMenu.value = true;
};

const handleKPIMouseLeave = () => {
  kpiMenuTimeout.value = setTimeout(() => {
    showKPIMenu.value = false;
  }, 300); // 300ms delay before hiding
};

const handleDashboardMouseEnter = () => {
  if (dashboardMenuTimeout.value) {
    clearTimeout(dashboardMenuTimeout.value);
    dashboardMenuTimeout.value = null;
  }
  showDashboardMenu.value = true;
};

const handleDashboardMouseLeave = () => {
  dashboardMenuTimeout.value = setTimeout(() => {
    showDashboardMenu.value = false;
  }, 300); // 300ms delay before hiding
};

const handleAboutMouseEnter = () => {
  if (aboutMenuTimeout.value) {
    clearTimeout(aboutMenuTimeout.value);
    aboutMenuTimeout.value = null;
  }
  showAboutMenu.value = true;
};

const handleAboutMouseLeave = () => {
  aboutMenuTimeout.value = setTimeout(() => {
    showAboutMenu.value = false;
  }, 300); // 300ms delay before hiding
};

// Click-to-hide functionality
const hideAllMenus = () => {
  showHRMenu.value = false;
  showKPIMenu.value = false;
  showDashboardMenu.value = false;
  showAboutMenu.value = false;

  // Clear any pending timeouts
  if (hrMenuTimeout.value) {
    clearTimeout(hrMenuTimeout.value);
    hrMenuTimeout.value = null;
  }
  if (kpiMenuTimeout.value) {
    clearTimeout(kpiMenuTimeout.value);
    kpiMenuTimeout.value = null;
  }
  if (dashboardMenuTimeout.value) {
    clearTimeout(dashboardMenuTimeout.value);
    dashboardMenuTimeout.value = null;
  }
  if (aboutMenuTimeout.value) {
    clearTimeout(aboutMenuTimeout.value);
    aboutMenuTimeout.value = null;
  }
};

// Handle clicking outside menus
const handleDocumentClick = (event) => {
  const dropdownElements = document.querySelectorAll('.nav-dropdown');
  let clickedInsideDropdown = false;

  dropdownElements.forEach(dropdown => {
    if (dropdown.contains(event.target)) {
      clickedInsideDropdown = true;
    }
  });

  if (!clickedInsideDropdown) {
    hideAllMenus();
  }
};

// Handle route changes - hide menus when navigating
const handleRouteChange = () => {
  hideAllMenus();
};

// Check if current route is in HR section
const isHRSectionActive = computed(() => {
  const hrRoutes = ['/units', '/employees', '/positions', '/roles'];
  return hrRoutes.includes(route.path);
});

// Check if current route is in KPI section
const isKPISectionActive = computed(() => {
  const kpiRoutes = ['/khoan-periods', '/kpi-definitions', '/kpi-config', '/employee-kpi-assignment', '/unit-kpi-assignment', '/kpi-actual-values', '/kpi-input', '/kpi-score', '/kpi-scoring', '/payroll-report'];
  return kpiRoutes.includes(route.path);
});

// Check if current route is in Dashboard section
const isDashboardSectionActive = computed(() => {
  const dashboardRoutes = ['/dashboard', '/dashboard/target-assignment', '/dashboard/calculation', '/dashboard/business-plan'];
  return dashboardRoutes.includes(route.path);
});

// Check if current route is in About section
const isAboutSectionActive = computed(() => {
  const aboutRoutes = ['/about/info', '/about/user-guide', '/about/software-info'];
  return aboutRoutes.includes(route.path);
});

// Dynamic background setup
const currentImageIndex = ref(0);
const backgroundImages = ref([]);
const backgroundNames = ref([]);

let backgroundInterval = null;

// 🎨 Tự động load TẤT CẢ ảnh từ thư mục backgrounds (Cải tiến)
const loadBackgroundImages = async () => {
  try {
    const backgroundPath = '/images/backgrounds/';
    const supportedExtensions = ['.jpg', '.jpeg', '.png', '.webp', '.gif'];

    // 📝 Danh sách tên file có thể có trong thư mục (bao gồm file hiện có + 2 ảnh mới từ Pexels)
    const potentialFileNames = [
      // ⭐ 2 Ảnh thiên nhiên siêu đẹp mới từ Pexels.com
      'epic-mountain-canyon', 'crater-lake-mountains',
      // Files hiện có trong thư mục
      'AgribankLaiChau_chuan', 'anh-dep-lai-chau-29', 'background-2', 'background-3', 'File_000',
      'nature-green-forest-path-hdr', 'nature-lake-forest-hdr', 'nature-mountain-sunset-hdr',
      // Tên thông thường
      'background-1', 'background-4', 'background-5',
      'bg-1', 'bg-2', 'bg-3', 'bg-4', 'bg-5',
      'image-1', 'image-2', 'image-3', 'image-4', 'image-5',
      // Các file lai châu khác (có thể tồn tại)
      'anh-dep-lai-chau-8', 'anh-dep-lai-chau-16', 'anh-dep-lai-chau-19', 'anh-dep-lai-chau-33',
      // Số đơn giản
      '1', '2', '3', '4', '5', '6', '7', '8', '9', '10', '11', '12', '13', '14', '15',
      // Các tên khác có thể
      'wallpaper-1', 'wallpaper-2', 'wallpaper-3', 'nature-1', 'nature-2',
      'scenery-1', 'scenery-2', 'landscape-1', 'landscape-2'
    ];

    const loadedImages = [];
    const loadedNames = [];

    // 🔍 Kiểm tra từng combination tên file + extension
    // console.log('🔍 Đang quét thư mục backgrounds...');
    let totalChecked = 0;

    for (const fileName of potentialFileNames) {
      for (const ext of supportedExtensions) {
        const fullPath = `${backgroundPath}${fileName}${ext}`;
        totalChecked++;

        try {
          // Test ảnh có load được không
          const img = new Image();
          await new Promise((resolve, reject) => {
            const timeout = setTimeout(() => reject(new Error('Timeout')), 2000);
            img.onload = () => {
              clearTimeout(timeout);
              resolve();
            };
            img.onerror = () => {
              clearTimeout(timeout);
              reject(new Error('Load failed'));
            };
            img.src = fullPath;
          });

          // ✅ Ảnh load thành công
          loadedImages.push(fullPath);

          // 🏷️ Tạo tên hiển thị đẹp cho các ảnh
          const displayName = fileName === 'epic-mountain-canyon'
            ? '🏔️ Grand Canyon hùng vĩ (Pexels)'
            : fileName === 'crater-lake-mountains'
            ? '🌊 Crater Lake núi tuyết (Pexels)'
            : fileName.includes('lai-chau') || fileName.includes('AgribankLaiChau')
            ? `🏔️ Lai Châu ${fileName.includes('AgribankLaiChau') ? 'chính thức' : fileName.split('-').pop()}`
            : fileName.includes('nature-green-forest')
            ? '🌲 Rừng xanh HDR'
            : fileName.includes('nature-lake-forest')
            ? '🏞️ Hồ rừng HDR'
            : fileName.includes('nature-mountain-sunset')
            ? '🌅 Núi hoàng hôn HDR'
            : fileName.includes('background')
            ? `🖼️ Nền ${fileName.split('-').pop()}`
            : fileName.includes('nature')
            ? `🌿 Thiên nhiên ${fileName.split('-').pop()}`
            : fileName.includes('landscape')
            ? `🏞️ Phong cảnh ${fileName.split('-').pop()}`
            : fileName.includes('File_')
            ? `📄 Ảnh ${fileName.split('_').pop()}`
            : `🎨 ${fileName}`;

          loadedNames.push(displayName);
          // console.log(`✅ Tìm thấy: ${fullPath} -> ${displayName}`);

        } catch (error) {
          // Ảnh không tồn tại hoặc lỗi, bỏ qua im lặng
        }
      }
    }

    // console.log(`📊 Đã kiểm tra ${totalChecked} file possibilities, tìm thấy ${loadedImages.length} ảnh`);

    // 🎯 Xử lý kết quả
    if (loadedImages.length === 0) {
      // console.log('⚠️ Không tìm thấy ảnh nền local, sử dụng ảnh online');
      backgroundImages.value = [
        // � 2 ảnh nền hiện đại mới tuyệt đẹp cho Homepage (SVG vector)
        '/images/backgrounds/modern-tech-city-night.svg', // Thành phố công nghệ đêm xanh
        '/images/backgrounds/modern-financial-green.svg', // Tài chính xanh hiện đại
        // �🌌 2 ảnh vũ trụ đẹp lung linh local theo yêu cầu anh
        '/src/assets/earth-from-space-1.jpg', // Trái Đất từ vũ trụ 1 (HDR 2K)
        '/src/assets/earth-from-space-2.jpg', // Trái Đất từ vũ trụ 2 (HDR 2K)
        // 🌌 Backup ảnh vũ trụ online tuyệt đẹp HDR
        'https://images.unsplash.com/photo-1506318137071-a8e063b4bec0?ixlib=rb-4.0.3&auto=format&fit=crop&w=2893&q=80', // Vũ trụ sao kim cương
        'https://images.unsplash.com/photo-1544197150-b99a580bb7a8?ixlib=rb-4.0.3&auto=format&fit=crop&w=2940&q=80', // Galaxy spiral tím xanh
        'https://images.unsplash.com/photo-1506905925346-21bda4d32df4?ixlib=rb-4.0.3&auto=format&fit=crop&w=2070&q=80'
      ];
      backgroundNames.value = ['🏙️ Thành phố công nghệ', '💰 Tài chính hiện đại', '🌍 Trái Đất vũ trụ 1', '🌍 Trái Đất vũ trụ 2', '🌟 Vũ trụ kim cương', '🌀 Galaxy xoắn ốc', '🏔️ Núi tuyết'];
    } else {
      // 🔄 Sắp xếp ảnh theo thứ tự tên file
      const sortedData = loadedImages.map((img, index) => ({
        image: img,
        name: loadedNames[index],
        sortKey: img.toLowerCase()
      })).sort((a, b) => a.sortKey.localeCompare(b.sortKey));

      backgroundImages.value = sortedData.map(item => item.image);
      backgroundNames.value = sortedData.map(item => item.name);

      // Nếu có ít hơn 7 ảnh local, thêm ảnh online để đủ
      if (backgroundImages.value.length < 7) {
        const additionalImages = [
          // 🎨 2 ảnh nền hiện đại mới tuyệt đẹp cho Homepage (SVG vector)
          '/images/backgrounds/modern-tech-city-night.svg', // Thành phố công nghệ đêm xanh
          '/images/backgrounds/modern-financial-green.svg', // Tài chính xanh hiện đại
          // 🌌 2 ảnh vũ trụ đẹp lung linh local theo yêu cầu anh (ưu tiên)
          '/src/assets/earth-from-space-1.jpg', // Trái Đất từ vũ trụ 1 (HDR 2K)
          '/src/assets/earth-from-space-2.jpg', // Trái Đất từ vũ trụ 2 (HDR 2K)
          // 🌌 Thêm ảnh vũ trụ HDR tuyệt đẹp khác
          'https://images.unsplash.com/photo-1502134249126-9f3755a50d78?ixlib=rb-4.0.3&auto=format&fit=crop&w=2940&q=80', // Nebula tím hồng
          'https://images.unsplash.com/photo-1519904981063-b0cf448d479e?ixlib=rb-4.0.3&auto=format&fit=crop&w=2940&q=80', // Tinh vân xanh
          'https://images.unsplash.com/photo-1506905925346-21bda4d32df4?ixlib=rb-4.0.3&auto=format&fit=crop&w=2070&q=80',
          'https://images.unsplash.com/photo-1518837695005-2083093ee35b?ixlib=rb-4.0.3&auto=format&fit=crop&w=2070&q=80',
          'https://images.unsplash.com/photo-1465146344425-f00d5f5c8f07?ixlib=rb-4.0.3&auto=format&fit=crop&w=2070&q=80'
        ];
        const additionalNames = ['🏙️ Thành phố công nghệ', '💰 Tài chính hiện đại', '🌍 Trái Đất vũ trụ 1', '🌍 Trái Đất vũ trụ 2', '🌌 Nebula hồng', '💙 Tinh vân xanh', '🏔️ Núi tuyết', '🌅 Bình minh', '🌾 Cánh đồng'];

        const needed = Math.min(7 - backgroundImages.value.length, additionalImages.length);
        backgroundImages.value.push(...additionalImages.slice(0, needed));
        backgroundNames.value.push(...additionalNames.slice(0, needed));
      }

      console.log(`🎉 Đã load ${backgroundImages.value.length} ảnh nền (${loadedImages.length} local + ${backgroundImages.value.length - loadedImages.length} online)!`);
      console.log('📋 Danh sách ảnh:', backgroundNames.value);
    }

  } catch (error) {
    console.error('❌ Lỗi nghiêm trọng khi load ảnh nền:', error);
    // Fallback cuối cùng
    backgroundImages.value = [
      'https://images.unsplash.com/photo-1506905925346-21bda4d32df4?ixlib=rb-4.0.3&auto=format&fit=crop&w=2070&q=80'
    ];
    backgroundNames.value = ['🏔️ Ảnh mặc định'];
  }
};

// Preload images for smooth transitions
const preloadImages = () => {
  backgroundImages.value.forEach(src => {
    const img = new Image();
    img.src = src;
  });
};

// Smart background selection based on time of day
const getTimeBasedImageIndex = () => {
  const imageCount = backgroundImages.value.length;
  if (imageCount === 0) return 0;

  const hour = new Date().getHours();
  if (hour >= 6 && hour < 12) return 0;  // Sáng - Ảnh đầu tiên
  if (hour >= 12 && hour < 17) return Math.min(1, imageCount - 1); // Trưa - Ảnh thứ 2
  if (hour >= 17 && hour < 20) return Math.min(2, imageCount - 1); // Chiều - Ảnh thứ 3
  return Math.min(3, imageCount - 1); // Tối - Ảnh thứ 4 hoặc cuối cùng
};

const startBackgroundRotation = () => {
  // Set initial background based on time
  currentImageIndex.value = getTimeBasedImageIndex();

  backgroundInterval = setInterval(() => {
    currentImageIndex.value = (currentImageIndex.value + 1) % backgroundImages.value.length;
  }, 12000); // Change image every 12 seconds
};

onMounted(async () => {
  // Load ảnh nền trước tiên
  await loadBackgroundImages();
  preloadImages();
  startBackgroundRotation();

  // Add document click listener for dropdown auto-hide
  document.addEventListener('click', handleDocumentClick);

  // Watch for route changes to hide menus
  router.afterEach(handleRouteChange);
});

onUnmounted(() => {
  if (backgroundInterval) {
    clearInterval(backgroundInterval);
  }

  // Remove event listeners
  document.removeEventListener('click', handleDocumentClick);

  // Clear any pending timeouts
  hideAllMenus();
});
</script>

<style>
/* 🌙 CSS Variables cho Dark/Light Theme */
:root {
  /* Light Theme (Default) */
  --primary-color: #007bff;
  --primary-hover: #0056b3;
  --secondary-color: #6c757d;
  --success-color: #28a745;
  --danger-color: #dc3545;
  --warning-color: #ffc107;
  --info-color: #17a2b8;

  /* Background Colors */
  --bg-primary: #ffffff;
  --bg-secondary: #f8f9fa;
  --bg-tertiary: #e9ecef;
  --bg-hover: #f5f5f5;
  --bg-card: #ffffff;

  /* Text Colors */
  --text-primary: #212529;
  --text-secondary: #6c757d;
  --text-muted: #adb5bd;
  --text-inverse: #ffffff;

  /* Border Colors */
  --border-color: #dee2e6;
  --border-light: #e9ecef;
  --border-dark: #adb5bd;

  /* Shadow */
  --shadow-light: rgba(0, 0, 0, 0.1);
  --shadow-medium: rgba(0, 0, 0, 0.15);
  --shadow-heavy: rgba(0, 0, 0, 0.3);

  /* Agribank Colors */
  --agribank-green: #00b14f;
  --agribank-blue: #0066cc;
  --agribank-orange: #ff6600;
}

/* 🌙 Dark Theme Variables */
.dark-theme {
  --primary-color: #4a9eff;
  --primary-hover: #357abd;
  --secondary-color: #868e96;
  --success-color: #51cf66;
  --danger-color: #ff6b6b;
  --warning-color: #ffd43b;
  --info-color: #339af0;

  /* Background Colors */
  --bg-primary: #1a1a1a;
  --bg-secondary: #2d2d2d;
  --bg-tertiary: #404040;
  --bg-hover: #3a3a3a;
  --bg-card: #262626;

  /* Text Colors */
  --text-primary: #f8f9fa;
  --text-secondary: #adb5bd;
  --text-muted: #6c757d;
  --text-inverse: #212529;

  /* Border Colors */
  --border-color: #404040;
  --border-light: #353535;
  --border-dark: #555555;

  /* Shadow */
  --shadow-light: rgba(0, 0, 0, 0.3);
  --shadow-medium: rgba(0, 0, 0, 0.4);
  --shadow-heavy: rgba(0, 0, 0, 0.6);

  /* Agribank Colors - Darker variants */
  --agribank-green: #00d662;
  --agribank-blue: #4a9eff;
  --agribank-orange: #ff8533;
}

/* 🌟 Transparent Theme - Hoàn toàn ẩn overlay để thấy ảnh wallpaper */
.transparent-theme .background-overlay {
  display: none !important; /* Ẩn hoàn toàn thay vì opacity */
}

/* Thêm hiệu ứng text shadow mạnh hơn cho chế độ trong suốt */
.transparent-theme .hero-title {
  text-shadow:
    0 3px 6px rgba(0, 0, 0, 0.8),
    0 0 15px rgba(255, 255, 255, 1),
    0 0 30px rgba(255, 255, 255, 0.8) !important;
}

.transparent-theme .hero-subtitle {
  text-shadow:
    0 2px 4px rgba(0, 0, 0, 0.8),
    0 0 10px rgba(255, 255, 255, 1),
    0 0 20px rgba(255, 255, 255, 0.7) !important;
}

/* Ẩn content-container background trong chế độ trong suốt */
.transparent-theme .content-container {
  background: transparent !important;
  backdrop-filter: none !important;
  box-shadow: none !important;
  border: none !important;
}

/* Global Styles */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body, html {
  height: 100%;
  overflow-x: hidden;
}

#app {
  /* 🇻🇳 Font stack tối ưu cho tiếng Việt */
  font-family: 'Roboto', 'Segoe UI', 'Helvetica Neue', 'Arial', 'Noto Sans', 'Liberation Sans', sans-serif, 'Apple Color Emoji', 'Segoe UI Emoji', 'Noto Color Emoji';
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2c3e50;
  height: 100vh;
  width: 100vw;
  /* 🇻🇳 Đảm bảo hỗ trợ Unicode tiếng Việt */
  text-rendering: optimizeLegibility;
}

/* App Container */
.app-container {
  position: relative;
  width: 100%;
  height: 100vh;
  overflow: hidden;
}

/* Dynamic Background */
.dynamic-background {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: -1;
}

.background-slide {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  opacity: 0;
  transition: opacity 2s ease-in-out;
}

.background-slide.active {
  opacity: 1;
}

.background-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: linear-gradient(
    135deg,
    rgba(0, 0, 0, 0.4) 0%,
    rgba(0, 0, 0, 0.2) 50%,
    rgba(0, 0, 0, 0.3) 100%
  );
  z-index: 1;
}

/* Background Indicators */
.background-indicators {
  position: fixed;
  bottom: 30px;
  right: 30px;
  display: flex;
  gap: 8px;
  z-index: 15;
}

.indicator {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.5);
  cursor: pointer;
  transition: all 0.3s ease;
  border: 1px solid rgba(255, 255, 255, 0.8);
}

.indicator.active {
  background: rgba(139, 21, 56, 0.9);
  transform: scale(1.3);
  box-shadow: 0 0 8px rgba(139, 21, 56, 0.5);
}

.indicator:hover {
  background: rgba(255, 255, 255, 0.8);
  transform: scale(1.1);
}

/* Background name display */
.background-name {
  position: fixed;
  bottom: 60px;
  right: 30px;
  background: rgba(0, 0, 0, 0.7);
  color: white;
  padding: 8px 16px;
  border-radius: 20px;
  font-size: 14px;
  font-weight: 500;
  z-index: 15;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  transition: opacity 0.3s ease;
}

/* Main Content */
.main-content {
  position: relative;
  z-index: 10;
  height: 100vh;
  display: flex;
  flex-direction: column;
}

/* Navigation */
.main-nav {
  display: flex;
  align-items: center;
  background: linear-gradient(135deg, #8B1538, #A91B47);
  backdrop-filter: blur(10px);
  padding: 0 24px;
  height: 60px;
  gap: 18px;
  box-shadow: 0 2px 20px rgba(0, 0, 0, 0.1);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  position: relative;
  z-index: 100;
}

.main-nav .nav-logo {
  font-weight: bold;
  color: #fff;
  font-size: 22px;
  margin-right: 24px;
  text-decoration: none;
  text-shadow: 0 1px 3px rgba(0, 0, 0, 0.3);
  transition: transform 0.3s ease;
  display: flex;
  align-items: center;
}

.nav-logo-img {
  height: 40px;
  width: auto;
  filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.3));
  transition: transform 0.3s ease;
}

.nav-logo-img:hover {
  transform: scale(1.05);
}

.main-nav .nav-logo:hover {
  transform: scale(1.05);
}

.main-nav a,
.main-nav .router-link-active {
  color: rgba(255, 255, 255, 0.9);
  text-decoration: none;
  font-weight: 700;
  font-size: 16px;
  padding: 10px 18px;
  border-radius: 8px;
  transition: all 0.3s ease;
  backdrop-filter: blur(5px);
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
}

.main-nav .router-link-exact-active {
  color: #8B1538;
  background: rgba(139, 21, 56, 0.15);
  transform: translateY(-1px);
  box-shadow: 0 2px 8px rgba(139, 21, 56, 0.4);
}

.main-nav .nav-spacer {
  flex: 1;
}

/* 🎛️ Compact Navigation Controls - Thu nhỏ theme switcher và logout */
.nav-compact-controls {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-left: auto;
}

/* 🚪 Compact Logout Button - Single Line */
.logout-btn {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.1) !important;
  color: rgba(255, 255, 255, 0.9) !important;
  text-decoration: none !important;
  padding: 8px 12px !important;
  border-radius: 8px !important;
  transition: all 0.3s ease !important;
  backdrop-filter: blur(5px) !important;
  min-width: 85px;
  height: 46px;
  font-size: 12px !important;
  font-weight: 600 !important;
  border: 1px solid rgba(255, 255, 255, 0.2) !important;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1) !important;
  gap: 6px;
}

.logout-btn:hover {
  background: rgba(255, 255, 255, 0.2) !important;
  transform: translateY(-1px) !important;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2) !important;
  color: #fff !important;
}

.logout-icon {
  font-size: 16px;
  display: block;
}

.logout-text {
  font-size: 12px !important;
  line-height: 1.2 !important;
  text-align: center;
  font-weight: 600 !important;
  white-space: nowrap;
}

/* 🎨 Compact Theme Switcher - Icon Only */
.nav-compact-controls .theme-switcher {
  padding: 6px 8px !important;
  min-width: 46px !important;
  height: 46px !important;
  display: flex !important;
  flex-direction: column !important;
  align-items: center !important;
  justify-content: center !important;
  font-size: 10px !important;
  gap: 2px !important;
  border-radius: 8px !important;
  background: rgba(255, 255, 255, 0.1) !important;
  border: 1px solid rgba(255, 255, 255, 0.2) !important;
  backdrop-filter: blur(5px) !important;
  transition: all 0.3s ease !important;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1) !important;
}

.nav-compact-controls .theme-switcher:hover {
  background: rgba(255, 255, 255, 0.2) !important;
  transform: translateY(-1px) !important;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2) !important;
}

.nav-compact-controls .theme-switcher .theme-icon {
  font-size: 18px !important;
  margin-bottom: 0px !important;
}

.nav-compact-controls .theme-switcher .theme-label {
  display: none !important; /* Ẩn hoàn toàn text label */
}

/* 📱 Mobile Responsive cho Compact Controls */
@media (max-width: 768px) {
  .nav-compact-controls {
    gap: 6px;
  }

  .logout-btn {
    min-width: 75px !important;
    height: 40px !important;
    padding: 6px 10px !important;
    font-size: 11px !important;
    gap: 4px;
  }

  .nav-compact-controls .theme-switcher {
    min-width: 40px !important;
    height: 40px !important;
    padding: 4px 6px !important;
  }

  .logout-icon,
  .nav-compact-controls .theme-switcher .theme-icon {
    font-size: 14px !important;
  }

  .indicator {
    width: 6px;
    height: 6px;
  }
}

/* Navigation Dropdown Styles */
.nav-dropdown {
  position: relative;
  display: inline-block;
  z-index: 200;
}

.nav-dropdown-trigger {
  display: flex;
  align-items: center;
  gap: 8px;
  color: rgba(255, 255, 255, 0.9);
  text-decoration: none;
  font-weight: 700;
  font-size: 16px;
  padding: 10px 18px;
  border-radius: 8px;
  transition: all 0.3s ease;
  backdrop-filter: blur(5px);
  cursor: pointer;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
}

.nav-dropdown-trigger.active {
  color: #8B1538;
  background: rgba(139, 21, 56, 0.15);
  transform: translateY(-1px);
  box-shadow: 0 2px 8px rgba(139, 21, 56, 0.4);
}

.nav-dropdown-trigger:hover {
  color: #8B1538;
  background: rgba(255, 255, 255, 0.1);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.dropdown-arrow {
  transition: transform 0.3s ease;
  color: currentColor;
}

.dropdown-arrow.rotated {
  transform: rotate(180deg);
}

.nav-dropdown-menu {
  position: absolute;
  top: 100%;
  left: 0;
  min-width: 320px;
  background: rgba(255, 255, 255, 0.98);
  backdrop-filter: blur(20px);
  border-radius: 12px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.15);
  border: 1px solid rgba(255, 255, 255, 0.3);
  opacity: 0;
  visibility: hidden;
  transform: translateY(-10px);
  transition: all 0.3s ease;
  z-index: 9999;
  overflow: hidden;
  margin-top: 8px;
}

.nav-dropdown-menu.show {
  opacity: 1;
  visibility: visible;
  transform: translateY(0);
}

.dropdown-item {
  display: flex;
  align-items: center;
  gap: 15px;
  padding: 15px 24px;
  color: #2c3e50;
  text-decoration: none;
  font-weight: 700;
  font-size: 16px;
  transition: all 0.3s ease;
  border-bottom: 1px solid rgba(0, 0, 0, 0.05);
}

.dropdown-item:last-child {
  border-bottom: none;
}

.dropdown-item:hover {
  background: linear-gradient(135deg, #8B1538, #A91B47);
  color: white;
  transform: translateX(5px);
}

.dropdown-item.router-link-exact-active {
  background: linear-gradient(135deg, #8B1538, #6B1028);
  color: white;
}

.item-icon {
  font-size: 22px;
  min-width: 28px;
  text-align: center;
}

/* Responsive dropdown */
@media (max-width: 768px) {
  .nav-dropdown-menu {
    min-width: 250px;
    position: fixed;
    top: 60px;
    left: 10px;
    right: 10px;
    min-width: auto;
  }

  .dropdown-item {
    padding: 10px 16px;
  }
}

/* Router View Container */
.content-container {
  flex: 1;
  overflow-y: auto;
  background: var(--bg-card);
  backdrop-filter: blur(10px);
  margin: 20px;
  border-radius: 12px;
  box-shadow: 0 8px 32px var(--shadow-medium);
  border: 1px solid var(--border-light);
  padding: 30px;
  text-align: left;
  color: var(--text-primary);
  margin-bottom: 60px; /* Khoảng trống cho footer */
  min-height: calc(100vh - 180px); /* Điều chỉnh để có chỗ cho footer */
}

.content-container h1 {
  color: var(--primary-color);
  margin-bottom: 24px;
  font-size: 28px;
  font-weight: 700;
  text-shadow: 0 1px 3px var(--shadow-light);
}

.content-container h2 {
  color: #A91B47;
  margin-bottom: 18px;
  font-size: 22px;
  font-weight: 600;
}

/* Enhance buttons and forms for the new design */
.action-button, .edit-btn, .delete-btn, .save-btn, .cancel-btn {
  padding: 10px 20px;
  border: none;
  border-radius: 8px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
  backdrop-filter: blur(5px);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.action-button {
  background: linear-gradient(135deg, #8B1538, #A91B47);
  color: white;
  margin-bottom: 20px;
}

.action-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(139, 21, 56, 0.4);
}

.edit-btn {
  background: linear-gradient(135deg, #8B1538, #A91B47);
  color: white;
  margin-right: 8px;
}

.delete-btn {
  background: linear-gradient(135deg, #DC143C, #B22222);
  color: white;
}

.save-btn {
  background: linear-gradient(135deg, #8B1538, #A91B47);
  color: white;
  margin-right: 8px;
}

.cancel-btn {
  background: linear-gradient(135deg, #8c8c8c, #595959);
  color: white;
}

/* List items styling */
.list-item {
  background: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(5px);
  border-radius: 8px;
  padding: 16px;
  margin-bottom: 12px;
  border: 1px solid rgba(255, 255, 255, 0.3);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  transition: transform 0.2s ease;
}

.list-item:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
}

/* Form styling */
.form-section {
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(10px);
  border-radius: 12px;
  padding: 24px;
  margin-bottom: 24px;
  border: 1px solid rgba(255, 255, 255, 0.3);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
}

input, textarea, select {
  background: rgba(255, 255, 255, 0.9);
  border: 1px solid rgba(0, 0, 0, 0.1);
  border-radius: 6px;
  padding: 10px;
  transition: all 0.3s ease;
}

input:focus, textarea:focus, select:focus {
  outline: none;
  border-color: #8B1538;
  box-shadow: 0 0 0 3px rgba(139, 21, 56, 0.1);
}

/* Error and success messages */
.error-message {
  background: rgba(220, 20, 60, 0.1);
  border: 1px solid rgba(220, 20, 60, 0.3);
  backdrop-filter: blur(5px);
  border-radius: 8px;
  padding: 12px;
  color: #DC143C;
  margin-bottom: 16px;
}

.success-message {
  background: rgba(139, 21, 56, 0.1);
  border: 1px solid rgba(139, 21, 56, 0.3);
  backdrop-filter: blur(5px);
  border-radius: 8px;
  padding: 12px;
  color: #8B1538;
  margin-bottom: 16px;
}

/* Responsive Design */
@media (max-width: 768px) {
  .main-nav {
    padding: 0 12px;
    gap: 8px;
    height: 56px;
    flex-wrap: wrap;
  }

  .main-nav .nav-logo {
    font-size: 18px;
    margin-right: 12px;
  }

  .nav-logo-img {
    height: 35px;
  }

  .main-nav a {
    padding: 6px 12px;
    font-size: 14px;
  }

  .main-content > .content-container {
    margin: 10px;
    padding: 20px;
  }

  .background-indicators {
    bottom: 20px;
    right: 20px;
  }

  .background-name {
    bottom: 50px;
    right: 20px;
    font-size: 12px;
    padding: 6px 12px;
  }

  .indicator {
    width: 6px;
    height: 6px;
  }

  .content-container h1 {
    font-size: 24px;
  }
}

@media (max-width: 480px) {
  .main-nav {
    height: auto;
    padding: 8px;
    min-height: 50px;
  }

  .nav-logo-img {
    height: 30px;
  }

  .main-nav a {
    padding: 4px 8px;
    font-size: 12px;
  }

  .content-container {
    padding: 16px;
  }

  .content-container h1 {
    font-size: 20px;
  }

  .background-indicators {
    bottom: 15px;
    right: 15px;
    gap: 6px;
  }

  .background-name {
    bottom: 40px;
    right: 15px;
    font-size: 11px;
    padding: 4px 8px;
  }
}

/* Animation for page transitions */
.router-view {
  transition: all 0.3s ease;
}

/* Fade in animation for content */
.content-container {
  animation: fadeInUp 0.6s ease-out;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Pulse animation for loading states */
@keyframes pulse {
  0% {
    opacity: 1;
  }
  50% {
    opacity: 0.7;
  }
  100% {
    opacity: 1;
  }
}

.action-button:disabled {
  animation: pulse 1.5s infinite;
  cursor: not-allowed;
}

/* Smooth transitions for background changes */
.background-slide {
  transition: opacity 3s ease-in-out;
}

/* Loading spinner for initial load */
@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.loading-spinner {
  border: 3px solid rgba(255, 255, 255, 0.3);
  border-radius: 50%;
  border-top: 3px solid #8B1538;
  width: 20px;
  height: 20px;
  animation: spin 1s linear infinite;
  display: inline-block;
  margin-left: 8px;
}

/* Custom scrollbar */
::-webkit-scrollbar {
  width: 8px;
}

::-webkit-scrollbar-track {
  background: rgba(255, 255, 255, 0.1);
  border-radius: 4px;
}

::-webkit-scrollbar-thumb {
  background: rgba(139, 21, 56, 0.6);
  border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
  background: rgba(139, 21, 56, 0.8);
}
</style>
