# Hãy hành động như một SIÊU lập trình viên Fullstack, ASP.NET, HTML, CSS, C#, Vue.js, Node.js, Vite chuyên nghiệp!
# Luôn xưng hô là em và gọi tôi là "anh".
# Luôn chú thích các dòng code bằng tiếng việt!
# 🚀 Phương án tối ưu từ Claude Opus 4ß

## 🧠 Ý tưởng & Lý do chọn
- Thuật toán: [tên thuật toán]
- Lý do lựa chọn: [vì sao đây là giải pháp tốt nhất]
- Cấu trúc dữ liệu: [loại cấu trúc dữ liệu]

## 📌 Triển khai SCD Type 2 cho các bảng dữ liệu (tối ưu dung lượng khi import)

triển khai tối ưu hóa dữ liệu import cho module "Quản lý Dữ liệu thô" với SCD Type 2 và các tính năng theo yêu cầu.

1. Tạo cấu trúc bảng SCD Type 2 cho các bảng dữ liệu thô

Apply
-- Script tạo bảng lịch sử cho LN01 với SCD Type 2
CREATE TABLE [dbo].[LN01_History] (
    [HistoryID] BIGINT IDENTITY(1,1) NOT NULL,
    [SourceID] NVARCHAR(50) NOT NULL, -- ID gốc từ bảng nguồn
    
    -- Các cột dữ liệu từ LN01
    [MANDT] NVARCHAR(3),
    [BUKRS] NVARCHAR(4),
    [LAND1] NVARCHAR(3),
    [WAERS] NVARCHAR(5),
    [SPRAS] NVARCHAR(1),
    [KTOPL] NVARCHAR(4),
    [WAABW] NVARCHAR(2),
    [PERIV] NVARCHAR(2),
    [KOKFI] NVARCHAR(1),
    [RCOMP] NVARCHAR(6),
    [ADRNR] NVARCHAR(10),
    [STCEG] NVARCHAR(20),
    [FIKRS] NVARCHAR(4),
    [XFMCO] NVARCHAR(1),
    [XFMCB] NVARCHAR(1),
    [XFMCA] NVARCHAR(1),
    [TXJCD] NVARCHAR(15),
    
    -- Metadata SCD Type 2
    [ValidFrom] DATETIME2 NOT NULL,
    [ValidTo] DATETIME2 NOT NULL DEFAULT '9999-12-31',
    [IsCurrent] BIT NOT NULL DEFAULT 1,
    [VersionNumber] INT NOT NULL DEFAULT 1,
    [RecordHash] VARBINARY(32), -- Hash để so sánh thay đổi
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT [PK_LN01_History] PRIMARY KEY CLUSTERED ([HistoryID])
) WITH (DATA_COMPRESSION = PAGE);

-- Tạo index cho truy vấn nhanh
CREATE NONCLUSTERED INDEX [IX_LN01_History_Current] 
ON [dbo].[LN01_History] ([IsCurrent], [SourceID]) 
WHERE [IsCurrent] = 1;

CREATE NONCLUSTERED INDEX [IX_LN01_History_ValidDates] 
ON [dbo].[LN01_History] ([ValidFrom], [ValidTo]) 
INCLUDE ([SourceID], [IsCurrent]);

CREATE NONCLUSTERED INDEX [IX_LN01_History_SourceID] 
ON [dbo].[LN01_History] ([SourceID], [ValidFrom], [ValidTo]);
2. Tạo view cho dữ liệu hiện tại

Apply
-- View để truy cập nhanh dữ liệu hiện tại
CREATE VIEW [dbo].[LN01_Current] AS
SELECT 
    [SourceID],
    [MANDT], [BUKRS], [LAND1], [WAERS], [SPRAS], 
    [KTOPL], [WAABW], [PERIV], [KOKFI], [RCOMP],
    [ADRNR], [STCEG], [FIKRS], [XFMCO], [XFMCB],
    [XFMCA], [TXJCD],
    [ValidFrom], [ModifiedDate]
FROM [dbo].[LN01_History]
WHERE [IsCurrent] = 1;
3. Stored Procedure để import dữ liệu với delta detection

Apply
CREATE PROCEDURE [dbo].[sp_ImportLN01_WithDelta]
    @ImportDate DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @ImportDate IS NULL
        SET @ImportDate = GETDATE();
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Tạo bảng tạm để chứa dữ liệu import
        CREATE TABLE #TempImport (
            [SourceID] NVARCHAR(50),
            [MANDT] NVARCHAR(3),
            [BUKRS] NVARCHAR(4),
            [LAND1] NVARCHAR(3),
            [WAERS] NVARCHAR(5),
            [SPRAS] NVARCHAR(1),
            [KTOPL] NVARCHAR(4),
            [WAABW] NVARCHAR(2),
            [PERIV] NVARCHAR(2),
            [KOKFI] NVARCHAR(1),
            [RCOMP] NVARCHAR(6),
            [ADRNR] NVARCHAR(10),
            [STCEG] NVARCHAR(20),
            [FIKRS] NVARCHAR(4),
            [XFMCO] NVARCHAR(1),
            [XFMCB] NVARCHAR(1),
            [XFMCA] NVARCHAR(1),
            [TXJCD] NVARCHAR(15),
            [RecordHash] VARBINARY(32)
        );
        
        -- Insert dữ liệu từ staging table và tính hash
        INSERT INTO #TempImport
        SELECT 
            [SourceID],
            [MANDT], [BUKRS], [LAND1], [WAERS], [SPRAS], 
            [KTOPL], [WAABW], [PERIV], [KOKFI], [RCOMP],
            [ADRNR], [STCEG], [FIKRS], [XFMCO], [XFMCB],
            [XFMCA], [TXJCD],
            HASHBYTES('SHA2_256', 
                CONCAT(
                    ISNULL([MANDT],''), '|',
                    ISNULL([BUKRS],''), '|',
                    ISNULL([LAND1],''), '|',
                    ISNULL([WAERS],''), '|',
                    ISNULL([SPRAS],''), '|',
                    ISNULL([KTOPL],''), '|',
                    ISNULL([WAABW],''), '|',
                    ISNULL([PERIV],''), '|',
                    ISNULL([KOKFI],''), '|',
                    ISNULL([RCOMP],''), '|',
                    ISNULL([ADRNR],''), '|',
                    ISNULL([STCEG],''), '|',
                    ISNULL([FIKRS],''), '|',
                    ISNULL([XFMCO],''), '|',
                    ISNULL([XFMCB],''), '|',
                    ISNULL([XFMCA],''), '|',
                    ISNULL([TXJCD],'')
                )
            ) AS RecordHash
        FROM [staging].[LN01_Import];
        
        -- Update các record đã thay đổi
        UPDATE h
        SET 
            [ValidTo] = @ImportDate,
            [IsCurrent] = 0,
            [ModifiedDate] = @ImportDate
        FROM [dbo].[LN01_History] h
        INNER JOIN #TempImport t ON h.[SourceID] = t.[SourceID]
        WHERE h.[IsCurrent] = 1 
        AND h.[RecordHash] <> t.[RecordHash];
        
        -- Insert các record mới và record đã thay đổi
        INSERT INTO [dbo].[LN01_History] (
            [SourceID],
            [MANDT], [BUKRS], [LAND1], [WAERS], [SPRAS], 
            [KTOPL], [WAABW], [PERIV], [KOKFI], [RCOMP],
            [ADRNR], [STCEG], [FIKRS], [XFMCO], [XFMCB],
            [XFMCA], [TXJCD],
            [ValidFrom], [ValidTo], [IsCurrent], [VersionNumber],
            [RecordHash], [CreatedDate], [ModifiedDate]
        )
        SELECT 
            t.[SourceID],
            t.[MANDT], t.[BUKRS], t.[LAND1], t.[WAERS], t.[SPRAS], 
            t.[KTOPL], t.[WAABW], t.[PERIV], t.[KOKFI], t.[RCOMP],
            t.[ADRNR], t.[STCEG], t.[FIKRS], t.[XFMCO], t.[XFMCB],
            t.[XFMCA], t.[TXJCD],
            @ImportDate, '9999-12-31', 1,
            ISNULL(h.MaxVersion, 0) + 1,
            t.[RecordHash], @ImportDate, @ImportDate
        FROM #TempImport t
        LEFT JOIN (
            SELECT [SourceID], MAX([VersionNumber]) AS MaxVersion
            FROM [dbo].[LN01_History]
            GROUP BY [SourceID]
        ) h ON t.[SourceID] = h.[SourceID]
        WHERE NOT EXISTS (
            SELECT 1 
            FROM [dbo].[LN01_History] h2
            WHERE h2.[SourceID] = t.[SourceID]
            AND h2.[IsCurrent] = 1
            AND h2.[RecordHash] = t.[RecordHash]
        );
        
        -- Đánh dấu các record bị xóa
        UPDATE h
        SET 
            [ValidTo] = @ImportDate,
            [IsCurrent] = 0,
            [ModifiedDate] = @ImportDate
        FROM [dbo].[LN01_History] h
        WHERE h.[IsCurrent] = 1
        AND NOT EXISTS (
            SELECT 1 
            FROM #TempImport t 
            WHERE t.[SourceID] = h.[SourceID]
        );
        
        DROP TABLE #TempImport;
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
4. Function để lấy snapshot tại thời điểm cụ thể

