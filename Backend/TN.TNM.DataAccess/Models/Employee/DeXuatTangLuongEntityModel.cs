using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class DeXuatTangLuongEntityModel
    {
        public int? DeXuatTangLuongId { get; set; }
        public string TenDeXuat { get; set; }
        public decimal? TongMucDeXuat { get; set; }
        public short LoaiDeXuat { get; set; }
        public DateTime NgayDeXuat { get; set; }
        public string NguoiDeXuatName { get; set; }
        public DateTime? NgayApDung { get; set; }
        public Guid NguoiDeXuatId { get; set; }
        public Guid? PhongBanId { get; set; }
        public Guid? ChucVuId { get; set; }
        public string GhiChu { get; set; }
        public short? TrangThai { get; set; }
        public bool? Active { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
