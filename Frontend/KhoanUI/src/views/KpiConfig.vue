<template>
  <div class="kpi-config-container">
    <!-- Header -->
    <div class="page-header">
      <h2 class="page-title">
        <i class="fas fa-chart-line"></i>
        Cấu hình KPI
      </h2>
      <p class="page-subtitle">Quản lý cấu hình KPI cho cán bộ và chi nhánh</p>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="loading-container">
      <div class="spinner"></div>
      <p>Đang tải dữ liệu KPI...</p>
    </div>

    <!-- Main Content -->
    <div v-else class="kpi-config-content">
      <!-- Tabs Navigation -->
      <div class="tabs-navigation">
        <button :class="['tab-btn', { active: activeTab === 'canbo' }]" @click="switchTab('canbo')">
          <i class="fas fa-users"></i>
          Dành cho Cán bộ
          <span class="tab-count">({{ canboTables.length }})</span>
        </button>
        <button :class="['tab-btn', { active: activeTab === 'chinhanh' }]" @click="switchTab('chinhanh')">
          <i class="fas fa-building"></i>
          Dành cho Chi nhánh
          <span class="tab-count">({{ chinhanhTables.length }})</span>
        </button>
      </div>

      <!-- Tab Content -->
      <div class="tab-content">
        <!-- Tab Cán bộ -->
        <div v-if="activeTab === 'canbo'" class="canbo-tab">
          <div class="tab-header">
            <h3>
              <i class="fas fa-user-tie"></i>
              Bảng KPI Cán bộ
            </h3>
            <div class="tab-stats">
              <span class="stat-item">
                <i class="fas fa-table"></i>
                {{ canboTables.length }} bảng
              </span>
              <span class="stat-item">
                <i class="fas fa-list-ol"></i>
                {{ getTotalIndicators('CANBO') }} chỉ tiêu
              </span>
            </div>
          </div>

          <!-- Danh sách bảng KPI cán bộ -->
          <div class="kpi-tables-grid">
            <div v-for="table in canboTables" :key="table.Id" class="kpi-table-card" @click="viewTableDetails(table)">
              <div class="card-header">
                <h4 class="table-name">{{ table.Description }}</h4>
                <span class="table-type">{{ table.TableName }}</span>
              </div>
              <div class="card-body">
                <div class="indicator-count">
                  <i class="fas fa-chart-bar"></i>
                  <span>{{ table.IndicatorCount || 0 }} chỉ tiêu</span>
                </div>
                <div class="card-actions">
                  <button class="btn-view" @click.stop="viewIndicators(table)">
                    <i class="fas fa-eye"></i>
                    Xem chi tiết
                  </button>
                  <button class="btn-edit" @click.stop="editTable(table)">
                    <i class="fas fa-edit"></i>
                    Chỉnh sửa
                  </button>
                </div>
              </div>
              <div class="card-status">
                <span :class="['status-badge', table.IsActive ? 'active' : 'inactive']">
                  {{ table.IsActive ? 'Hoạt động' : 'Tạm dừng' }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Tab Chi nhánh -->
        <div v-if="activeTab === 'chinhanh'" class="chinhanh-tab">
          <div class="tab-header">
            <h3>
              <i class="fas fa-building"></i>
              Bảng KPI Chi nhánh
            </h3>
            <div class="tab-stats">
              <span class="stat-item">
                <i class="fas fa-table"></i>
                {{ chinhanhTables.length }} bảng
              </span>
              <span class="stat-item">
                <i class="fas fa-list-ol"></i>
                {{ getTotalIndicators('CHINHANH') }} chỉ tiêu
              </span>
            </div>
          </div>

          <!-- Danh sách bảng KPI chi nhánh -->
          <div class="kpi-tables-grid">
            <div
              v-for="table in chinhanhTables"
              :key="table.Id"
              class="kpi-table-card branch-card"
              @click="viewTableDetails(table)"
            >
              <div class="card-header">
                <h4 class="table-name">{{ table.Description }}</h4>
                <span class="table-type">{{ table.TableName }}</span>
              </div>
              <div class="card-body">
                <div class="indicator-count">
                  <i class="fas fa-chart-line"></i>
                  <span>{{ table.IndicatorCount || 0 }} chỉ tiêu</span>
                </div>
                <div class="card-actions">
                  <button class="btn-view" @click.stop="viewIndicators(table)">
                    <i class="fas fa-eye"></i>
                    Xem chi tiết
                  </button>
                  <button class="btn-edit" @click.stop="editTable(table)">
                    <i class="fas fa-edit"></i>
                    Chỉnh sửa
                  </button>
                </div>
              </div>
              <div class="card-status">
                <span :class="['status-badge', table.IsActive ? 'active' : 'inactive']">
                  {{ table.IsActive ? 'Hoạt động' : 'Tạm dừng' }}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal xem chi tiết chỉ tiêu -->
    <div v-if="showIndicatorModal" class="modal-overlay" @click="closeIndicatorModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>
            <i class="fas fa-list-ol"></i>
            Chỉ tiêu KPI - {{ selectedTable?.Description }}
          </h3>
          <button class="btn-close" @click="closeIndicatorModal">
            <i class="fas fa-times"></i>
          </button>
        </div>
        <div class="modal-body">
          <div v-if="loadingIndicators" class="loading-indicators">
            <div class="spinner-small"></div>
            <p>Đang tải chỉ tiêu...</p>
          </div>
          <div v-else-if="tableIndicators.length === 0" class="no-indicators">
            <i class="fas fa-info-circle"></i>
            <p>Chưa có chỉ tiêu KPI nào được cấu hình.</p>
          </div>
          <div v-else class="indicators-list">
            <div v-for="indicator in tableIndicators" :key="indicator.Id" class="indicator-item">
              <div class="indicator-info">
                <div class="indicator-name">
                  <span class="order-index">{{ indicator.OrderIndex }}.</span>
                  {{ indicator.IndicatorName }}
                </div>
                <div class="indicator-details">
                  <span class="max-score">
                    <i class="fas fa-star"></i>
                    {{ indicator.MaxScore }} điểm
                  </span>
                  <span class="unit">
                    <i class="fas fa-tags"></i>
                    {{ indicator.Unit }}
                  </span>
                  <span class="value-type">
                    <i class="fas fa-chart-bar"></i>
                    {{ getValueTypeLabel(indicator.ValueType) }}
                  </span>
                </div>
              </div>
              <div class="indicator-status">
                <span :class="['status-badge', indicator.IsActive ? 'active' : 'inactive']">
                  {{ indicator.IsActive ? 'Hoạt động' : 'Tạm dừng' }}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { computed, onMounted, ref } from 'vue'

export default {
  name: 'KpiConfig',
  setup() {
    // Reactive data
    const loading = ref(true)
    const activeTab = ref('canbo')
    const allTables = ref([])
    const tableIndicators = ref([])
    const selectedTable = ref(null)
    const showIndicatorModal = ref(false)
    const loadingIndicators = ref(false)

    // Computed properties
    const canboTables = computed(() => allTables.value.filter(table => table.Category === 'CANBO'))

    const chinhanhTables = computed(() => allTables.value.filter(table => table.Category === 'CHINHANH'))

    // Methods
    const loadKpiTables = async () => {
      try {
        loading.value = true
        const response = await fetch('/api/KpiAssignmentTables')

        if (response.ok) {
          const data = await response.json()
          allTables.value = data
          console.log('📊 Loaded KPI tables:', data.length)
        } else {
          throw new Error(`HTTP ${response.status}`)
        }
      } catch (error) {
        console.error('❌ Error loading KPI tables:', error)
        alert('Lỗi khi tải danh sách bảng KPI: ' + error.message)
      } finally {
        loading.value = false
      }
    }

    const switchTab = tab => {
      activeTab.value = tab
    }

    const getTotalIndicators = category => {
      const tables = allTables.value.filter(table => table.Category === category)
      return tables.reduce((total, table) => total + (table.IndicatorCount || 0), 0)
    }

    const viewTableDetails = table => {
      console.log('📋 View table details:', table.TableName)
      // TODO: Navigate to table detail page
    }

    const viewIndicators = async table => {
      try {
        selectedTable.value = table
        showIndicatorModal.value = true
        loadingIndicators.value = true
        tableIndicators.value = []

        console.log('🔍 Loading indicators for table:', table.Id)

        const response = await fetch(`/api/KpiIndicators?tableId=${table.Id}`)

        if (response.ok) {
          const data = await response.json()
          tableIndicators.value = data.filter(indicator => indicator.TableId === table.Id)
          console.log('📊 Loaded indicators:', tableIndicators.value.length)
        } else {
          throw new Error(`HTTP ${response.status}`)
        }
      } catch (error) {
        console.error('❌ Error loading indicators:', error)
        alert('Lỗi khi tải chỉ tiêu KPI: ' + error.message)
      } finally {
        loadingIndicators.value = false
      }
    }

    const editTable = table => {
      console.log('✏️ Edit table:', table.TableName)
      // TODO: Open edit modal
    }

    const closeIndicatorModal = () => {
      showIndicatorModal.value = false
      selectedTable.value = null
      tableIndicators.value = []
    }

    const getValueTypeLabel = valueType => {
      const types = {
        NUMBER: 'Số',
        PERCENTAGE: 'Phần trăm',
        POINTS: 'Điểm',
        CURRENCY: 'Tiền tệ',
      }
      return types[valueType] || valueType
    }

    // Lifecycle
    onMounted(() => {
      loadKpiTables()
    })

    return {
      // Data
      loading,
      activeTab,
      allTables,
      tableIndicators,
      selectedTable,
      showIndicatorModal,
      loadingIndicators,

      // Computed
      canboTables,
      chinhanhTables,

      // Methods
      switchTab,
      getTotalIndicators,
      viewTableDetails,
      viewIndicators,
      editTable,
      closeIndicatorModal,
      getValueTypeLabel,
    }
  },
}
</script>

<style scoped>
/* Container chính */
.kpi-config-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}

