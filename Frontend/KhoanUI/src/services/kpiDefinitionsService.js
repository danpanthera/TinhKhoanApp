import api from './api'

// Service để lấy danh sách KPI cho dropdown với encoding UTF-8 đúng
export const kpiDefinitionsService = {
  // 👥 Lấy danh sách KPI cho dropdown Cán bộ
  async getKpiDropdownForCanBo() {
    try {
      const response = await api.get('/KPIDefinitions/dropdown/canbo')

      // Đảm bảo data được decode UTF-8 đúng cách
      let kpis = []
      if (response.data && Array.isArray(response.data.$values)) {
        kpis = response.data.$values
      } else if (Array.isArray(response.data)) {
        kpis = response.data
      }

      // Format data cho dropdown hiển thị mô tả tiếng Việt
      return kpis.map(kpi => ({
        id: kpi.id,
        code: kpi.code,
        name: kpi.name, // Hiển thị mô tả tiếng Việt thay vì code
        description: kpi.description,
        maxScore: kpi.maxScore,
        unitOfMeasure: kpi.unitOfMeasure,
        displayText: `${kpi.name}${kpi.unitOfMeasure ? ` (${kpi.unitOfMeasure})` : ''}`, // Text cho dropdown
      }))
    } catch (error) {
      console.error('Lỗi khi lấy KPI cho cán bộ:', error)
      throw new Error('Không thể tải danh sách KPI cho cán bộ')
    }
  },

  // 🏢 Lấy danh sách KPI cho dropdown Chi nhánh
  async getKpiDropdownForChiNhanh() {
    try {
      const response = await api.get('/KPIDefinitions/dropdown/chinhanh')

      // Đảm bảo data được decode UTF-8 đúng cách
      let kpis = []
      if (response.data && Array.isArray(response.data.$values)) {
        kpis = response.data.$values
      } else if (Array.isArray(response.data)) {
        kpis = response.data
      }

      // Format data cho dropdown hiển thị mô tả tiếng Việt
      return kpis.map(kpi => ({
        id: kpi.id,
        code: kpi.code,
        name: kpi.name, // Hiển thị mô tả tiếng Việt thay vì code
        description: kpi.description,
        maxScore: kpi.maxScore,
        unitOfMeasure: kpi.unitOfMeasure,
        displayText: `${kpi.name}${kpi.unitOfMeasure ? ` (${kpi.unitOfMeasure})` : ''}`, // Text cho dropdown
      }))
    } catch (error) {
      console.error('Lỗi khi lấy KPI cho chi nhánh:', error)
      throw new Error('Không thể tải danh sách KPI cho chi nhánh')
    }
  },

  // 📊 Lấy thống kê KPI
  async getKpiStats() {
    try {
      const response = await api.get('/KPIDefinitions/stats')
      return response.data
    } catch (error) {
      console.error('Lỗi khi lấy thống kê KPI:', error)
      throw new Error('Không thể tải thống kê KPI')
    }
  },

  // 📋 Lấy tất cả KPI (legacy)
  async getAllKpi() {
    try {
      const response = await api.get('/KPIDefinitions')

      let kpis = []
      if (response.data && Array.isArray(response.data.$values)) {
        kpis = response.data.$values
      } else if (Array.isArray(response.data)) {
        kpis = response.data
      }

      return kpis
    } catch (error) {
      console.error('Lỗi khi lấy tất cả KPI:', error)
      throw new Error('Không thể tải danh sách KPI')
    }
  },

  // 🔍 Lấy KPI theo ID
  async getKpiById(id) {
    try {
      const response = await api.get(`/KPIDefinitions/${id}`)
      return response.data
    } catch (error) {
      console.error(`Lỗi khi lấy KPI ID ${id}:`, error)
      throw new Error(`Không thể tải KPI ID ${id}`)
    }
  },

  // ➕ Tạo KPI mới
  async createKpi(kpiData) {
    try {
      const response = await api.post('/KPIDefinitions', kpiData)
      return response.data
    } catch (error) {
      console.error('Lỗi khi tạo KPI:', error)
      throw new Error('Không thể tạo KPI mới')
    }
  },

  // ✏️ Cập nhật KPI
  async updateKpi(id, kpiData) {
    try {
      const response = await api.put(`/KPIDefinitions/${id}`, kpiData)
      return response.data
    } catch (error) {
      console.error(`Lỗi khi cập nhật KPI ID ${id}:`, error)
      throw new Error(`Không thể cập nhật KPI ID ${id}`)
    }
  },

  // 🗑️ Xóa KPI
  async deleteKpi(id) {
    try {
      await api.delete(`/KPIDefinitions/${id}`)
      return { success: true }
    } catch (error) {
      console.error(`Lỗi khi xóa KPI ID ${id}:`, error)
      throw new Error(`Không thể xóa KPI ID ${id}`)
    }
  },
}

export default kpiDefinitionsService
