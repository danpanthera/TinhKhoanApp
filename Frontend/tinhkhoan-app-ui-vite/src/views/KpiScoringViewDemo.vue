<template>
  <div class="kpi-scoring-container">
    <!-- 📊 Header với thống kê -->
    <div class="scoring-header">
      <h2>🎯 Hệ Thống Chấm Điểm KPI</h2>
      <div class="stats-cards">
        <div class="stat-card">
          <div class="stat-number">{{ statistics.totalTargets || 0 }}</div>
          <div class="stat-label">Tổng KPI</div>
        </div>
        <div class="stat-card">
          <div class="stat-number">{{ statistics.scoredTargets || 0 }}</div>
          <div class="stat-label">Đã chấm</div>
        </div>
        <div class="stat-card">
          <div class="stat-number">{{ statistics.pendingTargets || 0 }}</div>
          <div class="stat-label">Chờ chấm</div>
        </div>
        <div class="stat-card">
          <div class="stat-number">{{ completionPercentage }}%</div>
          <div class="stat-label">Hoàn thành</div>
        </div>
      </div>
    </div>

    <!-- Loading và Error States -->
    <div v-if="loading" class="loading-state">⏳ Đang tải dữ liệu...</div>
    <div v-if="error" class="error-state">❌ {{ error }}</div>

    <div v-if="!loading && !error">
      <!-- 📝 PHẦN 1: KPI Định Tính - Chấm Tay -->
      <div class="scoring-section" v-if="kpiTargets.qualitative?.length > 0">
        <h3>📝 Chỉ Tiêu Định Tính - Chấm Điểm Thủ Công</h3>
        <div class="kpi-grid">
          <div v-for="kpi in kpiTargets.qualitative" :key="kpi.id" class="kpi-card qualitative">
            <div class="kpi-header">
              <h4>{{ kpi.kpiName }}</h4>
              <span class="kpi-badge">{{ kpi.maxScore }} điểm</span>
            </div>
            
            <div class="scoring-form">
              <div class="form-group">
                <label>Kết quả thực hiện (%):</label>
                <input 
                  type="number" 
                  v-model="manualScore"
                  :max="100"
                  :min="0"
                  step="0.1"
                  placeholder="Nhập điểm từ 0-100"
                />
              </div>
              <div class="form-group">
                <label>Ghi chú:</label>
                <textarea 
                  v-model="manualComments"
                  placeholder="Ghi chú (tùy chọn)"
                  rows="2"
                ></textarea>
              </div>
              <button 
                @click="submitManualScoring" 
                :disabled="manualScoringLoading || selectedQualitativeTarget?.id !== kpi.id"
                :class="{ active: selectedQualitativeTarget?.id === kpi.id }"
                class="btn-score"
              >
                {{ manualScoringLoading ? '⏳ Đang lưu...' : '💾 Chấm điểm' }}
              </button>
            </div>
            
            <div class="score-display">
              <div class="score-item">
                <span>Chỉ tiêu:</span>
                <strong>{{ kpi.targetValue }}%</strong>
              </div>
              <div class="score-item">
                <span>Kết quả:</span>
                <strong>{{ kpi.actualValue || 'Chưa chấm' }}{{ kpi.actualValue ? '%' : '' }}</strong>
              </div>
              <div class="score-item">
                <span>Điểm đạt:</span>
                <strong class="score-value">{{ kpi.score ? kpi.score.toFixed(1) : 0 }}/{{ kpi.maxScore }}</strong>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 🧮 PHẦN 2: KPI Định Lượng Tỷ Lệ - Tính Toán -->
      <div class="scoring-section" v-if="kpiTargets.quantitativeRatio?.length > 0">
        <h3>🧮 Chỉ Tiêu Định Lượng Tỷ Lệ - Tính Toán</h3>
        <div class="kpi-grid">
          <div v-for="kpi in kpiTargets.quantitativeRatio" :key="kpi.id" class="kpi-card ratio">
            <div class="kpi-header">
              <h4>{{ kpi.kpiName }}</h4>
              <span class="kpi-badge">{{ kpi.maxScore }} điểm</span>
            </div>
            
            <div class="ratio-form">
              <div class="input-row">
                <label>Tử số:</label>
                <input 
                  type="number" 
                  v-model="numerator"
                  step="0.01"
                  placeholder="Nhập tử số"
                />
              </div>
              <div class="input-row">
                <label>Mẫu số:</label>
                <input 
                  type="number" 
                  v-model="denominator"
                  step="0.01"
                  placeholder="Nhập mẫu số"
                />
              </div>
              <div class="calculation-result" v-if="numerator && denominator">
                <span>Kết quả:</span>
                <strong>{{ ((numerator/denominator) * 100).toFixed(2) }}%</strong>
              </div>
              <button 
                @click="submitRatioCalculation"
                :disabled="ratioCalculationLoading || !numerator || !denominator || selectedRatioTarget?.id !== kpi.id"
                :class="{ active: selectedRatioTarget?.id === kpi.id }"
                class="btn-calculate"
              >
                {{ ratioCalculationLoading ? '⏳ Đang tính...' : '🧮 Tính toán' }}
              </button>
            </div>
            
            <div class="score-display">
              <div class="score-item">
                <span>Chỉ tiêu:</span>
                <strong>{{ kpi.targetValue }}%</strong>
              </div>
              <div class="score-item">
                <span>Thực hiện:</span>
                <strong>{{ kpi.actualValue ? kpi.actualValue.toFixed(2) + '%' : 'Chưa tính' }}</strong>
              </div>
              <div class="score-item">
                <span>Điểm đạt:</span>
                <strong class="score-value">{{ kpi.score ? kpi.score.toFixed(1) : 0 }}/{{ kpi.maxScore }}</strong>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 📊 PHẦN 3: KPI Định Lượng Tuyệt Đối - Import -->
      <div class="scoring-section" v-if="kpiTargets.quantitativeAbsolute?.length > 0">
        <h3>📊 Chỉ Tiêu Định Lượng Tuyệt Đối - Import Dữ Liệu</h3>
        
        <!-- File Upload Section -->
        <div class="import-section">
          <div class="upload-area">
            <input 
              type="file" 
              ref="fileInput"
              @change="handleFileUpload"
              accept=".xlsx,.csv"
              style="display: none"
            />
            <button @click="$refs.fileInput.click()" class="btn-upload">
              📂 Chọn File Excel/CSV
            </button>
            <span class="upload-info">Format: employeeId,kpiDefinitionId,actualValue</span>
          </div>
          
          <!-- Import Preview -->
          <div v-if="importPreview.length > 0" class="import-preview">
            <h4>🔍 Xem Trước Dữ Liệu Import ({{ importPreview.length }} dòng)</h4>
            <div class="preview-table">
              <div v-for="(row, index) in importPreview" :key="index" class="preview-row">
                <span>Employee ID: {{ row.employeeId }}</span>
                <span>KPI ID: {{ row.kpiDefinitionId }}</span>
                <span>Giá trị: {{ row.actualValue }}</span>
              </div>
            </div>
            <div class="import-actions">
              <button @click="submitBulkImport" :disabled="bulkImportLoading" class="btn-confirm">
                {{ bulkImportLoading ? '⏳ Đang import...' : '✅ Xác Nhận Import' }}
              </button>
              <button @click="cancelImport" class="btn-cancel">❌ Hủy</button>
            </div>
          </div>
        </div>

        <!-- Current Absolute KPIs -->
        <div class="kpi-grid">
          <div v-for="kpi in kpiTargets.quantitativeAbsolute" :key="kpi.id" class="kpi-card absolute">
            <div class="kpi-header">
              <h4>{{ kpi.kpiName }}</h4>
              <span class="kpi-badge">{{ kpi.maxScore }} điểm</span>
            </div>
            
            <div class="score-display">
              <div class="score-item">
                <span>Chỉ tiêu:</span>
                <strong>{{ kpi.targetValue.toLocaleString() }} {{ kpi.unit }}</strong>
              </div>
              <div class="score-item">
                <span>Thực hiện:</span>
                <strong>{{ kpi.actualValue ? kpi.actualValue.toLocaleString() : 0 }} {{ kpi.unit }}</strong>
              </div>
              <div class="score-item">
                <span>Điểm đạt:</span>
                <strong class="score-value">{{ kpi.score ? kpi.score.toFixed(1) : 0 }}/{{ kpi.maxScore }}</strong>
              </div>
              <div class="completion-rate">
                <span>Tỷ lệ hoàn thành:</span>
                <strong>{{ kpi.actualValue ? ((kpi.actualValue / kpi.targetValue) * 100).toFixed(1) : 0 }}%</strong>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Demo Controls -->
      <div class="demo-controls">
        <h3>🎮 Demo Controls</h3>
        <div class="demo-buttons">
          <button @click="selectQualitativeKPI(0)" class="btn-demo">Chọn KPI Định Tính #1</button>
          <button @click="selectRatioKPI(0)" class="btn-demo">Chọn KPI Tỷ Lệ #1</button>
          <button @click="loadKpiTargets" class="btn-demo">🔄 Tải lại dữ liệu</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import { isAuthenticated } from '../services/auth';
