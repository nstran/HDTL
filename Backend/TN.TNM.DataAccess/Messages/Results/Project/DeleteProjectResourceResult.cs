using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Task
{
    public class DeleteProjectResourceResult : BaseResult
    {
        public Guid ProjectResourceId { get; set; }
    }
}
