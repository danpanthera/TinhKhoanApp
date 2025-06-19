import React, { useState, useEffect } from 'react';
import {
  Card,
  Row,
  Col,
  Statistic,
  Progress,
  Timeline,
  Alert,
  Spin,
  Typography,
  Tag,
  Button,
  Space,
  Tooltip
} from 'antd';
import {
  DatabaseOutlined,
  CheckCircleOutlined,
  ClockCircleOutlined,
  ExclamationCircleOutlined,
  ReloadOutlined,
  BarChartOutlined
} from '@ant-design/icons';
import { getRawDataStatistics, getRecentImports } from '@/services/rawDataService';
import moment from 'moment';

const { Title, Text } = Typography;

interface ImportStatistics {
  tableName: string;
  totalImports: number;
  successfulImports: number;
  failedImports: number;
  processingImports: number;
  lastImportDate?: string;
  totalRecordsProcessed: number;
  totalNewRecords: number;
  totalUpdatedRecords: number;
  totalDeletedRecords: number;
  avgDurationSeconds: number;
  successRate: number;
}

interface ImportLog {
  logID: number;
  batchId: string;
  tableName: string;
  importDate: string;
  status: string;
  totalRecords: number;
  processedRecords: number;
  newRecords: number;
  updatedRecords: number;
  deletedRecords: number;
  duration?: number;
  errorMessage?: string;
  createdBy: string;
}

