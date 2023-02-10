using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.BusinessLogic.Messages.Responses.Task
{
    public class GetMasterDataTimeSheetResponse : BaseResponse
    {
        public List<TaskEntityModel> ListTask { get; set; }
        public List<CategoryEntityModel> ListTimeType { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
        public EmployeeEntityModel Employee { get; set; }
        public ProjectEntityModel Project { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }


        /*Chi tiết*/
        public List<TimeSheetEntityModel> ListTimeSheet { get; set; }
    }
}
