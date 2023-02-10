using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class DeXuatXinNghiChiTiet
    {
        public int DeXuatXinNghiChiTietId { get; set; }
        public int DeXuatXinNghiId { get; set; }
        public DateTime Ngay { get; set; }
        public int LoaiCaLamViecId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
