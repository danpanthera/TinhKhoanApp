<template>
  <div class="employee-detail-modal" v-if="employee">
    <div class="modal-content">
      <button class="close-btn" @click="$emit('close')">×</button>
      <h3>Chi tiết nhân viên</h3>
      <div class="info-row"><strong>Mã NV:</strong> {{ employee.employeeCode }}</div>
      <div class="info-row"><strong>Mã CB:</strong> {{ employee.cbCode }}</div>
      <div class="info-row"><strong>Họ tên:</strong> {{ employee.fullName }}</div>
      <div class="info-row"><strong>Email:</strong> {{ employee.email }}</div>
      <div class="info-row"><strong>Số điện thoại:</strong> {{ employee.phoneNumber }}</div>
      <div class="info-row"><strong>Đơn vị:</strong> {{ employee.unit?.name }}</div>
      <div class="info-row"><strong>Chức vụ:</strong> {{ employee.position?.name }}</div>
      <div class="info-row">
        <strong>Trạng thái:</strong>
        <span :class="employee.isActive ? 'active' : 'inactive'">{{
          employee.isActive ? 'Đang làm việc' : 'Nghỉ việc'
        }}</span>
      </div>
    </div>
  </div>
</template>

<script setup>
const props = defineProps({
  employee: Object,
})
</script>

<style scoped>
.employee-detail-modal {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}
.modal-content {
  background: #fff;
  border-radius: 8px;
  padding: 32px 24px 24px 24px;
  min-width: 320px;
  max-width: 90vw;
  box-shadow: 0 2px 16px rgba(0, 0, 0, 0.15);
  position: relative;
}
.close-btn {
  position: absolute;
  top: 8px;
  right: 12px;
  background: none;
  border: none;
  font-size: 22px;
  cursor: pointer;
}
.info-row {
  margin-bottom: 10px;
}
.active {
  color: #2ecc40;
}
.inactive {
  color: #e74c3c;
}
</style>
