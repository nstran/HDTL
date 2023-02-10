using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Task
{
    public class CreateRelateTaskResult : BaseResult
    {
        public RelateTaskMappingEntityModel TaskRelate { get; set; }
    }
}
