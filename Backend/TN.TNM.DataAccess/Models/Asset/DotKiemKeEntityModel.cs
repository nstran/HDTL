using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Asset
{
    public  class DotKiemKeEntityModel
    {
        public int? DotKiemKeId { get; set; }
        public string TenDoiKiemKe { get; set; }
        public int SoLuongTaiSan { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public int? TrangThaiId { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
