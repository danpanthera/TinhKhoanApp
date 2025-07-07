const fetch = require('node-fetch');
const fs = require('fs');

const API_BASE_URL = 'http://localhost:5055/api';

// Function để lấy chỉ tiêu của bảng GiamdocCnl2
async function getGiamdocCnl2Indicators() {
  try {
    // Lấy tất cả bảng KPI
    const tablesResponse = await fetch(`${API_BASE_URL}/KpiAssignment/tables`);
    const tables = await tablesResponse.json();

    // Tìm bảng GiamdocCnl2
    const giamdocTable = tables.find(t => t.TableName === 'GiamdocCnl2');
    if (!giamdocTable) {
      console.error('❌ Không tìm thấy bảng GiamdocCnl2');
      return null;
    }

    console.log(`✅ Tìm thấy bảng GiamdocCnl2 với ID: ${giamdocTable.Id}`);
    console.log(`   Mô tả: ${giamdocTable.Description}`);

    // Lấy chỉ tiêu của bảng này
    const indicatorsResponse = await fetch(`${API_BASE_URL}/KpiAssignment/tables/${giamdocTable.Id}`);
    const tableDetails = await indicatorsResponse.json();
    const indicators = tableDetails.Indicators;

    console.log(`📊 Tìm thấy ${indicators.length} chỉ tiêu:`);
    indicators.forEach((ind, i) => {
      console.log(`   ${i+1}. ${ind.IndicatorName} | ${ind.MaxScore} điểm | ${ind.Unit} | Thứ tự: ${ind.OrderIndex}`);
    });

    return indicators;
  } catch (error) {
    console.error('❌ Lỗi khi lấy dữ liệu:', error.message);
    return null;
  }
}

// Function để tạo script SQL
function generateSQL(indicators) {
  if (!indicators || indicators.length === 0) {
    console.error('❌ Không có chỉ tiêu để tạo SQL');
    return;
  }

  let sql = `-- 🏢 Script SQL tạo chỉ tiêu KPI cho 9 bảng chi nhánh
-- Dựa trên bảng "Giám đốc Chi nhánh cấp 2" (${indicators.length} chỉ tiêu)
-- Ngày tạo: ${new Date().toLocaleString('vi-VN')}

-- Xóa chỉ tiêu cũ của 9 bảng chi nhánh (ID 24-32)
DELETE FROM KpiIndicators WHERE TableId BETWEEN 24 AND 32;

`;

  // Generate INSERT statements for each branch table (ID 24-32)
  for (let tableId = 24; tableId <= 32; tableId++) {
    sql += `-- Bảng ${tableId}\n`;

    indicators.forEach(indicator => {
      sql += `INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES (${tableId}, N'${indicator.IndicatorName}', ${indicator.MaxScore}, N'${indicator.Unit}', ${indicator.OrderIndex}, ${indicator.ValueType || 1}, ${indicator.IsActive ? 1 : 0});\n`;
    });

    sql += '\n';
  }

  sql += `-- ✅ Hoàn tất populate ${indicators.length} chỉ tiêu cho 9 bảng chi nhánh (ID 24-32)
-- Total: ${9 * indicators.length} records inserted
`;

  return sql;
}

// Main function
async function main() {
  console.log('🚀 Bắt đầu tạo script SQL populate 9 bảng KPI chi nhánh...\n');

  const indicators = await getGiamdocCnl2Indicators();
  if (!indicators) {
    console.error('❌ Không thể tiếp tục');
    return;
  }

  const sql = generateSQL(indicators);
  if (!sql) {
    console.error('❌ Không thể tạo script SQL');
    return;
  }

  // Lưu script ra file
  const filename = 'populate_9_branch_kpi_from_giamdoccnl2.sql';
  fs.writeFileSync(filename, sql, 'utf8');

  console.log(`\n✅ Đã tạo script SQL: ${filename}`);
  console.log(`📁 File size: ${(sql.length / 1024).toFixed(1)} KB`);
  console.log(`🔢 Total INSERT statements: ${9 * indicators.length}`);

  console.log('\n📋 Preview script:');
  console.log(sql.substring(0, 800) + '...\n');

  console.log('🎯 Để chạy script:');
  console.log(`   sqlcmd -S localhost -d TinhKhoanDB -i ${filename}`);
  console.log('   hoặc copy-paste vào SQL Server Management Studio');
}

// Check if node-fetch is available
try {
  require.resolve('node-fetch');
} catch (e) {
  console.error('❌ node-fetch chưa được cài đặt');
  console.log('📦 Chạy: npm install node-fetch@2');
  process.exit(1);
}

main().catch(console.error);
