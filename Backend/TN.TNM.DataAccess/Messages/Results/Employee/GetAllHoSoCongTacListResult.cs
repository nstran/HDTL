using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetAllHoSoCongTacListResult : BaseResult
    {
        public List<HoSoCongTacEntityModel> ListHoSoCongTac { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
    }
}
