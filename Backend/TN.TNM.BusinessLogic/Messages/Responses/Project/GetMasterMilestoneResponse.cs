using System;
using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Project;


namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class GetMasterMilestoneResponse : BaseResponse
    {      
        public ProjectModel Project { get; set; }
        public List<ProjectMilestoneModel> ListMilestone { get; set; }

    }
}
