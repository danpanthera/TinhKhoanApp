#!/bin/bash

# Script sao lưu dữ liệu bảng LN03 trước khi thay đổi kiểu dữ liệu
# Tạo bởi: Copilot
# Ngày: $(date +"%d/%m/%Y")

echo "=== Bắt đầu sao lưu dữ liệu bảng LN03 ==="

# Thiết lập các biến
DB_SERVER="localhost,1433"
DB_NAME="TinhKhoanDB"
DB_USER="sa"
DB_PASSWORD="Dientoan@303"
BACKUP_DIR="./backups"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_FILE="$BACKUP_DIR/ln03_backup_$TIMESTAMP.sql"

# Tạo thư mục backup nếu chưa tồn tại
mkdir -p $BACKUP_DIR

# Kiểm tra kết nối
echo "Kiểm tra kết nối đến cơ sở dữ liệu..."
if ! sqlcmd -S "$DB_SERVER" -U $DB_USER -P "$DB_PASSWORD" -d $DB_NAME -Q "SELECT TOP 1 * FROM LN03" -C -N -b -t 30 -y 30 > /dev/null 2>&1; then
    echo "Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra thông tin kết nối."
    exit 1
fi

# Đếm số lượng bản ghi
RECORD_COUNT=$(sqlcmd -S "$DB_SERVER" -U $DB_USER -P "$DB_PASSWORD" -d $DB_NAME -Q "SELECT COUNT(*) FROM LN03" -C -N -b -t 30 -y 30 -h-1 | tr -d '[:space:]')
echo "Số lượng bản ghi cần sao lưu: $RECORD_COUNT"

# Sao lưu schema và dữ liệu
echo "Đang sao lưu bảng LN03 vào $BACKUP_FILE..."

# Sao lưu schema
sqlcmd -S "$DB_SERVER" -U $DB_USER -P "$DB_PASSWORD" -d $DB_NAME -Q "
SELECT 'CREATE TABLE [dbo].[LN03](' + CHAR(13) + CHAR(10) +
    STRING_AGG(
        '    [' + c.name + '] ' + 
        t.name + 
        CASE 
            WHEN t.name IN ('varchar', 'nvarchar', 'char', 'nchar') THEN '(' + CASE WHEN c.max_length = -1 THEN 'MAX' ELSE CAST(c.max_length AS VARCHAR) END + ')'
            WHEN t.name IN ('decimal', 'numeric') THEN '(' + CAST(c.precision AS VARCHAR) + ', ' + CAST(c.scale AS VARCHAR) + ')'
            ELSE ''
        END +
        CASE WHEN c.is_nullable = 1 THEN ' NULL' ELSE ' NOT NULL' END,
        ',' + CHAR(13) + CHAR(10)
    ) +
    CHAR(13) + CHAR(10) + ');'
FROM 
    sys.columns c
    JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE 
    c.object_id = OBJECT_ID('dbo.LN03')
ORDER BY 
    c.column_id;
" -C -N -b -t 30 -y 30 -o "$BACKUP_FILE"

if [ $? -ne 0 ]; then
    echo "Không thể sao lưu schema bảng LN03. Vui lòng kiểm tra kết nối và thử lại."
    exit 1
fi

# Sao lưu dữ liệu
sqlcmd -S "$DB_SERVER" -U $DB_USER -P "$DB_PASSWORD" -d $DB_NAME -Q "
SET NOCOUNT ON;
SELECT 'INSERT INTO [dbo].[LN03] VALUES(' +
    STRING_AGG(
        CASE 
            WHEN [MaChiNhanh] IS NULL THEN 'NULL'
            ELSE '''' + REPLACE([MaChiNhanh], '''', '''''') + ''''
        END + ', ' +
        CASE 
            WHEN [TenChiNhanh] IS NULL THEN 'NULL'
            ELSE '''' + REPLACE([TenChiNhanh], '''', '''''') + ''''
        END + ', ' +
        CASE 
            WHEN [MaKH] IS NULL THEN 'NULL'
            ELSE '''' + REPLACE([MaKH], '''', '''''') + ''''
        END + ', ' +
        CASE 
            WHEN [TenKH] IS NULL THEN 'NULL'
            ELSE '''' + REPLACE([TenKH], '''', '''''') + ''''
        END + ', ' +
        CASE 
            WHEN [SoHopDong] IS NULL THEN 'NULL'
            ELSE '''' + REPLACE([SoHopDong], '''', '''''') + ''''
        END + ', ' +
        CASE 
            WHEN [SoTienXLRR] IS NULL THEN 'NULL'
            ELSE '''' + REPLACE([SoTienXLRR], '''', '''''') + ''''
        END + ', ' +
        CASE 
            WHEN [NgayPhatSinhXL] IS NULL THEN 'NULL'
            ELSE '''' + REPLACE([NgayPhatSinhXL], '''', '''''') + ''''
        END + ', ' +
        CASE 
            WHEN [ThuNoSauXL] IS NULL THEN 'NULL'
            ELSE '''' + REPLACE([ThuNoSauXL], '''', '''''') + ''''
        END + ', ' +
        CASE 
            WHEN [ConLaiNgoaiBang] IS NULL THEN 'NULL'
            ELSE '''' + REPLACE([ConLaiNgoaiBang], '''', '''''') + ''''
        END + ', ' +
        CASE 
            WHEN [DuNoNoiBang] IS NULL THEN 'NULL'
            ELSE '''' + REPLACE([DuNoNoiBang], '''', '''''') + ''''
        END + ', ' +
        CASE 
            WHEN [NhomNo] IS NULL THEN 'NULL'
            ELSE '''' + REPLACE([NhomNo], '''', '''''') + ''''
        END + ', ' +
        CASE 
            WHEN [MaCBTD] IS NULL THEN 'NULL'
            ELSE '''' + REPLACE([MaCBTD], '''', '''''') + ''''
        END + ', ' +
        CASE 
            WHEN [TenCBTD] IS NULL THEN 'NULL'
            ELSE '''' + REPLACE([TenCBTD], '''', '''''') + ''''
        END + ', ' +
        CASE 
            WHEN [MaPGD] IS NULL THEN 'NULL'
            ELSE '''' + REPLACE([MaPGD], '''', '''''') + ''''
        END + ', ' +
        CASE 
            WHEN [TaiKhoanHachToan] IS NULL THEN 'NULL'
            ELSE '''' + REPLACE([TaiKhoanHachToan], '''', '''''') + ''''
        END + ', ' +
        CASE 
            WHEN [RefNo] IS NULL THEN 'NULL'
            ELSE '''' + REPLACE([RefNo], '''', '''''') + ''''
        END + ', ' +
        CASE 
            WHEN [LoaiNguonVon] IS NULL THEN 'NULL'
            ELSE '''' + REPLACE([LoaiNguonVon], '''', '''''') + ''''
        END
    , '') + ');' AS InsertStatement
