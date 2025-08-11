#!/bin/bash
# SCRIPT: database_structure_verification.sh
# MỤC ĐÍCH: Kiểm tra Database structure alignment với Models cho 9 bảng

echo "🗄️  DATABASE STRUCTURE VERIFICATION - 9 TABLES"
echo "=============================================="

BACKEND_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api"
cd "$BACKEND_DIR" || exit 1

# Function kiểm tra database structure cho từng bảng
verify_database_table() {
    local table=$1
    echo "🗄️  VERIFYING DATABASE: $table"
    echo "=========================="

    # Kiểm tra table existence
    echo "📋 Checking table existence..."
    sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "
    SELECT
        TABLE_NAME,
        TABLE_TYPE
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_NAME = '$table';" 2>/dev/null

    if [ $? -eq 0 ]; then
        echo "✅ Table $table exists in database"
    else
        echo "❌ Table $table does not exist in database"
        return 1
    fi

    # Kiểm tra column structure
    echo ""
    echo "📊 Column structure analysis:"
    sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "
    SELECT
        ORDINAL_POSITION as Pos,
        COLUMN_NAME as Name,
        DATA_TYPE as Type,
        CHARACTER_MAXIMUM_LENGTH as MaxLen,
        IS_NULLABLE as Nullable
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = '$table'
    ORDER BY ORDINAL_POSITION;" 2>/dev/null

    # Đếm business columns (exclude system columns)
    echo ""
    echo "🔢 Column counts:"
    total_cols=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "
    SELECT COUNT(*)
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = '$table';" -h -1 2>/dev/null | tr -d ' ')

    business_cols=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "
    SELECT COUNT(*)
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = '$table'
    AND COLUMN_NAME NOT IN ('Id', 'NGAY_DL', 'CreatedAt', 'UpdatedAt', 'DataSource', 'ImportDateTime', 'CreatedBy', 'UpdatedBy', 'ValidFrom', 'ValidTo', 'FILE_NAME', 'CREATED_DATE', 'UPDATED_DATE', 'IMPORT_BATCH_ID', 'DATA_SOURCE', 'PROCESSING_STATUS', 'ERROR_MESSAGE', 'ROW_HASH', 'BATCH_ID', 'IMPORT_SESSION_ID', 'CREATED_BY', 'FILE_ORIGIN');" -h -1 2>/dev/null | tr -d ' ')

    echo "📊 Total columns in DB: $total_cols"
    echo "📊 Business columns in DB: $business_cols"

    # Kiểm tra temporal table
    echo ""
    echo "⏰ Temporal table check:"
    temporal_check=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "
    SELECT
        CASE WHEN temporal_type = 2 THEN 'SYSTEM_VERSIONED_TEMPORAL_TABLE'
             ELSE 'REGULAR_TABLE'
        END as TableType
    FROM sys.tables
    WHERE name = '$table';" -h -1 2>/dev/null | tr -d ' ')

    echo "🗄️  Table type: $temporal_check"

    # Kiểm tra indexes
    echo ""
    echo "📇 Index information:"
    sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "
    SELECT
        i.name as IndexName,
        i.type_desc as IndexType,
        CASE WHEN i.is_primary_key = 1 THEN 'PRIMARY KEY'
             WHEN i.is_unique = 1 THEN 'UNIQUE'
             ELSE 'NON-UNIQUE'
        END as KeyType
    FROM sys.indexes i
    INNER JOIN sys.tables t ON i.object_id = t.object_id
    WHERE t.name = '$table'
    ORDER BY i.index_id;" 2>/dev/null

    echo ""
    echo "-------------------------------------------"
    echo ""
}

# Function tổng hợp verification
summarize_verification() {
    echo "📋 DATABASE VERIFICATION SUMMARY"
    echo "==============================="

    echo "🎯 Checking all 9 core tables..."
    for table in "DP01" "DPDA" "EI01" "GL01" "GL02" "GL41" "LN01" "LN03" "RR01"; do
        table_exists=$(sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "
        SELECT COUNT(*)
        FROM INFORMATION_SCHEMA.TABLES
        WHERE TABLE_NAME = '$table';" -h -1 2>/dev/null | tr -d ' ')

        if [ "$table_exists" -eq 1 ]; then
            echo "✅ $table - EXISTS"
        else
            echo "❌ $table - MISSING"
        fi
    done

    echo ""
    echo "🎯 Temporal table status:"
    sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -d TinhKhoanDB -Q "
    SELECT
        t.name as TableName,
        CASE WHEN t.temporal_type = 2 THEN 'TEMPORAL ✅'
             ELSE 'REGULAR ⚠️'
        END as Status
    FROM sys.tables t
    WHERE t.name IN ('DP01', 'DPDA', 'EI01', 'GL01', 'GL02', 'GL41', 'LN01', 'LN03', 'RR01')
    ORDER BY t.name;" 2>/dev/null
}

# Main execution
echo "📅 Date: $(date)"
echo "🔗 Database: TinhKhoanDB on localhost:1433"
echo ""

# Test connection first
echo "🔌 Testing database connection..."
sqlcmd -S localhost,1433 -U sa -P 'Dientoan@303' -C -Q "SELECT @@VERSION" -h -1 2>/dev/null >/dev/null
if [ $? -eq 0 ]; then
    echo "✅ Database connection successful"
else
    echo "❌ Database connection failed"
    exit 1
fi

echo ""

# Quick summary first
summarize_verification

echo ""
echo "📊 DETAILED TABLE VERIFICATION"
echo "=============================="

# Detailed verification for each table
for table in "DP01" "DPDA" "EI01" "GL01" "GL02" "GL41" "LN01" "LN03" "RR01"; do
    verify_database_table "$table"
done

echo "✅ COMPLETED: Database Structure Verification"
echo ""
echo "📋 NEXT STEPS:"
echo "   1. Review any column mismatches"
echo "   2. Check temporal table configurations"
echo "   3. Verify index optimization"
echo "   4. Test DirectImport operations"
