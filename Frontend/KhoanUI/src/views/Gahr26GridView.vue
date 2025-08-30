<template>
  <div class="gahr26-grid-view">
    <h1>GAHR26 Grid (Excel style)</h1>
    <div class="toolbar">
      <input
        ref="fileInputRef"
        type="file"
        accept=".xls,.xlsx"
        @change="onFileChange"
      >
      <button
        class="action-button"
        :disabled="isLoading"
        @click="triggerSelect"
      >
        Chọn File GAHR26
      </button>
      <input
        v-model="globalSearch"
        class="global-search"
        type="text"
        placeholder="Tìm nhanh..."
      >
      <label class="freeze-label">
        Freeze:
        <select v-model.number="freezeCount">
          <option :value="0">0</option>
          <option :value="1">1</option>
          <option :value="2">2</option>
          <option :value="3">3</option>
          <option :value="4">4</option>
          <option :value="5">5</option>
        </select>
      </label>
      <button
        class="action-button"
        :disabled="rows.length===0"
        @click="exportXlsx"
      >
        Xuất Excel
      </button>
      <button
        class="action-button"
        @click="addRow"
      >
        + Thêm dòng
      </button>
      <button
        class="action-button danger"
        :disabled="selectedRows.size===0"
        @click="deleteSelected"
      >
        Xóa dòng đã chọn
      </button>
      <button
        class="action-button success"
        :disabled="!canSave || saving"
        @click="saveAll"
      >
        {{ saving? 'Đang lưu...' : 'Lưu vào hệ thống' }}
      </button>
      <span v-if="parseErrors.length" class="errors">{{ parseErrors.length }} lỗi</span>
    </div>
    <div v-if="validationErrors.length" class="warn-box">
      <div
        v-for="e in validationErrors"
        :key="e"
      >
        ❗ {{ e }}
      </div>
    </div>
    <div v-if="parseErrors.length" class="error-box">
      <div
        v-for="e in parseErrors"
        :key="e"
      >
        ⚠️ {{ e }}
      </div>
    </div>
    <div
      v-if="columns.length"
      class="grid-wrapper"
    >
      <table class="excel-grid">
        <thead>
          <tr>
            <th class="sticky-col sel-col">
              <input type="checkbox" :checked="allSelected" @change="toggleSelectAll">
            </th>
            <th
              v-for="(col, cIdx) in columns"
              :key="col.key"
              :class="['col-header', appendedKeys.includes(col.key)?'appended':'', isFrozen(cIdx)?'frozen-col':'']"
              :style="frozenStyle(cIdx)"
            >
              <div class="col-title">
                {{ col.label }}
              </div>
              <button
                class="mini-btn"
                @click="sortBy(col.key)"
              >
                ⇅
              </button>
            </th>
          </tr>
          <tr class="filter-row">
            <th class="sticky-col sel-col" />
            <th
              v-for="(col, cIdx) in columns"
              :key="col.key + '_flt'"
              :class="['col-header filter-cell', isFrozen(cIdx)?'frozen-col':'']"
              :style="frozenStyle(cIdx)"
            >
              <input
                v-model="columnFilters[col.key]"
                class="col-filter"
                :placeholder="'Lọc ' + col.label"
              >
            </th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="row in filteredRows"
            :key="row.__id"
            :class="{ 'selected-row': selectedRows.has(row.__id) }"
          >
            <td class="sticky-col sel-col">
              <input
                v-model="selectionProxy"
                type="checkbox"
                :value="row.__id"
              >
            </td>
            <td
              v-for="(col, cIdx) in columns"
              :key="col.key"
              :class="['cell', appendedKeys.includes(col.key)?'appended':'', isFrozen(cIdx)?'frozen-col':'', invalidCell(row, col.key)?'invalid':'' ]"
              :style="frozenStyle(cIdx)"
            >
              <input
                v-model="row[col.key]"
                :class="{ dirty: row.__dirty }"
                :placeholder="col.label"
                @input="onCellInput(row, col.key)"
              >
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <p
      v-else
      class="hint"
    >
      Hãy chọn file GAHR26 (.xls / .xlsx) để tải dữ liệu.
    </p>
  </div>
</template>

<script setup>
import { computed, nextTick, reactive, ref, watch } from 'vue'

