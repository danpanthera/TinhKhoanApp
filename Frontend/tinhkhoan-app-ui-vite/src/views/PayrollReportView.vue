<template>
  <div class="payroll-report-container">
    <h2>Bảng lương & Báo cáo nhân viên</h2>
    <div class="form-group">
      <label for="khoanPeriod">Chọn kỳ khoán</label>
      <select v-model="selectedPeriod" @change="fetchPayroll">
        <option v-for="period in khoanPeriods" :key="period.id" :value="period.id">
          {{ period.name }}
        </option>
      </select>
      <label for="unitFilter">Đơn vị</label>
      <select v-model="unitFilter">
        <option value="">Tất cả</option>
        <option v-for="unit in units" :key="unit.id" :value="unit.id">{{ unit.name }}</option>
      </select>
      <input
        v-model="search"
        placeholder="Tìm kiếm tên/mã NV..."
        style="padding: 4px 8px; border-radius: 4px; border: 1px solid #ccc; min-width: 180px"
      />
      <button @click="exportExcel" :disabled="!selectedPeriod">Xuất Excel</button>
      <button @click="exportPdf" :disabled="!selectedPeriod">Xuất PDF</button>
    </div>
    <div v-if="loading" class="loading">Đang tải dữ liệu...</div>
    <div v-if="error" class="error">{{ error }}</div>
    <div class="responsive-table">
      <table v-if="pagedPayroll.length" class="payroll-table">
        <thead>
          <tr>
            <th>Mã NV</th>
            <th>Họ tên</th>
            <th>Tổng điểm</th>
            <th>Lương</th>
            <th>V1</th>
            <th>V2</th>
            <th>Hệ số hoàn thành</th>
            <th>Chi tiết</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="row in pagedPayroll" :key="row.id">
            <td>{{ row.employeeCode }}</td>
            <td>{{ row.fullName }}</td>
            <td>{{ row.totalScore }}</td>
            <td>{{ row.totalAmount }}</td>
            <td>{{ row.v1 }}</td>
            <td>{{ row.v2 }}</td>
            <td>{{ row.completionFactor }}</td>
            <td><button @click="showDetail(row.id)">Xem</button></td>
          </tr>
        </tbody>
      </table>
      <div v-if="!pagedPayroll.length && !loading">Không có dữ liệu bảng lương.</div>
    </div>
    <div class="pagination" v-if="totalPages > 1">
      <button @click="prevPage" :disabled="page === 1">&lt;</button>
      <span>Trang {{ page }}/{{ totalPages }}</span>
      <button @click="nextPage" :disabled="page === totalPages">&gt;</button>
    </div>
    <EmployeeDetailModal v-if="showModal" :employee="selectedEmployee" @close="showModal = false" />
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import api from '../services/api'
import { useUnitStore } from '../stores/unitStore'
import { useEmployeeStore } from '../stores/employeeStore'
import EmployeeDetailModal from '../components/EmployeeDetailModal.vue'
import { toast } from 'vue3-toastify'

const khoanPeriods = ref([])
const selectedPeriod = ref(null)
const payroll = ref([])
const loading = ref(false)
const error = ref('')
const search = ref('')
const unitFilter = ref('')
const page = ref(1)
const pageSize = 10
const showModal = ref(false)
const selectedEmployee = ref(null)

const unitStore = useUnitStore()
const employeeStore = useEmployeeStore()
const units = computed(() => unitStore.allUnits)

const fetchPeriods = async () => {
  try {
    const res = await api.get('/KhoanPeriods')
    khoanPeriods.value = res.data
    if (khoanPeriods.value.length) {
      selectedPeriod.value = khoanPeriods.value[0].id
      fetchPayroll()
    }
  } catch (e) {
    error.value = 'Không lấy được danh sách kỳ khoán.'
  }
}

const fetchPayroll = async () => {
  if (!selectedPeriod.value) return
  loading.value = true
  error.value = ''
  try {
    const res = await api.get(`/Reports/employee-summary`, {
      params: { khoanPeriodId: selectedPeriod.value },
    })
    payroll.value = res.data.map(row => ({
      id: row.id,
      employeeCode: row.employeeCode,
      fullName: row.fullName,
      totalScore: row.totalScore,
      totalAmount: row.totalAmount,
      v1: row.v1,
      v2: row.v2,
      completionFactor: row.completionFactor,
      unitId: row.unitId, // cần backend trả về unitId nếu muốn lọc theo đơn vị
    }))
    page.value = 1
  } catch (e) {
    error.value = 'Không lấy được dữ liệu bảng lương.'
    payroll.value = []
  } finally {
    loading.value = false
  }
}

const filteredPayroll = computed(() => {
  let result = payroll.value
  if (search.value) {
    const s = search.value.toLowerCase()
    result = result.filter(
      row => row.fullName?.toLowerCase().includes(s) || row.employeeCode?.toLowerCase().includes(s)
    )
  }
  if (unitFilter.value) {
    result = result.filter(row => String(row.unitId) === String(unitFilter.value))
  }
  return result
})

const totalPages = computed(() => Math.ceil(filteredPayroll.value.length / pageSize))
const pagedPayroll = computed(() => {
  const start = (page.value - 1) * pageSize
  return filteredPayroll.value.slice(start, start + pageSize)
})

function prevPage() {
  if (page.value > 1) page.value--
}
function nextPage() {
  if (page.value < totalPages.value) page.value++
}

function showDetail(empId) {
  const emp = employeeStore.allEmployees.find(e => e.id === empId)
  if (emp) {
    selectedEmployee.value = emp
    showModal.value = true
  }
}

const exportExcel = () => {
  if (!selectedPeriod.value) return
  toast.info('Đang xuất file Excel...')
  window.open(
    `${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5055/api'}/Reports/employee-summary-excel?khoanPeriodId=${selectedPeriod.value}`
  )
}
const exportPdf = () => {
  if (!selectedPeriod.value) return
  toast.info('Đang xuất file PDF...')
  window.open(
    `${import.meta.env.VITE_API_BASE_URL || 'http://localhost:5055/api'}/Reports/employee-summary-pdf?khoanPeriodId=${selectedPeriod.value}`
  )
}

onMounted(async () => {
  await unitStore.fetchUnits()
  await employeeStore.fetchEmployees()
  await fetchPeriods()
})
</script>

<style scoped>
.payroll-report-container {
  max-width: 900px;
  margin: 40px auto;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
  padding: 32px 24px;
}
.form-group {
  margin-bottom: 18px;
  display: flex;
  align-items: center;
  gap: 12px;
}
.payroll-table {
  width: 100%;
  border-collapse: collapse;
  margin-bottom: 18px;
}
.payroll-table th,
.payroll-table td {
  border: 1px solid #e0e0e0;
  padding: 8px 10px;
  text-align: center;
}
.payroll-table th {
  background: #f5f7fa;
}
button {
  background: #2d8cf0;
  color: #fff;
  border: none;
  border-radius: 4px;
  padding: 6px 16px;
  font-weight: bold;
  cursor: pointer;
}
button:disabled {
  background: #b3d8fd;
}
.loading {
  color: #888;
  margin: 12px 0;
  text-align: center;
}
.error {
  color: #e74c3c;
  margin: 12px 0;
  text-align: center;
}
.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 8px;
  margin-top: 16px;
}
.responsive-table {
  overflow-x: auto;
}
</style>
