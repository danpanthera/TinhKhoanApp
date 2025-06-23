<template>
  <div class="home">
    <div class="welcome-hero">
      <div class="hero-content">
        <h1 class="hero-title">
          <img src="/src/assets/Logo-Agribank-2.png" alt="Agribank Logo" class="hero-logo" />
          <br>
          AGRIBANK LAI CHAU
        </h1>
        <p class="hero-subtitle">
          HỆ THỐNG QUẢN LÝ KHOÁN | HỆ THỐNG BÁO CÁO
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
  padding: 60px 20px 60px 20px; /* Tăng padding để tạo không gian trang trọng */
  background: transparent;
  min-height: 80vh; /* Chiếm phần lớn màn hình */
  display: flex;
  align-items: center; /* Căn giữa theo chiều dọc */
  justify-content: center; /* Căn giữa theo chiều ngang */
}

.hero-content {
  max-width: 1000px;
  margin: 0 auto;
  background: transparent;
}

.hero-title {
  font-size: 5.5rem; /* Tăng kích thước để trang trọng hơn */
  font-weight: 700; /* Độ đậm vừa phải, trang trọng */
  font-family: 'Playfair Display', 'Georgia', 'Times New Roman', serif; /* Font trang trọng, sang trọng */
  color: #8B1538; /* Màu đỏ bordeaux thương hiệu Agribank */
  margin-bottom: 30px; /* Tăng khoảng cách với dòng dưới */
  text-shadow: 
    0 6px 12px rgba(139, 21, 56, 0.3), /* Bóng mềm mại màu đỏ Agribank */
    0 3px 6px rgba(0, 0, 0, 0.2);      /* Bóng nhẹ để tạo chiều sâu */
  line-height: 1.1;
  letter-spacing: 0.05em; /* Khoảng cách chữ vừa phải, thanh lịch */
  text-transform: uppercase;
  /* Thêm hiệu ứng gradient text - màu bordeaux Agribank */
  background: linear-gradient(135deg, #8B1538 0%, #C41E3A 50%, #8B1538 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.nature-icon {
  font-size: 4rem;
  vertical-align: middle;
  margin-right: 16px;
  animation: gentle-sway 3s ease-in-out infinite;
}

.hero-logo {
  height: 80px; /* Tăng kích thước logo */
  width: auto;
  margin-bottom: 30px; /* Tăng khoảng cách */
  filter: drop-shadow(0 6px 12px rgba(139, 21, 56, 0.4)); /* Bóng màu Agribank */
  animation: gentle-sway 4s ease-in-out infinite; /* Chậm hơn, mềm mại hơn */
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
  font-size: 2.2rem; /* Tăng kích thước để cân đối với title */
  font-family: 'Crimson Text', 'Playfair Display', 'Georgia', serif; /* Font serif trang trọng */
  color: #8B1538; /* Màu đỏ bordeaux thương hiệu Agribank */
  margin-bottom: 0; /* Bỏ margin bottom vì không có dashboard */
  margin-top: 10px; /* Khoảng cách vừa phải với title */
  font-weight: 500; /* Trọng lượng vừa phải */
  font-style: normal; /* Bỏ italic để trang trọng hơn */
  text-shadow: 
    0 3px 6px rgba(139, 21, 56, 0.25), /* Bóng nhẹ màu Agribank */
    0 1px 3px rgba(0, 0, 0, 0.15);     /* Bóng nhẹ để tạo chiều sâu */
  letter-spacing: 0.03em; /* Khoảng cách chữ thanh lịch */
  line-height: 1.3; /* Tăng line-height để dễ đọc */
  /* Thêm hiệu ứng gradient text nhẹ - màu bordeaux Agribank */
  background: linear-gradient(135deg, #8B1538 0%, #C41E3A 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

/* Responsive Design */
@media (max-width: 768px) {
  .hero-title {
    font-size: 3.8rem; /* Responsive cho tablet */
  }
  
  .hero-logo {
    height: 60px;
  }
  
  .hero-subtitle {
    font-size: 1.8rem; /* Responsive cho tablet */
  }
  
  .welcome-hero {
    padding: 40px 15px 40px 15px;
    min-height: 70vh;
  }
}

@media (max-width: 480px) {
  .hero-title {
    font-size: 2.8rem; /* Responsive cho mobile */
  }
  
  .hero-logo {
    height: 50px;
  }
  
  .hero-subtitle {
    font-size: 1.4rem; /* Responsive cho mobile */
  }
  
  .welcome-hero {
    padding: 30px 15px 30px 15px;
    min-height: 60vh;
  }
}
</style>
