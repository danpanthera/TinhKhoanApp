// Store quản lý theme cho ứng dụng
import { defineStore } from 'pinia';
import { ref, watch } from 'vue';

export const useThemeStore = defineStore('theme', () => {
  // State
  const isDarkMode = ref(false);
  const currentTheme = ref('light');

  // Khởi tạo theme từ localStorage hoặc system preference
  const initTheme = () => {
    const savedTheme = localStorage.getItem('tinhkhoan-theme');
    
    if (savedTheme) {
      // Sử dụng theme đã lưu
      currentTheme.value = savedTheme;
      isDarkMode.value = savedTheme === 'dark';
    } else {
      // Kiểm tra system preference
      const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
      currentTheme.value = prefersDark ? 'dark' : 'light';
      isDarkMode.value = prefersDark;
    }
    
    applyTheme();
  };

  // Áp dụng theme vào DOM
  const applyTheme = () => {
    const root = document.documentElement;
    
    if (isDarkMode.value) {
      root.classList.add('dark-theme');
      root.classList.remove('light-theme', 'transparent-theme');
    } else {
      root.classList.add('transparent-theme');
      root.classList.remove('dark-theme', 'light-theme');
    }
    
    // Cập nhật meta theme-color cho mobile
    const metaThemeColor = document.querySelector('meta[name="theme-color"]');
    if (metaThemeColor) {
      metaThemeColor.setAttribute('content', isDarkMode.value ? '#1a1a1a' : 'transparent');
    }
  };

  // Toggle theme
  const toggleTheme = () => {
    isDarkMode.value = !isDarkMode.value;
    currentTheme.value = isDarkMode.value ? 'dark' : 'light';
    
    // Lưu vào localStorage
    localStorage.setItem('tinhkhoan-theme', currentTheme.value);
    
    applyTheme();
  };

  // Set specific theme
  const setTheme = (theme) => {
    if (theme === 'dark' || theme === 'light') {
      currentTheme.value = theme;
      isDarkMode.value = theme === 'dark';
      localStorage.setItem('tinhkhoan-theme', theme);
      applyTheme();
    }
  };

  // Watch for system theme changes
  const setupSystemThemeWatcher = () => {
    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
    
    mediaQuery.addEventListener('change', (e) => {
      // Chỉ tự động thay đổi nếu user chưa set theme riêng
      const savedTheme = localStorage.getItem('tinhkhoan-theme');
      if (!savedTheme) {
        isDarkMode.value = e.matches;
        currentTheme.value = e.matches ? 'dark' : 'light';
        applyTheme();
      }
    });
  };

  // Computed properties
  const themeIcon = () => isDarkMode.value ? 'fas fa-sun' : 'fas fa-eye';
  const themeLabel = () => isDarkMode.value ? 'Chế độ thường' : 'Chế độ trong suốt';

  return {
    // State
    isDarkMode,
    currentTheme,
    
    // Actions
    initTheme,
    toggleTheme,
    setTheme,
    setupSystemThemeWatcher,
    
    // Getters
    themeIcon,
    themeLabel
  };
});
