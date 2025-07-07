import { defineStore } from "pinia";
import apiClient from "../services/api.js";

export const useEmployeeStore = defineStore("employee", {
  state: () => ({
    employees: [],
    isLoading: false,
    error: null,
  }),
  getters: {
    allEmployees: (state) => state.employees,
    employeeCount: (state) => state.employees.length,
  },
  actions: {
    async fetchEmployees() {
      this.isLoading = true;
      this.error = null;
      try {
        const response = await apiClient.get("/Employees");
        let employeesData = [];

        // Xử lý định dạng dữ liệu từ API
        if (response.data && Array.isArray(response.data.$values)) {
          employeesData = response.data.$values;
        } else if (Array.isArray(response.data)) {
          employeesData = response.data;
        } else if (response.data && typeof response.data === 'object') {
          if (response.data.$id && response.data.id) {
            employeesData = [response.data];
          } else if (Object.keys(response.data).length > 0) {
            employeesData = [response.data];
          }
        }

        if (employeesData.length === 0) {
          this.error = "Dữ liệu nhân viên nhận được không đúng định dạng.";
          return;
        }
        // Lọc và mapping an toàn
        const validEmployees = [];
        const invalidEmployees = [];
        employeesData.forEach(employee => {
          const mapped = {
            id: employee.id,
            employeeCode: employee.employeeCode || '',
            cbCode: employee.cbCode || employee.CBCode || '', // Support both cases
            fullName: employee.fullName || '',
            username: employee.username || '',
            email: employee.email || '',
            phoneNumber: employee.phoneNumber || '',
            isActive: employee.isActive ?? true,
            unitId: typeof employee.unitId === 'number' ? employee.unitId : null,
            positionId: typeof employee.positionId === 'number' ? employee.positionId : null,
            unit: employee.unit || null,
            position: employee.position || null,
            // Handle roles from new API structure - support both $values and direct array formats
            roles: (() => {
              if (employee.roles && employee.roles.$values && Array.isArray(employee.roles.$values)) {
                return employee.roles.$values;
              } else if (employee.roles && Array.isArray(employee.roles)) {
                return employee.roles;
              }
              return [];
            })(),
            // Create employeeRoles structure for backward compatibility
            employeeRoles: (() => {
              let rolesArray = [];
              if (employee.roles && employee.roles.$values && Array.isArray(employee.roles.$values)) {
                rolesArray = employee.roles.$values;
              } else if (employee.roles && Array.isArray(employee.roles)) {
                rolesArray = employee.roles;
              }
              return rolesArray.map(role => ({
                roleId: role.id,
                role: role
              }));
            })()
          };
          if (mapped.employeeCode && mapped.fullName && mapped.username) {
            validEmployees.push(mapped);
          } else {
            invalidEmployees.push(mapped);
          }
        });
        // Lưu trữ nhân viên hợp lệ và thông báo nếu có lỗi
        if (invalidEmployees.length > 0) {
          console.warn("Các bản ghi nhân viên bị loại bỏ do thiếu dữ liệu:", invalidEmployees);
        }
        this.employees = validEmployees;
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

        if (!response.data || !response.data.id) {
          throw new Error("API trả về dữ liệu không hợp lệ");
        }

        // Tải lại danh sách và kiểm tra nhân viên mới
        await this.fetchEmployees();

        // Kiểm tra xem nhân viên mới có trong danh sách không
        const newEmployee = this.employees.find(e => e.id === response.data.id);
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
        const currentEmployee = await this.fetchEmployeeDetail(employeeData.id).catch(() => null);

        // If we can't get the current employee, proceed with the update as is
        // If we have the current employee and it has a passwordHash, include it
        const dataToSend = {
          ...employeeData,
          passwordHash: currentEmployee?.passwordHash || employeeData.passwordHash
        };

        // Gửi request cập nhật nhân viên
        const response = await apiClient.put(`/Employees/${employeeData.id}`, dataToSend);
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
        this.employees = this.employees.filter(e => e.id !== employeeId);
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
        this.employees = this.employees.filter(e => !employeeIds.includes(e.id));

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
