import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { toast } from 'vue3-toastify';

export const useOfflineStore = defineStore('offline', () => {
  // Reactive states
  const isOnline = ref(navigator.onLine);
  const pendingActions = ref([]);
  const syncInProgress = ref(false);
  const lastSyncTime = ref(null);

  // Computed
  const isOffline = computed(() => !isOnline.value);
  const hasPendingActions = computed(() => pendingActions.value.length > 0);

  // Network status handlers
  const handleOnline = () => {
    console.log('🔧 Offline Store: Network is online');
    isOnline.value = true;
    toast.success('Đã kết nối mạng - Đang đồng bộ dữ liệu...', {
      autoClose: 2000
    });
    
    // Tự động sync khi có mạng
    if (hasPendingActions.value) {
      syncPendingActions();
    }
  };

  const handleOffline = () => {
    console.log('🔧 Offline Store: Network is offline');
    isOnline.value = false;
    toast.warning('Mất kết nối mạng - Ứng dụng hoạt động offline', {
      autoClose: 3000
    });
  };

  // Thêm action vào pending queue
  const addPendingAction = (action) => {
    console.log('🔧 Offline Store: Adding pending action:', action);
    
    const actionWithId = {
      id: Date.now() + Math.random(),
      timestamp: new Date().toISOString(),
      ...action
    };
    
    pendingActions.value.push(actionWithId);
    savePendingActionsToStorage();
    
    toast.info('Hành động được lưu để đồng bộ khi có mạng', {
      autoClose: 2000
    });
  };

  // Lưu pending actions vào localStorage
  const savePendingActionsToStorage = () => {
    try {
      localStorage.setItem('pendingActions', JSON.stringify(pendingActions.value));
    } catch (error) {
      console.error('🔧 Offline Store: Error saving pending actions:', error);
    }
  };

  // Load pending actions từ localStorage
  const loadPendingActionsFromStorage = () => {
    try {
      const stored = localStorage.getItem('pendingActions');
      if (stored) {
        pendingActions.value = JSON.parse(stored);
        console.log('🔧 Offline Store: Loaded pending actions:', pendingActions.value.length);
      }
    } catch (error) {
      console.error('🔧 Offline Store: Error loading pending actions:', error);
      pendingActions.value = [];
    }
  };

  // Sync pending actions
  const syncPendingActions = async () => {
    if (syncInProgress.value || !isOnline.value || !hasPendingActions.value) {
      return;
    }

    syncInProgress.value = true;
    const actionsToSync = [...pendingActions.value];
    let successCount = 0;
    let failedActions = [];

    console.log('🔧 Offline Store: Starting sync of', actionsToSync.length, 'actions');

    for (const action of actionsToSync) {
      try {
        const response = await fetch(action.url, {
          method: action.method || 'POST',
          headers: {
            'Content-Type': 'application/json',
            ...action.headers
          },
          body: action.body
        });

        if (response.ok) {
          successCount++;
          console.log('🔧 Offline Store: Sync success for action:', action.id);
        } else {
          throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }
      } catch (error) {
        console.error('🔧 Offline Store: Sync failed for action:', action.id, error);
        failedActions.push(action);
      }
    }

    // Cập nhật pending actions (chỉ giữ lại những action failed)
    pendingActions.value = failedActions;
    savePendingActionsToStorage();

    // Cập nhật thời gian sync
    lastSyncTime.value = new Date().toISOString();
    localStorage.setItem('lastSyncTime', lastSyncTime.value);

    syncInProgress.value = false;

    // Hiển thị kết quả sync
    if (successCount > 0) {
      toast.success(`Đã đồng bộ thành công ${successCount} hành động`, {
        autoClose: 3000
      });
    }

    if (failedActions.length > 0) {
      toast.error(`${failedActions.length} hành động không thể đồng bộ`, {
        autoClose: 3000
      });
    }

    console.log('🔧 Offline Store: Sync completed -', successCount, 'success,', failedActions.length, 'failed');
  };

  // Xóa pending action
  const removePendingAction = (actionId) => {
    pendingActions.value = pendingActions.value.filter(action => action.id !== actionId);
    savePendingActionsToStorage();
  };

  // Xóa tất cả pending actions
  const clearPendingActions = () => {
    pendingActions.value = [];
    savePendingActionsToStorage();
    toast.info('Đã xóa tất cả hành động chờ đồng bộ');
  };

  // Thử sync thủ công
  const manualSync = async () => {
    if (!isOnline.value) {
      toast.warning('Không có kết nối mạng để đồng bộ');
      return false;
    }

    await syncPendingActions();
    return true;
  };

  // Load last sync time từ localStorage
  const loadLastSyncTime = () => {
    try {
      const stored = localStorage.getItem('lastSyncTime');
      if (stored) {
        lastSyncTime.value = stored;
      }
    } catch (error) {
      console.error('🔧 Offline Store: Error loading last sync time:', error);
    }
  };

  // Helper function để tạo offline-safe API request
  const offlineRequest = async (url, options = {}) => {
    if (isOnline.value) {
      try {
        const response = await fetch(url, options);
        return response;
      } catch (error) {
        console.error('🔧 Offline Store: Network request failed:', error);
        // Nếu request fail, thêm vào pending queue
        addPendingAction({
          url,
          method: options.method || 'GET',
          headers: options.headers,
          body: options.body
        });
        throw error;
      }
    } else {
      // Offline - thêm vào pending queue
      addPendingAction({
        url,
        method: options.method || 'GET',
        headers: options.headers,
        body: options.body
      });
      
      throw new Error('Offline - Action queued for sync');
    }
  };

  // Cache management
  const clearAppCache = async () => {
    try {
      if ('caches' in window) {
        const cacheNames = await caches.keys();
        await Promise.all(
          cacheNames.map(cacheName => caches.delete(cacheName))
        );
        toast.success('Đã xóa cache ứng dụng');
      }
    } catch (error) {
      console.error('🔧 Offline Store: Error clearing cache:', error);
      toast.error('Lỗi khi xóa cache');
    }
  };

  // Initialize
  const initialize = () => {
    // Load data từ storage
    loadPendingActionsFromStorage();
    loadLastSyncTime();

    // Setup network event listeners
    window.addEventListener('online', handleOnline);
    window.addEventListener('offline', handleOffline);

    // Initial sync nếu có pending actions và online
    if (isOnline.value && hasPendingActions.value) {
      setTimeout(() => {
        syncPendingActions();
      }, 2000); // Delay 2 giây để ứng dụng khởi tạo hoàn toàn
    }

    console.log('🔧 Offline Store: Initialized');
  };

  // Cleanup
  const cleanup = () => {
    window.removeEventListener('online', handleOnline);
    window.removeEventListener('offline', handleOffline);
  };

  return {
    // State
    isOnline,
    isOffline,
    pendingActions,
    syncInProgress,
    lastSyncTime,
    hasPendingActions,

    // Actions
    addPendingAction,
    syncPendingActions,
    removePendingAction,
    clearPendingActions,
    manualSync,
    offlineRequest,
    clearAppCache,
    initialize,
    cleanup
  };
});
