import { createApp } from "vue";
import { createPinia } from "pinia"; // 1. Import createPinia
import App from "./App.vue";
import router from "./router";
import 'vue3-toastify/dist/index.css';
import Vue3Toastify from 'vue3-toastify';
// Import Element Plus
import ElementPlus from 'element-plus';
import 'element-plus/dist/index.css';
import * as ElementPlusIconsVue from '@element-plus/icons-vue';
// Import Agribank theme CSS
import './assets/css/agribank-theme.css';
// Import theme store để khởi tạo theme
import { useThemeStore } from './stores/themeStore';
// Import offline store để khởi tạo offline functionality
import { useOfflineStore } from './stores/offlineStore';
// Import offline API service
import offlineApi from './services/offlineApi';

const app = createApp(App);
const pinia = createPinia();

app.use(pinia);
app.use(router);
app.use(ElementPlus);

// Register Element Plus icons
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component);
}

app.use(Vue3Toastify, { 
  autoClose: 2500, 
  position: 'top-right',
  hideProgressBar: false,
  closeOnClick: true,
  pauseOnHover: true,
  draggable: true
});

app.mount("#app");

// Khởi tạo theme sau khi app đã mount
const themeStore = useThemeStore();
themeStore.initTheme();
themeStore.setupSystemThemeWatcher();

// Khởi tạo offline store và PWA functionality
const offlineStore = useOfflineStore();
offlineStore.init();

// Khởi tạo offline API service
offlineApi.init();
