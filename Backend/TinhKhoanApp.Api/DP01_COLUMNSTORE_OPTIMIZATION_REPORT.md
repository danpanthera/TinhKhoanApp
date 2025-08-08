# BÁO CÁO TỐI ƯU BẢNG DP01 VỚI COLUMNSTORE INDEX

**Tác giả:** GitHub Copilot
**Ngày tạo:** 08/08/2025

## I. TÓM TẮT

Báo cáo này trình bày chi tiết về việc tối ưu bảng dữ liệu DP01 bằng cách:

1. Tạo Columnstore Index cho cả bảng gốc và bảng lịch sử
2. Đề xuất phương án sắp xếp lại thứ tự vật lý các cột để tối ưu hiệu năng

## II. COLUMNSTORE INDEX

### A. Lợi ích của Columnstore Index

1. **Nén dữ liệu hiệu quả**

    - Giảm 10-100 lần dung lượng lưu trữ
    - Giảm I/O do đọc/ghi ít dữ liệu hơn từ đĩa

2. **Xử lý song song**

    - Tối ưu cho các truy vấn phân tích (aggregation)
    - Tăng tốc xử lý lên đến 100 lần cho các truy vấn OLAP

3. **Quét cột có chọn lọc**
    - Chỉ đọc các cột cần thiết, bỏ qua các cột khác
    - Tối ưu cho các báo cáo chỉ sử dụng một số cột

### B. Loại Columnstore Index đã tạo

1. **Bảng DP01 (chính):**

    - **NONCLUSTERED COLUMNSTORE INDEX**
    - Áp dụng cho cột: NGAY_DL, MA_CN, MA_DON_VI, MA_KHOAN
    - Giữ nguyên Clustered Index gốc để tối ưu cho cả OLTP và OLAP

2. **Bảng DP01_History:**
    - **CLUSTERED COLUMNSTORE INDEX**
    - Áp dụng cho toàn bộ bảng
    - Phù hợp với dữ liệu lịch sử chỉ đọc và không thay đổi

## III. SẮP XẾP CỘT VẬT LÝ

### A. Nguyên tắc sắp xếp cột

1. **Cột truy vấn thường xuyên đặt đầu tiên**

    - Tận dụng ROW/PAGE compression
    - Tối ưu cho việc tìm kiếm và lọc dữ liệu

2. **Cột có kích thước cố định trước cột có kích thước thay đổi**

    - INT, DATE trước VARCHAR, NVARCHAR
    - Tối ưu việc bố trí dữ liệu trong trang

3. **Cột có độ dài nhỏ trước cột có độ dài lớn**

    - Tối ưu việc đọc dữ liệu từ đĩa

4. **Cột NULL đặt sau cùng**
    - Tiết kiệm không gian (NULL chỉ tốn 1 bit trong bitmask)

### B. Thứ tự cột đề xuất cho DP01

1. **Cột phân đoạn dữ liệu (thường dùng trong INDEX):**

    - NGAY_DL, MA_CN, MA_DON_VI, MA_KHOAN

2. **Cột số liệu chính:**

    - SO_DU_DAU_KY, SO_PHAT_SINH_NO, SO_PHAT_SINH_CO, SO_DU_CUOI_KY

3. **Cột metadata và ít được truy vấn:**
    - Các cột thông tin bổ sung
    - Các cột có thể NULL đặt sau cùng

## IV. HƯỚNG DẪN TRIỂN KHAI

### A. Tạo Columnstore Index

1. Sử dụng script `create_dp01_columnstore.sh` để tạo Columnstore Index

    ```bash
    ./create_dp01_columnstore.sh
    ```

2. Đã tạo sẵn file SQL `sql_scripts/create_dp01_columnstore.sql`

    ```sql
    CREATE NONCLUSTERED COLUMNSTORE INDEX IX_DP01_Columnstore
    ON dbo.DP01 (NGAY_DL, MA_CN, MA_DON_VI, MA_KHOAN);

    CREATE CLUSTERED COLUMNSTORE INDEX IX_DP01_History_Columnstore
    ON dbo.DP01_History;
    ```

### B. Sắp xếp lại thứ tự cột vật lý (tùy chọn)

1. Sử dụng script `reorder_dp01_columns.sh` để thực hiện sắp xếp

    ```bash
    ./reorder_dp01_columns.sh
    ```

2. Script sẽ:

    - Tạo bảng mới với thứ tự cột tối ưu
    - Sao chép dữ liệu sang bảng mới
    - So sánh hiệu năng trước và sau khi sắp xếp

3. Để hoàn tất quá trình, cần thực hiện bước đổi tên bảng trong SQL

## V. ĐÁNH GIÁ HIỆU NĂNG DỰ KIẾN

1. **Truy vấn phân tích (aggregation):** Tăng tốc 10-100 lần
2. **Thời gian thực thi báo cáo:** Giảm 70-90%
3. **Không gian lưu trữ:** Giảm khoảng 10 lần do nén dữ liệu

## VI. KẾT LUẬN

Việc tối ưu bảng DP01 bằng Columnstore Index và sắp xếp lại thứ tự cột vật lý sẽ mang lại cải thiện đáng kể về hiệu năng và không gian lưu trữ. Đặc biệt phù hợp cho các truy vấn báo cáo phân tích lớn trên dữ liệu DP01.
