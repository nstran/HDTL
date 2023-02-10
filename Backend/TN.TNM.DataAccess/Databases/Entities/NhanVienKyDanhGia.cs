using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class NhanVienKyDanhGia
    {
        public int NhanVienKyDanhGiaId { get; set; }
        public int? ParentId { get; set; }
        public int Level { get; set; }
        public int KyDanhGiaId { get; set; }
        public Guid NguoiDanhGiaId { get; set; }
        public Guid NguoiDuocDanhGiaId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid? PositionId { get; set; }
        public decimal QuyLuong { get; set; }
        public bool XemLuong { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public int? TrangThai { get; set; }
        public Guid? RootOrgId { get; set; }
    }
}
