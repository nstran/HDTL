using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.DataAccess.Messages.Parameters.Task
{
    public class CreateConstraintTaskParameter : BaseParameter
    {
        public TaskConstraintEntityModel TaskConstraint { get; set; }
    }
}
