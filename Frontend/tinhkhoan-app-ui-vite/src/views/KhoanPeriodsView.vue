<template>
  <div class="khoan-periods-view">
    <h1>Quản lý Kỳ Khoán</h1>
    <button
      @click="loadKhoanPeriods"
      :disabled="khoanPeriodStore.isLoading"
      class="action-button"
    >
      {{
        khoanPeriodStore.isLoading
          ? "Đang tải..."
          : "Tải lại Danh sách Kỳ Khoán"
      }}
    </button>

    <div v-if="formError || khoanPeriodStore.error" class="error-message">
      <p>{{ formError || khoanPeriodStore.error }}</p>
    </div>

    <ul
      v-if="
        khoanPeriodStore.khoanPeriods.length > 0 && !khoanPeriodStore.isLoading
      "
    >
      <li
        v-for="period in khoanPeriodStore.sortedKhoanPeriods"
        :key="getId(period)"
        class="list-item"
      >
        <div class="item-info">
          <strong>{{ period.name }}</strong>
          <span class="item-details">
            <div class="period-type">📊 Loại: <strong>{{ period.typeDisplay || period.Type }}</strong></div>
            <div class="period-dates">📅 Thời gian: {{ formatDate(period.StartDate || period.startDate) }} → {{ formatDate(period.EndDate || period.endDate) }}</div>
            <div class="period-status">🏷️ Trạng thái: <span class="status-badge" :class="getStatusClass(period.Status || period.status)">{{ period.statusDisplay || period.Status }}</span></div>
          </span>
        </div>
        <div class="actions">
          <button @click="startEditKhoanPeriod(period)" class="edit-btn">
            Sửa
          </button>
          <button
            @click="confirmDeleteKhoanPeriod(period.Id)"
            class="delete-btn"
          >
            Xóa
          </button>
        </div>
      </li>
    </ul>
    <p v-else-if="!khoanPeriodStore.isLoading && !displayError">
      Không có Kỳ Khoán nào để hiển thị.
    </p>
    <p
      v-if="
        khoanPeriodStore.isLoading && khoanPeriodStore.khoanPeriods.length === 0
      "
    >
      Đang tải danh sách Kỳ Khoán...
    </p>

    <hr class="separator" />

    <div class="form-container">
      <h2>{{ isEditing ? "Cập nhật Kỳ Khoán" : "Thêm Kỳ Khoán Mới" }}</h2>
      <form @submit.prevent="handleSubmitKhoanPeriod">
        <div class="form-group">
          <label for="periodName">Tên Kỳ Khoán:</label>
          <input
            type="text"
            id="periodName"
            v-model.trim="currentKhoanPeriod.name"
            required
          />
        </div>
        <div class="form-group">
          <label for="periodType">Loại Kỳ:</label>
          <select id="periodType" v-model="currentKhoanPeriod.type" required>
            <option :value="null" disabled>-- Chọn Loại Kỳ --</option>
            <option value="MONTHLY">Tháng</option>
            <option value="QUARTERLY">Quý</option>
            <option value="ANNUAL">Năm</option>
          </select>
        </div>
        <div class="form-group">
          <label for="startDate">Ngày Bắt đầu:</label>
          <input
            type="date"
            id="startDate"
            v-model="currentKhoanPeriod.startDate"
            required
          />
        </div>
        <div class="form-group">
          <label for="endDate">Ngày Kết thúc:</label>
          <input
            type="date"
            id="endDate"
            v-model="currentKhoanPeriod.endDate"
            required
          />
        </div>
        <div class="form-group">
          <label for="periodStatus">Trạng thái:</label>
          <select
            id="periodStatus"
            v-model="currentKhoanPeriod.status"
            required
          >
            <option :value="null" disabled>-- Chọn Trạng thái --</option>
            <option value="DRAFT">Nháp (Draft)</option>
            <option value="OPEN">Mở (Open)</option>
            <option value="PROCESSING">Đang xử lý (Processing)</option>
            <option value="PENDINGAPPROVAL">
              Chờ duyệt (Pending Approval)
            </option>
            <option value="CLOSED">Đã đóng (Closed)</option>
            <option value="ARCHIVED">Lưu trữ (Archived)</option>
          </select>
        </div>
        <div class="form-actions">
          <button
            type="submit"
            :disabled="khoanPeriodStore.isLoading"
            class="action-button"
          >
            {{
              khoanPeriodStore.isLoading
                ? isEditing
                  ? "Đang cập nhật..."
                  : "Đang thêm..."
                : isEditing
                ? "Lưu Thay Đổi"
                : "Thêm Kỳ Khoán"
            }}
          </button>
          <button
            type="button"
            @click="cancelEdit"
            v-if="isEditing"
            class="cancel-btn action-button"
          >
            Hủy
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from "vue";
import { useKhoanPeriodStore } from "../stores/khoanPeriodStore.js";
import { ensurePascalCase, getId, getName, safeGet } from "../utils/casingSafeAccess.js";

