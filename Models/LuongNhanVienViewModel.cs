namespace NailMVC.Models
{
    public class LuongNhanVienViewModel
    {
        public int Id { get; set; }
        public int NhanVienId { get; set; }
        public string HoTenNhanVien { get; set; } = string.Empty;
        public int ThangLuong { get; set; }
        public int NamLuong { get; set; }
        public decimal PhanTramHoaDon { get; set; }
        public decimal? TienTip { get; set; }
        public decimal? ThuongKhac { get; set; }
        public string? GhiChu { get; set; }
        public DateTime NgayTinhLuong { get; set; }
        public int TongHoaDon { get; set; }
        public decimal TongLuong { get; set; }

    }
}
