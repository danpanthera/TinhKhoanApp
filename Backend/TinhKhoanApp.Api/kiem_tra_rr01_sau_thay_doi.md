# Kiểm tra bảng RR01 sau khi thay đổi kiểu dữ liệu

## Kiểm tra cấu trúc cơ sở dữ liệu

### 1. Kiểm tra kiểu dữ liệu của các cột
```sql
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    CHARACTER_MAXIMUM_LENGTH,
    NUMERIC_PRECISION,
    NUMERIC_SCALE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'RR01'
ORDER BY ORDINAL_POSITION;
```

Kết quả mong đợi:
- Các cột số (SO_LDS, DUNO_GOC_BAN_DAU, v.v.) có DATA_TYPE = 'decimal'
- Các cột ngày (NGAY_GIAI_NGAN, NGAY_DEN_HAN, NGAY_XLRR) có DATA_TYPE = 'datetime2'

### 2. Kiểm tra tính năng Temporal Table
```sql
SELECT 
    name,
    temporal_type_desc,
    history_table_id,
    history_table_name
FROM sys.tables
WHERE name = 'RR01';

SELECT name FROM sys.tables WHERE name = 'RR01History';
```

Kết quả mong đợi:
- RR01 có temporal_type_desc = 'SYSTEM_VERSIONED_TEMPORAL_TABLE'
- history_table_name = 'RR01History'
- Bảng RR01History tồn tại

### 3. Kiểm tra Columnstore Index
```sql
SELECT 
    i.name as index_name,
    i.type_desc,
    OBJECT_NAME(i.object_id) as table_name
FROM sys.indexes i
WHERE i.object_id = OBJECT_ID('RR01') OR i.object_id = OBJECT_ID('RR01History');
```

Kết quả mong đợi:
- Tồn tại index với type_desc = 'CLUSTERED COLUMNSTORE'

## Kiểm tra dữ liệu

### 1. Kiểm tra tính toàn vẹn dữ liệu
```sql
-- So sánh số lượng bản ghi
SELECT COUNT(*) AS RR01_Count FROM RR01;
SELECT COUNT(*) AS RR01_Backup_Count FROM RR01_Backup_20250718;

-- So sánh giá trị của một số bản ghi mẫu
SELECT TOP 10 
    a.BRCD, a.MA_KH, a.SO_LDS, a.DUNO_GOC_HIENTAI, a.NGAY_GIAI_NGAN,
    b.BRCD, b.MA_KH, b.SO_LDS, b.DUNO_GOC_HIENTAI, b.NGAY_GIAI_NGAN
FROM RR01 a
JOIN RR01_Backup_20250718 b ON a.BRCD = b.BRCD AND a.MA_KH = b.MA_KH
ORDER BY a.NGAY_DL DESC;
```

Kết quả mong đợi:
- Số lượng bản ghi trong RR01 và bảng backup bằng nhau
- Giá trị của các cột giữ nguyên sau khi chuyển đổi kiểu dữ liệu

### 2. Kiểm tra dữ liệu NULL
```sql
-- Kiểm tra NULL trong các cột số
SELECT 
    SUM(CASE WHEN SO_LDS IS NULL THEN 1 ELSE 0 END) AS SO_LDS_NULL,
    SUM(CASE WHEN DUNO_GOC_BAN_DAU IS NULL THEN 1 ELSE 0 END) AS DUNO_GOC_BAN_DAU_NULL,
    SUM(CASE WHEN DUNO_LAI_TICHLUY_BD IS NULL THEN 1 ELSE 0 END) AS DUNO_LAI_TICHLUY_BD_NULL
FROM RR01;

-- Kiểm tra NULL trong các cột ngày
SELECT 
    SUM(CASE WHEN NGAY_GIAI_NGAN IS NULL THEN 1 ELSE 0 END) AS NGAY_GIAI_NGAN_NULL,
    SUM(CASE WHEN NGAY_DEN_HAN IS NULL THEN 1 ELSE 0 END) AS NGAY_DEN_HAN_NULL,
    SUM(CASE WHEN NGAY_XLRR IS NULL THEN 1 ELSE 0 END) AS NGAY_XLRR_NULL
FROM RR01;
```

Kết quả mong đợi:
- Số lượng NULL trong các cột phải giống với số lượng NULL trước khi chuyển đổi

### 3. Kiểm tra tính nhất quán của dữ liệu số
```sql
-- Kiểm tra tổng dư nợ
SELECT TOP 100
    BRCD, MA_KH, SO_LDS,
    DUNO_NGAN_HAN, DUNO_TRUNG_HAN, DUNO_DAI_HAN,
    (ISNULL(DUNO_NGAN_HAN, 0) + ISNULL(DUNO_TRUNG_HAN, 0) + ISNULL(DUNO_DAI_HAN, 0)) AS TONG_DUNO,
    DUNO_GOC_HIENTAI,
    ABS((ISNULL(DUNO_NGAN_HAN, 0) + ISNULL(DUNO_TRUNG_HAN, 0) + ISNULL(DUNO_DAI_HAN, 0)) - ISNULL(DUNO_GOC_HIENTAI, 0)) AS CHENH_LECH
FROM RR01
WHERE DUNO_GOC_HIENTAI IS NOT NULL
ORDER BY NGAY_DL DESC;
```

Kết quả mong đợi:
- Giá trị CHENH_LECH gần bằng 0 (có thể có chênh lệch nhỏ do làm tròn)

