using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class DeleteTaskRequest : BaseRequest<DeleteTaskParameter>
    {
        public Guid TaskId { get; set; }
        public override DeleteTaskParameter ToParameter()
        {
            return new DeleteTaskParameter
            {
                TaskId = this.TaskId,
                UserId = this.UserId
            };
        }
    }
}