/* Header */
.page-header {
  text-align: center;
  margin-bottom: 30px;
  color: white;
}

.page-title {
  font-size: 2.5rem;
  font-weight: 700;
  margin-bottom: 10px;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.3);
}

.page-title i {
  margin-right: 15px;
  color: #ffd700;
}

.page-subtitle {
  font-size: 1.2rem;
  opacity: 0.9;
  font-weight: 300;
}

/* Loading */
.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 400px;
  color: white;
}

.spinner {
  width: 60px;
  height: 60px;
  border: 4px solid rgba(255, 255, 255, 0.3);
  border-top: 4px solid #fff;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 20px;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

/* Content */
.kpi-config-content {
  max-width: 1400px;
  margin: 0 auto;
  background: white;
  border-radius: 20px;
  padding: 30px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
}

/* Tabs Navigation */
.tabs-navigation {
  display: flex;
  margin-bottom: 30px;
  border-bottom: 3px solid #f0f0f0;
}

.tab-btn {
  flex: 1;
  padding: 15px 25px;
  border: none;
  background: transparent;
  font-size: 1.1rem;
  font-weight: 600;
  color: #666;
  cursor: pointer;
  border-bottom: 3px solid transparent;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
}

.tab-btn:hover {
  color: #4caf50;
  background: rgba(76, 175, 80, 0.05);
}

.tab-btn.active {
  color: #4caf50;
  border-bottom-color: #4caf50;
  background: rgba(76, 175, 80, 0.1);
}

.tab-count {
  background: #4caf50;
  color: white;
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 0.9rem;
  font-weight: 500;
}

/* Tab Content */
.tab-content {
  min-height: 500px;
}

.tab-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 25px;
  padding-bottom: 15px;
  border-bottom: 2px solid #f0f0f0;
}

