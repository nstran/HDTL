using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class QuoteScope
    {
        public Guid ScopeId { get; set; }
        public string Tt { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public Guid? QuoteId { get; set; }
        public Guid? TenantId { get; set; }
        public int? Level { get; set; }
        public Guid? ParentId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedById { get; set; }
    }
}
