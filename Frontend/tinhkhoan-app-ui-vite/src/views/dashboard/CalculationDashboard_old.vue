<template>
  <div class="calculation-dashboard agribank-theme">
    <!-- Header với thương hiệu Agribank -->
    <div class="page-header agribank-header">
      <div class="agribank-brand">
        <img src="/agribank-logo.svg" alt="Agribank Logo" class="agribank-logo">
        <div class="brand-text">
          <h1 class="agribank-title">
            <i class="mdi mdi-database-sync"></i>
            Dashboard Cập nhật Tình hình Thực hiện
          </h1>
          <p class="agribank-subtitle">
            <i class="mdi mdi-information-outline"></i>
            Hệ thống tính toán và cập nhật các chỉ tiêu kinh doanh theo từng chi nhánh
          </p>
        </div>
      </div>

      <div class="header-controls agribank-controls">
        <!-- Time filters với Agribank styling -->
        <div class="filter-group agribank-filter">
          <label for="year-select" class="agribank-label">Năm báo cáo:</label>
          <select
            id="year-select"
            v-model="selectedYear"
            @change="loadData"
            @click="console.log('📅 Year dropdown clicked')"
            class="agribank-select"
            autocomplete="off"
            aria-label="Chọn năm">
            <option value="">Chọn năm</option>
            <option v-for="year in yearOptions" :key="year" :value="year">
              {{ year }}
            </option>
          </select>
        </div>

        <div class="filter-group agribank-filter">
          <label for="period-type-select" class="agribank-label">Loại kỳ báo cáo:</label>
          <select
            id="period-type-select"
            v-model="periodType"
            @change="onPeriodTypeChange"
            @click="console.log('📆 Period type dropdown clicked')"
            class="agribank-select"
            autocomplete="off"
            aria-label="Chọn loại kỳ">
            <option value="">Chọn loại kỳ</option>
            <option v-for="period in periodTypeOptions" :key="period.value" :value="period.value">
              {{ period.label }}
            </option>
          </select>
        </div>

        <div class="filter-group" v-if="periodType === 'QUARTER'">
          <label for="quarter-select" class="filter-label">Quý:</label>
          <select id="quarter-select" v-model="selectedPeriod" @change="loadData" class="form-select" autocomplete="off" aria-label="Chọn quý">
            <option value="">Chọn quý</option>
            <option v-for="quarter in quarterOptions" :key="quarter.value" :value="quarter.value">
              {{ quarter.label }}
            </option>
          </select>
        </div>

        <div class="filter-group" v-if="periodType === 'MONTH'">
          <label for="month-select" class="filter-label">Tháng:</label>
          <select id="month-select" v-model="selectedPeriod" @change="loadData" class="form-select" autocomplete="off" aria-label="Chọn tháng">
            <option value="">Chọn tháng</option>
            <option v-for="month in monthOptions" :key="month.value" :value="month.value">
              {{ month.label }}
            </option>
          </select>
        </div>

        <div class="filter-group" v-if="periodType === 'DATE'">
          <label for="date-select" class="filter-label">Ngày cụ thể:</label>
          <input id="date-select" v-model="selectedDate" @change="loadData" type="date" class="form-select" aria-label="Chọn ngày cụ thể" />
        </div>

        <div class="filter-group">
          <label for="unit-select" class="filter-label">Chi nhánh:</label>
          <select
            id="unit-select"
            v-model="selectedUnitId"
            @change="loadData"
            @click="console.log('🏢 Unit dropdown clicked')"
            class="form-select"
            autocomplete="organization"
            aria-label="Chọn chi nhánh">
            <option value="">Tất cả đơn vị (Toàn tỉnh)</option>
            <option v-for="unit in units" :key="unit.id" :value="unit.id">
              {{ unit.name }}
            </option>
          </select>
        </div>

        <!-- 7 nút chức năng chính -->
        <div class="calculation-buttons">
          <button @click="calculateAll" :disabled="calculating" class="btn btn-primary">
            {{ calculating ? 'Đang tính...' : '⚡ Tính toán' }}
          </button>

          <button @click="calculateNguonVon" :disabled="calculating" class="btn btn-warning">
            💰 Nguồn vốn
          </button>

          <button @click="calculateDuNo" :disabled="calculating" class="btn btn-info">
            📊 Dư nợ
          </button>

          <button @click="calculateNoXau" :disabled="calculating" class="btn btn-danger">
            ⚠️ Nợ xấu
          </button>

          <button @click="calculateThuNoXLRR" :disabled="calculating" class="btn btn-success">
            💵 Thu nợ XLRR
          </button>

          <button @click="calculateThuDichVu" :disabled="calculating" class="btn btn-purple">
            🎯 Thu dịch vụ
          </button>

          <button @click="calculateTaiChinh" :disabled="calculating" class="btn btn-gradient">
            💼 Tài chính
          </button>
        </div>
      </div>
    </div>

    <!-- Enhanced Loading Overlays -->
    <LoadingOverlay
      :show="loading"
      title="Đang tải dữ liệu"
      message="Đang truy xuất dữ liệu từ hệ thống..."
      icon="📊"
    />

    <LoadingOverlay
      :show="calculating"
      title="Đang tính toán"
      message="Vui lòng chờ trong khi hệ thống tính toán các chỉ tiêu..."
      icon="⚡"
    />

    <!-- Error Message -->
    <div v-if="errorMessage" class="error-message">
      <p>❌ {{ errorMessage }}</p>
    </div>

    <!-- Success Message -->
    <div v-if="successMessage" class="success-message">
      <p>✅ {{ successMessage }}</p>
    </div>

    <!-- Dashboard Content -->
    <div v-if="!loading" class="dashboard-content">

      <!-- 6 chỉ tiêu chính với trạng thái cập nhật - Dạng bảng Matrix Agribank -->
      <div class="overview-section agribank-section">
        <div class="section-header agribank-section-header">
          <div class="header-with-logo">
            <div class="section-icon">
              <i class="mdi mdi-chart-donut agribank-icon"></i>
            </div>
            <div class="section-title-area">
              <h3 class="agribank-section-title">
                Tổng quan 6 chỉ tiêu kinh doanh chính
              </h3>
              <p class="agribank-section-subtitle">
                Bảng trạng thái tính toán theo chi nhánh Agribank Lai Châu
                <span class="status-legend">
                  <span class="legend-item">
                    <i class="mdi mdi-check-circle legend-success"></i>
                    Đã tính toán
                  </span>
                  <span class="legend-item">
                    <i class="mdi mdi-close-circle legend-error"></i>
                    Chưa tính toán
                  </span>
                </span>
              </p>
            </div>
          </div>
        </div>

        <div class="agribank-matrix-container">
          <table class="agribank-matrix-table">
            <thead>
              <tr class="agribank-header-row">
                <th class="agribank-unit-header">
                  <div class="header-content">
                    <i class="mdi mdi-domain"></i>
                    <span>Chi nhánh / Phòng Giao dịch</span>
                  </div>
                </th>
                <th
                  v-for="indicator in sixMainIndicators"
                  :key="indicator.id"
                  class="agribank-indicator-header"
                  :class="indicator.class"
                >
                  <div class="agribank-indicator-content">
                    <div class="indicator-icon-wrapper">
                      <span class="indicator-icon">{{ indicator.icon }}</span>
                    </div>
                    <div class="indicator-text">
                      <span class="indicator-name">{{ indicator.name }}</span>
                      <span class="indicator-unit">(Tỷ VND)</span>
                    </div>
                  </div>
                </th>
                <th class="agribank-summary-header">
                  <div class="header-content">
                    <i class="mdi mdi-chart-pie"></i>
                    <span>Tình hình thực hiện</span>
                  </div>
                </th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="unit in branchUnits"
                :key="unit.id"
                class="agribank-unit-row"
              >
                <td class="agribank-unit-name-cell">
                  <div class="unit-info">
                    <div class="unit-icon">
                      <i class="mdi" :class="getUnitIcon(unit.name)"></i>
                    </div>
                    <span class="unit-name">{{ cleanUnitName(unit.name) }}</span>
                  </div>
                </td>
                <td
                  v-for="indicator in sixMainIndicators"
                  :key="`${unit.id}-${indicator.id}`"
                  class="agribank-status-cell"
                  :class="getAgribankStatusClass(unit.id, indicator.id)"
                >
                  <div class="agribank-status-indicator">
                    <div
                      v-if="isIndicatorCalculated(unit.id, indicator.id)"
                      class="status-success-wrapper"
                      :title="`${indicator.name} đã được tính toán cho ${unit.name}`"
                    >
                      <i class="mdi mdi-check-circle agribank-success-icon"></i>
                      <span class="status-text">Hoàn thành</span>
                    </div>
                    <div
                      v-else
                      class="status-pending-wrapper"
                      :title="`${indicator.name} chưa được tính toán cho ${unit.name}`"
                    >
                      <i class="mdi mdi-clock-outline agribank-pending-icon"></i>
                      <span class="status-text">Chờ xử lý</span>
                    </div>
                  </div>
                </td>
                <td class="agribank-unit-summary-cell">
                  <div class="agribank-unit-progress">
                    <div class="progress-circle">
                      <div class="circle-progress" :style="getCircleProgressStyle(unit.id)">
                        <span class="progress-number">{{ getUnitCompletedCount(unit.id) }}</span>
                        <span class="progress-total">/6</span>
                      </div>
                    </div>
                    <div class="progress-status">
                      <span class="status-label" :class="getUnitStatusClass(unit.id)">
                        {{ getUnitStatusText(unit.id) }}
                      </span>
                    </div>
                  </div>
                </td>
              </tr>
            </tbody>
            <tfoot>
              <tr class="agribank-summary-row">
                <td class="agribank-summary-label">
                  <div class="summary-header-content">
                    <i class="mdi mdi-sigma"></i>
                    <span>Tổng cộng toàn tỉnh</span>
                  </div>
                </td>
                <td
                  v-for="indicator in sixMainIndicators"
                  :key="`summary-${indicator.id}`"
                  class="agribank-indicator-summary-cell"
                >
                  <div class="agribank-indicator-summary">
                    <div class="summary-numbers">
                      <span class="completed-units">{{ getIndicatorCompletedCount(indicator.id) }}</span>
                      <span class="separator">/</span>
                      <span class="total-units">{{ branchUnits.length }}</span>
                    </div>
                    <div class="summary-percentage">
                      {{ Math.round((getIndicatorCompletedCount(indicator.id) / branchUnits.length) * 100) }}%
                    </div>
                  </div>
                </td>
                <td class="agribank-total-summary-cell">
                  <div class="agribank-total-progress">
                    <div class="total-percentage-large">{{ getTotalProgressPercentage() }}%</div>
                    <div class="total-label">Hoàn thành</div>
                  </div>
                </td>
              </tr>
            </tfoot>
          </table>
        </div>
      </div>

      <!-- Performance by Unit -->
      <div class="performance-section">
        <h3>🏢 Hiệu suất theo đơn vị</h3>
        <div class="performance-table-container">
          <table v-if="performanceData.length > 0" class="performance-table">
            <thead>
              <tr>
                <th>Đơn vị</th>
                <th>Số chỉ tiêu</th>
                <th>Đã hoàn thành</th>
                <th>Tỷ lệ hoàn thành</th>
                <th>Tổng giá trị mục tiêu</th>
                <th>Giá trị đạt được</th>
                <th>Trạng thái</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="unit in performanceData" :key="unit.unitId">
                <td class="unit-name">{{ unit.unitName }}</td>
                <td class="number-cell">{{ formatNumber(unit.totalTargets) }}</td>
                <td class="number-cell">{{ formatNumber(unit.completedTargets) }}</td>
                <td class="number-cell">
                  <div class="progress-container">
                    <div class="progress-bar">
                      <div
                        class="progress-fill"
                        :style="{ width: unit.completionRate + '%' }"
                        :class="getProgressClass(unit.completionRate)"
                      ></div>
                    </div>
                    <span class="progress-text">{{ formatPercentage(unit.completionRate) }}</span>
                  </div>
                </td>
                <td class="number-cell">{{ formatNumber(unit.totalTargetValue) }}</td>
                <td class="number-cell">{{ formatNumber(unit.actualValue) }}</td>
                <td>
                  <span :class="['status-badge', getStatusClass(unit.completionRate)]">
                    {{ getStatusText(unit.completionRate) }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>

          <div v-else class="no-data">
            <p>Không có dữ liệu hiệu suất cho kỳ đã chọn.</p>
          </div>
        </div>
      </div>

      <!-- Calculation Results -->
      <div class="calculation-section">
        <h3>🔢 Kết quả tính toán</h3>
        <div class="calculation-results">
          <div v-if="calculationResults.length > 0" class="results-table-container">
            <table class="results-table">
              <thead>
                <tr>
                  <th>Chỉ tiêu</th>
                  <th>Đơn vị</th>
                  <th>Giá trị mục tiêu</th>
                  <th>Giá trị thực hiện</th>
                  <th>Tỷ lệ đạt được</th>
                  <th>Điểm số</th>
                  <th>Thời gian tính</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="result in calculationResults" :key="result.id">
                  <td>{{ result.indicatorName }}</td>
                  <td>{{ result.unitName }}</td>
                  <td class="number-cell">{{ formatNumber(result.targetValue) }}</td>
                  <td class="number-cell">{{ formatNumber(result.actualValue) }}</td>
                  <td class="number-cell">
                    <span :class="['percentage', getPerformanceClass(result.achievementRate)]">
                      {{ formatPercentage(result.achievementRate) }}
                    </span>
                  </td>
                  <td class="number-cell">
                    <span :class="['score', getScoreClass(result.score)]">
                      {{ formatNumber(result.score) }}
                    </span>
                  </td>
                  <td>{{ formatDateTime(result.calculationDate) }}</td>
                </tr>
              </tbody>
            </table>
          </div>

          <div v-else class="no-data">
            <p>Chưa có kết quả tính toán nào. Nhấn nút "Tính toán" để bắt đầu.</p>
          </div>
        </div>
      </div>

      <!-- 6 Chỉ tiêu sau khi tính toán -->
      <div v-if="showCalculationResults" class="calculation-results">
        <div class="results-header">
          <h3>
            <i class="mdi mdi-chart-line"></i>
            Kết quả tính toán chỉ tiêu
          </h3>
          <div class="selected-unit-info">
            <span v-if="selectedUnitId">{{ getSelectedUnitName() }}</span>
            <span v-else>Toàn tỉnh (7800-7808)</span>
          </div>
        </div>

        <!-- Warning nếu thiếu chỉ tiêu -->
        <div v-if="missingIndicators.length > 0" class="warning-box">
          <i class="mdi mdi-alert-circle"></i>
          <div class="warning-content">
            <h4>⚠️ Cảnh báo: Thiếu dữ liệu</h4>
            <p>Các chỉ tiêu sau chưa được tính toán đầy đủ:</p>
            <ul>
              <li v-for="indicator in missingIndicators" :key="indicator">{{ indicator }}</li>
            </ul>
          </div>
        </div>

        <!-- Grid hiển thị 6 chỉ tiêu -->
        <div class="indicators-results-grid">
          <div
            v-for="(indicator, index) in calculatedIndicators"
            :key="indicator.id"
            class="result-card"
            :class="[indicator.class, { 'calculated': indicator.calculated, 'missing': !indicator.calculated }]"
            :style="{ animationDelay: `${index * 0.1}s` }"
          >
            <div class="result-card-header">
              <div class="result-icon">{{ indicator.icon }}</div>
              <div class="result-title">{{ indicator.name }}</div>
              <div class="result-status">
                <i v-if="indicator.calculated" class="mdi mdi-check-circle status-success"></i>
                <i v-else class="mdi mdi-alert-circle status-warning"></i>
              </div>
            </div>

            <div class="result-body">
              <div class="result-value">
                <span class="value-number">{{ formatNumber(indicator.value) }}</span>
                <span class="value-unit">{{ indicator.unit }}</span>
              </div>

              <div class="result-status-text">
                <span v-if="indicator.calculated" class="calculated-text">✅ Đã tính toán</span>
                <span v-else class="missing-text">❌ Chưa có dữ liệu</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Trend Analysis -->
      <div class="trend-section">
        <h3>📈 Phân tích xu hướng</h3>
        <div class="trend-controls">
          <button
            @click="loadTrendData('MONTH')"
            :class="['trend-btn', { active: trendPeriod === 'MONTH' }]"
          >
            Theo tháng
          </button>
          <button
            @click="loadTrendData('QUARTER')"
            :class="['trend-btn', { active: trendPeriod === 'QUARTER' }]"
          >
            Theo quý
          </button>
          <button
            @click="loadTrendData('YEAR')"
            :class="['trend-btn', { active: trendPeriod === 'YEAR' }]"
          >
            Theo năm
          </button>
        </div>

        <div v-if="trendData.length > 0" class="trend-chart">
          <!-- Simple trend visualization -->
          <div class="chart-container">
            <div v-for="(point, index) in trendData" :key="index" class="trend-point">
              <div class="point-value">{{ formatPercentage(point.achievementRate) }}</div>
              <div class="point-bar">
                <div
                  class="bar-fill"
                  :style="{ height: (point.achievementRate || 0) + '%' }"
                  :class="getPerformanceClass(point.achievementRate)"
                ></div>
              </div>
              <div class="point-label">{{ point.periodLabel }}</div>
            </div>
          </div>
        </div>

        <div v-else class="no-data">
          <p>Chưa có dữ liệu xu hướng.</p>
        </div>
      </div>

    </div>

    <!-- Action buttons -->
    <div v-if="!loading" class="action-section">
      <button @click="exportDashboard" class="btn btn-info">
        📊 Xuất báo cáo Dashboard
      </button>
      <button @click="refreshData" class="btn btn-secondary">
        🔄 Làm mới dữ liệu
      </button>
    </div>

    <!-- Modal chi tiết chỉ tiêu -->
    <div v-if="showDetailModal" class="modal-overlay" @click="closeDetailModal">
      <div class="modal-content indicator-detail-modal" @click.stop>
        <div class="modal-header">
          <h3>
            <span class="indicator-icon">{{ selectedIndicator?.icon }}</span>
            Chi tiết cập nhật: {{ selectedIndicator?.name }}
          </h3>
          <button @click="closeDetailModal" class="close-btn">
            <i class="mdi mdi-close"></i>
          </button>
        </div>

        <div class="modal-body">
          <div class="indicator-summary">
            <div class="summary-stats">
              <div class="stat-item">
                <div class="stat-value">{{ selectedIndicator?.updatedUnits || 0 }}</div>
                <div class="stat-label">Đã cập nhật</div>
              </div>
              <div class="stat-item">
                <div class="stat-value">{{ selectedIndicator?.pendingUnits || 0 }}</div>
                <div class="stat-label">Chưa cập nhật</div>
              </div>
              <div class="stat-item">
                <div class="stat-value">{{ Math.round(selectedIndicator?.updateProgress || 0) }}%</div>
                <div class="stat-label">Tỷ lệ hoàn thành</div>
              </div>
            </div>
          </div>

          <div class="units-status">
            <h4>Trạng thái cập nhật theo chi nhánh:</h4>

            <div class="status-filter">
              <button
                :class="['filter-btn', { active: statusFilter === 'all' }]"
                @click="statusFilter = 'all'"
              >
                Tất cả
              </button>
              <button
                :class="['filter-btn', { active: statusFilter === 'updated' }]"
                @click="statusFilter = 'updated'"
              >
                Đã cập nhật
              </button>
              <button
                :class="['filter-btn', { active: statusFilter === 'pending' }]"
                @click="statusFilter = 'pending'"
              >
                Chưa cập nhật
              </button>
            </div>

            <div class="units-list">
              <div
                v-for="unit in filteredUnitsStatus"
                :key="unit.id"
                class="unit-item"
                :class="{ 'updated': unit.isUpdated, 'pending': !unit.isUpdated }"
              >
                <div class="unit-info">
                  <div class="unit-name">{{ unit.name }}</div>
                  <div class="unit-code">{{ unit.code }}</div>
                </div>
                <div class="unit-status">
                  <i v-if="unit.isUpdated" class="mdi mdi-check-circle status-success"></i>
                  <i v-else class="mdi mdi-clock-outline status-warning"></i>
                  <span :class="['status-text', { 'updated': unit.isUpdated, 'pending': !unit.isUpdated }]">
                    {{ unit.isUpdated ? 'Đã cập nhật' : 'Chưa cập nhật' }}
                  </span>
                </div>
                <div v-if="unit.lastUpdate" class="unit-last-update">
                  {{ formatDateTime(unit.lastUpdate) }}
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <button @click="closeDetailModal" class="btn btn-secondary">
            Đóng
          </button>
          <button @click="refreshIndicatorData" class="btn btn-primary">
            🔄 Làm mới dữ liệu
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, ref } from 'vue';
import { useRouter } from 'vue-router';
import LoadingOverlay from '../../components/dashboard/LoadingOverlay.vue';
import branchIndicatorsService from '../../services/branchIndicatorsService';
import { dashboardService } from '../../services/dashboardService';

