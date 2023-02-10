using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class UpdateStatusProjectMilestoneRequest : BaseRequest<UpdateStatusProjectMilestoneParameter>
    {
        public Guid ProjectMilestoneId { get; set; }

        // 0: Đóng ; 1: Mở lại
        public int Type { get; set; }
        public override UpdateStatusProjectMilestoneParameter ToParameter()
        {
            return new UpdateStatusProjectMilestoneParameter
            {
                ProjectMilestoneId = this.ProjectMilestoneId,
                Type = this.Type,
                UserId = this.UserId
            };
        }
    }
}
