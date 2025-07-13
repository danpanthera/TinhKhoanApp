#!/bin/bash

echo "🔍 KIỂM TRA 8 BẢNG CORE DATA TABLES"
echo "=================================="

# Đọc SQL file và thực thi
SQL_QUERY=$(cat check_8_core_tables.sql)

# Gọi API để thực thi SQL
curl -s -X POST "http://localhost:5055/api/TestData/execute-sql" \
-H "Content-Type: application/json" \
-d "{\"query\": \"$SQL_QUERY\"}" | jq -r '
if type == "array" then
  . as $data |
  "TableName  | Status     | Business Columns | Expected",
  "-----------|------------|------------------|----------",
  ($data[] | "\(.TableName | ljust(10)) | \(.Status | ljust(10)) | \(.BusinessColumns | tostring | rjust(16)) | \(
    if .TableName == "DP01" then "63"
    elif .TableName == "LN01" then "79"
    elif .TableName == "LN03" then "17"
    elif .TableName == "GL01" then "27"
    elif .TableName == "GL41" then "13"
    elif .TableName == "DPDA" then "13"
    elif .TableName == "EI01" then "24"
    elif .TableName == "RR01" then "25"
    else "??"
    end
  )")
else
  "❌ Error: " + (.Error // .Message // "Unknown error")
end'

echo ""
echo "✅ Kết quả: So sánh Business Columns với Expected columns"
echo "📊 Expected columns theo README: DP01(63), LN01(79), LN03(17), GL01(27), GL41(13), DPDA(13), EI01(24), RR01(25)"
