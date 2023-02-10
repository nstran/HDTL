using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class UpdateProjectResult : BaseResult
    {
        public Guid ProjectId { get; set; }
    }
}
