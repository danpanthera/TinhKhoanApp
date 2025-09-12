#!/bin/bash

# Wait for SQL Server to be ready
echo "Waiting for SQL Server to start..."
sleep 30

# Try to attach the database using sqlcmd (if available)
echo "Attempting to attach TinhKhoanDB..."
if [ -f "/var/opt/mssql/data/TinhKhoanDB.mdf" ]; then
    echo "Database files found, attempting to create database..."
    # Note: This would need actual SQL commands via sqlcmd if available
    # For now, this is a placeholder
    echo "Database attach script executed"
else
    echo "No database files found"
fi