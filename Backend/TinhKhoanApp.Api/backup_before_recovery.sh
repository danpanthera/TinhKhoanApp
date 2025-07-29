#!/bin/bash

# 🔧 DATABASE BACKUP BEFORE RECOVERY
# Export critical data before Docker recovery
# Ngày: 18/07/2025

echo "💾 === DATABASE BACKUP BEFORE RECOVERY ==="
echo "📅 Ngày: $(date)"
echo ""

# Màu sắc
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Backup directory
BACKUP_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api/backup_$(date +%Y%m%d_%H%M%S)"
mkdir -p "$BACKUP_DIR"

echo -e "${BLUE}📊 1. Test container connectivity...${NC}"
if sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -Q "SELECT @@VERSION" -W -t 10 >/dev/null 2>&1; then
    echo -e "${GREEN}✅ Container is accessible${NC}"
else
    echo -e "${RED}❌ Container not accessible - proceeding with volume backup${NC}"
    echo ""
    echo -e "${BLUE}📁 2. Docker volume backup...${NC}"

    # Backup Docker volumes nếu có thể
    echo "Listing Docker volumes..."
    docker volume ls
    echo ""

    # Try to backup container filesystem
    echo "Attempting container filesystem copy..."
    docker cp azure_sql_edge_tinhkhoan:/var/opt/mssql "$BACKUP_DIR/container_data" 2>/dev/null && \
        echo -e "${GREEN}✅ Container data copied to $BACKUP_DIR/container_data${NC}" || \
        echo -e "${YELLOW}⚠️  Container copy failed${NC}"

    exit 0
fi

echo -e "${BLUE}💾 2. Export critical tables...${NC}"

# Export Units (46 đơn vị)
echo "Exporting Units..."
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB \
    -Q "SELECT * FROM Units" -o "$BACKUP_DIR/units_backup.csv" -s "," -W -t 30 2>/dev/null && \
    echo "✅ Units exported" || echo "⚠️  Units export failed"

# Export Roles (23 vai trò)
echo "Exporting Roles..."
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB \
    -Q "SELECT * FROM Roles" -o "$BACKUP_DIR/roles_backup.csv" -s "," -W -t 30 2>/dev/null && \
    echo "✅ Roles exported" || echo "⚠️  Roles export failed"

# Export Employees
echo "Exporting Employees..."
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB \
    -Q "SELECT * FROM Employees" -o "$BACKUP_DIR/employees_backup.csv" -s "," -W -t 30 2>/dev/null && \
    echo "✅ Employees exported" || echo "⚠️  Employees export failed"

# Export KPI Configuration
echo "Exporting KPI Configuration..."
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB \
    -Q "SELECT * FROM KpiAssignmentTables" -o "$BACKUP_DIR/kpi_assignment_tables_backup.csv" -s "," -W -t 30 2>/dev/null && \
    echo "✅ KPI Assignment Tables exported" || echo "⚠️  KPI Assignment Tables export failed"

sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB \
    -Q "SELECT * FROM KpiDefinitions" -o "$BACKUP_DIR/kpi_definitions_backup.csv" -s "," -W -t 30 2>/dev/null && \
    echo "✅ KPI Definitions exported" || echo "⚠️  KPI Definitions export failed"

# Export Khoan Periods
echo "Exporting Khoan Periods..."
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB \
    -Q "SELECT * FROM KhoanPeriods" -o "$BACKUP_DIR/khoan_periods_backup.csv" -s "," -W -t 30 2>/dev/null && \
    echo "✅ Khoan Periods exported" || echo "⚠️  Khoan Periods export failed"

echo ""
echo -e "${BLUE}📊 3. Data counts verification...${NC}"

# Count critical tables
echo "Verifying data counts..."
sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB \
    -Q "
    SELECT 'Units' as TableName, COUNT(*) as RecordCount FROM Units
    UNION ALL
    SELECT 'Roles', COUNT(*) FROM Roles
    UNION ALL
    SELECT 'Employees', COUNT(*) FROM Employees
    UNION ALL
    SELECT 'KpiAssignmentTables', COUNT(*) FROM KpiAssignmentTables
    UNION ALL
    SELECT 'KpiDefinitions', COUNT(*) FROM KpiDefinitions
    UNION ALL
    SELECT 'KhoanPeriods', COUNT(*) FROM KhoanPeriods
    " -W -t 30 2>/dev/null || echo "Count queries failed"

echo ""
echo -e "${BLUE}📋 4. Core data tables check...${NC}"

# Check if core data tables exist and have data
CORE_TABLES=("DP01" "EI01" "LN01" "GL01" "GL41" "DPDA" "LN03" "RR01")

for table in "${CORE_TABLES[@]}"; do
    echo "Checking $table..."
    COUNT=$(sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB \
        -Q "SELECT COUNT(*) FROM $table" -W -h -1 -t 10 2>/dev/null | tr -d ' \r\n')

    if [[ "$COUNT" =~ ^[0-9]+$ ]]; then
        echo "  $table: $COUNT records"
        if [ "$COUNT" -gt 0 ]; then
            # Export sample data for important tables
            if [ "$table" = "DP01" ] || [ "$table" = "EI01" ] || [ "$table" = "LN01" ]; then
                echo "  Exporting sample from $table..."
                sqlcmd -S localhost,1433 -U sa -P 'YourStrong@Password123' -C -d TinhKhoanDB \
                    -Q "SELECT TOP 100 * FROM $table" -o "$BACKUP_DIR/${table}_sample.csv" -s "," -W -t 30 2>/dev/null
            fi
        fi
    else
        echo "  $table: Count failed"
    fi
done

echo ""
echo -e "${GREEN}💾 === BACKUP COMPLETED ===${NC}"
echo "Backup location: $BACKUP_DIR"
echo ""
echo "Files created:"
ls -la "$BACKUP_DIR/" 2>/dev/null || echo "Backup directory check failed"
echo ""
echo -e "${BLUE}🔗 Ready for recovery:${NC}"
echo "  Run: ./docker_recovery_new_image.sh"
