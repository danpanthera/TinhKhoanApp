#!/bin/bash

# Script tạo 46 đơn vị - Fixed version
API_BASE="http://localhost:5055/api"
LOG_FILE="final_create_46_units.log"

echo "🎯 CREATE 46 UNITS - FIXED VERSION" | tee -a $LOG_FILE
echo "📅 Start: $(date)" | tee -a $LOG_FILE

# Fixed function to create unit
create_unit() {
    local code=$1
    local name=$2
    local type=$3
    local parent_id=$4

    if [ "$parent_id" = "null" ]; then
        JSON='{"Code":"'$code'","Name":"'$name'","Type":"'$type'","ParentUnitId":null}'
    else
        JSON='{"Code":"'$code'","Name":"'$name'","Type":"'$type'","ParentUnitId":'$parent_id'}'
    fi

    # Don't echo during function - only log to file
    echo "Creating: $name (Parent: $parent_id)" >> $LOG_FILE

    RESPONSE=$(curl -s -X POST "$API_BASE/Units" -H "Content-Type: application/json" -d "$JSON")

    # Extract ID
    ID=$(echo "$RESPONSE" | grep -o '"Id":[0-9]*' | head -1 | cut -d':' -f2)

    if [ -n "$ID" ] && [ "$ID" != "" ]; then
        echo "SUCCESS: $name (ID: $ID)" >> $LOG_FILE
        echo "$ID"  # This is the return value
    else
        echo "FAILED: $name - $RESPONSE" >> $LOG_FILE
        echo ""  # Return empty on failure
    fi
}

# Use existing root unit ID 89
ROOT_ID=89
echo "Using root unit ID: $ROOT_ID" | tee -a $LOG_FILE

# Create 9 branches
echo "Creating 9 branches..." | tee -a $LOG_FILE

HS_ID=$(create_unit "HS_FINAL" "Hoi So" "CNL2" "$ROOT_ID")
BL_ID=$(create_unit "BL_FINAL" "CN Binh Lu" "CNL2" "$ROOT_ID")
PT_ID=$(create_unit "PT_FINAL" "CN Phong Tho" "CNL2" "$ROOT_ID")
SH_ID=$(create_unit "SH_FINAL" "CN Sin Ho" "CNL2" "$ROOT_ID")
BT_ID=$(create_unit "BT_FINAL" "CN Bum To" "CNL2" "$ROOT_ID")
TU_ID=$(create_unit "TU_FINAL" "CN Than Uyen" "CNL2" "$ROOT_ID")
DK_ID=$(create_unit "DK_FINAL" "CN Doan Ket" "CNL2" "$ROOT_ID")
TUY_ID=$(create_unit "TUY_FINAL" "CN Tan Uyen" "CNL2" "$ROOT_ID")
NH_ID=$(create_unit "NH_FINAL" "CN Nam Hang" "CNL2" "$ROOT_ID")

echo "Branch IDs created:" | tee -a $LOG_FILE
echo "HS: $HS_ID" | tee -a $LOG_FILE
echo "BL: $BL_ID" | tee -a $LOG_FILE
echo "PT: $PT_ID" | tee -a $LOG_FILE
echo "SH: $SH_ID" | tee -a $LOG_FILE
echo "BT: $BT_ID" | tee -a $LOG_FILE
echo "TU: $TU_ID" | tee -a $LOG_FILE
echo "DK: $DK_ID" | tee -a $LOG_FILE
echo "TUY: $TUY_ID" | tee -a $LOG_FILE
echo "NH: $NH_ID" | tee -a $LOG_FILE

# Count successful branches
BRANCH_COUNT=0
[ -n "$HS_ID" ] && BRANCH_COUNT=$((BRANCH_COUNT + 1))
[ -n "$BL_ID" ] && BRANCH_COUNT=$((BRANCH_COUNT + 1))
[ -n "$PT_ID" ] && BRANCH_COUNT=$((BRANCH_COUNT + 1))
[ -n "$SH_ID" ] && BRANCH_COUNT=$((BRANCH_COUNT + 1))
[ -n "$BT_ID" ] && BRANCH_COUNT=$((BRANCH_COUNT + 1))
[ -n "$TU_ID" ] && BRANCH_COUNT=$((BRANCH_COUNT + 1))
[ -n "$DK_ID" ] && BRANCH_COUNT=$((BRANCH_COUNT + 1))
[ -n "$TUY_ID" ] && BRANCH_COUNT=$((TUY_ID + 1))
[ -n "$NH_ID" ] && BRANCH_COUNT=$((BRANCH_COUNT + 1))