.tab-header h3 {
  font-size: 1.5rem;
  color: #333;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 10px;
}

.tab-stats {
  display: flex;
  gap: 20px;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #666;
  font-weight: 500;
}

.stat-item i {
  color: #4caf50;
}

/* KPI Tables Grid */
.kpi-tables-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 20px;
}

.kpi-table-card {
  background: white;
  border: 2px solid #e0e0e0;
  border-radius: 15px;
  padding: 20px;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.kpi-table-card:hover {
  border-color: #4caf50;
  box-shadow: 0 8px 25px rgba(76, 175, 80, 0.15);
  transform: translateY(-2px);
}

.kpi-table-card.branch-card:hover {
  border-color: #2196f3;
  box-shadow: 0 8px 25px rgba(33, 150, 243, 0.15);
}

.card-header {
  margin-bottom: 15px;
}

.table-name {
  font-size: 1.2rem;
  font-weight: 700;
  color: #333;
  margin: 0 0 8px 0;
  line-height: 1.3;
}

.table-type {
  background: #f0f0f0;
  color: #666;
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 500;
}

.card-body {
  margin-bottom: 15px;
}

.indicator-count {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 15px;
  color: #666;
  font-weight: 500;
}

.indicator-count i {
  color: #4caf50;
}

.card-actions {
  display: flex;
  gap: 10px;
}

.btn-view,
.btn-edit {
  flex: 1;
  padding: 8px 12px;
  border: none;
  border-radius: 8px;
  font-size: 0.9rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
}

.btn-view {
  background: #2196f3;
  color: white;
}

.btn-view:hover {
  background: #1976d2;
}

.btn-edit {
  background: #ff9800;
  color: white;
}

.btn-edit:hover {
  background: #f57c00;
}

.card-status {
  position: absolute;
  top: 15px;
  right: 15px;
}

.status-badge {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 0.8rem;
  font-weight: 600;
}

.status-badge.active {
  background: #4caf50;
  color: white;
}

.status-badge.inactive {
  background: #f44336;
  color: white;
}

/* Modal */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 20px;
}

