using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterDataCreateDeXuatChucVuResult:BaseResult
    {
        public List<EmployeeEntityModel> ListEmp { get; set; }
        public Guid LoginEmployeeID { get; set; }
        public List<PositionModel> ListPosition { get; set; }
    }
}
