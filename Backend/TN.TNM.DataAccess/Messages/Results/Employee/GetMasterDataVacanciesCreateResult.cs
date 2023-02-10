using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterDataVacanciesCreateResult : BaseResult
    {
        public List<EmployeeRecruitmentEntityModel> ListEmployeeRecruit { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<CategoryEntityModel> ListLoaiCV { get; set; }
        public List<CategoryEntityModel> ListKinhNghiem { get; set; }
    }
}
