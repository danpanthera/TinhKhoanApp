<template>
  <div class="units-view">
    <h1>Danh sách Đơn vị</h1>

    <!-- 🔍 Debug Info Panel -->
    <div
      style="
        background: #e3f2fd;
        border: 2px solid #1976d2;
        padding: 15px;
        margin: 15px 0;
        border-radius: 8px;
        font-family: monospace;
      "
    >
      <h3 style="margin: 0 0 10px 0; color: #1976d2">🔍 DEBUG INFO</h3>
      <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 10px; font-size: 14px">
        <div><strong>Store Loading:</strong> {{ unitStore.isLoading }}</div>
        <div><strong>Store Error:</strong> {{ unitStore.error || 'None' }}</div>
        <div><strong>All Units Count:</strong> {{ unitStore.allUnits.length }}</div>
        <div><strong>Branches Count:</strong> {{ branches.length }}</div>
        <div style="grid-column: 1/-1">
          <strong>First Unit:</strong> {{ JSON.stringify(unitStore.allUnits[0] || {}) }}
        </div>
      </div>
      <button
        @click="loadUnits"
        style="margin-top: 10px; background: #1976d2; color: white; border: none; padding: 8px 16px; border-radius: 4px"
      >
        🔄 Reload Units
      </button>
    </div>

    <!-- Section quản lý đơn vị được chọn -->
    <div
      class="selection-management"
      style="background: #f8f9fa; padding: 16px; border-radius: 8px; margin-bottom: 20px; border: 1px solid #e9ecef"
    >
      <div style="display: flex; gap: 12px; align-items: center; margin-bottom: 16px">
        <button
          @click="toggleSelectionMode"
          class="action-button"
          :style="{
            backgroundColor: isSelectionMode ? '#e74c3c' : '#2ecc71',
            borderColor: isSelectionMode ? '#c0392b' : '#27ae60',
          }"
        >
          {{ isSelectionMode ? '✕ Thoát chế độ chọn' : '☑ Chọn Đơn vị' }}
        </button>

        <button
          v-if="isSelectionMode && selectedUnits.size > 0"
          @click="selectAllVisible"
          class="action-button"
          style="background-color: #3498db; border-color: #2980b9"
        >
          Chọn tất cả hiển thị
        </button>

        <button
          v-if="isSelectionMode && selectedUnits.size > 0"
          @click="clearSelection"
          class="action-button"
          style="background-color: #95a5a6; border-color: #7f8c8d"
        >
          Bỏ chọn tất cả
        </button>

        <button
          v-if="selectedUnits.size > 0"
          @click="confirmDeleteSelected"
          class="delete-btn"
          style="
            background-color: #e74c3c;
            color: white;
            padding: 8px 16px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
          "
        >
          🗑 Xóa đã chọn ({{ selectedUnits.size }})
        </button>
      </div>

      <!-- Hiển thị danh sách đơn vị đã chọn -->
      <div v-if="selectedUnits.size > 0" class="selected-units-display">
        <h4 style="margin: 0 0 12px 0; color: #2c3e50">Đơn vị đã chọn ({{ selectedUnits.size }}):</h4>
        <div
          style="
            max-height: 120px;
            overflow-y: auto;
            background: white;
            padding: 12px;
            border-radius: 4px;
            border: 1px solid #ddd;
          "
        >
          <div
            v-for="unitId in Array.from(selectedUnits)"
            :key="unitId"
            style="
              display: inline-block;
              background: #3498db;
              color: white;
              padding: 4px 8px;
              margin: 2px;
              border-radius: 4px;
              font-size: 0.85em;
            "
          >
            {{ getUnitDisplayName(unitId) }}
            <button
              @click="removeFromSelection(unitId)"
              style="background: none; border: none; color: white; margin-left: 6px; cursor: pointer; font-weight: bold"
            >
              ×
            </button>
          </div>
        </div>
      </div>
    </div>

    <button @click="openUnitModal" class="action-button" style="margin-bottom: 18px; margin-right: 12px">
      Nhập Đơn vị
    </button>
    <button @click="loadUnits" :disabled="unitStore.isLoading" class="action-button">
      {{ unitStore.isLoading ? 'Đang tải...' : 'Tải lại Danh sách Đơn vị' }}
    </button>

    <div v-if="formError || unitStore.error" class="error-message">
      <p>{{ (formError || unitStore.error) && getErrorMessage(formError || unitStore.error) }}</p>
    </div>

    <!-- Modal nhập đơn vị -->
    <!-- XÓA modal popup, trả lại form nhập đơn vị ở dưới cùng -->

    <div style="margin-bottom: 18px; display: flex; align-items: center; gap: 12px">
      <label for="viewMode" style="font-weight: bold; color: #34495e; min-width: 120px">Chế độ hiển thị:</label>
      <select
        id="viewMode"
        v-model="viewMode"
        style="min-width: 140px; padding: 6px 10px; border-radius: 4px; border: 1px solid #ced4da"
      >
        <option value="tree">Sơ đồ cây</option>
        <option value="grid">Dạng lưới</option>
      </select>
    </div>

    <div v-if="viewMode === 'tree'">
      <!-- Debug removed -->
      <ul
        v-if="unitStore.allUnits.length > 0 && !unitStore.isLoading"
        style="
          background: #fafdff;
          padding: 8px 0 8px 0;
          border: 1px solid #e0e0e0;
          border-radius: 6px;
          display: block;
          min-width: 0;
          overflow-x: visible;
        "
        class="tree-vertical"
      >
        <template v-for="branch in branches" :key="branch.Id">
          <li
            class="list-item branch-item branch-root tree-vertical-root"
            style="
              margin-bottom: 6px;
              min-height: 32px;
              font-size: 0.95em;
              max-width: none;
              white-space: normal;
              word-break: break-word;
              display: block;
              border-left: none;
              border-top: 5px solid #3498db;
              border-radius: 0;
              padding: 10px 12px;
            "
          >
            <!-- Hàng thông tin chính -->
            <div
              class="branch-main-info"
              style="display: flex; align-items: center; justify-content: space-between; margin-bottom: 8px"
            >
              <div class="unit-info" style="display: flex; align-items: center; gap: 8px; flex: 1">
                <!-- Checkbox cho chế độ chọn -->
                <input
                  v-if="isSelectionMode"
                  type="checkbox"
                  :checked="selectedUnits.has(branch.Id)"
                  @change="toggleUnitSelection(branch.Id)"
                  style="margin-right: 8px; transform: scale(1.2)"
                />
                <button
                  v-if="hasChildrenForBranch(branch.Id)"
                  @click="toggleNode(branch.Id)"
                  class="toggle-button-enhanced"
                  style="
                    background: #3498db;
                    border: none;
                    padding: 4px 8px;
                    cursor: pointer;
                    font-size: 1.1em;
                    color: white;
                    font-weight: bold;
                    margin-right: 6px;
                    border-radius: 50%;
                    transition: all 0.2s ease;
                    min-width: 28px;
                    min-height: 28px;
                    text-align: center;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    box-shadow: 0 2px 4px rgba(52, 152, 219, 0.3);
                  "
                  @mouseover="
                    $event.target.style.backgroundColor = '#2980b9';
                    $event.target.style.transform = 'scale(1.05)';
                  "
                  @mouseout="
                    $event.target.style.backgroundColor = '#3498db';
                    $event.target.style.transform = 'scale(1)';
                  "
                >
                  {{ expandedNodes.has(branch.Id) ? '−' : '+' }}
                </button>
                <span v-else :style="{ marginRight: isSelectionMode ? '6px' : '34px' }"></span>
                <span style="font-size: 1.1em">🏢</span>
                <strong style="font-size: 1.1em; color: #2c3e50">{{ branch.Name }}</strong>
              </div>
              <div class="actions" style="display: flex; gap: 8px; flex-shrink: 0">
                <button @click="startEditUnitWithModal(branch)" class="edit-btn">Sửa</button>
                <button @click="confirmDeleteUnit(branch.Id)" class="delete-btn">Xóa</button>
              </div>
            </div>
            <!-- Hàng thông tin chi tiết -->
            <div
              class="branch-details"
              style="
                display: flex;
                align-items: center;
                gap: 12px;
                margin-left: 34px;
                font-size: 0.9em;
                color: #7f8c8d;
                flex-wrap: wrap;
              "
            >
              <span style="background: #ecf0f1; padding: 2px 6px; border-radius: 3px">
                <strong>ID:</strong> {{ branch.Id }}
              </span>
              <span style="background: #ecf0f1; padding: 2px 6px; border-radius: 3px">
                <strong>Mã:</strong> {{ branch.code }}
              </span>
              <span style="background: #ecf0f1; padding: 2px 6px; border-radius: 3px">
                <strong>Loại:</strong> {{ branch.type || 'Chi nhánh' }}
              </span>
            </div>
            <TreeDepartments
              v-if="hasChildrenForBranch(branch.Id) && expandedNodes.has(branch.Id)"
              :parentId="branch.Id"
              :allUnits="unitStore.allUnits"
              :level="0"
              :isSelectionMode="isSelectionMode"
              :selectedUnits="selectedUnits"
              :expandedNodes="expandedNodes"
              @editUnit="startEditUnitWithModal"
              @deleteUnit="confirmDeleteUnit"
              @toggleSelection="toggleUnitSelection"
              @toggleNode="toggleNode"
            />
          </li>
        </template>
      </ul>
      <p v-else-if="!unitStore.isLoading && !unitStore.error && !formError">Không có đơn vị nào để hiển thị.</p>
      <p v-if="unitStore.isLoading && unitStore.allUnits.length === 0">Đang tải danh sách đơn vị...</p>
    </div>
    <div v-else-if="viewMode === 'grid'">
      <table style="width: 100%; border-collapse: collapse; background: #fafdff; border-radius: 6px; overflow: hidden">
        <thead>
          <tr style="background: #eaf6ff">
            <th v-if="isSelectionMode" style="padding: 10px; border-bottom: 1px solid #e0e0e0; width: 50px">
              <input
                type="checkbox"
                :checked="isAllVisibleSelected"
                @change="toggleSelectAllVisible"
                style="transform: scale(1.2)"
              />
            </th>
            <th style="padding: 10px; border-bottom: 1px solid #e0e0e0">ID</th>
            <th style="padding: 10px; border-bottom: 1px solid #e0e0e0">Mã Đơn vị</th>
            <th style="padding: 10px; border-bottom: 1px solid #e0e0e0">Tên Đơn vị</th>
            <th style="padding: 10px; border-bottom: 1px solid #e0e0e0">Loại</th>
            <th style="padding: 10px; border-bottom: 1px solid #e0e0e0">ID Cha</th>
            <th style="padding: 10px; border-bottom: 1px solid #e0e0e0">Thao tác</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="unit in sortedAllUnits" :key="unit.Id">
            <td v-if="isSelectionMode" style="padding: 8px 10px; border-bottom: 1px solid #e0e0e0; text-align: center">
              <input
                type="checkbox"
                :checked="selectedUnits.has(unit.Id)"
                @change="toggleUnitSelection(unit.Id)"
                style="transform: scale(1.2)"
              />
            </td>
            <td style="padding: 8px 10px; border-bottom: 1px solid #e0e0e0">{{ unit.Id }}</td>
            <td style="padding: 8px 10px; border-bottom: 1px solid #e0e0e0">{{ unit.Code }}</td>
            <td style="padding: 8px 10px; border-bottom: 1px solid #e0e0e0">{{ unit.Name }}</td>
            <td style="padding: 8px 10px; border-bottom: 1px solid #e0e0e0">{{ unit.Type }}</td>
            <td style="padding: 8px 10px; border-bottom: 1px solid #e0e0e0">{{ unit.ParentUnitId ?? '-' }}</td>
            <td style="padding: 8px 10px; border-bottom: 1px solid #e0e0e0">
              <button @click="startEditUnitWithModal(unit)" class="edit-btn">Sửa</button>
              <button @click="confirmDeleteUnit(unit.Id)" class="delete-btn">Xóa</button>
            </td>
          </tr>
        </tbody>
      </table>
      <p v-if="unitStore.allUnits.length === 0 && !unitStore.isLoading">Không có đơn vị nào để hiển thị.</p>
      <p v-if="unitStore.isLoading && unitStore.allUnits.length === 0">Đang tải danh sách đơn vị...</p>
    </div>
    <p v-else-if="!unitStore.isLoading && !unitStore.error && !formError">Không có đơn vị nào để hiển thị.</p>
    <p v-if="unitStore.isLoading && unitStore.allUnits.length === 0">Đang tải danh sách đơn vị...</p>

    <hr class="separator" />

    <div class="form-container">
      <h2>{{ isEditing ? 'Cập nhật Đơn vị' : 'Thêm Đơn vị Mới' }}</h2>
      <form @submit.prevent="handleSubmitUnit">
        <div class="form-group">
          <label for="unitCode">Mã Đơn vị:</label>
          <input
            type="text"
            id="unitCode"
            :value="currentUnit.code"
            @input="currentUnit.code = $event.target.value"
            required
          />
        </div>
        <div class="form-group">
          <label for="unitName">Tên Đơn vị:</label>
          <input
            type="text"
            id="unitName"
            :value="currentUnit.name"
            @input="currentUnit.name = $event.target.value"
            required
          />
        </div>
        <div class="form-group">
          <label for="parentUnitId">Chi nhánh cha (ID):</label>
          <input
            type="text"
            id="parentUnitId"
            :value="formatNumber(currentUnit.parentUnitId || 0)"
            @input="e => handleParentUnitIdInput(e)"
            @blur="e => handleParentUnitIdBlur(e)"
            placeholder="Nhập ID chi nhánh cha (bỏ trống nếu là chi nhánh gốc)"
            min="1"
            style="min-width: 200px; padding: 8px 12px; border-radius: 4px; border: 1px solid #ced4da"
          />
          <!-- Hiển thị thông báo cho CNL2 -->
          <div v-if="currentUnit.type === 'CNL2'" class="helper-text">
            <p style="margin: 8px 0 4px 0; font-size: 0.9em; color: #6c757d">
              💡 <strong>Lưu ý:</strong> Nếu bỏ trống, hệ thống sẽ tự động gán vào CNL1 đầu tiên có sẵn.
            </p>
            <!-- Hiển thị danh sách CNL1 có sẵn -->
            <div v-if="availableCNL1Units.length > 0" style="margin-top: 8px">
              <p style="margin: 4px 0; font-size: 0.85em; color: #495057">
                <strong>CNL1 có sẵn:</strong>
              </p>
              <ul style="margin: 4px 0; padding-left: 20px; font-size: 0.85em; color: #6c757d">
                <li v-for="cnl1 in availableCNL1Units" :key="cnl1.id">
                  ID: {{ cnl1.id }} - {{ cnl1.name }} ({{ cnl1.code }})
                </li>
              </ul>
            </div>
            <!-- Cảnh báo nếu không có CNL1 -->
            <div v-else style="margin-top: 8px">
              <p style="margin: 4px 0; font-size: 0.85em; color: #dc3545">
                ⚠️ <strong>Chưa có CNL1 nào!</strong> Bạn cần tạo CNL1 trước khi tạo CNL2.
              </p>
            </div>
          </div>
        </div>
        <div class="form-group">
          <label for="unitType">Loại đơn vị:</label>
          <select
            id="unitType"
            v-model="currentUnit.type"
            required
            style="min-width: 200px; padding: 8px 12px; border-radius: 4px; border: 1px solid #ced4da"
          >
            <option value="">-- Chọn loại đơn vị --</option>
            <option value="CNL1">CNL1</option>
            <option value="CNL2">CNL2</option>
            <option value="PGDL1">PGDL1</option>
            <option value="PGDL2">PGDL2</option>
            <option value="PNVL1">PNVL1</option>
            <option value="PNVL2">PNVL2</option>
          </select>
        </div>
        <div class="form-actions">
          <button type="submit" :disabled="unitStore.isLoading" class="action-button">
            {{
              unitStore.isLoading
                ? isEditing
                  ? 'Đang cập nhật...'
                  : 'Đang thêm...'
                : isEditing
                  ? 'Lưu Thay Đổi'
                  : 'Thêm Đơn vị'
            }}
          </button>
          <button type="button" @click="cancelEdit" v-if="isEditing" class="cancel-btn action-button">Hủy</button>
        </div>
      </form>
    </div>

    <!-- XÓA form-container cũ ở dưới cùng (ẩn form nhập đơn vị cũ) -->
  </div>
