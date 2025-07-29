<template>
  <Teleport to="body">
    <!-- Offline Status Banner -->
    <div v-if="offlineStore.isOffline" class="offline-banner">
      <div class="offline-content">
        <i class="fas fa-wifi-slash offline-icon"></i>
        <div class="offline-text">
          <strong>Chế độ Offline</strong>
          <span>Ứng dụng hoạt động mà không cần mạng</span>
        </div>
      </div>
    </div>

    <!-- Sync Status Indicator -->
    <div v-if="offlineStore.hasPendingActions || offlineStore.syncInProgress" class="sync-indicator">
      <div class="sync-content">
        <div class="sync-icon">
          <i v-if="offlineStore.syncInProgress" class="fas fa-sync-alt spinning"></i>
          <i v-else class="fas fa-clock"></i>
        </div>
        <div class="sync-text">
          <strong v-if="offlineStore.syncInProgress">Đang đồng bộ...</strong>
          <strong v-else>{{ offlineStore.pendingActions.length }} hành động chờ</strong>
          <button v-if="!offlineStore.syncInProgress && offlineStore.isOnline" @click="manualSync" class="sync-btn">
            <i class="fas fa-sync-alt"></i>
            Đồng bộ ngay
          </button>
        </div>
      </div>
    </div>

    <!-- Network Status Floating Widget -->
    <div class="network-widget" :class="{ offline: offlineStore.isOffline }">
      <div class="network-content" @click="toggleDetails">
        <i :class="networkIcon"></i>
        <span class="network-text">{{ networkStatus }}</span>
      </div>

      <!-- Detailed Panel -->
      <div v-if="showDetails" class="network-details">
        <div class="detail-header">
          <h4>Trạng thái kết nối</h4>
          <button @click="showDetails = false" class="close-btn">
            <i class="fas fa-times"></i>
          </button>
        </div>

        <div class="detail-body">
          <div class="detail-item">
            <i :class="networkIcon"></i>
            <span>{{ detailedStatus }}</span>
          </div>

          <div v-if="offlineStore.lastSyncTime" class="detail-item">
            <i class="fas fa-clock"></i>
            <span>Đồng bộ cuối: {{ formattedLastSync }}</span>
          </div>

          <div v-if="offlineStore.hasPendingActions" class="detail-item">
            <i class="fas fa-exclamation-triangle"></i>
            <span>{{ offlineStore.pendingActions.length }} hành động chờ đồng bộ</span>
          </div>
        </div>

        <div class="detail-actions">
          <button
            v-if="offlineStore.isOnline && offlineStore.hasPendingActions"
            @click="manualSync"
            :disabled="offlineStore.syncInProgress"
            class="action-btn primary"
          >
            <i class="fas fa-sync-alt" :class="{ spinning: offlineStore.syncInProgress }"></i>
            {{ offlineStore.syncInProgress ? 'Đang đồng bộ...' : 'Đồng bộ ngay' }}
          </button>

          <button
            v-if="offlineStore.hasPendingActions"
            @click="clearPending"
            :disabled="offlineStore.syncInProgress"
            class="action-btn danger"
          >
            <i class="fas fa-trash"></i>
            Xóa hành động chờ
          </button>

          <button @click="clearCache" class="action-btn secondary">
            <i class="fas fa-broom"></i>
            Xóa cache
          </button>
        </div>
      </div>
    </div>

    <!-- Sync Progress Toast (when syncing) -->
    <div v-if="offlineStore.syncInProgress" class="sync-toast">
      <div class="toast-content">
        <i class="fas fa-sync-alt spinning"></i>
        <span>Đang đồng bộ {{ offlineStore.pendingActions.length }} hành động...</span>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import { computed, ref } from 'vue'
import { useOfflineStore } from '../stores/offlineStore.js'

const offlineStore = useOfflineStore()
const showDetails = ref(false)

// Computed properties
const networkIcon = computed(() => {
  if (offlineStore.syncInProgress) return 'fas fa-sync-alt spinning'
  if (offlineStore.isOffline) return 'fas fa-wifi-slash'
  if (offlineStore.hasPendingActions) return 'fas fa-exclamation-triangle'
  return 'fas fa-wifi'
})

const networkStatus = computed(() => {
  if (offlineStore.syncInProgress) return 'Đang đồng bộ'
  if (offlineStore.isOffline) return 'Offline'
  if (offlineStore.hasPendingActions) return `${offlineStore.pendingActions.length} chờ`
  return 'Online'
})

const detailedStatus = computed(() => {
  if (offlineStore.isOffline) return 'Không có kết nối mạng'
  if (offlineStore.syncInProgress) return 'Đang đồng bộ dữ liệu'
  if (offlineStore.hasPendingActions) return 'Có dữ liệu chờ đồng bộ'
  return 'Kết nối ổn định'
})

const formattedLastSync = computed(() => {
  if (!offlineStore.lastSyncTime) return 'Chưa bao giờ'

  const date = new Date(offlineStore.lastSyncTime)
  const now = new Date()
  const diffMs = now - date
  const diffMinutes = Math.floor(diffMs / (1000 * 60))

  if (diffMinutes < 1) return 'Vừa xong'
  if (diffMinutes < 60) return `${diffMinutes} phút trước`

  const diffHours = Math.floor(diffMinutes / 60)
  if (diffHours < 24) return `${diffHours} giờ trước`

  const diffDays = Math.floor(diffHours / 24)
  return `${diffDays} ngày trước`
})

// Methods
const toggleDetails = () => {
  showDetails.value = !showDetails.value
}

const manualSync = async () => {
  try {
    const success = await offlineStore.manualSync()
    if (success) {
      showDetails.value = false
    }
  } catch (error) {
    console.error('Manual sync error:', error)
  }
}