.modal-content {
  background: white;
  border-radius: 15px;
  max-width: 800px;
  width: 100%;
  max-height: 80vh;
  overflow: hidden;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.3);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 25px;
  border-bottom: 2px solid #f0f0f0;
  background: #f8f9fa;
}

.modal-header h3 {
  margin: 0;
  color: #333;
  display: flex;
  align-items: center;
  gap: 10px;
}

.btn-close {
  background: #f44336;
  color: white;
  border: none;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.3s ease;
}

.btn-close:hover {
  background: #d32f2f;
}

.modal-body {
  padding: 25px;
  max-height: 60vh;
  overflow-y: auto;
}

.loading-indicators {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 40px 20px;
  color: #666;
}

.spinner-small {
  width: 40px;
  height: 40px;
  border: 3px solid #f0f0f0;
  border-top: 3px solid #4caf50;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 15px;
}

.no-indicators {
  text-align: center;
  padding: 40px 20px;
  color: #666;
}

.no-indicators i {
  font-size: 3rem;
  color: #ddd;
  margin-bottom: 15px;
}

.indicators-list {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.indicator-item {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 15px;
  border: 2px solid #f0f0f0;
  border-radius: 10px;
  transition: all 0.3s ease;
}

.indicator-item:hover {
  border-color: #4caf50;
  background: rgba(76, 175, 80, 0.05);
}

.indicator-info {
  flex: 1;
}

.indicator-name {
  font-size: 1.1rem;
  font-weight: 600;
  color: #333;
  margin-bottom: 8px;
  display: flex;
  align-items: center;
  gap: 10px;
}

.order-index {
  background: #4caf50;
  color: white;
  padding: 2px 8px;
  border-radius: 50%;
  font-size: 0.9rem;
  font-weight: 700;
  min-width: 24px;
  text-align: center;
}

.indicator-details {
  display: flex;
  gap: 15px;
  flex-wrap: wrap;
}

.indicator-details span {
  display: flex;
  align-items: center;
  gap: 5px;
  font-size: 0.9rem;
  color: #666;
}

.max-score i {
  color: #ff9800;
}

.unit i {
  color: #2196f3;
}

.value-type i {
  color: #9c27b0;
}

.indicator-status {
  margin-left: 15px;
}

/* Responsive */
@media (max-width: 768px) {
  .kpi-config-container {
    padding: 10px;
  }

  .kpi-config-content {
    padding: 20px;
  }

  .page-title {
    font-size: 2rem;
  }

  .kpi-tables-grid {
    grid-template-columns: 1fr;
  }

  .tab-btn {
    font-size: 1rem;
    padding: 12px 20px;
  }

  .tab-header {
    flex-direction: column;
    gap: 15px;
    align-items: flex-start;
  }

  .card-actions {
    flex-direction: column;
  }

  .modal-content {
    margin: 20px;
    max-height: 90vh;
  }

  .indicator-details {
    flex-direction: column;
    gap: 8px;
  }
}
</style>