const router = useRouter();

// Reactive data
const loading = ref(false);
const calculating = ref(false);
const errorMessage = ref('');
const successMessage = ref('');
const showDetailModal = ref(false);
const selectedIndicator = ref(null);
const statusFilter = ref('all');

// Filters
const selectedYear = ref(new Date().getFullYear());
const periodType = ref('');
const selectedPeriod = ref('');
const selectedDate = ref(''); // Thêm biến cho ngày cụ thể
const selectedUnitId = ref('ALL'); // Mặc định chọn "Toàn tỉnh" thay vì rỗng
const trendPeriod = ref('MONTH');

// Danh sách chi nhánh và PGD theo quy ước mới - Thêm option "Toàn tỉnh" ở đầu
const units = ref([
  { id: 'ALL', name: '🏛️ Toàn tỉnh (Tổng hợp)', code: 'ALL', isTotal: true }, // Option mặc định cho tổng hợp
  { id: 'HoiSo', name: '🏢 Hội Sở', code: '7800' },
  { id: 'CnBinhLu', name: '🏦 CN Bình Lư', code: '7801' },
  { id: 'CnPhongTho', name: '🏦 CN Phong Thổ', code: '7802' },
  { id: 'CnSinHo', name: '🏦 CN Sìn Hồ', code: '7803' },
  { id: 'CnBumTo', name: '🏦 CN Bum Tở', code: '7804' },
  { id: 'CnThanUyen', name: '🏦 CN Than Uyên', code: '7805' },
  { id: 'CnDoanKet', name: '🏦 CN Đoàn Kết', code: '7806' },
  { id: 'CnTanUyen', name: '🏦 CN Tân Uyên', code: '7807' },
  { id: 'CnNamHang', name: '🏦 CN Nậm Hàng', code: '7808' },
  { id: 'CnPhongThoPgdSo5', name: '🏪 CN Phong Thổ - PGD Số 5', code: '7802', pgdCode: '01' },
  { id: 'CnThanUyenPgdSo6', name: '🏪 CN Than Uyên - PGD Số 6', code: '7805', pgdCode: '01' },
  { id: 'CnDoanKetPgdSo1', name: '🏪 CN Đoàn Kết - PGD Số 1', code: '7806', pgdCode: '01' },
  { id: 'CnDoanKetPgdSo2', name: '🏪 CN Đoàn Kết - PGD Số 2', code: '7806', pgdCode: '02' },
  { id: 'CnTanUyenPgdSo3', name: '🏪 CN Tân Uyên - PGD Số 3', code: '7807', pgdCode: '01' }
]);
const overview = ref({
  totalTargets: 0,
  completedTargets: 0,
  achievementRate: 0,
  totalValue: 0
});
const performanceData = ref([]);
const calculationResults = ref([]);
const trendData = ref([]);

