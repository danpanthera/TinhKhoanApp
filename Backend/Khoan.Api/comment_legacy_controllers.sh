#!/bin/bash

# Script to comment out legacy import controllers and services
# Keep them for reference but remove from compilation

echo "🧹 Commenting out legacy import controllers and services..."

# 1. Comment out UniversalImportController
echo "📝 Commenting out UniversalImportController..."
sed -i '' '1i\
/*\
' /opt/Projects/Khoan/Backend/KhoanApp.Api/Controllers/UniversalImportController.cs
echo '*/' >> /opt/Projects/Khoan/Backend/KhoanApp.Api/Controllers/UniversalImportController.cs

# 2. Comment out RawDataImportController
echo "📝 Commenting out RawDataImportController..."
sed -i '' '1i\
/*\
' /opt/Projects/Khoan/Backend/KhoanApp.Api/Controllers/RawDataImportController.cs
echo '*/' >> /opt/Projects/Khoan/Backend/KhoanApp.Api/Controllers/RawDataImportController.cs

# 3. Comment out ExtendedRawDataImportController
echo "📝 Commenting out ExtendedRawDataImportController..."
sed -i '' '1i\
/*\
' /opt/Projects/Khoan/Backend/KhoanApp.Api/Controllers/ExtendedRawDataImportController.cs
echo '*/' >> /opt/Projects/Khoan/Backend/KhoanApp.Api/Controllers/ExtendedRawDataImportController.cs

echo "✅ Legacy controllers commented out successfully!"
echo "💡 Note: Services are still registered in Program.cs for other dependencies"
echo "🔄 You may need to rebuild the project"
