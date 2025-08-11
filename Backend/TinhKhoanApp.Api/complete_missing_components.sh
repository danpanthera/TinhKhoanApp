#!/bin/bash

echo "🔧 ==============================================="
echo "   HOÀN THIỆN CÁC THÀNH PHẦN CÒN THIẾU         "
echo "==============================================="
echo ""

echo "📋 1. KIỂM TRA SERVICE INTERFACES:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

declare -a tables=("DPDA" "EI01" "GL01" "GL02" "GL41" "LN01" "LN03" "RR01")

for table in "${tables[@]}"; do
    if [ -f "Services/Interfaces/I${table}Service.cs" ]; then
        echo "✅ I${table}Service.cs - EXISTS"
    else
        echo "❌ I${table}Service.cs - MISSING"
    fi
done
echo ""

echo "📋 2. KIỂM TRA REPOSITORY INTERFACES:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

for table in "${tables[@]}"; do
    if [ -f "Repositories/I${table}Repository.cs" ]; then
        echo "✅ I${table}Repository.cs - EXISTS"
    else
        echo "❌ I${table}Repository.cs - MISSING"
    fi
done
echo ""

echo "📋 3. KIỂM TRA CONTROLLERS:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

for table in "${tables[@]}"; do
    if [ -f "Controllers/${table}Controller.cs" ]; then
        # Check if file is not empty
        if [ -s "Controllers/${table}Controller.cs" ]; then
            echo "✅ ${table}Controller.cs - EXISTS ($(wc -l < Controllers/${table}Controller.cs) lines)"
        else
            echo "⚠️  ${table}Controller.cs - EXISTS but EMPTY"
        fi
    else
        echo "❌ ${table}Controller.cs - MISSING"
    fi
done
echo ""

echo "📋 4. KIỂM TRA DTOs STRUCTURE:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

for table in "${tables[@]}"; do
    if [ -d "Models/DTOs/${table}" ]; then
        echo "✅ Models/DTOs/${table}/ - EXISTS"
        # List DTO files
        dto_files=$(find "Models/DTOs/${table}" -name "*.cs" | wc -l)
        echo "   └── DTO Files: ${dto_files}"

        # Check for standard DTOs (Preview, Details, Create, Update, ImportResult, Summary)
        standard_dtos=("Preview" "Details" "Create" "Update" "ImportResult" "Summary")
        for dto in "${standard_dtos[@]}"; do
            if grep -q "${table}${dto}Dto" "Models/DTOs/${table}"/*.cs 2>/dev/null; then
                echo "   ├── ${table}${dto}Dto ✅"
            else
                echo "   ├── ${table}${dto}Dto ❌"
            fi
        done
    else
        echo "❌ Models/DTOs/${table}/ - MISSING"
    fi
    echo ""
done

echo "📋 5. KIỂM TRA PROGRAM.CS REGISTRATIONS:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

if [ -f "Program.cs" ]; then
    echo "Checking service registrations in Program.cs..."
    echo ""

    for table in "${tables[@]}"; do
        # Check service registration
        if grep -q "I${table}Service.*${table}Service" Program.cs; then
            if grep -q "^// builder.Services.AddScoped.*I${table}Service" Program.cs || grep -q "^//.*I${table}Service" Program.cs; then
                echo "⚠️  I${table}Service - COMMENTED OUT in Program.cs"
            else
                echo "✅ I${table}Service - REGISTERED in Program.cs"
            fi
        else
            echo "❌ I${table}Service - NOT REGISTERED in Program.cs"
        fi

        # Check repository registration
        if grep -q "I${table}Repository.*${table}Repository" Program.cs; then
            if grep -q "^// builder.Services.AddScoped.*I${table}Repository" Program.cs || grep -q "^//.*I${table}Repository" Program.cs; then
                echo "⚠️  I${table}Repository - COMMENTED OUT in Program.cs"
            else
                echo "✅ I${table}Repository - REGISTERED in Program.cs"
            fi
        else
            echo "❌ I${table}Repository - NOT REGISTERED in Program.cs"
        fi
        echo ""
    done
else
    echo "❌ Program.cs not found"
fi

echo "🎯 SUMMARY:"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

# Count completeness
service_interfaces=$(find Services/Interfaces -name "I*Service.cs" | grep -E "DPDA|EI01|GL01|GL02|GL41|LN01|LN03|RR01" | wc -l)
repository_interfaces=$(find Repositories -name "I*Repository.cs" | grep -E "DPDA|EI01|GL01|GL02|GL41|LN01|LN03|RR01" | wc -l)
controllers=$(find Controllers -name "*Controller.cs" | grep -E "DPDA|EI01|GL01|GL02|GL41|LN01|LN03|RR01" | wc -l)
dto_folders=$(find Models/DTOs -maxdepth 1 -type d | grep -E "DPDA|EI01|GL01|GL02|GL41|LN01|LN03|RR01" | wc -l)

echo "Service Interfaces: ${service_interfaces}/8"
echo "Repository Interfaces: ${repository_interfaces}/8"
echo "Controllers: ${controllers}/8"
echo "DTO Folders: ${dto_folders}/8"
echo ""

# Calculate completion percentage
total_components=$((service_interfaces + repository_interfaces + controllers + dto_folders))
max_components=32  # 8 tables x 4 components each

completion_percentage=$(($total_components * 100 / $max_components))
echo "📊 Overall Completion: ${completion_percentage}% (${total_components}/${max_components} components)"

if [ $completion_percentage -ge 90 ]; then
    echo "🎉 EXCELLENT: System is almost complete!"
elif [ $completion_percentage -ge 70 ]; then
    echo "👍 GOOD: Most components are in place"
elif [ $completion_percentage -ge 50 ]; then
    echo "⚠️  MODERATE: Significant work remaining"
else
    echo "🚧 NEEDS WORK: Major components missing"
fi

echo "==============================================="
