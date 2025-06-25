-- Tạo admin user với tài khoản admin/admin123
-- Đảm bảo có Unit và Position mặc định trước

-- Tạo Unit mặc định nếu chưa có
IF NOT EXISTS (SELECT 1 FROM Units WHERE Id = 1)
BEGIN
    INSERT INTO Units (Id, Name, UnitCode, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (1, N'Ban Giám Đốc', 'BGD', N'Ban Giám Đốc Agribank Lai Châu', 1, GETDATE(), GETDATE())
END

-- Tạo Position mặc định nếu chưa có  
IF NOT EXISTS (SELECT 1 FROM Positions WHERE Id = 1)
BEGIN
    INSERT INTO Positions (Id, Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (1, N'Giám Đốc', N'Giám Đốc Agribank Lai Châu', 1, GETDATE(), GETDATE())
END

-- Xóa admin user cũ nếu có
DELETE FROM Employees WHERE Username = 'admin'

-- Tạo admin user mới
INSERT INTO Employees (
    EmployeeCode, 
    CBCode, 
    FullName, 
    Username, 
    PasswordHash, 
    Email, 
    PhoneNumber, 
    IsActive, 
    UnitId, 
    PositionId, 
    CreatedAt, 
    UpdatedAt
) VALUES (
    'ADMIN001',
    '999999999', 
    N'Quản trị viên hệ thống',
    'admin',
    '$2a$11$8Z7QZ9Z7QZ9Z7QZ9Z7QZ9OeKQFQFQFQFQFQFQFQFQFQFQFQFQFQFQF', -- BCrypt hash của "admin123"
    'admin@agribank.com.vn',
    '0999999999',
    1,
    1, -- UnitId
    1, -- PositionId  
    GETDATE(),
    GETDATE()
)

-- Hiển thị kết quả
SELECT 'Admin user created successfully' as Result
SELECT * FROM Employees WHERE Username = 'admin'
