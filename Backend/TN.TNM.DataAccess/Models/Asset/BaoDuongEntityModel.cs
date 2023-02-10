using System;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Models.Asset
{
    public class BaoDuongEntityModel
    {
        public int TaiSanId { get; set; }
        public int BaoDuongTaiSanId { get; set; }
        public Guid NguoiPhuTrachId { get; set; }
        public string NguoiPhuTrach { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string MoTa { get; set; }
        public bool? TrangThai { get; set; }       
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }      
    }
}
