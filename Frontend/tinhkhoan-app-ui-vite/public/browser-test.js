// 🧪 Test API từ browser console
console.log('🚀 Starting browser API test...');

const API_BASE = 'http://localhost:5055/api';

async function testBrowserAPI() {
    console.log('🔍 Testing from browser...');

    try {
        // Test 1: Direct fetch
        console.log('📡 Testing direct fetch...');
        const response = await fetch(`${API_BASE}/units`);
        console.log('📊 Response status:', response.status);
        console.log('📊 Response headers:', [...response.headers.entries()]);

        const data = await response.json();
        console.log('📊 Data type:', typeof data);
        console.log('📊 Is array:', Array.isArray(data));
        console.log('📊 Length:', Array.isArray(data) ? data.length : 'N/A');
        console.log('📊 First item:', data[0]);

        return data;
    } catch (error) {
        console.error('❌ Browser API test failed:', error);
        throw error;
    }
}

// Test với axios như store
async function testAxiosAPI() {
    console.log('🔍 Testing with axios-like fetch...');

    try {
        const response = await fetch(`${API_BASE}/units`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            credentials: 'omit' // Không gửi cookies
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}: ${response.statusText}`);
        }

        const data = await response.json();
        console.log('✅ Axios-style test successful');
        console.log('📊 Data:', data);

        return data;
    } catch (error) {
        console.error('❌ Axios-style test failed:', error);
        throw error;
    }
}

// Run tests
(async () => {
    try {
        console.log('=== TEST 1: Direct fetch ===');
        await testBrowserAPI();

        console.log('\n=== TEST 2: Axios-style fetch ===');
        await testAxiosAPI();

        console.log('🎉 All browser tests completed successfully!');
    } catch (error) {
        console.error('💥 Browser tests failed:', error);
    }
})();

// Export for manual testing
window.testAPI = {
    testBrowserAPI,
    testAxiosAPI,
    API_BASE
};
