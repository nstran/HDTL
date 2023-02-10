using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Task
{
    public class DeleteRelateTaskParameter : BaseParameter
    {
        public Guid RelateTaskMappingId { get; set; }

    }
}
