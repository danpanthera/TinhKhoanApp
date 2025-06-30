/**
 * Branch Indicators Service
 * Service để tính toán 6 chỉ tiêu chính theo chi nhánh
 */

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5055/api'

export const branchIndicatorsService = {

  /**
   * Tính toán Nguồn vốn theo chi nhánh
   */
  async calculateNguonVon(branchId, date = null) {
    try {
      // Validate và xử lý tham số date
      let validDate = null;
      if (date && date.trim() !== '') {
        validDate = date;
      }

      console.log('🌐 API Call - branchId:', branchId, 'date:', validDate);

      // Chỉ gửi thuộc tính date khi có giá trị hợp lệ
      const requestBody = { branchId };
      if (validDate) {
        requestBody.date = validDate;
      }

      console.log('📋 Request body:', requestBody);

      const response = await fetch(`${API_BASE_URL}/BranchIndicators/nguon-von`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestBody)
      })

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const result = await response.json()
      return result
    } catch (error) {
      console.error('❌ Lỗi tính Nguồn vốn:', error)
      throw error
    }
  },

  /**
   * Tính toán Dư nợ theo chi nhánh
   */
  async calculateDuNo(branchId, date = null) {
    try {
      // Validate và xử lý tham số date
      let validDate = null;
      if (date && date.trim() !== '') {
        validDate = date;
      }

      console.log('🌐 API Call - branchId:', branchId, 'date:', validDate);

      // Chỉ gửi thuộc tính date khi có giá trị hợp lệ
      const requestBody = { branchId };
      if (validDate) {
        requestBody.date = validDate;
      }

      console.log('📋 Request body:', requestBody);

      const response = await fetch(`${API_BASE_URL}/BranchIndicators/du-no`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestBody)
      })

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const result = await response.json()
      return result
    } catch (error) {
      console.error('❌ Lỗi tính Dư nợ:', error)
      throw error
    }
  },

  /**
   * Tính toán Nợ xấu theo chi nhánh
   */
  async calculateNoXau(branchId, date = null) {
    try {
      // Validate và xử lý tham số date
      let validDate = null;
      if (date && date.trim() !== '') {
        validDate = date;
      }

      console.log('🌐 API Call - branchId:', branchId, 'date:', validDate);

      // Chỉ gửi thuộc tính date khi có giá trị hợp lệ
      const requestBody = { branchId };
      if (validDate) {
        requestBody.date = validDate;
      }

      console.log('📋 Request body:', requestBody);

      const response = await fetch(`${API_BASE_URL}/BranchIndicators/no-xau`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestBody)
      })

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const result = await response.json()
      return result
    } catch (error) {
      console.error('❌ Lỗi tính Nợ xấu:', error)
      throw error
    }
  },

  /**
   * Tính toán Thu hồi XLRR theo chi nhánh
   */
  async calculateThuHoiXLRR(branchId, date = null) {
    try {
      // Validate và xử lý tham số date
      let validDate = null;
      if (date && date.trim() !== '') {
        validDate = date;
      }

      console.log('🌐 API Call - branchId:', branchId, 'date:', validDate);

      // Chỉ gửi thuộc tính date khi có giá trị hợp lệ
      const requestBody = { branchId };
      if (validDate) {
        requestBody.date = validDate;
      }

      console.log('📋 Request body:', requestBody);

      const response = await fetch(`${API_BASE_URL}/BranchIndicators/thu-hoi-xlrr`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestBody)
      })

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const result = await response.json()
      return result
    } catch (error) {
      console.error('❌ Lỗi tính Thu hồi XLRR:', error)
      throw error
    }
  },

  /**
   * Tính toán Thu dịch vụ theo chi nhánh
   */
  async calculateThuDichVu(branchId, date = null) {
    try {
      // Validate và xử lý tham số date
      let validDate = null;
      if (date && date.trim() !== '') {
        validDate = date;
      }

      console.log('🌐 API Call - branchId:', branchId, 'date:', validDate);

      // Chỉ gửi thuộc tính date khi có giá trị hợp lệ
      const requestBody = { branchId };
      if (validDate) {
        requestBody.date = validDate;
      }

      console.log('📋 Request body:', requestBody);

      const response = await fetch(`${API_BASE_URL}/BranchIndicators/thu-dich-vu`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestBody)
      })

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const result = await response.json()
      return result
    } catch (error) {
      console.error('❌ Lỗi tính Thu dịch vụ:', error)
      throw error
    }
  },

  /**
   * Tính toán Lợi nhuận theo chi nhánh
   */
  async calculateLoiNhuan(branchId, date = null) {
    try {
      // Validate và xử lý tham số date
      let validDate = null;
      if (date && date.trim() !== '') {
        validDate = date;
      }

      console.log('🌐 API Call - branchId:', branchId, 'date:', validDate);

      // Chỉ gửi thuộc tính date khi có giá trị hợp lệ
      const requestBody = { branchId };
      if (validDate) {
        requestBody.date = validDate;
      }

      console.log('📋 Request body:', requestBody);

      const response = await fetch(`${API_BASE_URL}/BranchIndicators/loi-nhuan`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestBody)
      })

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const result = await response.json()
      return result
    } catch (error) {
      console.error('❌ Lỗi tính Lợi nhuận:', error)
      throw error
    }
  },

  /**
   * Tính toán tất cả 6 chỉ tiêu cùng lúc
   */
  async calculateAllIndicators(branchId, date = null) {
    try {
      // Validate và xử lý tham số date
      let validDate = null;
      if (date && date.trim() !== '') {
        validDate = date;
      }

      console.log(`🧮 Tính toán tất cả chỉ tiêu cho chi nhánh: ${branchId}, date: ${validDate}`);

      // Chỉ gửi thuộc tính date khi có giá trị hợp lệ
      const requestBody = { branchId };
      if (validDate) {
        requestBody.date = validDate;
      }

      console.log('📋 Request body:', requestBody);

      const response = await fetch(`${API_BASE_URL}/BranchIndicators/all-indicators`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestBody)
      })

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const result = await response.json()
      console.log(`✅ Kết quả tính toán cho ${branchId}:`, result)
      return result
    } catch (error) {
      console.error('❌ Lỗi tính tất cả chỉ tiêu:', error)
      throw error
    }
  },

  /**
   * Format số tiền
   */
  formatCurrency(value) {
    if (value === null || value === undefined) return '0'
    return new Intl.NumberFormat('vi-VN').format(value)
  },

  /**
   * Format phần trăm
   */
  formatPercentage(value) {
    if (value === null || value === undefined) return '0%'
    return `${value.toFixed(2)}%`
  }
}

export default branchIndicatorsService
