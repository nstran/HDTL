using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.DataAccess.Messages.Results.Task
{
    public class GetAllTaskByProjectScopeIdResult : BaseResult
    {
        public List<TaskEntityModel> ListTask { get; set; }
    }
}
