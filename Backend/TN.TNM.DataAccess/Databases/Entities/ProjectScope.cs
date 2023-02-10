using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class ProjectScope
    {
        public Guid ProjectId { get; set; }
        public string Description { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? ParentId { get; set; }
        public Guid? ResourceType { get; set; }
        public Guid? VendorId { get; set; }
        public Guid ProjectScopeId { get; set; }
        public string ProjectScopeCode { get; set; }
        public string ProjectScopeName { get; set; }
        public int? Level { get; set; }
    }
}
