#!/bin/bash
echo "Tạo migration cho LN03 với 20 cột..."
dotnet ef migrations add "UpdateLN03With20Columns" --context ApplicationDbContext
echo "Migration created! Run 'dotnet ef database update' to apply."
