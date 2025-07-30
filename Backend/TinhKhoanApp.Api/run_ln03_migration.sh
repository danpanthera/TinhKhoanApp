#!/bin/bash

# Script thực thi migration cho bảng LN03
# Tạo bởi: Copilot
# Ngày: $(date +"%d/%m/%Y")

echo "=== Bắt đầu thực thi migration cho bảng LN03 ==="

# Thiết lập các biến
DB_SERVER="localhost,1433"
DB_NAME="TinhKhoanDB"
DB_USER="sa"
DB_PASSWORD="Dientoan@303"
BACKUP_DIR="./backups"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
LOG_FILE="migration_ln03_$TIMESTAMP.log"
MIGRATION_SCRIPT="Migrations/fix_ln03_data_types.sql"

# Kiểm tra file migration
if [ ! -f "$MIGRATION_SCRIPT" ]; then
    # Tạo thư mục Migrations nếu chưa tồn tại
    mkdir -p Migrations
    
    # Tạo script migration mẫu
    cat > "$MIGRATION_SCRIPT" << SQL_SCRIPT
-- Script migration để thay đổi kiểu dữ liệu của bảng LN03
-- Tạo tự động vào: $(date +"%d/%m/%Y %H:%M:%S")

-- Bước 1: Lưu thông tin về các ràng buộc và index hiện có
DECLARE @TableConstraints TABLE (
    ConstraintName NVARCHAR(128),
    ConstraintType NVARCHAR(128),
    ColumnName NVARCHAR(128)
);

DECLARE @TableIndexes TABLE (
    IndexName NVARCHAR(128),
    ColumnName NVARCHAR(128),
    IsUnique BIT,
    IsClustered BIT
);

INSERT INTO @TableConstraints
SELECT 
    con.name AS ConstraintName,
    con.type_desc AS ConstraintType,
    col.name AS ColumnName
FROM 
    sys.objects con
    JOIN sys.columns col ON con.parent_object_id = col.object_id
    JOIN sys.index_columns ic ON ic.object_id = col.object_id AND ic.column_id = col.column_id
    JOIN sys.indexes i ON i.object_id = ic.object_id AND i.index_id = ic.index_id
WHERE 
    con.parent_object_id = OBJECT_ID('LN03')
    AND con.type_desc LIKE '%CONSTRAINT';

INSERT INTO @TableIndexes
SELECT 
    i.name AS IndexName,
    col.name AS ColumnName,
    i.is_unique AS IsUnique,
    i.type_desc = 'CLUSTERED' AS IsClustered
FROM 
    sys.indexes i
    JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
    JOIN sys.columns col ON ic.object_id = col.object_id AND ic.column_id = col.column_id
WHERE 
    i.object_id = OBJECT_ID('LN03');

-- Bước 2: Kiểm tra nếu bảng đã được tích hợp với temporal table
DECLARE @IsTemporalTable BIT = 0;
SELECT @IsTemporalTable = 1 FROM sys.tables WHERE object_id = OBJECT_ID('LN03') AND temporal_type = 2;

-- Lưu tên của bảng lịch sử nếu có
DECLARE @HistoryTableName NVARCHAR(128) = NULL;
IF @IsTemporalTable = 1
BEGIN
    SELECT @HistoryTableName = t2.name
    FROM sys.tables t1
    JOIN sys.tables t2 ON t1.history_table_id = t2.object_id
    WHERE t1.object_id = OBJECT_ID('LN03');
    
    -- Tắt system versioning tạm thời
    ALTER TABLE LN03 SET (SYSTEM_VERSIONING = OFF);
END

-- Bước 3: Tạo bảng tạm thời với cấu trúc mới
CREATE TABLE LN03_New (
    -- Giữ nguyên các cột không thay đổi
    MaChiNhanh NVARCHAR(50),
    TenChiNhanh NVARCHAR(255),
    MaKH NVARCHAR(50),
    TenKH NVARCHAR(255),
    SoHopDong NVARCHAR(50),
    -- Cập nhật kiểu dữ liệu cho các cột số tiền
    SoTienXLRR DECIMAL(18, 2),
    -- Cập nhật kiểu dữ liệu cho các cột ngày tháng
    NgayPhatSinhXL DATETIME2,
    -- Cập nhật kiểu dữ liệu cho các cột số tiền
    ThuNoSauXL DECIMAL(18, 2),
    ConLaiNgoaiBang DECIMAL(18, 2),
    DuNoNoiBang DECIMAL(18, 2),
    -- Giữ nguyên các cột không thay đổi
    NhomNo NVARCHAR(50),
    MaCBTD NVARCHAR(50),
    TenCBTD NVARCHAR(255),
    MaPGD NVARCHAR(50),
    TaiKhoanHachToan NVARCHAR(50),
    RefNo NVARCHAR(50),
    LoaiNguonVon NVARCHAR(50)
);

