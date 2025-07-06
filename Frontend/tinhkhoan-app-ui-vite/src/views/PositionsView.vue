<template>
  <div class="positions-view">
    <h1>Quản lý Chức vụ</h1>
    <button
      @click="loadPositions"
      :disabled="positionStore.isLoading"
      class="action-button"
    >
      {{
        positionStore.isLoading ? "Đang tải..." : "Tải lại Danh sách Chức vụ"
      }}
    </button>

    <div v-if="formError || positionStore.error" class="error-message">
      <p>{{ formError || positionStore.error }}</p>
    </div>

    <!-- Debug removed -->

    <ul v-if="positionStore.positions.length > 0 && !positionStore.isLoading">
      <li
        v-for="position in positionStore.allPositions"
        :key="position.id"
        class="list-item"
      >
        <div class="item-info">
          <div class="position-header">
            <strong>{{ position.name }}</strong>
            <span class="position-id">ID: {{ position.id }}</span>
          </div>
          <span class="item-details" v-if="position.description"
            >(Mô tả: {{ position.description }})</span
          >
        </div>
        <div class="actions">
          <button @click="startEditPosition(position)" class="edit-btn">
            Sửa
          </button>
          <button
            @click="deletePosition(position.id)"
            class="delete-btn"
          >
            Xóa
          </button>
        </div>
      </li>
    </ul>
    <p
      v-else-if="!positionStore.isLoading && !positionStore.error && !formError"
    >
      Không có chức vụ nào để hiển thị.
    </p>
    <p v-if="positionStore.isLoading && positionStore.positions.length === 0">
      Đang tải danh sách chức vụ...
    </p>

    <hr class="separator" />

    <div class="form-container">
      <h2>{{ isEditing ? "Cập nhật Chức vụ" : "Thêm Chức vụ Mới" }}</h2>
      <form @submit.prevent="handleSubmitPosition">
        <div class="form-group">
          <label for="positionName">Tên Chức vụ:</label>
          <input
            type="text"
            id="positionName"
            :value="currentPosition.name"
            @input="currentPosition.name = $event.target.value"
            required
          />
        </div>
        <div class="form-group">
          <label for="positionDescription">Mô tả:</label>
          <input
            type="text"
            id="positionDescription"
            :value="currentPosition.description"
            @input="currentPosition.description = $event.target.value"
            placeholder="Mô tả chi tiết (nếu có)"
          />
        </div>
        <div class="form-actions">
          <button
            type="submit"
            :disabled="positionStore.isLoading"
            class="action-button"
          >
            {{
              positionStore.isLoading
                ? isEditing
                  ? "Đang cập nhật..."
                  : "Đang thêm..."
                : isEditing
                ? "Lưu Thay Đổi"
                : "Thêm Chức vụ"
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
import { usePositionStore } from "@/stores/positionStore";
import { onMounted, ref } from "vue";

const positionStore = usePositionStore();

const currentPosition = ref({
  id: null,
  name: "",
  description: "",
});

const isEditing = ref(false);
const formError = ref(null);

onMounted(() => {
  if (positionStore.positions.length === 0 && !positionStore.isLoading) {
    positionStore.fetchPositions();
  }
});

const loadPositions = () => {
  formError.value = null;
  positionStore.error = null;
  positionStore.fetchPositions();
};

const handleSubmitPosition = async () => {
  formError.value = null;
  positionStore.error = null;

  const nameFromInput =
    typeof currentPosition.value.name === "string"
      ? currentPosition.value.name.trim()
      : "";
  const descriptionFromInput =
    typeof currentPosition.value.description === "string"
      ? currentPosition.value.description.trim()
      : "";

  const positionDataToValidateAndSubmit = {
    ...currentPosition.value,
    name: nameFromInput,
    description: descriptionFromInput,
  };

  console.log("--- Bắt đầu handleSubmitPosition (Chức vụ) ---");
  console.log(
    "Giá trị currentPosition.value (gốc từ form):",
    JSON.parse(JSON.stringify(currentPosition.value))
  );
  console.log(
    "Dữ liệu sẽ được kiểm tra và gửi đi:",
    JSON.parse(JSON.stringify(positionDataToValidateAndSubmit))
  );

  if (!positionDataToValidateAndSubmit.name) {
    formError.value = "Tên chức vụ không được để trống!";
    console.log("VALIDATION FAIL (Client-side): Tên chức vụ trống.");
    return;
  }

  console.log("VALIDATION PASS (Client-side): Tên chức vụ hợp lệ.");

  if (isEditing.value && positionDataToValidateAndSubmit.id !== null) {
    try {
      await positionStore.updatePosition(positionDataToValidateAndSubmit);
      alert("Cập nhật chức vụ thành công!");
      cancelEdit();
    } catch (error) {
      console.error("Lỗi khi cập nhật chức vụ:", error);
    }
  } else {
    try {
      // eslint-disable-next-line no-unused-vars
      const { id, ...newPositionData } = positionDataToValidateAndSubmit;
      await positionStore.createPosition(newPositionData);
      alert("Thêm chức vụ thành công!");
      resetForm();
    } catch (error) {
      console.error("Lỗi khi thêm chức vụ:", error);
    }
  }
};

const startEditPosition = (position) => {
  formError.value = null;
  positionStore.error = null;
  isEditing.value = true;
  currentPosition.value = JSON.parse(JSON.stringify(position));
  console.log(
    "Dữ liệu nạp vào form sửa (startEditPosition):",
    JSON.parse(JSON.stringify(currentPosition.value))
  );
};

const cancelEdit = () => {
  isEditing.value = false;
  resetForm();
  formError.value = null;
  positionStore.error = null;
};

const resetForm = () => {
  currentPosition.value = {
    id: null,
    name: "",
    description: "",
  };
};

const deletePosition = async (positionId) => {
  formError.value = null;
  positionStore.error = null;
  try {
    await positionStore.deletePosition(positionId);
    // Xóa thành công - không hiển thị thông báo
  } catch (error) {
    console.error("Lỗi khi xóa chức vụ:", error);
    // Hiển thị lỗi từ server
    if (error.response?.data?.message) {
      formError.value = error.response.data.message;
    } else if (positionStore.error) {
      formError.value = positionStore.error;
    } else {
      formError.value = "Không thể xóa chức vụ. Vui lòng thử lại.";
    }
  }
};
</script>

<style scoped>
/* CSS tương tự UnitsView.vue, đổi .units-view thành .positions-view nếu cần */
.positions-view {
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
}
.position-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 4px;
}
.position-id {
  background-color: #8B1538;
  color: white;
  font-size: 0.75em;
  font-weight: 600;
  padding: 3px 8px;
  border-radius: 12px;
  white-space: nowrap;
}
.item-details {
  font-size: 0.85em;
  color: #7f8c8d;
  margin-left: 10px;
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
.form-group input[type="text"] {
  flex-grow: 1;
  min-width: 200px;
  padding: 10px 12px;
  border: 1px solid #ced4da;
  border-radius: 4px;
  box-sizing: border-box;
  transition: border-color 0.2s ease, box-shadow 0.2s ease;
}
.form-group input[type="text"]:focus {
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
</style>
