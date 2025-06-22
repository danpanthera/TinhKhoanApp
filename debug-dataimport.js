// Quick debugging script cho DataImportView
// Copy paste vào browser console tại http://localhost:3000/data-import

async function debugDataImportView() {
    console.log('🔍 Debug DataImportView...');
    
    try {
        // Test API call trực tiếp
        const response = await fetch('/api/rawdata');
        const rawData = await response.json();
        
        console.log('📊 Raw API Response:', rawData);
        
        // Parse $values
        let data = rawData;
        if (rawData && rawData.$values) {
            data = rawData.$values;
        }
        
        console.log('📋 Parsed Data:', data);
        console.log('📈 Total Records:', data.length);
        
        // Calculate stats như frontend
        const stats = {};
        const dataTypes = ['API_IMPORT', 'LN01', 'LN02', 'DP01', 'EI01', 'GL01', 'DPDA', 'DB01', 'KH03', 'BC57', 'RR01', '7800_DT_KHKD1', 'GLCB41'];
        
        dataTypes.forEach(type => {
            stats[type] = { totalRecords: 0, lastUpdate: null };
        });
        
        data.forEach(imp => {
            console.log(`Processing import: ${imp.fileName} (${imp.dataType}) - ${imp.recordsCount} records`);
            if (stats[imp.dataType]) {
                stats[imp.dataType].totalRecords += imp.recordsCount || 0;
                
                const importDate = imp.importDate;
                if (importDate && importDate !== "0001-01-01T00:00:00") {
                    if (!stats[imp.dataType].lastUpdate || 
                        new Date(importDate) > new Date(stats[imp.dataType].lastUpdate)) {
                        stats[imp.dataType].lastUpdate = importDate;
                    }
                }
            } else {
                console.warn(`Unknown dataType: ${imp.dataType}`);
            }
        });
        
        console.log('📊 Calculated Stats:', stats);
        
        // Hiển thị kết quả UI-friendly
        const apiImportStats = stats['API_IMPORT'];
        console.log(`
✅ SUMMARY:
📊 API_IMPORT Total Records: ${apiImportStats.totalRecords}
📅 API_IMPORT Last Update: ${apiImportStats.lastUpdate || 'N/A'}
🔢 Total Import Records: ${data.length}
        `);
        
        return { data, stats };
        
    } catch (error) {
        console.error('❌ Debug failed:', error);
        return null;
    }
}

// Gọi function
debugDataImportView().then(result => {
    if (result) {
        console.log('🎉 Debug completed successfully!');
        console.log('💡 Suggestion: Check if Vue component reactive data is updating correctly');
    }
});
