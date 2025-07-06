#!/bin/bash

# Test script để gán role đầu tiên
BASE_URL="http://localhost:5055/api"

echo "🧪 TEST GÁN ROLE CHO 1 EMPLOYEE"

# Test health check
health=$(curl -s "$BASE_URL/health" | jq -r '.status')
echo "Health: $health"

if [ "$health" = "Healthy" ]; then
    echo "✅ Backend OK"

    # Test assign role to first employee
    echo "🔄 Gán role ID 12 cho employee ID 1..."

    # Create payload
    payload='{"RoleIds": [12]}'
    echo "Payload: $payload"

    # Make the API call
    response=$(curl -s -X PUT "$BASE_URL/employees/1" \
        -H "Content-Type: application/json" \
        -d "$payload")

    echo "Response: $response"

    # Check result
    echo ""
    echo "📊 Kiểm tra employee sau khi gán:"
    curl -s "$BASE_URL/employees/1" | jq '{Id: .Id, FullName: .FullName, Roles: .Roles}'

else
    echo "❌ Backend not healthy"
fi
