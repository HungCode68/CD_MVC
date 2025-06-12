namespace NailMVC.Models
{
    public class ChamCong
    {
        public int Id { get; set; }
        public int NhanVienId { get; set; }
        public DateTime Ngay { get; set; }
        public TimeSpan? GioVao { get; set; }
        public TimeSpan? GioRa { get; set; }
        public string? HinhAnh { get; set; } // Đường dẫn ảnh
        public string? GhiChu { get; set; }

    }
}
