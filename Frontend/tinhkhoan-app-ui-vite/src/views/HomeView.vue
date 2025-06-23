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

/* CSS cho chữ thẳng, không cong - Enhanced Responsive */
.hero-text {
  display: inline-block;
  transform: none;
  text-transform: uppercase;
  /* Smart text wrapping */
  word-break: keep-all; /* Giữ nguyên từ, không ngắt trong từ */
  overflow-wrap: anywhere; /* Chỉ ngắt khi thực sự cần thiết */
  hyphens: none; /* Không dùng dấu gạch ngang */
}

.hero-title {
  font-size: clamp(2.5rem, 8vw, 6.5rem); /* Responsive font-size tự động co giãn */
  font-weight: 800;
  font-family: 'Inter', 'Segoe UI', 'Roboto', 'Arial', sans-serif;
  color: #8B1538 !important;
  margin-bottom: 25px;
  text-shadow: 
    0 8px 16px rgba(139, 21, 56, 0.4),
    0 4px 8px rgba(0, 0, 0, 0.3),
    0 2px 4px rgba(139, 21, 56, 0.6);
  line-height: 1.1;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  background: none;
  -webkit-background-clip: unset;
  -webkit-text-fill-color: #8B1538;
  background-clip: unset;
  transform: none;
  filter: drop-shadow(0 10px 20px rgba(139, 21, 56, 0.3));
  /* Responsive text wrapping */
  white-space: normal; /* Cho phép xuống dòng */
  word-wrap: break-word; /* Tự động ngắt từ nếu quá dài */
  overflow-wrap: break-word; /* Hỗ trợ break word tốt hơn */
  hyphens: auto; /* Tự động thêm dấu gạch ngang khi ngắt từ */
  width: 100%;
  max-width: 100%;
  overflow: visible;
  text-align: center; /* Căn giữa text */
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
  font-size: clamp(1.2rem, 4vw, 2.6rem); /* Responsive font-size tự động co giãn */
  font-family: 'Inter', 'Segoe UI', 'Roboto', 'Arial', sans-serif;
  color: #8B1538 !important;
  margin-bottom: 0;
  margin-top: 5px;
  font-weight: 700;
  font-style: normal;
  text-shadow: 
    0 6px 12px rgba(139, 21, 56, 0.35),
    0 3px 6px rgba(0, 0, 0, 0.25),
    0 1px 3px rgba(139, 21, 56, 0.5);
  letter-spacing: 0.05em;
  line-height: 1.3;
  background: none;
  -webkit-background-clip: unset;
  -webkit-text-fill-color: #8B1538;
  background-clip: unset;
  transform: none;
  filter: drop-shadow(0 6px 12px rgba(139, 21, 56, 0.25));
  /* Responsive text wrapping */
  white-space: normal; /* Cho phép xuống dòng */
  word-wrap: break-word; /* Tự động ngắt từ nếu quá dài */
  overflow-wrap: break-word; /* Hỗ trợ break word tốt hơn */
  hyphens: auto; /* Tự động thêm dấu gạch ngang khi ngắt từ */
  width: 100%;
  max-width: 100%;
  overflow: visible;
  text-align: center; /* Căn giữa text */
}

/* Responsive Design - Enhanced */
@media (max-width: 1200px) {
  .hero-content {
    padding: 0 20px; /* Tăng padding cho màn hình lớn */
  }
}

@media (max-width: 768px) {
  .welcome-hero {
    min-height: 100vh;
    padding: 0 15px 20px 15px;
    padding-top: 1.5vh;
  }
  
  .hero-content {
    padding: 0 10px; /* Giảm padding cho tablet */
  }
  
  .hero-logo {
    height: 70px;
    transform: scale(1.05);
  }
}

@media (max-width: 480px) {
  .welcome-hero {
    min-height: 100vh;
    padding: 0 10px 20px 10px;
    padding-top: 1vh;
  }
  
  .hero-content {
    padding: 0 5px; /* Padding nhỏ nhất cho mobile */
  }
  
  .hero-logo {
    height: 60px;
    transform: scale(1.0);
  }
}

/* Extra small screens - điện thoại nhỏ */
@media (max-width: 320px) {
  .hero-content {
    padding: 0 2px; /* Padding tối thiểu */
  }
  
  .hero-title {
    letter-spacing: 0.02em; /* Giảm letter-spacing để tiết kiệm không gian */
  }
  
  .hero-subtitle {
    letter-spacing: 0.01em; /* Giảm letter-spacing cho subtitle */
  }
}
</style>
