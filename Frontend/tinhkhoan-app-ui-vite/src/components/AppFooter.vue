<template>
  <footer class="app-footer">
    <div class="footer-content">
      <!-- Thông tin user và session -->
      <div class="user-session-info">
        <div class="user-info">
          <span class="user-icon">👤</span>
          <span class="user-name">{{ currentUser || 'Admin' }}</span>
        </div>
        <div class="divider">|</div>
        <div class="login-info">
          <span class="login-icon">🕐</span>
          <span class="login-time">{{ formatLoginTime() }}</span>
        </div>
        <div class="divider">|</div>
        <div class="ip-info">
          <span class="ip-icon">🌐</span>
          <span class="ip-address">{{ userIP || 'Đang tải...' }}</span>
        </div>
      </div>
    </div>
  </footer>
</template>

<script setup>
import { ref, onMounted } from 'vue';

// Reactive data
const currentUser = ref('');
const loginTime = ref(new Date());
const userIP = ref('');

// Lấy thông tin user từ localStorage hoặc session
const getCurrentUser = () => {
  try {
    const userData = localStorage.getItem('user');
    if (userData) {
      const user = JSON.parse(userData);
      return user.username || user.fullName || 'Admin';
    }
    return localStorage.getItem('username') || 'Admin';
  } catch (error) {
    console.warn('Không thể lấy thông tin user:', error);
    return 'Admin';
  }
};

// Lấy thời gian login từ localStorage hoặc sử dụng thời gian hiện tại
const getLoginTime = () => {
  try {
    const storedLoginTime = localStorage.getItem('loginTime');
    if (storedLoginTime) {
      return new Date(storedLoginTime);
    }
    // Nếu không có thì lưu thời gian hiện tại
    const now = new Date();
    localStorage.setItem('loginTime', now.toISOString());
    return now;
  } catch (error) {
    console.warn('Không thể lấy thời gian login:', error);
    return new Date();
  }
};

// Format thời gian login hiển thị
const formatLoginTime = () => {
  try {
    return loginTime.value.toLocaleString('vi-VN', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    });
  } catch (error) {
    return 'N/A';
  }
};

// Lấy địa chỉ IP của user
const getUserIP = async () => {
  try {
    // Thử nhiều API khác nhau để lấy IP
    const ipAPIs = [
      'https://api.ipify.org?format=json',
      'https://ipapi.co/json/',
      'https://json.geoiplookup.io/'
    ];

    for (const api of ipAPIs) {
      try {
        const response = await fetch(api);
        if (response.ok) {
          const data = await response.json();
          // Xử lý response khác nhau cho từng API
          if (data.ip) {
            return data.ip;
          } else if (data.IPv4) {
            return data.IPv4;
          }
        }
      } catch (apiError) {
        console.warn(`API ${api} failed:`, apiError);
        continue;
      }
    }
    
    // Fallback: sử dụng IP local nếu không thể lấy được IP public
    return '127.0.0.1 (Local)';
  } catch (error) {
    console.warn('Không thể lấy địa chỉ IP:', error);
    return 'N/A';
  }
};

// Khởi tạo khi component được mount
onMounted(async () => {
  currentUser.value = getCurrentUser();
  loginTime.value = getLoginTime();
  userIP.value = await getUserIP();
});
</script>

<style scoped>
/* === APP FOOTER STYLES === */
.app-footer {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  background: linear-gradient(135deg, #722f37 0%, #8b1538 50%, #a91b60 100%);
  color: white;
  padding: 8px 20px;
  box-shadow: 0 -4px 16px rgba(114, 47, 55, 0.4);
  backdrop-filter: blur(10px);
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  z-index: 1000;
  transition: all 0.3s ease;
}

.footer-content {
  max-width: 1400px;
  margin: 0 auto;
  display: flex;
  justify-content: center;
  align-items: center;
}

.user-session-info {
  display: flex;
  align-items: center;
  gap: 12px;
  font-size: 14px;
  font-weight: 500;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.3);
}

.user-info,
.login-info,
.ip-info {
  display: flex;
  align-items: center;
  gap: 6px;
  white-space: nowrap;
}

.user-icon,
.login-icon,
.ip-icon {
  font-size: 16px;
  opacity: 0.9;
}

.user-name,
.login-time,
.ip-address {
  font-family: 'Segoe UI', 'Arial', sans-serif;
  letter-spacing: 0.3px;
}

.divider {
  color: rgba(255, 255, 255, 0.6);
  font-weight: 300;
  margin: 0 4px;
}

/* Hover effects */
.user-info:hover,
.login-info:hover,
.ip-info:hover {
  transform: translateY(-1px);
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.4);
  transition: all 0.2s ease;
}

/* Responsive design */
@media (max-width: 768px) {
  .app-footer {
    padding: 6px 15px;
  }
  
  .user-session-info {
    font-size: 12px;
    gap: 8px;
    flex-wrap: wrap;
    justify-content: center;
  }
  
  .user-icon,
  .login-icon,
  .ip-icon {
    font-size: 14px;
  }
}

@media (max-width: 480px) {
  .app-footer {
    padding: 5px 10px;
  }
  
  .user-session-info {
    font-size: 11px;
    gap: 6px;
  }
  
  .divider {
    margin: 0 2px;
  }
}

/* Animation cho footer */
@keyframes slideInUp {
  from {
    transform: translateY(100%);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.app-footer {
  animation: slideInUp 0.5s ease-out;
}
</style>
