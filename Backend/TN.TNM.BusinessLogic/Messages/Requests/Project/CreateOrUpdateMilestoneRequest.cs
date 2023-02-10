using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class CreateOrUpdateMilestoneRequest : BaseRequest<CreateOrUpdateMilestoneParameter>
    {
        public ProjectMilestoneEntityModel ProjectMilestone { get; set; }
        public override CreateOrUpdateMilestoneParameter ToParameter()
        {
            return new CreateOrUpdateMilestoneParameter
            {
                ProjectMilestone = this.ProjectMilestone,
                UserId = this.UserId
            };
        }
    }
}
