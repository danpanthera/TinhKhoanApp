/**
 * Branch Indicators Service
 * Service để tính toán 6 chỉ tiêu chính theo chi nhánh
 */

// API Base URL với fallback cho dev environment
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '/api'
const DIRECT_API_URL = 'http://localhost:5055/api' // Fallback trực tiếp cho dev

export const branchIndicatorsService = {

  /**
   * Tính toán Nguồn vốn theo chi nhánh từ dữ liệu thô DP01
   */
  async calculateNguonVon(branchId, date = null) {
    try {
      // Chuyển đổi date thành định dạng phù hợp cho API mới
      let targetDate = new Date();
      if (date && date.trim() !== '') {
        const parsedDate = new Date(date);
        if (!isNaN(parsedDate.getTime())) {
          targetDate = parsedDate;
        }
      }

      console.log('🌐 API Call - branchId:', branchId, 'date:', targetDate.toISOString());

      const requestBody = {
        unitCode: branchId,
        targetDate: targetDate.toISOString(),
        dateType: "month" // Mặc định tính theo tháng
      };

      console.log('📋 Request body:', requestBody);

      // Thử proxy trước, nếu lỗi thì thử direct URL
      let apiUrl = `${API_BASE_URL}/NguonVon/calculate`;
      console.log('🔗 Trying proxy API URL:', apiUrl);

      let response = await fetch(apiUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestBody)
      }).catch(async (proxyError) => {
        console.warn('⚠️ Proxy failed, trying direct URL:', proxyError.message);
        apiUrl = `${DIRECT_API_URL}/NguonVon/calculate`;
        console.log('🔗 Trying direct API URL:', apiUrl);

        return fetch(apiUrl, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(requestBody)
        });
      });

      console.log('📡 Response status:', response.status, response.statusText);

      if (!response.ok) {
        console.error('❌ API Error:', response.status, response.statusText);
        throw new Error(`HTTP error! status: ${response.status}`)
      }

      const result = await response.json()
      console.log('📥 API Response:', result);

      // Chuyển đổi định dạng response để tương thích với frontend hiện tại
      if (result.success && result.data) {
        return {
          total: result.data.totalBalance,
          unitName: result.data.unitName,
          recordCount: result.data.recordCount,
          topAccounts: result.data.topAccounts,
          message: result.message
        }
      } else {
        throw new Error(result.message || 'Không thể tính toán nguồn vốn')
      }
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
