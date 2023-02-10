using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetAllDeXuatCongTacResult : BaseResult
    {
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<DeXuatCongTacEntityModel> ListDeXuatCongTac { get; set; }
    }
}
