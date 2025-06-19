<template>
  <div v-if="showInstallPrompt" class="pwa-install-banner">
    <div class="install-content">
      <div class="install-icon">
        <img src="/Logo-Agribank-2.png" alt="Agribank Logo" class="app-logo" />
      </div>
      <div class="install-text">
        <h4>Cài đặt ứng dụng</h4>
        <p>Cài đặt Agribank TK để truy cập nhanh và sử dụng offline</p>
      </div>
      <div class="install-actions">
        <button @click="installApp" class="install-btn">
          <i class="fas fa-download"></i>
          Cài đặt
        </button>
        <button @click="dismissPrompt" class="dismiss-btn">
          <i class="fas fa-times"></i>
        </button>
      </div>
    </div>
  </div>

  <!-- PWA Status Indicator -->
  <div v-if="isStandalone || isPWAInstalled" class="pwa-status">
    <i class="fas fa-mobile-alt"></i>
    <span>Chế độ PWA</span>
  </div>

  <!-- Update Available Notification -->
  <div v-if="updateAvailable" class="pwa-update-banner">
    <div class="update-content">
      <div class="update-icon">
        <i class="fas fa-sync-alt"></i>
      </div>
      <div class="update-text">
        <h4>Cập nhật có sẵn</h4>
        <p>Phiên bản mới của ứng dụng đã sẵn sàng</p>
      </div>
      <div class="update-actions">
        <button @click="updateApp" class="update-btn">
          <i class="fas fa-refresh"></i>
          Cập nhật
        </button>
        <button @click="dismissUpdate" class="dismiss-btn">
          <i class="fas fa-times"></i>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue';
import { toast } from 'vue3-toastify';

// Reactive states
const showInstallPrompt = ref(false);
const updateAvailable = ref(false);
const isStandalone = ref(false);
const isPWAInstalled = ref(false);
const deferredPrompt = ref(null);
let registration = null;

// Phát hiện standalone mode
const checkStandaloneMode = () => {
  isStandalone.value = window.matchMedia('(display-mode: standalone)').matches ||
                      window.navigator.standalone ||
                      document.referrer.includes('android-app://');
};

// Phát hiện PWA đã được cài đặt
const checkPWAInstalled = () => {
  isPWAInstalled.value = isStandalone.value || 
                        localStorage.getItem('pwa-installed') === 'true';
};

// Xử lý sự kiện beforeinstallprompt
const handleBeforeInstallPrompt = (e) => {
  console.log('🔧 PWA: Before install prompt triggered');
  e.preventDefault();
  deferredPrompt.value = e;
  
  // Chỉ hiển thị prompt nếu chưa cài đặt PWA
  if (!isPWAInstalled.value && !isStandalone.value) {
    showInstallPrompt.value = true;
  }
};

// Cài đặt ứng dụng
const installApp = async () => {
  if (!deferredPrompt.value) {
    toast.warning('Trình duyệt không hỗ trợ cài đặt PWA');
    return;
  }

  try {
    // Hiển thị prompt cài đặt
    deferredPrompt.value.prompt();
    
    // Chờ người dùng phản hồi
    const { outcome } = await deferredPrompt.value.userChoice;
    
    if (outcome === 'accepted') {
      console.log('🔧 PWA: User accepted the install prompt');
      toast.success('Đang cài đặt ứng dụng...');
      localStorage.setItem('pwa-installed', 'true');
      isPWAInstalled.value = true;
    } else {
      console.log('🔧 PWA: User dismissed the install prompt');
      toast.info('Đã hủy cài đặt ứng dụng');
    }
    
    // Reset prompt
    deferredPrompt.value = null;
    showInstallPrompt.value = false;
  } catch (error) {
    console.error('🔧 PWA: Error during installation:', error);
    toast.error('Lỗi khi cài đặt ứng dụng');
  }
};

// Từ chối prompt cài đặt
const dismissPrompt = () => {
  showInstallPrompt.value = false;
  // Ghi nhớ người dùng đã từ chối (có thể hiển thị lại sau 30 ngày)
  localStorage.setItem('pwa-prompt-dismissed', Date.now().toString());
};

// Cập nhật ứng dụng
const updateApp = async () => {
  if (registration && registration.waiting) {
    // Gửi message để skip waiting
    registration.waiting.postMessage({ type: 'SKIP_WAITING' });
    toast.success('Đang cập nhật ứng dụng...');
    
    // Reload trang sau khi cập nhật
    setTimeout(() => {
      window.location.reload();
    }, 1000);
  }
  updateAvailable.value = false;
};

// Từ chối cập nhật
const dismissUpdate = () => {
  updateAvailable.value = false;
};

// Xử lý Service Worker updates
const handleSWUpdate = () => {
  if ('serviceWorker' in navigator) {
    navigator.serviceWorker.addEventListener('controllerchange', () => {
      console.log('🔧 PWA: Service Worker updated');
      toast.success('Ứng dụng đã được cập nhật thành công!');
    });

    navigator.serviceWorker.ready.then((reg) => {
      registration = reg;
      
      // Kiểm tra cập nhật định kỳ
      setInterval(() => {
        reg.update();
      }, 60000); // Kiểm tra mỗi phút

      // Lắng nghe sự kiện cập nhật
      reg.addEventListener('updatefound', () => {
        const newWorker = reg.installing;
        if (newWorker) {
          newWorker.addEventListener('statechange', () => {
            if (newWorker.state === 'installed' && navigator.serviceWorker.controller) {
              console.log('🔧 PWA: New content available');
              updateAvailable.value = true;
            }
          });
        }
      });
    });
  }
};

