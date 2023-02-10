using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class KyDanhGiaEntityModel
    {
        public int? KyDanhGiaId { get; set; }
        public string MaKyDanhGia { get; set; }
        public string TenKyDanhGia { get; set; }
        public string NguoiTaoName { get; set; }
        public DateTime? ThoiGianBatDau { get; set; }
        public DateTime? ThoiGianKetThuc { get; set; }
        public string LyDo { get; set; }
        public int? TrangThaiDanhGia { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
