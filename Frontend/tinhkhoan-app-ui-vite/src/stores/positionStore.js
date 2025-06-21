import { defineStore } from "pinia";
import apiClient from "@/services/api"; // Import instance Axios đã tạo

export const usePositionStore = defineStore("position", {
  // State: Nơi lưu trữ dữ liệu
  state: () => ({
    positions: [], // Mảng để lưu danh sách các chức vụ
    isLoading: false,
    error: null,
  }),

  // Getters: Cho phép lấy dữ liệu từ state
  getters: {
    allPositions: (state) => [...state.positions].sort((a, b) => a.id - b.id),
    positionCount: (state) => state.positions.length,
  },

  // Actions: Nơi định nghĩa các hàm để thay đổi state, thường dùng để gọi API
  actions: {
    async fetchPositions() {
      this.isLoading = true;
      this.error = null;
      try {
        const response = await apiClient.get("/Positions");
        let positionsData = [];
        if (response.data && Array.isArray(response.data.$values)) {
          positionsData = response.data.$values;
        } else if (Array.isArray(response.data)) {
          positionsData = response.data;
        } else if (response.data && typeof response.data === "object") {
          if (response.data.$id && response.data.id) {
            positionsData = [response.data];
          } else if (Object.keys(response.data).length > 0) {
            positionsData = [response.data];
          }
        }
        if (positionsData.length === 0) {
          console.error("Dữ liệu chức vụ không hợp lệ:", response.data);
          this.positions = [];
          this.error = "Dữ liệu chức vụ nhận được không đúng định dạng.";
          return;
        }
        this.positions = positionsData;
      } catch (err) {
        this.positions = [];
        this.error =
          "Không thể tải danh sách chức vụ. Lỗi: " +
          (err.response?.data?.message || err.message);
        console.error("Lỗi khi fetchPositions:", err);
      } finally {
        this.isLoading = false;
      }
    },

    async createPosition(positionData) {
      this.isLoading = true;
      this.error = null;
      try {
        const response = await apiClient.post("/Positions", positionData); // Gọi API POST /api/Positions

        if (!Array.isArray(this.positions)) {
          this.positions = [];
        }
        this.positions.push(response.data);
        return response.data;
      } catch (err) {
        this.error =
          "Không thể tạo chức vụ. Lỗi: " +
          (err.response?.data?.message ||
            err.response?.data?.title ||
            err.message);
        console.error(
          "Lỗi khi createPosition:",
          err.response?.data || err.message,
          err
        );
        throw err;
      } finally {
        this.isLoading = false;
      }
    },

    async updatePosition(positionData) {
      this.isLoading = true;
      this.error = null;
      try {
        await apiClient.put(`/Positions/${positionData.id}`, positionData); // Gọi API PUT /api/Positions/{id}

        const index = this.positions.findIndex((p) => p.id === positionData.id);
        if (index !== -1) {
          this.positions[index] = { ...this.positions[index], ...positionData };
        }
      } catch (err) {
        this.error =
          "Không thể cập nhật chức vụ. Lỗi: " +
          (err.response?.data?.message ||
            err.response?.data?.title ||
            (err.response?.data?.errors
              ? JSON.stringify(err.response.data.errors)
              : err.message));
        console.error(
          "Lỗi khi updatePosition:",
          err.response?.data || err.message,
          err
        );
        throw err;
      } finally {
        this.isLoading = false;
      }
    },

    async deletePosition(positionId) {
      this.isLoading = true;
      this.error = null;
      try {
        await apiClient.delete(`/Positions/${positionId}`); // Gọi API DELETE /api/Positions/{id}
        this.positions = this.positions.filter((p) => p.id !== positionId);
      } catch (err) {
        this.error =
          "Không thể xóa chức vụ. Lỗi: " +
          (err.response?.data?.message ||
            err.response?.data?.title ||
            err.message);
        console.error(
          "Lỗi khi deletePosition:",
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