const BASE_COLUMNS = [
  'BRCD','BRNM','DEPTNM','EMPNM','EMPNO','POSITION','TITLE','GRPTITLE','NEWOCCP','SEX','BIRTHDT','BPPROV','BPADDR','NPPROV','NPADDR','PRPROV','PRADDR','TRPROV','TRADDR','NATLITY','RACE','RELIGION','FMLYBG','FMLYPRTY','SLFPRTY','SNIRDT','BKINDSDT','AGBKDT','AGBKEMPDT','CURWKDT','J_JOINDT','J_JOINORG','J_PTYDT','J_PTYOFDT','D_EDULVL','D_MPRLVL','D_MPOLVL','D_MFOLVL','D_MFOLVL_1','D_EFTDT','D_EXPDT','D_MARK','D_MCOLVL','STRNPT','LONGJOB','IDNO','ISSUDT','ISSUEPL','ACADEMIC','V1_SCALE','V1_LVL','V1_FACTOR','POS_AMT','V1_EFTDT','V1_RAISEDT','V2_SCALE','V2_LVL','V2_FACTOR','V2_EFTDT','V2_RAISEDT','PO','AR','HO','HA','RE','OT','AD','RD','MT','HSDCV1','HSDCV2','A_EFTDT','A_NXTPROMDT','A_DCSNNO','A_SGNDT','L_CTRTKND','L_CTRTNO','L_EFTDT','L_SGNDT','J_INSURNO','J_INSISSUDT','J_CUSTCD','J_ACCTNO','J_BANKNM','HOMETEL','MOBILE','OFITEL','R_ARMYRNK','R_POS','R_OFFICE',
]
const APPENDED = ['UserAD','Email','UserIPCAS','MaCBTD']

const fileInputRef = ref(null)
const rows = reactive([])
const columns = ref([])
const parseErrors = ref([])
const isLoading = ref(false)
const selectedRows = reactive(new Set())
const sortState = reactive({ key: null, dir: 1 })
const saving = ref(false)
const globalSearch = ref('')
const columnFilters = reactive({})
const freezeCount = ref(1)
const validationErrors = ref([])
let validateTimer = null

const appendedKeys = APPENDED

function triggerSelect(){
  if (fileInputRef.value) fileInputRef.value.click()
}

function buildColumns(keys){
  const final = [...BASE_COLUMNS]
  keys.forEach(k=>{ if(!final.includes(k) && !APPENDED.includes(k)) final.push(k) })
  APPENDED.forEach(k=> final.push(k))
  columns.value = final.map(k=>({ key: k, label: k }))
  // init column filters
  final.forEach(k=>{ if(!(k in columnFilters)) columnFilters[k] = '' })
  nextTick(()=> recomputeFrozenOffsets())
}

function normalizeRow(obj){
  const r = {}
  columns.value.forEach(c=>{ r[c.key] = obj[c.key] ?? '' })
  r.__id = crypto.randomUUID()
  r.__dirty = false
  return r
}

async function onFileChange(e){
  const file = e.target.files[0]
  if(!file) return
  parseErrors.value = []
  rows.splice(0, rows.length)
  try{
    isLoading.value = true
    const XLSX = await import('xlsx')
    const data = await file.arrayBuffer()
    const wb = XLSX.read(data, { type:'array' })
    const ws = wb.Sheets[wb.SheetNames[0]]
    const json = XLSX.utils.sheet_to_json(ws, { defval:'', raw:true })
    if(json.length === 0){
      parseErrors.value.push('File không có dữ liệu.')
      return
    }
    const keys = Object.keys(json[0])
    buildColumns(keys)
    json.forEach(o=> rows.push(normalizeRow(o)))
  }catch(err){
    parseErrors.value.push('Lỗi đọc file: ' + err.message)
    console.error(err)
  }finally{
    isLoading.value = false
    e.target.value=''
  }
}

function markDirty(r){ r.__dirty = true }

function addRow(){
  if(columns.value.length === 0){ buildColumns([]) }
  const blank = {}
  columns.value.forEach(c=> blank[c.key]='')
  blank.__id = crypto.randomUUID()
  blank.__dirty = true
  rows.push(blank)
  triggerValidation()
}

const selectionProxy = computed({
  get(){ return Array.from(selectedRows) },
  set(vals){
    selectedRows.clear()
    vals.forEach(v=> selectedRows.add(v))
  },
})

const allSelected = computed(()=> rows.length>0 && selectedRows.size===rows.length)
function toggleSelectAll(){
  if(allSelected.value) selectedRows.clear()
  else { selectedRows.clear(); rows.forEach(r=> selectedRows.add(r.__id)) }
}

function deleteSelected(){
  for(let i=rows.length-1;i>=0;i--){ if(selectedRows.has(rows[i].__id)) rows.splice(i,1) }
  selectedRows.clear()
  triggerValidation()
}

function sortBy(key){
  if(sortState.key === key) sortState.dir*=-1; else { sortState.key = key; sortState.dir = 1 }
  rows.sort((a,b)=>{
    const av = a[key]??''; const bv = b[key]??''
    if(av===bv) return 0
    return av>bv ? sortState.dir : -sortState.dir
  })
}

