using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class UpdateRequiredConstrantRequest : BaseRequest<UpdateRequiredConstrantParameter>
    {
        public Guid TaskConstraintId { get; set; }
        public override UpdateRequiredConstrantParameter ToParameter()
        {
            return new UpdateRequiredConstrantParameter
            {
                TaskConstraintId = this.TaskConstraintId,
                UserId = this.UserId
            };
        }
    }
}
