using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.DataAccess.Messages.Parameters.Task
{
    public class DeleteProjectScopeParameter : BaseParameter
    {
        public Guid ProjectScopeId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ParentId { get; set; }
    }
}
