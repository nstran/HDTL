using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class GetMasterMilestoneResult : BaseResult
    {
        public ProjectEntityModel Project { get; set; }
        public List<ProjectMilestoneEntityModel> ListMilestone { get; set; }
        public List<ProjectTaskEntityModel> ListTask { get; set; }
    }
}
