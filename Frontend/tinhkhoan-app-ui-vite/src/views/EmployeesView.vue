next steps (for full DB removal of “Mã NV”)
Plan and apply an EF Core migration to drop EmployeeCode and related constraints/usages.
Update models/DTOs/controllers/tests to be CBCode-first.
Verify all downstream analytics/scripts that reference EmployeeCode.
Roll out with a DB backup and clear release notes.<template>
  <div class="employees-view">
    <h1>Quản lý Nhân viên</h1>
    <div style="display: flex; gap: 10px; margin-bottom: 20px">
      <button :disabled="isOverallLoading" class="action-button" @click="loadInitialData">
        {{ isOverallLoading ? 'Đang tải dữ liệu...' : 'Tải lại Danh sách Nhân viên' }}
      </button>
      <button class="action-button add-employee-btn" style="background-color: #28a745" @click="scrollToAddEmployeeForm">
        + Thêm nhân viên
      </button>
      <button
        class="action-button"
        style="background-color:#8e44ad"
        @click="triggerFileSelect"
      >
        Import Excel
      </button>
      <button
        class="action-button"
        style="background-color:#16a085"
        @click="downloadTemplate"
      >
        Tải mẫu Excel
      </button>
      <input
        ref="fileInputRef"
        type="file"
        accept=".xls,.xlsx"
        style="display:none"
        @change="handleFileChange"
      >

      <!-- Các nút cho tính năng chọn nhiều -->
      <template v-if="pagedEmployees.length > 0">
        <button class="action-button" style="background-color: #6c757d" @click="toggleSelectAll">
          {{ isAllSelected ? 'Bỏ chọn tất cả' : 'Chọn tất cả' }}
        </button>

        <button
          v-if="selectedEmployeeIds.length > 0"
          class="action-button"
          style="background-color: #dc3545; color: white"
          :disabled="isDeleting"
          @click="confirmDeleteSelected"
        >
          {{ isDeleting ? 'Đang xóa...' : `Xóa (${selectedEmployeeIds.length}) nhân viên đã chọn` }}
        </button>
      </template>
    </div>

    <!-- Hiển thị số lượng nhân viên đã chọn -->
    <div v-if="selectedEmployeeIds.length > 0" class="selection-info">
      <p style="color: #007bff; font-weight: bold; margin: 10px 0">
        Đã chọn {{ selectedEmployeeIds.length }} nhân viên
      </p>
    </div>

    <div v-if="displayError" class="error-message">
      <p>{{ displayError }}</p>
    </div>

    <!-- Debug removed -->

    <div style="display: flex; align-items: center; gap: 16px; margin-bottom: 12px">
      <label for="pageSize" style="font-weight: bold; color: #34495e">Số dòng/trang:</label>
      <select
        id="pageSize"
        v-model.number="pageSize"
        style="min-width: 80px; padding: 4px 8px; border-radius: 4px; border: 1px solid #ced4da"
      >
        <option :value="20">
          20
        </option>
        <option :value="50">
          50
        </option>
        <option :value="100">
          100
        </option>
      </select>
    </div>

    <template v-if="pagedEmployees.length > 0">
      <!-- Container có thể cuộn ngang cho bảng rộng -->
      <div class="table-container">
        <table class="employee-detail-table compact-table">
          <thead>
            <tr>
              <th style="width: 50px; min-width: 50px">
                <input
                  type="checkbox"
                  :checked="isAllSelected"
                  title="Chọn/Bỏ chọn tất cả"
                  @change="toggleSelectAll"
                >
              </th>
              <th style="width: 100px; min-width: 100px">
                Thao tác
              </th>
              <th style="width: 110px; min-width: 110px">
                Mã CB
              </th>
              <th style="width: 180px; min-width: 180px">
                Họ tên
              </th>
              <th style="width: 120px; min-width: 120px">
                Tên ĐN
              </th>
              <th style="width: 140px; min-width: 140px">
                User AD
              </th>
              <th style="width: 140px; min-width: 140px">
                User IPCAS
              </th>
              <th style="width: 120px; min-width: 120px">
                Mã CBTD
              </th>
              <th style="width: 150px; min-width: 150px">
                Chi nhánh
              </th>
              <th style="width: 150px; min-width: 150px">
                Phòng ban
              </th>
              <th style="width: 130px; min-width: 130px">
                Chức vụ
              </th>
              <th style="width: 200px; min-width: 200px">
                Email
              </th>
              <th style="width: 120px; min-width: 120px">
                SĐT
              </th>
              <!-- Bỏ cột Vai trò và Trạng thái theo yêu cầu -->
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="employee in pagedEmployees"
              :key="employee.Id"
              :class="{ 'selected-row': selectedEmployeeIds.includes(employee.Id) }"
            >
              <td class="checkbox-cell">
                <input
                  v-model="selectedEmployeeIds"
                  type="checkbox"
                  :value="employee.Id"
                  :disabled="employee.Username === 'admin'"
                  :title="employee.Username === 'admin' ? 'Không thể chọn tài khoản admin' : 'Chọn nhân viên này'"
                >
              </td>
              <td class="action-cell">
                <button class="edit-btn" @click="startEditEmployee(employee)">
                  Sửa
                </button>
                <button
                  class="delete-btn"
                  :disabled="employee.Username === 'admin'"
                  :title="employee.Username === 'admin' ? 'Không thể xóa tài khoản admin' : 'Xóa nhân viên'"
                  @click="confirmDeleteEmployee(employee.Id)"
                >
                  Xóa
                </button>
              </td>
              <td>{{ employee.CBCode || 'Chưa có mã CB' }}</td>
              <td title="{{ employee.FullName }}">
                {{ employee.FullName }}
              </td>
              <td title="{{ employee.Username }}">
                {{ employee.Username }}
              </td>
              <td title="{{ employee.UserAD || employee.userAd || '—' }}">
                {{ employee.UserAD || employee.userAd || '—' }}
              </td>
              <td title="{{ employee.UserIPCAS || employee.userIpcas || '—' }}">
                {{ employee.UserIPCAS || employee.userIpcas || '—' }}
              </td>
              <td title="{{ employee.MaCBTD || employee.maCbtd || '—' }}">
                {{ employee.MaCBTD || employee.maCbtd || '—' }}
              </td>
              <td
                title="{{
                unitStore.allUnits.find(
                  u => u.Id === unitStore.allUnits.find(x => x.Id === employee.UnitId)?.ParentUnitId
                )?.Name || 'N/A'
              }}"
              >
                {{
                  unitStore.allUnits.find(
                    u => u.Id === unitStore.allUnits.find(x => x.Id === employee.UnitId)?.ParentUnitId
                  )?.Name || 'N/A'
                }}
              </td>
              <td title="{{ unitStore.allUnits.find(u => u.Id === employee.UnitId)?.Name || 'N/A' }}">
                {{ unitStore.allUnits.find(u => u.Id === employee.UnitId)?.Name || 'N/A' }}
              </td>
              <td
                title="{{
                employee.PositionName ||
                  positionStore.allPositions.find(p => p.Id === employee.PositionId)?.Name ||
                  'N/A'
              }}"
              >
                {{
                  employee.PositionName ||
                    positionStore.allPositions.find(p => p.Id === employee.PositionId)?.Name ||
                    'N/A'
                }}
              </td>
              <!-- Vai trò đã bỏ khỏi bảng nhân viên -->
              <td title="{{ employee.Email || '' }}">
                {{ employee.Email || '—' }}
              </td>
              <td title="{{ employee.PhoneNumber || '' }}">
                {{ employee.PhoneNumber || '—' }}
              </td>
            <!-- Bỏ hiển thị trạng thái -->
            </tr>
          </tbody>
        </table>
      </div>
    </template>
    <template v-else>
      <p v-if="employeeStore.isLoading">
        Đang tải danh sách nhân viên...
      </p>
      <p v-else-if="displayError">
        {{ displayError }}
      </p>
      <p v-else>
        Không có nhân viên nào để hiển thị.
      </p>
    </template>
    <div
      v-if="totalPages > 1"
      style="margin: 12px 0; display: flex; align-items: center; gap: 12px; justify-content: flex-end"
    >
      <button
        :disabled="page === 1"
        class="action-button"
        style="padding: 4px 10px"
        @click="prevPage"
      >
        &lt;
      </button>
      <span>Trang {{ page }} / {{ totalPages }}</span>
      <button
        :disabled="page === totalPages"
        class="action-button"
        style="padding: 4px 10px"
        @click="nextPage"
      >
        &gt;
      </button>
    </div>

    <hr class="separator">

    <!-- Import Preview Section -->
    <div v-if="importPreview.length > 0 || importErrors.length > 0" class="form-container" style="border:2px dashed #8e44ad; background:#faf6ff">
      <h2>Preview Import</h2>
      <div v-if="importStats.total > 0" class="import-stats" style="margin-bottom: 15px; padding: 10px; background: #f8f9fa; border-radius: 5px;">
        <div style="font-weight: bold; margin-bottom: 5px;">
          📊 Thống kê dữ liệu:
        </div>
        <div style="display: grid; grid-template-columns: 1fr 1fr 1fr 1fr; gap: 10px; font-size: 14px;">
          <div>Tổng số: <strong>{{ importStats.total }}</strong></div>
          <div style="color: #27ae60;">
            Hợp lệ: <strong>{{ importStats.valid || importPreview.length }}</strong>
          </div>
          <div style="color: #e74c3c;">
            Lỗi: <strong>{{ importStats.invalid || importErrors.length }}</strong>
          </div>
          <div>Sẽ import: <strong>{{ importPreview.length }}</strong></div>
        </div>
        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 10px; margin-top: 8px; font-size: 14px;">
          <div style="color: #3498db;">
            Tạo mới: <strong>{{ importStats.new || 0 }}</strong>
          </div>
          <div style="color: #f39c12;">
            Cập nhật: <strong>{{ importStats.update || 0 }}</strong>
          </div>
        </div>
      </div>

      <div v-if="importErrors.length > 0" style="color:#c0392b; font-size:13px; margin-bottom:8px; max-height: 150px; overflow-y: auto; border: 1px solid #e74c3c; border-radius: 3px; padding: 8px; background: #fdf2f2;">
        <div style="font-weight: bold; margin-bottom: 5px;">
          ⚠️ Lỗi validation ({{ importErrors.length }} lỗi):
        </div>
        <div
          v-for="e in importErrors"
          :key="e"
          style="margin-bottom: 2px;"
        >
          • {{ e }}
        </div>
      </div>

      <div v-if="importPreview.length > 0" style="max-height:260px; overflow:auto; border:1px solid #ddd; background:#fff">
        <div style="font-weight: bold; padding: 8px; background: #f8f9fa; border-bottom: 1px solid #ddd;">
          📋 Dữ liệu sẽ được import ({{ importPreview.length }} records):
        </div>
        <table class="employee-detail-table compact-table">
          <thead>
            <tr>
              <th>Mã CB</th><th>Họ tên</th><th>Tên ĐN</th><th>User AD</th><th>Email</th><th>User IPCAS</th><th>Mã CBTD</th><th>SĐT</th><th>Trạng thái</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="r in importPreview.slice(0,50)" :key="r._row">
              <td>{{ r.cbCode }}</td>
              <td>{{ r.fullName }}</td>
              <td>{{ r.username }}</td>
              <td>{{ r.userAd }}</td>
              <td>{{ r.email }}</td>
              <td>{{ r.userIpcas }}</td>
              <td>{{ r.maCbtd }}</td>
              <td>{{ r.phoneNumber }}</td>
              <td>{{ r.isActive ? 'Hoạt động' : 'Không' }}</td>
            </tr>
          </tbody>
        </table>
        <p
          v-if="importPreview.length > 50"
          style="font-size:12px; padding:4px 8px; color:#555"
        >
          (Hiển thị 50 dòng đầu)
        </p>
      </div>
      <div class="form-actions" style="margin-top:15px">
        <button
          type="button"
          class="action-button success"
          :disabled="isImporting || importErrors.length > 0"
          @click="_performImport"
        >
          {{ isImporting ? 'Đang xử lý...' : 'Thực hiện Import (nháp)' }}
        </button>
        <button
          type="button"
          class="action-button secondary"
          @click="clearImport"
        >
          Hủy Import
        </button>
      </div>
    </div>

    <div class="form-container">
      <h2>
        {{ isEditing ? 'Cập nhật Thông tin Nhân viên' : 'Thêm Nhân viên Mới' }}
      </h2>
      <form @submit.prevent="handleSubmitEmployee">
        <div class="form-row">
          <div class="form-group">
            <label for="cbCode">Mã CB:</label>
            <input
              id="cbCode"
              v-model="currentEmployee.cbCode"
              type="text"
              required
              pattern="[0-9]*"
              inputmode="numeric"
              maxlength="9"
              placeholder="Nhập 9 chữ số"
              @input="onCBCodeInput"
            >
          </div>
        </div>
        <div class="form-group">
          <label for="fullName">Họ và Tên:</label>
          <input
            id="fullName"
            type="text"
            :value="currentEmployee.fullName"
            required
            @input="onInputTextOnly('fullName', $event)"
          >
        </div>
        <div class="form-group">
          <label for="username">Tên Đăng nhập:</label>
          <input
            id="username"
            type="text"
            :value="currentEmployee.username"
            required
            :disabled="isEditing"
            @input="onUsernameInput($event)"
          >
        </div>
        <div v-if="!isEditing" class="form-group">
          <label for="password">Mật khẩu:</label>
          <input
            id="password"
            type="password"
            :value="currentEmployee.passwordHash"
            placeholder="Nhập mật khẩu khi thêm mới"
            :required="!isEditing"
            @input="currentEmployee.passwordHash = $event.target.value"
          >
        </div>
        <div class="form-row">
          <div class="form-group">
            <label for="email">Email:</label>
            <input
              id="email"
              ref="emailInputRef"
              type="email"
              :value="currentEmployee.email"
              required
              @input="currentEmployee.email = $event.target.value"
            >
          </div>
          <div class="form-group">
            <label for="phoneNumber">Số Điện thoại:</label>
            <input
              id="phoneNumber"
              ref="phoneNumberInputRef"
              type="tel"
              :value="currentEmployee.phoneNumber"
              pattern="[0-9]*"
              inputmode="numeric"
              @input="onInputNumberOnly('phoneNumber', $event)"
            >
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label for="userAd">User AD:</label>
            <input
              id="userAd"
              v-model="currentEmployee.userAd"
              type="text"
              placeholder="Nhập User AD (nếu có)"
            >
          </div>
          <div class="form-group">
            <label for="userIpcas">User IPCAS:</label>
            <input
              id="userIpcas"
              v-model="currentEmployee.userIpcas"
              type="text"
              placeholder="Nhập User IPCAS"
            >
          </div>
          <div class="form-group">
            <label for="maCbtd">Mã CBTD:</label>
            <input
              id="maCbtd"
              v-model="currentEmployee.maCbtd"
              type="text"
              placeholder="Nhập Mã CBTD"
            >
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label for="branchId">Chi nhánh:</label>
            <select id="branchId" v-model.number="selectedBranchId" required>
              <option :value="null" disabled>
                -- Chọn Chi nhánh --
              </option>
              <option v-for="branch in branchOptions" :key="branch.Id || branch.Id" :value="branch.Id || branch.Id">
                {{ branch.Name || branch.Name }} ({{ branch.Code || branch.Code }})
              </option>
            </select>
          </div>
          <div class="form-group">
            <label for="departmentId">Phòng ban:</label>
            <select
              id="departmentId"
              v-model.number="currentEmployee.unitId"
              :disabled="!selectedBranchId"
              required
            >
              <option :value="null" disabled>
                -- Chọn Phòng ban --
              </option>
              <option v-for="dept in departmentOptions" :key="dept.Id || dept.Id" :value="dept.Id || dept.Id">
                {{ dept.Name || dept.Name }} ({{ dept.Code || dept.Code }})
              </option>
            </select>
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label for="positionId">Chức vụ:</label>
            <select
              id="positionId"
              :value="currentEmployee.positionId"
              required
              @change="currentEmployee.positionId = $event.target.value === '' ? null : Number($event.target.value)"
            >
              <option :value="null" disabled>
                -- Chọn Chức vụ --
              </option>
              <option
                v-for="position in positionStore.allPositions"
                :key="position.Id || position.Id"
                :value="position.Id || position.Id"
              >
                {{ position.Name || position.Name }}
              </option>
            </select>
          </div>
          <!-- Bỏ vai trò khỏi form theo yêu cầu -->
        </div>
        <!-- Bỏ trường Trạng thái; mặc định nhân viên là hoạt động -->

        <div class="form-actions">
          <button type="submit" :disabled="employeeStore.isLoading" class="action-button success">
            {{
              employeeStore.isLoading
                ? isEditing
                  ? 'Đang cập nhật...'
                  : 'Đang thêm...'
                : isEditing
                  ? 'Lưu Thay Đổi'
                  : 'Thêm Nhân viên'
            }}
          </button>
          <button
            v-if="isEditing"
            type="button"
            class="action-button secondary"
            @click="cancelEdit"
          >
            Hủy
          </button>
        </div>
      </form>
    </div>

    <!-- Ẩn hoàn toàn phần nhập KPI kỳ khoán nếu có -->
    <div v-if="false">
      <!-- Phần nhập KPI kỳ khoán sẽ không hiển thị -->
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, onUnmounted, ref, watch } from 'vue'
import apiClient from '../services/api.js'
import { useEmployeeStore } from '../stores/employeeStore.js'
import { usePositionStore } from '../stores/positionStore.js'
import { useUnitStore } from '../stores/unitStore.js'
import { getId, safeGet } from '../utils/casingSafeAccess.js'

