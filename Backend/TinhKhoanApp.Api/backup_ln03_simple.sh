#!/bin/bash

# Script sao lưu đơn giản cho bảng LN03
# Tạo bởi: Copilot
# Ngày: $(date +"%d/%m/%Y")

echo "=== Bắt đầu sao lưu đơn giản cho bảng LN03 ==="

# Thiết lập các biến
DB_SERVER="localhost,1433"
DB_NAME="TinhKhoanDB"
DB_USER="sa"
DB_PASSWORD="Dientoan@303"
BACKUP_DIR="./backups"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_FILE="$BACKUP_DIR/ln03_structure_backup_$TIMESTAMP.sql"
DATA_BACKUP_FILE="$BACKUP_DIR/ln03_data_backup_$TIMESTAMP.csv"

# Tạo thư mục backup nếu chưa tồn tại
mkdir -p $BACKUP_DIR

# Kiểm tra cấu trúc bảng LN03
echo "Đang sao lưu cấu trúc bảng LN03..."
sqlcmd -S "$DB_SERVER" -U $DB_USER -P "$DB_PASSWORD" -d $DB_NAME -Q "
SELECT 
    c.name AS ColumnName,
    t.name AS DataType,
    c.max_length AS MaxLength,
    c.precision AS Precision,
    c.scale AS Scale,
    c.is_nullable AS IsNullable
FROM 
    sys.columns c
    JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE 
    c.object_id = OBJECT_ID('dbo.LN03')
ORDER BY 
    c.column_id;
" -C -N -b -t 30 -y 30 -o "$BACKUP_FILE"

# Kiểm tra nếu có dữ liệu
echo "Kiểm tra số lượng bản ghi trong bảng LN03..."
RECORD_COUNT=$(sqlcmd -S "$DB_SERVER" -U $DB_USER -P "$DB_PASSWORD" -d $DB_NAME -Q "SELECT COUNT(*) FROM LN03" -C -N -b -t 30 -y 30 -h-1)
echo "Số lượng bản ghi: $RECORD_COUNT"

# Xuất dữ liệu sang CSV nếu có
if [ "$RECORD_COUNT" -gt 0 ]; then
    echo "Đang xuất dữ liệu bảng LN03 sang CSV..."
    sqlcmd -S "$DB_SERVER" -U $DB_USER -P "$DB_PASSWORD" -d $DB_NAME -Q "
    SET NOCOUNT ON;
    SELECT * FROM LN03;
    " -C -N -b -t 30 -y 30 -o "$DATA_BACKUP_FILE" -s"," -W
    
    echo "Dữ liệu đã được xuất sang: $DATA_BACKUP_FILE"
else
    echo "Không có dữ liệu trong bảng LN03 để sao lưu."
fi

# Tạo snapshot bảng (để đảm bảo an toàn khi cập nhật)
SNAPSHOT_NAME="${DB_NAME}_LN03_Snapshot_$TIMESTAMP"
echo "Đang tạo snapshot cho bảng LN03..."
sqlcmd -S "$DB_SERVER" -U $DB_USER -P "$DB_PASSWORD" -d $DB_NAME -Q "
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LN03_Backup')
BEGIN
    SELECT * INTO LN03_Backup FROM LN03;
    PRINT 'Đã tạo bảng LN03_Backup thành công.';
END
ELSE
BEGIN
    TRUNCATE TABLE LN03_Backup;
    INSERT INTO LN03_Backup SELECT * FROM LN03;
    PRINT 'Đã cập nhật bảng LN03_Backup thành công.';
END
" -C -N -b -t 30 -y 30

echo "=== Hoàn tất quá trình sao lưu đơn giản ==="
echo "Cấu trúc bảng đã được sao lưu vào: $BACKUP_FILE"
echo "Một bản sao của bảng LN03 đã được tạo trong cơ sở dữ liệu với tên LN03_Backup"
echo ""
echo "Lưu ý: Bạn có thể khôi phục dữ liệu bằng cách chạy lệnh sau:"
echo "  sqlcmd -S \"$DB_SERVER\" -U $DB_USER -P \"$DB_PASSWORD\" -d $DB_NAME -Q \"IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LN03_Backup') BEGIN DROP TABLE LN03; SELECT * INTO LN03 FROM LN03_Backup; END\""
