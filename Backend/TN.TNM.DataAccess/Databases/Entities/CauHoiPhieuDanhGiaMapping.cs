using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CauHoiPhieuDanhGiaMapping
    {
        public int CauHoiPhieuDanhGiaMappingId { get; set; }
        public int PhieuDanhGiaId { get; set; }
        public int CauHoiDanhGiaId { get; set; }
        public int? NguoiDanhGia { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public string NoiDungCauHoi { get; set; }
        public decimal? TiLe { get; set; }
        public int? LoaiCauTraLoiId { get; set; }
        public int? ParentId { get; set; }
        public decimal? Stt { get; set; }
    }
}
