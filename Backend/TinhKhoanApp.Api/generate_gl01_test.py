#!/usr/bin/env python3
"""
Generate GL01 test file with high volume for performance testing
"""
import random
import datetime
from datetime import timedelta

def generate_gl01_test_file(num_records=10000):
    """Generate GL01 test file with specified number of records"""

    # Header từ file gốc
    header = '﻿STS,NGAY_GD,NGUOI_TAO,DYSEQ,TR_TYPE,DT_SEQ,TAI_KHOAN,TEN_TK,SO_TIEN_GD,POST_BR,LOAI_TIEN,DR_CR,MA_KH,TEN_KH,CCA_USRID,TR_EX_RT,REMARK,BUS_CODE,UNIT_BUS_CODE,TR_CODE,TR_NAME,REFERENCE,VALUE_DATE,DEPT_CODE,TR_TIME,COMFIRM,TRDT_TIME'

    # Sample data templates
    companies = [
        "CONG TY CO PHAN THANH TOAN QUOC GIA",
        "NGAN HANG NONG NGHIEP VA PHAT TRIEN NONG THON",
        "CONG TY TNHH DAU TU VA PHAT TRIEN",
        "CONG TY CP DICH VU CONG NGHE TIN HOC",
        "NGAN HANG THUONG MAI CO PHAN"
    ]

    tr_codes = ["CT05", "CT06", "CT07", "EI", "AP", "GL"]
    bus_codes = ["EI", "AP", "GL", "CT", "TR"]
    currencies = ["VND", "USD", "EUR"]
    dr_cr = ["D", "C"]

    filename = f"gl01_test_{num_records}_records.csv"

    with open(filename, 'w', encoding='utf-8') as f:
        # Write header
        f.write(header + '\n')

        # Generate records
        base_date = datetime.datetime(2025, 3, 1)

        for i in range(1, num_records + 1):
            # Random data generation
            sts = i
            ngay_gd = (base_date + timedelta(days=random.randint(0, 30))).strftime('%Y%m%d')
            nguoi_tao = f"7808API{random.randint(0, 9)}"
            dyseq = random.randint(1, 99)
            tr_type = random.randint(0, 9)
            dt_seq = random.randint(1, 999)
            tai_khoan = f"{random.randint(100000, 999999)}"
            ten_tk = f'"Thu, chi hộ giữa Agribank và {random.choice(companies)}"'
            so_tien_gd = random.randint(1000000, 100000000)
            post_br = random.randint(1000, 9999)
            loai_tien = random.choice(currencies)
            dr_cr_val = random.choice(dr_cr)
            ma_kh = f"1000-{random.randint(100000000, 999999999)}"
            ten_kh = random.choice(companies)
            cca_usrid = "N"
            tr_ex_rt = 1
            remark = f"'CAO HUU THUONG chuyen tien FT{random.randint(10000000000000, 99999999999999)}"
            bus_code = random.choice(bus_codes)
            unit_bus_code = random.choice(bus_codes)
            tr_code = random.choice(tr_codes)
            tr_name = "Chạy Center cut PaymentHub"
            reference = f"7808-API-{random.randint(10000000, 99999999)}"
            value_date = (base_date + timedelta(days=random.randint(-30, 0))).strftime('%Y%m%d')
            dept_code = "   "
            tr_time = (base_date + timedelta(days=random.randint(0, 30))).strftime('%d-%b-%y')
            confirm = random.randint(0, 1)
            trdt_time = (base_date + timedelta(days=random.randint(0, 30), hours=random.randint(0, 23), minutes=random.randint(0, 59), seconds=random.randint(0, 59))).strftime('%Y%m%d %H:%M:%S')

            # Write record
            record = f"{sts},{ngay_gd},{nguoi_tao},{dyseq},{tr_type},{dt_seq},{tai_khoan},{ten_tk},{so_tien_gd},{post_br},{loai_tien},{dr_cr_val},{ma_kh},{ten_kh},{cca_usrid},{tr_ex_rt},{remark},{bus_code},{unit_bus_code},{tr_code},{tr_name},{reference},{value_date},{dept_code},{tr_time},{confirm},{trdt_time}"
            f.write(record + '\n')

            # Progress indicator
            if i % 1000 == 0:
                print(f"Generated {i:,} records...")

    print(f"✅ Generated {filename} with {num_records:,} records")
    return filename

if __name__ == "__main__":
    # Generate test files với different volumes
    files = [
        generate_gl01_test_file(1000),    # 1K records
        generate_gl01_test_file(5000),    # 5K records
        generate_gl01_test_file(10000),   # 10K records
    ]

    print(f"\n🎯 Test files created:")
    for file in files:
        print(f"  - {file}")
