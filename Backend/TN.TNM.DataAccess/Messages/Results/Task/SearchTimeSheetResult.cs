using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.DataAccess.Messages.Results.Task
{
    public class SearchTimeSheetResult : BaseResult
    {
        public List<TimeSheetEntityModel> ListTimeSheet { get; set; }
    }
}
