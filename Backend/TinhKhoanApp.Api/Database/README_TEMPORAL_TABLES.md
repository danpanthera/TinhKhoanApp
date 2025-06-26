# 🚀 Hướng Dẫn Sử Dụng Temporal Tables + Columnstore Indexes

## 📋 Tổng Quan

Hệ thống đã được hoàn thiện với **Temporal Tables** và **Columnstore Indexes** cho tất cả loại dữ liệu thô theo yêu cầu anh. Đặc biệt tối ưu cho các bảng dữ liệu trong màn hình **KHO DỮ LIỆU THÔ** như:

- 📊 **7800_DT_KHKD1** - Báo cáo KHKD (DT)
- 💰 **BC57** - Sao kê Lãi dự thu
- 💳 **DPDA** - Dữ liệu sao kê phát hành thẻ
- 💵 **DP01** - Dữ liệu Tiền gửi
- 🏦 **DB01** - Sao kê TSDB và Không TSDB
- 💸 **LN01** - Dữ liệu LOAN - Danh mục tín dụng
- 📋 **GL01** - Dữ liệu bút toán GDV
- 📱 **EI01** - Dữ liệu mobile banking

## 🛠️ Các Scripts Chính

### 1. MasterTemporalSetup.sql

**Script chính để thiết lập toàn bộ hệ thống**

```sql
-- Chạy script này để thiết lập hoàn chỉnh
USE TinhKhoanDB;
GO
:r MasterTemporalSetup.sql
```

### 2. CompleteTemporalTablesSetup.sql

**Tạo mới tất cả temporal tables cho dữ liệu thô**

```sql
-- Tạo temporal tables cho tất cả loại dữ liệu
:r CompleteTemporalTablesSetup.sql
```

### 3. FixExistingTemporalTables.sql

**Sửa chữa và nâng cấp các bảng hiện có**

```sql
-- Kiểm tra và sửa chữa các bảng hiện có
:r FixExistingTemporalTables.sql
```

## 🏗️ Cấu Trúc Temporal Tables

Mỗi bảng dữ liệu thô được tạo với cấu trúc:

```sql
CREATE TABLE [dbo].[{DataType}_RawData]
(
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [ImportBatchId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [RowNumber] INT NOT NULL,
    [RawDataLine] NVARCHAR(MAX) NOT NULL,
    [ParsedData] NVARCHAR(MAX) NULL,
    [DataType] NVARCHAR(20) NOT NULL,

    -- Thông tin ngày tháng
    [StatementDate] DATE NOT NULL,
    [ImportDate] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
    [ProcessedDate] DATETIME2(7) NULL,

    -- Thông tin cơ cấu tổ chức
    [BranchCode] NVARCHAR(10) NULL,
    [DepartmentCode] NVARCHAR(10) NULL,
    [EmployeeCode] NVARCHAR(20) NULL,
    [UnitCode] NVARCHAR(10) NULL,

    -- Dữ liệu số liệu
    [Amount] DECIMAL(18,4) NULL,
    [Quantity] INT NULL,
    [Rate] DECIMAL(10,6) NULL,
    [Ratio] DECIMAL(10,6) NULL,

    -- Temporal Columns (ẩn)
    [ValidFrom] DATETIME2(7) GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
    [ValidTo] DATETIME2(7) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,

    PERIOD FOR SYSTEM_TIME ([ValidFrom], [ValidTo])
)
WITH (
    SYSTEM_VERSIONING = ON (
        HISTORY_TABLE = [dbo].[{DataType}_RawData_History],
        HISTORY_RETENTION_PERIOD = 7 YEARS
    )
);
```

## 📊 Columnstore Indexes

### Bảng Chính (Main Tables)

- **Nonclustered Columnstore Index** cho analytical queries
- Tối ưu cho việc import và update data

### Bảng Lịch Sử (History Tables)

- **Clustered Columnstore Index** cho compression và analytical performance
- Tối ưu cho long-term storage và reporting

## 🔍 Stored Procedures Hỗ Trợ

### sp_TemporalTablesReport

