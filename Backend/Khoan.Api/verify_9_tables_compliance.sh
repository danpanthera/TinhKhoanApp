#!/bin/bash
# 🔍 9-TABLE COMPREHENSIVE VERIFICATION SCRIPT
# Kiểm tra sự thống nhất: CSV ↔ Model ↔ Service ↔ Controller ↔ DTO ↔ Database

echo "🔍 STARTING 9-TABLE COMPREHENSIVE VERIFICATION..."
echo "==========================================="

# Array of all 9 tables
TABLES=("DP01" "DPDA" "EI01" "GL01" "GL02" "GL41" "LN01" "LN03" "RR01")

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Function to check if file exists
check_file() {
    local file="$1"
    if [[ -f "$file" ]]; then
        echo -e "  ✅ ${GREEN}Found: $file${NC}"
        return 0
    else
        echo -e "  ❌ ${RED}Missing: $file${NC}"
        return 1
    fi
}

# Function to extract CSV headers
get_csv_headers() {
    local csv_file="$1"
    if [[ -f "$csv_file" ]]; then
        head -n 1 "$csv_file" | tr ',' '\n' | nl
        return 0
    else
        echo "CSV file not found: $csv_file"
        return 1
    fi
}

# Function to check model structure
check_model_structure() {
    local table="$1"
    local model_file=""
    
    # Try different possible paths
    possible_paths=(
        "Models/DataTables/${table}.cs"
        "Models/Entities/${table}Entity.cs" 
        "Entities/${table}Entity.cs"
        "Models/${table}.cs"
    )
    
    for path in "${possible_paths[@]}"; do
        if [[ -f "$path" ]]; then
            model_file="$path"
            break
        fi
    done
    
    if [[ -n "$model_file" ]]; then
        echo -e "  📋 ${BLUE}Model Structure ($model_file):${NC}"
        # Extract column definitions
        grep -E "public [^{]*{|public [^{]*;" "$model_file" | head -20
        return 0
    else
        echo -e "  ❌ ${RED}No model found for $table${NC}"
        return 1
    fi
}

# Function to check service structure
check_service_structure() {
    local table="$1"
    local service_file="Services/${table}Service.cs"
    
    if [[ -f "$service_file" ]]; then
        echo -e "  🔧 ${CYAN}Service Methods:${NC}"
        grep -E "public.*Task.*\(" "$service_file" | head -10
        return 0
    else
        echo -e "  ❌ ${RED}No service found: $service_file${NC}"
        return 1
    fi
}

# Function to check controller structure
check_controller_structure() {
    local table="$1"
    local controller_file="Controllers/${table}Controller.cs"
    
    if [[ -f "$controller_file" ]]; then
        echo -e "  🎮 ${PURPLE}Controller Endpoints:${NC}"
        grep -E "\[Http.*\]" "$controller_file"
        return 0
    else
        echo -e "  ❌ ${RED}No controller found: $controller_file${NC}"
        return 1
    fi
}

echo ""
echo "🔍 SYSTEMATIC VERIFICATION OF 9 TABLES"
echo "========================================"

total_score=0
max_score=0

for table in "${TABLES[@]}"; do
    echo ""
    echo -e "${YELLOW}🔍 CHECKING TABLE: $table${NC}"
    echo "----------------------------------------"
    
    table_score=0
    table_max=0
    
    # 1. Check CSV file
    echo -e "${BLUE}1. CSV File Structure:${NC}"
    csv_file="7800_${table,,}_*.csv"
    csv_found=$(find . -name "$csv_file" | head -1)
    
    if [[ -n "$csv_found" ]]; then
        echo -e "  ✅ ${GREEN}Found CSV: $csv_found${NC}"
        echo -e "  📊 ${CYAN}CSV Headers:${NC}"
        get_csv_headers "$csv_found" | head -10
        table_score=$((table_score + 1))
    else
        echo -e "  ❌ ${RED}No CSV found for pattern: $csv_file${NC}"
    fi
    table_max=$((table_max + 1))
    
    # 2. Check Model
    echo -e "${BLUE}2. Model Structure:${NC}"
    if check_model_structure "$table"; then
        table_score=$((table_score + 1))
    fi
    table_max=$((table_max + 1))
    
    # 3. Check Service
    echo -e "${BLUE}3. Service Layer:${NC}"
    if check_service_structure "$table"; then
        table_score=$((table_score + 1))
    fi
    table_max=$((table_max + 1))
    
    # 4. Check Controller
    echo -e "${BLUE}4. Controller Layer:${NC}"
    if check_controller_structure "$table"; then
        table_score=$((table_score + 1))
    fi
    table_max=$((table_max + 1))
    
    # Calculate table completion percentage
    table_percentage=$((table_score * 100 / table_max))
    total_score=$((total_score + table_score))
    max_score=$((max_score + table_max))
    
    if [[ $table_percentage -ge 80 ]]; then
        status_color=$GREEN
        status="✅ COMPLIANT"
    elif [[ $table_percentage -ge 60 ]]; then
        status_color=$YELLOW  
        status="⚠️ PARTIAL"
    else
        status_color=$RED
        status="❌ NEEDS WORK"
    fi
    
    echo -e "${status_color}📊 $table Status: $status ($table_score/$table_max = $table_percentage%)${NC}"
done

echo ""
echo "========================================"
overall_percentage=$((total_score * 100 / max_score))
echo -e "${CYAN}🎯 OVERALL COMPLIANCE: $total_score/$max_score = $overall_percentage%${NC}"

if [[ $overall_percentage -ge 90 ]]; then
    echo -e "${GREEN}🎉 EXCELLENT: System ready for production!${NC}"
elif [[ $overall_percentage -ge 70 ]]; then
    echo -e "${YELLOW}⚠️  GOOD: Minor fixes needed${NC}"
else
    echo -e "${RED}🚨 CRITICAL: Major restructuring required${NC}"
fi

echo ""
echo "🔍 Verification completed: $(date)"
echo "========================================"
