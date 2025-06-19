# SCD Type 2 với xử lý bản ghi bị xóa

## 📋 **Tổng quan**

Hệ thống đã được cập nhật để xử lý đúng trường hợp **giảm bản ghi** trong các lần import tiếp theo, sử dụng SCD Type 2 với tracking cho các bản ghi bị xóa.

## 🔧 **Những thay đổi đã thực hiện**

### 1. **Cập nhật Model**
- Thêm field `IsDeleted: bool` vào `SCDType2BaseEntity`
- Thêm field `RecordsDeleted: int` vào `SCDType2Result`

### 2. **Cập nhật Database**
```sql
ALTER TABLE RawDataRecords_SCD 
ADD COLUMN IsDeleted BOOLEAN NOT NULL DEFAULT 0;

-- Indexes cho performance
CREATE INDEX IX_RawDataRecords_SCD_IsDeleted ON RawDataRecords_SCD (IsDeleted);
CREATE INDEX IX_RawDataRecords_SCD_DataSource_IsDeleted_IsCurrent ON RawDataRecords_SCD (DataSource, IsDeleted, IsCurrent);
```

### 3. **Cập nhật Service Logic**

#### **Bước 1: Xử lý bản ghi có trong file mới**
```csharp
var newSourceIds = new HashSet<string>();
foreach (var newRecord in newRecords) {
    // Logic xử lý insert/update như cũ
    newSourceIds.Add(sourceId);
}
```

#### **Bước 2: Xử lý bản ghi bị xóa**
```csharp
var deletedRecords = await _context.RawDataRecords_SCD
    .Where(r => r.DataSource == dataSource && 
               r.BranchCode == branchCode &&
               r.DataType == importInfo.DataType &&
               r.IsCurrent && 
               !r.IsDeleted &&
               !newSourceIds.Contains(r.SourceId))
    .ToListAsync();

foreach (var deletedRecord in deletedRecords) {
    // Expire bản cũ
    deletedRecord.ValidTo = DateTime.UtcNow;
    deletedRecord.IsCurrent = false;
    
    // Tạo version mới đánh dấu deleted
    var deletedScdRecord = new RawDataRecord_SCD {
        // ... copy properties
        VersionNumber = deletedRecord.VersionNumber + 1,
        IsDeleted = true,
        IsCurrent = true,
        ProcessingStatus = "Deleted"
    };
}
```

## 📊 **Ví dụ hoạt động**

### **Trường hợp: File giảm từ 2100 → 2095 bản ghi**

**File 1: `7801_LN01_20250531.csv` (2100 bản ghi)**
```
Record_001: Version 1, IsCurrent=true, IsDeleted=false
Record_002: Version 1, IsCurrent=true, IsDeleted=false
...
Record_2100: Version 1, IsCurrent=true, IsDeleted=false
```

**File 2: `7801_LN01_20250601.csv` (2095 bản ghi - thiếu 5 bản ghi)**
```
Record_001: Không thay đổi → Giữ nguyên
Record_002: Có thay đổi → Tạo Version 2
...
Record_2096-2100: Không có trong file mới → Đánh dấu deleted
```

**Kết quả trong database:**
```
Record_001: Version 1, IsCurrent=true, IsDeleted=false
Record_002: Version 1, IsCurrent=false (expired)
Record_002: Version 2, IsCurrent=true, IsDeleted=false
...
Record_2096: Version 1, IsCurrent=false (expired)
Record_2096: Version 2, IsCurrent=true, IsDeleted=true
...
Record_2100: Version 1, IsCurrent=false (expired)
Record_2100: Version 2, IsCurrent=true, IsDeleted=true
```

## 🔍 **Query kiểm tra**

### **1. Tổng quan theo import**
```sql
SELECT 
    DataType, BranchCode, DATE(StatementDate),
    COUNT(*) as TotalRecords,
    SUM(CASE WHEN IsDeleted = 0 THEN 1 ELSE 0 END) as ActiveRecords,
    SUM(CASE WHEN IsDeleted = 1 THEN 1 ELSE 0 END) as DeletedRecords
FROM RawDataRecords_SCD
WHERE IsCurrent = 1
GROUP BY DataType, BranchCode, DATE(StatementDate);
```

### **2. Lịch sử version của 1 record**
```sql
SELECT SourceId, VersionNumber, IsCurrent, IsDeleted, ValidFrom, ValidTo
FROM RawDataRecords_SCD
WHERE SourceId = 'specific_source_id'
ORDER BY VersionNumber;
```

### **3. Các bản ghi bị xóa gần đây**
```sql
SELECT SourceId, DataType, BranchCode, ValidFrom, ProcessingNotes
FROM RawDataRecords_SCD
WHERE IsDeleted = 1 AND IsCurrent = 1
ORDER BY ValidFrom DESC;
```

## ✅ **Lợi ích**

1. **Theo dõi đầy đủ**: Biết được record nào bị xóa, khi nào
2. **Audit trail**: Giữ lại lịch sử thay đổi hoàn chỉnh  
3. **Performance**: Chỉ lưu thay đổi, không duplicate data
4. **Compliance**: Đáp ứng yêu cầu audit và regulatory
5. **Recovery**: Có thể khôi phục data tại bất kỳ thời điểm nào

## 🚀 **Sử dụng**

Service được gọi tự động khi import file mới. Không cần thay đổi gì ở frontend hoặc API calls hiện tại.

Kết quả trả về sẽ bao gồm thêm:
```json
{
  "recordsProcessed": 2095,
  "recordsInserted": 0,
  "recordsUpdated": 50,
  "recordsDeleted": 5,
  "recordsExpired": 55
}
```