</template>

<script setup>
import { computed, defineComponent, h, nextTick, onMounted, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useUnitStore } from '../stores/unitStore.js'
import { getId } from '../utils/casingSafeAccess.js'
import { useNumberInput } from '../utils/numberFormat'

const router = useRouter()

// 🔢 Initialize number input utility
const { handleInput, handleBlur, formatNumber, parseFormattedNumber } = useNumberInput({
  maxDecimalPlaces: 0,
  allowNegative: false,
})

const unitStore = useUnitStore()

const currentUnit = ref({
  id: null,
  code: '',
  name: '',
  type: '',
  parentUnitId: null,
})

const isEditing = ref(false)
const formError = ref(null)
const viewMode = ref('tree')
const isCNL2Checked = ref(false)
const parentType = ref('')
const parentUnitIdInput = ref('')
const parentCNL2IdInput = ref('')
const pgdNameInput = ref('')

// Biến cho chức năng chọn đơn vị
const isSelectionMode = ref(false)
const selectedUnits = ref(new Set())

// Computed property để lấy danh sách CNL1 có sẵn
const availableCNL1Units = computed(() => {
  if (!unitStore.allUnits || !Array.isArray(unitStore.allUnits)) {
    return []
  }
  return unitStore.allUnits.filter(u => {
    const type = (u.type || '').toUpperCase()
    return type === 'CNL1'
  })
})

