import apiClient from '../services/api.js';

// Employee KPI Assignment API Service
export const employeeKpiAssignmentApi = {
  // Single operations
  getAll: () => {
    return apiClient.get('/EmployeeKpiAssignment');
  },

  getById: (id) => {
    return apiClient.get(`/EmployeeKpiAssignment/${id}`);
  },

  getByEmployeeId: (employeeId) => {
    return apiClient.get(`/EmployeeKpiAssignment/employee/${employeeId}`);
  },

  getByKhoanPeriodId: (khoanPeriodId) => {
    return apiClient.get(`/EmployeeKpiAssignment/khoan-period/${khoanPeriodId}`);
  },

  create: (createDto) => {
    return apiClient.post('/EmployeeKpiAssignment', createDto);
  },

  update: (id, updateDto) => {
    return apiClient.put(`/EmployeeKpiAssignment/${id}`, updateDto);
  },

  delete: (id) => {
    return apiClient.delete(`/EmployeeKpiAssignment/${id}`);
  },

  checkExists: (employeeId, kpiDefinitionId, khoanPeriodId) => {
    return apiClient.get('/EmployeeKpiAssignment/exists', {
      params: { employeeId, kpiDefinitionId, khoanPeriodId }
    });
  },

  // Bulk operations - aligned with backend endpoints and DTOs

  // Create bulk assignments using individual assignment DTOs
  bulkCreateAssignments: (bulkCreateDto) => {
    console.log('🔧 API: Bulk create assignments:', bulkCreateDto);
    return apiClient.post('/EmployeeKpiAssignment/bulk', bulkCreateDto);
  },

  // Bulk assign KPIs to multiple employees (simpler bulk assignment)
  bulkAssignKPIs: (bulkKpiAssignmentDto) => {
    console.log('🔧 API: Bulk assign KPIs:', bulkKpiAssignmentDto);
    return apiClient.post('/EmployeeKpiAssignment/bulk-assign', bulkKpiAssignmentDto);
  },

  // Bulk update scores for multiple assignments
  bulkUpdateScores: (bulkScoreUpdateDto) => {
    console.log('🔧 API: Bulk update scores:', bulkScoreUpdateDto);
    return apiClient.put('/EmployeeKpiAssignment/bulk-update-scores', bulkScoreUpdateDto);
  },

  // Bulk delete assignments
  bulkDeleteAssignments: (bulkDeleteDto) => {
    console.log('🔧 API: Bulk delete assignments:', bulkDeleteDto);
    return apiClient.delete('/EmployeeKpiAssignment/bulk-delete', {
      data: bulkDeleteDto
    });
  },

  // Export functions
  exportAssignments: (khoanPeriodId) => {
    console.log('🔧 API: Export assignments for period:', khoanPeriodId);
    return apiClient.get(`/EmployeeKpiAssignment/export?khoanPeriodId=${khoanPeriodId}`, {
      responseType: 'blob'
    });
  },

  exportSummary: (khoanPeriodId) => {
    console.log('🔧 API: Export summary for period:', khoanPeriodId);
    return apiClient.get(`/EmployeeKpiAssignment/export-summary?khoanPeriodId=${khoanPeriodId}`, {
      responseType: 'blob'
    });
  }
};

export default employeeKpiAssignmentApi;
