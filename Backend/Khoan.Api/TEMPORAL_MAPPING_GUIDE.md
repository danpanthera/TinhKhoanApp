# Hướng dẫn bật Temporal Mapping theo bảng

Temporal Mapping cho phép theo dõi lịch sử thay đổi của từng bảng thông qua SysStartTime/SysEndTime và bảng _History tương ứng.

## Cấu hình nhanh (Development)

Chỉnh `appsettings.Development.json` trong `Khoan.Api`:

```json
{
  "TemporalMapping": {
    "DP01": true,
    "LN01": false,
    "LN03": false,
    "DPDA": false,
    "EI01": false,
    "RR01": false,
    "GL41": false,
    "GL01": false,
    "GL02": false
  }
}
```

- Khuyến nghị Dev: bật `DP01`, tắt các bảng còn lại để khởi động nhanh và tránh xung đột mô hình.
- Khi bật temporal cho một bảng, EF sẽ cấu hình cột kỳ hạn (SysStartTime/SysEndTime) dưới dạng shadow property. Không khai báo hai cột này trong class entity.

## Nguyên tắc an toàn khi bật

- Bật dần từng bảng (một-đến-hai bảng mỗi lần) và chạy API để kiểm tra:
  - Startup không báo lỗi về temporal/khóa chính.
  - Các truy vấn đến bảng đó vẫn đúng.
- Với các bảng có thuộc tính `Column(Order=...)` cần đảm bảo thứ tự duy nhất (ví dụ đã sửa `LN03`).

## Kiểm tra nhanh

- Gọi `GET /ready` để kiểm tra app sẵn sàng.
- Nếu gặp lỗi temporal kiểu "Period property 'X.SysStartTime' must be a shadow property":
  - Xoá thuộc tính `SysStartTime`/`SysEndTime` trong entity X (giữ shadow property).

## Prod

- Đánh giá dung lượng và tần suất thay đổi bảng trước khi bật toàn cục.
- Duy trì index phù hợp (đã cấu hình trong `ApplicationDbContext`).
