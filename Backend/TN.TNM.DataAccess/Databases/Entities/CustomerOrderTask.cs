using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CustomerOrderTask
    {
        public Guid Id { get; set; }
        public Guid ServicePacketMappingOptionsId { get; set; }
        public Guid? VendorId { get; set; }
        public Guid? EmpId { get; set; }
        public string Note { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid TenantId { get; set; }
        public bool? IsExtend { get; set; }
        public Guid? OrderActionId { get; set; }
        public string Path { get; set; }
        public int? Stt { get; set; }
    }
}