import { 
  getKpiTargetsByType, 
  submitManualScore, 
  calculateRatioScore, 
  bulkImportScores,
  getScoringStatistics 
} from '../services/kpiScoringService';
import { toast } from 'vue3-toastify';

const router = useRouter();

// Reactive data
const kpiTargets = ref({
  qualitative: [],
  quantitativeRatio: [],
  quantitativeAbsolute: []
});
const statistics = ref({});
const loading = ref(false);
const error = ref(null);

// Manual scoring
const selectedQualitativeTarget = ref(null);
const manualScore = ref('');
const manualComments = ref('');
const manualScoringLoading = ref(false);

// Ratio calculation
const selectedRatioTarget = ref(null);
const numerator = ref('');
const denominator = ref('');
const ratioCalculationLoading = ref(false);

// Bulk import
const importFile = ref(null);
const importPreview = ref([]);
const bulkImportLoading = ref(false);

// Computed properties
const completionPercentage = computed(() => {
  if (!statistics.value.totalTargets) return 0;
  return Math.round((statistics.value.scoredTargets / statistics.value.totalTargets) * 100);
});

const averageScorePercentage = computed(() => {
  return statistics.value.averageScore ? statistics.value.averageScore.toFixed(1) : '0.0';
});

// Methods
const loadKpiTargets = async () => {
  loading.value = true;
  error.value = null;
  
  try {
    const response = await getKpiTargetsByType();
    if (response.success) {
      kpiTargets.value = response.data;
      statistics.value = response.summary;
      toast.success('Tải danh sách KPI thành công');
    } else {
      throw new Error(response.error || 'Failed to load KPI targets');
    }
  } catch (err) {
    error.value = err.message;
    toast.error('Lỗi khi tải danh sách KPI: ' + err.message);
  } finally {
    loading.value = false;
  }
};

