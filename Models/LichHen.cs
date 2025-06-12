namespace NailMVC.Models
{
    public class LichHen
    {
        public int Id { get; set; }
        public DateTime? NgayHen { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; } // "DaDat", "DaHuy", "HoanThanh"
        public int KhachHangId { get; set; }
        public int NhanVienId { get; set; }
        public int DichVuId { get; set; }
    }
}
