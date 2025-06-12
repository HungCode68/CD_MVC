namespace NailMVC.Models
{
    public class LichHenViewModel
    {
        public int Id { get; set; }
        public DateTime? NgayHen { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; }

        public int KhachHangId { get; set; }
        public string TenKhachHang { get; set; }

        public int NhanVienId { get; set; }
        public string TenNhanVien { get; set; }

        public int DichVuId { get; set; }
        public string TenDichVu { get; set; }
    }
}
