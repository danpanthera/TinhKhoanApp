using System;
using System.ComponentModel.DataAnnotations;

namespace TinhKhoanApp.Api.Models.Dashboard
{
    /// <summary>
    /// Định nghĩa các chỉ tiêu Dashboard - 6 chỉ tiêu chính của ngân hàng
    /// Nguồn vốn huy động, Dư nợ cho vay, Tỷ lệ nợ xấu, Thu hồi nợ XLRR, Thu dịch vụ, Lợi nhuận
    /// </summary>
    public class DashboardIndicator
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty; // HuyDong, DuNo, TyLeNoXau, ThuHoiXLRR, ThuDichVu, LoiNhuan
        
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty; // Tên chỉ tiêu hiển thị
        
        [MaxLength(50)]
        public string? Unit { get; set; } // Đơn vị tính (tỷ đồng, %, triệu đồng)
        
        [MaxLength(100)]
        public string? Icon { get; set; } // Icon Material Design (mdi-xxx)
        
        [MaxLength(10)]
        public string? Color { get; set; } // Màu sắc hex (#4CAF50)
        
        public int SortOrder { get; set; } // Thứ tự hiển thị (1-6)
        
        public bool IsActive { get; set; } = true; // Có hiển thị hay không
        
        [MaxLength(500)]
        public string? Description { get; set; } // Mô tả chi tiết chỉ tiêu
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }
        
        // Soft delete
        public bool IsDeleted { get; set; } = false;
    }
}