echo "Branches created successfully: $BRANCH_COUNT/9" | tee -a $LOG_FILE

# Create departments for successful branches
echo "Creating departments..." | tee -a $LOG_FILE

DEPT_COUNT=0

# Hoi So departments (7)
if [ -n "$HS_ID" ]; then
    echo "Creating Hoi So departments..." | tee -a $LOG_FILE
    ID1=$(create_unit "HS_BGD" "Ban Giam Doc" "PNVL1" "$HS_ID")
    ID2=$(create_unit "HS_KTNQ" "P KTNQ" "PNVL1" "$HS_ID")
    ID3=$(create_unit "HS_KHDN" "P KHDN" "PNVL1" "$HS_ID")
    ID4=$(create_unit "HS_KHCN" "P KHCN" "PNVL1" "$HS_ID")
    ID5=$(create_unit "HS_KTGS" "P KTGS" "PNVL1" "$HS_ID")
    ID6=$(create_unit "HS_TH" "P Tong Hop" "PNVL1" "$HS_ID")
    ID7=$(create_unit "HS_KHQLRR" "P KHQLRR" "PNVL1" "$HS_ID")

    [ -n "$ID1" ] && DEPT_COUNT=$((DEPT_COUNT + 1))
    [ -n "$ID2" ] && DEPT_COUNT=$((DEPT_COUNT + 1))
    [ -n "$ID3" ] && DEPT_COUNT=$((DEPT_COUNT + 1))
    [ -n "$ID4" ] && DEPT_COUNT=$((DEPT_COUNT + 1))
    [ -n "$ID5" ] && DEPT_COUNT=$((DEPT_COUNT + 1))
    [ -n "$ID6" ] && DEPT_COUNT=$((DEPT_COUNT + 1))
    [ -n "$ID7" ] && DEPT_COUNT=$((DEPT_COUNT + 1))
fi

# Other branches departments (3 each for now - simplified)
for BRANCH_NAME in BL PT SH BT TU DK TUY NH; do
    eval BRANCH_ID=\$${BRANCH_NAME}_ID
    if [ -n "$BRANCH_ID" ]; then
        echo "Creating $BRANCH_NAME departments..." | tee -a $LOG_FILE
        ID1=$(create_unit "${BRANCH_NAME}_BGD" "Ban Giam Doc" "PNVL2" "$BRANCH_ID")
        ID2=$(create_unit "${BRANCH_NAME}_KTNQ" "P KTNQ" "PNVL2" "$BRANCH_ID")
        ID3=$(create_unit "${BRANCH_NAME}_KH" "P KH" "PNVL2" "$BRANCH_ID")

        [ -n "$ID1" ] && DEPT_COUNT=$((DEPT_COUNT + 1))
        [ -n "$ID2" ] && DEPT_COUNT=$((DEPT_COUNT + 1))
        [ -n "$ID3" ] && DEPT_COUNT=$((DEPT_COUNT + 1))
    fi
done

echo "Departments created successfully: $DEPT_COUNT" | tee -a $LOG_FILE

# Final stats
echo "📊 FINAL RESULTS:" | tee -a $LOG_FILE
ALL_UNITS=$(curl -s "$API_BASE/Units")
TOTAL_COUNT=$(echo "$ALL_UNITS" | grep -c '"Id":')

echo "Root units: 1 (existing)" | tee -a $LOG_FILE
echo "Branches created: $BRANCH_COUNT/9" | tee -a $LOG_FILE
echo "Departments created: $DEPT_COUNT" | tee -a $LOG_FILE
echo "Total units in system: $TOTAL_COUNT" | tee -a $LOG_FILE

if [ $TOTAL_COUNT -ge 46 ]; then
    echo "🎉 SUCCESS! System has $TOTAL_COUNT units (target: 46)" | tee -a $LOG_FILE
else
    echo "⚠️ Need more units. Current: $TOTAL_COUNT, Target: 46" | tee -a $LOG_FILE
fi

echo "📅 End: $(date)" | tee -a $LOG_FILE
