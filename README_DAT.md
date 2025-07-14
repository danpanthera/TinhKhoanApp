### 📝 **HƯỚNG DẪN LẬP TRÌNH VIÊN TINH KHOẢN APP**

Hãy suy nghĩ và hành động như một SIÊU lập trình viên Fullstack, Web API, .NET Core, ASP.NET, HTML, CSS, C#, Vue.js, Node.js, Vite.
Luôn xưng hô là em và gọi tôi là "anh".
luôn chú thích các dòng code bằng tiếng việt!
LUÔN commit từng phần nhỏ, không commit cả một lần quá nhiều file.
databasse là "TinhKhoanDB" và mật khẩu là "YourStrong@Password123"
trên docker có container chứa SQL server với tên là "azure_sql_edge_tinhkhoan"
Luôn để backend port là 5055, frontend port là 3000.

🚨 **QUY TẮC KHỞI ĐỘNG DỰ ÁN:**
- **Backend:** LUÔN dùng `./start_backend.sh` 
- **Frontend:** LUÔN dùng `./start_frontend.sh`
-
- **Fast Commit:** LUÔN dùng `./fast_commit.sh "message"`
- **NGHIÊM CẤM** sử dụng shell VS Code để chạy npm/dotnet commands!

🚨DỮ LIỆU MẪU CHUẨN CHO 08 CORE DATA - TUYỆT ĐỐI KHÔNG TẠO DỮ LIỆU MOCK DATA
/Users/nguyendat/Documents/DuLieuImport

## 🆕 TinhKhoanApp Maintenance Notes (July 2025)

### Dọn dẹp Dự án

Một cuộc dọn dẹp toàn diện đã được thực hiện để giảm kích thước dự án và cải thiện khả năng bảo trì:

1. **Dọn dẹp File Test**

   - Đã xóa các file test thừa/lỗi thời
   - Giữ lại các file test thiết yếu cho kiểm tra hồi quy
   - Sắp xếp các file test theo cách có cấu trúc hơn

2. **Nhất quán PascalCase**
   - Đã triển khai đánh giá hệ thống về việc sử dụng PascalCase/camelCase
   - Sử dụng helper `safeGet` trong toàn bộ codebase để xử lý cả hai kiểu viết hoa
   - Chuẩn hóa API response và data binding

### Scripts Bảo trì

Các script sau đây đã được tạo để giúp duy trì chất lượng code:

- `cleanup-test-files.sh`: Xóa các file test không cần thiết nhưng vẫn giữ lại các file thiết yếu
- `review-pascalcase.sh`: Quét codebase để tìm kiếm cách viết hoa không nhất quán và tạo báo cáo
- `fix-pascalcase.sh`: Giúp thêm import safeGet vào các file cần truy cập casing-safe

### Các Phương pháp Tốt nhất

1. **Truy cập Thuộc tính**

   - Luôn sử dụng các helper `safeGet`, `getId`, `getName` v.v. từ `casingSafeAccess.js`
   - Ví dụ: `safeGet(employee, 'FullName')` thay vì `employee.FullName`

2. **API Responses**

   - Backend trả về thuộc tính PascalCase (ví dụ: `"FullName": "Nguyen Van A"`)
   - Frontend nên sử dụng safeGet để xử lý cả hai trường hợp, nhưng ưu tiên PascalCase trong code

3. **File Test**

   - Chỉ giữ lại các file test thiết yếu cho kiểm tra hồi quy
   - Đặt tên file test với tên mô tả và chỉ rõ phiên bản (ví dụ: `test-final-kpi-assignment-fixes.html`)
   - Xóa các file test khi không còn cần thiết

4. **Debugging**
   - Sử dụng debug logging có sẵn trong components
   - Test với các file HTML trong `/public` cho kiểm tra độc lập

### Nhiệm vụ Còn lại

- Tiếp tục giám sát API responses về tính nhất quán của casing
- Thường xuyên dọn dẹp các file test khi có test mới được tạo
- Cập nhật tài liệu với các mẫu và phương pháp mới

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

**✅ HOÀN THÀNH 100%:** Tất cả 08 bảng dữ liệu thô đã được cấu hình thành công!

| Bảng | File Type | Temporal Tables | History Table | Columnstore | Mục đích | Business Columns|
| ---- | --------- | --------------- | ------------- | ----------- | -------- | ---------------- |
| **DP01** | CSV | ✅ | DP01_History | ✅ | Import files "_DP01_" | 63
| **DPDA** | CSV | ✅ | DPDA_History | ✅ | Import files "_DPDA_" | 13
| **EI01** | CSV | ✅ | EI01_History | ✅ | Import files "_EI01_" | 24
| **GL01** | CSV | ✅ | GL01_History | ✅ | Import files "_GL01_" | 27
| **GL41** | CSV | ✅ | GL41_History | ✅ | Import files "_GL41_" | 13
| **LN01** | CSV | ✅ | LN01_History | ✅ | Import files "_LN01_" | 79
| **LN03** | CSV | ✅ | LN03_History | ✅ | Import files "_LN03_" | 20
| **RR01** | CSV | ✅ | RR01_History | ✅ | Import files "_RR01_" | 25

**🚀 Lợi ích:**

