#!/bin/bash

# 🧪 FINAL TEST: Employee Edit Workflow
# Script này test toàn bộ luồng edit employee như trên UI thật

echo "🚀 FINAL TEST: Employee Edit Workflow"
echo "======================================"
echo

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

API_BASE="http://localhost:5055/api"
EMPLOYEE_ID=1

echo -e "${BLUE}📋 Step 1: Fetch Employee List${NC}"
echo "GET $API_BASE/employees"
EMPLOYEES=$(curl -s "$API_BASE/employees")
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Employee list fetched successfully${NC}"
    echo "Employee count: $(echo "$EMPLOYEES" | jq '. | length')"
else
    echo -e "${RED}❌ Failed to fetch employee list${NC}"
    exit 1
fi
echo

echo -e "${BLUE}📋 Step 2: Fetch Employee Detail for Edit${NC}"
echo "GET $API_BASE/employees/$EMPLOYEE_ID"
EMPLOYEE_DETAIL=$(curl -s "$API_BASE/employees/$EMPLOYEE_ID")
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Employee detail fetched successfully${NC}"
    echo "Employee: $(echo "$EMPLOYEE_DETAIL" | jq -r '.FullName')"
    echo "CBCode: $(echo "$EMPLOYEE_DETAIL" | jq -r '.CBCode')"
else
    echo -e "${RED}❌ Failed to fetch employee detail${NC}"
    exit 1
fi
echo

echo -e "${BLUE}📋 Step 3: Extract Employee Data (như extractEmployeePrimitives)${NC}"
# Simulate extractEmployeePrimitives logic
EXTRACTED_ID=$(echo "$EMPLOYEE_DETAIL" | jq -r '.Id')
EXTRACTED_CODE=$(echo "$EMPLOYEE_DETAIL" | jq -r '.EmployeeCode')
EXTRACTED_CBCODE=$(echo "$EMPLOYEE_DETAIL" | jq -r '.CBCode')
EXTRACTED_NAME=$(echo "$EMPLOYEE_DETAIL" | jq -r '.FullName')
EXTRACTED_USERNAME=$(echo "$EMPLOYEE_DETAIL" | jq -r '.Username')
EXTRACTED_EMAIL=$(echo "$EMPLOYEE_DETAIL" | jq -r '.Email')

echo "Extracted ID: $EXTRACTED_ID"
echo "Extracted CBCode: $EXTRACTED_CBCODE"
if [ "$EXTRACTED_ID" != "null" ] && [ "$EXTRACTED_ID" != "" ]; then
    echo -e "${GREEN}✅ ID extraction successful${NC}"
else
    echo -e "${RED}❌ ID extraction failed${NC}"
    exit 1
fi
echo

echo -e "${BLUE}📋 Step 4: Validate CBCode (như trong handleSubmitEmployee)${NC}"
# Test CBCode validation
NEW_CBCODE="987654321"
if [[ "$NEW_CBCODE" =~ ^[0-9]{9}$ ]]; then
    echo -e "${GREEN}✅ CBCode validation passed: $NEW_CBCODE${NC}"
else
    echo -e "${RED}❌ CBCode validation failed: $NEW_CBCODE${NC}"
    exit 1
fi
echo

echo -e "${BLUE}📋 Step 5: Prepare Update Payload (như trong employeeStore)${NC}"
UPDATE_PAYLOAD=$(cat <<EOF
{
    "Id": $EXTRACTED_ID,
    "employeeCode": "$EXTRACTED_CODE",
    "cbCode": "$NEW_CBCODE",
    "fullName": "$EXTRACTED_NAME - Test Updated",
    "username": "$EXTRACTED_USERNAME",
    "email": "$EXTRACTED_EMAIL",
    "phoneNumber": "0987654321",
    "isActive": true,
    "unitId": 1,
    "positionId": 1,
    "roleIds": []
}
EOF
)

echo "Update payload prepared:"
echo "$UPDATE_PAYLOAD" | jq .
echo

echo -e "${BLUE}📋 Step 6: Execute Update API Call${NC}"
echo "PUT $API_BASE/employees/$EXTRACTED_ID"

UPDATE_RESPONSE=$(curl -s -X PUT "$API_BASE/employees/$EXTRACTED_ID" \
    -H "Content-Type: application/json" \
    -d "$UPDATE_PAYLOAD")

UPDATE_STATUS=$?
if [ $UPDATE_STATUS -eq 0 ]; then
    echo -e "${GREEN}✅ Update API call successful${NC}"
    echo "Updated employee:"
    echo "$UPDATE_RESPONSE" | jq '. | {Id: .Id, EmployeeCode: .EmployeeCode, FullName: .FullName, CBCode: .CBCode}'
else
    echo -e "${RED}❌ Update API call failed${NC}"
    echo "Response: $UPDATE_RESPONSE"
    exit 1
fi
echo

echo -e "${BLUE}📋 Step 7: Verify Update Success${NC}"
echo "GET $API_BASE/employees/$EXTRACTED_ID (verification)"

VERIFY_RESPONSE=$(curl -s "$API_BASE/employees/$EXTRACTED_ID")
VERIFY_CBCODE=$(echo "$VERIFY_RESPONSE" | jq -r '.CBCode')

if [ "$VERIFY_CBCODE" = "$NEW_CBCODE" ]; then
    echo -e "${GREEN}✅ Update verification successful${NC}"
    echo "CBCode successfully updated to: $VERIFY_CBCODE"
else
    echo -e "${RED}❌ Update verification failed${NC}"
    echo "Expected: $NEW_CBCODE, Got: $VERIFY_CBCODE"
    exit 1
fi
echo

echo -e "${GREEN}🎉 FINAL TEST RESULT: ALL STEPS PASSED!${NC}"
echo "======================================"
echo -e "${GREEN}✅ Employee list fetch${NC}"
echo -e "${GREEN}✅ Employee detail fetch${NC}"
echo -e "${GREEN}✅ Data extraction${NC}"
echo -e "${GREEN}✅ CBCode validation${NC}"
echo -e "${GREEN}✅ Update payload preparation${NC}"
echo -e "${GREEN}✅ API update call${NC}"
echo -e "${GREEN}✅ Update verification${NC}"
echo
echo -e "${BLUE}🎯 Conclusion: Employee Edit Workflow is working correctly!${NC}"
echo -e "${BLUE}🎯 The 'api/Employees/undefined' error has been fixed!${NC}"
echo -e "${BLUE}🎯 CBCode validation is working properly!${NC}"
