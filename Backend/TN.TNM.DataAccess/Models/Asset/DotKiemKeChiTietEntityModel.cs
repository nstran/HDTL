using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Asset
{
    public class DotKiemKeChiTietEntityModel
    {
        public int? DotKiemKeChiTietId { get; set; }
        public int? DotKiemKeId { get; set; }
        public int? TaiSanId { get; set; }
        public Guid? NguoiKiemKeId { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }

        public string NguoiKiemKeName { get; set; }
        public string MaTaiSan { get; set; }
        public string TenTaiSan { get; set; }
        public string KhuVucName { get; set; }
        public string TinhTrangName { get; set; }
        public decimal? GiaTriTinhKhauHao { get; set; }
        public decimal? KhauHaoLuyKe { get; set; }
        public decimal? GiaTriConLai { get; set; }
        public string  MoTaTaiSan { get; set; }


        public Guid? ProvincecId { get; set; }
        public Guid? PhanLoaiTaiSanId { get; set; }
        public int? HienTrangTaiSan { get; set; }
    }
}
