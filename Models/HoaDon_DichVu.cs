namespace NailMVC.Models
{
    public class HoaDon_DichVu
    {
        public int Id { get; set; }
        public int HoaDonId { get; set; }
        public int DichVuId { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        public DichVu DichVu { get; set; }
    }
}