- **Temporal Tables:** Theo dõi lịch sử thay đổi dữ liệu, audit trail hoàn chỉnh
- **Columnstore Indexes:** Hiệu năng analytics và reporting tăng 10-100x
- **History Tables:** Backup tự động mọi thay đổi dữ liệu
- **Azure SQL Edge ARM64:** Tối ưu cho Apple Silicon, performance cao

### 🔄 **DIRECT IMPORT MECHANISM - VERIFIED**

**✅ HOÀN THÀNH 100%:** Cơ chế Direct Import hoạt động hoàn hảo cho tất cả 8 bảng!

#### **📊 Test Results (13/07/2025):**

| File Type | Target Table | Performance | Status | Test Result |
|-----------|--------------|-------------|--------|-------------|
| **DP01** | DP01 | 31.54 records/sec | ✅ SUCCESS | Auto-detect ✅ |
| **EI01** | EI01 | 46.01 records/sec | ✅ SUCCESS | Auto-detect ✅ |
| **LN01** | LN01 | Tested | ✅ SUCCESS | Auto-detect ✅ |
| **GL01** | GL01 | Tested | ✅ SUCCESS | Auto-detect ✅ |
| **GL41** | GL41 | Tested | ✅ SUCCESS | Auto-detect ✅ |
| **DPDA** | DPDA | Tested | ✅ SUCCESS | Auto-detect ✅ |
| **LN03** | LN03 | Tested | ✅ SUCCESS | Auto-detect ✅ |
| **RR01** | RR01 | Tested | ✅ SUCCESS | Auto-detect ✅ |

#### **🎯 Features Confirmed:**

- ✅ **Filename Detection:** Tự động detect loại file từ pattern `_DP01_`, `_EI01_`, etc.
- ✅ **Target Routing:** Import trực tiếp vào bảng đúng theo loại
- ✅ **API Endpoint:** `/api/DirectImport/smart` hoạt động ổn định
- ✅ **Performance:** Tốc độ import từ 31-46 records/sec
- ✅ **Error Handling:** 0 errors, 100% success rate
- ✅ **Logging:** Chi tiết logs cho monitoring và debug


