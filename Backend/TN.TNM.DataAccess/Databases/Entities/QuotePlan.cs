using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class QuotePlan
    {
        public Guid PlanId { get; set; }
        public int Tt { get; set; }
        public string Finished { get; set; }
        public string ExecTime { get; set; }
        public string SumExecTime { get; set; }
        public Guid? QuoteId { get; set; }
        public Guid? TenantId { get; set; }
    }
}
