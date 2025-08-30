# Hướng dẫn chi tiết cập nhật cơ sở dữ liệu RR01

## 1. Chuẩn bị

### 1.1. Tạo bản backup cơ sở dữ liệu
```sql
-- Tạo bản backup đầy đủ trước khi thực hiện thay đổi
BACKUP DATABASE KhoanApp 
TO DISK = 'D:\Backups\KhoanApp_BeforeRR01TypeFix.bak' 
WITH FORMAT, INIT, NAME = 'KhoanApp-Full Database Backup before RR01 data type changes';
```

### 1.2. Kiểm tra dữ liệu hiện tại
```sql
-- Kiểm tra dữ liệu hiện tại trước khi thay đổi
SELECT TOP 100 * FROM RR01 ORDER BY CREATED_DATE DESC;

-- Kiểm tra cấu trúc bảng hiện tại
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    CHARACTER_MAXIMUM_LENGTH, 
    IS_NULLABLE
FROM 
    INFORMATION_SCHEMA.COLUMNS
WHERE 
    TABLE_NAME = 'RR01'
ORDER BY 
    ORDINAL_POSITION;
```

## 2. Thực hiện cập nhật cấu trúc

### 2.1. Tạo bảng backup
```sql
-- Tạo bảng backup cho RR01
SELECT * INTO RR01_Backup_BeforeTypeFix FROM RR01;
```

### 2.2. Tắt tính năng temporal table
```sql
-- Tắt tạm thời tính năng temporal table
ALTER TABLE RR01 SET (SYSTEM_VERSIONING = OFF);
```

### 2.3. Cập nhật các cột kiểu số
```sql
-- Cập nhật các cột kiểu số sang decimal(18,2)
ALTER TABLE RR01 ALTER COLUMN SO_LDS decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_GOC_BAN_DAU decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_LAI_TICHLUY_BD decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DOC_DAUKY_DA_THU_HT decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_GOC_HIENTAI decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_LAI_HIENTAI decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_NGAN_HAN decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_TRUNG_HAN decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DUNO_DAI_HAN decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN THU_GOC decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN THU_LAI decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN BDS decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN DS decimal(18,2) NULL;
ALTER TABLE RR01 ALTER COLUMN TSK decimal(18,2) NULL;
```

### 2.4. Cập nhật các cột kiểu ngày
```sql
-- Cập nhật các cột kiểu ngày sang datetime2
ALTER TABLE RR01 ALTER COLUMN NGAY_GIAI_NGAN datetime2 NULL;
ALTER TABLE RR01 ALTER COLUMN NGAY_DEN_HAN datetime2 NULL;
ALTER TABLE RR01 ALTER COLUMN NGAY_XLRR datetime2 NULL;
```

### 2.5. Bật lại tính năng temporal table
```sql
-- Bật lại tính năng temporal table
ALTER TABLE RR01 SET (
    SYSTEM_VERSIONING = ON (
        HISTORY_TABLE = dbo.RR01_History
    )
);
```

## 3. Kiểm tra sau khi cập nhật

### 3.1. Kiểm tra cấu trúc bảng mới
```sql
-- Kiểm tra cấu trúc bảng sau khi cập nhật
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    NUMERIC_PRECISION,
    NUMERIC_SCALE,
    CHARACTER_MAXIMUM_LENGTH, 
    IS_NULLABLE
FROM 
    INFORMATION_SCHEMA.COLUMNS
WHERE 
    TABLE_NAME = 'RR01'
ORDER BY 
    ORDINAL_POSITION;
```

### 3.2. Thử nghiệm truy vấn với phép tính số học
```sql
-- Thử nghiệm truy vấn với phép tính số học
SELECT TOP 10
    MA_KH,
    TEN_KH,
    DUNO_GOC_BAN_DAU,
    DUNO_GOC_HIENTAI,
    (DUNO_GOC_BAN_DAU - DUNO_GOC_HIENTAI) AS GOC_DA_TRA,
    CASE 
        WHEN DUNO_GOC_BAN_DAU > 0 THEN 
            (DUNO_GOC_BAN_DAU - DUNO_GOC_HIENTAI) / DUNO_GOC_BAN_DAU * 100 
        ELSE 0 
    END AS TY_LE_TRA_NO,
    DUNO_NGAN_HAN + DUNO_TRUNG_HAN + DUNO_DAI_HAN AS TONG_DU_NO
FROM
    RR01
ORDER BY 
    CREATED_DATE DESC;
```