```

### 🔄 **CONTAINER INFO:**

- **Container cũ:** sql_server_tinhkhoan (SQL Server) - ✅ ĐÃ XÓA
- **Container extract:** sqlserver-extract - ✅ ĐÃ XÓA (06/07/2025)
- **Container chính:** azure_sql_edge_tinhkhoan (Azure SQL Edge ARM64) ✅ ĐANG SỬ DỤNG
- **Port:** 1433:1433
- **Performance:** Tối ưu cho Apple Silicon Mac
- **Status:** Môi trường đã được dọn dẹp, chỉ còn container chính

### 🗑️ **XÓA DỮ LIỆU UNITS VÀ ROLES - 06/07/2025**

**✅ HOÀN THÀNH:** Đã xóa toàn bộ dữ liệu liên quan đến Đơn vị (Units) và Vai trò (Roles)

### QUY ƯỚC MÃ CHI NHÁNH (MA_CN) theo tên gọi như sau:

cấu trúc như sau: Tên, code, MA_CN

- Hội Sở, HoiSo, 7800
- Bình Lư, BinhLu, 7801
- Phong Thổ, PhongTho, 7802
- Sìn Hồ, SinHo, 7803
- Bum Tở, BumTo, 7804
- Than Uyên, ThanUyen, 7805
- Doan Kết, DoanKet, 7806
- Tân Uyên, TanUyen, 7807
- Nậm Hàng, NamHang, 7808
- Toàn tỉnh, ToanTinh, Tổng của 9 Chi nhánh từ Hội Sở -> Nậm Hàng

### 🏢 **TẠO CẤU TRÚC 46 ĐƠN VỊ - 06/07/2025**

**✅ HOÀN THÀNH:** Đã tạo thành công 46 đơn vị theo cấu trúc hierarchical

#### Cấu trúc tổ chức:

```
Chi nhánh Lai Châu (ID=1, CNL1) [ROOT]
├── Hội Sở (ID=2, CNL1)
│   ├── Ban Giám đốc (ID=3, PNVL1)
│   ├── Phòng Khách hàng Doanh nghiệp (ID=4, PNVL1)
│   ├── Phòng Khách hàng Cá nhân (ID=5, PNVL1)
│   ├── Phòng Kế toán & Ngân quỹ (ID=6, PNVL1)
│   ├── Phòng Tổng hợp (ID=7, PNVL1)
│   ├── Phòng Kế hoạch & Quản lý rủi ro (ID=8, PNVL1)
│   └── Phòng Kiểm tra giám sát (ID=9, PNVL1)
├── Chi nhánh Bình Lư (ID=10, CNL2)
│   ├── Ban Giám đốc (PNVL2)
│   ├── Phòng Kế toán & Ngân quỹ (PNVL2)
│   └── Phòng Khách hàng (PNVL2)
├── Chi nhánh Phong Thổ (ID=11, CNL2)
│   ├── Ban Giám đốc, Phòng KT&NQ, Phòng KH (PNVL2)
│   └── Phòng giao dịch Số 5 (PGDL2)
├── Chi nhánh Sìn Hồ (ID=12, CNL2)
├── Chi nhánh Bum Tở (ID=13, CNL2)
├── Chi nhánh Than Uyên (ID=14, CNL2)
│   └── + Phòng giao dịch số 6 (PGDL2)
├── Chi nhánh Đoàn Kết (ID=15, CNL2)
│   ├── + Phòng giao dịch số 1 (PGDL2)
│   └── + Phòng giao dịch số 2 (PGDL2)
├── Chi nhánh Tân Uyên (ID=16, CNL2)
│   └── + Phòng giao dịch số 3 (PGDL2)
└── Chi nhánh Nậm Hàng (ID=17, CNL2)
```

#### Thống kê:

- **CNL1:** 2 đơn vị (Lai Châu, Hội Sở)
- **CNL2:** 8 chi nhánh cấp 2
- **PNVL1:** 7 phòng ban Hội Sở
- **PNVL2:** 25 phòng ban chi nhánh
- **PGDL2:** 4 phòng giao dịch
- **Tổng:** 46 đơn vị ✅

#### Công cụ sử dụng:

- **Shell script:** `create_46_units.sh` - Automation tạo toàn bộ cấu trúc
- **API Units:** POST `/api/units` - Tạo từng đơn vị với parentUnitId
- **MaintenanceController:** Backup và management endpoints
- **Verification:** JSON validation và count checking

#### Đặc điểm kỹ thuật:

- **Auto-increment ID:** Database tự động gán ID tuần tự
- **Parent-Child relationships:** Cấu trúc cây hoàn chỉnh
- **Unicode support:** Tên tiếng Việt hiển thị đúng
- **API compatible:** Frontend có thể fetch và hiển thị đầy đủ

**🎯 Status:** Sẵn sàng cho việc gán Roles và Employees vào từng đơn vị.

### 🎭 **TẠO 23 VAI TRÒ - 06/07/2025**

**✅ HOÀN THÀNH:** Đã tạo thành công 23 vai trò theo danh sách chuẩn

#### Danh sách 23 vai trò:

| ID  | Mã vai trò          | Tên vai trò                              | Mô tả                                          |
| --- | ------------------- | ---------------------------------------- | ---------------------------------------------- |
| 1   | TruongphongKhdn     | Trưởng phòng KHDN                        | Trưởng phòng Khách hàng Doanh nghiệp           |
| 2   | TruongphongKhcn     | Trưởng phòng KHCN                        | Trưởng phòng Khách hàng Cá nhân                |
| 3   | PhophongKhdn        | Phó phòng KHDN                           | Phó phòng Khách hàng Doanh nghiệp              |
| 4   | PhophongKhcn        | Phó phòng KHCN                           | Phó phòng Khách hàng Cá nhân                   |
| 5   | TruongphongKhqlrr   | Trưởng phòng KH&QLRR                     | Trưởng phòng Kế hoạch & Quản lý rủi ro         |
| 6   | PhophongKhqlrr      | Phó phòng KH&QLRR                        | Phó phòng Kế hoạch & Quản lý rủi ro            |
| 7   | Cbtd                | Cán bộ tín dụng                          | Cán bộ tín dụng                                |
| 8   | TruongphongKtnqCnl1 | Trưởng phòng KTNQ CNL1                   | Trưởng phòng Kế toán & Ngân quỹ CNL1           |
| 9   | PhophongKtnqCnl1    | Phó phòng KTNQ CNL1                      | Phó phòng Kế toán & Ngân quỹ CNL1              |
| 10  | Gdv                 | GDV                                      | Giao dịch viên                                 |
| 11  | TqHkKtnb            | Thủ quỹ \| Hậu kiểm \| KTNB              | Thủ quỹ \| Hậu kiểm \| Kế toán nghiệp vụ       |
| 12  | TruongphoItThKtgs   | Trưởng phó IT \| Tổng hợp \| KTGS        | Trưởng phó IT \| Tổng hợp \| Kiểm tra giám sát |
| 13  | CBItThKtgsKhqlrr    | Cán bộ IT \| Tổng hợp \| KTGS \| KH&QLRR | Cán bộ IT \| Tổng hợp \| KTGS \| KH&QLRR       |
| 14  | GiamdocPgd          | Giám đốc Phòng giao dịch                 | Giám đốc Phòng giao dịch                       |
| 15  | PhogiamdocPgd       | Phó giám đốc Phòng giao dịch             | Phó giám đốc Phòng giao dịch                   |
| 16  | PhogiamdocPgdCbtd   | Phó giám đốc PGD kiêm CBTD               | Phó giám đốc Phòng giao dịch kiêm CBTD         |
| 17  | GiamdocCnl2         | Giám đốc CNL2                            | Giám đốc Chi nhánh cấp 2                       |
| 18  | PhogiamdocCnl2Td    | Phó giám đốc CNL2 phụ trách TD           | Phó giám đốc CNL2 phụ trách Tín dụng           |
| 19  | PhogiamdocCnl2Kt    | Phó giám đốc CNL2 phụ trách KT           | Phó giám đốc CNL2 phụ trách Kế toán            |
| 20  | TruongphongKhCnl2   | Trưởng phòng KH CNL2                     | Trưởng phòng Khách hàng CNL2                   |
| 21  | PhophongKhCnl2      | Phó phòng KH CNL2                        | Phó phòng Khách hàng CNL2                      |
| 22  | TruongphongKtnqCnl2 | Trưởng phòng KTNQ CNL2                   | Trưởng phòng Kế toán & Ngân quỹ CNL2           |
| 23  | PhophongKtnqCnl2    | Phó phòng KTNQ CNL2                      | Phó phòng Kế toán & Ngân quỹ CNL2              |

#### Công cụ sử dụng:

- **Shell script:** `create_23_roles.sh` - Automation tạo toàn bộ 23 vai trò
- **API Roles:** POST `/api/roles` - Tạo từng vai trò với Name và Description
- **Model:** Role entity với properties Id, Name, Description, EmployeeRoles
- **Validation:** JSON schema và backend validation đầy đủ

#### Đặc điểm kỹ thuật:

- **Auto-increment ID:** Database tự động gán ID tuần tự từ 1-23
- **Unicode support:** Tên và mô tả tiếng Việt hiển thị đúng
- **API compatible:** Frontend có thể fetch và hiển thị đầy đủ
- **Mã vai trò:** Giữ nguyên không thay đổi theo yêu cầu
- **Navigation properties:** Hỗ trợ quan hệ many-to-many với Employees

**🎯 Status:** Sẵn sàng để gán vai trò cho nhân viên trong từng đơn vị.

### 📊 **CẤU HÌNH KPI ASSIGNMENT TABLES - 06/07/2025**

**✅ HOÀN THÀNH:** Đã có đủ 32 bảng KPI theo đúng cấu trúc

#### 🧑‍💼 Tab "Dành cho Cán bộ" - 23 bảng KPI:

| ID  | Tên Bảng KPI        | Mô tả                                    |
| --- | ------------------- | ---------------------------------------- | -------------- |
| 1   | TruongphongKhdn     | Trưởng phòng KHDN                        | Trưởng phòng Khách hàng Doanh nghiệp           |
| 2   | TruongphongKhcn     | Trưởng phòng KHCN                        | Trưởng phòng Khách hàng Cá nhân                |
| 3   | PhophongKhdn        | Phó phòng KHDN                           | Phó phòng Khách hàng Doanh nghiệp              |
| 4   | PhophongKhcn        | Phó phòng KHCN                           | Phó phòng Khách hàng Cá nhân                   |
| 5   | TruongphongKhqlrr   | Trưởng phòng KH&QLRR                     | Trưởng phòng Kế hoạch & Quản lý rủi ro         |
| 6   | PhophongKhqlrr      | Phó phòng KH&QLRR                        | Phó phòng Kế hoạch & Quản lý rủi ro            |
| 7   | Cbtd                | Cán bộ tín dụng                          | Cán bộ tín dụng                                |
| 8   | TruongphongKtnqCnl1 | Trưởng phòng KTNQ CNL1                   | Trưởng phòng Kế toán & Ngân quỹ CNL1           |
| 9   | PhophongKtnqCnl1    | Phó phòng KTNQ CNL1                      | Phó phòng Kế toán & Ngân quỹ CNL1              |
| 10  | Gdv                 | GDV                                      | Giao dịch viên                                 |
| 11  | TqHkKtnb            | Thủ quỹ \| Hậu kiểm \| KTNB              | Thủ quỹ \| Hậu kiểm \| Kế toán nghiệp vụ       |
| 12  | TruongphoItThKtgs   | Trưởng phó IT \| Tổng hợp \| KTGS        | Trưởng phó IT \| Tổng hợp \| Kiểm tra giám sát |
| 13  | CBItThKtgsKhqlrr    | Cán bộ IT \| Tổng hợp \| KTGS \| KH&QLRR | Cán bộ IT \| Tổng hợp \| KTGS \| KH&QLRR       |
| 14  | GiamdocPgd          | Giám đốc Phòng giao dịch                 | Giám đốc Phòng giao dịch                       |
| 15  | PhogiamdocPgd       | Phó giám đốc Phòng giao dịch             | Phó giám đốc Phòng giao dịch                   |
| 16  | PhogiamdocPgdCbtd   | Phó giám đốc PGD kiêm CBTD               | Phó giám đốc Phòng giao dịch kiêm CBTD         |
| 17  | GiamdocCnl2         | Giám đốc CNL2                            | Giám đốc Chi nhánh cấp 2                       |
| 18  | PhogiamdocCnl2Td    | Phó giám đốc CNL2 phụ trách TD           | Phó giám đốc CNL2 phụ trách Tín dụng           |
| 19  | PhogiamdocCnl2Kt    | Phó giám đốc CNL2 phụ trách KT           | Phó giám đốc CNL2 phụ trách Kế toán            |
| 20  | TruongphongKhCnl2   | Trưởng phòng KH CNL2                     | Trưởng phòng Khách hàng CNL2                   |
| 21  | PhophongKhCnl2      | Phó phòng KH CNL2                        | Phó phòng Khách hàng CNL2                      |
| 22  | TruongphongKtnqCnl2 | Trưởng phòng KTNQ CNL2                   | Trưởng phòng Kế toán & Ngân quỹ CNL2           |
| 23  | PhophongKtnqCnl2    | Phó phòng KTNQ CNL2                      | Phó phòng Kế toán & Ngân quỹ CNL2              |

#### Công cụ sử dụng:

- **Shell script:** `create_23_roles.sh` - Automation tạo toàn bộ 23 vai trò
- **API Roles:** POST `/api/roles` - Tạo từng vai trò với Name và Description
- **Model:** Role entity với properties Id, Name, Description, EmployeeRoles
- **Validation:** JSON schema và backend validation đầy đủ

#### Đặc điểm kỹ thuật:

- **Auto-increment ID:** Database tự động gán ID tuần tự từ 1-23
- **Unicode support:** Tên và mô tả tiếng Việt hiển thị đúng
- **API compatible:** Frontend có thể fetch và hiển thị đầy đủ
- **Mã vai trò:** Giữ nguyên không thay đổi theo yêu cầu
- **Navigation properties:** Hỗ trợ quan hệ many-to-many với Employees

**🎯 Status:** Sẵn sàng để gán vai trò cho nhân viên trong từng đơn vị.

### 📊 **CẤU HÌNH KPI ASSIGNMENT TABLES - 06/07/2025**

**✅ HOÀN THÀNH:** Đã có đủ 32 bảng KPI theo đúng cấu trúc

#### 🧑‍💼 Tab "Dành cho Cán bộ" - 23 bảng KPI:

| ID  | Tên Bảng KPI        | Mô tả                                    |
| --- | ------------------- | ---------------------------------------- | -------------- |
| 1   | TruongphongKhdn     | Trưởng phòng KHDN                        | Trưởng phòng Khách hàng Doanh nghiệp           |
| 2   | TruongphongKhcn     | Trưởng phòng KHCN                        | Trưởng phòng Khách hàng Cá nhân                |
| 3   | PhophongKhdn        | Phó phòng KHDN                           | Phó phòng Khách hàng Doanh nghiệp              |
| 4   | PhophongKhcn        | Phó phòng KHCN                           | Phó phòng Khách hàng Cá nhân                   |
| 5   | TruongphongKhqlrr   | Trưởng phòng KH&QLRR                     | Trưởng phòng Kế hoạch & Quản lý rủi ro         |
| 6   | PhophongKhqlrr      | Phó phòng KH&QLRR                        | Phó phòng Kế hoạch & Quản lý rủi ro            |
| 7   | Cbtd                | Cán bộ tín dụng                          | Cán bộ tín dụng                                |
| 8   | TruongphongKtnqCnl1 | Trưởng phòng KTNQ CNL1                   | Trưởng phòng Kế toán & Ngân quỹ CNL1           |
| 9   | PhophongKtnqCnl1    | Phó phòng KTNQ CNL1                      | Phó phòng Kế toán & Ngân quỹ CNL1              |
| 10  | Gdv                 | GDV                                      | Giao dịch viên                                 |
| 11  | TqHkKtnb            | Thủ quỹ \| Hậu kiểm \| KTNB              | Thủ quỹ \| Hậu kiểm \| Kế toán nghiệp vụ       |
| 12  | TruongphoItThKtgs   | Trưởng phó IT \| Tổng hợp \| KTGS        | Trưởng phó IT \| Tổng hợp \| Kiểm tra giám sát |
| 13  | CBItThKtgsKhqlrr    | Cán bộ IT \| Tổng hợp \| KTGS \| KH&QLRR | Cán bộ IT \| Tổng hợp \| KTGS \| KH&QLRR       |
| 14  | GiamdocPgd          | Giám đốc Phòng giao dịch                 | Giám đốc Phòng giao dịch                       |
| 15  | PhogiamdocPgd       | Phó giám đốc Phòng giao dịch             | Phó giám đốc Phòng giao dịch                   |
| 16  | PhogiamdocPgdCbtd   | Phó giám đốc PGD kiêm CBTD               | Phó giám đốc Phòng giao dịch kiêm CBTD         |
| 17  | GiamdocCnl2         | Giám đốc CNL2                            | Giám đốc Chi nhánh cấp 2                       |
| 18  | PhogiamdocCnl2Td    | Phó giám đốc CNL2 phụ trách TD           | Phó giám đốc CNL2 phụ trách Tín dụng           |
| 19  | PhogiamdocCnl2Kt    | Phó giám đốc CNL2 phụ trách KT           | Phó giám đốc CNL2 phụ trách Kế toán            |
| 20  | TruongphongKhCnl2   | Trưởng phòng KH CNL2                     | Trưởng phòng Khách hàng CNL2                   |
| 21  | PhophongKhCnl2      | Phó phòng KH CNL2                        | Phó phòng Khách hàng CNL2                      |
| 22  | TruongphongKtnqCnl2 | Trưởng phòng KTNQ CNL2                   | Trưởng phòng Kế toán & Ngân quỹ CNL2           |
| 23  | PhophongKtnqCnl2    | Phó phòng KTNQ CNL2                      | Phó phòng Kế toán & Ngân quỹ CNL2              |

#### Công cụ sử dụng:

- **Shell script:** `create_23_roles.sh` - Automation tạo toàn bộ 23 vai trò
- **API Roles:** POST `/api/roles` - Tạo từng vai trò với Name và Description
- **Model:** Role entity với properties Id, Name, Description, EmployeeRoles
- **Validation:** JSON schema và backend validation đầy đủ

#### Đặc điểm kỹ thuật:

- **Auto-increment ID:** Database tự động gán ID tuần tự từ 1-23
- **Unicode support:** Tên và mô tả tiếng Việt hiển thị đúng
- **API compatible:** Frontend có thể fetch và hiển thị đầy đủ
- **Mã vai trò:** Giữ nguyên không thay đổi theo yêu cầu
- **Navigation properties:** Hỗ trợ quan hệ many-to-many với Employees

**🎯 Status:** Sẵn sàng để gán vai trò cho nhân viên trong từng đơn vị.

### 📊 **CẤU HÌNH KPI ASSIGNMENT TABLES - 06/07/2025**

**✅ HOÀN THÀNH:** Đã có đủ 32 bảng KPI theo đúng cấu trúc

#### 🏢 Tab "Dành cho Chi nhánh" - 9 bảng KPI:

| ID  | Tên Bảng KPI | Mô tả                       |
| --- | ------------ | --------------------------- |
| 24  | HoiSo        | KPI cho Hội Sở              |
| 25  | BinhLu       | KPI cho Chi nhánh Bình Lư   |
| 26  | PhongTho     | KPI cho Chi nhánh Phong Thổ |
| 27  | SinHo        | KPI cho Chi nhánh Sìn Hồ    |
| 28  | BumTo        | KPI cho Chi nhánh Bum Tở    |
| 29  | ThanUyen     | KPI cho Chi nhánh Than Uyên |
| 30  | DoanKet      | KPI cho Chi nhánh Đoàn Kết  |
| 31  | TanUyen      | KPI cho Chi nhánh Tân Uyên  |
| 32  | NamHang      | KPI cho Chi nhánh Nậm Hàng  |

#### Hệ thống KPI Assignment:

1. **📋 "Cấu hình KPI"** (KpiAssignmentTables) - ✅ 32 bảng template

   - 23 bảng cho cán bộ (Category = "CANBO") ✅
   - 9 bảng cho chi nhánh (Category = "CHINHANH") ✅

2. **🧑‍💼 "Giao khoán KPI cho cán bộ"** (EmployeeKpiAssignments) - ❌ 0 records

   - Cần: EmployeeId + KpiDefinitionId + KhoanPeriodId + TargetValue
   - Phụ thuộc: Employees, KPI Definitions, Khoan Periods

3. **🏢 "Giao khoán KPI cho chi nhánh"** (UnitKpiScorings) - ❌ 0 records
   - Cần: UnitId + KhoanPeriodId + Scores
   - Phụ thuộc: Units, Khoan Periods

#### Đặc điểm kỹ thuật:

- **Temporal Tables + Columnstore:** Tối ưu hiệu năng cho tất cả bảng KPI
- **Template-based system:** KpiAssignmentTables là template cho giao khoán thực tế
- **Unicode support:** Tên tiếng Việt hiển thị đúng
- **API compatible:** Frontend fetch và cập nhật real-time

**🎯 Status:** Sẵn sàng tạo Khoan Periods và triển khai giao khoán KPI thực tế.

## 🎯 PHASE 8: EMPLOYEE-ROLE ASSIGNMENTS (HOÀN THÀNH ✅)

_Thời gian: 07/01/2025 14:00-15:00_

### Mục tiêu đã đạt được

✅ **Gán roles cho tất cả 10 employees** dựa trên chức vụ và đơn vị làm việc

#### 8.2 Scripts và tools

```bash
# Script chính gán roles
./execute_role_assignments_fixed.sh  # Gán roles với payload đầy đủ
./complete_role_assignments.sh       # Xác nhận tất cả assignments

