using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class UpdateProjectMilestoneRequest : BaseRequest<UpdateProjectMilestoneParameter>
    {       
        public ProjectMilestone ProjectMilestone { get; set; }
        public override UpdateProjectMilestoneParameter ToParameter()
        {
            return new UpdateProjectMilestoneParameter()   {                
                UserId = UserId,
                //ProjectMilestone = ProjectMilestone
            };
        }
    }
}
