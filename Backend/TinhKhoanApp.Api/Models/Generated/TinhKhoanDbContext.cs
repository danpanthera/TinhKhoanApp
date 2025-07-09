using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TinhKhoanApp.Api.Models.Generated;

public partial class TinhKhoanDbContext : DbContext
{
    public TinhKhoanDbContext()
    {
    }

    public TinhKhoanDbContext(DbContextOptions<TinhKhoanDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Dp01New> Dp01News { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=TinhKhoanDB;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dp01New>(entity =>
        {
            entity
                .ToTable("DP01_New")
                .ToTable(tb => tb.IsTemporal(ttb =>
                    {
                        ttb.UseHistoryTable("DP01_New_History", "dbo");
                        ttb
                            .HasPeriodStart("SysStartTime")
                            .HasColumnName("SysStartTime");
                        ttb
                            .HasPeriodEnd("SysEndTime")
                            .HasColumnName("SysEndTime");
                    }));

            entity.Property(e => e.CreatedDate).HasColumnName("CREATED_DATE");
            entity.Property(e => e.CurrentBalance)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("CURRENT_BALANCE");
            entity.Property(e => e.FileName)
                .HasMaxLength(200)
                .HasColumnName("FILE_NAME");
            entity.Property(e => e.MaCn)
                .HasMaxLength(20)
                .HasColumnName("MA_CN");
            entity.Property(e => e.MaPgd)
                .HasMaxLength(20)
                .HasColumnName("MA_PGD");
            entity.Property(e => e.NgayDl)
                .HasMaxLength(10)
                .HasColumnName("NGAY_DL");
            entity.Property(e => e.SoDuCuoiKy)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("SO_DU_CUOI_KY");
            entity.Property(e => e.SoDuDauKy)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("SO_DU_DAU_KY");
            entity.Property(e => e.SoPhatSinhCo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("SO_PHAT_SINH_CO");
            entity.Property(e => e.SoPhatSinhNo)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("SO_PHAT_SINH_NO");
            entity.Property(e => e.TaiKhoanHachToan)
                .HasMaxLength(50)
                .HasColumnName("TAI_KHOAN_HACH_TOAN");
            entity.Property(e => e.UpdatedDate).HasColumnName("UPDATED_DATE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