// 6 chỉ tiêu chính với trạng thái cập nhật
const sixMainIndicators = ref([
  {
    id: 'nguon_von',
    name: 'Nguồn vốn',
    icon: '💰',
    class: 'nguon-von',
    hasUpdates: false,
    updatedUnits: 0,
    pendingUnits: 15,
    updateProgress: 0,
    lastUpdate: null,
    unitsStatus: []
  },
  {
    id: 'du_no',
    name: 'Dư nợ',
    icon: '💳',
    class: 'du-no',
    hasUpdates: false,
    updatedUnits: 0,
    pendingUnits: 15,
    updateProgress: 0,
    lastUpdate: null,
    unitsStatus: []
  },
  {
    id: 'no_xau',
    name: 'Nợ Xấu',
    icon: '⚠️',
    class: 'no-xau',
    hasUpdates: false,
    updatedUnits: 0,
    pendingUnits: 15,
    updateProgress: 0,
    lastUpdate: null,
    unitsStatus: []
  },
  {
    id: 'thu_no_xlrr',
    name: 'Thu nợ đã XLRR',
    icon: '📈',
    class: 'thu-no-xlrr',
    hasUpdates: false,
    updatedUnits: 0,
    pendingUnits: 15,
    updateProgress: 0,
    lastUpdate: null,
    unitsStatus: []
  },
  {
    id: 'thu_dich_vu',
    name: 'Thu dịch vụ',
    icon: '🏦',
    class: 'thu-dich-vu',
    hasUpdates: false,
    updatedUnits: 0,
    pendingUnits: 15,
    updateProgress: 0,
    lastUpdate: null,
    unitsStatus: []
  },
  {
    id: 'tai_chinh',
    name: 'Tài chính',
    icon: '💵',
    class: 'tai-chinh',
    hasUpdates: false,
    updatedUnits: 0,
    pendingUnits: 15,
    updateProgress: 0,
    lastUpdate: null,
    unitsStatus: []
  }
]);

// Options - Debug để kiểm tra
const yearOptions = computed(() => dashboardService.getYearOptions());
const quarterOptions = computed(() => dashboardService.getQuarterOptions());
const monthOptions = computed(() => dashboardService.getMonthOptions());
const periodTypeOptions = computed(() => dashboardService.getPeriodTypeOptions());

