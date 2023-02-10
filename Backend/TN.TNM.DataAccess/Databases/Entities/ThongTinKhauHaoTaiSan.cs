using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ThongTinKhauHaoTaiSan
    {
        public int ThongTinKhauHaoTaiSanId { get; set; }
        public int TaiSanId { get; set; }
        public decimal? GiaTriNguyenGia { get; set; }
        public decimal? GiaTriConLai { get; set; }
        public decimal? ThoiGianKhauHao { get; set; }
        public DateTime? ThoiDiemBatDauKhauHao { get; set; }
        public DateTime? ThoiDiemKetThucKhauHao { get; set; }
        public int? PhuongPhapKhauHao { get; set; }
        public decimal? TiLeKhauHaoTheoNamThang { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
