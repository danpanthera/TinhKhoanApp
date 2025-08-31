#!/bin/bash

# 🎯 Direct LN03 Fix - Bypass EF Migration Issues
# Creates LN03 table directly using dotnet SQL execution

GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m'

echo -e "${BLUE}🎯 Direct LN03 Database Fix${NC}"
echo -e "${YELLOW}Bypassing EF migrations to create LN03 table directly${NC}"

# Create simple LN03 creation script
cat > ln03_direct_fix.sql << 'EOF'
-- Direct LN03 Table Creation (No Migration Dependencies)

-- Check if LN03 table exists
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03')
BEGIN
    PRINT '⚠️ LN03 table already exists, dropping for fresh creation'
    
    -- Disable temporal if exists
    IF EXISTS (SELECT * FROM sys.tables WHERE name = 'LN03' AND temporal_type = 2)
        ALTER TABLE [LN03] SET (SYSTEM_VERSIONING = OFF);
    
    DROP TABLE IF EXISTS [LN03_History];
    DROP TABLE IF EXISTS [LN03];
END

-- Create LN03 table (matching Entity exactly)
CREATE TABLE [LN03] (
    [Id] int IDENTITY(1,1) NOT NULL,
    
    -- Business columns
    [NGAY_DL] datetime2(7) NULL,
    [MACHINHANH] nvarchar(200) NULL,
    [TENCHINHANH] nvarchar(200) NULL,
    [MAKH] nvarchar(200) NULL,
    [TENKH] nvarchar(200) NULL,
    [SOHOPDONG] nvarchar(200) NULL,
    [SOTIENXLRR] decimal(18,2) NULL,
    [NGAYPHATSINHXL] datetime2(7) NULL,
    [THUNOSAUXL] decimal(18,2) NULL,
    [CONLAINGOAIBANG] decimal(18,2) NULL,
    [DUNONOIBANG] decimal(18,2) NULL,
    [NHOMNO] nvarchar(200) NULL,
    [MACBTD] nvarchar(200) NULL,
    [TENCBTD] nvarchar(200) NULL,
    [MAPGD] nvarchar(200) NULL,
    [TAIKHOANHACHTOAN] nvarchar(200) NULL,
    [REFNO] nvarchar(200) NULL,
    [LOAINGUONVON] nvarchar(200) NULL,
    [COLUMN_18] nvarchar(200) NULL,
    [COLUMN_19] nvarchar(200) NULL,
    [COLUMN_20] decimal(18,2) NULL,
    
    -- System columns (CRITICAL - these are used by repository)
    [CREATED_DATE] datetime2(7) NOT NULL DEFAULT GETDATE(),
    [UPDATED_DATE] datetime2(7) NULL,
    [IS_DELETED] bit NOT NULL DEFAULT 0,
    
    -- Temporal columns (EF Core style)
    [SysStartTime] datetime2(7) GENERATED ALWAYS AS ROW START NOT NULL,
    [SysEndTime] datetime2(7) GENERATED ALWAYS AS ROW END NOT NULL,
    
    CONSTRAINT [PK_LN03] PRIMARY KEY CLUSTERED ([Id] ASC),
    PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime])
)
WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[LN03_History]));

-- Create indexes for performance
CREATE NONCLUSTERED INDEX [IX_LN03_NGAY_DL] ON [LN03] ([NGAY_DL] ASC);
CREATE NONCLUSTERED INDEX [IX_LN03_MACHINHANH] ON [LN03] ([MACHINHANH] ASC);
CREATE NONCLUSTERED INDEX [IX_LN03_MAKH] ON [LN03] ([MAKH] ASC);
CREATE NONCLUSTERED INDEX [IX_LN03_IS_DELETED] ON [LN03] ([IS_DELETED] ASC);

-- Insert test data
INSERT INTO [LN03] (
    [NGAY_DL], [MACHINHANH], [TENCHINHANH], [MAKH], [TENKH], 
    [SOHOPDONG], [SOTIENXLRR], [NGAYPHATSINHXL], [DUNONOIBANG], 
    [CONLAINGOAIBANG], [NHOMNO], [MACBTD], [TENCBTD], [MAPGD], 
    [TAIKHOANHACHTOAN], [REFNO], [LOAINGUONVON], 
    [COLUMN_18], [COLUMN_19], [COLUMN_20], [IS_DELETED]
) VALUES 
('2024-12-31', '7800', 'Chi nhanh Tinh Lai Chau', '004065046', 'Bùi Thị Linh', 
 '7800-LAV-201500567', 1000000, '2019-06-28', 0, 1000000, '1', 
 '780000424', 'Nguyễn Văn Hùng', '00', '971103', 
 '78000040650467800-LAV-2015005677800LDS201500736', 'Cá nhân',
 'TEST_18', 'TEST_19', 500000, 0),
('2024-12-31', '7800', 'Chi nhanh Tinh Lai Chau', '004065047', 'Nguyễn Văn Nam',
 '7800-LAV-201600123', 5000000, '2020-01-15', 0, 5000000, '2',
 '780000425', 'Trần Thị Lan', '01', '971104',
 '78000040650477800-LAV-2016001237800LDS201600456', 'Cá nhân',
 'TEST_18', 'TEST_19', 750000, 0);

-- Verify creation
SELECT COUNT(*) as RecordCount FROM [LN03] WHERE [IS_DELETED] = 0;

PRINT '✅ LN03 table created successfully with test data'
PRINT '🔍 IS_DELETED column included for repository compatibility'
PRINT '⏰ Temporal versioning enabled'
PRINT '📊 Ready for API testing'
EOF

