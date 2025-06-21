#!/bin/bash

# This script applies the StandardizeBranchNames migration using Entity Framework Core

cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api

# Ensure we're using the correct .NET SDK
echo "Using .NET SDK version:"
dotnet --version

# Create a migration with EF Core
echo "Creating migration..."
dotnet ef migrations add StandardizeBranchNames --context ApplicationDbContext

# Apply the migration
echo "Applying migration..."
dotnet ef database update --context ApplicationDbContext

echo "Migration complete"
