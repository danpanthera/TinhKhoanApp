<template>
  <div class="kpi-definitions kpi-definitions-view b2-screen">
    <div class="header-section">
      <h1>⚙️ Cấu hình KPI</h1>
      <p class="subtitle">Quản lý các bảng giao khoán KPI và chỉ tiêu tương ứng</p>
    </div>

    <!-- Error Message -->
    <div v-if="errorMessage" class="error-message">
      <p>{{ errorMessage }}</p>
    </div>

    <!-- Success Message -->
    <div v-if="successMessage" class="success-message">
      <p>{{ successMessage }}</p>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="loading-section">
      <div class="loading-spinner"></div>
      <p>Đang tải dữ liệu...</p>
    </div>

    <!-- Tabs Navigation -->
    <div v-if="!loading" class="tabs-container">
      <div class="tab-navigation">
        <button
          :class="['tab-button', { active: activeTab === 'employee' }]"
          @click="switchTab('employee')"
        >
          👥 Dành cho Cán bộ
        </button>
        <button
          :class="['tab-button', { active: activeTab === 'branch' }]"
          @click="switchTab('branch')"
        >
          🏢 Dành cho Chi nhánh
        </button>
        <button
          :class="['tab-button', { active: activeTab === 'scoring' }]"
          @click="switchTab('scoring')"
        >
          ⚡ Điểm tăng giảm Chỉ tiêu Chi nhánh
        </button>
      </div>
    </div>

    <!-- Main Content Layout - Chia đôi màn hình -->
    <div v-if="!loading && (activeTab === 'employee' || activeTab === 'branch')" class="main-layout">
      <!-- Left Panel - Dropdown chọn bảng KPI -->
      <div class="left-panel">
        <div class="table-selector-container">
          <h2 v-if="activeTab === 'employee'">📋 Chọn Bảng KPI Cán bộ</h2>
          <h2 v-if="activeTab === 'branch'">🏢 Chọn Bảng KPI Chi nhánh</h2>

          <div v-if="filteredKpiTables.length === 0" class="no-data">
            <p v-if="activeTab === 'employee'">Chưa có bảng giao khoán KPI nào cho cán bộ.</p>
            <p v-if="activeTab === 'branch'">Chưa có bảng giao khoán KPI nào cho chi nhánh.</p>
          </div>

          <div v-else class="table-dropdown-section">
            <!-- Dropdown chọn bảng -->
            <div class="dropdown-group">
              <label for="tableSelect">Bảng giao khoán KPI:</label>
              <select
                id="tableSelect"
                v-model="selectedTableId"
                @change="onTableChange"
                class="table-dropdown"
              >
                <option value="">-- Chọn bảng KPI --</option>
                <option
                  v-for="table in filteredKpiTables"
                  :key="table.Id || table.Id"
                  :value="table.Id || table.Id"
                >
                  {{ cleanTableDescription(table.Description || table.description || table.TableName || table.tableName) }} ({{ getIndicatorCount(table.Id || table.Id) }} chỉ tiêu)
                </option>
              </select>
            </div>

            <!-- Thông tin bảng đã chọn -->
            <div v-if="selectedTable" class="selected-table-info">
              <h3>{{ cleanTableDescription(selectedTable.Description || selectedTable.description || selectedTable.TableName || selectedTable.tableName) }}</h3>

              <div class="table-details">
                <div class="detail-item">
                  <span class="label">Loại:</span>
                  <span class="value">
                    {{ getTableTypeName(selectedTable.TableType || selectedTable.tableType, selectedTable.TableName || selectedTable.tableName, selectedTable.Description || selectedTable.description) }}
                    <span class="table-code">{{ selectedTable.TableName || selectedTable.tableName }}</span>
                  </span>
                </div>
                <div class="detail-item">
                  <span class="label">Trạng thái:</span>
                  <span class="value" :class="{ active: (selectedTable.IsActive !== undefined ? selectedTable.IsActive : selectedTable.isActive), inactive: !(selectedTable.IsActive !== undefined ? selectedTable.IsActive : selectedTable.isActive) }">
                    {{ (selectedTable.IsActive !== undefined ? selectedTable.IsActive : selectedTable.isActive) ? 'Hoạt động' : 'Không hoạt động' }}
                  </span>
                </div>
                <div class="detail-item">
                  <span class="label">Số chỉ tiêu:</span>
                  <span class="value indicator-count">{{ getIndicatorCount(selectedTable.Id || selectedTable.Id) }}</span>
                </div>
                <div class="detail-item">
                  <span class="label">Ngày tạo:</span>
                  <span class="value">{{ formatDate(selectedTable.CreatedDate || selectedTable.createdDate) }}</span>
                </div>
              </div>

              <div class="description-box">
                <label>Mô tả:</label>
                <p>{{ selectedTable.Description || selectedTable.description }}</p>
              </div>

              <!-- Nút refresh -->
              <button
                @click="loadTableDetails"
                :disabled="loadingDetails"
                class="refresh-button"
              >
                {{ loadingDetails ? '🔄 Đang tải...' : '🔄 Refresh' }}
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Right Panel - Form chỉ tiêu KPI -->
      <div class="right-panel">
        <div v-if="!selectedTable" class="no-selection">
          <div class="empty-state">
            <div class="empty-icon">📊</div>
            <h3>Chọn bảng KPI để bắt đầu</h3>
            <p>Vui lòng chọn một bảng giao khoán KPI từ dropdown bên trái để xem và chỉnh sửa các chỉ tiêu.</p>
          </div>
        </div>

        <div v-else class="indicators-panel">
          <div class="indicators-header">
            <h2>⚡ Chỉ tiêu KPI - {{ (selectedTable.Description || selectedTable.description || selectedTable.TableName || selectedTable.tableName) }}</h2>
            <button
              @click="openAddIndicatorModal"
              class="action-button add-btn"
            >
              ➕ Thêm chỉ tiêu
            </button>
          </div>

          <!-- Có chỉ tiêu -->
          <div v-if="indicators.length > 0" class="indicators-content">
            <div class="indicators-table">
              <table class="kpi-table">
                <thead>
                  <tr>
                    <th>STT</th>
                    <th>Tên chỉ tiêu</th>
                    <th>Điểm tối đa</th>
                    <th>Đơn vị</th>
                    <th>Thứ tự</th>
                    <th>Trạng thái</th>
                    <th>Thao tác</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(indicator, index) in indicators" :key="getId(indicator)">
                    <td>{{ index + 1 }}</td>
                    <td class="indicator-name kpi-name">{{ safeGet(indicator, 'IndicatorName') }}</td>
                    <td class="max-score kpi-score">{{ safeGet(indicator, 'MaxScore') }}</td>
                    <td class="unit kpi-unit">{{ safeGet(indicator, 'Unit') }}</td>
                    <td class="order kpi-number">{{ safeGet(indicator, 'OrderIndex') }}</td>
                    <td class="status">
                      <span :class="{ active: safeGet(indicator, 'IsActive'), inactive: !safeGet(indicator, 'IsActive') }">
                        {{ safeGet(indicator, 'IsActive') ? 'Hoạt động' : 'Không hoạt động' }}
                      </span>
                    </td>
                    <td class="actions">
                      <button
                        @click="openEditIndicatorModal(indicator)"
                        class="action-btn edit-btn"
                        title="Chỉnh sửa"
                      >
                        ✏️
                      </button>
                      <button
                        @click="deleteIndicator(indicator)"
                        class="action-btn delete-btn"
                        title="Xóa"
                      >
                        🗑️
                      </button>
                      <button
                        @click="moveIndicatorUp(indicator)"
                        :disabled="indicator.orderIndex === 1"
                        class="action-btn move-btn"
                        title="Lên trên"
                      >
                        ⬆️
                      </button>
                      <button
                        @click="moveIndicatorDown(indicator)"
                        :disabled="indicator.orderIndex === indicators.length"
                        class="action-btn move-btn"
                        title="Xuống dưới"
                      >
                        ⬇️
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <!-- Summary with Score Validation -->
            <div class="summary-section">
              <div class="summary-stats">
                <div class="stat-item">
                  <span class="stat-label">Tổng chỉ tiêu:</span>
                  <span class="stat-value">{{ indicators.length }}</span>
                </div>
                <div class="stat-item">
                  <span class="stat-label">Hoạt động:</span>
                  <span class="stat-value">{{ activeIndicatorsCount }}</span>
                </div>
                <div class="stat-item">
                  <span class="stat-label">Tổng điểm:</span>
                  <span class="stat-value" :class="getScoreValidationClass(totalMaxScore)">
                    {{ totalMaxScore }}
                    <span class="validation-icon">{{ getScoreValidationIcon(totalMaxScore) }}</span>
                  </span>
                </div>
              </div>
              <!-- Score Validation Message -->
              <div v-if="totalMaxScore !== 100 && indicators.length > 0" class="score-validation-message">
                <div class="validation-warning">
                  <span class="warning-icon">⚠️</span>
                  <span class="warning-text">
                    Tổng điểm cần bằng 100. Hiện tại: {{ totalMaxScore }}/100
                    ({{ totalMaxScore > 100 ? 'thừa' : 'thiếu' }} {{ Math.abs(100 - totalMaxScore) }} điểm)
                  </span>
                </div>
              </div>
              <div v-else-if="totalMaxScore === 100 && indicators.length > 0" class="score-validation-message">
                <div class="validation-success">
                  <span class="success-icon">✅</span>
                  <span class="success-text">Tổng điểm hợp lệ (100 điểm)</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Chưa có chỉ tiêu -->
          <div v-else-if="!loadingDetails" class="no-indicators">
            <div class="empty-state">
              <div class="empty-icon">📊</div>
              <h3>Chưa có chỉ tiêu KPI</h3>
              <p>Bảng này chưa có chỉ tiêu KPI nào được định nghĩa.</p>
              <button
                @click="openAddIndicatorModal"
                class="action-button add-btn"
              >
                ➕ Thêm chỉ tiêu đầu tiên
              </button>
            </div>
          </div>

          <!-- Loading state -->
          <div v-if="loadingDetails" class="loading-indicators">
            <div class="loading-spinner"></div>
            <p>Đang tải chỉ tiêu...</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Scoring Rules Tab Content -->
    <div v-if="!loading && activeTab === 'scoring'" class="scoring-rules-content">
      <div class="scoring-rules-header">
        <h2>⚡ Cấu hình Điểm tăng giảm Chỉ tiêu Chi nhánh</h2>
        <p class="subtitle">Quản lý quy tắc tính điểm dựa trên tỷ lệ hoàn thành chỉ tiêu</p>

        <div class="scoring-actions">
          <button @click="openAddScoringRuleModal" class="action-button add-btn">
            ➕ Thêm quy tắc tính điểm
          </button>
          <button @click="loadScoringRules" class="action-button refresh-btn">
            🔄 Làm mới
          </button>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="loadingScoringRules" class="loading-section">
        <div class="loading-spinner"></div>
        <p>Đang tải quy tắc tính điểm...</p>
      </div>

      <!-- Scoring Rules Table -->
      <div v-else-if="scoringRules.length > 0" class="scoring-rules-table-container">
        <div class="table-responsive">
          <table class="scoring-rules-table kpi-table">
            <thead>
              <tr>
                <th>STT</th>
                <th>Tên chỉ tiêu</th>
                <th>Phương pháp tính</th>
                <th>Bước % thay đổi</th>
                <th>Điểm/Bước</th>
                <th>Điểm tối đa</th>
                <th>Điểm tối thiểu</th>
                <th>Áp dụng cho</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(rule, index) in scoringRules" :key="rule.Id">
                <td>{{ index + 1 }}</td>
                <td class="indicator-name kpi-name">{{ rule.kpiIndicatorName }}</td>
                <td>
                  <span class="scoring-method" :class="rule.scoringMethod.toLowerCase()">
                    {{ getScoringMethodLabel(rule.scoringMethod) }}
                  </span>
                </td>
                <td class="percentage-step kpi-percentage">{{ rule.percentageStep }}%</td>
                <td class="score-per-step kpi-score">{{ rule.scorePerStep }}</td>
                <td class="max-score kpi-score">{{ rule.maxScore || 'Không giới hạn' }}</td>
                <td class="min-score kpi-score">{{ rule.minScore || 'Không giới hạn' }}</td>
                <td class="unit-type">{{ getUnitTypeLabel(rule.applicableUnitType) }}</td>
                <td class="actions">
                  <button @click="editScoringRule(rule)" class="action-btn edit-btn">
                    ✏️
                  </button>
                  <button @click="deleteScoringRule(rule.Id)" class="action-btn delete-btn">
                    🗑️
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Summary -->
        <div class="scoring-rules-summary">
          <div class="summary-stats">
            <div class="stat-item">
              <span class="stat-label">Tổng quy tắc:</span>
              <span class="stat-value">{{ scoringRules.length }}</span>
            </div>
            <div class="stat-item">
              <span class="stat-label">Linear:</span>
              <span class="stat-value">{{ scoringRules.filter(r => r.scoringMethod === 'LINEAR').length }}</span>
            </div>
            <div class="stat-item">
              <span class="stat-label">Reverse Linear:</span>
              <span class="stat-value">{{ scoringRules.filter(r => r.scoringMethod === 'REVERSE_LINEAR').length }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else-if="!loadingScoringRules" class="no-scoring-rules">
        <div class="empty-state">
          <div class="empty-icon">⚡</div>
          <h3>Chưa có quy tắc tính điểm</h3>
          <p>Chưa có quy tắc tính điểm nào được cấu hình. Hãy tạo quy tắc đầu tiên.</p>
          <button @click="openAddScoringRuleModal" class="action-button add-btn">
            ➕ Tạo quy tắc đầu tiên
          </button>
        </div>
      </div>
    </div>

    <!-- Add/Edit Indicator Modal -->
    <div v-if="showIndicatorModal" class="modal-overlay" @click="handleModalClick">
      <div class="modal-content" @click.stop>>
        <div class="modal-header">
          <h3>{{ isEditMode ? 'Chỉnh sửa chỉ tiêu KPI' : 'Thêm chỉ tiêu KPI mới' }}</h3>
          <button @click="closeIndicatorModal" class="close-btn">✕</button>
        </div>

        <form @submit.prevent="saveIndicator" class="modal-form">
          <div class="form-group">
            <label for="indicatorName">Tên chỉ tiêu *</label>
            <input
              type="text"
              id="indicatorName"
              v-model="indicatorForm.indicatorName"
              required
              placeholder="Nhập tên chỉ tiêu KPI"
              class="form-input"
            />
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="maxScore">Điểm tối đa *</label>
              <input
                type="number"
                id="maxScore"
                v-model.number="indicatorForm.maxScore"
                required
                min="0"
                step="0.01"
                placeholder="0"
                class="form-input"
              />
            </div>

            <div class="form-group">
              <label for="unit">Đơn vị tính *</label>
              <input
                type="text"
                id="unit"
                v-model="indicatorForm.unit"
                required
                placeholder="%, Triệu VND, BT, cái..."
                class="form-input"
              />
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="valueType">Loại dữ liệu</label>
              <select
                id="valueType"
                v-model="indicatorForm.valueTypeString"
                class="form-input"
              >
                <option value="NUMBER">Số</option>
                <option value="PERCENTAGE">Phần trăm</option>
                <option value="CURRENCY">Tiền</option>
                <option value="POINTS">Điểm</option>
              </select>
            </div>

            <div class="form-group" v-if="isEditMode">
              <label for="isActive">Trạng thái</label>
              <select
                id="isActive"
                v-model="indicatorForm.isActive"
                class="form-input"
              >
                <option :value="true">Hoạt động</option>
                <option :value="false">Không hoạt động</option>
              </select>
            </div>
          </div>

          <div class="modal-footer">
            <button type="button" @click="closeIndicatorModal" class="btn-secondary">
              Hủy
            </button>
            <button type="submit" :disabled="savingIndicator" class="btn-primary">
              {{ savingIndicator ? 'Đang lưu...' : (isEditMode ? 'Cập nhật' : 'Thêm mới') }}
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Scoring Rules Modal -->
    <div
      v-if="showScoringRuleModal"
      class="modal-overlay"
      @click="handleModalClick"
      @keydown.esc="closeScoringRuleModal"
    >
      <div class="modal-content scoring-rule-modal">
        <div class="modal-header">
          <h3>{{ isEditScoringRuleMode ? '✏️ Chỉnh sửa quy tắc tính điểm' : '➕ Thêm quy tắc tính điểm' }}</h3>
          <button type="button" @click="closeScoringRuleModal" class="close-button">✕</button>
        </div>

        <form @submit.prevent="saveScoringRule" class="scoring-rule-form">
          <div class="modal-body">
            <div class="form-row">
              <div class="form-group">
                <label for="kpiIndicatorName">Tên chỉ tiêu KPI <span class="required">*</span></label>
                <select
                  id="kpiIndicatorName"
                  v-model="scoringRuleForm.kpiIndicatorName"
                  class="form-input"
                  required
                >
                  <option value="">-- Chọn chỉ tiêu KPI --</option>
                  <option
                    v-for="indicator in branchKpiIndicators"
                    :key="indicator.Name"
                    :value="indicator.Name"
                  >
                    {{ indicator.Name }}
                    <span v-if="indicator.Code"> ({{ indicator.Code }})</span>
                  </option>
                </select>
                <small class="form-help">Chỉ hiển thị các chỉ tiêu từ bảng KPI Chi nhánh</small>
              </div>

              <div class="form-group">
                <label for="scoringMethod">Phương pháp tính điểm <span class="required">*</span></label>
                <select
                  id="scoringMethod"
                  v-model="scoringRuleForm.scoringMethod"
                  class="form-input"
                  required
                >
                  <option value="">-- Chọn phương pháp --</option>
                  <option value="LINEAR">Linear - Điểm tăng theo hiệu suất</option>
                  <option value="REVERSE_LINEAR">Reverse Linear - Điểm giảm theo hiệu suất</option>
                  <option value="THRESHOLD">Threshold - Ngưỡng</option>
                  <option value="CUSTOM">Custom - Tùy chỉnh</option>
                </select>
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="percentageStep">Bước % thay đổi <span class="required">*</span></label>
                <input
                  id="percentageStep"
                  type="number"
                  step="0.1"
                  min="0.1"
                  max="100"
                  v-model.number="scoringRuleForm.percentageStep"
                  class="form-input"
                  placeholder="5"
                  required
                />
                <small class="form-help">Mỗi bước tăng/giảm bao nhiêu % so với chỉ tiêu</small>
              </div>

              <div class="form-group">
                <label for="scorePerStep">Điểm mỗi bước <span class="required">*</span></label>
                <input
                  id="scorePerStep"
                  type="number"
                  step="0.1"
                  min="0.1"
                  v-model.number="scoringRuleForm.scorePerStep"
                  class="form-input"
                  placeholder="1.5"
                  required
                />
                <small class="form-help">Điểm được cộng/trừ cho mỗi bước thay đổi</small>
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="maxScore">Điểm tối đa</label>
                <input
                  id="maxScore"
                  type="number"
                  step="0.1"
                  v-model.number="scoringRuleForm.maxScore"
                  class="form-input"
                  placeholder="Để trống nếu không giới hạn"
                />
              </div>

              <div class="form-group">
                <label for="minScore">Điểm tối thiểu</label>
                <input
                  id="minScore"
                  type="number"
                  step="0.1"
                  v-model.number="scoringRuleForm.minScore"
                  class="form-input"
                  placeholder="Để trống nếu không giới hạn"
                />
              </div>
            </div>

            <div class="form-group">
              <label for="applicableUnitType">Áp dụng cho loại chi nhánh</label>
              <select
                id="applicableUnitType"
                v-model="scoringRuleForm.applicableUnitType"
                class="form-input"
              >
                <option value="ALL">Tất cả chi nhánh</option>
                <option value="CNL1">Chi nhánh cấp 1</option>
                <option value="CNL2">Chi nhánh cấp 2</option>
              </select>
            </div>

            <div class="form-group">
              <label for="description">Mô tả</label>
              <textarea
                id="description"
                v-model="scoringRuleForm.description"
                class="form-input"
                rows="3"
                placeholder="Mô tả chi tiết quy tắc tính điểm..."
              ></textarea>
            </div>

            <!-- Example Preview -->
            <div v-if="scoringRuleForm.percentageStep && scoringRuleForm.scorePerStep" class="scoring-example">
              <h4>📊 Ví dụ tính điểm:</h4>
              <div class="example-grid">
                <div class="example-item">
                  <span class="example-label">Hoàn thành 100%:</span>
                  <span class="example-value">0 điểm (chuẩn)</span>
                </div>
                <div class="example-item">
                  <span class="example-label">Hoàn thành {{ 100 + scoringRuleForm.percentageStep }}%:</span>
                  <span class="example-value positive">+{{ scoringRuleForm.scorePerStep }} điểm</span>
                </div>
                <div class="example-item">
                  <span class="example-label">Hoàn thành {{ 100 - scoringRuleForm.percentageStep }}%:</span>
                  <span class="example-value negative">{{ scoringRuleForm.scoringMethod === 'REVERSE_LINEAR' ? '+' : '-' }}{{ scoringRuleForm.scorePerStep }} điểm</span>
                </div>
              </div>
            </div>
          </div>

          <div class="modal-footer">
            <button type="button" @click="closeScoringRuleModal" class="btn-secondary">
              Hủy bỏ
            </button>
            <button type="submit" :disabled="savingScoringRule" class="btn-primary">
              {{ savingScoringRule ? 'Đang lưu...' : (isEditScoringRuleMode ? 'Cập nhật quy tắc' : 'Tạo quy tắc mới') }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, nextTick, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import { kpiAssignmentService } from '../services/kpiAssignmentService';
import api from '../services/api';
import { getId, safeGet } from '../utils/casingSafeAccess.js';
import { useNumberInput } from '../utils/numberFormat';

const router = useRouter();

// 🔢 Initialize number input utility
const { handleInput, handleBlur, formatNumber, parseFormattedNumber } = useNumberInput({
  maxDecimalPlaces: 2,
  allowNegative: false
});

// Reactive data
const loading = ref(false);
const loadingDetails = ref(false);
const loadingScoringRules = ref(false);
const errorMessage = ref('');
const successMessage = ref('');
const showIndicatorModal = ref(false);
const isEditMode = ref(false);
const savingIndicator = ref(false);
const showScoringRuleModal = ref(false);
const isEditScoringRuleMode = ref(false);
const savingScoringRule = ref(false);

// Tab management
const activeTab = ref('employee'); // Default to employee tab

const kpiTables = ref([]);
const selectedTableId = ref(null);
const indicators = ref([]);
const currentIndicators = ref([]); // All indicators for counting purposes
const indicatorForm = ref({
  indicatorName: '',
  maxScore: null,
  unit: '',
  valueTypeString: 'NUMBER',
  isActive: true
});
const scoringRules = ref([]);
const scoringRuleForm = ref({
  kpiIndicatorName: '',
  scoringMethod: 'LINEAR',
  percentageStep: 0,
  scorePerStep: 0,
  maxScore: null,
  minScore: null,
  applicableUnitType: 'ALL',
  description: ''
});

// Computed properties
const selectedTable = computed(() => {
  return kpiTables.value.find(table => (table.Id || table.Id) === selectedTableId.value);
});

// Filter tables based on active tab using Category field
const filteredKpiTables = computed(() => {
  if (activeTab.value === 'employee') {
    // Filter for employee tables using actual Category values from API: "CANBO"
    return kpiTables.value
      .filter(table => {
        const category = (table.Category || table.category || '').toUpperCase();
        return category === 'CANBO' || category === 'VAI TRÒ CÁN BỘ';
      })
      .sort((a, b) => {
        const nameA = (a.Description || a.description || a.TableName || a.tableName || '').toLowerCase();
        const nameB = (b.Description || b.description || b.TableName || b.tableName || '').toLowerCase();
        return nameA.localeCompare(nameB, 'vi', { numeric: true });
      });
  } else if (activeTab.value === 'branch') {
    // Filter for branch tables - all others that are not employee tables
    return kpiTables.value
      .filter(table => {
        const category = (table.Category || table.category || '').toUpperCase();
        return category !== 'CANBO' && category !== 'VAI TRÒ CÁN BỘ';
      })
      .sort((a, b) => {
        // Custom ordering theo yêu cầu: Hội Sở → Bình Lư → Phong Thỏ → Sìn Hồ → Bum Tở → Than Uyên → Đoàn Kết → Tân Uyên → Nậm Hàng
        const customOrder = [
          'HoiSo_KPI_Assignment',           // KPI Hội sở
          'CnBinhLu_KPI_Assignment',        // KPI Chi nhánh Bình Lư
          'CnPhongTho_KPI_Assignment',      // KPI Chi nhánh Phong Thổ
          'CnSinHo_KPI_Assignment',         // KPI Chi nhánh Sìn Hồ
          'CnBumTo_KPI_Assignment',         // KPI Chi nhánh Bum Tở
          'CnThanUyen_KPI_Assignment',      // KPI Chi nhánh Than Uyên
          'CnDoanKet_KPI_Assignment',       // KPI Chi nhánh Đoàn Kết
          'CnTanUyen_KPI_Assignment',       // KPI Chi nhánh Tân Uyên
          'CnNamHang_KPI_Assignment'        // KPI Chi nhánh Nậm Hàng
        ];

        const tableNameA = a.TableName || a.tableName || '';
        const tableNameB = b.TableName || b.tableName || '';

        const indexA = customOrder.indexOf(tableNameA);
        const indexB = customOrder.indexOf(tableNameB);

        // If both tables are in the predefined order, sort by that order
        if (indexA !== -1 && indexB !== -1) {
          return indexA - indexB;
        }

        // If only one table is in the order, prioritize it
        if (indexA !== -1) return -1;
        if (indexB !== -1) return 1;

        // For other tables, sort alphabetically by description/name
        const nameA = a.Description || a.description || tableNameA;
        const nameB = b.Description || b.description || tableNameB;
        return nameA.localeCompare(nameB);
      });
  }
  return kpiTables.value;
});

const activeIndicatorsCount = computed(() => {
  return indicators.value.filter(indicator => safeGet(indicator, 'IsActive')).length;
});

const totalMaxScore = computed(() => {
  return indicators.value.reduce((sum, indicator) => sum + (safeGet(indicator, 'MaxScore') || 0), 0);
});

// Score validation methods
const getScoreValidationClass = (score) => {
  if (score === 100) return 'score-valid';
  if (score !== 100) return 'score-invalid';
  return '';
};

const getScoreValidationIcon = (score) => {
  if (score === 100) return '✅';
  if (score !== 100) return '⚠️';
  return '';
};

// Computed property để lấy danh sách tên chỉ tiêu từ bảng KPI chi nhánh
const branchKpiIndicators = computed(() => {
  const branchTables = kpiTables.value.filter(table =>
    table.category === 'Chi nhánh'
  );

  const allIndicators = [];
  branchTables.forEach(table => {
    if (table.indicators && Array.isArray(table.indicators)) {
      table.indicators.forEach(indicator => {
        if (indicator.isActive && indicator.indicatorName) {
          allIndicators.push({
            name: indicator.indicatorName,
            code: indicator.indicatorCode || '',
            tableId: table.Id,
            tableName: table.tableName,
            orderIndex: indicator.orderIndex || 999 // Default order if not specified
          });
        }
      });
    }
  });

  // Remove duplicates based on indicator name, keep the one with lowest orderIndex
  const uniqueIndicators = allIndicators.filter((indicator, index, self) => {
    const firstIndex = self.findIndex(i => i.Name === indicator.Name);
    if (firstIndex === index) return true;
    // If duplicate, keep the one with lower orderIndex
    return indicator.orderIndex < self[firstIndex].orderIndex;
  });

  // Sort by table order first, then by orderIndex within table
  return uniqueIndicators.sort((a, b) => {
    // If same table, sort by orderIndex
    if (a.tableId === b.tableId) {
      return (a.orderIndex || 999) - (b.orderIndex || 999);
    }
    // If different tables, sort by table name then orderIndex
    const tableCompare = a.tableName.localeCompare(b.tableName);
    if (tableCompare !== 0) return tableCompare;
    return (a.orderIndex || 999) - (b.orderIndex || 999);
  });
});

// Methods
const clearMessages = () => {
  errorMessage.value = '';
  successMessage.value = '';
};

const showError = (message) => {
  errorMessage.value = message;
  successMessage.value = '';
  setTimeout(() => {
    errorMessage.value = '';
  }, 5000);
};

const showSuccess = (message) => {
  successMessage.value = message;
  errorMessage.value = '';
  setTimeout(() => {
    successMessage.value = '';
  }, 3000);
};

// Tab switching method
const switchTab = (tab) => {
  activeTab.value = tab;
  selectedTableId.value = null; // Reset selection when switching tabs
  indicators.value = []; // Clear indicators
  scoringRules.value = []; // Clear scoring rules
  clearMessages();

  // Load scoring rules when switching to scoring tab
  if (tab === 'scoring') {
    loadScoringRules();
  }
};

const getIndicatorCount = (tableId) => {
  if (!tableId) return 0;

  // Try to get count from cached data first
  const cachedTable = kpiTables.value.find(t => (t.Id || t.id) === tableId);
  if (cachedTable && (cachedTable.IndicatorCount !== undefined || cachedTable.indicatorCount !== undefined)) {
    return cachedTable.IndicatorCount ?? cachedTable.indicatorCount;
  }

  // If not available, try to count from indicators if we have them
  if (currentIndicators.value && currentIndicators.value.length > 0) {
    return currentIndicators.value.filter(ind => (ind.TableId || ind.tableId) === tableId).length;
  }

  return 0;
};

const getTableTypeName = (tableType, tableName, description) => {
  // For branch tables, use the description directly
  if (tableName && tableName.startsWith('KPI_')) {
    return description || tableName;
  }

  // For employee tables, use the TableName as the actual type (not TableType)
  // TableName is the real identifier like "PhophongKtnqCnl2", "TruongphongKhdn", etc.
  const typeNames = {
    // Cán bộ tables - mapped by actual TableName
    'TruongphongKhdn': 'Trưởng phòng KHDN',
    'TruongphongKhcn': 'Trưởng phòng KHCN',
    'PhophongKhdn': 'Phó phòng KHDN',
    'PhophongKhcn': 'Phó phòng KHCN',
    'TruongphongKhqlrr': 'Trưởng phòng KH&QLRR',
    'PhophongKhqlrr': 'Phó phòng KH&QLRR',
    'Cbtd': 'Cán bộ tín dụng',
    'TruongphongKtnqCnl1': 'Trưởng phòng KTNQ CNL1',
    'PhophongKtnqCnl1': 'Phó phòng KTNQ CNL1',
    'Gdv': 'Giao dịch viên',
    'TqHkKtnb': 'Thủ quỹ | Hậu kiểm | KTNB',
    'TruongphongItThKtgs': 'Trưởng phó IT | Tổng hợp | KTGS',
    'CbItThKtgsKhqlrr': 'Cán bộ IT | Tổng hợp | KTGS | KH&QLRR',
    'GiamdocPgd': 'Giám đốc Phòng giao dịch',
    'PhogiamdocPgd': 'Phó giám đốc Phòng giao dịch',
    'PhogiamdocPgdCbtd': 'Phó giám đốc PGD kiêm CBTD',
    'GiamdocCnl2': 'Giám đốc CNL2',
    'PhogiamdocCnl2Td': 'Phó giám đốc CNL2 phụ trách TD',
    'PhogiamdocCnl2Kt': 'Phó giám đốc CNL2 phụ trách KT',
    'TruongphongKhCnl2': 'Trưởng phòng KH CNL2',
    'PhophongKhCnl2': 'Phó phòng KH CNL2',
    'TruongphongKtnqCnl2': 'Trưởng phòng KTNQ CNL2',
    'PhophongKtnqCnl2': 'Phó phòng KTNQ CNL2',
    'CanBoNghiepVuKhac': 'Cán bộ nghiệp vụ khác'
  };

  // Use TableName instead of TableType for employee tables to get the correct type name
  const realType = tableName || tableType;
  return description || typeNames[realType] || `${realType}`;
};

const formatDate = (dateString) => {
  if (!dateString) return '';
  const date = new Date(dateString);
  return date.toLocaleDateString('vi-VN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit'
  });
};

const fetchKpiTables = async () => {
  try {
    loading.value = true;
    clearMessages();

    const tablesData = await kpiAssignmentService.getTables();
    kpiTables.value = tablesData;

    console.log('KPI Tables loaded:', kpiTables.value.length);

    if (kpiTables.value.length > 0) {
      showSuccess(`Đã tải ${kpiTables.value.length} bảng giao khoán KPI.`);
    }
  } catch (error) {
    console.error('Error loading KPI tables:', error);
    showError('Không thể tải danh sách bảng KPI. Vui lòng thử lại.');
  } finally {
    loading.value = false;
  }
};

const selectTable = (tableId) => {
  selectedTableId.value = tableId;
  loadTableDetails();
};

const loadTableDetails = async () => {
  if (!selectedTableId.value) return;

  try {
    loadingDetails.value = true;
    clearMessages();

    const tableData = await kpiAssignmentService.getTableDetails(selectedTableId.value);

    if (tableData.indicators) {
      indicators.value = tableData.indicators;
    } else {
      indicators.value = [];
    }

    console.log('Table details loaded:', indicators.value.length, 'indicators');
  } catch (error) {
    console.error('Error loading table details:', error);
    showError('Không thể tải chi tiết bảng KPI. Vui lòng thử lại.');
    indicators.value = [];
  } finally {
    loadingDetails.value = false;
  }
};

const loadScoringRules = async () => {
  try {
    loadingScoringRules.value = true;
    const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7297/api';
    const token = localStorage.getItem('authToken');

    const response = await fetch(`${API_BASE_URL}/KpiScoringRules`, {
      headers: {
        'Authorization': token ? `Bearer ${token}` : ''
      }
    });

    if (response.ok) {
      scoringRules.value = await response.json();
    } else {
      throw new Error('Failed to load scoring rules');
    }
  } catch (error) {
    console.error('Error loading scoring rules:', error);
    showError('Không thể tải danh sách quy tắc tính điểm.');
  } finally {
    loadingScoringRules.value = false;
  }
};

const onTableChange = () => {
  console.log('Table selection changed:', selectedTableId.value);

  // Reset indicators và scoring rules khi thay đổi bảng
  indicators.value = [];
  scoringRules.value = [];

  // Load chi tiết bảng được chọn
  if (selectedTableId.value) {
    loadTableDetails();
    loadScoringRules();
  }
};

const openAddIndicatorModal = () => {
  isEditMode.value = false;
  indicatorForm.value = {
    indicatorName: '',
    maxScore: null,
    unit: '',
    valueTypeString: 'NUMBER',
    isActive: true
  };
  showIndicatorModal.value = true;

  console.log('🚀 Opening add modal...');

  nextTick(() => {
    const modalOverlay = document.querySelector('.modal-overlay');
    if (modalOverlay) {
      // Modal positioning cho layout mới - centered trên viewport
      modalOverlay.style.cssText = `
        position: fixed !important;
        top: 0 !important;
        left: 0 !important;
        right: 0 !important;
        bottom: 0 !important;
        width: 100% !important;
        height: 100vh !important;
        background: rgba(0, 0, 0, 0.75) !important;
        display: flex !important;
        justify-content: center !important;
        align-items: center !important;
        z-index: 99999 !important;
        backdrop-filter: blur(2px) !important;
      `;

      console.log('✅ ADD Modal positioned (fixed center)');
    }
  });
};

const openEditIndicatorModal = (indicator) => {
  isEditMode.value = true;
  indicatorForm.value = { ...indicator };
  showIndicatorModal.value = true;

  console.log('🚀 Opening edit modal for:', indicator.indicatorName);

  nextTick(() => {
    const modalOverlay = document.querySelector('.modal-overlay');
    if (modalOverlay) {
      // Modal positioning cho layout mới - centered trên viewport
      modalOverlay.style.cssText = `
        position: fixed !important;
        top: 0 !important;
        left: 0 !important;
        right: 0 !important;
        bottom: 0 !important;
        width: 100% !important;
        height: 100vh !important;
        background: rgba(0, 0, 0, 0.75) !important;
        display: flex !important;
        justify-content: center !important;
        align-items: center !important;
        z-index: 99999 !important;
        backdrop-filter: blur(2px) !important;
      `;

      console.log('✅ EDIT Modal positioned (fixed center) for:', indicator.indicatorName);
    } else {
      console.error('❌ Modal overlay element NOT FOUND after nextTick!');
    }
  });
};

const closeIndicatorModal = () => {
  console.log('🔒 Closing modal and cleaning up...');

  // Cleanup modal positioning BEFORE closing
  const modalOverlay = document.querySelector('.modal-overlay');
  if (modalOverlay) {
    // Reset tất cả attributes và styles
    modalOverlay.removeAttribute('style');
    modalOverlay.className = 'modal-overlay'; // Reset về class gốc

    console.log('🧹 Modal styles reset to default');
  }

  showIndicatorModal.value = false;
};

const saveIndicator = async () => {
  try {
    savingIndicator.value = true;
    clearMessages();

    const payload = {
      indicatorName: indicatorForm.value.indicatorName,
      maxScore: indicatorForm.value.maxScore,
      unit: indicatorForm.value.unit,
      valueTypeString: indicatorForm.value.valueTypeString,
      isActive: indicatorForm.value.isActive
    };

    if (isEditMode.value) {
      // Update existing indicator
      console.log('🔄 Updating indicator:', indicatorForm.value.Id, payload);
      const result = await kpiAssignmentService.updateIndicator(indicatorForm.value.Id, payload);

      if (result.success) {
        showSuccess(result.message || 'Cập nhật chỉ tiêu thành công!');
      } else {
        showError(result.message || 'Có lỗi xảy ra khi cập nhật chỉ tiêu.');
        return;
      }
    } else {
      // Create new indicator
      payload.kpiAssignmentTableId = selectedTableId.value;
      console.log('➕ Creating new indicator:', payload);
      const result = await kpiAssignmentService.createIndicator(payload);

      if (result.success) {
        showSuccess(result.message || 'Thêm chỉ tiêu mới thành công!');
      } else {
        showError(result.message || 'Có lỗi xảy ra khi thêm chỉ tiêu.');
        return;
      }
    }

    // Close modal and refresh data
    closeIndicatorModal();
    await loadTableDetails();

  } catch (error) {
    console.error('Error saving indicator:', error);
    if (error.response && error.response.data && error.response.data.message) {
      showError(`Lỗi: ${error.response.data.message}`);
    } else {
      showError('Có lỗi xảy ra khi lưu chỉ tiêu. Vui lòng thử lại.');
    }
  } finally {
    savingIndicator.value = false;
  }
};

const deleteIndicator = async (indicator) => {
  if (!confirm(`Bạn có chắc muốn xóa chỉ tiêu "${indicator.indicatorName}"?`)) {
    return;
  }

  try {
    clearMessages();
    console.log('🗑️ Deleting indicator:', indicator.Id);

    const result = await kpiAssignmentService.deleteIndicator(indicator.Id);

    if (result.success) {
      showSuccess(result.message || 'Xóa chỉ tiêu thành công!');
      await loadTableDetails();
    } else {
      showError(result.message || 'Có lỗi xảy ra khi xóa chỉ tiêu.');
    }
  } catch (error) {
    console.error('Error deleting indicator:', error);
    if (error.response && error.response.data && error.response.data.message) {
      showError(`Lỗi: ${error.response.data.message}`);
    } else {
      showError('Có lỗi xảy ra khi xóa chỉ tiêu. Vui lòng thử lại.');
    }
  }
};

const moveIndicatorUp = async (indicator) => {
  if (indicator.orderIndex <= 1) return;

  try {
    clearMessages();
    const newOrderIndex = indicator.orderIndex - 1;
    console.log('⬆️ Moving indicator up:', indicator.Id, 'to order:', newOrderIndex);

    const result = await kpiAssignmentService.reorderIndicator(indicator.Id, newOrderIndex);

    if (result.success) {
      await loadTableDetails();
    } else {
      showError(result.message || 'Có lỗi xảy ra khi thay đổi thứ tự.');
    }
  } catch (error) {
    console.error('Error moving indicator up:', error);
    showError('Có lỗi xảy ra khi thay đổi thứ tự. Vui lòng thử lại.');
  }
};

const moveIndicatorDown = async (indicator) => {
  if (indicator.orderIndex >= indicators.value.length) return;

  try {
    clearMessages();
    const newOrderIndex = indicator.orderIndex + 1;
    console.log('⬇️ Moving indicator down:', indicator.Id, 'to order:', newOrderIndex);

    const result = await kpiAssignmentService.reorderIndicator(indicator.Id, newOrderIndex);

    if (result.success) {
      await loadTableDetails();
    } else {
      showError(result.message || 'Có lỗi xảy ra khi thay đổi thứ tự.');
    }
  } catch (error) {
    console.error('Error moving indicator down:', error);
    showError('Có lỗi xảy ra khi thay đổi thứ tự. Vui lòng thử lại.');
  }
};

const lockBodyScroll = () => {
  // Store current scroll position
  const scrollY = window.scrollY;
  document.body.style.position = 'fixed';
  document.body.style.top = `-${scrollY}px`;
  document.body.style.width = '100%';
  document.body.setAttribute('data-scroll-lock', scrollY.toString());
};

const unlockBodyScroll = () => {
  // Restore scroll position
  const scrollY = document.body.getAttribute('data-scroll-lock');
  document.body.style.position = '';
  document.body.style.top = '';
  document.body.style.width = '';
  document.body.removeAttribute('data-scroll-lock');
  if (scrollY) {
    window.scrollTo(0, parseInt(scrollY));
  }
};

// Enhanced modal management with keyboard support
const handleEscapeKey = (event) => {
  if (event.key === 'Escape') {
    if (showIndicatorModal.value) {
      closeIndicatorModal();
    } else if (showScoringRuleModal.value) {
      closeScoringRuleModal();
    }
  }
};

const handleModalClick = (event) => {
  // Only close if clicking the overlay, not the modal content
  if (event.target === event.currentTarget) {
    if (showIndicatorModal.value) {
      closeIndicatorModal();
    } else if (showScoringRuleModal.value) {
      closeScoringRuleModal();
    }
  }
};

const editScoringRule = (rule) => {
  scoringRuleForm.value = { ...rule };
  isEditScoringRuleMode.value = true;
  showScoringRuleModal.value = true;
};

// Utility methods for scoring rules
const getScoringMethodLabel = (method) => {
  switch (method) {
    case 'LINEAR':
      return 'Tuyến tính';
    case 'REVERSE_LINEAR':
      return 'Tuyến tính ngược';
    case 'THRESHOLD':
      return 'Ngưỡng';
    case 'CUSTOM':
      return 'Tùy chỉnh';
    default:
      return method;
  }
};

const getUnitTypeLabel = (unitType) => {
  switch (unitType) {
    case 'ALL':
      return 'Tất cả';
    case 'CNL1':
      return 'CNL1';
    case 'CNL2':
      return 'CNL2';
    default:
      return unitType;
  }
};

const openAddScoringRuleModal = () => {
  // Reset form and open modal for adding new scoring rule
  scoringRuleForm.value = {
    kpiIndicatorName: '',
    scoringMethod: 'LINEAR',
    percentageStep: 5,
    scorePerStep: 1.5,
    maxScore: null,
    minScore: null,
    applicableUnitType: 'ALL',
    description: ''
  };
  isEditScoringRuleMode.value = false;
  showScoringRuleModal.value = true;
};

const closeScoringRuleModal = () => {
  showScoringRuleModal.value = false;
};

const saveScoringRule = async () => {
  try {
    savingScoringRule.value = true;
    clearMessages();

    const payload = {
      kpiIndicatorName: scoringRuleForm.value.kpiIndicatorName,
      scoringMethod: scoringRuleForm.value.scoringMethod,
      percentageStep: scoringRuleForm.value.percentageStep,
      scorePerStep: scoringRuleForm.value.scorePerStep,
      maxScore: scoringRuleForm.value.maxScore,
      minScore: scoringRuleForm.value.minScore,
      applicableUnitType: scoringRuleForm.value.applicableUnitType,
      description: scoringRuleForm.value.description
    };

    const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7297/api';
    const token = localStorage.getItem('authToken');

    const requestConfig = {
      method: isEditScoringRuleMode.value ? 'PUT' : 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': token ? `Bearer ${token}` : ''
      },
      body: JSON.stringify(payload)
    };

    const url = isEditScoringRuleMode.value
      ? `${API_BASE_URL}/KpiScoringRules/${scoringRuleForm.value.Id}`
      : `${API_BASE_URL}/KpiScoringRules`;

    const response = await fetch(url, requestConfig);

    if (!response.ok) {
      throw new Error(`API Error: ${response.Status}`);
    }

    showSuccess(isEditScoringRuleMode.value ? 'Cập nhật quy tắc tính điểm thành công.' : 'Thêm quy tắc tính điểm mới thành công.');
    closeScoringRuleModal();
    await loadScoringRules(); // Reload to get updated list
  } catch (error) {
    console.error('Error saving scoring rule:', error);
    showError('Không thể lưu quy tắc tính điểm. Vui lòng thử lại.');
  } finally {
    savingScoringRule.value = false;
  }
};

