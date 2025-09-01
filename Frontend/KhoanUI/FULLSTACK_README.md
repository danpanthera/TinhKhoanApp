# 🌟 KhoanUI + Khoan.Api Fullstack Application

## 📋 Cấu hình

- **Frontend (KhoanUI)**: Port 3000 - http://localhost:3000
- **Backend (Khoan.Api)**: Port 5055 - http://localhost:5055
- **API Documentation**: http://localhost:5055/swagger

## 🚀 Khởi động

### Khởi động toàn bộ hệ thống

```bash
cd /opt/Projects/Khoan/Frontend/KhoanUI
./start_fullstack.sh
```

Hoặc sử dụng npm script:

```bash
npm run fullstack
```

### Dừng toàn bộ hệ thống

```bash
./stop_fullstack.sh
```

Hoặc:

```bash
npm run stop
```

## 🔧 Khởi động riêng lẻ

### Chỉ Frontend

```bash
cd /opt/Projects/Khoan/Frontend/KhoanUI
npm run dev
```

### Chỉ Backend

```bash
cd /opt/Projects/Khoan/Backend/Khoan.Api
dotnet run --project Khoan.Api.csproj --urls "http://localhost:5055"
```

## 📊 Giám sát

- **Frontend logs**: `frontend.log`
- **Backend logs**: `backend.log`
- **Process IDs**: Lưu trong `frontend.pid` và `backend.pid`

## 🏥 Health Checks

- Backend: http://localhost:5055/health
- Frontend: http://localhost:3000

## ⚡ Tính năng ổn định

- ✅ Auto-kill existing processes before starting
- ✅ Graceful shutdown handling
- ✅ Health checks after startup
- ✅ PID tracking for process management
- ✅ Log rotation
- ✅ Error handling and retry mechanisms
- ✅ UTF-8 support for Vietnamese
- ✅ CORS configured between Frontend and Backend
- ✅ Proxy configuration: Frontend `/api/*` → Backend

## 🛠️ Troubleshooting

### Port đã được sử dụng

Script sẽ tự động kill các process cũ trước khi start.

### Backend không kết nối được database

Kiểm tra connection string trong `appsettings.json`:

```json
"DefaultConnection": "Server=localhost,1433;Database=TinhKhoanDB;..."
```

### Frontend không proxy được đến Backend

Kiểm tra cấu hình trong `vite.config.js`:

```javascript
proxy: {
  '/api': {
    target: 'http://localhost:5055',
    changeOrigin: true,
    secure: false
  }
}
```

### Memory issues

Script có log rotation tự động khi file log > 5MB.

## 📁 Cấu trúc

```
Frontend/KhoanUI/          # Vue 3 + Vite Frontend
├── src/                   # Source code
├── public/                # Static assets
├── vite.config.js         # Vite configuration
├── package.json           # NPM dependencies
├── start_fullstack.sh     # Start both services
└── stop_fullstack.sh      # Stop both services

Backend/Khoan.Api/         # .NET 8 Web API
├── Controllers/           # API Controllers
├── Services/              # Business logic
├── Data/                  # Database context
├── Program.cs             # Application entry point
├── appsettings.json       # Configuration
└── Khoan.Api.csproj       # Project file
```

## 🔒 Bảo mật

- JWT Authentication configured
- CORS enabled for Frontend→Backend communication
- HTTPS redirect available (commented in Program.cs)

## 📦 Dependencies

### Frontend

- Vue 3.5.13
- Element Plus UI
- Axios for API calls
- Pinia for state management

### Backend

- .NET 8
- Entity Framework Core
- SQL Server
- JWT Authentication

---

_Last updated: September 1, 2025_
