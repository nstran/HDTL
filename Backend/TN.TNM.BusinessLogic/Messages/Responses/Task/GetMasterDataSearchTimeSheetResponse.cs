using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Responses.Task
{
    public class GetMasterDataSearchTimeSheetResponse : BaseResponse
    {
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<CategoryEntityModel> ListTimeStyle { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }

        public InforExportExcelModel InforExportExcel { get; set; }
        public ProjectEntityModel Project { get; set; }
        public decimal ProjectTaskComplete { get; set; }
        public decimal TotalEstimateHour { get; set; }
        public decimal TotalHourUsed { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public bool IsRoot { get; set; }

        public List<ProjectEntityModel> ListProject { get; set; }
    }
}