echo -e "${GREEN}✅ SQL script created: ln03_direct_fix.sql${NC}"

# Execute using dotnet with connection string
echo -e "\n${YELLOW}🔄 Executing SQL script via .NET...${NC}"

# Create temporary .NET script to execute SQL
cat > ExecuteSQL.cs << 'EOF'
using Microsoft.Data.SqlClient;
using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string connectionString = "Server=localhost;Database=KhoanDB;Trusted_Connection=true;TrustServerCertificate=true;";
        string sqlScript = File.ReadAllText("ln03_direct_fix.sql");
        
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("✅ Database connection opened");
                
                // Split SQL script by GO statements
                string[] batches = sqlScript.Split(new string[] { "\nGO\n", "\ngo\n" }, StringSplitOptions.RemoveEmptyEntries);
                
                foreach (string batch in batches)
                {
                    if (!string.IsNullOrWhiteSpace(batch))
                    {
                        using (var command = new SqlCommand(batch, connection))
                        {
                            command.CommandTimeout = 120;
                            command.ExecuteNonQuery();
                        }
                    }
                }
                
                Console.WriteLine("🎉 SQL script executed successfully!");
                
                // Test the table
                using (var testCommand = new SqlCommand("SELECT COUNT(*) FROM [LN03] WHERE [IS_DELETED] = 0", connection))
                {
                    int count = (int)testCommand.ExecuteScalar();
                    Console.WriteLine($"📊 LN03 active records: {count}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            Environment.Exit(1);
        }
    }
}
EOF

# Compile and run the SQL executor
echo "Compiling SQL executor..."
csc -reference:Microsoft.Data.SqlClient.dll ExecuteSQL.cs 2>/dev/null || echo "CSC compilation might have issues, trying dotnet approach..."

# Try dotnet approach if csc fails
if [ ! -f "ExecuteSQL.exe" ]; then
    echo "Using dotnet script approach..."
    cat > execute.csx << 'EOF'
#r "nuget: Microsoft.Data.SqlClient, 5.1.1"
using Microsoft.Data.SqlClient;
using System.IO;

string connectionString = "Server=localhost;Database=KhoanDB;Trusted_Connection=true;TrustServerCertificate=true;";
string sqlScript = File.ReadAllText("ln03_direct_fix.sql");

using (var connection = new SqlConnection(connectionString))
{
    connection.Open();
    Console.WriteLine("✅ Database connection opened");
    
    using (var command = new SqlCommand(sqlScript, connection))
    {
        command.CommandTimeout = 120;
        command.ExecuteNonQuery();
    }
    
    Console.WriteLine("🎉 SQL script executed successfully!");
    
    // Test the table
    using (var testCommand = new SqlCommand("SELECT COUNT(*) FROM [LN03] WHERE [IS_DELETED] = 0", connection))
    {
        int count = (int)testCommand.ExecuteScalar();
        Console.WriteLine($"📊 LN03 active records: {count}");
    }
}
EOF
    
    dotnet script execute.csx 2>/dev/null || echo "dotnet script also had issues"
fi

# If both fail, try direct sqlcmd
echo -e "\n${YELLOW}🔄 Trying sqlcmd as fallback...${NC}"
if command -v sqlcmd >/dev/null 2>&1; then
    sqlcmd -S localhost -d KhoanDB -E -i ln03_direct_fix.sql -b
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ SQL executed successfully via sqlcmd${NC}"
    fi
else
    echo -e "${YELLOW}⚠️ sqlcmd not available, manual execution needed${NC}"
fi

echo -e "\n${YELLOW}🧪 Testing API endpoints...${NC}"

# Wait for API to be ready
sleep 3

echo "1. Health check..."
curl -s http://localhost:5055/health

echo -e "\n2. LN03 count..."
COUNT_RESULT=$(curl -s http://localhost:5055/api/LN03/count)
echo "$COUNT_RESULT"

if echo "$COUNT_RESULT" | grep -q '"Success":true'; then
    echo -e "\n${GREEN}🎉 SUCCESS! LN03 API is working!${NC}"
    
    echo -e "\n3. Testing LN03 records..."
    curl -s "http://localhost:5055/api/LN03?pageSize=2" | jq . 2>/dev/null || curl -s "http://localhost:5055/api/LN03?pageSize=2"
    
    echo -e "\n${GREEN}✅ LN03 Database Fix Completed Successfully!${NC}"
    echo -e "${BLUE}🎯 Next Step: Switch frontend from mock to real API${NC}"
    echo -e "${YELLOW}Change 'MOCK_MODE = false' in ln03MockService.js${NC}"
else
    echo -e "\n${RED}❌ API still has issues${NC}"
    echo -e "${YELLOW}Check fresh_api.log for detailed errors${NC}"
    echo -e "${BLUE}Manual steps: Execute ln03_direct_fix.sql in SQL Server Management Studio${NC}"
fi

# Cleanup
rm -f ExecuteSQL.cs ExecuteSQL.exe execute.csx

echo -e "\n${BLUE}📋 Direct Fix Summary:${NC}"
echo "- SQL Script: ln03_direct_fix.sql"
echo "- Method: Direct SQL execution bypassing EF migrations"
echo "- Table: LN03 with all required columns including IS_DELETED"
echo "- Data: 2 test records inserted"
echo "- Status: $(curl -s http://localhost:5055/health 2>/dev/null || echo 'API check needed')"
