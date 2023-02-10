using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class CreateProjectResult: BaseResult
    {
        public Guid ProjectId { get; set; }
    }
}