const deleteScoringRule = async (ruleId) => {
  if (!confirm('Bạn có chắc chắn muốn xóa quy tắc này?')) {
    return;
  }

  try {
    clearMessages();

    const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7297/api';
    const token = localStorage.getItem('authToken');

    const response = await fetch(`${API_BASE_URL}/KpiScoringRules/${ruleId}`, {
      method: 'DELETE',
      headers: {
        'Authorization': token ? `Bearer ${token}` : ''
      }
    });

    if (!response.ok) {
      throw new Error(`API Error: ${response.Status}`);
    }

    showSuccess('Xóa quy tắc tính điểm thành công.');
    await loadScoringRules();
  } catch (error) {
    console.error('Error deleting scoring rule:', error);
    showError('Không thể xóa quy tắc tính điểm. Vui lòng thử lại.');
  }
};

// Lifecycle hooks
onMounted(async () => {
  // Kiểm tra authentication
  // if (!isAuthenticated()) {
  //   router.push('/login');
  //   return;
  // }

  // Load KPI tables khi component được mount
  await fetchKpiTables();

  // Load all indicators for counting purposes
  await loadAllIndicators();

  // Load indicators cho dropdown scoring rules
  await loadAllBranchIndicators();
});

// Load all indicators from API for counting purposes
const loadAllIndicators = async () => {
  try {
    console.log('🔄 Loading all indicators for counting...');
    const response = await api.get('/KpiIndicators');
    if (response.data) {
      currentIndicators.value = response.data;
      console.log(`✅ Loaded ${currentIndicators.value.length} indicators for counting`);
    } else {
      console.warn('⚠️ Failed to load indicators:', response);
      currentIndicators.value = [];
    }
  } catch (error) {
    console.error('❌ Error loading all indicators:', error);
    currentIndicators.value = [];
  }
};

