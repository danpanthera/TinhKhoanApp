import api from './api';

export const kpiAssignmentService = {
  // Get all KPI assignment tables
  async getTables() {
    console.log('🔄 kpiAssignmentService.getTables() started');
    console.log('📡 Calling API: /KpiAssignmentTables');
    console.log('📡 Base URL:', import.meta.env.VITE_API_BASE_URL);

    const response = await api.get('/KpiAssignmentTables'); // Use raw SQL endpoint
    console.log('📨 KPI Tables API Response received:', response.status, response.data);

    let tablesData = [];
    if (response.data && Array.isArray(response.data.$values)) {
      console.log('✅ Found $values array with length:', response.data.$values.length);
      tablesData = response.data.$values;
    } else if (Array.isArray(response.data)) {
      console.log('✅ Found direct array with length:', response.data.length);
      tablesData = response.data;
    } else {
      console.error('❌ Invalid response format:', response.data);
    }

    console.log('📊 Final tablesData length:', tablesData.length);


    console.log('📊 Final tablesData length:', tablesData.length);

    // Enhanced categorization logic for better table classification
    const processedTables = tablesData.map(table => {
      // Use the category from backend if available, otherwise apply our logic
      if (table.Category || table.category) {
        // Backend provides 'CANBO' or 'CHINHANH' - keep these values unchanged
        table.category = table.Category || table.category;
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
          table.category = 'CANBO';
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
          table.category = 'CHINHANH';
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
            table.category = 'CHINHANH';
          }
          else if (
            tableNameLC.includes('cán bộ') ||
            tableNameLC.includes('nhân viên') ||
            tableNameLC.includes('trưởng phòng') ||
            tableNameLC.includes('giám đốc')
          ) {
            table.category = 'CANBO';
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

    console.log('✅ Processed KPI tables:', processedTables.length);
    console.log('🏷️ Categories found:', [...new Set(processedTables.map(t => t.category))]);

    return processedTables;
  },

  // Get table details with indicators
  async getTableDetails(tableId) {
    console.log('🔄 getTableDetails called with tableId:', tableId);
    try {
      const response = await api.get(`/KpiAssignment/tables/${tableId}`);
      const tableData = response.data;
      console.log('📨 getTableDetails API response:', tableData);

      // Handle both PascalCase (backend) and camelCase (frontend) indicators
      const indicators = tableData.Indicators || tableData.indicators;
      console.log('📊 Raw indicators from API:', indicators);

      if (indicators) {
        let indicatorsData = [];

        if (Array.isArray(indicators.$values)) {
          console.log('✅ Using indicators.$values, length:', indicators.$values.length);
          indicatorsData = indicators.$values;
        } else if (Array.isArray(indicators)) {
          console.log('✅ Using direct indicators array, length:', indicators.length);
          indicatorsData = indicators;
        } else {
          console.log('⚠️ indicators is not an array:', typeof indicators, indicators);
          indicatorsData = [];
        }

        // Sort by OrderIndex (PascalCase) or orderIndex (camelCase)
        tableData.indicators = indicatorsData.sort((a, b) => (a.OrderIndex || a.orderIndex || 0) - (b.OrderIndex || b.orderIndex || 0));
        console.log('📊 Final processed indicators count:', tableData.indicators.length);
        console.log('📊 First 2 indicators:', tableData.indicators.slice(0, 2));
      } else {
        console.log('❌ No indicators found in response');
        tableData.indicators = [];
      }

      return tableData;
    } catch (error) {
      console.error('❌ getTableDetails error:', error);
      throw error;
    }
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
