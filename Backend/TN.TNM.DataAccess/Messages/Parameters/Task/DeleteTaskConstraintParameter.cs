using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Task
{
    public class DeleteTaskConstraintParameter : BaseParameter
    {
        public Guid TaskConstraintId { get; set; }
    }
}
