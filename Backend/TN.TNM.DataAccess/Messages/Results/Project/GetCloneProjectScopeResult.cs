using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Note;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class GetCloneProjectScopeResult : BaseResult
    {
        public List<ProjectEntityModel> ListProject { get; set; }

        public List<Models.Project.ProjectScopeModel> ListProjectScope { get; set; }
    }
}
