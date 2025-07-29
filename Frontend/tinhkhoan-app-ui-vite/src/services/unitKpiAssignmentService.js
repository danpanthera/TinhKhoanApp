import apiClient from './api'

class UnitKpiAssignmentService {
  async getUnitKpiAssignments() {
    try {
      const response = await apiClient.get('/UnitKhoanAssignments')
      return response.data
    } catch (error) {
      console.error('Error fetching unit KPI assignments:', error)
      throw error
    }
  }

  async getUnitKpiAssignment(id) {
    try {
      const response = await apiClient.get(`/UnitKhoanAssignments/${id}`)
      return response.data
    } catch (error) {
      console.error('Error fetching unit KPI assignment:', error)
      throw error
    }
  }

  async createUnitKpiAssignment(assignment) {
    try {
      const response = await apiClient.post('/UnitKhoanAssignments', assignment)
      return response.data
    } catch (error) {
      console.error('Error creating unit KPI assignment:', error)
      throw error
    }
  }

  async updateUnitKpiAssignment(id, assignment) {
    try {
      const response = await apiClient.put(`/UnitKhoanAssignments/${id}`, assignment)
      return response.data
    } catch (error) {
      console.error('Error updating unit KPI assignment:', error)
      throw error
    }
  }

  async deleteUnitKpiAssignment(id) {
    try {
      await apiClient.delete(`/UnitKhoanAssignments/${id}`)
      return true
    } catch (error) {
      console.error('Error deleting unit KPI assignment:', error)
      throw error
    }
  }

  // Alias method for getAssignments (to match component usage)
  async getAssignments() {
    return await this.getUnitKpiAssignments()
  }

  // Create assignment (alias for createUnitKpiAssignment)
  async createAssignment(assignment) {
    return await this.createUnitKpiAssignment(assignment)
  }

  // Delete assignment (alias for deleteUnitKpiAssignment)
  async deleteAssignment(id) {
    return await this.deleteUnitKpiAssignment(id)
  }

  // Get KPI definitions by unit type
  async getKpiDefinitionsByUnitType(unitType) {
    try {
      // Map unit type to table type for KPI indicators
      let tableType = ''
      if (unitType === 'CNL1') {
        tableType = 'HoiSo' // Hội sở table for CNL1
      } else if (unitType === 'CNL2') {
        tableType = 'GiamdocCnl2' // Giám đốc CNL2 table for CNL2
      }

      if (tableType) {
        const result = await this.getKpiIndicatorsByTableType(tableType)
        return result.indicators || []
      }

      return []
    } catch (error) {
      console.error('Error fetching KPI definitions by unit type:', error)
      throw error
    }
  }

  // Get units by type (CNL1, CNL2, etc.)
  async getUnitsByType(type = null) {
    try {
      const response = await apiClient.get('/Units')
      let units = response.data

      if (type) {
        units = units.filter(unit => unit.type === type)
      }

      return units
    } catch (error) {
      console.error('Error fetching units:', error)
      throw error
    }
  }

  // Get CNL1 units (parent units)
  async getCNL1Units() {
    return await this.getUnitsByType('CNL1')
  }

  // Get CNL2 units (child units)
  async getCNL2Units() {
    return await this.getUnitsByType('CNL2')
  }

  // Get child units of a parent unit
  async getChildUnits(parentUnitId) {
    try {
      const response = await apiClient.get('/Units')
      const units = response.data
      return units.filter(unit => unit.parentUnitId === parentUnitId)
    } catch (error) {
      console.error('Error fetching child units:', error)
      throw error
    }
  }

  // Get KPI periods
  async getKhoanPeriods() {
    try {
      const response = await apiClient.get('/KhoanPeriods')
      let periods = response.data

      // Handle .NET JSON serialization format with $values
      if (periods && periods.$values) {
        periods = periods.$values
      }

      return periods
    } catch (error) {
      console.error('Error fetching khoan periods:', error)
      throw error
    }
  }

  // Get KPI tables for units
  async getKpiTables() {
    try {
      const response = await apiClient.get('/KpiAssignment/tables')
      let tables = response.data

      // Handle .NET JSON serialization format with $values
      if (tables && tables.$values) {
        tables = tables.$values
      }

      return tables
    } catch (error) {
      console.error('Error fetching KPI tables:', error)
      throw error
    }
  }

  // Get KPI indicators for a table
  async getKpiIndicators(tableId) {
    try {
      const response = await apiClient.get(`/KpiAssignment/tables/${tableId}`)
      let data = response.data

      // Handle .NET JSON serialization format with $values for indicators
      if (data.indicators && data.indicators.$values) {
        data.indicators = data.indicators.$values
      }

      return data
    } catch (error) {
      console.error('Error fetching KPI indicators:', error)
      throw error
    }
  }

  // Get KPI tables specifically for branches (CNL1, CNL2)
  async getBranchKpiTables() {
    try {
      const response = await apiClient.get('/KpiAssignment/tables')
      let tables = response.data

      // Handle .NET JSON serialization format with $values
      if (tables && tables.$values) {
        tables = tables.$values
      }

      // Filter for branch-specific tables
      const branchTables = tables.filter(
        table => table.tableType.includes('Cnl') || table.tableType.includes('CNL') || table.tableName.includes('CNL')
      )

      return branchTables
    } catch (error) {
      console.error('Error fetching branch KPI tables:', error)
      throw error
    }
  }

  // Get KPI indicators for specific unit type (CNL1, CNL2)
  async getKpiIndicatorsForUnitType(unitType) {
    try {
      // Determine the appropriate table type based on unit type
      let tableType = ''
      if (unitType === 'CNL1') {
        tableType = 'HoiSo' // Use HoiSo table for CNL1
      } else if (unitType === 'CNL2') {
        tableType = 'GiamdocCnl2' // Use GiamdocCnl2 table for CNL2
      }

      if (tableType) {
        return await this.getKpiIndicatorsByTableType(tableType)
      }

      return { indicators: [] }
    } catch (error) {
      console.error('Error fetching KPI indicators for unit type:', error)
      throw error
    }
  }

  // Get KPI indicators by specific table type
  async getKpiIndicatorsByTableType(tableType) {
    try {
      const response = await apiClient.get(`/KpiAssignment/table/${tableType}`)
      let data = response.data

      console.log(`📊 Raw API response for ${tableType}:`, data)

      // Extract indicators with proper $values handling
      let indicators = []
      if (data && data.indicators) {
        if (data.indicators.$values) {
          indicators = data.indicators.$values
        } else if (Array.isArray(data.indicators)) {
          indicators = data.indicators
        }
      }

      // Format the response to match expected structure
      const result = {
        tableId: data.id,
        tableName: data.tableName,
        tableType: data.tableType,
        indicators: indicators,
      }

      console.log(`📊 Processed KPI table ${tableType}:`, result)
      console.log(`📊 Indicator count: ${indicators.length}`)
      return result
    } catch (error) {
      console.error(`Error fetching KPI indicators for table type ${tableType}:`, error)
      throw error
    }
  }
}

export default new UnitKpiAssignmentService()