// Computed property để kiểm tra nếu có thể tạo CNL2
const canCreateCNL2 = computed(() => {
  return availableCNL1Units.value.length > 0
})

// Computed properties cho chức năng chọn đơn vị
const isAllVisibleSelected = computed(() => {
  if (viewMode.value === 'grid') {
    return sortedAllUnits.value.length > 0 && sortedAllUnits.value.every(unit => selectedUnits.value.has(unit.Id))
  } else {
    return branches.value.length > 0 && branches.value.every(branch => selectedUnits.value.has(branch.Id))
  }
})

// Methods for unit selection functionality
const toggleSelectionMode = () => {
  isSelectionMode.value = !isSelectionMode.value
  if (!isSelectionMode.value) {
    // Clear selection when exiting selection mode
    clearSelection()
  }
}

const toggleUnitSelection = unitId => {
  if (selectedUnits.value.has(unitId)) {
    selectedUnits.value.delete(unitId)
  } else {
    selectedUnits.value.add(unitId)
  }
  // Trigger reactivity
  selectedUnits.value = new Set(selectedUnits.value)
}

const selectAllVisible = () => {
  if (viewMode.value === 'grid') {
    sortedAllUnits.value.forEach(unit => {
      selectedUnits.value.add(unit.Id)
    })
  } else {
    // In tree view, select all branches and their children
    branches.value.forEach(branch => {
      selectedUnits.value.add(branch.Id)
      // Also select all children recursively
      const addChildrenRecursively = parentId => {
        unitStore.allUnits
          .filter(u => u.ParentUnitId === parentId)
          .forEach(child => {
            selectedUnits.value.add(child.Id)
            addChildrenRecursively(child.Id)
          })
      }
      addChildrenRecursively(branch.Id)
    })
  }
  // Trigger reactivity
  selectedUnits.value = new Set(selectedUnits.value)
}

const clearSelection = () => {
  selectedUnits.value.clear()
  selectedUnits.value = new Set()
}

const removeFromSelection = unitId => {
  selectedUnits.value.delete(unitId)
  selectedUnits.value = new Set(selectedUnits.value)
}

const getUnitDisplayName = unitId => {
  if (!unitStore.allUnits || !Array.isArray(unitStore.allUnits)) {
    return `ID: ${unitId}`
  }
  const unit = unitStore.allUnits.find(u => u.id === unitId)
  return unit ? `${unit.name} (${unit.code})` : `ID: ${unitId}`
}

