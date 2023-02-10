using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Models.Asset
{
    public class YeuCauCapPhatTaiSanEntityModel
    {
        public int YeuCauCapPhatTaiSanId { get; set; }
        public string MaYeuCau { get; set; }
        public decimal? SoLuong { get; set; }
        public string NguoiDeXuat { get; set; }
        public DateTime NgayDeXuat { get; set; }
        public Guid NguoiDeXuatId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public string PhongBan { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public int TrangThai { get; set; }
        public string TrangThaiString { get; set; }
        public Guid? NguoiGuiXacNhanId { get; set; }
        public List<YeuCauCapPhatTaiSanChiTietEntityModel> ListYeuCauCapPhatTaiSanChiTiet { get; set; }
    }
}
