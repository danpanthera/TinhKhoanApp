-- Create basic tables for Units, Positions, and Employees
USE [TinhKhoanDB];
GO

-- Create Units table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Units')
BEGIN
    CREATE TABLE [Units] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [UnitCode] nvarchar(50) NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] datetime2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Units] PRIMARY KEY ([Id])
    );
    
    PRINT 'Created Units table';
    
    -- Insert default unit
    INSERT INTO [Units] ([Name], [UnitCode], [Description], [IsActive])
    VALUES (N'Ban Giám Đốc', 'BGD', N'Ban Giám Đốc Agribank Lai Châu', 1);
    
    PRINT 'Inserted default Unit';
END

-- Create Positions table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Positions')
BEGIN
    CREATE TABLE [Positions] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] datetime2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Positions] PRIMARY KEY ([Id])
    );
    
    PRINT 'Created Positions table';
    
    -- Insert default position
    INSERT INTO [Positions] ([Name], [Description], [IsActive])
    VALUES (N'Giám Đốc', N'Giám Đốc Agribank Lai Châu', 1);
    
    PRINT 'Inserted default Position';
END

-- Create Employees table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Employees')
BEGIN
    CREATE TABLE [Employees] (
        [Id] int NOT NULL IDENTITY,
        [EmployeeCode] nvarchar(50) NULL,
        [CBCode] nvarchar(50) NULL,
        [FullName] nvarchar(200) NOT NULL,
        [Username] nvarchar(100) NOT NULL,
        [PasswordHash] nvarchar(200) NULL,
        [Email] nvarchar(200) NULL,
        [PhoneNumber] nvarchar(50) NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        [UnitId] int NULL,
        [PositionId] int NULL,
        [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] datetime2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Employees_Units_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [Units] ([Id]),
        CONSTRAINT [FK_Employees_Positions_PositionId] FOREIGN KEY ([PositionId]) REFERENCES [Positions] ([Id])
    );
    
    PRINT 'Created Employees table';
    
    -- Insert admin user
    INSERT INTO [Employees] (
        [EmployeeCode], 
        [CBCode], 
        [FullName], 
        [Username], 
        [PasswordHash], 
        [Email], 
        [PhoneNumber], 
        [IsActive], 
        [UnitId], 
        [PositionId]
    ) VALUES (
        'ADMIN001',
        '999999999', 
        N'Quản trị viên hệ thống',
        'admin',
        '$2a$11$XJSn7JjXTXUVBHrNhCDtm.9t.0qAH6DynBgBZk.pdVoEt8Tgo/U7i', -- "admin123"
        'admin@agribank.com.vn',
        '0999999999',
        1,
        1, 
        1
    );
    
    PRINT 'Inserted admin user';
END

PRINT 'DB setup complete';
