using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TaiSanYeuCauCapPhat
    {
        public int TaiSanYeuCauCapPhatId { get; set; }
        public int TaiSanId { get; set; }
        public int YeuCauCapPhatTaiSanId { get; set; }
        public string LyDo { get; set; }
        public string MoTa { get; set; }
        public DateTime? SuDungTu { get; set; }
        public DateTime? SuDungDen { get; set; }
        public decimal? SoLuong { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
