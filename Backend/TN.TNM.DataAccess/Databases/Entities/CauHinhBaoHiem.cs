using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CauHinhBaoHiem
    {
        public int CauHinhBaoHiemId { get; set; }
        public int? LoaiDong { get; set; }
        public decimal MucDong { get; set; }
        public decimal MucDongToiDa { get; set; }
        public decimal TiLePhanBoMucDongBhxhcuaNld { get; set; }
        public decimal TiLePhanBoMucDongBhxhcuaNsdld { get; set; }
        public decimal TiLePhanBoMucDongBhytcuaNld { get; set; }
        public decimal TiLePhanBoMucDongBhytcuaNsdld { get; set; }
        public decimal TiLePhanBoMucDongBhtncuaNld { get; set; }
        public decimal TiLePhanBoMucDongBhtncuaNsdld { get; set; }
        public decimal TiLePhanBoMucDongBhtnnncuaNld { get; set; }
        public decimal TiLePhanBoMucDongBhtnnncuaNsdld { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public decimal MucLuongCoSo { get; set; }
    }
}
