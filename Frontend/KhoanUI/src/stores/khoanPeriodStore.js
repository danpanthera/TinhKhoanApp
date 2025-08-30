import { defineStore } from 'pinia'
import apiClient from '../services/api.js'
import { getId, getName, getStatus, getType, normalizeArray } from '../utils/casingSafeAccess.js'

export const useKhoanPeriodStore = defineStore('khoanPeriod', {
  // State
  state: () => ({
    khoanPeriods: [], // Danh sách các kỳ khoán
    isLoading: false,
    error: null,
  }),

  // Getters
  getters: {
    allKhoanPeriods: state => state.khoanPeriods,
    // Sắp xếp theo ngày bắt đầu giảm dần để kỳ mới nhất lên đầu
    sortedKhoanPeriods: state => {
      return [...state.khoanPeriods].sort(
        (a, b) => new Date(b.StartDate || b.startDate) - new Date(a.StartDate || a.startDate),
      )
    },
    khoanPeriodCount: state => state.khoanPeriods.length,
  },

  // Actions
  actions: {
    // Helper methods for mapping between frontend and backend formats
    mapTypeToEnum(type) {
      const typeMap = {
        Tháng: 0,
        Quý: 1,
        Năm: 2,
        MONTHLY: 0,
        QUARTERLY: 1,
        ANNUAL: 2,
        // Handle numeric values already integers
        0: 0,
        1: 1,
        2: 2,
      }
      return typeMap[type] ?? 0 // Default to MONTHLY (0)
    },

    mapStatusToEnum(status) {
      const statusMap = {
        Nháp: 0,
        Mở: 1,
        'Tạm dừng': 2,
        'Chờ duyệt': 3,
        Đóng: 4,
        'Lưu trữ': 5,
        DRAFT: 0,
        OPEN: 1,
        PROCESSING: 2,
        PENDINGAPPROVAL: 3,
        CLOSED: 4,
        ARCHIVED: 5,
        // Handle numeric values already integers
        0: 0,
        1: 1,
        2: 2,
        3: 3,
        4: 4,
        5: 5,
      }
      return statusMap[status] ?? 0 // Default to DRAFT (0)
    },

    // Helper methods for mapping from backend to frontend display
    mapTypeFromEnum(type) {
      const typeMap = {
        // Backend returns string enum values
        MONTHLY: 'Tháng',
        QUARTERLY: 'Quý',
        ANNUAL: 'Năm',
        // Handle numeric values if needed
        0: 'Tháng', // MONTHLY
        1: 'Quý', // QUARTERLY
        2: 'Năm', // ANNUAL
      }
      return typeMap[type] || type
    },

    mapStatusFromEnum(status) {
      const statusMap = {
        // Backend returns string enum values
        DRAFT: 'Nháp',
        OPEN: 'Mở',
        PROCESSING: 'Tạm dừng',
        PENDINGAPPROVAL: 'Chờ duyệt',
        CLOSED: 'Đóng',
        ARCHIVED: 'Lưu trữ',
        // Handle numeric values if needed
        0: 'Nháp', // DRAFT
        1: 'Mở', // OPEN
        2: 'Tạm dừng', // PROCESSING
        3: 'Chờ duyệt', // PENDINGAPPROVAL
        4: 'Đóng', // CLOSED
        5: 'Lưu trữ', // ARCHIVED
      }
      return statusMap[status] || status
    },

    async fetchKhoanPeriods() {
      this.isLoading = true
      this.error = null
      try {
        const response = await apiClient.get('/KhoanPeriods')
        let rawData = []

        // Check for the new API response format: { Success: true, Data: [...] }
        if (response.data && response.data.Success && Array.isArray(response.data.Data)) {
          rawData = response.data.Data
        } else if (response.data && Array.isArray(response.data.$values)) {
          rawData = response.data.$values
        } else if (Array.isArray(response.data)) {
          rawData = response.data
        } else {
          console.error('🚨 Unexpected API response format:', response.data)
          this.khoanPeriods = []
          this.error = 'Dữ liệu Kỳ Khoán nhận được không đúng định dạng.'
          return
        }

        // Map backend enum data to frontend display format and normalize casing
        this.khoanPeriods = normalizeArray(rawData).map(period => ({
          ...period,
          typeDisplay: this.mapTypeFromEnum(getType(period)),
          statusDisplay: this.mapStatusFromEnum(getStatus(period)),
        }))

        console.log('🔄 fetchKhoanPeriods - raw data:', rawData)
        console.log('🔄 fetchKhoanPeriods - mapped data:', this.khoanPeriods)
      } catch (err) {
        this.khoanPeriods = []
        this.error = 'Không thể tải danh sách Kỳ Khoán. Lỗi: ' + (err.response?.data?.message || err.message)
      } finally {
        this.isLoading = false
      }
    },

    async createKhoanPeriod(periodData) {
      this.isLoading = true
      this.error = null
      try {
        console.log('🚀 Store - Creating KhoanPeriod with:', periodData)
        const response = await apiClient.post('/KhoanPeriods', periodData)
        console.log('🚀 Store - Backend response:', response)

        // Check for new API response format: { Success: true, Data: {...} }
        if (response.data && response.data.Success) {
          // Thay vì push, mình fetch lại để đảm bảo thứ tự và dữ liệu đầy đủ từ server
          await this.fetchKhoanPeriods()
          return {
            success: true,
            data: response.data.Data,
            message: response.data.Message || 'Tạo kỳ khoán thành công',
          }
        } else if (response.data && (response.data.success !== false)) {
          // Legacy format - keep for backward compatibility
          await this.fetchKhoanPeriods()
          return { success: true, data: response.data, message: 'Tạo kỳ khoán thành công' }
        } else {
          // Backend returned data but with error flag
          return {
            success: false,
            message: response.data?.Message || response.data?.message || response.data?.errors || 'Backend trả về lỗi không xác định',
            errors: response.data?.Errors || response.data?.errors,
          }
        }
      } catch (err) {
        console.error('🔥 Store - Error creating KhoanPeriod:', err)

        const errorMessage = 'Không thể tạo Kỳ Khoán. Lỗi: ' +
          (err.response?.data?.message ||
            err.response?.data?.title ||
            (err.response?.data?.errors ? JSON.stringify(err.response.data.errors) : err.message))

        this.error = errorMessage

        return {
          success: false,
          message: errorMessage,
          errors: err.response?.data?.errors,
          originalError: err,
        }
      } finally {
        this.isLoading = false
      }
    },

    async updateKhoanPeriod(periodData) {
      this.isLoading = true
      this.error = null
      try {
        const periodId = getId(periodData)

        // Map frontend data to backend format (PascalCase and proper enums)
        const updateData = {
          Id: periodId, // Ensure ID is integer
          Name: getName(periodData),
          Type: this.mapTypeToEnum(getType(periodData)),
          StartDate: periodData.StartDate || periodData.startDate,
          EndDate: periodData.EndDate || periodData.endDate,
          Status: this.mapStatusToEnum(getStatus(periodData)),
        }

        console.log('🔄 updateKhoanPeriod - original data:', periodData)
        console.log('🔄 updateKhoanPeriod - mapped data:', updateData)
        console.log('🔄 updateKhoanPeriod - periodId:', periodId, 'type:', typeof periodId)

        const response = await apiClient.put(`/KhoanPeriods/${periodId}`, updateData)

        // Check for new API response format
        if (response.data && response.data.Success) {
          await this.fetchKhoanPeriods() // Fetch lại để cập nhật
          return {
            success: true,
            data: response.data.Data,
            message: response.data.Message || 'Cập nhật kỳ khoán thành công',
          }
        } else {
          await this.fetchKhoanPeriods() // Legacy compatibility
        }
      } catch (err) {
        this.error =
          'Không thể cập nhật Kỳ Khoán. Lỗi: ' +
          (err.response?.data?.Message ||
            err.response?.data?.message ||
            err.response?.data?.title ||
            (err.response?.data?.errors ? JSON.stringify(err.response.data.errors) : err.message))
        throw err
      } finally {
        this.isLoading = false
      }
    },

    async deleteKhoanPeriod(periodId) {
      this.isLoading = true
      this.error = null
      try {
        await apiClient.delete(`/KhoanPeriods/${periodId}`)
        // Xóa khỏi state hoặc fetch lại
        this.khoanPeriods = this.khoanPeriods.filter(p => getId(p) !== periodId)
        // Hoặc await this.fetchKhoanPeriods();
      } catch (err) {
        this.error =
          'Không thể xóa Kỳ Khoán. Lỗi: ' + (err.response?.data?.message || err.response?.data?.title || err.message)
        throw err
      } finally {
        this.isLoading = false
      }
    },
  },
})
