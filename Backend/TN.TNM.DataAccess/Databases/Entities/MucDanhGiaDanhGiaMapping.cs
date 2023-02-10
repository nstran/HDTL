using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class MucDanhGiaDanhGiaMapping
    {
        public int MucDanhGiaDanhGiaMappingId { get; set; }
        public int? MucDanhGiaId { get; set; }
        public Guid? MucDanhGiaMasterDataId { get; set; }
        public decimal? DiemTu { get; set; }
        public decimal? DiemDen { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
