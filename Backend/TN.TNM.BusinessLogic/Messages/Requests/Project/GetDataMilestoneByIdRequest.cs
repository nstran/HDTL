using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class GetDataMilestoneByIdRequest : BaseRequest<GetDataMilestoneByIdParameter>
    {
        public Guid ProjectMilestoneId { get; set; }
        public Guid ProjectId { get; set; }
        public override GetDataMilestoneByIdParameter ToParameter()
        {
            return new GetDataMilestoneByIdParameter
            {
                ProjectMilestoneId = this.ProjectMilestoneId,
                ProjectId = this.ProjectId,
                UserId = this.UserId
            };
        }
    }
}
