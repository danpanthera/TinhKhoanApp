<template>
  <div class="software-info">
    <div class="info-header">
      <h1>💻 Thông tin phần mềm</h1>
      <p class="info-subtitle">Chi tiết thông số kỹ thuật và phiên bản hệ thống</p>
    </div>
    
    <div class="info-content">
      <div class="version-card">
        <div class="version-icon">🔖</div>
        <h2>Phiên bản hiện tại</h2>
        <div class="version-number">v1.0.0</div>
        <div class="version-date">Phát hành: Tháng 12, 2024</div>
      </div>
      
      <div class="details-grid">
        <div class="detail-card">
          <div class="detail-icon">🏗️</div>
          <h3>Thông tin xây dựng</h3>
          <div class="detail-content">
            <div class="detail-item">
              <strong>Tên hệ thống:</strong>
              <span>Agribank Lai Châu Center Management System</span>
            </div>
            <div class="detail-item">
              <strong>Mã sản phẩm:</strong>
              <span>AGLC-KPI-001</span>
            </div>
            <div class="detail-item">
              <strong>Ngày build:</strong>
              <span>{{ buildDate }}</span>
            </div>
          </div>
        </div>
        
        <div class="detail-card">
          <div class="detail-icon">⚙️</div>
          <h3>Thông số kỹ thuật</h3>
          <div class="detail-content">
            <div class="detail-item">
              <strong>Frontend:</strong>
              <span>Vue.js 3.x + Vite</span>
            </div>
            <div class="detail-item">
              <strong>Backend:</strong>
              <span>.NET Core API</span>
            </div>
            <div class="detail-item">
              <strong>Database:</strong>
              <span>SQLite / SQL Server</span>
            </div>
          </div>
        </div>
        
        <div class="detail-card">
          <div class="detail-icon">👨‍💻</div>
          <h3>Thông tin phát triển</h3>
          <div class="detail-content">
            <div class="detail-item">
              <strong>Phát triển bởi:</strong>
              <span>Bộ phận IT - Phòng KTNQ</span>
            </div>
            <div class="detail-item">
              <strong>Đơn vị:</strong>
              <span>Chi nhánh Lai Châu</span>
            </div>
            <div class="detail-item">
              <strong>Bản quyền:</strong>
              <span>© 2025 Agribank Lai Châu</span>
            </div>
          </div>
        </div>
        
        <div class="detail-card">
          <div class="detail-icon">🔧</div>
          <h3>Tính năng chính</h3>
          <div class="detail-content">
            <div class="feature-tag">Quản lý nhân sự</div>
            <div class="feature-tag">Đánh giá KPI</div>
            <div class="feature-tag">Tính lương tự động</div>
            <div class="feature-tag">Báo cáo thống kê</div>
          </div>
        </div>
      </div>
      
      <div class="changelog-card">
        <h3>📋 Lịch sử cập nhật</h3>
        <div class="changelog-list">
          <div class="changelog-item">
            <div class="changelog-version">v1.0.0</div>
            <div class="changelog-date">Tháng 12, 2024</div>
            <div class="changelog-description">
              • Phiên bản chính thức đầu tiên<br>
              • Hoàn thiện tất cả chức năng cơ bản<br>
              • Giao diện thương hiệu Agribank<br>
              • Hệ thống báo cáo đầy đủ
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { isAuthenticated } from '../services/auth';

const router = useRouter();
const buildDate = ref('');

onMounted(() => {
  if (!isAuthenticated()) {
    router.push('/login');
  }
  
  // Set current date as build date
  const now = new Date();
  buildDate.value = now.toLocaleDateString('vi-VN', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
});
</script>

<style scoped>
.software-info {
  max-width: 1000px;
  margin: 0 auto;
  padding: 20px;
}

.info-header {
  text-align: center;
  margin-bottom: 40px;
}

.info-header h1 {
  color: #8B1538;
  font-size: 2.5rem;
  font-weight: 700;
  text-shadow: 0 2px 4px rgba(139, 21, 56, 0.2);
  margin-bottom: 15px;
}

.info-subtitle {
  color: #A91B47;
  font-size: 1.2rem;
  font-weight: 500;
}

.info-content {
  display: flex;
  flex-direction: column;
  gap: 30px;
}

.version-card {
  background: linear-gradient(135deg, #8B1538, #A91B47);
  color: white;
  border-radius: 20px;
  padding: 40px;
  text-align: center;
  box-shadow: 0 12px 40px rgba(139, 21, 56, 0.3);
}

.version-icon {
  font-size: 4rem;
  margin-bottom: 20px;
}

.version-card h2 {
  font-size: 1.8rem;
  font-weight: 600;
  margin-bottom: 20px;
}

.version-number {
  font-size: 3rem;
  font-weight: 700;
  margin-bottom: 10px;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.3);
}

.version-date {
  font-size: 1.1rem;
  opacity: 0.9;
}

.details-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
}

.detail-card {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border-radius: 16px;
  padding: 25px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.3);
}

.detail-icon {
  font-size: 2.5rem;
  margin-bottom: 15px;
}

.detail-card h3 {
  color: #8B1538;
  font-size: 1.3rem;
  font-weight: 600;
  margin-bottom: 20px;
}

.detail-content {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.detail-item {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.detail-item strong {
  color: #8B1538;
  font-size: 0.9rem;
  font-weight: 600;
}

.detail-item span {
  color: #2c3e50;
  font-size: 1rem;
}

.feature-tag {
  display: inline-block;
  background: linear-gradient(135deg, #8B1538, #A91B47);
  color: white;
  padding: 6px 12px;
  border-radius: 15px;
  font-size: 0.9rem;
  font-weight: 500;
  margin: 4px;
}

.changelog-card {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border-radius: 16px;
  padding: 30px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.3);
}

.changelog-card h3 {
  color: #8B1538;
  font-size: 1.5rem;
  font-weight: 600;
  margin-bottom: 25px;
  text-align: center;
}

.changelog-list {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.changelog-item {
  background: rgba(139, 21, 56, 0.05);
  border-radius: 12px;
  padding: 20px;
  border-left: 4px solid #8B1538;
}

.changelog-version {
  color: #8B1538;
  font-size: 1.2rem;
  font-weight: 700;
  margin-bottom: 5px;
}

.changelog-date {
  color: #A91B47;
  font-size: 0.9rem;
  font-weight: 500;
  margin-bottom: 10px;
}

.changelog-description {
  color: #2c3e50;
  line-height: 1.6;
  font-size: 1rem;
}

/* Responsive Design */
@media (max-width: 768px) {
  .software-info {
    padding: 15px;
  }
  
  .info-header h1 {
    font-size: 2rem;
  }
  
  .info-subtitle {
    font-size: 1.1rem;
  }
  
  .version-card {
    padding: 30px 20px;
  }
  
  .version-number {
    font-size: 2.5rem;
  }
  
  .details-grid {
    grid-template-columns: 1fr;
  }
  
  .detail-card {
    padding: 20px;
  }
}

@media (max-width: 480px) {
  .info-header h1 {
    font-size: 1.6rem;
  }
  
  .version-card h2 {
    font-size: 1.5rem;
  }
  
  .version-number {
    font-size: 2rem;
  }
  
  .version-icon {
    font-size: 3rem;
  }
  
  .detail-icon {
    font-size: 2rem;
  }
  
  .detail-card h3 {
    font-size: 1.1rem;
  }
}
</style>