const refreshStatistics = async () => {
  try {
    const response = await getScoringStatistics();
    if (response.success) {
      statistics.value = response.data;
    }
  } catch (err) {
    console.error('Error refreshing statistics:', err);
  }
};

const selectQualitativeKPI = (index) => {
  if (kpiTargets.value.qualitative[index]) {
    selectedQualitativeTarget.value = kpiTargets.value.qualitative[index];
    manualScore.value = '';
    manualComments.value = '';
    toast.info(`Đã chọn KPI: ${selectedQualitativeTarget.value.kpiName}`);
  }
};

const selectRatioKPI = (index) => {
  if (kpiTargets.value.quantitativeRatio[index]) {
    selectedRatioTarget.value = kpiTargets.value.quantitativeRatio[index];
    numerator.value = '';
    denominator.value = '';
    toast.info(`Đã chọn KPI: ${selectedRatioTarget.value.kpiName}`);
  }
};

const submitManualScoring = async () => {
  if (!selectedQualitativeTarget.value || !manualScore.value) {
    toast.error('Vui lòng chọn KPI và nhập điểm');
    return;
  }
  
  const score = parseFloat(manualScore.value);
  if (score < 0 || score > 100) {
    toast.error('Điểm phải từ 0 đến 100');
    return;
  }
  
  manualScoringLoading.value = true;
  
  try {
    const response = await submitManualScore(
      selectedQualitativeTarget.value.id, 
      score, 
      manualComments.value
    );
    
    if (response.success) {
      toast.success(response.message);
      // Update local data
      const target = kpiTargets.value.qualitative.find(t => t.id === selectedQualitativeTarget.value.id);
      if (target) {
        target.actualValue = score;
        target.score = response.data.score;
      }
      // Reset form
      selectedQualitativeTarget.value = null;
      manualScore.value = '';
      manualComments.value = '';
      // Refresh statistics
      await refreshStatistics();
    } else {
      throw new Error(response.error);
    }
  } catch (err) {
    toast.error('Lỗi chấm điểm: ' + err.message);
  } finally {
    manualScoringLoading.value = false;
  }
};