const employeeStore = useEmployeeStore()
const unitStore = useUnitStore()
const positionStore = usePositionStore()

// Initial employee data function
const initialEmployeeData = () => ({
  id: null,
  cbCode: '',
  fullName: '',
  username: '',
  passwordHash: '',
  email: '',
  phoneNumber: '',
  userAd: '',
  userIpcas: '',
  maCbtd: '',
  isActive: true,
  unitId: null,
  positionId: null,
  // roleId removed per requirement
})

// Core reactive variables
const currentEmployee = ref(initialEmployeeData())
const fileInputRef = ref(null)
// Import Excel state
const importPreview = ref([]) // parsed rows
const importErrors = ref([])
const isImporting = ref(false)
const importStats = ref({ total: 0, new: 0, update: 0, processed: 0 })
const isEditing = ref(false)
const emailInputRef = ref(null)
const phoneNumberInputRef = ref(null)
const originalPasswordHash = ref('')
const isRoleDropdownOpen = ref(false)

// Biến cho tính năng chọn nhiều nhân viên
const selectedEmployeeIds = ref([])
const isDeleting = ref(false)

// PHÂN TRANG: Chỉ giữ lại một bộ duy nhất với sắp xếp theo thứ tự ưu tiên
const page = ref(1)
const pageSize = ref(20)

