using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Project
{
    public class GetDataMilestoneByIdParameter : BaseParameter
    {
        public Guid ProjectMilestoneId { get; set; }
        public Guid ProjectId { get; set; }
    }
}
