#!/bin/bash

# Script regenerate models theo đúng cấu trúc database
# Business columns first, system/temporal columns last

echo "🔧 REGENERATING MODELS TO MATCH DATABASE STRUCTURE"
echo "=================================================="

# Kiểm tra database structure cho từng bảng
echo "📊 Analyzing database structure..."

for table in DP01 EI01 GL01 GL41 LN01 LN03 RR01 DPDA; do
    echo "🔍 Processing table: $table"

    # Tạo file header cho model
    model_file="Models/DataTables/${table}.cs"
    backup_file="Models/DataTables/${table}.cs.backup.$(date +%Y%m%d_%H%M%S)"

    # Backup file cũ
    if [ -f "$model_file" ]; then
        cp "$model_file" "$backup_file"
        echo "   📋 Backed up to: $backup_file"
    fi

    echo "   🔍 Getting database column structure..."

    # Get columns from database theo đúng thứ tự
    sqlcmd -S localhost,1433 -U sa -P "YourStrong@Password123" -d TinhKhoanDB -C -h -1 -s '|' -W -Q "
        SELECT
            COLUMN_NAME,
            DATA_TYPE,
            ISNULL(CAST(CHARACTER_MAXIMUM_LENGTH AS VARCHAR), '0') as MAX_LENGTH,
            CASE WHEN IS_NULLABLE = 'YES' THEN '1' ELSE '0' END as IS_NULLABLE,
            CAST(ORDINAL_POSITION AS VARCHAR) as ORDINAL_POSITION
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = '$table'
        ORDER BY ORDINAL_POSITION
    " > "/tmp/${table}_columns.tsv" 2>/dev/null

    if [ $? -eq 0 ]; then
        echo "   ✅ Database structure extracted for $table"
    else
        echo "   ❌ Failed to extract structure for $table"
        continue
    fi

    # Generate model từ database structure
    cat > "$model_file" << EOF
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinhKhoanApp.Api.Models.DataTables
{
    /// <summary>
    /// Bảng $table - Generated from database structure
    /// Business columns first, system/temporal columns last
    /// Synchronized with database schema
    /// </summary>
    [Table("$table")]
    public class $table
    {
EOF

    # Process columns từ database
    echo "   🔨 Generating model properties..."

    while IFS='|' read -r col_name data_type max_length is_nullable ordinal_position; do
        # Skip header và empty lines
        if [[ "$col_name" == "COLUMN_NAME" ]] || [[ -z "$col_name" ]] || [[ "$col_name" =~ ^[[:space:]]*$ ]]; then
            continue
        fi

        # Clean whitespace
        col_name=$(echo "$col_name" | xargs)
        data_type=$(echo "$data_type" | xargs)
        max_length=$(echo "$max_length" | xargs)
        is_nullable=$(echo "$is_nullable" | xargs)

        # Determine C# type
        case "$data_type" in
            "bigint")
                if [[ "$col_name" == "Id" ]]; then
                    csharp_type="long"
                    echo "        [Key]" >> "$model_file"
                else
                    csharp_type="long"
                fi
                ;;
            "int")
                csharp_type="int"
                ;;
            "datetime"|"datetime2")
                csharp_type="DateTime"
                ;;
            "decimal"|"numeric")
                csharp_type="decimal"
                ;;
            "nvarchar"|"varchar")
                csharp_type="string"
                ;;
            "bit")
                csharp_type="bool"
                ;;
            *)
                csharp_type="string"
                ;;
        esac

        # Add nullable if applicable
        if [[ "$is_nullable" == "1" ]] && [[ "$col_name" != "Id" ]]; then
            if [[ "$csharp_type" == "string" ]]; then
                csharp_type="string?"
            else
                csharp_type="${csharp_type}?"
            fi
        fi

        # Add Column attribute
        echo "        [Column(\"$col_name\")]" >> "$model_file"

        # Add StringLength for string columns
        if [[ "$data_type" == "nvarchar" ]] || [[ "$data_type" == "varchar" ]]; then
            if [[ "$max_length" -gt 0 ]] && [[ "$max_length" != "NULL" ]] && [[ "$max_length" != "0" ]]; then
                echo "        [StringLength($max_length)]" >> "$model_file"
            fi
        fi

        # Add property
        echo "        public $csharp_type $col_name { get; set; }" >> "$model_file"
        echo "" >> "$model_file"

    done < "/tmp/${table}_columns.tsv"

    # Close class
    echo "    }" >> "$model_file"
    echo "}" >> "$model_file"

    echo "   ✅ Model generated: $model_file"

    # Cleanup temp file
    rm -f "/tmp/${table}_columns.tsv"

done

echo ""
echo "🎯 MODEL REGENERATION COMPLETE!"
echo "================================="
echo "✅ All models regenerated to match database structure"
echo "✅ Business columns first, system/temporal columns last"
echo "✅ Backup files created with timestamp"
echo ""
echo "📋 Next steps:"
echo "1. Review generated models"
echo "2. Build project to verify"
echo "3. Test API endpoints"