// Hàm load tất cả indicators từ bảng chi nhánh cho dropdown
const loadAllBranchIndicators = async () => {
  try {
    // Get all branch tables
    const branchTables = kpiTables.value.filter(table =>
      table.category === 'Chi nhánh'
    );

    // Load indicators for each branch table
    for (const table of branchTables) {
      try {
        const tableData = await kpiAssignmentService.getTableDetails(table.Id);
        if (tableData.indicators && Array.isArray(tableData.indicators)) {
          // Store indicators in the table object for use in computed property
          table.indicators = tableData.indicators;
        }
      } catch (error) {
        console.error(`Error loading indicators for table ${table.Id}:`, error);
      }
    }

    console.log('Loaded indicators for branch tables');
  } catch (error) {
    console.error('Error loading branch indicators:', error);
  }
};

// Clean table description by removing "Bảng KPI cho " prefix
const cleanTableDescription = (description) => {
  if (!description) return '';
  // Bỏ chữ "Bảng KPI cho " ở đầu
  return description.replace(/^Bảng KPI cho\s*/i, '');
};
</script>

<style scoped>
/* Import Google Fonts for better typography */
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&family=JetBrains+Mono:wght@400;500&display=swap');

.kpi-definitions {
  max-width: 1400px;
  margin: 0 auto;
  padding: 20px;
}

