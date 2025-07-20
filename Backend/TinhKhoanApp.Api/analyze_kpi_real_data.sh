#!/bin/bash

# Script phân tích dữ liệu KPI thực tế từ database
# Tạo bởi GitHub Copilot - July 20, 2025

echo "🔍 PHÂN TÍCH DỮ LIỆU KPI THỰC TẾ"
echo "================================"

# Kiểm tra backend
if ! curl -s "http://localhost:5055/api/health" > /dev/null; then
    echo "❌ Backend chưa chạy! Hãy chạy ./start_backend.sh trước"
    exit 1
fi

echo "✅ Backend đang chạy"
echo ""

# Tạo temporary Python script để phân tích
cat > /tmp/analyze_kpi.py << 'EOF'
import requests
import json

def get_kpi_data():
    try:
        # Lấy danh sách KPI Assignment Tables
        response = requests.get("http://localhost:5055/api/kpi/assignment-tables")
        if response.status_code != 200:
            print(f"❌ Lỗi API: {response.status_code}")
            return None

        tables = response.json()
        print(f"📊 Tổng số KPI Assignment Tables: {len(tables)}")
        print("=" * 60)

        # Nhóm theo category
        by_category = {}
        total_indicators = 0

        for table in tables:
            category = table.get('category', 'Unknown')
            table_name = table.get('tableName', 'Unknown')

            # Lấy indicators cho từng table
            try:
                indicators_response = requests.get(f"http://localhost:5055/api/kpi/assignment-tables/{table['id']}/indicators")
                indicators_count = len(indicators_response.json()) if indicators_response.status_code == 200 else 0
            except:
                indicators_count = 0

            if category not in by_category:
                by_category[category] = []

            by_category[category].append({
                'name': table_name,
                'indicators': indicators_count
            })
            total_indicators += indicators_count

        # In kết quả
        for category, tables_in_category in by_category.items():
            category_total = sum(t['indicators'] for t in tables_in_category)
            print(f"\n🏷️  {category}: {len(tables_in_category)} bảng - {category_total} chỉ tiêu")
            print("-" * 50)

            for table in tables_in_category:
                print(f"   📋 {table['name']}: {table['indicators']} chỉ tiêu")

        print(f"\n🔢 TỔNG KẾT:")
        print(f"   📊 Tổng số bảng: {len(tables)}")
        print(f"   📈 Tổng số chỉ tiêu: {total_indicators}")

        # Phân tích theo yêu cầu
        branch_tables = by_category.get('Chi nhánh', [])
        employee_tables = by_category.get('Cán bộ', [])

        print(f"\n📋 PHÂN TÍCH THEO YÊU CẦU:")
        print(f"   🏢 Chi nhánh: {len(branch_tables)} bảng")
        print(f"   👥 Cán bộ: {len(employee_tables)} bảng")

        if len(branch_tables) != 9:
            print(f"   ⚠️  Yêu cầu: 9 bảng Chi nhánh, hiện tại: {len(branch_tables)}")
        if len(employee_tables) != 23:
            print(f"   ⚠️  Yêu cầu: 23 bảng Cán bộ, hiện tại: {len(employee_tables)}")

        return by_category

    except Exception as e:
        print(f"❌ Lỗi: {e}")
        return None

if __name__ == "__main__":
    get_kpi_data()
EOF

# Chạy phân tích
python3 /tmp/analyze_kpi.py

# Dọn dẹp
rm -f /tmp/analyze_kpi.py

echo ""
echo "✅ Hoàn thành phân tích!"