Apply
CREATE FUNCTION [dbo].[fn_GetLN01_Snapshot]
(
    @SnapshotDate DATETIME2
)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        [SourceID],
        [MANDT], [BUKRS], [LAND1], [WAERS], [SPRAS], 
        [KTOPL], [WAABW], [PERIV], [KOKFI], [RCOMP],
        [ADRNR], [STCEG], [FIKRS], [XFMCO], [XFMCB],
        [XFMCA], [TXJCD],
        [ValidFrom], [ValidTo], [VersionNumber]
    FROM [dbo].[LN01_History]
    WHERE @SnapshotDate >= [ValidFrom] 
    AND @SnapshotDate < [ValidTo]
);
5. Tạo partition theo tháng (optional)

Apply
-- Tạo partition function và scheme
CREATE PARTITION FUNCTION [pf_MonthlyPartition] (DATETIME2)
AS RANGE RIGHT FOR VALUES 
('2024-01-01', '2024-02-01', '2024-03-01', '2024-04-01',
 '2024-05-01', '2024-06-01', '2024-07-01', '2024-08-01',
 '2024-09-01', '2024-10-01', '2024-11-01', '2024-12-01');

CREATE PARTITION SCHEME [ps_MonthlyPartition]
AS PARTITION [pf_MonthlyPartition]
ALL TO ([PRIMARY]);

-- Tạo bảng với partition
CREATE TABLE [dbo].[LN01_History_Partitioned] (
    -- Same columns as LN01_History
    CONSTRAINT [PK_LN01_History_Part] PRIMARY KEY CLUSTERED 
    ([ValidFrom], [HistoryID])
) ON [ps_MonthlyPartition]([ValidFrom]);
6. Tạo API endpoint để import dữ liệu

Apply
// src/api/raw-data/import.ts
import { Request, Response } from 'express';
import { importRawData } from '@/services/rawDataService';

export const importLN01Data = async (req: Request, res: Response) => {
  try {
    const { file, importDate } = req.body;
    
    // Validate và parse file
    const parsedData = await parseExcelFile(file);
    
    // Import vào staging table
    await bulkInsertToStaging('LN01_Import', parsedData);
    
    // Execute stored procedure để xử lý delta
    await executeProcedure('sp_ImportLN01_WithDelta', { importDate });
    
    // Lấy thống kê import
    const stats = await getImportStatistics('LN01', importDate);
    
    res.json({
      success: true,
      message: 'Import dữ liệu thành công',
      statistics: stats
    });
  } catch (error) {
    console.error('Import error:', error);
    res.status(500).json({
      success: false,
      message: 'Lỗi khi import dữ liệu',
      error: error.message
    });
  }
};
7. Update UI để hiển thị lịch sử thay đổi

Apply
// src/pages/raw-data/components/DataHistory.tsx
import React, { useState, useEffect } from 'react';
import { Table, DatePicker, Tag, Timeline } from 'antd';
import { getDataHistory } from '@/services/rawDataService';

interface DataHistoryProps {
  tableName: string;
  recordId?: string;
}

