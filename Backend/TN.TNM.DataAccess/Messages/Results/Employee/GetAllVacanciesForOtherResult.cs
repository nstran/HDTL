using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetAllVacanciesForOtherResult : BaseResult
    {
        public List<EmployeeVacanciesForOtherEntityModel> ListViTriTuyenDung { get; set; }
    }
}
