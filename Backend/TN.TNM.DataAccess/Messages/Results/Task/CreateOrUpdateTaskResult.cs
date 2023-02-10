using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Task
{
    public class CreateOrUpdateTaskResult : BaseResult
    {
        public Guid? TaskId { get; set; }
    }
}