export const DataHistory: React.FC<DataHistoryProps> = ({ tableName, recordId }) => {
  const [historyData, setHistoryData] = useState([]);
  const [loading, setLoading] = useState(false);

  const columns = [
    {
      title: 'Phiên bản',
      dataIndex: 'versionNumber',
      key: 'versionNumber',
      width: 100,
    },
    {
      title: 'Có hiệu lực từ',
      dataIndex: 'validFrom',
      key: 'validFrom',
      render: (date: string) => moment(date).format('DD/MM/YYYY HH:mm'),
    },
    {
      title: 'Có hiệu lực đến',
      dataIndex: 'validTo',
      key: 'validTo',
      render: (date: string) => {
        if (date === '9999-12-31') return <Tag color="green">Hiện tại</Tag>;
        return moment(date).format('DD/MM/YYYY HH:mm');
      },
    },
    {
      title: 'Thay đổi',
      key: 'changes',
      render: (record: any) => (
        <Timeline>
          {record.changes?.map((change: any, index: number) => (
            <Timeline.Item key={index}>
              <strong>{change.field}:</strong> {change.oldValue} → {change.newValue}
            </Timeline.Item>
          ))}
        </Timeline>
      ),
    },
  ];

  useEffect(() => {
    if (recordId) {
      loadHistory();
    }
  }, [tableName, recordId]);

  const loadHistory = async () => {
    setLoading(true);
    try {
      const data = await getDataHistory(tableName, recordId);
      setHistoryData(data);
    } catch (error) {
      console.error('Error loading history:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Table
      columns={columns}
      dataSource={historyData}
      loading={loading}
      pagination={{ pageSize: 10 }}
      rowKey="historyId"
    />
  );
};
8. Tạo job schedule để import tự động hàng ngày

Apply
// src/jobs/daily-import.job.ts
import { CronJob } from 'cron';
import { importAllRawDataTables } from '@/services/rawDataService';

export const setupDailyImportJob = () => {
  // Chạy lúc 2:00 AM mỗi ngày
  const job = new CronJob('0 2 * * *', async () => {
    console.log('Starting daily raw data import...');
    
    const tables = ['LN01', 'GL01', 'KNA1', 'KNB1', 'BSEG'];
    
    for (const table of tables) {
      try {
        await importRawDataTable(table);
        console.log(`Successfully imported ${table}`);
      } catch (error) {
        console.error(`Failed to import ${table}:`, error);
        // Send notification to admin
      }
    }
  });

  job.start();
  console.log('Daily import job scheduled');
};
9. Dashboard để monitor import status

Apply
// src/pages/raw-data/components/ImportDashboard.tsx
import React, { useState, useEffect } from 'react';
import { Card, Row, Col, Statistic, Progress, Timeline, Alert } from 'antd';
import { DatabaseOutlined, CheckCircleOutlined, ClockCircleOutlined } from '@ant-design/icons';

export const ImportDashboard: React.FC = () => {
  const [importStats, setImportStats] = useState<any>({});
  const [recentImports, setRecentImports] = useState<any[]>([]);

  useEffect(() => {
    loadDashboardData();
    // Refresh every 30 seconds
    const interval = setInterval(loadDashboardData, 30000);
    return () => clearInterval(interval);
  }, []);

  const loadDashboardData = async () => {
    try {
      const [stats, imports] = await Promise.all([
        getImportStatistics(),
        getRecentImports(10)
      ]);
      setImportStats(stats);
      setRecentImports(imports);
    } catch (error) {
      console.error('Error loading dashboard:', error);
    }
  };

  return (
    <div>
      <Row gutter={16}>
        <Col span={6}>
          <Card>
            <Statistic
              title="Tổng số bảng"
              value={importStats.totalTables}
              prefix={<DatabaseOutlined />}
            />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic
              title="Import hôm nay"
              value={importStats.todayImports}
              prefix={<CheckCircleOutlined />}
              valueStyle={{ color: '#3f8600' }}
            />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic
              title="Đang xử lý"
              value={importStats.processingImports}
              prefix={<ClockCircleOutlined />}
              valueStyle={{ color: '#cf1322' }}
            />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Progress
              type="circle"
              percent={importStats.successRate}
              format={percent => `${percent}%`}
            />
            <div style={{ textAlign: 'center', marginTop: 8 }}>
              Tỷ lệ thành công
            </div>
          </Card>
        </Col>
      </Row>

      <Card title="Import gần đây" style={{ marginTop: 16 }}>
        <Timeline>
          {recentImports.map((item, index) => (
            <Timeline.Item
              key={index}
              color={item.status === 'success' ? 'green' : 'red'}
            >
              <strong>{item.tableName}</strong> - {item.recordCount} bản ghi
              <br />
              <small>{moment(item.importDate).format('DD/MM/YYYY HH:mm:ss')}</small>
              {item.deltaCount > 0 && (
                <Alert
                  message={`Phát hiện ${item.deltaCount} thay đổi`}
                  type="info"
                  showIcon
                  style={{ marginTop: 8 }}
                />
              )}
            </Timeline.Item>
          ))}
        </Timeline>
      </Card>
    </div>
  );
};
Với thiết kế này, hệ thống sẽ:

Lưu trữ hiệu quả với SCD Type 2, chỉ lưu các thay đổi thực sự
Truy vấn nhanh nhờ các index và view được tối ưu
Dễ dàng truy vết lịch sử thay đổi của từng record
Tự động hóa import hàng ngày với job schedule
Monitor real-time qua dashboard
## B. Tạo dashboard báo cáo 6 chỉ tiêu, có hiển thị chuyên sâu chi tiết khi chọn vào các chỉ tiêu này ra từng chi nhánh, từng cán bộ, Khách hàng, v.v.
tạo dashboard (hiện đại, trực quan) ngay trang chủ Home, hiển thị các thông tin chính gồm 6 chỉ tiêu: (user của 7800 được xem số liệu toàn tỉnh: sum của (7800, 7801, 7802, 7803, 7804, 7805, 7806, 7807, 7808), nhưng user của các chi nhánh con  (1 trong các chi nhánh trên ngoại trừ 7800, user ở chi nhánh nào thì chỉ xem được số liệu của chi nhánh đó (VD: user của 7808 chỉ nhìn thấy số liệu đã được lọc ra của 7808), thêm nữa cũng phân quyền tương tự với các chỉ tiêu của bảng giao khoán (chỉ được xem của phòng mình, chi nhánh mình ngoại trừ các user được cho vào group moderator)

Nguồn vốn 

Dư nợ 

Tỷ lệ nợ xấu

Thu nợ XLRR

Thu dịch vụ

Tài chính

+ 06 chỉ tiêu này sau này a sẽ cung cấp cách tính sau (nhưng về cơ bản sẽ lấy từ các file import lọc ra và tính toán)

+ mỗi một chỉ tiêu khi chọn vào sẽ ra các thông tin chi tiết (từng chi nhánh ( chi nhánh nào biến động tăng sẽ có hiển thị mũi tên xanh và giảm với mũi tên đi xuống màu đỏ, nháy vào sẽ ra CB thực hiện, Khách hàng liên quan...) cách tính sẽ đề cập sau
Giải pháp:
sẽ tạo một dashboard hiện đại và trực quan cho trang chủ với phân quyền theo chi nhánh.
lưu ý:
7800 : CnLaiChau
7801: CnTamDuong
7802: CnPhongTho
7803: CnSinHo
7804: CnMuongTe
7805: CnThanUyen
7806: CnThanhPho
7807: CnTanUyen
7808: CnNamNhun
1. Cập nhật cấu trúc database cho phân quyền chi nhánh

Apply
-- Thêm bảng để lưu thông tin chi nhánh
CREATE TABLE [dbo].[Branches] (
    [BranchCode] NVARCHAR(10) PRIMARY KEY,
    [BranchName] NVARCHAR(100) NOT NULL,
    [ParentBranchCode] NVARCHAR(10),
    [IsHeadquarter] BIT DEFAULT 0,
    [IsActive] BIT DEFAULT 1,
    [CreatedDate] DATETIME DEFAULT GETDATE()
);

-- Insert dữ liệu chi nhánh
INSERT INTO [dbo].[Branches] ([BranchCode], [BranchName], [IsHeadquarter]) VALUES
('7800', N'Agribank Chi nhánh Tỉnh Lai Châu', 1),
('7801', N'Agribank Chi nhánh Huyện Tam Đường', 0),
('7802', N'Agribank Chi nhánh Huyện Phong Thổ', 0),
('7803', N'Agribank Chi nhánh Huyện Sin Hồ', 0),
('7804', N'Agribank Chi nhánh Huyện Mường Tè', 0),
('7805', N'Agribank Chi nhánh Huyện Than Uyên', 0),
('7806', N'Agribank Chi nhánh Huyện Thành Phố', 0),
('7807', N'Agribank Chi nhánh Huyện Tân Uyên', 0),
('7808', N'Agribank Chi nhánh Huyện Nam Nhùn', 0);

-- Thêm cột BranchCode vào bảng Users
ALTER TABLE [dbo].[Users] ADD [BranchCode] NVARCHAR(10);

-- Update foreign key
ALTER TABLE [dbo].[Users] 
ADD CONSTRAINT [FK_Users_Branches] 
FOREIGN KEY ([BranchCode]) REFERENCES [dbo].[Branches]([BranchCode]);
2. Tạo service cho dashboard với phân quyền

Apply
// src/services/dashboardService.ts
import { query } from '@/lib/db';
import { getUser } from '@/lib/auth';

export interface DashboardMetric {
  name: string;
  value: number;
  change: number;
  changePercent: number;
  trend: 'up' | 'down' | 'stable';
  unit: string;
  icon: string;
  color: string;
}

export interface BranchDetail {
  branchCode: string;
  branchName: string;
  value: number;
  change: number;
  changePercent: number;
  trend: 'up' | 'down' | 'stable';
  employees: EmployeePerformance[];
  customers: CustomerDetail[];
}

export class DashboardService {
  // Lấy dữ liệu dashboard theo phân quyền
  static async getDashboardMetrics(userId: string): Promise<DashboardMetric[]> {
    const user = await getUser(userId);
    const branchFilter = await this.getBranchFilter(user);

    const metrics = await Promise.all([
      this.getCapitalSource(branchFilter),
      this.getOutstandingLoan(branchFilter),
      this.getBadDebtRatio(branchFilter),
      this.getDebtCollection(branchFilter),
      this.getServiceRevenue(branchFilter),
      this.getFinancialMetrics(branchFilter)
    ]);

    return metrics;
  }

  // Xác định filter chi nhánh dựa trên user
  private static async getBranchFilter(user: any): Promise<string[]> {
    if (user.branchCode === '7800' || user.groups?.includes('moderator')) {
      // User của chi nhánh tỉnh hoặc moderator xem được toàn bộ
      return ['7800', '7801', '7802', '7803', '7804', '7805', '7806', '7807', '7808'];
    } else {
      // User chi nhánh con chỉ xem được chi nhánh của mình
      return [user.branchCode];
    }
  }

  // Lấy chỉ tiêu Nguồn vốn
  private static async getCapitalSource(branches: string[]): Promise<DashboardMetric> {
    const result = await query(`
      SELECT 
        SUM(CurrentValue) as totalValue,
        SUM(PreviousValue) as previousValue
      FROM DashboardMetrics
      WHERE MetricType = 'CAPITAL_SOURCE'
      AND BranchCode IN (${branches.map(() => '?').join(',')})
      AND ReportDate = CAST(GETDATE() AS DATE)
    `, branches);

    const current = result[0]?.totalValue || 0;
    const previous = result[0]?.previousValue || 0;
    const change = current - previous;
    const changePercent = previous > 0 ? (change / previous) * 100 : 0;

    return {
      name: 'Nguồn vốn',
      value: current,
      change: change,
      changePercent: changePercent,
      trend: change > 0 ? 'up' : change < 0 ? 'down' : 'stable',
      unit: 'tỷ đồng',
      icon: 'bank',
      color: '#1890ff'
    };
  }

  // Lấy chỉ tiêu Dư nợ
  private static async getOutstandingLoan(branches: string[]): Promise<DashboardMetric> {
    const result = await query(`
      SELECT 
        SUM(CurrentValue) as totalValue,
        SUM(PreviousValue) as previousValue
      FROM DashboardMetrics
      WHERE MetricType = 'OUTSTANDING_LOAN'
      AND BranchCode IN (${branches.map(() => '?').join(',')})
      AND ReportDate = CAST(GETDATE() AS DATE)
    `, branches);

    const current = result[0]?.totalValue || 0;
    const previous = result[0]?.previousValue || 0;
    const change = current - previous;
    const changePercent = previous > 0 ? (change / previous) * 100 : 0;

    return {
      name: 'Dư nợ',
      value: current,
      change: change,
      changePercent: changePercent,
      trend: change > 0 ? 'up' : change < 0 ? 'down' : 'stable',
      unit: 'tỷ đồng',
      icon: 'dollar',
      color: '#52c41a'
    };
  }

  // Lấy chi tiết theo chi nhánh cho một chỉ tiêu
  static async getBranchDetails(
    metricType: string, 
    userId: string
  ): Promise<BranchDetail[]> {
    const user = await getUser(userId);
    const branches = await this.getBranchFilter(user);

    const result = await query(`
      SELECT 
        b.BranchCode,
        b.BranchName,
        dm.CurrentValue,
        dm.PreviousValue,
        dm.CurrentValue - dm.PreviousValue as Change
      FROM Branches b
      LEFT JOIN DashboardMetrics dm ON b.BranchCode = dm.BranchCode
      WHERE dm.MetricType = ?
      AND b.BranchCode IN (${branches.map(() => '?').join(',')})
      AND dm.ReportDate = CAST(GETDATE() AS DATE)
      ORDER BY dm.CurrentValue DESC
    `, [metricType, ...branches]);

    return result.map(row => ({
      branchCode: row.BranchCode,
      branchName: row.BranchName,
      value: row.CurrentValue,
      change: row.Change,
      changePercent: row.PreviousValue > 0 ? (row.Change / row.PreviousValue) * 100 : 0,
      trend: row.Change > 0 ? 'up' : row.Change < 0 ? 'down' : 'stable',
      employees: [], // Will be populated separately
      customers: []  // Will be populated separately
    }));
  }
}
3. Tạo API endpoints cho dashboard

Apply
// src/pages/api/dashboard/metrics.ts
import { NextApiRequest, NextApiResponse } from 'next';
import { DashboardService } from '@/services/dashboardService';
import { verifyToken } from '@/lib/auth';

export default async function handler(req: NextApiRequest, res: NextApiResponse) {
  try {
    const token = req.headers.authorization?.replace('Bearer ', '');
    const decoded = verifyToken(token);
    
    if (!decoded) {
      return res.status(401).json({ error: 'Unauthorized' });
    }

    const metrics = await DashboardService.getDashboardMetrics(decoded.userId);
    res.status(200).json(metrics);
  } catch (error) {
    console.error('Dashboard metrics error:', error);
    res.status(500).json({ error: 'Internal server error' });
  }
}
4. Tạo component Dashboard hiện đại

Apply
// src/components/dashboard/DashboardHome.tsx
import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { 
  TrendingUp, 
  TrendingDown, 
  DollarSign, 
  Building2, 
  AlertCircle,
  Wallet,
  PieChart,
  Activity
} from 'lucide-react';
import { useAuth } from '@/hooks/useAuth';
import { formatCurrency, formatPercent } from '@/utils/format';
import MetricCard from './MetricCard';
import BranchDetailsModal from './BranchDetailsModal';
import { DashboardMetric } from '@/services/dashboardService';

const iconMap = {
  bank: Building2,
  dollar: DollarSign,
  alert: AlertCircle,
  wallet: Wallet,
  chart: PieChart,
  activity: Activity
};

export default function DashboardHome() {
  const { user } = useAuth();
  const [metrics, setMetrics] = useState<DashboardMetric[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedMetric, setSelectedMetric] = useState<string | null>(null);
  const [showDetails, setShowDetails] = useState(false);

  useEffect(() => {
    loadDashboardData();
    // Auto refresh every 5 minutes
    const interval = setInterval(loadDashboardData, 5 * 60 * 1000);
    return () => clearInterval(interval);
  }, []);

  const loadDashboardData = async () => {
    try {
      const response = await fetch('/api/dashboard/metrics', {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });
      const data = await response.json();
      setMetrics(data);
    } catch (error) {
      console.error('Error loading dashboard:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleMetricClick = (metricName: string) => {
    setSelectedMetric(metricName);
    setShowDetails(true);
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className="p-6 space-y-6 bg-gray-50 min-h-screen">
      {/* Header */}
      <div className="bg-white rounded-lg shadow-sm p-6">
        <h1 className="text-2xl font-bold text-gray-800">Dashboard Tổng quan</h1>
        <p className="text-gray-600 mt-2">
          {user?.branchCode === '7800' 
            ? 'Dữ liệu toàn tỉnh' 
            : `Dữ liệu chi nhánh ${user?.branchName}`}
        </p>
        <p className="text-sm text-gray-500 mt-1">
          Cập nhật lúc: {new Date().toLocaleString('vi-VN')}
        </p>
      </div>

      {/* Metrics Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {metrics.map((metric, index) => (
          <motion.div
            key={metric.name}
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: index * 0.1 }}
          >
            <MetricCard
              metric={metric}
              icon={iconMap[metric.icon as keyof typeof iconMap]}
              onClick={() => handleMetricClick(metric.name)}
            />
          </motion.div>
        ))}
      </div>

      {/* Branch Details Modal */}
      {showDetails && selectedMetric && (
        <BranchDetailsModal
          metricName={selectedMetric}
          isOpen={showDetails}
          onClose={() => setShowDetails(false)}
        />
      )}
    </div>
  );
}
5. Component MetricCard với animation

Apply
// src/components/dashboard/MetricCard.tsx
import React from 'react';
import { motion } from 'framer-motion';
import { TrendingUp, TrendingDown, Minus } from 'lucide-react';
import { formatCurrency, formatPercent } from '@/utils/format';
import { DashboardMetric } from '@/services/dashboardService';

interface MetricCardProps {
  metric: DashboardMetric;
  icon: React.ElementType;
  onClick: () => void;
}

export default function MetricCard({ metric, icon: Icon, onClick }: MetricCardProps) {
  const getTrendIcon = () => {
    switch (metric.trend) {
      case 'up':
        return <TrendingUp className="w-5 h-5 text-green-500" />;
      case 'down':
        return <TrendingDown className="w-5 h-5 text-red-500" />;
      default:
        return <Minus className="w-5 h-5 text-gray-400" />;
    }
  };

  const getTrendColor = () => {
    switch (metric.trend) {
      case 'up':
        return 'text-green-600 bg-green-50';
      case 'down':
        return 'text-red-600 bg-red-50';
      default:
        return 'text-gray-600 bg-gray-50';
    }
  };

  return (
    <motion.div
      whileHover={{ scale: 1.02 }}
      whileTap={{ scale: 0.98 }}
      className="bg-white rounded-lg shadow-sm p-6 cursor-pointer hover:shadow-md transition-shadow"
      onClick={onClick}
    >
      <div className="flex items-start justify-between">
        <div className="flex-1">
          <div className="flex items-center space-x-3">
            <div 
              className="p-3 rounded-lg"
              style={{ backgroundColor: `${metric.color}20` }}
            >
              <Icon 
                className="w-6 h-6" 
                style={{ color: metric.color }}
              />
            </div>
            <h3 className="text-gray-600 font-medium">{metric.name}</h3>
          </div>

          <div className="mt-4">
            <div className="text-3xl font-bold text-gray-800">
              {formatCurrency(metric.value)}
            </div>
            <div className="text-sm text-gray-500 mt-1">{metric.unit}</div>
          </div>

          <div className={`mt-4 flex items-center space-x-2 ${getTrendColor()} px-3 py-1 rounded-full inline-flex`}>
            {getTrendIcon()}
            <span className="text-sm font-medium">
              {formatPercent(Math.abs(metric.changePercent))}
            </span>
            <span className="text-xs">
              ({metric.change > 0 ? '+' : ''}{formatCurrency(metric.change)})
            </span>
          </div>
        </div>
      </div>

      <div className="mt-4 pt-4 border-t border-gray-100">
        <div className="flex items-center justify-between text-xs text-gray-500">
          <span>So với kỳ trước</span>
          <span className="text-blue-600 hover:text-blue-700">
            Xem chi tiết →
          </span>
        </div>
      </div>
    </motion.div>
  );
}
6. Modal hiển thị chi tiết theo chi nhánh

Apply
// src/components/dashboard/BranchDetailsModal.tsx
import React, { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { X, TrendingUp, TrendingDown, Users, UserCheck } from 'lucide-react';
import { formatCurrency, formatPercent } from '@/utils/format';
import { BranchDetail } from '@/services/dashboardService';

interface BranchDetailsModalProps {
  metricName: string;
  isOpen: boolean;
  onClose: () => void;
}

export default function BranchDetailsModal({ 
  metricName, 
  isOpen, 
  onClose 
}: BranchDetailsModalProps) {
  const [branches, setBranches] = useState<BranchDetail[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedBranch, setSelectedBranch] = useState<string | null>(null);

  useEffect(() => {
    if (isOpen) {
      loadBranchDetails();
    }
  }, [isOpen, metricName]);

  const loadBranchDetails = async () => {
    try {
      const response = await fetch(`/api/dashboard/branch-details?metric=${metricName}`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });
      const data = await response.json();
      setBranches(data);
    } catch (error) {
      console.error('Error loading branch details:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <AnimatePresence>
      {isOpen && (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          className="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center p-4"
          onClick={onClose}
        >
          <motion.div
            initial={{ scale: 0.9, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            exit={{ scale: 0.9, opacity: 0 }}
            className="bg-white rounded-lg shadow-xl max-w-4xl w-full max-h-[80vh] overflow-hidden"
            onClick={(e) => e.stopPropagation()}
          >
            {/* Header */}
            <div className="bg-gradient-to-r from-blue-600 to-blue-700 text-white p-6">
              <div className="flex items-center justify-between">
                <h2 className="text-2xl font-bold">{metricName} - Chi tiết theo chi nhánh</h2>
                <button
                  onClick={onClose}
                  className="p-2 hover:bg-white/20 rounded-full transition-colors"
                >
                  <X className="w-6 h-6" />
                </button>
              </div>
            </div>

            {/* Content */}
            <div className="p-6 overflow-y-auto max-h-[calc(80vh-80px)]">
              {loading ? (
                <div className="flex justify-center items-center h-64">
                  <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                </div>
              ) : (
                <div className="space-y-4">
                  {branches.map((branch) => (
                    <motion.div
                      key={branch.branchCode}
                      whileHover={{ scale: 1.01 }}
                      className="bg-gray-50 rounded-lg p-4 cursor-pointer hover:shadow-md transition-all"
                      onClick={() => setSelectedBranch(
                        selectedBranch === branch.branchCode ? null : branch.branchCode
                      )}
                    >
                      <div className="flex items-center justify-between">
                        <div className="flex-1">
                          <h3 className="font-semibold text-gray-800">
                            {branch.branchName}
                          </h3>
                          <div className="mt-2 flex items-center space-x-4">
                            <span className="text-2xl font-bold text-gray-900">
                              {formatCurrency(branch.value)}
                            </span>
                            <div className={`flex items-center space-x-1 px-2 py-1 rounded-full text-sm ${
                              branch.trend === 'up' 
                                ? 'bg-green-100 text-green-700' 
                                : branch.trend === 'down'
                                ? 'bg-red-100 text-red-700'
                                : 'bg-gray-100 text-gray-700'
                            }`}>
                              {branch.trend === 'up' && <TrendingUp className="w-4 h-4" />}
                              {branch.trend === 'down' && <TrendingDown className="w-4 h-4" />}
                              <span>{formatPercent(Math.abs(branch.changePercent))}</span>
                            </div>
                          </div>
                        </div>
                      </div>

                      {/* Expanded details */}
                      <AnimatePresence>
                        {selectedBranch === branch.branchCode && (
                          <motion.div
                            initial={{ height: 0, opacity: 0 }}
                            animate={{ height: 'auto', opacity: 1 }}
                            exit={{ height: 0, opacity: 0 }}
                            className="mt-4 pt-4 border-t border-gray-200"
                          >
                            <div className="grid grid-cols-2 gap-4">
                              <div className="bg-white rounded-lg p-4">
                                <h4 className="font-medium text-gray-700 flex items-center">
                                  <Users className="w-5 h-5 mr-2 text-blue-600" />
                                  Cán bộ thực hiện
                                </h4>
                                <div className="mt-2 space-y-2">
                                  {branch.employees?.slice(0, 5).map((emp, idx) => (
                                    <div key={idx} className="flex items-center justify-between text-sm">
                                      <span>{emp.name}</span>
                                      <span className="font-medium">{formatCurrency(emp.value)}</span>
                                    </div>
                                  ))}
                                </div>
                              </div>

                              <div className="bg-white rounded-lg p-4">
                                <h4 className="font-medium text-gray-700 flex items-center">
                                  <UserCheck className="w-5 h-5 mr-2 text-green-600" />
                                  Khách hàng liên quan
                                </h4>
                                <div className="mt-2 space-y-2">
                                  {branch.customers?.slice(0, 5).map((cust, idx) => (
                                    <div key={idx} className="flex items-center justify-between text-sm">
                                      <span>{cust.name}</span>
                                      <span className="font-medium">{formatCurrency(cust.value)}</span>
                                    </div>
                                  ))}
                                </div>
                              </div>
                            </div>
                          </motion.div>
                        )}
                      </AnimatePresence>
                    </motion.div>
                  ))}
                </div>
              )}
            </div>
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>
  );
}
7. Update layout để hiển thị dashboard

Apply
// src/pages/index.tsx
import React from 'react';
import { useAuth } from '@/hooks/useAuth';
import DashboardHome from '@/components/dashboard/DashboardHome';
import Layout from '@/components/Layout';

export default function HomePage() {
  const { user, loading } = useAuth();

  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (!user) {
    return null; // Redirect will be handled by auth hook
  }

  return (
    <Layout>
      <DashboardHome />
    </Layout>
  );
}
8. Tạo utility functions cho format

Apply
// src/utils/format.ts
export function formatCurrency(value: number): string {
  if (value >= 1000000000) {
    return `${(value / 1000000000).toFixed(1)} tỷ`;
  } else if (value >= 1000000) {
    return `${(value / 1000000).toFixed(1)} triệu`;
  }
  return new Intl.NumberFormat('vi-VN', {
    style: 'currency',
    currency: 'VND'
  }).format(value);
}

export function formatPercent(value: number): string {
  return `${value.toFixed(2)}%`;
}

export function formatNumber(value: number): string {
  return new Intl.NumberFormat('vi-VN').format(value);
}
9. Responsive design cho mobile

Apply
// src/components/dashboard/ResponsiveDashboard.tsx
import React from 'react';
import { useMediaQuery } from '@/hooks/useMediaQuery';
import DashboardHome from './DashboardHome';
import MobileDashboard from './MobileDashboard';

export default function ResponsiveDashboard() {
  const isMobile = useMediaQuery('(max-width: 768px)');
  
  return isMobile ? <MobileDashboard /> : <DashboardHome />;
}
Dashboard này có các tính năng:

Phân quyền theo chi nhánh: User chi nhánh tỉnh (7800) xem được toàn bộ, user chi nhánh con chỉ xem được của chi nhánh mình
6 chỉ tiêu chính với icon và màu sắc riêng biệt
Animation mượt mà với Framer Motion
Responsive design cho cả desktop và mobile
Real-time update với auto-refresh
Chi tiết drill-down khi click vào từng metric
Hiển thị trend với mũi tên lên/xuống và màu sắc tương ứng

# C. khi import báo thành công nhưng lại hiện thông báo "lỗi khi lấy dữ liệu thô " và không xem được (không hiển thị nút xem), nguyên nhân và sửa lỗi  


kiểm tra và sửa lỗi liên quan đến việc import và xem dữ liệu thô.

1. Kiểm tra file ImportPage.tsx

Apply
// src/components/import/ImportPage.tsx
// Tìm phần xử lý sau khi import thành công

const handleFileUpload = async (file: File, importType: ImportType) => {
  // ... existing code ...

  try {
    const response = await fetch('/api/import', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      },
      body: formData
    });

    const result = await response.json();

    if (response.ok) {
      // Thêm delay để đảm bảo dữ liệu đã được lưu
      setTimeout(() => {
        loadImportHistory();
        loadRawData(importType.id);
      }, 1000);
      
      toast.success(`Import ${importType.name} thành công!`);
    } else {
      toast.error(result.error || 'Lỗi khi import file');
    }
  } catch (error) {
    console.error('Upload error:', error);
    toast.error('Lỗi khi upload file');
  }
};

// Sửa hàm loadRawData
const loadRawData = async (typeId: string) => {
  try {
    console.log('Loading raw data for type:', typeId);
    
    const response = await fetch(`/api/import/raw-data?type=${typeId}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    console.log('Raw data loaded:', data);
    
    setRawData(prevState => ({
      ...prevState,
      [typeId]: data
    }));
  } catch (error) {
    console.error('Error loading raw data:', error);
    toast.error('Lỗi khi lấy dữ liệu thô');
    
    // Set empty data to show the view button
    setRawData(prevState => ({
      ...prevState,
      [typeId]: []
    }));
  }
};
2. Kiểm tra API endpoint raw-data

Apply
// src/pages/api/import/raw-data.ts
import { NextApiRequest, NextApiResponse } from 'next';
import { verifyToken } from '@/lib/auth';
import { query } from '@/lib/db';

export default async function handler(req: NextApiRequest, res: NextApiResponse) {
  if (req.method !== 'GET') {
    return res.status(405).json({ error: 'Method not allowed' });
  }

  try {
    const token = req.headers.authorization?.replace('Bearer ', '');
    const decoded = verifyToken(token);
    
    if (!decoded) {
      return res.status(401).json({ error: 'Unauthorized' });
    }

    const { type } = req.query;
    
    if (!type) {
      return res.status(400).json({ error: 'Import type is required' });
    }

    // Lấy bảng tương ứng với type
    const tableMap: Record<string, string> = {
      'customer_info': 'CustomerInfo',
      'account_info': 'AccountInfo', 
      'collateral': 'Collateral',
      'credit_limit': 'CreditLimit',
      'outstanding_balance': 'OutstandingBalance',
      'overdue_bad_debt': 'OverdueBadDebt',
      'write_off_debt': 'WriteOffDebt',
      'exemption_debt': 'ExemptionDebt',
      'revenue': 'Revenue',
      'operational_targets': 'OperationalTargets'
    };

    const tableName = tableMap[type as string];
    
    if (!tableName) {
      return res.status(400).json({ error: 'Invalid import type' });
    }

    // Thêm check xem bảng có tồn tại không
    const tableExistsQuery = `
      SELECT COUNT(*) as count 
      FROM INFORMATION_SCHEMA.TABLES 
      WHERE TABLE_NAME = ?
    `;
    
    const tableExists = await query(tableExistsQuery, [tableName]);
    
    if (!tableExists[0]?.count) {
      console.error(`Table ${tableName} does not exist`);
      return res.status(404).json({ error: `Table ${tableName} not found` });
    }

    // Lấy dữ liệu với LIMIT để tránh overload
    const rawDataQuery = `
      SELECT TOP 1000 * 
      FROM ${tableName}
      ORDER BY 
        CASE 
          WHEN COLUMN_NAME = 'CreatedDate' THEN CreatedDate
          WHEN COLUMN_NAME = 'ImportDate' THEN ImportDate
          ELSE GETDATE()
        END DESC
    `;

    const result = await query(rawDataQuery);
    
    console.log(`Fetched ${result.length} records from ${tableName}`);
    
    res.status(200).json(result);
  } catch (error) {
    console.error('Raw data API error:', error);
    res.status(500).json({ 
      error: 'Internal server error',
      details: error instanceof Error ? error.message : 'Unknown error'
    });
  }
}
3. Sửa lại phần hiển thị nút Xem

Apply
// src/components/import/ImportPage.tsx
// Trong phần render của component

{importTypes.map((type) => (
  <motion.div key={type.id} className="bg-white rounded-lg shadow-sm p-6">
    {/* ... existing code ... */}
    
    <div className="mt-6 flex items-center justify-between">
      <div className="text-sm text-gray-500">
        {importHistory[type.id] ? (
          <span>
            Import lần cuối: {new Date(importHistory[type.id].importDate).toLocaleDateString('vi-VN')}
          </span>
        ) : (
          <span>Chưa có dữ liệu</span>
        )}
      </div>
      
      {/* Luôn hiển thị nút Xem nếu có lịch sử import */}
      {importHistory[type.id] && (
        <button
          onClick={() => {
            setSelectedType(type);
            setShowDataModal(true);
            // Load lại data khi mở modal
            if (!rawData[type.id]) {
              loadRawData(type.id);
            }
          }}
          className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors flex items-center space-x-2"
        >
          <Eye className="w-4 h-4" />
          <span>Xem dữ liệu</span>
        </button>
      )}
    </div>
  </motion.div>
))}
4. Cập nhật RawDataModal để handle loading tốt hơn

Apply
// src/components/import/RawDataModal.tsx
import React, { useEffect, useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { X, Download, Search } from 'lucide-react';
import { ImportType } from './types';

interface RawDataModalProps {
  isOpen: boolean;
  onClose: () => void;
  type: ImportType | null;
  data: any[];
  onLoadData?: (typeId: string) => void;
}

export default function RawDataModal({ 
  isOpen, 
  onClose, 
  type, 
  data,
  onLoadData 
}: RawDataModalProps) {
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (isOpen && type && (!data || data.length === 0) && onLoadData) {
      setLoading(true);
      setError(null);
      
      // Load data với timeout
      const loadDataWithTimeout = async () => {
        try {
          await onLoadData(type.id);
          setLoading(false);
        } catch (err) {
          setError('Không thể tải dữ liệu. Vui lòng thử lại.');
          setLoading(false);
        }
      };
      
      loadDataWithTimeout();
    }
  }, [isOpen, type, data, onLoadData]);

  const filteredData = data?.filter(row => 
    Object.values(row).some(value => 
      String(value).toLowerCase().includes(searchTerm.toLowerCase())
    )
  ) || [];

  const columns = data && data.length > 0 ? Object.keys(data[0]) : [];

  const exportToCSV = () => {
    if (!data || data.length === 0) return;
    
    const csvContent = [
      columns.join(','),
      ...data.map(row => 
        columns.map(col => `"${row[col] || ''}"`).join(',')
      )
    ].join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `${type?.name}_${new Date().toISOString().split('T')[0]}.csv`;
    link.click();
  };

  return (
    <AnimatePresence>
      {isOpen && (
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          className="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center p-4"
          onClick={onClose}
        >
          <motion.div
            initial={{ scale: 0.9, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            exit={{ scale: 0.9, opacity: 0 }}
            className="bg-white rounded-lg shadow-xl max-w-6xl w-full max-h-[90vh] overflow-hidden"
            onClick={(e) => e.stopPropagation()}
          >
            {/* Header */}
            <div className="bg-gradient-to-r from-blue-600 to-blue-700 text-white p-6">
              <div className="flex items-center justify-between">
                <h2 className="text-2xl font-bold">
                  {type?.name} - Dữ liệu chi tiết
                </h2>
                <button
                  onClick={onClose}
                  className="p-2 hover:bg-white/20 rounded-full transition-colors"
                >
                  <X className="w-6 h-6" />
                </button>
              </div>
            </div>

            {/* Toolbar */}
            <div className="p-4 border-b bg-gray-50 flex items-center justify-between">
              <div className="flex items-center space-x-4">
                <div className="relative">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
                  <input
                    type="text"
                    placeholder="Tìm kiếm..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="pl-10 pr-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500"
                  />
                </div>
                <span className="text-sm text-gray-600">
                  {loading ? 'Đang tải...' : `${filteredData.length} bản ghi`}
                </span>
              </div>
              
              <button
                onClick={exportToCSV}
                disabled={loading || !data || data.length === 0}
                className="flex items-center space-x-2 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              >
                <Download className="w-4 h-4" />
                <span>Xuất CSV</span>
              </button>
            </div>

            {/* Content */}
            <div className="p-4 overflow-auto max-h-[60vh]">
              {loading ? (
                <div className="flex justify-center items-center h-64">
                  <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
                </div>
              ) : error ? (
                <div className="text-center py-12">
                  <p className="text-red-600 mb-4">{error}</p>
                  <button
                    onClick={() => onLoadData && onLoadData(type!.id)}
                    className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
                  >
                    Thử lại
                  </button>
                </div>
              ) : filteredData.length === 0 ? (
                <div className="text-center py-12 text-gray-500">
                  Không có dữ liệu để hiển thị
                </div>
              ) : (
                <div className="overflow-x-auto">
                  <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-gray-50">
                      <tr>
                        {columns.map((column) => (
                          <th
                            key={column}
                            className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                          >
                            {column}
                          </th>
                        ))}
                      </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                      {filteredData.slice(0, 100).map((row, index) => (
                        <tr key={index} className="hover:bg-gray-50">
                          {columns.map((column) => (
                            <td
                              key={column}
                              className="px-6 py-4 whitespace-nowrap text-sm text-gray-900"
                            >
                              {row[column] || '-'}
                            </td>
                          ))}
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>
  );
}
5. Cập nhật ImportPage với error handling tốt hơn

Apply
// src/components/import/ImportPage.tsx
// Thêm vào component

const [dataLoadError, setDataLoadError] = useState<Record<string, boolean>>({});

const loadRawData = async (typeId: string) => {
  try {
    setDataLoadError(prev => ({ ...prev, [typeId]: false }));
    
    const response = await fetch(`/api/import/raw-data?type=${typeId}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    });

    if (!response.ok) {
      const errorData = await response.json();
      throw new Error(errorData.error || `HTTP error! status: ${response.status}`);
    }

    const data = await response.json();
    
    if (!Array.isArray(data)) {
      throw new Error('Invalid data format');
    }
    
    setRawData(prevState => ({
      ...prevState,
      [typeId]: data
    }));
    
    return data;
  } catch (error) {
    console.error('Error loading raw data:', error);
    setDataLoadError(prev => ({ ...prev, [typeId]: true }));
    toast.error(`Lỗi khi lấy dữ liệu: ${error instanceof Error ? error.message : 'Unknown error'}`);
    throw error;
  }
};

// Update modal render
{showDataModal && selectedType && (
  <RawDataModal
    isOpen={showDataModal}
    onClose={() => setShowDataModal(false)}
    type={selectedType}
    data={rawData[selectedType.id] || []}
    onLoadData={loadRawData}
  />
)}
6. Debug query SQL cho raw data

Apply
// src/pages/api/import/raw-data.ts
// Sửa lại query để đảm bảo hoạt động với SQL Server

const rawDataQuery = `
  SELECT TOP 1000 * 
  FROM [dbo].[${tableName}]
  ORDER BY 
    CASE 
      WHEN EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[${tableName}]') AND name = 'CreatedDate') 
        THEN CreatedDate
      WHEN EXISTS(SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[dbo].[${tableName}]') AND name = 'ImportDate') 
        THEN ImportDate
      ELSE GETDATE()
    END DESC
`;

// Hoặc đơn giản hơn
const rawDataQuery = `SELECT TOP 1000 * FROM [dbo].[${tableName}]`;
Các nguyên nhân có thể gây lỗi:

Tên bảng không đúng: Mapping giữa type và table name không khớp
Lỗi SQL: Query không phù hợp với SQL Server
Timing issue: Dữ liệu chưa kịp lưu vào DB khi query
Permission: User không có quyền SELECT trên bảng
Giải pháp đã áp dụng:

Thêm error handling chi tiết
Luôn hiển thị nút Xem nếu có history
Retry mechanism khi load data
Better logging để debug
Simplified SQL query

## khi import báo thành công, nhưng báo lỗi tiếp không lấy được dữ liệu, không xem được dữ liệu thô, nhưng khi xóa thì báo xóa thành công 05 file. ? giải pháp?

Tôi thấy vấn đề: Import thành công (có 5 file trong history) nhưng không xem được dữ liệu thô. Điều này cho thấy dữ liệu được lưu vào bảng ImportHistory nhưng không được lưu vào các bảng dữ liệu tương ứng.

1. Kiểm tra lại API import để đảm bảo dữ liệu được lưu vào đúng bảng
Typescript

Apply
// src/pages/api/import/index.ts
import { NextApiRequest, NextApiResponse } from 'next';
import formidable from 'formidable';
import { parse } from 'csv-parse/sync';
import XLSX from 'xlsx';
import { query } from '@/lib/db';
import { verifyToken } from '@/lib/auth';
import fs from 'fs/promises';

export const config = {
  api: {
    bodyParser: false,
  },
};

export default async function handler(req: NextApiRequest, res: NextApiResponse) {
  if (req.method !== 'POST') {
    return res.status(405).json({ error: 'Method not allowed' });
  }

  try {
    const token = req.headers.authorization?.replace('Bearer ', '');
    const decoded = verifyToken(token);
    
    if (!decoded) {
      return res.status(401).json({ error: 'Unauthorized' });
    }

    const form = formidable();
    const [fields, files] = await form.parse(req);
    
    const file = Array.isArray(files.file) ? files.file[0] : files.file;
    const importType = Array.isArray(fields.type) ? fields.type[0] : fields.type;
    
    if (!file || !importType) {
      return res.status(400).json({ error: 'File and import type are required' });
    }

    // Đọc file
    const fileContent = await fs.readFile(file.filepath);
    let data: any[] = [];
    
    if (file.originalFilename?.endsWith('.csv')) {
      const csvContent = fileContent.toString('utf-8');
      data = parse(csvContent, {
        columns: true,
        skip_empty_lines: true,
        bom: true
      });
    } else if (file.originalFilename?.endsWith('.xlsx') || file.originalFilename?.endsWith('.xls')) {
      const workbook = XLSX.read(fileContent, { type: 'buffer' });
      const sheetName = workbook.SheetNames[0];
      data = XLSX.utils.sheet_to_json(workbook.Sheets[sheetName]);
    }

    console.log(`Parsed ${data.length} rows from file`);

    // Mapping import type to table name
    const tableMap: Record<string, string> = {
      'customer_info': 'CustomerInfo',
      'account_info': 'AccountInfo',
      'collateral': 'Collateral',
      'credit_limit': 'CreditLimit',
      'outstanding_balance': 'OutstandingBalance',
      'overdue_bad_debt': 'OverdueBadDebt',
      'write_off_debt': 'WriteOffDebt',
      'exemption_debt': 'ExemptionDebt',
      'revenue': 'Revenue',
      'operational_targets': 'OperationalTargets'
    };

    const tableName = tableMap[importType];
    
    if (!tableName) {
      return res.status(400).json({ error: 'Invalid import type' });
    }

    // Bắt đầu transaction
    await query('BEGIN TRANSACTION');

    try {
      // 1. Lưu vào ImportHistory
      const importResult = await query(`
        INSERT INTO ImportHistory (
          ImportType, 
          FileName, 
          RecordCount, 
          Status, 
          ImportDate, 
          ImportedBy
        ) 
        OUTPUT INSERTED.ImportID
        VALUES (?, ?, ?, ?, GETDATE(), ?)
      `, [importType, file.originalFilename, data.length, 'Success', decoded.userId]);

      const importId = importResult[0].ImportID;
      console.log('ImportHistory saved with ID:', importId);

      // 2. Xóa dữ liệu cũ (optional - có thể comment nếu muốn giữ lại)
      // await query(`DELETE FROM ${tableName}`);

      // 3. Insert dữ liệu mới vào bảng tương ứng
      if (data.length > 0) {
        const columns = Object.keys(data[0]);
        const validColumns = await getValidColumns(tableName, columns);
        
        if (validColumns.length === 0) {
          throw new Error('No matching columns found');
        }

        // Thêm ImportID vào data nếu bảng có cột này
        const hasImportIdColumn = await checkColumnExists(tableName, 'ImportID');
        
        // Insert từng batch 100 records
        const batchSize = 100;
        for (let i = 0; i < data.length; i += batchSize) {
          const batch = data.slice(i, i + batchSize);
          
          for (const row of batch) {
            const values = validColumns.map(col => row[col] || null);
            
            if (hasImportIdColumn) {
              validColumns.push('ImportID');
              values.push(importId);
            }
            
            const placeholders = validColumns.map(() => '?').join(', ');
            const insertQuery = `
              INSERT INTO ${tableName} (${validColumns.join(', ')})
              VALUES (${placeholders})
            `;
            
            await query(insertQuery, values);
          }
        }
        
        console.log(`Inserted ${data.length} records into ${tableName}`);
      }

      // Commit transaction
      await query('COMMIT');
      
      // Cleanup
      await fs.unlink(file.filepath);

      res.status(200).json({ 
        success: true, 
        message: 'Import successful',
        recordCount: data.length,
        importId: importId
      });

    } catch (error) {
      // Rollback nếu có lỗi
      await query('ROLLBACK');
      throw error;
    }

  } catch (error) {
    console.error('Import error:', error);
    res.status(500).json({ 
      error: 'Import failed', 
      details: error instanceof Error ? error.message : 'Unknown error' 
    });
  }
}

// Helper function để lấy các cột hợp lệ
async function getValidColumns(tableName: string, fileColumns: string[]): Promise<string[]> {
  const result = await query(`
    SELECT COLUMN_NAME
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = ?
    AND COLUMN_NAME IN (${fileColumns.map(() => '?').join(',')})
  `, [tableName, ...fileColumns]);
  
  return result.map((row: any) => row.COLUMN_NAME);
}

// Helper function kiểm tra cột tồn tại
async function checkColumnExists(tableName: string, columnName: string): Promise<boolean> {
  const result = await query(`
    SELECT COUNT(*) as count
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = ? AND COLUMN_NAME = ?
  `, [tableName, columnName]);
  
  return result[0].count > 0;
}
2. Tạo script kiểm tra và sửa cấu trúc bảng
Sql

Apply
-- src/scripts/check-tables.sql

-- Kiểm tra các bảng dữ liệu
SELECT 
    t.TABLE_NAME,
    COUNT(c.COLUMN_NAME) as ColumnCount
FROM INFORMATION_SCHEMA.TABLES t
LEFT JOIN INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME
WHERE t.TABLE_TYPE = 'BASE TABLE'
AND t.TABLE_NAME IN (
    'CustomerInfo', 'AccountInfo', 'Collateral', 'CreditLimit',
    'OutstandingBalance', 'OverdueBadDebt', 'WriteOffDebt',
    'ExemptionDebt', 'Revenue', 'OperationalTargets'
)
GROUP BY t.TABLE_NAME;

-- Kiểm tra dữ liệu trong các bảng
SELECT 'CustomerInfo' as TableName, COUNT(*) as RecordCount FROM CustomerInfo
UNION ALL
SELECT 'AccountInfo', COUNT(*) FROM AccountInfo
UNION ALL
SELECT 'Collateral', COUNT(*) FROM Collateral
UNION ALL
SELECT 'CreditLimit', COUNT(*) FROM CreditLimit
UNION ALL
SELECT 'OutstandingBalance', COUNT(*) FROM OutstandingBalance
UNION ALL
SELECT 'OverdueBadDebt', COUNT(*) FROM OverdueBadDebt
UNION ALL
SELECT 'WriteOffDebt', COUNT(*) FROM WriteOffDebt
UNION ALL
SELECT 'ExemptionDebt', COUNT(*) FROM ExemptionDebt
UNION ALL
SELECT 'Revenue', COUNT(*) FROM Revenue
UNION ALL
SELECT 'OperationalTargets', COUNT(*) FROM OperationalTargets;

-- Kiểm tra ImportHistory
SELECT TOP 10 * FROM ImportHistory ORDER BY ImportDate DESC;

-- Thêm cột ImportID vào các bảng nếu chưa có
DECLARE @TableName NVARCHAR(100)
DECLARE @SQL NVARCHAR(MAX)

DECLARE table_cursor CURSOR FOR
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
AND TABLE_NAME IN (
    'CustomerInfo', 'AccountInfo', 'Collateral', 'CreditLimit',
    'OutstandingBalance', 'OverdueBadDebt', 'WriteOffDebt',
    'ExemptionDebt', 'Revenue', 'OperationalTargets'
)

OPEN table_cursor
FETCH NEXT FROM table_cursor INTO @TableName

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Check if ImportID column exists
    IF NOT EXISTS (
        SELECT 1 
        FROM INFORMATION_SCHEMA.COLUMNS 
        WHERE TABLE_NAME = @TableName AND COLUMN_NAME = 'ImportID'
    )
    BEGIN
        SET @SQL = 'ALTER TABLE ' + @TableName + ' ADD ImportID INT NULL'
        EXEC sp_executesql @SQL
        PRINT 'Added ImportID to ' + @TableName
    END
    
    FETCH NEXT FROM table_cursor INTO @TableName
END

CLOSE table_cursor
DEALLOCATE table_cursor
3. Tạo API endpoint để debug
Typescript

Apply
// src/pages/api/import/debug.ts
import { NextApiRequest, NextApiResponse } from 'next';
import { verifyToken } from '@/lib/auth';
import { query } from '@/lib/db';

export default async function handler(req: NextApiRequest, res: NextApiResponse) {
  if (req.method !== 'GET') {
    return res.status(405).json({ error: 'Method not allowed' });
  }

  try {
    const token = req.headers.authorization?.replace('Bearer ', '');
    const decoded = verifyToken(token);
    
    if (!decoded) {
      return res.status(401).json({ error: 'Unauthorized' });
    }

    const { importId } = req.query;

    // 1. Lấy thông tin import
    const importInfo = await query(`
      SELECT * FROM ImportHistory 
      WHERE ImportID = ?
    `, [importId]);

    // 2. Kiểm tra dữ liệu trong các bảng
    const tableStats = await query(`
      SELECT 'CustomerInfo' as TableName, COUNT(*) as Count FROM CustomerInfo WHERE ImportID = ?
      UNION ALL
      SELECT 'AccountInfo', COUNT(*) FROM AccountInfo WHERE ImportID = ?
      UNION ALL
      SELECT 'Collateral', COUNT(*) FROM Collateral WHERE ImportID = ?
      UNION ALL
      SELECT 'CreditLimit', COUNT(*) FROM CreditLimit WHERE ImportID = ?
      UNION ALL
      SELECT 'OutstandingBalance', COUNT(*) FROM OutstandingBalance WHERE ImportID = ?
      UNION ALL
      SELECT 'OverdueBadDebt', COUNT(*) FROM OverdueBadDebt WHERE ImportID = ?
      UNION ALL
      SELECT 'WriteOffDebt', COUNT(*) FROM WriteOffDebt WHERE ImportID = ?
      UNION ALL
      SELECT 'ExemptionDebt', COUNT(*) FROM ExemptionDebt WHERE ImportID = ?
      UNION ALL
      SELECT 'Revenue', COUNT(*) FROM Revenue WHERE ImportID = ?
      UNION ALL
      SELECT 'OperationalTargets', COUNT(*) FROM OperationalTargets WHERE ImportID = ?
    `, Array(10).fill(importId));

    res.status(200).json({
      importInfo: importInfo[0],
      tableStats
    });
  } catch (error) {
    console.error('Debug API error:', error);
    res.status(500).json({ 
      error: 'Internal server error',
      details: error instanceof Error ? error.message : 'Unknown error'
    });
  }
}
4. Sửa lại raw-data API để handle trường hợp không có ImportID
Typescript

Apply
// src/pages/api/import/raw-data.ts
export default async function handler(req: NextApiRequest, res: NextApiResponse) {
  if (req.method !== 'GET') {
    return res.status(405).json({ error: 'Method not allowed' });
  }

  try {
    const token = req.headers.authorization?.replace('Bearer ', '');
    const decoded = verifyToken(token);
    
    if (!decoded) {
      return res.status(401).json({ error: 'Unauthorized' });
    }

    const { type } = req.query;
    
    if (!type) {
      return res.status(400).json({ error: 'Import type is required' });
    }

    const tableMap: Record<string, string> = {
      'customer_info': 'CustomerInfo',
      'account_info': 'AccountInfo',
      'collateral': 'Collateral', 
      'credit_limit': 'CreditLimit',
      'outstanding_balance': 'OutstandingBalance',
      'overdue_bad_debt': 'OverdueBadDebt',
      'write_off_debt': 'WriteOffDebt',
      'exemption_debt': 'ExemptionDebt',
      'revenue': 'Revenue',
      'operational_targets': 'OperationalTargets'
    };

    const tableName = tableMap[type as string];
    
    if (!tableName) {
      return res.status(400).json({ error: 'Invalid import type' });
    }

    // Kiểm tra bảng tồn tại
    const checkTableQuery = `
      SELECT COUNT(*) as count 
      FROM INFORMATION_SCHEMA.TABLES 
      WHERE TABLE_NAME = ? AND TABLE_TYPE = 'BASE TABLE'
    `;
    
    const tableCheck = await query(checkTableQuery, [tableName]);
    
    if (!tableCheck[0]?.count) {
      console.error(`Table ${tableName} does not exist`);
      
      // Tạo bảng nếu chưa tồn tại
      await createTableIfNotExists(tableName, type as string);
    }

    // Đếm tổng số records
    const countQuery = `SELECT COUNT(*) as total FROM ${tableName}`;
    const countResult = await query(countQuery);
    const totalRecords = countResult[0]?.total || 0;

    console.log(`Table ${tableName} has ${totalRecords} records`);

    if (totalRecords === 0) {
      return res.status(200).json([]);
    }

    // Lấy dữ liệu
    const rawDataQuery = `
      SELECT TOP 1000 * 
      FROM ${tableName}
      ORDER BY 1 DESC
    `;

    const result = await query(rawDataQuery);
    
    console.log(`Fetched ${result.length} records from ${tableName}`);
    
    res.status(200).json(result);
  } catch (error) {
    console.error('Raw data API error:', error);
    res.status(500).json({ 
      error: 'Internal server error',
      details: error instanceof Error ? error.message : 'Unknown error'
    });
  }
}

// Helper function tạo bảng
async function createTableIfNotExists(tableName: string, type: string) {
  const tableSchemas: Record<string, string> = {
    'CustomerInfo': `
      CREATE TABLE CustomerInfo (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        CustomerCode NVARCHAR(50),
        CustomerName NVARCHAR(255),
        Address NVARCHAR(500),
        Phone NVARCHAR(50),
        Email NVARCHAR(100),
        ImportID INT,
        CreatedDate DATETIME DEFAULT GETDATE()
      )
    `,
    'AccountInfo': `
      CREATE TABLE AccountInfo (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        AccountNumber NVARCHAR(50),
        CustomerCode NVARCHAR(50),
        AccountType NVARCHAR(100),
        Status NVARCHAR(50),
        OpenDate DATE,
        ImportID INT,
        CreatedDate DATETIME DEFAULT GETDATE()
      )
    `,
    // Thêm các bảng khác...
  };

  if (tableSchemas[tableName]) {
    await query(tableSchemas[tableName]);
    console.log(`Created table ${tableName}`);
  }
}
5. Thêm logging chi tiết hơn trong ImportPage
Typescript

Apply
// src/components/import/ImportPage.tsx
const handleFileUpload = async (file: File, importType: ImportType) => {
  // ... existing code ...

  try {
    const response = await fetch('/api/import', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      },
      body: formData
    });

    const result = await response.json();
    console.log('Import result:', result);

    if (response.ok) {
      toast.success(`Import ${importType.name} thành công! (${result.recordCount} records)`);
      
      // Debug: Check import details
      if (result.importId) {
        const debugResponse = await fetch(`/api/import/debug?importId=${result.importId}`, {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        });
        
        if (debugResponse.ok) {
          const debugData = await debugResponse.json();
          console.log('Import debug info:', debugData);
        }
      }
      
      // Reload data
      setTimeout(() => {
        loadImportHistory();
        loadRawData(importType.id);
      }, 2000); // Tăng delay
    } else {
      toast.error(result.error || 'Lỗi khi import file');
    }
  } catch (error) {
    console.error('Upload error:', error);
    toast.error('Lỗi khi upload file');
  }
};
Tóm tắt vấn đề và giải pháp:
Vấn đề chính: Dữ liệu được lưu vào ImportHistory nhưng không được lưu vào các bảng dữ liệu tương ứng.

Nguyên nhân có thể:

Tên bảng không đúng hoặc bảng không tồn tại
Cột trong file không khớp với cột trong bảng
Lỗi trong quá trình insert dữ liệu
Transaction bị rollback
Giải pháp:

Kiểm tra và tạo bảng nếu chưa tồn tại
Log chi tiết quá trình import
Thêm debug endpoint để kiểm tra
Handle mapping cột linh hoạt hơn
Thêm ImportID vào các bảng để tracking
Hãy chạy script SQL ở bước 2 để kiểm tra cấu trúc bảng và dữ liệu, sau đó xem log trong console khi import để biết chính xác lỗi ở đâu.
# D. khi ấn vào nút xem thô không xảy ra sự kiện gì; khi ấn nút xem import, xuất hiện kết quả lọc ở bảng kết quả, ấn tiếp nút xem thì hiện ra popup "Xem trước: "Tên file" với nội dung "Không có dữ liệu để hiện thị"Nguyên nhân và cách khắc phục để có thể xem được file dữ liệu theo các ngày import vào!


