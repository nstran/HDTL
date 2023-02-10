using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class GetMasterMilestoneRequest : BaseRequest<GetMasterMilestoneParameter>
    {
        public Guid ProjectId { get; set; }
        public override GetMasterMilestoneParameter ToParameter()
        {
            return new GetMasterMilestoneParameter()
            {
                ProjectId = this.ProjectId,
                UserId = this.UserId
            };
        }
    }
}