const khoanPeriodStore = useKhoanPeriodStore();

const initialKhoanPeriodData = () => ({
  id: null,
  name: "",
  type: null,
  startDate: "",
  endDate: "",
  status: "DRAFT",
});

const currentKhoanPeriod = ref(initialKhoanPeriodData());
const isEditing = ref(false);
const formError = ref(null);

const displayError = computed(() => {
  return formError.value || khoanPeriodStore.error;
});

const loadKhoanPeriods = () => {
  formError.value = null;
  khoanPeriodStore.error = null;
  khoanPeriodStore.fetchKhoanPeriods();
};

onMounted(() => {
  if (
    khoanPeriodStore.khoanPeriods.length === 0 &&
    !khoanPeriodStore.isLoading
  ) {
    loadKhoanPeriods();
  }
});

// Hàm định dạng ngày tháng YYYY-MM-DD cho input type="date"
const toDateInputValue = (dateString) => {
  if (!dateString) return "";
  const date = new Date(dateString);
  const year = date.getFullYear();
  const month = ("0" + (date.getMonth() + 1)).slice(-2);
  const day = ("0" + date.getDate()).slice(-2);
  return `${year}-${month}-${day}`;
};

// Hàm định dạng ngày cho hiển thị
const formatDate = (dateString) => {
  if (!dateString) return "";
  const date = new Date(dateString);
  return date.toLocaleDateString('vi-VN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit'
  });
};

// Hàm lấy class CSS cho status badge
const getStatusClass = (status) => {
  const statusMap = {
    'DRAFT': 'status-draft',
    'OPEN': 'status-open',
    'PROCESSING': 'status-processing',
    'PENDINGAPPROVAL': 'status-pending',
    'CLOSED': 'status-closed',
    'ARCHIVED': 'status-archived',
    'Nháp': 'status-draft',
    'Mở': 'status-open',
    'Tạm dừng': 'status-processing',
    'Chờ duyệt': 'status-pending',
    'Đóng': 'status-closed',
    'Lưu trữ': 'status-archived'
  };
  return statusMap[status] || 'status-default';
};

const handleSubmitKhoanPeriod = async () => {
  formError.value = null;
  khoanPeriodStore.error = null;

  let dataToSubmit = { ...currentKhoanPeriod.value };

  // Client-side Validation
  if (!dataToSubmit.name?.trim()) {
    formError.value = "Tên Kỳ Khoán không được để trống!";
    return;
  }
  if (!dataToSubmit.type) {
    formError.value = "Vui lòng chọn Loại Kỳ Khoán.";
    return;
  }
  if (!dataToSubmit.startDate) {
    formError.value = "Vui lòng chọn Ngày Bắt đầu.";
    return;
  }
  if (!dataToSubmit.endDate) {
    formError.value = "Vui lòng chọn Ngày Kết thúc.";
    return;
  }
  if (new Date(dataToSubmit.startDate) >= new Date(dataToSubmit.endDate)) {
    formError.value = "Ngày Kết thúc phải sau Ngày Bắt đầu.";
    return;
  }
  if (!dataToSubmit.status) {
    formError.value = "Vui lòng chọn Trạng thái.";
    return;
  }

  // Đảm bảo startDate và endDate đúng định dạng
  dataToSubmit.startDate = toDateInputValue(dataToSubmit.startDate);
  dataToSubmit.endDate = toDateInputValue(dataToSubmit.endDate);

  console.log('🔍 handleSubmitKhoanPeriod - dataToSubmit:', dataToSubmit);
  console.log('🔍 handleSubmitKhoanPeriod - isEditing:', isEditing.value);

  if (isEditing.value && (getId(dataToSubmit) !== null && getId(dataToSubmit) !== undefined)) {
    try {
      console.log('🔄 Calling updateKhoanPeriod with:', dataToSubmit);
      await khoanPeriodStore.updateKhoanPeriod(dataToSubmit);
      alert("Cập nhật Kỳ Khoán thành công!");
      cancelEdit();
    } catch (error) {
      console.error("Lỗi khi cập nhật Kỳ Khoán:", error);
    }
  } else {
    try {
      //const { id, ...newPeriodData } = dataToSubmit;
      //await khoanPeriodStore.createKhoanPeriod(newPeriodData);
      const newPeriodData = { ...dataToSubmit };
      // Remove both camelCase and PascalCase ID fields to let backend generate new ID
      delete newPeriodData.id;
      delete newPeriodData.Id;
      await khoanPeriodStore.createKhoanPeriod(newPeriodData);
      alert("Thêm Kỳ Khoán thành công!");
      resetForm();
    } catch (error) {
      console.error("Lỗi khi thêm Kỳ Khoán:", error);
    }
  }
};

