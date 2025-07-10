// 🗄️ rawDataService.js - Dịch vụ xử lý Kho Dữ liệu Thô với DIRECT IMPORT
import api from './api';

class RawDataService {
  constructor() {
    // ✅ Sử dụng endpoint DataImport/records để lấy import history
    this.baseURL = '/DataImport/records';
  }

  // 📋 Lấy danh sách tất cả dữ liệu thô đã import
  async getAllImports() {
    try {
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
      return mappedData;
    } catch (error) {
      console.error('❌ Error in getAllImports:', error);
      if (error.response?.status === 404) {
        console.warn('⚠️ API endpoint not found, returning empty array');
        return [];
      }
      throw new Error(`Failed to get imports: ${error.response?.data?.message || error.message}`);
    }
  }

  // 📤 Import dữ liệu sử dụng DIRECT IMPORT với auto-detection từ filename
  async importData(dataType, files, options = {}) {
    try {
      console.log(`📤 Starting DirectImport for ${files.length} files (ignoring legacy dataType: ${dataType})`);

      const results = {
        success: true,
        totalFiles: files.length,
        successCount: 0,
        failureCount: 0,
        results: [],
        message: ''
      };

      // Process each file individually using DirectImport/smart
      for (let i = 0; i < files.length; i++) {
        const file = files[i];
        console.log(`📤 Processing file ${i + 1}/${files.length}: ${file.name}`);

        try {
          const formData = new FormData();
          formData.append('file', file); // DirectImport expects single 'file' parameter

          // Add options to DirectImport format
          if (options.notes) {
            formData.append('notes', options.notes);
          }
          if (options.statementDate) {
            formData.append('statementDate', options.statementDate);
          }

          // Update progress for current file
          if (options.onProgress) {
            const fileProgress = Math.round(((i) / files.length) * 100);
            options.onProgress({
              percentage: fileProgress,
              currentFile: file.name,
              processedFiles: i,
              totalFiles: files.length
            });
          }

          const response = await api.post('/DirectImport/smart', formData, {
            headers: {
              'Content-Type': 'multipart/form-data'
            },
            timeout: 300000, // 5 phút timeout cho mỗi file
            maxContentLength: 100 * 1024 * 1024, // 100MB max per file
            maxBodyLength: 100 * 1024 * 1024,
            onUploadProgress: (progressEvent) => {
              if (options.onProgress) {
                const fileProgress = Math.round((progressEvent.loaded * 100) / progressEvent.total);
                const overallProgress = Math.round(((i + fileProgress/100) / files.length) * 100);

                options.onProgress({
                  percentage: overallProgress,
                  currentFile: file.name,
                  fileProgress: fileProgress,
                  processedFiles: i,
                  totalFiles: files.length
                });
              }
            }
          });

          // Handle successful response
          if (response.data && response.data.success) {
            results.successCount++;
            results.results.push({
              file: file.name,
              success: true,
              dataType: response.data.dataType,
              recordsCount: response.data.processedRecords,
              message: `Import thành công: ${response.data.processedRecords} records`
            });
            console.log(`✅ File ${file.name} imported successfully: ${response.data.processedRecords} records`);
          } else {
            throw new Error(response.data?.errorMessage || 'Unknown import error');
          }

        } catch (error) {
          console.error(`❌ Error importing file ${file.name}:`, error);
          results.failureCount++;
          results.results.push({
            file: file.name,
            success: false,
            error: error.response?.data?.message || error.message || 'Import failed'
          });
        }
      }

      // Final progress update
      if (options.onProgress) {
        options.onProgress({
          percentage: 100,
          currentFile: 'Completed',
          processedFiles: files.length,
          totalFiles: files.length
        });
      }

      // Set overall success status
      results.success = results.successCount > 0;
      results.message = `Import completed: ${results.successCount}/${results.totalFiles} files successful`;

      console.log(`📊 Import summary:`, results);
      return results;

    } catch (error) {
      console.error('🔥 Import error:', error);
      throw new Error(`Import failed: ${error.response?.data?.message || error.message}`);
    }
  }

  // 📊 Lấy thống kê import theo data type
  async getImportStatistics(dataType = null) {
    try {
      const imports = await this.getAllImports();

      // Filter by dataType if specified
      const filteredImports = dataType
        ? imports.filter(imp => imp.dataType === dataType)
        : imports;

      // Calculate statistics
      const stats = {
        totalImports: filteredImports.length,
        totalRecords: filteredImports.reduce((sum, imp) => sum + (imp.recordsCount || 0), 0),
        successfulImports: filteredImports.filter(imp => imp.status === 'Completed').length,
        failedImports: filteredImports.filter(imp => imp.status === 'Failed').length,
        lastImportDate: filteredImports.length > 0 ?
          Math.max(...filteredImports.map(imp => new Date(imp.importDate).getTime())) : null
      };

      return stats;
    } catch (error) {
      console.error('❌ Error getting import statistics:', error);
      return {
        totalImports: 0,
        totalRecords: 0,
        successfulImports: 0,
        failedImports: 0,
        lastImportDate: null
      };
    }
  }

  // 📋 Lấy danh sách định nghĩa các loại dữ liệu (for DataImportViewFull.vue compatibility)
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
      'GL41': {
        name: 'GL41',
        description: 'Bảng cân đối',
        icon: '⚖️',
        acceptedFormats: ['.csv', '.xlsx', '.xls', '.zip', '.rar', '.7z'],
        requiredKeyword: 'GL41'
      }
    };
  }

  // 🔧 Utility functions
  formatFileSize(bytes) {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }

  formatDate(date) {
    if (!date) return '';
    return new Date(date).toLocaleDateString('vi-VN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  formatRecordCount(count) {
    return new Intl.NumberFormat('vi-VN').format(count || 0);
  }

  // 🔊 Audio notifications (optional)
  playCompletionSound() {
    try {
      // Simple audio notification using Web Audio API
      const audioContext = new (window.AudioContext || window.webkitAudioContext)();
      const oscillator = audioContext.createOscillator();
      const gainNode = audioContext.createGain();

      oscillator.connect(gainNode);
      gainNode.connect(audioContext.destination);

      oscillator.frequency.setValueAtTime(800, audioContext.currentTime);
      oscillator.frequency.setValueAtTime(600, audioContext.currentTime + 0.1);
      oscillator.frequency.setValueAtTime(800, audioContext.currentTime + 0.2);

      gainNode.gain.setValueAtTime(0.3, audioContext.currentTime);
      gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.3);

      oscillator.start(audioContext.currentTime);
      oscillator.stop(audioContext.currentTime + 0.3);
    } catch (error) {
      console.log('Audio notification not available');
    }
  }
}

// Create and export service instance
const rawDataService = new RawDataService();
export default rawDataService;
