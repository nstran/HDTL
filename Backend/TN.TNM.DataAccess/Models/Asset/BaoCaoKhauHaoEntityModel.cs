using System;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Models.Asset
{
    public class BaoCaoKhauHaoEntityModel
    {
        public int TaiSanId { get; set; }
        public string MaTaiSan { get; set; }
        public string TenTaiSan { get; set; }       
        public Guid? PhanLoaiTaiSanId { get; set; }     
        public string LoaiTaiSanStr { get; set; }
        public int HienTrangTaiSan { get; set; }
        public string HienTrangTaiSanStr { get; set; }
        public DateTime? NgayVaoSo { get; set; }        
        public decimal GiaTriNguyenGia { get; set; }       
        public decimal GiaTriTinhKhauHao { get; set; }
        public int TiLeKhauHaoTheoThang { get; set; }
        public int TiLeKhauHaoTheoNam { get; set; }
        public decimal GiaTriKhauHaoTheoThang { get; set; }
        public decimal GiaTriKhauHaoTheoNam { get; set; }
        public DateTime? ThoiGianKhauHaoDen { get; set; }
        public decimal GiaTriKhauHaoLuyKe { get; set; }
        public decimal GiaTriConLai { get; set; }
        public int ThoiGianKhauHao { get; set; }
        public int PhuongPhapTinhKhauHao { get; set; }
        public DateTime? ThoiDiemBdtinhKhauHao { get; set; }
        public DateTime? ThoiDiemKTKhauHao { get; set; }



    }
}
