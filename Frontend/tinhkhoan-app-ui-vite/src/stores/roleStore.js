import apiClient from "@/services/api"; // Import instance Axios đã tạo
import { defineStore } from "pinia";

export const useRoleStore = defineStore("role", {
  // State: Nơi lưu trữ dữ liệu
  state: () => ({
    roles: [], // Mảng để lưu danh sách các vai trò
    isLoading: false,
    error: null,
  }),

  // Getters: Cho phép lấy dữ liệu từ state
  getters: {
    allRoles: (state) => state.roles,
    roleCount: (state) => state.roles.length,
  },

  // Actions: Nơi định nghĩa các hàm để thay đổi state, thường dùng để gọi API
  actions: {
    async fetchRoles() {
      console.log('🚀 fetchRoles() called - Starting API request...');
      this.isLoading = true;
      this.error = null;
      try {
        console.log('📞 Making API call to:', '/Roles');
        const response = await apiClient.get("/Roles"); // Gọi API GET /api/Roles
        console.log('📦 API Response received:', {
          status: response.status,
          dataType: typeof response.data,
          isArray: Array.isArray(response.data),
          dataLength: response.data?.length,
          hasValues: !!response.data?.$values
        });

        if (response.data && Array.isArray(response.data.$values)) {
          console.log('✅ Using $values format, length:', response.data.$values.length);
          this.roles = response.data.$values;
        } else if (Array.isArray(response.data)) {
          console.log('✅ Using direct array format, length:', response.data.length);
          this.roles = response.data;
        } else {
          console.error(
            "❌ Dữ liệu trả về từ API fetchRoles không phải là mảng hoặc không có trường $values hợp lệ:",
            response.data
          );
          this.roles = [];
          this.error = "Dữ liệu vai trò nhận được không đúng định dạng.";
        }

        console.log('🎯 Final roles state:', {
          rolesCount: this.roles.length,
          firstRole: this.roles[0]
        });
      } catch (err) {
        console.error('❌ fetchRoles error:', err);
        this.roles = [];
        this.error =
          "Không thể tải danh sách vai trò. Lỗi: " +
          (err.response?.data?.message || err.message);
        console.error("Lỗi khi fetchRoles:", err);
      } finally {
        this.isLoading = false;
        console.log('🏁 fetchRoles() completed, isLoading:', this.isLoading);
      }
    },

    async createRole(roleData) {
      this.isLoading = true;
      this.error = null;
      try {
        const response = await apiClient.post("/Roles", roleData); // Gọi API POST /api/Roles

        if (!Array.isArray(this.roles)) {
          this.roles = [];
        }
        this.roles.push(response.data);
        return response.data;
      } catch (err) {
        this.error =
          "Không thể tạo vai trò. Lỗi: " +
          (err.response?.data?.message ||
            err.response?.data?.title ||
            err.message);
        console.error(
          "Lỗi khi createRole:",
          err.response?.data || err.message,
          err
        );
        throw err;
      } finally {
        this.isLoading = false;
      }
    },

    async updateRole(roleData) {
      this.isLoading = true;
      this.error = null;
      try {
        await apiClient.put(`/Roles/${roleData.id}`, roleData); // Gọi API PUT /api/Roles/{id}

        const index = this.roles.findIndex((r) => r.id === roleData.id);
        if (index !== -1) {
          this.roles[index] = { ...this.roles[index], ...roleData };
        }
      } catch (err) {
        this.error =
          "Không thể cập nhật vai trò. Lỗi: " +
          (err.response?.data?.message ||
            err.response?.data?.title ||
            (err.response?.data?.errors
              ? JSON.stringify(err.response.data.errors)
              : err.message));
        console.error(
          "Lỗi khi updateRole:",
          err.response?.data || err.message,
          err
        );
        throw err;
      } finally {
        this.isLoading = false;
      }
    },

    async deleteRole(roleId) {
      this.isLoading = true;
      this.error = null;
      try {
        await apiClient.delete(`/Roles/${roleId}`); // Gọi API DELETE /api/Roles/{id}
        this.roles = this.roles.filter((r) => r.id !== roleId);
      } catch (err) {
        this.error =
          "Không thể xóa vai trò. Lỗi: " +
          (err.response?.data?.message ||
            err.response?.data?.title ||
            err.message);
        console.error(
          "Lỗi khi deleteRole:",
          err.response?.data || err.message,
          err
        );
        throw err;
      } finally {
        this.isLoading = false;
      }
    },
  },
});
