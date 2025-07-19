<template>
  <div class="debug-panel" v-if="showDebug">
    <div class="debug-header">
      <h4>🐛 Debug Information</h4>
      <button @click="toggleDebug" class="btn btn-sm btn-outline-secondary">
        {{ showDebug ? 'Hide' : 'Show' }}
      </button>
    </div>

    <div class="debug-content">
      <div class="row">
        <div class="col-md-6">
          <h5>Units Store State</h5>
          <div class="debug-info">
            <p><strong>Units Count:</strong> {{ unitStore.units.length }}</p>
            <p><strong>Loading:</strong> {{ unitStore.isLoading }}</p>
            <p><strong>Error:</strong> {{ unitStore.error || 'None' }}</p>
            <details v-if="unitStore.units.length > 0">
              <summary>First 3 Units</summary>
              <pre>{{ JSON.stringify(unitStore.units.slice(0, 3), null, 2) }}</pre>
            </details>
          </div>
        </div>

        <div class="col-md-6">
          <h5>KPI Tables State</h5>
          <div class="debug-info">
            <p><strong>Tables Count:</strong> {{ kpiTables.length }}</p>
            <p><strong>Loading:</strong> {{ isLoadingKpi }}</p>
            <p><strong>Error:</strong> {{ kpiError || 'None' }}</p>
            <details v-if="kpiTables.length > 0">
              <summary>First 3 KPI Tables</summary>
              <pre>{{ JSON.stringify(kpiTables.slice(0, 3), null, 2) }}</pre>
            </details>
          </div>
        </div>
      </div>

      <div class="row mt-3">
        <div class="col-md-12">
          <h5>Action Buttons</h5>
          <button @click="refreshUnits" class="btn btn-sm btn-primary me-2">
            Refresh Units
          </button>
          <button @click="refreshKpiTables" class="btn btn-sm btn-primary me-2">
            Refresh KPI Tables
          </button>
          <button @click="testApiDirectly" class="btn btn-sm btn-info">
            Test API Directly
          </button>
        </div>
      </div>

      <div class="row mt-3" v-if="directApiResult">
        <div class="col-md-12">
          <h5>Direct API Test Result</h5>
          <pre class="api-result">{{ directApiResult }}</pre>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import apiClient from '../services/api.js'
import { kpiAssignmentService } from '../services/kpiAssignmentService.js'
import { useUnitStore } from '../stores/unitStore.js'

const showDebug = ref(true)
const unitStore = useUnitStore()
const kpiTables = ref([])
const isLoadingKpi = ref(false)
const kpiError = ref(null)
const directApiResult = ref('')

const toggleDebug = () => {
  showDebug.value = !showDebug.value
}

const refreshUnits = async () => {
  console.log('🔄 Manual refresh units triggered')
  await unitStore.fetchUnits()
}

const refreshKpiTables = async () => {
  console.log('🔄 Manual refresh KPI tables triggered')
  isLoadingKpi.value = true
  kpiError.value = null
  try {
    const tables = await kpiAssignmentService.getTables()
    kpiTables.value = tables
    console.log('✅ KPI tables refreshed:', tables.length)
  } catch (error) {
    kpiError.value = error.message
    console.error('❌ Error refreshing KPI tables:', error)
  } finally {
    isLoadingKpi.value = false
  }
}

const testApiDirectly = async () => {
  console.log('🧪 Testing API directly')
  try {
    const [unitsResponse, kpiResponse] = await Promise.all([
      apiClient.get('/Units'),
      apiClient.get('/KpiAssignmentTables')
    ])

    directApiResult.value = {
      units: {
        status: unitsResponse.status,
        dataType: Array.isArray(unitsResponse.data) ? 'array' : typeof unitsResponse.data,
        count: Array.isArray(unitsResponse.data) ? unitsResponse.data.length :
               (unitsResponse.data?.$values ? unitsResponse.data.$values.length : 'unknown')
      },
      kpiTables: {
        status: kpiResponse.status,
        dataType: Array.isArray(kpiResponse.data) ? 'array' : typeof kpiResponse.data,
        count: Array.isArray(kpiResponse.data) ? kpiResponse.data.length :
               (kpiResponse.data?.$values ? kpiResponse.data.$values.length : 'unknown')
      }
    }
  } catch (error) {
    directApiResult.value = { error: error.message }
  }
}

onMounted(() => {
  refreshKpiTables()
})
</script>

<style scoped>
.debug-panel {
  position: fixed;
  top: 10px;
  right: 10px;
  width: 60%;
  max-height: 80vh;
  overflow-y: auto;
  background: rgba(255, 255, 255, 0.95);
  border: 2px solid #007bff;
  border-radius: 8px;
  padding: 15px;
  z-index: 9999;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
}

.debug-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 15px;
  border-bottom: 1px solid #ddd;
  padding-bottom: 10px;
}

.debug-info {
  background: #f8f9fa;
  padding: 10px;
  border-radius: 4px;
  margin-bottom: 10px;
}

.debug-info p {
  margin: 5px 0;
  font-size: 14px;
}

pre {
  background: #343a40;
  color: #fff;
  padding: 10px;
  border-radius: 4px;
  font-size: 12px;
  max-height: 200px;
  overflow-y: auto;
}

.api-result {
  background: #e9ecef;
  color: #495057;
}

details summary {
  cursor: pointer;
  font-weight: bold;
  margin-bottom: 5px;
}
</style>
