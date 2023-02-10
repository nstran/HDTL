using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CauHinhCaLamViec
    {
        public Int32 CaLamViecId { get; set; }
        
        public string TenCaLamViec { get; set; }
        public string GioVao { get; set; }
        public string GioRa { get; set; }
        public int? TrangThai { get; set; }      
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? TenantId { get; set; }
       
    }
}