export const RawDataDashboard: React.FC = () => {
  const [overallStats, setOverallStats] = useState<{
    totalTables: number;
    todayImports: number;
    processingImports: number;
    successRate: number;
  }>({
    totalTables: 0,
    todayImports: 0,
    processingImports: 0,
    successRate: 0
  });

  const [tableStats, setTableStats] = useState<ImportStatistics[]>([]);
  const [recentImports, setRecentImports] = useState<ImportLog[]>([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  useEffect(() => {
    loadDashboardData();
    
    // Auto refresh every 30 seconds
    const interval = setInterval(() => {
      loadDashboardData(true);
    }, 30000);

    return () => clearInterval(interval);
  }, []);

  const loadDashboardData = async (silent = false) => {
    if (!silent) setLoading(true);
    setRefreshing(true);

    try {
      const [ln01Stats, gl01Stats, dp01Stats, imports] = await Promise.all([
        getRawDataStatistics('LN01'),
        getRawDataStatistics('GL01'),
        getRawDataStatistics('DP01'),
        getRecentImports(15)
      ]);

      const allStats = [ln01Stats, gl01Stats, dp01Stats].filter(s => s);
      setTableStats(allStats);

      // Calculate overall statistics
      const today = moment().startOf('day');
      const todayImports = imports.filter(i => 
        moment(i.importDate).isAfter(today)
      ).length;

      const processingImports = imports.filter(i => 
        i.status === 'PROCESSING'
      ).length;

      const totalSuccessful = allStats.reduce((sum, s) => sum + s.successfulImports, 0);
      const totalImports = allStats.reduce((sum, s) => sum + s.totalImports, 0);
      const overallSuccessRate = totalImports > 0 ? (totalSuccessful / totalImports) * 100 : 0;

      setOverallStats({
        totalTables: allStats.length,
        todayImports,
        processingImports,
        successRate: overallSuccessRate
      });

      setRecentImports(imports);
    } catch (error) {
      console.error('Error loading dashboard data:', error);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  const handleRefresh = () => {
    loadDashboardData();
  };

  const getStatusColor = (status: string) => {
    switch (status.toLowerCase()) {
      case 'success': return 'green';
      case 'failed': return 'red';
      case 'processing': return 'blue';
      default: return 'default';
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status.toLowerCase()) {
      case 'success': return <CheckCircleOutlined />;
      case 'failed': return <ExclamationCircleOutlined />;
      case 'processing': return <ClockCircleOutlined />;
      default: return <ClockCircleOutlined />;
    }
  };

  const formatDuration = (seconds?: number) => {
    if (!seconds) return 'N/A';
    if (seconds < 60) return `${seconds}s`;
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}m ${remainingSeconds}s`;
  };

  if (loading) {
    return (
      <div style={{ textAlign: 'center', padding: '50px' }}>
        <Spin size="large" />
        <div style={{ marginTop: 16 }}>Đang tải dữ liệu...</div>
      </div>
    );
  }

  return (
    <div style={{ padding: '24px' }}>
      <Row justify="space-between" align="middle" style={{ marginBottom: 24 }}>
        <Col>
          <Title level={2}>
            <DatabaseOutlined /> Dashboard Quản lý Dữ liệu Thô
          </Title>
        </Col>
        <Col>
          <Space>
            <Tooltip title="Refresh dữ liệu">
              <Button 
                icon={<ReloadOutlined />} 
                onClick={handleRefresh}
                loading={refreshing}
              >
                Refresh
              </Button>
            </Tooltip>
          </Space>
        </Col>
      </Row>

      {/* Overview Statistics */}
      <Row gutter={16} style={{ marginBottom: 24 }}>
        <Col xs={24} sm={12} md={6}>
          <Card>
            <Statistic
              title="Tổng số bảng"
              value={overallStats.totalTables}
              prefix={<DatabaseOutlined />}
              valueStyle={{ color: '#1890ff' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} md={6}>
          <Card>
            <Statistic
              title="Import hôm nay"
              value={overallStats.todayImports}
              prefix={<CheckCircleOutlined />}
              valueStyle={{ color: '#52c41a' }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} md={6}>
          <Card>
            <Statistic
              title="Đang xử lý"
              value={overallStats.processingImports}
              prefix={<ClockCircleOutlined />}
              valueStyle={{ 
                color: overallStats.processingImports > 0 ? '#faad14' : '#52c41a' 
              }}
            />
          </Card>
        </Col>
        <Col xs={24} sm={12} md={6}>
          <Card>
            <div style={{ textAlign: 'center' }}>
              <Progress
                type="circle"
                percent={Math.round(overallStats.successRate)}
                format={percent => `${percent}%`}
                strokeColor={{
                  '0%': '#ff4d4f',
                  '50%': '#faad14',
                  '100%': '#52c41a',
                }}
              />
              <div style={{ marginTop: 8, fontWeight: 'bold' }}>
                Tỷ lệ thành công
              </div>
            </div>
          </Card>
        </Col>
      </Row>

      {/* Table Statistics */}
      <Row gutter={16} style={{ marginBottom: 24 }}>
        {tableStats.map((stats) => (
          <Col xs={24} md={8} key={stats.tableName}>
            <Card 
              title={
                <Space>
                  <BarChartOutlined />
                  <span>{stats.tableName}</span>
                </Space>
              }
              extra={
                <Tag color={stats.successRate >= 95 ? 'green' : stats.successRate >= 80 ? 'orange' : 'red'}>
                  {stats.successRate.toFixed(1)}% thành công
                </Tag>
              }
            >
              <Row gutter={16}>
                <Col span={12}>
                  <Statistic
                    title="Tổng import"
                    value={stats.totalImports}
                    valueStyle={{ fontSize: 16 }}
                  />
                </Col>
                <Col span={12}>
                  <Statistic
                    title="Bản ghi xử lý"
                    value={stats.totalRecordsProcessed}
                    valueStyle={{ fontSize: 16 }}
                  />
                </Col>
              </Row>
              
              <Row gutter={16} style={{ marginTop: 16 }}>
                <Col span={8}>
                  <Statistic
                    title="Mới"
                    value={stats.totalNewRecords}
                    valueStyle={{ fontSize: 14, color: '#52c41a' }}
                  />
                </Col>
                <Col span={8}>
                  <Statistic
                    title="Cập nhật"
                    value={stats.totalUpdatedRecords}
                    valueStyle={{ fontSize: 14, color: '#1890ff' }}
                  />
                </Col>
                <Col span={8}>
                  <Statistic
                    title="Xóa"
                    value={stats.totalDeletedRecords}
                    valueStyle={{ fontSize: 14, color: '#ff4d4f' }}
                  />
                </Col>
              </Row>

              <div style={{ marginTop: 16 }}>
                <Text type="secondary">
                  Thời gian TB: {formatDuration(stats.avgDurationSeconds)}
                </Text>
                {stats.lastImportDate && (
                  <div>
                    <Text type="secondary">
                      Import cuối: {moment(stats.lastImportDate).format('DD/MM/YYYY HH:mm')}
                    </Text>
                  </div>
                )}
              </div>
            </Card>
          </Col>
        ))}
      </Row>

      {/* Recent Imports Timeline */}
      <Card 
        title="Import gần đây" 
        style={{ marginBottom: 24 }}
      >
        <Timeline>
          {recentImports.map((item) => (
            <Timeline.Item
              key={item.logID}
              color={getStatusColor(item.status)}
              dot={getStatusIcon(item.status)}
            >
              <div>
                <Space>
                  <Text strong>{item.tableName}</Text>
                  <Tag color={getStatusColor(item.status)}>
                    {item.status}
                  </Tag>
                  <Text type="secondary">
                    {moment(item.importDate).format('DD/MM/YYYY HH:mm:ss')}
                  </Text>
                </Space>
              </div>
              
              <div style={{ marginTop: 4 }}>
                <Text>
                  {item.totalRecords} bản ghi • 
                  Thời gian: {formatDuration(item.duration)} • 
                  Bởi: {item.createdBy}
                </Text>
              </div>

              {(item.newRecords > 0 || item.updatedRecords > 0 || item.deletedRecords > 0) && (
                <div style={{ marginTop: 4 }}>
                  <Space>
                    {item.newRecords > 0 && (
                      <Tag color="green">{item.newRecords} mới</Tag>
                    )}
                    {item.updatedRecords > 0 && (
                      <Tag color="blue">{item.updatedRecords} cập nhật</Tag>
                    )}
                    {item.deletedRecords > 0 && (
                      <Tag color="red">{item.deletedRecords} xóa</Tag>
                    )}
                  </Space>
                </div>
              )}

              {item.errorMessage && (
                <Alert
                  message="Lỗi import"
                  description={item.errorMessage}
                  type="error"
                  showIcon
                  style={{ marginTop: 8, fontSize: 12 }}
                />
              )}
            </Timeline.Item>
          ))}
        </Timeline>
      </Card>
    </div>
  );
};