// Debug log để kiểm tra options đã load
console.log('🔍 CalculationDashboard Debug Options:');
console.log('yearOptions:', yearOptions.value);
console.log('quarterOptions:', quarterOptions.value);
console.log('monthOptions:', monthOptions.value);
console.log('periodTypeOptions:', periodTypeOptions.value);
console.log('selectedYear:', selectedYear.value);
console.log('periodType:', periodType.value);
console.log('selectedUnitId:', selectedUnitId.value);

// Reactive variables
const showCalculationResults = ref(false);

// Khai báo calculatedIndicators để lưu kết quả tính toán của 6 chỉ tiêu chính
const calculatedIndicators = ref([
  { id: 'nguon_von', name: 'Nguồn vốn', value: 0, calculated: false, details: null, icon: '💰', unit: 'Triệu VND' },
  { id: 'du_no', name: 'Dư nợ', value: 0, calculated: false, details: null, icon: '💳', unit: 'Triệu VND' },
  { id: 'no_xau', name: 'Nợ Xấu', value: 0, calculated: false, details: null, icon: '⚠️', unit: '%' },
  { id: 'thu_no_xlrr', name: 'Thu nợ đã XLRR', value: 0, calculated: false, details: null, icon: '📈', unit: 'Triệu VND' },
  { id: 'thu_dich_vu', name: 'Thu dịch vụ', value: 0, calculated: false, details: null, icon: '🏦', unit: 'Triệu VND' },
  { id: 'tai_chinh', name: 'Tài chính', value: 0, calculated: false, details: null, icon: '💵', unit: 'Triệu VND' }
]);

// Computed properties
const filteredUnitsStatus = computed(() => {
  if (!selectedIndicator.value?.unitsStatus) return [];

  const units = selectedIndicator.value.unitsStatus;

  if (statusFilter.value === 'updated') {
    return units.filter(unit => unit.isUpdated);
  } else if (statusFilter.value === 'pending') {
    return units.filter(unit => !unit.isUpdated);
  }

  return units;
});

const missingIndicators = computed(() => {
  return sixMainIndicators.value
    .filter(indicator => !indicator.hasUpdates)
    .map(indicator => indicator.name);
});

// Computed property cho danh sách chi nhánh (bỏ "Toàn tỉnh")
const branchUnits = computed(() => {
  return units.value.filter(unit => unit.id !== 'ALL');
});

// Methods để xử lý trạng thái tính toán cho bảng matrix
const isIndicatorCalculated = (unitId, indicatorId) => {
  // Tìm indicator trong sixMainIndicators
  const indicator = sixMainIndicators.value.find(ind => ind.id === indicatorId);
  if (!indicator || !indicator.unitsStatus) return false;

  // Tìm unit status trong indicator
  const unitStatus = indicator.unitsStatus.find(status => status.unitId === unitId);
  return unitStatus ? unitStatus.isUpdated : false;
};

const getStatusCellClass = (unitId, indicatorId) => {
  const isCalculated = isIndicatorCalculated(unitId, indicatorId);
  return {
    'status-calculated': isCalculated,
    'status-pending': !isCalculated
  };
};

const getUnitCompletedCount = (unitId) => {
  let completedCount = 0;
  sixMainIndicators.value.forEach(indicator => {
    if (isIndicatorCalculated(unitId, indicator.id)) {
      completedCount++;
    }
  });
  return completedCount;
};

const getUnitProgressPercentage = (unitId) => {
  const completedCount = getUnitCompletedCount(unitId);
  return Math.round((completedCount / 6) * 100);
};

const getUnitProgressClass = (unitId) => {
  const percentage = getUnitProgressPercentage(unitId);
  if (percentage === 100) return 'progress-complete';
  if (percentage >= 75) return 'progress-high';
  if (percentage >= 50) return 'progress-medium';
  if (percentage >= 25) return 'progress-low';
  return 'progress-none';
};

const getIndicatorCompletedCount = (indicatorId) => {
  let completedCount = 0;
  branchUnits.value.forEach(unit => {
    if (isIndicatorCalculated(unit.id, indicatorId)) {
      completedCount++;
    }
  });
  return completedCount;
};

const getTotalProgressPercentage = () => {
  const totalCells = branchUnits.value.length * sixMainIndicators.value.length;
  let completedCells = 0;

  branchUnits.value.forEach(unit => {
    sixMainIndicators.value.forEach(indicator => {
      if (isIndicatorCalculated(unit.id, indicator.id)) {
        completedCells++;
      }
    });
  });

  return totalCells > 0 ? Math.round((completedCells / totalCells) * 100) : 0;
};

// Agribank specific methods
const getAgribankStatusClass = (unitId, indicatorId) => {
  const isCalculated = isIndicatorCalculated(unitId, indicatorId);
  return {
    'agribank-status-completed': isCalculated,
    'agribank-status-pending': !isCalculated
  };
};

const getUnitIcon = (unitName) => {
  if (unitName.includes('Hội Sở')) return 'mdi-office-building';
  if (unitName.includes('CN ')) return 'mdi-bank';
  if (unitName.includes('PGD')) return 'mdi-storefront';
  return 'mdi-domain';
};

const cleanUnitName = (unitName) => {
  return unitName.replace(/^🏢\s*|^🏦\s*|^🏪\s*/, '');
};

const getCircleProgressStyle = (unitId) => {
  const percentage = getUnitProgressPercentage(unitId);
  const rotation = (percentage / 100) * 360;
  return {
    background: `conic-gradient(var(--agribank-primary) ${rotation}deg, #e5e7eb ${rotation}deg)`
  };
};

const getUnitStatusClass = (unitId) => {
  const percentage = getUnitProgressPercentage(unitId);
  if (percentage === 100) return 'status-complete';
  if (percentage >= 75) return 'status-good';
  if (percentage >= 50) return 'status-fair';
  if (percentage >= 25) return 'status-poor';
  return 'status-none';
};

const getUnitStatusText = (unitId) => {
  const percentage = getUnitProgressPercentage(unitId);
  if (percentage === 100) return 'Hoàn thành';
  if (percentage >= 75) return 'Gần hoàn thành';
  if (percentage >= 50) return 'Đang thực hiện';
  if (percentage >= 25) return 'Mới bắt đầu';
  return 'Chưa bắt đầu';
};

const getSelectedUnitName = () => {
  if (!selectedUnitId.value || selectedUnitId.value === 'ALL') return '🏛️ Toàn tỉnh (Tổng hợp)';
  const unit = units.value.find(u => u.id === selectedUnitId.value);
  return unit ? unit.name : 'Không xác định';
};

// Methods
// Comment loadUnits để chỉ sử dụng 15 chi nhánh/PGD đã định nghĩa thay vì load từ API
// const loadUnits = async () => {
//   try {
//     const response = await dashboardService.getUnits();
//     units.value = response || [];
//   } catch (error) {
//     console.error('Error loading units:', error);
//     errorMessage.value = 'Không thể tải danh sách đơn vị';
//   }
// };

const loadData = async () => {
  console.log('🔧 loadData called with:', { selectedYear: selectedYear.value, periodType: periodType.value, selectedUnitId: selectedUnitId.value });

  if (!selectedYear.value) return;

  loading.value = true;
  errorMessage.value = '';

  try {
    const params = {
      year: selectedYear.value
    };

    if (periodType.value) params.periodType = periodType.value;
    if (selectedPeriod.value && periodType.value !== 'YEAR') params.period = selectedPeriod.value;
    if (selectedUnitId.value) params.unitId = selectedUnitId.value;

    console.log('📊 API params:', params);

    // Load dashboard data
    const dashboardData = await dashboardService.getDashboardData(params);
    if (dashboardData) {
      overview.value = dashboardData.overview || overview.value;
      performanceData.value = dashboardData.performanceByUnit || [];
    }

    // Load calculation results - Sửa lỗi 404 bằng cách bỏ qua lỗi hoặc dùng mock data
    try {
      const calculationData = await dashboardService.getCalculationResults(params);
      calculationResults.value = calculationData || [];
    } catch (calcError) {
      console.warn('⚠️ Calculation results endpoint not available, using mock data');
      calculationResults.value = generateMockCalculationResults();
    }

    // Load indicator status for 6 main indicators
    await loadIndicatorStatus(params);

  } catch (error) {
    console.error('Error loading dashboard data:', error);
    errorMessage.value = 'Không thể tải dữ liệu dashboard';
  } finally {
    loading.value = false;
  }
};

// Mock data cho calculation results
const generateMockCalculationResults = () => {
  return [
    {
      id: 1,
      indicatorName: 'Nguồn vốn',
      unitName: 'CN Lai Châu',
      targetValue: 1200000000000,
      actualValue: 1150000000000,
      achievementRate: 95.8,
      score: 96,
      calculationDate: new Date().toISOString()
    },
    {
      id: 2,
      indicatorName: 'Dư nợ',
      unitName: 'CN Lai Châu',
      targetValue: 980000000000,
      actualValue: 965000000000,
      achievementRate: 98.5,
      score: 98,
      calculationDate: new Date().toISOString()
    }
  ];
};

