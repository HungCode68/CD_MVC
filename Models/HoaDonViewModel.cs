namespace NailMVC.ViewModels
{
    public class HoaDonViewModel
    {
        public int Id { get; set; }
        public DateTime? NgayLap { get; set; }

        // Khách hàng
        public int KhachHangId { get; set; }
        public string TenKhachHang { get; set; }

        // Nhân viên
        public int NhanVienId { get; set; }
        public string TenNhanVien { get; set; }

        // Danh sách dịch vụ (từ bảng chi tiết hóa đơn)
        public List<DichVuItem> DichVus { get; set; } = new();

        // Thanh toán
        public decimal TongTien { get; set; }
        public decimal GiamGia { get; set; }
        public decimal ThanhTien { get; set; }
        public string HinhThucThanhToan { get; set; } // "TienMat", "ChuyenKhoan"
    }

    public class DichVuItem
    {
        public int DichVuId { get; set; }
        public string TenDichVu { get; set; }
        public decimal Gia { get; set; }
    }
}