FROM [dbo].[LN03]
FOR JSON PATH;
" -C -N -b -t 30 -y 30 >> "$BACKUP_FILE"

if [ $? -ne 0 ]; then
    echo "Không thể sao lưu dữ liệu bảng LN03. Vui lòng kiểm tra kết nối và thử lại."
    exit 1
fi

# Tạo script khôi phục
cat > "$BACKUP_DIR/restore_ln03_backup.sh" << RESTORE_SCRIPT
#!/bin/bash

# Script khôi phục dữ liệu bảng LN03 từ bản sao lưu
# Tạo tự động vào: $(date +"%d/%m/%Y %H:%M:%S")

echo "=== Bắt đầu khôi phục dữ liệu bảng LN03 ==="

# Thiết lập các biến
DB_SERVER="$DB_SERVER"
DB_NAME="$DB_NAME"
DB_USER="$DB_USER"
DB_PASSWORD="$DB_PASSWORD"
BACKUP_FILE="$BACKUP_FILE"

# Kiểm tra file backup
if [ ! -f "\$BACKUP_FILE" ]; then
    echo "File backup \$BACKUP_FILE không tồn tại. Không thể khôi phục."
    exit 1
fi

# Kiểm tra kết nối
echo "Kiểm tra kết nối đến cơ sở dữ liệu..."
if ! sqlcmd -S "\$DB_SERVER" -U \$DB_USER -P "\$DB_PASSWORD" -d \$DB_NAME -Q "SELECT 1" -C -N -b -t 30 -y 30 > /dev/null 2>&1; then
    echo "Không thể kết nối đến cơ sở dữ liệu. Vui lòng kiểm tra thông tin kết nối."
    exit 1
fi

# Xóa bảng LN03 hiện tại
echo "Đang xóa bảng LN03 hiện tại..."
sqlcmd -S "\$DB_SERVER" -U \$DB_USER -P "\$DB_PASSWORD" -d \$DB_NAME -Q "
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LN03')
BEGIN
    DROP TABLE [dbo].[LN03];
END
" -C -N -b -t 30 -y 30

if [ \$? -ne 0 ]; then
    echo "Không thể xóa bảng LN03 hiện tại. Vui lòng kiểm tra quyền truy cập."
    exit 1
fi

# Khôi phục từ backup
echo "Đang khôi phục bảng LN03 từ \$BACKUP_FILE..."
sqlcmd -S "\$DB_SERVER" -U \$DB_USER -P "\$DB_PASSWORD" -d \$DB_NAME -i "\$BACKUP_FILE" -C -N -b -t 30 -y 30

if [ \$? -ne 0 ]; then
    echo "Không thể khôi phục bảng LN03. Vui lòng kiểm tra file backup."
    exit 1
fi

# Kiểm tra sau khi khôi phục
RECORD_COUNT=\$(sqlcmd -S "\$DB_SERVER" -U \$DB_USER -P "\$DB_PASSWORD" -d \$DB_NAME -Q "SELECT COUNT(*) FROM LN03" -C -N -b -t 30 -y 30 -h-1 | tr -d '[:space:]')
echo "Số lượng bản ghi sau khi khôi phục: \$RECORD_COUNT"

echo "=== Hoàn tất quá trình khôi phục dữ liệu ==="
RESTORE_SCRIPT

chmod +x "$BACKUP_DIR/restore_ln03_backup.sh"

echo "=== Hoàn tất quá trình sao lưu dữ liệu ==="
echo "Dữ liệu đã được sao lưu vào: $BACKUP_FILE"
echo "Script khôi phục đã được tạo: $BACKUP_DIR/restore_ln03_backup.sh"
echo ""
echo "Lưu ý: Trước khi thực hiện script khôi phục, hãy kiểm tra thông tin kết nối và cấu hình trong script."
