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
      
      // 🔧 Parse .NET $values format
      let data = response.data;
      if (data && data.$values) {
        data = data.$values;
      }
      
      return {
        success: true,
        data: data || []
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
        errorMessage = 'Lỗi server nội bộ';
      } else if (error.response?.data?.message) {
        errorMessage = error.response.data.message;
      }
      
      return {
        success: false,
        error: errorMessage,
        errorCode: error.code,
        errorStatus: error.response?.status,
        // Fallback data để demo vẫn hoạt động
        fallbackData: this.getMockData()
      };
    }
  }

  // 🔄 Mock data cho demo khi server không có
  getMockData() {
    return [
      {
        id: 'demo-1',
        dataType: 'excel',
        fileName: 'demo-data-lai-chau.xlsx',
        uploadDate: '2025-06-21T10:30:00Z',
        status: 'Completed',
        recordCount: 1250,
        fileSize: 2048576
      },
      {
        id: 'demo-2', 
        dataType: 'csv',
        fileName: 'agribank-branches.csv',
        uploadDate: '2025-06-20T15:45:00Z',
        status: 'Completed',
        recordCount: 23,
        fileSize: 524288
      },
      {
        id: 'demo-3',
        dataType: 'archive',
        fileName: 'kpi-data-2025.zip',
        uploadDate: '2025-06-19T09:15:00Z', 
        status: 'Processing',
        recordCount: 0,
        fileSize: 10485760
      }
    ];
  }

  // 📤 Import dữ liệu theo loại với progress tracking và audio notification
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

      // Tính tổng file size để track progress
      const totalSize = files.reduce((sum, file) => sum + file.size, 0);
      const startTime = Date.now();
      let lastLoadedAmount = 0;
      let lastTime = startTime;

      // Store this context for callback
      const self = this;

      const response = await api.post(`${this.baseURL}/import/${dataType}`, formData, {
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

      // 🔧 Parse .NET $values format trong response
      let data = response.data;
      if (data && data.$values) {
        data = data.$values;
      }

      return {
        success: true,
        data: data
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
      
      // 🔧 Parse .NET $values format
      let data = response.data;
      if (data && data.$values) {
        data = data.$values;
      }
      
      return {
        success: true,
        data: data
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

  // 🎨 Lấy màu sắc cho loại dữ liệu - ĐỒNG BỘ TẤT CẢ 13 LOẠI
  getDataTypeColor(dataType) {
    const colors = {
      'LN01': '#10B981', // green - Dữ liệu LOAN
      'LN02': '#059669', // emerald - Sao kê biến động nhóm nợ
      'LN03': '#F59E0B', // amber - Dữ liệu Nợ XLRR
      'DP01': '#3B82F6', // blue - Dữ liệu Tiền gửi
      'EI01': '#8B5CF6', // purple - Dữ liệu mobile banking
      'GL01': '#EF4444', // red - Dữ liệu bút toán GDV
      'DPDA': '#06B6D4', // cyan - Dữ liệu sao kê phát hành thẻ
      'DB01': '#84CC16', // lime - Sao kê TSDB và Không TSDB
      'KH03': '#F97316', // orange - Sao kê Khách hàng pháp nhân
      'BC57': '#EC4899', // pink - Sao kê Lãi dự thu
      'RR01': '#DC2626', // red-600 - Sao kê dư nợ gốc, lãi XLRR
      '7800_DT_KHKD1': '#7C2D12', // brown - Báo cáo KHKD (DT)
      'GLCB41': '#1E40AF' // blue-800 - Bảng cân đối
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
        
        // Envelope cho âm thanh mượt mà
        gainNode.gain.setValueAtTime(0, startTime);
        gainNode.gain.linearRampToValueAtTime(0.3, startTime + 0.05);
        gainNode.gain.exponentialRampToValueAtTime(0.01, startTime + note.duration);
        
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
