using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.DataAccess.Messages.Results.Task
{
    public class GetMasterDataCreateConstraintResult : BaseResult
    {
        public List<CategoryEntityModel> ListConstraint { get; set; }
        public List<TaskEntityModel> ListTask { get; set; }
    }
}
