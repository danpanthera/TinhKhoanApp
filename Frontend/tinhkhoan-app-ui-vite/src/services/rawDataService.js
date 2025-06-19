// 🗄️ rawDataService.js - Dịch vụ xử lý Kho Dữ liệu Thô
import api from './api';

class RawDataService {
  constructor() {
    this.baseURL = '/rawdata';
  }

  // 📋 Lấy danh sách tất cả dữ liệu thô đã import
  async getAllImports() {
    try {
      const response = await api.get(this.baseURL);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi lấy danh sách import:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 📤 Import dữ liệu theo loại
  async importData(dataType, files, options = {}) {
    try {
      const formData = new FormData();
      
      // Thêm files vào FormData
      files.forEach(file => {
        formData.append('Files', file);
      });

      // Thêm các tùy chọn
      if (options.archivePassword) {
        formData.append('ArchivePassword', options.archivePassword);
      }
      
      if (options.notes) {
        formData.append('Notes', options.notes);
      }

      const response = await api.post(`${this.baseURL}/import/${dataType}`, formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        },
        timeout: 600000 // 10 phút timeout cho upload file lớn
      });

      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error(`❌ Lỗi import dữ liệu ${dataType}:`, error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 👁️ Xem trước dữ liệu đã import
  async previewData(importId) {
    try {
      const response = await api.get(`${this.baseURL}/${importId}/preview`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi xem trước dữ liệu:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 🗑️ Xóa dữ liệu import
  async deleteImport(importId) {
    try {
      const response = await api.delete(`${this.baseURL}/${importId}`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi xóa dữ liệu import:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 📅 Lấy dữ liệu import theo ngày sao kê
  async getImportsByStatementDate(statementDate, fileType = null) {
    try {
      const params = new URLSearchParams();
      if (statementDate) {
        params.append('statementDate', statementDate);
      }
      if (fileType) {
        params.append('fileType', fileType);
      }
      
      const response = await api.get(`/DataImport/by-statement-date?${params.toString()}`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi lấy dữ liệu theo ngày sao kê:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 📦 Lấy dữ liệu đã nén
  async getDecompressedData(importId) {
    try {
      const response = await api.get(`/DataImport/${importId}/decompress`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi giải nén dữ liệu:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 📤 Import dữ liệu mới với tính năng statement date và compression
  async importDataNew(files, category = 'General') {
    try {
      const formData = new FormData();
      
      // Thêm files vào FormData
      files.forEach(file => {
        formData.append('Files', file);
      });

      // Thêm category
      formData.append('Category', category);

      const response = await api.post('/DataImport/upload', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        },
        timeout: 300000 // 5 phút timeout cho upload file lớn
      });

      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi import dữ liệu:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 📋 Lấy tất cả dữ liệu import từ API mới
  async getAllImportsNew() {
    try {
      const response = await api.get('/DataImport');
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi lấy danh sách import:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 🗑️ Xóa toàn bộ dữ liệu import
  async clearAllData() {
    try {
      const response = await api.delete(`${this.baseURL}/clear-all`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi xóa toàn bộ dữ liệu:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 🔍 Kiểm tra dữ liệu trùng lặp theo ngày sao kê
  async checkDuplicateData(dataType, statementDate, fileName = null) {
    try {
      let url = `${this.baseURL}/check-duplicate/${dataType}/${statementDate}`;
      if (fileName) {
        url += `?fileName=${encodeURIComponent(fileName)}`;
      }
      const response = await api.get(url);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi kiểm tra dữ liệu trùng lặp:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 🗑️ Xóa dữ liệu theo ngày sao kê
  async deleteByStatementDate(dataType, statementDate) {
    try {
      const response = await api.delete(`${this.baseURL}/by-date/${dataType}/${statementDate}`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi xóa dữ liệu theo ngày:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 📋 Lấy dữ liệu theo ngày sao kê
  async getByStatementDate(dataType, statementDate) {
    try {
      const response = await api.get(`${this.baseURL}/by-date/${dataType}/${statementDate}`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi lấy dữ liệu theo ngày:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 📋 Lấy dữ liệu theo khoảng ngày
  async getByDateRange(dataType, fromDate, toDate) {
    try {
      const response = await api.get(`${this.baseURL}/by-date-range/${dataType}?fromDate=${fromDate}&toDate=${toDate}`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi lấy dữ liệu theo khoảng ngày:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 🗄️ Lấy dữ liệu thô trực tiếp từ bảng động
  async getRawDataFromTable(dataType, statementDate = null) {
    try {
      const params = new URLSearchParams();
      if (statementDate) {
        params.append('statementDate', statementDate);
      }
      
      const url = `${this.baseURL}/table/${dataType}${params.toString() ? '?' + params.toString() : ''}`;
      const response = await api.get(url);
      
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error(`❌ Lỗi lấy dữ liệu thô từ bảng ${dataType}:`, error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server',
        details: error.response?.data?.details || null
      };
    }
  }

  // 🔧 Utility methods

  // 📋 Định nghĩa các loại dữ liệu và mô tả
  getDataTypeDefinitions() {
    return {
      'LN01': {
        name: 'LN01',
        description: 'Dữ liệu LOAN',
        icon: '💰',
        acceptedFormats: ['.csv', '.xlsx', '.xls'],
        requiredKeyword: 'LN01'
      },
      'LN03': {
        name: 'LN03', 
        description: 'Dữ liệu Nợ XLRR',
        icon: '📊',
        acceptedFormats: ['.csv', '.xlsx', '.xls'],
        requiredKeyword: 'LN03'
      },
      'DP01': {
        name: 'DP01',
        description: 'Dữ liệu Tiền gửi',
        icon: '🏦',
        acceptedFormats: ['.csv', '.xlsx', '.xls'],
        requiredKeyword: 'DP01'
      },
      'EI01': {
        name: 'EI01',
        description: 'Dữ liệu mobile banking',
        icon: '📱',
        acceptedFormats: ['.csv', '.xlsx', '.xls'],
        requiredKeyword: 'EI01'
      },
      'GL01': {
        name: 'GL01',
        description: 'Dữ liệu bút toán GDV',
        icon: '✍️',
        acceptedFormats: ['.csv', '.xlsx', '.xls'],
        requiredKeyword: 'GL01'
      },
      'DPDA': {
        name: 'DPDA',
        description: 'Dữ liệu sao kê phát hành thẻ',
        icon: '💳',
        acceptedFormats: ['.csv', '.xlsx', '.xls'],
        requiredKeyword: 'DPDA'
      },
      'DB01': {
        name: 'DB01',
        description: 'Sao kê TSDB và Không TSDB',
        icon: '📋',
        acceptedFormats: ['.csv', '.xlsx', '.xls'],
        requiredKeyword: 'DB01'
      },
      'KH03': {
        name: 'KH03',
        description: 'Sao kê Khách hàng pháp nhân',
        icon: '🏢',
        acceptedFormats: ['.csv', '.xlsx', '.xls'],
        requiredKeyword: 'KH03'
      },
      'BC57': {
        name: 'BC57',
        description: 'Sao kê Lãi dự thu',
        icon: '📈',
        acceptedFormats: ['.csv', '.xlsx', '.xls'],
        requiredKeyword: 'BC57'
      }
    };
  }

  // 🔍 Validate file for import
  validateFile(file, dataType) {
    if (!file) {
      return { valid: false, error: 'File không hợp lệ' }
    }
    
    if (!dataType) {
      return { valid: false, error: 'Chưa chọn loại dữ liệu' }
    }
    
    const dataTypeDef = this.getDataTypeDefinitions()[dataType]
    if (!dataTypeDef) {
      return { valid: false, error: 'Loại dữ liệu không được hỗ trợ' }
    }
    
    const fileName = file.name.toLowerCase()
    const validExtensions = [...dataTypeDef.acceptedFormats, '.zip', '.7z', '.rar']
    const hasValidExtension = validExtensions.some(ext => fileName.endsWith(ext.toLowerCase()))
    
    if (!hasValidExtension) {
      return { 
        valid: false, 
        error: `File phải có định dạng: ${validExtensions.join(', ')}` 
      }
    }
    
    // Check if filename contains data type (for non-archive files)
    if (!this.isArchiveFile(fileName) && !fileName.includes(dataType.toLowerCase())) {
      return { 
        valid: false, 
        error: `Tên file phải chứa mã loại dữ liệu '${dataType}'` 
      }
    }
    
    return { valid: true }
  }

  // 🗂️ Kiểm tra file nén
  isArchiveFile(fileName) {
    const archiveExtensions = ['.zip', '.7z', '.rar', '.tar', '.gz'];
    const extension = '.' + fileName.split('.').pop().toLowerCase();
    return archiveExtensions.includes(extension);
  }

  // 📅 Trích xuất ngày từ tên file (yyyymmdd)
  extractDateFromFileName(fileName) {
    const dateMatch = fileName.match(/(\d{8})/);
    if (dateMatch) {
      const dateStr = dateMatch[1];
      const year = dateStr.substring(0, 4);
      const month = dateStr.substring(4, 6);
      const day = dateStr.substring(6, 8);
      
      try {
        const date = new Date(year, month - 1, day);
        if (!isNaN(date.getTime())) {
          return date;
        }
      } catch (e) {
        // Ignore date parsing errors
      }
    }
    return null;
  }

  // 🎨 Lấy màu sắc cho loại dữ liệu
  getDataTypeColor(dataType) {
    const colors = {
      'LN01': '#10B981', // green
      'LN03': '#F59E0B', // amber
      'DP01': '#3B82F6', // blue
      'EI01': '#8B5CF6', // purple
      'GL01': '#EF4444', // red
      'DPDA': '#06B6D4', // cyan
      'DB01': '#84CC16', // lime
      'KH03': '#F97316', // orange
      'BC57': '#EC4899'  // pink
    };
    return colors[dataType] || '#6B7280'; // gray default
  }

  // 📊 Format số lượng records
  formatRecordCount(count) {
    if (count >= 1000000) {
      return `${(count / 1000000).toFixed(1)}M`;
    } else if (count >= 1000) {
      return `${(count / 1000).toFixed(1)}K`;
    }
    return count.toString();
  }

  // 📅 Format ngày
  formatDate(date) {
    if (!date) return 'N/A';
    
    if (typeof date === 'string') {
      date = new Date(date);
    }
    
    return date.toLocaleDateString('vi-VN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  // 📊 Lấy thống kê tổng quan
  getImportStats(imports) {
    const stats = {
      totalImports: imports.length,
      totalRecords: 0,
      dataTypes: {},
      recentImports: [],
      successfulImports: 0,
      failedImports: 0
    };

    imports.forEach(imp => {
      stats.totalRecords += imp.recordsCount || 0;
      
      // Đếm theo loại dữ liệu
      if (!stats.dataTypes[imp.dataType]) {
        stats.dataTypes[imp.dataType] = {
          count: 0,
          records: 0
        };
      }
      stats.dataTypes[imp.dataType].count++;
      stats.dataTypes[imp.dataType].records += imp.recordsCount || 0;

      // Đếm thành công/thất bại
      if (imp.status === 'Completed') {
        stats.successfulImports++;
      } else if (imp.status === 'Failed') {
        stats.failedImports++;
      }
    });

    // Lấy 5 import gần nhất
    stats.recentImports = imports
      .sort((a, b) => new Date(b.importDate) - new Date(a.importDate))
      .slice(0, 5);

    return stats;
  }

  // ⚡ Performance Optimized APIs
  
  // 📋 Lấy danh sách imports với pagination và caching
  async getOptimizedImports(page = 1, pageSize = 50, searchTerm = '', sortBy = 'ImportDate', sortOrder = 'desc') {
    try {
      const params = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString(),
        searchTerm: searchTerm || '',
        sortBy: sortBy || 'ImportDate',
        sortOrder: sortOrder || 'desc'
      });

      const response = await api.get(`${this.baseURL}/optimized/imports?${params.toString()}`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi lấy danh sách imports tối ưu:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 📊 Lấy records với virtual scrolling
  async getOptimizedRecords(importId, offset = 0, limit = 100, searchTerm = '') {
    try {
      const params = new URLSearchParams({
        offset: offset.toString(),
        limit: limit.toString(),
        searchTerm: searchTerm || ''
      });

      const response = await api.get(`${this.baseURL}/optimized/records/${importId}?${params.toString()}`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi lấy records tối ưu:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 🔄 Lấy SCD records với tối ưu hóa
  async getOptimizedSCDRecords(tableName, page = 1, pageSize = 50, entityId = null, fromDate = null, toDate = null) {
    try {
      const params = new URLSearchParams({
        tableName: tableName,
        page: page.toString(),
        pageSize: pageSize.toString()
      });

      if (entityId) params.append('entityId', entityId);
      if (fromDate) params.append('fromDate', fromDate);
      if (toDate) params.append('toDate', toDate);

      const response = await api.get(`${this.baseURL}/optimized/scd?${params.toString()}`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi lấy SCD records tối ưu:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 📈 Lấy dashboard statistics với caching
  async getDashboardStats() {
    try {
      const response = await api.get(`${this.baseURL}/optimized/dashboard-stats`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi lấy dashboard stats:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 🎯 Tìm kiếm nâng cao với full-text search
  async advancedSearch(searchTerm, dataTypes = [], dateFrom = null, dateTo = null, page = 1, pageSize = 50) {
    try {
      const params = new URLSearchParams({
        searchTerm: searchTerm || '',
        page: page.toString(),
        pageSize: pageSize.toString()
      });

      if (dataTypes.length > 0) {
        dataTypes.forEach(type => params.append('dataTypes', type));
      }
      if (dateFrom) params.append('dateFrom', dateFrom);
      if (dateTo) params.append('dateTo', dateTo);

      const response = await api.get(`${this.baseURL}/optimized/search?${params.toString()}`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi tìm kiếm nâng cao:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 📊 Lấy thống kê hiệu suất theo thời gian
  async getPerformanceStats(timeRange = '24h') {
    try {
      const response = await api.get(`${this.baseURL}/optimized/performance-stats?timeRange=${timeRange}`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi lấy thống kê hiệu suất:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 🔄 Refresh cache cho một endpoint cụ thể
  async refreshCache(cacheKey) {
    try {
      const response = await api.post(`${this.baseURL}/optimized/refresh-cache`, { cacheKey });
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi refresh cache:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 🎨 Performance utility methods
  
  // 📊 Format performance metrics
  formatPerformanceMetric(value, unit = 'ms') {
    if (value < 1000 && unit === 'ms') {
      return `${Math.round(value)}ms`;
    } else if (value >= 1000 && unit === 'ms') {
      return `${(value / 1000).toFixed(2)}s`;
    }
    return `${value}${unit}`;
  }

  // 🎯 Calculate cache hit rate percentage
  calculateCacheHitRate(hits, misses) {
    const total = hits + misses;
    if (total === 0) return 0;
    return Math.round((hits / total) * 100);
  }

  // 📈 Get performance color based on response time
  getPerformanceColor(responseTime) {
    if (responseTime < 100) return '#52c41a'; // green - excellent
    if (responseTime < 500) return '#faad14'; // yellow - good
    if (responseTime < 1000) return '#fa8c16'; // orange - fair
    return '#f5222d'; // red - poor
  }

  // 🔍 Format search highlight
  formatSearchHighlight(text, searchTerm) {
    if (!searchTerm || !text) return text;
    
    const regex = new RegExp(`(${searchTerm})`, 'gi');
    return text.replace(regex, '<mark>$1</mark>');
  }
}

export default new RawDataService();
