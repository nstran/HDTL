using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.DataAccess.Messages.Results.Task
{
    public class GetProjectScopeByProjectIdResult : BaseResult
    {
        public List<ProjectScopeModel> ListProjectScrope { get; set; }
    }
}
