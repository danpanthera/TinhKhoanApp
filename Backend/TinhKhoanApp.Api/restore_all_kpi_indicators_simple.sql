-- =====================================================
-- PHỤC HỒI CHỈ TIÊU KPI CHO TẤT CẢ 32 BẢNG
-- =====================================================

-- Xóa tất cả indicators hiện có
DELETE FROM KpiIndicators;

-- Tạo chỉ tiêu cho tất cả bảng từ 1-32 (đơn giản hóa)
DECLARE @tableId INT = 1;
DECLARE @indicatorCount INT;

WHILE @tableId <= 32
BEGIN
    -- Tùy vào tableId mà tạo số lượng chỉ tiêu khác nhau
    IF @tableId IN (1, 2, 3, 4, 10, 15) -- KHDN/KHCN/CBTD/PGD: 8 chỉ tiêu
        SET @indicatorCount = 8;
    ELSE IF @tableId IN (5, 6, 7, 8, 9, 18, 22) -- KHQLRR/KTNQ/GDV: 6 chỉ tiêu
        SET @indicatorCount = 6;
    ELSE IF @tableId IN (13, 14, 16, 19, 20, 21) -- PGD/CNL2: 9 chỉ tiêu
        SET @indicatorCount = 9;
    ELSE IF @tableId = 11 -- IT/TH/KTGS: 5 chỉ tiêu
        SET @indicatorCount = 5;
    ELSE IF @tableId = 12 -- CB IT/TH/KTGS: 4 chỉ tiêu
        SET @indicatorCount = 4;
    ELSE IF @tableId = 17 -- TP KH CNL2: 11 chỉ tiêu
        SET @indicatorCount = 11;
    ELSE -- Các bảng khác: 6 chỉ tiêu
        SET @indicatorCount = 6;

    -- Tạo indicators cho bảng hiện tại
    DECLARE @i INT = 1;
    WHILE @i <= @indicatorCount
    BEGIN
        INSERT INTO KpiIndicators (TableId, IndicatorCode, IndicatorName, Description, Unit, IsActive)
        VALUES (
            @tableId,
            CONCAT('KPI_', @tableId, '_', FORMAT(@i, '00')),
            CONCAT(N'Chỉ tiêu ', @i, N' của bảng ', @tableId),
            CONCAT(N'Mô tả chi tiết chỉ tiêu ', @i, N' cho bảng KPI ID ', @tableId),
            CASE
                WHEN @i % 4 = 1 THEN N'Triệu VND'
                WHEN @i % 4 = 2 THEN N'%'
                WHEN @i % 4 = 3 THEN N'Điểm'
                ELSE N'Khách hàng'
            END,
            1
        );

        SET @i = @i + 1;
    END;

    PRINT CONCAT('✅ Đã tạo ', @indicatorCount, ' chỉ tiêu cho bảng ID ', @tableId);
    SET @tableId = @tableId + 1;
END;

-- Thống kê kết quả
SELECT
    kat.Id as TableId,
    kat.TableName,
    kat.Category,
    COUNT(ki.Id) as IndicatorCount
FROM KpiAssignmentTables kat
LEFT JOIN KpiIndicators ki ON kat.Id = ki.TableId
GROUP BY kat.Id, kat.TableName, kat.Category
ORDER BY kat.Id;

SELECT COUNT(*) as 'Total_Indicators_Created' FROM KpiIndicators;