.header-section {
  text-align: center;
  margin-bottom: 30px;
}

.header-section h1 {
  color: #8B1538;
  font-size: 2.5rem;
  font-weight: 700;
  margin-bottom: 10px;
}

.subtitle {
  color: #A91B47;
  font-size: 1.2rem;
  font-weight: 500;
}

/* Tab Navigation Styles */
.tabs-container {
  margin: 20px 0;
}

.tab-navigation {
  display: flex;
  border-bottom: 2px solid #e0e0e0;
  margin-bottom: 20px;
}

.tab-button {
  background-color: transparent;
  border: none;
  padding: 15px 25px;
  font-size: 1.1rem;
  font-weight: 600;
  color: #666;
  cursor: pointer;
  border-bottom: 3px solid transparent;
  transition: all 0.3s ease;
  position: relative;
}

.tab-button:hover {
  color: #8B1538;
  background-color: #f8f9fa;
}

.tab-button.active {
  color: #8B1538;
  border-bottom: 3px solid #8B1538;
  background-color: #fff;
}

.tab-button.active::after {
  content: '';
  position: absolute;
  bottom: -2px;
  left: 0;
  right: 0;
  height: 2px;
  background-color: #fff;
}

.error-message {
  background-color: #f8d7da;
  color: #721c24;
  border: 1px solid #f5c6cb;
  padding: 12px 18px;
  margin-bottom: 20px;
  border-radius: 6px;
  text-align: center;
}

