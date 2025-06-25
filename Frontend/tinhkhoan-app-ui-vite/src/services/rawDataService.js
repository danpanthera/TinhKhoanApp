// 🗄️ rawDataService.js - Dịch vụ xử lý Kho Dữ liệu Thô
import api from './api';

class RawDataService {
  constructor() {
    // ✅ FIX: Sửa baseURL thành /RawData không cần /api vì api.js đã có
    this.baseURL = '/RawData';  // Controller route là api/RawData
  }

  // 📋 Lấy danh sách tất cả dữ liệu thô đã import
  async getAllImports() {
    try {
      // ✅ FIX: Gọi đúng endpoint RawData
      console.log('📊 Calling API endpoint:', this.baseURL);
      const response = await api.get(this.baseURL);

      // Debug response data structure
      console.log('📊 Raw API response:', typeof response.data, response.data ? Object.keys(response.data) : 'No data');

      // 🔧 Parse .NET $values format và map fields đúng
      let data = response.data;
      if (data && data.$values) {
        console.log('🔧 Found $values array in response');
        data = data.$values;
      }

      // Handle empty or null data gracefully
      if (!data || !Array.isArray(data)) {
        console.warn('⚠️ API returned empty or invalid data:', data);
        data = [];
      }

      // 🔧 ĐỒNG BỘ FIELD MAPPING để fix vấn đề backend trả category, frontend dùng dataType
      const mappedData = data.map(item => ({
        ...item,
        // ✅ FIX TRIỆT ĐỂ: Backend trả về category="LN01", ưu tiên category trước
        dataType: item.category || item.dataType || item.fileType || 'UNKNOWN',
        // 🔧 Preserve original fields để debug
        originalFileType: item.fileType,
        originalDataType: item.dataType,
        originalCategory: item.category,
        // Format date đúng
        importDate: item.importDate ? new Date(item.importDate) : new Date(),
        // Đảm bảo recordsCount luôn là số nguyên
        recordsCount: parseInt(item.recordsCount || 0),
        // Normalize fileName
        fileName: item.fileName || item.name || 'Unknown File'
      }));

      console.log('✅ Mapped getAllImports data:', mappedData.length, 'items');
      if (mappedData.length > 0) {
        console.log('� First import data sample:', {
          id: mappedData[0].id,
          fileName: mappedData[0].fileName,
          fileType: mappedData[0].originalFileType,
          dataType: mappedData[0].dataType,
          category: mappedData[0].originalCategory,
          recordsCount: mappedData[0].recordsCount
        });
      } else {
        console.log('ℹ️ No import data returned from API');
      }

      return {
        success: true,
        data: mappedData
      };
    } catch (error) {
      console.error('❌ Lỗi lấy danh sách import:', error);

      // Xử lý các loại lỗi cụ thể
      let errorMessage = 'Lỗi kết nối server';
      if (error.code === 'ERR_NETWORK' || error.code === 'ERR_CONNECTION_REFUSED') {
        errorMessage = 'Không thể kết nối đến server backend. Vui lòng kiểm tra:\n• Server backend có đang chạy?\n• Cổng kết nối có đúng không?\n• Firewall có chặn kết nối không?';
      } else if (error.response?.status === 404) {
        errorMessage = 'API endpoint không tồn tại';
      } else if (error.response?.status >= 500) {
        errorMessage = `Lỗi server nội bộ: ${error.response?.data?.message || error.message}`;
      } else if (error.response?.data?.message) {
        errorMessage = error.response.data.message;
      }

      return {
        success: false,
        error: errorMessage,
        errorCode: error.code,
        errorStatus: error.response?.status,
        // Removed mock fallback data - throw error instead
        data: []
      };
    }
  }

  // Removed mock data method - only use real API data

