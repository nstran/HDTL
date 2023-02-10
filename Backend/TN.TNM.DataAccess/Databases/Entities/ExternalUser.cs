using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ExternalUser
    {
        public Guid ExternalUserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid? ObjectId { get; set; }
        public Guid? ObjectType { get; set; }
        public bool Disabled { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Active { get; set; }
        public DateTime? ResetCodeDate { get; set; }
        public string ResetCode { get; set; }
        public Guid? TenantId { get; set; }
        public Guid EmployeeId { get; set; }
    }
}