const clearPending = () => {
  if (confirm('Bạn có chắc muốn xóa tất cả hành động chờ đồng bộ?')) {
    offlineStore.clearPendingActions()
    showDetails.value = false
  }
}

const clearCache = async () => {
  if (confirm('Bạn có chắc muốn xóa tất cả cache ứng dụng? Ứng dụng sẽ tải lại.')) {
    await offlineStore.clearAppCache()
    showDetails.value = false
    // Reload trang để tải lại cache
    setTimeout(() => {
      window.location.reload()
    }, 1000)
  }
}
</script>

<style scoped>
/* Offline Banner */
.offline-banner {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  background: linear-gradient(135deg, #dc3545, #c82333);
  color: white;
  z-index: 9998;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
  animation: slideDown 0.3s ease-out;
}

@keyframes slideDown {
  from {
    transform: translateY(-100%);
  }
  to {
    transform: translateY(0);
  }
}

.offline-content {
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 12px 20px;
  gap: 12px;
}

.offline-icon {
  font-size: 1.2rem;
}

.offline-text {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.offline-text strong {
  font-size: 0.9rem;
  font-weight: 600;
}

.offline-text span {
  font-size: 0.8rem;
  opacity: 0.9;
}

/* Sync Indicator */
.sync-indicator {
  position: fixed;
  top: 20px;
  left: 50%;
  transform: translateX(-50%);
  background: linear-gradient(135deg, #ffc107, #fd7e14);
  color: #212529;
  border-radius: 25px;
  padding: 8px 16px;
  z-index: 9997;
  box-shadow: 0 4px 16px rgba(255, 193, 7, 0.3);
  animation: slideDown 0.3s ease-out;
}

.sync-content {
  display: flex;
  align-items: center;
  gap: 10px;
}

.sync-icon {
  font-size: 1rem;
}

.sync-text {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 0.85rem;
}

.sync-btn {
  background: #212529;
  color: white;
  border: none;
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 0.75rem;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  gap: 4px;
}

.sync-btn:hover {
  background: #495057;
}

/* Network Widget */
.network-widget {
  position: fixed;
  bottom: 20px;
  left: 20px;
  background: linear-gradient(135deg, #28a745, #20c997);
  color: white;
  border-radius: 50px;
  z-index: 9996;
  box-shadow: 0 4px 16px rgba(40, 167, 69, 0.3);
  transition: all 0.3s ease;
  cursor: pointer;
}

.network-widget.offline {
  background: linear-gradient(135deg, #dc3545, #c82333);
  box-shadow: 0 4px 16px rgba(220, 53, 69, 0.3);
}

.network-content {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 10px 16px;
  transition: all 0.3s ease;
}

.network-widget:hover .network-content {
  transform: scale(1.05);
}

.network-text {
  font-size: 0.85rem;
  font-weight: 500;
}

/* Network Details Panel */
.network-details {
  position: absolute;
  bottom: 100%;
  left: 0;
  margin-bottom: 10px;
  background: white;
  color: #212529;
  border-radius: 12px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.2);
  border: 1px solid rgba(0, 0, 0, 0.1);
  min-width: 280px;
  animation: fadeInUp 0.3s ease-out;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.detail-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 20px 12px;
  border-bottom: 1px solid #e9ecef;
}

.detail-header h4 {
  margin: 0;
  font-size: 1rem;
  font-weight: 600;
}

.close-btn {
  background: none;
  border: none;
  color: #6c757d;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.3s ease;
}

.close-btn:hover {
  background: #f8f9fa;
  color: #495057;
}

.detail-body {
  padding: 12px 20px;
}

.detail-item {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 8px;
  font-size: 0.85rem;
}

.detail-item:last-child {
  margin-bottom: 0;
}

.detail-item i {
  width: 16px;
  text-align: center;
  color: #6c757d;
}

.detail-actions {
  padding: 12px 20px 16px;
  border-top: 1px solid #e9ecef;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.action-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  padding: 8px 12px;
  border: none;
  border-radius: 6px;
  font-size: 0.8rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
}

.action-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.action-btn.primary {
  background: #8b1538;
  color: white;
}

.action-btn.primary:hover:not(:disabled) {
  background: #a91b47;
}

.action-btn.danger {
  background: #dc3545;
  color: white;
}

.action-btn.danger:hover:not(:disabled) {
  background: #c82333;
}

.action-btn.secondary {
  background: #6c757d;
  color: white;
}

.action-btn.secondary:hover:not(:disabled) {
  background: #5a6268;
}

/* Sync Toast */
.sync-toast {
  position: fixed;
  bottom: 80px;
  left: 50%;
  transform: translateX(-50%);
  background: rgba(0, 0, 0, 0.8);
  color: white;
  border-radius: 25px;
  padding: 12px 20px;
  z-index: 9995;
  animation: fadeInUp 0.3s ease-out;
}

.toast-content {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 0.85rem;
}

/* Spinning animation */
.spinning {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

/* Responsive Design */
@media (max-width: 768px) {
  .network-widget {
    bottom: 80px;
    left: 10px;
  }

  .network-details {
    min-width: 250px;
  }

  .offline-content {
    padding: 10px 15px;
    gap: 10px;
  }

  .sync-indicator {
    left: 10px;
    right: 10px;
    transform: none;
  }

  .sync-toast {
    left: 10px;
    right: 10px;
    transform: none;
  }
}

@media (max-width: 480px) {
  .network-details {
    min-width: calc(100vw - 40px);
    left: -10px;
  }

  .offline-text {
    align-items: center;
    text-align: center;
  }

  .sync-content {
    flex-direction: column;
    gap: 6px;
    text-align: center;
  }
}
</style>
