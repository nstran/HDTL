using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CauHinhQuyTrinh
    {
        public Guid Id { get; set; }
        public decimal SoTienTu { get; set; }
        public string TenCauHinh { get; set; }
        public Guid? ServicePacketId { get; set; }
        public string QuyTrinh { get; set; }
        public Guid QuyTrinhId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
