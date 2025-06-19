#!/bin/bash
# Test script to verify branch ordering is working correctly

echo "🧪 Testing Branch Ordering in Frontend and Backend..."

# Test 1: Backend API returns units in correct order with SortOrder
echo ""
echo "📍 Test 1: Backend Units API with SortOrder field"
echo "Expected: Branches should be ordered by SortOrder (1=Hội sở, 2=Tam Đường, 3=Phong Thổ, etc.)"

curl -s http://localhost:5055/api/Units | \
  jq -r '.["$values"] | map(select(.type == "CNL1" or .type == "CNL2")) | sort_by(.sortOrder) | .[] | "\(.sortOrder // "NULL"): \(.name) (\(.code))"'

echo ""
echo "📍 Test 2: First 10 units from /api/Units (should be pre-sorted by SortOrder)"
curl -s http://localhost:5055/api/Units | \
  jq -r '.["$values"][0:10] | .[] | "\(.sortOrder // "NULL"): \(.name) (\(.code)) - \(.type)"'

echo ""
echo "✅ Test completed! Check above output to verify:"
echo "   - CNL1 (Hội sở) should have sortOrder=1 and appear first" 
echo "   - CNL2 branches should have sortOrder 2-9 in the specified order"
echo "   - Other units should have sortOrder=999 or NULL"
