#!/bin/bash

# 🚀 FAST COMMIT SCRIPT - Tối ưu cho commit nhanh
# Usage: ./fast_commit.sh [message]

set -e  # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}�� FAST COMMIT STARTING...${NC}"

# Check if we're in a git repository
if ! git rev-parse --git-dir > /dev/null 2>&1; then
    echo -e "${RED}❌ Not in a git repository!${NC}"
    exit 1
fi

# Get commit message from parameter or use default
if [ -n "$1" ]; then
    COMMIT_MSG="$1"
else
    COMMIT_MSG="🎉 Fix Smart Import System - Complete Solution

✅ Core Fixes:
- DateTime conversion: string '31/12/2024' → DateTime in DataTable
- Column mapping: exclude non-existent DATA_DATE column
- Enhanced CSV config: RFC4180 mode, proper quote/escape handling
- Advanced string cleaning: BOM removal, quote unescaping

🔧 Special RR01 Parser:
- Custom parser for non-standard CSV format
- Handles nested quotes and escaped characters
- Successfully parses banking data with complex formatting

📊 Test Results:
- EI01: ✅ 1 record
- DP01: ✅ 1 record  
- GL41: ✅ 394 records (5,819 records/sec)
- RR01: ✅ 1 record (special parser)

🚀 All core data types now importing successfully!"
fi

echo -e "${YELLOW}📝 Adding files...${NC}"
git add . --all

echo -e "${YELLOW}📊 Checking status...${NC}"
git status --porcelain

echo -e "${YELLOW}💾 Committing...${NC}"
# Use -F flag to read message from stdin to handle special characters
echo "$COMMIT_MSG" | git commit -F -

echo -e "${GREEN}✅ COMMIT COMPLETED SUCCESSFULLY!${NC}"
echo -e "${BLUE}📋 Commit details:${NC}"
git log --oneline -1
