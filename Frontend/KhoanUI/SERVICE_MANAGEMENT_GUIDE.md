# TinhKhoan Service Management Documentation

## Enhanced Service Management Scripts

### Overview

The TinhKhoan Banking Application now has a comprehensive service management system with enhanced startup and shutdown scripts for both backend (.NET Core 8.0) and frontend (Vue.js 3) services.

### Service Architecture

- **Backend API**: .NET Core 8.0 running on port 5055
- **Frontend**: Vue.js 3 with Vite dev server on port 3000
- **Database**: SQL Server (managed via Docker container)

### Enhanced Scripts

#### 1. Backend Scripts

- **`start_backend.sh`**: Enhanced startup with build verification, health monitoring, and 20-step startup process
- **`stop_backend.sh`**: Enhanced shutdown with graceful termination, aggressive cleanup, and port verification

#### 2. Frontend Scripts

- **`start_frontend.sh`**: Enhanced startup with dependency checks, cache cleanup, and 60-second monitoring
- **`stop_frontend.sh`**: Enhanced shutdown with process cleanup and port verification

#### 3. Master Service Manager

- **`service_manager.sh`**: One-command management for all services

### Usage Instructions

#### Starting Services

**Option 1: Individual Services**

```bash
# Start backend only
cd Backend/Khoan.Api
./start_backend.sh

# Start frontend only
cd Frontend/KhoanUI
./start_frontend.sh
```

**Option 2: All Services at Once**

```bash
cd Frontend/KhoanUI
./service_manager.sh start
```

**Option 3: Using VS Code Tasks**

- `🚀 Start Full App (Database + Backend + Frontend)`
- `🚀 Start Backend API`
- `🎨 Start Frontend Dev Server`

#### Stopping Services

**Individual Services:**

```bash
# Stop backend
cd Backend/Khoan.Api
./stop_backend.sh

# Stop frontend
cd Frontend/KhoanUI
./stop_frontend.sh
```

**All Services:**

```bash
cd Frontend/KhoanUI
./service_manager.sh stop
```

#### Restarting Services

```bash
cd Frontend/KhoanUI
./service_manager.sh restart
```

#### Checking Service Status

```bash
cd Frontend/KhoanUI
./service_manager.sh status
```

### Key Features

#### Enhanced Error Handling

- Build verification before starting services
- Port conflict detection and resolution
- Process cleanup with aggressive fallback
- Health check monitoring with timeout

#### Logging & Monitoring

- Timestamped logging for all operations
- PID file management for process tracking
- Health status verification
- Detailed error reporting

#### Cross-Platform Compatibility

- macOS optimized (replacing timeout command with kill-based timeout)
- UTF-8 Vietnamese language support
- Proper signal handling (SIGTERM -> SIGKILL cascade)

#### Process Management

- Graceful shutdown with fallback to force kill
- Port cleanup and verification
- PID file management
- Background process handling

### Service Health Verification

The scripts automatically verify service health:

**Backend Health Check:**

```bash
curl -s http://localhost:5055/health
# Expected: HTTP 200 with "healthy" response
```

**Frontend Health Check:**

```bash
curl -s http://localhost:3000
# Expected: HTTP 200 with application response
```

### Troubleshooting

#### Common Issues

**Port Already in Use:**

- Scripts automatically detect and kill processes using required ports
- Use `lsof -ti:5055` or `lsof -ti:3000` to manually check port usage

**Build Failures:**

- Backend: Check `dotnet build` output in logs
- Frontend: Check `npm run build` output in logs
- Scripts will abort startup if build fails

**Process Cleanup:**

- Stop scripts use aggressive cleanup to ensure clean shutdown
- PID files are maintained for precise process management
- Orphaned processes are detected and cleaned up

#### Log Files

- **Backend**: `backend.log` with PID tracking in `backend.log.pid`
- **Frontend**: `frontend.log` with PID tracking in `frontend.log.pid`

### Development Workflow

1. **Initial Setup**: Use service manager to start all services
2. **Development**: Services run in background with hot reload
3. **Testing**: Use enhanced scripts to restart services cleanly
4. **Deployment**: Scripts ensure clean shutdown and startup

### Integration Status

All menu integrations verified and working:

- ✅ **B3 (Employee KPI Assignment)**: Integrated with A1 (branches), A2 (employees), B2 (KPI tables), accounting periods
- ✅ **B4 (Unit KPI Assignment)**: Integrated with A1 (branches), B2 (KPI tables), automatic hierarchy matching
- ✅ **API Endpoints**: All corrected and tested
- ✅ **Service Health**: Both services running and responsive

### Next Steps

The service management system is now production-ready with:

- Comprehensive error handling and logging
- Automated build verification
- Health monitoring and status checking
- Clean startup/shutdown procedures
- Master service management interface

Use `./service_manager.sh help` for complete usage information.