async function exportXlsx(){
  const XLSX = await import('xlsx')
  const plain = rows.map(r=>{
    const o = {}
    columns.value.forEach(c=>{ o[c.key]=r[c.key] })
    return o
  })
  const ws = XLSX.utils.json_to_sheet(plain)
  const wb = XLSX.utils.book_new()
  XLSX.utils.book_append_sheet(wb, ws, 'GAHR26')
  XLSX.writeFile(wb, 'gahr26_export.xlsx')
}

// Filtering
const filteredRows = computed(()=>{
  const g = globalSearch.value.trim().toLowerCase()
  return rows.filter(r=>{
    if(g){
      const joined = columns.value.map(c=> (r[c.key]??'').toString().toLowerCase()).join('\u0001')
      if(!joined.includes(g)) return false
    }
    for(const c of columns.value){
      const fv = (columnFilters[c.key]||'').trim().toLowerCase()
      if(fv){
        const cell = (r[c.key]??'').toString().toLowerCase()
        if(!cell.includes(fv)) return false
      }
    }
    return true
  })
})

// Validation
function validate(){
  validationErrors.value = []
  const cbSeen = new Set()
  rows.forEach((r, idx)=>{
    const empCode = (r.EMPNO || r.EmployeeCode || '').toString().trim()
    if(!empCode){ validationErrors.value.push(`Dòng ${idx+1}: Thiếu EMPNO (Mã NV)`); }
    const cb = (r.CBCODE || r.CBCode || r.J_CUSTCD || '').toString().trim()
    if(cb){
      if(!/^\d{9}$/.test(cb)) validationErrors.value.push(`Dòng ${idx+1}: CBCode phải 9 chữ số`)
      if(cbSeen.has(cb)) validationErrors.value.push(`Dòng ${idx+1}: Trùng CBCode ${cb}`)
      cbSeen.add(cb)
    }
    const email = (r.Email || '').toString().trim()
    if(email && !/.+@.+\..+/.test(email)) validationErrors.value.push(`Dòng ${idx+1}: Email không hợp lệ`)
  })
}
function triggerValidation(){
  if(validateTimer) clearTimeout(validateTimer)
  validateTimer = setTimeout(()=> validate(), 300)
}
function onCellInput(row, _key){
  markDirty(row)
  triggerValidation()
}
function invalidCell(row, key){
  if(key==='Email'){ const v=(row[key]||'').trim(); return v && !/.+@.+\..+/.test(v) }
  if(key==='CBCode' || key==='CBCODE' || key==='J_CUSTCD'){ const v=(row[key]||'').trim(); return v && !/^\d{9}$/.test(v) }
  return false
}

// Freeze columns logic
const frozenOffsets = ref([]) // px offsets for first N columns
function recomputeFrozenOffsets(){
  frozenOffsets.value = []
  const table = document.querySelector('.excel-grid')
  if(!table) return
  const ths = table.querySelectorAll('thead tr:first-child th')
  let acc = 32 // width of selection col
  for(let i=0;i<freezeCount.value;i++){
    const th = ths[i+1] // +1 skip selection
    if(!th) break
    frozenOffsets.value[i] = acc
    acc += th.offsetWidth
  }
}
function isFrozen(idx){ return idx < freezeCount.value }
function frozenStyle(idx){
  if(!isFrozen(idx)) return null
  const left = frozenOffsets.value[idx] || 32 * (idx+1)
  return { left: left + 'px', position:'sticky', zIndex: 2 }
}
watch(freezeCount, ()=> nextTick(()=> recomputeFrozenOffsets()))
window.addEventListener('resize', ()=> recomputeFrozenOffsets())

