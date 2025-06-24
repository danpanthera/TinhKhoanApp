<template>
  <div class="raw-data-warehouse">
    <!-- Header Section -->
    <div class="header-section">
      <h1>🏦 KHO DỮ LIỆU THÔ</h1>
      <p class="subtitle">Hệ thống quản lý và import dữ liệu nghiệp vụ ngân hàng chuyên nghiệp</p>
    </div>

    <!-- Thông báo -->
    <div v-if="errorMessage" class="alert alert-error">
      <span class="alert-icon">⚠️</span>
      {{ errorMessage }}
      <button @click="clearMessage" class="alert-close">×</button>
    </div>

    <div v-if="successMessage" class="alert alert-success">
      <span class="alert-icon">✅</span>
      {{ successMessage }}
      <button @click="clearMessage" class="alert-close">×</button>
    </div>

    <!-- Loading indicator -->
    <div v-if="loading" class="loading-section">
      <div class="loading-spinner"></div>
      <p>{{ loadingMessage || 'Đang xử lý dữ liệu...' }}</p>
    </div>

    <!-- Simple content for testing -->
    <div class="content">
      <p>Đây là phiên bản đơn giản để test. Dữ liệu sẽ được load từ API...</p>
      <button @click="refreshAllData" :disabled="loading">🔄 Tải lại dữ liệu</button>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import rawDataService from '@/services/rawDataService'

// Reactive state
const loading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')
const loadingMessage = ref('')

// Methods
const clearMessage = () => {
  errorMessage.value = ''
  successMessage.value = ''
}

const showError = (message) => {
  errorMessage.value = message
  setTimeout(() => {
    errorMessage.value = ''
  }, 5000)
}

const showSuccess = (message) => {
  successMessage.value = message
  setTimeout(() => {
    successMessage.value = ''
  }, 3000)
}

// Data management methods
const refreshAllData = async () => {
  try {
    loading.value = true
    loadingMessage.value = 'Đang tải lại dữ liệu...'
    
    console.log('🔄 Starting refresh all data...')
    
    const result = await rawDataService.getAllImports()
    console.log('📊 Raw result from getAllImports:', result)

    if (result.success) {
      console.log('✅ Loaded imports:', result.data?.length || 0, 'items')
      showSuccess(`✅ Đã tải lại dữ liệu thành công (${result.data?.length || 0} imports)`)
    } else {
      const errorMsg = result.error || 'Không thể tải dữ liệu'
      console.error('🔥 Chi tiết lỗi:', result)
      showError(errorMsg)
    }
    
  } catch (error) {
    console.error('Error refreshing data:', error)
    showError('Có lỗi xảy ra khi tải dữ liệu')
  } finally {
    loading.value = false
    loadingMessage.value = ''
  }
}

// Lifecycle
onMounted(async () => {
  await refreshAllData()
})
</script>

<style scoped>
.raw-data-warehouse {
  padding: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.header-section {
  background: linear-gradient(135deg, #8B1538 0%, #C41E3A 50%, #8B1538 100%);
  color: white;
  padding: 40px 30px;
  text-align: center;
  margin-bottom: 30px;
  border-radius: 15px;
  box-shadow: 0 10px 30px rgba(139, 21, 56, 0.3);
}

.alert {
  padding: 15px;
  margin: 10px 0;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.alert-error {
  background-color: #fef2f2;
  border: 1px solid #fca5a5;
  color: #dc2626;
}

.alert-success {
  background-color: #f0fdf4;
  border: 1px solid #86efac;
  color: #059669;
}

.alert-close {
  background: none;
  border: none;
  font-size: 18px;
  cursor: pointer;
}

.loading-section {
  text-align: center;
  padding: 40px;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f4f6;
  border-top: 4px solid #8B1538;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 20px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.content {
  background: white;
  padding: 30px;
  border-radius: 10px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  text-align: center;
}

button {
  background: #8B1538;
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 6px;
  cursor: pointer;
  margin-top: 20px;
}

button:hover {
  background: #a11d42;
}

button:disabled {
  background: #9ca3af;
  cursor: not-allowed;
}
</style>
