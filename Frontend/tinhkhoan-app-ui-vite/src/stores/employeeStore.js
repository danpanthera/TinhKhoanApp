import { defineStore } from "pinia";
import apiClient from "../services/api.js";
import { getId, normalizeArray, safeGet } from "../utils/casingSafeAccess.js";

export const useEmployeeStore = defineStore("employee", {
  state: () => ({
    employees: [],
    isLoading: false,
    error: null,
  }),
  getters: {
    allEmployees: (state) => state.employees,
    employeeCount: (state) => state.employees.length,
    // Safe getters using utility functions
    activeEmployees: (state) => state.employees.filter(emp => safeGet(emp, 'IsActive') === true),
    employeesByUnit: (state) => (unitId) => state.employees.filter(emp => getId(emp) === unitId),
  },
  actions: {
    async fetchEmployees() {
      console.log('🔍 Starting fetchEmployees...');
      this.isLoading = true;
      this.error = null;
      try {
        const response = await apiClient.get("/Employees");
        console.log('📡 API Response:', response.data);
        let employeesData = [];

        // Xử lý định dạng dữ liệu từ API
        if (response.data && Array.isArray(response.data.$values)) {
          employeesData = response.data.$values;
          console.log('📋 Using $values format, count:', employeesData.length);
        } else if (Array.isArray(response.data)) {
          employeesData = response.data;
          console.log('📋 Using direct array format, count:', employeesData.length);
        } else if (response.data && typeof response.data === 'object') {
          if (response.data.$id && getId(response.data)) {
            employeesData = [response.data];
            console.log('📋 Using single object with $id');
          } else if (Object.keys(response.data).length > 0) {
            employeesData = [response.data];
            console.log('📋 Using single object');
          }
        }

        console.log('📊 Final employeesData count:', employeesData.length);

        if (employeesData.length === 0) {
          this.error = "Dữ liệu nhân viên nhận được không đúng định dạng.";
          console.error('❌ No employee data to process!');
          return;
        }

        // Normalize array để đảm bảo PascalCase properties
        this.employees = normalizeArray(employeesData);
        console.log('✅ Employees normalized and stored:', this.employees.length);
      } catch (err) {
        this.employees = [];
        this.error = "Không thể tải danh sách nhân viên. Lỗi: " + (err.response?.data?.message || err.message);
        console.error("Lỗi khi fetchEmployees:", err);
      } finally {
        this.isLoading = false;
      }
    },

    async createEmployee(employeeData) {
      this.isLoading = true;
      this.error = null;
      try {
        // Gửi request tạo nhân viên mới
        const response = await apiClient.post("/Employees", employeeData);

        if (!response.data || !response.data.Id) {
          throw new Error("API trả về dữ liệu không hợp lệ");
        }

        // Tải lại danh sách và kiểm tra nhân viên mới
        await this.fetchEmployees();

        // Kiểm tra xem nhân viên mới có trong danh sách không
        const newEmployee = this.employees.find(e => e.Id === response.data.Id);
        if (!newEmployee) {
          console.error("Không tìm thấy nhân viên mới trong danh sách sau khi tải lại");
        }

        return response.data;
      } catch (err) {
        const errorMessage = err.response?.data?.message ||
                           err.response?.data?.title ||
                           (err.response?.data?.errors ? JSON.stringify(err.response.data.errors) : err.message);

        this.error = "Không thể tạo nhân viên. Lỗi: " + errorMessage;
        console.error("Lỗi khi createEmployee:", err);
        throw err;
      } finally {
        this.isLoading = false;
      }
    },

    async updateEmployee(employeeData) {
      this.isLoading = true;
      this.error = null;
      try {
        // Make sure we include passwordHash if it exists on the current employee
        const currentEmployee = await this.fetchEmployeeDetail(employeeData.Id).catch(() => null);

        // If we can't get the current employee, proceed with the update as is
        // If we have the current employee and it has a passwordHash, include it
        const dataToSend = {
          ...employeeData,
          passwordHash: currentEmployee?.passwordHash || employeeData.passwordHash
        };

        // Get employee ID - try both cases for compatibility
        const employeeId = employeeData.Id || employeeData.id;
        
        if (!employeeId) {
          throw new Error("Employee ID is required for update");
        }

        // Gửi request cập nhật nhân viên
        console.log("🔍 Updating employee with ID:", employeeId);
        const response = await apiClient.put(`/Employees/${employeeId}`, dataToSend);
        await this.fetchEmployees();
      } catch (err) {
        const errorMessage = err.response?.data?.message ||
                           err.response?.data?.title ||
                           (err.response?.data?.errors ? JSON.stringify(err.response.data.errors) : err.message);

        this.error = "Không thể cập nhật nhân viên. Lỗi: " + errorMessage;
        console.error("Lỗi khi updateEmployee:", err);
        throw err;
      } finally {
        this.isLoading = false;
      }
    },

    async deleteEmployee(employeeId) {
      this.isLoading = true;
      this.error = null;
      try {
        await apiClient.delete(`/Employees/${employeeId}`);
        this.employees = this.employees.filter(e => e.Id !== employeeId);
      } catch (err) {
        const errorMessage = err.response?.data?.message ||
                           err.response?.data?.title ||
                           err.message;

        this.error = "Không thể xóa nhân viên. Lỗi: " + errorMessage;
        console.error("Lỗi khi deleteEmployee:", err);
        throw err;
      } finally {
        this.isLoading = false;
      }
    },

    // Xóa nhiều nhân viên cùng lúc
    async deleteMultipleEmployees(employeeIds) {
      this.isLoading = true;
      this.error = null;
      try {
        const response = await apiClient.delete("/Employees/bulk", {
          data: employeeIds // Gửi array IDs trong body
        });

        // Cập nhật state local bằng cách loại bỏ các nhân viên đã xóa
        this.employees = this.employees.filter(e => !employeeIds.includes(e.Id));

        return response.data;
      } catch (err) {
        const errorMessage = err.response?.data?.message ||
                           err.response?.data?.title ||
                           err.message;

        this.error = "Không thể xóa nhân viên. Lỗi: " + errorMessage;
        console.error("Lỗi khi deleteMultipleEmployees:", err);
        throw err;
      } finally {
        this.isLoading = false;
      }
    },

    async fetchEmployeeDetail(id) {
      this.isLoading = true;
      this.error = null;
      try {
        const response = await apiClient.get(`/Employees/${id}`);
        if (response.data) {
          return response.data;
        } else {
          throw new Error("Không lấy được chi tiết nhân viên");
        }
      } catch (err) {
        this.error =
          "Không thể lấy chi tiết nhân viên. Lỗi: " +
          (err.response?.data?.message ||
            err.response?.data?.title ||
            (err.response?.data?.errors
              ? JSON.stringify(err.response.data.errors)
              : err.message));
        console.error("Lỗi khi fetchEmployeeDetail:", err);
        throw err;
      } finally {
        this.isLoading = false;
      }
    },
  },
});
