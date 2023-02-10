using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CapPhatTaiSan
    {
        public int CapPhatTaiSanId { get; set; }
        public int TaiSanId { get; set; }
        public Guid NguoiSuDungId { get; set; }
        public Guid MucDichSuDungId { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public Guid NguoiCapPhatId { get; set; }
        public string LyDo { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public bool? TrangThai { get; set; }
        public int? LoaiCapPhat { get; set; }
        public int? YeuCauCapPhatTaiSanChiTietId { get; set; }
    }
}