const confirmDeleteSelected = () => {
  if (selectedUnits.value.size === 0) {
    alert('Không có đơn vị nào được chọn để xóa!')
    return
  }

  const unitNames = Array.from(selectedUnits.value).map(id => getUnitDisplayName(id))
  const confirmMessage = `Bạn có chắc chắn muốn xóa ${selectedUnits.value.size} đơn vị sau không?\n\n${unitNames.join('\n')}`

  if (confirm(confirmMessage)) {
    deleteSelectedUnits()
  }
}

const deleteSelectedUnits = async () => {
  const unitsToDelete = Array.from(selectedUnits.value)
  let successCount = 0
  let failCount = 0
  const errors = []

  for (const unitId of unitsToDelete) {
    try {
      await unitStore.deleteUnit(unitId)
      successCount++
    } catch (error) {
      failCount++
      errors.push(`ID ${unitId}: ${error.message || 'Lỗi không xác định'}`)
      console.error(`Error deleting unit ${unitId}:`, error)
    }
  }

  // Clear selection after deletion attempt
  clearSelection()

  // Show results
  if (successCount > 0 && failCount === 0) {
    alert(`Xóa thành công ${successCount} đơn vị!`)
  } else if (successCount > 0 && failCount > 0) {
    alert(`Xóa thành công ${successCount} đơn vị, thất bại ${failCount} đơn vị.\n\nLỗi:\n${errors.join('\n')}`)
  } else {
    alert(`Xóa thất bại tất cả ${failCount} đơn vị.\n\nLỗi:\n${errors.join('\n')}`)
  }
}

const toggleSelectAllVisible = () => {
  if (isAllVisibleSelected.value) {
    clearSelection()
  } else {
    selectAllVisible()
  }
}

// Function để kiểm tra xem một branch có children không
const hasChildrenForBranch = branchId => {
  if (!unitStore.allUnits || !Array.isArray(unitStore.allUnits)) {
    return false
  }
  return unitStore.allUnits.some(unit => unit.ParentUnitId === branchId)
}

// Computed property để sort tất cả units theo ID cho grid view
const sortedAllUnits = computed(() => {
  if (!unitStore.allUnits || !Array.isArray(unitStore.allUnits)) {
    return []
  }
  return [...unitStore.allUnits].sort((a, b) => (a.Id || 0) - (b.Id || 0))
})

function loadUnits() {
  console.log('loadUnits called')
  formError.value = null
  unitStore.error = null
  console.log('About to call unitStore.fetchUnits()')
  unitStore.fetchUnits()
  console.log('unitStore.fetchUnits() called')
}

// Load data khi component được mount
onMounted(() => {
  console.log('UnitsView mounted, current units count:', unitStore.allUnits.length)
  console.log('Is loading:', unitStore.isLoading)
  console.log('Error:', unitStore.error)

  // Force load units mỗi khi mount
  loadUnits()
})

// Hàm mở modal/form nhập đơn vị với auto-focus
const openUnitModal = async () => {
  // Reset form về trạng thái tạo mới
  isEditing.value = false
  currentUnit.value = {
    id: null,
    code: '',
    name: '',
    type: '',
    parentUnitId: null,
  }
  formError.value = null
  unitStore.error = null

  // Đợi DOM update
  await nextTick()

  // Scroll xuống form với smooth animation
  const formContainer = document.querySelector('.form-container')
  if (formContainer) {
    formContainer.scrollIntoView({
      behavior: 'smooth',
      block: 'start',
    })

    // Đợi animation scroll hoàn thành rồi focus
    setTimeout(() => {
      const firstInput = document.getElementById('unitCode')
      if (firstInput) {
        firstInput.focus()
        firstInput.select() // Select all text for better UX
      }
    }, 500) // Đợi 500ms để scroll animation hoàn thành
  }
}

// Hàm bắt đầu sửa đơn vị với auto-focus
const startEditUnitWithModal = async unit => {
  console.log('Bắt đầu sửa đơn vị:', unit)

  isEditing.value = true
  currentUnit.value = {
    id: unit.id,
    code: unit.code || '',
    name: unit.name || '',
    type: unit.type || '',
    parentUnitId: unit.parentUnitId || null,
  }
  formError.value = null
  unitStore.error = null

  // Đợi DOM update
  await nextTick()

  // Scroll xuống form với smooth animation
  const formContainer = document.querySelector('.form-container')
  if (formContainer) {
    formContainer.scrollIntoView({
      behavior: 'smooth',
      block: 'start',
    })

    // Đợi animation scroll hoàn thành rồi focus
    setTimeout(() => {
      const firstInput = document.getElementById('unitCode')
      if (firstInput) {
        firstInput.focus()
        firstInput.select() // Select all text for better UX
      }
    }, 500) // Đợi 500ms để scroll animation hoàn thành
  }
}

