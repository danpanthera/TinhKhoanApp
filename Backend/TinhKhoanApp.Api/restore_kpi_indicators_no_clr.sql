-- Script tạo chỉ tiêu KPI cho tất cả 32 bảng (không dùng CLR)
DECLARE @TableId INT = 1;
DECLARE @MaxTableId INT = 32;
DECLARE @IndicatorIndex INT;
DECLARE @MaxIndicators INT;
DECLARE @IndicatorCode NVARCHAR(50);
DECLARE @IndicatorName NVARCHAR(200);
DECLARE @Description NVARCHAR(500);

-- Danh sách các loại chỉ tiêu cơ bản
DECLARE @IndicatorTypes TABLE (
    TypeCode NVARCHAR(50),
    TypeName NVARCHAR(200),
    TypeDesc NVARCHAR(500)
);

INSERT INTO @IndicatorTypes VALUES
('DOANHTHU', N'Doanh thu', N'Chỉ tiêu về doanh thu huy động và cho vay'),
('LAISUONG', N'Lãi suông', N'Chỉ tiêu về lãi suông từ hoạt động kinh doanh'),
('HUYDONG', N'Huy động vốn', N'Chỉ tiêu về khả năng huy động vốn từ khách hàng'),
('CHOVAT', N'Cho vay', N'Chỉ tiêu về hoạt động cho vay khách hàng'),
('NPLQUAHAN', N'NPL quá hạn', N'Chỉ tiêu về nợ xấu và nợ quá hạn'),
('CHIPHIHOATDONG', N'Chi phí hoạt động', N'Chỉ tiêu về chi phí hoạt động và quản lý'),
('KHACHHANG', N'Khách hàng mới', N'Chỉ tiêu về phát triển khách hàng mới'),
('SANPHAM', N'Sản phẩm dịch vụ', N'Chỉ tiêu về bán sản phẩm dịch vụ'),
('TYLENHUAN', N'Tỷ lệ nhuận', N'Chỉ tiêu về tỷ lệ nhuận từ hoạt động'),
('HIEUSUAT', N'Hiệu suất', N'Chỉ tiêu về hiệu suất công việc'),
('CHATLUONG', N'Chất lượng', N'Chỉ tiêu về chất lượng dịch vụ');

WHILE @TableId <= @MaxTableId
BEGIN
    -- Xác định số lượng chỉ tiêu cho từng bảng (4-11 chỉ tiêu)
    SET @MaxIndicators = CASE
        WHEN @TableId <= 4 THEN 8    -- Các trưởng phó phòng chính: 8 chỉ tiêu
        WHEN @TableId <= 9 THEN 6    -- Các nhân viên khác: 6 chỉ tiêu
        WHEN @TableId = 10 THEN 8    -- CBTD: 8 chỉ tiêu
        WHEN @TableId = 11 THEN 5    -- Trưởng phòng IT: 5 chỉ tiêu
        WHEN @TableId = 12 THEN 4    -- CB IT: 4 chỉ tiêu
        WHEN @TableId <= 16 THEN 9   -- Giám đốc và phó: 9 chỉ tiêu
        WHEN @TableId = 17 THEN 11   -- Phó phòng KH CNL2: 11 chỉ tiêu
        WHEN @TableId <= 21 THEN 6   -- Các trưởng phó phòng khác: 6 chỉ tiêu
        WHEN @TableId <= 23 THEN 6   -- CB và nhân viên: 6 chỉ tiêu
        ELSE 6                       -- Chi nhánh: 6 chỉ tiêu
    END;

    SET @IndicatorIndex = 1;

    WHILE @IndicatorIndex <= @MaxIndicators
    BEGIN
        -- Lấy loại chỉ tiêu theo vòng lặp (cycling through indicator types)
        SELECT TOP 1
            @IndicatorCode = TypeCode + '_' + CAST(@TableId AS VARCHAR(10)) + '_' + CAST(@IndicatorIndex AS VARCHAR(10)),
            @IndicatorName = TypeName + ' (Bảng ' + CAST(@TableId AS VARCHAR(10)) + ')',
            @Description = TypeDesc + ' - Chỉ tiêu số ' + CAST(@IndicatorIndex AS VARCHAR(10))
        FROM @IndicatorTypes
        ORDER BY ((@IndicatorIndex - 1) % 11) + 1;  -- Cycle through 11 types

        -- Chèn chỉ tiêu vào bảng KpiIndicators
        INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive, CreatedAt, UpdatedAt)
        VALUES (
            @TableId,
            @IndicatorCode,
            @IndicatorName,
            @Description,
            N'VND',
            1,
            GETDATE(),
            GETDATE()
        );

        SET @IndicatorIndex = @IndicatorIndex + 1;
    END;

    PRINT N'Đã tạo ' + CAST(@MaxIndicators AS VARCHAR(10)) + N' chỉ tiêu cho bảng ID ' + CAST(@TableId AS VARCHAR(10));
    SET @TableId = @TableId + 1;
END;

-- Hiển thị kết quả
SELECT
    kat.Id as TableId,
    kat.TableName,
    kat.Category,
    COUNT(ki.Id) as IndicatorCount
FROM KpiAssignmentTables kat
LEFT JOIN KpiIndicators ki ON kat.Id = ki.TableId
GROUP BY kat.Id, kat.TableName, kat.Category
ORDER BY kat.Id;

-- Tổng số chỉ tiêu đã tạo
SELECT COUNT(*) as Total_Indicators_Created
FROM KpiIndicators;
