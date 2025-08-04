#!/bin/bash

# Script để xóa tất cả các script liên quan đến indicators, Units, Employee, KPI, Roles
# Clean all scripts related to indicators, Units, Employee, KPI, and Roles

echo "🧹 Starting cleanup of indicators, Units, Employee, KPI, and Roles scripts..."
echo "📅 Started at: $(date)"

# Get list of files to be deleted
echo "🔍 Finding files to delete..."

files_to_delete=(
    # Units related scripts
    "*unit*.sh"
    "*Unit*.sh"
    "*units*.sh"
    "*Units*.sh"
    "restructure_organizational_units.sh"
    "complete_restructure.sh"
    "create_departments.sh"

    # Employee related scripts
    "*employee*.sh"
    "*Employee*.sh"
    "*employees*.sh"
    "*Employees*.sh"

    # KPI related scripts
    "*kpi*.sh"
    "*Kpi*.sh"
    "*KPI*.sh"

    # Role related scripts
    "*role*.sh"
    "*Role*.sh"
    "*roles*.sh"
    "*Roles*.sh"

    # Indicator related scripts
    "*indicator*.sh"
    "*Indicator*.sh"
    "*indicators*.sh"
    "*Indicators*.sh"

    # SQL scripts
    "*unit*.sql"
    "*Unit*.sql"
    "*employee*.sql"
    "*Employee*.sql"
    "*kpi*.sql"
    "*Kpi*.sql"
    "*KPI*.sql"
    "*role*.sql"
    "*Role*.sql"
    "*indicator*.sql"
    "*Indicator*.sql"
)

# Count total files to delete
total_files=0
for pattern in "${files_to_delete[@]}"; do
    count=$(find . -maxdepth 1 -name "$pattern" | wc -l)
    total_files=$((total_files + count))
done

echo "🗑️ Found $total_files files to delete"

# Create backup directory
backup_dir="script_backup_$(date +%Y%m%d_%H%M%S)"
mkdir -p "$backup_dir"
echo "📦 Created backup directory: $backup_dir"

# Move files to backup directory instead of deleting
for pattern in "${files_to_delete[@]}"; do
    for file in $(find . -maxdepth 1 -name "$pattern"); do
        if [ "$file" != "./restructure_units_ver2.sh" ]; then
            echo "📋 Moving $file to backup directory"
            mv "$file" "$backup_dir/"
        else
            echo "🔒 Keeping $file (new unit restructuring script)"
        fi
    done
done

# Make new script executable
chmod +x restructure_units_ver2.sh

echo "✅ Cleanup completed!"
echo "🗄️ All files backed up to: $backup_dir"
echo "📅 Finished at: $(date)"
echo ""
echo "🚀 Next steps: Run ./restructure_units_ver2.sh to implement the new organizational structure"