const handleSubmitUnit = async () => {
  formError.value = null
  unitStore.error = null

  let codeToSubmit = typeof currentUnit.value.code === 'string' ? currentUnit.value.code.trim() : ''
  let nameToSubmit = typeof currentUnit.value.name === 'string' ? currentUnit.value.name.trim() : ''
  let parentIdToSubmit =
    currentUnit.value.parentUnitId !== null && currentUnit.value.parentUnitId !== ''
      ? Number(currentUnit.value.parentUnitId)
      : null
  let typeToSubmit = typeof currentUnit.value.type === 'string' ? currentUnit.value.type.trim() : ''

  if (!codeToSubmit) {
    formError.value = 'Mã đơn vị không được để trống!'
    return
  }
  if (!nameToSubmit) {
    formError.value = 'Tên đơn vị không được để trống!'
    return
  }
  if (!typeToSubmit) {
    formError.value = 'Loại đơn vị không được để trống!'
    return
  }

  const unitDataForSubmission = {
    id: isEditing.value ? currentUnit.value.id : undefined,
    code: codeToSubmit,
    name: nameToSubmit,
    parentUnitId: parentIdToSubmit,
    type: typeToSubmit,
  }

  console.log('--- Bắt đầu handleSubmitUnit (Đơn vị) ---')
  console.log('Chế độ sửa:', isEditing.value)
  console.log('Giá trị currentUnit.value (gốc từ form):', JSON.parse(JSON.stringify(currentUnit.value)))
  console.log(
    'Dữ liệu sẽ được kiểm tra và gửi đi (unitDataForSubmission):',
    JSON.parse(JSON.stringify(unitDataForSubmission))
  )
  // Log typeof từng trường để debug lỗi backend
  Object.entries(unitDataForSubmission).forEach(([k, v]) => {
    console.log(`typeof ${k}:`, typeof v, '| value:', v)
  })
  console.log(
    'Danh sách đơn vị hiện có:',
    unitStore.allUnits.map(u => ({ id: u.id, code: u.code, type: u.type, parentUnitId: u.parentUnitId }))
  )

  if (!unitDataForSubmission.code) {
    formError.value = 'Mã đơn vị không được để trống!'
    console.log('VALIDATION FAIL (Client-side): Mã đơn vị trống.')
    return
  }
  if (!unitDataForSubmission.name) {
    formError.value = 'Tên đơn vị không được để trống!'
    console.log('VALIDATION FAIL (Client-side): Tên đơn vị trống.')
    return
  }
  if (!unitDataForSubmission.type) {
    formError.value = 'Loại đơn vị không được để trống!'
    console.log('VALIDATION FAIL (Client-side): Loại đơn vị trống.')
    return
  }

  // Validation đặc biệt cho CNL2
  if (typeToSubmit === 'CNL2') {
    // Kiểm tra xem có CNL1 nào tồn tại không
    if (availableCNL1Units.value.length === 0) {
      formError.value = 'Không thể tạo CNL2 vì chưa có CNL1 nào! Vui lòng tạo CNL1 trước.'
      return
    }

    // Nếu người dùng nhập parentUnitId, kiểm tra xem có phải CNL1 không
    if (parentIdToSubmit) {
      if (!unitStore.allUnits || !Array.isArray(unitStore.allUnits)) {
        formError.value = 'Dữ liệu đơn vị chưa được tải!'
        return
      }
      const parent = unitStore.allUnits.find(u => u.id === parentIdToSubmit)
      if (!parent || (parent.type || '').toUpperCase() !== 'CNL1') {
        formError.value = 'ID chi nhánh cha phải là ID của một đơn vị CNL1!'
        return
      }
    }
    // Nếu không nhập parentUnitId, backend sẽ tự động gán vào CNL1 đầu tiên
  }
  if (unitLevel.value === 3 && (typeToSubmit === 'PGD' || typeToSubmit === 'KH' || typeToSubmit === 'KTNQ')) {
    // Phòng nghiệp vụ/PGD chỉ được trực thuộc CNL2
    if (!unitStore.allUnits || !Array.isArray(unitStore.allUnits)) {
      formError.value = 'Dữ liệu đơn vị chưa được tải!'
      return
    }
    const parent = unitStore.allUnits.find(u => u.id === parentIdToSubmit)
    if (!parent || (parent.type || '').toUpperCase() !== 'CNL2') {
      formError.value = 'Phòng giao dịch, Phòng KH, Phòng KTNQ chỉ được trực thuộc CNL2!'
      return
    }
  }

  console.log('VALIDATION PASS (Client-side): Dữ liệu hợp lệ, tiến hành submit.')

  if (isEditing.value && unitDataForSubmission.id !== null && getId(unitDataForSubmission) !== null) {
    try {
      await unitStore.updateUnit(unitDataForSubmission)
      alert('Cập nhật đơn vị thành công!')
      cancelEdit()
    } catch (error) {
      console.error('Lỗi khi cập nhật đơn vị:', error)
    }
  } else {
    try {
      // eslint-disable-next-line no-unused-vars
      const { id, ...newUnitData } = unitDataForSubmission
      await unitStore.createUnit(newUnitData)
      alert('Thêm đơn vị thành công!')
      resetForm()
    } catch (error) {
      // Log chi tiết lỗi backend nếu có
      if (error && error.response) {
        console.error('Lỗi khi thêm đơn vị (backend):', error.response.data || error.response)
        formError.value =
          error.response.data?.message || error.response.data || error.message || 'Lỗi không xác định từ backend.'
      } else {
        console.error('Lỗi khi thêm đơn vị:', error)
        formError.value = error.message || 'Lỗi không xác định.'
      }
    }
  }
}

const startEditUnit = unit => {
  formError.value = null
  unitStore.error = null
  isEditing.value = true
  currentUnit.value = JSON.parse(JSON.stringify(unit))
  currentUnit.value.parentUnitId = unit.parentUnitId === null ? null : Number(unit.parentUnitId)
  console.log('Dữ liệu được nạp vào form sửa (startEditUnit):', JSON.parse(JSON.stringify(currentUnit.value)))
}

const cancelEdit = () => {
  isEditing.value = false
  resetForm()
  formError.value = null
  unitStore.error = null
}

const resetForm = () => {
  currentUnit.value = {
    id: null,
    code: '',
    name: '',
    type: '',
    parentUnitId: null,
  }
  pgdNameInput.value = ''
  // Reset các biến liên quan nếu có
  isEditing.value = false
  isCNL2Checked.value = false
  parentType.value = ''
  parentUnitIdInput.value = ''
  parentCNL2IdInput.value = ''
}

const confirmDeleteUnit = async unitId => {
  formError.value = null
  unitStore.error = null
  if (confirm(`Bạn có chắc chắn muốn xóa đơn vị có ID: ${unitId} không?`)) {
    try {
      await unitStore.deleteUnit(unitId)
      alert('Xóa đơn vị thành công!')
    } catch (error) {
      console.error('Lỗi khi xóa đơn vị:', error)
    }
  }
}

// Computed property cho các nhánh gốc (CNL1)
const branches = computed(() => {
  if (!unitStore.allUnits || !Array.isArray(unitStore.allUnits)) {
    return []
  }
  // Dựa vào dữ liệu thực tế, units chỉ có Id, Code, Name - không có Type hay ParentUnitId
  // Hiển thị tất cả units và sắp xếp theo Id
  return [...unitStore.allUnits].sort((a, b) => (a.Id || 0) - (b.Id || 0))
})

// Computed property cho các phòng ban theo từng nhánh
const departmentsByBranch = computed(() => {
  if (!unitStore.allUnits || !Array.isArray(unitStore.allUnits)) {
    return {}
  }
  const map = {}
  unitStore.allUnits.forEach(u => {
    if (u.ParentUnitId) {
      // ✅ Sử dụng u.ParentUnitId thay vì u.parentUnitId
      if (!map[u.ParentUnitId]) map[u.ParentUnitId] = []
      map[u.ParentUnitId].push(u)
    }
  })
  // Sort từng nhóm department theo ID
  Object.keys(map).forEach(key => {
    map[key].sort((a, b) => (a.id || 0) - (b.id || 0))
  })
  return map
})

// Function xử lý khi thay đổi loại chi nhánh cha
function onCNLTypeChange(type) {
  parentType.value = type
  if (type === 'CNL1') {
    // Tự động chọn chi nhánh cha là Chi nhánh Lai Châu (giả sử code là 'CNL1' hoặc tên tương ứng)
    const cnl1 = unitStore.allUnits.find(u => (u.type || '').toUpperCase().includes('CNL1'))
    currentUnit.value.parentUnitId = cnl1 ? cnl1.id : null
  } else if (type === 'CNL2') {
    // Nếu đang tạo mới CNL2 (tích vào "Có phải là CNL2?"), sau khi lưu sẽ cho phép chọn chính CNL2 đó làm chi nhánh cha
    // Nếu đang tạo mới (chưa có id), tạm thời không chọn gì
    if (isCNL2Checked.value && currentUnit.value.id) {
      currentUnit.value.parentUnitId = currentUnit.value.id
    } else {
      currentUnit.value.parentUnitId = null
    }
  }
  // Reset phòng nghiệp vụ khi đổi loại chi nhánh cha
  currentUnit.value.type = ''
}