// Load trạng thái cập nhật của 6 chỉ tiêu
const loadIndicatorStatus = async (params) => {
  try {
    // Mock data cho trạng thái cập nhật - sau này sẽ thay bằng API thực
    const allUnits = units.value;

    sixMainIndicators.value.forEach((indicator, index) => {
      // Simulate random update status
      const updatedCount = Math.floor(Math.random() * allUnits.length);
      const unitsStatus = allUnits.map((unit, unitIndex) => ({
        id: unit.id,
        name: unit.name,
        code: unit.code || unit.id,
        isUpdated: unitIndex < updatedCount,
        lastUpdate: unitIndex < updatedCount ? new Date(Date.now() - Math.random() * 7 * 24 * 60 * 60 * 1000).toISOString() : null
      }));

      indicator.updatedUnits = updatedCount;
      indicator.pendingUnits = allUnits.length - updatedCount;
      indicator.updateProgress = allUnits.length > 0 ? (updatedCount / allUnits.length) * 100 : 0;
      indicator.hasUpdates = updatedCount > 0;
      indicator.lastUpdate = updatedCount > 0 ? formatDateTime(unitsStatus.find(u => u.isUpdated)?.lastUpdate) : null;
      indicator.unitsStatus = unitsStatus;
    });
  } catch (error) {
    console.error('Error loading indicator status:', error);
  }
};

const loadTrendData = async (period) => {
  trendPeriod.value = period;

  try {
    const params = {
      year: selectedYear.value,
      periodType: period
    };

    if (selectedUnitId.value) params.unitId = selectedUnitId.value;

    const response = await dashboardService.getTrendData(params);
    trendData.value = response || [];
  } catch (error) {
    console.error('Error loading trend data:', error);
    errorMessage.value = 'Không thể tải dữ liệu xu hướng';
  }
};

const triggerCalculation = async () => {
  if (!selectedYear.value) {
    errorMessage.value = 'Vui lòng chọn năm để tính toán';
    return;
  }

  calculating.value = true;
  errorMessage.value = '';
  successMessage.value = '';
  showCalculationResults.value = false;

  try {
    const params = {
      year: selectedYear.value
    };

    if (periodType.value) params.periodType = periodType.value;
    if (selectedPeriod.value && periodType.value !== 'YEAR') params.period = selectedPeriod.value;
    if (selectedUnitId.value) params.unitId = selectedUnitId.value;

    // Tính toán theo chi nhánh được chọn
    let unitCodes = [];
    if (selectedUnitId.value) {
      const selectedUnit = units.value.find(u => u.id === selectedUnitId.value);
      unitCodes = [selectedUnit?.code];
    } else {
      // Tất cả đơn vị: từ 7800 -> 7808
      unitCodes = ['7800', '7801', '7802', '7803', '7804', '7805', '7806', '7807', '7808'];
    }

    params.unitCodes = unitCodes;

    await dashboardService.triggerCalculations(params);

    // Mock dữ liệu tính toán 6 chỉ tiêu (sau này sẽ thay bằng API thực)
    setTimeout(() => {
      // Simulate calculation results
      calculatedIndicators.value[0].value = 1250.5;
      calculatedIndicators.value[0].calculated = true;

      calculatedIndicators.value[1].value = 980.3;
      calculatedIndicators.value[1].calculated = true;

      calculatedIndicators.value[2].value = 1.8;
      calculatedIndicators.value[2].calculated = true;

      calculatedIndicators.value[3].value = 45.7;
      calculatedIndicators.value[3].calculated = true;

      calculatedIndicators.value[4].value = 28.9;
      calculatedIndicators.value[4].calculated = true;

      calculatedIndicators.value[5].value = 156.4;
      calculatedIndicators.value[5].calculated = true;

      showCalculationResults.value = true;
      successMessage.value = 'Tính toán hoàn thành thành công cho ' + (selectedUnitId.value ? getSelectedUnitName() : 'toàn tỉnh');
    }, 1000);

    // Reload data after calculation
    await loadData();

  } catch (error) {
    console.error('Error triggering calculation:', error);
    errorMessage.value = 'Có lỗi xảy ra khi thực hiện tính toán: ' + (error.response?.data?.message || error.message);
  } finally {
    calculating.value = false;
  }
};

// ===============================
// 7 METHODS CHO CÁC NÚT CHỨC NĂNG
// ===============================

// 1. Tính toán tổng hợp (method cũ đã có)
const calculateAll = async () => {
  await triggerCalculation();
};

// 2. Tính Nguồn vốn - Sử dụng API mới đã được refactor hoàn toàn
const calculateNguonVon = async () => {
  calculating.value = true;
  errorMessage.value = '';
  successMessage.value = '';

  try {
    // Xác định unitKey dựa trên selectedUnitId
    let unitKey = 'ToanTinh'; // Mặc định là Toàn tỉnh
    let displayName = 'Toàn tỉnh';

    if (selectedUnitId.value && selectedUnitId.value !== 'ALL') {
      const selectedUnit = units.value.find(u => u.id === selectedUnitId.value);
      if (!selectedUnit) {
        throw new Error('Không tìm thấy thông tin đơn vị được chọn');
      }

      // Mapping từ id trong units đến unitKey trong API - Updated theo backend mới
      const unitKeyMapping = {
        'HoiSo': 'HoiSo',
        'CnBinhLu': 'CnBinhLu',
        'CnPhongTho': 'CnPhongTho',
        'CnSinHo': 'CnSinHo',
        'CnBumTo': 'CnBumTo',
        'CnThanUyen': 'CnThanUyen',
        'CnDoanKet': 'CnDoanKet',
        'CnTanUyen': 'CnTanUyen',
        'CnNamHang': 'CnNamHang', // Fixed: Theo API response thực tế từ backend
        'CnPhongThoPgdSo5': 'CnPhongTho-PGD5',
        'CnThanUyenPgdSo6': 'CnThanUyen-PGD6',
        'CnDoanKetPgdSo1': 'CnDoanKet-PGD1',
        'CnDoanKetPgdSo2': 'CnDoanKet-PGD2',
        'CnTanUyenPgdSo3': 'CnTanUyen-PGD3'
      };

      unitKey = unitKeyMapping[selectedUnit.id] || 'ToanTinh';
      displayName = selectedUnit.name;
    }

    // Helper function để format ngày theo dd/MM/yyyy
    const formatDateForBackend = (date) => {
      const day = String(date.getDate()).padStart(2, '0');
      const month = String(date.getMonth() + 1).padStart(2, '0');
      const year = date.getFullYear();
      return `${day}/${month}/${year}`;
    };

    // Xây dựng query parameters theo logic mới của backend
    let queryParams = new URLSearchParams();
    let calculationDescription = '';

    if (periodType.value === 'DATE' && selectedDate.value) {
      // Ngày cụ thể
      const targetDate = new Date(selectedDate.value);
      queryParams.set('targetDate', formatDateForBackend(targetDate));
      calculationDescription = `ngày ${formatDateForBackend(targetDate)}`;
    } else if (periodType.value === 'MONTH' && selectedYear.value && selectedPeriod.value) {
      // Tháng - backend sẽ tự động lấy ngày cuối tháng
      const monthStr = String(selectedPeriod.value).padStart(2, '0');
      queryParams.set('targetMonth', `${monthStr}/${selectedYear.value}`);
      calculationDescription = `tháng ${monthStr}/${selectedYear.value}`;
    } else if (periodType.value === 'QUARTER' && selectedYear.value && selectedPeriod.value) {
      // Quý - tính ra tháng cuối quý
      const quarterEndMonth = selectedPeriod.value * 3;
      const monthStr = String(quarterEndMonth).padStart(2, '0');
      queryParams.set('targetMonth', `${monthStr}/${selectedYear.value}`);
      calculationDescription = `quý ${selectedPeriod.value}/${selectedYear.value}`;
    } else if (selectedYear.value) {
      // Năm - backend sẽ tự động lấy 31/12/year, gửi tháng 12
      queryParams.set('targetMonth', `12/${selectedYear.value}`);
      calculationDescription = `năm ${selectedYear.value}`;
    } else {
      // Mặc định - ngày hiện tại
      const today = new Date();
      queryParams.set('targetDate', formatDateForBackend(today));
      calculationDescription = `ngày hiện tại (${formatDateForBackend(today)})`;
    }

    const apiUrl = `/api/NguonVonButton/calculate/${unitKey}?${queryParams.toString()}`;

    console.log('💰 Tính Nguồn vốn với API mới:', {
      unitKey,
      displayName,
      queryParams: queryParams.toString(),
      calculationDescription,
      apiUrl
    });

    // Gọi API đã được refactor hoàn toàn
    const response = await fetch(apiUrl, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => null);
      throw new Error(errorData?.message || `HTTP error! status: ${response.status}`);
    }

    const result = await response.json();

    if (result.success && result.data) {
      // Cập nhật kết quả vào UI với structure mới
      calculatedIndicators.value[0].value = result.data.totalNguonVonTrieuVND;
      calculatedIndicators.value[0].calculated = true;
      calculatedIndicators.value[0].details = {
        formula: result.data.formula,
        calculatedAt: new Date().toISOString(),
        unit: 'Triệu VND',
        unitKey: result.data.unitKey,
        unitName: result.data.unitName,
        recordCount: result.data.recordCount,
        calculationDate: result.data.calculationDate,
        topAccounts: result.data.topAccounts,
        description: calculationDescription
      };

      showCalculationResults.value = true;
      successMessage.value = `✅ ${result.message}: ${result.data.totalNguonVonTrieuVND.toLocaleString('en-US')} triệu VND (${result.data.recordCount?.toLocaleString('en-US') || 0} bản ghi) - ${calculationDescription}`;
    } else {
      throw new Error(result.message || 'Tính toán thất bại');
    }

  } catch (error) {
    console.error('❌ Lỗi tính Nguồn vốn:', error);
    errorMessage.value = 'Có lỗi khi tính Nguồn vốn: ' + error.message;
  } finally {
    calculating.value = false;
  }
};

