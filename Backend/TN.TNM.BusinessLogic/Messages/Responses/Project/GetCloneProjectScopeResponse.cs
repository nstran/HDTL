using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Project;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class GetCloneProjectScopeResponse : BaseResponse
    {      
        public List<ProjectEntityModel> ListProject { get; set; }

        public List<Models.Project.ProjectScopeModel> ListProjectScope { get; set; }

    }
}
