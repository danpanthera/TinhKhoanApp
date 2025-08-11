#!/bin/bash
# SCRIPT: services_repository_verification.sh
# MỤC ĐÍCH: Kiểm tra Services/Repository sử dụng đúng column names cho 9 bảng

echo "🔧 SERVICES/REPOSITORY VERIFICATION - 9 TABLES"
echo "==============================================="

BACKEND_DIR="/Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api"
cd "$BACKEND_DIR" || exit 1

# Function kiểm tra Services
check_service() {
    local table=$1
    echo "🔧 CHECKING SERVICE: ${table}Service"
    echo "=========================="

    local service_file="Services/${table}Service.cs"
    local interface_file="Services/I${table}Service.cs"

    if [ -f "$service_file" ]; then
        echo "✅ Service file exists: $service_file"

        # Kiểm tra column references trong service
        echo "📋 Column references in service:"
        grep -n -E "\b(MA_CN|BRCD|MA_KH|TEN_KH|CCY|NGAY_DL)\b" "$service_file" 2>/dev/null | head -5

        # Kiểm tra DTO mapping
        echo "📋 DTO mapping patterns:"
        grep -n -E "(MapTo|Select|new.*{)" "$service_file" 2>/dev/null | head -3

    else
        echo "❌ Service file missing: $service_file"
    fi

    if [ -f "$interface_file" ]; then
        echo "✅ Interface exists: $interface_file"
    else
        echo "❌ Interface missing: $interface_file"
    fi

    echo ""
}

# Function kiểm tra Repository
check_repository() {
    local table=$1
    echo "🗄️ CHECKING REPOSITORY: ${table}Repository"
    echo "============================"

    local repo_file="Repositories/${table}Repository.cs"
    local interface_file="Repositories/I${table}Repository.cs"

    if [ -f "$repo_file" ]; then
        echo "✅ Repository file exists: $repo_file"

        # Kiểm tra Entity usage
        echo "📋 Entity references:"
        grep -n -E "(DataTables\.${table}|${table}\s+\w+|new ${table})" "$repo_file" 2>/dev/null | head -3

        # Kiểm tra query patterns
        echo "📋 Query patterns:"
        grep -n -E "(Where|Select|OrderBy|Include)" "$repo_file" 2>/dev/null | head -3

    else
        echo "❌ Repository file missing: $repo_file"
    fi

    if [ -f "$interface_file" ]; then
        echo "✅ Interface exists: $interface_file"
    else
        echo "❌ Interface missing: $interface_file"
    fi

    echo ""
}

# Function kiểm tra DTO
check_dto() {
    local table=$1
    echo "📋 CHECKING DTO: ${table}Dto"
    echo "======================"

    # Tìm DTO files
    local dto_files=$(find DTOs -name "*${table}*" 2>/dev/null)

    if [ -n "$dto_files" ]; then
        echo "✅ DTO files found:"
        echo "$dto_files"

        for dto_file in $dto_files; do
            echo "📋 Properties in $dto_file:"
            grep -n -E "public.*{.*get.*set" "$dto_file" 2>/dev/null | head -5
        done
    else
        echo "❌ No DTO files found for $table"
    fi

    echo ""
}

# Function kiểm tra Controller
check_controller() {
    local table=$1
    echo "🎮 CHECKING CONTROLLER: ${table}Controller"
    echo "============================="

    local controller_file="Controllers/${table}Controller.cs"

    if [ -f "$controller_file" ]; then
        echo "✅ Controller file exists: $controller_file"

        # Kiểm tra API endpoints
        echo "📋 API endpoints:"
        grep -n -E "\[Http(Get|Post|Put|Delete)\]" "$controller_file" 2>/dev/null | head -5

        # Kiểm tra service injection
        echo "📋 Service dependency:"
        grep -n -E "_.*Service|I.*Service" "$controller_file" 2>/dev/null | head -3

    else
        echo "❌ Controller file missing: $controller_file"
    fi

    echo ""
}

# Function tổng hợp verification cho 1 bảng
verify_table_services() {
    local table=$1
    echo "🎯 COMPLETE VERIFICATION: $table"
    echo "================================="

    check_service "$table"
    check_repository "$table"
    check_dto "$table"
    check_controller "$table"

    echo "-------------------------------------------"
    echo ""
}

# Main execution
echo "📅 Date: $(date)"
echo "📍 Location: $BACKEND_DIR"
echo ""

# Kiểm tra tổng quan structure
echo "📊 OVERALL ARCHITECTURE STATUS"
echo "=============================="

echo "🔧 Services folder structure:"
ls -la Services/ | grep -E "\.(cs)$" | wc -l | xargs echo "   • Service files:"

echo "🗄️ Repositories folder structure:"
ls -la Repositories/ | grep -E "\.(cs)$" | wc -l | xargs echo "   • Repository files:"

echo "📋 DTOs folder structure:"
ls -la DTOs/ | grep -E "\.(cs)$" | wc -l | xargs echo "   • DTO files:"

echo "🎮 Controllers folder structure:"
ls -la Controllers/ | grep -E "\.(cs)$" | wc -l | xargs echo "   • Controller files:"

echo ""
echo "📊 TABLE-BY-TABLE VERIFICATION"
echo "=============================="

# Verify từng bảng
for table in "DP01" "DPDA" "EI01" "GL01" "GL02" "GL41" "LN01" "LN03" "RR01"; do
    verify_table_services "$table"
done

echo "✅ COMPLETED: Services/Repository Verification"
echo ""
echo "📋 SUMMARY ANALYSIS:"
echo "   • Check for missing Service/Repository/DTO files"
echo "   • Verify column name consistency across layers"
echo "   • Ensure proper Entity Framework usage"
echo "   • Validate API endpoint patterns"
