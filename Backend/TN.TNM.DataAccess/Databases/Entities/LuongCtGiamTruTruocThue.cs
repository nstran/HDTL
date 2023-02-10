using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class LuongCtGiamTruTruocThue
    {
        public int LuongCtGiamTruTruocThueId { get; set; }
        public int KyLuongId { get; set; }
        public Guid EmployeeId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PositionId { get; set; }
        public decimal GiamTruCaNhan { get; set; }
        public decimal GiamTruNguoiPhuThuoc { get; set; }
        public int SoNguoiPhuThuoc { get; set; }
        public decimal TienDongBaoHiem { get; set; }
        public decimal GiamTruKhac { get; set; }
        public Guid? TenantId { get; set; }
    }
}
