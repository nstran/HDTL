using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CauTraLoi
    {
        public int CauTraLoiId { get; set; }
        public int CauHoiDanhGiaId { get; set; }
        public string NoiDungTraLoi { get; set; }
        public bool? LuaChonDs { get; set; }
        public decimal? TrongSo { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
    }
}
