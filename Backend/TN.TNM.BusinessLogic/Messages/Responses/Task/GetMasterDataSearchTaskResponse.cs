using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Project;
using TN.TNM.DataAccess.Models.Quote;

namespace TN.TNM.BusinessLogic.Messages.Responses.Task
{
    public class GetMasterDataSearchTaskResponse : BaseResponse
    {
        public List<CategoryEntityModel> ListTaskType { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<EmployeeEntityModel> ListPersionInCharged { get; set; }
        public InforExportExcelModel InforExportExcel { get; set; }
        public ProjectEntityModel Project { get; set; }
        public decimal ProjectTaskComplete { get; set; }
        public decimal TotalEstimateHour { get; set; }
        public EmployeeEntityModel Employee { get; set; }
        public bool IsContainResource { get; set; }

        public List<ProjectEntityModel> listProject { get; set; }
    }
}
