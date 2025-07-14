#!/bin/bash

# 🔧 Script enable Temporal Tables + Columnstore Indexes cho 8 core data tables
# Tác giả: TinhKhoanApp Team
# Ngày tạo: July 14, 2025

# Màu sắc cho output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

DB_PASSWORD="YourStrong@Password123"

echo -e "${BLUE}🔧 TinhKhoanApp - Enable Temporal Tables + Columnstore${NC}"
echo -e "${BLUE}====================================================${NC}"

# Kiểm tra sqlcmd có sẵn không
if ! command -v sqlcmd > /dev/null 2>&1; then
    echo -e "${RED}❌ Sqlcmd không có sẵn trên macOS${NC}"
    exit 1
fi

# Danh sách 8 bảng core data
CORE_TABLES=("DP01" "DPDA" "EI01" "GL01" "GL41" "LN01" "LN03" "RR01")

echo -e "${YELLOW}📋 Processing 8 core data tables...${NC}"

for table in "${CORE_TABLES[@]}"; do
    echo -e "${YELLOW}🔧 Processing table: ${table}${NC}"

    # 1. Enable Temporal Tables
    echo -e "${YELLOW}  ⏰ Enabling Temporal Tables for ${table}...${NC}"
    sqlcmd -S localhost,1433 -U sa -P "${DB_PASSWORD}" -C -d TinhKhoanDB -Q "
    ALTER TABLE ${table}
    ADD PERIOD FOR SYSTEM_TIME (CREATED_DATE, UPDATED_DATE);

    ALTER TABLE ${table}
    SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.${table}_History));
    " > /dev/null 2>&1

    if [ $? -eq 0 ]; then
        echo -e "${GREEN}  ✅ Temporal Tables enabled for ${table}${NC}"
    else
        echo -e "${RED}  ❌ Failed to enable Temporal Tables for ${table}${NC}"
    fi

    # 2. Create Columnstore Index
    echo -e "${YELLOW}  📊 Creating Columnstore Index for ${table}...${NC}"
    sqlcmd -S localhost,1433 -U sa -P "${DB_PASSWORD}" -C -d TinhKhoanDB -Q "
    CREATE NONCLUSTERED COLUMNSTORE INDEX IX_${table}_Columnstore
    ON ${table} (Id, NGAY_DL, CREATED_DATE, UPDATED_DATE);
    " > /dev/null 2>&1

    if [ $? -eq 0 ]; then
        echo -e "${GREEN}  ✅ Columnstore Index created for ${table}${NC}"
    else
        echo -e "${RED}  ❌ Failed to create Columnstore Index for ${table}${NC}"
    fi

    echo -e "${BLUE}  ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
done

echo -e "${YELLOW}🔍 Verifying results...${NC}"

# Verify Temporal Tables
echo -e "${YELLOW}📋 Temporal Tables status:${NC}"
sqlcmd -S localhost,1433 -U sa -P "${DB_PASSWORD}" -C -d TinhKhoanDB -Q "
SELECT
    t.name as TableName,
    CASE WHEN t.temporal_type = 2 THEN 'System-Versioned'
         WHEN t.temporal_type = 1 THEN 'History Table'
         ELSE 'Regular Table' END as TemporalType,
    h.name as HistoryTable
FROM sys.tables t
LEFT JOIN sys.tables h ON t.history_table_id = h.object_id
WHERE t.name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
ORDER BY t.name
" -h -1

# Verify Columnstore Indexes
echo -e "${YELLOW}📊 Columnstore Indexes status:${NC}"
sqlcmd -S localhost,1433 -U sa -P "${DB_PASSWORD}" -C -d TinhKhoanDB -Q "
SELECT
    t.name as TableName,
    i.name as IndexName,
    i.type_desc as IndexType
FROM sys.tables t
INNER JOIN sys.indexes i ON t.object_id = i.object_id
WHERE t.name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL41', 'LN01', 'LN03', 'RR01')
  AND i.type IN (5, 6) -- Columnstore indexes
ORDER BY t.name, i.name
" -h -1

echo -e "${BLUE}====================================================${NC}"
echo -e "${GREEN}🎯 Temporal Tables + Columnstore setup completed!${NC}"
echo -e "${YELLOW}💡 Benefits:${NC}"
echo -e "${BLUE}   • Temporal Tables: Full audit trail + history tracking${NC}"
echo -e "${BLUE}   • Columnstore: 10-100x faster analytics queries${NC}"
echo -e "${BLUE}   • History Tables: Automatic backup on every change${NC}"
