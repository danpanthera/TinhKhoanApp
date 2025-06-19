import React, { useState, useRef } from 'react';
import {
  Card,
  Upload,
  Button,
  Select,
  Form,
  message,
  Progress,
  Table,
  Tag,
  Space,
  Alert,
  Typography,
  Row,
  Col,
  Divider,
  Modal,
  Steps
} from 'antd';
import {
  UploadOutlined,
  FileExcelOutlined,
  CheckCircleOutlined,
  ExclamationCircleOutlined,
  LoadingOutlined,
  HistoryOutlined
} from '@ant-design/icons';
import { importRawData, validateExcelFile } from '@/services/rawDataService';
import type { UploadFile, UploadProps } from 'antd';

const { Title, Text } = Typography;
const { Option } = Select;
const { Step } = Steps;

interface ImportResult {
  success: boolean;
  message: string;
  batchId: string;
  statistics: {
    tableName: string;
    totalRecordsProcessed: number;
    newRecords: number;
    updatedRecords: number;
    deletedRecords: number;
  };
  warnings: string[];
  errors: string[];
}

interface ValidationResult {
  isValid: boolean;
  message: string;
  missingHeaders: string[];
  extraHeaders: string[];
  rowCount: number;
}

export const RawDataImport: React.FC = () => {
  const [form] = Form.useForm();
  const [currentStep, setCurrentStep] = useState(0);
  const [selectedTable, setSelectedTable] = useState<string>('');
  const [fileList, setFileList] = useState<UploadFile[]>([]);
  const [uploading, setUploading] = useState(false);
  const [validating, setValidating] = useState(false);
  const [validationResult, setValidationResult] = useState<ValidationResult | null>(null);
  const [importResult, setImportResult] = useState<ImportResult | null>(null);
  const [importProgress, setImportProgress] = useState(0);
  const [showHistory, setShowHistory] = useState(false);
  const fileInputRef = useRef<any>(null);

  const tableOptions = [
    { value: 'LN01', label: 'LN01 - Thông tin Công ty', description: 'Dữ liệu thông tin cơ bản của công ty' },
    { value: 'GL01', label: 'GL01 - Bút toán Kế toán', description: 'Dữ liệu bút toán và chứng từ kế toán' },
    { value: 'DP01', label: 'DP01 - Thông tin Khách hàng', description: 'Dữ liệu chính về khách hàng' }
  ];

  const steps = [
    {
      title: 'Chọn bảng dữ liệu',
      description: 'Chọn loại bảng cần import'
    },
    {
      title: 'Tải file Excel',
      description: 'Upload và validate file dữ liệu'
    },
    {
      title: 'Xác nhận import',
      description: 'Kiểm tra và thực hiện import'
    },
    {
      title: 'Kết quả',
      description: 'Xem kết quả import'
    }
  ];

  const handleTableSelect = (value: string) => {
    setSelectedTable(value);
    setCurrentStep(1);
    setFileList([]);
    setValidationResult(null);
    setImportResult(null);
  };

  const handleFileChange: UploadProps['onChange'] = (info) => {
    setFileList(info.fileList.slice(-1)); // Only keep the last file
    
    if (info.file.status === 'done') {
      handleFileValidation(info.file.originFileObj as File);
    }
  };

  const handleFileValidation = async (file: File) => {
    if (!selectedTable) {
      message.error('Vui lòng chọn loại bảng trước');
      return;
    }

    setValidating(true);
    try {
      const result = await validateExcelFile(file, selectedTable);
      setValidationResult(result);
      
      if (result.isValid) {
        message.success('File Excel hợp lệ!');
        setCurrentStep(2);
      } else {
        message.error(result.message);
      }
    } catch (error) {
      message.error('Lỗi khi validate file');
      console.error('Validation error:', error);
    } finally {
      setValidating(false);
    }
  };

  const handleImport = async () => {
    if (!fileList[0] || !selectedTable) {
      message.error('Thiếu thông tin cần thiết để import');
      return;
    }

    const formData = new FormData();
    formData.append('file', fileList[0].originFileObj as File);
    formData.append('tableName', selectedTable);
    formData.append('batchId', `${selectedTable}_${Date.now()}`);
    formData.append('createdBy', 'USER'); // Should get from auth context

    setUploading(true);
    setImportProgress(0);

    // Simulate progress
    const progressInterval = setInterval(() => {
      setImportProgress(prev => {
        if (prev >= 90) {
          clearInterval(progressInterval);
          return prev;
        }
        return prev + Math.random() * 10;
      });
    }, 500);

    try {
      const result = await importRawData(selectedTable, formData);
      setImportResult(result);
      setImportProgress(100);
      setCurrentStep(3);
      
      if (result.success) {
        message.success('Import dữ liệu thành công!');
      } else {
        message.error('Import thất bại');
      }
    } catch (error) {
      message.error('Lỗi khi import dữ liệu');
      console.error('Import error:', error);
    } finally {
      setUploading(false);
      clearInterval(progressInterval);
    }
  };

  const handleReset = () => {
    setCurrentStep(0);
    setSelectedTable('');
    setFileList([]);
    setValidationResult(null);
    setImportResult(null);
    setImportProgress(0);
    form.resetFields();
  };

  const renderStepContent = () => {
    switch (currentStep) {
      case 0:
        return (
          <div style={{ textAlign: 'center', padding: '40px 0' }}>
            <Title level={4}>Chọn loại bảng dữ liệu cần import</Title>
            <Select
              style={{ width: '100%', maxWidth: 400 }}
              placeholder="Chọn bảng dữ liệu"
              size="large"
              onChange={handleTableSelect}
              value={selectedTable}
            >
              {tableOptions.map(option => (
                <Option key={option.value} value={option.value}>
                  <div>
                    <div style={{ fontWeight: 'bold' }}>{option.label}</div>
                    <div style={{ fontSize: '12px', color: '#666' }}>
                      {option.description}
                    </div>
                  </div>
                </Option>
              ))}
            </Select>
          </div>
        );

      case 1:
        return (
          <div>
            <Title level={4}>
              <FileExcelOutlined /> Upload file Excel cho {selectedTable}
            </Title>
            
            <Upload.Dragger
              name="file"
              accept=".xlsx,.xls"
              fileList={fileList}
              onChange={handleFileChange}
              beforeUpload={() => false} // Prevent auto upload
              maxCount={1}
            >
              <p className="ant-upload-drag-icon">
                <UploadOutlined />
              </p>
              <p className="ant-upload-text">Click hoặc kéo file vào đây để upload</p>
              <p className="ant-upload-hint">
                Chỉ hỗ trợ file Excel (.xlsx, .xls). Tối đa 1 file.
              </p>
            </Upload.Dragger>

            {validating && (
              <div style={{ textAlign: 'center', margin: '20px 0' }}>
                <LoadingOutlined style={{ fontSize: 24 }} />
                <div>Đang validate file...</div>
              </div>
            )}

            {validationResult && (
              <Alert
                style={{ marginTop: 16 }}
                message={validationResult.isValid ? 'File hợp lệ' : 'File không hợp lệ'}
                description={
                  <div>
                    <div>{validationResult.message}</div>
                    <div>Số dòng dữ liệu: {validationResult.rowCount}</div>
                    {validationResult.missingHeaders.length > 0 && (
                      <div style={{ color: 'red' }}>
                        Thiếu cột: {validationResult.missingHeaders.join(', ')}
                      </div>
                    )}
                    {validationResult.extraHeaders.length > 0 && (
                      <div style={{ color: 'orange' }}>
                        Cột thừa: {validationResult.extraHeaders.join(', ')}
                      </div>
                    )}
                  </div>
                }
                type={validationResult.isValid ? 'success' : 'error'}
                showIcon
              />
            )}
          </div>
        );

      case 2:
        return (
          <div>
            <Title level={4}>
              <CheckCircleOutlined /> Xác nhận thông tin import
            </Title>
            
            <Card>
              <Row gutter={16}>
                <Col span={12}>
                  <Text strong>Bảng dữ liệu:</Text> {selectedTable}
                </Col>
                <Col span={12}>
                  <Text strong>File:</Text> {fileList[0]?.name}
                </Col>
              </Row>
              
              {validationResult && (
                <div style={{ marginTop: 16 }}>
                  <Text strong>Số dòng dữ liệu:</Text> {validationResult.rowCount}
                </div>
              )}
            </Card>

            <div style={{ textAlign: 'center', margin: '30px 0' }}>
              <Button 
                type="primary" 
                size="large"
                onClick={handleImport}
                loading={uploading}
                disabled={!validationResult?.isValid}
              >
                Bắt đầu Import
              </Button>
            </div>

            {uploading && (
              <Card>
                <div style={{ textAlign: 'center' }}>
                  <Progress percent={Math.round(importProgress)} />
                  <div style={{ marginTop: 8 }}>Đang xử lý dữ liệu...</div>
                </div>
              </Card>
            )}
          </div>
        );

      case 3:
        return (
          <div>
            <Title level={4}>
              {importResult?.success ? (
                <><CheckCircleOutlined style={{ color: 'green' }} /> Import thành công</>
              ) : (
                <><ExclamationCircleOutlined style={{ color: 'red' }} /> Import thất bại</>
              )}
            </Title>

            {importResult && (
              <Card>
                <Alert
                  message={importResult.message}
                  type={importResult.success ? 'success' : 'error'}
                  showIcon
                  style={{ marginBottom: 16 }}
                />

                {importResult.success && importResult.statistics && (
                  <Row gutter={16}>
                    <Col span={6}>
                      <Statistic
                        title="Tổng bản ghi"
                        value={importResult.statistics.totalRecordsProcessed}
                      />
                    </Col>
                    <Col span={6}>
                      <Statistic
                        title="Bản ghi mới"
                        value={importResult.statistics.newRecords}
                        valueStyle={{ color: '#52c41a' }}
                      />
                    </Col>
                    <Col span={6}>
                      <Statistic
                        title="Cập nhật"
                        value={importResult.statistics.updatedRecords}
                        valueStyle={{ color: '#1890ff' }}
                      />
                    </Col>
                    <Col span={6}>
                      <Statistic
                        title="Xóa"
                        value={importResult.statistics.deletedRecords}
                        valueStyle={{ color: '#ff4d4f' }}
                      />
                    </Col>
                  </Row>
                )}

                {importResult.warnings.length > 0 && (
                  <Alert
                    message="Cảnh báo"
                    description={
                      <ul>
                        {importResult.warnings.map((warning, index) => (
                          <li key={index}>{warning}</li>
                        ))}
                      </ul>
                    }
                    type="warning"
                    showIcon
                    style={{ marginTop: 16 }}
                  />
                )}

                {importResult.errors.length > 0 && (
                  <Alert
                    message="Lỗi"
                    description={
                      <ul>
                        {importResult.errors.map((error, index) => (
                          <li key={index}>{error}</li>
                        ))}
                      </ul>
                    }
                    type="error"
                    showIcon
                    style={{ marginTop: 16 }}
                  />
                )}

                <div style={{ textAlign: 'center', marginTop: 24 }}>
                  <Space>
                    <Button onClick={handleReset}>
                      Import bảng khác
                    </Button>
                    <Button 
                      icon={<HistoryOutlined />}
                      onClick={() => setShowHistory(true)}
                    >
                      Xem lịch sử
                    </Button>
                  </Space>
                </div>
              </Card>
            )}
          </div>
        );

      default:
        return null;
    }
  };

  return (
    <div style={{ padding: '24px' }}>
      <Title level={2}>Import Dữ liệu Thô</Title>
      
      <Card>
        <Steps current={currentStep} style={{ marginBottom: 32 }}>
          {steps.map((step, index) => (
            <Step 
              key={index}
              title={step.title}
              description={step.description}
            />
          ))}
        </Steps>

        <Divider />

        {renderStepContent()}
      </Card>
    </div>
  );
};
