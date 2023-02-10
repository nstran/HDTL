using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LichSuCauHinhBaoHiem
    {
        public int LichSuCauHinhBaoHiemId { get; set; }
        public int CauHinhBaoHiemCapNhatId { get; set; }
        public bool MucDongCu { get; set; }
        public string MucDongToiDaCu { get; set; }
        public decimal TiLePhanBoMucDongBhxhcuaNldcu { get; set; }
        public decimal TiLePhanBoMucDongBhxhcuaNsdldcu { get; set; }
        public decimal TiLePhanBoMucDongBhytcuaNldcu { get; set; }
        public decimal TiLePhanBoMucDongBhytcuaNsdldcu { get; set; }
        public decimal TiLePhanBoMucDongBhtncuaNldcu { get; set; }
        public decimal TiLePhanBoMucDongBhtncuaNsdldcu { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