// Computed để sắp xếp nhân viên theo thứ tự ưu tiên
const sortedEmployees = computed(() => {
  if (!employeeStore.allEmployees || !Array.isArray(employeeStore.allEmployees)) {
    return []
  }

  console.log('🔍 sortedEmployees: Bắt đầu sắp xếp', employeeStore.allEmployees.length, 'nhân viên')

  return [...employeeStore.allEmployees].sort((a, b) => {
    // 1. Sắp xếp theo thứ tự chi nhánh
    const getBranchOrder = (employee) => {
      const unit = unitStore.allUnits?.find(u => u.Id === employee.UnitId)
      if (!unit) return 999

      const parentUnit = unitStore.allUnits?.find(u => u.Id === unit.ParentUnitId)
      const branchName = (parentUnit?.Name || '').toLowerCase()

      if (branchName.includes('hội sở')) return 0
      if (branchName.includes('bình lư')) return 1
      if (branchName.includes('phong thổ')) return 2
      if (branchName.includes('sìn hồ')) return 3
      if (branchName.includes('bum tở')) return 4
      if (branchName.includes('than uyên')) return 5
      if (branchName.includes('đoàn kết')) return 6
      if (branchName.includes('tân uyên')) return 7
      if (branchName.includes('nậm hàng')) return 8
      return 999
    }

    const branchOrderA = getBranchOrder(a)
    const branchOrderB = getBranchOrder(b)

    if (branchOrderA !== branchOrderB) {
      return branchOrderA - branchOrderB
    }

    // 2. Sắp xếp theo thứ tự phòng ban trong chi nhánh - Updated theo yêu cầu mới
    const getDeptOrder = (employee) => {
      const unit = unitStore.allUnits?.find(u => u.Id === employee.UnitId)
      if (!unit) return 999

      const deptName = (unit.Name || '').toLowerCase()

      // Thứ tự ưu tiên mới: Ban Giám đốc → Phòng Kế toán & Ngân quỹ → Phòng Khách hàng → Phòng Giao dịch
      if (deptName.includes('ban giám đốc') || deptName.includes('giám đốc')) return 0 // Ban Giám đốc
      if (deptName.includes('kế toán') || deptName.includes('ngân quỹ')) {
        if (deptName.includes('kế toán') && deptName.includes('ngân quỹ')) return 1 // Phòng Kế toán & Ngân quỹ
        if (deptName.includes('kế toán')) return 2 // Phòng Kế toán
        if (deptName.includes('ngân quỹ')) return 3 // Phòng Ngân quỹ
      }
      if (deptName.includes('khách hàng')) return 4
      if (deptName.includes('giao dịch')) return 5
      return 999 // Các phòng khác
    }

    const deptOrderA = getDeptOrder(a)
    const deptOrderB = getDeptOrder(b)

    if (deptOrderA !== deptOrderB) {
      return deptOrderA - deptOrderB
    }

    // 3. Sắp xếp theo chức vụ từ cao xuống thấp trong cùng phòng ban
    const getPositionOrder = (employee) => {
      const positionName = (
        employee.PositionName || 
        positionStore.allPositions?.find(p => p.Id === employee.PositionId)?.Name ||
        ''
      ).toLowerCase()

      // Thứ tự chức vụ: Giám đốc → Phó Giám đốc → Trưởng phòng → Phó phòng → Nhân viên
      if (positionName.includes('giám đốc')) {
        if (positionName.includes('phó')) return 1 // Phó Giám đốc
        return 0 // Giám đốc
      }
      if (positionName.includes('trưởng phòng') || positionName.includes('trưởng bộ phận')) return 2
      if (positionName.includes('phó phòng') || positionName.includes('phó bộ phận')) return 3
      if (positionName.includes('nhân viên') || positionName.includes('cán bộ')) return 4
      return 999 // Các chức vụ khác
    }

    const positionOrderA = getPositionOrder(a)
    const positionOrderB = getPositionOrder(b)

    if (positionOrderA !== positionOrderB) {
      return positionOrderA - positionOrderB
    }

    // 4. Cuối cùng sắp xếp theo họ và tên (ưu tiên theo họ) theo ABC
    const fullNameA = (a.FullName || '').trim()
    const fullNameB = (b.FullName || '').trim()

    return fullNameA.localeCompare(fullNameB, 'vi', { sensitivity: 'base' })
  })
})

const pagedEmployees = computed(() => {
  const start = (page.value - 1) * pageSize.value
  return sortedEmployees.value.slice(start, start + pageSize.value)
})
const totalPages = computed(() => {
  return Math.ceil(sortedEmployees.value.length / pageSize.value)
})
function prevPage() {
  if (page.value > 1) page.value--
}
function nextPage() {
  if (page.value < totalPages.value) page.value++
}
watch(pageSize, () => {
  page.value = 1
})

// Computed properties cho tính năng chọn nhiều
const selectableEmployees = computed(() => {
  // Lọc ra những nhân viên có thể chọn (không phải admin)
  if (!pagedEmployees.value || !Array.isArray(pagedEmployees.value)) return []
  return pagedEmployees.value.filter(emp => emp.username !== 'admin')
})

const isAllSelected = computed(() => {
  if (selectableEmployees.value.length === 0) return false
  return selectableEmployees.value.every(emp => selectedEmployeeIds.value.includes(emp.Id))
})

const isOverallLoading = computed(() => {
  return employeeStore.isLoading || unitStore.isLoading || positionStore.isLoading
})

// Computed để sắp xếp roles theo ABC
// Roles removed from this screen per requirement

const formError = ref(null)
const displayError = computed(() => {
  return formError.value || employeeStore.error || unitStore.error || positionStore.error
})

// Updated branchOptions: Custom ordering theo yêu cầu Hội Sở → Nậm Hàng
const branchOptions = computed(() => {
  console.log('🔍 branchOptions computed - unitStore.allUnits:', unitStore.allUnits)

  if (!unitStore.allUnits || !Array.isArray(unitStore.allUnits)) return []

  // Định nghĩa thứ tự theo yêu cầu: Hội Sở → Bình Lư → Phong Thổ → Sìn Hồ → Bum Tở → Than Uyên → Đoàn Kết → Tân Uyên → Nậm Hàng
  const _customOrder = [
    'HoiSo', // Hội Sở (ID=2)
    'BinhLu', // Chi nhánh Bình Lư (ID=10)
    'PhongTho', // Chi nhánh Phong Thổ (ID=11)
    'SinHo', // Chi nhánh Sìn Hồ (ID=12)
    'BumTo', // Chi nhánh Bum Tở (ID=13)
    'ThanUyen', // Chi nhánh Than Uyên (ID=14)
    'DoanKet', // Chi nhánh Đoàn Kết (ID=15)
    'TanUyen', // Chi nhánh Tân Uyên (ID=16)
    'NamHang', // Chi nhánh Nậm Hàng (ID=17)
  ]

  const branches = unitStore.allUnits
    .filter(u => {
      const type = (u.Type || u.Type || '').toUpperCase()
      return type === 'CNL1' || type === 'CNL2'
    })
    .sort((a, b) => {
      // Function để map Name thành customOrder index
      const getOrderIndex = unitName => {
        const name = (unitName || '').toLowerCase()
        if (name.includes('hội sở')) return 0
        if (name.includes('bình lư')) return 1
        if (name.includes('phong thổ')) return 2
        if (name.includes('sìn hồ')) return 3
        if (name.includes('bum tở')) return 4
        if (name.includes('than uyên')) return 5
        if (name.includes('đoàn kết')) return 6
        if (name.includes('tân uyên')) return 7
        if (name.includes('nậm hàng')) return 8
        return 999 // Unknown units go to the end
      }

      const indexA = getOrderIndex(a.Name || a.Name)
      const indexB = getOrderIndex(b.Name || b.Name)

      return indexA - indexB
    })

  console.log('🔍 branchOptions result:', branches)
  return branches
})