const startEditKhoanPeriod = (period) => {
  formError.value = null;
  khoanPeriodStore.error = null;
  isEditing.value = true;

  // Backend returns string enums, map to frontend form values
  // Type mapping: backend string → frontend form value
  const typeMapping = {
    'MONTHLY': 'MONTHLY',
    'QUARTERLY': 'QUARTERLY',
    'ANNUAL': 'ANNUAL',
    // Handle numeric if needed
    0: 'MONTHLY',
    1: 'QUARTERLY',
    2: 'ANNUAL'
  };

  // Status mapping: backend string → frontend form value
  const statusMapping = {
    'DRAFT': 'DRAFT',
    'OPEN': 'OPEN',
    'PROCESSING': 'PROCESSING',
    'PENDINGAPPROVAL': 'PENDINGAPPROVAL',
    'CLOSED': 'CLOSED',
    'ARCHIVED': 'ARCHIVED',
    // Handle numeric if needed
    0: 'DRAFT',
    1: 'OPEN',
    2: 'PROCESSING',
    3: 'PENDINGAPPROVAL',
    4: 'CLOSED',
    5: 'ARCHIVED'
  };

  currentKhoanPeriod.value = {
    ...ensurePascalCase(period),
    id: getId(period), // Use helper to get ID safely
    name: getName(period),
    type: typeMapping[safeGet(period, 'Type')] || safeGet(period, 'Type'),
    status: statusMapping[safeGet(period, 'Status')] || safeGet(period, 'Status'),
    startDate: toDateInputValue(safeGet(period, 'StartDate')),
    endDate: toDateInputValue(safeGet(period, 'EndDate')),
  };
  console.log('🔍 startEditKhoanPeriod - original period:', period);
  console.log('🔍 startEditKhoanPeriod - currentKhoanPeriod:', currentKhoanPeriod.value);
};

const cancelEdit = () => {
  isEditing.value = false;
  resetForm();
  formError.value = null;
  khoanPeriodStore.error = null;
};

const resetForm = () => {
  currentKhoanPeriod.value = initialKhoanPeriodData();
};

const confirmDeleteKhoanPeriod = async (periodId) => {
  formError.value = null;
  khoanPeriodStore.error = null;
  if (confirm(`Bạn có chắc chắn muốn xóa Kỳ Khoán có ID: ${periodId} không?`)) {
    try {
      await khoanPeriodStore.deleteKhoanPeriod(periodId);
      alert("Xóa Kỳ Khoán thành công!");
    } catch (error) {
      console.error("Lỗi khi xóa Kỳ Khoán:", error);
    }
  }
};
</script>

