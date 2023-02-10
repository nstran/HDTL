using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class ChangeStatusTaskRequest : BaseRequest<ChangeStatusTaskParameter>
    {
        public Guid TaskId { get; set; }
        public Guid StatusId { get; set; }
        public int Type { get; set; }
        public override ChangeStatusTaskParameter ToParameter()
        {
            return new ChangeStatusTaskParameter
            {
                TaskId = this.TaskId,
                StatusId = this.StatusId,
                Type = this.Type,
                UserId = this.UserId
            };
        }
    }
}
