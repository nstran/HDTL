using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Task;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.BusinessLogic.Messages.Requests.Task
{
    public class CreateConstraintTaskRequest : BaseRequest<CreateConstraintTaskParameter>
    {
        public TaskConstraintEntityModel TaskConstraint { get; set; }
        public override CreateConstraintTaskParameter ToParameter()
        {
            return new CreateConstraintTaskParameter
            {
                TaskConstraint = this.TaskConstraint,
                UserId = this.UserId
            };
        }
    }
}
