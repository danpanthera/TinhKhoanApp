<template>
  <div class="home">
    <div class="welcome-hero">
      <div class="hero-content">
        <h1 class="hero-title">
          <img src="/src/assets/Logo-Agribank-2.png" alt="Agribank Logo" class="hero-logo" />
          <br>
          <span class="hero-text">AGRIBANK LAI CHAU CENTER</span>
        </h1>
        <p class="hero-subtitle">
          <span class="hero-text">HỆ THỐNG QUẢN LÝ KHOÁN | HỆ THỐNG BÁO CÁO</span>
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
  margin: 0;
  min-height: calc(100vh - 120px);
  background: transparent !important; /* Đảm bảo không có nền nào */
  overflow-x: hidden; /* Ngăn cuộn ngang */
  width: 100%; /* Đảm bảo chiều rộng không vượt quá viewport */
  box-sizing: border-box; /* Đảm bảo padding không làm tăng width */
}

.welcome-hero {
  text-align: center;
  padding: 0 0 20px 0; /* Bỏ padding ngang để tránh overflow */
  background: transparent;
  min-height: 100vh; /* Chiếm toàn bộ màn hình */
  display: flex;
  align-items: flex-start; /* Đẩy lên trên */
  justify-content: center; /* Căn giữa theo chiều ngang */
  padding-top: 2vh; /* Giảm từ 4vh xuống 2vh để đưa lên cao hơn khoảng 1.5cm */
  width: 100%; /* Đảm bảo chiều rộng đúng với viewport */
  overflow-x: hidden; /* Ngăn cuộn ngang */
  box-sizing: border-box; /* Đảm bảo padding không làm tăng width */
}

.hero-content {
  width: auto;
  margin: 0 auto;
  background: transparent;
  padding: 0 10px; /* Thêm padding để tránh text sát rìa */
  box-sizing: border-box; /* Đảm bảo padding không làm tăng width */
  overflow: visible; /* Cho phép nội dung hiển thị đầy đủ */
}

/* CSS cho chữ thẳng, không cong */
.hero-text {
  display: inline-block;
  transform: none; /* Chữ hoàn toàn thẳng */
  text-transform: uppercase;
}

.hero-title {
  font-size: 6.5rem; /* Tăng kích thước để nổi bật hơn */
  font-weight: 800; /* Tăng độ đậm để "nổi" hơn */
  font-family: 'Inter', 'Segoe UI', 'Roboto', 'Arial', sans-serif; /* Font hiện đại, clean */
  color: #8B1538 !important; /* Màu đỏ bordeaux thương hiệu Agribank cố định */
  margin-bottom: 25px; /* Giảm khoảng cách để gọn hơn */
  text-shadow: 
    0 8px 16px rgba(139, 21, 56, 0.4), /* Bóng đậm hơn màu đỏ Agribank */
    0 4px 8px rgba(0, 0, 0, 0.3),      /* Bóng đen để tạo chiều sâu */
    0 2px 4px rgba(139, 21, 56, 0.6);  /* Bóng gần tạo độ "nổi" */
  line-height: 1.0; /* Giảm line-height để gọn hơn */
  letter-spacing: 0.08em; /* Tăng khoảng cách chữ để nổi bật */
  text-transform: uppercase;
  /* Bỏ gradient, dùng màu bordeaux cố định */
  background: none;
  -webkit-background-clip: unset;
  -webkit-text-fill-color: #8B1538;
  background-clip: unset;
  /* Bỏ hiệu ứng 3D để chữ thẳng */
  transform: none;
  filter: drop-shadow(0 10px 20px rgba(139, 21, 56, 0.3));
  /* Đảm bảo chữ không bị cắt */
  white-space: nowrap;
  width: auto;
  overflow: visible;
  text-overflow: initial;
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
  font-family: 'Inter', 'Segoe UI', 'Roboto', 'Arial', sans-serif; /* Font hiện đại, clean */
  color: #8B1538 !important; /* Màu đỏ bordeaux thương hiệu Agribank cố định */
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
  /* Bỏ gradient, dùng màu bordeaux cố định */
  background: none;
  -webkit-background-clip: unset;
  -webkit-text-fill-color: #8B1538;
  background-clip: unset;
  /* Bỏ hiệu ứng 3D để chữ thẳng */
  transform: none;
  filter: drop-shadow(0 6px 12px rgba(139, 21, 56, 0.25));
  /* Đảm bảo chữ không bị cắt */
  white-space: nowrap;
  width: auto;
  overflow: visible;
  text-overflow: initial;
}

/* Responsive Design */
@media (max-width: 768px) {
  .welcome-hero {
    min-height: 100vh; /* Vẫn chiếm toàn màn hình trên tablet */
    padding: 0 15px 20px 15px; /* Giảm padding cho tablet */
    padding-top: 1.5vh; /* Khoảng cách từ trên xuống trên tablet - giảm xuống */
  }
  
  .hero-title {
    font-size: 3.8rem; /* Giảm size cho tablet để fit "CENTER" */
    transform: none; /* Bỏ hiệu ứng 3D để chữ thẳng */
    white-space: normal; /* Cho phép xuống dòng trên tablet nếu cần */
    overflow: visible;
    width: auto;
  }
  
  .hero-logo {
    height: 70px;
    transform: scale(1.05); /* Giảm scale cho tablet */
  }
  
  .hero-subtitle {
    font-size: 1.8rem; /* Giảm size cho tablet để fit */
    transform: none; /* Bỏ hiệu ứng 3D để chữ thẳng */
    white-space: normal; /* Cho phép xuống dòng trên tablet nếu cần */
    overflow: visible;
    width: auto;
  }
}

@media (max-width: 480px) {
  .welcome-hero {
    min-height: 100vh; /* Vẫn chiếm toàn màn hình trên mobile */
    padding: 0 10px 20px 10px; /* Giảm padding cho mobile */
    padding-top: 1vh; /* Khoảng cách từ trên xuống trên mobile - giảm xuống */
  }
  
  .hero-title {
    font-size: 2.8rem; /* Giảm size cho mobile để fit "CENTER" */
    transform: none; /* Bỏ hiệu ứng 3D để chữ thẳng */
    white-space: normal; /* Cho phép xuống dòng trên mobile */
    overflow: visible;
    width: auto;
  }
  
  .hero-logo {
    height: 60px;
    transform: scale(1.0); /* Bỏ scale cho mobile */
  }
  
  .hero-subtitle {
    font-size: 1.3rem; /* Giảm size cho mobile để fit */
    transform: none; /* Bỏ hiệu ứng 3D để chữ thẳng */
    white-space: normal; /* Cho phép xuống dòng trên mobile */
    overflow: visible;
    width: auto;
  }
}
</style>
