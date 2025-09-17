using System;
using System.Collections.Generic;

namespace Khoan.Api.Models.Generated;

public partial class Dp01New
{
    public int Id { get; set; }

    public string NgayDl { get; set; } = null!;

    public string? MaCn { get; set; }

    public string? MaPgd { get; set; }

    public string? TaiKhoanHachToan { get; set; }

    public decimal? CurrentBalance { get; set; }

    public decimal? SoDuDauKy { get; set; }

    public decimal? SoPhatSinhNo { get; set; }

    public decimal? SoPhatSinhCo { get; set; }

    public decimal? SoDuCuoiKy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? FileName { get; set; }
}