// Sửa departmentOptions: lọc phòng nghiệp vụ theo loại chi nhánh đã chọn với sắp xếp theo thứ tự yêu cầu
const departmentOptions = computed(() => {
  console.log('🔍 departmentOptions computed - selectedBranchId:', selectedBranchId.value)

  if (!selectedBranchId.value) return []
  if (!unitStore.allUnits || !Array.isArray(unitStore.allUnits)) return []

  const branch = unitStore.allUnits.find(u => (u.Id || u.Id) === Number(selectedBranchId.value))

  console.log('🔍 departmentOptions - found branch:', branch)

  if (!branch) return []

  // Lấy các phòng nghiệp vụ con của chi nhánh đã chọn
  const children = unitStore.allUnits.filter(u => (u.ParentUnitId || u.parentUnitId) === (branch.Id || branch.Id))

  console.log('🔍 departmentOptions - children units:', children)

  // Lọc chỉ lấy các phòng nghiệp vụ (PNVL1, PNVL2) và phòng giao dịch (PGD), loại bỏ các chi nhánh con (CNL2)
  const departments = children.filter(u => {
    const unitType = (u.Type || u.Type || '').toUpperCase()
    return unitType.includes('PNVL') || unitType === 'PGD'
  })

  console.log('🔍 departmentOptions - filtered departments:', departments)

  // Sắp xếp theo thứ tự: Ban Giám đốc, Phòng Khách hàng, Phòng Kế toán & Ngân quỹ, Phòng giao dịch
  const result = departments.sort((a, b) => {
    const getOrder = name => {
      const lowerName = (name || '').toLowerCase()
      if (lowerName.includes('ban giám đốc')) return 1
      if (lowerName.includes('phòng khách hàng')) return 2
      if (lowerName.includes('phòng kế toán')) return 3
      if (lowerName.includes('phòng giao dịch')) return 4
      return 999
    }

    return getOrder(a.Name || a.Name) - getOrder(b.Name || b.Name)
  })

  console.log('🔍 departmentOptions final result:', result)
  return result
})

// Thêm biến selectedBranchId để điều khiển chọn branch
const selectedBranchId = ref(null) // Sửa từ undefined thành null

// Lấy danh sách phòng nghiệp vụ con của branch đã chọn (debug)
const _branchChildren = computed(() => {
  if (!selectedBranchId.value && selectedBranchId.value !== 0) return []
  const branch = unitStore.allUnits.find(u => (u.Id || u.Id) === Number(selectedBranchId.value))
  if (!branch) return []
  return unitStore.allUnits.filter(u => (u.ParentUnitId || u.parentUnitId) === (branch.Id || branch.Id))
})

// Đảm bảo selectedBranchId luôn là kiểu number hoặc null
watch(selectedBranchId, (val, _oldVal) => {
  if (val !== null && typeof val !== 'number') {
    const numVal = Number(val)
    selectedBranchId.value = isNaN(numVal) ? null : numVal
  }
})

// Khi edit hoặc thêm mới, đồng bộ selectedBranchId với unitId của employee (nếu có)
function syncSelectedBranchWithEmployeeUnit() {
  const dept = unitStore.allUnits.find(u => (u.Id || u.Id) === currentEmployee.value.unitId)
  if (dept && (dept.ParentUnitId || dept.parentUnitId)) {
    selectedBranchId.value = dept.ParentUnitId || dept.parentUnitId
  } else {
    selectedBranchId.value = null
  }
}

// Sửa lại prepareAddEmployee để reset selectedBranchId về null khi thêm mới
function _prepareAddEmployee() {
  isEditing.value = false
  currentEmployee.value = initialEmployeeData()
  selectedBranchId.value = null
}

// Sửa lại startEditEmployee để đồng bộ selectedBranchId với parentUnitId của phòng nghiệp vụ hiện tại
const startEditEmployee = async employee => {
  formError.value = null
  employeeStore.error = null
  isEditing.value = true

  // Fetch chi tiết nhân viên để lấy passwordHash gốc
  try {
    const detail = await employeeStore.fetchEmployeeDetail(employee.Id)
    // Merge các trường từ employee và detail
    currentEmployee.value = extractEmployeePrimitives({ ...employee, ...detail })
    originalPasswordHash.value = detail.passwordHash || ''
    console.log('✅ fetchEmployeeDetail thành công - đã sửa JSON cycle với JsonIgnore')
  } catch {
    // Nếu lỗi, fallback về dữ liệu cũ
    currentEmployee.value = extractEmployeePrimitives(employee)
    originalPasswordHash.value = employee.passwordHash || ''
    formError.value = 'Không lấy được thông tin chi tiết nhân viên. Có thể không cập nhật được nếu thiếu passwordHash.'
  }

  // Đảm bảo các field có giá trị đúng
  currentEmployee.value.unitId = currentEmployee.value.unitId ? Number(currentEmployee.value.unitId) : null
  currentEmployee.value.positionId = currentEmployee.value.positionId ? Number(currentEmployee.value.positionId) : null

  // Đồng bộ selectedBranchId
  syncSelectedBranchWithEmployeeUnit()
  // Đồng bộ selectedBranchId (backup logic)
  const dept = unitStore.allUnits.find(u => (u.Id || u.Id) === currentEmployee.value.unitId)
  if (dept && (dept.ParentUnitId || dept.parentUnitId)) {
    selectedBranchId.value = dept.ParentUnitId || dept.parentUnitId
  } else {
    selectedBranchId.value = null
  }
  console.log(
    'Dữ liệu nhân viên được nạp vào form sửa (startEditEmployee):',
    JSON.parse(JSON.stringify(currentEmployee.value)),
  )
}

const loadInitialData = async () => {
  formError.value = null
  employeeStore.error = null
  unitStore.error = null
  positionStore.error = null

  await Promise.all([
    employeeStore.fetchEmployees(),
    unitStore.units.length === 0 ? unitStore.fetchUnits() : Promise.resolve(),
  positionStore.positions.length === 0 ? positionStore.fetchPositions() : Promise.resolve(),
  ])
}

// Function to extract only primitive fields from employee object
function extractEmployeePrimitives(employee) {
  if (!employee) return {}

  console.log('🔍 extractEmployeePrimitives - employee:', employee)

  const extractedId = getId(employee)
  console.log('🔍 extractEmployeePrimitives - extracted ID:', extractedId)

  return {
    id: extractedId,
    Id: extractedId, // Compatibility - include both cases
    cbCode: safeGet(employee, 'CBCode') || safeGet(employee, 'cbCode') || '',
    fullName: safeGet(employee, 'FullName'),
    username: safeGet(employee, 'Username'),
    passwordHash: safeGet(employee, 'PasswordHash'),
    email: safeGet(employee, 'Email'),
    phoneNumber: safeGet(employee, 'PhoneNumber'),
  userAd: safeGet(employee, 'UserAD') || safeGet(employee, 'UserAd') || safeGet(employee, 'userAd') || '',
  userIpcas: safeGet(employee, 'UserIPCAS') || safeGet(employee, 'UserIpcas') || safeGet(employee, 'userIpcas') || '',
  maCbtd: safeGet(employee, 'MaCBTD') || safeGet(employee, 'MACBTD') || safeGet(employee, 'maCbtd') || '',
    isActive:
      typeof employee.IsActive === 'boolean'
        ? employee.IsActive
        : typeof employee.isActive === 'boolean'
          ? employee.isActive
          : true,
    unitId: safeGet(employee, 'UnitId'),
    positionId: safeGet(employee, 'PositionId'),
  }
}

