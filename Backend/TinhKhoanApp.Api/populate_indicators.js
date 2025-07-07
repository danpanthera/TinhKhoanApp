#!/usr/bin/env node

// 🚀 Script Node.js để populate indicators cho 9 bảng KPI chi nhánh

const baseUrl = 'http://localhost:5055/api';

// Danh sách chỉ tiêu giống bảng ID=17
const indicators = [
  { name: 'Tổng dư nợ BQ', maxScore: 30.00, unit: 'Triệu VND', order: 1 },
  { name: 'Tỷ lệ nợ xấu', maxScore: 15.00, unit: '%', order: 2 },
  { name: 'Phát triển Khách hàng', maxScore: 10.00, unit: 'Khách hàng', order: 3 },
  { name: 'Thu nợ đã XLRR', maxScore: 10.00, unit: 'Triệu VND', order: 4 },
  { name: 'Thực hiện nhiệm vụ theo chương trình công tác', maxScore: 10.00, unit: '%', order: 5 },
  { name: 'Chấp hành quy chế, quy trình nghiệp vụ, văn hóa Agribank', maxScore: 10.00, unit: '%', order: 6 },
  { name: 'Tổng nguồn vốn huy động BQ', maxScore: 10.00, unit: 'Triệu VND', order: 7 },
  { name: 'Hoàn thành chỉ tiêu giao khoán SPDV', maxScore: 5.00, unit: '%', order: 8 }
];

// Danh sách 9 bảng chi nhánh (ID 24-32)
const branchTableIds = [24, 25, 26, 27, 28, 29, 30, 31, 32];

async function populateIndicators() {
  console.log('🔧 Bắt đầu populate indicators cho 9 bảng KPI chi nhánh...');

  // Thử gọi API để kiểm tra
  try {
    const response = await fetch(`${baseUrl}/KpiAssignment/tables`);
    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${response.statusText}`);
    }
    console.log('✅ API connection OK');
  } catch (error) {
    console.error('❌ Không thể kết nối API:', error.message);
    return;
  }

  // Vì API không có POST indicators, ta sẽ tạo dữ liệu trực tiếp bằng SQL thông qua console log
  let sqlScript = `
-- 🏢 Script SQL tạo chỉ tiêu KPI cho 9 bảng chi nhánh
-- Dựa trên mẫu bảng "Phó phòng Khách hàng CNL2" (ID=17)

-- Xóa chỉ tiêu cũ của 9 bảng chi nhánh (ID 24-32)
DELETE FROM KpiIndicators WHERE KpiAssignmentTableId BETWEEN 24 AND 32;

`;

  // Tạo INSERT statements cho từng bảng
  for (const tableId of branchTableIds) {
    sqlScript += `-- Bảng ${tableId}\n`;
    for (const indicator of indicators) {
      sqlScript += `INSERT INTO KpiIndicators (KpiAssignmentTableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (${tableId}, N'${indicator.name}', ${indicator.maxScore}, N'${indicator.unit}', ${indicator.order}, 'NUMBER', 1);\n`;
    }
    sqlScript += '\n';
  }

  // Thêm kiểm tra kết quả
  sqlScript += `-- Kiểm tra kết quả
SELECT t.Id, t.TableName, COUNT(i.Id) as IndicatorCount
FROM KpiAssignmentTables t
LEFT JOIN KpiIndicators i ON t.Id = i.KpiAssignmentTableId
WHERE t.Id BETWEEN 24 AND 32
GROUP BY t.Id, t.TableName
ORDER BY t.Id;

PRINT N'✅ Hoàn thành tạo chỉ tiêu cho 9 bảng KPI chi nhánh';`;

  console.log('\n🔧 SQL Script để chạy trên database:');
  console.log('='.repeat(80));
  console.log(sqlScript);
  console.log('='.repeat(80));

  // Lưu script vào file
  const fs = require('fs');
  const path = './populate_9_kpi_indicators_final.sql';
  fs.writeFileSync(path, sqlScript);
  console.log(`\n📁 Script đã được lưu vào: ${path}`);

  // Gợi ý cách chạy
  console.log('\n💡 Cách chạy script:');
  console.log('1. Sao chép script SQL ở trên');
  console.log('2. Kết nối vào SQL Server Management Studio hoặc Azure Data Studio');
  console.log('3. Chọn database TinhKhoanDB');
  console.log('4. Dán và chạy script SQL');
  console.log('5. Hoặc sử dụng docker exec nếu có sqlcmd:');
  console.log(`   docker exec -i azure_sql_edge_tinhkhoan sqlcmd -S localhost -U sa -P "YourStrongPassword123" -d TinhKhoanDB < ${path}`);

  // Kiểm tra kết quả hiện tại
  console.log('\n🔍 Kiểm tra tình trạng hiện tại của 9 bảng chi nhánh:');
  try {
    const response = await fetch(`${baseUrl}/KpiAssignment/tables`);
    const tables = await response.json();
    const branchTables = tables.filter(t => t.Id >= 24 && t.Id <= 32);

    branchTables.forEach(table => {
      console.log(`ID ${table.Id}: ${table.TableName} - ${table.Description} (${table.IndicatorCount} indicators)`);
    });
  } catch (error) {
    console.error('❌ Lỗi khi kiểm tra tables:', error.message);
  }
}

// Chạy script
populateIndicators().catch(console.error);
