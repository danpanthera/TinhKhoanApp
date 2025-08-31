#!/bin/bash

echo "🚀 Starting LN03 API Server & Testing CSV Import..."

# Start API server in background
cd /opt/Projects/Khoan/Backend/Khoan.Api
echo "⏳ Starting API server..."
dotnet run --project /opt/Projects/Khoan/Backend/Khoan.Api/Khoan.Api.csproj --urls=http://localhost:5000 &
API_PID=$!

# Wait for API to start
echo "⏳ Waiting for API to start (15 seconds)..."
sleep 15

# Test API health
echo "🩺 Testing API health..."
HEALTH_CHECK=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/health 2>/dev/null || echo "000")

if [[ "$HEALTH_CHECK" == "200" ]]; then
    echo "✅ API is healthy"
else
    echo "🔍 Trying swagger endpoint..."
    SWAGGER_CHECK=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/swagger 2>/dev/null || echo "000")
    
    if [[ "$SWAGGER_CHECK" == "200" ]]; then
        echo "✅ API is running (swagger accessible)"
    else
        echo "⏳ API still starting, waiting 10 more seconds..."
        sleep 10
    fi
fi

# Test LN03 count
echo "📊 Testing LN03 record count..."
COUNT_RESPONSE=$(curl -s "http://localhost:5000/api/LN03/count" 2>/dev/null || echo "error")

if [[ "$COUNT_RESPONSE" != "error" ]]; then
    echo "✅ LN03 API accessible. Current count: $COUNT_RESPONSE"
else
    echo "❌ LN03 API not accessible"
fi

# List available endpoints
echo "🔍 Testing available endpoints..."
curl -s "http://localhost:5000/api/LN03" -I 2>/dev/null || echo "❌ Main endpoint not accessible"

echo ""
echo "🧪 Manual test commands available:"
echo "1. Test count: curl -s 'http://localhost:5000/api/LN03/count'"
echo "2. Test CSV import: curl -X POST 'http://localhost:5000/api/LN03/import-csv' -F 'file=@7800_ln03_20241231.csv'"
echo "3. View records: curl -s 'http://localhost:5000/api/LN03?pageSize=5'"
echo "4. Access Swagger: http://localhost:5000/swagger"
echo ""
echo "🛑 To stop API server: kill $API_PID"

# Keep script running to maintain API
echo "📡 API server is running in background (PID: $API_PID)"
echo "Press Ctrl+C to stop this script and the API server"

# Trap to cleanup on exit
cleanup() {
    echo "🛑 Stopping API server..."
    kill $API_PID 2>/dev/null
    exit 0
}
trap cleanup EXIT INT TERM

# Keep running
wait $API_PID