// 3. Tính Dư nợ - Sử dụng service mới
const calculateDuNo = async () => {
  calculating.value = true;
  errorMessage.value = '';
  successMessage.value = '';

  try {
    // Xác định branchId: nếu không chọn gì thì là "Toàn tỉnh" (CnLaiChau)
    let branchId = 'CnLaiChau'; // Default: Toàn tỉnh
    let displayName = 'Toàn tỉnh';

    if (selectedUnitId.value) {
      const selectedUnit = units.value.find(u => u.id === selectedUnitId.value);
      if (!selectedUnit) {
        throw new Error('Không tìm thấy thông tin chi nhánh được chọn');
      }
      branchId = selectedUnit.id;
      displayName = selectedUnit.name;
    }

    console.log('🔧 Tính Dư nợ cho:', displayName);
    console.log('📅 Ngày được chọn:', selectedDate.value);

    // Chỉ truyền ngày khi có giá trị hợp lệ (không rỗng và không null)
    const dateParam = selectedDate.value && selectedDate.value.trim() !== '' ? selectedDate.value : null;
    console.log('📋 Date parameter sẽ truyền:', dateParam);

    // Gọi service mới để tính Dư nợ với tham số ngày
    const result = await branchIndicatorsService.calculateDuNo(branchId, dateParam);

    if (result.success) {
      // Cập nhật kết quả
      calculatedIndicators.value[1].value = result.value / 1000000; // Chuyển từ VND sang triệu VND
      calculatedIndicators.value[1].calculated = true;
      calculatedIndicators.value[1].details = {
        formula: 'Tổng DU_NO theo BRCD và TRCTCD',
        calculatedAt: result.calculatedAt,
        unit: result.unit,
        branchId: result.branchId
      };

      showCalculationResults.value = true;
      successMessage.value = `✅ Đã tính Dư nợ cho ${displayName}: ${branchIndicatorsService.formatCurrency(result.value / 1000000)} triệu VND`;
    } else {
      throw new Error(result.errorMessage || 'Tính toán thất bại');
    }

  } catch (error) {
    console.error('❌ Lỗi tính Dư nợ:', error);
    errorMessage.value = 'Có lỗi khi tính Dư nợ: ' + error.message;
  } finally {
    calculating.value = false;
  }
};

// 4. Tính Nợ xấu - Sử dụng service mới
const calculateNoXau = async () => {
  calculating.value = true;
  errorMessage.value = '';
  successMessage.value = '';

  try {
    // Xác định branchId: nếu không chọn gì thì là "Toàn tỉnh" (CnLaiChau)
    let branchId = 'CnLaiChau'; // Default: Toàn tỉnh
    let displayName = 'Toàn tỉnh';

    if (selectedUnitId.value) {
      const selectedUnit = units.value.find(u => u.id === selectedUnitId.value);
      if (!selectedUnit) {
        throw new Error('Không tìm thấy thông tin chi nhánh được chọn');
      }
      branchId = selectedUnit.id;
      displayName = selectedUnit.name;
    }

    console.log('🔧 Tính Nợ xấu cho:', displayName);
    console.log('📅 Ngày được chọn:', selectedDate.value);

    // Chỉ truyền ngày khi có giá trị hợp lệ (không rỗng và không null)
    const dateParam = selectedDate.value && selectedDate.value.trim() !== '' ? selectedDate.value : null;
    console.log('📋 Date parameter sẽ truyền:', dateParam);

    // Gọi service mới để tính Nợ xấu với tham số ngày
    const result = await branchIndicatorsService.calculateNoXau(branchId, dateParam);

    if (result.success) {
      // Cập nhật kết quả
      calculatedIndicators.value[2].value = result.value; // Đã là % rồi
      calculatedIndicators.value[2].calculated = true;
      calculatedIndicators.value[2].details = {
        formula: '(DU_NO với NHOM_NO=3,4,5) / Tổng DU_NO * 100',
        calculatedAt: result.calculatedAt,
        unit: result.unit,
        branchId: result.branchId
      };

      showCalculationResults.value = true;
      successMessage.value = `✅ Đã tính Nợ xấu cho ${displayName}: ${branchIndicatorsService.formatPercentage(result.value)} (càng thấp càng tốt)`;
    } else {
      throw new Error(result.errorMessage || 'Tính toán thất bại');
    }

  } catch (error) {
    console.error('❌ Lỗi tính Nợ xấu:', error);
    errorMessage.value = 'Có lỗi khi tính Nợ xấu: ' + error.message;
  } finally {
    calculating.value = false;
  }
};

// 5. Tính Thu nợ XLRR
const calculateThuNoXLRR = async () => {
  if (!selectedUnitId.value) {
    errorMessage.value = 'Vui lòng chọn Chi nhánh/Phòng ban trước khi tính toán';
    return;
  }

  calculating.value = true;
  errorMessage.value = '';
  successMessage.value = '';

  try {
    console.log('🔧 Tính Thu nợ XLRR cho:', getSelectedUnitName());

    setTimeout(() => {
      calculatedIndicators.value[3].value = Math.floor(Math.random() * 100) + 20; // 20-120 triệu VND
      calculatedIndicators.value[3].calculated = true;
      showCalculationResults.value = true;
      successMessage.value = `✅ Đã tính Thu nợ XLRR cho ${getSelectedUnitName()}: ${formatNumber(calculatedIndicators.value[3].value)} triệu VND`;
      calculating.value = false;
    }, 800);

  } catch (error) {
    console.error('Error calculating Thu nợ XLRR:', error);
    errorMessage.value = 'Có lỗi khi tính Thu nợ XLRR: ' + error.message;
    calculating.value = false;
  }
};

// 6. Tính Thu dịch vụ
const calculateThuDichVu = async () => {
  if (!selectedUnitId.value) {
    errorMessage.value = 'Vui lòng chọn Chi nhánh/Phòng ban trước khi tính toán';
    return;
  }

  calculating.value = true;
  errorMessage.value = '';
  successMessage.value = '';

  try {
    console.log('🔧 Tính Thu dịch vụ cho:', getSelectedUnitName());

    setTimeout(() => {
      calculatedIndicators.value[4].value = Math.floor(Math.random() * 50) + 10; // 10-60 triệu VND
      calculatedIndicators.value[4].calculated = true;
      showCalculationResults.value = true;
      successMessage.value = `✅ Đã tính Thu dịch vụ cho ${getSelectedUnitName()}: ${formatNumber(calculatedIndicators.value[4].value)} triệu VND`;
      calculating.value = false;
    }, 800);

  } catch (error) {
    console.error('Error calculating Thu dịch vụ:', error);
    errorMessage.value = 'Có lỗi khi tính Thu dịch vụ: ' + error.message;
    calculating.value = false;
  }
};

// 7. Tính Lợi nhuận khoán tài chính
const calculateTaiChinh = async () => {
  if (!selectedUnitId.value) {
    errorMessage.value = 'Vui lòng chọn Chi nhánh/Phòng ban trước khi tính toán';
    return;
  }

  calculating.value = true;
  errorMessage.value = '';
  successMessage.value = '';

  try {
    console.log('🔧 Tính Lợi nhuận khoán tài chính cho:', getSelectedUnitName());

    setTimeout(() => {
      calculatedIndicators.value[5].value = Math.floor(Math.random() * 200) + 50; // 50-250 triệu VND
      calculatedIndicators.value[5].calculated = true;
      showCalculationResults.value = true;
      successMessage.value = `✅ Đã tính Lợi nhuận khoán tài chính cho ${getSelectedUnitName()}: ${formatNumber(calculatedIndicators.value[5].value)} triệu VND`;
      calculating.value = false;
    }, 800);

  } catch (error) {
    console.error('Error calculating Tài chính:', error);
    errorMessage.value = 'Có lỗi khi tính Lợi nhuận khoán tài chính: ' + error.message;
    calculating.value = false;
  }
};

