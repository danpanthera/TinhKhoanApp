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
  padding: 40px 20px 60px 20px; /* Giảm padding top để đưa nội dung lên cao hơn */
  background: transparent;
  min-height: 70vh; /* Giảm min-height để đưa nội dung lên trên */
  display: flex;
  align-items: flex-start; /* Căn lên trên thay vì center */
  justify-content: center; /* Căn giữa theo chiều ngang */
  padding-top: 80px; /* Thêm padding-top cụ thể để control vị trí */
}

.hero-content {
  max-width: 1000px;
  margin: 0 auto;
  background: transparent;
}

.hero-title {
  font-size: 6.5rem; /* Tăng kích thước để nổi bật hơn */
  font-weight: 800; /* Tăng độ đậm để "nổi" hơn */
  font-family: 'Playfair Display', 'Georgia', 'Times New Roman', serif; /* Font trang trọng, sang trọng */
  color: #8B1538; /* Màu đỏ bordeaux thương hiệu Agribank */
  margin-bottom: 25px; /* Giảm khoảng cách để gọn hơn */
  text-shadow: 
    0 8px 16px rgba(139, 21, 56, 0.4), /* Bóng đậm hơn màu đỏ Agribank */
    0 4px 8px rgba(0, 0, 0, 0.3),      /* Bóng đen để tạo chiều sâu */
    0 2px 4px rgba(139, 21, 56, 0.6);  /* Bóng gần tạo độ "nổi" */
  line-height: 1.0; /* Giảm line-height để gọn hơn */
  letter-spacing: 0.08em; /* Tăng khoảng cách chữ để nổi bật */
  text-transform: uppercase;
  /* Gradient text với độ tương phản cao hơn */
  background: linear-gradient(135deg, #8B1538 0%, #DC143C 30%, #B22222 70%, #8B1538 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  /* Thêm hiệu ứng 3D */
  transform: perspective(500px) rotateX(15deg);
  filter: drop-shadow(0 10px 20px rgba(139, 21, 56, 0.3));
}

.nature-icon {
  font-size: 4rem;
  vertical-align: middle;
  margin-right: 16px;
  animation: gentle-sway 3s ease-in-out infinite;
}

.hero-logo {
  height: 90px; /* Tăng kích thước logo để phù hợp */
  width: auto;
  margin-bottom: 20px; /* Giảm khoảng cách để gọn hơn */
  filter: drop-shadow(0 8px 16px rgba(139, 21, 56, 0.5)); /* Bóng đậm hơn màu Agribank */
  animation: gentle-sway 4s ease-in-out infinite; /* Chậm hơn, mềm mại hơn */
  transform: scale(1.1); /* Phóng to nhẹ để nổi bật */
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
  font-size: 2.6rem; /* Tăng kích thước để nổi bật hơn */
  font-family: 'Crimson Text', 'Playfair Display', 'Georgia', serif; /* Font serif trang trọng */
  color: #8B1538; /* Màu đỏ bordeaux thương hiệu Agribank */
  margin-bottom: 0; /* Bỏ margin bottom vì không có dashboard */
  margin-top: 5px; /* Giảm khoảng cách với title */
  font-weight: 700; /* Tăng trọng lượng để nổi hơn */
  font-style: normal; /* Bỏ italic để trang trọng hơn */
  text-shadow: 
    0 6px 12px rgba(139, 21, 56, 0.35), /* Bóng đậm hơn màu Agribank */
    0 3px 6px rgba(0, 0, 0, 0.25),      /* Bóng đen để tạo chiều sâu */
    0 1px 3px rgba(139, 21, 56, 0.5);   /* Bóng gần tạo độ "nổi" */
  letter-spacing: 0.05em; /* Tăng khoảng cách chữ để nổi bật */
  line-height: 1.2; /* Giảm line-height để gọn hơn */
  /* Gradient text với độ tương phản cao hơn */
  background: linear-gradient(135deg, #8B1538 0%, #DC143C 50%, #B22222 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  /* Thêm hiệu ứng 3D nhẹ */
  transform: perspective(300px) rotateX(10deg);
  filter: drop-shadow(0 6px 12px rgba(139, 21, 56, 0.25));
}

/* Responsive Design */
@media (max-width: 768px) {
  .welcome-hero {
    padding-top: 60px; /* Giảm padding-top cho tablet */
    min-height: 60vh;
  }
  
  .hero-title {
    font-size: 4.5rem; /* Responsive cho tablet nhưng vẫn nổi bật */
    transform: perspective(400px) rotateX(10deg); /* Giảm hiệu ứng 3D */
  }
  
  .hero-logo {
    height: 70px;
    transform: scale(1.05); /* Giảm scale cho tablet */
  }
  
  .hero-subtitle {
    font-size: 2.2rem; /* Responsive cho tablet nhưng vẫn nổi bật */
    transform: perspective(250px) rotateX(8deg); /* Giảm hiệu ứng 3D */
  }
}

@media (max-width: 480px) {
  .welcome-hero {
    padding-top: 40px; /* Giảm padding-top cho mobile */
    min-height: 50vh;
  }
  
  .hero-title {
    font-size: 3.2rem; /* Responsive cho mobile nhưng vẫn nổi bật */
    transform: perspective(300px) rotateX(8deg); /* Giảm hiệu ứng 3D */
  }
  
  .hero-logo {
    height: 60px;
    transform: scale(1.0); /* Bỏ scale cho mobile */
  }
  
  .hero-subtitle {
    font-size: 1.8rem; /* Responsive cho mobile nhưng vẫn nổi bật */
    transform: perspective(200px) rotateX(5deg); /* Giảm hiệu ứng 3D */
  }
}
</style>
