using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.DataAccess.Models.Project
{
    public class ProjectScopeModel
    {
        public Guid ProjectScopeId { get; set; }
        public string ProjectScopeCode { get; set; }
        public string ProjectScopeName { get; set; }
        public string Description { get; set; }
        public Guid? ResourceType { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? ParentId { get; set; }
        public DateTime CreateDate { get; set; }
        public Guid CreateBy { get; set; }
        public List<ProjectScopeModel> ScopeChildList { get; set; }
        public List<string> ListTask { get; set; }
        public List<string> ListEmployee { get; set; }
        public int? Level { get; set; }
        public string StyleClass { get; set; }
    }
}