// Kiểm tra nếu nên hiển thị install prompt
const shouldShowInstallPrompt = () => {
  const dismissed = localStorage.getItem('pwa-prompt-dismissed');
  if (dismissed) {
    const dismissedTime = parseInt(dismissed);
    const thirtyDays = 30 * 24 * 60 * 60 * 1000; // 30 ngày
    if (Date.now() - dismissedTime < thirtyDays) {
      return false;
    }
  }
  return !isPWAInstalled.value && !isStandalone.value;
};

onMounted(() => {
  checkStandaloneMode();
  checkPWAInstalled();
  handleSWUpdate();

  // Lắng nghe sự kiện beforeinstallprompt
  window.addEventListener('beforeinstallprompt', handleBeforeInstallPrompt);

  // Lắng nghe sự kiện appinstalled
  window.addEventListener('appinstalled', () => {
    console.log('🔧 PWA: App was installed');
    toast.success('Ứng dụng đã được cài đặt thành công!');
    localStorage.setItem('pwa-installed', 'true');
    isPWAInstalled.value = true;
    showInstallPrompt.value = false;
  });

  // Delay một chút trước khi hiển thị prompt
  setTimeout(() => {
    if (shouldShowInstallPrompt() && deferredPrompt.value) {
      showInstallPrompt.value = true;
    }
  }, 3000); // Hiển thị sau 3 giây
});

onUnmounted(() => {
  window.removeEventListener('beforeinstallprompt', handleBeforeInstallPrompt);
});
</script>

<style scoped>
/* PWA Install Banner */
.pwa-install-banner {
  position: fixed;
  bottom: 20px;
  left: 20px;
  right: 20px;
  background: linear-gradient(135deg, #8B1538, #A91B47);
  color: white;
  border-radius: 12px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
  z-index: 9999;
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.1);
  animation: slideUp 0.5s ease-out;
}

@keyframes slideUp {
  from {
    transform: translateY(100%);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.install-content {
  display: flex;
  align-items: center;
  padding: 16px 20px;
  gap: 16px;
}

.install-icon {
  flex-shrink: 0;
}

.app-logo {
  width: 48px;
  height: 48px;
  border-radius: 8px;
  object-fit: contain;
  background: rgba(255, 255, 255, 0.1);
  padding: 4px;
}

.install-text {
  flex: 1;
}

.install-text h4 {
  margin: 0 0 4px 0;
  font-size: 1.1rem;
  font-weight: 600;
}

.install-text p {
  margin: 0;
  font-size: 0.9rem;
  opacity: 0.9;
}

.install-actions {
  display: flex;
  gap: 8px;
  align-items: center;
}

.install-btn {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: 1px solid rgba(255, 255, 255, 0.3);
  padding: 10px 16px;
  border-radius: 8px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 8px;
}

.install-btn:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: translateY(-1px);
}

.dismiss-btn {
  background: none;
  color: white;
  border: none;
  padding: 8px;
  border-radius: 50%;
  cursor: pointer;
  transition: all 0.3s ease;
  opacity: 0.7;
}

.dismiss-btn:hover {
  background: rgba(255, 255, 255, 0.2);
  opacity: 1;
}

/* PWA Status Indicator */
.pwa-status {
  position: fixed;
  top: 80px;
  right: 20px;
  background: linear-gradient(135deg, #28a745, #20c997);
  color: white;
  padding: 8px 12px;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 500;
  z-index: 1000;
  display: flex;
  align-items: center;
  gap: 6px;
  box-shadow: 0 4px 16px rgba(40, 167, 69, 0.3);
}

/* PWA Update Banner */
.pwa-update-banner {
  position: fixed;
  top: 20px;
  left: 20px;
  right: 20px;
  background: linear-gradient(135deg, #ffc107, #fd7e14);
  color: #212529;
  border-radius: 12px;
  box-shadow: 0 8px 32px rgba(255, 193, 7, 0.3);
  z-index: 9999;
  animation: slideDown 0.5s ease-out;
}

@keyframes slideDown {
  from {
    transform: translateY(-100%);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.update-content {
  display: flex;
  align-items: center;
  padding: 16px 20px;
  gap: 16px;
}

.update-icon {
  flex-shrink: 0;
  font-size: 1.5rem;
  color: #212529;
}

.update-text {
  flex: 1;
}

.update-text h4 {
  margin: 0 0 4px 0;
  font-size: 1.1rem;
  font-weight: 600;
  color: #212529;
}

.update-text p {
  margin: 0;
  font-size: 0.9rem;
  color: #495057;
}

.update-actions {
  display: flex;
  gap: 8px;
  align-items: center;
}

.update-btn {
  background: #212529;
  color: white;
  border: none;
  padding: 10px 16px;
  border-radius: 8px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 8px;
}

.update-btn:hover {
  background: #495057;
  transform: translateY(-1px);
}

/* Responsive Design */
@media (max-width: 768px) {
  .pwa-install-banner,
  .pwa-update-banner {
    left: 10px;
    right: 10px;
  }
  
  .install-content,
  .update-content {
    padding: 12px 16px;
    gap: 12px;
  }
  
  .install-text h4,
  .update-text h4 {
    font-size: 1rem;
  }
  
  .install-text p,
  .update-text p {
    font-size: 0.85rem;
  }
  
  .app-logo {
    width: 40px;
    height: 40px;
  }
  
  .pwa-status {
    top: 70px;
    right: 10px;
    font-size: 0.8rem;
    padding: 6px 10px;
  }
}

@media (max-width: 480px) {
  .install-content,
  .update-content {
    flex-direction: column;
    text-align: center;
    gap: 12px;
  }
  
  .install-actions,
  .update-actions {
    justify-content: center;
  }
}
</style>
