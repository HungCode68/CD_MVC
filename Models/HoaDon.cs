namespace NailMVC.Models
{
    public class HoaDon
    {
        public int Id { get; set; }
        public DateTime NgayLap { get; set; }
        public int KhachHangId { get; set; }
        public int NhanVienId { get; set; }
        public decimal TongTien { get; set; }
        public decimal GiamGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string HinhThucThanhToan { get; set; } // "TienMat", "ChuyenKhoan"


        public KhachHang KhachHang { get; set; }
        public NhanVien NhanVien { get; set; }

        public List<HoaDon_DichVu> HoaDonDichVus { get; set; }
    }
}
