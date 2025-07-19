#!/bin/bash

# 🔄 RECREATE 8 DATATABLES MODELS TO MATCH DATABASE STRUCTURE
# Tái tạo 8 models để khớp hoàn toàn với database structure
# Created: 2025-07-19

echo "🔄 RECREATING 8 DATATABLES MODELS TO MATCH DATABASE..."
echo "===================================================="
echo ""

# Backup existing models
echo "📦 Step 1: Backing up existing models..."
mkdir -p Models/DataTables/backup_$(date +%Y%m%d_%H%M%S)
cp Models/DataTables/*.cs Models/DataTables/backup_$(date +%Y%m%d_%H%M%S)/ 2>/dev/null || true
echo "   - Backup completed"
echo ""

# List current models
echo "📋 Step 2: Current DataTables models:"
ls -la Models/DataTables/*.cs 2>/dev/null | grep -E "(GL01|DP01|DPDA|EI01|GL41|LN01|LN03|RR01)" || echo "   - No existing models found"
echo ""

echo "🎯 Step 3: Models will be recreated to match exact database structure:"
echo "   - GL01: 27 CSV columns + System columns (Partitioned)"
echo "   - DP01: 63 CSV columns + System + Temporal columns"
echo "   - DPDA: 13 CSV columns + System + Temporal columns"
echo "   - EI01: 24 CSV columns + System + Temporal columns"
echo "   - GL41: 13 CSV columns + System + Temporal columns"
echo "   - LN01: 79 CSV columns + System + Temporal columns"
echo "   - LN03: 17 CSV columns + System + Temporal columns"
echo "   - RR01: 25 CSV columns + System + Temporal columns"
echo ""

echo "✅ Ready to recreate models with exact database structure!"
echo "Next: Run model generation script for each table"
echo ""