  // 📤 Import dữ liệu theo loại với progress tracking và audio notification
  async importData(dataType, files, options = {}) {
    try {
      console.log(`📤 Starting import for dataType: ${dataType}, files:`,
        files.map(f => ({ name: f.name, size: f.size, type: f.type })));

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

      // Thêm statement date nếu có
      if (options.statementDate) {
        formData.append('StatementDate', options.statementDate);
      }

      // Tính tổng file size để track progress
      const totalSize = files.reduce((sum, file) => sum + file.size, 0);
      const startTime = Date.now();
      let lastLoadedAmount = 0;
      let lastTime = startTime;

      // Store this context for callback
      const self = this;

      // ✅ FIX: Gọi đúng endpoint import/{dataType} với Category parameter
      const endpoint = `/import/${dataType}`;
      console.log(`📤 Calling API endpoint: ${this.baseURL}${endpoint}`);
      console.log('📤 Form data contents:', {
        files: Array.from(formData.getAll('Files')).map(f => f.name),
        options: {
          archivePassword: formData.get('ArchivePassword') ? '***' : undefined,
          notes: formData.get('Notes'),
          statementDate: formData.get('StatementDate')
        }
      });

      const response = await api.post(`${this.baseURL}${endpoint}`, formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        },
        timeout: 600000, // 10 phút timeout cho upload file lớn
        maxContentLength: 500 * 1024 * 1024, // 500MB max
        maxBodyLength: 500 * 1024 * 1024,
        onUploadProgress: (progressEvent) => {
          const percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total);
          const currentTime = Date.now();
          const elapsed = currentTime - startTime;

          // Tính tốc độ upload hiện tại (smoothing với exponential moving average)
          const timeDelta = currentTime - lastTime;
          const loadedDelta = progressEvent.loaded - lastLoadedAmount;

          if (timeDelta > 0) {
            const currentSpeed = loadedDelta / timeDelta * 1000; // bytes per second
            const remainingBytes = progressEvent.total - progressEvent.loaded;

            // Ước tính thời gian còn lại dựa trên tốc độ hiện tại
            let remainingTime = remainingBytes > 0 && currentSpeed > 0 ? (remainingBytes / currentSpeed * 1000) : 0;

            // Nếu gần xong (> 95%) thì ước tính thời gian còn lại ngắn hơn
            if (percentCompleted > 95) {
              remainingTime = Math.min(remainingTime, 5000); // tối đa 5 giây
            }

            console.log(`📊 Tiến độ upload ${dataType}: ${percentCompleted}%`);

            // Gọi callback progress nếu có
            if (options.onProgress) {
              console.log(`📊 Progress update: ${percentCompleted}%, Speed: ${self.formatFileSize(currentSpeed)}/s, Remaining: ${self.formatTime(remainingTime)}`);
              options.onProgress({
                loaded: progressEvent.loaded,
                total: progressEvent.total,
                percentage: percentCompleted,
                remainingTime: Math.max(0, remainingTime), // milliseconds
                remainingTimeFormatted: self.formatTime(remainingTime), // mm:ss format
                uploadSpeed: currentSpeed, // bytes per second
                formattedSpeed: self.formatFileSize(currentSpeed) + '/s',
                formattedRemaining: self.formatTime(remainingTime),
                formattedLoaded: self.formatFileSize(progressEvent.loaded),
                formattedTotal: self.formatFileSize(progressEvent.total),
                isNearCompletion: percentCompleted > 95
              });
            }

            lastLoadedAmount = progressEvent.loaded;
            lastTime = currentTime;
          }
        }
      });

      // 🔊 Phát âm thanh thông báo hoàn thành với notification
      this.playCompletionSound();

      // Hiển thị browser notification nếu được phép
      if ('Notification' in window && Notification.permission === 'granted') {
        new Notification('🎉 Đã upload xong!', {
          body: `Import dữ liệu ${dataType} đã hoàn tất thành công`,
          icon: '/favicon.ico',
          tag: 'upload-complete'
        });
      }

      // Log response để debug
      console.log(`� Import response:`, response.data);

      // �🔧 Parse .NET $values format trong response
      let data = response.data;
      if (data && data.$values) {
        data = data.$values;
      }

      return {
        success: true,
        data: data,
        originalResponse: response.data // Lưu response gốc để debug
      };
    } catch (error) {
      console.error(`❌ Lỗi import dữ liệu ${dataType}:`, error);
      console.error('Error details:', {
        message: error.message,
        response: error.response,
        data: error.response?.data,
        status: error.response?.status
      });

      return {
        success: false,
        error: error.response?.data?.message || error.message || 'Lỗi kết nối server',
        errorDetails: {
          status: error.response?.status,
          data: error.response?.data,
          code: error.code
        }
      };
    }
  }

  // 👁️ Xem trước dữ liệu đã import
  async previewData(importId) {
    try {
      console.log(`🔍 Getting preview data for import ID: ${importId}`);
      const response = await api.get(`${this.baseURL}/${importId}/preview`);

      // 🔧 Parse .NET $values format
      let data = response.data;
      if (data && data.$values) {
        data = data.$values;
      }

      console.log('📊 Backend response:', data);

      // 🔧 Check for previewData field specifically
      if (data && data.previewData) {
        console.log('📋 Found previewData field in response, using it directly');
        return {
          success: true,
          data: {
            ...data,
            previewRows: data.previewData
          }
        };
      }

      // ✅ FIX: Sử dụng PreviewData từ backend thay vì tạo mock
      if (data && data.PreviewData && Array.isArray(data.PreviewData)) {
        console.log(`� Found ${data.PreviewData.length} real records from backend`);
        return {
          success: true,
          data: {
            ...data,
            previewRows: data.PreviewData,
            importInfo: {
              DataType: data.Category || 'UNKNOWN',
              RecordsCount: data.PreviewData.length,
              FileName: data.FileName
            }
          }
        };
      }

      // Nếu không có dữ liệu thực, trả về lỗi thay vì mock
      console.warn('⚠️ No real preview data found in backend response');
      return {
        success: false,
        error: 'Không tìm thấy dữ liệu preview từ backend. Có thể file chưa được import đúng cách.'
      };

    } catch (error) {
      console.error('❌ Lỗi xem trước dữ liệu:', error);
      return {
        success: false,
        error: error.response?.data?.message || error.message || 'Lỗi kết nối server'
      };
    }
  }

  // Removed mock record creation - only use real API data

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

  // 🗑️ Xóa một bản ghi theo ID
  async deleteImportRecord(id) {
    try {
      const response = await api.delete(`/DataImport/${id}`);

      if (response.status === 200 || response.status === 204) {
        return {
          success: true,
          data: response.data,
          message: response.data?.message || 'Đã xóa bản ghi thành công'
        };
      } else {
        return {
          success: false,
          error: response.data?.message || 'Lỗi không xác định'
        };
      }
    } catch (error) {
      console.error('❌ Lỗi xóa bản ghi:', error);

      return {
        success: false,
        error: error.response?.data?.message || error.message || 'Lỗi khi xóa bản ghi',
        errorCode: error.code,
        errorStatus: error.response?.status
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
      console.log('🗑️ Bắt đầu xóa TOÀN BỘ dữ liệu...');

      const response = await api.delete(`${this.baseURL}/clear-all`);

      console.log('✅ Kết quả xóa dữ liệu:', response.data);

      return {
        success: true,
        data: response.data,
        message: response.data.message || 'Đã xóa thành công toàn bộ dữ liệu',
        recordsCleared: response.data.recordsCleared || 0,
        itemsCleared: response.data.itemsCleared || 0,
        dynamicTablesCleared: response.data.dynamicTablesCleared || 0
      };
    } catch (error) {
      console.error('❌ Lỗi khi xóa toàn bộ dữ liệu:', error);

      let errorMessage = 'Lỗi khi xóa dữ liệu';
      if (error.response?.data?.message) {
        errorMessage = error.response.data.message;
      } else if (error.code === 'ERR_NETWORK') {
        errorMessage = 'Không thể kết nối đến server để xóa dữ liệu';
      }

      return {
        success: false,
        message: errorMessage,
        error: error.response?.data || error.message
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

  // 📋 Lấy danh sách tất cả dữ liệu thô đã import
  async getAllData() {
    try {
      // Gọi API endpoint tổng quan (DataImport), nơi lưu trữ tất cả bản ghi import
      console.log('📊 Calling getAllData API endpoint: /DataImport');
      const response = await api.get('/DataImport');

      console.log('📊 getAllData API response:', {
        status: response.status,
        dataType: typeof response.data,
        isArray: Array.isArray(response.data),
        length: Array.isArray(response.data) ? response.data.length : 'N/A',
        sample: Array.isArray(response.data) && response.data.length > 0 ? response.data[0] : 'No data'
      });

      // Normalize data
      let data = response.data || [];

      // Nếu không phải array, thử kiểm tra $values (format .NET)
      if (!Array.isArray(data) && data.$values) {
        console.log('🔧 Found $values array in getAllData response');
        data = data.$values;
      }

      // Đảm bảo data luôn là array
      if (!Array.isArray(data)) {
        console.warn('⚠️ API returned non-array data for getAllData:', data);
        data = [];
      }

      // Map dữ liệu để chuẩn hóa các trường
      const mappedData = data.map(item => ({
        ...item,
        // ✅ Normalize dataType/category/fileType để trùng khớp khi lọc
        dataType: item.dataType || item.category || item.fileType || 'UNKNOWN',
        category: item.category || item.dataType || '',
        fileType: item.fileType || item.dataType || '',
        // Đảm bảo các trường khác đúng định dạng
        recordsCount: parseInt(item.recordsCount || 0),
        fileName: item.fileName || item.name || 'Unknown File'
      }));

      console.log('✅ Mapped getAllData:', {
        resultCount: mappedData.length,
        sample: mappedData.length > 0 ? mappedData[0] : 'No data'
      });

      return {
        success: true,
        data: mappedData
      };
    } catch (error) {
      console.error('❌ Lỗi lấy tất cả dữ liệu:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 📋 Lấy dữ liệu theo ngày sao kê
  async getByStatementDate(dataType, statementDate) {
    try {
      // Nếu không có ngày, lấy tất cả dữ liệu
      if (!statementDate) {
        console.log('📊 No statement date provided, getting all data for type:', dataType);
        return await this.getAllData();
      }

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

  // ✅ Lấy danh sách import gần đây nhất
  async getRecentImports(limit = 20) {
    try {
      console.log('📊 Getting recent imports with limit:', limit);
      const response = await api.get(`${this.baseURL}/recent?limit=${limit}`);

      let data = response.data;
      if (data && data.$values) {
        data = data.$values;
      }

      if (!Array.isArray(data)) {
        data = [];
      }

      const mappedData = data.map(item => ({
        ...item,
        dataType: item.category || item.dataType || item.fileType || 'UNKNOWN',
        category: item.category || item.dataType || '',
        fileType: item.fileType || item.dataType || '',
        recordsCount: parseInt(item.recordsCount || 0),
        fileName: item.fileName || 'Unknown File'
      }));

      console.log('✅ Recent imports loaded:', mappedData.length, 'items');

      return {
        success: true,
        data: mappedData
      };
    } catch (error) {
      console.error('❌ Error getting recent imports:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
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
        acceptedFormats: ['.csv', '.xlsx', '.xls', '.zip', '.rar', '.7z'],
        requiredKeyword: 'LN01'
      },
      'LN02': {
        name: 'LN02',
        description: 'Sao kê biến động nhóm nợ',
        icon: '🔄',
        acceptedFormats: ['.csv', '.xlsx', '.xls', '.zip', '.rar', '.7z'],
        requiredKeyword: 'LN02'
      },
      'LN03': {
        name: 'LN03',
        description: 'Dữ liệu Nợ XLRR',
        icon: '📊',
        acceptedFormats: ['.csv', '.xlsx', '.xls', '.zip', '.rar', '.7z'],
        requiredKeyword: 'LN03'
      },
      'DP01': {
        name: 'DP01',
        description: 'Dữ liệu Tiền gửi',
        icon: '🏦',
        acceptedFormats: ['.csv', '.xlsx', '.xls', '.zip', '.rar', '.7z'],
        requiredKeyword: 'DP01'
      },
      'EI01': {
        name: 'EI01',
        description: 'Dữ liệu mobile banking',
        icon: '📱',
        acceptedFormats: ['.csv', '.xlsx', '.xls', '.zip', '.rar', '.7z'],
        requiredKeyword: 'EI01'
      },
      'GAHR26': {
        name: 'GAHR26',
        description: 'Báo cáo nhân sự GAHR26',
        icon: '👥',
        acceptedFormats: ['.csv', '.xlsx', '.xls', '.zip', '.rar', '.7z'],
        requiredKeyword: 'GAHR26'
      },
      'GL01': {
        name: 'GL01',
        description: 'Dữ liệu bút toán GDV',
        icon: '✍️',
        acceptedFormats: ['.csv', '.xlsx', '.xls', '.zip', '.rar', '.7z'],
        requiredKeyword: 'GL01'
      },
      'DPDA': {
        name: 'DPDA',
        description: 'Dữ liệu sao kê phát hành thẻ',
        icon: '💳',
        acceptedFormats: ['.csv', '.xlsx', '.xls', '.zip', '.rar', '.7z'],
        requiredKeyword: 'DPDA'
      },
      'DB01': {
        name: 'DB01',
        description: 'Sao kê TSDB và Không TSDB',
        icon: '📋',
        acceptedFormats: ['.csv', '.xlsx', '.xls', '.zip', '.rar', '.7z'],
        requiredKeyword: 'DB01'
      },
      'KH03': {
        name: 'KH03',
        description: 'Sao kê Khách hàng pháp nhân',
        icon: '🏢',
        acceptedFormats: ['.csv', '.xlsx', '.xls', '.zip', '.rar', '.7z'],
        requiredKeyword: 'KH03'
      },
      'BC57': {
        name: 'BC57',
        description: 'Sao kê Lãi dự thu',
        icon: '📈',
        acceptedFormats: ['.csv', '.xlsx', '.xls', '.zip', '.rar', '.7z'],
        requiredKeyword: 'BC57'
      },
      'RR01': {
        name: 'RR01',
        description: 'Sao kê dư nợ gốc, lãi XLRR',
        icon: '📉',
        acceptedFormats: ['.csv', '.xlsx', '.xls', '.zip', '.rar', '.7z'],
        requiredKeyword: 'RR01'
      },
      '7800_DT_KHKD1': {
        name: '7800_DT_KHKD1',
        description: 'Báo cáo KHKD (DT)',
        icon: '📑',
        acceptedFormats: ['.csv', '.xlsx', '.xls', '.zip', '.rar', '.7z'],
        requiredKeyword: '7800_DT_KHKD1'
      },
      'GLCB41': {
        name: 'GLCB41',
        description: 'Bảng cân đối',
        icon: '⚖️',
        acceptedFormats: ['.csv', '.xlsx', '.xls', '.zip', '.rar', '.7z'],
        requiredKeyword: 'GLCB41'
      },
      'API_IMPORT': {
        name: 'API_IMPORT',
        description: 'Import qua API/Temporal',
        icon: '🔗',
        acceptedFormats: ['.json', '.csv', '.xlsx', '.xls', '.zip', '.rar', '.7z'],
        requiredKeyword: 'API_IMPORT'
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

    // Check if filename contains data type
    if (!fileName.includes(dataType.toLowerCase())) {
      return {
        valid: false,
        error: `Tên file phải chứa mã loại dữ liệu '${dataType}'`
      }
    }

    return { valid: true }
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

  // 🎨 Lấy màu sắc cho loại dữ liệu - ĐỒNG BỘ TẤT CẢ 13 LOẠI
  // 🎨 Lấy màu sắc cho từng loại dữ liệu theo nhóm chữ cái đầu
  getDataTypeColor(dataType) {
    if (!dataType) return '#6B7280'; // gray default

    const firstChar = dataType.charAt(0).toUpperCase();

    // Phân nhóm màu theo chữ cái đầu
    if (firstChar === 'D') {
      // Loại dữ liệu bắt đầu bằng "D" - màu xanh lá
      return '#10B981'; // green
    } else if (firstChar === 'L' || firstChar === 'R') {
      // Loại dữ liệu bắt đầu bằng "L" hoặc "R" - màu cam
      return '#F97316'; // orange
    } else if (firstChar === 'G') {
      // Loại dữ liệu bắt đầu bằng "G" - màu tím
      return '#8B5CF6'; // purple
    } else {
      // Các loại khác - màu mặc định theo định nghĩa riêng
      const colors = {
        '7800_DT_KHKD1': '#7C2D12', // brown - Báo cáo KHKD (DT)
        'API_IMPORT': '#1E40AF', // blue-800 - Import qua API/Temporal
        'BC57': '#EC4899', // pink - Sao kê Lãi dự thu
        'EI01': '#06B6D4', // cyan - Dữ liệu mobile banking
        'KH03': '#84CC16' // lime - Sao kê Khách hàng pháp nhân
      };
      return colors[dataType] || '#6B7280'; // gray default
    }
  }

  // 📊 Format số lượng records with thousand separators (#,###)
  formatRecordCount(count) {
    if (!count && count !== 0) return '0';

    // Convert to number if it's a string
    const num = typeof count === 'string' ? parseInt(count) : count;

    // Add thousand separators using Vietnamese locale for #,### format
    return new Intl.NumberFormat('en-US').format(num);
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

  // 🔊 Phát âm thanh thông báo hoàn thành
  playCompletionSound() {
    try {
      // Tạo AudioContext để phát âm thanh
      const audioContext = new (window.AudioContext || window.webkitAudioContext)();

      // Tạo âm thanh thông báo dạng melody (3 nốt nhạc)
      const notes = [
        { freq: 523.25, duration: 0.2 }, // C5
        { freq: 659.25, duration: 0.2 }, // E5
        { freq: 783.99, duration: 0.4 }  // G5
      ];

      let startTime = audioContext.currentTime;

      notes.forEach((note, index) => {
        const oscillator = audioContext.createOscillator();
        const gainNode = audioContext.createGain();

        // Kết nối audio nodes
        oscillator.connect(gainNode);
        gainNode.connect(audioContext.destination);

        // Thiết lập tần số và âm lượng
        oscillator.frequency.setValueAtTime(note.freq, startTime);
        oscillator.type = 'sine'; // Âm thanh mềm mại

        // Envelope cho âm thanh mượt mà - TĂNG VOLUME LÊN GẤP ĐÔI
        gainNode.gain.setValueAtTime(0, startTime);
        gainNode.gain.linearRampToValueAtTime(0.6, startTime + 0.05); // 0.3 -> 0.6 (tăng gấp đôi)
        gainNode.gain.exponentialRampToValueAtTime(0.02, startTime + note.duration);

        // Phát âm thanh
        oscillator.start(startTime);
        oscillator.stop(startTime + note.duration);

        startTime += note.duration + 0.1; // Gap giữa các nốt
      });

      console.log('🔊 Đã phát âm thanh thông báo "Đã upload xong"');
    } catch (error) {
      console.warn('⚠️ Không thể phát âm thanh:', error);
      // Fallback: sử dụng notification API nếu có
      if ('Notification' in window && Notification.permission === 'granted') {
        new Notification('🎉 Đã upload xong!', {
          body: 'Import dữ liệu đã hoàn tất',
          icon: '/favicon.ico',
          tag: 'upload-complete'
        });
      }
    }
  }

  // ⏰ Format thời gian từ milliseconds sang mm:ss
  formatTime(milliseconds) {
    if (!milliseconds || milliseconds <= 0) return '00:00';

    const totalSeconds = Math.floor(milliseconds / 1000);
    const minutes = Math.floor(totalSeconds / 60);
    const seconds = totalSeconds % 60;

    return `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
  }

  // 📁 Format file size to human readable format
  formatFileSize(bytes) {
    if (!bytes || bytes === 0) return '0 Bytes';

    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));

    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }

  // 📁 Format file size (bytes to human readable) - Legacy method
  formatBytes(bytes) {
    return this.formatFileSize(bytes);
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

  // ⚡ Temporal Table APIs (Migrated from SCD Type 2)

  // 📋 Lấy danh sách imports với temporal data
  async getOptimizedImports(page = 1, pageSize = 50, searchTerm = '', sortBy = 'ImportDate', sortOrder = 'desc') {
    try {
      const params = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString(),
        searchTerm: searchTerm || '',
        sortBy: sortBy || 'ImportDate',
        sortOrder: sortOrder || 'desc'
      });

      const response = await api.get(`${this.baseURL}/temporal/query/RawDataImport?${params.toString()}`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi lấy danh sách imports từ temporal table:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 📊 Lấy records với temporal queries
  async getOptimizedRecords(importId, offset = 0, limit = 100, searchTerm = '') {
    try {
      const params = new URLSearchParams({
        page: Math.floor(offset / limit) + 1,
        pageSize: limit.toString(),
        searchTerm: searchTerm || '',
        importId: importId.toString()
      });

      const response = await api.get(`${this.baseURL}/temporal/query/RawData?${params.toString()}`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi lấy records từ temporal table:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 🔄 Thêm các phương thức mới cho Temporal Tables (Migrated from SCD Type 2)

  // 📈 Lấy dashboard statistics từ temporal data
  async getDashboardStats() {
    try {
      const response = await api.get(`${this.baseURL}/temporal/stats/RawData`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi lấy dashboard stats từ temporal table:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 🎯 Tìm kiếm nâng cao với temporal data
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
      if (dateFrom) params.append('startDate', dateFrom);
      if (dateTo) params.append('endDate', dateTo);

      const response = await api.get(`${this.baseURL}/temporal/query/RawData?${params.toString()}`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi tìm kiếm nâng cao trong temporal table:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 📊 Lấy thống kê hiệu suất từ temporal table
  async getPerformanceStats(timeRange = '24h') {
    try {
      const response = await api.get(`${this.baseURL}/temporal/stats/RawData?timeRange=${timeRange}`);
      return {
        success: true,
        data: response.data
      };
    } catch (error) {
      console.error('❌ Lỗi lấy thống kê hiệu suất từ temporal table:', error);
      return {
        success: false,
        error: error.response?.data?.message || 'Lỗi kết nối server'
      };
    }
  }

  // 🔄 Refresh temporal table statistics and indexes
  async refreshCache(cacheKey) {
    try {
      // For temporal tables, we refresh statistics instead of cache
      const response = await api.post(`${this.baseURL}/temporal/index/RawData`);
      return {
        success: true,
        data: {
          message: 'Temporal table statistics and indexes refreshed successfully',
          timestamp: new Date().toISOString(),
          cacheKey: cacheKey
        }
      };
    } catch (error) {
      console.error('❌ Lỗi refresh temporal table statistics:', error);
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