// Computed property để xác định cấp độ của đơn vị (CNL1, CNL2, PGD, ...)
const unitLevel = computed(() => {
  // Nếu là CNL1 (Chi nhánh Lai Châu)
  if (
    !currentUnit.value.parentUnitId &&
    (currentUnit.value.type === '' || (currentUnit.value.type || '').toUpperCase() === 'CNL1')
  ) {
    return 1
  }
  // Nếu là CNL2 hoặc phòng nghiệp vụ cấp 2 (trực thuộc CNL1)
  const parent = unitStore.allUnits.find(u => u.id === Number(currentUnit.value.parentUnitId))
  if (parent && (parent.type || '').toUpperCase() === 'CNL1') {
    if ((currentUnit.value.type || '').toUpperCase() === 'CNL2') return 2
    return 2
  }
  // Nếu là phòng nghiệp vụ/PGD trực thuộc CNL2
  if (parent && (parent.type || '').toUpperCase() === 'CNL2') {
    return 3
  }
  return 0
})

// Computed property để lấy danh sách các tùy chọn chi nhánh cha
const parentBranchOptions = computed(() => {
  if (!unitStore.allUnits || !Array.isArray(unitStore.allUnits)) {
    return []
  }
  if (unitLevel.value === 1) {
    // CNL1 không có cha
    return []
  }
  if (unitLevel.value === 2) {
    // CNL2 hoặc phòng nghiệp vụ cấp 2 chỉ chọn được CNL1 làm cha
    return unitStore.allUnits.filter(u => (u.type || '').toUpperCase() === 'CNL1')
  }
  if (unitLevel.value === 3) {
    // Phòng nghiệp vụ/PGD chỉ chọn được CNL2 làm cha
    return unitStore.allUnits.filter(u => (u.type || '').toUpperCase() === 'CNL2')
  }
  return []
})

// Computed property để lấy danh sách các phòng nghiệp vụ theo loại đơn vị
const departmentOptions = computed(() => {
  if (!unitStore.allUnits || !Array.isArray(unitStore.allUnits)) {
    return []
  }
  if (!currentUnit.value.parentUnitId) return []
  const parent = unitStore.allUnits.find(u => u.id === Number(currentUnit.value.parentUnitId))
  if (!parent) return []
  const type = (parent.type || '').toUpperCase()

  if (type === 'CNL1') {
    // Lấy các phòng nghiệp vụ PNVL1 thực tế từ database
    const pnvl1Units = unitStore.allUnits.filter(
      u => u.parentUnitId === parent.id && (u.type || '').toUpperCase() === 'PNVL1',
    )
    return pnvl1Units.map(u => ({
      value: u.code,
      label: u.name,
    }))
  } else if (type === 'CNL2') {
    // Nếu là CNL2 thì trả về các loại phòng nghiệp vụ chuẩn
    return [
      { value: 'PhongKhachHang', label: 'Phòng Khách hàng' },
      { value: 'PhongKtnq', label: 'Phòng Kế toán & Ngân quỹ' },
      { value: 'PGD', label: 'Phòng giao dịch (PGD) - Nhập tên riêng' },
    ]
  }
  return []
})

// Theo dõi sự thay đổi của checkbox "Có phải là CNL2?"
watch(isCNL2Checked, val => {
  if (val) {
    // Nếu chọn là CNL2 thì reset trường phòng nghiệp vụ
    currentUnit.value.type = ''
  }
})

// Computed property để xác định xem trường phòng nghiệp vụ có bắt buộc hay không
const isDepartmentRequired = computed(() => {
  // Nếu chọn là CNL2 thì không required phòng nghiệp vụ
  if (isCNL2Checked.value) return false
  // Nếu không chọn là CNL2 thì required phòng nghiệp vụ
  return true
})

// State để quản lý expand/collapse cho toàn bộ tree
const expandedNodes = ref(new Set())

// Toggle function để expand/collapse nodes
const toggleNode = nodeId => {
  if (expandedNodes.value.has(nodeId)) {
    expandedNodes.value.delete(nodeId)
  } else {
    expandedNodes.value.add(nodeId)
  }
  // Trigger reactivity
  expandedNodes.value = new Set(expandedNodes.value)
}

