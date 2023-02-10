using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class MucDanhGiaDanhGiaMappingEntityModel
    {
        public int? MucDanhGiaId { get; set; }
        public string TenMucDanhGia { get; set; }
        public decimal? DiemDanhGia { get; set; }
        public int? DieuKienDanhGia { get; set; }
        public decimal? DiemDanhGiaCuoi { get; set; }
        public decimal? ThangDiemDanhGia { get; set; }
        public DateTime? NgayApDung { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }


        public int? MucDanhGiaDanhGiaMappingId { get; set; }
        public int? DieuKien { get; set; }
        public Guid? MucDanhGiaMasterDataId { get; set; }
        public string MucDanhGiaMasterDataName { get; set; }
        public string mucDanhGiaMasterDataNameCustom { get; set; }
        public decimal? DiemTu { get; set; }
        public decimal? DiemDen { get; set; }

    }
}
