// 🕒 temporalService.js - Dịch vụ xử lý SQL Server Temporal Tables thay thế SCD Type 2
import api from './api';

class TemporalService {
  constructor() {
    this.baseURL = '/temporal';
  }

  // 🔍 Query temporal data với filters và pagination
  async queryTemporalData(queryRequest) {
    try {
      const response = await api.post(`${this.baseURL}/query`, queryRequest);
      return {
        success: true,
        data: response.data.data,
        message: response.data.message
      };
    } catch (error) {
      console.error('❌ Lỗi query temporal data:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 📚 Lấy lịch sử thay đổi của một entity
  async getEntityHistory(entityId, fromDate = null, toDate = null) {
    try {
      const params = {};
      if (fromDate) params.fromDate = fromDate;
      if (toDate) params.toDate = toDate;

      const response = await api.get(`${this.baseURL}/history/${entityId}`, { params });
      return {
        success: true,
        data: response.data.data,
        message: response.data.message
      };
    } catch (error) {
      console.error('❌ Lỗi lấy lịch sử entity:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // ⏰ Lấy trạng thái entity tại thời điểm cụ thể
  async getAsOfDate(entityId, asOfDate) {
    try {
      const response = await api.get(`${this.baseURL}/as-of/${entityId}`, {
        params: { asOfDate }
      });
      return {
        success: true,
        data: response.data.data,
        message: response.data.message
      };
    } catch (error) {
      console.error('❌ Lỗi lấy entity as-of-date:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 🛠️ Bật temporal table cho một bảng
  async enableTemporalTable(tableName) {
    try {
      const response = await api.post(`${this.baseURL}/enable/${tableName}`);
      return {
        success: true,
        message: response.data.message
      };
    } catch (error) {
      console.error('❌ Lỗi bật temporal table:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 📈 Tạo columnstore index
  async createColumnstoreIndex(tableName, indexName, columns) {
    try {
      const response = await api.post(`${this.baseURL}/index/${tableName}`, {
        indexName,
        columns
      });
      return {
        success: true,
        message: response.data.message
      };
    } catch (error) {
      console.error('❌ Lỗi tạo columnstore index:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 📊 Lấy thống kê temporal table
  async getTemporalStatistics(tableName) {
    try {
      const response = await api.get(`${this.baseURL}/statistics/${tableName}`);
      return {
        success: true,
        data: response.data.data,
        message: response.data.message
      };
    } catch (error) {
      console.error('❌ Lỗi lấy thống kê temporal:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 🔍 So sánh dữ liệu giữa hai thời điểm (thay thế SCD Type 2 comparison)
  async compareTemporalData(date1, date2, filter = null) {
    try {
      const response = await api.post(`${this.baseURL}/compare`, {
        date1,
        date2,
        filter
      });
      return {
        success: true,
        data: response.data.data,
        message: response.data.message
      };
    } catch (error) {
      console.error('❌ Lỗi so sánh temporal data:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 📅 Lấy dữ liệu trong khoảng thời gian
  async getDataBetweenDates(fromDate, toDate, options = {}) {
    const queryRequest = {
      fromDate,
      toDate,
      filter: options.filter || null,
      orderBy: options.orderBy || 'ImportDate DESC',
      page: options.page || 1,
      pageSize: options.pageSize || 50
    };

    return await this.queryTemporalData(queryRequest);
  }

  // 📊 Lấy dữ liệu tại một thời điểm cụ thể
  async getDataAsOf(asOfDate, options = {}) {
    const queryRequest = {
      asOfDate,
      filter: options.filter || null,
      orderBy: options.orderBy || 'ImportDate DESC',
      page: options.page || 1,
      pageSize: options.pageSize || 50
    };

    return await this.queryTemporalData(queryRequest);
  }

  // 🔄 Lấy tất cả lịch sử thay đổi (thay thế SCD Type 2 history)
  async getAllHistory(options = {}) {
    const queryRequest = {
      // Không set asOfDate hoặc fromDate/toDate để lấy tất cả history
      filter: options.filter || null,
      orderBy: options.orderBy || 'ValidFrom DESC',
      page: options.page || 1,
      pageSize: options.pageSize || 100
    };

    return await this.queryTemporalData(queryRequest);
  }

  // 📋 Utility methods cho frontend components
  
  // Format temporal query request từ UI filters
  formatQueryRequest(uiFilters) {
    const request = {};
    
    if (uiFilters.asOfDate) {
      request.asOfDate = uiFilters.asOfDate;
    } else if (uiFilters.fromDate || uiFilters.toDate) {
      if (uiFilters.fromDate) request.fromDate = uiFilters.fromDate;
      if (uiFilters.toDate) request.toDate = uiFilters.toDate;
    }

    if (uiFilters.branchCode) {
      request.filter = `BranchCode = '${uiFilters.branchCode}'`;
    }

    if (uiFilters.employeeCode) {
      const filterCondition = `EmployeeCode = '${uiFilters.employeeCode}'`;
      request.filter = request.filter ? 
        `${request.filter} AND ${filterCondition}` : 
        filterCondition;
    }

    if (uiFilters.kpiCode) {
      const filterCondition = `KpiCode = '${uiFilters.kpiCode}'`;
      request.filter = request.filter ? 
        `${request.filter} AND ${filterCondition}` : 
        filterCondition;
    }

    request.page = uiFilters.page || 1;
    request.pageSize = uiFilters.pageSize || 50;
    request.orderBy = uiFilters.sortBy || 'ImportDate DESC';

    return request;
  }

  // Validate temporal query request
  validateQueryRequest(request) {
    const errors = [];

    if (request.asOfDate && (request.fromDate || request.toDate)) {
      errors.push('Không thể sử dụng cả asOfDate và fromDate/toDate cùng lúc');
    }

    if (request.page < 1) {
      errors.push('Page phải lớn hơn 0');
    }

    if (request.pageSize < 1 || request.pageSize > 1000) {
      errors.push('PageSize phải trong khoảng 1-1000');
    }

    return {
      isValid: errors.length === 0,
      errors
    };
  }

  // Format temporal statistics cho UI display
  formatStatisticsForUI(stats) {
    return {
      tableName: stats.tableName,
      currentRecords: stats.currentRecords?.toLocaleString() || '0',
      historyRecords: stats.historyRecords?.toLocaleString() || '0',
      totalRecords: (stats.currentRecords + stats.historyRecords)?.toLocaleString() || '0',
      earliestChange: stats.earliestChange ? 
        new Date(stats.earliestChange).toLocaleDateString('vi-VN') : 
        'N/A',
      latestChange: stats.latestChange ? 
        new Date(stats.latestChange).toLocaleDateString('vi-VN') : 
        'N/A',
      uniqueEntities: stats.uniqueEntities?.toLocaleString() || '0',
      generatedAt: new Date(stats.generatedAt).toLocaleString('vi-VN')
    };
  }
}

export default new TemporalService();
