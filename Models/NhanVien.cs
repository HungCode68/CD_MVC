namespace NailMVC.Models
{
    public class NhanVien
    {
        public int Id { get; set; }
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string TrangThai { get; set; }
        public DateTime? NgayVaoLam { get; set; }
        public int TaiKhoanId { get; set; }
    }
}
