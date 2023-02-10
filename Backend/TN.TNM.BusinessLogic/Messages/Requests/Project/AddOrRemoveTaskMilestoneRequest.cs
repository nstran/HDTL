using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Project;

namespace TN.TNM.BusinessLogic.Messages.Requests.Project
{
    public class AddOrRemoveTaskMilestoneRequest : BaseRequest<AddOrRemoveTaskMilestoneParameter>
    {
        public List<Guid> ListTaskId { get; set; }
        public Guid ProjectMilestoneId { get; set; }
        public int Type { get; set; }
        public override AddOrRemoveTaskMilestoneParameter ToParameter()
        {
            return new AddOrRemoveTaskMilestoneParameter
            {
                ListTaskId = this.ListTaskId,
                ProjectMilestoneId = this.ProjectMilestoneId,
                Type = this.Type,
                UserId = this.UserId
            };
        }
    }
}
