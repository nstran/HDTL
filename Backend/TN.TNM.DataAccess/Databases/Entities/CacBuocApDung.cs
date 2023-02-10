using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CacBuocApDung
    {
        public Guid Id { get; set; }
        public Guid ObjectId { get; set; }
        public int DoiTuongApDung { get; set; }
        public Guid QuyTrinhId { get; set; }
        public Guid CauHinhQuyTrinhId { get; set; }
        public Guid CacBuocQuyTrinhId { get; set; }
        public int Stt { get; set; }
        public int LoaiPheDuyet { get; set; }
        public int TrangThai { get; set; }
        public Guid? TenantId { get; set; }
        public int? ObjectNumber { get; set; }
    }
}
