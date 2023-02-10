using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Salary
{
    public class GetMasterThongKeChamCongResult : BaseResult
    {
        public List<TrangThaiGeneral> ListKyHieuChamCong { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
    }
}
