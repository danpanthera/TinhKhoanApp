import React, { useState, useEffect, useCallback, useMemo, useRef } from 'react';
import {
  Table,
  Card,
  Input,
  Select,
  Button,
  Space,
  Tag,
  Tooltip,
  message,
  Spin,
  Alert,
  Row,
  Col,
  Statistic,
  Badge,
  Typography,
  Drawer
} from 'antd';
import {
  SearchOutlined,
  ReloadOutlined,
  FilterOutlined,
  DownloadOutlined,
  EyeOutlined,
  BarChartOutlined,
  ThunderboltOutlined
} from '@ant-design/icons';
import { FixedSizeList as List } from 'react-window';
import rawDataService from '../../services/rawDataService';
import moment from 'moment';

const { Search } = Input;
const { Option } = Select;
const { Text, Title } = Typography;

interface OptimizedDataTableProps {
  dataType?: string;
  height?: number;
  showFilters?: boolean;
  showStats?: boolean;
  enableVirtualScroll?: boolean;
}

interface TableData {
  id: string;
  [key: string]: any;
}

interface TableStats {
  totalRecords: number;
  loadedRecords: number;
  avgResponseTime: number;
  cacheHitRate: number;
}

const OptimizedDataTable: React.FC<OptimizedDataTableProps> = ({
  dataType = 'all',
  height = 600,
  showFilters = true,
  showStats = true,
  enableVirtualScroll = true
}) => {
  // State management
  const [data, setData] = useState<TableData[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [sortBy, setSortBy] = useState('ImportDate');
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('desc');
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(50);
  const [total, setTotal] = useState(0);
  const [stats, setStats] = useState<TableStats>({
    totalRecords: 0,
    loadedRecords: 0,
    avgResponseTime: 0,
    cacheHitRate: 0
  });
  const [selectedDataTypes, setSelectedDataTypes] = useState<string[]>([]);
  const [dateRange, setDateRange] = useState<[string?, string?]>([]);
  const [performanceVisible, setPerformanceVisible] = useState(false);
  const [performanceData, setPerformanceData] = useState<any>(null);

  // Virtual scrolling state
  const [virtualData, setVirtualData] = useState<TableData[]>([]);
  const [hasNextPage, setHasNextPage] = useState(true);
  const [isLoadingMore, setIsLoadingMore] = useState(false);
  const listRef = useRef<any>(null);

  // Performance tracking
  const [responseTime, setResponseTime] = useState(0);
  const startTimeRef = useRef<number>(0);

  // Data type definitions from service
  const dataTypeDefinitions = rawDataService.getDataTypeDefinitions();

  // Debounced search
  const debouncedSearch = useCallback(
    debounce((term: string) => {
      setSearchTerm(term);
      setPage(1);
      if (enableVirtualScroll) {
        setVirtualData([]);
        loadVirtualData(0, pageSize, term);
      } else {
        loadData(1, pageSize, term);
      }
    }, 300),
    [pageSize, enableVirtualScroll]
  );

  // Load regular paginated data
  const loadData = async (pageNum: number = page, size: number = pageSize, search: string = searchTerm) => {
    setLoading(true);
    startTimeRef.current = performance.now();

    try {
      const result = await rawDataService.getOptimizedImports(pageNum, size, search, sortBy, sortOrder);
      
      if (result.success) {
        setData(result.data.items || []);
        setTotal(result.data.totalCount || 0);
        
        // Update stats
        const endTime = performance.now();
        const currentResponseTime = endTime - startTimeRef.current;
        setResponseTime(currentResponseTime);
        
        setStats(prev => ({
          totalRecords: result.data.totalCount || 0,
          loadedRecords: result.data.items?.length || 0,
          avgResponseTime: (prev.avgResponseTime + currentResponseTime) / 2,
          cacheHitRate: result.data.cacheHitRate || 0
        }));
      } else {
        message.error(result.error || 'Lỗi tải dữ liệu');
      }
    } catch (error) {
      console.error('Error loading data:', error);
      message.error('Lỗi tải dữ liệu');
    } finally {
      setLoading(false);
    }
  };

  // Load virtual scroll data
  const loadVirtualData = async (offset: number, limit: number, search: string = searchTerm) => {
    if (offset === 0) {
      setVirtualData([]);
      setIsLoadingMore(false);
    }
    
    if (isLoadingMore) return;
    setIsLoadingMore(true);
    startTimeRef.current = performance.now();

    try {
      const result = await rawDataService.getOptimizedRecords('all', offset, limit, search);
      
      if (result.success) {
        const newData = result.data.items || [];
        
        if (offset === 0) {
          setVirtualData(newData);
        } else {
          setVirtualData(prev => [...prev, ...newData]);
        }
        
        setHasNextPage(newData.length === limit);
        
        // Update stats
        const endTime = performance.now();
        const currentResponseTime = endTime - startTimeRef.current;
        setResponseTime(currentResponseTime);
        
        setStats(prev => ({
          totalRecords: result.data.totalCount || 0,
          loadedRecords: virtualData.length + newData.length,
          avgResponseTime: (prev.avgResponseTime + currentResponseTime) / 2,
          cacheHitRate: result.data.cacheHitRate || 0
        }));
      } else {
        message.error(result.error || 'Lỗi tải dữ liệu');
      }
    } catch (error) {
      console.error('Error loading virtual data:', error);
      message.error('Lỗi tải dữ liệu');
    } finally {
      setIsLoadingMore(false);
    }
  };

  // Load more for virtual scrolling
  const loadMore = useCallback(() => {
    if (hasNextPage && !isLoadingMore) {
      loadVirtualData(virtualData.length, pageSize);
    }
  }, [virtualData.length, pageSize, hasNextPage, isLoadingMore]);

  // Load performance data
  const loadPerformanceData = async () => {
    try {
      const result = await rawDataService.getPerformanceStats('24h');
      if (result.success) {
        setPerformanceData(result.data);
      }
    } catch (error) {
      console.error('Error loading performance data:', error);
    }
  };

  // Effect hooks
  useEffect(() => {
    if (enableVirtualScroll) {
      loadVirtualData(0, pageSize);
    } else {
      loadData();
    }
  }, [page, pageSize, sortBy, sortOrder, enableVirtualScroll]);

  useEffect(() => {
    if (showStats) {
      loadPerformanceData();
    }
  }, [showStats]);

  // Table columns configuration
  const columns = useMemo(() => [
    {
      title: 'ID',
      dataIndex: 'id',
      key: 'id',
      width: 80,
      fixed: 'left' as const,
    },
    {
      title: 'Loại dữ liệu',
      dataIndex: 'dataType',
      key: 'dataType',
      width: 120,
      render: (dataType: string) => {
        const definition = dataTypeDefinitions[dataType];
        return (
          <Tag color={rawDataService.getDataTypeColor(dataType)}>
            {definition?.icon} {dataType}
          </Tag>
        );
      },
    },
    {
      title: 'Tên file',
      dataIndex: 'fileName',
      key: 'fileName',
      width: 200,
      ellipsis: {
        showTitle: false,
      },
      render: (fileName: string) => (
        <Tooltip title={fileName}>
          <Text>{fileName}</Text>
        </Tooltip>
      ),
    },
    {
      title: 'Số bản ghi',
      dataIndex: 'recordsCount',
      key: 'recordsCount',
      width: 120,
      render: (count: number) => (
        <Badge count={rawDataService.formatRecordCount(count)} showZero />
      ),
    },
    {
      title: 'Ngày import',
      dataIndex: 'importDate',
      key: 'importDate',
      width: 150,
      render: (date: string) => moment(date).format('DD/MM/YYYY HH:mm'),
    },
    {
      title: 'Trạng thái',
      dataIndex: 'status',
      key: 'status',
      width: 100,
      render: (status: string) => {
        const color = status === 'Completed' ? 'success' : status === 'Failed' ? 'error' : 'processing';
        return <Tag color={color}>{status}</Tag>;
      },
    },
    {
      title: 'Thao tác',
      key: 'actions',
      width: 120,
      fixed: 'right' as const,
      render: (record: TableData) => (
        <Space>
          <Tooltip title="Xem chi tiết">
            <Button size="small" icon={<EyeOutlined />} />
          </Tooltip>
          <Tooltip title="Tải xuống">
            <Button size="small" icon={<DownloadOutlined />} />
          </Tooltip>
        </Space>
      ),
    },
  ], [dataTypeDefinitions]);

  // Virtual row renderer
  const VirtualRow = ({ index, style }) => {
    const record = virtualData[index];
    
    // Load more when near the end
    if (index === virtualData.length - 5) {
      loadMore();
    }

    if (!record) {
      return (
        <div style={style}>
          <Spin size="small" />
        </div>
      );
    }

    return (
      <div style={style} className="virtual-row">
        <Row gutter={16} align="middle">
          <Col span={2}>{record.id}</Col>
          <Col span={3}>
            <Tag color={rawDataService.getDataTypeColor(record.dataType)}>
              {record.dataType}
            </Tag>
          </Col>
          <Col span={6}>
            <Text ellipsis>{record.fileName}</Text>
          </Col>
          <Col span={3}>
            <Badge count={rawDataService.formatRecordCount(record.recordsCount)} showZero />
          </Col>
          <Col span={4}>
            {moment(record.importDate).format('DD/MM/YYYY HH:mm')}
          </Col>
          <Col span={3}>
            <Tag color={record.status === 'Completed' ? 'success' : 'error'}>
              {record.status}
            </Tag>
          </Col>
          <Col span={3}>
            <Space>
              <Button size="small" icon={<EyeOutlined />} />
              <Button size="small" icon={<DownloadOutlined />} />
            </Space>
          </Col>
        </Row>
      </div>
    );
  };

  return (
    <div className="optimized-data-table">
      {showStats && (
        <Row gutter={16} style={{ marginBottom: 16 }}>
          <Col span={6}>
            <Card size="small">
              <Statistic
                title="Tổng số bản ghi"
                value={stats.totalRecords}
                formatter={(value) => rawDataService.formatRecordCount(Number(value))}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card size="small">
              <Statistic
                title="Đã tải"
                value={stats.loadedRecords}
                formatter={(value) => rawDataService.formatRecordCount(Number(value))}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card size="small">
              <Statistic
                title="Thời gian phản hồi"
                value={responseTime}
                formatter={(value) => rawDataService.formatPerformanceMetric(Number(value))}
                valueStyle={{ color: rawDataService.getPerformanceColor(responseTime) }}
                prefix={<ThunderboltOutlined />}
              />
            </Card>
          </Col>
          <Col span={6}>
            <Card size="small">
              <Statistic
                title="Cache Hit Rate"
                value={stats.cacheHitRate}
                formatter={(value) => `${value}%`}
                valueStyle={{ color: stats.cacheHitRate > 80 ? '#52c41a' : '#faad14' }}
              />
            </Card>
          </Col>
        </Row>
      )}

      {showFilters && (
        <Card size="small" style={{ marginBottom: 16 }}>
          <Row gutter={16} align="middle">
            <Col span={8}>
              <Search
                placeholder="Tìm kiếm..."
                allowClear
                onChange={(e) => debouncedSearch(e.target.value)}
                prefix={<SearchOutlined />}
              />
            </Col>
            <Col span={4}>
              <Select
                placeholder="Loại dữ liệu"
                allowClear
                mode="multiple"
                style={{ width: '100%' }}
                value={selectedDataTypes}
                onChange={setSelectedDataTypes}
              >
                {Object.entries(dataTypeDefinitions).map(([key, def]) => (
                  <Option key={key} value={key}>
                    {def.icon} {key}
                  </Option>
                ))}
              </Select>
            </Col>
            <Col span={4}>
              <Select
                value={sortBy}
                onChange={setSortBy}
                style={{ width: '100%' }}
              >
                <Option value="ImportDate">Ngày import</Option>
                <Option value="RecordsCount">Số bản ghi</Option>
                <Option value="FileName">Tên file</Option>
              </Select>
            </Col>
            <Col span={4}>
              <Select
                value={sortOrder}
                onChange={setSortOrder}
                style={{ width: '100%' }}
              >
                <Option value="desc">Giảm dần</Option>
                <Option value="asc">Tăng dần</Option>
              </Select>
            </Col>
            <Col span={4}>
              <Space>
                <Button
                  icon={<ReloadOutlined />}
                  onClick={() => enableVirtualScroll ? loadVirtualData(0, pageSize) : loadData()}
                  loading={loading}
                >
                  Làm mới
                </Button>
                <Button
                  icon={<BarChartOutlined />}
                  onClick={() => setPerformanceVisible(true)}
                >
                  Hiệu suất
                </Button>
              </Space>
            </Col>
          </Row>
        </Card>
      )}

      <Card>
        {enableVirtualScroll ? (
          <div>
            <div style={{ marginBottom: 8 }}>
              <Text type="secondary">
                Đang hiển thị {virtualData.length} / {stats.totalRecords} bản ghi
                {isLoadingMore && <Spin size="small" style={{ marginLeft: 8 }} />}
              </Text>
            </div>
            <List
              ref={listRef}
              height={height}
              itemCount={virtualData.length + (hasNextPage ? 1 : 0)}
              itemSize={50}
              itemData={virtualData}
            >
              {VirtualRow}
            </List>
          </div>
        ) : (
          <Table
            columns={columns}
            dataSource={data}
            loading={loading}
            pagination={{
              current: page,
              pageSize: pageSize,
              total: total,
              showSizeChanger: true,
              showQuickJumper: true,
              showTotal: (total, range) => 
                `${range[0]}-${range[1]} của ${total} bản ghi`,
              onChange: (pageNum, size) => {
                setPage(pageNum);
                setPageSize(size || pageSize);
              }
            }}
            scroll={{ x: 1200, y: height }}
            size="small"
            rowKey="id"
          />
        )}
      </Card>

      <Drawer
        title="Thống kê hiệu suất"
        placement="right"
        width={400}
        open={performanceVisible}
        onClose={() => setPerformanceVisible(false)}
      >
        {performanceData && (
          <div>
            <Card size="small" style={{ marginBottom: 16 }}>
              <Statistic
                title="Tổng số truy vấn (24h)"
                value={performanceData.totalQueries}
              />
            </Card>
            <Card size="small" style={{ marginBottom: 16 }}>
              <Statistic
                title="Thời gian phản hồi trung bình"
                value={performanceData.avgResponseTime}
                formatter={(value) => rawDataService.formatPerformanceMetric(Number(value))}
              />
            </Card>
            <Card size="small" style={{ marginBottom: 16 }}>
              <Statistic
                title="Cache Hit Rate tổng"
                value={performanceData.cacheHitRate}
                formatter={(value) => `${value}%`}
              />
            </Card>
          </div>
        )}
      </Drawer>
    </div>
  );
};

// Utility function for debouncing
function debounce<T extends (...args: any[]) => any>(
  func: T,
  wait: number
): (...args: Parameters<T>) => void {
  let timeout: NodeJS.Timeout | null = null;
  return (...args: Parameters<T>) => {
    if (timeout) clearTimeout(timeout);
    timeout = setTimeout(() => func(...args), wait);
  };
}

export default OptimizedDataTable;
