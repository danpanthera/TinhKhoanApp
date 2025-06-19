import api from './api';

export const kpiAssignmentService = {
  // Get all KPI assignment tables
  async getTables() {
    const response = await api.get('/KpiAssignment/tables');
    
    let tablesData = [];
    if (response.data && Array.isArray(response.data.$values)) {
      tablesData = response.data.$values;
    } else if (Array.isArray(response.data)) {
      tablesData = response.data;
    }
    
    return tablesData.sort((a, b) => a.tableType - b.tableType);
  },

  // Get table details with indicators
  async getTableDetails(tableId) {
    const response = await api.get(`/KpiAssignment/tables/${tableId}`);
    const tableData = response.data;
    
    if (tableData && tableData.indicators) {
      let indicatorsData = [];
      
      if (Array.isArray(tableData.indicators.$values)) {
        indicatorsData = tableData.indicators.$values;
      } else if (Array.isArray(tableData.indicators)) {
        indicatorsData = tableData.indicators;
      }
      
      tableData.indicators = indicatorsData.sort((a, b) => a.orderIndex - b.orderIndex);
    }
    
    return tableData;
  },

  // Assign KPI to employee
  async assignKPI(assignmentData) {
    const response = await api.post('/KpiAssignment/assign', assignmentData);
    return response.data;
  },

  // Get employee KPI assignments for a period
  async getEmployeeAssignments(employeeId, periodId) {
    const response = await api.get(`/KpiAssignment/employee/${employeeId}/period/${periodId}`);
    
    let assignmentsData = [];
    if (response.data && Array.isArray(response.data.$values)) {
      assignmentsData = response.data.$values;
    } else if (Array.isArray(response.data)) {
      assignmentsData = response.data;
    }
    
    return assignmentsData;
  },

  // Update actual values
  async updateActualValues(updates) {
    const response = await api.put('/KpiAssignment/update-single-actual', updates);
    return response.data;
  },

  // CRUD operations for KPI Indicators
  async createIndicator(indicatorData) {
    const response = await api.post('/KpiAssignment/indicators', indicatorData);
    return response.data;
  },

  async updateIndicator(indicatorId, indicatorData) {
    const response = await api.put(`/KpiAssignment/indicators/${indicatorId}`, indicatorData);
    return response.data;
  },

  async deleteIndicator(indicatorId) {
    const response = await api.delete(`/KpiAssignment/indicators/${indicatorId}`);
    return response.data;
  },

  async reorderIndicator(indicatorId, newOrderIndex) {
    const response = await api.put(`/KpiAssignment/indicators/${indicatorId}/reorder`, {
      newOrderIndex
    });
    return response.data;
  }
};

export default kpiAssignmentService;
