<template>
  <div class="about-info">
    <div class="info-header">
      <h1>📋 Thông tin bản quyền</h1>
    </div>

    <div class="info-content">
      <div class="copyright-card">
        <div class="card-icon">🏛️</div>
        <h2>Thông tin sản phẩm</h2>
        <p class="copyright-text">Đây là sản phẩm của <strong>Bộ phận IT - Phòng KTNQ Chi nhánh Lai Châu</strong></p>

        <div class="contact-info">
          <h3>📞 Thông tin liên hệ</h3>
          <div class="contact-details">
            <div class="contact-item">
              <span class="contact-icon">📱</span>
              <span>Hotline: <strong>(0213).3876642</strong></span>
            </div>
          </div>
          <p class="support-text">
            Mọi vướng mắc xin vui lòng liên hệ theo số điện thoại trên để được hỗ trợ kịp thời.
          </p>
        </div>
      </div>

      <div class="agribank-info">
        <div class="bank-logo">🏦</div>
        <h3>Ngân hàng Nông nghiệp và Phát triển Nông thôn Việt Nam</h3>
        <p>Chi nhánh Lai Châu</p>
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
.about-info {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
}

.info-header {
  text-align: center;
  margin-bottom: 30px;
}

.info-header h1 {
  color: #8b1538;
  font-size: 2.5rem;
  font-weight: 700;
  text-shadow: 0 2px 4px rgba(139, 21, 56, 0.2);
}

.info-content {
  display: flex;
  flex-direction: column;
  gap: 30px;
}

.copyright-card {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border-radius: 16px;
  padding: 30px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.3);
  text-align: center;
}

.card-icon {
  font-size: 4rem;
  margin-bottom: 20px;
}

.copyright-card h2 {
  color: #8b1538;
  font-size: 1.8rem;
  font-weight: 600;
  margin-bottom: 20px;
}

.copyright-text {
  font-size: 1.2rem;
  color: #2c3e50;
  line-height: 1.6;
  margin-bottom: 30px;
}

.contact-info {
  background: rgba(139, 21, 56, 0.05);
  border-radius: 12px;
  padding: 25px;
  margin-top: 20px;
}

.contact-info h3 {
  color: #8b1538;
  font-size: 1.4rem;
  font-weight: 600;
  margin-bottom: 20px;
  text-align: center;
}

.contact-details {
  display: flex;
  flex-direction: column;
  gap: 15px;
  margin-bottom: 20px;
}

.contact-item {
  display: flex;
  align-items: center;
  gap: 12px;
  font-size: 1.1rem;
  color: #2c3e50;
}

.contact-icon {
  font-size: 1.3rem;
  width: 30px;
  text-align: center;
}

.support-text {
  font-style: italic;
  color: #666;
  font-size: 0.95rem;
  text-align: center;
  margin-top: 15px;
}

.agribank-info {
  background: linear-gradient(135deg, #8b1538, #a91b47);
  color: white;
  border-radius: 16px;
  padding: 25px;
  text-align: center;
  box-shadow: 0 8px 24px rgba(139, 21, 56, 0.3);
}

.bank-logo {
  font-size: 3rem;
  margin-bottom: 15px;
}

.agribank-info h3 {
  font-size: 1.4rem;
  font-weight: 600;
  margin-bottom: 10px;
}

.agribank-info p {
  font-size: 1.1rem;
  opacity: 0.9;
}

/* Responsive Design */
@media (max-width: 768px) {
  .about-info {
    padding: 15px;
  }

  .info-header h1 {
    font-size: 2rem;
  }

  .copyright-card {
    padding: 20px;
  }

  .copyright-text {
    font-size: 1.1rem;
  }

  .contact-details {
    gap: 12px;
  }

  .contact-item {
    font-size: 1rem;
  }
}

@media (max-width: 480px) {
  .info-header h1 {
    font-size: 1.6rem;
  }

  .copyright-card h2 {
    font-size: 1.5rem;
  }

  .copyright-text {
    font-size: 1rem;
  }

  .contact-info h3 {
    font-size: 1.2rem;
  }

  .card-icon {
    font-size: 3rem;
  }

  .bank-logo {
    font-size: 2.5rem;
  }
}
</style>
