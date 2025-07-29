#!/bin/bash

# 🔧 DOCKER BACKUP & RESTORE SCRIPT
# Backup toàn bộ container config và data trước khi reinstall Docker

set -e

BACKUP_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/docker_backup_$(date +%Y%m%d_%H%M%S)"
CONTAINER_NAME="azure_sql_edge_tinhkhoan"

echo "🎯 DOCKER BACKUP & RESTORE WORKFLOW"
echo "===================================="

# Tạo thư mục backup
mkdir -p "$BACKUP_DIR"
cd "$BACKUP_DIR"

echo "📁 Backup directory: $BACKUP_DIR"

# 1. Backup container configuration
echo ""
echo "🔧 1. BACKING UP CONTAINER CONFIG..."
docker inspect $CONTAINER_NAME > container_config.json
echo "✅ Container config saved to: container_config.json"

# 2. Export container image để backup
echo ""
echo "📦 2. BACKING UP CONTAINER IMAGE..."
docker commit $CONTAINER_NAME tinhkhoan_backup_image
docker save tinhkhoan_backup_image -o tinhkhoan_container_backup.tar
echo "✅ Container image backed up to: tinhkhoan_container_backup.tar"

# 3. Backup database data (nếu container đang chạy)
echo ""
echo "🗄️ 3. BACKING UP DATABASE DATA..."
if docker exec $CONTAINER_NAME sqlcmd -S localhost -U sa -P 'YourStrong@Password123' -Q "SELECT @@VERSION" > /dev/null 2>&1; then
    echo "✅ Database accessible, creating full backup..."

    # Backup database qua SQL
    docker exec $CONTAINER_NAME sqlcmd -S localhost -U sa -P 'YourStrong@Password123' -Q "BACKUP DATABASE TinhKhoanDB TO DISK = '/tmp/TinhKhoanDB_backup.bak'"

    # Copy backup file ra host
    docker cp $CONTAINER_NAME:/tmp/TinhKhoanDB_backup.bak ./TinhKhoanDB_backup.bak
    echo "✅ Database backup saved to: TinhKhoanDB_backup.bak"
else
    echo "⚠️ Database not accessible, skipping SQL backup"
fi

# 4. Backup volumes (nếu có)
echo ""
echo "💾 4. BACKING UP CONTAINER DATA..."
docker exec $CONTAINER_NAME tar czf /tmp/container_data.tar.gz /var/opt/mssql 2>/dev/null || echo "⚠️ No data volume found"
if docker cp $CONTAINER_NAME:/tmp/container_data.tar.gz ./container_data.tar.gz 2>/dev/null; then
    echo "✅ Container data saved to: container_data.tar.gz"
fi

# 5. Tạo restore script
echo ""
echo "📝 5. CREATING RESTORE SCRIPT..."
cat > restore_docker_container.sh << 'EOF'
#!/bin/bash

# 🔄 DOCKER RESTORE SCRIPT
# Restore container sau khi reinstall Docker

set -e

CONTAINER_NAME="azure_sql_edge_tinhkhoan"
BACKUP_DIR="$(pwd)"

echo "🔄 RESTORING DOCKER CONTAINER..."
echo "================================="

# 1. Load container image
echo "📦 1. Loading container image..."
docker load -i tinhkhoan_container_backup.tar
echo "✅ Container image loaded"

# 2. Run container với config từ backup
echo "🚀 2. Creating container với cấu hình cũ..."
docker run -e "ACCEPT_EULA=Y" \
           -e "MSSQL_SA_PASSWORD=YourStrong@Password123" \
           -p 1433:1433 \
           --name $CONTAINER_NAME \
           --restart=unless-stopped \
           --memory=4g \
           --memory-swap=8g \
           -d tinhkhoan_backup_image

echo "✅ Container created and started"

# 3. Wait for SQL Server startup
echo "⏳ 3. Waiting for SQL Server startup..."
sleep 30

# 4. Restore database nếu có backup
if [ -f "TinhKhoanDB_backup.bak" ]; then
    echo "🗄️ 4. Restoring database..."

    # Copy backup file vào container
    docker cp TinhKhoanDB_backup.bak $CONTAINER_NAME:/tmp/

    # Restore database
    docker exec $CONTAINER_NAME sqlcmd -S localhost -U sa -P 'YourStrong@Password123' -Q "RESTORE DATABASE TinhKhoanDB FROM DISK = '/tmp/TinhKhoanDB_backup.bak' WITH REPLACE"

    echo "✅ Database restored successfully"
else
    echo "⚠️ No database backup found, skipping restore"
fi

# 5. Test connection
echo "🔍 5. Testing connection..."
if docker exec $CONTAINER_NAME sqlcmd -S localhost -U sa -P 'YourStrong@Password123' -Q "SELECT COUNT(*) as TableCount FROM INFORMATION_SCHEMA.TABLES" > /dev/null 2>&1; then
    echo "✅ Database connection successful"
    docker exec $CONTAINER_NAME sqlcmd -S localhost -U sa -P 'YourStrong@Password123' -Q "SELECT COUNT(*) as TableCount FROM INFORMATION_SCHEMA.TABLES"
else
    echo "❌ Database connection failed"
fi

echo ""
echo "🎉 RESTORE COMPLETED!"
echo "Container: $CONTAINER_NAME"
echo "Status: $(docker ps --filter name=$CONTAINER_NAME --format 'table {{.Status}}')"
EOF

chmod +x restore_docker_container.sh
echo "✅ Restore script created: restore_docker_container.sh"

# 6. Tóm tắt backup
echo ""
echo "📋 BACKUP SUMMARY:"
echo "=================="
echo "📁 Backup location: $BACKUP_DIR"
echo "📄 Files created:"
ls -la
echo ""
echo "🔄 NEXT STEPS:"
echo "1. Quit Docker Desktop"
echo "2. Reinstall Docker Desktop"
echo "3. cd $BACKUP_DIR"
echo "4. ./restore_docker_container.sh"
echo ""
echo "✅ BACKUP COMPLETED SUCCESSFULLY!"