### 3.3. Thử nghiệm truy vấn với điều kiện ngày tháng
```sql
-- Thử nghiệm truy vấn với điều kiện ngày tháng
SELECT 
    COUNT(*) AS TOTAL_RECORDS,
    COUNT(CASE WHEN NGAY_GIAI_NGAN IS NOT NULL THEN 1 END) AS HAS_NGAY_GIAI_NGAN,
    MIN(NGAY_GIAI_NGAN) AS MIN_NGAY_GIAI_NGAN,
    MAX(NGAY_GIAI_NGAN) AS MAX_NGAY_GIAI_NGAN,
    COUNT(CASE WHEN NGAY_DEN_HAN IS NOT NULL THEN 1 END) AS HAS_NGAY_DEN_HAN,
    COUNT(CASE WHEN DATEDIFF(MONTH, NGAY_GIAI_NGAN, NGAY_DEN_HAN) > 12 THEN 1 END) AS LONG_TERM_LOANS
FROM 
    RR01
WHERE 
    NGAY_DL >= DATEADD(YEAR, -1, GETDATE());
```

## 4. Import dữ liệu kiểm thử

### 4.1. Chuẩn bị dữ liệu mẫu
Sử dụng tập tin `sample_rr01_test.csv` để import thử nghiệm

### 4.2. Kiểm tra dữ liệu đã import
```sql
-- Kiểm tra dữ liệu đã import
SELECT * FROM RR01 
WHERE FILE_NAME LIKE '%test%' 
ORDER BY CREATED_DATE DESC;
```

### 4.3. Xác nhận tính toán chính xác
```sql
-- Xác nhận tính toán chính xác
SELECT
    MA_KH,
    TEN_KH,
    DUNO_NGAN_HAN,
    DUNO_TRUNG_HAN,
    DUNO_DAI_HAN,
    (DUNO_NGAN_HAN + DUNO_TRUNG_HAN + DUNO_DAI_HAN) AS TOTAL_1,
    DUNO_GOC_HIENTAI AS TOTAL_2,
    CASE 
        WHEN ABS((DUNO_NGAN_HAN + DUNO_TRUNG_HAN + DUNO_DAI_HAN) - DUNO_GOC_HIENTAI) < 0.01 
        THEN 'ĐÚNG' 
        ELSE 'SAI' 
    END AS KIEM_TRA
FROM
    RR01
WHERE 
    FILE_NAME LIKE '%test%';
```

## 5. Rollback (nếu cần)

### 5.1. Quay về cấu trúc cũ
Nếu có vấn đề, có thể khôi phục từ bản backup:

```sql
-- Quay về cấu trúc cũ từ bản backup
RESTORE DATABASE KhoanApp 
FROM DISK = 'D:\Backups\KhoanApp_BeforeRR01TypeFix.bak' 
WITH REPLACE;
```

hoặc khôi phục dữ liệu từ bảng backup:

```sql
-- Xóa dữ liệu trong bảng RR01
TRUNCATE TABLE RR01;

-- Khôi phục dữ liệu từ bảng backup
INSERT INTO RR01 SELECT * FROM RR01_Backup_BeforeTypeFix;
```

## 6. Xác nhận hoàn tất

Sau khi hoàn tất cập nhật, cần xác nhận:

1. Cấu trúc bảng đã được cập nhật đúng theo yêu cầu
2. Dữ liệu vẫn được giữ nguyên và không bị mất
3. Các truy vấn số học và điều kiện ngày tháng hoạt động chính xác
4. Tính năng temporal table vẫn hoạt động bình thường
5. Không có lỗi trong quá trình import dữ liệu mới
