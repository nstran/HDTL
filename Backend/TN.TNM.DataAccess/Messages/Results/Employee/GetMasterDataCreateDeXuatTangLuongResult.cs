using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterDataCreateDeXuatTangLuongResult:BaseResult
    {
        public List<EmployeeEntityModel> ListEmp { get; set; }
        public Guid LoginEmployeeID { get; set; }

    }
}
