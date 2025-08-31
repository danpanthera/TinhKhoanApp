/**
 * 🎭 LN03 Mock Service for Frontend Development
 * Provides fake data for UI testing while backend is being fixed
 */

// Generate mock data similar to the CSV structure
const generateMockLN03Data = () => {
  const branches = ['7800', '7801', '7802', '7803']
  const branchNames = {
    '7800': 'Chi nhánh Tỉnh Lai Châu',
    '7801': 'Chi nhánh Phong Thổ',
    '7802': 'Chi nhánh Tân Uyên',
    '7803': 'Chi nhánh Mường Lay',
  }

  const customers = [
    { code: "'004065046", name: 'Bùi Thị Linh' },
    { code: "'004065047", name: 'Nguyễn Văn Nam' },
    { code: "'004065048", name: 'Trần Thị Mai' },
    { code: "'004065049", name: 'Lê Văn Đức' },
    { code: "'004065050", name: 'Phạm Thị Hoa' },
  ]

  const officers = [
    { code: '780000424', name: 'Nguyễn Văn Hùng' },
    { code: '780000425', name: 'Trần Thị Lan' },
    { code: '780000426', name: 'Lê Văn Minh' },
    { code: '780000427', name: 'Phạm Thị Thu' },
  ]

  const mockRecords = []

  for (let i = 1; i <= 50; i++) {
    const branch = branches[Math.floor(Math.random() * branches.length)]
    const customer = customers[Math.floor(Math.random() * customers.length)]
    const officer = officers[Math.floor(Math.random() * officers.length)]

    // Generate random date between 2019 and 2024
    const startDate = new Date('2019-01-01')
    const endDate = new Date('2024-12-31')
    const randomDate = new Date(startDate.getTime() + Math.random() * (endDate.getTime() - startDate.getTime()))

    mockRecords.push({
      id: i,
      ngayDL: randomDate.toISOString().split('T')[0],
      machinhAnh: branch,
      tenChiNhanh: branchNames[branch],
      maKH: customer.code,
      tenKH: customer.name,
      maCBTD: officer.code,
      tenCBTD: officer.name,
      soHopDong: `${branch}-LAV-${2015 + Math.floor(Math.random() * 10)}${String(Math.floor(Math.random() * 10000)).padStart(5, '0')}`,
      soTien1: Math.floor(Math.random() * 10000000000), // SOTIENXLRR
      soTien2: Math.floor(Math.random() * 5000000000),  // DUNONOIBANG
      laiSuat: (Math.random() * 15).toFixed(2), // Random interest rate 0-15%
      taiKhoanHachToan: `97${Math.floor(Math.random() * 10000)}`,
      refNo: `${branch}${customer.code}${Math.random().toString(36).substr(2, 20)}`.substr(0, 50),
      loaiNguonVon: Math.random() > 0.5 ? 'Cá nhân' : 'Tổ chức',
      nhomNo: `${Math.floor(Math.random() * 5) + 1}`,
      ngayPhatSinhXL: randomDate.toISOString().split('T')[0],
      maPGD: `'${String(Math.floor(Math.random() * 100)).padStart(2, '0')}`,
      conLaiNgoaiBang: Math.floor(Math.random() * 8000000000),
      column_18: `COL18_${Math.random().toString(36).substr(2, 8)}`,
      column_19: Math.floor(Math.random() * 1000000),
      column_20: `COL20_${Math.random().toString(36).substr(2, 8)}`,
      createdDate: new Date().toISOString(),
      updatedDate: null,
    })
  }

  return mockRecords
}

