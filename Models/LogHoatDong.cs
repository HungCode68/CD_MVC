namespace NailMVC.Models
{
    public class LogHoatDong
    {
        public int Id { get; set; }
        public int TaiKhoanId { get; set; }
        public string HoatDong { get; set; }
        public DateTime ThoiGian { get; set; }
        public string? DiaChiIP { get; set; }
        public string? TrinhDuyet { get; set; }
        public string? DuLieuThem { get; set; }
    }
}