// Handle form submission for employee (create/update)
const handleSubmitEmployee = async () => {
  formError.value = null
  employeeStore.error = null

  // Extract and clean data for submission
  const dataToProcess = extractEmployeePrimitives(currentEmployee.value)

  // Vai trò đã bỏ khỏi form

  console.log('🔍 handleSubmitEmployee - dataToProcess before trim:', dataToProcess)

  for (const key in dataToProcess) {
    if (
      key !== 'unitId' &&
      key !== 'positionId' &&
  key !== 'isActive' &&
      typeof dataToProcess[key] === 'string'
    ) {
      dataToProcess[key] = dataToProcess[key].trim()
    }
  }

  // Validate email must contain @
  if (!dataToProcess.email || !dataToProcess.email.includes('@')) {
    formError.value = 'Email phải chứa ký tự @'
    if (emailInputRef.value) {
      emailInputRef.value.focus()
    }
    return
  }

  // Validate cbCode: only numbers and exactly 9 characters
  if (!dataToProcess.cbCode || !/^\d{9}$/.test(dataToProcess.cbCode)) {
    formError.value = `Mã CB phải là đúng 9 chữ số (hiện tại: "${dataToProcess.cbCode || ''}" - ${(dataToProcess.cbCode || '').length} ký tự)`
    return
  }

  // Validate phone number if provided
  if (dataToProcess.phoneNumber && !/^\d{10}$/.test(dataToProcess.phoneNumber)) {
    formError.value = 'Số điện thoại sai chuẩn, đề nghị nhập lại'
    if (phoneNumberInputRef.value) {
      phoneNumberInputRef.value.focus()
    }
    return
  }

  // Ensure unitId and positionId are valid numbers
  if (dataToProcess.unitId !== null && isNaN(Number(dataToProcess.unitId))) {
    dataToProcess.unitId = null
  } else if (dataToProcess.unitId !== null) {
    dataToProcess.unitId = Number(dataToProcess.unitId)
  }

  if (dataToProcess.positionId !== null && isNaN(Number(dataToProcess.positionId))) {
    dataToProcess.positionId = null
  } else if (dataToProcess.positionId !== null) {
    dataToProcess.positionId = Number(dataToProcess.positionId)
  }

  // Basic field validation
  if (!dataToProcess.fullName) {
    formError.value = 'Họ và tên không được để trống!'
    return
  }
  if (!dataToProcess.username) {
    formError.value = 'Tên đăng nhập không được để trống!'
    return
  }
  if (!isEditing.value && !dataToProcess.passwordHash) {
    formError.value = 'Mật khẩu không được để trống khi thêm mới!'
    return
  }
  if (dataToProcess.unitId === null || dataToProcess.unitId === undefined) {
    formError.value = 'Vui lòng chọn Đơn vị.'
    return
  }
  if (dataToProcess.positionId === null || dataToProcess.positionId === undefined) {
    formError.value = 'Vui lòng chọn Chức vụ.'
    return
  }

  console.log('--- Bắt đầu handleSubmitEmployee (Nhân viên) ---')
  console.log('Chế độ sửa:', isEditing.value)
  console.log('🔍 CB Code trước khi submit:', currentEmployee.value.cbCode)
  console.log('🔍 Employee ID trước khi submit:', dataToProcess.id)
  console.log('Dữ liệu sau khi trim và chuẩn bị (dataToProcess):', JSON.parse(JSON.stringify(dataToProcess)))

  if (isEditing.value && dataToProcess.id !== null && dataToProcess.id !== undefined) {
    try {
      // If not entering new password, always send original passwordHash
      const updateData = { ...dataToProcess }
      if (!dataToProcess.passwordHash) {
        updateData.passwordHash = originalPasswordHash.value
      }
      await employeeStore.updateEmployee(updateData)
      alert('Cập nhật nhân viên thành công!')
      cancelEdit()
    } catch (error) {
      let backendMsg = ''
      if (error?.response?.data?.errors) {
        backendMsg = Object.values(error.response.data.errors)
          .map(arr => (Array.isArray(arr) ? arr.join(', ') : typeof arr === 'string' ? arr : JSON.stringify(arr)))
          .join(' | ')
      } else if (error?.response?.data?.message) {
        backendMsg = error.response.data.message
      } else {
        backendMsg = error.message
      }
      formError.value = 'Không thể cập nhật nhân viên. ' + backendMsg
      console.error('Lỗi khi cập nhật nhân viên:', error)
    }
  } else {
    try {
      // eslint-disable-next-line no-unused-vars
  const { id: _discardId, ...newEmployeeData } = dataToProcess
      // Remove passwordHash if empty string or undefined
      if (!newEmployeeData.passwordHash) {
        delete newEmployeeData.passwordHash
      }
      // Ensure unitId and positionId are valid numbers
      if (newEmployeeData.unitId === null || isNaN(Number(newEmployeeData.unitId))) {
        formError.value = 'Vui lòng chọn Đơn vị.'
        return
      }
      if (newEmployeeData.positionId === null || isNaN(Number(newEmployeeData.positionId))) {
        formError.value = 'Vui lòng chọn Chức vụ.'
        return
      }
      newEmployeeData.unitId = Number(newEmployeeData.unitId)
      newEmployeeData.positionId = Number(newEmployeeData.positionId)
      console.log('Dữ liệu gửi đi cho createEmployee (đã làm sạch):', JSON.parse(JSON.stringify(newEmployeeData)))
      await employeeStore.createEmployee(newEmployeeData)
      alert('Thêm nhân viên thành công!')
      resetForm()
    } catch (error) {
      let backendMsg = ''
      if (error?.response?.data?.errors) {
        // If backend returns multiple errors like errors: { Field: [msg] }
        backendMsg = Object.values(error.response.data.errors)
          .map(arr => arr.join(', '))
          .join(' | ')
      } else if (error?.response?.data?.message) {
        backendMsg = error.response.data.message
      } else {
        backendMsg = error.message
      }
      formError.value = 'Không thể tạo nhân viên. ' + backendMsg
      console.error('Lỗi khi thêm nhân viên:', error)
    }
  }
}

// Cancel edit and reset form
const cancelEdit = () => {
  isEditing.value = false
  resetForm()
  formError.value = null
  employeeStore.error = null
}

// Reset form to initial state
const resetForm = () => {
  currentEmployee.value = initialEmployeeData()
  selectedBranchId.value = null
}

// Confirm and delete employee
const confirmDeleteEmployee = async employeeId => {
  formError.value = null
  employeeStore.error = null
  // Check valid ID before calling API
  if (!employeeId || isNaN(Number(employeeId))) {
    formError.value = 'ID nhân viên không hợp lệ!'
    return
  }
  if (confirm(`Bạn có chắc chắn muốn xóa nhân viên có ID: ${employeeId} không?`)) {
    try {
      await employeeStore.deleteEmployee(Number(employeeId))
      alert('Xóa nhân viên thành công!')
    } catch (error) {
      console.error('Lỗi khi xóa nhân viên:', error)
    }
  }
}

// ========================================
// METHODS CHO TÍNH NĂNG CHỌN NHIỀU
// ========================================

// Chọn/bỏ chọn tất cả nhân viên có thể chọn được
const toggleSelectAll = () => {
  if (isAllSelected.value) {
    // Bỏ chọn tất cả
    selectedEmployeeIds.value = []
  } else {
    // Chọn tất cả nhân viên có thể chọn (không phải admin)
    selectedEmployeeIds.value = selectableEmployees.value.map(emp => emp.Id)
  }
}

// Xác nhận và xóa các nhân viên đã chọn
const confirmDeleteSelected = async () => {
  if (selectedEmployeeIds.value.length === 0) {
    alert('Vui lòng chọn ít nhất một nhân viên để xóa.')
    return
  }

  const selectedEmployees = employeeStore.allEmployees.filter(emp => selectedEmployeeIds.value.includes(emp.Id))

  const employeeNames = selectedEmployees.map(emp => emp.fullName).join(', ')

  if (
    confirm(`Bạn có chắc chắn muốn xóa ${selectedEmployeeIds.value.length} nhân viên sau không?\n\n${employeeNames}`)
  ) {
    await deleteSelectedEmployees()
  }
}

// Thực hiện xóa các nhân viên đã chọn
const deleteSelectedEmployees = async () => {
  isDeleting.value = true
  formError.value = null
  employeeStore.error = null

  try {
    const result = await employeeStore.deleteMultipleEmployees(selectedEmployeeIds.value)

    // Reset danh sách chọn
    selectedEmployeeIds.value = []

    // Hiển thị thông báo thành công
    alert(`✅ ${result.deletedCount} nhân viên đã được xóa thành công!`)
  } catch (error) {
    console.error('Lỗi khi xóa nhiều nhân viên:', error)
    formError.value = employeeStore.error || 'Có lỗi xảy ra khi xóa nhân viên.'
  } finally {
    isDeleting.value = false
  }
}

// Xóa selection khi chuyển trang
watch(page, () => {
  selectedEmployeeIds.value = []
})

// Xóa selection khi reload data
watch(
  () => employeeStore.allEmployees,
  () => {
    // Lọc ra những ID không còn tồn tại
    const existingIds = employeeStore.allEmployees.map(emp => emp.Id)
    selectedEmployeeIds.value = selectedEmployeeIds.value.filter(id => existingIds.includes(id))
  },
)

// ========================================
// ROLE DROPDOWN FUNCTIONS
// ========================================
// INPUT VALIDATION FUNCTIONS
// ========================================
function onInputNumberOnly(field, event) {
  const val = event.target.value.replace(/[^0-9]/g, '')
  currentEmployee.value[field] = val
}

function onCBCodeInput(event) {
  let val = event.target.value.replace(/[^0-9]/g, '')
  if (val.length > 9) {
    val = val.substring(0, 9)
  }
  currentEmployee.value.cbCode = val
}

function onInputTextOnly(field, event) {
  const val = event.target.value.replace(/[^a-zA-ZÀ-ỹ\s]/g, '')
  currentEmployee.value[field] = val
}

function onUsernameInput(event) {
  const val = event.target.value.replace(/[^a-zA-Z0-9]/g, '')
  currentEmployee.value.username = val

  // Auto-generate email when creating new employee (not when editing)
  if (!isEditing.value && val) {
    currentEmployee.value.email = `${val}@agribank.com.vn`
  }
}

