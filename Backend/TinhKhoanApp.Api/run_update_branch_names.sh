#!/bin/bash

# This script compiles and runs the UpdateBranchNamesProgram.cs to update branch names and codes

cd /Users/nguyendat/Documents/Projects/TinhKhoanApp/Backend/TinhKhoanApp.Api

echo "Compiling and running the UpdateBranchNamesProgram..."
dotnet run --project . /UpdateBranchNamesProgram.cs

echo "Done"
