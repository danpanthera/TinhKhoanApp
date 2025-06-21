<template>
  <div class="home">
    <div class="welcome-hero">
      <div class="hero-content">
        <h1 class="hero-title">
          <img src="/Logo-Agribank-2.png" alt="Agribank Logo" class="hero-logo" />
          <br>
          Agribank Lai Châu Center
        </h1>
        <p class="hero-subtitle">
          Hệ thống quản lý tính khoán và lương | Hệ thống Báo cáo
        </p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { isAuthenticated } from '../services/auth';

const router = useRouter();

onMounted(() => {
  if (!isAuthenticated()) {
    router.push('/login');
  }
});
</script>

<style scoped>
.home {
  padding: 0;
  margin: -30px;
  min-height: calc(100vh - 120px);
  background: transparent !important; /* Đảm bảo không có nền nào */
}

.welcome-hero {
  text-align: center;
  padding: 60px 20px;
  background: transparent; /* Làm hoàn toàn trong suốt */
}

.hero-content {
  max-width: 1200px;
  margin: 0 auto;
  background: transparent; /* Làm hoàn toàn trong suốt */
}

.hero-title {
  font-size: 3.5rem;
  font-weight: 700;
  color: #8B1538;
  margin-bottom: 20px;
  text-shadow: 
    0 2px 4px rgba(139, 21, 56, 0.3),
    0 0 10px rgba(255, 255, 255, 0.8),
    0 0 20px rgba(255, 255, 255, 0.6); /* Thêm hiệu ứng ánh sáng để nổi bật trên ảnh nền */
  line-height: 1.2;
}

.nature-icon {
  font-size: 4rem;
  vertical-align: middle;
  margin-right: 16px;
  animation: gentle-sway 3s ease-in-out infinite;
}

.hero-logo {
  height: 60px;
  width: auto;
  margin-bottom: 20px;
  filter: drop-shadow(0 4px 8px rgba(0, 0, 0, 0.3));
  animation: gentle-sway 3s ease-in-out infinite;
}

@keyframes gentle-sway {
  0%, 100% { transform: rotate(-5deg); }
  50% { transform: rotate(5deg); }
}

.hero-subtitle {
  font-size: 1.5rem;
  color: #A91B47;
  margin-bottom: 50px;
  font-weight: 500;
  text-shadow: 
    0 2px 4px rgba(255, 255, 255, 0.9),
    0 0 8px rgba(255, 255, 255, 0.7); /* Tăng cường độ nổi bật của text */
}

/* Responsive Design */
@media (max-width: 768px) {
  .hero-title {
    font-size: 2.5rem;
  }
  
  .hero-logo {
    height: 50px;
  }
  
  .hero-subtitle {
    font-size: 1.2rem;
  }
  
  .welcome-hero {
    padding: 40px 15px;
  }
}

@media (max-width: 480px) {
  .hero-title {
    font-size: 2rem;
  }
  
  .hero-logo {
    height: 40px;
  }
  
  .nature-icon {
    font-size: 2.5rem;
  }
}
</style>