// ========================================
// UTILITY FUNCTIONS
// ========================================
function scrollToAddEmployeeForm() {
  const formContainer = document.querySelector('.form-container')
  if (formContainer) {
    formContainer.scrollIntoView({ behavior: 'smooth' })
  }
}

// Vai trò không còn hiển thị nên bỏ helper getRoleNames

// ========================================
// EXCEL IMPORT FUNCTIONS
// ========================================
function triggerFileSelect() {
  if (fileInputRef.value) fileInputRef.value.click()
}

function downloadTemplate() {
  // Tạo file Excel .xlsx đúng chuẩn theo yêu cầu mới, KHÔNG chứa cột "Mã NV"
  // Cột mẫu:
  // - Mã CB
  // - Họ và tên
  // - User (thay cho Tên đăng nhập)
  // - Password (thay cho Mật khẩu)
  // - Email
  // - Số điện thoại
  // - User AD
  // - User IPCAS
  // - Mã CBTD
  // - Chi nhánh
  // - Phòng ban (thay cho Phòng nghiệp vụ)
  // - Chức vụ
  import('xlsx').then(mod => {
    const XLSX = mod.default || mod
    const wsData = [
      ['Mã CB', 'Họ và tên', 'User', 'Password', 'Email', 'Số điện thoại', 'User AD', 'User IPCAS', 'Mã CBTD', 'Chi nhánh', 'Phòng ban', 'Chức vụ'],
      // Dòng ví dụ (có thể chỉnh sửa hoặc xóa):
      ['123456789', 'Nguyễn Văn A', 'nva', '123456', 'nva@agribank.com.vn', '0987654321', 'NVA', 'IPCAS001', 'CBTD01', 'Chi nhánh Lai Châu', 'Phòng KHCN', 'Nhân viên'],
    ]
    const ws = XLSX.utils.aoa_to_sheet(wsData)
    const wb = XLSX.utils.book_new()
    XLSX.utils.book_append_sheet(wb, ws, 'Mau_Import')
    XLSX.writeFile(wb, 'mau_import_nhan_vien.xlsx')
  })
}

async function handleFileChange(e) {
  const file = e.target.files[0]
  if (!file) return
  importErrors.value = []
  importPreview.value = []

  console.log('🔍 Debug mapping data:')
  console.log('Available Units:', (unitStore.allUnits || []).map(u => ({ id: u.Id || u.id, name: u.Name || u.name, code: u.Code || u.code })))
  console.log('Available Positions:', (positionStore.allPositions || positionStore.positions || []).map(p => ({ id: p.Id || p.id, name: p.Name || p.name })))

  try {
    const XLSX = await import('xlsx')
    const data = await file.arrayBuffer()
    const workbook = XLSX.read(data, { type: 'array' })
    const sheetName = workbook.SheetNames[0]
    const worksheet = workbook.Sheets[sheetName]
    const rows = XLSX.utils.sheet_to_json(worksheet, { defval: '' })

    console.log(`📊 Excel data: ${rows.length} rows`)
    if (rows.length > 0) {
      console.log('Sample row:', rows[0])
    }

    // Normalize keys
    const normalize = s => ('' + (s ?? '')).trim().toLowerCase()
    const mapped = rows.map((r, idx) => {
      const rowIndex = idx + 2 // excel header at row 1
      const cbCode = (r['Mã CB'] || r['Ma CB'] || r['MACB'] || '').toString().trim()
      const branchName = r['Chi nhánh'] || r['Chi nhanh'] || r['Chi Nhanh'] || r['Branch'] || ''
      const deptName = r['Phòng ban'] || r['Phong ban'] || r['Phong Ban'] || r['Department'] || ''
      const positionName = r['Chức vụ'] || r['Chuc vu'] || r['Chuc Vu'] || r['Position'] || ''

      // Map Chi nhánh/Phòng ban/Chức vụ sang ID với logic cải thiện - FIX: Lọc theo Chi nhánh + Phòng ban
      let unitId = null
      let positionId = null

      // Tìm đơn vị (Unit) dựa trên Chi nhánh và Phòng ban - PHẢI CÓ CẢ 2 THÔNG TIN
      if (branchName && deptName) {
        const allUnits = unitStore.allUnits || []

        // BƯỚC 1: Tìm chi nhánh trước
        const targetBranch = normalize(branchName)
        const matchedBranches = allUnits.filter(u => {
          const unitName = normalize(u.Name || u.name)
          const unitCode = normalize(u.Code || u.code)
          // Chi nhánh thường có Type = 'CNL1' hoặc 'CNL2'
          const unitType = (u.Type || '').toUpperCase()
          return (unitName.includes(targetBranch) || targetBranch.includes(unitName) ||
                  unitCode.includes(targetBranch) || targetBranch.includes(unitCode)) &&
                 (unitType === 'CNL1' || unitType === 'CNL2' || unitName.includes('hội sở'))
        })

        if (matchedBranches.length > 0) {
          console.log(`🏢 Found ${matchedBranches.length} matching branches for "${branchName}":`, matchedBranches.map(b => b.Name))

          // BƯỚC 2: Trong các chi nhánh tìm được, tìm phòng ban thuộc chi nhánh đó
          const targetDept = normalize(deptName)
          let matchedUnit = null

          for (const branch of matchedBranches) {
            // Tìm phòng ban có ParentUnitId = branch.Id
            const deptInBranch = allUnits.find(u =>
              (u.ParentUnitId === branch.Id) &&
              (normalize(u.Name || u.name).includes(targetDept) || targetDept.includes(normalize(u.Name || u.name))),
            )

            if (deptInBranch) {
              matchedUnit = deptInBranch
              console.log(`✅ Found department "${deptName}" in branch "${branch.Name}": ${deptInBranch.Name} (${deptInBranch.Id})`)
              break
            }
          }

          if (matchedUnit) {
            unitId = matchedUnit.Id || matchedUnit.id
          } else {
            console.log(`❌ Department "${deptName}" not found in any branch matching "${branchName}"`)
            importErrors.value.push(`Dòng ${rowIndex}: Không tìm thấy phòng ban "${deptName}" trong chi nhánh "${branchName}"`)
          }
        } else {
          console.log(`❌ Branch "${branchName}" not found`)
          importErrors.value.push(`Dòng ${rowIndex}: Không tìm thấy chi nhánh "${branchName}"`)
        }
      } else {
        // Fallback: Nếu thiếu thông tin chi nhánh hoặc phòng ban
        if (!branchName) importErrors.value.push(`Dòng ${rowIndex}: Thiếu thông tin Chi nhánh`)
        if (!deptName) importErrors.value.push(`Dòng ${rowIndex}: Thiếu thông tin Phòng ban`)
      }

      // Map Chức vụ sang PositionId
      if (positionName) {
        const allPositions = positionStore.allPositions || positionStore.positions || []
        const targetPos = normalize(positionName)

        // Tìm chính xác trước
        let matchedPosition = allPositions.find(p => normalize(p.Name || p.name) === targetPos)

        // Tìm kiếm mờ nếu không tìm thấy chính xác
        if (!matchedPosition) {
          matchedPosition = allPositions.find(p => {
            const posName = normalize(p.Name || p.name)
            return posName.includes(targetPos) || targetPos.includes(posName)
          })
        }

        if (matchedPosition) {
          positionId = matchedPosition.Id || matchedPosition.id
          console.log(`✅ Position match: "${positionName}" -> ${matchedPosition.Name} (${positionId})`)
        }
      }

      // Debug logging for mapping với thông tin chi tiết - Updated for new logic
      if (rowIndex <= 5) { // Log first 5 rows để không spam
        console.log(`\n=== Row ${rowIndex} Enhanced Mapping Debug ===`)
        console.log(`Excel data: Chi nhánh="${branchName}", Phòng ban="${deptName}", Chức vụ="${positionName}"`)
        console.log(`Mapped IDs: UnitId=${unitId}, PositionId=${positionId}`)
        console.log(`Status: ${unitId ? '✅' : '❌'} Unit (Branch+Dept), ${positionId ? '✅' : '❌'} Position`)
        if (unitId) {
          const matchedUnit = (unitStore.allUnits || []).find(u => u.Id === unitId)
          const parentUnit = (unitStore.allUnits || []).find(u => u.Id === matchedUnit?.ParentUnitId)
          console.log(`Final mapping: ${parentUnit?.Name || 'N/A'} -> ${matchedUnit?.Name || 'N/A'}`)
        }
        console.log('================================\n')
      }

      // Safe value extraction with type conversion
      const safeGetString = (row, keys) => {
        for (const key of keys) {
          const val = row[key]
          if (val !== undefined && val !== null) {
            // Convert to string and trim, handle numbers/formulas
            if (typeof val === 'number') return val.toString().trim()
            if (typeof val === 'string') return val.trim()
            if (typeof val === 'object' && val.v !== undefined) return String(val.v).trim() // Excel formula result
            return String(val).trim()
          }
        }
        return ''
      }

      const obj = {
        cbCode,
        fullName: safeGetString(r, ['Họ và tên', 'Ho va ten', 'Họ tên', 'Ho ten', 'HOTEN']),
        username: safeGetString(r, ['User', 'USER', 'Username', 'Tên ĐN', 'Ten DN', 'TENDN']),
        password: safeGetString(r, ['Password', 'PASSWORD']),
        email: safeGetString(r, ['Email', 'EMAIL']),
        phoneNumber: safeGetString(r, ['Số điện thoại', 'So dien thoai', 'SĐT', 'SDT', 'Phone']),
        userAd: safeGetString(r, ['User AD', 'USER AD', 'UserAD']),
        userIpcas: safeGetString(r, ['User IPCAS', 'USER IPCAS', 'UserIPCAS']),
        maCbtd: safeGetString(r, ['Mã CBTD', 'Ma CBTD', 'MACBTD']),
        branchName: branchName || '',
        departmentName: deptName || '',
        positionName: positionName || '',
        unitId,
        positionId,
        _row: rowIndex,
      }
      return obj
    })
    // Improved validation với detailed error messages
    const validRecords = []
    mapped.forEach(m => {
      const recordErrors = []

      // CBCode validation
      if (!m.cbCode) {
        recordErrors.push('Thiếu Mã CB')
      } else if (m.cbCode.length !== 9 || !/^\d{9}$/.test(m.cbCode)) {
        recordErrors.push(`Mã CB "${m.cbCode}" phải gồm đúng 9 chữ số`)
      }

      // Email validation
      if (m.email && !m.email.includes('@')) {
        recordErrors.push('Email không hợp lệ')
      }

      // UnitId validation với chi tiết mapping
      if (!m.unitId) {
        if (m.branchName || m.departmentName) {
          recordErrors.push(`Không tìm thấy đơn vị cho Chi nhánh: "${m.branchName}", Phòng ban: "${m.departmentName}"`)
        } else {
          recordErrors.push('Thiếu thông tin Chi nhánh/Phòng ban')
        }
      }

      // PositionId validation
      if (!m.positionId) {
        if (m.positionName) {
          recordErrors.push(`Không tìm thấy chức vụ cho "${m.positionName}"`)
        } else {
          recordErrors.push('Thiếu thông tin Chức vụ')
        }
      }

      // Add errors to global error list
      recordErrors.forEach(err => {
        importErrors.value.push(`Dòng ${m._row}: ${err}`)
      })

      // Chỉ add vào validRecords nếu không có lỗi
      if (recordErrors.length === 0) {
        validRecords.push(m)
      }
    })

    console.log(`📋 Total records: ${mapped.length}, Valid records: ${validRecords.length}, Errors: ${importErrors.value.length}`)
    importPreview.value = validRecords // Chỉ show records hợp lệ

    // Tính toán import statistics với validRecords
    const existingCBCodes = new Set(employeeStore.allEmployees.map(e => e.CBCode || e.cbCode).filter(code => code))
    let update = 0; let create = 0
    validRecords.forEach(m => {
      if (m.cbCode && existingCBCodes.has(m.cbCode)) update++
      else create++
    })
    importStats.value = { total: mapped.length, new: create, update, processed: 0, valid: validRecords.length, invalid: mapped.length - validRecords.length }

    // Hiển thị summary
    if (importErrors.value.length > 0) {
      console.warn(`⚠️ Import validation: ${validRecords.length} valid records, ${importErrors.value.length} errors`)
    } else {
      console.log(`✅ All ${validRecords.length} records are valid for import`)
    }
  } catch (err) {
    importErrors.value.push('Không thể đọc file: ' + err.message)
    console.error(err)
  } finally {
    // reset input so same file can be selected again
    e.target.value = ''
  }
}

