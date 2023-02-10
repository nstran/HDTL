using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class TongHopChamCong
    {
        public int TongHopChamCongId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public decimal NgayLamViecThucTe { get; set; }
        public decimal CongTac { get; set; }
        public decimal DaoTaoHoiThao { get; set; }
        public decimal NghiPhep { get; set; }
        public decimal NghiLe { get; set; }
        public decimal NghiCheDo { get; set; }
        public decimal NghiHuongLuongKhac { get; set; }
        public decimal NghiBu { get; set; }
        public decimal NghiHuongBhxh { get; set; }
        public decimal NghiKhongPhep { get; set; }
        public decimal NghiKhongLuong { get; set; }
        public decimal TongNgayDmvs { get; set; }
        public decimal SoLanTruChuyenCan { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
