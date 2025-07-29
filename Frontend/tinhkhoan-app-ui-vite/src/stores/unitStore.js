import { defineStore } from 'pinia'
import apiClient from '../services/api.js'
import { getId } from '../utils/casingSafeAccess.js'

export const useUnitStore = defineStore('unit', {
  // State: Nơi lưu trữ dữ liệu
  state: () => ({
    units: [], // Khởi tạo units là một mảng rỗng
    isLoading: false,
    error: null,
  }),

  // Getters: Giống như computed properties, cho phép lấy dữ liệu từ state (có thể biến đổi)
  getters: {
    allUnits: state => state.units,
    unitCount: state => state.units.length,
  },

  // Actions: Nơi định nghĩa các hàm để thay đổi state, thường dùng để gọi API
  actions: {
    async fetchUnits() {
      console.log('🔄 fetchUnits started')
      this.isLoading = true
      this.error = null
      try {
        console.log('📡 Calling API: /Units')
        console.log('📡 Base URL:', import.meta.env.VITE_API_BASE_URL)
        const response = await apiClient.get('/Units')
        console.log('📨 API Response received:', response.status, response.data)
        console.log('📨 Response headers:', response.headers)

        let unitsData = []
        if (response.data && Array.isArray(response.data.$values)) {
          console.log('✅ Found $values array with length:', response.data.$values.length)
          unitsData = response.data.$values
        } else if (Array.isArray(response.data)) {
          console.log('✅ Found direct array with length:', response.data.length)
          unitsData = response.data
        } else if (response.data && typeof response.data === 'object') {
          console.log('⚠️ Response is object, trying to convert...')
          if (response.data.$id && getId(response.data)) {
            unitsData = [response.data]
          } else if (Object.keys(response.data).length > 0) {
            unitsData = [response.data]
          }
        }

        console.log('📊 Final unitsData length:', unitsData.length)
        if (unitsData.length === 0) {
          console.error('❌ Dữ liệu đơn vị không hợp lệ:', response.data)
          this.units = []
          this.error = 'Dữ liệu đơn vị nhận được không đúng định dạng.'
          return
        }
        this.units = unitsData
        console.log('✅ Units stored successfully:', this.units.length)
      } catch (err) {
        console.error('❌ fetchUnits error:', err)
        this.units = []
        this.error = 'Không thể tải danh sách đơn vị. Lỗi: ' + (err.response?.data?.message || err.message)
        console.error('Lỗi khi fetchUnits:', err)
      } finally {
        this.isLoading = false
        console.log('🔄 fetchUnits completed')
      }
    },

    async createUnit(unitData) {
      this.isLoading = true
      this.error = null
      try {
        const response = await apiClient.post('/Units', unitData) // Gọi API POST /api/Units

        if (!Array.isArray(this.units)) {
          console.warn('this.units không phải là mảng khi cố gắng thêm đơn vị mới. Khởi tạo lại units.')
          this.units = []
        }

        // Sau khi tạo thành công, backend trả về đối tượng Unit đầy đủ (không phải DTO)
        // Để đồng bộ với danh sách đang hiển thị (là UnitListItemDto), tốt nhất là fetch lại toàn bộ danh sách
        // Hoặc Sếp có thể tạo một UnitListItemDto từ response.data và push vào this.units
        // this.units.push(response.data); // Cách này có thể không đúng nếu response.data là Unit đầy đủ
        await this.fetchUnits() // Tải lại danh sách để đảm bảo dữ liệu hiển thị đồng nhất
        return response.data // Trả về đối tượng Unit đầy đủ vừa tạo từ API
      } catch (err) {
        this.error =
          'Không thể tạo đơn vị. Lỗi: ' +
          (err.response?.data?.message ||
            err.response?.data?.title ||
            (err.response?.data?.errors ? JSON.stringify(err.response.data.errors) : err.message))
        console.error('Lỗi khi createUnit:', err.response?.data || err.message, err)
        throw err
      } finally {
        this.isLoading = false
      }
    },

    async updateUnit(unitData) {
      this.isLoading = true
      this.error = null
      try {
        await apiClient.put(`/Units/${getId(unitData)}`, unitData) // unitData gửi lên là Unit đầy đủ

        // Sau khi cập nhật, tải lại danh sách để đảm bảo dữ liệu hiển thị là mới nhất và đúng định dạng DTO
        await this.fetchUnits()
      } catch (err) {
        let errorMessage = 'Không thể cập nhật đơn vị.'
        if (err.response && err.response.data) {
          if (err.response.data.errors) {
            const validationErrors = err.response.data.errors
            let messages = []
            for (const key in validationErrors) {
              messages.push(`${key}: ${validationErrors[key].join(', ')}`)
            }
            errorMessage += ' Lỗi: ' + messages.join('; ')
          } else if (err.response.data.message) {
            errorMessage += ' Lỗi: ' + err.response.data.message
          } else if (err.response.data.title) {
            // Cho ProblemDetails
            errorMessage += ' Lỗi: ' + err.response.data.title
          } else {
            errorMessage += ' Lỗi: ' + (err.message || 'Lỗi không xác định từ server.')
          }
        } else {
          errorMessage += ' Lỗi: ' + (err.message || 'Lỗi mạng hoặc server không phản hồi.')
        }
        this.error = errorMessage
        console.error('Lỗi khi updateUnit:', err.response?.data || err.message, err)
        throw err
      } finally {
        this.isLoading = false
      }
    },

    async deleteUnit(unitId) {
      this.isLoading = true
      this.error = null
      try {
        await apiClient.delete(`/Units/${unitId}`)
        // Xóa unit khỏi mảng state units dựa trên id
        this.units = this.units.filter(u => getId(u) !== unitId)
      } catch (err) {
        let errorMessage = 'Không thể xóa đơn vị.'
        if (err.response && err.response.data) {
          if (err.response.data.message) {
            errorMessage += ' Lỗi: ' + err.response.data.message
          } else if (err.response.data.title) {
            errorMessage += ' Lỗi: ' + err.response.data.title
          } else {
            errorMessage += ' Lỗi: ' + (err.message || 'Lỗi không xác định từ server.')
          }
        } else {
          errorMessage += ' Lỗi: ' + (err.message || 'Lỗi mạng hoặc server không phản hồi.')
        }
        this.error = errorMessage
        console.error('Lỗi khi deleteUnit:', err.response?.data || err.message, err)
        throw err
      } finally {
        this.isLoading = false
      }
    },
  },
})