<style scoped>
.khoan-periods-view {
  max-width: 900px;
  margin: 20px auto;
  padding: 20px;
  /* 🇻🇳 Sử dụng font tiếng Việt tối ưu */
  font-family: var(--font-primary, 'Roboto', 'Segoe UI', 'Arial', sans-serif);
  color: #2c3e50;
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

h1,
h2 {
  color: #34495e;
}

.error-message {
  color: white;
  background-color: #e74c3c;
  border: 1px solid #c0392b;
  padding: 12px 18px;
  margin-top: 15px;
  margin-bottom: 20px;
  border-radius: 5px;
  text-align: left;
}
.error-message p {
  margin: 0;
}

ul {
  list-style-type: none;
  padding: 0;
  margin-top: 20px;
}

.list-item {
  background-color: #ecf0f1;
  border: 1px solid #bdc3c7;
  padding: 12px 18px;
  margin-bottom: 12px;
  border-radius: 5px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  transition: background-color 0.2s ease-in-out;
}
.list-item:hover {
  background-color: #e0e6e8;
}

.item-info {
  flex-grow: 1;
  text-align: left;
}

.item-info strong {
  color: #2c3e50;
  display: block;
  margin-bottom: 4px;
}

.item-details {
  display: flex;
  flex-direction: column;
  gap: 5px;
  margin-top: 8px;
}

.period-type, .period-dates, .period-status {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 0.9em;
}

.period-type strong {
  color: #2c3e50;
  font-weight: 600;
}

.period-dates {
  color: #34495e;
  font-weight: 500;
}

.actions {
  display: flex;
  gap: 10px;
  flex-shrink: 0;
}

.action-button,
.edit-btn,
.delete-btn,
.cancel-btn {
  padding: 8px 15px;
  font-size: 0.9em;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  color: white;
  font-weight: 500;
  transition: background-color 0.2s ease;
}

.action-button:disabled,
.edit-btn:disabled,
.delete-btn:disabled,
.cancel-btn:disabled {
  background-color: #bdc3c7;
  cursor: not-allowed;
}

.edit-btn {
  background-color: #f39c12;
}
.edit-btn:hover:not(:disabled) {
  background-color: #e67e22;
}

.delete-btn {
  background-color: #e74c3c;
}
.delete-btn:hover:not(:disabled) {
  background-color: #c82333;
}

.cancel-btn {
  background-color: #95a5a6;
}
.cancel-btn:hover:not(:disabled) {
  background-color: #7f8c8d;
}

.separator {
  margin: 35px 0;
  border: 0;
  border-top: 1px solid #bdc3c7;
}

.form-container {
  background-color: #ffffff;
  padding: 25px;
  border-radius: 6px;
  border: 1px solid #dde0e3;
  box-shadow: 0 1px 5px rgba(0, 0, 0, 0.05);
}

.form-container h2 {
  margin-top: 0;
  margin-bottom: 25px;
  text-align: center;
  color: #34495e;
}

.form-group {
  margin-bottom: 18px;
  display: flex;
  flex-wrap: wrap;
  align-items: center;
}

.form-group label {
  flex-basis: 160px;
  margin-right: 15px;
  text-align: right;
  font-weight: bold;
  color: #34495e;
}

.form-group input[type="text"],
.form-group input[type="date"],
.form-group select {
  flex-grow: 1;
  min-width: 200px;
  padding: 10px 12px;
  border: 1px solid #ced4da;
  border-radius: 4px;
  box-sizing: border-box;
  transition: border-color 0.2s ease, box-shadow 0.2s ease;
}
.form-group input[type="text"]:focus,
.form-group input[type="date"]:focus,
.form-group select:focus {
  border-color: #80bdff;
  outline: 0;
  box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
}

.form-actions {
  text-align: center;
  margin-top: 25px;
}

.form-actions .action-button {
  margin-right: 12px;
}
.form-actions .action-button:last-child {
  margin-right: 0;
}

button.action-button {
  background-color: #007bff;
}
button.action-button:hover:not(:disabled) {
  background-color: #0056b3;
}

/* Status badge styles */
.status-draft {
  background-color: #3498db;
  color: white;
}
.status-open {
  background-color: #2ecc71;
  color: white;
}
.status-processing {
  background-color: #f39c12;
  color: white;
}
.status-pending {
  background-color: #e67e22;
  color: white;
}
.status-closed {
  background-color: #e74c3c;
  color: white;
}
.status-archived {
  background-color: #95a5a6;
  color: white;
}
.status-default {
  background-color: #bdc3c7;
  color: white;
}
</style>