const submitRatioCalculation = async () => {
  if (!selectedRatioTarget.value || !numerator.value || !denominator.value) {
    toast.error('Vui lòng chọn KPI và nhập đầy đủ tử số, mẫu số');
    return;
  }
  
  const num = parseFloat(numerator.value);
  const den = parseFloat(denominator.value);
  
  if (isNaN(num) || isNaN(den) || den === 0) {
    toast.error('Tử số và mẫu số phải là số hợp lệ, mẫu số khác 0');
    return;
  }
  
  ratioCalculationLoading.value = true;
  
  try {
    const response = await calculateRatioScore(selectedRatioTarget.value.id, num, den);
    
    if (response.success) {
      toast.success(response.message);
      // Update local data
      const target = kpiTargets.value.quantitativeRatio.find(t => t.id === selectedRatioTarget.value.id);
      if (target) {
        target.numerator = num;
        target.denominator = den;
        target.actualValue = parseFloat(response.data.ratio);
        target.score = parseFloat(response.data.score);
      }
      // Reset form
      selectedRatioTarget.value = null;
      numerator.value = '';
      denominator.value = '';
      // Refresh statistics
      await refreshStatistics();
    } else {
      throw new Error(response.error);
    }
  } catch (err) {
    toast.error('Lỗi tính toán tỷ lệ: ' + err.message);
  } finally {
    ratioCalculationLoading.value = false;
  }
};

const handleFileUpload = (event) => {
  const file = event.target.files[0];
  if (!file) return;
  
  importFile.value = file;
  
  // Simple CSV preview simulation
  const reader = new FileReader();
  reader.onload = (e) => {
    const content = e.target.result;
    const lines = content.split('\n').slice(1, 6); // Preview first 5 data rows
    
    importPreview.value = lines.map((line, index) => {
      const [employeeId, kpiDefinitionId, actualValue] = line.split(',');
      return {
        row: index + 2,
        employeeId: parseInt(employeeId?.trim()),
        kpiDefinitionId: parseInt(kpiDefinitionId?.trim()),
        actualValue: parseFloat(actualValue?.trim())
      };
    }).filter(item => !isNaN(item.employeeId));
    
    toast.info(`Đã tải file: ${file.name}, preview ${importPreview.value.length} dòng`);
  };
  reader.readAsText(file);
};

