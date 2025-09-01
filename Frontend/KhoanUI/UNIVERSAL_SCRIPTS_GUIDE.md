# 🌟 Universal Scripts - Chạy từ bất cứ đâu!

## 📋 Tổng quan

Các script universal này có thể chạy từ bất cứ thư mục nào trong project workspace:

- **Frontend (KhoanUI)**: `./frontend.sh` hoặc `./universal_frontend.sh`
- **Backend (Khoan.Api)**: `./backend.sh` hoặc `./universal_backend.sh`
- **Fullstack**: `./fullstack.sh` hoặc `./universal_fullstack.sh`

## 🚀 Cách sử dụng

### Từ thư mục Frontend (`/opt/Projects/Khoan/Frontend/KhoanUI`)

```bash
# Chạy backend từ thư mục frontend
./backend.sh

# Chạy frontend từ thư mục frontend
./frontend.sh

# Chạy fullstack từ thư mục frontend
./fullstack.sh
```

### Từ thư mục Backend (`/opt/Projects/Khoan/Backend/Khoan.Api`)

```bash
# Chạy backend từ thư mục backend
./backend.sh

# Chạy frontend từ thư mục backend
./frontend.sh

# Chạy fullstack từ thư mục backend
./fullstack.sh
```

### Từ bất cứ thư mục nào trong workspace

Scripts sẽ tự động detect đúng thư mục project dù bạn đang ở đâu!

## 🔍 Auto-detection Logic

Scripts sử dụng logic thông minh để detect thư mục project:

1. **Kiểm tra thư mục hiện tại**:
   - Nếu có `package.json` + `vite.config.js` → Frontend directory
   - Nếu có `Khoan.Api.csproj` → Backend directory

2. **Tìm kiếm ngược lên thư mục cha**:
   - Walk up directory tree để tìm project root
   - Look for `/Frontend/KhoanUI` và `/Backend/Khoan.Api`

3. **Fallback**:
   - Sử dụng absolute paths: `/opt/Projects/Khoan/`

## ⚡ Tính năng

### ✅ Universal Detection

- Tự động detect project directories
- Hoạt động từ bất cứ đâu trong workspace
- Fallback an toàn đến absolute paths

### ✅ Smart Process Management

- Kill existing processes trước khi start
- PID tracking để manage processes
- Graceful shutdown với SIGTERM/SIGKILL

### ✅ Comprehensive Logging

- Logs được lưu trong thư mục tương ứng:
  - Frontend: `/opt/Projects/Khoan/Frontend/KhoanUI/frontend.log`
  - Backend: `/opt/Projects/Khoan/Backend/Khoan.Api/backend.log`
- Log rotation tự động
- UTF-8 support cho tiếng Việt

### ✅ Health Checks

- Validate directories trước khi start
- Port availability checks
- HTTP health checks sau khi start
- Build validation (backend)
- Dependencies check (frontend)

## 📁 File Structure

```
Frontend/KhoanUI/
├── backend.sh              # Alias to start backend
├── frontend.sh             # Alias to start frontend
├── fullstack.sh            # Alias to start fullstack
├── universal_backend.sh    # Universal backend script
├── universal_frontend.sh   # Universal frontend script
└── universal_fullstack.sh  # Universal fullstack script

Backend/Khoan.Api/
├── backend.sh              # Alias to start backend
├── frontend.sh             # Alias to start frontend
├── fullstack.sh            # Alias to start fullstack
├── universal_backend.sh    # Universal backend script
├── universal_frontend.sh   # Universal frontend script
└── universal_fullstack.sh  # Universal fullstack script
```

## 🎯 Ports & URLs

- **Frontend**: http://localhost:3000
- **Backend**: http://localhost:5055
- **API Docs**: http://localhost:5055/swagger
- **Health Check**: http://localhost:5055/health

## 🛠️ Troubleshooting

### Script không tìm thấy project directories

Scripts sẽ báo lỗi rõ ràng:

```bash
ERROR: Could not find frontend directory: /path/to/frontend
Please run this script from within the Khoan project workspace
```

### Port conflicts

Scripts tự động kill existing processes trước khi start mới.

### Build failures

Check logs trong respective directories:

- Frontend: `frontend.log`
- Backend: `backend.log`

## 💡 Advanced Usage

### Environment Variables

```bash
# Frontend log settings
export FRONTEND_MAX_LOG_SIZE=10M
export FRONTEND_LOG_BACKUPS=10

# Backend log settings
export BACKEND_MAX_LOG_SIZE=10M
export BACKEND_LOG_BACKUPS=10
```

### Manual Process Management

```bash
# Kill by PID files
kill $(cat /opt/Projects/Khoan/Frontend/KhoanUI/frontend.pid)
kill $(cat /opt/Projects/Khoan/Backend/Khoan.Api/backend.pid)

# Kill by process pattern
pkill -f 'npm.*dev'
pkill -f 'dotnet.*Khoan.Api'

# Kill by port
lsof -ti:3000 | xargs kill -9
lsof -ti:5055 | xargs kill -9
```

## 🎉 Success Output Example

```bash
$ cd /opt/Projects/Khoan/Backend/Khoan.Api
$ ./fullstack.sh

[2025-09-01 08:15:00] 🌟 Universal Fullstack Startup Script
[2025-09-01 08:15:00] 📂 Current directory: /opt/Projects/Khoan/Backend/Khoan.Api
[2025-09-01 08:15:00] 🎯 Project root: /opt/Projects/Khoan
[2025-09-01 08:15:00] 📊 Frontend: /opt/Projects/Khoan/Frontend/KhoanUI
[2025-09-01 08:15:00] 🔧 Backend: /opt/Projects/Khoan/Backend/Khoan.Api
[2025-09-01 08:15:00]
[2025-09-01 08:15:00] 🌐 Frontend: http://localhost:3000
[2025-09-01 08:15:00] 🔧 Backend API: http://localhost:5055
[2025-09-01 08:15:00] 📖 API Docs: http://localhost:5055/swagger
[2025-09-01 08:15:02] ✅ Backend started successfully on port 5055
[2025-09-01 08:15:05] ✅ Frontend started successfully on port 3000
[2025-09-01 08:15:08] ✅ Backend health check passed
[2025-09-01 08:15:08] ✅ Frontend health check passed
[2025-09-01 08:15:08] 🎉 Fullstack application started successfully!
```

---

_Universal Scripts được tạo ngày 1 September 2025_
