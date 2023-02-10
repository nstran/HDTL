using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Task
{
    public class GetProjectScopeByProjectIdParameter : BaseParameter
    {
        public Guid ProjectId { get; set; }
    }
}
