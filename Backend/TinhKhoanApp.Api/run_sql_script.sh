#!/bin/bash

# Run SQL script to update branch names in the database
# Requires the sqlcmd utility (included with SQL Server command-line tools)

# Path to the SQL script
SQL_SCRIPT="standardize_branch_names.sql"

# SQL Server connection parameters from appsettings.json
SERVER="localhost,1433"
DATABASE="TinhKhoanDB"
USERNAME="sa"
PASSWORD="YourStrong@Password123"

echo "Running SQL script to standardize branch names..."

# Execute the SQL script using sqlcmd
sqlcmd -S $SERVER -d $DATABASE -U $USERNAME -P $PASSWORD -i $SQL_SCRIPT

echo "SQL script executed. Branch names have been standardized."