.success-message {
  background-color: #d4edda;
  color: #155724;
  border: 1px solid #c3e6cb;
  padding: 12px 18px;
  margin-bottom: 20px;
  border-radius: 6px;
  text-align: center;
}

.loading-section {
  text-align: center;
  padding: 40px;
}

.loading-spinner {
  border: 3px solid rgba(139, 21, 56, 0.3);
  border-radius: 50%;
  border-top: 3px solid #8B1538;
  width: 30px;
  height: 30px;
  animation: spin 1s linear infinite;
  display: inline-block;
  margin-bottom: 16px;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.form-container {
  background-color: #ffffff;
  padding: 25px;
  border-radius: 12px;
  border: 1px solid #dde0e3;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
  margin-bottom: 25px;
}

.form-container h2 {
  margin-top: 0;
  margin-bottom: 20px;
  color: #8B1538;
  font-size: 1.5rem;
  font-weight: 600;
}

.no-data {
  text-align: center;
  padding: 40px;
  color: #6c757d;
  font-style: italic;
}

/* =========================
   MAIN LAYOUT STYLES - NEW
   ========================= */

.main-layout {
  display: flex;
  gap: 30px;
  margin-top: 20px;
  min-height: 70vh;
}

.left-panel {
  flex: 0 0 400px;
  background: #ffffff;
  border-radius: 12px;
  border: 2px solid #e2e8f0;
  padding: 25px;
  height: fit-content;
  position: sticky;
  top: 20px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.right-panel {
  flex: 1;
  background: #ffffff;
  border-radius: 12px;
  border: 2px solid #e2e8f0;
  padding: 25px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.table-selector-container h2 {
  color: #8B1538;
  font-size: 1.4rem;
  font-weight: 700;
  margin-bottom: 20px;
  display: flex;
  align-items: center;
  gap: 10px;
}

.dropdown-group {
  margin-bottom: 20px;
}

.dropdown-group label {
  display: block;
  font-weight: 600;
  color: #495057;
  margin-bottom: 8px;
  font-size: 0.95rem;
}

.table-dropdown {
  width: 100%;
  padding: 12px 16px;
  border: 2px solid #e2e8f0;
  border-radius: 8px;
  font-size: 1rem;
  background: white;
  cursor: pointer;
  transition: all 0.3s ease;
}

.table-dropdown:focus {
  outline: none;
  border-color: #8B1538;
  box-shadow: 0 0 0 3px rgba(139, 21, 56, 0.1);
}

.table-dropdown:hover {
  border-color: #8B1538;
}

.selected-table-info {
  background: linear-gradient(135deg, #fdf2f8, #fce7f3);
  border-radius: 10px;
  padding: 20px;
  margin-top: 15px;
}

.selected-table-info h3 {
  color: #8B1538;
  font-size: 1.2rem;
  font-weight: 700;
  margin-bottom: 15px;
}

.table-details {
  margin-bottom: 15px;
}

.detail-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 0;
  border-bottom: 1px solid #e2e8f0;
}

.detail-item:last-child {
  border-bottom: none;
}

.detail-item .label {
  font-weight: 600;
  color: #495057;
  font-size: 0.9rem;
}

.detail-item .value {
  font-weight: 500;
  color: #2c3e50;
}

.detail-item .value.indicator-count {
  color: #8B1538;
  font-weight: 700;
  font-size: 1.1rem;
}

.description-box {
  margin-top: 15px;
  padding-top: 15px;
  border-top: 1px solid #e2e8f0;
}

.description-box label {
  font-weight: 600;
  color: #495057;
  font-size: 0.9rem;
  display: block;
  margin-bottom: 8px;
}

.description-box p {
  color: #2c3e50;
  line-height: 1.6;
  margin: 0;
  font-size: 0.95rem;
}

.refresh-button {
  width: 100%;
  padding: 10px;
  margin-top: 15px;
  background: linear-gradient(135deg, #6c757d, #5a6268);
  color: white;
  border: none;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.refresh-button:hover:not(:disabled) {
  background: linear-gradient(135deg, #545b62, #4e555b);
  transform: translateY(-1px);
}

.refresh-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.no-selection {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 400px;
}

.empty-state {
  text-align: center;
  color: #6c757d;
}

.empty-state .empty-icon {
  font-size: 4rem;
  margin-bottom: 20px;
}

.empty-state h3 {
  color: #495057;
  font-size: 1.3rem;
  font-weight: 600;
  margin-bottom: 10px;
}

.empty-state p {
  font-size: 1rem;
  line-height: 1.6;
  margin-bottom: 25px;
}

/* =========================
   INDICATORS PANEL STYLES
   ========================= */

.indicators-header h2 {
  color: #8B1538; /* Bordeaux color for KPI titles */
  font-size: 1.4rem;
  font-weight: 700;
  margin-bottom: 20px;
}

.indicators-table .indicator-name {
  color: #2c3e50; /* Black color for individual indicators */
  font-weight: 500;
  font-size: 1rem;
}

.indicators-table td {
  color: #2c3e50; /* Black color for all table content */
}

.indicators-table th {
  color: #495057; /* Slightly lighter for table headers */
  font-weight: 600;
}

/* =========================
   SCORING RULES CONTENT STYLES
   ========================= */

.scoring-rules-content {
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
}

.scoring-rules-header {
  background: linear-gradient(135deg, #8B1538 0%, #A91B47 50%, #C02456 100%);
  color: white;
  padding: 30px;
  border-radius: 16px;
  margin-bottom: 30px;
  box-shadow: 0 8px 32px rgba(139, 21, 56, 0.3);
  position: relative;
  overflow: hidden;
}

.scoring-rules-header::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(45deg, rgba(255,255,255,0.1) 0%, rgba(255,255,255,0.05) 100%);
  pointer-events: none;
}

.scoring-rules-header h2 {
  font-size: 2.2rem;
  font-weight: 700;
  margin: 0 0 10px 0;
  text-shadow: 0 2px 4px rgba(0,0,0,0.2);
  letter-spacing: -0.5px;
}

.scoring-rules-header .subtitle {
  font-size: 1.1rem;
  opacity: 0.9;
  margin-bottom: 25px;
  font-weight: 400;
}

.scoring-actions {
  display: flex;
  gap: 15px;
  align-items: center;
}

/* Beautiful Table Styles */
.scoring-rules-table-container {
  background: white;
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 12px 48px rgba(0, 0, 0, 0.08);
  border: 1px solid rgba(139, 21, 56, 0.1);
  margin-bottom: 30px;
}

.table-responsive {
  overflow-x: auto;
}

.scoring-rules-table {
  width: 100%;
  border-collapse: collapse;
  font-family: 'Inter', sans-serif;
  font-size: 0.95rem;
}

.scoring-rules-table thead {
  background: linear-gradient(135deg, #8B1538 0%, #A91B47 100%);
  color: white;
  position: relative;
}

.scoring-rules-table thead::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 2px;
  background: linear-gradient(90deg, #C02456, #8B1538, #C02456);
}

.scoring-rules-table th {
  padding: 18px 16px;
  text-align: left;
  font-weight: 600;
  font-size: 0.9rem;
  letter-spacing: 0.5px;
  text-transform: uppercase;
  border: none;
  position: relative;
}

.scoring-rules-table th:not(:last-child)::after {
  content: '';
  position: absolute;
  right: 0;
  top: 20%;
  bottom: 20%;
  width: 1px;
  background: rgba(255, 255, 255, 0.2);
}

.scoring-rules-table tbody tr {
  transition: all 0.3s ease;
  border-bottom: 1px solid #f1f5f9;
}

.scoring-rules-table tbody tr:hover {
  background: linear-gradient(90deg, rgba(139, 21, 56, 0.03), rgba(139, 21, 56, 0.01), rgba(139, 21, 56, 0.03));
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(139, 21, 56, 0.1);
}

.scoring-rules-table td {
  padding: 16px;
  border: none;
  color: #334155;
  font-weight: 500;
  vertical-align: middle;
}

/* Special column styling */
.indicator-name {
  font-weight: 600 !important;
  color: #8B1538 !important;
  font-size: 1rem;
}

.scoring-method {
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 0.85rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  font-family: 'JetBrains Mono', monospace;
}

.scoring-method.linear {
  background: linear-gradient(135deg, #10b981, #059669);
  color: white;
}

.scoring-method.reverse_linear {
  background: linear-gradient(135deg, #f59e0b, #d97706);
  color: white;
}

.scoring-method.threshold {
  background: linear-gradient(135deg, #3b82f6, #2563eb);
  color: white;
}

.scoring-method.custom {
  background: linear-gradient(135deg, #8b5cf6, #7c3aed);
  color: white;
}

.percentage-step,
.score-per-step,
.max-score,
.min-score {
  text-align: center;
  font-weight: 600;
  font-family: 'JetBrains Mono', monospace;
  color: #1e293b;
  font-size: 0.95rem;
}

.unit-type {
  text-align: center;
  font-weight: 500;
  color: #64748b;
}

.actions {
  text-align: center;
}

.action-btn {
  background: none;
  border: none;
  padding: 8px 10px;
  margin: 0 3px;
  border-radius: 8px;
  cursor: pointer;
  font-size: 1.1rem;
  transition: all 0.3s ease;
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.action-btn.edit-btn:hover {
  background: linear-gradient(135deg, #3b82f6, #2563eb);
  color: white;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(59, 130, 246, 0.3);
}

.action-btn.delete-btn:hover {
  background: linear-gradient(135deg, #ef4444, #dc2626);
  color: white;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(239, 68, 68, 0.3);
}

/* =========================
   SCORING RULES MODAL STYLES
   ========================= */

.scoring-rule-modal {
  max-width: 900px;
  width: 95%;
  font-family: 'Inter', sans-serif;
}

.scoring-rule-modal .modal-content {
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 25px 50px rgba(0, 0, 0, 0.25);
  border: 1px solid rgba(139, 21, 56, 0.1);
}

.scoring-rule-modal .modal-header {
  background: linear-gradient(135deg, #8B1538 0%, #A91B47 50%, #C02456 100%);
  color: white;
  padding: 25px 30px;
  border-bottom: none;
  position: relative;
}

.scoring-rule-modal .modal-header::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: linear-gradient(45deg, rgba(255,255,255,0.1) 0%, rgba(255,255,255,0.05) 100%);
  pointer-events: none;
}

.scoring-rule-modal .modal-header h3 {
  font-size: 1.5rem;
  font-weight: 700;
  margin: 0;
  text-shadow: 0 2px 4px rgba(0,0,0,0.2);
  letter-spacing: -0.3px;
}

.scoring-rule-form {
  display: flex;
  flex-direction: column;
}

.modal-body {
  max-height: 70vh;
  overflow-y: auto;
  padding: 30px;
  background: #fafbfc;
}

.form-row {
  display: flex;
  gap: 20px;
  margin-bottom: 25px;
}

.form-row .form-group {
  flex: 1;
}

.form-group {
  margin-bottom: 25px;
}

.form-group label {
  display: block;
  margin-bottom: 8px;
  font-weight: 600;
  color: #1e293b;
  font-size: 0.95rem;
}

.form-input {
  width: 100%;
  padding: 12px 16px;
  border: 2px solid #e2e8f0;
  border-radius: 10px;
  font-size: 1rem;
  font-family: 'Inter', sans-serif;
  background: white;
  transition: all 0.3s ease;
  color: #334155;
}

.form-input:focus {
  outline: none;
  border-color: #8B1538;
  box-shadow: 0 0 0 4px rgba(139, 21, 56, 0.1);
  background: #fff;
}

.form-input:hover {
  border-color: #cbd5e1;
}

.form-help {
  color: #64748b;
  font-size: 0.85rem;
  margin-top: 6px;
  display: block;
  font-style: italic;
}

.required {
  color: #ef4444;
  font-weight: 700;
}

.scoring-example {
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 20px;
  margin-top: 25px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
}

.scoring-example h4 {
  color: #1e293b;
  margin: 0 0 15px 0;
  font-size: 1.1rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 8px;
}

.example-grid {
  display: grid;
  gap: 12px;
}

.example-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  background: white;
  border-radius: 8px;
  font-size: 0.95rem;
  border: 1px solid #e2e8f0;
  transition: all 0.2s ease;
}

.example-item:hover {
  border-color: #8B1538;
  transform: translateY(-1px);
  box-shadow: 0 2px 8px rgba(139, 21, 56, 0.1);
}

.example-label {
  color: #475569;
  font-weight: 500;
}

.example-value {
  font-weight: 700;
  font-family: 'JetBrains Mono', monospace;
}

.example-value.positive {
  color: #059669;
}

.example-value.negative {
  color: #dc2626;
}

.close-button {
  background: rgba(255, 255, 255, 0.2);
  border: none;
  color: white;
  font-size: 1.3rem;
  cursor: pointer;
  padding: 8px;
  border-radius: 8px;
  transition: all 0.3s ease;
  backdrop-filter: blur(10px);
}

.close-button:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: scale(1.1);
}

/* =========================
   MODAL FOOTER STYLES
   ========================= */

.modal-footer {
  padding: 25px 30px;
  background: #f8fafc;
  border-top: 1px solid #e2e8f0;
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.btn-primary,
.btn-secondary {
  padding: 12px 24px;
  border: none;
  border-radius: 10px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  font-size: 1rem;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  text-decoration: none;
}

.btn-primary {
  background: linear-gradient(135deg, #8B1538 0%, #A91B47 50%, #C02456 100%);
  color: white;
  box-shadow: 0 4px 12px rgba(139, 21, 56, 0.3);
  text-shadow: 0 1px 2px rgba(0,0,0,0.1);
}

.btn-primary:hover:not(:disabled) {
  background: linear-gradient(135deg, #A91B47 0%, #C02456 50%, #D42C65 100%);
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(139, 21, 56, 0.4);
}

.btn-primary:disabled {
  background: linear-gradient(135deg, #94a3b8, #64748b);
  cursor: not-allowed;
  transform: none;
  box-shadow: 0 2px 4px rgba(148, 163, 184, 0.2);
}

.btn-secondary {
  background: linear-gradient(135deg, #f1f5f9 0%, #e2e8f0 100%);
  color: #475569;
  border: 2px solid #e2e8f0;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.btn-secondary:hover {
  background: linear-gradient(135deg, #e2e8f0 0%, #cbd5e1 100%);
  color: #334155;
  border-color: #cbd5e1;
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

/* =========================
   RESPONSIVE DESIGN
   ========================= */

.percentage-step,
.score-per-step,
.max-score,
.min-score,
.unit-type {
  text-align: center;
}

.action-button.add-btn {
  background: linear-gradient(135deg, #8B1538 0%, #A91B47 50%, #C02456 100%);
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: inline-flex;
  align-items: center;
  gap: 10px;
  font-size: 1rem;
  text-shadow: 0 1px 2px rgba(0,0,0,0.1);
  box-shadow: 0 4px 12px rgba(139, 21, 56, 0.3);
}

.action-button.add-btn:hover {
  background: linear-gradient(135deg, #A91B47 0%, #C02456 50%, #D42C65 100%);
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(139, 21, 56, 0.4);
}

.action-button.add-btn:active {
  transform: translateY(0);
  box-shadow: 0 4px 12px rgba(139, 21, 56, 0.3);
}

.action-button.refresh-btn {
  background: linear-gradient(135deg, #64748b 0%, #475569 100%);
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 12px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: inline-flex;
  align-items: center;
  gap: 10px;
  font-size: 1rem;
  text-shadow: 0 1px 2px rgba(0,0,0,0.1);
  box-shadow: 0 4px 12px rgba(100, 116, 139, 0.2);
}

.action-button.refresh-btn:hover {
  background: linear-gradient(135deg, #475569 0%, #334155 100%);
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(100, 116, 139, 0.3);
}

.action-button.refresh-btn:active {
  transform: translateY(0);
  box-shadow: 0 4px 12px rgba(100, 116, 139, 0.2);
}

/* =========================
   SCORING RULES SUMMARY STYLES
   ========================= */

.scoring-rules-summary {
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
  border-radius: 16px;
  padding: 25px;
  margin-top: 20px;
  border: 1px solid rgba(139, 21, 56, 0.1);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
}

.summary-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
}

.stat-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 20px;
  background: white;
  border-radius: 12px;
  border: 1px solid #e2e8f0;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.stat-item::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  width: 4px;
  height: 100%;
  background: linear-gradient(135deg, #8B1538, #C02456);
  transition: width 0.3s ease;
}

.stat-item:hover {
  border-color: rgba(139, 21, 56, 0.2);
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(139, 21, 56, 0.1);
}

.stat-item:hover::before {
  width: 6px;
}

.stat-label {
  font-weight: 600;
  color: #475569;
  font-size: 0.95rem;
}

.stat-value {
  font-weight: 700;
  color: #8B1538;
  font-size: 1.2rem;
  font-family: 'JetBrains Mono', monospace;
  display: flex;
  align-items: center;
}

/* =========================
   EMPTY STATE STYLES
   ========================= */

.no-scoring-rules {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 400px;
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
  border-radius: 16px;
  border: 2px dashed rgba(139, 21, 56, 0.2);
  margin: 20px 0;
}

.empty-state {
  text-align: center;
  color: #64748b;
  max-width: 400px;
  padding: 40px 20px;
}

/* Responsive styles */
@media (max-width: 768px) {
  .main-layout {
    flex-direction: column;
    gap: 20px;
  }

  .left-panel {
    flex: none;
    position: relative;
    top: 0;
  }

  .scoring-rules-header {
    padding: 20px;
    text-align: center;
  }

  .scoring-rules-header h2 {
    font-size: 1.8rem;
  }

  .scoring-actions {
    flex-direction: column;
    gap: 12px;
  }

  .action-button.add-btn,
  .action-button.refresh-btn {
    width: 100%;
    justify-content: center;
  }

  .scoring-rule-modal {
    max-width: 95%;
    margin: 10px;
  }

  .modal-body {
    padding: 20px;
  }

  .modal-footer {
    padding: 20px;
    flex-direction: column-reverse;
  }

  .btn-primary,
  .btn-secondary {
    width: 100%;
    justify-content: center;
  }

  .form-row {
    flex-direction: column;
    gap: 15px;
  }

  .example-grid {
    gap: 8px;
  }

  .example-item {
    flex-direction: column;
    align-items: flex-start;
    gap: 5px;
    text-align: left;
  }

  .scoring-rules-table {
    font-size: 0.85rem;
  }

  .scoring-rules-table th,
  .scoring-rules-table td {
    padding: 12px 8px;
  }

  .summary-stats {
    grid-template-columns: 1fr;
    gap: 12px;
  }

  .stat-item {
    padding: 12px 16px;
  }
}

/* =========================
   SCORE VALIDATION STYLES
   ========================= */

.stat-value.score-valid {
  color: #28a745 !important;
  font-weight: 700;
}

.stat-value.score-invalid {
  color: #dc3545 !important;
  font-weight: 700;
}

.validation-icon {
  margin-left: 8px;
  font-size: 1rem;
}

.score-validation-message {
  margin-top: 15px;
  padding: 12px 16px;
  border-radius: 8px;
  font-weight: 500;
}

.validation-warning {
  background: linear-gradient(135deg, #fff3cd, #ffeaa7);
  border: 1px solid #ffeaa7;
  color: #856404;
  display: flex;
  align-items: center;
  gap: 10px;
}

.validation-success {
  background: linear-gradient(135deg, #d4edda, #a3d977);
  border: 1px solid #a3d977;
  color: #155724;
  display: flex;
  align-items: center;
  gap: 10px;
}

.warning-icon, .success-icon {
  font-size: 1.2rem;
  flex-shrink: 0;
}

.warning-text, .success-text {
  font-size: 0.95rem;
  line-height: 1.4;
}

.validation-warning .warning-text {
  color: #856404;
  font-weight: 600;
}

.validation-success .success-text {
  color: #155724;
  font-weight: 600;
}

/* Enhanced stat-item styles for better visual hierarchy */
.stat-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 0;
  border-bottom: 1px solid #e9ecef;
  transition: all 0.2s ease;
}

.stat-item:last-child {
  border-bottom: none;
}

.stat-item:hover {
  background-color: rgba(139, 21, 56, 0.02);
  border-radius: 6px;
  padding-left: 8px;
  padding-right: 8px;
}

.stat-label {
  font-weight: 600;
  color: #495057;
  font-size: 0.95rem;
}

.stat-value {
  font-weight: 700;
  color: #2c3e50;
  font-size: 1.1rem;
  display: flex;
  align-items: center;
}

/* Table code styling in dropdown */
.table-dropdown option {
  padding: 8px 12px;
  font-size: 14px;
}

.table-code {
  display: inline-block;
  margin-left: 8px;
  font-weight: 600;
  color: #8B1538;
  background-color: rgba(139, 21, 56, 0.1);
  padding: 2px 6px;
  border-radius: 4px;
  font-size: 0.85em;
}

/* Add styling for detail-view table code */
.detail-item .value .table-code {
  display: inline-block;
  margin-left: 6px;
  font-weight: 600;
  color: #8B1538;
  background-color: rgba(139, 21, 56, 0.1);
  padding: 2px 6px;
  border-radius: 4px;
  font-size: 0.85em;
}
</style>
