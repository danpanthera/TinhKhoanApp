#!/bin/bash

# Create test Excel files from CSV for testing
python3 -c "
import pandas as pd
import os

# Test LN01 file
try:
    df = pd.read_csv('/Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite/7801_LN01_20250531.csv')
    df.to_excel('/Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite/test_LN01.xlsx', index=False)
    print('Created test_LN01.xlsx')
except Exception as e:
    print(f'Error creating LN01 Excel: {e}')

# Test GL01 file  
try:
    df = pd.read_csv('/Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite/7800_GL01_20250531.csv')
    df.to_excel('/Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite/test_GL01.xlsx', index=False)
    print('Created test_GL01.xlsx')
except Exception as e:
    print(f'Error creating GL01 Excel: {e}')

# Test DP01 file
try:
    df = pd.read_csv('/Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite/7802_DP01_20250531.csv')
    df.to_excel('/Users/nguyendat/Documents/Projects/TinhKhoanApp/Frontend/tinhkhoan-app-ui-vite/test_DP01.xlsx', index=False)
    print('Created test_DP01.xlsx')
except Exception as e:
    print(f'Error creating DP01 Excel: {e}')
"
