// Centralized Vietnamese messages (có thể mở rộng i18n sau)
export const messages = {
  emptyUnitAssignments: (unitName, periodName) => `Chưa có giao khoán KPI cho chi nhánh "${unitName}" trong kỳ "${periodName}".`,
  emptyEmployeeAssignments: 'Chưa có giao khoán KPI cho tiêu chí tìm kiếm hiện tại.',
  noAssignmentsHint: 'Bạn có thể tiến hành giao KPI hoặc kiểm tra lại bộ lọc.',
  updateSuccess: 'Cập nhật giá trị thực hiện thành công!',
  unitUpdateSuccess: 'Cập nhật giá trị thực hiện cho chi nhánh thành công!',
  loadingUnitEmptyInfo: 'Đang tải thông tin chi nhánh...',
  unitEmptyInfoError: 'Không thể tải thông tin chi nhánh/kỳ khoán.',
}

export default messages
