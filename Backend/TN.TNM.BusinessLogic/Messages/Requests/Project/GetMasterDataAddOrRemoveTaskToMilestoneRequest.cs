using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class GetMasterDataAddOrRemoveTaskToMilestoneRequest : BaseRequest<GetMasterDataAddOrRemoveTaskToMilestoneParameter>
    {
        public Guid ProjectMilestoneId { get; set; }
        public Guid ProjectId { get; set; }
        // 0 - Add; 1 - Remove
        public int Type { get; set; }
        public override GetMasterDataAddOrRemoveTaskToMilestoneParameter ToParameter()
        {
            return new GetMasterDataAddOrRemoveTaskToMilestoneParameter
            {
                ProjectMilestoneId = this.ProjectMilestoneId,
                ProjectId = this.ProjectId,
                Type = this.Type,
                UserId = this.UserId
            };
        }
    }
}
