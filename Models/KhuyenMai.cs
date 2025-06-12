namespace NailMVC.Models
{
    public class KhuyenMai
    {
        public int Id { get; set; }
        public string TenKhuyenMai { get; set; }
        public string MoTa { get; set; }
        public int PhanTramGiam { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
    }
}
