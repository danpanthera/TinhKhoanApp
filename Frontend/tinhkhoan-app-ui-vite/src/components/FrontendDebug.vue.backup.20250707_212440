<template>
  <div style="padding: 20px; font-family: monospace;">
    <h2>🔧 Frontend Store Debug Test</h2>

    <div style="background: #f0f0f0; padding: 15px; margin: 10px 0; border-radius: 8px;">
      <h3>📡 API Test</h3>
      <button @click="testDirectAPI" style="padding: 8px 16px; margin: 5px; background: #007bff; color: white; border: none; border-radius: 4px;">Test Direct API Call</button>
      <div v-if="apiTestResult" style="margin-top: 10px; padding: 10px; background: white; border-radius: 4px;">
        <strong>API Result:</strong>
        <pre>{{ JSON.stringify(apiTestResult, null, 2) }}</pre>
      </div>
    </div>

    <div style="background: #e8f5e8; padding: 15px; margin: 10px 0; border-radius: 8px;">
      <h3>🏪 Store Test</h3>
      <button @click="testUnitStore" style="padding: 8px 16px; margin: 5px; background: #28a745; color: white; border: none; border-radius: 4px;">Test Unit Store</button>
      <div style="margin-top: 10px;">
        <div><strong>Store Loading:</strong> {{ unitStore.isLoading }}</div>
        <div><strong>Store Error:</strong> {{ unitStore.error || 'None' }}</div>
        <div><strong>Units Count:</strong> {{ unitStore.units.length }}</div>
        <div><strong>First Unit:</strong> {{ JSON.stringify(unitStore.units[0] || {}) }}</div>
      </div>
    </div>

    <div style="background: #fff3cd; padding: 15px; margin: 10px 0; border-radius: 8px;">
      <h3>🔍 Environment Debug</h3>
      <div><strong>Base URL:</strong> {{ baseURL }}</div>
      <div><strong>Environment:</strong> {{ envMode }}</div>
      <div><strong>Dev:</strong> {{ envDev }}</div>
      <div><strong>API URL:</strong> {{ envApiUrl }}</div>
    </div>

    <div style="background: #f8d7da; padding: 15px; margin: 10px 0; border-radius: 8px;">
      <h3>📋 Console Logs</h3>
      <div v-for="(log, index) in logs" :key="index" style="margin: 2px 0; font-size: 12px;">
        {{ log }}
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { useUnitStore } from '../stores/unitStore.js'

const unitStore = useUnitStore()
const apiTestResult = ref(null)
const logs = ref([])
const baseURL = import.meta.env.VITE_API_BASE_URL || "http://localhost:5055/api"

// Environment variables as reactive refs
const envMode = ref(import.meta.env.MODE)
const envDev = ref(import.meta.env.DEV)
const envApiUrl = ref(import.meta.env.VITE_API_BASE_URL)

function addLog(message) {
  logs.value.push(`${new Date().toLocaleTimeString()} - ${message}`)
  console.log(message)
}

async function testDirectAPI() {
  addLog('🧪 Testing direct API call...')
  try {
    const response = await fetch(`${baseURL}/units`)
    addLog(`📡 Response status: ${response.status}`)

    const data = await response.json()
    addLog(`📊 Data type: ${typeof data}, Array: ${Array.isArray(data)}, Length: ${Array.isArray(data) ? data.length : 'N/A'}`)

    apiTestResult.value = {
      status: response.status,
      headers: Object.fromEntries(response.headers.entries()),
      dataType: typeof data,
      isArray: Array.isArray(data),
      length: Array.isArray(data) ? data.length : null,
      firstItem: Array.isArray(data) ? data[0] : data
    }

    addLog('✅ Direct API call successful')
  } catch (error) {
    addLog(`❌ Direct API call failed: ${error.message}`)
    apiTestResult.value = { error: error.message }
  }
}

async function testUnitStore() {
  addLog('🏪 Testing unit store...')
  try {
    addLog('📞 Calling unitStore.fetchUnits()')
    await unitStore.fetchUnits()
    addLog(`✅ Store call completed. Units count: ${unitStore.units.length}`)
  } catch (error) {
    addLog(`❌ Store call failed: ${error.message}`)
  }
}

onMounted(() => {
  addLog('🚀 Component mounted')
  addLog(`🔧 Base URL: ${baseURL}`)
  addLog(`🔧 Environment: ${envMode.value}`)
  addLog(`🔧 Dev mode: ${envDev.value}`)
  addLog(`🔧 API URL: ${envApiUrl.value}`)
})
</script>
