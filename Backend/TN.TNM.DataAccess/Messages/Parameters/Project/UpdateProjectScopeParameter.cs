using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class UpdateProjectScopeParameter : BaseParameter
    {       
       // public List<ProjectScopeModel> ListProjectScope { get; set; }
        public ProjectScopeModel ProjectScope { get; set; }
        public Guid ParentId { get; set; }
    }

}
