using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class HoSoCongTac
    {
        public int HoSoCongTacId { get; set; }
        public string MaHoSoCongTac { get; set; }
        public int DeXuatCongTacId { get; set; }
        public int? TrangThai { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
        public string KetQua { get; set; }
    }
}
