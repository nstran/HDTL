using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class GetMasterDataProjectMilestoneRequest : BaseRequest<GetMasterDataProjectMilestoneParameter>
    {
        public Guid ProjectId { get; set; }
        public override GetMasterDataProjectMilestoneParameter ToParameter()
        {
            return new GetMasterDataProjectMilestoneParameter
            {
                ProjectId = this.ProjectId,
                UserId = this.UserId
            };
        }
    }
}