const submitBulkImport = async () => {
  if (!importFile.value) {
    toast.error('Vui lòng chọn file để import');
    return;
  }
  
  bulkImportLoading.value = true;
  
  try {
    // Use preview data for demo
    const response = await bulkImportScores(importPreview.value);
    
    if (response.success) {
      toast.success(response.message);
      // Update local data
      response.data.successful.forEach(item => {
        const target = kpiTargets.value.quantitativeAbsolute.find(t => t.id === item.targetId);
        if (target) {
          target.actualValue = item.actualValue;
          target.score = item.score;
        }
      });
      // Reset form
      importFile.value = null;
      importPreview.value = [];
      // Refresh statistics
      await refreshStatistics();
    } else {
      throw new Error(response.error);
    }
  } catch (err) {
    toast.error('Lỗi import dữ liệu: ' + err.message);
  } finally {
    bulkImportLoading.value = false;
  }
};

const cancelImport = () => {
  importFile.value = null;
  importPreview.value = [];
  toast.info('Đã hủy import');
};

// Lifecycle
onMounted(async () => {
  if (!isAuthenticated()) {
    router.push('/login');
    return;
  }
  
  await loadKpiTargets();
});
</script>

<style scoped>
.kpi-scoring-container {
  padding: 20px;
  max-width: 1400px;
  margin: 0 auto;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.scoring-header {
  margin-bottom: 30px;
  text-align: center;
}

.scoring-header h2 {
  color: #2c3e50;
  margin-bottom: 20px;
  font-size: 2.2em;
}

.stats-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  margin-top: 20px;
}

.stat-card {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 25px;
  border-radius: 15px;
  text-align: center;
  box-shadow: 0 4px 15px rgba(0,0,0,0.1);
}

.stat-number {
  font-size: 2.5em;
  font-weight: bold;
  margin-bottom: 8px;
}

.stat-label {
  font-size: 1em;
  opacity: 0.9;
}

.loading-state,
.error-state {
  text-align: center;
  padding: 40px;
  font-size: 1.2em;
  border-radius: 10px;
  margin: 20px 0;
}

.loading-state {
  background: #e3f2fd;
  color: #1976d2;
}

.error-state {
  color: #d32f2f;
  background: #ffebee;
}

.scoring-section {
  background: white;
  margin-bottom: 30px;
  border-radius: 15px;
  box-shadow: 0 4px 20px rgba(0,0,0,0.08);
  overflow: hidden;
}

.scoring-section h3 {
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  margin: 0;
  padding: 25px;
  border-bottom: 1px solid #dee2e6;
  font-size: 1.4em;
  color: #495057;
  font-weight: 600;
}

.kpi-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(450px, 1fr));
  gap: 25px;
  padding: 25px;
}

.kpi-card {
  border: 2px solid #e9ecef;
  border-radius: 12px;
  padding: 25px;
  background: #ffffff;
  transition: all 0.3s ease;
}

.kpi-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0,0,0,0.1);
}

.kpi-card.qualitative {
  border-left: 6px solid #28a745;
}

.kpi-card.ratio {
  border-left: 6px solid #007bff;
}

.kpi-card.absolute {
  border-left: 6px solid #ffc107;
}

.kpi-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 20px;
}

.kpi-header h4 {
  margin: 0;
  flex: 1;
  font-size: 1.2em;
  line-height: 1.4;
  color: #2c3e50;
  font-weight: 600;
}

.kpi-badge {
  background: #6c757d;
  color: white;
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 0.85em;
  font-weight: 500;
  margin-left: 15px;
}

.scoring-form,
.ratio-form {
  margin-bottom: 20px;
  padding: 15px;
  background: #f8f9fa;
  border-radius: 8px;
}

.form-group {
  margin-bottom: 15px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: 500;
  color: #495057;
}

.form-group input,
.form-group textarea {
  width: 100%;
  padding: 10px;
  border: 1px solid #ced4da;
  border-radius: 6px;
  font-size: 14px;
  transition: border-color 0.3s ease;
}

.form-group input:focus,
.form-group textarea:focus {
  outline: none;
  border-color: #007bff;
  box-shadow: 0 0 0 0.2rem rgba(0,123,255,.25);
}