// Định nghĩa component TreeDepartments ngay trong block này
const TreeDepartments = defineComponent({
  name: 'TreeDepartments',
  props: {
    parentId: { type: Number, required: true },
    allUnits: { type: Array, required: true, default: () => [] },
    level: { type: Number, default: 0 },
    isSelectionMode: { type: Boolean, default: false },
    selectedUnits: { type: Set, default: () => new Set() },
    expandedNodes: { type: Set, default: () => new Set() },
  },
  emits: ['editUnit', 'deleteUnit', 'toggleSelection', 'toggleNode'],
  setup(props, { emit }) {
    // Bảo vệ nếu props không hợp lệ
    const safeParentId = computed(() => (typeof props.parentId === 'number' ? props.parentId : 0))
    const safeAllUnits = computed(() => (Array.isArray(props.allUnits) ? props.allUnits : []))
    const children = computed(() =>
      safeAllUnits.value.filter(u => u.ParentUnitId === safeParentId.value).sort((a, b) => (a.Id || 0) - (b.Id || 0))
    )

    // Check if a node has children
    const hasChildren = unitId => {
      return safeAllUnits.value.some(u => u.ParentUnitId === unitId)
    }

    // Check if a node is expanded
    const isExpanded = unitId => {
      return props.expandedNodes.has(unitId)
    }

    return () => {
      if (!children.value.length) return null
      return h(
        'ul',
        {
          class: 'department-list',
          style: 'margin: 2px 0 0 18px; padding-left: 0; transition: all 0.3s ease;',
        },
        children.value.map(dept => {
          const hasChildNodes = hasChildren(dept.Id)
          const isNodeExpanded = isExpanded(dept.Id)
          const isLeafNode = !hasChildNodes

          return h(
            'li',
            {
              class: 'list-item department-item tree-node',
              style:
                'margin-bottom: 4px; min-height: 32px; font-size: 0.92em; display: block; padding: 8px 12px; border-radius: 0 4px 4px 0;',
              key: dept.Id,
            },
            [
              // Hàng thông tin chính
              h(
                'div',
                {
                  class: 'department-main-info',
                  style: 'display: flex; align-items: center; justify-content: space-between; margin-bottom: 6px;',
                },
                [
                  h(
                    'div',
                    {
                      class: 'unit-info',
                      style: 'display: flex; align-items: center; gap: 6px; flex: 1;',
                    },
                    [
                      // Checkbox cho chế độ chọn
                      props.isSelectionMode
                        ? h('input', {
                            type: 'checkbox',
                            checked: props.selectedUnits.has(dept.Id),
                            onChange: () => emit('toggleSelection', dept.Id),
                            style: 'margin-right: 6px; transform: scale(1.1);',
                          })
                        : null,
                      // Toggle button for nodes with children
                      hasChildNodes
                        ? h(
                            'button',
                            {
                              class: 'toggle-button-enhanced',
                              style: `
                    background: #27ae60;
                    border: none;
                    padding: 3px 7px;
                    cursor: pointer;
                    font-size: 1em;
                    color: white;
                    font-weight: bold;
                    margin-right: 4px;
                    border-radius: 50%;
                    transition: all 0.2s ease;
                    min-width: 24px;
                    min-height: 24px;
                    text-align: center;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    box-shadow: 0 2px 4px rgba(39, 174, 96, 0.3);
                  `,
                              onClick: () => emit('toggleNode', dept.Id),
                              onMouseover: e => {
                                e.target.style.backgroundColor = '#229954'
                                e.target.style.transform = 'scale(1.05)'
                              },
                              onMouseout: e => {
                                e.target.style.backgroundColor = '#27ae60'
                                e.target.style.transform = 'scale(1)'
                              },
                            },
                            isNodeExpanded ? '−' : '+'
                          )
                        : h(
                            'span',
                            {
                              style: 'font-size: 0.9em; margin-right: 30px; color: #bdc3c7;',
                            },
                            isLeafNode ? '└─' : '├─'
                          ),
                      h('span', { style: 'font-size: 0.9em;' }, dept.type === 'CNL2' ? '🏢' : '🏬'),
                      h('strong', { style: 'font-size: 1em; color: #2c3e50;' }, dept.Name),
                    ]
                  ),
                  h(
                    'div',
                    {
                      class: 'actions',
                      style: 'display: flex; gap: 6px; flex-shrink: 0;',
                    },
                    [
                      h('button', { class: 'edit-btn', onClick: () => emit('editUnit', dept) }, 'Sửa'),
                      h('button', { class: 'delete-btn', onClick: () => emit('deleteUnit', dept.Id) }, 'Xóa'),
                    ]
                  ),
                ]
              ),
              // Hàng thông tin chi tiết
              h(
                'div',
                {
                  class: 'department-details',
                  style:
                    'display: flex; align-items: center; gap: 10px; margin-left: 30px; font-size: 0.85em; color: #7f8c8d; flex-wrap: wrap;',
                },
                [
                  h(
                    'span',
                    {
                      style: 'background: #f1f2f6; padding: 2px 5px; border-radius: 3px;',
                    },
                    `ID: ${dept.Id}`
                  ),
                  h(
                    'span',
                    {
                      style: 'background: #f1f2f6; padding: 2px 5px; border-radius: 3px;',
                    },
                    `Mã: ${dept.Code}`
                  ),
                  h(
                    'span',
                    {
                      style: 'background: #f1f2f6; padding: 2px 5px; border-radius: 3px;',
                    },
                    `Loại: ${dept.Type || 'Phòng nghiệp vụ'}`
                  ),
                ]
              ),
              // Only render children if node is expanded and has children
              hasChildNodes && isNodeExpanded
                ? h(TreeDepartments, {
                    parentId: dept.Id,
                    allUnits: safeAllUnits.value,
                    level: props.level + 1,
                    isSelectionMode: props.isSelectionMode,
                    selectedUnits: props.selectedUnits,
                    expandedNodes: props.expandedNodes,
                    onEditUnit: unit => emit('editUnit', unit),
                    onDeleteUnit: id => emit('deleteUnit', id),
                    onToggleSelection: id => emit('toggleSelection', id),
                    onToggleNode: id => emit('toggleNode', id),
                  })
                : null,
            ]
          )
        })
      )
    }
  },
})
function getErrorMessage(msg) {
  if (!msg) return ''
  // Một số lỗi phổ biến dịch sang tiếng Việt
  if (typeof msg === 'string') {
    if (msg.includes('Request failed with status code 400')) return 'Yêu cầu không hợp lệ hoặc dữ liệu gửi lên bị sai.'
    if (msg.includes('Network Error')) return 'Không thể kết nối tới máy chủ. Vui lòng kiểm tra mạng hoặc thử lại sau.'
    if (msg.includes('not found')) return 'Không tìm thấy dữ liệu.'
    if (msg.includes('already exists')) return 'Dữ liệu đã tồn tại.'
    if (msg.includes('required')) return 'Vui lòng nhập đầy đủ thông tin bắt buộc.'
    if (msg.includes('Unauthorized')) return 'Bạn không có quyền thực hiện thao tác này.'
    if (msg.includes('Internal Server Error')) return 'Lỗi hệ thống. Vui lòng thử lại sau.'
    // Thêm các mẫu khác nếu cần
    return msg.replace('Bad Request', 'Yêu cầu không hợp lệ').replace('Error', 'Lỗi')
  }
  return msg
}
</script>

