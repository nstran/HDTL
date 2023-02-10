using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CacBuocQuyTrinh
    {
        public Guid Id { get; set; }
        public int Stt { get; set; }
        public int LoaiPheDuyet { get; set; }
        public Guid CauHinhQuyTrinhId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
