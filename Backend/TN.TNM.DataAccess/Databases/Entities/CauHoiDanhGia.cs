using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CauHoiDanhGia
    {
        public int CauHoiDanhGiaId { get; set; }
        public string NoiDungCauHoi { get; set; }
        public int? CauHoiMucDanhGiaMapId { get; set; }
        public int LoaiCauTraLoi { get; set; }
        public decimal? TrongSo { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