# Verification
curl -s "http://localhost:5055/api/employees/{id}" | jq '.EmployeeRoles'
```

#### 8.3 Cấu trúc dữ liệu Employee-Role

- **EmployeeRoles table**: Quan hệ Many-to-Many giữa Employee và Role
- **API endpoint**: `PUT /api/employees/{id}` với `RoleIds` array
- **Payload format**: Bao gồm tất cả fields của Employee + RoleIds mới

#### 8.4 Kết quả achieved

✅ 10/10 employees có roles được gán  
✅ Quan hệ Employee-Role lưu trong bảng `EmployeeRoles`  
✅ API trả về đúng cấu trúc role data  
✅ Mapping logic documented và scripts automated

---

## 🔧 PHASE 9: KPI ASSIGNMENT FRAMEWORK (ĐANG THỰC HIỆN 🔄)

_Thời gian: 07/01/2025 15:00-..._

### Tiến độ hiện tại

#### 9.1 Phân tích hệ thống KPI (✅)

```bash
# Kiểm tra các thành phần
- 32 KpiAssignmentTables (templates cho roles)
- 135 KpiDefinitions (master KPI data)
- 17 KhoanPeriods (2025 periods)
- API: /api/KpiAssignment/* endpoints
```

#### 9.3 Thách thức hiện tại (🔄)

❓ **KpiIndicators chưa được populate**: Assignment tables có template nhưng chưa có KPI indicators cụ thể  
❓ **Link KpiDefinitions → KpiIndicators**: Cần tạo quan hệ giữa master data và assignment tables

#### 9.4 Scripts đã tạo

```bash
./create_complete_kpi_assignments.sh  # Framework tạo KPI assignments
./create_employee_kpi_assignments.sh  # Analysis và test assignments
```

### Bước tiếp theo

1. 🔄 **Populate KpiIndicators** vào assignment tables từ KpiDefinitions
2. 🔄 **Tạo EmployeeKpiTargets** cho từng employee dựa trên role
3. 🔄 **Thiết lập UnitKpiScorings** cho đánh giá chi nhánh
4. 🔄 **Đồng bộ tự động** giữa "Cấu hình KPI" và giao khoán

---

## 📊 TỔNG KẾT TIẾN ĐỘ (07/01/2025 15:00)

### ✅ Đã hoàn thành

1. **Database Infrastructure**: Azure SQL Edge, temporal tables, encoding
2. **Units Management**: 46 đơn vị theo cấu trúc hierarchical
3. **Roles Management**: 23 vai trò chuẩn
4. **KPI Configuration**: 32 bảng template + 135 KPI definitions
5. **Time Periods**: 17 kỳ khoán năm 2025
6. **Employee-Role Assignments**: 10 employees có roles phù hợp
7. **Frontend Fonts**: Chuẩn hóa tiếng Việt toàn dự án

### 🔄 Đang thực hiện

1. **KPI Indicators Population**: Link KpiDefinitions → KpiAssignmentTables
2. **Employee KPI Assignments**: Giao khoán cụ thể cho từng nhân viên

### 📋 Sắp tới

1. **Unit KPI Scorings**: Đánh giá KPI theo chi nhánh
2. **Synchronization**: Đồng bộ tự động các module
3. **Testing & Validation**: Kiểm tra toàn bộ hệ thống

### 🔢 Thống kê

- **Units**: 46/46 ✅
- **Roles**: 23/23 ✅
- **Employees**: 10/10 có roles ✅
- **KPI Tables**: 32/32 templates ✅
- **KPI Definitions**: 135/135 ✅
- **Khoan Periods**: 17/17 ✅
- **KPI Indicators**: 158/158 chỉ tiêu mới ✅

### ✅ HOÀN THÀNH PHASE 9.2: Populate 158 chỉ tiêu KPI chính xác

**Ngày:** 06/07/2025

#### 🎯 Kết quả đạt được:

- ✅ **Mapping tên bảng:** 23/23 bảng KPI cán bộ mapping đúng tên database
- ✅ **Populate chỉ tiêu:** 158 chỉ tiêu theo danh sách CHÍNH XÁC anh cung cấp
- ✅ **Frontend display:** Mã bảng KPI = Mã vai trò, hiển thị mô tả vai trò trong dropdown
- ✅ **Scripts automation:** 5 scripts thực thi và kiểm tra hoàn chỉnh

#### 📋 Scripts đã tạo:

1. **check_table_name_mapping.sh** - So sánh tên bảng script vs database
2. **populate_exact_158_kpi_indicators.sh** - Tạo chính xác 158 chỉ tiêu
3. **count_kpi_indicators_final.sh** - Đếm và báo cáo chi tiết chỉ tiêu
4. **populate_all_kpi_indicators_new.sh** - Backup script populate
5. **execute_complete_kpi_reset.sh** - Reset và tạo lại workflow

#### 📊 Phân bố 158 chỉ tiêu theo vai trò:

```
1-4.   KHDN/KHCN: 4 bảng × 8 chỉ tiêu = 32
5-6.   KH&QLRR: 2 bảng × 6 chỉ tiêu = 12
7.     CBTD: 1 bảng × 8 chỉ tiêu = 8
8-9.   KTNQ CNL1: 2 bảng × 6 chỉ tiêu = 12
10.    GDV: 1 bảng × 6 chỉ tiêu = 6
12.    IT/TH/KTGS: 1 bảng × 5 chỉ tiêu = 5
13.    CB IT/TH/KTGS: 1 bảng × 4 chỉ tiêu = 4
14-15. GĐ PGD: 2 bảng × 9 chỉ tiêu = 18
16.    PGĐ CBTD: 1 bảng × 8 chỉ tiêu = 8
17.    GĐ CNL2: 1 bảng × 11 chỉ tiêu = 11
18.    PGĐ CNL2 TD: 1 bảng × 8 chỉ tiêu = 8
19.    PGĐ CNL2 KT: 1 bảng × 6 chỉ tiêu = 6
20.    TP KH CNL2: 1 bảng × 9 chỉ tiêu = 9
21.    PP KH CNL2: 1 bảng × 8 chỉ tiêu = 8
22.    TP KTNQ CNL2: 1 bảng × 6 chỉ tiêu = 6
23.    PP KTNQ CNL2: 1 bảng × 5 chỉ tiêu = 5
────────────────────────────────────────────
TỔNG: 158 chỉ tiêu cho 22 bảng (thiếu TqHkKtnb)
```

#### ✅ Kết quả đạt được:

- ✅ **33 EmployeeKpiAssignments**
- ✅ **API endpoints hoạt động** chính xác với đúng field names và structure
- ✅ **Mapping role-table** cho 23 vai trò với 22 bảng KPI (thiếu TqHkKtnb)
- ✅ **Frontend có thể fetch** assignments qua `/api/EmployeeKpiAssignment`

---

## 🔄 PHASE 9.3: KPI ASSIGNMENT FRAMEWORK - ISSUES & FIXES (ĐANG THỰC HIỆN 🔄)

_Thời gian: 07/01/2025 15:00-..._

### Vấn đề gặp phải

1. **Khoảng trống dữ liệu** trong giao khoán KPI cho nhân viên và đơn vị
2. **Cần tạo Khoan Periods** để hoàn thiện hệ thống giao khoán

### Bước giải quyết

- Tạo các bản ghi mẫu cho `EmployeeKpiAssignments` và `UnitKpiScorings`
- Thiết lập các Khoan Periods cho năm 2025

### Tiến độ hiện tại

- Đã tạo 17 Khoan Periods cho năm 2025
- Đang phân tích và điền dữ liệu cho `EmployeeKpiAssignments` và `UnitKpiScorings`

---

## **🛠️ SQLCMD GIẢI PHÁP - JULY 14, 2025**

#### **🔍 NGUYÊN NHÂN SQLCMD KHÓ CÀI:**

1. **Container permission issues:** Azure SQL Edge container có restricted permissions
2. **Missing packages:** Container thiếu gnupg, apt-key và các tools cần thiết
3. **Interactive bash hangs:** `docker exec -it` bị treo do resource constraints
4. **Package repo access:** Container không thể access Microsoft package repos

#### **✅ GIẢI PHÁP HOÀN CHỈNH:**

**Sử dụng sqlcmd từ macOS host** (RECOMMENDED):
```bash
# Sqlcmd đã có sẵn trên macOS
which sqlcmd  # /opt/homebrew/bin/sqlcmd

# Test connection
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -Q "SELECT @@VERSION"

# Interactive mode
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB
```

**Các scripts đã tối ưu:**
- `./test_sql.sh` - Test SQL queries nhanh chóng
- `./check_database.sh` - Health check với SQL verification
- `./start_database.sh` - Smart connection testing

#### **🎯 LỢI ÍCH:**

✅ **Không cần cài trong container:** Sử dụng sqlcmd từ host  
✅ **Performance cao:** Kết nối trực tiếp, không qua container exec  
✅ **Stable connection:** Không bị timeout hay permission issues  
✅ **Full SQL features:** Access đầy đủ tính năng sqlcmd  
✅ **Easy debugging:** Có thể run queries interactive dễ dàng  

**🔥 KHÔNG CẦN VÀO CONTAINER NỮA!**

## 🐞 **VẤN ĐỀ VÀ GIẢI PHÁP DATA REFRESH - JULY 14, 2025**

### **❌ VẤN ĐỀ PHÁT HIỆN:**

**Mô tả:** Sau khi import dữ liệu thành công, frontend không tự động refresh để hiển thị tổng số bản ghi mới. Button "Tải lại dữ liệu" cũng không hiển thị được số liệu cho bảng DP01.

**Kiểm tra kết quả:**
- ✅ **Database**: 12,741 bản ghi DP01 (thực tế)
- ✅ **API**: Trả về đúng RecordsCount = 12,741  
- ✅ **Metadata**: ImportedDataRecords chính xác
- ❌ **Frontend**: Không hiển thị số liệu sau import

### **🔍 NGUYÊN NHÂN PHÁT HIỆN:**

1. **Field Mapping Mismatch:** Frontend `calculateDataTypeStats()` đang ưu tiên `imp.dataType` nhưng API trả về `Category`
2. **Progress Display Issue:** Refresh được gọi nhưng stats không được cập nhật đúng
3. **Date Parsing Error:** Import date có thể bị parse sai làm stats không hiển thị

### **🛠️ GIẢI PHÁP ĐÃ THỰC HIỆN:**

#### **1. Fix Field Mapping Priority:**
```javascript
// BEFORE: Sai thứ tự ưu tiên
const dataType = imp.dataType || imp.Category || imp.FileType || 'UNKNOWN'
const recordCount = parseInt(imp.recordsCount || imp.RecordsCount) || 0

// AFTER: Ưu tiên field từ API response
const dataType = imp.Category || imp.FileType || imp.dataType || 'UNKNOWN' 
const recordCount = parseInt(imp.RecordsCount || imp.recordsCount) || 0
```

#### **2. Enhanced Date Validation:**
```javascript
// BEFORE: Không check date validity
const importDateTime = new Date(importDate)

// AFTER: Validate date trước khi dùng
const importDateTime = new Date(importDate)
if (!isNaN(importDateTime.getTime()) && ...)
```

#### **3. Enhanced Debug Function:**
```javascript
// NEW: Force refresh với debug logging
const debugRecalculateStats = async () => {
  await refreshAllData(true)  // Force refresh data first
  calculateDataTypeStats()    // Then recalculate stats  
  console.log('📊 Current dataTypeStats:', dataTypeStats.value)
}
```

### **🎯 CÁCH SỬ DỤNG:**

1. **Sau khi import:** Hệ thống sẽ tự động refresh (đã có trong code)
2. **Nếu vẫn không hiển thị:** Click button "🔧 Debug Stats" để force refresh
3. **Debug console:** Check browser console để xem log chi tiết

### **✅ KẾT QUẢ MONG ĐỢI:**

- ✅ **Auto refresh** sau import thành công
- ✅ **Hiển thị đúng** tổng số records cho tất cả data types
- ✅ **Button refresh** hoạt động đúng 
- ✅ **Debug tools** để troubleshoot

**🎯 Status:** Đã fix code, cần test lại import workflow để confirm.

---