async function _performImport() {
  if (importPreview.value.length === 0 || isImporting.value) return
  isImporting.value = true
  importErrors.value = []
  importStats.value.processed = 0
  const existingByCBCode = {}
  employeeStore.allEmployees.forEach(e => {
    const cbCode = e.CBCode || e.cbCode;
    if (cbCode) existingByCBCode[cbCode] = e;
  })
  try {
    // Gọi API backend bulk import theo CBCode
  const requestPayload = {
      rows: importPreview.value.map(r => ({
        CBCode: r.cbCode || null,
        FullName: r.fullName || null,
        Username: r.username || (r.cbCode ? String(r.cbCode).toLowerCase() : null),
        Password: r.password || null,
        UserAD: r.userAd || null,
        Email: r.email || null,
        UserIPCAS: r.userIpcas || null,
        MaCBTD: r.maCbtd || null,
        PhoneNumber: r.phoneNumber || null,
        // Dùng Unit/Position được map theo từng dòng
        UnitId: r.unitId || null,
        PositionId: r.positionId || null,
      })),
      overwriteExisting: true,
      autoGenerateMissingUsernames: true,
    }
    console.log('🚀 Sending import request with payload:', {
      rowsCount: requestPayload.rows.length,
      sampleRow: requestPayload.rows[0], // Log first row for debugging
      problemRow: requestPayload.rows[1], // Log second row that's causing issues
      overwriteExisting: requestPayload.overwriteExisting,
      autoGenerateMissingUsernames: requestPayload.autoGenerateMissingUsernames,
    })

    // Additional debug logging for MaCBTD
    requestPayload.rows.forEach((row, index) => {
      if (index < 3) { // Log first 3 rows
        console.log(`🔍 Row ${index}: MaCBTD type=${typeof row.MaCBTD}, value=`, row.MaCBTD)
      }
    })

    const resp = await apiClient.post('/Employees/import-cbcode', requestPayload)
    const data = resp.data || {}
    alert(`✅ Import hoàn tất. Tạo mới: ${data.inserted || 0}, Cập nhật: ${data.updated || 0}, Bỏ qua: ${data.skipped || 0}`)
    // Reload employees
    await employeeStore.fetchEmployees()
    clearImport()
  } catch (err) {
    console.error('Import lỗi:', err)

    // Detailed error logging
    if (err.response?.data) {
      console.error('📋 Backend validation errors:', err.response.data)

      // Log specific errors object for debugging
      if (err.response.data.errors) {
        console.error('🔍 Detailed errors:', err.response.data.errors)
        console.table(err.response.data.errors)
      }

      // Parse validation errors from ASP.NET format
      const errorData = err.response.data
      let errorMessage = 'Import thất bại: '

      if (errorData.errors) {
        // ASP.NET validation errors format
        const validationErrors = []
        Object.keys(errorData.errors).forEach(field => {
          const fieldErrors = errorData.errors[field]
          if (Array.isArray(fieldErrors)) {
            fieldErrors.forEach(error => {
              validationErrors.push(`${field}: ${error}`)
            })
          } else {
            validationErrors.push(`${field}: ${fieldErrors}`)
          }
        })
        errorMessage += validationErrors.join(', ')
      } else if (errorData.title) {
        errorMessage += errorData.title
      } else if (errorData.message) {
        errorMessage += errorData.message
      } else {
        errorMessage += JSON.stringify(errorData)
      }

      importErrors.value.push(errorMessage)
    } else {
      const msg = err.response?.data?.message || err.response?.data || err.message
      importErrors.value.push('Import thất bại: ' + msg)
    }
  } finally {
    isImporting.value = false
  }
}

function clearImport() {
  importPreview.value = []
  importErrors.value = []
  importStats.value = { total: 0, new: 0, update: 0, processed: 0 }
}

// Initialize employee code when mounting
onMounted(() => {
  loadInitialData()
  syncSelectedBranchWithEmployeeUnit()

  // Add click outside listener for role dropdown
  document.addEventListener('click', handleClickOutside)
})

// Cleanup event listener
onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})

// Handle click outside dropdown
const handleClickOutside = event => {
  const dropdownContainer = event.target.closest('.role-dropdown-container')
  if (!dropdownContainer && isRoleDropdownOpen.value) {
    isRoleDropdownOpen.value = false
  }
}

// Watch for currentEmployee.unitId changes to sync branch selection
watch(() => currentEmployee.value.unitId, syncSelectedBranchWithEmployeeUnit)

// When choosing branch, reset department if it doesn't belong to that branch
watch(selectedBranchId, newVal => {
  if (!newVal || !currentEmployee.value.unitId) return
  const dept = unitStore.allUnits.find(u => u.Id === currentEmployee.value.unitId)
  if (!dept || dept.parentUnitId !== newVal) {
    currentEmployee.value.unitId = null
  }
})
</script>

<style scoped>
/* Phần CSS giữ nguyên như Sếp đã yêu cầu ở các file View trước */
.employees-view {
  max-width: 100%;
  width: 95%;
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
  flex-wrap: wrap;
  justify-content: space-between;
  align-items: center;
  transition: background-color 0.2s ease-in-out;
}
.list-item:hover {
  background-color: #e0e6e8;
}

.item-info {
  text-align: left;
  margin-bottom: 8px;
}

.item-info strong {
  color: #2c3e50;
  display: block;
  margin-bottom: 4px;
}

