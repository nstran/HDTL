using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.DataAccess.Messages.Results.Task
{
    public class GetMasterDataCreateRelateTaskResult : BaseResult
    {
        public List<CategoryEntityModel> ListTaskType { get; set; }
        public List<TaskEntityModel> ListRelateTask { get; set; }
    }
}
