<template>
  <div class="user-guide">
    <div class="guide-header">
      <h1>📖 Hướng dẫn sử dụng phần mềm</h1>
      <p class="guide-subtitle">Tài liệu hướng dẫn chi tiết về cách sử dụng hệ thống KPI Agribank Lai Châu Center</p>
    </div>

    <div class="guide-content">
      <div class="coming-soon-card">
        <div class="construction-icon">🚧</div>
        <h2>Đang xây dựng</h2>
        <p class="coming-soon-text">
          Tài liệu hướng dẫn sử dụng chi tiết đang được hoàn thiện và sẽ sớm được cập nhật.
        </p>

        <div class="preview-features">
          <h3>📋 Nội dung sẽ có trong hướng dẫn:</h3>
          <ul class="feature-list">
            <li>
              <span class="feature-icon">👥</span>
              <span>Quản lý thông tin nhân viên và tổ chức</span>
            </li>
            <li>
              <span class="feature-icon">📊</span>
              <span>Thiết lập và theo dõi KPI</span>
            </li>
            <li>
              <span class="feature-icon">📈</span>
              <span>Nhập và đánh giá điểm KPI</span>
            </li>
            <li>
              <span class="feature-icon">💰</span>
              <span>Tính toán và báo cáo lương</span>
            </li>
            <li>
              <span class="feature-icon">📋</span>
              <span>Xuất báo cáo và thống kê</span>
            </li>
          </ul>
        </div>

        <div class="contact-support">
          <h3>🤝 Cần hỗ trợ ngay?</h3>
          <p>Liên hệ Bộ phận IT - Phòng KTNQ:</p>
          <div class="support-contacts">
            <span class="support-item">📞 (0231).3876642</span>
            <span class="support-item">📱 0912.955.113</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { isAuthenticated } from '../services/auth'

const router = useRouter()

onMounted(() => {
  if (!isAuthenticated()) {
    router.push('/login')
  }
})
</script>

<style scoped>
.user-guide {
  max-width: 900px;
  margin: 0 auto;
  padding: 20px;
}

.guide-header {
  text-align: center;
  margin-bottom: 40px;
}

.guide-header h1 {
  color: #8b1538;
  font-size: 2.5rem;
  font-weight: 700;
  text-shadow: 0 2px 4px rgba(139, 21, 56, 0.2);
  margin-bottom: 15px;
}

.guide-subtitle {
  color: #a91b47;
  font-size: 1.2rem;
  font-weight: 500;
}

.guide-content {
  display: flex;
  justify-content: center;
}

.coming-soon-card {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border-radius: 20px;
  padding: 40px;
  box-shadow: 0 12px 40px rgba(0, 0, 0, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.3);
  text-align: center;
  max-width: 700px;
  width: 100%;
}

.construction-icon {
  font-size: 5rem;
  margin-bottom: 20px;
}

.coming-soon-card h2 {
  color: #8b1538;
  font-size: 2rem;
  font-weight: 600;
  margin-bottom: 20px;
}

.coming-soon-text {
  font-size: 1.2rem;
  color: #2c3e50;
  line-height: 1.6;
  margin-bottom: 40px;
}

.preview-features {
  background: rgba(139, 21, 56, 0.05);
  border-radius: 15px;
  padding: 30px;
  margin-bottom: 30px;
  text-align: left;
}

.preview-features h3 {
  color: #8b1538;
  font-size: 1.4rem;
  font-weight: 600;
  margin-bottom: 20px;
  text-align: center;
}

.feature-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.feature-list li {
  display: flex;
  align-items: center;
  gap: 15px;
  padding: 12px 0;
  font-size: 1.1rem;
  color: #2c3e50;
  border-bottom: 1px solid rgba(139, 21, 56, 0.1);
}

.feature-list li:last-child {
  border-bottom: none;
}

.feature-icon {
  font-size: 1.5rem;
  width: 40px;
  text-align: center;
}

.contact-support {
  background: linear-gradient(135deg, #8b1538, #a91b47);
  color: white;
  border-radius: 15px;
  padding: 25px;
}

.contact-support h3 {
  font-size: 1.3rem;
  font-weight: 600;
  margin-bottom: 15px;
}

.contact-support p {
  font-size: 1.1rem;
  margin-bottom: 15px;
  opacity: 0.9;
}

.support-contacts {
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
  justify-content: center;
}

.support-item {
  background: rgba(255, 255, 255, 0.2);
  padding: 8px 16px;
  border-radius: 8px;
  font-weight: 600;
  font-size: 1rem;
}

/* Responsive Design */
@media (max-width: 768px) {
  .user-guide {
    padding: 15px;
  }

  .guide-header h1 {
    font-size: 2rem;
  }

  .guide-subtitle {
    font-size: 1.1rem;
  }

  .coming-soon-card {
    padding: 30px 20px;
  }

  .coming-soon-text {
    font-size: 1.1rem;
  }

  .preview-features {
    padding: 20px;
  }

  .feature-list li {
    font-size: 1rem;
  }

  .support-contacts {
    flex-direction: column;
    gap: 10px;
  }
}

@media (max-width: 480px) {
  .guide-header h1 {
    font-size: 1.6rem;
  }

  .coming-soon-card h2 {
    font-size: 1.6rem;
  }

  .coming-soon-text {
    font-size: 1rem;
  }

  .construction-icon {
    font-size: 4rem;
  }

  .preview-features h3 {
    font-size: 1.2rem;
  }

  .contact-support h3 {
    font-size: 1.1rem;
  }
}
</style>