**Báo cáo trạng thái tất cả temporal tables**

```sql
EXEC sp_TemporalTablesReport;
```

### sp_ImportRawData

**Import dữ liệu thô vào temporal tables**

```sql
EXEC sp_ImportRawData
    @DataType = 'BC57',
    @RawDataLines = '["line1", "line2", "line3"]',
    @StatementDate = '2025-06-26',
    @BranchCode = 'AGR001',
    @SourceFileName = 'BC57_20250626.txt';
```

## 📈 Views Tổng Hợp

### vw_AllRawDataCurrent

**View tổng hợp tất cả dữ liệu thô hiện tại**

```sql
SELECT * FROM vw_AllRawDataCurrent
WHERE StatementDate >= '2025-01-01';
```

### vw_RawDataSummary

**View tổng quan về dữ liệu**

```sql
SELECT * FROM vw_RawDataSummary;
```

## 🕐 Temporal Queries

### Xem Dữ Liệu Hiện Tại

```sql
SELECT * FROM BC57_RawData;
```

### Xem Tất Cả Lịch Sử Thay Đổi

```sql
SELECT * FROM BC57_RawData FOR SYSTEM_TIME ALL;
```

### Xem Dữ Liệu Tại Thời Điểm Cụ Thể

```sql
SELECT * FROM BC57_RawData
FOR SYSTEM_TIME AS OF '2025-06-01 12:00:00';
```

### Xem Thay Đổi Trong Khoảng Thời Gian

```sql
SELECT * FROM BC57_RawData
FOR SYSTEM_TIME BETWEEN '2025-06-01' AND '2025-06-26';
```

## 🔧 Maintenance & Monitoring

### Kiểm Tra Compression Ratio

```sql
SELECT
    object_name(object_id) AS TableName,
    *
FROM sys.dm_db_column_store_row_group_physical_stats
WHERE object_name(object_id) LIKE '%_History';
```

### Rebuild Columnstore Indexes (định kỳ 3-6 tháng)

```sql
ALTER INDEX ALL ON BC57_RawData_History REBUILD;
```

### Kiểm Tra Dung Lượng

```sql
SELECT
    t.name AS TableName,
    p.rows AS RowCount,
    CAST(SUM(a.total_pages) * 8.0 / 1024 AS DECIMAL(10,2)) AS SizeMB
FROM sys.tables t
INNER JOIN sys.partitions p ON t.object_id = p.object_id
INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
WHERE t.name LIKE '%_RawData%'
GROUP BY t.name, p.rows
ORDER BY SizeMB DESC;
```

## 🎯 Lợi Ích Đạt Được

✅ **Hiệu năng cao**: Columnstore compression giảm dung lượng 5-10 lần
✅ **Audit hoàn chỉnh**: Tự động theo dõi mọi thay đổi dữ liệu
✅ **Query linh hoạt**: Truy vấn dữ liệu theo thời gian
✅ **Backup tối ưu**: History tables có compression cao
✅ **Retention tự động**: Tự động xóa dữ liệu cũ sau 7 năm
✅ **Analytical performance**: Tối ưu cho báo cáo và phân tích

## 🚨 Lưu Ý Quan Trọng

⚠️ **Không xóa History Tables**: Các bảng lịch sử được quản lý tự động
⚠️ **Temporal queries**: Sử dụng FOR SYSTEM_TIME để truy vấn lịch sử
⚠️ **Index maintenance**: Định kỳ rebuild columnstore indexes
⚠️ **Monitoring**: Theo dõi dung lượng và performance thường xuyên

## 📞 Hỗ Trợ

Nếu có vấn đề, anh có thể:

1. Chạy `sp_TemporalTablesReport` để kiểm tra trạng thái
2. Xem log trong bảng `ScriptExecutionLog`
3. Chạy lại script fix nếu cần thiết

---

**Được thực hiện bởi**: Em (siêu lập trình viên Fullstack)
**Ngày hoàn thành**: 26/06/2025
**Tuân thủ**: Temporal Tables + Columnstore Indexes cho hiệu năng tối ưu 🚀