// Save (bulk import)
const canSave = computed(()=> rows.length>0 && validationErrors.value.length===0)
function mapRowToDto(r){
  const employeeCode = (r.EMPNO || r.EmployeeCode || '').toString().trim()
  const cbCode = (r.CBCODE || r.CBCode || r.J_CUSTCD || '').toString().trim() || null
  const fullName = (r.EMPNM || r.FullName || '').toString().trim() || null
  const username = (r.Username || employeeCode).toString().trim() || null
  const email = (r.Email || '').toString().trim() || null
  const phone = (r.MOBILE || r.HOMETEL || r.OFITEL || r.PhoneNumber || '').toString().trim() || null
  return {
    employeeCode,
    cbCode: cbCode || null,
    fullName,
    username,
    email,
    phoneNumber: phone || null,
    userAD: (r.UserAD||'').trim()||null,
    userIPCAS: (r.UserIPCAS||'').trim()||null,
    maCBTD: (r.MaCBTD||'').trim()||null,
  }
}
async function saveAll(){
  triggerValidation(); validate();
  if(validationErrors.value.length){ return }
  saving.value = true
  try{
    const payload = {
      rows: rows.map(r=>{
        const dto = mapRowToDto(r)
        return {
          EmployeeCode: dto.employeeCode,
            CBCode: dto.cbCode,
            FullName: dto.fullName,
            Username: dto.username,
            Email: dto.email,
            PhoneNumber: dto.phoneNumber,
            UserAD: dto.userAD,
            UserIPCAS: dto.userIPCAS,
            MaCBTD: dto.maCBTD,
        }
      }),
      overwriteExisting: true,
      autoGenerateMissingUsernames: true,
    }
    const res = await fetch('/api/Gahr26Import/bulk-employees', {
      method:'POST',
      headers:{ 'Content-Type':'application/json' },
      body: JSON.stringify(payload),
    })
    if(!res.ok){
      const txt = await res.text()
      alert('Lưu thất bại: '+ txt)
      return
    }
    const data = await res.json()
    alert(`Lưu thành công. Thêm mới: ${data.inserted}, Cập nhật: ${data.updated}, Bỏ qua: ${data.skipped}`)
    rows.forEach(r=> r.__dirty=false)
  }catch(err){
    console.error(err)
    alert('Lỗi khi lưu: '+ err.message)
  }finally{
    saving.value = false
  }
}
// Initial validation after first load
watch(rows, ()=> triggerValidation())
</script>

<style scoped>
.gahr26-grid-view { max-width: 100%; padding: 16px; font-family: var(--font-primary, Arial, sans-serif); }
h1 { margin: 4px 0 16px; font-size: 20px; }
.toolbar { display: flex; flex-wrap: wrap; gap: 8px; align-items: center; margin-bottom: 12px; }
.action-button { background:#3498db; color:#fff; border:none; padding:6px 14px; border-radius:4px; cursor:pointer; font-size:13px; }
.action-button:hover { background:#2176b5; }
.action-button.danger { background:#e74c3c; }
.action-button.danger:hover { background:#c0392b; }
.action-button.success { background:#27ae60; }
.action-button.success:hover { background:#1e8c4c; }
.grid-wrapper { overflow:auto; border:1px solid #d0d7de; max-height: 70vh; }
table.excel-grid { border-collapse: separate; border-spacing:0; min-width:1600px; font-size:12px; }
table.excel-grid thead th { position: sticky; top:0; background:#f3f6fa; z-index:2; border-bottom:1px solid #b6c2cd; padding:4px 6px; text-align:left; font-weight:600; white-space:nowrap; }
th.col-header { position: sticky; top:0; }
th.sel-col { width:32px; text-align:center; }
.sticky-col { position: sticky; left:0; background:#f3f6fa; z-index:3; box-shadow: 2px 0 2px -1px rgba(0,0,0,0.1); }
tbody .sticky-col { background:#fff; }
tbody tr:hover { background:#f5fbff; }
tbody tr.selected-row { background:#e3f4ff; }
td.cell, th.col-header { border-right:1px solid #e1e7ec; }
td.cell { padding:0; min-width:120px; }
td.cell input { width:100%; border:none; padding:4px 6px; font-size:12px; background:transparent; outline:none; }
td.cell input:focus { background:#fffceb; box-shadow: inset 0 0 0 1px #f1c40f; }
td.cell input.dirty { background:#fdf7e3; }
.invalid input { background:#ffecec !important; }
.col-header .col-title { display:inline-block; margin-right:4px; }
.mini-btn { font-size:10px; padding:2px 4px; border:1px solid #ccd2d8; background:#fff; cursor:pointer; border-radius:3px; }
.mini-btn:hover { background:#eef2f6; }
.appended { background:#f9f2ff; }
.errors { color:#c0392b; font-weight:600; }
.error-box { background:#fff5f5; border:1px solid #f5b7b1; padding:8px 10px; margin-bottom:10px; font-size:12px; max-height:140px; overflow:auto; }
.hint { color:#555; font-style:italic; }
.global-search { padding:4px 8px; border:1px solid #ccd2d8; border-radius:4px; font-size:13px; }
.col-filter { width:100%; border:1px solid #dde2e6; padding:2px 4px; font-size:11px; }
.filter-row th { top:32px; background:#fafbfd; z-index:2; }
.frozen-col { background:#fffbe9; box-shadow: 2px 0 2px -1px rgba(0,0,0,0.08); }
.warn-box { background:#fffaf0; border:1px solid #f9d29d; padding:6px 8px; margin:8px 0; font-size:12px; max-height:120px; overflow:auto; }
.freeze-label { display:flex; align-items:center; gap:4px; font-size:12px; }
</style>
