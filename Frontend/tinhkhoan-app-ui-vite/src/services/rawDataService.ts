// Raw Data Service - API calls for import and statistics
import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

// Types
export interface ImportRequestDto {
  tableName: string;
  batchId?: string;
  importDate?: string;
  createdBy?: string;
  detectChangesOnly?: boolean;
  backupBeforeImport?: boolean;
}

export interface ImportStatisticsDto {
  tableName: string;
  totalImports: number;
  successfulImports: number;
  failedImports: number;
  processingImports: number;
  lastImportDate?: string;
  totalRecordsProcessed: number;
  totalNewRecords: number;
  totalUpdatedRecords: number;
  totalDeletedRecords: number;
  avgDurationSeconds: number;
  successRate: number;
}

export interface ImportResponseDto {
  success: boolean;
  message: string;
  batchId: string;
  statistics: ImportStatisticsDto;
  warnings: string[];
  errors: string[];
}

export interface ImportLog {
  logID: number;
  batchId: string;
  tableName: string;
  importDate: string;
  status: string;
  totalRecords: number;
  processedRecords: number;
  newRecords: number;
  updatedRecords: number;
  deletedRecords: number;
  duration?: number;
  errorMessage?: string;
  createdBy: string;
}

export interface ValidationResult {
  isValid: boolean;
  message: string;
  missingHeaders: string[];
  extraHeaders: string[];
  rowCount: number;
}

export interface HistoryRecord {
  historyID: number;
  sourceID: string;
  validFrom: string;
  validTo: string;
  isCurrent: boolean;
  versionNumber: number;
  createdDate: string;
  modifiedDate: string;
  [key: string]: any; // For dynamic data fields
}

// API Service Class
class RawDataService {
  private baseURL: string;

  constructor() {
    this.baseURL = `${API_BASE_URL}/RawDataImport`;
  }

  // Import LN01 data
  async importLN01Data(file: File, request: ImportRequestDto): Promise<ImportResponseDto> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('tableName', request.tableName);
    formData.append('batchId', request.batchId || '');
    formData.append('createdBy', request.createdBy || 'USER');
    
    if (request.importDate) {
      formData.append('importDate', request.importDate);
    }

    const response = await axios.post(`${this.baseURL}/ln01/upload`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });

    return response.data;
  }

  // Import GL01 data
  async importGL01Data(file: File, request: ImportRequestDto): Promise<ImportResponseDto> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('tableName', request.tableName);
    formData.append('batchId', request.batchId || '');
    formData.append('createdBy', request.createdBy || 'USER');
    
    if (request.importDate) {
      formData.append('importDate', request.importDate);
    }

    const response = await axios.post(`${this.baseURL}/gl01/upload`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });

    return response.data;
  }

  // Generic import function
  async importRawData(tableName: string, formData: FormData): Promise<ImportResponseDto> {
    const endpoint = tableName.toLowerCase() === 'ln01' ? 'ln01/upload' : 'gl01/upload';
    
    const response = await axios.post(`${this.baseURL}/${endpoint}`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });

    return response.data;
  }

  // Validate Excel file
  async validateExcelFile(file: File, tableType: string): Promise<ValidationResult> {
    const formData = new FormData();
    formData.append('file', file);

    const response = await axios.post(`${this.baseURL}/validate-file?tableType=${tableType}`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });

    return response.data;
  }

  // Get import statistics for a table
  async getImportStatistics(tableName: string): Promise<ImportStatisticsDto> {
    const response = await axios.get(`${this.baseURL}/statistics/${tableName}`);
    return response.data;
  }

  // Get recent imports
  async getRecentImports(limit: number = 20): Promise<ImportLog[]> {
    const response = await axios.get(`${this.baseURL}/recent?limit=${limit}`);
    return response.data;
  }

  // Get LN01 history for a specific record
  async getLN01History(sourceId: string): Promise<HistoryRecord[]> {
    const response = await axios.get(`${this.baseURL}/ln01/history/${sourceId}`);
    return response.data;
  }

  // Get GL01 history for a specific record
  async getGL01History(sourceId: string): Promise<HistoryRecord[]> {
    const response = await axios.get(`${this.baseURL}/gl01/history/${sourceId}`);
    return response.data;
  }

  // Get LN01 snapshot at specific date
  async getLN01Snapshot(snapshotDate: string): Promise<HistoryRecord[]> {
    const response = await axios.get(`${this.baseURL}/ln01/snapshot?snapshotDate=${snapshotDate}`);
    return response.data;
  }

  // Get GL01 snapshot at specific date
  async getGL01Snapshot(snapshotDate: string): Promise<HistoryRecord[]> {
    const response = await axios.get(`${this.baseURL}/gl01/snapshot?snapshotDate=${snapshotDate}`);
    return response.data;
  }

  // Cleanup old data
  async cleanupOldData(tableName: string, retainMonths: number = 12): Promise<boolean> {
    const response = await axios.post(`${this.baseURL}/cleanup/${tableName}?retainMonths=${retainMonths}`);
    return response.data;
  }

  // Get dashboard statistics for all tables
  async getDashboardStatistics(): Promise<{
    totalTables: number;
    todayImports: number;
    processingImports: number;
    successRate: number;
    tableStats: ImportStatisticsDto[];
  }> {
    try {
      const [ln01Stats, gl01Stats, dp01Stats, recentImports] = await Promise.all([
        this.getImportStatistics('LN01'),
        this.getImportStatistics('GL01'),  
        this.getImportStatistics('DP01'),
        this.getRecentImports(50)
      ]);

      const tableStats = [ln01Stats, gl01Stats, dp01Stats].filter(stats => stats);
      
      const today = new Date();
      today.setHours(0, 0, 0, 0);
      
      const todayImports = recentImports.filter(imp => 
        new Date(imp.importDate) >= today
      ).length;

      const processingImports = recentImports.filter(imp => 
        imp.status === 'PROCESSING'
      ).length;

      const totalSuccessful = tableStats.reduce((sum, s) => sum + s.successfulImports, 0);
      const totalImports = tableStats.reduce((sum, s) => sum + s.totalImports, 0);
      const successRate = totalImports > 0 ? (totalSuccessful / totalImports) * 100 : 0;

      return {
        totalTables: tableStats.length,
        todayImports,
        processingImports,
        successRate,
        tableStats
      };
    } catch (error) {
      console.error('Error getting dashboard statistics:', error);
      throw error;
    }
  }
}

// Create service instance
const rawDataService = new RawDataService();

// Export service methods
export const importRawData = (tableName: string, formData: FormData) => 
  rawDataService.importRawData(tableName, formData);

export const validateExcelFile = (file: File, tableType: string) => 
  rawDataService.validateExcelFile(file, tableType);

export const getRawDataStatistics = (tableName: string) => 
  rawDataService.getImportStatistics(tableName);

export const getRecentImports = (limit?: number) => 
  rawDataService.getRecentImports(limit);

export const getLN01History = (sourceId: string) => 
  rawDataService.getLN01History(sourceId);

export const getGL01History = (sourceId: string) => 
  rawDataService.getGL01History(sourceId);

export const getLN01Snapshot = (snapshotDate: string) => 
  rawDataService.getLN01Snapshot(snapshotDate);

export const getGL01Snapshot = (snapshotDate: string) => 
  rawDataService.getGL01Snapshot(snapshotDate);

export const cleanupOldData = (tableName: string, retainMonths?: number) => 
  rawDataService.cleanupOldData(tableName, retainMonths);

export const getDashboardStatistics = () => 
  rawDataService.getDashboardStatistics();

export default rawDataService;
