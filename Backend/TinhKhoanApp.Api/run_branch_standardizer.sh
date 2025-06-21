#!/bin/bash

# This script compiles and runs the BranchStandardizer.cs to update branch names directly

cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api

echo "Compiling BranchStandardizer..."
dotnet new console -n BranchStandardizerApp -o ./BranchStandardizerApp
cp BranchStandardizer.cs ./BranchStandardizerApp/Program.cs
cd ./BranchStandardizerApp

# Add required packages
dotnet add package Microsoft.Data.SqlClient
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Configuration.FileConfigurationExtensions
dotnet add package Microsoft.Extensions.Configuration.Json

# Copy appsettings.json to the output directory
cp ../appsettings.json ./

# Build and run
echo "Running BranchStandardizer..."
dotnet run

echo "Done"
