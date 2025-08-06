-- ============================================================================
-- SIMPLE KPI RESTORATION - BASIC STRUCTURE ONLY
-- Date: August 6, 2025
-- Description: Restore basic KPI structure for testing
-- ============================================================================

-- Create KPI Assignment Tables (32 tables)
INSERT INTO KpiAssignmentTables (TableType, TableName, Description, Category, IsActive, CreatedDate) VALUES
-- First 5 CANBO tables
(1, 'TruongphongKhdn', N'KPI Trưởng phòng Khách hàng Doanh nghiệp', 'CANBO', 1, GETDATE()),
(1, 'TruongphongKhcn', N'KPI Trưởng phòng Khách hàng Cá nhân', 'CANBO', 1, GETDATE()),
(1, 'PhophongKhdn', N'KPI Phó phòng Khách hàng Doanh nghiệp', 'CANBO', 1, GETDATE()),
(1, 'PhophongKhcn', N'KPI Phó phòng Khách hàng Cá nhân', 'CANBO', 1, GETDATE()),
(1, 'Cbtd', N'KPI Cán bộ Tín dụng', 'CANBO', 1, GETDATE()),

-- First 3 CHINHANH tables  
(2, 'HoiSo', N'KPI Hội Sở', 'CHINHANH', 1, GETDATE()),
(2, 'BinhLu', N'KPI Chi nhánh Bình Lư', 'CHINHANH', 1, GETDATE()),
(2, 'PhongTho', N'KPI Chi nhánh Phong Thổ', 'CHINHANH', 1, GETDATE());

-- Sample KPI Indicators (basic set)
-- TruongphongKhdn indicators (TableId = 1)
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(1, N'Tăng trưởng dư nợ tín dụng', 100, N'%', 1, 1, 1),
(1, N'Thu hồi nợ quá hạn', 100, N'%', 2, 1, 1),
(1, N'Huy động vốn', 100, N'Tỷ đồng', 3, 2, 1),
(1, N'Chất lượng tín dụng', 100, N'%', 4, 1, 1);

-- HoiSo indicators (TableId = 6)  
INSERT INTO KpiIndicators (TableId, IndicatorName, MaxScore, Unit, OrderIndex, ValueType, IsActive) VALUES
(6, N'Tăng trưởng tín dụng', 100, N'%', 1, 1, 1),
(6, N'Huy động vốn', 100, N'%', 2, 1, 1),
(6, N'Lợi nhuận', 100, N'%', 3, 1, 1),
(6, N'Thu dịch vụ', 100, N'%', 4, 1, 1);

PRINT 'Basic KPI System restoration completed!'
PRINT 'KPI Tables created: 8 (5 CANBO + 3 CHINHANH)'
PRINT 'KPI Indicators created: 8 (4 per table)'
PRINT 'ValueType: 1=PERCENTAGE, 2=CURRENCY, 3=NUMBER, 4=SCORE'
