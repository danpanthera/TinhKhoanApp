# TinhKhoanApp - Hướng dẫn cho nhà phát triển

Đây là tài liệu hướng dẫn toàn diện cho các nhà phát triển làm việc với dự án TinhKhoanApp. Tài liệu cung cấp thông tin về cấu trúc dự án, quy trình phát triển, và các best practices.

## Cấu trúc dự án

Dự án TinhKhoanApp được chia thành hai phần chính:

### 1. Frontend (tinhkhoan-app-ui-vite)

Phần frontend được xây dựng bằng Vue.js 3 và Vite, sử dụng TypeScript để type checking.

```
tinhkhoan-app-ui-vite/
├── public/             # Tài nguyên tĩnh (images, sounds, etc.)
├── src/
│   ├── api/            # Các API calls
│   ├── assets/         # Assets (images, css, etc.)
│   ├── components/     # Vue components tái sử dụng
│   ├── composables/    # Composable functions (Composition API)
│   ├── router/         # Vue Router configuration
│   ├── services/       # Business logic services
│   ├── stores/         # State management (Pinia/Vuex)
│   ├── utils/          # Utility functions
│   ├── views/          # Vue components tương ứng với routes
│   ├── App.vue         # Root component
│   ├── main.js         # Entry point
│   └── style.css       # Global styles
├── .eslintrc.cjs       # ESLint configuration
├── index.html          # HTML entry point
├── package.json        # Dependencies và scripts
├── tsconfig.json       # TypeScript configuration
└── vite.config.js      # Vite configuration
```

### 2. Backend (TinhKhoanApp.Api)

Backend được xây dựng với ASP.NET Core và Entity Framework Core.

```
TinhKhoanApp.Api/
├── Controllers/        # API Controllers
├── Data/               # Database context và migrations
├── Models/             # Domain models và DTOs
├── Services/           # Business logic services
├── Repositories/       # Data access layer
├── Program.cs          # Application entry point
├── Startup.cs          # Application configuration
└── appsettings.json    # Configuration files
```

## Quy trình phát triển

### Cài đặt môi trường phát triển

#### Frontend

```bash
# Clone dự án
git clone <repository-url>

# Di chuyển vào thư mục frontend
cd TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite

# Cài đặt dependencies
npm install

# Khởi động dev server
npm run dev
# Hoặc sử dụng task
./start_frontend.sh
```

#### Backend

```bash
# Di chuyển vào thư mục backend
cd TinhKhoanApp/Backend/TinhKhoanApp.Api

# Restore packages
dotnet restore

# Khởi động API
dotnet run
# Hoặc sử dụng task
./start_backend.sh
```

### Các lệnh npm quan trọng (Frontend)

```bash
# Khởi động dev server
npm run dev

# Build project
npm run build

# Preview build
npm run preview

# Kiểm tra lỗi TypeScript
npm run type-check

# Kiểm tra và sửa lỗi ESLint
npm run lint

# Kiểm tra và sửa định dạng code
npm run format

# Kiểm tra các warning phổ biến
npm run fix-warnings

# Kiểm tra toàn diện
npm run quality-check

# Dọn dẹp cache và node_modules
npm run clean-all
```

### Các lệnh dotnet quan trọng (Backend)

```bash
# Build project
dotnet build

# Run tests
dotnet test

# Update database từ migrations
dotnet ef database update

# Tạo migration mới
dotnet ef migrations add <migration-name>

# Dọn dẹp project
dotnet clean
```

## Best Practices

### Coding Standards

- Tuân thủ hướng dẫn trong `CODE_QUALITY_GUIDE.md`
- Sử dụng TypeScript cho mọi file JavaScript mới
- Sử dụng Composition API cho Vue components
- Viết tests cho các chức năng quan trọng

### Git Workflow

1. Pull latest từ main branch
2. Tạo feature branch mới
3. Commit thường xuyên với commit messages rõ ràng
4. Kiểm tra code quality trước khi push
5. Tạo Pull Request để review
6. Merge sau khi đã review và test

### Quy trình Review Code

- Kiểm tra coding standards
- Đảm bảo không có bugs hoặc edge cases
- Đảm bảo code dễ bảo trì và hiểu
- Tìm kiếm các optimizations nếu có thể

## Debugging

### Frontend

- Sử dụng Vue DevTools để debug Vue components
- Sử dụng browser developer tools
- Kiểm tra logs trong console
- Component `DebugPanel.vue` có thể được sử dụng để debug

### Backend

- Sử dụng VS Code debugger với .NET
- Kiểm tra logs trong console hoặc log files
- Sử dụng Swagger UI để test API endpoints

## API Endpoints

Danh sách các API endpoints chính:

- `GET /api/users` - Lấy danh sách users
- `GET /api/departments` - Lấy danh sách phòng ban
- `GET /api/kpi` - Lấy danh sách KPI
- `POST /api/upload` - Upload dữ liệu

Chi tiết đầy đủ có thể được tìm thấy trong file `api-tests.http` hoặc tài liệu Swagger.

## Tài nguyên bổ sung

- [Vue.js Documentation](https://vuejs.org/guide/introduction.html)
- [Vite Documentation](https://vitejs.dev/guide/)
- [TypeScript Documentation](https://www.typescriptlang.org/docs/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)

## Troubleshooting

### Các vấn đề phổ biến và cách giải quyết

#### 1. Frontend Dev Server không khởi động

- Kiểm tra port 5173 đã được sử dụng chưa
- Kiểm tra node_modules đã cài đặt đầy đủ chưa
- Thử chạy `npm run clean-all` và cài đặt lại

#### 2. Type Errors trong TypeScript

- Kiểm tra `tsconfig.json` cấu hình đúng
- Đảm bảo các types đã được khai báo đầy đủ
- Thử chạy `npm run type-check` để phát hiện lỗi

#### 3. Backend API không phản hồi

- Kiểm tra database connection string
- Đảm bảo migrations đã được applied
- Kiểm tra logs để tìm lỗi cụ thể

## Liên hệ

Nếu bạn có câu hỏi hoặc gặp vấn đề không được đề cập trong tài liệu này, vui lòng liên hệ:

- **Project Lead**: [Tên người quản lý]
- **Frontend Lead**: [Tên lead frontend]
- **Backend Lead**: [Tên lead backend]
- **Email**: [email@example.com]
