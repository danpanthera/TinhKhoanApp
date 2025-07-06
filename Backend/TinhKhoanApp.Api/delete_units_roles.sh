#!/bin/bash

# Script to execute SQL commands for deleting Units and Roles data
# Make sure backend API is running on localhost:5055

echo "Starting deletion process for Units and Roles data..."
echo "=================================="

# First, create backup
echo "Step 1: Creating backup..."
curl -X POST "http://localhost:5055/api/execute-sql" \
  -H "Content-Type: application/json" \
  -d '{
    "sql": "SELECT * INTO UnitsBackup_20250706 FROM Units; SELECT * INTO RolesBackup_20250706 FROM Roles;"
  }'

echo "Backup created."

# Execute deletion script
echo "Step 2: Executing deletion..."
curl -X POST "http://localhost:5055/api/execute-sql" \
  -H "Content-Type: application/json" \
  -d '{
    "sql": "EXEC sp_MSforeachtable \"ALTER TABLE ? NOCHECK CONSTRAINT all\"; DELETE FROM EmployeeRoles; UPDATE Employees SET UnitId = NULL WHERE UnitId IS NOT NULL; DELETE FROM Units WHERE ParentUnitId IS NOT NULL; DELETE FROM Units WHERE ParentUnitId IS NULL; DELETE FROM Roles; EXEC sp_MSforeachtable \"ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all\";"
  }'

echo "Deletion completed."

# Verify results
echo "Step 3: Verifying deletion..."
echo "Remaining Units:"
curl -s "http://localhost:5055/api/units" | jq 'length'

echo "Remaining Roles:"
curl -s "http://localhost:5055/api/roles" | jq 'length'

echo "=================================="
echo "Deletion process completed!"
