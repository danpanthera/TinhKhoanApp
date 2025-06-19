<template>
  <div class="debug-dropdown-container">
    <h3>🔧 Debug Dropdown Cascade</h3>
    
    <div class="debug-info">
      <p><strong>Units loaded:</strong> {{ units.length }}</p>
      <p><strong>Root unit:</strong> {{ rootUnit?.name || 'Not found' }} (ID: {{ rootUnit?.id }})</p>
      <p><strong>Branch options:</strong> {{ branchOptions.length }}</p>
      <p><strong>Department options:</strong> {{ departmentOptions.length }}</p>
    </div>

    <div class="form-group">
      <label>🏢 Cấp đơn vị:</label>
      <select v-model="selectedUnitLevel" @change="onUnitLevelChange">
        <option value="">-- Chọn cấp đơn vị --</option>
        <option value="CNL1">🏢 CNL1 (Trụ sở chính)</option>
        <option value="CNL2">🏪 CNL2 (Chi nhánh)</option>
      </select>
    </div>

    <div v-if="selectedUnitLevel === 'CNL2'" class="form-group">
      <label>🏪 Chi nhánh:</label>
      <select v-model="selectedBranch" @change="onBranchChange">
        <option value="">-- Chọn chi nhánh --</option>
        <option v-for="branch in branchOptions" :key="branch.id" :value="branch.id">
          {{ branch.name }} ({{ branch.code }})
        </option>
      </select>
    </div>

    <div v-if="showDepartments" class="form-group">
      <label>🏢 Phòng nghiệp vụ:</label>
      <select v-model="selectedDepartment">
        <option value="">-- Chọn phòng nghiệp vụ --</option>
        <option v-for="dept in departmentOptions" :key="dept.id" :value="dept.id">
          {{ dept.name }} ({{ dept.code }})
        </option>
      </select>
    </div>

    <div class="debug-output">
      <h4>Debug Output:</h4>
      <pre>{{ debugOutput }}</pre>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import api from '../services/api';

// Reactive data
const units = ref([]);
const selectedUnitLevel = ref('');
const selectedBranch = ref('');
const selectedDepartment = ref('');

// Computed properties
const rootUnit = computed(() => {
  return units.value.find(unit => unit.parentUnitId === null);
});

// Updated branchOptions: Use SortOrder from backend instead of hardcoded sorting
const branchOptions = computed(() => {
  if (!units.value || !rootUnit.value) return [];
  
  const branches = units.value.filter(unit => 
    unit.parentUnitId === rootUnit.value.id && 
    (unit.name?.includes('CN ') || /^\d+$/.test(unit.code))
  );
  
  // Sort by SortOrder (from backend), then by Name as fallback
  const sortedBranches = branches.sort((a, b) => {
    // Primary sort: SortOrder (nulls last)
    const sortOrderA = a.sortOrder ?? Number.MAX_SAFE_INTEGER;
    const sortOrderB = b.sortOrder ?? Number.MAX_SAFE_INTEGER;
    
    if (sortOrderA !== sortOrderB) {
      return sortOrderA - sortOrderB;
    }
    
    // Secondary sort: Name
    return (a.name || '').localeCompare(b.name || '');
  });
  
  console.log('Branch options:', sortedBranches);
  return sortedBranches;
});

const departmentOptions = computed(() => {
  if (!units.value || !units.value.length) return [];
  
  if (selectedUnitLevel.value === 'CNL1') {
    const departments = units.value.filter(unit => 
      unit.parentUnitId === rootUnit.value?.id && 
      (unit.name?.includes('Phòng') || unit.name?.includes('Ban'))
    );
    console.log('CNL1 departments:', departments);
    return departments;
  } 
  else if (selectedUnitLevel.value === 'CNL2' && selectedBranch.value) {
    const departments = units.value.filter(unit => 
      unit.parentUnitId === selectedBranch.value
    );
    console.log('CNL2 departments:', departments);
    return departments;
  }
  
  return [];
});

const showDepartments = computed(() => {
  return selectedUnitLevel.value === 'CNL1' || 
         (selectedUnitLevel.value === 'CNL2' && selectedBranch.value);
});

const debugOutput = computed(() => {
  return {
    selectedUnitLevel: selectedUnitLevel.value,
    selectedBranch: selectedBranch.value,
    selectedDepartment: selectedDepartment.value,
    rootUnitId: rootUnit.value?.id,
    branchCount: branchOptions.value.length,
    departmentCount: departmentOptions.value.length,
    totalUnits: units.value.length
  };
});

// Methods
const fetchUnits = async () => {
  try {
    const response = await api.get('/Units');
    let unitsData = [];
    
    if (response.data && Array.isArray(response.data.$values)) {
      unitsData = response.data.$values;
    } else if (Array.isArray(response.data)) {
      unitsData = response.data;
    }
    
    units.value = unitsData;
    console.log('Units loaded:', unitsData.length);
    console.log('Sample units:', unitsData.slice(0, 3));
  } catch (error) {
    console.error('Error loading units:', error);
  }
};

const onUnitLevelChange = () => {
  selectedBranch.value = '';
  selectedDepartment.value = '';
  console.log('Unit level changed to:', selectedUnitLevel.value);
};

const onBranchChange = () => {
  selectedDepartment.value = '';
  console.log('Branch changed to:', selectedBranch.value);
};

// Lifecycle
onMounted(() => {
  fetchUnits();
});
</script>

<style scoped>
.debug-dropdown-container {
  max-width: 600px;
  margin: 20px auto;
  padding: 20px;
  border: 2px solid #007bff;
  border-radius: 10px;
  background: #f8f9fa;
}

.debug-info {
  background: #e9ecef;
  padding: 10px;
  border-radius: 5px;
  margin-bottom: 15px;
}

.form-group {
  margin-bottom: 15px;
}

label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
}

select {
  width: 100%;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.debug-output {
  background: #fff;
  border: 1px solid #ddd;
  border-radius: 4px;
  padding: 10px;
  margin-top: 15px;
}

pre {
  margin: 0;
  font-size: 12px;
  white-space: pre-wrap;
}
</style>
