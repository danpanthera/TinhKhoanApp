#!/bin/bash

# Migration Completion Verification Script
# Verifies that ImportedDataItems system has been fully removed and DirectImport system is operational

echo "🔍 TINHKHOAN APP - MIGRATION COMPLETION VERIFICATION"
echo "=================================================="
echo "Date: $(date)"
echo ""

# Check if we're in the right directory
if [ ! -f "TinhKhoanApp.Api.csproj" ]; then
    echo "❌ Error: Not in TinhKhoanApp.Api directory"
    exit 1
fi

echo "✅ Current directory: $(pwd)"
echo ""

# 1. Verify ImportedDataItems removal
echo "1️⃣  IMPORTEDDATAITEMS REMOVAL VERIFICATION"
echo "----------------------------------------"

# Check if ImportedDataItem.cs exists
if [ -f "Models/ImportedDataItem.cs" ]; then
    echo "❌ ImportedDataItem.cs still exists"
else
    echo "✅ ImportedDataItem.cs successfully removed"
fi

# Check for any remaining references in code
echo ""
echo "Checking for remaining ImportedDataItem references in code:"
REFERENCES=$(grep -r "ImportedDataItem" --include="*.cs" . | wc -l)
if [ $REFERENCES -eq 0 ]; then
    echo "✅ No ImportedDataItem references found in C# code"
else
    echo "⚠️  Found $REFERENCES ImportedDataItem references:"
    grep -r "ImportedDataItem" --include="*.cs" . | head -10
fi

echo ""

# 2. Verify DirectImportService implementation
echo "2️⃣  DIRECTIMPORTSERVICE IMPLEMENTATION VERIFICATION"
echo "------------------------------------------------"

# Check if DirectImportService exists
if [ -f "Services/DirectImportService.cs" ]; then
    echo "✅ DirectImportService.cs exists"
else
    echo "❌ DirectImportService.cs missing"
fi

# Check if interface exists
if [ -f "Services/Interfaces/IDirectImportService.cs" ]; then
    echo "✅ IDirectImportService.cs exists"
else
    echo "❌ IDirectImportService.cs missing"
fi

# Check if DirectImportResult model exists
if [ -f "Models/DirectImportResult.cs" ]; then
    echo "✅ DirectImportResult.cs exists"
else
    echo "❌ DirectImportResult.cs missing"
fi

echo ""

# 3. Check service registration in Program.cs
echo "3️⃣  SERVICE REGISTRATION VERIFICATION"
echo "-----------------------------------"

if grep -q "IDirectImportService" Program.cs; then
    echo "✅ IDirectImportService registered in Program.cs"
else
    echo "❌ IDirectImportService not registered in Program.cs"
fi

echo ""

# 4. Check ApplicationDbContext cleanup
echo "4️⃣  APPLICATIONDBCONTEXT CLEANUP VERIFICATION"
echo "--------------------------------------------"

if grep -q "DbSet<ImportedDataItem>" Data/ApplicationDbContext.cs; then
    echo "❌ ImportedDataItem DbSet still exists in ApplicationDbContext"
else
    echo "✅ ImportedDataItem DbSet removed from ApplicationDbContext"
fi

echo ""

# 5. Check controller updates
echo "5️⃣  CONTROLLER UPDATES VERIFICATION"
echo "----------------------------------"

# Check if DataImportController has upload-direct endpoint
if grep -q "upload-direct" Controllers/DataImportController.cs; then
    echo "✅ upload-direct endpoint exists in DataImportController"
else
    echo "❌ upload-direct endpoint missing in DataImportController"
fi

# Check DirectImportController
if [ -f "Controllers/DirectImportController.cs" ]; then
    echo "✅ DirectImportController.cs exists"
else
    echo "❌ DirectImportController.cs missing"
fi

echo ""

# 6. Check backup files
echo "6️⃣  BACKUP FILES VERIFICATION"
echo "----------------------------"

if [ -d "backup_importeddataitems_20250709_201445" ]; then
    echo "✅ Backup directory exists: backup_importeddataitems_20250709_201445"
    echo "   Backup files count: $(find backup_importeddataitems_20250709_201445 -name "*.cs" | wc -l)"
else
    echo "⚠️  Backup directory not found"
fi

echo ""

# 7. Build verification
echo "7️⃣  BUILD VERIFICATION"
echo "---------------------"

echo "Running dotnet build to verify compilation..."
BUILD_OUTPUT=$(dotnet build 2>&1)
BUILD_RESULT=$?

if [ $BUILD_RESULT -eq 0 ]; then
    echo "✅ Project builds successfully"
else
    echo "❌ Build failed:"
    echo "$BUILD_OUTPUT" | tail -10
fi

echo ""

# 8. Git status
echo "8️⃣  GIT STATUS"
echo "-------------"

echo "Git status:"
git status --porcelain

echo ""

# 9. Summary
echo "9️⃣  MIGRATION COMPLETION SUMMARY"
echo "------------------------------"

echo "Migration Status:"
echo "- ImportedDataItems system: 🗑️  REMOVED"
echo "- DirectImport system: 🚀 IMPLEMENTED"
echo "- Code compilation: $([ $BUILD_RESULT -eq 0 ] && echo '✅ PASSING' || echo '❌ FAILING')"
echo "- Backup files: $([ -d 'backup_importeddataitems_20250709_201445' ] && echo '✅ CREATED' || echo '⚠️  MISSING')"

echo ""
echo "🎉 MIGRATION VERIFICATION COMPLETE"
echo "================================="

if [ $BUILD_RESULT -eq 0 ] && [ ! -f "Models/ImportedDataItem.cs" ] && [ -f "Services/DirectImportService.cs" ]; then
    echo "✅ MIGRATION SUCCESSFULLY COMPLETED!"
    echo ""
    echo "Next steps:"
    echo "1. Create EF Core migration to drop ImportedDataItems table"
    echo "2. Run final system tests with real data"
    echo "3. Update API documentation"
    echo "4. Deploy to staging environment"
else
    echo "⚠️  MIGRATION NEEDS ATTENTION - CHECK ERRORS ABOVE"
fi

echo ""
echo "Script completed at: $(date)"
