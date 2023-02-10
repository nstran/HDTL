using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.BusinessLogic.Messages.Responses.Task
{
    public class SearchTimeSheetResponse : BaseResponse
    {
        public List<TimeSheetEntityModel> ListTimeSheet { get; set; }
    }
}
