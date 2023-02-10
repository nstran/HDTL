using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class GetMasterDataCreateOrUpdateMilestoneResponse : BaseResponse
    {
        public List<ProjectEntityModel> ListProject { get; set; }
        public ProjectMilestoneEntityModel ProjectMilestone { get; set; }
    }
}
