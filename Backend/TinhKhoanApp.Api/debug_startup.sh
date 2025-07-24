#!/bin/bash
echo "🚀 Starting TinhKhoan Backend with Debug Output..."
echo "📂 Current directory: $(pwd)"

# Set verbose environment
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_LOGGING__LOGLEVEL__DEFAULT=Debug
export ASPNETCORE_LOGGING__LOGLEVEL__MICROSOFT=Information
export ASPNETCORE_LOGGING__CONSOLE__INCLUDESCOPES=true

echo "🔍 Environment Variables:"
echo "ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT"

echo "🔧 Building project first..."
dotnet build --configuration Debug --verbosity minimal

if [ $? -eq 0 ]; then
    echo "✅ Build successful!"
    echo "🌐 Starting backend with debug logging..."
    dotnet run --urls=http://localhost:5055 --configuration Debug
else
    echo "❌ Build failed!"
    exit 1
fi