### 4. Kiểm tra tính nhất quán của dữ liệu ngày
```sql
-- Kiểm tra quan hệ giữa các ngày
SELECT TOP 100
    BRCD, MA_KH, SO_LDS,
    NGAY_GIAI_NGAN, NGAY_DEN_HAN,
    DATEDIFF(DAY, NGAY_GIAI_NGAN, NGAY_DEN_HAN) AS SO_NGAY_VAY
FROM RR01
WHERE NGAY_GIAI_NGAN IS NOT NULL AND NGAY_DEN_HAN IS NOT NULL
ORDER BY NGAY_DL DESC;
```

Kết quả mong đợi:
- SO_NGAY_VAY phải là số dương (NGAY_DEN_HAN sau NGAY_GIAI_NGAN)

## Kiểm tra tính năng import

### 1. Import dữ liệu mẫu
```sql
-- Xóa dữ liệu test trước đó (nếu có)
DELETE FROM RR01 WHERE BRCD = 'TEST_RR01';

-- Import dữ liệu mẫu từ file CSV
-- (Sử dụng API hoặc giao diện người dùng để import file sample_rr01_test.csv)
```

Kết quả mong đợi:
- Import thành công không có lỗi
- Dữ liệu được lưu với kiểu dữ liệu đúng

### 2. Kiểm tra dữ liệu đã import
```sql
-- Kiểm tra dữ liệu số
SELECT 
    BRCD, MA_KH, SO_LDS, DUNO_GOC_BAN_DAU, DUNO_GOC_HIENTAI,
    DUNO_NGAN_HAN, DUNO_TRUNG_HAN, DUNO_DAI_HAN
FROM RR01
WHERE BRCD = 'TEST_RR01';

-- Kiểm tra dữ liệu ngày
SELECT 
    BRCD, MA_KH, SO_LDS, 
    NGAY_GIAI_NGAN, NGAY_DEN_HAN, NGAY_XLRR
FROM RR01
WHERE BRCD = 'TEST_RR01';
```

Kết quả mong đợi:
- Dữ liệu số hiển thị đúng định dạng decimal
- Dữ liệu ngày hiển thị đúng định dạng datetime

### 3. Kiểm tra các trường hợp đặc biệt
```sql
-- Kiểm tra trường hợp số âm
SELECT * FROM RR01 WHERE DUNO_GOC_HIENTAI < 0;

-- Kiểm tra trường hợp có phần thập phân
SELECT * FROM RR01 WHERE DUNO_GOC_HIENTAI - FLOOR(DUNO_GOC_HIENTAI) > 0;

-- Kiểm tra trường hợp ngày đặc biệt (ngày cuối tháng, năm nhuận)
SELECT * FROM RR01 WHERE DAY(NGAY_GIAI_NGAN) = 29 AND MONTH(NGAY_GIAI_NGAN) = 2;
```

Kết quả mong đợi:
- Các trường hợp đặc biệt được hiển thị chính xác

## Kiểm tra hiệu năng

### 1. Kiểm tra thời gian truy vấn
```sql
-- Đo thời gian truy vấn tổng hợp dữ liệu số
SET STATISTICS TIME ON;
SELECT 
    BRCD,
    SUM(ISNULL(DUNO_GOC_HIENTAI, 0)) AS TONG_DUNO,
    AVG(ISNULL(DUNO_GOC_HIENTAI, 0)) AS DUNO_TRUNG_BINH
FROM RR01
GROUP BY BRCD;
SET STATISTICS TIME OFF;

-- Đo thời gian truy vấn dữ liệu ngày
SET STATISTICS TIME ON;
SELECT 
    MONTH(NGAY_GIAI_NGAN) AS THANG,
    YEAR(NGAY_GIAI_NGAN) AS NAM,
    COUNT(*) AS SO_LUONG,
    SUM(ISNULL(DUNO_GOC_HIENTAI, 0)) AS TONG_DUNO
FROM RR01
WHERE NGAY_GIAI_NGAN IS NOT NULL
GROUP BY YEAR(NGAY_GIAI_NGAN), MONTH(NGAY_GIAI_NGAN)
ORDER BY NAM, THANG;
SET STATISTICS TIME OFF;
```

Kết quả mong đợi:
- Thời gian thực thi truy vấn giảm hoặc tương đương so với trước khi thay đổi

### 2. Kiểm tra hiệu năng import
```sql
-- Đo thời gian import một tập dữ liệu lớn
-- (Sử dụng công cụ đo thời gian khi import file CSV có nhiều bản ghi)
```

Kết quả mong đợi:
- Thời gian import tương đương hoặc cải thiện so với trước khi thay đổi

## Kiểm tra tích hợp

### 1. Kiểm tra tích hợp với API
```
-- Gọi API lấy dữ liệu RR01
GET /api/rr01?branch=1808&top=10
```

Kết quả mong đợi:
- API trả về dữ liệu đúng định dạng
- Các cột số hiển thị dạng số, không phải chuỗi
- Các cột ngày hiển thị dạng ISO datetime

### 2. Kiểm tra tích hợp với báo cáo
```
-- Kiểm tra các báo cáo sử dụng dữ liệu RR01
```

Kết quả mong đợi:
- Các báo cáo hiển thị dữ liệu chính xác
- Tính toán trong báo cáo không bị ảnh hưởng
