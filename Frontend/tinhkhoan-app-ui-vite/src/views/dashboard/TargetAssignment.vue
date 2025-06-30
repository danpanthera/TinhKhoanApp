<template>
  <div class="target-assignment">
    <!-- Header với controls -->
    <div class="page-header">
      <div class="header-title">
        <h2>
          <i class="mdi mdi-target"></i>
          Giao Chỉ Tiêu Kế Hoạch Kinh Doanh
        </h2>
        <p class="subtitle">Quản lý và phân bổ chỉ tiêu cho các đơn vị</p>
      </div>

      <div class="header-controls">
        <!-- Chọn đơn vị/phòng ban - 15 chi nhánh chuẩn hóa -->
        <select v-model="selectedUnitId" @change="loadTargets" class="form-select" style="min-width: 200px;">
          <option value="">Tất cả đơn vị (Toàn tỉnh)</option>
          <option value="HoiSo">Hội Sở</option>
          <option value="CnTamDuong">CN Tam Đường</option>
          <option value="CnPhongTho">CN Phong Thổ</option>
          <option value="CnSinHo">CN Sin Hồ</option>
          <option value="CnMuongTe">CN Mường Tè</option>
          <option value="CnThanUyen">CN Than Uyên</option>
          <option value="CnThanhPho">CN Thành Phố</option>
          <option value="CnTanUyen">CN Tân Uyên</option>
          <option value="CnNamNhun">CN Nậm Nhùn</option>
          <option value="CnPhongThoPgdMuongSo">CN Phong Thổ - PGD Mường So</option>
          <option value="CnThanUyenPgdMuongThan">CN Than Uyên - PGD Mường Than</option>
          <option value="CnThanhPhoPgdSo1">CN Thành Phố - PGD Số 1</option>
          <option value="CnThanhPhoPgdSo2">CN Thành Phố - PGD Số 2</option>
          <option value="CnTanUyenPgdSo3">CN Tân Uyên - PGD Số 3</option>
        </select>

        <!-- Chọn năm -->
        <select v-model="selectedYear" @change="loadTargets" class="form-select">
          <option value="">Chọn năm</option>
          <option v-for="year in yearOptions" :key="year" :value="year">
            {{ year }}
          </option>
        </select>

        <!-- Chọn kỳ -->
        <select v-model="periodType" @change="onPeriodTypeChange" class="form-select">
          <option value="">Chọn loại kỳ</option>
          <option v-for="period in periodTypeOptions" :key="period.value" :value="period.value">
            {{ period.label }}
          </option>
        </select>

        <!-- Chọn quý/tháng tùy theo loại kỳ -->
        <select v-if="periodType === 'QUARTER'" v-model="selectedPeriod" @change="loadTargets" class="form-select">
          <option value="">Chọn quý</option>
          <option v-for="quarter in quarterOptions" :key="quarter.value" :value="quarter.value">
            {{ quarter.label }}
          </option>
        </select>

        <select v-if="periodType === 'MONTH'" v-model="selectedPeriod" @change="loadTargets" class="form-select">
          <option value="">Chọn tháng</option>
          <option v-for="month in monthOptions" :key="month.value" :value="month.value">
            {{ month.label }}
          </option>
        </select>

        <!-- Nút actions -->
        <button @click="loadTargets" :disabled="loading" class="btn btn-primary">
          {{ loading ? 'Đang tải...' : '🔄 Tải lại' }}
        </button>

        <button @click="showCreateModal = true" class="btn btn-success">
          ➕ Thêm chỉ tiêu
        </button>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="loading-section">
      <div class="loading-spinner"></div>
      <p>Đang tải dữ liệu chỉ tiêu...</p>
    </div>

    <!-- Error Message -->
    <div v-if="errorMessage" class="error-message">
      <p>❌ {{ errorMessage }}</p>
    </div>

    <!-- Success Message -->
    <div v-if="successMessage" class="success-message">
      <p>✅ {{ successMessage }}</p>
    </div>

    <!-- Bảng chỉ tiêu cho đơn vị được chọn -->
    <div v-if="!loading && selectedUnitId" class="targets-section">
      <div class="section-header">
        <h3>
          <i class="mdi mdi-chart-line"></i>
          Chỉ tiêu cho {{ getSelectedUnitName() }}
        </h3>
      </div>

      <!-- Bảng chỉ tiêu -->
      <div class="targets-table-container">
        <table v-if="filteredTargets.length > 0" class="targets-table">
          <thead>
            <tr>
              <th>STT</th>
              <th>Tên chỉ tiêu</th>
              <th>Giá trị mục tiêu</th>
              <th>Đơn vị tính</th>
              <th>Kỳ</th>
              <th>Loại kỳ</th>
              <th>Trạng thái</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(target, index) in filteredTargets" :key="target.id">
              <td class="text-center">{{ index + 1 }}</td>
              <td>{{ target.indicatorName }}</td>
              <td class="number-cell">{{ formatNumber(target.targetValue) }}</td>
              <td>{{ target.unit || 'VND' }}</td>
              <td>{{ target.period }}</td>
              <td>{{ getPeriodTypeLabel(target.periodType) }}</td>
              <td>
                <span :class="['status-badge', target.isActive ? 'active' : 'inactive']">
                  {{ target.isActive ? 'Hoạt động' : 'Không hoạt động' }}
                </span>
              </td>
              <td>
                <div class="action-buttons">
                  <button @click="editTarget(target)" class="btn btn-sm btn-warning">
                    ✏️ Sửa
                  </button>
                  <button @click="deleteTarget(target.id)" class="btn btn-sm btn-danger">
                    🗑️ Xóa
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>

        <div v-else class="no-data">
          <div class="no-data-icon">📊</div>
          <h4>Chưa có dữ liệu</h4>
          <p>Chưa có chỉ tiêu nào được giao cho đơn vị này trong kỳ đã chọn.</p>
          <button @click="showCreateModal = true" class="btn btn-primary">
            ➕ Thêm chỉ tiêu đầu tiên
          </button>
        </div>
      </div>
    </div>

    <!-- Thông báo chọn đơn vị -->
    <div v-if="!loading && !selectedUnitId" class="select-unit-message">
      <div class="message-icon">🏢</div>
      <h3>Vui lòng chọn đơn vị</h3>
      <p>Hãy chọn đơn vị/phòng ban để xem và quản lý chỉ tiêu</p>
    </div>

    <!-- Modal tạo/sửa chỉ tiêu -->
    <div v-if="showCreateModal || showEditModal" class="modal-overlay" @click="closeModals">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>{{ showEditModal ? '✏️ Sửa chỉ tiêu' : '➕ Thêm chỉ tiêu mới' }}</h3>
          <button @click="closeModals" class="close-btn">❌</button>
        </div>

        <div class="modal-body">
          <form @submit.prevent="saveTarget">
            <div class="form-group">
              <label>Tên chỉ tiêu *</label>
              <select
                v-model="targetForm.indicatorName"
                class="form-select"
                required
              >
                <option value="">Chọn chỉ tiêu</option>
                <option v-for="indicator in businessIndicators" :key="indicator.value" :value="indicator.value">
                  {{ indicator.label }}
                </option>
              </select>
            </div>

            <div class="form-group">
              <label>Đơn vị/Phòng ban *</label>
              <select v-model="targetForm.unitId" class="form-select" required>
                <option value="">Tất cả đơn vị (Toàn tỉnh)</option>
                <option value="HoiSo">Hội Sở</option>
                <option value="CnTamDuong">CN Tam Đường</option>
                <option value="CnPhongTho">CN Phong Thổ</option>
                <option value="CnSinHo">CN Sin Hồ</option>
                <option value="CnMuongTe">CN Mường Tè</option>
                <option value="CnThanUyen">CN Than Uyên</option>
                <option value="CnThanhPho">CN Thành Phố</option>
                <option value="CnTanUyen">CN Tân Uyên</option>
                <option value="CnNamNhun">CN Nậm Nhùn</option>
                <option value="CnPhongThoPgdMuongSo">CN Phong Thổ - PGD Mường So</option>
                <option value="CnThanUyenPgdMuongThan">CN Than Uyên - PGD Mường Than</option>
                <option value="CnThanhPhoPgdSo1">CN Thành Phố - PGD Số 1</option>
                <option value="CnThanhPhoPgdSo2">CN Thành Phố - PGD Số 2</option>
                <option value="CnTanUyenPgdSo3">CN Tân Uyên - PGD Số 3</option>
              </select>
            </div>

            <div class="form-group">
              <label>Năm *</label>
              <select v-model="targetForm.year" class="form-select" required>
                <option value="">Chọn năm</option>
                <option v-for="year in yearOptions" :key="year" :value="year">
                  {{ year }}
                </option>
              </select>
            </div>

            <div class="form-group">
              <label>Loại kỳ *</label>
              <select v-model="targetForm.periodType" @change="onFormPeriodTypeChange" class="form-select" required>
                <option value="">Chọn loại kỳ</option>
                <option v-for="period in periodTypeOptions" :key="period.value" :value="period.value">
                  {{ period.label }}
                </option>
              </select>
            </div>

            <div v-if="targetForm.periodType === 'QUARTER'" class="form-group">
              <label>Quý *</label>
              <select v-model="targetForm.period" class="form-select" required>
                <option value="">Chọn quý</option>
                <option v-for="quarter in quarterOptions" :key="quarter.value" :value="quarter.value">
                  {{ quarter.label }}
                </option>
              </select>
            </div>

            <div v-if="targetForm.periodType === 'MONTH'" class="form-group">
              <label>Tháng *</label>
              <select v-model="targetForm.period" class="form-select" required>
                <option value="">Chọn tháng</option>
                <option v-for="month in monthOptions" :key="month.value" :value="month.value">
                  {{ month.label }}
                </option>
              </select>
            </div>

            <div class="form-group">
              <label>Giá trị mục tiêu *</label>
              <input
                v-model="targetForm.targetValueFormatted"
                @input="(e) => onTargetValueInput(e)"
                @blur="(e) => onTargetValueBlur(e)"
                type="text"
                class="form-input number-input"
                required
                placeholder="Nhập giá trị mục tiêu (VD: 1,000,000,000)"
                autocomplete="off"
              />
              <small class="form-hint">Số sẽ được tự động định dạng khi nhập. Dấu "," ngăn cách hàng nghìn, dấu "." cho thập phân</small>
            </div>

            <div class="form-group">
              <label>Đơn vị tính</label>
              <input
                v-model="targetForm.unit"
                type="text"
                class="form-input"
                placeholder="VD: VND, %, lần, ..."
              />
            </div>

            <div class="form-group">
              <label class="checkbox-label">
                <input
                  v-model="targetForm.isActive"
                  type="checkbox"
                />
                Kích hoạt chỉ tiêu
              </label>
            </div>

            <div class="form-actions">
              <button type="button" @click="closeModals" class="btn btn-secondary">
                Hủy
              </button>
              <button type="submit" :disabled="saving" class="btn btn-primary">
                {{ saving ? 'Đang lưu...' : (showEditModal ? 'Cập nhật' : 'Tạo mới') }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Action buttons cuối trang -->
    <div v-if="!loading" class="action-buttons">
      <button @click="exportData" class="btn btn-info">
        📊 Xuất Excel
      </button>
      <button @click="showBulkImportModal = true" class="btn btn-warning">
        📁 Import từ Excel
      </button>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import { isAuthenticated } from '../../services/auth';
import { dashboardService } from '../../services/dashboardService';
import { useNumberInput } from '../../utils/numberFormat';

const router = useRouter();

// 🔢 Initialize number input utility
const { handleInput, handleBlur, formatNumber, parseFormattedNumber } = useNumberInput({
  maxDecimalPlaces: 2,
  allowNegative: false
});

// Reactive data
const loading = ref(false);
const saving = ref(false);
const errorMessage = ref('');
const successMessage = ref('');
const showCreateModal = ref(false);
const showEditModal = ref(false);
const showBulkImportModal = ref(false);

// Form data
const targetForm = ref({
  indicatorName: '',
  unitId: '',
  year: new Date().getFullYear(),
  periodType: '',
  period: '',
  targetValue: '',
  targetValueFormatted: '',
  unit: 'VND',
  isActive: true
});

// Data
const targets = ref([]);
const units = ref([
  { id: 'HO', unitName: 'Hội sở', name: 'Hội sở' },
  { id: 'CN_HCM', unitName: 'Chi nhánh TP.HCM', name: 'Chi nhánh TP.HCM' },
  { id: 'CN_HN', unitName: 'Chi nhánh Hà Nội', name: 'Chi nhánh Hà Nội' },
  { id: 'CN_DN', unitName: 'Chi nhánh Đà Nẵng', name: 'Chi nhánh Đà Nẵng' },
  { id: 'CN_CT', unitName: 'Chi nhánh Cần Thơ', name: 'Chi nhánh Cần Thơ' },
  { id: 'CN_HP', unitName: 'Chi nhánh Hải Phòng', name: 'Chi nhánh Hải Phòng' }
]);
const selectedUnitId = ref('');
const selectedYear = ref(new Date().getFullYear());
const periodType = ref('');
const selectedPeriod = ref('');
const editingTarget = ref(null);

// Options
const yearOptions = ref(dashboardService.getYearOptions());
const quarterOptions = ref(dashboardService.getQuarterOptions());
const monthOptions = ref(dashboardService.getMonthOptions());
const periodTypeOptions = ref(dashboardService.getPeriodTypeOptions());

// Danh sách 6 chỉ tiêu kinh doanh cố định
const businessIndicators = ref([
  { value: 'Nguồn vốn', label: '1. Nguồn vốn' },
  { value: 'Dư nợ', label: '2. Dư nợ' },
  { value: 'Tỷ lệ nợ xấu', label: '3. Tỷ lệ nợ xấu' },
  { value: 'Thu nợ đã XLRR', label: '4. Thu nợ đã XLRR' },
  { value: 'Thu dịch vụ', label: '5. Thu dịch vụ' },
  { value: 'Lợi nhuận khoán tài chính', label: '6. Lợi nhuận khoán tài chính' }
]);

// Computed
const filteredTargets = computed(() => {
  if (!selectedUnitId.value) return [];

  return targets.value.filter(target => {
    let matches = target.unitId === selectedUnitId.value;

    if (selectedYear.value) {
      matches = matches && target.year === selectedYear.value;
    }

    if (periodType.value) {
      matches = matches && target.periodType === periodType.value;
    }

    if (selectedPeriod.value && periodType.value !== 'YEAR') {
      matches = matches && target.period === selectedPeriod.value;
    }

    return matches;
  });
});

// Methods
const loadTargets = async () => {
  if (!selectedYear.value) return;

  loading.value = true;
  errorMessage.value = '';

  try {
    const params = {
      year: selectedYear.value
    };

    if (periodType.value) {
      params.periodType = periodType.value;
    }

    if (selectedPeriod.value && periodType.value !== 'YEAR') {
      params.period = selectedPeriod.value;
    }

    if (selectedUnitId.value) {
      params.unitId = selectedUnitId.value;
    }

    const response = await dashboardService.getTargets(params);
    targets.value = response || [];
  } catch (error) {
    console.error('Error loading targets:', error);
    errorMessage.value = 'Không thể tải dữ liệu chỉ tiêu';
  } finally {
    loading.value = false;
  }
};

const getSelectedUnitName = () => {
  const unitMap = {
    'HO': 'Hội sở',
    'CN_HCM': 'Chi nhánh TP.HCM',
    'CN_HN': 'Chi nhánh Hà Nội',
    'CN_DN': 'Chi nhánh Đà Nẵng',
    'CN_CT': 'Chi nhánh Cần Thơ',
    'CN_HP': 'Chi nhánh Hải Phòng'
  };
  return unitMap[selectedUnitId.value] || '';
};

const getPeriodTypeLabel = (type) => {
  const option = periodTypeOptions.value.find(o => o.value === type);
  return option ? option.label : type;
};

const formatNumber = (value) => {
  if (!value && value !== 0) return '';
  return Number(value).toLocaleString('vi-VN');
};

// Number input handlers using the new utility
const onTargetValueInput = (event) => {
  const formattedValue = handleInput(event);
  targetForm.value.targetValueFormatted = formattedValue;
  // Store the numeric value for backend
  targetForm.value.targetValue = parseFormattedNumber(formattedValue);
};

const onTargetValueBlur = (event) => {
  const formattedValue = handleBlur(event);
  targetForm.value.targetValueFormatted = formattedValue;
  targetForm.value.targetValue = parseFormattedNumber(formattedValue);
};

const onPeriodTypeChange = () => {
  selectedPeriod.value = '';
  loadTargets();
};

const onFormPeriodTypeChange = () => {
  targetForm.value.period = '';
};

const editTarget = (target) => {
  editingTarget.value = target;
  targetForm.value = {
    indicatorName: target.indicatorName,
    unitId: target.unitId,
    year: target.year,
    periodType: target.periodType,
    period: target.period,
    targetValue: target.targetValue,
    targetValueFormatted: formatNumber(target.targetValue),
    unit: target.unit,
    isActive: target.isActive
  };
  showEditModal.value = true;
};

const saveTarget = async () => {
  saving.value = true;
  errorMessage.value = '';
  successMessage.value = '';

  try {
    if (showEditModal.value && editingTarget.value) {
      // Update existing target
      await dashboardService.updateTarget(editingTarget.value.id, targetForm.value);
      successMessage.value = 'Cập nhật chỉ tiêu thành công';
    } else {
      // Create new target
      await dashboardService.createTarget(targetForm.value);
      successMessage.value = 'Tạo chỉ tiêu mới thành công';
    }

    closeModals();
    await loadTargets();
  } catch (error) {
    console.error('Error saving target:', error);
    errorMessage.value = 'Có lỗi xảy ra khi lưu chỉ tiêu: ' + (error.response?.data?.message || error.message);
  } finally {
    saving.value = false;
  }
};

const deleteTarget = async (targetId) => {
  if (!confirm('Bạn có chắc chắn muốn xóa chỉ tiêu này?')) return;

  try {
    await dashboardService.deleteTarget(targetId);
    successMessage.value = 'Xóa chỉ tiêu thành công';
    await loadTargets();
  } catch (error) {
    console.error('Error deleting target:', error);
    errorMessage.value = 'Có lỗi xảy ra khi xóa chỉ tiêu';
  }
};

const closeModals = () => {
  showCreateModal.value = false;
  showEditModal.value = false;
  showBulkImportModal.value = false;
  editingTarget.value = null;

  // Reset form
  targetForm.value = {
    indicatorName: '',
    unitId: '',
    year: new Date().getFullYear(),
    periodType: '',
    period: '',
    targetValue: '',
    targetValueFormatted: '',
    unit: 'VND',
    isActive: true
  };
};

const exportData = () => {
  // TODO: Implement export functionality
  alert('Chức năng xuất Excel sẽ được phát triển trong phiên bản tiếp theo');
};

// Clear messages after 5 seconds
watch([errorMessage, successMessage], () => {
  setTimeout(() => {
    errorMessage.value = '';
    successMessage.value = '';
  }, 5000);
});

// Lifecycle
onMounted(async () => {
  if (!isAuthenticated()) {
    router.push('/login');
    return;
  }

  await loadTargets();
});
</script>

<style scoped>
.target-assignment {
  padding: 20px;
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  min-height: calc(100vh - 60px);
}

.page-header {
  background: linear-gradient(135deg, #8B1538 0%, #A6195C 50%, #B91D47 100%);
  color: white;
  padding: 30px;
  box-shadow: 0 4px 20px rgba(139, 21, 56, 0.3);
  position: relative;
  overflow: hidden;
}

.page-header::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23ffffff' fill-opacity='0.03'%3E%3Ccircle cx='30' cy='30' r='2'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E") repeat;
  z-index: 1;
}

.header-title {
  position: relative;
  z-index: 2;
  margin-bottom: 25px;
}

.page-header h2 {
  margin: 0;
  color: white;
  font-weight: 600;
  font-size: 28px;
  display: flex;
  align-items: center;
  gap: 15px;
  font-family: 'Segoe UI', 'Open Sans', sans-serif;
}

.page-header h2 i {
  font-size: 32px;
  opacity: 0.9;
}

.subtitle {
  margin: 8px 0 0 47px;
  font-size: 16px;
  opacity: 0.9;
  font-family: 'Segoe UI', 'Open Sans', sans-serif;
}

.header-controls {
  display: flex;
  gap: 15px;
  flex-wrap: wrap;
  align-items: center;
  position: relative;
  z-index: 2;
}

.targets-section {
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  margin-top: 20px;
}

.section-header {
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  padding: 20px 30px;
  border-bottom: 1px solid #dee2e6;
}

.section-header h3 {
  margin: 0;
  color: #8B1538;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 10px;
  font-family: 'Segoe UI', 'Open Sans', sans-serif;
}

.section-header h3 i {
  font-size: 24px;
}

.select-unit-message {
  text-align: center;
  padding: 60px 20px;
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  margin-top: 20px;
}

.message-icon {
  font-size: 48px;
  margin-bottom: 20px;
}

.select-unit-message h3 {
  color: #8B1538;
  margin: 0 0 10px 0;
  font-weight: 600;
}

.select-unit-message p {
  color: #6c757d;
  margin: 0;
  font-size: 16px;
}

.text-center {
  text-align: center;
}

.no-data {
  text-align: center;
  padding: 60px 20px;
  color: #8c8c8c;
}

.no-data-icon {
  font-size: 48px;
  margin-bottom: 20px;
  opacity: 0.6;
}

.no-data h4 {
  color: #6c757d;
  margin: 0 0 10px 0;
  font-weight: 600;
}

.no-data p {
  margin: 0 0 20px 0;
  font-size: 16px;
}

.number-input {
  text-align: right;
  font-family: 'Courier New', monospace;
  font-weight: 500;
}

.form-hint {
  display: block;
  margin-top: 4px;
  color: #6c757d;
  font-size: 12px;
  font-style: italic;
}

.targets-table-container {
  overflow-x: auto;
  padding: 20px 30px 30px 30px;
}

.targets-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.targets-table th,
.targets-table td {
  padding: 12px;
  text-align: left;
  border-bottom: 1px solid #eee;
}

.targets-table th {
  background: #f5f7fa;
  font-weight: 600;
  color: #303133;
}

.number-cell {
  text-align: right;
  font-family: monospace;
}

.status-badge {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.status-badge.active {
  background: #d4edda;
  color: #155724;
}

.status-badge.inactive {
  background: #f8d7da;
  color: #721c24;
}

.action-buttons {
  display: flex;
  gap: 8px;
}

.btn {
  padding: 8px 16px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.3s ease;
}

.btn-sm {
  padding: 4px 8px;
  font-size: 12px;
}

.btn-primary {
  background: #8B1538;
  color: white;
}

.btn-primary:hover {
  background: #40a9ff;
}

.btn-success {
  background: #52c41a;
  color: white;
}

.btn-success:hover {
  background: #73d13d;
}

.btn-warning {
  background: #faad14;
  color: white;
}

.btn-warning:hover {
  background: #ffc53d;
}

.btn-danger {
  background: #ff4d4f;
  color: white;
}

.btn-danger:hover {
  background: #ff7875;
}

.btn-secondary {
  background: #d9d9d9;
  color: #333;
}

.btn-secondary:hover {
  background: #f0f0f0;
}

.btn-info {
  background: #13c2c2;
  color: white;
}

.btn-info:hover {
  background: #36cfc9;
}

.form-select,
.form-input {
  padding: 8px 12px;
  border: 1px solid #d9d9d9;
  border-radius: 4px;
  font-size: 14px;
}

.form-select:focus,
.form-input:focus {
  outline: none;
  border-color: #8B1538;
  box-shadow: 0 0 0 2px rgba(24, 144, 255, 0.2);
}

.loading-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 60px 20px;
  text-align: center;
}

.loading-spinner {
  width: 40px;
  height: 40px;
  border: 4px solid #f3f3f3;
  border-top: 4px solid #8B1538;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-bottom: 16px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error-message {
  background: #fff2f0;
  border: 1px solid #ffccc7;
  color: #a8071a;
  padding: 12px 16px;
  border-radius: 4px;
  margin-bottom: 16px;
}

.success-message {
  background: #f6ffed;
  border: 1px solid #b7eb8f;
  color: #389e0d;
  padding: 12px 16px;
  border-radius: 4px;
  margin-bottom: 16px;
}

.no-data {
  text-align: center;
  padding: 40px;
  color: #8c8c8c;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 8px;
  width: 90%;
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  border-bottom: 1px solid #f0f0f0;
}

.modal-header h3 {
  margin: 0;
  color: #303133;
}

.close-btn {
  background: none;
  border: none;
  font-size: 16px;
  cursor: pointer;
}

.modal-body {
  padding: 20px;
}

.form-group {
  margin-bottom: 16px;
}

.form-group label {
  display: block;
  margin-bottom: 4px;
  color: #303133;
  font-weight: 500;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
}

.form-actions {
  display: flex;
  gap: 12px;
  justify-content: flex-end;
  margin-top: 24px;
}

.action-buttons {
  margin-top: 20px;
  text-align: right;
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

/* Responsive */
@media (max-width: 768px) {
  .header-controls {
    flex-direction: column;
    align-items: stretch;
  }

  .tab-navigation {
    flex-direction: column;
  }

  .targets-table {
    font-size: 12px;
  }

  .modal-content {
    width: 95%;
    margin: 10px;
  }
}
</style>