<style scoped>
/* Phần CSS giữ nguyên như Sếp đã yêu cầu ở các tin nhắn trước */
.units-view {
  max-width: 900px;
  margin: 20px auto;
  padding: 20px;
  /* 🇻🇳 Sử dụng font tiếng Việt tối ưu */
  font-family: var(--font-primary, 'Roboto', 'Segoe UI', 'Arial', sans-serif);
  color: #2c3e50;
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}
h1,
h2 {
  color: #34495e;
}
.error-message {
  color: white;
  background-color: #e74c3c;
  border: 1px solid #c0392b;
  padding: 12px 18px;
  margin-top: 15px;
  margin-bottom: 20px;
  border-radius: 5px;
  text-align: left;
}
.error-message p {
  margin: 0;
}
ul {
  list-style-type: none;
  padding: 0;
  margin-top: 20px;
}
.list-item {
  background-color: #ecf0f1;
  border: 1px solid #bdc3c7;
  padding: 12px 18px;
  margin-bottom: 12px;
  border-radius: 5px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  transition: background-color 0.2s ease-in-out;
}
.list-item:hover {
  background-color: #e0e6e8;
}
.item-info {
  flex-grow: 1;
  text-align: left;
}
.item-info strong {
  color: #2c3e50;
}
.item-details {
  font-size: 0.85em;
  color: #7f8c8d;
  margin-left: 10px;
}
/* CSS cho phần pre debug unit trong danh sách */
.item-info pre {
  font-size: 0.7em;
  background: #ffffff; /* Nền trắng để dễ đọc hơn */
  border: 1px dashed #ccc; /* Border đứt nét để phân biệt */
  padding: 5px;
  margin-top: 5px;
  white-space: pre-wrap; /* Giữ nguyên định dạng và xuống dòng */
  word-break: break-all; /* Ngắt từ nếu quá dài */
  max-height: 100px; /* Giới hạn chiều cao, thêm scroll nếu cần */
  overflow-y: auto; /* Thêm thanh cuộn dọc nếu nội dung dài */
}
.actions {
  display: flex;
  gap: 10px;
  flex-shrink: 0;
}
.action-button,
.edit-btn,
.delete-btn,
.cancel-btn {
  padding: 8px 15px;
  font-size: 0.9em;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  color: white;
  font-weight: 500;
  transition: background-color 0.2s ease;
}
.action-button:disabled,
.edit-btn:disabled,
.delete-btn:disabled,
.cancel-btn:disabled {
  background-color: #bdc3c7;
  cursor: not-allowed;
}
.edit-btn {
  background-color: #f39c12;
}
.edit-btn:hover:not(:disabled) {
  background-color: #e67e22;
}
.delete-btn {
  background-color: #e74c3c;
}
.delete-btn:hover:not(:disabled) {
  background-color: #c82333;
}
.cancel-btn {
  background-color: #95a5a6;
}
.cancel-btn:hover:not(:disabled) {
  background-color: #7f8c8d;
}
.separator {
  margin: 35px 0;
  border: 0;
  border-top: 1px solid #bdc3c7;
}
.form-container {
  background-color: #ffffff;
  padding: 25px;
  border-radius: 6px;
  border: 1px solid #dde0e3;
  box-shadow: 0 1px 5px rgba(0, 0, 0, 0.05);
}
.form-container h2 {
  margin-top: 0;
  margin-bottom: 25px;
  text-align: center;
  color: #34495e;
}
.form-group {
  margin-bottom: 18px;
  display: flex;
  flex-wrap: wrap;
  align-items: center;
}
.form-group label {
  flex-basis: 160px;
  margin-right: 15px;
  text-align: right;
  font-weight: bold;
  color: #34495e;
}
.form-group input[type='text'],
.form-group input[type='number'] {
  flex-grow: 1;
  min-width: 200px;
  padding: 10px 12px;
  border: 1px solid #ced4da;
  border-radius: 4px;
  box-sizing: border-box;
  transition:
    border-color 0.2s ease,
    box-shadow 0.2s ease;
}
.form-group input[type='text']:focus,
.form-group input[type='number']:focus {
  border-color: #80bdff;
  outline: 0;
  box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
}
.form-actions {
  text-align: center;
  margin-top: 25px;
}
.form-actions .action-button {
  margin-right: 12px;
}
.form-actions .action-button:last-child {
  margin-right: 0;
}
button.action-button {
  background-color: #007bff;
}
button.action-button:hover:not(:disabled) {
  background-color: #0056b3;
}
.branch-item {
  background-color: #eaf6ff;
  border-left: 5px solid #3498db;
}
.tree-vertical {
  padding-left: 0;
  margin-left: 0;
}
.tree-vertical-root {
  border-left: none !important;
  border-top: 5px solid #3498db !important;
  margin-left: 0 !important;
  padding-left: 0 !important;
}
.department-list {
  margin: 0 0 0 32px;
  padding: 0;
  border-left: 2px solid #7ed6df;
  border-top: none;
  position: relative;
  transition: all 0.3s ease;
  overflow: hidden;
}

/* Animation for expanding/collapsing */
.department-list.expanding {
  animation: expandList 0.3s ease-out;
}

.department-list.collapsing {
  animation: collapseList 0.3s ease-out;
}

@keyframes expandList {
  from {
    max-height: 0;
    opacity: 0.5;
  }
  to {
    max-height: 1000px;
    opacity: 1;
  }
}

@keyframes collapseList {
  from {
    max-height: 1000px;
    opacity: 1;
  }
  to {
    max-height: 0;
    opacity: 0.5;
  }
}

/* CSS cho modal */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(44, 62, 80, 0.35);
  z-index: 1000;
  display: flex;
  align-items: flex-start;
  justify-content: center;
  overflow-y: auto;
}
.modal-content {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 4px 32px rgba(0, 0, 0, 0.18);
  margin-top: 60px;
  padding: 32px 32px 24px 32px;
  min-width: 380px;
  max-width: 98vw;
  min-height: 0;
  position: relative;
  animation: modalFadeIn 0.18s;
}
@keyframes modalFadeIn {
  from {
    opacity: 0;
    transform: translateY(-30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
@media (max-width: 600px) {
  .modal-content {
    min-width: 90vw;
    padding: 18px 6vw;
  }
}

/* CSS cho helper text trong form */
.helper-text {
  background-color: #f8f9fa;
  border: 1px solid #e9ecef;
  border-radius: 4px;
  padding: 12px;
  margin-top: 8px;
}

.helper-text p {
  margin: 0;
  line-height: 1.4;
}

.helper-text ul {
  margin: 4px 0;
  padding-left: 20px;
}

.helper-text li {
  margin-bottom: 2px;
}

/* CSS cho expand/collapse tree functionality */
.tree-node {
  transition: all 0.3s ease;
}

.toggle-button {
  background: none !important;
  border: none !important;
  padding: 2px 6px !important;
  cursor: pointer !important;
  font-size: 0.8em !important;
  color: #3498db !important;
  font-weight: bold !important;
  margin-right: 2px !important;
  border-radius: 3px !important;
  transition: all 0.2s ease !important;
  min-width: 20px !important;
  text-align: center !important;
}

.toggle-button:hover {
  background-color: #ecf0f1 !important;
  color: #2980b9 !important;
}

.toggle-button:active {
  background-color: #d5dbdb !important;
}

/* Enhanced toggle buttons for better visibility */
.toggle-button-enhanced {
  background: #3498db !important;
  border: none !important;
  padding: 4px 8px !important;
  cursor: pointer !important;
  font-size: 1.1em !important;
  color: white !important;
  font-weight: bold !important;
  margin-right: 6px !important;
  border-radius: 50% !important;
  transition: all 0.2s ease !important;
  min-width: 28px !important;
  min-height: 28px !important;
  text-align: center !important;
  display: flex !important;
  align-items: center !important;
  justify-content: center !important;
  box-shadow: 0 2px 4px rgba(52, 152, 219, 0.3) !important;
}

.toggle-button-enhanced:hover {
  background-color: #2980b9 !important;
  transform: scale(1.05) !important;
  box-shadow: 0 3px 6px rgba(52, 152, 219, 0.4) !important;
}

.toggle-button-enhanced:active {
  transform: scale(0.95) !important;
  box-shadow: 0 1px 2px rgba(52, 152, 219, 0.3) !important;
}

/* For department level toggle buttons - use green color */
.department-item .toggle-button-enhanced {
  background: #27ae60 !important;
  min-width: 24px !important;
  min-height: 24px !important;
  font-size: 1em !important;
  box-shadow: 0 2px 4px rgba(39, 174, 96, 0.3) !important;
}

.department-item .toggle-button-enhanced:hover {
  background-color: #229954 !important;
  box-shadow: 0 3px 6px rgba(39, 174, 96, 0.4) !important;
}

.department-item .toggle-button-enhanced:active {
  box-shadow: 0 1px 2px rgba(39, 174, 96, 0.3) !important;
}
</style>