.item-details {
  font-size: 0.85em;
  color: #7f8c8d;
  display: block;
  line-height: 1.4;
}

.actions {
  display: flex;
  gap: 10px;
  flex-shrink: 0;
  margin-top: 8px;
  justify-content: flex-start;
}

/* Button Base Styles */
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
  transition: all 0.2s ease;
}

.action-button:disabled,
.edit-btn:disabled,
.delete-btn:disabled,
.cancel-btn:disabled {
  background-color: #bdc3c7;
  cursor: not-allowed;
}

/* Button Color Variants */
.action-button {
  background-color: #3498db;
}
.action-button:hover:not(:disabled) {
  background-color: #2980b9;
}
.action-button.success {
  background-color: #27ae60;
}
.action-button.success:hover:not(:disabled) {
  background-color: #229954;
}
.action-button.secondary {
  background-color: #95a5a6;
}
.action-button.secondary:hover:not(:disabled) {
  background-color: #7f8c8d;
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
  background-color: #7f8c8c;
}

/* Form Container Styling */
.form-container {
  background-color: #ffffff;
  padding: 25px;
  border-radius: 6px;
  border: 1px solid #dde0e3;
  box-shadow: 0 1px 5px rgba(0, 0, 0, 0.05);
  margin-bottom: 30px;
}

.form-container h2 {
  margin-top: 0;
  margin-bottom: 25px;
  text-align: center;
  color: #34495e;
}

/* Form styling */
.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: 600;
  color: #2c3e50;
}

/* Form Input Styles */
.form-group input,
.form-group select {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid #bdc3c7;
  border-radius: 4px;
  font-size: 14px;
  transition: border-color 0.2s ease;
  box-sizing: border-box;
}

.form-group input:focus,
.form-group select:focus {
  outline: none;
  border-color: #3498db;
  box-shadow: 0 0 0 2px rgba(52, 152, 219, 0.2);
}

.form-row {
  display: flex;
  gap: 15px;
  margin-bottom: 20px;
}

.form-row .form-group {
  flex: 1;
  margin-bottom: 0;
}

.form-actions {
  text-align: center;
  padding-top: 20px;
  border-top: 1px solid #ecf0f1;
  margin-top: 25px;
}

/* Enhanced Action Button Styles */
.action-button {
  padding: 10px 20px;
  font-size: 14px;
  font-weight: 600;
  margin-right: 10px;
}

.action-button:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

/* Loading and disabled states */
.loading {
  opacity: 0.7;
  pointer-events: none;
}

/* Separator styling */
.separator {
  margin: 35px 0;
  border: 0;
  border-top: 1px solid #bdc3c7;
}

/* Responsive design */
@media (max-width: 768px) {
  .employees-view {
    margin: 10px;
    padding: 15px;
  }

  .form-row {
    flex-direction: column;
    gap: 0;
  }

  .list-item {
    flex-direction: column;
    align-items: flex-start;
  }

  .actions {
    width: 100%;
    justify-content: space-between;
    margin-top: 15px;
  }
}

/* Animation for smooth transitions */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* Role Dropdown Styles - Improved */
.role-dropdown-container {
  position: relative;
  width: 100%;
}

.role-dropdown-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 14px;
  border: 2px solid #e2e8f0;
  border-radius: 8px;
  background-color: #ffffff;
  cursor: pointer;
  min-height: 44px;
  transition: all 0.3s ease;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  user-select: none;
}

.role-dropdown-header:hover {
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
  transform: translateY(-1px);
}

.role-dropdown-header.active {
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.2);
  background-color: #f8fafc;
}

.selected-roles-text {
  flex: 1;
  text-align: left;
  color: #374151;
  font-size: 14px;
  font-weight: 500;
  line-height: 1.4;
}

.dropdown-arrow {
  transition: transform 0.3s ease;
  color: #6b7280;
  font-size: 14px;
  font-weight: bold;
  margin-left: 10px;
}

.dropdown-arrow.rotated {
  transform: rotate(180deg);
  color: #3b82f6;
}

.role-dropdown-menu {
  position: absolute;
  top: calc(100% + 4px);
  left: 0;
  right: 0;
  background: #ffffff;
  border: 2px solid #3b82f6;
  border-radius: 8px;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
  max-height: 280px;
  overflow-y: auto;
  z-index: 1000;
  animation: dropdownFadeIn 0.2s ease-out;
}

@keyframes dropdownFadeIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.role-option {
  display: flex;
  align-items: flex-start;
  padding: 12px 16px;
  cursor: pointer;
  transition: all 0.2s ease;
  border-bottom: 1px solid #f1f5f9;
  position: relative;
}

.role-option:hover {
  background-color: #f8fafc;
  border-left: 4px solid #3b82f6;
  padding-left: 12px;
}

.role-option:last-child {
  border-bottom: none;
  border-radius: 0 0 6px 6px;
}

.role-option:first-child {
  border-radius: 6px 6px 0 0;
}

.role-option input[type='checkbox'] {
  margin-right: 12px;
  margin-top: 1px;
  width: 16px;
  height: 16px;
  cursor: pointer;
  accent-color: #3b82f6;
}

.role-option label {
  flex: 1;
  margin: 0;
  cursor: pointer;
  font-weight: 600;
  color: #1f2937;
  font-size: 14px;
  line-height: 1.3;
}

.role-description {
  display: block;
  color: #6b7280;
  font-size: 12px;
  margin-top: 4px;
  font-weight: 400;
  font-style: normal;
  line-height: 1.2;
}

.no-roles {
  padding: 20px 16px;
  text-align: center;
  color: #9ca3af;
  font-style: italic;
  font-size: 14px;
}

/* Custom scrollbar for dropdown */
.role-dropdown-menu::-webkit-scrollbar {
  width: 6px;
}

.role-dropdown-menu::-webkit-scrollbar-track {
  background: #f1f5f9;
  border-radius: 3px;
}

.role-dropdown-menu::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 3px;
}

.role-dropdown-menu::-webkit-scrollbar-thumb:hover {
  background: #94a3b8;
}

/* Responsive adjustments for role dropdown */
@media (max-width: 768px) {
  .role-dropdown-header {
    padding: 8px 12px;
    min-height: 40px;
  }

  .role-option {
    padding: 10px 14px;
  }

  .role-dropdown-menu {
    max-height: 240px;
  }
}

/* Multi-select styling */
.checkbox-cell {
  text-align: center;
  padding: 8px !important;
  vertical-align: middle;
}

.checkbox-cell input[type='checkbox'] {
  transform: scale(1.1);
  cursor: pointer;
  accent-color: #3498db;
}

.checkbox-cell input[type='checkbox']:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.selected-row {
  background-color: #e3f2fd !important;
  border-left: 4px solid #2196f3;
}

.selected-row:hover {
  background-color: #bbdefb !important;
}

.action-cell {
  white-space: nowrap;
  padding: 8px !important;
}

.action-cell .edit-btn,
.action-cell .delete-btn {
  padding: 4px 8px;
  font-size: 12px;
  margin-right: 4px;
  border-radius: 3px;
}

.selection-info {
  background: linear-gradient(135deg, #e3f2fd 0%, #bbdefb 100%);
  border: 1px solid #2196f3;
  border-radius: 6px;
  padding: 10px 15px;
  margin: 10px 0;
}

.selection-info p {
  margin: 0;
  display: flex;
  align-items: center;
  gap: 8px;
}

.selection-info p::before {
  content: '✓';
  background: #2196f3;
  color: white;
  border-radius: 50%;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: bold;
}

/* Bảng nhân viên rộng với cuộn ngang */
.table-container {
  width: 100%;
  overflow-x: auto;
  overflow-y: visible;
  border: 1px solid #dee2e6;
  border-radius: 6px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  background-color: #ffffff;
}

.employee-detail-table {
  width: 100%;
  min-width: 1600px; /* Đảm bảo bảng có độ rộng tối thiểu */
  border-collapse: collapse;
  font-size: 14px;
  table-layout: fixed; /* Cố định layout để control width tốt hơn */
}

.employee-detail-table th,
.employee-detail-table td {
  padding: 12px 8px;
  text-align: left;
  border-bottom: 1px solid #dee2e6;
  vertical-align: middle;
  white-space: nowrap; /* Không wrap text */
  overflow: hidden;
  text-overflow: ellipsis; /* Hiển thị ... khi text quá dài */
}

.employee-detail-table th {
  background-color: #f8f9fa;
  font-weight: 600;
  color: #495057;
  position: sticky;
  top: 0;
  z-index: 10;
  border-bottom: 2px solid #dee2e6;
}

.employee-detail-table tbody tr:hover {
  background-color: #f5f5f5;
}

.employee-detail-table tbody tr:nth-child(even) {
  background-color: #fafafa;
}

/* Responsive cho bảng */
@media (max-width: 1200px) {
  .table-container {
    margin: 0 -20px; /* Mở rộng ra ngoài container */
  }

  .employees-view {
    padding: 20px 10px;
  }
}
</style>
