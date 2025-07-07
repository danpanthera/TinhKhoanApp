<template>
  <div class="store-debug-panel">
    <h2>🔍 Store Debug Panel</h2>

    <!-- Units Store Test -->
    <div class="debug-section">
      <h3>📦 Units Store Test</h3>
      <button @click="testUnitsStore" :disabled="testing">Test Units Store</button>
      <div class="debug-results">
        <p><strong>Loading:</strong> {{ unitStore.isLoading }}</p>
        <p><strong>Error:</strong> {{ unitStore.error || 'None' }}</p>
        <p><strong>Units Count:</strong> {{ unitStore.allUnits.length }}</p>
        <div v-if="unitStore.allUnits.length > 0" class="data-preview">
          <strong>First 3 Units:</strong>
          <pre>{{ JSON.stringify(unitStore.allUnits.slice(0, 3), null, 2) }}</pre>
        </div>
      </div>
    </div>

    <!-- Roles Store Test -->
    <div class="debug-section">
      <h3>🎭 Roles Store Test</h3>
      <button @click="testRolesStore" :disabled="testing">Test Roles Store</button>
      <div class="debug-results">
        <p><strong>Loading:</strong> {{ roleStore.isLoading }}</p>
        <p><strong>Error:</strong> {{ roleStore.error || 'None' }}</p>
        <p><strong>Roles Count:</strong> {{ roleStore.allRoles.length }}</p>
        <div v-if="roleStore.allRoles.length > 0" class="data-preview">
          <strong>First 3 Roles:</strong>
          <pre>{{ JSON.stringify(roleStore.allRoles.slice(0, 3), null, 2) }}</pre>
        </div>
      </div>
    </div>

    <!-- Employees Store Test -->
    <div class="debug-section">
      <h3>👥 Employees Store Test</h3>
      <button @click="testEmployeesStore" :disabled="testing">Test Employees Store</button>
      <div class="debug-results">
        <p><strong>Loading:</strong> {{ employeeStore.isLoading }}</p>
        <p><strong>Error:</strong> {{ employeeStore.error || 'None' }}</p>
        <p><strong>Employees Count:</strong> {{ employeeStore.allEmployees.length }}</p>
        <div v-if="employeeStore.allEmployees.length > 0" class="data-preview">
          <strong>First 3 Employees:</strong>
          <pre>{{ JSON.stringify(employeeStore.allEmployees.slice(0, 3), null, 2) }}</pre>
        </div>
      </div>
    </div>

    <!-- Debug Log -->
    <div class="debug-section">
      <h3>📋 Debug Log</h3>
      <button @click="clearLog">Clear Log</button>
      <div class="log-container">
        <div v-for="(log, index) in debugLogs" :key="index" class="log-entry">
          {{ log }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useEmployeeStore } from '../stores/employeeStore.js'
import { useRoleStore } from '../stores/roleStore.js'
import { useUnitStore } from '../stores/unitStore.js'

const unitStore = useUnitStore()
const roleStore = useRoleStore()
const employeeStore = useEmployeeStore()

const testing = ref(false)
const debugLogs = ref([])

const addLog = (message) => {
  const timestamp = new Date().toLocaleTimeString()
  debugLogs.value.push(`[${timestamp}] ${message}`)
  console.log(message)
}

const testUnitsStore = async () => {
  testing.value = true
  addLog('🚀 Testing Units Store...')
  try {
    addLog('📞 Calling unitStore.fetchUnits()')
    await unitStore.fetchUnits()
    addLog(`✅ Units Store Success: ${unitStore.allUnits.length} units loaded`)
  } catch (error) {
    addLog(`❌ Units Store Failed: ${error.message}`)
  } finally {
    testing.value = false
  }
}

const testRolesStore = async () => {
  testing.value = true
  addLog('🚀 Testing Roles Store...')
  try {
    addLog('📞 Calling roleStore.fetchRoles()')
    await roleStore.fetchRoles()
    addLog(`✅ Roles Store Success: ${roleStore.allRoles.length} roles loaded`)
  } catch (error) {
    addLog(`❌ Roles Store Failed: ${error.message}`)
  } finally {
    testing.value = false
  }
}

const testEmployeesStore = async () => {
  testing.value = true
  addLog('🚀 Testing Employees Store...')
  try {
    addLog('📞 Calling employeeStore.fetchEmployees()')
    await employeeStore.fetchEmployees()
    addLog(`✅ Employees Store Success: ${employeeStore.allEmployees.length} employees loaded`)
  } catch (error) {
    addLog(`❌ Employees Store Failed: ${error.message}`)
  } finally {
    testing.value = false
  }
}

const clearLog = () => {
  debugLogs.value = []
}

// Auto-test on mount
const testAllStores = async () => {
  addLog('🔄 Auto-testing all stores on component mount...')
  await testUnitsStore()
  await testRolesStore()
  await testEmployeesStore()
}

// Test on mount
testAllStores()
</script>

<style scoped>
.store-debug-panel {
  background: #f8f9fa;
  border: 2px solid #007bff;
  border-radius: 8px;
  padding: 20px;
  margin: 20px;
  font-family: 'Segoe UI', sans-serif;
}

.debug-section {
  background: white;
  border: 1px solid #dee2e6;
  border-radius: 6px;
  padding: 15px;
  margin: 15px 0;
}

.debug-section h3 {
  margin-top: 0;
  color: #495057;
}

.debug-results {
  margin: 10px 0;
}

.debug-results p {
  margin: 5px 0;
  font-family: monospace;
}

.data-preview {
  background: #f1f3f4;
  border: 1px solid #ddd;
  border-radius: 4px;
  padding: 10px;
  margin: 10px 0;
  max-height: 300px;
  overflow-y: auto;
}

.data-preview pre {
  margin: 0;
  font-size: 12px;
  white-space: pre-wrap;
}

button {
  background: #007bff;
  color: white;
  border: none;
  padding: 8px 16px;
  border-radius: 4px;
  cursor: pointer;
  margin: 5px;
}

button:hover {
  background: #0056b3;
}

button:disabled {
  background: #6c757d;
  cursor: not-allowed;
}

.log-container {
  background: #000;
  color: #00ff00;
  padding: 10px;
  border-radius: 4px;
  max-height: 300px;
  overflow-y: auto;
  font-family: monospace;
  font-size: 12px;
}

.log-entry {
  margin: 2px 0;
}
</style>