// Mock API responses
const mockLN03Service = {
  async getCount() {
    await new Promise(resolve => setTimeout(resolve, 500)) // Simulate API delay
    return {
      success: true,
      data: 273,
      message: "Record count retrieved successfully (MOCK)",
    }
  },

  async getRecords(params = {}) {
    await new Promise(resolve => setTimeout(resolve, 800))
    const allData = generateMockLN03Data()

    const page = params.page || 1
    const pageSize = params.pageSize || 25
    const startIndex = (page - 1) * pageSize

    let filteredData = allData

    // Apply filters
    if (params.keyword) {
      const keyword = params.keyword.toLowerCase()
      filteredData = filteredData.filter(item =>
        item.maKH.toLowerCase().includes(keyword) ||
        item.tenKH.toLowerCase().includes(keyword) ||
        item.machinhAnh.toLowerCase().includes(keyword) ||
        item.soHopDong.toLowerCase().includes(keyword),
      )
    }

    if (params.branchCode) {
      filteredData = filteredData.filter(item =>
        item.machinhAnh === params.branchCode,
      )
    }

    if (params.startDate && params.endDate) {
      filteredData = filteredData.filter(item => {
        const itemDate = new Date(item.ngayDL)
        return itemDate >= new Date(params.startDate) && itemDate <= new Date(params.endDate)
      })
    }

    const paginatedData = filteredData.slice(startIndex, startIndex + pageSize)

    return {
      success: true,
      data: {
        items: paginatedData,
        totalCount: filteredData.length,
        page: page,
        pageSize: pageSize,
        totalPages: Math.ceil(filteredData.length / pageSize),
      },
      message: "Records retrieved successfully (MOCK)",
    }
  },

  async getSummary() {
    await new Promise(resolve => setTimeout(resolve, 600))
    return {
      success: true,
      data: {
        totalRecords: 273,
        branchCount: 4,
        totalAmount: 50000000000,
        lastUpdateDate: '2024-12-31',
        averageLoanAmount: 183150183,
        topBranch: '7800',
        oldestRecord: '2019-06-28',
        newestRecord: '2024-12-31',
      },
      message: "Summary retrieved successfully (MOCK)",
    }
  },

  async getByBranch(branchCode, params = {}) {
    await new Promise(resolve => setTimeout(resolve, 700))
    const allData = generateMockLN03Data()
    const branchData = allData.filter(item => item.machinhAnh === branchCode)

    return {
      success: true,
      data: branchData.slice(0, params.pageSize || 10),
      message: `Records for branch ${branchCode} retrieved successfully (MOCK)`,
    }
  },

  async importCsv(file, replaceExistingData = false) {
    await new Promise(resolve => setTimeout(resolve, 2000)) // Simulate longer import process

    // Simulate various import results
    const scenarios = [
      { successCount: 273, errorCount: 0, message: "All records imported successfully" },
      { successCount: 269, errorCount: 4, message: "Most records imported, some validation errors" },
      { successCount: 0, errorCount: 273, message: "Import failed - data format issues" },
    ]

    const result = scenarios[0] // Always use successful scenario for demo

    return {
      success: result.successCount > 0,
      data: {
        successCount: result.successCount,
        errorCount: result.errorCount,
        totalRecords: result.successCount + result.errorCount,
        fileName: file.name,
        importTime: new Date().toISOString(),
        replacedExistingData: replaceExistingData,
      },
      message: `${result.message} (MOCK)`,
    }
  },

  async exportToCsv(params = {}) {
    await new Promise(resolve => setTimeout(resolve, 1500))

    // Create mock CSV content
    const csvContent = `MACHINHANH,TENCHINHANH,MAKH,TENKH,SOHOPDONG,SOTIENXLRR,NGAYPHATSINHXL,CONLAINGOAIBANG,DUNONOIBANG,NHOMNO,MACBTD,TENCBTD,MAPGD,TAIKHOANHACHTOAN,REFNO,LOAINGUONVON,COLUMN_18,COLUMN_19,COLUMN_20
"7800","Chi nhánh Tỉnh Lai Châu","'004065046","Bùi Thị Linh","7800-LAV-201500567","1000000","20190628","1000000","0","1","780000424","Nguyễn Văn Hùng","'00","971103","78000040650467800-LAV-2015005677800LDS201500736","Cá nhân","COL18_ABC","500000","COL20_XYZ"
"7800","Chi nhánh Tỉnh Lai Châu","'004065047","Nguyễn Văn Nam","7800-LAV-201600123","5000000","20200115","5000000","0","2","780000425","Trần Thị Lan","'01","971104","78000040650477800-LAV-2016001237800LDS201600456","Cá nhân","COL18_DEF","750000","COL20_UVW"`

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
    return blob
  },

  async getTemporalHistory(id) {
    await new Promise(resolve => setTimeout(resolve, 600))

    const mockHistory = [
      {
        id: id,
        maKH: "'004065046",
        soTien1: 1000000,
        soTien2: 0,
        laiSuat: 8.5,
        sysStartTime: '2024-01-15T10:30:00',
        sysEndTime: '2024-06-20T14:15:00',
      },
      {
        id: id,
        maKH: "'004065046",
        soTien1: 1500000,
        soTien2: 500000,
        laiSuat: 9.0,
        sysStartTime: '2024-06-20T14:15:00',
        sysEndTime: '9999-12-31T23:59:59',
      },
    ]

    return {
      success: true,
      data: mockHistory,
      message: `Temporal history for record ${id} retrieved successfully (MOCK)`,
    }
  },

  async getAvailableDates() {
    await new Promise(resolve => setTimeout(resolve, 400))

    const dates = []
    const startDate = new Date('2019-06-01')
    const endDate = new Date('2024-12-31')

    for (let d = new Date(startDate); d <= endDate; d.setMonth(d.getMonth() + 3)) {
      dates.push(d.toISOString().split('T')[0])
    }

    return {
      success: true,
      data: dates,
      message: "Available dates retrieved successfully (MOCK)",
    }
  },
}

// Export for use in components
export default mockLN03Service

// Enable/disable mock mode
export const MOCK_MODE = false // Set to false when backend is ready

// Helper to switch between real and mock service
export const getLN03Service = () => {
  if (MOCK_MODE) {
    console.log('🎭 Using MOCK LN03 Service for frontend development')
    return mockLN03Service
  } else {
    // Import real service when ready
    return import('./ln03Service').then(module => module.default)
  }
}
