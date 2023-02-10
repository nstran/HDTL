using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class DeleteTaskConstraintRequest : BaseRequest<DeleteTaskConstraintParameter>
    {
        public Guid TaskConstraintId { get; set; }
        public override DeleteTaskConstraintParameter ToParameter()
        {
            return new DeleteTaskConstraintParameter
            {
                TaskConstraintId = this.TaskConstraintId,
                UserId = this.UserId
            };
        }
    }
}
