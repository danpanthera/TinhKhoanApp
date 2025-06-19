<template>
  <div class="theme-switcher">
    <button 
      @click="toggleTheme" 
      class="theme-btn"
      :title="themeStore.themeLabel()"
      aria-label="Chuyển đổi chế độ giao diện"
    >
      <i :class="themeStore.themeIcon()" class="theme-icon"></i>
      <span class="theme-label">{{ themeStore.themeLabel() }}</span>
    </button>
  </div>
</template>

<script setup>
import { useThemeStore } from '@/stores/themeStore';

// Store
const themeStore = useThemeStore();

// Methods
const toggleTheme = () => {
  themeStore.toggleTheme();
};
</script>

<style scoped>
.theme-switcher {
  display: flex;
  align-items: center;
}

.theme-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 12px;
  background: var(--bg-secondary);
  border: 1px solid var(--border-color);
  border-radius: 8px;
  color: var(--text-primary);
  cursor: pointer;
  transition: all 0.3s ease;
  font-size: 14px;
  font-weight: 500;
}

.theme-btn:hover {
  background: var(--bg-hover);
  border-color: var(--primary-color);
  transform: translateY(-1px);
  box-shadow: 0 2px 8px var(--shadow-light);
}

.theme-btn:active {
  transform: translateY(0);
}

.theme-btn i {
  font-size: 16px;
  transition: transform 0.3s ease;
}

.theme-btn:hover i {
  transform: rotate(15deg);
}

.theme-text {
  font-size: 13px;
  font-weight: 500;
}

/* Responsive */
@media (max-width: 768px) {
  .theme-text {
    display: none;
  }
  
  .theme-btn {
    padding: 10px;
    border-radius: 50%;
    aspect-ratio: 1;
  }
}
</style>