.input-row {
  display: flex;
  align-items: center;
  margin-bottom: 15px;
}

.input-row label {
  width: 100px;
  font-weight: 500;
  color: #495057;
}

.input-row input {
  flex: 1;
  padding: 8px 12px;
  border: 1px solid #ced4da;
  border-radius: 6px;
  margin-left: 10px;
}

.calculation-result {
  text-align: center;
  padding: 15px;
  background: #e3f2fd;
  border-radius: 8px;
  margin: 15px 0;
  color: #1976d2;
  font-weight: 500;
}

.btn-score,
.btn-calculate,
.btn-upload,
.btn-confirm,
.btn-cancel,
.btn-demo {
  padding: 12px 20px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.3s ease;
  font-size: 14px;
}

.btn-score,
.btn-calculate {
  width: 100%;
  background: #007bff;
  color: white;
}

.btn-score:hover,
.btn-calculate:hover {
  background: #0056b3;
  transform: translateY(-1px);
}

.btn-score.active,
.btn-calculate.active {
  background: #28a745;
}

.btn-score:disabled,
.btn-calculate:disabled {
  background: #6c757d;
  cursor: not-allowed;
  transform: none;
}

.btn-upload {
  background: #28a745;
  color: white;
  margin-right: 15px;
}

.btn-upload:hover {
  background: #1e7e34;
}

.btn-confirm {
  background: #28a745;
  color: white;
  margin-right: 10px;
}

.btn-cancel {
  background: #dc3545;
  color: white;
}

.btn-demo {
  background: #17a2b8;
  color: white;
  margin: 5px;
}

.score-display {
  background: white;
  padding: 20px;
  border-radius: 8px;
  border: 1px solid #e9ecef;
}

.score-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 12px;
  align-items: center;
}

.score-item span {
  color: #6c757d;
  font-weight: 500;
}

.score-value {
  color: #28a745;
  font-size: 1.2em;
  font-weight: bold;
}

.completion-rate {
  margin-top: 15px;
  padding-top: 15px;
  border-top: 1px solid #e9ecef;
  color: #495057;
}

.import-section {
  padding: 25px;
  background: #f8f9fa;
  margin: 25px;
  border-radius: 10px;
}

.upload-area {
  text-align: center;
  padding: 30px;
  border: 2px dashed #ced4da;
  border-radius: 10px;
  margin-bottom: 25px;
  background: white;
}

.upload-info {
  color: #6c757d;
  font-size: 0.9em;
  display: block;
  margin-top: 10px;
}

.import-preview {
  background: white;
  padding: 20px;
  border-radius: 10px;
  border: 1px solid #dee2e6;
}

.preview-table {
  max-height: 200px;
  overflow-y: auto;
  margin: 15px 0;
}

.preview-row {
  display: flex;
  justify-content: space-between;
  padding: 8px 12px;
  border-bottom: 1px solid #f1f3f4;
  font-size: 0.9em;
}

.preview-row:nth-child(even) {
  background: #f8f9fa;
}

.import-actions {
  text-align: center;
  margin-top: 20px;
}

.demo-controls {
  background: #fff3cd;
  padding: 20px;
  border-radius: 10px;
  margin-top: 30px;
  border: 1px solid #ffeaa7;
}

.demo-controls h3 {
  margin-top: 0;
  color: #856404;
}

.demo-buttons {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

/* Responsive Design */
@media (max-width: 768px) {
  .kpi-grid {
    grid-template-columns: 1fr;
  }
  
  .stats-cards {
    grid-template-columns: repeat(2, 1fr);
  }
  
  .kpi-header {
    flex-direction: column;
    align-items: flex-start;
  }
  
  .kpi-badge {
    margin-left: 0;
    margin-top: 10px;
  }
  
  .input-row {
    flex-direction: column;
    align-items: flex-start;
  }
  
  .input-row input {
    margin-left: 0;
    margin-top: 5px;
  }
  
  .demo-buttons {
    flex-direction: column;
  }
}
</style>
