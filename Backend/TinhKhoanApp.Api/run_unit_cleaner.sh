#!/bin/bash

# This script compiles and runs the UnitCleaner.cs to remove units with ID >= 1026 and lock the unit list

cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api

echo "Compiling UnitCleaner..."
dotnet new console -n UnitCleanerApp -o ./UnitCleanerApp
cp UnitCleaner.cs ./UnitCleanerApp/Program.cs
cd ./UnitCleanerApp

# Add required packages
dotnet add package Microsoft.Data.SqlClient
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.Json

# Copy appsettings.json to the output directory
cp ../appsettings.json ./

# Build and run
echo "Running UnitCleaner..."
dotnet run

echo "Done"