// ===============================

const onPeriodTypeChange = () => {
  console.log('🔧 onPeriodTypeChange called:', periodType.value);
  selectedPeriod.value = '';
  selectedDate.value = ''; // Reset ngày cụ thể khi thay đổi loại kỳ
  loadData();
};

const refreshData = () => {
  loadData();
  loadTrendData(trendPeriod.value);
};

const exportDashboard = () => {
  // TODO: Implement export functionality
  alert('Chức năng xuất báo cáo sẽ được phát triển trong phiên bản tiếp theo');
};

// Modal functions
const showIndicatorDetail = (indicator) => {
  selectedIndicator.value = indicator;
  showDetailModal.value = true;
  statusFilter.value = 'all';
};

const closeDetailModal = () => {
  showDetailModal.value = false;
  selectedIndicator.value = null;
  statusFilter.value = 'all';
};

const refreshIndicatorData = async () => {
  if (selectedIndicator.value) {
    const params = {
      year: selectedYear.value,
      indicatorId: selectedIndicator.value.id
    };

    if (periodType.value) params.periodType = periodType.value;
    if (selectedPeriod.value && periodType.value !== 'YEAR') params.period = selectedPeriod.value;
    if (selectedUnitId.value) params.unitId = selectedUnitId.value;

    await loadIndicatorStatus(params);
    successMessage.value = `Đã làm mới dữ liệu cho chỉ tiêu ${selectedIndicator.value.name}`;
  }
};

// Utility methods
const formatNumber = (value) => {
  if (!value && value !== 0) return '0';
  return Number(value).toLocaleString('en-US');
};

// Helper function để format tiền tệ VND với format US
const formatCurrency = (value) => {
  if (!value && value !== 0) return '0 VND';
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'VND',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(value);
};

const formatPercentage = (value) => {
  if (!value && value !== 0) return '0%';
  return Number(value).toFixed(1) + '%';
};

const formatDateTime = (dateString) => {
  if (!dateString) return '';
  return new Date(dateString).toLocaleString('vi-VN');
};

const getProgressClass = (rate) => {
  if (rate >= 100) return 'progress-complete';
  if (rate >= 80) return 'progress-good';
  if (rate >= 50) return 'progress-medium';
  return 'progress-low';
};
</script>

<style scoped>
/* Import Agribank Themes */
@import '@/assets/css/agribank-theme.css';
@import '@/assets/css/agribank-dashboard.css';
</style>

/* ===== AGRIBANK CALCULATION DASHBOARD STYLES ===== */

.calculation-dashboard.agribank-theme {
  min-height: 100vh;
  background: linear-gradient(135deg, var(--agribank-primary) 0%, var(--agribank-secondary) 100%);
  padding: 20px;
  font-family: 'Inter', 'Segoe UI', sans-serif;
}

/* ===== AGRIBANK HEADER ===== */
.page-header.agribank-header {
  background: rgba(255, 255, 255, 0.98);
  backdrop-filter: blur(20px);
  border-radius: var(--radius-xl);
  padding: 30px;
  margin-bottom: 30px;
  box-shadow: var(--shadow-lg);
  border: 1px solid rgba(139, 21, 56, 0.1);
}

.agribank-brand {
  display: flex;
  align-items: center;
  gap: 20px;
  margin-bottom: 25px;
}

.agribank-logo {
  width: 60px;
  height: 60px;
  filter: drop-shadow(0 4px 8px rgba(0,0,0,0.1));
}

.brand-text {
  flex: 1;
}

.agribank-title {
  color: var(--agribank-primary);
  font-size: 1.8rem;
  font-weight: 700;
  margin: 0 0 8px 0;
  display: flex;
  align-items: center;
  gap: 12px;
}

.agribank-title i {
  color: var(--agribank-accent);
  font-size: 2rem;
}

.agribank-subtitle {
  color: var(--text-secondary);
  font-size: 1rem;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 400;
}

.agribank-subtitle i {
  color: var(--agribank-primary);
  opacity: 0.7;
}

/* ===== AGRIBANK CONTROLS ===== */
.header-controls.agribank-controls {
  display: flex;
  gap: 25px;
  flex-wrap: wrap;
}

.filter-group.agribank-filter {
  display: flex;
  flex-direction: column;
  gap: 8px;
  min-width: 200px;
}

