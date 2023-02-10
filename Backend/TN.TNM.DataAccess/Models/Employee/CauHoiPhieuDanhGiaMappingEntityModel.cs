using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class CauHoiPhieuDanhGiaMappingEntityModel
    {
        public int? CauHoiPhieuDanhGiaMappingId { get; set; }
        public int? PhieuDanhGiaId { get; set; }
        public int? CauHoiDanhGiaId { get; set; }
        public int? NguoiDanhGia { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public string NoiDungCauHoi { get; set; }
        public decimal? TiLe { get; set; }
        public int? LoaiCauTraLoiId { get; set; }
        public decimal? Stt { get; set; }
        public int? ParentId { get; set; }
        public List<CategoryEntityModel> DanhMucItem { get; set; }
        public CategoryEntityModel CauTraLoiModel { get; set; }
    }
}
