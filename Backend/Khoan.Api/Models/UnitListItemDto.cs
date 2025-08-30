namespace TinhKhoanApp.Api.Models // Hoặc TinhKhoanApp.Api.Models.Dtos nếu Sếp tạo thư mục Dtos
{
    public class UnitListItemDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Type { get; set; }
        public int? ParentUnitId { get; set; }
        public string? ParentUnitName { get; set; } // Chỉ lấy tên của đơn vị cha
    }
}