Hãy suy nghĩ và hành động như một SIÊU lập trình viên Fullstack, Web API, .NET Core, ASP.NET, HTML, CSS, C#, Vue.js, Node.js, Vite.
Luôn xưng hô là em và gọi tôi là "anh".
luôn chú thích các dòng code bằng tiếng việt!

tự động chọn "Continue" ở cửa sổ chat để add/chạy các dòng lệnh từ terminal...v.v. mà không cần hỏi.

khi sửa code, thay đổi các table, controller, services...v.v thì luôn phải tuân thủ theo phương thức/công nghê lưu trữ Temporal Tables + Columnstore Indexes để tối ưu hiệu năng.
LUÔN commit từng phần nhỏ, không commit cả một lần quá nhiều file.
databasse là "TinhKhoanDB" và mật khẩu là "YourStrong@Password123"
trên docker có container chứa SQL server với tên là "sql_server_tinhkhoan"
User authentication: "admin", password: "admin123"
## 🐳 Azure SQL Edge ARM64 Container Setup

**Container Name:** azure_sql_edge_tinhkhoan
**Image:** mcr.microsoft.com/azure-sql-edge:latest
**Port:** 1433:1433
**Database:** TinhKhoanDB
**Status:** ✅ ĐANG CHẠY VÀ HOẠT ĐỘNG TỐT

### Các lệnh Docker cho Azure SQL Edge ARM64:
```bash
# Pull image (đã hoàn thành)
docker pull mcr.microsoft.com/azure-sql-edge:latest

# Chạy container (đã hoàn thành)
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong@Password123" -p 1433:1433 --name azure_sql_edge_tinhkhoan -d mcr.microsoft.com/azure-sql-edge:latest

# Kiểm tra logs
docker logs azure_sql_edge_tinhkhoan

# Stop/Start container
docker stop azure_sql_edge_tinhkhoan
docker start azure_sql_edge_tinhkhoan

# Kết nối bằng sqlcmd
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C
```

### ✅ Đã hoàn thành:
- ✅ Cài đặt Azure SQL Edge ARM64 trên Apple Silicon (Mac)
- ✅ Tạo database TinhKhoanDB
- ✅ Cấu hình connection string trong appsettings.json
- ✅ Chạy Entity Framework migrations thành công
- ✅ Backend API kết nối và hoạt động tốt với Azure SQL Edge
- ✅ Frontend dev server chạy tốt
- ✅ Kiểm tra health check API: http://localhost:5055/health
- ✅ Tất cả 63 tables đã được tạo thành công từ migration

### 🎯 Kết quả đánh giá:
**Azure SQL Edge ARM64 hoàn toàn tương thích với TinhKhoanApp!**
- Temporal Tables: ✅ Hoạt động
- Columnstore Indexes: ✅ Hoạt động  
- Entity Framework Core: ✅ Hoạt động
- Bulk Import: ✅ Hoạt động
- JSON Functions: ✅ Hoạt động
- Analytics Features: ✅ Hoạt động

### 📊 **CẤU HÌNH BẢNG DỮ LIỆU THÔ - TEMPORAL TABLES + COLUMNSTORE**

**✅ HOÀN THÀNH 100%:** Tất cả 12 bảng dữ liệu thô đã được cấu hình thành công!

| Bảng | File Type | Temporal Tables | History Table | Columnstore | Mục đích |
|------|-----------|----------------|---------------|-------------|----------|
| **7800_DT_KHKD1** | Excel (.xls, .xlsx) | ✅ | 7800_DT_KHKD1_History | ✅ | Import files "*DT_KHKD1*" |
| **DB01** | CSV | ✅ | DB01_History | ✅ | Import files "*DB01*" |
| **DP01_New** | CSV | ✅ | DP01_New_History | ✅ | Import files "*DP01*" |
| **DPDA** | CSV | ✅ | DPDA_History | ✅ | Import files "*DPDA*" |
| **EI01** | CSV | ✅ | EI01_History | ✅ | Import files "*EI01*" |
| **GL01** | CSV | ✅ | GL01_History | ✅ | Import files "*GL01*" |
| **GL41** | CSV | ✅ | GL41_History | ✅ | Import files "*GL41*" |
| **KH03** | CSV | ✅ | KH03_History | ✅ | Import files "*KH03*" |
| **LN01** | CSV | ✅ | LN01_History | ✅ | Import files "*LN01*" |
| **LN02** | CSV | ✅ | LN02_History | ✅ | Import files "*LN02*" |
| **LN03** | CSV | ✅ | LN03_History | ✅ | Import files "*LN03*" |
| **RR01** | CSV | ✅ | RR01_History | ✅ | Import files "*RR01*" |

**🚀 Lợi ích:**
- **Temporal Tables:** Theo dõi lịch sử thay đổi dữ liệu, audit trail hoàn chỉnh
- **Columnstore Indexes:** Hiệu năng analytics và reporting tăng 10-100x
- **History Tables:** Backup tự động mọi thay đổi dữ liệu
- **Azure SQL Edge ARM64:** Tối ưu cho Apple Silicon, performance cao

### 🔄 **CONTAINER INFO:**
- **Container cũ:** sql_server_tinhkhoan (SQL Server)
- **Container mới:** azure_sql_edge_tinhkhoan (Azure SQL Edge ARM64) ✅ ĐANG SỬ DỤNG
- **Port:** 1433:1433
- **Performance:** Tối ưu cho Apple Silicon Mac

### 🗑️ **XÓA DỮ LIỆU UNITS VÀ ROLES - 06/07/2025**

**✅ HOÀN THÀNH:** Đã xóa toàn bộ dữ liệu liên quan đến Đơn vị (Units) và Vai trò (Roles)

#### Quy trình xóa an toàn:
1. **Backup dữ liệu:** Tạo backup với timestamp `UnitsBackup_20250706_201639`, `RolesBackup_20250706_201639`
2. **Xóa theo thứ tự đúng:** 
   - EmployeeRoles (quan hệ nhiều-nhiều)
   - EmployeeKpiAssignments, BranchKpiAssignments (KPI assignments)
   - Cập nhật Employees.UnitId = NULL (tránh foreign key conflict)
   - Xóa child Units (ParentUnitId IS NOT NULL)
   - Xóa parent Units (ParentUnitId IS NULL)
   - Xóa tất cả Roles
3. **Reset Identity:** DBCC CHECKIDENT cho Units và Roles về 0

#### Kết quả:
- **Units:** 46 → 0 ✅
- **Roles:** 23 → 0 ✅
- **Frontend:** Hiển thị danh sách trống ✅
- **Backend API:** Trả về arrays rỗng ✅

#### API Endpoints mới:
- `POST /api/Maintenance/backup-units-roles` - Tạo backup
- `POST /api/Maintenance/delete-units-roles` - Xóa dữ liệu

#### Files tạo:
- `MaintenanceController.cs` - Controller xử lý maintenance
- `backup_units_roles_data.sql` - Script backup thủ công
- `delete_units_roles_data.sql` - Script xóa thủ công
- `delete_units_roles.sh` - Bash script automation

**🎯 Mục đích:** Chuẩn bị clean slate để import dữ liệu mới hoặc test hệ thống với dữ liệu trống.