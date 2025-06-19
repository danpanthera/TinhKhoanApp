#!/usr/bin/env python3
"""
Excel Test File Generator for RawData Import Testing
Creates properly formatted Excel files that match the backend's expected format
"""

import pandas as pd
import os
from datetime import datetime
import uuid

def create_ln01_test_file(filename="test_LN01_import.xlsx"):
    """Create LN01 test data in Excel format"""
    data = {
        'SourceID': [f'LN01_{str(uuid.uuid4())[:8]}' for _ in range(5)],
        'MANDT': ['800'] * 5,
        'BUKRS': ['7801', '7802', '7803', '7804', '7805'],
        'LAND1': ['VN'] * 5,
        'WAERS': ['VND'] * 5,
        'SPRAS': ['V'] * 5,
        'KTOPL': ['VFCA'] * 5,
        'WAABW': ['X'] * 5,
        'PERIV': ['K4'] * 5,
        'KOKFI': ['FI01'] * 5,
        'RCOMP': ['7801', '7802', '7803', '7804', '7805'],
        'ADRNR': ['12345', '12346', '12347', '12348', '12349'],
        'STCEG': ['123456789', '123456790', '123456791', '123456792', '123456793'],
        'FIKRS': ['FI01'] * 5,
        'XFMCO': ['X'] * 5,
        'XFMCB': ['X'] * 5,
        'XFMCA': ['X'] * 5,
        'TXJCD': ['V0'] * 5
    }
    
    df = pd.DataFrame(data)
    df.to_excel(filename, index=False)
    print(f"Created {filename} with {len(df)} LN01 records")
    return filename

def create_gl01_test_file(filename="test_GL01_import.xlsx"):
    """Create GL01 test data in Excel format"""
    data = {
        'SourceID': [f'GL01_{str(uuid.uuid4())[:8]}' for _ in range(10)],
        'MANDT': ['800'] * 10,
        'BUKRS': ['7801'] * 10,
        'GJAHR': ['2025'] * 10,
        'BELNR': [f'100000000{i}' for i in range(1, 11)],
        'BUZEI': ['001'] * 10,
        'AUGDT': [''] * 10,
        'AUGCP': [''] * 10,
        'AUGBL': [''] * 10,
        'BSCHL': ['40', '50', '40', '50', '40', '50', '40', '50', '40', '50'],
        'KOART': ['D', 'K', 'D', 'K', 'D', 'K', 'D', 'K', 'D', 'K'],
        'UMSKZ': [''] * 10,
        'UMSKS': [''] * 10,
        'ZUMSK': [''] * 10,
        'SHKZG': ['S', 'H', 'S', 'H', 'S', 'H', 'S', 'H', 'S', 'H'],
        'GSBER': [''] * 10,
        'PARGB': [''] * 10,
        'MWSKZ': [''] * 10,
        'QSSKZ': [''] * 10,
        'DMBTR': [1000000, 2000000, 1500000, 2500000, 3000000, 3500000, 4000000, 4500000, 5000000, 5500000],
        'WRBTR': [1000000, 2000000, 1500000, 2500000, 3000000, 3500000, 4000000, 4500000, 5000000, 5500000],
        'KZBTR': [1000000, 2000000, 1500000, 2500000, 3000000, 3500000, 4000000, 4500000, 5000000, 5500000],
        'PSWBT': [1000000, 2000000, 1500000, 2500000, 3000000, 3500000, 4000000, 4500000, 5000000, 5500000],
        'PSWSL': ['VND'] * 10,
        'HKONT': [f'111000000{i}' for i in range(1, 11)],
        'KUNNR': [''] * 10,
        'LIFNR': [''] * 10,
        'SGTXT': [f'Test GL01 Entry {i}' for i in range(1, 11)]
    }
    
    df = pd.DataFrame(data)
    df.to_excel(filename, index=False)
    print(f"Created {filename} with {len(df)} GL01 records")
    return filename

def create_dp01_test_file(filename="test_DP01_import.xlsx"):
    """Create DP01 test data in Excel format (assuming similar structure)"""
    data = {
        'SourceID': [f'DP01_{str(uuid.uuid4())[:8]}' for _ in range(8)],
        'MANDT': ['800'] * 8,
        'KUNNR': [f'100000000{i}' for i in range(1, 9)],
        'LAND1': ['VN'] * 8,
        'NAME1': [f'CUSTOMER {i}' for i in range(1, 9)],
        'NAME2': [f'COMPANY {i}' for i in range(1, 9)],
        'ORT01': ['Ha Noi', 'Ho Chi Minh', 'Da Nang', 'Hai Phong', 'Can Tho', 'Bien Hoa', 'Hue', 'Nha Trang'],
        'PSTLZ': ['100000', '700000', '550000', '180000', '900000', '810000', '530000', '650000'],
        'REGIO': ['01', '79', '43', '31', '92', '75', '46', '56'],
        'SPRAS': ['V'] * 8,
        'TELF1': [f'024123456{i}' for i in range(1, 9)],
        'TELFX': [f'024123456{i+10}' for i in range(1, 9)],
        'SMTP_ADDR': [f'customer{i}@company{i}.com' for i in range(1, 9)]
    }
    
    df = pd.DataFrame(data)
    df.to_excel(filename, index=False)
    print(f"Created {filename} with {len(df)} DP01 records")
    return filename

def create_invalid_test_file(filename="test_INVALID_import.xlsx"):
    """Create an invalid test file to test validation"""
    data = {
        'WrongColumn1': ['data1', 'data2'],
        'WrongColumn2': ['data3', 'data4'],
        'WrongColumn3': [123, 456]
    }
    
    df = pd.DataFrame(data)
    df.to_excel(filename, index=False)
    print(f"Created {filename} - Invalid file for testing validation")
    return filename

def main():
    """Generate all test files"""
    print("🔄 Generating Excel test files for RawData import testing...")
    print("=" * 60)
    
    # Create test files
    files_created = []
    
    try:
        files_created.append(create_ln01_test_file())
        files_created.append(create_gl01_test_file())
        files_created.append(create_dp01_test_file())
        files_created.append(create_invalid_test_file())
        
        print("\n✅ All test files created successfully!")
        print("=" * 60)
        print("Files created:")
        for file in files_created:
            size = os.path.getsize(file) if os.path.exists(file) else 0
            print(f"  📄 {file} ({size} bytes)")
        
        print("\n📋 Usage Instructions:")
        print("1. Use these Excel files with the comprehensive test HTML file")
        print("2. Select the appropriate table type in the test interface")
        print("3. Upload the corresponding Excel file")
        print("4. The backend expects Excel format (.xlsx), not CSV")
        print("5. Use the INVALID file to test validation error handling")
        
        print("\n🔍 File Contents Summary:")
        print("• LN01: Company information with SAP-style fields")
        print("• GL01: Accounting entries with financial data")
        print("• DP01: Customer information with address details")
        print("• INVALID: Wrong columns to test validation")
        
    except Exception as e:
        print(f"❌ Error creating test files: {e}")
        print("Make sure you have pandas installed: pip install pandas openpyxl")

if __name__ == "__main__":
    main()
