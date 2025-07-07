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

    // Enhanced categorization logic for better table classification
    return tablesData.map(table => {
      // Use the category from backend if available, otherwise apply our logic
      if (table.category) {
        // Backend already provides the category, just ensure consistent naming
        if (table.category === 'Dành cho Cán bộ') {
          table.category = 'Vai trò cán bộ';
        } else if (table.category === 'Dành cho Chi nhánh') {
          table.category = 'Chi nhánh';
        }
      } else {
        // Fallback logic if category is not provided by backend
        // Safely check tableType - ensure it's a string
        const tableType = table.tableType || '';
        const tableTypeLC = typeof tableType === 'string' ? tableType.toLowerCase() : '';

        // Employee tables - using a more comprehensive list of keywords
        if (
          tableTypeLC.includes('role') ||
          tableTypeLC.includes('employee') ||
          tableTypeLC.includes('canbo') ||
          tableTypeLC.includes('cán bộ') ||
          tableTypeLC.includes('nhân viên') ||
          tableTypeLC.includes('nhân sự') ||
          tableTypeLC.includes('trưởng phòng') ||
          tableTypeLC.includes('giám đốc') ||
          tableTypeLC.includes('director') ||
          tableTypeLC.includes('staff') ||
          tableTypeLC.includes('manager') ||
          tableTypeLC.startsWith('tp_') || // Trưởng phòng
          tableTypeLC.startsWith('nv_') || // Nhân viên
          tableTypeLC.startsWith('cb_') ||   // Cán bộ
          // Add numeric checks for employee tables (0-22 are employee tables based on requirement)
          (typeof table.tableType === 'number' && table.tableType >= 0 && table.tableType <= 22) ||
          (typeof table.tableType === 'string' && /^\d+$/.test(table.tableType) && parseInt(table.tableType) >= 0 && parseInt(table.tableType) <= 22)
        ) {
          table.category = 'Vai trò cán bộ';
        }
        // Branch tables - using a more comprehensive list of keywords
        else if (
          tableTypeLC.includes('cn') ||
          tableTypeLC.includes('branch') ||
          tableTypeLC.includes('chinhanh') ||
          tableTypeLC.includes('chi nhánh') ||
          tableTypeLC.includes('phòng giao dịch') ||
          tableTypeLC.includes('pgd') ||
          tableTypeLC.includes('hội sở') ||
          tableTypeLC.includes('hoiso') ||
          tableTypeLC.includes('văn phòng') ||
          tableTypeLC.includes('vpdd') ||
          tableTypeLC.includes('office') ||
          tableTypeLC.startsWith('hs_') || // Hội sở
          tableTypeLC.startsWith('cn_')    // Chi nhánh
        ) {
          table.category = 'Chi nhánh';
        }
        // Fallback for other cases
        else {
          // Check for known patterns in table names
          const tableName = table.tableName || '';
          const tableNameLC = typeof tableName === 'string' ? tableName.toLowerCase() : '';

          if (
            tableNameLC.includes('chi nhánh') ||
            tableNameLC.includes('phòng giao dịch') ||
            tableNameLC.includes('hội sở')
          ) {
            table.category = 'Chi nhánh';
          }
          else if (
            tableNameLC.includes('cán bộ') ||
            tableNameLC.includes('nhân viên') ||
            tableNameLC.includes('trưởng phòng') ||
            tableNameLC.includes('giám đốc')
          ) {
            table.category = 'Vai trò cán bộ';
          }
          else {
            // Default category if we can't determine
            table.category = 'Khác';
          }
        }
      }

      // Format the table code for better display in the UI
      if (table.tableType) {
        table.displayCode = String(table.tableType).toUpperCase();
      }

      return table;
    }).sort((a, b) => {
      // Custom sorting for branch names in the specified order (cập nhật tên mới)
      const branchOrder = [
        'CnLaiChau', 'CnBinhLu', 'CnPhongTho', 'CnSinHo', 'CnBumTo',
        'CnThanUyen', 'CnDoanKet', 'CnTanUyen', 'CnNamHang'
      ];

      const aType = String(a.tableType || '');
      const bType = String(b.tableType || '');

      const aOrder = branchOrder.indexOf(aType);
      const bOrder = branchOrder.indexOf(bType);

      // If both are in the custom order list, sort by that order
      if (aOrder !== -1 && bOrder !== -1) {
        return aOrder - bOrder;
      }
      // If only one is in the custom order list, prioritize it
      if (aOrder !== -1) return -1;
      if (bOrder !== -1) return 1;

      // Default alphabetical sort for everything else
      return aType.localeCompare(bType);
    });
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