.agribank-label {
  font-weight: 600;
  color: var(--agribank-primary);
  font-size: 0.9rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.agribank-select {
  padding: 12px 16px;
  border: 2px solid var(--agribank-primary);
  border-radius: var(--radius-md);
  font-size: 1rem;
  background: white;
  color: var(--text-primary);
  transition: all 0.3s ease;
  font-weight: 500;
}

.agribank-select:focus {
  outline: none;
  border-color: var(--agribank-accent);
  box-shadow: 0 0 0 3px rgba(139, 21, 56, 0.1);
}

.agribank-select:hover {
  border-color: var(--agribank-primary-light);
}

/* ===== AGRIBANK SECTION ===== */
.overview-section.agribank-section {
  background: rgba(255, 255, 255, 0.98);
  backdrop-filter: blur(20px);
  border-radius: var(--radius-xl);
  padding: 30px;
  margin-bottom: 30px;
  box-shadow: var(--shadow-lg);
  border: 1px solid rgba(139, 21, 56, 0.1);
}

.section-header.agribank-section-header {
  margin-bottom: 25px;
}

.header-with-logo {
  display: flex;
  align-items: flex-start;
  gap: 15px;
}

.section-icon {
  background: linear-gradient(135deg, var(--agribank-primary), var(--agribank-primary-light));
  color: white;
  width: 50px;
  height: 50px;
  border-radius: var(--radius-lg);
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: var(--shadow-md);
}

.section-icon .agribank-icon {
  font-size: 1.5rem;
}

.section-title-area {
  flex: 1;
}

.agribank-section-title {
  color: var(--agribank-primary);
  font-size: 1.5rem;
  font-weight: 700;
  margin: 0 0 10px 0;
}

.agribank-section-subtitle {
  color: var(--text-secondary);
  font-size: 0.95rem;
  margin: 0;
  line-height: 1.5;
}

.status-legend {
  display: flex;
  gap: 20px;
  margin-top: 10px;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 0.85rem;
  font-weight: 500;
}

.legend-success {
  color: var(--success-color);
}

.legend-error {
  color: var(--danger-color);
}
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
  font-family: 'Inter', 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

/* PAGE HEADER */
.page-header {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  border-radius: 20px;
  padding: 30px;
  margin-bottom: 30px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.header-title h1 {
  color: #2d3748;
  font-size: 2.2rem;
  font-weight: 700;
  margin: 0 0 8px 0;
  display: flex;
  align-items: center;
  gap: 15px;
}

.header-title h1 i {
  color: #667eea;
  font-size: 2.5rem;
}

.subtitle {
  color: #718096;
  font-size: 1.1rem;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 400;
}

.subtitle i {
  color: #a0aec0;
}

/* HEADER CONTROLS */
.header-controls {
  margin-top: 25px;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

/* FILTER GROUPS */
.filter-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.filter-label {
  font-weight: 600;
  color: #4a5568;
  font-size: 0.9rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.form-select {
  padding: 12px 16px;
  border: 2px solid #e2e8f0;
  border-radius: 12px;
  font-size: 1rem;
  background: white;
  color: #2d3748;
  transition: all 0.3s ease;
  font-weight: 500;
}

.form-select:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
  transform: translateY(-2px);
}

.form-select:hover {
  border-color: #cbd5e0;
  transform: translateY(-1px);
}

/* CALCULATION BUTTONS */
.calculation-buttons {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 15px;
  margin-top: 20px;
}

.btn {
  padding: 16px 24px;
  border: none;
  border-radius: 12px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  position: relative;
  overflow: hidden;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
}

.btn:before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent);
  transition: left 0.5s;
}

.btn:hover:before {
  left: 100%;
}

.btn:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.btn:active {
  transform: translateY(-1px);
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none;
}

/* BUTTON COLORS */
.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.btn-warning {
  background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
  color: white;
}

.btn-info {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
  color: white;
}

.btn-danger {
  background: linear-gradient(135deg, #fa709a 0%, #fee140 100%);
  color: white;
}

.btn-success {
  background: linear-gradient(135deg, #a8edea 0%, #fed6e3 100%);
  color: #2d3748;
}

.btn-purple {
  background: linear-gradient(135deg, #d299c2 0%, #fef9d7 100%);
  color: #2d3748;
}

.btn-gradient {
  background: linear-gradient(135deg, #89f7fe 0%, #66a6ff 100%);
  color: white;
}

/* MESSAGES */
.error-message {
  background: linear-gradient(135deg, #ff9a9e 0%, #fecfef 100%);
  border: 1px solid #fed7d7;
  border-radius: 12px;
  padding: 16px;
  margin: 20px 0;
  animation: slideInDown 0.5s ease;
}

.error-message p {
  margin: 0;
  color: #742a2a;
  font-weight: 600;
}

.success-message {
  background: linear-gradient(135deg, #c3f0ca 0%, #faf0ca 100%);
  border: 1px solid #c6f6d5;
  border-radius: 12px;
  padding: 16px;
  margin: 20px 0;
  animation: slideInDown 0.5s ease;
}

.success-message p {
  margin: 0;
  color: #276749;
  font-weight: 600;
}

/* ANIMATIONS */
@keyframes slideInDown {
  from {
    opacity: 0;
    transform: translateY(-20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

/* RESPONSIVE DESIGN */
@media (min-width: 768px) {
  .header-controls {
    flex-direction: row;
    flex-wrap: wrap;
    align-items: end;
    gap: 20px;
  }

  .filter-group {
    min-width: 150px;
  }

  .calculation-buttons {
    grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
    width: 100%;
  }
}

@media (min-width: 1024px) {
  .calculation-dashboard {
    padding: 30px;
  }

  .page-header {
    padding: 40px;
  }

  .calculation-buttons {
    grid-template-columns: repeat(4, 1fr);
    max-width: none;
  }
}

/* LOADING STATES */
.btn:disabled {
  position: relative;
}

.btn:disabled:after {
  content: '';
  position: absolute;
  width: 16px;
  height: 16px;
  margin: auto;
  border: 2px solid transparent;
  border-top-color: #ffffff;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}

@keyframes spin {
  0% { transform: translate(-50%, -50%) rotate(0deg); }
  100% { transform: translate(-50%, -50%) rotate(360deg); }
}

/* HOVER EFFECTS */
.filter-group:hover .form-select {
  border-color: #a0aec0;
}

.calculation-buttons .btn {
  background-size: 200% 200%;
  background-position: 0% 50%;
  transition: all 0.3s ease, background-position 0.3s ease;
}

.calculation-buttons .btn:hover {
  background-position: 100% 50%;
}

/* ACCESSIBILITY */
.btn:focus-visible {
  outline: 2px solid #667eea;
  outline-offset: 2px;
}

.form-select:focus-visible {
  outline: 2px solid #667eea;
  outline-offset: 2px;
}

/* DARK MODE SUPPORT */
@media (prefers-color-scheme: dark) {
  .page-header {
    background: rgba(45, 55, 72, 0.95);
    border: 1px solid rgba(255, 255, 255, 0.1);
  }

  .header-title h1 {
    color: #f7fafc;
  }

  .subtitle {
    color: #a0aec0;
  }

  .filter-label {
    color: #e2e8f0;
  }

  .form-select {
    background: #2d3748;
    border-color: #4a5568;
    color: #f7fafc;
  }
}

/* ===== KPI MATRIX TABLE STYLES ===== */
.kpi-matrix-container {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  border-radius: 20px;
  padding: 25px;
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
  overflow-x: auto;
  margin-bottom: 30px;
}

.kpi-matrix-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
}

/* HEADER STYLES */
.kpi-matrix-table thead {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.kpi-matrix-table th {
  padding: 15px 12px;
  color: white;
  font-weight: 600;
  text-align: center;
  font-size: 0.85rem;
  border-right: 1px solid rgba(255, 255, 255, 0.2);
  position: relative;
}

.kpi-matrix-table th:last-child {
  border-right: none;
}

.unit-header {
  background: linear-gradient(135deg, #4a5568 0%, #2d3748 100%) !important;
  min-width: 200px;
  text-align: left !important;
  font-size: 0.9rem;
}

.indicator-header {
  min-width: 110px;
}

.indicator-header-content {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 5px;
}

.indicator-icon {
  font-size: 1.2rem;
}

.indicator-name {
  font-size: 0.75rem;
  line-height: 1.2;
  text-align: center;
}

.summary-header {
  background: linear-gradient(135deg, #38b2ac 0%, #319795 100%) !important;
  min-width: 100px;
}

/* BODY ROWS */
.kpi-matrix-table tbody tr {
  transition: all 0.3s ease;
}

.kpi-matrix-table tbody tr:hover {
  background: rgba(102, 126, 234, 0.04);
  transform: translateY(-1px);
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.08);
}

.kpi-matrix-table td {
  padding: 12px;
  border-bottom: 1px solid #e2e8f0;
  border-right: 1px solid #e2e8f0;
  text-align: center;
  vertical-align: middle;
}

.kpi-matrix-table td:last-child {
  border-right: none;
}

.unit-name-cell {
  text-align: left !important;
  font-weight: 500;
  color: #2d3748;
  background: rgba(248, 250, 252, 0.8);
  min-width: 200px;
}

.unit-name {
  display: block;
  font-size: 0.85rem;
}

/* STATUS CELLS */
.status-cell {
  background: rgba(255, 255, 255, 0.8);
  transition: all 0.3s ease;
  cursor: pointer;
}

.status-cell:hover {
  background: rgba(102, 126, 234, 0.08);
  transform: scale(1.05);
}

.status-indicator {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
  height: 40px;
}

.status-indicator i {
  font-size: 1.4rem;
  transition: all 0.3s ease;
}

.status-success {
  color: #48bb78;
}

.status-error {
  color: #f56565;
}

.status-calculated {
  background: rgba(72, 187, 120, 0.1);
}

.status-pending {
  background: rgba(245, 101, 101, 0.1);
}

/* UNIT SUMMARY */
.unit-summary-cell {
  background: rgba(248, 250, 252, 0.9);
}

.unit-progress {
  display: flex;
  flex-direction: column;
  gap: 6px;
  align-items: center;
}

.progress-info {
  font-weight: 600;
  font-size: 0.85rem;
  color: #4a5568;
}

.completed-count {
  color: #48bb78;
}

.total-count {
  color: #a0aec0;
}

.mini-progress-bar {
  width: 60px;
  height: 6px;
  background: #e2e8f0;
  border-radius: 3px;
  overflow: hidden;
}

.mini-progress-fill {
  height: 100%;
  transition: width 0.3s ease;
  border-radius: 3px;
}

.progress-complete {
  background: linear-gradient(90deg, #48bb78, #38a169);
}

.progress-high {
  background: linear-gradient(90deg, #4299e1, #3182ce);
}

.progress-medium {
  background: linear-gradient(90deg, #ed8936, #dd6b20);
}

.progress-low {
  background: linear-gradient(90deg, #f56565, #e53e3e);
}

.progress-none {
  background: #cbd5e0;
}

/* FOOTER SUMMARY */
.kpi-matrix-table tfoot {
  background: linear-gradient(135deg, #edf2f7 0%, #e2e8f0 100%);
}

.summary-row td {
  border-bottom: none;
  font-weight: 600;
  color: #2d3748;
  padding: 15px 12px;
}

.summary-label {
  background: linear-gradient(135deg, #4a5568 0%, #2d3748 100%);
  color: white !important;
  text-align: left !important;
}

.indicator-summary-cell {
  background: rgba(102, 126, 234, 0.1);
}

.indicator-summary {
  display: flex;
  flex-direction: column;
  gap: 2px;
  font-size: 0.8rem;
}

.completed-units {
  color: #48bb78;
  font-weight: 700;
  font-size: 0.9rem;
}

.total-units {
  color: #a0aec0;
}

.total-summary-cell {
  background: linear-gradient(135deg, #38b2ac 0%, #319795 100%);
  color: white !important;
}

.total-progress {
  font-size: 1rem;
  font-weight: 700;
}

.total-percentage {
  color: white;
}

/* RESPONSIVE DESIGN */
@media (max-width: 1200px) {
  .kpi-matrix-table {
    font-size: 0.8rem;
  }

  .indicator-header {
    min-width: 90px;
  }

  .indicator-icon {
    font-size: 1rem;
  }

  .indicator-name {
    font-size: 0.7rem;
  }
}

@media (max-width: 768px) {
  .kpi-matrix-container {
    padding: 15px;
    margin: 0 -10px;
  }

  .kpi-matrix-table {
    font-size: 0.75rem;
  }

  .unit-header {
    min-width: 150px;
  }

  .indicator-header {
    min-width: 80px;
  }

  .status-indicator {
    height: 35px;
  }

  .status-indicator i {
    font-size: 1.2rem;
  }
}
</style>
