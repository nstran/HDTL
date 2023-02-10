using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Salary;

namespace TN.TNM.DataAccess.Messages.Results.Salary
{
    public class GetListPhieuLuongResult : BaseResult
    {
        public List<KyLuongModel> ListKyLuong { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<PhieuLuongModel> ListPhieuLuong { get; set; }
    }
}
