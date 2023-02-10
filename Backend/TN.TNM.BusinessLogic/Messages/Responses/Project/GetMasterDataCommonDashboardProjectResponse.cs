using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.BusinessLogic.Messages.Responses.Project
{
    public class GetMasterDataCommonDashboardProjectResponse : BaseResponse
    {
        public List<TaskEntityModel> ListTaskOverdue { get; set; }
        public List<TaskEntityModel> ListTaskComplete { get; set; }
        public ProjectEntityModel Project { get; set; }
        public List<ProjectEntityModel> ListProject { get; set; }

        public List<EmployeeEntityModel> ListEmployee { get; set; }

        public List<TaskEntityModel> ListAllTask { get; set; }
        public List<ChartBudget> ListChartBudget { get; set; }

        public bool IsManager { get; set; }
        public decimal ProjectTaskComplete { get; set; }
        public decimal TotalEstimateHour { get; set; }
        public decimal TotalHourUsed { get; set; }
        
        public decimal TotalEE { get; set; }
    }
}
