import React, { useState, useEffect } from 'react';
import {
  Table,
  Card,
  DatePicker,
  Select,
  Input,
  Button,
  Tag,
  Timeline,
  Modal,
  Descriptions,
  Typography,
  Space,
  Row,
  Col,
  Alert,
  Tooltip
} from 'antd';
import {
  HistoryOutlined,
  SearchOutlined,
  EyeOutlined,
  CalendarOutlined,
  FileTextOutlined,
  DiffOutlined
} from '@ant-design/icons';
import { getLN01History, getGL01History, getLN01Snapshot, getGL01Snapshot } from '@/services/rawDataService';
import type { ColumnsType } from 'antd/lib/table';
import moment from 'moment';

const { Title, Text } = Typography;
const { Option } = Select;
const { RangePicker } = DatePicker;

interface HistoryRecord {
  historyID: number;
  sourceID: string;
  validFrom: string;
  validTo: string;
  isCurrent: boolean;
  versionNumber: number;
  createdDate: string;
  modifiedDate: string;
  [key: string]: any;
}

interface DataHistoryProps {
  tableName?: string;
  sourceId?: string;
  onClose?: () => void;
}

export const DataHistory: React.FC<DataHistoryProps> = ({ 
  tableName = 'LN01', 
  sourceId,
  onClose 
}) => {
  const [historyData, setHistoryData] = useState<HistoryRecord[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchSourceId, setSearchSourceId] = useState(sourceId || '');
  const [selectedRecord, setSelectedRecord] = useState<HistoryRecord | null>(null);
  const [showDetailModal, setShowDetailModal] = useState(false);
  const [compareModalVisible, setCompareModalVisible] = useState(false);
  const [compareRecords, setCompareRecords] = useState<HistoryRecord[]>([]);
  const [snapshotDate, setSnapshotDate] = useState<moment.Moment | null>(null);
  const [snapshotData, setSnapshotData] = useState<HistoryRecord[]>([]);

  useEffect(() => {
    if (sourceId) {
      loadHistory(sourceId);
    }
  }, [tableName, sourceId]);

  const loadHistory = async (id: string) => {
    if (!id.trim()) return;

    setLoading(true);
    try {
      let data: HistoryRecord[] = [];
      
      if (tableName === 'LN01') {
        data = await getLN01History(id);
      } else if (tableName === 'GL01') {
        data = await getGL01History(id);
      }
      
      setHistoryData(data);
    } catch (error) {
      console.error('Error loading history:', error);
    } finally {
      setLoading(false);
    }
  };

  const loadSnapshot = async (date: moment.Moment) => {
    setLoading(true);
    try {
      let data: HistoryRecord[] = [];
      const dateStr = date.format('YYYY-MM-DD');
      
      if (tableName === 'LN01') {
        data = await getLN01Snapshot(dateStr);
      } else if (tableName === 'GL01') {
        data = await getGL01Snapshot(dateStr);
      }
      
      setSnapshotData(data);
    } catch (error) {
      console.error('Error loading snapshot:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = () => {
    loadHistory(searchSourceId);
  };

  const handleViewDetails = (record: HistoryRecord) => {
    setSelectedRecord(record);
    setShowDetailModal(true);
  };

  const handleCompare = (record1: HistoryRecord, record2: HistoryRecord) => {
    setCompareRecords([record1, record2]);
    setCompareModalVisible(true);
  };

  const getChangedFields = (oldRecord: HistoryRecord, newRecord: HistoryRecord) => {
    const changes: Array<{field: string, oldValue: any, newValue: any}> = [];
    
    Object.keys(newRecord).forEach(key => {
      if (key !== 'historyID' && key !== 'validFrom' && key !== 'validTo' && 
          key !== 'createdDate' && key !== 'modifiedDate' && key !== 'versionNumber') {
        if (oldRecord[key] !== newRecord[key]) {
          changes.push({
            field: key,
            oldValue: oldRecord[key],
            newValue: newRecord[key]
          });
        }
      }
    });
    
    return changes;
  };

  const historyColumns: ColumnsType<HistoryRecord> = [
    {
      title: 'Phiên bản',
      dataIndex: 'versionNumber',
      width: 100,
      sorter: (a, b) => a.versionNumber - b.versionNumber,
    },
    {
      title: 'Có hiệu lực từ',
      dataIndex: 'validFrom',
      width: 150,
      render: (date: string) => moment(date).format('DD/MM/YYYY HH:mm'),
      sorter: (a, b) => moment(a.validFrom).unix() - moment(b.validFrom).unix(),
    },
    {
      title: 'Có hiệu lực đến',
      dataIndex: 'validTo',
      width: 150,
      render: (date: string) => {
        if (moment(date).year() === 9999) {
          return <Tag color="green">Hiện tại</Tag>;
        }
        return moment(date).format('DD/MM/YYYY HH:mm');
      },
    },
    {
      title: 'Trạng thái',
      dataIndex: 'isCurrent',
      width: 100,
      render: (isCurrent: boolean) => (
        <Tag color={isCurrent ? 'green' : 'default'}>
          {isCurrent ? 'Hiện tại' : 'Lịch sử'}
        </Tag>
      ),
    },
    {
      title: 'Ngày tạo',
      dataIndex: 'createdDate',
      width: 150,
      render: (date: string) => moment(date).format('DD/MM/YYYY HH:mm'),
    },
    {
      title: 'Hành động',
      width: 200,
      render: (_, record, index) => (
        <Space>
          <Tooltip title="Xem chi tiết">
            <Button 
              type="text" 
              icon={<EyeOutlined />} 
              onClick={() => handleViewDetails(record)}
            />
          </Tooltip>
          {index < historyData.length - 1 && (
            <Tooltip title="So sánh với phiên bản trước">
              <Button 
                type="text" 
                icon={<DiffOutlined />} 
                onClick={() => handleCompare(historyData[index + 1], record)}
              />
            </Tooltip>
          )}
        </Space>
      ),
    },
  ];

  const renderCompareModal = () => {
    if (compareRecords.length !== 2) return null;
    
    const [oldRecord, newRecord] = compareRecords;
    const changes = getChangedFields(oldRecord, newRecord);
    
    return (
      <Modal
        title="So sánh phiên bản"
        open={compareModalVisible}
        onCancel={() => setCompareModalVisible(false)}
        footer={null}
        width={800}
      >
        <Row gutter={16}>
          <Col span={12}>
            <Title level={5}>
              Phiên bản {oldRecord.versionNumber} 
              <Text type="secondary">
                ({moment(oldRecord.validFrom).format('DD/MM/YYYY HH:mm')})
              </Text>
            </Title>
          </Col>
          <Col span={12}>
            <Title level={5}>
              Phiên bản {newRecord.versionNumber}
              <Text type="secondary">
                ({moment(newRecord.validFrom).format('DD/MM/YYYY HH:mm')})
              </Text>
            </Title>
          </Col>
        </Row>

        <Alert
          message={`Có ${changes.length} thay đổi`}
          type="info"
          style={{ marginBottom: 16 }}
        />

        <Timeline>
          {changes.map((change, index) => (
            <Timeline.Item key={index}>
              <div>
                <Text strong>{change.field}:</Text>
                <div style={{ marginLeft: 16 }}>
                  <Text delete style={{ color: '#ff4d4f' }}>{change.oldValue || 'N/A'}</Text>
                  <br />
                  <Text style={{ color: '#52c41a' }}>{change.newValue || 'N/A'}</Text>
                </div>
              </div>
            </Timeline.Item>
          ))}
        </Timeline>
      </Modal>
    );
  };

  return (
    <div>
      <Card>
        <Row gutter={16} align="middle" style={{ marginBottom: 16 }}>
          <Col>
            <Title level={4}>
              <HistoryOutlined /> Lịch sử dữ liệu {tableName}
            </Title>
          </Col>
        </Row>

        <Row gutter={16} style={{ marginBottom: 16 }}>
          <Col flex={1}>
            <Input
              placeholder="Nhập Source ID để tìm lịch sử"
              value={searchSourceId}
              onChange={(e) => setSearchSourceId(e.target.value)}
              onPressEnter={handleSearch}
            />
          </Col>
          <Col>
            <Button 
              type="primary" 
              icon={<SearchOutlined />} 
              onClick={handleSearch}
            >
              Tìm kiếm
            </Button>
          </Col>
        </Row>

        <Row gutter={16} style={{ marginBottom: 16 }}>
          <Col>
            <DatePicker
              placeholder="Chọn ngày để xem snapshot"
              value={snapshotDate}
              onChange={(date) => {
                setSnapshotDate(date);
                if (date) {
                  loadSnapshot(date);
                }
              }}
            />
          </Col>
        </Row>

        {historyData.length > 0 && (
          <Table
            columns={historyColumns}
            dataSource={historyData}
            loading={loading}
            pagination={{ pageSize: 10 }}
            rowKey="historyID"
            size="small"
          />
        )}

        {snapshotData.length > 0 && (
          <Card title={`Snapshot tại ${snapshotDate?.format('DD/MM/YYYY')}`} style={{ marginTop: 16 }}>
            <Text>Tìm thấy {snapshotData.length} bản ghi tại thời điểm này</Text>
            {/* Add snapshot data table here if needed */}
          </Card>
        )}
      </Card>

      {/* Detail Modal */}
      <Modal
        title={`Chi tiết phiên bản ${selectedRecord?.versionNumber}`}
        open={showDetailModal}
        onCancel={() => setShowDetailModal(false)}
        footer={null}
        width={800}
      >
        {selectedRecord && (
          <Descriptions column={2} bordered size="small">
            <Descriptions.Item label="Source ID">
              {selectedRecord.sourceID}
            </Descriptions.Item>
            <Descriptions.Item label="Phiên bản">
              {selectedRecord.versionNumber}
            </Descriptions.Item>
            <Descriptions.Item label="Có hiệu lực từ">
              {moment(selectedRecord.validFrom).format('DD/MM/YYYY HH:mm:ss')}
            </Descriptions.Item>
            <Descriptions.Item label="Có hiệu lực đến">
              {moment(selectedRecord.validTo).year() === 9999 
                ? 'Hiện tại' 
                : moment(selectedRecord.validTo).format('DD/MM/YYYY HH:mm:ss')
              }
            </Descriptions.Item>
            <Descriptions.Item label="Trạng thái">
              <Tag color={selectedRecord.isCurrent ? 'green' : 'default'}>
                {selectedRecord.isCurrent ? 'Hiện tại' : 'Lịch sử'}
              </Tag>
            </Descriptions.Item>
            <Descriptions.Item label="Ngày tạo">
              {moment(selectedRecord.createdDate).format('DD/MM/YYYY HH:mm:ss')}
            </Descriptions.Item>
            
            {/* Dynamic fields */}
            {Object.keys(selectedRecord)
              .filter(key => !['historyID', 'sourceID', 'validFrom', 'validTo', 
                             'isCurrent', 'versionNumber', 'createdDate', 'modifiedDate', 'recordHash'].includes(key))
              .map(key => (
                <Descriptions.Item label={key} key={key}>
                  {selectedRecord[key] || 'N/A'}
                </Descriptions.Item>
              ))
            }
          </Descriptions>
        )}
      </Modal>

      {renderCompareModal()}
    </div>
  );
};
