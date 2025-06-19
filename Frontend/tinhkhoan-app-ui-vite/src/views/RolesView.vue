<template>
  <div class="roles-view">
    <h1>Quản lý Vai trò</h1>
    <div class="header-controls">
      <button
        @click="loadRoles"
        :disabled="roleStore.isLoading"
        class="action-button"
      >
        {{ roleStore.isLoading ? "Đang tải..." : "Tải lại Danh sách Vai trò" }}
      </button>
      <span v-if="roleStore.roles.length > 0" class="roles-count">
        (Tổng số: {{ roleStore.roles.length }} vai trò)
      </span>
    </div>

    <div v-if="formError || roleStore.error" class="error-message">
      <p>{{ formError || roleStore.error }}</p>
    </div>

    <!-- Debug removed -->

    <ul v-if="roleStore.roles.length > 0 && !roleStore.isLoading">
      <li v-for="role in roleStore.allRoles" :key="role.id" class="list-item">
        <div class="item-info">
          <strong>{{ role.name }}</strong>
          <span class="item-details" v-if="role.description"
            >(Mô tả: {{ role.description }})</span
          >
        </div>
        <div class="actions">
          <button @click="startEditRole(role)" class="edit-btn">Sửa</button>
          <button @click="confirmDeleteRole(role.id)" class="delete-btn">
            Xóa
          </button>
        </div>
      </li>
    </ul>
    <p v-else-if="!roleStore.isLoading && !roleStore.error && !formError">
      Không có vai trò nào để hiển thị.
    </p>
    <p v-if="roleStore.isLoading && roleStore.roles.length === 0">
      Đang tải danh sách vai trò...
    </p>

    <hr class="separator" />

    <div class="form-container">
      <h2>{{ isEditing ? "Cập nhật Vai trò" : "Thêm Vai trò Mới" }}</h2>
      <form @submit.prevent="handleSubmitRole">
        <div class="form-group">
          <label for="roleName">Tên Vai trò:</label>
          <input
            type="text"
            id="roleName"
            :value="currentRole.name"
            @input="currentRole.name = $event.target.value"
            required
          />
        </div>
        <div class="form-group">
          <label for="roleDescription">Mô tả:</label>
          <input
            type="text"
            id="roleDescription"
            :value="currentRole.description"
            @input="currentRole.description = $event.target.value"
            placeholder="Mô tả chi tiết (nếu có)"
          />
        </div>
        <div class="form-actions">
          <button
            type="submit"
            :disabled="roleStore.isLoading"
            class="action-button"
          >
            {{
              roleStore.isLoading
                ? isEditing
                  ? "Đang cập nhật..."
                  : "Đang thêm..."
                : isEditing
                ? "Lưu Thay Đổi"
                : "Thêm Vai trò"
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
import { ref, onMounted } from "vue";
import { useRoleStore } from "@/stores/roleStore";

const roleStore = useRoleStore();

const currentRole = ref({
  id: null,
  name: "",
  description: "",
});

const isEditing = ref(false);
const formError = ref(null);

onMounted(() => {
  if (roleStore.roles.length === 0 && !roleStore.isLoading) {
    roleStore.fetchRoles();
  }
});

const loadRoles = () => {
  formError.value = null;
  roleStore.error = null;
  roleStore.fetchRoles();
};

const handleSubmitRole = async () => {
  formError.value = null;
  roleStore.error = null;

  const nameFromInput =
    typeof currentRole.value.name === "string"
      ? currentRole.value.name.trim()
      : "";
  const descriptionFromInput =
    typeof currentRole.value.description === "string"
      ? currentRole.value.description.trim()
      : "";

  const roleDataToValidateAndSubmit = {
    ...currentRole.value,
    name: nameFromInput,
    description: descriptionFromInput,
  };

  console.log("--- Bắt đầu handleSubmitRole (Vai trò) ---");
  console.log(
    "Dữ liệu gửi đi:",
    JSON.parse(JSON.stringify(roleDataToValidateAndSubmit))
  );

  if (!roleDataToValidateAndSubmit.name) {
    formError.value = "Tên vai trò không được để trống!";
    console.log("VALIDATION FAIL (Client-side): Tên vai trò trống.");
    return;
  }
  console.log("VALIDATION PASS (Client-side): Tên vai trò hợp lệ.");

  if (isEditing.value && roleDataToValidateAndSubmit.id !== null) {
    try {
      await roleStore.updateRole(roleDataToValidateAndSubmit);
      alert("Cập nhật vai trò thành công!");
      cancelEdit();
    } catch (error) {
      console.error("Lỗi khi cập nhật vai trò:", error);
    }
  } else {
    try {
      // eslint-disable-next-line no-unused-vars
      const { id, ...newRoleData } = roleDataToValidateAndSubmit;
      await roleStore.createRole(newRoleData);
      alert("Thêm vai trò thành công!");
      resetForm();
    } catch (error) {
      console.error("Lỗi khi thêm vai trò:", error);
    }
  }
};

const startEditRole = (role) => {
  formError.value = null;
  roleStore.error = null;
  isEditing.value = true;
  currentRole.value = JSON.parse(JSON.stringify(role));
  console.log(
    "Dữ liệu nạp vào form sửa (startEditRole):",
    JSON.parse(JSON.stringify(currentRole.value))
  );
};

const cancelEdit = () => {
  isEditing.value = false;
  resetForm();
  formError.value = null;
  roleStore.error = null;
};

const resetForm = () => {
  currentRole.value = {
    id: null,
    name: "",
    description: "",
  };
};

const confirmDeleteRole = async (roleId) => {
  formError.value = null;
  roleStore.error = null;
  if (confirm(`Bạn có chắc chắn muốn xóa vai trò có ID: ${roleId} không?`)) {
    try {
      await roleStore.deleteRole(roleId);
      alert("Xóa vai trò thành công!");
    } catch (error) {
      console.error("Lỗi khi xóa vai trò:", error);
    }
  }
};
</script>

<style scoped>
/* CSS tương tự, đổi .roles-view nếu cần */
.roles-view {
  max-width: 900px;
  margin: 20px auto;
  padding: 20px;
  font-family: Avenir, Helvetica, Arial, sans-serif;
  color: #2c3e50;
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.header-controls {
  display: flex;
  align-items: center;
  gap: 15px;
  margin-bottom: 20px;
}

.roles-count {
  color: #6c757d;
  font-weight: 500;
  font-size: 14px;
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
  background-color: #c0392b;
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
