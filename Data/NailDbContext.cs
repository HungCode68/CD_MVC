using Microsoft.EntityFrameworkCore;
using NailMVC.Models;
using System.Collections.Generic;

namespace NailMVC.Data
{
    public class NailDbContext : DbContext
    {
        public NailDbContext(DbContextOptions<NailDbContext> options) : base(options)
        {
        }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<TheThanhVien> TheThanhViens { get; set; }
        public DbSet<DichVu> DichVus { get; set; }
        public DbSet<VatTu> VatTus { get; set; }
        public DbSet<KhuyenMai> KhuyenMais { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<HoaDon_DichVu> HoaDon_DichVus { get; set; }
        public DbSet<LichHen> LichHens { get; set; }
        public DbSet<LuongNhanVien> LuongNhanViens { get; set; }
        public DbSet<BaoCao> BaoCaos { get; set; }
        public DbSet<ChamCong> ChamCong { get; set; }

    }
}
