using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.BusinessLogic.Messages.Responses.Task
{
    public class SearchTaskResponse : BaseResponse
    {
        public List<TaskEntityModel> ListTask { get; set; }
        public int NumberTaskNew { get; set; }
        public int NumberTaskDL { get; set; }
        public int NumberTaskHT { get; set; }
        public int NumberTaskClose { get; set; }
        public int NumberTotalTask { get; set; }
        public decimal ProjectTaskComplete { get; set; }
        public decimal TotalEstimateHour { get; set; }
    }
}
