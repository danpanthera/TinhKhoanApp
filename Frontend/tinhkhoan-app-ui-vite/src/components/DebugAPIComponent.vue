<template>
  <div class="debug-component">
    <h2>🔍 Debug API Connection</h2>

    <div class="section">
      <h3>📊 Store Status</h3>
      <p><strong>Units Count:</strong> {{ unitStore.unitCount }}</p>
      <p><strong>Is Loading:</strong> {{ unitStore.isLoading }}</p>
      <p><strong>Error:</strong> {{ unitStore.error || 'None' }}</p>
    </div>

    <div class="section">
      <h3>🔧 Actions</h3>
      <button @click="testFetchUnits" :disabled="unitStore.isLoading">
        {{ unitStore.isLoading ? 'Loading...' : 'Test Fetch Units' }}
      </button>
      <button @click="testDirectAPI">Test Direct API Call</button>
    </div>

    <div class="section" v-if="directApiResult">
      <h3>📡 Direct API Result</h3>
      <pre>{{ directApiResult }}</pre>
    </div>

    <div class="section" v-if="unitStore.units.length > 0">
      <h3>📋 Units Data (First 5)</h3>
      <div v-for="unit in unitStore.units.slice(0, 5)" :key="unit.Id" class="unit-item">
        <strong>{{ unit.Name }}</strong> ({{ unit.Code }}) - {{ unit.Type }}
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import apiClient from '../services/api.js'
import { useUnitStore } from '../stores/unitStore.js'

const unitStore = useUnitStore()
const directApiResult = ref('')

const testFetchUnits = async () => {
  console.log('🧪 Testing fetchUnits from component')
  try {
    await unitStore.fetchUnits()
    console.log('✅ fetchUnits completed')
  } catch (error) {
    console.error('❌ fetchUnits error:', error)
  }
}

const testDirectAPI = async () => {
  console.log('🧪 Testing direct API call')
  try {
    const response = await apiClient.get('/Units')
    directApiResult.value = {
      status: response.status,
      dataType: Array.isArray(response.data) ? 'array' : typeof response.data,
      length: Array.isArray(response.data) ? response.data.length : 'N/A',
      sample: Array.isArray(response.data) ? response.data.slice(0, 3) : response.data
    }
    console.log('✅ Direct API call success:', directApiResult.value)
  } catch (error) {
    console.error('❌ Direct API call error:', error)
    directApiResult.value = { error: error.message }
  }
}

// Auto-test on mount
setTimeout(() => {
  console.log('🚀 Debug component mounted, auto-testing...')
  testFetchUnits()
}, 1000)
</script>

<style scoped>
.debug-component {
  padding: 20px;
  max-width: 800px;
}

.section {
  margin: 20px 0;
  padding: 15px;
  border: 1px solid #ddd;
  border-radius: 5px;
  background: #f8f9fa;
}

.unit-item {
  padding: 8px;
  margin: 4px 0;
  background: white;
  border-radius: 3px;
  border-left: 3px solid #007bff;
}

button {
  padding: 10px 15px;
  margin: 5px;
  background: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

button:disabled {
  background: #6c757d;
  cursor: not-allowed;
}

pre {
  background: #f1f1f1;
  padding: 10px;
  border-radius: 3px;
  overflow-x: auto;
}
</style>
