<template>
  <div class="home">
    <div class="welcome-hero">
      <div class="hero-content">
        <h1 class="hero-title">
          <img src="/src/assets/Logo-Agribank-2.png" alt="Agribank Logo" class="hero-logo" />
          <br>
          AGRIBANK LAI CHAU CENTER
        </h1>
        <p class="hero-subtitle">
          HỆ THỐNG QUẢN LÝ TÍNH KHOÁN | HỆ THỐNG BÁO CÁO
        </p>
      </div>
    </div>

    <!-- Dashboard với 6 bảng chỉ tiêu KPI -->
    <div class="dashboard-section">
      <BusinessPlanDashboard />
    </div>
  </div>
</template>

<script setup>
import { onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { isAuthenticated } from '../services/auth';
import BusinessPlanDashboard from './dashboard/BusinessPlanDashboard.vue';

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
  padding: 40px 20px 20px 20px; /* Giảm padding bottom để dashboard gần hơn */
  background: transparent; /* Làm hoàn toàn trong suốt */
}

.hero-content {
  max-width: 1200px;
  margin: 0 auto;
  background: transparent; /* Làm hoàn toàn trong suốt */
}

.hero-title {
  font-size: 4.5rem; /* Kích thước lớn cho tiêu đề chính */
  font-weight: 900; /* Tăng độ đậm tối đa */
  font-family: 'New York', 'Times New Roman', serif; /* Font New York theo yêu cầu anh */
  color: #8B1538; /* Màu bordeaux Agribank */
  margin-bottom: 10px; /* Giảm từ 20px xuống 10px để sát với dòng dưới */
  text-shadow: 
    0 4px 8px rgba(139, 21, 56, 0.4), /* Bóng chính màu đỏ Agribank */
    0 2px 4px rgba(0, 0, 0, 0.3);     /* Bóng đậm để tạo chiều sâu */
  line-height: 1.2;
  letter-spacing: 0.08em; /* Tăng khoảng cách chữ để tạo sự mạnh mẽ */
  text-transform: uppercase; /* Viết hoa toàn bộ để tạo sự chuyên nghiệp */
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
  /* Thêm animation lắc lư như cũ */
  animation: gentle-sway 3s ease-in-out infinite;
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
  font-size: 1.9rem; /* Tăng từ 1.5rem lên 1.9rem (+2 cỡ) */
  font-family: 'Labora', 'Arial', sans-serif; /* Font Labora theo yêu cầu anh */
  color: #8B1538; /* Màu đỏ bordeaux Agribank */
  margin-bottom: 30px; /* Giảm từ 50px xuống 30px để dashboard gần hơn */
  margin-top: 1mm; /* Sát lại dòng trên 1mm theo yêu cầu anh */
  font-weight: 600; /* Tăng độ đậm để nổi bật hơn */
  font-style: italic; /* Nghiêng để tạo sự thanh lịch */
  text-shadow: 
    0 2px 4px rgba(139, 21, 56, 0.3),
    0 1px 2px rgba(0, 0, 0, 0.2); /* Bóng nhẹ để tạo chiều sâu */
  letter-spacing: 0.02em; /* Khoảng cách chữ nhẹ */
}

.dashboard-section {
  margin-top: 0px; /* Không có margin để dashboard liền kề */
  padding: 0; /* Không có padding để dashboard full width */
  background: transparent;
}

/* Responsive Design */
@media (max-width: 768px) {
  .hero-title {
    font-size: 3.2rem; /* Tăng từ 2.5rem lên 3.2rem (+2 cỡ) */
  }
  
  .hero-logo {
    height: 50px;
  }
  
  .hero-subtitle {
    font-size: 1.6rem; /* Tăng từ 1.2rem lên 1.6rem (+2 cỡ) */
    margin-bottom: 20px; /* Giảm margin cho mobile */
  }
  
  .welcome-hero {
    padding: 30px 15px 15px 15px; /* Giảm padding cho mobile */
  }
}

@media (max-width: 480px) {
  .hero-title {
    font-size: 2.4rem; /* Tăng từ 2rem lên 2.4rem (+2 cỡ) */
  }
  
  .hero-logo {
    height: 40px;
  }
  
  .hero-subtitle {
    font-size: 1.4rem; /* Thêm responsive cho subtitle mobile */
    margin-bottom: 15px; /* Giảm margin cho mobile nhỏ */
  }
  
  .nature-icon {
    font-size: 2.5rem;
  }
  
  .welcome-hero {
    padding: 20px 15px 10px 15px; /* Giảm thêm padding cho mobile nhỏ */
  }
}
</style>
