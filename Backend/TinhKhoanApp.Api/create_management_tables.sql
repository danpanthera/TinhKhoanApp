USE TinhKhoanDB;

-- 1. Tạo bảng Positions
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Positions')
BEGIN
    CREATE TABLE [Positions] (
        [Id] int NOT NULL IDENTITY(1,1),
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(500) NULL,
        [Level] int NOT NULL DEFAULT(1),
        [IsActive] bit NOT NULL DEFAULT(1),
        [CreatedDate] datetime2 NOT NULL DEFAULT(GETDATE()),
        [ModifiedDate] datetime2 NULL,
        [IsDeleted] bit NOT NULL DEFAULT(0),
        CONSTRAINT [PK_Positions] PRIMARY KEY ([Id])
    );
    PRINT 'Created table: Positions';
END

-- 2. Tạo bảng Units
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Units')
BEGIN
    CREATE TABLE [Units] (
        [Id] int NOT NULL IDENTITY(1,1),
        [Code] nvarchar(20) NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Type] nvarchar(50) NOT NULL,
        [ParentUnitId] int NULL,
        [SortOrder] int NOT NULL DEFAULT(0),
        [IsActive] bit NOT NULL DEFAULT(1),
        [CreatedDate] datetime2 NOT NULL DEFAULT(GETDATE()),
        [ModifiedDate] datetime2 NULL,
        [IsDeleted] bit NOT NULL DEFAULT(0),
        CONSTRAINT [PK_Units] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Units_Units_ParentUnitId] FOREIGN KEY ([ParentUnitId]) REFERENCES [Units] ([Id])
    );
    PRINT 'Created table: Units';
END

-- 3. Tạo bảng Roles
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
BEGIN
    CREATE TABLE [Roles] (
        [Id] int NOT NULL IDENTITY(1,1),
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsActive] bit NOT NULL DEFAULT(1),
        [CreatedDate] datetime2 NOT NULL DEFAULT(GETDATE()),
        [ModifiedDate] datetime2 NULL,
        [IsDeleted] bit NOT NULL DEFAULT(0),
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
    PRINT 'Created table: Roles';
END

-- 4. Tạo bảng Employees
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Employees')
BEGIN
    CREATE TABLE [Employees] (
        [Id] int NOT NULL IDENTITY(1,1),
        [EmployeeCode] nvarchar(50) NOT NULL,
        [CBCode] nvarchar(50) NULL,
        [FullName] nvarchar(200) NOT NULL,
        [Username] nvarchar(100) NULL,
        [Email] nvarchar(200) NULL,
        [PhoneNumber] nvarchar(20) NULL,
        [IsActive] bit NOT NULL DEFAULT(1),
        [UnitId] int NOT NULL,
        [PositionId] int NOT NULL,
        [CreatedDate] datetime2 NOT NULL DEFAULT(GETDATE()),
        [ModifiedDate] datetime2 NULL,
        [IsDeleted] bit NOT NULL DEFAULT(0),
        CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Employees_Units_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [Units] ([Id]),
        CONSTRAINT [FK_Employees_Positions_PositionId] FOREIGN KEY ([PositionId]) REFERENCES [Positions] ([Id])
    );
    PRINT 'Created table: Employees';
END

-- 5. Tạo bảng EmployeeRoles (Many-to-Many)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmployeeRoles')
BEGIN
    CREATE TABLE [EmployeeRoles] (
        [EmployeeId] int NOT NULL,
        [RoleId] int NOT NULL,
        [AssignedDate] datetime2 NOT NULL DEFAULT(GETDATE()),
        [AssignedBy] nvarchar(100) NULL,
        [IsActive] bit NOT NULL DEFAULT(1),
        CONSTRAINT [PK_EmployeeRoles] PRIMARY KEY ([EmployeeId], [RoleId]),
        CONSTRAINT [FK_EmployeeRoles_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_EmployeeRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
    );
    PRINT 'Created table: EmployeeRoles';
END

-- 6. Tạo bảng KhoanPeriods
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'KhoanPeriods')
BEGIN
    CREATE TABLE [KhoanPeriods] (
        [Id] int NOT NULL IDENTITY(1,1),
        [Name] nvarchar(200) NOT NULL,
        [Type] nvarchar(50) NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [Status] nvarchar(50) NOT NULL DEFAULT('Active'),
        [IsActive] bit NOT NULL DEFAULT(1),
        [CreatedDate] datetime2 NOT NULL DEFAULT(GETDATE()),
        [ModifiedDate] datetime2 NULL,
        [IsDeleted] bit NOT NULL DEFAULT(0),
        CONSTRAINT [PK_KhoanPeriods] PRIMARY KEY ([Id])
    );
    PRINT 'Created table: KhoanPeriods';
END

-- 7. Tạo index cho performance
CREATE NONCLUSTERED INDEX [IX_Employees_UnitId] ON [Employees] ([UnitId]);
CREATE NONCLUSTERED INDEX [IX_Employees_PositionId] ON [Employees] ([PositionId]);
CREATE NONCLUSTERED INDEX [IX_Units_ParentUnitId] ON [Units] ([ParentUnitId]);
CREATE UNIQUE INDEX [IX_Employees_EmployeeCode] ON [Employees] ([EmployeeCode]);
CREATE UNIQUE INDEX [IX_Units_Code] ON [Units] ([Code]);

PRINT 'All management tables created successfully!';
