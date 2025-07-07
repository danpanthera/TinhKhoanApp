import { defineStore } from "pinia";
import apiClient from "../services/api.js";

export const useKhoanPeriodStore = defineStore("khoanPeriod", {
  // State
  state: () => ({
    khoanPeriods: [], // Danh sách các kỳ khoán
    isLoading: false,
    error: null,
  }),

  // Getters
  getters: {
    allKhoanPeriods: (state) => state.khoanPeriods,
    // Sắp xếp theo ngày bắt đầu giảm dần để kỳ mới nhất lên đầu
    sortedKhoanPeriods: (state) => {
      return [...state.khoanPeriods].sort(
        (a, b) => new Date(b.startDate) - new Date(a.startDate)
      );
    },
    khoanPeriodCount: (state) => state.khoanPeriods.length,
  },

  // Actions
  actions: {
    async fetchKhoanPeriods() {
      this.isLoading = true;
      this.error = null;
      try {
        const response = await apiClient.get("/KhoanPeriods");
        if (response.data && Array.isArray(response.data.$values)) {
          this.khoanPeriods = response.data.$values;
        } else if (Array.isArray(response.data)) {
          this.khoanPeriods = response.data;
        } else {
          this.khoanPeriods = [];
          this.error = "Dữ liệu Kỳ Khoán nhận được không đúng định dạng.";
        }
      } catch (err) {
        this.khoanPeriods = [];
        this.error =
          "Không thể tải danh sách Kỳ Khoán. Lỗi: " +
          (err.response?.data?.message || err.message);
      } finally {
        this.isLoading = false;
      }
    },

    async createKhoanPeriod(periodData) {
      this.isLoading = true;
      this.error = null;
      try {
        const response = await apiClient.post("/KhoanPeriods", periodData);
        // Thay vì push, mình fetch lại để đảm bảo thứ tự và dữ liệu đầy đủ từ server
        await this.fetchKhoanPeriods();
        return response.data;
      } catch (err) {
        this.error =
          "Không thể tạo Kỳ Khoán. Lỗi: " +
          (err.response?.data?.message ||
            err.response?.data?.title ||
            (err.response?.data?.errors
              ? JSON.stringify(err.response.data.errors)
              : err.message));
        throw err;
      } finally {
        this.isLoading = false;
      }
    },

    async updateKhoanPeriod(periodData) {
      this.isLoading = true;
      this.error = null;
      try {
        await apiClient.put(`/KhoanPeriods/${periodData.id}`, periodData);
        await this.fetchKhoanPeriods(); // Fetch lại để cập nhật
      } catch (err) {
        this.error =
          "Không thể cập nhật Kỳ Khoán. Lỗi: " +
          (err.response?.data?.message ||
            err.response?.data?.title ||
            (err.response?.data?.errors
              ? JSON.stringify(err.response.data.errors)
              : err.message));
        throw err;
      } finally {
        this.isLoading = false;
      }
    },

    async deleteKhoanPeriod(periodId) {
      this.isLoading = true;
      this.error = null;
      try {
        await apiClient.delete(`/KhoanPeriods/${periodId}`);
        // Xóa khỏi state hoặc fetch lại
        this.khoanPeriods = this.khoanPeriods.filter((p) => p.id !== periodId);
        // Hoặc await this.fetchKhoanPeriods();
      } catch (err) {
        this.error =
          "Không thể xóa Kỳ Khoán. Lỗi: " +
          (err.response?.data?.message ||
            err.response?.data?.title ||
            err.message);
        throw err;
      } finally {
        this.isLoading = false;
      }
    },
  },
});