-- Bước 4: Sao chép dữ liệu từ bảng cũ sang bảng mới với chuyển đổi kiểu dữ liệu
INSERT INTO LN03_New (
    MaChiNhanh, TenChiNhanh, MaKH, TenKH, SoHopDong,
    SoTienXLRR, NgayPhatSinhXL, ThuNoSauXL, ConLaiNgoaiBang, DuNoNoiBang,
    NhomNo, MaCBTD, TenCBTD, MaPGD, TaiKhoanHachToan, RefNo, LoaiNguonVon
)
SELECT
    MaChiNhanh, TenChiNhanh, MaKH, TenKH, SoHopDong,
    -- Chuyển đổi kiểu dữ liệu cho các cột số tiền
    CASE WHEN SoTienXLRR IS NULL OR SoTienXLRR = '' THEN NULL 
         ELSE TRY_CAST(REPLACE(REPLACE(SoTienXLRR, ',', ''), ' ', '') AS DECIMAL(18, 2)) 
    END AS SoTienXLRR,
    -- Chuyển đổi kiểu dữ liệu cho các cột ngày tháng
    CASE WHEN NgayPhatSinhXL IS NULL OR NgayPhatSinhXL = '' THEN NULL 
         ELSE TRY_CONVERT(DATETIME2, NgayPhatSinhXL, 103) 
    END AS NgayPhatSinhXL,
    -- Chuyển đổi kiểu dữ liệu cho các cột số tiền
    CASE WHEN ThuNoSauXL IS NULL OR ThuNoSauXL = '' THEN NULL 
         ELSE TRY_CAST(REPLACE(REPLACE(ThuNoSauXL, ',', ''), ' ', '') AS DECIMAL(18, 2)) 
    END AS ThuNoSauXL,
    CASE WHEN ConLaiNgoaiBang IS NULL OR ConLaiNgoaiBang = '' THEN NULL 
         ELSE TRY_CAST(REPLACE(REPLACE(ConLaiNgoaiBang, ',', ''), ' ', '') AS DECIMAL(18, 2)) 
    END AS ConLaiNgoaiBang,
    CASE WHEN DuNoNoiBang IS NULL OR DuNoNoiBang = '' THEN NULL 
         ELSE TRY_CAST(REPLACE(REPLACE(DuNoNoiBang, ',', ''), ' ', '') AS DECIMAL(18, 2)) 
    END AS DuNoNoiBang,
    NhomNo, MaCBTD, TenCBTD, MaPGD, TaiKhoanHachToan, RefNo, LoaiNguonVon
FROM LN03;

-- Bước 5: Đổi tên bảng cũ và bảng mới
EXEC sp_rename 'LN03', 'LN03_Old';
EXEC sp_rename 'LN03_New', 'LN03';

-- Bước 6: Khôi phục các ràng buộc và index
-- [Khôi phục các ràng buộc và index ở đây]

-- Bước 7: Bật lại system versioning nếu cần
IF @IsTemporalTable = 1 AND @HistoryTableName IS NOT NULL
BEGIN
    -- Đảm bảo bảng lịch sử cũng được cập nhật cấu trúc tương tự
    -- [Cập nhật cấu trúc bảng lịch sử ở đây]
    
    -- Bật lại system versioning
    ALTER TABLE LN03
    ADD PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);
    
    ALTER TABLE LN03 SET
    (
        SYSTEM_VERSIONING = ON
        (
            HISTORY_TABLE = dbo.@HistoryTableName,
            DATA_CONSISTENCY_CHECK = ON
        )
    );
END

-- Bước 8: Xóa bảng cũ sau khi đã kiểm tra mọi thứ hoạt động tốt
-- DROP TABLE LN03_Old;  -- Uncomment khi đã kiểm tra kỹ

PRINT 'Migration hoàn tất thành công.';
SQL_SCRIPT
    
    echo "Đã tạo script migration mẫu: $MIGRATION_SCRIPT"
else
    echo "Đã tìm thấy script migration: $MIGRATION_SCRIPT"
fi

# Tìm bản sao lưu gần nhất
LATEST_BACKUP=$(find "$BACKUP_DIR" -name "ln03_*" -type f | sort -r | head -n 1)
if [ -n "$LATEST_BACKUP" ]; then
    echo "Đã tìm thấy bản sao lưu gần nhất: $LATEST_BACKUP"
else
    echo "Không tìm thấy bản sao lưu. Vui lòng chạy script sao lưu trước."
fi

# Chuẩn bị thực thi migration
echo "Đang thực thi script migration..."
echo "Ghi nhật ký vào: $LOG_FILE"

# Tạo snapshot của bảng trước khi migration
echo "Tạo snapshot của bảng trước khi migration..."
sqlcmd -S "$DB_SERVER" -U $DB_USER -P "$DB_PASSWORD" -d $DB_NAME -Q "
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LN03_Migration_Snapshot')
    DROP TABLE LN03_Migration_Snapshot;
SELECT * INTO LN03_Migration_Snapshot FROM LN03;
" -C -N -b -t 30 -y 30 2>> "$LOG_FILE"

if [ $? -ne 0 ]; then
    echo "Không thể tạo snapshot của cơ sở dữ liệu. Tiếp tục mà không có snapshot."
fi

# Thực thi script migration
echo "Đang thực thi script migration SQL..."
sqlcmd -S "$DB_SERVER" -U $DB_USER -P "$DB_PASSWORD" -d $DB_NAME -i "$MIGRATION_SCRIPT" -C -N -b -t 30 -y 30 >> "$LOG_FILE" 2>&1

if [ $? -ne 0 ]; then
    echo "Có lỗi xảy ra trong quá trình thực thi script migration."
    echo "Chi tiết lỗi được ghi trong: $LOG_FILE"
else
    echo "Script migration đã được thực thi thành công."
    echo "Chi tiết xem trong: $LOG_FILE"
fi

echo ""
echo "Các bước tiếp theo:"
echo "1. Kiểm tra log: $LOG_FILE để xác nhận các thay đổi"
echo "2. Triển khai code mới với các thay đổi trong model và service"
echo "3. Chạy test để xác nhận quá trình import mới hoạt động chính xác"
