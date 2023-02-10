using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class GetMasterDataCreateOrUpdateMilestoneRequest : BaseRequest<GetMasterDataCreateOrUpdateMilestoneParameter>
    {
        public Guid ProjectId { get; set; }
        public Guid ProjectMilestoneId { get; set; }
        public override GetMasterDataCreateOrUpdateMilestoneParameter ToParameter()
        {
            return new GetMasterDataCreateOrUpdateMilestoneParameter
            {
                ProjectId = this.ProjectId,
                ProjectMilestoneId = this.ProjectMilestoneId,
                UserId = this.UserId
            };
        }
    }
}
