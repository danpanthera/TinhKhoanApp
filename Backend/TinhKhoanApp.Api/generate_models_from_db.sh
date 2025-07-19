#!/bin/bash

# 🔄 AUTO-GENERATE 8 DATATABLES MODELS FROM DATABASE
# Tự động tạo models từ database structure
# Created: 2025-07-19

echo "🔄 AUTO-GENERATING 8 DATATABLES MODELS FROM DATABASE..."
echo "======================================================="
echo ""

# Database connection info
DB_SERVER="localhost,1433"
DB_NAME="TinhKhoanDB"
DB_USER="sa"
DB_PASS="Dientoan@303"

# Function to generate model for a table
generate_model() {
    local table_name=$1
    local is_temporal=$2

    echo "📊 Generating model for $table_name..."

    # Get column information from database
    sqlcmd -S $DB_SERVER -U $DB_USER -P "$DB_PASS" -d $DB_NAME -C -Q "
        SELECT
            COLUMN_NAME,
            DATA_TYPE,
            CHARACTER_MAXIMUM_LENGTH,
            NUMERIC_PRECISION,
            NUMERIC_SCALE,
            IS_NULLABLE,
            ORDINAL_POSITION
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = '$table_name'
        ORDER BY ORDINAL_POSITION
    " -h -1 -s "," > "temp_${table_name}_columns.csv"

    # Generate C# model file
    cat > "Models/DataTables/${table_name}_Generated.cs" << 'EOF'
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng ${TABLE_NAME} - Auto-generated from database structure
    /// Generated: $(date '+%Y-%m-%d %H:%M:%S')
    /// ${TEMPORAL_INFO}
    /// </summary>
    [Table("${TABLE_NAME}")]
    public class ${TABLE_NAME}
    {
EOF

    # Process columns and generate properties
    while IFS=',' read -r col_name data_type max_len precision scale is_nullable ordinal; do
        # Skip empty lines
        [[ -z "$col_name" ]] && continue

        # Clean up column name
        col_name=$(echo "$col_name" | tr -d ' ')
        data_type=$(echo "$data_type" | tr -d ' ')
        is_nullable=$(echo "$is_nullable" | tr -d ' ')

        # Skip if column name is empty or invalid
        [[ -z "$col_name" || "$col_name" == "COLUMN_NAME" ]] && continue

        echo "        // Column: $col_name, Type: $data_type" >> "Models/DataTables/${table_name}_Generated.cs"

        # Determine C# type
        csharp_type=""
        nullable_suffix=""

        if [[ "$is_nullable" == "YES" ]]; then
            nullable_suffix="?"
        fi

        case "$data_type" in
            "bigint")
                csharp_type="long"
                ;;
            "int")
                csharp_type="int"
                ;;
            "decimal"|"numeric")
                csharp_type="decimal"
                ;;
            "nvarchar"|"varchar"|"char"|"nchar")
                csharp_type="string"
                nullable_suffix="?"
                ;;
            "datetime2"|"datetime")
                csharp_type="DateTime"
                ;;
            "date")
                csharp_type="DateTime"
                ;;
            "bit")
                csharp_type="bool"
                ;;
            *)
                csharp_type="string"
                nullable_suffix="?"
                ;;
        esac

        # Add Key attribute for Id column
        if [[ "$col_name" == "Id" ]]; then
            echo "        [Key]" >> "Models/DataTables/${table_name}_Generated.cs"
        fi

        # Add Column attribute
        echo "        [Column(\"$col_name\")]" >> "Models/DataTables/${table_name}_Generated.cs"

        # Add StringLength for string types
        if [[ "$data_type" == "nvarchar" || "$data_type" == "varchar" ]] && [[ "$max_len" =~ ^[0-9]+$ ]] && [[ "$max_len" != "-1" ]]; then
            echo "        [StringLength($max_len)]" >> "Models/DataTables/${table_name}_Generated.cs"
        fi

        # Generate property
        echo "        public ${csharp_type}${nullable_suffix} ${col_name} { get; set; }" >> "Models/DataTables/${table_name}_Generated.cs"
        echo "" >> "Models/DataTables/${table_name}_Generated.cs"

    done < "temp_${table_name}_columns.csv"

    # Close class
    echo "    }" >> "Models/DataTables/${table_name}_Generated.cs"
    echo "}" >> "Models/DataTables/${table_name}_Generated.cs"

    # Replace placeholders
    sed -i '' "s/\${TABLE_NAME}/$table_name/g" "Models/DataTables/${table_name}_Generated.cs"

    if [[ "$is_temporal" == "true" ]]; then
        sed -i '' "s/\${TEMPORAL_INFO}/Temporal Table with History tracking/g" "Models/DataTables/${table_name}_Generated.cs"
    else
        sed -i '' "s/\${TEMPORAL_INFO}/Partitioned Table by NGAY_DL/g" "Models/DataTables/${table_name}_Generated.cs"
    fi

    # Clean up temp file
    rm -f "temp_${table_name}_columns.csv"

    echo "   ✅ Generated: Models/DataTables/${table_name}_Generated.cs"
}

# Generate models for all 8 tables
echo "📋 Generating models for all 8 DataTables..."
echo ""

generate_model "GL01" "false"
generate_model "DP01" "true"
generate_model "DPDA" "true"
generate_model "EI01" "true"
generate_model "GL41" "true"
generate_model "LN01" "true"
generate_model "LN03" "true"
generate_model "RR01" "true"

echo ""
echo "✅ ALL 8 MODELS GENERATED SUCCESSFULLY!"
echo "======================================"
echo ""
echo "📁 Generated files:"
ls -la Models/DataTables/*_Generated.cs
echo ""
echo "🎯 Next steps:"
echo "1. Review generated models"
echo "2. Replace old models with generated ones"
echo "3. Update DbContext if needed"
echo "4. Test compilation"
echo ""
