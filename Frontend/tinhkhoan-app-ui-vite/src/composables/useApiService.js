// Composable để xử lý các API calls thông dụng
// Đây là một abstraction layer cho việc gọi API

import api from '../services/api.js'

/**
 * Composable cung cấp các method tiện ích để gọi API
 * @returns {Object} Object chứa các method get, post, put, delete
 */
export function useApiService() {
  /**
   * Method GET - Lấy dữ liệu từ API
   * @param {string} url - Đường dẫn API endpoint
   * @param {Object} config - Cấu hình thêm cho request
   * @returns {Promise} Promise chứa data response
   */
  const get = async (url, config = {}) => {
    try {
      const response = await api.get(url, config)

      // Xử lý response data - nếu có $values thì unwrap nó
      let data = response.data
      if (data && data.$values && Array.isArray(data.$values)) {
        data = data.$values
      }

      return data
    } catch (error) {
      console.error(`❌ Lỗi GET ${url}:`, error)
      throw error
    }
  }

  /**
   * Method POST - Gửi dữ liệu lên API
   * @param {string} url - Đường dẫn API endpoint
   * @param {Object} data - Dữ liệu gửi lên
   * @param {Object} config - Cấu hình thêm cho request
   * @returns {Promise} Promise chứa data response
   */
  const post = async (url, data = {}, config = {}) => {
    try {
      const response = await api.post(url, data, config)

      // Xử lý response data
      let responseData = response.data
      if (responseData && responseData.$values && Array.isArray(responseData.$values)) {
        responseData = responseData.$values
      }

      return responseData
    } catch (error) {
      console.error(`❌ Lỗi POST ${url}:`, error)
      throw error
    }
  }

  /**
   * Method PUT - Cập nhật dữ liệu
   * @param {string} url - Đường dẫn API endpoint
   * @param {Object} data - Dữ liệu cập nhật
   * @param {Object} config - Cấu hình thêm cho request
   * @returns {Promise} Promise chứa data response
   */
  const put = async (url, data = {}, config = {}) => {
    try {
      const response = await api.put(url, data, config)

      // Xử lý response data
      let responseData = response.data
      if (responseData && responseData.$values && Array.isArray(responseData.$values)) {
        responseData = responseData.$values
      }

      return responseData
    } catch (error) {
      console.error(`❌ Lỗi PUT ${url}:`, error)
      throw error
    }
  }

  /**
   * Method DELETE - Xóa dữ liệu
   * @param {string} url - Đường dẫn API endpoint
   * @param {Object} config - Cấu hình thêm cho request
   * @returns {Promise} Promise chứa data response
   */
  const del = async (url, config = {}) => {
    try {
      const response = await api.delete(url, config)

      // Xử lý response data
      let responseData = response.data
      if (responseData && responseData.$values && Array.isArray(responseData.$values)) {
        responseData = responseData.$values
      }

      return responseData
    } catch (error) {
      console.error(`❌ Lỗi DELETE ${url}:`, error)
      throw error
    }
  }

  // Trả về các method để sử dụng
  return {
    get,
    post,
    put,
    delete: del, // 'delete' là từ khóa reserved nên dùng 'del'
  }
}

// Export default để có thể import theo nhiều cách
export default useApiService
