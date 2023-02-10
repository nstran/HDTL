using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProjectContractMapping
    {
        public Guid ProjectContractId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ContractId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
