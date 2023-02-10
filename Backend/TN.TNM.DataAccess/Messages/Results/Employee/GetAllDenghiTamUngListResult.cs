using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetAllDenghiTamUngListResult : BaseResult
    {
        public List<DeNghiTamHoanUngEntityModel> ListDeNghiTamHoanUng { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
    }
}
